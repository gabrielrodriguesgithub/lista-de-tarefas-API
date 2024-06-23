namespace ListaDeTarefasAPI.Modelos;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public DateOnly Prazo { get; set; }
    public bool Concluido { get; set; }
    public string Descricao { get; set; }
    public int PessoaComAcessoId { get; set; }
    public virtual PessoaComAcesso PessoaComAcesso { get; set; }

    public Tarefa()
    {
    }
    public Tarefa(int id, string titulo, DateOnly prazo, bool concluido, string descricao)
    {
        Id = id;
        Titulo = titulo;
        Prazo = prazo;
        Concluido = concluido;
        Descricao = descricao;
    }
}

