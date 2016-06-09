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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class CStatementCodeGenBase : StatementCodeGenBase
  {
	public CStatementCodeGenBase(CodeGen codeGen)
	  :
	  base(codeGen)
	{
	}
	bool HasChildVariables(Statement statement)
	{
	  if (statement == null)
		return false;
	  int count = statement.NodeCount;
	  for (int i = 0; i < count; i++)
	  {
		object node = statement.Nodes[i];
		if (node is BaseVariable)
		  return true;
	  }
	  return false;
	}
	void GenerateInitializers(LanguageElementList initializers)
	{
	  if (initializers == null)
		return;
	  for (int i = 0; i < initializers.Count; i++)
	  {
		LanguageElement initializer = initializers[i];
		if (IsSkiped(initializer))
		  continue;
		CodeGen.GenerateElement(initializer);
		if (i > 0 && !(initializer is BaseVariable))
		  CodeGen.Code.Write(GetCommaDelimeter());
	  }
	}
	protected bool IsIfStatementWithElseAndNestedIf(Statement statement)
	{
	  If ifStatement = statement as If;
	  if (ifStatement == null)
		return false;
	  Else elseStatement = ifStatement.ElseStatement as Else;
	  if (elseStatement == null)
		return false;
	  If nestedIf = ifStatement.FindChildByElementType(LanguageElementType.If) as If;
	  return nestedIf != null && nestedIf.Parent == statement;
	}
	protected bool NeedToAddBraces(Statement statement, CurlyBraceOption option)
	{
	  Block block = statement as Block;
	  if (block != null && block.ForceDelimiters)
		return true;
	  if (IsIfStatementWithElseAndNestedIf(statement))
		return true;
	  int statementsCount = ParserUtils.CountChildStatements(statement);
	  if (statementsCount == 1 && HasChildVariables(statement))
		return true;
	  switch (option)
	  {
		case CurlyBraceOption.None:
		  return false;
		case CurlyBraceOption.Always:
		  return true;
		case CurlyBraceOption.MoreThanOneNode:
		  return statementsCount > 1 || statementsCount == 0;
	  }
	  return true;
	}
	protected void GenerateBlockStatementContents(Statement statement)
	{
	  GenerateBlockStatementContents(statement, CurlyBraceOption.MoreThanOneNode);
	}
	protected void GenerateBlockStatementContents(Statement statement, CurlyBraceOption option)
	{
	  GenerateBlockStatementContents(statement, option, true);
	}
	protected void GenerateBlockStatementContents(Statement statement, CurlyBraceOption option, bool increaseIndent)
	{
	  GenerateBlockStatementContents(statement, option, increaseIndent, false);
	}
	protected void GenerateBlockStatementContents(Statement statement, CurlyBraceOption option, bool increaseIndent, bool needWriteLine)
	{
	  bool addBraces = NeedToAddBraces(statement, option);
	  if (addBraces)
	  {
		bool onNewLine = Options.LineBreaks.PlaceOpenBraceOnNewLineForCodeBlocks && statement.ElementType != LanguageElementType.Block;
		bool spaceBefore = Options.Spacing.BeforeOpenCurlyBraceOnTheSameLine && !onNewLine && !string.IsNullOrEmpty(Code.LastLineInSpacesWithouIndent);
		WriteOpenBrace(onNewLine, spaceBefore, false, false);
	  }
	  Code.WriteLine();
	  bool hasNodes = statement.NodeCount > 0;
	  if (hasNodes)
	  {
		if (increaseIndent)
		  Code.IncreaseIndent();
		GenerateElementCollection(statement.Nodes, "", true);
		if (increaseIndent)
		  Code.DecreaseIndent();
	  }
	  if (addBraces)
		WriteCloseBrace(Options.LineBreaks.PlaceCloseBraceOnNewLineForCodeBlocks && hasNodes, false, false);
	  if (needWriteLine)
		Code.WriteLine();
	}
	protected void GenerateParentingStatement(ParentingStatement statement, string keyword, string delimiter)
	{
	  GenerateParentingStatement(statement, keyword, delimiter, CurlyBraceOption.MoreThanOneNode);
	}
	protected void GenerateParentingStatement(ParentingStatement statement, string keyword, string delimiter, CurlyBraceOption option, bool increaseIndent)
	{
	  Code.Write(keyword + " (");
	  GenerateElementCollection(statement.DetailNodes, delimiter);
	  Code.Write(")");
	  GenerateBlockStatementContents(statement, option, increaseIndent);
	}
	protected void GenerateParentingStatement(ParentingStatement statement, string keyword, string delimiter, CurlyBraceOption option)
	{
	  Code.Write(keyword + " (");
	  GenerateElementCollection(statement.DetailNodes, delimiter);
	  Code.Write(")");
	  GenerateBlockStatementContents(statement, option);
	}
	protected virtual void GenerateLabelStatement(Label statement)
	{
	  bool indentLabels = Options.Indention.IndentLabels;
	  int indentLevel = Code.IndentLevel;
	  if (!indentLabels)
		Code.IndentLevel = 0;
	  Code.Write(statement.Name);
	  if (Options.Spacing.BeforeColonInLabels)
		Code.Write(" ");
	  Code.Write(":");
	  if (!indentLabels)
		Code.IndentLevel = indentLevel;
	}
	protected override void GenerateOptionStatement(OptionStatement statement)
	{
	}
	protected override bool GenerateStatement(Statement statement)
	{
	  bool lResult = base.GenerateStatement(statement);
	  if (lResult)
		return lResult;
	  if (statement.ElementType == LanguageElementType.Label)
	  {
		GenerateLabelStatement(statement as Label);
		return true;
	  }
	  return false;
	}
	protected override void GenerateAbortStatement(Abort statement)
	{
	}
	protected override void GenerateGotoStatement(Goto statement)
	{
	  Code.Write("goto ");
	  if (statement.IsGotoCaseLabel)
		Code.Write("case ");
	  if (statement.IsGotoCaseDefault)
		Code.Write("default");
	  Code.Write(statement.Label);
	  WriteSemicolon(false);
	}
	protected override void GenerateBreakStatement(Break statement)
	{
	  Code.Write("break");
	  WriteSemicolon(false);
	}
	protected override void GenerateContinueStatement(Continue statement)
	{
	  Code.Write("continue");
	  WriteSemicolon(false);
	}
	protected override void GenerateStatementStatement(Statement statement)
	{
	  if (statement.SourceExpression != null)
		CodeGen.GenerateCode(Code, statement.SourceExpression);
	  else
		Code.Write(statement.Name);
	  if (!statement.IsDetailNode)
		WriteSemicolon(false);
	}
	protected override void GenerateMethodCallStatement(MethodCall statement)
	{
	  CodeGen.GenerateCode(Code, statement.Qualifier);
	  Code.Write("(");
	  GenerateParameters(statement.Arguments, GetCommaDelimeter());
	  Code.Write(")");
	  WriteSemicolon(false);
	}
	protected override void GenerateAssignmentStatement(Assignment statement)
	{
	  CodeGen.GenerateCode(Code, statement.LeftSide);
	  CExpressionCodeGenBase.GenerateAssignmentOperatorText(Code, statement.AssignmentOperator);
	  CodeGen.GenerateCode(Code, statement.Expression);
	  WriteSemicolon(false);
	}
	protected override void GenerateSwitchStatement(Switch statement)
	{
	  bool caseStatementFromSwitchStatementIdention = Options.Indention.CaseStatementFromSwitchStatement;
	  Code.Write("switch");
	  if (Options.Spacing.BeforeSwitchParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  bool withinSwitchParenthesesSpacing = Options.Spacing.WithinSwitchParentheses;
	  if (withinSwitchParenthesesSpacing)
		Code.Write(" ");
	  GenerateElementCollection(statement.DetailNodes, GetCommaDelimeter());
	  if (withinSwitchParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	  GenerateBlockStatementContents(statement, CurlyBraceOption.Always, caseStatementFromSwitchStatementIdention);
	}
	protected override void GenerateCaseStatement(Case statement)
	{
	  if (statement.IsDefault)
		Code.Write("default");
	  else
	  {
		Code.Write("case ");
		CodeGen.GenerateCode(Code, statement.Expression);
	  }
	  if (Options.Spacing.BeforeColonInCaseStatement)
		Code.Write(" ");
	  Code.Write(":");
	  bool caseStatementContentsIdention = Options.Indention.CaseStatementContents;
	  if (caseStatementContentsIdention)
		Code.IncreaseIndent();
	  int nodesCount = statement.NodeCount;
	  LanguageElement firstChild = nodesCount > 0 ? statement.Nodes[0] as LanguageElement : null;
	  if (!Options.LineBreaks.PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement &&
		  nodesCount == 1 && firstChild is Block)
	  {
		bool indention = Options.Indention.CodeBlockContents;
		WriteOpenBrace(Options.LineBreaks.PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement, false, false, indention);
		if (firstChild.NodeCount > 0)
		  Code.WriteLine();
		GenerateElementCollection(firstChild.Nodes, Environment.NewLine);
		WriteCloseBrace(Options.LineBreaks.PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement, false, indention);
	  }
	  else
	  {
		if (nodesCount > 0)
		  Code.WriteLine();
		GenerateElementCollection(statement.Nodes, Environment.NewLine);
	  }
	  if (caseStatementContentsIdention)
		Code.DecreaseIndent();
	}
	protected override void GenerateTryStatement(Try statement)
	{
	  Code.Write("try");
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  GenerateBlockStatementContents(statement, CurlyBraceOption.Always, codeBlockContentsIdention, false);
	  LanguageElement sibling = statement.NextCodeSibling;
	  while (sibling != null && (sibling.ElementType == LanguageElementType.Catch || sibling.ElementType == LanguageElementType.Finally))
	  {
		CodeGen.GenerateCode(Code, sibling);
		CodeGen.AddSkiped(sibling);
		sibling = sibling.NextCodeSibling;
	  }
	}
	protected override void GenerateCatchStatement(Catch statement)
	{
	  if (Options.LineBreaks.PlaceCatchStatementOnNewLine)
		Code.WriteLine();
	  else
		Code.Write(" ");
	  Code.Write("catch");
	  if (statement.DetailNodeCount > 0)
	  {
		if (Options.Spacing.BeforeCatchParentheses)
		  Code.Write(" ");
		Code.Write("(");
		bool withinCatchParenthesesSpacing = Options.Spacing.WithinCatchParentheses;
		if (withinCatchParenthesesSpacing)
		  Code.Write(" ");
		GenerateElementCollection(statement.DetailNodes, GetCommaDelimeter());
		if (withinCatchParenthesesSpacing)
		  Code.Write(" ");
		Code.Write(")");
	  }
	  GenerateBlockStatementContents(statement, CurlyBraceOption.Always, Options.Indention.CodeBlockContents);
	}
	protected override void GenerateFinallyStatement(Finally statement)
	{
	  if (Options.LineBreaks.PlaceFinallyStatementOnNewLine)
		Code.WriteLine();
	  else
		Code.Write(" ");
	  Code.Write("finally");
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  GenerateBlockStatementContents(statement, CurlyBraceOption.Always, codeBlockContentsIdention);
	}
	protected override void GenerateUsingStatement(UsingStatement statement)
	{
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  CurlyBraceOption curlyBraceOptions = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOptions = CurlyBraceOption.Always;
	  GenerateParentingStatement(statement, "using", GetCommaDelimeter(), curlyBraceOptions, codeBlockContentsIdention);
	}
	protected override void GenerateDoStatement(Do statement)
	{
	  Code.Write("do");
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  CurlyBraceOption curlyBraceOptionMoreThanOneNode = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOptionMoreThanOneNode = CurlyBraceOption.Always;
	  bool newLine = (curlyBraceOptionMoreThanOneNode != CurlyBraceOption.Always && statement.NodeCount == 1) ||
		  Options.LineBreaks.PlaceWhileStatementOnNewLine;
	  GenerateBlockStatementContents(statement, curlyBraceOptionMoreThanOneNode, codeBlockContentsIdention, newLine);
	  Code.Write("while");
	  if (Options.Spacing.BeforeWhileParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  CodeGen.GenerateCode(Code, statement.Condition);
	  Code.Write(")");
	  WriteSemicolon(false);
	}
	protected override void GenerateForStatement(For statement)
	{
	  Code.Write("for");
	  if (Options.Spacing.BeforeForForeachParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  bool withinForForeachParenthesesSpacing = Options.Spacing.WithinForForeachParentheses;
	  if (withinForForeachParenthesesSpacing)
		Code.Write(" ");
	  GenerateInitializers(statement.Initializers);
	  bool before = Options.Spacing.BeforeSemicolonInForStatement;
	  bool after = Options.Spacing.AfterSemicolonInForStatement;
	  WriteWhithing(before, ";", after);
	  CodeGen.GenerateElement(statement.Condition);
	  WriteWhithing(before, ";", after);
	  GenerateElementCollection(statement.Incrementors, GetCommaDelimeter());
	  if (withinForForeachParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	  CurlyBraceOption curlyBrace = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBrace = CurlyBraceOption.Always;
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  GenerateBlockStatementContents(statement, curlyBrace, codeBlockContentsIdention);
	}
	protected override void GenerateForEachStatement(ForEach statement)
	{
	  System.Diagnostics.Debug.Assert(statement.DetailNodeCount == 2);
	  Code.Write("foreach");
	  if (Options.Spacing.BeforeForForeachParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  bool withinForForeachParenthesesSpacing = Options.Spacing.WithinForForeachParentheses;
	  if (withinForForeachParenthesesSpacing)
		Code.Write(" ");
	  CodeGen.GenerateCode(Code, (LanguageElement)statement.DetailNodes[0]);
	  Code.Write(" in ");
	  CodeGen.GenerateCode(Code, (LanguageElement)statement.DetailNodes[1]);
	  if (withinForForeachParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	  CurlyBraceOption curlyBraceOption = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOption = CurlyBraceOption.Always;
	  GenerateBlockStatementContents(statement, curlyBraceOption, Options.Indention.CodeBlockContents);
	}
	protected override void GenerateWhileStatement(While statement)
	{
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  Code.Write("while");
	  if (Options.Spacing.BeforeWhileParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  bool withinWhileParenthesesSpacing = Options.Spacing.WithinWhileParentheses;
	  if (withinWhileParenthesesSpacing)
		Code.Write(" ");
	  GenerateElementCollection(statement.DetailNodes, string.Empty);
	  if (withinWhileParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	  CurlyBraceOption curlyBraceOptionMoreThanOneNode = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOptionMoreThanOneNode = CurlyBraceOption.Always;
	  if (curlyBraceOptionMoreThanOneNode != CurlyBraceOption.Always && Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && statement.NodeCount == 1)
		CodeGen.GenerateElement((LanguageElement)statement.Nodes[0]);
	  else
		GenerateBlockStatementContents(statement, curlyBraceOptionMoreThanOneNode, codeBlockContentsIdention);
	}
	protected override void GenerateIfStatement(If statement)
	{
	  Code.Write("if");
	  bool withinIfParenthesesSpacing = Options.Spacing.WithinIfParentheses;
	  WriteOpenParen(Options.Spacing.BeforeIfParentheses, withinIfParenthesesSpacing);
	  GenerateElementCollection(statement.DetailNodes, string.Empty);
	  WriteCloseParen(withinIfParenthesesSpacing);
	  CurlyBraceOption curlyBraceOption = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOption = CurlyBraceOption.Always;
	  Else nextElse = statement.NextCodeSibling as Else;
	  bool needNewLine = nextElse != null && Options.LineBreaks.PlaceElseStatementOnNewLine;
	  if (curlyBraceOption != CurlyBraceOption.Always && Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && statement.NodeCount == 1)
		CodeGen.GenerateElement((LanguageElement)statement.Nodes[0]);
	  else
		GenerateBlockStatementContents(statement, curlyBraceOption, Options.Indention.CodeBlockContents, needNewLine);
	  if (nextElse != null)
	  {
		CodeGen.GenerateElement(nextElse);
		CodeGen.AddSkiped(nextElse);
	  }
	}
	protected override void GenerateElseIfStatement(ElseIf statement)
	{
	  GenerateParentingStatement(statement, "else if", " ");
	}
	protected override void GenerateElseStatement(Else statement)
	{
	  if (!Options.LineBreaks.PlaceElseStatementOnNewLine && !string.IsNullOrEmpty(Code.LastLineInSpacesWithouIndent))
		Code.Write(" ");
	  Code.Write("else");
	  CurlyBraceOption curlyBraceOption = CurlyBraceOption.MoreThanOneNode;
	  if (!statement.BlockStart.IsEmpty)
		curlyBraceOption = CurlyBraceOption.Always;
	  if (curlyBraceOption != CurlyBraceOption.Always && Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && statement.NodeCount == 1)
	  {
		Code.Write(" ");
		CodeGen.GenerateElement((LanguageElement)statement.Nodes[0]);
	  }
	  else
		GenerateBlockStatementContents(statement, curlyBraceOption, Options.Indention.CodeBlockContents);
	}
	protected override void GenerateEmptyStatement(EmptyStatement statement)
	{
	  GenerateBlockStatementContents(statement, CurlyBraceOption.None);
	}
	protected override void GenerateBlockStatement(Block statement)
	{
	  bool codeBlockContentsIdention = Options.Indention.CodeBlockContents;
	  GenerateBlockStatementContents(statement, CurlyBraceOption.Always, codeBlockContentsIdention);
	}
	protected override void GenerateReturnStatement(Return statement)
	{
	  Code.Write("return");
	  if (statement.Expression != null)
		Code.Write(" ");
	  CodeGen.GenerateCode(Code, statement.Expression);
	  WriteSemicolon(false);
	}
  }
}
