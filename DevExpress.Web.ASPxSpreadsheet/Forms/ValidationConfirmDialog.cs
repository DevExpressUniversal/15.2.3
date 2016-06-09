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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	class ValidationConfirmDialog : SpreadsheetDialogBase {
		protected ASPxButton RetryButton { get; private set; }
		protected ASPxLabel Message { get; private set; }
		protected ASPxButton CreateRetryButton() {
			ASPxButton button = new ASPxButton();
			button.ID = "btnRetry";
			button.AutoPostBack = false;
			button.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			button.CausesValidation = false;
			button.ClientInstanceName = GetControlClientInstanceName("_dxBtnRetry");
			return button;
		}
		protected void CreateMessageLabel() {
			Message = new ASPxLabel();
			Message.ID = "lblMessage";
			Message.ClientInstanceName = GetControlClientInstanceName("_dxLblMessage");
		}
		protected override void PopulateContentArea(Control container) {
			WebControl wrapperDiv = RenderUtils.CreateDiv();
			wrapperDiv.CssClass = "dxssValidationMessage";
			CreateMessageLabel();
			wrapperDiv.Controls.Add(Message);
			container.Controls.Add(wrapperDiv);
		}
		protected override void InitializeMiddleButtons(Control container) {
			RetryButton = CreateRetryButton();
			container.Controls.Add(RetryButton);
		}
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.ValidationConfirmDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxValidationConfirmForm";
		}
	}
}
