using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;


namespace CCApiLibrary.CustomAttributes
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
