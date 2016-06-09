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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.lblTableSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.lblColumns")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.lblRows")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.spinColumns")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertTableForm.spinRows")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class InsertTableForm : DevExpress.XtraEditors.XtraForm {
		readonly InsertTableFormController controller;
		InsertTableForm() {
			InitializeComponent();
		}
		public InsertTableForm(InsertTableFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			SubscribeControlsEvents();
			UpdateForm();
		}
		protected internal InsertTableFormController Controller { get { return controller; } }
		protected internal virtual InsertTableFormController CreateController(InsertTableFormControllerParameters controllerParameters) {
			return new InsertTableFormController(controllerParameters);
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
			this.spinRows.EditValueChanged -= OnSpinRowsEditValueChanged;
			this.spinColumns.EditValueChanged -= OnSpinColumnsEditValueChanged;
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.spinRows.EditValueChanged += OnSpinRowsEditValueChanged;
			this.spinColumns.EditValueChanged += OnSpinColumnsEditValueChanged;
		}
		protected internal virtual void UpdateFormCore() {
			this.spinColumns.EditValue = Controller.ColumnCount;
			this.spinRows.EditValue = Controller.RowCount;
		}
		void OnSpinColumnsEditValueChanged(object sender, EventArgs e) {
			Controller.ColumnCount = (int)spinColumns.Value;
		}
		void OnSpinRowsEditValueChanged(object sender, EventArgs e) {
			Controller.RowCount = (int)spinRows.Value;
		}
		void btnOk_Click(object sender, EventArgs e) {
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
}
