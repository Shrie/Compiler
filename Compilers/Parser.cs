﻿using System;
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
		private StringReader tokens;

		public Parser (string tokens_in)
		{
			//initialize StringReader instance
			tokens = new StringReader (tokens_in);

			//initialize stack with <SystemGoal> and $
			//$ is the end of parse token
			rock.Push ("$");
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

		}


		//Parse function checks to see if the input stream of tokens is a valid program
		public void Parse(){
			string nextToken;
			string nextStackToken;
			Boolean continueParse = true;
			nextToken = Peek ();
			while(continueParse){

				nextStackToken = StackPeek ();

				if (nextStackToken == nextToken) {
					nextToken = Peek ();
					nextStackToken = StackPeek ();
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
						default:
							Console.WriteLine("What the hell did you do!?");
							break;
						}

					}catch(KeyNotFoundException){
						Console.WriteLine ("Syntax Error.");
					}
				}



				if (nextStackToken == "$") {
					continueParse = false;
				}
			}
		}

		//grab the next token from the token stream
		public string Peek(){
			//grab the next token, lexeme, column & row number
			string currentLine = tokens.ReadLine();
			string nextToken;
			StringBuilder token = new StringBuilder ();
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
			//Console.Write (nextToken);
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
			rock.Push ("MP_EOF");
			rock.Push ("<Program>");
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
			rock.Push ("MP_PROGRAM");
			rock.Push ("<ProgramIdentifier>");
		
		}

		void Rule4(){
			Console.WriteLine ("Rule 4 used");
			rock.Push ("<VariableDeclarationPart>");
			rock.Push ("<ProcedureAndFunctionDeclationPart>");
			rock.Push ("<StatementPart>");
		}

		void Rule5(){
			Console.WriteLine ("Rule 5 used");
			rock.Push ("MP_VAR");
			rock.Push ("<VariableDeclaration>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<VariableDeclarationTail>");		
		}

		void Rule6(){
			Console.WriteLine ("Rule 6 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule7(){
			Console.WriteLine ("Rule 7 used");
			rock.Push ("<VariableDeclaration>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<VariableDeclarationTail>");
		}

		void Rule8(){
			Console.WriteLine ("Rule 8 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule9(){
			Console.WriteLine ("Rule 9 used");
			rock.Push ("<IdentifierList>");
			rock.Push ("MP_COLON");
			rock.Push ("<Type>");
		}

		void Rule10(){
			Console.WriteLine ("Rule 10 used");
			rock.Push ("MP_INTEGER");
		}

		void Rule11(){
			Console.WriteLine ("Rule 11 used");
			rock.Push ("MP_FLOAT");
		}

		void Rule12(){
			Console.WriteLine ("Rule 12 used");
			rock.Push ("MP_STRING");
		}

		void Rule13(){
			Console.WriteLine ("Rule 13 used");
			rock.Push ("MP_BOOLEAN");
		}

		void Rule13(){
			Console.WriteLine ("Rule 13 used");
			rock.Push ("<ProcedureDeclaration>");
			rock.Push ("<ProcedureAndFunctionDeclarationPart>");
		}

		void Rule14(){
			Console.WriteLine ("Rule 14 used");
			rock.Push ("<FunctionDeclaration>");
			rock.Push ("<ProcedureAndFunctionDeclarationPart>");
		}

		void Rule15(){
			Console.WriteLine ("Rule 15 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule16(){
			Console.WriteLine ("Rule 16 used");
			rock.Push ("<ProcedureHeading>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
		}

		void Rule17(){
			Console.WriteLine ("Rule 17 used");
			rock.Push ("<FunctionHeading>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
		}

		void Rule18(){
			Console.WriteLine ("Rule 18 used");
			rock.Push ("<ProcedureHeading>");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Block>");
			rock.Push ("MP_SCOLON");
		}

		void Rule19(){
			Console.WriteLine ("Rule 19 used");
			rock.Push ("MP_PROCEDURE");
			rock.Push ("<ProcedureIdentifier");
			rock.Push ("<OptionalFormalParameterList>");
		}

		void Rule20(){
			Console.WriteLine ("Rule 20 used");
			rock.Push ("MP_FUNCTION");
			rock.Push ("<FunctionIdentifier>");
			rock.Push ("<OptionalFormalParameterList>");
			rock.Push ("MP_COLON");
			rock.Push ("<Type>");
		}

		void Rule21(){
			Console.WriteLine ("Rule 21 used");
			rock.Push ("MP_LPAREN");
			rock.Push ("<FormalParameterSection>");
			rock.Push ("<FormalParameterSectionTail>");
			rock.Push ("MP_RPAREN");
		}

		void Rule22(){
			Console.WriteLine ("Rule 22 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule23(){
			Console.WriteLine ("Rule 23 used");
			rock.Push ("MP_SCOLON");
			rock.Push ("<FormalParameterSection>");
			rock.Push ("<FormalParameterSectionTail>");
		}

		void Rule24(){
			Console.WriteLine ("Rule 24 used");
			rock.Push ("MP_EPSILON");
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
			rock.Push ("<IdentifierList>");
			rock.Push ("MP_COLON");
			rock.Push ("<Type>");
		}

		void Rule28(){
			Console.WriteLine ("Rule 28 used");
			rock.Push ("MP_VAR");
			rock.Push ("<IdentifierList>");
			rock.Push ("MP_COLON");
			rock.Push ("<Type>");
		}

		void Rule29(){
			Console.WriteLine ("Rule 29 used");
			rock.Push ("<CompoundStatement>");
		}

		void Rule30(){
			Console.WriteLine ("Rule 30 used");
			rock.Push ("MP_BEGIN");
			rock.Push ("<StatementSequence>");
			rock.Push ("MP_END");
		}

		void Rule31(){
			Console.WriteLine ("Rule 31 used");
			rock.Push ("<Statement>");
			rock.Push ("<StatementTail>");
		}

		void Rule32(){
			Console.WriteLine ("Rule 32 used");
			rock.Push ("MP_SCOLON");
			rock.Push ("<Statement>");
			rock.Push ("<StatementTail>");
		}

		void Rule33(){
			Console.WriteLine ("Rule 33 used");
			rock.Push ("MP_EPSILON");
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
			rock.Push ("<AssignmentStatement>");
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
			rock.Push ("<ProcedureStatement>");
		}

		void Rule45(){
			Console.WriteLine ("Rule 45 used");
			rock.Push ("MP_READ");
			rock.Push ("MP_LPAREN");
			rock.Push ("<ReadParameter>");
			rock.Push ("<ReadParameterTail>");
			rock.Push ("MP_RPAREN");
		}

		void Rule46(){
			Console.WriteLine ("Rule 46 used");
			rock.Push ("MP_COMMA");
			rock.Push ("<ReadParameter>");
			rock.Push ("<ReadParameterTail>");
		}

		void Rule47(){
			Console.WriteLine ("Rule 47 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule48(){
			Console.WriteLine ("Rule 48 used");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule49(){
			Console.WriteLine ("Rule 49 used");
			rock.Push ("MP_WRITE");
			rock.Push ("MP_LPAREN");
			rock.Push ("<WriteParameter>");
			rock.Push ("<WriteParameterTail>");
			rock.Push ("MP_RPAREN");
		}

		void Rule50(){
			Console.WriteLine ("Rule 50 used");
			rock.Push ("MP_WRITELN");
			rock.Push ("MP_LPAREN");
			rock.Push ("<WriteParameter>");
			rock.Push ("<WriteParameterTail>");
			rock.Push ("MP_RPAREN");
		}

		void Rule51(){
			Console.WriteLine ("Rule 51 used");
			rock.Push ("MP_COMMA");
			rock.Push ("<WriteParameter>");
			rock.Push ("<WriteParameterTail>");
		}

		void Rule52(){
			Console.WriteLine ("Rule 52 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule53(){
			Console.WriteLine ("Rule 53 used");
			rock.Push ("<OrdinalExpression>");
		}

		void Rule54(){
			Console.WriteLine ("Rule 54 used");
			rock.Push ("<VariableIdentifier>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<Expression>");
		}

		void Rule55(){
			Console.WriteLine ("Rule 55 used");
			rock.Push ("<FunctionIdentifier>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<Expression>");
		}

		void Rule56(){
			Console.WriteLine ("Rule 56 used");
			rock.Push ("MP_IF");
			rock.Push ("<BooleanExpression>");
			rock.Push ("MP_THEN");
			rock.Push ("<Statement>");
			rock.Push ("<OptionalElsePart>");
		}

		void Rule57(){
			Console.WriteLine ("Rule 57 used");
			rock.Push ("MP_ELSE");
			rock.Push ("<Statement>");
		}

		void Rule58(){
			Console.WriteLine ("Rule 58 used");
			rock.Push ("MP_EPSILON");
		}

		void Rule59(){
			Console.WriteLine ("Rule 59 used");
			rock.Push ("MP_REPEAT");
			rock.Push ("<StatmentSequence>");
			rock.Push ("MP_Until");
			rock.Push ("<BooleanExpression>");
		}

		void Rule60(){
			Console.WriteLine ("Rule 60 used");
			rock.Push ("MP_WHILE");
			rock.Push ("<BooleanExpression>");
			rock.Push ("MP_DO");
			rock.Push ("<Statement>");
		}

		void Rule61(){
			Console.WriteLine ("Rule 61 used");
			rock.Push ("MP_FOR");
			rock.Push ("<ControlVariable>");
			rock.Push ("MP_ASSIGN");
			rock.Push ("<InitialValue>");
			rock.Push ("<StepValue>");
			rock.Push ("<FinalValue>");
			rock.Push ("MP_DO");
			rock.Push ("<Statement>");
		}

		void Rule62(){
			Console.WriteLine ("Rule 62 used");
			rock.Push ("<VariableIdentifier>");
		}

		void Rule63(){
			Console.WriteLine ("Rule 63 used");
			rock.Push ("<OridnalExpression>");
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
			rock.Push ("<OridnalExpression>");
		}

		void Rule67(){
			Console.WriteLine ("Rule 67 used");
			rock.Push ("<ProcedureIdentifier>");
			rock.Push ("<OptionalActualParameterList>");
		}

		void Rule68(){
			Console.WriteLine ("Rule 68 used");
			rock.Push ("MP_LPAREN");
			rock.Push ("<ActualParameter>");
			rock.Push ("<ActualParameterTail>");
			rock.Push ("MP_RPAREN");
		}

		void Rule69(){
			Console.WriteLine ("Rule 69 used");
			rock.Push ("MP_EPSILON");
		}

		//voodoo

		void Rule70(){
			Console.WriteLine ("Rule 70 used");
			rock.Push ("MP_COMMA");
			rock.Push ("<ActualParameter>");
			rock.Push ("<ActualParameterTail>");
		}
	}
}

