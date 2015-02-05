using System;
using System.IO;
using System.Text;

namespace Compilers
{
	public class Scanner
	{
		//dispatcher method destroys whitespace and sends strings to the scanner
		public static string Dispatcher (string x)
		{
			//a string and StringBuilder to build the output token string
			string tokens;
			StringBuilder tokenizer = new StringBuilder ();

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
			//append EOF
			tokenizer.Append ("MP_EOF");

			//move StringBuilder content to output string
			tokens = tokenizer.ToString ();
			return tokens;
		}


		//this method is called by Dispatcher to determine what tokens are in a string

		public static string Scan(string y){
			//output string
			string process;
			StringBuilder build = new StringBuilder ();

			//read the input string with StringReader
			using (StringReader check = new StringReader (y)) {
				//IMPORTANT!
				//you can find the unicode table online to get char decimal values

				bool white_space = false;
				while (!white_space) {

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
								//reached the end of the id, return id token and exit the loop
								build.Append ("MP_IDENTIFIER ");
								is_id = false;
							}
						}
					}

					//check if it begins with a decimal
					//if yes, then it is either a int, fixed, or float
					else if(comp >= 48 && comp <= 57){

						//grab the rest of the number in a while loop
						bool is_num = true;
						bool is_fixed = false;
						bool is_float = false;
						while (is_num) {
							//see if the next char is also part of the number
							int comp2 = check.Peek ();
							if (comp2 >= 48 && comp2 <= 57) {
								//consume the next char
								int junk = check.Read ();
							}
							//if it's a decimal point it could be a fixed or float
							else if(comp2 == 46) {
								if (is_fixed) {
									build.Append ("MP_ERROR ");
									is_num = false;
								} else {
									is_fixed = true;
								}
								int junk = check.Read ();

								//make sure the next char is a digit
								//otherwise it's an error
								int err_spot = check.Peek ();
								if (err_spot >= 48 && err_spot <= 57) {

								} else {
									build.Append ("MP_ERROR ");
									is_num = false;
								}
							}
							//if it's an e or E, then it's a float
							else if(comp2 == 69 || comp2 == 101){
								if (is_float) {
									build.Append ("MP_ERROR ");
									is_num = false;
								} else {
									is_float = true;
								}
								int junk = check.Read ();

								//make sure the next char is a digit or minus sign
								int err_spot = check.Peek ();
								if (err_spot == 45){
									//see if the minus sign is followed by a digit
									int junk2 = check.Read ();
									int err_spot2 = check.Peek ();
									if (err_spot2 >= 48 && err_spot2 <= 57) {

									} else {
										build.Append ("MP_ERROR ");
										is_num = false;
									}
								}
								else if (err_spot >= 48 && err_spot <= 57) {

								} else {
									build.Append ("MP_ERROR ");
									is_num = false;
								}
							}
							else {
								//reached the end of the number, check what kind of
								//number it is based on the flags

								if (is_float) {
									build.Append ("MP_FLOAT_LIT ");
								} else if (is_fixed) {
									build.Append ("MP_FIXED_LIT ");
								} else {
									build.Append ("MP_INTEGER_LIT ");
								}

								is_num = false;
							}
						}

					}
					else if(false){

						//OTHER SCANNERS GO HERE

					}
					else{
						//character unrecognized, append error to string
						build.Append ("MP_ERROR ");
					}

					white_space = Char.IsWhiteSpace ((char)check.Peek ());
					if (check.Peek () == -1) {
						white_space = true;
					}

				}
			}

			process = build.ToString ();
			return process;
		}


		//method to check if an identifier is a reserved word
		public static string reserved_word(string y){
			string process = " ";

			return process;
		}
	}




}

