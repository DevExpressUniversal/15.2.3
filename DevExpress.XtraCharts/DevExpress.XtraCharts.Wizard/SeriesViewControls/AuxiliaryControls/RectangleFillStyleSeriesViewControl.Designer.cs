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
	partial class RectangleFillStyleSeriesViewControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RectangleFillStyleSeriesViewControl));
			this.chartPanelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbHatchStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbGradientMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.colorEdit = new DevExpress.XtraEditors.ColorEdit();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbFillMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).BeginInit();
			this.chartPanelControl8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbHatchStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			this.chartPanelControl6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbGradientMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbFillMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl8, "chartPanelControl8");
			this.chartPanelControl8.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl8.Controls.Add(this.cbHatchStyle);
			this.chartPanelControl8.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl8.Name = "chartPanelControl8";
			resources.ApplyResources(this.cbHatchStyle, "cbHatchStyle");
			this.cbHatchStyle.Name = "cbHatchStyle";
			this.cbHatchStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbHatchStyle.Properties.Buttons"))))});
			this.cbHatchStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbHatchStyle.SelectedValueChanged += new System.EventHandler(this.cbHatchStyle_SelectedItemChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Controls.Add(this.cbGradientMode);
			this.chartPanelControl6.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl6.Name = "chartPanelControl6";
			resources.ApplyResources(this.cbGradientMode, "cbGradientMode");
			this.cbGradientMode.Name = "cbGradientMode";
			this.cbGradientMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbGradientMode.Properties.Buttons"))))});
			this.cbGradientMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbGradientMode.SelectedIndexChanged += new System.EventHandler(this.cbGradientMode_SelectedIndexChanged);
			resources.ApplyResources(this.colorEdit, "colorEdit");
			this.colorEdit.Name = "colorEdit";
			this.colorEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit.Properties.Buttons"))))});
			this.colorEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.colorEdit.EditValueChanged += new System.EventHandler(this.colorEdit_EditValueChanged);
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.Name = "chartPanelControl5";
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
			resources.GetString("cbFillMode.Properties.Items2"),
			resources.GetString("cbFillMode.Properties.Items3")});
			this.cbFillMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFillMode.SelectedIndexChanged += new System.EventHandler(this.cbFillMode_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.colorEdit);
			this.chartPanelControl3.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl8);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl6);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl5);
			this.Controls.Add(this.chartPanelControl4);
			this.Name = "RectangleFillStyleSeriesViewControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).EndInit();
			this.chartPanelControl8.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbHatchStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			this.chartPanelControl6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbGradientMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbFillMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl chartPanelControl8;
		protected DevExpress.XtraEditors.ComboBoxEdit cbHatchStyle;
		private ChartLabelControl chartLabelControl4;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl chartPanelControl6;
		protected DevExpress.XtraEditors.ComboBoxEdit cbGradientMode;
		protected DevExpress.XtraEditors.ColorEdit colorEdit;
		protected ChartPanelControl chartPanelControl5;
		private ChartPanelControl chartPanelControl4;
		protected DevExpress.XtraEditors.ComboBoxEdit cbFillMode;
		private ChartLabelControl chartLabelControl3;
		protected ChartPanelControl chartPanelControl1;
		private ChartLabelControl chartLabelControl1;
		protected ChartPanelControl chartPanelControl2;
		private ChartPanelControl chartPanelControl3;
	}
}
