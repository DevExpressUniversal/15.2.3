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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[
	TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas)),
	NonCategorized
	]
	public class MultiScaleImage : Control, IViewportAnimatableElement {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		public static readonly DependencyProperty SourceProperty = DependencyPropertyManager.Register("Source",
			typeof(MultiScaleTileSource), typeof(MultiScaleImage), new PropertyMetadata(null, SourcePropertyChanged));
		public static readonly DependencyProperty ViewportOriginProperty = DependencyPropertyManager.Register("ViewportOrigin",
			typeof(Point), typeof(MultiScaleImage), new PropertyMetadata(new Point(0.0, 0.0), ViewportOriginPropertyChanged));
		public static readonly DependencyProperty ViewportWidthProperty = DependencyPropertyManager.Register("ViewportWidth",
			typeof(double), typeof(MultiScaleImage), new PropertyMetadata(1.0, ViewportWidthPropertyChanged));
		public static readonly DependencyProperty UseSpringsProperty = DependencyPropertyManager.Register("UseSprings",
			typeof(bool), typeof(MultiScaleImage), new PropertyMetadata(true));
		public static readonly DependencyProperty CacheOptionsProperty = DependencyPropertyManager.Register("CacheOptions",
			typeof(CacheOptions), typeof(MultiScaleImage), new PropertyMetadata());
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty MapKindProperty = DependencyPropertyManager.Register("MapKind",
			typeof(object), typeof(MultiScaleImage), new PropertyMetadata("", MapKindPropertyChanged));
		public MultiScaleTileSource Source {
			get { return (MultiScaleTileSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public Point ViewportOrigin {
			get { return (Point)GetValue(ViewportOriginProperty); }
			set { SetValue(ViewportOriginProperty, value); }
		}
		public double ViewportWidth {
			get { return (double)GetValue(ViewportWidthProperty); }
			set { SetValue(ViewportWidthProperty, value); }
		}
		public bool UseSprings {
			get { return (bool)GetValue(UseSpringsProperty); }
			set { SetValue(UseSpringsProperty, value); }
		}
		public CacheOptions CacheOptions {
			get { return (CacheOptions)GetValue(CacheOptionsProperty); }
			set { SetValue(CacheOptionsProperty, value); }
		}
		internal object MapKind {
			get { return (object)GetValue(MapKindProperty); }
			set { SetValue(MapKindProperty, value); }
		}
		static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MultiScaleImage multiScaleImage = d as MultiScaleImage;
			if (multiScaleImage != null) {
				if (multiScaleImage.Source != null)
					multiScaleImage.Reset();				   
				else if (multiScaleImage.canvas != null)
					multiScaleImage.canvas.Children.Clear();
			}
		}
		static void ViewportOriginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MultiScaleImage multiScaleImage = d as MultiScaleImage;
			if (multiScaleImage != null) {
				multiScaleImage.anchorViewportOrigin = multiScaleImage.CalcActualViewportOrigin((Point)e.OldValue);
				if (multiScaleImage.UseSprings)
					multiScaleImage.AnimateViewportOrigin();
				else
					multiScaleImage.Invalidate();
			}
		}
		static void ViewportWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MultiScaleImage multiScaleImage = d as MultiScaleImage;
			if (multiScaleImage != null) {
				multiScaleImage.anchorViewportWidth = multiScaleImage.CalcActualViewportWidth((double)e.OldValue);
				if (multiScaleImage.UseSprings)
					multiScaleImage.AnimateViewportSize();
				else
					multiScaleImage.Invalidate();
			}
		}
		static void MapKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MultiScaleImage multiScaleImage = d as MultiScaleImage;
			if (multiScaleImage != null)
				multiScaleImage.Reset();
		}
		static Point defaultAnchorViewportOrigin = new Point(0.0, 0.0);
		static double defaultAnchorViewportWidth = 1.0;
		Canvas canvas;
		int zoomLevel;
		int startTileX;
		int endTileX;
		int startTileY;
		int endTileY;
		int maxTile;
		Storyboard viewportOriginStoryboard = null;
		Storyboard viewportSizeStoryboard = null;
		Point anchorViewportOrigin = defaultAnchorViewportOrigin;
		double anchorViewportWidth = defaultAnchorViewportWidth;
		bool viewportOriginAnimationInProgress = false;
		bool viewportSizeAnimationInProgress = false;
		bool viewportOriginStoryboardStarted;
		bool viewportSizeStoryboardStarted;
		CacheOptions defaultCacheOptions = new CacheOptions();
		Size printingSize = Size.Empty;
		readonly MultiScaleTilesManager tilesManager;
		readonly ViewportAnimationProgress progress;
		Point ActualViewportOrigin { get { return UseSprings ? CalcActualViewportOrigin(ViewportOrigin) : ViewportOrigin; } }
		double ActualViewportWidth { get { return UseSprings ? CalcActualViewportWidth(ViewportWidth) : ViewportWidth; } }
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
		internal Canvas Canvas { get { return canvas; } }
		internal int ZoomLevel { get { return zoomLevel; } }
		internal int MaxZoomLevel { get { return Source != null ? Source.MaxZoomLevel : 0; } }
		internal int ZoomLevelOffset { get { return Source != null ? Source.ZoomLevelOffset : 0; } }
		internal int StartTileX { get { return startTileX; } }
		internal int StartTileY { get { return startTileY; } }
		internal int EndTileX { get { return endTileX; } }
		internal int EndTileY { get { return endTileY; } }
		internal int MaxTile { get { return maxTile; } }
		internal CacheOptions ActualCacheOptions { get { return CacheOptions != null ? CacheOptions : defaultCacheOptions; } }
		internal bool IsDataReady { get; private set; }
		internal ImageTilesLayer Layer { get; set; }
		public MultiScaleImage() {
			DefaultStyleKey = typeof(MultiScaleImage);
			tilesManager = new MultiScaleTilesManager(this);
			progress = new ViewportAnimationProgress(this);
			this.Loaded += new RoutedEventHandler(MultiScaleImage_Loaded);			
		}
		#region IViewportAnimatableElement implementation
		bool IViewportAnimatableElement.OriginInProgress { get { return viewportOriginAnimationInProgress; } }
		bool IViewportAnimatableElement.SizeInProgress { get { return viewportSizeAnimationInProgress; } }
		void IViewportAnimatableElement.ProgressChanged() {
			Invalidate();
		}
		void IViewportAnimatableElement.BeforeSizeProgressCompleting() {
		}
		#endregion
		void MultiScaleImage_Loaded(object sender, RoutedEventArgs e) {
			Invalidate();
		}
		Point CalcActualViewportOrigin(Point viewportOrigin) {
			return anchorViewportOrigin + (viewportOrigin - anchorViewportOrigin) * progress.ActualOriginProgress;
		}
		double CalcActualViewportWidth(double viewportWidth) {
			return anchorViewportWidth + (viewportWidth - anchorViewportWidth) * progress.ActualSizeProgress;
		}
		void StopViewportOriginAnimation() {
			viewportOriginAnimationInProgress = false;
		}
		void StopViewportSizeAnimation() {
			viewportSizeAnimationInProgress = false;
		}
		void AnimateViewportOrigin() {
			if (progress.ActualOriginProgress > 0) {
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
		void PrepareViewportOriginStoryboard() {
			if (viewportOriginStoryboardStarted)
				ViewportOriginStoryboard.Stop();
			ViewportOriginStoryboard.Children.Clear();
			AnimationHelper.PrepareStoryboard(ViewportOriginStoryboard, progress, ViewportAnimationProgress.SizeProgressProperty);
		}
		void PrepareViewportSizeStoryboard() {
			if (viewportSizeStoryboardStarted)
				ViewportSizeStoryboard.Stop();
			ViewportSizeStoryboard.Children.Clear();
			AnimationHelper.PrepareStoryboard(ViewportSizeStoryboard, progress, ViewportAnimationProgress.OriginProgressProperty);
		}
		void OnViewportOriginStoryboardCompleted(object sender, EventArgs e) {
			anchorViewportOrigin = ViewportOrigin;
			StopViewportOriginAnimation();
		}
		void OnViewportSizeStoryboardCompleted(object sender, EventArgs e) {
			anchorViewportWidth = ViewportWidth;
			StopViewportSizeAnimation();
		}
		void RaiseDataLoaded() {
			if (Layer != null)
				Layer.RaiseDataLoaded();
		}
		void Invalidate() {
			if(canvas != null && canvas.ActualHeight > 0 && Source != null) {
				Invalidate(canvas.ActualWidth, canvas.ActualHeight);
			}
		}
		void Invalidate(double actualWidth, double actualHeight) {
			if (canvas != null && actualWidth > 0 && Source != null) {
				double viewportHeight = actualHeight * ActualViewportWidth / actualWidth;
				if (ActualViewportWidth != 0.0 && !double.IsNaN(ActualViewportWidth) && viewportHeight != 0.0 && !double.IsNaN(viewportHeight)) {
					int tilesOnCanvasSide = Convert.ToInt32(Math.Ceiling(actualWidth / (double)Source.TileWidth));
					int previousZoomLevel = zoomLevel;
					zoomLevel = Math.Max(Convert.ToInt32(Math.Floor(Math.Log((double)tilesOnCanvasSide / ActualViewportWidth, 2.0))), 1);
					if (zoomLevel <= MaxZoomLevel) {
						maxTile = Convert.ToInt32(Math.Pow(2.0, (double)zoomLevel));
						startTileX = Math.Max(Convert.ToInt32(Math.Floor(ActualViewportOrigin.X * (double)maxTile)), 0);
						endTileX = Convert.ToInt32(Math.Min(Math.Ceiling((ActualViewportOrigin.X + ActualViewportWidth) * (double)maxTile), (double)maxTile));
						double tilesCountRatio = (double)Source.ImageHeight / (double)Source.ImageWidth * ((double)Source.TileWidth / (double)Source.TileHeight);
						startTileY = Math.Max(Convert.ToInt32(Math.Floor(ActualViewportOrigin.Y * (double)maxTile)), 0);
						endTileY = Convert.ToInt32(Math.Min(Math.Ceiling((ActualViewportOrigin.Y + viewportHeight) * (double)maxTile), (double)maxTile * tilesCountRatio));
						InvalidateImage(previousZoomLevel);
					}
				}
			}
		}
		void InvalidateImage(int previousZoomLevel) {
			tilesManager.BeginUpdate(zoomLevel != previousZoomLevel);
			double viewportHeight = canvas.ActualHeight * ActualViewportWidth / canvas.ActualWidth;
			double canvasActualWidth = printingSize != Size.Empty ? printingSize.Width : canvas.ActualWidth;
			double canvasActualHeight = printingSize != Size.Empty ? printingSize.Height : canvas.ActualHeight;
			double scaleX = canvasActualWidth / (ActualViewportWidth * (double)Source.TileWidth) / (double)maxTile;
			double scaleY = canvasActualHeight / (viewportHeight * (double)Source.TileHeight) / (double)maxTile;
			double tileLeft = (double)(startTileX * Source.TileWidth) - ActualViewportOrigin.X * (double)Source.TileWidth * (double)maxTile;
			tileLeft *= scaleX;
			for (int i = startTileX; i < endTileX; i++) {
				double tileTop = (double)(startTileY * Source.TileHeight) - ActualViewportOrigin.Y * (double)Source.TileHeight * (double)maxTile;
				tileTop *= scaleY;
				for (int j = startTileY; j < endTileY; j++) {
					tilesManager.Update(new TilePositionData(zoomLevel, i, j), tileLeft, tileTop, scaleX, scaleY, zoomLevel != previousZoomLevel, previousZoomLevel);
					tileTop += (double)Source.TileHeight * scaleY;
				}
				tileLeft += (double)Source.TileWidth * scaleX;
			}
			tilesManager.EndUpdate(new TilePositionData(zoomLevel, (startTileX + endTileX) / 2, (startTileY + endTileY) / 2));
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			base.MeasureOverride(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			PrintContainer printContainer = LayoutHelper.FindParentObject<PrintContainer>(this); 
			if(printContainer != null) {
				this.printingSize = finalSize;
				Invalidate(printingSize.Width, printingSize.Height);
			} else {
				this.printingSize = Size.Empty;
				Invalidate();
			}
			RectangleGeometry clipGeometry = new RectangleGeometry();
			clipGeometry.Rect = new Rect(new Point(0.0, 0.0), finalSize);
			Clip = clipGeometry;
			return base.ArrangeOverride(finalSize);
		}
		internal void OnTileWebRequest(MapWebRequestEventArgs e) {
			Action action = new Action(() => { if (Source != null) Source.OnTileWebRequest(e); });
			if (Dispatcher.CheckAccess())
				action.Invoke();
			else
				Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
		}
		internal void SetDataReady(bool value) {
			IsDataReady = value;
			if (value)
				RaiseDataLoaded();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			canvas = (GetTemplateChild("PART_Canvas") as Canvas);
		}
		public void Reset() { 
			 this.tilesManager.Reset();
			 Invalidate();
		}
	}
}
