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
  public abstract class TokenExpressionCodeGenBase : ExpressionCodeGenBase
  {
	public TokenExpressionCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected virtual void GenerateExpressionList(ExpressionCollectionBase expressions, FormattingTokenType delimiter)
	{
	  GenerateExpressionList(expressions, delimiter, false);
	}
	protected virtual void GenerateExpressionList(ExpressionCollectionBase expressions, FormattingTokenType delimiter, bool lineBreakAfterDelimiter)
	{
	  if (expressions == null)
		return;
	  for (int i = 0; i < expressions.Count; i++)
	  {
		CodeGen.GenerateElement(expressions[i] as Expression);
		if (i < expressions.Count - 1)
		  Write(delimiter);
	  }
	}
	protected virtual FormattingTokenType GetBinaryOperatorToken(BinaryOperatorType op)
	{
	  switch (op)
	  {
		case BinaryOperatorType.Add:
		  return FormattingTokenType.Plus;
		case BinaryOperatorType.Append:
		  return FormattingTokenType.PlusEqual;
		case BinaryOperatorType.Assign:
		  return FormattingTokenType.Equal;
		case BinaryOperatorType.BitwiseAnd:
		  return FormattingTokenType.Ampersand;
		case BinaryOperatorType.BitwiseOr:
		  return FormattingTokenType.VerticalBar;
		case BinaryOperatorType.BooleanAnd:
		  return FormattingTokenType.AmpersandAmpersand;
		case BinaryOperatorType.BooleanOr:
		  return FormattingTokenType.VerticalBarVerticalBar;
		case BinaryOperatorType.ExclusiveOr:
		  return FormattingTokenType.Caret;
		case BinaryOperatorType.Divide:
		  return FormattingTokenType.Slash;
		case BinaryOperatorType.GreaterThan:
		  return FormattingTokenType.GreaterThen;
		case BinaryOperatorType.GreaterThanOrEqual:
		  return FormattingTokenType.GreaterThenEqual;
		case BinaryOperatorType.IdentityEquality:
		case BinaryOperatorType.ValueEquality:
		  return FormattingTokenType.EqualEqual;
		case BinaryOperatorType.LessThan:
		  return FormattingTokenType.LessThan;
		case BinaryOperatorType.LessThanOrEqual:
		  return FormattingTokenType.LessThanEqual;
		case BinaryOperatorType.IntegerDivision:
		case BinaryOperatorType.Modulus:
		  return FormattingTokenType.Percent;
		case BinaryOperatorType.Multiply:
		  return FormattingTokenType.Asterisk;
		case BinaryOperatorType.Subtract:
		  return FormattingTokenType.Minus;
		case BinaryOperatorType.StrictEquality:
		  return FormattingTokenType.EqualEqualEqual;
		case BinaryOperatorType.StrictInequality:
		  return FormattingTokenType.ExclamationEqualEqual;
		case BinaryOperatorType.In:
		  return FormattingTokenType.In;
		case BinaryOperatorType.UnsignedShiftRight:
		  return FormattingTokenType.GreatThanGreatThanGreatThan;
		case BinaryOperatorType.ShiftRight:
		  return FormattingTokenType.GreatThanGreatThan;
		case BinaryOperatorType.ShiftLeft:
		  return FormattingTokenType.LessThanLessThan;
		case BinaryOperatorType.IdentityInequality:
		  return FormattingTokenType.ExclamationEqual;
	  }
	  return FormattingTokenType.None;
	}
	protected virtual FormattingTokenType GetOperatorToken(UnaryOperatorExpression expression)
	{
	  UnaryOperatorType op = expression.UnaryOperator;
	  switch (op)
	  {
		case UnaryOperatorType.PointerDereference:
		  return FormattingTokenType.Asterisk;
		case UnaryOperatorType.OnesComplement:
		  return FormattingTokenType.Tilde;
		case UnaryOperatorType.UnaryNegation:
		  return FormattingTokenType.Minus;
		case UnaryOperatorType.UnaryPlus:
		  return FormattingTokenType.Plus;
		default:
		  string opText = expression.OperatorText;
		  switch (opText)
		  {
			case "*":
			  return FormattingTokenType.Asterisk;
			case "-":
			  return FormattingTokenType.Minus;
			case "+":
			  return FormattingTokenType.Plus;
			case "~":
			  return FormattingTokenType.Tilde;
			default:
			  return FormattingTokenType.None;
		  }
	  }
	}	
	protected override void GenerateReferenceExpressionBase(ReferenceExpressionBase expression, string delimiter)
	{
	  Expression lSource = expression.Qualifier;
	  if (lSource != null)
	  {
		CodeGen.GenerateElement(lSource);
		if (expression.ElementType != LanguageElementType.QualifiedAliasExpression)
		  Write(delimiter);
	  }
	  Write(FormattingTokenType.Ident);
	  if (expression.IsGeneric)
		GenerateGenericTypeArguments(expression.TypeArguments);
	}
	protected override void GenerateMethodCallExpression(MethodCallExpression expression)
	{
	  CodeGen.GenerateElement(expression.Qualifier);
	  Write(FormattingTokenType.ParenOpen); 
	  GenerateExpressionList(expression.Arguments);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GenerateShortInitializeExpression(ShortInitializeExpression expression)
	{
	  Write(FormattingTokenType.ParenOpen); 
	  GenerateExpressionList(expression.Arguments);
	  Write(FormattingTokenType.ParenClose); 
	}
	protected override void GenerateParenthesizedExpression(ParenthesizedExpression expression)
	{
	  Write(FormattingTokenType.ParenOpen);
	  CodeGen.GenerateElement(expression.Expression);
	  Write(FormattingTokenType.ParenClose);
	}
	protected override void GeneratePrimitiveExpression(PrimitiveExpression expression)
	{
	  object value = expression.PrimitiveValue;
	  string name = expression.Name;
	  bool isEscapedString = IsEscapedString(name);
	  PrimitiveType type = expression.PrimitiveType;
	  CodeWritePrevFormattingElements(FormattingTokenType.Ident);
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
					Write(name);
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
	protected override void GenerateUnaryOperatorExpression(UnaryOperatorExpression expression)
	{
	  GenerateUnaryOperator(expression, GetOperatorToken(expression));
	}
	protected override void GenerateBinaryOperatorExpression(BinaryOperatorExpression expression)
	{
	  CodeGen.GenerateElement(expression.LeftSide);
	  GenerateBinaryOperator(expression);
	  CodeGen.GenerateElement(expression.RightSide);
	}
	protected void GenerateUnaryOperator(UnaryOperatorExpression expression, FormattingTokenType operatorType)
	{
	  if (!expression.IsPostOperator)
		Write(operatorType);
	  CodeGen.GenerateElement(expression.Expression);
	  if (expression.IsPostOperator)
		Write(operatorType);
	}
  }
}
