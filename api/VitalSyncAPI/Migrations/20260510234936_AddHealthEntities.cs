using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "health_conditions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_conditions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "health_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_health_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "metric_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: false),
                    min_normal = table.Column<double>(type: "double precision", nullable: true),
                    max_normal = table.Column<double>(type: "double precision", nullable: true),
                    icon = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metric_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "health_profile_conditions",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condition_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_profile_conditions", x => new { x.profile_id, x.condition_id });
                    table.ForeignKey(
                        name: "FK_health_profile_conditions_health_conditions_condition_id",
                        column: x => x.condition_id,
                        principalTable: "health_conditions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_health_profile_conditions_health_profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "health_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "health_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_type_id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_health_records_health_profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "health_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_health_records_metric_types_metric_type_id",
                        column: x => x.metric_type_id,
                        principalTable: "metric_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "health_conditions",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Hipertensão" },
                    { 2, "Diabetes tipo 1" },
                    { 3, "Diabetes tipo 2" },
                    { 4, "Obesidade" },
                    { 5, "Doença cardíaca" },
                    { 6, "Asma" },
                    { 7, "Doença renal crônica" }
                });

            migrationBuilder.InsertData(
                table: "metric_types",
                columns: new[] { "id", "icon", "max_normal", "min_normal", "name", "sort_order", "unit" },
                values: new object[,]
                {
                    { 1, "blood-pressure", 120.0, 90.0, "Pressão Sistólica", 1, "mmHg" },
                    { 2, "blood-pressure", 80.0, 60.0, "Pressão Diastólica", 2, "mmHg" },
                    { 3, "droplet", 99.0, 70.0, "Glicemia", 3, "mg/dL" },
                    { 4, "heart-pulse", 100.0, 60.0, "Frequência Cardíaca", 4, "bpm" },
                    { 5, "scale", null, null, "Peso", 5, "kg" },
                    { 6, "lungs", 100.0, 95.0, "SpO2", 6, "%" },
                    { 7, "moon", 9.0, 7.0, "Sono", 7, "h" },
                    { 8, "smile", null, null, "Humor", 8, "1-5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_health_profile_conditions_condition_id",
                table: "health_profile_conditions",
                column: "condition_id");

            migrationBuilder.CreateIndex(
                name: "IX_health_profiles_user_id",
                table: "health_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_health_records_metric_type_id",
                table: "health_records",
                column: "metric_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_health_records_profile_id",
                table: "health_records",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "health_profile_conditions");

            migrationBuilder.DropTable(
                name: "health_records");

            migrationBuilder.DropTable(
                name: "health_conditions");

            migrationBuilder.DropTable(
                name: "health_profiles");

            migrationBuilder.DropTable(
                name: "metric_types");
        }
    }
}
