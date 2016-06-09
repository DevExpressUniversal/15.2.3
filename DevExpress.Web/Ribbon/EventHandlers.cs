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
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class RibbonItemEventArgs : EventArgs {
		public RibbonItemEventArgs(RibbonItemBase item)
			: base() {
			Item = item;
		}
		public RibbonItemBase Item { get; set; }
	}
	public delegate void RibbonItemEventHandler(object source, RibbonItemEventArgs e);
	public class RibbonTabEventArgs : EventArgs {
		public RibbonTabEventArgs(RibbonTab tab)
			: base() {
				Tab = tab;
		}
		public RibbonTab Tab { get; private set; }
	}
	public delegate void RibbonTabEventHandler(object source, RibbonTabEventArgs e);
	public class RibbonGroupEventArgs : EventArgs {
		public RibbonGroupEventArgs(RibbonGroup group)
			: base() {
			Group = group;
		}
		public RibbonGroup Group { get; private set; }
	}
	public delegate void RibbonGroupEventHandler(object source, RibbonGroupEventArgs e);
	public class RibbonCommandExecutedEventArgs : EventArgs {
		public RibbonCommandExecutedEventArgs(RibbonItemBase item, string parameter)
			: base() {
			Item = item;
			Parameter = parameter;
		}
		public RibbonItemBase Item { get; private set; }
		public string Parameter { get; private set; }		
	}
	public delegate void RibbonCommandExecutedEventHandler(object source, RibbonCommandExecutedEventArgs e);
	public class DialogBoxLauncherClickedEventArgs : EventArgs {
		public DialogBoxLauncherClickedEventArgs(RibbonGroup group)
			: base() {
			Group = group;
		}
		public RibbonGroup Group { get; private set; }
	}
	public delegate void RibbonDialogBoxLauncherClickedEventHandler(object source, DialogBoxLauncherClickedEventArgs e);
}
