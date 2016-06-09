#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ComparisonTypeItem {
		public static ComparisonTypeItem WrapDefault() {
			return new ComparisonTypeItem(DashboardFormatConditionComparisonType.GreaterOrEqual);
		}
		public static ComparisonTypeItem Wrap(DashboardFormatConditionComparisonType comparisonType) {
			return new ComparisonTypeItem(comparisonType);
		}
		ComparisonTypeItem(DashboardFormatConditionComparisonType comparisonType) {
			ComparisonType = comparisonType;
		}
		public DashboardFormatConditionComparisonType ComparisonType { get; set; }
		public override string ToString() {
			return ComparisonType == DashboardFormatConditionComparisonType.Greater ? ">" : ">=";
		}
	}
	class MinInfinityItem {
		public const string NumericMinInfinity = "-" + FormatRuleRangeEditorControl.Infinity;
		const string PercentMinInfinity = "0.00 %";
		public static bool Is(object value) {
			return value is MinInfinityItem || object.Equals(value, MinInfinityItem.NumericMinInfinity);
		}
		readonly FormatRuleRangeEditorControl editorControl;
		public MinInfinityItem(FormatRuleRangeEditorControl editorControl) {
			this.editorControl = editorControl;
		}
		public override string ToString() {
			return editorControl.IsPercent ? PercentMinInfinity : NumericMinInfinity;
		}
	}
	class LevelDimensionItemWrapperList : ItemWrapperList<Dimension> {
		public LevelDimensionItemWrapperList(DimensionCollection items)
			: base(items, true) {
		}
		protected override string ConvertToString(Dimension item) {
			if(item == null) {
				return '[' + DashboardLocalizer.GetString(DashboardStringId.DataEngineGrandTotal) + ']';
			} else {
				return item.DisplayName;
			}
		}
	}
	class FilterDataItemWrapperList : ItemWrapperList<DataItem> {
		public FilterDataItemWrapperList(IEnumerable items)
			: base(items, true) {
		}
		protected override string ConvertToString(DataItem item) {
			if(item == null) {
				return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleDataItemsAll);
			} else {
				return item.DisplayName;
			}
		}
	}
	class DataItemWrapperList : ItemWrapperList<DataItem> {
		public DataItemWrapperList()
			: base() {
		}
		public DataItemWrapperList(IEnumerable items)
			: base(items, false) {
		}
		protected override string ConvertToString(DataItem item) {
			return item.DisplayName;
		}
	}
	class LevelModeWrapperList : ItemWrapperList<FormatConditionIntersectionLevelMode> {
		public LevelModeWrapperList() {
		}
		protected override string ConvertToString(FormatConditionIntersectionLevelMode item) {
			return item.Localize();
		}
	}
	class ValueTypeWrapperList : ItemWrapperList<DashboardFormatConditionValueType> {
		public ValueTypeWrapperList() {
		}
		protected override string ConvertToString(DashboardFormatConditionValueType item) {
			return item.Localize();
		}
	}
	abstract class ItemWrapperList<T> : IEnumerable {
		class ItemWrapper {
			readonly ItemWrapperList<T> owner;
			readonly T item;
			public T Item {
				get { return item; }
			}
			public ItemWrapper(T item, ItemWrapperList<T> owner) {
				this.item = item;
				this.owner = owner;
			}
			public override string ToString() {
				return owner.ConvertToString(item);
			}
			public override bool Equals(object obj) {
				T objItem = ((ItemWrapper)obj).Item;
				return object.Equals(this.item, objItem);
			}
			public override int GetHashCode() {
				return item != null ? item.GetHashCode() : 0;
			}
		}
		readonly List<ItemWrapper> items;
		public T this[int index] {
			get {
				return (index >= 0) ? items[index].Item : default(T);
			}
		}
		protected ItemWrapperList() {
			this.items = new List<ItemWrapper>();
		}
		protected ItemWrapperList(IEnumerable items, bool addEmpty) : this() {
			if(addEmpty) 
				Add(default(T));
			foreach(T item in items) {
				Add(item);
			}
		}
		public void Add(T item) {
			items.Add(new ItemWrapper(item, this));
		}
		public int IndexOf(T item) {
			return items.IndexOf(new ItemWrapper(item, this));
		}
		protected abstract string ConvertToString(T item);
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		#endregion
	}
}
