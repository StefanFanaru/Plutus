import * as React from "react";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Stack from "@mui/material/Stack";
import { useLocation, useNavigate } from "react-router";
import NavMenuItems from "./NavMenuItems";

// const secondaryListItems = [
//   // { text: "Settings", icon: <SettingsRoundedIcon /> },
//   // { text: "About", icon: <InfoRoundedIcon /> },
//   // { text: "Feedback", icon: <HelpRoundedIcon /> },
// ];

export default function MenuContent() {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Stack sx={{ flexGrow: 1, p: 1, justifyContent: "space-between" }}>
      <List dense>
        {NavMenuItems.map((item, index) => (
          <ListItem
            onClick={() => {
              navigate(item.path);
            }}
            key={index}
            disablePadding
            sx={{ display: "block" }}
          >
            <ListItemButton selected={item.path === location.pathname}>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <List dense>
        {/* {secondaryListItems.map((item, index) => ( */}
        {/*   <ListItem key={index} disablePadding sx={{ display: "block" }}> */}
        {/*     <ListItemButton> */}
        {/*       <ListItemIcon>{item.icon}</ListItemIcon> */}
        {/*       <ListItemText primary={item.text} /> */}
        {/*     </ListItemButton> */}
        {/*   </ListItem> */}
        {/* ))} */}
      </List>
    </Stack>
  );
}
