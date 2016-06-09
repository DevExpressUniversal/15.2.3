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
	partial class RadarAxisXControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarAxisXControl));
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.generalTabsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisGeneralTabsControl();
			this.tbScaleOptions = new DevExpress.XtraTab.XtraTabPage();
			this.axisScaleOptionsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisScaleOptionsControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.axisAppearanceControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.RadarAxisAppearanceControl();
			this.tbElements = new DevExpress.XtraTab.XtraTabPage();
			this.axisElementsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.RadarAxesElementsControl();
			this.tbLabels = new DevExpress.XtraTab.XtraTabPage();
			this.radarAxisXLabelControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.LabelsGeneralControl();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbScaleOptions.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbElements.SuspendLayout();
			this.tbLabels.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.tbGeneral;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbScaleOptions,
			this.tbAppearance,
			this.tbElements,
			this.tbLabels});
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
			this.tbElements.Controls.Add(this.axisElementsControl);
			this.tbElements.Name = "tbElements";
			resources.ApplyResources(this.tbElements, "tbElements");
			resources.ApplyResources(this.axisElementsControl, "axisElementsControl");
			this.axisElementsControl.Name = "axisElementsControl";
			this.tbLabels.Controls.Add(this.radarAxisXLabelControl);
			this.tbLabels.Name = "tbLabels";
			resources.ApplyResources(this.tbLabels, "tbLabels");
			resources.ApplyResources(this.radarAxisXLabelControl, "radarAxisXLabelControl");
			this.radarAxisXLabelControl.Name = "radarAxisXLabelControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.xtraTabControl);
			this.Name = "RadarAxisXControl";
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbScaleOptions.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbElements.ResumeLayout(false);
			this.tbElements.PerformLayout();
			this.tbLabels.ResumeLayout(false);
			this.tbLabels.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl xtraTabControl;
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbLabels;
		private RadarAxisAppearanceControl axisAppearanceControl;
		private AxisGeneralTabsControl generalTabsControl;
		private DevExpress.XtraTab.XtraTabPage tbElements;
		private RadarAxesElementsControl axisElementsControl;
		private LabelsGeneralControl radarAxisXLabelControl;
		private XtraTab.XtraTabPage tbScaleOptions;
		private AxisScaleOptionsControl axisScaleOptionsControl;
	}
}
