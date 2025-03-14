import { Chip } from "@mui/material";
import { SparkLineChart } from "@mui/x-charts";
import { GridCellParams, GridColDef } from "@mui/x-data-grid";

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

function renderAmount(amount: string) {
  return !!amount && amount != "" ? (
    <Chip label={amount} color="error" size="small" />
  ) : (
    <div />
  );
}

export const categoryColumns: GridColDef[] = [
  {
    field: "name",
    headerName: "Name",
    flex: 1.5,
    minWidth: 130,
    maxWidth: 130,
  },
  {
    field: "ammountCreditedThisMonth",
    headerName: "Month Amount Credited",
    headerAlign: "right",
    align: "right",
    flex: 1,
    minWidth: 100,
    renderCell: (params) => renderAmount(params.value as any),
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
    field: "totalAmmountCredited",
    headerName: "Total Amount Credited",
    headerAlign: "right",
    align: "right",
    flex: 1,
    minWidth: 140,
  },
  {
    field: "latestTransaction",
    headerName: "Latest",
    flex: 1,
    minWidth: 145,
  },
  {
    field: "amountPerMonth",
    headerName: "Amount Per Month",
    flex: 1,
    minWidth: 150,
    renderCell: renderSparklineCell,
    filterable: false,
    sortable: false,
  },
];
