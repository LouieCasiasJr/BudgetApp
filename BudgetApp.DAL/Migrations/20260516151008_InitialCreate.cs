using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpendingBuckets",
                columns: table => new
                {
                    BucketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BucketLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultPriority = table.Column<byte>(type: "tinyint", nullable: false),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketID", x => x.BucketID);
                });

            migrationBuilder.CreateTable(
                name: "EstablishedLinks",
                columns: table => new
                {
                    LinkID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainsText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BucketID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkID", x => x.LinkID);
                    table.ForeignKey(
                        name: "FK_EstablishedLinks_SpendingBuckets",
                        column: x => x.BucketID,
                        principalTable: "SpendingBuckets",
                        principalColumn: "BucketID");
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBudget",
                columns: table => new
                {
                    MonthlyBudgetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BucketID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBudgetID", x => x.MonthlyBudgetID);
                    table.ForeignKey(
                        name: "FK_MonthlyBudget_SpendingBuckets",
                        column: x => x.BucketID,
                        principalTable: "SpendingBuckets",
                        principalColumn: "BucketID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Card = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Debit = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    BucketID = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionID", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_SpendingBuckets",
                        column: x => x.BucketID,
                        principalTable: "SpendingBuckets",
                        principalColumn: "BucketID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishedLinks_BucketID",
                table: "EstablishedLinks",
                column: "BucketID");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudget_BucketID",
                table: "MonthlyBudget",
                column: "BucketID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BucketID",
                table: "Transactions",
                column: "BucketID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishedLinks");

            migrationBuilder.DropTable(
                name: "MonthlyBudget");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "SpendingBuckets");
        }
    }
}
