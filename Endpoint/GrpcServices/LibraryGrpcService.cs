using Application;
using Grpc.Core;

namespace Endpoint.GrpcServices;

public class LibraryGrpcService : LibraryWork.LibraryWorkBase
{
    private readonly LibraryService _libraryService;

    public LibraryGrpcService(LibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public override async Task<AddBookResponse> AddBook(AddBookRequest request, ServerCallContext context)
    {
        await _libraryService.AddBook(ToDomain(request.NewBook));
        return new AddBookResponse();
    }

    public override async Task<AddBooksResponse> AddBooks(AddBooksRequest request, ServerCallContext context)
    {
        await _libraryService.AddBooks(request.NewBooks.Select(ToDomain).ToArray());
        return new AddBooksResponse();
    }

    public override async Task<GetBooksByGenreResponse> GetBooksByGenre(GetBooksByGenreRequest request, ServerCallContext context)
    {
        var books = await _libraryService.GetBooks(request.Genre);
        return new GetBooksByGenreResponse
        {
            Books = { books.Select(ToProto) }
        };
    }

    public override async Task<DeleteBookByIdResponse> DeleteBookById(DeleteBookByIdRequest request, ServerCallContext context)
    {
        await _libraryService.DeleteBookById(request.BookId);
        return new DeleteBookByIdResponse();
    }

    public override async Task<DeleteAllBooksByGenreResponse> DeleteAllBooksByGenre(DeleteAllBooksByGenreRequest request, ServerCallContext context)
    {
        await _libraryService.DeleteBookByGenre(request.Genre);
        return new DeleteAllBooksByGenreResponse();
    }

    public override Task<DeleteBookByIdsResponse> DeleteBookByIds(DeleteBookByIdsRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    private static Domain.Book ToDomain(BookToAdd book)
    {
        return new Domain.Book(Guid.NewGuid(), book.Name, book.Genre, ToDomain(book.Author));
    }

    private static BookToGet ToProto(Domain.Book book)
    {
        return new BookToGet
        {
            Id = book.Id.ToString(),
            Name = book.Name, 
            Genre = book.Genre, 
            Author = ToProto(book.Author)
        };
    }

    private static Domain.Author ToDomain(Author author)
    {
        return new Domain.Author(author.Name, author.Surname);
    }
    
    private static Author ToProto(Domain.Author author)
    {
        return new Author
        {
            Name = author.Name,
            Surname = author.Surname
        };
    }
}