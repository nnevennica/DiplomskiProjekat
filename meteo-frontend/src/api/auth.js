import { api } from "./api";

export async function login(payload) {
  const res = await api.post("/auth/login", payload);
  return res.data;
}

export async function register(payload) {
  const res = await api.post("/auth/register", payload);
  return res.data;
}