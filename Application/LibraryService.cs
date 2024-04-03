using Domain;
using Microsoft.Extensions.Logging;
using RedisOperations;

namespace Application;

public class LibraryService
{
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(ILogger<LibraryService> logger)
    {
        _logger = logger;
    }

    public async Task AddBook(Book book)
    {
        await RedisRepository.AddValue(book.Genre, new List<KeyValuePair<Guid, Book>>
        {
            new(book.Id, book)
        });
        _logger.LogInformation($"Книга с идентификатором {book.Id} была добавлена в кеш.");
    }

    public async Task AddBooks(IEnumerable<Book> books)
    {
        var booksByGenres = books.GroupBy(book => book.Genre);

        foreach (var booksByGenre in booksByGenres)
        {
            var booksToAdd = booksByGenre
                .Select(book => new KeyValuePair<Guid, Book>(book.Id, book))
                .ToArray();
            var booksIdToAdd = booksToAdd
                .Select(book => book.Value.Id.ToString());
            await RedisRepository.AddValue(booksByGenre.Key, booksToAdd);
            
            _logger.LogInformation($"Книги с идентификатором {string.Join(", ", booksIdToAdd)} были добавлены в кеш. Жанр {booksByGenre.Key}");
        }
    }

    public async Task<IReadOnlyCollection<Book>> GetBooks(string genre)
    {
        var books = await RedisRepository.Get<Book>(genre);
        return books;
    }
}