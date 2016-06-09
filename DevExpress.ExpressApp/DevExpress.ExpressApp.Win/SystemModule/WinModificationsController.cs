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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class WinModificationsController : ModificationsController {
		private const String isModifiedKey = "Is modified";
		private const String autoCommitListViewKey = "AutoCommitListView";
		private Boolean isClosing;
		private ActionBase refreshAction;
		private Boolean refreshViewAfterObjectSpaceCommit;
		private void ObjectSpace_ConfirmationRequired(Object sender, ConfirmationEventArgs e) {
			OnObjectSpaceConfirmationRequired(e);
		}
		private void View_QueryCanChangeCurrentObject(Object sender, CancelEventArgs e) {
			if(!e.Cancel) {
				OnViewQueryCanChangeCurrentObject(e);
			}
		}
		private void View_QueryCanClose(Object sender, CancelEventArgs e) {
			if(!e.Cancel) {
				OnViewQueryCanClose(e);
			}
		}
		private void ListView_ValidateObject(Object sender, ValidateObjectEventArgs e) {
			if(e.Valid) {
				OnListViewValidateObject(e);
			}
		}
		protected Boolean IsChangesManagementActionExecuting() {
			return SaveAction.IsExecuting
				|| SaveAndCloseAction.IsExecuting
				|| SaveAndNewAction.IsExecuting
				|| CancelAction.IsExecuting
				|| (refreshAction != null) && refreshAction.IsExecuting;
		}
		protected void RollbackChanges() {
			skipConfirmation = true;
			try {
				ObjectSpace.Rollback();
			}
			finally {
				skipConfirmation = false;
			}
		}
		protected virtual void OnObjectSpaceConfirmationRequired(ConfirmationEventArgs e) {
			if(!skipConfirmation) {
				if(modificationsHandlingMode == ModificationsHandlingMode.Confirmation) {
					e.ConfirmationResult = Application.AskConfirmation(e.ConfirmationType);
				}
			}
		}
		protected virtual void OnViewQueryCanChangeCurrentObject(CancelEventArgs e) {
			if((modificationsCheckingMode == ModificationsCheckingMode.Always) && ObjectSpace.IsModified && !ObjectSpace.IsDeleting && !IsChangesManagementActionExecuting()) {
				if(modificationsHandlingMode == ModificationsHandlingMode.Confirmation) {
					ConfirmationResult answer = Application.AskConfirmation(ConfirmationType.NeedSaveChanges);
					if(answer == ConfirmationResult.Cancel) {
						e.Cancel = true;
					}
					else if(answer == ConfirmationResult.Yes) {
						ObjectSpace.CommitChanges();
					}
					else {
						RollbackChanges();
					}
				}
				else if(modificationsHandlingMode == ModificationsHandlingMode.AutoCommit) {
					if(ObjectSpace.ModifiedObjects.Count > 0) {
						ObjectSpace.CommitChanges();
					}
				}
				else if(modificationsHandlingMode == ModificationsHandlingMode.AutoRollback) {
					RollbackChanges();
				}
			}
		}
		protected virtual void OnViewQueryCanClose(CancelEventArgs e) {
			if(!isClosing && ObjectSpace.IsModified) {
				if(modificationsHandlingMode == ModificationsHandlingMode.Confirmation) {
					WinWindow window = Frame as WinWindow;
					if(window != null && window.Form != null) {
						window.Form.Activate();
					}
					ConfirmationResult answer = Application.AskConfirmation(ConfirmationType.NeedSaveChanges);
					if(answer == ConfirmationResult.Cancel) {
						e.Cancel = true;
					}
					else if(answer == ConfirmationResult.Yes) {
						ObjectSpace.CommitChanges();
					}
				}
				else if(modificationsHandlingMode == ModificationsHandlingMode.AutoCommit) {
					if(!ObjectSpace.IsDeleting && (ObjectSpace.ModifiedObjects.Count > 0)) {
						ObjectSpace.CommitChanges();
					}
				}
			}
		}
		protected virtual void OnListViewValidateObject(ValidateObjectEventArgs e) {
			if((modificationsCheckingMode == ModificationsCheckingMode.Always) && ObjectSpace.IsModified && !ObjectSpace.IsDeleting && !IsChangesManagementActionExecuting()) {
				if(modificationsHandlingMode == ModificationsHandlingMode.Confirmation) {
					ConfirmationResult answer = Application.AskConfirmation(ConfirmationType.NeedSaveChanges);
					if(answer == ConfirmationResult.Cancel) {
						e.Valid = false;
					}
					else if(answer == ConfirmationResult.Yes) {
						ObjectSpace.CommitChanges();
					}
					else {
						RollbackChanges();
					}
				}
			}
		}
		protected override void Save(SimpleActionExecuteEventArgs args) {
			ObjectSpace.CommitChanges();
			if(refreshViewAfterObjectSpaceCommit) {
				View.Refresh();
			}
			UpdateActionState();
		}
		protected override void SaveAndClose(SimpleActionExecuteEventArgs args) {
			ObjectSpace.CommitChanges();
			if(!View.ObjectSpace.IsModified) {
				View.Close();
			}
		}
		protected override void Cancel(SimpleActionExecuteEventArgs args) {
			if((modificationsHandlingMode != ModificationsHandlingMode.Confirmation) || (Application == null) || (Application.AskConfirmation(ConfirmationType.CancelChanges) == ConfirmationResult.Yes)) {
				if(View is DetailView) {
					if(ObjectSpace.IsNewObject(DetailView.CurrentObject)) {
						isClosing = true;
						try {
							View.Close();
						}
						finally {
							isClosing = false;
						}
					}
					else {
						RollbackChanges();
						UpdateActionState();
					}
				}
				if(View is ListView) {
					RollbackChanges();
					UpdateActionState();
				}
			}
		}
		protected override void UpdateActionState() {
			base.UpdateActionState();
			if((View != null)) {
				Boolean changed = ObjectSpace.IsModified || ObjectSpace.IsNewObject(((ObjectView)View).CurrentObject);
				SaveAction.Enabled[isModifiedKey] = changed;
				CancelAction.Enabled[isModifiedKey] = changed;
			}
			if(ListView != null) {
				SaveAction.Active[autoCommitListViewKey] = (modificationsHandlingMode == ModificationsHandlingMode.Confirmation);
				SaveAndCloseAction.Active[autoCommitListViewKey] = (modificationsHandlingMode == ModificationsHandlingMode.Confirmation);
				CancelAction.Active[autoCommitListViewKey] = (modificationsHandlingMode == ModificationsHandlingMode.Confirmation);
			}
			else {
				SaveAction.Active.RemoveItem(autoCommitListViewKey);
				SaveAndCloseAction.Active.RemoveItem(autoCommitListViewKey);
				CancelAction.Active.RemoveItem(autoCommitListViewKey);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame != null) {
				RefreshController refreshController = Frame.GetController<RefreshController>();
				if(refreshController != null) {
					refreshAction = refreshController.RefreshAction;
				}
			}
			ObjectSpace.ConfirmationRequired += new EventHandler<ConfirmationEventArgs>(ObjectSpace_ConfirmationRequired);
			View.QueryCanClose += new EventHandler<CancelEventArgs>(View_QueryCanClose);
			((ObjectView)View).QueryCanChangeCurrentObject += new EventHandler<CancelEventArgs>(View_QueryCanChangeCurrentObject);
			if(ListView != null) {
				ListView.ValidateObject += new EventHandler<ValidateObjectEventArgs>(ListView_ValidateObject);
				modificationsCheckingMode = ModificationsCheckingMode.OnCloseOnly;
				if((ListView.Model != null) && (ListView.Model.AllowEdit || (ListView.Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView))) {
					modificationsHandlingMode = ModificationsHandlingMode.Confirmation;
				}
				else {
					modificationsHandlingMode = ModificationsHandlingMode.AutoCommit;
				}
			}
			else if(DetailView != null) {
				modificationsCheckingMode = ModificationsCheckingMode.Always;
				modificationsHandlingMode = ModificationsHandlingMode.Confirmation;
			}
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			ObjectSpace.ConfirmationRequired -= new EventHandler<ConfirmationEventArgs>(ObjectSpace_ConfirmationRequired);
			View.QueryCanClose -= new EventHandler<CancelEventArgs>(View_QueryCanClose);
			((ObjectView)View).QueryCanChangeCurrentObject -= new EventHandler<CancelEventArgs>(View_QueryCanChangeCurrentObject);
			if(ListView != null) {
				ListView.ValidateObject -= new EventHandler<ValidateObjectEventArgs>(ListView_ValidateObject);
			}
			base.OnDeactivated();
		}
		public WinModificationsController() {
			TargetViewNesting = Nesting.Root;
			refreshViewAfterObjectSpaceCommit = true;
		}
		public Boolean RefreshViewAfterObjectSpaceCommit {
			get { return refreshViewAfterObjectSpaceCommit; }
			set { refreshViewAfterObjectSpaceCommit = value; }
		}
	}
}
