import {
  DataGrid,
  GridFilterModel,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { getObligorsColumns } from "./tableData";
import { useEffect, useState } from "react";
import { addCommasToNumber, makePrettyDate } from "../../common/helpers";
import { ListResponse } from "../../common/dtos/ListResponse";
import { ListRequest } from "../../common/dtos/ListRequest";
import axiosClient from "../../axiosClient";

export interface ObligorsColumnsProps {
  onFixedExpenseChange: (obligorId: string, isFixedExpense: boolean) => void;
}

export interface ObligorListItem {
  id: string;
  name: string;
  transactionsThisMonthCount: number;
  totalTransactionsCount: number;
  ammountCreditedThisMonth: number;
  totalAmmountCredited: number;
  latestTransaction: string;
  isForFixedExpenses: boolean;
}

export interface ObligorAmmountPerMonth {
  obligorId: string;
  ammountCreditedPerMonth: { [key: string]: number };
}

export interface ObligorFormattedListItem {
  id: string;
  name: string;
  monthlyAmount: string;
  ammountCreditedThisMonth: string;
  totalAmmountCredited: string;
  latestTransaction: string;
  transactionsPerMonth: number[];
  isForFixedExpenses: boolean;
}

export default function ObligorsTable() {
  const [obligors, setObligors] = useState<ObligorFormattedListItem[]>([]);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [filterModel, setFilterModel] = useState<GridFilterModel>({
    items: [],
  });
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    pageSize: 20,
    page: 0,
  });

  const [sortModel, setSortModel] = useState<GridSortModel>([
    { field: "ammountCreditedThisMonth", sort: "asc" },
  ]);

  const [listRequest, setListRequest] = useState<ListRequest>();

  const [transactionsPerMonth, setTransactionsPerMonth] = useState<
    ObligorAmmountPerMonth[]
  >([]);

  const fetchObligors = async () => {
    if (!listRequest) {
      return;
    }
    const response = await axiosClient.post<ListResponse<ObligorListItem>>(
      "/api/Obligors/list",
      listRequest,
    );
    setObligors(formatResponse(response.data.items));
    setTotalCount(response.data.totalCount);
    fetchObligorsTransactionsPerMonth(
      response.data.items.map((obligor) => obligor.id),
    );
  };

  const fetchObligorsTransactionsPerMonth = async (
    transactionsPerMonthRequest: string[],
  ) => {
    const response = await axiosClient.post<ObligorAmmountPerMonth[]>(
      "/api/Obligors/month-ammount",
      transactionsPerMonthRequest,
    );
    setTransactionsPerMonth(response.data);
  };

  useEffect(() => {
    fetchObligors();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    setObligors((prev) =>
      prev.map((obligor) => {
        const amounts = transactionsPerMonth.find(
          (transaction) => transaction.obligorId === obligor.id,
        );
        const averageAmount =
          Object.values(amounts?.ammountCreditedPerMonth ?? {}).reduce(
            (acc, cur) => acc + cur,
            0,
          ) / Object.keys(amounts?.ammountCreditedPerMonth ?? {}).length;
        return {
          ...obligor,
          monthlyAverage: addCommasToNumber(
            isNaN(averageAmount) ? 0 : averageAmount,
          ),
          transactionsPerMonth: amounts
            ? Object.values(amounts.ammountCreditedPerMonth)
            : [],
        };
      }),
    );
  }, [transactionsPerMonth]);

  useEffect(() => {
    setListRequest((prev) => ({
      ...prev,
      pageNumber: paginationModel.page,
      pageSize: paginationModel.pageSize,
      sortField: sortModel[0]?.field,
      sortOrder: sortModel[0]?.sort,
      filter: {
        field: filterModel.items[0]?.field,
        operator: filterModel.items[0]?.operator,
        value: filterModel.items[0]?.value,
      },
    }));
  }, [filterModel, paginationModel, sortModel]);

  useEffect(() => {
    fetchObligors();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [listRequest]);

  const formatResponse = (
    response: ObligorListItem[],
  ): ObligorFormattedListItem[] => {
    return response.map((obligor) => {
      return {
        ...obligor,
        ammountCreditedThisMonth: addCommasToNumber(
          obligor.ammountCreditedThisMonth,
        ),
        totalAmmountCredited: addCommasToNumber(obligor.totalAmmountCredited),
        latestTransaction: makePrettyDate(obligor.latestTransaction),
        transactionsPerMonth: [],
        monthlyAmount: "",
      };
    });
  };

  function updateObligorFixedExpense(obligorId: string, value: boolean) {
    setObligors((prev) =>
      prev.map((obligor) => {
        if (obligor.id === obligorId) {
          return {
            ...obligor,
            isForFixedExpenses: value,
          };
        }

        return obligor;
      }),
    );
  }

  async function onObligorFixedChange(obligorId: string, value: boolean) {
    updateObligorFixedExpense(obligorId, value);
    const response = await axiosClient.post(
      "/api/Obligors/set-fixed-expense",
      {},
      {
        params: {
          obligorId,
          value,
        },
      },
    );

    if (response.status !== 200) {
      updateObligorFixedExpense(obligorId, value);
    }
  }

  return (
    <DataGrid
      // checkboxSelection
      rows={obligors}
      disableRowSelectionOnClick
      filterModel={filterModel}
      onFilterModelChange={setFilterModel}
      paginationModel={paginationModel}
      onPaginationModelChange={setPaginationModel}
      sortModel={sortModel}
      onSortModelChange={setSortModel}
      columns={getObligorsColumns({
        onFixedExpenseChange: onObligorFixedChange,
      })}
      paginationMode="server"
      sortingMode="server"
      filterMode="server"
      rowCount={totalCount}
      localeText={{
        noRowsLabel: "No rows matching this filter",
      }}
      getRowClassName={(params) =>
        params.indexRelativeToCurrentPage % 2 === 0 ? "even" : "odd"
      }
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
