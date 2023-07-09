using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DadosInCached.CustomAttribute
{

    //Em resumo, esse método está armazenando o resultado bem-sucedido de uma ação no cache,
    //com uma chave específica e um tempo de expiração.
    //Isso permite que o resultado seja reutilizado para solicitações futuras sem precisar recalculá-lo,
    //desde que a entrada do cache ainda não tenha expirado.
    [AttributeUsage(AttributeTargets.Class)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        protected int _expirationTime;

       //armazenará as chaves dos itens em cache
        protected readonly List<string> KeyList = new();

        //classe que fornece um armazenamento em cache na memória para objetos.
        protected readonly MemoryCache ApiCache = new(new MemoryCacheOptions());

        public CachedAttribute(int expirationTime = 5)
        {
            _expirationTime = expirationTime;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string _cachekey = CreateCacheKey(context.HttpContext.Request);

            //se a requisição feita não for um get o cache é limpado para ser atualizado.
            if (context.HttpContext.Request.Method != "GET")
            {
                CleanCache();
                await next();
                return;
            }

            //verifica se tem valor no cache com o id '_cachekey'.
            if (ApiCache.TryGetValue(_cachekey, out IActionResult cachedResult))
            {
                context.Result = cachedResult;
                return;
            }

            var executedContext = await next();
            ArmazenarRespostaEmCache(executedContext);
        }

        private void ArmazenarRespostaEmCache(ActionExecutedContext context)
        {
            if (context.Result is OkObjectResult okResult)
            {
                string cacheKey = CreateCacheKey(context.HttpContext.Request);

                ApiCache.Set(cacheKey, okResult,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(_expirationTime))); //tempo de expiração do cache

                KeyList.Add(cacheKey);
            }
        }

        protected string CreateCacheKey(HttpRequest request)
        {
            string baseUri = $"{request.Scheme}://{request.Host.Value}";
            string fullPath = $"{request.Path.Value}{request.QueryString.Value}";

            return $"{baseUri}{fullPath}";
        }

        protected void CleanCache()
        {
            foreach (var key in KeyList)
            {
                ApiCache.Remove(key);
            }

            KeyList.Clear();
        }
    }
}
