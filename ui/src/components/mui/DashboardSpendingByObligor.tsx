import * as React from "react";
import { pieArcLabelClasses, PieChart } from "@mui/x-charts/PieChart";
import Typography from "@mui/material/Typography";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import LinearProgress, {
  linearProgressClasses,
} from "@mui/material/LinearProgress";

import axiosClient from "../../axiosClient";
import { useEffect } from "react";
import { addCommasToNumber } from "../../common/helpers";

interface SpentByObligorItem {
  obligorId: string;
  obligorName: string;
  amount: number;
  percentage: number;
}

interface SpendingData {
  items: SpentByObligorItem[];
}

interface GraphData {
  label: string;
  value: number;
}

interface GraphItem {
  name: string;
  value: number;
  // flag: JSX.Element;
  color: string;
}

const colors = [
  "hsl(220, 20%, 100%)",
  "hsl(220, 20%, 80%)",
  "hsl(220, 20%, 60%)",
  "hsl(220, 20%, 40%)",
  "hsl(220, 20%, 25%)",
  "hsl(220, 20%, 25%)",
  "hsl(220, 20%, 25%)",
];

export default function DashboardSpendingByCategory() {
  const [graphData, setGraphData] = React.useState<GraphData[]>([]);
  const [graphItems, setGraphItems] = React.useState<GraphItem[]>([]);

  useEffect(() => {
    fetch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetch = async () => {
    const response = await axiosClient.get<SpendingData>(
      "/api/Dashboard/spending-by-category",
    );
    const data = response.data.items.map((item) => ({
      label: item.obligorName,
      value: Math.abs(item.amount),
    }));
    setGraphData(data);

    const countries = response.data.items.map((item, index) => ({
      name: item.obligorName,
      value: item.percentage,
      color: colors[index],
    }));
    setGraphItems(countries);
  };

  return (
    <Card
      variant="outlined"
      sx={{
        display: "flex",
        flexDirection: "column",
        gap: "8px",
        flexGrow: 1,
        width: "100%",
      }}
    >
      <CardContent>
        <Typography component="h2" variant="subtitle2">
          Spending by category
        </Typography>
        <Typography variant="caption" sx={{ color: "text.secondary" }}>
          Spening by category for the last 30 days
        </Typography>
        <Box sx={{ display: "flex", alignItems: "center", width: "100%" }}>
          <PieChart
            colors={colors}
            margin={{
              left: 80,
              right: 80,
              top: 80,
              bottom: 80,
            }}
            sx={{
              [`& .${pieArcLabelClasses.root}`]: {
                fill: "black",
              },
            }}
            series={[
              {
                data: graphData,
                arcLabel: (item) => `${item.label}`,
                arcLabelMinAngle: 35,
                outerRadius: 120,
                highlightScope: { faded: "global", highlighted: "item" },
                valueFormatter: (value) =>
                  addCommasToNumber(value.value * -1) + " RON",
              },
            ]}
            height={270}
            width={270}
            slotProps={{
              legend: { hidden: true },
            }}
          ></PieChart>
        </Box>
        {graphItems.map((country, index) => (
          <Stack
            key={index}
            direction="row"
            sx={{ alignItems: "center", gap: 2, pb: 2 }}
          >
            {/* {country.flag} */}
            <Stack sx={{ gap: 1, flexGrow: 1 }}>
              <Stack
                direction="row"
                sx={{
                  justifyContent: "space-between",
                  alignItems: "center",
                  gap: 2,
                }}
              >
                <Typography variant="body2" sx={{ fontWeight: "500" }}>
                  {country.name}
                </Typography>
                <Typography variant="body2" sx={{ color: "text.secondary" }}>
                  {country.value}%
                </Typography>
              </Stack>
              <LinearProgress
                variant="determinate"
                aria-label="Number of users by country"
                value={country.value}
                sx={{
                  [`& .${linearProgressClasses.bar}`]: {
                    backgroundColor: country.color,
                  },
                }}
              />
            </Stack>
          </Stack>
        ))}
      </CardContent>
    </Card>
  );
}
