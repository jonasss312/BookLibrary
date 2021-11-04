 using Newtonsoft.Json;
using System;
using System.IO;

namespace BookLibrary
{
	// Class that implements Book class and has additional methods
	public class LibraryBook : Book
	{
		// Attribute that shows who has taken the book or if book is not taken [null]
		public string WhoTaken { get; set; }
		// Attribute that shows for how many days person took this book
		public int PeriodDays { get; set; }
		// Attribute used to calculate if person is not too late to return a book
		public DateTime DateTaken { get; set; }

		// Empty constructor
		public LibraryBook() { }

		// Constructor used for creating LibraryBook class object from Book class object
		public LibraryBook(Book book)
		{
			this.Name = book.Name;
			this.Author = book.Author;
			this.Category = book.Category;
			this.Language = book.Language;
			this.Publication_date = book.Publication_date;
			this.ISBN = book.ISBN;
		}

		// Function that checks if book name and author matches
		public bool CheckBookNameAndAuthor(string wantedBookName, string wantedBookAuthor)
		{
			if (this.Author == wantedBookAuthor && this.Name == wantedBookName)
				return true;
			return false;
		}

		// Function that checks if book is taken
		public bool BookIsReadyToTake()
		{
			if (this.WhoTaken == null)
				return true;
			return false;
		}

		// Function that checks if specific person has this book
		public bool CheckIfThisPersonHasThisBook(string who)
		{
			if (this.WhoTaken == who)
				return true;
			return false;
		}

		// Method that makes this book taken by a person for specific day count
		public void MakeBookRent(string whoTaken, int periodDays)
		{
			// Assigning attributes
			this.WhoTaken = whoTaken;
			this.PeriodDays = periodDays;
			// Making a datestamp
			this.DateTaken = DateTime.Now;
		}

		// Method that makes this book available for next take
		public void RemoveBookRent()
		{
			// Unassigning atttribute
			this.WhoTaken = null;
			// Calculating how many days passed after taking this book
			int daysPassed = (new DateToday().Today - this.DateTaken).Days;
			//Checking if this person returned book in time
			if (daysPassed > PeriodDays)
			{
				Console.WriteLine("We already deleted this book from our list..(joke)");
			}
			// Unassigning atttribute
			this.PeriodDays = 0;
		}

		// Function that returns Book class object converted from json
		public override Book ReadBook(string bookPath)
		{
			return JsonConvert.DeserializeObject<LibraryBook>(File.ReadAllText(bookPath));
		}

		//Used for testing
		public LibraryBook Return()
		{
			return this;
		}
	}
}
