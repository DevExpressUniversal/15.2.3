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
	partial class StripsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StripsControl));
			this.tbcStripSettings = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
			this.stripsRedactControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.StripsRedactControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.stripsGeneralControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.StripsLimitsControl();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.stripAppearanceControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.StripAppearanceControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.stripsListControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.StripsListRedactControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcStripSettings)).BeginInit();
			this.tbcStripSettings.SuspendLayout();
			this.xtraTabPage3.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			this.xtraTabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcStripSettings, "tbcStripSettings");
			this.tbcStripSettings.Name = "tbcStripSettings";
			this.tbcStripSettings.SelectedTabPage = this.xtraTabPage3;
			this.tbcStripSettings.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage3,
			this.xtraTabPage1,
			this.xtraTabPage2});
			this.xtraTabPage3.Controls.Add(this.stripsRedactControl);
			this.xtraTabPage3.Name = "xtraTabPage3";
			resources.ApplyResources(this.xtraTabPage3, "xtraTabPage3");
			resources.ApplyResources(this.stripsRedactControl, "stripsRedactControl");
			this.stripsRedactControl.Name = "stripsRedactControl";
			this.xtraTabPage1.Controls.Add(this.stripsGeneralControl);
			this.xtraTabPage1.Name = "xtraTabPage1";
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			resources.ApplyResources(this.stripsGeneralControl, "stripsGeneralControl");
			this.stripsGeneralControl.Name = "stripsGeneralControl";
			this.xtraTabPage2.Controls.Add(this.stripAppearanceControl);
			this.xtraTabPage2.Name = "xtraTabPage2";
			resources.ApplyResources(this.xtraTabPage2, "xtraTabPage2");
			resources.ApplyResources(this.stripAppearanceControl, "stripAppearanceControl");
			this.stripAppearanceControl.Name = "stripAppearanceControl";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.stripsListControl, "stripsListControl");
			this.stripsListControl.Name = "stripsListControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcStripSettings);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.stripsListControl);
			this.Name = "StripsControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcStripSettings)).EndInit();
			this.tbcStripSettings.ResumeLayout(false);
			this.xtraTabPage3.ResumeLayout(false);
			this.xtraTabPage3.PerformLayout();
			this.xtraTabPage1.ResumeLayout(false);
			this.xtraTabPage1.PerformLayout();
			this.xtraTabPage2.ResumeLayout(false);
			this.xtraTabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcStripSettings;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private StripsLimitsControl stripsGeneralControl;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
		private StripsRedactControl stripsRedactControl;
		private StripAppearanceControl stripAppearanceControl;
		private ChartPanelControl panelControl1;
		private StripsListRedactControl stripsListControl;
	}
}
