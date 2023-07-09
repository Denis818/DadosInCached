using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DadosInCached.CustomAttribute
{

    //Em resumo, esse método está armazenando o resultado bem-sucedido de uma ação no cache,
    //com uma chave específica e um tempo de expiração.
    //Isso permite que o resultado seja reutilizado para solicitações futuras sem precisar recalculá-lo,
    //desde que a entrada do cache ainda não tenha expirado.
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        protected readonly MemoryCache WebApiCache = new(new MemoryCacheOptions());
        protected int _timespan;
        protected readonly List<string> KeyList = new();


        public CachedAttribute(int timespan = 40)
        {
            _timespan = timespan;
        }

        //será chamado antes e depois da execução de uma ação do controlador.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Headers["Referer"]serve para obter a URL completa de onde a solicitação foi originada.
            //Com isso montamos uma key para usar como a chave de cache.
            string _cachekey = CreateCacheKey(context.HttpContext.Request);

            // verifica se a ação atual é "cacheável"
            if (context.HttpContext.Request.Method != "GET")
            {
                CleanCache();
                await next();
                return;
            }

            //Aqui, estamos tentando obter um valor do cache usando a chave de cache.
            //O valor esperado é uma tupla que contém um objeto DateTime e um objeto IActionResult.
            if (WebApiCache.TryGetValue(_cachekey, out Tuple<DateTime, IActionResult> cachedResult))
            {
                //verifica se o tempo atual é menor que o tempo de expiração armazenado no cache.
                if (DateTime.Now < cachedResult.Item1)
                {
                    //A resposta é recuperada do cache e o método retorna
                    context.Result = cachedResult.Item2;
                    return;
                }
                else
                {
                    //Se o tempo atual for maior ou igual ao tempo de expiração,
                    //então o valor é removido do cache.
                    WebApiCache.Remove(_cachekey);
                }
            }

            //Se a ação não estiver no cache ou o valor no cache tiver expirado,
            //a ação do controlador é chamada.
            var executedContext = await next();

            //Após a execução da ação, a resposta é armazenada no cache.
            ArmazenarRespostaEmCache(executedContext);
        }

        private void ArmazenarRespostaEmCache(ActionExecutedContext executedContext)
        {
            //cria a chave de cache.
            string _cachekey = CreateCacheKey(executedContext.HttpContext.Request);

            if (executedContext.Result is OkObjectResult okResult)
            {
                //configurando as opções de entrada do cache.
                //Estamos definindo uma expiração absoluta, que é o tempo total máximo
                //que a entrada pode permanecer no cache antes de ser removida.
                //Este tempo é especificado pela variável '_timespan',
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_timespan));

                //Tipo 'Tuple': tupla é uma classe de coleção no .NET que permite armazenar vários elementos diferentes ex: Tuple<int, string, DateTime> 
                //criando uma tupla para armazenar no cache, a hora em o cache deve expirar
                // e o resultado da ação.
                var resultToCache = new Tuple<DateTime, IActionResult>(DateTime.Now.AddSeconds(_timespan), okResult);

                //Adicionando a entrada ao cache.
                //Usando a chave de cache que criamos antes,
                //os dados da tupla que queremos armazenar
                //e as opções de entrada do cache que configuramos
                WebApiCache.Set(_cachekey, resultToCache, cacheEntryOptions);
                KeyList.Add(_cachekey);
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
                WebApiCache.Remove(key);
            }

            KeyList.Clear();
        }
    }
}
