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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraEditors;
using System.Collections;
using System.Drawing;
namespace DevExpress.DashboardExport {
	public class RangeFilterDashboardItemExporter : ChartDashboardItemExporterBase {
		class DashboardPrintingRangeControl : PrintingRangeControl {
			protected override bool EnableAzureCompatibility { get { return DashboardExportSettings.CompatibilityMode == DashboardExportCompatibilityMode.Restricted; } }
			public DashboardPrintingRangeControl() {
			}
			public DashboardPrintingRangeControl(PrintImageFormat imageFormat)
				: base(imageFormat) {
			}
		}
		readonly DashboardChartControlViewerBase controlViewer;
		readonly DashboardPrintingRangeControl rangeControl;
		public override XtraPrinting.IPrintable PrintableComponent {
			get {
				return Mode == DashboardExportMode.EntireDashboard ? rangeControl.Printable : base.PrintableComponent;
			}
		}
		public RangeFilterDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
			DashboardItemViewModel viewModel;
			RangeFilterDashboardItemViewModel rangeFilterViewModel = (RangeFilterDashboardItemViewModel)ServerData.ViewModel;
			DevExpress.DashboardCommon.Service.ClientArea viewerArea = ClientState.ViewerArea;
			if(mode == DashboardExportMode.EntireDashboard) {
				viewModel = ServerData.ViewModel;
				rangeControl = new DashboardPrintingRangeControl(ExportHelper.SupportMetafileImageFormat ? PrintImageFormat.Metafile : PrintImageFormat.Bitmap);
				rangeControl.Size = new Size(viewerArea.Width, viewerArea.Height);
				controlViewer = new DashboardRangeFilterControlViewer(rangeControl, ChartControl);
			}
			else {
				ChartDashboardItemViewModel chartViewModel = new ChartDashboardItemViewModel {
					Argument = rangeFilterViewModel.Argument,
					SummarySeriesMember = rangeFilterViewModel.SummarySeriesMember,
					AxisX = new ChartAxisViewModel {
						Visible = true
					},
					Caption = rangeFilterViewModel.Caption,
					Legend = new ChartLegendViewModel {
						Visible = false
					},
					Rotated = false,
					ShowCaption = false
				};
				chartViewModel.Panes.Add(new ChartPaneViewModel {
					SeriesTemplates = rangeFilterViewModel.SeriesTemplates,
					PrimaryAxisY = new ChartAxisViewModel {
						Visible = true
					}
				});
				viewModel = chartViewModel;
				controlViewer = new DashboardChartControlViewer(ChartControl);
			}
			controlViewer.InitializeControl();
			controlViewer.Update(viewModel, CreateMultiDimensionalData(), GetDrillDownState());
			if(mode == DashboardExportMode.EntireDashboard) {
				IList minMaxValues = (IList)ServerData.SelectedValues[0];
				((DashboardRangeFilterControlViewer)controlViewer).UpdateMinMaxValues(minMaxValues[0], minMaxValues[1]);
			}
			else
				((IDashboardChartControl)ChartControl).Size = new Size(viewerArea.Width, viewerArea.Height);
		}
	}
}
