using AuthorBooksAPI.Dto.Vinculo;
using AuthorBooksAPI.Models;

namespace AuthorBooksAPI.Dto.Livro;

public class LivroCriacaoDto
{
    public string Titulo {get; set; }
    public string Tipo { get; set; }
    public AutorVinculoDto Autor { get; set; }
}