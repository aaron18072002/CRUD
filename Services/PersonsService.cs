using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTOS.PersonDTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;


namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        //private readonly ICountriesService _countriesService;
        public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService> logger, IDiagnosticContext diagnosticContext)
        {
            this._personsRepository = personsRepository; 
            this._logger = logger;
            this._diagnosticContext = diagnosticContext;
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //Model Validation
            ValidationHelper.ModelValidationHelper(request);

            Person person = request.ToPerson();
            person.PersonID = Guid.NewGuid();

            await this._personsRepository.AddPerson(person);

            //this._db.sp_InsertPerson(person);

            return person.ToPersonResponse();
        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            this._logger.LogInformation("GetAllPersons method of " +
                "PersonsService");
            var persons = await this._personsRepository.GetAllPersons();
            List<PersonResponse> result = persons. 
                Select(person => person.ToPersonResponse()).ToList();
            //List<PersonResponse> result = this._db.sp_GetAllPersons()
            //    .Select(person => person.ToPersonResponse()).ToList();
            //foreach (var personResponse in result)
            //{
            //    personResponse.CountryName = 
            //        this?._countriesService?.
            //            GetCountryByCountryID(personResponse.CountryID)?.CountryName;
            //}
            return result;
        }
        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person = await this._personsRepository
                .GetPersonById(personID.Value);

            if (person == null) return null;

            return person.ToPersonResponse();
        }
        public async Task<List<PersonResponse>> GetFilteredPersons
            (string searchBy, string? searchString)
        {
            this._logger.LogInformation("GetFilteredPersons method of " +
                "PersonsService");
            List<PersonResponse> allPersons =
                await this.GetAllPersons();
            if (string.IsNullOrEmpty(searchString) ||
                string.IsNullOrEmpty(searchBy))
            {
                return allPersons;
            }
            List<Person> filteredPersons;
            using (Operation.Time("Time for Filtered Persons from database"))
            {
                filteredPersons = await (searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.PersonName != null &&
                                 p.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.Email != null &&
                                 p.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.DateOfBirth.HasValue &&
                                 p.DateOfBirth.Value.ToString("dd MMMM yyyy")
                                 .Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.Gender != null &&
                                 p.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.Country != null &&
                                 p.Country.CountryName != null &&
                                 p.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                        this._personsRepository.GetFilteredPersons(
                            p => p.Address != null &&
                                 p.Address.Contains(searchString)),

                    _ => this._personsRepository.GetAllPersons(),
                });
            }

            this._diagnosticContext.Set("Persons", filteredPersons);

            return filteredPersons.Select(p => p.ToPersonResponse()).ToList();
        }
        public async Task<List<PersonResponse>> GetSortedPersons
            (List<PersonResponse> allPersons,
            string sortBy, SortOrderOptions sortOrderOptions)
        {
            this._logger.LogInformation("GetSortedPersons method of " +
                "PersonsService");
            if (string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sortedPersons =
                (sortBy, sortOrderOptions)
            switch
                {
                    (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                        => allPersons.OrderBy(temp => temp.PersonName,
                        StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                        => allPersons.OrderByDescending(temp => temp.PersonName,
                        StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                        => allPersons.OrderBy(temp => temp.Email,
                        StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                        => allPersons.OrderByDescending(temp => temp.Email,
                        StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                    (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                    (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                    (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.ReceiveNewsLetter), SortOrderOptions.ASC) =>
                        allPersons.OrderBy(temp => temp.ReceiveNewsLetter).ToList(),

                    (nameof(PersonResponse.ReceiveNewsLetter), SortOrderOptions.DESC)
                        => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetter).ToList(),

                    (_,_) => allPersons
                };
            return sortedPersons;
        }
        public async Task<PersonResponse> UpdatePerson
            (PersonUpdateRequest? request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Model validation
            ValidationHelper.ModelValidationHelper(request);

            Person? matchingPerson = await this._personsRepository
                .GetPersonById(request.PersonID);
            if (matchingPerson == null)
            {
                throw new ArgumentException
                    ("Given person id doesn't exist");
            }

            //update all details
            matchingPerson.PersonName = request.PersonName;
            matchingPerson.Email = request.Email;
            matchingPerson.DateOfBirth = request.DateOfBirth;
            matchingPerson.Gender =
                ConvertTypesHelper.ConvertGenderOptionsToString(request.Gender);
            matchingPerson.CountryID = request.CountryID;
            matchingPerson.Address = request.Address;
            matchingPerson.ReceiveNewsLetter =
                request.ReceiveNewsLetter;

            await this._personsRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();
        }
        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? matchedPerson = await this._personsRepository
                .GetPersonById(personID.Value);
            if(matchedPerson == null)
            {   
                return false;
            }

            await this._personsRepository
                .DeletePersonByPersonID(personID.Value);

            return true;
        }
        public async Task<MemoryStream> GetPersonCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration
                (CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter,
                CultureInfo.InvariantCulture, leaveOpen: true);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.CountryName));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetter));
            csvWriter.NextRecord();

            List<Person> persons = await this._personsRepository
                .GetAllPersons();
            var repsonsePersons = persons.Select
                (p => p.ToPersonResponse()).ToList();

            foreach (var person in repsonsePersons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if(person.DateOfBirth != null)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString
                        ("yyyy-MM-dd"));
                } else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.CountryName);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetter);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await this.GetAllPersons();
                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.CountryName;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetter;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
