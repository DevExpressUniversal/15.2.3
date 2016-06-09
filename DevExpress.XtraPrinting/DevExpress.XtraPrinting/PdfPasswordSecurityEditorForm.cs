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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraPrinting.Native.WinControls {
	public partial class PdfPasswordSecurityEditorForm : XtraForm {
		#region inner classes
		class PrintingPermissionsComboItem {
			PrintingPermissions printingPermissions;
			string text;
			public PrintingPermissionsComboItem(PrintingPermissions printingPermissions) {
				this.printingPermissions = printingPermissions;
				text = ToString();
			}
			public string Text { get { return text; } }
			public PrintingPermissions PrintingPermissions { get { return printingPermissions; } }
			public override string ToString() {
				switch (printingPermissions) {
					case PrintingPermissions.None:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_None);
					case PrintingPermissions.LowResolution:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_LowResolution);
					case PrintingPermissions.HighResolution:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_HighResolution);
					default:
						throw new ArgumentException("printingPermissions");
				}
			}
		}
		class ChangingPermissionsComboItem {
			ChangingPermissions changingPermissions;
			string text;
			public ChangingPermissionsComboItem(ChangingPermissions changingPermissions) {
				this.changingPermissions = changingPermissions;
				text = ToString();
			}
			public string Text { get { return text; } }
			public ChangingPermissions ChangingPermissions { get { return changingPermissions; } }
			public override string ToString() {
				switch(changingPermissions) {
					case ChangingPermissions.None:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_None);
					case ChangingPermissions.InsertingDeletingRotating:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_InsertingDeletingRotating);
					case ChangingPermissions.FillingSigning:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_FillingSigning);
					case ChangingPermissions.CommentingFillingSigning:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_CommentingFillingSigning);
					case ChangingPermissions.AnyExceptExtractingPages:
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_AnyExceptExtractingPages);
					default:
						throw new ArgumentException("changingPermissions");
				}
			}
		}
		#endregion
		PdfPasswordSecurityOptions options = new PdfPasswordSecurityOptions();
		public PdfPasswordSecurityOptions PdfPasswordSecurityOptions {
			get { return options; }
		}
		public PdfPasswordSecurityEditorForm() {
			InitializeComponent();
			InitComboBoxes();
			UpdateControls();
		}
		void chbOpenPassword_CheckedChanged(object sender, EventArgs e) {
			UpdateControls();
		}
		void chbRestrict_CheckedChanged(object sender, EventArgs e) {
			UpdateControls();
		}
		void btnOK_Click(object sender, EventArgs e) {
			if(!ConfirmOpenPassword())
				return;
			if(!ConfirmPermissionsPassword())
				return;
			Flush();
			DialogResult = DialogResult.OK;
			Close();
		}
		void UpdateControls() {
			bool enableOpenPassword = chbOpenPassword.Checked;
			teOpenPassword.Enabled = enableOpenPassword;
			lbOpenPassword.Enabled = enableOpenPassword;
			bool enableRestrictions = chbRestrict.Checked;
			tePermissionsPassword.Enabled = enableRestrictions;
			lbPermissionsPassword.Enabled = enableRestrictions;
			lkpPrintingAllowed.Enabled = enableRestrictions;
			lbPrintingAllowed.Enabled = enableRestrictions;
			lkpChangesAllowed.Enabled = enableRestrictions;
			lbChangesAllowed.Enabled = enableRestrictions;
			chbEnableCoping.Enabled = enableRestrictions;
			chbEnableScreenReaders.Enabled = enableRestrictions;
		}
		void InitComboBoxes() {
			List<PrintingPermissionsComboItem> printingPermissions = new List<PrintingPermissionsComboItem>();
			printingPermissions.Add(new PrintingPermissionsComboItem(PrintingPermissions.None));
			printingPermissions.Add(new PrintingPermissionsComboItem(PrintingPermissions.LowResolution));
			printingPermissions.Add(new PrintingPermissionsComboItem(PrintingPermissions.HighResolution));
			lkpPrintingAllowed.Properties.Columns.Add(new LookUpColumnInfo("Text"));
			lkpPrintingAllowed.Properties.DataSource = printingPermissions;
			lkpPrintingAllowed.Properties.DropDownRows = printingPermissions.Count;
			lkpPrintingAllowed.Properties.DisplayMember = "Text";
			lkpPrintingAllowed.Properties.ValueMember = "PrintingPermissions";
			lkpPrintingAllowed.EditValue = PrintingPermissions.None;
			List<ChangingPermissionsComboItem> changingPermissions = new List<ChangingPermissionsComboItem>();
			changingPermissions.Add(new ChangingPermissionsComboItem(ChangingPermissions.None));
			changingPermissions.Add(new ChangingPermissionsComboItem(ChangingPermissions.InsertingDeletingRotating));
			changingPermissions.Add(new ChangingPermissionsComboItem(ChangingPermissions.FillingSigning));
			changingPermissions.Add(new ChangingPermissionsComboItem(ChangingPermissions.CommentingFillingSigning));
			changingPermissions.Add(new ChangingPermissionsComboItem(ChangingPermissions.AnyExceptExtractingPages));
			lkpChangesAllowed.Properties.Columns.Add(new LookUpColumnInfo("Text"));
			lkpChangesAllowed.Properties.DataSource = changingPermissions;
			lkpChangesAllowed.Properties.DropDownRows = changingPermissions.Count;
			lkpChangesAllowed.Properties.DisplayMember = "Text";
			lkpChangesAllowed.Properties.ValueMember = "ChangingPermissions";
			lkpChangesAllowed.EditValue = ChangingPermissions.None;
		}
		void InitControls() {
			teOpenPassword.Text = options.OpenPassword;
			chbOpenPassword.Checked = !string.IsNullOrEmpty(options.OpenPassword);
			tePermissionsPassword.Text = options.PermissionsPassword;
			chbRestrict.Checked = !string.IsNullOrEmpty(options.PermissionsPassword);
			lkpPrintingAllowed.EditValue = options.PermissionsOptions.PrintingPermissions;
			lkpChangesAllowed.EditValue = options.PermissionsOptions.ChangingPermissions;
			chbEnableCoping.Checked = options.PermissionsOptions.EnableCopying;
			chbEnableScreenReaders.Checked = options.PermissionsOptions.EnableScreenReaders;
		}
		bool ConfirmPasswordCore(string password, string caption, string note, string passwordName) {
			if(string.IsNullOrEmpty(password))
				return true;
			PasswordConfirmationForm form = new PasswordConfirmationForm();
			form.Init(caption, note, passwordName);
			form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			if(form.ShowDialog(this) != DialogResult.OK)
				return false;
			if(password != form.Password) {
				NotificationService.ShowMessage<PrintingSystemBase>(LookAndFeel, this, PreviewStringId.ExportOption_ConfirmationDoesNotMatchForm_Msg.GetString(), PreviewStringId.Msg_Caption.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return ConfirmPasswordCore(password, caption, note, passwordName);
			}
			return true;
		}
		bool ConfirmOpenPassword() {
			return !chbOpenPassword.Checked ? true :
				ConfirmPasswordCore(teOpenPassword.Text,
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Caption),
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Note),
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Name));
		}
		bool ConfirmPermissionsPassword() {
			return !chbRestrict.Checked ? true :
				ConfirmPasswordCore(tePermissionsPassword.Text,
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Caption),
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Note),
				PreviewLocalizer.GetString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Name));
		}
		void Flush() {
			options.OpenPassword = chbOpenPassword.Checked ? teOpenPassword.Text : string.Empty;
			options.PermissionsPassword = chbRestrict.Checked ? tePermissionsPassword.Text : string.Empty;
			options.PermissionsOptions.PrintingPermissions = (PrintingPermissions)lkpPrintingAllowed.EditValue;
			options.PermissionsOptions.ChangingPermissions = (ChangingPermissions)lkpChangesAllowed.EditValue;
			options.PermissionsOptions.EnableCopying = chbEnableCoping.Checked;
			options.PermissionsOptions.EnableScreenReaders = chbEnableScreenReaders.Checked;
		}
		public void Init(PdfPasswordSecurityOptions options) {
			this.options.Assign(options);
			InitControls();
			UpdateControls();
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			if(levent.AffectedProperty == "Visible") {
				DevExpress.Utils.LayoutHelper.DoLabelsEditorsLayout(
					new BaseControl[] { lbOpenPassword, lbPermissionsPassword, lbPrintingAllowed, lbChangesAllowed },
					new BaseControl[] { teOpenPassword, tePermissionsPassword, lkpPrintingAllowed, lkpChangesAllowed }
					);
				DevExpress.Utils.LayoutHelper.DoButtonLayout(this.btnOK, this.btnCancel);
			}
		}
	}
}
