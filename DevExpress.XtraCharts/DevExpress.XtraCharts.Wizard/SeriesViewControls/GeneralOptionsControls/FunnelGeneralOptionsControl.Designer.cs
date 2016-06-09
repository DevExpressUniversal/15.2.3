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
	partial class FunnelGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunnelGeneralOptionsControl));
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grLayout = new DevExpress.XtraEditors.GroupControl();
			this.pnlPointDistance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnPointDistance = new DevExpress.XtraEditors.SpinEdit();
			this.lbPointDistance = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAlignToCenter = new DevExpress.XtraEditors.CheckEdit();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.pnlHeightToWidthRatio = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnHeightToWidthRatio = new DevExpress.XtraEditors.SpinEdit();
			this.lbHeightToWidthRatio = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAuto = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).BeginInit();
			this.grLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPointDistance)).BeginInit();
			this.pnlPointDistance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnPointDistance.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAlignToCenter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlHeightToWidthRatio)).BeginInit();
			this.pnlHeightToWidthRatio.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnHeightToWidthRatio.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAuto.Properties)).BeginInit();
			this.SuspendLayout();
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.grLayout, "grLayout");
			this.grLayout.Controls.Add(this.pnlPointDistance);
			this.grLayout.Controls.Add(this.chartPanelControl1);
			this.grLayout.Controls.Add(this.chAlignToCenter);
			this.grLayout.Name = "grLayout";
			resources.ApplyResources(this.pnlPointDistance, "pnlPointDistance");
			this.pnlPointDistance.BackColor = System.Drawing.Color.Transparent;
			this.pnlPointDistance.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPointDistance.Controls.Add(this.spnPointDistance);
			this.pnlPointDistance.Controls.Add(this.lbPointDistance);
			this.pnlPointDistance.Name = "pnlPointDistance";
			resources.ApplyResources(this.spnPointDistance, "spnPointDistance");
			this.spnPointDistance.Name = "spnPointDistance";
			this.spnPointDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnPointDistance.Properties.IsFloatValue = false;
			this.spnPointDistance.Properties.Mask.EditMask = resources.GetString("spnPointDistance.Properties.Mask.EditMask");
			this.spnPointDistance.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spnPointDistance.Properties.ValidateOnEnterKey = true;
			this.spnPointDistance.EditValueChanged += new System.EventHandler(this.spnPointDistance_EditValueChanged_1);
			resources.ApplyResources(this.lbPointDistance, "lbPointDistance");
			this.lbPointDistance.Name = "lbPointDistance";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chAlignToCenter, "chAlignToCenter");
			this.chAlignToCenter.Name = "chAlignToCenter";
			this.chAlignToCenter.Properties.Caption = resources.GetString("chAlignToCenter.Properties.Caption");
			this.chAlignToCenter.CheckedChanged += new System.EventHandler(this.chAlignToCenter_CheckedChanged_1);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Controls.Add(this.pnlHeightToWidthRatio);
			this.groupControl1.Controls.Add(this.panelControl5);
			this.groupControl1.Controls.Add(this.chAuto);
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.pnlHeightToWidthRatio, "pnlHeightToWidthRatio");
			this.pnlHeightToWidthRatio.BackColor = System.Drawing.Color.Transparent;
			this.pnlHeightToWidthRatio.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlHeightToWidthRatio.Controls.Add(this.spnHeightToWidthRatio);
			this.pnlHeightToWidthRatio.Controls.Add(this.lbHeightToWidthRatio);
			this.pnlHeightToWidthRatio.Name = "pnlHeightToWidthRatio";
			resources.ApplyResources(this.spnHeightToWidthRatio, "spnHeightToWidthRatio");
			this.spnHeightToWidthRatio.Name = "spnHeightToWidthRatio";
			this.spnHeightToWidthRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHeightToWidthRatio.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnHeightToWidthRatio.Properties.MaxValue = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.spnHeightToWidthRatio.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnHeightToWidthRatio.Properties.ValidateOnEnterKey = true;
			this.spnHeightToWidthRatio.EditValueChanged += new System.EventHandler(this.spnHeightToWidthRatio_EditValueChanged_1);
			resources.ApplyResources(this.lbHeightToWidthRatio, "lbHeightToWidthRatio");
			this.lbHeightToWidthRatio.Name = "lbHeightToWidthRatio";
			this.panelControl5.BackColor = System.Drawing.Color.Transparent;
			this.panelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl5, "panelControl5");
			this.panelControl5.Name = "panelControl5";
			resources.ApplyResources(this.chAuto, "chAuto");
			this.chAuto.Name = "chAuto";
			this.chAuto.Properties.Caption = resources.GetString("chAuto.Properties.Caption");
			this.chAuto.CheckedChanged += new System.EventHandler(this.chAuto_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupControl1);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.grLayout);
			this.Name = "FunnelGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).EndInit();
			this.grLayout.ResumeLayout(false);
			this.grLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPointDistance)).EndInit();
			this.pnlPointDistance.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnPointDistance.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAlignToCenter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlHeightToWidthRatio)).EndInit();
			this.pnlHeightToWidthRatio.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnHeightToWidthRatio.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAuto.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl2;
		private DevExpress.XtraEditors.GroupControl grLayout;
		private DevExpress.XtraEditors.CheckEdit chAlignToCenter;
		private ChartPanelControl pnlPointDistance;
		private DevExpress.XtraEditors.SpinEdit spnPointDistance;
		private ChartLabelControl lbPointDistance;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private ChartPanelControl pnlHeightToWidthRatio;
		private DevExpress.XtraEditors.SpinEdit spnHeightToWidthRatio;
		private ChartLabelControl lbHeightToWidthRatio;
		private ChartPanelControl panelControl5;
		private DevExpress.XtraEditors.CheckEdit chAuto;
	}
}
