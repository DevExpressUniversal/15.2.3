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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class ProcessDataLockingInfoController : ObjectViewController {
		private ModificationsController modificationsViewController;
		protected override void OnActivated() {
			base.OnActivated();
			modificationsViewController = Frame.GetController<ModificationsController>();
			if(modificationsViewController != null) {
				modificationsViewController.SaveAction.Executing += new CancelEventHandler(action_Executing);
				modificationsViewController.SaveAndNewAction.Executing += new CancelEventHandler(action_Executing);
				modificationsViewController.SaveAndCloseAction.Executing += new CancelEventHandler(action_Executing);
			}
		}
		protected override void OnDeactivated() {
			if(modificationsViewController != null) {
				modificationsViewController.SaveAction.Executing -= new CancelEventHandler(action_Executing);
				modificationsViewController.SaveAndNewAction.Executing -= new CancelEventHandler(action_Executing);
				modificationsViewController.SaveAndCloseAction.Executing -= new CancelEventHandler(action_Executing);
				modificationsViewController = null;
			}
			base.OnDeactivated();
		}
		private void action_Executing(object sender, CancelEventArgs e) {
			if(!e.Cancel) {
				Boolean cancel;
				ProcessDataLockingInfo(out cancel);
				e.Cancel = cancel;
			}
		}
		protected void ProcessDataLockingInfo(out Boolean cancelAction) {
			DataLockingInfo dataLockingInfo = GetDataLockingInfo();
			ProcessDataLockingInfo(dataLockingInfo, out cancelAction);
		}
		protected virtual DataLockingInfo GetDataLockingInfo() {
			DataLockingInfo result;
			IDataLockingManager manager = ObjectSpace as IDataLockingManager;
			if(manager != null && manager.IsActive) {
				result = manager.GetDataLockingInfo();
			}
			else {
				result = DataLockingInfo.Empty;
			}
			return result;
		}
		protected virtual void ProcessDataLockingInfo(DataLockingInfo dataLockingInfo, out Boolean cancelAction) {
			Guard.ArgumentNotNull(dataLockingInfo, "dataLockingInfo");
			IDataLockingManager manager = ObjectSpace as IDataLockingManager;
			if(manager != null && manager.IsActive && dataLockingInfo.IsLocked) {
				ProcessDataLockingInfoDialogResult dialogResult = GetUserChoice(dataLockingInfo);
				switch(dialogResult) {
					case ProcessDataLockingInfoDialogResult.Merge:
						manager.MergeData(dataLockingInfo);
						break;
					case ProcessDataLockingInfoDialogResult.Refresh:
						manager.RefreshData(dataLockingInfo);
						break;
				}
				cancelAction = true;
			}
			else {
				cancelAction = false;
			}
		}
		protected virtual ProcessDataLockingInfoDialogResult GetUserChoice(DataLockingInfo dataLockingInfo) {
			ProcessDataLockingInfoDialogResult dialogResult;
			ProcessDataLockingInfoDialogController controller = Application.CreateController<ProcessDataLockingInfoDialogController>();
			if(dataLockingInfo.CanMerge) {
				dialogResult = controller.ShowMergeDialog(Application);
			}
			else {
				dialogResult = controller.ShowRefreshDialog(Application);
			}
			return dialogResult;
		}
	}
	[DomainComponent]
	public class ProcessDataLockingInfoDialogObject { }
	public enum ProcessDataLockingInfoDialogResult {
		None,
		Merge,
		Refresh,
		Cancel
	}
	public sealed class ProcessDataLockingInfoDialogController : ViewController<DetailView> {
		const String MergeDialog_DetailView_Id = "ProcessDataLockingInfoMergeDialog_DetailView";
		const String RefreshDialog_DetailView_Id = "ProcessDataLockingInfoRefreshDialog_DetailView";
		private SimpleAction mergeAction;
		private SimpleAction refreshAction;
		private SimpleAction cancelAction;
		private ProcessDataLockingInfoDialogResult result;
		public ProcessDataLockingInfoDialogController() {
			TargetObjectType = typeof(ProcessDataLockingInfoDialogObject);
			mergeAction = new SimpleAction(this, "MergeAction", DialogController.DialogActionContainerName);
			mergeAction.TargetViewId = MergeDialog_DetailView_Id;
			mergeAction.Caption = "Merge";
			mergeAction.Execute += new SimpleActionExecuteEventHandler(mergeAction_Execute);
			mergeAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(action_ExecuteCompleted);
			refreshAction = new SimpleAction(this, "RefreshAction", DialogController.DialogActionContainerName);
			refreshAction.Caption = "Refresh";
			refreshAction.Execute += new SimpleActionExecuteEventHandler(refreshAction_Execute);
			refreshAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(action_ExecuteCompleted);
			cancelAction = new SimpleAction(this, "CancelAction", DialogController.DialogActionContainerName);
			cancelAction.Caption = "Cancel";
			cancelAction.Execute += new SimpleActionExecuteEventHandler(cancelAction_Execute);
			cancelAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(action_ExecuteCompleted);
		}
		private void mergeAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			result = ProcessDataLockingInfoDialogResult.Merge;
		}
		private void refreshAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			result = ProcessDataLockingInfoDialogResult.Refresh;
		}
		private void cancelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			result = ProcessDataLockingInfoDialogResult.Cancel;
		}
		private void action_ExecuteCompleted(object sender, ActionBaseEventArgs e) {
			CloseWindow();
		}
		private void CloseWindow() {
			((Window)Frame).Close();
		}
		public ProcessDataLockingInfoDialogResult ShowMergeDialog(XafApplication application) {
			return ShowDialog(application, MergeDialog_DetailView_Id);
		}
		public ProcessDataLockingInfoDialogResult ShowRefreshDialog(XafApplication application) {
			return ShowDialog(application, RefreshDialog_DetailView_Id);
		}
		private ProcessDataLockingInfoDialogResult ShowDialog(XafApplication application, String viewId) {
			result = ProcessDataLockingInfoDialogResult.None;
			IObjectSpace objectSpace = application.CreateObjectSpace(typeof(ProcessDataLockingInfoDialogObject));
			ProcessDataLockingInfoDialogObject obj = new ProcessDataLockingInfoDialogObject();
			DetailView detailView = application.CreateDetailView(objectSpace, viewId, true, obj);
			ShowViewParameters showViewParameters = new ShowViewParameters(detailView);
			showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			showViewParameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = application.CreateController<DialogController>();
			dialogController.Active["ProcessDataLockingInfoDialogController"] = false;
			showViewParameters.Controllers.Add(dialogController);
			showViewParameters.Controllers.Add(this);
			application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(null, null));
			return result;
		}
	}
}
