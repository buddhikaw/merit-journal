using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeritJournal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalEntryTagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the table exists before trying operations on it
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'journalentrytags') THEN
                        ALTER TABLE ""JournalEntryTags"" RENAME TO ""JournalEntryTag"";
                    ELSE
                        -- Create the table if it doesn't exist
                        CREATE TABLE ""JournalEntryTag"" (
                            ""JournalEntryId"" integer NOT NULL,
                            ""TagId"" integer NOT NULL,
                            CONSTRAINT ""PK_JournalEntryTag"" PRIMARY KEY (""JournalEntryId"", ""TagId""),
                            CONSTRAINT ""FK_JournalEntryTag_JournalEntries_JournalEntryId"" FOREIGN KEY (""JournalEntryId"") REFERENCES ""JournalEntries"" (""Id"") ON DELETE CASCADE,
                            CONSTRAINT ""FK_JournalEntryTag_Tags_TagId"" FOREIGN KEY (""TagId"") REFERENCES ""Tags"" (""Id"") ON DELETE CASCADE
                        );
                        
                        CREATE INDEX ""IX_JournalEntryTag_TagId"" ON ""JournalEntryTag"" (""TagId"");
                    END IF;
                END
                $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryTag_JournalEntries_JournalEntryId",
                table: "JournalEntryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryTag_Tags_TagId",
                table: "JournalEntryTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalEntryTag",
                table: "JournalEntryTag");

            migrationBuilder.RenameTable(
                name: "JournalEntryTag",
                newName: "JournalEntryTags");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntryTag_TagId",
                table: "JournalEntryTags",
                newName: "IX_JournalEntryTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalEntryTags",
                table: "JournalEntryTags",
                columns: new[] { "JournalEntryId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryTags_JournalEntries_JournalEntryId",
                table: "JournalEntryTags",
                column: "JournalEntryId",
                principalTable: "JournalEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryTags_Tags_TagId",
                table: "JournalEntryTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
