import Stack from "@mui/material/Stack";
import MenuButton from "./MenuButton";
import ColorModeIconDropdown from "./ColorModeIconDropdown";
import { Link, Typography } from "@mui/material";
import { useLocation } from "react-router";
import { useEffect, useState } from "react";
import CustomDateRangePicker from "../custom/range-picker/CustomDateRangePicker";
import NavMenuItems from "./NavMenuItems";
import GitHubIcon from "@mui/icons-material/GitHub";

export default function Header() {
  const location = useLocation();
  const [headerText, setHeaderText] = useState("");
  useEffect(() => {
    const text = NavMenuItems.find(
      (item) => item.path === location.pathname,
    )?.text;
    setHeaderText((text ?? "") == "Dashboard" ? "" : text!);
  }, [location]);

  return (
    <Stack
      direction="row"
      sx={{
        display: { xs: "none", md: "flex" },
        width: "100%",
        alignItems: { xs: "flex-start", md: "center" },
        justifyContent: "space-between",
        maxWidth: { sm: "100%", md: "1700px" },
        pt: 1.5,
      }}
      spacing={2}
    >
      {/* <NavbarBreadcrumbs /> */}
      <Typography component="h2" variant="h5" sx={{ mb: 2 }}>
        {headerText}
      </Typography>
      <Stack direction="row" sx={{ gap: 1 }}>
        {/* <Search /> */}
        {/* <CustomDatePicker /> */}
        {/* <DatePickerButton /> */}
        <CustomDateRangePicker />
        <Link
          underline="none"
          href="https://github.com/StefanFanaru/Plutus"
          target="_blank"
        >
          <MenuButton aria-label="Open notifications">
            <GitHubIcon />
          </MenuButton>
        </Link>
        <ColorModeIconDropdown />
      </Stack>
    </Stack>
  );
}
