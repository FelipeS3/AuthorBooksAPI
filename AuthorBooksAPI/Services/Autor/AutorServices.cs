using AuthorBooksAPI.Data;
using AuthorBooksAPI.Dto.Autor;
using AuthorBooksAPI.Logs;
using AuthorBooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorBooksAPI.Services.Autor;

public class AutorServices : IAutorInterface
{
    private readonly AppDbContext _context;

    public AutorServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<AutorModel>> BuscarAutorPorId(int idAutor)
    {
        ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();
        try
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(autorBanco => autorBanco.Id == idAutor);

            if (autor == null)
            {
                resposta.Mensagem = "Nenhum registro localizado!";

                Log.LogToFile("Buscar Autor - Aviso", $"Autor Id {idAutor} não encontrado.");

                return resposta;
            }

            resposta.Dados = autor;
            resposta.Mensagem = "Autor Localizado!";

            Log.LogToFile("Buscar Autor - Sucesso", $"Autor Id {autor.Id} Encontrado: {autor.Nome} {autor.Sobrenome}");


            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Buscar Autor - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<AutorModel>> BuscarAutorPorIdLivro(int idLivro)
    {
        ResponseModel<AutorModel> resposta = new ResponseModel<AutorModel>();
        try
        {
            Log.LogToFile("Buscar Autor - Acesso", $"Buscar Autor por livro Id {idLivro}");

            var livro = await _context.Livros
                .Include(a => a.Autor)
                .FirstOrDefaultAsync(livroBanco => livroBanco.Id == idLivro);
            if (livro == null)
            {
                resposta.Mensagem = "Nenhum registro localizado!";

                Log.LogToFile("Buscar Autor - Aviso", $"Livro Id {idLivro} não encontrado.");

                return resposta;
            }

            resposta.Dados = livro.Autor;
            resposta.Mensagem = "Autor localizado!";

            Log.LogToFile("Buscar Autor - Sucesso", $"Autor encontrado para livro ID {idLivro}: {livro.Autor.Nome} {livro.Autor.Sobrenome}");
            return resposta;

        }

        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Buscar Autor - Erro", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<AutorModel>>> CriarAutor(AutorCriacaoDto autorCriacaoDto)
    {
        ResponseModel<List<AutorModel>> resposta = new ResponseModel<List<AutorModel>>();
        try
        {
            var autor = new AutorModel()
            {
                Nome = autorCriacaoDto.Nome,
                Sobrenome = autorCriacaoDto.Sobrenome
            };

            _context.Add(autor);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Autores.ToListAsync();
            resposta.Status = true;
            resposta.Mensagem = "Autor criado com sucesso!";

            Log.LogToFile("Criar Autor - Sucesso", $"Autor Id {autor.Id} Criado com sucesso!");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Criar Autor - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<AutorModel>>> EditarAutor(AutorEdicaoDto autorEdicaoDto)
    {
        ResponseModel<List<AutorModel>> resposta = new ResponseModel<List<AutorModel>>();
        try
        {
            Log.LogToFile("Editar Autor - Acesso", $"Tentativa de edição do autor ID {autorEdicaoDto.Id}");

            var autor = await _context.Autores.FirstOrDefaultAsync(autorBanco => autorBanco.Id == autorEdicaoDto.Id);
            if (autor == null)
            {
                resposta.Mensagem = "Nenhum autor localizado!";

                Log.LogToFile("Editar Autor - Aviso", $"Autor Id {autorEdicaoDto.Id} não encontrado.");

                return resposta;
            }

            autor.Nome = autorEdicaoDto.Nome;
            autor.Sobrenome = autorEdicaoDto.Sobrenome;

            _context.Update(autor);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Autores.ToListAsync();
            resposta.Mensagem = "Dados alterados com sucesso!";

            Log.LogToFile("Editar Autor - Sucesso", $"Autor ID {autor.Id} Editado: {autor.Nome} {autor.Sobrenome}");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message; 
            resposta.Status = false;

            Log.LogToFile("Editar Autor - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<AutorModel>>> ExcluirAutor(int idAutor)
    {
        ResponseModel<List<AutorModel>> resposta = new ResponseModel<List<AutorModel>>();
        try
        {
            var autor = await _context.Autores
                .FirstOrDefaultAsync(autorBanco => autorBanco.Id == idAutor);
            if (autor == null)
            {
                resposta.Mensagem = "Nenhum autor localizado!";

                Log.LogToFile("Excluir Autor - Aviso", $"Autor com Id {idAutor} Não encontrado.");

                return resposta;

            }

            _context.Remove(autor);
            await _context.SaveChangesAsync();

            resposta.Dados = await _context.Autores.ToListAsync();
            resposta.Mensagem = "Autor removido com sucesso!";

            Log.LogToFile("Excluir Autor - Sucesso", $"Autor Id {idAutor} Excluído com sucesso!");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Excluir Autor - Error", $"{ex.Message}\n{ex.StackTrace}");

            return resposta;
        }
    }

    public async Task<ResponseModel<List<AutorModel>>> ListarAutores()
    {
        ResponseModel<List<AutorModel>> resposta = new ResponseModel<List<AutorModel>>();
        try
        {
            Log.LogToFile("Listar Autores - Acesso", $"Tentativa de listar todos os Autores.");

            var autores = await _context.Autores.ToListAsync();

            resposta.Dados = autores;
            resposta.Mensagem = "Todos os autores foram listados com sucesso!";

            Log.LogToFile("Listar Autores - Sucesso", $"Autores listados com sucesso! Total: {autores.Count}");

            return resposta;
        }
        catch (Exception ex)
        {
            resposta.Mensagem = ex.Message;
            resposta.Status = false;

            Log.LogToFile("Listar Autores - Error", $"{ex.Message}\n{ex.StackTrace}");
            return resposta;
        }
    }

}