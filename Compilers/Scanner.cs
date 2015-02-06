using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Compilers
{
	public class Scanner
	{
	
		//constructor
		static Scanner ()
		{


		}


		//dispatcher method destroys whitespace and sends strings to the scanner
		public static string Dispatcher (string x)
		{

			//a string and StringBuilder to build the output token string
			string tokens;
			StringBuilder tokenizer = new StringBuilder ();

			//disposable stringreader to process the input string
			using (StringReader win = new StringReader (x)) {

				//loop while the next char is not end of file
				while (win.Peek () != -1) {
					//chomp whitespace until next no empty char
					if (Char.IsWhiteSpace ((char)win.Peek ())) {
						char junk = (char)win.Read ();
					} 
					else if (win.Peek() == 123) {
						int junk3 = win.Read ();
						bool eoc = false;
						while (!eoc) {
							int comp2 = win.Peek ();
							if (comp2 == 125) {
								int junk = win.Read ();
								eoc = true;
							} 
							else if(Char.IsWhiteSpace((char)comp2)){
								int junk2 = win.Read ();

							}else {
								int junk = win.Read ();
							}

						}
						//int junk2 = check.Read();
						tokenizer.Append ("MP_COMMENT \n");
					}

					else if (win.Peek() == 39) {


						bool eoc = false;
						while (!eoc) {
							int junk3 = win.Read ();
							int comp2 = win.Peek ();
							if (comp2 == 39) {
								int junk = win.Read ();
								eoc = true;
							} 
							else if(Char.IsWhiteSpace((char)comp2)||comp2 == -1){
								int junk2 = win.Read ();
							}else {
								int junk = win.Read ();
							}

						}
						//int junk2 = check.Read();
						tokenizer.Append ("MP_STRING_LIT \n");
					}
					//when a char is found, build a string to feed to a scanner
					else {
						StringBuilder str = new StringBuilder ();
						bool white_space = false;
						while (!white_space) {
							//loop until the next char is whitespace or EOF
							str.Append ((char)win.Read ());
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

		public static string Scan (string y)
		{
			//output string
			string process;
			StringBuilder build = new StringBuilder ();

			Dictionary<string, string> reserved = new Dictionary<string, string> ();
			reserved.Add ("and", "MP_AND \n");
			reserved.Add ("begin", "MP_BEGIN \n");
			reserved.Add ("div", "MP_DIV \n");
			reserved.Add ("do", "MP_DO \n");
			reserved.Add ("downto", "MP_DOWNTO \n");
			reserved.Add ("else", "MP_ELSE \n");
			reserved.Add ("end", "MP_END \n");
			reserved.Add ("fixed", "MP_FIXED \n");
			reserved.Add ("float", "MP_FLOAT \n");
			reserved.Add ("for", "MP_FOR \n");
			reserved.Add ("function", "MP_FUNCTION \n");
			reserved.Add ("if", "MP_IF \n");
			reserved.Add ("integer", "MP_INTEGER \n");
			reserved.Add ("mod", "MP_MOD \n");
			reserved.Add ("not", "MP_NOT \n");
			reserved.Add ("or", "MP_OR \n");
			reserved.Add ("proceedure", "MP_PROCEEDURE \n");
			reserved.Add ("program", "MP_PROGRAM \n");
			reserved.Add ("read", "MP_READ \n");
			reserved.Add ("repeat", "MP_REPEAT \n");
			reserved.Add ("then", "MP_THEN \n");
			reserved.Add ("to", "MP_TO \n");
			reserved.Add ("until", "MP_UNTIL \n");
			reserved.Add ("var", "MP_VAR \n");
			reserved.Add ("while", "MP_WHILE \n");
			reserved.Add ("write", "MP_WRITE \n");

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
					if ((comp >= 65 && comp <= 90) || (comp >= 97 && comp <= 122) || (comp == 95)) {
						StringBuilder id_build = new StringBuilder ();
						id_build.Append ((char)comp);
						//grab the rest of the identifier in while loop
						bool is_id = true;
						while (is_id) {
							//see if the next char is also part of the identifier
							int comp2 = check.Peek ();
							if ((comp2 >= 65 && comp2 <= 90) || (comp2 >= 97 && comp2 <= 122) || (comp2 == 95) || (comp2 >= 48 && comp <= 57)) {
								//consume the next char
								id_build.Append ((char)check.Read ());
							} else {

								//check if the id is a reserved word
								string res_check = id_build.ToString ();
								try {
									string res = reserved [res_check];
									build.Append (res);
								} catch (KeyNotFoundException) {
									//reached the end of the id, return id token and exit the loop
									build.Append ("MP_IDENTIFIER \n");
								}


								is_id = false;
							}
						}
					}

					//check if it begins with a decimal
					//if yes, then it is either a int, fixed, or float
					else if (comp >= 48 && comp <= 57) {

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
							else if (comp2 == 46) {
								if (is_fixed) {
									build.Append ("MP_ERROR \n");
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
									build.Append ("MP_ERROR \n");
									is_num = false;
								}
							}
							//if it's an e or E, then it's a float
							else if (comp2 == 69 || comp2 == 101) {
								if (is_float) {
									build.Append ("MP_ERROR \n");
									is_num = false;
								} else {
									is_float = true;
								}
								int junk = check.Read ();

								//make sure the next char is a digit or minus sign
								int err_spot = check.Peek ();
								if (err_spot == 45) {
									//see if the minus sign is followed by a digit
									int junk2 = check.Read ();
									int err_spot2 = check.Peek ();
									if (err_spot2 >= 48 && err_spot2 <= 57) {

									} else {
										build.Append ("MP_ERROR \n");
										is_num = false;
									}
								}
								//check if the E is followed by a digit
								else if (err_spot >= 48 && err_spot <= 57) {

								} else {
									build.Append ("MP_ERROR \n");
									is_num = false;
								}
							} else {
								//reached the end of the number, check what kind of
								//number it is based on the flags

								if (is_float) {
									build.Append ("MP_FLOAT_LIT \n");
								} else if (is_fixed) {
									build.Append ("MP_FIXED_LIT \n");
								} else {
									build.Append ("MP_INTEGER_LIT \n");
								}

								is_num = false;
							}
						}

					} else if (comp == 46) {
						build.Append ("MP_PERIOD \n");
					} else if (comp == 44) {
						build.Append ("MP_COMMA \n");
					} else if (comp == 59) {
						build.Append ("MP_SCOLON \n");
					} else if (comp == 40) {
						build.Append ("MP_LPAREN \n");
					} else if (comp == 41) {
						build.Append ("MP_RPAREN \n");
					} else if (comp == 61) {
						build.Append ("MP_EQUAL \n");
					} else if (comp == 62) {
						int i62 = check.Peek ();
						if (i62 == 61) {
							int junk = check.Read ();
							build.Append ("MP_GEQUAL \n");
						} else {
							build.Append ("MP_GTHAN \n");
						} 
					} else if (comp == 60) {
						int i60 = check.Peek ();
						if (i60 == 61) {
							int junk = check.Read ();
							build.Append ("MP_LEQUAL \n");
						} else if (i60 == 62) {
							int junk = check.Read ();
							build.Append ("NEQUAL \n");
						} else {
							build.Append ("MP_LTHAN \n");
						} 
					} else if (comp == 58) {
						int i58 = check.Peek ();
						if (i58 == 61) {
							int junk = check.Read ();
							build.Append ("MP_ASSIGN \n");
						} else {
							build.Append ("MP_COLON \n");
						}
					} else if (comp == 43) {
						build.Append ("MP_PLUS \n");
					} else if (comp == 45) {
						build.Append ("MP_MINUS \n");
					} else if (comp == 39) {


						bool eos = false;
						while (!eos) {
							int comp2 = check.Peek ();
							bool white_space2 = Char.IsWhiteSpace ((char)comp2);
							if (comp2 == 39) {
								int junk = check.Read ();
								int comp3 = check.Peek ();
								if (comp3 == 39) {
									int junk2 = check.Read ();
								} else {
									int junk2 = check.Read ();
									build.Append ("MP_STRING_LIT \n");
									eos = true;
								}
							} else if (white_space2) {
								int junky = check.Read ();
							} else {
								int junk = check.Read ();
							}

						}

					} else if (comp == 42) {
						build.Append ("MP_TIMES \n");
					} else if (comp == 123) {
						bool eoc = false;
						while (!eoc) {
							int comp2 = check.Peek ();
							if (comp2 == 125) {
								int junk = check.Read ();
								eoc = true;
							} 
							else if(Char.IsWhiteSpace((char)comp2)||comp2 == -1){
								int junk2 = check.Read ();
								Console.Write ("no");
						}else {
								int junk = check.Read ();
							}

						}
						//int junk2 = check.Read();
						build.Append ("MP_COMMENT \n");
					}
							
					//check for apostrophe
					//apostrophe starts a string literal
					else if (false) {

					} else if (false) {
						//OTHER SCANNERS GO HERE
					} else {
						Console.Write (comp);
						//character unrecognized, append error to string
						build.Append ("MP_ERROR \n");
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


	}




}

