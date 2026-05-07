using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ustalar.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "NameAz", "NameRu", "Slug" },
                values: new object[,]
                {
                    { 1, "Bakı", "Баку", "baku" },
                    { 2, "Gəncə", "Гянджа", "ganja" },
                    { 3, "Sumqayıt", "Сумгайыт", "sumgayit" },
                    { 4, "Mingəçevir", "Мингечевир", "mingachevir" },
                    { 5, "Lənkəran", "Ленкорань", "lankaran" },
                    { 6, "Şirvan", "Ширван", "shirvan" },
                    { 7, "Naxçıvan", "Нахчыван", "nakhchivan" },
                    { 8, "Quba", "Куба", "quba" }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "NameAz", "NameRu", "Slug" },
                values: new object[,]
                {
                    { 1, "Elektrik", "Электрик", "elektrik" },
                    { 2, "Santexnik", "Сантехник", "santexnik" },
                    { 3, "Rəssam-Boyaqçı", "Маляр", "boyaqci" },
                    { 4, "Dülgər", "Плотник", "dulger" },
                    { 5, "Qaynaqçı", "Сварщик", "qaynaqci" },
                    { 6, "Kafel ustası", "Плиточник", "kafel" },
                    { 7, "Gipsokarton ustası", "Гипсокартон", "gipsokarton" },
                    { 8, "Kondisioner montajı", "Кондиционеры", "kondisioner" },
                    { 9, "Parket ustası", "Паркетчик", "parket" },
                    { 10, "Suvaqçı", "Штукатур", "suvaqci" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
