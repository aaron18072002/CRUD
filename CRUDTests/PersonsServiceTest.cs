using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOS.CountryDTO;
using ServiceContracts.DTOS.PersonDTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            this._fixture = new Fixture();
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new
                DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>
                    ().Options
                );
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock
                (temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock
                (temp => temp.Persons, personsInitialData);
            this._countriesService = new CountriesService(null);
            this._personsService = new PersonsService
                (null);
            this._outputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> actual = async () =>
            {
                await this._personsService.AddPerson(personAddRequest);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>(actual);
        }
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, null as string)
                .Create();

            //Act
            Func<Task> actual = async () =>
            {
                await this._personsService.AddPerson(personAddRequest);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(actual);
        }
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest =
               this._fixture.Build<PersonAddRequest>()
               .With(p => p.Email, "someone@example.com")
               .Create();

            //Act
            PersonResponse expectedValue = await
                this._personsService.AddPerson(personAddRequest);
            List<PersonResponse> actualValue = await
                this._personsService.GetAllPersons();

            //Assert
            expectedValue.PersonID.Should().NotBe(Guid.Empty);
            //Assert.True(expectedValue.PersonID != Guid.Empty);
            actualValue.Should().Contain(expectedValue);
            //Assert.Contains(expectedValue, actualValue);
        }
        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? actualValue = await
                this._personsService.GetPersonByPersonID(personID);

            //Assert
            actualValue.Should().BeNull();
            //Assert.Null(actualValue);
        }
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest =
                this._fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await
                this._countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample@gmail.com")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();

            //Act
            PersonResponse expectedValue = await
                this._personsService.AddPerson(personAddRequest);
            PersonResponse? actualValue = await
                this._personsService.GetPersonByPersonID
                    (expectedValue.PersonID);

            //Assert
            actualValue.Should().Be(expectedValue);
            //Assert.Equal(expectedValue, actualValue);
        }
        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> personResponsesList = await
                this._personsService.GetAllPersons();

            //Assert
            personResponsesList.Should().BeEmpty();
            //Assert.Empty(personResponsesList);
        }
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 =
                this._fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 =
                this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await
                this._countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await
                this._countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personRequest1 =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample@gmail.com")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest2 =
                 this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample2@gmail.com")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest3 =
                 this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample3@gmail.com")
                .With(p => p.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequestsList
                 = new List<PersonAddRequest>()
                 {
                     personRequest1, personRequest2, personRequest3
                 };
            List<PersonResponse> expectedValue =
                new List<PersonResponse>();

            //Act
            foreach (var personAddRequest in personAddRequestsList)
            {
                var personResponse = await
                    this._personsService.AddPerson(personAddRequest);
                expectedValue.Add(personResponse);
            }
            List<PersonResponse> actualValue = await
                this._personsService.GetAllPersons();
            this._outputHelper.WriteLine("Actual value");
            foreach (var personResponse in actualValue)
            {
                this._outputHelper.WriteLine
                    (personResponse.ToString());
            }

            //Assert
            actualValue.Should().BeEquivalentTo(expectedValue);
            //foreach (var personResponse in expectedValue)
            //{
            //    Assert.Contains(personResponse, actualValue);
            //}
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 =
               this._fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 =
                this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await
                this._countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await
                this._countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personRequest1 =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample@gmail.com")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest2 =
                 this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample2@gmail.com")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest3 =
                 this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample3@gmail.com")
                .With(p => p.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonResponse> expectedValue =
                new List<PersonResponse>();
            List<PersonAddRequest> personRequestsList
                = new List<PersonAddRequest>()
                {
                    personRequest1, personRequest2, personRequest3
                };
            foreach (var personRequest in personRequestsList)
            {
                PersonResponse personResponse = await
                    this._personsService.AddPerson(personRequest);
                expectedValue.Add(personResponse);
            }

            //Act
            List<PersonResponse> actualValue = await
                this._personsService.GetFilteredPersons
                    (nameof(Person.PersonName), "");

            //Assert
            actualValue.Should().BeEquivalentTo(expectedValue);
            //foreach (var personResponse in expectedValue)
            //{
            //    Assert.Contains(personResponse, actualValue);
            //}
        }
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 =
               this._fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 =
                this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await
                this._countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await
                this._countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personRequest1 =
                this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest2 =
                 this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest3 =
                 this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Scott")
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonResponse> expectedValue =
                new List<PersonResponse>();
            List<PersonAddRequest> personRequestsList
                = new List<PersonAddRequest>()
                {
                    personRequest1, personRequest2, personRequest3
                };
            foreach (var personRequest in personRequestsList)
            {
                PersonResponse personResponse = await
                    this._personsService.AddPerson(personRequest);
                expectedValue.Add(personResponse);
            }

            //Act
            List<PersonResponse> actualValue = await
                this._personsService.GetFilteredPersons
                    (nameof(Person.PersonName), "ma");

            this._outputHelper.WriteLine("Expected Value");
            foreach (var personResponse in expectedValue)
            {
                this._outputHelper.
                    WriteLine(personResponse.ToString());
            }
            this._outputHelper.WriteLine("Actual Value");
            foreach (var personResponse in actualValue)
            {
                this._outputHelper.
                    WriteLine(personResponse.ToString());
            }

            //Assert
            actualValue.Should().OnlyContain
                (p => p.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
            //foreach (var personResponse in expectedValue)
            //{
            //    if (personResponse.PersonName != null)
            //    {
            //        if (personResponse.PersonName.Contains
            //            ("ma", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(personResponse, actualValue);
            //        }
            //    }
            //}
        }
        #endregion

        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersosn()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 =
               this._fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 =
                this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await
                this._countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await
                this._countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personRequest1 =
                this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest2 =
                 this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personRequest3 =
                 this._fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Scott")
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonResponse> expectedValue =
                new List<PersonResponse>();
            List<PersonAddRequest> personRequestsList
                = new List<PersonAddRequest>()
                {
                    personRequest1, personRequest2, personRequest3
                };
            foreach (var personRequest in personRequestsList)
            {
                PersonResponse personResponse = await
                    this._personsService.AddPerson(personRequest);
                expectedValue.Add(personResponse);
            }

            //Act
            List<PersonResponse> allPersons = await
                this._personsService.GetAllPersons();
            List<PersonResponse> actualValue = await
                this._personsService.GetSortedPersons
                (allPersons, nameof(Person.PersonName),
                SortOrderOptions.DESC);
            expectedValue = expectedValue.OrderByDescending
                (person => person.PersonName).ToList();

            this._outputHelper.WriteLine("Expected Value");
            foreach (var personResponse in expectedValue)
            {
                this._outputHelper.
                    WriteLine(personResponse.ToString());
            }
            this._outputHelper.WriteLine("Actual Value");
            foreach (var personResponse in actualValue)
            {
                this._outputHelper.
                    WriteLine(personResponse.ToString());
            }

            //Assert
            actualValue.Should().BeInDescendingOrder
                (p => p.PersonName);
            //for (int i = 0; i < expectedValue.Count; i++)
            //{
            //    Assert.Equal(expectedValue[i], actualValue[i]);
            //}
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Act
            Func<Task> actual = async () =>
            {
                await this._personsService.UpdatePerson
                    (personUpdateRequest);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>(actual);
        }
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest =
                this._fixture.Create<PersonUpdateRequest>();

            //Act
            Func<Task> actual = async () =>

            {
                await this._personsService.UpdatePerson
                    (personUpdateRequest);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(actual);
        }
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            //Arrange
            CountryAddRequest countryAddRequest =
               this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await
                this._countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personRequest =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "Rahman")
                .With(p => p.Email, "sample@gmail.com")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();

            PersonResponse personResponse = await
                this._personsService.AddPerson(personRequest);

            PersonUpdateRequest personUpdateRequest =
                personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            //Act
            Func<Task> actual = async () =>
            {
                await this._personsService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await actual.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(actual);
        }

        [Fact]
        public async Task UpdatePerson_FullDetailsUpdations()
        {

            //Arrange
            CountryAddRequest countryAddRequest =
              this._fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await
                this._countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personRequest =
                this._fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample@gmail.com")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();

            PersonResponse personResponse = await
                this._personsService.AddPerson(personRequest);

            PersonUpdateRequest personUpdateRequest =
                personResponse.ToPersonUpdateRequest();

            //Act
            PersonResponse expectedValue = await
                this._personsService.UpdatePerson(personUpdateRequest);
            PersonResponse? actualValue = await
                this._personsService.GetPersonByPersonID
                    (personUpdateRequest.PersonID);

            //Assert
            expectedValue.Should().Be(actualValue);
            //Assert.Equal(expectedValue, actualValue);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isSuccess = await
                this._personsService.DeletePerson(Guid.NewGuid());

            //Assert
            isSuccess.Should().BeFalse();
            // Assert.False(isSuccess);
        }
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest =
                new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse = await
                this._countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = countryResponse.CountryID,
                Email = "sample@email.com"
            };
            PersonResponse personResponse = await
                this._personsService.AddPerson(personAddRequest);

            //Act
            bool isSuccess = await
                this._personsService.DeletePerson
                    (personResponse.PersonID);

            //Assert
            isSuccess.Should().BeTrue();
            //Assert.True(isSuccess);
        }
        #endregion
    }
}
