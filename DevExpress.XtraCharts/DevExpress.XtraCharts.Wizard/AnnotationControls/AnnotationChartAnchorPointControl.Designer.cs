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

namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	partial class AnnotationChartAnchorPointControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationChartAnchorPointControl));
			this.pnlX = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnX = new DevExpress.XtraEditors.SpinEdit();
			this.lblX = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepX = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlY = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnY = new DevExpress.XtraEditors.SpinEdit();
			this.lblY = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlX)).BeginInit();
			this.pnlX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnX.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlY)).BeginInit();
			this.pnlY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnY.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlX, "pnlX");
			this.pnlX.BackColor = System.Drawing.Color.Transparent;
			this.pnlX.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlX.Controls.Add(this.spnX);
			this.pnlX.Controls.Add(this.lblX);
			this.pnlX.Name = "pnlX";
			resources.ApplyResources(this.spnX, "spnX");
			this.spnX.Name = "spnX";
			this.spnX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnX.Properties.IsFloatValue = false;
			this.spnX.Properties.Mask.EditMask = resources.GetString("spnX.Properties.Mask.EditMask");
			this.spnX.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnX.EditValueChanged += new System.EventHandler(this.spnX_EditValueChanged);
			resources.ApplyResources(this.lblX, "lblX");
			this.lblX.Name = "lblX";
			this.sepX.BackColor = System.Drawing.Color.Transparent;
			this.sepX.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepX, "sepX");
			this.sepX.Name = "sepX";
			resources.ApplyResources(this.pnlY, "pnlY");
			this.pnlY.BackColor = System.Drawing.Color.Transparent;
			this.pnlY.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlY.Controls.Add(this.spnY);
			this.pnlY.Controls.Add(this.lblY);
			this.pnlY.Name = "pnlY";
			resources.ApplyResources(this.spnY, "spnY");
			this.spnY.Name = "spnY";
			this.spnY.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnY.Properties.IsFloatValue = false;
			this.spnY.Properties.Mask.EditMask = resources.GetString("spnY.Properties.Mask.EditMask");
			this.spnY.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnY.EditValueChanged += new System.EventHandler(this.spnY_EditValueChanged);
			resources.ApplyResources(this.lblY, "lblY");
			this.lblY.Name = "lblY";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlY);
			this.Controls.Add(this.sepX);
			this.Controls.Add(this.pnlX);
			this.Name = "AnnotationChartAnchorPointControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlX)).EndInit();
			this.pnlX.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnX.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlY)).EndInit();
			this.pnlY.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnY.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlX;
		private DevExpress.XtraEditors.SpinEdit spnX;
		private ChartLabelControl lblX;
		private ChartPanelControl sepX;
		private ChartPanelControl pnlY;
		private DevExpress.XtraEditors.SpinEdit spnY;
		private ChartLabelControl lblY;
	}
}
