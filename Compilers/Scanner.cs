using System;

namespace Compilers
{
	public class Dispatcher
	{
		public static void Reader (string x)
		{
			//Console.WriteLine (x); //for testing
			//Console.WriteLine (x.Length);
			for (int i = 0; i < x.Length; i++)
			{
				/* Sudo code
				 * 
				 * scan_next_char
				 * 
				 * if(char_scanned.isWhiteSpace())
				 * {
				 * 		i++
				 * }
				 * else
				 * {
				 * 		while(char_scanned.isNotWhiteSpace())
				 * 		{
				 * 			y += char_scanned
				 * 			i++
				 * 		}
				 * }
				 * if(y.lenth != 0)
				 * {
				 * 		Scanner(String y)
				 * }
				 */
			}
		}
	}
}

