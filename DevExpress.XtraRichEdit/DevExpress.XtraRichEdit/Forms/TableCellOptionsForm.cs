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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblCellMargins")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.chkSameAsWholeTable")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.spnTopMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.spnBottomMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.spnLeftMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.spnRightMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.lblOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.chkWrapText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.chkFitText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableCellOptionsForm.btnCancel")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region TableCellOptionsForm
	[DXToolboxItem(false)]
	public partial class TableCellOptionsForm : XtraForm {
		#region Fields
		readonly RichEditControl control;
		TableCellOptionsFormController controller;
		#endregion
		TableCellOptionsForm() {
			InitializeComponent();
		}
		public TableCellOptionsForm(List<TableCell> tableCells, RichEditControl control) {
			Guard.ArgumentNotNull(tableCells, "tableCells");
			Guard.ArgumentPositive(tableCells.Count, "tableCellsCount");
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.controller = CreateController(tableCells);
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		#region Properties
		public RichEditControl Control { get { return control; } }
		public TableCellOptionsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TableCellOptionsFormController CreateController(List<TableCell> tableCells) {
			return new TableCellOptionsFormController(tableCells);
		}
		void InitializeForm() {
			DocumentUnit UIUnit = Control.InnerControl.UIUnit;
			this.spnTopMargin.Properties.DefaultUnitType = UIUnit;
			this.spnRightMargin.Properties.DefaultUnitType = UIUnit;
			this.spnBottomMargin.Properties.DefaultUnitType = UIUnit;
			this.spnLeftMargin.Properties.DefaultUnitType = UIUnit;
			DocumentModelUnitConverter unitConverter = Control.DocumentModel.UnitConverter;
			this.spnTopMargin.ValueUnitConverter = unitConverter;
			this.spnRightMargin.ValueUnitConverter = unitConverter;
			this.spnBottomMargin.ValueUnitConverter = unitConverter;
			this.spnLeftMargin.ValueUnitConverter = unitConverter;
			this.spnTopMargin.Properties.MaxValue = TableCellOptionsFormDefaults.MaxTopMarginByDefault;
			this.spnTopMargin.Properties.MinValue = TableCellOptionsFormDefaults.MinTopMarginByDefault;
			this.spnRightMargin.Properties.MaxValue = TableCellOptionsFormDefaults.MaxRightMarginByDefault;
			this.spnRightMargin.Properties.MinValue = TableCellOptionsFormDefaults.MinRightMarginByDefault;
			this.spnBottomMargin.Properties.MaxValue = TableCellOptionsFormDefaults.MaxBottomMarginByDefault;
			this.spnBottomMargin.Properties.MinValue = TableCellOptionsFormDefaults.MinBottomMarginByDefault;
			this.spnLeftMargin.Properties.MaxValue = TableCellOptionsFormDefaults.MaxLeftMarginByDefault;
			this.spnLeftMargin.Properties.MinValue = TableCellOptionsFormDefaults.MinLeftMarginByDefault;
		}
		protected internal virtual void SubscribeControlsEvents() {
			spnTopMargin.ValueChanged += OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged += OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged += OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged += OnSpnLeftMarginValueChanged;
			chkSameAsWholeTable.CheckStateChanged += OnChkSameAsWholeTableCheckStateChanged;
			chkWrapText.CheckStateChanged += OnChkWrapTextCheckStateChanged;
			chkFitText.CheckStateChanged += OnChkFitTextCheckStateChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			spnTopMargin.ValueChanged -= OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged -= OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged -= OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged -= OnSpnLeftMarginValueChanged;
			chkSameAsWholeTable.CheckStateChanged -= OnChkSameAsWholeTableCheckStateChanged;
			chkWrapText.CheckStateChanged -= OnChkWrapTextCheckStateChanged;
			chkFitText.CheckStateChanged -= OnChkFitTextCheckStateChanged;
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
			spnTopMargin.Value = Controller.TopMargin;
			spnRightMargin.Value = Controller.RightMargin;
			spnBottomMargin.Value = Controller.BottomMargin;
			spnLeftMargin.Value = Controller.LeftMargin;
			bool cellMarginsSameAsTable = Controller.CellMarginsSameAsTable;
			chkSameAsWholeTable.Checked = cellMarginsSameAsTable;
			SetControlsEnabled(!cellMarginsSameAsTable);
			UpdateCheckEdit(chkWrapText, !Controller.NoWrap);
			UpdateCheckEdit(chkFitText, Controller.FitText);
		}
		protected internal virtual void UpdateCheckEdit(CheckEdit checkEdit, bool? check) {
			if (check == null) {
				checkEdit.Properties.AllowGrayed = true;
				checkEdit.CheckState = CheckState.Indeterminate;
				return;
			}
			if (check.Value)
				checkEdit.CheckState = CheckState.Checked;
			else
				checkEdit.CheckState = CheckState.Unchecked;
		}
		void OnSpnTopMarginValueChanged(object sender, EventArgs e) {
			Controller.TopMargin = spnTopMargin.Value;
		}
		void OnSpnRightMarginValueChanged(object sender, EventArgs e) {
			Controller.RightMargin = spnRightMargin.Value;
		}
		void OnSpnBottomMarginValueChanged(object sender, EventArgs e) {
			Controller.BottomMargin = spnBottomMargin.Value;
		}
		void OnSpnLeftMarginValueChanged(object sender, EventArgs e) {
			Controller.LeftMargin = spnLeftMargin.Value;
		}
		void OnChkSameAsWholeTableCheckStateChanged(object sender, EventArgs e) {
			bool check = chkSameAsWholeTable.Checked;
			Controller.CellMarginsSameAsTable = check;
			SetControlsEnabled(!check);
		}
		void SetControlsEnabled(bool enabled) {
			spnTopMargin.Enabled = enabled;
			spnRightMargin.Enabled = enabled;
			spnBottomMargin.Enabled = enabled;
			spnLeftMargin.Enabled = enabled;
			lblTop.Enabled = enabled;
			lblRight.Enabled = enabled;
			lblBottom.Enabled = enabled;
			lblLeft.Enabled = enabled;
		}
		void OnChkWrapTextCheckStateChanged(object sender, EventArgs e) {
			Controller.NoWrap = !CheckStateToNullableBool(chkWrapText.CheckState);
		}
		void OnChkFitTextCheckStateChanged(object sender, EventArgs e) {
			Controller.FitText = CheckStateToNullableBool(chkFitText.CheckState);
		}
		bool? CheckStateToNullableBool(CheckState checkState) {
			switch (checkState) {
				case CheckState.Checked:
					return true;
				case CheckState.Unchecked:
					return false;
				case CheckState.Indeterminate:
					return null;
			}
			return null;
		}
		void OnBtnOkClick(object sender, EventArgs e) {
			Controller.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
	}
	#endregion
}
