using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Liv.GrepTests
{
	class Program
	{
		static void Main(string[] args)
		{
			//var input = File.ReadAllText(@"c:\q.txt");
			string input = "";
			// Get Pipe

			//Debugger.Launch();
			if (IsPipedInput())
			{
				string s;
				while ((s = Console.ReadLine()) != null)
				{
					input += s + Environment.NewLine;
				}
			}

			Console.WriteLine("input = " + input);
			if (input == "") throw new Exception("No input was provided");

			var g = new Grep(input);
			var cols = g.Columns;

			g.GetColumn("sessionname").Values.ForEach(Console.WriteLine);
		}

		private static bool IsPipedInput()
		{
			try
			{
				bool isKey = Console.KeyAvailable;
				return false;
			}
			catch
			{
				return true;
			}
		}
	}
}
