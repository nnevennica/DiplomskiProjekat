import { api } from "./api";

export async function getCities() {
  const res = await api.get("/cities");
  return res.data; // ["Beograd","NoviSad","Nis"...]
}
