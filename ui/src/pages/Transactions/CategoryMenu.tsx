import { FormControl, MenuItem, Select, SxProps, Theme } from "@mui/material";
import { CategoryDto } from "../../common/dtos/CategoryDto";
import "./CategoryMenu.css";
import Constants from "../../common/constants";

export interface CategoryMenuProps {
  onChange: (categoryId: string) => Promise<void>;
  categories: CategoryDto[];
  transactionId: string;
  categoryId: string;
  disabled: boolean;
  formSx: SxProps<Theme>;
  selectSx: SxProps<Theme>;
  isFixedExpense: boolean;
}

function CategoryMenu(props: CategoryMenuProps) {
  return (
    <FormControl size="small" sx={props.formSx}>
      <Select
        labelId="demo-simple-select-label"
        disabled={props.disabled}
        id="category-select"
        classes={{
          select:
            props.categoryId === Constants.UncategorizedCategoryId
              ? "category-select-uncat"
              : "",
        }}
        value={props.categoryId}
        label="Age"
        sx={props.selectSx}
        onChange={async (value) => {
          const newCategoryId = value.target.value as string;
          await props.onChange(newCategoryId);
        }}
      >
        {props.isFixedExpense && (
          <MenuItem value={Constants.FixedCategoryId} disabled>
            Fixed
          </MenuItem>
        )}
        <MenuItem value={Constants.UncategorizedCategoryId}>
          Uncategorized
        </MenuItem>
        {props.categories.map((category) => (
          <MenuItem key={category.id} value={category.id}>
            {category.name}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}

export default CategoryMenu;
