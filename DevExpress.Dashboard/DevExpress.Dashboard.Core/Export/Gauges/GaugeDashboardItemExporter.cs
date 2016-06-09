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
using System.Drawing;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public class GaugeDashboardItemExporter : DashboardItemExporter {
		readonly DashboardGaugePrinter printer;
		public GaugeDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data, DashboardReportOptions opts)
			: base(mode, data) {
			bool exportMode = mode == DashboardExportMode.EntireDashboard;
			bool autoArrange = false;
			bool pdfMode = true;
			if(opts != null) {
				pdfMode = opts.FormatOptions.Format == DashboardExportFormat.PDF;
				if(opts.ItemContentOptions != null && opts.ItemContentOptions.ArrangerOptions != null)
					autoArrange = opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent;
			}
			printer = new DashboardGaugePrinter((GaugeDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), ClientState, exportMode, SelectedValues, autoArrange, pdfMode, data.FontInfo, DrillDownState != null);
		}
		public override IPrintable PrintableComponent { get { return printer; } }
		public override int ActualPrintWidth {
			get { return printer.ActualWidth; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				printer.Dispose();
			base.Dispose(disposing);
		}
		protected override void CalculateScrollBarSize() {
		}
	}
	public class DashboardGaugePrinter : DashboardExportPrinter {
		readonly ExportGaugeControl gaugeControl;
		readonly IList selectedValues;
		readonly Size actualPrintingSize;
		public DashboardGaugePrinter(GaugeDashboardItemViewModel viewModel, MultiDimensionalData data, ItemViewerClientState clientState, bool entireMode, IList selectedValues, bool autoArrange, bool pdfMode, DashboardFontInfo fontInfo, bool drilledDown) {
			this.selectedValues = selectedValues;
			gaugeControl = new ExportGaugeControl();
			DashboardGaugeControlViewer viewControl = new DashboardGaugeControlViewer(gaugeControl);
			ExportContentScrollableControl exportContentScrollableControl = new ExportContentScrollableControl(viewControl, viewModel.ContentDescription, clientState, entireMode);
			try {
				viewControl.Update(viewModel, data, drilledDown);
				if(FontHelper.HasValue(fontInfo)) {
					foreach(ExportGauge gauge in gaugeControl.Gauges) {
						BaseGaugeWin gaugeWin = gauge.IGauge as BaseGaugeWin;
						if(gaugeWin != null) {
							foreach(LabelComponent label in gaugeWin.Labels)
								label.AppearanceText.Font = FontHelper.GetFont(label.AppearanceText.Font, fontInfo);
						}
					}
				}
				if(entireMode || (pdfMode && autoArrange))
					exportContentScrollableControl.ContentScrollableControlModel.ContentArrangementOptions |= ContentArrangementOptions.AlignCenter;
				if(entireMode) {
					ScrollingState vScrollState = clientState.VScrollingState;
					ScrollingState hScrollState = clientState.HScrollingState;
					int vOffset = vScrollState != null ? (int)(vScrollState.PositionRatio * exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Height) : 0;
					int hOffset = hScrollState != null ? (int)(hScrollState.PositionRatio * exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Width) : 0;
					viewControl.ClientOffset = new Point(hOffset, vOffset);
				}
				else {
					exportContentScrollableControl.ContentScrollableControlModel.ContentArrangementOptions |= ContentArrangementOptions.AlignCenter;
				}
			}
			catch { }
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
				gaugeControl.Dispose();
			base.Dispose(disposing);
		}
		protected override void CreateDetail(IBrickGraphics graph) {
			try {
				gaugeControl.Print(graph, selectedValues, actualPrintingSize);
			} catch { }
		}
	}
}
