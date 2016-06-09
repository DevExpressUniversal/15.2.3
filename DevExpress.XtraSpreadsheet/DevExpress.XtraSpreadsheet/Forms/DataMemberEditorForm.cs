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
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class DataMemberEditorForm : XtraForm {
		#region Fields
		readonly DataMemberEditorViewModel viewModel;
		readonly SpreadsheetControl control;
		#endregion
		DataMemberEditorForm() {
			InitializeComponent();
		}
		public DataMemberEditorForm(DataMemberEditorViewModel viewModel, SpreadsheetControl control) {
			this.viewModel = viewModel;
			this.control = control;
			InitializeComponent();
			InitializeEditors();
		}
		#region Properties
		public DataMemberEditorViewModel ViewModel { get { return viewModel; } }
		#endregion
		void InitializeEditors() {
			fieldList.SpreadsheetControl = control;
			fieldList.DataMember = control.DocumentModel.MailMergeDataMember;
			fieldList.RefreshTreeList();
			slctDataMember.Properties.Items.AddRange(viewModel.DataMembersNames);
			slctDataMember.SelectedIndex = 0;
			edtDataMember.Properties.ReadOnly = control.Document.MailMergeDataSource != null;
		}
		void fieldList_SelectionChanged(object sender, EventArgs e) {
			string shortResult = fieldList.GetSelectedNodeShortName();
			if (!String.IsNullOrEmpty(shortResult)) {
				string fullDisplayName = control.DocumentModel.GetMailMergeDisplayName(fieldList.GetSelectedNodeName(), true);
				string[] displayNames = fullDisplayName.Split(".".ToCharArray());
				edtDataMember.Text = displayNames[displayNames.Length - 1];
				viewModel.DataMembers[slctDataMember.SelectedIndex] = shortResult;
			}
		}
		void edtDataMember_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				viewModel.DataMembers[slctDataMember.SelectedIndex] = edtDataMember.Text;
		}
		void slctDataMember_SelectedIndexChanged(object sender, EventArgs e) {
			string name = viewModel.DataMembers[slctDataMember.SelectedIndex];
			edtDataMember.Text = control.DocumentModel.GetMailMergeDisplayName(name, true);
			fieldList.SelectNode(name);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (edtDataMember.Text != control.DocumentModel.GetMailMergeDisplayName(viewModel.DataMembers[slctDataMember.SelectedIndex], true))
				viewModel.DataMembers[slctDataMember.SelectedIndex] = edtDataMember.Text;
			this.DialogResult = DialogResult.OK;
		}
		protected override void Dispose(bool disposing) {
			this.fieldList.SelectionChanged -= fieldList_SelectionChanged;
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
