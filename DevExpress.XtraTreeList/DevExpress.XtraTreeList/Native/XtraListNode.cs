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
using System.Drawing;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraTreeList.Native {
	public class XtraListNode : TreeListNode {
		string text;
		public string Text { get { return text; } set { text = value; } }
		public Rectangle Bounds { get { return GetBounds(this); } }
		public XtraListNode(TreeListNodes owner)
			: base(0, owner) {
		}
		public XtraListNode(string text, TreeListNodes owner)
			: this(owner) {
			this.text = text;
		}
		public override object GetValue(object columnID) {
			return text;
		}
		public override string GetDisplayText(object columnID) {
			return text;
		}
		protected override TreeListNodes CreateNodes(TreeList treeList) {
			return new XtraListNodes(treeList, this);
		}
		public void Expand() {
			this.Expanded = true;
		}
		public void Collapse() {
			this.Expanded = false;
		}
		public void Remove() {
			this.TreeList.DeleteNode(this);
		}
		internal void SetID(int id) {
			base.SetId(id);
		}
		internal void SetOwner(TreeListNodes owner) {
			this.owner = owner;
		}
		public override bool HasChildren {
			get {
				return base.HasChildren || this.Nodes.Count > 0;
			}
			set {
				base.HasChildren = value;
			}
		}
		Rectangle GetBounds(XtraListNode node) {
			if ((node is INode && ((INode)node).IsDummyNode) || node.TreeList == null)
				return Rectangle.Empty;
			return GetBounds(node, node.TreeList.ViewInfo.RowsInfo);
		}
		Rectangle GetBounds(XtraListNode node, DevExpress.XtraTreeList.ViewInfo.RowsInfo rowsInfo) {
			if (rowsInfo.Rows.Count == 0 || rowsInfo[node] == null)
				return Rectangle.Empty;
			return rowsInfo[node].Bounds;
		}
	}
}
