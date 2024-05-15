using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOS.PersonDTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countriesService;
        public PersonsService(ApplicationDbContext personsDbContext,
            ICountriesService countriesService)
        {
            this._db = personsDbContext; 
            this._countriesService = countriesService;          
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //Model Validation
            ValidationHelper.ModelValidationHelper(request);

            Person person = request.ToPerson();
            person.PersonID = Guid.NewGuid();

            this._db.Persons.Add(person);
            await this._db.SaveChangesAsync();

            //this._db.sp_InsertPerson(person);

            return person.ToPersonResponse();
        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await this._db.Persons
                .Include("Country").ToListAsync();
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

            Person? person = await this._db.Persons.Include("Country").
                FirstOrDefaultAsync(person => person.PersonID == personID);

            if (person == null) return null;

            return person.ToPersonResponse();
        }
        public async Task<List<PersonResponse>> GetFilteredPersons
            (string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons =
                await this.GetAllPersons();
            if (string.IsNullOrEmpty(searchString) ||
                string.IsNullOrEmpty(searchBy))
            {
                return allPersons;
            }

            List<PersonResponse> filteredPersons =
                allPersons;
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    filteredPersons = allPersons.Where(
                        person => !string.IsNullOrEmpty(person.PersonName) ?
                        person.PersonName.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                case nameof(PersonResponse.Email):
                    filteredPersons = allPersons.Where(
                        person => !string.IsNullOrEmpty(person.Email) ?
                        person.Email.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    filteredPersons = allPersons.Where(
                        person => person.DateOfBirth != null ?
                        person.DateOfBirth.Value.ToString("dd MM yyyy").
                        Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    filteredPersons = allPersons.Where(
                        person => !string.IsNullOrEmpty(person.Gender) ?
                        person.Gender.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                case nameof(PersonResponse.CountryID):
                    filteredPersons = allPersons.Where(
                        person => !string.IsNullOrEmpty(person.CountryID.ToString()) ?
                        person.CountryID.ToString().Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                case nameof(PersonResponse.Address):
                    filteredPersons = allPersons.Where(
                        person => !string.IsNullOrEmpty(person.Address) ?
                        person.Address.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) : true).
                        ToList();
                    break;
                default: filteredPersons = allPersons; break;
            }

            return filteredPersons;
        }
        public async Task<List<PersonResponse>> GetSortedPersons
            (List<PersonResponse> allPersons,
            string sortBy, SortOrderOptions sortOrderOptions)
        {
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

            Person? matchingPerson = await this._db.Persons
                .FirstOrDefaultAsync(person => person.PersonID == request.PersonID);
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

            //this._db.sp_UpdatePerson(matchingPerson);

            await this._db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }
        public async Task<bool> DeletePerson(Guid? personID)
        {
            Person? matchedPerson = await this._db.Persons.FirstOrDefaultAsync
                (person => person.PersonID == personID);
            if(matchedPerson == null || personID == null)
            {   
                return false;
            }

            this._db.Persons.Remove
                (this._db.Persons.First(person => person.PersonID == personID));
            await this._db.SaveChangesAsync();

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

            List<PersonResponse> persons = await this._db.Persons
                .Include("Country")
                .Select(p => p.ToPersonResponse()).ToListAsync();

            foreach (var person in persons)
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
    }
}
