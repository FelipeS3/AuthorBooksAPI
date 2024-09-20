using System.Text.Json.Serialization;

namespace AuthorBooksAPI.Models;

public class AutorModel
{ 
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }

    /* EF Relations */
    [JsonIgnore]
    public ICollection<LivroModel> livros { get; set; }
}