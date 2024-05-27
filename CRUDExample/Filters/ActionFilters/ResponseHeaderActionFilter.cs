using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;
        public int Order { get; set; }
        public ResponseHeaderActionFilter
            (ILogger<ResponseHeaderActionFilter> logger, 
            string key, string value, int order)
        {
            this._logger = logger;
            this._key = key;
            this._value = value;
            this.Order = order;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            this._logger.LogInformation("{FilterName}.{MethodName} method",
                nameof(ResponseHeaderActionFilter),
                nameof(OnActionExecuted));

            context.HttpContext.Response.Headers[this._key] = this._value;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            this._logger.LogInformation("{FilterName}.{MethodName} method",
                nameof(ResponseHeaderActionFilter),
                nameof(OnActionExecuting));           
        }
    }
}
