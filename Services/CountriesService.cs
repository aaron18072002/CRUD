using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        //public async Task<MemoryStream> GetPersonsExcel()
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        //    {
        //        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
        //        workSheet.Cells["A1"].Value = "Person Name";
        //        workSheet.Cells["B1"].Value = "Email";
        //        workSheet.Cells["C1"].Value = "Date of Birth";
        //        workSheet.Cells["D1"].Value = "Age";
        //        workSheet.Cells["E1"].Value = "Gender";
        //        workSheet.Cells["F1"].Value = "Country";
        //        workSheet.Cells["G1"].Value = "Address";
        //        workSheet.Cells["H1"].Value = "Receive News Letters";

        //        using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
        //        {
        //            headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            headerCells.Style.Font.Bold = true;
        //        }

        //        int row = 2;
        //        List<PersonResponse> persons = _db.Persons
        //          .Include("Country").Select(temp => temp.ToPersonResponse())
        //          .ToList();
        //        foreach (PersonResponse person in persons)
        //        {
        //            workSheet.Cells[row, 1].Value = person.PersonName;
        //            workSheet.Cells[row, 2].Value = person.Email;
        //            if (person.DateOfBirth.HasValue)
        //                workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
        //            workSheet.Cells[row, 4].Value = person.Age;
        //            workSheet.Cells[row, 5].Value = person.Gender;
        //            workSheet.Cells[row, 6].Value = person.Country;
        //            workSheet.Cells[row, 7].Value = person.Address;
        //            workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

        //            row++;
        //        }

        //        workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

        //        await excelPackage.SaveAsync();
        //    }

        //    memoryStream.Position = 0;
        //    return memoryStream;
        //}
    }
}
