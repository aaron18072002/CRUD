using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOS.CountryDTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new
                DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>
                    ().Options
                );
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock
                (temp => temp.Countries,countriesInitialData);
            this._countriesService = new CountriesService(null);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? _request = null;

            //Act
            Func<Task> actual = async () =>
            {
                await this._countriesService.AddCountry(_request);
            };         
            
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(actual);
        }
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Act
            Func<Task> actual = async () =>
            {
                await this._countriesService.AddCountry(request);
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(actual);
        }
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            //Act
            Func<Task> actual = async () =>
            {
                await this._countriesService.AddCountry(request1);
                await this._countriesService.AddCountry(request2);
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(actual);
        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest _request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            //Act
            CountryResponse response = 
                await this._countriesService.AddCountry(_request);
            List<CountryResponse> allCountries =
                await this._countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, allCountries);
        }
        #endregion  

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual = await
                this._countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetAllCountries_GetFewCountries()
        {
            //Arrange
            List<CountryResponse> expectedCountryResponseList =
                new List<CountryResponse>();
            List<CountryAddRequest> countryAddRequestsList =
                new List<CountryAddRequest>()
                {
                    new CountryAddRequest() { CountryName = "Brazil" },
                    new CountryAddRequest() { CountryName = "Vietnam" },
                };

            //Act
            foreach (var countryAddRequest in countryAddRequestsList)
            {
                expectedCountryResponseList.Add(await
                    this._countriesService.AddCountry(countryAddRequest));
            }
            List<CountryResponse> actualCountryResponseList =
                await this._countriesService.GetAllCountries();

            //Assert
            foreach (var expectedCountry in expectedCountryResponseList)
            {
                Assert.Contains(expectedCountry, 
                    actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            CountryResponse? expectedValue = null;

            //Act
            CountryResponse? actualValue = await
                this._countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Equal(expectedValue, actualValue);
        }
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = 
                new CountryAddRequest()
                {
                    CountryName = "USA"
                };

            //Act
            CountryResponse? expectedValue = await
                this._countriesService.AddCountry(countryAddRequest);
            CountryResponse? actualValue = await this._countriesService.
                GetCountryByCountryID(expectedValue.CountryID);

            //Assert
            Assert.Equal(expectedValue, actualValue);
        }
        #endregion
    }
}