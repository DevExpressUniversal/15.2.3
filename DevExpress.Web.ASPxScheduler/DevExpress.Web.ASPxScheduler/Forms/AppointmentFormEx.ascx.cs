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
using System.Web.UI;
using DevExpress.XtraScheduler;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.Web.ASPxScheduler.Localization;
namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
public partial class AppointmentFormEx : SchedulerFormControl {
	AppointmentRecurrenceControl recurrenceControl;
	public bool CanShowReminders {
		get {
			return ((AppointmentFormTemplateContainer)Parent).Control.Storage.EnableReminders;
		}
	}
	public bool ResourceSharing {
		get {
			return ((AppointmentFormTemplateContainer)Parent).Control.Storage.ResourceSharing;
		}
	}
	public IEnumerable ResourceDataSource {
		get {
			return ((AppointmentFormTemplateContainer)Parent).ResourceDataSource;
		}
	}
	public AppointmentRecurrenceControl RecurrenceControl { get { return recurrenceControl; } }
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		Localize();
		RenderRecurrenceControl(false);
		tbSubject.Focus();
	}
	void Localize() {
		lblSubject.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Subject);
		lblLocation.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Location);
		lblLabel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Label);
		lblStartDate.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_StartTime);
		lblEndDate.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_EndTime);
		lblStatus.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Status);
		lblAllDay.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_AllDayEvent);
		lblResource.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Resource);
		if (CanShowReminders)
			lblReminder.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Reminder);
		btnOk.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonOk);
		btnCancel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel);
		btnDelete.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonDelete);
		chkRecurrence.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Recurrence);
	}
	protected override void Render(System.Web.UI.HtmlTextWriter writer) {
		AppointmentFormTemplateContainer container = (AppointmentFormTemplateContainer)Parent;
		ASPxScheduler control = container.Control;
		RecurrenceControl.EditorsInfo = new EditorsInfo(control, control.Styles.FormEditors, control.Images.FormEditors, control.Styles.Buttons);
		RecurrenceControl.Visible = chkRecurrence.Checked && chkRecurrence.Visible;
		base.Render(writer);
	}
	public override void DataBind() {
		base.DataBind();
		AppointmentFormTemplateContainer container = (AppointmentFormTemplateContainer)Parent;
		Appointment apt = container.Appointment;
		IAppointmentStorageBase appointmentStorage = container.Control.Storage.Appointments;
		IAppointmentLabel label = appointmentStorage.Labels.GetById(apt.LabelKey);
		IAppointmentStatus status = appointmentStorage.Statuses.GetById(apt.StatusKey);
		edtLabel.SelectedIndex = appointmentStorage.Labels.IndexOf(label);
		edtStatus.SelectedIndex = appointmentStorage.Statuses.IndexOf(status);
		PopulateResourceEditors(apt, container);
		chkRecurrence.Visible = container.ShouldShowRecurrence;
		if(container.Appointment.HasReminder) {
			cbReminder.Value = container.Appointment.Reminder.TimeBeforeStart.ToString();
			chkReminder.Checked = true;
		}
		else {
			cbReminder.ClientEnabled = false;
		}
		btnOk.ClientSideEvents.Click = container.SaveHandler;
		btnCancel.ClientSideEvents.Click = container.CancelHandler;
		btnDelete.ClientSideEvents.Click = container.DeleteHandler;
	}
	void PopulateResourceEditors(Appointment apt, AppointmentFormTemplateContainer container) {
		if(ResourceSharing) {
			ASPxListBox edtMultiResource = ddResource.FindControl("edtMultiResource") as ASPxListBox;
			if(edtMultiResource == null)
				return;
			SetListBoxSelectedValues(edtMultiResource, apt.ResourceIds);
			List<String> multiResourceString = GetListBoxSelectedItemsText(edtMultiResource);
			string stringResourceNone = SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone);
			ddResource.Value = stringResourceNone;
			if(multiResourceString.Count > 0)
				ddResource.Value = String.Join(", ", multiResourceString.ToArray());
			ddResource.JSProperties.Add("cp_Caption_ResourceNone", stringResourceNone);
		}
		else {
			if(!Object.Equals(apt.ResourceId, EmptyResourceId.Id))
				edtResource.Value = apt.ResourceId.ToString();
			else
				edtResource.Value = SchedulerIdHelper.EmptyResourceId;
		}
	}
	List<String> GetListBoxSelectedItemsText(ASPxListBox listBox) {
		List<String> result = new List<string>();
		foreach(ListEditItem editItem in listBox.Items) {
			if(editItem.Selected)
				result.Add(editItem.Text);
		}
		return result;
	}
	void SetListBoxSelectedValues(ASPxListBox listBox, IEnumerable values) {
		listBox.Value = null;
		foreach(object value in values) {
			ListEditItem item = listBox.Items.FindByValue(value.ToString());
			if(item != null)
				item.Selected = true;
		}
	} 
	protected override ASPxEditBase[] GetChildEditors() {
		ASPxEditBase[] edits = new ASPxEditBase[] {
			lblSubject, tbSubject,
			lblLocation, tbLocation,
			lblLabel, edtLabel,
			lblStartDate, edtStartDate,
			lblEndDate, edtEndDate,
			lblStatus, edtStatus,
			lblAllDay, chkAllDay,
			lblResource, edtResource,
			tbDescription, cbReminder,
			ddResource
		};
		return edits;
	}
	protected override ASPxButton[] GetChildButtons() {
		ASPxButton[] buttons = new ASPxButton[] {
			btnOk, btnCancel, btnDelete
		};
		return buttons;
	}
	protected void OnCallback(object sender, CallbackEventArgsBase e) {
		RenderRecurrenceControl(true);
	}
	protected void RenderRecurrenceControl(bool isRecurring) {
		if(this.recurrenceControl != null) {
			this.recurrenceControl.Visible = isRecurring;
			return;
		}
		AppointmentFormTemplateContainer container = (AppointmentFormTemplateContainer)Parent;
		this.recurrenceControl = new AppointmentRecurrenceControl();
		recurrenceControl.ID = "RecurrenceControl1";
		recurrenceControl.Visible = isRecurring;
		recurrenceControl.DayNumber = container.RecurrenceDayNumber;
		recurrenceControl.End = container.RecurrenceEnd;
		recurrenceControl.Month = container.RecurrenceMonth;
		recurrenceControl.OccurrenceCount = container.RecurrenceOccurrenceCount;
		recurrenceControl.Periodicity = container.RecurrencePeriodicity;
		recurrenceControl.RecurrenceRange = container.RecurrenceRange;
		recurrenceControl.Start = container.RecurrenceStart;
		recurrenceControl.WeekDays = container.RecurrenceWeekDays;
		recurrenceControl.WeekOfMonth = container.RecurrenceWeekOfMonth;
		recurrenceControl.RecurrenceType = container.RecurrenceType;
		recurrenceControl.IsFormRecreated = container.IsFormRecreated;
		RecurrencePanel.Controls.Add(recurrenceControl);
		recurrenceControl.EditorsInfo = new EditorsInfo(container.Control, container.Control.Styles.FormEditors, container.Control.Images.FormEditors, container.Control.Styles.Buttons);
	}
}
}
