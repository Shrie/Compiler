// Symbol Table class keeps track of all of the
// records in a current scope

using System;
using System.Collections;
using System.Collections.Generic;

namespace Compilers
{
	public class SymbolTable
	{
		// Instance variables
		string name;
		int depth;
		int size;
		string label;
		ArrayList records;
		SymbolTable parent;
		Dictionary<string,int> lookUp;

		// Constructor

		public SymbolTable (string in_name, int in_depth, string in_label)
		{
			name = in_name;
			depth = in_depth;
			label = in_label;
			records = new ArrayList ();
			lookUp = new Dictionary<string,int>();
		}

		// Add a new record to the table

		public void AddRecord(TableRecord in_record){
			lookUp.Add (in_record.Lexeme(),size);
			in_record.SetOffset (size);
			size = in_record.Size () + size;
			records.Add (in_record);
			in_record.printRecord ();
		}

		// Getter method for size

		public int Size(){
			return size;
		}

		// Getter method for record offset

		public int GetOffset(string lex){
			try{
				return lookUp[lex];
			}catch(KeyNotFoundException){
				return -1;
			}
		}

		// Getter method for a record, given the offset

		public TableRecord GetRecord(int index){
			return (TableRecord)records [index];
		}

		// Setter method for parent table

		public void SetParent(SymbolTable in_parent){
			parent = in_parent;
		}

		// Getter method for parent table

		public SymbolTable GetParent(){
			return parent;
		}

		// Getter method for table depths

		public int GetDepth(){
			return depth;
		}
	}
}

