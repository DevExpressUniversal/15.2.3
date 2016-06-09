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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram.Native {
	public class LayerInfo {
		static readonly DependencyPropertyKey LayerPropertyKey;
		public static readonly DependencyProperty LayerProperty;
		public static ILayer GetLayer(UIElement layerVisual) { return (ILayer)layerVisual.GetValue(LayerProperty); }
		static void SetLayer(UIElement layerVisual, ILayer layer) { layerVisual.SetValue(LayerPropertyKey, layer); }
		static LayerInfo() {
			DependencyPropertyRegistrator<LayerInfo>.New()
				.RegisterAttachedReadOnly((UIElement d) => GetLayer(d), out LayerPropertyKey, out LayerProperty, null)
				;
		}
		public LayerInfo(ILayer layer, UIElement layerUI = null) {
			Layer = layer;
			LayerUI = layerUI ?? (UIElement)layer;
			SetLayer(LayerUI, Layer);
		}
		public ILayer Layer { get; private set; }
		public UIElement LayerUI { get; private set; }
	}
	public interface ILayer {
		void Update(Point offset, Point location, Size viewport, double zoomFactor);
	}
}
namespace DevExpress.Xpf.Diagram {
	[TemplatePart(Name = PART_DiagramArea, Type = typeof(Canvas))]
	[TemplatePart(Name = PART_HorizontalRulerArea, Type = typeof(Decorator))]
	[TemplatePart(Name = PART_VerticalRulerArea, Type = typeof(Decorator))]
	public class LayersHost : Control, IScrollInfo, ILayersHost {
		protected const string PART_DiagramArea = "DiagramArea";
		protected internal const string PART_HorizontalRulerArea = "HorizontalRulerArea";
		protected internal const string PART_VerticalRulerArea = "VerticalRulerArea";
		static LayersHost() {
			DependencyPropertyRegistrator<LayersHost>.New()
				.OverrideDefaultStyleKey()
			;
		}
		public readonly LayersHostController Controller;
		DiagramControl Diagram { get { return (DiagramControl)Controller.Diagram; } }
		ScrollViewer viewer;
		Canvas diagramArea;
		Decorator hRulerArea, vRulerArea;
		LayerInfo[] layers;
		HorizontalRuler hRuler;
		VerticalRuler vRuler;
#if DEBUGTEST
		internal Canvas DiagramAreaForTests { get { return diagramArea; } }
#endif 
		public LayersHost() {
			Controller = new LayersHostController(this);
			layers = new LayerInfo[0];
		}
		#region ILayersHost
		LayersHostController ILayersHost.Controller { get { return Controller; } }
		void ILayersHost.SetCursor(DiagramCursor cursor) {
			Cursor = cursor.ToCursor();
		}
		void ILayersHost.FocusSurface() {
			Focus();
		}
		IInputElement ILayersHost.FindInputElement(Point displayPosition) {
			return FindInputElement(displayPosition);
		}
		IMouseArgs ILayersHost.CreatePlatformMouseArgs() {
			return CreatePlatformMouseArgs();
		}
		Size ILayersHost.ViewportSize { get { return diagramArea.RenderSize; } }
		void ILayersHost.InvalidateScrollInfo() {
			InvalidateScrollInfo();
		}
		void ILayersHost.UpdateLayers(double zoom) {
			foreach(var layer in layers.Select(x => x.Layer).Concat(hRuler.YieldIfNotNull()).Concat(vRuler.YieldIfNotNull())) {
				layer.Update(Controller.DisplayOffset, Controller.Extent.Location, Controller.Viewport, zoom);
			}
		}
		#endregion
		public override void OnApplyTemplate() {
			DettachFromDiagramArea();
			base.OnApplyTemplate();
			diagramArea = (Canvas)GetTemplateChild(PART_DiagramArea);
			hRulerArea = (Decorator)GetTemplateChild(PART_HorizontalRulerArea);
			vRulerArea = (Decorator)GetTemplateChild(PART_VerticalRulerArea);
			DiagramInput.SetInputElementFactory(diagramArea, () => new RootItemInputElement(() => Diagram.RootItem));
			AssignLayers();
			AttachToDiagramArea();
		}
		internal void CaptureMouseInput() {
			Mouse.Capture(diagramArea);
		}
		internal void ReleaseMouseInput() {
			Mouse.Capture(null);
		}
		internal Point GetPosition(Func<FrameworkElement, Point> getPosition) {
			return Controller.TransformToLogicPoint(getPosition(diagramArea));
		}
		internal void SetLayers(LayerInfo[] layers, HorizontalRuler hRuler, VerticalRuler vRuler) {
			this.layers = layers;
			this.hRuler = hRuler;
			this.vRuler = vRuler;
			AssignLayers();
		}
		void AssignLayers() {
			if(diagramArea != null) {
				diagramArea.Children.Clear();
				layers.ForEach(x => diagramArea.Children.Add(x.LayerUI));
			}
			if(hRulerArea != null)
				hRulerArea.Child = hRuler;
			if(vRulerArea != null)
				vRulerArea.Child = vRuler;
		}
		IInputElement FindInputElement(Point displayPoint) {
			var element = diagramArea.Children.Cast<FrameworkElement>().Reverse().Concat(diagramArea.Yield())
				.Select(x => FindInputElementInLayer(displayPoint, x)).FirstOrDefault(x => x != null);
#if DEBUGTEST
#endif
			return element ?? NullInputElement.Instance;
		}
		IInputElement FindInputElementInLayer(Point displayPoint, FrameworkElement layer) {
			var position = diagramArea.TranslatePoint(displayPoint, layer);
			IInputElement inputElement = null;
			VisualTreeHelper.HitTest(
				layer, potentialHitTestTarget => HitTestFilterBehavior.Continue,
				result => {
					var hitTestElement = result.With(x => x.VisualHit);
					if((hitTestElement as FrameworkElement).If(x => x.Visibility == Visibility.Visible).ReturnSuccess()) {
						var parents = new DependencyObject[] { hitTestElement }.Concat(LayoutTreeHelper.GetVisualParents(hitTestElement, layer)).ToArray();
						inputElement = parents
						   .OfType<FrameworkElement>()
						   .Select(x => DiagramInput.GetInputElementFactory(x))
						   .FirstOrDefault(x => x != null)
						   .With(x => x());
					}
					return inputElement != null ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
				},
				new PointHitTestParameters(position));
			return inputElement;
		}
		void InvalidateScrollInfo() {
			viewer.Do(x => x.InvalidateScrollInfo());
		}
		#region scroll info
		bool IScrollInfo.CanHorizontallyScroll { get; set; }
		bool IScrollInfo.CanVerticallyScroll { get; set; }
		double IScrollInfo.ExtentHeight { get { return Controller.Extent.Height; } }
		double IScrollInfo.ExtentWidth { get { return Controller.Extent.Width; } }
		double IScrollInfo.HorizontalOffset { get { return Controller.Offset.OffsetPoint(Controller.Extent.Location.InvertPoint()).X; } }
		double IScrollInfo.VerticalOffset { get { return Controller.Offset.OffsetPoint(Controller.Extent.Location.InvertPoint()).Y; } }
		double IScrollInfo.ViewportHeight { get { return Controller.Viewport.Height; } }
		double IScrollInfo.ViewportWidth { get { return Controller.Viewport.Width; } }
		ScrollViewer IScrollInfo.ScrollOwner {
			get { return viewer; }
			set {
				viewer = value;
				InvalidateScrollInfo();
			}
		}
		const double lineDelta = 5;
		const double pageCoeff = .5;
		const double wheelDelta = 20;
		#region line
		void IScrollInfo.LineDown() {
			Controller.ChangeVerticalOffset(lineDelta);
		}
		void IScrollInfo.LineUp() {
			Controller.ChangeVerticalOffset(-lineDelta);
		}
		void IScrollInfo.LineLeft() {
			Controller.ChangeHorizontalOffset(-lineDelta);
		}
		void IScrollInfo.LineRight() {
			Controller.ChangeHorizontalOffset(lineDelta);
		}
		#endregion
		#region page
		void IScrollInfo.PageDown() {
			Controller.ChangeVerticalOffset(Controller.Viewport.Height * pageCoeff);
		}
		void IScrollInfo.PageUp() {
			Controller.ChangeVerticalOffset(-Controller.Viewport.Height * pageCoeff);
		}
		void IScrollInfo.PageLeft() {
			Controller.ChangeHorizontalOffset(-Controller.Viewport.Width * pageCoeff);
		}
		void IScrollInfo.PageRight() {
			Controller.ChangeHorizontalOffset(Controller.Viewport.Width * pageCoeff);
		}
		#endregion
		#region wheel
		void IScrollInfo.MouseWheelDown() {
			if((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
				Controller.ChangeHorizontalOffset(wheelDelta);
			else
				Controller.ChangeVerticalOffset(wheelDelta);
		}
		void IScrollInfo.MouseWheelUp() {
			if((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
				Controller.ChangeHorizontalOffset(-wheelDelta);
			else
				Controller.ChangeVerticalOffset(-wheelDelta);
		}
		void IScrollInfo.MouseWheelLeft() {
			Controller.ChangeHorizontalOffset(-wheelDelta);
		}
		void IScrollInfo.MouseWheelRight() {
			Controller.ChangeHorizontalOffset(wheelDelta);
		}
		#endregion
		void IScrollInfo.SetHorizontalOffset(double offset) {
			Controller.SetHorizontalOffset(offset + Controller.Extent.Location.X);
		}
		void IScrollInfo.SetVerticalOffset(double offset) {
			Controller.SetVerticalOffset(offset + Controller.Extent.Location.Y);
		}
		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
			return rectangle;
		}
		#endregion
		#region input
		void AttachToDiagramArea() {
			diagramArea.Do(x => {
				x.MouseMove += OnHostMouseMove;
				x.MouseLeave += OnHostMouseLeave;
				x.MouseDown += OnHostMouseDown;
				x.MouseUp += OnHostMouseUp;
				x.LostMouseCapture += OnLostMouseCapture;
				x.MouseWheel += OnMouseWheel;
				x.SizeChanged += OnSizeChanged;
				Controller.UpdateOnInit();
			});
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			Controller.UpdateOnChangeViewport();
		}
		void DettachFromDiagramArea() {
			diagramArea.Do(x => {
				x.MouseMove -= OnHostMouseMove;
				x.MouseLeave -= OnHostMouseLeave;
				x.MouseDown -= OnHostMouseDown;
				x.MouseUp -= OnHostMouseUp;
				x.LostMouseCapture -= OnLostMouseCapture;
				x.MouseWheel -= OnMouseWheel;
				x.SizeChanged -= OnSizeChanged;
			});
		}
		void OnHostMouseUp(object sender, MouseButtonEventArgs e) {
			e.HandleEvent(() => Diagram.Controller.ProcessMouseUp(CreatePlatformMouseButtonArgs(e)));
		}
		void OnHostMouseDown(object sender, MouseButtonEventArgs e) {
			e.HandleEvent(() => Diagram.Controller.ProcessMouseDown(CreatePlatformMouseButtonArgs(e)));
		}
		void OnHostMouseMove(object sender, MouseEventArgs e) {
			e.HandleEvent(() => Diagram.Controller.ProcessMouseMove(CreatePlatformMouseArgs()));
		}
		void OnHostMouseLeave(object sender, MouseEventArgs e) {
			e.HandleEvent(() => Diagram.Controller.ProcessMouseLeave(CreatePlatformMouseArgs()));
		}
		void OnMouseWheel(object sender, MouseWheelEventArgs e) {
			if((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
				e.Handled = true;
				Diagram.Controller.Zoom(e.Delta, CreatePlatformMouseArgs());
			}
		}
		void OnLostMouseCapture(object sender, MouseEventArgs e) {
			Diagram.Controller.ProcessLostMouseCapture(CreatePlatformMouseArgs());
		}
		IMouseButtonArgs CreatePlatformMouseButtonArgs(MouseButtonEventArgs e) {
			return new PlatformMouseButtonArgs(e, this);
		}
		internal IMouseArgs CreatePlatformMouseArgs() {
			return new PlatformMouseArgs(this);
		}
		#endregion
	}
}
