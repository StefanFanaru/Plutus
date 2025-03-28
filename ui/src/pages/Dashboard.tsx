import Grid from "@mui/material/Grid2";
import Box from "@mui/material/Box";
import StatCard, { StatCardProps } from "../components/mui/StatCard";
import Copyright from "../components/mui/internals/components/Copyright";
import DashboardSpendingStats from "../components/mui/DashboardSpendingStats";
import { useEffect, useState } from "react";
import { addCommasToNumber, makePrettyDate } from "../common/helpers";
import DashboardInfoCard, {
  DashboardInfoCardProps,
} from "../components/mui/DashBoardInfoCard";
import axiosClient from "../axiosClient";
import DashboardSpendingThisWeek from "../components/mui/DashboardSpendingThisWeek";
import DashboardSpendingByCategory from "../components/mui/DashboardSpendingByCategory";
import DashboardSpendingByObligor from "../components/mui/DashboardSpendingByObligor";

export interface DashboardStats {
  balanceDetails: BalanceDetails;
  lastTransaction: LastTransaction;
}

export interface BalanceDetails {
  balance: number;
  recordedAt: string;
  balancePerDay: number[];
}

export interface LastTransaction {
  obligorName: string;
  bookingDate: string;
  amount: number;
  isCredit: boolean;
}

export default function Dashboard() {
  const [balanceDetails, setBalanceDetails] = useState<StatCardProps | null>(
    null,
  );
  const [lastTransaction, setLastTransaction] =
    useState<DashboardInfoCardProps | null>(null);
  const fetchBudget = async () => {
    const response = await axiosClient.get<DashboardStats>(
      "/api/Dashboard/stats-cards",
    );

    const balanceDetails = response.data.balanceDetails;
    if (!balanceDetails) {
      return;
    }
    setBalanceDetails({
      title: "Balance",
      value: addCommasToNumber(balanceDetails.balance, false) + " RON",
      interval: makePrettyDate(balanceDetails.recordedAt),
      trend: {
        color: "neutral",
        label: "30 days",
      },
      data: balanceDetails.balancePerDay,
    });

    const lastTransaction = response.data.lastTransaction;
    if (!lastTransaction) {
      return;
    }
    setLastTransaction({
      title: "Latest Transaction",
      value: addCommasToNumber(lastTransaction.amount, false) + " RON",
      obligorName: lastTransaction.obligorName,
      bookingDate: makePrettyDate(lastTransaction.bookingDate),
      isCredit: lastTransaction.isCredit,
    });
  };

  useEffect(() => {
    fetchBudget();
  }, []);

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Grid
        container
        spacing={2}
        columns={12}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >
        <Grid
          size={{
            xs: 12,
            sm: lastTransaction ? 6 : 12,
            lg: lastTransaction ? 9 : 12,
          }}
        >
          {balanceDetails && <StatCard {...balanceDetails} />}
        </Grid>
        {lastTransaction && (
          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <DashboardInfoCard {...lastTransaction} />
          </Grid>
        )}
        <Grid size={{ xs: 12, md: 6 }}>
          <DashboardSpendingStats />
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <DashboardSpendingThisWeek />
        </Grid>
      </Grid>
      <Grid container spacing={2} columns={12}>
        <Grid size={{ xs: 12, lg: 6 }}>
          <DashboardSpendingByCategory />
        </Grid>
        <Grid size={{ xs: 12, lg: 6 }}>
          <DashboardSpendingByObligor />
        </Grid>
      </Grid>
      <Copyright sx={{ my: 4 }} />
    </Box>
  );
}
