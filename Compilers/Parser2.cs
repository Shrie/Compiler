using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Compilers
{
	public class Parser2
	{
		private LabelMaker labelMe;

		private SymbolTable currentTable;
		private string tablename;
		private int depth;
		private string label;

		private TableRecord cRecord;
		private string rLex;
		private string rType;
		private string rKind;
		private string rMode;
		private int rSize;
		private int rOffset;
		private Stack rLexStack;

		private StringReader tokens;
		private string currentLexeme;
		private string ct;
		private int curCol;
		private int curRow;
		private ArrayList tokes;
		private int tokePoint;

		private SemanticAnalyzer annie;
		private bool assignFlag;

		public Parser2 (string tokens_in, ArrayList tokes_in)
		{
			tokens = new StringReader (tokens_in);
			tokes = tokes_in;
			tokePoint = 0;

			annie = new SemanticAnalyzer ();
			assignFlag = false;

			depth = 0;

			labelMe = new LabelMaker ("L");

			rLexStack = new Stack ();
			rMode = "null";
			rOffset = 0;
		}

		public void Peek(){
			if (tokePoint < tokes.Count) {
				ct = ((Token)tokes[tokePoint]).GetName();
				currentLexeme = ((Token)tokes[tokePoint]).GetLex();
				curCol = ((Token)tokes[tokePoint]).GetCol();
				curRow = ((Token)tokes[tokePoint]).GetRow();

				Console.Write ("Looking at token: ");
				Console.Write (ct);
				Console.Write (" ");
				Console.WriteLine (currentLexeme);

				tokePoint++;

				if(ct == "MP_COMMENT"){
					Peek ();
				}


			} else {
				Console.WriteLine ("Out of tokens.");
			}
		}

		public void Parse(){
			//Peek to grab the first token
			Peek ();

			//start with the SystemGoal rule 1
			if (ct == "MP_PROGRAM") {
				Program ();
			} else {
				Console.Write ("Syntax error on row: ");
				Console.Write (curRow);
				Console.Write ("  column: ");
				Console.Write (curCol);
				Console.Write ("  Expected keyword 'program', got ");
				Console.WriteLine (currentLexeme);
			}

			if (ct == "MP_EOF"){
				Console.WriteLine ("Program parsed successfully!");
				annie.PrintCode ();
			}
		}

		public void Program(){
			if (ct == "MP_PROGRAM") {
				//Rule 2
				ProgramHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					//error check
					Console.Write ("Syntax error on row: ");
					Console.Write (curRow);
					Console.Write ("  column: ");
					Console.Write (curCol);
					Console.Write ("  Expected ';', got ");
					Console.WriteLine (currentLexeme);
				}

				Block ();

				if (ct == "MP_PERIOD") {
					Peek ();
				} else {
					//error check
					Console.Write ("Syntax error on row: ");
					Console.Write (curRow);
					Console.Write ("  column: ");
					Console.Write (curCol);
					Console.Write ("  Expected '.', got ");
					Console.WriteLine (currentLexeme);
				}
			} else {
				//error check
				Console.Write ("Syntax error on row: ");
				Console.Write (curRow);
				Console.Write ("  column: ");
				Console.Write (curCol);
				Console.Write ("  Expected keyword 'program', got ");
				Console.WriteLine (currentLexeme);
			}
		}

		public void ProgramHeading(){
			if (ct == "MP_PROGRAM") {
				//rule 3
				if (ct == "MP_PROGRAM") {
					Peek ();
				}
				//set new table name and label
				tablename = currentLexeme;
				label = labelMe.MakeLabel ();
				currentTable = new SymbolTable (tablename, depth, label);
				depth++;

				ProgramIdentifier ();
			} else {
				//error check
				Console.Write ("Syntax error on row: ");
				Console.Write (curRow);
				Console.Write ("  column: ");
				Console.Write (curCol);
				Console.Write ("  Expected keyword'program', got ");
				Console.WriteLine (currentLexeme);
			}
		}

		public void Block(){
			if(ct == "MP_BEGIN"||ct=="MP_FUNCTION"||ct=="MP_PROCEDURE"||ct=="MP_VAR"){
				//rule 4
				VariableDeclarationPart ();

				ProcedureAndFunctionDeclarationPart ();

				//pass code to analyzer and gen code
				annie.AddTable (currentTable);
				annie.GenTable();

				StatementPart ();
			}
		}

		public void VariableDeclarationPart(){
			if(ct == "MP_VAR"){
				//rule 5
				if(ct == "MP_VAR"){
					Peek ();
				}

				//set kind to var
				rKind = "var";

				VariableDeclaration ();

				if(ct == "MP_SCOLON"){
					Peek ();
				}

				VariableDeclarationTail ();
			}

			if(ct=="MP_BEGIN"||ct=="MP_PROCEDURE"||ct=="MP_FUNCTION"){
				//rule6 - lambda
			}
		}

		public void VariableDeclarationTail(){
			if(ct == "MP_IDENTIFIER"){
				//rule 7
				VariableDeclaration ();

				if(ct == "MP_SCOLON"){
					Peek ();
				}

				VariableDeclarationTail ();
			}

			if(ct=="MP_BEGIN"||ct=="MP_PROCEDURE"||ct=="MP_FUNCTION"){
				//rule8 - lambda
			}
		}

		public void VariableDeclaration(){
			if (ct == "MP_IDENTIFIER") {
				//rule9
				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				}

				Type ();

				//insert vars to symbol table
				while(rLexStack.Count != 0){
					rLex = (string)rLexStack.Pop ();
					cRecord = new TableRecord (rLex,rType,rKind,rMode,rSize);
					//cRecord.SetOffset (rOffset);
					//rOffset++;
					currentTable.AddRecord (cRecord);
				}
			}
		}

		public void Type(){
			if (ct == "MP_INTEGER") {
				//rule10
				Peek ();

				rType = "int";
				rSize = 1;
			}
			else if(ct == "MP_FLOAT"){
				//rule11
				Peek ();

				rType = "float";
				rSize = 1;
			}
			else if(ct == "MP_STRING"){
				//rule12
				Peek ();

				rType = "string";
				rSize = 1;
			}
			else if(ct == "MP_BOOLEAN"){
				//rule13
				Peek ();

				rType = "bool";
				rSize = 1;
			}
		}

		public void ProcedureAndFunctionDeclarationPart(){
			if (ct == "MP_PROCEDURE") {
				//rule14
				ProcedureDeclaration ();

				ProcedureAndFunctionDeclarationPart ();
			} else if (ct == "MP_FUNCTION") {
				//rule15
				FunctionDeclaration ();

				ProcedureAndFunctionDeclarationPart ();
			} else if (ct == "MP_BEGIN"){
				//rule16 - lambda
			}
		}

		public void ProcedureDeclaration(){
			if (ct == "MP_PROCEDURE") {
				//rule17
				ProcedureHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				}

				Block ();

				if (ct == "MP_SCOLON") {
					Peek ();
				}
			}
		}

		public void FunctionDeclaration(){
			if (ct == "MP_FUNCTION") {
				//rule18
				FunctionHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				}

				Block ();

				if (ct == "MP_SCOLON") {
					Peek ();
				}
			}
		}

		public void ProcedureHeading(){
			if (ct == "MP_PROCEDURE") {
				//rule19
				Peek ();

				ProcedureIdentifier ();

				OptionalFormalParameterList ();
			}
		}

		public void FunctionHeading(){
			if (ct == "MP_FUNCTION") {
				//rule20
				Peek ();

				FunctionIdentifier ();

				OptionalFormalParameterList ();

				if (ct == "MP_COLON") {
					Peek ();
				}

				Type ();
			}
		}

		public void OptionalFormalParameterList(){
			if (ct == "MP_LPAREN") {
				//rule21
				Peek ();

				FormalParameterSection ();

				FormalParameterSectionTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			} else if (ct == "MP_SCOLON" || ct == "MP_COLON") {
				//rule22 - lambda
			}
		}

		public void FormalParameterSectionTail(){
			if (ct == "MP_SCOLON") {
				//rule23
				Peek ();

				FormalParameterSection ();

				FormalParameterSectionTail ();
			} else if (ct == "MP_RPAREN") {
				//rule24 - lambda
			}
		}

		public void FormalParameterSection(){
			if (ct == "MP_IDENTIFIER") {
				//rule25
				ValueParameterSection ();
			} else if (ct == "MP_VAR") {
				//rule26
				VariableParameterSection ();
			}
		}

		public void ValueParameterSection(){
			if (ct == "MP_IDENTIFIER") {
				//rule27
				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				}

				Type ();
			}
		}

		public void VariableParameterSection(){
			if (ct == "MP_VAR") {
				//rule28
				Peek ();

				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				}

				Type ();
			}
		}

		public void StatementPart(){
			if (ct == "MP_BEGIN") {
				//rule29
				CompoundStatement ();
			}
		}

		public void CompoundStatement(){
			if (ct == "MP_BEGIN") {
				//rule30
				Peek ();

				StatementSequence ();

				if (ct == "MP_END") {
					Peek ();
				}
			}
		}

		public void StatementSequence(){
			if (ct == "MP_BEGIN" || ct == "MP_END" || ct == "MP_IF" || ct == "MP_FOR" || ct == "MP_READ" || ct == "MP_REPEAT" || ct == "MP_UNTIL" ||
			   ct == "MP_WHILE" || ct == "MP_WRITE" || ct == "MP_WRITELN" || ct == "MP_IDENTIFIER" || ct == "MP_SCOLON") {
				//rule31
				Statement ();

				StatementTail ();
			}
		}

		public void StatementTail(){
			if (ct == "MP_SCOLON") {
				//rule32
				Peek ();

				Statement ();

				StatementTail ();
			} else if (ct == "MP_UNTIL" || ct == "MP_END") {
				//rule33 - lambda
			}
		}

		public void Statement(){
			if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule34
				EmptyStatement ();
			} else if (ct == "MP_BEGIN") {
				//rule35
				CompoundStatement ();
			} else if (ct == "MP_READ") {
				//rule36
				ReadStatement ();
			} else if (ct == "MP_WRITE" || ct == "MP_WRITELN") {
				//rule37
				WriteStatement ();
			} else if (ct == "MP_IDENTIFIER") {
				//rule38
				AssignProcedureStatement ();
			} else if (ct == "MP_IF") {
				//rule39
				IfStatement ();
			} else if (ct == "MP_WHILE") {
				//rule40
				WhileStatement ();
			} else if (ct == "MP_REPEAT") {
				//rule41
				RepeatStatement ();
			} else if (ct == "MP_FOR") {
				//rule42
				ForStatement ();
			}
			//there is no rule43
		}

		public void EmptyStatement(){
			if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule44 - lambda
			}
		}

		public void ReadStatement(){
			if (ct == "MP_READ") {
				//rule45
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				}

				ReadParameter ();

				ReadParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			}
		}

		public void ReadParameterTail(){
			if (ct == "MP_COMMA") {
				//rule46
				Peek ();

				ReadParameter ();

				ReadParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule47 - lambda
			}
		}

		public void ReadParameter(){
			if (ct == "MP_IDENTIFIER") {
				//rule48
				VariableIdentifier ();
			}
		}

		public void WriteStatement(){
			if (ct == "MP_WRITE") {
				//rule49
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				}

				WriteParameter ();

				WriteParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			} else if (ct == "MP_WRITELN") {
				//rule50
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				}

				WriteParameter ();

				WriteParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			}
		}

		public void WriteParameterTail(){
			if (ct == "MP_COMMA") {
				//rule51
				Peek ();

				WriteParameter ();

				WriteParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule52 - lambda
			}
		}

		public void WriteParameter(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule53
				OrdinalExpression ();
			} else {
				Console.WriteLine ("Syntax Error");
			}
		}

		public void AssignmentStatement(){
			//currently unused
		}

		public void IfStatement(){
			if (ct == "MP_IF") {
				//rule56
				Peek ();

				BooleanExpression ();

				if (ct == "MP_THEN") {
					Peek ();
				}

				Statement ();

				OptionalElsePart ();
			}
		}

		public void OptionalElsePart(){
			if (ct == "MP_ELSE") {
				//rule57
				Peek ();

				Statement ();
			} else if (ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule58 - lambda
			}
		}

		public void RepeatStatement(){
			if (ct == "MP_REPEAT") {
				//rule59
				Peek ();

				StatementSequence ();

				if (ct == "MP_UNTIL") {
					Peek ();
				}

				BooleanExpression ();
			}
		}

		public void WhileStatement(){
			if (ct == "MP_WHILE") {
				//rule60
				Peek ();

				BooleanExpression ();

				if (ct == "MP_DO") {
					Peek ();
				}

				Statement ();
			}
		}

		public void ForStatement(){
			if (ct == "MP_FOR") {
				//rule61
				Peek ();

				ControlVariable ();

				if (ct == "MP_ASSIGN") {
					Peek ();
				}

				InitialValue ();

				StepValue ();

				FinalValue ();

				if (ct == "MP_DO") {
					Peek ();
				}

				Statement ();
			}
		}

		public void ControlVariable(){
			if (ct == "MP_IDENTIFIER") {
				//rule62
				VariableIdentifier ();
			}
		}

		public void InitialValue(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule63
				OrdinalExpression ();
			}
		}

		public void StepValue(){
			if (ct == "MP_TO") {
				//rule64
				Peek ();
			} else if (ct == "MP_DOWNTO") {
				//rule65
				Peek ();
			}
		}

		public void FinalValue(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule66
				OrdinalExpression ();
			}
		}

		public void ProcedureStatement(){
			//currently not used
		}

		public void OptionalActualParameterList(){
			if (ct == "MP_LPAREN") {
				//rule68
				Peek ();

				ActualParameter ();

				ActualParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			} else if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL") {
				//rule69 - lambda
			}
		}

		public void ActualParameterTail(){
			if (ct == "MP_COMMA") {
				//rule70
				Peek ();

				ActualParameter ();

				ActualParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule71 - lambda
			}
		}

		public void ActualParameter(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule72
				OrdinalExpression ();
			}
		}

		public void Expression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule73
				SimpleExpression ();

				OptionalRelationalPart ();
			}
		}

		public void OptionalRelationalPart(){
			if (ct == "MP_EQUAL" || ct == "MP_GEQUAL" || ct == "MP_LEQUAL" || ct == "MP_GTHAN" || ct == "MP_LTHAN" ||
			    ct == "MP_NEQUAL") {
				//rule74
				annie.PassOp (currentLexeme);
				RelationalOperator ();

				SimpleExpression ();
				annie.GenArithmetic ();
			} else if (ct == "MP_DO" || ct == "MP_DOWNTO" || ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_THEN" ||
			           ct == "MP_TO" || ct == "MP_UNTIL" || ct == "MP_SCOLON" || ct == "MP_COMMA" || ct == "MP_LPAREN") {
				//rule75 - lambda
			}
		}

		public void RelationalOperator(){
			if (ct == "MP_EQUAL") {
				//rule76
				Peek ();
			} else if (ct == "MP_LTHAN") {
				//rule77
				Peek ();
			} else if (ct == "MP_GTHAN") {
				//rule78
				Peek ();
			} else if (ct == "MP_LEQUAL") {
				//rule79
				Peek ();
			} else if (ct == "MP_GEQUAL") {
				//rule80
				Peek ();
			} else if (ct == "MP_NEQUAL") {
				//rule81
				Peek ();
			}
		}

		public void SimpleExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule82
				OptionalSign ();

				Term ();

				TermTail ();
			}
		}

		public void TermTail(){
			if (ct == "MP_PLUS" || ct == "MP_MINUS" || ct == "MP_OR") {
				//rule83
				annie.PassOp (currentLexeme);
				AddingOperator ();

				Term ();
				annie.GenArithmetic ();

				TermTail ();
			} else if (ct == "MP_DO" || ct == "MP_DOWNTO" || ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_THEN" || ct == "MP_TO" || ct == "MP_UNTIL" ||
			           ct == "MP_COMMA" || ct == "MP_SCOLON" || ct == "MP_RPAREN" || ct == "MP_EQUAL" || ct == "MP_LTHAN" || ct == "MP_GTHAN" || ct == "MP_LEQUAL" ||
			           ct == "MP_GEQUAL" || ct == "MP_NEQUAL") {
				//rule84 - lambda
			}
		}

		public void OptionalSign(){
			if (ct == "MP_PLUS") {
				//rule85
				Peek ();
			} else if (ct == "MP_MINUS") {
				//rule86
				Peek ();
			} else if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			           ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN") {
				//rule87 - lambda
			}
		}

		public void AddingOperator(){
			if (ct == "MP_PLUS") {
				//rule88
				Peek ();
			} else if (ct == "MP_MINUS") {
				//rule89
				Peek ();
			} else if (ct == "MP_OR") {
				//rule90
				Peek ();
			}
		}

		public void Term(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN") {
				//rule91
				Factor ();

				FactorTail ();
			}
		}

		public void FactorTail(){
			if (ct == "MP_AND" || ct == "MP_DIV" || ct == "MP_MOD" || ct == "MP_TIMES" || ct == "MP_DIVIDE") {
				//rule92
				annie.PassOp (currentLexeme);
				MultiplyingOperator ();

				Factor ();
				annie.GenArithmetic ();

				FactorTail ();
			} else if (ct == "MP_DO" || ct == "MP_DOWNTO" || ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_OR" || ct == "MP_THEN" ||
			           ct == "MP_TO" || ct == "MP_UNTIL" || ct == "MP_COMMA" || ct == "MP_SCOLON" || ct == "MP_RPAREN" || ct == "MP_EQUAL" ||
			           ct == "MP_LTHAN" || ct == "MP_GTHAN" || ct == "MP_LEQUAL" || ct == "MP_GEQUAL" || ct == "MP_NEQUAL" ||
			           ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule93 - lambda
			}
		}

		public void MultiplyingOperator(){
			if (ct == "MP_TIMES") {
				//rule94
				Peek ();
			} else if (ct == "MP_DIVIDE") {
				//rule95
				Peek ();
			} else if (ct == "MP_DIV") {
				//rule96
				Peek ();
			} else if (ct == "MP_MOD") {
				//rule97
				Peek ();
			} else if (ct == "MP_AND") {
				//rule98
				Peek ();
			}
		}

		public void Factor(){
			if (ct == "MP_INTEGER_LIT") {
				//rule99
				annie.GenPushLit (currentLexeme,"int");
				Peek ();
			} else if (ct == "MP_FLOAT_LIT") {
				//rule100
				annie.GenPushLit (currentLexeme,"float");
				Peek ();
			} else if (ct == "MP_STRING_LIT") {
				//rule101
				annie.GenPushLit (currentLexeme,"string");
				Peek ();
			} else if (ct == "MP_TRUE") {
				//rule102
				annie.GenPushLit (currentLexeme,"boolean");
				Peek ();
			} else if (ct == "MP_FALSE") {
				//rule103
				Peek ();
			} else if (ct == "MP_NOT") {
				//rule104
				Peek ();

				Factor ();
			} else if (ct == "MP_LPAREN") {
				//rule105
				Peek ();

				Expression ();

				if (ct == "MP_RPAREN") {
					Peek ();
				}
			} else if (ct == "MP_IDENTIFIER") {
				//rule116
				annie.GenPushID (currentLexeme);
				VariableIdentifier ();
			}
		}

		public void ProgramIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule107
				Peek ();
			}
		}

		public void VariableIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule108
				Peek ();
			}
		}

		public void ProcedureIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule109
				Peek ();
			}
		}

		public void FunctionIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule110
				Peek ();
			}
		}

		public void BooleanExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule111
				Expression ();
			}
		}

		public void OrdinalExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
				ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule112
				Expression ();
			}
		}

		public void IdentifierList(){
			if (ct == "MP_IDENTIFIER") {
				//rule113
				//push on the lex on the stack
				rLexStack.Push (currentLexeme);
				VariableIdentifier ();

				IdentifierTail ();
			}
		}

		public void IdentifierTail(){
			if (ct == "MP_COMMA") {
				//rule114
				Peek ();

				rLexStack.Push (currentLexeme);
				VariableIdentifier ();

				IdentifierTail ();
			} else if (ct == "MP_COLON") {
				//rule115 - lambda
			}
		}

		public void AssignProcedureStatement(){
			if (ct == "MP_IDENTIFIER") {
				//rule117
				string target = currentLexeme;
				VariableIdentifier ();

				AssignProcedureTail ();

				if (assignFlag) {
					annie.GenAssign (target);
					assignFlag = false;
				}
			}
		}

		public void AssignProcedureTail(){
			if (ct == "MP_ASSIGN") {
				//rule118
				assignFlag = true;
				Peek ();

				Expression ();
			} else if (ct == "MP_LPAREN") {
				OptionalActualParameterList ();
			}
		}
	}
}

