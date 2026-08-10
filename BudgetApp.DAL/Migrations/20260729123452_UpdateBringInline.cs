using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBringInline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[MonthlySums]
                AS 
                SELECT
                    BucketID, 
                    DATEPART(YEAR, TransactionDate) AS TransactionYear, 
                    DATEPART(MONTH, TransactionDate) AS TransactionMonth, 
                    SUM(CASE WHEN Debit = 1 THEN - Amount ELSE Amount END) AS TotalAmount
                FROM dbo.Transactions
                GROUP BY 
                    BucketID, 
                   DATEPART(YEAR, TransactionDate), 
                    DATEPART(MONTH, TransactionDate)
            ");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "MonthlyBudget",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BudgetPriorities",
                columns: table => new
                {
                    PriorityID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriorityID", x => x.PriorityID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpendingBuckets_DefaultPriority",
                table: "SpendingBuckets",
                column: "DefaultPriority");

            migrationBuilder.AddForeignKey(
                name: "FK_SpendingBuckets_BudgetPriorities",
                table: "SpendingBuckets",
                column: "DefaultPriority",
                principalTable: "BudgetPriorities",
                principalColumn: "PriorityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpendingBuckets_BudgetPriorities",
                table: "SpendingBuckets");

            migrationBuilder.DropTable(
                name: "BudgetPriorities");

            migrationBuilder.DropIndex(
                name: "IX_SpendingBuckets_DefaultPriority",
                table: "SpendingBuckets");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "MonthlyBudget");
        }
    }
}
