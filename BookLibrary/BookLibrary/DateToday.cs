using System;

namespace BookLibrary
{
	//This class is used for return command to catch who returned book too late
	class DateToday
	{
		//Add days to catch who is late to give back a book
		public DateTime Today = DateTime.Now.AddDays(30);
	}
}
