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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class PieDiagramControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PieDiagramControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.groupMargin = new DevExpress.XtraEditors.GroupControl();
			this.marginsControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDirection = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDimension = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDimension = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupMargin)).BeginInit();
			this.groupMargin.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDirection)).BeginInit();
			this.pnlDirection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDimension)).BeginInit();
			this.pnlDimension.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDimension.Properties)).BeginInit();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral});
			this.tbGeneral.Controls.Add(this.groupMargin);
			this.tbGeneral.Controls.Add(this.panelControl3);
			this.tbGeneral.Controls.Add(this.pnlDirection);
			this.tbGeneral.Controls.Add(this.chartPanelControl2);
			this.tbGeneral.Controls.Add(this.pnlDimension);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.groupMargin, "groupMargin");
			this.groupMargin.Controls.Add(this.marginsControl);
			this.groupMargin.Name = "groupMargin";
			resources.ApplyResources(this.marginsControl, "marginsControl");
			this.marginsControl.Name = "marginsControl";
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.pnlDirection, "pnlDirection");
			this.pnlDirection.BackColor = System.Drawing.Color.Transparent;
			this.pnlDirection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDirection.Controls.Add(this.cbDirection);
			this.pnlDirection.Controls.Add(this.chartLabelControl1);
			this.pnlDirection.Name = "pnlDirection";
			resources.ApplyResources(this.cbDirection, "cbDirection");
			this.cbDirection.Name = "cbDirection";
			this.cbDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDirection.Properties.Buttons"))))});
			this.cbDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDirection.Properties.Items"),
			resources.GetString("cbDirection.Properties.Items1")});
			this.cbDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDirection.SelectedIndexChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.pnlDimension, "pnlDimension");
			this.pnlDimension.BackColor = System.Drawing.Color.Transparent;
			this.pnlDimension.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDimension.Controls.Add(this.txtDimension);
			this.pnlDimension.Controls.Add(this.chartLabelControl4);
			this.pnlDimension.Name = "pnlDimension";
			resources.ApplyResources(this.txtDimension, "txtDimension");
			this.txtDimension.Name = "txtDimension";
			this.txtDimension.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDimension.Properties.IsFloatValue = false;
			this.txtDimension.Properties.Mask.EditMask = resources.GetString("txtDimension.Properties.Mask.EditMask");
			this.txtDimension.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtDimension.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtDimension.EditValueChanged += new System.EventHandler(this.txtDimension_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			resources.ApplyResources(this, "$this");
			this.Name = "PieDiagramControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupMargin)).EndInit();
			this.groupMargin.ResumeLayout(false);
			this.groupMargin.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDirection)).EndInit();
			this.pnlDirection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDimension)).EndInit();
			this.pnlDimension.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDimension.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabPage tbGeneral;
		private ChartPanelControl pnlDirection;
		private DevExpress.XtraEditors.ComboBoxEdit cbDirection;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl pnlDimension;
		private DevExpress.XtraEditors.SpinEdit txtDimension;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl panelControl3;
		private DevExpress.XtraEditors.GroupControl groupMargin;
		private RectangleIndentsControl marginsControl;
	}
}
