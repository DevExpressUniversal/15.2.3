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
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Map {
	public class DataLoadedEventArgs : EventArgs {
	}
	public class MapItemsLoadedEventArgs : DataLoadedEventArgs {
		readonly IEnumerable<MapItem> items;
		public IEnumerable<MapItem> Items { get { return items; } }
		public MapItemsLoadedEventArgs(IEnumerable<MapItem> items) {
			this.items = items;
		}
	}
	public delegate void DataLoadedEventHandler(object sender, DataLoadedEventArgs e);
	[TemplatePart(Name = "PART_RootVisual", Type = typeof(Canvas))]
	public abstract class LayerBase : MapElement, IViewportAnimatableElement {
		internal static Size DefaultMapSize = new Size(512, 512);
		const double ZoomLevelStrategyThreshold = 12.0;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty BoundsProperty = DependencyPropertyManager.Register("Bounds",
		   typeof(CoordPointCollection), typeof(LayerBase), new PropertyMetadata(null, BoundsPropertyChanged));
		public static readonly DependencyProperty MinZoomLevelProperty = DependencyPropertyManager.Register("MinZoomLevel",
			typeof(double), typeof(LayerBase), new PropertyMetadata(0.0, ZoomRangePropertyChanged));
		public static readonly DependencyProperty MaxZoomLevelProperty = DependencyPropertyManager.Register("MaxZoomLevel",
			typeof(double), typeof(LayerBase), new PropertyMetadata(0.0, ZoomRangePropertyChanged));
		[Category(Categories.Behavior)]
		public CoordPointCollection Bounds {
			get { return (CoordPointCollection)GetValue(BoundsProperty); }
			set { SetValue(BoundsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MinZoomLevel {
			get { return (double)GetValue(MinZoomLevelProperty); }
			set { SetValue(MinZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MaxZoomLevel {
			get { return (double)GetValue(MaxZoomLevelProperty); }
			set { SetValue(MaxZoomLevelProperty, value); }
		}
		public bool UseSprings {
			get { return Map != null ? Map.UseSprings : MapControl.DefaultUseSprings; }
		}
		static void ZoomRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LayerBase layer = d as LayerBase;
			if (layer != null)
				layer.UpdateContentVisible();
		}
		static void BoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LayerBase layer = d as LayerBase;
			if (layer != null) {
				CoordPointCollection oldCollection = e.OldValue as CoordPointCollection;
				if (oldCollection != null)
					oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(layer.BoundsChanged);
				CoordPointCollection newCollection = e.NewValue as CoordPointCollection;
				if (newCollection != null)
					newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(layer.BoundsChanged);
				layer.UpdateContentClip();
			}
		}
		#region Hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FlowDirection FlowDirection { get { return base.FlowDirection; } set { base.FlowDirection = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Height { get { return base.Height; } set { base.Height = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalAlignment { get { return base.HorizontalAlignment; } set { base.HorizontalAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsEnabled { get { return base.IsEnabled; } set { base.IsEnabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsHitTestVisible { get { return base.IsHitTestVisible; } set { base.IsHitTestVisible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsTabStop { get { return base.IsTabStop; } set { base.IsTabStop = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Margin { get { return base.Margin; } set { base.Margin = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MaxHeight { get { return base.MaxHeight; } set { base.MaxHeight = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MaxWidth { get { return base.MaxWidth; } set { base.MaxWidth = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size RenderSize { get { return base.RenderSize; } set { base.RenderSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool UseLayoutRounding { get { return base.UseLayoutRounding; } set { base.UseLayoutRounding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalAlignment { get { return base.VerticalAlignment; } set { base.VerticalAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Width { get { return base.Width; } set { base.Width = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BindingGroup BindingGroup { get { return base.BindingGroup; } set { base.BindingGroup = value; } }
#pragma warning disable 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete]
		public new BitmapEffect BitmapEffect { get { return base.BitmapEffect; } set { base.BitmapEffect = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete]
		public new BitmapEffectInput BitmapEffectInput { get { return base.BitmapEffectInput; } set { base.BitmapEffectInput = value; } }
#pragma warning restore 0618
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ClipToBounds { get { return base.ClipToBounds; } set { base.ClipToBounds = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ForceCursor { get { return base.ForceCursor; } set { base.ForceCursor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new InputScope InputScope { get { return base.InputScope; } set { base.InputScope = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsManipulationEnabled { get { return base.IsManipulationEnabled; } set { base.IsManipulationEnabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OverridesDefaultStyle { get { return base.OverridesDefaultStyle; } set { base.OverridesDefaultStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool SnapsToDevicePixels { get { return base.SnapsToDevicePixels; } set { base.SnapsToDevicePixels = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Uid { get { return base.Uid; } set { base.Uid = value; } }
		#endregion
		public event ViewportChangedEventHandler ViewportChanged;
		public event DataLoadedEventHandler DataLoaded;
		protected readonly ViewportAnimationProgress progress;
		Rect viewport = new Rect(0, 0, 1, 1);
		Point anchorViewportOrigin = new Point(0, 0);
		Size anchorViewportSize = new Size(1.0, 1.0);
		bool viewportOriginAnimationInProgress = false;
		bool viewportSizeAnimationInProgress = false;
		bool viewportSizeStoryboardStarted = false;
		bool viewportOriginStoryboardStarted = false;
		bool viewportOriginEverUpdated = false;
		Storyboard viewportOriginStoryboard = null;
		Storyboard viewportSizeStoryboard = null;
		Panel rootVisualElement = null;
		protected double ZoomLevel { get { return View != null ? View.ZoomLevel : 1; } }
		protected CoordPoint CenterPoint { get { return View != null ? View.CenterPoint : new GeoPoint(); } }
		protected Size MapSize { get { return View != null ? View.InitialMapSize : MapControl.DefaultInitialMapSize; } }
		protected Size ViewportInPixels { get { return View != null ? View.ViewportInPixels : MapControl.DefaultInitialMapSize; } }
		protected internal MapCoordinateSystem ActualCoordinateSystem { get { return View != null ? View.CoordinateSystem : MapControl.DefaultCoordinateSystem; } }
		protected internal Visibility ActualVisibility {
			get {
				if (Visibility == Visibility.Visible)
					return rootVisualElement != null ? rootVisualElement.Visibility : Visibility.Collapsed;
				return Visibility;
			}
		}
		protected internal bool CanApplyZoomStrategy { get { return Map != null ? Map.ZoomLevel > ZoomLevelStrategyThreshold : false; } }
		protected internal ViewportAnimationProgress ViewportAnimationProgress { get { return progress; } }
		protected internal Rect Viewport {
			get { return viewport; }
			set {
				if (viewport != value) {
					bool zoomLevelChanged = viewport.Height != value.Height || viewport.Width != value.Width;
					if (UseSprings) {
						if (zoomLevelChanged) {
							anchorViewportSize = CalcActualViewportSize(new Size(viewport.Width, viewport.Height));
							AnimateViewportSize();
						}
						if (value.X != viewport.X || value.Y != viewport.Y) {
							SetViewportOrigin(CalcActualViewportOrigin(new Point(viewport.X, viewport.Y)), true);
							AnimateViewportOrigin(zoomLevelChanged);
						}
						viewport = value;
					}
					else {
						viewport = value;
						ViewportUpdated(zoomLevelChanged);
					}
					if (ViewportChanged != null) {
						GeoPoint topLeft = ActualProjection.MapUnitToGeoPoint(new MapUnit(Viewport.X, Viewport.Y));
						GeoPoint bottomRight = ActualProjection.MapUnitToGeoPoint(new MapUnit(Viewport.Right, Viewport.Bottom));
						ViewportChanged(this, new ViewportChangedEventArgs(topLeft, bottomRight, ZoomLevel));
					}
				}
			}
		}
		protected Rect ActualViewport {
			get {
				return new Rect(CalcActualViewportOrigin(new Point(Viewport.X, Viewport.Y)),
					CalcActualViewportSize(new Size(Viewport.Width, Viewport.Height)));
			}
		}
		Storyboard ViewportOriginStoryboard {
			get {
				if (viewportOriginStoryboard == null) {
					viewportOriginStoryboardStarted = false;
					viewportOriginStoryboard = new Storyboard();
					viewportOriginStoryboard.Completed += new EventHandler(OnViewportOriginStoryboardCompleted);
					AnimationHelper.AddStoryboard(this, viewportOriginStoryboard, viewportOriginStoryboard.GetHashCode());
				}
				return viewportOriginStoryboard;
			}
		}
		Storyboard ViewportSizeStoryboard {
			get {
				if (viewportSizeStoryboard == null) {
					viewportSizeStoryboardStarted = false;
					viewportSizeStoryboard = new Storyboard();
					viewportSizeStoryboard.Completed += new EventHandler(OnViewportSizeStoryboardCompleted);
					AnimationHelper.AddStoryboard(this, viewportSizeStoryboard, viewportSizeStoryboard.GetHashCode());
				}
				return viewportSizeStoryboard;
			}
		}
		protected internal IMapView View { get { return Owner as IMapView; } }
		protected internal abstract ProjectionBase ActualProjection { get; }
		protected internal virtual bool IsDataReady { get { return true; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MapControl Map { get { return Owner as MapControl; } }
		public LayerBase() {
			this.SetValue(BoundsProperty, new CoordPointCollection());
			progress = new ViewportAnimationProgress(this);
			IsVisibleChanged += LayerBaseIsVisibleChanged;
		}
		#region IViewportAnimatableElement implementation
		bool IViewportAnimatableElement.OriginInProgress { get { return viewportOriginAnimationInProgress; } }
		bool IViewportAnimatableElement.SizeInProgress { get { return viewportSizeAnimationInProgress; } }
		void IViewportAnimatableElement.ProgressChanged() {
			ViewportUpdated(viewportSizeAnimationInProgress);
		}
		void IViewportAnimatableElement.BeforeSizeProgressCompleting() {
			BeforeSizeProgressCompleting();
		}
		#endregion
		protected virtual void BeforeSizeProgressCompleting() {
		}
		void SetViewportOrigin(Point origin, bool needCheckEquality) {
			if(needCheckEquality && anchorViewportOrigin == origin)
				return;
			this.anchorViewportOrigin = origin;
			this.viewportOriginEverUpdated = true;
		}
		void BoundsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateContentClip();
		}
		void UpdateContentClip() {
			if (rootVisualElement != null) {
				if (Bounds != null && Bounds.Count > 2) {
					PointCollection points = new PointCollection();
					foreach (CoordPoint coordPoint in Bounds)
						points.Add(CoordPointToScreenPointZeroOffset(coordPoint, true, true));
					PathFigure figure = new PathFigure() { StartPoint = points[0] };
					figure.Segments.Add(new PolyLineSegment() { Points = points });
					PathFigureCollection figures = new PathFigureCollection();
					figures.Add(figure);
					rootVisualElement.Clip = new PathGeometry() { Figures = figures };
				}
				else
					rootVisualElement.Clip = null;
			}
		}
		void UpdateContentVisible() {
			if (rootVisualElement != null) {
				if ((MinZoomLevel < 1 || ZoomLevel >= MinZoomLevel) &&
					(MaxZoomLevel < 1 || ZoomLevel <= MaxZoomLevel))
					rootVisualElement.Visibility = Visibility.Visible;
				else
					rootVisualElement.Visibility = Visibility.Collapsed;
			}
		}
		void LayerBaseIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnIsVisibleChanged();
		}
		void OnViewportOriginStoryboardCompleted(object sender, EventArgs e) {
			SetViewportOrigin(new Point(Viewport.X, Viewport.Y), false);
			StopViewportOriginAnimation();
		}
		protected virtual void OnViewportSizeStoryboardCompleted(object sender, EventArgs e) {
			anchorViewportSize = new Size(Viewport.Width, Viewport.Height);
			StopViewportSizeAnimation();
		}
		void StopViewportOriginAnimation() {
			viewportOriginAnimationInProgress = false;
		}
		void StopViewportSizeAnimation() {
			viewportSizeAnimationInProgress = false;
		}
		void AnimateViewportOrigin(bool zoomChanged) {
			if (progress.ActualOriginProgress > 0 || zoomChanged) {
				StopViewportOriginAnimation();
				if (UseSprings) {
					PrepareViewportOriginStoryboard();
					viewportOriginAnimationInProgress = true;
					progress.StartOriginAnimation();
					viewportOriginStoryboardStarted = true;
					ViewportOriginStoryboard.Begin();
				}
			}
		}
		void AnimateViewportSize() {
			StopViewportSizeAnimation();
			if (UseSprings) {
				PrepareViewportSizeStoryboard();
				viewportSizeAnimationInProgress = true;
				progress.StartSizeAnimation();
				viewportSizeStoryboardStarted = true;
				ViewportSizeStoryboard.Begin();
			}
		}
		protected Point CalcActualViewportOrigin(Point viewportOrigin) {
			return new Point(anchorViewportOrigin.X + (viewportOrigin.X - anchorViewportOrigin.X) * progress.ActualOriginProgress,
						  anchorViewportOrigin.Y + (viewportOrigin.Y - anchorViewportOrigin.Y) * progress.ActualOriginProgress);
		}
		Size CalcActualViewportSize(Size viewportSize) {
			return new Size(anchorViewportSize.Width + (viewportSize.Width - anchorViewportSize.Width) * progress.ActualSizeProgress,
				anchorViewportSize.Height + (viewportSize.Height - anchorViewportSize.Height) * progress.ActualSizeProgress);
		}
		void PrepareViewportOriginStoryboard() {
			if (viewportOriginStoryboardStarted)
				ViewportOriginStoryboard.Stop();
			ViewportOriginStoryboard.Children.Clear();
			AnimationHelper.PrepareStoryboard(ViewportOriginStoryboard, progress, ViewportAnimationProgress.OriginProgressProperty);
		}
		void PrepareViewportSizeStoryboard() {
			if (viewportSizeStoryboardStarted)
				ViewportSizeStoryboard.Stop();
			ViewportSizeStoryboard.Children.Clear();
			AnimationHelper.PrepareStoryboard(ViewportSizeStoryboard, progress, ViewportAnimationProgress.SizeProgressProperty);
		}
		protected virtual void OnIsVisibleChanged() {
			if (Map != null)
				Map.UpdateBoundingRect();
		}
		protected virtual void OnLeftMouseClick(Point point) {
		}
		protected virtual void ViewportUpdated(bool zoomLevelChanged) {
			UpdateContentVisible();
			UpdateContentClip();
			if (Map != null)
				Map.UpdateScalePanel();
		}
		protected virtual DataLoadedEventArgs CreateDataLoadedEventArgs() {
			return new DataLoadedEventArgs();
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			if (Map != null) 
				Viewport = Map.Viewport;
			UpdateContentVisible();
			UpdateContentClip();
		}
		protected internal virtual Size GetMapBaseSizeInPixels() {
			return DefaultMapSize;
		}
		protected abstract Size GetMapSizeInPixels(double zoomLevel);
		protected internal abstract void CheckCompatibility();
		protected internal virtual CoordBounds GetBoundingRect() {
			return CoordBounds.Empty;
		}
		protected internal virtual void UpdateItemsLayout() {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			rootVisualElement = GetTemplateChild("PART_RootVisual") as Panel;
			UpdateContentVisible();
		}
		internal virtual void UpdateBoundingRect() {
			if (Map != null)
				Map.UpdateBoundingRect();
		}
		internal void LeftMouseClick(Point point) {
			OnLeftMouseClick(point);
		}
		protected internal virtual Point CoordPointToScreenPointZeroOffset(CoordPoint point, bool useSpringsAnimation, bool shouldNormalize) {
			if (shouldNormalize)
				point = CoordinateSystemHelper.CreateNormalizedPoint(point);
			return MapUnitToScreenPoint(ActualCoordinateSystem.CoordPointToMapUnit(point), useSpringsAnimation);
		}
		protected internal Point CoordPointToScreenPoint(CoordPoint point, bool useSpringsAnimation, bool shouldNormalize) {
			return MapUnitToScreenPoint(ActualCoordinateSystem.CoordPointToMapUnit(point, shouldNormalize), useSpringsAnimation);
		}
		protected internal Rect CalculateViewportRegion(CoordPoint leftTop, CoordPoint rightBottom) {
			MapUnit leftTopUnit = ActualCoordinateSystem.CoordPointToMapUnit(leftTop);
			MapUnit rightBottomUnit = ActualCoordinateSystem.CoordPointToMapUnit(rightBottom);
			Point leftTopPt = MapUnitToScreenPoint(leftTopUnit, false);
			Point rightBottomPt = MapUnitToScreenPoint(rightBottomUnit, false);
			double x = Math.Min(leftTopPt.X, rightBottomPt.X);
			double y = Math.Min(leftTopPt.Y, rightBottomPt.Y);
			return new Rect(x, y, Math.Abs(rightBottomPt.X - leftTopPt.X), Math.Abs(rightBottomPt.Y - leftTopPt.Y));
		}
		protected internal double CalculateRegionZoom(CoordPoint leftTop, CoordPoint rightBottom, double zoomLevel, double padding) {
			Rect viewPortRect = CalculateViewportRegion(leftTop, rightBottom);
			double zoomCoeff = 1.0f;
			bool adjustByHeight = (viewPortRect.Height / ViewportInPixels.Height) < (viewPortRect.Width / ViewportInPixels.Width);
			if (adjustByHeight)
				zoomCoeff = ViewportInPixels.Width / viewPortRect.Width;
			else
				zoomCoeff = ViewportInPixels.Height / viewPortRect.Height;
			Size currentSize = GetMapSizeInPixels(zoomLevel);
			Size newSize = new Size(currentSize.Width * zoomCoeff, currentSize.Height * zoomCoeff);
			Size baseSize = GetMapBaseSizeInPixels();
			double baseLen = adjustByHeight ? baseSize.Height : baseSize.Width;
			double newLen = adjustByHeight ? newSize.Height : newSize.Width;
			double value = newLen / baseLen;
			padding = Math.Min(1.0, Math.Max(padding, 0.0));
			value *= 1.0 - padding;
			if (value < 1.0) {
				return value;
			}
			double newZoomLevel = Math.Log(value, 2) + 1;
			return newZoomLevel;
		}
		protected internal Point MapUnitToScreenZeroOffset(MapUnit mapUnit) {
			if (CanApplyZoomStrategy)
				return MapUnitToScreenPoint(mapUnit, true);
			if (View == null)
				return new Point();
			Rect viewport = IsLoaded && viewportOriginEverUpdated ? ActualViewport : ActualCoordinateSystem.CalculateOffsetViewport(ZoomLevel, ViewportInPixels, MapSize);
			if (viewport.IsEmpty)
				return new Point();
			double unitFactorX = viewport.Width > 0 ? ViewportInPixels.Width / viewport.Width : 0.0;
			double unitFactorY = viewport.Height > 0 ? ViewportInPixels.Height / viewport.Height : 0.0;
			return new Point(mapUnit.X * unitFactorX, mapUnit.Y * unitFactorY);
		}
		protected internal void RaiseDataLoaded() { 
			if (DataLoaded != null)
				DataLoaded(this, CreateDataLoadedEventArgs());
		}
		protected internal MapUnit ScreenPointToMapUnitInternal(Point point, bool useSpringsAnimation, bool shouldNormalize) {
			if(View == null)
				return new MapUnit();
			Rect viewport = useSpringsAnimation ? ActualViewport : View.Viewport;
			double pixelFactorX = viewport.Width / ViewportInPixels.Width;
			double pixelFactorY = viewport.Height / ViewportInPixels.Height;
			MapUnit unit = new MapUnit(point.X * pixelFactorX + viewport.X, point.Y * pixelFactorY + viewport.Y);
			return shouldNormalize ? MapUnit.Normalize(unit) : unit;
		}
		public MapUnit ScreenPointToMapUnit(Point point) {
			return ScreenPointToMapUnit(point, false);
		}
		public Point MapUnitToScreenPoint(MapUnit mapUnit) {
			return MapUnitToScreenPoint(mapUnit, false);
		}
		public GeoPoint ScreenToGeoPoint(Point point) {
			return ActualProjection != null ? ActualProjection.MapUnitToGeoPoint(ScreenPointToMapUnit(point, false)) : new GeoPoint();
		}
		public Point GeoToScreenPoint(GeoPoint point) {
			return CoordPointToScreenPoint(point, false, true);
		}
		public MapUnit ScreenPointToMapUnit(Point point, bool useSpringsAnimation) {
			return ScreenPointToMapUnitInternal(point, useSpringsAnimation, true);
		}
		public Point MapUnitToScreenPoint(MapUnit mapUnit, bool useSpringsAnimation) {
			if (View == null)
				return new Point();
			Rect viewport = useSpringsAnimation ? ActualViewport : View.Viewport;
			double unitFactorX = viewport.Width > 0 ? ViewportInPixels.Width / viewport.Width : 0.0;
			double unitFactorY = viewport.Height > 0 ? ViewportInPixels.Height / viewport.Height : 0.0;
			return new Point((mapUnit.X - viewport.X) * unitFactorX, (mapUnit.Y - viewport.Y) * unitFactorY);
		}
		public GeoPoint ScreenToGeoPoint(Point point, bool useSpringsAnimation) {
			return ActualProjection != null ? ActualProjection.MapUnitToGeoPoint(ScreenPointToMapUnit(point, useSpringsAnimation)) : new GeoPoint();
		}
		public Point GeoToScreenPoint(GeoPoint point, bool useSpringsAnimation) {
			return CoordPointToScreenPoint(point, useSpringsAnimation, true);
		}
		public MapUnit GeoPointToMapUnit(GeoPoint point) {
			GeoPoint normalized = GeoPoint.Normalize(point);
			return ActualProjection.GeoPointToMapUnit(normalized);
		}
		public GeoPoint MapUnitToGeoPoint(MapUnit unit) {
			MapUnit normalized = MapUnit.Normalize(unit);
			return ActualProjection.MapUnitToGeoPoint(normalized);
		}
		public Size GeoToKilometersSize(GeoPoint anchorPoint, Size size) {
			return ActualProjection != null ? ActualProjection.GeoToKilometersSize(anchorPoint, size) : new Size(0, 0);
		}
		public Size KilometersToGeoSize(GeoPoint anchorPoint, Size size) {
			return ActualProjection != null ? ActualProjection.KilometersToGeoSize(anchorPoint, size) : new Size(0, 0);
		} 
	}
}
