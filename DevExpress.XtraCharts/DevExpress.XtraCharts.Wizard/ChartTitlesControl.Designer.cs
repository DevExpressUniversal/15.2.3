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

using DevExpress.XtraCharts.Wizard.ChartTitleControls;
namespace DevExpress.XtraCharts.Wizard {
	partial class ChartTitlesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartTitlesControl));
			this.titleControl = new DevExpress.XtraCharts.Wizard.ChartTitleControls.TitleControl();
			this.panelControl10 = new DevExpress.XtraEditors.PanelControl();
			this.titleListRedactControl = new DevExpress.XtraCharts.Wizard.ChartTitleControls.TitleListRedactControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).BeginInit();
			this.panelControl10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanel, "chartPanel");
			this.splitter.Panel2.Controls.Add(this.titleControl);
			this.splitter.Panel2.Controls.Add(this.panelControl10);
			resources.ApplyResources(this.splitter, "splitter");
			this.splitter.SplitterPosition = 305;
			resources.ApplyResources(this.titleControl, "titleControl");
			this.titleControl.Name = "titleControl";
			resources.ApplyResources(this.panelControl10, "panelControl10");
			this.panelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl10.Controls.Add(this.titleListRedactControl);
			this.panelControl10.Controls.Add(this.chartPanelControl1);
			this.panelControl10.Name = "panelControl10";
			resources.ApplyResources(this.titleListRedactControl, "titleListRedactControl");
			this.titleListRedactControl.Name = "titleListRedactControl";
			this.titleListRedactControl.SelectedElementChanged += new DevExpress.XtraCharts.Wizard.SelectedElementChangedEventHandler(this.titleListRedactControl_SelectedElementChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this, "$this");
			this.Name = "ChartTitlesControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).EndInit();
			this.panelControl10.ResumeLayout(false);
			this.panelControl10.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private TitleControl titleControl;
		private DevExpress.XtraEditors.PanelControl panelControl10;
		private DevExpress.XtraCharts.Wizard.ChartTitleControls.TitleListRedactControl titleListRedactControl;
		private ChartPanelControl chartPanelControl1;
	}
}
