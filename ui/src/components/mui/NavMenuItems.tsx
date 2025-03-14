import HomeRoundedIcon from "@mui/icons-material/HomeRounded";
import PeopleRoundedIcon from "@mui/icons-material/PeopleRounded";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";
import CategoryIcon from "@mui/icons-material/Category";
import EventNoteIcon from "@mui/icons-material/EventNote";

const NavMenuItems = [
  { text: "Dashboard", path: "/", icon: <HomeRoundedIcon /> },
  { text: "Transactions", path: "/transactions", icon: <ReceiptLongIcon /> },
  { text: "Obligors", path: "/obligors", icon: <PeopleRoundedIcon /> },
  { text: "Categories", path: "/categories", icon: <CategoryIcon /> },
  { text: "Fixed expenses", path: "/fixed-expenses", icon: <EventNoteIcon /> },
];

export default NavMenuItems;
