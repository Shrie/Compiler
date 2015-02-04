using System;

namespace Compilers
{
	public class Scanner
	{
		public static void ScannerReader (string x)
		{
			//Console.WriteLine (x); //for testing
			for (int i = 0; i < x.Length; i++)
				using (StringReader sr = new StringReader (x)) {
					sr.Read(checked, 0, 1);

				}
		}
	}
}

