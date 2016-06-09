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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public class GridDashboardItemExporter : DashboardItemExporter {
		readonly ExportGridControl exportControl;
		readonly GridDashboardItemViewControl viewControl;
		readonly DashboardGridPrinter printer;
		public override IPrintable PrintableComponent {
			get { return printer; }
		}
		public override IDashboardItemFooterProvider DashboardItemFooterProvider {
			get { return printer; }
		}
		GridDashboardItemViewModel ViewModel {
			get { return (GridDashboardItemViewModel)ServerData.ViewModel; }
		}
		public GridDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData exportData, DashboardReportOptions opts)
			: base(mode, exportData) {
			this.exportControl = new ExportGridControl();
			this.viewControl = new GridDashboardItemViewControl(exportControl);
			viewControl.Update(ViewModel, ServerData.ConditionalFormattingModel, CreateMultiDimensionalData());
			ItemContentOptions options = (opts != null && mode == DashboardExportMode.SingleItem ? opts.ItemContentOptions : null);
			PrintAppearance appearance = new PrintAppearance();
			if(FontHelper.HasValue(exportData.FontInfo)) {
				ExportHelper.ApplyPivotPrintAppearance(appearance, exportData.FontInfo);
			}
			exportControl.DataBind();
			int pathRowIndex = 0;
			int pathColumnIndex = 0;
			if(mode == DashboardExportMode.EntireDashboard) {
				if(ClientState.VScrollingState != null) {
					Dictionary<int, object> scrollPositionRow = GetScrollPositionRow(exportData);
					pathRowIndex = FindRowIndex(exportControl.Data, scrollPositionRow);
				}
				if(ClientState.HScrollingState != null)
					pathColumnIndex = GetPathColumnIndex(exportData);
			}
			this.printer = new DashboardGridPrinter(viewControl, exportControl, exportData.ServerData.SelectedValues, ViewModel, appearance, options, mode, ClientState, pathRowIndex, pathColumnIndex);
			ShowHScroll = printer.ShowHScroll;
			ShowVScroll = printer.ShowVScroll;
			if(mode == DashboardExportMode.EntireDashboard && (pathRowIndex > 0 || pathColumnIndex > 0)) {
				GridDashboardItemExportScroller scroller = new GridDashboardItemExportScroller(exportControl.Data);
				scroller.ScrollTo(pathRowIndex, pathColumnIndex);
				exportControl.ScrollTo(pathColumnIndex);
				exportControl.Data = scroller.ScrolledData;
				exportControl.DataBind();
			}
			printer.CustomDrawCell += OnPrinterCustomDrawCell;
		}
		void OnPrinterCustomDrawCell(object sender, GridCustomDrawCellEventArgsBase e) {
			exportControl.OnRequestCustomDrawCell(sender, e); 
		}
		protected internal override void CalculateShowScrollbars() {
		}
		protected internal override Rectangle GetViewerBounds() {
			ClientArea viewerArea = ClientState.ViewerArea;
			return new Rectangle(viewerArea.Left, viewerArea.Top, viewerArea.Width, viewerArea.Height);
		}
		Dictionary<int, object> GetScrollPositionRow(DashboardItemExportData data) {
			Dictionary<int, object> scrollPositionRow = new Dictionary<int, object>();
			ScrollingState scrollingState = data.ViewerClientState.VScrollingState;
			string[] dataIds = ViewModel.RowIdentificatorDataMembers;
			for(int dataMemberIndex = 0; dataMemberIndex < dataIds.Length; dataMemberIndex++) {
				string dataId = dataIds[dataMemberIndex];
				GridColumnViewModel columnViewModel = ViewModel.Columns.First((column) => { return column.DataId == dataId; });
				int dataIndex = ViewModel.Columns.IndexOf(columnViewModel);
				scrollPositionRow[dataIndex] = scrollingState.PositionListSourceRow[dataMemberIndex];
			}
			return scrollPositionRow;
		}
		int FindRowIndex(PivotGridThinClientData data, Dictionary<int, object> path) {
			if(path == null || path.Count == 0)
				return 0;
			for(int i = 0; i < data.RowFieldValues.Count; i++) {
				bool found = true;
				int dataIndex = 0;
				ThinClientValueItem cell;
				while(data.TryGetCell(null, data.RowFieldValues[i], dataIndex, out cell)) {
					if(path.ContainsKey(dataIndex)) {
						if(!EqualsFieldValues(cell.Value, path[dataIndex])) {
							found = false;
							break;
						}
					}
					dataIndex++;
				}
				if(found)
					return i;
			}
			return 0;
		}
		int GetPathColumnIndex(DashboardItemExportData data) {
			ScrollingState scrollingState = data.ViewerClientState.HScrollingState;
			int index = 0;
			if(scrollingState != null && scrollingState.PositionListSourceRow != null && scrollingState.PositionListSourceRow.Count<object>() != 0)
				index = (int)scrollingState.PositionListSourceRow[0];
			return index;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				printer.CustomDrawCell -= OnPrinterCustomDrawCell;
				printer.Dispose();
				exportControl.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
