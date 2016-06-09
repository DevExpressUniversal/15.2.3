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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class XYDiagramSeriesViewBaseControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XYDiagramSeriesViewBaseControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.generalControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.XYDiagramSeriesViewBaseGeneralControl();
			this.tbIndicators = new DevExpress.XtraTab.XtraTabPage();
			this.indicatorsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.IndicatorsControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.appearanceControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SeriesViewAppearanceControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			this.tbMarker = new DevExpress.XtraTab.XtraTabPage();
			this.tbMinMarker = new DevExpress.XtraTab.XtraTabPage();
			this.minMarkerControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerControl();
			this.tbMaxMarker = new DevExpress.XtraTab.XtraTabPage();
			this.maxMarkerControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerControl();
			this.tbMarker1 = new DevExpress.XtraTab.XtraTabPage();
			this.marker1Control = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerControl();
			this.tbMarker2 = new DevExpress.XtraTab.XtraTabPage();
			this.marker2Control = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbIndicators.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbShadow.SuspendLayout();
			this.tbMinMarker.SuspendLayout();
			this.tbMaxMarker.SuspendLayout();
			this.tbMarker1.SuspendLayout();
			this.tbMarker2.SuspendLayout();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance,
			this.tbBorder,
			this.tbShadow,
			this.tbMarker,
			this.tbMinMarker,
			this.tbMaxMarker,
			this.tbMarker1,
			this.tbMarker2,
			this.tbIndicators});
			this.tbGeneral.Controls.Add(this.generalControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.generalControl, "generalControl");
			this.generalControl.Name = "generalControl";
			this.tbIndicators.Controls.Add(this.indicatorsControl);
			this.tbIndicators.Name = "tbIndicators";
			resources.ApplyResources(this.tbIndicators, "tbIndicators");
			resources.ApplyResources(this.indicatorsControl, "indicatorsControl");
			this.indicatorsControl.Name = "indicatorsControl";
			this.tbAppearance.Controls.Add(this.appearanceControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.appearanceControl, "appearanceControl");
			this.appearanceControl.Name = "appearanceControl";
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.Name = "shadowControl";
			this.tbMarker.Name = "tbMarker";
			resources.ApplyResources(this.tbMarker, "tbMarker");
			this.tbMinMarker.Controls.Add(this.minMarkerControl);
			this.tbMinMarker.Name = "tbMinMarker";
			resources.ApplyResources(this.tbMinMarker, "tbMinMarker");
			resources.ApplyResources(this.minMarkerControl, "minMarkerControl");
			this.minMarkerControl.Name = "minMarkerControl";
			this.tbMaxMarker.Controls.Add(this.maxMarkerControl);
			this.tbMaxMarker.Name = "tbMaxMarker";
			resources.ApplyResources(this.tbMaxMarker, "tbMaxMarker");
			resources.ApplyResources(this.maxMarkerControl, "maxMarkerControl");
			this.maxMarkerControl.Name = "maxMarkerControl";
			this.tbMarker1.Controls.Add(this.marker1Control);
			this.tbMarker1.Name = "tbMarker1";
			resources.ApplyResources(this.tbMarker1, "tbMarker1");
			resources.ApplyResources(this.marker1Control, "marker1Control");
			this.marker1Control.Name = "marker1Control";
			this.tbMarker2.Controls.Add(this.marker2Control);
			this.tbMarker2.Name = "tbMarker2";
			resources.ApplyResources(this.tbMarker2, "tbMarker2");
			resources.ApplyResources(this.marker2Control, "marker2Control");
			this.marker2Control.Name = "marker2Control";
			resources.ApplyResources(this, "$this");
			this.Name = "XYDiagramSeriesViewBaseControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.tbIndicators.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.tbMinMarker.ResumeLayout(false);
			this.tbMinMarker.PerformLayout();
			this.tbMaxMarker.ResumeLayout(false);
			this.tbMaxMarker.PerformLayout();
			this.tbMarker1.ResumeLayout(false);
			this.tbMarker1.PerformLayout();
			this.tbMarker2.ResumeLayout(false);
			this.tbMarker2.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbIndicators;
		private XYDiagramSeriesViewBaseGeneralControl generalControl;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private SeriesViewAppearanceControl appearanceControl;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private ShadowControl shadowControl;
		private DevExpress.XtraTab.XtraTabPage tbMarker;
		private DevExpress.XtraTab.XtraTabPage tbMinMarker;
		private DevExpress.XtraTab.XtraTabPage tbMaxMarker;
		private MarkerControl minMarkerControl;
		private MarkerControl maxMarkerControl;
		private IndicatorsControl indicatorsControl;
		private DevExpress.XtraTab.XtraTabPage tbMarker1;
		private DevExpress.XtraTab.XtraTabPage tbMarker2;
		private MarkerControl marker1Control;
		private MarkerControl marker2Control;
	}
}
