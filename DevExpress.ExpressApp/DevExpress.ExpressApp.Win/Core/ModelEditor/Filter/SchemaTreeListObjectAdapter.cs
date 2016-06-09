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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class SchemaTreeListObjectAdapter : NodeObjectAdapter, IDisposable {
		FastModelEditorHelper fastModelEditorHelper = new FastModelEditorHelper();
		ModelApplicationBase modelApplication = null;
		public ModelApplicationBase ModelApplication {
			get { return modelApplication; }
			set { modelApplication = value; }
		}
		public override IEnumerable GetChildren(object nodeObject) {
			ITypeInfo info = nodeObject as ITypeInfo;
			if(info == null) {
				info = ((IMemberInfo)nodeObject).MemberTypeInfo;
			}
			Item parentIntem = GetParentItem(info);
			List<ITypeInfo> result = new List<ITypeInfo>();
			ModelNodeInfo nodeInfo = modelApplication.CreatorInstance.GetNodeInfo(info.Type);
			foreach(KeyValuePair<string, Type> item in nodeInfo.GetChildrenTypes()) {
				if(CanReturnType(item.Value, parentIntem)) {
					BrowsableAttribute br = fastModelEditorHelper.GetPropertyAttribute<BrowsableAttribute>(info, item.Key);
					if(br == null || br.Browsable) {
						result.Add(XafTypesInfo.Instance.FindTypeInfo(item.Value));
					}
				}
			}
			CheckResult(parentIntem, result);
			return result;
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			return ModelApplicationCreator.GetDefaultXmlName(((ITypeInfo)nodeObject).Type);
		}
		public override bool HasChildren(object nodeObject) {
			return GetChildren(nodeObject).GetEnumerator().MoveNext();
		}
		public override object GetParent(object nodeObject) {
			return null;
		}
		public override string DisplayPropertyName {
			get { return "Name"; }
		}
		internal Dictionary<string, bool> CollectSelectedNodePath(ObjectTreeList schemaTreeList) {
			Dictionary<string, bool> selectedNodePaths = new Dictionary<string, bool>();
			TreeListNode rootNode = schemaTreeList.GetNodeByVisibleIndex(0);
			if(rootNode != null) {
				string rootTypeName = ((ITypeInfo)((ObjectTreeListNode)rootNode).Object).Name;
				foreach(TreeListNode node in rootNode.Nodes) {
					string typeName = rootTypeName + "." + ((ITypeInfo)((ObjectTreeListNode)node).Object).Name;
					selectedNodePaths.Add(typeName, node.Checked);
					CollectSelectedNodePath(selectedNodePaths, node, typeName);
				}
			}
			return selectedNodePaths;
		}
		private void CollectSelectedNodePath(Dictionary<string, bool> selectedNodePaths, TreeListNode startNode, string prefix) {
			foreach(TreeListNode node in startNode.Nodes) {
				string typeName = prefix + "." + ((ITypeInfo)((ObjectTreeListNode)node).Object).Name;
				if(!selectedNodePaths.ContainsKey(typeName)) {
					selectedNodePaths.Add(typeName, node.Checked);
				}
				CollectSelectedNodePath(selectedNodePaths, node, typeName);
			}
		}
		#region B157784 stackoverflow guard for build child nodes recursive
		private Dictionary<Type, Item> collectItems = new Dictionary<Type, Item>();
		private Item root = null;
		private Item GetParentItem(ITypeInfo info) {
			if(root == null) {
				root = new Item(info.Type);
				collectItems.Add(info.Type, root);
			}
			if(collectItems.Count == 0) {
				collectItems.Add(root.ItemType, root);
			}
			Item parentIntem = GetParentItem(info.Type);
			return parentIntem;
		}
		private void CheckResult(Item parentIntem, List<ITypeInfo> result) {
			if(result.Count > 0) {
				foreach(ITypeInfo typeInfo in result) {
					Item x;
					if(!collectItems.TryGetValue(typeInfo.Type, out x)) {
						x = new Item(typeInfo.Type);
						collectItems.Add(typeInfo.Type, x);
					}
					parentIntem.AddItem(x);
				}
			}
		}
		private Item GetParentItem(Type type) {
			Item targetItem;
			collectItems.TryGetValue(type, out targetItem);
			return targetItem;
		}
		private bool CanReturnType(Type type, Item parentIntem) {
			List<Item> result = new List<Item>();
			parentIntem.FindParentItems(type, result);
			if(result.Count > 0) {
				return false;
			}
			return true;
		}
		private class Item : IDisposable {
			private Type itemType;
			private List<Item> parents = new List<Item>();
			private List<Item> items = new List<Item>();
			public Item(Type itemType) {
				this.itemType = itemType;
			}
			public void FindParentItems(Type type, List<Item> result) {
				if(itemType == type) {
					if(!result.Contains(this)) {
						result.Add(this);
					}
				}
				foreach(Item item in parents) {
					if(!result.Contains(item)) {
						items.Add(item);
						item.FindParentItems(type, result);
					}
				}
			}
			public Type ItemType {
				get { return itemType; }
			}
			public void AddItem(Item item) {
				if(!items.Contains(item)) {
					item.parents.Add(this);
					items.Add(item);
				}
			}
			public override string ToString() {
				if(itemType != null) {
					return itemType.Name;
				}
				return base.ToString();
			}
			#region IDisposable Members
			public void Dispose() {
				itemType = null;
				if(parents != null) {
					parents.Clear();
					parents = null;
				}
				if(items != null) {
					items.Clear();
					items = null;
				}
			}
			#endregion
		}
		#endregion
		public void Reset() {
			ClearFields();
			collectItems = new Dictionary<Type, Item>();
		}
		#region IDisposable Members
		public virtual void Dispose() {
			modelApplication = null;
			ClearFields();
		}
		private void ClearFields() {
			if(collectItems != null) {
				foreach(KeyValuePair<Type, Item> _item in collectItems) {
					_item.Value.Dispose();
				}
				collectItems.Clear();
				collectItems = null;
			}
			if(root != null) {
				root.Dispose();
				root = null;
			}
		}
		#endregion
	}
}
