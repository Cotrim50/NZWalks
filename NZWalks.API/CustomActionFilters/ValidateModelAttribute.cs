using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NZWalks.API.CustomActionFilters
{
  public class ValidateModelAttribute: ActionFilterAttribute
  {
    override public void OnActionExecuting(ActionExecutingContext context)
    {
      if (!context.ModelState.IsValid)
      {
        context.Result = new BadRequestResult();
      }
    }
  }
}
