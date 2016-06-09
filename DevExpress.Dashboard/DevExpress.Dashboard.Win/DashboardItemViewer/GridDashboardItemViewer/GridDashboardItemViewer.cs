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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[
	DXToolboxItem(false),
	DashboardItemDesigner(typeof(GridDashboardItemDesigner))
	]
	public class GridDashboardItemViewer : DataDashboardItemViewer {
		public const int EmptyRowHandle = -1;
		const string TargetAxis = DashboardDataAxisNames.DefaultAxis;
		readonly GridDashboardControl gridDashboardControl = new GridDashboardControl();
		UserLookAndFeel lookAndFeel;
		readonly ToolTipController toolTipController = new ToolTipController();
		readonly GridDashboardItemViewControl viewControl;
		MouseEventArgs mouseDownEventArgs = null;
		MouseEventArgs mouseDoubleClickEvetnArgs = null;
		public GridDashboardControl GridControl { get { return gridDashboardControl; } }
		public GridDashboardView GridView { get { return gridDashboardControl.DashboardView; } }
		GridDashboardItemViewModel GridViewModel { get { return (GridDashboardItemViewModel)ViewModel; } }
		GridInteractivityController GridInteractivityController { get { return (GridInteractivityController)InteractivityController; } }
		public GridDashboardItemViewer() {
			this.viewControl = new GridDashboardItemViewControl(gridDashboardControl);
			lookAndFeel = new ControlUserLookAndFeel(this);
			lookAndFeel.StyleChanged += OnStyleChanged;
			GridDashboardView view = new GridDashboardView();
			this.gridDashboardControl.MainView = view;
			this.gridDashboardControl.ViewCollection.Add(view);
			view.GridControl = this.gridDashboardControl;
			toolTipController.GetActiveObjectInfo += OnToolTipControllerGetActiveObjectInfo;
			GridControl.ToolTipController = toolTipController;
		}
		void SubscribeGridEvents() {
			GridDashboardView gridView = GridView;
			GridControl.MouseDown += OnGridControlMouseDown;
			GridControl.MouseClick += OnGridControlMouseClick;
			GridControl.MouseDoubleClick += OnGridControlMouseDoubleClick;
			GridControl.ProcessGridKey += OnProcessGridKey;
			GridControl.KeyDown += OnGridControlKeyDown;
			GridControl.KeyUp += OnGridControlKeyUp;
			GridControl.LostFocus += OnGridControlLostFocus;
			GridControl.MouseMove += OnGridControlMouseMove;
			GridControl.MouseLeave += OnGridControlMouseLeave;
			GridControl.MouseEnter += OnGridControlMouseEnter;
			GridControl.MouseUp += OnGridControlMouseUp;
			GridControl.MouseWheel += OnGridControlMouseWheel;
			GridControl.MouseHover += OnGridControlMouseHover;
			GridControl.Load += OnGridControlLoad;
			gridView.SelectionChanged += OnGridViewSelectionChanged;
			gridView.ShowFilterPopupListBox += OnGridViewShowFilterPopupListBox;
		}
		void UnsubscribeGridEvents() {
			if(GridControl != null) {
				GridControl.MouseDown -= OnGridControlMouseDown;
				GridControl.MouseClick -= OnGridControlMouseClick;
				GridControl.MouseDoubleClick -= OnGridControlMouseDoubleClick;
				GridControl.MouseMove -= OnGridControlMouseMove;
				GridControl.ProcessGridKey -= OnProcessGridKey;
				GridControl.KeyDown -= OnGridControlKeyDown;
				GridControl.KeyUp -= OnGridControlKeyUp;
				GridControl.LostFocus -= OnGridControlLostFocus;
				GridControl.MouseLeave -= OnGridControlMouseLeave;
				GridControl.MouseEnter -= OnGridControlMouseEnter;
				GridControl.MouseUp -= OnGridControlMouseUp;
				GridControl.MouseWheel -= OnGridControlMouseWheel;
				GridControl.MouseHover -= OnGridControlMouseHover;
				GridControl.Load -= OnGridControlLoad;
			}
			if(GridView != null) {
				GridView.SelectionChanged -= OnGridViewSelectionChanged;
				GridView.ShowFilterPopupListBox -= OnGridViewShowFilterPopupListBox;
			}
		}
		void OnGridViewSelectionChanged(object sender, SelectionChangedEventArgs e) {
			GridInteractivityController.ProcessGridControlSelectionChanged(e.Action);
		}
		void OnGridViewShowFilterPopupListBox(object sender, FilterPopupListBoxEventArgs e) {
			Dictionary<object, string> displayTexts = viewControl.GetDisplayTexts(e.Column.FieldName);
			for(int i = 0; i < e.ComboBox.Items.Count; i++) {
				FilterItem item = e.ComboBox.Items[i] as FilterItem;
				if(item != null) {
					string text;
					if(displayTexts.TryGetValue(item.Value, out text))
						item.Text = text;
				}
			}
		}
		private void OnGridControlLoad(object sender, EventArgs e) {
			SetGridControlSelection(GridInteractivityController.ActualSelection);
		}
		void OnGridControlMouseMove(object sender, MouseEventArgs e) {
			RaiseMouseMove(e.Location);
		}
		void OnGridControlKeyDown(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyDown(e);
		}
		void OnGridControlMouseLeave(object sender, EventArgs e) {
			RaiseMouseLeave();
		}
		void OnGridControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnGridControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
			if (mouseDoubleClickEvetnArgs == null) { 
				if (mouseDownEventArgs != null && GetRowHandle(mouseDownEventArgs.Location) != EmptyRowHandle) {
					((DXMouseEventArgs)e).Handled = true;
					OnDashboardItemViewerMouseClick(mouseDownEventArgs);
				}
			}
			mouseDownEventArgs = null;
			mouseDoubleClickEvetnArgs = null;
		}
		void OnGridControlMouseClick(object sender, MouseEventArgs e) {
			RaiseClick(e.Location);
		}
		void OnGridControlMouseDown(object sender, MouseEventArgs e) {
			mouseDownEventArgs = e;
			RaiseMouseDown(e.Location);
			if (e.Button == MouseButtons.Right)
			   ((DXMouseEventArgs)e).Handled = true;
		}
		void OnGridControlMouseDoubleClick(object sender, MouseEventArgs e) {
			mouseDoubleClickEvetnArgs = e;
			RaiseDoubleClick(e.Location);
			OnDashboardItemViewerMouseDoubleClick(e);
		}
		void OnGridControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnGridControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		void OnGridControlKeyUp(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Space:
					e.Handled = true;
					break;
			}
			OnDashboardItemViewerKeyUp(e);
		}
		void OnProcessGridKey(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Back:
				case Keys.Escape:
				case Keys.Left:
				case Keys.Right:
					InteractivityController.ProcessKeyDown(e);
					e.Handled = true;
					break;
			}
		}
		void OnGridControlLostFocus(object sender, EventArgs e) {
			OnDashboardItemViewerLostFocus();
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			DataPointInfo dataPoint = new DataPointInfo();
			int rowHandle = GetRowHandle(location);
			if(rowHandle != EmptyRowHandle) {
				IList values = GetUniqueRowValues(GridView.GetDataSourceRowIndex(rowHandle));
				dataPoint.DimensionValues.Add(TargetAxis, values);
				IList<GridColumnViewModel> columns = GridViewModel.Columns;
				if(columns != null) {
					foreach(GridColumnViewModel column in columns) {
						switch(column.ColumnType) {
							case GridColumnType.Measure:
							case GridColumnType.Sparkline:
								dataPoint.Measures.Add(column.DataId); 
								break;
							case GridColumnType.Delta:
								dataPoint.Deltas.Add(column.DataId);
								break;
						}
					}
				}
				return dataPoint;
			}
			return null;
		}
		IList GetUniqueRowValues(int dataSourceRowIndex) {
			IList<GridColumnViewModel> columns = GridViewModel.Columns;
			IList<string> columnIds = new List<string>();
			if(columns != null) {
				foreach(GridColumnViewModel column in columns) 
					if(column.ColumnType == GridColumnType.Dimension) {
						columnIds.Add(column.DataId);
				}
				return viewControl.GetUniqueValues(dataSourceRowIndex, columnIds).ToList();
			}
			return null;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			ClearInnerValues();
		}
		void ClearInnerValues() {
			gridDashboardControl.ClearInnerValues();
		}
		void OnToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e) {
			GridDashboardView gridView = GridView;
			GridHitInfo info = gridView.CalcHitInfo(e.ControlMousePosition);
			GridDashboardColumn gridColumn = info.Column as GridDashboardColumn;
			if(gridColumn != null) {
				int dataSourceRowIndex = gridView.GetDataSourceRowIndex(info.RowHandle);
				GridCellInfo gridCellInfo = ((GridViewInfo)gridView.GetViewInfo()).GetGridCellInfo(info);
				GridColumnViewModel column = viewControl.GetColumnViewModel(gridColumn.FieldName);
				if(column.DisplayMode == GridColumnDisplayMode.Bar || gridColumn.TextIsHidden) {
					string value = gridView.GetListSourceRowCellValue(dataSourceRowIndex, gridColumn.FieldName + GridMultiDimensionalDataSource.DisplayTextPostfix) as string;
					if(value != null)
						e.Info = new ToolTipControlInfo(gridCellInfo, value, true, ToolTipIconType.None);
				}
				if(column.DisplayMode == GridColumnDisplayMode.Sparkline) {
					string start = gridView.GetListSourceRowCellValue(dataSourceRowIndex, gridColumn.FieldName + GridMultiDimensionalDataSource.SparklineStartDisplayText) as string;
					string end = gridView.GetListSourceRowCellValue(dataSourceRowIndex, gridColumn.FieldName + GridMultiDimensionalDataSource.SparklineEndDisplayText) as string;
					string min = gridView.GetListSourceRowCellValue(dataSourceRowIndex, gridColumn.FieldName + GridMultiDimensionalDataSource.SparklineMinDisplayText) as string;
					string max = gridView.GetListSourceRowCellValue(dataSourceRowIndex, gridColumn.FieldName + GridMultiDimensionalDataSource.SparklineMaxDisplayText) as string;
					e.Info = DashboardWinHelper.GetSparklineTooltip(start, end, min, max, gridCellInfo);
				}
			}
		}
		object GetClientValue(int dataSourceRowIndex, string dataMember) {
			IList clientValues = GetClientValues(dataSourceRowIndex, new string[] { dataMember });
			return clientValues != null && clientValues.Count > 0 ? clientValues[0] : null;
		}
		IList GetClientValues(int dataSourceRowIndex, string[] dataMembers) {
			ITypedList typedList = GridView.DataSource as ITypedList;
			IList values = new List<object>();
			if(typedList != null) {
				IList dataSourceList = (IList)typedList;
				object component = dataSourceList[dataSourceRowIndex];
				foreach(string dataMember in dataMembers) {
					PropertyDescriptor pd = typedList.GetItemProperties(null)[dataMember];
					object value = pd.GetValue(component);
					values.Add(value);
				}
			}
			return values;
		}
		public void SetGridControlSelection(List<AxisPointTuple> selection) {
			GridDashboardView gridView = GridView;
			gridView.BeginSelection();
			gridView.ClearSelection();
			try {
				if(selection.Count > 0) {
					foreach(int index in viewControl.GetDataSourceIndices(selection, TargetAxis)) {
						int rowHandle = gridView.GetRowHandle(index);
						gridView.SelectRow(rowHandle);
					}
				}
			}
			finally {
				gridView.EndSelection();
			}
		}
		protected override Control GetViewControl() {
			return GridControl;
		}
		protected override Control GetUnderlyingControl() {
			return GridControl;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				UnsubscribeGridEvents();
				gridDashboardControl.Dispose();
				DisposeToopTipController();
				if(lookAndFeel != null) {					
					lookAndFeel.StyleChanged -= OnStyleChanged;
					lookAndFeel.Dispose();
					lookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		void DisposeToopTipController() {
			GridControl.ToolTipController = null;
			toolTipController.GetActiveObjectInfo -= OnToolTipControllerGetActiveObjectInfo;
			toolTipController.Dispose();
		}
		protected override void PrepareViewControl() {
			base.PrepareViewControl();
			GridDashboardView gridView = GridView;
			gridView.BestFitMaxRowCount = 1000;
			gridView.BorderStyle = BorderStyles.NoBorder;
			gridView.OptionsDetail.EnableMasterViewMode = false;
			gridView.OptionsMenu.EnableColumnMenu = false;
			gridView.OptionsMenu.EnableColumnMenu = false;
			gridView.OptionsFilter.AllowFilterEditor = false;
			gridView.OptionsView.RowAutoHeight = true;
			gridView.OptionsView.ShowIndicator = false;
			gridView.OptionsView.ColumnAutoWidth = false;
			gridView.ColumnWidthMode = GridColumnWidthMode.Manual;
			gridView.OptionsView.ShowFooter = false;
			gridView.OptionsView.ShowGroupPanel = false;
			gridView.OptionsBehavior.Editable = false;
			gridView.OptionsBehavior.ReadOnly = true;
			gridView.OptionsBehavior.AllowPixelScrolling = DefaultBoolean.True;
			gridView.OptionsSelection.MultiSelect = true;
			gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
			gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
			gridView.OptionsSelection.EnableAppearanceHideSelection = false;
			SubscribeGridEvents();
			bool isDesignMode = DesignerProvider.GetDesigner<GridDashboardItemDesigner>() != null;
			gridView.OptionsMenu.EnableGroupPanelMenu = !isDesignMode;
			gridView.OptionsMenu.EnableFooterMenu = false;
			gridView.IsDesignerMode = isDesignMode;
			foreach (GridColumn column in gridView.Columns)
				column.OptionsColumn.AllowGroup = isDesignMode ? DefaultBoolean.False : DefaultBoolean.True;
		}
		protected override void SetSelection(List<AxisPointTuple> selectionTuples) {
			SetGridControlSelection(selectionTuples);
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();			
			if(ViewModel.ShouldIgnoreUpdate) {
				gridDashboardControl.Invalidate();
				return;
			}
			viewControl.Update(GridViewModel, ConditionalFormattingModel, MultiDimensionalData);
			if(MultiDimensionalData != null)
				ClearInnerValues();
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(gridDashboardControl);
			GridDashboardView gridView = GridView;
			int index = gridView.TopRowIndex;
			string[] rowIdentificatorDataMembers = GridViewModel.RowIdentificatorDataMembers;
			object[] positionPath = new object[rowIdentificatorDataMembers.Length];
			for(int i = 0; i < positionPath.Length; i++)
				positionPath[i] = gridView.GetListSourceRowCellValue(gridView.GetVisibleIndex(gridView.GetVisibleRowHandle(index)), rowIdentificatorDataMembers[i]);
			state.VScrollingState = new ScrollingState { PositionListSourceRow = positionPath };
			state.SpecificState = new Dictionary<string, object> { { "ColumnsWidthOptionsState", GridView.ColumnsWidthOptionsInfo } };
			object[] columnPath = new object[] { gridView.LeftPrintedColumnIndex };
			state.HScrollingState = new ScrollingState { PositionListSourceRow = columnPath };
		}
		protected override InteractivityController CreateInteractivityController() {
			GridInteractivityController controller = new GridInteractivityController(this, this);
			return (InteractivityController)(controller);
		}
		protected override void InitializePopupMenuCreatorsData(PopupMenuCreatorsData data, Point point) {
			base.InitializePopupMenuCreatorsData(data, point);
			Point pointToClient = gridDashboardControl.PointToClient(point);
			GridDashboardView gridView = GridView;
			GridHitInfo hitInfo = gridView.CalcHitInfo(pointToClient.X, pointToClient.Y);
			GridDashboardItemDesigner designer = DesignerProvider.GetDesigner<GridDashboardItemDesigner>();
			bool isDesigner = designer != null;
			if(hitInfo.Column != null) {
				int columnIndex = ((IGridColumn)hitInfo.Column).ActualIndex;
				if(hitInfo.InColumnPanel) {
					data.DashboardItemArea = DashboardItemArea.GridColumnHeader;
					if(isDesigner) {
						data.UseViewerPopup = true;
						foreach(DashboardItemViewerPopupMenuCreator creator in designer.GetColumnWidthPopupMenuCreators(columnIndex))
							data.Creators.Add(creator);
						data.Creators.Add(designer.GetGridColumnTotalPopupMenuCreator(columnIndex));
					}
					else if(GridViewModel.ColumnWidthMode != GridColumnWidthMode.AutoFitToContents && gridView.Columns.Count > 1)
						data.Creators.Insert(0, new GridViewerPopupMenuCreator(gridView.ColumnsResized));
				}
				else if (isDesigner) {
					GridFooterCellInfoArgs footerInfo = hitInfo.FooterCell;
					if (footerInfo != null && footerInfo.SummaryItem != null) {
						data.UseViewerPopup = true;
						data.DashboardItemArea = DashboardItemArea.GridColumnTotal;
						data.Creators.AddRange(designer.GetChangeGridColumnTotalPopupMenuCreators(columnIndex, footerInfo.SummaryItem.Index));
					}
				}
			}
		}
		internal void ResetGridWidthOptions() {
			viewControl.ResetGridWidthOptions(GridViewModel);
		}
		int GetRowHandle(Point location) {
			GridView view = GridControl.GetViewAt(location) as GridView;
			if(view != null) {
				GridHitInfo hitInfo = view.CalcHitInfo(location);
				if(hitInfo.HitTest == GridHitTest.RowCell || hitInfo.HitTest == GridHitTest.RowEdge) {
					return hitInfo.RowHandle;
				}
			}
			return EmptyRowHandle;
		}
		public AxisPointTuple GetRowTuple(int rowHandler) {
			if(rowHandler != EmptyRowHandle) {
				int dataSourceRowIndex = GridView.GetDataSourceRowIndex(rowHandler);
				if(dataSourceRowIndex >= 0) {
					IList uniqueRowValue = GetUniqueRowValues(dataSourceRowIndex);
					if(uniqueRowValue.Count > 0) {
						IList<AxisPoint> axisPoints = new List<AxisPoint>();
						foreach(string axis in TargetAxisCore) {
							AxisPoint axisPoint = GetAxisPoint(axis, uniqueRowValue);
							while(!IsMasterFilterDataMember(axisPoint) && axisPoint.Parent != null)
								axisPoint = axisPoint.Parent;
							if(axisPoint != null)
								axisPoints.Add(axisPoint);
						}
						return MultiDimensionalData.CreateTuple(axisPoints);
					}
				}
			}
			return null;
		}
		public List<AxisPointTuple> GetSelectedTuples() {
			List<AxisPointTuple> selection = new List<AxisPointTuple>();
			foreach(int rowHandle in GridView.GetSelectedRows()) {
				AxisPointTuple tuple = GetRowTuple(rowHandle);
				if(tuple != null && !selection.Contains(tuple))
					selection.Add(tuple);
			}
			return selection;
		}
		public int GetFocusedRowHandle() {
			return GridView.FocusedRowHandle;
		}
		protected override bool IsMasterFilterDataMember(AxisPoint axisPoint) {
			bool isMasterFilterDataMember = true;
			if(axisPoint == null || axisPoint.Dimension == null || !GridViewModel.SelectionDataMembers.Contains(axisPoint.Dimension.ID))
				isMasterFilterDataMember = false;
			return base.IsMasterFilterDataMember(axisPoint) && isMasterFilterDataMember;
		}
	}
}
