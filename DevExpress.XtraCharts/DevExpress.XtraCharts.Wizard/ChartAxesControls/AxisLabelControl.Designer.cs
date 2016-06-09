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
	partial class AxisLabelControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisLabelControl));
			this.ceVisible = new DevExpress.XtraEditors.CheckEdit();
			this.panelLayoutPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grLayout = new DevExpress.XtraEditors.GroupControl();
			this.axisLabelLayoutControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisLabelLayoutControl();
			this.panelFormatPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.axisLabelTextSettingsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisLabelTextSettingsControl();
			this.grTextSettings = new DevExpress.XtraEditors.GroupControl();
			this.panelOverlappingSettingPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.axisLabelOverlappingSettingsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisLabelOverlappingSettingsControl();
			((System.ComponentModel.ISupportInitialize)(this.ceVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelLayoutPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).BeginInit();
			this.grLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelFormatPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grTextSettings)).BeginInit();
			this.grTextSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelOverlappingSettingPadding)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.ceVisible, "ceVisible");
			this.ceVisible.Name = "ceVisible";
			this.ceVisible.Properties.Caption = resources.GetString("ceVisible.Properties.Caption");
			this.ceVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.panelLayoutPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelLayoutPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelLayoutPadding, "panelLayoutPadding");
			this.panelLayoutPadding.Name = "panelLayoutPadding";
			resources.ApplyResources(this.grLayout, "grLayout");
			this.grLayout.Controls.Add(this.axisLabelLayoutControl);
			this.grLayout.Name = "grLayout";
			resources.ApplyResources(this.axisLabelLayoutControl, "axisLabelLayoutControl");
			this.axisLabelLayoutControl.Name = "axisLabelLayoutControl";
			this.panelFormatPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelFormatPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelFormatPadding, "panelFormatPadding");
			this.panelFormatPadding.Name = "panelFormatPadding";
			resources.ApplyResources(this.axisLabelTextSettingsControl, "axisLabelTextSettingsControl");
			this.axisLabelTextSettingsControl.Name = "axisLabelTextSettingsControl";
			resources.ApplyResources(this.grTextSettings, "grTextSettings");
			this.grTextSettings.Controls.Add(this.axisLabelTextSettingsControl);
			this.grTextSettings.Name = "grTextSettings";
			this.panelOverlappingSettingPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelOverlappingSettingPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelOverlappingSettingPadding, "panelOverlappingSettingPadding");
			this.panelOverlappingSettingPadding.Name = "panelOverlappingSettingPadding";
			resources.ApplyResources(this.axisLabelOverlappingSettingsControl, "axisLabelOverlappingSettingsControl");
			this.axisLabelOverlappingSettingsControl.Name = "axisLabelOverlappingSettingsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.axisLabelOverlappingSettingsControl);
			this.Controls.Add(this.panelOverlappingSettingPadding);
			this.Controls.Add(this.grTextSettings);
			this.Controls.Add(this.panelFormatPadding);
			this.Controls.Add(this.grLayout);
			this.Controls.Add(this.panelLayoutPadding);
			this.Controls.Add(this.ceVisible);
			this.Name = "AxisLabelControl";
			((System.ComponentModel.ISupportInitialize)(this.ceVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelLayoutPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).EndInit();
			this.grLayout.ResumeLayout(false);
			this.grLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelFormatPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grTextSettings)).EndInit();
			this.grTextSettings.ResumeLayout(false);
			this.grTextSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelOverlappingSettingPadding)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.CheckEdit ceVisible;
		protected ChartPanelControl panelLayoutPadding;
		protected DevExpress.XtraEditors.GroupControl grLayout;
		private AxisLabelLayoutControl axisLabelLayoutControl;
		protected ChartPanelControl panelFormatPadding;
		private AxisLabelTextSettingsControl axisLabelTextSettingsControl;
		private DevExpress.XtraEditors.GroupControl grTextSettings;
		protected ChartPanelControl panelOverlappingSettingPadding;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisLabelOverlappingSettingsControl axisLabelOverlappingSettingsControl;
	}
}
