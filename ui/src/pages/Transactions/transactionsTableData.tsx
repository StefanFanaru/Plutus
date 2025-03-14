import { Chip, IconButton, Stack, Tooltip } from "@mui/material";
import { GridCellParams, GridColDef } from "@mui/x-data-grid";
import CallSplitIcon from "@mui/icons-material/CallSplit";
import NotInterestedIcon from "@mui/icons-material/NotInterested";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import CallMergeIcon from "@mui/icons-material/CallMerge";
import { changeCategory, excludeFromAnalysis } from "./transactionsActions";
import { CategoryDto } from "../../common/dtos/CategoryDto";
import CategoryMenu from "./CategoryMenu";
import TransactionType from "../../common/transactionType";

function renderCategory(params: GridCellParams, categories: CategoryDto[]) {
  const transactionId = params.row.id;
  const categoryId = params.row.categoryId;
  const isFixedExpense = params.row.isFixedExpense;

  return (
    <CategoryMenu
      {...{
        disabled: params.row.isExcluded || params.row.isFixedExpense,
        isFixedExpense,
        categories,
        transactionId,
        categoryId,
        onChange: async (newCategoryId: string) => {
          const success = await changeCategory(transactionId, newCategoryId);
          if (success) {
            params.row.categoryId = newCategoryId;
            params.api.updateRows([params.row]);
          }
        },
        formSx: {
          m: 1,
          margin: 0,
          height: "100%",
          width: "100%",
          display: "flex",
          flexDirection: "row",
          alignItems: "center",
          justifyContent: "start",
        },
        selectSx: {
          height: "1.5rem",
          width: "100%",
          maxWidth: 150,
        },
      }}
    />
  );
}

function renderType(type: TransactionType) {
  switch (type) {
    case TransactionType.CardPayment:
      return "Card Payment";
    case TransactionType.CardRefund:
      return "Refund";
    case TransactionType.Transfer:
      return "Transfer";
    case TransactionType.RevolutPayment:
      return "Revolut Payment";
    default:
      return "Unknown";
  }
}
function renderTransactionAmount(amount: string, isExcluded: boolean) {
  function getColor(amount: number) {
    if (isExcluded) {
      return "default";
    }
    if (amount < 0) {
      return "error";
    }
    return "success";
  }

  const amountNumber = parseFloat(amount);
  return <Chip label={amount} color={getColor(amountNumber)} size="small" />;
}

function renderObligorName(name: string, displayName: string) {
  return (
    <Tooltip title={name}>
      <span>{displayName}</span>
    </Tooltip>
  );
}

function renderActions(params: GridCellParams, props: TransactionColumnsProps) {
  return (
    <Stack
      direction="row"
      spacing={1}
      alignItems="center"
      justifyItems="center"
      justifyContent="center"
      alignSelf="center"
      height="100%"
      width="100%"
    >
      <Tooltip
        title={
          params.row.isSplitItem
            ? "Merge into the original transaction"
            : "Split into multiple transactions"
        }
      >
        <span>
          {/* if i do control click on IconButton, it will execute undoTransactionSplit */}
          <IconButton
            disabled={params.row.isExcluded || params.row.isFixedExpense}
            onClick={(e) => {
              e.preventDefault();
              if (params.row.isSplitItem) {
                props.unsplitTransaction(params.row.id);
                return;
              }
              props.splitTransaction(params.row.id);
            }}
            sx={{ width: "1.75rem", height: "1.75rem" }}
            size="small"
            color="primary"
          >
            {params.row.isSplitItem ? <CallMergeIcon /> : <CallSplitIcon />}
          </IconButton>
        </span>
      </Tooltip>
      <Tooltip
        title={
          params.row.isExcluded
            ? "Include in analysis"
            : "Exclude from analysis"
        }
      >
        <span>
          <IconButton
            disabled={params.row.isFixedExpense}
            onClick={async () => {
              const success = await excludeFromAnalysis(
                params.row.id,
                !params.row.isExcluded,
              );
              if (success) {
                params.row.isExcluded = !params.row.isExcluded;
                params.api.updateRows([params.row]);
              }
            }}
            size="small"
            color="error"
            sx={{ width: "1.75rem", height: "1.75rem" }}
          >
            {params.row.isExcluded ? (
              <AddCircleOutlineIcon />
            ) : (
              <NotInterestedIcon />
            )}
          </IconButton>
        </span>
      </Tooltip>
    </Stack>
  );
}

export interface TransactionColumnsProps {
  categories: CategoryDto[];
  unsplitTransaction: (transactionId: string) => void;
  splitTransaction: (transactionId: string) => void;
}

export function getTransactionsColumns(
  props: TransactionColumnsProps,
): GridColDef[] {
  return [
    {
      field: "obligorDisplayName",
      headerName: "Obligor",
      flex: 1.5,
      minWidth: 180,
      maxWidth: 220,
      renderCell: (params) =>
        renderObligorName(
          params.row.obligorName,
          params.row.obligorDisplayName,
        ),
    },
    {
      field: "amount",
      headerName: "Amount",
      flex: 1,
      maxWidth: 90,
      headerAlign: "right",
      align: "right",
      minWidth: 90,
      renderCell: (params) =>
        renderTransactionAmount(params.value as any, params.row.isExcluded),
    },
    {
      field: "categoryName",
      headerName: "Category",
      minWidth: 170,
      flex: 1,
      renderCell: (params) => renderCategory(params, props.categories),
    },
    {
      field: "type",
      headerName: "Type",
      flex: 1,
      minWidth: 120,
      renderCell: (params) => renderType(params.value as any),
    },
    {
      field: "bookedAt",
      headerName: "Booked At",
      minWidth: 145,
      flex: 1,
    },
    {
      field: "id",
      headerName: "Actions",
      headerAlign: "center",
      align: "center",
      sortable: false,
      disableColumnMenu: true,
      filterable: false,
      renderCell: (params) => renderActions(params, props),
    },
  ];
}
