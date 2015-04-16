using System;
using System.Collections;
using System.Text;

namespace Compilers
{

	public class SemanticAnalyzer
	{
		private ArrayList tables;
		private int tableNum;
		private Stack operators;
		private Stack ids;
		private SType currentType;
		private StringBuilder prog;

		private ArrayList D0;
		private ArrayList D1;
		private ArrayList D2;
		private ArrayList D3;
		private ArrayList D4;
		private ArrayList D5;
		private ArrayList D6;
		private ArrayList D7;
		private ArrayList D8;
		private ArrayList D9;


		public SemanticAnalyzer ()
		{
			tables = new ArrayList ();
			prog = new StringBuilder ();
			tableNum = 0;
		}

		public void AddTable(SymbolTable in_table){
			tables.Add (in_table);
		}

		public void PassID(string id){
			ids.Push (id);
		}

		public void PassOp(string op){
			operators.Push (op);
		}

		public void SetType(SType in_type){
			currentType = in_type;
		}

		public void Analyze(){
			//pushes a new symbol table on the stack
			if (currentType == 0) {
				SymbolTable curr = (SymbolTable)tables[tableNum];

				prog.Append ("PUSH D");
				prog.Append (tableNum);
				prog.Append ("\n");
				prog.Append ("MOV SP D");
				prog.Append (tableNum);
				prog.Append ("\n");
				prog.Append ("ADD SP #");
				prog.Append (curr.Size());
				prog.Append (" SP\n");
			}


		}
	}
}

