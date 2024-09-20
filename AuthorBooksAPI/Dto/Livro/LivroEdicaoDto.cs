using AuthorBooksAPI.Models;

namespace AuthorBooksAPI.Dto.Livro;

public class LivroEdicaoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Tipo { get; set; }
    public AutorModel Autor { get; set; }

}