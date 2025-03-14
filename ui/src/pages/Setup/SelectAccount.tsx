import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Grid2";
import axiosClient from "../../axiosClient";
import { useEffect, useState } from "react";
import { TransactionListItem } from "../../common/dtos/TransactionListItem";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { addCommasToNumber, makePrettyDate } from "../../common/helpers";
import "./SelectAccount.css";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import { useNavigate } from "react-router";
import { green } from "@mui/material/colors";
import Loader from "../../components/custom/loader/Loader";
import Globals from "../../common/globals";
import { UserStatus } from "../../common/dtos/User";

interface TransacitonTableRow {
  bookedAt: string;
  amount: number;
  obligorName: string;
}

interface AccountResponseItem {
  accountId: string;
  iban: string;
  currency: string;
  transactions: TransactionListItem[];
}

interface Response {
  accounts: AccountResponseItem[];
}

interface State {
  accounts: AccountResponseItem[];
  transacitonTableRows: { [key: string]: TransacitonTableRow[] };
  isSelectingAccount?: boolean;
  selectedAccountId?: string;
  isAccountsLoading?: boolean;
}
function SelectAccount() {
  const navigate = useNavigate();
  const [state, setState] = useState<State>({
    accounts: [],
    transacitonTableRows: {},
  });

  async function fetchAccounts() {
    setState((prev) => ({
      ...prev,
      isAccountsLoading: true,
    }));
    const response = await axiosClient.get<Response>(
      "/api/Setup/list-accounts",
      { timeout: 30000 },
    );

    setState((prev) => ({
      ...prev,
      isAccountsLoading: false,
    }));

    const transacitonTableRows: { [key: string]: TransacitonTableRow[] } = {};
    response.data.accounts.forEach((account: AccountResponseItem) => {
      transacitonTableRows[account.accountId] = account.transactions.map(
        (transaction) => ({
          bookedAt: transaction.bookedAt,
          amount: transaction.amount,
          obligorName: transaction.obligorName,
        }),
      );
    });

    setState({
      accounts: response.data.accounts,
      transacitonTableRows: transacitonTableRows,
    });

    console.log(state);
  }

  useEffect(() => {
    fetchAccounts();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function onAccountSelected(accountId: string) {
    setState((prev) => ({
      ...prev,
      selectedAccountId: accountId,
    }));
  }

  async function onSaveClick() {
    setState((prev) => ({
      ...prev,
      isSelectingAccount: true,
    }));

    const response = await axiosClient.post(
      "/api/Setup/select-account",
      {},
      {
        timeout: 60000,
        params: {
          accountId: state.selectedAccountId,
        },
      },
    );

    if (response.status === 200) {
      Globals.appUser!.status = UserStatus.RevolutConfirmed;
      navigate("/");
    }
  }

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      {state.isAccountsLoading ? (
        <Loader />
      ) : (
        <Grid
          container
          spacing={2}
          columns={5}
          alignItems="center"
          justifyContent="center"
        >
          <Grid size={{ xs: 12, lg: 9, xl: 3 }}>
            {/* */}
            <Card variant="outlined">
              <CardContent>
                <Typography variant="h5" component="div">
                  Select your Revolut account
                </Typography>
                <Typography variant="body2">
                  <br />
                  Plutus has detected that you have multiple Revolut accounts.
                  <br />
                  Please select the account you would like to use with Plutus.
                  <br />
                  You can always change this later in the settings.
                </Typography>
              </CardContent>
            </Card>
            {state.accounts.map((account) => (
              <Card
                sx={{
                  mt: "1rem",
                }}
                className={
                  state.selectedAccountId === account.accountId
                    ? "selected-account"
                    : ""
                }
                variant="outlined"
                key={account.accountId}
              >
                <CardContent>
                  <Grid container justifyContent="space-between" columns={12}>
                    <Grid>
                      <Typography variant="h6" component="div">
                        {account.currency} Account
                      </Typography>
                      <Typography variant="body2">
                        IBAN - {account.iban}
                      </Typography>
                    </Grid>
                    <Grid>
                      {state.selectedAccountId == account.accountId && (
                        <CheckCircleOutlineIcon
                          fontSize="large"
                          color="success"
                        />
                      )}
                    </Grid>
                  </Grid>
                  {state.transacitonTableRows[account.accountId].length > 0 && (
                    <Accordion
                      className={
                        state.selectedAccountId === account.accountId
                          ? "selected-account"
                          : ""
                      }
                      sx={{ mt: "1rem" }}
                    >
                      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                        <Typography component="span">
                          Recent transactions
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails sx={{ padding: "none" }}>
                        <TableContainer
                          className={
                            state.selectedAccountId === account.accountId
                              ? "selected-account"
                              : ""
                          }
                        >
                          <Table size="small" aria-label="simple table">
                            <TableHead>
                              <TableRow>
                                <TableCell
                                  sx={{
                                    fontWeight: "bold",
                                    minWidth: "180px",
                                    maxWidth: "220px",
                                  }}
                                >
                                  Obligor Name
                                </TableCell>
                                <TableCell
                                  sx={{ fontWeight: "bold" }}
                                  align="left"
                                >
                                  Amount
                                </TableCell>
                                <TableCell
                                  align="left"
                                  sx={{
                                    fontWeight: "bold",
                                    minWidth: "145px",
                                  }}
                                >
                                  Booked at
                                </TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {state.transacitonTableRows[
                                account.accountId
                              ].map((row) => (
                                <TableRow
                                  key={row.bookedAt}
                                  sx={{
                                    "&:last-child td, &:last-child th": {
                                      border: 0,
                                    },
                                  }}
                                >
                                  <TableCell>{row.obligorName}</TableCell>
                                  <TableCell align="left">
                                    <Chip
                                      label={addCommasToNumber(row.amount)}
                                      color="error"
                                      size="small"
                                    />
                                  </TableCell>
                                  <TableCell align="left">
                                    {makePrettyDate(row.bookedAt)}
                                  </TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </TableContainer>
                      </AccordionDetails>
                    </Accordion>
                  )}
                </CardContent>
                <Button
                  sx={{ mt: "1rem" }}
                  disabled={state.selectedAccountId === account.accountId}
                  onClick={() => onAccountSelected(account.accountId)}
                  variant="outlined"
                  color="secondary"
                  component="label"
                  size="small"
                >
                  Select
                </Button>
              </Card>
            ))}
            <Box sx={{ mt: 1, display: "flex", alignItems: "center" }}>
              <Box sx={{ m: 1, position: "relative" }}>
                <Button
                  variant="contained"
                  color="secondary"
                  disabled={
                    state.isSelectingAccount || !state.selectedAccountId
                  }
                  onClick={onSaveClick}
                >
                  Save selection
                </Button>
                {state.isSelectingAccount && (
                  <CircularProgress
                    size={24}
                    sx={{
                      color: green[500],
                      position: "absolute",
                      top: "50%",
                      left: "50%",
                      marginTop: "-12px",
                      marginLeft: "-12px",
                    }}
                  />
                )}
              </Box>
            </Box>
            {state.isSelectingAccount && (
              <Typography variant="body2" sx={{ mt: 1 }}>
                Loading Revolut account data... Do not close or refresh the
                page.
              </Typography>
            )}
          </Grid>
        </Grid>
      )}
    </Box>
  );
}

export default SelectAccount;
