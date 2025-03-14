import Chip from "@mui/material/Chip";

function SpendingDiffChip({ diff }: { diff: number }) {
  return (
    <Chip
      size="small"
      color={diff && diff > 0 ? "error" : diff == 0 ? "default" : "success"}
      label={(diff && diff > 0 ? "+" : "") + diff + "%"}
    />
  );
}

export default SpendingDiffChip;
