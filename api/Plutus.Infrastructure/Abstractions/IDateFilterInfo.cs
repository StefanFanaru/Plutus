
namespace Plutus.Infrastructure.Abstractions
{
    public interface IDateFilterInfo
    {
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
}
