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

			//unicode test
			//int bunyuns = (int)'a';
			//Console.Write (bunyuns);

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

						scan_it = Scan (scan_it);

						//add tokens to the output
						tokenizer.Append (scan_it);

					}
				}
			}
			//move StringBuilder content to output string
			tokens = tokenizer.ToString ();
			return tokens;
		}


		public static string Scan(string y){
			//output string
			string process;
			StringBuilder build = new StringBuilder ();

			//read the input string with StringReader
			using (StringReader check = new StringReader (y)) {
				//you can find the unicode table online to get char decimal values

				//store next char's decimal value in comp
				int comp = check.Read ();

				//check if first char is A-Z,a-z, or _
				//if it is, it's an identifier
				if((comp >= 65 && comp <= 90)||(comp >= 97 && comp <= 122)||(comp == 95)){
					//grab the rest of the identifier in while loop
					bool is_id = true;
					while (is_id) {
						//see if the next char is also part of the identifier
						int comp2 = check.Peek ();
						if ((comp2 >= 65 && comp2 <= 90) || (comp2 >= 97 && comp2 <= 122) || (comp2 == 95) || (comp2 >= 48 && comp <= 57)) {
							//consume the next char
							int junk = check.Read ();
						} else {
							build.Append ("MP_IDENTIFIER ");
							is_id = false;
						}
					}
				}
				else if(false){
					//fill with other scans
				}
				else if(false){

				}
				else{
					//character unrecognized, append error to string
					build.Append ("MP_ERROR ");
				}
			}

			process = build.ToString ();
			return process;
		}

		public static string reserved_word(string y){
			string process = " ";

			return process;
		}
	}




}

