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
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class NavBarItemEventArgs : EventArgs {
		private NavBarItem fItem = null;
		public NavBarItem Item {
			get { return fItem; }
		}
		public NavBarItemEventArgs(NavBarItem item)
			: base() {
			fItem = item;
		}
	}
	public delegate void NavBarItemEventHandler(object source, NavBarItemEventArgs e);
	public class NavBarGroupEventArgs : EventArgs {
		private NavBarGroup fGroup = null;
		public NavBarGroup Group {
			get { return fGroup; }
		}
		public NavBarGroupEventArgs(NavBarGroup group)
			: base() {
			fGroup = group;
		}
	}
	public delegate void NavBarGroupEventHandler(object source, NavBarGroupEventArgs e);
	public class NavBarGroupCancelEventArgs : NavBarGroupEventArgs {
		private bool fCancel = false;
		public bool Cancel {
			get { return fCancel; }
			set { fCancel = value;  }
		}
		public NavBarGroupCancelEventArgs(NavBarGroup group)
			: base(group) {
		}
	}
	public delegate void NavBarGroupCancelEventHandler(object source, NavBarGroupCancelEventArgs e);
	public class NavBarGroupCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private NavBarGroup fGroup = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public NavBarGroup Group {
			get { return fGroup; }
		}
		public NavBarGroupCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public NavBarGroupCommandEventArgs(NavBarGroup group, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fGroup = group;
		}
	}
	public delegate void NavBarGroupCommandEventHandler(object source, NavBarGroupCommandEventArgs e);
	public class NavBarItemCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private NavBarItem fItem = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public NavBarItem Item {
			get { return fItem; }
		}
		public NavBarItemCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public NavBarItemCommandEventArgs(NavBarItem item, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fItem = item;
		}
	}
	public delegate void NavBarItemCommandEventHandler(object source, NavBarItemCommandEventArgs e);
}
