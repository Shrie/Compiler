using System;
using System.IO;
using System.Text;

namespace Compilers
{
	public class Dispatcher
	{
		public static string Reader (string x)
		{
			string tokens;
			StringBuilder tokenizer = new StringBuilder ();
			//Console.WriteLine (x); //for testing
			//Console.WriteLine (x.Length);
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
				 * if(y.length != 0)
				 * {
				 * 		Scanner(String y)
				 * }
				 */
			using (StringReader win = new StringReader (x)) 
			{

				while (win.Peek() != -1)
				{
					if (Char.IsWhiteSpace((char)win.Peek())) 
					{
						char junk = (char)win.Read();


					} 
					else 
					{
						StringBuilder str = new StringBuilder ();
						bool white_space = false;
						while (white_space) 
						{
							Console.Write ();
							str.Append ((char)win.Read());
							white_space = Char.IsWhiteSpace ((char)win.Peek ());
						}

						string scan_it = str.ToString ();
						tokenizer.Append (scan_it);

					}
				}
			}
			tokens = tokenizer.ToString ();
			return tokens;
		}
	}




}

