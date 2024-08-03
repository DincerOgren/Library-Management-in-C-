using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Library_Management
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int key = 1;
            
            Library library = new Library(Library.defaultPathToRegisteredBooks);
            
            while (true)
            {
                Console.WriteLine("Current book amount in library = " + library.currentBookCount);
                Console.WriteLine("Current Day: " + library.currentTime.ToShortDateString());
                Console.WriteLine();

                WriteMenu();
                Console.WriteLine();
                Console.Write("Please enter the key of the action you want to perform.: ");
                if (int.TryParse(Console.ReadLine(), out int parsedKey))
                {
                    key = parsedKey;
                }
                else
                {
                    Console.WriteLine("Please enter a numerical value. Press any key to continue");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                switch (key)
                {
                    case 0:
                        Console.WriteLine("Exiting the program..");
                        break;
                    case 1:
                        library.BorrowBook();
                        break;
                    case 2:
                        library.ReturnBook();
                        break;
                    case 3:
                        library.AddBook();
                        break;
                    case 4:
                        Console.WriteLine();
                        Console.WriteLine("1 for search book with book name,");
                        Console.WriteLine("2 for search book with author name.");
                        Console.Write("Which search metod u wanna use: ");
                        if (int.TryParse(Console.ReadLine(), out int searchKey))
                        {
                            if (searchKey == 1)
                            {
                                Console.WriteLine();
                                Console.Write("Please write the book name u wanna find: ");
                                string searchWord = Console.ReadLine();
                                var tempList = library.SearchBooks(searchWord);
                                if (tempList.Count == 0)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("There is no book in library starts with or called: {0}", searchWord);
                                    Console.WriteLine("Press any key to continue...");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    foreach (var book in tempList)
                                    {
                                        Console.WriteLine("-----------");
                                        Console.WriteLine("Book ID: {0}, Book Name: {1} Author: {2}, ISBN: {3}, Copy Count: {4}", book.id, book.NAME, book.AUTHOR, book.ISBNNUMBER, book.COPYCOUNT);
                                        Console.WriteLine("-----------");
                                    }
                                    Console.WriteLine();
                                    Console.WriteLine("If you want to borrow any of the book above press 1");
                                    Console.WriteLine("If you want to go to main menu Press any other key to continue...");
                                    if (int.TryParse(Console.ReadLine(), out int searchKey2))
                                    {
                                        if (searchKey2 == 1)
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Enter the ID number of the book you want to borrow: ");
                                            if (int.TryParse(Console.ReadLine(), out int borrowBookID))
                                            {
                                                var matchingBook = tempList.Find(x => x.id == borrowBookID);

                                                if (matchingBook != null)
                                                {
                                                    library.BorrowBook(matchingBook.id);

                                                    //matchingBook.copyIDs.Remove(matchingBook.id);
                                                    //matchingBook.COPYCOUNT--;
                                                    //library.UpdateCopyCounts(matchingBook);
                                                    //library.RemoveBookManually(library.GetBookList(),matchingBook);
                                                    //library.AddBookManually(library.GetBorrowedBookList(), matchingBook);
                                                    tempList.Remove(matchingBook);
                                                    Console.WriteLine($"You borrowed {matchingBook.NAME} book");
                                                    Console.WriteLine("Press any key to continue...");
                                                    Console.ReadKey();
                                                    Console.Clear();
                                                    continue;
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"There is no book with id {borrowBookID} in above");
                                                    Console.ReadKey();
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Please enter a numeric value for borrow a book. Press any key to continue...");
                                                Console.ReadKey();
                                            }
                                            Console.WriteLine("");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Please enter a numeric value for borrow a book. Press any key to continue...");
                                                Console.ReadKey();
                                            tempList.Clear();
                                        }
                                    }
                                    else
                                    {
                                        tempList.Clear();
                                    }
                                }

                            }
                            else if (searchKey == 2)
                            {
                                Console.WriteLine();
                                Console.Write("Please write the author u wanna find: ");
                                string searchWord = Console.ReadLine();
                                var tempList = library.SearchBooks(searchWord, true);
                                if (tempList.Count == 0)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("There is no book in library with Author name starts with or called: {0}", searchWord);
                                    Console.WriteLine("Press any key to continue...");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    foreach (var book in tempList)
                                    {
                                        Console.WriteLine("-----------");
                                        Console.WriteLine("Book ID: {0}, Book Name: {1} Author: {2}, ISBN: {3}, Copy Count: {4}", book.id, book.NAME, book.AUTHOR, book.ISBNNUMBER, book.COPYCOUNT);
                                        Console.WriteLine("-----------");
                                    }
                                    Console.WriteLine();
                                    Console.WriteLine("If you want to borrow any of the book above press 1");
                                    Console.WriteLine("If you want to go to main menu Press any other key to continue...");
                                    if (int.TryParse(Console.ReadLine(), out int searchKey2))
                                    {
                                        if (searchKey2 == 1)
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Enter the ID number of the book you want to borrow: ");
                                            if (int.TryParse(Console.ReadLine(), out int borrowBookID))
                                            {
                                                var matchingBook = tempList.Find(x => x.id == borrowBookID);

                                                if (matchingBook != null)
                                                {
                                                    library.BorrowBook(matchingBook.id);
                                                    //matchingBook.copyIDs.Remove(matchingBook.id);
                                                    //matchingBook.COPYCOUNT--;
                                                    //library.UpdateCopyCounts(matchingBook);
                                                    
                                                    //library.RemoveBookManually(library.GetBookList(), matchingBook);
                                                    //library.AddBookManually(library.GetBorrowedBookList(), matchingBook);
                                                    tempList.Remove(matchingBook);
                                                    Console.WriteLine($"You borrowed {matchingBook.NAME} book");
                                                    Console.WriteLine("Press any key to continue...");
                                                    Console.ReadKey();
                                                    Console.Clear();
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"There is no book with id {borrowBookID} in above");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Please enter a numeric value to borrow a book. Press any key to continue...");
                                                Console.ReadKey();
                                            }
                                            Console.WriteLine("");
                                        }
                                        else
                                        {
                                            tempList.Clear();
                                        }
                                    }
                                    else
                                    {
                                        tempList.Clear();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("You entered a wrong number for search method. Please check your number and try again. Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please enter '1' or '2' for search methods. Press any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case 5:
                        //library.ViewALLLIST();
                        library.ViewCurrentBooksList();
                        Console.ReadKey();
                        break;
                    case 6:
                        library.ViewExpiredBooks();
                        Console.ReadKey();
                        break;
                    case 7:
                        library.ViewBorrowedBooksList();
                        Console.ReadKey();
                        break;
                    case 8:
                        Console.WriteLine();
                        Console.WriteLine("Please write a number to add days to the current time: ");
                        if (int.TryParse(Console.ReadLine(), out int parsedDay))
                        {
                            library.currentTime = library.currentTime.AddDays(parsedDay);
                            Console.WriteLine("Added {0} days to current time", parsedDay);
                            library.UpdateBookDates();
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Please write a numeric value. Press any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    default:
                        break;
                }
                if (key == 0)
                {
                    break;
                }

                if (key > 8 || key < 0)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Please enter a number between 0 and 8 and try again. Press any key to continue");
                    Console.ReadKey();

                }
                Console.Clear();
            }


            Console.WriteLine();
            Console.WriteLine("***");
            Console.WriteLine("Program shut down.");
            Console.ReadKey();
        }
        private static void WriteMenu()
        {
            Console.WriteLine("1 => to borrow a book");
            Console.WriteLine("2 => to return a book");
            Console.WriteLine("3 => tp add a new book");
            Console.WriteLine("4 => to search a book");
            Console.WriteLine("5 => to display all books");
            Console.WriteLine("6 => to display expired books");
            Console.WriteLine("7 => to display borrowed books");
            Console.WriteLine("8 => to add days to current date");
            Console.WriteLine();
            Console.WriteLine("0 => to exit the application.");
            Console.WriteLine();
        }
    }





    
}
