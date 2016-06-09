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
	partial class XYDiagramControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XYDiagramControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPaneLayoutDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPaneLayoutDirection = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlThickness = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtPaneDistance = new DevExpress.XtraEditors.SpinEdit();
			this.lblPaneDistance = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.marginsControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.sepRotated = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chRotated = new DevExpress.XtraEditors.CheckEdit();
			this.tbElements = new DevExpress.XtraTab.XtraTabPage();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.panesRedact = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.PanesRedact();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupControl5 = new DevExpress.XtraEditors.GroupControl();
			this.secondaryAxesY = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.SecondaryAxesRedact();
			this.panelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
			this.secondaryAxesX = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.SecondaryAxesRedact();
			this.tbScrollingZooming = new DevExpress.XtraTab.XtraTabPage();
			this.scrollingZooming2DControl = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollingZooming2DControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPaneLayoutDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPaneDistance.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepRotated)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chRotated.Properties)).BeginInit();
			this.tbElements.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl5)).BeginInit();
			this.groupControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
			this.groupControl4.SuspendLayout();
			this.tbScrollingZooming.SuspendLayout();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbElements,
			this.tbScrollingZooming});
			this.tbGeneral.Controls.Add(this.groupControl1);
			this.tbGeneral.Controls.Add(this.panelControl6);
			this.tbGeneral.Controls.Add(this.groupControl3);
			this.tbGeneral.Controls.Add(this.sepRotated);
			this.tbGeneral.Controls.Add(this.chRotated);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Controls.Add(this.chartPanelControl4);
			this.groupControl1.Controls.Add(this.pnlThickness);
			this.groupControl1.Controls.Add(this.chartPanelControl1);
			this.groupControl1.Name = "groupControl1";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.cbPaneLayoutDirection);
			this.chartPanelControl4.Controls.Add(this.lblPaneLayoutDirection);
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.cbPaneLayoutDirection, "cbPaneLayoutDirection");
			this.cbPaneLayoutDirection.Name = "cbPaneLayoutDirection";
			this.cbPaneLayoutDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaneLayoutDirection.Properties.Buttons"))))});
			this.cbPaneLayoutDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPaneLayoutDirection.Properties.Items"),
			resources.GetString("cbPaneLayoutDirection.Properties.Items1")});
			this.cbPaneLayoutDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPaneLayoutDirection.SelectedIndexChanged += new System.EventHandler(this.cbPaneLayoutDirection_SelectedIndexChanged);
			resources.ApplyResources(this.lblPaneLayoutDirection, "lblPaneLayoutDirection");
			this.lblPaneLayoutDirection.Name = "lblPaneLayoutDirection";
			this.pnlThickness.BackColor = System.Drawing.Color.Transparent;
			this.pnlThickness.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlThickness, "pnlThickness");
			this.pnlThickness.Name = "pnlThickness";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtPaneDistance);
			this.chartPanelControl1.Controls.Add(this.lblPaneDistance);
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtPaneDistance, "txtPaneDistance");
			this.txtPaneDistance.Name = "txtPaneDistance";
			this.txtPaneDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtPaneDistance.Properties.IsFloatValue = false;
			this.txtPaneDistance.Properties.Mask.EditMask = resources.GetString("txtPaneDistance.Properties.Mask.EditMask");
			this.txtPaneDistance.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtPaneDistance.Properties.ValidateOnEnterKey = true;
			this.txtPaneDistance.ValueChanged += new System.EventHandler(this.txtPaneDistance_ValueChanged);
			resources.ApplyResources(this.lblPaneDistance, "lblPaneDistance");
			this.lblPaneDistance.Name = "lblPaneDistance";
			this.panelControl6.BackColor = System.Drawing.Color.Transparent;
			this.panelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl6, "panelControl6");
			this.panelControl6.Name = "panelControl6";
			resources.ApplyResources(this.groupControl3, "groupControl3");
			this.groupControl3.Controls.Add(this.marginsControl);
			this.groupControl3.Name = "groupControl3";
			resources.ApplyResources(this.marginsControl, "marginsControl");
			this.marginsControl.Name = "marginsControl";
			this.sepRotated.BackColor = System.Drawing.Color.Transparent;
			this.sepRotated.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepRotated, "sepRotated");
			this.sepRotated.Name = "sepRotated";
			resources.ApplyResources(this.chRotated, "chRotated");
			this.chRotated.Name = "chRotated";
			this.chRotated.Properties.Caption = resources.GetString("chRotated.Properties.Caption");
			this.chRotated.CheckedChanged += new System.EventHandler(this.chReverse_CheckedChanged);
			this.tbElements.Controls.Add(this.groupControl2);
			this.tbElements.Controls.Add(this.chartPanelControl5);
			this.tbElements.Controls.Add(this.groupControl5);
			this.tbElements.Controls.Add(this.panelControl7);
			this.tbElements.Controls.Add(this.groupControl4);
			this.tbElements.Name = "tbElements";
			resources.ApplyResources(this.tbElements, "tbElements");
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Controls.Add(this.panesRedact);
			this.groupControl2.Name = "groupControl2";
			resources.ApplyResources(this.panesRedact, "panesRedact");
			this.panesRedact.Name = "panesRedact";
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.groupControl5, "groupControl5");
			this.groupControl5.Controls.Add(this.secondaryAxesY);
			this.groupControl5.Name = "groupControl5";
			resources.ApplyResources(this.secondaryAxesY, "secondaryAxesY");
			this.secondaryAxesY.Name = "secondaryAxesY";
			this.panelControl7.BackColor = System.Drawing.Color.Transparent;
			this.panelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl7, "panelControl7");
			this.panelControl7.Name = "panelControl7";
			resources.ApplyResources(this.groupControl4, "groupControl4");
			this.groupControl4.Controls.Add(this.secondaryAxesX);
			this.groupControl4.Name = "groupControl4";
			resources.ApplyResources(this.secondaryAxesX, "secondaryAxesX");
			this.secondaryAxesX.Name = "secondaryAxesX";
			this.tbScrollingZooming.Controls.Add(this.scrollingZooming2DControl);
			this.tbScrollingZooming.Name = "tbScrollingZooming";
			resources.ApplyResources(this.tbScrollingZooming, "tbScrollingZooming");
			resources.ApplyResources(this.scrollingZooming2DControl, "scrollingZooming2DControl");
			this.scrollingZooming2DControl.Name = "scrollingZooming2DControl";
			resources.ApplyResources(this, "$this");
			this.Name = "XYDiagramControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPaneLayoutDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtPaneDistance.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			this.groupControl3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepRotated)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chRotated.Properties)).EndInit();
			this.tbElements.ResumeLayout(false);
			this.tbElements.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.groupControl2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl5)).EndInit();
			this.groupControl5.ResumeLayout(false);
			this.groupControl5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
			this.groupControl4.ResumeLayout(false);
			this.groupControl4.PerformLayout();
			this.tbScrollingZooming.ResumeLayout(false);
			this.tbScrollingZooming.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraEditors.CheckEdit chRotated;
		private DevExpress.XtraEditors.GroupControl groupControl3;
		private ChartPanelControl sepRotated;
		private RectangleIndentsControl marginsControl;
		private ChartPanelControl panelControl6;
		private DevExpress.XtraTab.XtraTabPage tbElements;
		private DevExpress.XtraEditors.GroupControl groupControl5;
		private SecondaryAxesRedact secondaryAxesY;
		private ChartPanelControl panelControl7;
		private DevExpress.XtraEditors.GroupControl groupControl4;
		private SecondaryAxesRedact secondaryAxesX;
		private DevExpress.XtraEditors.SpinEdit txtPaneDistance;
		private ChartLabelControl lblPaneDistance;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl4;
		private ChartPanelControl pnlThickness;
		protected DevExpress.XtraEditors.ComboBoxEdit cbPaneLayoutDirection;
		private ChartLabelControl lblPaneLayoutDirection;
		private PanesRedact panesRedact;
		private DevExpress.XtraEditors.GroupControl groupControl2;
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraTab.XtraTabPage tbScrollingZooming;
		private ScrollingZooming2DControl scrollingZooming2DControl;
	}
}
