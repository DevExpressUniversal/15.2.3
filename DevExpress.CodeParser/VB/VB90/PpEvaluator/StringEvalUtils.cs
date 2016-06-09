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
namespace DevExpress.CodeRush.StructuralParser.VB.Preprocessor
#else
namespace DevExpress.CodeParser.VB.Preprocessor
#endif
{
	public abstract partial class Evaluator
	{
		static string GetString(object val)
		{
			if (val == null)
				return null;
			string result = val as string;
			if (result != null)
				return result;
			return String.Empty;
		}
		static object ProcessBinaryOperation(int type, string lExp, string rExp)
		{
			switch (type)
			{
				case Tokens.NotEquals:
					return lExp != rExp;
				case Tokens.EqualsSymbol:
					return lExp == rExp;
				case Tokens.Plus:
					return lExp + rExp;
				case Tokens.GreaterThan:
					return GetNumberFromCondition(String.Compare(lExp, rExp, StringComparison.CurrentCulture) > 0);
				case Tokens.GreaterOrEqual:
					return GetNumberFromCondition(String.Compare(lExp, rExp, StringComparison.CurrentCulture) >= 0);
				case Tokens.LessThan:
					return GetNumberFromCondition(String.Compare(lExp, rExp, StringComparison.CurrentCulture) < 0);
				case Tokens.LessOrEqual:
					return GetNumberFromCondition(String.Compare(lExp, rExp, StringComparison.CurrentCulture) <= 0);
			}
			return GetZeroInt();
		}
		static bool GetConditionFromNumber(Number num)
		{
			if (num == null)
				return false;
			return num.IsTrue();
		}
		public static Number GetNumberFromCondition(bool cond)
		{
			Number num = new IntNum();
			if (cond)
				num.Item = -1;
			else
				num.Item = 0;
			return num;
		}
	}
}
