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
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.SystemModule {
	public class DialogControllerAcceptingEventArgs : CancelEventArgs {
		private ShowViewParameters showViewParameters;
		private SimpleActionExecuteEventArgs acceptActionArgs;
		public DialogControllerAcceptingEventArgs(ShowViewParameters showViewParameters, SimpleActionExecuteEventArgs acceptActionArgs)
			: base(false) {
			this.showViewParameters = showViewParameters;
			this.acceptActionArgs = acceptActionArgs;
		}
		public ShowViewParameters ShowViewParameters {
			get { return showViewParameters; }
		}
		public SimpleActionExecuteEventArgs AcceptActionArgs {
			get { return acceptActionArgs; }
		}
	}
	public class DialogController : WindowController {
		public const string DialogActionContainerName = "PopupActions";
		private const String isTargetTemplateContextKey = "IsTargetTemplateContext";
		private List<Controller> controllers = new List<Controller>();
		private bool canCloseWindow = true;
		private SimpleAction acceptAction;
		private SimpleAction cancelAction;
		private SimpleAction closeAction;
		private bool saveOnAccept = true;
		private bool acceptCancelled = false;
		private Boolean closeOnCurrentObjectProcessing;
		private void Window_TemplateChanged(object sender, EventArgs e) {
			OnWindowTemplateChanged();
		}
		private void acceptAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Accept(e);
		}
		private void acceptAction_ExecuteCompleted(object sender, ActionBaseEventArgs e) {
			if(Window != null && CanCloseWindow && !acceptCancelled) {
				Window.Close(e.ShowViewParameters.CreatedView == null);
			}
		}
		private void cancelAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Cancel(e);
		}
		private void Window_ViewChanging(object sender, ViewChangingEventArgs e) {
			WindowViewChanging();
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			WindowViewChanged();
			if(SaveOnAccept && (Window != null) && (Window.View is ObjectView)) {
				if(((View)Window.View.ObjectSpace.Owner != null) && (Window.View != (View)Window.View.ObjectSpace.Owner)) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.IncompatibleParametersCombinationOnViewChanged));
				}
			}
		}
		private void processCurrentObjectController_CustomProcessSelectedItem(Object sender, CustomProcessListViewSelectedItemEventArgs e) {
			e.Handled = true;
			if(closeAction.Available) {
				closeAction.DoExecute();
			}
		}
		private bool CanProcessSelectedItem() {
			return Window.View is ListView && CloseOnCurrentObjectProcessing;
		}
		protected virtual void OnWindowTemplateChanged() {
			if(WindowTemplateChanged != null) {
				WindowTemplateChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void WindowViewChanging() {
			ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
		}
		protected virtual void WindowViewChanged() {
			if(CanProcessSelectedItem()) {
				ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
				if(processCurrentObjectController != null) {
					processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
				}
			}
		}
		protected virtual void Accept(SimpleActionExecuteEventArgs args) {
			acceptCancelled = false;
			DialogControllerAcceptingEventArgs cancelArgs = new DialogControllerAcceptingEventArgs(args.ShowViewParameters, args);
			cancelArgs.Cancel = false;
			if(Accepting != null) {
				Accepting(this, cancelArgs);
				acceptCancelled = cancelArgs.Cancel;
			}
			if(!cancelArgs.Cancel && (Window != null) && (Window.Context == TemplateContext.PopupWindow) && saveOnAccept) {
				ModificationsController modificationsController = Window.GetController<ModificationsController>();
				if((modificationsController != null) && modificationsController.SaveAction.Available) {
					acceptCancelled = !modificationsController.SaveAction.DoExecute();
				}
			}
		}
		protected virtual void Cancel(SimpleActionExecuteEventArgs args) {
			if(Cancelling != null) {
				Cancelling(this, EventArgs.Empty);
			}
			if(Window != null && CanCloseWindow) {
				Window.Close(false);
			}
		}
		protected virtual void UpdateActionState() {
			acceptAction.Active["Has caption"] = !String.IsNullOrEmpty(AcceptAction.Caption);
			cancelAction.Active["Has caption"] = !String.IsNullOrEmpty(CancelAction.Caption);
			closeAction.Active["CloseOnCurrentObjectProcessing"] = CloseOnCurrentObjectProcessing;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.ViewChanging += new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
			Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			Window.TemplateChanged += new EventHandler(Window_TemplateChanged);
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			if(Window != null) {
				Window.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
				Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
				Window.TemplateChanged -= new EventHandler(Window_TemplateChanged);
			}
			base.OnDeactivated();
		}
		protected virtual SimpleAction CreateAcceptAction() {
			SimpleAction result = new SimpleAction(this, "DialogOK", DialogActionContainerName);
			result.Caption = "OK";
			return result;
		}
		protected virtual SimpleAction CreateCancelAction() {
			SimpleAction result = new SimpleAction(this, "DialogCancel", DialogActionContainerName);
			result.Caption = "Cancel";
			return result;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			controllers = null;
		}
		public DialogController() {
			closeOnCurrentObjectProcessing = true;
			acceptAction = CreateAcceptAction();
			acceptAction.ActionMeaning = ActionMeaning.Accept;
			acceptAction.Execute += new SimpleActionExecuteEventHandler(acceptAction_OnExecute);
			acceptAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(acceptAction_ExecuteCompleted);
			cancelAction = CreateCancelAction();
			cancelAction.ActionMeaning = ActionMeaning.Cancel;
			cancelAction.Execute += new SimpleActionExecuteEventHandler(cancelAction_OnExecute);
			closeAction = new SimpleAction(this, "DialogClose", "None");
			closeAction.Caption = "Close";
			closeAction.Execute += new SimpleActionExecuteEventHandler(acceptAction_OnExecute);
			closeAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(acceptAction_ExecuteCompleted);
			closeAction.TargetViewType = ViewType.ListView;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DialogControllerSaveOnAccept")]
#endif
		public Boolean SaveOnAccept {
			get { return saveOnAccept; }
			set { saveOnAccept = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DialogControllerAcceptAction")]
#endif
		public SimpleAction AcceptAction {
			get { return acceptAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DialogControllerCancelAction")]
#endif
		public SimpleAction CancelAction {
			get { return cancelAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DialogControllerCloseAction")]
#endif
		public SimpleAction CloseAction {
			get { return closeAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DialogControllerCanCloseWindow")]
#endif
		public bool CanCloseWindow {
			get { return canCloseWindow; }
			set { canCloseWindow = value; }
		}
		[Browsable(false)]
		public List<Controller> Controllers {
			get { return controllers; }
		}
		[DefaultValue(true)]
		public Boolean CloseOnCurrentObjectProcessing {
			get { return closeOnCurrentObjectProcessing; }
			set { closeOnCurrentObjectProcessing = value; }
		}
		public event EventHandler WindowTemplateChanged;
		public event EventHandler<DialogControllerAcceptingEventArgs> Accepting;
		public event EventHandler Cancelling;
	}
}
