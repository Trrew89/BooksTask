import { getBooks, getWords, getWordsByPrefix, deleteBook, addBook } from './http.js';

let bookName;
const prefixInput = document.getElementById('prefix');
const bookNameInput = document.getElementById('book_name');
const bookTextInput = document.getElementById('book_text');
const ol = document.querySelector('.top_10_words');
const olPrefix = document.querySelector('.words_prefix');
const blackout = document.querySelector('.blackout');
const addBookModal = document.querySelector('.add_book_modal');
const bookModal = document.querySelector('.book_modal');


function showWordsByPrefix(words) {
    olPrefix.innerHTML = ''
    words.forEach((word, i) => {
        const li = document.createElement("li")
        li.textContent = (i + 1) + ". " + word
        olPrefix.appendChild(li);
    })
}

function showWords(words) {
    words.forEach(({ word, count }) => {
        const li = document.createElement('li');
        li.textContent = '"' + word + '" ' + count + " times.";
        ol.appendChild(li);
    })
}

function createBookList(books) {
    const bookList = document.querySelector('.book_list');
    bookList.innerHTML = `<ul>${books.map(book => createBookListItem(book)).join('')}</ul>`;
    attachEventListeners();
}

function createBookListItem(book) {
    return `<li class="book">${book}<button class="deleteBtn">Delete</button></li>`;
}

async function updateList() {
    const books = await getBooks();
    createBookList(books);
}

function attachEventListeners() {
    const bookList = document.querySelector('.book_list');
    bookList.removeEventListener('click', handleBookListClick);
    bookList.addEventListener('click', handleBookListClick);
}

async function handleBookListClick(event) {
    const target = event.target;
    if (target.classList.contains('deleteBtn')) {
        bookName = target.parentElement.textContent.trim().replace("Delete", "");
        await deleteBook(bookName);
        updateList();
    } else if (target.classList.contains('book')) {
        bookName = target.textContent.trim().replace("Delete", "");
        showModal(bookModal, bookName);
    }
}

function showModal(modal, bookName) {
    modal.style.display = "flex"
    setTimeout(function () {
        modal.style.opacity = 1;
    }, 10);
    blackout.style.display = 'block'
    if (modal.classList[0] == "book_modal")
        getWords(bookName, showWords)
}

function closeModalListener(modal) {
    document.addEventListener('click', function (event) {
        if (blackout == event.target) {
            closeModal(modal)
        }
    });
}

function closeModal(modal) {
    modal.style.display = "none"
    modal.style.opacity = 0;
    blackout.style.display = "none"
    if (modal.classList[0] === "book_modal") {
        ol.innerHTML = '<h3>Top 10 used words:</h3>';
        prefixInput.value = "";
        olPrefix.innerHTML = ""
    } else {
        bookNameInput.value = '';
        bookTextInput.value = '';
    }
}

async function fetchDataAndProcess() {
    const books = await getBooks();
    createBookList(books);
    attachEventListeners();
    closeModalListener(addBookModal)
    closeModalListener(bookModal)
}

fetchDataAndProcess()

const modalButton = document.querySelector(".add_book");
modalButton.addEventListener("click", () => {
    showModal(addBookModal)
});

const addButton = document.querySelector('.add_btn');
addButton.addEventListener("click", () => {
    const nameInput = document.querySelector('#book_name');
    const textInput = document.querySelector('#book_text');
    if (nameInput.value && textInput.value) {
        addBook(nameInput.value, textInput.value);
        updateList();
        closeModal(addBookModal);
    } else {
        alert("Both fields needs to be filled")
    }
})

document.addEventListener('DOMContentLoaded', function () {
    let timeoutId;
    prefixInput.addEventListener('input', function () {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(function () {
            const prefix = prefixInput.value;
            if (prefix.length >= 3) {
                getWordsByPrefix(bookName, prefix, showWordsByPrefix);
            } else if (prefix.length === 0) {
                olPrefix.innerHTML = "";
            } else {
                alert('Please enter at least 3 characters for the prefix.');
            }
        }, 500);
    });
});
