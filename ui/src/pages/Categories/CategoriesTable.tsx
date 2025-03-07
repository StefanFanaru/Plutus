import {
  DataGrid,
  GridFilterModel,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { categoryColumns } from "./categoriesTableData";
import { useEffect, useState } from "react";
import { addCommasToNumber, makePrettyDate } from "../../common/helpers";
import { ListRequest } from "../../common/dtos/ListRequest";
import { ListResponse } from "../../common/dtos/ListResponse";
import axiosClient from "../../axiosClient";

export interface CategoriesListItem {
  id: string;
  name: string;
  totalTransactionsCount: number;
  ammountCreditedThisMonth: number;
  totalAmmountCredited: number;
  latestTransaction: string;
}

export interface CategoryAmmountPerMonth {
  categoryId: string;
  ammountCreditedPerMonth: { [key: string]: number };
}

export interface CategoryFormattedListItem {
  id: string;
  name: string;
  monthlyAverage: string;
  ammountCreditedThisMonth: string;
  totalAmmountCredited: string;
  latestTransaction: string;
  amountPerMonth: number[];
}

export default function CategoriesTable() {
  const [categories, setCategories] = useState<CategoryFormattedListItem[]>([]);
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

  const [amountPerMonth, setAmountPerMonth] = useState<
    CategoryAmmountPerMonth[]
  >([]);

  const fetchItems = async () => {
    if (!listRequest) {
      return;
    }
    const response = await axiosClient.post<ListResponse<CategoriesListItem>>(
      "/api/Categories/list",
      listRequest,
    );
    setCategories(formatResponse(response.data.items));
    setTotalCount(response.data.totalCount);
    fetchCategoriesAmountPerMonth(response.data.items.map((item) => item.id));
  };

  const fetchCategoriesAmountPerMonth = async (
    amountPerMonthRequest: string[],
  ) => {
    const response = await axiosClient.post<CategoryAmmountPerMonth[]>(
      "/api/Categories/month-ammount",
      amountPerMonthRequest,
    );
    setAmountPerMonth(response.data);
  };

  useEffect(() => {
    fetchItems();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    setCategories((prev) =>
      prev.map((category) => {
        const amounts = amountPerMonth.find(
          (amount) => amount.categoryId === category.id,
        );

        const averageAmount =
          Object.values(amounts?.ammountCreditedPerMonth ?? {}).reduce(
            (acc, cur) => acc + cur,
            0,
          ) / Object.keys(amounts?.ammountCreditedPerMonth ?? {}).length;

        return {
          ...category,
          monthlyAverage: addCommasToNumber(
            isNaN(averageAmount) ? 0 : averageAmount,
          ),
          amountPerMonth: amounts
            ? Object.values(amounts.ammountCreditedPerMonth)
            : [],
        };
      }),
    );
  }, [amountPerMonth]);

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
    fetchItems();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [listRequest]);

  const formatResponse = (
    response: CategoriesListItem[],
  ): CategoryFormattedListItem[] => {
    return response.map((category) => {
      return {
        ...category,
        ammountCreditedThisMonth: addCommasToNumber(
          category.ammountCreditedThisMonth,
        ),
        totalAmmountCredited: addCommasToNumber(category.totalAmmountCredited),
        latestTransaction: makePrettyDate(category.latestTransaction),
        monthlyAverage: "",
        amountPerMonth: [],
      };
    });
  };

  return (
    <DataGrid
      // checkboxSelection
      rows={categories}
      disableRowSelectionOnClick
      filterModel={filterModel}
      onFilterModelChange={setFilterModel}
      paginationModel={paginationModel}
      onPaginationModelChange={setPaginationModel}
      sortModel={sortModel}
      onSortModelChange={setSortModel}
      columns={categoryColumns}
      paginationMode="server"
      sortingMode="server"
      filterMode="server"
      localeText={{
        noRowsLabel: "No rows matching this filter",
      }}
      rowCount={totalCount}
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
