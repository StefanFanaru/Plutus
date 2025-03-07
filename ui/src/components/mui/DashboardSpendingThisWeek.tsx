import * as React from "react";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import { BarChart } from "@mui/x-charts/BarChart";
import { useTheme } from "@mui/material/styles";
import axiosClient from "../../axiosClient";
import { useEffect } from "react";
import { BarSeriesType } from "@mui/x-charts";
import { MakeOptional } from "@mui/x-charts/internals";
import { addCommasToNumber } from "../../common/helpers";
import SpendingDiffChip from "./SpendingDiffChip";

interface SpentByCategoryItem {
  categoryID: string;
  categoryName: string;
  data: number[];
}

interface Response {
  spentByCategoryItems: SpentByCategoryItem[];
  totalSpent: number;
  percentageSpendingChange: number;
}

export default function DashboardSpendingThisWeek() {
  const [response, setResponse] = React.useState<Response | null>(null);
  const [series, setSeries] = React.useState<
    MakeOptional<BarSeriesType, "type">[]
  >([]);

  const theme = useTheme();
  const colorPalette = [
    (theme.vars || theme).palette.primary.dark,
    (theme.vars || theme).palette.primary.main,
    (theme.vars || theme).palette.primary.light,
  ];

  function getLast7Days() {
    const result: string[] = [];
    const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    const today = new Date();
    for (let i = 6; i >= 0; i--) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);
      result.push(days[date.getDay()]);
    }
    return result;
  }

  useEffect(() => {
    fetchItems();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function buildRequest() {
    const timezoneOffset = new Date().getTimezoneOffset(); // This returns the offset in minutes
    const now = new Date();
    const startDate = new Date(
      now.getTime() - 6 * 24 * 60 * 60 * 1000 - timezoneOffset * 60 * 1000,
    );
    const endDate = new Date(now.getTime() - timezoneOffset * 60 * 1000);
    return {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString(),
    };
  }

  const fetchItems = async () => {
    const response = await axiosClient.post<Response>(
      "/api/Dashboard/spending-by-period",
      buildRequest(),
    );

    setResponse(response.data);

    const series = response.data.spentByCategoryItems.map(
      (item) =>
        ({
          id: item.categoryID,
          label: item.categoryName,
          data: item.data,
          stack: "A",
          stackOrder: "reverse",
          valueFormatter: (value) =>
            value ? `${addCommasToNumber(value!)} RON` : null,
        }) as MakeOptional<BarSeriesType, "type">,
    );
    setSeries(series);
  };

  return (
    <Card variant="outlined" sx={{ width: "100%" }}>
      <CardContent>
        <Typography component="h2" variant="subtitle2" gutterBottom>
          Spending last 7 days
        </Typography>
        <Stack sx={{ justifyContent: "space-between" }}>
          <Stack
            direction="row"
            sx={{
              alignContent: { xs: "center", sm: "flex-start" },
              alignItems: "center",
              gap: 1,
            }}
          >
            <Typography variant="h4" component="p">
              {response?.totalSpent} RON
            </Typography>
            <SpendingDiffChip diff={response?.percentageSpendingChange ?? 0} />
          </Stack>
          <Typography variant="caption" sx={{ color: "text.secondary" }}>
            Spening statistics for the last 7 days
          </Typography>
        </Stack>
        <BarChart
          colors={colorPalette}
          // borderRadius={8}
          xAxis={
            [
              {
                scaleType: "band",
                categoryGapRatio: 0.5,
                data: getLast7Days(),
                tickPlacement: "middle",
              },
            ] as any
          }
          series={series}
          yAxis={[{ reverse: true }]}
          height={250}
          margin={{ left: 50, right: 0, top: 20, bottom: 20 }}
          grid={{ horizontal: true }}
          slotProps={{
            legend: {
              hidden: true,
            },
          }}
        />
      </CardContent>
    </Card>
  );
}
