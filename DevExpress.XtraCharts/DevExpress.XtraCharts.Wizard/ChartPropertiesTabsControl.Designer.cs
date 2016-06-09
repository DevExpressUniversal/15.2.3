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

namespace DevExpress.XtraCharts.Wizard {
	partial class ChartPropertiesTabsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartPropertiesTabsControl));
			this.tbcPages = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtThickness = new DevExpress.XtraEditors.SpinEdit();
			this.lblThickness = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceBorderColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.tbPadding = new DevExpress.XtraTab.XtraTabPage();
			this.paddingControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.tbcEmptyChartText = new DevExpress.XtraTab.XtraTabPage();
			this.emptyChartTextControl = new DevExpress.XtraCharts.Wizard.NotificationControl();
			this.tbcSmallChartText = new DevExpress.XtraTab.XtraTabPage();
			this.smallChartTextControl = new DevExpress.XtraCharts.Wizard.NotificationControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAutoLayout = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbcPages)).BeginInit();
			this.tbcPages.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbBorder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			this.chartPanelControl6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			this.tbPadding.SuspendLayout();
			this.tbcEmptyChartText.SuspendLayout();
			this.tbcSmallChartText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAutoLayout.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcPages, "tbcPages");
			this.tbcPages.Name = "tbcPages";
			this.tbcPages.SelectedTabPage = this.tbGeneral;
			this.tbcPages.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbBorder,
			this.tbPadding,
			this.tbcEmptyChartText,
			this.tbcSmallChartText});
			this.tbGeneral.Controls.Add(this.chartPanelControl2);
			this.tbGeneral.Controls.Add(this.chAutoLayout);
			this.tbGeneral.Controls.Add(this.backgroundControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.tbBorder.Controls.Add(this.chartPanelControl1);
			this.tbBorder.Controls.Add(this.chartPanelControl3);
			this.tbBorder.Controls.Add(this.chartPanelControl6);
			this.tbBorder.Controls.Add(this.chartPanelControl4);
			this.tbBorder.Controls.Add(this.labelControl1);
			this.tbBorder.Controls.Add(this.chVisible);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtThickness);
			this.chartPanelControl1.Controls.Add(this.lblThickness);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtThickness, "txtThickness");
			this.txtThickness.Name = "txtThickness";
			this.txtThickness.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtThickness.Properties.IsFloatValue = false;
			this.txtThickness.Properties.Mask.EditMask = resources.GetString("txtThickness.Properties.Mask.EditMask");
			this.txtThickness.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtThickness.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtThickness.EditValueChanged += new System.EventHandler(this.txtThickness_EditValueChanged);
			resources.ApplyResources(this.lblThickness, "lblThickness");
			this.lblThickness.Name = "lblThickness";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Controls.Add(this.ceBorderColor);
			this.chartPanelControl6.Controls.Add(this.lblColor);
			this.chartPanelControl6.Name = "chartPanelControl6";
			resources.ApplyResources(this.ceBorderColor, "ceBorderColor");
			this.ceBorderColor.Name = "ceBorderColor";
			this.ceBorderColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceBorderColor.Properties.Buttons"))))});
			this.ceBorderColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceBorderColor.EditValueChanged += new System.EventHandler(this.ceBorderColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.AllowGrayed = true;
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckStateChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.tbPadding.Controls.Add(this.paddingControl);
			this.tbPadding.Name = "tbPadding";
			resources.ApplyResources(this.tbPadding, "tbPadding");
			resources.ApplyResources(this.paddingControl, "paddingControl");
			this.paddingControl.Name = "paddingControl";
			this.tbcEmptyChartText.Controls.Add(this.emptyChartTextControl);
			this.tbcEmptyChartText.Name = "tbcEmptyChartText";
			resources.ApplyResources(this.tbcEmptyChartText, "tbcEmptyChartText");
			resources.ApplyResources(this.emptyChartTextControl, "emptyChartTextControl");
			this.emptyChartTextControl.CausesValidation = false;
			this.emptyChartTextControl.Name = "emptyChartTextControl";
			this.tbcSmallChartText.Controls.Add(this.smallChartTextControl);
			this.tbcSmallChartText.Name = "tbcSmallChartText";
			resources.ApplyResources(this.tbcSmallChartText, "tbcSmallChartText");
			resources.ApplyResources(this.smallChartTextControl, "smallChartTextControl");
			this.smallChartTextControl.CausesValidation = false;
			this.smallChartTextControl.Name = "smallChartTextControl";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chAutoLayout, "chAutoLayout");
			this.chAutoLayout.Name = "chAutoLayout";
			this.chAutoLayout.Properties.Caption = resources.GetString("chAutoLayout.Properties.Caption");
			this.chAutoLayout.CheckedChanged += new System.EventHandler(this.chAutoLayout_CheckedChanged);
			this.Controls.Add(this.tbcPages);
			resources.ApplyResources(this, "$this");
			this.Name = "ChartPropertiesTabsControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPages)).EndInit();
			this.tbcPages.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			this.chartPanelControl6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			this.tbPadding.ResumeLayout(false);
			this.tbPadding.PerformLayout();
			this.tbcEmptyChartText.ResumeLayout(false);
			this.tbcEmptyChartText.PerformLayout();
			this.tbcSmallChartText.ResumeLayout(false);
			this.tbcSmallChartText.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAutoLayout.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcPages;
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.SpinEdit txtThickness;
		private ChartLabelControl lblThickness;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl6;
		private DevExpress.XtraEditors.ColorEdit ceBorderColor;
		private ChartLabelControl lblColor;
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private BackgroundControl backgroundControl;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraTab.XtraTabPage tbPadding;
		private RectangleIndentsControl paddingControl;
		private DevExpress.XtraTab.XtraTabPage tbcEmptyChartText;
		private DevExpress.XtraTab.XtraTabPage tbcSmallChartText;
		private NotificationControl emptyChartTextControl;
		private NotificationControl smallChartTextControl;
		private ChartPanelControl chartPanelControl2;
		private XtraEditors.CheckEdit chAutoLayout;
	}
}
