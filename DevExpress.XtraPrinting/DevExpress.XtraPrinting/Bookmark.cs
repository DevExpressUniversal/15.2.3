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
using System.Collections;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraPrinting.Native.WinControls {
	[ToolboxItem(false)]
	public class BookmarkTreeView : XtraTreeView {
		public BookmarkTreeView() {
			Dock = DockStyle.Left;
			Width = 150;
			OptionsSelection.EnableAppearanceFocusedCell = false;
		}
		public void FillNodes(Document document) {
			BeginUpdate();
			try {
				Nodes.Clear();
				if(document.BookmarkNodes.Count > 0) {
					AddNode((XtraListNodes)Nodes, document.RootBookmark, document);
					if(Nodes.Count > 0)
						Nodes[0].Expanded = true;
				}
			} finally {
				EndUpdate();
			}
		}
		private void AddNode(XtraListNodes nodes, BookmarkNode bmNode, Document document) {
			System.Diagnostics.Debug.Assert(bmNode != null);
			if(!bmNode.IsValid(document))
				return;
			XtraListNode node = new BrickPageNode(bmNode, nodes);
			((IList)nodes).Add(node);
			foreach(BookmarkNode item in bmNode.Nodes) {
				AddNode((XtraListNodes)node.Nodes, item, document);
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			Selection.Clear();
		}
		public void SetFocusedNode(BrickPagePair pair) {
			if(Nodes.Count <= 0)
				return;
			TreeListNode node = FindNodeByBrickPagePair(Nodes[0], pair);
			if(node == null)
				return;			
			SetFocusedNode(node);			
		}
		static TreeListNode FindNodeByBrickPagePair(TreeListNode rootNode, BrickPagePair pair) {
			foreach(TreeListNode item in rootNode.Nodes) {
				BrickPageNode node = (BrickPageNode)item;
				if(node.Pair == pair)
					return node;
				TreeListNode foundNode = FindNodeByBrickPagePair(node, pair);
				if(foundNode != null)
					return foundNode;
			}
			return null;
		}
	}
	public class BrickPageNode : XtraListNode {
		BookmarkNode bmNode;
		public BrickPagePair Pair {
			get { return bmNode.Pair; }
		}
		public BrickPageNode(BookmarkNode bmNode, XtraListNodes owner)
			: base(owner) {
			if(bmNode == null) throw new ArgumentException("bmNode");
			Text = bmNode.Text;
			this.bmNode = bmNode;
		}
		public void ShowAssociatedBrick(PrintControl printControl) {
			if(bmNode is RootBookmarkNode && printControl.Document != null && printControl.Document.Pages.Count > 0)
				printControl.ShowPage(printControl.Document.Pages[0]);
			else
				printControl.ShowBrick(bmNode.Pair);
		}
	}
}
