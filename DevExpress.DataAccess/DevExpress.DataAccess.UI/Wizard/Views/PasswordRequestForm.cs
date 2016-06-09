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

using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class PasswordRequestForm : XtraForm {
		public PasswordRequestForm(bool needPassword, bool canSavePassword) {
			InitializeComponent();
			LocalizeComponent();
			layoutItemPassword.Visibility = needPassword ? LayoutVisibility.Always : LayoutVisibility.Never;
			layoutItemSavePassword.Visibility = canSavePassword ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		PasswordRequestForm() : this(true, true) {
		}
		public string FileName {
			get { return editFileName.Text; }
			set { editFileName.Text = value; }
		}
		public string Password {
			get { return textPassword.Text; }
		}
		public bool SavePassword {
			get { return checkSavePassword.Checked; }
			set { checkSavePassword.Checked = value; }
		}
		public string JustificationText {
			get { return labelJustification.Text; }
			set { labelJustification.Text = value; }
		}
		void LocalizeComponent() {
			buttonOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			buttonCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			checkSavePassword.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.PasswordRequest_SavePassword);
			layoutItemPassword.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.PasswordRequest_Password);
			layoutControlFileName.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.PasswordRequest_FileName);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.PasswordRequest);
		}
		void editFileName_ButtonClick(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			openDialog.Filter = FileNameFilterStrings.ExcelCsv;
			if(openDialog.ShowDialog() == DialogResult.OK)
				editFileName.Text = openDialog.FileName;
		}
	}
}
