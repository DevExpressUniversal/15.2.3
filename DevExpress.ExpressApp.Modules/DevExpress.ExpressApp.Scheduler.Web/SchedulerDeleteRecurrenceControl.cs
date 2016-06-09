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
using System.Text;
using DevExpress.Web;
using DevExpress.ExpressApp.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web.ASPxScheduler;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class DeleteResultEventArgs : EventArgs {
		private DeleteResult deleteResult;
		public DeleteResultEventArgs(DeleteResult deleteResult) {
			this.deleteResult = deleteResult;
		}
		public DeleteResult DeleteResult {
			get { return deleteResult; }
		}
	}
	public class ASPxSchedulerDeleteRecurrenceControl : ASPxCallbackPanel {
		private const string radioButtonListId = "radioButtonList";
		private ASPxPopupControl popupControl;
		private ASPxLabel confirmationTextLabel;
		private ASPxScheduler scheduler;
		private ASPxPopupControl CreateDeleteRecurrencePopupControl() {
			ASPxPopupControl popupControl = new ASPxPopupControl();
			popupControl.ClientInstanceName = "deleteRecurrencePopupControl";
			popupControl.ID = "deleteRecurrencePopupControl";
			popupControl.Modal = true;
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popupControl.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popupControl.HeaderText = SchedulerAspNetModuleLocalizer.Active.GetLocalizedString(SchedulerAspNetModuleId.DeleteRecurrencePopupControlHeader);
			popupControl.AllowDragging = true;
			popupControl.EnableViewState = false;
			popupControl.Width = Unit.Pixel(100);
			popupControl.CloseAction = CloseAction.None;
			RenderHelper.SetupASPxWebControl(popupControl);
			confirmationTextLabel = RenderHelper.CreateASPxLabel();
			Panel panel = new Panel();
			panel.Style[System.Web.UI.HtmlTextWriterStyle.Margin] = "0px 0px 10px 10px";
			panel.Controls.Add(confirmationTextLabel);
			popupControl.Controls.Add(panel);
			ASPxRadioButtonList radioButtonList = new ASPxRadioButtonList();
			radioButtonList.Style[System.Web.UI.HtmlTextWriterStyle.Margin] = "0px 0px 5px 10px";
			radioButtonList.ID = "radioButtonList";
			radioButtonList.ClientInstanceName = radioButtonListId;
			radioButtonList.Items.Add(SchedulerAspNetModuleLocalizer.Active.GetLocalizedString(SchedulerAspNetModuleId.DeleteConfirmationOccurrence), DeleteResult.Occurrence);
			radioButtonList.Items.Add(SchedulerAspNetModuleLocalizer.Active.GetLocalizedString(SchedulerAspNetModuleId.DeleteConfirmationSeries), DeleteResult.Series);
			radioButtonList.SelectedIndex = 0;
			RenderHelper.SetupASPxWebControl(radioButtonList);
			popupControl.Controls.Add(radioButtonList);
			Table table = RenderHelper.CreateTable();
			table.CellSpacing = 5;
			table.CellPadding = 5;
			TableRow row = new TableRow();
			TableCell cell1 = new TableCell();
			TableCell cell2 = new TableCell();
			ASPxButton okButton = RenderHelper.CreateASPxButton();
			okButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "OK");
			okButton.Width = new Unit("80px");
			okButton.ID = "okButton";
			okButton.ClientSideEvents.Click = String.Format("function(s, e) {{ {0}.Hide(); {1}.PerformCallback({2}.GetSelectedItem().value); }}",
				popupControl.ClientInstanceName, ClientInstanceName, radioButtonListId);
			cell1.Controls.Add(okButton);
			ASPxButton cancelButton = RenderHelper.CreateASPxButton();
			cancelButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Cancel");
			cancelButton.Width = new Unit("80px");
			cancelButton.ID = "cancelButton";
			cancelButton.ClientSideEvents.Click = String.Format("function(s, e) {{ {0}.Hide(); {1}.PerformCallback('{2}'); }}",popupControl.ClientInstanceName, ClientInstanceName, DeleteResult.Cancel);
			cell2.Controls.Add(cancelButton);
			row.Cells.Add(cell1);
			row.Cells.Add(cell2);
			table.Rows.Add(row);
			popupControl.Controls.Add(table);
			return popupControl;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			base.RaiseCallbackEvent(eventArgument);
			if(!String.IsNullOrEmpty(eventArgument)) {
				OnConfirm((DeleteResult)Enum.Parse(typeof(DeleteResult), eventArgument));
			}
		}
		protected virtual void OnConfirm(DeleteResult deleteResult) {
			if(Confirm != null) {
				Confirm(this, new DeleteResultEventArgs(deleteResult));
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.ClientSideEvents.EndCallback = String.Format("function(s,e) {{ {0}.Refresh(ASPxClientSchedulerRefreshAction.VisibleIntervalChanged); }}", scheduler.ClientID);
		}
		protected override object GetCallbackResult() {
			return base.GetCallbackResult();
		}
		public ASPxSchedulerDeleteRecurrenceControl(ASPxScheduler scheduler) {
			ClientInstanceName = "deleteRecurrenceCallbackPanel";
			popupControl = CreateDeleteRecurrencePopupControl();
			Controls.Add(popupControl);
			this.scheduler = scheduler;
		}
		public void ShowConfirmation(string message) {
			popupControl.ShowOnPageLoad = true;
			confirmationTextLabel.Text = message;
		}
		public void HidePopup() {
			popupControl.ShowOnPageLoad = false;
		}
		public event EventHandler<DeleteResultEventArgs> Confirm;
	}
}
