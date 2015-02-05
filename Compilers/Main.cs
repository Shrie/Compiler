using System;
using System.IO;

namespace Compilers
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			string contents = File.ReadAllText(@"c:\users\Austo89\Desktop\compilers\test1.txt");
			string output = Dispatcher.Reader (contents);
			Console.Write(output);
		}
	}
}

