﻿// The Semantic Analyzer class accepts
// Semantic records as input from the Parser
// and generates working machine code that
// is functionally equivalent to the original
// microPascal input

using System;
using System.Collections;
using System.Text;

namespace Compilers
{

	public class SemanticAnalyzer
	{
		// instance variables
		private Parser2 parse;

		private SymbolTable tables;
		private int tableNum;
		private Stack operators;
		private Stack operands;
		private Stack operandType;
		private SType currentType;
		private StringBuilder prog;

		private string controlVar;

		private ArrayList errors;
		private bool semanticError;

		private bool negFlag;

		// Constructor

		public SemanticAnalyzer (Parser2 in_parse)
		{
			parse = in_parse;

			//tables = new SymbolTable ();
			prog = new StringBuilder ();
			tableNum = 0;

			operators = new Stack ();
			operands = new Stack ();
			operandType = new Stack ();

			semanticError = false;
			errors = new ArrayList ();

			negFlag = false;
		}

		// Add a table to the symbol table collection

		public void AddTable(SymbolTable in_table, int in_tabnum){
			tables = in_table;
			tableNum = in_tabnum;
		}

		// Allows passing of identifiers as operands

		public void PassID(string id){
			operands.Push (id);
		}

		// Allows passing of literals

		public void PassNum(string lex,string type){
			operands.Push (lex);
		}

		// Allows passing of operations

		public void PassOp(string op){
			operators.Push (op);
		}

		// This might be deprecated

		public void SetType(SType in_type){
			currentType = in_type;
		}

		// Tells us the next value is to be
		// negated

		public void SetNeg(){
			negFlag = true;
		}

		// Function call to build code for a
		// new symbol table
		// Called when entering a new scope

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

		// Function to make code for an assignment

		public void GenAssign(string target){

			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;

			// Search symbol tables for the identifier
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

			// If it's the control var, throw an error
			if (target == controlVar) {
				ErrorMessage ("Can't assign to the control variable.");
				semanticError = true;
			}

			if(cIndex == -1){
				// Didn't find the id in the symbol tables
				ErrorMessage ("Identifier " + parse.currentLexeme + 
					"doesn't exist in this scope.");
				semanticError = true;
			} else {
				// Found it, generate assign code
				string returnType = (string)operandType.Pop();
				string targetType = tables.GetRecord (cIndex).Type();

				// Correct type, make assign
				if (returnType == targetType) {
					prog.Append ("POP ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} 
				// Widening conversion, then assign
				else if (returnType == "int" && targetType == "float"){ 
					prog.Append ("CASTSF\n");
					prog.Append ("POP ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else {
					// Type mismatch
					ErrorMessage ("Incompatible assignment types.");
					semanticError = true;
				}
			}
		}

		// Create code for a Read statement

		public void GenRead(string target){
			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;

			// Search symbol tables for id
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

			// Couldn't find the id in the symbol tables
			if(cIndex == -1){
				ErrorMessage ("Identifier " + parse.currentLexeme + 
					"doesn't exist in this scope.");
				semanticError = true;
			} else {
				string targetType = tables.GetRecord (cIndex).Type();

				// Make a read int
				if (targetType == "int") {
					prog.Append ("RD ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} 
				// Make a read float
				else if (targetType == "float") {
					prog.Append ("RDF ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} 
				// Make a read string
				else if (targetType == "string") {
					prog.Append ("RDS ");
					prog.Append (cIndex);
					prog.Append ("(D");
					prog.Append (tableNum);
					prog.Append (")\n");
				} else {
					ErrorMessage ("incompatible read type.");
					semanticError = true;
				}
			}
		}

		// Create code for arithmetic

		public void GenArithmetic(){
			string type1 = (string)operandType.Pop ();
			string type2 = (string)operandType.Pop ();

			// Both are ints, no problem
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
					ErrorMessage ("Operator incompatible with types.");
					semanticError = true;
				}
			} 
			// Both are floats, no problem
			else if (type1 == "float" && type2 == "float") {
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
					ErrorMessage ("Mod only works on integers.");
					semanticError = true;
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
					ErrorMessage ("Operator incompatible with types.");
					semanticError = true;
				}
			} 
			// One int and one float, cast int to float
			else if (type1 == "int" && type2 == "float") {
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
					ErrorMessage ("Mod only works on integers.");
					semanticError = true;
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
					ErrorMessage ("Operator incompatible with types.");
					semanticError = true;
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
					ErrorMessage ("Mod only works on integers.");
					semanticError = true;
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
					ErrorMessage ("Operator incompatible with types.");
					semanticError = true;
				}
			} 
			// Both are boolean, only works for certain operations
			else if (type1 == "bool" && type2 == "bool") {
				string op = (string)operators.Pop ();

				if (op == "and") {
					prog.Append ("ANDS\n");
					operandType.Push ("bool");
				} else if (op == "or") {
					prog.Append ("ORS\n");
					operandType.Push ("bool");
				} else {
					ErrorMessage ("Operator incompatible with types.");
					semanticError = true;
				}
			} 
			// Type mismatch
			else {
				ErrorMessage ("Incompatible types.");
				semanticError = true;
			}
		}

		// Build code to push an identifier on the stack

		public void GenPushID(string lex){
			SymbolTable curr = tables;
			int tableActual = tableNum;
			int cIndex = -1;
			bool keepSearching = true;

			// Search the symbol tables for the id
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

			// Couldn't find it
			if (cIndex == -1) {
				ErrorMessage ("Identifier " + parse.currentLexeme + 
					"doesn't exist in this scope.");
				semanticError = true;
			} 

			// Did find it, push it
			else {
				TableRecord cRec = curr.GetRecord (cIndex);
				string cType = cRec.Type ();
				operandType.Push (cType);

				prog.Append ("PUSH ");
				prog.Append (cIndex);
				prog.Append ("(D");
				prog.Append (tableActual);
				prog.Append (")\n");


				if (!negFlag) {
					//do nothing
				}
				else if (negFlag && cType == "int") {
					prog.Append ("NEGS\n");
					negFlag = false;
				} else if (negFlag && cType == "float") {
					prog.Append ("NEGSF\n");
					negFlag = false;
				} else {
					semanticError = true;
					ErrorMessage ("Tried to negate an invalid type.");
					negFlag = false;
				}
			}
		}

		// Code for tearing down a scope

		public void GenTearDown(){
			SymbolTable curr = tables;

			prog.Append ("SUB SP #");
			prog.Append (curr.Size());
			prog.Append (" SP\n");
			prog.Append ("POP D");
			prog.Append (tableNum);
			prog.Append ("\n");
		}

		// Code that goes at the end of every program

		public void GenHalt (){
			prog.Append ("HLT\n");
		}

		// Code to write stack

		public void GenWrite(){
			prog.Append ("WRTS\n");
		}

		// Code to make a new line

		public void GenWriteLine(){
			prog.Append ("WRTLN #\"\"\n");
		}
			
		// Code to push a literal on the stack

		public void GenPushLit(string lex,string type){
			operandType.Push (type);

			prog.Append ("PUSH #");
			prog.Append (lex);
			prog.Append ("\n");

			if (!negFlag) {
				//do nothing
			}

			// Add it's type to the type stack
			else if (negFlag && type == "int") {
				prog.Append ("NEGS\n");
				negFlag = false;
			} else if (negFlag && type == "float") {
				prog.Append ("NEGSF\n");
				negFlag = false;
			} else {
				semanticError = true;
				ErrorMessage ("Tried to negate an invalid type.");
				negFlag = false;
			}
		}

		// Code for branching, used by ifs and loops

		public void GenBranchConditional(string in_label){

			string cond = (string)operandType.Pop ();
			if (cond == "bool") {
				prog.Append ("BRFS ");
				prog.Append (in_label);
				prog.Append ("\n");
			} else {
				ErrorMessage ("Expecting type boolean.");
				semanticError = true;
			}
		}

		// Code for conditional branches on true,
		// used by the repeat statement

		public void GenBranchConditionalT(string in_label){
			string cond = (string)operandType.Pop ();
			if (cond == "bool") {
				prog.Append ("BRTS ");
				prog.Append (in_label);
				prog.Append ("\n");
			} else {
				ErrorMessage ("Expecting type boolean.");
				semanticError = true;
			}
		}

		// Code for a new label

		public void GenLabel(string in_label){
			prog.Append (in_label);
			prog.Append (":\n");
		}

		// Code for an unconditional branch

		public void GenBranchUnconditional(string in_label){
			prog.Append ("BR ");
			prog.Append (in_label);
			prog.Append ("\n");
		}

		// Code for a for-loop, to check end condition

		public void GenCompareEqual(string step){
			string type1 = (string)operandType.Pop ();
			string type2 = (string)operandType.Pop ();

			if (step == "to") {
				if (type1 == "int" && type2 == "int") {
					prog.Append ("CMPGES\n");
					operandType.Push ("bool");
				}  else {
					ErrorMessage ("Control and initial " +
						"value must be integers.");
					semanticError = true;
				}
			} else {
				if (type1 == "int" && type2 == "int") {
					prog.Append ("CMPLES\n");
					operandType.Push ("bool");
				}  else {
					ErrorMessage ("Control and initial value must " +
						"be integers.");
					semanticError = true;
				}
			}
		}

		// Code to flip a boolean value

		public void GenNot(){
			string type = (string)operandType.Pop ();

			if (type == "bool") {
				prog.Append ("NOTS\n");
				operandType.Push ("bool");
			} else {
				ErrorMessage ("Performed NOT on a non-boolean.");
				semanticError = true;
			}
		}

		// Code to handle for-loop final value

		public void GenStackDup(){
			prog.Append ("PUSH -1(SP)\n");
		}

		// Code to remove for-loop final value

		public void GenStackSub(){
			prog.Append ("SUB SP #1 SP\n");
		}

		public void SetControlVariable(string in_var){
			controlVar = in_var;
		}

		public void ReleaseControlVariable(){
			controlVar = "null";
		}

		// Function to print out the code generated

		public void PrintCode(){

			// If there are errors, don't print
			if (semanticError) {
				Console.WriteLine ("There are semantic errors! Oh no!");
				StringBuilder bob = new StringBuilder ();
				int i = 0;
				while(i < errors.Count){
					bob.Append ((string)errors[i]);
					i++;
				}

				// Build an error report
				string errorString = bob.ToString ();
				System.IO.File.WriteAllText (@".\errorReport.txt",errorString);
			} 

			// no errors, print into a .il file
			else {
				string code = prog.ToString ();
				Console.Write (code);
				string[] codeLines = code.Split ("\n".ToCharArray(), 1000000);
				System.IO.File.WriteAllLines (@".\" + parse.fileName +".il",codeLines);
			}
		}

		//function to add error messages to output

		public void ErrorMessage (string errorType){

			//build an error string and add it to the error Array
			StringBuilder tom = new StringBuilder ();
			tom.Append ("Encountered a semantic error.\n");
			tom.Append (errorType);
			tom.Append (" at row: ");
			tom.Append (parse.curRow);
			tom.Append (", Col: ");
			tom.Append (parse.curCol);
			tom.Append ("\n");
			string errOut = tom.ToString ();
			errors.Add (errOut);
		}

		//function prints error messages

		public void ErrorPrint(){

			//for every error message in the array, print
			int i = 0;
			while(i < errors.Count){
				Console.Write ((string)errors[i]);
				i++;
			}
		}
	}
}

