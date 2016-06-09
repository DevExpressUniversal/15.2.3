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
	partial class XYDiagramSeriesViewLinkedElementsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XYDiagramSeriesViewLinkedElementsControl));
			this.panelAxisX = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAxisX = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAxisX = new DevExpress.XtraEditors.LabelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelAxisY = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAxisY = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAxisY = new DevExpress.XtraEditors.LabelControl();
			this.panelPane = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPane = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPane = new DevExpress.XtraEditors.LabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelAxisX)).BeginInit();
			this.panelAxisX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAxisX.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAxisY)).BeginInit();
			this.panelAxisY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAxisY.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPane)).BeginInit();
			this.panelPane.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPane.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelAxisX, "panelAxisX");
			this.panelAxisX.BackColor = System.Drawing.Color.Transparent;
			this.panelAxisX.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelAxisX.Controls.Add(this.cbAxisX);
			this.panelAxisX.Controls.Add(this.lblAxisX);
			this.panelAxisX.Name = "panelAxisX";
			resources.ApplyResources(this.cbAxisX, "cbAxisX");
			this.cbAxisX.Name = "cbAxisX";
			this.cbAxisX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAxisX.Properties.Buttons"))))});
			this.cbAxisX.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAxisX.SelectedIndexChanged += new System.EventHandler(this.cbAxisX_SelectedIndexChanged);
			resources.ApplyResources(this.lblAxisX, "lblAxisX");
			this.lblAxisX.Name = "lblAxisX";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.panelAxisY, "panelAxisY");
			this.panelAxisY.BackColor = System.Drawing.Color.Transparent;
			this.panelAxisY.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelAxisY.Controls.Add(this.cbAxisY);
			this.panelAxisY.Controls.Add(this.lblAxisY);
			this.panelAxisY.Name = "panelAxisY";
			resources.ApplyResources(this.cbAxisY, "cbAxisY");
			this.cbAxisY.Name = "cbAxisY";
			this.cbAxisY.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAxisY.Properties.Buttons"))))});
			this.cbAxisY.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAxisY.SelectedIndexChanged += new System.EventHandler(this.cbAxisY_SelectedIndexChanged);
			resources.ApplyResources(this.lblAxisY, "lblAxisY");
			this.lblAxisY.Name = "lblAxisY";
			resources.ApplyResources(this.panelPane, "panelPane");
			this.panelPane.BackColor = System.Drawing.Color.Transparent;
			this.panelPane.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelPane.Controls.Add(this.cbPane);
			this.panelPane.Controls.Add(this.lblPane);
			this.panelPane.Name = "panelPane";
			resources.ApplyResources(this.cbPane, "cbPane");
			this.cbPane.Name = "cbPane";
			this.cbPane.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPane.Properties.Buttons"))))});
			this.cbPane.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPane.SelectedIndexChanged += new System.EventHandler(this.cbPane_SelectedIndexChanged);
			resources.ApplyResources(this.lblPane, "lblPane");
			this.lblPane.Name = "lblPane";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelPane);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.panelAxisY);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.panelAxisX);
			this.Name = "XYDiagramSeriesViewLinkedElementsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelAxisX)).EndInit();
			this.panelAxisX.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAxisX.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAxisY)).EndInit();
			this.panelAxisY.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAxisY.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPane)).EndInit();
			this.panelPane.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPane.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelAxisX;
		private DevExpress.XtraEditors.LabelControl lblAxisX;
		private DevExpress.XtraEditors.ComboBoxEdit cbAxisX;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelControl2;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelAxisY;
		private DevExpress.XtraEditors.ComboBoxEdit cbAxisY;
		private DevExpress.XtraEditors.LabelControl lblAxisY;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelPane;
		private DevExpress.XtraEditors.ComboBoxEdit cbPane;
		private DevExpress.XtraEditors.LabelControl lblPane;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl2;
	}
}
