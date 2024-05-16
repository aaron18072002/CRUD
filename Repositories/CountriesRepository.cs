using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;
        public CountriesRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<Country> AddCountry(Country country)
        {
            this._db.Countries.Add(country);
            await this._db.SaveChangesAsync();
            return country;
        }
        public async Task<List<Country>> GetAllCountries()
        {
            var countries = await this._db.Countries
                .ToListAsync();
            return countries;
        }
        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            var country = await this._db.Countries
                .FirstOrDefaultAsync(c => c.CountryName == countryName);
            return country;
        }
        public async Task<Country?> GetCountryById(Guid countryID)
        {
            var country = await this._db.Countries
                .FirstOrDefaultAsync(c => c.CountryID == countryID);
            return country;
        }
    }
}
