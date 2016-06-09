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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	partial class AxisNumericScaleOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisNumericScaleOptionsControl));
			this.pnlInnerSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbMeasureUnit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlMeasureUnitsSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlCustomMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnCustomMeasureUnit = new DevExpress.XtraEditors.SpinEdit();
			this.lblCustomMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlCustomMeasureUnitSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlGridAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbGridAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblGridAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlGridAlignmentSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlCustomGridAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnCustomGridAlignment = new DevExpress.XtraEditors.SpinEdit();
			this.lblCustomGridAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnit)).BeginInit();
			this.pnlMeasureUnit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbMeasureUnit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnitsSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomMeasureUnit)).BeginInit();
			this.pnlCustomMeasureUnit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnCustomMeasureUnit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomMeasureUnitSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignment)).BeginInit();
			this.pnlGridAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbGridAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignmentSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomGridAlignment)).BeginInit();
			this.pnlCustomGridAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnCustomGridAlignment.Properties)).BeginInit();
			this.SuspendLayout();
			this.pnlInnerSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlInnerSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlInnerSeparator, "pnlInnerSeparator");
			this.pnlInnerSeparator.Name = "pnlInnerSeparator";
			resources.ApplyResources(this.pnlMeasureUnit, "pnlMeasureUnit");
			this.pnlMeasureUnit.BackColor = System.Drawing.Color.Transparent;
			this.pnlMeasureUnit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMeasureUnit.Controls.Add(this.cmbMeasureUnit);
			this.pnlMeasureUnit.Controls.Add(this.lblMeasureUnit);
			this.pnlMeasureUnit.Name = "pnlMeasureUnit";
			resources.ApplyResources(this.cmbMeasureUnit, "cmbMeasureUnit");
			this.cmbMeasureUnit.Name = "cmbMeasureUnit";
			this.cmbMeasureUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbMeasureUnit.Properties.Buttons"))))});
			this.cmbMeasureUnit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbMeasureUnit.SelectedIndexChanged += new System.EventHandler(this.cmbMeasureUnit_SelectedIndexChanged);
			resources.ApplyResources(this.lblMeasureUnit, "lblMeasureUnit");
			this.lblMeasureUnit.Name = "lblMeasureUnit";
			this.pnlMeasureUnitsSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlMeasureUnitsSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlMeasureUnitsSeparator, "pnlMeasureUnitsSeparator");
			this.pnlMeasureUnitsSeparator.Name = "pnlMeasureUnitsSeparator";
			resources.ApplyResources(this.pnlCustomMeasureUnit, "pnlCustomMeasureUnit");
			this.pnlCustomMeasureUnit.BackColor = System.Drawing.Color.Transparent;
			this.pnlCustomMeasureUnit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCustomMeasureUnit.Controls.Add(this.spnCustomMeasureUnit);
			this.pnlCustomMeasureUnit.Controls.Add(this.lblCustomMeasureUnit);
			this.pnlCustomMeasureUnit.Name = "pnlCustomMeasureUnit";
			resources.ApplyResources(this.spnCustomMeasureUnit, "spnCustomMeasureUnit");
			this.spnCustomMeasureUnit.Name = "spnCustomMeasureUnit";
			this.spnCustomMeasureUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spnCustomMeasureUnit.Properties.Buttons"))))});
			this.spnCustomMeasureUnit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spnCustomMeasureUnit.Properties.MaxValue = new decimal(new int[] {
			100000000,
			0,
			0,
			0});
			this.spnCustomMeasureUnit.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			589824});
			this.spnCustomMeasureUnit.Properties.ValidateOnEnterKey = true;
			this.spnCustomMeasureUnit.EditValueChanged += new System.EventHandler(this.spnCustomMeasureUnit_EditValueChanged);
			resources.ApplyResources(this.lblCustomMeasureUnit, "lblCustomMeasureUnit");
			this.lblCustomMeasureUnit.Name = "lblCustomMeasureUnit";
			this.pnlCustomMeasureUnitSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlCustomMeasureUnitSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlCustomMeasureUnitSeparator, "pnlCustomMeasureUnitSeparator");
			this.pnlCustomMeasureUnitSeparator.Name = "pnlCustomMeasureUnitSeparator";
			resources.ApplyResources(this.pnlGridAlignment, "pnlGridAlignment");
			this.pnlGridAlignment.BackColor = System.Drawing.Color.Transparent;
			this.pnlGridAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGridAlignment.Controls.Add(this.cmbGridAlignment);
			this.pnlGridAlignment.Controls.Add(this.lblGridAlignment);
			this.pnlGridAlignment.Name = "pnlGridAlignment";
			resources.ApplyResources(this.cmbGridAlignment, "cmbGridAlignment");
			this.cmbGridAlignment.Name = "cmbGridAlignment";
			this.cmbGridAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbGridAlignment.Properties.Buttons"))))});
			this.cmbGridAlignment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbGridAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbGridAlignment_SelectedIndexChanged);
			this.cmbGridAlignment.Validating += new System.ComponentModel.CancelEventHandler(this.cmbGridAlignment_Validating);
			this.cmbGridAlignment.Validated += new System.EventHandler(this.cmbGridAlignment_Validated);
			resources.ApplyResources(this.lblGridAlignment, "lblGridAlignment");
			this.lblGridAlignment.Name = "lblGridAlignment";
			this.pnlGridAlignmentSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlGridAlignmentSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlGridAlignmentSeparator, "pnlGridAlignmentSeparator");
			this.pnlGridAlignmentSeparator.Name = "pnlGridAlignmentSeparator";
			resources.ApplyResources(this.pnlCustomGridAlignment, "pnlCustomGridAlignment");
			this.pnlCustomGridAlignment.BackColor = System.Drawing.Color.Transparent;
			this.pnlCustomGridAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCustomGridAlignment.Controls.Add(this.spnCustomGridAlignment);
			this.pnlCustomGridAlignment.Controls.Add(this.lblCustomGridAlignment);
			this.pnlCustomGridAlignment.Name = "pnlCustomGridAlignment";
			resources.ApplyResources(this.spnCustomGridAlignment, "spnCustomGridAlignment");
			this.spnCustomGridAlignment.Name = "spnCustomGridAlignment";
			this.spnCustomGridAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spnCustomGridAlignment.Properties.Buttons"))))});
			this.spnCustomGridAlignment.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spnCustomGridAlignment.Properties.MaxValue = new decimal(new int[] {
			100000000,
			0,
			0,
			0});
			this.spnCustomGridAlignment.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			589824});
			this.spnCustomGridAlignment.Properties.ValidateOnEnterKey = true;
			this.spnCustomGridAlignment.Validating += new System.ComponentModel.CancelEventHandler(this.spnCustomGridAlignment_Validating);
			this.spnCustomGridAlignment.Validated += new System.EventHandler(this.spnCustomGridAlignment_Validated);
			resources.ApplyResources(this.lblCustomGridAlignment, "lblCustomGridAlignment");
			this.lblCustomGridAlignment.Name = "lblCustomGridAlignment";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlCustomGridAlignment);
			this.Controls.Add(this.pnlGridAlignmentSeparator);
			this.Controls.Add(this.pnlGridAlignment);
			this.Controls.Add(this.pnlCustomMeasureUnitSeparator);
			this.Controls.Add(this.pnlCustomMeasureUnit);
			this.Controls.Add(this.pnlMeasureUnitsSeparator);
			this.Controls.Add(this.pnlMeasureUnit);
			this.Controls.Add(this.pnlInnerSeparator);
			this.Name = "AxisNumericScaleOptionsControl";
			this.Controls.SetChildIndex(this.pnlInnerSeparator, 0);
			this.Controls.SetChildIndex(this.pnlMeasureUnit, 0);
			this.Controls.SetChildIndex(this.pnlMeasureUnitsSeparator, 0);
			this.Controls.SetChildIndex(this.pnlCustomMeasureUnit, 0);
			this.Controls.SetChildIndex(this.pnlCustomMeasureUnitSeparator, 0);
			this.Controls.SetChildIndex(this.pnlGridAlignment, 0);
			this.Controls.SetChildIndex(this.pnlGridAlignmentSeparator, 0);
			this.Controls.SetChildIndex(this.pnlCustomGridAlignment, 0);
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnit)).EndInit();
			this.pnlMeasureUnit.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbMeasureUnit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnitsSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomMeasureUnit)).EndInit();
			this.pnlCustomMeasureUnit.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnCustomMeasureUnit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomMeasureUnitSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignment)).EndInit();
			this.pnlGridAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbGridAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignmentSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCustomGridAlignment)).EndInit();
			this.pnlCustomGridAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnCustomGridAlignment.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlInnerSeparator;
		private ChartPanelControl pnlMeasureUnit;
		private XtraEditors.ComboBoxEdit cmbMeasureUnit;
		protected ChartLabelControl lblMeasureUnit;
		private ChartPanelControl pnlMeasureUnitsSeparator;
		private ChartPanelControl pnlCustomMeasureUnit;
		protected ChartLabelControl lblCustomMeasureUnit;
		private ChartPanelControl pnlCustomMeasureUnitSeparator;
		private ChartPanelControl pnlGridAlignment;
		private XtraEditors.ComboBoxEdit cmbGridAlignment;
		protected ChartLabelControl lblGridAlignment;
		private ChartPanelControl pnlGridAlignmentSeparator;
		private ChartPanelControl pnlCustomGridAlignment;
		protected ChartLabelControl lblCustomGridAlignment;
		private XtraEditors.SpinEdit spnCustomMeasureUnit;
		private XtraEditors.SpinEdit spnCustomGridAlignment;
	}
}
