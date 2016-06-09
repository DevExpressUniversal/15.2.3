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
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Automation;
namespace DevExpress.Xpf.Bars {
	public class BarButtonItemLinkControl : BarItemLinkControl {
		#region static
		static BarButtonItemLinkControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarButtonItemLinkControl), typeof(BarButtonItemLinkControlAutomationPeer), owner => new BarButtonItemLinkControlAutomationPeer((BarButtonItemLinkControl)owner));
			KeyboardNavigation.AcceptsReturnProperty.OverrideMetadata(typeof(BarButtonItemLinkControl), new FrameworkPropertyMetadata(true));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarButtonItemLinkControl), new FrameworkPropertyMetadata(typeof(BarButtonItemLinkControl)));
		}
		#endregion
		public BarButtonItemLinkControl() {
		}
		protected BarButtonItemLinkControl(BarItemLink link)
			: base(link) {
		}
		public BarButtonItemLinkControl(BarButtonItemLink link)
			: this((BarItemLink)link) { }
		protected internal virtual bool CloseSubMenuOnClick { get { return (ButtonItem == null) || !ButtonItem.CloseSubMenuOnClick.HasValue ? true : ButtonItem.CloseSubMenuOnClick.Value; } }
		protected virtual bool ShouldPressItem() { return true; }
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}				
		protected internal override void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDownCore(e);
			MouseButtonState state = e.ButtonState;
			if(state == MouseButtonState.Pressed) {
				if (ContainerType != LinkContainerType.Menu && ContainerType != LinkContainerType.MainMenu)
					MouseHelper.Capture(this);
				if(state == MouseButtonState.Pressed && ShouldPressItem())
					IsPressed = true;
			}
			e.Handled = true;
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarButtonItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarButtonItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return (key) => new BarButtonItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		protected override void UpdateLayoutPanel() {
			base.UpdateLayoutPanel();
			if(LayoutPanel == null) return;
			LayoutPanel.ShowFirstBorder = true;
			LayoutPanel.IsFirstBorderActive = true;
			LayoutPanel.ShowGlyphBackground = true;
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);
			if(IsLinkControlInMenu)
				e.Handled = true;
		}
		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
			base.OnMouseRightButtonUp(e);
			if(IsLinkControlInMenu)
				e.Handled = true;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			e.Handled = true;
			bool wasPressed = IsPressed;
			if(MouseHelper.Captured == this)
				MouseHelper.ReleaseCapture(this);
			wasPressed &= IsMouseOver;
			if(ShouldClickOnMouseLeftButtonUp(wasPressed) && IsVisible)
				OnClick();
			if(MouseHelper.Captured == this)
				MouseHelper.ReleaseCapture(this);
			base.OnMouseLeftButtonUp(e);
		}
		protected virtual bool ShouldClickOnMouseLeftButtonUp(bool wasPressed) {
			if(ContainerType == LinkContainerType.RadialMenu && !ActualIsContentEnabled)
				return false;
			return wasPressed || (IsMouseOver && IsLinkControlInMenu && mouseMoved);
		}
		Point oldPosition = new Point(-10, -10);
		bool mouseMoved = false;
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);			
			if(oldPosition.X==-10 && oldPosition.Y == -10) {
				oldPosition = e.GetPosition(this);
			} else {
				if (e.GetPosition(this) != oldPosition)
					mouseMoved = true;
			}			
			CheckUpdateIsPressed(e);
		}		
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			mouseMoved = false;
			oldPosition = new Point(-10, -10);
			if (MouseHelper.Captured == this || !ShouldUnpressOnMouseLeave(e))
				return;
			if (IsLinkControlInMenu || IsLinkControlInMainMenu)
				UnpressOnMouseLeave(e);
		}
		protected virtual void UnpressOnMouseLeave(MouseEventArgs e) {
			IsPressed = false;
		}
		protected virtual bool ShouldUnpressOnMouseLeave(MouseEventArgs e) { return true; }
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			if(e.OriginalSource == this) {
				IsPressed = false;
			}
		}
		protected internal override bool ProcessKeyDown(KeyEventArgs e) {
			if ((e.Key == Key.Return) && ((bool)GetValue(KeyboardNavigation.AcceptsReturnProperty)) ||
				e.Key == Key.Space) {
				OnClick();
				return true;
			}
			return base.ProcessKeyDown(e);
		}
		public BarButtonItemLink ButtonLink { get { return base.Link as BarButtonItemLink; } }
		protected BarButtonItem ButtonItem { get { return Item as BarButtonItem; } }
	}
}
