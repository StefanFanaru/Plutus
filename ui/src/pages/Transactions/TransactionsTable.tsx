import {
  DataGrid,
  GridColDef,
  GridFilterModel,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { getTransactionsColumns } from "./transactionsTableData";
import { useEffect, useState } from "react";
import { addCommasToNumber, makePrettyDate } from "../../common/helpers";
import { ListRequest } from "../../common/dtos/ListRequest";
import { ListResponse } from "../../common/dtos/ListResponse";
import { CategoryDto } from "../../common/dtos/CategoryDto";
import { TransactionSplitItem } from "./TransacitonSplitter";
import "./TransactionsTable.css";
import axiosClient, { datePickerFilter } from "../../axiosClient";
import Constants from "../../common/constants";
import { TransactionListItem } from "../../common/dtos/TransactionListItem";

export interface TransactionFormattedListItem {
  id: string;
  bookedAt: string;
  amount: string;
  type: number;
  obligorName: string;
  categoryName: string;
  isExcluded: boolean;
  isSplitItem: boolean;
  isFixedExpense: boolean;
}

interface Props {
  setTransactionToSplit: (transactionToSplit: string | null) => void;
  setTransactions: (transactions: TransactionFormattedListItem[]) => void;
  setCategories: (categories: CategoryDto[]) => void;
  onSplitSubmit: (
    transactionId: string,
    splits: TransactionSplitItem[],
  ) => void;
  refreshToken: string;
}

interface State {
  categories: CategoryDto[];
  totalCount: number;
  transactions: TransactionFormattedListItem[];
  transactionsColumns: GridColDef[];
  filterModel: GridFilterModel;
  paginationModel: GridPaginationModel;
  sortModel: GridSortModel;
  listRequest?: ListRequest;
  internalRefreshToken?: string;
}

export default function TransactionsTable(props: Props) {
  const [state, setState] = useState<State>({
    categories: [],
    totalCount: 0,
    transactions: [],
    transactionsColumns: [],
    filterModel: { items: [] },
    paginationModel: { pageSize: 20, page: 0 },
    sortModel: [{ field: "bookedAt", sort: "desc" }],
    listRequest: undefined,
  });

  async function fetchData() {
    if (!state.listRequest) {
      return;
    }

    const response = await axiosClient.post<ListResponse<TransactionListItem>>(
      "/api/Transactions/list",
      state.listRequest,
    );

    const responseCategories = await axiosClient.get<CategoryDto[]>(
      "/api/Categories/all",
    );

    const categoriesDtos = responseCategories.data.filter(
      (category) =>
        category.id != Constants.UncategorizedCategoryId &&
        category.id != Constants.FixedCategoryId,
    );

    const formattedTransactions = formatResponse(response.data.items);
    setState((prev) => ({
      ...prev,
      categories: categoriesDtos,
      transactionsColumns: getTransactionsColumns({
        categories: categoriesDtos,
        splitTransaction: props.setTransactionToSplit,
        unsplitTransaction: unsplitTransaction,
      }),
      transactions: formattedTransactions,
      totalCount: response.data.totalCount,
    }));

    props.setTransactions(formattedTransactions);
    props.setCategories(categoriesDtos);
  }

  useEffect(() => {
    fetchData();
    datePickerFilter.subscribe((_) => {
      setState((prev) => ({
        ...prev,
        internalRefreshToken: new Date().toISOString(),
      }));
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (state.internalRefreshToken) {
      fetchData();
    }
    if (props.refreshToken) {
      fetchData();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.refreshToken, state.internalRefreshToken]);

  useEffect(() => {
    setState((prev) => ({
      ...prev,
      listRequest: {
        pageNumber: state.paginationModel.page,
        pageSize: state.paginationModel.pageSize,
        sortField: state.sortModel[0]?.field,
        sortOrder: state.sortModel[0]?.sort,
        filter: {
          field: state.filterModel.items[0]?.field,
          operator: state.filterModel.items[0]?.operator,
          value: state.filterModel.items[0]?.value,
        },
      },
    }));
  }, [state.filterModel, state.paginationModel, state.sortModel]);

  useEffect(() => {
    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state.listRequest]);

  const formatResponse = (
    response: TransactionListItem[],
  ): TransactionFormattedListItem[] => {
    return response.map((item) => {
      return {
        ...item,
        amount: addCommasToNumber(item.amount),
        bookedAt: makePrettyDate(item.bookedAt),
      };
    });
  };

  async function unsplitTransaction(transactionId: string) {
    await axiosClient.post(
      "/api/Transactions/unsplit",
      {},
      {
        params: {
          transactionId,
        },
      },
    );

    fetchData();
  }

  return (
    <DataGrid
      // checkboxSelection
      rows={[...state.transactions]}
      filterModel={state.filterModel}
      onFilterModelChange={(model) =>
        setState((prev) => ({ ...prev, filterModel: model }))
      }
      paginationModel={state.paginationModel}
      onPaginationModelChange={(model) =>
        setState((prev) => ({ ...prev, paginationModel: model }))
      }
      sortModel={state.sortModel}
      onSortModelChange={(model) =>
        setState((prev) => ({ ...prev, sortModel: model }))
      }
      columns={state.transactionsColumns}
      paginationMode="server"
      sortingMode="server"
      filterMode="server"
      localeText={{
        noRowsLabel: "No rows matching this filter",
      }}
      rowCount={state.totalCount}
      disableRowSelectionOnClick
      getRowClassName={(params) => {
        const excluded = params.row.isExcluded ? `plutus-theme--excluded` : "";
        const fixedExpense = params.row.isFixedExpense
          ? `plutus-theme--fixed-expense`
          : "";

        return `${excluded} ${fixedExpense}`;
      }}
      pageSizeOptions={[10, 20, 50]}
      // disableColumnResize
      density="compact"
      slotProps={{
        filterPanel: {
          sx: {
            backgroundColor: "background.paper",
          },
        },
        pagination: {
          showFirstButton: true,
          showLastButton: true,
        },
      }}
    />
  );
}
