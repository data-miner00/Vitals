using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitals.Integrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateTriggerForUserAndCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TRIGGER SetUserUpdatedDate
                AFTER UPDATE ON Users
                FOR EACH ROW
                BEGIN
                    UPDATE Users
                    SET UpdatedAt = CURRENT_TIMESTAMP
                    WHERE Id = OLD.Id;
                END;

                CREATE TRIGGER SetCredentialUpdatedDate
                AFTER UPDATE ON Credentials
                FOR EACH ROW
                BEGIN
                    UPDATE Credentials
                    SET UpdatedAt = CURRENT_TIMESTAMP
                    WHERE Id = OLD.Id;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS SetUserUpdatedDate;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS SetCredentialUpdatedDate;");
        }
    }
}
