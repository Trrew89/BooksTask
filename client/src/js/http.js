export async function getBooks() {
    try {
        const response = await fetch("http://localhost:5000/api/books");
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching books:', error);
    }
}

export function getWords(bookName, showWords) {
    fetch(`http://localhost:5000/api/books/${bookName}`)
        .then(res => res.json())
        .then(words => showWords(words))
        .catch(error => console.error('Error fetching book details:', error));;
}

export function getWordsByPrefix(bookName, prefix, showWordsByPrefix) {
    console.log(bookName);
    fetch(`http://localhost:5000/api/books/${bookName}/search/${prefix}`)
        .then(res => res.json())
        .then(words => showWordsByPrefix(words))
        .catch(error => console.error('Error fetching words by prefix:', error));
}

export function deleteBook(bookName) {
    return fetch(`http://localhost:5000/api/books/${bookName}`, { method: 'DELETE'})
            .catch(error => console.error('Error deleting the book:', error));
}

export function addBook(bookName, bookText) {
    console.log({body: {bookName, bookText}});
    fetch(`http://localhost:5000/api/books`, { method: 'POST', headers: {
        'Content-Type': 'application/json'
      }, body: JSON.stringify({bookName, bookText})})
        .then(alert("Book added successfully"))
        .catch(error => console.error('Error adding the book:', error));
}