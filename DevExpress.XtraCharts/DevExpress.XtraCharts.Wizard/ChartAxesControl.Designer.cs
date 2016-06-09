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
	partial class ChartAxesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.tabPanel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAxes = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnl = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAxes = new DevExpress.XtraCharts.Wizard.ChartComboBox();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAxes)).BeginInit();
			this.pnlAxes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAxes.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.SuspendLayout();
			this.chartPanel.Size = new System.Drawing.Size(353, 508);
			this.chartPanel.TabIndex = 0;
			this.splitter.Panel2.Controls.Add(this.tabPanel);
			this.splitter.Panel2.Controls.Add(this.pnlAxes);
			this.splitter.Size = new System.Drawing.Size(691, 511);
			this.splitter.SplitterPosition = 355;
			this.tabPanel.BackColor = System.Drawing.Color.Transparent;
			this.tabPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.tabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabPanel.Location = new System.Drawing.Point(2, 27);
			this.tabPanel.Name = "tabPanel";
			this.tabPanel.Size = new System.Drawing.Size(329, 484);
			this.tabPanel.TabIndex = 1;
			this.pnlAxes.AutoSize = true;
			this.pnlAxes.BackColor = System.Drawing.Color.Transparent;
			this.pnlAxes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAxes.Controls.Add(this.pnl);
			this.pnlAxes.Controls.Add(this.cbAxes);
			this.pnlAxes.Controls.Add(this.chartPanelControl1);
			this.pnlAxes.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlAxes.Location = new System.Drawing.Point(2, 0);
			this.pnlAxes.Name = "pnlAxes";
			this.pnlAxes.Size = new System.Drawing.Size(329, 27);
			this.pnlAxes.TabIndex = 0;
			this.pnl.BackColor = System.Drawing.Color.Transparent;
			this.pnl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnl.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl.Location = new System.Drawing.Point(0, 20);
			this.pnl.Name = "pnl";
			this.pnl.Size = new System.Drawing.Size(304, 7);
			this.pnl.TabIndex = 10;
			this.cbAxes.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbAxes.Location = new System.Drawing.Point(0, 0);
			this.cbAxes.Name = "cbAxes";
			this.cbAxes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbAxes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAxes.Size = new System.Drawing.Size(304, 20);
			this.cbAxes.TabIndex = 0;
			this.cbAxes.SelectedIndexChanged += new System.EventHandler(this.cbAxes_SelectedIndexChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Dock = System.Windows.Forms.DockStyle.Right;
			this.chartPanelControl1.Location = new System.Drawing.Point(304, 0);
			this.chartPanelControl1.Name = "chartPanelControl1";
			this.chartPanelControl1.Size = new System.Drawing.Size(25, 27);
			this.chartPanelControl1.TabIndex = 11;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "ChartAxesControl";
			this.Size = new System.Drawing.Size(691, 511);
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAxes)).EndInit();
			this.pnlAxes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAxes.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl tabPanel;
		private ChartPanelControl pnlAxes;
		private ChartComboBox cbAxes;
		private ChartPanelControl pnl;
		private ChartPanelControl chartPanelControl1;
	}
}
