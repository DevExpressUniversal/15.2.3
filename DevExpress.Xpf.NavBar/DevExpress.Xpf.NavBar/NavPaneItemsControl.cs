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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Input;
using System.Collections.Specialized;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public class NavPaneItemsControl : ItemsControl {
		public INotifyCollectionChanged ItemsSourceCore {
			get { return (INotifyCollectionChanged)GetValue(ItemsSourceCoreProperty); }
			set { SetValue(ItemsSourceCoreProperty, value); }
		}		
		#region static
		public static readonly DependencyProperty VisibleElementCountProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty NavBarProperty;
		public static readonly DependencyProperty ItemsSourceCoreProperty;		
		static NavPaneItemsControl() {
			VisibleElementCountProperty = DependencyPropertyManager.Register("VisibleElementCount", typeof(int), typeof(NavPaneItemsControl), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnVisibleElementCountPropertyChanged)));
			NavBarProperty = DependencyPropertyManager.Register("NavBar", typeof(NavBarControl), typeof(NavPaneItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNavBarPropertyChanged)));
			ItemsSourceCoreProperty = DependencyPropertyManager.Register("ItemsSourceCore", typeof(INotifyCollectionChanged), typeof(NavPaneItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceCorePropertyChanged)));
		}
		protected static void OnVisibleElementCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneItemsControl)d).VisibleElementCountChanged(e);
		}
		protected static void OnNavBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneItemsControl)d).OnNavBarChanged(e);
		}
		protected static void OnItemsSourceCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneItemsControl)d).ItemsSource = ((NavPaneItemsControl)d).ItemsSourceCore as System.Collections.IEnumerable;
		}
		#endregion
		public NavPaneItemsControl() {
			this.SetDefaultStyleKey(typeof(NavPaneItemsControl));
			CreateBindings();
		}
		protected virtual void CreateBindings() {
			Binding b = new Binding("DataContext") { Source = this };
			BindingOperations.SetBinding(this, NavBarProperty, b);
		}
		public int VisibleElementCount {
			get { return (int)GetValue(VisibleElementCountProperty); }
			set { SetValue(VisibleElementCountProperty, value); }
		}
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); }
			set { SetValue(NavBarProperty, value); }
		}
		protected override DependencyObject GetContainerForItemOverride() {
			NavBarGroupControl group = new NavBarGroupControl();
			group.SetValue(NavigationPaneView.ElementProperty, this.GetValue(NavigationPaneView.ElementProperty));
			if(DataContext as NavBarControl != null)
			group.SetBinding(ScrollingSettings.ScrollModeProperty, new Binding("(0)") { Path = new PropertyPath(ScrollingSettings.ScrollModeProperty), Source = ((NavBarControl)DataContext).View });
			return group;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			NavBarGroupControl group = element as NavBarGroupControl;
			if (group != null) {
				group.SetBinding(NavBarGroupControl.NavBarProperty, new Binding("NavBar") { Source = this });
			}
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			NavBarGroupControl group = element as NavBarGroupControl;
			if (group != null) {
				group.ClearValue(NavBarGroupControl.NavBarProperty);
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		protected virtual void OnNavBarChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void VisibleElementCountChanged(DependencyPropertyChangedEventArgs e) {
		}
	}
	public class NavPaneGroupButtonPanelControl : NavPaneItemsControl {
		public Size GetChildrenDesizedSizeByInfinity() {
			return MeasureOverride(SizeHelper.Infinite);
		}
	}
	public class NavPaneGroupButtonPanel : NavigationPanePanelBase {
		public NavPaneGroupButtonPanel() { }
		protected NavigationPaneView View { get; set; }
		protected int GetMaxVisibleGroupCount() {
			if(View == null)
				View = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<NavigationPaneView>(this);
			if(View == null)
				return Children.Count;
			return View.ActualMaxVisibleGroupCount;
		}
		void UpdateGroupPosition(int count) {
			for(int i = 0; i < count; i++) {
				SetGroupPosition(Children[i], count);
			}
		}
		protected virtual void SetGroupPosition(UIElement elem, int count) {
			if(count == 1)
				NavBarPositionPanel.SetGroupPosition(elem, GroupPosition.Single);
			else if(Children.IndexOf(elem) == 0)
				NavBarPositionPanel.SetGroupPosition(elem, GroupPosition.First);
			else if(Children.IndexOf(elem) == count - 1)
				NavBarPositionPanel.SetGroupPosition(elem, GroupPosition.Last);
			else
				NavBarPositionPanel.SetGroupPosition(elem, GroupPosition.Middle);
		}
		protected override Size MeasureOverride(Size availableSize) {
			UpdateNavigationPaneView();
			double totalHeight = 0;
			double maxWidth = 0;
			foreach(UIElement elem in Children) {
				elem.Measure(CreateSize(GetWidth(availableSize), double.PositiveInfinity));
				maxWidth = Math.Max(maxWidth, GetWidth(elem.DesiredSize));
			}
			int index = 0;
			for(; index < Math.Min(Children.Count, GetMaxVisibleGroupCount()); index++) {
				UIElement elem = Children[index];
				elem.IsHitTestVisible = true;
				if(totalHeight + GetHeight(elem.DesiredSize) > GetHeight(availableSize)) {
					break;
				}
				totalHeight += GetHeight(elem.DesiredSize);
			}
			if(View != null)
				View.ActualVisibleGroupCount = index;
			UpdateGroupPosition(index);
			for(;index<Children.Count; index++) {
				UIElement elem = Children[index];
				elem.IsHitTestVisible = false;
			}
			return CreateSize(Math.Min(maxWidth, GetWidth(availableSize)), totalHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double y = 0;
			foreach(UIElement elem in Children) {
				elem.Arrange(CreateRect(0, y, GetWidth(finalSize), GetHeight(elem.DesiredSize)));
				y += GetHeight(elem.DesiredSize);
			}
			return finalSize;
		}
	}
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "IsMouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "IsPressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "IsActiveTrue", GroupName = "IsActiveStates")]
	[TemplateVisualState(Name = "IsActiveFalse", GroupName = "IsActiveStates")]
	[TemplateVisualState(Name = "Expanded", GroupName = "ExpandStates")]
	[TemplateVisualState(Name = "Collapsed", GroupName = "ExpandStates")]
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "WithSplitter", GroupName = "SplitterVisibilityStates")]
	[TemplateVisualState(Name = "WithoutSplitter", GroupName = "SplitterVisibilityStates")]
	public class NavPaneGroupButton : NavPaneXPFButton {
		#region Fields
		public static readonly DependencyProperty GroupPositionProperty;
		public static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty IsSplitterVisibleProperty;
		public static readonly DependencyProperty OrientationProperty;
		internal static readonly DependencyProperty IsMouseOverCoreProperty;
		internal static readonly DependencyProperty IsPressedCoreProperty;
		public static readonly DependencyProperty ShowBorderProperty;		
		#endregion
		static NavPaneGroupButton() {
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavPaneGroupButton), new FrameworkPropertyMetadata(true, (d, e) => ((NavPaneGroupButton)d).OnShowBorderChanged((bool)e.OldValue)));
			GroupPositionProperty = DependencyPropertyManager.Register("GroupPosition", typeof(GroupPosition), typeof(NavPaneGroupButton), new PropertyMetadata(GroupPosition.First, (d, e) => ((NavPaneGroupButton)d).OnGroupPositionPropertyChanged()));
			IsActiveProperty = DependencyPropertyManager.Register("IsActive", typeof(bool), typeof(NavPaneGroupButton), new FrameworkPropertyMetadata(false, (d, e) => ((NavPaneGroupButton)d).OnIsActivePropertyChanged()));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(NavPaneGroupButton), new PropertyMetadata(true, (d, e) => ((NavPaneGroupButton)d).OnIsExpandedPropertyChanged()));
			IsSplitterVisibleProperty = DependencyPropertyManager.Register("IsSplitterVisible", typeof(bool), typeof(NavPaneGroupButton), new PropertyMetadata(true, (d, e) => ((NavPaneGroupButton)d).OnIsSplitterVisiblePropertyChanged()));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavPaneGroupButton), new FrameworkPropertyMetadata(Orientation.Vertical, (d, e) => ((NavPaneGroupButton)d).OnOrientationPropertyChanged()));
			IsMouseOverCoreProperty = DependencyPropertyManager.Register("IsMouseOverCore", typeof(bool), typeof(NavPaneGroupButton), new PropertyMetadata(false, (d, e) => ((NavPaneGroupButton)d).OnIsMouseOverPropertyChanged()));
			IsPressedCoreProperty = DependencyPropertyManager.Register("IsPressedCore", typeof(bool), typeof(NavPaneGroupButton), new PropertyMetadata(false, (d, e) => ((NavPaneGroupButton)d).OnIsPressedPropertyChanged()));
		}
		public NavPaneGroupButton() {
			this.SetDefaultStyleKey(typeof(NavPaneGroupButton));
			SetBindings();
		}
		#region Properties
		public GroupPosition GroupPosition {
			get { return (GroupPosition)GetValue(GroupPositionProperty); }
			set { SetValue(GroupPositionProperty, value); }
		}
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool IsSplitterVisible {
			get { return (bool)GetValue(IsSplitterVisibleProperty); }
			set { SetValue(IsSplitterVisibleProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		internal bool IsMouseOverCore {
			get { return (bool)GetValue(IsMouseOverCoreProperty); }
			set { SetValue(IsMouseOverCoreProperty, value); }
		}
		internal bool IsPressedCore {
			get { return (bool)GetValue(IsPressedCoreProperty); }
			set { SetValue(IsPressedCoreProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}		
		#endregion
		#region Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualStates();
		}
		void OnGroupPositionPropertyChanged() {
			UpdateSplitterVisibilityStates();
		}
		void OnIsActivePropertyChanged() {
			UpdateActiveState();
		}
		void OnIsExpandedPropertyChanged() {
			UpdateStateIsExpanded();
		}
		void OnIsMouseOverPropertyChanged() {
			UpdateCommonStates();
		}
		void OnIsPressedPropertyChanged() {
			UpdateCommonStates();
		}
		void OnIsSplitterVisiblePropertyChanged() {
			UpdateSplitterVisibilityStates();
		}
		void OnOrientationPropertyChanged() {
			UpdateStateOrientation();
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			UpdateShowBorderStates();
		}
		void SetBindings() {
			SetBinding(GroupPositionProperty, new Binding("(0)") { Path = new PropertyPath(NavBarPositionPanel.GroupPositionProperty) });
			SetBinding(IsActiveProperty, new Binding("IsActive"));
			SetBinding(IsMouseOverCoreProperty, new Binding("IsMouseOver") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
			SetBinding(IsPressedCoreProperty, new Binding("IsPressed") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
			SetBinding(IsExpandedProperty, new Binding("NavBar.View.IsExpanded"));
			SetBinding(IsSplitterVisibleProperty, new Binding("NavBar.View.IsSplitterVisible"));
			SetBinding(OrientationProperty, new Binding("NavBar.View.Orientation"));
			SetBinding(ShowBorderProperty, new Binding("NavBar.View.ShowBorder"));
		}
		void UpdateActiveState() {
			VisualStateManager.GoToState(this, IsActive ? "IsActiveTrue" : "IsActiveFalse", false);
		}
		void UpdateCommonStates() {
			string state = "Normal";
			if(IsPressedCore && !IsActive)
				state = "IsPressed";
			else if(IsMouseOverCore && !IsActive)
				state = "IsMouseOver";
			VisualStateManager.GoToState(this, state, false);
		}
		void UpdateSplitterVisibilityStates() {
			string stateName = (!IsSplitterVisible && (GroupPosition == GroupPosition.First || GroupPosition == GroupPosition.Single)) ? "WithoutSplitter" : "WithSplitter";
			VisualStateManager.GoToState(this, stateName, false);
		}
		void UpdateStateIsExpanded() {
			VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", false);
		}
		void UpdateStateOrientation() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", false);
		}
		void UpdateShowBorderStates() {
			VisualStateManager.GoToState(this, ShowBorder ? "WithBorder" : "WithoutBorder", false);
		}
		void UpdateVisualStates() {
			UpdateActiveState();
			UpdateShowBorderStates();
			UpdateSplitterVisibilityStates();
			UpdateStateIsExpanded();
			UpdateStateOrientation();
		}
		#endregion                
	}
	public class NavPaneXPFButton : XPFButton {
		static NavPaneXPFButton() {
			IsManipulationEnabledProperty.OverrideMetadata(typeof(NavPaneXPFButton), new FrameworkPropertyMetadata(true));
		}
		public NavPaneXPFButton() {
			Internal.NavBarGroupHelper.SetIsGroupHeader(this, true);
		}
		#region Touch
		bool emulateClick = false;
		TouchPoint point = null;
		protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchDown(e);
			point = e.GetTouchPoint(this);
			emulateClick = true;
		}
		protected override void OnTouchMove(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchMove(e);
			if(point == null || !(object.Equals(e.GetTouchPoint(this).Position, point.Position)))
				emulateClick = false;
		}
		protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchUp(e);
			if(emulateClick) {
				point = null;
				emulateClick = false;
				if(!e.Handled) {
					NavBarGroup group = DataContext as NavBarGroup;
					group.With(x => x.NavBar).Do(x => x.SelectionStrategy.SelectGroup(group));
				}
			}
		}
		#endregion
	}
	public class NavPaneGroupButtonPanelContentPresenter : ContentPresenter {
		public static readonly DependencyProperty WithoutSplitterThicknessProperty;
		public static readonly DependencyProperty WithSplitterThicknessProperty;
		internal static readonly DependencyProperty IsSplitterVisibleProperty;
		internal static readonly DependencyProperty ItemsControlGroupCountProperty;
		public NavPaneGroupButtonPanelContentPresenter() {			
		}
		static NavPaneGroupButtonPanelContentPresenter() {
			IsSplitterVisibleProperty = DependencyPropertyManager.Register("IsSplitterVisible", typeof(bool), typeof(NavPaneGroupButtonPanelContentPresenter), new PropertyMetadata(true, (d, e) => ((NavPaneGroupButtonPanelContentPresenter)d).OnIsSplitterVisiblePropertyChanged()));
			ItemsControlGroupCountProperty = DependencyPropertyManager.Register("ItemsControlGroupCount", typeof(int), typeof(NavPaneGroupButtonPanelContentPresenter), new PropertyMetadata(0, (d, e) => ((NavPaneGroupButtonPanelContentPresenter)d).OnItemsControlGroupCountPropertyChanged()));
			WithoutSplitterThicknessProperty = DependencyPropertyManager.Register("WithoutSplitterThickness", typeof(Thickness), typeof(NavPaneGroupButtonPanelContentPresenter), new FrameworkPropertyMetadata(new Thickness(0)));
			WithSplitterThicknessProperty = DependencyPropertyManager.Register("WithSplitterThickness", typeof(Thickness), typeof(NavPaneGroupButtonPanelContentPresenter), new FrameworkPropertyMetadata(new Thickness(0)));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBindings();
		}
		public Thickness WithSplitterThickness {
			get { return (Thickness)GetValue(WithSplitterThicknessProperty); }
			set { SetValue(WithSplitterThicknessProperty, value); }
		}
		public Thickness WithoutSplitterThickness {
			get { return (Thickness)GetValue(WithoutSplitterThicknessProperty); }
			set { SetValue(WithoutSplitterThicknessProperty, value); }
		}
		internal bool IsSplitterVisible {
			get { return (bool)GetValue(IsSplitterVisibleProperty); }
			set { SetValue(IsSplitterVisibleProperty, value); }
		}
		internal int ItemsControlGroupCount {
			get { return (int)GetValue(ItemsControlGroupCountProperty); }
			set { SetValue(ItemsControlGroupCountProperty, value); }
		}
		void OnItemsControlGroupCountPropertyChanged() {
			UpdateSplitterBehaviorStates();
		}
		void OnIsSplitterVisiblePropertyChanged() {
			UpdateSplitterBehaviorStates();
		}
		void SetBindings() {
			SetBinding(IsSplitterVisibleProperty, new Binding("View.IsSplitterVisible"));
			SetBinding(ItemsControlGroupCountProperty, new Binding("View.ItemsControlGroupCount"));
		}
		void UpdateSplitterBehaviorStates() {
			if(ItemsControlGroupCount == 0) {
				this.Margin = new Thickness(0);
				return;
			}
			this.Margin = IsSplitterVisible ? WithSplitterThickness : WithoutSplitterThickness;
		}
	}
}
