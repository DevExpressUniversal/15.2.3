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

using System.IO;
using System.Drawing;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList.Native;
namespace DevExpress.Snap.Native {
	public abstract class ReportExplorerTreeListBase : XtraTreeView {
		static ImageCollection ImageCollection;
		static ReportExplorerTreeListBase() {
			ImageCollection = new ImageCollection();
			Stream imageStream = typeof(SnapDocumentModel).Assembly.GetManifestResourceStream("DevExpress.Snap.Images.List_16x16.png");
			ImageCollection.AddImage(Image.FromStream(imageStream));
			imageStream = typeof(SnapDocumentModel).Assembly.GetManifestResourceStream("DevExpress.Snap.Images.Group_16x16.png");
			ImageCollection.AddImage(Image.FromStream(imageStream));
		}
		protected ReportExplorerTreeListBase() {
			this.StateImageList = ImageCollection;
			this.Controller = CreateController();
			SubscribeControlEvents();
		}
		public ReportExplorerController Controller { get; private set; }
		protected internal virtual void SubscribeControlEvents() {
		}
		protected virtual ReportExplorerController CreateController(){
			return new ReportExplorerController();
		}
		public Pair<string, int> GetFocusedNodeTag() {
			if (FocusedNode == null)
				return null;
			return FocusedNode.Tag as Pair<string, int>;
		}
		public XtraListNode FindNodeByTag(XtraListNodes nodes, object tag) {
			foreach (XtraListNode item in nodes) {
				if (object.Equals(tag, item.Tag))
					return item;
				XtraListNode node = FindNodeByTag((XtraListNodes)item.Nodes, tag);
				if (node != null)
					return node;
			}
			return null;
		}
	}
	public class ReorderingReportExplorerTreeList : ReportExplorerTreeListBase {
		protected override ReportExplorerController CreateController() {
			return new ReorderingReportExplorerController();
		}
		Pair<string, int> GetNextNodeTag() {
			if (FocusedNode == null || FocusedNode.NextNode == null)
				return null;
			return FocusedNode.NextNode.Tag as Pair<string, int>;
		}
		public bool CanMoveUp() {
			if (FocusedNode == null || FocusedNode.PrevNode == null || GetFocusedNodeTag().Second < 0)
				return false;
			return true;
		}
		public bool CanMoveDown() {
			if (FocusedNode == null || FocusedNode.NextNode == null)
				return false;
			if (GetFocusedNodeTag().Second < 0 || GetNextNodeTag().Second < 0)
				return false;
			return true;
		}
		public void MoveFocusedNodeUp() {
			Controller.SwapNodeTags(FocusedNode, FocusedNode.PrevNode);
			SetNodeIndex(FocusedNode, GetNodeIndex(FocusedNode) - 1);
		}
		public void MoveFocusedNodeDown() {
			Controller.SwapNodeTags(FocusedNode, FocusedNode.NextNode);
			SetNodeIndex(FocusedNode, GetNodeIndex(FocusedNode) + 1);
		}
	}
}
