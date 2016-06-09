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
using System.IO;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
namespace DevExpress.WinRTPresenter {
	public sealed partial class AboutFlyoutView : UserControl {
		SettingsCommand showFlayoutCommandCore;
		public SettingsCommand ShowFlayoutCommand {
			get {
				if(showFlayoutCommandCore == null) {
					showFlayoutCommandCore = CreateCommand();
				}
				return showFlayoutCommandCore;
			}
		}
		public AboutFlyoutView() {
			this.InitializeComponent();
		}
		SettingsCommand CreateCommand() {
			return new SettingsCommand("", "About", new Windows.UI.Popups.UICommandInvokedHandler(OnCommandInvoked));
		}
		Popup popup;
		private void OnCommandInvoked(Windows.UI.Popups.IUICommand command) {
			popup = new Popup();
			popup.Closed += OnPopupClosed;
			Window.Current.Activated += OnWindowActivated;
			popup.IsLightDismissEnabled = true;
			popup.Child = this;
			popup.ChildTransitions = new TransitionCollection();
			popup.ChildTransitions.Add(new PaneThemeTransition());
			((FrameworkElement)popup.Child).Width = 346;
			((FrameworkElement)popup.Child).Height = Window.Current.Bounds.Height;
			popup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - 346) : 0);
			popup.SetValue(Canvas.TopProperty, 0);
			popup.Closed += OnPopupClosed;
			popup.IsOpen = true;
		}
		private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e) {
			if(e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
				popup.IsOpen = false;
		}
		void OnPopupClosed(object sender, object e) {
			popup.Closed -= OnPopupClosed;
			Window.Current.Activated -= OnWindowActivated;
		}
		private void Button_Click_1(object sender, RoutedEventArgs e) {
			if(popup!= null) 
				popup.IsOpen = false;
		}
	}
}
