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
	partial class ConnectorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectorControl));
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lineStyleControl1 = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlLineSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtLineLength = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceLineColor = new DevExpress.XtraEditors.ColorEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLineSettings)).BeginInit();
			this.pnlLineSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtLineLength.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceLineColor.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.AllowGrayed = true;
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckStateChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.lineStyleControl1, "lineStyleControl1");
			this.lineStyleControl1.Name = "lineStyleControl1";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.pnlLineSettings, "pnlLineSettings");
			this.pnlLineSettings.BackColor = System.Drawing.Color.Transparent;
			this.pnlLineSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlLineSettings.Controls.Add(this.chartPanelControl2);
			this.pnlLineSettings.Controls.Add(this.chartPanelControl3);
			this.pnlLineSettings.Controls.Add(this.chartPanelControl4);
			this.pnlLineSettings.Name = "pnlLineSettings";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.txtLineLength);
			this.chartPanelControl2.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.txtLineLength, "txtLineLength");
			this.txtLineLength.Name = "txtLineLength";
			this.txtLineLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtLineLength.Properties.IsFloatValue = false;
			this.txtLineLength.Properties.Mask.EditMask = resources.GetString("txtLineLength.Properties.Mask.EditMask");
			this.txtLineLength.Properties.MaxValue = new decimal(new int[] {
			200,
			0,
			0,
			0});
			this.txtLineLength.EditValueChanged += new System.EventHandler(this.txtLineLength_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.ceLineColor);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.ceLineColor, "ceLineColor");
			this.ceLineColor.Name = "ceLineColor";
			this.ceLineColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceLineColor.Properties.Buttons"))))});
			this.ceLineColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceLineColor.EditValueChanged += new System.EventHandler(this.ceLineColor_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlLineSettings);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.lineStyleControl1);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.chVisible);
			this.Name = "ConnectorControl";
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLineSettings)).EndInit();
			this.pnlLineSettings.ResumeLayout(false);
			this.pnlLineSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtLineLength.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceLineColor.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelControl2;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl lineStyleControl1;
		private ChartPanelControl panelControl1;
		private ChartPanelControl pnlLineSettings;
		private ChartPanelControl chartPanelControl2;
		private DevExpress.XtraEditors.SpinEdit txtLineLength;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.ColorEdit ceLineColor;
		private ChartLabelControl chartLabelControl3;
		protected DevExpress.XtraEditors.CheckEdit chVisible;
		private DevExpress.XtraEditors.LabelControl labelControl1;
	}
}
