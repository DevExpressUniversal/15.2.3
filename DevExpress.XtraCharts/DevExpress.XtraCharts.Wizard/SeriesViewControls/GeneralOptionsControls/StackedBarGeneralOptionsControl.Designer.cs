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
	partial class StackedBarGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StackedBarGeneralOptionsControl));
			this.grpBarOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlStackedGroup = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblStackedGroup = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.seriesGroupsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SeriesGroupsControl();
			((System.ComponentModel.ISupportInitialize)(this.grpBarOptions)).BeginInit();
			this.grpBarOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlStackedGroup)).BeginInit();
			this.pnlStackedGroup.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.grpBarOptions, "grpBarOptions");
			this.grpBarOptions.Controls.Add(this.pnlStackedGroup);
			this.grpBarOptions.Name = "grpBarOptions";
			resources.ApplyResources(this.pnlStackedGroup, "pnlStackedGroup");
			this.pnlStackedGroup.BackColor = System.Drawing.Color.Transparent;
			this.pnlStackedGroup.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlStackedGroup.Controls.Add(this.seriesGroupsControl);
			this.pnlStackedGroup.Controls.Add(this.lblStackedGroup);
			this.pnlStackedGroup.Name = "pnlStackedGroup";
			resources.ApplyResources(this.lblStackedGroup, "lblStackedGroup");
			this.lblStackedGroup.Name = "lblStackedGroup";
			resources.ApplyResources(this.seriesGroupsControl, "seriesGroupsControl");
			this.seriesGroupsControl.Name = "seriesGroupsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpBarOptions);
			this.Name = "StackedBarGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpBarOptions)).EndInit();
			this.grpBarOptions.ResumeLayout(false);
			this.grpBarOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlStackedGroup)).EndInit();
			this.pnlStackedGroup.ResumeLayout(false);
			this.pnlStackedGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpBarOptions;
		private ChartPanelControl pnlStackedGroup;
		private ChartLabelControl lblStackedGroup;
		private SeriesGroupsControl seriesGroupsControl;
	}
}
