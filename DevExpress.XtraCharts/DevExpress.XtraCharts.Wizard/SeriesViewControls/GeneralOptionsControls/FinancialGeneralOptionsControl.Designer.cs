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
	partial class FinancialGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinancialGeneralOptionsControl));
			this.grpStyle = new DevExpress.XtraEditors.GroupControl();
			this.styleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FinancialLineStyleControl();
			this.sepSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpReduction = new DevExpress.XtraEditors.GroupControl();
			this.reductionOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ReductionOptionsControl();
			((System.ComponentModel.ISupportInitialize)(this.grpStyle)).BeginInit();
			this.grpStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpReduction)).BeginInit();
			this.grpReduction.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.grpStyle, "grpStyle");
			this.grpStyle.Controls.Add(this.styleControl);
			this.grpStyle.Name = "grpStyle";
			resources.ApplyResources(this.styleControl, "styleControl");
			this.styleControl.Name = "styleControl";
			this.sepSeparator.BackColor = System.Drawing.Color.Transparent;
			this.sepSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSeparator, "sepSeparator");
			this.sepSeparator.Name = "sepSeparator";
			resources.ApplyResources(this.grpReduction, "grpReduction");
			this.grpReduction.Controls.Add(this.reductionOptionsControl);
			this.grpReduction.Name = "grpReduction";
			resources.ApplyResources(this.reductionOptionsControl, "reductionOptionsControl");
			this.reductionOptionsControl.Name = "reductionOptionsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpReduction);
			this.Controls.Add(this.sepSeparator);
			this.Controls.Add(this.grpStyle);
			this.Name = "FinancialGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpStyle)).EndInit();
			this.grpStyle.ResumeLayout(false);
			this.grpStyle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpReduction)).EndInit();
			this.grpReduction.ResumeLayout(false);
			this.grpReduction.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpStyle;
		private ChartPanelControl sepSeparator;
		private DevExpress.XtraEditors.GroupControl grpReduction;
		private FinancialLineStyleControl styleControl;
		private ReductionOptionsControl reductionOptionsControl;
	}
}
