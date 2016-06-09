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
	partial class ScaleBreakGeneralControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleBreakGeneralControl));
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.sepVisible = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlName = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtName = new DevExpress.XtraEditors.TextEdit();
			this.lblName = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepName = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtEdge1 = new DevExpress.XtraEditors.TextEdit();
			this.lblEdge1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepEdge = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlEdge2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtEdge2 = new DevExpress.XtraEditors.TextEdit();
			this.lblEdge2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.pnlControls = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepVisible)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlName)).BeginInit();
			this.pnlName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtEdge1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepEdge)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlEdge2)).BeginInit();
			this.pnlEdge2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtEdge2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.sepVisible.BackColor = System.Drawing.Color.Transparent;
			this.sepVisible.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepVisible, "sepVisible");
			this.sepVisible.Name = "sepVisible";
			resources.ApplyResources(this.pnlName, "pnlName");
			this.pnlName.BackColor = System.Drawing.Color.Transparent;
			this.pnlName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlName.Controls.Add(this.txtName);
			this.pnlName.Controls.Add(this.lblName);
			this.pnlName.Name = "pnlName";
			resources.ApplyResources(this.txtName, "txtName");
			this.txtName.EnterMoveNextControl = true;
			this.txtName.Name = "txtName";
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.Name = "lblName";
			this.sepName.BackColor = System.Drawing.Color.Transparent;
			this.sepName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepName, "sepName");
			this.sepName.Name = "sepName";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtEdge1);
			this.chartPanelControl1.Controls.Add(this.lblEdge1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtEdge1, "txtEdge1");
			this.txtEdge1.EnterMoveNextControl = true;
			this.txtEdge1.Name = "txtEdge1";
			this.txtEdge1.Validating += new System.ComponentModel.CancelEventHandler(this.txtEdge1_Validating);
			this.txtEdge1.Validated += new System.EventHandler(this.txtEdge1_Validated);
			resources.ApplyResources(this.lblEdge1, "lblEdge1");
			this.lblEdge1.Name = "lblEdge1";
			this.sepEdge.BackColor = System.Drawing.Color.Transparent;
			this.sepEdge.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepEdge, "sepEdge");
			this.sepEdge.Name = "sepEdge";
			resources.ApplyResources(this.pnlEdge2, "pnlEdge2");
			this.pnlEdge2.BackColor = System.Drawing.Color.Transparent;
			this.pnlEdge2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlEdge2.Controls.Add(this.txtEdge2);
			this.pnlEdge2.Controls.Add(this.lblEdge2);
			this.pnlEdge2.Name = "pnlEdge2";
			resources.ApplyResources(this.txtEdge2, "txtEdge2");
			this.txtEdge2.EnterMoveNextControl = true;
			this.txtEdge2.Name = "txtEdge2";
			this.txtEdge2.Validating += new System.ComponentModel.CancelEventHandler(this.txtEdge2_Validating);
			this.txtEdge2.Validated += new System.EventHandler(this.txtEdge2_Validated);
			resources.ApplyResources(this.lblEdge2, "lblEdge2");
			this.lblEdge2.Name = "lblEdge2";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.pnlControls, "pnlControls");
			this.pnlControls.BackColor = System.Drawing.Color.Transparent;
			this.pnlControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControls.Controls.Add(this.pnlEdge2);
			this.pnlControls.Controls.Add(this.sepEdge);
			this.pnlControls.Controls.Add(this.chartPanelControl1);
			this.pnlControls.Controls.Add(this.sepName);
			this.pnlControls.Controls.Add(this.pnlName);
			this.pnlControls.Name = "pnlControls";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.sepVisible);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.chVisible);
			this.Name = "ScaleBreakGeneralControl";
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepVisible)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlName)).EndInit();
			this.pnlName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtEdge1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepEdge)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlEdge2)).EndInit();
			this.pnlEdge2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtEdge2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepVisible;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlName;
		private DevExpress.XtraEditors.TextEdit txtName;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblName;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepName;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.TextEdit txtEdge1;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblEdge1;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepEdge;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlEdge2;
		private DevExpress.XtraEditors.TextEdit txtEdge2;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblEdge2;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private ChartPanelControl pnlControls;
	}
}
