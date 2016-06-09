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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraCharts.Designer {
	public partial class ChartStructureControl : XtraUserControl {
		const int diagramNodeIndex = 1;
		const int chartTitlesCollectionNodeIndex = 2;
		const int defaultTreeExpansionLevel = 1;
		DesignerChartModel chartModel;
		object selectedElement;
		object highlightedElement;
		TreeListNode selectedNode;
		TreeListNode highlightedNode;
		bool popupOpened = false;
		DesignerDiagramModel DiagramModel {
			get { return ChartModel != null ? ChartModel.Diagram : null; }
		}
		XYDiagramModel XYDiagramModel { get { return ChartModel != null ? ChartModel.Diagram as XYDiagramModel : null; } }
		internal DesignerChartModel ChartModel {
			get { return chartModel; }
			set {
				chartModel = value;
				OnChartChanged();
			}
		}
		internal object SelectedElement {
			get { return selectedElement; }
			set {
				if (selectedElement != value) {
					selectedElement = value;
					OnSelectedElementChanged();
				}
			}
		}
		internal object HighlightedElement {
			get { return highlightedElement; }
			set {
				if (highlightedElement != value) {
					highlightedElement = value;
					OnHighlightedElementChanged();
				}
			}
		}
		internal event ChartTreeSelectionChanged SelectionChanged;
		internal event ChartTreeHighlightedItemChanged HighlightedItemChanged;
		internal event HotKeyPressedEventHandler HotKeyPressed;
		public ChartStructureControl() {
			InitializeComponent();
			DesignerImageHelper.PushTreeImagesToList(liTreeImages);
		}
		void OnSelectedElementChanged() {
			TreeListNode nodeToSelect = FindTreeNode(selectedElement, tlChartTree.Nodes[0]);
			UpdateSelectedNode(nodeToSelect);
			ChartTreeElement treeElement = nodeToSelect != null ? (ChartTreeElement)nodeToSelect.GetValue(0) : null;
			if (treeElement != null && treeElement.ChartModel != null)
				SelectionChanged(new ChartTreeSelectionChangedEventArgs(treeElement.ChartModel));
		}
		void OnHighlightedElementChanged() {
			if (popupOpened)
				return;
			TreeListNode nodeToHighlight = highlightedElement != null ? FindTreeNode(highlightedElement, tlChartTree.Nodes[0]) : null;
			tlChartTree.Focus();
			tlChartTree.FocusedNode = nodeToHighlight;
		}
		void OnButtonEditPlusPressed(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			if (tlChartTree.FocusedNode != null) {
				UpdateSelectedNode(tlChartTree.FocusedNode);
				ChartTreeCollectionElement treeCollectionElement = (ChartTreeCollectionElement)tlChartTree.FocusedNode.GetValue(0);
				if (treeCollectionElement != null)
					treeCollectionElement.AddNewElement();
			}
		}
		void OnButtonEditEyeDeletePressed(object sender, ButtonPressedEventArgs e) {
			ChartTreeElement treeElement = (ChartTreeElement)tlChartTree.FocusedNode.GetValue(0);
			if (e.Button.Tag != null && e.Button.Tag.Equals("Delete"))
				treeElement.ParentCollection.DeleteElement(treeElement);
			else {
				ISupportModelVisibility model = treeElement.ChartModel as ISupportModelVisibility;
				if (model != null)
					model.Visible = !model.Visible;
			}
		}
		RepositoryItem GetNodeButton(bool isCollection, bool hasGallery, bool isCollectionElement, bool isHighLighted, bool supportsVisibility, bool visible, bool isAutoCreatedSeries) {
			if (isAutoCreatedSeries)
				return defaultButtonEdit;
			if (!isHighLighted)
				return defaultButtonEdit;
			if (isCollection && hasGallery)
				return popupContainerEdit;
			if (isCollection)
				return buttonEditPlus;
			if (isCollectionElement && supportsVisibility)
				return visible ? buttonEditEyeDelete : buttonEditDisabledEyeDelete;
			if (isCollectionElement && !supportsVisibility)
				return buttonEditDelete;
			if (supportsVisibility)
				return visible ? buttonEditEye : buttonEditDisabledEye;
			return defaultButtonEdit;
		}
		void OnChartTreeCustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e) {
			ChartTreeElement treeElement = e.Node.GetValue(0) as ChartTreeElement;
			if (treeElement != null) {
				ISupportModelVisibility modelVisibility = treeElement.ChartModel as ISupportModelVisibility;
				var collectionElement = treeElement as ChartTreeCollectionElement;
				bool isCollectionNode = collectionElement != null;
				bool hasGallery = false;
				if (collectionElement != null)
					hasGallery = collectionElement.HasGalleryForElementAdding;
				bool isCollectionElement = treeElement.ParentCollection != null;
				bool supportsVisibility = modelVisibility != null;
				bool visible = modelVisibility == null || modelVisibility.Visible;
				bool isHighlighted = e.Node.Selected || e.Node.Focused;
				e.RepositoryItem = GetNodeButton(isCollectionNode, hasGallery, isCollectionElement, isHighlighted,
					supportsVisibility, visible, treeElement.ChartModel.GetType() == typeof(DesignerSeriesModelBase));
			}
		}
		void OnChartTreeNodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e) {
			ChartTreeElement treeElement = e.Node.GetValue(0) as ChartTreeElement;
			if (treeElement != null) {
				ISupportModelVisibility modelVisibility = treeElement.ChartModel as ISupportModelVisibility;
				if (modelVisibility != null && !modelVisibility.Visible)
					e.Appearance.ForeColor = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.GrayText);
				if (e.Node.Focused)
					e.Appearance.BackColor = Color.Transparent;
			}
		}
		void OnChartTreeMouseMove(object sender, MouseEventArgs e) {
			if (popupOpened)
				return;
			if (!tlChartTree.Focused)
				tlChartTree.Focus();
			TreeListHitInfo hitInfo = tlChartTree.CalcHitInfo(e.Location);
			TreeListNode node = hitInfo.Node;
			if (node != null)
				tlChartTree.FocusedNode = node;
		}
		void OnChartTreeButtonEditClick(object sender, EventArgs e) {
			if (UpdateSelectedNode(tlChartTree.FocusedNode))
				UpdateSelectedElement(tlChartTree.FocusedNode);
		}
		void OnChartTreeButtonDoubleClick(object sender, EventArgs e) {
			if (highlightedNode != null)
				highlightedNode.Expanded = !highlightedNode.Expanded;
		}
		void OnChartTreeFocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			if (UpdateHighlightedNode(e.Node))
				UpdateHighlightedElement(e.Node);
		}
		void OnChartTreeSelectionChanged(object sender, EventArgs e) {
			if (tlChartTree.Selection.Count > 2)
				tlChartTree.Selection.Set(new TreeListNode[] { selectedNode, highlightedNode });
		}
		bool CanSelectNode(TreeListNode node) {
			return node != null && !(node.GetValue(0) is ChartTreeCollectionElement);
		}
		bool UpdateSelectedNode(TreeListNode nodeToSelect) {
			if (!CanSelectNode(nodeToSelect) || selectedNode == nodeToSelect)
				return false;
			tlChartTree.BeginUpdate();
			if (selectedNode != null)
				selectedNode.Selected = false;
			if (nodeToSelect != null) {
				nodeToSelect.Selected = true;
				tlChartTree.MakeNodeVisible(nodeToSelect);
			}
			selectedNode = nodeToSelect;
			tlChartTree.EndUpdate();
			return true;
		}
		bool UpdateHighlightedNode(TreeListNode nodeToHighlight) {
			if (highlightedNode == nodeToHighlight)
				return false;
			tlChartTree.BeginUpdate();
			if (highlightedNode != null && highlightedNode != selectedNode)
				highlightedNode.Selected = false;
			if (nodeToHighlight != null)
				nodeToHighlight.Selected = true;
			highlightedNode = nodeToHighlight;
			tlChartTree.EndUpdate();
			return true;
		}
		void UpdateHighlightedElement(TreeListNode node) {
			if (HighlightedItemChanged != null) {
				DesignerChartElementModelBase highlightedModel = null;
				if (node != null) {
					ChartTreeElement treeElement = (ChartTreeElement)node.GetValue(0);
					highlightedModel = treeElement.ChartModel;
					highlightedElement = highlightedModel != null ? highlightedModel.ChartElement : null;
				}
				HighlightedItemChanged(new ChartTreeHighlightedItemChangedEventArgs(highlightedModel));
			}
		}
		void UpdateSelectedElement(TreeListNode node) {
			if (SelectionChanged != null) {
				DesignerChartElementModelBase selectedModel = null;
				if (node != null) {
					ChartTreeElement treeElement = (ChartTreeElement)node.GetValue(0);
					selectedModel = treeElement.ChartModel;
					selectedElement = selectedModel != null ? selectedModel.ChartElement : null;
				}
				SelectionChanged(new ChartTreeSelectionChangedEventArgs(selectedModel));
			}
		}
		void OnChartTreeMouseLeave(object sender, EventArgs e) {
			if (popupOpened)
				return;
			tlChartTree.FocusedNode = null;
		}
		void AppendTreeElement(TreeListNode parentNode, DesignerChartElementModelBase chartModel) {
			ChartTreeCollectionElement collectionElement = parentNode != null ? parentNode.GetValue(0) as ChartTreeCollectionElement : null;
			ChartTreeElement treeElement = chartModel.GetTreeElement(collectionElement);
			if (treeElement != null) {
				string imageKey = chartModel.ChartTreeImageKey;
				ChartCollectionBaseModel collectionModel = chartModel as ChartCollectionBaseModel;
				if (collectionModel != null)
					imageKey += DesignerImageKeys.CollectionPrefix;
				int imageIndex = liTreeImages.Images.IndexOfKey(imageKey);
				int parentNodeId = parentNode != null ? parentNode.Id : -1;
				TreeListNode elementNode = tlChartTree.AppendNode(new object[] { treeElement }, parentNodeId, imageIndex, imageIndex, imageIndex);
				if (treeElement.NodeIndex >= 0)
					tlChartTree.SetNodeIndex(elementNode, treeElement.NodeIndex);
				foreach (DesignerChartElementModelBase childModel in chartModel.Children)
					AppendTreeElement(elementNode, childModel);
			}
			else if (!(chartModel is DesignerSeriesModel))
				foreach (DesignerChartElementModelBase childModel in chartModel.Children)
					AppendTreeElement(parentNode, childModel);
		}
		void ResetTreeElements(TreeListNode parentNode) {
			ChartTreeElement treeElement = parentNode != null ? parentNode.GetValue(0) as ChartTreeElement : null;
			if (treeElement != null)
				treeElement.Updated = false;
			foreach (TreeListNode node in parentNode.Nodes)
				ResetTreeElements(node);
		}
		void UpdateTreeElement(TreeListNode parentNode, DesignerChartElementModelBase chartModel) {
			ChartTreeElement parentTreeElement = parentNode != null ? parentNode.GetValue(0) as ChartTreeElement : null;
			if (parentTreeElement != null) {
				foreach (DesignerChartElementModelBase childModel in chartModel.Children) {
					ChartTreeElement newChildTreeElement = childModel.GetTreeElement(parentTreeElement as ChartTreeCollectionElement);
					if (newChildTreeElement != null) {
						bool nodeExist = false;
						foreach (TreeListNode childNode in parentNode.Nodes) {
							ChartTreeElement existChildTreeElement = childNode.GetValue(0) as ChartTreeElement;
							if (existChildTreeElement.AreModelsEquals(newChildTreeElement)) {
								childNode.SetValue(0, newChildTreeElement);
								UpdateTreeElement(childNode, childModel);
								nodeExist = true;
								break;
							}
						}
						if (!nodeExist) {
							string imageKey = childModel.ChartTreeImageKey;
							ChartCollectionBaseModel collectionModel = childModel as ChartCollectionBaseModel;
							if (collectionModel != null)
								imageKey += DesignerImageKeys.CollectionPrefix;
							int imageIndex = liTreeImages.Images.IndexOfKey(imageKey);
							int parentNodeId = parentNode != null ? parentNode.Id : -1;
							TreeListNode elementNode = tlChartTree.AppendNode(new object[] { newChildTreeElement }, parentNodeId, imageIndex, imageIndex, imageIndex);
							if (parentTreeElement is ChartTreeCollectionElement)
								tlChartTree.SetNodeIndex(elementNode, ((ChartCollectionBaseModel)chartModel).IndexOf(childModel));
							if (newChildTreeElement.NodeIndex >= 0)
								tlChartTree.SetNodeIndex(elementNode, newChildTreeElement.NodeIndex);
							UpdateTreeElement(elementNode, childModel);
						}
					}
					else if (!(childModel is DesignerSeriesModel) && !(childModel is PaneAnchorPointModel))
						UpdateTreeElement(parentNode, childModel);
				}
			}
		}
		void DeleteNodes(TreeListNode parentNode) {
			List<TreeListNode> nodesToRemove = new List<TreeListNode>();
			foreach (TreeListNode node in parentNode.Nodes) {
				ChartTreeElement treeElement = node.GetValue(0) as ChartTreeElement;
				if (!treeElement.Updated)
					nodesToRemove.Add(node);
			}
			foreach (TreeListNode node in nodesToRemove)
				tlChartTree.DeleteNode(node);
			foreach (TreeListNode node in parentNode.Nodes)
				DeleteNodes(node);
		}
		void OnChartChanged() {
			tlChartTree.Nodes.Clear();
			if (chartModel != null) {
				AppendTreeElement(tlChartTree.Nodes.FirstNode, chartModel);
				SelectedElement = chartModel.ChartElement;
			}
			tlChartTree.ExpandToLevel(defaultTreeExpansionLevel);
		}
		void popupContainerEdit_QueryPopUp(object sender, CancelEventArgs e) {
			if (tlChartTree.FocusedNode != null) {
				UpdateSelectedNode(tlChartTree.FocusedNode);
				ChartTreeCollectionElement treeCollectionElement = (ChartTreeCollectionElement)tlChartTree.FocusedNode.GetValue(0);
				ChartStructurePopupUserControl uc = treeCollectionElement.GetUserControlForPopup();
				uc.ClosePopup += uc_ClosePopup;
				popupContainerControl.Controls.Clear();
				popupContainerControl.Controls.Add(uc);
				uc.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			}
		}
		void uc_ClosePopup(object sender, EventArgs e) {
			popupContainerControl.OwnerEdit.ClosePopup();
		}
		void popupContainerEdit_Popup(object sender, EventArgs e) {
			popupOpened = true;
		}
		void popupContainerEdit_Closed(object sender, ClosedEventArgs e) {
			popupOpened = false;
			var uc = popupContainerControl.Controls[0] as ChartStructurePopupUserControl;
		}
		void popupContainerEdit_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			if (popupContainerControl.Controls.Count == 0)
				return;
			var uc = popupContainerControl.Controls[0] as ChartStructurePopupUserControl;
			if (uc == null) {
				ChartDebug.Fail("Only the ChartStructurePopupUserControl can be placed in content of the chart structure adding element popup.");
				return;
			}
			uc.ClosePopup -= uc_ClosePopup;
			ChartTreeCollectionElement treeCollectionElement = (ChartTreeCollectionElement)tlChartTree.FocusedNode.GetValue(0);
			treeCollectionElement.AddNewElement(uc.SelectedInPopupValue);
		}
		void popupContainerEdit_EditValueChanged(object sender, EventArgs e) {
			var uc = popupContainerControl.Controls[0] as ChartStructurePopupUserControl;
			uc.Dispose();
		}
		TreeListNode FindTreeNode(object chartlement, TreeListNode rootNode) {
			ChartTreeElement treeElement = (ChartTreeElement)rootNode.GetValue(0);
			if (treeElement != null && treeElement.ChartModel != null)
				if (treeElement.ChartModel.ChartElement == chartlement)
					return rootNode;
			foreach (TreeListNode node in rootNode.Nodes) {
				TreeListNode findedNode = FindTreeNode(chartlement, node);
				if (findedNode != null)
					return findedNode;
			}
			return null;
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch (keyData) {
				case Keys.Delete:
					if (HotKeyPressed != null)
						HotKeyPressed(this, new HotKeyPressedEventArgs(keyData));
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		internal void UpdateTree() {
			ResetTreeElements(tlChartTree.Nodes.FirstNode);
			UpdateTreeElement(tlChartTree.Nodes.FirstNode, chartModel);
			DeleteNodes(tlChartTree.Nodes.FirstNode);
		}
	}
}
namespace DevExpress.XtraCharts.Designer.Native {
	public enum TreeNodeUpdateType {
		None,
		Replace,
		Remove
	}
	public class ChartTreeSelectionChangedEventArgs : EventArgs {
		readonly DesignerChartElementModelBase selectedChartElementModel;
		public DesignerChartElementModelBase SelectedChartElementModel { get { return selectedChartElementModel; } }
		public ChartTreeSelectionChangedEventArgs(DesignerChartElementModelBase selectedChartElementModel) {
			this.selectedChartElementModel = selectedChartElementModel;
		}
	}
	public class ChartTreeHighlightedItemChangedEventArgs : EventArgs {
		readonly DesignerChartElementModelBase highlightedChartElementModel;
		public DesignerChartElementModelBase HighlightedChartElementModel { get { return highlightedChartElementModel; } }
		public ChartTreeHighlightedItemChangedEventArgs(DesignerChartElementModelBase highlightedChartElementModel) {
			this.highlightedChartElementModel = highlightedChartElementModel;
		}
	}
	public delegate void ChartTreeSelectionChanged(ChartTreeSelectionChangedEventArgs e);
	public delegate void ChartTreeHighlightedItemChanged(ChartTreeHighlightedItemChangedEventArgs e);
	public static class DesignerImageKeys {
		public const string AxisKey = "Axis";
		public const string AxisXKey = "AxisX";
		public const string AxisYKey = "AxisY";
		public const string SeriesKey = "Series";
		public const string LegendKey = "Legend";
		public const string PaneKey = "Pane";
		public const string ChartKey = "Chart";
		public const string TitleKey = "Title";
		public const string DiagramKey = "XYDiagram2D";
		public const string SeriesLabelKey = "SeriesLabel";
		public const string StripKey = "Strip";
		public const string ConstantLineKey = "ConstantLine";
		public const string ScaleBreakKey = "ScaleBreak";
		public const string IndicatorKey = "Indicator";
		public const string SeriesTitleKey = "SeriesTitle";
		public const string AnnotationKey = "Annotation";
		public const string TextAnnotationKey = "TextAnnotation";
		public const string ImageAnnotationKey = "ImageAnnotation";
		public const string CollectionPrefix = "Collection";
	}
	public static class DesignerImageHelper {
		static void PushImageToList(ImageList list, string imageKey, bool isCollection, Assembly asm) {
			if (isCollection)
				imageKey += DesignerImageKeys.CollectionPrefix;
			try {
				Image image = ResourceImageHelper.CreateBitmapFromResources(ImageResourcesUtils.DesignerImagePath + "ChartTree." + imageKey + ".png", asm);
				list.Images.Add(imageKey, image);
			}
			catch { };
		}
		public static void PushTreeImagesToList(ImageList list) {
			Assembly asm = typeof(DesignerImageHelper).Assembly;
			PushImageToList(list, DesignerImageKeys.ChartKey, false, asm);
			PushImageToList(list, DesignerImageKeys.LegendKey, false, asm);
			PushImageToList(list, DesignerImageKeys.SeriesKey, true, asm);
			PushImageToList(list, DesignerImageKeys.TitleKey, true, asm);
			PushImageToList(list, DesignerImageKeys.AnnotationKey, true, asm);
			PushImageToList(list, DesignerImageKeys.DiagramKey, false, asm);
		}
	}
}
