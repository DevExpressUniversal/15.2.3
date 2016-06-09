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
	partial class RadarDiagramControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarDiagramControl));
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.marginsControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.drawingStyle = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.RadarDiagramDrawingStyleControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceBorderColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbShadow.SuspendLayout();
			this.tbBorder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			this.SuspendLayout();
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance,
			this.tbBorder,
			this.tbShadow});
			this.tbGeneral.Controls.Add(this.groupControl3);
			this.tbGeneral.Controls.Add(this.panelControl3);
			this.tbGeneral.Controls.Add(this.groupControl1);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.groupControl3, "groupControl3");
			this.groupControl3.Controls.Add(this.marginsControl);
			this.groupControl3.Name = "groupControl3";
			resources.ApplyResources(this.marginsControl, "marginsControl");
			this.marginsControl.Name = "marginsControl";
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Controls.Add(this.drawingStyle);
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.drawingStyle, "drawingStyle");
			this.drawingStyle.Name = "drawingStyle";
			this.tbAppearance.Controls.Add(this.backgroundControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			this.shadowControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.shadowControl.Name = "shadowControl";
			this.tbBorder.Controls.Add(this.panelControl4);
			this.tbBorder.Controls.Add(this.panelControl1);
			this.tbBorder.Controls.Add(this.panelControl2);
			this.tbBorder.Controls.Add(this.labelControl2);
			this.tbBorder.Controls.Add(this.chVisible);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.ceBorderColor);
			this.panelControl1.Controls.Add(this.lblColor);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.ceBorderColor, "ceBorderColor");
			this.ceBorderColor.Name = "ceBorderColor";
			this.ceBorderColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceBorderColor.Properties.Buttons"))))});
			this.ceBorderColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceBorderColor.EditValueChanged += new System.EventHandler(this.ceBorderColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Name = "RadarDiagramControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			this.groupControl3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private DevExpress.XtraEditors.GroupControl groupControl3;
		private RectangleIndentsControl marginsControl;
		private ChartPanelControl panelControl3;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private RadarDiagramDrawingStyleControl drawingStyle;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl shadowControl;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private ChartPanelControl panelControl4;
		private ChartPanelControl panelControl1;
		private DevExpress.XtraEditors.ColorEdit ceBorderColor;
		private ChartLabelControl lblColor;
		private ChartPanelControl panelControl2;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private BackgroundControl backgroundControl;
		private DevExpress.XtraEditors.LabelControl labelControl2;
	}
}
