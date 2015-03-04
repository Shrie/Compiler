using System;
using System.Collections.Generic;
using System.Collections;

namespace Compilers
{
	public class Parser
	{
		Dictionary<string, Dictionary<string,int>> lltable = new Dictionary<string, Dictionary<string,int>>();
		Stack rock = new Stack();

		public Parser ()
		{
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
			inner_dict.Add ("MP_VAR", 4);
			inner_dict.Add ("MP_EPSILON", 4);
			lltable.Add ("<Block>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_VAR", 5);
			inner_dict.Add ("MP_EPSILON", 6);
			lltable.Add ("<VariableDeclarationPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 7);
			inner_dict.Add ("MP_EPSILON", 8);
			lltable.Add ("<VariableDeclarationTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 9);
			lltable.Add ("<VariableDeclarationPart>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_INTEGER", 10);
			inner_dict.Add ("MP_FLOAT", 11);
			inner_dict.Add ("MP_STRING", 12);
			inner_dict.Add ("MP_BOOLEAN", 13);
			lltable.Add ("<Type>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PROCEDURE", 14);
			inner_dict.Add ("MP_FUNCTION", 15);
			inner_dict.Add ("MP_EPSILON", 16);
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
			inner_dict.Add ("MP_EPSILON", 22);
			lltable.Add ("<OptionalFormalParameterList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_SCOLON", 23);
			inner_dict.Add ("MP_EPSILON", 24);
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
			inner_dict.Add ("MP_FOR", 31);
			inner_dict.Add ("MP_EPSILON", 31);
			lltable.Add ("<StatementSequence>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_SCOLON", 32);
			inner_dict.Add ("MP_EPSILON", 33);
			lltable.Add ("<StatementTail>", inner_dict);

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
			inner_dict.Add ("MP_EPSILON", 34);
			lltable.Add ("<Statement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 54);
			inner_dict.Add ("MP_ASSIGN", 43);
			inner_dict.Add ("MP_EPSILON", 54);
			lltable.Add ("<AssignmentProcedureStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_EPSILON", 44);
			lltable.Add ("<EmptyStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_READ", 45);
			lltable.Add ("<ReadStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 46);
			inner_dict.Add ("MP_EPSILON", 47);
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
			inner_dict.Add ("MP_EPSILON", 52);
			lltable.Add ("<WriteParameterTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 53);
			inner_dict.Add ("MP_MINUS", 53);
			inner_dict.Add ("MP_EPSILON", 53);
			lltable.Add ("<WriteParameter>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IF", 56);
			lltable.Add ("<IfStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_ELSE", 57);
			inner_dict.Add ("MP_EPSILON", 58);
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
			inner_dict.Add ("MP_EPSILON", 63);
			lltable.Add ("<InitialValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_TO", 64);
			inner_dict.Add ("MP_DOWNTO", 65);
			lltable.Add ("<StepValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 66);
			inner_dict.Add ("MP_MINUS", 66);
			inner_dict.Add ("MP_EPSILON", 66);
			lltable.Add ("<FinalValue>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_EPLISON", 67);
			inner_dict.Add ("MP_EPSILON", 67);
			lltable.Add ("<ProcedureStatement>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_LPAREN", 68);
			inner_dict.Add ("MP_EPSILON", 69);
			lltable.Add ("<OptionalActualParameterList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 70);
			inner_dict.Add ("MP_EPSILON", 71);
			lltable.Add ("<ActualParameterTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 72);
			inner_dict.Add ("MP_MINUS", 72);
			inner_dict.Add ("MP_EPSILON", 72);
			lltable.Add ("<ActualParameter>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 73);
			inner_dict.Add ("MP_MINUS", 73);
			inner_dict.Add ("MP_EPSILON", 73);
			lltable.Add ("<Expression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_EQUAL", 74);
			inner_dict.Add ("MP_LTHAN", 74);
			inner_dict.Add ("MP_GTHAN", 74);
			inner_dict.Add ("MP_GEQUAL", 74);
			inner_dict.Add ("MP_LEQUAL", 74);
			inner_dict.Add ("MP_NEQUAL", 74);
			inner_dict.Add ("MP_EPSILON", 75);
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
			inner_dict.Add ("MP_EPSILON", 82);
			lltable.Add ("<SimpleExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 83);
			inner_dict.Add ("MP_MINUS", 83);
			inner_dict.Add ("MP_OR", 83);
			inner_dict.Add ("MP_EPSILON", 84);
			lltable.Add ("<TermTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 85);
			inner_dict.Add ("MP_MINUS", 86);
			inner_dict.Add ("MP_EPSILON", 87);
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
			inner_dict.Add ("MP_EPSILON", 93);
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
			inner_dict.Add ("MP_IDENTIFIER", 106);
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
			lltable.Add ("<ProcedureIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 109);
			inner_dict.Add ("MP_MINUS", 109);
			inner_dict.Add ("MP_EPSILON", 109);
			lltable.Add ("<BooleanExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_PLUS", 110);
			inner_dict.Add ("MP_MINUS", 110);
			inner_dict.Add ("MP_EPSILON", 110);
			lltable.Add ("<OrdinalExpression>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 111);
			lltable.Add ("<IdentifierList>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_COMMA", 112);
			inner_dict.Add ("MP_EPSILON", 113);
			lltable.Add ("<IdentifierTail>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 115);
			lltable.Add ("<FunctionIdentifier>", inner_dict);

			inner_dict = new Dictionary<string,int> ();
			inner_dict.Add ("MP_IDENTIFIER", 116);
			lltable.Add ("<VariableIdentifier>", inner_dict);



//			Dictionary<string,int> holyCow = lltable["<SystemGoal>"];
//			int printy = holyCow ["MP_PROGRAM"];
//			Console.WriteLine (printy);
		}

		public void Parse(string of_tokens){

		}

		public string Peek(){
			string nothing = "";
			return nothing;
		}

		public string StackPeek(){
			string nothing = "";
			return nothing;
		}



	}
}

