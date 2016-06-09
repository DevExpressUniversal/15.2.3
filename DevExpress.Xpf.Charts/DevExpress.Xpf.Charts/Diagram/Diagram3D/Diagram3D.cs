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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Diagram3D : Diagram {
		internal const double minZoomPercent = 1.0;
		const double maxZoomPercent = 500.0;
		const double zoomPercentStep = 1.5;
		const double maxScrollPercent = 100.0;
		const double mouseWheelStep = 0.1;
		public static readonly DependencyProperty ContentTransformProperty;
		public static readonly DependencyProperty PerspectiveAngleProperty;
		public static readonly DependencyProperty ZoomPercentProperty;
		public static readonly DependencyProperty HorizontalScrollPercentProperty;
		public static readonly DependencyProperty VerticalScrollPercentProperty;
		public static readonly DependencyProperty LightingProperty;
		public static readonly DependencyProperty RuntimeRotationProperty;
		public static readonly DependencyProperty RuntimeScrollingProperty;
		public static readonly DependencyProperty RuntimeZoomingProperty;
		public static readonly DependencyProperty NavigationOptionsProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DContentTransform"),
#endif
		Category(Categories.Presentation)
		]
		public Transform3D ContentTransform {
			get { return (Transform3D)GetValue(ContentTransformProperty); }
			set { SetValue(ContentTransformProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DPerspectiveAngle"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double PerspectiveAngle {
			get { return (double)GetValue(PerspectiveAngleProperty); }
			set { SetValue(PerspectiveAngleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DZoomPercent"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double ZoomPercent {
			get { return (double)GetValue(ZoomPercentProperty); }
			set { SetValue(ZoomPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DVerticalScrollPercent"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double VerticalScrollPercent {
			get { return (double)GetValue(VerticalScrollPercentProperty); }
			set { SetValue(VerticalScrollPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DHorizontalScrollPercent"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double HorizontalScrollPercent {
			get { return (double)GetValue(HorizontalScrollPercentProperty); }
			set { SetValue(HorizontalScrollPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DLighting"),
#endif
		Category(Categories.Presentation)
		]
		public DiagramLightingCollection Lighting {
			get { return (DiagramLightingCollection)GetValue(LightingProperty); }
			set { SetValue(LightingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DRuntimeRotation"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool RuntimeRotation {
			get { return (bool)GetValue(RuntimeRotationProperty); }
			set { SetValue(RuntimeRotationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DRuntimeScrolling"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool RuntimeScrolling {
			get { return (bool)GetValue(RuntimeScrollingProperty); }
			set { SetValue(RuntimeScrollingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DRuntimeZooming"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool RuntimeZooming {
			get { return (bool)GetValue(RuntimeZoomingProperty); }
			set { SetValue(RuntimeZoomingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DNavigationOptions"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public NavigationOptions3D NavigationOptions {
			get { return (NavigationOptions3D)GetValue(NavigationOptionsProperty); }
			set { SetValue(NavigationOptionsProperty, value); }
		}
		static Diagram3D() {
			Type ownerType = typeof(Diagram3D);
			ContentTransformProperty = DependencyProperty.Register("ContentTransform", typeof(Transform3D), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ContentTransformPropertyChanged));
			PerspectiveAngleProperty = DependencyProperty.Register("PerspectiveAngle", typeof(double), ownerType,
				new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender, ChartElementHelper.Update),
				new ValidateValueCallback(ValidatePerspectiveAngle));
			ZoomPercentProperty = DependencyProperty.Register("ZoomPercent", typeof(double), ownerType,
				new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender),
				new ValidateValueCallback(ValidateZoomPercent));
			HorizontalScrollPercentProperty = DependencyProperty.Register("HorizontalScrollPercent", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
				new ValidateValueCallback(ValidateScrollPercent));
			VerticalScrollPercentProperty = DependencyProperty.Register("VerticalScrollPercent", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
				new ValidateValueCallback(ValidateScrollPercent));
			LightingProperty = DependencyProperty.Register("Lighting", typeof(DiagramLightingCollection), ownerType,
				new FrameworkPropertyMetadata(null,
					FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentArrange,
					ChartElementHelper.ChangeOwnerAndUpdate));
			RuntimeRotationProperty = DependencyProperty.Register("RuntimeRotation", typeof(bool), ownerType, new PropertyMetadata(true));
			RuntimeScrollingProperty = DependencyProperty.Register("RuntimeScrolling", typeof(bool), ownerType, new PropertyMetadata(true));
			RuntimeZoomingProperty = DependencyProperty.Register("RuntimeZooming", typeof(bool), ownerType, new PropertyMetadata(true));
			NavigationOptionsProperty = DependencyProperty.Register("NavigationOptions", typeof(NavigationOptions3D), typeof(Diagram3D));
		}
		static void ContentTransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Diagram3D diagram = d as Diagram3D;
			if (diagram != null) {
				Transform3D transform = e.NewValue as Transform3D;
				diagram.actualContentTransform = transform == null ? diagram.CreateDefaultContentTransform() : transform;
			}
		}
		static bool ValidateMinZoomPercent(double zoom) {
			return zoom >= minZoomPercent;
		}
		static bool ValidateMaxZoomPercent(double zoom) {
			return zoom <= maxZoomPercent;
		}
		internal static bool ValidatePerspectiveAngle(object value) {
			double perspectiveAngle = (double)value;
			return perspectiveAngle >= 0.0 && perspectiveAngle < 180.0;
		}
		internal static bool ValidateZoomPercent(object value) {
			double zoomPercent = (double)value;
			return ValidateMinZoomPercent(zoomPercent) && ValidateMaxZoomPercent(zoomPercent);
		}
		internal static bool ValidateScrollPercent(object value) {
			double scrollPercent = (double)value;
			return scrollPercent >= -maxScrollPercent && scrollPercent <= maxScrollPercent;
		}
		double startZoomPercent;
		Transform3D actualContentTransform;
		readonly List<VisualContainer> visualContainers = new List<VisualContainer>();
		internal List<VisualContainer> VisualContainers { get { return visualContainers; } }
		bool AnimationInProgress {
			get {
				ChartControl chart = ChartControl;
				if (chart != null)
					foreach (ChartAnimationRecord record in chart.AnimationRecords)
						foreach (ChartAnimation animation in record.Animations)
							if ((animation is Diagram3DAnimation) && record.Progress < 1.0)
								return true;
				return false;
			}
		}
		internal Matrix3D ContentTransformMatrix { get { return ActualContentTransform.Value; } }
		internal NavigationOptions3D ActualNavigationOptions { get { return NavigationOptions ?? new NavigationOptions3D(); } }
		internal DiagramLightingCollection ActualLighting { get { return Lighting.Count == 0 ? CreateDefaultLighting() : Lighting; } }
		protected internal override bool IsMouseNavigationEnabled { get { return !AnimationInProgress && ActualNavigationOptions.UseMouse; } }
		protected internal override bool IsKeyboardNavigationEnabled { get { return !AnimationInProgress && ActualNavigationOptions.UseKeyboard; } }
		protected internal override bool IsManipulationNavigationEnabled { get { return !AnimationInProgress && ActualNavigationOptions.UseTouchDevice; } }
		protected internal override Rect ActualViewport {
			get {
				double width = Math.Max(DesiredSize.Width - Margin.Left - BorderThickness.Left - Padding.Left - Padding.Right - BorderThickness.Right - Margin.Right, 0);
				double height = Math.Max(DesiredSize.Height - Margin.Top - BorderThickness.Top - Padding.Top - Padding.Bottom - BorderThickness.Bottom - Margin.Bottom, 0);
				return new Rect(0, 0, width, height);
			}
		}
		protected override bool Is3DView { get { return true; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Diagram3DActualContentTransform")]
#endif
		public Transform3D ActualContentTransform { get { return actualContentTransform; } }
		public Diagram3D() {
			actualContentTransform = CreateDefaultContentTransform();
			Lighting = new DiagramLightingCollection();
		}
		void PerformDrag(double dx, double dy) {
			if (RuntimeRotation) {
				double factor = Math.PI * 0.25;
				double xAngle = dx * factor;
				double yAngle = dy * factor;
				ContentTransform = new MatrixTransform3D(Diagram3DDomain.PerformRotation(ContentTransformMatrix, xAngle, yAngle));
			}
		}
		void SetZoomPercent(double zoomPercent) {
			if (RuntimeZooming) {
				if (ValidateMinZoomPercent(zoomPercent))
					ZoomPercent = ValidateMaxZoomPercent(zoomPercent) ? zoomPercent : maxZoomPercent;
				else
					ZoomPercent = minZoomPercent;
			}
		}
		void SetHorizontalScrollPercent(double scrollPercent) {
			if (RuntimeScrolling)
				HorizontalScrollPercent = ValidateScrollPercent(scrollPercent) ? scrollPercent : maxScrollPercent * Math.Sign(scrollPercent);
		}
		void SetVerticalScrollPercent(double scrollPercent) {
			if (RuntimeScrolling)
				VerticalScrollPercent = ValidateScrollPercent(scrollPercent) ? scrollPercent : maxScrollPercent * Math.Sign(scrollPercent);
		}
		bool InChart(Point chartPosition) {
			return chartPosition.X > 0 && chartPosition.Y > 0 && chartPosition.X < ChartControl.ActualWidth && chartPosition.Y < ChartControl.ActualHeight;
		}
		protected void PerformRender() {
			foreach (VisualContainer container in visualContainers)
				container.Clear();
			if (ChartControl != null && visualContainers.Count != 0 && Visibility == Visibility.Visible && ActualViewport.Width > 0 && ActualViewport.Height > 0) {
				foreach (VisualContainer visualContainer in visualContainers) {
					Diagram3DDomain domain = CreateDomain(visualContainer);
					if (domain != null) {
						domain.ValidateSeriesPointsCache();
						domain.Render(visualContainer);
					}
				}
			}
		}
		protected void PerformTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) {
			if (oldTemplate != newTemplate)
				VisualContainers.Clear();
		}
		protected virtual Transform3D CreateDefaultContentTransform() {
			Transform3DGroup transform = new Transform3DGroup();
			transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -40)));
			transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 20)));
			return transform;
		}
		protected virtual DiagramLightingCollection CreateDefaultLighting() {
			Color ambientColor = new Color();
			ambientColor.R = ambientColor.G = ambientColor.B = 105;
			DiagramLightingCollection collection = new DiagramLightingCollection();
			collection.Add(new AmbientLight(ambientColor));
			collection.Add(new DirectionalLight(Colors.White, new Vector3D(1.1, -1, -1.8)));
			return collection;
		}
		protected override void OnRender(DrawingContext drawingContext) {
			PerformRender();
		}
		protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) {
			base.OnTemplateChanged(oldTemplate, newTemplate);
			PerformTemplateChanged(oldTemplate, newTemplate);
		}
		protected override ISeriesItem GetSeriesItem(Series series) {
			return series.Cache;
		}
		protected internal virtual Diagram3DDomain CreateDomain(VisualContainer visualContainer) {
			return null;
		}
		protected internal override void InvalidateDiagram() {
			InvalidateVisual();
		}
		protected internal override void NavigationDrag(Point chartPosition, int dx, int dy, NavigationType navigationType, MouseEventArgs e) {
			PerformDrag(dx, dy);
		}
		protected internal override void NavigationZoomIn() {
			SetZoomPercent(ZoomPercent * zoomPercentStep);
		}
		protected internal override void NavigationZoomOut() {
			SetZoomPercent(ZoomPercent / zoomPercentStep);
		}
		protected internal override bool NavigationZoom(Point position, int delta, ZoomingKind zoomingKind, bool isDragging) {
			SetZoomPercent(ZoomPercent * (1.0 + mouseWheelStep * delta));
			ChartControl.Focus();
			return true;
		}
		protected internal override bool NavigationBeginDrag(Point position, MouseButtonEventArgs e, bool isShiftKey) {
			ChartControl.Focus();
			if (isShiftKey) {
				NavigationZoomIn();
				return false;
			}
			return e.LeftButton == MouseButtonState.Pressed && RuntimeRotation;
		}
		protected internal override bool NavigationScrollHorizontally(int delta, NavigationType navigationType) {
			SetHorizontalScrollPercent(HorizontalScrollPercent - delta);
			return true;
		}
		protected internal override bool NavigationScrollVertically(int delta, NavigationType navigationType) {
			SetVerticalScrollPercent(VerticalScrollPercent + delta);
			return true;
		}
		protected internal override bool NavigationCanZoomIn(Point chartPosition, bool useFocusedPane) {
			if (InChart(chartPosition) && RuntimeZooming)
				return ZoomPercent != maxZoomPercent;
			return false;
		}
		protected internal override bool NavigationCanZoomOut(Point chartPosition, bool useFocusedPane) {
			if (InChart(chartPosition) && RuntimeZooming)
				return ValidateMinZoomPercent(ZoomPercent / zoomPercentStep);
			return false;
		}
		protected internal override bool NavigationInDiagram(Point chartPosition) {
			return true;
		}
		protected internal override bool NavigationCanDrag(Point chartPosition, bool isDragging) {
			return IsMouseNavigationEnabled && RuntimeRotation && InChart(chartPosition);
		}
		protected internal override bool ManipulationStart(Point pt) {
			ChartControl.Focus();
			startZoomPercent = ZoomPercent;
			return true;
		}
		protected internal override void ManipulationZoom(double scaleX, double scaleY) {
			SetZoomPercent(startZoomPercent * Math.Sqrt(scaleX * scaleY));
		}
		protected internal override void ManipulationRotate(double degreeAngle) {
			if (RuntimeRotation)
				ContentTransform = new MatrixTransform3D(Diagram3DDomain.PerformRotation(ContentTransformMatrix, -degreeAngle));
		}
		protected internal override Point ManipulationDrag(Point translation) {
			PerformDrag(translation.X, translation.Y);
			return new Point();
		}
	}
	public class DiagramLightingCollection : ChartElementCollection<Light> {
	}
}
