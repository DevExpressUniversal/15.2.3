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
using DevExpress.Data;
namespace DevExpress.PivotGrid.DataCalculation {
	public class AggregationLevel : List<AggregationCalculation> {
		 public static void SetErrorValue(IList<AggregationLevel> levels) {
			foreach(AggregationLevel level in levels)
				foreach(AggregationCalculation calc in level)
					foreach(AggregationItemValue item in calc)
						item.SetValue(DevExpress.XtraPivotGrid.Data.PivotCellValue.ErrorValue.Value);
		}
		int row, column;
		public AggregationLevel(IEnumerable<AggregationCalculation> calcs, int rowLevel, int columnLevel) : base(calcs) {
			this.row = rowLevel;
			this.column = columnLevel;
		}
		public int Row { get { return row; } }
		public int Column { get { return column; } }
		public override bool Equals(object obj) {
			AggregationLevel item = obj as AggregationLevel;
			return item != null && item.Row == row && item.Column == column;
		}
		public override int GetHashCode() {
			return row * 100 & column;
		}
	}
	public enum AggregationCalculatationTarget {
		Data,
		Row,
		Column
	}
	public class AggregationCalculation : List<AggregationItemValue> {
		readonly AggregationCalculatationTarget target;
		readonly int index;
		public AggregationCalculation(IEnumerable<AggregationItemValue> items, int index)
			: this(items, AggregationCalculatationTarget.Data, index) {
		}
		public AggregationCalculation(IEnumerable<AggregationItemValue> items, AggregationCalculatationTarget target, int index)
			: base(items) {
			this.target = target;
			this.index = index;
		}
		public AggregationCalculatationTarget Target { get { return target; } }
		public int Index { get { return index; } }
		public override bool Equals(object obj) {
			AggregationCalculation item = obj as AggregationCalculation;
			if(item == null)
				return false;
			return Index == item.Index;
		}
		public override int GetHashCode() {
			return index;
		}
	}
	public class AggregationItem {
		readonly SummaryItemTypeEx summaryType;
		readonly decimal summaryArgument;
		public SummaryItemTypeEx SummaryType { get { return summaryType; } }
		public decimal SummaryArgument { get { return summaryArgument; } }
		decimal SummaryArgumentCore { get { return summaryArgument > 0 ? summaryArgument / 100 : summaryArgument; } }
		public AggregationItem(SummaryItemTypeEx summaryType, decimal summaryArgument) {
			this.summaryType = summaryType;
			this.summaryArgument = summaryArgument;
		}
		public override bool Equals(object obj) {
			AggregationItem item = obj as AggregationItem;
			if(item == null)
				return false;
			return item.SummaryArgument == SummaryArgument && item.SummaryType == SummaryType;
		}
		public override int GetHashCode() {
			return SummaryArgument.GetHashCode() & SummaryType.GetHashCode();
		}
		public Data.Filtering.Aggregate ToAggregate() {
			return SummaryType.ToAggregate();
		}	   
		public int GetUnpercentedValue(object count) {
			return Convert.ToInt32(SummaryArgumentCore * Convert.ToInt32(count));
		}
	}
	public class AggregationItemValue : AggregationItem {
		readonly Action<object> setAction;
		bool isCalculated;
		public bool IsCalculated { get { return isCalculated; } }
		public AggregationItemValue(SummaryItemTypeEx summaryType, decimal summaryArgument, Action<object> setAction) : base(summaryType, summaryArgument) {
			this.setAction = setAction;
		}
		public void SetValue(object value) {
			setAction(value);
			isCalculated = true;
		}
	}
}
