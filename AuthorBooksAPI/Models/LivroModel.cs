namespace AuthorBooksAPI.Models;

public class LivroModel
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Tipo { get; set; }
    
    /* EF Relations */
    public AutorModel Autor { get; set; }
}