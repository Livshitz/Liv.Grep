using System;
using System.Linq;
using Liv.CommandlineArguments;
using Liv.Logging;
using System.Diagnostics;

namespace Liv.GrepCmd
{
	class Program
	{
		private static ArgsManager<Args> _args;
		public enum Args
		{
			[Option(Description = "Find column")]
			Column,
			[Option(ShortName = "r", Type = typeof(int), Description = "Find Row")]
			RowNumber,
			[Option(ShortName = "f", Description = "Find Row contianing this string")]
			Find,
			[Option(Description = "Write output with column names")]
			WriteColumns,
			[Option()]
			Debug,
		}

		static void Main(string[] args)
		{
			Log.SetConsoleTracing(false, Log.TraceLevel.Error);

			_args = new ArgsManager<Args>(args);
			//Console.WriteLine(_args.TraceArguments());

			string input = "";

			if (_args.HasValue(Args.Debug))
			{
				Debugger.Launch();
				Debugger.Break();
			}

			// Get Pipe
			if (IsPipedInput())
			{
				string s;
				while ((s = Console.ReadLine()) != null)
				{
					input += s + Environment.NewLine;
				}
			}

			if (input == "") throw new Exception("No input was provided");
			if (args.Length == 0)
			{
				Console.WriteLine(input);
				return;
			}

			var g = new Grep(input);

			// Only find rows, without column specification
			if (!_args.HasValue(Args.Column) && _args.HasValue(Args.Find))
			{
				if (_args.HasValue(Args.WriteColumns)) Log.Write(g.GetColumnsRow());
				Array.ForEach(g.GetRowsContaining(_args.GetValue(Args.Find)), Log.Write);
				return;
			}

			// Column specific
			var col = g.GetColumn(_args.GetValue(Args.Column)); //.Values.ForEach(Console.WriteLine);

			if (col == null)
			{
				Log.Error("Column '{0}' could not be found!", _args.GetValue(Args.Column));
				return;
			}

			if (_args.HasValue(Args.Find))
			{
				var rows = g.FindRowsContaining(_args.GetValue(Args.Find));
				foreach (var row in rows)
				{
					Log.Write(col.Values[row]);
				}
			}
			else if (!_args.HasValue(Args.RowNumber))
			{
				col.Values.ForEach(Log.Write);
			}
			else
			{
				var val = col.Values[_args.GetValue<int>(Args.RowNumber)];
				Log.Write("Value= ".Yellow() + val);
				return;
			}
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
