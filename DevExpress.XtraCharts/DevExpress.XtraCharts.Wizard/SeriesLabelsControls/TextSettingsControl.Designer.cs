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
	partial class TextSettingsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextSettingsControl));
			this.grTextSettings = new DevExpress.XtraEditors.GroupControl();
			this.pnlMaxLineCount = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speMaxLineCount = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxLineCount = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speMaxWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.peFont = new DevExpress.XtraEditors.PopupContainerEdit();
			this.fontEditControlContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.fontEditControl = new DevExpress.XtraCharts.Design.FontEditControl();
			this.lblFont = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceTextColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepAntialiasing = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAntialize = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grTextSettings)).BeginInit();
			this.grTextSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxLineCount)).BeginInit();
			this.pnlMaxLineCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speMaxLineCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxWidth)).BeginInit();
			this.pnlMaxWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speMaxWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).BeginInit();
			this.pnlAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			this.chartPanelControl6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.peFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).BeginInit();
			this.fontEditControlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAntialiasing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grTextSettings, "grTextSettings");
			this.grTextSettings.Controls.Add(this.pnlMaxLineCount);
			this.grTextSettings.Controls.Add(this.chartPanelControl4);
			this.grTextSettings.Controls.Add(this.pnlMaxWidth);
			this.grTextSettings.Controls.Add(this.chartPanelControl1);
			this.grTextSettings.Controls.Add(this.pnlAlignment);
			this.grTextSettings.Controls.Add(this.chartPanelControl5);
			this.grTextSettings.Controls.Add(this.chartPanelControl6);
			this.grTextSettings.Controls.Add(this.sepColor);
			this.grTextSettings.Controls.Add(this.chartPanelControl3);
			this.grTextSettings.Controls.Add(this.sepAntialiasing);
			this.grTextSettings.Controls.Add(this.chAntialize);
			this.grTextSettings.Name = "grTextSettings";
			resources.ApplyResources(this.pnlMaxLineCount, "pnlMaxLineCount");
			this.pnlMaxLineCount.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxLineCount.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxLineCount.Controls.Add(this.speMaxLineCount);
			this.pnlMaxLineCount.Controls.Add(this.lblMaxLineCount);
			this.pnlMaxLineCount.Name = "pnlMaxLineCount";
			resources.ApplyResources(this.speMaxLineCount, "speMaxLineCount");
			this.speMaxLineCount.Name = "speMaxLineCount";
			this.speMaxLineCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.speMaxLineCount.Properties.IsFloatValue = false;
			this.speMaxLineCount.Properties.Mask.EditMask = resources.GetString("speMaxLineCount.Properties.Mask.EditMask");
			this.speMaxLineCount.Properties.MaxValue = new decimal(new int[] {
			20,
			0,
			0,
			0});
			this.speMaxLineCount.EditValueChanged += new System.EventHandler(this.speMaxLineCount_EditValueChanged);
			resources.ApplyResources(this.lblMaxLineCount, "lblMaxLineCount");
			this.lblMaxLineCount.Name = "lblMaxLineCount";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.pnlMaxWidth, "pnlMaxWidth");
			this.pnlMaxWidth.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxWidth.Controls.Add(this.speMaxWidth);
			this.pnlMaxWidth.Controls.Add(this.lblMaxWidth);
			this.pnlMaxWidth.Name = "pnlMaxWidth";
			resources.ApplyResources(this.speMaxWidth, "speMaxWidth");
			this.speMaxWidth.Name = "speMaxWidth";
			this.speMaxWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.speMaxWidth.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.speMaxWidth.Properties.IsFloatValue = false;
			this.speMaxWidth.Properties.Mask.EditMask = resources.GetString("speMaxWidth.Properties.Mask.EditMask");
			this.speMaxWidth.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.speMaxWidth.EditValueChanged += new System.EventHandler(this.speMaxWidth_EditValueChanged);
			resources.ApplyResources(this.lblMaxWidth, "lblMaxWidth");
			this.lblMaxWidth.Name = "lblMaxWidth";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.pnlAlignment, "pnlAlignment");
			this.pnlAlignment.BackColor = System.Drawing.Color.Transparent;
			this.pnlAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAlignment.Controls.Add(this.cbAlignment);
			this.pnlAlignment.Controls.Add(this.lblAlignment);
			this.pnlAlignment.Name = "pnlAlignment";
			resources.ApplyResources(this.cbAlignment, "cbAlignment");
			this.cbAlignment.Name = "cbAlignment";
			this.cbAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAlignment.Properties.Buttons"))))});
			this.cbAlignment.Properties.Items.AddRange(new object[] {
			resources.GetString("cbAlignment.Properties.Items"),
			resources.GetString("cbAlignment.Properties.Items1"),
			resources.GetString("cbAlignment.Properties.Items2")});
			this.cbAlignment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAlignment.SelectedIndexChanged += new System.EventHandler(this.cbAlignment_SelectedIndexChanged);
			resources.ApplyResources(this.lblAlignment, "lblAlignment");
			this.lblAlignment.Name = "lblAlignment";
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Controls.Add(this.peFont);
			this.chartPanelControl6.Controls.Add(this.lblFont);
			this.chartPanelControl6.Name = "chartPanelControl6";
			resources.ApplyResources(this.peFont, "peFont");
			this.peFont.Name = "peFont";
			this.peFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("peFont.Properties.Buttons"))))});
			this.peFont.Properties.PopupControl = this.fontEditControlContainer;
			this.peFont.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.peFont_QueryResultValue);
			this.peFont.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.peFont_QueryPopUp);
			this.fontEditControlContainer.Controls.Add(this.fontEditControl);
			resources.ApplyResources(this.fontEditControlContainer, "fontEditControlContainer");
			this.fontEditControlContainer.Name = "fontEditControlContainer";
			resources.ApplyResources(this.fontEditControl, "fontEditControl");
			this.fontEditControl.EditedFont = null;
			this.fontEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.fontEditControl.Name = "fontEditControl";
			this.fontEditControl.OnNeedClose += new System.EventHandler(this.fontEditControl_OnNeedClose);
			resources.ApplyResources(this.lblFont, "lblFont");
			this.lblFont.Name = "lblFont";
			this.sepColor.BackColor = System.Drawing.Color.Transparent;
			this.sepColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepColor, "sepColor");
			this.sepColor.Name = "sepColor";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.ceTextColor);
			this.chartPanelControl3.Controls.Add(this.lblColor);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.ceTextColor, "ceTextColor");
			this.ceTextColor.Name = "ceTextColor";
			this.ceTextColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceTextColor.Properties.Buttons"))))});
			this.ceTextColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceTextColor.EditValueChanged += new System.EventHandler(this.ceTextColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.sepAntialiasing.BackColor = System.Drawing.Color.Transparent;
			this.sepAntialiasing.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAntialiasing, "sepAntialiasing");
			this.sepAntialiasing.Name = "sepAntialiasing";
			resources.ApplyResources(this.chAntialize, "chAntialize");
			this.chAntialize.Name = "chAntialize";
			this.chAntialize.Properties.AllowGrayed = true;
			this.chAntialize.Properties.Caption = resources.GetString("chAntialize.Properties.Caption");
			this.chAntialize.CheckStateChanged += new System.EventHandler(this.chAntialize_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grTextSettings);
			this.Controls.Add(this.fontEditControlContainer);
			this.Name = "TextSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.grTextSettings)).EndInit();
			this.grTextSettings.ResumeLayout(false);
			this.grTextSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxLineCount)).EndInit();
			this.pnlMaxLineCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speMaxLineCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxWidth)).EndInit();
			this.pnlMaxWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speMaxWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).EndInit();
			this.pnlAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			this.chartPanelControl6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.peFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).EndInit();
			this.fontEditControlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAntialiasing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grTextSettings;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl6;
		private DevExpress.XtraEditors.PopupContainerEdit peFont;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblFont;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepColor;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl3;
		private DevExpress.XtraEditors.ColorEdit ceTextColor;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblColor;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepAntialiasing;
		private DevExpress.XtraEditors.CheckEdit chAntialize;
		private DevExpress.XtraEditors.PopupContainerControl fontEditControlContainer;
		private DevExpress.XtraCharts.Design.FontEditControl fontEditControl;
		private ChartPanelControl pnlMaxWidth;
		private DevExpress.XtraEditors.SpinEdit speMaxWidth;
		protected ChartLabelControl lblMaxWidth;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl pnlMaxLineCount;
		private DevExpress.XtraEditors.SpinEdit speMaxLineCount;
		protected ChartLabelControl lblMaxLineCount;
		private ChartPanelControl chartPanelControl4;
		private ChartPanelControl chartPanelControl5;
		private ChartPanelControl pnlAlignment;
		private DevExpress.XtraEditors.ComboBoxEdit cbAlignment;
		protected ChartLabelControl lblAlignment;
	}
}
