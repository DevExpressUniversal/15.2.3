#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
#if DXCORE
using StructuralParser = DevExpress.CodeRush.StructuralParser;
#else
using StructuralParser = DevExpress.CodeParser;
#endif
	public  class Tokens
	{
		public const int FINALLY = 14;
		public const int SHIFTRIGHT = 58;
		public const int FALSE = 33;
		public const int XORSYMBOL = 62;
		public const int ANDEQUAL = 77;
		public const int XOREQUAL = 79;
		public const int BITAND = 60;
		public const int STRINGLITERAL = 5;
		public const int BREAK = 6;
		public const int MINUSMINUS = 56;
		public const int LPAR = 36;
		public const int DO = 12;
		public const int XMLCOMMENT = 84;
		public const int LESSTHAN = 43;
		public const int DOUBLEEQUALS = 47;
		public const int PLUS = 51;
		public const int SHIFTLEFT = 57;
		public const int SEMICOLON = 41;
		public const int OROR = 66;
		public const int SINGLELINECOMMENT = 82;
		public const int WITH = 30;
		public const int IDENTIFIER = 1;
		public const int SLASH = 80;
		public const int NEW = 20;
		public const int ASTERISK = 53;
		public const int TRIPLESHIFTRIGHT = 59;
		public const int COLON = 68;
		public const int OREQUAL = 78;
		public const int TRIPLEEQUALS = 49;
		public const int IFKEYWORD = 17;
		public const int EOF = 0;
		public const int RBRACE = 35;
		public const int GREATERTHAN = 44;
		public const int QUESTIONSYMBOL = 67;
		public const int CODEEMBEDDING = 2;
		public const int SWITCH = 22;
		public const int MODEQUAL = 73;
		public const int INSTANCEOF = 19;
		public const int DECIMALLITERAL = 4;
		public const int BITOR = 61;
		public const int PERCENTSYMBOL = 54;
		public const int LBRACK = 38;
		public const int WHILE = 29;
		public const int CONTINUE = 9;
		public const int FOR = 15;
		public const int ELSE = 13;
		public const int RETURN = 21;
		public const int TRY = 25;
		public const int CATCH = 8;
		public const int TILDE = 64;
		public const int DIVEQUAL = 81;
		public const int LBRACE = 34;
		public const int DEFAULT = 10;
		public const int DOT = 40;
		public const int EXCLAMATIONSYMBOL = 63;
		public const int FUNCTION = 16;
		public const int REGEXPLITERAL = 85;
		public const int RBRACK = 39;
		public const int NOTDOUBLEEQUALS = 50;
		public const int THIS = 23;
		public const int RPAR = 37;
		public const int MULEQUAL = 72;
		public const int EQUALSSYMBOL = 69;
		public const int INTEGERLITERAL = 3;
		public const int NOTEQUALS = 48;
		public const int NULL = 31;
		public const int CASE = 7;
		public const int PLUSPLUS = 55;
		public const int MULTILINECOMMENT = 83;
		public const int ANDAND = 65;
		public const int MINUS = 52;
		public const int VAR = 27;
		public const int VOID = 28;
		public const int DELETE = 11;
		public const int COMMA = 42;
		public const int SHIFTRIGHTEQUAL = 75;
		public const int TYPEOF = 26;
		public const int LESSOREQUAL = 45;
		public const int MINUSEQUAL = 71;
		public const int TRIPLESHIFTRIGHTEQUAL = 76;
		public const int THROW = 24;
		public const int SHIFTLEFTEQUAL = 74;
		public const int PLUSEQUAL = 70;
		public const int GREATEROREQUAL = 46;
		public const int TRUE = 32;
		public const int IN = 18;
		public const int MaxTokens = 86;
		public static int[] Keywords = {
		};
	}
	partial class JavaScriptParser
	{
		protected override void HandlePragmas()
		{
		}
			void Parser()
	{
		if (la.Type == Tokens.XMLCOMMENT )
		{
			Get();
		}
		SourceElement();
		while (StartOf(1))
		{
			SourceElement();
		}
	}
	void SourceElement()
	{
		if (la.Type != Tokens.FUNCTION)
		{
			Statement();
		}
		else if (la.Type == Tokens.FUNCTION )
		{
			FunctionDeclaration();
		}
		else
			SynErr(87);
	}
	void Statement()
	{
		if (IsLabelledStatement())
		{
			LabelledStatement();
		}
		else if (StartOf(1))
		{
			if (la.Type == Tokens.LBRACE)
			{
				Block();
			}
			else if (la.Type == Tokens.SEMICOLON )
			{
				EmptyStatement();
			}
			else if (StartOf(2))
			{
				ExpressionStatement();
			}
			else if (la.Type == Tokens.IFKEYWORD )
			{
				IfStatement();
			}
			else if (la.Type == Tokens.DO )
			{
				DoStatement();
			}
			else if (la.Type == Tokens.WHILE )
			{
				WhileStatement();
			}
			else if (la.Type == Tokens.FOR )
			{
				ForStatements();
			}
			else if (la.Type == Tokens.CONTINUE )
			{
				ContinueStatement();
			}
			else if (la.Type == Tokens.BREAK )
			{
				BreakStatement();
			}
			else if (la.Type == Tokens.RETURN )
			{
				ReturnStatement();
			}
			else if (la.Type == Tokens.WITH )
			{
				WithStatement();
			}
			else if (la.Type == Tokens.VAR )
			{
				VariableStatement();
			}
			else if (la.Type == Tokens.SWITCH )
			{
				SwitchStatement();
			}
			else if (la.Type == Tokens.THROW )
			{
				ThrowStatement();
			}
			else
			{
				TryStatement();
			}
		}
		else
			SynErr(88);
	}
	void FunctionDeclaration()
	{
		Method method = null;
		SourceRange nameRange = SourceRange.Empty;
		SourceRange parenOpenRange = SourceRange.Empty;
		SourceRange parenCloseRange = SourceRange.Empty;
		LanguageElementCollection parameters = null;
		Expect(Tokens.FUNCTION );
		method = new Method(la.Value);
		method.Visibility = MemberVisibility.Public;
		OpenContext(method);
		method.SetRange(tToken.Range);
		nameRange = la.Range;
		Identifier();
		Expect(Tokens.LPAR );
		parenOpenRange = tToken.Range;
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			FormalParameterList(out parameters);
		}
		Expect(Tokens.RPAR );
		parenCloseRange = tToken.Range;
		Expect(Tokens.LBRACE );
		SetMethodProperties(method, nameRange, parenOpenRange, parenCloseRange, parameters);
		ReadBlockStart(tToken.Range);
		FunctionBody();
		ReadBlockEnd(la.Range);
		Expect(Tokens.RBRACE );
		CloseContext();
		method.SetRange(GetRange(method, tToken));
	}
	void Identifier()
	{
		if (la.Type == Tokens.IDENTIFIER )
		{
			Get();
		}
		else if (la.Type == Tokens.CODEEMBEDDING )
		{
			Get();
		}
		else
			SynErr(89);
	}
	void FormalParameterList(out LanguageElementCollection parameters)
	{
		parameters = new LanguageElementCollection();
		Param parameter = null;
		FormalParameter(out parameter);
		if (parameter != null)
		 parameters.Add(parameter);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			FormalParameter(out parameter);
			if (parameter != null)
			 parameters.Add(parameter);
		}
	}
	void FunctionBody()
	{
		SourceElement();
		while (StartOf(1))
		{
			SourceElement();
		}
	}
	void FormalParameter(out Param parameter)
	{
		parameter = CreateParam(la);
		Identifier();
	}
	void LabelledStatement()
	{
		Label label = new Label();
		label.Name = la.Value;
		AddNode(label);
		Identifier();
		label.SetRange(GetRange(tToken, la));
		Expect(Tokens.COLON );
		Statement();
	}
	void Block()
	{
		Block block = null;
		if ((Context != null && Context is DelimiterCapableBlock && !(Context as DelimiterCapableBlock).BlockStart.IsEmpty) || Context is Block || Context is Case)
		{
		  block = new Block();
		  OpenContext(block);
		  block.SetRange(la.Range);
		}
		ReadBlockStart(la.Range); 
		if (Context != null && Context is ParentingStatement)
		  (Context as ParentingStatement).HasBlock = true;
		Expect(Tokens.LBRACE );
		if (StartOf(1))
		{
			StatementList();
		}
		Expect(Tokens.RBRACE );
		ReadBlockEnd(tToken.Range); 
		if (block != null)
		{
		  block.SetRange(GetRange(block, tToken.Range));
		  CloseContext();
		}
	}
	void EmptyStatement()
	{
		StructuralParser.Statement emptyStatement = new EmptyStatement();
		emptyStatement.Name = ";";
		emptyStatement.SetRange(la.Range);
		AddNode(emptyStatement);
		Expect(Tokens.SEMICOLON );
	}
	void ExpressionStatement()
	{
		Expression expression = null;
		StructuralParser.Statement statement = null;
		Token testToken = tToken; 
		Expression(out expression);
		Expect(Tokens.SEMICOLON );
		if (expression != null)
		{
		  if (expression is AssignmentExpression)
			statement = Assignment.FromAssignmentExpression(expression as AssignmentExpression);
		  else if (expression is MethodCallExpression)
			statement = MethodCall.FromMethodCallExpression(expression as MethodCallExpression);
		  else
			statement = StructuralParser.Statement.FromExpression(expression);
		  statement.SetRange(GetRange(expression, tToken));
		}
		else
		{
		  if (tToken.Type == Tokens.SEMICOLON)
		  {
			statement = new Statement(tToken.Value);
			statement.SetRange(tToken.Range);
		  }
		  else
			Get();
		}
		if (testToken == tToken)
		  Get();
		if (statement != null)
		  AddNode(statement);
	}
	void IfStatement()
	{
		Expression expression = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		If ifStatement = new If();
		ifStatement.SetRange(la.Range);
		OpenContext(ifStatement);
		Expect(Tokens.IFKEYWORD );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Expression(out expression);
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Statement();
		CloseContext();
		ifStatement.SetRange(GetRange(ifStatement, tToken));
		ifStatement.SetParensRange(GetRange(lparRange, rparRange));
		if (expression != null)
		  ifStatement.SetExpression(expression);
		if (la.Type == Tokens.ELSE )
		{
			Get();
			Else elseStatement = new Else();
			elseStatement.SetRange(tToken.Range);
			OpenContext(elseStatement);
			Statement();
			CloseContext();
			elseStatement.SetRange(GetRange(elseStatement, tToken));
		}
	}
	void DoStatement()
	{
		Expression expression = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Do doStatement = new Do();
		doStatement.SetRange(la.Range);
		OpenContext(doStatement);
		Expect(Tokens.DO );
		Statement();
		Expect(Tokens.WHILE );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Expression(out expression);
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Expect(Tokens.SEMICOLON );
		CloseContext();
		if (expression != null)
		  doStatement.SetCondition(expression);
		doStatement.SetParensRange(GetRange(lparRange, rparRange));
		doStatement.SetRange(GetRange(doStatement, tToken));
	}
	void WhileStatement()
	{
		Expression expression = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		While whileStatement = new While();
		whileStatement.SetRange(la.Range);
		OpenContext(whileStatement);
		Expect(Tokens.WHILE );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Expression(out expression);
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Statement();
		CloseContext();
		if (expression != null)
		  whileStatement.SetCondition(expression);
		whileStatement.SetParensRange(GetRange(lparRange, rparRange));
		whileStatement.SetRange(GetRange(whileStatement, tToken));
	}
	void ForStatements()
	{
		Expression expression = null;
		Expression inExpression = null;
		Expression initializer = null;
		Expression condition = null;
		Expression incrementor = null;
		Variable variable = null;
		SourceRange varRange = SourceRange.Empty;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		SourceRange startRange = la.Range;
		LanguageElement forBlock = null;
		LanguageElementCollection variables = null;
		Expect(Tokens.FOR );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		if (IsForEachLoop())
		{
			InsideForEachLoop = true; 
			if (StartOf(2))
			{
				Expression(out expression);
			}
			else if (la.Type == Tokens.VAR )
			{
				Get();
				VariableDeclaration(tToken.Range, tToken.Range, out variable);
			}
			else
				SynErr(90);
			Expect(Tokens.IN );
			Expression(out inExpression);
			Expect(Tokens.RPAR );
			rparRange = tToken.Range;
			ForEach forEach = new ForEach(variable, inExpression, null);
			forEach.SetRange(startRange);
			OpenContext(forEach);
			forBlock = forEach;
			forEach.SetParensRange(GetRange(lparRange, rparRange));
			forEach.FieldType = String.Empty;
			if (expression != null)
			{
			  forEach.AddDetailNode(expression);
			  forEach.Field = expression.ToString();
			}
			else
			{
			  if (variable != null)
			  {
				forEach.AddDetailNode(variable);
				forEach.Field = variable.Name;
			  }
			}
			if (inExpression != null)
			{
			  forEach.Expression = inExpression;
			  forEach.Collection = inExpression.ToString();
			}
			InsideForEachLoop = false;
			Statement();
		}
		else if (StartOf(3))
		{
			if (StartOf(4))
			{
				if (StartOf(2))
				{
					Expression(out initializer);
				}
			}
			else
			{
				Get();
				VariableDeclarationList(out variables);
			}
			Expect(Tokens.SEMICOLON );
			if (StartOf(2))
			{
				Expression(out condition);
			}
			Expect(Tokens.SEMICOLON );
			if (StartOf(2))
			{
				Expression(out incrementor);
			}
			Expect(Tokens.RPAR );
			rparRange = tToken.Range;
			For forStatement = new For();
			forStatement.SetRange(startRange);
			OpenContext(forStatement);
			forBlock = forStatement;
			forStatement.SetParensRange(GetRange(lparRange, rparRange));
			if (initializer != null)
			{
			  forStatement.AddInitializer(initializer);
			}
			else
			{
			  if (variables != null)
				for (int i = 0; i < variables.Count; i++)
				  forStatement.AddInitializer(variables[i]);
			}
			if (condition != null)
			{
			  forStatement.Condition = condition;
			  forStatement.AddDetailNode(condition);
			}
			if (incrementor != null)
			  forStatement.AddIncrementor(incrementor);
			Statement();
		}
		else
			SynErr(91);
		if (forBlock != null)
		{
		  CloseContext();
		 forBlock.SetRange(GetRange(forBlock, tToken));
		}
	}
	void ContinueStatement()
	{
		Continue continueStatement = new Continue();
		AddNode(continueStatement);
		SourceRange startRange = la.Range;
		Expect(Tokens.CONTINUE );
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			Identifier();
			continueStatement.Name = tToken.Value;
		}
		Expect(Tokens.SEMICOLON );
		continueStatement.SetRange(GetRange(startRange, tToken.Range));
	}
	void BreakStatement()
	{
		Break breakStatement = new Break();
		AddNode(breakStatement);
		SourceRange startRange = la.Range;
		Expect(Tokens.BREAK );
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			Identifier();
			breakStatement.Name = tToken.Value;
		}
		Expect(Tokens.SEMICOLON );
		breakStatement.SetRange(GetRange(startRange, tToken.Range));
	}
	void ReturnStatement()
	{
		Expression expression = null;
		Return returnStatement = new Return();
		AddNode(returnStatement);
		SourceRange startRange = la.Range;
		Expect(Tokens.RETURN );
		if (StartOf(2))
		{
			Expression(out expression);
			if (expression != null)
			{
			  returnStatement.Expression = expression;
			  returnStatement.AddDetailNode(expression);
			}
		}
		Expect(Tokens.SEMICOLON );
		returnStatement.SetRange(GetRange(startRange, tToken.Range));
	}
	void WithStatement()
	{
		With withStatement = new With();
		Expression expression = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		withStatement.SetRange(la.Range);
		OpenContext(withStatement);
		Expect(Tokens.WITH );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Expression(out expression);
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Statement();
		if (expression != null)
		{
		  withStatement.Expression = expression;
		  withStatement.AddDetailNode(expression);
		}
		withStatement.SetRange(GetRange(withStatement, tToken));
		withStatement.SetParensRange(GetRange(lparRange, rparRange));
		CloseContext();
	}
	void VariableStatement()
	{
		LanguageElementCollection variableList = null;
		Expect(Tokens.VAR );
		VariableDeclarationList(out variableList);
		if (variableList != null)
		 for (int i = 0; i < variableList.Count; i++)
		   AddNode(variableList[i]);
		Expect(Tokens.SEMICOLON );
	}
	void SwitchStatement()
	{
		Expression expression = null;
		Switch switchStatement = new Switch();
		switchStatement.SetRange(la.Range);
		OpenContext(switchStatement);
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Expect(Tokens.SWITCH );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Expression(out expression);
		if (expression != null)
		 switchStatement.Expression = expression;	
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Expect(Tokens.LBRACE );
		ReadBlockStart(tToken.Range);
		while (la.Type == Tokens.CASE  || la.Type == Tokens.DEFAULT )
		{
			CaseClause();
		}
		Expect(Tokens.RBRACE );
		ReadBlockEnd(tToken.Range);
		switchStatement.SetParensRange(GetRange(lparRange, rparRange));
		CloseContext();
		switchStatement.SetRange(GetRange(switchStatement, tToken));
	}
	void ThrowStatement()
	{
		Expression expression = null;
		Throw throwStatement = new Throw();
		AddNode(throwStatement);
		SourceRange startRange = la.Range;
		Expect(Tokens.THROW );
		Expression(out expression);
		if (expression != null)
		{
		  throwStatement.Expression = expression;
		  throwStatement.AddDetailNode(expression);
		}
		Expect(Tokens.SEMICOLON );
		throwStatement.SetRange(GetRange(startRange, tToken.Range)); 
	}
	void TryStatement()
	{
		Try tryBlock = new Try();
		OpenContext(tryBlock);
		tryBlock.SetRange(la.Range);
		Expect(Tokens.TRY );
		Block();
		CloseContext();
		tryBlock.SetRange(GetRange(tryBlock, tToken));
		while (la.Type == Tokens.CATCH  || la.Type == Tokens.FINALLY )
		{
			if (la.Type == Tokens.CATCH )
			{
				Catch();
			}
			else
			{
				Finally();
			}
		}
	}
	void StatementList()
	{
		Statement();
		while (StartOf(1))
		{
			Statement();
		}
		Expect(Tokens.SEMICOLON );
	}
	void VariableDeclarationList(out LanguageElementCollection variableList)
	{
		variableList = new LanguageElementCollection();
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		SourceRange varRange = tToken.Range;
		VariableDeclaration(varRange, varRange, out variable);
		if (variable != null)
		{
		  firstVariable = variable;
		  prevVariable = variable;
		  variableList.Add(variable);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			VariableDeclaration(varRange, la.Range, out variable);
			if (variable != null)
			{
			  variable.SetPreviousVariable(prevVariable);
			  variable.SetAncestorVariable(firstVariable);
			  prevVariable.SetNextVariable(variable);
			  prevVariable = variable;
			  variableList.Add(variable);
			}
		}
	}
	void VariableDeclaration(SourceRange varRange, SourceRange startRange, out Variable variable)
	{
		variable = null;
		Expression expression = null;
		SourceRange equalsRange = SourceRange.Empty;
		SourceRange nameRange = la.Range;
		String name = la.Value;
		SourceRange endRange = SourceRange.Empty;
		Identifier();
		if (la.Type == Tokens.EQUALSSYMBOL )
		{
			Get();
			equalsRange = tToken.Range;
			AssignmentExpression(out expression);
		}
		if (la.Type == Tokens.SEMICOLON || la.Type == Tokens.COMMA)
		 endRange = la.Range;
		else
		  endRange = tToken.Range;
		variable = CreateVariable(name, nameRange, expression, varRange, startRange, endRange, equalsRange);
	}
	void AssignmentExpression(out Expression result)
	{
		Expression rightPart = null;
		result = null;
		Token operatorToken = null;
		ConditionalExpression(out result);
		if (StartOf(5))
		{
			AssignmentOperator();
			operatorToken = tToken; 
			AssignmentExpression(out rightPart);
			result = CreateAssignmentExpression(result, rightPart, operatorToken);
		}
	}
	void Expression(out Expression result)
	{
		Expression nextExpression = null;
		ComplexExpression complexExpression = null;
		result = null;
		AssignmentExpression(out result);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			AssignmentExpression(out nextExpression);
			if (nextExpression != null)
			{
			  if (complexExpression == null && result != null)
			  {
				complexExpression = new ComplexExpression();
				complexExpression.Expressions.Add(result);
				complexExpression.AddDetailNode(result);
				complexExpression.SetRange(result.Range);
				result = complexExpression;
			  }
			  if (complexExpression != null)
			  {
				complexExpression.Expressions.Add(nextExpression);
				complexExpression.AddDetailNode(nextExpression);
				complexExpression.SetRange(GetRange(complexExpression, nextExpression));
			  }
			}
		}
	}
	void CaseClause()
	{
		Case caseClause = new Case();
		caseClause.SetRange(la.Range);
		if (la.Type == Tokens.DEFAULT)
		  caseClause.IsDefault = true;
		OpenContext(caseClause);
		Expression expression = null;
		if (la.Type == Tokens.CASE )
		{
			Get();
			Expression(out expression);
			if (expression != null)
			{
			  caseClause.Expression = expression;
			  caseClause.AddDetailNode(expression);
			}
			Expect(Tokens.COLON );
			if (StartOf(1))
			{
				StatementList();
			}
		}
		else if (la.Type == Tokens.DEFAULT )
		{
			Get();
			Expect(Tokens.COLON );
			if (StartOf(1))
			{
				StatementList();
			}
		}
		else
			SynErr(92);
		CloseContext();
		caseClause.SetRange(GetRange(caseClause, tToken));
	}
	void Catch()
	{
		Catch catchBlock = new Catch();
		OpenContext(catchBlock);
		catchBlock.SetRange(la.Range);
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Token catchesException = null;
		Expect(Tokens.CATCH );
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		Identifier();
		catchesException = tToken;
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		Block();
		catchBlock.SetParensRange(GetRange(lparRange, rparRange));
		if (catchesException != null)
		{
		  catchBlock.CatchesException = catchesException.Value;
		  TypeReferenceExpression exception = CreateTypeReference(catchesException);
		  catchBlock.Exception = exception;
		}
		CloseContext();
		catchBlock.SetRange(GetRange(catchBlock, tToken));
	}
	void Finally()
	{
		Finally finallyBlock = new Finally();
		OpenContext(finallyBlock);
		finallyBlock.SetRange(la.Range);
		Expect(Tokens.FINALLY );
		Block();
		CloseContext();
		finallyBlock.SetRange(GetRange(finallyBlock, tToken));
	}
	void PrimaryExpression(out Expression result)
	{
		result = null;
		SourceRange startRange = la.Range;
		switch (la.Type)
		{
		case Tokens.THIS : 
		{
			Get();
			result = CreateThisExpression(tToken); 
			break;
		}
		case Tokens.IDENTIFIER : 
case Tokens.CODEEMBEDDING : 
		{
			Identifier();
			result = CreateElementReference(tToken); 
			break;
		}
		case Tokens.INTEGERLITERAL : 
case Tokens.DECIMALLITERAL : 
case Tokens.STRINGLITERAL : 
case Tokens.NULL : 
case Tokens.TRUE : 
case Tokens.FALSE : 
		{
			Literal(out result);
			break;
		}
		case Tokens.SLASH : 
case Tokens.DIVEQUAL : 
		{
			Token startToken = la; (scanner as JavaScriptScanner).ShouldReadRegExpToken = true;
			if (la.Type == Tokens.SLASH )
			{
				Get();
			}
			else
			{
				Get();
			}
			(scanner as JavaScriptScanner).ShouldReadRegExpToken = false;
			Expect(Tokens.REGEXPLITERAL );
			result = new PrimitiveExpression(String.Concat(startToken.Value, tToken.Value), GetRange(startToken, tToken.Range));
			(result as PrimitiveExpression).PrimitiveType = PrimitiveType.RegularExpression;
			break;
		}
		case Tokens.LBRACK : 
		{
			ArrayLiteral(out result);
			break;
		}
		case Tokens.LBRACE : 
		{
			ObjectLiteral(out result);
			break;
		}
		case Tokens.LPAR : 
		{
			Get();
			Expression(out result);
			Expect(Tokens.RPAR );
			result = CreateParenthesizedExpression(result, startRange, tToken.Range); 
			break;
		}
		default: SynErr(93); break;
		}
	}
	void Literal(out Expression result)
	{
		result = null;
		PrimitiveExpression primitive = new PrimitiveExpression(la.Value, la.Range);
		primitive.PrimitiveType = JavaScriptPrimitiveTypeUtils.ToPrimitiveType(la.Type, la.Value);
		primitive.PrimitiveValue = JavaScriptPrimitiveTypeUtils.ToPrimitiveValue(la.Type, la.Value);
		result = primitive;
		switch (la.Type)
		{
		case Tokens.INTEGERLITERAL : 
		{
			Get();
			break;
		}
		case Tokens.DECIMALLITERAL : 
		{
			Get();
			break;
		}
		case Tokens.STRINGLITERAL : 
		{
			Get();
			break;
		}
		case Tokens.NULL : 
		{
			Get();
			break;
		}
		case Tokens.TRUE : 
		{
			Get();
			break;
		}
		case Tokens.FALSE : 
		{
			Get();
			break;
		}
		default: SynErr(94); break;
		}
	}
	void ArrayLiteral(out Expression result)
	{
		result = null;
		ExpressionCollection initializers = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.LBRACK );
		if (StartOf(6))
		{
			ElementList(out initializers);
		}
		Expect(Tokens.RBRACK );
		result = CreateArrayInitializerExpression(startRange, tToken.Range, initializers);
	}
	void ObjectLiteral(out Expression result)
	{
		result = null;
		ExpressionCollection initializers = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.LBRACE );
		if (StartOf(7))
		{
			PropertyNameAndValueList(out initializers);
		}
		Expect(Tokens.RBRACE );
		result = CreateObjectInitializer(initializers, startRange, tToken.Range);
	}
	void ElementList(out ExpressionCollection initializers)
	{
		Expression expression = null;
		Expression emptyElements = null;
		initializers = new ExpressionCollection();
		EmptyArrayElementExpression(out emptyElements);
		if (emptyElements != null)
		 initializers.Add(emptyElements);
		AssignmentExpression(out expression);
		if (expression != null)
		 initializers.Add(expression);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			EmptyArrayElementExpression(out emptyElements);
			if (emptyElements != null)
			 initializers.Add(emptyElements);
			AssignmentExpression(out expression);
			if (expression != null)
			 initializers.Add(expression);
			else if ((emptyElements as EmptyArrayElementExpression) != null)
			  (emptyElements as EmptyArrayElementExpression).EmptyElementsCount++;
		}
	}
	void EmptyArrayElementExpression(out Expression result)
	{
		result = null;
		int commaCount = 0;
		SourceRange startRange = la.Range;
		while (la.Type == Tokens.COMMA )
		{
			Get();
			commaCount ++;
		}
		if (commaCount > 0)
		{
		  EmptyArrayElementExpression emptyElement = new EmptyArrayElementExpression(commaCount);
		  emptyElement.SetRange(GetRange(startRange, tToken));
		  result = emptyElement;
		}
	}
	void PropertyNameAndValueList(out ExpressionCollection initializers)
	{
		initializers = new ExpressionCollection();
		Expression initializer = null;
		NameValuePair(out initializer);
		if (initializer != null)
		 initializers.Add(initializer);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			NameValuePair(out initializer);
			if (initializer != null)
			 initializers.Add(initializer);
		}
	}
	void NameValuePair(out Expression initializer)
	{
		initializer = null;
		Expression expression = null;
		Token nameToken = la;
		PropertyName();
		Expect(Tokens.COLON );
		AssignmentExpression(out expression);
		initializer = CreateMemberInitializer(nameToken, expression);
	}
	void PropertyName()
	{
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			Identifier();
		}
		else if (la.Type == Tokens.STRINGLITERAL )
		{
			Get();
		}
		else if (la.Type == Tokens.DECIMALLITERAL )
		{
			Get();
		}
		else if (la.Type == Tokens.INTEGERLITERAL )
		{
			Get();
		}
		else
			SynErr(95);
	}
	void TypeReference(out TypeReferenceExpression typeReference)
	{
		typeReference = null;
		if (la.Type == Tokens.LPAR )
		{
			Get();
			SourceRange startRange = tToken.Range;
			Expression expression = null;
			Expression(out expression);
			ParenthesizedTypeReferenceExpression result = new ParenthesizedTypeReferenceExpression();
			result.Expression = expression;
			result.SetRange(GetRange(startRange, la.Range));
			typeReference = result;
			Expect(Tokens.RPAR );
		}
		else if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING  || la.Type == Tokens.THIS )
		{
			if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
			{
				Identifier();
			}
			else
			{
				Get();
			}
			typeReference = CreateTypeReference(tToken); 
			while (la.Type == Tokens.DOT )
			{
				Get();
				typeReference = CreateTypeReference(typeReference, la); 
				if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
				{
					Identifier();
				}
				else if (la.Type == Tokens.THIS )
				{
					Get();
				}
				else
					SynErr(96);
			}
		}
		else
			SynErr(97);
	}
	void NewExpression(out Expression result)
	{
		result = null;
		TypeReferenceExpression typeReference = null;
		ExpressionCollection arguments = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		SourceRange startRange = la.Range;
		Expect(Tokens.NEW );
		if (StartOf(8))
		{
			TypeReference(out typeReference);
			if (la.Type == Tokens.LPAR )
			{
				Get();
				lparRange = tToken.Range;
				if (StartOf(2))
				{
					ArgumentList(out arguments);
				}
				Expect(Tokens.RPAR );
				rparRange = tToken.Range;
			}
			result = CreateObjectCreationExpression(typeReference, arguments, lparRange, rparRange, startRange, tToken.Range); 
		}
		else if (la.Type == Tokens.FUNCTION )
		{
			FunctionExpression(out result);
		}
		else
			SynErr(98);
	}
	void ArgumentList(out ExpressionCollection arguments)
	{
		arguments = new ExpressionCollection();
		Expression expression = null;
		AssignmentExpression(out expression);
		if (expression != null)
		 arguments.Add(expression);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			AssignmentExpression(out expression);
			if (expression != null)
			 arguments.Add(expression);
		}
	}
	void FunctionExpression(out Expression result)
	{
		result = null;
		bool isConstructor = tToken.Type == Tokens.NEW;
		LanguageElementCollection parameters = null;
		SourceRange startRange = SourceRange.Empty;
		if (isConstructor)
		  startRange = tToken.Range;
		else
		  startRange = la.Range;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Token nameToken = null;
		Expect(Tokens.FUNCTION );
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			Identifier();
			nameToken = tToken; 
		}
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		if (la.Type == Tokens.IDENTIFIER  || la.Type == Tokens.CODEEMBEDDING )
		{
			FormalParameterList(out parameters);
		}
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		AnonymousMethodExpression anonymousMethod = null;
		if (isConstructor)
		  anonymousMethod = CreateAnonymousConstructor(startRange, lparRange, rparRange, parameters, nameToken);
		else
		  anonymousMethod = CreateAnonymousMethod(startRange, lparRange, rparRange, parameters, nameToken);
		LanguageElement oldContext = Context;
		SetContext(anonymousMethod);
		Expect(Tokens.LBRACE );
		ReadBlockStart(tToken.Range);
		FunctionBody();
		Expect(Tokens.RBRACE );
		ReadBlockEnd(tToken.Range);
		anonymousMethod.SetRange(GetRange(anonymousMethod,tToken));
		SetContext(oldContext);
		result = anonymousMethod; 
	}
	void MemberExpression(out Expression result)
	{
		result = null;
		Expression expression = null;
		if (StartOf(9))
		{
			PrimaryExpression(out result);
		}
		else if (la.Type == Tokens.FUNCTION )
		{
			FunctionExpression(out result);
		}
		else if (la.Type == Tokens.NEW )
		{
			NewExpression(out result);
		}
		else
			SynErr(99);
		while (la.Type == Tokens.LPAR  || la.Type == Tokens.LBRACK  || la.Type == Tokens.DOT )
		{
			if (la.Type == Tokens.LBRACK )
			{
				Get();
				Expression(out expression);
				Expect(Tokens.RBRACK );
				result = CreateIndexerExpression(result, expression, tToken.Range);
			}
			else if (la.Type == Tokens.DOT )
			{
				Get();
				Identifier();
				result = CreateElementReference(result, tToken); 
			}
			else
			{
				Get();
				SourceRange lparRange = tToken.Range; 
				ExpressionCollection arguments = null; 
				if (StartOf(2))
				{
					ArgumentList(out arguments);
				}
				Expect(Tokens.RPAR );
				SourceRange rparRange = tToken.Range;
				result = CreateMethodCallExpression(result, arguments, lparRange, rparRange);
			}
		}
	}
	void PostfixExpression(out Expression result)
	{
		result = null;
		MemberExpression(out result);
		if (la.Type == Tokens.PLUSPLUS  || la.Type == Tokens.MINUSMINUS )
		{
			if (la.Type == Tokens.PLUSPLUS )
			{
				Get();
				result = CreateUnaryPostfixIncrement(result, tToken); 
			}
			else
			{
				Get();
				result = CreateUnaryPostfixDecrement(result, tToken); 
			}
		}
	}
	void UnaryExpression(out Expression result)
	{
		result = null;
		Token operatorToken = la;
		if (StartOf(10))
		{
			PostfixExpression(out result);
		}
		else if (StartOf(11))
		{
			switch (la.Type)
			{
			case Tokens.DELETE : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateDeleteExpression(result, operatorToken); 
				break;
			}
			case Tokens.EXCLAMATIONSYMBOL : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateLogicalInversion(result, operatorToken); 
				break;
			}
			case Tokens.TYPEOF : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateTypeOfExpression(result, operatorToken); 
				break;
			}
			case Tokens.PLUSPLUS : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryPrefixIncrement(result, operatorToken); 
				break;
			}
			case Tokens.MINUSMINUS : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryPrefixDecrement(result, operatorToken); 
				break;
			}
			case Tokens.TILDE : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryExpression(result, operatorToken); 
				break;
			}
			case Tokens.VOID : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryExpression(result, operatorToken); 
				break;
			}
			case Tokens.PLUS : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryExpression(result, operatorToken); 
				break;
			}
			case Tokens.MINUS : 
			{
				Get();
				UnaryExpression(out result);
				result = CreateUnaryExpression(result, operatorToken); 
				break;
			}
			}
		}
		else
			SynErr(100);
	}
	void MultiplicativeExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		UnaryExpression(out result);
		while (la.Type == Tokens.ASTERISK  || la.Type == Tokens.PERCENTSYMBOL  || la.Type == Tokens.SLASH )
		{
			if (la.Type == Tokens.ASTERISK )
			{
				Get();
			}
			else if (la.Type == Tokens.SLASH )
			{
				Get();
			}
			else
			{
				Get();
			}
			operatorToken = tToken;
			UnaryExpression(out rightPart);
			result = CreateBinaryOperatorExpression(result, rightPart, operatorToken); 
		}
	}
	void AdditiveExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		int mulExpCount = 0;
		MultiplicativeExpression(out result);
		while (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
		{
			if (la.Type == Tokens.PLUS )
			{
				Get();
			}
			else
			{
				Get();
			}
			operatorToken = tToken;
			mulExpCount++;
			MultiplicativeExpression(out rightPart);
			if (mulExpCount < _MaxMult)
			 result = CreateBinaryOperatorExpression(result, rightPart, operatorToken);
		}
	}
	void ShiftExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		AdditiveExpression(out result);
		while (la.Type == Tokens.SHIFTLEFT  || la.Type == Tokens.SHIFTRIGHT  || la.Type == Tokens.TRIPLESHIFTRIGHT )
		{
			if (la.Type == Tokens.SHIFTLEFT )
			{
				Get();
			}
			else if (la.Type == Tokens.SHIFTRIGHT )
			{
				Get();
			}
			else
			{
				Get();
			}
			operatorToken = tToken; 
			AdditiveExpression(out rightPart);
			result = CreateBinaryOperatorExpression(result, rightPart, operatorToken); 
		}
	}
	void RelationalExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		ShiftExpression(out result);
		if (la.Type == Tokens.IN && InsideForEachLoop)
		 return;
		while (StartOf(12))
		{
			operatorToken = la; 
			if (la.Type == Tokens.IN )
			{
				Get();
				ShiftExpression(out rightPart);
				result = CreateBinaryOperatorExpression(result, rightPart, operatorToken); 
			}
			else if (la.Type == Tokens.INSTANCEOF )
			{
				Get();
				ShiftExpression(out rightPart);
				result = CreateTypeCheckExpression(result, rightPart, operatorToken); 
			}
			else
			{
				if (la.Type == Tokens.LESSTHAN )
				{
					Get();
				}
				else if (la.Type == Tokens.GREATERTHAN )
				{
					Get();
				}
				else if (la.Type == Tokens.GREATEROREQUAL )
				{
					Get();
				}
				else if (la.Type == Tokens.LESSOREQUAL )
				{
					Get();
				}
				else
					SynErr(101);
				ShiftExpression(out rightPart);
				result = CreateRelationalOperation(result, rightPart, operatorToken); 
			}
		}
	}
	void EqualityExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		RelationalExpression(out result);
		while (StartOf(13))
		{
			if (la.Type == Tokens.DOUBLEEQUALS )
			{
				Get();
			}
			else if (la.Type == Tokens.NOTEQUALS )
			{
				Get();
			}
			else if (la.Type == Tokens.TRIPLEEQUALS )
			{
				Get();
			}
			else
			{
				Get();
			}
			operatorToken = tToken;
			RelationalExpression(out rightPart);
			result = CreateRelationalOperation(result, rightPart, operatorToken); 
		}
	}
	void BitwiseAndExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		EqualityExpression(out result);
		while (la.Type == Tokens.BITAND )
		{
			Get();
			operatorToken = tToken; 
			EqualityExpression(out rightPart);
			result = CreateLogicalOperation(result, rightPart, operatorToken); 
		}
	}
	void BitwiseXorExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		BitwiseAndExpression(out result);
		while (la.Type == Tokens.XORSYMBOL )
		{
			Get();
			operatorToken = tToken;
			BitwiseAndExpression(out rightPart);
			result = CreateLogicalOperation(result, rightPart, operatorToken); 
		}
	}
	void BitwiseOrExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		BitwiseXorExpression(out result);
		while (la.Type == Tokens.BITOR )
		{
			Get();
			operatorToken = tToken;
			BitwiseXorExpression(out rightPart);
			result = CreateLogicalOperation(result, rightPart, operatorToken); 
		}
	}
	void LogicalAndExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		BitwiseOrExpression(out result);
		while (la.Type == Tokens.ANDAND )
		{
			Get();
			operatorToken = tToken;
			BitwiseOrExpression(out rightPart);
			result = CreateLogicalOperation(result, rightPart, operatorToken); 
		}
	}
	void LogicalOrExpression(out Expression result)
	{
		Expression rightPart = null;
		Token operatorToken = null;
		result = null;
		LogicalAndExpression(out result);
		while (la.Type == Tokens.OROR )
		{
			Get();
			operatorToken = tToken;
			LogicalAndExpression(out rightPart);
			result = CreateLogicalOperation(result, rightPart, operatorToken); 
		}
	}
	void ConditionalExpression(out Expression result)
	{
		Expression trueExpression = null;
		Expression falseExpression = null;
		result = null;
		LogicalOrExpression(out result);
		if (la.Type == Tokens.QUESTIONSYMBOL )
		{
			Get();
			AssignmentExpression(out trueExpression);
			Expect(Tokens.COLON );
			AssignmentExpression(out falseExpression);
			result = CreateConditionalExpression(result, trueExpression, falseExpression);
		}
	}
	void AssignmentOperator()
	{
		switch (la.Type)
		{
		case Tokens.EQUALSSYMBOL : 
		{
			Get();
			break;
		}
		case Tokens.MULEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.DIVEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.MODEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.PLUSEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.MINUSEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.SHIFTLEFTEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.SHIFTRIGHTEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.TRIPLESHIFTRIGHTEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.ANDEQUAL : 
		{
			Get();
			break;
		}
		case Tokens.OREQUAL : 
		{
			Get();
			break;
		}
		case Tokens.XOREQUAL : 
		{
			Get();
			break;
		}
		default: SynErr(102); break;
		}
	}
		void Parse()
		{
			la = new Token();
			la.Value = "";
			Get();
			Parser();
			Expect(0);
			if (Context != null)
				Context.SetRange(GetRange(Context, tToken));
			CloseContext();
			BindComments();
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
			{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,x, x,T,x,T, T,x,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,T,x, T,x,x,T, T,T,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,T,T, T,x,x,T, T,T,T,x, T,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,T,x, T,x,x,T, T,T,T,x, T,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, x,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,T,x, T,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}
			};
			return set;
		}
	} 
	public class JavaScriptParserErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "IDENTIFIER expected"; break;
			case 2: s = "CODEEMBEDDING expected"; break;
			case 3: s = "INTEGERLITERAL expected"; break;
			case 4: s = "DECIMALLITERAL expected"; break;
			case 5: s = "STRINGLITERAL expected"; break;
			case 6: s = "BREAK expected"; break;
			case 7: s = "CASE expected"; break;
			case 8: s = "CATCH expected"; break;
			case 9: s = "CONTINUE expected"; break;
			case 10: s = "DEFAULT expected"; break;
			case 11: s = "DELETE expected"; break;
			case 12: s = "DO expected"; break;
			case 13: s = "ELSE expected"; break;
			case 14: s = "FINALLY expected"; break;
			case 15: s = "FOR expected"; break;
			case 16: s = "FUNCTION expected"; break;
			case 17: s = "IFKEYWORD expected"; break;
			case 18: s = "IN expected"; break;
			case 19: s = "INSTANCEOF expected"; break;
			case 20: s = "NEW expected"; break;
			case 21: s = "RETURN expected"; break;
			case 22: s = "SWITCH expected"; break;
			case 23: s = "THIS expected"; break;
			case 24: s = "THROW expected"; break;
			case 25: s = "TRY expected"; break;
			case 26: s = "TYPEOF expected"; break;
			case 27: s = "VAR expected"; break;
			case 28: s = "VOID expected"; break;
			case 29: s = "WHILE expected"; break;
			case 30: s = "WITH expected"; break;
			case 31: s = "NULL expected"; break;
			case 32: s = "TRUE expected"; break;
			case 33: s = "FALSE expected"; break;
			case 34: s = "LBRACE expected"; break;
			case 35: s = "RBRACE expected"; break;
			case 36: s = "LPAR expected"; break;
			case 37: s = "RPAR expected"; break;
			case 38: s = "LBRACK expected"; break;
			case 39: s = "RBRACK expected"; break;
			case 40: s = "DOT expected"; break;
			case 41: s = "SEMICOLON expected"; break;
			case 42: s = "COMMA expected"; break;
			case 43: s = "LESSTHAN expected"; break;
			case 44: s = "GREATERTHAN expected"; break;
			case 45: s = "LESSOREQUAL expected"; break;
			case 46: s = "GREATEROREQUAL expected"; break;
			case 47: s = "DOUBLEEQUALS expected"; break;
			case 48: s = "NOTEQUALS expected"; break;
			case 49: s = "TRIPLEEQUALS expected"; break;
			case 50: s = "NOTDOUBLEEQUALS expected"; break;
			case 51: s = "PLUS expected"; break;
			case 52: s = "MINUS expected"; break;
			case 53: s = "ASTERISK expected"; break;
			case 54: s = "PERCENTSYMBOL expected"; break;
			case 55: s = "PLUSPLUS expected"; break;
			case 56: s = "MINUSMINUS expected"; break;
			case 57: s = "SHIFTLEFT expected"; break;
			case 58: s = "SHIFTRIGHT expected"; break;
			case 59: s = "TRIPLESHIFTRIGHT expected"; break;
			case 60: s = "BITAND expected"; break;
			case 61: s = "BITOR expected"; break;
			case 62: s = "XORSYMBOL expected"; break;
			case 63: s = "EXCLAMATIONSYMBOL expected"; break;
			case 64: s = "TILDE expected"; break;
			case 65: s = "ANDAND expected"; break;
			case 66: s = "OROR expected"; break;
			case 67: s = "QUESTIONSYMBOL expected"; break;
			case 68: s = "COLON expected"; break;
			case 69: s = "EQUALSSYMBOL expected"; break;
			case 70: s = "PLUSEQUAL expected"; break;
			case 71: s = "MINUSEQUAL expected"; break;
			case 72: s = "MULEQUAL expected"; break;
			case 73: s = "MODEQUAL expected"; break;
			case 74: s = "SHIFTLEFTEQUAL expected"; break;
			case 75: s = "SHIFTRIGHTEQUAL expected"; break;
			case 76: s = "TRIPLESHIFTRIGHTEQUAL expected"; break;
			case 77: s = "ANDEQUAL expected"; break;
			case 78: s = "OREQUAL expected"; break;
			case 79: s = "XOREQUAL expected"; break;
			case 80: s = "SLASH expected"; break;
			case 81: s = "DIVEQUAL expected"; break;
			case 82: s = "SINGLELINECOMMENT expected"; break;
			case 83: s = "MULTILINECOMMENT expected"; break;
			case 84: s = "XMLCOMMENT expected"; break;
			case 85: s = "REGEXPLITERAL expected"; break;
			case 86: s = "??? expected"; break;
			case 87: s = "invalid SourceElement"; break;
			case 88: s = "invalid Statement"; break;
			case 89: s = "invalid Identifier"; break;
			case 90: s = "invalid ForStatements"; break;
			case 91: s = "invalid ForStatements"; break;
			case 92: s = "invalid CaseClause"; break;
			case 93: s = "invalid PrimaryExpression"; break;
			case 94: s = "invalid Literal"; break;
			case 95: s = "invalid PropertyName"; break;
			case 96: s = "invalid TypeReference"; break;
			case 97: s = "invalid TypeReference"; break;
			case 98: s = "invalid NewExpression"; break;
			case 99: s = "invalid MemberExpression"; break;
			case 100: s = "invalid UnaryExpression"; break;
			case 101: s = "invalid RelationalExpression"; break;
			case 102: s = "invalid AssignmentOperator"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
