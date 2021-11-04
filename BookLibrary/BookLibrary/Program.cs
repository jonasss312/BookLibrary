using System;

namespace BookLibrary
{
	class Program
	{
		// Library data file
		static string libraryPath = "Library.json";
		static void Main(string[] args)
		{
			// Creating library instance
			Library library = new Library(libraryPath);
			while (true)
			{
				// Printing instructions
				WriteCommands();
				// Getting input from user
				string input = Console.ReadLine();
				// Sending input to library
				library.CommunicateToLibrary(input);

				Console.WriteLine("Press any key to continue");
				Console.ReadKey();
				Console.Clear();
			}
		}

		static void WriteCommands()
		{
			Console.WriteLine("Welcome to Library\n" +
				"Available commands:\n" +
				"add [book_path + file_name.json]\n" +
				"take -> [who_is_taking] -> [period_in_days] -> [book_name]/[book_author]\n" +
				"return [book_name]/[book_author]\n" +
				"list [by:(author/category/language/ISBN/name/taken/available)] [book_author/book_category/etc..]\n" +
				"remove [book_name]/[book_author]");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\nExamples:\nadd NewBook.json\n" +
				"take -> Reader10025 -> 30 -> In Search of Lost Time/Marcel Proust\n" +
				"return In Search of Lost Time/Marcel Proust\n" +
				"list author/category Marcel Proust/Novel\n" +
				"remove In Search of Lost Time/Marcel Proust\n");
			Console.ResetColor();
		}
	}
}
