using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management
{
    public class Library : ILibrary
    {
        public int currentBookCount = 0;
        public DateTime currentTime;
        public int totalBookCount = 0;
        int expireDays = 30;
        List<Book> books;
        List<Book> borrowedBooks;
        List<Book> expiredBooks;
        public static readonly string defaultPathToRegisteredBooks = "..\\..\\source/RegisteredBooks.txt";
        public static readonly string pathToBorrowedBooks = "..\\..\\source/BorrowedBooks.txt";
        public static readonly string pathToExpiredBooks = "..\\..\\source/ExpiredBooks.txt";
        readonly bool shouldUpdateTxtFile = false;

        public Library()
        {
            books = new List<Book>();
            borrowedBooks = new List<Book>();
            expiredBooks = new List<Book>();
            currentTime = DateTime.Now;
        }

        public Library(string pathToTxt)
        {
            if (File.Exists(pathToTxt))
            {
                books = new List<Book>();
                borrowedBooks = new List<Book>();
                expiredBooks = new List<Book>();
                currentTime = DateTime.Now;
                try
                {


                    StreamReader sr = new StreamReader(pathToTxt);
                    while (!sr.EndOfStream)
                    {
                        Book book = new Book();
                        string line = sr.ReadLine();
                        string[] parts = line.Split('/');

                        if (parts.Length >= 3)
                        {
                            book.NAME = parts[0];
                            book.AUTHOR = parts[1];
                            if (int.TryParse(parts[2], out int copyCount))
                            {
                                book.COPYCOUNT = copyCount;
                            }
                            else
                            {
                                Console.WriteLine($"Error: The copy count of the book {book.NAME} in the Registered txt, {parts[2]}, is not a numerical value. Please correct that and try again.");
                                Console.ReadKey();
                                break;
                            }
                        }
                        if (books.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
                        {
                            Book exsistedBook = FindBook(book, books);

                            if (book.COPYCOUNT > 1)
                            {
                                for (int i = 0; i < book.COPYCOUNT; i++)
                                {
                                    Book copyBook = new Book(book);

                                    totalBookCount++;
                                    currentBookCount++;
                                    shouldUpdateTxtFile = true;
                                    exsistedBook.COPYCOUNT++;
                                    copyBook.ISBNNUMBER = exsistedBook.ISBNNUMBER;
                                    copyBook.id = totalBookCount;
                                    exsistedBook.copyIDs.Add(copyBook.id);
                                    copyBook.copyIDs = exsistedBook.copyIDs;
                                    books.Add(copyBook);

                                }

                            }
                            else
                            {

                                totalBookCount++;
                                currentBookCount++;
                                shouldUpdateTxtFile = true;
                                exsistedBook.COPYCOUNT += 1;
                                book.ISBNNUMBER = exsistedBook.ISBNNUMBER;
                                book.id = totalBookCount;
                                exsistedBook.copyIDs.Add(book.id);
                                book.copyIDs = exsistedBook.copyIDs;
                                books.Add(book);
                                UpdateCopyCounts(exsistedBook, books);
                            }
                        }
                        else
                        {
                            if (book.COPYCOUNT > 1)
                            {
                                for (int i = 0; i < book.COPYCOUNT; i++)
                                {
                                    Book copyBook = new Book(book);
                                    if (books.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
                                    {
                                        totalBookCount++;
                                        currentBookCount++;
                                        Book exsistedBook = FindBook(book, books);
                                        shouldUpdateTxtFile = true;
                                        copyBook.id = totalBookCount;
                                        exsistedBook.copyIDs.Add(copyBook.id);
                                        copyBook.copyIDs = exsistedBook.copyIDs;
                                        books.Add(copyBook);

                                    }
                                    else
                                    {

                                        totalBookCount++;
                                        currentBookCount++;
                                        copyBook.id = totalBookCount;
                                        copyBook.copyIDs.Add(copyBook.id);
                                        books.Add(copyBook);
                                    }

                                }
                            }
                            else
                            {
                                totalBookCount++;
                                currentBookCount++;
                                book.id = totalBookCount;
                                book.copyIDs.Add(book.id);
                                books.Add(book);
                            }


                        }
                    }

                    sr.Close();

                    if (shouldUpdateTxtFile)
                    {
                        shouldUpdateTxtFile = false;
                        WriteListToTxtFile(pathToTxt, books);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There is an error finding library's 'txt' file. Error = " + e);
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Path couldnt find for library's 'txt' file please update the path");
                Console.ReadKey();
            }

            if (File.Exists(pathToBorrowedBooks))
            {
                StreamReader sr = new StreamReader(pathToBorrowedBooks);
                while (!sr.EndOfStream)
                {
                    Book book = new Book();
                    string line = sr.ReadLine();
                    string[] parts = line.Split('/');

                    if (parts.Length == 4)
                    {
                        book.NAME = parts[0];
                        book.AUTHOR = parts[1];
                        book.BORROWEDCOPIES = Convert.ToInt32(parts[2]);
                        book.ExpireDate = Convert.ToDateTime(parts[3]);
                    }

                    if (books.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
                    {
                        Book copyBook = FindBook(book, books);

                        copyBook.BORROWEDCOPIES++;
                        UpdateCopyCounts(copyBook, books);
                    }

                    totalBookCount++;
                    book.id = totalBookCount;
                    book.copyIDs.Add(book.id);
                    borrowedBooks.Add(book);
                    UpdateCopyCounts(book, borrowedBooks);

                }
                sr.Close();
            }


        }



        public void AddBook()
        {
            
            Book book = new Book();
            Console.WriteLine();
            Console.Write("Enter the name of the book you want to add: ");
            book.NAME = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Enter the author of the book you want to add: ");
            book.AUTHOR = Console.ReadLine();
            if (books.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
            {
                totalBookCount++;
                currentBookCount++;
                Book exsistedBook = FindBook(book, books);
                exsistedBook.COPYCOUNT++;
                Console.WriteLine("Book added to library.");
                book.COPYCOUNT = exsistedBook.COPYCOUNT;
                book.id = totalBookCount;
                exsistedBook.copyIDs.Add(book.id);
                book.ISBNNUMBER = exsistedBook.ISBNNUMBER;

                book.copyIDs = exsistedBook.copyIDs;
                books.Add(book);
                UpdateCopyCounts(exsistedBook, books);
                books = books.OrderBy(x => x.id).ToList();

            }
            else
            {
                currentBookCount++;
                totalBookCount++;
                book.id = totalBookCount;
                books.Add(book);
            }
            Console.WriteLine("Book information you added: Name:{0}, Author: {1}", book.NAME, book.AUTHOR);
            UpdateTXTFile(defaultPathToRegisteredBooks, books);

            Console.ReadKey();

        }
        public void BorrowBook()
        {
            ViewBookList(books);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please write the ID of the book you want to borrow.: ");
            int id;

            if (int.TryParse(Console.ReadLine(), out int parsedKey))
            {
                id = parsedKey;

                Book borrowedBook = FindBookWithID(id, books);

                if (borrowedBook == null)
                {
                    Console.ReadKey();
                    return;
                }


                borrowedBook.copyIDs.Remove(borrowedBook.id);

                if (borrowedBooks.Exists(x => x.NAME == borrowedBook.NAME && x.AUTHOR == borrowedBook.AUTHOR))
                {
                    var exsistedBook = FindBook(borrowedBook, borrowedBooks);
                    exsistedBook.BORROWEDCOPIES++;
                    exsistedBook.COPYCOUNT--;
                    borrowedBook.BORROWEDCOPIES = exsistedBook.BORROWEDCOPIES;
                    UpdateCopyCounts(exsistedBook, books);

                    borrowedBook.ExpireDate = currentTime.AddDays(expireDays);
                    currentBookCount--;
                    borrowedBooks.Add(borrowedBook);

                }
                else
                {
                    borrowedBook.BORROWEDCOPIES++;
                    borrowedBook.COPYCOUNT--;
                    currentBookCount--;
                    borrowedBook.ExpireDate = currentTime.AddDays(expireDays);
                    borrowedBooks.Add(borrowedBook);
                    UpdateCopyCounts(borrowedBook, books);


                }

                books.Remove(borrowedBook);

                UpdateTXTFile(defaultPathToRegisteredBooks, books);
                UpdateTXTFile(pathToBorrowedBooks, borrowedBooks, true);
                Console.WriteLine();
                Console.WriteLine($"You borrowed the {borrowedBook.NAME} by {borrowedBook.AUTHOR}. Press any key to continue...");
                Console.ReadKey();

            }
            else
            {
                Console.WriteLine("Please enter a numeric value for book id. Press any key to continue");
                Console.ReadKey();
            }


        }
        public void BorrowBook(int bookID)
        {
            var book = FindBookWithID(bookID, books);
            book.copyIDs.Remove(book.id);

            if (borrowedBooks.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
            {
                var exsistedBook = FindBook(book, borrowedBooks);
                exsistedBook.BORROWEDCOPIES++;
                exsistedBook.COPYCOUNT--;
                book.BORROWEDCOPIES = exsistedBook.BORROWEDCOPIES;
                UpdateCopyCounts(exsistedBook, books);

                book.ExpireDate = currentTime.AddDays(expireDays);
                currentBookCount--;
                borrowedBooks.Add(book);

            }
            else
            {
                book.BORROWEDCOPIES++;
                book.COPYCOUNT--;
                currentBookCount--;
                book.ExpireDate = currentTime.AddDays(expireDays);
                UpdateCopyCounts(book, books);

                borrowedBooks.Add(book);
            }
            UpdateCopyCounts(book, borrowedBooks);
            books.Remove(book);
            UpdateTXTFile(defaultPathToRegisteredBooks, books);
            UpdateTXTFile(pathToBorrowedBooks, borrowedBooks, true);
        }
        public void ReturnBook()
        {

            if (borrowedBooks.Count < 1)
            {
                Console.WriteLine("You don't have any borrowed books to return to the library..");
                Console.ReadKey();
                return;
            }
            ViewBookList(borrowedBooks, true);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please enter book id that you want to return to library.");
            if (int.TryParse(Console.ReadLine(), out int parsedKey))
            {
                Book returnBook = FindBookWithID(parsedKey, borrowedBooks);
                if (returnBook != null)
                {
                    if (books.Exists(x => x.NAME == returnBook.NAME && x.AUTHOR == returnBook.AUTHOR))
                    {
                        currentBookCount++;
                        Book exsistedBook = FindBook(returnBook, books);
                        exsistedBook.COPYCOUNT++;
                        exsistedBook.BORROWEDCOPIES--;
                        returnBook.COPYCOUNT = exsistedBook.COPYCOUNT;
                        returnBook.BORROWEDCOPIES = exsistedBook.BORROWEDCOPIES;
                        exsistedBook.copyIDs.Add(returnBook.id);
                        exsistedBook.copyIDs = exsistedBook.copyIDs.OrderBy(num => num).ToList();
                        returnBook.ISBNNUMBER = exsistedBook.ISBNNUMBER;

                        returnBook.copyIDs = exsistedBook.copyIDs;
                        UpdateCopyCounts(returnBook, borrowedBooks);

                        borrowedBooks.Remove(returnBook);
                        if (expiredBooks.Exists(x => x.NAME == returnBook.NAME && x.AUTHOR == returnBook.AUTHOR))
                        {
                            expiredBooks.Remove(returnBook);
                        }
                        books.Add(returnBook);
                        UpdateCopyCounts(exsistedBook, books);
                        Console.WriteLine("Book returned to the library.");

                    }
                    else
                    {
                        currentBookCount++;
                        returnBook.COPYCOUNT++;
                        returnBook.BORROWEDCOPIES--;
                        returnBook.copyIDs.Add(returnBook.id);
                        UpdateCopyCounts(returnBook, borrowedBooks);

                        borrowedBooks.Remove(returnBook);
                        if (expiredBooks.Exists(x => x.NAME == returnBook.NAME && x.AUTHOR == returnBook.AUTHOR))
                        {
                            expiredBooks.Remove(returnBook);
                        }
                        books.Add(returnBook);
                        Console.WriteLine("Book returned to the library.");
                        UpdateCopyCounts(returnBook, books);

                    }

                    UpdateTXTFile(pathToBorrowedBooks, borrowedBooks, true);
                    UpdateTXTFile(defaultPathToRegisteredBooks, books);

                    Console.WriteLine($"You have returned this book: {returnBook.NAME}. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"There is no book borrowed with id {parsedKey}. Press any key to continue..");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Please enter a numeric value for book id. Press any key to continue...");
                Console.ReadKey();
            }
        }

        public void UpdateCopyCounts(Book book, List<Book> list)
        {
            foreach (var item in list)
            {
                if (item.NAME == book.NAME && item.AUTHOR == book.AUTHOR)
                {
                    item.COPYCOUNT = book.COPYCOUNT;
                    item.BORROWEDCOPIES = book.BORROWEDCOPIES;
                    item.copyIDs = book.copyIDs;
                }
            }
        }
        public void WriteListToTxtFile(string path, List<Book> list, bool isBorrowedList = false)
        {
            if (File.Exists(path))
            {

                StreamWriter sr = new StreamWriter(path);
                for (int i = 0; i < list.Count; i++)
                {
                    if (isBorrowedList)
                    {
                        sr.WriteLine(list[i].NAME + '/' + list[i].AUTHOR + '/' + list[i].BORROWEDCOPIES + '/' + list[i].ExpireDate.ToShortDateString());
                    }
                    else
                    {
                        sr.WriteLine(list[i].NAME + '/' + list[i].AUTHOR + '/' + "1");
                    }
                }
                sr.Close();
            }
        }


        public void UpdateTXTFile(string path, List<Book> list, bool isBorrowedBooks = false)
        {
            WriteListToTxtFile(path, list, isBorrowedBooks);
        }
        public Book FindBook(Book book, List<Book> list)
        {

            var selected = list.Find(x => x.AUTHOR.Equals(book.AUTHOR) && x.NAME.Equals(book.NAME));
            if (selected != null)
            {
                return selected;
            }
            else
            {

                Console.WriteLine("Cant find book in library. ");
                return null;
            }
        }

        public Book FindBookWithID(int id, List<Book> list)
        {
            foreach (var item in list)
            {
                if (id == item.id)
                {
                    return item;
                }
            }

            Console.WriteLine($"Cant find book with id: {id}. ");
            return null;
        }

        public List<Book> SearchBooks(string keyword, bool searchWithAuthor = false)
        {
            if (searchWithAuthor)
            {
                return books.Where(book => book.AUTHOR.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            else
                return books.Where(book => book.NAME.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        private void ViewBookList(List<Book> list, bool shouldShowExpireDate = false)
        {
            if (list.Count <= 0 && !shouldShowExpireDate)
            {
                Console.WriteLine("There is no book in library. Press any key to continue...");
                Console.ReadKey();
                return;
            }
            if (list.Count <= 0 && shouldShowExpireDate)
            {
                Console.WriteLine("There is no borrowed books. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            List<Book> tempList = new List<Book>();
            foreach (var book in list)
            {
                if (shouldShowExpireDate)
                {

                    Console.WriteLine();
                    Console.WriteLine("-------------");

                    Console.Write("Book ID: {0}, Book Name: {1} Author: {2},Total Borrowed Copy Count: {3}, Expire Date: {4}, ",
                    book.id, book.NAME, book.AUTHOR, book.BORROWEDCOPIES, book.ExpireDate.ToShortDateString());
                    if (book.ExpireDate.Subtract(currentTime).Days < 0)
                    {
                        Console.WriteLine("DayLeftForExpire: EXPIRED");
                    }
                    else
                        Console.WriteLine("DayLeftForExpire: {0} ", book.ExpireDate.Subtract(currentTime).Days);
                    Console.WriteLine("-------------");

                }
                else
                {
                    if (book.copyIDs.Count > 1 && !tempList.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
                    {
                        Console.WriteLine("-------------");
                        Console.Write("Book ID's: ");
                        for (int i = 0; i < book.copyIDs.Count; i++)
                        {
                            Console.Write(book.copyIDs[i] + " ");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Book Name: {0} Author: {1}, ISBN: {2} ,Copy Count: {3}", book.NAME, book.AUTHOR, book.ISBNNUMBER, book.COPYCOUNT);
                        Console.WriteLine("-------------");
                        Console.WriteLine();
                        tempList.Add(book);
                    }
                    else if (!tempList.Exists(x => x.NAME == book.NAME && x.AUTHOR == book.AUTHOR))
                    {
                        Console.WriteLine("-------------");
                        Console.WriteLine("Book ID: {0}, Book Name: {1} Author: {2}, ISBN: {3}, Copy Count: {4}", book.id, book.NAME, book.AUTHOR, book.ISBNNUMBER, book.COPYCOUNT);
                        Console.WriteLine("-------------");
                    }

                }

            }

            tempList.Clear();
        }
        public void ViewCurrentBooksList()
        {
            ViewBookList(books);
        }
        public void ViewBorrowedBooksList()
        {
            ViewBookList(borrowedBooks, true);
        }
        public void ViewExpiredBooks()
        {
            if (expiredBooks.Count < 1)
            {
                Console.WriteLine("There is no expired books now. Press any key to continue...");
                Console.ReadKey();
                return;
            }
            foreach (var book in expiredBooks)
            {
                Console.WriteLine("----------");
                Console.WriteLine("Book ID: {0}, Book Name: {1} Author: {2}, ISBN: {3} ,Copy Count: {4}, Expire Date: {5}, Day Passed After Expire Date: {6}",
                   book.id, book.NAME, book.AUTHOR, book.ISBNNUMBER, book.COPYCOUNT, book.ExpireDate.ToShortDateString(), currentTime.Subtract(book.ExpireDate).Days);
                Console.WriteLine("----------");

            }
        }

        public void UpdateBookDates()
        {
            foreach (var item in borrowedBooks)
            {
                if (item.ExpireDate.Subtract(currentTime).Days < 0)
                {
                    if (!item.ISEXPIRED)
                    {
                        item.ISEXPIRED = true;

                        expiredBooks.Add(item);
                        UpdateTXTFile(pathToExpiredBooks, expiredBooks, true);
                    }
                }
            }
        }

    }

    public interface ILibrary
    {
        void AddBook();
        List<Book> SearchBooks(string keyword, bool searchWithAuthor = false);
        void BorrowBook();
        void ReturnBook();
        Book FindBook(Book book, List<Book> list);
        void ViewExpiredBooks();
    }
}
