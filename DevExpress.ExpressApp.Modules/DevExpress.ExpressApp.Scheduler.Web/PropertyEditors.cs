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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public abstract class ASPxSchedulerPropertyEditor<T> : ASPxPropertyEditor, ITestable where T : IUserInterfaceObject {
		private ASPxScheduler scheduler;
		private void dataSource_CollectionChanged(object sender, CollectionChangedEventArgs<T> e) {
			FillControlValues();
		}
		private void FillControlValues() {
			if(Editor != null) {
				Editor.Items.Clear();
				Editor.ValueType = typeof(System.String);
				for(int i = 0; i < DataSource.Count; i++) {
					Editor.Items.Add(DataSource[i].DisplayName, i);
				}
			}
		}
		public ASPxScheduler Scheduler {
			get { return scheduler; }
		}
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if(control is ASPxComboBox) {
				ASPxComboBox editor = (ASPxComboBox)control;
				editor.SelectedIndexChanged += new EventHandler(EditValueChangedHandler);
				FillControlValues();
			}
		}
		protected override void SetImmediatePostDataCompanionScript(string script) {
			((ASPxComboBox)Editor).ClientSideEvents.GotFocus = script;
		}
		protected override object GetControlValueCore() {
			return Editor.SelectedIndex;
		}
		protected override void ReadEditModeValueCore() {
			object value = PropertyValue;
			if(value is int) {
				Editor.SelectedIndex = (int)value;
			}
		}
		protected override string GetPropertyDisplayValue() {
			object value = PropertyValue;
			if(value is int) {
				return DataSource[(int)value].DisplayName;
			}
			return string.Empty;
		}
		protected override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxComboBoxTestControl();
		}
		protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore() {
			return RenderHelper.CreateASPxComboBox();
		}
		protected override void Dispose(bool disposing) {
			if(DataSource != null) {
				DataSource.CollectionChanged -= new CollectionChangedEventHandler<T>(dataSource_CollectionChanged);
			}
			base.Dispose(disposing);
		}
		public abstract UserInterfaceObjectCollection<T> DataSource {
			get;
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.SelectedIndexChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public ASPxSchedulerPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			scheduler = new ASPxScheduler();
			DataSource.CollectionChanged += new CollectionChangedEventHandler<T>(dataSource_CollectionChanged);
		}
		public new ASPxComboBox Editor {
			get { return (ASPxComboBox)base.Editor; }
		}
	}
	public class ASPxSchedulerLabelPropertyEditor : ASPxSchedulerPropertyEditor<IAppointmentLabel> {
		public override UserInterfaceObjectCollection<IAppointmentLabel> DataSource {
			get {
				if(Scheduler != null && Scheduler.Storage != null && Scheduler.Storage.Appointments != null) {
					return Scheduler.Storage.Appointments.Labels;
				}
				return null;
			}
		}
		public ASPxSchedulerLabelPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
		}
	}
	public class ASPxSchedulerStatusPropertyEditor : ASPxSchedulerPropertyEditor<IAppointmentStatus> {
		public override UserInterfaceObjectCollection<IAppointmentStatus> DataSource {
			get {
				if(Scheduler != null && Scheduler.Storage != null && Scheduler.Storage.Appointments != null) {
					return Scheduler.Storage.Appointments.Statuses;
				}
				return null;
			}
		}
		public ASPxSchedulerStatusPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
		}
	}
	public class ASPxSchedulerRecurrenceInfoPropertyEditor : ASPxPropertyEditor, ITestable, ITestableContainer {
		private SchedulerRecurrenceInfoPropertyEditorHelper recurrenceInfoHelper;
		private ASPxSchedulerRecurrenceInfoEdit recurrenceInfoEdit;
		private ASPxLabel recurrenceInfoView;
		public ASPxSchedulerRecurrenceInfoPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			recurrenceInfoHelper = new SchedulerRecurrenceInfoPropertyEditorHelper(this);
			recurrenceInfoHelper.RecurrenceInfoChanged += new EventHandler(recurrenceInfoHelper_RecurrenceInfoChanged);
			ControlInitialized += new EventHandler<ControlInitializedEventArgs>(ASPxSchedulerRecurrenceInfoPropertyEditor_ControlInitialized);
		}
		void ASPxSchedulerRecurrenceInfoPropertyEditor_ControlInitialized(object sender, ControlInitializedEventArgs e) {
			if(TestScriptsManager.EasyTestEnabled) {
				foreach(ITestable control in ((ITestableContainer)this).GetTestableControls()) {
					if(control is TestableContolWrapper) {
						((TestableContolWrapper)control).OnControlInitialized();
					}
				}
			}
		}
		private void recurrenceInfoHelper_RecurrenceInfoChanged(object sender, EventArgs e) {
			OnControlValueChanged();
		}
		protected override void SetImmediatePostDataScript(string script) {
			if(Editor != null) {
				((ASPxSchedulerRecurrenceInfoEdit)Editor).ImmediatePostDataScriptBody = script;
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			this.recurrenceInfoEdit = new ASPxSchedulerRecurrenceInfoEdit(recurrenceInfoHelper);
			return recurrenceInfoEdit;
		}
		protected override WebControl CreateViewModeControlCore() {
			this.recurrenceInfoView = RenderHelper.CreateASPxLabel();
			return recurrenceInfoView;
		}
		protected override void ReadValueCore() {
			string recurrenceInfoDescription;
			recurrenceInfoHelper.TryGetRecurrenceInfoDescription(out recurrenceInfoDescription);
			if(ViewEditMode == ViewEditMode.Edit) {
				recurrenceInfoEdit.ButtonEditText = recurrenceInfoDescription;
			}
			else {
				recurrenceInfoView.Text = recurrenceInfoDescription;
			}
		}
		#region ITestable Members
		string ITestable.TestCaption {
			get { return this.Caption; }
		}
		string ITestable.ClientId {
			get {
				if(ViewEditMode == ViewEditMode.Edit) {
					return recurrenceInfoEdit != null ? recurrenceInfoEdit.ButtonEdit.ClientID : String.Empty;
				}
				else {
					return recurrenceInfoView != null ? recurrenceInfoView.ClientID : String.Empty;
				}
			}
		}
		IJScriptTestControl ITestable.TestControl {
			get {
				if(ViewEditMode == ViewEditMode.Edit) {
					return new JSASPxSchedulerRecurrenceInfoEditTestControl();
				}
				else {
					return new JSLabelTestControl();
				}
			}
		}
		#endregion
		#region ITestableContainer Members
		private ITestable[] editControls = null;
		ITestable[] ITestableContainer.GetTestableControls() {
			ITestable[] controls = new ITestable[] { };
			if(ViewEditMode == ViewEditMode.Edit && recurrenceInfoEdit != null) {
				if(editControls == null) {
					editControls = new ITestable[] {
						new TestableContolWrapper("Occurrence Count", recurrenceInfoEdit.RecurrenceControl.FindControl("RangeCtl").FindControl("SpnOccCnt").ClientID, new JSASPxSpinTestControl(), TestControlType.Field),
						new TestableContolWrapper("End After", recurrenceInfoEdit.RecurrenceControl.FindControl("RangeCtl").FindControl("DeEndByOccNo").ClientID, new JSASPxRadioButtonTestControl(), TestControlType.Field),
						new TestableContolWrapper("End By", recurrenceInfoEdit.RecurrenceControl.FindControl("RangeCtl").FindControl("DeEndByDate").ClientID, new JSASPxRadioButtonTestControl(), TestControlType.Field),
						new TestableContolWrapper("End By Date", recurrenceInfoEdit.RecurrenceControl.FindControl("RangeCtl").FindControl("DeEnd").ClientID, new JSASPxDateTestControl(), TestControlType.Field)
					};
				}
				controls = editControls;
			}
			return controls;
		}
#pragma warning disable 0067
		public event EventHandler TestableControlsCreated;
#pragma warning restore 0067
		#endregion
	}
	public class JSASPxSchedulerRecurrenceInfoEditTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxSchedulerRecurrenceInfoEdit";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.IsEnabledFunctionBody = @"
return !this.control.inputElement.readOnly;
				";
				result.SetTextFunctionBody = @"
this.control.SetValue(value);
				";
				result.GetTextFunctionBody = @"
return this.control.GetValue();
				";
				result.ActFunctionBody = @"
if(!value) {
    if(!this.control.clientEnabled) {
        this.LogOperationError('Unable to execute actions in disabled control: ' + this.caption);					
    }
    else if(this.control.GetButton(0).isDisabled) {
        this.LogOperationError('Unable to execute Edit action in control: ' + this.caption);
    }
    else {
        ASPx.BEClick(this.control.name, 0);
    }		   
}
else {	
    if(!recurrencePopupControl.GetVisible()) { 
        this.LogOperationError('Unable to click ' + value + ' button if recurrence edit popup is not visible');					
    }
    for(var text in recurrencePopupControl.cpButtonTextToButtonIdMap) {
        if(value == text) {
            window[recurrencePopupControl.cpButtonTextToButtonIdMap[text]].DoClick();
            return;
        }
    }
    this.LogOperationError('Button ' + value + ' is not found for editor ' + this.caption);
}
";
				return result;
			}
		}
		#endregion
	}
	public class ASPxSchedulerRecurrenceInfoEdit : Panel, INamingContainer {
		private SchedulerRecurrenceInfoPropertyEditorHelper recurrenceInfoHelper;
		private ASPxButtonEdit buttonEdit;
		private ASPxPopupControl popupControl;
		private ASPxButton okButton;
		private ASPxButton cancelButton;
		private ASPxButton removeRecurrenceButton;
		private ASPxCallbackPanel callbackPanel;
		private AppointmentRecurrenceControl recurrenceControl;
		internal ASPxButtonEdit ButtonEdit {
			get {
				EnsureChildControls();
				return buttonEdit; 
			}
		}
		public AppointmentRecurrenceControl RecurrenceControl {
			get { return recurrenceControl; }
		}
		public ASPxSchedulerRecurrenceInfoEdit(SchedulerRecurrenceInfoPropertyEditorHelper recurrenceInfoHelper) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(recurrenceInfoHelper, "recurrenceInfoHelper");
			this.recurrenceInfoHelper = recurrenceInfoHelper;
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			callbackPanel = new ASPxCallbackPanel();
			callbackPanel.ID = "CP";
			callbackPanel.ClientInstanceName = "recurrenceInfoCallbackPanel";
			callbackPanel.Callback += new CallbackEventHandlerBase(callbackPanel_Callback);
			callbackPanel.Unload += new EventHandler(callbackPanel_Unload);
			Controls.Add(callbackPanel);
			buttonEdit = new ASPxButtonEdit();
			buttonEdit.ID = "BE";
			buttonEdit.Width = Unit.Percentage(100);
			buttonEdit.ReadOnly = true;
			RenderHelper.SetupASPxWebControl(buttonEdit);
			buttonEdit.ClientEnabled = true;
			buttonEdit.ClientSideEvents.ButtonClick = string.Format(@"function(s, e) {{ {0}.PerformCallback(); }}", callbackPanel.ClientInstanceName);
			EditButton editButton = buttonEdit.Buttons.Add();
			editButton.Text = "...";
			callbackPanel.Controls.Add(buttonEdit);
			recurrenceControl = CreateRecurrenceControl();
			CreateRecurrencePopupControl(recurrenceControl);
			callbackPanel.Controls.Add(popupControl);
			if(recurrenceInfoHelper.IsEnabled) {
				AssignRecurrenceInfoToControl(recurrenceControl);
			}
			else {
				buttonEdit.ClientEnabled = false;
				buttonEdit.Buttons[0].Enabled = false;
			}
		}
		private void callbackPanel_Unload(object sender, EventArgs e) {
			if (callbackPanel != null) {
				callbackPanel.Callback -= new CallbackEventHandlerBase(callbackPanel_Callback);
				callbackPanel.Unload -= new EventHandler(callbackPanel_Unload);
			}
		}
		private void callbackPanel_Callback(object sender, CallbackEventArgsBase e) {
			if(String.IsNullOrEmpty(e.Parameter)) {
				string description;
				if(recurrenceInfoHelper.TryGetRecurrenceInfoDescription(out description)) {
					ShowPopup();
				}
				buttonEdit.Text = description;
			}
			else {
				IRecurrentEvent recurrentEvent = recurrenceInfoHelper.OwnerEvent;
				RecurrenceResult recurrenceResult = (RecurrenceResult)Enum.Parse(typeof(RecurrenceResult), e.Parameter);
				recurrenceInfoHelper.ProcessResult(recurrenceResult, GetRecurrenceInfoFromControl(), recurrentEvent.StartOn, recurrentEvent.EndOn, recurrentEvent.AllDay);
			}
		}
		private void ShowPopup() {
			AssignRecurrenceInfoToControl(recurrenceControl);
			popupControl.ShowOnPageLoad = true;
		}
		private RecurrenceInfo GetRecurrenceInfoFromControl() {
			RecurrenceInfo recurrenceInfo = new RecurrenceInfo();
			recurrenceInfo.BeginUpdate();
			recurrenceInfo.Type = recurrenceControl.ClientRecurrenceType;
			recurrenceInfo.DayNumber = recurrenceControl.ClientDayNumber;
			recurrenceInfo.Periodicity = recurrenceControl.ClientPeriodicity;
			recurrenceInfo.Month = recurrenceControl.ClientMonth;
			recurrenceInfo.WeekDays = recurrenceControl.ClientWeekDays;
			recurrenceInfo.WeekOfMonth = recurrenceControl.ClientWeekOfMonth;
			recurrenceInfo.OccurrenceCount = recurrenceControl.ClientOccurrenceCount;
			recurrenceInfo.Range = recurrenceControl.ClientRecurrenceRange;
			recurrenceInfo.Start = recurrenceControl.Start;
			recurrenceInfo.End = recurrenceControl.ClientEnd;
			recurrenceInfo.EndUpdate();
			return recurrenceInfo;
		}
		private void AssignRecurrenceInfoToControl(AppointmentRecurrenceControl recurrenceControl) {
			DevExpress.XtraScheduler.IRecurrenceInfo recurrenceInfo = recurrenceInfoHelper.GetRecurrenceInfo();
			if(recurrenceInfo != null) {
				recurrenceControl.RecurrenceType = recurrenceInfo.Type;
				recurrenceControl.DayNumber = recurrenceInfo.DayNumber;
				recurrenceControl.Periodicity = recurrenceInfo.Periodicity;
				recurrenceControl.Month = recurrenceInfo.Month;
				recurrenceControl.WeekDays = recurrenceInfo.WeekDays;
				recurrenceControl.WeekOfMonth = recurrenceInfo.WeekOfMonth;
				recurrenceControl.OccurrenceCount = recurrenceInfo.OccurrenceCount;
				recurrenceControl.RecurrenceRange = recurrenceInfo.Range;
				recurrenceControl.Start = recurrenceInfo.Start;
				recurrenceControl.End = recurrenceInfo.End;
			}
			else {
				recurrenceControl.Start = recurrenceInfoHelper.OwnerEvent.StartOn;
				recurrenceControl.End = recurrenceInfoHelper.OwnerEvent.EndOn;
			}
		}
		private AppointmentRecurrenceControl CreateRecurrenceControl() {
			AppointmentRecurrenceControl recurrenceControl = new AppointmentRecurrenceControl();
			recurrenceControl.ID = "recurrenceControl";
			recurrenceControl.EnableViewState = false;
			RenderHelper.SetupASPxWebControl(recurrenceControl);
			return recurrenceControl;
		}
		private void UpdateClientClickScript(ASPxButton button, ASPxPopupControl popupControl, RecurrenceResult result) {
			button.ClientSideEvents.Click = GetClickScript(popupControl, result);
		}
		private void CreateRecurrencePopupControl(AppointmentRecurrenceControl recurrenceControl) {
			popupControl = new ASPxPopupControl();
			popupControl.ClientInstanceName = "recurrencePopupControl";
			popupControl.Modal = true;
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popupControl.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popupControl.HeaderText = SchedulerAspNetModuleLocalizer.Active.GetLocalizedString(SchedulerAspNetModuleId.RecurrencePopupControlHeader);
			popupControl.CustomJSProperties += popupControl_CustomJSProperties;
			popupControl.AllowDragging = true;
			popupControl.EnableViewState = false;
			RenderHelper.SetupASPxWebControl(popupControl);
			popupControl.Controls.Add(recurrenceControl);
			Table table = RenderHelper.CreateTable();
			table.CellSpacing = 10;
			table.Style[HtmlTextWriterStyle.MarginTop] = "20px";
			TableRow row = new TableRow();
			TableCell cell1 = new TableCell();
			TableCell cell2 = new TableCell();
			TableCell cell3 = new TableCell();
			okButton = RenderHelper.CreateASPxButton();
			okButton.ID = "okButton";
			okButton.ClientInstanceName = "recurrenceControlOkButton";
			okButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "OK"); ;
			UpdateClientClickScript(okButton, popupControl, RecurrenceResult.OK);
			cell1.Controls.Add(okButton);
			cancelButton = RenderHelper.CreateASPxButton();
			cancelButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Cancel");
			UpdateClientClickScript(cancelButton, popupControl, RecurrenceResult.Cancel);
			cell2.Controls.Add(cancelButton);
			removeRecurrenceButton = RenderHelper.CreateASPxButton();
			removeRecurrenceButton.Text = SchedulerAspNetModuleLocalizer.Active.GetLocalizedString("RecurrencePopupControlRemoveRecurrence");
			UpdateClientClickScript(removeRecurrenceButton, popupControl, RecurrenceResult.RemoveRecurrence);
			cell3.Controls.Add(removeRecurrenceButton);
			row.Cells.Add(cell1);
			row.Cells.Add(cell2);
			row.Cells.Add(cell3);
			table.Rows.Add(row);
			popupControl.Controls.Add(table);
			recurrenceControl.ClientSideEvents.ValidationCompleted = string.Format("function(s,e) {{ {0}.SetEnabled(e.isValid);}}", GetButtonId(okButton));
			if(RecurrencePopupControlCreated != null) {
				RecurrencePopupControlCreated(this, new RecurrencePopupControlCreatedEventArgs(popupControl, recurrenceControl, table, okButton, cancelButton, removeRecurrenceButton));
			}
		}
		private void popupControl_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
			ASPxPopupControl popupControl = (ASPxPopupControl)sender ;
			Dictionary<string, string> buttonTextToButtonIdMap = new Dictionary<string, string>();
			buttonTextToButtonIdMap.Add(okButton.Text, GetButtonId(okButton));
			buttonTextToButtonIdMap.Add(cancelButton.Text, GetButtonId(cancelButton));
			buttonTextToButtonIdMap.Add(removeRecurrenceButton.Text, GetButtonId(removeRecurrenceButton));
			popupControl.JSProperties["cpButtonTextToButtonIdMap"] = buttonTextToButtonIdMap;
		}   
		private string GetButtonId(ASPxButton button) {
			return !string.IsNullOrEmpty(button.ClientInstanceName) ? button.ClientInstanceName : button.ClientID;
		}
		private string GetClickScript(ASPxPopupControl popupControl, RecurrenceResult result) {
			string script = 
				"function(s, e) { " + 
					popupControl.ClientInstanceName + ".Hide();" +
					callbackPanel.ClientInstanceName + ".PerformCallback('" + result.ToString() + "');";
			if (result != RecurrenceResult.Cancel && !string.IsNullOrEmpty(ImmediatePostDataScriptBody)) {
				script += "(" + ImmediatePostDataScriptBody + ")();";
			}
			script += " }";
			return script;
		}
		private string immediatePostDataScriptBody;
		public string ImmediatePostDataScriptBody { 
			get { return immediatePostDataScriptBody; }
			set {
				immediatePostDataScriptBody = value;
				if(okButton != null) {
					UpdateClientClickScript(okButton, popupControl, RecurrenceResult.OK);
				}
				if(removeRecurrenceButton != null) {
					UpdateClientClickScript(removeRecurrenceButton, popupControl, RecurrenceResult.RemoveRecurrence);
				}
			}
		}
		public ASPxCallbackPanel CallbackPanel {
			get { return callbackPanel; }
		}
		public string ButtonEditText {
			get { return buttonEdit.Text; }
			set {
				EnsureChildControls();
				buttonEdit.Text = value;
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(popupControl != null) {
				popupControl.CustomJSProperties -= popupControl_CustomJSProperties;
			}
		}
		public event EventHandler<RecurrencePopupControlCreatedEventArgs> RecurrencePopupControlCreated;
	}
	public class RecurrencePopupControlCreatedEventArgs : EventArgs {
		public RecurrencePopupControlCreatedEventArgs(ASPxPopupControl popupControl, AppointmentRecurrenceControl recurrenceControl, Table buttonsTable, ASPxButton okButton, ASPxButton cancelButton, ASPxButton removeRecurrenceButton) {
			this.PopupControl = popupControl;
			this.RecurrenceControl = recurrenceControl;
			this.ButtonsTable = buttonsTable;
			this.OKButton = okButton;
			this.CancelButton = cancelButton;
			this.RemoveRecurrenceButton = removeRecurrenceButton;
		}
		public ASPxPopupControl PopupControl { get; private set; }
		public AppointmentRecurrenceControl RecurrenceControl { get; private set; }
		public Table ButtonsTable { get; private set; }
		public ASPxButton OKButton { get; private set; }
		public ASPxButton CancelButton { get; private set; }
		public ASPxButton RemoveRecurrenceButton { get; private set; }
	}
}
