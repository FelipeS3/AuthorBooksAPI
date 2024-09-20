using AuthorBooksAPI.Dto.Autor;
using AuthorBooksAPI.Dto.Livro;
using AuthorBooksAPI.Models;

namespace AuthorBooksAPI.Services.Livro;

public interface ILivroInterface
{
    Task<ResponseModel<List<LivroModel>>> ListarLivros();
    Task<ResponseModel<LivroModel>> BuscarLivroPorId(int idLivro);
    Task<ResponseModel<List<LivroModel>>> BuscarLivroPorIdAutor(int idAutor);
    Task<ResponseModel<List<LivroModel>>> CriarLivro(LivroCriacaoDto livroCriacaoDto);
    Task<ResponseModel<List<LivroModel>>> EditarLivro(LivroEdicaoDto livroEdicaoDto);
    Task<ResponseModel<List<LivroModel>>> ExcluirLivro(int idLivro);
}