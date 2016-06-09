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
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.XtraPivotGrid.Data {
	public interface IPivotGridEventsImplementorBase {
		void DataSourceChanged();
		void BeginRefresh();
		void EndRefresh();
		void LayoutChanged();
		bool FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex);
		object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue);
		void CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo);
		void GroupFilterChanged(PivotGridGroup group);
		void FieldFilterChanged(PivotGridFieldBase field);
		bool FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values);
		void FieldAreaChanged(PivotGridFieldBase field);
		void FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field);
		void FieldWidthChanged(PivotGridFieldBase field);
		void FieldUnboundExpressionChanged(PivotGridFieldBase field);
		void FieldAreaIndexChanged(PivotGridFieldBase field);
		void FieldVisibleChanged(PivotGridFieldBase field);
		void FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName);
		string FieldValueDisplayText(PivotGridFieldBase field, object value);
		string FieldValueDisplayText(PivotFieldValueItem item, string defaultText);
		string FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember member);
		object CustomGroupInterval(PivotGridFieldBase field, object value);
		int? GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder);
		void PrefilterCriteriaChanged();
		void OLAPQueryTimeout();
		bool QueryException(System.Exception ex);
		string CustomCellDisplayText(PivotGridCellItem cellItem);
		object CustomCellValue(PivotGridCellItem cellItem);
		bool BeforeFieldValueChangeExpanded(PivotFieldValueItem item);
		void AfterFieldValueChangeExpanded(PivotFieldValueItem item);
		void AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field);
		object CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value);
		void CustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows);
		bool CustomFilterPopupItems(PivotGridFilterItems items);
		bool CustomFieldValueCells(PivotVisualItemsBase items);
		int? QuerySorting(IQueryMemberProvider value0, IQueryMemberProvider value1, PivotGridFieldBase field, ICustomSortHelper helper);
	}
}
