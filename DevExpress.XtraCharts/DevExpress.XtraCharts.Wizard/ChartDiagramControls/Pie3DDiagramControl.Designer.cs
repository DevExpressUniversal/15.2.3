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
	partial class Pie3DDiagramControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pie3DDiagramControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
			this.txtPerspectiveAngle = new DevExpress.XtraEditors.SpinEdit();
			this.panelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chUsePerspective = new DevExpress.XtraEditors.CheckEdit();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grLayout = new DevExpress.XtraEditors.GroupControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDimension = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.tbRotation = new DevExpress.XtraTab.XtraTabPage();
			this.rotateControl = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.RotateControl();
			this.tbScrollingZooming = new DevExpress.XtraTab.XtraTabPage();
			this.scrollingZooming3DControl = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollingZooming3DControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
			this.groupControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPerspectiveAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).BeginInit();
			this.panelControl8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chUsePerspective.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).BeginInit();
			this.grLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDimension.Properties)).BeginInit();
			this.tbRotation.SuspendLayout();
			this.tbScrollingZooming.SuspendLayout();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbRotation,
			this.tbScrollingZooming});
			this.tbGeneral.Controls.Add(this.groupControl4);
			this.tbGeneral.Controls.Add(this.panelControl3);
			this.tbGeneral.Controls.Add(this.grLayout);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.groupControl4, "groupControl4");
			this.groupControl4.Controls.Add(this.txtPerspectiveAngle);
			this.groupControl4.Controls.Add(this.panelControl8);
			this.groupControl4.Controls.Add(this.panelControl7);
			this.groupControl4.Controls.Add(this.chUsePerspective);
			this.groupControl4.Name = "groupControl4";
			resources.ApplyResources(this.txtPerspectiveAngle, "txtPerspectiveAngle");
			this.txtPerspectiveAngle.Name = "txtPerspectiveAngle";
			this.txtPerspectiveAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtPerspectiveAngle.Properties.IsFloatValue = false;
			this.txtPerspectiveAngle.Properties.Mask.EditMask = resources.GetString("txtPerspectiveAngle.Properties.Mask.EditMask");
			this.txtPerspectiveAngle.Properties.MaxValue = new decimal(new int[] {
			179,
			0,
			0,
			0});
			this.txtPerspectiveAngle.EditValueChanged += new System.EventHandler(this.txtPerspectiveAngle_EditValueChanged);
			this.panelControl8.BackColor = System.Drawing.Color.Transparent;
			this.panelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl8.Controls.Add(this.chartLabelControl2);
			resources.ApplyResources(this.panelControl8, "panelControl8");
			this.panelControl8.Name = "panelControl8";
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.panelControl7.BackColor = System.Drawing.Color.Transparent;
			this.panelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl7, "panelControl7");
			this.panelControl7.Name = "panelControl7";
			resources.ApplyResources(this.chUsePerspective, "chUsePerspective");
			this.chUsePerspective.Name = "chUsePerspective";
			this.chUsePerspective.Properties.Caption = resources.GetString("chUsePerspective.Properties.Caption");
			this.chUsePerspective.CheckedChanged += new System.EventHandler(this.chUsePerspective_CheckedChanged);
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.grLayout, "grLayout");
			this.grLayout.Controls.Add(this.chartPanelControl1);
			this.grLayout.Controls.Add(this.panelControl2);
			this.grLayout.Controls.Add(this.chartPanelControl4);
			this.grLayout.Name = "grLayout";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.cbDirection);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.cbDirection, "cbDirection");
			this.cbDirection.Name = "cbDirection";
			this.cbDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDirection.Properties.Buttons"))))});
			this.cbDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDirection.Properties.Items"),
			resources.GetString("cbDirection.Properties.Items1")});
			this.cbDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDirection.EditValueChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.txtDimension);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl4.Name = "chartPanelControl4";
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
			this.tbRotation.Controls.Add(this.rotateControl);
			this.tbRotation.Name = "tbRotation";
			resources.ApplyResources(this.tbRotation, "tbRotation");
			resources.ApplyResources(this.rotateControl, "rotateControl");
			this.rotateControl.Name = "rotateControl";
			this.tbScrollingZooming.Controls.Add(this.scrollingZooming3DControl);
			this.tbScrollingZooming.Name = "tbScrollingZooming";
			resources.ApplyResources(this.tbScrollingZooming, "tbScrollingZooming");
			resources.ApplyResources(this.scrollingZooming3DControl, "scrollingZooming3DControl");
			this.scrollingZooming3DControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			this.scrollingZooming3DControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.scrollingZooming3DControl.Name = "scrollingZooming3DControl";
			resources.ApplyResources(this, "$this");
			this.Name = "Pie3DDiagramControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
			this.groupControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtPerspectiveAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl8)).EndInit();
			this.panelControl8.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chUsePerspective.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grLayout)).EndInit();
			this.grLayout.ResumeLayout(false);
			this.grLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDimension.Properties)).EndInit();
			this.tbRotation.ResumeLayout(false);
			this.tbRotation.PerformLayout();
			this.tbScrollingZooming.ResumeLayout(false);
			this.tbScrollingZooming.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabPage tbGeneral;
		protected DevExpress.XtraTab.XtraTabPage tbRotation;
		protected DevExpress.XtraTab.XtraTabPage tbScrollingZooming;
		private DevExpress.XtraEditors.GroupControl groupControl4;
		private DevExpress.XtraEditors.SpinEdit txtPerspectiveAngle;
		private ChartPanelControl panelControl8;
		private ChartPanelControl panelControl7;
		private DevExpress.XtraEditors.CheckEdit chUsePerspective;
		private RotateControl rotateControl;
		private ChartPanelControl panelControl3;
		private DevExpress.XtraEditors.GroupControl grLayout;
		private DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollingZooming3DControl scrollingZooming3DControl;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.ComboBoxEdit cbDirection;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl panelControl2;
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.SpinEdit txtDimension;
		private ChartLabelControl chartLabelControl4;
	}
}
