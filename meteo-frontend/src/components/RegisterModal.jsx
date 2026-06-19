import { useState } from "react";
import { register as registerApi } from "../api/auth";

const CITY_OPTIONS = ["Subotica", "Novi Sad", "Beograd", "Niš", "Kragujevac"];

export default function RegisterModal({ open, onClose, onSuccess }) {
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    city: CITY_OPTIONS[0],
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  if (!open) return null;

  function setField(name, value) {
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function submit(e) {
    e.preventDefault();
    setError("");

    const emailNorm = form.email.trim().toLowerCase();
    const passNorm = form.password.trim();

    if (!form.firstName.trim() || !form.lastName.trim()) {
      setError("Unesi ime i prezime.");
      return;
    }

    if (!emailNorm || !emailNorm.includes("@")) {
      setError("Unesi validan email.");
      return;
    }

    if (!passNorm) {
      setError("Unesi šifru."); // ✅ nema više min 6
      return;
    }

    setLoading(true);
    try {
      const data = await registerApi({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: emailNorm,
        password: passNorm,
        city: form.city,
      });

      onSuccess(data); // { user, accessToken }
    } catch (err) {
      const msg =
        err?.response?.data?.toString?.() ||
        err?.response?.data?.title ||
        "Registracija nije uspela.";
      setError(msg);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="modalOverlay" onMouseDown={onClose}>
      <div className="modalCard" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modalHeader">
          <h3>Registracija</h3>
          <button className="iconBtn" onClick={onClose} aria-label="Close">
            ✕
          </button>
        </div>

        <form onSubmit={submit} className="modalForm">
          <div className="twoCol">
            <div className="field">
              <label>Ime</label>
              <input
                value={form.firstName}
                onChange={(e) => setField("firstName", e.target.value)}
                placeholder="Ime"
                autoComplete="given-name"
              />
            </div>
            <div className="field">
              <label>Prezime</label>
              <input
                value={form.lastName}
                onChange={(e) => setField("lastName", e.target.value)}
                placeholder="Prezime"
                autoComplete="family-name"
              />
            </div>
          </div>

          <div className="field">
            <label>Email</label>
            <input
              value={form.email}
              onChange={(e) => setField("email", e.target.value)}
              placeholder="Email"
              autoComplete="email"
              inputMode="email"
            />
          </div>

          <div className="field">
            <label>Šifra</label>
            <input
              type="password"
              value={form.password}
              onChange={(e) => setField("password", e.target.value)}
              placeholder="Password"
              autoComplete="new-password"
            />
          </div>

          <div className="field">
            <label>Lokacija (grad)</label>
            <select
              value={form.city}
              onChange={(e) => setField("city", e.target.value)}
            >
              {CITY_OPTIONS.map((c) => (
                <option value={c} key={c}>
                  {c}
                </option>
              ))}
            </select>
          </div>

          {error ? <div className="errorBox">{error}</div> : null}

          <button className="primaryBtn" disabled={loading}>
            {loading ? "Registrujem..." : "Registruj se"}
          </button>
        </form>
      </div>
    </div>
  );
}
