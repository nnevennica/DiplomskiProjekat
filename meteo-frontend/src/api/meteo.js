import { api } from "./api";

export async function getMeteoSeries(city, day = 1) {
  const c = encodeURIComponent(city || "Subotica");
  const res = await api.get(`/meteo/${c}/series`, { params: { day } });
  return res.data;
}