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
		private Stack operands;
		private Stack operandType;
		private SType currentType;
		private StringBuilder prog;

		//private Stack D0;
		//private Stack D1;
		//private Stack D2;
		//private Stack D3;
		//private Stack D4;
		//private Stack D5;
		//private Stack D6;
		//private Stack D7;
		//private Stack D8;
		//private Stack D9;


		public SemanticAnalyzer ()
		{
			tables = new ArrayList ();
			prog = new StringBuilder ();
			tableNum = 0;

			operators = new Stack ();
			operands = new Stack ();
			operandType = new Stack ();
		}

		public void AddTable(SymbolTable in_table){
			tables.Add (in_table);
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

		public void GenAssign(string target){
			SymbolTable curr = (SymbolTable)tables[tableNum];
			int cIndex = curr.GetOffset (target);

			prog.Append ("POP ");
			prog.Append (cIndex);
			prog.Append ("(D");
			prog.Append (tableNum);
			prog.Append (")\n");
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
			SymbolTable curr = (SymbolTable)tables[tableNum];
			int cIndex = curr.GetOffset (lex);
			TableRecord cRec = curr.GetRecord (cIndex);
			string cType = cRec.Type ();
			operandType.Push (cType);

			prog.Append ("PUSH ");
			prog.Append (cIndex);
			prog.Append ("(D");
			prog.Append (tableNum);
			prog.Append (")\n");
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
	}
}

