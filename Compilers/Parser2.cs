using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Compilers
{
	public class Parser2
	{
		private bool hasError;
		private ArrayList errors;

		private LabelMaker labelMe;

		private SymbolTable currentTable;
		private SymbolTable nextTable;
		private string tablename;
		private int depth;
		private string label;

		private TableRecord cRecord;
		private TableRecord pfRecord;
		private string rLex;
		private string rType;
		private string rKind;
		private string rMode;
		private int rSize;
		//private int rOffset;
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
			hasError = false;
			errors = new ArrayList ();

			tokens = new StringReader (tokens_in);
			tokes = tokes_in;
			tokePoint = 0;

			annie = new SemanticAnalyzer ();
			assignFlag = false;

			depth = 0;

			labelMe = new LabelMaker ("L");

			rLexStack = new Stack ();
			rMode = "null";
			//rOffset = 0;
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
				Console.WriteLine ("Using rule 1");
				Program ();
			} else {
				errorMessage ("keyword 'program'");
			}

			if (ct == "MP_EOF"){
				if (hasError) {
					Console.WriteLine ("Parse failed!!! What have you done!?");
					PrintErrors ();
				} else {
					Console.WriteLine ("Program parsed successfully!");
					annie.GenTearDown ();
					annie.GenHalt ();
					annie.PrintCode ();
				}
			} else if (hasError) {
				Console.WriteLine ("Parse failed!!! What have you done!?");
				PrintErrors ();
			}
		}

		public void Program(){
			if (ct == "MP_PROGRAM") {
				//Rule 2
				Console.WriteLine ("Using rule 2");
				ProgramHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					//error check
					errorMessage (";");
				}

				Block ();

				if (ct == "MP_PERIOD") {
					Peek ();
				} else {
					//error check
					errorMessage (".");
				}
			} else {
				//error check
				errorMessage ("keyword 'program'");
			}
		}

		public void ProgramHeading(){
			if (ct == "MP_PROGRAM") {
				//rule 3
				Console.WriteLine ("Using rule 3");
				Peek ();

				//set new table name and label
				tablename = currentLexeme;
				label = labelMe.MakeLabel ();
				currentTable = new SymbolTable (tablename, depth, label);
				depth++;

				ProgramIdentifier ();
			} else {
				errorMessage ("keyword 'program'");
			}
		}

		public void Block(){
			if (ct == "MP_BEGIN" || ct == "MP_FUNCTION" || ct == "MP_PROCEDURE" || ct == "MP_VAR") {
				//rule 4
				Console.WriteLine ("Using rule 4");
				VariableDeclarationPart ();

				//pass code to analyzer and gen code
				annie.AddTable (currentTable, currentTable.GetDepth());
				annie.GenTable ();

				ProcedureAndFunctionDeclarationPart ();

				StatementPart ();
			} else {
				//error check
				errorMessage ("'begin','function','procedure' or 'var'");
			}
		}

		public void VariableDeclarationPart(){
			if (ct == "MP_VAR") {
				//rule 5
				Console.WriteLine ("Using rule 5");

				Peek ();

				//set kind to var
				rKind = "var";

				VariableDeclaration ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}

				VariableDeclarationTail ();
			} else if (ct == "MP_BEGIN" || ct == "MP_PROCEDURE" || ct == "MP_FUNCTION") {
				//rule6 - lambda
				Console.WriteLine ("Using rule 6");
			} else {
				errorMessage ("'var','begin','procedure' or 'function'");
			}
		}

		public void VariableDeclarationTail(){
			if (ct == "MP_IDENTIFIER") {
				//rule 7
				Console.WriteLine ("Using rule 7");
				VariableDeclaration ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}

				VariableDeclarationTail ();
			} else if (ct == "MP_BEGIN" || ct == "MP_PROCEDURE" || ct == "MP_FUNCTION") {
				//rule8 - lambda
				Console.WriteLine ("Using rule 8");
			} else {
				errorMessage ("an identifier,'begin','procedure' or 'function'");
			}
		}

		public void VariableDeclaration(){
			if (ct == "MP_IDENTIFIER") {
				//rule9
				Console.WriteLine ("Using rule 9");
				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				} else {
					errorMessage (":");
				}

				Type ();

				//insert vars to symbol table
				while (rLexStack.Count != 0) {
					rLex = (string)rLexStack.Pop ();
					cRecord = new TableRecord (rLex, rType, rKind, rMode, rSize);
					//cRecord.SetOffset (rOffset);
					//rOffset++;
					currentTable.AddRecord (cRecord);
				}
			} else {
				errorMessage ("an identifier");
			}
		}

		public void Type(){
			if (ct == "MP_INTEGER") {
				//rule10
				Console.WriteLine ("Using rule 10");
				Peek ();

				rType = "int";
				rSize = 1;
			} else if (ct == "MP_FLOAT") {
				//rule11
				Console.WriteLine ("Using rule 11");
				Peek ();

				rType = "float";
				rSize = 1;
			} else if (ct == "MP_STRING") {
				//rule12
				Console.WriteLine ("Using rule 12");
				Peek ();

				rType = "string";
				rSize = 1;
			} else if (ct == "MP_BOOLEAN") {
				//rule13
				Console.WriteLine ("Using rule 13");
				Peek ();

				rType = "bool";
				rSize = 1;
			} else {
				errorMessage ("'boolean','string','float' or 'integer'");
			}
		}

		public void ProcedureAndFunctionDeclarationPart(){
			if (ct == "MP_PROCEDURE") {
				//rule14
				Console.WriteLine ("Using rule 14");
				ProcedureDeclaration ();
				depth--;

				ProcedureAndFunctionDeclarationPart ();
			} else if (ct == "MP_FUNCTION") {
				//rule15
				Console.WriteLine ("Using rule 15");
				FunctionDeclaration ();
				depth--;

				ProcedureAndFunctionDeclarationPart ();
			} else if (ct == "MP_BEGIN") {
				//rule16 - lambda
				Console.WriteLine ("Using rule 16");
			} else {
				errorMessage ("'procedure','function' or 'begin'");
			}
		}

		public void ProcedureDeclaration(){
			if (ct == "MP_PROCEDURE") {
				//rule17
				Console.WriteLine ("Using rule 17");
				ProcedureHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}

				Block ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}
			} else {
				errorMessage ("keyword 'procedure'");
			}
		}

		public void FunctionDeclaration(){
			if (ct == "MP_FUNCTION") {
				//rule18
				Console.WriteLine ("Using rule 18");
				FunctionHeading ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}

				Block ();

				if (ct == "MP_SCOLON") {
					Peek ();
				} else {
					errorMessage (";");
				}
			} else {
				errorMessage ("keyword 'function'");
			}
		}

		public void ProcedureHeading(){
			if (ct == "MP_PROCEDURE") {
				//rule19
				Console.WriteLine ("Using rule 19");
				Peek ();
				string labelIt = labelMe.MakeLabel ();
				nextTable = new SymbolTable (currentLexeme, depth, labelIt);
				depth++;

				rKind = "procedure";
				rSize = 0;
				rLex = currentLexeme;
				rType = "null";
				pfRecord = new TableRecord (rLex,rType,rKind,rMode,rSize);

				ProcedureIdentifier ();

				OptionalFormalParameterList ();

				currentTable.AddRecord (pfRecord);
				currentTable = nextTable;
			} else {
				errorMessage ("keyword 'procedure'");
			}
		}

		public void FunctionHeading(){
			if (ct == "MP_FUNCTION") {
				//rule20
				Console.WriteLine ("Using rule 20");
				Peek ();

				string labelIt = labelMe.MakeLabel ();
				nextTable = new SymbolTable (currentLexeme, depth, labelIt);
				depth++;

				rKind = "function";
				rSize = 0;
				rLex = currentLexeme;
				rType = "null";
				pfRecord = new TableRecord (rLex,rType,rKind,rMode,rSize);

				FunctionIdentifier ();

				OptionalFormalParameterList ();

				if (ct == "MP_COLON") {
					Peek ();
				} else {
					errorMessage (":");
				}

				Type ();

				pfRecord.SetType (rType);
				currentTable.AddRecord (pfRecord);
				currentTable = nextTable;
			} else {
				errorMessage ("keyword 'function'");
			}
		}

		public void OptionalFormalParameterList(){
			if (ct == "MP_LPAREN") {
				//rule21
				Console.WriteLine ("Using rule 21");
				Peek ();
				rKind = "var";

				FormalParameterSection ();

				FormalParameterSectionTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else if (ct == "MP_SCOLON" || ct == "MP_COLON") {
				//rule22 - lambda
				Console.WriteLine ("Using rule 22");
			} else {
				errorMessage ("')',';' or ':'");
			}
		}

		public void FormalParameterSectionTail(){
			if (ct == "MP_SCOLON") {
				//rule23
				Console.WriteLine ("Using rule 23");
				Peek ();

				FormalParameterSection ();

				FormalParameterSectionTail ();
			} else if (ct == "MP_RPAREN") {
				//rule24 - lambda
				Console.WriteLine ("Using rule 24");
			} else {
				errorMessage ("';' or ')'");
			}
		}

		public void FormalParameterSection(){
			if (ct == "MP_IDENTIFIER") {
				//rule25
				Console.WriteLine ("Using rule 25");
				ValueParameterSection ();
			} else if (ct == "MP_VAR") {
				//rule26
				Console.WriteLine ("Using rule 26");
				VariableParameterSection ();
			} else {
				errorMessage ("an identifier or 'var'");
			}
		}

		public void ValueParameterSection(){
			if (ct == "MP_IDENTIFIER") {
				//rule27
				rMode = "in";
				Console.WriteLine ("Using rule 27");
				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				} else {
					errorMessage ("':'");
				}

				Type ();

				Stack tStack = new Stack ();
				while (rLexStack.Count != 0) {
					rLex = (string)rLexStack.Pop ();
					cRecord = new TableRecord (rLex, rType, rKind, rMode, rSize);
					//cRecord.SetOffset (rOffset);
					//rOffset++;
					nextTable.AddRecord (cRecord);
					Parameter pParam = new Parameter (rMode,rType);
					tStack.Push (pParam);
				}
				while(tStack.Count != 0){
					pfRecord.addParam ((Parameter)tStack.Pop());
				}

				rMode = "null";
			} else {
				errorMessage ("an identifier");
			}
		}

		public void VariableParameterSection(){
			if (ct == "MP_VAR") {
				//rule28
				rMode = "inout";
				Console.WriteLine ("Using rule 28");
				Peek ();

				IdentifierList ();

				if (ct == "MP_COLON") {
					Peek ();
				} else {
					errorMessage ("':'");
				}

				Type ();

				Stack tStack = new Stack ();
				while (rLexStack.Count != 0) {
					rLex = (string)rLexStack.Pop ();
					cRecord = new TableRecord (rLex, rType, rKind, rMode, rSize);
					nextTable.AddRecord (cRecord);
					Parameter fParam = new Parameter (rMode,rType);
					tStack.Push (fParam);
				}
				while(tStack.Count != 0){
					pfRecord.addParam ((Parameter)tStack.Pop());
				}

				rMode = "null";
			} else {
				errorMessage ("'var'");
			}
		}

		public void StatementPart(){
			if (ct == "MP_BEGIN") {
				//rule29
				Console.WriteLine ("Using rule 29");
				CompoundStatement ();
			} else {
				errorMessage ("keyword 'begin'");
			}
		}

		public void CompoundStatement(){
			if (ct == "MP_BEGIN") {
				//rule30
				Console.WriteLine ("Using rule 30");
				Peek ();

				StatementSequence ();

				if (ct == "MP_END") {
					Peek ();
				} else {
					errorMessage ("keyword 'end'");
				}
			} else {
				errorMessage ("keyword 'begin'");
			}
		}

		public void StatementSequence(){
			if (ct == "MP_BEGIN" || ct == "MP_END" || ct == "MP_IF" || ct == "MP_FOR" || ct == "MP_READ" || ct == "MP_REPEAT" || ct == "MP_UNTIL" ||
			    ct == "MP_WHILE" || ct == "MP_WRITE" || ct == "MP_WRITELN" || ct == "MP_IDENTIFIER" || ct == "MP_SCOLON") {
				//rule31
				Console.WriteLine ("Using rule 31");
				Statement ();

				StatementTail ();
			} else {
				errorMessage ("a statment beginning");
			}
		}

		public void StatementTail(){
			if (ct == "MP_SCOLON") {
				//rule32
				Console.WriteLine ("Using rule 32");
				Peek ();

				Statement ();

				StatementTail ();
			} else if (ct == "MP_UNTIL" || ct == "MP_END") {
				//rule33 - lambda
				Console.WriteLine ("Using rule 33");
			} else {
				errorMessage ("';','until' or 'end'");
			}
		}

		public void Statement(){
			if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule34
				Console.WriteLine ("Using rule 34");
				EmptyStatement ();
			} else if (ct == "MP_BEGIN") {
				//rule35
				Console.WriteLine ("Using rule 35");
				CompoundStatement ();
			} else if (ct == "MP_READ") {
				//rule36
				Console.WriteLine ("Using rule 36");
				ReadStatement ();
			} else if (ct == "MP_WRITE" || ct == "MP_WRITELN") {
				//rule37
				Console.WriteLine ("Using rule 37");
				WriteStatement ();
			} else if (ct == "MP_IDENTIFIER") {
				//rule38
				Console.WriteLine ("Using rule 38");
				AssignProcedureStatement ();
			} else if (ct == "MP_IF") {
				//rule39
				Console.WriteLine ("Using rule 39");
				IfStatement ();
			} else if (ct == "MP_WHILE") {
				//rule40
				Console.WriteLine ("Using rule 40");
				WhileStatement ();
			} else if (ct == "MP_REPEAT") {
				//rule41
				Console.WriteLine ("Using rule 41");
				RepeatStatement ();
			} else if (ct == "MP_FOR") {
				//rule42
				Console.WriteLine ("Using rule 42");
				ForStatement ();
			} else {
				errorMessage ("statement beginning");
			}
			//there is no rule43
		}

		public void EmptyStatement(){
			if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule44 - lambda
				Console.WriteLine ("Using rule 44");
			} else {
				errorMessage ("'else','end','until' or ';'");
			}
		}

		public void ReadStatement(){
			if (ct == "MP_READ") {
				//rule45
				Console.WriteLine ("Using rule 45");
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				} else {
					errorMessage ("'('");
				}

				ReadParameter ();

				ReadParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else {
				errorMessage ("keyword 'read'");
			}
		}

		public void ReadParameterTail(){
			if (ct == "MP_COMMA") {
				//rule46
				Console.WriteLine ("Using rule 46");
				Peek ();

				ReadParameter ();

				ReadParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule47 - lambda
				Console.WriteLine ("Using rule 47");
			} else {
				errorMessage ("',' or ')'");
			}
		}

		public void ReadParameter(){
			if (ct == "MP_IDENTIFIER") {
				//rule48
				Console.WriteLine ("Using rule 48");
				annie.GenRead (currentLexeme);
				VariableIdentifier ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void WriteStatement(){
			if (ct == "MP_WRITE") {
				//rule49
				Console.WriteLine ("Using rule 49");
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				} else {
					errorMessage ("'('");
				}

				WriteParameter ();

				WriteParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else if (ct == "MP_WRITELN") {
				//rule50
				Console.WriteLine ("Using rule 50");
				Peek ();

				if (ct == "MP_LPAREN") {
					Peek ();
				} else {
					errorMessage ("'('");
				}

				WriteParameter ();

				WriteParameterTail ();

				annie.GenWriteLine ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else {
				errorMessage ("'write' or 'writeln'");
			}
		}

		public void WriteParameterTail(){
			if (ct == "MP_COMMA") {
				//rule51
				Console.WriteLine ("Using rule 51");
				Peek ();

				WriteParameter ();

				WriteParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule52 - lambda
				Console.WriteLine ("Using rule 52");
			} else {
				errorMessage ("',' or ')'");
			}
		}

		public void WriteParameter(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule53
				Console.WriteLine ("Using rule 53");
				OrdinalExpression ();
				annie.GenWrite ();
			} else {
				errorMessage ("a write parameter");
			}
		}

		public void AssignmentStatement(){
			//currently unused
		}

		public void IfStatement(){
			if (ct == "MP_IF") {
				//rule56
				Console.WriteLine ("Using rule 56");
				Peek ();

				BooleanExpression ();

				string elseLabel = labelMe.MakeLabel ();
				string endLabel = labelMe.MakeLabel ();
				annie.GenBranchConditional (elseLabel);

				if (ct == "MP_THEN") {
					Peek ();
				} else {
					errorMessage ("keyword 'then'");
				}

				Statement ();
				annie.GenBranchUnconditional (endLabel);
				annie.GenLabel (elseLabel);

				OptionalElsePart ();
				annie.GenLabel (endLabel);
			} else {
				errorMessage ("keyword 'if'");
			}
		}

		public void OptionalElsePart(){
			if (ct == "MP_ELSE") {
				//rule57
				Console.WriteLine ("Using rule 57");
				Peek ();

				Statement ();
			} else if (ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_SCOLON") {
				//rule58 - lambda
				Console.WriteLine ("Using rule 58");
			} else {
				errorMessage ("'else','end','until' or ';'");
			}
		}

		public void RepeatStatement(){
			if (ct == "MP_REPEAT") {
				//rule59
				Console.WriteLine ("Using rule 59");
				Peek ();
				string loopLabel = labelMe.MakeLabel ();
				string endLabel = labelMe.MakeLabel ();
				annie.GenLabel (loopLabel);

				StatementSequence ();

				if (ct == "MP_UNTIL") {
					Peek ();
				} else {
					errorMessage ("keyword 'until'");
				}

				BooleanExpression ();
				annie.GenBranchConditionalT (endLabel);
				annie.GenBranchUnconditional (loopLabel);
				annie.GenLabel (endLabel);
			} else {
				errorMessage ("keyword 'repeat'");
			}
		}

		public void WhileStatement(){
			if (ct == "MP_WHILE") {
				//rule60
				Console.WriteLine ("Using rule 60");
				Peek ();
				string loopLabel = labelMe.MakeLabel ();
				string endLabel = labelMe.MakeLabel ();
				annie.GenLabel (loopLabel);

				BooleanExpression ();
				annie.GenBranchConditional (endLabel);

				if (ct == "MP_DO") {
					Peek ();
				} else {
					errorMessage ("keyword 'do'");
				}

				Statement ();

				annie.GenBranchUnconditional (loopLabel);
				annie.GenLabel (endLabel);
			} else {
				errorMessage ("keyword 'while'");
			}
		}

		public void ForStatement(){
			if (ct == "MP_FOR") {
				//rule61
				Console.WriteLine ("Using rule 61");
				Peek ();
				string loopLabel = labelMe.MakeLabel ();
				string endLabel = labelMe.MakeLabel ();

				string controlLex = currentLexeme;
				ControlVariable ();

				if (ct == "MP_ASSIGN") {
					Peek ();
				} else {
					errorMessage ("':='");
				}

				InitialValue ();
				annie.GenAssign (controlLex);
				annie.GenLabel (loopLabel);

				string stepType = currentLexeme;
				StepValue ();

				annie.GenPushID (controlLex);
				FinalValue ();
				annie.GenCompareEqual (stepType);
				annie.GenBranchConditional (endLabel);

				if (ct == "MP_DO") {
					Peek ();
				} else {
					errorMessage ("keyword 'do'");
				}

				Statement ();
				annie.GenPushID (controlLex);
				annie.GenPushLit ("1","int");
				if (stepType == "to") {
					annie.PassOp ("+");
				} else {
					annie.PassOp ("-");
				}
				annie.GenArithmetic ();
				annie.GenAssign (controlLex);
				annie.GenBranchUnconditional (loopLabel);
				annie.GenLabel (endLabel);
			} else {
				errorMessage ("keyword 'for'");
			}
		}

		public void ControlVariable(){
			if (ct == "MP_IDENTIFIER") {
				//rule62
				Console.WriteLine ("Using rule 62");
				VariableIdentifier ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void InitialValue(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule63
				Console.WriteLine ("Using rule 63");
				OrdinalExpression ();
			} else {
				errorMessage ("a value or expression");
			}
		}

		public void StepValue(){
			if (ct == "MP_TO") {
				//rule64
				Console.WriteLine ("Using rule 64");
				Peek ();
			} else if (ct == "MP_DOWNTO") {
				//rule65
				Console.WriteLine ("Using rule 65");
				Peek ();
			} else {
				errorMessage ("keyword 'to' or 'downto'");
			}
		}

		public void FinalValue(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule66
				Console.WriteLine ("Using rule 66");
				OrdinalExpression ();
			} else {
				errorMessage ("value or expression");
			}
		} 

		public void ProcedureStatement(){
			//currently not used
		}

		public void OptionalActualParameterList(){
			if (ct == "MP_LPAREN") {
				//rule68
				Console.WriteLine ("Using rule 68");
				Peek ();

				ActualParameter ();

				ActualParameterTail ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else if (ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_UNTIL" || ct == "MP_EQUAL" || ct == "MP_LTHAN" ||
				ct == "MP_GTHAN" || ct == "MP_LEQUAL" || ct == "MP_GEQUAL" || ct == "MP_NEQUAL" || ct == "MP_PLUS" || 
				ct == "MP_TIMES" || ct == "MP_MINUS" || ct == "MP_DIVIDE" || ct == "MP_DIV" || ct == "MP_AND" ||
				ct == "MP_OR" || ct == "MP_DO" || ct == "MP_COMMA" || ct == "MP_RPAREN" || ct == "MP_SCOLON") {
				//rule69 - lambda
				Console.WriteLine ("Using rule 69");
			} else {
				errorMessage ("'(','else','end' or 'until'");
			}
		}

		public void ActualParameterTail(){
			if (ct == "MP_COMMA") {
				//rule70
				Console.WriteLine ("Using rule 70");
				Peek ();

				ActualParameter ();

				ActualParameterTail ();
			} else if (ct == "MP_RPAREN") {
				//rule71 - lambda
				Console.WriteLine ("Using rule 71");
			} else {
				errorMessage ("',' or ')");
			}
		}

		public void ActualParameter(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule72
				Console.WriteLine ("Using rule 72");
				OrdinalExpression ();
			} else {
				errorMessage ("an expression");
			}
		}

		public void Expression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule73
				Console.WriteLine ("Using rule 73");
				SimpleExpression ();

				OptionalRelationalPart ();
			} else {
				errorMessage ("an expression");
			}
		}

		public void OptionalRelationalPart(){
			if (ct == "MP_EQUAL" || ct == "MP_GEQUAL" || ct == "MP_LEQUAL" || ct == "MP_GTHAN" || ct == "MP_LTHAN" ||
			    ct == "MP_NEQUAL") {
				//rule74
				Console.WriteLine ("Using rule 74");
				annie.PassOp (currentLexeme);
				RelationalOperator ();

				SimpleExpression ();
				annie.GenArithmetic ();
			} else if (ct == "MP_DO" || ct == "MP_DOWNTO" || ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_THEN" ||
				ct == "MP_TO" || ct == "MP_UNTIL" || ct == "MP_SCOLON" || ct == "MP_COMMA" || ct == "MP_LPAREN" || ct == "MP_RPAREN") {
				//rule75 - lambda
				Console.WriteLine ("Using rule 75");
			} else {
				errorMessage ("a relational operator or expression end");
			}
		}

		public void RelationalOperator(){
			if (ct == "MP_EQUAL") {
				//rule76
				Console.WriteLine ("Using rule 76");
				Peek ();
			} else if (ct == "MP_LTHAN") {
				//rule77
				Console.WriteLine ("Using rule 77");
				Peek ();
			} else if (ct == "MP_GTHAN") {
				//rule78
				Console.WriteLine ("Using rule 78");
				Peek ();
			} else if (ct == "MP_LEQUAL") {
				//rule79
				Console.WriteLine ("Using rule 79");
				Peek ();
			} else if (ct == "MP_GEQUAL") {
				//rule80
				Console.WriteLine ("Using rule 80");
				Peek ();
			} else if (ct == "MP_NEQUAL") {
				//rule81
				Console.WriteLine ("Using rule 81");
				Peek ();
			} else {
				errorMessage ("a relational operator");
			}
		}

		public void SimpleExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule82
				Console.WriteLine ("Using rule 82");
				OptionalSign ();

				Term ();

				TermTail ();
			} else {
				errorMessage ("an expression");
			}
		}

		public void TermTail(){
			if (ct == "MP_PLUS" || ct == "MP_MINUS" || ct == "MP_OR") {
				//rule83
				Console.WriteLine ("Using rule 83");
				annie.PassOp (currentLexeme);
				AddingOperator ();

				Term ();
				annie.GenArithmetic ();

				TermTail ();
			} else if (ct == "MP_DO" || ct == "MP_DOWNTO" || ct == "MP_ELSE" || ct == "MP_END" || ct == "MP_THEN" || ct == "MP_TO" || ct == "MP_UNTIL" ||
			           ct == "MP_COMMA" || ct == "MP_SCOLON" || ct == "MP_RPAREN" || ct == "MP_EQUAL" || ct == "MP_LTHAN" || ct == "MP_GTHAN" || ct == "MP_LEQUAL" ||
			           ct == "MP_GEQUAL" || ct == "MP_NEQUAL") {
				//rule84 - lambda
				Console.WriteLine ("Using rule 84");
			} else {
				errorMessage ("an addition operator or expression end");
			}
		}

		public void OptionalSign(){
			if (ct == "MP_PLUS") {
				//rule85
				Console.WriteLine ("Using rule 85");
				Peek ();
			} else if (ct == "MP_MINUS") {
				//rule86
				Console.WriteLine ("Using rule 86");
				Peek ();
			} else if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			           ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN") {
				//rule87 - lambda
				Console.WriteLine ("Using rule 87");
			} else {
				errorMessage ("expected a sign or factor");
			}
		}

		public void AddingOperator(){
			if (ct == "MP_PLUS") {
				//rule88
				Console.WriteLine ("Using rule 88");
				Peek ();
			} else if (ct == "MP_MINUS") {
				//rule89
				Console.WriteLine ("Using rule 89");
				Peek ();
			} else if (ct == "MP_OR") {
				//rule90
				Console.WriteLine ("Using rule 90");
				Peek ();
			} else {
				errorMessage ("an addition operator");
			}
		}

		public void Term(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN") {
				//rule91
				Console.WriteLine ("Using rule 91");
				Factor ();

				FactorTail ();
			} else {
				errorMessage ("a factor");
			}
		}

		public void FactorTail(){
			if (ct == "MP_AND" || ct == "MP_DIV" || ct == "MP_MOD" || ct == "MP_TIMES" || ct == "MP_DIVIDE") {
				//rule92
				Console.WriteLine ("Using rule 92");
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
				Console.WriteLine ("Using rule 93");
			} else {
				errorMessage ("multiplication operator or expression end");
			}
		}

		public void MultiplyingOperator(){
			if (ct == "MP_TIMES") {
				//rule94
				Console.WriteLine ("Using rule 94");
				Peek ();
			} else if (ct == "MP_DIVIDE") {
				//rule95
				Console.WriteLine ("Using rule 95");
				Peek ();
			} else if (ct == "MP_DIV") {
				//rule96
				Console.WriteLine ("Using rule 96");
				Peek ();
			} else if (ct == "MP_MOD") {
				//rule97
				Console.WriteLine ("Using rule 97");
				Peek ();
			} else if (ct == "MP_AND") {
				//rule98
				Console.WriteLine ("Using rule 98");
				Peek ();
			} else {
				errorMessage ("a multiplication operator");
			}
		}

		public void Factor(){
			if (ct == "MP_INTEGER_LIT") {
				//rule99
				Console.WriteLine ("Using rule 99");
				annie.GenPushLit (currentLexeme, "int");
				Peek ();
			} else if (ct == "MP_FLOAT_LIT") {
				//rule100
				Console.WriteLine ("Using rule 100");
				annie.GenPushLit (currentLexeme, "float");
				Peek ();
			} else if (ct == "MP_STRING_LIT") {
				//rule101
				Console.WriteLine ("Using rule 101");
				annie.GenPushLit (currentLexeme, "string");
				Peek ();
			} else if (ct == "MP_TRUE") {
				//rule102
				Console.WriteLine ("Using rule 102");
				annie.GenPushLit ("1", "bool");
				Peek ();
			} else if (ct == "MP_FALSE") {
				//rule103
				Console.WriteLine ("Using rule 103");
				annie.GenPushLit ("0", "bool");
				Peek ();
			} else if (ct == "MP_NOT") {
				//rule104
				Console.WriteLine ("Using rule 104");
				Peek ();

				Factor ();
			} else if (ct == "MP_LPAREN") {
				//rule105
				Console.WriteLine ("Using rule 105");
				Peek ();

				Expression ();

				if (ct == "MP_RPAREN") {
					Peek ();
				} else {
					errorMessage ("')'");
				}
			} else if (ct == "MP_IDENTIFIER") {
				//rule116
				Console.WriteLine ("Using rule 116");
				annie.GenPushID (currentLexeme);
				VariableIdentifier ();

				OptionalActualParameterList ();
			} else {
				errorMessage ("a factor");
			}
		}

		public void ProgramIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule107
				Console.WriteLine ("Using rule 107");
				Peek ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void VariableIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule108
				Console.WriteLine ("Using rule 108");
				Peek ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void ProcedureIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule109
				Console.WriteLine ("Using rule 109");
				Peek ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void FunctionIdentifier(){
			if (ct == "MP_IDENTIFIER") {
				//rule110
				Console.WriteLine ("Using rule 110");
				Peek ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void BooleanExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule111
				Console.WriteLine ("Using rule 111");
				Expression ();
			} else {
				errorMessage ("an expression");
			}
		}

		public void OrdinalExpression(){
			if (ct == "MP_FALSE" || ct == "MP_NOT" || ct == "MP_TRUE" || ct == "MP_IDENTIFIER" || ct == "MP_INTEGER_LIT" ||
			    ct == "MP_FLOAT_LIT" || ct == "MP_STRING_LIT" || ct == "MP_LPAREN" || ct == "MP_PLUS" || ct == "MP_MINUS") {
				//rule112
				Console.WriteLine ("Using rule 112");
				Expression ();
			} else {
				errorMessage ("an expression");
			}
		}

		public void IdentifierList(){
			if (ct == "MP_IDENTIFIER") {
				//rule113
				Console.WriteLine ("Using rule 113");
				//push on the lex on the stack
				rLexStack.Push (currentLexeme);
				VariableIdentifier ();

				IdentifierTail ();
			} else {
				errorMessage ("an identifier");
			}
		}

		public void IdentifierTail(){
			if (ct == "MP_COMMA") {
				//rule114
				Console.WriteLine ("Using rule 114");
				Peek ();

				rLexStack.Push (currentLexeme);
				VariableIdentifier ();

				IdentifierTail ();
			} else if (ct == "MP_COLON") {
				//rule115 - lambda
				Console.WriteLine ("Using rule 115");
			} else {
				errorMessage ("':' or ','");
			}
		}

		public void AssignProcedureStatement(){
			if (ct == "MP_IDENTIFIER") {
				//rule117
				Console.WriteLine ("Using rule 117");
				string target = currentLexeme;
				VariableIdentifier ();

				AssignProcedureTail ();

				if (assignFlag) {
					annie.GenAssign (target);
					assignFlag = false;
				}
			} else {
				errorMessage ("an identifier");
			}
		}

		public void AssignProcedureTail(){
			if (ct == "MP_ASSIGN") {
				//rule118
				Console.WriteLine ("Using rule 118");
				assignFlag = true;
				Peek ();

				Expression ();
			} else if (ct == "MP_LPAREN" || ct == "MP_SCOLON") {
				OptionalActualParameterList ();
			} else {
				errorMessage ("'(' or ':='");
			}
		}

		public void errorMessage(string expected){
			//error check
			StringBuilder errorOutput = new StringBuilder ();
			errorOutput.Append ("Syntax error on row: ");
			errorOutput.Append (curRow);
			errorOutput.Append ("  column: ");
			errorOutput.Append (curCol);
			errorOutput.Append ("  Expected ");
			errorOutput.Append (expected);
			errorOutput.Append (", got ");
			errorOutput.Append (currentLexeme);
			string newError = errorOutput.ToString ();
			errors.Add (newError);

			hasError = true;

			Peek ();
		}

		public void PrintErrors(){
			for (int i = 0; i < errors.Count; i++){
				Console.WriteLine ((string)errors[i]);
			}
		}
	}
}

