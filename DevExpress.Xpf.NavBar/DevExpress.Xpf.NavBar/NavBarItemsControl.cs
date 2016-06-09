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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Linq;
using System;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public partial class NavBarItemsControl : ItemsControl {
		static NavBarItemsControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavBarItemsControl), new FrameworkPropertyMetadata(typeof(NavBarItemsControl)));
		}
		public INotifyCollectionChanged ItemsSourceCore {
			get { return (INotifyCollectionChanged)GetValue(ItemsSourceCoreProperty); }
			set { SetValue(ItemsSourceCoreProperty, value); }
		}
		public static readonly DependencyProperty ItemsSourceCoreProperty =
			DependencyPropertyManager.Register("ItemsSourceCore", typeof(INotifyCollectionChanged), typeof(NavBarItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceCorePropertyChanged)));
		protected static void OnItemsSourceCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItemsControl)d).ItemsSource = ((NavBarItemsControl)d).ItemsSourceCore as IEnumerable;
		}
		NavBarControl navBar = null;
		NavBarControl NavBar {
			get {
				if (navBar == null)
					navBar = LayoutHelper.FindParentObject<NavBarControl>(this);
				return navBar;
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			NavBarItemControl itemControl = element as NavBarItemControl;	 
			base.PrepareContainerForItemOverride(element, item);
			if (itemControl != null) {
				itemControl.NavBar = this.NavBar;
				itemControl.Source = item;
			}
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			(element as NavBarItemControl).Do(x => { x.NavBar = null; x.Source = null; });			
		}
	}
	[TemplateVisualState(Name = "ShowBottomBorder", GroupName = "NavPaneBorderStates"), TemplateVisualState(Name = "HideBottomBorder", GroupName = "NavPaneBorderStates")]
	public class GroupItemsContainer : XPFContentControl, IScrollMode {
		#region Fields
		public static readonly DependencyProperty NavBarViewKindProperty;
		public static readonly DependencyProperty ItemsCountProperty;
		public static readonly DependencyProperty DisplaySourceProperty;
		public static readonly DependencyProperty NavBarGroupContentProperty;
		public static readonly DependencyProperty GroupPositionProperty;
		public static readonly DependencyProperty AnimationProgressProperty;
		internal static readonly DependencyProperty IsOverflowPanelVisibleProperty;
		internal static readonly DependencyProperty IsSplitterVisibleProperty;
		internal static readonly DependencyProperty ItemsControlGroupCountProperty;
		internal static readonly DependencyProperty ViewOrientationProperty;
		internal static readonly DependencyProperty ItemsPanelOrientationProperty;
		internal static readonly DependencyProperty ScrollModeProperty;
		internal static readonly DependencyProperty OwnerViewProperty;
		public static readonly DependencyProperty ShowBorderProperty;		
		#endregion
		static GroupItemsContainer() {
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(GroupItemsContainer), new FrameworkPropertyMetadata(true, (d, e) => ((GroupItemsContainer)d).OnShowBorderChanged((bool)e.OldValue)));
			NavBarViewKindProperty = DependencyPropertyManager.Register("NavBarViewKind", typeof(NavBarViewKind), typeof(GroupItemsContainer), new PropertyMetadata(NavBarViewKind.ExplorerBar, (d, e) => ((GroupItemsContainer)d).OnNavBarViewKindChanged()));
			DisplaySourceProperty = DependencyPropertyManager.Register("DisplaySource", typeof(DisplaySource), typeof(GroupItemsContainer), new PropertyMetadata(DisplaySource.Items, (d, e) => ((GroupItemsContainer)d).OnDisplaySourceChanged()));
			ItemsCountProperty = DependencyPropertyManager.Register("ItemsCount", typeof(int), typeof(GroupItemsContainer), new PropertyMetadata(0, (d, e) => ((GroupItemsContainer)d).OnItemsCountChanged()));
			NavBarGroupContentProperty = DependencyPropertyManager.Register("NavBarGroupContent", typeof(object), typeof(GroupItemsContainer), new PropertyMetadata(null, (d, e) => ((GroupItemsContainer)d).OnNavBarGroupContentChanged()));
			GroupPositionProperty = DependencyPropertyManager.Register("GroupPosition", typeof(GroupPosition), typeof(GroupItemsContainer), new PropertyMetadata(GroupPosition.First, (d, e) => ((GroupItemsContainer)d).OnGroupPositionChanged()));
			AnimationProgressProperty = DependencyPropertyManager.Register("AnimationProgress", typeof(double), typeof(GroupItemsContainer), new PropertyMetadata(0.0, (d, e) => ((GroupItemsContainer)d).OnAnimationProgressChanged()));
			IsOverflowPanelVisibleProperty = DependencyPropertyManager.Register("IsOverflowPanelVisible", typeof(bool), typeof(GroupItemsContainer), new PropertyMetadata(true, (d, e) => ((GroupItemsContainer)d).IsOverflowPanelVisiblePropertyChanged()));
			IsSplitterVisibleProperty = DependencyPropertyManager.Register("IsSplitterVisible", typeof(bool), typeof(GroupItemsContainer), new PropertyMetadata(true, (d, e) => ((GroupItemsContainer)d).OnIsSplitterVisiblePropertyChanged()));
			ItemsControlGroupCountProperty = DependencyPropertyManager.Register("ItemsControlGroupCount", typeof(int), typeof(GroupItemsContainer), new PropertyMetadata(0, (d, e) => ((GroupItemsContainer)d).OnItemsControlGroupCountPropertyChanged()));
			ItemsPanelOrientationProperty = DependencyPropertyManager.Register("ItemsPanelOrientation", typeof(Orientation), typeof(GroupItemsContainer), new PropertyMetadata(Orientation.Vertical, (d, e) => ((GroupItemsContainer)d).OnItemsPanelOrientationPropertyChanged()));
			ViewOrientationProperty = DependencyPropertyManager.Register("ViewOrientation", typeof(Orientation), typeof(GroupItemsContainer), new PropertyMetadata(Orientation.Vertical, (d, e) => ((GroupItemsContainer)d).OnOrientationPropertyChanged()));
			ScrollModeProperty = DependencyPropertyManager.Register("ScrollMode", typeof(ScrollMode), typeof(GroupItemsContainer), new PropertyMetadata(ScrollMode.Buttons, (d, e) => ((GroupItemsContainer)d).OnScrollModePropertyChanged()));
			OwnerViewProperty = DependencyPropertyManager.Register("OwnerView", typeof(NavBarViewBase), typeof(GroupItemsContainer), new PropertyMetadata(null));
		}
		public GroupItemsContainer() {
			this.SetDefaultStyleKey(typeof(GroupItemsContainer));
			DataContextChanged += OnDataContextChanged;
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.OldValue == null) SetBindings();
		}
		#region Properties
		public NavBarViewKind NavBarViewKind {
			get { return (NavBarViewKind)GetValue(NavBarViewKindProperty); }
			set { SetValue(NavBarViewKindProperty, value); }
		}
		public DisplaySource DisplaySource {
			get { return (DisplaySource)GetValue(DisplaySourceProperty); }
			set { SetValue(DisplaySourceProperty, value); }
		}
		public int ItemsCount {
			get { return (int)GetValue(ItemsCountProperty); }
			set { SetValue(ItemsCountProperty, value); }
		}
		public object NavBarGroupContent {
			get { return GetValue(NavBarGroupContentProperty); }
			set { SetValue(NavBarGroupContentProperty, value); }
		}
		public GroupPosition GroupPosition {
			get { return (GroupPosition)GetValue(GroupPositionProperty); }
			set { SetValue(GroupPositionProperty, value); }
		}
		public double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
		internal bool IsOverflowPanelVisible {
			get { return (bool)GetValue(IsOverflowPanelVisibleProperty); }
			set { SetValue(IsOverflowPanelVisibleProperty, value); }
		}
		internal bool IsSplitterVisible {
			get { return (bool)GetValue(IsSplitterVisibleProperty); }
			set { SetValue(IsSplitterVisibleProperty, value); }
		}
		internal int ItemsControlGroupCount {
			get { return (int)GetValue(ItemsControlGroupCountProperty); }
			set { SetValue(ItemsControlGroupCountProperty, value); }
		}
		internal Orientation ItemsPanelOrientation {
			get { return (Orientation)GetValue(ItemsPanelOrientationProperty); }
			set { SetValue(ItemsPanelOrientationProperty, value); }
		}
		internal Orientation ViewOrientation {
			get { return (Orientation)GetValue(ViewOrientationProperty); }
			set { SetValue(ViewOrientationProperty, value); }
		}
		internal ScrollMode ScrollMode {
			get { return (ScrollMode)GetValue(ScrollModeProperty); }
			set { SetValue(ScrollModeProperty, value); }
		}
		internal NavBarViewBase OwnerView {
			get { return (NavBarViewBase)GetValue(OwnerViewProperty); }
			set { SetValue(OwnerViewProperty, value); }
		}
		ScrollControl IScrollMode.ScrollControl {
			get { return (ScrollControl)GetTemplateChild("scrollControl"); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OwnerView = LayoutHelper.FindParentObject<NavBarViewBase>(this);
			SetBindings();
			Initialize();
		}
		void Initialize() {
			OnNavBarViewKindChanged();
			OnDisplaySourceChanged();
			OnItemsCountChanged();
			UpdateScrollModeStates();
			UpdateVisualState();
		}
		void SetBindings() {
			if(OwnerView != null) {
				SetBinding(NavBarViewKindProperty, new Binding("NavBarViewKind") { Source = OwnerView });
				SetBinding(ShowBorderProperty, new Binding("ShowBorder") { Source = OwnerView });
				SetBinding(ItemsPanelOrientationProperty, new Binding("ItemsPanelOrientation") { Source = OwnerView });
				SetBinding(ViewOrientationProperty, new Binding("Orientation") { Source = OwnerView });
				if(OwnerView is NavigationPaneView) {
					SetBinding(IsSplitterVisibleProperty, new Binding("IsSplitterVisible") { Source = OwnerView });
					SetBinding(ItemsControlGroupCountProperty, new Binding("ItemsControlGroupCount") { Source = OwnerView });
					SetBinding(IsOverflowPanelVisibleProperty, new Binding("IsOverflowPanelVisible") { Source = OwnerView });
				}
			}
			if(DataContext != null) {
				SetBinding(DisplaySourceProperty, new Binding("DisplaySource"));
				SetBinding(ItemsCountProperty, new Binding("Items.Count"));
				SetBinding(NavBarGroupContentProperty, new Binding("Content"));
				SetBinding(ScrollModeProperty, new Binding("ActualScrollMode"));
				SetBinding(ScrollingSettings.ScrollModeProperty, new Binding("ActualScrollMode"));
				SetBinding(GroupPositionProperty, new Binding("(0)") { Path = new PropertyPath(NavBarPositionPanel.GroupPositionProperty) });
			}
		}
		void OnAnimationProgressChanged() {
			UpdateVisualState();
		}
		void OnDisplaySourceChanged() {
			UpdateStateExplorerBarContentEmptyOrItemsCountZero();
		}
		void OnGroupPositionChanged() {
			UpdateVisualState();
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			UpdateVisualState();
		}
		void OnItemsCountChanged() {
			UpdateStateExplorerBarContentEmptyOrItemsCountZero();
		}
		void OnItemsPanelOrientationPropertyChanged() {
			UpdateVisualState();
		}
		void OnOrientationPropertyChanged() {
			UpdateVisualState();
		}
		void OnNavBarGroupContentChanged() {
			UpdateStateExplorerBarContentEmptyOrItemsCountZero();
		}
		void OnNavBarViewKindChanged() {
			UpdateStateNavBarViewKind();
			UpdateStateExplorerBarContentEmptyOrItemsCountZero();
			UpdateNavPaneBorders();
			UpdateVisualState();			
		}
		void IsOverflowPanelVisiblePropertyChanged() {
			UpdateNavPaneBorders();
		}
		void OnItemsControlGroupCountPropertyChanged() {
			UpdateNavPaneBorders();
		}
		void OnIsSplitterVisiblePropertyChanged() {
			UpdateNavPaneBorders();
		}
		void OnScrollModePropertyChanged() {
			UpdateVisualState();
		}
		void UpdateStateNavBarViewKind() {
			switch (NavBarViewKind) {
				case NavBarViewKind.ExplorerBar:
					VisualStateManager.GoToState(this, "NavBarViewKindExplorerBar", false);
					break;
				case NavBarViewKind.NavigationPane:
					VisualStateManager.GoToState(this, "NavBarViewKindNavigationPane", false);
					break;
				case NavBarViewKind.SideBar:
					VisualStateManager.GoToState(this, "NavBarViewKindSideBar", false);
					break;
				default:
					break;
			}
		}
		void UpdateNavPaneBorders() {
			if (ShowBorder) {
				string state = (NavBarViewKind == NavBarViewKind.NavigationPane && ItemsControlGroupCount == 0 && !IsSplitterVisible && !IsOverflowPanelVisible) ? "ShowBottomBorder" : "HideBottomBorder";
				VisualStateManager.GoToState(this, state, false);
			}
		}
		void UpdateScrollModeStates() {
			ScrollingSettings.UpdateScrollModeStates(this);
		}
		void UpdateStateExplorerBarContentEmptyOrItemsCountZero() {
			bool isContentEmpty =
				NavBarViewKind == NavBarViewKind.ExplorerBar && DisplaySource == DisplaySource.Content && NavBarGroupContent == null;
			bool isItemsCountZero =
				NavBarViewKind == NavBarViewKind.ExplorerBar && DisplaySource == DisplaySource.Items && ItemsCount == 0;
			if (isContentEmpty || isItemsCountZero)
				VisualStateManager.GoToState(this, "ExplorerBarItemsOrContentEmpty", false);
			else
				VisualStateManager.GoToState(this, "ExplorerBarItemsOrContentNotEmpty", false);
		}
		void UpdateVisualState() {
			string state = String.Empty;
			state = DefineStateByNavBarViewKind();
			if (ShowBorder) {
				state += DefineStateByGroupPosition();
				if (AnimationProgress == 0)
					state += "0";
			} else {
				state += "NoBorder";
			}
			state += DefineStateByScrollMode(state);
			VisualStateManager.GoToState(this, state, false);
		}
		string DefineStateByGroupPosition() {
			switch (GroupPosition) {
				case GroupPosition.First:
					return "First";
				case GroupPosition.Last:
					return "Last";
				case GroupPosition.Middle:
					return "Middle";
				case GroupPosition.Single:
					return "Single";
				default:
					return String.Empty;
			}
		}
		string DefineStateByNavBarViewKind() {
			switch (NavBarViewKind) {
				case NavBarViewKind.ExplorerBar:
					return "ExplorerBar";
				case NavBarViewKind.NavigationPane:
					return "NavigationPane";
				case NavBarViewKind.SideBar:
					return "SideBar";
				default:
					return String.Empty;
			}
		}
		string DefineStateByScrollMode(string state) {
			if(state == "SideBarLast" && OwnerView != null && ScrollingSettings.GetScrollMode(OwnerView) == ScrollMode.ScrollBar)
				return "ScrollModeNormal";
			return String.Empty;
		}
	}
}
