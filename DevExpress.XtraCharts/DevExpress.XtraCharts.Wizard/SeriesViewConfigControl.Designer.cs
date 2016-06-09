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
	partial class SeriesViewConfigControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.panelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl9 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl10 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.controlsPanel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl11 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl12 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chSeries = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl15 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.tabPanel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.controlsPanel)).BeginInit();
			this.controlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).BeginInit();
			this.panelControl11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).BeginInit();
			this.panelControl12.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chSeries.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).BeginInit();
			this.SuspendLayout();
			this.chartPanel.Size = new System.Drawing.Size(341, 493);
			this.splitter.Panel2.Controls.Add(this.tabPanel);
			this.splitter.Panel2.Controls.Add(this.panelControl15);
			this.splitter.Panel2.Controls.Add(this.panelControl12);
			this.splitter.Size = new System.Drawing.Size(666, 496);
			this.splitter.SplitterPosition = 343;
			this.comboBoxEdit1.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxEdit1.EditValue = "";
			this.comboBoxEdit1.Location = new System.Drawing.Point(22, 0);
			this.comboBoxEdit1.Name = "comboBoxEdit1";
			this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxEdit1.Size = new System.Drawing.Size(293, 20);
			this.comboBoxEdit1.TabIndex = 2;
			this.comboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.cbSeries_SelectedIndexChanged);
			this.panelControl8.BackColor = System.Drawing.Color.Transparent;
			this.panelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl8.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelControl8.Location = new System.Drawing.Point(0, 0);
			this.panelControl8.Name = "panelControl8";
			this.panelControl8.Size = new System.Drawing.Size(22, 20);
			this.panelControl8.TabIndex = 4;
			this.panelControl9.BackColor = System.Drawing.Color.Transparent;
			this.panelControl9.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl9.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelControl9.Location = new System.Drawing.Point(315, 0);
			this.panelControl9.Name = "panelControl9";
			this.panelControl9.Size = new System.Drawing.Size(3, 20);
			this.panelControl9.TabIndex = 5;
			this.panelControl10.BackColor = System.Drawing.Color.Transparent;
			this.panelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl10.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl10.Location = new System.Drawing.Point(0, 20);
			this.panelControl10.Name = "panelControl10";
			this.panelControl10.Size = new System.Drawing.Size(318, 7);
			this.panelControl10.TabIndex = 7;
			this.controlsPanel.BackColor = System.Drawing.Color.Transparent;
			this.controlsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.controlsPanel.Controls.Add(this.panelControl10);
			this.controlsPanel.Controls.Add(this.panelControl11);
			this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlsPanel.Location = new System.Drawing.Point(0, 0);
			this.controlsPanel.Name = "controlsPanel";
			this.controlsPanel.Size = new System.Drawing.Size(318, 496);
			this.controlsPanel.TabIndex = 3;
			this.panelControl11.AutoSize = true;
			this.panelControl11.BackColor = System.Drawing.Color.Transparent;
			this.panelControl11.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl11.Controls.Add(this.comboBoxEdit1);
			this.panelControl11.Controls.Add(this.panelControl8);
			this.panelControl11.Controls.Add(this.panelControl9);
			this.panelControl11.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl11.Location = new System.Drawing.Point(0, 0);
			this.panelControl11.Name = "panelControl11";
			this.panelControl11.Size = new System.Drawing.Size(318, 20);
			this.panelControl11.TabIndex = 6;
			this.panelControl12.AutoSize = true;
			this.panelControl12.BackColor = System.Drawing.Color.Transparent;
			this.panelControl12.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl12.Controls.Add(this.chSeries);
			this.panelControl12.Controls.Add(this.chartPanelControl1);
			this.panelControl12.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl12.Location = new System.Drawing.Point(2, 0);
			this.panelControl12.Name = "panelControl12";
			this.panelControl12.Size = new System.Drawing.Size(316, 20);
			this.panelControl12.TabIndex = 0;
			this.chSeries.Dock = System.Windows.Forms.DockStyle.Top;
			this.chSeries.Location = new System.Drawing.Point(0, 0);
			this.chSeries.Name = "chSeries";
			this.chSeries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.chSeries.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.chSeries.Size = new System.Drawing.Size(291, 20);
			this.chSeries.TabIndex = 2;
			this.chSeries.SelectedIndexChanged += new System.EventHandler(this.cbSeries_SelectedIndexChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Dock = System.Windows.Forms.DockStyle.Right;
			this.chartPanelControl1.Location = new System.Drawing.Point(291, 0);
			this.chartPanelControl1.Name = "chartPanelControl1";
			this.chartPanelControl1.Size = new System.Drawing.Size(25, 20);
			this.chartPanelControl1.TabIndex = 3;
			this.panelControl15.BackColor = System.Drawing.Color.Transparent;
			this.panelControl15.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl15.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl15.Location = new System.Drawing.Point(2, 20);
			this.panelControl15.Name = "panelControl15";
			this.panelControl15.Size = new System.Drawing.Size(316, 7);
			this.panelControl15.TabIndex = 1;
			this.tabPanel.BackColor = System.Drawing.Color.Transparent;
			this.tabPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.tabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabPanel.Location = new System.Drawing.Point(2, 27);
			this.tabPanel.Name = "tabPanel";
			this.tabPanel.Size = new System.Drawing.Size(316, 469);
			this.tabPanel.TabIndex = 2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "SeriesViewConfigControl";
			this.Size = new System.Drawing.Size(666, 496);
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.controlsPanel)).EndInit();
			this.controlsPanel.ResumeLayout(false);
			this.controlsPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).EndInit();
			this.panelControl11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).EndInit();
			this.panelControl12.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chSeries.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl controlsPanel;
		private ChartPanelControl panelControl10;
		private ChartPanelControl panelControl11;
		private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
		private ChartPanelControl panelControl8;
		private ChartPanelControl panelControl9;
		private ChartPanelControl panelControl12;
		private DevExpress.XtraEditors.ComboBoxEdit chSeries;
		private ChartPanelControl tabPanel;
		private ChartPanelControl panelControl15;
		private ChartPanelControl chartPanelControl1;
	}
}
