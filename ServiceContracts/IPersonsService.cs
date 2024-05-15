using ServiceContracts.DTOS.PersonDTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Add person to datasource
        /// </summary>
        /// <param name="request">The PersonAddRequest object</param>
        /// <returns>Return a PersonResponse</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? request);

        /// <summary>
        /// Get all persons from datasource
        /// </summary>
        /// <returns>Return list of PersonResponse objects</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Get Person object by given person ID
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Return matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Return all person objects that matches with the given
        /// search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Searcj string to search</param>
        /// <returns>Returns all matching object based on 
        /// search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons
            (string searchBy, string? searchString);

        /// <summary>
        /// Return sorted of persons    
        /// </summary>
        /// <param name="allPersons">Represent list of persons
        /// to sort</param>
        /// <param name="sortBy">Repersent field you want to sort</param>
        /// <param name="sortOrderOptions">ASC or DESC</param>
        /// <returns>Return list of persons after sorting</returns>
        Task<List<PersonResponse>> GetSortedPersons
            (List<PersonResponse> allPersons,
            string sortBy, SortOrderOptions sortOrderOptions);

        /// <summary>
        /// Use specific details of an person to update
        /// this person
        /// </summary>
        /// <param name="request">Specific details person</param>
        /// <returns>Return person object after updated</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);

        /// <summary>
        /// Delete an person object in datasource by
        /// specific id of person
        /// </summary>
        /// <param name="personID">Represent ID of person want to 
        /// remove</param>
        /// <returns>Return true or false</returns>
        Task<bool> DeletePerson(Guid? personID);

        /// <summary>
        /// Return persons as CSV
        /// </summary>
        /// <returns>Return the memory stream 
        /// with CSV data</returns>
        Task<MemoryStream> GetPersonCSV();
    }
}
