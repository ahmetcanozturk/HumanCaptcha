using HumanCaptchaBackend.Business;
using HumanCaptchaBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HumanCaptchaBackend.Services
{
    public class TokenAuthenticationAttribute : ActionFilterAttribute
    {
        private readonly HumanCaptchaContext context;
        private readonly IExceptionManager exceptionManager;

        public TokenAuthenticationAttribute(HumanCaptchaContext _context, IExceptionManager _exceptionManager)
        {
            this.context = _context;
            this.exceptionManager = _exceptionManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var actionContext = context.HttpContext;
            if (actionContext.Request.Headers == null)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                var headers = actionContext.Request.Headers;
                if (headers != null && headers.Keys.Count > 0)
                {
                    if (headers.ContainsKey("Token"))
                    {
                        string token = headers["Token"];

                        // validate token
                        var model = new TokenModel(this.context, this.exceptionManager);
                        var isValid = model.IsTokenValid(token).Result;
                        if (!isValid)
                        {
                            // returns unauthorized error  
                            context.Result = new UnauthorizedResult();
                        }
                        else
                            base.OnActionExecuting(context);
                    }
                    else
                        context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
