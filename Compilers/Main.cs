using System;
using System.IO;

namespace Compilers
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			string contents = File.ReadAllText(@"c:\users\Hunter\Desktop\compilers\test1.txt");
			Dispatcher.Reader (contents);
		}
	}
}

