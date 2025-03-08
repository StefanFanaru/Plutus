import * as React from "react";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import Divider from "@mui/material/Divider";
import Drawer, { drawerClasses } from "@mui/material/Drawer";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import { useAuth } from "react-oidc-context";
import MenuContent from "./MenuContent";
import MenuButton from "./MenuButton";
import { Link } from "@mui/material";
import GitHubIcon from "@mui/icons-material/GitHub";

interface SideMenuMobileProps {
  open: boolean | undefined;
  toggleDrawer: (newOpen: boolean) => () => void;
}

export default function SideMenuMobile({
  open,
  toggleDrawer,
}: SideMenuMobileProps) {
  const auth = useAuth();
  function onLogoutClick(_: any): void {
    auth.signoutRedirect({
      post_logout_redirect_uri: window.location.origin,
    });
  }

  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={toggleDrawer(false)}
      sx={{
        zIndex: (theme) => theme.zIndex.drawer + 1,
        [`& .${drawerClasses.paper}`]: {
          backgroundImage: "none",
          backgroundColor: "background.paper",
        },
      }}
    >
      <Stack
        sx={{
          maxWidth: "70dvw",
          height: "100%",
        }}
      >
        <Stack direction="row" sx={{ p: 2, pb: 0, gap: 1 }}>
          <Stack
            direction="row"
            sx={{ gap: 1, alignItems: "center", flexGrow: 1, p: 1 }}
          >
            <Avatar
              sizes="small"
              alt={auth.user?.profile.name}
              sx={{ width: 24, height: 24 }}
            />
            <Typography component="p" variant="h6">
              {auth.user?.profile.name}
            </Typography>
          </Stack>
          <Link
            underline="none"
            href="https://github.com/StefanFanaru/Plutus"
            target="_blank"
          >
            <MenuButton aria-label="Open notifications">
              <GitHubIcon />
            </MenuButton>
          </Link>
        </Stack>
        <Divider />
        <Stack sx={{ flexGrow: 1 }}>
          <MenuContent />
          <Divider />
        </Stack>
        {/* <CardAlert /> */}
        <Stack sx={{ p: 2 }}>
          <Button
            variant="outlined"
            fullWidth
            onClick={onLogoutClick}
            startIcon={<LogoutRoundedIcon />}
          >
            Logout
          </Button>
        </Stack>
      </Stack>
    </Drawer>
  );
}
