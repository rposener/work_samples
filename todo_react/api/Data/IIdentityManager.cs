
namespace TodoApi.Data
{
    public interface IIdentityManager
    {
        Task<string> GetNextIdAsync(string todoPartition);
    }
}