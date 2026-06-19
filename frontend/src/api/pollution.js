import { api } from "./api";

export async function getPollutionSeries(city, day = 1) {
  const c = encodeURIComponent(city || "Subotica");
  const res = await api.get(`/pollution/${c}/series`, { params: { day } });
  return res.data;
}