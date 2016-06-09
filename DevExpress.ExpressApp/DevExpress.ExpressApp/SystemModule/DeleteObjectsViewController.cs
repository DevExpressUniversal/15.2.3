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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.SystemModule {
	public class DeleteObjectsViewController : ViewController {
		private SimpleAction deleteAction;
		private bool autoCommit;
		private IContainer components;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.deleteAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.deleteAction.Caption = "Delete";
			this.deleteAction.Category = "Edit";
			this.deleteAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
			this.deleteAction.Id = "Delete";
			this.deleteAction.ImageName = "MenuBar_Delete";
			this.deleteAction.Shortcut = "CtrlD";
			this.deleteAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.deleteAction_OnExecute);
			this.deleteAction.ConfirmationMessage = "You are about to delete the selected record(s). Do you want to proceed?";
		}
		private void DoUpdateActionState() {
			if(View != null) {
				deleteAction.BeginUpdate();
				try {
					UpdateActionState();
				}
				finally {
					deleteAction.EndUpdate();
				}
			}
		}
		private void deleteAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Delete(e);
		}
		private void CollectionSource_CollectionChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			if (!ObjectSpace.IsReloading) {
				DoUpdateActionState();
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void View_ReadOnlyChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void View_AllowDeleteChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListView listView = View as ListView;
			if(listView != null && listView.CollectionSource != null) {
				listView.CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionChanged);
			}
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			((ObjectView)View).CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			((ObjectView)View).SelectionChanged += new EventHandler(View_SelectionChanged);
			View.AllowDeleteChanged += new EventHandler(View_AllowDeleteChanged);
			DoUpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(View != null) {
				ListView listView = View as ListView;
				if(listView != null && listView.CollectionSource != null) {
					listView.CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionChanged);
				}
				View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
				((ObjectView)View).CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
				((ObjectView)View).SelectionChanged -= new EventHandler(View_SelectionChanged);
				View.AllowDeleteChanged -= new EventHandler(View_AllowDeleteChanged);
			}
		}
		protected virtual void UpdateActionState() {
			String diagnosticInfo;
			deleteAction.Active.SetItemValue("ListAllowDelete", DataManipulationRight.IsRemoveFromCollectionAllowed(View, out diagnosticInfo));
			deleteAction.DiagnosticInfo = "ListAllowDelete: " + diagnosticInfo;
			deleteAction.Active.SetItemValue("AllowDelete", View.AllowDelete);
			deleteAction.Active.SetItemValue("IsPersistent",
				((ObjectView)View).ObjectTypeInfo.IsPersistent || ObjectSpace.CanInstantiate(((ObjectView)View).ObjectTypeInfo.Type));
			if ((View is DetailView) && View.IsRoot) {
				deleteAction.Enabled.SetItemValue("IsCurrentObjectNew",
					!View.ObjectSpace.IsNewObject(((DetailView)View).CurrentObject) || (View.ObjectSpace is INestedObjectSpace));
			}
			else {
				deleteAction.Enabled.RemoveItem("IsCurrentObjectNew");
			}
			bool isDeleteGrantedForAllSelectedObjects = false;
			if(((ObjectView)View).SelectedObjects.Count == 1) {
				isDeleteGrantedForAllSelectedObjects = DataManipulationRight.CanDelete(((ObjectView)View).ObjectTypeInfo.Type, ((ObjectView)View).SelectedObjects[0], LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
			}
			else {
				isDeleteGrantedForAllSelectedObjects = DataManipulationRight.CanDelete(((ObjectView)View).ObjectTypeInfo.Type, null, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
				if(isDeleteGrantedForAllSelectedObjects) {
					foreach(object selectedObject in ((ObjectView)View).SelectedObjects) {
						if(!DataManipulationRight.CanDelete(((ObjectView)View).ObjectTypeInfo.Type, selectedObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace)) {
							isDeleteGrantedForAllSelectedObjects = false;
							break;
						}
					}
				}
			}
			deleteAction.Active.SetItemValue("Has permission to delete", isDeleteGrantedForAllSelectedObjects);
			if(View is DetailView) {
				deleteAction.Active.SetItemValue("RootDetailView", View.IsRoot);
			}
			else {
				deleteAction.Active.RemoveItem("RootDetailView");
			}
		}
		protected void OnDeleting(DeletingEventArgs args) {
			if(Deleting != null) {
				Deleting(this, args);
			}
		}
		protected virtual void Delete(SimpleActionExecuteEventArgs args) {
			DeletingEventArgs deletingEventArgs = new DeletingEventArgs(args.SelectedObjects);
			OnDeleting(deletingEventArgs);
			ListView listView = View as ListView;
			ISupportUpdate supportUpdate = null;
			if(listView != null) {
				supportUpdate = listView.Editor as ISupportUpdate;
			}
			try {
				if(supportUpdate != null) {
					supportUpdate.BeginUpdate();
				}
				ObjectSpace.Delete(deletingEventArgs.Objects);
				if(autoCommit || View.IsRoot) {
					ObjectSpace.CommitChanges();
				}
				if(listView != null) {
					if((autoCommit || View.IsRoot)
						&&
						(
							(listView.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.Server)
							||
							(listView.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)
						)
					) {
						listView.CollectionSource.Reload();
					}
					else {
						foreach(object obj in deletingEventArgs.Objects) {
							listView.CollectionSource.Remove(obj);
						}
					}
					listView.Editor.Refresh();
				}
			}
			finally {
				if(supportUpdate != null) {
					supportUpdate.EndUpdate();
				}
			}
			if(listView == null && deletingEventArgs.Objects.IndexOf(((ObjectView)View).CurrentObject) >= 0) {
				View.Close();
			}
		}
		public DeleteObjectsViewController()
			: base() {
			InitializeComponent();
			TypeOfView = typeof(ObjectView);
			RegisterActions(components);
			autoCommit = false;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DeleteObjectsViewControllerDeleteAction")]
#endif
		public SimpleAction DeleteAction {
			get { return deleteAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DeleteObjectsViewControllerAutoCommit")]
#endif
		public bool AutoCommit {
			get { return autoCommit; }
			set { autoCommit = value; }
		}
		public event EventHandler<DeletingEventArgs> Deleting;
	}
}
