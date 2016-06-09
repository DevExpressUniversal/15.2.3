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

namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	partial class LegendMarkerControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendMarkerControl));
			this.panelControl14 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtVertical = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtHorizontal = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl5 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			this.pnlSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtVertical.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			this.chartPanelControl7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtHorizontal.Properties)).BeginInit();
			this.SuspendLayout();
			this.panelControl14.BackColor = System.Drawing.Color.Transparent;
			this.panelControl14.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl14, "panelControl14");
			this.panelControl14.Name = "panelControl14";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Controls.Add(this.chartPanelControl2);
			this.pnlSize.Controls.Add(this.chartPanelControl1);
			this.pnlSize.Controls.Add(this.chartPanelControl7);
			this.pnlSize.Name = "pnlSize";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.txtVertical);
			this.chartPanelControl2.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.txtVertical, "txtVertical");
			this.txtVertical.Name = "txtVertical";
			this.txtVertical.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtVertical.Properties.IsFloatValue = false;
			this.txtVertical.Properties.Mask.EditMask = resources.GetString("txtVertical.Properties.Mask.EditMask");
			this.txtVertical.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.txtVertical.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtVertical.EditValueChanged += new System.EventHandler(this.txtVertical_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl7.Controls.Add(this.txtHorizontal);
			this.chartPanelControl7.Controls.Add(this.chartLabelControl5);
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.txtHorizontal, "txtHorizontal");
			this.txtHorizontal.Name = "txtHorizontal";
			this.txtHorizontal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtHorizontal.Properties.IsFloatValue = false;
			this.txtHorizontal.Properties.Mask.EditMask = resources.GetString("txtHorizontal.Properties.Mask.EditMask");
			this.txtHorizontal.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.txtHorizontal.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtHorizontal.EditValueChanged += new System.EventHandler(this.txtHorizontal_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl5, "chartLabelControl5");
			this.chartLabelControl5.Name = "chartLabelControl5";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlSize);
			this.Controls.Add(this.panelControl14);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.chVisible);
			this.Name = "LegendMarkerControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			this.pnlSize.ResumeLayout(false);
			this.pnlSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtVertical.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			this.chartPanelControl7.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtHorizontal.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelControl14;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private ChartPanelControl pnlSize;
		private ChartPanelControl chartPanelControl2;
		private DevExpress.XtraEditors.SpinEdit txtVertical;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl7;
		private DevExpress.XtraEditors.SpinEdit txtHorizontal;
		private ChartLabelControl chartLabelControl5;
		private DevExpress.XtraEditors.LabelControl labelControl1;
	}
}
