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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp.SystemModule {
	public enum ModificationsHandlingMode { Confirmation, AutoCommit, AutoRollback }
	public enum ModificationsCheckingMode { Always, OnCloseOnly }
	public class ModificationsController : ObjectViewController {
		private SimpleAction saveAction;
		private SimpleAction saveAndCloseAction;
		private SimpleAction cancelAction;
		private SingleChoiceAction saveAndNewAction;
		private NewObjectViewController newObjectViewController;
		private Boolean isSaveAndNewExecuting;
		protected Boolean skipConfirmation;
		protected ModificationsHandlingMode modificationsHandlingMode;
		protected ModificationsCheckingMode modificationsCheckingMode;
		private void View_CurrentObjectChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(Object sender, EventArgs e) {
			ObjectSpaceModifiedChanged();
		}
		private void saveAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Save(e);
		}
		private void saveAndCloseAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			SaveAndClose(e);
		}
		private void saveAndNewAction_Execute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			SaveAndNew(e);
		}
		private void cancelAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Cancel(e);
		}
		private void DetailView_ViewEditModeChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void NewObjectAction_Changed(Object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.Active || e.ChangedPropertyType == ActionChangedType.Enabled) {
				UpdateActionState();
			}
		}
		private void NewObjectAction_ItemsChanged(Object sender, ItemsChangedEventArgs e) {
			saveAndNewAction.BeginUpdate();
			try {
				saveAndNewAction.Items.Clear();
				foreach(ChoiceActionItem choiceActionItem in newObjectViewController.NewObjectAction.Items) {
					saveAndNewAction.Items.Add(choiceActionItem.Clone());
				}
			}
			finally {
				saveAndNewAction.EndUpdate();
			}
		}
		protected virtual void ObjectSpaceModifiedChanged() {
			UpdateActionState();
		}
		protected virtual void Save(SimpleActionExecuteEventArgs args) {
		}
		protected virtual void SaveAndClose(SimpleActionExecuteEventArgs args) {
		}
		protected virtual void SaveAndNew(SingleChoiceActionExecuteEventArgs args) {
			if(!isSaveAndNewExecuting) {
				isSaveAndNewExecuting = true;
				try {
					if(newObjectViewController != null) {
						ObjectSpace.CommitChanges();
						Object prevObject = View.CurrentObject;
						Frame.SaveModel();
						if(newObjectViewController.NewObjectAction.Available) {
							newObjectViewController.NewObjectAction.DoExecute(args.SelectedChoiceActionItem);
						}
						if(prevObject == View.CurrentObject) {
							View.Close();
						}
					}
				}
				finally {
					isSaveAndNewExecuting = false;
				}
			}
		}
		protected virtual void Cancel(SimpleActionExecuteEventArgs args) {
		}
		protected virtual void UpdateActionState() {
			if(View != null) {
				saveAction.Active["IsRoot"] = View.IsRoot;
				saveAndCloseAction.Active["IsRoot"] = View.IsRoot;
				saveAndNewAction.Active["IsRoot"] = View.IsRoot;
				if(View is DetailView) {
					saveAndNewAction.Active["EditMode"] = (((DetailView)View).ViewEditMode == ViewEditMode.Edit);
				}
				saveAndNewAction.Enabled["View.AllowNew"] = View.AllowNew;
				cancelAction.Active["IsRoot"] = View.IsRoot;
				bool securityCanEdit;
				if(SecuritySystem.Instance is IRequestSecurity) {
					securityCanEdit = true;
				}
				else {
					securityCanEdit = DataManipulationRight.CanEdit(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
				}
				saveAction.Enabled["Security"] = securityCanEdit;
				saveAndCloseAction.Enabled["Security"] = securityCanEdit;
				saveAndNewAction.Enabled["Security"] = securityCanEdit;
			}
			saveAndNewAction.Active["NewObjectAction is active"] = (newObjectViewController != null) && newObjectViewController.NewObjectAction.Active;
			saveAndNewAction.Enabled["NewObjectAction is enabled"] = (newObjectViewController != null) && newObjectViewController.NewObjectAction.Enabled;
			saveAndNewAction.Active["IsDetailView"] = (DetailView != null);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			if(DetailView != null) {
				DetailView.ViewEditModeChanged += new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
			}
			if(Frame != null) {
				newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.NewObjectAction.Changed += new EventHandler<ActionChangedEventArgs>(NewObjectAction_Changed);
					newObjectViewController.NewObjectAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(NewObjectAction_ItemsChanged);
				}
			}
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(ObjectSpace != null) {
				ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			}
			if(View != null) {
				View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			}
			if(View is DetailView) {
				((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
			}
			if(newObjectViewController != null) {
				newObjectViewController.NewObjectAction.Changed -= new EventHandler<ActionChangedEventArgs>(NewObjectAction_Changed);
				newObjectViewController.NewObjectAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(NewObjectAction_ItemsChanged);
			}
			newObjectViewController = null;
		}
		public ModificationsController()
			: base() {
			TypeOfView = typeof(ObjectView);
			saveAction = new SimpleAction();
			saveAction.Caption = "Save";
			saveAction.Category = "Save";
			saveAction.Id = "Save";
			saveAction.ImageName = "MenuBar_Save";
			saveAction.Shortcut = "CtrlS";
			saveAction.QuickAccess = true;
			saveAction.Execute += new SimpleActionExecuteEventHandler(saveAction_OnExecute);
			saveAndCloseAction = new SimpleAction();
			saveAndCloseAction.Caption = "Save and Close";
			saveAndCloseAction.Category = "Save";
			saveAndCloseAction.Id = "SaveAndClose";
			saveAndCloseAction.ImageName = "MenuBar_SaveAndClose";
			saveAndCloseAction.Shortcut = "Control+Enter";
			saveAndCloseAction.QuickAccess = true;
			saveAndCloseAction.Execute += new SimpleActionExecuteEventHandler(saveAndCloseAction_OnExecute);
			saveAndNewAction = new SingleChoiceAction();
			saveAndNewAction.Caption = "Save and New";
			saveAndNewAction.Category = "Save";
			saveAndNewAction.Id = "SaveAndNew";
			saveAndNewAction.ImageName = "MenuBar_SaveAndNew";
			saveAndNewAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			saveAndNewAction.Execute += new SingleChoiceActionExecuteEventHandler(saveAndNewAction_Execute);
			cancelAction = new SimpleAction();
			cancelAction.Caption = "Cancel";
			cancelAction.Category = "UndoRedo";
			cancelAction.Id = "Cancel";
			cancelAction.ImageName = "MenuBar_Cancel";
			cancelAction.QuickAccess = true;
			cancelAction.Execute += new SimpleActionExecuteEventHandler(cancelAction_OnExecute);
			RegisterActions(saveAction, saveAndCloseAction, saveAndNewAction, cancelAction);
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerCancelAction")]
#endif
		public SimpleAction CancelAction {
			get {
				return cancelAction;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerSaveAction")]
#endif
		public SimpleAction SaveAction {
			get {
				return saveAction;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerSaveAndCloseAction")]
#endif
		public SimpleAction SaveAndCloseAction {
			get {
				return saveAndCloseAction;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerSaveAndNewAction")]
#endif
		public SingleChoiceAction SaveAndNewAction {
			get {
				return saveAndNewAction;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerDetailView")]
#endif
		public DetailView DetailView {
			get { return View as DetailView; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerListView")]
#endif
		public ListView ListView {
			get { return View as ListView; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerModificationsHandlingMode")]
#endif
		public ModificationsHandlingMode ModificationsHandlingMode {
			get { return modificationsHandlingMode; }
			set {
				modificationsHandlingMode = value;
				UpdateActionState();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModificationsControllerModificationsCheckingMode")]
#endif
		public ModificationsCheckingMode ModificationsCheckingMode {
			get { return modificationsCheckingMode; }
			set { modificationsCheckingMode = value; }
		}
	}
}
