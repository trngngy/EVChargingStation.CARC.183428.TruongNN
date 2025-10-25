using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVChargingStation.CARC.Domain.TruongNN.Migrations
{
    /// <inheritdoc />
    public partial class fixtableSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_InvoiceTruongNN_InvoiceId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_ReservationLongLQ_ReservationId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Sessions",
                newName: "ReservationLongLQId");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Sessions",
                newName: "InvoiceTruongNNId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_ReservationId",
                table: "Sessions",
                newName: "IX_Sessions_ReservationLongLQId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_InvoiceId",
                table: "Sessions",
                newName: "IX_Sessions_InvoiceTruongNNId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_InvoiceTruongNN_InvoiceTruongNNId",
                table: "Sessions",
                column: "InvoiceTruongNNId",
                principalTable: "InvoiceTruongNN",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_ReservationLongLQ_ReservationLongLQId",
                table: "Sessions",
                column: "ReservationLongLQId",
                principalTable: "ReservationLongLQ",
                principalColumn: "TruongNNID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_InvoiceTruongNN_InvoiceTruongNNId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_ReservationLongLQ_ReservationLongLQId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ReservationLongLQId",
                table: "Sessions",
                newName: "ReservationId");

            migrationBuilder.RenameColumn(
                name: "InvoiceTruongNNId",
                table: "Sessions",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_ReservationLongLQId",
                table: "Sessions",
                newName: "IX_Sessions_ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_InvoiceTruongNNId",
                table: "Sessions",
                newName: "IX_Sessions_InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_InvoiceTruongNN_InvoiceId",
                table: "Sessions",
                column: "InvoiceId",
                principalTable: "InvoiceTruongNN",
                principalColumn: "TruongNNID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_ReservationLongLQ_ReservationId",
                table: "Sessions",
                column: "ReservationId",
                principalTable: "ReservationLongLQ",
                principalColumn: "TruongNNID");
        }
    }
}
