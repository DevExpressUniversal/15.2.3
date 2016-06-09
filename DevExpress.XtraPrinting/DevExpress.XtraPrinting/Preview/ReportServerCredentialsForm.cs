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

using DevExpress.Skins;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Preview {
	partial class ReportServerCredentialsForm : XtraForm, IReportServerCredentialsForm {
		public ReportServerCredentialsForm() {
			InitializeComponent();
			loginButton.Enabled = CanLogin;
		}
		public bool CanLogin { get { return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password); } }
		public string UserName { get { return userNameEdit.Text; } }
		public string Password { get { return passwordEdit.Text; } }
		void OnCredentialsChanged(object sender, EventArgs e) {
			loginButton.Enabled = CanLogin;
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch(keyData) {
				case Keys.Escape:
					cancelButton.PerformClick();
					return false;
				case Keys.Return:
					if(CanLogin) {
						loginButton.PerformClick();
						return true;
					}
					return base.ProcessCmdKey(ref msg, keyData);
				default:
					return base.ProcessCmdKey(ref msg, keyData);
			}
		}
	}
	interface IReportServerCredentialsForm {
		string UserName { get; }
		string Password { get; }
		DialogResult ShowDialog();
		DialogResult ShowDialog(IWin32Window parent);
	}
}
