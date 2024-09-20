using AuthorBooksAPI.Data;
using AuthorBooksAPI.Dto.Livro;
using AuthorBooksAPI.Logs;
using AuthorBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;

namespace AuthorBooksAPI.Services.Livro;

public class LivroServices : ILivroInterface
{
    private readonly AppDbContext _context;
    public LivroServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<LivroModel>> BuscarLivroPorId(int idLivro)
    {   

        ResponseModel<LivroModel> resposta = new ResponseModel<LivroModel>();
        try
        {


            var livro = await _context.Livros.FirstOrDefaultAsync(livroBanco => livroBanco.Id == idLivro);
            if (livro == null)
            {
                resposta.Mensagem = "Nenhum registro localizado!";

                Log.LogToFile("Buscar Livro - Aviso", $"Livro Id {idLivro} não encontrado.");

                return resposta;
            }

            resposta.Dados = livro;
            resposta.Mensagem = "Livro localizado com sucesso!";

            Log.LogToFile("Buscar Livro - Sucesso", $"Livro Id {livro.Id} Encontrado: {livro.Titulo} {livro.Tipo}");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Buscar Livro - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<LivroModel>>> BuscarLivroPorIdAutor(int idAutor)
    {
        ResponseModel<List<LivroModel>> resposta = new ResponseModel<List<LivroModel>>();
        try
        {
            Log.LogToFile("Buscar Livro - Acesso", $"Tentativa de buscar livros do autor Id {idAutor}");

            var livro = await _context.Livros
                .Include(a => a.Autor)
                .Where(livroBanco => livroBanco.Autor.Id == idAutor)
                .ToListAsync();

            if (livro == null)
            {
                resposta.Mensagem = "Nenhum registro localizado!";

                Log.LogToFile("Buscar Livro - Aviso", $"Nenhum livro encontrado por Autor Id {idAutor}");

                return resposta;
            }

            resposta.Dados = livro;
            resposta.Mensagem = "Livros localizados!";

            Log.LogToFile("Buscar Livro - Sucesso", $"Livros encontrados por Autor Id {idAutor}: {livro.Count} livros.");

            return resposta;

        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Buscar Livro Por Id Autor - Erro", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }

    }
    public async Task<ResponseModel<List<LivroModel>>> CriarLivro(LivroCriacaoDto livroCriacaoDto)
    {
        ResponseModel<List<LivroModel>> resposta = new ResponseModel<List<LivroModel>>();
        try
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(autorBanco =>
                autorBanco.Id == livroCriacaoDto.Autor.Id);
            if (autor == null)
            {
                resposta.Mensagem = "Nenhum registro de autor localizado!";
                return resposta;
            }

            var livro = new LivroModel()
            {
                Titulo = livroCriacaoDto.Titulo,
                Tipo = livroCriacaoDto.Tipo,
                Autor = autor
            };

            _context.Add(livro);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Livros.Include(a => a.Autor).ToListAsync();
            return resposta;

        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;
            return resposta;
        }
    }

    public async Task<ResponseModel<List<LivroModel>>> EditarLivro(LivroEdicaoDto livroEdicaoDto)
    {
        ResponseModel<List<LivroModel>> resposta = new ResponseModel<List<LivroModel>>();
        try
        {
            Log.LogToFile("Editar Livro - Acesso", $"Tentativa de Edição do Livro Id {livroEdicaoDto.Id}");

            var livro = await _context.Livros.Include(a => a.Autor)
                .FirstOrDefaultAsync(livroBanco => livroBanco.Id == livroEdicaoDto.Id);

            var autor = await _context.Autores.FirstOrDefaultAsync(autorBanco => autorBanco.Id == livroEdicaoDto.Autor.Id);
            if (autor == null)
            {
                resposta.Mensagem = "Nenhum registro de autor localizado!";

                Log.LogToFile("Editar Livro - Aviso", $"Autor Id {livroEdicaoDto.Autor.Id} não encontrado.");

                return resposta;
            }

            if (livro == null)
            {
                resposta.Mensagem = "Nenhum registro de livro localizado!";

                Log.LogToFile("Editar Livro - Aviso", $"Livro ID {livroEdicaoDto.Id} não encontrado.");

                return resposta;
            }

            livro.Titulo = livroEdicaoDto.Titulo;
            livro.Tipo = livroEdicaoDto.Tipo;
            livro.Autor = autor;

            _context.Update(livro);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Livros.ToListAsync();
            resposta.Mensagem = "Dados alterados com sucesso!";

            Log.LogToFile("Editar Livros - Sucesso", $"Livro Id {livro.Id} Editado: {livro.Titulo}, {livro.Tipo}");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Editar Livro - Erro", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<LivroModel>>> ExcluirLivro(int idLivro)
    {
        ResponseModel<List<LivroModel>> resposta = new ResponseModel<List<LivroModel>>();
        try
        {
            var livro = await _context.Livros
                .FirstOrDefaultAsync(livroBanco => livroBanco.Id == idLivro);
            if (livro == null)
            {
                resposta.Mensagem = "Nenhum livro localizado!";

                Log.LogToFile("Excluir Livro - Aviso", $"Livro com Id {idLivro} Não encontrado.");

                return resposta;
            }

            _context.Remove(livro);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Livros.ToListAsync();
            resposta.Mensagem = "Livro removido com sucesso!";

            Log.LogToFile("Excluir Livro - Sucesso", $"Livro Id {idLivro} Excluído com sucesso!");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Excluir Livro - Aviso", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<LivroModel>>> ListarLivros()
    {
        ResponseModel<List<LivroModel>> resposta = new ResponseModel<List<LivroModel>>();
        try
        {

            Log.LogToFile("Listar Livros - Acesso", "Tentativa de listar todos os livros.");

            var livros = await _context.Livros.Include(a=>a.Autor).ToListAsync();

            resposta.Dados = livros;
            resposta.Mensagem = "Todos os livros foram listados com sucesso!";

            Log.LogToFile("Listar Livros - Sucesso", $"Livros listados com sucesso! Total: {livros.Count}");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Listar Livros - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }
}