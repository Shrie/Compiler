// Table Record class defines all of
// the attributes of a variable, function,
// or procedure in the program in a given
// scope.

using System;
using System.Collections;

namespace Compilers
{
	public class TableRecord
	{
		// Instance varibles
		private string lexeme;
		private string type;
		private string kind;
		private string mode;
		private int size;
		private int offset;
		private ArrayList parameters;

		// Constructor

		public TableRecord (string in_lex,string in_type,
			string in_kind,string in_mode,int in_size)
		{
			lexeme = in_lex;
			type = in_type;
			kind = in_kind;
			mode = in_mode;
			size = in_size;
			offset = 0;
			parameters = new ArrayList ();
		}

		// Add a parameter to functions and procedures

		public void addParam(Parameter in_param){
			//Parameter newParam = new Parameter (in_mode,in_type);
			parameters.Add (in_param);
		}

		// Getter method for size

		public int Size(){
			return size;
		}

		// Getter method for lexeme

		public string Lexeme(){
			return lexeme;
		}

		// Setter method for offset

		public void SetOffset(int in_offset){
			offset = in_offset;
		}

		// Getter method for type

		public string Type(){
			return type;
		}

		// Setter method for type

		public void SetType(string in_type){
			type = in_type;
		}

		// Print out a record

		public void printRecord(){
			Console.Write (lexeme);
			Console.Write (" ");
			Console.Write (type);
			Console.Write (" ");
			Console.Write (kind);
			Console.Write (" ");
			Console.Write (mode);
			Console.Write (" ");
			Console.Write (size);
			Console.Write (" ");
			Console.WriteLine (offset);

		}
	}
}

