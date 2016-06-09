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
	partial class StripAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StripAppearanceControl));
			this.pnlEditors = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlControls = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbColor = new DevExpress.XtraEditors.ColorEdit();
			this.pnlLabels = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.fillStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyleSeriesViewControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLabels)).BeginInit();
			this.pnlLabels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlEditors, "pnlEditors");
			this.pnlEditors.BackColor = System.Drawing.Color.Transparent;
			this.pnlEditors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlEditors.Name = "pnlEditors";
			resources.ApplyResources(this.pnlControls, "pnlControls");
			this.pnlControls.BackColor = System.Drawing.Color.Transparent;
			this.pnlControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControls.Controls.Add(this.cbColor);
			this.pnlControls.Controls.Add(this.pnlLabels);
			this.pnlControls.Name = "pnlControls";
			resources.ApplyResources(this.cbColor, "cbColor");
			this.cbColor.Name = "cbColor";
			this.cbColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbColor.Properties.Buttons"))))});
			this.cbColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.cbColor.EditValueChanged += new System.EventHandler(this.cbColor_EditValueChanged);
			this.pnlLabels.BackColor = System.Drawing.Color.Transparent;
			this.pnlLabels.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlLabels.Controls.Add(this.chartLabelControl1);
			resources.ApplyResources(this.pnlLabels, "pnlLabels");
			this.pnlLabels.Name = "pnlLabels";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this.fillStyleControl, "fillStyleControl");
			this.fillStyleControl.Name = "fillStyleControl";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.fillStyleControl);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.pnlEditors);
			this.Name = "StripAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLabels)).EndInit();
			this.pnlLabels.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlEditors;
		private ChartPanelControl pnlControls;
		private DevExpress.XtraEditors.ColorEdit cbColor;
		private ChartPanelControl pnlLabels;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyleSeriesViewControl fillStyleControl;
		private ChartPanelControl panelControl1;
		private ChartLabelControl chartLabelControl1;
	}
}
