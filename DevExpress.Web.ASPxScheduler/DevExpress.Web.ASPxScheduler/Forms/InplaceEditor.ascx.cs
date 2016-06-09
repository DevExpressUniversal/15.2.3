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
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
public partial class InplaceEditor : InplaceEditorBaseFormControl {
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		PrepareMainContainer();
		Localize();
		memSubject.Focus();
	}
	void PrepareMainContainer() {
		RenderUtils.SetTableSpacings(mainContainer, 2, 0);
		RenderUtils.SetAlignAttributes(buttonContainer, null, "top");
	}
	void Localize() {
		btnSave.ToolTip = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_Save);
		btnCancel.ToolTip = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel);
		btnEditForm.ToolTip = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_OpenEditForm);
	}
	public override void DataBind() {
		base.DataBind();
		AppointmentInplaceEditorTemplateContainer container = (AppointmentInplaceEditorTemplateContainer)Parent;
		btnCancel.Image.Assign(container.Control.Images.GetInplaceEditorCancelImage(Page));
		btnSave.Image.Assign(container.Control.Images.GetInplaceEditorSaveImage(Page));
		btnEditForm.Image.Assign(container.Control.Images.GetInplaceEditorEditFormImage(Page));
		btnSave.ClientSideEvents.Click = container.SaveHandler;
		btnCancel.ClientSideEvents.Click = container.CancelHandler;
		btnEditForm.ClientSideEvents.Click = container.EditFormHandler;
	}
	protected override ASPxEditBase[] GetChildEditors() {
		ASPxEditBase[] edits = new ASPxEditBase[] { memSubject };
		return edits;
	}
	protected override ASPxButton[] GetChildButtons() {
		ASPxButton[] buttons = new ASPxButton[] {
			btnSave, btnCancel, btnEditForm
		};
		return buttons;
	}
}
}
