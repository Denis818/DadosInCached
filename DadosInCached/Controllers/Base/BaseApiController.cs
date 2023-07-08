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

        public async Task<string> ReadRequestBody()
        {
            Request.EnableBuffering();

            var body = "";

            using (var reader = new StreamReader(Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }
            return body;
        }
    }
}
