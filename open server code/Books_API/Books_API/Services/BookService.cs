using System.Text.RegularExpressions;
using System.Collections;

namespace Books_API.Services
{
    public class BookService
    {
              private readonly string _booksDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
              private Dictionary<string, IEnumerable> _topWordsCache = new Dictionary<string, IEnumerable>();

        public IEnumerable<string> GetBooks()
        {
            try
            {
                var bookTitles = Directory.GetFiles(_booksDirectory)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(title => ToTitleCase(title))
                .ToList();
                return bookTitles;
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                 throw new Exception( $"Failed to get books: {ex.Message}");
            }

        }

        public string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public string AddBook(string fileName, string bookText)
        {
            if (string.IsNullOrEmpty(bookText))
                throw new ArgumentException("Book content cannot be empty.");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be empty.");

            string filePath = Path.Combine(_booksDirectory, $"{fileName}.txt");

            try
            {
                File.WriteAllText(filePath, bookText);
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add book: {ex.Message}");
            }
        }

        public void RemoveBook(string id)
        {
            string fileName = Path.Combine(_booksDirectory, $"{id}.txt");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("Book not found.");

            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove book: {ex.Message}");
            }
        }

        private IEnumerable<string> GetWords(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return Regex.Matches(text, @"\b\w+\b").Cast<Match>().Select(m => m.Value);
        }

        public IEnumerable GetTopWords(string bookName)
        {
            if (_topWordsCache.ContainsKey(bookName))
            {
                return _topWordsCache[bookName];
            }

            var words = GetTopWordsWithoutCache(bookName);
            _topWordsCache[bookName] = words;
            return words;
        }

        public IEnumerable GetTopWordsWithoutCache(string bookName)
        {

            string filePath = Path.Combine(_booksDirectory, $"{bookName}.txt");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Book not found.");

            var words = GetWords(filePath)
                        .Where(word => word.Length >= 5)
                        .GroupBy(word => word.ToUpper())
                        .OrderByDescending(group => group.Count())
                        .Take(10)
                        .Select(group => new { Word = ToTitleCase(group.Key), Count = group.Count() })
                        .ToList();
            return words;
        }

        public IEnumerable<string> WordSearchByPrefix(string id, string prefix)
        {
            if (prefix.Length < 3)
                throw new Exception("Prefix is less than 3 characters");

            string filePath = Path.Combine(_booksDirectory, $"{id}.txt");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Book not found.");

            var words = GetWords(filePath)
                .Where(word => word.Length >= 3 && word.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(word => Regex.Replace(word, @"[^a-zA-Z]", ""))
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word => ToTitleCase(word))
                .Distinct()
                .ToList();

            return words;
        }
    }
}