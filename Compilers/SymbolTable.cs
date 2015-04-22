using System;
using System.Collections;
using System.Collections.Generic;

namespace Compilers
{
	public class SymbolTable
	{
		string name;
		int depth;
		int size;
		string label;
		ArrayList records;
		SymbolTable parent;
		Dictionary<string,int> lookUp;


		public SymbolTable (string in_name, int in_depth, string in_label)
		{
			name = in_name;
			depth = in_depth;
			label = in_label;
			records = new ArrayList ();
			lookUp = new Dictionary<string,int>();
		}

		public void AddRecord(TableRecord in_record){
			lookUp.Add (in_record.Lexeme(),size);
			in_record.SetOffset (size);
			size = in_record.Size () + size;
			records.Add (in_record);
			in_record.printRecord ();
		}

		public int Size(){
			return size;
		}

		public int GetOffset(string lex){
			try{
				return lookUp[lex];
			}catch(KeyNotFoundException){
				return -1;
			}
		}

		public TableRecord GetRecord(int index){
			return (TableRecord)records [index];
		}

		public void SetParent(SymbolTable in_parent){
			parent = in_parent;
		}

		public SymbolTable GetParent(){
			return parent;
		}

		public int GetDepth(){
			return depth;
		}
	}
}

