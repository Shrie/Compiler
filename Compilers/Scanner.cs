﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace Compilers
{
	public class Scanner
	{
		static int row_counter = 1;
		static int column_counter = 1;
		static ArrayList tokies = new ArrayList();
	
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
					//chomp whitespace until next non empty char
					if (win.Peek() == 13 ||win.Peek() == 133 ||win.Peek() == -1) {
						column_counter = 0;
						row_counter++;
						int junk = win.Read ();
					}

					else if (Char.IsWhiteSpace ((char)win.Peek ())) {
						char junk = (char)win.Read ();
						column_counter++;
					} 
					//check for a comment
					//has to happen in the dispatcher at this point
					else if (win.Peek () == 123) {
						int add_count = 0;
						int row_add = 0;
						//build the comment into a lexeme
						StringBuilder lexeme = new StringBuilder ();
						lexeme.Append ((char)win.Read ());
						add_count++;

						bool eoc = false;
						bool err_flag = false;
						while (!eoc) {

							int comp2 = win.Peek ();
							if (comp2 == 125) {
								lexeme.Append ((char)win.Read ());
								eoc = true;
							} else if(comp2 == 13 ||comp2 == 133) {
								lexeme.Append ((char)win.Read ());
								add_count = 0;
								row_add++;
							} else if (comp2 == -1) {
								tokies.Add (new Token("MP_RUN_COMMENT",lexeme.ToString(),row_counter,column_counter));
								eoc = true;
								err_flag = true;
								column_counter += add_count;
							} else if (Char.IsWhiteSpace ((char)comp2)) {
								lexeme.Append ((char)win.Read ());
								add_count++;
							} else {
								lexeme.Append ((char)win.Read ());
								add_count++;
							}

						}

						if (!err_flag) {
							tokies.Add (new Token("MP_COMMENT",lexeme.ToString(),row_counter,column_counter));
							column_counter += add_count;
							row_counter += row_add;
						}
					} else if (win.Peek () == 39) {
						int add_count = 0;
						//build the string into a lexeme
						StringBuilder lexeme = new StringBuilder ();
						int chompApos = win.Read ();
						lexeme.Append ("\"");
						add_count++;

						bool run_flag = false;
						bool eos = false;
						while (!eos) {
							int comp2 = win.Peek ();
							if (comp2 == 39) {
								lexeme.Append ("\"");
								chompApos = win.Read ();
								eos = true;
								add_count++;
							} else if (comp2 == 13 || comp2 == 133 || comp2 == -1) {
								tokies.Add (new Token("MP_STRING_RUN",lexeme.ToString(),row_counter,column_counter));
								eos = true;
								run_flag = true;
								column_counter += add_count;
							} else if (Char.IsWhiteSpace ((char)comp2) || comp2 == -1) {
								lexeme.Append ((char)win.Read ());
								add_count++;
							} else {
								lexeme.Append ((char)win.Read ());
								add_count++;
							}
						}
						if (!run_flag) {
							tokies.Add (new Token("MP_STRING_LIT",lexeme.ToString(),row_counter,column_counter));
							column_counter += add_count;
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
							if (win.Peek () == -1 || win.Peek() == 39) {
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
			tokies.Add (new Token("MP_EOF","$",row_counter,column_counter));

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

			Dictionary<string, string> reserved2 = new Dictionary<string, string> ();
			reserved2.Add ("and", "MP_AND");
			reserved2.Add ("begin", "MP_BEGIN");
			reserved2.Add ("boolean", "MP_BOOLEAN");
			reserved2.Add ("div", "MP_DIV");
			reserved2.Add ("do", "MP_DO");
			reserved2.Add ("downto", "MP_DOWNTO");
			reserved2.Add ("else", "MP_ELSE");
			reserved2.Add ("end", "MP_END");
			reserved2.Add ("false", "MP_FALSE");
			reserved2.Add ("fixed","MP_FIXED");
			reserved2.Add ("float", "MP_FLOAT");
			reserved2.Add ("for", "MP_FOR");
			reserved2.Add ("function", "MP_FUNCTION");
			reserved2.Add ("if","MP_IF");
			reserved2.Add ("integer","MP_INTEGER");
			reserved2.Add ("mod", "MP_MOD");
			reserved2.Add ("not","MP_NOT");
			reserved2.Add ("or", "MP_OR");
			reserved2.Add ("procedure", "MP_PROCEDURE");
			reserved2.Add ("program","MP_PROGRAM");
			reserved2.Add ("read",  "MP_READ");
			reserved2.Add ("repeat", "MP_REPEAT");
			reserved2.Add ("string","MP_STRING");
			reserved2.Add ("then", "MP_THEN");
			reserved2.Add ("to","MP_TO");
			reserved2.Add ("type","MP_TYPE");
			reserved2.Add ("until","MP_UNTIL");
			reserved2.Add ("var", "MP_VAR");
			reserved2.Add ("while",  "MP_WHILE");
			reserved2.Add ("write", "MP_WRITE");
			reserved2.Add ("writeln", "MP_WRITELN");
			reserved2.Add ("true", "MP_TRUE");



			//read the input string with StringReader
			using (StringReader check = new StringReader (y)) {
				//IMPORTANT!
				//you can find the unicode table online to get char decimal values

				bool white_space = false;
				while (!white_space) {
					int add_count = 0;
					//store next char's decimal value in comp
					int comp = check.Read ();
					add_count++;

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
							if ((comp2 >= 65 && comp2 <= 90) || (comp2 >= 97 && comp2 <= 122) || (comp2 == 95) || (comp2 >= 48 && comp2 <= 57)) {
								//consume the next char
								id_build.Append ((char)check.Read ());
								add_count++;
							} else {
								//check if the id is a reserved word
								string res_check = id_build.ToString ();
								string lower_case = res_check.ToLower ();
								try {
									string stuff = reserved2 [lower_case];
									tokies.Add (new Token(stuff,lower_case,row_counter,column_counter));
									column_counter += add_count;
								} catch (KeyNotFoundException) {
									//reached the end of the id, return id token and exit the loop
									tokies.Add (new Token("MP_IDENTIFIER",lower_case.ToString(),row_counter,column_counter));
									column_counter += add_count;
									add_count = 0;
								}
								is_id = false;
							}
						}
					}

					//check if it begins with a decimal
					//if yes, then it is either a int, fixed, or float
					else if (comp >= 48 && comp <= 57) {
						//build a lexeme string while it's reading the chars
						StringBuilder lexeme = new StringBuilder ();
						lexeme.Append ((char)comp);

						//grab the rest of the number in a while loop
						//flags to check the type of number
						bool is_num = true;
						bool is_fixed = false;
						bool is_float = false;
						while (is_num) {
							//see if the next char is also part of the number
							int comp2 = check.Peek ();
							if (comp2 >= 48 && comp2 <= 57) {
								//append the number to the lexeme
								lexeme.Append ((char)check.Read ());
								add_count++;
							}
							//if it's a decimal point it could be a fixed or float
							else if (comp2 == 46) {
								if (is_fixed || is_float) {
									//it already contains a decimal, so it's an error
									tokies.Add (new Token("MP_ERROR",lexeme.ToString(),row_counter,column_counter));
									is_num = false;
									column_counter += add_count;
									add_count = 0;
								} else {
									//the number could be a fixed point or float
									is_fixed = true;
								}
								//append the decimal to the lexeme
								lexeme.Append ((char)check.Read ());

								//make sure the next char is a digit
								//otherwise it's an error
								int err_spot = check.Peek ();
								if (err_spot >= 48 && err_spot <= 57) {
									//the next char is a digit, do nothing
								} else {
									//not a digit, raise an error flag
									tokies.Add (new Token("MP_ERROR",lexeme.ToString(),row_counter,column_counter));
									is_num = false;
									column_counter += add_count;
									add_count = 0;
								}
							}
							//if it's an e or E, then it's a float
							else if (comp2 == 69 || comp2 == 101) {
								if (is_float) {
									//the number already contained an e or E
									//it's an error
									tokies.Add (new Token("MP_ERROR",lexeme.ToString(),row_counter,column_counter));
									is_num = false;
									column_counter += add_count;
									add_count = 0;
								} else {
									//mark the float flag
									is_float = true;
								}
								//append the e or E to the lexeme
								lexeme.Append ((char)check.Read ());

								//make sure the next char is a digit or minus sign
								int err_spot = check.Peek ();
								if (err_spot == 45) {
									//see if the minus sign is followed by a digit
									lexeme.Append ((char)check.Read ());
									add_count++;
									int err_spot2 = check.Peek ();
									if (err_spot2 >= 48 && err_spot2 <= 57) {
										//found a digit, all is well
									} else {
										//not a digit, return an error
										tokies.Add (new Token("MP_ERROR",lexeme.ToString(),row_counter,column_counter));
										is_num = false;
										column_counter += add_count;
										add_count = 0;
									}
								}
								//check if the E is followed by a digit
								else if (err_spot >= 48 && err_spot <= 57) {
									//found a digit, everything checks out
								} else {
									//no digit, no token for you
									tokies.Add (new Token("MP_ERROR",lexeme.ToString(),row_counter,column_counter));
									is_num = false;
									column_counter += add_count;
									add_count = 0;
								}
							} else {
								//reached the end of the number, check what kind of
								//number it is based on the flags

								if (is_float) {
									tokies.Add (new Token("MP_FLOAT_LIT",lexeme.ToString(),row_counter,column_counter));
									column_counter += add_count;
									add_count = 0;
								} else if (is_fixed) {
									tokies.Add (new Token("MP_FLOAT_LIT",lexeme.ToString(),row_counter,column_counter));
									column_counter += add_count;
									add_count = 0;
								} else {
									tokies.Add (new Token("MP_INTEGER_LIT",lexeme.ToString(),row_counter,column_counter));
									column_counter += add_count;
									add_count = 0;
								}
								//mark the next symbol as not part of the number
								//exit the loop
								is_num = false;
							}
						}

					} else if (comp == 46) {
						tokies.Add (new Token("MP_PERIOD",".",row_counter,column_counter));
						column_counter++;
					} else if (comp == 44) {
						tokies.Add (new Token("MP_COMMA",",",row_counter,column_counter));
						column_counter++;
					} else if (comp == 59) {
						tokies.Add (new Token("MP_SCOLON",";",row_counter,column_counter));
						column_counter++;
					} else if (comp == 40) {
						tokies.Add (new Token("MP_LPAREN","(",row_counter,column_counter));
						column_counter++;
					} else if (comp == 41) {
						tokies.Add (new Token("MP_RPAREN",")",row_counter,column_counter));
						column_counter++;
					} else if (comp == 61) {
						tokies.Add (new Token("MP_EQUAL","=",row_counter,column_counter));
						column_counter++;
					} else if (comp == 62) {
						int i62 = check.Peek ();
						if (i62 == 61) {
							int junk = check.Read ();
							column_counter++;
							tokies.Add (new Token("MP_GEQUAL",">=",row_counter,column_counter));
						} else {
							tokies.Add (new Token("MP_GTHAN",">",row_counter,column_counter));
						} 
					} else if (comp == 60) {
						int i60 = check.Peek ();
						if (i60 == 61) {
							int junk = check.Read ();
							column_counter++;
							tokies.Add (new Token("MP_LEQUAL","<=",row_counter,column_counter));
						} else if (i60 == 62) {
							int junk = check.Read ();
							column_counter++;
							tokies.Add (new Token("MP_NEQUAL","<>",row_counter,column_counter));
						} else {
							tokies.Add (new Token("MP_LTHAN","<",row_counter,column_counter));
						} 
					} else if (comp == 58) {
						int i58 = check.Peek ();
						if (i58 == 61) {
							int junk = check.Read ();
							tokies.Add (new Token("MP_ASSIGN",":=",row_counter,column_counter));
							column_counter += 2;
						} else {
							tokies.Add (new Token("MP_COLON",":",row_counter,column_counter));
							column_counter++;
						}
					} else if (comp == 43) {
						tokies.Add (new Token("MP_PLUS","+",row_counter,column_counter));
						column_counter++;
					} else if (comp == 47) { 
						tokies.Add (new Token("MP_DIVIDE","/",row_counter,column_counter));
						column_counter++;
					} else if (comp == 45) {
						tokies.Add (new Token("MP_MINUS","-",row_counter,column_counter));
						column_counter++;
					} else if (comp == 42) {
						tokies.Add (new Token("MP_TIMES","*",row_counter,column_counter));
						column_counter++;
					} else {
						tokies.Add (new Token("MP_ERROR",comp.ToString(),row_counter,column_counter));
						column_counter++;
					}

					white_space = Char.IsWhiteSpace ((char)check.Peek ());
					if (check.Peek () == -1 || check.Peek() == 123 || check.Peek() == 39) {
						white_space = true;
					}

				}
			}

			process = build.ToString ();
			return process;
		}

		public static ArrayList GetTokenArray(){
			return tokies;
		}

		public static void PrintTokies(){
			int i = 0;
			while (i < tokies.Count) {
				Token printTok = (Token)tokies [i];
				Console.Write (String.Format ("{0,-16}{1,-20}{2,-5}{3,-5}{4,0}", 
					printTok.GetName(), printTok.GetLex(), printTok.GetRow(), printTok.GetCol(), "\n"));
				i++;
			}
		}
	}
}