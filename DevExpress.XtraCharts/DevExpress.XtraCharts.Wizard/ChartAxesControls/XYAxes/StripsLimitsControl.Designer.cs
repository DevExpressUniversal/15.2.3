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
	partial class StripsLimitsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StripsLimitsControl));
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.pnlMinValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtMinValue = new DevExpress.XtraEditors.TextEdit();
			this.panelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl9 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chMinEnabled = new DevExpress.XtraEditors.CheckEdit();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.pnlMaxValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtMaxValue = new DevExpress.XtraEditors.TextEdit();
			this.panelControl11 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl12 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chMaxEnabled = new DevExpress.XtraEditors.CheckEdit();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValue)).BeginInit();
			this.pnlMinValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtMinValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).BeginInit();
			this.panelControl8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chMinEnabled.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValue)).BeginInit();
			this.pnlMaxValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtMaxValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).BeginInit();
			this.panelControl11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chMaxEnabled.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.groupControl3, "groupControl3");
			this.groupControl3.Controls.Add(this.pnlMinValue);
			this.groupControl3.Controls.Add(this.panelControl9);
			this.groupControl3.Controls.Add(this.chMinEnabled);
			this.groupControl3.Name = "groupControl3";
			resources.ApplyResources(this.pnlMinValue, "pnlMinValue");
			this.pnlMinValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlMinValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMinValue.Controls.Add(this.txtMinValue);
			this.pnlMinValue.Controls.Add(this.panelControl8);
			this.pnlMinValue.Name = "pnlMinValue";
			resources.ApplyResources(this.txtMinValue, "txtMinValue");
			this.txtMinValue.Name = "txtMinValue";
			this.txtMinValue.Properties.ValidateOnEnterKey = true;
			this.txtMinValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinValue_Validating);
			this.txtMinValue.Validated += new System.EventHandler(this.txtMinValue_Validated);
			this.panelControl8.BackColor = System.Drawing.Color.Transparent;
			this.panelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl8.Controls.Add(this.chartLabelControl1);
			resources.ApplyResources(this.panelControl8, "panelControl8");
			this.panelControl8.Name = "panelControl8";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl9.BackColor = System.Drawing.Color.Transparent;
			this.panelControl9.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl9, "panelControl9");
			this.panelControl9.Name = "panelControl9";
			resources.ApplyResources(this.chMinEnabled, "chMinEnabled");
			this.chMinEnabled.Name = "chMinEnabled";
			this.chMinEnabled.Properties.Caption = resources.GetString("chMinEnabled.Properties.Caption");
			this.chMinEnabled.CheckedChanged += new System.EventHandler(this.chMinVisible_CheckedChanged);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Controls.Add(this.pnlMaxValue);
			this.groupControl1.Controls.Add(this.panelControl12);
			this.groupControl1.Controls.Add(this.chMaxEnabled);
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.pnlMaxValue, "pnlMaxValue");
			this.pnlMaxValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxValue.Controls.Add(this.txtMaxValue);
			this.pnlMaxValue.Controls.Add(this.panelControl11);
			this.pnlMaxValue.Name = "pnlMaxValue";
			resources.ApplyResources(this.txtMaxValue, "txtMaxValue");
			this.txtMaxValue.Name = "txtMaxValue";
			this.txtMaxValue.Properties.ValidateOnEnterKey = true;
			this.txtMaxValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxValue_Validating);
			this.txtMaxValue.Validated += new System.EventHandler(this.txtMaxValue_Validated);
			this.panelControl11.BackColor = System.Drawing.Color.Transparent;
			this.panelControl11.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl11.Controls.Add(this.chartLabelControl2);
			resources.ApplyResources(this.panelControl11, "panelControl11");
			this.panelControl11.Name = "panelControl11";
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.panelControl12.BackColor = System.Drawing.Color.Transparent;
			this.panelControl12.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl12, "panelControl12");
			this.panelControl12.Name = "panelControl12";
			resources.ApplyResources(this.chMaxEnabled, "chMaxEnabled");
			this.chMaxEnabled.Name = "chMaxEnabled";
			this.chMaxEnabled.Properties.Caption = resources.GetString("chMaxEnabled.Properties.Caption");
			this.chMaxEnabled.CheckedChanged += new System.EventHandler(this.chMaxVivible_CheckedChanged);
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupControl1);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.groupControl3);
			this.Name = "StripsLimitsControl";
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			this.groupControl3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValue)).EndInit();
			this.pnlMinValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtMinValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).EndInit();
			this.panelControl8.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chMinEnabled.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValue)).EndInit();
			this.pnlMaxValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtMaxValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).EndInit();
			this.panelControl11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chMaxEnabled.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl groupControl3;
		private ChartPanelControl pnlMinValue;
		private DevExpress.XtraEditors.TextEdit txtMinValue;
		private ChartPanelControl panelControl8;
		private ChartPanelControl panelControl9;
		private DevExpress.XtraEditors.CheckEdit chMinEnabled;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private ChartPanelControl pnlMaxValue;
		private DevExpress.XtraEditors.TextEdit txtMaxValue;
		private ChartPanelControl panelControl11;
		private ChartPanelControl panelControl12;
		private DevExpress.XtraEditors.CheckEdit chMaxEnabled;
		private ChartPanelControl panelControl1;
		private ChartLabelControl chartLabelControl1;
		private ChartLabelControl chartLabelControl2;
	}
}
