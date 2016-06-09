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
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebModificationsController : ModificationsController {
		private SimpleAction editAction;
		private bool isTurnedToEditInternally;
		private bool isObjectArrivedAsNew;
		private void editAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			ExecuteEdit(args);
		}
		private void View_ControlsCreated(Object sender, EventArgs e) {
			foreach(ListPropertyEditor listPropertyEditor in DetailView.GetItems<ListPropertyEditor>()) {
				Control control = listPropertyEditor.Control as Control;
				if(control != null) {
					control.Visible = ShowCollectionsInEditMode || (DetailView.ViewEditMode == ViewEditMode.View);
				}
			}
		}
		private void View_QueryCanChangeCurrentObject(Object sender, CancelEventArgs e) {
			OnViewQueryCanChangeCurrentObject(e);
		}
		private void DetailView_ViewEditModeChanged(Object sender, EventArgs e) {
			UpdateListPropertyEditors();
		}
		private void ObjectSpace_Reloaded(Object sender, EventArgs e) {
			SetObjectSpaceModified();
		}
		private void RecreateDetailViewControls() {
			DetailView.BreakLinksToControls();
			DetailView.CreateControls();
		}
		private void OnQueryCloseAfterSave(QueryCloseAfterSaveEventArgs args) {
			if(QueryCloseAfterSave != null) {
				QueryCloseAfterSave(this, args);
			}
		}
		protected virtual bool CalculateDefaultCloseAfterSave() {
			if(IsObjectArrivedAsNew && !ShowCollectionsInEditMode) {
				return false;
			}
			return true;
		}
		protected virtual void OnViewQueryCanChangeCurrentObject(CancelEventArgs e) {
			if(View.IsRoot && ObjectSpace.IsModified) {
				ObjectSpace.Rollback();
			}
		}
		protected void UpdateListPropertyEditors() {
			foreach(ListPropertyEditor listPropertyEditor in DetailView.GetItems<ListPropertyEditor>()) {
				if(ShowCollectionsInEditMode) {
					listPropertyEditor.AllowEdit.SetItemValue("ShowCollectionsInEditMode", DetailView.ViewEditMode == ViewEditMode.Edit);
				}
				else {
					listPropertyEditor.AllowEdit.SetItemValue("ShowCollectionsInEditMode", DetailView.ViewEditMode == ViewEditMode.View);
				}
			}
		}
		protected virtual void SetObjectSpaceModified() {
			if(DetailView.ViewEditMode == ViewEditMode.Edit) {
				ObjectSpace.SetModified(null);
			}
		}
		protected virtual void ExecuteEdit(SimpleActionExecuteEventArgs args) {
			IsTurnedToEditInternally = true;
			ObjectSpace.Refresh();
			ObjectSpace.SetModified(View.CurrentObject);
			DetailView.ViewEditMode = ViewEditMode.Edit;
			RecreateDetailViewControls();
		}
		protected override void UpdateActionState() {
			base.UpdateActionState();
			String editModeKey = "EditMode";
			String emptyCurrentObjectKey = "Empty current object";
			String readOnlyKey = "AllowEdit";
			String isRootKey = "IsRoot";
			String detailViewKey = "DetailView";
			Boolean isDetailView = (DetailView != null);
			SaveAction.Active[detailViewKey] = isDetailView;
			SaveAndCloseAction.Active[detailViewKey] = isDetailView;
			CancelAction.Active[detailViewKey] = isDetailView;
			editAction.Active[detailViewKey] = isDetailView;
			if(isDetailView) {
				SaveAction.Active[editModeKey] = (DetailView.ViewEditMode == ViewEditMode.Edit);
				SaveAction.Active[emptyCurrentObjectKey] = (View.CurrentObject != null);
				SaveAndCloseAction.Active[editModeKey] = (DetailView.ViewEditMode == ViewEditMode.Edit);
				SaveAndCloseAction.Active[emptyCurrentObjectKey] = (View.CurrentObject != null);
				CancelAction.Active[editModeKey] = (DetailView.ViewEditMode == ViewEditMode.Edit);
				CancelAction.Active[emptyCurrentObjectKey] = (View.CurrentObject != null);
				editAction.Active[editModeKey] = (DetailView.ViewEditMode == ViewEditMode.View);
				editAction.Active[emptyCurrentObjectKey] = (View.CurrentObject != null);
				editAction.Active[readOnlyKey] = View.AllowEdit;
				editAction.Active[isRootKey] = View.IsRoot;
				String isGrantedToEditKey = "Is granted to edit";
				Boolean isEditingGranted = DataManipulationRight.CanEdit(View, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame));
				editAction.Active[isGrantedToEditKey] = isEditingGranted;
			}
		}
		protected override void Save(SimpleActionExecuteEventArgs args) {
			View.ObjectSpace.CommitChanges();
			SetObjectSpaceModified();
		}
		protected override void SaveAndClose(SimpleActionExecuteEventArgs args) {
			View.ObjectSpace.CommitChanges();
			if(!View.ObjectSpace.IsModified) {
				QueryCloseAfterSaveEventArgs closeAfterSaveEventArgs = new QueryCloseAfterSaveEventArgs();
				closeAfterSaveEventArgs.CloseAfterSave = CalculateDefaultCloseAfterSave();
				OnQueryCloseAfterSave(closeAfterSaveEventArgs);
				if(closeAfterSaveEventArgs.CloseAfterSave) {
					View.Close();
				}
				else {
					DetailView.ViewEditMode = ViewEditMode.View;
					RecreateDetailViewControls();
				}
			}
		}
		protected override void Cancel(SimpleActionExecuteEventArgs args) {
			bool isCurrentObjectNew = ObjectSpace.IsNewObject(View.CurrentObject);
			if(!IsTurnedToEditInternally || isCurrentObjectNew) {
				View.Close();
			}
			else {
				DetailView.ViewEditMode = ViewEditMode.View;
				ObjectSpace.Rollback();
				RecreateDetailViewControls();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(DetailView != null) {
				DetailView.ViewEditModeChanged += new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
				UpdateActionState();
				UpdateListPropertyEditors();
				Boolean objectPresented = View.CurrentObject != null;
				if(objectPresented) {
					IsTurnedToEditInternally = false;
					IsObjectArrivedAsNew = ObjectSpace.IsNewObject(View.CurrentObject);
					if(ObjectSpace.IsModified || IsObjectArrivedAsNew) {
						DetailView.ViewEditMode = ViewEditMode.Edit;
					}
				}
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
				View.QueryCanChangeCurrentObject += new EventHandler<CancelEventArgs>(View_QueryCanChangeCurrentObject);
				ObjectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
			}
			modificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
		}
		protected override void OnDeactivated() {
			if(DetailView != null) {
				View.ControlsCreated -= new EventHandler(View_ControlsCreated);
				DetailView.ViewEditModeChanged -= new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
				View.QueryCanChangeCurrentObject -= new EventHandler<CancelEventArgs>(View_QueryCanChangeCurrentObject);
				ObjectSpace.Reloaded -= new EventHandler(ObjectSpace_Reloaded);
			}
			base.OnDeactivated();
		}
		protected override void ObjectSpaceModifiedChanged() {
			if (!ObjectSpace.IsReloading) {
				base.ObjectSpaceModifiedChanged();
			}
		}
		protected bool IsTurnedToEditInternally {
			get { return isTurnedToEditInternally; }
			set { isTurnedToEditInternally = value; }
		}
		protected bool IsObjectArrivedAsNew {
			get { return isObjectArrivedAsNew; }
			set { isObjectArrivedAsNew = value; }
		}
		protected bool ShowCollectionsInEditMode {
			get {
				ViewEditMode? collectionsEditMode = ShowViewStrategy.GetCollectionsEditMode(Frame, Application);
				return collectionsEditMode.HasValue && collectionsEditMode.Value == ViewEditMode.Edit;
			}
		}
		public WebModificationsController() {
			editAction = new SimpleAction(this, "SwitchToEditMode", PredefinedCategory.Edit);
			editAction.Caption = "Edit";
			editAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			editAction.ImageName = "MenuBar_Edit";
			editAction.Execute += new SimpleActionExecuteEventHandler(editAction_OnExecute);
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebModificationsControllerEditAction")]
#endif
		public SimpleAction EditAction {
			get { return editAction; }
		}
		public event EventHandler<QueryCloseAfterSaveEventArgs> QueryCloseAfterSave;
	}
	public class QueryCloseAfterSaveEventArgs : EventArgs {
		private bool closeAfterSave;
		public bool CloseAfterSave {
			get { return closeAfterSave; }
			set { closeAfterSave = value; }
		}
	}
}
