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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VBCodeGen : CodeGen
	{
	bool _CheckForDetailNodes = true;
	LanguageElement _FirstElement;
		public VBCodeGen()
	  : this(new CodeGenOptions(ParserLanguageID.Basic))
		{
		}
	public VBCodeGen(CodeGenOptions options)
	  : base(options)
		{
		}
		void GenerateCaseClauseList(CaseClausesList caseClausesList)
		{
			if (caseClausesList == null)
				return;
	  StatementGen.GenerateElementCollection(caseClausesList.Clauses, FormattingTokenType.Comma);
		}
		void GenerateCaseClause(CaseClause caseClause)
		{
			if (caseClause == null)
				return;
	  ((VBStatementCodeGen)StatementGen).GenerateCaseClause(caseClause);
		}
		void GenerateArrayNameModifier(ArrayNameModifier element)
		{
			if (element == null)
				return;
			ExpressionCollection coll = element.SizeInitializers;
			Write(FormattingTokenType.ParenOpen);
			int collCount = coll.Count;
			int count = element.Rank;
			if (collCount > element.Rank)
			{
				count = collCount;
			}
			for (int i = 0; i < count; i++)
			{
		if (i != 0)
		  Write(FormattingTokenType.Comma, i - 1);
				if (coll != null && i < collCount)
				{
					LanguageElement init = coll[i];
					GenerateElement(init);
				}
			}
			Write(FormattingTokenType.ParenClose);
		}
	void AddOperatorTokens(FormattingTokenType tokenType, FormattingElements tokens)
	{
	  Method method = Context as Method;
	  if (method != null && method.IsClassOperator)
		return;
	  switch (tokenType)
	  {
		case FormattingTokenType.Slash: 
		case FormattingTokenType.BackSlash:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.Minus:
		case FormattingTokenType.Plus:
		case FormattingTokenType.Caret:
		  if (Options.Spacing.AroundOneCharOperators && !ContextMatch(LanguageElementType.UnaryOperatorExpression))
			tokens.AddWhiteSpace();
		  break;
		case FormattingTokenType.Equal:
		  if (!ContextMatch(LanguageElementType.NamespaceReference) && 
			!ContextMatch(LanguageElementType.XmlNamespaceReference) && 
			Options.Spacing.AroundOneCharOperators)
			tokens.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThan: 
		case FormattingTokenType.GreaterThen: 
		  if (!ContextMatch(LanguageElementType.AttributeSection)
			&& !ContextMatch(LanguageElementType.XmlNamespaceReference) 
			&& !ContextMatch(LanguageElementType.XmlElementReference)
			&& !ContextMatch(LanguageElementType.XmlReference)
			&& Options.Spacing.AroundOneCharOperators)
			tokens.AddWhiteSpace();
		  break;
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.LessThanLessThan:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.GreaterThenEqual: 
		case FormattingTokenType.LessThanGreaterThan:
		case FormattingTokenType.MinusEqual:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.ColonEquals:
		  if (Options.Spacing.AroundTwoCharOperators)
			tokens.AddWhiteSpace();
		  break;
		case FormattingTokenType.Mod:
		case FormattingTokenType.Like:
		case FormattingTokenType.And:
		case FormattingTokenType.Or:
		case FormattingTokenType.AndAlso:
		case FormattingTokenType.OrElse:
		case FormattingTokenType.Xor: 
		  tokens.AddWhiteSpace();
		  break;
	  }
	}
	void VBContextPoped(LanguageElement oldContext)
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
	bool ContextIsEmptyArgumentList()
	{
	  IWithArguments withArgs = Context as IWithArguments;
	  return withArgs != null && (withArgs.Args == null || withArgs.Args.Count == 0);
	}
	bool ContextIsNotEmptyArgumentList()
	{
	  IWithArguments withArgs = Context as IWithArguments;
	  return withArgs != null && withArgs.Args != null && withArgs.Args.Count > 0;
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
			return new VBDirectiveCodeGen(this);
		}
		protected override ExpressionCodeGenBase CreateExpressionGen()
		{
			return new VBExpressionCodeGen(this);
		}
		protected override MemberCodeGenBase CreateMemberGen() 
		{
			return new VBMemberCodeGen(this);
		}
		protected override StatementCodeGenBase CreateStatementGen() 
		{
			return new VBStatementCodeGen(this);
		}
		protected override SupportElementCodeGenBase CreateSupportElementGen() 
		{
			return new VBSupportElementCodeGen(this);
		}
		protected override TypeDeclarationCodeGenBase CreateTypeDeclarationGen() 
		{
			return new VBTypeDeclarationCodeGen(this);
		}
		protected override XmlCodeGenBase CreateXmlGen() 
		{
			return new VBXmlCodeGen(this);
		}
		protected override NamespaceReferenceGenBase CreateNamespaceReferenceGen() 
		{
			return new VBNamespaceReferenceGen(this);
		}
		protected override NamespaceGenBase CreateNamespaceGen() 
		{
			return new VBNamespaceGen(this);
		}
		protected override SnippetCodeGenBase CreateSnippetGen()
		{
			return new VBSnippetCodeGen(this);
		}
	protected override bool IsSpecificElement(LanguageElement element)
	{
	  if (element == null)
		return false;
	  if (element.ElementType == LanguageElementType.ArrayNameModifier ||
		element.ElementType == LanguageElementType.CaseClause ||
		element.ElementType == LanguageElementType.CaseClausesList)
		return true;
	  return false;
	}
	protected override bool IsFirstTypeAncestor(LanguageElement context, TypeDeclaration type)
	{
	  if (context == null || type == null || type.SecondaryAncestorTypes == null || type.SecondaryAncestorTypes.Count == 0)
		return false;
	  return type.SecondaryAncestorTypes[0] == context;
	}
	protected override bool IsFirst(LanguageElement context)
	{
	  if (base.IsFirst(context))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
	  {		
		TypeParameter typeParam = (TypeParameter)parent;
		if (typeParam == null || typeParam.Constraints == null || typeParam.Constraints.Count <= 1)
		  return false;
		return IsFirst(typeParam.Constraints, context);
	  }
	  return false;
	}
	protected override bool IsLast(LanguageElement context)
	{
	  if (base.IsLast(context))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
	  {
		TypeParameter typeParam = (TypeParameter)parent;
		return IsLast(typeParam.Constraints, context);
	  }
	  return false;
	}
	protected override bool IsNotFirst(LanguageElement context)
	{
	  if (base.IsNotFirst(context))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
		  return IsNotFirst(((TypeParameter)parent).Constraints, context);
	  return false;
	}
	protected override bool NeedIndenting(LanguageElement context, bool pushed)
	{
	  if (base.NeedIndenting(context, pushed))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
	  {
		if (Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint ||
			!Options.WrappingAlignment.WrapFirstTypeParameterConstraint &&
			!Options.WrappingAlignment.WrapTypeParameterConstraints)
		  return false;
		return ((TypeParameter)parent).Constraints.Contains(context);
	  }
	  return false;
	}
	protected override bool NeedWrapWithoutFirst(LanguageElement context)
	{
	  if (base.NeedWrapWithoutFirst(context))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
	  {
		TypeParameter typeParam = (TypeParameter)parent;
		return Options.WrappingAlignment.WrapTypeParameterConstraints && IsNotFirst(typeParam.Constraints, context);
	  }
	  return false;
	}
	protected override bool NeedWrapFirst(LanguageElement context)
	{
	  if (base.NeedWrapFirst(context))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
		return Options.WrappingAlignment.WrapFirstTypeParameterConstraint;
	  return false;
	}
	protected override bool NeedAlignment(LanguageElement context, bool pushed)
	{
	  if (base.NeedAlignment(context, pushed))
		return true;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (parent.ElementType == LanguageElementType.TypeParameter)
		return Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint && !Options.WrappingAlignment.WrapFirstTypeParameterConstraint;
	  return false;
	}
	protected override void GenerateSpecificElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  if (element.ElementType == LanguageElementType.ArrayNameModifier)
		GenerateArrayNameModifier(element as ArrayNameModifier);
	  else if (element.ElementType == LanguageElementType.CaseClause)
		GenerateCaseClause(element as CaseClause);
	  else if (element.ElementType == LanguageElementType.CaseClausesList)
		GenerateCaseClauseList(element as CaseClausesList);
	}
	protected override void ContextPushed()
	{
	  Alignment(Context, true);
	}
	protected override void ContextPoped(LanguageElement oldContext)
	{
	  Alignment(oldContext, false);
	  VBContextPoped(oldContext);
	}
	protected override void AddWrappingNewLine()
	{
	  Write(FormattingTokenType.LineContinuation);
	}
	protected bool ContextMatch(LanguageElementType type)
	{
	  return Context.ElementType == type;
	}
	protected override void GenerateElementList(LanguageElement element)
	{
	  ExpressionGen.GenerateElementCollection(element.Nodes, FormattingTokenType.None, true);
	}
	protected override string LineContinuation
	{
	  get { return " _"; }
	}
	internal override FormattingTable FormattingTable
	{
	  get { return VBFormattingTable.Instance; }
	}
	public void GenerateVisibility(MemberVisibility visibility)
	{
	  switch (visibility)
	  {
		case MemberVisibility.Public:
		  Write(FormattingTokenType.Public);
		  return;
		case MemberVisibility.Private:
		  Write(FormattingTokenType.Private);
		  return;
		case MemberVisibility.Protected:
		  Write(FormattingTokenType.Protected);
		  return;
		case MemberVisibility.Friend:
		  Write(FormattingTokenType.Friend);
		  return;
		case MemberVisibility.ProtectedFriend:
		  Write(FormattingTokenType.Protected);
		  Write(FormattingTokenType.Friend);
		  return;
	  }
	}
	public override void GenerateMemberVisibilitySpecifier(MemberVisibilitySpecifier specifier)
	{
	  if (specifier == null)
		return;
	  GenerateVisibility(specifier.MemberVisibility);
	}
	public override void GenerateElement(LanguageElement element)
	{
	  if (_FirstElement == null)
		_FirstElement = element;
	  base.GenerateElement(element);
	  if (_FirstElement == element && (element is Statement || (!element.IsDetailNode && element is IMemberElement && !(element is Param))))
		if (!string.IsNullOrEmpty(Code.LastLine))
		  Code.WriteLine();
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Public:
		case FormattingTokenType.Private:
		case FormattingTokenType.Friend:
		case FormattingTokenType.Protected:
		case FormattingTokenType.Of:
		case FormattingTokenType.Partial:
		case FormattingTokenType.Is:
		case FormattingTokenType.To:
		case FormattingTokenType.ByVal:
		case FormattingTokenType.ByRef:
		case FormattingTokenType.Dim:
		case FormattingTokenType.Aggregate:
		case FormattingTokenType.From:
		case FormattingTokenType.In:
		case FormattingTokenType.Join:
		case FormattingTokenType.Group:
		case FormattingTokenType.On:
		case FormattingTokenType.Equals:
		case FormattingTokenType.Into:
		case FormattingTokenType.Step:
		case FormattingTokenType.Exit:
		case FormattingTokenType.Do:
		case FormattingTokenType.Until:
		case FormattingTokenType.WithEvents:
		case FormattingTokenType.AddHandler:
		case FormattingTokenType.RemoveHandler:
		case FormattingTokenType.SyncLock:
		case FormattingTokenType.Shared:
		case FormattingTokenType.Implements:
		case FormattingTokenType.Declare:
		case FormattingTokenType.Lib:
		case FormattingTokenType.Alias:
		case FormattingTokenType.GoTo:
		case FormattingTokenType.Continue:
		case FormattingTokenType.Error:
		case FormattingTokenType.Where:
		case FormattingTokenType.Select:
		case FormattingTokenType.Key:
		case FormattingTokenType.Using:
		case FormattingTokenType.While:
		case FormattingTokenType.Return:
		case FormattingTokenType.By:
		case FormattingTokenType.Const:
		case FormattingTokenType.ParamArray:
		case FormattingTokenType.End:
		case FormattingTokenType.Overloads:
		case FormattingTokenType.Out:
		case FormattingTokenType.Skip:
		case FormattingTokenType.Async:
		case FormattingTokenType.When:
		case FormattingTokenType.Operator:
		case FormattingTokenType.OrderBy:
		case FormattingTokenType.ReDim:
		case FormattingTokenType.Resume:
		case FormattingTokenType.Take:
		case FormattingTokenType.Erase:
		case FormattingTokenType.Option:
		case FormattingTokenType.Compare:
		case FormattingTokenType.Strict:
		case FormattingTokenType.Explicit:
		case FormattingTokenType.Infer:
		case FormattingTokenType.With:
		case FormattingTokenType.Extern:
		case FormattingTokenType.Handles:
		case FormattingTokenType.Static:
		case FormattingTokenType.Overrides:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Comma:
		  if (!Options.Spacing.AfterComma)
			break;
		  if (Context.Parent != null && Context.Parent.ElementType == LanguageElementType.ArrayCreateExpression)
			break;
		  TypeReferenceExpression expr = Context as TypeReferenceExpression;
		  if (expr != null && (expr.IsUnbound || expr.IsArrayType))
			break;
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.LineContinuation:
		case FormattingTokenType.Try:
		  result.AddNewLine();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  if (Options.Spacing.WithinArrayInitializerBraces)
			if (ContextMatch(LanguageElementType.ArrayInitializerExpression))
			  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Dot:
		  if (Options.Spacing.AfterDot)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenClose:
		  if (Options.Spacing.AfterTypeCastParentheses && ContextMatch(LanguageElementType.CTypeExpression))
			result.AddWhiteSpace();
		  if (Options.WrappingAlignment.WrapBeforeOpenBraceInDeclaration)
			if (Context is IWithParameters)
			  result.AddDecreaseIndent();
		  if (Options.WrappingAlignment.WrapBeforeOpenBraceInInvocation)
			if (Context is IWithArguments)
			  result.AddDecreaseIndent();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (Options.Spacing.WithinTypeCastParentheses && ContextMatch(LanguageElementType.CTypeExpression))
			result.AddWhiteSpace();
		  if (Options.Spacing.WithinArrayRankParentheses && Context is TypeReferenceExpression)
		  {
			expr = (TypeReferenceExpression)Context;
			if (expr.Rank > 1)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinArrayRankEmptyParentheses && Context is TypeReferenceExpression)
		  {
			expr = (TypeReferenceExpression)Context;
			if (expr.Rank == 1)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinArgumentsListParentheses)
		  {
			if (ContextIsNotEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinEmptyArgumentsListParentheses)
		  {
			if (ContextIsEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinExpressionParentheses)
		  {
			if (ContextMatch(LanguageElementType.ParenthesizedExpression))
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinMethodDeclarationParentheses)
		  {
			Method method = Context as Method;
			if (method != null && method.ParameterCount > 0)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinTypeArgumentParenthesis)
		  {
			ReferenceExpressionBase refExpr = Context as ReferenceExpressionBase;
			if (refExpr != null && refExpr.IsGeneric)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinTypeParameterParenthesis)
		  {
			GenericModifier genericModifier = Context as GenericModifier;
			if (genericModifier != null)
			  result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.LessThan:
		  if (Options.Spacing.WithinAttributeBrackets)
			if (ContextMatch(LanguageElementType.AttributeSection))
			  result.AddWhiteSpace();
		  break;
	  }
	  AddOperatorTokens(tokenType, result);
	  return result;
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
		case FormattingTokenType.Is:
		case FormattingTokenType.To:
		case FormattingTokenType.In:
		case FormattingTokenType.Equals:
		case FormattingTokenType.Step:
		case FormattingTokenType.Lock:
		case FormattingTokenType.Until:
		case FormattingTokenType.Lib:
		case FormattingTokenType.Alias:
		case FormattingTokenType.When:
		case FormattingTokenType.Ascending:
		case FormattingTokenType.Handles:
		case FormattingTokenType.LineContinuation:
		case FormattingTokenType.Where:
		case FormattingTokenType.Order:
		case FormattingTokenType.OrderBy:
		case FormattingTokenType.Select:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.From:
		  if (!(Context is FromExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  if (ContextMatch(LanguageElementType.ArrayInitializerExpression) && Context.Parent is ArrayCreateExpression)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.CurlyBraceClose:
		  if (Options.Spacing.WithinArrayInitializerBraces)
			if (ContextMatch(LanguageElementType.ArrayInitializerExpression))
			  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Implements:
		  if (Context is Method || Context is Property)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.With:
		  if (ContextMatch(LanguageElementType.ObjectInitializerExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Dot:
		  if (Options.Spacing.BeforeDot)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenClose:
		  if (Options.Spacing.WithinTypeCastParentheses && ContextMatch(LanguageElementType.CTypeExpression))
			result.AddWhiteSpace();
		  if (Options.Spacing.WithinArrayRankParentheses && Context is TypeReferenceExpression)
		  {
			TypeReferenceExpression expr = (TypeReferenceExpression)Context;
			if (expr.Rank > 1)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinArrayRankEmptyParentheses && Context is TypeReferenceExpression)
		  {
			TypeReferenceExpression expr = (TypeReferenceExpression)Context;
			if (expr.Rank == 1)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinArgumentsListParentheses)
		  {
			if (ContextIsNotEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinEmptyArgumentsListParentheses)
		  {
			if (ContextIsEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinEmptyMethodDeclarationParentheses)
		  {
			Method method = Context as Method;
			if (method != null && method.ParameterCount == 0)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinExpressionParentheses)
		  {
			if (ContextMatch(LanguageElementType.ParenthesizedExpression))
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinMethodDeclarationParentheses)
		  {
			Method method = Context as Method;
			if (method != null && method.ParameterCount > 0)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinTypeArgumentParenthesis)
		  {
			ReferenceExpressionBase refExpr = Context as ReferenceExpressionBase;
			if (refExpr != null && refExpr.IsGeneric)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.WithinTypeParameterParenthesis)
		  {
			GenericModifier genericModifier = Context as GenericModifier;
			if (genericModifier != null)
			  result.AddWhiteSpace();
		  }
		  if (Options.WrappingAlignment.WrapBeforeCloseBraceInDeclaration)
		  {
			if (Context is IWithParameters)
			  AddWrappingNewLine();
		  }
		  if (Options.WrappingAlignment.WrapBeforeCloseBraceInInvocation)
		  {
			if (Context is IWithArguments)
			  AddWrappingNewLine();
		  }
		  break;
		case FormattingTokenType.ParenOpen:
		  if (Options.Spacing.BeforeArrayRankParentheses && Context is TypeReferenceExpression)
			result.AddWhiteSpace();
		  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodCallWithArguments)
		  {
			if (ContextIsNotEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodCallWithoutArguments)
		  {
			if (ContextIsEmptyArgumentList())
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodWithParameters)
		  {
			Method method = Context as Method;
			if (method != null && method.ParameterCount > 0)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.BeforeOpeningParenthesisOfAMethodWithoutParameters)
		  {
			Method method = Context as Method;
			if (method != null && method.ParameterCount == 0)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.BeforeTypeArgumentParenthesis)
		  {
			ReferenceExpressionBase refExpr = Context as ReferenceExpressionBase;
			if (refExpr != null && refExpr.IsGeneric)
			  result.AddWhiteSpace();
		  }
		  if (Options.Spacing.BeforeTypeParameterParenthesis)
		  {
			GenericModifier genericModifier = Context as GenericModifier;
			if (genericModifier != null)
			  result.AddWhiteSpace();
		  }
		  if (Options.WrappingAlignment.WrapBeforeOpenBraceInDeclaration)
			if (Context is IWithParameters)
			{
			  result.AddIncreaseIndent();
			  AddWrappingNewLine();
			}
		  if (Options.WrappingAlignment.WrapBeforeOpenBraceInInvocation)
			if (Context is IWithArguments)
			{
			  result.AddIncreaseIndent();
			  AddWrappingNewLine();
			}
		  break;
		case FormattingTokenType.Colon:
		  if (Options.Spacing.BeforeColonInLabels && ContextMatch(LanguageElementType.Label))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.GreaterThen:
		  if (Options.Spacing.WithinAttributeBrackets)
			if (ContextMatch(LanguageElementType.AttributeSection))
			  result.AddWhiteSpace();
		  break;
	  } 
	  AddOperatorTokens(tokenType, result);
	  return result;
	}
		public bool CheckForDetailNodes
		{
			get
			{
				return _CheckForDetailNodes;
			}
			set
			{
				_CheckForDetailNodes = value;
			}
		}
	}
}
