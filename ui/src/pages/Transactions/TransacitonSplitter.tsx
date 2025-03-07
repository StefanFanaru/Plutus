import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import { TransactionFormattedListItem } from "./TransactionsTable";
import { useEffect, useState } from "react";
import { Chip, IconButton, Stack } from "@mui/material";
import Grid from "@mui/material/Grid2";
import CategoryMenu from "./CategoryMenu";
import { CategoryDto } from "../../common/dtos/CategoryDto";
import Constants from "../../common/constants";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import axiosClient from "../../axiosClient";

export interface TransactionSplitterProps {
  transactionToSplit: string | null;
  transactions: TransactionFormattedListItem[] | null;
  categories: CategoryDto[];
  onSubmit: () => void;
}

export interface TransactionSplitItem {
  amount?: number;
  categoryId: string;
}

interface State {
  isVisible: boolean;
  splits: TransactionSplitItem[];
  amountLeft?: number;
  transaction?: TransactionFormattedListItem | null;
}

export default function TransactionSplitter(props: TransactionSplitterProps) {
  const [state, setState] = useState<State>({ isVisible: true, splits: [] });

  useEffect(() => {
    if (!props.transactionToSplit) {
      return;
    }

    setState((state) => {
      const transaction = props.transactions?.find(
        (t) => t.id === props.transactionToSplit,
      );
      return {
        ...state,
        transaction,
        amountLeft: Math.abs(
          parseFloat(transaction?.amount?.replace(",", ".") ?? "0"),
        ),
        splits: [
          {
            amount: 0,
            categoryId: Constants.UncategorizedCategoryId,
          },
          {
            amount: undefined,
            categoryId: Constants.UncategorizedCategoryId,
          },
        ],
      } as State;
    });
  }, [props]);

  const handleSplit = async () => {
    // post the splits
    await axiosClient.post("/api/transactions/split", {
      transactionId: state.transaction?.id,
      splits: state.splits.filter((split) => split.amount && split.amount > 0),
    });

    props.onSubmit();
  };

  function handleSplitChange(
    index: number,
    amount: number | undefined,
    categoryId: string,
  ) {
    const newSplits = [...state.splits];
    newSplits[index] = { amount, categoryId };
    const left = calculateLeftAmount(state, newSplits);

    setState({ ...state, splits: newSplits, amountLeft: left });
  }

  function handleAddSplit() {
    setState({
      ...state,
      splits: [
        ...state.splits,
        {
          amount: 0,
          categoryId: Constants.UncategorizedCategoryId,
        },
      ],
    });
  }

  function handleDeleteSplit(index: number) {
    const newSplits = [...state.splits];
    newSplits.splice(index, 1);
    const left = calculateLeftAmount(state, newSplits);
    setState({ ...state, splits: newSplits, amountLeft: left });
  }

  return (
    <Dialog
      open={props.transactionToSplit !== null && state.isVisible}
      onClose={props.onSubmit}
      maxWidth="md"
    >
      <DialogTitle>
        Split the transaction with "{state.transaction?.obligorName}"
      </DialogTitle>
      <DialogContent>
        <DialogContentText sx={{ mb: "0.5rem" }}>
          You can split this transaction into multiple categories with different
          amounts.
        </DialogContentText>
        Amount left uncategorized
        <Chip
          size="medium"
          label={state.amountLeft}
          color={
            state.amountLeft === 0
              ? "success"
              : state.amountLeft! < 0
                ? "error"
                : "default"
          }
          sx={{ marginLeft: "0.5rem" }}
        />
        {state.splits.map((split, index) => (
          <React.Fragment key={index}>
            <Grid
              key={index}
              container
              direction={{ xs: "column", sm: "row" }}
              size={{ xs: 12, sm: 6 }}
              spacing={2}
            >
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  autoFocus
                  margin="dense"
                  id="amount"
                  name="amount"
                  error={!!split.amount && split.amount <= 0}
                  label="Amount"
                  type="number"
                  fullWidth
                  variant="standard"
                  onChange={(event) => {
                    handleSplitChange(
                      index,
                      parseFloat(event.target.value),
                      split.categoryId,
                    );
                  }}
                />
              </Grid>
              <Grid
                container
                alignItems="end"
                size={{ xs: 12, sm: 6 }}
                spacing={2}
              >
                <Grid size={10}>
                  <CategoryMenu
                    {...{
                      disabled: false,
                      categories: props.categories,
                      transactionId: state.transaction?.id || "",
                      isFixedExpense: false,
                      categoryId: split.categoryId,
                      onChange: async (newCategoryId: string) => {
                        handleSplitChange(index, split.amount, newCategoryId);
                      },
                      formSx: {
                        paddingBottom: "5px",
                        height: "100%",
                        width: "100%",
                        display: "flex",
                        flexDirection: "row",
                        alignItems: "end",
                        justifyContent: "end",
                        backgroundColor: "unset",
                      },
                      selectSx: {
                        height: "2.5rem",
                        width: "100%",
                        backgroundColor: "unset",
                      },
                    }}
                  />
                </Grid>
                <Grid size={2} sx={{ paddingBottom: "5px" }}>
                  <IconButton
                    disabled={state.splits.length <= 2}
                    onClick={() => handleDeleteSplit(index)}
                    aria-label="delete"
                    color="primary"
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </Grid>
              </Grid>
            </Grid>
          </React.Fragment>
        ))}
      </DialogContent>
      <DialogActions>
        <Stack direction="row" justifyContent="space-between" width="100%">
          <Button
            onClick={handleAddSplit}
            variant="text"
            startIcon={<AddIcon />}
          >
            Add Split
          </Button>
          <Stack direction="row" spacing={2}>
            <Button onClick={props.onSubmit}>Cancel</Button>
            <Button
              onClick={handleSplit}
              disabled={
                state.amountLeft! < 0 ||
                state.splits.filter((split) => split.amount && split.amount > 0)
                  .length == 0 ||
                state.splits.some((split) => split.amount && split.amount <= 0)
              }
              type="submit"
              variant="outlined"
            >
              Split
            </Button>
          </Stack>
        </Stack>
      </DialogActions>
    </Dialog>
  );
}
function calculateLeftAmount(state: State, newSplits: TransactionSplitItem[]) {
  return (
    Math.abs(parseFloat(state.transaction!.amount.replace(",", "."))) -
    newSplits.reduce(
      (acc, split) => acc + (!split.amount ? 0 : split.amount),
      0,
    )
  );
}
