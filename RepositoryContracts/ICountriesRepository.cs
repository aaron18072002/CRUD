using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Countries entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Return the country object after adding it 
        /// to the data source</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Return all countries in the data store
        /// </summary>
        /// <returns>All countries in the data store</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Return a country that have id matching with given country id
        /// </summary>
        /// <param name="countryID">CountryID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryById(Guid countryID);

        /// <summary>
        /// Return a country that have name matching with given country name
        /// </summary>
        /// <param name="countryName">CountryName to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
