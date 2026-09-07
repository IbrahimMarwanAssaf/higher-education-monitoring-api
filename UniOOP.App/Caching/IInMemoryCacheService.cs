namespace UNIOOP.App.Caching
{
    public interface IInMemoryCacheService
    {
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory);
        Task RemoveAsync(string key);
    }
}