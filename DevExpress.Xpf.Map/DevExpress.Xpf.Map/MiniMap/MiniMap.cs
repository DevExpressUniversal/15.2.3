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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using DevExpress.Map;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public enum MiniMapAlignment {
		TopLeft = 1,
		TopRight = 4,
		BottomLeft = 8,
		BottomRight = 16
	}
	[
	TemplatePart(Name = "PART_ViewportPanel", Type = typeof(MiniMapViewportPanel)),
	TemplatePart(Name = "PART_InvalidKeyPanel", Type = typeof(Grid)),
	ContentProperty("Layers")
	]
	public class MiniMap : MapElement, IOverlayInfo, IMapView, ISupportProjection, IInvalidKeyPanelHolder {
		static readonly MiniMapBehavior DefaultBehavior = new DynamicMiniMapBehavior();
		const double defaultWidth = 200.0;
		const double defaultHeight = 200.0;
		readonly MapOverlayLayout layout = new MapOverlayLayout();
		readonly List<LayerBase> layers = new List<LayerBase>();
		readonly MiniMapNavigationController navigationController;
		MiniMapViewportPanel viewportPanel;
		Grid invalidKeyPanel = null;
		Rect viewport = new Rect(0, 0, 1, 1);
		Size viewportInPixels = Size.Empty;
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(MiniMapLayerCollection), typeof(MiniMap), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AlignmentProperty = DependencyPropertyManager.Register("Alignment",
			typeof(MiniMapAlignment), typeof(MiniMap), new PropertyMetadata(MiniMapAlignment.BottomLeft, OnAlignmentChanged));
		public static readonly DependencyProperty BehaviorProperty = DependencyPropertyManager.Register("Behavior",
			typeof(MiniMapBehavior), typeof(MiniMap), new PropertyMetadata(null, OnBehaviorChanged));
		public static readonly DependencyProperty EnableZoomingProperty = DependencyPropertyManager.Register("EnableZooming",
			typeof(bool), typeof(MiniMap), new PropertyMetadata(true));
		public static readonly DependencyProperty EnableScrollingProperty = DependencyPropertyManager.Register("EnableScrolling",
			typeof(bool), typeof(MiniMap), new PropertyMetadata(true));
		public static readonly DependencyProperty SetMapCenterOnClickProperty = DependencyPropertyManager.Register("SetMapCenterOnClick",
			typeof(bool), typeof(MiniMap), new PropertyMetadata(true));
		public static readonly DependencyProperty ViewportTemplateProperty = DependencyPropertyManager.Register("ViewportTemplate",
			typeof(DataTemplate), typeof(MiniMap));
		[Category(Categories.Data)]
		public MiniMapLayerCollection Layers {
			get { return (MiniMapLayerCollection)GetValue(LayersProperty); }
		}
		[Category(Categories.Appearance)]
		public MiniMapAlignment Alignment {
			get { return (MiniMapAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		[Category(Categories.Behavior)]
		public MiniMapBehavior Behavior {
			get { return (MiniMapBehavior)GetValue(BehaviorProperty); }
			set { SetValue(BehaviorProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool EnableZooming {
			get { return (bool)GetValue(EnableZoomingProperty); }
			set { SetValue(EnableZoomingProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool EnableScrolling {
			get { return (bool)GetValue(EnableScrollingProperty); }
			set { SetValue(EnableScrollingProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool SetMapCenterOnClick {
			get { return (bool)GetValue(SetMapCenterOnClickProperty); }
			set { SetValue(SetMapCenterOnClickProperty, value); }
		}
		[Category(Categories.Presentation)]
		public DataTemplate ViewportTemplate {
			get { return (DataTemplate)GetValue(ViewportTemplateProperty); }
			set { SetValue(ViewportTemplateProperty, value); }
		}
		MapControl Map { get { return Owner as MapControl; } }
		internal double ActualZoomLevel {
			get {
				if (Map != null)
					return Map.ValidateMinMaxZoomLevel(ActualBehavior.CalculateZoomLevel(Map.ZoomLevel));
				return ActualBehavior.CalculateZoomLevel(1);
			}
		}
		internal CoordPoint ActualCenterPoint { get { return ActualBehavior.Center != null ? ActualBehavior.Center : (Map != null ? Map.ActualCenterPoint : new GeoPoint(0, 0)); } }
		MapCoordinateSystem ActualCoordinateSystem { get { return Map != null ? Map.ActualCoordinateSystem : MapControl.DefaultCoordinateSystem; } }
		Size InitialMapSize { get { return Map != null ? Map.InitialMapSize : MapControl.DefaultInitialMapSize; } }
		internal MiniMapBehavior ActualBehavior { get { return Behavior != null ? Behavior : DefaultBehavior; } }
		internal bool CanZoom { get { return Map != null ? Map.EnableZooming && EnableZooming : EnableZooming; } }
		internal bool CanScroll { get { return Map != null ? Map.EnableScrolling && EnableScrolling : EnableScrolling; } }
		internal Rect Viewport { get { return viewport; } }
		public MiniMap() {
			DefaultStyleKey = typeof(MiniMap);
			this.SetValue(LayersPropertyKey, new MiniMapLayerCollection(this));
			navigationController = new MiniMapNavigationController(this);
		}
		static void OnBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MiniMap miniMap = d as MiniMap;
			if (miniMap != null)
				miniMap.UpdateViewport();
		}
		static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MiniMap miniMap = d as MiniMap;
			if (miniMap != null)
				miniMap.UpdateMapOverlays();
		}
		#region IOverlayInfo implementation
		MapOverlayLayout IOverlayInfo.Layout { get { return layout; } }
		HorizontalAlignment IOverlayInfo.HorizontalAlignment { get { return GetHorizontalAlignment(); } }
		VerticalAlignment IOverlayInfo.VerticalAlignment { get { return GetVerticalAlignment(); } }
		Control IOverlayInfo.GetPresentationControl() { return this; }
		HorizontalAlignment GetHorizontalAlignment() {
			if (Alignment == MiniMapAlignment.BottomRight || Alignment == MiniMapAlignment.TopRight)
				return HorizontalAlignment.Right;
			return HorizontalAlignment.Left;
		}
		VerticalAlignment GetVerticalAlignment() {
			if (Alignment == MiniMapAlignment.BottomLeft || Alignment == MiniMapAlignment.BottomRight)
				return VerticalAlignment.Bottom;
			return VerticalAlignment.Top;
		}
		void IOverlayInfo.OnAlignmentUpdated() {
			UpdateMapOverlays();
		}
		#endregion
		#region IMapView implementation
		double IMapView.ZoomLevel { get { return ActualZoomLevel; } }
		CoordPoint IMapView.CenterPoint { get { return ActualCenterPoint; } }
		Size IMapView.InitialMapSize { get { return Map != null ? Map.InitialMapSize : MapControl.DefaultInitialMapSize; } }
		MapCoordinateSystem IMapView.CoordinateSystem { get { return ActualCoordinateSystem; } }
		Rect IMapView.Viewport { get { return Viewport; } }
		Size IMapView.ViewportInPixels { get { return viewportInPixels; } }
		#endregion
		#region ISupportProjection implementation
		ProjectionBase ISupportProjection.Projection { get { return Map != null ? ((ISupportProjection)Map).Projection : MapControl.DefaultMapProjection; } }
		#endregion
		#region IInvalidKeyPanelHolder implementation
		Grid IInvalidKeyPanelHolder.InvalidKeyPanel { get { return invalidKeyPanel; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			viewportPanel = GetTemplateChild("PART_ViewportPanel") as MiniMapViewportPanel;
			invalidKeyPanel = GetTemplateChild("PART_InvalidKeyPanel") as Grid;
			UpdateViewport();
		}
		Rect CalculateMapViewportRect() { 
			if (Map == null)
				return Rect.Empty;
			Rect mapViewport = Map.Viewport;
			Point topLeft = ActualCoordinateSystem.MapUnitToScreenPoint(new MapUnit(mapViewport.Left, mapViewport.Top), Viewport, viewportInPixels);
			Point bottomRight = ActualCoordinateSystem.MapUnitToScreenPoint(new MapUnit(mapViewport.Right, mapViewport.Bottom), Viewport, viewportInPixels);
			return new Rect(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}
		void UpdateMapOverlays() {
			if (Map != null)
				Map.InvalidateOverlays();
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			UpdateViewport();
		}
		protected override Size MeasureOverride(Size constraint) {
			double constraintWidth = double.IsInfinity(constraint.Width) ? defaultWidth : constraint.Width;
			double constraintHeight = double.IsInfinity(constraint.Height) ? defaultHeight : constraint.Height;
			viewportInPixels = new Size(constraintWidth, constraintHeight);
			Size res = base.MeasureOverride(constraint);
			UpdateViewport();
			return res;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			UpdateMapViewport();
			layout.Size = arrangeBounds;
			return base.ArrangeOverride(arrangeBounds);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			OverlayLayoutUpdater.LayotPropertyChanged(e.Property, this);
		}
		internal void UpdateViewport() {
			if (!viewportInPixels.IsEmpty)
				viewport = ActualCoordinateSystem.CalculateViewport(ActualZoomLevel, ActualCenterPoint, viewportInPixels, InitialMapSize);
			foreach (MiniMapLayerBase layer in Layers)
				layer.UpdateViewport();
			UpdateMapViewport();
		}
		internal void Move(Point offset) {
			if (Map != null) {
				MapUnit centerInMapUnit = ActualCoordinateSystem.CoordPointToMapUnit(ActualCenterPoint, true);
				Point center = ActualCoordinateSystem.MapUnitToScreenPoint(centerInMapUnit, Viewport, viewportInPixels);
				center.Offset(offset.X, offset.Y);
				CoordPoint newCenter = ActualCoordinateSystem.ScreenPointToCoordPoint(center, Viewport, viewportInPixels);
				Map.SetCenterPoint(newCenter);
			}
		}
		internal void Zoom(double zoomOffset) {
			if (Map != null)
				Map.SetZoomLevel(Math.Round(Map.ZoomLevel + zoomOffset));
		}
		internal void SetCenterPointInPixels(Point point) {
			if (Map != null) {
				CoordPoint centerPoint = ActualCoordinateSystem.ScreenPointToCoordPoint(point, Viewport, viewportInPixels);
				Map.SetCenterPoint(centerPoint);
			}
		}
		internal void UpdateMapViewport() {
			if (viewportPanel != null)
				viewportPanel.Viewport = CalculateMapViewportRect();
		}
	}
	[NonCategorized]
	public class MiniMapViewportPanel : Panel {
		public static readonly DependencyProperty ViewportProperty = DependencyPropertyManager.Register("Viewport",
			typeof(Rect), typeof(MiniMapViewportPanel), new PropertyMetadata(Rect.Empty, OnViewportChanged));
		public Rect Viewport {
			get { return (Rect)GetValue(ViewportProperty); }
			set { SetValue(ViewportProperty, value); }
		}
		static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MiniMapViewportPanel panel = d as MiniMapViewportPanel;
			if (panel != null)
				panel.InvalidateMeasure();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement children in Children) {
				if (!Viewport.IsEmpty && Viewport.Width > 0 && Viewport.Height > 0) {
					children.Visibility = Visibility.Visible;
					children.Arrange(Viewport);
				}
				else children.Visibility = Visibility.Collapsed;
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
