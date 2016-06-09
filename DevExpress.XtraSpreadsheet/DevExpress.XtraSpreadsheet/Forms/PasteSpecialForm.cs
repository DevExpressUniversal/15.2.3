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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpreadsheet.Forms.PasteSpecialForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpreadsheet.Forms.PasteSpecialForm.lblPasteAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpreadsheet.Forms.PasteSpecialForm.lbCommandType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpreadsheet.Forms.PasteSpecialForm.btnOk")]
#endregion
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class PasteSpecialForm : XtraForm {
		readonly PasteSpecialFormController controller;
		readonly ISpreadsheetControl control;
		PasteSpecialForm() {
			InitializeComponent();
		}
		public PasteSpecialForm(PasteSpecialFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		protected internal PasteSpecialFormController Controller { get { return controller; } }
		public ISpreadsheetControl Control { get { return control; } }
		protected internal virtual PasteSpecialFormController CreateController(PasteSpecialFormControllerParameters controllerParameters) {
			return new PasteSpecialFormController(controllerParameters);
		}
		void InitializeForm() {
			ListBoxItemCollection items = lbCommandType.Items;
			items.BeginUpdate();
			try {
				items.Clear();
				IList<Type> availableCommandTypes = Controller.AvailableCommandTypes;
				int count = availableCommandTypes.Count;
				for(int i = 0; i < count; i++) {
					PasteSpecialListBoxItem item = new PasteSpecialListBoxItem(control, availableCommandTypes[i]);
					items.Add(item);
				}
			}
			finally {
				items.EndUpdate();
			}
			if(lbCommandType.ItemCount > 0)
				lbCommandType.SelectedIndex = 0;
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
		protected internal virtual void UpdateFormCore() {
		}
		protected internal virtual void SubscribeControlsEvents() {
			lbCommandType.DoubleClick += OnLbDocumentFormatDoubleClick;
			lbCommandType.SelectedIndexChanged += OnLbDocumentFormatSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			lbCommandType.DoubleClick -= OnLbDocumentFormatDoubleClick;
			lbCommandType.SelectedIndexChanged -= OnLbDocumentFormatSelectedIndexChanged;
		}
		void OnLbDocumentFormatSelectedIndexChanged(object sender, EventArgs e) {
			CommitValuesToController();
		}
		void OnLbDocumentFormatDoubleClick(object sender, EventArgs e) {
			int index = lbCommandType.IndexFromPoint(lbCommandType.PointToClient(Cursor.Position));
			if(index >= 0)
				OnBtnOkClick(sender, e);
		}
		protected internal virtual void CommitValuesToController() {
			PasteSpecialListBoxItem item = lbCommandType.SelectedItem as PasteSpecialListBoxItem;
			if(item != null)
				Controller.PasteCommandType = item.CommandType;
		}
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			CommitValuesToController();
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		private void PasteSpecialForm_Load(object sender, EventArgs e) {
		}
	}
}
