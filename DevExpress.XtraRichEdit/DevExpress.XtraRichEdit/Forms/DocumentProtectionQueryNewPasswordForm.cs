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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.lblPassword")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.lblRepeatPassword")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.edtPassword")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.edtRepeatPassword")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.DocumentProtectionQueryNewPasswordForm.btnOk")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class DocumentProtectionQueryNewPasswordForm : XtraForm {
		readonly DocumentProtectionQueryNewPasswordFormController controller;
		public DocumentProtectionQueryNewPasswordForm() {
			InitializeComponent();
		}
		public DocumentProtectionQueryNewPasswordForm(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
		}
		public DocumentProtectionQueryNewPasswordFormController Controller { get { return controller; } }
		protected virtual DocumentProtectionQueryNewPasswordFormController CreateController(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters) {
			return new DocumentProtectionQueryNewPasswordFormController(controllerParameters);
		}
		private void btnOk_Click(object sender, EventArgs e) {
			if (edtPassword.Text != edtRepeatPassword.Text) {
				XtraMessageBox.Show(LookAndFeel, this, XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DocumentProtectionInvalidPasswordConfirmation), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				DialogResult = DialogResult.None;
				return;
			}
			Controller.Password = edtPassword.Text;
			Controller.ApplyChanges();
		}
	}
}
