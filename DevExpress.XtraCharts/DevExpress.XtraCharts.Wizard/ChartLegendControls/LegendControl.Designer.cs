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

using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	partial class LegendControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendControl));
			this.tbcPageControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.legendGeneralControl = new DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendGeneralControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.tbInterior = new DevExpress.XtraTab.XtraTabPage();
			this.interiorControl = new DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendInteriorControl();
			this.pnlEditors = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.tbMarker = new DevExpress.XtraTab.XtraTabPage();
			this.markerControl = new DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendMarkerControl();
			this.tbText = new DevExpress.XtraTab.XtraTabPage();
			this.textControl = new DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendTextControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.appearanceControl = new DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendAppearanceControl();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPageControl)).BeginInit();
			this.tbcPageControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			this.tbAppearance.SuspendLayout();
			this.tbInterior.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).BeginInit();
			this.tbMarker.SuspendLayout();
			this.tbText.SuspendLayout();
			this.tbBorder.SuspendLayout();
			this.tbShadow.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcPageControl, "tbcPageControl");
			this.tbcPageControl.Name = "tbcPageControl";
			this.tbcPageControl.SelectedTabPage = this.tbGeneral;
			this.tbcPageControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance,
			this.tbInterior,
			this.tbMarker,
			this.tbText,
			this.tbBorder,
			this.tbShadow});
			this.tbGeneral.Controls.Add(this.legendGeneralControl);
			this.tbGeneral.Controls.Add(this.panelControl1);
			this.tbGeneral.Controls.Add(this.labelControl1);
			this.tbGeneral.Controls.Add(this.chVisible);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.legendGeneralControl, "legendGeneralControl");
			this.legendGeneralControl.Name = "legendGeneralControl";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.AllowGrayed = true;
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckStateChanged += new System.EventHandler(this.chVisible_CheckStateChanged);
			this.tbAppearance.Controls.Add(this.backgroundControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.tbInterior.Controls.Add(this.interiorControl);
			this.tbInterior.Controls.Add(this.pnlEditors);
			this.tbInterior.Name = "tbInterior";
			resources.ApplyResources(this.tbInterior, "tbInterior");
			resources.ApplyResources(this.interiorControl, "interiorControl");
			this.interiorControl.Name = "interiorControl";
			resources.ApplyResources(this.pnlEditors, "pnlEditors");
			this.pnlEditors.BackColor = System.Drawing.Color.Transparent;
			this.pnlEditors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlEditors.Name = "pnlEditors";
			this.tbMarker.Controls.Add(this.markerControl);
			this.tbMarker.Name = "tbMarker";
			resources.ApplyResources(this.tbMarker, "tbMarker");
			resources.ApplyResources(this.markerControl, "markerControl");
			this.markerControl.Name = "markerControl";
			this.tbText.Controls.Add(this.textControl);
			this.tbText.Name = "tbText";
			resources.ApplyResources(this.tbText, "tbText");
			this.textControl.AllowDrop = true;
			resources.ApplyResources(this.textControl, "textControl");
			this.textControl.Name = "textControl";
			this.tbBorder.Controls.Add(this.appearanceControl);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.appearanceControl, "appearanceControl");
			this.appearanceControl.Name = "appearanceControl";
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.Name = "shadowControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcPageControl);
			this.Name = "LegendControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPageControl)).EndInit();
			this.tbcPageControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbInterior.ResumeLayout(false);
			this.tbInterior.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).EndInit();
			this.tbMarker.ResumeLayout(false);
			this.tbMarker.PerformLayout();
			this.tbText.ResumeLayout(false);
			this.tbText.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private DevExpress.XtraTab.XtraTabPage tbMarker;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl shadowControl;
		private DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendGeneralControl legendGeneralControl;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private ChartPanelControl panelControl1;
		private DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendAppearanceControl appearanceControl;
		private DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendMarkerControl markerControl;
		private DevExpress.XtraTab.XtraTabPage tbText;
		private DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendTextControl textControl;
		public DevExpress.XtraTab.XtraTabControl tbcPageControl;
		private BackgroundControl backgroundControl;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraTab.XtraTabPage tbInterior;
		private ChartPanelControl pnlEditors;
		private DevExpress.XtraCharts.Wizard.ChartLegendControls.LegendInteriorControl interiorControl;
	}
}
