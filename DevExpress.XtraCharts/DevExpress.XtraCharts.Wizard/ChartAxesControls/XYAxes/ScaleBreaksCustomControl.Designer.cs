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
	partial class ScaleBreaksCustomControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleBreaksCustomControl));
			this.scaleBreakListRedactControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreakListRedactControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.scaleBreakGeneralControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.ScaleBreakGeneralControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.scaleBreakListRedactControl, "scaleBreakListRedactControl");
			this.scaleBreakListRedactControl.Name = "scaleBreakListRedactControl";
			this.scaleBreakListRedactControl.SelectedElementChanged += new DevExpress.XtraCharts.Wizard.SelectedElementChangedEventHandler(this.scaleBreakListRedactControl_SelectedElementChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.scaleBreakGeneralControl, "scaleBreakGeneralControl");
			this.scaleBreakGeneralControl.Name = "scaleBreakGeneralControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.scaleBreakGeneralControl);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.scaleBreakListRedactControl);
			this.Name = "ScaleBreaksCustomControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ScaleBreakListRedactControl scaleBreakListRedactControl;
		private ChartPanelControl chartPanelControl1;
		private ScaleBreakGeneralControl scaleBreakGeneralControl;
	}
}
