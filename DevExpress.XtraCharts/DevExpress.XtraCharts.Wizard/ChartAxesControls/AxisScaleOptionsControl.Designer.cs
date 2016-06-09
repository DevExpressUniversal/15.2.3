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
	partial class AxisScaleOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisScaleOptionsControl));
			this.axisDateTimeScaleOptionsControl = new DevExpress.XtraCharts.Wizard.AxisDateTimeScaleOptionsControl();
			this.axisNumericScaleOptionsControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisNumericScaleOptionsControl();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.tbWorkdays = new DevExpress.XtraTab.XtraTabPage();
			this.workdaysControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.WorkdaysControl();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbWorkdays.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.axisDateTimeScaleOptionsControl, "axisDateTimeScaleOptionsControl");
			this.axisDateTimeScaleOptionsControl.Name = "axisDateTimeScaleOptionsControl";
			resources.ApplyResources(this.axisNumericScaleOptionsControl, "axisNumericScaleOptionsControl");
			this.axisNumericScaleOptionsControl.Name = "axisNumericScaleOptionsControl";
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tbGeneral;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbWorkdays});
			this.tbGeneral.Controls.Add(this.axisNumericScaleOptionsControl);
			this.tbGeneral.Controls.Add(this.axisDateTimeScaleOptionsControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			this.tbWorkdays.Controls.Add(this.workdaysControl);
			this.tbWorkdays.Name = "tbWorkdays";
			resources.ApplyResources(this.tbWorkdays, "tbWorkdays");
			resources.ApplyResources(this.workdaysControl, "workdaysControl");
			this.workdaysControl.Name = "workdaysControl";
			this.Controls.Add(this.tabControl);
			this.Name = "AxisScaleOptionsControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.tbWorkdays.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private AxisDateTimeScaleOptionsControl axisDateTimeScaleOptionsControl;
		private AxisNumericScaleOptionsControl axisNumericScaleOptionsControl;
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage tbGeneral;
		private XtraTab.XtraTabPage tbWorkdays;
		private WorkdaysControl workdaysControl;
	}
}
