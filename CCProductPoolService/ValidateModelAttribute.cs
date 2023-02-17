using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace CCProductPoolService
{

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Result = new UnprocessableEntityObjectResult(actionContext.ModelState);
                //actionContext.HttpContext.Response.StatusCode = 400;
                //actionContext.HttpContext.Response.Body= null;
              

                //  actionContext.Response = actionContext.Request.CreateErrorResponse(
                //HttpStatusCode.BadRequest, actionContext.ModelState);

            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
