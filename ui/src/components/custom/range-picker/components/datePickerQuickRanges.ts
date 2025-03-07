import moment from "moment";

const today = moment();
export const datePickerQuickRanges = [
  {
    label: "All",
    action: "all",
    start: null,
    end: null,
  },
  {
    label: "Today",
    action: "today",
    start: today.clone().startOf("day"),
    end: today.clone().endOf("day"),
  },
  {
    label: "Yesterday",
    action: "yesterday",
    start: today.clone().subtract(1, "day").startOf("day"),
    end: today.clone().subtract(1, "day").endOf("day"),
  },
  {
    label: "Last 7 days",
    action: "lastWeek",
    start: today.clone().subtract(7, "days"),
    end: today.clone().endOf("day"),
  },
  {
    label: "Last 30 days",
    action: "lastMonth",
    start: today.clone().subtract(30, "days"),
    end: today.clone().endOf("day"),
  },
  {
    label: "This month",
    action: "thisMonth",
    start: today.clone().startOf("month"),
    end: today.clone().endOf("month"),
  },
  {
    label: "Last quarter",
    action: "lastQuarter",
    start: today.clone().subtract(1, "quarter").startOf("quarter"),
    end: today.clone().subtract(1, "quarter").endOf("quarter"),
  },
];
