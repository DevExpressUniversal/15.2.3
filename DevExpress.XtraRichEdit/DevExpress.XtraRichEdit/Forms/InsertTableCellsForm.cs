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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using System.Windows.Forms;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableCellsForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableCellsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableCellsForm.rgCellOperation")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class InsertTableCellsForm : XtraForm {
		#region Fields
		readonly InsertTableCellsFormController controller;
		#endregion
		InsertTableCellsForm() {
			InitializeComponent();
		}
		public InsertTableCellsForm(InsertTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			UpdateForm();
		}
		#region Properties
		protected internal InsertTableCellsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual InsertTableCellsFormController CreateController(InsertTableCellsFormControllerParameters controllerParameters) {
			return new InsertTableCellsFormController(controllerParameters);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.rgCellOperation.SelectedIndexChanged -= OnCellOperationSelectedIndexChanged;
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.rgCellOperation.SelectedIndexChanged += OnCellOperationSelectedIndexChanged;
		}
		protected internal virtual void UpdateFormCore() {
			this.rgCellOperation.SelectedIndex = (int)Controller.CellOperation;
		}
		private void OnCellOperationSelectedIndexChanged(object sender, EventArgs e) {
			Controller.CellOperation = (TableCellOperation)rgCellOperation.SelectedIndex;
		}
		private void OnOkClick(object sender, EventArgs e) {
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
}
