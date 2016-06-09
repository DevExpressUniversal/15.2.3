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
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public sealed class CSharpOperatorHelper
	{
		static OperatorType GetUnaryOperatorType(string name)
		{
			switch (name)
			{
				case "--":
					return OperatorType.Decrement;
				case "++":
					return OperatorType.Increment;
				case "-":
					return OperatorType.UnaryNegation;
				case "+":
					return OperatorType.UnaryPlus;
				case "!":
					return OperatorType.LogicalNot;
				case "true":
					return OperatorType.True;
				case "false":
					return OperatorType.False;
				case "&":
					return OperatorType.AddressOf;
				case "~":
					return OperatorType.OnesComplement;
				case "*":
					return OperatorType.PointerDereference;
			}
			return OperatorType.None;
		}
		static OperatorType GetBinaryOperatorType(string name)
		{
			switch (name)
			{
				case "+":
					return OperatorType.Addition;
				case "-":
					return OperatorType.Subtraction;
				case "*":
					return OperatorType.Multiply;
				case "/":
					return OperatorType.Division;
				case "%":
					return OperatorType.Modulus;
				case "^":
					return OperatorType.ExclusiveOr;
				case "&":
					return OperatorType.BitwiseAnd;
				case "|":
					return OperatorType.BitwiseOr;
				case "&&":
					return OperatorType.LogicalAnd;
				case "||":
					return OperatorType.LogicalOr;
				case "=":
					return OperatorType.Assign;
				case "<<":
					return OperatorType.LeftShift;
				case ">>":
					return OperatorType.RightShift;
				case "==":
					return OperatorType.Equality;
				case ">":
					return OperatorType.GreaterThan;
				case "<":
					return OperatorType.LessThan;
				case "!=":
					return OperatorType.Inequality;
				case ">=":
					return OperatorType.GreaterThanOrEqual;
				case "<=":
					return OperatorType.LessThanOrEqual;
				case "->":
					return OperatorType.MemberSelection;
				case ">>=":
					return OperatorType.RightShiftAssignment;
				case "*=":
					return OperatorType.MultiplicationAssignment;
				case "->*":
					return OperatorType.PointerToMemberSelection;
				case "-=":
					return OperatorType.SubtractionAssignment;
				case "^=":
					return OperatorType.ExclusiveOrAssignment;
				case "<<=":
					return OperatorType.LeftShiftAssignment;
				case "%=":
					return OperatorType.ModulusAssignment;
				case "+=":
					return OperatorType.AdditionAssignment;
				case "&=":
					return OperatorType.BitwiseAndAssignment;
				case "|=":
					return OperatorType.BitwiseOrAssignment;
				case ",":
					return OperatorType.Comma;
				case "/=":
					return OperatorType.DivisionAssignment;
			}
			return OperatorType.None;
		}
	static OperatorType GetUnaryOperatorTypeByMethodName(string methodName)
	{
	  switch (methodName)
	  {
		case "op_Decrement":
		  return OperatorType.Decrement;
		case "op_Increment":
		  return OperatorType.Increment;
		case "op_UnaryNegation":
		  return OperatorType.UnaryNegation;
		case "op_UnaryPlus":
		  return OperatorType.UnaryPlus;
		case "op_LogicalNot":
		  return OperatorType.LogicalNot;
		case "op_True":
		  return OperatorType.True;
		case "op_False":
		  return OperatorType.False;
		case "op_AddressOf":
		  return OperatorType.AddressOf;
		case "op_OnesComplement":
		  return OperatorType.OnesComplement;
		case "op_PointerDereference":
		  return OperatorType.PointerDereference;
	  }
	  return OperatorType.None;
	}
	static OperatorType GetBinaryOperatorTypeByMethodName(string methodName)
	{
	  switch (methodName)
	  {
		case "op_Addition":
		  return OperatorType.Addition;
		case "op_Subtraction":
		  return OperatorType.Subtraction;
		case "op_Multiply":
		  return OperatorType.Multiply;
		case "op_Division":
		  return OperatorType.Division;
		case "op_Modulus":
		  return OperatorType.Modulus;
		case "op_ExclusiveOr":
		  return OperatorType.ExclusiveOr;
		case "op_BitwiseAnd":
		  return OperatorType.BitwiseAnd;
		case "op_BitwiseOr":
		  return OperatorType.BitwiseOr;
		case "op_LogicalAnd":
		  return OperatorType.LogicalAnd;
		case "op_LogicalOr":
		  return OperatorType.LogicalOr;
		case "op_Assign":
		  return OperatorType.Assign;
		case "op_LeftShift":
		  return OperatorType.LeftShift;
		case "op_RightShift":
		  return OperatorType.RightShift;
		case "op_Equality":
		  return OperatorType.Equality;
		case "op_GreaterThan":
		  return OperatorType.GreaterThan;
		case "op_LessThan":
		  return OperatorType.LessThan;
		case "op_Inequality":
		  return OperatorType.Inequality;
		case "op_GreaterThanOrEqual":
		  return OperatorType.GreaterThanOrEqual;
		case "op_LessThanOrEqual":
		  return OperatorType.LessThanOrEqual;
		case "op_MemberSelection":
		  return OperatorType.MemberSelection;
		case "op_RightShiftAssignment":
		  return OperatorType.RightShiftAssignment;
		case "op_MultiplicationAssignment":
		  return OperatorType.MultiplicationAssignment;
		case "op_PointerToMemberSelection":
		  return OperatorType.PointerToMemberSelection;
		case "op_SubtractionAssignment":
		  return OperatorType.SubtractionAssignment;
		case "op_ExclusiveOrAssignment":
		  return OperatorType.ExclusiveOrAssignment;
		case "op_LeftShiftAssignment":
		  return OperatorType.LeftShiftAssignment;
		case "op_ModulusAssignment":
		  return OperatorType.ModulusAssignment;
		case "op_AdditionAssignment":
		  return OperatorType.AdditionAssignment;
		case "op_BitwiseAndAssignment":
		  return OperatorType.BitwiseAndAssignment;
		case "op_BitwiseOrAssignment":
		  return OperatorType.BitwiseOrAssignment;
		case "op_Comma":
		  return OperatorType.Comma;
		case "op_DivisionAssignment":
		  return OperatorType.DivisionAssignment;
	  }
	  return OperatorType.None;
	}
	public static string GetBinaryOperatorText(OperatorType type)
	{
	  switch (type)
	  {
		case OperatorType.Addition:
		  return "+";
		case OperatorType.Subtraction:
		  return "-";
		case OperatorType.Multiply:
		  return "*";
		case OperatorType.Division:
		  return "/";
		case OperatorType.Modulus:
		  return "%";
		case OperatorType.ExclusiveOr:
		  return "^";
		case OperatorType.BitwiseAnd:
		  return "&";
		case OperatorType.BitwiseOr:
		  return "|";
		case OperatorType.LogicalAnd:
		  return "&&";
		case OperatorType.LogicalOr:
		  return "||";
		case OperatorType.Assign:
		  return "=";
		case OperatorType.LeftShift:
		  return "<<";
		case OperatorType.RightShift:
		  return ">>";
		case OperatorType.Equality:
		  return "==";
		case OperatorType.GreaterThan:
		  return ">";
		case OperatorType.LessThan:
		  return "<";
		case OperatorType.Inequality:
		  return "!=";
		case OperatorType.GreaterThanOrEqual:
		  return ">=";
		case OperatorType.LessThanOrEqual:
		  return "<=";
		case OperatorType.MemberSelection:
		  return "->";
		case OperatorType.RightShiftAssignment:
		  return ">>=";
		case OperatorType.MultiplicationAssignment:
		  return "*=";
		case OperatorType.PointerToMemberSelection:
		  return "->*";
		case OperatorType.SubtractionAssignment:
		  return "-=";
		case OperatorType.ExclusiveOrAssignment:
		  return "^=";
		case OperatorType.LeftShiftAssignment:
		  return "<<=";
		case OperatorType.ModulusAssignment:
		  return "%=";
		case OperatorType.AdditionAssignment:
		  return "+=";
		case OperatorType.BitwiseAndAssignment:
		  return "&=";
		case OperatorType.BitwiseOrAssignment:
		  return "|=";
		case OperatorType.Comma:
		  return ",";
		case OperatorType.DivisionAssignment:
		  return "/=";
	  }
	  return string.Empty;
	}
		static string GetUnaryOperatorName(OperatorType type)
		{
			switch (type)
			{
				case OperatorType.Decrement:
					return "op_Decrement";
				case OperatorType.Increment:
					return "op_Increment";
				case OperatorType.UnaryNegation:
					return "op_UnaryNegation";
				case OperatorType.UnaryPlus:
					return "op_UnaryPlus";
				case OperatorType.LogicalNot:
					return "op_LogicalNot";
				case OperatorType.True:
					return "op_True";
				case OperatorType.False:
					return "op_False";
				case OperatorType.AddressOf:
					return "op_AddressOf";
				case OperatorType.OnesComplement:
					return "op_OnesComplement";
				case OperatorType.PointerDereference:
					return "op_PointerDereference";
			}
			return String.Empty;
		}
		static string GetBinaryOperatorName(OperatorType type)
		{
			switch (type)
			{
				case OperatorType.Addition:
					return "op_Addition";
				case OperatorType.Subtraction:
					return "op_Subtraction";
				case OperatorType.Multiply:
					return "op_Multiply";
				case OperatorType.Division:
					return "op_Division";
				case OperatorType.Modulus:
					return "op_Modulus";
				case OperatorType.ExclusiveOr:
					return "op_ExclusiveOr";
				case OperatorType.BitwiseAnd:
					return "op_BitwiseAnd";
				case OperatorType.BitwiseOr:
					return "op_BitwiseOr";
				case OperatorType.LogicalAnd:
					return "op_LogicalAnd";
				case OperatorType.LogicalOr:
					return "op_LogicalOr";
				case OperatorType.Assign:
					return "op_Assign";
				case OperatorType.LeftShift:
					return "op_LeftShift";
				case OperatorType.RightShift:
					return "op_RightShift";
				case OperatorType.Equality:
					return "op_Equality";
				case OperatorType.GreaterThan:
					return "op_GreaterThan";
				case OperatorType.LessThan:
					return "op_LessThan";
				case OperatorType.Inequality:
					return "op_Inequality";
				case OperatorType.GreaterThanOrEqual:
					return "op_GreaterThanOrEqual";
				case OperatorType.LessThanOrEqual:
					return "op_LessThanOrEqual";
				case OperatorType.MemberSelection:
					return "op_MemberSelection";
				case OperatorType.RightShiftAssignment:
					return "op_RightShiftAssignment";
				case OperatorType.MultiplicationAssignment:
					return "op_MultiplicationAssignment";
				case OperatorType.PointerToMemberSelection:
					return "op_PointerToMemberSelection";
				case OperatorType.SubtractionAssignment:
					return "op_SubtractionAssignment";
				case OperatorType.ExclusiveOrAssignment:
					return "op_ExclusiveOrAssignment";
				case OperatorType.LeftShiftAssignment:
					return "op_LeftShiftAssignment";
				case OperatorType.ModulusAssignment:
					return "op_ModulusAssignment";
				case OperatorType.AdditionAssignment:
					return "op_AdditionAssignment";
				case OperatorType.BitwiseAndAssignment:
					return "op_BitwiseAndAssignment";
				case OperatorType.BitwiseOrAssignment:
					return "op_BitwiseOrAssignment";
				case OperatorType.Comma:
					return "op_Comma";
				case OperatorType.DivisionAssignment:
					return "op_DivisionAssignment";
			}
			return String.Empty;
		}
		public static OperatorType GetOperatorType(string value, int parameterCount)
		{
			if (parameterCount == 1)
				return GetUnaryOperatorType(value);
			else if (parameterCount == 2)
				return GetBinaryOperatorType(value);
			return OperatorType.None;
		}
	public static OperatorType GetOperatorTypeByMethodName(string methodName)
	{
	  OperatorType result = GetUnaryOperatorTypeByMethodName(methodName);
	  if(result != OperatorType.None)
		return result;
	  return GetBinaryOperatorTypeByMethodName(methodName);
	}
	public static OperatorType GetOperatorTypeByMethodName(string methodName, int parameterCount)
	{
	  if (parameterCount == 1)
		return GetUnaryOperatorTypeByMethodName(methodName);
	  else if (parameterCount == 2)
		return GetBinaryOperatorTypeByMethodName(methodName);
	  return OperatorType.None;
	}
		public static string GetOperatorName(OperatorType type, int parameterCount)
		{
			if (parameterCount == 1)
				return GetUnaryOperatorName(type);
			else if (parameterCount == 2)
				return GetBinaryOperatorName(type);
			return "";
		}
	public static FormattingTokenType GetOperatorToken(OperatorType type)
	{
	  switch (type)
	  {
		case OperatorType.None: return FormattingTokenType.None;
		case OperatorType.Decrement: return FormattingTokenType.MinusMinus;
		case OperatorType.Increment: return FormattingTokenType.PlusPlus;
		case OperatorType.UnaryNegation: return FormattingTokenType.Minus;
		case OperatorType.UnaryPlus: return FormattingTokenType.Plus;
		case OperatorType.LogicalNot: return FormattingTokenType.Exclamation;
		case OperatorType.True: return FormattingTokenType.True;
		case OperatorType.False: return FormattingTokenType.False;
		case OperatorType.AddressOf: return FormattingTokenType.Ampersand;
		case OperatorType.OnesComplement: return FormattingTokenType.Tilde;
		case OperatorType.PointerDereference: return FormattingTokenType.Asterisk;
		case OperatorType.Addition: return FormattingTokenType.Plus;
		case OperatorType.Subtraction: return FormattingTokenType.Minus;
		case OperatorType.Multiply: return FormattingTokenType.Asterisk;
		case OperatorType.Division: return FormattingTokenType.Slash;
		case OperatorType.Modulus: return FormattingTokenType.Percent;
		case OperatorType.ExclusiveOr: return FormattingTokenType.Caret;
		case OperatorType.BitwiseAnd: return FormattingTokenType.Ampersand;
		case OperatorType.BitwiseOr: return FormattingTokenType.VerticalBar;
		case OperatorType.LogicalAnd: return FormattingTokenType.AmpersandAmpersand;
		case OperatorType.LogicalOr: return FormattingTokenType.VerticalBarVerticalBar;
		case OperatorType.Assign: return FormattingTokenType.Equal;
		case OperatorType.LeftShift: return FormattingTokenType.LessThanLessThan;
		case OperatorType.RightShift: return FormattingTokenType.GreatThanGreatThan;
		case OperatorType.SignedRightShift: return FormattingTokenType.GreatThanGreatThan;
		case OperatorType.UnsignedRightShift: return FormattingTokenType.GreatThanGreatThan;
		case OperatorType.Equality: return FormattingTokenType.EqualEqual;
		case OperatorType.GreaterThan: return FormattingTokenType.GreaterThen;
		case OperatorType.LessThan: return FormattingTokenType.LessThan;
		case OperatorType.Inequality: return FormattingTokenType.ExclamationEqual;
		case OperatorType.GreaterThanOrEqual: return FormattingTokenType.GreaterThenEqual;
		case OperatorType.LessThanOrEqual: return FormattingTokenType.LessThanEqual;
		case OperatorType.UnsignedRightShiftAssignment: return FormattingTokenType.GreatThanGreatThanEqual;
		case OperatorType.MemberSelection: return FormattingTokenType.Dot;
		case OperatorType.RightShiftAssignment: return FormattingTokenType.GreatThanGreatThanEqual;
		case OperatorType.MultiplicationAssignment: return FormattingTokenType.AsteriskEqual;
		case OperatorType.PointerToMemberSelection: return FormattingTokenType.MinusGreaterThenAsterisk;
		case OperatorType.SubtractionAssignment: return FormattingTokenType.MinusEqual;
		case OperatorType.ExclusiveOrAssignment: return FormattingTokenType.CaretEqual;
		case OperatorType.LeftShiftAssignment: return FormattingTokenType.LessThanLessThanEqual;
		case OperatorType.ModulusAssignment: return FormattingTokenType.PercentEqual;
		case OperatorType.AdditionAssignment: return FormattingTokenType.PlusEqual;
		case OperatorType.BitwiseAndAssignment: return FormattingTokenType.AmpersandEqual;
		case OperatorType.BitwiseOrAssignment: return FormattingTokenType.VerticalBarEqual;
		case OperatorType.Comma: return FormattingTokenType.Comma;
		case OperatorType.DivisionAssignment: return FormattingTokenType.SlashEqual;
		case OperatorType.Concatenate: return FormattingTokenType.Plus;
		case OperatorType.Exponent: return FormattingTokenType.Caret;
		case OperatorType.IntegerDivision: return FormattingTokenType.Slash;
		case OperatorType.Like: return FormattingTokenType.None;
		default: return FormattingTokenType.None;
	  }
	}
	public static BinaryOperatorExpression GetBinaryOperator(Expression left, OperatorType type, Expression right)
	{
	  switch (type)
	  {
		case OperatorType.Addition:
		case OperatorType.Subtraction:
		case OperatorType.Multiply:
		case OperatorType.Division:
		case OperatorType.Modulus:
		  return new BinaryOperatorExpression(left, GetBinaryOperatorText(type), right);
		case OperatorType.ExclusiveOr:
		  return new LogicalOperation(left, LogicalOperator.ExclusiveOr, right);
		case OperatorType.BitwiseAnd:
		  return new LogicalOperation(left, LogicalOperator.And, right);
		case OperatorType.BitwiseOr:
		  return new LogicalOperation(left, LogicalOperator.Or, right);
		case OperatorType.LogicalAnd:
		  return new LogicalOperation(left, LogicalOperator.ShortCircuitAnd, right);
		case OperatorType.LogicalOr:
		  return new LogicalOperation(left, LogicalOperator.ShortCircuitOr, right);
		case OperatorType.Assign:
		  return new AssignmentExpression(left, GetBinaryOperatorText(type), right);
		case OperatorType.LeftShift:
		case OperatorType.RightShift:
		  return new BinaryOperatorExpression(left, GetBinaryOperatorText(type), right);
		case OperatorType.Equality:
		  return new RelationalOperation(left, RelationalOperator.Equality, right);
		case OperatorType.GreaterThan:
		  return new RelationalOperation(left, RelationalOperator.GreaterThan, right);
		case OperatorType.LessThan:
		  return new RelationalOperation(left, RelationalOperator.LessThan, right);
		case OperatorType.Inequality:
		  return new RelationalOperation(left, RelationalOperator.Inequality, right);
		case OperatorType.GreaterThanOrEqual:
		  return new RelationalOperation(left, RelationalOperator.GreaterOrEqual, right);
		case OperatorType.LessThanOrEqual:
		  return new RelationalOperation(left, RelationalOperator.LessOrEqual, right);
		case OperatorType.PointerToMemberSelection:
		  return null; 
		case OperatorType.MemberSelection:
		  return null; 
		case OperatorType.LeftShiftAssignment:
		case OperatorType.RightShiftAssignment:
		case OperatorType.MultiplicationAssignment:
		case OperatorType.SubtractionAssignment:
		case OperatorType.ExclusiveOrAssignment:
		case OperatorType.ModulusAssignment:
		case OperatorType.AdditionAssignment:
		case OperatorType.BitwiseAndAssignment:
		case OperatorType.BitwiseOrAssignment:
		case OperatorType.DivisionAssignment:
		  return new AssignmentExpression(left, GetBinaryOperatorText(type), right);
		case OperatorType.Comma:
		  return null;
	  }
	  return null;
	}
	}
}
