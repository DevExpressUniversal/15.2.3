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
using System.Windows.Media.Animation;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Content", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_Bounds", Type = typeof(FrameworkElement))]
	public class DropBoundsControl : ContentControl {
		#region static
		public static readonly DependencyProperty DropTypeProperty;
		public static readonly DependencyProperty IsDragSourceProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		static DropBoundsControl() {
			var dProp = new DependencyPropertyRegistrator<DropBoundsControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("DropType", ref DropTypeProperty, DropType.None,
				(dObj, e) => ((DropBoundsControl)dObj).OnDropTypeChanged((DropType)e.NewValue));
			dProp.Register("IsDragSource", ref IsDragSourceProperty, false, 
				(dObj, e) => ((DropBoundsControl)dObj).OnIsDragSourceChanged((bool)e.NewValue));
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null);
		}
		#endregion static
		public DropBoundsControl() {
			Focusable = false;
			Loaded += new RoutedEventHandler(DropBoundsControl_Loaded);
			Unloaded += new RoutedEventHandler(DropBoundsControl_Unloaded);
		}
		void DropBoundsControl_Unloaded(object sender, RoutedEventArgs e) {
			if(Container != null) {
				UnsubscribeEvents(Container);
				Container = null;
			}
		}
		DockLayoutManager Container;
		void DropBoundsControl_Loaded(object sender, RoutedEventArgs e) {
			Container = DockLayoutManager.FindManager(this);
			if(Container != null)
				SubscribeEvents(Container);
		}
		protected virtual void SubscribeEvents(DockLayoutManager manager) {
			if(manager.IsDisposing) return;
			if(manager.CustomizationController != null)
				manager.CustomizationController.DragInfoChanged += OnDragInfoChanged;
		}
		protected virtual void UnsubscribeEvents(DockLayoutManager manager) {
			if(manager.IsDisposing) return;
			if(manager.CustomizationController != null)
				manager.CustomizationController.DragInfoChanged -= OnDragInfoChanged;
		}
		public DropType DropType {
			get { return (DropType)GetValue(DropTypeProperty); }
			set { SetValue(DropTypeProperty, value); }
		}
		public bool IsDragSource {
			get { return (bool)GetValue(IsDragSourceProperty); }
			set { SetValue(IsDragSourceProperty, value); }
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		protected FrameworkElement PartBounds { get; private set; }
		protected FrameworkElement PartCenter { get; private set; }
		protected RowDefinition Row0 { get; private set; }
		protected RowDefinition Row1 { get; private set; }
		protected RowDefinition Row2 { get; private set; }
		protected ColumnDefinition Col0 { get; private set; }
		protected ColumnDefinition Col1 { get; private set; }
		protected ColumnDefinition Col2 { get; private set; }
		protected BaseLayoutItem Item { get { return LayoutItem ?? itemCore; } }
		protected BaseLayoutItem itemCore;
		private bool CanShowCenterZone {
			get { return Item is LayoutGroup && ((LayoutGroup)Item).GroupBorderStyle != GroupBorderStyle.Tabbed; }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DockLayoutManager.Ensure(this);
			itemCore = DataContext as BaseLayoutItem;
			PartCenter = GetTemplateChild("PART_Center") as FrameworkElement;
			PartBounds = GetTemplateChild("PART_Bounds") as FrameworkElement;
			Row0 = GetTemplateChild("PART_Row0") as RowDefinition;
			Row1 = GetTemplateChild("PART_Row1") as RowDefinition;
			Row2 = GetTemplateChild("PART_Row2") as RowDefinition;
			Col0 = GetTemplateChild("PART_Col0") as ColumnDefinition;
			Col1 = GetTemplateChild("PART_Col1") as ColumnDefinition;
			Col2 = GetTemplateChild("PART_Col2") as ColumnDefinition;
		}
		void OnDragInfoChanged(object sender, Customization.DragInfoChangedEventArgs e) {
			if(e.Info == null) {
				ClearValue(DropTypeProperty);
				ClearValue(IsDragSourceProperty);
			}
			else {
				DropType = DropBoundsHelper.CalcDropType(Item, e.Info);
				IsDragSource = e.Info.Item == Item;
			}
		}
		void ExpandWithAnimation(DependencyObject element, DependencyProperty property, GridLength from, GridLength to) {
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(DropBoundsHelper.CreateGridLengthAnimation(element, property, from, to));
			storyboard.Completed += OnExpandAnimationCompleted;
			PartBounds.BeginStoryboard(storyboard);
		}
		void ShowCenterZoneWithAnimation(double width, double height) {
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(DropBoundsHelper.CreateDoubleAnimation(PartCenter, FrameworkElement.WidthProperty, 0, width));
			storyboard.Children.Add(DropBoundsHelper.CreateDoubleAnimation(PartCenter, FrameworkElement.HeightProperty, 0, height));
			PartCenter.BeginStoryboard(storyboard);
		}
		void EnsureCollapsedWithAnimation() {
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(DropBoundsHelper.CreateGridLengthAnimation(Col0, ColumnDefinition.WidthProperty, Col0.Width, GridLengthAnimation.Zero));
			storyboard.Children.Add(DropBoundsHelper.CreateGridLengthAnimation(Col2, ColumnDefinition.WidthProperty, Col2.Width, GridLengthAnimation.Zero));
			storyboard.Children.Add(DropBoundsHelper.CreateGridLengthAnimation(Row0, RowDefinition.HeightProperty, Row0.Height, GridLengthAnimation.Zero));
			storyboard.Children.Add(DropBoundsHelper.CreateGridLengthAnimation(Row2, RowDefinition.HeightProperty, Row2.Height, GridLengthAnimation.Zero));
			storyboard.Completed += OnCollapseAnimationCompleted;
			PartBounds.BeginStoryboard(storyboard);
			PartCenter.ClearValue(FrameworkElement.WidthProperty);
			PartCenter.ClearValue(FrameworkElement.HeightProperty);
		}
		void OnExpandAnimationCompleted(object sender, EventArgs e) {
			ChangeSplitterSelection(DropType);
		}
		void OnCollapseAnimationCompleted(object sender, EventArgs e) {
			ChangeSplitterSelection(DropType);
		}
		int lockChanged;
		protected virtual void UpdateVisualState() {
			if(DropType == DropType.Center)
				VisualStateManager.GoToState(this, "DropCenterState", false);
			else {
				VisualStateManager.GoToState(this, IsDragSource ? "DragSourceState" : "EmptyDraggingState", false);
			}
		}
		protected virtual void OnIsDragSourceChanged(bool newValue) {
			UpdateVisualState();
		}
		protected virtual void OnDropTypeChanged(Layout.Core.DropType type) {
			if(lockChanged > 0) return;
			lockChanged++;
			UpdateVisualState();
			EnsureSplitters();
			EnsureCollapsedWithAnimation();
			if(type != DropType.None) {
				double splitterSize = 0;
				if(Item.Parent != null) {
					Splitter splitter = DropBoundsHelper.ChooseSplitter(type, prevSplitter, nextSplitter);
					bool fHorz = Item.Parent.Orientation == Orientation.Horizontal;
					splitterSize = (splitter == null || Item.Parent.Orientation != type.ToOrientation()) ? 0 : (fHorz ? splitter.ActualWidth : splitter.ActualHeight);
					SelectSplitter(type, splitter);
				}
				double limit = type.ToOrientation() == Orientation.Horizontal &&
					(Item.ItemType == LayoutItemType.ControlItem) ? 32.0 : 14.0;
				limit = splitterSize > limit ? 4.0 : limit - splitterSize;
				GridLength endWidth = new GridLength(Math.Min(limit, Math.Max(4, PartBounds.ActualWidth * 0.15)), GridUnitType.Pixel);
				GridLength endHeight = new GridLength(Math.Min(limit, Math.Max(4, PartBounds.ActualHeight * 0.15)), GridUnitType.Pixel);
				switch(type) {
					case DropType.Left:
						ExpandWithAnimation(Col0, ColumnDefinition.WidthProperty, GridLengthAnimation.Zero, endWidth);
						break;
					case DropType.Right:
						ExpandWithAnimation(Col2, ColumnDefinition.WidthProperty, GridLengthAnimation.Zero, endWidth);
						break;
					case DropType.Top:
						ExpandWithAnimation(Row0, RowDefinition.HeightProperty, GridLengthAnimation.Zero, endHeight);
						break;
					case DropType.Bottom:
						ExpandWithAnimation(Row2, RowDefinition.HeightProperty, GridLengthAnimation.Zero, endHeight);
						break;
					case DropType.Center:
						if(CanShowCenterZone)
							ShowCenterZoneWithAnimation(PartBounds.ActualWidth * 0.6, PartBounds.ActualHeight * 0.6);
						break;
				}
			}
			lockChanged--;
		}
		void SelectSplitter(DropType type, Splitter splitter) {
			if(splitter != null && type != DropType.None && type != DropType.Center) {
				if(type.ToOrientation() == Item.Parent.Orientation)
					splitter.IsDragDropOver = true;
			}
		}
		void ChangeSplitterSelection(DropType type) {
			if(type == DropType.None || type == DropType.Center) {
				if(nextSplitter != null) nextSplitter.IsDragDropOver = false;
				if(prevSplitter != null) prevSplitter.IsDragDropOver = false;
				return;
			}
			Splitter splitter = DropBoundsHelper.ChooseSplitter(type, prevSplitter, nextSplitter);
			if(splitter != null)
				splitter.IsDragDropOver = (type.ToOrientation() == Item.Parent.Orientation);
		}
		Splitter nextSplitter;
		Splitter prevSplitter;
		void EnsureSplitters() {
			nextSplitter = null;
			prevSplitter = null;
			if(Item.Parent == null) return;
			var collection = Item.Parent.ItemsInternal;
			int index = collection.IndexOf(Item);
			if(index - 1 > 0)
				prevSplitter = collection[index - 1] as Splitter;
			if(index + 1 < collection.Count - 1)
				nextSplitter = collection[index + 1] as Splitter;
		}
	}
	public class DropBoundsHelper {
		public static DropType CalcDropType(BaseLayoutItem item, Customization.DragInfo info) {
			DropType type = info.Type;
			BaseLayoutItem target = info.Target;
			BaseLayoutItem drag = info.Item;
			if(item == null || LayoutItemsHelper.IsParent(target, drag)) return DropType.None;
			if(item == target && type == DropType.Center) return type;
			DropType result = DropType.None;
			if(item.Parent == null || item == drag || target == drag) return result;
			if(item != null && target != null) {
				LayoutGroup parent = item.Parent;
				if(type.ToOrientation() != parent.Orientation) {
					return target == item ? type : DropType.None;
				}
				if(target == item) {
					result = type;
					if(LayoutItemsHelper.AreInSameGroup(drag, item)) {
						bool? isNext = LayoutItemsHelper.IsNextNeighbour(drag, item);
						if(isNext != null) {
							if(isNext.Value == true) {
								if(type == DropType.Left || type == DropType.Top)
									result = DropType.None;
							}
							else {
								if(type == DropType.Right || type == DropType.Bottom)
									result = DropType.None;
							}
						}
					}
				}
				else {
					if(LayoutItemsHelper.AreInSameGroup(target, item)) {
						bool? isNext = LayoutItemsHelper.IsNextNeighbour(target, item);
						if(isNext != null) {
							if(isNext.Value) {
								if(type == DropType.Right)
									result = DropType.Left;
								if(type == DropType.Bottom)
									result = DropType.Top;
							}
							else {
								if(type == DropType.Left)
									result = DropType.Right;
								if(type == DropType.Top)
									result = DropType.Bottom;
							}
						}
					}
				}
			}
			return result;
		}
		public static Splitter ChooseSplitter(DropType type, Splitter prev, Splitter next) {
			switch(type) {
				case DropType.Left:
				case DropType.Top:
					return prev;
				case DropType.Right:
				case DropType.Bottom:
					return next;
			}
			return null;
		}
		public static DoubleAnimation CreateDoubleAnimation(DependencyObject element, DependencyProperty property, double from, double to) {
			DoubleAnimation animation = new DoubleAnimation()
			{
				From = from,
				To = to,
				BeginTime = TimeSpan.Zero,
				Duration = TimeSpan.FromMilliseconds(25)
			};
			Storyboard.SetTarget(animation, element);
			Storyboard.SetTargetProperty(animation, new PropertyPath(property));
			return animation;
		}
		public static GridLengthAnimation CreateGridLengthAnimation(DependencyObject element, DependencyProperty property,
			GridLength from, GridLength to) {
			GridLengthAnimation animation = new GridLengthAnimation()
			{
				From = from,
				To = to,
				BeginTime = TimeSpan.Zero,
				Duration = TimeSpan.FromMilliseconds(20)
			};
			Storyboard.SetTarget(animation, element);
			Storyboard.SetTargetProperty(animation, new PropertyPath(property));
			return animation;
		}
	}
}
