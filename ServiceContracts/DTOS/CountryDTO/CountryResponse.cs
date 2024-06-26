﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOS.CountryDTO
{
    /// <summary>
    /// DTO class for convert Country object
    /// to CountryResponse object as a value return
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(CountryResponse)) return false;

            CountryResponse countryToCompare =
                (CountryResponse)obj;
            return CountryID == countryToCompare.CountryID &&
                CountryName == countryToCompare.CountryName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse
            (this Country country)
        {
            return new CountryResponse
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }
}
