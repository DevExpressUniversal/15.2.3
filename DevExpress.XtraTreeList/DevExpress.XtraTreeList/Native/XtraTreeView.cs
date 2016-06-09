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
using DevExpress.XtraTreeList;
using System.ComponentModel;
using DevExpress.XtraTreeList.Nodes;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Data;
using DevExpress.Data.Browsing.Design;
using DevExpress.Utils;
namespace DevExpress.XtraTreeList.Native {
	[ToolboxItem(false)]
	public class XtraTreeView : TreeList {
		XtraListNode draggedNode;
		TreeListHitInfo dragStartHitInfo;
		public XtraListNode DraggedNode { get { return draggedNode; } set { draggedNode = value; } }
		public XtraListNode SelectedNode { get { return (XtraListNode)this.Selection[0]; } }
		internal protected TreeListData DataTreeListController { get { return Data; } }
		public event ItemDragEventHandler ItemDrag;
		public XtraTreeView()
			: base(null) {
			this.OptionsBehavior.Editable = false;
			this.OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.None;
			this.OptionsPrint.PrintHorzLines = false;
			this.OptionsPrint.PrintVertLines = false;
			this.OptionsPrint.PrintImages = true;
			this.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.OptionsSelection.MultiSelect = false;
			this.OptionsView.AutoWidth = false;
			this.OptionsView.ShowColumns = false;
			this.OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
			this.OptionsView.ShowHorzLines = false;
			this.OptionsView.ShowIndicator = false;
			this.OptionsView.ShowVertLines = false;
			this.OptionsView.ShowButtons = true;
			this.OptionsView.AutoWidth = true;
			this.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
			this.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			new DevExpress.XtraTreeList.Columns.TreeListColumn()});
			this.Columns[0].Visible = true;
			this.Columns[0].OptionsColumn.AllowFocus = true;
			this.MouseDown += new MouseEventHandler(ItemDown);
			this.MouseMove += new MouseEventHandler(ItemMove);
		}
		public new XtraListNodes Nodes { get { return (XtraListNodes)base.Nodes; } }
		void ItemDown(object sender, MouseEventArgs e) {
			if((e.Button.IsLeft() || e.Button.IsRight()) && System.Windows.Forms.Control.ModifierKeys == Keys.None) {
				TreeList tl = sender as TreeList;
				dragStartHitInfo = tl.CalcHitInfo(new Point(e.X, e.Y));
				draggedNode = (XtraListNode)dragStartHitInfo.Node;
			}
		}
		protected virtual void ItemMove(object sender, MouseEventArgs e) {
			if((e.Button.IsLeft() || e.Button.IsRight())
				&& dragStartHitInfo != null && (dragStartHitInfo.HitInfoType == HitInfoType.Cell || dragStartHitInfo.HitInfoType == HitInfoType.StateImage)) {
				Size dragSize = SystemInformation.DragSize;
				Rectangle dragRect = new Rectangle(new Point(dragStartHitInfo.MousePoint.X - dragSize.Width / 2,
					dragStartHitInfo.MousePoint.Y - dragSize.Height / 2), dragSize);
				if(!dragRect.Contains(new Point(e.X, e.Y))) {
					string dragObject = dragStartHitInfo.Node.GetDisplayText(dragStartHitInfo.Column);
					if(ItemDrag != null)
						ItemDrag(this, new ItemDragEventArgs(e.Button, dragStartHitInfo.Node)); 
				}
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if (e.KeyData == Keys.Left && SelectedNode != null)
				SelectedNode.Collapse();
			else if (e.KeyData == Keys.Right && SelectedNode != null)
				SelectedNode.Expand();
			else if(e.KeyData == Keys.Enter)
				OnClick(EventArgs.Empty);
		}
		protected override TreeListNodes CreateNodes() {
			return new XtraListNodes(this, null);
		}
		public new void SelectNode(TreeListNode node) {
			this.FocusedNode = node;
		}
		public TreeListNode GetNodeAt(int x, int y) {
			return this.GetNodeAt(new Point(x, y));
		}
		public TreeListNode GetNodeAt(Point point) {
			return this.CalcHitInfo(point).Node;
		}
	}
}
