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
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Localization;
namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
public partial class RecurrentAppointmentEditForm : SchedulerFormControl {
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		rbAction.SelectedIndex = 0;
		Localize();
	}
	public override void DataBind() {
		base.DataBind();
		Localize((RecurrentAppointmentEditFormTemplateContainer)Parent);
		AssignStatusImage(Image);
		SubscribeButtons();
	}
	void Localize() {
		btnOk.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonOk);
		btnCancel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel);
		if (rbAction.Items.Count == 2) {
			ListEditItem seriesItem = rbAction.Items[0];
			seriesItem.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Series);
			ListEditItem occurrenceItem = rbAction.Items[1];
			occurrenceItem.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Occurrence);
		}		
	}
	void Localize(RecurrentAppointmentEditFormTemplateContainer container) {
		string formatString = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ConfirmEdit);
		lblConfirm.Text = String.Format(formatString, container.Appointment.Subject);
	}
	void AssignStatusImage(ASPxImage image) {
		RecurrentAppointmentEditFormTemplateContainer container = (RecurrentAppointmentEditFormTemplateContainer)Parent;
		ASPxSchedulerImages images = container.Control.Images;
		image.SpriteCssClass = images.GetStatusInfoImage(Page).SpriteProperties.CssClass;
		image.SpriteImageUrl = images.SpriteImageUrl;
	}
	void SubscribeButtons() {
		RecurrentAppointmentEditFormTemplateContainer container = (RecurrentAppointmentEditFormTemplateContainer)Parent;
		this.btnOk.ClientSideEvents.Click = container.ApplyHandler;
		this.btnCancel.ClientSideEvents.Click = container.CancelHandler;
	}
	protected override ASPxEditBase[] GetChildEditors() {
		ASPxEditBase[] edits = new ASPxEditBase[] { lblConfirm, rbAction };
		return edits;
	}
	protected override ASPxButton[] GetChildButtons() {
		ASPxButton[] buttons = new ASPxButton[] {
			btnOk, btnCancel
		};
		return buttons;
	}
}
}
