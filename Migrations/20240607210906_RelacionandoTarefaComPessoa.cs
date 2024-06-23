using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListaDeTarefasAPI.Migrations
{
    /// <inheritdoc />
    public partial class RelacionandoTarefaComPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_AspNetUsers_PessoaComAcessoId",
                table: "Tarefas");

            migrationBuilder.AlterColumn<int>(
                name: "PessoaComAcessoId",
                table: "Tarefas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_AspNetUsers_PessoaComAcessoId",
                table: "Tarefas",
                column: "PessoaComAcessoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_AspNetUsers_PessoaComAcessoId",
                table: "Tarefas");

            migrationBuilder.AlterColumn<int>(
                name: "PessoaComAcessoId",
                table: "Tarefas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_AspNetUsers_PessoaComAcessoId",
                table: "Tarefas",
                column: "PessoaComAcessoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
