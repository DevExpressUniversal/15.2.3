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
	partial class SeriesLabelsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesLabelsControl));
			this.tabPanel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl15 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl12 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chSeries = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).BeginInit();
			this.panelControl12.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chSeries.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanel, "chartPanel");
			this.splitter.Panel2.Controls.Add(this.tabPanel);
			this.splitter.Panel2.Controls.Add(this.panelControl15);
			this.splitter.Panel2.Controls.Add(this.panelControl12);
			resources.ApplyResources(this.splitter, "splitter");
			this.splitter.SplitterPosition = 295;
			this.tabPanel.BackColor = System.Drawing.Color.Transparent;
			this.tabPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.tabPanel, "tabPanel");
			this.tabPanel.Name = "tabPanel";
			this.panelControl15.BackColor = System.Drawing.Color.Transparent;
			this.panelControl15.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl15, "panelControl15");
			this.panelControl15.Name = "panelControl15";
			resources.ApplyResources(this.panelControl12, "panelControl12");
			this.panelControl12.BackColor = System.Drawing.Color.Transparent;
			this.panelControl12.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl12.Controls.Add(this.chSeries);
			this.panelControl12.Controls.Add(this.chartPanelControl1);
			this.panelControl12.Name = "panelControl12";
			resources.ApplyResources(this.chSeries, "chSeries");
			this.chSeries.Name = "chSeries";
			this.chSeries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("chSeries.Properties.Buttons"))))});
			this.chSeries.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.chSeries.SelectedIndexChanged += new System.EventHandler(this.chSeries_SelectedIndexChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this, "$this");
			this.Name = "SeriesLabelsControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tabPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).EndInit();
			this.panelControl12.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chSeries.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl tabPanel;
		private ChartPanelControl panelControl15;
		private ChartPanelControl panelControl12;
		private DevExpress.XtraEditors.ComboBoxEdit chSeries;
		private ChartPanelControl chartPanelControl1;
	}
}
