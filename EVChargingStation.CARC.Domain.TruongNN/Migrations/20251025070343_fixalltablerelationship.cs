using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVChargingStation.CARC.Domain.TruongNN.Migrations
{
    /// <inheritdoc />
    public partial class fixalltablerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connectors_StationAnhDHV_StationId",
                table: "Connectors");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_InvoiceTruongNN_InvoiceId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_StationAnhDHV_StationId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffStations_StationAnhDHV_StationId",
                table: "StaffStations");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "StaffStations",
                newName: "StationAnhDHVId");

            migrationBuilder.RenameIndex(
                name: "IX_StaffStations_StationId",
                table: "StaffStations",
                newName: "IX_StaffStations_StationAnhDHVId");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Recommendations",
                newName: "StationAnhDHVId");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_StationId",
                table: "Recommendations",
                newName: "IX_Recommendations_StationAnhDHVId");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Payments",
                newName: "InvoiceTruongNNId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                newName: "IX_Payments_InvoiceTruongNNId");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Connectors",
                newName: "StationAnhDHVId");

            migrationBuilder.RenameIndex(
                name: "IX_Connectors_StationId",
                table: "Connectors",
                newName: "IX_Connectors_StationAnhDHVId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connectors_StationAnhDHV_StationAnhDHVId",
                table: "Connectors",
                column: "StationAnhDHVId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_InvoiceTruongNN_InvoiceTruongNNId",
                table: "Payments",
                column: "InvoiceTruongNNId",
                principalTable: "InvoiceTruongNN",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_StationAnhDHV_StationAnhDHVId",
                table: "Recommendations",
                column: "StationAnhDHVId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffStations_StationAnhDHV_StationAnhDHVId",
                table: "StaffStations",
                column: "StationAnhDHVId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connectors_StationAnhDHV_StationAnhDHVId",
                table: "Connectors");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_InvoiceTruongNN_InvoiceTruongNNId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_StationAnhDHV_StationAnhDHVId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffStations_StationAnhDHV_StationAnhDHVId",
                table: "StaffStations");

            migrationBuilder.RenameColumn(
                name: "StationAnhDHVId",
                table: "StaffStations",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_StaffStations_StationAnhDHVId",
                table: "StaffStations",
                newName: "IX_StaffStations_StationId");

            migrationBuilder.RenameColumn(
                name: "StationAnhDHVId",
                table: "Recommendations",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_StationAnhDHVId",
                table: "Recommendations",
                newName: "IX_Recommendations_StationId");

            migrationBuilder.RenameColumn(
                name: "InvoiceTruongNNId",
                table: "Payments",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_InvoiceTruongNNId",
                table: "Payments",
                newName: "IX_Payments_InvoiceId");

            migrationBuilder.RenameColumn(
                name: "StationAnhDHVId",
                table: "Connectors",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Connectors_StationAnhDHVId",
                table: "Connectors",
                newName: "IX_Connectors_StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connectors_StationAnhDHV_StationId",
                table: "Connectors",
                column: "StationId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_InvoiceTruongNN_InvoiceId",
                table: "Payments",
                column: "InvoiceId",
                principalTable: "InvoiceTruongNN",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_StationAnhDHV_StationId",
                table: "Recommendations",
                column: "StationId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffStations_StationAnhDHV_StationId",
                table: "StaffStations",
                column: "StationId",
                principalTable: "StationAnhDHV",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
