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
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.UI;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerLabelPropertyEditor : DXPropertyEditor {
		public SchedulerLabelPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			ControlBindingProperty = "SelectedIndex";
		}
		protected override object CreateControlCore() {
			SchedulerStorage storage = new SchedulerStorage();
			AppointmentLabelEdit editor = new AppointmentLabelEdit();
			editor.Storage = storage;
			return editor;
		}
	}
	public class SchedulerStatusPropertyEditor : DXPropertyEditor {
		public SchedulerStatusPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			ControlBindingProperty = "SelectedIndex";
		}
		protected override object CreateControlCore() {
			SchedulerStorage storage = new SchedulerStorage();
			AppointmentStatusEdit editor = new AppointmentStatusEdit();
			editor.Storage = storage;
			return editor;
		}
	}
	public class SchedulerRecurrenceInfoPropertyEditor : DXPropertyEditor {
		private SchedulerRecurrenceInfoPropertyEditorHelper recurrenceInfoHelper;
		private bool enableRecurrenceChangeConfirmation = true;
		public SchedulerRecurrenceInfoPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			CanUpdateControlEnabled = true;
			recurrenceInfoHelper = new SchedulerRecurrenceInfoPropertyEditorHelper(this);
			recurrenceInfoHelper.RecurrenceInfoChanged += new EventHandler(recurrenceInfoHelper_RecurrenceInfoChanged);
		}
		private void recurrenceInfoHelper_RecurrenceInfoChanged(object sender, EventArgs e) {
			OnControlValueChanged();
		}
		private void buttonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			TryChangeRecurrenceInfo();
		}
		private void buttonEdit_DoubleClick(object sender, EventArgs e) {
			TryChangeRecurrenceInfo();
		}
		private RecurrenceResult ShowAppointmentRecurrenceForm(Appointment appointment) {
			Appointment sourceAppointment = appointment.Copy();
			DialogResult dialogResult = ShowAppointmentRecurrenceFormCore(appointment);
			if(!ConfirmRecurrenceChange(sourceAppointment, appointment)) {
				return RecurrenceResult.Cancel;
			}
			switch(dialogResult) {
				case DialogResult.OK: return RecurrenceResult.OK;
				case DialogResult.Cancel: return RecurrenceResult.Cancel;
				case DialogResult.Abort: return RecurrenceResult.RemoveRecurrence;
				default: return RecurrenceResult.Cancel;
			}
		}
		private bool ConfirmRecurrenceChange(Appointment sourceAppointment, Appointment newAppointment) {
			bool needConfirmation = enableRecurrenceChangeConfirmation &&
				recurrenceInfoHelper.OwnerEvent.Type == (int)AppointmentType.Pattern &&
				!sourceAppointment.RecurrenceInfo.Equals(newAppointment.RecurrenceInfo);
			return !needConfirmation || Messaging.DefaultMessaging.GetUserChoice(SchedulerModuleBaseLocalizer.Active.GetLocalizedString("RecurrenceChangingMessage"), SchedulerModuleBaseLocalizer.Active.GetLocalizedString("RecurrenceChangingHeader"), MessageBoxButtons.OKCancel) == DialogResult.OK;
		}
		protected new ButtonEdit Control {
			get { return (ButtonEdit)base.Control; }
		}
		protected virtual void TryChangeRecurrenceInfo() {
			if(!AllowEdit) { return; }
			try {
				Appointment appointment = recurrenceInfoHelper.CreateEditingAppointment();
				RecurrenceResult result = ShowAppointmentRecurrenceForm(appointment);
				recurrenceInfoHelper.ProcessResult(result, appointment.RecurrenceInfo, appointment.Start, appointment.End, appointment.AllDay);
			}
			catch(ArgumentException) {
				Control.Text = SchedulerModuleBaseLocalizer.Active.GetLocalizedString("ObjectNotValid"); ;
			}
		}
		protected virtual DialogResult ShowAppointmentRecurrenceFormCore(Appointment appointment) {
			AppointmentRecurrenceForm form = new AppointmentRecurrenceForm(appointment,
							FirstDayOfWeek.Monday, null);
			DialogResult dialogResult = form.ShowDialog();
			return dialogResult;
		}
		protected override object CreateControlCore() {
			ButtonEdit buttonEdit = new ButtonEdit();
			buttonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(buttonEdit_ButtonClick);
			buttonEdit.DoubleClick += new EventHandler(buttonEdit_DoubleClick);
			buttonEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			return buttonEdit;
		}
		private void UpdateAllowEdit() {
			AllowEdit["SchedulerRecurrenceInfoPropertyEditorHelper.IsEnabled"] = recurrenceInfoHelper.IsEnabled;
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			UpdateAllowEdit();
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			UpdateAllowEdit();
		}
		protected override void ReadValueCore() {
			string description;
			recurrenceInfoHelper.TryGetRecurrenceInfoDescription(out description);
			Control.Text = description;
		}
		protected override void Dispose(bool disposing) {
			if (Control != null && disposing) {
				Control.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(buttonEdit_ButtonClick);
				Control.DoubleClick -= new EventHandler(buttonEdit_DoubleClick);
			}
			if (disposing && recurrenceInfoHelper != null) {
				recurrenceInfoHelper.Dispose();
				recurrenceInfoHelper = null;
			}
			base.Dispose(disposing);
		}
		public bool RecurrenceChangeConfirmationEnabled {
			get { return enableRecurrenceChangeConfirmation; }
			set { enableRecurrenceChangeConfirmation = value; }
		}
#if DebugTest
		public SchedulerRecurrenceInfoPropertyEditorHelper DebugTest_RecurrenceInfoHelper {
			get { return recurrenceInfoHelper; }
		}
#endif
	}
}
