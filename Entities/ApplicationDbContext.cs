using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public ApplicationDbContext(DbContextOptions options) : 
            base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country> countries = JsonSerializer.
                Deserialize<List<Country>>(countriesJson);
            foreach (var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //Seed to Countries
            string personsJson = File.ReadAllText("persons.json");
            List<Person> persons = JsonSerializer.
                Deserialize<List<Person>>(personsJson);
            foreach (var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent API Add Column
            modelBuilder.Entity<Person>().Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            //Fluent API Add Constraint
            //modelBuilder.Entity<Person>()
            //    .HasIndex(p => p.TIN).IsUnique();

            modelBuilder.Entity<Person>()
                .HasCheckConstraint("CHK_TIN", 
                "len([TaxIdentificationNumber]) = 8");

            //Table Relations
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(p => p.Country)
            //    .WithMany(c => c.Persons)
            //    .HasForeignKey(p => p.CountryID);
            //});
        }
        public List<Person> sp_GetAllPersons()
        {
            var result = Persons.FromSqlRaw
                ("EXECUTE [dbo].[GetAllPersons]").ToList();
            return result;
        }
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetter", 
                person.ReceiveNewsLetter),
            };
            var result = Database.ExecuteSqlRaw
                (@"EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName,
                @Email, @DateOfBirth, @Gender, @CountryID, @Address, 
                @ReceiveNewsLetter", parameters);
            return result;
        }
        public int sp_UpdatePerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetter",
                person.ReceiveNewsLetter),
            };
            var result = Database.ExecuteSqlRaw
                (@"EXECUTE [dbo].[UpdatePerson] @PersonId, @PersonName, 
                @Email, @DateOfBirth, @Gender, @CountryId, @Address, 
                @ReceiveNewsLetter", parameters);
            return result;
        }
    }
}
