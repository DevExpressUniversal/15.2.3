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
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Filtering;
namespace DevExpress.PivotGrid.DataCalculation {
	public static class PivotSummaryTypeExtension {
		public static Aggregate ToAggregate(this SummaryItemTypeEx ex) {
			switch(ex) {
				case SummaryItemTypeEx.Average:
					return Aggregate.Avg;
					  case SummaryItemTypeEx.Count:
					return Aggregate.Count;
				case SummaryItemTypeEx.Max:
					return Aggregate.Max;
				case SummaryItemTypeEx.Min:
					return Aggregate.Min;
				case SummaryItemTypeEx.Sum:
					return Aggregate.Sum;
				default:
					throw new ArgumentException("SummaryItemTypeEx");
			}
		}
		public static bool IsTop(this  SummaryItemTypeEx ex) {
			return ex == SummaryItemTypeEx.TopPercent || ex == SummaryItemTypeEx.Top;
		}
		public static bool IsTopNonPercentage(this SummaryItemTypeEx ex) {
			return ex == SummaryItemTypeEx.Bottom || ex == SummaryItemTypeEx.Top;
		}
		public static bool IsTopPercentage(this SummaryItemTypeEx ex) {
			return ex == SummaryItemTypeEx.BottomPercent || ex == SummaryItemTypeEx.TopPercent;
		}
		public static PivotSummaryType ToPivotSummaryType(this SummaryItemTypeEx ex) {
			switch(ex) {
				case SummaryItemTypeEx.Average:
					return PivotSummaryType.Average;
				case SummaryItemTypeEx.Sum:
					return PivotSummaryType.Sum;
				case SummaryItemTypeEx.Count:
					return PivotSummaryType.Count;
				case SummaryItemTypeEx.Max:
					return PivotSummaryType.Max;
				case SummaryItemTypeEx.Min:
					return PivotSummaryType.Min;
				case SummaryItemTypeEx.Bottom:
				case SummaryItemTypeEx.BottomPercent:
				case SummaryItemTypeEx.Custom:
				case SummaryItemTypeEx.Duplicate:
				case SummaryItemTypeEx.None:
				case SummaryItemTypeEx.Top:
				case SummaryItemTypeEx.TopPercent:
				case SummaryItemTypeEx.Unique:
					return PivotSummaryType.Custom;
				default:
					throw new Exception("SummaryItemTypeEx");
			}
		}
	}
}
