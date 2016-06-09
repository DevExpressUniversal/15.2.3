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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	partial class Axis3DAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Axis3DAppearanceControl));
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.interlaceFillStyle = new DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyle3DSeriesViewControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlControls = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceInterlaceColor = new DevExpress.XtraEditors.ColorEdit();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEnableInterlace = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceInterlaceColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableInterlace.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Controls.Add(this.interlaceFillStyle);
			this.groupControl2.Controls.Add(this.chartPanelControl1);
			this.groupControl2.Controls.Add(this.pnlControls);
			this.groupControl2.Controls.Add(this.panelControl3);
			this.groupControl2.Controls.Add(this.chEnableInterlace);
			this.groupControl2.Name = "groupControl2";
			resources.ApplyResources(this.interlaceFillStyle, "interlaceFillStyle");
			this.interlaceFillStyle.Name = "interlaceFillStyle";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.pnlControls, "pnlControls");
			this.pnlControls.BackColor = System.Drawing.Color.Transparent;
			this.pnlControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControls.Controls.Add(this.ceInterlaceColor);
			this.pnlControls.Controls.Add(this.panelControl1);
			this.pnlControls.Name = "pnlControls";
			resources.ApplyResources(this.ceInterlaceColor, "ceInterlaceColor");
			this.ceInterlaceColor.Name = "ceInterlaceColor";
			this.ceInterlaceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceInterlaceColor.Properties.Buttons"))))});
			this.ceInterlaceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceInterlaceColor.EditValueChanged += new System.EventHandler(this.ceInterlaceColor_EditValueChanged);
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.chartLabelControl1);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.chEnableInterlace, "chEnableInterlace");
			this.chEnableInterlace.Name = "chEnableInterlace";
			this.chEnableInterlace.Properties.Caption = resources.GetString("chEnableInterlace.Properties.Caption");
			this.chEnableInterlace.CheckedChanged += new System.EventHandler(this.chEnableInterlace_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupControl2);
			this.Name = "Axis3DAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.groupControl2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceInterlaceColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableInterlace.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl groupControl2;
		private DevExpress.XtraEditors.CheckEdit chEnableInterlace;
		private ChartPanelControl pnlControls;
		private DevExpress.XtraEditors.ColorEdit ceInterlaceColor;
		private ChartPanelControl panelControl1;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyle3DSeriesViewControl interlaceFillStyle;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl panelControl3;
	}
}
