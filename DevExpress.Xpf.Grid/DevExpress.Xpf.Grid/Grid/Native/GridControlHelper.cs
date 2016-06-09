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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Data.Native;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using System;
using System.Linq;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Grid.Native {
	public static class GridControlHelper {
		public static void SetVerticalOffsetForce(GridViewBase view, double value) {
			view.DataPresenter.SetVerticalOffsetForce(value);
		}
		public static GridRow GetRow(GridViewBase view, int rowHandle) {
			return (GridRow)view.GetRowElementByRowHandle(rowHandle);
		}
		public static IScrollInfo GetScrollInfo(GridViewBase view) {
			return (IScrollInfo)view.DataPresenter;
		}
		public static DataPresenterBase GetDataPresenter(DataViewBase view) {
			return view.DataPresenter;
		}
		public static void DoBandMoveAction(DataControlBase dataControl, Action action) {
			dataControl.BandsLayoutCore.DoMoveAction(action);
		}
		public static BandsLayoutBase GetBandsLayout(DataControlBase control) {
			return control.BandsLayoutCore;
		}
		public static IBandsOwner GetOwner(BandBase band) {
			return band.Owner;
		}
		public static object GetDesignTimeSource(DataControlBase dataControl) {
			object itemsSource = dataControl.ItemsSource;
			if(IsDesignTimeDataSource(itemsSource))
				dataControl.DesignTimeShowSampleData = false;
			if(dataControl != null && dataControl.DesignTimeShowSampleData)
				return new GridDesignTimeDataSource(dataControl.DesignTimeDataObjectType, dataControl.DesignTimeDataSourceRowCount, dataControl.DesignTimeUseDistinctSampleValues, itemsSource, GridControlHelper.GetDesignTimePropertyInfo(dataControl));
			return itemsSource;
		}
		static bool IsDesignTimeDataSource(object value) {
			if(value == null) return false;
#if !SL
			Type valueType = value.GetType();
			if(valueType.FullName == "MS.Internal.Data.CollectionViewProxy")
				return valueType.GetProperty("ProxiedView").GetValue(value, null) is IDesignTimeDataSource;
#endif
			return value is IDesignTimeDataSource;
		}
		public static void FillBandsColumns(DataControlBase control) {
			if(control.BandsLayoutCore == null)
				return;
			control.BandsLayoutCore.FillColumns();
		}
		public static void OnColumnHeaderClick(GridViewBase view, GridColumn column) {
			view.OnColumnHeaderClick(column);
		}
		public static void InvalidateDesignTimeDataSource(DataControlBase dataControl) {
			dataControl.InvalidateDesignTimeDataSource();
		}
		public static void ChangeGroupExpanded(GridViewBase view, int visibleIndex) {
			view.Grid.ChangeGroupExpanded(visibleIndex);
		}
		public static GridViewNavigationBase GetNavigation(GridViewBase view) {
			return view.Navigation;
		}
		public static DataViewBase GetView(DataControlBase dataControl) {
			return dataControl.DataView;
		}
		public static IColumnCollection GetColumns(DataControlBase dataControl) {
			return dataControl.ColumnsCore;
		}
		public static IBandColumnsCollection GetColumns(BandBase bandBase) {
			return bandBase.ColumnsCore;
		}
		public static DataProviderBase GetDataProvider(DataControlBase dataControl) {
			return dataControl.DataProviderBase;
		}
		public static NodeContainer GetRootNode(GridViewBase view) {
			return view.RootNodeContainer;
		}
		public static CellEditorBase GetCellPresenterEditor(CellContentPresenter presenter) {
			return presenter.Editor;
		}
		public static DragDropElementHelper GetColumnHeaderDragDropHelper(BaseGridHeader header) {
			return header.DragDropHelper;
		}
		public static ContentPresenter GetDragPreviewElement(HeaderDragElementBase headerDragElement) {
			return headerDragElement.DragPreviewElement;
		}
		public static FrameworkElement GetColumnHeaderContent(GridColumnHeader header) {
			return header.HeaderContent;
		}
		public static ReadOnlyGridSortInfoCollection GetActualSortInfo(GridControl gridControl) {
			return gridControl.ActualSortInfo;
		}
		public static void SetDesignTimeEventsListener(DataControlBase dataControl, IDesignTimeAdornerBase listener) {
			dataControl.DesignTimeAdorner = listener;
		}
		public static void BeginUpdateColumnsLayout(DataViewBase view) {
			view.BeginUpdateColumnsLayout();
		}
		public static void EndUpdateColumnsLayout(DataViewBase view, bool calcLayout) {
			view.EndUpdateColumnsLayout(calcLayout);
		}
		public static IEnumerable<string> GetAllColumnNames(DataControlBase dataControl) {
			return GetDataColumnInfo(dataControl).Select(x => x.Name);
		}
		public static IEnumerable<DataColumnInfo> GetDataColumnInfo(DataControlBase dataControl) {
			dataControl.InvalidateDesignTimeDataSource();
			if(dataControl.DataProviderBase != null)
				return dataControl.DataProviderBase.Columns.Cast<DataColumnInfo>().Where(x => x.Visible);
			return new List<DataColumnInfo>();
		}
		public static void ClearAutoGeneratedBandsAndColumns(DataControlBase dataControl) {
			dataControl.ClearBands(dataControl.BandsCore);
			dataControl.ClearAutoGeneratedColumns();
		}
		public static object GetActualItemsSource(DataControlBase dataControl) {
			return dataControl.ActualItemsSource;
		}
		public static void ApplyColumnAttributes(IModelItem dataControl, IModelItem column) {
			((DataControlBase)dataControl.GetCurrentValue()).PopulateColumnsAndApplyAttributes(dataControl, GetColumnAttibutesPopulatorCallback(column), false, false, true, false, null, GetColumnAttributesSweeperCallback());
		}
		static Func<IModelItem, IModelItemCollection, bool, AllColumnsInfo, ColumnCreatorBase> GetColumnAttibutesPopulatorCallback(IModelItem column) {
			return (dataControl, columns, canCreateNewColumns, columnsInfo) => {
				return new ColumnAttributesPopulator(dataControl, columns, column, columnsInfo);
			};
		}
		static Func<IModelItemCollection, ColumnSweeperBase> GetColumnAttributesSweeperCallback() {
			return (columns) => new ApplyColumnSettingsSweeper();
		}
		public static void PopulateColumns(IModelItem dataControl, Func<IModelItem, IModelItemCollection, bool, AllColumnsInfo, ColumnCreatorBase> createPopulatorCallback) {
			((DataControlBase)dataControl.GetCurrentValue()).PopulateColumnsAndApplyAttributes(dataControl, createPopulatorCallback, false, true, true, false);
		}
		public static IDesignTimeAdornerBase GetDesignTimeAdorner(DataViewBase view) {
			return view.DesignTimeAdorner;
		}
		public static bool GetIsDesignTimeAdornerPanelLeftAligned(DataViewBase view) {
			return view.IsDesignTimeAdornerPanelLeftAligned;
		}
		public static FloatingWindowContainer GetFloatingWindowContainer(ColumnHeaderDragElement headerDragElement) {
			return headerDragElement.WindowContainer;
		}
		public static IEnumerable<DesignTimePropertyInfo> GetDesignTimePropertyInfo(DataControlBase dataControl) {
			foreach(ColumnBase column in dataControl.ColumnsCore) {
				yield return new DesignTimePropertyInfo(column.FieldName, typeof(string), false);
			}
		}
		public static IList<ColumnBase> GetVisibleColumns(DataViewBase view) {
			return view.VisibleColumnsCore;
		}
		public static IList<BandBase> GetVisibleBands(DataControlBase dataControl) {
			return dataControl.BandsLayoutCore.VisibleBands;
		}
		public static IBandsOwner GetBandOwner(BandBase band) {
			return band.Owner;
		}
		public static GridColumnHeader[] GetColumnHeaderElements(DataControlBase dataControl, ColumnBase column) {
			if(!column.Visible)
				return null;
			List<GridColumnHeader> result = new List<GridColumnHeader>();
			bool isGrouped = (column is GridColumn) && ((GridColumn)column).IsGrouped;
			if(isGrouped) {
				GridColumnHeader headerFromGroupPanel = GetHeaderElementFromPanel(dataControl, column, ((GridViewBase)dataControl.DataView).GroupPanel);
				if(headerFromGroupPanel != null)
					result.Add(headerFromGroupPanel);
			}
			bool showGroupedColumns = (dataControl.DataView is GridViewBase) && ((GridViewBase)dataControl.DataView).ShowGroupedColumns;
			if(!isGrouped || showGroupedColumns) {
				GridColumnHeader headerFromHeadersPanel = GetHeaderElementFromPanel(dataControl, column, dataControl.DataView.HeadersPanel);
				if(headerFromHeadersPanel != null)
					result.Add(headerFromHeadersPanel);
			}
			return result.Count > 0 ? result.ToArray() : null;
		}
		public static BandHeaderControl GetBandHeaderElement(DataControlBase dataControl, BandBase band) {
			if(!band.Visible)
				return null;
			foreach(DependencyObject d in new VisualTreeEnumerable(dataControl.BandsLayoutCore.GetBandsContainerControl())) {
				BandHeaderControl header = d as BandHeaderControl;
				if(header != null && (header.DataContext == band) && header.IsVisible())
					return header;
			}
			return null;
		}
		static GridColumnHeader GetHeaderElementFromPanel(DataControlBase dataControl, ColumnBase column, FrameworkElement rootPanel) {
			if(rootPanel == null)
				return null;
			foreach(DependencyObject d in new VisualTreeEnumerable(rootPanel)) {
				GridColumnHeader header = d as GridColumnHeader;
				if((header != null) && (header.DataContext == column) && (header.IsVisible()))
					return header;
			}
			return null;
		}
		public static bool GetIsDirty(RowData rowData) {
			return rowData.IsDirty;
		}
		public static List<RowData> GetRowsToUpdate(GridViewBase view) {
			return view.MasterRootRowsContainer.RowsToUpdate;
		}
		public static DataNodeContainer GetRootNodeContainer(GridViewBase view) {
			return view.RootNodeContainer;
		}
		public static FrameworkElement GetRowElement(RowData rowData) {
			return rowData.RowElement;
		}
		public static RowData GetRowData(GridViewBase view, int rowHandle) {
			return view.GetRowData(rowHandle);
		}
#if !SL
		public static string GetOwnerPredefinedFormatsPropertyName(FormatConditionBase formatCondition) {
			return formatCondition.OwnerPredefinedFormatsPropertyName;
		}
		public static void ClearFormatProperty(FormatConditionBase formatCondition) {
			System.Windows.Data.BindingOperations.ClearBinding(formatCondition, formatCondition.FormatPropertyForBinding);
		}
#endif
	}
}
