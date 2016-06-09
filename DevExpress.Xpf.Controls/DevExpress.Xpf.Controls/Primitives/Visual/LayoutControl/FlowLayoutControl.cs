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
#endif
namespace DevExpress.Xpf.Controls.Primitives {
	public interface IFlowPanelModel : ILayoutPanelModelBase {
		Orientation Orientation { get; }
	}
	public interface IFlowLayoutPanel : ILayoutPanelBase, IFlowPanelModel {
		void OnItemPositionChanged(int oldPosition, int newPosition);
		bool AnimateItemMoving { get; }
		TimeSpan ItemMovingAnimationDuration { get; }
		double LayerMinWidth { get; }
		double LayerMaxWidth { get; }
		double LayerWidth { get; set; }
	}
#if !SILVERLIGHT
#endif
	public class FlowLayoutPanel : LayoutPanelBase, IFlowLayoutPanel {
		public const int DefaultItemMovingAnimationDuration = 200;
		public static double DefaultLayerMinWidth = 20;
		public const double DefaultLayerSpace = 7;
		public static TimeSpan ItemDropAnimationDuration = TimeSpan.FromMilliseconds(500);
		#region Dependency Properties
		public static readonly DependencyProperty AllowItemClippingProperty =
			DependencyProperty.Register("AllowItemClipping", typeof(bool), typeof(FlowLayoutPanel),
				new PropertyMetadata(true, (o, e) => ((FlowLayoutPanel)o).OnAllowItemClippingChanged()));
		public static readonly DependencyProperty AllowItemAlignmentProperty =
			DependencyProperty.Register("AllowItemAlignment", typeof(bool), typeof(FlowLayoutPanel),
				new PropertyMetadata(false, (o, e) => ((FlowLayoutPanel)o).OnAllowItemAlignmentChanged()));
		public static readonly DependencyProperty AllowItemMovingProperty =
			DependencyProperty.Register("AllowItemMoving", typeof(bool), typeof(FlowLayoutPanel),
			new PropertyMetadata(true, (o, e) => ((FlowLayoutPanel)o).OnAllowItemMovingChanged()));
		public static readonly DependencyProperty AnimateItemMovingProperty =
			DependencyProperty.Register("AnimateItemMoving", typeof(bool), typeof(FlowLayoutPanel), null);
		public static readonly DependencyProperty IsFlowBreakProperty =
			DependencyProperty.RegisterAttached("IsFlowBreak", typeof(bool), typeof(FlowLayoutPanel), new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty ItemMovingAnimationDurationProperty =
			DependencyProperty.Register("ItemMovingAnimationDuration", typeof(TimeSpan), typeof(FlowLayoutPanel),
				new PropertyMetadata(TimeSpan.FromMilliseconds(DefaultItemMovingAnimationDuration)));
		public static readonly DependencyProperty LayerSpaceProperty =
			DependencyProperty.Register("LayerSpace", typeof(double), typeof(FlowLayoutPanel),
				new PropertyMetadata(DefaultLayerSpace,
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (FlowLayoutPanel)o;
						control.OnLayerSpaceChanged((double)e.OldValue, (double)e.NewValue);
					}));
		public static readonly DependencyProperty MovingItemPlaceHolderBrushProperty =
			DependencyProperty.Register("MovingItemPlaceHolderBrush", typeof(Brush), typeof(FlowLayoutPanel),
				new PropertyMetadata(DefaultMovingItemPlaceHolderBrush));
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowLayoutPanel),
				new PropertyMetadata(Orientation.Horizontal, (o, e) => ((FlowLayoutPanel)o).OnOrientationChanged()));
		public static readonly DependencyProperty StretchContentProperty =
			DependencyProperty.Register("StretchContent", typeof(bool), typeof(FlowLayoutPanel),
				new PropertyMetadata((o, e) => ((FlowLayoutPanel)o).OnStretchContentChanged()));
		public static bool GetIsFlowBreak(UIElement element) {
			return (bool)element.GetValue(IsFlowBreakProperty);
		}
		public static void SetIsFlowBreak(UIElement element, bool value) {
			element.SetValue(IsFlowBreakProperty, value);
		}
		#endregion Dependency Properties
		internal FlowLayoutPanel() {
		}
		public bool AllowItemClipping {
			get { return (bool)GetValue(AllowItemClippingProperty); }
			set { SetValue(AllowItemClippingProperty, value); }
		}
		public bool AllowItemAlignment {
			get { return (bool)GetValue(AllowItemAlignmentProperty); }
			set { SetValue(AllowItemAlignmentProperty, value); }
		}
		public bool AllowItemMoving {
			get { return (bool)GetValue(AllowItemMovingProperty); }
			set { SetValue(AllowItemMovingProperty, value); }
		}
		public bool AnimateItemMoving {
			get { return (bool)GetValue(AnimateItemMovingProperty); }
			set { SetValue(AnimateItemMovingProperty, value); }
		}
		public new FlowLayoutPanelController Controller { get { return (FlowLayoutPanelController)base.Controller; } }
		public TimeSpan ItemMovingAnimationDuration {
			get { return (TimeSpan)GetValue(ItemMovingAnimationDurationProperty); }
			set { SetValue(ItemMovingAnimationDurationProperty, value); }
		}
		public double LayerMinWidth {
			get { return Math.Max(DefaultLayerMinWidth, LayoutProvider.GetLayerMinWidth(ChildrenMinSize)); }
		}
		public double LayerMaxWidth {
			get { return LayoutProvider.GetLayerMaxWidth(ChildrenMaxSize); }
		}
		public double LayerSpace {
			get { return (double)GetValue(LayerSpaceProperty); }
			set { SetValue(LayerSpaceProperty, Math.Max(0, value)); }
		}
		public double LayerWidth {
			get {
				Size itemSize;
				return LayoutProvider.GetLayerWidth(GetLogicalChildren(false), out itemSize);
			}
			set {
				value = Math.Max(LayerMinWidth, Math.Min(value, LayerMaxWidth));
				var items = GetLogicalChildren(false);
				Size prevSize, size;
				if(LayoutProvider.GetLayerWidth(items, out prevSize) == value)
					return;
				LayoutProvider.SetLayerWidth(items, value, out size);
				if(ItemsSizeChanged != null)
					ItemsSizeChanged(this, new ValueChangedEventArgs<Size>(prevSize, size));
			}
		}
		public Brush MovingItemPlaceHolderBrush {
			get { return (Brush)GetValue(MovingItemPlaceHolderBrushProperty); }
			set { SetValue(MovingItemPlaceHolderBrushProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool StretchContent {
			get { return (bool)GetValue(StretchContentProperty); }
			set { SetValue(StretchContentProperty, value); }
		}
		public event ValueChangedEventHandler<int> ItemPositionChanged;
		public event ValueChangedEventHandler<Size> ItemsSizeChanged;
		#region Children
		protected virtual FrameworkElement CreateItem() {
			return new ContentPresenter();
		}
		protected override IEnumerable<UIElement> GetInternalElements() {
			foreach(UIElement element in BaseGetInternalElements())
				yield return element;
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(ContentPresenter.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected override bool NeedsChildChangeNotifications { get { return true; } }
		private IEnumerable<UIElement> BaseGetInternalElements() {
			return base.GetInternalElements();
		}
		#endregion Children
		#region Layout
		protected override LayoutPanelProviderBase CreateLayoutProvider() {
			return new FlowLayoutPanelProvider(this);
		}
		protected override LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new FlowLayoutPanelParameters(ItemSpacing, LayerSpace, StretchContent, AllowItemClipping, AllowItemAlignment);
		}
		protected new FlowLayoutPanelProvider LayoutProvider { get { return (FlowLayoutPanelProvider)base.LayoutProvider; } }
		#endregion Layout
		protected override PanelControllerBase CreateController() {
			return new FlowLayoutPanelController(this);
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if(property == IsFlowBreakProperty)
				OnIsFlowBreakChanged(child);
		}
		protected virtual void OnBreakFlowToFitChanged() {
			SetOffset(new Point(0, 0));
			Changed();
		}
		protected virtual void OnIsFlowBreakChanged(FrameworkElement child) {
			Changed();
		}
		protected virtual void OnLayerSpaceChanged(double oldValue, double newValue) {
			Changed();
		}
		protected virtual void OnOrientationChanged() {
			SetOffset(new Point(0, 0));
			Changed();
		}
		protected virtual void OnAllowItemMovingChanged() { }
		protected virtual void OnStretchContentChanged() {
			Changed();
		}
		protected virtual void OnAllowItemClippingChanged() {
			Changed();
		}
		protected virtual void OnAllowItemAlignmentChanged() {
			Changed();
		}
		protected virtual void OnItemPositionChanged(int oldPosition, int newPosition) {
			if(ItemPositionChanged != null)
				ItemPositionChanged(this, new ValueChangedEventArgs<int>(oldPosition, newPosition));
		}
		#region IFlowLayoutControl
		void IFlowLayoutPanel.OnItemPositionChanged(int oldPosition, int newPosition) {
			OnItemPositionChanged(oldPosition, newPosition);
		}
		#endregion IFlowLayoutControl
	}
	public class FlowLayoutPanelParameters : LayoutPanelParametersBase {
		public FlowLayoutPanelParameters(double itemSpace, double layerSpace, bool stretchContent, bool allowItemClipping, bool allowItemAlignment)
			: base(itemSpace) {
			LayerSpace = layerSpace;
			StretchContent = stretchContent;
			AllowItemClipping = allowItemClipping;
			AllowItemAlignment = allowItemAlignment;
		}
		public double LayerSpace { get; private set; }
		public bool StretchContent { get; private set; }
		public bool AllowItemClipping { get; private set; }
		public bool AllowItemAlignment { get; private set; }
	}
	public struct FlowLayoutPanelItemPosition {
		public FlowLayoutPanelItemPosition(double layerOffset, double itemOffset) {
			LayerOffset = layerOffset;
			ItemOffset = itemOffset;
		}
		public double LayerOffset;
		public double ItemOffset;
	}
	public struct FlowLayoutPanelItemSize {
		public FlowLayoutPanelItemSize(double width, double length) {
			Width = width;
			Length = length;
		}
		public double Width;
		public double Length;
	}
	public class FlowLayoutPanelLayerInfo {
		public FlowLayoutPanelLayerInfo(int firstItemIndex, bool isHardFlowBreak, FlowLayoutPanelItemPosition position) {
			FirstItemIndex = firstItemIndex;
			IsHardFlowBreak = isHardFlowBreak;
			Position = position;
			SlotFirstItemIndexes = new List<int>();
		}
		public int FirstItemIndex { get; private set; }
		public bool IsHardFlowBreak { get; private set; }
		public FlowLayoutPanelItemPosition Position { get; private set; }
		public FlowLayoutPanelItemSize Size;
		public int SlotCount { get { return SlotFirstItemIndexes.Count; } }
		public List<int> SlotFirstItemIndexes { get; private set; }
	}
	public enum FlowPanelLayerBreak { None, Existing, New }
	public class FlowLayoutPanelProvider : LayoutPanelProviderBase {
		public FlowLayoutPanelProvider(IFlowPanelModel model)
			: base(model) {
			LayerInfos = new List<FlowLayoutPanelLayerInfo>();
			VisibleChildren = new List<FrameworkElement>();
		}
		public override void CopyLayoutInfo(FrameworkElement from, FrameworkElement to) {
			base.CopyLayoutInfo(from, to);
			FlowLayoutPanel.SetIsFlowBreak(to, FlowLayoutPanel.GetIsFlowBreak(from));
		}
		public virtual int GetItemIndex(UIElementCollection items, int visibleIndex) {
			if(visibleIndex == LayoutItems.Count)
				return items.IndexOf(LayoutItems[LayoutItems.Count - 1]) + 1;
			else
				return items.IndexOf(LayoutItems[visibleIndex]);
		}
		public virtual int GetItemPlaceIndex(FrameworkElement item, Rect bounds, Point p, out FlowPanelLayerBreak flowBreakKind) {
			flowBreakKind = FlowPanelLayerBreak.None;
			if(!bounds.Contains(p) || LayoutItems.Count == 0)
				return -1;
			for(int layerIndex = 0; layerIndex < LayerInfos.Count; layerIndex++) {
				FlowLayoutPanelLayerInfo layerInfo = LayerInfos[layerIndex];
				if(!GetElementHitTestBounds(GetLayerBounds(layerInfo), bounds, true, true).Contains(p))
					continue;
				for(int slotIndex = 0; slotIndex < layerInfo.SlotCount; slotIndex++) {
					Rect slotBounds = GetSlotBounds(layerInfo, slotIndex);
					if(!GetElementHitTestBounds(slotBounds, bounds, slotIndex == 0, slotIndex == layerInfo.SlotCount - 1).Contains(p))
						continue;
					bool isBeforeItemPlace;
					int result = GetSlotItemPlaceIndex(layerInfo, slotIndex, item, bounds, p, out isBeforeItemPlace);
					if(isBeforeItemPlace) {
						if(result == 0 || FlowLayoutPanel.GetIsFlowBreak(LayoutItems[result]))
							flowBreakKind = FlowPanelLayerBreak.Existing;
					}
					else
						result++;
					return result;
				}
			}
			foreach(FlowLayoutPanelLayerInfo layerInfo in LayerInfos)
				if(layerInfo.IsHardFlowBreak && GetLayerSpaceBounds(layerInfo, bounds).Contains(p)) {
					if(CanAddHardFlowBreaks)
						flowBreakKind = FlowPanelLayerBreak.New;
					return layerInfo.FirstItemIndex;
				}
			if(GetRemainderBounds(bounds, GetLayerBounds(GetLayerInfo(LayoutItems.Count - 1))).Contains(p)) {
				if(CanAddHardFlowBreaks)
					flowBreakKind = FlowPanelLayerBreak.New;
				return LayoutItems.Count;
			}
			return -1;
		}
		public double GetLayerMinWidth(Size itemsMinSize) {
			return GetItemSize(itemsMinSize).Width;
		}
		public double GetLayerMaxWidth(Size itemsMaxSize) {
			return GetItemSize(itemsMaxSize).Width;
		}
		public double GetLayerWidth(FrameworkElements items, out Size itemSize) {
			var result = double.PositiveInfinity;
			itemSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			foreach(var item in items) {
				var itemWidth = GetItemSize(item.GetSize()).Width;
				if(!double.IsPositiveInfinity(result) && itemWidth != result)
					return double.PositiveInfinity;
				result = itemWidth;
			}
			if(double.IsPositiveInfinity(result))
				result = 0;
			itemSize = GetItemSize(new FlowLayoutPanelItemSize(result, double.PositiveInfinity));
			return result;
		}
		public void SetLayerWidth(FrameworkElements items, double value, out Size itemSize) {
			itemSize = GetItemSize(new FlowLayoutPanelItemSize(value, Double.PositiveInfinity));
			Size itemPrevSize;
			if(GetLayerWidth(items, out itemPrevSize) == value)
				return;
			foreach(var item in items) {
				if(!double.IsPositiveInfinity(itemSize.Width))
					item.Width = itemSize.Width;
				if(!double.IsPositiveInfinity(itemSize.Height))
					item.Height = itemSize.Height;
			}
		}
		public virtual double GetLayerDistance(bool isHardFlowBreak) {
			return GetLayerSpace(isHardFlowBreak);
		}
		public virtual double GetLayerSpace(bool isHardFlowBreak) {
			return Parameters.LayerSpace;
		}
		public virtual bool CanAddHardFlowBreaks {
			get { return true; }
		}
		public FrameworkElements LayoutItems { get; protected set; }
		public virtual Orientation Orientation {
			get {
				return Model.Orientation;
			}
		}
		public virtual bool StretchContent {
			get { return Parameters.StretchContent; }
		}
		protected delegate Size MeasureItem(FrameworkElement item);
		protected delegate void ArrangeItem(FrameworkElement item, FlowLayoutPanelItemPosition itemPosition, FlowLayoutPanelItemSize itemSize);
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			CalculateLayout(items, RectHelper.New(maxSize),
				delegate(FrameworkElement item) {
					Size availableSize;
					var itemSize = new FlowLayoutPanelItemSize(double.PositiveInfinity, double.PositiveInfinity);
					if(StretchContent)
						itemSize.Width = GetLayerWidth(RectHelper.New(maxSize));
					availableSize = GetItemSize(itemSize);
					item.Measure(availableSize);
					return item.GetDesiredSize();
				},
				null);
			var contentMaxSize = new FlowLayoutPanelItemSize(0, 0);
			if(items.Count != 0) {
				for(int i = 0; i < LayerInfos.Count; i++) {
					FlowLayoutPanelLayerInfo layerInfo = LayerInfos[i];
					if(i == 0)
						contentMaxSize.Width += layerInfo.Size.Width;
					contentMaxSize.Length = Math.Max(contentMaxSize.Length, layerInfo.Size.Length);
				}
			}
			return GetItemSize(contentMaxSize);
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			CalculateLayout(items, bounds,
				delegate(FrameworkElement item) {
					return item.GetDesiredSize();
				},
				delegate(FrameworkElement item, FlowLayoutPanelItemPosition itemPosition, FlowLayoutPanelItemSize itemSize) {
					{
						itemSize = GetItemSize(item.DesiredSize);   
						if(StretchContent)
							itemSize.Width = GetLayerWidth(viewPortBounds);
					}
					var itemBounds = GetItemBounds(itemPosition, itemSize);
					ArrangeItemCore(item, itemBounds);
				});
			return bounds.Size();
		}
		protected virtual bool CanArrangeItem(FrameworkElement item) {
			return true;
		}
		internal int HiddenItemsCount { get; private set; }
		protected internal List<FrameworkElement> VisibleChildren { get; private set; }
		protected virtual void CalculateLayout(FrameworkElements items, Rect bounds, MeasureItem measureItem, ArrangeItem arrangeItem) {
			double contentStartOffset = GetLayerContentStart(bounds);
			double slotOffset = contentStartOffset;
			var itemPosition = new FlowLayoutPanelItemPosition(GetLayerStart(bounds), slotOffset);
			LayoutItems = items;
			LayerInfos.Clear();
			FlowLayoutPanelLayerInfo layerInfo = null;
			HiddenItemsCount = 0;
			VisibleChildren.Clear();
			ArrangeInfos = new LayoutPanelArrangeInfos();
			for(int i = 0; i < items.Count; i++) {
				FrameworkElement item = items[i];
				FlowLayoutPanelItemSize itemSize = GetItemSize(measureItem(item));
				if(i == 0) {
					bool isHardFlowBreak = false;
					bool isFlowBreak = IsFlowBreak(item, bounds, slotOffset, GetSlotLength(items, i, itemSize), out isHardFlowBreak);
					layerInfo = new FlowLayoutPanelLayerInfo(0, isFlowBreak, itemPosition);
					LayerInfos.Add(layerInfo);
				}
				else {
					bool isHardFlowBreak = false;
					if(IsFlowBreak(item, bounds, slotOffset, GetSlotLength(items, i, itemSize), out isHardFlowBreak)) {
						itemPosition.ItemOffset = slotOffset + GetLayerDistance(isHardFlowBreak);
						layerInfo = new FlowLayoutPanelLayerInfo(i, isHardFlowBreak, itemPosition);
						LayerInfos.Add(layerInfo);
					}
					else {
						itemPosition.ItemOffset = slotOffset;
					}
				}
				layerInfo.SlotFirstItemIndexes.Add(i);
				FlowLayoutPanelItemSize slotSize = itemSize;
				double length = slotOffset + slotSize.Length - contentStartOffset; 
				double width = Math.Max(layerInfo.Size.Width, slotSize.Width); 
				if(!CanArrangeItem(items[i]) || (Parameters.AllowItemClipping && length > (IsHorizontal ? bounds.Width : bounds.Height) + 0.5)) {
					HiddenItemsCount++;
					if(arrangeItem != null)
						ArrangeItemCore(item, new Rect());
				}
				else {
					if(arrangeItem != null)
						arrangeItem(item, itemPosition, itemSize);
					layerInfo.Size.Length = slotOffset + slotSize.Length - contentStartOffset;
					layerInfo.Size.Width = Math.Max(layerInfo.Size.Width, slotSize.Width);
					slotOffset += GetSlotMaxLength(item, itemSize) + Parameters.ItemSpace;
					VisibleChildren.Add(item);
				}
			}
		}
		void ArrangeItemCore(UIElement item, Rect bounds) {
			ArrangeInfos.Add(new LayoutPanelArrangeInfo() { ArrangeBounds = bounds });
			item.Arrange(bounds);
		}
		protected virtual double GetSlotLength(FrameworkElements items, int itemIndex, FlowLayoutPanelItemSize itemSize) {
			return itemSize.Length;
		}
		protected virtual double GetSlotMaxLength(FrameworkElement item, FlowLayoutPanelItemSize itemSize) {
			return itemSize.Length;
		}
		protected virtual double GetSlotMaxWidth(FrameworkElement item, FlowLayoutPanelItemSize itemSize) {
			return itemSize.Width;
		}
		protected double GetSlotMaxWidth(FrameworkElements items) {
			double result = 0;
			for(int i = 0; i < items.Count; i++) {
				FrameworkElement item = items[i];
				FlowLayoutPanelItemSize itemSize = GetItemSize(item.GetDesiredSize());
				result = Math.Max(result, GetSlotMaxWidth(item, itemSize));
			}
			return result;
		}
		protected virtual bool NeedsFullSlot(FrameworkElement item) {
			return true;
		}
		protected FlowLayoutPanelLayerInfo GetLayerInfo(int itemIndex) {
			for(int i = 0; i < LayerInfos.Count; i++)
				if(itemIndex < LayerInfos[i].FirstItemIndex)
					return i == 0 ? null : LayerInfos[i - 1];
			return LayerInfos.Count != 0 ? LayerInfos[LayerInfos.Count - 1] : null;
		}
		protected int GetSlotLastItemIndex(FlowLayoutPanelLayerInfo layerInfo, int slotIndex) {
			if(slotIndex == layerInfo.SlotCount - 1) {
				int layerInfoIndex = LayerInfos.IndexOf(layerInfo);
				if(layerInfoIndex == LayerInfos.Count - 1)
					return LayoutItems.Count - 1;
				else
					return LayerInfos[layerInfoIndex + 1].FirstItemIndex - 1;
			}
			else
				return layerInfo.SlotFirstItemIndexes[slotIndex + 1] - 1;
		}
		protected virtual bool IsFlowBreak(FrameworkElement item, Rect bounds, double slotOffset, double slotLength, out bool isHardFlowBreak) {
			isHardFlowBreak = FlowLayoutPanel.GetIsFlowBreak(item);
			return !StretchContent && isHardFlowBreak;
		}
		protected Rect GetItemBounds(FlowLayoutPanelItemPosition itemPosition, FlowLayoutPanelItemSize itemSize) {
			if(IsHorizontal)
				return new Rect(itemPosition.ItemOffset, itemPosition.LayerOffset, itemSize.Length, itemSize.Width);
			else
				return new Rect(itemPosition.LayerOffset, itemPosition.ItemOffset, itemSize.Width, itemSize.Length);
		}
		protected void GetItemPositionAndSize(Rect itemBounds, out FlowLayoutPanelItemPosition itemPosition, out FlowLayoutPanelItemSize itemSize) {
			if(IsHorizontal) {
				itemPosition = new FlowLayoutPanelItemPosition(itemBounds.Y, itemBounds.X);
				itemSize = new FlowLayoutPanelItemSize(itemBounds.Height, itemBounds.Width);
			}
			else {
				itemPosition = new FlowLayoutPanelItemPosition(itemBounds.X, itemBounds.Y);
				itemSize = new FlowLayoutPanelItemSize(itemBounds.Width, itemBounds.Height);
			}
		}
		protected FlowLayoutPanelItemSize GetItemSize(Size itemSize) {
			FlowLayoutPanelItemPosition itemPosition;
			FlowLayoutPanelItemSize result;
			GetItemPositionAndSize(RectHelper.New(itemSize), out itemPosition, out result);
			return result;
		}
		protected Size GetItemSize(FlowLayoutPanelItemSize itemSize) {
			return GetItemBounds(new FlowLayoutPanelItemPosition(0, 0), itemSize).Size();
		}
		protected double GetLayerStart(Rect bounds) {
			return IsHorizontal ? bounds.Top : bounds.Left;
		}
		protected double GetLayerWidth(Rect bounds) {
			return IsHorizontal ? bounds.Height : bounds.Width;
		}
		protected double GetLayerEnd(Rect bounds) {
			return IsHorizontal ? bounds.Bottom : bounds.Right;
		}
		protected double GetLayerCenter(Rect bounds) {
			return (GetLayerStart(bounds) + GetLayerEnd(bounds)) / 2;
		}
		protected double GetLayerOffset(Point p) {
			return IsHorizontal ? p.Y : p.X;
		}
		protected double GetLayerContentStart(Rect bounds) {
			return IsHorizontal ? bounds.Left : bounds.Top;
		}
		protected double GetLayerContentLength(Rect bounds) {
			return IsHorizontal ? bounds.Width : bounds.Height;
		}
		protected double GetLayerContentEnd(Rect bounds) {
			return GetLayerContentStart(bounds) + GetLayerContentLength(bounds);
		}
		protected double GetLayerContentCenter(Rect bounds) {
			return (GetLayerContentStart(bounds) + GetLayerContentEnd(bounds)) / 2;
		}
		protected double GetLayerContentOffset(Point p) {
			return IsHorizontal ? p.X : p.Y;
		}
		protected virtual Rect GetElementHitTestBounds(Rect elementBounds, Rect bounds, bool isFront, bool isBack) {
			FlowLayoutPanelItemPosition elementPosition;
			FlowLayoutPanelItemSize elementSize;
			GetItemPositionAndSize(elementBounds, out elementPosition, out elementSize);
			elementSize.Width += Parameters.ItemSpace;
			if(isFront) {
				double spaceLength = Math.Max(0, elementPosition.ItemOffset - GetLayerContentStart(bounds));
				elementPosition.ItemOffset -= spaceLength;
				elementSize.Length += spaceLength;
			}
			if(isBack)
				elementSize.Length = Math.Max(0, GetLayerContentEnd(bounds) - elementPosition.ItemOffset);
			else
				elementSize.Length += Parameters.ItemSpace;
			return GetItemBounds(elementPosition, elementSize);
		}
		protected Rect GetLayerBounds(FlowLayoutPanelLayerInfo layerInfo) {
			return GetItemBounds(layerInfo.Position, layerInfo.Size);
		}
		protected Rect GetLayerSpaceBounds(FlowLayoutPanelLayerInfo layerInfo, Rect bounds) {
			var layerSpacePosition = new FlowLayoutPanelItemPosition(0, GetLayerContentStart(bounds));
			var layerSpaceSize = new FlowLayoutPanelItemSize(0, GetLayerContentLength(bounds));
			if(layerInfo.FirstItemIndex == 0)
				layerSpaceSize.Width = Math.Max(0, layerInfo.Position.LayerOffset - GetLayerStart(bounds));
			else
				layerSpaceSize.Width = GetLayerDistance(layerInfo.IsHardFlowBreak);
			layerSpacePosition.LayerOffset = layerInfo.Position.LayerOffset - layerSpaceSize.Width;
			return GetItemBounds(layerSpacePosition, layerSpaceSize);
		}
		protected Rect GetRemainderBounds(Rect bounds, Rect lastLayerBounds) {
			FlowLayoutPanelItemPosition position;
			FlowLayoutPanelItemSize size;
			GetItemPositionAndSize(bounds, out position, out size);
			double layersWidth = GetLayerEnd(lastLayerBounds) - position.LayerOffset;
			position.LayerOffset += layersWidth;
			size.Width = Math.Max(0, size.Width - layersWidth);
			return GetItemBounds(position, size);
		}
		protected Rect GetSlotBounds(FlowLayoutPanelLayerInfo layerInfo, int slotIndex) {
			FlowLayoutPanelItemPosition slotPosition;
			FlowLayoutPanelItemSize slotSize;
			GetItemPositionAndSize(GetLayerBounds(layerInfo), out slotPosition, out slotSize);
			int slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex];
			FlowLayoutPanelItemPosition itemPosition;
			FlowLayoutPanelItemSize itemSize;
			GetItemPositionAndSize(LayoutItems[slotFirstItemIndex].GetBounds(), out itemPosition, out itemSize);
			slotSize.Length = slotPosition.ItemOffset + slotSize.Length - itemPosition.ItemOffset;
			slotPosition.ItemOffset = itemPosition.ItemOffset;
			if(slotIndex < layerInfo.SlotCount - 1) {
				slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex + 1];
				GetItemPositionAndSize(LayoutItems[slotFirstItemIndex].GetBounds(), out itemPosition, out itemSize);
				slotSize.Length = itemPosition.ItemOffset - Parameters.ItemSpace - slotPosition.ItemOffset;
			}
			return GetItemBounds(slotPosition, slotSize);
		}
		protected virtual int GetSlotItemPlaceIndex(FlowLayoutPanelLayerInfo layerInfo, int slotIndex, FrameworkElement item,
			Rect bounds, Point p, out bool isBeforeItemPlace) {
			int slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex];
			if(NeedsFullSlot(item) || NeedsFullSlot(LayoutItems[slotFirstItemIndex])) {
				isBeforeItemPlace = GetLayerContentOffset(p) < GetLayerContentCenter(GetSlotBounds(layerInfo, slotIndex));
				if(isBeforeItemPlace)
					return slotFirstItemIndex;
				else
					return GetSlotLastItemIndex(layerInfo, slotIndex);
			}
			isBeforeItemPlace = false;
			return -1;
		}
		private bool IsHorizontal {
			get { return Orientation == Orientation.Horizontal; }
		}
		protected new IFlowPanelModel Model { get { return (IFlowPanelModel)base.Model; } }
		protected List<FlowLayoutPanelLayerInfo> LayerInfos { get; private set; }
		protected new FlowLayoutPanelParameters Parameters { get { return base.Parameters as FlowLayoutPanelParameters; } }
		internal LayoutPanelArrangeInfos ArrangeInfos { get; private set; }
	}
	public class FlowLayoutPanelController : LayoutPanelControllerBase {
		public FlowLayoutPanelController(IFlowLayoutPanel control) : base(control) { }
		public new IFlowLayoutPanel ILayoutControl { get { return base.ILayoutControl as IFlowLayoutPanel; } }
		public new FlowLayoutPanelProvider LayoutProvider { get { return base.LayoutProvider as FlowLayoutPanelProvider; } }	
		public override bool IsScrollable() {
			return false;
		}
		#region Drag&Drop
		protected override DragAndDropController CreateItemDragAndDropController(Point startDragPoint, FrameworkElement dragControl) {
			return new FlowLayoutPanelDragAndDropControllerBase(this, startDragPoint, dragControl);
		}
		#endregion Drag&Drop
	}
	public class FlowLayoutPanelDragAndDropControllerBase : LayoutPanelDragAndDropControllerBase {
		public FlowLayoutPanelDragAndDropControllerBase(Controller controller, Point startDragPoint, FrameworkElement dragControl)
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
			FlowPanelLayerBreak itemPlaceFlowBreakKind;
			int itemPlaceVisibleIndex = Controller.LayoutProvider.GetItemPlaceIndex(DragControl, Controller.ClientBounds,
				GetItemPlacePoint(p), out itemPlaceFlowBreakKind);
			if(itemPlaceVisibleIndex == -1)
				return;
			int itemPlaceIndex = Controller.LayoutProvider.GetItemIndex(DragControlParent.Children, itemPlaceVisibleIndex);
			if(itemPlaceFlowBreakKind == FlowPanelLayerBreak.New)
				itemPlaceFlowBreakKind = itemPlaceVisibleIndex == 0 ? FlowPanelLayerBreak.Existing : FlowPanelLayerBreak.None;
			bool itemPlaceIsFlowBreak = itemPlaceFlowBreakKind != FlowPanelLayerBreak.None;
			FrameworkElements visibleChildren = Controller.LayoutProvider.LayoutItems;
			int placeHolderVisibleIndex = visibleChildren.IndexOf(DragControlPlaceHolder);
			int placeHolderIndex = DragControlParent.Children.IndexOf(DragControlPlaceHolder);
			bool placeHolderIsFlowBreak = placeHolderVisibleIndex == 0 || FlowLayoutPanel.GetIsFlowBreak(DragControlPlaceHolder);
			bool placeHolderIsOneItemGroup = placeHolderIsFlowBreak &&
				(placeHolderVisibleIndex == visibleChildren.Count - 1 || FlowLayoutPanel.GetIsFlowBreak(visibleChildren[placeHolderVisibleIndex + 1]));
			if(itemPlaceIndex > placeHolderIndex)
				itemPlaceIndex--;
			if(placeHolderIndex != itemPlaceIndex ||
				placeHolderVisibleIndex + 1 == itemPlaceVisibleIndex && itemPlaceIsFlowBreak &&
					(!placeHolderIsFlowBreak || itemPlaceFlowBreakKind == FlowPanelLayerBreak.Existing) ||
				itemPlaceVisibleIndex == placeHolderVisibleIndex &&
					(!itemPlaceIsFlowBreak && placeHolderIsFlowBreak ||
					 itemPlaceFlowBreakKind == FlowPanelLayerBreak.New && !placeHolderIsOneItemGroup)) {
				if(Controller.ILayoutControl.AnimateItemMoving) {
					ElementPositionsAnimation = new ElementBoundsAnimation(visibleChildren);
					ElementPositionsAnimation.StoreOldElementBounds();
				}
				if(placeHolderIsFlowBreak)
					if(placeHolderIsOneItemGroup)
						OnGroupFirstItemChanged(DragControlPlaceHolder, null);
					else
						OnGroupFirstItemChanged(DragControlPlaceHolder, visibleChildren[placeHolderVisibleIndex + 1]);
				if(itemPlaceFlowBreakKind == FlowPanelLayerBreak.Existing)
					OnGroupFirstItemChanged(visibleChildren[itemPlaceVisibleIndex], DragControlPlaceHolder);
				DragControlParent.Children.RemoveAt(placeHolderIndex);
				DragControlParent.Children.Insert(itemPlaceIndex, DragControlPlaceHolder);
				if(placeHolderIsFlowBreak && placeHolderVisibleIndex < visibleChildren.Count - 1)
					SetIsFlowBreakAndStoreOriginalValue(visibleChildren[placeHolderVisibleIndex + 1], true);
				if(itemPlaceIsFlowBreak && itemPlaceVisibleIndex < visibleChildren.Count)
					SetIsFlowBreakAndStoreOriginalValue(visibleChildren[itemPlaceVisibleIndex], itemPlaceFlowBreakKind == FlowPanelLayerBreak.New);
				FlowLayoutPanel.SetIsFlowBreak(DragControlPlaceHolder, itemPlaceIsFlowBreak);
				if(Controller.ILayoutControl.AnimateItemMoving) {
					Controller.Control.UpdateLayout();
					ElementPositionsAnimation.StoreNewElementBounds();
					ElementPositionsAnimation.Begin(Controller.ILayoutControl.ItemMovingAnimationDuration);
				}
			}
		}
		public override void EndDragAndDrop(bool accept) {
			if(ElementPositionsAnimation != null) {
				ElementPositionsAnimation.Stop();
				ElementPositionsAnimation = null;
			}
			if(Controller.ILayoutControl.AnimateItemMoving) {
				ElementPositionsAnimation = new ElementBoundsAnimation(new FrameworkElements { DragControl });
				ElementPositionsAnimation.StoreOldElementBounds(Controller.Control);
			}
			int dropIndex = DragControlParent.Children.IndexOf(DragControlPlaceHolder);
			bool dropIsFlowBreak = FlowLayoutPanel.GetIsFlowBreak(DragControlPlaceHolder);
			base.EndDragAndDrop(accept);
			if(accept) {
				if(DragControlIndex != dropIndex || FlowLayoutPanel.GetIsFlowBreak(DragControl) != dropIsFlowBreak) {
					int oldPosition = Controller.ILayoutControl.GetLogicalChildren(false).IndexOf(DragControl);
					if(CanModifyChildrenCollection) {
						DragControlParent.Children.RemoveAt(DragControlIndex);
						DragControlParent.Children.Insert(dropIndex, DragControl);
					}
					FlowLayoutPanel.SetIsFlowBreak(DragControl, dropIsFlowBreak);
					int newPosition = CanModifyChildrenCollection ? Controller.ILayoutControl.GetLogicalChildren(false).IndexOf(DragControl) : dropIndex;
					Controller.ILayoutControl.OnItemPositionChanged(oldPosition, newPosition);
				}
				SendIsFlowBreakChangeNotifications();
			}
			else
				RestoreIsFlowBreakOriginalValues();
			if(Controller.ILayoutControl.AnimateItemMoving) {
#if !SILVERLIGHT
				object storedDragControlOpacity = DragControl.StorePropertyValue(UIElement.OpacityProperty);
				DragControl.Opacity = 0;
#endif
				Controller.Control.UpdateLayout();
				if(IsConnectedToPresentationSource(DragControl)) {
					ElementPositionsAnimation.StoreNewElementBounds();
					object storedDragControlZIndex = DragControl.StorePropertyValue(Canvas.ZIndexProperty);
					DragControl.SetZIndex(PanelBase.HighZIndex);
					object storedDragControlIsHitTestVisible = DragControl.StorePropertyValue(UIElement.IsHitTestVisibleProperty);
					DragControl.IsHitTestVisible = false;
					ElementPositionsAnimation.Begin(FlowLayoutPanel.ItemDropAnimationDuration, new ExponentialEase { Exponent = 5 },
						delegate {
							DragControl.RestorePropertyValue(Canvas.ZIndexProperty, storedDragControlZIndex);
							DragControl.RestorePropertyValue(UIElement.IsHitTestVisibleProperty, storedDragControlIsHitTestVisible);
							ElementPositionsAnimation = null;
						});
				}
				else {
					ElementPositionsAnimation = null;
				}
#if !SILVERLIGHT
				Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate {
					DragControl.RestorePropertyValue(UIElement.OpacityProperty, storedDragControlOpacity);
				}), DispatcherPriority.Render);
#endif
			}
		}
		protected bool IsConnectedToPresentationSource(UIElement element) {
			return PresentationSource.FromVisual(element) != null;
		}
		protected static ElementBoundsAnimation ElementPositionsAnimation { get; set; }
		protected virtual Point GetItemPlacePoint(Point p) {
			return p;
		}
		protected virtual void OnGroupFirstItemChanged(FrameworkElement oldValue, FrameworkElement newValue) { }
		protected void RestoreIsFlowBreakOriginalValues() {
			if(IsFlowBreakOriginalValues == null)
				return;
			foreach(KeyValuePair<UIElement, bool> originalValue in IsFlowBreakOriginalValues)
				FlowLayoutPanel.SetIsFlowBreak(originalValue.Key, originalValue.Value);
		}
		protected void SendIsFlowBreakChangeNotifications() {
			if(IsFlowBreakOriginalValues == null)
				return;
			FrameworkElements children = Controller.ILayoutControl.GetLogicalChildren(false);
			foreach(KeyValuePair<UIElement, bool> originalValue in IsFlowBreakOriginalValues)
				if(FlowLayoutPanel.GetIsFlowBreak(originalValue.Key) != originalValue.Value) {
					int position = children.IndexOf((FrameworkElement)originalValue.Key);
					Controller.ILayoutControl.OnItemPositionChanged(position, position);
				}
		}
		protected void SetIsFlowBreakAndStoreOriginalValue(UIElement element, bool value) {
			if(FlowLayoutPanel.GetIsFlowBreak(element) == value)
				return;
			if(IsFlowBreakOriginalValues == null)
				IsFlowBreakOriginalValues = new Dictionary<UIElement, bool>();
			if(!IsFlowBreakOriginalValues.ContainsKey(element))
				IsFlowBreakOriginalValues.Add(element, FlowLayoutPanel.GetIsFlowBreak(element));
			FlowLayoutPanel.SetIsFlowBreak(element, value);
		}
		protected override Core.Container CreateDragImageContainer() {
			return new NonLogicalContainer();
		}
		protected override Panel GetDragImageParent() {
			return Controller.IPanel as Panel ?? base.GetDragImageParent();
		}
		protected virtual bool CanModifyChildrenCollection { get { return true; } }
		protected new FlowLayoutPanelController Controller { get { return (FlowLayoutPanelController)base.Controller; } }
		private Dictionary<UIElement, bool> IsFlowBreakOriginalValues { get; set; }
		class NonLogicalContainer : DevExpress.Xpf.Core.Container {
			protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
				return base.CreateUIElementCollection(null);
			}
		}
	}
}
