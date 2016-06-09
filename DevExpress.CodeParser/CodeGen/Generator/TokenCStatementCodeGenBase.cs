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
  public abstract class TokenCStatementCodeGenBase : StatementCodeGenBase
  {
	public TokenCStatementCodeGenBase(CodeGen codeGen)
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
	static bool HasAtLeastOneSnipppet(LanguageElement element)
	{
	  if (element == null)
		return false;
	  NodeList list = element.Nodes;
	  if (list == null)
		return false;
	  foreach (LanguageElement node in list)
	  {
		SnippetCodeElement snippet = node as SnippetCodeElement;
		if (snippet == null)
		  continue;
		if (IsSystemSnippet(snippet))
		  continue;
		return true;
	  }
	  return false;
	}
	static bool IsSystemSnippet(SnippetCodeElement element)
	{
	  if (element == null)
		return false;
	  string name = element.Name;
	  return name == "«Caret»" || name == "«BlockAnchor»";
	}
	protected static bool IsIfStatementWithElseAndNestedIf(Statement statement)
	{
	  If ifStatement = statement as If;
	  if (ifStatement == null)
		return false;
	  Else elseStatement = ifStatement.ElseStatement as Else;
	  if (elseStatement == null)
		return false;
	  If nestedIf = ifStatement.FindChildByElementType(LanguageElementType.If) as If;
	  return nestedIf != null && nestedIf.Parent == statement && nestedIf.ElseStatement == null;
	}
	protected void GenerateBlockStatementContents(Statement statement)
	{
	  bool alwaysHasBlock;
	  bool addBraces = NeedToAddBraces(statement, Options, out alwaysHasBlock);
	  if (addBraces)
	  {
		if (alwaysHasBlock)
		{
		  Write(FormattingTokenType.CurlyBraceOpen);
		  GenerateElementCollection(statement.Nodes);
		  Write(FormattingTokenType.CurlyBraceClose);
		  return;
		}
		TokenGenArgs tempArgs = null;
		bool hasCurlyOpen = (statement.Tokens != null) && statement.Tokens.HasElement(FormattingTokenType.CurlyBraceOpen);
		if (!hasCurlyOpen)
		  tempArgs = CodeGen.GetArgs().LogicForProcessComment();
		Write(FormattingTokenType.CurlyBraceOpen);
		if (!hasCurlyOpen)
		  CodeGen.RestoreSavedArgs(tempArgs);
		GenerateElementCollection(statement.Nodes);
		bool hasCurlyClose = (statement.Tokens != null) && statement.Tokens.HasElement(FormattingTokenType.CurlyBraceClose);
		if (!hasCurlyClose)
		  tempArgs = CodeGen.GetArgs().LogicForProcessComment();
		Write(FormattingTokenType.CurlyBraceClose);
		if (!hasCurlyClose)
		  CodeGen.RestoreSavedArgs(tempArgs);
	  }
	  else
		GenerateElementCollection(statement.Nodes);
	}
	protected void GenerateParentingStatement(ParentingStatement statement, FormattingTokenType keyword, FormattingTokenType delimiter)
	{
	  GenerateParentingStatement(statement, statement.DetailNodes, keyword, delimiter);
	}
	protected void GenerateParentingStatement(ParentingStatement statement, NodeList collection, FormattingTokenType keyword, FormattingTokenType delimiter)
	{
	  Write(keyword);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(collection, delimiter);
	  Write(FormattingTokenType.ParenClose);
	  GenerateBlockStatementContents(statement);
	  CodeGen.GenerateContextBackComment();
	}
	protected virtual void GenerateLabelStatement(Label statement)
	{
	  using (GetClearIndent(!Options.Indention.IndentLabels))
	  {
		Write(FormattingTokenType.Ident);
		Write(FormattingTokenType.Colon);
	  }
	}
	protected override void GenerateUnsafeStatement(UnsafeStatement statement)
	{
	  Write(FormattingTokenType.Unsafe);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateOptionStatement(OptionStatement statement)
	{
	}
	protected override bool GenerateStatement(Statement statement)
	{
	  bool lResult = base.GenerateStatement(statement);
	  if (lResult)
		return lResult;
	  if (statement.ElementType != LanguageElementType.Label)
		return false;
	  GenerateLabelStatement(statement as Label);
	  return true;
	}
	protected override void GenerateAbortStatement(Abort statement)
	{
	}
	protected override void GenerateGotoStatement(Goto statement)
	{
	  Write(FormattingTokenType.GoTo);
	  if (statement.IsGotoCaseLabel)
		Write(FormattingTokenType.Case);
	  if (statement.IsGotoCaseDefault)
		Write(FormattingTokenType.Default);
	  else
		Write(statement.Label);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateBreakStatement(Break statement)
	{
	  Write(FormattingTokenType.Break);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateContinueStatement(Continue statement)
	{
	  Write(FormattingTokenType.Continue);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateStatementStatement(Statement statement)
	{
	  if (statement.SourceExpression != null)
		CodeGen.GenerateElement(statement.SourceExpression);
	  else
		Write(FormattingTokenType.Ident);
	  if (!statement.IsDetailNode)
		Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateMethodCallStatement(MethodCall statement)
	{
	  CodeGen.GenerateElement(statement.Qualifier);
	  GenerateParameters(statement.Arguments);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateAssignmentStatement(Assignment statement)
	{
	  CodeGen.GenerateElement(statement.LeftSide);
	  TokenCExpressionCodeGenBase.GenerateAssignmentOperatorText(this);
	  CodeGen.GenerateElement(statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateSwitchStatement(Switch statement)
	{
	  GenerateParentingStatement(statement, FormattingTokenType.Switch, FormattingTokenType.Comma);
	}
	protected override void GenerateCaseStatement(Case statement)
	{
	  if (statement.IsDefault)
		Write(FormattingTokenType.Default);
	  else
	  {
		Write(FormattingTokenType.Case);
		CodeGen.GenerateElement(statement.Expression);
	  }
	  Write(FormattingTokenType.Colon);
	  GenerateElementCollection(statement.Nodes);
	}
	protected override void GenerateTryStatement(Try statement)
	{
	  Write(FormattingTokenType.Try);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateCatchStatement(Catch statement)
	{
	  if (statement.DetailNodeCount > 0)
		GenerateParentingStatement(statement, FormattingTokenType.Catch, FormattingTokenType.Comma);
	  else
	  {
		Write(FormattingTokenType.Catch);
		GenerateBlockStatementContents(statement);
	  }
	}
	protected override void GenerateFinallyStatement(Finally statement)
	{
	  Write(FormattingTokenType.Finally);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateUsingStatement(UsingStatement statement)
	{
	  GenerateParentingStatement(statement, statement.Initializers, FormattingTokenType.Using, FormattingTokenType.Comma);
	}
	protected override void GenerateDoStatement(Do statement)
	{
	  Write(FormattingTokenType.Do);
	  GenerateBlockStatementContents(statement);
	  Write(FormattingTokenType.While);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(statement.Condition);
	  Write(FormattingTokenType.ParenClose);
	  Write(FormattingTokenType.Semicolon);
	}
	protected override void GenerateForStatement(For statement)
	{
	  Write(FormattingTokenType.For);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(statement.Initializers, FormattingTokenType.Comma);
	  Write(FormattingTokenType.Semicolon);
	  CodeGen.GenerateElement(statement.Condition);
	  Write(FormattingTokenType.Semicolon);
	  GenerateElementCollection(statement.Incrementors, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateForEachStatement(ForEach statement)
	{
	  GenerateParentingStatement(statement, FormattingTokenType.Foreach, FormattingTokenType.In);
	}
	protected override void GenerateWhileStatement(While statement)
	{
	  GenerateParentingStatement(statement, FormattingTokenType.While, FormattingTokenType.None);
	}
	protected override void GenerateIfStatement(If statement)
	{
	  GenerateParentingStatement(statement, FormattingTokenType.If, FormattingTokenType.None);
	}
	protected override void GenerateElseIfStatement(ElseIf statement)
	{
	  Write(FormattingTokenType.Else);
	  GenerateParentingStatement(statement, FormattingTokenType.If, FormattingTokenType.WhiteSpace);
	}
	protected override void GenerateElseStatement(Else statement)
	{
	  Write(FormattingTokenType.Else);
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateEmptyStatement(EmptyStatement statement)
	{
	  GenerateBlockStatementContents(statement);
	}
	protected override void GenerateBlockStatement(Block statement)
	{
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(statement.Nodes);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateReturnStatement(Return statement)
	{
	  Write(FormattingTokenType.Return);
	  CodeGen.GenerateCode(Code, statement.Expression);
	  Write(FormattingTokenType.Semicolon);
	}
	internal static bool NeedToAddBraces(Statement statement, CodeGenOptions options)
	{
	  bool alwaysHasBlock;
	  return NeedToAddBraces(statement, options, out alwaysHasBlock);
	}
	internal static bool NeedToAddBraces(Statement statement, CodeGenOptions options, out bool alwaysHasBlock)
	{
	  alwaysHasBlock = false;
	  if (statement == null || options == null)
		return false;
	  if (HasAtLeastOneSnipppet(statement))
		return true;
	  switch (statement.ElementType)
	  {
		case LanguageElementType.Try:
		case LanguageElementType.Catch:
		case LanguageElementType.Finally:
		case LanguageElementType.Checked:
		case LanguageElementType.Unchecked:
		case LanguageElementType.Switch:
		case LanguageElementType.Block:
		case LanguageElementType.UnsafeStatement:
		case LanguageElementType.PropertyAccessorGet:
		case LanguageElementType.PropertyAccessorSet:
		case LanguageElementType.EventAdd:
		case LanguageElementType.EventRemove:
		  alwaysHasBlock = true;
		  return true;
		case LanguageElementType.For:
		case LanguageElementType.ForEach:
		case LanguageElementType.If:
		case LanguageElementType.Else:
		case LanguageElementType.Fixed:
		case LanguageElementType.Do:
		case LanguageElementType.While:
		case LanguageElementType.UsingStatement:
		case LanguageElementType.Lock:
		  ParentingStatement parentingStatement = statement as ParentingStatement;
		  if (parentingStatement != null && parentingStatement.HasBlock)
			return true;
		  break;
	  }
	  if (IsIfStatementWithElseAndNestedIf(statement))
		return true;
	  int nodeCount = statement.NodeCount;
	  int codeElementCount = 0;
	  for (int i = 0; i < nodeCount; i++)
	  {
		LanguageElement node = statement.Nodes[i] as LanguageElement;
		if (node == null)
		  continue;
		if (IsSystemSnippet(node as SnippetCodeElement))
		  continue;
		if (node is CodeElement &&
			  node.ElementType != LanguageElementType.Catch &&
			  node.ElementType != LanguageElementType.Finally &&
			  node.ElementType != LanguageElementType.Else &&
			  node.ElementType != LanguageElementType.ElseIf)
		  codeElementCount++;
	  }
	  return codeElementCount != 1;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  CSharp.CSharpCodeGen csCodeGen = null;
	  switch (tokenType)
	  {
		case FormattingTokenType.ParenOpen:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.For:
			case LanguageElementType.ForEach:
			  if (Options.Spacing.BeforeForForeachParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Lock:
			case LanguageElementType.UsingStatement:
			  if (Options.Spacing.BeforeUsingLockParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Fixed:
			  if (Options.Spacing.BeforeFixedParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.While:
			  if (Options.Spacing.BeforeWhileParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.If:
			  if (Options.Spacing.BeforeIfParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Switch:
			  if (Options.Spacing.BeforeSwitchParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Catch:
			  if (Options.Spacing.BeforeCatchParentheses)
				result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.For:
			case LanguageElementType.ForEach:
			  if (Options.Spacing.WithinForForeachParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.UsingStatement:
			case LanguageElementType.Lock:
			  if (Options.Spacing.WithinUsingLockParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Fixed:
			  if (Options.Spacing.WithinFixedParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.While:
			  if (Options.Spacing.WithinWhileParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.If:
			  if (Options.Spacing.WithinIfParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Catch:
			  if (Options.Spacing.WithinCatchParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Switch:
			  if (Options.Spacing.WithinSwitchParentheses)
				result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.Semicolon:
		  if (Context.ElementType == LanguageElementType.For && Options.Spacing.BeforeSemicolonInForStatement)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.In:
		  if (Context.ElementType == LanguageElementType.ForEach)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Context.ElementType == LanguageElementType.Label && Options.Spacing.BeforeColonInLabels)
			result.AddWhiteSpace();
		  else
			if (Context.ElementType == LanguageElementType.Case && Options.Spacing.BeforeColonInCaseStatement)
			  result.AddWhiteSpace();
			else
			  if (ContextMatch(LanguageElementType.ConstructorInitializer))
				if (!Options.LineBreaks.PlaceConstructorInitializerOnSameLine)
				  result.AddNewLine();
		  break;
		case FormattingTokenType.Else:
		  csCodeGen = CodeGen as CSharp.CSharpCodeGen;
		  if (csCodeGen != null && csCodeGen.ContextIsFirstElement)
			break;
		  If prevIf = Context.PreviousCodeSibling as If;
		  bool needElseNewLine = Options.LineBreaks.PlaceElseStatementOnNewLine && !(Context.PreviousSibling is SupportElement);
		  bool prevIfNotHasBlock = prevIf != null && !prevIf.HasBlock;
		  bool isSimple = Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && prevIf != null && prevIf.NodeCount == 1 && Context.NodeCount == 1;
		  if ((needElseNewLine || prevIfNotHasBlock) && !isSimple)
			result.AddNewLine();
		  else
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Finally:
		  csCodeGen = CodeGen as CSharp.CSharpCodeGen;
		  if (csCodeGen != null && csCodeGen.ContextIsFirstElement)
			break;
		  if (!Options.LineBreaks.PlaceFinallyStatementOnNewLine)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Catch:
		  csCodeGen = CodeGen as CSharp.CSharpCodeGen;
		  if (csCodeGen != null && csCodeGen.ContextIsFirstElement)
			break;
		  if (!Options.LineBreaks.PlaceCatchStatementOnNewLine)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.While:
		  if (ContextMatch(LanguageElementType.Do) && Options.LineBreaks.PlaceWhileStatementOnNewLine)
		  {
			result.AddNewLine();
			bool alwaysHasBlock;
			if (!NeedToAddBraces(Context as Statement, Options, out alwaysHasBlock) && Options.Indention.CodeBlockContents)
			  result.AddDecreaseIndent();
		  }
		  break;
		case FormattingTokenType.Add:
		case FormattingTokenType.Remove:
		  if (ContextMatch(LanguageElementType.EventAdd) || ContextMatch(LanguageElementType.EventRemove))
		  {
			IHasAttributes hasAttributes = Context as IHasAttributes;
			if (hasAttributes != null && hasAttributes.Attributes.Count > 0)
			  result.AddNewLine();
		  }
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.ParenOpen:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.For:
			case LanguageElementType.ForEach:
			  if (Options.WrappingAlignment.WrapForStatementHeader)
				result.AddIncreaseIndent();
			  else
				if (Options.Spacing.WithinForForeachParentheses)
				  result.AddWhiteSpace();
			  break;
			case LanguageElementType.UsingStatement:
			case LanguageElementType.Lock:
			  if (Options.Spacing.WithinUsingLockParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Fixed:
			  if (Options.Spacing.WithinFixedParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.While:
			  if (Options.Spacing.WithinWhileParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.If:
			  if (Options.Spacing.WithinIfParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Catch:
			  if (Options.Spacing.WithinCatchParentheses)
				result.AddWhiteSpace();
			  break;
			case LanguageElementType.Switch:
			  if (Options.Spacing.WithinSwitchParentheses)
				result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  Statement statement = Context as Statement;
		  if (statement != null)
			if (!NeedToAddBraces(statement, Options) &&
				Context.ElementType != LanguageElementType.MethodCall &&
				Context.ElementType != LanguageElementType.MethodCallExpression &&
				Context.ElementType != LanguageElementType.Do)
			{
			  if (Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && statement.NodeCount == 1)
				result.AddWhiteSpace();
			  else
				result.AddNewLine();
			  if (statement.ElementType == LanguageElementType.UsingStatement)
			  {
				if (statement.NodeCount > 0)
				{
				  bool needBreak = false;
				  foreach (LanguageElement node in statement.Nodes)
					if (node.ElementType == LanguageElementType.UsingStatement)
					{
					  if (Options.Indention.IndentNestedUsingStatements)
						result.AddIncreaseIndent();
					  needBreak = true;
					  break;
					}
				  if (needBreak)
					break;
				}
			  }
			  if (Options.Indention.CodeBlockContents)
				result.AddIncreaseIndent();
			}
		  if (ContextMatch(LanguageElementType.For) && Options.WrappingAlignment.WrapForStatementHeader)
			result.AddDecreaseIndent();
		  break;
		case FormattingTokenType.Else:
		  if (Context.ElementType == LanguageElementType.ElseIf)
			result.AddWhiteSpace();
		  if (NeedToAddBraces((Statement)Context, Options))
			break;
		  if (Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && Context.NodeCount == 1 ||
			  Context.NextNode is If && !Options.LineBreaks.PlaceIfStatementFollowedByElseOnNewLine)
		  {
			result.AddWhiteSpace();
			break;
		  }
		  result.AddNewLine();
		  if (Options.Indention.CodeBlockContents)
			result.AddIncreaseIndent();
		  break;
		case FormattingTokenType.Semicolon:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.For:
			  if (Options.WrappingAlignment.WrapForStatementHeader)
				result.AddNewLine();
			  else
				if (Options.Spacing.AfterSemicolonInForStatement)
				  result.AddWhiteSpace();
			  break;
			case LanguageElementType.PropertyAccessorGet:
			case LanguageElementType.PropertyAccessorSet:
			case LanguageElementType.EventAdd:
			case LanguageElementType.EventRemove:
			case LanguageElementType.EventRaise:
			  bool hasNextSibling = Context.NextSibling != null;
			  if (!hasNextSibling)
				break;
			  AccessSpecifiedElement accessSpec = Context.Parent as AccessSpecifiedElement;
			  if (accessSpec == null)
				break;
			  Property prop = accessSpec as Property;
			  if (prop != null && prop.IsAutoImplemented)
			  {
				if (!Options.LineBreaks.PlaceAutoImplementedPropertyOnSingleLine)
				  result.AddNewLine();
				break;
			  }
			  if (Options.LineBreaks.PlaceAbstractInterfaceMemberOnSingleLine)
				break;
			  if (accessSpec.IsAbstract)
			  {
				result.AddNewLine();
				break;
			  }
			  Interface @interface = accessSpec.Parent as Interface;
			  if (@interface == null)
				break;
			  result.AddNewLine();
			  break;
		  }
		  break;
		case FormattingTokenType.In:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.Case:
			  if (Options.Indention.CaseStatementContents)
				result.AddIncreaseIndent();
			  if (Context.NodeCount > 0 && !(Context.Nodes[0] is Block))
				result.AddNewLine();
			  break;
		  }
		  break;
		case FormattingTokenType.GoTo:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Case:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Return:
		  if (ContextMatch(LanguageElementType.Return))
		  {
			if (((Return)Context).Expression != null)
			  result.AddWhiteSpace();
		  }
		  else
			if (ContextMatch(LanguageElementType.YieldReturn))
			{
			  if (((YieldReturn)Context).Expression != null)
				result.AddWhiteSpace();
			}
		  break;
		case FormattingTokenType.Do:
		  if (!TokenCStatementCodeGenBase.NeedToAddBraces(Context as Statement, Options))
		  {
			result.AddNewLine();
			if (Options.Indention.CodeBlockContents)
			  result.AddIncreaseIndent();
		  }
		  break;
		case FormattingTokenType.While:
		  if (ContextMatch(LanguageElementType.Do) && Options.Spacing.BeforeWhileParentheses)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
  }
}
