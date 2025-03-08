namespace Plutus.API.Asp;

public class DateFilterInfo : IDateFilterInfo
{
    public DateFilterInfo(IHttpContextAccessor httpContextAccessor)
    {
        var headerValue = httpContextAccessor.HttpContext.Request.Headers["Date-Filter"];

        if (string.IsNullOrEmpty(headerValue))
        {
            return;
        }

        var dates = headerValue.ToString().Split(';');
        if (dates.Length != 2)
        {
            return;
        }

        if (DateTime.TryParse(dates[0], out var startDate))
        {
            StartDate = startDate;
        }

        if (DateTime.TryParse(dates[1], out var endDate))
        {
            EndDate = endDate;
        }
    }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
