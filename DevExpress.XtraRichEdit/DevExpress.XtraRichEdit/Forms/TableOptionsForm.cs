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
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.spnTopMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblDefaultCellMargins")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.spnBottomMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.spnLeftMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.spnRightMargin")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblDefaultCellSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.cbAllowCellSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.spnSpacingBetweenCells")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.lblOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TableOptionsForm.chkResizeToFitContent")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region TableOptionsForm
	[DXToolboxItem(false)]
	public partial class TableOptionsForm : XtraForm {
		#region Fields
		readonly IRichEditControl control;
		TableOptionsFormController controller;
		#endregion
		TableOptionsForm() {
			InitializeComponent();
		}
		public TableOptionsForm(TableOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public TableOptionsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TableOptionsFormController CreateController(TableOptionsFormControllerParameters controllerParameters) {
			return new TableOptionsFormController(controllerParameters);
		}
		private void InitializeForm() {
			DocumentUnit UIUnit = Control.InnerControl.UIUnit;
			this.spnTopMargin.Properties.DefaultUnitType = UIUnit;
			this.spnRightMargin.Properties.DefaultUnitType = UIUnit;
			this.spnBottomMargin.Properties.DefaultUnitType = UIUnit;
			this.spnLeftMargin.Properties.DefaultUnitType = UIUnit;
			this.spnSpacingBetweenCells.Properties.DefaultUnitType = UIUnit;
			DocumentModelUnitConverter unitConverter = Control.InnerControl.DocumentModel.UnitConverter;
			this.spnTopMargin.ValueUnitConverter = unitConverter;
			this.spnRightMargin.ValueUnitConverter = unitConverter;
			this.spnBottomMargin.ValueUnitConverter = unitConverter;
			this.spnLeftMargin.ValueUnitConverter = unitConverter;
			this.spnSpacingBetweenCells.ValueUnitConverter = unitConverter;
			this.spnTopMargin.Properties.MaxValue = TableOptionsFormDefaults.MaxTopMarginByDefault;
			this.spnTopMargin.Properties.MinValue = TableOptionsFormDefaults.MinTopMarginByDefault;
			this.spnRightMargin.Properties.MaxValue = TableOptionsFormDefaults.MaxRightMarginByDefault;
			this.spnRightMargin.Properties.MinValue = TableOptionsFormDefaults.MinRightMarginByDefault;
			this.spnBottomMargin.Properties.MaxValue = TableOptionsFormDefaults.MaxBottomMarginByDefault;
			this.spnBottomMargin.Properties.MinValue = TableOptionsFormDefaults.MinBottomMarginByDefault;
			this.spnLeftMargin.Properties.MaxValue = TableOptionsFormDefaults.MaxLeftMarginByDefault;
			this.spnLeftMargin.Properties.MinValue = TableOptionsFormDefaults.MinLeftMarginByDefault;
			this.spnSpacingBetweenCells.Properties.MaxValue = TableOptionsFormDefaults.MaxCellSpacingByDefault;
			this.spnSpacingBetweenCells.Properties.MinValue = TableOptionsFormDefaults.MinCellSpacingByDefault;
		}
		protected internal virtual void SubscribeControlsEvents() {
			spnTopMargin.ValueChanged += OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged += OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged += OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged += OnSpnLeftMarginValueChanged;
			spnSpacingBetweenCells.ValueChanged += OnSpnSpacingBetweenCellsValueChanged;
			cbAllowCellSpacing.CheckedChanged += OnCbAllowCellSpacingCheckedChanged;
			chkResizeToFitContent.CheckedChanged += OnChkResizeToFitContentCheckedChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			spnTopMargin.ValueChanged -= OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged -= OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged -= OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged -= OnSpnLeftMarginValueChanged;
			spnSpacingBetweenCells.ValueChanged -= OnSpnSpacingBetweenCellsValueChanged;
			cbAllowCellSpacing.CheckedChanged -= OnCbAllowCellSpacingCheckedChanged;
			chkResizeToFitContent.CheckedChanged -= OnChkResizeToFitContentCheckedChanged;
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
			spnSpacingBetweenCells.Value = Controller.CellSpacing;
			spnSpacingBetweenCells.Enabled = Controller.AllowCellSpacing;
			cbAllowCellSpacing.Checked = Controller.AllowCellSpacing;
			chkResizeToFitContent.Checked = Controller.ResizeToFitContent;
		}
		void OnBtnOkClick(object sender, EventArgs e) {
			Controller.ApplyChanges();
			DialogResult = DialogResult.OK;
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
		void OnSpnSpacingBetweenCellsValueChanged(object sender, EventArgs e) {
			Controller.CellSpacing = spnSpacingBetweenCells.Value;
		}
		void OnCbAllowCellSpacingCheckedChanged(object sender, EventArgs e) {
			bool cellSpacingChecked = cbAllowCellSpacing.Checked;
			if (cellSpacingChecked && spnSpacingBetweenCells.Value == 0)
				spnSpacingBetweenCells.Value = TableOptionsFormDefaults.MinCellSpacing;
			Controller.AllowCellSpacing = cellSpacingChecked;
			spnSpacingBetweenCells.Enabled = cellSpacingChecked;
		}
		void OnChkResizeToFitContentCheckedChanged(object sender, EventArgs e) {
			Controller.ResizeToFitContent = chkResizeToFitContent.Checked;
		}
	}
	#endregion
}
