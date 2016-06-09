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
	partial class AxisScaleOptionsControlBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisScaleOptionsControlBase));
			this.pnlScaleMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbScaleMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblScaleMode = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlScaleModeSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAggregateFunction = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbAggregateFunction = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAggregateFunction = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.grpGrid = new DevExpress.XtraEditors.GroupControl();
			this.sepGridAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlGridOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnGridOffset = new DevExpress.XtraEditors.SpinEdit();
			this.lblGridOffset = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlGridSpacing = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnGridSpacing = new DevExpress.XtraEditors.SpinEdit();
			this.lblGridSpacing = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAuto = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAutoGrid = new DevExpress.XtraEditors.CheckEdit();
			this.sepGrid = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlScaleMode)).BeginInit();
			this.pnlScaleMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbScaleMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlScaleModeSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAggregateFunction)).BeginInit();
			this.pnlAggregateFunction.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbAggregateFunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpGrid)).BeginInit();
			this.grpGrid.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepGridAlignment)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridOffset)).BeginInit();
			this.pnlGridOffset.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnGridOffset.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridSpacing)).BeginInit();
			this.pnlGridSpacing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAuto)).BeginInit();
			this.pnlAuto.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chAutoGrid.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGrid)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlScaleMode, "pnlScaleMode");
			this.pnlScaleMode.BackColor = System.Drawing.Color.Transparent;
			this.pnlScaleMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlScaleMode.Controls.Add(this.cmbScaleMode);
			this.pnlScaleMode.Controls.Add(this.lblScaleMode);
			this.pnlScaleMode.Name = "pnlScaleMode";
			resources.ApplyResources(this.cmbScaleMode, "cmbScaleMode");
			this.cmbScaleMode.Name = "cmbScaleMode";
			this.cmbScaleMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbScaleMode.Properties.Buttons"))))});
			this.cmbScaleMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbScaleMode.SelectedIndexChanged += new System.EventHandler(this.cmbScaleMode_SelectedIndexChanged);
			resources.ApplyResources(this.lblScaleMode, "lblScaleMode");
			this.lblScaleMode.Name = "lblScaleMode";
			this.pnlScaleModeSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlScaleModeSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlScaleModeSeparator, "pnlScaleModeSeparator");
			this.pnlScaleModeSeparator.Name = "pnlScaleModeSeparator";
			resources.ApplyResources(this.pnlAggregateFunction, "pnlAggregateFunction");
			this.pnlAggregateFunction.BackColor = System.Drawing.Color.Transparent;
			this.pnlAggregateFunction.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAggregateFunction.Controls.Add(this.cmbAggregateFunction);
			this.pnlAggregateFunction.Controls.Add(this.lblAggregateFunction);
			this.pnlAggregateFunction.Name = "pnlAggregateFunction";
			resources.ApplyResources(this.cmbAggregateFunction, "cmbAggregateFunction");
			this.cmbAggregateFunction.Name = "cmbAggregateFunction";
			this.cmbAggregateFunction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbAggregateFunction.Properties.Buttons"))))});
			this.cmbAggregateFunction.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbAggregateFunction.SelectedIndexChanged += new System.EventHandler(this.cmbAggregateFunction_SelectedIndexChanged);
			resources.ApplyResources(this.lblAggregateFunction, "lblAggregateFunction");
			this.lblAggregateFunction.Name = "lblAggregateFunction";
			resources.ApplyResources(this.grpGrid, "grpGrid");
			this.grpGrid.Controls.Add(this.sepGridAlignment);
			this.grpGrid.Controls.Add(this.pnlGridOffset);
			this.grpGrid.Controls.Add(this.sepOffset);
			this.grpGrid.Controls.Add(this.pnlGridSpacing);
			this.grpGrid.Controls.Add(this.sepValue);
			this.grpGrid.Controls.Add(this.pnlAuto);
			this.grpGrid.Name = "grpGrid";
			this.sepGridAlignment.BackColor = System.Drawing.Color.Transparent;
			this.sepGridAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepGridAlignment, "sepGridAlignment");
			this.sepGridAlignment.Name = "sepGridAlignment";
			resources.ApplyResources(this.pnlGridOffset, "pnlGridOffset");
			this.pnlGridOffset.BackColor = System.Drawing.Color.Transparent;
			this.pnlGridOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGridOffset.Controls.Add(this.spnGridOffset);
			this.pnlGridOffset.Controls.Add(this.lblGridOffset);
			this.pnlGridOffset.Name = "pnlGridOffset";
			resources.ApplyResources(this.spnGridOffset, "spnGridOffset");
			this.spnGridOffset.Name = "spnGridOffset";
			this.spnGridOffset.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnGridOffset.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnGridOffset.Properties.MinValue = new decimal(new int[] {
			10000,
			0,
			0,
			-2147483648});
			this.spnGridOffset.Properties.ValidateOnEnterKey = true;
			this.spnGridOffset.EditValueChanged += new System.EventHandler(this.spnGridOffset_EditValueChanged);
			resources.ApplyResources(this.lblGridOffset, "lblGridOffset");
			this.lblGridOffset.Name = "lblGridOffset";
			this.sepOffset.BackColor = System.Drawing.Color.Transparent;
			this.sepOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepOffset, "sepOffset");
			this.sepOffset.Name = "sepOffset";
			resources.ApplyResources(this.pnlGridSpacing, "pnlGridSpacing");
			this.pnlGridSpacing.BackColor = System.Drawing.Color.Transparent;
			this.pnlGridSpacing.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGridSpacing.Controls.Add(this.spnGridSpacing);
			this.pnlGridSpacing.Controls.Add(this.lblGridSpacing);
			this.pnlGridSpacing.Name = "pnlGridSpacing";
			resources.ApplyResources(this.spnGridSpacing, "spnGridSpacing");
			this.spnGridSpacing.Name = "spnGridSpacing";
			this.spnGridSpacing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnGridSpacing.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnGridSpacing.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnGridSpacing.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnGridSpacing.Properties.ValidateOnEnterKey = true;
			this.spnGridSpacing.EditValueChanged += new System.EventHandler(this.spnGridSpacing_EditValueChanged);
			resources.ApplyResources(this.lblGridSpacing, "lblGridSpacing");
			this.lblGridSpacing.Name = "lblGridSpacing";
			this.sepValue.BackColor = System.Drawing.Color.Transparent;
			this.sepValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepValue, "sepValue");
			this.sepValue.Name = "sepValue";
			resources.ApplyResources(this.pnlAuto, "pnlAuto");
			this.pnlAuto.BackColor = System.Drawing.Color.Transparent;
			this.pnlAuto.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAuto.Controls.Add(this.chAutoGrid);
			this.pnlAuto.Name = "pnlAuto";
			resources.ApplyResources(this.chAutoGrid, "chAutoGrid");
			this.chAutoGrid.Name = "chAutoGrid";
			this.chAutoGrid.Properties.Caption = resources.GetString("chAutoGrid.Properties.Caption");
			this.chAutoGrid.CheckedChanged += new System.EventHandler(this.chGridSpacingAuto_CheckedChanged);
			this.sepGrid.BackColor = System.Drawing.Color.Transparent;
			this.sepGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepGrid, "sepGrid");
			this.sepGrid.Name = "sepGrid";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpGrid);
			this.Controls.Add(this.sepGrid);
			this.Controls.Add(this.pnlAggregateFunction);
			this.Controls.Add(this.pnlScaleModeSeparator);
			this.Controls.Add(this.pnlScaleMode);
			this.Name = "AxisScaleOptionsControlBase";
			((System.ComponentModel.ISupportInitialize)(this.pnlScaleMode)).EndInit();
			this.pnlScaleMode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbScaleMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlScaleModeSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAggregateFunction)).EndInit();
			this.pnlAggregateFunction.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbAggregateFunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpGrid)).EndInit();
			this.grpGrid.ResumeLayout(false);
			this.grpGrid.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepGridAlignment)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridOffset)).EndInit();
			this.pnlGridOffset.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnGridOffset.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridSpacing)).EndInit();
			this.pnlGridSpacing.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnGridSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAuto)).EndInit();
			this.pnlAuto.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chAutoGrid.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlScaleMode;
		private XtraEditors.ComboBoxEdit cmbScaleMode;
		protected ChartLabelControl lblScaleMode;
		private ChartPanelControl pnlScaleModeSeparator;
		private ChartPanelControl pnlAggregateFunction;
		private XtraEditors.ComboBoxEdit cmbAggregateFunction;
		protected ChartLabelControl lblAggregateFunction;
		private XtraEditors.GroupControl grpGrid;
		private ChartPanelControl sepOffset;
		private ChartPanelControl pnlGridOffset;
		private XtraEditors.SpinEdit spnGridOffset;
		private ChartLabelControl lblGridOffset;
		private ChartPanelControl pnlGridSpacing;
		protected XtraEditors.SpinEdit spnGridSpacing;
		private ChartLabelControl lblGridSpacing;
		private ChartPanelControl sepValue;
		private ChartPanelControl pnlAuto;
		private XtraEditors.CheckEdit chAutoGrid;
		private ChartPanelControl sepGrid;
		private ChartPanelControl sepGridAlignment;
	}
}
