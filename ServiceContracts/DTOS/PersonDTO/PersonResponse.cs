using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOS.PersonDTO
{
    /// <summary>
    /// Repersent DTO class which is used as a return value
    /// type for most methods of PersonService
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        public int? Age { get; set; } 

        /// <summary>
        /// Compares current object to parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse object to
        /// compare</param>
        /// <returns>True or False, indicating whether current object
        /// and parameter object are matching</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse personResponse = (PersonResponse)obj;

            return PersonID == personResponse.PersonID &&
                PersonName == personResponse.PersonName;
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return 
                $"Person ID: {this.PersonID}, " +
                $"Person Name: {this.PersonName}, " +
                $"Email: {this.Email}, Date of Birth: " +
                $"{this.DateOfBirth?.ToString("dd MMM yyyy")}, " +
                $"Gender: {this.Gender}, " +
                $"Country ID: {this.CountryID}, " +
                $"Address: {this.Address}, " +
                $"Receive News Letters: {this.ReceiveNewsLetter} " +
                $"Age: {this.Age}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this?.DateOfBirth,
                Gender = (GenderOptions)Enum.Parse
                    (typeof(GenderOptions), this.Gender, true),
                Address = this.Address,
                CountryID = this.CountryID,
                ReceiveNewsLetter = this.ReceiveNewsLetter,
            };
        }
    }
    public static class PersonExtension
    {
        /// <summary>
        /// Convert Person object into PersonResponse object
        /// </summary>
        /// <param name="person">Person object to convert</param>
        /// <returns>New PersonResponse object</returns>
        public static PersonResponse ToPersonResponse
            (this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetter = person.ReceiveNewsLetter,
                Age = CalculateAge(person.DateOfBirth),
                CountryName = person?.Country?.CountryName
            };
        }
        //public static PersonResponse ConvertPersonToPersonResponse
        //    (Person person)
        //{
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    personResponse.CountryName = person?.Country?.CountryName;
        //    return personResponse;
        //}
        public static int? CalculateAge(DateTime? dateTime)
        {
            if (dateTime == null) return null;
            int result = (int)Math.Floor(
                (DateTime.Now - dateTime.Value).TotalDays / 365.25);
            return result;
        }
    }
}
