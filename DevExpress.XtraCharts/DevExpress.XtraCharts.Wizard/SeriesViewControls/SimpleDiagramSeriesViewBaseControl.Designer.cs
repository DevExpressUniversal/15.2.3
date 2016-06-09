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
	partial class SimpleDiagramSeriesViewBaseControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleDiagramSeriesViewBaseControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.appearanceControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SeriesViewAppearanceControl();
			this.tbExplodedPoints = new DevExpress.XtraTab.XtraTabPage();
			this.explodedPointsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ExplodedPointsControl();
			this.tbTitles = new DevExpress.XtraTab.XtraTabPage();
			this.titlesControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.AuxiliaryControls.SeriesTitlesControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.borderControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbExplodedPoints.SuspendLayout();
			this.tbTitles.SuspendLayout();
			this.tbBorder.SuspendLayout();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance,
			this.tbBorder,
			this.tbExplodedPoints,
			this.tbTitles});
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			this.tbAppearance.Controls.Add(this.appearanceControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.appearanceControl, "appearanceControl");
			this.appearanceControl.Name = "appearanceControl";
			this.tbExplodedPoints.Controls.Add(this.explodedPointsControl);
			this.tbExplodedPoints.Name = "tbExplodedPoints";
			resources.ApplyResources(this.tbExplodedPoints, "tbExplodedPoints");
			resources.ApplyResources(this.explodedPointsControl, "explodedPointsControl");
			this.explodedPointsControl.Name = "explodedPointsControl";
			this.tbTitles.Controls.Add(this.titlesControl);
			this.tbTitles.Name = "tbTitles";
			resources.ApplyResources(this.tbTitles, "tbTitles");
			resources.ApplyResources(this.titlesControl, "titlesControl");
			this.titlesControl.Name = "titlesControl";
			this.tbBorder.Controls.Add(this.borderControl);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.borderControl, "borderControl");
			this.borderControl.Name = "borderControl";
			resources.ApplyResources(this, "$this");
			this.Name = "SimpleDiagramSeriesViewBaseControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbExplodedPoints.ResumeLayout(false);
			this.tbExplodedPoints.PerformLayout();
			this.tbTitles.ResumeLayout(false);
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbExplodedPoints;
		private DevExpress.XtraTab.XtraTabPage tbTitles;
		private SeriesViewAppearanceControl appearanceControl;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private BorderControl borderControl;
		private ExplodedPointsControl explodedPointsControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.AuxiliaryControls.SeriesTitlesControl titlesControl;
	}
}
