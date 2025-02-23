namespace OTD.ServiceLayer.Abstract
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task<bool> SetCacheAsync<T>(string key, T value, short? minute = null);
        Task<bool> RemoveCacheAsync(string key);
        Task<bool> FlushAllDatabases();
        Task<bool> FlushAllDatabases(string includeKey);
    }
}
