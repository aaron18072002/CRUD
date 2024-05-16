using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<Person> AddPerson(Person person)
        {
            this._db.Add(person);
            await this._db.SaveChangesAsync();
            return person;
        }
        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            this._db.Persons.RemoveRange(this._db.Persons.
                Where(p => p.PersonID == personID));
            int rowsAffected = await this._db.SaveChangesAsync();
            if(rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Person>> GetAllPersons()
        {
            var result = await this._db.Persons.Include("Country")
                .ToListAsync();
            return result;
        }
        public async Task<List<Person>> GetFilteredPersons
            (Expression<Func<Person, bool>> predicate)
        {
            var result = await this._db.Persons.Include("Country")
                .Where(predicate).ToListAsync();
            return result;
        }
        public async Task<Person?> GetPersonById(Guid personID)
        {
            var person = await this._db.Persons.Include("Country")
                .FirstOrDefaultAsync(p => p.PersonID == personID);
            return person;
        }
        public async Task<Person> UpdatePerson(Person person)
        {
            var matchingPerson = await this._db.Persons.Include("Country")
                .FirstOrDefaultAsync(p => p.PersonID == person.PersonID);
            if (matchingPerson == null)
            {
                return person;
            }
            matchingPerson.PersonName = person.PersonName;  
            matchingPerson.Gender = person.Gender;  
            matchingPerson.Address = person.Address;  
            matchingPerson.Email = person.Email;  
            matchingPerson.DateOfBirth = person.DateOfBirth;  
            matchingPerson.ReceiveNewsLetter = person.ReceiveNewsLetter;  

            await this._db.SaveChangesAsync();

            return matchingPerson;
        }
    }
}
