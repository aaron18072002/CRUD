using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTOS.CountryDTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
            this._countriesRepository = countriesRepository;            
        }
        public async Task<CountryResponse> AddCountry
            (CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException
                    ("Argument is not supplied");
            }

            if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException
                    ("CountryName is null or empty");
            }

            if (await this._countriesRepository
                .GetCountryByCountryName(countryAddRequest.CountryName) == null)
            {
                throw new ArgumentException
                    ("Given CountryName is already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();

            await this._countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            var countries = await this._countriesRepository
                .GetAllCountries();
            List<CountryResponse> result = 
                countries.Select(c => c.ToCountryResponse()).ToList();
            return result;
        }
        public async Task<CountryResponse?> GetCountryByCountryID
            (Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? country = await this._countriesRepository
                .GetCountryById(countryID.Value);

            if (country == null)
            {
                return null;
            }

            return country.ToCountryResponse();
        }
    }
}
