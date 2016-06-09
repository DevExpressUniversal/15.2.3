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
	partial class ScaleBreaksControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleBreaksControl));
			this.tbcScaleBreaks = new DevExpress.XtraTab.XtraTabControl();
			this.tbpAuto = new DevExpress.XtraTab.XtraTabPage();
			this.scaleBreaksAutoControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreaksAutoControl();
			this.tbpCustom = new DevExpress.XtraTab.XtraTabPage();
			this.scaleBreaksCustomControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreaksCustomControl();
			this.tbpAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.scaleBreakAppearanceControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreakAppearanceControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcScaleBreaks)).BeginInit();
			this.tbcScaleBreaks.SuspendLayout();
			this.tbpAuto.SuspendLayout();
			this.tbpCustom.SuspendLayout();
			this.tbpAppearance.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcScaleBreaks, "tbcScaleBreaks");
			this.tbcScaleBreaks.Name = "tbcScaleBreaks";
			this.tbcScaleBreaks.SelectedTabPage = this.tbpAuto;
			this.tbcScaleBreaks.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbpAuto,
			this.tbpCustom,
			this.tbpAppearance});
			this.tbpAuto.Controls.Add(this.scaleBreaksAutoControl);
			this.tbpAuto.Name = "tbpAuto";
			resources.ApplyResources(this.tbpAuto, "tbpAuto");
			resources.ApplyResources(this.scaleBreaksAutoControl, "scaleBreaksAutoControl");
			this.scaleBreaksAutoControl.Name = "scaleBreaksAutoControl";
			this.tbpCustom.Controls.Add(this.scaleBreaksCustomControl);
			this.tbpCustom.Name = "tbpCustom";
			resources.ApplyResources(this.tbpCustom, "tbpCustom");
			resources.ApplyResources(this.scaleBreaksCustomControl, "scaleBreaksCustomControl");
			this.scaleBreaksCustomControl.Name = "scaleBreaksCustomControl";
			this.tbpAppearance.Controls.Add(this.scaleBreakAppearanceControl);
			this.tbpAppearance.Name = "tbpAppearance";
			resources.ApplyResources(this.tbpAppearance, "tbpAppearance");
			resources.ApplyResources(this.scaleBreakAppearanceControl, "scaleBreakAppearanceControl");
			this.scaleBreakAppearanceControl.Name = "scaleBreakAppearanceControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcScaleBreaks);
			this.Name = "ScaleBreaksControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcScaleBreaks)).EndInit();
			this.tbcScaleBreaks.ResumeLayout(false);
			this.tbpAuto.ResumeLayout(false);
			this.tbpAuto.PerformLayout();
			this.tbpCustom.ResumeLayout(false);
			this.tbpCustom.PerformLayout();
			this.tbpAppearance.ResumeLayout(false);
			this.tbpAppearance.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcScaleBreaks;
		private DevExpress.XtraTab.XtraTabPage tbpAuto;
		private DevExpress.XtraTab.XtraTabPage tbpCustom;
		private DevExpress.XtraTab.XtraTabPage tbpAppearance;
		private ScaleBreakAppearanceControl scaleBreakAppearanceControl;
		private ScaleBreaksCustomControl scaleBreaksCustomControl;
		private ScaleBreaksAutoControl scaleBreaksAutoControl;
	}
}
