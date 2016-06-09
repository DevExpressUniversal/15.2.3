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
	partial class MarkerBorderControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkerBorderControl));
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbColor = new DevExpress.XtraEditors.ColorEdit();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this.cbColor, "cbColor");
			this.cbColor.Name = "cbColor";
			this.cbColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbColor.Properties.Buttons"))))});
			this.cbColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.cbColor.EditValueChanged += new System.EventHandler(this.cbColor_EditValueChanged);
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.chartLabelControl1);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.Add(this.cbColor);
			this.panelControl3.Controls.Add(this.panelControl1);
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chVisible);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.panelControl3);
			this.Controls.Add(this.panelControl4);
			this.Name = "MarkerBorderControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelControl4;
		private DevExpress.XtraEditors.ColorEdit cbColor;
		private ChartPanelControl panelControl1;
		private ChartPanelControl panelControl2;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private ChartPanelControl panelControl3;
		private ChartLabelControl chartLabelControl1;
	}
}
