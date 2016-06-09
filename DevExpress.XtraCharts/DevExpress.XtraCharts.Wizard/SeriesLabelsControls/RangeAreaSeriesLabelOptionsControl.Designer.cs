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

namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	partial class RangeAreaSeriesLabelOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RangeAreaSeriesLabelOptionsControl));
			this.pnlKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelKind = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlMinMaxAngles = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMinValueAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnMinValueAngle = new DevExpress.XtraEditors.SpinEdit();
			this.lblMinValueAngle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.separator1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxValueAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnMaxValueAngle = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxValueAngle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.separator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).BeginInit();
			this.pnlKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinMaxAngles)).BeginInit();
			this.pnlMinMaxAngles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValueAngle)).BeginInit();
			this.pnlMinValueAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnMinValueAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValueAngle)).BeginInit();
			this.pnlMaxValueAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnMaxValueAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlKind, "pnlKind");
			this.pnlKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlKind.Controls.Add(this.cbKind);
			this.pnlKind.Controls.Add(this.labelKind);
			this.pnlKind.Name = "pnlKind";
			resources.ApplyResources(this.cbKind, "cbKind");
			this.cbKind.Name = "cbKind";
			this.cbKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbKind.Properties.Buttons"))))});
			this.cbKind.Properties.Items.AddRange(new object[] {
			resources.GetString("cbKind.Properties.Items"),
			resources.GetString("cbKind.Properties.Items1"),
			resources.GetString("cbKind.Properties.Items2"),
			resources.GetString("cbKind.Properties.Items3"),
			resources.GetString("cbKind.Properties.Items4"),
			resources.GetString("cbKind.Properties.Items5")});
			this.cbKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbKind.SelectedIndexChanged += new System.EventHandler(this.cbKind_SelectedIndexChanged);
			resources.ApplyResources(this.labelKind, "labelKind");
			this.labelKind.Name = "labelKind";
			resources.ApplyResources(this.pnlMinMaxAngles, "pnlMinMaxAngles");
			this.pnlMinMaxAngles.BackColor = System.Drawing.Color.Transparent;
			this.pnlMinMaxAngles.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMinMaxAngles.Controls.Add(this.pnlMinValueAngle);
			this.pnlMinMaxAngles.Controls.Add(this.separator1);
			this.pnlMinMaxAngles.Controls.Add(this.pnlMaxValueAngle);
			this.pnlMinMaxAngles.Controls.Add(this.separator);
			this.pnlMinMaxAngles.Name = "pnlMinMaxAngles";
			resources.ApplyResources(this.pnlMinValueAngle, "pnlMinValueAngle");
			this.pnlMinValueAngle.BackColor = System.Drawing.Color.Transparent;
			this.pnlMinValueAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMinValueAngle.Controls.Add(this.spnMinValueAngle);
			this.pnlMinValueAngle.Controls.Add(this.lblMinValueAngle);
			this.pnlMinValueAngle.Name = "pnlMinValueAngle";
			resources.ApplyResources(this.spnMinValueAngle, "spnMinValueAngle");
			this.spnMinValueAngle.Name = "spnMinValueAngle";
			this.spnMinValueAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnMinValueAngle.Properties.IsFloatValue = false;
			this.spnMinValueAngle.Properties.Mask.EditMask = resources.GetString("spnMinValueAngle.Properties.Mask.EditMask");
			this.spnMinValueAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.spnMinValueAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.spnMinValueAngle.EditValueChanged += new System.EventHandler(this.spnMinValueAngle_EditValueChanged);
			resources.ApplyResources(this.lblMinValueAngle, "lblMinValueAngle");
			this.lblMinValueAngle.Name = "lblMinValueAngle";
			this.separator1.BackColor = System.Drawing.Color.Transparent;
			this.separator1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator1, "separator1");
			this.separator1.Name = "separator1";
			resources.ApplyResources(this.pnlMaxValueAngle, "pnlMaxValueAngle");
			this.pnlMaxValueAngle.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxValueAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxValueAngle.Controls.Add(this.spnMaxValueAngle);
			this.pnlMaxValueAngle.Controls.Add(this.lblMaxValueAngle);
			this.pnlMaxValueAngle.Name = "pnlMaxValueAngle";
			resources.ApplyResources(this.spnMaxValueAngle, "spnMaxValueAngle");
			this.spnMaxValueAngle.Name = "spnMaxValueAngle";
			this.spnMaxValueAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnMaxValueAngle.Properties.IsFloatValue = false;
			this.spnMaxValueAngle.Properties.Mask.EditMask = resources.GetString("spnMaxValueAngle.Properties.Mask.EditMask");
			this.spnMaxValueAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.spnMaxValueAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.spnMaxValueAngle.EditValueChanged += new System.EventHandler(this.spnMaxValueAngle_EditValueChanged);
			resources.ApplyResources(this.lblMaxValueAngle, "lblMaxValueAngle");
			this.lblMaxValueAngle.Name = "lblMaxValueAngle";
			this.separator.BackColor = System.Drawing.Color.Transparent;
			this.separator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator, "separator");
			this.separator.Name = "separator";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlMinMaxAngles);
			this.Controls.Add(this.pnlKind);
			this.Name = "RangeAreaSeriesLabelOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).EndInit();
			this.pnlKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinMaxAngles)).EndInit();
			this.pnlMinMaxAngles.ResumeLayout(false);
			this.pnlMinMaxAngles.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValueAngle)).EndInit();
			this.pnlMinValueAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnMinValueAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValueAngle)).EndInit();
			this.pnlMaxValueAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnMaxValueAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlKind;
		private DevExpress.XtraEditors.ComboBoxEdit cbKind;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl labelKind;
		private ChartPanelControl pnlMinMaxAngles;
		private ChartPanelControl pnlMinValueAngle;
		private DevExpress.XtraEditors.SpinEdit spnMinValueAngle;
		private ChartLabelControl lblMinValueAngle;
		private ChartPanelControl separator1;
		private ChartPanelControl pnlMaxValueAngle;
		private DevExpress.XtraEditors.SpinEdit spnMaxValueAngle;
		private ChartLabelControl lblMaxValueAngle;
		private ChartPanelControl separator;
	}
}
