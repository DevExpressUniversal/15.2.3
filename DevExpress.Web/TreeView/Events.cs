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
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
namespace DevExpress.Web {
	public delegate void TreeViewNodeCommandEventHandler(object source, TreeViewNodeCommandEventArgs e);
	public delegate void TreeViewNodeEventHandler(object source, TreeViewNodeEventArgs e);
	public delegate void TreeViewNodeCancelEventHandler(object source, TreeViewNodeCancelEventArgs e);
	public delegate void TreeViewVirtualModeCreateChildrenEventHandler(object source,
		TreeViewVirtualModeCreateChildrenEventArgs e);
	public class TreeViewNodeCommandEventArgs : CommandEventArgs {
		object commandSource = null;
		TreeViewNode node = null;
		public object CommandSource {
			get { return this.commandSource; }
		}
		public TreeViewNode Node {
			get { return this.node; }
		}
		public TreeViewNodeCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			this.commandSource = commandSource;
		}
		public TreeViewNodeCommandEventArgs(TreeViewNode node, object commandSource,
			CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			this.node = node;
		}
	}
	public class TreeViewNodeEventArgs : EventArgs {
		TreeViewNode node = null;
		public TreeViewNode Node {
			get { return this.node; }
		}
		public TreeViewNodeEventArgs(TreeViewNode node)
			: base() {
			this.node = node;
		}
	}
	public class TreeViewNodeCancelEventArgs : TreeViewNodeEventArgs {
		private bool cancel = false;
		public bool Cancel {
			get { return this.cancel; }
			set { this.cancel = value; }
		}
		public TreeViewNodeCancelEventArgs(TreeViewNode node)
			: base(node) {
		}
	}
	public class TreeViewVirtualModeCreateChildrenEventArgs : EventArgs {
		IList<TreeViewVirtualNode> children = null;
		string nodeName = string.Empty;
		public TreeViewVirtualModeCreateChildrenEventArgs(string nodeName)
			: base() {
			this.nodeName = nodeName;
		}
		public IList<TreeViewVirtualNode> Children {
			get { return this.children; }
			set { this.children = value; }
		}
		public string NodeName {
			get { return this.nodeName; }
			set { this.nodeName = value; }
		}
	}
}
