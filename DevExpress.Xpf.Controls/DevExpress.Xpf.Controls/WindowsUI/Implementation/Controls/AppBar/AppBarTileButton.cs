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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AppBarTileButton : AppBarButtonBase, IAppBarElement {
		#region static
		static AppBarTileButton() {
			var dProp = new DependencyPropertyRegistrator<AppBarTileButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		AppBarTileButton() {
		}
		ButtonBase PartFlyoutButton;
		ButtonBase PartButton;
		public override void OnApplyTemplate() {
			if(PartFlyoutButton != null) PartFlyoutButton.Click -= PartFlyoutButton_Click;
			if(PartButton != null) PartButton.Click -= PartButton_Click;
			base.OnApplyTemplate();
			PartFlyoutButton = GetTemplateChild("PART_FlyoutButton") as ButtonBase;
			if(PartFlyoutButton != null)
				PartFlyoutButton.Click += PartFlyoutButton_Click;
			PartButton = GetTemplateChild("PART_Button") as ButtonBase;
			if(PartButton != null) PartButton.Click += PartButton_Click;
		}
		private void InvokeClick() {
			ButtonAutomationPeer peer = new ButtonAutomationPeer(this);
			IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			invokeProv.Invoke();
		}
		void PartButton_Click(object sender, RoutedEventArgs e) {
			InvokeClick();
		}
		protected override bool ShowFlyoutOnClick { get { return false; } }
		void PartFlyoutButton_Click(object sender, RoutedEventArgs e) {
			ProcessFlyoutClick();
		}
		protected override void PrepareFlyout() {
			base.PrepareFlyout();
			if(HasFlyout) {
				Flyout.IndicatorTarget = this;
				var appBar = LayoutHelper.FindParentObject<AppBar>(this);
				if(appBar != null) Flyout.PlacementTarget = appBar;
			}
		}
		#region IAppBarElement Members
		bool IAppBarElement.IsCompact {
			get { return false; }
			set { }
		}
		#endregion
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class AppBarTileButtonArrowBorder : System.Windows.Controls.Primitives.ButtonBase {
		public AppBarTileButtonArrowBorder() {
			DefaultStyleKey = typeof(AppBarTileButtonArrowBorder);
		}
	}
	public class AppBarTileButtonBorder : System.Windows.Controls.Primitives.ButtonBase {
		public AppBarTileButtonBorder() {
			DefaultStyleKey = typeof(AppBarTileButtonBorder);
		}
	}
}
