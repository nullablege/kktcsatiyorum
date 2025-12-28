using BusinessLayer.Common;
using Microsoft.Extensions.Caching.Memory;

namespace KKTCSatiyorum.Extensions
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly HashSet<string> _keys = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T? value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration;
            else
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);

            _cache.Set(key, value, options);
            lock (_keys) { _keys.Add(key); }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            lock (_keys) { _keys.Remove(key); }
        }

        public void RemoveByPrefix(string prefix)
        {
            List<string> keysToRemove;
            lock (_keys)
            {
                keysToRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();
            }
            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
        }
    }
}
