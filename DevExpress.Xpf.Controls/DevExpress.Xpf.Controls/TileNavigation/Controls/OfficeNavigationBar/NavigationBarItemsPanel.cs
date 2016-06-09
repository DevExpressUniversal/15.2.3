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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Navigation.Internal {
	public class NavigationBarItemsPanel : FlowLayoutPanel, IDragPanel, IDragPanelVisual {
		#region static
		public static readonly DependencyProperty MaxItemCountProperty;
		private static void OnMaxItemCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItemsPanel navigationBarItemsPanel = o as NavigationBarItemsPanel;
			if(navigationBarItemsPanel != null)
				navigationBarItemsPanel.OnMaxItemCountChanged((int)e.OldValue, (int)e.NewValue);
		}
		static NavigationBarItemsPanel() {
			MaxItemCountProperty = DependencyProperty.Register("MaxItemCount", typeof(int), typeof(NavigationBarItemsPanel), new UIPropertyMetadata(-1, OnMaxItemCountChanged));
		}
		#endregion
		public NavigationBarItemsPanel() {
			EnsureDragController();
		}
		protected virtual LocalDragPanelController<NavigationBarItemsPanel, NavigationBarItemsPanel> ResolveDragController() {
			return new NavigationBarDragController();
		}
		void EnsureDragController() {
			DragController.Do(x => x.Uninitialize());
			DragController = null;
			if(AllowItemMoving) {
				DragController = ResolveDragController();
				DragController.Initialize(this);
			}
		}
		protected override void OnAllowItemMovingChanged() {
			base.OnAllowItemMovingChanged();
			EnsureDragController();
		}
		protected virtual void OnMaxItemCountChanged(int oldValue, int newValue) {
			InvalidateMeasure();
		}
		protected override LayoutPanelProviderBase CreateLayoutProvider() {
			return new NavigationBarItemsPanelProvider(this);
		}
		protected override LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new NavigationBarItemsPanelParameters(ItemSpacing, LayerSpace, AllowItemClipping, AllowItemAlignment, MaxItemCount);
		}
		bool IsValidIndex(IList collection, int index) {
			return index < collection.Count && index >= 0;
		}
		private void SyncCollectionItems(int oldPosition, int newPosition, IEnumerable targetCollection) {
			IList targetList = targetCollection as IList;
			if(targetList == null) targetList = new EnumerableObservableWrapper<object>(targetCollection);
			ILockable lockable = targetList as ILockable;
			try {
				Owner.LockSelection();
				lockable.Do(x => x.BeginUpdate());
				if(!IsValidIndex(targetList, oldPosition) || !IsValidIndex(targetList, newPosition)) return;
				var item = targetList[oldPosition];
				targetList.RemoveAt(oldPosition);
				targetList.Insert(newPosition, item);
			}
			finally {
				this.Owner.UnlockSelection();
				lockable.Do(x => x.EndUpdate());
			}
		}
		void SyncItems(int oldPosition, int newPosition) {
			if(Owner == null || oldPosition == newPosition) return;
			ISupportInitialize supportInitialize = Owner.NavigationClient as ISupportInitialize;
			supportInitialize.Do(x => x.BeginInit());
			var selectedIndex = this.Owner.SelectedIndex;
			var targetCollection = Owner.ItemsSource ?? Owner.Items;
			var collectionChanged = targetCollection as System.Collections.Specialized.INotifyCollectionChanged;
			if(collectionChanged != null) {
				SyncCollectionItems(oldPosition, newPosition, targetCollection);
				if(selectedIndex == oldPosition) this.Owner.SelectedIndex = newPosition;
			}
			supportInitialize.Do(x => x.EndInit());
		}
		protected override PanelControllerBase CreateController() {
			return new NavigationBarItemsPanelController(this);
		}
		protected override void AfterArrange() {
			base.AfterArrange();
			childrenChanged.Do(x => x(this, EventArgs.Empty));
		}
		protected FrameworkElement GetActiveChild() {
			foreach(FrameworkElement child in LayoutProvider.VisibleChildren) {
				if((bool)child.GetValue(System.Windows.Controls.Primitives.Selector.IsSelectedProperty))
					return child;
			}
			return null;
		}
		internal OfficeNavigationBar Owner { get; set; }
		public int MaxItemCount {
			get { return (int)GetValue(MaxItemCountProperty); }
			set { SetValue(MaxItemCountProperty, value); }
		}
		public int ActualHiddenItemsCount {
			get { return LayoutProvider.HiddenItemsCount; }
		}
		protected new NavigationBarItemsPanelProvider LayoutProvider { get { return (NavigationBarItemsPanelProvider)base.LayoutProvider; } }
		LocalDragPanelController<NavigationBarItemsPanel, NavigationBarItemsPanel> DragController;
		#region IDragPanel Members
		IDragPanel IDragPanelVisual.GetDragPanel(IDragPanel sourceDragPanel) { return this;		 }
		IDragPanelVisual IDragPanel.VisualPanel { get { return this; }  }
		EventHandler childrenChanged;
		event EventHandler IDragPanel.ChildrenChanged {
			add { childrenChanged += value; }
			remove { childrenChanged -= value; }
		}
		bool IDragPanel.CanStartDrag(FrameworkElement child) {
			return true;
		}
		IEnumerable<FrameworkElement> IDragPanel.Children {
			get { return LayoutProvider.VisibleChildren; }
		}
		DragControllerBase IDragPanel.Controller {
			get { return DragController; }
		}
		DragWidgetWindow IDragPanel.CreateDragWidget(FrameworkElement child) {
			return null;
		}
		void IDragPanel.DropOnEmptySpace(FrameworkElement child) {
		}
		FrameworkElement IDragPanel.Insert(FrameworkElement child, int index) {
			return ((IDragPanel)this).Move(child, index);
		}
		FrameworkElement IDragPanel.Move(FrameworkElement child, int index) {
			SyncItems(Owner.IndexOf(child), index);
			InvalidateMeasure();
			InvalidateArrange();
			UpdateLayout();
			return GetActiveChild();
		}
		Orientation IDragPanel.Orientation {
			get { return Orientation; }
		}
		string IDragPanel.Region {
			get { return string.Empty; }
		}
		void IDragPanel.Remove(FrameworkElement child) {
		}
		void IDragPanel.SetVisibility(FrameworkElement child, Visibility visibility) {
		 }
		void IDragPanel.OnDragFinished() { }
		#endregion
		public class NavigationBarItemsPanelProvider :FlowLayoutPanelProvider {
			protected new NavigationBarItemsPanelParameters Parameters { get { return (NavigationBarItemsPanelParameters)base.Parameters; } }
			public NavigationBarItemsPanelProvider(IFlowPanelModel model)
				: base(model) {
			}
			protected override bool CanArrangeItem(FrameworkElement item) {
				return Parameters.MaxItemCount <= 0 || LayoutItems.IndexOf(item) < Parameters.MaxItemCount;
			}
		}
		public class NavigationBarItemsPanelParameters : FlowLayoutPanelParameters {
			public int MaxItemCount { get; private set; }
			public NavigationBarItemsPanelParameters(double itemSpace, double layerSpace, bool allowItemClipping, bool allowItemAlignment, int maxItemCount)
				: base(itemSpace, layerSpace, false, allowItemClipping, allowItemAlignment) {
				MaxItemCount = maxItemCount;
			}
		}
		public class NavigationBarItemsPanelController : FlowLayoutPanelController {
			public NavigationBarItemsPanelController(IFlowLayoutPanel control)
				: base(control) {   
			}
			protected override bool CanItemDragAndDrop() {
				return false;
			}
		}
	}
	public class NavigationBarDragController : LocalDragPanelController<NavigationBarItemsPanel, NavigationBarItemsPanel> {
		IDragPanel IDragPanel { get { return (IDragPanel)DragPanel; } }
		IDragItem IDragItem { get { return (IDragItem)DragChild; } }
		protected override double GetDragChildMaxOffset() {
			return base.GetDragChildMaxOffset() + DragPanel.ItemSpacing * (IDragPanel.Children.Count() - DragChildIndex - 1);
		}
		protected override double GetDragChildMinOffset() {
			return base.GetDragChildMinOffset() - DragPanel.ItemSpacing * (DragChildIndex);
		}
		protected override double GetDragOffset() {
			return base.GetDragOffset() + DragPanel.ItemSpacing;
		}
		protected override void OnDrop() {
			DragPanel.Owner.Do(x => x.LockSelection());
			base.OnDrop();
			DragPanel.Owner.Do(x => x.UnlockSelection());
			IDragItem.Do(x => x.IsDragStarted = false);
		}
		protected override void OnDragStarted() {
			base.OnDragStarted();
			IDragItem.Do(x => x.IsDragStarted = true);
		}
	}
	public class NavigationBarItems2Panel : Panel {
		#region static
		public static readonly DependencyProperty Content1Property = DependencyPropertyManager.Register("Content1", typeof(UIElement), typeof(NavigationBarItems2Panel),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent1PropertyChanged)));
		public static readonly DependencyProperty Content2Property = DependencyPropertyManager.Register("Content2", typeof(UIElement), typeof(NavigationBarItems2Panel),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent2PropertyChanged)));
		public static readonly DependencyProperty Content1LocationProperty = DependencyProperty.Register("Content1Location", typeof(Dock), typeof(NavigationBarItems2Panel),
			new PropertyMetadata(Dock.Left, new PropertyChangedCallback(OnContent1LocationChanged)));
		public static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register("ItemSpacing", typeof(double), typeof(NavigationBarItems2Panel),
			new PropertyMetadata(0d, new PropertyChangedCallback(OnItemSpacingChanged)));
		static protected void OnContent2PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((NavigationBarItems2Panel)o).OnContent2Changed(e.OldValue as UIElement);
		}
		static protected void OnContent1PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((NavigationBarItems2Panel)o).OnContent1Changed(e.OldValue as UIElement);
		}
		private static void OnContent1LocationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItems2Panel navigationBarStackPanel = o as NavigationBarItems2Panel;
			if(navigationBarStackPanel != null)
				navigationBarStackPanel.OnContent1LocationChanged((Dock)e.OldValue, (Dock)e.NewValue);
		}
		private static void OnItemSpacingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItems2Panel navigationBarStackPanel = o as NavigationBarItems2Panel;
			if(navigationBarStackPanel != null)
				navigationBarStackPanel.OnItemSpacingChanged((double)e.OldValue, (double)e.NewValue);
		}
		#endregion
		public UIElement Content2 {
			get { return (UIElement)GetValue(Content2Property); }
			set { SetValue(Content2Property, value); }
		}
		public UIElement Content1 {
			get { return (UIElement)GetValue(Content1Property); }
			set { SetValue(Content1Property, value); }
		}
		public Dock Content1Location {
			get { return (Dock)GetValue(Content1LocationProperty); }
			set { SetValue(Content1LocationProperty, value); }
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		protected virtual void OnContent1LocationChanged(Dock oldValue, Dock newValue) {
			InvalidateMeasure();
		}
		protected virtual void OnItemSpacingChanged(double oldValue, double newValue) {
			InvalidateMeasure();
		}
		protected virtual void OnContent2Changed(UIElement oldValue) {
			if(oldValue != null) Children.Remove(oldValue);
			if(Content2 != null) Children.Add(Content2);
		}
		protected virtual void OnContent1Changed(UIElement oldValue) {
			if(oldValue != null) Children.Remove(oldValue);
			if(Content1 != null) Children.Add(Content1);
		}
		protected override Size MeasureOverride(Size availableSize) {
			var Content1 = this.Content1;
			var Content2 = this.Content2;
			Content1.Measure(availableSize);
			Content2.Measure(availableSize);
			Size size1 = Content1.DesiredSize;
			Size size2 = Content2.DesiredSize;
			bool isHorz = Content1Location == Dock.Left || Content1Location == Dock.Right;
			if(isHorz) {
				if(size1.Width + size2.Width + ItemSpacing > availableSize.Width) {
					Content1.Measure(new Size(Math.Max(0, availableSize.Width - size2.Width - ItemSpacing), availableSize.Height));
					size1 = Content1.DesiredSize;
					Content2.Measure(new Size(Math.Max(0, availableSize.Width - size1.Width - ItemSpacing), availableSize.Height));
					size2 = Content2.DesiredSize;
				}
			}
			else {
				if(size1.Height + size2.Height + ItemSpacing > availableSize.Height) {
					Content1.Measure(new Size(availableSize.Width, Math.Max(0, availableSize.Height - size2.Height - ItemSpacing)));
					size1 = Content1.DesiredSize;
					Content2.Measure(new Size(availableSize.Width, Math.Max(0, availableSize.Height - size1.Height - ItemSpacing)));
					size2 = Content2.DesiredSize;
				}
			}
			double availableLength = 0;
			if(isHorz) availableLength = double.IsInfinity(availableSize.Width) ? size1.Width + size2.Width : Math.Min(availableSize.Width, size1.Width + size2.Width);
			else availableLength = double.IsInfinity(availableSize.Height) ? size1.Height + size2.Height : Math.Min(availableSize.Height, size1.Height + size2.Height);
			availableLength += ItemSpacing;
			return isHorz ? new Size(availableLength, Math.Max(size1.Height, size2.Height)) :
							new Size(Math.Max(size1.Width, size2.Width), availableLength);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			var Content1 = this.Content1;
			var Content2 = this.Content2;
			Size size1 = Content1.DesiredSize;
			Size size2 = Content2.DesiredSize;
			switch(Content1Location) {
				case Dock.Top: {
						Content2.Arrange(new Rect(0, 0, finalSize.Width, size2.Height));
						Content1.Arrange(new Rect(0, size2.Height + ItemSpacing, finalSize.Width, Math.Max(0, finalSize.Height - size2.Height - ItemSpacing)));
					}
					break;
				case Dock.Left: {
						Content2.Arrange(new Rect(0, 0, size2.Width, finalSize.Height));
						Content1.Arrange(new Rect(size2.Width + ItemSpacing, 0, Math.Max(0, finalSize.Width - size2.Width - ItemSpacing), finalSize.Height));
					}
					break;
				case Dock.Bottom: {
						double height = Math.Max(finalSize.Height - size1.Height - ItemSpacing, size2.Height);
						Content1.Arrange(new Rect(0, 0, finalSize.Width, Math.Max(0, finalSize.Height - height - ItemSpacing)));
						Content2.Arrange(new Rect(0, finalSize.Height - height, finalSize.Width, height));
					}
					break;
				default: {
						double width = Math.Max(finalSize.Width - size1.Width - ItemSpacing, size2.Width);
						Content1.Arrange(new Rect(0, 0, Math.Max(0, finalSize.Width - width - ItemSpacing), finalSize.Height));
						Content2.Arrange(new Rect(finalSize.Width - width, 0, width, finalSize.Height));
					}
					break;
			}
			return finalSize;
		}
	}
}
