﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOS.CountryDTO
{
    public class CountryAddRequest
    {
        /// <summary>
        /// DTO class for add new country
        /// </summary>
        public string? CountryName { get; set; }
        public Country ToCountry()
        {
            return new Country
            {
                CountryName = CountryName,
            };
        }
    }
}
