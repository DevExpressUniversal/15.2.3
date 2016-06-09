#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class CloseMdiChildWindowController : WindowController {
		private bool isWindowClosingLocked;
		private void View_Disposing(object sender, System.ComponentModel.CancelEventArgs e) {
			View view = (View)sender;
			view.Closed -= new EventHandler(View_Closed);
			view.Disposing -= new System.ComponentModel.CancelEventHandler(View_Disposing);
		}
		private void Window_ViewChanging(object sender, ViewChangingEventArgs e) {
			isWindowClosingLocked = true;
		}
		private void View_Closed(object sender, EventArgs e) {
			WinWindow window = Frame as WinWindow;
			if(window != null && window.Form != null && !isWindowClosingLocked) {
				window.Close();
			}
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			isWindowClosingLocked = false;
			if(Window.View != null) {
				Window.View.Closed += new EventHandler(View_Closed);
				Window.View.Disposing += new System.ComponentModel.CancelEventHandler(View_Disposing);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Application.ShowViewStrategy is MdiShowViewStrategy) {
				Window.ViewChanging += new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
				Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			}
		}
		protected override void OnDeactivated() {
			if(Application.ShowViewStrategy is MdiShowViewStrategy) {
				Window.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
				Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			}
			base.OnDeactivated();
		}
		public CloseMdiChildWindowController() {
			TargetWindowType = WindowType.Child;
		}
	}
}
