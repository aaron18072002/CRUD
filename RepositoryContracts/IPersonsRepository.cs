using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person object to the data source
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Return the added person object after
        /// add it into the datasource</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Return all person in the data source
        /// </summary>
        /// <returns>List of person object from table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Return a person object that have id matching with 
        /// given personID
        /// </summary>
        /// <param name="personID">PersonID(Guid) to search</param>
        /// <returns>Matching person or null</returns>
        Task<Person> GetPersonById(Guid personID);

        /// <summary>
        /// Return all person objects based on give expression
        /// </summary>
        /// <param name="predicate">LINQ Expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons
            (Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person that have id matching with personID
        /// </summary>
        /// <param name="personID">Person ID to delete</param>
        /// <returns>Return true if the deletion is sucessful and
        /// </returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates a person object (person name and other details) 
        /// based on the given person id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Return a updated person after update it 
        /// on the datasource</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
