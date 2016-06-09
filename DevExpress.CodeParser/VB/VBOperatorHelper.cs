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
	public sealed class VBOperatorHelper
	{
		public static FormattingTokenType GetOperatorNaturalName(string name)
		{
			if (name == null || name == String.Empty)
				return FormattingTokenType.Ident;
			switch (name.ToLower())
			{
		case "op_subtraction":
		case "op_unarynegation":
					return FormattingTokenType.Minus;
		case "op_addition":
		case "op_unaryplus":
					return FormattingTokenType.Plus;
				case "op_true":
					return FormattingTokenType.IsTrue;
				case "op_false":
					return FormattingTokenType.IsFalse;
				case "op_onescomplement":
					return FormattingTokenType.Not;
				case "op_multiply":
					return FormattingTokenType.Asterisk;
				case "op_division":
					return FormattingTokenType.Slash;
				case "op_integerdivision":
					return FormattingTokenType.BackSlash;
				case "op_modulus":
					return FormattingTokenType.Mod;
				case "op_exclusiveor":
					return FormattingTokenType.Xor;
				case "op_bitwiseand":
					return FormattingTokenType.And;
				case "op_bitwiseor":
					return FormattingTokenType.Or;
				case "op_leftshift":
					return FormattingTokenType.LessThanLessThan;
				case "op_rightshift":
					return FormattingTokenType.GreatThanGreatThan;
				case "op_equality":
					return FormattingTokenType.Equal;
				case "op_greaterthan":
					return FormattingTokenType.GreaterThen;
				case "op_lessthan":
					return FormattingTokenType.LessThan;
				case "op_inequality":
		  return FormattingTokenType.LessThanGreaterThan;
				case "op_greaterthanorequal":
					return FormattingTokenType.GreaterThenEqual;
				case "op_lessthanorequal":
					return FormattingTokenType.LessThanEqual;
				case "op_exponent":
					return FormattingTokenType.Caret;
				case "op_Like":
					return FormattingTokenType.Like;
				case "op_concatenate":
					return FormattingTokenType.Ampersand;
			}
			return FormattingTokenType.Ident;
		}
		static OperatorType GetUnaryOperatorType(string name)
		{
			switch (name.ToLower())
			{
				case "-":
					return OperatorType.UnaryNegation;
				case "+":
					return OperatorType.UnaryPlus;
				case "not":
					return OperatorType.OnesComplement;
				case "istrue":
					return OperatorType.True;
				case "isfalse":
					return OperatorType.False;
			}
			return OperatorType.None;
		}
		static OperatorType GetBinaryOperatorType(string name)
		{
			switch (name.ToLower())
			{
				case "+":
					return OperatorType.Addition;
				case "-":
					return OperatorType.Subtraction;
				case "*":
					return OperatorType.Multiply;
				case "/":
					return OperatorType.Division;
				case "\\":
					return OperatorType.IntegerDivision;
				case "mod":
					return OperatorType.Modulus;
				case "xor":
					return OperatorType.ExclusiveOr;
				case "and":
					return OperatorType.BitwiseAnd;
				case "or":
					return OperatorType.BitwiseOr;
				case "<<":
					return OperatorType.LeftShift;
				case ">>":
					return OperatorType.RightShift;
				case "=":
					return OperatorType.Equality;
				case ">":
					return OperatorType.GreaterThan;
				case "<":
					return OperatorType.LessThan;
				case "<>":
					return OperatorType.Inequality;
				case ">=":
					return OperatorType.GreaterThanOrEqual;
				case "<=":
					return OperatorType.LessThanOrEqual;
				case "^":
					return OperatorType.Exponent;
				case "like":
					return OperatorType.Like;
				case "&":
					return OperatorType.Concatenate;
			}
			return OperatorType.None;
		}
		static string GetUnaryOperatorName(OperatorType type)
		{
			switch (type)
			{
				case OperatorType.UnaryNegation:
					return "op_UnaryNegation";
				case OperatorType.UnaryPlus:
					return "op_UnaryPlus";
				case OperatorType.True:
					return "op_True";
				case OperatorType.False:
					return "op_False";
				case OperatorType.OnesComplement:
					return "op_OnesComplement";
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
				case OperatorType.IntegerDivision:
					return "op_IntegerDivision";
				case OperatorType.Modulus:
					return "op_Modulus";
				case OperatorType.ExclusiveOr:
					return "op_ExclusiveOr";
				case OperatorType.BitwiseAnd:
					return "op_BitwiseAnd";
				case OperatorType.BitwiseOr:
					return "op_BitwiseOr";
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
				case OperatorType.Exponent:
					return "op_Exponent";
				case OperatorType.Like:
					return "op_Like";
				case OperatorType.Concatenate:
					return "op_Concatenate";
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
		public static string GetOperatorName(OperatorType type, int parameterCount)
		{
			if (parameterCount == 1)
				return GetUnaryOperatorName(type);
			else if (parameterCount == 2)
				return GetBinaryOperatorName(type);
			return "";
		}
	}
}
