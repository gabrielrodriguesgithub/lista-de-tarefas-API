using ListaDeTarefasAPI.Dados;
using ListaDeTarefasAPI.Modelos;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ListaDeTarefasAPI.EndPoints;

public static class TarefaExtension
{
    public static void AddEndPointsTarefas(this WebApplication app)
    {
        
        var groupBuilder = app.MapGroup("tarefas").RequireAuthorization().WithTags("Tarefas");
       

        groupBuilder.MapGet("", async (
            HttpContext context,
            ListaDeTarefasContext db ) =>
        {
            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value
           ?? throw new InvalidOperationException("Pessoa não está conectada");

            var pessoa = db.PessoaComAcesso.FirstOrDefault(p => p.Email.Equals(email))
            ?? throw new InvalidOperationException("Pessoa não está conectada");



            var listaTarefas = await db.Tarefas.Where(t => t.PessoaComAcessoId.Equals(pessoa.Id)).ToListAsync();
            if (listaTarefas.Count == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(listaTarefas);
        });

        groupBuilder.MapGet("{id}", async (ListaDeTarefasContext db, int id) =>
        {
            var tarefa =  await db.Tarefas.FirstOrDefaultAsync(t => t.Id.Equals(id));
            if (tarefa is null) return Results.NotFound();
            return Results.Ok(tarefa);
        });

        groupBuilder.MapPost("", (HttpContext context,
            ListaDeTarefasContext db, 
            [FromBody] Tarefa tarefa ) =>
        {
            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value 
            ?? throw new InvalidOperationException("Pessoa não está conectada");

            var pessoa = db.PessoaComAcesso.FirstOrDefault(p => p.Email.Equals(email)) 
            ?? throw new InvalidOperationException("Pessoa não está conectada");

            pessoa.AdicionarTarefa(tarefa);
            
            db.Tarefas.Add(tarefa);
            db.SaveChanges();
            return Results.Ok(tarefa);
        });

        groupBuilder.MapPut("", async (ListaDeTarefasContext db, [FromBody] Tarefa tarefa) =>
        {
            var tarefaRecuperada = await db.Tarefas.FirstOrDefaultAsync(t => t.Id.Equals(tarefa.Id));
            if(tarefaRecuperada is null) return Results.NotFound();   

            if(tarefa.Titulo is not null)
            {
                tarefaRecuperada.Titulo = tarefa.Titulo;
            }
            if (tarefa.Prazo != tarefaRecuperada.Prazo)
            {
                tarefaRecuperada.Prazo = tarefa.Prazo;
            }
            if (tarefa.Concluido != tarefaRecuperada.Concluido)
            {
                tarefaRecuperada.Concluido = tarefa.Concluido;
            }
            if (tarefa.Descricao is not null)
            {
                tarefaRecuperada.Descricao = tarefa.Descricao;
            }

            await db.SaveChangesAsync();
            return Results.Ok();
        });

        groupBuilder.MapDelete("{id}", (ListaDeTarefasContext db, int id) =>
        {
            var tarefa = db.Tarefas.FirstOrDefault(t => t.Id.Equals(id));
            if (tarefa is null) return Results.NotFound();
            db.Tarefas.Remove(tarefa);
            db.SaveChanges();
            return Results.NoContent();
        });

    }
}
