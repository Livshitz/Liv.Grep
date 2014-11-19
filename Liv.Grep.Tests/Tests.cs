using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Liv.GrepTests
{
	[TestFixture]
	public class QuerySessionTests
	{
		private string Input;

		[TestFixtureSetUp]
		public void Setup()
		{
			Input = @" SESSIONNAME       USERNAME                 ID  STATE   TYPE        DEVICE 
 services                                    0  Disc                        
>rdp-tcp#6         eli                       1  Active                      
 console                                     3  Conn                        
 7a78855482a04...                        65536  Listen                      
 rdp-tcp                                 65537  Listen                      
";
		}

		[Test]
		public void GeneralCountTest()
		{
			var g = new Grep(Input);
			Assert.AreEqual(g.Columns.Length, 6);
			foreach (var c in g.Columns)
			{
				Assert.AreEqual(c.Values.Count, 5); 
			}
		}

		[Test]
		public void GetColumnTest()
		{
			var g = new Grep(Input);
			var val = g.GetColumn("SessionName").Values[0];
			Assert.AreEqual(val, "services");
		}

		[Test]
		public void BasicColumnTest()
		{
			var g = new Grep(Input);

			Assert.AreEqual(g.GetColumn("SessionName").Values[0], "services");
			Assert.AreEqual(g.GetColumn("UserName").Values[0], "");
			Assert.AreEqual(g.GetColumn("Id").Values[0], "0");
			Assert.AreEqual(g.GetColumn("state").Values[0], "Disc");
			Assert.AreEqual(g.GetColumn("TYPE").Values[0], "");
			Assert.AreEqual(g.GetColumn("DeViCe").Values[0], "");
		}

		[Test]
		public void SlippingColumnTest()
		{
			var g = new Grep(Input);

			Assert.AreEqual(g.GetColumn("SessionName").Values[3], "7a78855482a04...");
			Assert.AreEqual(g.GetColumn("UserName").Values[3], "");
			Assert.AreEqual(g.GetColumn("Id").Values[3], "65536");
			Assert.AreEqual(g.GetColumn("state").Values[3], "Listen");
			Assert.AreEqual(g.GetColumn("TYPE").Values[3], "");
			Assert.AreEqual(g.GetColumn("DeViCe").Values[3], "");
		}
	}

}
