using ListaDeTarefasAPI.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Configuration;

namespace ListaDeTarefasAPI.Dados;

public class ListaDeTarefasContext : IdentityDbContext <PessoaComAcesso, PerfilDeAcesso, int>
{
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<PessoaComAcesso> PessoaComAcesso { get; set; }

    private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    public ListaDeTarefasContext()
    {

    }
    public ListaDeTarefasContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        optionsBuilder
            .UseSqlServer(connectionString).UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
