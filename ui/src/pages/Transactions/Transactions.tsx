import { Box } from "@mui/material";
import Grid from "@mui/material/Grid2";
import TransactionsTable, {
  TransactionFormattedListItem,
} from "./TransactionsTable";
import TransactionSplitter from "./TransacitonSplitter";
import { memo, useMemo, useState } from "react";
import { CategoryDto } from "../../common/dtos/CategoryDto";

interface Props {
  setTransactionToSplit: (transactionToSplit: string | null) => void;
  setTransactions: (transactions: TransactionFormattedListItem[]) => void;
  setCategories: (categories: CategoryDto[]) => void;
  onSplitSubmit: () => void;
  refreshToken: string;
}

const TransactionsTableMemo: React.FC<Props> = memo(
  ({
    setTransactionToSplit,
    setTransactions,
    setCategories,
    onSplitSubmit,
    refreshToken,
  }) => (
    <TransactionsTable
      setCategories={setCategories}
      setTransactions={setTransactions}
      setTransactionToSplit={setTransactionToSplit}
      onSplitSubmit={onSplitSubmit}
      refreshToken={refreshToken}
    />
  ),
);

function Transactions() {
  const [transactions, setTransactions] = useState<
    TransactionFormattedListItem[]
  >([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [refreshToken, setRefreshToken] = useState<string>();
  const [transactionToSplit, setTransactionToSplit] = useState<string | null>();

  function onSplitSubmit() {
    setTransactionToSplit(null);
    setRefreshToken(new Date().toISOString());
  }

  const data = useMemo(() => {
    return {
      setTransactionToSplit,
      setTransactions,
      setCategories,
      onSplitSubmit,
      refreshToken,
    } as Props;
  }, [refreshToken]);

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Grid container spacing={2} columns={5}>
        <Grid size={{ xs: 12, lg: 9 }}>
          <TransactionsTableMemo {...data} />
          {transactionToSplit && (
            <TransactionSplitter
              categories={categories}
              transactionToSplit={transactionToSplit}
              transactions={transactions}
              onSubmit={onSplitSubmit}
            />
          )}
        </Grid>
      </Grid>
    </Box>
  );
}

export default Transactions;
