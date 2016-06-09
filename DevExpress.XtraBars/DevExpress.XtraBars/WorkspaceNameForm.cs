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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars {
	public partial class WorkspaceNameForm : XtraForm {
		WorkspaceManager managerCore;
		WorkspaceNameForm() {
			InitializeComponent();
			SetActualSize();
		}
		public WorkspaceNameForm(WorkspaceManager manager)
			: this() {
			managerCore = manager;
		}
		void SetActualSize() {
			Size topPanelSize = stackPanel1.GetMinSize();
			Size bottomPanelSize = stackPanel2.GetMinSize();
			Size clientSize = new Size(Math.Max(topPanelSize.Width, bottomPanelSize.Width), bottomPanelSize.Height + topPanelSize.Height);
			Size = Size - ClientSize + clientSize;
		}
		void OnOkButtonClick(object sender, System.EventArgs e) {
			string errorMessage = ValidateWorkspaceName();
			if(!String.IsNullOrEmpty(errorMessage)) {
				XtraMessageBox.Show(this.LookAndFeel, this, errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			DialogResult result = System.Windows.Forms.DialogResult.Yes;
			foreach(var item in managerCore.Workspaces) {
				if(item.Name == edtWorkspaceName.Text) {
					errorMessage = string.Format(DocumentManagerLocalizer.GetString(DocumentManagerStringId.WorkspaceNameFormErrorMessage), edtWorkspaceName.Text);
					result = XtraMessageBox.Show(this.LookAndFeel, this, errorMessage, DocumentManagerLocalizer.GetString(DocumentManagerStringId.WorkspaceNameFormCaption), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				}
			}
			WorkspaceName = edtWorkspaceName.Text;
			if(result == System.Windows.Forms.DialogResult.Yes)
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		public string WorkspaceName { get; set; }
		string ValidateWorkspaceName() {
			if(string.IsNullOrEmpty(edtWorkspaceName.Text))
				return DocumentManagerLocalizer.GetString(DocumentManagerStringId.WorkspaceNameWarningMessage);
			return string.Empty;
		}
	}
}
