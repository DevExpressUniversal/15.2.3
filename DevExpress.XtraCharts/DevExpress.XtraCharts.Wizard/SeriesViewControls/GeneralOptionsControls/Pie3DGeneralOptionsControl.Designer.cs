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
	partial class Pie3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pie3DGeneralOptionsControl));
			this.grpPieOptions = new DevExpress.XtraEditors.GroupControl();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDepth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDepth = new DevExpress.XtraEditors.SpinEdit();
			this.lblDepth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepDepth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpPieOptions)).BeginInit();
			this.grpPieOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDepth)).BeginInit();
			this.pnlDepth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDepth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDepth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpPieOptions, "grpPieOptions");
			this.grpPieOptions.Controls.Add(this.panelControl4);
			this.grpPieOptions.Controls.Add(this.pnlDepth);
			this.grpPieOptions.Controls.Add(this.sepDepth);
			this.grpPieOptions.Controls.Add(this.pnlSize);
			this.grpPieOptions.Name = "grpPieOptions";
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this.pnlDepth, "pnlDepth");
			this.pnlDepth.BackColor = System.Drawing.Color.Transparent;
			this.pnlDepth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDepth.Controls.Add(this.txtDepth);
			this.pnlDepth.Controls.Add(this.lblDepth);
			this.pnlDepth.Name = "pnlDepth";
			resources.ApplyResources(this.txtDepth, "txtDepth");
			this.txtDepth.Name = "txtDepth";
			this.txtDepth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDepth.Properties.IsFloatValue = false;
			this.txtDepth.Properties.Mask.EditMask = resources.GetString("txtDepth.Properties.Mask.EditMask");
			this.txtDepth.Properties.MaxValue = new decimal(new int[] {
			99,
			0,
			0,
			0});
			this.txtDepth.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtDepth.Properties.ValidateOnEnterKey = true;
			this.txtDepth.EditValueChanged += new System.EventHandler(this.txtDepth_EditValueChanged);
			resources.ApplyResources(this.lblDepth, "lblDepth");
			this.lblDepth.Name = "lblDepth";
			this.sepDepth.BackColor = System.Drawing.Color.Transparent;
			this.sepDepth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepDepth, "sepDepth");
			this.sepDepth.Name = "sepDepth";
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Name = "pnlSize";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpPieOptions);
			this.Name = "Pie3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpPieOptions)).EndInit();
			this.grpPieOptions.ResumeLayout(false);
			this.grpPieOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDepth)).EndInit();
			this.pnlDepth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDepth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDepth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpPieOptions;
		private ChartPanelControl panelControl4;
		private ChartPanelControl pnlDepth;
		private DevExpress.XtraEditors.SpinEdit txtDepth;
		private ChartLabelControl lblDepth;
		private ChartPanelControl sepDepth;
		private ChartPanelControl pnlSize;
	}
}
