using System;
using System.Collections.Generic;

namespace Coding_challenge
{
    public class SimpleCache<T> where T : class
    {
        private readonly Dictionary<string, CacheItem<T>> _cache;
        private readonly TimeSpan _expirationTime;

        private class CacheItem<TItem>
        {
            public TItem Value { get; set; }
            public DateTime ExpirationTime { get; set; }
        }

        public SimpleCache(int expirationInSeconds = 300)
        {
            _cache = new Dictionary<string, CacheItem<T>>();
            _expirationTime = TimeSpan.FromSeconds(expirationInSeconds);
        }

        public void Add(string key, T item)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty");

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var cacheItem = new CacheItem<T>
            {
                Value = item,
                ExpirationTime = DateTime.UtcNow.Add(_expirationTime)
            };

            lock (_cache)
            {
                _cache[key] = cacheItem;
            }
        }

        public T Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty");

            lock (_cache)
            {
                if (!_cache.TryGetValue(key, out var cacheItem))
                    throw new KeyNotFoundException($"Key '{key}' not found in cache");

                if (DateTime.UtcNow > cacheItem.ExpirationTime)
                {
                    _cache.Remove(key);
                    throw new InvalidOperationException($"Cache item with key '{key}' has expired");
                }

                return cacheItem.Value;
            }
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty");

            lock (_cache)
            {
                return _cache.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        public List<string> GetAllKeys()
        {
            lock (_cache)
            {
                RemoveExpiredItems();
                return new List<string>(_cache.Keys);
            }
        }

        private void RemoveExpiredItems()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _cache.Where(kvp => now > kvp.Value.ExpirationTime)
                                  .Select(kvp => kvp.Key)
                                  .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }
        }
    }
}