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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB.Preprocessor
#else
namespace DevExpress.CodeParser.VB.Preprocessor
#endif
{
	public abstract partial class Evaluator
	{
		static Number GetNumber(object val)
		{
			if (val == null)
				return null;
			Number num = val as Number;
			if (num != null)
				return num;
			Nothing noth = val as Nothing;
			if (noth != null)
				return Nothing.GetNumber();
			return GetZeroInt();
		}
		#region GetZeroInt
		public static IntNum GetZeroInt()
		{
			IntNum lResult = new IntNum();
			lResult.Item = 0;
			return lResult;
		}
		#endregion
		static object OrElse(Number lExp, Number rExp)
		{
			bool bl = lExp.IsTrue();
			bool br = rExp.IsTrue();
			bool result = bl || br;
			return GetNumberFromCondition(result);
		}
		static object AndAlso(Number lExp, Number rExp)
		{
			bool bl = lExp.IsTrue();
			bool br = rExp.IsTrue();
			bool result = bl && br;
			return GetNumberFromCondition(result);
		}
		static object NotEquals(Number lExp, Number rExp)
		{
			bool result = !lExp.IsEqualTo(rExp);
			return GetNumberFromCondition(result);
		}
		static object Equals(Number lExp, Number rExp)
		{
			bool result = lExp.IsEqualTo(rExp);
			return GetNumberFromCondition(result);
		}
		static object ProcessBinaryOperation(int type, Number lExp, Number rExp)
		{
			switch (type)
			{
				case Tokens.Xor:
					return lExp.Xor(rExp); ;
				case Tokens.Or:
					return lExp.Or(rExp);
				case Tokens.OrElse:
					return OrElse(lExp, rExp);
				case Tokens.And:
					return lExp.And(rExp);
				case Tokens.AndAlso:
					return AndAlso(lExp, rExp);
				case Tokens.NotEquals:
					return NotEquals(lExp, rExp);
				case Tokens.EqualsSymbol:
					return Equals(lExp, rExp);
				case Tokens.LessThan:
					return lExp.IsBelow(rExp);
				case Tokens.GreaterThan:
					return lExp.IsGreater(rExp);
				case Tokens.LessOrEqual:
					return lExp.IsBelowOrEqual(rExp);
				case Tokens.GreaterOrEqual:
					return lExp.IsGreaterOrEqual(rExp);
				case Tokens.ShiftLeft:
					return lExp.LeftShift(rExp);
				case Tokens.ShiftRight:
					return lExp.RightShift(rExp);
				case Tokens.BitAnd:
					return lExp.And(rExp);
				case Tokens.Plus:
					return lExp.Add(rExp);
				case Tokens.Minus:
					return lExp.Sub(rExp);
				case Tokens.Mod:
					return lExp.ModDiv(rExp);
				case Tokens.BackSlash:
					return lExp.Div(rExp);
				case Tokens.Asterisk:
					return lExp.Mul(rExp);
				case Tokens.Slash:
					return lExp.Div(rExp);
			}
			return GetZeroInt();
		}
		static object Not(Number lExp)
		{
			bool result = !GetConditionFromNumber(lExp);
			return GetNumberFromCondition(result);
		}
		static object ProcessUnaryOperation(int type, Number lExp)
		{
			switch (type)
			{
				case Tokens.Not:
					return Not(lExp);
				case Tokens.Minus:
					return lExp.MinusSign(); ;
			}
			return GetZeroInt();
		}
	}
}
