using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DadosInCached.CustomAttribute
{

    /// <summary>
    /// Atributo utiliza o cache em memória para armazenar respostas de uma action bem-sucedida.<br/>
    /// Se a requisição feita não for um <b>GET</b>, o cache é limpo para ser atualizado. <br/>
    /// Permite a configuração de um tempo de expiração para os dados em cache, por padrão é <b>5min</b>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expirationTime;

       //armazenará as chaves dos itens em cache
        private readonly List<string> KeyList = new();

        //classe que fornece um armazenamento em cache na memória para objetos.
        private static readonly MemoryCache _apiCache = new (new MemoryCacheOptions());

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
            if (_apiCache.TryGetValue(_cachekey, out IActionResult cachedResult))
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

                _apiCache.Set(cacheKey, okResult,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(_expirationTime))); //tempo de expiração do cache

                KeyList.Add(cacheKey);
            }
        }

        private string CreateCacheKey(HttpRequest request)
        {
            string baseUri = $"{request.Scheme}://{request.Host.Value}";
            string fullPath = $"{request.Path.Value}{request.QueryString.Value}";

            return $"{baseUri}{fullPath}";
        }

        private void CleanCache()
        {
            foreach (var key in KeyList)
            {
                _apiCache.Remove(key);
            }

            KeyList.Clear();
        }
    }
}
