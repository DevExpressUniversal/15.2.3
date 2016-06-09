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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls.Primitives {
	public interface ILayoutPanelModelBase { }
	public interface ILayoutPanelBase : IScrollControl, ILayoutPanelModelBase {
		FrameworkElement GetMoveableItem(Point p);
		bool AllowItemMoving { get; }
		LayoutPanelProviderBase LayoutProvider { get; }
		Brush MovingItemPlaceHolderBrush { get; }
	}
#if !SILVERLIGHT
#endif
	public abstract class LayoutPanelBase : ScrollControl, ILayoutPanelBase {
		public const double DefaultItemSpacing = 4;
		public const double DefaultPadding = 0;
		#region Dependency Properties
		public static readonly DependencyProperty ItemSpacingProperty =
			DependencyProperty.Register("ItemSpacing", typeof(double), typeof(LayoutPanelBase),
				new PropertyMetadata(DefaultItemSpacing,
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutPanelBase)o;
						control.OnItemSpacingChanged((double)e.OldValue, (double)e.NewValue);
					}));
		public static readonly DependencyProperty PaddingProperty =
			DependencyProperty.Register("Padding", typeof(Thickness), typeof(LayoutPanelBase),
				new PropertyMetadata(new Thickness(DefaultPadding),
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutPanelBase)o;
						control.OnPaddingChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
					}));
		protected static readonly Brush DefaultMovingItemPlaceHolderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
		#endregion Dependency Properties
		internal LayoutPanelBase() {
			LayoutProvider = CreateLayoutProvider();
		}
		protected override void AttachToEvents() {
			Loaded += (sender, e) => OnLoaded();
			SizeChanged += (sender, e) => OnSizeChanged(e);
			Unloaded += (sender, e) => OnUnloaded();
		}
		public new LayoutPanelControllerBase Controller { get { return (LayoutPanelControllerBase)base.Controller; } }
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, Math.Max(0, value)); }
		}
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		#region Children
		protected override Rect GetChildBounds(FrameworkElement child) {
			var result = base.GetChildBounds(child);
			LayoutProvider.UpdateChildBounds(child, ref result);
			return result;
		}
		#endregion Children
		#region Layout
		protected override Size ArrangeOverride(Size finalSize) {
			Size baseSize =  base.ArrangeOverride(finalSize);
			AfterArrange();
			return baseSize;
		}
		protected override Size OnMeasure(Size availableSize) {
			foreach(var child in GetLogicalChildren(false))
				if(!child.GetVisible())
					child.Measure(availableSize);
			return LayoutProvider.Measure(GetLogicalChildren(true), availableSize, CreateLayoutProviderParameters());
		}
		protected override Size OnArrange(Rect bounds) {
			var viewPortBounds = bounds;
			if(Controller.IsScrollable())
				RectHelper.Offset(ref bounds, -HorizontalOffset, -VerticalOffset);
			return LayoutProvider.Arrange(GetLogicalChildren(true), bounds, viewPortBounds, CreateLayoutProviderParameters());
		}
		protected override Thickness GetContentPadding() {
			var result = base.GetContentPadding();
			ThicknessHelper.Inc(ref result, Padding);
			return result;
		}
		protected virtual void AfterArrange() { }
		protected abstract LayoutPanelProviderBase CreateLayoutProvider();
		protected virtual LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new LayoutPanelParametersBase(ItemSpacing);
		}
		protected LayoutPanelProviderBase LayoutProvider { get; private set; }
		#endregion Layout
		protected override PanelControllerBase CreateController() {
			return new LayoutPanelControllerBase(this);
		}
		protected virtual void OnItemSpacingChanged(double oldValue, double newValue) {
			Changed();
		}
		protected virtual void OnPaddingChanged(Thickness oldValue, Thickness newValue) {
			Changed();
		}
		#region ILayoutControlBase
		FrameworkElement ILayoutPanelBase.GetMoveableItem(Point p) {
			return Controller.GetMoveableItem(p);
		}
		bool ILayoutPanelBase.AllowItemMoving { get { return false; } }
		LayoutPanelProviderBase ILayoutPanelBase.LayoutProvider { get { return LayoutProvider; } }
		Brush ILayoutPanelBase.MovingItemPlaceHolderBrush { get { return null; } }
		#endregion ILayoutControlBase
	}
	public class LayoutPanelParametersBase {
		public LayoutPanelParametersBase(double itemSpace) {
			ItemSpace = itemSpace;
		}
		public double ItemSpace { get; set; }
	}
	public abstract class LayoutPanelProviderBase {
		private LayoutPanelParametersBase _Parameters;
		public LayoutPanelProviderBase(ILayoutPanelModelBase model) {
			Model = model;
		}
		public Size Measure(FrameworkElements items, Size maxSize, LayoutPanelParametersBase parameters) {
			Parameters = parameters;
			return OnMeasure(items, maxSize);
		}
		public Size Arrange(FrameworkElements items, Rect bounds, Rect viewPortBounds, LayoutPanelParametersBase parameters) {
			Parameters = parameters;
			return OnArrange(items, bounds, viewPortBounds);
		}
		public virtual void CopyLayoutInfo(FrameworkElement from, FrameworkElement to) {
			to.Width = from.Width;
			to.Height = from.Height;
			to.MinWidth = from.DesiredSize.Width - (from.Margin.Left + from.Margin.Right);
			to.MinHeight = from.DesiredSize.Height - (from.Margin.Top + from.Margin.Bottom);
			to.MaxWidth = from.MaxWidth;
			to.MaxHeight = from.MaxHeight;
			to.Margin = from.Margin;
			to.HorizontalAlignment = from.HorizontalAlignment;
			to.VerticalAlignment = from.VerticalAlignment;
		}
		public virtual void UpdateChildBounds(FrameworkElement child, ref Rect bounds) {
		}
		public virtual void UpdateScrollableAreaBounds(ref Rect bounds) {
		}
		protected virtual Size OnMeasure(FrameworkElements items, Size maxSize) {
			return new Size(0, 0);
		}
		protected virtual Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			return bounds.Size();
		}
		protected virtual void OnParametersChanged() {
		}
		protected ILayoutPanelModelBase Model { get; private set; }
		protected LayoutPanelParametersBase Parameters {
			get { return _Parameters; }
			private set {
				_Parameters = value;
				OnParametersChanged();
			}
		}
	}
	public class LayoutPanelControllerBase : ScrollControlController {
		public LayoutPanelControllerBase(ILayoutPanelBase control)
			: base(control) {
		}
		public ILayoutPanelBase ILayoutControl { get { return IControl as ILayoutPanelBase; } }
		public LayoutPanelProviderBase LayoutProvider { get { return ILayoutControl.LayoutProvider; } }  
		#region Scrolling
		protected override Rect ScrollableAreaBounds {
			get {
				var result = base.ScrollableAreaBounds;
				LayoutProvider.UpdateScrollableAreaBounds(ref result);
				return result;
			}
		}
		#endregion Scrolling
		#region Drag&Drop
		public virtual FrameworkElement GetMoveableItem(Point p) {
			return (FrameworkElement)IPanel.ChildAt(p, true, true, false);
		}
		protected virtual bool CanItemDragAndDrop() {
			return ILayoutControl.AllowItemMoving;
		}
		protected virtual bool CanDragAndDropItem(FrameworkElement item) {
			return true;
		}
		protected virtual DragAndDropController CreateItemDragAndDropController(Point startDragPoint, FrameworkElement dragControl) {
			return null;
		}
		protected override bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			if(WantsItemDragAndDrop(p, point => GetMoveableItem(point), out controller))
				return true;
			return base.WantsDragAndDrop(p, out controller);
		}
		protected bool WantsItemDragAndDrop(Point p, Func<Point, FrameworkElement> getItem, out DragAndDropController controller) {
			if(CanItemDragAndDrop()) {
				FrameworkElement item = getItem(p);
				if(item != null && CanDragAndDropItem(item)) {
					controller = CreateItemDragAndDropController(p, item);
					return true;
				}
			}
			controller = null;
			return false;
		}
		#endregion
	}
	public abstract class LayoutPanelDragAndDropControllerBase : DragAndDropController {
		public static double DragImageOpacity = 0.7;
		public LayoutPanelDragAndDropControllerBase(Controller controller, Point startDragPoint, FrameworkElement dragControl)
			: base(controller, startDragPoint) {
			DragControl = dragControl;
			DragControlParent = (Panel)controller.Control;
			if(IsDragControlVisible) {
				DragControlOrigin = DragControl.GetPosition(Controller.Control);
				DragControlPlaceHolder = CreateDragControlPlaceHolder();
				InitDragControlPlaceHolder();
			}
			else
				DragControlOrigin = PointHelper.Empty;
			DragControlIndex = DragControlParent != null ? DragControlParent.Children.IndexOf(DragControl) : -1;
			if(!PointHelper.IsEmpty(DragControlOrigin))
				StartDragRelativePoint = new Point(
					(StartDragPoint.X - DragControlOrigin.X) / DragControl.ActualWidth,
					(StartDragPoint.Y - DragControlOrigin.Y) / DragControl.ActualHeight);
		}
		public Point StartDragRelativePoint { get; set; }
		protected virtual FrameworkElement CreateDragControlPlaceHolder() {
			return new Canvas();
		}
		protected virtual void InitDragControlPlaceHolder() {
			if(DragControlPlaceHolder is Panel)
				((Panel)DragControlPlaceHolder).Background = Controller.ILayoutControl.MovingItemPlaceHolderBrush;
			Controller.LayoutProvider.CopyLayoutInfo(DragControl, DragControlPlaceHolder);
		}
		protected override bool AllowAutoScrolling { get { return true; } }
		protected new LayoutPanelControllerBase Controller { get { return (LayoutPanelControllerBase)base.Controller; } }
		protected FrameworkElement DragControl { get; private set; }
		protected int DragControlIndex { get; private set; }
		protected Point DragControlOrigin { get; private set; }
		protected Panel DragControlParent { get; private set; }
		protected FrameworkElement DragControlPlaceHolder { get; private set; }
		protected bool IsDragControlVisible { get { return DragControlParent != null && DragControlParent.GetVisible(); } }
		#region Drag Image
		private object _MaxWidthStoredValue;
		private object _MaxHeightStoredValue;
		private object _OpacityStoredValue;
		protected override FrameworkElement CreateDragImage() {
			return DragControl;
		}
		protected override void InitializeDragImage() {
			_MaxWidthStoredValue = DragImage.StorePropertyValue(FrameworkElement.MaxWidthProperty);
			_MaxHeightStoredValue = DragImage.StorePropertyValue(FrameworkElement.MaxHeightProperty);
			_OpacityStoredValue = DragImage.StorePropertyValue(UIElement.OpacityProperty);
			base.InitializeDragImage();
			if(double.IsNaN(DragImage.Width))
				DragImage.MaxWidth = Math.Min(DragImage.MaxWidth, Controller.IPanel.ContentBounds.Width);
			if(double.IsNaN(DragImage.Height))
				DragImage.MaxHeight = Math.Min(DragImage.MaxHeight, Controller.IPanel.ContentBounds.Height);
			DragImage.Opacity = DragImageOpacity;
			if(IsDragControlVisible)
				DragControlParent.Children.Insert(DragControlIndex, DragControlPlaceHolder);
		}
		protected override void FinalizeDragImage() {
			if(IsDragControlVisible)
				DragControlParent.Children.Remove(DragControlPlaceHolder);
			DragImage.RestorePropertyValue(FrameworkElement.MaxWidthProperty, _MaxWidthStoredValue);
			DragImage.RestorePropertyValue(FrameworkElement.MaxHeightProperty, _MaxHeightStoredValue);
			DragImage.RestorePropertyValue(UIElement.OpacityProperty, _OpacityStoredValue);
			base.FinalizeDragImage();
		}
		protected override Point GetDragImageOffset() {
			Size dragImageSize = DragImageSize;
			return new Point(
				-Math.Floor(StartDragRelativePoint.X * dragImageSize.Width) - DragImage.Margin.Left,
				-Math.Floor(StartDragRelativePoint.Y * dragImageSize.Height) - DragImage.Margin.Top);
		}
		protected Size DragImageSize {
			get {
				Size result = DragImage.GetSize();
				if(result == SizeHelper.Zero) {
					result = DragImage.DesiredSize;
					SizeHelper.Deflate(ref result, DragImage.Margin);
				}
				return result;
			}
		}
		#endregion Drag Image
	}
}
