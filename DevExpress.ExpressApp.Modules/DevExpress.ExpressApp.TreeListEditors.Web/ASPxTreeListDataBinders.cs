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
using System.Text;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Controls;
using System.Collections;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public abstract class ASPxTreeListDataBinderBase : IDisposable {
		private IList dataSource;
		private ASPxTreeList treeList;
		private object rootValue;
		private bool holdRootValue;
		private void SubscribeToTreeListEvents() {
			TreeList.ProcessDragNode += new TreeListNodeDragEventHandler(TreeList_ProcessDragNode);
		}
		private void UnsubscribeFromTreeListEvents() {
			TreeList.ProcessDragNode -= new TreeListNodeDragEventHandler(TreeList_ProcessDragNode);
		}
		private void TreeList_ProcessDragNode(object sender, TreeListNodeDragEventArgs e) {
			OnProcessDragNode(e);
		}
		protected virtual void OnProcessDragNode(TreeListNodeDragEventArgs e) {
		}
		protected virtual void OnDataSourceChanging() {
		}
		protected virtual void OnDataSourceChanged() {
			if(DataSource != null) {
				DataBind();
			}
		}
		protected virtual void OnDataBinding() {
			if(DataBinding != null) {
				DataBinding(this, EventArgs.Empty);
			}
		}
		protected virtual void OnDataBound() {
			if(DataBound != null) {
				DataBound(this, EventArgs.Empty);
			}
		}
		protected virtual void OnRootValueChanged() {
		}
		protected virtual void OnRootValueChanging() {
		}
		protected virtual void OnHoldRootValueChanged() {
		}
		protected virtual void OnHoldRootValueChanging() {		
		}
		protected abstract void DataBindCore();
		public ASPxTreeListDataBinderBase(ASPxTreeList treeList) {
			Guard.ArgumentNotNull(treeList, "treeList");
			this.treeList = treeList;
			SubscribeToTreeListEvents();
		}
		public void DataBind() {
			if(TreeList.Page != null) {
				OnDataBinding();
				DataBindCore();
				OnDataBound();
			}
		}
		public IList DataSource {
			get { return dataSource; }
			set {
				OnDataSourceChanging();
				dataSource = value;
				OnDataSourceChanged();
			}
		}
		public ASPxTreeList TreeList {
			get { return treeList; }
		}
		public object RootValue {
			get { return rootValue; }
			set {
				if(rootValue != value) {					
					OnRootValueChanging();
					rootValue = value;
					OnRootValueChanged();
				}
			}
		}
		public bool HoldRootValue {
			get { return holdRootValue; }
			set {
				if(holdRootValue != value) {
					OnHoldRootValueChanging();
					holdRootValue = value;
					OnHoldRootValueChanged();
				}
			}
		}
		public event EventHandler<EventArgs> DataBinding;
		public event EventHandler<EventArgs> DataBound;
		#region IDisposable Members
		public virtual void Dispose() {
			if(treeList != null) {
				UnsubscribeFromTreeListEvents();
			}
			treeList = null;
			dataSource = null;
		}
		#endregion
	}
	public abstract class ASPxTreeListDataBinderUnboundBase : ASPxTreeListDataBinderBase {
		private NodeObjectAdapter adapter;
		private ITypeInfo typeInfo;
		private IEnumerable<string> displayableProperties;
		private void bindingList_ListChanged(object sender, ListChangedEventArgs e) {
			DataBind();
		}
		private void SubscribeToDataSource() {
			if(DataSource != null && DataSource is IBindingList) {
				IBindingList bindingList = (IBindingList)DataSource;
				bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
			}
		}
		private void UnsubscribeFromDataSource() {
			if(DataSource != null && DataSource is IBindingList) {
				IBindingList bindingList = (IBindingList)DataSource;
				bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
			}
		}
		protected virtual object GetObjectKey(object obj) {
			if(typeInfo.IsPersistent) {
				return Adapter.ObjectSpace.GetKeyValueAsString(obj);
			}
			else {
				return DataSource.IndexOf(obj);
			}
		}
		protected static object GetNodeObject(TreeListNode node) {
			return node[ASPxTreeListEditor.RowObjectColumnName];
		}
		protected IList GetRootObjects() {
			ArrayList list = new ArrayList();
			if(DataSource != null) {
				foreach(object obj in DataSource) {
					if(Adapter.IsRoot(obj)) {
						list.Add(obj);
					}
				}
			}
			return list;
		}
		protected virtual void AssignObjectToNode(object nodeObject, TreeListNode node) {
			foreach(string propertyName in DisplayableProperties) {
				IMemberInfo info = TypeInfo.FindMember(propertyName);
				node[propertyName] = info.GetValue(nodeObject);
				node.AllowSelect = true;
			}
			node[ASPxTreeListEditor.RowObjectColumnName] = nodeObject;
		}
		protected override void OnDataSourceChanging() {
			base.OnDataSourceChanging();
			UnsubscribeFromDataSource();
		}
		protected override void OnDataSourceChanged() {
			SubscribeToDataSource();
			if(adapter is TreeNodeInterfaceAdapter) {
				((TreeNodeInterfaceAdapter)adapter).Collection = DataSource;
			}
			base.OnDataSourceChanged();
		}
		protected override void OnRootValueChanged() {
			adapter.RootValue = RootValue;
			base.OnRootValueChanged();
		}
		protected override void OnHoldRootValueChanged() {
			if(adapter is TreeNodeInterfaceAdapter) {
				((TreeNodeInterfaceAdapter)adapter).HoldRootValue = HoldRootValue;
			}
			base.OnHoldRootValueChanged();
		}
		protected override void OnProcessDragNode(TreeListNodeDragEventArgs e) {
			ISupportNodeDragDrop supportNodeDragDrop = adapter as ISupportNodeDragDrop;
			if(supportNodeDragDrop != null) {
				supportNodeDragDrop.SetParent(GetNodeObject(e.Node), GetNodeObject(e.NewParentNode));
				e.Handled = true;
			}
		}
		public override void Dispose() {
			UnsubscribeFromDataSource();
			typeInfo = null;
			adapter = null;
			displayableProperties = null;
			base.Dispose();
		}
		public ASPxTreeListDataBinderUnboundBase(ASPxTreeList treeList, NodeObjectAdapter adapter, ITypeInfo typeInfo, IEnumerable<string> displayableProperties)
			: base(treeList) {
			Guard.ArgumentNotNull(adapter, "adapter");
			Guard.ArgumentNotNull(typeInfo, "typeInfo");
			Guard.ArgumentNotNull(displayableProperties, "displayableProperties");
			this.adapter = adapter;
			this.typeInfo = typeInfo;
			this.displayableProperties = displayableProperties;
		}
		public NodeObjectAdapter Adapter {
			get { return adapter; }
		}
		public IObjectSpace ObjectSpace {
			get { return adapter.ObjectSpace; }
			set { adapter.ObjectSpace = value; }
		}
		public ITypeInfo TypeInfo {
			get { return typeInfo; }
		}
		public IEnumerable<string> DisplayableProperties {
			get { return displayableProperties; }
		}
	}
	public class ASPxTreeListDataBinderUnboundMode : ASPxTreeListDataBinderUnboundBase {
		protected virtual TreeListNode BuildNode(object nodeObject, TreeListNode parentNode) {
			TreeListNode node = TreeList.AppendNode(GetObjectKey(nodeObject), parentNode);
			AssignObjectToNode(nodeObject, node);
			OnNodeBuilt(node);
			return node;
		}
		protected override void DataBindCore() {
			TreeList.ClearNodes();
			foreach(object rootObject in GetRootObjects()) {
				BuildNode(rootObject, null);
			}
		}
		protected virtual void OnNodeBuilt(TreeListNode node) {
			foreach(object child in Adapter.GetChildren(GetNodeObject(node))) {
				BuildNode(child, node);
			}
		}
		public ASPxTreeListDataBinderUnboundMode(ASPxTreeList treeList, NodeObjectAdapter adapter, ITypeInfo typeInfo, IEnumerable<string> displayableProperties)
			: base(treeList, adapter, typeInfo, displayableProperties) {
		}
	}
	public class ASPxTreeListDataBinderVirtualMode : ASPxTreeListDataBinderUnboundBase {
		protected virtual void treeList_VirtualModeCreateChildren(object sender, TreeListVirtualModeCreateChildrenEventArgs e) {
			ArrayList list = new ArrayList();
			IEnumerable childrent = e.NodeObject == null ? GetRootObjects() : Adapter.GetChildren(e.NodeObject);
			foreach(object obj in childrent) {
				list.Add(obj);
			}
			e.Children = list;
		}
		protected virtual void treeList_VirtualModeNodeCreating(object sender, TreeListVirtualModeNodeCreatingEventArgs e) {
			e.NodeKeyValue = GetObjectKey(e.NodeObject);
			e.IsLeaf = !Adapter.HasChildren(e.NodeObject);
		}
		protected virtual void treeList_VirtualModeNodeCreated(object sender, TreeListVirtualNodeEventArgs e) {
			AssignObjectToNode(e.NodeObject, e.Node);
		}
		protected override void DataBindCore() {
			TreeList.RefreshVirtualTree();
		}
		public ASPxTreeListDataBinderVirtualMode(ASPxTreeList treeList, NodeObjectAdapter adapter, ITypeInfo typeInfo, IEnumerable<string> displayableProperties)
			: base(treeList, adapter, typeInfo, displayableProperties) {
			TreeList.VirtualModeCreateChildren += new TreeListVirtualModeCreateChildrenEventHandler(treeList_VirtualModeCreateChildren);
			TreeList.VirtualModeNodeCreating += new TreeListVirtualModeNodeCreatingEventHandler(treeList_VirtualModeNodeCreating);
			TreeList.VirtualModeNodeCreated += new TreeListVirtualNodeEventHandler(treeList_VirtualModeNodeCreated);
		}
		public override void Dispose() {
			TreeList.VirtualModeCreateChildren -= new TreeListVirtualModeCreateChildrenEventHandler(treeList_VirtualModeCreateChildren);
			TreeList.VirtualModeNodeCreating -= new TreeListVirtualModeNodeCreatingEventHandler(treeList_VirtualModeNodeCreating);
			TreeList.VirtualModeNodeCreated -= new TreeListVirtualNodeEventHandler(treeList_VirtualModeNodeCreated);
			base.Dispose();
		}
	}
}
