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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if !SILVERLIGHT
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.LayoutControl {
	public interface IElementSizer : IControl {
		bool CollapseOnDoubleClick { get; }
		FrameworkElement Element { get; }
		Side Side { get; }
		bool UseSizingStep { get; }
	}
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	public class ElementSizer : ControlBase, IElementSizer {
		public static int SizingStep = 5;
		#region Dependency Properties
		public static readonly DependencyProperty CollapseOnDoubleClickProperty =
			DependencyProperty.Register("CollapseOnDoubleClick", typeof(bool), typeof(ElementSizer), new PropertyMetadata(true));
		public static readonly DependencyProperty ElementProperty =
			DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(ElementSizer), null);
		public static readonly DependencyProperty UseSizingStepProperty =
			DependencyProperty.Register("UseSizingStep", typeof(bool), typeof(ElementSizer), null);
		#endregion Dependency Properties
		private Side _Side;
		private double _SizingAreaWidth;
		public ElementSizer() {
			DefaultStyleKey = typeof(ElementSizer);
		}
		public void UpdateBounds() {
			Measure(SizeHelper.Infinite);
			Arrange(CalculateBounds());
		}
		public bool CollapseOnDoubleClick {
			get { return (bool)GetValue(CollapseOnDoubleClickProperty); }
			set { SetValue(CollapseOnDoubleClickProperty, value); }
		}
		public new ElementSizerController Controller { get { return (ElementSizerController)base.Controller; } }
		public FrameworkElement Element {
			get { return (FrameworkElement)GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		public double ElementSize { get { return Controller.ElementSize; } }
		public bool IsSizing { get { return Controller.IsSizing; } }
		public Side Side {
			get { return _Side; }
			set {
				if(_Side != value) {
					_Side = value;
					UpdateTemplate();
				}
			}
		}
		public double SizingAreaWidth {
			get { return _SizingAreaWidth; }
			set { _SizingAreaWidth = Math.Max(0, value); }
		}
		public bool UseSizingStep {
			get { return (bool)GetValue(UseSizingStepProperty); }
			set { SetValue(UseSizingStepProperty, value); }
		}
		public event EventHandler ElementSizeChanging {
			add { Controller.ElementSizeChanging += value; }
			remove { Controller.ElementSizeChanging -= value; }
		}
		public event EventHandler IsSizingChanged {
			add { Controller.IsSizingChanged += value; }
			remove { Controller.IsSizingChanged -= value; }
		}
		#region Template
		const string HorizontalRootElementName = "HorizontalRootElement";
		const string VerticalRootElementName = "VerticalRootElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			VerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if (HorizontalRootElement != null) {
				HorizontalRootElement.SetVisible(Orientation == Orientation.Horizontal);
				if (HorizontalRootElement.GetVisible())
					Cursor = HorizontalRootElement.Cursor;
			}
			if (VerticalRootElement != null) {
				VerticalRootElement.SetVisible(Orientation == Orientation.Vertical);
				if (VerticalRootElement.GetVisible())
					Cursor = VerticalRootElement.Cursor;
			}
#if !SILVERLIGHT
			InvalidateMeasure();
#endif
		}
		protected FrameworkElement HorizontalRootElement { get; set; }
		protected FrameworkElement VerticalRootElement { get; set; }
		#endregion Template
		#region Layout
		protected virtual Rect CalculateBounds() {
			if(Element == null)
				return Bounds;
			var result = LayoutInformation.GetLayoutSlot(Element);
			switch(Side) {
				case Side.Left:
					result.X = result.Left - SizingAreaWidth;
					break;
				case Side.Top:
					result.Y = result.Top - SizingAreaWidth;
					break;
				case Side.Right:
					result.X = result.Right;
					break;
				case Side.Bottom:
					result.Y = result.Bottom;
					break;
			}
			if(Orientation == Orientation.Horizontal)
				result.Height = SizingAreaWidth;
			else
				result.Width = SizingAreaWidth;
			return result;
		}
		protected Orientation Orientation { get { return Side.GetOrientation(); } }
		#endregion Layout
		protected override ControlControllerBase CreateController() {
			return new ElementSizerController(this);
		}
	}
	public class ElementSizerController : ControlControllerBase {
		private bool _IsSizing;
		public ElementSizerController(IElementSizer control)
			: base(control) {
		}
		public double ElementSize {
			get {
				if(IsElementAutoSize) {
					var elementSize = IElementSizer.Element.GetVisualSize();
					return Orientation == Orientation.Horizontal ? elementSize.Height : elementSize.Width;
				}
				else
					return Orientation == Orientation.Horizontal ? IElementSizer.Element.GetRealHeight() : IElementSizer.Element.GetRealWidth();
			}
			set {
				var prevElementSize = ElementSize;
				value = Math.Max(0, value);
				LastElementSize = value;
				if(IsUsingSizingStep())
					value = Math.Round(value / ElementSizer.SizingStep) * ElementSizer.SizingStep;
				if(Orientation == Orientation.Horizontal)
					IElementSizer.Element.Height = value;
				else
					IElementSizer.Element.Width = value;
				if(ElementSize != prevElementSize)
					OnElementSizeChanging();
			}
		}
		public IElementSizer IElementSizer { get { return IControl as IElementSizer; } }
		public bool IsSizing {
			get { return _IsSizing; }
			protected set {
				if(IsSizing == value)
					return;
				_IsSizing = value;
				OnIsSizingChanged();
			}
		}
		public event EventHandler ElementSizeChanging;
		public event EventHandler IsSizingChanged;
		protected virtual bool IsUsingSizingStep() {
			return IElementSizer.UseSizingStep && !Keyboard2.IsControlPressed;
		}
		protected virtual void OnElementSizeChanging() {
			if(ElementSizeChanging != null)
				ElementSizeChanging(Control, EventArgs.Empty);
		}
		protected virtual void OnIsSizingChanged() {
			if(IsSizingChanged != null)
				IsSizingChanged(Control, EventArgs.Empty);
		}
		protected void UpdateElementSize() {
			if(IElementSizer.UseSizingStep && !double.IsNaN(LastElementSize))
				ElementSize = LastElementSize;
		}
		protected bool IsElementAutoSize {
			get {
				return double.IsNaN(Orientation == Orientation.Horizontal ? IElementSizer.Element.Height : IElementSizer.Element.Width);
			}
		}
		protected double LastElementSize { get; private set; }
		protected Orientation Orientation { get { return IElementSizer.Side.GetOrientation(); } }
		#region Keyboard and Mouse Handling
		protected override void OnMouseDoubleClick(DXMouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
			ElementSize = IElementSizer.CollapseOnDoubleClick && IsElementAutoSize ? 0 : double.NaN;
			e.Handled = true;
		}
		#endregion Keyboard and Mouse Handling
		#region Drag&Drop
		protected override bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			controller = null;
			return IElementSizer.Element != null;
		}
		protected override void StartDragAndDrop(Point p) {
			AbsoluteStartDragPoint = Control.MapPoint(StartDragPoint, null);
			OriginalElementSize = ElementSize;
			OriginalIsElementAutoSize = IsElementAutoSize;
			LastElementSize = double.NaN;
			IsSizing = true;
			base.StartDragAndDrop(p);
		}
		protected override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			ElementSize = OriginalElementSize + GetElementSizeChange(Control.MapPoint(p, null));
		}
		protected override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			if(!accept)
				ElementSize = OriginalIsElementAutoSize ? double.NaN : OriginalElementSize;
			IsSizing = false;
		}
		protected double GetElementSizeChange(Point absoluteDragPoint) {
			double result;
			if (Orientation == Orientation.Horizontal)
				result = absoluteDragPoint.Y - AbsoluteStartDragPoint.Y;
			else {
				result = absoluteDragPoint.X - AbsoluteStartDragPoint.X;
				if (Control.FlowDirection == FlowDirection.RightToLeft)
					result = -result;
			}
			if (IElementSizer.Side == Side.Left || IElementSizer.Side == Side.Top)
				result = -result;
#if !SILVERLIGHT
			if (!BrowserInteropHelper.IsBrowserHosted)
				result = PresentationSource.FromVisual(Control).CompositionTarget.TransformFromDevice.Transform(new Point(result, 0)).X;
#endif
			return result;
		}
		protected override void OnDragAndDropKeyDown(DXKeyEventArgs e) {
			base.OnDragAndDropKeyDown(e);
#if SILVERLIGHT
			if (e.Key == Key.Ctrl)
#else
			if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
#endif
				UpdateElementSize();
		}
		protected override void OnDragAndDropKeyUp(DXKeyEventArgs e) {
			base.OnDragAndDropKeyUp(e);
#if SILVERLIGHT
			if (e.Key == Key.Ctrl)
#else
			if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
#endif
				UpdateElementSize();
		}
		protected Point AbsoluteStartDragPoint { get; private set; }
		protected override bool IsImmediateDragAndDrop { get { return true; } }
		protected double OriginalElementSize { get; private set; }
		protected bool OriginalIsElementAutoSize { get; private set; }
		#endregion Drag&Drop
	}
	public class ElementSizers : ElementPool<ElementSizer> {
		private double _SizingAreaWidth;
		public ElementSizers(PanelBase container)
			: base(container) {
		}
		public ElementSizer Add(FrameworkElement element, Side side) {
			var result = Add();
			result.Element = element;
			result.Side = side;
			result.UpdateBounds();
			return result;
		}
		public double SizingAreaWidth {
			get { return _SizingAreaWidth; }
			set {
				value = Math.Max(0, value);
				if(_SizingAreaWidth != value) {
					_SizingAreaWidth = value;
					foreach(var item in Items)
						item.SizingAreaWidth = SizingAreaWidth;
				}
			}
		}
		protected override ElementSizer CreateItem() {
			var result = base.CreateItem();
			result.SizingAreaWidth = SizingAreaWidth;
			return result;
		}
	}
}
