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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.Web.Design {
	[ToolboxItem(false)]
	public class TreeViewEx : TreeView {
		private static int ExpandTime = 1000;
		private bool fCanDragTargetAnimation = false;
		private TreeNode fDraggedItem = null;
		private TreeNode fDragOverNode = null;
		private TreeNode fLastestDragOverNode = null;
		private System.Timers.Timer fTimer = null;
		protected internal bool DragHighlighting {
			get { return fCanDragTargetAnimation; }
			set { fCanDragTargetAnimation = value; }
		}
		public TreeViewEx()
			: base() {
			fTimer = new System.Timers.Timer(ExpandTime);
			fTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			fTimer.SynchronizingObject = this;
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			OverShadowNode(fLastestDragOverNode);
			fLastestDragOverNode = null;
			fDraggedItem = null;
			base.OnDragDrop(drgevent);
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			TreeNode dragOverNode = GetNodeAt(PointToClient(new Point(drgevent.X, drgevent.Y)));
			Point point = PointToClient(new Point(drgevent.X, drgevent.Y));
			TreeViewHitTestLocations hitTestInfo = HitTest(point).Location;
			if (hitTestInfo == TreeViewHitTestLocations.PlusMinus)
				StartExpand(dragOverNode);
			else
				StopExpand();
			if (DragHighlighting)
				DragTargetAnimation(dragOverNode);
			else
				OverShadowNode(fLastestDragOverNode);
			base.OnDragOver(drgevent);
		}
		protected override void OnItemDrag(ItemDragEventArgs e) {
			fDraggedItem = e.Item as TreeNode;
			base.OnItemDrag(e);
		}
		protected void DragTargetAnimation(TreeNode dragOverNode) {
			if (fLastestDragOverNode != null)
				if (fLastestDragOverNode != dragOverNode)
					OverShadowNode(fLastestDragOverNode);
				else {
					if (fDraggedItem != fLastestDragOverNode)
						HighlightNode(fLastestDragOverNode);
				}
			if (dragOverNode != null)
				fLastestDragOverNode = dragOverNode;
		}
		protected void StartExpand(TreeNode dragOverNode) {
			if ((dragOverNode != null) && (fDragOverNode != dragOverNode))
				fTimer.Start();
			fDragOverNode = dragOverNode;
		}
		protected void StopExpand() {
			fTimer.Stop();
			fDragOverNode = null;
		}
		private void HighlightNode(TreeNode node) {
			if (node != null) {
				node.BackColor = SystemColors.Highlight;
				node.ForeColor = Color.White;
			}
		}
		private void OverShadowNode(TreeNode node) {
			if (node != null) {
				node.BackColor = Color.Empty;
				node.ForeColor = Color.Empty;
			}
		}
		private void OnTimedEvent(object source, ElapsedEventArgs e) {
			fTimer.Stop();
			if (fDragOverNode != null)
				fDragOverNode.Expand();
			fDragOverNode = null;
		}
	}
}
