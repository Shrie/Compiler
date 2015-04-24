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
				string returnType = (string)operandType.Pop();
				string targetType = tables.GetRecord (cIndex).Type();
				if (returnType == targetType) {
					prog.Append ("POP ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else {
					ErrorMessage ();
				}
			}
		}

		public void GenRead(string target){

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
				string targetType = tables.GetRecord (cIndex).Type();
				if (targetType == "int") {
					prog.Append ("RD ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else if (targetType == "float") {
					prog.Append ("RDF ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else if (targetType == "string") {
					prog.Append ("RDS ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else {
					ErrorMessage ();
				}
			}
		}

		public void GenArithmetic(){
			string type1 = (string)operandType.Pop ();
			string type2 = (string)operandType.Pop ();


			if (type1 == "int" && type2 == "int") {
				string op = (string)operators.Pop ();
				if (op == "+") {
					prog.Append ("ADDS\n");
					operandType.Push ("int");
				} else if (op == "-") {
					prog.Append ("SUBS\n");
					operandType.Push ("int");
				} else if (op == "*") {
					prog.Append ("MULS\n");
					operandType.Push ("int");
				} else if (op == "/" || op == "div") {
					prog.Append ("DIVS\n");
					operandType.Push ("int");
				} else if (op == "mod") {
					prog.Append ("MODS\n");
					operandType.Push ("int");
				} else if (op == "=") {
					prog.Append ("CMPEQS\n");
					operandType.Push ("bool");
				} else if (op == "<") {
					prog.Append ("CMPLTS\n");
					operandType.Push ("bool");
				} else if (op == ">") {
					prog.Append ("CMPGTS\n");
					operandType.Push ("bool");
				} else if (op == "<=") {
					prog.Append ("CMPLES\n");
					operandType.Push ("bool");
				} else if (op == ">=") {
					prog.Append ("CMPGES\n");
					operandType.Push ("bool");
				} else if (op == "<>") {
					prog.Append ("CMPNES\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ();
				}
			} else if (type1 == "float" && type2 == "float") {
				string op = (string)operators.Pop ();
				if (op == "+") {
					prog.Append ("ADDSF\n");
					operandType.Push ("float");
				} else if (op == "-") {
					prog.Append ("SUBSF\n");
					operandType.Push ("float");
				} else if (op == "*") {
					prog.Append ("MULSF\n");
					operandType.Push ("float");
				} else if (op == "/" || op == "div") {
					prog.Append ("DIVSF\n");
					operandType.Push ("float");
				} else if (op == "mod") {
					//prog.Append ("MODS\n");
					ErrorMessage ();
				} else if (op == "=") {
					prog.Append ("CMPEQSF\n");
					operandType.Push ("bool");
				} else if (op == "<") {
					prog.Append ("CMPLTSF\n");
					operandType.Push ("bool");
				} else if (op == ">") {
					prog.Append ("CMPGTSF\n");
					operandType.Push ("bool");
				} else if (op == "<=") {
					prog.Append ("CMPLESF\n");
					operandType.Push ("bool");
				} else if (op == ">=") {
					prog.Append ("CMPGESF\n");
					operandType.Push ("bool");
				} else if (op == "<>") {
					prog.Append ("CMPNESF\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ();
				}
			} else if (type1 == "int" && type2 == "float") {
				prog.Append ("CASTSF\n");

				string op = (string)operators.Pop ();
				if (op == "+") {
					prog.Append ("ADDSF\n");
					operandType.Push ("float");
				} else if (op == "-") {
					prog.Append ("SUBSF\n");
					operandType.Push ("float");
				} else if (op == "*") {
					prog.Append ("MULSF\n");
					operandType.Push ("float");
				} else if (op == "/" || op == "div") {
					prog.Append ("DIVSF\n");
					operandType.Push ("float");
				} else if (op == "mod") {
					//prog.Append ("MODS\n");
					ErrorMessage ();
				} else if (op == "=") {
					prog.Append ("CMPEQSF\n");
					operandType.Push ("bool");
				} else if (op == "<") {
					prog.Append ("CMPLTSF\n");
					operandType.Push ("bool");
				} else if (op == ">") {
					prog.Append ("CMPGTSF\n");
					operandType.Push ("bool");
				} else if (op == "<=") {
					prog.Append ("CMPLESF\n");
					operandType.Push ("bool");
				} else if (op == ">=") {
					prog.Append ("CMPGESF\n");
					operandType.Push ("bool");
				} else if (op == "<>") {
					prog.Append ("CMPNESF\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ();
				}
			} else if (type1 == "float" && type2 == "int") {
				prog.Append ("POP 0(SP)\n");
				prog.Append ("CASTSF\n");
				prog.Append ("PUSH 0(SP)\n");

				string op = (string)operators.Pop ();
				if (op == "+") {
					prog.Append ("ADDSF\n");
					operandType.Push ("float");
				} else if (op == "-") {
					prog.Append ("SUBSF\n");
					operandType.Push ("float");
				} else if (op == "*") {
					prog.Append ("MULSF\n");
					operandType.Push ("float");
				} else if (op == "/" || op == "div") {
					prog.Append ("DIVSF\n");
					operandType.Push ("float");
				} else if (op == "mod") {
					//prog.Append ("MODS\n");
					ErrorMessage ();
				} else if (op == "=") {
					prog.Append ("CMPEQSF\n");
					operandType.Push ("bool");
				} else if (op == "<") {
					prog.Append ("CMPLTSF\n");
					operandType.Push ("bool");
				} else if (op == ">") {
					prog.Append ("CMPGTSF\n");
					operandType.Push ("bool");
				} else if (op == "<=") {
					prog.Append ("CMPLESF\n");
					operandType.Push ("bool");
				} else if (op == ">=") {
					prog.Append ("CMPGESF\n");
					operandType.Push ("bool");
				} else if (op == "<>") {
					prog.Append ("CMPNESF\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ();
				}
			} else if (type1 == "bool" && type2 == "bool") {
				string op = (string)operators.Pop ();

				if (op == "and") {
					prog.Append ("ANDS\n");
					operandType.Push ("bool");
				} else if (op == "or") {
					prog.Append ("ORS\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ();
				}
			} else {
				ErrorMessage ();
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
			prog.Append ("WRTLN #\"\"\n");
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

