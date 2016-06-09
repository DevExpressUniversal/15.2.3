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
	public class IntNum : Number
	{
		protected Int32 IntItem
		{
			get 
			{ 
				return (Int32)Item;
			}
			set
			{
				Item = value;
			}
		}
		protected Number DoCalculate(Number param)
		{
			Number lResult=null;
			if (param is FloatNum)
			{
				lResult=new FloatNum();
				lResult.Item=Convert.ToDecimal(Item);
			}
			if (param is IntNum)			
			{
				lResult=new IntNum();
				lResult.Item=(param as IntNum).IntItem;
			}	
			return lResult;
		}
		protected  Number ReturnNumberForItem(Int32 item)
		{
			IntNum lResult = new IntNum();
			lResult.Item = item;
			return lResult;
		}
		public override Number Add(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=(lResult as IntNum).IntItem+IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).Add(param);
			return null;
		}
		public override Number Sub(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem-(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).Sub(param);
			return null;
		}
		public override Number Mul(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem*(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).Mul(param);
			return null;
		}
		public override Number Div(Number param)
		{
			Number lResult=DoCalculate(param);
			try
			{
				if (lResult is IntNum)
				{
					(lResult as IntNum).Item=IntItem/(lResult as IntNum).IntItem;
					return lResult;
				}
				if (lResult is FloatNum)
					return (lResult as FloatNum).Div(param);
				return null;
			}
			catch
			{
				return lResult;
			}
		}
		public override Number ModDiv(Number param)
		{
			Number lResult=DoCalculate(param);
			try
			{
				if (lResult is IntNum)
				{
					(lResult as IntNum).Item=IntItem%(lResult as IntNum).IntItem;
					return lResult;
				}
				if (lResult is FloatNum)
					return (lResult as FloatNum).ModDiv(param);
				return null;
			}
			catch
			{
				return lResult;
			}
		}
		public override Number MinusSign()
		{
			return ReturnNumberForItem(-(+IntItem));
		}
		public override Number PlusSign()
		{
			return ReturnNumberForItem(-(+IntItem));
		}
		public override Number Decrement()
		{
			return ReturnNumberForItem(IntItem-1);
		}
		public override Number Increment()
		{
			return ReturnNumberForItem(IntItem+1);
		}
		public override Number And(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem&(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).And(param);
			return null;
		}
		public override Number Or(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem|(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).Or(param);
			return null;
		}
		public override Number Xor(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem^(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).Xor(param);
			return null;
		}
		public override Number LeftShift(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem<<(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).LeftShift(param);
			return null;
		}
		public override Number RightShift(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				(lResult as IntNum).Item=IntItem>>(lResult as IntNum).IntItem;
				return lResult;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).RightShift(param);
			return null;
		}
		public override bool IsTrue()
		{
			return IntItem!=0;
		}
		public override bool IsEqualTo(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				return (lResult as IntNum).IntItem==IntItem;
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).IsEqualTo(param);
			return false;
		}
		public override bool IsBelow(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				return (IntItem<(lResult as IntNum).IntItem);
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).IsBelow(param);
			return false;
		}
		public override bool IsBelowOrEqual(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				return (IntItem<=(lResult as IntNum).IntItem);
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).IsBelowOrEqual(param);
			return false;
		}
		public override bool IsGreater(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				return (IntItem>(lResult as IntNum).IntItem);
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).IsGreater(param);
			return false;
		}
		public override bool IsGreaterOrEqual(Number param)
		{
			Number lResult=DoCalculate(param);
			if (lResult is IntNum)
			{
				return (IntItem>=(lResult as IntNum).IntItem);
			}
			if (lResult is FloatNum)
				return (lResult as FloatNum).IsGreaterOrEqual(param);
			return false;
		}
	}
}
