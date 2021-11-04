using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookLibrary.Tests
{
	[TestClass]
	public class LibraryBookTests
	{
		[TestMethod]
		public void CheckBookNameAndAuthor_NameAndAuthorMatch_ReturnsTrue()
		{
			var book = new LibraryBook();
			string bookName = "name";
			string bookAuthor = "author";
			book.Name = bookName;
			book.Author = bookAuthor;

			var result = book.CheckBookNameAndAuthor(bookName, bookAuthor);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CheckBookNameAndAuthor_OnlyNameMatch_ReturnsFalse()
		{
			var book = new LibraryBook();
			string bookName = "name";
			book.Name = bookName;
			book.Author = "Author1";

			var result = book.CheckBookNameAndAuthor(bookName, "Author2");

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CheckBookNameAndAuthor_OnlyAuthorMatch_ReturnsFalse()
		{
			var book = new LibraryBook();
			string bookAuthor = "name";
			book.Name = "Book1";
			book.Author = bookAuthor;

			var result = book.CheckBookNameAndAuthor("Book2", bookAuthor);

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CheckBookNameAndAuthor_NameAndAuthorDoNotMatch_ReturnsFalse()
		{
			var book = new LibraryBook();
			book.Name = "Book1";
			book.Author = "Author1";

			var result = book.CheckBookNameAndAuthor("Book2", "Author2");

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void BookIsReadyToTake_WhoAtrributeIsNotNull_ReturnsFalse()
		{
			var book = new LibraryBook();
			book.WhoTaken = "Me";

			var result = book.BookIsReadyToTake();

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void BookIsReadyToTake_WhoAtrributeIsNull_ReturnsTrue()
		{
			var book = new LibraryBook();

			var result = book.BookIsReadyToTake();

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CheckIfThisPersonHasThisBook_PersonHasThisBook_ReturnsTrue()
		{
			var book = new LibraryBook();
			book.WhoTaken = "Me";

			var result = book.CheckIfThisPersonHasThisBook("Me");

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CheckIfThisPersonHasThisBook_PersonDoNotHaveThisBook_ReturnsFalse()
		{
			var book = new LibraryBook();
			book.WhoTaken = "Me";

			var result = book.CheckIfThisPersonHasThisBook("Not me");

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void MakeBookRent_AllDataPassed_AssignsBookAtrributes()
		{
			var book = new LibraryBook();

			book.MakeBookRent("Me", 10);

			Assert.IsTrue(book.WhoTaken != null && book.PeriodDays != null && book.DateTaken != null);
		}

		[TestMethod]
		public void RemoveBookRent_BookIsRentedAndReadyToDerent_DeassignsBookAtrributes()
		{
			var book = new LibraryBook();
			book.MakeBookRent("Me", 10);

			book.RemoveBookRent();

			Assert.IsTrue(book.WhoTaken == null && book.PeriodDays == 0);
		}

		[TestMethod]
		public void ReadBook_BookDataIsCorrect_ReturnsBookData()
		{
			string testBookPath = "..\\..\\..\\TestBook.json";
			Book book = new LibraryBook().ReadBook(testBookPath);

			Assert.IsTrue(book != null);
		}
	}
}
