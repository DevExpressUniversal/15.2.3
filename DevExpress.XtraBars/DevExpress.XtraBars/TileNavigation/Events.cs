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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Navigation;
namespace DevExpress.XtraBars.Navigation {
	public delegate void NavigationBarNavigationClientItemEventHandler(
		object sender, NavigationBarNavigationClientItemEventArgs e);
	public delegate void NavigationBarItemClickEventHandler(
		object sender, NavigationBarItemEventArgs e);
	public delegate void NavigationBarItemCancelEventHandler(
		object sender, SelectedItemChangingEventArgs e);
	public delegate void QueryPeekFormContentEventHandler(
		object sender, QueryPeekFormContentEventArgs e);
	public delegate void NavigationBarPopupMenuShowingEventHandler(
		object sender, NavigationBarPopupMenuShowingEventArgs e);
	public class QueryPeekFormContentEventArgs : EventArgs {
		public QueryPeekFormContentEventArgs() { }
		public QueryPeekFormContentEventArgs(NavigationBarItem item)
			: base() {
			this.itemCore = item;
		}
		NavigationBarItem itemCore;
		public NavigationBarItem Item {
			get { return itemCore; }
		}
		Control controlCore;
		public Control Control {
			get { return controlCore; }
			set { controlCore = value; }
		}
	}
	public class NavigationBarPopupMenuShowingEventArgs : CancelEventArgs {
		public NavigationBarPopupMenuShowingEventArgs(NavigationBarItem item) { itemCore = item; }
		public NavigationBarPopupMenuShowingEventArgs(NavigationBarButton button) { buttonCore = button; }
		NavigationBarItem itemCore;
		NavigationBarButton buttonCore;
		public NavigationBarMenuKind MenuKind { get; internal set; }
		public NavigationBarMenu Menu { get; internal set; }
		public NavigationBarItem Item { get { return itemCore; } }
	}
	public class SelectedItemChangingEventArgs : CancelEventArgs {
		public NavigationBarItem Item { get; set; }
		public NavigationBarItem PreviousItem { get; set; }
	}
	public class NavigationBarItemEventArgs : EventArgs {
		public NavigationBarItemEventArgs(NavigationBarItem item) {
			this.Item = item;
		}
		public NavigationBarItem Item { get; private set; }
	}
	public class NavigationBarNavigationClientItemEventArgs : NavigationBarItemEventArgs {
		public NavigationBarNavigationClientItemEventArgs(NavigationBarItem item, INavigationItem navigationItem)
			: base(item) {
			NavigationItem = navigationItem;
		}
		public INavigationItem NavigationItem { get; private set; }
	}
	public class NavigationPeekFormButtonClickEventArgs : DevExpress.Utils.FlyoutPanelButtonClickEventArgs {
		public NavigationPeekFormButtonClickEventArgs(DevExpress.Utils.PeekFormButton button, NavigationBarItem item)
			: base(button) {
			NavigationItem = item;
		}
		public NavigationBarItem NavigationItem { get; private set; }
	}
	public class NavigationPeekFormEventArgs : DevExpress.Utils.FlyoutPanelEventArgs {
		public NavigationPeekFormEventArgs(NavigationBarItem item, Control control) {
			NavigationItem = item;
			Control = control;
		}
		public NavigationBarItem NavigationItem { get; private set; }
		public Control Control { get; set; }
	}
}
