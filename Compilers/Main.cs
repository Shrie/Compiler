﻿using System;
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
			string contents = File.ReadAllText(@"c:\users\Austo89\Desktop\compilers\test1.txt");
			string output = Scanner.Dispatcher (contents);
			Console.Write(output);
			Parser testes = new Parser ();
		}
	}
}

