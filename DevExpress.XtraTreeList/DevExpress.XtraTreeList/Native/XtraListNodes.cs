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
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraTreeList.Native {
	public class XtraListNodes : TreeListNodes, IList {
		XtraListNode node;
		public XtraListNodes(TreeList treeList, TreeListNode parentNode) :
			base(treeList, parentNode) {
		}
		#region IList Members
		int IList.Add(object value) {
			node = (XtraListNode)value;
			try {
				this.TreeList.CreateCustomNode += new CreateCustomNodeEventHandler(TreeList_CreateCustomNode);
				this.TreeList.AppendNode(null, ParentNode);
				if (node.Nodes.Count > 0)
					node.HasChildren = true;
			}
			finally {
				this.TreeList.CreateCustomNode -= new CreateCustomNodeEventHandler(TreeList_CreateCustomNode);
				node = null;
			}
			return this.Count - 1;
		}
		void TreeList_CreateCustomNode(object sender, CreateCustomNodeEventArgs e) {
			e.Node = this.node;
			this.node.SetID(e.NodeID);
			this.node.SetOwner(e.Owner);
		}
		bool IList.Contains(object value) {
			return this.IndexOf((TreeListNode)value) >= 0;
		}
		int IList.IndexOf(object value) {
			return this.IndexOf((TreeListNode)value);
		}
		void IList.Insert(int index, object value) {
			((IList)this).Add(value);
			this.TreeList.SetNodeIndex((TreeListNode)value, index);
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		bool IList.IsReadOnly {
			get { return false; }
		}
		void IList.Remove(object value) {
			this.Remove((TreeListNode)value);
		}
		object IList.this[int index] {
			get { return this[index]; }
			set { throw new Exception("The method or operation is not implemented."); }
		}
		#endregion
	}
}
