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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public class CustomizePopupWindowParamsEventArgs : EventArgs {
		private PopupWindowShowAction action;
		private XafApplication application;
		private TemplateContext context;
		private View view;
		private bool isSizeable = true;
		private DialogController controller;
		public CustomizePopupWindowParamsEventArgs(PopupWindowShowAction action, XafApplication application) {
			this.action = action;
			this.application = application;
			this.isSizeable = action.IsSizeable;
		}
		public View View {
			get { return view; }
			set { view = value; }
		}
		public TemplateContext Context {
			get { return context; }
			set { context = value; }
		}
		public bool IsSizeable { 
			get { return isSizeable; }
			set { isSizeable = value; }
		}
		public XafApplication Application {
			get { return application; }
		}
		public DialogController DialogController {
			get { return controller; }
			set { controller = value; }
		}
		public PopupWindowShowAction Action {
			get { return action; }
		}
	}
	public delegate void CustomizePopupWindowParamsEventHandler(Object sender, CustomizePopupWindowParamsEventArgs e);
	public class PopupWindowShowActionExecuteEventArgs : SimpleActionExecuteEventArgs {
		private Window window;
		private bool canCloseWindow = true;
		public PopupWindowShowActionExecuteEventArgs(ActionBase action, ISelectionContext context, Window window)
			: base(action, context) {
			this.window = window;
		}
		public Window PopupWindow {
			get { return window; }
		}
		public View PopupWindowView {
			get {
				if(window != null) {
					return window.View;
				}
				else {
					return null;
				}
			}
		}
		public Object PopupWindowViewCurrentObject {
			get {
				if(PopupWindowView != null) {
					return PopupWindowView.CurrentObject;
				}
				else {
					return null;
				}
			}
		}
		public IList PopupWindowViewSelectedObjects {
			get {
				if(PopupWindowView != null) {
					return PopupWindowView.SelectedObjects;
				}
				else {
					return null;
				}
			}
		}
		public bool CanCloseWindow {
			get { return canCloseWindow; }
			set { canCloseWindow = value; }
		}
	}
	public delegate void PopupWindowShowActionExecuteEventHandler(Object sender, PopupWindowShowActionExecuteEventArgs e);
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(XafApplication), "Resources.Actions.Action_PopupWindowShowAction.bmp")]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafActions)]
	public class PopupWindowShowAction : ActionBase {
		public static string DefaultAcceptButtonCaption = "OK";
		public static string DefaultCancelButtonCaption = "Cancel";
		private bool modal = true;
		private bool sizeable = true;
		private ShowViewParameters currentShowViewParameters;
		private string acceptButtonCaption;
		private string cancelButtonCaption;
		private void DialogController_WindowTemplateChanged(object sender, EventArgs e) {
			Window window = ((DialogController)sender).Window;
			OnCustomizeTemplate(window.Template, window.Context);
		}
		private void DialogController_Cancelling(object sender, EventArgs e) {
			RaiseCancel();
		}
		private void DialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
			if(!e.Cancel) {
				try {
					currentShowViewParameters = null;
					e.Cancel = !DoExecute(((DialogController)sender).Window);
					if(currentShowViewParameters != null) {
						e.ShowViewParameters.Assign(currentShowViewParameters);
					}
				}
				finally {
					currentShowViewParameters = null;
				}
			}
		}
		protected override void OnProcessCreatedView(ActionBaseEventArgs e) {
			currentShowViewParameters = new ShowViewParameters();
			currentShowViewParameters.Assign(e.ShowViewParameters);
			e.ShowViewParameters.CreatedView = null;
		}
		protected internal override void RaiseExecute(ActionBaseEventArgs eventArgs) {
			Execute(this, (PopupWindowShowActionExecuteEventArgs)eventArgs);
		}
		public PopupWindowShowAction() : this(null) { }
		public PopupWindowShowAction(IContainer container) : base(container) { }
		public PopupWindowShowAction(Controller owner, string id, PredefinedCategory category) : base(owner, id, category) { }
		public PopupWindowShowAction(Controller owner, string id, string category) : base(owner, id, category) { }
		public CustomizePopupWindowParamsEventArgs GetPopupWindowParams() {
			Tracing.Tracer.LogSubSeparator("->GetPopupWindowParams");
			LogActionInfo();
			CustomizePopupWindowParamsEventArgs result = new CustomizePopupWindowParamsEventArgs(this, Application);
			if(Application != null) {
				result.DialogController = Application.CreateController<DialogController>();
			}
			else {
				result.DialogController = new DialogController();
			}
			if(!String.IsNullOrEmpty(AcceptButtonCaption)) {
				result.DialogController.AcceptAction.Caption = AcceptButtonCaption;
			}
			if(!String.IsNullOrEmpty(CancelButtonCaption)) {
				result.DialogController.CancelAction.Caption = CancelButtonCaption;
			}
			if(CustomizePopupWindowParams != null) {
				CustomizePopupWindowParams(this, result);
			}
			result.DialogController.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(DialogController_Accepting);
			result.DialogController.Cancelling += new EventHandler(DialogController_Cancelling);
			result.DialogController.WindowTemplateChanged += new EventHandler(DialogController_WindowTemplateChanged);
			Tracing.Tracer.LogSubSeparator("<- GetPopupWindowParams");
			return result;
		}
		public void OnCustomizeTemplate(IFrameTemplate frameTemplate, TemplateContext context) {
			if(CustomizeTemplate != null) {
				CustomizeTemplate(this, new CustomizeTemplateEventArgs(frameTemplate, context));
			}
		}
		public bool DoExecute(Window window) {
			PopupWindowShowActionExecuteEventArgs args = new PopupWindowShowActionExecuteEventArgs(this, SelectionContext, window);
			bool executingResult = ExecuteCore(Execute, args);
			return executingResult && args.CanCloseWindow;
		}
		public void RaiseCancel() {
			if(Cancel != null) {
				Cancel(this, EventArgs.Empty);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionAcceptButtonCaption"),
#endif
 Category("Appearance"), DefaultValue("OK")]
		public string AcceptButtonCaption {
			get { return acceptButtonCaption; }
			set { acceptButtonCaption = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionCancelButtonCaption"),
#endif
 Category("Appearance"), DefaultValue("Cancel")]
		public string CancelButtonCaption {
			get { return cancelButtonCaption; }
			set { cancelButtonCaption = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionIsModal"),
#endif
 Category("Behavior"), DefaultValue(true)]
		public bool IsModal {
			get { return modal; }
			set { modal = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionIsSizeable"),
#endif
 Category("Behavior"), DefaultValue(true)]
		public bool IsSizeable {
			get { return sizeable; }
			set { sizeable = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionCustomizePopupWindowParams"),
#endif
 Category("Action")]
		public event CustomizePopupWindowParamsEventHandler CustomizePopupWindowParams;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionCustomizeTemplate"),
#endif
 Category("Action")]
		public event EventHandler<CustomizeTemplateEventArgs> CustomizeTemplate;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionExecute"),
#endif
 Category("Action")]
		public event PopupWindowShowActionExecuteEventHandler Execute;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("PopupWindowShowActionCancel"),
#endif
 Category("Action")]
		public event EventHandler Cancel;
	}
}
