using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTOS.CountryDTO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

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
        public async Task<int> UploadCountriesFromExcelFile
            (IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }
    }
}
