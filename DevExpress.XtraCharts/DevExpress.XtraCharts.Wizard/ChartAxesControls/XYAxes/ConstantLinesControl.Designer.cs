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
	partial class ConstantLinesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConstantLinesControl));
			this.tbcConstantLines = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.lineGeneral = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLineGeneralControl();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.lineAppearanceControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLineAppearanceControl();
			this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
			this.lineTitleControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLineTitleControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lineListRedact = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLineListRedactControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcConstantLines)).BeginInit();
			this.tbcConstantLines.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			this.xtraTabPage2.SuspendLayout();
			this.xtraTabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcConstantLines, "tbcConstantLines");
			this.tbcConstantLines.Name = "tbcConstantLines";
			this.tbcConstantLines.SelectedTabPage = this.xtraTabPage1;
			this.tbcConstantLines.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage1,
			this.xtraTabPage2,
			this.xtraTabPage3});
			this.xtraTabPage1.Controls.Add(this.lineGeneral);
			this.xtraTabPage1.Name = "xtraTabPage1";
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			resources.ApplyResources(this.lineGeneral, "lineGeneral");
			this.lineGeneral.Name = "lineGeneral";
			this.xtraTabPage2.Controls.Add(this.lineAppearanceControl);
			this.xtraTabPage2.Name = "xtraTabPage2";
			resources.ApplyResources(this.xtraTabPage2, "xtraTabPage2");
			resources.ApplyResources(this.lineAppearanceControl, "lineAppearanceControl");
			this.lineAppearanceControl.Name = "lineAppearanceControl";
			this.xtraTabPage3.Controls.Add(this.lineTitleControl);
			this.xtraTabPage3.Name = "xtraTabPage3";
			resources.ApplyResources(this.xtraTabPage3, "xtraTabPage3");
			resources.ApplyResources(this.lineTitleControl, "lineTitleControl");
			this.lineTitleControl.Name = "lineTitleControl";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.lineListRedact, "lineListRedact");
			this.lineListRedact.Name = "lineListRedact";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcConstantLines);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.lineListRedact);
			this.Name = "ConstantLinesControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcConstantLines)).EndInit();
			this.tbcConstantLines.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			this.xtraTabPage1.PerformLayout();
			this.xtraTabPage2.ResumeLayout(false);
			this.xtraTabPage2.PerformLayout();
			this.xtraTabPage3.ResumeLayout(false);
			this.xtraTabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcConstantLines;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
		private ConstantLineGeneralControl lineGeneral;
		private ConstantLineTitleControl lineTitleControl;
		private ChartPanelControl panelControl1;
		private ConstantLineListRedactControl lineListRedact;
		private ConstantLineAppearanceControl lineAppearanceControl;
	}
}
