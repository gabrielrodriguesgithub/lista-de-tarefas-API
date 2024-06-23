using Microsoft.AspNetCore.Identity;

namespace ListaDeTarefasAPI.Modelos;

public class PessoaComAcesso : IdentityUser<int>
{
    public virtual ICollection<Tarefa> ListaDeTarefas { get; set; } = new List<Tarefa>();


    public void AdicionarTarefa(Tarefa tarefa)
    {
        ListaDeTarefas.Add(tarefa);
    }
}
