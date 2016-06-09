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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public class ExplorerBarScrollableContentControl : ScrollableContentControl {
		public ExplorerBarScrollableContentControl() {
			this.SetDefaultStyleKey(typeof(ExplorerBarScrollableContentControl));
		}
	}
	public class GroupScrollableContentControl : ScrollableContentControl {
		public GroupScrollableContentControl() {
			this.SetDefaultStyleKey(typeof(GroupScrollableContentControl));
			DataContextChanged += OnDataContextChanged;
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			SetBindings();
		}
		protected internal override void OnScrollModeChanged() {
			if (DataContext == null)
				return;
			UpdateScrollingTemplate();
		}
		protected internal override void SetBindings() {
			if (DataContext == null)
				return;
			SetBinding(ScrollModeProperty, new Binding("ActualScrollMode"));
		}
	}
	public class NavPaneScrollableContentControl : ScrollableContentControl {
		public NavPaneScrollableContentControl() {
			this.SetDefaultStyleKey(typeof(NavPaneScrollableContentControl));
			Loaded += OnLoaded;
		}		
		void OnLoaded(object sender, RoutedEventArgs e) {
			InvalidateMeasure();
		}		
		protected internal override void OnScrollModeChanged() {
			if (DataContext == null)
				return;
			UpdateScrollingTemplate();
		}
		protected internal override void SetBindings() {
			if (DataContext == null)
				return;
			SetBinding(ScrollModeProperty, new Binding("ActualScrollMode"));
		}
	}
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	public class ScrollableContentControl : ContentControl {
		public static readonly DependencyProperty AllowScrollingProperty;
		public static readonly DependencyProperty ButtonUpTemplateProperty;
		public static readonly DependencyProperty ButtonDownTemplateProperty;
		public static readonly DependencyProperty NavBarViewKindProperty;
		public static readonly DependencyProperty NotAllowScrollingTemplateProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ScrollModeProperty;
		static ScrollableContentControl() {
			AllowScrollingProperty = DependencyPropertyManager.Register("AllowScrolling", typeof(bool), typeof(ScrollableContentControl), new FrameworkPropertyMetadata(true, (d, e) => ((ScrollableContentControl)d).OnAllowScrollingChanged()));
			ButtonUpTemplateProperty = DependencyPropertyManager.Register("ButtonUpTemplate", typeof(ControlTemplate), typeof(ScrollableContentControl));
			ButtonDownTemplateProperty = DependencyPropertyManager.Register("ButtonDownTemplate", typeof(ControlTemplate), typeof(ScrollableContentControl));
			NavBarViewKindProperty = DependencyPropertyManager.Register("NavBarViewKind", typeof(NavBarViewKind), typeof(ScrollableContentControl), new PropertyMetadata(NavBarViewKind.ExplorerBar, (d, e) => ((ScrollableContentControl)d).OnNavBarViewKindChanged()));
			NotAllowScrollingTemplateProperty = DependencyPropertyManager.Register("NotAllowScrollingTemplate", typeof(ControlTemplate), typeof(ScrollableContentControl));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(ScrollableContentControl), new FrameworkPropertyMetadata(Orientation.Vertical, (d, e) => ((ScrollableContentControl)d).OnOrientationChanged()));
			ScrollModeProperty = DependencyPropertyManager.Register("ScrollMode", typeof(ScrollMode), typeof(ScrollableContentControl), new PropertyMetadata(ScrollMode.Buttons, (d, e) => ((ScrollableContentControl)d).OnScrollModeChanged()));
			FocusableProperty.OverrideMetadata(typeof(ScrollableContentControl), new FrameworkPropertyMetadata(false));
		}
		public ScrollableContentControl() {
			this.SetDefaultStyleKey(typeof(ScrollableContentControl));
		}
		public bool AllowScrolling {
			get { return (bool)GetValue(AllowScrollingProperty); }
			set { SetValue(AllowScrollingProperty, value); }
		}
		public ControlTemplate ButtonDownTemplate {
			get { return (ControlTemplate)GetValue(ButtonDownTemplateProperty); }
			set { SetValue(ButtonDownTemplateProperty, value); }
		}
		public ControlTemplate ButtonUpTemplate {
			get { return (ControlTemplate)GetValue(ButtonUpTemplateProperty); }
			set { SetValue(ButtonUpTemplateProperty, value); }
		}
		public NavBarViewKind NavBarViewKind {
			get { return (NavBarViewKind)GetValue(NavBarViewKindProperty); }
			set { SetValue(NavBarViewKindProperty, value); }
		}
		public ControlTemplate NotAllowScrollingTemplate {
			get { return (ControlTemplate)GetValue(NotAllowScrollingTemplateProperty); }
			set { SetValue(NotAllowScrollingTemplateProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public ScrollMode ScrollMode {
			get { return (ScrollMode)GetValue(ScrollModeProperty); }
			set { SetValue(ScrollModeProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateAnimations();
			SetBindings();
			UpdateOrientationStates();
			OnNavBarViewKindChanged();
		}
		void OnAllowScrollingChanged() {
			UpdateScrollingTemplate();
		}
		void OnNavBarViewKindChanged() {
			if(NavBarViewKind == NavBarViewKind.ExplorerBar)
				VisualStateManager.GoToState(this, "ExplorerBar", false);
			else
				VisualStateManager.GoToState(this, "NavBarViewKindNormal", false);
		}
		void OnOrientationChanged() {
			UpdateOrientationStates();
		}
		protected internal virtual void OnScrollModeChanged() { }
		protected internal virtual void SetBindings() {
			string strView = DataContext is NavBarGroup ? "NavBar.View." : "View.";
			SetBinding(OrientationProperty, new Binding(strView + "Orientation"));
			SetBinding(NavBarViewKindProperty, new Binding(strView + "NavBarViewKind"));
		}
		void UpdateAnimations() {
			NavBarVisualStateHelper.UpdateStates(this, "OrientationStates");
		}
		protected internal void UpdateScrollingTemplate() {
			UpdateScrollingTemplate(AllowScrolling && ScrollMode != NavBar.ScrollMode.None);
		}
		protected internal void UpdateScrollingTemplate(bool allowScrolling) {
			if(allowScrolling)
				ClearValue(ScrollableContentControl.TemplateProperty);
			else {
				SetBindings();
				Binding b = new Binding("NotAllowScrollingTemplate");
				b.Source = this;
				SetBinding(ScrollableContentControl.TemplateProperty, b);
			}
		}
		void UpdateOrientationStates() {
			if(Orientation == Orientation.Horizontal)
				VisualStateManager.GoToState(this, "Horizontal", false);
			else
				VisualStateManager.GoToState(this, "Vertical", false);
		}
	}
	public class NavBarRepeatButton : RepeatButton {
		#region DependencyProperties
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemsPanelOrientationProperty;
		internal static readonly DependencyProperty GroupPositionProperty;
		internal static readonly DependencyProperty NavBarViewKindProperty;
		#endregion
		DispatcherTimer timer;
		int delayCore;
		static NavBarRepeatButton() {
			GroupPositionProperty = DependencyPropertyManager.Register("GroupPosition", typeof(GroupPosition), typeof(NavBarRepeatButton), new PropertyMetadata(GroupPosition.Middle, (d, e) => ((NavBarRepeatButton)d).OnGroupPositionPropertyChanged()));
			ItemsPanelOrientationProperty = DependencyPropertyManager.Register("ItemsPanelOrientation", typeof(Orientation), typeof(NavBarRepeatButton), new PropertyMetadata(Orientation.Vertical, (d,e)=> ((NavBarRepeatButton)d).OnItemsPanelOrientationPropertyChanged()));
			NavBarViewKindProperty = DependencyPropertyManager.Register("NavBarViewKind", typeof(NavBarViewKind), typeof(NavBarRepeatButton), new PropertyMetadata(NavBarViewKind.ExplorerBar, (d, e) => ((NavBarRepeatButton)d).OnNavBarViewKindPropertyChanged()));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavBarRepeatButton), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavBarRepeatButton)d).OnOrientationPropertyChanged()));
		}		
		public NavBarRepeatButton() {
			this.SetDefaultStyleKey(typeof(NavBarRepeatButton));
			Delay = 200;
		}
		#region Properties
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public Orientation ItemsPanelOrientation {
			get { return (Orientation)GetValue(ItemsPanelOrientationProperty); }
			set { SetValue(ItemsPanelOrientationProperty, value); }
		}
		internal GroupPosition GroupPosition {
			get { return (GroupPosition)GetValue(GroupPositionProperty); }
			set { SetValue(GroupPositionProperty, value); }
		}
		internal NavBarViewKind NavBarViewKind {
			get { return (NavBarViewKind)GetValue(NavBarViewKindProperty); }
			set { SetValue(NavBarViewKindProperty, value); }
		}		
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBindings();
			UpdateOrientationStates();
		}
		protected internal virtual void OnGroupPositionPropertyChanged() {
			UpdateGroupPositionStates();
		}
		protected internal virtual void OnItemsPanelOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		protected internal virtual void OnNavBarViewKindPropertyChanged() {
			UpdateOrientationStates();
		}
		protected internal virtual void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			if (ClickMode != ClickMode.Hover) {
				base.OnMouseEnter(e);
				return;
			}
			SetDelayOnMouseEnter(e);
		}
		void OnTimeout(MouseEventArgs e) {
			Delay = 0;
			timer.Stop();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			if (ClickMode == ClickMode.Hover) {
				Delay = delayCore;
				if (timer != null)
					timer.Stop();
			}
			base.OnMouseLeave(e);
		}
		void SetBindings() {									
			SetBinding(ItemsPanelOrientationProperty, new Binding((DataContext is NavBarGroup ? "NavBar.": "") + "View.ItemsPanelOrientation"));
			SetBinding(NavBarViewKindProperty, new Binding((DataContext is NavBarGroup ? "NavBar." : "") + "View.NavBarViewKind"));
			SetBinding(GroupPositionProperty, new Binding("(0)") { Path = new PropertyPath(NavBarPositionPanel.GroupPositionProperty) });
		}
		void SetDelayOnMouseEnter(MouseEventArgs e) {
			delayCore = Delay;
			if (timer == null) {
				timer = new DispatcherTimer();
				timer.Interval = new TimeSpan(0, 0, 0, 0, Delay);
				timer.Tick += (s, ev) => this.OnTimeout(e);
			}
			timer.Start();
		}
		string UpdateGroupPositionStates() {
			if (NavBarViewKind == NavBarViewKind.SideBar && (GroupPosition == NavBar.GroupPosition.Last || GroupPosition == NavBar.GroupPosition.Single)) {
				return "Last";
			}
			return String.Empty;
		}
		void UpdateOrientationStates() {
			String state = String.Empty;
			switch (NavBarViewKind) {
				case NavBarViewKind.ExplorerBar:
					state = "ExplorerBar";
					break;
				case NavBarViewKind.NavigationPane:
					state = "NavigationPane";
					break;
				case NavBarViewKind.SideBar:
					state = "SideBar";
					break;
			}
			if (Orientation == Orientation.Vertical && ItemsPanelOrientation == Orientation.Horizontal)
				state += "ViewVerticalAndItemsPanelHorizontal";
			else if (Orientation == Orientation.Horizontal && ItemsPanelOrientation == Orientation.Vertical)
				state += "ViewHorizontalAndItemsPanelVertical";
			state += UpdateGroupPositionStates();
			VisualStateManager.GoToState(this, state, false);
		}
	}
	public class NavBarSmoothScroller : SmoothScroller {				
		public static readonly DependencyProperty ButtonDownProperty;
		public static readonly DependencyProperty ButtonUpProperty;
		static NavBarSmoothScroller() {
			ButtonUpProperty = DependencyPropertyManager.Register("ButtonUp", typeof(NavBarRepeatButton), typeof(NavBarSmoothScroller), new PropertyMetadata(null, (d, e) => ((NavBarSmoothScroller)d).OnButtonUpPropertyChanged()));
			ButtonDownProperty = DependencyPropertyManager.Register("ButtonDown", typeof(NavBarRepeatButton), typeof(NavBarSmoothScroller), new PropertyMetadata(null, (d, e) => ((NavBarSmoothScroller)d).OnButtonDownPropertyChanged()));
		}
		public NavBarRepeatButton ButtonDown {
			get { return (NavBarRepeatButton)GetValue(ButtonDownProperty); }
			set { SetValue(ButtonDownProperty, value); }
		}
		public NavBarRepeatButton ButtonUp {
			get { return (NavBarRepeatButton)GetValue(ButtonUpProperty); }
			set { SetValue(ButtonUpProperty, value); }
		}
		void OnButtonDownPropertyChanged() {
			UpdateAllowScrolling();
		}
		void OnButtonUpPropertyChanged() {
			UpdateAllowScrolling();
		}		
		protected override Vector GetMinScrollDeltaOverride() {
			var customValue = ScrollingSettings.GetMinScrollValue(this);
			if (!customValue.HasValue)
				return new Vector(3, 3);
			return new Vector(customValue.Value, customValue.Value);
		}
		protected override void UpdateAllowScrolling() {
			base.UpdateAllowScrolling();
			if (ButtonDown == null || ButtonUp == null)
				return;
			ButtonDown.Visibility = AllowScrollDown || AllowScrollUp ? Visibility.Visible : Visibility.Collapsed;
			ButtonUp.Visibility = AllowScrollDown || AllowScrollUp ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
