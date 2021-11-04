using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BookLibrary.Tests
{
	[TestClass]
	public class LibraryTests
	{
		static string testLibraryPath = "..\\..\\..\\TestLibrary.json";
		static string testBookPath = "..\\..\\..\\TestBook.json";
		Library testLibrary = new Library(testLibraryPath);
		static Book book = new LibraryBook().ReadBook(testBookPath);
		LibraryBook testBook = new LibraryBook(book);

		[TestMethod]
		public void A_CommunicateToLibrary_AddBookThatExists_AddsToList()
		{
			string command = "add " + testBookPath;
			int initialBookListSize = testLibrary.GetBookListCount();

			testLibrary.CommunicateToLibrary(command);
			
			int finishedBookListSize = testLibrary.GetBookListCount();

			Assert.IsTrue(finishedBookListSize > initialBookListSize);
		}

		[TestMethod]
		public void B_CommunicateToLibrary_AddBookThatDoesNotExist_DoesNotAddToList()
		{
			string command = "add " + "I_dont_exist";
			int initialBookListSize = testLibrary.GetBookListCount();

			testLibrary.CommunicateToLibrary(command);

			int finishedBookListSize = testLibrary.GetBookListCount();

			Assert.IsTrue(finishedBookListSize == initialBookListSize);
		}

		[TestMethod]
		public void C_CommunicateToLibrary_TakeBookThatIsAvailable_ChangesBookAtributeWhoTaken()
		{
			List<LibraryBook> filteredBooks;
			List<LibraryBook> list = testLibrary.GetBookList();

			filteredBooks = list.FindAll(book => book.WhoTaken == null
			&& book.CheckBookNameAndAuthor(testBook.Name, testBook.Author));
			filteredBooks.ForEach(book => list.Remove(book.Return()));
			filteredBooks.ForEach(book => book.MakeBookRent("tester", 10));
			filteredBooks.ForEach(book => list.Add(book.Return()));
			testLibrary.SetList(list);

			list = testLibrary.GetBookList();
			filteredBooks = list.FindAll(book => book.WhoTaken == "tester"
			&& book.CheckBookNameAndAuthor(testBook.Name, testBook.Author));

			bool failed = true;
			foreach (var book in list)
				if (book.WhoTaken == "tester")
					failed = false;

			Assert.IsFalse(failed);
		}
		
		[TestMethod]
		public void D_CommunicateToLibrary_ReturnBookThatIsTaken_ChangesBookAtributeWhoTakenToNull()
		{
			string command = "return " + $"{testBook.Name}/{testBook.Author}";

			testLibrary.CommunicateToLibrary(command);

			List<LibraryBook> list = testLibrary.GetBookList();
			bool failed = true;
			foreach (var book in list)
				if (book.WhoTaken == null)
					failed = false;

			Assert.IsFalse(failed);
		}

		[TestMethod]
		public void E_CommunicateToLibrary_ReturnBookThatIsNotTaken_DoesNothing()
		{
			string command = "return " + $"{testBook.Name}/{testBook.Author}";

			testLibrary.CommunicateToLibrary(command);

			List<LibraryBook> list = testLibrary.GetBookList();
			bool failed = false;
			foreach (var book in list)
				if (book.WhoTaken != null)
					failed = true;

			Assert.IsFalse(failed);
		}

		[TestMethod]
		public void F_CommunicateToLibrary_RemoveBookThatDoesNotExist_ListCountDoesNotChange()
		{
			string command = "remove " + $"I dont exist/I dont exist";
			int initialList = testLibrary.GetBookListCount();

			testLibrary.CommunicateToLibrary(command);

			int finishedListCount = testLibrary.GetBookListCount();

			Assert.IsTrue(initialList == finishedListCount);
		}

		[TestMethod]
		public void G_CommunicateToLibrary_RemoveBookThatExists_ListCountDecreases()
		{
			string command = "remove " + $"{testBook.Name}/{testBook.Author}";
			int initialList = testLibrary.GetBookListCount();

			testLibrary.CommunicateToLibrary(command);

			int finishedListCount = testLibrary.GetBookListCount();

			Assert.IsTrue(initialList > finishedListCount);
		}
	}
}
