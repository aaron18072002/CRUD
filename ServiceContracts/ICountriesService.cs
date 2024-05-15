using Entities;
using ServiceContracts.DTOS.CountryDTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represent business logic for maniplulating
    /// Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add a country object to list of countries
        /// </summary>
        /// <param name="countryAddRequest">
        /// Country object to add</param>
        /// <returns>Return a country response object</returns>
        Task<CountryResponse> AddCountry
            (CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Get all existing countries from datasource
        /// </summary>
        /// <returns>Return a list of country objects</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Get corresponding 
        /// country by pass countryID as a argument
        /// </summary>
        /// <param name="countryID">CountryID as a Guid type</param>
        /// <returns></returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
