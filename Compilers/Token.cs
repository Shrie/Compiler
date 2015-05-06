// Token class defines the atrributes of a token
// for the compiler to use while parsing

using System;

namespace Compilers
{
	public class Token
	{
		// Instance variables
		private string name;
		private string lex;
		private int column;
		private int row;

		// Constructor

		public Token (string in_name,string in_lex,int in_row,int in_col)
		{
			name = in_name;
			lex = in_lex;
			row = in_row; 
			column = in_col;
		}

		// Getter method for name

		public string GetName(){
			return name;
		}

		// Getter method for lexeme

		public string GetLex(){
			return lex;
		}

		// Getter method for row

		public int GetRow(){
			return row;
		}

		// Getter method for column

		public int GetCol(){
			return column;
		}
	}
}

