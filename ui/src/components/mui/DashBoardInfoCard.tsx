import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

export type DashboardInfoCardProps = {
  title: string;
  value: string;
  obligorName: string;
  bookingDate: string;
  isCredit: boolean;
};

export default function DashboardInfoCard({
  title,
  value,
  obligorName,
  bookingDate,
  isCredit,
}: DashboardInfoCardProps) {
  return (
    <Card variant="outlined" sx={{ height: "100%", flexGrow: 1 }}>
      <CardContent>
        <Typography component="h2" variant="subtitle2" gutterBottom>
          {title}
        </Typography>
        <Stack
          direction="column"
          sx={{ justifyContent: "space-between", flexGrow: "1", gap: 1 }}
        >
          <Stack sx={{ justifyContent: "space-between" }}>
            <Stack
              direction="row"
              sx={{
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: 3,
              }}
            >
              <Typography
                variant="h4"
                component="p"
                sx={{
                  color: isCredit ? "text.primary" : "success.main",
                }}
              >
                {value}
              </Typography>
            </Stack>
            <Typography variant="subtitle2" sx={{ color: "text.primary" }}>
              {obligorName}
            </Typography>
            <Typography variant="caption" sx={{ color: "text.secondary" }}>
              {bookingDate}
            </Typography>
          </Stack>
        </Stack>
      </CardContent>
    </Card>
  );
}
