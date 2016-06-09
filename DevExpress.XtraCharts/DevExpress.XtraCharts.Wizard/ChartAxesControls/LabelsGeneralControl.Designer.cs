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
	partial class LabelsGeneralControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelsGeneralControl));
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabPageGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.axisLabelControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisLabelControl();
			this.tabPageAuto = new DevExpress.XtraTab.XtraTabPage();
			this.labelTextPatternControl = new DevExpress.XtraCharts.Wizard.AxisLabelTextPatternControl();
			this.tabPageCustom = new DevExpress.XtraTab.XtraTabPage();
			this.customLabelsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.CustomLabelsControl();
			this.panelPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.redactControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.CustomLabelsListRedactControl();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.tabPageAuto.SuspendLayout();
			this.tabPageCustom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelPadding)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.tabPageGeneral;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabPageGeneral,
			this.tabPageAuto,
			this.tabPageCustom});
			this.tabPageGeneral.Controls.Add(this.axisLabelControl);
			this.tabPageGeneral.Name = "tabPageGeneral";
			resources.ApplyResources(this.tabPageGeneral, "tabPageGeneral");
			resources.ApplyResources(this.axisLabelControl, "axisLabelControl");
			this.axisLabelControl.Name = "axisLabelControl";
			this.tabPageAuto.Controls.Add(this.labelTextPatternControl);
			this.tabPageAuto.Name = "tabPageAuto";
			resources.ApplyResources(this.tabPageAuto, "tabPageAuto");
			resources.ApplyResources(this.labelTextPatternControl, "labelTextPatternControl");
			this.labelTextPatternControl.Name = "labelTextPatternControl";
			this.tabPageCustom.Controls.Add(this.customLabelsControl);
			this.tabPageCustom.Controls.Add(this.panelPadding);
			this.tabPageCustom.Controls.Add(this.redactControl);
			this.tabPageCustom.Name = "tabPageCustom";
			resources.ApplyResources(this.tabPageCustom, "tabPageCustom");
			resources.ApplyResources(this.customLabelsControl, "customLabelsControl");
			this.customLabelsControl.Name = "customLabelsControl";
			this.panelPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelPadding, "panelPadding");
			this.panelPadding.Name = "panelPadding";
			resources.ApplyResources(this.redactControl, "redactControl");
			this.redactControl.Name = "redactControl";
			this.Controls.Add(this.xtraTabControl);
			this.Name = "LabelsGeneralControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageGeneral.PerformLayout();
			this.tabPageAuto.ResumeLayout(false);
			this.tabPageAuto.PerformLayout();
			this.tabPageCustom.ResumeLayout(false);
			this.tabPageCustom.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelPadding)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl xtraTabControl;
		private DevExpress.XtraTab.XtraTabPage tabPageAuto;
		private DevExpress.XtraTab.XtraTabPage tabPageCustom;
		private CustomLabelsListRedactControl redactControl;
		private CustomLabelsControl customLabelsControl;
		private ChartPanelControl panelPadding;
		private DevExpress.XtraTab.XtraTabPage tabPageGeneral;
		private AxisLabelControl axisLabelControl;
		private AxisLabelTextPatternControl labelTextPatternControl;
	}
}
