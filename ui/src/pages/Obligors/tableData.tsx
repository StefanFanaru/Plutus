import { Checkbox, Chip, Tooltip } from "@mui/material";
import { SparkLineChart } from "@mui/x-charts";
import { GridCellParams, GridColDef } from "@mui/x-data-grid";
import {
  ObligorFormattedListItem,
  ObligorsColumnsProps,
} from "./ObligorsTable";

function renderForFixedExepenses(
  obligor: ObligorFormattedListItem,
  onFixedExpenseChange: (obligorId: string, isFixedExpense: boolean) => void,
) {
  return (
    <Checkbox
      checked={obligor.isForFixedExpenses}
      onChange={(e) =>
        onFixedExpenseChange(obligor.displayName, e.target.checked)
      }
      sx={{ "& .MuiSvgIcon-root": { fontSize: 28 } }}
    />
  );
}

function renderObligorName(name: string, displayName: string) {
  return (
    <Tooltip title={name}>
      <span>{displayName}</span>
    </Tooltip>
  );
}

function renderAmount(amount: string) {
  return !!amount && amount != "" ? (
    <Chip label={amount} color="error" size="small" />
  ) : (
    <div />
  );
}

type SparkLineData = number[];

function renderSparklineCell(params: GridCellParams<SparkLineData, any>) {
  // data is the months of the year
  const data = [
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

  const currentMonth = new Date().getMonth();
  const orderedData = [
    ...data.slice(currentMonth + 1),
    ...data.slice(0, currentMonth + 1),
  ];

  const { value, colDef } = params;
  // apply same ordering for value
  const orderedValue = [
    ...value.slice(currentMonth + 1),
    ...value.slice(0, currentMonth + 1),
  ];

  if (!value || value.length === 0) {
    return null;
  }

  return (
    <div style={{ display: "flex", alignItems: "center", height: "100%" }}>
      <SparkLineChart
        data={orderedValue}
        yAxis={{ reverse: true }}
        width={colDef.computedWidth || 100}
        height={32}
        plotType="bar"
        showHighlight
        showTooltip
        colors={["hsl(210, 98%, 42%)"]}
        xAxis={{
          scaleType: "band",
          data: orderedData,
        }}
      />
    </div>
  );
}

export function getObligorsColumns(props: ObligorsColumnsProps): GridColDef[] {
  return [
    {
      field: "displayName",
      headerName: "Name",
      flex: 1.5,
      minWidth: 200,
      renderCell: (params) =>
        renderObligorName(params.row.name, params.row.displayName),
    },
    {
      field: "isForFixedExpenses",
      headerName: "Fixed Expenses",
      flex: 1,
      minWidth: 80,
      renderCell: (params) =>
        renderForFixedExepenses(
          params.row as ObligorFormattedListItem,
          props.onFixedExpenseChange,
        ),
    },
    {
      field: "monthlyMedian",
      headerName: "Month Median",
      flex: 1,
      minWidth: 100,
      headerAlign: "right",
      align: "right",
      renderCell: (params) => renderAmount(params.value as any),
    },
    {
      field: "ammountCreditedThisMonth",
      headerName: "Month Amount Credited",
      headerAlign: "right",
      align: "right",
      flex: 1,
      minWidth: 100,
    },
    {
      field: "totalAmmountCredited",
      headerName: "Total Amount Credited",
      headerAlign: "right",
      align: "right",
      flex: 1,
      minWidth: 110,
    },
    {
      field: "latestTransaction",
      headerName: "Latest",
      headerAlign: "right",
      align: "right",
      flex: 1,
      minWidth: 145,
    },
    {
      field: "transactionsPerMonth",
      headerName: "Amount Per Month",
      flex: 1,
      minWidth: 150,
      renderCell: renderSparklineCell,
      filterable: false,
      sortable: false,
    },
  ];
}
