using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "block",
                columns: table => new
                {
                    idx = table.Column<long>(type: "INTEGER", nullable: false),
                    nonce = table.Column<long>(type: "INTEGER", nullable: false),
                    hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    mrkl_root = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    previous_hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_block", x => x.idx);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    content = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    occured_on_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    error = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outbox_message", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "voter",
                columns: table => new
                {
                    addr = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    pkey = table.Column<string>(type: "TEXT", maxLength: 182, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_voter", x => x.addr);
                });

            migrationBuilder.CreateTable(
                name: "vote",
                columns: table => new
                {
                    hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    addr = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    party_id = table.Column<int>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    signature = table.Column<string>(type: "TEXT", nullable: false),
                    nonce = table.Column<long>(type: "INTEGER", nullable: false),
                    block_index = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vote", x => x.hash);
                    table.ForeignKey(
                        name: "f_k_vote__voter_addr",
                        column: x => x.addr,
                        principalTable: "voter",
                        principalColumn: "addr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_vote_block_block_index",
                        column: x => x.block_index,
                        principalTable: "block",
                        principalColumn: "idx");
                });

            migrationBuilder.CreateIndex(
                name: "i_x_block_hash",
                table: "block",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vote_addr",
                table: "vote",
                column: "addr");

            migrationBuilder.CreateIndex(
                name: "i_x_vote_block_index",
                table: "vote",
                column: "block_index");

            migrationBuilder.CreateIndex(
                name: "i_x_vote_party_id",
                table: "vote",
                column: "party_id");

            migrationBuilder.CreateIndex(
                name: "i_x_voter_pkey",
                table: "voter",
                column: "pkey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_message");

            migrationBuilder.DropTable(
                name: "vote");

            migrationBuilder.DropTable(
                name: "voter");

            migrationBuilder.DropTable(
                name: "block");
        }
    }
}
