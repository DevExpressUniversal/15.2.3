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
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.DashboardWin.Native {
	public class GridDashboardView : GridView {
		static void CalcWidths(int newLeftColumnWidth, GridDashboardColumn leftColumn, GridDashboardColumn rigthColumn) {
			int delta = newLeftColumnWidth - leftColumn.Width;
			bool newLeftWidthNotLessThanLeftMinWidth = newLeftColumnWidth >= GridDashboardColumn.DefaultMinWidth;
			bool newRigthWidthNotLessThanRigthMinWidth = rigthColumn.Width - delta >= GridDashboardColumn.DefaultMinWidth;
			if(newLeftWidthNotLessThanLeftMinWidth && newRigthWidthNotLessThanRigthMinWidth) {
				leftColumn.Width = newLeftColumnWidth;
				rigthColumn.Width -= delta;
			}
			else if(newLeftWidthNotLessThanLeftMinWidth) {
				leftColumn.Width += rigthColumn.Width - GridDashboardColumn.DefaultMinWidth;
				rigthColumn.Width = GridDashboardColumn.DefaultMinWidth;
			}
			else {
				rigthColumn.Width += leftColumn.Width - GridDashboardColumn.DefaultMinWidth;
				leftColumn.Width = GridDashboardColumn.DefaultMinWidth;
			}
		}
		ColumnsWidthOptionsInfo columnsWidthOptionsInfo;
		ColumnsWidthOptionsClientState widhtClientState;
		float charWidth;
		bool columnsResized = false;
		IGridControl grid;
		public DevExpress.DashboardCommon.GridColumnWidthMode ColumnWidthMode { get; set; }
		public bool IsDesignerMode { get; set; }
		public bool AllColumnsFixed { get; set; }
		public GridDashboardColumnCollection DashboardColumns { get { return (GridDashboardColumnCollection)base.Columns; } }
		public float CharWidth { get { return charWidth; } }
		public int LeftPrintedColumnIndex {
			get {
				int widthSum = 0;
				for(int i = 0; i < Columns.Count; i++) {
					GridColumn column = Columns[i];
					if(LeftCoord < widthSum + column.Width / 2)
						return i;
					widthSum += column.Width;
				}
				return 0;
			}
		}
		public ColumnsWidthOptionsInfo ColumnsWidthOptionsInfo {
			get {
				if(columnsWidthOptionsInfo == null) {
					RecalcWidthOptionsInfo();
					if(!IsDesignerMode)
						ApplyClientState();
				}
				return columnsWidthOptionsInfo;
			}
			set { columnsWidthOptionsInfo = value; }
		}
		public bool ColumnsResized { get { return columnsResized; } }
		public GridDashboardViewInfo DashboardViewInfo { get { return ViewInfo as GridDashboardViewInfo; } }
		protected override string ViewName { get { return "GridDashboardView"; } }
		public bool WordWrap { get { return grid.WordWrap; } }
		internal event GridColumnWidthChangedEventHandler GridColumnWidthChanged;
		public GridDashboardView(GridDashboardControl grid)
			: base(grid) {
			SubscribeEvents();
		}
		public GridDashboardView()
			: base() {
			SubscribeEvents();
		}
		protected override void OnLookAndFeelChanged() {
			base.OnLookAndFeelChanged();
			RecalcWidthOptionsInfo();
		}
		public bool IsSelectedRow(int rowIndex) {
			if(OptionsView.AllowCellMerge) 
				return false;
			int[] selectedRowIndexes = GetSelectedRows();
			return selectedRowIndexes.Contains(rowIndex);
		}
		internal void EndColumnSizingCore(GridDashboardColumn column, int newLeftColumnWidth) {
			int columnIndex = Columns.IndexOf(column);
			if(ColumnWidthMode == DashboardCommon.GridColumnWidthMode.AutoFitToContents || Columns.Count == columnIndex + 1)
				return;
			columnsResized = true;
			GridDashboardColumn leftColumn = (GridDashboardColumn)Columns[columnIndex];
			GridDashboardColumn rigthColumn = (GridDashboardColumn)Columns[columnIndex + 1];
			GridControl.BeginInit();
			try {
				CalcWidths(newLeftColumnWidth, leftColumn, rigthColumn);
				UpdateColumnsWidthOptionsInfo(columnIndex, leftColumn.Width, rigthColumn.Width);
			}
			finally {
				GridControl.EndInit();
			}
			if(GridColumnWidthChanged != null)
				GridColumnWidthChanged(this, new GridColumnWidthsChangedEventArgs(ColumnsWidthOptionsInfo.Clone(), ((IGridColumn)leftColumn).ActualIndex, ((IGridColumn)rigthColumn).ActualIndex));
			OnColumnWidthChanged(leftColumn);
			OnColumnWidthChanged(rigthColumn);
			if(!IsDesignerMode)
				UpdateClientState();
		}
		internal void ResetColumnsWidthOptionsInfo() {
			ColumnsWidthOptionsInfo = null;
		}
		internal void ResetClientState() {
			widhtClientState = null;
			columnsResized = false;
		}
		protected override BaseGridController CreateDataController() {
			return new DashboardDataController();
		}
		protected override void OnGridControlChanged(GridControl prevControl) {
			base.OnGridControlChanged(prevControl);
			grid = (IGridControl)this.GridControl;
		}
		protected override object[] GetFilterPopupValues(GridColumn column, bool showAll, OperationCompleted completed) {
			object[] values = base.GetFilterPopupValues(column, showAll, completed);
			return values;
		}
		protected override GridColumnCollection CreateColumnCollection() {
			return new GridDashboardColumnCollection(this);
		}
		protected override void EndColumnSizing() {
			GridDashboardColumn column = Painter.ReSizingObject as GridDashboardColumn;
			GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
			int newWidth = column.Width;
			if(ci != null) {
				newWidth = Painter.CurrentSizerPos - ci.Bounds.Left;
				if(newWidth < 0)
					newWidth = ci.Bounds.Right - Painter.CurrentSizerPos;
			}
			EndColumnSizingCore(column, newWidth);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				UnsubscribeEvents();
		}
		void SubscribeEvents() {
			CustomSummaryCalculate += OnCustomSummaryCalculate;
		}
		void UnsubscribeEvents() {
			CustomSummaryCalculate -= OnCustomSummaryCalculate;
		}
		void OnCustomSummaryCalculate(object sender, CustomSummaryEventArgs e) {
			if(e.IsTotalSummary)
				e.TotalValueReady = true;
		}
		void UpdateColumnsWidthOptionsInfo(int columnIndex, int leftColumnWidth, int rigthColumnWidth) {
			ColumnWidthMode = DashboardCommon.GridColumnWidthMode.Manual;
			ColumnWidthOptionsInfo leftColumnInfo = ColumnsWidthOptionsInfo.ColumnsInfo[columnIndex];
			ColumnWidthOptionsInfo rightColumnInfo = ColumnsWidthOptionsInfo.ColumnsInfo[columnIndex + 1];
			leftColumnInfo.ActualWidth = leftColumnWidth;
			rightColumnInfo.ActualWidth = rigthColumnWidth;
			ColumnsWidthOptionsInfo.ColumnWidthMode = ColumnWidthMode;
			if(AllColumnsFixed) {
				AllColumnsFixed = false;
				for(int i = 0; i < DashboardColumns.Count; i++)
					ColumnsWidthOptionsInfo.ColumnsInfo[i].WidthType = DashboardCommon.GridColumnFixedWidthType.Weight;
			}
			else {
				leftColumnInfo.WidthType = DashboardCommon.GridColumnFixedWidthType.Weight;
				rightColumnInfo.WidthType = DashboardCommon.GridColumnFixedWidthType.Weight;
			}
			ColumnsWidthOptionsInfo.CalcWeight();
		}
		void RecalcWidthOptionsInfo() {
			ColumnsWidthOptionsInfo = CalcWidthOptionsInfo();
		}
		ColumnsWidthOptionsInfo CalcWidthOptionsInfo() {
			charWidth = FontMeasurer.MeasureMaxDigitWidthF(Appearance.Row.Font);
			ColumnsWidthOptionsInfo optionsInfo = new ColumnsWidthOptionsInfo() { ColumnWidthMode = ColumnWidthMode };
			for(int i = 0; i < Columns.Count; i++) {
				IGridColumn col = (IGridColumn)Columns[i];
				bool manualColumnWidthMode = ColumnWidthMode == DashboardCommon.GridColumnWidthMode.Manual;
				double initialWidth = 0;
				if(!manualColumnWidthMode || col.WidthType == DashboardCommon.GridColumnFixedWidthType.FitToContent)
					initialWidth = DashboardViewInfo.GetBestWidth(col);
				else if(manualColumnWidthMode && col.WidthType == DashboardCommon.GridColumnFixedWidthType.FixedWidth)
					initialWidth = Convert.ToInt32(col.FixedWidth * charWidth);
				else
					initialWidth = col.Weight;
				ColumnWidthOptionsInfo columnInfo = new ColumnWidthOptionsInfo() {
					WidthType = col.WidthType,
					InitialWidth = initialWidth,
					MinWidth = GridDashboardColumn.DefaultMinWidth,
					ActualIndex = col.ActualIndex,
					Weight = col.Weight,
					FixedWidth = col.FixedWidth,
					DisplayMode = col.DisplayMode,
					DefaultBestCharacterCount = col.DefaultBestCharacterCount
				};
				optionsInfo.ColumnsInfo.Add(columnInfo);
			}
			return optionsInfo;
		}
		void UpdateClientState() {
			List<ColumnWidthOptionsInfo> columnsInfo = new List<ColumnWidthOptionsInfo>();
			foreach(ColumnWidthOptionsInfo info in columnsWidthOptionsInfo.ColumnsInfo)
				columnsInfo.Add(info.Clone());
			ColumnsWidthOptionsClientState res = new ColumnsWidthOptionsClientState() {
				ColumnWidthMode = columnsWidthOptionsInfo.ColumnWidthMode,
				AllColumnsFixed = AllColumnsFixed,
				ColumnsInfo = columnsInfo,
				ColumnsResized = columnsResized
			};
			if(widhtClientState != null) {
				for(int i = 0; i < widhtClientState.ColumnsInfo.Count; i++) {
					bool found = false;
					for(int j = 0; j < res.ColumnsInfo.Count; j++) {
						if(widhtClientState.ColumnsInfo[i].ActualIndex == res.ColumnsInfo[j].ActualIndex) {
							found = true;
							break;
						}
					}
					if(!found) {
						res.ColumnsInfo.Add(widhtClientState.ColumnsInfo[i].Clone());
					}
				}
			}
			widhtClientState = res;
		}
		void ApplyClientState() {
			if(widhtClientState != null) {
				double initialWidth = 0;
				columnsResized = widhtClientState.ColumnsResized;
				columnsWidthOptionsInfo.ColumnWidthMode = widhtClientState.ColumnWidthMode;
				AllColumnsFixed = widhtClientState.AllColumnsFixed;
				for(int i = 0; i < columnsWidthOptionsInfo.ColumnsInfo.Count; i++) {
					ColumnWidthOptionsInfo info = columnsWidthOptionsInfo.ColumnsInfo[i];
					foreach(ColumnWidthOptionsInfo clientInfo in widhtClientState.ColumnsInfo) {
						if(info.ActualIndex == clientInfo.ActualIndex) {
							info.WidthType = clientInfo.WidthType;
							info.Weight = clientInfo.Weight;
							info.MinWidth = clientInfo.MinWidth;
							if(widhtClientState.ColumnWidthMode != DashboardCommon.GridColumnWidthMode.Manual || clientInfo.WidthType == DashboardCommon.GridColumnFixedWidthType.FitToContent)
								initialWidth = DashboardViewInfo.GetBestWidth((IGridColumn)Columns[i]);
							else
								initialWidth = clientInfo.InitialWidth;
							info.InitialWidth = initialWidth;
						}
						else if(ColumnWidthMode != DashboardCommon.GridColumnWidthMode.Manual && widhtClientState.ColumnWidthMode == DashboardCommon.GridColumnWidthMode.Manual) {
							info.InitialWidth = info.Weight;
						}
					}
				}
			}
		}
	}
	public class GridDashboardInfoRegistrator : GridInfoRegistrator {
		public override string ViewName { get { return "GridDashboardView"; } }
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new GridDashboardViewInfo((GridDashboardView)view);
		}
		public override BaseView CreateView(GridControl grid) {
			return new GridDashboardView((GridDashboardControl)grid);
		}
	}
	public class ColumnsWidthOptionsClientState {
		public DevExpress.DashboardCommon.GridColumnWidthMode ColumnWidthMode { get; set; }
		public bool AllColumnsFixed { get; set; }
		public bool ColumnsResized { get; set; }
		public List<ColumnWidthOptionsInfo> ColumnsInfo { get; set; }
	}
	public class DashboardDataController : CurrencyDataController {
		protected override ValueComparer CreateValueComparer() {
			return new DashboardValueComparer();
		}
	}
}
