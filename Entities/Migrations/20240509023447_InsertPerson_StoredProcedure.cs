using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class InsertPerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
                CREATE PROCEDURE [dbo].[InsertPerson]
                (
                    @PersonID uniqueidentifier, 
                    @PersonName nvarchar(40), 
                    @Email nvarchar(50), 
                    @DateOfBirth datetime2(7), 
                    @Gender varchar(10), 
                    @CountryID uniqueidentifier, 
                    @Address nvarchar(1000),
                    @ReceiveNewsLetter bit
                )
                AS 
                BEGIN
                    INSERT INTO [dbo].[Persons](PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetter)
                    VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetter)
                END";
            migrationBuilder.Sql(sp_InsertPerson);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = 
                @"DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
