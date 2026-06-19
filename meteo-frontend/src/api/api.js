import axios from "axios";

export const api = axios.create({
  baseURL: "http://localhost:5132/api",
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  const url = config?.url ?? "";
  const isAuthRoute = url.includes("/auth/login") || url.includes("/auth/register");

  if (token && !isAuthRoute) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});