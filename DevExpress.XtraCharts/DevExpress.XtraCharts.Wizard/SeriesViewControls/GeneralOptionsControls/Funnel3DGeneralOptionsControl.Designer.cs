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
	partial class Funnel3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Funnel3DGeneralOptionsControl));
			this.pnlHoleRadiusPercent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnHoleRadiusPercent = new DevExpress.XtraEditors.SpinEdit();
			this.lbHoleRadiusPercent = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlPointDistance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnPointDistance = new DevExpress.XtraEditors.SpinEdit();
			this.lbPointDistance = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlHeightToWidthRatio = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnHeightToWidthRatio = new DevExpress.XtraEditors.SpinEdit();
			this.lbHeightToWidthRatio = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlHoleRadiusPercent)).BeginInit();
			this.pnlHoleRadiusPercent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnHoleRadiusPercent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPointDistance)).BeginInit();
			this.pnlPointDistance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnPointDistance.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlHeightToWidthRatio)).BeginInit();
			this.pnlHeightToWidthRatio.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnHeightToWidthRatio.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlHoleRadiusPercent, "pnlHoleRadiusPercent");
			this.pnlHoleRadiusPercent.BackColor = System.Drawing.Color.Transparent;
			this.pnlHoleRadiusPercent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlHoleRadiusPercent.Controls.Add(this.spnHoleRadiusPercent);
			this.pnlHoleRadiusPercent.Controls.Add(this.lbHoleRadiusPercent);
			this.pnlHoleRadiusPercent.Name = "pnlHoleRadiusPercent";
			resources.ApplyResources(this.spnHoleRadiusPercent, "spnHoleRadiusPercent");
			this.spnHoleRadiusPercent.Name = "spnHoleRadiusPercent";
			this.spnHoleRadiusPercent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHoleRadiusPercent.Properties.IsFloatValue = false;
			this.spnHoleRadiusPercent.Properties.Mask.EditMask = resources.GetString("spnHoleRadiusPercent.Properties.Mask.EditMask");
			this.spnHoleRadiusPercent.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spnHoleRadiusPercent.Properties.ValidateOnEnterKey = true;
			this.spnHoleRadiusPercent.EditValueChanged += new System.EventHandler(this.spnHoleRadiusPercent_EditValueChanged);
			resources.ApplyResources(this.lbHoleRadiusPercent, "lbHoleRadiusPercent");
			this.lbHoleRadiusPercent.Name = "lbHoleRadiusPercent";
			this.panelControl5.BackColor = System.Drawing.Color.Transparent;
			this.panelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl5, "panelControl5");
			this.panelControl5.Name = "panelControl5";
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
			this.spnPointDistance.EditValueChanged += new System.EventHandler(this.spnPointDistance_EditValueChanged);
			resources.ApplyResources(this.lbPointDistance, "lbPointDistance");
			this.lbPointDistance.Name = "lbPointDistance";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
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
			this.spnHeightToWidthRatio.EditValueChanged += new System.EventHandler(this.spnHeightToWidthRatio_EditValueChanged);
			resources.ApplyResources(this.lbHeightToWidthRatio, "lbHeightToWidthRatio");
			this.lbHeightToWidthRatio.Name = "lbHeightToWidthRatio";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlHeightToWidthRatio);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.pnlPointDistance);
			this.Controls.Add(this.panelControl5);
			this.Controls.Add(this.pnlHoleRadiusPercent);
			this.Name = "Funnel3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlHoleRadiusPercent)).EndInit();
			this.pnlHoleRadiusPercent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnHoleRadiusPercent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPointDistance)).EndInit();
			this.pnlPointDistance.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnPointDistance.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlHeightToWidthRatio)).EndInit();
			this.pnlHeightToWidthRatio.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnHeightToWidthRatio.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlHoleRadiusPercent;
		private DevExpress.XtraEditors.SpinEdit spnHoleRadiusPercent;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lbHoleRadiusPercent;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelControl5;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlPointDistance;
		private DevExpress.XtraEditors.SpinEdit spnPointDistance;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lbPointDistance;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl1;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlHeightToWidthRatio;
		private DevExpress.XtraEditors.SpinEdit spnHeightToWidthRatio;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lbHeightToWidthRatio;
	}
}
