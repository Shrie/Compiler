using System;
using System.Collections;

namespace Compilers
{
	public class TableRecord
	{
		private string lexeme;
		private string type;
		private string kind;
		private string mode;
		private int size;
		private int offset;
		private ArrayList parameters;

		public TableRecord (string in_lex,string in_type,string in_kind,string in_mode,int in_size)
		{
			lexeme = in_lex;
			type = in_type;
			kind = in_kind;
			mode = in_mode;
			size = in_size;
			offset = 0;
			parameters = new ArrayList ();
		}

		public void addParam(Parameter in_param){
			//Parameter newParam = new Parameter (in_mode,in_type);
			parameters.Add (in_param);
		}

		public int Size(){
			return size;
		}

		public string Lexeme(){
			return lexeme;
		}

		public void SetOffset(int in_offset){
			offset = in_offset;
		}

		public string Type(){
			return type;
		}

		public void SetType(string in_type){
			type = in_type;
		}

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

