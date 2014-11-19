using System;
using System.Collections.Generic;
using System.Linq;

namespace Liv
{
    public class Grep
    {
		private const int ColumnBorderSize = 2;
		private string Input;
		private string[] Rows;

	    public Column[] Columns { get; private set; }

	    public Grep(string input)
	    {
		    Input = input;
			Rows = Input.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			BreakColumns();
	    }

	    public string GetColumnsRow()
	    {
		    return Rows[0];
	    }

	    public Column GetColumn(string columnName)
	    {
		    return Columns.SingleOrDefault(x => x.Title.ToLower() == columnName.ToLower());
	    }

	    public string GetValue(string columnName, int lineNum)
	    {
		    return GetColumn(columnName).Values[lineNum];
	    }

	    public int[] FindRowsContaining(string needle)
	    {
		    var ret = new List<int>();
		    for (int i = 0; i < Rows.Length; i++)
		    {
			    var row = Rows[i];
			    if (!row.ToLower().Contains(needle.ToLower())) continue;
				ret.Add(i - 1);
		    }
		    return ret.ToArray();
	    }

	    public string[] GetRowsContaining(string needle)
	    {
		    var lines = FindRowsContaining(needle);
		    var ret = new List<string>();
		    foreach (var line in lines)
		    {
			    ret.Add(Rows[line + 1]);
		    }
		    return ret.ToArray();
	    }

	    private void BreakColumns()
	    {
			var lines = Rows;
			var columnsStr = lines[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			var columns = new List<Column>();
			for (int i = 0; i < lines.Length; i++)
			{
				for (int i2 = 0; i2 < columnsStr.Length; i2++)
				{
					if (i == 0)
					{
						var c = columnsStr[i2];
						var newCol = new Column(c);
						newCol.PosX = lines[0].IndexOf(c);
						if (i2 < columnsStr.Length - 1)
							newCol.Width = lines[0].IndexOf(columnsStr[i2 + 1]) - newCol.PosX - ColumnBorderSize;
						else
							newCol.Width = lines[0].Length - newCol.PosX;

						columns.Add(newCol);
					}
					else
					{
						var curCol = columns[i2];
						var val = lines[i].Substring(curCol.PosX, curCol.Width);
						var valStartPos = val.IndexOf(val.Trim());
						bool hasValueFromStart = val[0] != ' ';
						bool hasValueFromEnd = false;
						bool hasSlippingValue = false;
						if (valStartPos != 0)
						{
							hasValueFromEnd = val[curCol.Width - 1] != ' ';
						}
						if (hasValueFromEnd)
						{
							hasSlippingValue = lines[i].Substring(curCol.PosX + curCol.Width + 1, 1) != " ";
						}
						if (hasSlippingValue)
						{
							var slippingLength = 0;
							for (int n = val.Length - 1; n >= 0; n--)
							{
								var curChar = val[n];
								if (curChar == ' ')
								{
									slippingLength = val.Length - n - 1;
									break;
								}
							}
							curCol.Width = curCol.Width - slippingLength - ColumnBorderSize;
							var nextCol = columns[i2 + 1];
							nextCol.Width = nextCol.Width + slippingLength + ColumnBorderSize;
							nextCol.PosX = nextCol.PosX - slippingLength - ColumnBorderSize;

							val = lines[i].Substring(curCol.PosX, curCol.Width);
						}
						curCol.Values.Add(val.Trim());
					}
				}
			}

			Columns = columns.ToArray();
	    }

		public class Column
		{
			public string Title { get; set; }
			public int PosX { get; set; }
			public int Width { get; set; }
			public List<string> Values { get; set; }
			public ColumnAlignment Alignment { get; set; }

			public Column(string title)
			{
				Title = title;
				Values = new List<string>();
			}

			public override string ToString()
			{
				return Title;
			}
		}

		public enum ColumnAlignment
		{
			Left,
			Right,
			Center,
			Justify
		}
    }
}
