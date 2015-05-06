// Main driver class for the Compiler project
// This will handle the file read and instances
// of the major classes Scanner and Parser

using System;
using System.IO;
using System.Collections;

namespace Compilers
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			//try to grab the file out of args[0]

			try{
				string contents = File.ReadAllText(args[0]);
				string output = Scanner.Dispatcher (contents);
				Scanner.PrintTokies ();

				ArrayList outies = Scanner.GetTokenArray();
				Console.WriteLine (output);

				Parser2 parser = new Parser2 (output,outies, args[0]);
				parser.Parse ();
			} catch (IOException){
				Console.WriteLine ("Couldn't open file.");
			} catch (IndexOutOfRangeException){
				Console.WriteLine ("Expecting a file argument.");
			}
		}
	}
}

