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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
  using CodeStyle.Formatting;
  public class CSharpCodeGen : CodeGen
  {
	LanguageElement _FirstElement;
	public CSharpCodeGen()
	  : this(new CodeGenOptions(ParserLanguageID.CSharp))
	{
	}
	public CSharpCodeGen(CodeGenOptions options)
	  : base(options)
	{
	}
	void AddCharOperatorsToFormattingElements(FormattingTokenType tokenType, FormattingElements result)
	{
	  if (result == null)
		return;
	  switch (tokenType)
	  {
		case FormattingTokenType.Equal:
		  if (Context.ElementType == LanguageElementType.NamespaceReference)
		  {
			if (Options.Spacing.AroundEqualsInNamespaceAliasDeclaration)
			  result.AddWhiteSpace();
		  }
		  else
		  {
			LanguageElement contextParent = Context.Parent;
			if (contextParent != null && contextParent.ElementType == LanguageElementType.MarkupExtensionExpression)
			  break;
			if (Options.Spacing.AroundOneCharOperators)
			  result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.BackSlashEqual:
		case FormattingTokenType.AmpersandEqual:
		case FormattingTokenType.VerticalBarEqual:
		case FormattingTokenType.MinusEqual:
		case FormattingTokenType.PercentEqual:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.LessThanLessThanEqual:
		case FormattingTokenType.GreatThanGreatThanEqual:
		case FormattingTokenType.GreaterThenEqual:
		case FormattingTokenType.SlashEqual:
		case FormattingTokenType.AsteriskEqual:
		case FormattingTokenType.CaretEqual:
		case FormattingTokenType.GreatThanGreatThanGreatThanEqual:
		case FormattingTokenType.QuestionQuestion:
		case FormattingTokenType.EqualEqual:
		case FormattingTokenType.AmpersandAmpersand:
		case FormattingTokenType.VerticalBarVerticalBar:
		case FormattingTokenType.ExclamationEqual:
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.LessThanLessThan:
		  if (Options.Spacing.AroundTwoCharOperators && (Context is BinaryOperatorExpression || Context is Assignment))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Plus:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.VerticalBar:
		case FormattingTokenType.Caret:
		case FormattingTokenType.Slash:
		case FormattingTokenType.Percent:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.GreaterThen:
		case FormattingTokenType.LessThan:
		case FormattingTokenType.Minus:
		  if (Options.Spacing.AroundOneCharOperators && Context is BinaryOperatorExpression)
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.In:
		  if (Context is BinaryOperatorExpression)
		  result.AddWhiteSpace();
		  break;
	  }
	}
	void CSharpContextPoped(LanguageElement oldContext)
	{
	  if (oldContext == null || oldContext.IsRemoved)
		return;
	  if (StatementGen != null && StatementGen.GenerateElementTail(oldContext))
		return;
	  if (TypeDeclarationGen != null && TypeDeclarationGen.GenerateElementTail(oldContext))
		return;
	  if (MemberGen != null && MemberGen.GenerateElementTail(oldContext))
		return;
	  if (NamespaceReferenceGen != null && NamespaceReferenceGen.GenerateElementTail(oldContext))
		return;
	  if (NamespaceGen != null && NamespaceGen.GenerateElementTail(oldContext))
		return;
	  if (ExpressionGen != null && ExpressionGen.GenerateElementTail(oldContext))
		return;
	  if (SupportElementGen != null && SupportElementGen.GenerateElementTail(oldContext))
		return;
	  if (XmlGen != null && XmlGen.GenerateElementTail(oldContext))
		return;
	  if (DirectiveGen != null && DirectiveGen.GenerateElementTail(oldContext))
		return;
	}
	protected override HtmlXmlCodeGenBase CreateHtmlXmlGen()
	{
	  return null;
	}
	protected override TemplateParameterCodeGenBase CreateTemplateParameterGen()
	{
	  return null;
	}
	protected override TemplateCodeGenBase CreateTemplateGen()
	{
	  return null;
	}
	protected override DirectiveCodeGenBase CreateDirectiveGen()
	{
	  return new CSharpDirectiveCodeGen(this);
	}
	protected override ExpressionCodeGenBase CreateExpressionGen()
	{
	  return new CSharpExpressionCodeGen(this);
	}
	protected override MemberCodeGenBase CreateMemberGen()
	{
	  return new CSharpMemberCodeGen(this);
	}
	protected override StatementCodeGenBase CreateStatementGen()
	{
	  return new CSharpStatementCodeGen(this);
	}
	protected override SupportElementCodeGenBase CreateSupportElementGen()
	{
	  return new CSharpSupportElementCodeGen(this);
	}
	protected override TypeDeclarationCodeGenBase CreateTypeDeclarationGen()
	{
	  return new CSharpTypeDeclarationCodeGen(this);
	}
	protected override XmlCodeGenBase CreateXmlGen()
	{
	  return new CSharpXmlCodeGen(this);
	}
	protected override NamespaceReferenceGenBase CreateNamespaceReferenceGen()
	{
	  return new CSharpNamespaceReferenceGen(this);
	}
	protected override NamespaceGenBase CreateNamespaceGen()
	{
	  return new CSharpNamespaceGen(this);
	}
	protected override SnippetCodeGenBase CreateSnippetGen()
	{
	  return new CSharpSnippetCodeGen(this);
	}
	protected override SourceFileCodeGenBase CreateSourceFileGen()
	{
	  return new TokenSourceFileCodeGenBase(this);
	}
	protected override void ContextPushed()
	{
	  Alignment(Context, true);
	}
	protected override bool NeedGenerateSideComment(LanguageElement element)
	{
	  return !SaveFormat && !GenCommentsFromToken;
	}
	protected override void ContextPoped(LanguageElement oldContext)
	{
	  CSharpContextPoped(oldContext);
	  Alignment(oldContext, false);
	  if (oldContext == Context)
		return;
	  ParentingStatement statement = oldContext as ParentingStatement;
	  if (statement != null)
		if (!TokenCStatementCodeGenBase.NeedToAddBraces(statement, Options) && statement.ElementType != LanguageElementType.Case)
		{
		  if (statement.ElementType == LanguageElementType.UsingStatement)
		  {
			if (statement.NodeCount > 0)
			  foreach (LanguageElement node in statement.Nodes)
				if (node.ElementType == LanguageElementType.UsingStatement)
				{
				  if (Options.Indention.IndentNestedUsingStatements)
					AddDecreaseIndent();
				  return;
				}
		  }
		  if (Options.Indention.CodeBlockContents && statement.ElementType != LanguageElementType.Do)
		  {
			If prevIf = statement.PreviousCodeSibling as If;
			bool needElseNewLine = Options.LineBreaks.PlaceElseStatementOnNewLine && !(statement.PreviousSibling is SupportElement);
			bool prevIfNotHasBlock = prevIf != null && !prevIf.HasBlock;
			bool isSimple = Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && prevIf != null && prevIf.NodeCount == 1 && statement.NodeCount == 1;
			if (statement.ElementType != LanguageElementType.Else || (!(statement.FirstChild is If && !Options.LineBreaks.PlaceIfStatementFollowedByElseOnNewLine) && ((needElseNewLine || prevIfNotHasBlock || DecreaseIndentInElseStatement(statement as Else)) && !isSimple)))
			  AddDecreaseIndent();
		  }
		}
	  switch (oldContext.ElementType)
	  {
		case LanguageElementType.Case:
		  if (Options.Indention.CaseStatementContents)
			AddDecreaseIndent();
		  break;
	  }
	}
	bool DecreaseIndentInElseStatement(Else elseSt)
	{
	  if (elseSt == null)
		return false;
	  if (elseSt.ElementType == LanguageElementType.ElseIf)
		return false;
	  if (TokenCStatementCodeGenBase.NeedToAddBraces(elseSt, Options))
		return false;
	  if (Options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine && Context.NodeCount == 1 || Context.NextNode is If && !Options.LineBreaks.PlaceIfStatementFollowedByElseOnNewLine)
		return false;
	  return Options.Indention.CodeBlockContents;
	}
	protected internal override void CalculateIndent(LanguageElement element)
	{
	  if (element == null)
		return;
	  if (element.ElementType == LanguageElementType.SourceFile)
		return;
	  LanguageElement currentElement = element.Parent;
	  while (currentElement != null)
	  {
		switch (currentElement.ElementType)
		{
		  case LanguageElementType.Case:
			if (Options.Indention.CaseStatementContents)
			  Code.IncreaseIndent();
			break;
		  case LanguageElementType.Switch:
			if (Options.Indention.CaseStatementFromSwitchStatement)
			  Code.IncreaseIndent();
			break;
		  case LanguageElementType.ArrayInitializerExpression:
		  case LanguageElementType.ObjectCollectionInitializer:
		  case LanguageElementType.ObjectInitializerExpression:
			if ((Options.WrappingAlignment.WrapArrayObjectAndCollectionInitializers ||
			  Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer) &&
			  Options.Indention.ArrayObjectAndCollectionInitializerContents)
			  Code.IncreaseIndent();
			break;
		  case LanguageElementType.AnonymousMethodExpression:
		  case LanguageElementType.LambdaExpression:
			if (Options.Indention.AnonymousMethodContents)
			  Code.IncreaseIndent();
			break;
		  case LanguageElementType.Else:
			LanguageElement child = currentElement.FirstChild;
			if (child != null && child.ElementType == LanguageElementType.If && !Options.LineBreaks.PlaceIfStatementFollowedByElseOnNewLine)
			  break;
			if (Options.Indention.CodeBlockContents && currentElement.NodeCount > 0)
			  Code.IncreaseIndent();
			break;
		  default:
			if (Options.Indention.CodeBlockContents)
			{
			  IHasBlock block = currentElement as IHasBlock;
			  if (block != null && currentElement.NodeCount > 0)
				Code.IncreaseIndent();
			}
			break;
		}
		currentElement = currentElement.Parent;
	  }
	}
	protected internal override void ResetIndent()
	{
	  Code.IndentLevel = 0;
	}
#if DXCORE
	internal override DevExpress.CodeRush.StructuralParser.FormattingTable FormattingTable
#else
	internal override DevExpress.CodeParser.FormattingTable FormattingTable
#endif
	{
	  get { return CSharpFormattingTable.Instance; }
	}
	public override void GenerateMemberVisibilitySpecifier(MemberVisibilitySpecifier specifier)
	{
	  CodeGenHelper.GenerateVisibility(this.MemberGen, specifier.MemberVisibility);
	}
	public override void GenerateElement(LanguageElement element)
	{
	  if (_FirstElement == null)
		_FirstElement = element;
	  base.GenerateElement(element);
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Comma:
		  if (Options.Spacing.BeforeComma)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Dot:
		  if (Options.Spacing.BeforeDot)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  CurlyBraceHelper.AddTokensBeforeOpen(result, this);
		  break;
		case FormattingTokenType.CurlyBraceClose:
		  CurlyBraceHelper.AddTokensBeforeClose(result, this);
		  break;
		case FormattingTokenType.Semicolon:
		  if (Options.Spacing.BeforeSemicolon && Context.ElementType != LanguageElementType.For)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.MethodCallExpression:
			case LanguageElementType.MethodCall:
			  if (Options.WrappingAlignment.WrapBeforeOpenBraceInInvocation)
			  {
				result.AddNewLine();
				Code.IncreaseIndent();
			  }
			  else
				if (((IHasArguments)Context).ArgumentsCount > 0)
				{
				  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodCallWithArguments)
					result.AddWhiteSpace();
				}
				else
				  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodCallWithoutArguments)
					result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.MethodCallExpression:
			case LanguageElementType.MethodCall:
			  if (Options.WrappingAlignment.WrapBeforeCloseBraceInInvocation)
				result.AddNewLine();
			  else
				if (Options.Spacing.WithinArgumentsListParentheses && ((IHasArguments)Context).ArgumentsCount > 0)
				  result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.BracketClose:
		  if (Context.ElementType != LanguageElementType.AttributeSection && Options.Spacing.WithinSquareBrackets)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Ident:
		  if (!Options.WrappingAlignment.WrapFirstMultiVariableDeclaration)
			break;
		  Variable var = Context as Variable;
		  if (var == null || var.PreviousVariable != null || var.NextVariable == null)
			break;
		  result.AddNewLine();
		  break;
	  }
	  AddCharOperatorsToFormattingElements(tokenType, result);
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Comma:
		  if (Context.ElementType == LanguageElementType.TypeReferenceExpression)
		  {
			TypeReferenceExpression typeRef = Context as TypeReferenceExpression;
			if (typeRef.IsUnbound || typeRef.IsArrayType)
			  break;
		  }
		  if (Options.Spacing.AfterComma && Context.ElementType != LanguageElementType.EnumElement)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Dot:
		  if (Options.Spacing.AfterDot)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  CurlyBraceHelper.AddTokensAfterOpen(result, this);
		  break;
		case FormattingTokenType.CurlyBraceClose:
		  CurlyBraceHelper.AddTokensAfterClose(result, this);
		  break;
		case FormattingTokenType.Semicolon:
		  if (!(Context is PropertyAccessor))
			break;
		  Property parentProperty = Context.Parent as Property;
		  if (parentProperty == null)
			break;
		  if ((Options.LineBreaks.PlaceAutoImplementedPropertyOnSingleLine && parentProperty.IsAutoImplemented) ||
			(Options.LineBreaks.PlaceAbstractInterfaceMemberOnSingleLine && (parentProperty.IsAbstract || parentProperty.Parent is Interface)))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Public:
		case FormattingTokenType.Private:
		case FormattingTokenType.Protected:
		case FormattingTokenType.Internal:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  switch (Context.ElementType)
		  {
			case LanguageElementType.MethodCallExpression:
			case LanguageElementType.MethodCall:
			  if (((IHasArguments)Context).ArgumentsCount > 0)
			  {
				if (Options.Spacing.WithinArgumentsListParentheses)
				  result.AddWhiteSpace();
			  }
			  else
				if (Options.Spacing.WithinEmptyArgumentsListParentheses)
				  result.AddWhiteSpace();
			  break;
		  }
		  break;
		case FormattingTokenType.ParenClose:
		  if (Context.ElementType == LanguageElementType.MethodCall || Context.ElementType == LanguageElementType.MethodCallExpression)
			if (Options.WrappingAlignment.WrapBeforeOpenBraceInInvocation)
			  Code.DecreaseIndent();
		  break;
		case FormattingTokenType.BracketOpen:
		  if (Context.ElementType != LanguageElementType.AttributeSection && Options.Spacing.WithinSquareBrackets)
			result.AddWhiteSpace();
		  break;
	  }
	  AddCharOperatorsToFormattingElements(tokenType, result);
	  switch (Context.ElementType)
	  {
		case LanguageElementType.Comment:
		  Comment comment = (Comment)Context;
		  if (comment.CommentType == CommentType.SingleLine && comment.Position != SupportElementPosition.Inside)
			result.AddNewLine();
		  break;
	  }
	  return result;
	}
	public bool ContextIsFirstElement
	{
	  get { return Context == _FirstElement; }
	}
  }
}
