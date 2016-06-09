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

namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	partial class LabelControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelControl));
			this.tbcPagesControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.generalControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.SeriesLabelGeneralControl();
			this.tbPointOptions = new DevExpress.XtraTab.XtraTabPage();
			this.pointOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesLabelTextPatternControl();
			this.tbConnector = new DevExpress.XtraTab.XtraTabPage();
			this.labelConnectorControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.ConnectorControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.borderControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbPointOptions.SuspendLayout();
			this.tbConnector.SuspendLayout();
			this.tbBorder.SuspendLayout();
			this.tbShadow.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.Name = "tbcPagesControl";
			this.tbcPagesControl.SelectedTabPage = this.tbAppearance;
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbPointOptions,
			this.tbConnector,
			this.tbAppearance,
			this.tbBorder,
			this.tbShadow});
			this.tbAppearance.Controls.Add(this.backgroundControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.tbGeneral.Controls.Add(this.generalControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.generalControl, "generalControl");
			this.generalControl.Name = "generalControl";
			this.tbPointOptions.Controls.Add(this.pointOptionsControl);
			this.tbPointOptions.Name = "tbPointOptions";
			resources.ApplyResources(this.tbPointOptions, "tbPointOptions");
			resources.ApplyResources(this.pointOptionsControl, "pointOptionsControl");
			this.pointOptionsControl.Name = "pointOptionsControl";
			this.tbConnector.Controls.Add(this.labelConnectorControl);
			this.tbConnector.Name = "tbConnector";
			resources.ApplyResources(this.tbConnector, "tbConnector");
			resources.ApplyResources(this.labelConnectorControl, "labelConnectorControl");
			this.labelConnectorControl.Name = "labelConnectorControl";
			this.tbBorder.Controls.Add(this.borderControl);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.borderControl, "borderControl");
			this.borderControl.Name = "borderControl";
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.Name = "shadowControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcPagesControl);
			this.Name = "LabelControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.tbPointOptions.ResumeLayout(false);
			this.tbPointOptions.PerformLayout();
			this.tbConnector.ResumeLayout(false);
			this.tbConnector.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.XtraTab.XtraTabControl tbcPagesControl;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl shadowControl;
		protected DevExpress.XtraTab.XtraTabPage tbConnector;
		protected ConnectorControl labelConnectorControl;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl borderControl;
		private SeriesLabelGeneralControl generalControl;
		private BackgroundControl backgroundControl;
		private DevExpress.XtraTab.XtraTabPage tbPointOptions;
		private SeriesLabelTextPatternControl pointOptionsControl;
	}
}
