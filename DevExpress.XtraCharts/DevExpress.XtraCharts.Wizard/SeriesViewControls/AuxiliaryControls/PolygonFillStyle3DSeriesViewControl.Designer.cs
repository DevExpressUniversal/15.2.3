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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class PolygonFillStyle3DSeriesViewControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PolygonFillStyle3DSeriesViewControl));
			this.pnlControls = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbFillMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbGradientMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.colorEdit = new DevExpress.XtraEditors.ColorEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbFillMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbGradientMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlControls, "pnlControls");
			this.pnlControls.BackColor = System.Drawing.Color.Transparent;
			this.pnlControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControls.Name = "pnlControls";
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.cbFillMode);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.cbFillMode, "cbFillMode");
			this.cbFillMode.Name = "cbFillMode";
			this.cbFillMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFillMode.Properties.Buttons"))))});
			this.cbFillMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbFillMode.Properties.Items"),
			resources.GetString("cbFillMode.Properties.Items1"),
			resources.GetString("cbFillMode.Properties.Items2")});
			this.cbFillMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFillMode.SelectedIndexChanged += new System.EventHandler(this.cbFillMode_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.cbGradientMode);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.cbGradientMode, "cbGradientMode");
			this.cbGradientMode.Name = "cbGradientMode";
			this.cbGradientMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbGradientMode.Properties.Buttons"))))});
			this.cbGradientMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbGradientMode.Properties.Items"),
			resources.GetString("cbGradientMode.Properties.Items1"),
			resources.GetString("cbGradientMode.Properties.Items2"),
			resources.GetString("cbGradientMode.Properties.Items3"),
			resources.GetString("cbGradientMode.Properties.Items4"),
			resources.GetString("cbGradientMode.Properties.Items5"),
			resources.GetString("cbGradientMode.Properties.Items6"),
			resources.GetString("cbGradientMode.Properties.Items7"),
			resources.GetString("cbGradientMode.Properties.Items8"),
			resources.GetString("cbGradientMode.Properties.Items9")});
			this.cbGradientMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbGradientMode.SelectedIndexChanged += new System.EventHandler(this.cbGradientMode_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.colorEdit);
			this.chartPanelControl2.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.colorEdit, "colorEdit");
			this.colorEdit.Name = "colorEdit";
			this.colorEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit.Properties.Buttons"))))});
			this.colorEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.colorEdit.EditValueChanged += new System.EventHandler(this.colorEdit_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.panelControl4);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.panelControl3);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.pnlControls);
			this.Name = "PolygonFillStyle3DSeriesViewControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbFillMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbGradientMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlControls;
		private ChartPanelControl chartPanelControl4;
		protected DevExpress.XtraEditors.ComboBoxEdit cbFillMode;
		private ChartLabelControl chartLabelControl3;
		private ChartPanelControl chartPanelControl1;
		protected DevExpress.XtraEditors.ComboBoxEdit cbGradientMode;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl2;
		protected DevExpress.XtraEditors.ColorEdit colorEdit;
		private ChartLabelControl chartLabelControl2;
		protected ChartPanelControl panelControl3;
		private ChartPanelControl panelControl4;
	}
}
