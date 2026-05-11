using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthEntitiesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_health_records_health_profiles_profile_id",
                table: "health_records");

            migrationBuilder.DropTable(
                name: "health_profile_conditions");

            migrationBuilder.DropTable(
                name: "health_profiles");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "health_records",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_health_records_profile_id",
                table: "health_records",
                newName: "IX_health_records_user_id");

            migrationBuilder.CreateTable(
                name: "user_conditions",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condition_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_conditions", x => new { x.user_id, x.condition_id });
                    table.ForeignKey(
                        name: "FK_user_conditions_health_conditions_condition_id",
                        column: x => x.condition_id,
                        principalTable: "health_conditions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_conditions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_conditions_condition_id",
                table: "user_conditions",
                column: "condition_id");

            migrationBuilder.AddForeignKey(
                name: "FK_health_records_users_user_id",
                table: "health_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_health_records_users_user_id",
                table: "health_records");

            migrationBuilder.DropTable(
                name: "user_conditions");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "health_records",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_health_records_user_id",
                table: "health_records",
                newName: "IX_health_records_profile_id");

            migrationBuilder.CreateTable(
                name: "health_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_health_profile_conditions_condition_id",
                table: "health_profile_conditions",
                column: "condition_id");

            migrationBuilder.CreateIndex(
                name: "IX_health_profiles_user_id",
                table: "health_profiles",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_health_records_health_profiles_profile_id",
                table: "health_records",
                column: "profile_id",
                principalTable: "health_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
