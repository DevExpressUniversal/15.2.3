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
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarSplitCheckItemLinkControl : BarSplitButtonItemLinkControl, IBarCheckItemLinkControl {
		#region static
		public static readonly DependencyProperty IsTriStateBorderVisibleProperty;
		static readonly DependencyPropertyKey IsTriStateBorderVisiblePropertyKey;
		public static readonly DependencyProperty IsCheckVisibleProperty;
		static readonly DependencyPropertyKey IsCheckVisiblePropertyKey;
		public static readonly DependencyProperty IsCheckGlyphBorderVisibleProperty;
		static readonly DependencyPropertyKey IsCheckGlyphBorderVisiblePropertyKey;
		public static readonly DependencyProperty IsTriStateGlyphBorderVisibleProperty;
		static readonly DependencyPropertyKey IsTriStateGlyphBorderVisiblePropertyKey;
		static BarSplitCheckItemLinkControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarSplitCheckItemLinkControl), typeof(BarSplitCheckItemLinkControlAutomationPeer), owner => new BarSplitCheckItemLinkControlAutomationPeer((BarSplitCheckItemLinkControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarSplitCheckItemLinkControl), new FrameworkPropertyMetadata(typeof(BarSplitButtonItemLinkControl)));
			IsTriStateBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTriStateBorderVisible", typeof(bool), typeof(BarSplitCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsTriStateBorderVisiblePropertyChanged)));
			IsTriStateBorderVisibleProperty = IsTriStateBorderVisiblePropertyKey.DependencyProperty;
			IsCheckVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCheckVisible", typeof(bool), typeof(BarSplitCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckVisiblePropertyChanged)));
			IsCheckVisibleProperty = IsCheckVisiblePropertyKey.DependencyProperty;
			IsCheckGlyphBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCheckGlyphBorderVisible", typeof(bool), typeof(BarSplitCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckGlyphBorderVisiblePropertyChanged)));
			IsCheckGlyphBorderVisibleProperty = IsCheckGlyphBorderVisiblePropertyKey.DependencyProperty;
			IsTriStateGlyphBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTriStateGlyphBorderVisible", typeof(bool), typeof(BarSplitCheckItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsTriStateGlyphBorderVisiblePropertyChanged)));
			IsTriStateGlyphBorderVisibleProperty = IsTriStateGlyphBorderVisiblePropertyKey.DependencyProperty;
		}
		protected static void OnIsTriStateBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnIsCheckVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnIsCheckGlyphBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnIsTriStateGlyphBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		#endregion
		#region dep props
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
		#endregion
		public BarSplitCheckItemLinkControl()
			: base() {
				CreateBindings();
		}
		public BarSplitCheckItemLinkControl(BarSplitCheckItemLink link) : base(link) {
			CreateBindings();
		}
		protected internal override bool CloseSubMenuOnClick {
			get {
				if(ContainerType == LinkContainerType.RadialMenu && ButtonItem != null && !ButtonItem.CloseSubMenuOnClick.HasValue)
					return false;
				return base.CloseSubMenuOnClick;
			}
		}
		public BarSplitCheckItemLink SplitCheckLink { get { return base.Link as BarSplitCheckItemLink; } }
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override bool? IsChecked {
			get {				
				if(!(Item is BarSplitCheckItem))
					return false;
				return GetIsChecked();
			}
		}
		protected virtual bool? GetIsChecked() {
			if(!(Item is BarSplitCheckItem))
				return false;
			return ((BarSplitCheckItem)Item).IsChecked;
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarSplitCheckItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarSplitCheckItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return key => new BarSplitCheckItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		protected internal virtual void UpdateCheckVisualState() {
			if(GetIsChecked().HasValue) {
				if(GetIsChecked().Value) {
					ShowPressedBorder = true;
				} else {
					ShowPressedBorder = false;
				}
			} else {
				ShowPressedBorder = false;
			}
			UpdateCheckBorders();
			UpdateItemBorderAndContent();
			UpdateActualIsChecked();
		}		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateCheckBorders();
			UpdateCheckVisualState();
		}
		protected override void SetArrowBorderState(BorderState state) {			
			if(IsLinkInApplicationMenu || ContainerType == LinkContainerType.DropDownGallery || ContainerType == LinkContainerType.Menu) {
				base.SetArrowBorderState(state);
				return;
			}
			BorderState correctState = state;
			if((IsChecked==true) && state == BorderState.Normal) correctState = BorderState.Hover;
			base.SetArrowBorderState(correctState);
		}
		protected override void SetItemBorderState(BorderState state) {
			if(IsLinkInApplicationMenu || ContainerType == LinkContainerType.DropDownGallery || ContainerType == LinkContainerType.Menu) {
				base.SetItemBorderState(state);
				return;
			}
			BorderState correctState = state;
			if((IsChecked == true) && state == BorderState.Normal) correctState = BorderState.Pressed;
			if((IsChecked == true) && state == BorderState.Hover) correctState = BorderState.Pressed;
			base.SetItemBorderState(correctState);
		}
		protected override bool ShouldClickOnMouseLeftButtonUp(bool wasPressed) {
			if(ContainerType != LinkContainerType.RadialMenu)
				return base.ShouldClickOnMouseLeftButtonUp(wasPressed);			
			return wasPressed && !ActualActAsDropDown;
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
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateCheckBorders();
		}
		protected internal virtual void OnSourceIsCheckedChanged() {
			UpdateCheckVisualState();
			UpdateActualIsChecked();
		}
		protected virtual void UpdateActualIsChecked() {			
			ClearValue(ActualIsCheckedPropertyKey);
			ActualIsChecked = GetIsChecked();
		}
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualIsChecked();
			UpdateCheckVisualState();			
		}
		void IBarCheckItemLinkControl.OnSourceIsCheckedChanged() { OnSourceIsCheckedChanged(); }
	}
}
