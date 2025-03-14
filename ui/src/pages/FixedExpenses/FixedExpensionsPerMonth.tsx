import * as React from "react";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import { BarChart } from "@mui/x-charts/BarChart";
import { useTheme } from "@mui/material/styles";
import { useEffect } from "react";
import { addCommasToNumber, getLast12Months } from "../../common/helpers";

interface Props {
  spentByMonthItems: { value: number; month: string }[];
}

export default function FixedExpensionsPerMonth(props: Props) {
  const [monthsOfYear, setMonths] = React.useState<string[]>([]);

  const theme = useTheme();
  const colorPalette = [
    (theme.vars || theme).palette.primary.dark,
    (theme.vars || theme).palette.primary.main,
    (theme.vars || theme).palette.primary.light,
  ];

  useEffect(() => {
    if (!props.spentByMonthItems) {
      return;
    }
    setMonths(getLast12Months());

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.spentByMonthItems]);

  return (
    <Card variant="outlined" sx={{ width: "100%" }}>
      <CardContent>
        <Typography component="h2" variant="subtitle2" gutterBottom>
          Fixed expenses per month
        </Typography>
        <BarChart
          colors={colorPalette}
          // borderRadius={8}
          xAxis={
            [
              {
                scaleType: "band",
                categoryGapRatio: 0.5,
                data: monthsOfYear,
                tickPlacement: "middle",
              },
            ] as any
          }
          dataset={props.spentByMonthItems}
          series={[
            {
              dataKey: "value",
              label: "Total",
              valueFormatter: (value) => `${addCommasToNumber(value)} RON`,
            },
          ]}
          yAxis={[
            {
              reverse: true,
              dataKey: "value",
            },
          ]}
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
