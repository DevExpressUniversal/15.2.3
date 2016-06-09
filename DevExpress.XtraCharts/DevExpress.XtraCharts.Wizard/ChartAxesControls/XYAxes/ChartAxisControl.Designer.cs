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
	partial class ChartAxisControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartAxisControl));
			this.tbcTabPages = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.generalTabsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisGeneralTabsControl();
			this.tbScaleOptions = new DevExpress.XtraTab.XtraTabPage();
			this.axisScaleOptionsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisScaleOptionsControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.axisAppearanceControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisAppearanceControl();
			this.tbElements = new DevExpress.XtraTab.XtraTabPage();
			this.axesElementsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxesElementsControl();
			this.tbLabels = new DevExpress.XtraTab.XtraTabPage();
			this.labelsGeneralControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.LabelsGeneralControl();
			this.tbStrips = new DevExpress.XtraTab.XtraTabPage();
			this.stripsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.StripsControl();
			this.tbConstantLines = new DevExpress.XtraTab.XtraTabPage();
			this.constantLinesControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLinesControl();
			this.tbScaleBreaks = new DevExpress.XtraTab.XtraTabPage();
			this.scaleBreaksControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreaksControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcTabPages)).BeginInit();
			this.tbcTabPages.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbScaleOptions.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbElements.SuspendLayout();
			this.tbLabels.SuspendLayout();
			this.tbStrips.SuspendLayout();
			this.tbConstantLines.SuspendLayout();
			this.tbScaleBreaks.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcTabPages, "tbcTabPages");
			this.tbcTabPages.Name = "tbcTabPages";
			this.tbcTabPages.SelectedTabPage = this.tbGeneral;
			this.tbcTabPages.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbScaleOptions,
			this.tbAppearance,
			this.tbElements,
			this.tbLabels,
			this.tbStrips,
			this.tbConstantLines,
			this.tbScaleBreaks});
			this.tbGeneral.Controls.Add(this.generalTabsControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.generalTabsControl, "generalTabsControl");
			this.generalTabsControl.Name = "generalTabsControl";
			this.tbScaleOptions.Controls.Add(this.axisScaleOptionsControl);
			this.tbScaleOptions.Name = "tbScaleOptions";
			resources.ApplyResources(this.tbScaleOptions, "tbScaleOptions");
			resources.ApplyResources(this.axisScaleOptionsControl, "axisScaleOptionsControl");
			this.axisScaleOptionsControl.Name = "axisScaleOptionsControl";
			this.tbAppearance.Controls.Add(this.axisAppearanceControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.axisAppearanceControl, "axisAppearanceControl");
			this.axisAppearanceControl.Name = "axisAppearanceControl";
			this.tbElements.Controls.Add(this.axesElementsControl);
			this.tbElements.Name = "tbElements";
			resources.ApplyResources(this.tbElements, "tbElements");
			resources.ApplyResources(this.axesElementsControl, "axesElementsControl");
			this.axesElementsControl.Name = "axesElementsControl";
			this.tbLabels.Controls.Add(this.labelsGeneralControl);
			this.tbLabels.Name = "tbLabels";
			resources.ApplyResources(this.tbLabels, "tbLabels");
			resources.ApplyResources(this.labelsGeneralControl, "labelsGeneralControl");
			this.labelsGeneralControl.Name = "labelsGeneralControl";
			this.tbStrips.Controls.Add(this.stripsControl);
			this.tbStrips.Name = "tbStrips";
			resources.ApplyResources(this.tbStrips, "tbStrips");
			resources.ApplyResources(this.stripsControl, "stripsControl");
			this.stripsControl.Name = "stripsControl";
			this.tbConstantLines.Controls.Add(this.constantLinesControl);
			this.tbConstantLines.Name = "tbConstantLines";
			resources.ApplyResources(this.tbConstantLines, "tbConstantLines");
			resources.ApplyResources(this.constantLinesControl, "constantLinesControl");
			this.constantLinesControl.Name = "constantLinesControl";
			this.tbScaleBreaks.Controls.Add(this.scaleBreaksControl);
			this.tbScaleBreaks.Name = "tbScaleBreaks";
			resources.ApplyResources(this.tbScaleBreaks, "tbScaleBreaks");
			resources.ApplyResources(this.scaleBreaksControl, "scaleBreaksControl");
			this.scaleBreaksControl.Name = "scaleBreaksControl";
			this.Controls.Add(this.tbcTabPages);
			this.Name = "ChartAxisControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.tbcTabPages)).EndInit();
			this.tbcTabPages.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbScaleOptions.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbElements.ResumeLayout(false);
			this.tbElements.PerformLayout();
			this.tbLabels.ResumeLayout(false);
			this.tbStrips.ResumeLayout(false);
			this.tbStrips.PerformLayout();
			this.tbConstantLines.ResumeLayout(false);
			this.tbConstantLines.PerformLayout();
			this.tbScaleBreaks.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisAppearanceControl axisAppearanceControl;
		private DevExpress.XtraTab.XtraTabPage tbElements;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxesElementsControl axesElementsControl;
		private DevExpress.XtraTab.XtraTabPage tbLabels;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.LabelsGeneralControl labelsGeneralControl;
		private DevExpress.XtraTab.XtraTabPage tbStrips;
		private DevExpress.XtraTab.XtraTabPage tbConstantLines;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.StripsControl stripsControl;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.ConstantLinesControl constantLinesControl;
		public DevExpress.XtraTab.XtraTabControl tbcTabPages;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisGeneralTabsControl generalTabsControl;
		private DevExpress.XtraTab.XtraTabPage tbScaleBreaks;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreaksControl scaleBreaksControl;
		private XtraTab.XtraTabPage tbScaleOptions;
		private AxisScaleOptionsControl axisScaleOptionsControl;
	}
}
