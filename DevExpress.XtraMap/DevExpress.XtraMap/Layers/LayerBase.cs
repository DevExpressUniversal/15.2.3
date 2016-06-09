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

using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraMap {
	public abstract class LayerBase : MapDisposableObject, IOwnedElement, IRenderItemProvider, ISupportObjectChanged, IComparable<LayerBase> {
		const int DefaultMaxZoomLevel = 0;
		const int DefaultMinZoomLevel = 0;
		MapRect identityViewport = MapRect.Empty;
		bool visible = true;
		bool actualVisibility = true;
		int minZoomLevel = DefaultMinZoomLevel;
		int maxZoomLevel = DefaultMaxZoomLevel;
		int zIndex;
		object owner;
		string name = string.Empty;
		object updateLocker = new object();
		MapPoint renderOffset = MapPoint.Empty;
		double renderScale = 1.0;
		double renderScaleFactorX = double.NaN;
		double renderScaleFactorY = double.NaN;
		Rectangle clipBounds = Rectangle.Empty;
		protected internal bool ActualVisibility {
			get { return actualVisibility; }
			set {
				if(value == actualVisibility)
					return;
				actualVisibility = value;
				RaiseVisibilityChanged(actualVisibility);
			}
		}
		protected abstract int DefaultZIndex { get; }
		protected object Owner { get { return owner; } }
		protected internal IMapView View { get { return owner as IMapView; } }
		protected internal MapUnitConverter UnitConverter {
			get {
				ISupportUnitConverter support = owner as ISupportUnitConverter;
				return support != null ? support.UnitConverter : EmptyUnitConverter.Instance;
			}
		}
		internal object UpdateLocker { get { return GetUpdateLocker(); } }
		internal bool IsDesignMode { get { return Map != null && Map.IsDesignMode; } }
		internal InnerMap Map { get { return Owner as InnerMap; } }
		internal CoordPoint AnchorPoint { get { return View != null ? View.AnchorPoint : InnerMap.DefaultCenterPoint; } }
		protected internal MapRect AnimatedViewportRect { get { return View != null ? View.Viewport.AnimatedViewportRect : MapViewportInternal.DefaultViewPort; } }
		protected internal Size ViewportInPixels { get { return View != null ? View.Viewport.ViewportInPixels : Size.Empty; } }
		protected internal Size InitialMapSize { get { return View != null ? View.InitialMapSize : Size.Empty; } }
		protected internal virtual LayerBase LayerInCollection { get { return this; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LayerBaseName"),
#endif
 DefaultValue(""), Category(SRCategoryNames.Map)]
		public string Name {
			get { return name; }
			set {
				if(value == null) value = string.Empty;
				if(string.Compare(name, value, StringComparison.InvariantCulture) == 0)
					return;
				string oldName = name;
				this.name = value;
				RaiseNameChanged(name, oldName);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LayerBaseMinZoomLevel"),
#endif
		DefaultValue(LayerBase.DefaultMinZoomLevel), Category(SRCategoryNames.Behavior)]
		public int MinZoomLevel {
			get { return minZoomLevel; }
			set {
				if(minZoomLevel == value)
					return;
				minZoomLevel = value;
				OnMinMaxZoomLevelChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LayerBaseMaxZoomLevel"),
#endif
		DefaultValue(LayerBase.DefaultMaxZoomLevel), Category(SRCategoryNames.Behavior)]
		public int MaxZoomLevel {
			get { return maxZoomLevel; }
			set {
				if(maxZoomLevel == value)
					return;
				maxZoomLevel = value;
				OnMinMaxZoomLevelChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LayerBaseZIndex"),
#endif
		Category(SRCategoryNames.Data)]
		public int ZIndex {
			get { return zIndex; }
			set {
				if(zIndex == value)
					return;
				zIndex = value;
				OnZIndexChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LayerBaseVisible"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Appearance)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				visible = value;
				OnVisibleChanged();
			}
		}
		void UpdateActualVisibility() {
			ActualVisibility = CheckVisibility();
		}
		protected virtual void OnVisibleChanged() {
			UpdateActualVisibility();
			RaiseChanged();
			InvalidateRender();
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("LayerBaseViewportChanged")]
#endif
		public event ViewportChangedEventHandler ViewportChanged;
#if !SL
	[DevExpressXtraMapLocalizedDescription("LayerBaseDataLoaded")]
#endif
		public event DataLoadedEventHandler DataLoaded;
		public event LayerVisibleChangedEventHandler VisibleChanged;
		protected LayerBase() {
			zIndex = DefaultZIndex;
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				ChangeOwner(value);
				OwnerChanged();
			}
		}
		#endregion
		#region IRenderItemProvider Members
		Rectangle IRenderItemProvider.RenderClipBounds {
			get { return clipBounds; }
		}
		MapColorizer IRenderItemProvider.GetColorizer(IRenderItem item) {
			return GetColorizer(item);
		}
		MapPoint IRenderItemProvider.RenderOffset { get { return renderOffset; } }
		double IRenderItemProvider.RenderScale { get { return renderScale; } }
		double IRenderItemProvider.RenderScaleFactorX {
			get {
				if(double.IsNaN(renderScaleFactorX))
					renderScaleFactorX = CalculateRenderScaleFactorX();
				return renderScaleFactorX / MapShape.RenderScaleFactor * this.renderScale;
			}
		}
		double IRenderItemProvider.RenderScaleFactorY {
			get {
				if(double.IsNaN(renderScaleFactorY))
					renderScaleFactorY = CalculateRenderScaleFactorY();
				return renderScaleFactorY / MapShape.RenderScaleFactor * this.renderScale;
			}
		}
		bool IRenderItemProvider.IsReady { get { return IsReadyForRender; } }
		IEnumerable<IRenderItem> IRenderItemProvider.RenderItems {
			get { return GetRenderItems(); }
		}
		void IRenderItemProvider.OnRenderComplete() {
			AfterRender();
		}
		void IRenderItemProvider.PrepareData() {
			PrepareForRendering();
			EnsureDataLoaded();
		}
		MapPoint[] IRenderItemProvider.GeometryToScreenPoints(MapUnit[] geometry) {
			return GeometryToScreenPoints(geometry);
		}
		void IRenderItemProvider.UpdateRenderParameters(IRenderContext renderContext) {
			UpdateRenderParameters(renderContext.ZoomLevel, renderContext.CenterPoint, renderContext.ContentBounds, renderContext.ClipBounds);
		}
		#endregion
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		protected internal void RaiseChanged() {
			if(onChanged != null) onChanged(this, EventArgs.Empty);
		}
		#endregion
		bool ShouldSerializeZIndex() { return zIndex != DefaultZIndex; }
		void ResetZIndex() { zIndex = DefaultZIndex; }
		void OnZIndexChanged() {
			if(Map != null) {
				Map.ExecuteSafeRenderAction(() => {
					lock(Map.ActualLayers) {
						Map.ActualLayers.Sort(LayerZIndexComparer.Default);
					}
				});
			}
		}
		protected virtual void RaiseVisibilityChanged(bool visibility) {
			if(VisibleChanged != null)
				VisibleChanged(this, new LayerVisibleChangedEventArgs(visibility));
		}
		int IComparable<LayerBase>.CompareTo(LayerBase other) {
			if(Map == null) return 0;
			int index1 = Map.Layers.IndexOf(this);
			int index2 = Map.Layers.IndexOf(other);
			if(index1 == -1)
				return -1;
			if(index2 == -1)
				return 1;
			if(index1 >= 0 && index2 >= 0)
				return index2 - index1;
			return 0;
		}
		internal event NameChangedEventHandler NameChanged;
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if(NameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				NameChanged(this, args);
			}
		}
		double CalculateRenderScaleFactorX() {
			return View.Viewport.ViewportInPixels.Width / identityViewport.Width;
		}
		double CalculateRenderScaleFactorY() {
			return View.Viewport.ViewportInPixels.Height / identityViewport.Height;
		}
		protected abstract IEnumerable<IRenderItem> GetRenderItems();
		protected abstract bool IsReadyForRender { get; }
		protected virtual DataLoadedEventArgs CreateDataLoadedEventArgs() {
			return new DataLoadedEventArgs();
		}
		protected internal virtual void ViewportUpdated() {
			UpdateActualVisibility();
		}
		protected virtual void OnMinMaxZoomLevelChanged() {
			UpdateActualVisibility();
		}
		protected virtual void ChangeOwner(object newOwner) {
			owner = newOwner;
		}
		protected virtual void OwnerChanged() {
			if(Map != null)
				Map.SetClientSize(Map.ContentRectangle.Size);
		}
		protected InnerMap GetInnerMap() {
			IServiceProvider provider = Owner as IServiceProvider;
			IInnerMapService svc = provider != null ? provider.GetService(typeof(IInnerMapService)) as IInnerMapService : null;
			return svc != null ? svc.Map : null;
		}
		protected virtual MapColorizer GetColorizer(IRenderItem item) {
			return null;
		}
		protected virtual bool ShouldLoadData() {
			return View != null && View.ReadyForRender;
		}
		protected virtual void PrepareForRendering() { }
		protected virtual bool CanInvalidateRender() {
			return Map != null;
		}
		protected virtual object GetUpdateLocker() {
			return this.updateLocker;
		}
		protected void RaiseEventAsync(Action action) {
			ISynchronizeInvoke invoker = Map != null ? Map.ExternalInvoker : null;
			MapUtils.BeginInvokeAction(invoker, action);
		}
		protected MapPoint[] GeometryToScreenPoints(MapUnit[] geometry) {
			int count = geometry.Length;
			Size viewportInPixels = ViewportInPixels;
			if(viewportInPixels.Width == 0 || viewportInPixels.Height == 0)
				return new MapPoint[0];
			List<MapPoint> result = new List<MapPoint>(count);
			MapPoint prevPt = new MapPoint();
			for(int i = 0; i < count; i++) {
				MapPoint point = new MapPoint(geometry[i].X, geometry[i].Y);
				if(!prevPt.IsEmpty && prevPt == point)
					continue;
				result.Add(point);
				prevPt = point;
			}
			return result.ToArray();
		}
		protected internal virtual bool CheckVisibility() {
			return Visible && View != null && (MaxZoomLevel == DefaultMaxZoomLevel || View.ZoomLevel <= MaxZoomLevel || View.ZoomLevel == View.MaxZoomLevel) &&
				   (MinZoomLevel == DefaultMinZoomLevel || View.ZoomLevel >= MinZoomLevel);
		}
		internal virtual void AfterRender() { }
		internal void RaiseDataLoadedCore() {
			DataLoaded(this, CreateDataLoadedEventArgs());
		}
		internal void RaiseDataLoaded() {
			if(DataLoaded != null)
				RaiseEventAsync(new Action(RaiseDataLoadedCore));
		}
		internal void EnsureDataLoaded() {
			if(ShouldLoadData())
				LoadData();
		}
		internal MapPoint CalculateCenterOffset(double zoomLevel, CoordPoint centerPoint, Size clientSize) {
			if(centerPoint == new GeoPoint(0, 0))
				return MapPoint.Empty;
			MapUnit unit1 = UnitConverter.CoordPointToMapUnit(AnchorPoint, true);
			MapUnit unit2 = UnitConverter.CoordPointToMapUnit(centerPoint, true);
			MapPoint point1 = UnitConverter.MapUnitToScreenPoint(unit1, zoomLevel, centerPoint, clientSize);
			MapPoint point2 = UnitConverter.MapUnitToScreenPoint(unit2, zoomLevel, centerPoint, clientSize);
			return new MapPoint(point1.X - point2.X, point1.Y - point2.Y);
		}
		internal MapPoint CalculateViewOffset(double zoomLevel, Rectangle contentBounds) {
			MapSize mapSize = GetMapSizeInPixels(zoomLevel);
			double dx = contentBounds.X + (contentBounds.Width - mapSize.Width) / 2.0;
			double dy = contentBounds.Y + (contentBounds.Height - mapSize.Height) / 2.0;
			return new MapPoint(dx, dy);
		}
		internal void InvalidateRender() {
			if(CanInvalidateRender()) Map.Render();
		}
		internal void RaiseViewportChanged() {
			if(ViewportChanged != null && View != null) {
				ViewportChanged(this, new ViewportChangedEventArgs(
					UnitConverter.MapUnitToCoordPoint(new MapUnit(AnimatedViewportRect.Left, AnimatedViewportRect.Top)),
					UnitConverter.MapUnitToCoordPoint(new MapUnit(AnimatedViewportRect.Right, AnimatedViewportRect.Bottom)),
					View.ZoomLevel, View.AnimationInProgress));
			}
		}
		protected internal abstract MapSize GetMapSizeInPixels(double zoomLevel);
		protected internal virtual void ApplyPredefinedColorSchema() { 
		}
		protected internal virtual void OnSetClientSize(Size size) {
			ResetViewport();
		}
		protected internal virtual Size GetMapBaseSizeInPixels() {
			return new Size(InnerMap.DefaultMapSize, InnerMap.DefaultMapSize);
		}
		protected internal virtual void UpdateRenderParameters(double zoomLevel, CoordPoint centerPoint, Rectangle contentBounds, Rectangle clipBounds) {
			if(contentBounds.Size == Size.Empty)
				return;
			this.clipBounds = clipBounds;
			this.renderOffset = CalculateRenderOffset(zoomLevel, centerPoint, contentBounds);
			this.renderScale = RenderController.CalculateRenderScale(zoomLevel);
		}
		protected internal virtual MapPoint CalculateRenderOffset(double zoomLevel, CoordPoint centerPoint, Rectangle contentBounds) {
			MapPoint centerOffset = CalculateCenterOffset(zoomLevel, centerPoint, contentBounds.Size);
			MapPoint viewOffset = CalculateViewOffset(zoomLevel, contentBounds);
			return new MapPoint(centerOffset.X + viewOffset.X, centerOffset.Y + viewOffset.Y);
		}
		protected internal virtual void LoadData() { }
		protected internal void ResetViewport() {
			this.identityViewport = UnitConverter.CalculateViewport(1.0, UnitConverter.PointFactory.CreatePoint(0, 0), ViewportInPixels, InitialMapSize);
			this.renderScaleFactorX = double.NaN;
			this.renderScaleFactorY = double.NaN;
		}
	}
}
