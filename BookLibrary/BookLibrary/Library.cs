using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookLibrary
{
	public class Library
	{
		// Attribute used for storing data location string (for reading and saving)
		private string Path {get;set;}
		// List which contains library books
		private List<LibraryBook> BookList { get; set; }

		// Constructor for Library class object
		public Library(string path)
		{
			// Assigning attribute
			Path = path;
			// Assigning library book list from data file
			BookList = LibraryBookList();
		}

		// Method that add book to library
		// On success: book added and sucess message
		// On failure: conflict message
		private void AddBook(Book newBook)
		{
			// If Book class object exists
			if(newBook != null)
			{
				// Adds a book to BookList
				BookList.Add(new LibraryBook(newBook));
				// Synchronizing data file with BookList
				SaveLibraryBookList();
				// Conflict message
				Console.WriteLine("Book added!");
			}
			else
			{
				// Text error
				Console.WriteLine("Book file is corrupted or missing data");
			}
		}

		// Method that gives book to person
		// On success : book assigned to person and success message
		// On failure : no book assigned and reason message
		private void TakeBook(string wantedBookName, string wantedBookAuthor, string whoTakes, int periodDays)
		{
			// Checks if book borrowing period does not exceed two mounths
			if (periodDays > 60)
			{
				// Reason message
				Console.WriteLine("You cannot take book for a longer period than two months!");
				return;
			}
			// Checks if person is allowed to take more books
			if(!CheckIfPersonCanTakeMoreBooks(whoTakes))
			{
				// Reason message
				Console.WriteLine("You cannot take more books!");
				return;
			}
			// Checks if library has this book and borrows it for this person
			if (CheckIfBookIsAvailableAndBookIt(wantedBookName, wantedBookAuthor, whoTakes, periodDays))
			{
				// Success message
				Console.WriteLine("Here is your book.");
				return;
			}

			// Reason message
			Console.WriteLine("We currently do not have this book in our library..");
		}

		// Method that gives book back to library
		// On success: book becomes available and sucess message
		// On failure: conflict message
		private void ReturnBook(string bookName, string bookAuthor)
		{
			// Finding desired book whcih is taken
			LibraryBook wantedBook = BookList.Find(book => book.CheckBookNameAndAuthor(bookName, bookAuthor)
			&& !book.BookIsReadyToTake());
			// Checking if taht book is found
			if (wantedBook != null)
			{
				// Removing desired book from BookList
				BookList.Remove(wantedBook);
				// Making desired book available
				wantedBook.RemoveBookRent();
				// Adding book to BookList
				BookList.Add(wantedBook);
				// Synchronizing data file
				SaveLibraryBookList();
				// Success message
				Console.WriteLine("Book returned.");
				return;
			}
			// Conflict message
			Console.WriteLine("We do not have this book or book is not taken.");
		}

		// Method that shows books
		// On success: shows filtered or not filtered book list
		// On failure: conflict message
		private void ShowLibraryBookList(string[] categories, string[] filters)
		{
			// Declaring list which we will filter
			List<LibraryBook> filteredBooks = BookList;
			// Checking if list command got any parameters
			if (categories.Count() > 0 && filters.Count() > 0)
			{
				// Checking if count of categories and filters are not the same
				if (categories.Count() != filters.Count())
				{
					// Conflict message
					Console.WriteLine("Filter and text elements count must match.");
					return;
				}
				// Iterating through parameters
				for (int i = 0; i < categories.Count(); i++)
				{
					// Filtering
					if (categories[i] == "author")
					{
						filteredBooks = filteredBooks.FindAll(book => book.Author == filters[i]);
					}
					else if (categories[i] == "category")
					{
						filteredBooks = filteredBooks.FindAll(book => book.Category == filters[i]);
					}
					else if (categories[i] == "language")
					{
						filteredBooks = filteredBooks.FindAll(book => book.Language == filters[i]);
					}
					else if (categories[i] == "ISBN")
					{
						filteredBooks = filteredBooks.FindAll(book => book.ISBN == filters[i]);
					}
					else if (categories[i] == "name")
					{
						filteredBooks = filteredBooks.FindAll(book => book.Name == filters[i]);
					}
					// required command: list taken books
					else if (categories[i] == "taken")
					{
						if (filters[i] != "books")
						{
							// Conflict message
							Console.WriteLine("Bad command, example: list taken books");
							return;
						}
						filteredBooks = filteredBooks.FindAll(book => !book.BookIsReadyToTake());
					}
					// required command: list available books
					else if (categories[i] == "available")
					{
						if (filters[i] != "books")
						{
							// Conflict message
							Console.WriteLine("Bad command, example: list available books");
							return;
						}
						filteredBooks = filteredBooks.FindAll(book => book.BookIsReadyToTake());
					}
					else
					{
						// Conflict message
						Console.WriteLine($"No such category as {categories[i]}");
						return;
					}
				}
			}
			// Checking if any books were found
			if(filteredBooks.Count() == 0)
			{
				// Conflict message
				Console.WriteLine("No books found.");
				return;
			}
			// Filtered list
			Console.WriteLine(JsonConvert.SerializeObject(filteredBooks, Formatting.Indented));
		}

		// Method that deletes books
		// On success: book on library is deleted and success message
		// On failure: conflict message
		private void DeleteBook(string wantedBookName, string wantedBookAuthor)
		{
			// Searching for desired book
			LibraryBook wantedBook = BookList.Find(book => book.CheckBookNameAndAuthor(wantedBookName, wantedBookAuthor));
			// Checking if desired book was found
			if (wantedBook != null)
			{
				// Removing desired book from BookList
				BookList.Remove(wantedBook);
				// Synchronizing data file
				SaveLibraryBookList();
				// Success message
				Console.WriteLine("Book deleted.");
				return;
			}
			// Conflict message
			Console.WriteLine("No such book found."); ;
		}

		// Method that checks if person can take more books (does not exceed 3 book limit)
		private bool CheckIfPersonCanTakeMoreBooks(string whoTakes)
		{
			// Borrowed books by this person
			List<LibraryBook> personsBooks = BookList.FindAll(book => book.CheckIfThisPersonHasThisBook(whoTakes));
			// Checking if borrowed book count does not exceed 3 books limit
			if (personsBooks.Count() >= 3)
				return false;
			return true;
		}

		// Method that checks if book is available and if so - borrows it
		private bool CheckIfBookIsAvailableAndBookIt(string wantedBookName, string wantedBookAuthor, string whoTakes, int periodDays)
		{
			// Finding desired book
			LibraryBook wantedBook = BookList.Find(book => book.CheckBookNameAndAuthor(wantedBookName, wantedBookAuthor) 
			&& book.BookIsReadyToTake());
			// If book is found
			if (wantedBook != null)
			{
				// Removing desired book from BookList
				BookList.Remove(wantedBook);
				// Making desired book borrowed
				wantedBook.MakeBookRent(whoTakes, periodDays);
				// Adding desired book to BookList
				BookList.Add(wantedBook);
				// Synchronizing data file with BookList
				SaveLibraryBookList();
				return true;
			}
			return false;
		}

		// Function that returns book list from data file
		private List<LibraryBook> LibraryBookList()
		{
			return JsonConvert.DeserializeObject<List<LibraryBook>>(File.ReadAllText(Path));
		}

		// Method that writes BookList to data file
		private void SaveLibraryBookList()
		{
			File.WriteAllText(Path, JsonConvert.SerializeObject(BookList, Formatting.Indented));
		}

		// Main method to reach library
		// On success: command completed and success message
		// On failure: Conflict message
		public void CommunicateToLibrary(string input)
		{
			// Splitting command to determine type
			string[] command = input.Split(new char[] { ' ' }, 2);
			if (command[0] == "add")
			{
				HandleAdd(command);
			}
			else if (command[0] == "take")
			{
				HandleTake(command);
			}
			else if (command[0] == "return")
			{
				HandleReturn(command);
			}
			else if (command[0] == "list")
			{
				HandleList(command);
			}
			else if (command[0] == "remove")
			{
				HandleRemove(command);
			}
			else
			{
				// Conflict message
				Console.WriteLine($"No such command as [{command[0]}]");
			}
		}

		// Method that handles add command
		private void HandleAdd(string[] command)
		{
			// Checking if command, argument is correct
			if (command.Count() == 2 && File.Exists(command[1]))
			{
				// Trying to add the book
				AddBook(new LibraryBook().ReadBook(command[1]));
			}
			else
			{
				// Conflict message
				Console.WriteLine("Expected one argument which is existing file.");
			}
		}

		// Method that handles take command
		private void HandleTake(string[] command)
		{
			// Checking if command did not have any attributes
			if (command.Count() == 1)
			{
				// Getting reader
				Console.WriteLine("Who is taking the book?");
				string whoIsTaking = Console.ReadLine();
				// Getting period in days for how long book is taken
				Console.WriteLine("For what period (in days)?");
				string periodDaysInput = Console.ReadLine();
				// Declaring attribute which we will use for parsing
				int periodDays = 0;
				// Trying to parse integer
				bool isParsable = Int32.TryParse(periodDaysInput, out periodDays);
				// Parsing until succeess
				while (!isParsable)
				{
					// Asking for integer
					Console.WriteLine("Only integers acceptable, try again..");
					periodDaysInput = Console.ReadLine();
					// Trying to parse integer
					isParsable = Int32.TryParse(periodDaysInput, out periodDays);
				}
				// Asking for desired book name and author
				Console.WriteLine("Book Name/Author");
				string[] bookNameAndAuthor = Console.ReadLine().Split('/');
				// Asking till name and author is written
				while (bookNameAndAuthor.Count() != 2)
				{
					// Asking again for name and author
					Console.WriteLine("You need book name/author");
					bookNameAndAuthor = Console.ReadLine().Split('/');
				}
				// Trying to take the book from library
				TakeBook(bookNameAndAuthor[0], bookNameAndAuthor[1], whoIsTaking, periodDays);
			}
			else
			{
				// Conflict message
				Console.WriteLine("take command takes no arguments");
			}
		}

		// Method that handles return command
		private void HandleReturn(string[] command)
		{
			// Checking if command and argument received
			if (command.Count() == 2)
			{
				// Getting book name and author
				string[] bookNameAndAuthor = command[1].Split('/');
				// Repeating till book name and author is received
				while (bookNameAndAuthor.Count() != 2)
				{
					// Requesting book name and author
					Console.WriteLine("You need book name/author");
					bookNameAndAuthor = Console.ReadLine().Split('/');
				}
				// Trying to return book
				ReturnBook(bookNameAndAuthor[0], bookNameAndAuthor[1]);
			}
			else
			{
				// Conflict message
				Console.WriteLine("Expected one argument which is book name/author.");
			}
		}

		// Method that handles list command
		private void HandleList(string[] command)
		{
			// Declaring attributes
			string[] filter = { };
			string[] fill = { };
			// Checking if command had attributes
			if (command.Count() == 2)
			{
				// Getting filter by and filter text
				string[] filterAndFill = command[1].Split(' ');
				// Checking if filter attributes were correct
				if (filterAndFill.Count() == 2)
				{
					// Getting multiple filters and filter texts
					filter = filterAndFill[0].Split('/');
					fill = filterAndFill[1].Split('/');
					// Trying to filter book list
					ShowLibraryBookList(filter, fill);
				}
				else
				{
					// Conflict message
					Console.WriteLine("Expected filter and filter text");
				}
			}
			// Checking if command had no attributes
			else if (command.Count() == 1)
			{
				// Showing not filtered book list
				ShowLibraryBookList(filter, fill);
			}
			else
			{
				// Conflict message
				Console.WriteLine("Expected no arguments or only two argument which is filter and filter text");
			}
		}

		// Method that handles remove command
		private void HandleRemove(string[] command)
		{
			// Checking if command has arguments
			if (command.Count() == 2)
			{
				// Getting book name and author
				string[] bookNameAndAuthor = command[1].Split('/');
				// Checking if name and author received
				if (bookNameAndAuthor.Count() == 2)
				{
					// Trying to delte desired book
					DeleteBook(bookNameAndAuthor[0], bookNameAndAuthor[1]);
				}
				else
				{
					// Conflict message
					Console.WriteLine("Expected name/author");
				}
			}
			else
			{
				// Conflict message
				Console.WriteLine("Expected one argument which should be name/author");
			}
		}

		// Used for testing
		public List<LibraryBook> GetBookList()
		{
			return BookList;
		}

		// Used for testing
		public void SetList(List<LibraryBook> newList)
		{
			BookList = newList;
			SaveLibraryBookList();
		}

		// Used for testing
		public int GetBookListCount()
		{
			return BookList.Count();
		}
	}
}
