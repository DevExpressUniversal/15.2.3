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

using DevExpress.XtraPivotGrid;
namespace DevExpress.Data.PivotGrid {
	public class PivotFilteredValues {
		NullableHashtable values;
		PivotFilterType filteredType;
		bool showBlanks;
		public PivotFilteredValues(NullableHashtable values, PivotFilterType filteredType, bool showBlanks) {
			if(values == null)
				values = new NullableHashtable();
			this.values = values;
			this.filteredType = filteredType;
			this.showBlanks = showBlanks;
		}
		public PivotFilteredValues()
			: this(null, PivotFilterType.Excluded, false) {
		}
		public NullableHashtable Values { get { return values; } }
		public PivotFilterType FilteredType { get { return filteredType; } }
		public bool ShowBlanks { get { return showBlanks; } }
		public bool IsValueFit(object value) {
			if(value == null)
				return ShowBlanks;
			else return FilteredType == PivotFilterType.Included ? Values.ContainsKey(value) : !Values.ContainsKey(value);
		}
	}
	public class PivotSummaryFilteredValues {
		readonly PivotSummaryFilter filter;
		readonly int rowLevel;
		readonly int columnLevel;
		public PivotSummaryFilteredValues(PivotSummaryFilter filter, int rowLevel, int columnLevel) {
			this.filter = filter;
			this.rowLevel = rowLevel;
			this.columnLevel = columnLevel;
		}
		public PivotSummaryFilterMode Mode {
			get { return filter.Mode; }
		}
		public int RowLevel {
			get { return rowLevel; }
		}
		public int ColumnLevel {
			get { return columnLevel; }
		}
		public bool IsValueFit(object value, PivotValueComparerBase comparer) {
			int resStart = (filter.StartValueInternal == null) ? -1 : comparer.UnsafeCompare(filter.StartValueInternal, value),
				resEnd = (filter.EndValueInternal == null) ? 1 : comparer.UnsafeCompare(filter.EndValueInternal, value);
			if(resStart == PivotValueComparer.CompareError 
				|| resEnd == PivotValueComparer.CompareError)
				return false;
			return resStart <= 0 && resEnd >= 0;
		}
	}
	public delegate object GetRowValueDelegate(int listSourceRow, int columnIndex);
	public class PivotGroupFilteredValues {
		PivotGroupFilterValuesCollection values;
		PivotFilterType filterType;
		int[] columnIndexes;
		public PivotGroupFilteredValues(PivotFilterType filterType, PivotGroupFilterValuesCollection values, int[] columnIndexes) {
			this.values = values;
			this.filterType = filterType;
			this.columnIndexes = columnIndexes;
		}
		public bool IsRowFit(GetRowValueDelegate getValueDelegate, int listRowIndex) {
			this.getValueDelegate = getValueDelegate;
			this.listRowIndex = listRowIndex;
			try {
				bool isMatch = IsRowMatch();
				return filterType == PivotFilterType.Included ? isMatch : !isMatch;
			} finally {
				this.getValueDelegate = null;
			}
		}
		GetRowValueDelegate getValueDelegate;
		int listRowIndex;
		protected object GetValue(int level) {
			return getValueDelegate(listRowIndex, columnIndexes[level]);
		}
		protected bool IsRowMatch() {
			int level = 0;
			PivotGroupFilterValuesCollection levelValues = this.values;
			while(level < columnIndexes.Length && levelValues.Count > 0) {
				object value = GetValue(level);
				PivotGroupFilterValue filterValue = levelValues[value];
				if(filterValue == null) return false;
				levelValues = filterValue.ChildValues;
				level++;
			}
			return true;
		}
	}
}
