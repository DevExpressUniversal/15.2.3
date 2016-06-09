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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class LinksNodeHelper {
		FastModelEditorHelper fastModelEditorHelper;
		public LinksNodeHelper(FastModelEditorHelper fastModelEditorHelper) {
			this.fastModelEditorHelper = fastModelEditorHelper;
		}
		public IList<DevExpress.ExpressApp.Actions.ChoiceActionItem> GetCreatableItems(ModelTreeListNode node, ExtendModelInterfaceAdapter extendModelInterfaceAdapter) {
			if(node.ModelTreeListNodeType != ModelTreeListNodeType.Links) {
				List<ChoiceActionItem> result = new List<ChoiceActionItem>();
				foreach(KeyValuePair<string, Type> item in GetCreatableItems(node)) {
					result.Add(new ChoiceActionItem(item.Key, item.Value));
				}
				return result;
			}
			else {
				return GetLinksCreatableItems(node, extendModelInterfaceAdapter);
			}
		}
		private Dictionary<string, Type> GetCreatableItems(ModelTreeListNode node) {
			Dictionary<string, Type> result = null;
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Primary ||
				node.ModelTreeListNodeType == ModelTreeListNodeType.CollectionItem) {
				result = fastModelEditorHelper.GetChildNodeTypes(node.ModelNode);
			}
			else {
				result = GetLinkedChildNodesTypes(node.ModelNode, node.Owner.ModelNode);
			}
			return FilterCreatableItems(result, node.ModelNode);
		}
		private Dictionary<string, Type> FilterCreatableItems(Dictionary<string, Type> items, ModelNode targetNode) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			if(items != null) {
				foreach(KeyValuePair<string, Type> item in items) {
					ModelVirtualTreeCreatableItemsFilterAttribute[] filterAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeCreatableItemsFilterAttribute>(item.Value, true);
					bool canAdd = true;
					foreach(ModelVirtualTreeCreatableItemsFilterAttribute att in filterAttributes) {
						if(att.FilteredTypes != null && canAdd) {
							foreach(Type filerType in att.FilteredTypes) {
								if(filerType.IsAssignableFrom(targetNode.GetType())) {
									canAdd = false;
									break;
								}
							}
						}
					}
					if(canAdd) {
						result.Add(item.Key, item.Value);
					}
				}
			}
			return result;
		}
		private IList<DevExpress.ExpressApp.Actions.ChoiceActionItem> GetLinksCreatableItems(ModelTreeListNode node, ExtendModelInterfaceAdapter extendModelInterfaceAdapter) {
			Dictionary<string, ChoiceActionItem> choiceActionTree = new Dictionary<string, ChoiceActionItem>();
			ChoiceActionItem root = new ChoiceActionItem();
			choiceActionTree[ModelEditorHelper.GetModelNodePath(node.Root.ModelNode)] = root;
			List<string[]> addModelNodePaths = new List<string[]>();
			foreach(AssociationInfo aliases in extendModelInterfaceAdapter.AssociationCollection.GetAssociations(node.Owner.ModelNode.NodeInfo.GeneratedClass)) {
				addModelNodePaths.Add(aliases.Path);
			}
			foreach(string[] addModelNodePath in addModelNodePaths) {
				string listModelNodeTypeName = null;
				ModelNode modelNode = FindModelNodeBySchemaPath(node.Root.ModelNode.Application, addModelNodePath, out listModelNodeTypeName);
				if(modelNode != null) {
					AddNodeToChoiceActionTree(node, choiceActionTree, modelNode, listModelNodeTypeName);
				}
			}
			return root.Items;
		}
		private Dictionary<string, Type> GetLinkedChildNodesTypes(ModelNode node, ModelNode ownerModelNode) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			foreach(KeyValuePair<string, Type> item in fastModelEditorHelper.GetChildNodeTypes(node)) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(item.Value);
				foreach(IMemberInfo memberInfo in typeInfo.OwnMembers) {
					if(memberInfo.MemberType.IsAssignableFrom(ownerModelNode.GetType()) && memberInfo.MemberType != typeof(Object)) {
						BrowsableAttribute browsableAttribute = fastModelEditorHelper.GetPropertyAttribute<BrowsableAttribute>(typeInfo, memberInfo.Name);
						bool isVisible = browsableAttribute == null ? true : browsableAttribute.Browsable;
						if(!result.ContainsKey(item.Key) && isVisible) {
							result.Add(item.Key, item.Value);
						}
					}
				}
			}
			return result;
		}
		private ModelNode FindModelNodeBySchemaPath(IModelApplication modelApplication, string[] schemaPath, out string listModelNodeTypeName) {
			if(schemaPath == null || schemaPath.Length < 2) {
				listModelNodeTypeName = null;
				return null;
			}
			ModelNode currentModelNode = (ModelNode)modelApplication;
			for(int i = 0; i < schemaPath.Length - 1; i++) {
				string id = schemaPath[i];
				currentModelNode = currentModelNode[id];
				if(currentModelNode == null) {
					listModelNodeTypeName = null;
					return null;
				}
			}
			listModelNodeTypeName = schemaPath[schemaPath.Length - 1];
			return currentModelNode;
		}
		private ChoiceActionItem CreateChoiceActionItemForActionTree(Dictionary<string, DevExpress.ExpressApp.Actions.ChoiceActionItem> choiceActionTree, ModelNode modelNodePath) {
			DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
			choiceActionItem.Caption = fastModelEditorHelper.GetModelNodeDisplayValue(modelNodePath);
			string nodePath = ModelEditorHelper.GetModelNodePath(modelNodePath);
			choiceActionItem.Data = nodePath;
			choiceActionTree[nodePath] = choiceActionItem;
			return choiceActionItem;
		}
		private void CreateTargetChoiceActionItem(ChoiceActionItem parent, string caption, Type modelNodeType) {
			bool isActionItemExists = false;
			foreach(ChoiceActionItem item in parent.Items) {
				if((Type)item.Data == modelNodeType) {
					isActionItemExists = true;
					break;
				}
			}
			if(!isActionItemExists) {
				DevExpress.ExpressApp.Actions.ChoiceActionItem childModelNodeActionItem = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
				childModelNodeActionItem.Caption = caption;
				childModelNodeActionItem.Data = modelNodeType;
				parent.Items.Add(childModelNodeActionItem);
			}
		}
		private void AddNodeToChoiceActionTree(ModelTreeListNode node, Dictionary<string, DevExpress.ExpressApp.Actions.ChoiceActionItem> choiceActionTree, ModelNode modelNode, string targetNodeTypeName) {
			Dictionary<string, Type> visibleChildNodesTypes = GetLinkedChildNodesTypes(modelNode, node.Owner.ModelNode);
			Type targetNodeType = null;
			if(visibleChildNodesTypes.TryGetValue(targetNodeTypeName, out targetNodeType)) {
				string modelNodePath = ModelEditorHelper.GetModelNodePath(modelNode);
				DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem = null;
				bool isModelNodeExists = true;
				if(!choiceActionTree.TryGetValue(modelNodePath, out choiceActionItem)) {
					choiceActionItem = CreateChoiceActionItemForActionTree(choiceActionTree, modelNode);
					isModelNodeExists = false;
				}
				CreateTargetChoiceActionItem(choiceActionItem, targetNodeTypeName, targetNodeType);
				if(!isModelNodeExists) {
					ModelNode parentNode = modelNode.Parent;
					DevExpress.ExpressApp.Actions.ChoiceActionItem currentChoiceActionItem = choiceActionItem;
					do {
						string parentPath = ModelEditorHelper.GetModelNodePath(parentNode);
						DevExpress.ExpressApp.Actions.ChoiceActionItem parentItem;
						if(!choiceActionTree.TryGetValue(parentPath, out parentItem)) {
							parentItem = CreateChoiceActionItemForActionTree(choiceActionTree, parentNode);
							parentNode = parentNode.Parent;
						}
						else {
							parentNode = null;
						}
						parentItem.Items.Add(currentChoiceActionItem);
						currentChoiceActionItem = parentItem;
					} while(parentNode != null);
				}
			}
		}
	}
}
