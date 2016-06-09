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
	partial class AxisLabelOverlappingSettingsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisLabelOverlappingSettingsControl));
			this.grOverlappingSettings = new DevExpress.XtraEditors.GroupControl();
			this.spnIndent = new DevExpress.XtraEditors.SpinEdit();
			this.lblMinIndent = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAllowHide = new DevExpress.XtraEditors.CheckEdit();
			this.chAllowRotate = new DevExpress.XtraEditors.CheckEdit();
			this.chAllowStagger = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControlIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.grOverlappingSettings)).BeginInit();
			this.grOverlappingSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAlignment)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowHide.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowRotate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowStagger.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelIndent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelMode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlIndent)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grOverlappingSettings, "grOverlappingSettings");
			this.grOverlappingSettings.Controls.Add(this.spnIndent);
			this.grOverlappingSettings.Controls.Add(this.lblMinIndent);
			this.grOverlappingSettings.Controls.Add(this.sepAlignment);
			this.grOverlappingSettings.Controls.Add(this.chAllowHide);
			this.grOverlappingSettings.Controls.Add(this.chAllowRotate);
			this.grOverlappingSettings.Controls.Add(this.chAllowStagger);
			this.grOverlappingSettings.Controls.Add(this.chartPanelIndent);
			this.grOverlappingSettings.Controls.Add(this.chartPanelMode);
			this.grOverlappingSettings.Controls.Add(this.panelControlIndent);
			this.grOverlappingSettings.Name = "grOverlappingSettings";
			resources.ApplyResources(this.spnIndent, "spnIndent");
			this.spnIndent.Name = "spnIndent";
			this.spnIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnIndent.Properties.IsFloatValue = false;
			this.spnIndent.Properties.Mask.EditMask = resources.GetString("spnIndent.Properties.Mask.EditMask");
			this.spnIndent.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnIndent.Properties.MinValue = new decimal(new int[] {
			10000,
			0,
			0,
			-2147483648});
			this.spnIndent.EditValueChanged += new System.EventHandler(this.spnIndent_EditValueChanged);
			resources.ApplyResources(this.lblMinIndent, "lblMinIndent");
			this.lblMinIndent.Name = "lblMinIndent";
			this.sepAlignment.BackColor = System.Drawing.Color.Transparent;
			this.sepAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAlignment, "sepAlignment");
			this.sepAlignment.Name = "sepAlignment";
			resources.ApplyResources(this.chAllowHide, "chAllowHide");
			this.chAllowHide.Name = "chAllowHide";
			this.chAllowHide.Properties.Caption = resources.GetString("chAllowHide.Properties.Caption");
			this.chAllowHide.CheckedChanged += new System.EventHandler(this.chAllowHide_CheckedChanged);
			resources.ApplyResources(this.chAllowRotate, "chAllowRotate");
			this.chAllowRotate.Name = "chAllowRotate";
			this.chAllowRotate.Properties.Caption = resources.GetString("chAllowRotate.Properties.Caption");
			this.chAllowRotate.CheckedChanged += new System.EventHandler(this.chAllowRotate_CheckedChanged);
			resources.ApplyResources(this.chAllowStagger, "chAllowStagger");
			this.chAllowStagger.Name = "chAllowStagger";
			this.chAllowStagger.Properties.Caption = resources.GetString("chAllowStagger.Properties.Caption");
			this.chAllowStagger.CheckedChanged += new System.EventHandler(this.chAllowStagger_CheckedChanged);
			resources.ApplyResources(this.chartPanelIndent, "chartPanelIndent");
			this.chartPanelIndent.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelIndent.Name = "chartPanelIndent";
			resources.ApplyResources(this.chartPanelMode, "chartPanelMode");
			this.chartPanelMode.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelMode.Name = "chartPanelMode";
			resources.ApplyResources(this.panelControlIndent, "panelControlIndent");
			this.panelControlIndent.BackColor = System.Drawing.Color.Transparent;
			this.panelControlIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControlIndent.Name = "panelControlIndent";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grOverlappingSettings);
			this.Name = "AxisLabelOverlappingSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.grOverlappingSettings)).EndInit();
			this.grOverlappingSettings.ResumeLayout(false);
			this.grOverlappingSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAlignment)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowHide.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowRotate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAllowStagger.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelIndent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelMode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlIndent)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelControlIndent;
		protected DevExpress.XtraEditors.GroupControl grOverlappingSettings;
		private ChartPanelControl chartPanelIndent;
		private ChartPanelControl chartPanelMode;
		private DevExpress.XtraEditors.CheckEdit chAllowHide;
		private DevExpress.XtraEditors.CheckEdit chAllowRotate;
		private ChartPanelControl sepAlignment;
		private DevExpress.XtraEditors.SpinEdit spnIndent;
		private ChartLabelControl lblMinIndent;
		private DevExpress.XtraEditors.CheckEdit chAllowStagger;
	}
}
