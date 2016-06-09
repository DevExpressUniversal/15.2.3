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

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Web.ASPxScheduler {
	#region SchedulerFormVisibility
	public enum SchedulerFormVisibility {
		None,
		PopupWindow,
		FillControlArea
	}
	#endregion
	#region SchedulerFormTemplateContainer
	public abstract class SchedulerFormTemplateContainer : SchedulerTemplateContainerBase {
		#region Fields
		readonly TimeZoneHelper timeZoneEngine;
		readonly ASPxScheduler control;
		string caption;
		#endregion
		protected SchedulerFormTemplateContainer(ASPxScheduler control)
			: base(0, null) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
			this.timeZoneEngine = Control.InnerControl.TimeZoneHelper;
			this.caption = String.Empty;
		}
		#region Properties
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerControl")]
#endif
		public ASPxScheduler Control { get { return control; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerControlClientId")]
#endif
		public string ControlClientId { get { return Control.ClientID; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerControlId")]
#endif
		public string ControlId { get { return Control.ID; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerTimeZoneHelper")]
#endif
		public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerCancelHandler")]
#endif
		public abstract string CancelHandler { get; }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerCancelScript")]
#endif
		public abstract string CancelScript { get; }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerIsFormRecreated")]
#endif
		public bool IsFormRecreated { get { return control.IsFormRecreated; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("SchedulerFormTemplateContainerCaption")]
#endif
		public string Caption { get { return caption; } set { caption = value; } }
		#endregion
		#region DataBind
		public override void DataBind() {
			base.DataBind();
			if (!Control.IgnoreFormPostData)
				LoadPostData(this);
		}
		#endregion
		#region LoadPostData
		protected internal virtual void LoadPostData(Control parent) {
		}
		#endregion
		#region FindControl
		public override Control FindControl(string id) {
			if (!Control.Loaded)
				return null;
			TemplateControl templateControl = Controls[0] as TemplateControl;
			if (templateControl != null) {
				Control result = templateControl.FindControl(id);
				if (result != null)
					return result;
			}
			return base.FindControl(id);
		}
		protected override Control FindControl(string id, int pathOffset) {
			if (!Control.Loaded)
				return null;
			return base.FindControl(id, pathOffset);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public enum SchedulerFormType { None, Appointment, AppointmentInplace, GotoDate, RecurrentAppointmentDelete, RecurrentAppointmentEdit, Reminder }
	public static class SchedulerFormNames {
		public const string AppointmentForm = "AppointmentForm";
		public const string AppointmentFormEx = "AppointmentFormEx";
		public const string InplaceEditorForm = "InplaceEditor";
		public const string GotoDateForm = "GotoDateForm";
		public const string RecurrentAppointmentDeleteForm = "RecurrentAppointmentDeleteForm";
		public const string RecurrentAppointmentEditForm = "RecurrentAppointmentEditForm";
		public const string HorizontalAppointmentTemplate = "HorizontalAppointmentTemplate";
		public const string HorizontalSameDayAppointmentTemplate = "HorizontalSameDayAppointmentTemplate";
		public const string VerticalAppointmentTemplate = "VerticalAppointmentTemplate";
		public const string RemindersForm = "ReminderForm";
		public const string AppointmentToolTip = "AppointmentToolTip";
		public const string AppointmentDragToolTip = "AppointmentDragToolTip";
		public const string SelectionToolTip = "SelectionToolTip";
		public const string MessageBox = "MessageBox";
	}
	#region FormContainerBuilder
	public abstract class FormContainerBuilder {
		string schedulerClientId;
		protected FormContainerBuilder(string schedulerClientId) {
			this.schedulerClientId = schedulerClientId;
		}
		public string SchedulerClientId { get { return schedulerClientId; } }
		protected internal void BuildFormContainer(ASPxScheduler control, Control parent, string formContainerId, SchedulerFormTemplateContainer templateContainer) {
			Control formContainer = CreateFormContainer(control, parent, formContainerId);
			InitializeFormContainerProperties(formContainer, control);
			InitializeFormContainerStyles(formContainer, control);
			AddTemplateContainer(formContainer, templateContainer);
			PrepareFormContainerScripts(formContainer, templateContainer);
		}
		protected internal abstract Control CreateFormContainer(ASPxScheduler control, Control parent, string id);
		protected internal abstract void AddTemplateContainer(Control container, SchedulerFormTemplateContainer child);
		protected internal virtual void InitializeFormContainerProperties(Control formContainer, ASPxScheduler control) {
		}
		protected internal virtual void InitializeFormContainerStyles(Control formContainer, ASPxScheduler control) {
		}
		protected internal virtual void PrepareFormContainerScripts(Control formContainer, SchedulerFormTemplateContainer templateContainer) {
		}
	}
	#endregion
	#region PopupFormBaseContainerBuilder
	public abstract class PopupFormBaseContainerBuilder : FormContainerBuilder {
		int defaultWidht = 0;
		int defaultHeight = 0;
		string headerText = string.Empty;
		DevExpress.Web.ASPxPopupControl popupControl;
		protected PopupFormBaseContainerBuilder(string schedulerClientId)
			: base(schedulerClientId) {
		}
		#region Properties
		public string HeaderText { get { return headerText; } set { headerText = value; } }
		public int DefaultWidth { get { return defaultWidht; } set { defaultWidht = value; } }
		public int DefaultHeight { get { return defaultHeight; } set { defaultHeight = value; } }
		internal DevExpress.Web.ASPxPopupControl PopupControl { get { return popupControl; } }
		#endregion
		protected virtual ASPxSchedulerPopupForm CreatePopupForm(ASPxScheduler control) {
			return new ASPxSchedulerPopupForm(control);
		}
		protected internal override Control CreateFormContainer(ASPxScheduler control, Control parent, string id) {
			ASPxSchedulerPopupForm popup = CreatePopupForm(control);
			popup.ID = id;
			parent.Controls.Add(popup);
			this.popupControl = popup;
			return popup;
		}
		protected internal override void AddTemplateContainer(Control container, SchedulerFormTemplateContainer child) {
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)container;
			popup.Controls.Add(child);
		}
		protected internal override void InitializeFormContainerProperties(Control formContainer, ASPxScheduler control) {
			base.InitializeFormContainerProperties(formContainer, control);
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			if (popup.Width == Unit.Empty)
				popup.Width = DefaultWidth;
			if (popup.Height == Unit.Empty)
				popup.Height = DefaultHeight;
		}
	}
	#endregion
	#region PopupFormContainerBuilder
	public class PopupFormContainerBuilder : PopupFormBaseContainerBuilder {
		public PopupFormContainerBuilder(string schedulerClientId)
			: base(schedulerClientId) {
		}
		protected internal override void InitializeFormContainerProperties(Control formContainer, ASPxScheduler control) {
			base.InitializeFormContainerProperties(formContainer, control);
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			popup.Modal = true;
			popup.CloseAction = CloseAction.CloseButton;
			popup.ParentSkinOwner = control;
			popup.AllowDragging = true;
			popup.AllowResize = false;
			popup.HeaderText = HeaderText;
			popup.ContentStyle.VerticalAlign = VerticalAlign.Top;
			popup.ClientSideEvents.PopUp =
@"function(s, e) {{
    if (ASPx.Browser.IE) {{
        s.GetWindowMainCell(s.GetWindowElement(-1)).style.height = '0px';
        var windowElement = s.GetWindowContentElement(-1);
        s.SetHeight(0);
    }}
}}";
		}
		protected internal override void InitializeFormContainerStyles(Control formContainer, ASPxScheduler control) {
			base.InitializeFormContainerStyles(formContainer, control);
			PopupFormStyles styles = control.Styles.PopupForm;
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			popup.ControlStyle.CopyFrom(styles.ControlStyle);
			popup.CloseButtonStyle.CopyFrom(styles.CloseButton);
			popup.HeaderStyle.CopyFrom(styles.Header);
			popup.ContentStyle.CopyFrom(styles.Content);
			popup.ModalBackgroundStyle.CopyFrom(styles.ModalBackground);
			popup.CloseButtonImage.CopyFrom(control.Images.FormCloseButton);
		}
		protected internal override void PrepareFormContainerScripts(Control formContainer, SchedulerFormTemplateContainer templateContainer) {
			base.PrepareFormContainerScripts(formContainer, templateContainer);
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			popup.ClientSideEvents.Init = String.Format("function(s,e){{ ASPx.ShowFormPopupWindow('{0}','{1}'); }}", SchedulerClientId, popup.ID);
			popup.ClientSideEvents.CloseUp = String.Format("function(s,e){{ {0}; }}", templateContainer.CancelScript);
		}
	}
	#endregion
	#region PopupInplaceEditorContainerBuilder
	public class PopupInplaceEditorContainerBuilder : PopupFormBaseContainerBuilder {
		public PopupInplaceEditorContainerBuilder(string schedulerClientId)
			: base(schedulerClientId) {
		}
		protected override ASPxSchedulerPopupForm CreatePopupForm(ASPxScheduler control) {
			return new ASPxSchedulerInplacePopupForm(control);
		}
		protected internal override void InitializeFormContainerStyles(Control formContainer, ASPxScheduler control) {
			base.InitializeFormContainerStyles(formContainer, control);
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			popup.ContentStyle.CopyFrom(control.Styles.GetInplaceEditorStyle());
		}
		protected internal override void PrepareFormContainerScripts(Control formContainer, SchedulerFormTemplateContainer templateContainer) {
			base.PrepareFormContainerScripts(formContainer, templateContainer);
			ASPxSchedulerPopupForm popup = (ASPxSchedulerPopupForm)formContainer;
			AppointmentInplaceEditorTemplateContainer inplaceContainer = (AppointmentInplaceEditorTemplateContainer)templateContainer;
			string aptId = GetEditedAppointmentId(inplaceContainer);
			popup.ClientSideEvents.Init = String.Format("function(s,e){{ ASPx.ShowInplacePopupWindow('{0}','{1}', '{2}'); }}", SchedulerClientId, popup.ID, aptId);
			popup.ClientSideEvents.CloseUp = String.Format("function(s,e){{ {0}; }}", templateContainer.CancelScript);
		}
		private string GetEditedAppointmentId(AppointmentInplaceEditorTemplateContainer templateContainer) {
			Appointment apt = templateContainer.Appointment;
			return templateContainer.Control.GetAppointmentClientId(apt);
		}
	}
	#endregion
	#region ControlAreaContainerBuilder
	public class ControlAreaContainerBuilder : FormContainerBuilder {
		public ControlAreaContainerBuilder(string schedulerClientId)
			: base(schedulerClientId) {
		}
		protected internal override Control CreateFormContainer(ASPxScheduler control, Control parent, string id) {
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			div.ID = id;
			parent.Controls.Add(div);
			return div;
		}
		protected internal override void AddTemplateContainer(Control container, SchedulerFormTemplateContainer child) {
			container.Controls.Add(child);
		}
		protected internal override void InitializeFormContainerStyles(Control formContainer, ASPxScheduler control) {
			base.InitializeFormContainerStyles(formContainer, control);
			WebControl container = (WebControl)formContainer;
			container.ControlStyle.CopyFrom(control.Styles.GetControlAreaFormControlStyle());
		}
	}
	#endregion
	#region FormRenderer
	public abstract class FormRenderer {
		#region CreateInstance
		public static FormRenderer CreateInstance(SchedulerFormType type, ASPxScheduler control) {
			switch (type) {
				case SchedulerFormType.Appointment:
					return new AppointmentFormRenderer(control);
				case SchedulerFormType.AppointmentInplace:
					return new AppointmentInplaceEditorRenderer(control);
				case SchedulerFormType.GotoDate:
					return new GotoDateFormRenderer(control);
				case SchedulerFormType.RecurrentAppointmentDelete:
					return new RecurrentAppointmentDeleteFormRenderer(control);
				case SchedulerFormType.RecurrentAppointmentEdit:
					return new RecurrentAppointmentEditFormRenderer(control);
				case SchedulerFormType.Reminder:
					return new RemindersFormRenderer(control);
			}
			return null;
		}
		#endregion
		bool abortExecution;
		ASPxScheduler control;
		protected FormRenderer(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		protected internal ASPxScheduler Control { get { return control; } }
		protected internal abstract string FormTemplateUrl { get; }
		protected internal abstract string FormName { get; }
		protected internal abstract string FormContainerId { get; }
		public abstract SchedulerFormVisibility FormContainerVisibility { get; }
		#endregion
		protected internal abstract SchedulerFormTemplateContainer CreateFormTemplateContainer();
		protected internal abstract SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container);
		protected internal abstract void RaiseFormShowingEvent(SchedulerFormEventArgs args);
		protected internal abstract void BeforeExecute();
		protected internal abstract bool CanContinueExecute();
		#region Execute
		public virtual void Execute(Control parent) {
			XtraSchedulerDebug.Assert(FormContainerVisibility != SchedulerFormVisibility.None);
			BeforeExecute();
			if (CanContinueExecute())   
				ExecuteCore(parent);
			AfterExecute();
		}
		protected internal virtual void ExecuteCore(Control root) {
			WebControl parent = RenderUtils.CreateDiv();
			root.Controls.Add(parent);
			SchedulerFormTemplateContainer defaultContainer = CreateFormTemplateContainer();
			string formUrl = FormTemplateUrl;
			SchedulerFormEventArgs args = CreateFormShowingEventArgs(defaultContainer);
			args.FormTemplateUrl = formUrl;
			RaiseFormShowingEvent(args);
			if (args.Cancel) {
				this.abortExecution = true;
				return;
			}
			Control formControl = CreateFormControl(args.FormTemplateUrl);
			if (formControl == null) {
				this.abortExecution = true;
				return;
			}
			SchedulerFormTemplateContainer container = ValidateTemplateContainer(args.Container, defaultContainer);
			FormContainerBuilder formBuilder = CreateFormContainerBuilder(FormContainerVisibility, Control.ClientID);
			PrepareFormContainerBuilder(formBuilder, container);
			formBuilder.BuildFormContainer(Control, parent, FormContainerId, container);
			RaisePreparePopupContainer(formBuilder);
			SchedulerUserControl.PrepareUserControl(formControl, container, FormName);
			container.DataBind();
			AttachEnterSupport(formControl, parent);
			Control.FormBlock.CurrentFormTemplateContainer = container;
		}
		void AttachEnterSupport(Control formControl, WebControl parentDiv) {
			ASPxSchedulerFormWithClientScriptSupportBase clientForm = formControl as ASPxSchedulerFormWithClientScriptSupportBase;
			if (clientForm != null) {
				WebControl button = clientForm.ObtainDefaultButton();
				if (button != null)
					parentDiv.Attributes.Add("onkeypress", string.Format(ASPxScheduler.SchedulerFireDefaultButtonHandlerName, button.ClientID));
			}
		}
		Control CreateFormControl(string formUrl) {
			return IsCreateUserControl(formUrl) ? CreateUserControl(formUrl) : CreateDefaultFormControl();
		}
		protected virtual bool IsCreateUserControl(string formUrl) {
			return !String.IsNullOrEmpty(formUrl);
		}
		protected abstract Control CreateDefaultFormControl();
		protected virtual Control CreateUserControl(string formUrl) {
			if (control.Page == null)
				return null;
			return control.Page.LoadControl(formUrl);
		}
		protected abstract void RaisePreparePopupContainer(FormContainerBuilder builder);
		protected internal virtual void AfterExecute() {
			if (this.abortExecution) {
				Control.ActiveFormType = SchedulerFormType.None;
				Control.ProtectedResetControlHierarchy();
			}
		}
		#endregion
		#region CreateFormContainerManager
		protected internal virtual FormContainerBuilder CreateFormContainerBuilder(SchedulerFormVisibility visibility, string schedulerClientId) {
			switch (visibility) {
				case SchedulerFormVisibility.None:
					return null;
				case SchedulerFormVisibility.PopupWindow:
					return new PopupFormContainerBuilder(schedulerClientId);
				case SchedulerFormVisibility.FillControlArea:
					return new ControlAreaContainerBuilder(schedulerClientId);
			}
			return null;
		}
		#endregion
		protected internal SchedulerFormTemplateContainer ValidateTemplateContainer(SchedulerFormTemplateContainer container, SchedulerFormTemplateContainer defaultContainer) {
			if (container != null && String.IsNullOrEmpty(container.ID)) {
				container.ID = SchedulerIdHelper.GenerateCustomContainerId(FormContainerId);
			}
			return container != null ? container : defaultContainer;
		}
		protected internal virtual void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
		}
		#region FindTemplateControl
		protected internal virtual TemplateControl FindTemplateControl(SchedulerFormTemplateContainer container) {
			return (TemplateControl)container.Controls[0];
		}
		#endregion
	}
	#endregion
	#region AppointmentFormRendererBase
	public abstract class AppointmentFormRendererBase : FormRenderer {
		#region Fields
		Appointment editedAppointment;
		#endregion
		protected AppointmentFormRendererBase(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public Appointment EditedAppointment {
			get {
				if (editedAppointment == null)
					editedAppointment = GetEditedAppointment();
				if (editedAppointment == null) {
					editedAppointment = CreateNewAppointment();
				}
				return editedAppointment;
			}
		}
		public IDefaultUserData UserData { get { return GetUserData(); } }
		#endregion
		protected internal virtual Appointment CreateNewAppointment() {
			Appointment apt = Control.Storage.Appointments.CreateAppointment(AppointmentType.Normal);
			apt.SetId(SchedulerIdHelper.NewAppointmentId);
			return apt;
		}
		#region GetEditedAppointment
		protected internal virtual Appointment GetEditedAppointment() {
			return Control.GetEditedAppointment();
		}
		#endregion
		protected internal virtual IDefaultUserData GetUserData() {
			return Control.GetDefaultUserData();
		}
		protected internal override void BeforeExecute() {
		}
		protected internal override bool CanContinueExecute() {
			return EditedAppointment != null;
		}
	}
	#endregion
	#region AppointmentFormRenderer
	public class AppointmentFormRenderer : AppointmentFormRendererBase {
		internal const int FormDefaultWidth = 675;
		internal const int FormDefaultHeight = 300;
		public AppointmentFormRenderer(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.AppointmentFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.AppointmentForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.AppointmentFormContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return Control.OptionsForms.AppointmentFormVisibility; } }
		#endregion
		#region CreateFormTemplateContainer
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			AppointmentFormTemplateContainer result = new AppointmentFormTemplateContainer(Control);
			return result;
		}
		#endregion
		protected internal override void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
			base.PrepareFormContainerBuilder(formBuilder, container);
			PopupFormContainerBuilder builder = formBuilder as PopupFormContainerBuilder;
			if (builder != null) {
				builder.DefaultWidth = FormDefaultWidth;
				builder.DefaultHeight = FormDefaultHeight;
				builder.HeaderText = (String.IsNullOrEmpty(container.Caption)) ? SchedulerUtils.FormatAppointmentFormCaption(EditedAppointment.AllDay, EditedAppointment.Subject, false) : container.Caption;
			}
		}
		#region CreateFormShowingEventArgs
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			AppointmentFormTemplateContainer formContainer = (AppointmentFormTemplateContainer)container;
			return new AppointmentFormEventArgs(formContainer, formContainer.Appointment, CalculateFormAction(formContainer.IsNewAppointment));
		}
		#endregion
		#region RaiseFormShowingEvent
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseAppointmentFormShowing((AppointmentFormEventArgs)args);
		}
		#endregion
		#region CalculateFormAction
		protected SchedulerFormAction CalculateFormAction(bool isNewAppointment) {
			return isNewAppointment ? SchedulerFormAction.Create : SchedulerFormAction.Edit;
		}
		#endregion
		#region CanContinueExecute
		protected internal override bool CanContinueExecute() {
			return true;
		}
		#endregion
		#region RaisePreparePopupContainer
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareAppointmentFormPopupContainer(popupBuilder.PopupControl);
		}
		#endregion
		protected override Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentForm();
		}
	}
	#endregion
	#region AppointmentInplaceEditorRenderer
	public class AppointmentInplaceEditorRenderer : AppointmentFormRendererBase {
		public AppointmentInplaceEditorRenderer(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.AppointmentInplaceEditorFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.InplaceEditorForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.AppointmentInplaceEditorContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return SchedulerFormVisibility.PopupWindow; } }
		#endregion
		#region CreateFormTemplateContainer
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			AppointmentInplaceEditorTemplateContainer result = new AppointmentInplaceEditorTemplateContainer(Control);
			result.ID = SchedulerIdHelper.AppointmentEditorTemplateContainerId;
			return result;
		}
		#endregion
		#region CreateFormShowingEventArgs
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			AppointmentInplaceEditorTemplateContainer formContainer = (AppointmentInplaceEditorTemplateContainer)container;
			return new AppointmentInplaceEditorEventArgs(formContainer, formContainer.Appointment, CalculateFormAction(formContainer.IsNewAppointment));
		}
		#endregion
		#region RaiseFormShowingEvent
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseAppointmentInplaceEditorFormShowing((AppointmentInplaceEditorEventArgs)args);
		}
		#endregion
		#region CreateFormContainerManager
		protected internal override FormContainerBuilder CreateFormContainerBuilder(SchedulerFormVisibility visibility, string schedulerClientId) {
			return new PopupInplaceEditorContainerBuilder(schedulerClientId);
		}
		#endregion
		#region CalculateFormAction
		protected SchedulerFormAction CalculateFormAction(bool isNewAppointment) {
			return isNewAppointment ? SchedulerFormAction.Create : SchedulerFormAction.Edit;
		}
		#endregion
		#region RaisePreparePopupContainer
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareAppointmentInplaceEditorPopupContainer(popupBuilder.PopupControl);
		}
		#endregion
		protected override Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.InplaceEditor();
		}
	}
	#endregion
	#region GotoDateFormRenderer
	public class GotoDateFormRenderer : FormRenderer {
		internal const int FormDefaultWidth = 150;
		internal const int FormDefaultHeight = 75;
		public GotoDateFormRenderer(ASPxScheduler control)
			: base(control) {
		}
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.GotoDateFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.GotoDateForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.GotoDateFormContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return Control.OptionsForms.GotoDateFormVisibility; } }
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			GotoDateFormTemplateContainer result = new GotoDateFormTemplateContainer(Control);
			result.ID = SchedulerIdHelper.GotoDateFormTemplateContainerId;
			return result;
		}
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			return new GotoDateFormEventArgs((GotoDateFormTemplateContainer)container);
		}
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseGotoDateFormShowing((GotoDateFormEventArgs)args);
		}
		protected internal override void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
			base.PrepareFormContainerBuilder(formBuilder, container);
			PopupFormContainerBuilder builder = formBuilder as PopupFormContainerBuilder;
			if (builder != null) {
				builder.DefaultWidth = FormDefaultWidth;
				builder.DefaultHeight = FormDefaultHeight;
				builder.HeaderText = (String.IsNullOrEmpty(container.Caption)) ? ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_GotoDate) : container.Caption;
			}
		}
		protected internal override void BeforeExecute() {
		}
		protected internal override bool CanContinueExecute() {
			return true;
		}
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareGotoDateFormPopupContainer(popupBuilder.PopupControl);
		}
		protected override Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.GotoDateForm();
		}
	}
	#endregion
	#region RecurrentAppointmentDeleteFormRenderer
	public class RecurrentAppointmentDeleteFormRenderer : AppointmentFormRendererBase {
		internal const int FormDefaultWidth = 350;
		internal const int FormDefaultHeight = 150;
		public RecurrentAppointmentDeleteFormRenderer(ASPxScheduler control)
			: base(control) {
		}
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.RecurrentAppointmentDeleteFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.RecurrentAppointmentDeleteForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.RecurrentAppointmentDeleteFormContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return Control.OptionsForms.RecurrentAppointmentDeleteFormVisibility; } }
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			RecurrentAppointmentDeleteFormTemplateContainer result = new RecurrentAppointmentDeleteFormTemplateContainer(Control, EditedAppointment);
			result.ID = SchedulerIdHelper.RecurrentAppointmentDeleteFormTemplateContainerId;
			return result;
		}
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			return new RecurrentAppointmentDeleteFormEventArgs((RecurrentAppointmentDeleteFormTemplateContainer)container);
		}
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseRecurrentAppointmentDeleteFormShowing((RecurrentAppointmentDeleteFormEventArgs)args);
		}
		protected internal override void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
			base.PrepareFormContainerBuilder(formBuilder, container);
			PopupFormContainerBuilder builder = formBuilder as PopupFormContainerBuilder;
			if (builder != null) {
				builder.DefaultWidth = FormDefaultWidth;
				builder.DefaultHeight = FormDefaultHeight;
				builder.HeaderText = (String.IsNullOrEmpty(container.Caption)) ? ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_DeleteRecurrentApt) : container.Caption;
			}
		}
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareRecurrenceAppointmentDeleteFormPopupContainer(popupBuilder.PopupControl);
		}
		protected override Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.RecurrentAppointmentDeleteForm();
		}
	}
	#endregion
	#region RecurrentAppointmentEditFormRenderer
	public class RecurrentAppointmentEditFormRenderer : AppointmentFormRendererBase {
		internal const int FormDefaultWidth = 350;
		internal const int FormDefaultHeight = 150;
		public RecurrentAppointmentEditFormRenderer(ASPxScheduler control)
			: base(control) {
		}
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.RecurrentAppointmentEditFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.RecurrentAppointmentEditForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.RecurrentAppointmentEditFormContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return Control.OptionsForms.RecurrentAppointmentEditFormVisibility; } }
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			RecurrentAppointmentEditFormTemplateContainer result = new RecurrentAppointmentEditFormTemplateContainer(Control, EditedAppointment);
			result.ID = SchedulerIdHelper.RecurrentAppointmentEditFormTemplateContainerId;
			return result;
		}
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			return new RecurrentAppointmentEditFormEventArgs((RecurrentAppointmentEditFormTemplateContainer)container);
		}
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseRecurrentAppointmentEditFormShowing((RecurrentAppointmentEditFormEventArgs)args);
		}
		protected internal override void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
			base.PrepareFormContainerBuilder(formBuilder, container);
			PopupFormContainerBuilder builder = formBuilder as PopupFormContainerBuilder;
			if (builder != null) {
				builder.DefaultWidth = FormDefaultWidth;
				builder.DefaultHeight = FormDefaultHeight;
				builder.HeaderText = (String.IsNullOrEmpty(container.Caption)) ? ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_OpenRecurrentApt) : container.Caption;
			}
		}
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareRecurrenceAppointmentEditFormPopupContainer(popupBuilder.PopupControl);
		}
		protected override Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.RecurrentAppointmentEditForm();
		}
	}
	#endregion
}
