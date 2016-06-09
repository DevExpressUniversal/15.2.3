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
using System.Globalization;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class ExpressionCodeGenBase : LanguageElementCodeGenBase
  {
	class ExpressionInfo
	{
	  Expression _Expression;
	  BinaryOperatorType _Operator;
	  public ExpressionInfo(Expression expression, BinaryOperatorType op)
	  {
		_Expression = expression;
		_Operator = op;
	  }
	  public BinaryOperatorType Operator
	  {
		get { return _Operator; }
		set { _Operator = value; }
	  }
	  public Expression Expression
	  {
		get { return _Expression; }
	  }
	}
	public ExpressionCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	List<ExpressionInfo> GetExpressionListFromBinaryExpression(BinaryOperatorExpression expression, LanguageElementType elementType)
	{
	  List<ExpressionInfo> result = new List<ExpressionInfo>();
	  GetExpressionListFromBinaryExpression(expression, result, elementType, BinaryOperatorType.None);
	  return result;
	}
	void GetExpressionListFromBinaryExpression(BinaryOperatorExpression expression, List<ExpressionInfo> list, LanguageElementType elementType, BinaryOperatorType op)
	{
	  Expression left = expression.LeftSide;
	  if (left.ElementType == elementType)
		GetExpressionListFromBinaryExpression(left as BinaryOperatorExpression, list, elementType, expression.BinaryOperator);
	  else
		list.Add(new ExpressionInfo(left, expression.BinaryOperator));
	  Expression right = expression.RightSide;
	  if (right.ElementType == elementType)
		GetExpressionListFromBinaryExpression(right as BinaryOperatorExpression, list, elementType, op);
	  else
		list.Add(new ExpressionInfo(right, op));
	}
	string GetBinaryOperatorText(BinaryOperatorExpression expression)
	{
	  string op = expression.OperatorText;
	  if (op != null && op.Length > 0)
		op = String.Format(" {0} ", op);
	  BinaryOperatorType operatorType = expression.BinaryOperator;
	  if (operatorType != BinaryOperatorType.None)
		op = GetBinaryOperatorText(expression.BinaryOperator);
	  return op;
	}
	void GenerateElementForeComment(LanguageElement element)
	{
	  if (element == null || CodeGen == null)
		return;
	  CodeGen.GenerateElementForeComment(element);
	}
	void GenerateElementBackComment(LanguageElement element)
	{
	  if (element == null || CodeGen == null)
		return;
	  CodeGen.GenerateElementBackComment(element);
	}
	protected string GetStringValue(object value)
	{
	  if (value == null)
		return String.Empty;
	  return Convert.ToString(value, CultureInfo.InvariantCulture);
	}
	protected abstract bool IsEscapedString(string value);
	protected abstract string GetBinaryOperatorText(BinaryOperatorType operatorType);
	protected abstract void GenerateFunctionPointerTypeReference(FunctionPointerTypeReference expression);
	protected abstract void GenerateTypedElementReferenceExpression(TypedElementReferenceExpression expression);
	protected abstract void GenerateDistinctExpression(DistinctExpression expression);
	protected abstract void GenerateInExpression(InExpression expression);
	protected abstract void GenerateMemberInitializerExpression(MemberInitializerExpression expression);
	protected abstract void GenerateObjectInitializerExpression(ObjectInitializerExpression expression);
	protected abstract void GenerateLambdaExpression(LambdaExpression expression);
	protected abstract void GenerateFromExpression(FromExpression expression);
	protected abstract void GenerateJoinExpression(JoinExpression expression);
	protected abstract void GenerateQueryExpression(QueryExpression expression);
	protected abstract void GenerateLetExpression(LetExpression expression);
	protected abstract void GenerateWhereExpression(WhereExpression expression);
	protected abstract void GenerateOrderingExpression(OrderingExpression expression);
	protected abstract void GenerateOrderByExpression(OrderByExpression expression);
	protected abstract void GenerateSelectExpression(SelectExpression expression);
	protected abstract void GenerateIntoExpression(IntoExpression expression);
	protected abstract void GenerateEqualsExpression(EqualsExpression expression);
	protected abstract void GenerateGroupByExpression(GroupByExpression expression);
	protected abstract void GenerateJoinIntoExpression(JoinIntoExpression expression);
	protected abstract void GenerateCppQualifiedElementReference(CppQualifiedElementReference expression);
	protected abstract void GenerateExpressionTypeArgument(ExpressionTypeArgument expression);
	protected abstract void GenerateGenericTypeArguments(TypeReferenceExpressionCollection arguments);
	protected abstract void GenerateArgumentDirection(ArgumentDirection direction);
	protected abstract void GenerateAddressOfExpression(AddressOfExpression expression);
	protected abstract void GenerateArgumentDirectionExpression(ArgumentDirectionExpression expression);
	protected abstract void GenerateArrayCreateExpression(ArrayCreateExpression expression);
	protected abstract void GenerateArrayInitializerExpression(ArrayInitializerExpression expression);
	protected abstract void GenerateAssignmentExpression(AssignmentExpression expression);
	protected abstract void GenerateBaseReferenceExpression(BaseReferenceExpression expression);
	protected abstract void GenerateCheckedExpression(CheckedExpression expression);
	protected abstract void GenerateConditionalExpression(ConditionalExpression expression);
	protected abstract void GenerateIndexerExpression(IndexerExpression expression);
	protected abstract void GenerateObjectCreationExpression(ObjectCreationExpression expression);
	protected abstract void GenerateBooleanLiteral(bool value);
	protected abstract void GenerateStringLiteral(string value);
	protected abstract void GenerateCharLiteral(char value);
	protected abstract void GenerateNullLiteral();
	protected abstract void GenerateNumberLiteral(string name, object value, PrimitiveType type);
	protected abstract void GenerateDateTime(object value);
	protected abstract void GenerateSizeOfExpression(SizeOfExpression expression);
	protected abstract void GenerateThisReferenceExpression(ThisReferenceExpression expression);
	protected abstract void GenerateMyClassExpression(MyClassExpression expression);
	protected abstract void GenerateTypeCastExpression(TypeCastExpression expression);
	protected abstract void GenerateTypeOfExpression(TypeOfExpression expression);
	protected abstract void GenerateTypeOfIsExpression(TypeOfIsExpression expression);
	protected abstract void GenerateTypeReferenceExpression(TypeReferenceExpression expression);
	protected abstract void GenerateUncheckedExpression(UncheckedExpression expression);
	protected abstract void GenerateLogicalInversion(LogicalInversion expression);
	protected abstract void GenerateUnaryIncrement(UnaryIncrement expression);
	protected abstract void GenerateUnaryDecrement(UnaryDecrement expression);
	protected abstract void GenerateTypeCheck(TypeCheck expression);
	protected abstract void GenerateConditionalTypeCast(ConditionalTypeCast expression);
	protected abstract void GenerateIsNot(IsNot expression);
	protected abstract void GenerateAnonymousMethodExpression(AnonymousMethodExpression expression);
	protected abstract void GenerateDefaultValueExpression(DefaultValueExpression expression);
	protected abstract void GenerateNullCoalescingExpression(NullCoalescingExpression expression);
	protected abstract void GenerateQualifiedAliasExpression(QualifiedAliasExpression expression);
	protected abstract void GenerateComplexExpression(ComplexExpression expression);
	protected abstract void GenerateDeleteExpression(DeleteExpression expression);
	protected abstract void GenerateDeleteArrayExpression(DeleteArrayExpression expression);
	protected abstract void GenerateElaboratedTypeReference(ElaboratedTypeReference expression);
	protected abstract void GenerateManagedObjectCreationExpression(ManagedObjectCreationExpression expression);
	protected abstract void GenerateParametizedArrayCreateExpression(ParametrizedArrayCreateExpression expression);
	protected abstract void GenerateParametizedObjectCreationExpression(ParametrizedObjectCreationExpression expression);
	protected abstract void GeneratePointerElementReference(PointerElementReference expression);
	protected abstract void GenerateQualifiedNestedReference(QualifiedNestedReference expression);
	protected abstract void GenerateQualifiedNestedTypeReference(QualifiedNestedTypeReference expression);
	protected abstract void GenerateQualifiedTypeReferenceExpression(QualifiedTypeReferenceExpression expression);
	protected abstract void GenerateQualifiedMethodReference(QualifiedMethodReference expression);
	protected abstract void GeneratePointerMethodReference(PointerMethodReference expression);
	protected abstract void GenerateManagedArrayCreateExpression(ManagedArrayCreateExpression expression);
	protected abstract bool GenerateMacroExpression(Expression expression);
	protected abstract void GenerateAttributeVariableInitializer(AttributeVariableInitializer expression);
	protected virtual void GenerateMarkupExtensionExpression(MarkupExtensionExpression expression)
	{
	}
	protected virtual bool GenerateExpression(Expression expression)
	{
	  if (expression == null)
		return false;
	  GenerateElementForeComment(expression);
	  try
	  {
		return GenerateExpressionCore(expression);
	  }
	  finally
	  {
		GenerateElementBackComment(expression);
	  }
	}
	protected virtual bool GenerateExpressionCore(Expression expression)
	{
	  if (expression == null)
		return false;
	  if (GenerateMacroExpression(expression))
		return true;
	  switch (expression.ElementType)
	  {
		case LanguageElementType.AddressOfExpression:
		  GenerateAddressOfExpression(expression as AddressOfExpression);
		  return true;
		case LanguageElementType.ArgumentDirectionExpression:
		  GenerateArgumentDirectionExpression(expression as ArgumentDirectionExpression);
		  return true;
		case LanguageElementType.ArrayCreateExpression:
		  GenerateArrayCreateExpression(expression as ArrayCreateExpression);
		  return true;
		case LanguageElementType.ArrayInitializerExpression:
		  GenerateArrayInitializerExpression(expression as ArrayInitializerExpression);
		  return true;
		case LanguageElementType.AssignmentExpression:
		  GenerateAssignmentExpression(expression as AssignmentExpression);
		  return true;
		case LanguageElementType.BaseReferenceExpression:
		  GenerateBaseReferenceExpression(expression as BaseReferenceExpression);
		  return true;
		case LanguageElementType.BinaryOperatorExpression:
		  GenerateBinaryOperatorExpression(expression as BinaryOperatorExpression);
		  return true;
		case LanguageElementType.CheckedExpression:
		  GenerateCheckedExpression(expression as CheckedExpression);
		  return true;
		case LanguageElementType.ConditionalExpression:
		  GenerateConditionalExpression(expression as ConditionalExpression);
		  return true;
		case LanguageElementType.ElementReferenceExpression:
		case LanguageElementType.AggregateElementReferenceExpression:
		  GenerateElementReferenceExpression(expression as ElementReferenceExpression);
		  return true;
		case LanguageElementType.IndexerExpression:
		  GenerateIndexerExpression(expression as IndexerExpression);
		  return true;
		case LanguageElementType.MethodCallExpression:
		case LanguageElementType.AggregateMethodCallExpression:
		  GenerateMethodCallExpression(expression as MethodCallExpression);
		  return true;
		case LanguageElementType.MethodReferenceExpression:
		  GenerateMethodReferenceExpression(expression as MethodReferenceExpression);
		  return true;
		case LanguageElementType.ObjectCreationExpression:
		  GenerateObjectCreationExpression(expression as ObjectCreationExpression);
		  return true;
		case LanguageElementType.ParenthesizedExpression:
		  GenerateParenthesizedExpression(expression as ParenthesizedExpression);
		  return true;
		case LanguageElementType.PrimitiveExpression:
		  GeneratePrimitiveExpression(expression as PrimitiveExpression);
		  return true;
		case LanguageElementType.SizeOfExpression:
		  GenerateSizeOfExpression(expression as SizeOfExpression);
		  return true;
		case LanguageElementType.ThisReferenceExpression:
		  GenerateThisReferenceExpression(expression as ThisReferenceExpression);
		  return true;
		case LanguageElementType.MyClassExpression:
		  GenerateMyClassExpression(expression as MyClassExpression);
		  return true;
		case LanguageElementType.TypeCastExpression:
		  GenerateTypeCastExpression(expression as TypeCastExpression);
		  return true;
		case LanguageElementType.TypeOfExpression:
		  GenerateTypeOfExpression(expression as TypeOfExpression);
		  return true;
		case LanguageElementType.TypeOfIsExpression:
		  GenerateTypeOfIsExpression(expression as TypeOfIsExpression);
		  return true;
		case LanguageElementType.TypeReferenceExpression:
		  GenerateTypeReferenceExpression(expression as TypeReferenceExpression);
		  return true;
		case LanguageElementType.UnaryOperatorExpression:
		  GenerateUnaryOperatorExpression(expression as UnaryOperatorExpression);
		  return true;
		case LanguageElementType.UncheckedExpression:
		  GenerateUncheckedExpression(expression as UncheckedExpression);
		  return true;
		case LanguageElementType.LogicalOperation:
		  GenerateLogicalOperation(expression as LogicalOperation);
		  return true;
		case LanguageElementType.LogicalInversion:
		  GenerateLogicalInversion(expression as LogicalInversion);
		  return true;
		case LanguageElementType.UnaryIncrement:
		  GenerateUnaryIncrement(expression as UnaryIncrement);
		  return true;
		case LanguageElementType.UnaryDecrement:
		  GenerateUnaryDecrement(expression as UnaryDecrement);
		  return true;
		case LanguageElementType.RelationalOperation:
		  GenerateRelationalOperation(expression as RelationalOperation);
		  return true;
		case LanguageElementType.TypeCheck:
		  GenerateTypeCheck(expression as TypeCheck);
		  return true;
		case LanguageElementType.ConditionalTypeCast:
		  GenerateConditionalTypeCast(expression as ConditionalTypeCast);
		  return true;
		case LanguageElementType.IsNot:
		  GenerateIsNot(expression as IsNot);
		  return true;
		case LanguageElementType.AnonymousMethodExpression:
		  GenerateAnonymousMethodExpression(expression as AnonymousMethodExpression);
		  return true;
		case LanguageElementType.DefaultValueExpression:
		  GenerateDefaultValueExpression(expression as DefaultValueExpression);
		  return true;
		case LanguageElementType.NullCoalescingExpression:
		  GenerateNullCoalescingExpression(expression as NullCoalescingExpression);
		  return true;
		case LanguageElementType.QualifiedAliasExpression:
		  GenerateQualifiedAliasExpression(expression as QualifiedAliasExpression);
		  return true;
		case LanguageElementType.SnippetExpression:
		  GenerateSnippetExpression(expression as SnippetExpression);
		  return true;
		case LanguageElementType.AttributeVariableInitializer:
		  GenerateAttributeVariableInitializer(expression as AttributeVariableInitializer);
		  return true;
		case LanguageElementType.MarkupExtensionExpression:
		  GenerateMarkupExtensionExpression(expression as MarkupExtensionExpression);
		  return true;
		case LanguageElementType.ComplexExpression:
		  GenerateComplexExpression(expression as ComplexExpression);
		  return true;
		case LanguageElementType.DeleteExpression:
		  GenerateDeleteExpression(expression as DeleteExpression);
		  return true;
		case LanguageElementType.DeleteArrayExpression:
		  GenerateDeleteArrayExpression(expression as DeleteArrayExpression);
		  return true;
		case LanguageElementType.ElaboratedTypeReference:
		  GenerateElaboratedTypeReference(expression as ElaboratedTypeReference);
		  return true;
		case LanguageElementType.ManagedObjectCreationExpression:
		  GenerateManagedObjectCreationExpression(expression as ManagedObjectCreationExpression);
		  return true;
		case LanguageElementType.ParametrizedArrayCreateExpression:
		  GenerateParametizedArrayCreateExpression(expression as ParametrizedArrayCreateExpression);
		  return true;
		case LanguageElementType.ParametrizedObjectCreationExpression:
		  GenerateParametizedObjectCreationExpression(expression as ParametrizedObjectCreationExpression);
		  return true;
		case LanguageElementType.PointerElementReference:
		  GeneratePointerElementReference(expression as PointerElementReference);
		  return true;
		case LanguageElementType.QualifiedNestedReference:
		  GenerateQualifiedNestedReference(expression as QualifiedNestedReference);
		  return true;
		case LanguageElementType.QualifiedNestedTypeReference:
		  GenerateQualifiedNestedTypeReference(expression as QualifiedNestedTypeReference);
		  return true;
		case LanguageElementType.QualifiedTypeReferenceExpression:
		  GenerateQualifiedTypeReferenceExpression(expression as QualifiedTypeReferenceExpression);
		  return true;
		case LanguageElementType.QualifiedMethodReference:
		  GenerateQualifiedMethodReference(expression as QualifiedMethodReference);
		  return true;
		case LanguageElementType.PointerMethodReference:
		  GeneratePointerMethodReference(expression as PointerMethodReference);
		  return true;
		case LanguageElementType.ManagedArrayCreateExpression:
		  GenerateManagedArrayCreateExpression(expression as ManagedArrayCreateExpression);
		  return true;
		case LanguageElementType.CppQualifiedElementReference:
		  GenerateCppQualifiedElementReference(expression as CppQualifiedElementReference);
		  return true;
		case LanguageElementType.ExpressionTypeArgument:
		  GenerateExpressionTypeArgument(expression as ExpressionTypeArgument);
		  return true;
		case LanguageElementType.MemberInitializerExpression:
		  GenerateMemberInitializerExpression(expression as MemberInitializerExpression);
		  return true;
		case LanguageElementType.ObjectInitializerExpression:
		  GenerateObjectInitializerExpression(expression as ObjectInitializerExpression);
		  return true;
		case LanguageElementType.LambdaExpression:
		  GenerateLambdaExpression(expression as LambdaExpression);
		  return true;
		case LanguageElementType.FromExpression:
		  GenerateFromExpression(expression as FromExpression);
		  return true;
		case LanguageElementType.JoinExpression:
		  GenerateJoinExpression(expression as JoinExpression);
		  return true;
		case LanguageElementType.QueryExpression:
		  GenerateQueryExpression(expression as QueryExpression);
		  return true;
		case LanguageElementType.LetExpression:
		  GenerateLetExpression(expression as LetExpression);
		  return true;
		case LanguageElementType.WhereExpression:
		  GenerateWhereExpression(expression as WhereExpression);
		  return true;
		case LanguageElementType.OrderingExpression:
		  GenerateOrderingExpression(expression as OrderingExpression);
		  return true;
		case LanguageElementType.OrderByExpression:
		  GenerateOrderByExpression(expression as OrderByExpression);
		  return true;
		case LanguageElementType.SelectExpression:
		  GenerateSelectExpression(expression as SelectExpression);
		  return true;
		case LanguageElementType.IntoExpression:
		  GenerateIntoExpression(expression as IntoExpression);
		  return true;
		case LanguageElementType.InExpression:
		  GenerateInExpression(expression as InExpression);
		  return true;
		case LanguageElementType.Distinct:
		  GenerateDistinctExpression(expression as DistinctExpression);
		  return true;
		case LanguageElementType.JoinIntoExpression:
		  GenerateJoinIntoExpression(expression as JoinIntoExpression);
		  return true;
		case LanguageElementType.EqualsExpression:
		  GenerateEqualsExpression(expression as EqualsExpression);
		  return true;
		case LanguageElementType.GroupByExpression:
		  GenerateGroupByExpression(expression as GroupByExpression);
		  return true;
		case LanguageElementType.TypedElementReferenceExpression:
		  GenerateTypedElementReferenceExpression(expression as TypedElementReferenceExpression);
		  return true;
		case LanguageElementType.FunctionPointerTypeReference:
		  GenerateFunctionPointerTypeReference(expression as FunctionPointerTypeReference);
		  return true;
		case LanguageElementType.ShortInitializeExpression:
		  GenerateShortInitializeExpression(expression as ShortInitializeExpression);
		  return true;
		default:
		  return GenerateSpecificExpression(expression);
	  }
	}
	protected virtual bool GenerateSpecificExpression(Expression expressiom)
	{
	  return false;
	}
	protected virtual void GenerateExpressionList(ExpressionCollectionBase expressions)
	{
	  GenerateExpressionList(expressions, GetCommaDelimeter());
	}
	protected virtual void GenerateExpressionList(ExpressionCollectionBase expressions, string delimiter)
	{
	  GenerateExpressionList(expressions, delimiter, false);
	}
	protected virtual void GenerateExpressionList(ExpressionCollectionBase expressions, string delimiter, bool lineBreakAfterDelimiter)
	{
	  if (expressions == null)
		return;
	  for (int i = 0; i < expressions.Count; i++)
	  {
		CodeGen.GenerateElement(expressions[i] as Expression);
		if (i < expressions.Count - 1)
		{
		  Code.Write(delimiter);
		  if (lineBreakAfterDelimiter)
			Code.WriteLine();
		}
	  }
	}
	protected virtual void GenerateLogicalOperator(LogicalOperator logicalOperator)
	{
	  BinaryOperatorType op = LogicalOperation.GetBinaryOperatorType(logicalOperator);
	  GenerateBinaryOperator(op);
	}
	protected virtual void GenerateRelationalOperator(RelationalOperator relationalOperator)
	{
	  BinaryOperatorType op = RelationalOperation.GetBinaryOperatorType(relationalOperator);
	  GenerateBinaryOperator(op);
	}
	protected virtual void GenerateBinaryOperator(BinaryOperatorType op)
	{
	  Code.Write(GetBinaryOperatorText(op));
	}
	protected virtual void GenerateBinaryOperator(BinaryOperatorExpression expression)
	{
	  Code.Write(GetBinaryOperatorText(expression));
	}
	protected virtual void GenerateBinaryOperatorExpression(BinaryOperatorExpression expression)
	{
	  List<ExpressionInfo> expressionList = GetExpressionListFromBinaryExpression(expression, LanguageElementType.BinaryOperatorExpression);
	  for (int i = 0; i < expressionList.Count; i++)
	  {
		ExpressionInfo info = expressionList[i];
		CodeGen.GenerateElement(info.Expression);
		if (info.Operator == BinaryOperatorType.None)
		  continue;
		GenerateBinaryOperator(info.Operator);
	  }
	}
	protected virtual void GenerateSnippetExpression(SnippetExpression expression)
	{
	  if (expression == null || expression.Name == null)
		return;
	  Code.Write(expression.Name);
	}
	protected virtual void GenerateReferenceExpressionBase(ReferenceExpressionBase expression)
	{
	  string lDelimeter = ".";
	  if (expression.IsMulOperator)
		lDelimeter = ".*";
	  if (expression.ElementType == LanguageElementType.QualifiedAliasExpression)
		lDelimeter = String.Empty;
	  GenerateReferenceExpressionBase(expression, lDelimeter);
	}
	protected virtual void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, string delimiter)
	{
	  Expression lSource = expression.Qualifier;
	  if (lSource != null)
	  {
		CodeGen.GenerateElement(lSource);
		if (expression.ElementType != LanguageElementType.QualifiedAliasExpression)
		  Code.Write(delimiter);
	  }
	  Code.Write(expression.Name);
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	}
	protected virtual void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, TokenList delimiters)
	{
	  GenerateReferenceExpressionBase(expression, FormattingTokenType.None, FormattingTokenType.None, delimiters);
	}
	protected virtual void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, FormattingTokenType prevName, FormattingTokenType afterName, FormattingTokenType delimiter)
	{
	  GenerateReferenceExpressionBase(expression, prevName, afterName, new TokenList(delimiter));
	}
	protected virtual void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, FormattingTokenType prevName, FormattingTokenType afterName, TokenList delimiters)
	{
	  Expression lSource = expression.Qualifier;
	  if (lSource != null)
	  {
		CodeGen.GenerateElement(lSource);
		if (expression.ElementType != LanguageElementType.QualifiedAliasExpression)
		  Write(delimiters);
	  }
	  if (prevName != FormattingTokenType.None)
		Write(prevName);
	  Write(FormattingTokenType.Ident);
	  if (afterName != FormattingTokenType.None)
		Write(afterName);
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	}
	protected virtual void GenerateElementReferenceExpression(ElementReferenceExpression expression)
	{
	  GenerateReferenceExpressionBase(expression);
	}
	protected virtual void GenerateMethodCallExpression(MethodCallExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  Code.Write("(");
	  GenerateExpressionList(expression.Arguments);
	  Code.Write(")");
	}
	protected virtual void GenerateShortInitializeExpression(ShortInitializeExpression expression)
	{
	  Code.Write("(");
	  GenerateExpressionList(expression.Arguments);
	  Code.Write(")");
	}
	protected virtual void GenerateMethodReferenceExpression(MethodReferenceExpression expression)
	{
	  GenerateReferenceExpressionBase(expression);
	}
	protected virtual void GenerateParenthesizedExpression(ParenthesizedExpression expression)
	{
	  Code.Write("(");
	  bool withinExpressionParenthesesSpacing = Options.Spacing.WithinExpressionParentheses;
	  if (withinExpressionParenthesesSpacing)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.Expression);
	  if (withinExpressionParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	}
	protected virtual void GenerateStringLiteralWithEscapeSequence(string value)
	{
	  GenerateStringLiteral(value);
	}
	protected virtual void GeneratePrimitiveExpression(PrimitiveExpression expression)
	{
	  object value = expression.PrimitiveValue;
	  string name = expression.Name;
	  bool isEscapedString = IsEscapedString(name);
	  PrimitiveType type = expression.PrimitiveType;
	  if (value is bool)
		GenerateBooleanLiteral((bool)value);
	  else if (value == null && (name == null || name.Length == 0) ||
		type == PrimitiveType.Void)
		GenerateNullLiteral();
	  else if (expression.Literal != null)
		Code.Write(expression.Literal, false);
	  else if (value is char)
		GenerateCharLiteral((char)value);
	  else if (type == PrimitiveType.Char)
		Code.Write(name);
	  else if (value is string || type == PrimitiveType.String)
	  {
		if (name != null && name != String.Empty && isEscapedString)
		  Code.Write(name, false);
		else
		{
		  if (expression.CanHasEscapeSequence)
			GenerateStringLiteralWithEscapeSequence((string)value);
		  else
			GenerateStringLiteral((string)value);
		}
	  }
	  else if (expression.IsNumberLiteral)
		GenerateNumberLiteral(name, value, expression.PrimitiveType);
	  else if (expression.IsDateTime && (name == null || name.Length == 0))
		GenerateDateTime(value);
	  else
		Code.Write(name);
	}
	protected virtual void GenerateUnaryOperatorExpression(UnaryOperatorExpression expression)
	{
	  if (!expression.IsPostOperator)
		Code.Write("{0}", expression.OperatorText);
	  CodeGen.GenerateElement(expression.Expression);
	  if (expression.IsPostOperator)
		Code.Write("{0}", expression.OperatorText);
	}
	protected virtual void GenerateLogicalOperation(LogicalOperation expression)
	{
	  List<ExpressionInfo> expressionList = GetExpressionListFromBinaryExpression(expression, LanguageElementType.LogicalOperation);
	  int indentLevel = Code.IndentLevel;
	  string space = Code.LastLineInSpacesWithouIndent;
	  for (int i = 0; i < expressionList.Count; i++)
	  {
		ExpressionInfo info = expressionList[i];
		CodeGen.GenerateElement(info.Expression);
		if (info.Operator == BinaryOperatorType.None)
		  continue;
		if (Options.WrappingAlignment.WrapBeforeOperatorInLogicalExpression)
		{
		  Code.WriteLine(GetLineContinuation());
		  if (i == 0)
			Code.IncreaseIndent();
		}
		GenerateBinaryOperator(info.Operator);
		if (Options.WrappingAlignment.WrapLogicalOperatorExpression)
		{
		  Code.WriteLine(GetLineContinuation());
		  if (Options.WrappingAlignment.AlignWithFirstLogicalExpressionItem)
			Code.Write(space);
		  else
			if (i == 0)
			  Code.IncreaseIndent();
		}
	  }
	  Code.IndentLevel = indentLevel;
	}
	protected virtual void GenerateRelationalOperation(RelationalOperation expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  GenerateRelationalOperator(expression.RelationalOperator);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	public override void GenerateElement(LanguageElement languageElement)
	{
	  if (languageElement == null)
		return;
	  GenerateExpression(languageElement as Expression);
	}
  }
}
