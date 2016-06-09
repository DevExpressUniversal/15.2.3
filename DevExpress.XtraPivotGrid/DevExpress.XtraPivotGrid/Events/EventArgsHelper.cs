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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotEventArgsHelper {
		PivotGridViewInfoData data;
		IThreadSafeAccessible threadSafeAccess;
		public PivotEventArgsHelper(PivotGridViewInfoData data, IThreadSafeAccessible threadSafeAccess) {
			this.data = data;
			this.threadSafeAccess = threadSafeAccess;
		}
		PivotGridViewInfoData Data { get { return data; } }
		PivotGridViewInfo ViewInfo { get { return Data.ViewInfo; } }
		IThreadSafeAccessible ThreadSafeAccess { get { return threadSafeAccess; } }
		public PivotAreaChangingEventArgs CreateAreaChangingEventArgs(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			return new PivotAreaChangingEventArgs(field, newArea, newAreaIndex);
		}
		public CustomFieldDataEventArgs CreateCustomFieldDataEventArgs(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return new CustomFieldDataEventArgs(Data, field as PivotGridField, listSourceRowIndex, expValue);
		}
		public PivotGridCustomSummaryEventArgs CreateCustomSummaryEventArgs(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			return new PivotGridCustomSummaryEventArgs(Data, field as PivotGridField, customSummaryInfo);
		}
		public PivotGridCustomFieldSortEventArgs CreateCustomFieldSortEventArgs(PivotGridFieldBase field) {
			return new PivotGridCustomFieldSortEventArgs(Data, field as PivotGridField);
		}
		public CustomServerModeSortEventArgs CreateCustomServerModeSortEventArgs(PivotGridField field) {
			return new CustomServerModeSortEventArgs(field);
		}
		public CustomizationFormShowingEventArgs CreateCustomizationFormShowingEventArgs(Form customizationForm, ref Control parentControl) {
			return new CustomizationFormShowingEventArgs(customizationForm, parentControl);
		}
		public PivotGroupEventArgs CreateGroupEventArgs(PivotGridGroup group) {
			return new PivotGroupEventArgs(group);
		}
		public PivotFieldEventArgs CreateFieldEventArgs(PivotGridField field) {
			return new PivotFieldEventArgs(field);
		}
		public PivotFieldFilterChangingEventArgs CreateFieldFilterChangingEventArgs(PivotGridField field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return new PivotFieldFilterChangingEventArgs(field, filterType, showBlanks, values);
		}
		public PivotFieldPropertyChangedEventArgs CreateFieldPropertyChangedEventArgs(PivotGridField field, PivotFieldPropertyName propertyName) {
			return new PivotFieldPropertyChangedEventArgs(field, propertyName);
		}
		public PivotFieldDisplayTextEventArgs CreateFieldDisplayTextEventArgs(PivotGridField field, object value, string defaultText) {
			return new PivotFieldDisplayTextEventArgs(field, value, defaultText);
		}
		public PivotFieldDisplayTextEventArgs CreateFieldDisplayTextEventArgs(PivotFieldValueItem item, string defaultText) {
			return new PivotFieldDisplayTextEventArgs(item, defaultText);
		}
		public PivotCustomGroupIntervalEventArgs CreateCustomGroupIntervalEventArgs(PivotGridField field, object value) {
			return new PivotCustomGroupIntervalEventArgs(field, value);
		}
		public PivotCustomChartDataSourceDataEventArgs CreateCustomChartDataSourceDataEventArgs(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return new PivotCustomChartDataSourceDataEventArgs(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		public PivotCustomChartDataSourceRowsEventArgs CreateCustomChartDataSourceRowsEventArgs(PivotChartDataSource ds, IList<PivotChartDataSourceRowBase> rows) {
			return new PivotCustomChartDataSourceRowsEventArgs(ds, rows);
		}
		public PivotCustomFilterPopupItemsEventArgs CreateCustomFilterPopupItemsEventArgs(PivotGridFilterItems items) {
			return new PivotCustomFilterPopupItemsEventArgs(items);
		}
		public PivotCustomFieldValueCellsEventArgs CreateCustomFieldValueCellsEventArgs(PivotVisualItemsBase items) {
			return new PivotCustomFieldValueCellsEventArgs(items);
		}
		public PivotCellDisplayTextEventArgs CreateCellDisplayTextEventArgs(PivotGridCellItem cellItem) {
			return new PivotCellDisplayTextEventArgs(cellItem);
		}
		public PivotCellValueEventArgs CreateCellValueEventArgs(PivotGridCellItem cellItem) {
			return new PivotCellValueEventArgs(cellItem);
		}
		public PivotCellEventArgs CreateCellEventArgs(PivotCellViewInfo cellViewInfo) {
			return new PivotCellEventArgs(cellViewInfo, ViewInfo);
		}
		public PivotLeftTopCellChangedEventArgs CreateLeftTopCellChangedEventArgs(Point oldValue, Point newValue) {
			return new PivotLeftTopCellChangedEventArgs(oldValue, newValue);
		}
		public PivotGridMenuEventArgsBase CreateMenuEventArgsBase(DXPopupMenu menu, PivotGridMenuType menuType, PivotGridField field, PivotArea menuArea, Point menuLocation) {
			return new PivotGridMenuEventArgsBase(ViewInfo, menuType, menu, field, menuArea, menuLocation);
		}
		public PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgsBase(DXPopupMenu menu, PivotGridMenuType menuType, PivotGridField field, PivotArea menuArea, Point menuLocation, DXMenuItem menuItem) {
			return new PivotGridMenuItemClickEventArgsBase(ViewInfo, menuType, menu, field, menuArea, menuLocation, menuItem);
		}
		public PivotGridMenuItemClickEventArgs CreateGridMenuItemClickEventArgs(DXPopupMenu menu, PivotGridMenuType menuType, PivotGridField field, PivotArea menuArea, Point menuLocation, DXMenuItem menuItem) {
			return new PivotGridMenuItemClickEventArgs(ViewInfo, menuType, menu, field, menuArea, menuLocation, menuItem);
		}
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
		public PivotGridMenuEventArgs CreateMenuEventArgs(DXPopupMenu menu, PivotGridMenuType menuType, PivotGridField field, PivotArea menuArea, Point menuLocation) {
			return new PivotGridMenuEventArgs(ViewInfo, menuType, menu, field, menuArea, menuLocation);
		}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		public PivotFieldValueEventArgs CreateFieldValueEventArgs(PivotFieldValueItem item) {
			return new PivotFieldValueEventArgs(item);
		}
		public PivotFieldValueEventArgs CreateFieldValueEventArgs(PivotGridField field) {
			return new PivotFieldValueEventArgs(field);
		}
		public PivotFieldValueCancelEventArgs CreateFieldValueCancelEventArgs(PivotFieldValueItem item) {
			return new PivotFieldValueCancelEventArgs(item);
		}
		public PivotFieldTooltipShowingEventArgs CreateFieldTooltipShowingEventArgs(Point point, string text) {
			return new PivotFieldTooltipShowingEventArgs(ViewInfo, point, text);
		}
		public PivotOlapExceptionEventArgs CreateOlapExceptionEventArgs(System.Exception ex) {
			return new PivotOlapExceptionEventArgs(ex);
		}
		public PivotOlapExceptionEventArgs CreateQueryExceptionEventArgs(System.Exception ex) {
			return new PivotOlapExceptionEventArgs(ex);
		}
		public EditValueChangedEventArgs CreateEditValueChangedEventArgs(PivotCellViewInfo cellInfo, BaseEdit edit) {
			return new EditValueChangedEventArgs(cellInfo, edit);
		}
		public CustomEditValueEventArgs CreateCustomEditValueEventArgs(object value, PivotGridCellItem cellItem) {
			return new CustomEditValueEventArgs(value, cellItem);
		}
		public CancelPivotCellEditEventArgs CreateCancelPivotCellEditEventArgs(PivotCellViewInfo cellInfo, RepositoryItem repositoryItem) {
			return new CancelPivotCellEditEventArgs(cellInfo, repositoryItem);
		}
		public LayoutUpgradeEventArgs CreateLayoutUpgradeEventArgs(string restoredVersion) {
			return new LayoutUpgradeEventArgs(restoredVersion);
		}
		public PivotCellEditEventArgs CreateCellEditEventArgs(PivotCellViewInfo cellInfo, BaseEdit edit) {
			return new PivotCellEditEventArgs(cellInfo, edit);
		}
		public PivotCustomCellEditEventArgs CreateCustomCellEditEventArgs(PivotGridCellItem cellItem, RepositoryItem repositoryItem) {
			return new PivotCustomCellEditEventArgs(cellItem, repositoryItem);
		}
		public CustomExportHeaderEventArgs CreateCustomExportHeaderEventArgs(IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return new CustomExportHeaderEventArgs(brick, field, (ExportAppearanceObject)appearance, data.GetField(field), ref rect);
		}
		public CustomExportFieldValueEventArgs CreateCustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldValueItem fieldValueItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return new CustomExportFieldValueEventArgs(brick, fieldValueItem, (ExportAppearanceObject)appearance, ref rect);
		}
		public CustomExportCellEventArgs CreateCustomExportCellEventArgs(IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return new CustomExportCellEventArgs(brick, cellItem, (ExportAppearanceObject)appearance, ViewInfo, ViewInfo.Data, ViewInfo.Data.PivotGrid.printer, graphicsUnit, ref rect);
		}
		public PivotCustomRowHeightEventArgs CreateCustomRowHeightEventArgs(PivotFieldValueItem item, int height) {
			return new PivotCustomRowHeightEventArgs(item, height);
		}
		public PivotCustomColumnWidthEventArgs CreateCustomColumnWidthEventArgs(PivotFieldValueItem item, int width) {
			return new PivotCustomColumnWidthEventArgs(item, width);
		}
		public PivotFieldImageIndexEventArgs CreateFieldImageIndexEventArgs(PivotFieldValueItem item) {
			return new PivotFieldImageIndexEventArgs(ThreadSafeAccess, item);
		}
		public PivotCustomDrawCellEventArgs CreateCustomDrawCellEventArgs(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			return new PivotCustomDrawCellEventArgs(ThreadSafeAccess, cellViewInfo, appearance, paintArgs, ViewInfo, defaultDraw);
		}
		public PivotCustomAppearanceEventArgs CreateCustomAppearanceEventArgs(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			return new PivotCustomAppearanceEventArgs(ThreadSafeAccess, cellItem, appearance, ViewInfo, bounds);
		}
		public PivotCustomDrawFieldHeaderEventArgs CreateCustomDrawFieldHeaderEventArgs(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return new PivotCustomDrawFieldHeaderEventArgs(ThreadSafeAccess, headerViewInfo, paintArgs, painter, defaultDraw);
		}
		public PivotCustomDrawFieldValueEventArgs CreateCustomDrawFieldValueEventArgs(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return new PivotCustomDrawFieldValueEventArgs(ThreadSafeAccess, fieldCellViewInfo, info, paintArgs, painter, defaultDraw);
		}
		public PivotCustomDrawHeaderAreaEventArgs CreateCustomDrawHeaderAreaEventArgs(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return new PivotCustomDrawHeaderAreaEventArgs(ThreadSafeAccess, headersViewInfo, paintArgs, bounds, defaultDraw);
		}
		public PivotCustomDrawEventArgs CreateCustomDrawEventArgs(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return new PivotCustomDrawEventArgs(ThreadSafeAccess, appearanceOwner, paintArgs, bounds, defaultDraw);
		}
	}
}
