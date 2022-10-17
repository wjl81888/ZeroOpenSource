// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    /// <summary>
    /// Extension methods for setting data in an <see cref="IDistributedCache" />.
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Sets a string in the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="value">The data to store in the cache.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        public static void SetObject<TItem>(this IDistributedCache cache, string key, TItem value)
        {
            cache.SetObject(key, value, new DistributedCacheEntryOptions());
        }

        /// <summary>
        /// Sets a string in the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="value">The data to store in the cache.</param>
        /// <param name="options">The cache options for the entry.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        public static void SetObject<TItem>(this IDistributedCache cache, string key, TItem value, DistributedCacheEntryOptions options)
        {
            cache.SetString(key, JsonSerializer.Serialize(value), options);
        }

        /// <summary>
        /// Asynchronously sets a string in the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="value">The data to store in the cache.</param>
        /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous set operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        public static Task SetObjectAsync<TItem>(this IDistributedCache cache, string key, TItem value, CancellationToken token = default)
        {
            return cache.SetObjectAsync(key, value, new DistributedCacheEntryOptions(), token);
        }

        /// <summary>
        /// Asynchronously sets a string in the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="value">The data to store in the cache.</param>
        /// <param name="options">The cache options for the entry.</param>
        /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous set operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        public static Task SetObjectAsync<TItem>(this IDistributedCache cache, string key, TItem value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            return cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, token);
        }

        /// <summary>
        /// Gets a string from the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to get the stored data for.</param>
        /// <returns>The string value from the stored cache key.</returns>
        public static TItem GetObject<TItem>(this IDistributedCache cache, string key)
        {
            var dataString = cache.GetString(key);
            if (dataString == null)
            {
                return default!;
            }
            return JsonSerializer.Deserialize<TItem>(dataString)!;
        }

        /// <summary>
        /// Asynchronously gets a string from the specified cache with the specified key.
        /// </summary>
        /// <param name="cache">The cache in which to store the data.</param>
        /// <param name="key">The key to get the stored data for.</param>
        /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
        /// <returns>A task that gets the string value from the stored cache key.</returns>
        public static async Task<TItem> GetObjectAsync<TItem>(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            var dataString = await cache.GetStringAsync(key, token);
            if (dataString == null)
            {
                return default!;
            }
            return JsonSerializer.Deserialize<TItem>(dataString)!;
        }

        public static TItem GetOrCreate<TItem>(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, TItem> factory)
        {
            if (!cache.TryGetValue(key, out TItem result))
            {
                var entryOptions = new DistributedCacheEntryOptions();
                result = factory(entryOptions);
                cache.SetObjectAsync(key, result, entryOptions);
            }

            return result;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, Task<TItem>> factory)
        {
            if (!cache.TryGetValue(key, out TItem result))
            {
                var entryOptions = new DistributedCacheEntryOptions();
                result = await factory(entryOptions);
                await cache.SetObjectAsync(key, result, entryOptions);
            }

            return result;
        }

        public static bool TryGetValue<TItem>(this IDistributedCache cache, string key, out TItem value)
        {
            var dataString = cache.GetString(key);

            if (dataString == null)
            {
                value = default!;
                return false;
            }

            value = JsonSerializer.Deserialize<TItem>(dataString)!;
            return true;
        }
    }
}