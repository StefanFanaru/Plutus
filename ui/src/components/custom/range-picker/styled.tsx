import { Box, styled } from "@mui/material";

export const DateRangePickerStyled = styled("div")(() => ({
  display: "flex",
  flexDirection: "column",
  alignItems: "center",
  ".MuiInputBase-root": {
    cursor: "pointer",
  },
  ".MuiOutlinedInput-notchedOutline": {
    borderWidth: "1px !important",
    // borderColor: `${theme.palette.border.inactive} !important`,
  },
  // ".Mui-focused .MuiOutlinedInput-notchedOutline": {
  //   borderColor: `${"#0186BE"} !important`,
  // },
}));

export const CalenderLayoutContainer = styled(Box)(() => ({
  display: "flex",
  paddingLeft: 2,
  marginTop: 8,
  height: "320px",
  ".MuiDateCalendar-root": {
    height: "unset",
    ".MuiPickersDay-root, .MuiDayCalendar-weekDayLabel": {
      fontSize: "14px",
    },
    ".MuiPickersDay-dayOutsideMonth": {
      color: "var(--template-palette-text-disabled)",
    },
  },
  ".toolbar-container": {
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-around",
    button: {
      textTransform: "unset",
      display: "block",
      minWidth: "unset",
    },
    ".toolbar-button": {
      margin: "4px",
      border: "none",
      background: "none",
      // hover
      ":hover": {
        background: "var(--template-palette-action-hover)",
      },
      // color: theme.palette.grey[900],
      fontSize: 14,
      "&.disabled": {
        color: "#D5D5D5",
      },
      "&.selected": {
        borderRadius: "4px",
        color: "var(--template-palette-primary-main)",
        backgroundColor: "var(--template-palette-action-hover)",
        cursor: "default",
      },
      "&.reset-button": {
        color: "var(--template-palette-error-main)",
        fontWeight: 700,
        minWidth: "unset",
        textTransform: "unset",
        display: "block",
        // fontSize: 18,
      },
      "&.reset-button.disabled": {
        color: "var(--template-palette-text-disabled)",
      },
    },
  },
  ".calendar-header-container": {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    button: {
      // color: theme.palette.grey[900],
      // svg: { fontSize: 14 },
    },
    ".year-label": {
      fontSize: 18,
      fontWeight: 700,
      textTransform: "capitalize",
      width: "170px",
    },
    ".month-change-btn": {
      fontSize: "16px",
    },
  },
  ".MuiPickersMonth-monthButton , .MuiPickersYear-yearButton": {
    "&.Mui-selected": {
      backgroundColor: "var(--template-palette-primary-main)",
      color: "#ffffff",
    },
  },
}));

export const StyledDayContainer = styled(Box)(() => ({
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  borderRadius: 0,
  "&.day-start": {
    // background:
    //   "linear-gradient(to right, transparent 50%, rgba(0,89,179,0.2) 50%)",
    borderRadius: "18px 0 0 18px",
  },
  "&.day-start.day-end": {
    background: "none",
  },
  "&.day-end": {
    // background:
    //   "linear-gradient(to left, transparent 50%, rgba(0,89,179,0.2) 50%)",
    borderRadius: "0 18px 18px 0",
  },
  "&.rounded": {
    borderRadius: "18px",
    backgroundColor: "transparent",
  },
  "&.highlighted-text": {
    ".MuiPickersDay-root": {
      backgroundColor: "var(--template-palette-primary-main)",
      color: "#ffffff",
    },
  },
}));
