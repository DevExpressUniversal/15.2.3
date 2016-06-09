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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.ExpressApp.Controls;
namespace DevExpress.ExpressApp.TreeListEditors {
	public class TreeNodeController : ViewController {
		public const string EnableNewItemKey = "It's possible to add this object into the Children collection";
		public static Type GetChildrenElementType(ITreeNode parent) {
			Type result = null;
			if(parent != null) {
				Type childrenCollectionType = parent.Children.GetType();
				Type childrenTypedIEnumerable = null;
				foreach(Type implemented in childrenCollectionType.GetInterfaces()) {
					if(implemented.IsGenericType && implemented.GetGenericTypeDefinition() == typeof(IEnumerable<>).GetGenericTypeDefinition()) {
						childrenTypedIEnumerable = implemented;
						break;
					}
				}
				if(childrenTypedIEnumerable != null) {
					result = childrenTypedIEnumerable.GetGenericArguments()[0];
				}
			}
			return result;
		}
		private ITreeNode GetSelectedNode(IObjectSpace objectSpace) {
			if(View.CurrentObject != null) {
				ITreeNode parent = null;
				if(View is DetailView)
					parent = (ITreeNode)objectSpace.GetObject(((ITreeNode)View.CurrentObject).Parent);
				else
					if(View is ListView)
						parent = (ITreeNode)objectSpace.GetObject(View.CurrentObject);
				return parent;
			}
			return null;
		}
		private void MoveNode(ITreeNode node, ITreeNode parentNode) {
			Guard.ArgumentNotNull(node, "node");
			Guard.ArgumentNotNull(parentNode, "parentNode");
			if(node == parentNode) {
				throw new ArgumentException("Cannot move node to itself");
			}
			Type childrenType = GetChildrenElementType(parentNode);
			if(childrenType != null) {
				if(childrenType.IsAssignableFrom(node.GetType())) {
					parentNode.Children.Add(node);
				}
			}
			else {
				parentNode.Children.Add(node);
			}
		}
		private void NewObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
		}
		private void NewObjectViewController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
			if(e.CreatedObject is ITreeNode) {
				ITreeNode newObject = (ITreeNode)e.CreatedObject;
				IObjectSpace objectSpace = e.ObjectSpace;
				ProcessNewTreeNodeEventArgs args = new ProcessNewTreeNodeEventArgs(newObject, objectSpace);
				OnProcessNewTreeNode(args);
				if(!args.Handled) {
					ITreeNode parent = GetSelectedNode(objectSpace);
					if(parent != null && parent != newObject) {
						MoveNode((ITreeNode)newObject, parent);
						SetLinkNewObjectToParentImmediately();
					}
				}
			}
		}
		private void SetLinkNewObjectToParentImmediately() {
			LinkToListViewController controller = Frame.GetController<LinkToListViewController>();
			if(controller != null && controller.Link != null && controller.Link.PropertyCollectionSourceLink != null) {
				controller.Link.PropertyCollectionSourceLink.LinkNewObjectToParentImmediately = true;
			}
		}
		private void UpdateNewActionItemsCore() {
			NewObjectViewController.NewObjectAction.BeginUpdate();
			try {
				foreach(ChoiceActionItem item in NewObjectViewController.NewObjectAction.Items) {
					item.Active.RemoveItem(EnableNewItemKey);
				}
				UpdateNewActionItemsEventArgs args = new UpdateNewActionItemsEventArgs(NewObjectViewController.NewObjectAction.Items);
				OnUpdateNewActionItems(args);
				if(!args.Handled) {
					Type childrentBaseType = GetChildrenElementType(((ListView)View).CurrentObject as ITreeNode);
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(childrentBaseType);
					if(typeInfo != null) {
						foreach(ChoiceActionItem item in NewObjectViewController.NewObjectAction.Items) {
							item.Active.SetItemValue(EnableNewItemKey, childrentBaseType.IsAssignableFrom((Type)item.Data) && typeInfo.IsDomainComponent);
						}
					}
				}
			}
			finally {
				NewObjectViewController.NewObjectAction.EndUpdate();
			}
		}
		private void listView_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateNewActionItemsCore();
		}
		private void listView_EditorChanged(object sender, EventArgs e) {
			Active.SetItemValue("TreeListEditor is used", ((ListView)sender).Editor is INodeObjectAdapterProvider);
		}
		private NewObjectViewController NewObjectViewController {
			get {
				return Frame.GetController<NewObjectViewController>();
			}
		}
		protected virtual void OnUpdateNewActionItems(UpdateNewActionItemsEventArgs args) {
			if(UpdateNewActionItems != null) {
				UpdateNewActionItems(this, args);
			}
		}
		protected virtual void OnProcessNewTreeNode(ProcessNewTreeNodeEventArgs args) {
			if(ProcessNewTreeNode != null) {
				ProcessNewTreeNode(this, args);
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			Active.SetItemValue("ITreeNode", (view is ObjectView) && typeof(ITreeNode).IsAssignableFrom(((ObjectView)view).ObjectTypeInfo.Type));
			if(View is ListView) {
				((ListView)View).EditorChanged -= new EventHandler(listView_EditorChanged);
			}
			if(view is ListView) {
				((ListView)view).EditorChanged += new EventHandler(listView_EditorChanged);
				Active.SetItemValue("TreeListEditor is used", ((ListView)view).Editor is INodeObjectAdapterProvider);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListView listView = View as ListView;
			if(listView != null) {
				UpdateNewActionItemsCore();
				listView.CurrentObjectChanged += new EventHandler(listView_CurrentObjectChanged);
			}
			NewObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
			NewObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(View is ListView) {
					((ListView)View).EditorChanged -= new EventHandler(listView_EditorChanged);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ListView listView = View as ListView;
			if(listView != null) {
				listView.CurrentObjectChanged -= new EventHandler(listView_CurrentObjectChanged);
			}
			NewObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
			NewObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
		}
		public TreeNodeController() : base() { }
		public event EventHandler<UpdateNewActionItemsEventArgs> UpdateNewActionItems;
		public event EventHandler<ProcessNewTreeNodeEventArgs> ProcessNewTreeNode;
	}
	public class UpdateNewActionItemsEventArgs : HandledEventArgs {
		private ChoiceActionItemCollection items;
		public UpdateNewActionItemsEventArgs(ChoiceActionItemCollection items)
			: base() {
			this.items = items;
		}
		public ChoiceActionItemCollection Items {
			get { return items; }
		}
	}
	public class ProcessNewTreeNodeEventArgs : HandledEventArgs {
		private ITreeNode newTreeNode;
		private IObjectSpace objectSpace;
		public ProcessNewTreeNodeEventArgs(ITreeNode node, IObjectSpace objectSpace)
			: base() {
			this.newTreeNode = node;
			this.objectSpace = objectSpace;
		}
		public ITreeNode NewTreeNode {
			get { return newTreeNode; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
	}
}
