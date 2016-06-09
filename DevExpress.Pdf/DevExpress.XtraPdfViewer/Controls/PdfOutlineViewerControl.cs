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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Pdf.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Controls {
	[DXToolboxItem(false)]
	public partial class PdfOutlineViewerControl : XtraUserControl {
		readonly PdfViewer viewer;
		internal PdfViewer Viewer { get { return viewer; } }
		internal BarManager OutlineViewerBarManager { get { return outlineViewerBarManager; } }
		internal PdfOutlineViewerTreeList TreeList { get { return outlineViewerTreeList; } }
		internal PdfOutlineViewerNode FocusedOutlineViewerNode { get { return outlineViewerTreeList.GetDataRecordByNode(outlineViewerTreeList.FocusedNode) as PdfOutlineViewerNode; } }
		internal bool AreTopLevelTreeNodesExpanded {
			get {
				for (TreeListNode node = outlineViewerTreeList.Nodes.FirstNode; node != null; node = node.NextNode)
					if (node.HasChildren && node.Expanded)
						return true;
				return false;
			}
		}
		public PdfOutlineViewerControl() {
			InitializeComponent();
			outlineViewerMenu.OutlineViewerControl = this;
		}
		public PdfOutlineViewerControl(PdfViewer viewer) : this() {
			this.viewer = viewer;
			UserLookAndFeel lookAndFeel = viewer.LookAndFeel;
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			outlineViewerBarAndDockingController.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			outlineViewerTreeList.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			outlineViewerTreeList.Load += new EventHandler(OnTreeListLoad);
			outlineViewerTreeList.FocusedNodeChanged += new FocusedNodeChangedEventHandler(OnNodeChanged);
			outlineViewerTreeList.AfterExpand += new NodeEventHandler(OnNodeChanged);
			outlineViewerTreeList.AfterCollapse += new NodeEventHandler(OnNodeChanged);
			outlineViewerTreeList.MouseDown += new MouseEventHandler(OnTreeListMouseDown);
			outlineViewerTreeList.MouseUp += new MouseEventHandler(OnTreeListMouseUp);
			outlineViewerTreeList.KeyDown += new KeyEventHandler(OnTreeListKeyDown);
			expandCurrentNodeButton.ItemClick += new ItemClickEventHandler(OnExpandCurrentNodeButtonClick);
			UpdateExpandCurrentNodeButton();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
				outlineViewerTreeList.Load -= new EventHandler(OnTreeListLoad);
				outlineViewerTreeList.FocusedNodeChanged -= new FocusedNodeChangedEventHandler(OnNodeChanged);
				outlineViewerTreeList.AfterExpand -= new NodeEventHandler(OnNodeChanged);
				outlineViewerTreeList.AfterCollapse -= new NodeEventHandler(OnNodeChanged);
				outlineViewerTreeList.MouseDown -= new MouseEventHandler(OnTreeListMouseDown);
				outlineViewerTreeList.MouseUp -= new MouseEventHandler(OnTreeListMouseUp);
				outlineViewerTreeList.KeyDown -= new KeyEventHandler(OnTreeListKeyDown);
				expandCurrentNodeButton.ItemClick -= new ItemClickEventHandler(OnExpandCurrentNodeButtonClick);
			}
			base.Dispose(disposing);
		}
		internal void SetOutlineViewerNodes(IList<PdfOutlineViewerNode> nodes) {
			ClearFilters();
			outlineViewerTreeList.DataSource = nodes;
			if (!outlineViewerTreeList.OptionsView.AutoWidth)
				titleColumn.BestFit();
		}
		internal void ClearOutlineViewerNodes() {
			ClearFilters();
			outlineViewerTreeList.DataSource = null;
		}
		internal void ExpandCurrentOutlineViewerNode() {
			TreeListNode focusedNode = outlineViewerTreeList.FocusedNode;
			if (focusedNode != null)
				focusedNode.Expanded = true;
		}
		internal void ExpandCollapseTopLevelOutlineViewerNodes() {
			bool expanded = !AreTopLevelTreeNodesExpanded;
			for (TreeListNode node = outlineViewerTreeList.Nodes.FirstNode; node != null; node = node.NextNode)
				node.Expanded = expanded;
		}
		internal void GotoFocusedOutlineViewerNode() {
			PdfOutlineViewerNode outlineViewerNode = FocusedOutlineViewerNode;
			PdfDocumentStateController documentStateController = viewer.DocumentStateController;
			if (outlineViewerNode != null && documentStateController != null) {
				documentStateController.ExecuteInteractiveOperation(outlineViewerNode.InteractiveOperation);
				if (viewer.OutlineViewerSettings.HideAfterUse)
					viewer.NavigationPane.State = NavigationPaneState.Collapsed;
			}
		}
		internal bool CanPrintPagesNumbers(bool printSections) {
			PdfDocumentState documentState = viewer.DocumentState;
			return documentState != null && documentState.CanPrintOutlineNodesPages(GetSelectedNodes(), printSections);
		}
		internal void PrintSelectedOutlineNodesPages(bool printSections) {
			PdfPageSetupDialogShowingEventArgs args = new PdfPageSetupDialogShowingEventArgs();
			args.PrinterSettings.PageNumbers = GetPrintPageNumbers(printSections);
			viewer.ShowPageSetupDialog(args);
		}
		IEnumerable<PdfOutlineViewerNode> GetSelectedNodes() {
			List<PdfOutlineViewerNode> selectedNodes = new List<PdfOutlineViewerNode>();
			foreach (TreeListNode node in outlineViewerTreeList.Selection) {
				PdfOutlineViewerNode outlineViewerNode = outlineViewerTreeList.GetDataRecordByNode(node) as PdfOutlineViewerNode;
				if (outlineViewerNode != null)
					selectedNodes.Add(outlineViewerNode);
			}
			return selectedNodes;
		}
		int[] GetPrintPageNumbers(bool printSection) {
			PdfDocumentState documentState = viewer.DocumentState;
			return documentState != null ? documentState.GetOutlineNodesPrintPageNumbers(GetSelectedNodes(), printSection) : new int[0];
		}
		void ClearFilters() {
			outlineViewerTreeList.ClearNodes();
			outlineViewerTreeList.ClearSorting();
			outlineViewerTreeList.ClearColumnsFilter();
		}
		void UpdateExpandCurrentNodeButton() {
			TreeListNode node = outlineViewerTreeList.FocusedNode;
			expandCurrentNodeButton.Enabled = node != null && node.HasChildren && !node.Expanded;
		}
		void OnTreeListLoad(object sender, EventArgs e) {
			outlineViewerTreeList.UpdateSettings(viewer.OutlineViewerSettings);
		}
		void OnNodeChanged(object sender, EventArgs e) {
			UpdateExpandCurrentNodeButton();
		}
		void OnTreeListMouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				TreeListHitInfo hitInfo = outlineViewerTreeList.CalcHitInfo(e.Location);
				if (hitInfo != null && hitInfo.HitInfoType != HitInfoType.Button) {
					TreeListNode selectedNode = hitInfo.Node;
					if (selectedNode != null) {
						List<TreeListNode> selectedNodes = new List<TreeListNode>();
						bool isSelectedNode = outlineViewerTreeList.Selection.Contains(selectedNode);
						if (isSelectedNode)
							foreach (TreeListNode node in outlineViewerTreeList.Selection)
								selectedNodes.Add(node);
						outlineViewerTreeList.FocusedNode = selectedNode;
						if (isSelectedNode)
							outlineViewerTreeList.Selection.Set(selectedNodes);
						popupMenu.BeginInit();
						try {
							popupMenu.ClearLinks();
							popupMenu.AddItem(new PdfOutlinesGotoCommand(viewer).CreateContextMenuBarItem(outlineViewerBarManager));
							popupMenu.AddItem(new PdfOutlinesPrintPagesCommand(viewer).CreateContextMenuBarItem(outlineViewerBarManager));
							popupMenu.AddItem(new PdfOutlinesPrintSectionsCommand(viewer).CreateContextMenuBarItem(outlineViewerBarManager));
							popupMenu.AddItem(new PdfOutlinesWrapLongLinesCommand(viewer).CreateContextMenuBarItem(outlineViewerBarManager)).BeginGroup = true;
						}
						finally {
							popupMenu.EndInit();
						}
						viewer.RaisePopupMenuShowing(new PdfPopupMenuShowingEventArgs(popupMenu, PdfPopupMenuKind.BookmarkTree));
						popupMenu.ShowPopup(Control.MousePosition);
					}
				}
			}
		}
		void OnTreeListMouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && ((int)Control.ModifierKeys & (int)(Keys.Control | Keys.Shift)) == 0) {
				TreeListHitInfo hitInfo = outlineViewerTreeList.CalcHitInfo(e.Location);
				if (hitInfo != null && hitInfo.HitInfoType != HitInfoType.Button) {
					TreeListNode hitNode = hitInfo.Node;
					if (hitNode != null) {
						TreeListMultiSelection selection = outlineViewerTreeList.Selection;
						if (selection != null && selection.Count == 1) {
							TreeListNode selectedNode = selection[0];
							if (selectedNode != null && selectedNode.Id == hitNode.Id)
								GotoFocusedOutlineViewerNode();
						}
					}
				}
			}
		}
		void OnTreeListKeyDown(object sender, KeyEventArgs e) {
			TreeListNode focusedNode = outlineViewerTreeList.FocusedNode;
			if (focusedNode != null)
				switch (e.KeyCode) {
					case Keys.Enter:
					case Keys.Space:
						Keys modifiers = e.Modifiers;
						if (modifiers == Keys.Shift || modifiers == Keys.None)
							GotoFocusedOutlineViewerNode();
						break;
					case Keys.Right:
						if (focusedNode.Expanded && focusedNode.HasChildren)
							outlineViewerTreeList.FocusedNode = focusedNode.FirstNode;
						else
							focusedNode.Expanded = true;
						break;
					case Keys.Left:
						if (focusedNode.Expanded)
							focusedNode.Expanded = false;
						else {
							TreeListNode parentNode = focusedNode.ParentNode;
							if (parentNode != null)
								outlineViewerTreeList.FocusedNode = parentNode;
						}
						break;
				}
		}
		void OnExpandCurrentNodeButtonClick(object sender, ItemClickEventArgs e) {
			ExpandCurrentOutlineViewerNode();
		}
	}
}
