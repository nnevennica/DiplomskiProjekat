import { api } from "./api";

export async function getAlerts(city, day = 1) {
  const c = encodeURIComponent(city || "Subotica");
  const res = await api.get(`/alerts/${c}`, { params: { day } });
  return res.data;
}