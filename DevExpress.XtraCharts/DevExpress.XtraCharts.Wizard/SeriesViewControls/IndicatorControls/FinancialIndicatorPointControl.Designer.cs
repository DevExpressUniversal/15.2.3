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
	partial class FinancialIndicatorPointControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinancialIndicatorPointControl));
			this.lblArgument = new DevExpress.XtraEditors.LabelControl();
			this.lblValueLevel = new DevExpress.XtraEditors.LabelControl();
			this.txtArgument = new DevExpress.XtraEditors.TextEdit();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cmbValueLevel = new DevExpress.XtraEditors.ComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.txtArgument.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbValueLevel.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblArgument, "lblArgument");
			this.lblArgument.Name = "lblArgument";
			resources.ApplyResources(this.lblValueLevel, "lblValueLevel");
			this.lblValueLevel.Name = "lblValueLevel";
			resources.ApplyResources(this.txtArgument, "txtArgument");
			this.txtArgument.Name = "txtArgument";
			this.txtArgument.Properties.ValidateOnEnterKey = true;
			this.txtArgument.Validating += new System.ComponentModel.CancelEventHandler(this.txtArgument_Validating);
			this.txtArgument.Validated += new System.EventHandler(this.txtArgument_Validated);
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.txtArgument);
			this.chartPanelControl3.Controls.Add(this.lblArgument);
			this.chartPanelControl3.Name = "chartPanelControl3";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.cmbValueLevel);
			this.chartPanelControl2.Controls.Add(this.lblValueLevel);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.cmbValueLevel, "cmbValueLevel");
			this.cmbValueLevel.Name = "cmbValueLevel";
			this.cmbValueLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbValueLevel.Properties.Buttons"))))});
			this.cmbValueLevel.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbValueLevel.SelectedIndexChanged += new System.EventHandler(this.cmbValueLevel_SelectedIndexChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl3);
			this.Name = "FinancialIndicatorPointControl";
			((System.ComponentModel.ISupportInitialize)(this.txtArgument.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbValueLevel.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl lblArgument;
		private DevExpress.XtraEditors.LabelControl lblValueLevel;
		private DevExpress.XtraEditors.TextEdit txtArgument;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl2;
		private DevExpress.XtraEditors.ComboBoxEdit cmbValueLevel;
	}
}
