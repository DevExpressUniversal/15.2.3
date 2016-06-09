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
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarCheckItemLinkControl : BarButtonItemLinkControl, IBarCheckItemLinkControl {
		#region Static
		public static readonly DependencyProperty IsTriStateBorderVisibleProperty;
		static readonly DependencyPropertyKey IsTriStateBorderVisiblePropertyKey;
		public static readonly DependencyProperty IsCheckVisibleProperty;
		static readonly DependencyPropertyKey IsCheckVisiblePropertyKey;
		public static readonly DependencyProperty IsCheckGlyphBorderVisibleProperty;
		static readonly DependencyPropertyKey IsCheckGlyphBorderVisiblePropertyKey;
		public static readonly DependencyProperty IsTriStateGlyphBorderVisibleProperty;
		static readonly DependencyPropertyKey IsTriStateGlyphBorderVisiblePropertyKey;
		static BarCheckItemLinkControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarCheckItemLinkControl), typeof(BarCheckItemLinkControlAutomationPeer), owner => new BarCheckItemLinkControlAutomationPeer((BarCheckItemLinkControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarCheckItemLinkControl), new FrameworkPropertyMetadata(typeof(BarCheckItemLinkControl)));
			IsTriStateBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTriStateBorderVisible", typeof(bool), typeof(BarCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsTriStateBorderVisiblePropertyChanged)));
			IsTriStateBorderVisibleProperty = IsTriStateBorderVisiblePropertyKey.DependencyProperty;
			IsCheckVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCheckVisible", typeof(bool), typeof(BarCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckVisiblePropertyChanged)));
			IsCheckVisibleProperty = IsCheckVisiblePropertyKey.DependencyProperty;
			IsCheckGlyphBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCheckGlyphBorderVisible", typeof(bool), typeof(BarCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckGlyphBorderVisiblePropertyChanged)));
			IsCheckGlyphBorderVisibleProperty = IsCheckGlyphBorderVisiblePropertyKey.DependencyProperty;
			IsTriStateGlyphBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTriStateGlyphBorderVisible", typeof(bool), typeof(BarCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsTriStateGlyphBorderVisiblePropertyChanged)));
			IsTriStateGlyphBorderVisibleProperty = IsTriStateGlyphBorderVisiblePropertyKey.DependencyProperty;
		}
		protected static void OnIsTriStateBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCheckItemLinkControl)d).OnIsTriStateBorderVisibleChanged();
		}
		protected static void OnIsCheckVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCheckItemLinkControl)d).OnIsCheckVisibleChanged();
		}
		protected static void OnIsCheckGlyphBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCheckItemLinkControl)d).OnIsCheckGlyphBorderVisibleChanged();
		}
		protected static void OnIsTriStateGlyphBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarCheckItemLinkControl)d).OnIsTriStateGlyphBorderVisibleChanged();
		}
		#endregion Static
		public BarCheckItemLinkControl() : base() {
		}
		public BarCheckItemLinkControl(BarCheckItemLink link) : base(link) {
		}
		protected internal override bool CloseSubMenuOnClick {
			get {
				if(ContainerType == LinkContainerType.RadialMenu && ButtonItem != null && !ButtonItem.CloseSubMenuOnClick.HasValue)
					return false;
				return base.CloseSubMenuOnClick;
			}
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarCheckItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarCheckItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return (key) => new BarCheckItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		public BarCheckItemLink CheckLink { get { return base.Link as BarCheckItemLink; } }
		protected BarCheckItem CheckItem { get { return Item as BarCheckItem; } }
		public bool IsTriStateBorderVisible {
			get { return (bool)GetValue(IsTriStateBorderVisibleProperty); }
			internal set { this.SetValue(IsTriStateBorderVisiblePropertyKey, value); }
		}
		public bool IsCheckVisible {
			get { return (bool)GetValue(IsCheckVisibleProperty); }
			internal set { this.SetValue(IsCheckVisiblePropertyKey, value); }
		}
		public bool IsTriStateGlyphBorderVisible {
			get { return (bool)GetValue(IsTriStateGlyphBorderVisibleProperty); }
			internal set { this.SetValue(IsTriStateGlyphBorderVisiblePropertyKey, value); }
		}
		public bool IsCheckGlyphBorderVisible {
			get { return (bool)GetValue(IsCheckGlyphBorderVisibleProperty); }
			internal set { this.SetValue(IsCheckGlyphBorderVisiblePropertyKey, value); }
		}
		protected override bool? IsChecked { get { return GetIsChecked(); } }
		protected virtual bool? GetIsChecked() {
			if(CheckItem != null)
				return CheckItem.IsChecked;
			if(CheckLink != null)
				return CheckLink.IsChecked;
			return false;
		}
		protected override void UpdateItemBorderAndContent() {
			base.UpdateItemBorderAndContent();
			if(!ShowCustomizationBorder && !ShowHotBorder && !ShowPressedBorder)
				UpdateCheckVisualState();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateCheckBorders();
			UpdateState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateCheckBorders();
			UpdateState();
			UpdateCheckVisualState();
		}
		protected internal virtual void UpdateCheckBorders() {
			if(!IsLoaded)
				return;
			if(GetIsChecked().HasValue) {
				if(GetIsChecked().Value) {
					IsCheckVisible = !HasGlyph;
					IsCheckGlyphBorderVisible = HasGlyph;
					IsTriStateBorderVisible = false;
					IsTriStateGlyphBorderVisible = false;
				}
				else {
					IsCheckVisible = false;
					IsCheckGlyphBorderVisible = false;
					IsTriStateBorderVisible = false;
					IsTriStateGlyphBorderVisible = false;
				}
			}
			else {
				IsCheckVisible = false;
				IsCheckGlyphBorderVisible = false;
				IsTriStateBorderVisible = !HasGlyph;
				IsTriStateGlyphBorderVisible = HasGlyph;
			}
			UpdateLayoutPanelCheckInfo();
		}		
		protected internal virtual void UpdateCheckVisualState() {
			UpdateActualIsChecked();
			if(LayoutPanel == null) return;
			UpdateItemVisualState();
			if(GetIsChecked().HasValue) {
				if(GetIsChecked().Value) {
					if(!IsLinkControlInMenu) {
					}
				} else {
				}
			} else {
			}
			UpdateLayoutPanelCheckInfo();						
		}
		protected override void UpdateLayoutPanel() {
			if(LayoutPanel == null) return;
			base.UpdateLayoutPanel();
			UpdateLayoutPanelCheckInfo();
		}
		protected virtual void UpdateLayoutPanelCheckInfo() {
			if(LayoutPanel == null || LayoutPanel.ElementGlyphBackground == null) return;
			BarCheckItemLinkControlStatesProvider.SetCheckVisibility(LayoutPanel.ElementGlyphBackground, IsCheckVisible ? Visibility.Visible : Visibility.Collapsed);
			BarCheckItemLinkControlStatesProvider.SetGlyphCheckVisibility(LayoutPanel.ElementGlyphBackground, IsCheckGlyphBorderVisible ? Visibility.Visible : Visibility.Collapsed);
			BarCheckItemLinkControlStatesProvider.SetGlyphTriStateVisibility(LayoutPanel.ElementGlyphBackground, IsTriStateGlyphBorderVisible ? Visibility.Visible : Visibility.Collapsed);
			BarCheckItemLinkControlStatesProvider.SetTriStateVisibility(LayoutPanel.ElementGlyphBackground, IsTriStateBorderVisible ? Visibility.Visible : Visibility.Collapsed);
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
		}
		protected virtual void OnIsTriStateBorderVisibleChanged() {
			UpdateState();
		}
		protected virtual void OnIsCheckVisibleChanged() {
			UpdateState();
		}
		protected virtual void OnIsCheckGlyphBorderVisibleChanged() {
			UpdateState();
		}
		protected virtual void OnIsTriStateGlyphBorderVisibleChanged() {
			UpdateState();
		}
		protected virtual void UpdateState() {
			VisualStateManager.GoToState(this, IsTriStateBorderVisible ? "TriStateBorderVisible" : "TriStateBorderHidden", false);
			VisualStateManager.GoToState(this, IsCheckVisible ? "CheckVisible" : "CheckHidden", false);
			VisualStateManager.GoToState(this, IsTriStateGlyphBorderVisible ? "TriStateGlyphBorderVisible" : "TriStateGlyphBorderHidden", false);
			VisualStateManager.GoToState(this, IsCheckGlyphBorderVisible ? "GlyphCheckVisible" : "GlyphCheckHidden", false);
		}
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateCheckBorders();
			UpdateCheckVisualState();
			UpdateActualIsChecked();
		}
		protected internal void OnSourceIsCheckedChanged() {
			UpdateActualIsChecked();
			UpdateCheckBorders();
			UpdateCheckVisualState();			
		}
		protected virtual void UpdateActualIsChecked() {
			ClearValue(ActualIsCheckedPropertyKey);
			ActualIsChecked = GetIsChecked();
		}
		void IBarCheckItemLinkControl.OnSourceIsCheckedChanged() { OnSourceIsCheckedChanged(); }
	}
	public class BarCheckItemLinkControlStatesProvider {
		public static Visibility GetTriStateVisibility(DependencyObject obj) {
			return (Visibility)obj.GetValue(TriStateVisibilityProperty);
		}
		public static void SetTriStateVisibility(DependencyObject obj, Visibility value) {
			obj.SetValue(TriStateVisibilityProperty, value);
		}
		public static Visibility GetGlyphCheckVisibility(DependencyObject obj) {
			return (Visibility)obj.GetValue(GlyphCheckVisibilityProperty);
		}
		public static void SetGlyphCheckVisibility(DependencyObject obj, Visibility value) {
			obj.SetValue(GlyphCheckVisibilityProperty, value);
		}
		public static Visibility GetGlyphTriStateVisibility(DependencyObject obj) {
			return (Visibility)obj.GetValue(GlyphTriStateVisibilityProperty);
		}
		public static void SetGlyphTriStateVisibility(DependencyObject obj, Visibility value) {
			obj.SetValue(GlyphTriStateVisibilityProperty, value);
		}
		public static Visibility GetCheckVisibility(DependencyObject obj) {
			return (Visibility)obj.GetValue(CheckVisibilityProperty);
		}
		public static void SetCheckVisibility(DependencyObject obj, Visibility value) {
			obj.SetValue(CheckVisibilityProperty, value);
		}
		public static readonly DependencyProperty CheckVisibilityProperty =
			DependencyPropertyManager.RegisterAttached("CheckVisibility", typeof(Visibility), typeof(BarCheckItemLinkControlStatesProvider), new FrameworkPropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty GlyphTriStateVisibilityProperty =
			DependencyPropertyManager.RegisterAttached("GlyphTriStateVisibility", typeof(Visibility), typeof(BarCheckItemLinkControlStatesProvider), new FrameworkPropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty GlyphCheckVisibilityProperty =
			DependencyPropertyManager.RegisterAttached("GlyphCheckVisibility", typeof(Visibility), typeof(BarCheckItemLinkControlStatesProvider), new FrameworkPropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty TriStateVisibilityProperty =
			DependencyPropertyManager.RegisterAttached("TriStateVisibility", typeof(Visibility), typeof(BarCheckItemLinkControlStatesProvider), new FrameworkPropertyMetadata(Visibility.Collapsed));
	}
}
