using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Diary.Domain.Extensions;

public static class DistributedCacheExtensions
{
    public static T GetObject<T>(this IDistributedCache cache, string key)
    {
        var data = cache.Get(key);
        return data.Length > 0 ? JsonSerializer.Deserialize<T>(data) : default(T);
    }

    public static void SetObject<T>(this IDistributedCache cache, string key, T obj,
        DistributedCacheEntryOptions? ops = null)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(obj);
        if (data?.Length > 0)
        {
            cache.Set(key, data, ops ?? new DistributedCacheEntryOptions());
        }
    }

    public static void RefreshObject<T>(this IDistributedCache cache, string key,
        DistributedCacheEntryOptions? ops = null)
    {
        var isObj = cache.GetObject<T>(key);
        if (isObj == null)
            return;
        cache.Refresh(key);
    }
}
