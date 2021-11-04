namespace BookLibrary
{
	// Abstract class that helps to read books data from json file
	public abstract class Book
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string Category { get; set; }
		public string Language { get; set; }
		public string Publication_date { get; set; }
		public string ISBN { get; set; }

		// Abstract method which will be implemented in LibraryBook
		public abstract Book ReadBook(string bookPath);
	}
}
