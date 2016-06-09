#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraScheduler;
	using DevExpress.XtraScheduler.Native;
	[ToolboxItem(false)]
	public class MVCxScheduler: ASPxScheduler, IViewContext {
		ViewContext viewContext;
		NameValueCollection postDataCollection;
		System.Web.Mvc.IValueProvider defaultfValueProvider;
		public MVCxScheduler()
			: this(null) {
		}
		protected internal MVCxScheduler(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.defaultfValueProvider = ExtensionsHelper.CreateDefaultValueProvider();
		}
		public new MVCxSchedulerStorage Storage { get { return (MVCxSchedulerStorage)base.Storage; } }
		public new MVCxSchedulerOptionsForms OptionsForms { get { return (MVCxSchedulerOptionsForms)base.OptionsForms; } }
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public object EditAppointmentRouteValues { get; set; }
		protected internal string DateNavigatorId { get; set; }
		protected internal new SchedulerFormType ActiveFormType { get { return base.ActiveFormType; } set { base.ActiveFormType = value; } }
		protected internal new InnerSchedulerControl InnerControl { get { return base.InnerControl; } }
		protected internal new MVCxSchedulerCallbackCommandManager CallbackCommandManager {
			get { return (MVCxSchedulerCallbackCommandManager)base.CallbackCommandManager; }
		}
		protected internal new MVCxFormBlock FormBlock {
			get { return (MVCxFormBlock)base.FormBlock; }
		}
		public override bool IsCallback {
			get { return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == ID; }
		}
		public override bool IsLoading() {
			return false;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxSchedulerClientSideEvents();
		}
		protected override bool IsForceBeginInitAfterApplyingSkin() {
			return false;
		}
		ViewContext ViewContext { get { return viewContext; } }
		ControllerBase Controller {
			get { return ViewContext != null ? ViewContext.Controller : null; }
		}
		System.Web.Mvc.IValueProvider ValueProvider {
			get { return Controller != null && Controller.ValueProvider != null ? Controller.ValueProvider : this.defaultfValueProvider; }
		}
		protected override NameValueCollection PostDataCollection {
			get {
				if(postDataCollection == null)
					postDataCollection = new MvcPostDataCollection(ValueProvider);
				return postDataCollection;
			}
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(CallbackRouteValues!= null)
				sb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				sb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
			if (EditAppointmentRouteValues != null)
				sb.Append(localVarName + ".editAppointmentsUrl=\"" + Utils.GetUrl(EditAppointmentRouteValues) + "\";\n");
			if (OptionsForms.AppointmentFormRouteValues != null)
				sb.Append(localVarName + ".appointmentFormUrl=\"" + Utils.GetUrl(OptionsForms.AppointmentFormRouteValues) + "\";\n");
			if (OptionsForms.AppointmentInplaceEditorFormRouteValues != null)
				sb.Append(localVarName + ".appointmentInplaceEditorFormUrl=\"" + Utils.GetUrl(OptionsForms.AppointmentInplaceEditorFormRouteValues) + "\";\n");
			if (OptionsForms.GotoDateFormRouteValues != null)
				sb.Append(localVarName + ".gotoDateFormUrl=\"" + Utils.GetUrl(OptionsForms.GotoDateFormRouteValues) + "\";\n");
			if (OptionsForms.RecurrentAppointmentDeleteFormRouteValues != null)
				sb.Append(localVarName + ".recurrentAppointmentDeleteFormUrl=\"" + Utils.GetUrl(OptionsForms.RecurrentAppointmentDeleteFormRouteValues) + "\";\n");
			if (OptionsForms.RemindersFormRouteValues != null)
				sb.Append(localVarName + ".remindersFormUrl=\"" + Utils.GetUrl(OptionsForms.RemindersFormRouteValues) + "\";\n");
			sb.Append(localVarName + ".start=" + HtmlConvertor.ToScript(Start) + ";\n");
			if (Storage.ResourceSharing)
				sb.Append(localVarName + ".resourceSharing=true;\n");
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxScheduler), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxScheduler), Utils.SchedulerScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientScheduler";
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override ASPxSchedulerStorage CreateSchedulerStorageCore() {
			return new MVCxSchedulerStorage(this);
		}
		protected override SchedulerCallbackCommandManager CreateCallbackCommandManager() {
			return new MVCxSchedulerCallbackCommandManager(this);
		}
		protected internal new CallbackCommandInfo CreateCommandInfo(string callbackArgument) {
			return base.CreateCommandInfo(callbackArgument);
		}
		protected override ASPxSchedulerOptionsForms CreateOptionsForms() {
			return new MVCxSchedulerOptionsForms();
		}
		protected override void ExecuteCallbackCommand(CallbackCommandInfo commandInfo) {
			base.ExecuteCallbackCommand(commandInfo);
			IEditableAppointments command = CallbackCommandManager.GetCommandForEditableActions(commandInfo) as IEditableAppointments;
			if (command != null)
				command.FinalizeExecute();
		}
		protected internal new Appointment GetEditedAppointment() {
			return base.GetEditedAppointment();
		}
		#region Template Controls
		protected internal Control AppointmentFormTemplateControl { get; set; }
		protected internal Control AppointmentInplaceEditorFormTemplateControl { get; set; }
		protected internal Control GotoDateFormTemplateControl { get; set; }
		protected internal Control RecurrentAppointmentDeleteFormControl { get; set; }
		protected internal Control RecurrentAppointmentEditFormTemplateContentControl { get; set; }
		protected internal Control RemindersFormTemplateContentControl { get; set; }
		protected internal Control AppointmentToolTipTemplateControl { get; set; }
		protected internal Control AppointmentDragToolTipTemplateControl { get; set; }
		protected internal Control SelectionToolTipTemplateControl { get; set; }
		#endregion
		#region Create Blocks
		protected override ResourceNavigatorBlock CreateResourceNavigatorBlock() {
			return new MVCxResourceNavigatorBlock(this);
		}
		protected override ViewSelectorBlock CreateViewSelectorBlock() {
			return new MVCxViewSelectorBlock(this);
		}
		protected override ViewNavigatorBlock CreateViewNavigatorBlock() {
			return new MVCxViewNavigatorBlock(this);
		}
		protected override ViewVisibleIntervalBlock CreateViewVisibleIntervalBlock() {
			return new MVCxViewVisibleIntervalBlock(this);
		}
		protected override MenuControlBlockBase CreateViewMenuBlock() {
			return new MVCxViewMenuBlock(this);
		}
		protected override SchedulerStatusInfoManagerBlock CreateStatusInfoManagerBlock() {
			return new MVCxSchedulerStatusInfoManagerBlock(this);
		}
		protected override FormBlock CreateFormBlock() {
			return new MVCxFormBlock(this);
		}
		protected override ASPxSchedulerControlScriptBlock CreateScriptBlock() {
			return new MVCxSchedulerControlScriptBlock(this, this);
		}
		protected override MessageBoxBlock CreateMessageBoxBlock() {
			return new MVCxSchedulerMessageBoxBlock(this);
		}
		protected override ToolTipControlBlock CreateToolTipControlBlock() {
			return new MVCxToolTipControlBlock(this);
		}
		#endregion
		#region IViewContext Members
		ViewContext IViewContext.ViewContext { get { return ViewContext; } }
		#endregion
	}
}
