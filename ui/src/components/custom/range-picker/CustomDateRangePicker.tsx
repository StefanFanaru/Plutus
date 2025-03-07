import React, { useRef } from "react";
import { AdapterMoment } from "@mui/x-date-pickers/AdapterMoment";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import {
  DatePickerProps,
  PickerValidDate,
  StaticDatePicker,
} from "@mui/x-date-pickers";
import moment, { Moment } from "moment";
import { useEffect, useState } from "react";
import CustomCalendarHeader from "./components/CustomCalendarHeader";
import Layout from "./components/CustomCalenderLayout";
import Day from "./components/CustomCalenderDay";
import { DateRangePickerStyled } from "./styled";
import { Button } from "@mui/material";
import CalendarTodayRoundedIcon from "@mui/icons-material/CalendarTodayRounded";
import "./CustomDateRangePicker.css";
import { datePickerFilter } from "../../../axiosClient";
import { datePickerQuickRanges } from "./components/datePickerQuickRanges";

moment.updateLocale("en", {
  week: {
    dow: 1,
  },
});

export type DateRange = [Moment | null, Moment | null];

type DateRangePickerProps = Omit<
  DatePickerProps<PickerValidDate, boolean>,
  "onChange" | "value"
>;

const CustomDateRangePicker = ({ ...restProps }: DateRangePickerProps) => {
  const [startDate, setStartDate] = useState<Moment | null>(null);
  const [endDate, setEndDate] = useState<Moment | null>(null);
  const [open, setOpen] = useState(false);
  const [isDatePickerOpen, setIsDatePickerOpen] = useState(false);
  const [buttonText, setButtonText] = useState("Filter");
  const refPicker = useRef<HTMLDivElement>(null);

  const isInRange = (date: Moment): boolean => {
    if (!startDate || !endDate) return false;
    return date.isBetween(startDate, endDate, "day", "[]");
  };

  const selectAndCloseCalendar = (
    start: Moment | null,
    end: Moment | null,
    buttonText: string,
  ) => {
    if (start && !end) {
      end = start.clone();
    }
    setOpen(false);
    setIsDatePickerOpen(false);

    datePickerFilter.setValue({
      startDate: start?.format("YYYY-MM-DD") || "",
      endDate: end?.format("YYYY-MM-DD") || "",
    });

    sessionStorage.setItem(
      "date-picker-filter",
      JSON.stringify({ ...datePickerFilter.getValue(), buttonText }),
    );
  };

  const handleToolbarAction = (
    start: Moment | null,
    end: Moment | null,
    action: string,
    label: string,
  ) => {
    setButtonText(label);
    setStartDate(start);
    setEndDate(end);
    if (action !== "reset") {
      selectAndCloseCalendar(start, end, buttonText);
    }
  };

  const handleDateChange = (date: Moment | null) => {
    if (!startDate || endDate || (date && date.isBefore(startDate, "day"))) {
      setStartDate(date);
      setEndDate(null);
      setButtonText("Custom");
    } else {
      setEndDate(date);
      selectAndCloseCalendar(startDate, date, "Custom");
      handleSelectedRange(startDate, date!);
    }
  };

  const handleSelectedRange = (startMoment: Moment, endMoment: Moment) => {
    const selectedRange = datePickerQuickRanges.find(
      ({ start, end }) =>
        startMoment?.isSame(start, "day") && endMoment?.isSame(end, "day"),
    );
    if (selectedRange) {
      setButtonText(selectedRange.label);
    }
  };

  useEffect(() => {
    const localFilter = sessionStorage.getItem("date-picker-filter");
    if (localFilter) {
      const { startDate, endDate, buttonText } = JSON.parse(localFilter);
      const startMoment = moment(startDate);
      const endMoment = moment(endDate);

      setStartDate(startMoment);
      setEndDate(endMoment);
      setButtonText(buttonText);

      handleSelectedRange(startMoment, endMoment);
    }

    const handleClickOutside = (event: MouseEvent) => {
      const target = event.target as HTMLElement;
      if (target.id === "date-range-picker-button") return;
      // Check if the click was outside the component
      if (refPicker.current && !refPicker.current.contains(target)) {
        setIsDatePickerOpen(false);
      }
    };

    // Bind the event listener
    document.addEventListener("mousedown", handleClickOutside);

    // Clean up the event listener on component unmount
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const handleTouchStart = (event: React.TouchEvent<HTMLDivElement>) => {
    event.preventDefault(); // Prevent scrolling on touch start
  };

  const handleTouchMove = (event: React.TouchEvent<HTMLDivElement>) => {
    event.preventDefault(); // Prevent scrolling on touch move
  };

  return (
    <div>
      <Button
        variant="outlined"
        id="date-range-picker-button"
        size="small"
        onClick={setIsDatePickerOpen.bind(null, (prev) => !prev)}
        startIcon={<CalendarTodayRoundedIcon fontSize="small" />}
        sx={{ minWidth: "fit-content" }}
      >
        {buttonText}
      </Button>
      {isDatePickerOpen && (
        <div
          className="date-range-wrapper"
          onTouchStart={handleTouchStart} // Prevent scroll on touch start
          onTouchMove={handleTouchMove} // Prevent scroll on touch move
          ref={refPicker}
        >
          <DateRangePickerStyled>
            <LocalizationProvider dateAdapter={AdapterMoment}>
              <StaticDatePicker
                views={["month", "year", "day"]}
                value={endDate || startDate || null}
                closeOnSelect={false}
                disableHighlightToday
                open={open}
                onOpen={() => setOpen(true)}
                showDaysOutsideCurrentMonth
                dayOfWeekFormatter={(day) =>
                  ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"][day.day()]
                }
                slots={{
                  day: (day) => (
                    <Day
                      isInRange={isInRange}
                      startDate={startDate}
                      endDate={endDate}
                      onDateClick={handleDateChange}
                      {...day}
                    />
                  ),
                  calendarHeader: (props) => (
                    <CustomCalendarHeader
                      date={props.currentMonth}
                      onMonthChange={props.onMonthChange}
                      onViewChange={props.onViewChange}
                    />
                  ),
                  layout: (prop) => (
                    <Layout
                      handleToolbarAction={handleToolbarAction}
                      startDate={startDate}
                      endDate={endDate}
                    >
                      {prop.children}
                    </Layout>
                  ),
                }}
                {...restProps}
              />
            </LocalizationProvider>
          </DateRangePickerStyled>
        </div>
      )}
    </div>
  );
};

export default CustomDateRangePicker;
