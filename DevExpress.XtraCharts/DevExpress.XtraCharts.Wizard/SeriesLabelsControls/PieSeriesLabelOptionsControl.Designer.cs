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
	partial class PieSeriesLabelOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PieSeriesLabelOptionsControl));
			this.pnlPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPosition = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnIndent = new DevExpress.XtraEditors.SpinEdit();
			this.lblIndent = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).BeginInit();
			this.pnlPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepIndent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlIndent)).BeginInit();
			this.pnlIndent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnIndent.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlPosition, "pnlPosition");
			this.pnlPosition.BackColor = System.Drawing.Color.Transparent;
			this.pnlPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPosition.Controls.Add(this.cbPosition);
			this.pnlPosition.Controls.Add(this.lblPosition);
			this.pnlPosition.Name = "pnlPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.lblPosition, "lblPosition");
			this.lblPosition.Name = "lblPosition";
			this.sepIndent.BackColor = System.Drawing.Color.Transparent;
			this.sepIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepIndent, "sepIndent");
			this.sepIndent.Name = "sepIndent";
			resources.ApplyResources(this.pnlIndent, "pnlIndent");
			this.pnlIndent.BackColor = System.Drawing.Color.Transparent;
			this.pnlIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlIndent.Controls.Add(this.spnIndent);
			this.pnlIndent.Controls.Add(this.lblIndent);
			this.pnlIndent.Name = "pnlIndent";
			resources.ApplyResources(this.spnIndent, "spnIndent");
			this.spnIndent.Name = "spnIndent";
			this.spnIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnIndent.Properties.IsFloatValue = false;
			this.spnIndent.Properties.Mask.EditMask = resources.GetString("spnIndent.Properties.Mask.EditMask");
			this.spnIndent.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnIndent.EditValueChanged += new System.EventHandler(this.spnIndent_EditValueChanged);
			resources.ApplyResources(this.lblIndent, "lblIndent");
			this.lblIndent.Name = "lblIndent";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlIndent);
			this.Controls.Add(this.sepIndent);
			this.Controls.Add(this.pnlPosition);
			this.Name = "PieSeriesLabelOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).EndInit();
			this.pnlPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepIndent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlIndent)).EndInit();
			this.pnlIndent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnIndent.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlPosition;
		private ChartLabelControl lblPosition;
		private DevExpress.XtraEditors.ComboBoxEdit cbPosition;
		private ChartPanelControl sepIndent;
		private ChartPanelControl pnlIndent;
		private DevExpress.XtraEditors.SpinEdit spnIndent;
		private ChartLabelControl lblIndent;
	}
}
