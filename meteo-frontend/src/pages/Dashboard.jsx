import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  Legend,
  CartesianGrid,
} from "recharts";

import "../styles/dashboard.css";
import { getMeteoSeries } from "../api/meteo";
import { getPollutionSeries } from "../api/pollution";
import { getCities } from "../api/cities";
import { getAlerts } from "../api/alerts";

function formatShortDate(date) {
  return new Intl.DateTimeFormat("en-GB", {
    day: "2-digit",
    month: "short",
    year: "2-digit",
  })
    .format(date)
    .replace(/ /g, "-");
}

function formatNotificationTime(value) {
  if (!value) return "--:--";

  const date = value instanceof Date ? value : new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "--:--";
  }

  return new Intl.DateTimeFormat("sr-RS", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
}

export default function Dashboard() {
  const navigate = useNavigate();

  const user = useMemo(() => {
    try {
      return JSON.parse(localStorage.getItem("user") || "null");
    } catch {
      return null;
    }
  }, []);

  const todayLabel = useMemo(() => formatShortDate(new Date()), []);

  const [cities, setCities] = useState([]);
  const [selectedCity, setSelectedCity] = useState(user?.city || "Subotica");
  const [meteoPoints, setMeteoPoints] = useState([]);
  const [pollutionPoints, setPollutionPoints] = useState([]);
  const [error, setError] = useState("");
  const [activeGroup, setActiveGroup] = useState("meteo");
  const [activeMetric, setActiveMetric] = useState("temp");

  const [isNotifOpen, setIsNotifOpen] = useState(false);
  const [notifications, setNotifications] = useState([]);

  const unreadCount = notifications.filter((n) => n.unread).length;

  function logout() {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("user");
    navigate("/");
  }

  useEffect(() => {
    let cancelled = false;

    async function loadCities() {
      try {
        const list = await getCities();
        if (cancelled) return;

        setCities(list);

        const userCity = user?.city?.trim();
        const found =
          userCity &&
          list.find((c) => c.toLowerCase() === userCity.toLowerCase());

        if (found) setSelectedCity(found);
        else if (list.length > 0) setSelectedCity(list[0]);
      } catch (e) {
        console.error(e);

        const fallback = ["Subotica", "NoviSad", "Beograd", "Nis", "Uzice"];
        if (cancelled) return;

        setCities(fallback);

        const userCity = user?.city?.trim();
        const found =
          userCity &&
          fallback.find((c) => c.toLowerCase() === userCity.toLowerCase());

        setSelectedCity(found || "Subotica");
      }
    }

    loadCities();

    return () => {
      cancelled = true;
    };
  }, [user]);

  async function load() {
    try {
      setError("");

      if (activeGroup === "meteo") {
        const data = await getMeteoSeries(selectedCity);
        const points = Array.isArray(data) ? data : data?.points || [];
        setMeteoPoints(points);
      } else {
        const data = await getPollutionSeries(selectedCity);
        const points = Array.isArray(data) ? data : data?.points || [];
        setPollutionPoints(points);
      }
    } catch (e) {
      console.error(e);
      setError(
        "Ne mogu da učitam merenja. Proveri da li backend radi i da li si ulogovana."
      );
    }
  }

  async function loadAlerts() {
    try {
      const data = await getAlerts(selectedCity);
      const items = Array.isArray(data) ? data : data?.items || [];

      if (!items.length) return;

      const mapped = items.map((a, i) => ({
        id: `${selectedCity}-${a.title || "alert"}-${a.message || i}`,
        city: selectedCity,
        title: a.title || "Obaveštenje",
        message: a.message || "",
        level: a.level || "info",
        receivedAt: a.simTimeLabel || formatNotificationTime(a.createdAtUtc),
        unread: !isNotifOpen,
      }));

      setNotifications((prev) => {
        const existingIds = new Set(prev.map((n) => n.id));
        const fresh = mapped.filter((n) => !existingIds.has(n.id));

        return [...fresh, ...prev];
      });
    } catch (e) {
      console.error("Alerts error:", e);
    }
  }

  useEffect(() => {
    if (isNotifOpen) {
      setNotifications((prev) =>
        prev.map((n) => ({
          ...n,
          unread: false,
        }))
      );
    }
  }, [isNotifOpen]);

  useEffect(() => {
    if (!selectedCity) return;

    load();
    loadAlerts();

    const id = setInterval(() => {
      load();
      loadAlerts();
    }, 5000);

    return () => clearInterval(id);
  }, [selectedCity, activeGroup]);

  useEffect(() => {
    setNotifications([]);
    setIsNotifOpen(false);
  }, [selectedCity]);

  function switchToMeteo() {
    setActiveGroup("meteo");
    setActiveMetric("temp");
  }

  function switchToPollution() {
    setActiveGroup("pollution");
    setActiveMetric("pm25");
  }

  function removeNotification(id) {
    setNotifications((prev) => prev.filter((n) => n.id !== id));
  }

  const chartTitle =
    activeGroup === "meteo"
      ? activeMetric === "temp"
        ? "Temperatura (°C)"
        : activeMetric === "hum"
        ? "Vlažnost (%)"
        : "Pritisak (hPa)"
      : activeMetric === "pm25"
      ? "PM2.5"
      : activeMetric === "pm10"
      ? "PM10"
      : "O₃";

  return (
    <div className="dashBg">
      <div className="dashContainer">
        <div className="dashHeaderCard">
          <div className="dashUser">
            <div className="dashTitle">
              {user?.firstName || ""} {user?.lastName || ""}
            </div>
            <div className="dashSub">
              {user?.email || "Nepoznat korisnik"} • Lokacija: <b>{selectedCity}</b>
            </div>
            <div className="dashHint">
              Datum: <b>{todayLabel}</b>
            </div>
          </div>

          <div className="dashActions">
            <button
              className="notifBell"
              type="button"
              onClick={() => setIsNotifOpen((v) => !v)}
              aria-label="Obaveštenja"
              title="Obaveštenja"
            >
              🔔
              {unreadCount > 0 ? (
                <span className="notifBadge">{unreadCount}</span>
              ) : null}
            </button>

            <div className="dashField">
              <select
                value={selectedCity}
                onChange={(e) => setSelectedCity(e.target.value)}
                disabled={cities.length === 0}
              >
                {cities.map((c) => (
                  <option key={c} value={c}>
                    {c}
                  </option>
                ))}
              </select>
            </div>

            <button
              className="ghostBtn logoutBtn"
              type="button"
              onClick={logout}
            >
              Logout
            </button>
          </div>
        </div>

        <div className="dashTabs">
          <button
            type="button"
            className={activeGroup === "meteo" ? "tabBtn active" : "tabBtn"}
            onClick={switchToMeteo}
          >
            Meteo stanice
          </button>

          <button
            type="button"
            className={activeGroup === "pollution" ? "tabBtn active" : "tabBtn"}
            onClick={switchToPollution}
          >
            Kvalitet vazduha
          </button>
        </div>

        <div className="dashChips">
          {activeGroup === "meteo" ? (
            <>
              <button
                type="button"
                className={activeMetric === "temp" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("temp")}
              >
                Temperatura
              </button>
              <button
                type="button"
                className={activeMetric === "hum" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("hum")}
              >
                Vlažnost
              </button>
              <button
                type="button"
                className={activeMetric === "press" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("press")}
              >
                Pritisak
              </button>
            </>
          ) : (
            <>
              <button
                type="button"
                className={activeMetric === "pm25" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("pm25")}
              >
                PM2.5
              </button>
              <button
                type="button"
                className={activeMetric === "pm10" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("pm10")}
              >
                PM10
              </button>
              <button
                type="button"
                className={activeMetric === "o3" ? "chip active" : "chip"}
                onClick={() => setActiveMetric("o3")}
              >
                O₃
              </button>
            </>
          )}
        </div>

        {error ? <div className="dashError">{error}</div> : null}

        <div className="chartCard">
          <div className="chartHeader">
            <div className="chartTitle">
              {selectedCity} — {chartTitle}
            </div>
            <div className="chartNote">Avg stanice • real-time</div>
          </div>

          <div className="chartBody">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={activeGroup === "meteo" ? meteoPoints : pollutionPoints}
                margin={{ top: 10, right: 20, left: 0, bottom: 0 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="label" />
                <YAxis />
                <Tooltip />
                <Legend />

                {activeGroup === "meteo" && activeMetric === "temp" && (
                  <Line
                    type="monotone"
                    dataKey="tempAvg"
                    name="Temp (°C)"
                    dot={false}
                  />
                )}
                {activeGroup === "meteo" && activeMetric === "hum" && (
                  <Line
                    type="monotone"
                    dataKey="humAvg"
                    name="Vlaga (%)"
                    dot={false}
                  />
                )}
                {activeGroup === "meteo" && activeMetric === "press" && (
                  <Line
                    type="monotone"
                    dataKey="pressAvg"
                    name="Pritisak (hPa)"
                    dot={false}
                  />
                )}

                {activeGroup === "pollution" && activeMetric === "pm25" && (
                  <Line
                    type="monotone"
                    dataKey="pm25Avg"
                    name="PM2.5"
                    dot={false}
                  />
                )}
                {activeGroup === "pollution" && activeMetric === "pm10" && (
                  <Line
                    type="monotone"
                    dataKey="pm10Avg"
                    name="PM10"
                    dot={false}
                  />
                )}
                {activeGroup === "pollution" && activeMetric === "o3" && (
                  <Line
                    type="monotone"
                    dataKey="o3Avg"
                    name="O₃"
                    dot={false}
                  />
                )}
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      <aside className={isNotifOpen ? "notifPanel open" : "notifPanel"}>
        <div className="notifPanelHeader">
          <div className="notifPanelTitle">Obaveštenja</div>

          <div className="notifPanelActions">
            <button
              type="button"
              className="notifCloseBtn"
              onClick={() => setIsNotifOpen(false)}
            >
              ✕
            </button>
          </div>
        </div>

        <div className="notifPanelBody">
          {notifications.length === 0 ? (
            <div className="notifEmpty">Nema novih notifikacija.</div>
          ) : (
            notifications.map((n) => (
              <div key={n.id} className={`notifItem notifItem--${n.level || "info"}`}>
                <div className="notifItemTopRow">
                  <div className="notifItemTime">{n.receivedAt}</div>
                  <button
                    type="button"
                    className="notifItemClose"
                    onClick={() => removeNotification(n.id)}
                    aria-label="Obriši notifikaciju"
                    title="Obriši"
                  >
                    ✕
                  </button>
                </div>

                <div className="notifItemTitle">{n.title}</div>
                <div className="notifItemMessage">{n.message}</div>
              </div>
            ))
          )}
        </div>
      </aside>
    </div>
  );
}