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

using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Services;
using DevExpress.XtraPrinting;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardExport {
	public abstract class MapDashboardItemExporter : DashboardItemExporter, IDashboardMapControl {
		readonly InnerMap innerMap;
		readonly MapDashboardItemViewControl viewControl;
		readonly DashboardMapPrinter printer;
		DXCollection<LayerBase> IDashboardMapControl.Layers { get { return innerMap.Layers; } }
		bool IDashboardMapControl.NavigationPanelOptionsVisible { get { return innerMap.NavigationPanelOptions.Visible; } set { innerMap.NavigationPanelOptions.Visible = value; } }
		ToolTipController IDashboardMapControl.ToolTipController { get { return null; } }
		ColorCollection IDashboardMapControl.GradientColors { get { return ColorizerPaletteHelper.GetGradientColors(null); } }
		ColorCollection IDashboardMapControl.DeltaColors { get { return ColorizerPaletteHelper.GetCriteriaColors(null); } }
		ColorCollection IDashboardMapControl.ClusterColors { 
			get {
				Skin skin = CommonSkins.GetSkin(null);
				return new ColorCollection() { skin.Colors.GetColor("Question"), skin.Colors.GetColor("Information"), skin.Colors.GetColor("Warning"), skin.Colors.GetColor("Critical") };
			} 
		}
		ColorCollection IDashboardMapControl.PaletteColors { get { return ColorizerPaletteHelper.GetPaletteColors(null); } }
		double IDashboardMapControl.ZoomLevel { get { return innerMap.ZoomLevel; } set { innerMap.ZoomLevel = value; } }
		double IDashboardMapControl.MinZoomLevel { get { return innerMap.MinZoomLevel; } set { innerMap.MinZoomLevel = value; } }
		IServiceContainer IDashboardMapControl.LegendServiceContainer { get { return innerMap; } }
		LegendCollection IDashboardMapControl.Legends { get { return innerMap.Legends; } }
		bool IDashboardMapControl.EnableNavigation { get; set; }
		IMapControlEventsListener IDashboardMapControl.BaseListener { get { return null; } }
		void IDashboardMapControl.SetExternalListener(IMapControlEventsListener listener) {
			innerMap.SetExternalListener(listener);
		}
		public override IPrintable PrintableComponent { get { return printer; } }
		protected MapDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
			ClientArea clientArea = ClientState.ViewerArea;
			try {
				innerMap = new InnerMap();
				innerMap.SetClientRectangle(new Rectangle(0, 0, clientArea.Width, clientArea.Height));
				innerMap.BorderStyle = BorderStyles.NoBorder;
				innerMap.BackColor = Color.White;
				viewControl = InitializeViewControl();
				foreach(MapLegendBase legend in innerMap.Legends) {
					if(legend != null) {
						legend.BackgroundStyle.Fill = Color.FromArgb(200, 255, 255, 255);
						legend.ItemStyle.TextColor = Color.Black;
						if(DevExpress.DashboardCommon.Printing.FontHelper.HasValue(data.FontInfo))
							legend.ItemStyle.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(legend.ItemStyle.Font, data.FontInfo);
					}
					SizeLegend sizeLegend = legend as SizeLegend;
					if(sizeLegend != null)
						sizeLegend.ItemStyle.Fill = Color.FromArgb(200, 200, 200);
				}
				MapItemStyle defaultShapeItemsStyle = ((VectorItemsLayer)innerMap.Layers[0]).ItemStyle;
				defaultShapeItemsStyle.Fill = Color.FromArgb(200, 200, 200);
				defaultShapeItemsStyle.StrokeWidth = 1;
				if(NeedToSetClientViewportState()) {
					ItemViewerClientState state = data.ViewerClientState;
					if(state.SpecificState != null && state.SpecificState.ContainsKey("MapViewportState"))
						viewControl.SetViewportState((MapViewportState)state.SpecificState["MapViewportState"]);
				}
				if(mode == DashboardExportMode.EntireDashboard)
					viewControl.UpdateSelection(data.ServerData.SelectedValues);
			}
			catch { }
			this.printer = new DashboardMapPrinter(innerMap);
		}
		protected virtual bool NeedToSetClientViewportState() {
			return Mode == DashboardExportMode.EntireDashboard;
		}
		protected abstract MapDashboardItemViewControl InitializeViewControl();
		protected override void Dispose(bool disposing) {
			if(innerMap != null)
				innerMap.Dispose();
			base.Dispose(disposing);
		}
		void IDashboardMapControl.ZoomToRegion(GeoPoint leftTop, GeoPoint rightBottom, GeoPoint centerPoint) {
			IZoomToRegionService zoomService = ((IServiceProvider)innerMap).GetService(typeof(IZoomToRegionService)) as IZoomToRegionService;
			zoomService.ZoomToRegion(leftTop, rightBottom, centerPoint);
		}
	}
	public class DashboardMapPrinter : IPrintable {
		readonly IPrintable printer;
		bool Initialized { get { return printer != null; } }
		public DashboardMapPrinter(InnerMap map) {
			if(map != null) {
				IServiceProvider provider = map as IServiceProvider;
				IPrintableService printableSvc = provider.GetService(typeof(IPrintableService)) as IPrintableService;
				this.printer = printableSvc.Printable;
			}
		}
		void IPrintable.AcceptChanges() {
			if(Initialized)
				printer.AcceptChanges();
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return Initialized && printer.CreatesIntersectedBricks; }
		}
		bool IPrintable.HasPropertyEditor() {
			return Initialized && printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl {
			get { return Initialized ? printer.PropertyEditorControl : null; }
		}
		void IPrintable.RejectChanges() {
			if(Initialized)
				printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			if(Initialized)
				printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return Initialized && printer.SupportsHelp();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			try {
				if(Initialized)
					printer.CreateArea(areaName, graph);
			}
			catch { }
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(Initialized)
				printer.Finalize(ps, link);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			if(Initialized)
				printer.Initialize(ps, link);
		}
	}
	public class ChoroplethMapDashboardItemExporter : MapDashboardItemExporter {
		public ChoroplethMapDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override MapDashboardItemViewControl InitializeViewControl() {
			ChoroplethMapDashboardItemViewControl viewControl = new ChoroplethMapDashboardItemViewControl(this);
			viewControl.Update((ChoroplethMapDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), true);
			return viewControl;
		}
	}
	public abstract class GeoPointMapDashboardItemExporterBase : MapDashboardItemExporter {
		protected GeoPointMapDashboardItemExporterBase(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override bool NeedToSetClientViewportState() {
			GeoPointMapDashboardItemViewModelBase viewModel = (GeoPointMapDashboardItemViewModelBase)this.ServerData.ViewModel;
			return base.NeedToSetClientViewportState() || viewModel.EnableClustering;
		}
	}
	public class GeoPointMapDashboardItemExporter : GeoPointMapDashboardItemExporterBase {
		public GeoPointMapDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override MapDashboardItemViewControl InitializeViewControl() {
			GeoPointMapDashboardItemViewControl viewControl = new GeoPointMapDashboardItemViewControl(this);
			viewControl.Update((GeoPointMapDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), true);
			return viewControl;
		}
	}
	public class BubbleMapDashboardItemExporter : GeoPointMapDashboardItemExporterBase {
		public BubbleMapDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override MapDashboardItemViewControl InitializeViewControl() {
			BubbleMapDashboardItemViewControl viewControl = new BubbleMapDashboardItemViewControl(this);
			viewControl.Update((BubbleMapDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), true);
			return viewControl;
		}
	}
	public class PieMapDashboardItemExporter : GeoPointMapDashboardItemExporterBase {
		public PieMapDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override MapDashboardItemViewControl InitializeViewControl() {
			PieMapDashboardItemViewControl viewControl = new PieMapDashboardItemViewControl(this);
			viewControl.Update((PieMapDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), true);
			return viewControl;
		}
	}
}
