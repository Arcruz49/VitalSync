using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "body_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight_kg = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    bmi = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    ideal_weight_min_kg = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ideal_weight_max_kg = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    bmr = table.Column<decimal>(type: "numeric(7,2)", nullable: false),
                    tdee = table.Column<decimal>(type: "numeric(7,2)", nullable: false),
                    calorie_goal = table.Column<decimal>(type: "numeric(7,2)", nullable: false),
                    protein_goal_g = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    carbs_goal_g = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    fat_goal_g = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_body_metrics", x => x.id);
                    table.ForeignKey(
                        name: "FK_body_metrics_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personal_ranges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_type_id = table.Column<int>(type: "integer", nullable: false),
                    min_normal = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    max_normal = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    method = table.Column<int>(type: "integer", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal_ranges", x => x.id);
                    table.ForeignKey(
                        name: "FK_personal_ranges_metric_types_metric_type_id",
                        column: x => x.metric_type_id,
                        principalTable: "metric_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_personal_ranges_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_medications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medication_class = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_medications", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_medications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    height_cm = table.Column<decimal>(type: "numeric", nullable: false),
                    activity_level = table.Column<int>(type: "integer", nullable: false),
                    goal = table.Column<int>(type: "integer", nullable: false),
                    target_weight_kg = table.Column<decimal>(type: "numeric", nullable: true),
                    waist_circumference_cm = table.Column<decimal>(type: "numeric", nullable: true),
                    body_fat_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    training_frequency_days = table.Column<int>(type: "integer", nullable: false),
                    training_types = table.Column<string>(type: "text", nullable: false),
                    hours_seated_per_day = table.Column<int>(type: "integer", nullable: false),
                    habitual_sleep_hours = table.Column<decimal>(type: "numeric", nullable: false),
                    sleep_quality = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Pré-diabetes");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Obesidade");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Doença cardíaca");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 7,
                column: "name",
                value: "Colesterol alto");

            migrationBuilder.InsertData(
                table: "health_conditions",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 8, "Hipotireoidismo" },
                    { 9, "Hipertireoidismo" },
                    { 10, "Asma" },
                    { 11, "DPOC" },
                    { 12, "Doença renal crônica" },
                    { 13, "Apneia do sono" },
                    { 14, "Ansiedade" },
                    { 15, "Depressão" },
                    { 16, "Fibromialgia" }
                });

            migrationBuilder.UpdateData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Glicemia em jejum");

            migrationBuilder.InsertData(
                table: "metric_types",
                columns: new[] { "id", "icon", "max_normal", "min_normal", "name", "sort_order", "unit" },
                values: new object[,]
                {
                    { 9, "thermometer", 37.5, 36.0, "Temperatura Corporal", 9, "°C" },
                    { 10, "droplet", 140.0, null, "Glicemia Pós-prandial", 10, "mg/dL" },
                    { 11, "footsteps", null, 5000.0, "Passos Diários", 11, "passos" },
                    { 12, "brain", 4.0, null, "Nível de Estresse", 12, "1-10" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_body_metrics_user_id",
                table: "body_metrics",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_personal_ranges_metric_type_id",
                table: "personal_ranges",
                column: "metric_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_personal_ranges_user_id_metric_type_id",
                table: "personal_ranges",
                columns: new[] { "user_id", "metric_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_medications_user_id",
                table: "user_medications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "body_metrics");

            migrationBuilder.DropTable(
                name: "personal_ranges");

            migrationBuilder.DropTable(
                name: "user_medications");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Obesidade");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Doença cardíaca");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Asma");

            migrationBuilder.UpdateData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 7,
                column: "name",
                value: "Doença renal crônica");

            migrationBuilder.UpdateData(
                table: "metric_types",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Glicemia");
        }
    }
}
