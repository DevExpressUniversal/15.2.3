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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class AxisScrollBarOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisScrollBarOptionsControl));
			this.ceVisibleScrollBar = new DevExpress.XtraEditors.CheckEdit();
			this.lbPosition = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.cbPosition = new DevExpress.XtraCharts.Wizard.ChartComboBox();
			this.pnlPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.ceVisibleScrollBar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).BeginInit();
			this.pnlPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.ceVisibleScrollBar, "ceVisibleScrollBar");
			this.ceVisibleScrollBar.Name = "ceVisibleScrollBar";
			this.ceVisibleScrollBar.Properties.Caption = resources.GetString("ceVisibleScrollBar.Properties.Caption");
			this.ceVisibleScrollBar.CheckedChanged += new System.EventHandler(this.ceVisibleScrollBar_CheckedChanged);
			resources.ApplyResources(this.lbPosition, "lbPosition");
			this.lbPosition.Name = "lbPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPosition.Properties.Items"),
			resources.GetString("cbPosition.Properties.Items1")});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.pnlPosition, "pnlPosition");
			this.pnlPosition.BackColor = System.Drawing.Color.Transparent;
			this.pnlPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPosition.Controls.Add(this.cbPosition);
			this.pnlPosition.Controls.Add(this.lbPosition);
			this.pnlPosition.Name = "pnlPosition";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlPosition);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.ceVisibleScrollBar);
			this.Name = "AxisScrollBarOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.ceVisibleScrollBar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).EndInit();
			this.pnlPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit ceVisibleScrollBar;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lbPosition;
		private DevExpress.XtraCharts.Wizard.ChartComboBox cbPosition;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlPosition;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl2;
		private DevExpress.XtraEditors.LabelControl labelControl1;
	}
}
