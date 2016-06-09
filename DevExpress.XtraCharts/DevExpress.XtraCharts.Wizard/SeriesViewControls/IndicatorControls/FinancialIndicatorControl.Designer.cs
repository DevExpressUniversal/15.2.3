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
	partial class FinancialIndicatorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinancialIndicatorControl));
			this.point1 = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FinancialIndicatorPointControl();
			this.gcPoint1 = new DevExpress.XtraEditors.GroupControl();
			this.panelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.gcPoint2 = new DevExpress.XtraEditors.GroupControl();
			this.point2 = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FinancialIndicatorPointControl();
			((System.ComponentModel.ISupportInitialize)(this.gcPoint1)).BeginInit();
			this.gcPoint1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPoint2)).BeginInit();
			this.gcPoint2.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.point1, "point1");
			this.point1.Name = "point1";
			resources.ApplyResources(this.gcPoint1, "gcPoint1");
			this.gcPoint1.Controls.Add(this.point1);
			this.gcPoint1.Name = "gcPoint1";
			this.panelControl5.BackColor = System.Drawing.Color.Transparent;
			this.panelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl5, "panelControl5");
			this.panelControl5.Name = "panelControl5";
			resources.ApplyResources(this.gcPoint2, "gcPoint2");
			this.gcPoint2.Controls.Add(this.point2);
			this.gcPoint2.Name = "gcPoint2";
			resources.ApplyResources(this.point2, "point2");
			this.point2.Name = "point2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.gcPoint2);
			this.Controls.Add(this.panelControl5);
			this.Controls.Add(this.gcPoint1);
			this.Name = "FinancialIndicatorControl";
			((System.ComponentModel.ISupportInitialize)(this.gcPoint1)).EndInit();
			this.gcPoint1.ResumeLayout(false);
			this.gcPoint1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPoint2)).EndInit();
			this.gcPoint2.ResumeLayout(false);
			this.gcPoint2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl gcPoint1;
		private ChartPanelControl panelControl5;
		private DevExpress.XtraEditors.GroupControl gcPoint2;
		public FinancialIndicatorPointControl point1;
		public FinancialIndicatorPointControl point2;
	}
}
