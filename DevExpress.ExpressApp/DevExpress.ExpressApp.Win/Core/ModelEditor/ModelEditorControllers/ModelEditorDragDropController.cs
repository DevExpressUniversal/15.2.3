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
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public abstract class ModelEditorDragDropController : ModelEditorControllerBase {
		private List<ModelTreeListNode> draggedNodes = new List<ModelTreeListNode>();
		private ModelTreeListNode nodeToCreate;
		private ModelTreeListNode targetNode;
		private bool doIndexChange = false;
		public ModelEditorDragDropController(IModelApplication modelApplication)
			: base(modelApplication) {
		}
#if DebugTest
		public void SetDragDropTargetNode(ModelTreeListNode node) {
			targetNode = node;
		}
		public void SetDragDropDraggedNodes(List<ModelTreeListNode> draggedNodes) {
			this.draggedNodes = draggedNodes;
		}
		public void SetDoIndexChange(bool doIndexChange) {
			this.doIndexChange = doIndexChange;
		}
		public DragDropEffects DebugTest_CustomDragOver(int keyState, ModelTreeListNode destinationNode) {
			return CustomDragOver(keyState, destinationNode);
		}
		public void DebugTest_CustomDragDrop(DragDropEffects effect) {
			CustomDragDrop(effect);
		}
		public void DebugTest_ChangeNodeIndex(ModelTreeListNode targetNode, ModelTreeListNode draggedNode) {
			ChangeNodeIndex(targetNode, draggedNode);
		}
#endif
		private void modelTreeList_DragDrop(object sender, DragEventArgs e) {
			SafeExecute(delegate() {
				CustomDragDrop(e.Effect);
				e.Effect = DragDropEffects.None;
			});
		}
		private void CustomDragDrop(DragDropEffects effect) {
			UpdatedTreeListCall(delegate() {
				Adapter.SortNodes(draggedNodes);
				List<ModelTreeListNode> newNodes = new List<ModelTreeListNode>();
				List<TreeListNode> nodesForSelect = new List<TreeListNode>();
				foreach(ModelTreeListNode draggedNode in draggedNodes) {
					ModelTreeListNode node = CustomDragDrop(effect, draggedNode);
					if(node != null) {
						newNodes.Add(node);
					}
				}
				if(ModelEditorControl != null) {
					foreach(ModelTreeListNode newNode in newNodes) {
						ObjectTreeListNode controlNode = ModelEditorControl.modelTreeList.FindBuiltAncestorNode(newNode);
						nodesForSelect.Add(controlNode);
					}
				}
				if(newNodes.Count > 0) {
					CurrentModelNode = newNodes[0];
				}
				SelectNodes(nodesForSelect);
			});
		}
		private ModelTreeListNode CustomDragDrop(DragDropEffects effect, ModelTreeListNode draggedNode) {
			if(targetNode != null && TrySetModified()) {
				ModelTreeListNode newNode = null;
				UpdatedTreeListCall(delegate() {
					if(draggedNode.VirtualTreeNode) {
						newNode = VirtualTreeDragDrop(effect, draggedNode);
					}
					else {
						newNode = StandartDragDrop(effect, draggedNode);
					}
				});
				return newNode;
			}
			return null;
		}
		private ModelTreeListNode VirtualTreeDragDrop(DragDropEffects effect, ModelTreeListNode draggedNode) {
			ModelTreeListNode newNode = null;
			if(effect == DragDropEffects.Move) {
				if(doIndexChange) {
					ChangeNodeIndex(targetNode, draggedNode);
					if(ModelEditorControl != null) {
						newNode = draggedNode;
					}
				}
				else {
					newNode = VirtualTreeDragDropCore(targetNode, draggedNode);
				}
			}
			return newNode;
		}
		protected ModelTreeListNode VirtualTreeDragDropCore(ModelTreeListNode targetNode, ModelTreeListNode draggedNode) {
			ModelTreeListNode newNode = null;
			if(targetNode.ModelVirtualTreeSetParent(targetNode, draggedNode)) {
				newNode = Adapter.CloneNode(targetNode, draggedNode, false);
				Unsubscribe(draggedNode.ModelNode);
				Adapter.DeleteNode(draggedNode, false);
			}
			return newNode;
		}
		private ModelTreeListNode StandartDragDrop(DragDropEffects effect, ModelTreeListNode draggedNode) {
			ModelTreeListNode newNode = null;
			if(effect == DragDropEffects.Move) {
				if(doIndexChange) {
					ChangeNodeIndex(targetNode, draggedNode);
					if(ModelEditorControl != null) {
						newNode = draggedNode;
					}
				}
				else {
					newNode = Adapter.CloneNode(targetNode, draggedNode, true);
					Adapter.DeleteNode(draggedNode);
				}
			}
			else {
				newNode = Adapter.CloneNode(targetNode, draggedNode, true);
			}
			return newNode;
		}
		private void ChangeNodeIndex(ModelTreeListNode targetNode, ModelTreeListNode draggedNode) {
			int counter = 0;
			int targetNodeIndex = 0;
			foreach(ModelTreeListNode item in Adapter.GetChildren(targetNode.Parent)) {
				if(ModelEditorHelper.IsNodeEqual(GetModelNode(targetNode), item.ModelNode)) {
					targetNodeIndex = counter;
					break;
				}
				counter++;
			}
			if(draggedNodes.Count > 1) {
				int itemIndex = draggedNodes.IndexOf(draggedNode);
				if(itemIndex > 0) {
					ModelTreeListNode prevItem = draggedNodes[itemIndex - 1];
					int prevNodeIndex = (int)prevItem.ModelNode.Index;
					if(targetNodeIndex < prevNodeIndex) {
						targetNodeIndex = prevNodeIndex;
					}
				}
			}
			ChangeNodeIndex(draggedNode, targetNodeIndex);
		}
		private void modelTreeList_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e) {
			SafeExecute(delegate() {
				if((e.DragArgs.KeyState & 4) == 0) {
					e.ImageIndex = 0;
				}
			});
		}
		private DragDropEffects CustomDragOver(int keyState, ModelTreeListNode destinationNode) {
			DragDropEffects result = DragDropEffects.None;
			if(destinationNode == null) {
				return DragDropEffects.None;
			}
			draggedNodes.Clear();
			foreach(ModelTreeListNode draggedNode in SelectedNodes) {
				if(draggedNode.ModelTreeListNodeType == ModelTreeListNodeType.Links ||
					draggedNode.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
					return DragDropEffects.None;
				}
				DragDropEffects dragDropEffectForDraggedNode = CustomDragOver(keyState, destinationNode, draggedNode);
				if(dragDropEffectForDraggedNode == DragDropEffects.None) {
					return DragDropEffects.None;
				}
				if(result != DragDropEffects.None && result != dragDropEffectForDraggedNode) {
					return DragDropEffects.None;
				}
				result = dragDropEffectForDraggedNode;
				draggedNodes.Add(draggedNode);
			}
			return result;
		}
		public bool CanDragNode(ModelTreeListNode destinationNode, ModelTreeListNode draggedNode) {
			if(destinationNode == null || draggedNode == null) {
				return false;
			}
			if(draggedNode.VirtualTreeNode) {
				if(!destinationNode.VirtualTreeNode) {
					return false;
				}
				IModelNode destNodeModel = GetModelNode(destinationNode);
				IModelNode draggedNodeModel = GetModelNode(draggedNode);
				if(destNodeModel == null || draggedNodeModel == null) {
					return false;
				}
				bool canDrag = false;
				ModelVirtualTreeDragDropItemAttribute[] dragDropAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeDragDropItemAttribute>(destNodeModel.GetType(), true);
				if(dragDropAttributes != null) {
					foreach(ModelVirtualTreeDragDropItemAttribute att in dragDropAttributes) {
						if(att.SupportedTypes != null) {
							foreach(Type supportedType in att.SupportedTypes) {
								if(supportedType.IsAssignableFrom(draggedNodeModel.GetType())) {
									canDrag = true;
									break;
								}
							}
						}
					}
				}
				return canDrag;
			}
			else {
				return Adapter.fastModelEditorHelper.CanDragNode(GetModelNode(destinationNode), GetModelNode(draggedNode));
			}
		}
		private DragDropEffects CustomDragOver(int keyState, ModelTreeListNode destinationNode, ModelTreeListNode draggedNode) {
			DragDropEffects result = DragDropEffects.None;
			targetNode = destinationNode;
			doIndexChange = (keyState & 4) != 0;
			if(ModelEditorControl != null) {
				ModelEditorControl.modelTreeList.OptionsDragAndDrop.ExpandNodeOnDrag = !doIndexChange;
			}
			if(!doIndexChange) {
				bool isCopy = (keyState == 9);
				bool canDragNode = CanDragNode(destinationNode, draggedNode);
				if(isCopy && canDragNode) {
					canDragNode = Adapter.fastModelEditorHelper.CanAddNode(GetModelNode(destinationNode), GetModelNode(draggedNode));
				}
				if(!isCopy) {
					canDragNode = canDragNode && !TargetNodeIsChildDraggedNode(GetModelNode(draggedNode), GetModelNode(destinationNode)) && !ModelEditorHelper.IsNodeEqual(GetModelNode(draggedNode.Parent), GetModelNode(destinationNode));
					canDragNode &= draggedNode.Parent == null || draggedNode.Parent.ModelTreeListNodeType != ModelTreeListNodeType.Group;
				}
				if(canDragNode) {
					result = isCopy ? DragDropEffects.Copy : DragDropEffects.Move;
				}
			}
			else {
				bool nodesOneLevel = ModelEditorHelper.IsNodeEqual(GetModelNode(draggedNode.Parent), GetModelNode(destinationNode.Parent));
				if(nodesOneLevel && draggedNode.ModelNode.Id != destinationNode.ModelNode.Id) {
					result = DragDropEffects.Move;
				}
			}
			return result;
		}
		private void modelTreeList_DragOver(object sender, DragEventArgs drgevent) {
			SafeExecute(delegate() {
				Point pt = ModelEditorControl.modelTreeList.PointToClient(new Point(drgevent.X, drgevent.Y));
				TreeListHitInfo hitInfo = ModelEditorControl.modelTreeList.CalcHitInfo(pt);
				drgevent.Effect = DragDropEffects.None;
				if(hitInfo.Node != null && CurrentModelNode != null) {
					drgevent.Effect = CustomDragOver(drgevent.KeyState, GetModelTreeListNode(hitInfo.Node));
				}
			});
		}
		private bool TargetNodeIsChildDraggedNode(ModelNode draggedNode, ModelNode targetNode) {
			ModelNode parent = targetNode;
			while(parent != null) {
				if(ModelEditorHelper.IsNodeEqual(draggedNode, parent)) {
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}
		private void modelTreeList_CreateCustomNode(object sender, DevExpress.XtraTreeList.CreateCustomNodeEventArgs e) {
			SafeExecute(delegate() {
				if(nodeToCreate != null) {
					ObjectTreeListNode result = new ObjectTreeListNode(e.NodeID, e.Owner);
					result.Object = nodeToCreate;
					e.Node = result;
					nodeToCreate = null;
				}
			});
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			ModelEditorControl.modelTreeList.CreateCustomNode += new DevExpress.XtraTreeList.CreateCustomNodeEventHandler(modelTreeList_CreateCustomNode);
			ModelEditorControl.modelTreeList.DragOver += new DragEventHandler(modelTreeList_DragOver);
			ModelEditorControl.modelTreeList.CalcNodeDragImageIndex += new CalcNodeDragImageIndexEventHandler(modelTreeList_CalcNodeDragImageIndex);
			ModelEditorControl.modelTreeList.DragDrop += new DragEventHandler(modelTreeList_DragDrop);
		}
		protected override void UnSubscribeEvents() {
			base.UnSubscribeEvents();
			if(ModelEditorControl != null) {
				ModelEditorControl.modelTreeList.CreateCustomNode -= new DevExpress.XtraTreeList.CreateCustomNodeEventHandler(modelTreeList_CreateCustomNode);
				ModelEditorControl.modelTreeList.DragOver -= new DragEventHandler(modelTreeList_DragOver);
				ModelEditorControl.modelTreeList.CalcNodeDragImageIndex -= new CalcNodeDragImageIndexEventHandler(modelTreeList_CalcNodeDragImageIndex);
				ModelEditorControl.modelTreeList.DragDrop -= new DragEventHandler(modelTreeList_DragDrop);
			}
		}
		public override void Dispose() {
			if(draggedNodes != null) {
				draggedNodes.Clear();
				draggedNodes = null;
			}
			base.Dispose();
		}
		public override void SelectNodes(List<TreeListNode> nodes) {
			if(ModelEditorControl != null) {
				base.SelectNodes(nodes);
			}
			else {
#if DebugTest
				selectedNodesForTests.Clear();
				foreach(ModelTreeListNode node in draggedNodes) {
					selectedNodesForTests.Add(node);
				}
#endif
			}
		}
	}
}
