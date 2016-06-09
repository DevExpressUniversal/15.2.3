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
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Localization;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
public partial class GotoDateForm : SchedulerFormControl {
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		Localize();
		edtDate.Focus();
	}
	void Localize() {
		lblDate.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Date);
		lblView.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ShowIn);
		btnOk.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonOk);
		btnCancel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel);
	}
	public override void DataBind() {
		base.DataBind();
		GotoDateFormTemplateContainer container = (GotoDateFormTemplateContainer)Parent;
		cbView.Value = container.ActiveViewType.ToString();
		btnOk.ClientSideEvents.Click = container.ApplyHandler;
		btnCancel.ClientSideEvents.Click = container.CancelHandler;
		ASPxScheduler scheduler = container.Control;
		string actualSchedulerInstanceName = scheduler.ClientInstanceName;
		if(String.IsNullOrEmpty(actualSchedulerInstanceName))
			actualSchedulerInstanceName = scheduler.ClientID;
		edtDate.ClientSideEvents.LostFocus = String.Format(@"function(s,e) {{
                var date = s.GetDate();
                if (date == null || date == false) {{
                    var selectionInterval = {0}.GetSelectedInterval();
                    var startDate = selectionInterval.GetStart();
                    s.SetDate(startDate);
                }}
            }}", actualSchedulerInstanceName);
	}
	protected override ASPxEditBase[] GetChildEditors() {
		ASPxEditBase[] edits = new ASPxEditBase[] {
			lblDate, edtDate,
			lblView, cbView
		};
		return edits;
	}
	protected override ASPxButton[] GetChildButtons() {
		ASPxButton[] buttons = new ASPxButton[] {
			btnOk, btnCancel
		};
		return buttons;
	}
	protected override WebControl GetDefaultButton() {
		return btnOk;
	}
}
}
