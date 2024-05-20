using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTOS.PersonDTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter
            (ILogger<PersonsListActionFilter> logger)
        {
            this._logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            this._logger.LogInformation
                ("PersonListActionFilter.OnActionExecuted method");
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            this._logger.LogInformation
                ("PersonListActionFilter.OnActionExecuting method");
            if(context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString
                    (context.ActionArguments["searchBy"]);
                if(!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };
                    if(searchByOptions.Any
                        (temp => temp == searchBy) == false)
                    {
                        this._logger.LogInformation
                            ($"searchBy actual value {searchBy}");
                    }
                }
            }
        }
    }
}
