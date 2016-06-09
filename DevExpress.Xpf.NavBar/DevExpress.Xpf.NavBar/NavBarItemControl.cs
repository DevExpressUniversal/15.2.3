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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Controls;
using System;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOverState", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "PressedState", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "EnabledState", GroupName = "DisabledStates")]
	[TemplateVisualState(Name = "DisabledState", GroupName = "DisabledStates")]
	[TemplateVisualState(Name = "Unselected", GroupName = "SelectedStates")]
	[TemplateVisualState(Name = "Selected", GroupName = "SelectedStates")]
	[TemplateVisualState(Name = "Vertical", GroupName = "ViewOrientationStates")]
	[TemplateVisualState(Name = "Horizontal", GroupName = "ViewOrientationStates")]
	public partial class NavBarItemControl : ButtonBase, INavBarContainer {
		#region DependencyProperties
		internal static readonly DependencyProperty IsEnabledCoreProperty;
		internal static readonly DependencyProperty IsSelectedCoreProperty;
		internal static readonly DependencyProperty IsMouseOverCoreProperty;
		internal static readonly DependencyProperty ItemsPanelOrientationProperty;
		internal static readonly DependencyProperty ViewOrientationProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty LayoutSettingsProperty;
		public static readonly DependencyProperty ImageSettingsProperty;
		public static readonly DependencyProperty FontSettingsProperty;
		public static readonly DependencyProperty NavBarProperty;		
		#endregion
		public NavBarItemControl() {
			requestContainerHandler = new RequestContainersWeakEventHandler(this, (container, sender, args)=> args.AddContainer(container));
			this.SetDefaultStyleKey(typeof(NavBarItemControl));
			IsEnabledChanged += OnIsEnabledChanged;
		}
		static NavBarItemControl() {
			IsEnabledCoreProperty = DependencyPropertyManager.Register("IsEnabledCore", typeof(bool), typeof(NavBarItemControl), new PropertyMetadata(true, (d, e) => ((NavBarItemControl)d).OnIsEnabledCorePropertyChanged()));
			IsSelectedCoreProperty = DependencyPropertyManager.Register("IsSelectedCore", typeof(bool), typeof(NavBarItemControl), new PropertyMetadata(false, (d, e) => ((NavBarItemControl)d).OnIsSelectedCorePropertyChanged()));
			IsMouseOverCoreProperty = DependencyPropertyManager.Register("IsMouseOverCore", typeof(bool), typeof(NavBarItemControl), new PropertyMetadata(false, (d, e) => ((NavBarItemControl)d).OnIsMouseOverCorePropertyChanged()));
			ItemsPanelOrientationProperty = DependencyPropertyManager.Register("ItemsPanelOrientation", typeof(Orientation), typeof(NavBarItemControl), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavBarItemControl)d).OnOrientationPropertyChanged()));
			ViewOrientationProperty = DependencyPropertyManager.Register("ViewOrientation", typeof(Orientation), typeof(NavBarItemControl), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavBarItemControl)d).OnOrientationPropertyChanged()));			
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DisplayMode), typeof(NavBarItemControl), new FrameworkPropertyMetadata(DisplayMode.Default));
			LayoutSettingsProperty = DependencyPropertyManager.Register("LayoutSettings", typeof(LayoutSettings), typeof(NavBarItemControl), new FrameworkPropertyMetadata(null));
			ImageSettingsProperty = DependencyPropertyManager.Register("ImageSettings", typeof(ImageSettings), typeof(NavBarItemControl), new FrameworkPropertyMetadata(null));
			FontSettingsProperty = DependencyPropertyManager.Register("FontSettings", typeof(FontSettings), typeof(NavBarItemControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFontSettingsPropertyChanged)));
			NavBarProperty = DependencyPropertyManager.Register("NavBar", typeof(NavBarControl), typeof(NavBarItemControl), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarItemControl)d).OnNavBarChanged((NavBarControl)e.OldValue)));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(NavBarItemControl), typeof(DevExpress.Xpf.NavBar.Automation.NavBarItemControlAutomationPeer), owner => new DevExpress.Xpf.NavBar.Automation.NavBarItemControlAutomationPeer((NavBarItemControl)owner));
			TextElementHelper<NavBarItemControl>.OverrideMetadata(iControl => iControl.DataContext as NavBarItem);		
		}		
		protected static void OnFontSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItemControl)d).OnFontSettingsChanged((FontSettings)e.OldValue);
		}
		System.ComponentModel.PropertyChangedEventHandler propertyChangedEventHandler = null;
		protected virtual void OnFontSettingsChanged(FontSettings oldValue) {
			if(oldValue != null && propertyChangedEventHandler!=null) {
				oldValue.WeakPropertyChanged -= propertyChangedEventHandler;
			}
			if(FontSettings != null) {
				propertyChangedEventHandler = new System.ComponentModel.PropertyChangedEventHandler(OnFontSettingsPropertyChanged);
				FontSettings.WeakPropertyChanged += propertyChangedEventHandler;
				FontSettings.CheckAllProperties(this);
			}
		}
		protected virtual void OnNavBarChanged(NavBarControl oldValue) {
			this.OnNavBarChanged(oldValue, NavBar);
		}
		void OnFontSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			FontSettings.SetValueIfNotDefaultOrClearIfUnset(this, e.PropertyName);
		}
		#region Properties
		internal new bool IsEnabledCore {
			get { return (bool)GetValue(IsEnabledCoreProperty); }
			set { SetValue(IsEnabledCoreProperty, value); }
		}
		internal bool IsMouseOverCore {
			get { return (bool)GetValue(IsMouseOverCoreProperty); }
			set { SetValue(IsMouseOverCoreProperty, value); }
		}
		internal bool IsSelectedCore {
			get { return (bool)GetValue(IsSelectedCoreProperty); }
			set { SetValue(IsSelectedCoreProperty, value); }
		}
		public Orientation ItemsPanelOrientation {
			get { return (Orientation)GetValue(ItemsPanelOrientationProperty); }
			set { SetValue(ItemsPanelOrientationProperty, value); }
		}
		public Orientation ViewOrientation {
			get { return (Orientation)GetValue(ViewOrientationProperty); }
			set { SetValue(ViewOrientationProperty, value); }
		}
		public ImageSettings ImageSettings {
			get { return (ImageSettings)GetValue(ImageSettingsProperty); }
			set { SetValue(ImageSettingsProperty, value); }
		}
		public LayoutSettings LayoutSettings {
			get { return (LayoutSettings)GetValue(LayoutSettingsProperty); }
			set { SetValue(LayoutSettingsProperty, value); }
		}
		public DisplayMode DisplayMode {
			get { return (DisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public FontSettings FontSettings {
			get { return (FontSettings)GetValue(FontSettingsProperty); }
			set { SetValue(FontSettingsProperty, value); }
		}
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); }
			set { SetValue(NavBarProperty, value); }
		}
		protected internal object Source { get; set; }
		#endregion
		protected ContentControl BorderControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			BorderControl = GetTemplateChild("PART_Border") as ContentControl;
			NavBarVisualStateHelper.UpdateStates(this, "DisabledStates");			
			Dispatcher.BeginInvoke(new Action(() => BorderControl.Do(x => NavBarVisualStateHelper.UpdateStates(x, "DisabledStates"))));
			SetBindings();
			UpdateVisualState();
		}
		protected virtual void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateVisualState();
		}
		protected override void OnIsPressedChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			base.OnIsPressedChanged(e);
			UpdateVisualState();
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			UpdateVisualState();
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			FrameworkElement nFe = newContent as FrameworkElement;
			FrameworkContentElement nFce = newContent as FrameworkContentElement;
			if(nFe != null && nFe.Parent != null || nFce != null && nFce.Parent != null) {
				base.RemoveLogicalChild(oldContent);
				return;
			}
			base.OnContentChanged(oldContent, newContent);
		}
		bool GoToState(string stateName) {
			Dispatcher.BeginInvoke(new Action(() => { if (BorderControl != null) VisualStateManager.GoToState(BorderControl, stateName, false); }));
			return VisualStateManager.GoToState(this, stateName, false);
		}
		void OnIsEnabledCorePropertyChanged() {
			UpdateVisualState();
		}
		void OnIsMouseOverCorePropertyChanged() {
			UpdateVisualState();
		}
		void OnIsSelectedCorePropertyChanged() {
			GoToState(IsSelectedCore ? "Selected" : "Unselected");
		}
		void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		void SetBindings() {
			this.SetBinding(IsEnabledCoreProperty, new Binding("IsEnabled"));
			this.SetBinding(IsSelectedCoreProperty, new Binding("IsSelected"));
			this.SetBinding(IsMouseOverCoreProperty, new Binding("IsMouseOver") { Source = this });
			this.SetBinding(ItemsPanelOrientationProperty, new Binding("Group.NavBar.View.ItemsPanelOrientation"));
			this.SetBinding(ViewOrientationProperty, new Binding("Group.NavBar.View.Orientation"));
		}
		void UpdateOrientationStates() {
			string state = String.Empty;
			state += ViewOrientation == Orientation.Vertical ? "ViewVertical" : "ViewHorizontal";
			state += ItemsPanelOrientation == Orientation.Vertical ? "AndItemsPanelVertical" : "AndItemsPanelHorizontal";
			VisualStateManager.GoToState(this, state, false);
		}
		void UpdateVisualState() {
			NavBarItem item = this.DataContext as NavBarItem;
			if (item == null)
				return;
			if (!IsEnabledCore) {
				GoToState("DisabledState");
				if (!item.Group.NavBar.AllowSelectDisabledItem)
					return;
			} else
				GoToState("EnabledState");
			if (IsPressed && !IsSelectedCore)
				GoToState("PressedState");
			else if (IsMouseOver)
				GoToState("MouseOverState");
			else if (!IsMouseOver && IsSelectedCore) {
				GoToState("Normal");
				GoToState("Unselected");
				GoToState("Selected");
			} else
				GoToState("Normal");
		}
#if DEBUGTEST
		internal void PerformClick() {
			OnClick();
		}
#endif
		#region INavBarContainer Members
		readonly RequestContainersWeakEventHandler requestContainerHandler;
		RequestContainersWeakEventHandler INavBarContainer.RequestContainerHandler {
			get { return requestContainerHandler; }
		}
		#endregion
	}
}
