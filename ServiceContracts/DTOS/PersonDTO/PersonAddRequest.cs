using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServiceContracts.DTOS.PersonDTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = 
            "Email value should be a valid email")]
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        /// <summary>
        /// Convert PersonAddRequest object into
        /// Person object
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = ConvertGenderOptionsToString(),
                CountryID = this.CountryID,
                Address = this.Address,
                ReceiveNewsLetter = this.ReceiveNewsLetter,
            };
        }
        public string ConvertGenderOptionsToString()
        {
            switch (this.Gender)
            {
                case GenderOptions.Male: return "Male";
                case GenderOptions.Female: return "Female";
                default: return "Other";
            }
        }
    }
}
