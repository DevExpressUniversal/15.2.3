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

namespace DevExpress.XtraCharts.Wizard {
	partial class AxisDateTimeScaleOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisDateTimeScaleOptionsControl));
			this.pnlMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbMeasureUnit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblMeasureUnit = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlMeasureUnitsSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlGridAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbGridAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblGridAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlInnerSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnit)).BeginInit();
			this.pnlMeasureUnit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbMeasureUnit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnitsSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignment)).BeginInit();
			this.pnlGridAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbGridAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerSeparator)).BeginInit();
			this.SuspendLayout();
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
			this.pnlInnerSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlInnerSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlInnerSeparator, "pnlInnerSeparator");
			this.pnlInnerSeparator.Name = "pnlInnerSeparator";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlGridAlignment);
			this.Controls.Add(this.pnlMeasureUnitsSeparator);
			this.Controls.Add(this.pnlMeasureUnit);
			this.Controls.Add(this.pnlInnerSeparator);
			this.Name = "AxisDateTimeScaleOptionsControl";
			this.Controls.SetChildIndex(this.pnlInnerSeparator, 0);
			this.Controls.SetChildIndex(this.pnlMeasureUnit, 0);
			this.Controls.SetChildIndex(this.pnlMeasureUnitsSeparator, 0);
			this.Controls.SetChildIndex(this.pnlGridAlignment, 0);
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnit)).EndInit();
			this.pnlMeasureUnit.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbMeasureUnit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMeasureUnitsSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridAlignment)).EndInit();
			this.pnlGridAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbGridAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerSeparator)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlMeasureUnit;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlMeasureUnitsSeparator;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlGridAlignment;
		private DevExpress.XtraEditors.ComboBoxEdit cmbMeasureUnit;
		protected DevExpress.XtraCharts.Wizard.ChartLabelControl lblMeasureUnit;
		protected DevExpress.XtraCharts.Wizard.ChartLabelControl lblGridAlignment;
		private DevExpress.XtraEditors.ComboBoxEdit cmbGridAlignment;
		private ChartPanelControl pnlInnerSeparator;
	}
}
