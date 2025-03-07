import { useTheme } from "@mui/material/styles";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import { LineChart } from "@mui/x-charts/LineChart";
import { useEffect, useState } from "react";
import { LineSeriesType } from "@mui/x-charts";
import { addCommasToNumber } from "../../common/helpers";
import axiosClient from "../../axiosClient";
import SpendingDiffChip from "./SpendingDiffChip";

function AreaGradient({ color, id }: { color: string; id: string }) {
  return (
    <defs>
      <linearGradient id={id} x1="50%" y1="0%" x2="50%" y2="100%">
        <stop offset="0%" stopColor={color} stopOpacity={0.5} />
        <stop offset="100%" stopColor={color} stopOpacity={0} />
      </linearGradient>
    </defs>
  );
}

export interface SpendingStatsReponse {
  spentPerDayLast25Days: SpentPerDay[];
  projectionNext5Days: SpentPerDay[];
  percentageSpendingChange: number;
  totalSpendLast30Days: number;
}

export interface SpentPerDay {
  date: string;
  amount: number;
}

export default function DashboardSpendingStats() {
  const theme = useTheme();
  const [dates, setDates] = useState<string[]>([]);
  const [spendingStatsReponse, setSpedingStatsResponse] =
    useState<SpendingStatsReponse | null>(null);
  const [spedingStats, setSpendingStats] = useState<LineSeriesType | null>(
    null,
  );

  const fetchExpenses = async () => {
    const response = await axiosClient.get<SpendingStatsReponse>(
      "/api/Dashboard/spending-stats",
    );

    const spent25Days = response.data.spentPerDayLast25Days.map(
      (x) => x.amount,
    );
    const projection5Days = response.data.projectionNext5Days.map(
      (x) => x.amount,
    );
    const data = [...spent25Days.reverse(), ...projection5Days];

    setSpendingStats({
      id: "direct",
      label: "Spent",
      showMark: false,
      curve: "linear",
      stack: "total",
      type: "line",
      area: true,
      stackOrder: "ascending",
      valueFormatter: (value) => addCommasToNumber(value!) + " RON",
      data: data,
    });

    const spent25DaysDates = response.data.spentPerDayLast25Days.map(
      (x) => x.date,
    );
    const projection5DaysDates = response.data.projectionNext5Days.map(
      (x) => x.date,
    );
    const datesValues = [
      ...spent25DaysDates.reverse(),
      ...projection5DaysDates,
    ];
    // from dates values which are iso strings we extract the day and month in format "MMM DD"
    const dates = datesValues.map((date, index) => {
      const dateObj = new Date(date);
      // for the last 5 days add prefix "Projected" to the date
      if (index >= spent25DaysDates.length) {
        return (
          "Projected: " +
          dateObj.toLocaleDateString("en-US", {
            month: "short",
            day: "numeric",
          })
        );
      }
      return dateObj.toLocaleDateString("en-US", {
        month: "short",
        day: "numeric",
      });
    });

    setSpedingStatsResponse(response.data);
    setDates(dates);
  };

  useEffect(() => {
    fetchExpenses();
  }, []);

  const colorPalette = [
    theme.palette.primary.light,
    theme.palette.primary.main,
    theme.palette.primary.dark,
  ];

  return (
    <Card variant="outlined" sx={{ width: "100%" }}>
      <CardContent>
        <Typography component="h2" variant="subtitle2" gutterBottom>
          Spending last 30 days
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
              {addCommasToNumber(spendingStatsReponse?.totalSpendLast30Days)}{" "}
              RON
            </Typography>
            <SpendingDiffChip
              diff={spendingStatsReponse?.percentageSpendingChange ?? 0}
            />
          </Stack>
          <Typography variant="caption" sx={{ color: "text.secondary" }}>
            Spending per day for the last 30 days
          </Typography>
        </Stack>
        {spedingStats && dates && (
          <LineChart
            colors={colorPalette}
            xAxis={[
              {
                scaleType: "point",
                data: dates,
                tickInterval: (_, i) => (i + 1) % 5 === 0,
              },
            ]}
            yAxis={[{ reverse: true }]}
            series={[{ ...spedingStats }]}
            height={250}
            margin={{ left: 50, right: 20, top: 20, bottom: 20 }}
            grid={{ horizontal: true }}
            sx={{
              "& .MuiAreaElement-series-organic": {
                fill: "url('#organic')",
              },
              "& .MuiAreaElement-series-referral": {
                fill: "url('#referral')",
              },
              "& .MuiAreaElement-series-direct": {
                fill: "url('#direct')",
              },
            }}
            slotProps={{
              legend: {
                hidden: true,
              },
            }}
          >
            <AreaGradient color={theme.palette.primary.dark} id="organic" />
            <AreaGradient color={theme.palette.primary.main} id="referral" />
            <AreaGradient color={theme.palette.primary.light} id="direct" />
          </LineChart>
        )}
      </CardContent>
    </Card>
  );
}
