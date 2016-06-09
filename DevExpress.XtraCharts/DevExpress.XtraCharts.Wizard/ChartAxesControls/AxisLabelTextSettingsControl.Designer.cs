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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls
{
	partial class AxisLabelTextSettingsControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisLabelTextSettingsControl));
			this.panelFont = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pceFont = new DevExpress.XtraEditors.PopupContainerEdit();
			this.fontEditControlContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.fontEditControl = new DevExpress.XtraCharts.Design.FontEditControl();
			this.labelFont = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelFontPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.labelColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelColorPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceAntialiasing = new DevExpress.XtraEditors.CheckEdit();
			this.pnlMaxWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speMaxWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxLineCount = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speMaxLineCount = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxLineCount = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelFont)).BeginInit();
			this.panelFont.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pceFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).BeginInit();
			this.fontEditControlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelFontPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).BeginInit();
			this.panelColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColorPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAntialiasing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxWidth)).BeginInit();
			this.pnlMaxWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speMaxWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxLineCount)).BeginInit();
			this.pnlMaxLineCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speMaxLineCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).BeginInit();
			this.pnlAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelFont, "panelFont");
			this.panelFont.BackColor = System.Drawing.Color.Transparent;
			this.panelFont.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelFont.Controls.Add(this.pceFont);
			this.panelFont.Controls.Add(this.labelFont);
			this.panelFont.Name = "panelFont";
			resources.ApplyResources(this.pceFont, "pceFont");
			this.pceFont.Name = "pceFont";
			this.pceFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("pceFont.Properties.Buttons"))))});
			this.pceFont.Properties.PopupControl = this.fontEditControlContainer;
			this.pceFont.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.pceFont_QueryResultValue);
			this.pceFont.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.pceFont_QueryPopUp);
			this.fontEditControlContainer.Controls.Add(this.fontEditControl);
			resources.ApplyResources(this.fontEditControlContainer, "fontEditControlContainer");
			this.fontEditControlContainer.Name = "fontEditControlContainer";
			resources.ApplyResources(this.fontEditControl, "fontEditControl");
			this.fontEditControl.EditedFont = null;
			this.fontEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.fontEditControl.Name = "fontEditControl";
			this.fontEditControl.OnNeedClose += new System.EventHandler(this.fontEditControl_OnNeedClose);
			resources.ApplyResources(this.labelFont, "labelFont");
			this.labelFont.Name = "labelFont";
			this.panelFontPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelFontPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelFontPadding, "panelFontPadding");
			this.panelFontPadding.Name = "panelFontPadding";
			resources.ApplyResources(this.panelColor, "panelColor");
			this.panelColor.BackColor = System.Drawing.Color.Transparent;
			this.panelColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelColor.Controls.Add(this.ceColor);
			this.panelColor.Controls.Add(this.labelColor);
			this.panelColor.Name = "panelColor";
			resources.ApplyResources(this.ceColor, "ceColor");
			this.ceColor.Name = "ceColor";
			this.ceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceColor.Properties.Buttons"))))});
			this.ceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceColor.EditValueChanged += new System.EventHandler(this.ceColor_EditValueChanged);
			resources.ApplyResources(this.labelColor, "labelColor");
			this.labelColor.Name = "labelColor";
			this.panelColorPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelColorPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelColorPadding, "panelColorPadding");
			this.panelColorPadding.Name = "panelColorPadding";
			resources.ApplyResources(this.ceAntialiasing, "ceAntialiasing");
			this.ceAntialiasing.Name = "ceAntialiasing";
			this.ceAntialiasing.Properties.AllowGrayed = true;
			this.ceAntialiasing.Properties.Caption = resources.GetString("ceAntialiasing.Properties.Caption");
			this.ceAntialiasing.CheckStateChanged += new System.EventHandler(this.ceAntialiasing_CheckStateChanged);
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
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlMaxLineCount);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.pnlMaxWidth);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.pnlAlignment);
			this.Controls.Add(this.chartPanelControl5);
			this.Controls.Add(this.fontEditControlContainer);
			this.Controls.Add(this.panelFont);
			this.Controls.Add(this.panelFontPadding);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.panelColorPadding);
			this.Controls.Add(this.ceAntialiasing);
			this.Name = "AxisLabelTextSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelFont)).EndInit();
			this.panelFont.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pceFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).EndInit();
			this.fontEditControlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelFontPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).EndInit();
			this.panelColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColorPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAntialiasing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxWidth)).EndInit();
			this.pnlMaxWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speMaxWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxLineCount)).EndInit();
			this.pnlMaxLineCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speMaxLineCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).EndInit();
			this.pnlAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelFont;
		private DevExpress.XtraEditors.PopupContainerEdit pceFont;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl labelFont;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelFontPadding;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelColor;
		private DevExpress.XtraEditors.ColorEdit ceColor;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl labelColor;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelColorPadding;
		private DevExpress.XtraEditors.CheckEdit ceAntialiasing;
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
		private ChartPanelControl pnlAlignment;
		private DevExpress.XtraEditors.ComboBoxEdit cbAlignment;
		protected ChartLabelControl lblAlignment;
		private ChartPanelControl chartPanelControl5;
	}
}
