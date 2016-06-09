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
	partial class ConstantLineTitleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConstantLineTitleControl));
			this.fontEditControlContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.fontEditControl = new DevExpress.XtraCharts.Design.FontEditControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.chShowBelow = new DevExpress.XtraEditors.CheckEdit();
			this.pnlTextSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtFont = new DevExpress.XtraEditors.PopupContainerEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceTextColor = new DevExpress.XtraEditors.ColorEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtText = new DevExpress.XtraEditors.TextEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAntialize = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).BeginInit();
			this.fontEditControlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowBelow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTextSettings)).BeginInit();
			this.pnlTextSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			this.chartPanelControl7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).BeginInit();
			this.SuspendLayout();
			this.fontEditControlContainer.Controls.Add(this.fontEditControl);
			resources.ApplyResources(this.fontEditControlContainer, "fontEditControlContainer");
			this.fontEditControlContainer.Name = "fontEditControlContainer";
			resources.ApplyResources(this.fontEditControl, "fontEditControl");
			this.fontEditControl.EditedFont = null;
			this.fontEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.fontEditControl.Name = "fontEditControl";
			this.fontEditControl.OnNeedClose += new System.EventHandler(this.fontEditControl_OnNeedClose);
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			resources.ApplyResources(this.chShowBelow, "chShowBelow");
			this.chShowBelow.Name = "chShowBelow";
			this.chShowBelow.Properties.Caption = resources.GetString("chShowBelow.Properties.Caption");
			this.chShowBelow.CheckedChanged += new System.EventHandler(this.chShowBelow_CheckedChanged);
			resources.ApplyResources(this.pnlTextSettings, "pnlTextSettings");
			this.pnlTextSettings.BackColor = System.Drawing.Color.Transparent;
			this.pnlTextSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTextSettings.Controls.Add(this.chartPanelControl7);
			this.pnlTextSettings.Controls.Add(this.chartPanelControl4);
			this.pnlTextSettings.Controls.Add(this.chartPanelControl5);
			this.pnlTextSettings.Controls.Add(this.chartPanelControl2);
			this.pnlTextSettings.Controls.Add(this.chartPanelControl3);
			this.pnlTextSettings.Controls.Add(this.panelControl6);
			this.pnlTextSettings.Controls.Add(this.chartPanelControl1);
			this.pnlTextSettings.Controls.Add(this.panelControl1);
			this.pnlTextSettings.Controls.Add(this.chAntialize);
			this.pnlTextSettings.Name = "pnlTextSettings";
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl7.Controls.Add(this.txtFont);
			this.chartPanelControl7.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.txtFont, "txtFont");
			this.txtFont.Name = "txtFont";
			this.txtFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("txtFont.Properties.Buttons"))))});
			this.txtFont.Properties.PopupControl = this.fontEditControlContainer;
			this.txtFont.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.txtFont_QueryResultValue);
			this.txtFont.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.txtFont_QueryPopUp);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.ceTextColor);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.ceTextColor, "ceTextColor");
			this.ceTextColor.Name = "ceTextColor";
			this.ceTextColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceTextColor.Properties.Buttons"))))});
			this.ceTextColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceTextColor.EditValueChanged += new System.EventHandler(this.ceTextColor_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.cbAlignment);
			this.chartPanelControl3.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.cbAlignment, "cbAlignment");
			this.cbAlignment.Name = "cbAlignment";
			this.cbAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAlignment.Properties.Buttons"))))});
			this.cbAlignment.Properties.Items.AddRange(new object[] {
			resources.GetString("cbAlignment.Properties.Items"),
			resources.GetString("cbAlignment.Properties.Items1")});
			this.cbAlignment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAlignment.SelectedIndexChanged += new System.EventHandler(this.cbAlignment_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl6.BackColor = System.Drawing.Color.Transparent;
			this.panelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl6, "panelControl6");
			this.panelControl6.Name = "panelControl6";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtText);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtText, "txtText");
			this.txtText.Name = "txtText";
			this.txtText.EditValueChanged += new System.EventHandler(this.txtText_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.chAntialize, "chAntialize");
			this.chAntialize.Name = "chAntialize";
			this.chAntialize.Properties.AllowGrayed = true;
			this.chAntialize.Properties.Caption = resources.GetString("chAntialize.Properties.Caption");
			this.chAntialize.CheckStateChanged += new System.EventHandler(this.chAntialize_CheckedChanged);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlTextSettings);
			this.Controls.Add(this.fontEditControlContainer);
			this.Controls.Add(this.chShowBelow);
			this.Controls.Add(this.panelControl3);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.chVisible);
			this.Name = "ConstantLineTitleControl";
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).EndInit();
			this.fontEditControlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowBelow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTextSettings)).EndInit();
			this.pnlTextSettings.ResumeLayout(false);
			this.pnlTextSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			this.chartPanelControl7.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.PopupContainerControl fontEditControlContainer;
		private DevExpress.XtraCharts.Design.FontEditControl fontEditControl;
		private ChartPanelControl panelControl3;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private DevExpress.XtraEditors.CheckEdit chShowBelow;
		private ChartPanelControl pnlTextSettings;
		private ChartPanelControl chartPanelControl7;
		private DevExpress.XtraEditors.PopupContainerEdit txtFont;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl4;
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraEditors.ColorEdit ceTextColor;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl chartPanelControl3;
		private DevExpress.XtraEditors.ComboBoxEdit cbAlignment;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl panelControl6;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.TextEdit txtText;
		private ChartLabelControl chartLabelControl3;
		private ChartPanelControl panelControl1;
		private DevExpress.XtraEditors.CheckEdit chAntialize;
		private DevExpress.XtraEditors.LabelControl labelControl2;
	}
}
