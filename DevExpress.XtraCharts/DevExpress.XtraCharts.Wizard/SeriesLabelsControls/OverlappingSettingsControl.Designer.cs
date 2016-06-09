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

namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	partial class OverlappingSettingsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlappingSettingsControl));
			this.grOverlappingSettings = new DevExpress.XtraEditors.GroupControl();
			this.chartPanelIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spinIndent = new DevExpress.XtraEditors.SpinEdit();
			this.labelIndent = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.comboBoxMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelMode = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControlIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.grOverlappingSettings)).BeginInit();
			this.grOverlappingSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelIndent)).BeginInit();
			this.chartPanelIndent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelMode)).BeginInit();
			this.chartPanelMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlIndent)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grOverlappingSettings, "grOverlappingSettings");
			this.grOverlappingSettings.Controls.Add(this.chartPanelIndent);
			this.grOverlappingSettings.Controls.Add(this.chartPanelControl2);
			this.grOverlappingSettings.Controls.Add(this.chartPanelMode);
			this.grOverlappingSettings.Controls.Add(this.panelControlIndent);
			this.grOverlappingSettings.Name = "grOverlappingSettings";
			resources.ApplyResources(this.chartPanelIndent, "chartPanelIndent");
			this.chartPanelIndent.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelIndent.Controls.Add(this.spinIndent);
			this.chartPanelIndent.Controls.Add(this.labelIndent);
			this.chartPanelIndent.Name = "chartPanelIndent";
			resources.ApplyResources(this.spinIndent, "spinIndent");
			this.spinIndent.Name = "spinIndent";
			this.spinIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinIndent.Properties.IsFloatValue = false;
			this.spinIndent.Properties.Mask.EditMask = resources.GetString("spinIndent.Properties.Mask.EditMask");
			this.spinIndent.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spinIndent.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			-2147483648});
			this.spinIndent.EditValueChanged += new System.EventHandler(this.txtIndent_EditValueChanged);
			resources.ApplyResources(this.labelIndent, "labelIndent");
			this.labelIndent.Name = "labelIndent";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartPanelMode, "chartPanelMode");
			this.chartPanelMode.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelMode.Controls.Add(this.comboBoxMode);
			this.chartPanelMode.Controls.Add(this.labelMode);
			this.chartPanelMode.Name = "chartPanelMode";
			resources.ApplyResources(this.comboBoxMode, "comboBoxMode");
			this.comboBoxMode.Name = "comboBoxMode";
			this.comboBoxMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("comboBoxMode.Properties.Buttons"))))});
			this.comboBoxMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxMode_SelectedIndexChanged);
			resources.ApplyResources(this.labelMode, "labelMode");
			this.labelMode.Name = "labelMode";
			resources.ApplyResources(this.panelControlIndent, "panelControlIndent");
			this.panelControlIndent.BackColor = System.Drawing.Color.Transparent;
			this.panelControlIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControlIndent.Name = "panelControlIndent";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grOverlappingSettings);
			this.Name = "OverlappingSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.grOverlappingSettings)).EndInit();
			this.grOverlappingSettings.ResumeLayout(false);
			this.grOverlappingSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelIndent)).EndInit();
			this.chartPanelIndent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spinIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelMode)).EndInit();
			this.chartPanelMode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.comboBoxMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlIndent)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelControlIndent;
		protected DevExpress.XtraEditors.GroupControl grOverlappingSettings;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl chartPanelIndent;
		private ChartPanelControl chartPanelMode;
		private DevExpress.XtraEditors.ComboBoxEdit comboBoxMode;
		private ChartLabelControl labelMode;
		private DevExpress.XtraEditors.SpinEdit spinIndent;
		private ChartLabelControl labelIndent;
	}
}
