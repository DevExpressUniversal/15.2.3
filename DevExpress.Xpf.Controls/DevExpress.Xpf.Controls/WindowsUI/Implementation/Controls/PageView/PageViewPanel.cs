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

using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class PageViewStackPanel : StackLayoutPanel, IScrollablePanel {
		#region static
		public static readonly DependencyProperty LayoutTypeProperty;
		public static readonly DependencyProperty IsScrollableProperty;
		static readonly DependencyPropertyKey IsScrollablePropertyKey;
		static PageViewStackPanel() {
			Type ownerType = typeof(PageViewStackPanel);
			AllowItemAlignmentProperty.OverrideMetadata(ownerType, new PropertyMetadata(true));
			AllowItemClippingProperty.OverrideMetadata(ownerType, new PropertyMetadata(false));
			LayoutTypeProperty = DependencyProperty.Register("LayoutType", typeof(PageHeadersLayoutType), ownerType, new PropertyMetadata(PageHeadersLayoutType.Default, OnLayoutTypeChanged));
			IsScrollablePropertyKey = DependencyProperty.RegisterReadOnly("IsScrollable", typeof(bool), ownerType, new PropertyMetadata(false));
			IsScrollableProperty = IsScrollablePropertyKey.DependencyProperty;
		}
		private static void OnLayoutTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PageViewStackPanel)d).OnIsLayoutTypeChanged((PageHeadersLayoutType)e.OldValue, (PageHeadersLayoutType)e.NewValue);
		}
		#endregion
		public PageViewStackPanel() {
		}
		protected override System.Windows.Media.Geometry GetGeometry() {
			var result = RectHelper.New(this.GetSize());
			Thickness clipPadding = IsHorizontal ? new Thickness(ContentPadding.Left, 0, ContentPadding.Right, 0) : new Thickness(0, ContentPadding.Top, 0, ContentPadding.Bottom);
			RectHelper.Deflate(ref result, clipPadding);
			return new RectangleGeometry { Rect = result };
		}
		protected override void OnPaddingChanged(Thickness oldValue, Thickness newValue) {
			base.OnPaddingChanged(oldValue, newValue);
			UpdateClip();
		}
		protected virtual void OnIsLayoutTypeChanged(PageHeadersLayoutType oldValue, PageHeadersLayoutType newValue) {
			IsScrollable = newValue == PageHeadersLayoutType.Scroll;
			Controller.EnsureScrolling();
			Changed();
		}
		protected override void AfterArrange() {
			base.AfterArrange();
			if(ScrollableControl != null) ScrollableControl.InvalidateScroll();
		}
		private double CalculateForwardScrollOffset() {
			bool isHorz = IsHorizontal;
			double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
			double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
			double offset = isHorz ? HorizontalOffset : VerticalOffset;
			double extent = scrollSize - offset;
			Point hitPoint = isHorz ? new Point(offset + viewPort + ChildrenBounds.X, Size.Height / 2) : new Point(Size.Width / 2, offset + viewPort + ChildrenBounds.Y);
			FrameworkElement child = this.ChildAt(hitPoint, true, true, true) as FrameworkElement;
			if(child != null) {
				return isHorz ? this.GetChildBounds(child).X - ChildrenBounds.X : this.GetChildBounds(child).Y - ChildrenBounds.Y;
			}
			return offset + viewPort;
		}
		private double CalculateBackwardScrollOffset() {
			bool isHorz = IsHorizontal;
			double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
			double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
			double offset = isHorz ? HorizontalOffset : VerticalOffset;
			double extent = scrollSize - offset;
			Point hitPoint = isHorz ? new Point(0, Size.Height / 2) : new Point(Size.Width / 2, 0);
			FrameworkElement child = this.ChildAt(hitPoint, true, true, true) as FrameworkElement;
			if(child != null) {
				Rect childBounds = this.GetChildBounds(child);
				return isHorz ? offset + childBounds.X - ContentBounds.X - viewPort + childBounds.Width :
					offset + childBounds.Y - ContentBounds.Y - viewPort + childBounds.Height;
			}
			return offset - viewPort;
		}
		protected override PanelControllerBase CreateController() {
			return new PageViewStackPanelController(this);
		}
		protected override LayoutPanelProviderBase CreateLayoutProvider() {
			return new PageViewStackPanelProvider(this);
		}
		protected override LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new PageViewStackPanelParameters(LayoutType, ItemSpacing, AllowItemClipping, AllowItemAlignment);
		}
		public new PageViewStackPanelController Controller {
			get { return (PageViewStackPanelController)base.Controller; }
		}
		protected override bool IsClipped {
			get { return base.IsClipped || ClipToBounds; }
		}
		public bool IsScrollable {
			get { return (bool)GetValue(IsScrollableProperty); }
			private set { SetValue(IsScrollablePropertyKey, value); }
		}
		public PageHeadersLayoutType LayoutType {
			get { return (PageHeadersLayoutType)GetValue(LayoutTypeProperty); }
			set { SetValue(LayoutTypeProperty, value); }
		}
		#region IScrollablePanel Members
		ScrollableControl ScrollableControl;
		ScrollableControl IScrollablePanel.ScrollOwner {
			get { return ScrollableControl; }
			set { ScrollableControl = value; }
		}
		bool IScrollablePanel.CanScrollPrev {
			get { return IsScrollable && (IsHorizontal ? HorizontalOffset : VerticalOffset) > 0; }
		}
		bool IScrollablePanel.CanScrollNext {
			get {
				bool isHorz = IsHorizontal;
				double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
				double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
				double offset = isHorz ? HorizontalOffset : VerticalOffset;
				return IsScrollable &&  scrollSize - offset > viewPort;
			}
		}
		void IScrollablePanel.ScrollNext() {
			Controller.Scroll(Orientation, CalculateForwardScrollOffset(), true);
		}
		void IScrollablePanel.ScrollPrev() {
			Controller.Scroll(Orientation, CalculateBackwardScrollOffset(), true);
		}
		bool IScrollablePanel.BringChildIntoView(FrameworkElement child) {
			return BringChildIntoView(child, true);
		}
		Orientation IScrollablePanel.Orientation {
			get { return Orientation; }
			set { Orientation = value; }
		}
		#endregion
		class PageViewStackPanelProvider : ClippedStackLayoutPanelProvider {
			public PageViewStackPanelProvider(IStackPanelModel model)
				: base(model) {
			}
			public override HorizontalAlignment GetItemHorizontalAlignment(FrameworkElement item) {
					return Model.IsHorizontal ? HorizontalAlignment.Right : HorizontalAlignment.Stretch;
			}
			protected new PageViewStackPanelParameters Parameters { get { return (PageViewStackPanelParameters)base.Parameters; } }
			ILayoutCalculatorResult measureResult;
			protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
				if(IsScrollable || measureResult == null) return base.OnArrange(items, bounds, viewPortBounds);
				for(int i = 0; i < items.Count; i++) {
					items[i].Arrange(measureResult.ItemRects[i]);
				}
				return bounds.Size;
			}
			bool IsScrollable { get { return Parameters.LayoutType == PageHeadersLayoutType.Scroll; } }
			protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
				measureResult = null;
				var layoutType = Parameters.LayoutType;
				Size baseSize = base.OnMeasure(items, maxSize);
				if(IsScrollable) return baseSize;
				IItemHeaderInfo[] headers = new IItemHeaderInfo[items.Count];
				for(int i = 0; i < items.Count; i++) {
					ISelectorItem selectorItem = items[i] as ISelectorItem;
					headers[i] = new BaseItemOptions(items[i].DesiredSize, selectorItem != null ? selectorItem.IsSelected : false);
				}
				ILayoutCalculator calculator = LayoutCalculatorFactory.Resolve(Model.IsHorizontal, Parameters.LayoutType);
				measureResult = calculator.Measure(new BaseMeasureOptions(maxSize, headers, Parameters.ItemSpace));
				return measureResult.TotalSize;
			}
		}
		public class PageViewStackPanelController : StackLayoutPanelController {
			public PageViewStackPanelController(PageViewStackPanel control)
				: base(control) {
				ScrollBars = Core.ScrollBars.None;
			}
			public override bool IsScrollable() {
				return Control.IsScrollable;
			}
			protected override void CheckScrollBars() {
			}
			public void EnsureScrolling() {
				CheckScrollParams();
			}
			public new PageViewStackPanel Control { get { return (PageViewStackPanel)base.Control; } }
		}
		class PageViewStackPanelParameters : StackLayoutPanelParameters {
			public PageViewStackPanelParameters(PageHeadersLayoutType layoutType, double itemSpace, bool allowItemClipping, bool allowItemAlignment)
				: base(itemSpace, allowItemClipping, allowItemAlignment) {
					LayoutType = layoutType;
			}
			public PageHeadersLayoutType LayoutType { get; private set; }
		}
	}
	public class PageViewPanel : SplitPanel {
		static PageViewPanel() {
			var dProp = new DependencyPropertyRegistrator<PageViewPanel>();
			dProp.OverrideMetadata(ItemSizeModeProperty, ItemSizeMode.AutoSize);
		}
		public PageViewPanel() {
#if SILVERLIGHT
			ItemSizeMode = ItemSizeMode.AutoSize;
#endif
		}
		ILayoutCalculator _layoutCalculatorCore;
		ILayoutCalculator layoutCalculatorCore {
			get {
				ClipLayoutCalculator c = _layoutCalculatorCore as ClipLayoutCalculator;
				if(c != null) {
					c.IsHorizontal = Orientation == System.Windows.Controls.Orientation.Horizontal;
				}
				return _layoutCalculatorCore;
			}
			set {
				_layoutCalculatorCore = value;
			}
		}
		ILayoutCalculatorResult measureResult;
		protected virtual ILayoutCalculator GetDefaultLayoutCalculator() {
			return new ClipLayoutCalculator() { IsHorizontal = Orientation == System.Windows.Controls.Orientation.Horizontal };
		}
		protected ILayoutCalculator LayoutCalculator {
			get {
				if(layoutCalculatorCore == null) {
					layoutCalculatorCore = GetDefaultLayoutCalculator();
				}
				return layoutCalculatorCore;
			}
		}
		protected override Size OnMeasure(Size availableSize) {
			base.OnMeasure(availableSize);
			IItemHeaderInfo[] headers = new IItemHeaderInfo[Children.Count];
			for(int i = 0; i < Children.Count; i++) {
				if(Children[i] is ISelectorItem) {
					headers[i] = new BaseItemOptions(Children[i].DesiredSize, ((ISelectorItem)Children[i]).IsSelected);
				} else {
					headers[i] = new BaseItemOptions(new Size(), false);
				}
			}
			measureResult = LayoutCalculator.Measure(new BaseMeasureOptions(availableSize, headers, ItemSpacing));
			return measureResult.TotalSize;
		}
		protected override System.Windows.Size OnArrange(System.Windows.Size finalSize) {
			for(int i = 0; i < Children.Count; i++) {
				Children[i].Arrange(measureResult.ItemRects[i]);
			}
			return finalSize;
		}
	}
}
