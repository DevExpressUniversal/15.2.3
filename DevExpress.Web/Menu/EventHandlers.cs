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
	public class MenuItemEventArgs: EventArgs {
		private MenuItem fItem = null;
		public MenuItem Item {
			get { return fItem; }
		}
		public MenuItemEventArgs(MenuItem item)
			: base() {
			fItem = item;
		}
	}
	public delegate void MenuItemEventHandler(object source, MenuItemEventArgs e);
	public class MenuItemCommandEventArgs: CommandEventArgs {
		private object fCommandSource = null;
		private MenuItem fItem = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public MenuItem Item {
			get { return fItem; }
		}
		public MenuItemCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public MenuItemCommandEventArgs(MenuItem item, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fItem = item;
		}
	}
	public delegate void MenuItemCommandEventHandler(object source, MenuItemCommandEventArgs e);
}
