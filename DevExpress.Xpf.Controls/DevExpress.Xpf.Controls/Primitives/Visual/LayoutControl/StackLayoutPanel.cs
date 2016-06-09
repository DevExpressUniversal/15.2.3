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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if !SILVERLIGHT
using System.Windows.Threading;
using DevExpress.Xpf.WindowsUI.Base;
#endif
namespace DevExpress.Xpf.Controls.Primitives {
	public interface IStackPanelModel : ILayoutPanelModelBase {
		bool IsHorizontal { get; }
	}
	public interface IStackLayoutPanel : ILayoutPanelBase, IStackPanelModel {
		void OnItemPositionChanged(int oldPosition, int newPosition);
		bool AnimateItemMoving { get; }
		TimeSpan ItemMovingAnimationDuration { get; }
	}
#if !SILVERLIGHT
#endif
	public class StackLayoutPanel : LayoutPanelBase, IStackLayoutPanel {
		public const int DefaultItemMovingAnimationDuration = 200;
		public static double DefaultLayerMinWidth = 20;
		public const double DefaultLayerSpace = 7;
		public static TimeSpan ItemDropAnimationDuration = TimeSpan.FromMilliseconds(500);
		#region Dependency Properties
		public static readonly DependencyProperty AllowItemMovingProperty =
			DependencyProperty.Register("AllowItemMoving", typeof(bool), typeof(StackLayoutPanel),
			new PropertyMetadata(false, (o, e) => ((StackLayoutPanel)o).OnAllowItemMovingChanged()));
		public static readonly DependencyProperty AnimateItemMovingProperty =
			DependencyProperty.Register("AnimateItemMoving", typeof(bool), typeof(StackLayoutPanel), null);
		public static readonly DependencyProperty ItemMovingAnimationDurationProperty =
			DependencyProperty.Register("ItemMovingAnimationDuration", typeof(TimeSpan), typeof(StackLayoutPanel),
				new PropertyMetadata(TimeSpan.FromMilliseconds(DefaultItemMovingAnimationDuration)));
		public static readonly DependencyProperty MovingItemPlaceHolderBrushProperty =
			DependencyProperty.Register("MovingItemPlaceHolderBrush", typeof(Brush), typeof(StackLayoutPanel),
				new PropertyMetadata(DefaultMovingItemPlaceHolderBrush));
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackLayoutPanel),
				new PropertyMetadata(Orientation.Horizontal, (o, e) => ((StackLayoutPanel)o).OnOrientationChanged()));
		public static readonly DependencyProperty AllowItemClippingProperty =
			DependencyProperty.Register("AllowItemClipping", typeof(bool), typeof(StackLayoutPanel),
				new PropertyMetadata(true, (o, e) => ((StackLayoutPanel)o).OnAllowItemClippingChanged()));
		public static readonly DependencyProperty AllowItemAlignmentProperty =
			DependencyProperty.Register("AllowItemAlignment", typeof(bool), typeof(StackLayoutPanel),
				new PropertyMetadata(false, (o, e) => ((StackLayoutPanel)o).OnAllowItemAlignmentChanged()));
		#endregion Dependency Properties
		internal StackLayoutPanel() {
		}
		public bool AllowItemMoving {
			get { return (bool)GetValue(AllowItemMovingProperty); }
			set { SetValue(AllowItemMovingProperty, value); }
		}
		public bool AnimateItemMoving {
			get { return (bool)GetValue(AnimateItemMovingProperty); }
			set { SetValue(AnimateItemMovingProperty, value); }
		}
		public new StackLayoutPanelController Controller { get { return (StackLayoutPanelController)base.Controller; } }
		public TimeSpan ItemMovingAnimationDuration {
			get { return (TimeSpan)GetValue(ItemMovingAnimationDurationProperty); }
			set { SetValue(ItemMovingAnimationDurationProperty, value); }
		}
		public Brush MovingItemPlaceHolderBrush {
			get { return (Brush)GetValue(MovingItemPlaceHolderBrushProperty); }
			set { SetValue(MovingItemPlaceHolderBrushProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool AllowItemClipping {
			get { return (bool)GetValue(AllowItemClippingProperty); }
			set { SetValue(AllowItemClippingProperty, value); }
		}
		public bool AllowItemAlignment {
			get { return (bool)GetValue(AllowItemAlignmentProperty); }
			set { SetValue(AllowItemAlignmentProperty, value); }
		}
		private bool _IsHorizontal = true;
		public bool IsHorizontal {
			get { return _IsHorizontal; }
			private set { _IsHorizontal = value; }
		}
		public event ValueChangedEventHandler<int> ItemPositionChanged;
		#region Children
		protected override IEnumerable<UIElement> GetInternalElements() {
			foreach(UIElement element in BaseGetInternalElements())
				yield return element;
		}
		protected override bool NeedsChildChangeNotifications { get { return true; } }
		private IEnumerable<UIElement> BaseGetInternalElements() {
			return base.GetInternalElements();
		}
		#endregion Children
		#region Layout
		protected override LayoutPanelProviderBase CreateLayoutProvider() {
			return new StackLayoutPanelProvider(this);
		}
		protected override LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new StackLayoutPanelParameters(ItemSpacing, AllowItemClipping, AllowItemAlignment);
		}
		protected Point OffsetBeforeElementMaximization { get; set; }
		#endregion Layout
		protected override PanelControllerBase CreateController() {
			return new StackLayoutPanelController(this);
		}
		protected virtual void OnItemPositionChanged(int oldPosition, int newPosition) {
			if(ItemPositionChanged != null)
				ItemPositionChanged(this, new ValueChangedEventArgs<int>(oldPosition, newPosition));
		}
		protected virtual void OnOrientationChanged() {
			SetOffset(new Point(0, 0));
			Changed();
			IsHorizontal = Orientation == System.Windows.Controls.Orientation.Horizontal;
		}
		protected virtual void OnAllowItemMovingChanged() { }
		protected virtual void OnAllowItemClippingChanged() {
			Changed();
		}
		protected virtual void OnAllowItemAlignmentChanged() {
			Changed();
		}
		#region IStackLayoutPanel
		void IStackLayoutPanel.OnItemPositionChanged(int oldPosition, int newPosition) {
			OnItemPositionChanged(oldPosition, newPosition);
		}
		#endregion IStackLayoutPanel
	}
	public class StackLayoutPanelParameters : LayoutPanelParametersBase {
		public StackLayoutPanelParameters(double itemSpace, bool allowItemClipping, bool allowItemAlignment)
			: base(itemSpace) {
			LayerSpace = 0;
			AllowItemClipping = allowItemClipping;
			AllowItemAlignment = allowItemAlignment;
		}
		public double LayerSpace { get; private set; }
		public bool AllowItemClipping { get; private set; }
		public bool AllowItemAlignment { get; private set; }
	}
	public class StackLayoutPanelProvider : LayoutPanelProviderBase {
		protected new IStackPanelModel Model { get { return (IStackPanelModel)base.Model; } }
		protected new StackLayoutPanelParameters Parameters { get { return (StackLayoutPanelParameters)base.Parameters; } }
		public StackLayoutPanelProvider(IStackPanelModel model)
			: base(model) {
			VisibleChildren = new List<FrameworkElement>();
		}
		protected void SetItemLength(ref Size itemSize, double length) {
			length = Math.Max(0, length);
			if(Model.IsHorizontal)
				itemSize.Width = length;
			else
				itemSize.Height = length;
		}
		protected double GetItemLength(Size itemSize) {
			return Model.IsHorizontal ? itemSize.Width : itemSize.Height;
		}
		protected Size GetItemSize(double itemLength, double itemWidth) {
			var result = new Size();
			SetItemLength(ref result, itemLength);
			SetItemWidth(ref result, itemWidth);
			return result;
		}
		protected void SetItemWidth(ref Size itemSize, double width) {
			if(Model.IsHorizontal)
				itemSize.Height = width;
			else
				itemSize.Width = width;
		}
		protected double GetItemWidth(Size itemSize) {
			return Model.IsHorizontal ? itemSize.Height : itemSize.Width;
		}
		protected Size GetItemMinSize(FrameworkElement item, bool getActualMinSize) {
			return GetItemMinOrMaxSize(item, item.GetMinSize());
		}
		protected Size GetItemMaxSize(FrameworkElement item, bool getActualMaxSize) {
			return GetItemMinOrMaxSize(item, item.GetMaxSize());
		}
		private Size GetItemMinOrMaxSize(FrameworkElement item, Size originalMinOrMaxSize) {
			var result = originalMinOrMaxSize;
			if(!double.IsNaN(item.Width))
				result.Width = item.GetRealWidth();
			if(!double.IsNaN(item.Height))
				result.Height = item.GetRealHeight();
			SizeHelper.Inflate(ref result, item.Margin);
			return result;
		}
		protected Size GetItemDesiredSize(FrameworkElement item) {
			return item.GetDesiredSize();
		}
		protected virtual void UpdateTotalSize(ref double totalLength, ref double totalWidth, Size itemSize) {
			totalLength += GetItemLength(itemSize);
			totalWidth = Math.Max(totalWidth, GetItemWidth(itemSize));
		}
		protected virtual void MeasureItem(FrameworkElement item, Size maxSize) {
			Size minSize = GetItemMinSize(item, false);
			SetItemLength(ref maxSize, Math.Max(GetItemLength(minSize), GetItemLength(maxSize)));
			SetItemWidth(ref maxSize, double.PositiveInfinity);
			item.Measure(maxSize);
		}
		protected void MeasureItem(FrameworkElement item, Size maxSize, ref double totalLength, ref double totalWidth) {
			MeasureItem(item, maxSize);
			UpdateTotalSize(ref totalLength, ref totalWidth, GetItemDesiredSize(item));
		}
		public virtual HorizontalAlignment GetItemHorizontalAlignment(FrameworkElement item) {
			var result = item.HorizontalAlignment;
			if(result == HorizontalAlignment.Stretch && !double.IsNaN(item.Width))
				result = Model.IsHorizontal ? HorizontalAlignment.Left : HorizontalAlignment.Center;
			return result;
		}
		public virtual VerticalAlignment GetItemVerticalAlignment(FrameworkElement item) {
			var result = item.VerticalAlignment;
			if(result == VerticalAlignment.Stretch && !double.IsNaN(item.Height))
				result = Model.IsHorizontal ? VerticalAlignment.Center : VerticalAlignment.Top;
			return result;
		}
		internal ItemAlignment GetItemAlignment(FrameworkElement item) {
			if(!Parameters.AllowItemAlignment) return ItemAlignment.Start;
			return Model.IsHorizontal ? GetItemHorizontalAlignment(item).GetItemAlignment() : GetItemVerticalAlignment(item).GetItemAlignment();
		}
		protected virtual void MeasureStretchedItems(FrameworkElements stretchedItems, double availableLength, double availableWidth,
			ref double totalLength, ref double totalWidth) {
			if(double.IsInfinity(availableLength)) {
				Size itemSize = GetItemSize(availableLength, availableWidth);
				foreach(FrameworkElement item in stretchedItems)
					MeasureItem(item, itemSize, ref totalLength, ref totalWidth);
			}
			else {
				var calculator = new StretchedLengthsCalculator(availableLength);
				foreach(FrameworkElement item in stretchedItems)
					calculator.Add(new StackLayoutPanelItemInfo(0, GetItemLength(GetItemMinSize(item, true)),
												GetItemLength(GetItemMaxSize(item, true)), ItemAlignment.Stretch));
				calculator.Calculate();
				bool useFullLength = false;
				for(int i = 0; i < stretchedItems.Count; i++) {
					MeasureItem(stretchedItems[i], GetItemSize(calculator[i].Length, availableWidth));
					useFullLength = useFullLength || GetItemLength(GetItemDesiredSize(stretchedItems[i])) == calculator[i].Length;
				}
				if(useFullLength && !calculator.NeedsMoreLength) {
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(availableLength, 0));
				}
				else {
					double maxItemLength = 0;
					foreach(FrameworkElement item in stretchedItems)
						maxItemLength = Math.Max(maxItemLength, GetItemLength(GetItemDesiredSize(item)));
					double totalItemLength = 0;
					for(int i = 0; i < stretchedItems.Count; i++) {
						if(calculator[i].Length < maxItemLength && calculator[i].Length != calculator[i].MaxLength)
							MeasureItem(stretchedItems[i], GetItemSize(maxItemLength, availableWidth));
						totalItemLength += Math.Min(maxItemLength, calculator[i].MaxLength);
					}
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(totalItemLength, 0));
				}
				foreach(FrameworkElement item in stretchedItems)
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(0, GetItemWidth(GetItemDesiredSize(item))));
			}
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			LayoutItems = items;
			double itemsSpace = (items.Count - 1) * Parameters.ItemSpace;
			SetItemLength(ref maxSize, Math.Max(0, GetItemLength(maxSize) - itemsSpace));
			double length;
			double width;
			while(true) {
				Size itemMaxSize = maxSize;
				SetItemLength(ref itemMaxSize, double.PositiveInfinity);
				length = 0;
				width = 0;
				var stretchedItems = new FrameworkElements();
				foreach(FrameworkElement item in items)
					if(GetItemAlignment(item) == ItemAlignment.Stretch)
						stretchedItems.Add(item);
					else
						MeasureItem(item, itemMaxSize, ref length, ref width);
				if(stretchedItems.Count != 0)
					MeasureStretchedItems(stretchedItems, Math.Max(0, GetItemLength(maxSize) - length), GetItemWidth(maxSize), ref length, ref width);
				if(width <= GetItemWidth(maxSize))
					break;
				else
					SetItemWidth(ref maxSize, width);
			}
			Size itemSize = GetItemSize(length != 0 ? length + itemsSpace : 0, width);
			return new Size(Math.Min(itemSize.Width, maxSize.Width), Math.Min(itemSize.Height, maxSize.Height));
		}
		StackLayoutPanelItemInfos CreateItemInfos(double availableLength, double itemSpace) {
			return new StackLayoutPanelItemInfos(availableLength, itemSpace);
		}
		protected double GetItemOffset(Rect itemBounds) {
			return Model.IsHorizontal ? itemBounds.X : itemBounds.Y;
		}
		protected void SetItemOffset(ref Point itemLocation, double offset) {
			if(Model.IsHorizontal)
				itemLocation.X = offset;
			else
				itemLocation.Y = offset;
		}
		protected void SetItemOffset(ref Rect itemBounds, double offset) {
			Point itemLocation = itemBounds.Location();
			SetItemOffset(ref itemLocation, offset);
			RectHelper.SetLocation(ref itemBounds, itemLocation);
		}
		protected virtual bool CanArrangeItem(FrameworkElement item) {
			return true;
		}
		protected internal List<FrameworkElement> VisibleChildren { get; private set; }
		internal int HiddenItemsCount { get; private set; }
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			HiddenItemsCount = 0;
			VisibleChildren.Clear();
			LayoutItems = items;
			StackLayoutPanelItemInfos itemInfos = CreateItemInfos(GetItemLength(bounds.Size()), Parameters.ItemSpace);
			foreach(FrameworkElement item in items)
				itemInfos.Add(new StackLayoutPanelItemInfo(GetItemLength(GetItemDesiredSize(item)), GetItemLength(GetItemMinSize(item, true)),
										   GetItemLength(GetItemMaxSize(item, true)), GetItemAlignment(item)));
			itemInfos.Calculate();
			ArrangeInfos = new LayoutPanelArrangeInfos();
			double startOffset = GetItemOffset(bounds);
			for(int i = 0; i < items.Count; i++) {
				var itemLocation = new Point();
				FrameworkElement item = items[i];
				Size itemSize = GetItemDesiredSize(item);
				SetItemOffset(ref itemLocation, startOffset + itemInfos[i].Offset);
				SetItemLength(ref itemSize, itemInfos[i].Length);
				var itemBounds = new Rect(itemLocation, itemSize);
				if(Model.IsHorizontal) {
					RectHelper.AlignVertically(ref itemBounds, bounds, GetItemVerticalAlignment(item));
					if(!CanArrangeItem(item) || (Parameters.AllowItemClipping && itemBounds.Right > (bounds.Width + 0.5))) {
						itemBounds.Width = itemBounds.Height = 0;
						HiddenItemsCount++;
					}
					else VisibleChildren.Add(item);
				}
				else {
					RectHelper.AlignHorizontally(ref itemBounds, bounds, GetItemHorizontalAlignment(item));
					if(Parameters.AllowItemClipping) {
						if(itemBounds.Bottom > bounds.Height) {
							itemBounds.Width = itemBounds.Height = 0;
						}
					}
					else VisibleChildren.Add(item);
				}
				item.Arrange(itemBounds);
				ArrangeInfos.Add(new LayoutPanelArrangeInfo() { ArrangeBounds = itemBounds });
			}
			return bounds.Size();
		}
		public FrameworkElements LayoutItems { get; protected set; }
		protected internal LayoutPanelArrangeInfos ArrangeInfos { get; private set; }
	}
	public class ClippedStackLayoutPanelProvider : LayoutPanelProviderBase {
		protected new IStackPanelModel Model { get { return (IStackPanelModel)base.Model; } }
		protected new StackLayoutPanelParameters Parameters { get { return (StackLayoutPanelParameters)base.Parameters; } }
		public ClippedStackLayoutPanelProvider(IStackPanelModel model)
			: base(model) {
		}
		protected void SetItemLength(ref Size itemSize, double length) {
			length = Math.Max(0, length);
			if(Model.IsHorizontal)
				itemSize.Width = length;
			else
				itemSize.Height = length;
		}
		protected double GetItemLength(Size itemSize) {
			return Model.IsHorizontal ? itemSize.Width : itemSize.Height;
		}
		protected Size GetItemSize(double itemLength, double itemWidth) {
			var result = new Size();
			SetItemLength(ref result, itemLength);
			SetItemWidth(ref result, itemWidth);
			return result;
		}
		protected void SetItemWidth(ref Size itemSize, double width) {
			if(Model.IsHorizontal)
				itemSize.Height = width;
			else
				itemSize.Width = width;
		}
		protected double GetItemWidth(Size itemSize) {
			return Model.IsHorizontal ? itemSize.Height : itemSize.Width;
		}
		protected Size GetItemMinSize(FrameworkElement item, bool getActualMinSize) {
			return GetItemMinOrMaxSize(item, item.GetMinSize());
		}
		protected Size GetItemMaxSize(FrameworkElement item, bool getActualMaxSize) {
			return GetItemMinOrMaxSize(item, item.GetMaxSize());
		}
		private Size GetItemMinOrMaxSize(FrameworkElement item, Size originalMinOrMaxSize) {
			var result = originalMinOrMaxSize;
			if(!double.IsNaN(item.Width))
				result.Width = item.GetRealWidth();
			if(!double.IsNaN(item.Height))
				result.Height = item.GetRealHeight();
			SizeHelper.Inflate(ref result, item.Margin);
			return result;
		}
		protected Size GetItemDesiredSize(FrameworkElement item) {
			return item.GetDesiredSize();
		}
		protected virtual void UpdateTotalSize(ref double totalLength, ref double totalWidth, Size itemSize) {
			totalLength += GetItemLength(itemSize);
			totalWidth = Math.Max(totalWidth, GetItemWidth(itemSize));
		}
		protected virtual void MeasureItem(FrameworkElement item, Size maxSize) {
			Size minSize = GetItemMinSize(item, false);
			SetItemLength(ref maxSize, Math.Max(GetItemLength(minSize), GetItemLength(maxSize)));
			SetItemWidth(ref maxSize, double.PositiveInfinity);
			item.Measure(maxSize);
		}
		protected void MeasureItem(FrameworkElement item, Size maxSize, ref double totalLength, ref double totalWidth) {
			MeasureItem(item, maxSize);
			UpdateTotalSize(ref totalLength, ref totalWidth, GetItemDesiredSize(item));
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			Size bounds = maxSize;
			LayoutItems = items;
			double itemsSpace = (items.Count - 1) * Parameters.ItemSpace;
			SetItemLength(ref maxSize, Math.Max(0, GetItemLength(maxSize) - itemsSpace));
			double length;
			double width;
			int visibleItemsCount = 0;
			while(true) {
				Size itemMaxSize = maxSize;
				SetItemLength(ref itemMaxSize, double.PositiveInfinity);
				length = 0;
				width = 0;
				visibleItemsCount = 0;
				for(int i = 0; i < items.Count; i++) {
					var item = items[i];
					MeasureItem(item, itemMaxSize, ref length, ref width);
					Size desiredSize = GetItemDesiredSize(item);
					if(!CanArrangeItem(item) || Parameters.AllowItemClipping && length + Parameters.ItemSpace * i > bounds.Width) {
						length -= desiredSize.Width;
						break;
					}
					else
						visibleItemsCount++;
				}
				if(width <= GetItemWidth(maxSize))
					break;
				else
					SetItemWidth(ref maxSize, width);
			}
			itemsSpace = (visibleItemsCount - 1) * Parameters.ItemSpace;
			Size itemSize = GetItemSize(length != 0 ? length + itemsSpace : 0, width);
			return new Size(Math.Min((int)(itemSize.Width + 0.5), bounds.Width), Math.Min(itemSize.Height, bounds.Height));
		}
		StackLayoutPanelItemInfos CreateItemInfos(double availableLength, double itemSpace) {
			return new StackLayoutPanelItemInfos(availableLength, itemSpace);
		}
		protected double GetItemOffset(Rect itemBounds) {
			return Model.IsHorizontal ? itemBounds.X : itemBounds.Y;
		}
		protected void SetItemOffset(ref Point itemLocation, double offset) {
			if(Model.IsHorizontal)
				itemLocation.X = offset;
			else
				itemLocation.Y = offset;
		}
		protected void SetItemOffset(ref Rect itemBounds, double offset) {
			Point itemLocation = itemBounds.Location();
			SetItemOffset(ref itemLocation, offset);
			RectHelper.SetLocation(ref itemBounds, itemLocation);
		}
		internal int HiddenItemsCount { get; private set; }
		protected virtual bool CanArrangeItem(FrameworkElement item) {
			return true;
		}
		public virtual HorizontalAlignment GetItemHorizontalAlignment(FrameworkElement item) {
			var result = item.HorizontalAlignment;
			if(result == HorizontalAlignment.Stretch && !double.IsNaN(item.Width))
				result = Model.IsHorizontal ? HorizontalAlignment.Left : HorizontalAlignment.Center;
			return result;
		}
		public virtual VerticalAlignment GetItemVerticalAlignment(FrameworkElement item) {
			var result = item.VerticalAlignment;
			if(result == VerticalAlignment.Stretch && !double.IsNaN(item.Height))
				result = Model.IsHorizontal ? VerticalAlignment.Center : VerticalAlignment.Top;
			return result;
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			LayoutItems = items;
			StackLayoutPanelItemInfos ItemInfos = CreateItemInfos(GetItemLength(bounds.Size()), Parameters.ItemSpace);
			foreach(FrameworkElement item in items)
				ItemInfos.Add(new StackLayoutPanelItemInfo(GetItemLength(GetItemDesiredSize(item)), GetItemLength(GetItemMinSize(item, true)),
										   GetItemLength(GetItemMaxSize(item, true)), ItemAlignment.Start));
			ItemInfos.Calculate();
			ArrangeInfos = new LayoutPanelArrangeInfos();
			double startOffset = GetItemOffset(bounds);
			HiddenItemsCount = 0;
			for(int i = 0; i < items.Count; i++) {
				var itemLocation = new Point();
				Size itemSize = GetItemDesiredSize(items[i]);
				SetItemOffset(ref itemLocation, startOffset + ItemInfos[i].Offset);
				SetItemLength(ref itemSize, ItemInfos[i].Length);
				var itemBounds = new Rect(itemLocation, itemSize);
				if(Model.IsHorizontal) {
					RectHelper.AlignVertically(ref itemBounds, bounds, GetItemVerticalAlignment(items[i]));
					if(!CanArrangeItem(items[i]) || (Parameters.AllowItemClipping && itemBounds.Right > (bounds.Width+0.5))) {
						itemBounds.Width = itemBounds.Height = 0;
						HiddenItemsCount++;
					}
				}
				else {
					RectHelper.AlignHorizontally(ref itemBounds, bounds, GetItemHorizontalAlignment(items[i]));
					if(Parameters.AllowItemClipping) {
						if(itemBounds.Bottom > bounds.Height) {
							itemBounds.Width = itemBounds.Height = 0;
							HiddenItemsCount++;
						}
					}
				}
				items[i].Arrange(itemBounds);
				ArrangeInfos.Add(new LayoutPanelArrangeInfo() { ArrangeBounds = itemBounds });
			}
			return bounds.Size();
		}
		public FrameworkElements LayoutItems { get; protected set; }
		protected internal LayoutPanelArrangeInfos ArrangeInfos { get; private set; }
	}
	public class LayoutPanelArrangeInfo {
		public Rect ArrangeBounds { get; set; }
	}
	public class LayoutPanelArrangeInfos : List<LayoutPanelArrangeInfo> {
	}
	internal enum ItemAlignment { Start, Center, End, Stretch }
	internal static class ItemAlignmentExtensions {
		public static ItemAlignment GetItemAlignment(this HorizontalAlignment alignment) {
			switch(alignment) {
				case HorizontalAlignment.Left:
					return ItemAlignment.Start;
				case HorizontalAlignment.Center:
					return ItemAlignment.Center;
				case HorizontalAlignment.Right:
					return ItemAlignment.End;
				case HorizontalAlignment.Stretch:
					return ItemAlignment.Stretch;
				default:
					return ItemAlignment.Start;
			}
		}
		public static ItemAlignment GetItemAlignment(this VerticalAlignment alignment) {
			switch(alignment) {
				case VerticalAlignment.Top:
					return ItemAlignment.Start;
				case VerticalAlignment.Center:
					return ItemAlignment.Center;
				case VerticalAlignment.Bottom:
					return ItemAlignment.End;
				case VerticalAlignment.Stretch:
					return ItemAlignment.Stretch;
				default:
					return ItemAlignment.Start;
			}
		}
		public static HorizontalAlignment GetHorizontalAlignment(this ItemAlignment alignment) {
			switch(alignment) {
				case ItemAlignment.Start:
					return HorizontalAlignment.Left;
				case ItemAlignment.Center:
					return HorizontalAlignment.Center;
				case ItemAlignment.End:
					return HorizontalAlignment.Right;
				case ItemAlignment.Stretch:
					return HorizontalAlignment.Stretch;
				default:
					return HorizontalAlignment.Left;
			}
		}
		public static VerticalAlignment GetVerticalAlignment(this ItemAlignment alignment) {
			switch(alignment) {
				case ItemAlignment.Start:
					return VerticalAlignment.Top;
				case ItemAlignment.Center:
					return VerticalAlignment.Center;
				case ItemAlignment.End:
					return VerticalAlignment.Bottom;
				case ItemAlignment.Stretch:
					return VerticalAlignment.Stretch;
				default:
					return VerticalAlignment.Top;
			}
		}
	}
	internal class StackLayoutPanelItemInfo {
		public StackLayoutPanelItemInfo(double length, double minLength, double maxLength, ItemAlignment alignment) {
			MinLength = minLength;
			MaxLength = maxLength;
			Length = length;
			Alignment = alignment;
		}
		public ItemAlignment Alignment { get; private set; }
		public bool IsProcessed { get; set; }
		public double Length { get; set; }
		public double MinLength { get; set; }
		public double MaxLength { get; private set; }
		public double Offset = double.NaN;
	}
	internal class StackLayoutPanelItemInfos : List<StackLayoutPanelItemInfo> {
		public StackLayoutPanelItemInfos(double availableLength, double itemSpace) {
			AvailableLength = availableLength;
			ItemSpace = itemSpace;
		}
		public virtual void ArrangeItems(FrameworkElements items, Func<FrameworkElement, ItemAlignment> getItemAlignment) {
			var centerAlignedItemCount = 0;
			var endAlignedItemCount = 0;
			var i = 0;
			FrameworkElement item;
			while(i < items.Count - (centerAlignedItemCount + endAlignedItemCount)) {
				item = items[i];
				switch(getItemAlignment(item)) {
					case ItemAlignment.Center:
						items.RemoveAt(i);
						items.Insert(items.Count - endAlignedItemCount, item);
						centerAlignedItemCount++;
						break;
					case ItemAlignment.End:
						items.RemoveAt(i);
						items.Insert(items.Count, item);
						endAlignedItemCount++;
						break;
					default:
						i++;
						break;
				}
			}
		}
		public virtual void Calculate() {
			CalculateStretchedLengths();
			CalculateOffsets();
		}
		public double AvailableLength { get; private set; }
		public double ItemSpace { get; private set; }
		protected virtual void CalculateOffsets() {
			var startOffset = CalculateOffsets(0, true,
				itemInfo => itemInfo.Alignment == ItemAlignment.Start || itemInfo.Alignment == ItemAlignment.Stretch);
			var endOffset = CalculateOffsets(Math.Max(Length, AvailableLength), false,
				itemInfo => itemInfo.Alignment == ItemAlignment.End);
			CalculateOffsets(Math.Round((startOffset + endOffset - CenteredLength) / 2), true,
				itemInfo => itemInfo.Alignment == ItemAlignment.Center);
		}
		protected virtual double CalculateOffsets(double offset, bool forward, Predicate<StackLayoutPanelItemInfo> isItemForProcessing) {
			StackLayoutPanelItemInfo itemInfo;
			var i = forward ? 0 : Count - 1;
			while(forward && i < Count || !forward && i >= 0) {
				itemInfo = this[i];
				if(isItemForProcessing(itemInfo)) {
					if(!forward)
						offset -= itemInfo.Length;
					itemInfo.Offset = offset;
					if(forward)
						offset += itemInfo.Length + ItemSpace;
					else
						offset -= ItemSpace;
				}
				if(forward)
					i++;
				else
					i--;
			}
			return offset;
		}
		protected virtual void CalculateStretchedLengths() {
			var stretchedItems = GetStretchedItems();
			if(stretchedItems.Count == 0)
				return;
			var calculator = new StretchedLengthsCalculator(
				Math.Max(0, AvailableLength - FixedLength - (stretchedItems.Count - 1) * ItemSpace));
			calculator.AddRange(stretchedItems);
			if(calculator.CanFitItemsUniformly())
				calculator.ForEach(
					delegate(StackLayoutPanelItemInfo item) {
						item.MinLength = item.Length;
						item.Length = 0;
					});
			calculator.Calculate();
		}
		protected virtual double GetLength(ItemAlignment? alignment) {
			double result = 0;
			foreach(var itemInfo in this)
				if(alignment == null || itemInfo.Alignment == alignment)
					result += itemInfo.Length + ItemSpace;
			if(result != 0)
				result -= ItemSpace;
			return result;
		}
		protected List<StackLayoutPanelItemInfo> GetStretchedItems() {
			var result = new List<StackLayoutPanelItemInfo>();
			foreach(var itemInfo in this)
				if(itemInfo.Alignment == ItemAlignment.Stretch)
					result.Add(itemInfo);
			return result;
		}
		protected double CenteredLength { get { return GetLength(ItemAlignment.Center); } }
		protected double FixedLength { get { return Length - GetLength(ItemAlignment.Stretch); } }
		protected double Length { get { return GetLength(null); } }
	}
	internal class StretchedLengthsCalculator : List<StackLayoutPanelItemInfo> {
		public StretchedLengthsCalculator(double availableLength) {
			AvailableLength = availableLength;
		}
		public void Calculate() {
			if(Count == 0)
				return;
			var availableLength = double.IsInfinity(AvailableLength) ? ItemsLength : AvailableLength;
			do {
				var itemsLength = ItemsLength;
				double offset = 0, stretchedOffset, prevStretchedOffset = 0;
				double neededLength = 0, extraLength = 0;
				for(int i = 0; i < Count; i++) {
					StackLayoutPanelItemInfo itemInfo = this[i];
					if(!itemInfo.IsProcessed) {
						if(itemsLength != 0)
							stretchedOffset = Math.Round(availableLength * (offset + itemInfo.Length) / itemsLength);
						else
							stretchedOffset = GetUniformStretchedOffset(i + 1);
						offset += itemInfo.Length;
						itemInfo.Length = stretchedOffset - prevStretchedOffset;
						prevStretchedOffset += itemInfo.Length;
						if(itemInfo.Length < itemInfo.MinLength)
							neededLength += itemInfo.MinLength - itemInfo.Length;
						if(itemInfo.Length > itemInfo.MaxLength)
							extraLength += itemInfo.Length - itemInfo.MaxLength;
					}
				}
				if(neededLength == 0 && extraLength == 0)
					break;
				NeedsMoreLength = neededLength != 0;
				ProcessMinOrMaxLengthItems(neededLength > extraLength, ref availableLength);
			}
			while(true);
		}
		public bool CanFitItemsUniformly() {
			return ItemsLength != AvailableLength;
		}
		public double AvailableLength { get; private set; }
		public bool NeedsMoreLength { get; private set; }
		protected double GetUniformStretchedOffset(int index) {
			if(Count == 1)
				return AvailableLength * index;
			else
				return Math.Round(AvailableLength * index / Count);
		}
		protected void ProcessMinOrMaxLengthItems(bool processMinLengthItems, ref double availableLength) {
			foreach(var itemInfo in this)
				if(!itemInfo.IsProcessed &&
					(processMinLengthItems && itemInfo.Length < itemInfo.MinLength ||
					 !processMinLengthItems && itemInfo.Length > itemInfo.MaxLength)) {
					if(processMinLengthItems)
						itemInfo.Length = itemInfo.MinLength;
					else
						itemInfo.Length = itemInfo.MaxLength;
					itemInfo.IsProcessed = true;
					availableLength -= itemInfo.Length;
				}
			availableLength = Math.Max(0, availableLength);
		}
		protected double ItemsLength {
			get {
				double result = 0;
				foreach(var itemInfo in this)
					if(!itemInfo.IsProcessed)
						result += itemInfo.Length;
				return result;
			}
		}
	}
	public class StackLayoutPanelController : LayoutPanelControllerBase {
		public StackLayoutPanelController(IStackLayoutPanel control) : base(control) { }
		public new IStackLayoutPanel ILayoutControl { get { return base.ILayoutControl as IStackLayoutPanel; } }
		public new StackLayoutPanelProvider LayoutProvider { get { return (StackLayoutPanelProvider)base.LayoutProvider; } }	
		public override bool IsScrollable() {
			return false;
		}
		#region Drag&Drop
		protected override DragAndDropController CreateItemDragAndDropController(Point startDragPoint, FrameworkElement dragControl) {
			return new StackLayoutPanelDragAndDropControllerBase(this, startDragPoint, dragControl);
		}
		#endregion Drag&Drop
	}
	public class StackLayoutPanelDragAndDropControllerBase : LayoutPanelDragAndDropControllerBase {
		public StackLayoutPanelDragAndDropControllerBase(Controller controller, Point startDragPoint, FrameworkElement dragControl)
			: base(controller, startDragPoint, dragControl) {
			if(ElementPositionsAnimation != null) {
				ElementPositionsAnimation.Stop();
				ElementPositionsAnimation = null;
			}
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			if(ElementPositionsAnimation != null && ElementPositionsAnimation.IsActive)
				return;
			var child = Controller.IPanel.ChildAt(p, true, true, true);
		}
		protected static ElementBoundsAnimation ElementPositionsAnimation { get; set; }
		protected override Core.Container CreateDragImageContainer() {
			return new NonLogicalContainer();
		}
		protected override Panel GetDragImageParent() {
			return Controller.IPanel as Panel ?? base.GetDragImageParent();
		}
		protected new StackLayoutPanelController Controller { get { return (StackLayoutPanelController)base.Controller; } }
		class NonLogicalContainer : DevExpress.Xpf.Core.Container {
			protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
				return base.CreateUIElementCollection(null);
			}
		}
	}
}
