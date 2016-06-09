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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class FloatNum : Number
	{
		protected Decimal DecimalItem
		{
			get 
			{ 
				return (Decimal)Item;
			}
			set
			{
				Item = value;
			}
		}
		protected FloatNum GetZeroNumber()
		{
			FloatNum lResult= new FloatNum();
			lResult.Item=Decimal.Zero;
			return lResult;
		}
		protected FloatNum DoCalculate(Number param)
		{
			Decimal lDecParam=0;
			if (param is FloatNum) 
				lDecParam = (param as FloatNum).DecimalItem;
			if( param is IntNum)
				lDecParam = Convert.ToDecimal(param.Item);
			FloatNum lResult = new FloatNum();
			lResult.Item = lDecParam;
			return lResult;
		}
		public override Number Add(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			lResult.Item = DecimalItem +lResult.DecimalItem;
			return lResult;
		}
		public override Number Sub(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			lResult.Item=DecimalItem-lResult.DecimalItem;
			return lResult;
		}
		public override Number Mul(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			lResult.Item = DecimalItem*lResult.DecimalItem;
			return lResult;
		}
		public override Number Div(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			if ((Decimal)lResult.Item==0)
				return lResult;
			lResult.Item=DecimalItem/lResult.DecimalItem;
			return lResult;
		}
		public override Number ModDiv(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			if ((Decimal)lResult.Item==0)
				return lResult;
			lResult.Item=DecimalItem % lResult.DecimalItem;
			return lResult;
		}
		public override Number MinusSign()
		{
			FloatNum lResult = new FloatNum();
			lResult.Item=-(+(Decimal)Item);
			return lResult;
		}
		public override Number PlusSign()
		{
			FloatNum lResult = new FloatNum();
			lResult.Item=+lResult.DecimalItem;
			return lResult;
		}
		public override Number Decrement()
		{
			return GetZeroNumber();
		}
		public override Number Increment()
		{
			return GetZeroNumber();
		}
		public override Number And(Number param)
		{
			return GetZeroNumber();
		}
		public override Number Or(Number param)
		{
			return GetZeroNumber();
		}
		public override Number Xor(Number param)
		{
			return GetZeroNumber();
		}
		public override Number LeftShift(Number param)
		{
			return GetZeroNumber();
		}
		public override Number RightShift(Number param)
		{
			return GetZeroNumber();
		}
		public override bool IsTrue()
		{
			return DecimalItem!=0;
		}
		public override bool IsEqualTo(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			return (lResult.DecimalItem==DecimalItem);
		}
		public override bool IsBelow(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			return (DecimalItem<(lResult as FloatNum).DecimalItem);
		}
		public override bool IsBelowOrEqual(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			return (DecimalItem<=(lResult as FloatNum).DecimalItem);
		}
		public override bool IsGreater(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			return (DecimalItem>(lResult as FloatNum).DecimalItem);
		}
		public override bool IsGreaterOrEqual(Number param)
		{
			FloatNum lResult = DoCalculate(param);
			return (DecimalItem>=(lResult as FloatNum).DecimalItem);
		}
	}
}
