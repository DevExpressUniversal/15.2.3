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
  public abstract class TokenCExpressionCodeGenBase : TokenExpressionCodeGenBase
  {
	public TokenCExpressionCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	static FormattingTokenType GetTokenType(AssignmentOperatorType opType)
	{
	  switch (opType)
	  {
		case AssignmentOperatorType.Assignment:
		  return FormattingTokenType.Equal;
		case AssignmentOperatorType.BackSlashEquals:
		  return FormattingTokenType.BackSlashEqual;
		case AssignmentOperatorType.BitAndEquals:
		  return FormattingTokenType.AmpersandEqual;
		case AssignmentOperatorType.BitOrEquals:
		  return FormattingTokenType.VerticalBarEqual;
		case AssignmentOperatorType.MinusEquals:
		  return FormattingTokenType.MinusEqual;
		case AssignmentOperatorType.PercentEquals:
		  return FormattingTokenType.PercentEqual;
		case AssignmentOperatorType.PlusEquals:
		  return FormattingTokenType.PlusEqual;
		case AssignmentOperatorType.ShiftLeftEquals:
		  return FormattingTokenType.LessThanLessThanEqual;
		case AssignmentOperatorType.ShiftRightEquals:
		  return FormattingTokenType.GreatThanGreatThanEqual;
		case AssignmentOperatorType.SlashEquals:
		  return FormattingTokenType.SlashEqual;
		case AssignmentOperatorType.StarEquals:
		  return FormattingTokenType.AsteriskEqual;
		case AssignmentOperatorType.XorEquals:
		  return FormattingTokenType.CaretEqual;
		case AssignmentOperatorType.UnsignedShiftRightEquals:
		  return FormattingTokenType.GreatThanGreatThanGreatThanEqual;
	  }
	  return FormattingTokenType.None;
	}
	protected virtual void GenerateDateTimeMinValue()
	{
	  Write("DateTime.MinValue");
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
	protected override void GenerateTypedElementReferenceExpression(TypedElementReferenceExpression expression)
	{
	}
	protected override bool GenerateExpression(Expression expression)
	{
	  return base.GenerateExpressionCore(expression);
	}
	protected override bool GenerateMacroExpression(Expression expression)
	{
	  string macroCall = expression.MacroCall;
	  if (macroCall == null || macroCall == String.Empty)
		return false;
	  Code.Write(macroCall);
	  return true;
	}
	protected override void GeneratePrimitiveExpression(PrimitiveExpression expression)
	{
	  object value = expression.PrimitiveValue;
	  string name = expression.Name;
	  bool isEscapedString = IsEscapedString(name);
	  PrimitiveType type = expression.PrimitiveType;
	  if (type == PrimitiveType.Path && value != null)
	  {
		Code.Write(value.ToString());
		return;
	  }
	  if (value is bool)
		GenerateBooleanLiteral((bool)value);
	  else
		if (value == null && (name == null || name.Length == 0) || type == PrimitiveType.Void)
		  GenerateNullLiteral();
		else
		  if (expression.Literal != null)
			Write(expression.Literal);
		  else
			if (value is char)
			  GenerateCharLiteral((char)value);
			else
			  if (type == PrimitiveType.Char)
				Write(name);
			  else
				if (value is string || type == PrimitiveType.String)
				{
				  if (name != null && name != String.Empty && isEscapedString)
				  {
					Write((string)name, false);
				  }
				  else
				  {
					if (expression.CanHasEscapeSequence)
					  GenerateStringLiteralWithEscapeSequence((string)value);
					else
					  GenerateStringLiteral((string)value);
				  }
				}
				else
				  if (expression.IsNumberLiteral)
					GenerateNumberLiteral(name, value, expression.PrimitiveType);
				  else
					if (expression.IsDateTime && (name == null || name.Length == 0))
					  GenerateDateTime(value);
					else
					  Write(name);
	}
	protected override void GenerateExpressionTypeArgument(ExpressionTypeArgument expression)
	{
	  GenerateElement(expression.SourceExpression);
	}
	protected override void GenerateLogicalOperation(LogicalOperation expression)
	{
	  GenerateBinaryOperatorExpression(expression);
	}
	protected override void GenerateBinaryOperator(BinaryOperatorExpression expression)
	{
	  if (expression == null)
		return;
	  FormattingTokenType binaryType = GetBinaryOperatorToken(expression.BinaryOperator);
	  if (binaryType == FormattingTokenType.None && expression.OperatorText != null)
	  {
		string opText = String.Format(" {0} ", expression.OperatorText);
		Write(opText);
		return;
	  }
	  Write(binaryType);
	}
	protected override void GenerateBinaryOperator(BinaryOperatorType op)
	{
	  Write(GetBinaryOperatorToken(op));
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
	  using (GetIndent(Options.WrappingAlignment.WrapTernaryExpression))
	  {
		Write(FormattingTokenType.Question);
	  CodeGen.GenerateElement(expression.TrueExpression);
		Write(FormattingTokenType.Colon);
	  CodeGen.GenerateElement(expression.FalseExpression);
	}
	}
	protected override void GenerateThisReferenceExpression(ThisReferenceExpression expression)
	{
	  Write(FormattingTokenType.This);
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
	  if (HasName(expression))
	  {
		if (expression.BaseType != null || (expression.Qualifier != null &&
		  expression.Qualifier.ElementType != LanguageElementType.QualifiedAliasExpression))
		  Write(FormattingTokenType.Dot);
		GenerateName(expression);
	  }
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	  else if (expression.IsArrayType)
	  {
		bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets && expression.Rank > 1;
		Write(FormattingTokenType.BracketOpen);
		for (int i = 1; i < expression.Rank; i++)
		  Write(FormattingTokenType.Comma, i - 1);
		Write(FormattingTokenType.BracketClose);
	  }
	  else if (expression.IsPointerType)
		Write(FormattingTokenType.Asterisk);
	}
	protected override void GenerateIndexerExpression(IndexerExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  Write(FormattingTokenType.BracketOpen);
	  GenerateExpressionList(expression.Arguments);
	  Write(FormattingTokenType.BracketClose);
	}
	protected override void GenerateLogicalInversion(LogicalInversion expression)
	{
	  Write(FormattingTokenType.Exclamation);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateAddressOfExpression(AddressOfExpression expression)
	{
	  Write(FormattingTokenType.Ampersand);
	  CodeGen.GenerateElement(expression.Expression);
	}
	protected override void GenerateAssignmentExpression(AssignmentExpression expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  GenerateAssignmentOperatorText(this);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected override void GenerateTypeCastExpression(TypeCastExpression expression)
	{
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
	  Expression lTarget = expression.Target;
	  if (lTarget != null)
		CodeGen.GenerateElement(lTarget);
	}
	protected override void GenerateArrayInitializerExpression(ArrayInitializerExpression expression)
	{
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Initializers, FormattingTokenType.Comma);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateUnaryIncrement(UnaryIncrement expression)
	{
	  GenerateUnaryOperator(expression, FormattingTokenType.PlusPlus);
	}
	protected override void GenerateUnaryDecrement(UnaryDecrement expression)
	{
	  GenerateUnaryOperator(expression, FormattingTokenType.MinusMinus);
	}
	protected override void GenerateArrayCreateExpression(ArrayCreateExpression expression)
	{
	  if (!expression.IsStackAlloc)
		Write(FormattingTokenType.New);
	  else
		Write(FormattingTokenType.Stackalloc);
	  CodeGen.GenerateElement(expression.BaseType);
	  bool lBaseTypeIsArray = expression.BaseType != null && expression.BaseType.IsArrayType;
	  bool hasDimensions = expression.Dimensions != null && expression.Dimensions.Count > 0;
	  if (!lBaseTypeIsArray)
		Write(FormattingTokenType.BracketOpen);
	  if (hasDimensions)
	  {
		Write(FormattingTokenType.BracketOpen);
		GenerateElementCollection(expression.Dimensions, FormattingTokenType.Comma);
		Write(FormattingTokenType.BracketClose);
	  }
	  if (!lBaseTypeIsArray)
		Write(FormattingTokenType.BracketClose);
	  CodeGen.GenerateElement(expression.Initializer);
	}
	protected override void GenerateObjectCreationExpression(ObjectCreationExpression expression)
	{
	  if (expression == null)
		return;
	  Write(FormattingTokenType.New);
	  Expression objType = expression.ObjectType;
	  CodeGen.GenerateElement(objType);
	  ExpressionCollection arguments = expression.Arguments;
	  if (arguments != null)
	  {
		Write(FormattingTokenType.ParenOpen);
		GenerateElementCollection(expression.Arguments, FormattingTokenType.Comma);
		Write(FormattingTokenType.ParenClose);
	  }
	  this.GenerateObjectInitializerExpression(expression.ObjectInitializer);
	}
	protected override void GenerateAnonymousMethodExpression(AnonymousMethodExpression expression)
	{
	  if (expression.IsAsynchronous)
		Write(FormattingTokenType.Async);
	  Write(FormattingTokenType.Delegate);
	  if (!expression.ParameterListOmitted)
		GenerateParameters(expression.Parameters);
	  Write(FormattingTokenType.CurlyBraceOpen);
	  GenerateElementCollection(expression.Nodes, FormattingTokenType.None);
	  Write(FormattingTokenType.CurlyBraceClose);
	}
	protected override void GenerateDateTime(object value)
	{
	  if ((DateTime)value == DateTime.MinValue)
		GenerateDateTimeMinValue();
	  else
	  {
		DateTime dtValue = (DateTime)value;
		string val = String.Empty;
		if (dtValue.Second > 0 || dtValue.Minute > 0 || dtValue.Hour > 0)
		  if (dtValue.Millisecond > 0)
			val = String.Format("new DateTime({0}, {1}, {2}, {3}, {4}, {5}, {6})", dtValue.Year, dtValue.Month, dtValue.Day, dtValue.Hour, dtValue.Minute, dtValue.Second, dtValue.Millisecond);
		  else
			val = String.Format("new DateTime({0}, {1}, {2}, {3}, {4}, {5})", dtValue.Year, dtValue.Month, dtValue.Day, dtValue.Hour, dtValue.Minute, dtValue.Second);
		else
		  val = String.Format("new DateTime({0}, {1}, {2})", dtValue.Year, dtValue.Month, dtValue.Day);
		Write(val);
	  }
	}
	protected override void GenerateSizeOfExpression(SizeOfExpression expression)
	{
	  Write(FormattingTokenType.Sizeof);
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.TypeReference);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateFunctionPointerTypeReference(FunctionPointerTypeReference expression)
	{
	}
	public virtual bool HasName(TypeReferenceExpression expression)
	{
	  if (expression == null)
		return false;
	  return !String.IsNullOrEmpty(expression.Name) || expression.IsDynamic;
	}
	public virtual void GenerateName(TypeReferenceExpression expression)
	{
	  if (expression == null)
		return;
	  if (expression.IsDynamic && String.IsNullOrEmpty(expression.Name))
	  {
		Write("dynamic");
		return;
	  }
	  Write(FormattingTokenType.Ident);
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Plus:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.Equal:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.VerticalBar:
		case FormattingTokenType.AmpersandAmpersand:
		case FormattingTokenType.VerticalBarVerticalBar:
		case FormattingTokenType.Caret:
		case FormattingTokenType.Slash:
		case FormattingTokenType.GreaterThen:
		case FormattingTokenType.GreaterThenEqual:
		case FormattingTokenType.EqualEqual:
		case FormattingTokenType.LessThan:
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.Percent:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.Minus:
		case FormattingTokenType.EqualEqualEqual:
		case FormattingTokenType.ExclamationEqualEqual:
		case FormattingTokenType.In:
		case FormattingTokenType.GreatThanGreatThanGreatThan:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.LessThanLessThan:
		case FormattingTokenType.ExclamationEqual:
		  if (ContextMatch(LanguageElementType.LogicalOperation) && Options.WrappingAlignment.WrapBeforeOperatorInLogicalExpression)
		  {
			result.AddNewLine();
			break;
		  }
		  if (ContextMatch(LanguageElementType.BinaryOperatorExpression) && Options.WrappingAlignment.WrapBeforeOperatorInBinaryExpression)
		  {
			result.AddNewLine();
			break;
		  }
		  if (tokenType != FormattingTokenType.Asterisk)
			break;
		  TypeReferenceExpression exp = Context as TypeReferenceExpression;
		  if (exp != null && !exp.IsGeneric && !exp.IsArrayType && exp.IsPointerType && Options.Spacing.BeforeUnsafePointerDeclarationOperator)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (Options.Spacing.BeforeCheckedUncheckedParentheses &&
			(ContextMatch(LanguageElementType.CheckedExpression) || ContextMatch(LanguageElementType.UncheckedExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.BeforeTypeofSizeofParentheses &&
			(ContextMatch(LanguageElementType.TypeOfExpression) || ContextMatch(LanguageElementType.SizeOfExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.BeforeDefaultParentheses && ContextMatch(LanguageElementType.DefaultValueExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenClose:
		  if (ContextMatch(LanguageElementType.ParenthesizedExpression) && Options.Spacing.WithinExpressionParentheses)
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.TypeCastExpression) && Options.Spacing.WithinTypeCastParentheses)
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinCheckedUncheckedParentheses &&
			(ContextMatch(LanguageElementType.CheckedExpression) || ContextMatch(LanguageElementType.UncheckedExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinTypeofSizeofParentheses &&
			(ContextMatch(LanguageElementType.TypeOfExpression) || ContextMatch(LanguageElementType.SizeOfExpression)))
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinDefaultParentheses && ContextMatch(LanguageElementType.DefaultValueExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketOpen:
		   exp = Context as TypeReferenceExpression;
		  if (exp != null && !exp.IsGeneric && exp.IsArrayType && Options.Spacing.BeforeArrayRankBrackets)
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.IndexerExpression) && Options.Spacing.BeforeOpenSquareBracket)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketClose:
		  exp = Context as TypeReferenceExpression;
		  if (exp != null && !exp.IsGeneric && exp.IsArrayType && Options.Spacing.WithinArrayRankBrackets && exp.Rank > 1)
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.IndexerExpression) && Options.Spacing.WithinSquareBrackets)
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.ArrayCreateExpression))
		  {
			ArrayCreateExpression expression = Context as ArrayCreateExpression;
			if (expression == null)
			  break;
			TypeReferenceExpression baseType = expression.BaseType;
			bool baseTypeIsArray = baseType != null && baseType.IsArrayType;
			int dimensionsCount = -1;
			if (expression.Dimensions != null)
			  dimensionsCount = expression.Dimensions.Count;
			bool hasDimensions = expression.Dimensions != null && expression.Dimensions.Count > 0;
			bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets && hasDimensions;
			if (dimensionsCount > 0 && baseTypeIsArray)
			{
			  if (withinArrayRankBracketsSpacing)
				result.AddWhiteSpace();
			}
			else
			{
			  if (hasDimensions && baseTypeIsArray && withinArrayRankBracketsSpacing)
				result.AddWhiteSpace();
			  if (!baseTypeIsArray && withinArrayRankBracketsSpacing)
				result.AddWhiteSpace();
			}
		  }
		  break;
		case FormattingTokenType.Question:
		  if (Options.Spacing.AroundTernaryQuestionOperator && ContextMatch(LanguageElementType.ConditionalExpression))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Options.Spacing.AroundTernaryColonOperator && ContextMatch(LanguageElementType.ConditionalExpression))
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Async:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenOpen:
		  if (ContextMatch(LanguageElementType.ParenthesizedExpression) && Options.Spacing.WithinExpressionParentheses)
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.TypeCastExpression) && Options.Spacing.WithinTypeCastParentheses)
			result.AddWhiteSpace();
		  else if (Options.Spacing.WithinTypeofSizeofParentheses &&
			(ContextMatch(LanguageElementType.TypeOfExpression) || ContextMatch(LanguageElementType.SizeOfExpression)))
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.ParenClose:
		  if (ContextMatch(LanguageElementType.TypeCastExpression) && Options.Spacing.AfterTypeCastParentheses)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketOpen:
		  TypeReferenceExpression exp = Context as TypeReferenceExpression;
		  if (exp != null)
		  {
			bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets && exp.Rank > 1;
			if (!exp.IsGeneric && exp.IsArrayType && (withinArrayRankBracketsSpacing ||
			(Options.Spacing.WithinArrayRankEmptyBrackets && exp.Rank < 2)))
			  result.AddWhiteSpace();
		  }
		  else if (ContextMatch(LanguageElementType.ArrayCreateExpression))
		  {
			ArrayCreateExpression expression = Context as ArrayCreateExpression;
			if (expression == null)
			  break;
			TypeReferenceExpression baseType = expression.BaseType;
			bool baseTypeIsArray = baseType != null && baseType.IsArrayType;
			int dimensionsCount = -1;
			if (expression.Dimensions != null)
			  dimensionsCount = expression.Dimensions.Count;
			bool hasDimensions = expression.Dimensions != null && expression.Dimensions.Count > 0;
			bool withinArrayRankEmptyBracketsSpacing = Options.Spacing.WithinArrayRankEmptyBrackets && !hasDimensions;
			bool withinArrayRankBracketsSpacing = Options.Spacing.WithinArrayRankBrackets && hasDimensions;
			if (dimensionsCount > 0 && baseTypeIsArray)
			{
			  if (withinArrayRankBracketsSpacing)
				result.AddWhiteSpace();
			}
			else
			{
			  if (!baseTypeIsArray && (withinArrayRankEmptyBracketsSpacing || withinArrayRankBracketsSpacing))
				result.AddWhiteSpace();
			  else if (hasDimensions && baseTypeIsArray && withinArrayRankBracketsSpacing)
				result.AddWhiteSpace();
			}
		  }
		  break;
		case FormattingTokenType.New:
		  if (ContextMatch(LanguageElementType.ArrayCreateExpression))
			result.AddWhiteSpace();
		  else if (ContextMatch(LanguageElementType.ObjectCreationExpression))
		  {
			ObjectCreationExpression expression = Context as ObjectCreationExpression;
			if (expression == null)
			  break;
			Expression objType = expression.ObjectType;
			if (objType == null)
			  break;
			bool isAnonymousType = objType == null;
			if (isAnonymousType)
			{
			  if (Options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousTypes)
				result.AddNewLine();
			}
			else
			  result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.Question:
		  if (ContextMatch(LanguageElementType.ConditionalExpression))
		  {
			if (Options.WrappingAlignment.WrapTernaryExpression)
			{
			  IncreaseIndent();
			  result.AddNewLine();
			}
			else
			  if (Options.Spacing.AroundTernaryQuestionOperator)
				result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.Colon:
		  if (ContextMatch(LanguageElementType.ConditionalExpression))
		  {
			if (Options.WrappingAlignment.WrapTernaryExpression)
			  result.AddNewLine();
			else
			  if (Options.Spacing.AroundTernaryColonOperator)
				result.AddWhiteSpace();
		  }
		  break;
		case FormattingTokenType.Plus:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.Equal:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.VerticalBar:
		case FormattingTokenType.AmpersandAmpersand:
		case FormattingTokenType.VerticalBarVerticalBar:
		case FormattingTokenType.Caret:
		case FormattingTokenType.Slash:
		case FormattingTokenType.GreaterThen:
		case FormattingTokenType.GreaterThenEqual:
		case FormattingTokenType.EqualEqual:
		case FormattingTokenType.LessThan:
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.Percent:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.Minus:
		case FormattingTokenType.EqualEqualEqual:
		case FormattingTokenType.ExclamationEqualEqual:
		case FormattingTokenType.In:
		case FormattingTokenType.GreatThanGreatThanGreatThan:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.LessThanLessThan:
		case FormattingTokenType.ExclamationEqual:
		  if (ContextMatch(LanguageElementType.LogicalOperation) && Options.WrappingAlignment.WrapLogicalOperatorExpression)
			result.AddNewLine();
		  else if (ContextMatch(LanguageElementType.BinaryOperatorExpression) && Options.WrappingAlignment.WrapBinaryOperatorExpression)
			  result.AddNewLine();
		  break;
	  }
	  return result;
	}
	public static void GenerateAssignmentOperatorText(LanguageElementCodeGenBase codeGen)
	{
	  if (codeGen == null)
		return;
	  Assignment assg = codeGen.Context as Assignment;
	  AssignmentOperatorType opType = AssignmentOperatorType.None;
	  if (assg != null)
		opType = assg.AssignmentOperator;
	  else
	  {
		AssignmentExpression aExp = codeGen.Context as AssignmentExpression;
		opType = aExp.AssignmentOperator;
	  }
	  codeGen.Write(GetTokenType(opType));
	}
  }
}
