﻿using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        /// <summary>
        /// Domain Model For Country
        /// </summary>
        ///
        [Key]
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public virtual ICollection<Person>? Persons { get; set;}
    }
}
