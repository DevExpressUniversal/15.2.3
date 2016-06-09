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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	[TemplateVisualState(Name = "ExplorerBarFirst", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarFirst0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarMiddle", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarMiddle0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarLast", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarLast0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarSingle", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "ExplorerBarSingle0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "IsActiveTrue", GroupName = "IsActiveStates")]
	[TemplateVisualState(Name = "IsActiveFalse", GroupName = "IsActiveStates")]
	public class NavBarGroupHeader : Button {
		public static readonly DependencyProperty NavBarViewKindProperty;
		public static readonly DependencyProperty AnimationProgressProperty;
		public static readonly DependencyProperty GroupPositionProperty;
		internal static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty ShowBorderProperty;		
		static NavBarGroupHeader() {
			AnimationProgressProperty = DependencyPropertyManager.Register("AnimationProgress", typeof(double), typeof(NavBarGroupHeader), new PropertyMetadata(0.0, (d, e) => ((NavBarGroupHeader)d).OnAnimationProgressPropertyChanged()));
			GroupPositionProperty = DependencyPropertyManager.Register("GroupPosition", typeof(GroupPosition), typeof(NavBarGroupHeader), new PropertyMetadata(GroupPosition.First, (d, e) => ((NavBarGroupHeader)d).OnGroupPositionPropertyChanged()));
			IsActiveProperty = DependencyPropertyManager.Register("IsActive", typeof(bool), typeof(NavBarGroupHeader), new PropertyMetadata(false, (d, e) => ((NavBarGroupHeader)d).OnIsActivePropertyChanged()));
			NavBarViewKindProperty = DependencyPropertyManager.Register("NavBarViewKind", typeof(NavBarViewKind), typeof(NavBarGroupHeader), new PropertyMetadata((d, e) => ((NavBarGroupHeader)d).OnNavBarViewKindPropertyChanged()));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavBarGroupHeader), new FrameworkPropertyMetadata(true, (d, e) => ((NavBarGroupHeader)d).OnShowBorderChanged((bool)e.OldValue)));
			IsManipulationEnabledProperty.OverrideMetadata(typeof(NavBarGroupHeader), new FrameworkPropertyMetadata(true));
			FocusableProperty.OverrideMetadata(typeof(NavBarGroupHeader), new FrameworkPropertyMetadata(false));
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
			if(point==null || !(object.Equals(e.GetTouchPoint(this).Position, point.Position)))
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
		#region Properties
		public double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
		public GroupPosition GroupPosition {
			get { return (GroupPosition)GetValue(GroupPositionProperty); }
			set { SetValue(GroupPositionProperty, value); }
		}
		public NavBarViewKind NavBarViewKind {
			get { return (NavBarViewKind)GetValue(NavBarViewKindProperty); }
			set { SetValue(NavBarViewKindProperty, value); }
		}
		internal bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}		
		protected internal ImageAndTextContentPresenter GroupHeaderLabel { get; private set; }
		protected internal ExplorerBarExpandButton ExplorerBarExpandButton { get; private set; }
		protected internal NavPaneExpandButton NavPaneExpandButton { get; private set; }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Initialize();
		}
		public NavBarGroupHeader() {
			Internal.NavBarGroupHelper.SetIsGroupHeader(this, true);
		}
		protected internal virtual void Initialize() {
			ExplorerBarExpandButton = (ExplorerBarExpandButton)GetTemplateChild("explorerBarExpandButton");
			NavPaneExpandButton = (NavPaneExpandButton)GetTemplateChild("navPaneExpandButton");
			GroupHeaderLabel = (ImageAndTextContentPresenter)GetTemplateChild("groupHeaderLabel");
			SetBindings();
			OnNavBarViewKindChanged();
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
			string state = String.Empty;
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
			if (ShowBorder) {
				if (GroupPosition == GroupPosition.First)
					state += "First";
				else if (GroupPosition == GroupPosition.Middle)
					state += "Middle";
				else if (GroupPosition == GroupPosition.Last)
					state += "Last";
				else if (GroupPosition == GroupPosition.Single)
					state += "Single";
				if (AnimationProgress == 0)
					state += "0";
			} else {
				state += "NoBorder";
			}
			VisualStateManager.GoToState(this, state, false);
		}
		void OnAnimationProgressPropertyChanged() {
			UpdateVisualState();
		}
		void OnGroupPositionPropertyChanged() {
			UpdateVisualState();
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			UpdateVisualState();
		}
		void OnNavBarViewKindPropertyChanged() {
			OnNavBarViewKindChanged();
			UpdateVisualState();
		}
		bool isMouseOver = false;
		bool isMousePressed = false;
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			isMouseOver = true;
			UpdateCommonVisualState();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			isMouseOver = false;
			UpdateCommonVisualState();
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			isMousePressed = true;
			UpdateCommonVisualState();
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			isMousePressed = false;
			UpdateCommonVisualState();
		}
		protected virtual void UpdateCommonVisualState() {
			if(this.GetTemplatedParent() is NavPaneActiveGroupControl && (DataContext as NavBarGroup)!=null && ((NavBarGroup)DataContext).NavBar != null && ((NavBarGroup)DataContext).NavBar.View is NavigationPaneView) {
				VisualStateManager.GoToState(this, "NormalHeader", false);
				return;
			}
			if(!IsEnabled) {
				VisualStateManager.GoToState(this, "DisabledHeader", false);
				return;
			}
			if(isMousePressed){
				VisualStateManager.GoToState(this, "PressedHeader", false);
				return;
			}
			if(isMouseOver) {
				VisualStateManager.GoToState(this, "MouseOverHeader", false);
				return;
			}
			VisualStateManager.GoToState(this, "NormalHeader", false);
		}
		internal void SetBindings() {
			if (ExplorerBarExpandButton != null && NavPaneExpandButton != null) {
				BindingOperations.SetBinding(this, NavBarGroupHeader.NavBarViewKindProperty, new Binding("NavBar.View.NavBarViewKind"));
				BindingOperations.SetBinding(this, NavBarGroupHeader.ShowBorderProperty, new Binding("NavBar.View.ShowBorder"));
			}
			SetBinding(GroupPositionProperty, new Binding("(0)") { Path = new PropertyPath(NavBarPositionPanel.GroupPositionProperty) });
			SetBinding(IsActiveProperty, new Binding("IsActive"));
		}
		protected internal void OnIsActivePropertyChanged() {
			VisualStateManager.GoToState(this, IsActive ? "IsActiveTrue" : "IsActiveFalse", false);
		}
		protected internal virtual void OnNavBarViewKindChanged() {
			if(DataContext as NavBarGroup == null || ExplorerBarExpandButton == null || NavPaneExpandButton == null || (DataContext as NavBarGroup).NavBar == null)
				return;
			ExplorerBarExpandButton.SetValue(Button.VisibilityProperty, Visibility.Collapsed);
			NavPaneExpandButton.SetValue(Button.VisibilityProperty, Visibility.Collapsed);
			switch(NavBarViewKind) {
				case NavBarViewKind.ExplorerBar:
					ExplorerBarExpandButton.SetValue(Button.VisibilityProperty, Visibility.Visible);
					GroupHeaderLabel.Opacity = 1;
					break;
				case NavBarViewKind.NavigationPane:
					BindingOperations.SetBinding(NavPaneExpandButton, Button.VisibilityProperty, new Binding("NavBar.View.IsExpandButtonVisible") { Converter = new BooleanToVisibilityConverter() });
					BindingOperations.SetBinding(GroupHeaderLabel, ImageAndTextContentPresenter.OpacityProperty, new Binding() { Path = new PropertyPath(NavBarAnimationOptions.AnimationProgressProperty) });
					break;
				case NavBarViewKind.SideBar:
					GroupHeaderLabel.Opacity = 1;
					break;
				default:
					break;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size res = base.MeasureOverride(availableSize);
			DependencyObject owner = this.GetTemplatedParent();
			if(owner is NavBarGroupControl)
				((NavBarGroupControl)owner).MinDesiredSize = new Size(double.IsNaN(Width) ? res.Width : Width, double.IsNaN(Height) ? res.Height : Height);
			return res;
		}
	}
}
