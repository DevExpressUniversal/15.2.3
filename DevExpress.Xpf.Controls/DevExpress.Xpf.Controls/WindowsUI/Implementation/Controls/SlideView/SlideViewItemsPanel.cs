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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class SlideViewItemsPanel : SplitPanel {
#if SILVERLIGHT
		Rect[] cells;
		protected override Rect[] SplitCore(Rect content, ILayoutCell[] layoutCells, double spacing, bool horz) {
			cells = base.SplitCore(content, layoutCells, spacing, horz);
			return cells;
		}
		internal Rect GetChildRect(UIElement child) {
			if(!Children.Contains(child)) return new Rect();
			int cellIndex = Children.IndexOf(child);
			int index = NextLayoutCellIndex(ref cellIndex, child, cells.Length);
			return cells[index];
		}
#endif
	}
	[ContentProperty("Child")]
	public class SlideViewScrollPanel : ScrollControl {
		#region static
		public static readonly DependencyProperty ChildProperty;
		public static readonly DependencyProperty OrientationProperty;
		static SlideViewScrollPanel() {
			var dProp = new DependencyPropertyRegistrator<SlideViewScrollPanel>();
			dProp.Register("Child", ref ChildProperty, (UIElement)null,
				(d, e) => ((SlideViewScrollPanel)d).OnChildChanged((UIElement)e.OldValue, (UIElement)e.NewValue));
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal, (d,e) => ((SlideViewScrollPanel)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue));
		}
		#endregion
		DispatcherTimer timer;
		public SlideViewScrollPanel() {
			timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
			timer.Tick += timer_Tick;
			timer.Start();
			Controller.HorzScrollBar.Opacity = 0;
			MouseMove += SlideViewScrollPanel_MouseMove;
		}
		protected virtual void OnChildChanged(UIElement oldValue, UIElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null) Children.Add(newValue);
		}
		protected virtual LayoutParametersBase CreateLayoutProviderParameters() {
			return new LayoutParametersBase(0d);
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			if (Orientation == Orientation.Horizontal) {
				SetOffset(new Point(VerticalOffset * ScrollAreaSize.Height / ScrollAreaSize.Width, 0));
			} else {
				SetOffset(new Point(0, ScrollAreaSize.Width == 0 ? 0 : HorizontalOffset * ScrollAreaSize.Height / ScrollAreaSize.Width));
			}
			InvalidateMeasure();
		}
		protected override Size OnMeasure(Size availableSize) {
			foreach(var child in GetLogicalChildren(false))
				if(!child.GetVisible())
					child.Measure(availableSize);
			return LayoutProvider.Measure(GetLogicalChildren(true), availableSize, CreateLayoutProviderParameters());
		}
		void timer_Tick(object sender, EventArgs e) {
			HideScrollBars();
			timer.Stop();
		}
		void AnimateOpacity(UIElement control, double from, double to) {
			var animation = new DoubleAnimation { To = to, Duration = new Duration(TimeSpan.FromMilliseconds(500)) };
			var storyboard = new Storyboard();
			Storyboard.SetTarget(storyboard, control);
			Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
			storyboard.Children.Add(animation);
			storyboard.Begin();
		}
		void ShowScrollBars() {
			if(Controller.HorzScrollBar.Visibility == Visibility.Visible && Controller.HorzScrollBar.Opacity == 0)
				AnimateOpacity(Controller.HorzScrollBar, 0, 1);
			if(Controller.VertScrollBar.Visibility == Visibility.Visible && Controller.VertScrollBar.Opacity == 0)
				AnimateOpacity(Controller.VertScrollBar, 0, 1);
		}
		void HideScrollBars() {
			if(Controller.HorzScrollBar.Visibility == Visibility.Visible)
				AnimateOpacity(Controller.HorzScrollBar, 1, 0);
			if(Controller.VertScrollBar.Visibility == Visibility.Visible)
				AnimateOpacity(Controller.VertScrollBar, 1, 0);
		}
		void SlideViewScrollPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			timer.Stop();
			timer.Start();
			ShowScrollBars();
		}
		public Rect ViewportBounds { get; private set; }
		protected override Size OnArrange(Rect bounds) {
			ViewportBounds = bounds;
			if(Controller.IsScrollable())
				RectHelper.Offset(ref bounds, -HorizontalOffset, -VerticalOffset);
			Size layoutProviderArrange = LayoutProvider.Arrange(GetLogicalChildren(true), bounds, ViewportBounds, CreateLayoutProviderParameters()); ;
			if(afterArrangeAction != null) {
				afterArrangeAction();
				afterArrangeAction = null;
			}
			return layoutProviderArrange;
		}
		protected internal void QueueArrangeAction(System.Action action) {
			afterArrangeAction = action;
		}	   
		protected override void OnOffsetChanged(bool isHorizontal, double oldValue, double newValue) {
			base.OnOffsetChanged(isHorizontal, oldValue, newValue);
			QueueArrangeAction(new Action(() => RaiseScrollChanged()));
		}
		void RaiseScrollChanged() {
			var handler = ScrollChanged;
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		public event EventHandler ScrollChanged;
		Action afterArrangeAction;
		public UIElement Child {
			get { return (UIElement)GetValue(ChildProperty); }
			set { SetValue(ChildProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value);}
		}
		private LayoutProviderBase _LayoutProvider;
		public LayoutProviderBase LayoutProvider {
			get {
				if(_LayoutProvider == null) _LayoutProvider = CreateLayoutProvider(); ;
				return _LayoutProvider;
			}
		}
		protected virtual LayoutProviderBase CreateLayoutProvider() {
			return new SlideViewLayoutProvider(this);
		}
		protected override PanelControllerBase CreateController() {
			return new SlideViewScrollPanelController(this);
		}
	}
	public class SlideViewScrollPanelController : ScrollControlController {
		public SlideViewScrollPanelController(SlideViewScrollPanel control) : base(control) {
		}
		public override void GetClientPadding(ref Thickness padding) {
		}
		protected override Rect GetHorzScrollBarBounds() {
			var result = ClientBounds;
			result.Y = result.Bottom - HorzScrollBar.DesiredSize.Height;
			result.Height = HorzScrollBar.DesiredSize.Height;
			return result;
		}
		protected override Rect GetVertScrollBarBounds() {
			var result = ClientBounds;
			result.X = result.Right - VertScrollBar.DesiredSize.Width;
			result.Width = VertScrollBar.DesiredSize.Width;
			return result;
		}
	}
	public class LayoutParametersBase {
		public LayoutParametersBase(double itemSpace) {
			ItemSpace = itemSpace;
		}
		public double ItemSpace { get; set; }
	}
	public abstract class LayoutProviderBase {
		private LayoutParametersBase _Parameters;
		public Size Measure(FrameworkElements items, Size maxSize, LayoutParametersBase parameters) {
			Parameters = parameters;
			Size s = OnMeasure(items, maxSize);
			return s;
		}
		public Size Arrange(FrameworkElements items, Rect bounds, Rect viewPortBounds, LayoutParametersBase parameters) {
			Parameters = parameters;
			return OnArrange(items, bounds, viewPortBounds);
		}
		protected virtual Size OnMeasure(FrameworkElements items, Size maxSize) {
			return new Size(0, 0);
		}
		protected virtual Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			return bounds.Size();
		}
		protected virtual void OnParametersChanged() {
		}
		protected LayoutParametersBase Parameters {
			get { return _Parameters; }
			private set {
				_Parameters = value;
				OnParametersChanged();
			}
		}
	}
	public class SlideViewLayoutProvider : LayoutProviderBase {
		public class SlideViewItemInfo {
			public SlideViewItemInfo(SlideViewLayoutItemSize size, SlideViewLayoutItemPosition position) {
				Position = position;
				Size = size;
			}
			public SlideViewLayoutItemPosition Position { get; private set; }
			public SlideViewLayoutItemSize Size;
		}
		public struct SlideViewLayoutItemPosition {
			public SlideViewLayoutItemPosition(double layerOffset, double itemOffset) {
				LayerOffset = layerOffset;
				ItemOffset = itemOffset;
			}
			public double LayerOffset;
			public double ItemOffset;
		}
		public struct SlideViewLayoutItemSize {
			public SlideViewLayoutItemSize(double width, double length) {
				Width = width;
				Length = length;
			}
			public double Width;
			public double Length;
		}
		public SlideViewLayoutProvider() {
		}
		public SlideViewLayoutProvider(SlideViewScrollPanel panel) {
			Panel = panel;
		}
		protected SlideViewScrollPanel Panel { get; set; }
		protected delegate Size MeasureItem(FrameworkElement item);
		protected delegate void ArrangeItem(FrameworkElement item, SlideViewLayoutItemPosition itemPosition, SlideViewLayoutItemSize itemSize);
		public virtual Orientation Orientation { get {
			return Panel == null ? Orientation.Horizontal : Panel.Orientation;
		} }
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			CalculateLayout(items, bounds,
			delegate(FrameworkElement item) {
				if (Orientation == Orientation.Horizontal)
					return new Size(item.GetDesiredSize().Width, bounds.Height);
				return new Size(bounds.Width, item.GetDesiredSize().Height);
			},
			delegate(FrameworkElement item, SlideViewLayoutItemPosition itemPosition, SlideViewLayoutItemSize itemSize) {
				item.Arrange(GetItemBounds(itemPosition, itemSize));
			});
			return bounds.Size();
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			CalculateLayout(items, RectHelper.New(maxSize),
				delegate(FrameworkElement item) {
					SlideViewLayoutItemSize itemSize;
					if(Orientation == Orientation.Horizontal)
						itemSize = new SlideViewLayoutItemSize(maxSize.Height, double.PositiveInfinity);
					else 
						itemSize = new SlideViewLayoutItemSize(maxSize.Width, double.PositiveInfinity);
					Size availableSize = GetItemSize(itemSize, maxSize);
					item.Measure(availableSize);
					return item.GetDesiredSize();
				},
				null);
			var contentMaxSize = new SlideViewLayoutItemSize(0, 0);
			if(items.Count != 0) {
				for(int i = 0; i < LayerInfos.Count; i++) {
					SlideViewItemInfo layerInfo = LayerInfos[i];
					if (Orientation == Orientation.Horizontal) {
						contentMaxSize.Width += layerInfo.Size.Width;
						contentMaxSize.Length = Math.Max(contentMaxSize.Length, layerInfo.Size.Length);
					} else {
						contentMaxSize.Length += layerInfo.Size.Length;
						contentMaxSize.Width = Math.Max(contentMaxSize.Width, layerInfo.Size.Width);
					}
				}
			}
			var size = GetItemSize(contentMaxSize);
			if (Orientation == Orientation.Horizontal) {
				if(!double.IsInfinity(maxSize.Height))
					size.Height = maxSize.Height;
			} else {
				if(!double.IsInfinity(maxSize.Width))
					size.Width = maxSize.Width;
			}
			return size;
		}
		protected double GetLayerContentStart(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Left : bounds.Top;
		}
		protected double GetLayerStart(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Top : bounds.Left;
		}
		protected Size GetItemSize(SlideViewLayoutItemSize itemSize) {
			return GetItemBounds(new SlideViewLayoutItemPosition(0, 0), itemSize).Size();
		}
		protected virtual Size GetItemSize(SlideViewLayoutItemSize itemSize, Size maxSize) {
			return GetItemSize(itemSize);
		}
		protected void GetItemPositionAndSize(Rect itemBounds, out SlideViewLayoutItemPosition itemPosition, out SlideViewLayoutItemSize itemSize) {
			if(Orientation == Orientation.Horizontal) {
				itemPosition = new SlideViewLayoutItemPosition(itemBounds.Y, itemBounds.X);
				itemSize = new SlideViewLayoutItemSize(itemBounds.Height, itemBounds.Width);
			}
			else {
				itemPosition = new SlideViewLayoutItemPosition(itemBounds.X, itemBounds.Y);
				itemSize = new SlideViewLayoutItemSize(itemBounds.Width, itemBounds.Height);
			}
		}
		protected SlideViewLayoutItemSize GetItemSize(Size itemSize) {
			SlideViewLayoutItemPosition itemPosition;
			SlideViewLayoutItemSize result;
			GetItemPositionAndSize(RectHelper.New(itemSize), out itemPosition, out result);
			return result;
		}
		protected Rect GetItemBounds(SlideViewLayoutItemPosition itemPosition, SlideViewLayoutItemSize itemSize) {
			if(Orientation == Orientation.Horizontal)
				return new Rect(itemPosition.ItemOffset, itemPosition.LayerOffset, itemSize.Length, itemSize.Width);
			else
				return new Rect(itemPosition.LayerOffset, itemPosition.ItemOffset, itemSize.Width, itemSize.Length);
		}
		protected List<SlideViewItemInfo> LayerInfos = new List<SlideViewItemInfo>();
		protected virtual void CalculateLayout(FrameworkElements items, Rect bounds, MeasureItem measureItem, ArrangeItem arrangeItem) {
			double itemStartOffset = GetLayerContentStart(bounds);
			var itemPosition = new SlideViewLayoutItemPosition(GetLayerStart(bounds), itemStartOffset);
			SlideViewItemInfo layerInfo = null;
			LayerInfos.Clear();
			for(int i = 0; i < items.Count; i++) {
				FrameworkElement item = items[i];
				SlideViewLayoutItemSize itemSize = GetItemSize(measureItem(item));
				layerInfo = new SlideViewItemInfo(itemSize, itemPosition);
				LayerInfos.Add(layerInfo);
				if(arrangeItem != null)
					arrangeItem(item, itemPosition, itemSize);
				itemPosition.ItemOffset += itemSize.Length + Parameters.ItemSpace;
			}
		}
	}
}
