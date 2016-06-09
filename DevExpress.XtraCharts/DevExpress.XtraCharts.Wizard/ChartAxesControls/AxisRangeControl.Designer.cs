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
	partial class AxisRangeControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisRangeControl));
			this.pnlAuto = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAuto = new DevExpress.XtraEditors.CheckEdit();
			this.sepMinValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMinValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtMinValue = new DevExpress.XtraEditors.TextEdit();
			this.lblMinValue = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepMaxValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtMaxValue = new DevExpress.XtraEditors.TextEdit();
			this.lblMaxValue = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlShowZeroLevel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chShowZeroLevel = new DevExpress.XtraEditors.CheckEdit();
			this.lblMinValueLine = new DevExpress.XtraEditors.LabelControl();
			this.sepShowZeroLevel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepShowZeroLevelContent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepSideMargins = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpSideMargins = new DevExpress.XtraEditors.GroupControl();
			this.pnlSideMarginsValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtSideMarginsValue = new DevExpress.XtraEditors.SpinEdit();
			this.lblSideMarginsValue = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepSideMarginsValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAutoSideMargins = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAutoSideMargins = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAuto)).BeginInit();
			this.pnlAuto.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chAuto.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMinValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValue)).BeginInit();
			this.pnlMinValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtMinValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMaxValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValue)).BeginInit();
			this.pnlMaxValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtMaxValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShowZeroLevel)).BeginInit();
			this.pnlShowZeroLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chShowZeroLevel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShowZeroLevel)).BeginInit();
			this.sepShowZeroLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShowZeroLevelContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSideMargins)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpSideMargins)).BeginInit();
			this.grpSideMargins.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSideMarginsValue)).BeginInit();
			this.pnlSideMarginsValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtSideMarginsValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSideMarginsValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAutoSideMargins)).BeginInit();
			this.pnlAutoSideMargins.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chAutoSideMargins.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlAuto, "pnlAuto");
			this.pnlAuto.BackColor = System.Drawing.Color.Transparent;
			this.pnlAuto.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAuto.Controls.Add(this.chAuto);
			this.pnlAuto.Name = "pnlAuto";
			resources.ApplyResources(this.chAuto, "chAuto");
			this.chAuto.Name = "chAuto";
			this.chAuto.Properties.Caption = resources.GetString("chAuto.Properties.Caption");
			this.chAuto.CheckedChanged += new System.EventHandler(this.chAuto_CheckedChanged);
			this.sepMinValue.BackColor = System.Drawing.Color.Transparent;
			this.sepMinValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepMinValue, "sepMinValue");
			this.sepMinValue.Name = "sepMinValue";
			resources.ApplyResources(this.pnlMinValue, "pnlMinValue");
			this.pnlMinValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlMinValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMinValue.Controls.Add(this.txtMinValue);
			this.pnlMinValue.Controls.Add(this.lblMinValue);
			this.pnlMinValue.Name = "pnlMinValue";
			resources.ApplyResources(this.txtMinValue, "txtMinValue");
			this.txtMinValue.Name = "txtMinValue";
			this.txtMinValue.Properties.Appearance.Options.UseTextOptions = true;
			this.txtMinValue.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtMinValue.Properties.ValidateOnEnterKey = true;
			this.txtMinValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinValue_Validating);
			this.txtMinValue.Validated += new System.EventHandler(this.txtMinValue_Validated);
			resources.ApplyResources(this.lblMinValue, "lblMinValue");
			this.lblMinValue.Name = "lblMinValue";
			this.sepMaxValue.BackColor = System.Drawing.Color.Transparent;
			this.sepMaxValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepMaxValue, "sepMaxValue");
			this.sepMaxValue.Name = "sepMaxValue";
			resources.ApplyResources(this.pnlMaxValue, "pnlMaxValue");
			this.pnlMaxValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxValue.Controls.Add(this.txtMaxValue);
			this.pnlMaxValue.Controls.Add(this.lblMaxValue);
			this.pnlMaxValue.Name = "pnlMaxValue";
			resources.ApplyResources(this.txtMaxValue, "txtMaxValue");
			this.txtMaxValue.Name = "txtMaxValue";
			this.txtMaxValue.Properties.Appearance.Options.UseTextOptions = true;
			this.txtMaxValue.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtMaxValue.Properties.ValidateOnEnterKey = true;
			this.txtMaxValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxValue_Validating);
			this.txtMaxValue.Validated += new System.EventHandler(this.txtMaxValue_Validated);
			resources.ApplyResources(this.lblMaxValue, "lblMaxValue");
			this.lblMaxValue.Name = "lblMaxValue";
			resources.ApplyResources(this.pnlShowZeroLevel, "pnlShowZeroLevel");
			this.pnlShowZeroLevel.BackColor = System.Drawing.Color.Transparent;
			this.pnlShowZeroLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlShowZeroLevel.Controls.Add(this.chShowZeroLevel);
			this.pnlShowZeroLevel.Name = "pnlShowZeroLevel";
			resources.ApplyResources(this.chShowZeroLevel, "chShowZeroLevel");
			this.chShowZeroLevel.Name = "chShowZeroLevel";
			this.chShowZeroLevel.Properties.Caption = resources.GetString("chShowZeroLevel.Properties.Caption");
			this.chShowZeroLevel.CheckedChanged += new System.EventHandler(this.chShowZeroLevel_CheckedChanged);
			resources.ApplyResources(this.lblMinValueLine, "lblMinValueLine");
			this.lblMinValueLine.LineVisible = true;
			this.lblMinValueLine.Name = "lblMinValueLine";
			resources.ApplyResources(this.sepShowZeroLevel, "sepShowZeroLevel");
			this.sepShowZeroLevel.BackColor = System.Drawing.Color.Transparent;
			this.sepShowZeroLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.sepShowZeroLevel.Controls.Add(this.chartPanelControl1);
			this.sepShowZeroLevel.Controls.Add(this.sepShowZeroLevelContent);
			this.sepShowZeroLevel.Name = "sepShowZeroLevel";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			this.sepShowZeroLevelContent.BackColor = System.Drawing.Color.Transparent;
			this.sepShowZeroLevelContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepShowZeroLevelContent, "sepShowZeroLevelContent");
			this.sepShowZeroLevelContent.Name = "sepShowZeroLevelContent";
			this.sepSideMargins.BackColor = System.Drawing.Color.Transparent;
			this.sepSideMargins.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSideMargins, "sepSideMargins");
			this.sepSideMargins.Name = "sepSideMargins";
			resources.ApplyResources(this.grpSideMargins, "grpSideMargins");
			this.grpSideMargins.Controls.Add(this.pnlSideMarginsValue);
			this.grpSideMargins.Controls.Add(this.sepSideMarginsValue);
			this.grpSideMargins.Controls.Add(this.pnlAutoSideMargins);
			this.grpSideMargins.Name = "grpSideMargins";
			resources.ApplyResources(this.pnlSideMarginsValue, "pnlSideMarginsValue");
			this.pnlSideMarginsValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlSideMarginsValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSideMarginsValue.Controls.Add(this.txtSideMarginsValue);
			this.pnlSideMarginsValue.Controls.Add(this.lblSideMarginsValue);
			this.pnlSideMarginsValue.Name = "pnlSideMarginsValue";
			resources.ApplyResources(this.txtSideMarginsValue, "txtSideMarginsValue");
			this.txtSideMarginsValue.Name = "txtSideMarginsValue";
			this.txtSideMarginsValue.Properties.Appearance.Options.UseTextOptions = true;
			this.txtSideMarginsValue.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtSideMarginsValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("txtSideMarginsValue.Properties.Buttons"))))});
			this.txtSideMarginsValue.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.txtSideMarginsValue.Properties.Increment = new decimal(new int[] {
			5,
			0,
			0,
			65536});
			this.txtSideMarginsValue.Properties.MaxValue = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.txtSideMarginsValue.Properties.ValidateOnEnterKey = true;
			this.txtSideMarginsValue.EditValueChanged += new System.EventHandler(this.txtSideMarginsValue_EditValueChanged);
			resources.ApplyResources(this.lblSideMarginsValue, "lblSideMarginsValue");
			this.lblSideMarginsValue.Name = "lblSideMarginsValue";
			this.sepSideMarginsValue.BackColor = System.Drawing.Color.Transparent;
			this.sepSideMarginsValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSideMarginsValue, "sepSideMarginsValue");
			this.sepSideMarginsValue.Name = "sepSideMarginsValue";
			resources.ApplyResources(this.pnlAutoSideMargins, "pnlAutoSideMargins");
			this.pnlAutoSideMargins.BackColor = System.Drawing.Color.Transparent;
			this.pnlAutoSideMargins.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAutoSideMargins.Controls.Add(this.chAutoSideMargins);
			this.pnlAutoSideMargins.Name = "pnlAutoSideMargins";
			resources.ApplyResources(this.chAutoSideMargins, "chAutoSideMargins");
			this.chAutoSideMargins.Name = "chAutoSideMargins";
			this.chAutoSideMargins.Properties.Caption = resources.GetString("chAutoSideMargins.Properties.Caption");
			this.chAutoSideMargins.CheckedChanged += new System.EventHandler(this.chAutoSideMargins_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpSideMargins);
			this.Controls.Add(this.sepSideMargins);
			this.Controls.Add(this.pnlShowZeroLevel);
			this.Controls.Add(this.sepShowZeroLevel);
			this.Controls.Add(this.pnlMaxValue);
			this.Controls.Add(this.sepMaxValue);
			this.Controls.Add(this.pnlMinValue);
			this.Controls.Add(this.sepMinValue);
			this.Controls.Add(this.lblMinValueLine);
			this.Controls.Add(this.pnlAuto);
			this.Name = "AxisRangeControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlAuto)).EndInit();
			this.pnlAuto.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chAuto.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMinValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinValue)).EndInit();
			this.pnlMinValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtMinValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMaxValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxValue)).EndInit();
			this.pnlMaxValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtMaxValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShowZeroLevel)).EndInit();
			this.pnlShowZeroLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chShowZeroLevel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShowZeroLevel)).EndInit();
			this.sepShowZeroLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShowZeroLevelContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSideMargins)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpSideMargins)).EndInit();
			this.grpSideMargins.ResumeLayout(false);
			this.grpSideMargins.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSideMarginsValue)).EndInit();
			this.pnlSideMarginsValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtSideMarginsValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSideMarginsValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAutoSideMargins)).EndInit();
			this.pnlAutoSideMargins.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chAutoSideMargins.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlAuto;
		private DevExpress.XtraEditors.CheckEdit chAuto;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepMinValue;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlMinValue;
		private DevExpress.XtraEditors.TextEdit txtMinValue;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepMaxValue;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlMaxValue;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblMaxValue;
		private DevExpress.XtraEditors.TextEdit txtMaxValue;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlShowZeroLevel;
		private DevExpress.XtraEditors.CheckEdit chShowZeroLevel;
		private DevExpress.XtraEditors.LabelControl lblMinValueLine;
		private ChartPanelControl sepShowZeroLevel;
		private ChartPanelControl sepShowZeroLevelContent;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl sepSideMargins;
		private XtraEditors.GroupControl grpSideMargins;
		private ChartPanelControl pnlAutoSideMargins;
		private XtraEditors.CheckEdit chAutoSideMargins;
		private ChartPanelControl sepSideMarginsValue;
		private ChartLabelControl lblMinValue;
		private ChartPanelControl pnlSideMarginsValue;
		private ChartLabelControl lblSideMarginsValue;
		private XtraEditors.SpinEdit txtSideMarginsValue;
	}
}
