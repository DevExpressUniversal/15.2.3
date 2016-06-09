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
	partial class ChartPanesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartPanesControl));
			this.pnlPanes = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnl = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPanes = new DevExpress.XtraCharts.Wizard.ChartComboBox();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlTab = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPanes)).BeginInit();
			this.pnlPanes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPanes.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).BeginInit();
			this.SuspendLayout();
			this.splitter.Panel2.Controls.Add(this.pnlTab);
			this.splitter.Panel2.Controls.Add(this.pnlPanes);
			resources.ApplyResources(this.pnlPanes, "pnlPanes");
			this.pnlPanes.BackColor = System.Drawing.Color.Transparent;
			this.pnlPanes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPanes.Controls.Add(this.pnl);
			this.pnlPanes.Controls.Add(this.cbPanes);
			this.pnlPanes.Controls.Add(this.chartPanelControl2);
			this.pnlPanes.Name = "pnlPanes";
			this.pnl.BackColor = System.Drawing.Color.Transparent;
			this.pnl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnl, "pnl");
			this.pnl.Name = "pnl";
			resources.ApplyResources(this.cbPanes, "cbPanes");
			this.cbPanes.Name = "cbPanes";
			this.cbPanes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPanes.Properties.Buttons"))))});
			this.cbPanes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPanes.SelectedIndexChanged += new System.EventHandler(this.cbPanes_SelectedIndexChanged);
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			this.pnlTab.BackColor = System.Drawing.Color.Transparent;
			this.pnlTab.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlTab, "pnlTab");
			this.pnlTab.Name = "pnlTab";
			resources.ApplyResources(this, "$this");
			this.Name = "ChartPanesControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPanes)).EndInit();
			this.pnlPanes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPanes.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl pnlPanes;
		private ChartPanelControl pnl;
		private ChartComboBox cbPanes;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl pnlTab; 
	}
}
