using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace UNIOOP.App.Caching
{
    public class InMemoryCacheService : IInMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory)
        {
            T? cachedValue = _memoryCache.Get<T>(key);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            SemaphoreSlim semaphore = GetSemaphore(key);

            await semaphore.WaitAsync();

            try
            {
                cachedValue = _memoryCache.Get<T>(key);

                if (cachedValue is not null)
                {
                    return cachedValue;
                }

                T? value = await factory();

                if (value is not null)
                {
                    _memoryCache.Set(key, value);
                }

                return value;
            }
            finally
            {
                semaphore.Release();
                RemoveSemaphore(key, semaphore);
            }
        }

        public async Task RemoveAsync(string key)
        {
            SemaphoreSlim semaphore = GetSemaphore(key);

            await semaphore.WaitAsync();

            try
            {
                _memoryCache.Remove(key);
            }
            finally
            {
                semaphore.Release();
                RemoveSemaphore(key, semaphore);
            }
        }

        private SemaphoreSlim GetSemaphore(string key)
        {
            return _semaphores.GetOrAdd(key,
               _ => new SemaphoreSlim(1, 1));
        }

        private void RemoveSemaphore(string key, SemaphoreSlim semaphore)
        {
            _semaphores.TryRemove(
                new KeyValuePair<string, SemaphoreSlim>(key, semaphore));
        }
    }
}