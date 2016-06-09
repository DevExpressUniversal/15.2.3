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
		public enum ResultType
		{
			Invalid,
			Number,
			String,
			Nothing			
		}
		static object PrepareExp(object exp)
		{
			if (exp == null)
				return exp;
			if (exp is bool)
				return GetNumberFromCondition((bool)exp);
			return exp;
		}
		public static object Eval(object lExp, object rExp, int operatorType)
		{
			if (lExp == null && rExp == null)
				return null;
			lExp = PrepareExp(lExp);
			rExp = PrepareExp(rExp);
			ResultType ppType = GetConversationType(lExp, rExp);
			object result = ProcessOperation(operatorType, ppType, lExp, rExp);
			return result;
		}
		static object ProcessOperation(int opType, ResultType ppType, Object lExp, Object rExp)
		{
			if (ppType == ResultType.Invalid)
				return GetZeroInt();
			if (ppType == ResultType.Nothing)
				return GetZeroInt();
			if (ppType == ResultType.Number)
				return ProcessOperation(opType, GetNumber(lExp), GetNumber(rExp));
			if (ppType == ResultType.String)
				return ProcessOperation(opType, GetString(lExp), GetString(rExp));
			return GetZeroInt();
		}
		public static PpResult GetResult(object exp)
		{
			if (exp == null)
				return PpResult.Invalid;
			if (exp is PpResult)
				return (PpResult)exp;
			if (exp is bool)
				return GetPpResult((bool)exp);
			if (exp is string)
				return PpResult.Invalid;
			Number num = exp as Number;
			if (num != null)
				return GetPpResult(num.IsTrue());
			if (exp is Nothing)
				return PpResult.False;
			return PpResult.Invalid;
		}
		static PpResult GetPpResult(bool cond)
		{
			if (cond)
				return PpResult.True;
			return PpResult.False;
		}
		static ResultType GetConversationType(object lExp, object rExp)
		{
			ResultType lType = GetType(lExp);
			if (rExp == null)
				return lType;
			ResultType rType = GetType(rExp);
			return GetConversationType(lType, rType);
		}
		static ResultType GetConversationType(ResultType lType, ResultType rType)
		{
			if (lType == ResultType.Invalid || rType == ResultType.Invalid)
				return ResultType.Invalid;
			if (lType == ResultType.String)
			{
				if (rType == ResultType.String || rType == ResultType.Nothing)
					return ResultType.String;
				return ResultType.Invalid;
			}
			else if (lType == ResultType.Number)
			{
				if (rType == ResultType.Number || rType == ResultType.Nothing)
					return ResultType.Number;
				return ResultType.Invalid;
			}
			else if (lType == ResultType.Nothing)
			{
				if (rType == ResultType.Number)
					return ResultType.Number;
				if (lType == ResultType.String)
					return ResultType.String;
				return ResultType.Nothing;
			}
			return ResultType.Invalid;
		}
		static ResultType GetType(object exp)
		{
			if (exp == null)
				return ResultType.Invalid;
			if (exp is string)
				return ResultType.String;
			if (exp is Number)
				return ResultType.Number;
			if (exp is Nothing)
				return ResultType.Nothing;
			return ResultType.Invalid;
		}
	}
}
