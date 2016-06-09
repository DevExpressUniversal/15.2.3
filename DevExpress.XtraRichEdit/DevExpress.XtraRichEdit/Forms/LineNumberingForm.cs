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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Office;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.rgrpNumberingRestart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.lblStartAt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.chkAddLineNumbering")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.edtStartAt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.lblFromText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.edtFromText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.edtCountBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.lblCountBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.lblNumbering")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.LineNumberingForm.btnOk")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class LineNumberingForm : XtraForm {
		readonly LineNumberingFormController controller;
		readonly IRichEditControl control;
		public LineNumberingForm() {
			InitializeComponent();
		}
		public LineNumberingForm(LineNumberingFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			InitializeForm();
			this.controller = CreateController(controllerParameters);
			SubscribeControlsEvents();
			UpdateForm();
		}
		protected internal LineNumberingFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		protected internal virtual LineNumberingFormController CreateController(LineNumberingFormControllerParameters controllerParameters) {
			return new LineNumberingFormController(controllerParameters);
		}
		void InitializeForm() {
			RadioGroupItemCollection items = rgrpNumberingRestart.Properties.Items;
			items[0].Value = LineNumberingRestart.NewPage;
			items[1].Value = LineNumberingRestart.NewSection;
			items[2].Value = LineNumberingRestart.Continuous;
			edtFromText.ValueUnitConverter = UnitConverter;
			edtFromText.Properties.DefaultUnitType = Control.InnerControl.UIUnit;
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
			chkAddLineNumbering.Checked = Controller.Step > 0;
			edtStartAt.EditValue = Controller.StartingLineNumber;
			edtCountBy.EditValue = Controller.Step;
			edtFromText.Value = Controller.Distance;
			rgrpNumberingRestart.EditValue = Controller.NumberingRestartType;
			EnableControls(chkAddLineNumbering.Checked);
		}
		protected internal virtual void EnableControls(bool enable) {
			lblStartAt.Enabled = enable;
			edtStartAt.Enabled = enable;
			lblFromText.Enabled = enable;
			edtFromText.Enabled = enable;
			lblCountBy.Enabled = enable;
			edtCountBy.Enabled = enable;
			lblNumbering.Enabled = enable;
			rgrpNumberingRestart.Enabled = enable;
		}
		protected internal virtual void SubscribeControlsEvents() {
			chkAddLineNumbering.CheckedChanged += chkAddLineNumbering_CheckedChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			chkAddLineNumbering.CheckedChanged -= chkAddLineNumbering_CheckedChanged;
		}
		void chkAddLineNumbering_CheckedChanged(object sender, EventArgs e) {
			EnableControls(chkAddLineNumbering.Checked);
			if (chkAddLineNumbering.Checked) {
				if (Convert.ToInt32(edtCountBy.EditValue) <= 0)
					edtCountBy.EditValue = 1;
				CommitValuesToController();
			}
		}
		protected internal virtual void CommitValuesToController() {
			if (!chkAddLineNumbering.Checked)
				Controller.Step = 0;
			else
				Controller.Step = Convert.ToInt32(edtCountBy.EditValue);
			Controller.NumberingRestartType = (LineNumberingRestart)rgrpNumberingRestart.EditValue;
			Controller.StartingLineNumber = Convert.ToInt32(edtStartAt.EditValue);
			Controller.Distance = edtFromText.Value.HasValue ? edtFromText.Value.Value : 0;
		}
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			CommitValuesToController();
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
}
