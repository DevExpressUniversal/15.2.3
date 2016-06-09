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
	partial class AxisGeneralTabsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisGeneralTabsControl));
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pageGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.generalControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisGeneralControl();
			this.pageVisualRange = new DevExpress.XtraTab.XtraTabPage();
			this.visualRangeControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisRangeControl();
			this.pageWholeRange = new DevExpress.XtraTab.XtraTabPage();
			this.wholeRangeControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisRangeControl();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.pageGeneral.SuspendLayout();
			this.pageVisualRange.SuspendLayout();
			this.pageWholeRange.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.pageGeneral;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageGeneral,
			this.pageVisualRange,
			this.pageWholeRange});
			this.pageGeneral.Controls.Add(this.generalControl);
			this.pageGeneral.Name = "pageGeneral";
			resources.ApplyResources(this.pageGeneral, "pageGeneral");
			resources.ApplyResources(this.generalControl, "generalControl");
			this.generalControl.Name = "generalControl";
			this.pageVisualRange.Controls.Add(this.visualRangeControl);
			this.pageVisualRange.Name = "pageVisualRange";
			resources.ApplyResources(this.pageVisualRange, "pageVisualRange");
			resources.ApplyResources(this.visualRangeControl, "visualRangeControl");
			this.visualRangeControl.Name = "visualRangeControl";
			this.pageWholeRange.Controls.Add(this.wholeRangeControl);
			this.pageWholeRange.Name = "pageWholeRange";
			resources.ApplyResources(this.pageWholeRange, "pageWholeRange");
			resources.ApplyResources(this.wholeRangeControl, "wholeRangeControl");
			this.wholeRangeControl.Name = "wholeRangeControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tabControl);
			this.Name = "AxisGeneralTabsControl";
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.pageGeneral.ResumeLayout(false);
			this.pageGeneral.PerformLayout();
			this.pageVisualRange.ResumeLayout(false);
			this.pageVisualRange.PerformLayout();
			this.pageWholeRange.ResumeLayout(false);
			this.pageWholeRange.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tabControl;
		private DevExpress.XtraTab.XtraTabPage pageGeneral;
		private DevExpress.XtraTab.XtraTabPage pageVisualRange;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisGeneralControl generalControl;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisRangeControl visualRangeControl;
		private DevExpress.XtraTab.XtraTabPage pageWholeRange;
		private AxisRangeControl wholeRangeControl;
	}
}
