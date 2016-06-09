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
using System.Drawing;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public enum WindowType { Any, Main, Child }
	[ToolboxItem(false)]
	public class WindowController : Controller {
		public const string WindowIsAssignedReason = "Window is assigned";
		private WindowType targetWindowType = WindowType.Any;
		private Window window;
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			if(Window != null && Window.View != null) {
				SubscribeToViewEvents(Window.View);
			}
		}
		private bool IsFitToWindowType(bool isMain, WindowType windowType, BoolList results) {
			bool result = true;
			if(windowType != WindowType.Any) {
				result = windowType == WindowType.Main && isMain
							|| windowType == WindowType.Child && !isMain;
				results["Window type is" + windowType] = result;
			}
			return result;
		}
		private void SetWindowCore(Window window) {
			if(window != null) {
				this.window = window;
			}
			Active.SetItemValue(WindowIsAssignedReason, window != null);
			if(window == null) {
				this.window = null;
			}
		}
		protected virtual void OnWindowChanging(Window window) {
			IsFitToWindowType(window.IsMain, targetWindowType, Active);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Window != null) {
				if(Window.View != null) {
					SubscribeToViewEvents(Window.View);
				}
				Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			}
		}
		protected override void OnDeactivated() {
			if(Window != null) {
				Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			}
			base.OnDeactivated();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					SetWindowCore(null);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnViewClosing(object sender, EventArgs e) {
			if(ViewClosing != null) {
				ViewClosing(sender, e);
			}
		}
		protected virtual void OnViewClosed(object sender, EventArgs e) {
			if(ViewClosed != null) {
				ViewClosed(sender, e);
			}
			View view = sender as View;
			if(view != null) {
				UnsubscribeFromViewEvents(view);
			}
		}
		protected virtual void OnViewQueryCanClose(object sender, CancelEventArgs e) {
			if(ViewQueryCanClose != null) {
				ViewQueryCanClose(sender, e);
			}
		}
		protected virtual void SubscribeToViewEvents(View view) {
			view.QueryCanClose +=new EventHandler<CancelEventArgs>(OnViewQueryCanClose);
			view.Closing += new EventHandler(OnViewClosing);
			view.Closed += new EventHandler(OnViewClosed);
		}
		protected virtual void UnsubscribeFromViewEvents(View view) {
			view.QueryCanClose -= new EventHandler<CancelEventArgs>(OnViewQueryCanClose);
			view.Closing -= new EventHandler(OnViewClosing);
			view.Closed -= new EventHandler(OnViewClosed);
		}
		public WindowController() {
			Active.SetItemValue(WindowIsAssignedReason, false);
		}
		public void SetWindow(Window newWindow) {
			if(newWindow == null) {
				SetWindowCore(null);
			}
			else {
				if(Active) {
					throw new InvalidOperationException(string.Format(
						"The '{0}' controller is active.", GetType()));
				}
				Active.BeginUpdate();
				try {
					OnWindowChanging(newWindow);
					SetWindowCore(newWindow);
				}
				finally {
					Active.EndUpdate();
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("WindowControllerTargetWindowType"),
#endif
 Category("Target Window"), DefaultValue(WindowType.Any)]
		public WindowType TargetWindowType {
			get { return targetWindowType; }
			set { targetWindowType = value; }
		}
		[DefaultValue(null), Browsable(false)]
		public Window Window {
			get { return window; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("WindowControllerViewQueryCanClose"),
#endif
 Category("Events")]
		public event EventHandler<CancelEventArgs> ViewQueryCanClose;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("WindowControllerViewClosing"),
#endif
 Category("Events")]
		public event EventHandler ViewClosing;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("WindowControllerViewClosed"),
#endif
 Category("Events")]
		public event EventHandler ViewClosed;
	}
}
