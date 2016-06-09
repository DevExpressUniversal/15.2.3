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
	partial class PieGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PieGeneralOptionsControl));
			this.txtRotation = new DevExpress.XtraEditors.SpinEdit();
			this.pnlRotation = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblRotation = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblRatio = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.txtRatio = new DevExpress.XtraEditors.SpinEdit();
			this.sepRotation = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlRatio = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepRatio = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpPieOptions = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.txtRotation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRotation)).BeginInit();
			this.pnlRotation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtRatio.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepRotation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRatio)).BeginInit();
			this.pnlRatio.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPieOptions)).BeginInit();
			this.grpPieOptions.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.txtRotation, "txtRotation");
			this.txtRotation.Name = "txtRotation";
			this.txtRotation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtRotation.Properties.IsFloatValue = false;
			this.txtRotation.Properties.Mask.EditMask = resources.GetString("txtRotation.Properties.Mask.EditMask");
			this.txtRotation.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.txtRotation.Properties.ValidateOnEnterKey = true;
			this.txtRotation.EditValueChanged += new System.EventHandler(this.txtRotation_EditValueChanged);
			resources.ApplyResources(this.pnlRotation, "pnlRotation");
			this.pnlRotation.BackColor = System.Drawing.Color.Transparent;
			this.pnlRotation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlRotation.Controls.Add(this.txtRotation);
			this.pnlRotation.Controls.Add(this.lblRotation);
			this.pnlRotation.Name = "pnlRotation";
			resources.ApplyResources(this.lblRotation, "lblRotation");
			this.lblRotation.Name = "lblRotation";
			resources.ApplyResources(this.lblRatio, "lblRatio");
			this.lblRatio.Name = "lblRatio";
			resources.ApplyResources(this.txtRatio, "txtRatio");
			this.txtRatio.Name = "txtRatio";
			this.txtRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtRatio.Properties.IsFloatValue = false;
			this.txtRatio.Properties.Mask.EditMask = resources.GetString("txtRatio.Properties.Mask.EditMask");
			this.txtRatio.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtRatio.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtRatio.Properties.ValidateOnEnterKey = true;
			this.txtRatio.EditValueChanged += new System.EventHandler(this.txtRation_EditValueChanged);
			this.sepRotation.BackColor = System.Drawing.Color.Transparent;
			this.sepRotation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepRotation, "sepRotation");
			this.sepRotation.Name = "sepRotation";
			resources.ApplyResources(this.pnlRatio, "pnlRatio");
			this.pnlRatio.BackColor = System.Drawing.Color.Transparent;
			this.pnlRatio.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlRatio.Controls.Add(this.txtRatio);
			this.pnlRatio.Controls.Add(this.lblRatio);
			this.pnlRatio.Name = "pnlRatio";
			this.sepRatio.BackColor = System.Drawing.Color.Transparent;
			this.sepRatio.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepRatio, "sepRatio");
			this.sepRatio.Name = "sepRatio";
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Name = "pnlSize";
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this.grpPieOptions, "grpPieOptions");
			this.grpPieOptions.Controls.Add(this.pnlRotation);
			this.grpPieOptions.Controls.Add(this.sepRotation);
			this.grpPieOptions.Controls.Add(this.pnlRatio);
			this.grpPieOptions.Controls.Add(this.sepRatio);
			this.grpPieOptions.Controls.Add(this.pnlSize);
			this.grpPieOptions.Name = "grpPieOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpPieOptions);
			this.Controls.Add(this.panelControl4);
			this.Name = "PieGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.txtRotation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRotation)).EndInit();
			this.pnlRotation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtRatio.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepRotation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRatio)).EndInit();
			this.pnlRatio.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sepRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPieOptions)).EndInit();
			this.grpPieOptions.ResumeLayout(false);
			this.grpPieOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SpinEdit txtRotation;
		private ChartPanelControl pnlRotation;
		private ChartLabelControl lblRotation;
		private ChartLabelControl lblRatio;
		private DevExpress.XtraEditors.SpinEdit txtRatio;
		private ChartPanelControl sepRotation;
		private ChartPanelControl pnlRatio;
		private ChartPanelControl sepRatio;
		private ChartPanelControl pnlSize;
		private ChartPanelControl panelControl4;
		private DevExpress.XtraEditors.GroupControl grpPieOptions;
	}
}
