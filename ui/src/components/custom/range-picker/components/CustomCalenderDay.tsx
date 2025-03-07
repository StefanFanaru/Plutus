import { PickersDay, PickersDayProps } from "@mui/x-date-pickers";
import { Moment } from "moment";
import classNames from "classnames";
import { StyledDayContainer } from "../styled";
import { Dayjs } from "dayjs";

interface DayProps extends Omit<PickersDayProps<Moment | Dayjs>, "onClick"> {
  isInRange: (date: Moment) => boolean;
  startDate: Moment | null;
  endDate: Moment | null;
  onDateClick: (date: Moment | null) => void;
}

const Day = ({
  day,
  isInRange,
  startDate,
  endDate,
  onDateClick,
  ...pickersDayProps
}: DayProps) => {
  // convert day to Moment
  const isHighlighted = isInRange(day as Moment);
  const isStart = !!startDate?.isSame(day as Moment, "day");
  const isEnd = !!endDate?.isSame(day as Moment, "day");
  return (
    <StyledDayContainer
      className={classNames({
        "day-start": isStart,
        "day-end": isEnd,
        "day-range": isHighlighted,
        rounded: isStart && isEnd,
        "highlighted-text": isStart || isEnd,
      })}
      key={day.toString()}
    >
      <PickersDay
        {...pickersDayProps}
        day={day}
        onClick={() => onDateClick(day as Moment)}
      />
    </StyledDayContainer>
  );
};

export default Day;
