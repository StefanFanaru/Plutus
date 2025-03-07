import { Box } from "@mui/material";
import Grid from "@mui/material/Grid2";
import ObligorsTable from "./ObligorsTable";

function Obligors() {
  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Grid container spacing={2} columns={5}>
        <Grid size={{ xs: 12, lg: 9 }}>
          <ObligorsTable />
        </Grid>
      </Grid>
    </Box>
  );
}

export default Obligors;
