using Microsoft.AspNetCore.Mvc;
using Books_API.Services;

namespace Books_API.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly string _booksDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _booksDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
            _bookService = bookService;
        }

        [HttpGet]
        public IActionResult GetBooks()
        {

            var bookTitles = _bookService.GetBooks();
            return Ok(bookTitles);
        }

        public class BookModel
        {
            public string BookName { get; set; }
            public string BookText { get; set; }
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] BookModel book)
        {
            string fileName = _bookService.AddBook(book.BookName, book.BookText);

            return Created(fileName, "Book added successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveBook(string id)
        {

            _bookService.RemoveBook(id);
            return Ok("Book removed successfully.");
        }

        [HttpGet("{id}")]
        public IActionResult GetTopWords(string id)
        {
            var topWords = _bookService.GetTopWords(id);
            return Ok(topWords);
        }

        [HttpGet("{id}/search/{prefix}")]
        public IActionResult WordSearchByPrefix(string id, string prefix)
        {
            var words = _bookService.WordSearchByPrefix(id, prefix);
            return Ok(words);
        }
    }
}
