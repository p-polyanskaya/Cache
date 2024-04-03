using Domain;
using Microsoft.Extensions.Logging;
using RedisOperations;

namespace Application;

public class LibraryService
{
    private readonly ILogger<LibraryService> _logger;
    private readonly RedisRepository _redisRepository;

    public LibraryService(ILogger<LibraryService> logger, RedisRepository redisRepository)
    {
        _logger = logger;
        _redisRepository = redisRepository;
    }

    public async Task AddBook(Book book)
    {
        await _redisRepository.AddValue(book.Genre, new List<KeyValuePair<Guid, Book>>
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
            await _redisRepository.AddValue(booksByGenre.Key, booksToAdd);
            
            _logger.LogInformation($"Книги с идентификатором {string.Join(", ", booksIdToAdd)} были добавлены в кеш. Жанр {booksByGenre.Key}");
        }
    }

    public async Task<IReadOnlyCollection<Book>> GetBooks(string genre)
    {
        var books = await _redisRepository.Get<Book>(genre);
        return books;
    }

    public async Task DeleteBookById(string id)
    {
        await _redisRepository.DeleteValue(id);
    }
    
    public async Task DeleteBookByGenre(string genre)
    {
        await _redisRepository.DeleteValuesByRootKey(genre);
    }
}