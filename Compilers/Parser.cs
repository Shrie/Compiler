using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Compilers
{
	public class Parser
	{
		private Dictionary<string, Dictionary<string,int>> lltable = new Dictionary<string, Dictionary<string,int>>();
		private Stack rock = new Stack();
		private Stack tables = new Stack();
		private LabelMaker stamps = new LabelMaker ("L");

		private SymbolTable currentTable;
		private string tablename;
		private int depth;
		private string label;

		private TableRecord currentRecord;
		private string currentLexeme;
		private string currentKind;
		private string currentType;
		private string currentMode;
		private int currentSize;
		private Stack lexStack = new Stack();

		private StringReader tokens;

		private SemanticAnalyzer annie;

		public Parser (string tokens_in)
		{
			//initialize StringReader instance
			tokens = new StringReader (tokens_in);
			depth = 0;

			currentMode = "null";

			annie = new SemanticAnalyzer();

			//initialize stack with <SystemGoal> and MP_EOF
			//MP_EOF is the end of parse token
			rock.Push ("MP_EOF");
			rock.Push ("<SystemGoal>");

			//fill in the Dictionary using the LL(1) Table
			//will allow constant time look up for which rule to apply
			Dictionary<string, int> inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROGRAM", 1);
			lltable.Add ("<SystemGoal>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROGRAM", 2);
			lltable.Add ("<Program>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROGRAM", 3);
			lltable.Add ("<ProgramHeading>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FUNCTION",4);
			inner_dict.Add ("MP_PROCEDURE",4);
			inner_dict.Add ("MP_VAR", 4);
			inner_dict.Add ("MP_BEGIN", 4);
			lltable.Add ("<Block>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_VAR", 5);
			inner_dict.Add ("MP_FUNCTION",6);
			inner_dict.Add ("MP_PROCEDURE",6);
			inner_dict.Add ("MP_BEGIN", 6);
			lltable.Add ("<VariableDeclarationPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 7);
			inner_dict.Add ("MP_BEGIN", 8);
			inner_dict.Add ("MP_FUNCTION", 8);
			inner_dict.Add ("MP_PROCEDURE", 8);
			lltable.Add ("<VariableDeclarationTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 9);
			lltable.Add ("<VariableDeclaration>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_INTEGER", 10);
			inner_dict.Add ("MP_FLOAT", 11);
			inner_dict.Add ("MP_STRING", 12);
			inner_dict.Add ("MP_BOOLEAN", 13);
			lltable.Add ("<Type>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROCEDURE", 14);
			inner_dict.Add ("MP_FUNCTION", 15);
			inner_dict.Add ("MP_BEGIN", 16);
			lltable.Add ("<ProcedureAndFunctionDeclarationPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROCEDURE", 17);
			lltable.Add ("<ProcedureDeclaration>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FUNCTION", 18);
			lltable.Add ("<FunctionDeclaration>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROCEDURE", 19);
			lltable.Add ("<ProcedureHeading>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FUNCTION", 20);
			lltable.Add ("<FunctionHeading>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 21);
			inner_dict.Add ("MP_SCOLON", 22);
			inner_dict.Add ("MP_COLON", 22);
			lltable.Add ("<OptionalFormalParameterList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_SCOLON", 23);
			inner_dict.Add ("MP_RPAREN", 24);
			lltable.Add ("<FormalParameterSectionTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_VAR", 26);
			inner_dict.Add ("MP_IDENTIFER", 25);
			lltable.Add ("<FormalParameterSection>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFER", 27);
			lltable.Add ("<ValueParameterSection>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_VAR", 28);
			lltable.Add ("<VariableParameterSection>", inner_dict);


			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_BEGIN", 29);
			lltable.Add ("<StatementPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_BEGIN", 30);
			lltable.Add ("<CompoundStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_BEGIN", 31);
			inner_dict.Add ("MP_IDENTIFIER", 31);
			inner_dict.Add ("MP_READ", 31);
			inner_dict.Add ("MP_WRITE", 31);
			inner_dict.Add ("MP_WRITELN", 31);
			inner_dict.Add ("MP_IF", 31);
			inner_dict.Add ("MP_REPEAT", 31);
			inner_dict.Add ("MP_WHILE", 31);
			inner_dict.Add ("MP_FOR", 31);;
			lltable.Add ("<StatementSequence>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_SCOLON", 32);
			inner_dict.Add ("MP_END", 33);
			inner_dict.Add ("MP_UNTIL",33);
			lltable.Add ("<StatementTail>", inner_dict);

			//add in functionality for procedure statement
			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_BEGIN", 35);
			inner_dict.Add ("MP_IDENTIFIER", 38);
			inner_dict.Add ("MP_READ", 36);
			inner_dict.Add ("MP_WRITE", 37);
			inner_dict.Add ("MP_WRITELN", 37);
			inner_dict.Add ("MP_IF", 39);
			inner_dict.Add ("MP_REPEAT", 41);
			inner_dict.Add ("MP_WHILE", 40);
			inner_dict.Add ("MP_FOR", 42);
			inner_dict.Add ("MP_ELSE", 34);
			inner_dict.Add ("MP_END", 34);
			inner_dict.Add ("MP_UNTIL", 34);
			inner_dict.Add ("MP_SCOLON", 34);
			inner_dict.Add ("MP_PROCEDURE", 43);
			lltable.Add ("<Statement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_ELSE", 44);
			inner_dict.Add ("MP_END", 44);
			inner_dict.Add ("MP_UNTIL", 44);
			inner_dict.Add ("MP_SCOLON", 44);
			lltable.Add ("<EmptyStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_READ", 45);
			lltable.Add ("<ReadStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 46);
			inner_dict.Add ("MP_RPAREN", 47);
			lltable.Add ("<ReadParameterTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 48);
			lltable.Add ("<ReadParameter>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_WRITE", 49);
			inner_dict.Add ("MP_WRITELN", 50);
			lltable.Add ("<WriteStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 51);
			inner_dict.Add ("MP_RPAREN", 52);
			lltable.Add ("<WriteParameterTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 53);
			inner_dict.Add ("MP_MINUS", 53);
			inner_dict.Add ("MP_FALSE", 53);
			inner_dict.Add ("MP_NOT", 53);
			inner_dict.Add ("MP_TRUE", 53);
			inner_dict.Add ("MP_INTEGER_LIT", 53);
			inner_dict.Add ("MP_STRING_LIT", 53);
			inner_dict.Add ("MP_FLOAT_LIT", 53);
			inner_dict.Add ("MP_LPAREN", 53);
			lltable.Add ("<WriteParameter>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 54);
			lltable.Add ("<AssignmentStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IF", 56);
			lltable.Add ("<IfStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_ELSE", 57);
			inner_dict.Add ("MP_END", 58);
			inner_dict.Add ("MP_SCOLON", 58);
			inner_dict.Add ("MP_UNTIL", 58);
			lltable.Add ("<OptionalElsePart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_REPEAT", 59);
			lltable.Add ("<RepeatStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_WHILE", 60);
			lltable.Add ("<WhileStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FOR", 61);
			lltable.Add ("<ForStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 62);
			lltable.Add ("<ControlVariable>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 63);
			inner_dict.Add ("MP_MINUS", 63);
			inner_dict.Add ("MP_FALSE", 63);
			inner_dict.Add ("MP_NOT", 63);
			inner_dict.Add ("MP_TRUE", 63);
			inner_dict.Add ("MP_IDENTIFIER", 63);
			inner_dict.Add ("MP_INTEGER_LIT", 63);
			inner_dict.Add ("MP_FLOAT_LIT", 63);
			inner_dict.Add ("MP_STRING_LIT", 63);
			inner_dict.Add ("MP_LPAREN", 63);
			lltable.Add ("<InitialValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_TO", 64);
			inner_dict.Add ("MP_DOWNTO", 65);
			lltable.Add ("<StepValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 66);
			inner_dict.Add ("MP_MINUS", 66);
			inner_dict.Add ("MP_FALSE", 66);
			inner_dict.Add ("MP_NOT", 66);
			inner_dict.Add ("MP_TRUE", 66);
			inner_dict.Add ("MP_IDENTIFIER", 66);
			inner_dict.Add ("MP_INTEGER_LIT", 66);
			inner_dict.Add ("MP_FLOAT_LIT", 66);
			inner_dict.Add ("MP_STRING_LIT", 66);
			inner_dict.Add ("MP_LPAREN", 66);
			lltable.Add ("<FinalValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 67);
			lltable.Add ("<ProcedureStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 68);
			inner_dict.Add ("MP_ELSE", 69);
			inner_dict.Add ("MP_END", 69);
			inner_dict.Add ("MP_UNTIL", 69);
			inner_dict.Add ("MP_SCOLON", 69);
			lltable.Add ("<OptionalActualParameterList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 70);
			inner_dict.Add ("MP_RPAREN", 71);
			lltable.Add ("<ActualParameterTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 72);
			inner_dict.Add ("MP_MINUS", 72);
			inner_dict.Add ("MP_FALSE", 72);
			inner_dict.Add ("MP_NOT", 72);
			inner_dict.Add ("MP_TRUE", 72);
			inner_dict.Add ("MP_IDENTIFIER", 72);
			inner_dict.Add ("MP_INTEGER_LIT", 72);
			inner_dict.Add ("MP_FLOAT_LIT", 72);
			inner_dict.Add ("MP_STRING_LIT", 72);
			inner_dict.Add ("MP_LPAREN", 72);
			lltable.Add ("<ActualParameter>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 73);
			inner_dict.Add ("MP_MINUS", 73);
			inner_dict.Add ("MP_FALSE", 73);
			inner_dict.Add ("MP_NOT", 73);
			inner_dict.Add ("MP_TRUE", 73);
			inner_dict.Add ("MP_IDENTIFIER", 73);
			inner_dict.Add ("MP_INTEGER_LIT", 73);
			inner_dict.Add ("MP_FLOAT_LIT", 73);
			inner_dict.Add ("MP_STRING_LIT", 73);
			inner_dict.Add ("MP_LPAREN", 73);
			lltable.Add ("<Expression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_EQUAL", 74);
			inner_dict.Add ("MP_LTHAN", 74);
			inner_dict.Add ("MP_GTHAN", 74);
			inner_dict.Add ("MP_GEQUAL", 74);
			inner_dict.Add ("MP_LEQUAL", 74);
			inner_dict.Add ("MP_NEQUAL", 74);
			inner_dict.Add ("MP_DO", 75);
			inner_dict.Add ("MP_DOWNTO", 75);
			inner_dict.Add ("MP_ELSE", 75);
			inner_dict.Add ("MP_END", 75);
			inner_dict.Add ("MP_THEN", 75);
			inner_dict.Add ("MP_TO", 75);
			inner_dict.Add ("MP_UNTIL", 75);
			inner_dict.Add ("MP_COMMA", 75);
			inner_dict.Add ("MP_SCOLON", 75);
			inner_dict.Add ("MP_LPAREN", 75);
			inner_dict.Add ("MP_RPAREN", 75);
			lltable.Add ("<OptionalRelationalPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_EQUAL", 76);
			inner_dict.Add ("MP_LTHAN", 77);
			inner_dict.Add ("MP_GTHAN", 78);
			inner_dict.Add ("MP_GEQUAL", 79);
			inner_dict.Add ("MP_LEQUAL", 80);
			inner_dict.Add ("MP_NEQUAL", 81);
			lltable.Add ("<RelationalOperator>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 82);
			inner_dict.Add ("MP_MINUS", 82);
			inner_dict.Add ("MP_FALSE", 82);
			inner_dict.Add ("MP_NOT", 82);
			inner_dict.Add ("MP_TRUE", 82);
			inner_dict.Add ("MP_IDENTIFIER", 82);
			inner_dict.Add ("MP_INTEGER_LIT", 82);
			inner_dict.Add ("MP_FLOAT_LIT", 82);
			inner_dict.Add ("MP_STRING_LIT", 82);
			inner_dict.Add ("MP_LPAREN", 82);
			lltable.Add ("<SimpleExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 83);
			inner_dict.Add ("MP_MINUS", 83);
			inner_dict.Add ("MP_OR", 83);
			inner_dict.Add ("MP_DO", 84);
			inner_dict.Add ("MP_DOWNTO", 84);
			inner_dict.Add ("MP_ELSE", 84);
			inner_dict.Add ("MP_END", 84);
			inner_dict.Add ("MP_THEN", 84);
			inner_dict.Add ("MP_TO", 84);
			inner_dict.Add ("MP_UNTIL", 84);
			inner_dict.Add ("MP_COMMA", 84);
			inner_dict.Add ("MP_SCOLON", 84);
			inner_dict.Add ("MP_RPAREN", 84);
			inner_dict.Add ("MP_EQUAL", 84);
			inner_dict.Add ("MP_LEQUAL", 84);
			inner_dict.Add ("MP_GEQUAL", 84);
			inner_dict.Add ("MP_LTHAN", 84);
			inner_dict.Add ("MP_GTHAN", 84);
			inner_dict.Add ("MP_NEQUAL", 84);
			lltable.Add ("<TermTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 85);
			inner_dict.Add ("MP_MINUS", 86);
			inner_dict.Add ("MP_FALSE", 87);
			inner_dict.Add ("MP_NOT", 87);
			inner_dict.Add ("MP_TRUE", 87);
			inner_dict.Add ("MP_IDENTIFIER", 87);
			inner_dict.Add ("MP_INTEGER_LIT", 87);
			inner_dict.Add ("MP_STRING_LIT", 87);
			inner_dict.Add ("MP_FLOAT_LIT", 87);
			inner_dict.Add ("MP_LPAREN", 87);
			lltable.Add ("<OptionalSign>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 88);
			inner_dict.Add ("MP_MINUS", 89);
			inner_dict.Add ("MP_OR", 90);
			lltable.Add ("<AddingOperator>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 91);
			inner_dict.Add ("MP_IDENTIFIER", 91);
			inner_dict.Add ("MP_INTEGER_LIT", 91);
			inner_dict.Add ("MP_FLOAT_LIT", 91);
			inner_dict.Add ("MP_STRING_LIT", 91);
			inner_dict.Add ("MP_TRUE", 91);
			inner_dict.Add ("MP_FALSE", 91);
			inner_dict.Add ("MP_NOT", 91);
			lltable.Add ("<Term>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_TIMES", 92);
			inner_dict.Add ("MP_DIVIDE", 92);
			inner_dict.Add ("MP_DIV", 92);
			inner_dict.Add ("MP_MOD", 92);
			inner_dict.Add ("MP_AND", 92);
			inner_dict.Add ("MP_DO", 93);
			inner_dict.Add ("MP_DOWNTO", 93);
			inner_dict.Add ("MP_ELSE", 93);
			inner_dict.Add ("MP_END", 93);
			inner_dict.Add ("MP_OR", 93);
			inner_dict.Add ("MP_THEN", 93);
			inner_dict.Add ("MP_TO", 93);
			inner_dict.Add ("MP_UNTIL", 93);
			inner_dict.Add ("MP_COMMA", 93);
			inner_dict.Add ("MP_SCOLON", 93);
			inner_dict.Add ("MP_RPAREN", 93);
			inner_dict.Add ("MP_EQUAL", 93);
			inner_dict.Add ("MP_LEQUAL", 93);
			inner_dict.Add ("MP_GEQUAL", 93);
			inner_dict.Add ("MP_LTHAN", 93);
			inner_dict.Add ("MP_GTHAN", 93);
			inner_dict.Add ("MP_NEQUAL", 93);
			inner_dict.Add ("MP_PLUS", 93);
			inner_dict.Add ("MP_MINUS", 93);
			lltable.Add ("<FactorTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_TIMES", 94);
			inner_dict.Add ("MP_DIVIDE", 95);
			inner_dict.Add ("MP_DIV", 96);
			inner_dict.Add ("MP_MOD", 97);
			inner_dict.Add ("MP_AND", 98);
			lltable.Add ("<MultiplyingOperator>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 105);
			inner_dict.Add ("MP_IDENTIFIER", 116);
			inner_dict.Add ("MP_INTEGER_LIT", 99);
			inner_dict.Add ("MP_FLOAT_LIT", 100);
			inner_dict.Add ("MP_STRING_LIT", 101);
			inner_dict.Add ("MP_TRUE", 102);
			inner_dict.Add ("MP_FALSE", 103);
			inner_dict.Add ("MP_NOT", 104);
			lltable.Add ("<Factor>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 107);
			lltable.Add ("<ProgramIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 108);
			lltable.Add ("<VariableIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 109);
			lltable.Add ("<ProcedureIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 110);
			lltable.Add ("<FunctionIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FALSE", 111);
			inner_dict.Add ("MP_NOT", 111);
			inner_dict.Add ("MP_TRUE", 111);
			inner_dict.Add ("MP_IDENTIFIER", 111);
			inner_dict.Add ("MP_INTEGER_LIT", 111);
			inner_dict.Add ("MP_STRING_LIT", 111);
			inner_dict.Add ("MP_FLOAT_LIT", 111);
			inner_dict.Add ("MP_LPAREN", 111);
			inner_dict.Add ("MP_PLUS", 111);
			inner_dict.Add ("MP_MINUS", 111);
			lltable.Add ("<BooleanExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_FALSE", 112);
			inner_dict.Add ("MP_NOT", 112);
			inner_dict.Add ("MP_TRUE", 112);
			inner_dict.Add ("MP_IDENTIFIER", 112);
			inner_dict.Add ("MP_INTEGER_LIT", 112);
			inner_dict.Add ("MP_STRING_LIT", 112);
			inner_dict.Add ("MP_FLOAT_LIT", 112);
			inner_dict.Add ("MP_LPAREN", 112);
			inner_dict.Add ("MP_PLUS", 112);
			inner_dict.Add ("MP_MINUS", 112);
			lltable.Add ("<OrdinalExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 113);
			lltable.Add ("<IdentifierList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 114);
			inner_dict.Add ("MP_COLON", 115);
			lltable.Add ("<IdentifierTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 117);
			lltable.Add ("<AssignProcedureStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_ASSIGN", 118);
			inner_dict.Add ("MP_LPAREN", 119);
			inner_dict.Add ("MP_ELSE", 119);
			inner_dict.Add ("MP_END", 119);
			inner_dict.Add ("MP_UNTIL", 119);
			inner_dict.Add ("MP_SCOLON", 119);
			lltable.Add ("<AssignProcedureTail>", inner_dict);
		}


		//Parse function checks to see if the input stream of tokens is a valid program
		public void Parse(){
			string nextToken;
			string nextStackToken;
			Boolean continueParse = true;
			nextToken = Peek ();
			while(continueParse){

				nextStackToken = StackPeek ();
				if (nextToken == "MP_EOF" || nextStackToken == "MP_EOF") {
					continueParse = false;
				} else if (nextStackToken == nextToken) {
					//Console.WriteLine (nextToken);
					//Console.WriteLine (nextStackToken);
					nextToken = Peek ();
					//nextStackToken = StackPeek ();
				} else {
					try{
						Dictionary<string, int> inside = lltable[nextStackToken];
						int rule = inside[nextToken];

						//use a rule function based on what int (rule number) was recieved from the Dictionary
						switch(rule){
						case 1:
							Rule1();
							break;
						case 2:
							Rule2();
							break;
						case 3:
							Rule3();
							break;
						case 4:
							Rule4();
							break;
						case 5:
							Rule5();
							break;
						case 6:
							Rule6();
							break;
						case 7:
							Rule7();
							break;
						case 8:
							Rule8();
							break;
						case 9:
							Rule9();
							break;
						case 10:
							Rule10();
							break;
						case 11:
							Rule11();
							break;
						case 12:
							Rule12();
							break;
						case 13:
							Rule13();
							break;
						case 14:
							Rule14();
							break;
						case 15:
							Rule15();
							break;
						case 16:
							Rule16();
							break;
						case 17:
							Rule17();
							break;
						case 18:
							Rule18();
							break;
						case 19:
							Rule19();
							break;
						case 20:
							Rule20();
							break;
						case 21:
							Rule21();
							break;
						case 22:
							Rule22();
							break;
						case 23:
							Rule23();
							break;
						case 24:
							Rule24();
							break;
						case 25:
							Rule25();
							break;
						case 26:
							Rule26();
							break;
						case 27:
							Rule27();
							break;
						case 28:
							Rule28();
							break;
						case 29:
							Rule29();
							break;
						case 30:
							Rule30();
							break;
						case 31:
							Rule31();
							break;
						case 32:
							Rule32();
							break;
						case 33:
							Rule33();
							break;
						case 34:
							Rule34();
							break;
						case 35:
							Rule35();
							break;
						case 36:
							Rule36();
							break;
						case 37:
							Rule37();
							break;
						case 38:
							Rule38();
							break;
						case 39:
							Rule39();
							break;
						case 40:
							Rule40();
							break;
						case 41:
							Rule41();
							break;
						case 42:
							Rule42();
							break;
						case 43:
							Rule43();
							break;
						case 44:
							Rule44();
							break;
						case 45:
							Rule45();
							break;
						case 46:
							Rule46();
							break;
						case 47:
							Rule47();
							break;
						case 48:
							Rule48();
							break;
						case 49:
							Rule49();
							break;
						case 50:
							Rule50();
							break;
						case 51:
							Rule51();
							break;
						case 52:
							Rule52();
							break;
						case 53:
							Rule53();
							break;
						case 54:
							Rule54();
							break;
						case 55:
							Rule55();
							break;
						case 56:
							Rule56();
							break;
						case 57:
							Rule57();
							break;
						case 58:
							Rule58();
							break;
						case 59:
							Rule59();
							break;
						case 60:
							Rule60();
							break;
						case 61:
							Rule61();
							break;
						case 62:
							Rule62();
							break;
						case 63:
							Rule63();
							break;
						case 64:
							Rule64();
							break;
						case 65:
							Rule65();
							break;
						case 66:
							Rule66();
							break;
						case 67:
							Rule67();
							break;
						case 68:
							Rule68();
							break;
						case 69:
							Rule69();
							break;
						case 70:
							Rule70();
							break;
						case 71:
							Rule71();
							break;
						case 72:
							Rule72();
							break;
						case 73:
							Rule73();
							break;
						case 74:
							Rule74();
							break;
						case 75:
							Rule75();
							break;
						case 76:
							Rule76();
							break;
						case 77:
							Rule77();
							break;
						case 78:
							Rule78();
							break;
						case 79:
							Rule79();
							break;
						case 80:
							Rule80();
							break;
						case 81:
							Rule81();
							break;
						case 82:
							Rule82();
							break;
						case 83:
							Rule83();
							break;
						case 84:
							Rule84();
							break;
						case 85:
							Rule85();
							break;
						case 86:
							Rule86();
							break;
						case 87:
							Rule87();
							break;
						case 88:
							Rule88();
							break;
						case 89:
							Rule89();
							break;
						case 90:
							Rule90();
							break;
						case 91:
							Rule91();
							break;
						case 92:
							Rule92();
							break;
						case 93:
							Rule93();
							break;
						case 94:
							Rule94();
							break;
						case 95:
							Rule95();
							break;
						case 96:
							Rule96();
							break;
						case 97:
							Rule97();
							break;
						case 98:
							Rule98();
							break;
						case 99:
							Rule99();
							break;
						case 100:
							Rule100();
							break;
						case 101:
							Rule101();
							break;
						case 102:
							Rule102();
							break;
						case 103:
							Rule103();
							break;
						case 104:
							Rule104();
							break;
						case 105:
							Rule105();
							break;
						case 106:
							Rule106();
							break;
						case 107:
							Rule107();
							break;
						case 108:
							Rule108();
							break;
						case 109:
							Rule109();
							break;
						case 110:
							Rule110();
							break;
						case 111:
							Rule111();
							break;
						case 112:
							Rule112();
							break;
						case 113:
							Rule113();
							break;
						case 114:
							Rule114();
							break;
						case 115:
							Rule115();
							break;
						case 116:
							Rule116();
							break;
						case 117:
							Rule117();
							break;
						case 118:
							Rule118();
							break;
						case 119:
							Rule119();
							break;
						}

					}catch(KeyNotFoundException){
						Console.Write ("Syntax Error.");
						Console.WriteLine (nextStackToken);
						Console.WriteLine (nextToken);
					}
				}
			}
		}

		//grab the next token from the token stream
		public string Peek(){
			//grab the next token, lexeme, column & row number
			string currentLine = tokens.ReadLine();
			string nextToken;
			StringBuilder token = new StringBuilder ();
			StringBuilder lexeme = new StringBuilder ();
			//grab just the token from the front of the line
			using (StringReader grabToken = new StringReader(currentLine)){
				Boolean isToken = true;
				while (isToken) {
					if (Char.IsWhiteSpace ((char)grabToken.Peek ())) {
						isToken = false;
					} else {
						token.Append ((char)grabToken.Read());
					}
				}
			}
			//dump the token in a string and return it
			nextToken = token.ToString ();

			//eat up whitespace


			//grab the lexeme and put it in the currentLexeme string
			using (StringReader grabLexeme = new StringReader(currentLine)){
				Boolean isToken = true;
				Boolean isLex = true;
				while (isLex) {
					if (Char.IsWhiteSpace ((char)grabLexeme.Peek ())) {
						if (isToken) {
							isToken = false;
							//remove the whitespace between token and lexeme
							while(Char.IsWhiteSpace ((char)grabLexeme.Peek ())){
								grabLexeme.Read ();
							}
						} else {
							isLex = false;
						}
					} else {
						if (isToken) {
							//ignore the token
							grabLexeme.Read ();
						} else {
							//append to the lexeme
							lexeme.Append ((char)grabLexeme.Read());
						}
					}
				}
			}
			currentLexeme = lexeme.ToString ();
			Console.Write (nextToken);
			Console.Write (" ");
			Console.WriteLine (currentLexeme);
			return nextToken;
		}

		//peek at the top symbol on the stack
		public string StackPeek(){
			//pop the top symbol off the stack and return it.
			string nothing = (string)rock.Pop();
			//Console.Write (nothing);
			return nothing;
		}


		//The rules from the grammar, just push the right hand side
		//of the grammar rule onto the stack from right to left.
		void Rule1(){
			Console.WriteLine ("Rule 1 used");
			//rock.Push ("MP_EOF");
			rock.Push ("<Program>");

			annie.SetType (SType.Table);
		}

		void Rule2(){
			Console.WriteLine ("Rule 2 used");
			rock.Push ("MP_PERIOD");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<ProgramHeading>");
		}

		void Rule3(){
			Console.WriteLine ("Rule 3 used");
			rock.Push ("<ProgramIdentifier>");
			rock.Push ("MP_PROGRAM");
		}

		void Rule4(){
			Console.WriteLine ("Rule 4 used");
			rock.Push ("<StatementPart>");
			rock.Push ("<ProcedureAndFunctionDeclarationPart>");
			rock.Push ("<VariableDeclarationPart>");
		}

		void Rule5(){
			//set up to record variables
			Console.WriteLine ("Rule 5 used");
			rock.Push ("<VariableDeclarationTail>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<VariableDeclaration>");
			rock.Push ("MP_VAR");

			currentKind = "var";
		}

		void Rule6(){
			Console.WriteLine ("Rule 6 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule7(){
			Console.WriteLine ("Rule 7 used");
			rock.Push ("<VariableDeclarationTail>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<VariableDeclaration>");
		}

		void Rule8(){
			Console.WriteLine ("Rule 8 used");
			//rock.Push ("MP_EPSILON");

			annie.AddTable (currentTable);
		}

		void Rule9(){
			Console.WriteLine ("Rule 9 used");
			rock.Push ("<Type>");
			rock.Push ("MP_COLON");
			rock.Push ("<IdentifierList>");
		}

		void Rule10(){
			Console.WriteLine ("Rule 10 used");
			rock.Push ("MP_INTEGER");

			currentType = "integer";
			currentSize = 1;
			while(lexStack.Count != 0){
				string nextLex = lexStack.Pop ().ToString();
				currentRecord = new TableRecord (nextLex, currentType, currentKind, currentMode, currentSize);
				currentTable.AddRecord (currentRecord);
			}
		}

		void Rule11(){
			Console.WriteLine ("Rule 11 used");
			rock.Push ("MP_FLOAT");

			currentType = "float";
			currentSize = 1;
			while(lexStack.Count != 0){
				string nextLex = lexStack.Pop ().ToString();
				currentRecord = new TableRecord (nextLex, currentType, currentKind, currentMode, currentSize);
				currentTable.AddRecord (currentRecord);
			}
		}

		void Rule12(){
			Console.WriteLine ("Rule 12 used");
			rock.Push ("MP_STRING");

			currentType = "string";
			currentSize = 1;
			while(lexStack.Count != 0){
				string nextLex = lexStack.Pop ().ToString();
				currentRecord = new TableRecord (nextLex, currentType, currentKind, currentMode, currentSize);
				currentTable.AddRecord (currentRecord);
			}
		}

		void Rule13(){
			Console.WriteLine ("Rule 13 used");
			rock.Push ("MP_BOOLEAN");

			currentType = "boolean";
			currentSize = 1;
			while(lexStack.Count != 0){
				string nextLex = lexStack.Pop ().ToString();
				currentRecord = new TableRecord (nextLex, currentType, currentKind, currentMode, currentSize);
				currentTable.AddRecord (currentRecord);
			}
		}

		void Rule14(){
			Console.WriteLine ("Rule 14 used");
			rock.Push ("<ProcedureAndFunctionDeclarationPart>");
			rock.Push ("<ProcedureDeclaration>");
		}

		void Rule15(){
			Console.WriteLine ("Rule 15 used");
			rock.Push ("<ProcedureAndFunctionDeclarationPart>");
			rock.Push ("<FunctionDeclaration>");
		}

		void Rule16(){
			Console.WriteLine ("Rule 16 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule17(){
			Console.WriteLine ("Rule 17 used");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<ProcedureHeading>");
		}

		void Rule18(){
			Console.WriteLine ("Rule 18 used");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<FunctionHeading>");
		}

		void Rule19(){
			Console.WriteLine ("Rule 19 used");
			rock.Push ("<OptionalFormalParameterList>");
			rock.Push ("<ProcedureIdentifier");
			rock.Push ("MP_PROCEDURE");
		}

		void Rule20(){
			Console.WriteLine ("Rule 20 used");
			rock.Push ("<Type>");
			rock.Push ("MP_COLON");
			rock.Push ("<OptionalFormalParameterList>");
			rock.Push ("<FunctionIdentifier>");
			rock.Push ("MP_FUNCTION");
		}

		void Rule21(){
			Console.WriteLine ("Rule 21 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<FormalParameterSectionTail>");
			rock.Push ("<FormalParameterSection>");
			rock.Push ("MP_LPAREN");
		}

		void Rule22(){
			Console.WriteLine ("Rule 22 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule23(){
			Console.WriteLine ("Rule 23 used");
			rock.Push ("<FormalParameterSectionTail>");
			rock.Push ("<FormalParameterSection>");
			rock.Push ("MP_SCOLON");
		}

		void Rule24(){
			Console.WriteLine ("Rule 24 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule25(){
			Console.WriteLine ("Rule 25 used");
			rock.Push ("<ValueParameterSerction>");
		}

		void Rule26(){
			Console.WriteLine ("Rule 26 used");
			rock.Push ("<VariableParameterSerction>");
		}

		void Rule27(){
			Console.WriteLine ("Rule 27 used");
			rock.Push ("<Type>");
			rock.Push ("MP_COLON");
			rock.Push ("<IdentifierList>");
		}

		void Rule28(){
			Console.WriteLine ("Rule 28 used");
			rock.Push ("<Type>");
			rock.Push ("MP_COLON");
			rock.Push ("<IdentifierList>");
			rock.Push ("MP_VAR");
		}

		void Rule29(){
			Console.WriteLine ("Rule 29 used");
			rock.Push ("<CompoundStatement>");
		}

		void Rule30(){
			Console.WriteLine ("Rule 30 used");
			rock.Push ("MP_END");
			rock.Push ("<StatementSequence>");
			rock.Push ("MP_BEGIN");
		}

		void Rule31(){
			Console.WriteLine ("Rule 31 used");
			rock.Push ("<StatementTail>");
			rock.Push ("<Statement>");
		}

		void Rule32(){
			Console.WriteLine ("Rule 32 used");
			rock.Push ("<StatementTail>");
			rock.Push ("<Statement>");
			rock.Push ("MP_SCOLON");
		}

		void Rule33(){
			Console.WriteLine ("Rule 33 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule34(){
			Console.WriteLine ("Rule 34 used");
			rock.Push ("<EmptyStatement>");
		}

		void Rule35(){
			Console.WriteLine ("Rule 35 used");
			rock.Push ("<CompoundStatement>");
		}

		void Rule36(){
			Console.WriteLine ("Rule 36 used");
			rock.Push ("<ReadStatement>");
		}

		void Rule37(){
			Console.WriteLine ("Rule 37 used");
			rock.Push ("<WriteStatement>");
		}

		void Rule38(){
			Console.WriteLine ("Rule 38 used");
			rock.Push ("<AssignProcedureStatement>");
		}

		void Rule39(){
			Console.WriteLine ("Rule 39 used");
			rock.Push ("<IfStatement>");
		}

		void Rule40(){
			Console.WriteLine ("Rule 40 used");
			rock.Push ("<WhileStatement>");
		}

		void Rule41(){
			Console.WriteLine ("Rule 41 used");
			rock.Push ("<RepeatStatement>");
		}

		void Rule42(){
			Console.WriteLine ("Rule 42 used");
			rock.Push ("<ForStatement>");
		}

		void Rule43(){
			Console.WriteLine ("Rule 43 used");
			rock.Push ("<ForStatement>");
		}

		void Rule44(){
			Console.WriteLine ("Rule 44 used");
		}

		void Rule45(){
			Console.WriteLine ("Rule 45 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<ReadParameterTail>");
			rock.Push ("<ReadParameter>");
			rock.Push ("MP_LPAREN");
			rock.Push ("MP_READ");
		}

		void Rule46(){
			Console.WriteLine ("Rule 46 used");
			rock.Push ("<ReadParameterTail>");
			rock.Push ("<ReadParameter>");
			rock.Push ("MP_COMMA");
		}

		void Rule47(){
			Console.WriteLine ("Rule 47 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule48(){
			Console.WriteLine ("Rule 48 used");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule49(){
			Console.WriteLine ("Rule 49 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<WriteParameterTail>");
			rock.Push ("<WriteParameter>");
			rock.Push ("MP_LPAREN");
			rock.Push ("MP_WRITE");
		}

		void Rule50(){
			Console.WriteLine ("Rule 50 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<WriteParameterTail>");
			rock.Push ("<WriteParameter>");
			rock.Push ("MP_LPAREN");
			rock.Push ("MP_WRITELN");
		}

		void Rule51(){
			Console.WriteLine ("Rule 51 used");
			rock.Push ("<WriteParameterTail>");
			rock.Push ("<WriteParameter>");
			rock.Push ("MP_COMMA");
		}

		void Rule52(){
			Console.WriteLine ("Rule 52 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule53(){
			Console.WriteLine ("Rule 53 used");
			rock.Push ("<OrdinalExpression>");
		}

		void Rule54(){
			Console.WriteLine ("Rule 54 used");
			rock.Push ("<Expression>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule55(){
			Console.WriteLine ("Rule 55 used");
			rock.Push ("<Expression>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<FunctionIdentifier>");
		}

		void Rule56(){
			Console.WriteLine ("Rule 56 used");
			rock.Push ("<OptionalElsePart>");
			rock.Push ("<Statement>");
			rock.Push ("MP_THEN");
			rock.Push ("<BooleanExpression>");
			rock.Push ("MP_IF");
		}

		void Rule57(){
			Console.WriteLine ("Rule 57 used");
			rock.Push ("<Statement>");
			rock.Push ("MP_ELSE");
		}

		void Rule58(){
			Console.WriteLine ("Rule 58 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule59(){
			Console.WriteLine ("Rule 59 used");
			rock.Push ("<BooleanExpression>");
			rock.Push ("MP_Until");
			rock.Push ("<StatementSequence>");
			rock.Push ("MP_REPEAT");
		}

		void Rule60(){
			Console.WriteLine ("Rule 60 used");
			rock.Push ("<Statement>");
			rock.Push ("MP_DO");
			rock.Push ("<BooleanExpression>");
			rock.Push ("MP_WHILE");
		}

		void Rule61(){
			Console.WriteLine ("Rule 61 used");
			rock.Push ("<Statement>");
			rock.Push ("MP_DO");
			rock.Push ("<FinalValue>");
			rock.Push ("<StepValue>");
			rock.Push ("<InitialValue>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<ControlVariable>");
			rock.Push ("MP_FOR");
		}

		void Rule62(){
			Console.WriteLine ("Rule 62 used");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule63(){
			Console.WriteLine ("Rule 63 used");
			rock.Push ("<OrdinalExpression>");
		}

		void Rule64(){
			Console.WriteLine ("Rule 64 used");
			rock.Push ("MP_TO");
		}

		void Rule65(){
			Console.WriteLine ("Rule 65 used");
			rock.Push ("MP_DOWNTO");
		}

		void Rule66(){
			Console.WriteLine ("Rule 66 used");
			rock.Push ("<OrdinalExpression>");
		}

		void Rule67(){
			Console.WriteLine ("Rule 67 used");
			rock.Push ("<OptionalActualParameterList>");
			rock.Push ("<ProcedureIdentifier>");
		}

		void Rule68(){
			Console.WriteLine ("Rule 68 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<ActualParameterTail>");
			rock.Push ("<ActualParameter>");
			rock.Push ("MP_LPAREN");
		}

		void Rule69(){
			Console.WriteLine ("Rule 69 used");
			//rock.Push ("MP_EPSILON");
		}

		//voodoo

		void Rule70(){
			Console.WriteLine ("Rule 70 used");
			rock.Push ("<ActualParameterTail>");
			rock.Push ("<ActualParameter>");
			rock.Push ("MP_COMMA");
		}

		void Rule71(){
			Console.WriteLine ("Rule 71 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule72(){
			Console.WriteLine ("Rule 72 used");
			rock.Push ("<OrdinalExpression>");
		}

		void Rule73(){
			Console.WriteLine ("Rule 73 used");
			rock.Push ("<OptionalRelationalPart>");
			rock.Push ("<SimpleExpression>");
		}

		void Rule74(){
			Console.WriteLine ("Rule 74 used");
			rock.Push ("<SimpleExpression>");
			rock.Push ("<RelationalOperator>");
		}

		void Rule75(){
			Console.WriteLine ("Rule 75 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule76(){
			Console.WriteLine ("Rule 76 used");
			rock.Push ("MP_EQUAL");
		}

		void Rule77(){
			Console.WriteLine ("Rule 77 used");
			rock.Push ("MP_LTHAN");
		}

		void Rule78(){
			Console.WriteLine ("Rule 78 used");
			rock.Push ("MP_GTHAN");
		}

		void Rule79(){
			Console.WriteLine ("Rule 79 used");
			rock.Push ("MP_LEQUAL");
		}

		void Rule80(){
			Console.WriteLine ("Rule 80 used");
			rock.Push ("MP_GEQUAL");
		}

		void Rule81(){
			Console.WriteLine ("Rule 81 used");
			rock.Push ("MP_NEQUAL");
		}

		void Rule82(){
			Console.WriteLine ("Rule 82 used");
			rock.Push ("<TermTail>");
			rock.Push ("<Term>");
			rock.Push ("<OptionalSign>");
		}

		void Rule83(){
			Console.WriteLine ("Rule 83 used");
			rock.Push ("<TermTail>");
			rock.Push ("<Term>");
			rock.Push ("<AddingOperator>");
		}

		void Rule84(){
			Console.WriteLine ("Rule 84 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule85(){
			Console.WriteLine ("Rule 85 used");
			rock.Push ("MP_PLUS");
		}

		void Rule86(){
			Console.WriteLine ("Rule 86 used");
			rock.Push ("MP_MINUS");
		}

		void Rule87(){
			Console.WriteLine ("Rule 87 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule88(){
			Console.WriteLine ("Rule 88 used");
			rock.Push ("MP_PLUS");
		}

		void Rule89(){
			Console.WriteLine ("Rule 89 used");
			rock.Push ("MP_MINUS");
		}

		void Rule90(){
			Console.WriteLine ("Rule 90 used");
			rock.Push ("MP_OR");
		}

		void Rule91(){
			Console.WriteLine ("Rule 91 used");
			rock.Push ("<FactorTail>");
			rock.Push ("<Factor>");
		}

		void Rule92(){
			Console.WriteLine ("Rule 92 used");
			rock.Push ("<FactorTail>");
			rock.Push ("<Factor>");
			rock.Push ("<MultiplyingOperator>");
		}

		void Rule93(){
			Console.WriteLine ("Rule 93 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule94(){
			Console.WriteLine ("Rule 94 used");
			rock.Push ("MP_TIMES");
		}

		void Rule95(){
			Console.WriteLine ("Rule 95 used");
			rock.Push ("MP_DIVIDE");
		}

		void Rule96(){
			Console.WriteLine ("Rule 96 used");
			rock.Push ("MP_DIV");
		}

		void Rule97(){
			Console.WriteLine ("Rule 97 used");
			rock.Push ("MP_MOD");
		}

		void Rule98(){
			Console.WriteLine ("Rule 98 used");
			rock.Push ("MP_AND");
		}

		void Rule99(){
			Console.WriteLine ("Rule 99 used");
			rock.Push ("MP_INTEGER_LIT");
		}

		void Rule100(){
			Console.WriteLine ("Rule 100 used");
			rock.Push ("MP_FLOAT_LIT");
		}

		void Rule101(){
			Console.WriteLine ("Rule 101 used");
			rock.Push ("MP_STRING_LIT");
		}

		void Rule102(){
			Console.WriteLine ("Rule 102 used");
			rock.Push ("MP_TRUE");
		}

		void Rule103(){
			Console.WriteLine ("Rule 103 used");
			rock.Push ("MP_FALSE");
		}

		void Rule104(){
			Console.WriteLine ("Rule 104 used");
			rock.Push ("<Factor>");
			rock.Push ("MP_NOT");
		}

		void Rule105(){
			Console.WriteLine ("Rule 105 used");
			rock.Push ("MP_RPAREN");
			rock.Push ("<Expression>");
			rock.Push ("MP_LPAREN");
		}

		void Rule106(){
			Console.WriteLine ("Rule 106 used");
			rock.Push ("<OptionalActualParameterList>");
			rock.Push ("<FunctionIdentifier>");
		}

		void Rule107(){
			//enter in a program level symbol table
			Console.WriteLine ("Rule 107 used");
			rock.Push ("MP_IDENTIFIER");

			tablename = currentLexeme;
			label = stamps.MakeLabel ();
			//Console.WriteLine (label);
			currentTable = new SymbolTable (tablename, depth, label);

			depth++;
		}

		void Rule108(){
			Console.WriteLine ("Rule 108 used");
			rock.Push ("MP_IDENTIFIER");

			lexStack.Push (currentLexeme);
		}

		void Rule109(){
			Console.WriteLine ("Rule 109 used");
			rock.Push ("MP_IDENTIFIER");
		}

		void Rule110(){
			Console.WriteLine ("Rule 110 used");
			rock.Push ("MP_IDENTIFIER");
		}

		void Rule111(){
			Console.WriteLine ("Rule 111 used");
			rock.Push ("<Expression>");
		}

		void Rule112(){
			Console.WriteLine ("Rule 112 used");
			rock.Push ("<Expression>");
		}

		void Rule113(){
			Console.WriteLine ("Rule 113 used");
			rock.Push ("<IdentifierTail>");
			//rock.Push ("MP_IDENTIFIER");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule114(){
			Console.WriteLine ("Rule 114 used");
			rock.Push ("<IdentifierTail>");
			//rock.Push ("MP_IDENTIFIER");
			rock.Push ("<VariableIdentifier>");
			rock.Push ("MP_COMMA");
		}

		void Rule115(){
			Console.WriteLine ("Rule 115 used");
			//rock.Push ("MP_EPSILON");
		}

		void Rule116(){
			Console.WriteLine ("Rule 116 used");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule117(){
			Console.WriteLine ("Rule 117 used");
			rock.Push ("<AssignProcedureTail>");
			rock.Push ("MP_IDENTIFIER");
		}

		void Rule118(){
			Console.WriteLine ("Rule 118 used");
			rock.Push ("<Expression>");
			rock.Push ("MP_ASSIGN");
		}

		void Rule119(){
			Console.WriteLine ("Rule 119 used");
			rock.Push ("<OptionalActualParameterList>");
		}
	}
}

