import { useEffect, useState } from "react";
import axiosClient from "../../axiosClient";
import { TransactionListItem } from "../../common/dtos/TransactionListItem";
import FixedExpensionsPerMonth from "./FixedExpensionsPerMonth";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import Grid from "@mui/material/Grid2";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  Card,
  CardContent,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  addCommasToNumber,
  getLast12Months,
  makePrettyDate,
  shortMonthsMapping,
} from "../../common/helpers";

interface ResponseItem {
  value: number;
  year: number;
  monthName: string;
  transactions: TransactionListItem[];
}

interface State {
  response?: ResponseItem[];
  barChardDataSet?: { value: number; month: string }[];
}

function FixedExpenses() {
  const [state, setState] = useState<State>({});

  async function fetch() {
    const response = await axiosClient.get<ResponseItem[]>(
      "/api/FixedExpenses/expenses-per-month",
    );
    const orderedItems = getLast12Months()
      .map((month) => response.data.find((item) => item.monthName === month)!)
      .reverse();
    setState({
      response: orderedItems,
      barChardDataSet: orderedItems.map((item) => {
        return {
          value: item.value,
          month: item.monthName,
        };
      }),
    });
  }
  useEffect(() => {
    fetch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Grid container columns={12}>
        <Grid size={{ xs: 12 }}>
          {state.barChardDataSet && (
            <FixedExpensionsPerMonth
              spentByMonthItems={state.barChardDataSet}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12 }}>
          {state.response &&
            state.response.map((month) => (
              <Card
                sx={{
                  mt: "1rem",
                }}
                variant="outlined"
                key={month.monthName}
              >
                <CardContent>
                  <Grid container justifyContent="space-between" columns={12}>
                    <Grid>
                      <Typography variant="h6" component="div">
                        {shortMonthsMapping[month.monthName]} {month.year}
                        <Chip
                          sx={{ ml: 1 }}
                          label={addCommasToNumber(month.value)}
                          color="error"
                          size="medium"
                        />
                      </Typography>
                    </Grid>
                  </Grid>
                  <Accordion sx={{ mt: "1rem" }}>
                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                      <Typography component="span">Transactions</Typography>
                    </AccordionSummary>
                    <AccordionDetails sx={{ padding: "none" }}>
                      <TableContainer>
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
                            {month.transactions.map((row) => (
                              <TableRow
                                key={row.bookedAt}
                                sx={{
                                  "&:last-child td, &:last-child th": {
                                    border: 0,
                                  },
                                }}
                              >
                                <TableCell>
                                  <Tooltip title={row.obligorName}>
                                    <span>{row.obligorDisplayName}</span>
                                  </Tooltip>
                                </TableCell>
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
                </CardContent>
              </Card>
            ))}
        </Grid>
      </Grid>
    </Box>
  );
}

export default FixedExpenses;
