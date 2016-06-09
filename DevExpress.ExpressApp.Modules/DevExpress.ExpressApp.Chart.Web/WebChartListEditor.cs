#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.IO;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.Chart.Web {
	public class ASPxChartListEditor : ChartListEditorBase, IExportable {
		private int GetChartWidthInPixels() {
			if(ChartControl.Width.Type == UnitType.Pixel) {
				return (int)ChartControl.Width.Value;
			}
			return ((IModelWebChartSettings)((IModelChartListView)Model).ChartSettings).PreferredWidth;
		}
		private int GetChartHeightInPixels() {
			if(ChartControl.Height.Type == UnitType.Pixel) {
				return (int)ChartControl.Height.Value;
			}
			return ((IModelWebChartSettings)((IModelChartListView)Model).ChartSettings).PreferredHeight;
		}
		private WebChartControlContainer WebChartControlContainer {
			get { return (WebChartControlContainer)base.chartControlContainer; }
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected override object CreateControlsCore() {
			object control = base.CreateControlsCore();
			OnPrintableChanged();
			return control;
		}
		protected override ChartControlContainer CreateChartControlContainerCore() {
			WebChartControl chartControlCore = new WebChartControlWithEmptyArguments();
			chartControlCore.ID = "Chart";
			this.chartControl = chartControlCore;
			WebChartControlContainer webChartControlContainer = new WebChartControlContainer(chartControlCore);
			IModelWebChartSettings settings = (IModelWebChartSettings)((IModelChartListView)Model).ChartSettings;
			webChartControlContainer.PreferredWidth = settings.PreferredWidth;
			webChartControlContainer.PreferredHeight = settings.PreferredHeight;
			return webChartControlContainer;
		}
		public ASPxChartListEditor(IModelListView model) : base(model) { }
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			OnPrintableChanged();
		}
		public void OnExporting() { }
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
		public new WebChartControl ChartControl {
			get { return (WebChartControl)base.ChartControl; }
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>() {
						ExportTarget.Image,
						ExportTarget.Mht,
						ExportTarget.Pdf,
						ExportTarget.Rtf,
						ExportTarget.Xls,
						ExportTarget.Xlsx
					};
				}
			}
		}
		public IPrintable Printable {
			get {
				if(ChartControl != null) {
					ChartControl chart = new ChartControl();
					using(MemoryStream stream = new MemoryStream()) {
						ChartControl.SaveToStream(stream);
						stream.Seek(0, SeekOrigin.Begin);
						chart.LoadFromStream(stream);
					}
					chart.DataSource = ChartControl.DataSource;
					chart.Width = GetChartWidthInPixels();
					chart.Height = GetChartHeightInPixels();
					return chart;
				}
				return null;
			}
		}
		public int PreferredWidth {
			get { return WebChartControlContainer.PreferredWidth; }
			set { WebChartControlContainer.PreferredWidth = value; }
		}
		public int PreferredHeight {
			get { return WebChartControlContainer.PreferredHeight; }
			set { WebChartControlContainer.PreferredHeight = value; }
		}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
	}
}
