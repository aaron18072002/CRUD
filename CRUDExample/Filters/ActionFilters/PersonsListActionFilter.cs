using CRUDExample.Controllers;
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
                ($"{nameof(PersonsListActionFilter)}.{nameof(OnActionExecuted)} method");
            PersonsController personsController = (PersonsController)
                context.Controller;
            IDictionary<string,object?>? parameters = (IDictionary<string,object?>?)
                context.HttpContext.Items["arguments"];
            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
            }

            personsController.ViewBag.SearchFields = 
                new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] =
                context.ActionArguments;
            this._logger.LogInformation
                ($"{nameof(PersonsListActionFilter)}.{nameof(OnActionExecuting)} method");
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
                        context.ActionArguments["searchBy"] = 
                            nameof(PersonResponse.PersonName);
                        this._logger.LogInformation($"searchBy updated value " +
                            $"{searchBy}");
                    }
                }
            }
        }
    }
}
