import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login as loginApi } from "../api/auth";
import RegisterModal from "../components/RegisterModal";

const AVATAR = "/img.jpg";

export default function LoginPage() {
  const navigate = useNavigate();

  const [showRegister, setShowRegister] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function submit(e) {
    e.preventDefault();
    if (loading) return;

    setError("");

    const emailNorm = email.trim().toLowerCase();
    const passNorm = password.trim();

    if (!emailNorm || !emailNorm.includes("@")) {
      setError("Unesite validan mail.");
      return;
    }

    if (!passNorm) {
      setError("Unesite šifru.");
      return;
    }

    setLoading(true);

    try {
      const data = await loginApi({
        email: emailNorm,
        password: passNorm,
      });

      const token = data?.accessToken ?? data?.token;

      if (!token) {
        setError("Login uspešan, ali token nije vraćen sa servera.");
        return;
      }

      localStorage.setItem("accessToken", token);

      if (data?.user) {
        localStorage.setItem("user", JSON.stringify(data.user));
      }

      navigate("/dashboard", { replace: true });
    } catch (err) {
      const serverData = err?.response?.data;

      const msg =
        (typeof serverData === "string" && serverData) ||
        serverData?.title ||
        serverData?.message ||
        (err?.response?.status === 401
          ? "Pogrešan email ili šifra."
          : "Greška pri prijavi. Pokušajte ponovo.");

      setError(msg);
    } finally {
      setLoading(false);
    }
  }

  function onRegisterSuccess(data) {
    const token = data?.accessToken ?? data?.token;

    if (token) {
      localStorage.setItem("accessToken", token);
    }

    if (data?.user) {
      localStorage.setItem("user", JSON.stringify(data.user));
    }

    setShowRegister(false);
    navigate("/dashboard", { replace: true });
  }

  return (
    <div className="authBg">
      <div className="authCard">
        <div className="authLeft">
          <div className="illustration">
            <img src={AVATAR} alt="Weather" className="loginAvatar" />
          </div>
        </div>

        <div className="authRight">
          <h2 className="title">Member Login</h2>

          <form onSubmit={submit} className="authForm" noValidate>
            <div className="inputWrap">
              <span className="icon" aria-hidden="true">
                ✉
              </span>
              <input
                id="email"
                name="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email"
                autoComplete="email"
                inputMode="email"
                required
              />
            </div>

            <div className="inputWrap">
              <span className="icon" aria-hidden="true">
                🔒
              </span>
              <input
                id="password"
                name="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Password"
                autoComplete="current-password"
                required
              />
            </div>

            {error ? (
              <div className="errorBox" role="alert" aria-live="polite">
                {error}
              </div>
            ) : null}

            <button className="primaryBtn" type="submit" disabled={loading}>
              {loading ? "LOGIN..." : "LOGIN"}
            </button>
          </form>

          <div className="createRow">
            <button
              className="linkBtn"
              type="button"
              onClick={() => setShowRegister(true)}
              disabled={loading}
            >
              Create your Account →
            </button>
          </div>
        </div>
      </div>

      <RegisterModal
        open={showRegister}
        onClose={() => setShowRegister(false)}
        onSuccess={onRegisterSuccess}
      />
    </div>
  );
}