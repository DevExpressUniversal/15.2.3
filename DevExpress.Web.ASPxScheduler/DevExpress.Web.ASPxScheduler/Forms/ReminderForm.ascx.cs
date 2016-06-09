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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Localization;
namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
public partial class ReminderForm : SchedulerFormControl {
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		Localize();
	}
	void Localize() {
		btnDismissAll.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonDismissAll);
		btnDismiss.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonDismiss);
		lblClickSnooze.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_SnoozeInfo);
		btnSnooze.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonSnooze);
	}
	public override void DataBind() {
		base.DataBind();
		RemindersFormTemplateContainer container = (RemindersFormTemplateContainer)Parent;
		btnDismiss.ClientSideEvents.Click = container.DismissReminderHandler;
		btnDismissAll.ClientSideEvents.Click = container.DismissAllRemindersHandler;
		btnSnooze.ClientSideEvents.Click = container.SnoozeRemindersHandler;
		InitItemListBox(container);
		InitSnoozeCombo(container);
	}
	void InitItemListBox(RemindersFormTemplateContainer container) {
		ReminderCollection reminders = container.Reminders;
		int count = reminders.Count;
		for(int i = 0; i < count; i++) {
			Reminder reminder = reminders[i];
			ListEditItem item = new ListEditItem(reminder.Subject, i);
			lbItems.Items.Add(item);
		}
		lbItems.SelectedIndex = 0;
	}
	void InitSnoozeCombo(RemindersFormTemplateContainer container) {
		cbSnooze.Items.Clear();
		TimeSpan[] timeSpans = container.SnoozeTimeSpans;
		int count = timeSpans.Length;
		for(int i = 0; i < count; i++) {
			TimeSpan timeSpan = timeSpans[i];
			cbSnooze.Items.Add(new ListEditItem(container.ConvertSnoozeTimeSpanToString(timeSpan), timeSpan));
		}
		cbSnooze.SelectedIndex = 4;
	}
	protected override ASPxEditBase[] GetChildEditors() {
		ASPxEditBase[] edits = new ASPxEditBase[] {
			lbItems, lblClickSnooze, cbSnooze
		};
		return edits;
	}
	protected override ASPxButton[] GetChildButtons() {
		ASPxButton[] buttons = new ASPxButton[] {
			btnDismissAll, btnDismiss, btnSnooze
		};
		return buttons;
	}
}
}
