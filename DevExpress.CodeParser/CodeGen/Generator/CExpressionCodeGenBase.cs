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
  public abstract class CExpressionCodeGenBase : ExpressionCodeGenBase
  {
	public CExpressionCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected override string GetBinaryOperatorText(BinaryOperatorType operatorType)
	{
	  string result = string.Empty;
	  switch (operatorType)
	  {
		case BinaryOperatorType.None:
		  return " ";
		case BinaryOperatorType.Add:
		  result = "+";
		  break;
		case BinaryOperatorType.Assign:
		  result = "=";
		  break;
		case BinaryOperatorType.BitwiseAnd:
		  result = "&";
		  break;
		case BinaryOperatorType.BitwiseOr:
		  result = "|";
		  break;
		case BinaryOperatorType.BooleanAnd:
		  result = "&&";
		  break;
		case BinaryOperatorType.BooleanOr:
		  result = "||";
		  break;
		case BinaryOperatorType.ExclusiveOr:
		  result = "^";
		  break;
		case BinaryOperatorType.Divide:
		  result = "/";
		  break;
		case BinaryOperatorType.GreaterThan:
		  result = ">";
		  break;
		case BinaryOperatorType.GreaterThanOrEqual:
		  result = ">=";
		  break;
		case BinaryOperatorType.IdentityEquality:
		  result = "==";
		  break;
		case BinaryOperatorType.IdentityInequality:
		  result = "!=";
		  break;
		case BinaryOperatorType.LessThan:
		  result = "<";
		  break;
		case BinaryOperatorType.LessThanOrEqual:
		  result = "<=";
		  break;
		case BinaryOperatorType.Modulus:
		case BinaryOperatorType.IntegerDivision:
		  result = "%";
		  break;
		case BinaryOperatorType.Multiply:
		  result = "*";
		  break;
		case BinaryOperatorType.Subtract:
		  result = "-";
		  break;
		case BinaryOperatorType.ValueEquality:
		  result = "==";
		  break;
		case BinaryOperatorType.StrictEquality:
		  result = "===";
		  break;
		case BinaryOperatorType.StrictInequality:
		  result = "!==";
		  break;
		case BinaryOperatorType.In:
		  return " in ";
		case BinaryOperatorType.UnsignedShiftRight:
		  result = ">>>";
		  break;
		case BinaryOperatorType.ShiftRight:
		  result = ">>";
		  break;
		case BinaryOperatorType.ShiftLeft:
		  result = "<<";
		  break;
	  }
	  if (string.IsNullOrEmpty(result))
		return " ";
	  if (Options.Spacing.AroundOneCharOperators && result.Length == 1)
		return string.Format(" {0} ", result);
	  if (Options.Spacing.AroundTwoCharOperators && result.Length > 1)
		return string.Format(" {0} ", result);
	  return result;
	}
	protected override bool GenerateMacroExpression(Expression expression)
	{
	  string macroCall = expression.MacroCall;
	  if (macroCall == null || macroCall == String.Empty)
		return false;
	  Code.Write(macroCall);
	  return true;
	}
	protected virtual void GenerateTypeReferenceExpressionName(TypeReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  string name = expression.Name;
	  if (String.IsNullOrEmpty(name) && expression.IsDynamic)
		name = "dynamic";
	  if (String.IsNullOrEmpty(name))
		return;
	  Code.Write(name);
	}
	protected virtual void GenerateDateTimeMinValue()
	{
	  Code.Write("DateTime.MinValue");
	}
	protected virtual void GenerateUnaryOperator(UnaryOperatorExpression expression, string op)
	{
	  if (!expression.IsPostOperator)
		Code.Write(op);
	  CodeGen.GenerateElement(expression.Expression);
	  if (expression.IsPostOperator)
		Code.Write(op);
	}
	protected override void GenerateTypedElementReferenceExpression(TypedElementReferenceExpression expression)
	{
	}
	protected override void GenerateExpressionTypeArgument(ExpressionTypeArgument expression)
	{
	  GenerateElement(expression.SourceExpression);
	}
	protected override void GenerateTypeOfIsExpression(TypeOfIsExpression expression)
	{
	}
	protected override void GenerateAttributeVariableInitializer(AttributeVariableInitializer expression)
	{
	}
	protected override void GenerateIsNot(IsNot expression)
	{
	  return;
	}
	protected override void GenerateConditionalExpression(ConditionalExpression expression)
	{
	  CodeGen.GenerateElement(expression.Condition);
	  bool wrap = Options.WrappingAlignment.WrapTernaryExpression;
	  bool spaceAroundTernary = Options.Spacing.AroundTernaryQuestionOperator;
	  if (spaceAroundTernary)
		Code.Write(" ");
	  Code.Write("?");
	  if (wrap)
	  {
		Code.IncreaseIndent();
		Code.WriteLine();
	  }
	  else if (spaceAroundTernary)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.TrueExpression);
	  bool spaceArounColon = Options.Spacing.AroundTernaryColonOperator;
	  if (spaceArounColon)
		Code.Write(" ");
	  Code.Write(":");
	  if (wrap)
		Code.WriteLine();
	  else if (spaceArounColon)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.FalseExpression);
	  if (wrap)
		Code.DecreaseIndent();
	}
	protected override void GenerateThisReferenceExpression(ThisReferenceExpression expression)
	{
	  Code.Write("this");
	}
	protected override void GenerateMyClassExpression(MyClassExpression expression)
	{	  
	}
	protected override void GenerateTypeReferenceExpression(TypeReferenceExpression expression)
	{
	  if (expression.BaseType != null)
		CodeGen.GenerateElement(expression.BaseType);
	  else if (expression.Qualifier != null)
		CodeGen.GenerateElement(expression.Qualifier);
	  if (!String.IsNullOrEmpty(expression.Name))
	  {
		if (expression.BaseType != null || (expression.Qualifier != null &&
		  expression.Qualifier.ElementType != LanguageElementType.QualifiedAliasExpression))
		  WriteDot();
	  }
	  GenerateTypeReferenceExpressionName(expression);
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	  else if (expression.IsArrayType)
	  {
		if (Options.Spacing.BeforeArrayRankBrackets)
		  Code.Write(" ");
		Code.Write("[");
		if (Options.Spacing.WithinArrayRankEmptyBrackets && expression.Rank < 2)
		  Code.Write(" ");
		bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets && expression.Rank > 1;
		if (withinArrayRankBracketsSpacing)
		  Code.Write(" ");
		for (int i = 1; i < expression.Rank; i++)
		  Code.Write(GetCommaDelimeter());
		if (withinArrayRankBracketsSpacing)
		  Code.Write(" ");
		Code.Write("]");
	  }
	  else if (expression.IsPointerType)
	  {
		if (Options.Spacing.BeforeUnsafePointerDeclarationOperator)
		  Code.Write(" ");
		Code.Write("*");
	  }
	}
	protected override void GenerateIndexerExpression(IndexerExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  if (Options.Spacing.BeforeOpenSquareBracket)
		Code.Write(" ");
	  Code.Write("[");
	  bool withinSquareBracketsSpacing = Options.Spacing.WithinSquareBrackets;
	  if (withinSquareBracketsSpacing)
		Code.Write(" ");
	  GenerateExpressionList(expression.Arguments);
	  if (withinSquareBracketsSpacing)
		Code.Write(" ");
	  Code.Write("]");
	}
	protected override void GenerateLogicalInversion(LogicalInversion expression)
	{
	  Code.Write("!");
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateAddressOfExpression(AddressOfExpression expression)
	{
	  Code.Write("&");
	  if (Options.Spacing.AroundUnsafeOperators)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateAssignmentExpression(AssignmentExpression expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  GenerateAssignmentOperatorText(Code, expression.AssignmentOperator);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateTypeCastExpression(TypeCastExpression expression)
	{
	  Code.Write("(");
	  bool withinTypeCastParenthesesSpacing = Options.Spacing.WithinTypeCastParentheses;
	  if (withinTypeCastParenthesesSpacing)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.TypeReference);
	  if (withinTypeCastParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	  if (Options.Spacing.AfterTypeCastParentheses)
		Code.Write(" ");
	  Expression lTarget = expression.Target;
	  if (lTarget != null)
		CodeGen.GenerateElement(lTarget);
	}
	protected override void GenerateArrayInitializerExpression(ArrayInitializerExpression expression)
	{
	  Code.Write("{");
	  bool spacingWithinBraces = Options.Spacing.WithinArrayInitializerBraces;
	  if (spacingWithinBraces)
		Code.Write(" ");
	  ElementsGenerationRules rules = new ElementsGenerationRules();
	  rules.AlignIfWrap = Options.WrappingAlignment.AlignWithFirstArrayObjectOrCollectionInitializerItem;
	  rules.WrapFirst = Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer;
	  rules.WrapParams = Options.WrappingAlignment.WrapArrayObjectAndCollectionInitializers;
	  rules.Indenting = Options.Indention.ArrayObjectAndCollectionInitializerContents;
	  rules.StringDelimiter = GetCommaDelimeter();
	  GenerateElementsByRules(expression.Initializers, rules);
	  if (spacingWithinBraces)
		Code.Write(" ");
	  Code.Write("}");
	}
	protected override void GenerateUnaryIncrement(UnaryIncrement expression)
	{
	  GenerateUnaryOperator(expression, "++");
	}
	protected override void GenerateUnaryDecrement(UnaryDecrement expression)
	{
	  GenerateUnaryOperator(expression, "--");
	}
	protected override void GenerateArrayCreateExpression(ArrayCreateExpression expression)
	{
	  if (!expression.IsStackAlloc)
		Code.Write("new ");
	  else
		Code.Write("stackalloc ");
	  CodeGen.GenerateElement(expression.BaseType);
	  bool lBaseTypeIsArray = expression.BaseType != null && expression.BaseType.IsArrayType;
	  bool hasDimensions = expression.Dimensions != null && expression.Dimensions.Count > 0;
	  bool withinArrayRankEmptyBracketsSpacing = Options.Spacing.WithinArrayRankEmptyBrackets && !hasDimensions;
	  bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets;
	  if (!lBaseTypeIsArray)
	  {
		Code.Write("[");
		if (withinArrayRankEmptyBracketsSpacing)
		  Code.Write(" ");
	  }
	  if (hasDimensions)
	  {
		if (lBaseTypeIsArray)
		  Code.Write("[");
		if (withinArrayRankBracketsSpacing)
		  Code.Write(" ");
		GenerateElementCollection(expression.Dimensions, GetCommaDelimeter());
		if (withinArrayRankBracketsSpacing)
		  Code.Write(" ");
		if (lBaseTypeIsArray)
		  Code.Write("]");
	  }
	  if (!lBaseTypeIsArray)
		Code.Write("]");
	  if (expression.Initializer != null && Code.Options.Indention.ArrayObjectAndCollectionInitializerContents)
		Code.WriteLine();
	  CodeGen.GenerateElement(expression.Initializer);
	}
	protected override void GenerateObjectCreationExpression(ObjectCreationExpression expression)
	{
	  if (expression == null)
		return;
	  Code.Write("new");
	  Expression objType = expression.ObjectType;
	  if (objType != null)
	  {
		Code.Write(" ");
		CodeGen.GenerateElement(objType);
	  }
	  ExpressionCollection arguments = expression.Arguments;
	  if (arguments != null)
	  {
		Code.Write("(");
		GenerateElementCollection(expression.Arguments, GetCommaDelimeter());
		Code.Write(")");
	  }
	  ObjectInitializerExpression init = expression.ObjectInitializer;
	  if (init == null)
		return;
	  bool objectInitializerContentsIdention = Code.Options.Indention.ArrayObjectAndCollectionInitializerContents;
	  if (objectInitializerContentsIdention)
		Code.WriteLine();
	  else
		Code.Write(" ");
	  CodeGen.GenerateElement(init);
	}
	protected override void GenerateAnonymousMethodExpression(AnonymousMethodExpression expression)
	{
	  Code.Write("delegate");
	  bool hasParams = expression.Parameters != null && expression.Parameters.Count > 0;
	  if (hasParams)
		GenerateParameters(expression.Parameters);
	  int lNodesCount = expression.NodeCount;
	  bool within = Options.Spacing.WithinSingleLineAnonymousMethod && lNodesCount < 2;
	  bool onSingleLine = Options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && lNodesCount < 2;
	  bool openBraceOnNewLine = !onSingleLine && Options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousMethods;
	  bool indention = Options.Indention.AnonymousMethodContents;
	  WriteOpenBrace(openBraceOnNewLine,
		!openBraceOnNewLine && Options.Spacing.BeforeOpenCurlyBraceOnTheSameLine,
		onSingleLine && within,
		indention);
	  if (!onSingleLine && lNodesCount > 0)
		Code.WriteLine();
	  if (lNodesCount > 0)
		if (lNodesCount == 1)
		  CodeGen.GenerateCode(Code, (LanguageElement)expression.Nodes[0]);
		else
		  GenerateElementCollection(expression.Nodes, String.Empty);
	  WriteCloseBrace(!onSingleLine && Options.LineBreaks.PlaceCloseBraceOnNewLineForAnonymousMethods,
		onSingleLine && within,
		indention);
	}
	protected override void GenerateDateTime(object value)
	{
	  if ((DateTime)value == DateTime.MinValue)
		GenerateDateTimeMinValue();
	  else
	  {
		DateTime dtValue = (DateTime)value;
		if (dtValue.Second > 0 || dtValue.Minute > 0 || dtValue.Hour > 0)
		{
		  if (dtValue.Millisecond > 0)
			Code.Write("new DateTime({0}, {1}, {2}, {3}, {4}, {5}, {6})", dtValue.Year, dtValue.Month, dtValue.Day, dtValue.Hour, dtValue.Minute, dtValue.Second, dtValue.Millisecond);
		  else
			Code.Write("new DateTime({0}, {1}, {2}, {3}, {4}, {5})", dtValue.Year, dtValue.Month, dtValue.Day, dtValue.Hour, dtValue.Minute, dtValue.Second);
		}
		else
		  Code.Write("new DateTime({0}, {1}, {2})", dtValue.Year, dtValue.Month, dtValue.Day);
	  }
	}
	protected override void GenerateSizeOfExpression(SizeOfExpression expression)
	{
	  Code.Write("sizeof");
	  if (Options.Spacing.BeforeTypeofSizeofParentheses)
		Code.Write(" ");
	  Code.Write("(");
	  bool withinTypeofSizeofParenthesesSpacing = Options.Spacing.WithinTypeofSizeofParentheses;
	  if (withinTypeofSizeofParenthesesSpacing)
		Code.Write(" ");
	  CodeGen.GenerateElement(expression.TypeReference);
	  if (withinTypeofSizeofParenthesesSpacing)
		Code.Write(" ");
	  Code.Write(")");
	}
	protected override void GenerateFunctionPointerTypeReference(FunctionPointerTypeReference expression)
	{
	}
	public static void GenerateAssignmentOperatorText(CodeWriter codeWriter, AssignmentOperatorType op)
	{
	  string code = string.Empty;
	  switch (op)
	  {
		case AssignmentOperatorType.Assignment:
		  code = "=";
		  break;
		case AssignmentOperatorType.BackSlashEquals:
		  code = "/="; 
		  break;
		case AssignmentOperatorType.BitAndEquals:
		  code = "&=";
		  break;
		case AssignmentOperatorType.BitOrEquals:
		  code = "|=";
		  break;
		case AssignmentOperatorType.MinusEquals:
		  code = "-=";
		  break;
		case AssignmentOperatorType.None:
		  break;
		case AssignmentOperatorType.PercentEquals:
		  code = "%=";
		  break;
		case AssignmentOperatorType.PlusEquals:
		  code = "+=";
		  break;
		case AssignmentOperatorType.ShiftLeftEquals:
		  code = "<<=";
		  break;
		case AssignmentOperatorType.ShiftRightEquals:
		  code = ">>=";
		  break;
		case AssignmentOperatorType.SlashEquals:
		  code = "/=";
		  break;
		case AssignmentOperatorType.StarEquals:
		  code = "*=";
		  break;
		case AssignmentOperatorType.XorEquals:
		  code = "^=";
		  break;
		case AssignmentOperatorType.UnsignedShiftRightEquals:
		  code = ">>>=";
		  break;
	  }
	  if (string.IsNullOrEmpty(code))
		return;
	  if (codeWriter.Options.Spacing.AroundOneCharOperators && code.Length == 1)
	  {
		codeWriter.Write(" " + code + " ");
		return;
	  }
	  if (codeWriter.Options.Spacing.AroundTwoCharOperators && code.Length > 1)
		code = " " + code + " ";
	  codeWriter.Write(code);
	}
  }
}
