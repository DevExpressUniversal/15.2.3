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

using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System;
using System.Windows.Media;
using System.Linq;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid.Hierarchy {
	public enum HierarchyChangeType { Invalidated, ItemRemoved }
	public class HierarchyChangedEventArgs : EventArgs {
		public static readonly HierarchyChangedEventArgs Default = new HierarchyChangedEventArgs(HierarchyChangeType.Invalidated, null);
		public HierarchyChangedEventArgs(HierarchyChangeType changeType, IItem item = null) {
			this.ChangeType = changeType;
			this.Item = item;
		}
		public HierarchyChangeType ChangeType { get; private set; }
		public IItem Item { get; private set; }
	}
	public delegate void HierarchyChangedEventHandler(object sender, HierarchyChangedEventArgs e);
	public interface IDetailRootItemsContainer : IItemsContainer {
	}
	public interface IRootItemsContainer : IDetailRootItemsContainer {
		event HierarchyChangedEventHandler HierarchyChanged;
		double ScrollItemOffset { get; }
		IItem ScrollItem { get; }
	}
	public interface IItemsContainer {
		IList<IItem> Items { get; }
		Size DesiredSize { get; set; }
		Size RenderSize { get; set; }
		double AnimationProgress { get; }
	}
	public class EmptyItemsContainer : IItemsContainer {
		public static readonly IItemsContainer Instance = new EmptyItemsContainer();
		static readonly IItem[] EmptyItems = new IItem[0];
		EmptyItemsContainer() { }
		IList<IItem> IItemsContainer.Items { get { return EmptyItems; } }
		Size IItemsContainer.DesiredSize { get; set; }
		Size IItemsContainer.RenderSize { get; set; }
		double IItemsContainer.AnimationProgress { get { return 1; } }
	}
	public interface IItem : ISupportVisibleIndex {
		FrameworkElement Element { get; }
		IItemsContainer ItemsContainer { get; }
		bool IsFixedItem { get; }
		bool IsItemsContainer { get; }
		bool IsRowVisible { get; }
	}
	public class HierarchyPanel : Panel {
		internal static double GetScrollElementOffset(UIElement element, double scrollOffset) {
			double offset = Math.Round(element.DesiredSize.Height * scrollOffset);
			double dpiOffset = offset * ScreenHelper.ScaleX;
			offset -= (dpiOffset - Math.Round(dpiOffset)) / ScreenHelper.ScaleX;
			return -offset;
		}
		internal Dictionary<IItem, object> cache = new Dictionary<IItem, object>();
		public static readonly DependencyProperty ItemsContainerProperty;
		public static readonly DependencyProperty OrientationProperty;
		private static readonly DependencyPropertyKey IsItemVisiblePropertyKey;
		public static readonly DependencyProperty IsItemVisibleProperty;
		public static readonly DependencyProperty FixedElementsProperty;
		static readonly DependencyPropertyKey FixedElementsPropertyKey;
		public static readonly DependencyProperty DataPresenterProperty;
		static HierarchyPanel() {
			ItemsContainerProperty = DependencyPropertyManager.Register("ItemsContainer", typeof(IRootItemsContainer), typeof(HierarchyPanel), new PropertyMetadata(null, (d, e) => ((HierarchyPanel)d).OnItemsContainerChanged((IRootItemsContainer)e.OldValue)));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(HierarchyPanel), new PropertyMetadata(Orientation.Vertical, (d, e) => ((HierarchyPanel)d).OnOrientationChanged()));
			IsItemVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsItemVisible", typeof(bool), typeof(HierarchyPanel), new FrameworkPropertyMetadata(true));
			IsItemVisibleProperty = IsItemVisiblePropertyKey.DependencyProperty;
			FixedElementsPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedElements", typeof(IList<FrameworkElement>), typeof(HierarchyPanel), new FrameworkPropertyMetadata(null));
			FixedElementsProperty = FixedElementsPropertyKey.DependencyProperty;
			DataPresenterProperty = DependencyPropertyManager.Register("DataPresenter", typeof(DataPresenterBase), typeof(HierarchyPanel), new PropertyMetadata(null, (d, e) => ((HierarchyPanel)d).OnDataPresenterChanged((DataPresenterBase)e.OldValue)));
		}
		static void SetIsItemVisible(DependencyObject element, bool value) {
			element.SetValue(IsItemVisiblePropertyKey, value);
		}
		public static bool GetIsItemVisible(DependencyObject element) {
			return (bool)element.GetValue(IsItemVisibleProperty);
		}
		public IRootItemsContainer ItemsContainer {
			get { return (IRootItemsContainer)GetValue(ItemsContainerProperty); }
			set { SetValue(ItemsContainerProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public IList<FrameworkElement> FixedElements {
			get { return (IList<FrameworkElement>)GetValue(FixedElementsProperty); }
			private set { this.SetValue(FixedElementsPropertyKey, value); }
		}
		public DataPresenterBase DataPresenter {
			get { return (DataPresenterBase)GetValue(DataPresenterProperty); }
			set { SetValue(DataPresenterProperty, value); }
		}
		protected SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		List<FrameworkElement> fixedElements;
		Dictionary<int, List<IItem>> hierarchy;
		void AddToHierarchy(IItem item, int level) {
			List<IItem> levelList = null;
			if(!hierarchy.TryGetValue(level, out levelList)) {
				levelList = new List<IItem>();
				hierarchy[level] = levelList;
			}
			levelList.Add(item);
		}
		void AssignZIndex() {
			int zIndex = 0;
			foreach(int key in hierarchy.Keys.OrderByDescending(x => x)) {
				foreach(IItem item in hierarchy[key]) {
					SetZIndex(item.Element, zIndex++);
				}
			}
		}
		protected override Size ArrangeOverride(Size finalSize) {
			fixedElements = new List<FrameworkElement>();
			hierarchy = new Dictionary<int, List<IItem>>();
			ArrangeItemsContainer(0, finalSize, ItemsContainer, 0);
#if SL
			Dispatcher.BeginInvoke(new Action(() => {
#endif
				FixedElements = fixedElements;
#if SL
			}));
#endif
			AssignZIndex();
			return finalSize;
		}
		protected virtual Size ArrangeItemsContainer(double offset, Size availableSize, IItemsContainer itemsContainer, int level) {
			double defineSize = 0;
			double containerSize = itemsContainer != null ? SizeHelper.GetDefineSize(itemsContainer.RenderSize) * itemsContainer.AnimationProgress : 0;
			foreach(IItem item in GetSortedChildrenElements(itemsContainer)) {
				double elementSize = Math.Max(0, SizeHelper.GetDefineSize(item.Element.DesiredSize) + GetItemOffset(item));
				Rect rect = new Rect(SizeHelper.CreatePoint(defineSize + offset, 0), SizeHelper.CreateSize(Math.Max(0, Math.Min(elementSize, containerSize - defineSize)), SizeHelper.GetSecondarySize(availableSize)));
				ArrangeItem(item, rect, containerSize - defineSize > 0, level);
				defineSize += elementSize;
				defineSize += SizeHelper.GetDefineSize(ArrangeItemsContainer(defineSize + offset, SizeHelper.CreateSize(Math.Max(0, SizeHelper.GetDefineSize(availableSize) - defineSize), SizeHelper.GetSecondarySize(availableSize)), item.ItemsContainer, level + 1));
			}
			return SizeHelper.CreateSize(containerSize, SizeHelper.GetSecondarySize(availableSize));
		}
		protected void ArrangeItem(IItem item, Rect rect, bool isVisible, int level) {
			if(item.IsFixedItem)
				fixedElements.Add(item.Element);
			double absoluteOffset = GetScrollElementOffset(item.Element, item == ItemsContainer.ScrollItem ? ItemsContainer.ScrollItemOffset : 0);
			if(absoluteOffset != 0) {
				item.Element.Clip = new RectangleGeometry { Rect = new Rect(new Point(0, -absoluteOffset), rect.Size()) };
			}
			else {
				if(item.Element.Clip != null)
					item.Element.Clip = null;
			}
			rect.Y += absoluteOffset;
			rect.Height -= absoluteOffset;
			SetIsItemVisible(item as DependencyObject ?? item.Element, isVisible);
			AddToHierarchy(item, level);
			item.Element.Arrange(rect);
		}
#if DEBUGTEST
		internal int MeasureCount { get; set; }
#endif
		internal double Viewport { get; set; }
		internal int FullyVisibleItemsCount { get; set; }
		protected override Size MeasureOverride(Size availableSize) {
#if DEBUGTEST
			MeasureCount++;
#endif
			ValidateTree(ItemsContainer);
			Size desiredSize = MeasurePixelSnapperHelper.MeasureOverride(MeasureItemsContainer(availableSize, ItemsContainer), SnapperType.Ceil);
			CalcViewport(DataPresenter != null ? DataPresenter.LastConstraint : availableSize);
			return desiredSize;
		}
		protected virtual Size MeasureItemsContainer(Size availableSize, IItemsContainer itemsContainer) {
			availableSize = SizeHelper.CreateSize(double.PositiveInfinity, SizeHelper.GetSecondarySize(availableSize));
			double defineSize = 0;
			double secondarySize = 0;
			foreach(IItem item in GetSortedChildrenElements(itemsContainer)) {
				item.Element.Measure(availableSize);
				defineSize += Math.Max(0, SizeHelper.GetDefineSize(item.Element.DesiredSize) + GetItemOffset(item));
				secondarySize = Math.Max(secondarySize, SizeHelper.GetSecondarySize(item.Element.DesiredSize));
				item.ItemsContainer.DesiredSize = MeasureItemsContainer(SizeHelper.CreateSize(SizeHelper.GetDefineSize(availableSize) - defineSize, SizeHelper.GetSecondarySize(availableSize)), item.ItemsContainer);
				defineSize += SizeHelper.GetDefineSize(item.ItemsContainer.DesiredSize);
				secondarySize = Math.Max(secondarySize, SizeHelper.GetSecondarySize(item.ItemsContainer.DesiredSize));
			}
			if(itemsContainer == null)
				return new Size();
			itemsContainer.RenderSize = SizeHelper.CreateSize(defineSize, secondarySize);
			return SizeHelper.CreateSize(defineSize * itemsContainer.AnimationProgress, secondarySize);
		}
		internal void CalcViewport(Size availableSize) {
			FullyVisibleItemsCount = 0;
			CalcViewportCore(availableSize);
			if(Viewport == 0)
				Viewport = 1;
		}
		protected virtual void CalcViewportCore(Size availableSize) {
			Viewport = 0;
			CalcViewport(availableSize, ItemsContainer);
		}
		protected bool AllowPerPixelScrolling { get { return DataPresenter != null && DataPresenter.View != null ? DataPresenter.View.ViewBehavior.AllowPerPixelScrolling : true; } }
		double CalcViewport(Size availableSize, IItemsContainer itemsContainer) {
			double defineSize = SizeHelper.GetDefineSize(availableSize);
			foreach(IItem item in GetSortedChildrenElements(itemsContainer)) {
				double itemOffset = GetItemOffset(item);
				double visiblePart = Math.Min(SizeHelper.GetDefineSize(item.Element.DesiredSize) + itemOffset, Math.Max(0, defineSize)) / (SizeHelper.GetDefineSize(item.Element.DesiredSize));
				defineSize -= SizeHelper.GetDefineSize(item.Element.DesiredSize) + itemOffset;
				if(SizeHelper.GetDefineSize(item.Element.DesiredSize) != 0) {
					if(visiblePart == 1 || AllowPerPixelScrolling)
						Viewport += visiblePart;
					if(visiblePart == 1)
						FullyVisibleItemsCount++;
				}
				defineSize = CalcViewport(SizeHelper.CreateSize(Math.Max(0, defineSize), SizeHelper.GetSecondarySize(availableSize)), item.ItemsContainer);
			}
			return defineSize;
		}
		double GetItemOffset(IItem item) {
			return GetScrollElementOffset(item.Element, item == ItemsContainer.ScrollItem ? ItemsContainer.ScrollItemOffset : 0);
		}
		protected IList<IItem> GetSortedChildrenElements(IItemsContainer itemsContainer) {
			if(itemsContainer == null)
				return new IItem[0];
			return OrderPanelBase.GetSortedElements<IItem>(new SimpleBridgeList<IItem, IItem>(itemsContainer.Items, item => item), true, item => item.VisibleIndex);
		}
		void OnItemsContainerChanged(IRootItemsContainer oldValue) {
			if(oldValue != null)
				oldValue.HierarchyChanged -= new HierarchyChangedEventHandler(ItemsContainer_HierarchyChanged);
			if(ItemsContainer != null)
				ItemsContainer.HierarchyChanged += new HierarchyChangedEventHandler(ItemsContainer_HierarchyChanged);
			Children.Clear();
			cache.Clear();
			ClearFixedElements();
			InvalidateMeasure();
		}
		void OnDataPresenterChanged(DataPresenterBase oldValue) {
			if(oldValue != null) {
				oldValue.Panel = null;
			}
			if(DataPresenter != null)
				DataPresenter.Panel = this;
		}
		void ClearFixedElements() {
			FixedElements = null;
		}
		void ItemsContainer_HierarchyChanged(object sender, HierarchyChangedEventArgs e) {
			if(e.ChangeType == HierarchyChangeType.ItemRemoved) {
				e.Item.Element.Visibility = Visibility.Collapsed;				
			}
			InvalidateMeasure();
		}
		void OnOrientationChanged() {
			InvalidateMeasure();
		}
		protected void ValidateTree(IItemsContainer itemsContainer) {
			if(itemsContainer == null)
				return;
			foreach(IItem item in itemsContainer.Items) {
				if(!cache.ContainsKey(item)) {
					DetachItem(item);
					Children.Add(item.Element);
					cache.Add(item, null);
				}
				OrderPanelBase.UpdateChildVisibility(item.Element, true, item.VisibleIndex);
				ValidateTree(item.ItemsContainer);
			}
		}
		internal static void DetachItem(IItem item) {
			HierarchyPanel parent = (HierarchyPanel)VisualTreeHelper.GetParent(item.Element);
			if (parent != null) {
				parent.Children.Remove(item.Element);
				parent.cache.Remove(item);
				parent.ClearFixedElements();
			}
		}
	}
}
