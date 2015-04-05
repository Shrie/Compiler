using System;
using System.IO;

namespace Compilers
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			//test file paths
			//@"c:\users\Hunter\Desktop\compilers\test1.txt"
			//@"c:\users\Austo89\Desktop\compilers\test1.txt"
			//@"/Users/David/Desktop/test1.txt"
			string contents = File.ReadAllText(@"c:\users\Austo89\Desktop\compilers\test1.txt");
			string output = Scanner.Dispatcher (contents);
			//.Write(output);
			Console.WriteLine (output);
			Parser testes = new Parser (output);
			testes.Parse ();

		}
	}
}

