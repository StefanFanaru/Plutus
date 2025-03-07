import axios from "axios";
import { createObservable } from "./common/observable";

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL, // Replace with your API base URL
  timeout: 10000, // Set timeout (in milliseconds)
});

interface AxiosDatePickerHeader {
  startDate?: string;
  endDate?: string;
}

export const datePickerFilter = createObservable<AxiosDatePickerHeader>();

function handleDatePickerFilterHeader(config) {
  let axiosDatePickerHeader = datePickerFilter.getValue()!;

  if (!axiosDatePickerHeader) {
    const localFilter = sessionStorage.getItem("date-picker-filter");
    if (localFilter) {
      const { startDate, endDate } = JSON.parse(localFilter);
      axiosDatePickerHeader = {
        startDate: startDate,
        endDate: endDate,
      };
    }
  }

  if (axiosDatePickerHeader?.startDate && axiosDatePickerHeader?.endDate) {
    config.headers["Date-Filter"] =
      `${axiosDatePickerHeader.startDate};${axiosDatePickerHeader.endDate}`;
  }
}

// Optional: Add request interceptors
axiosClient.interceptors.request.use(
  (config) => {
    const clientId = import.meta.env.VITE_OIDC_CLIENT_ID;
    const oidcUser = localStorage.getItem(
      `oidc.user:https://auth.stefanaru.com/realms/stefanaru:${clientId}`,
    );

    if (oidcUser) {
      const user = JSON.parse(oidcUser!);
      config.headers["Authorization"] = `Bearer ${user.access_token}`;
    }

    handleDatePickerFilterHeader(config);

    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

// Optional: Add response interceptors
// axiosClient.interceptors.response.use(
//   (response) => {
//     // You can modify the response here if needed
//     return response;
//   },
//   (error) => {
//     // Handle errors globally
//     return Promise.reject(error);
//   },
// );

export default axiosClient;
