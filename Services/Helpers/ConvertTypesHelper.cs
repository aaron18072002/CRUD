using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public static class ConvertTypesHelper
    {
        public static string? ConvertGenderOptionsToString
            (GenderOptions? genderOptions)
        {          
            switch (genderOptions)
            {
                case GenderOptions.Male: return "Male";
                case GenderOptions.Female: return "Female";
                case GenderOptions.Other: return "Other";
                default: return null;
            }
        }
    }
}
