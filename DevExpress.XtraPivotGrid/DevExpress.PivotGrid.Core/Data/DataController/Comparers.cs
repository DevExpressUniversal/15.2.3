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
using System.Collections;
using System.Globalization;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
#if !SL
using System.ComponentModel;
#else
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.PivotGrid {
	public class DataControllerTotalSummaries : IEvaluatorDataAccess {
		PivotDataController controller;
		Hashtable values;
		public DataControllerTotalSummaries(PivotDataController controller) {
			values = new Hashtable();
			this.controller = controller;
		}
		public object this[object key] {
			get { return values[key]; }
			set { values[key] = value; }
		}
		public void Clear() { values.Clear(); }
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			GroupRowInfo rowInfo = (GroupRowInfo)theObject;
			SummaryPropertyDescriptor spd = descriptor as SummaryPropertyDescriptor;
			if(spd == null) return PivotSummaryValue.ErrorValue;
			PivotSummaryItem summaryItem = spd.SummaryItem;
			if(!values.ContainsKey(summaryItem))
				controller.CalcTotalSummaryValue(summaryItem);
			if(summaryItem != null) {
				object value = ((PivotSummaryValue)values[summaryItem]).GetValue(summaryItem.SummaryType);
				if(summaryItem.SummaryType == PivotSummaryType.Custom && (value is PivotGridCustomValues))
					((PivotGridCustomValues)value).TryGetValue(summaryItem.Name, out value);
				return value;
			} else {
				return ((IEvaluatorDataAccess)this).GetValue(descriptor, rowInfo.Index);
			}
		}
	}
	public class PivotValueComparerBase : ValueComparer, IComparer<object> {
		public PivotValueComparerBase() : base() { }
		public virtual int UnsafeCompare(object x, object y) {
			if(x == DBNull.Value) x = null;
			if(y == DBNull.Value) y = null;
			if(x == y) return 0;
			if(x == null) return -1;
			if(y == null) return 1;
			return UnsafeCompareCore(x, y);
		}
		protected virtual int UnsafeCompareCore(object x, object y) {
			return base.CompareCore(x, y);
		}
		#region IComparer<object> Members
		int IComparer<object>.Compare(object x, object y) {
			return UnsafeCompare(x, y);
		}
		#endregion
	}
	public class PivotValueComparer : PivotValueComparerBase {
		PivotDataController controller;
		public static int CompareError { get { return -3; } }
		public PivotValueComparer(PivotDataController controller) {
			this.controller = controller;
		}
		protected virtual bool CaseSensitive { get { return controller.CaseSensitive; } }
		protected StringComparison StringComparision {
			get {
				return CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			}
		}
		protected override int CompareCore(object x, object y) {
			int res = UnsafeCompareCore(x, y);
			if(res == CompareError) {
				string val1 = Convert.ToString(x);
				string val2 = Convert.ToString(y);
				return StringCompare(val1, val2);
			}
			return res;
		}
		protected override int UnsafeCompareCore(object x, object y) {
			string val1 = x as string,
				val2 = y as string;
			if(val1 != null && val2 != null)
				return StringCompare(val1, val2);
			else
				return CompareErrorValueCore(x, y);
		}
		protected virtual int CompareErrorValueCore(object x, object y) {
			PivotErrorValue err1 = x as PivotErrorValue,
							err2 = y as PivotErrorValue;
			if(err1 != null) {
				if(err2 != null)
					return 0;
				return 1;
			}
			if(err2 != null)
				return -1;
			try {
				int res = base.CompareCore(x, y);
				if(res > 0)
					return 1;
				if(res < 0)
					return -1;
				return 0;
			} catch {
				return CompareError;
			}
		}
		protected int StringCompare(string x, string y) {
			return CultureInfo.CurrentCulture.CompareInfo.Compare(x, y, CaseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase);
		}
		protected override bool ObjectEqualsCore(object x, object y) {
			string val1 = x as string,
				val2 = y as string;
			if(val1 != null && val2 != null)
				return String.Equals(val1, val2, StringComparision);
			else
				return base.ObjectEqualsCore(x, y);
		}
	}
	public class PivotStorageObjectValueComparer : PivotValueComparer {
		bool caseSensitive;
		public PivotStorageObjectValueComparer(bool caseSensitive)
			: base(null) {
			this.caseSensitive = caseSensitive;
		}
		protected override bool CaseSensitive { get { return caseSensitive; } }
		public override int Compare(object x, object y) {
			return base.CompareCore(x, y);
		}
		protected override int UnsafeCompareCore(object x, object y) {
			return CompareErrorValueCore(x, y);
		}
	}
	public class PivotGroupSummaryComparer : IComparer<GroupRowInfo>,IComparer {
		readonly DataControllerBase controller;
		readonly PivotSummaryItem summaryItem;
		readonly ColumnSortOrder sortOrder;
		readonly PivotSummaryType summaryType;
		readonly PivotGridFieldBase field;
		public PivotGroupSummaryComparer(PivotGridFieldBase field, DataControllerBase controller,
				PivotSummaryItem summaryItem, ColumnSortOrder sortOrder, PivotSummaryType summaryType) {
			this.controller = controller;
			this.summaryItem = summaryItem;
			this.sortOrder = sortOrder;
			this.summaryType = summaryType;
			this.field = field;
		}
		protected DataControllerBase Controller { get { return controller; } }
		protected ValueComparer ValueComparer { get { return Controller.ValueComparer; } }
		protected PivotGridFieldBase Field { get { return field; } }
		public int Compare(object x, object y) {
			GroupRowInfo groupRow1 = (GroupRowInfo)x, groupRow2 = (GroupRowInfo)y;
			int res = 0;
			if(this.summaryItem != null) {
				res = Compare(groupRow1, groupRow2);
			}
			if(res == 0) res = Comparer.Default.Compare(groupRow1.Index, groupRow2.Index);
			if(this.sortOrder == ColumnSortOrder.Ascending) return res;
			res = (res > 0 ? -1 : 1);
			return res;
		}
		public int Compare(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			object val1 = GetValueByGroupRow(groupRow1);
			object val2 = GetValueByGroupRow(groupRow2);
			int res = ValueComparer.Compare(val1, val2);
			if (res == 0) res = Comparer.Default.Compare(groupRow1.Index, groupRow2.Index);
			if (this.sortOrder == ColumnSortOrder.Ascending) return res;
			res = (res > 0 ? -1 : 1);
			return res;
		}
		object GetValueByGroupRow(GroupRowInfo groupRow) {
			PivotSummaryValue sumValue = groupRow.GetSummaryValue(summaryItem) as PivotSummaryValue;
			if(sumValue != null && summaryType == PivotSummaryType.Custom)
				return sumValue.GetCustomValue(Field);
			return sumValue != null ? sumValue.GetValue(summaryType) : null;
		}
	}
	public class PivotGroupValueComparer : IComparer<GroupRowInfo>, IComparer {
		readonly DataControllerGroupHelperBase groupHelper;
		readonly ColumnSortOrder sortOrder;
		readonly NullableDictionary<object, int> valueRanks;
		public PivotGroupValueComparer(DataControllerGroupHelperBase groupHelper, ColumnSortOrder sortOrder, IList<object> groupValues) {
			this.groupHelper = groupHelper;
			this.sortOrder = sortOrder;
			this.valueRanks = new NullableDictionary<object, int>(groupValues.Count);
			for(int i = 0; i < groupValues.Count; i++) {
				this.valueRanks.Add(groupValues[i], i);
			}
		}
		protected DataControllerGroupHelperBase GroupHelper { get { return groupHelper; } }
		public int Compare(object x, object y) {
			GroupRowInfo groupRow1 = (GroupRowInfo)x, groupRow2 = (GroupRowInfo)y;
			return Compare(groupRow1, groupRow2);
		}
		public int Compare(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			object val1 = groupHelper.GetValue(groupRow1),
				val2 = groupHelper.GetValue(groupRow2);
			int rank1, rank2;
			bool isOthers1, isOthers2;
			isOthers1 = !valueRanks.TryGetValue(val1, out rank1);
			isOthers2 = !valueRanks.TryGetValue(val2, out rank2);
			if(isOthers1 && isOthers2)
				return 0;
			if(isOthers1)
				return 1;
			if(isOthers2)
				return -1;
			int res = Comparer<int>.Default.Compare(rank1, rank2);
			return sortOrder == ColumnSortOrder.Descending ? -res : res;
		}
	}
	public class PivotConditionalGroupSummaryComparer : IComparer<GroupRowInfo>, IComparer {
		readonly PivotDataController controller;
		readonly GroupRowInfo sortbyGroup;
		readonly PivotSummaryItem summaryItem;
		readonly PivotSummaryType summaryType;
		readonly ValueComparer valueComparer;
		readonly bool isColumn, isAscending;
		readonly PivotGridFieldBase field;
		readonly bool firstPass;
		public PivotConditionalGroupSummaryComparer(PivotDataController controller, PivotGridFieldBase field,
				GroupRowInfo sortbyGroup, bool isAscending,
				bool isColumn, PivotSummaryItem summaryItem, PivotSummaryType summaryType,
				bool firstPass) {
			this.controller = controller;
			this.valueComparer = controller.ValueComparer;
			this.field = field;
			this.sortbyGroup = sortbyGroup;
			this.summaryItem = summaryItem;
			this.summaryType = summaryType;
			this.isColumn = isColumn;
			this.isAscending = isAscending;
			this.firstPass = firstPass;
		}
		#region IComparer Members
		public int Compare(object x, object y) {
			GroupRowInfo group1 = (GroupRowInfo)x, group2 = (GroupRowInfo)y;
			return CompareCore(group1, group2);
		}
		#endregion
		public int Compare(GroupRowInfo x, GroupRowInfo y) {
			return CompareCore(x, y);
		}
		int CompareCore(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			object val1 = GetValueByGroupRow(groupRow1);
			object val2 = GetValueByGroupRow(groupRow2);
			int res = valueComparer.Compare(val1, val2);
			return isAscending ? res : -res;
		}
		object GetValueByGroupRow(GroupRowInfo groupRow) {
			GroupRowInfo summaryRow = controller.GetSummaryGroupRow(isColumn ? groupRow : sortbyGroup,
				isColumn ? sortbyGroup : groupRow, firstPass);
			if(summaryRow == null) return null;
			PivotSummaryValue sumValue = (PivotSummaryValue)summaryRow.GetSummaryValue(summaryItem);
			if(sumValue == null) return null;
			if(summaryType == PivotSummaryType.Custom)
				return sumValue.GetCustomValue(field);
			else
				return sumValue.GetValue(summaryType);
		}
	}
}
