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
	partial class DoughnutOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DoughnutOptionsControl));
			this.grpDoughnutOptions = new DevExpress.XtraEditors.GroupControl();
			this.spnHoleRadius = new DevExpress.XtraEditors.SpinEdit();
			this.lblHoleRadius = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpDoughnutOptions)).BeginInit();
			this.grpDoughnutOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnHoleRadius.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpDoughnutOptions, "grpDoughnutOptions");
			this.grpDoughnutOptions.Controls.Add(this.spnHoleRadius);
			this.grpDoughnutOptions.Controls.Add(this.lblHoleRadius);
			this.grpDoughnutOptions.Name = "grpDoughnutOptions";
			resources.ApplyResources(this.spnHoleRadius, "spnHoleRadius");
			this.spnHoleRadius.Name = "spnHoleRadius";
			this.spnHoleRadius.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHoleRadius.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spnHoleRadius.Properties.ValidateOnEnterKey = true;
			this.spnHoleRadius.EditValueChanged += new System.EventHandler(this.spnHoleRadius_EditValueChanged);
			resources.ApplyResources(this.lblHoleRadius, "lblHoleRadius");
			this.lblHoleRadius.Name = "lblHoleRadius";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpDoughnutOptions);
			this.Name = "DoughnutOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpDoughnutOptions)).EndInit();
			this.grpDoughnutOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnHoleRadius.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpDoughnutOptions;
		private DevExpress.XtraEditors.SpinEdit spnHoleRadius;
		private ChartLabelControl lblHoleRadius;
	}
}
