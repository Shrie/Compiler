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
					} else if (win.Peek () == 123) {
						int junk3 = win.Read ();
						bool eoc = false;
						bool err_flag = false;
						while (!eoc) {
							int comp2 = win.Peek ();
							if (comp2 == 125) {
								int junk = win.Read ();
								eoc = true;
							} else if (comp2 == -1) {

								tokenizer.Append ("MP_RUN_COMMENT \n");
								eoc = true;
								err_flag = true;
							} else if (Char.IsWhiteSpace ((char)comp2)) {
								int junk2 = win.Read ();
							} else {
								int junk = win.Read ();
							}

						}
						//int junk2 = check.Read();
						if (!err_flag) {
							tokenizer.Append ("MP_COMMENT \n");
						}
					} else if (win.Peek () == 39) {
						int junk3 = win.Read ();

						bool run_flag = false;
						bool eoc = false;
						while (!eoc) {
							//int junk3 = win.Read ();
							int comp2 = win.Peek ();
							if (comp2 == 39) {
								int junk = win.Read ();
								eoc = true;
							} else if (comp2 == 13 || comp2 == 133 || comp2 == -1) {

								tokenizer.Append ("MP_STRING_RUN \n");
								eoc = true;
								run_flag = true;
							} else if (Char.IsWhiteSpace ((char)comp2) || comp2 == -1) {
								int junk2 = win.Read ();
							} else {
								int junk = win.Read ();
							}

						}
						//int junk2 = check.Read();
						if (!run_flag) {
							tokenizer.Append ("MP_STRING_LIT \n");
						}
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
			reserved.Add ("and", String.Format("{0,-16}{1,-20}{2,0}","MP_AND", "and", "\n"));
			reserved.Add ("begin", String.Format("{0,-16}{1,-20}{2,0}","MP_BEGIN", "begin", "\n"));
			reserved.Add ("Boolean", String.Format ("{0,-16}{1,-20}{2,0}", "MP_BOOLEAN", "Boolea", "\n"));
			reserved.Add ("div", String.Format("{0,-16}{1,-20}{2,0}","MP_DIV", "div", "\n"));
			reserved.Add ("do", String.Format("{0,-16}{1,-20}{2,0}","MP_DO", "do", "\n"));
			reserved.Add ("downto", String.Format("{0,-16}{1,-20}{2,0}","MP_DOWNTO", "downto", "\n"));
			reserved.Add ("else", String.Format("{0,-16}{1,-20}{2,0}","MP_ELSE", "else", "\n"));
			reserved.Add ("end", String.Format("{0,-16}{1,-20}{2,0}","MP_END", "end", "\n"));
			reserved.Add ("false", String.Format ("{0,-16}{1,-20}{2,0}", "MP_FALSE", "false", "\n"));
			reserved.Add ("fixed", String.Format("{0,-16}{1,-20}{2,0}","MP_FIXED", "fixed", "\n"));
			reserved.Add ("float", String.Format("{0,-16}{1,-20}{2,0}","MP_FLOAT", "float", "\n"));
			reserved.Add ("for", String.Format("{0,-16}{1,-20}{2,0}","MP_FOR", "for", "\n"));
			reserved.Add ("function", String.Format("{0,-16}{1,-20}{2,0}","MP_FUNCTION", "function", "\n"));
			reserved.Add ("if", String.Format("{0,-16}{1,-20}{2,0}","MP_IF", "if", "\n"));
			reserved.Add ("integer", String.Format("{0,-16}{1,-20}{2,0}","MP_INTEGER", "integer", "\n"));
			reserved.Add ("mod", String.Format("{0,-16}{1,-20}{2,0}","MP_MOD", "mod", "\n"));
			reserved.Add ("not", String.Format("{0,-16}{1,-20}{2,0}","MP_NOT", "not", "\n"));
			reserved.Add ("or", String.Format("{0,-16}{1,-20}{2,0}","MP_OR", "or", "\n"));
			reserved.Add ("procedure", String.Format("{0,-16}{1,-20}{2,0}","MP_PROCEEDURE", "proceedure", "\n"));
			reserved.Add ("program", String.Format("{0,-16}{1,-20}{2,0}","MP_PROGRAM", "program", "\n"));
			reserved.Add ("read", String.Format("{0,-16}{1,-20}{2,0}","MP_READ", "read", "\n"));
			reserved.Add ("repeat", String.Format("{0,-16}{1,-20}{2,0}","MP_REPEAT", "repeat", "\n"));
			reserved.Add ("string", String.Format("{0,-16}{1,-20}{2,0}","MP_STRING", "string", "\n"));
			reserved.Add ("then", String.Format("{0,-16}{1,-20}{2,0}","MP_THEN", "then", "\n"));
			reserved.Add ("to", String.Format("{0,-16}{1,-20}{2,0}","MP_TO", "to", "\n"));
			reserved.Add ("type", String.Format("{0,-16}{1,-20}{2,0}","MP_TYPE", "type", "\n"));
			reserved.Add ("until", String.Format("{0,-16}{1,-20}{2,0}","MP_UNTIL", "until", "\n"));
			reserved.Add ("var", String.Format("{0,-16}{1,-20}{2,0}","MP_VAR", "var", "\n"));
			reserved.Add ("while", String.Format("{0,-16}{1,-20}{2,0}","MP_WHILE", "while", "\n"));
			reserved.Add ("write", String.Format("{0,-16}{1,-20}{2,0}","MP_WRITE", "write", "\n"));

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
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_PERIOD", ".", "\n"));
					} else if (comp == 44) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_COMMA", ",", "\n"));
					} else if (comp == 59) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_SCOLON", ";", "\n"));
					} else if (comp == 40) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_LPAREN", "(", "\n"));
					} else if (comp == 41) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_RPAREN", ")", "\n"));
					} else if (comp == 61) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_EQUAL", "=", "\n"));
					} else if (comp == 62) {
						int i62 = check.Peek ();
						if (i62 == 61) {
							int junk = check.Read ();
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_GEQUAL", ">=", "\n"));
						} else {
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_GTHAN", ">", "\n"));
						} 
					} else if (comp == 60) {
						int i60 = check.Peek ();
						if (i60 == 61) {
							int junk = check.Read ();
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_LEQUAL", "<=", "\n"));
						} else if (i60 == 62) {
							int junk = check.Read ();
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","NEQUAL", "<>", "\n"));
						} else {
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_LTHAN", ">", "\n"));
						} 
					} else if (comp == 58) {
						int i58 = check.Peek ();
						if (i58 == 61) {
							int junk = check.Read ();
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_ASSIGN", ":=", "\n"));
						} else {
							build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_COLON", ":", "\n"));
						}
					} else if (comp == 43) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_PLUS", "+", "\n"));
					} else if(comp == 47){ 
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_FLOAT_DIVIDE", "\\", "\n"));
				}else if (comp == 45) {
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_MINUS", "-", "\n"));
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
						build.Append (String.Format("{0,-16}{1,-20}{2,0}","MP_TIMES", "*", "\n"));
					} else if (comp == 123) {
						bool eoc = false;
						while (!eoc) {
							int comp2 = check.Peek ();
							if (comp2 == 125) {
								int junk = check.Read ();
								eoc = true;
							} else if (Char.IsWhiteSpace ((char)comp2) || comp2 == -1) {
								int junk2 = check.Read ();
								Console.Write ("no");
							} else {
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

