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

using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Events {
	public class PivotGridEmptyEventsImplementorBase : IPivotGridEventsImplementorBase {
		protected virtual void CalcCustomSummaryCore(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) { }
		protected virtual string FieldValueDisplayTextCore(PivotFieldValueItem item, string defaultText) {
			return defaultText;
		}
		protected virtual string CustomCellDisplayTextCore(PivotGridCellItem cellItem) {
			return cellItem.Text;
		}
		#region IPivotGridEventsImplementorBase Members
		public void DataSourceChanged() { }
		public void BeginRefresh() { }
		public void EndRefresh() { }
		public void LayoutChanged() { }
		public bool FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			return true;
		}
		public object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return expValue;
		}
		public void CalcCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			CalcCustomSummaryCore(field, customSummaryInfo);
		}
		public void GroupFilterChanged(PivotGridGroup group) { }
		public void FieldFilterChanged(PivotGridFieldBase field) { }
		public bool FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return false;
		}
		public void FieldAreaChanged(PivotGridFieldBase field) { }
		public void FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) { }
		public void FieldWidthChanged(PivotGridFieldBase field) { }
		public void FieldUnboundExpressionChanged(PivotGridFieldBase field) { }
		public void FieldAreaIndexChanged(PivotGridFieldBase field) { }
		public void FieldVisibleChanged(PivotGridFieldBase field) { }
		public void FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) { }
		public string FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember member) {
			return field.GetValueText(member);
		}
		public int? QuerySorting(IQueryMemberProvider value0, IQueryMemberProvider value1, PivotGridFieldBase field, ICustomSortHelper helper) { return null; }
		public string FieldValueDisplayText(PivotGridFieldBase field, object value) {
			return field.GetValueText(value);
		}
		public string FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			return FieldValueDisplayTextCore(item, defaultText);
		}
		public object CustomGroupInterval(PivotGridFieldBase field, object value) {
			return value;
		}
		public int? GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return null;
		}
		public void PrefilterCriteriaChanged() { }
		public void OLAPQueryTimeout() { }
		public virtual bool QueryException(System.Exception ex) {
			return false;
		}
		public string CustomCellDisplayText(PivotGridCellItem cellItem) {
			return CustomCellDisplayTextCore(cellItem);
		}
		public object CustomCellValue(PivotGridCellItem cellItem) {
			return cellItem.Value;
		}
		public bool BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			return true;
		}
		public void AfterFieldValueChangeExpanded(PivotFieldValueItem item) { }
		public void AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) { }
		public object CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return value;
		}
		public void CustomChartDataSourceRows(System.Collections.Generic.IList<PivotChartDataSourceRowBase> rows) { }
		public bool CustomFilterPopupItems(PivotGridFilterItems items) {
			return false;
		}
		public bool CustomFieldValueCells(PivotVisualItemsBase items) {
			return false;
		}
		#endregion
	}
}
