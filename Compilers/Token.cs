using System;

namespace Compilers
{
	public class Token
	{
		private string name;
		private string lex;
		private int column;
		private int row;


		public Token (string in_name,string in_lex,int in_row,int in_col)
		{
			name = in_name;
			lex = in_lex;
			row = in_row; 
			column = in_col;
		}

		public string GetName(){
			return name;
		}

		public string GetLex(){
			return lex;
		}

		public int GetRow(){
			return row;
		}

		public int GetCol(){
			return column;
		}
	}
}

