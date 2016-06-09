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
	partial class SeriesLabelGeneralControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesLabelGeneralControl));
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.overlappingSettingsControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.OverlappingSettingsControl();
			this.sepLocation = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepOverlappingSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepTextSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.textSettingsControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.TextSettingsControl();
			this.pnlResolveOverlappingSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelLocationControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.LayoutSettingsControl();
			this.pnlOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepLocation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepOverlappingSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTextSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlResolveOverlappingSettings)).BeginInit();
			this.pnlResolveOverlappingSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlOptions)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.AllowGrayed = true;
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckStateChanged += new System.EventHandler(this.chVisible_CheckStateChanged);
			resources.ApplyResources(this.overlappingSettingsControl, "overlappingSettingsControl");
			this.overlappingSettingsControl.Name = "overlappingSettingsControl";
			resources.ApplyResources(this.sepLocation, "sepLocation");
			this.sepLocation.BackColor = System.Drawing.Color.Transparent;
			this.sepLocation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.sepLocation.Name = "sepLocation";
			this.sepOverlappingSettings.BackColor = System.Drawing.Color.Transparent;
			this.sepOverlappingSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepOverlappingSettings, "sepOverlappingSettings");
			this.sepOverlappingSettings.Name = "sepOverlappingSettings";
			this.sepTextSettings.BackColor = System.Drawing.Color.Transparent;
			this.sepTextSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepTextSettings, "sepTextSettings");
			this.sepTextSettings.Name = "sepTextSettings";
			resources.ApplyResources(this.textSettingsControl, "textSettingsControl");
			this.textSettingsControl.Name = "textSettingsControl";
			resources.ApplyResources(this.pnlResolveOverlappingSettings, "pnlResolveOverlappingSettings");
			this.pnlResolveOverlappingSettings.BackColor = System.Drawing.Color.Transparent;
			this.pnlResolveOverlappingSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlResolveOverlappingSettings.Controls.Add(this.overlappingSettingsControl);
			this.pnlResolveOverlappingSettings.Controls.Add(this.sepOverlappingSettings);
			this.pnlResolveOverlappingSettings.Name = "pnlResolveOverlappingSettings";
			resources.ApplyResources(this.labelLocationControl, "labelLocationControl");
			this.labelLocationControl.Name = "labelLocationControl";
			resources.ApplyResources(this.pnlOptions, "pnlOptions");
			this.pnlOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnlOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlOptions.Name = "pnlOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlResolveOverlappingSettings);
			this.Controls.Add(this.textSettingsControl);
			this.Controls.Add(this.sepTextSettings);
			this.Controls.Add(this.labelLocationControl);
			this.Controls.Add(this.sepLocation);
			this.Controls.Add(this.pnlOptions);
			this.Controls.Add(this.chVisible);
			this.Name = "SeriesLabelGeneralControl";
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepLocation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepOverlappingSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTextSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlResolveOverlappingSettings)).EndInit();
			this.pnlResolveOverlappingSettings.ResumeLayout(false);
			this.pnlResolveOverlappingSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private OverlappingSettingsControl overlappingSettingsControl;
		private ChartPanelControl sepLocation;
		private ChartPanelControl sepOverlappingSettings;
		private ChartPanelControl sepTextSettings;
		private TextSettingsControl textSettingsControl;
		private ChartPanelControl pnlResolveOverlappingSettings;
		private LayoutSettingsControl labelLocationControl;
		private ChartPanelControl pnlOptions;
	}
}
