import { Moment } from "moment";
import { useEffect, useState } from "react";
import { CalenderLayoutContainer } from "../styled";
import ToolbarButton from "./QuickRangeSelector";
import React from "react";
import { datePickerQuickRanges } from "./datePickerQuickRanges";

interface LayoutProps {
  handleToolbarAction: (
    startDate: Moment | null,
    endDate: Moment | null,
    action: string,
    label: string,
  ) => void;
  children: React.ReactNode;
  startDate: Moment | null;
  endDate: Moment | null;
}

const Layout = ({
  handleToolbarAction,
  children,
  startDate,
  endDate,
}: LayoutProps) => {
  const [selectedToolbarAction, setSelectedToolbarAction] = useState<
    string | null
  >(null);

  const handleQuickRangeChange = (action: string, label: string) => {
    const range = datePickerQuickRanges.find(
      (range) => range.action === action,
    );
    handleToolbarAction(
      range?.start || null,
      range?.end || null,
      action,
      label,
    );
  };

  useEffect(() => {
    const selectedRange = datePickerQuickRanges.find(
      ({ start, end }) =>
        startDate?.isSame(start, "day") && endDate?.isSame(end, "day"),
    );
    setSelectedToolbarAction(selectedRange?.action || null);

    if (!startDate && !endDate) {
      setSelectedToolbarAction("all");
    }
  }, [startDate, endDate]);

  return (
    <CalenderLayoutContainer>
      <div className="toolbar-container" data-testid="quick-range-container">
        <div>
          {datePickerQuickRanges.map(({ label, action }) => (
            <ToolbarButton
              key={action}
              label={label}
              selected={selectedToolbarAction === action}
              action={action}
              onActionClick={handleQuickRangeChange}
            />
          ))}
          {/* <ToolbarButton */}
          {/*   className={classNames("reset-button", { */}
          {/*     disabled: !startDate && !endDate, */}
          {/*   })} */}
          {/*   label="Reset" */}
          {/*   action="reset" */}
          {/*   onActionClick={handleQuickRangeChange} */}
          {/* /> */}
        </div>
      </div>
      {children}
    </CalenderLayoutContainer>
  );
};

export default Layout;
