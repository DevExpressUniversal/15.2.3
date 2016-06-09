#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	internal class FilterModelDataHelper : ModelInterfaceAdapter {
		public override string GetModelNodeDisplayValue_Cached(ModelNode modelNode) {
			string result;
			if(!nodesDisplayValue.TryGetValue(modelNode.GetHashCode(), out result)) {
				result = base.GetModelNodeDisplayValue_Cached(modelNode);
				nodesDisplayValue.Add(modelNode.GetHashCode(), result);
			}
			return result;
		}
		public void ResetCache() {
			nodesDisplayValue.Clear();
		}
		Dictionary<int, string> nodesDisplayValue = new Dictionary<int, string>();
	}
	internal class FilterModelTreeList : TreeList {
		private int ShowViewByKeyNavigationDelay = 50;
		private bool navigationByKeyOn;
		private TreeListNode oldNode;
		private Timer showViewByKeyNavigationTimer;
		private bool raiseFocusedNodeChanged = true;
		private Dictionary<string, bool> selectedNodePaths;
		private string filterText = "";
		private List<string> imageNames = null;
		FilterModelDataHelper dataHelper = new FilterModelDataHelper();
		private FastModelEditorHelper fastModelEditorHelper;
		private void focusedNodeChangedOnKeyUpTimer_Tick(object sender, EventArgs e) {
			raiseFocusedNodeChanged = true;
			RaiseFocusedNodeChanged(oldNode, FocusedNode);
		}
		private void CreateShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer = new Timer();
			showViewByKeyNavigationTimer.Interval = ShowViewByKeyNavigationDelay;
			showViewByKeyNavigationTimer.Tick += new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		private void StopShowViewByKeyNavigationTimer() {
			if(showViewByKeyNavigationTimer != null) {
				showViewByKeyNavigationTimer.Stop();
				showViewByKeyNavigationTimer.Enabled = false;
			}
		}
		private void StartShowViewByKeyNavigationTimer() {
			if(showViewByKeyNavigationTimer != null) {
				showViewByKeyNavigationTimer.Enabled = true;
				showViewByKeyNavigationTimer.Start();
			}
		}
		private void ReleaseShowViewByKeyNavigationTimer() {
			StopShowViewByKeyNavigationTimer();
			showViewByKeyNavigationTimer.Tick -= new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		public FilterModelTreeList(FastModelEditorHelper fastModelEditorHelper) {
			this.fastModelEditorHelper = fastModelEditorHelper;
			OptionsView.ShowIndicator = false;
			OptionsView.ShowHorzLines = false;
			OptionsView.ShowVertLines = false;
			OptionsView.ShowButtons = true;
			OptionsView.FocusRectStyle = DrawFocusRectStyle.CellFocus;
			OptionsSelection.EnableAppearanceFocusedRow = true;
			OptionsView.ShowColumns = false;
			OptionsView.ShowRoot = true;
			OptionsView.AutoWidth = true;
			OptionsBehavior.Editable = false;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.None;
			OptionsDragAndDrop.ExpandNodeOnDrag = true;
			OptionsSelection.MultiSelect = false; 
			OptionsBehavior.KeepSelectedOnClick = true;
			OptionsNavigation.AutoFocusNewNode = false;
			this.StateImageList = new ImageCollection();
			this.imageNames = new List<string>();
			CreateShowViewByKeyNavigationTimer();
			navigationByKeyOn = false;
			this.VirtualTreeGetCellValue += new VirtualTreeGetCellValueEventHandler(filteringTree_VirtualTreeGetCellValue);
			this.VirtualTreeGetChildNodes += new VirtualTreeGetChildNodesEventHandler(filteringTree_VirtualTreeGetChildNodes);
			this.VirtualTreeSetCellValue += new VirtualTreeSetCellValueEventHandler(filteringTree_VirtualTreeSetCellValue);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ReleaseShowViewByKeyNavigationTimer();
			}
			base.Dispose(disposing);
		}
		public void SetFilter(string filterText, Dictionary<string, bool> selectedNodePaths) {
			isLoadBegin = false;
			sortedNodeKeys.Clear();
			dataHelper.ResetCache();
			childrenNodes.Clear();
			this.filterText = !string.IsNullOrEmpty(filterText) ? filterText.ToLower() : "";
			this.selectedNodePaths = selectedNodePaths;
		}
		protected override void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			this.oldNode = oldNode;
			if(navigationByKeyOn) {
				StopShowViewByKeyNavigationTimer();
				StartShowViewByKeyNavigationTimer();
			}
			if(raiseFocusedNodeChanged) {
				StopShowViewByKeyNavigationTimer();
				raiseFocusedNodeChanged = false;
				base.RaiseFocusedNodeChanged(oldNode, newNode);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(!e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
				navigationByKeyOn = true;
				base.OnKeyDown(e);
			}
			else {
				base.OnKeyDown(e);
			}
		}
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.KeyCode == Keys.Enter) {
				raiseFocusedNodeChanged = true;
				RaiseFocusedNodeChanged(oldNode, FocusedNode);
			}
			else
				if(!e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
					StopShowViewByKeyNavigationTimer();
					StartShowViewByKeyNavigationTimer();
				}
			navigationByKeyOn = false;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && State == TreeListState.Regular) {
				TreeListHitInfo info = CalcHitInfo(e.Location);
				if(info.HitInfoType == HitInfoType.Cell || info.HitInfoType == HitInfoType.RowIndicator) {
					raiseFocusedNodeChanged = true;
					FocusedNode = info.Node;
				}
			}
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left) {
				TreeListHitInfo li = CalcHitInfo(e.Location);
				if((li.HitInfoType == HitInfoType.Row || li.HitInfoType == HitInfoType.RowIndent ||
					li.HitInfoType == HitInfoType.RowIndicator || li.HitInfoType == HitInfoType.RowPreview ||
					li.HitInfoType == HitInfoType.Cell || li.HitInfoType == HitInfoType.StateImage ||
					li.HitInfoType == HitInfoType.SelectImage || li.HitInfoType == HitInfoType.RowIndicatorEdge) ||
					(li.HitInfoType == HitInfoType.Button && (oldNode == null || li.Node.Id != oldNode.Id))) {
					raiseFocusedNodeChanged = true;
					RaiseFocusedNodeChanged(oldNode, li.Node);
				}
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(!(e.Button == MouseButtons.Right) && !(ModifierKeys == Keys.None)) {
				base.OnMouseUp(e);
			}
		}
		protected override void RaiseGetStateImage(TreeListNode node, ref int nodeImageIndex) {
			base.RaiseGetStateImage(node, ref nodeImageIndex);
			UpdateControlNodeImage(node, ref nodeImageIndex);
		}
		protected override void RaiseCustomDrawNodeCell(CustomDrawNodeCellEventArgs e) {
			base.RaiseCustomDrawNodeCell(e);
			SearchTextHighlight(e);
		}
		Dictionary<int, bool> sortedNodeKeys = new Dictionary<int, bool>();
		Dictionary<int, List<ModelNode>> childrenNodes = new Dictionary<int, List<ModelNode>>();
		private void UpdateControlNodeImage(TreeListNode controlNode, ref int nodeImageIndex) {
			string imageKey = "";
			ModelNode modelNode = (ModelNode)GetDataRecordByNode(controlNode);
			Image nodeImage = dataHelper.GetImage(modelNode, out imageKey);
			if(nodeImage != null) {
				if(string.IsNullOrEmpty(imageKey)) {
					imageKey = modelNode.GetHashCode().ToString();
				}
				if(this.imageNames.IndexOf(imageKey) == -1) {
					this.imageNames.Add(imageKey);
					this.StateImageList.AddImage(nodeImage);
				}
				nodeImageIndex = this.imageNames.IndexOf(imageKey);
			}
		}
		public new ImageCollection StateImageList {
			get { return (ImageCollection)base.StateImageList; }
			set { base.StateImageList = value; }
		}
		private List<ModelNode> GetChildrenNodes(ModelNode parrent) {
			List<ModelNode> result;
			if(parrent == null || string.IsNullOrEmpty(filterText)) { return new List<ModelNode>(); }
			if(!childrenNodes.TryGetValue(parrent.GetHashCode(), out result)) {
				result = new List<ModelNode>();
				foreach(ModelNode node in ModelEditorHelper.GetChildNodes(parrent)) {
					if(CheckNode(node)) {
						result.Add(node);
					}
					else {
						if(FindInSubNode(node)) {
							result.Add(node);
						}
					}
				}
				result.Sort(new ModelTreeListNodeComparer(fastModelEditorHelper));
				childrenNodes.Add(parrent.GetHashCode(), result);
			}
			return result;
		}
		private int checkNodeLevelLimiter = 0;
		private bool CheckNode(ModelNode modelNode) {
			bool isChecked;
			bool result;
			try {
				checkNodeLevelLimiter++;
				if(!sortedNodeKeys.TryGetValue(modelNode.GetHashCode(), out result)) {
					if(!selectedNodePaths.TryGetValue(GetNodeTypePath(modelNode, modelNode.GetType().Name), out isChecked)) {
						isChecked = true;
					}
					if(isChecked) {
						if(GetNodeDisplayText(modelNode).ToLower().Contains(filterText)) {
							result = true;
						}
						else {
							if(checkNodeLevelLimiter < 20) { 
								result = FindInSubNode(modelNode);
							}
							else {
								result = false;
							}
						}
					}
					else {
						result = false;
					}
					sortedNodeKeys.Add(modelNode.GetHashCode(), result);
				}
				return result;
			}
			finally {
				checkNodeLevelLimiter--;
			}
		}
		private bool FindInSubNode(ModelNode node) {
			List<ModelNode> result;
			if(!childrenNodes.TryGetValue(node.GetHashCode(), out result)) {
				result = new List<ModelNode>();
				foreach(ModelNode modelNode in ModelEditorHelper.GetChildNodes(node)) {
					if(CheckNode(modelNode)) {
						result.Add(modelNode);
					}
				}
				result.Sort(new ModelTreeListNodeComparer(fastModelEditorHelper));
				childrenNodes.Add(node.GetHashCode(), result);
			}
			return result.Count > 0;
		}
		private string GetNodeTypePath(ModelNode node, string prefix) {
			string result = prefix;
			if(node.Parent != null) {
				result = node.Parent.GetType().Name + "." + result;
				result = GetNodeTypePath(node.Parent, result);
			}
			return result;
		}
		private void filteringTree_VirtualTreeSetCellValue(object sender, VirtualTreeSetCellValueInfo e) { }
		private void filteringTree_VirtualTreeGetChildNodes(object sender, VirtualTreeGetChildNodesInfo e) {
			if(!isLoadBegin) {
				isLoadBegin = true;
				List<ModelNode> rootNode_ = new List<ModelNode>();
				if(!string.IsNullOrEmpty(filterText) && selectedNodePaths != null) {
					CheckNode(rootNode);
					rootNode_.Add(rootNode);
				}
				e.Children = rootNode_;
			}
			else {
				if(e.Node is ModelNode) {
					e.Children = GetChildrenNodes((ModelNode)e.Node);
				}
			}
		}
		private void filteringTree_VirtualTreeGetCellValue(object sender, VirtualTreeGetCellValueInfo e) {
			e.CellData = GetNodeDisplayText(e.Node);
		}
		private string GetNodeDisplayText(object node) {
			return dataHelper.GetModelNodeDisplayValue_Cached((ModelNode)node);
		}
		private void SearchTextHighlight(CustomDrawNodeCellEventArgs e) {
			e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
			if(!string.IsNullOrEmpty(filterText) && !e.Node.Focused) {
				int indexOf = e.CellText.ToLower().IndexOf(filterText);
				if(indexOf != -1) {
					InitBrushes();
					string beforeHighlightTextValue = e.CellText.Substring(0, indexOf);
					string highlightTextValue = e.CellText.Substring(indexOf, filterText.Length);
					string afterHighlightTextValue = e.CellText.Substring(indexOf + filterText.Length);
					GraphicsCache cache = new GraphicsCache(e.Graphics);
					SizeF beforeHighlightTextSize = e.Appearance.CalcTextSize(e.Graphics, beforeHighlightTextValue, e.Bounds.Width);
					SizeF highlightTextSize = e.Appearance.CalcTextSize(e.Graphics, highlightTextValue, e.Bounds.Width);
					SizeF afterHighlightTextSize = e.Appearance.CalcTextSize(e.Graphics, afterHighlightTextValue, e.Bounds.Width);
					Rectangle beforeHighlightTextBounds = new Rectangle(e.Bounds.X, e.Bounds.Y, (int)beforeHighlightTextSize.Width, e.Bounds.Height);
					Rectangle highlightTextBounds = new Rectangle(e.Bounds.X + beforeHighlightTextBounds.Width, e.Bounds.Y, (int)highlightTextSize.Width, e.Bounds.Height);
					Rectangle afterHighlightTextBounds = new Rectangle(highlightTextBounds.X + highlightTextBounds.Width, e.Bounds.Y, (int)afterHighlightTextSize.Width, e.Bounds.Height);
					int highlightBounds_Y = highlightTextBounds.Y;
					if(e.Bounds.Height > (int)highlightTextSize.Height) {
						highlightBounds_Y = ((e.Bounds.Height - (int)highlightTextSize.Height) / 2) + highlightTextBounds.Y;
					}
					Rectangle highlightBounds = new Rectangle(highlightTextBounds.X, highlightBounds_Y,
						highlightTextBounds.Width, (int)highlightTextSize.Height);
					e.Graphics.FillRectangle(limeGreenBrush, highlightBounds);
					e.Appearance.DrawString(cache, beforeHighlightTextValue, beforeHighlightTextBounds, windowText);
					e.Appearance.DrawString(cache, highlightTextValue, highlightTextBounds, highlightText);
					e.Appearance.DrawString(cache, afterHighlightTextValue, afterHighlightTextBounds, windowText);
					e.Handled = true;
				}
			}
		}
		bool isInitBrushes = false;
		private void InitBrushes() {
			if(!isInitBrushes) {
				isInitBrushes = true;
				limeGreenBrush = new SolidBrush(SystemColors.Highlight);
				windowText = new SolidBrush(ViewInfo.PaintAppearance.Row.ForeColor);
				highlightText = new SolidBrush(SystemColors.HighlightText);
			}
		}
		private Brush limeGreenBrush;
		private Brush windowText;
		private Brush highlightText;
		#region IVirtualTreeListData Members
		public void SetRootNode(ModelNode rootNode) {
			if(this.rootNode == null) {
				this.rootNode = rootNode;
			}
		}
		private ModelNode rootNode = null;
		private bool isLoadBegin = false;
		#endregion
	}
}
