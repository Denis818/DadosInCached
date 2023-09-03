using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Azure.Core;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using NuGet.Protocol;

namespace DadosInCached.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expirationTime;

        private readonly List<string> KeyList = new();

        private static readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

        public CachedAttribute(int expirationTime = 5)
        {
            _expirationTime = expirationTime;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method != "GET")
            {
                CleanCache();
                await next();
            }
            else if (_memoryCache.TryGetValue(CreateCacheKey(context.HttpContext.Request), out IActionResult cachedResult))
            {
                context.Result = cachedResult;
            }
            else
            {
                SaveResponseToCache(await next());
            }
        }

        private void SaveResponseToCache(ActionExecutedContext context)
        {
            if (context.Result is OkObjectResult okResult)
            {
                string cacheKey = CreateCacheKey(context.HttpContext.Request);

                _memoryCache.Set(cacheKey, okResult,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(_expirationTime)));

                KeyList.Add(cacheKey);
            }
        }

        private static string CreateCacheKey(HttpRequest request)
        {
            string fullEncodedUrl = $"{request.GetEncodedUrl()}/{request.QueryString.Value}";
            string route = $"{request.Method}/{request.HttpContext.GetRouteData().Values.FirstOrDefault()}";
            return $"{fullEncodedUrl}{route}";
        }

        private void CleanCache()
        {
            foreach (var key in KeyList)
            {
                _memoryCache.Remove(key);
            }

            KeyList.Clear();
        }
    }
}
