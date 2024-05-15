using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOS.CountryDTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db;
        public CountriesService(ApplicationDbContext personsDbContext)
        {
            this._db = personsDbContext;            
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

            if (await this._db.Countries.CountAsync
                (country => country.CountryName == 
                countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException
                    ("Given CountryName is already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();

            this._db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            var result = await this._db.Countries
                .Select(country => country.ToCountryResponse()).ToListAsync();
            return result; 
        }
        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? country = await this._db.Countries.FirstOrDefaultAsync
                (country => country.CountryID == countryID);

            if (country == null)
            {
                return null;
            }

            return country.ToCountryResponse();
        }
    }
}
