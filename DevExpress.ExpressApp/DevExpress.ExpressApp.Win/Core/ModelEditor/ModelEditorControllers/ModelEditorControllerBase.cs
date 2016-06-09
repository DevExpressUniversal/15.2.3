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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public abstract class ModelEditorControllerBase {
		public event CancelEventHandler Modifying;
		public event EventHandler<FileModelStoreCancelEventArgs> CanSaveBoFiles;
		private ModelApplicationBase modelApplication;
		private ModelNode notifyPropertyChangedObject = null;
		private ModelTreeListNode currentModelNode;
		private int lockUpdate = 0;
		private bool isModified;
		private bool readOnly = false;
		private bool designMode = false;
		private bool isStandalone = false;
		private static bool askConfirmation = true;
		private ModelEditorControl modelEditorControl;
		private ExtendModelInterfaceAdapter _adapter = null;
#pragma warning disable 414
		private bool disposing = false;
#pragma warning restore
		public ModelEditorControllerBase(IModelApplication modelApplication) {
			this.modelApplication = (ModelApplicationBase)modelApplication;
		}
		internal void ChangeNodeIndex(ModelTreeListNode node, int nodeIndex) {
			ModelTreeListNode targetNode = node;
			int index = 0;
			if(nodeIndex != -1) {
				bool targetNodeFinded = false;
				foreach(ModelTreeListNode item in Adapter.GetChildren(node.Parent)) {
					if(index == nodeIndex) {
						index++;
						targetNodeFinded = true;
					}
					if(item.ModelNode.Id != targetNode.ModelNode.Id && (item.ModelNode.Index > -1 || item.ModelNode.Index == null)) {
						if(item.ModelNode.Index != index) {
							if(item.ModelNode.Index == null && targetNodeFinded) {
								break;
							}
							item.ModelNode.Index = index;
						}
						index++;
					}
				}
			}
			targetNode.ModelNode.Index = nodeIndex;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected ExtendModelInterfaceAdapter Adapter {
			get {
				if(_adapter == null) {
					_adapter = new ExtendModelInterfaceAdapter();
					SubscribeAdapterEvents();
				}
				return _adapter;
			}
		}
		public IModelApplication ModelApplication {
			get {
				return (IModelApplication)modelApplication;
			}
		}
		public ModelEditorControl ModelEditorControl {
			get {
				return modelEditorControl;
			}
		}
		public bool TrySetModified() {
			if(!ReadOnly && !IsModified) {
				CancelEventArgs args = new CancelEventArgs(false);
				if(Modifying != null) {
					Modifying(this, args);
				}
				IsModified = !args.Cancel;
				if(IsModified) {
					UpdateActionState();
				}
			}
			return IsModified;
		}
		public bool TrySaveBoFiles(string[] boFiles) {
			bool result = true;
			if(CanSaveBoFiles != null) {
				FileModelStoreCancelEventArgs args = new FileModelStoreCancelEventArgs(boFiles, false);
				CanSaveBoFiles(this, args);
				result = !args.Cancel;
			}
			return result;
		}
		public bool IsModified {
			get { return isModified; }
			protected set {
				isModified = value;
			}
		}
		public bool ReadOnly {
			get { return readOnly; }
			set { readOnly = value; }
		}
		public bool DesignMode {
			get {
				return designMode;
			}
			set {
				designMode = value;
				UpdateActionState();
			}
		}
		public bool IsStandalone {
			get {
				return isStandalone;
			}
			set {
				isStandalone = value;
			}
		}
		public static bool AskConfirmation {
			get { return askConfirmation; }
			set { askConfirmation = value; }
		}
		public bool Disposing {
			get {
				return disposing;
			}
		}
		public virtual void Dispose() {
			disposing = true;
			UnSubscribeAdapterEvents();
			UnSubscribeEvents();
			DisposeAdapter();
			CurrentModelNode = null;
			modelEditorControl = null;
			notifyPropertyChangedObject = null;
			modelApplication = null;
#if DebugTest
			if(selectedNodesForTests != null) {
				selectedNodesForTests.Clear();
				selectedNodesForTests = null;
			}
#endif
		}
		public virtual void SetControl(ModelEditorControl _modelEditorControl) {
			modelEditorControl = _modelEditorControl;
			UnSubscribeAdapterEvents();
			_adapter = (ExtendModelInterfaceAdapter)ModelEditorControl.modelTreeList.Adapter;
			SubscribeAdapterEvents();
			SubscribeEvents();
		}
		public List<ModelTreeListNode> SelectedNodes {
			get {
				if(ModelEditorControl == null) {
#if DebugTest
					if(CurrentModelNode != null && !selectedNodesForTests.Contains(CurrentModelNode)) {
						selectedNodesForTests.Add(CurrentModelNode);
					}
					return selectedNodesForTests;
#else
					return new List<ModelTreeListNode>();
#endif
				}
				else {
					return ModelEditorControl.SelectedNodes;
				}
			}
		}
		public virtual void SelectNodes(List<TreeListNode> nodes) {
			if(ModelEditorControl != null) {
				ModelEditorControl.SelectNodes(nodes);
			}
		}
		protected void Unsubscribe(ModelNode notifyPropertyChangedObject) {
			if(notifyPropertyChangedObject != null) {
				ModelEditorHelper.RemovePropertyChangedHandler(notifyPropertyChangedObject, new PropertyChangedEventHandler(ModelNodePropertyChanged));
			}
		}
		public virtual ModelTreeListNode CurrentModelNode {
			get {
				if(ModelEditorControl == null) {
					return currentModelNode;
				}
				else {
					return ModelEditorControl.CurrentModelTreeListNode;
				}
			}
			set {
				Unsubscribe(notifyPropertyChangedObject);
				if(ModelEditorControl == null) {
					currentModelNode = value;
#if DebugTest
					ClearSelectedNodes_Tests();
#endif
				}
				else {
					ModelEditorControl.CurrentModelTreeListNode = value;
				}
				if(value != null && value.ModelNode != null) {
					notifyPropertyChangedObject = value.ModelNode;
					Unsubscribe(notifyPropertyChangedObject);
					ModelEditorHelper.AddPropertyChangedHandler(notifyPropertyChangedObject, new PropertyChangedEventHandler(ModelNodePropertyChanged));
				}
			}
		}
		protected virtual void UpdateActionState() { }
		protected int LockUpdate {
			get {
				return lockUpdate;
			}
		}
		protected ModelNode GetModelNode(ModelTreeListNode node) {
			return node != null ? node.ModelNode : null;
		}
		protected bool IsEditableNode(ModelTreeListNode node) {
			return !(node == null || (node.ModelTreeListNodeType == ModelTreeListNodeType.Links
				|| node.ModelTreeListNodeType == ModelTreeListNodeType.Group || (node.VirtualTreeNode && !node.IsRootVirtualTreeNode)));
		}
		protected void SafeExecute(System.Threading.ThreadStart method) {
			try {
				method.Invoke();
			}
			catch(Exception ex) {
				t_HandleException(this, new CustomHandleExceptionEventArgs(ex));
			}
		}
		protected EventHandler<T> SafeHandler<T>(EventHandler<T> method) where T : EventArgs {
			HandlerWrapper<T> t = new HandlerWrapper<T>(method);
			t.HandleException += new EventHandler<CustomHandleExceptionEventArgs>(t_HandleException);
			return t.Handler;
		}
		protected EventHandler SafeHandler(EventHandler method) {
			HandlerWrapper t = new HandlerWrapper(method);
			t.HandleException += new EventHandler<CustomHandleExceptionEventArgs>(t_HandleException);
			return t.Handler;
		}
		protected ModelTreeListNode GetModelTreeListNode(TreeListNode treeNode) {
			if(treeNode != null) {
				return (ModelTreeListNode)((ObjectTreeListNode)treeNode).Object;
			}
			return null;
		}
		protected void UpdatedTreeListCall(System.Threading.ThreadStart method) {
			bool isAssigned = modelEditorControl != null && modelEditorControl.modelTreeList != null;
			if(isAssigned) { modelEditorControl.modelTreeList.BeginUpdate(); }
			try {
				lockUpdate++;
				method.Invoke();
			}
			catch(Exception ex) {
				t_HandleException(this, new CustomHandleExceptionEventArgs(ex));
			}
			finally {
				lockUpdate--;
				if(isAssigned) {
					modelEditorControl.modelTreeList.EndUpdate();
				}
			}
		}
		public static void ShowError(Exception ex, string errorMessage) {
			string warningMessage = string.IsNullOrEmpty(errorMessage) ? "" :
				string.Format("{0}{1}{1}", errorMessage, Environment.NewLine);
			if(DesignerOnlyCalculator.IsRunFromDesigner) {
				string message = string.Format("{0}{2}{1}{3}", warningMessage, Environment.NewLine, ex.Message, ex.StackTrace);
				Messaging.GetMessaging(null).Show(new Exception(message));
			}
			else {
				string message = string.Format("{0}{2}{1}", warningMessage, Environment.NewLine, ex.Message);
				Messaging.GetMessaging(null).Show(new Exception(message));
			}
		}
		public static void ShowError(Exception ex) {
			ShowError(ex, "");
		}
		protected void RefreshCurrentModelNode() {
			ModelTreeListNode currentNode = CurrentModelNode;
			if(!currentNode.IsDisposed) {
				if(ModelEditorControl != null) {
					ModelEditorControl.modelTreeList.RefreshObject(currentNode);
				}
				CurrentModelNode = currentNode;
			}
			else {
				CurrentModelNode = null;
			}
		}
		protected ModelTreeListNode FindLinksModelNode(string targetNodePath, string ownerNodePath) {
			ModelTreeListNode result = null;
			ModelNode targetModelNode = ModelEditorHelper.FindNodeByPath(targetNodePath, modelApplication, false, false);
			ModelTreeListNode targetModelTreeListNode = Adapter.FindPrimaryNode(targetModelNode);
			if(targetModelTreeListNode.LinksCount == 0) {
				ModelNode ownerModelNode = ModelEditorHelper.FindNodeByPath(ownerNodePath, modelApplication, false, false);
				ModelTreeListNode ownerModelTreeListNode = Adapter.FindPrimaryNode(ownerModelNode);
				FillModelNodeLinks(ownerModelTreeListNode);
			}
			foreach(ModelTreeListNode linkNode in targetModelTreeListNode.Links) {
				if(ModelEditorHelper.GetModelNodePath(linkNode.Owner.ModelNode) == ownerNodePath) {
					result = linkNode;
					break;
				}
			}
			return result;
		}
		protected void FillModelNodeLinks(ModelTreeListNode modelNode) {
			if(modelNode.LinksCount == 0) {
				Queue<ModelTreeListNode> modelNodesQueue = new Queue<ModelTreeListNode>();
				modelNodesQueue.Enqueue(modelNode);
				while(modelNodesQueue.Count > 0) {
					ModelTreeListNode currentModelNode = modelNodesQueue.Dequeue();
					foreach(ModelTreeListNode childModelNode in Adapter.GetChildren(currentModelNode)) {
						modelNodesQueue.Enqueue(childModelNode);
					}
				}
			}
		}
		protected ModelApplicationBase ModelApplicationCore {
			get {
				return modelApplication;
			}
		}
		protected void SetModelApplication(ModelApplicationBase modelApplication) {
			this.modelApplication = modelApplication;
		}
		protected virtual void SubscribeEvents() {
			UnSubscribeEvents();
			ModelEditorControl.OnDisposing += new EventHandler(ModelEditorControl_Disposing);
		}
		protected virtual void UnSubscribeEvents() {
			if(ModelEditorControl != null) {
				ModelEditorControl.OnDisposing -= new EventHandler(ModelEditorControl_Disposing);
			}
		}
		protected virtual void ModelNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
		protected virtual void LinksCollecting() { }
		protected virtual void LinksCollected() { }
		private void DisposeAdapter() {
			if(_adapter != null) {
				UnSubscribeAdapterEvents();
				_adapter.Dispose();
				_adapter = null;
			}
		}
		private void SubscribeAdapterEvents() {
			_adapter.LinksCollected += new EventHandler(_adapter_LinksCollected);
			_adapter.LinksCollecting += new EventHandler(_adapter_LinksCollecting);
		}
		private void UnSubscribeAdapterEvents() {
			if(_adapter != null) {
				_adapter.LinksCollecting -= new EventHandler(_adapter_LinksCollecting);
				_adapter.LinksCollected -= new EventHandler(_adapter_LinksCollected);
			}
		}
		private void _adapter_LinksCollecting(object sender, EventArgs e) {
			LinksCollecting();
		}
		private void _adapter_LinksCollected(object sender, EventArgs e) {
			LinksCollected();
		}
		private void t_HandleException(object sender, CustomHandleExceptionEventArgs e) {
#if DebugTest
			if(DebugTest_HandleException) {
#endif
			ShowError(e.Exception);
			try {
				CurrentModelNode = null;
			}
			catch { }
#if DebugTest
			}
			else {
				throw e.Exception;
			}
#endif
		}
		private void ModelEditorControl_Disposing(object sender, EventArgs e) {
			Dispose();
		}
#if DebugTest
		public static bool DebugTest_HandleException = true;
		public bool clearSelection = true;
		protected List<ModelTreeListNode> selectedNodesForTests = new List<ModelTreeListNode>();
		public void AddSelectedNode_Tests(ModelTreeListNode node) {
			selectedNodesForTests.Add(node);
			UpdateActionState();
		}
		public void ClearSelectedNodes_Tests() {
			if(clearSelection) {
				selectedNodesForTests = new List<ModelTreeListNode>();
				UpdateActionState();
			}
		}
		public void ModelEditorClosing() {
			Dispose();
		}
		public ExtendModelInterfaceAdapter DebugTest_Adapter {
			get { return Adapter; }
		}
#endif
	}
	public class FileModelStoreCancelEventArgs : CancelEventArgs {
		string[] boFiles;
		public FileModelStoreCancelEventArgs(string[] boFiles, bool cancel)
			: base(cancel) {
			this.boFiles = boFiles;
		}
		public string[] BoFiles {
			get {
				return boFiles;
			}
		}
	}
}
