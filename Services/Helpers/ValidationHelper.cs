using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class ValidationHelper
    {
        public static void ModelValidationHelper(object request)
        {
            ValidationContext validationContext =
               new ValidationContext(request);
            List<ValidationResult> validationResults =
                new List<ValidationResult>();
            bool isValid = Validator.
                TryValidateObject(request,
                validationContext, validationResults, true);
            if (!isValid)
            {
                throw new ArgumentException(
                    validationResults?.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
