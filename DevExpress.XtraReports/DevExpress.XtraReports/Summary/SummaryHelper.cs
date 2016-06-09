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
using System.Text;
using System.Collections;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.Summary {
	public class SummaryHelper {
		static bool IsDateTimeValues(IList values) {
			int count = values.Count;
			for (int i = 0; i < count; i++) {
				object obj = values[i];
				if (obj != null && !(obj is DBNull))
					return obj is DateTime;
			}
			return false;
		}
		static bool IsTimeSpanValues(IList values) {
			int count = values.Count;
			for (int i = 0; i < count; i++) {
				object obj = values[i];
				if (obj != null && !(obj is DBNull))
					return obj is TimeSpan;
			}
			return false;
		}
		internal static object CalcResult(SummaryFunc func, IList values) {
			if (func == SummaryFunc.Count || func == SummaryFunc.RecordNumber) return values.Count;
			if (func == SummaryFunc.DCount) return EvaluatorBase.CalcDistinctCount(values);
			try {
				EvaluatorBase evaluator;
				if (IsDateTimeValues(values))
					evaluator = new DateTimeEvaluator();
				else if (IsTimeSpanValues(values))
					evaluator = new TimeSpanEvaluator();
				else
					evaluator = new DecimalEvaluator();
				object result = null;
				switch (func) {
					case SummaryFunc.Percentage:
					case SummaryFunc.RunningSum:
					case SummaryFunc.Sum:
						return evaluator.Sum(values);
					case SummaryFunc.DSum:
						return evaluator.DSum(values);
					case SummaryFunc.Max:
						result = evaluator.Max(values);
						break;
					case SummaryFunc.Min:
						result = evaluator.Min(values);
						break;
					case SummaryFunc.Median:
						result = evaluator.Median(values);
						break;
					case SummaryFunc.Avg:
						result = evaluator.Avg(values);
						break;
					case SummaryFunc.DAvg:
						result = evaluator.DAvg(values);
						break;
					case SummaryFunc.Var:
						result = evaluator.Var(values);
						break;
					case SummaryFunc.Count:
						result = values.Count;
						break;
					case SummaryFunc.DVar:
						result = evaluator.DVar(values);
						break;
					case SummaryFunc.VarP:
						result = evaluator.VarP(values);
						break;
					case SummaryFunc.DVarP:
						result = evaluator.DVarP(values);
						break;
					case SummaryFunc.StdDev:
						result = evaluator.StdDev(values);
						break;
					case SummaryFunc.DStdDev:
						result = evaluator.DStdDev(values);
						break;
					case SummaryFunc.StdDevP:
						result = evaluator.StdDevP(values);
						break;
					case SummaryFunc.DStdDevP:
						result = evaluator.DStdDevP(values);
						break;
					default: return null;
				}
				return result;
			}
			catch { return null; }
		}
		internal static object CalcResult(SortingSummaryFunction function, IList values) {
			if (function == SortingSummaryFunction.Count) return values.Count;
			if (function == SortingSummaryFunction.DCount) return EvaluatorBase.CalcDistinctCount(values);
			try {
				EvaluatorBase evaluator;
				if (IsDateTimeValues(values))
					evaluator = new DateTimeEvaluator();
				else if (IsTimeSpanValues(values))
					evaluator = new TimeSpanEvaluator();
				else
					evaluator = new DecimalEvaluator();
				object result = null;
				switch (function) {
					case SortingSummaryFunction.Sum:
						return evaluator.Sum(values);
					case SortingSummaryFunction.DSum:
						return evaluator.DSum(values);
					case SortingSummaryFunction.Max:
						result = evaluator.Max(values);
						break;
					case SortingSummaryFunction.Min:
						result = evaluator.Min(values);
						break;
					case SortingSummaryFunction.Median:
						result = evaluator.Median(values);
						break;
					case SortingSummaryFunction.Avg:
						result = evaluator.Avg(values);
						break;
					case SortingSummaryFunction.DAvg:
						result = evaluator.DAvg(values);
						break;
					case SortingSummaryFunction.Var:
						result = evaluator.Var(values);
						break;
					case SortingSummaryFunction.Count:
						result = values.Count;
						break;
					case SortingSummaryFunction.DVar:
						result = evaluator.DVar(values);
						break;
					case SortingSummaryFunction.VarP:
						result = evaluator.VarP(values);
						break;
					case SortingSummaryFunction.DVarP:
						result = evaluator.DVarP(values);
						break;
					case SortingSummaryFunction.StdDev:
						result = evaluator.StdDev(values);
						break;
					case SortingSummaryFunction.DStdDev:
						result = evaluator.DStdDev(values);
						break;
					case SortingSummaryFunction.StdDevP:
						result = evaluator.StdDevP(values);
						break;
					case SortingSummaryFunction.DStdDevP:
						result = evaluator.DStdDevP(values);
						break;
					default: return null;
				}
				return result;
			}
			catch { return null; }
		}
	}
}
