import React from "react";
import { Button, IconButton } from "@mui/material";
import { Moment } from "moment";
import { DateView } from "@mui/x-date-pickers";
import { SlideDirection } from "@mui/x-date-pickers/DateCalendar/PickersSlideTransition";
import { Dayjs } from "dayjs";
import NavigateNextIcon from "@mui/icons-material/NavigateNext";
import NavigateBeforeIcon from "@mui/icons-material/NavigateBefore";

interface CustomCalendarHeaderProps {
  date: Moment | Dayjs;
  onMonthChange: (date: Moment, slideDirection: SlideDirection) => void;
  onViewChange?: (view: DateView) => void;
}

const CustomCalendarHeader: React.FC<CustomCalendarHeaderProps> = ({
  date,
  onMonthChange,
  onViewChange,
}) => {
  const handleMonthOrYearChange = (
    unit: "year" | "month",
    amount: number,
    direction: SlideDirection,
  ) => {
    date = date as Moment;
    onMonthChange(date.clone().add(amount, unit), direction);
  };

  return (
    <div className="calendar-header-container">
      {/* <IconButton */}
      {/*   onClick={() => handleMonthOrYearChange("year", -1, "left")} */}
      {/*   data-testid="prev-year-btn" */}
      {/*   size="small" */}
      {/*   sx={{ marginRight: "5px", width: "2rem", height: "2rem" }} */}
      {/* > */}
      {/*   <FirstPageIcon /> */}
      {/* </IconButton> */}
      <IconButton
        onClick={() => handleMonthOrYearChange("month", -1, "left")}
        data-testid="prev-month-btn"
        size="small"
        sx={{ width: "2rem", height: "2rem" }}
      >
        <NavigateBeforeIcon />
      </IconButton>
      <Button
        className="year-label"
        onClick={() => onViewChange?.("month")}
        data-testid="select-year-btn"
      >
        {date.format("MMMM YYYY")}
      </Button>
      <IconButton
        onClick={() => handleMonthOrYearChange("month", 1, "right")}
        data-testid="next-month-btn"
        size="small"
        sx={{ marginRight: "5px", width: "2rem", height: "2rem" }}
      >
        <NavigateNextIcon />
      </IconButton>
      {/* <IconButton */}
      {/*   onClick={() => handleMonthOrYearChange("year", 1, "right")} */}
      {/*   data-testid="next-year-btn" */}
      {/*   size="small" */}
      {/*   sx={{ width: "2rem", height: "2rem" }} */}
      {/* > */}
      {/*   <LastPageIcon /> */}
      {/* </IconButton> */}
    </div>
  );
};

export default CustomCalendarHeader;
