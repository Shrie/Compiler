using System;
using System.IO;
using System.Text;

namespace Compilers
{
	public class Scanner
	{
		public static string Dispatcher (string x)
		{
			//a string and StringBuilder to build the output token string
			string tokens;
			StringBuilder tokenizer = new StringBuilder ();

			//psuedo code for dispatcher
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

			//disposable stringreader to process the input string
			using (StringReader win = new StringReader (x)) 
			{

				//loop while the next char is not end of file
				while (win.Peek() != -1)
				{
					//chomp whitespace until next no empty char
					if (Char.IsWhiteSpace((char)win.Peek())) 
					{
						char junk = (char)win.Read();
					} 
					//when a char is found, build a string to feed to a scanner
					else 
					{
						StringBuilder str = new StringBuilder ();
						bool white_space = false;
						while (!white_space) 
						{
							//loop until the next char is whitespace or EOF
							str.Append ((char)win.Read());
							white_space = Char.IsWhiteSpace ((char)win.Peek ());
							if (win.Peek () == -1) {
								white_space = true;
							}
						}
						//move to a scannable string
						string scan_it = str.ToString ();

						//add tokens to the output
						tokenizer.Append (scan_it);

					}
				}
			}
			//move StringBuilder content to output string
			tokens = tokenizer.ToString ();
			return tokens;
		}
	}




}

