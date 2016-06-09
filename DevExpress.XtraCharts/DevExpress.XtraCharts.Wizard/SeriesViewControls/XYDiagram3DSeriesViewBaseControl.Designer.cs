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
	partial class XYDiagram3DSeriesViewBaseControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XYDiagram3DSeriesViewBaseControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.appearanceControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SeriesViewAppearanceControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance});
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			this.tbAppearance.Controls.Add(this.appearanceControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.appearanceControl, "appearanceControl");
			this.appearanceControl.Name = "appearanceControl";
			this.Name = "XYDiagram3DSeriesViewBaseControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private SeriesViewAppearanceControl appearanceControl;
	}
}
