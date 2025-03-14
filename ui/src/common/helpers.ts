export function addCommasToNumber(
  number: number | undefined | null,
  forceDecimals: boolean = true,
): string {
  if (!number) {
    return "0.00";
  }
  // Convert the number to a string
  const strNumber = number.toFixed(2);

  // Split the number into integer and decimal parts (if any)
  const parts = strNumber.split(".");
  const integerPart = parts[0];
  const decimalPart = parts[1] || "";

  // Add commas to the integer part
  const integerWithCommas = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

  // Combine the integer and decimal parts (if any) with commas
  const numberWithCommas = decimalPart
    ? `${integerWithCommas},${decimalPart}`
    : integerWithCommas;

  if (forceDecimals) {
    return numberWithCommas;
  } else {
    return numberWithCommas.replace(/,00/, "");
  }
}

export function hoursAndMinutes(hours: number, minutes: number) {
  return `${hours > 9 ? hours : "0" + hours}:${minutes > 9 ? minutes : "0" + minutes}`;
}

export function makeSimpleDateString(date: Date) {
  return `${date.getDate() > 9 ? date.getDate() : "0" + date.getDate()}/${
    date.getMonth() + 1 > 9 ? date.getMonth() + 1 : "0" + (date.getMonth() + 1)
  }/${date.getFullYear()}`;
}

export function dayOfWeekAsString(dayIndex: number) {
  return ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"][dayIndex] || "";
}

export function makePrettyDate(value: string): string {
  if (!value) {
    return "N/A";
  }

  const date: Date = new Date(Date.parse(value));
  date.setMinutes(date.getMinutes() - date.getTimezoneOffset());

  const now: Date = new Date();
  const hours = date.getHours();
  const minutes = date.getMinutes();

  const isNextWeekSameday =
    date.getDay() === now.getDay() &&
    Math.abs(now.getDate() - date.getDate()) > 0;
  const isSameYear = now.getFullYear() === date.getFullYear();
  const isSameMonth = now.getMonth() === date.getMonth();

  if (
    Math.abs(now.getDate() - date.getDate()) > 7 ||
    !isSameMonth ||
    isNextWeekSameday ||
    !isSameYear
  ) {
    return `${makeSimpleDateString(date)} ${hoursAndMinutes(hours, minutes)}`;
  }

  if (date.getDate() === now.getDate() && date.getMonth() === now.getMonth()) {
    return hoursAndMinutes(hours, minutes);
  }

  return `${dayOfWeekAsString(date.getDay())} ${hoursAndMinutes(hours, minutes)}`;
}

export function calculateMedian(numbers: number[]): number {
  if (numbers.length === 0) {
    return 0;
  }

  numbers = numbers.filter((n) => n > 0 || n < 0);
  // Sort the array in ascending order
  const sortedNumbers = numbers.slice().sort((a, b) => a - b);
  const midIndex = Math.floor(sortedNumbers.length / 2);

  // If the length of the array is odd, return the middle element
  if (sortedNumbers.length % 2 === 1) {
    return sortedNumbers[midIndex];
  } else {
    // If the length is even, return the average of the two middle elements
    return (sortedNumbers[midIndex - 1] + sortedNumbers[midIndex]) / 2;
  }
}

export const shortMonthsMapping = {
  Jan: "January",
  Feb: "February",
  Mar: "March",
  Apr: "April",
  May: "May",
  Jun: "June",
  Jul: "July",
  Aug: "August",
  Sep: "September",
  Oct: "October",
  Nov: "November",
  Dec: "December",
};

export function getLast12Months() {
  const result: string[] = [];
  const months = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];
  const today = new Date();
  for (let i = 11; i >= 0; i--) {
    const date = new Date(today);
    date.setMonth(today.getMonth() - i);
    result.push(months[date.getMonth()]);
  }
  return result;
}
