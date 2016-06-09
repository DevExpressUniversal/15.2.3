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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Map;
using DevExpress.Services;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Services;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardMapControl : MapControl, IDashboardMapControl {
		 readonly MapOverlay overlay = new MapOverlay();
		public event MouseEventHandler DashboardMouseWheel;
		DXCollection<LayerBase> IDashboardMapControl.Layers { get { return base.Layers; } }
		bool IDashboardMapControl.NavigationPanelOptionsVisible { get { return NavigationPanelOptions.Visible; } set { NavigationPanelOptions.Visible = value; } }
		ToolTipController IDashboardMapControl.ToolTipController {
			get {
				if(ToolTipController == null)
					ToolTipController = new ToolTipController();
				return ToolTipController;
			}
		}
		ColorCollection IDashboardMapControl.GradientColors { get { return ColorizerPaletteHelper.GetGradientColors(LookAndFeel); } }
		ColorCollection IDashboardMapControl.DeltaColors { get { return ColorizerPaletteHelper.GetCriteriaColors(LookAndFeel); } }
		ColorCollection IDashboardMapControl.PaletteColors { get { return ColorizerPaletteHelper.GetPaletteColors(LookAndFeel); } }
		ColorCollection IDashboardMapControl.ClusterColors { 
			get {
				Skin skin = CommonSkins.GetSkin(LookAndFeel);
				return new ColorCollection() { skin.Colors.GetColor("Question"), skin.Colors.GetColor("Information"), skin.Colors.GetColor("Warning"), skin.Colors.GetColor("Critical") }; 
			} 
		}
		IServiceContainer IDashboardMapControl.LegendServiceContainer { get { return this; } }
		IMapControlEventsListener IDashboardMapControl.BaseListener { get { return this; } }
		void IDashboardMapControl.SetExternalListener(IMapControlEventsListener listener) {
			IInnerMapService svc = ((IServiceProvider)this).GetService(typeof(IInnerMapService)) as IInnerMapService;
			if(svc != null)
				svc.Map.SetExternalListener(listener);
		}
		protected override bool IsDesigntimeProcess { get { return false; } }
		IZoomToRegionService ZoomService { get { return ((IServiceProvider)this).GetService(typeof(IZoomToRegionService)) as IZoomToRegionService; } }
		bool IDashboardMapControl.EnableNavigation {
			get { return EnableZooming && EnableScrolling; }
			set {
				EnableZooming = value;
				EnableScrolling = value;
			}
		}
		public bool DrawIgnoreUpdatesState { get { return overlay.Visible; } set { overlay.Visible = value; } }
		public DashboardMapControl() {
			IServiceContainer container = (IServiceContainer)this;
			IMouseHandlerService oldMouseHandler = (IMouseHandlerService)container.GetService(typeof(IMouseHandlerService));
			if(oldMouseHandler != null) {
				container.RemoveService(typeof(IMouseHandlerService));
				DashboardMapZoomServiceWrapper newMouseHandler = new DashboardMapZoomServiceWrapper(oldMouseHandler, this);
				newMouseHandler.MouseWheel += (s, e) => { DashboardMouseWheel(s, e); };
				container.AddService(typeof(IMouseHandlerService), newMouseHandler);
			}
			LookAndFeel.StyleChanged += OnLookAndFeelChanged;
			overlay.Location = new Point(0, 0);			
			SetupOverlaySize();
			SetupOverlayColor();
			Overlays.Add(overlay);
		}		
		public MapViewportState GetViewportState() {
			CoordPoint leftTopPt = ZoomService.RegionLeftTop;
			CoordPoint rightBottomPt = ZoomService.RegionRightBottom;
			CoordPoint center = ZoomService.CenterPoint;
			GeoPoint leftTop = new GeoPoint(leftTopPt.GetY(), leftTopPt.GetX());
			GeoPoint rightBottom = new GeoPoint(rightBottomPt.GetY(), rightBottomPt.GetX());
			GeoPoint centerPoint = new GeoPoint(center.GetY(), center.GetX());
			return new MapViewportState {
				TopLatitude = leftTop.Latitude,
				BottomLatitude = rightBottom.Latitude,
				LeftLongitude = leftTop.Longitude,
				RightLongitude = rightBottom.Longitude,
				CenterPointLatitude = centerPoint.Latitude,
				CenterPointLongitude = centerPoint.Longitude
			};
		}
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			SetupOverlayColor();
		}
		void SetupOverlayColor() {
			overlay.BackgroundStyle.Fill = DashboardWinHelper.GetIgnoreUpdatesColor(LookAndFeel);
		}
		void SetupOverlaySize() {
			overlay.Size = new Size(Width, Height);
		}
		void IDashboardMapControl.ZoomToRegion(GeoPoint leftTop, GeoPoint rightBottom, GeoPoint centerPoint) {
			ZoomService.ZoomToRegion(leftTop, rightBottom, centerPoint);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			SetupOverlaySize();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				if(LookAndFeel!=null)
					LookAndFeel.StyleChanged -= OnLookAndFeelChanged;
		}
	}
	public class DashboardMapZoomServiceWrapper : MouseHandlerServiceWrapper {
		readonly MapControl map;
		public event MouseEventHandler MouseWheel;
		public DashboardMapZoomServiceWrapper(IMouseHandlerService service, MapControl map)
			: base(service) {
			this.map = map;		
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			MouseWheel(map, e);
		}
	}
}
