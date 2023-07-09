using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text;

namespace DadosInCached.Controllers.Base
{
    public class BaseApiController : Controller
    {
        public string UserId { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            UserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> ReadRequestBody()
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, leaveOpen: true);

            string body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            return body;
        }
    }
}
