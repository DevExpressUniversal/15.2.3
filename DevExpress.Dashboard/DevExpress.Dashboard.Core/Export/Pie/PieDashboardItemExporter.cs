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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public class PieDashboardItemExporter : DashboardItemExporter {
		readonly DashboardPiePrinter printer;
		public override IPrintable PrintableComponent { get { return printer; } }
		protected override string[] TargetAxisNames { 
			get {
				IList<string> axisNames = new List<string>();
				ChartDashboardItemBaseViewModel viewModel = ServerData.ViewModel as ChartDashboardItemBaseViewModel;
				if(viewModel != null) {
					if(viewModel.SelectionMode.HasFlag(ChartSelectionModeViewModel.Argument))
						axisNames.Add(DashboardDataAxisNames.ChartArgumentAxis);
					if(viewModel.SelectionMode.HasFlag(ChartSelectionModeViewModel.Series))
						axisNames.Add(DashboardDataAxisNames.ChartSeriesAxis);
				}
				return axisNames.ToArray();
			}
		}
		public PieDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data, DashboardReportOptions exportOptions)
			: base(mode, data) {
			bool exportMode = mode == DashboardExportMode.EntireDashboard;
			bool autoArrange = false;
			bool pdfMode = true;
			if(exportOptions != null) {
				pdfMode = exportOptions.FormatOptions.Format == DashboardExportFormat.PDF;
				if(exportOptions.ItemContentOptions != null && exportOptions.ItemContentOptions.ArrangerOptions != null)
					autoArrange = exportOptions.ItemContentOptions.ArrangerOptions.AutoArrangeContent;
			}
			printer = new DashboardPiePrinter((PieDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), GetDrillDownState(), ClientState, exportMode, SelectedValues, autoArrange, pdfMode, data.FontInfo);
		}
		protected override void CalculateScrollBarSize() {
		}
		public override int ActualPrintWidth {
			get { return printer.ActualWidth; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				printer.Dispose();
			base.Dispose(disposing);
		}
	}
	public class DashboardPiePrinter : DashboardExportPrinter {
		readonly ExportPieControl pieControl;
		readonly IList selectedValues;
		readonly Size actualPrintingSize;
		public DashboardPiePrinter(PieDashboardItemViewModel viewModel, MultiDimensionalData data, IDictionary<string, IList> drillDownState,  ItemViewerClientState clientState, bool entireMode, IList selectedValues, bool autoArrange, bool pdfMode, DashboardFontInfo fontInfo) {
			pieControl = new ExportPieControl();
			DashboardPieControlViewer controlViewer = new DashboardPieControlViewer(pieControl);
			controlViewer.InitializeControl();
			ExportContentScrollableControl exportContentScrollableControl = new ExportContentScrollableControl(controlViewer, viewModel.ContentDescription, clientState, entireMode);
			controlViewer.Update(viewModel, data, drillDownState);
			if(viewModel.SelectionMode == ChartSelectionModeViewModel.Argument) 
				controlViewer.InteractivityMode = ChartInteractivityMode.Argument;
			else if(viewModel.SelectionMode == ChartSelectionModeViewModel.Series)
				controlViewer.InteractivityMode = ChartInteractivityMode.Series;
			else
				controlViewer.InteractivityMode = ChartInteractivityMode.Point;
			if(controlViewer.InteractivityMode.HasFlag(ChartInteractivityMode.Argument)) {
				if(selectedValues != null && selectedValues.Count>0) {
					ChartDashboardItemExporter.UpdateChartSelectionByTargetAxis(controlViewer, data, selectedValues, viewModel as ChartDashboardItemBaseViewModel, drillDownState);
				}
			}
			if(FontHelper.HasValue(fontInfo)) {
				foreach(Series series in ((IDashboardChartControl)pieControl).Series) {
					PieSeriesView view = series.View as PieSeriesView;
					if(view !=null)
						foreach(SeriesTitle title in view.Titles)
							title.Font = FontHelper.GetFont(title.Font, fontInfo);
					series.Label.Font = FontHelper.GetFont(series.Label.Font, fontInfo);
				}
			}
			if(entireMode || (pdfMode && autoArrange))
				exportContentScrollableControl.ContentScrollableControlModel.ContentArrangementOptions = ContentArrangementOptions.AlignCenter;
			this.selectedValues = selectedValues;
			if(autoArrange) {
				clientState.ViewerArea.Height = exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Height;
				exportContentScrollableControl.UpdateClientState(clientState);
			}
			if(pdfMode)
				actualPrintingSize = new Size(clientState.ViewerArea.Width, clientState.ViewerArea.Height);
			else
				actualPrintingSize = exportContentScrollableControl.ContentScrollableControlModel.VirtualSize;
		}
		public int ActualWidth { get { return actualPrintingSize.Width; } }
		protected override void Dispose(bool disposing) {
			if(disposing)
				pieControl.Dispose();
			base.Dispose(disposing);
		}
		protected override void CreateDetail(IBrickGraphics graph) {
			try {
				pieControl.Print(graph, selectedValues, actualPrintingSize);
			} catch { }
		}
	}
}
