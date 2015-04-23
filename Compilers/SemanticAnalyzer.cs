using System;
using System.Collections;
using System.Text;

namespace Compilers
{

	public class SemanticAnalyzer
	{
		private SymbolTable tables;
		private int tableNum;
		private Stack operators;
		private Stack operands;
		private Stack operandType;
		private SType currentType;
		private StringBuilder prog;

		private ArrayList errors;
		private bool semanticError;

		public SemanticAnalyzer ()
		{
			//tables = new SymbolTable ();
			prog = new StringBuilder ();
			tableNum = 0;

			operators = new Stack ();
			operands = new Stack ();
			operandType = new Stack ();

			semanticError = false;
			errors = new ArrayList ();
		}

		public void AddTable(SymbolTable in_table, int in_tabnum){
			//tables.Add (in_table);
			tables = in_table;
			tableNum = in_tabnum;
		}

		public void PassID(string id){
			operands.Push (id);
		}

		public void PassNum(string lex,string type){
			operands.Push (lex);
		}

		public void PassOp(string op){
			operators.Push (op);
		}

		public void SetType(SType in_type){
			currentType = in_type;
		}

		public void GenTable(){
			//pushes a new symbol table on the stack
			//SymbolTable curr = (SymbolTable)tables[tableNum];
			SymbolTable curr = tables;

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

		public void GenAssign(string target){
			//SymbolTable curr = (SymbolTable)tables[tableNum];
			//int cIndex = curr.GetOffset (target);

			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;
			while (keepSearching) {
				cIndex = curr.GetOffset (target);
				if (cIndex != -1) {
					keepSearching = false;
				} else if (cIndex == -1 && curr.GetParent() != null) {
					curr = curr.GetParent ();
					tableActual = tableActual - 1;
				} else {
					keepSearching = false;
					semanticError = true;
				}
			}

			if(cIndex == -1){
				ErrorMessage ();
			} else {
				prog.Append ("POP ");
				prog.Append (cIndex);
				prog.Append ("(D");
				prog.Append (tableNum);
				prog.Append (")\n");
			}
		}

		public void GenRead(string target){
			//SymbolTable curr = (SymbolTable)tables[tableNum];
			//int cIndex = curr.GetOffset (target);

			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;
			while (keepSearching) {
				cIndex = curr.GetOffset (target);
				if (cIndex != -1) {
					keepSearching = false;
				} else if (cIndex == -1 && curr.GetParent() != null) {
					curr = curr.GetParent ();
					tableActual = tableActual - 1;
				} else {
					keepSearching = false;
					semanticError = true;
				}
			}

			if(cIndex == -1){
				ErrorMessage ();
			} else {
				prog.Append ("RD ");
				prog.Append (cIndex);
				prog.Append ("(D");
				prog.Append (tableNum);
				prog.Append (")\n");
			}
		}

		public void GenArithmetic(){
			string op = (string)operators.Pop ();
			if (op == "+") {
				prog.Append ("ADDS\n");
			} else if (op == "-") {
				prog.Append ("SUBS\n");
			} else if (op == "*") {
				prog.Append ("MULS\n");
			} else if (op == "/" || op == "div") {
				prog.Append ("DIVS\n");
			} else if (op == "mod") {
				prog.Append ("MODS\n");
			} else if (op == "=") {
				prog.Append ("CMPEQS\n");
			} else if (op == "<") {
				prog.Append ("CMPLTS\n");
			} else if (op == ">") {
				prog.Append ("CMPGTS\n");
			} else if (op == "<=") {
				prog.Append ("CMPLES\n");
			} else if (op == ">=") {
				prog.Append ("CMPGES\n");
			} else if (op == "<>") {
				prog.Append ("CMPNES\n");
			}
		}

		public void GenPushID(string lex){
			//SymbolTable curr = (SymbolTable)tables[tableNum];
			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;
			while (keepSearching) {
				cIndex = curr.GetOffset (lex);
				if (cIndex != -1) {
					keepSearching = false;
				} else if (cIndex == -1 && curr.GetParent() != null) {
					curr = curr.GetParent ();
					tableActual = tableActual - 1;
				} else {
					keepSearching = false;
					semanticError = true;
				}
			}

			if (cIndex == -1) {
				ErrorMessage ();
			} else {
				TableRecord cRec = curr.GetRecord (cIndex);
				string cType = cRec.Type ();
				operandType.Push (cType);

				prog.Append ("PUSH ");
				prog.Append (cIndex);
				prog.Append ("(D");
				prog.Append (tableActual);
				prog.Append (")\n");
			}
		}

		public void GenTearDown(){
			SymbolTable curr = tables;

			prog.Append ("SUB SP #");
			prog.Append (curr.Size());
			prog.Append (" SP\n");
			prog.Append ("POP D");
			prog.Append (tableNum);
			prog.Append ("\n");
		}

		public void GenHalt (){
			prog.Append ("HLT\n");
		}

		public void GenWrite(){
			prog.Append ("WRTS\n");
		}

		public void GenWriteLine(){
			prog.Append ("WRTLN \"\"\n");
		}
			

		public void GenPushLit(string lex,string type){
			operandType.Push (type);

			prog.Append ("PUSH #");
			prog.Append (lex);
			prog.Append ("\n");
		}

		public void PrintCode(){
			string code = prog.ToString ();
			Console.Write (code);
		}

		public void ErrorMessage(){

		}
	}
}

