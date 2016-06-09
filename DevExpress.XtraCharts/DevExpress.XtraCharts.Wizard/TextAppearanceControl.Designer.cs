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
	partial class TextAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextAppearanceControl));
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popFont = new DevExpress.XtraEditors.PopupContainerEdit();
			this.fontEditControlContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.fontEditControl = new DevExpress.XtraCharts.Design.FontEditControl();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.clrColor = new DevExpress.XtraEditors.ColorEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAntialiasing = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).BeginInit();
			this.fontEditControlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialiasing.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.popFont);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.popFont, "popFont");
			this.popFont.Name = "popFont";
			this.popFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("popFont.Properties.Buttons"))))});
			this.popFont.Properties.PopupControl = this.fontEditControlContainer;
			this.popFont.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.popFont_QueryResultValue);
			this.popFont.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.popFont_QueryPopUp);
			this.fontEditControlContainer.Controls.Add(this.fontEditControl);
			resources.ApplyResources(this.fontEditControlContainer, "fontEditControlContainer");
			this.fontEditControlContainer.Name = "fontEditControlContainer";
			resources.ApplyResources(this.fontEditControl, "fontEditControl");
			this.fontEditControl.EditedFont = null;
			this.fontEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.fontEditControl.Name = "fontEditControl";
			this.fontEditControl.OnNeedClose += new System.EventHandler(this.fontEditControl_OnNeedClose);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.clrColor);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.clrColor, "clrColor");
			this.clrColor.Name = "clrColor";
			this.clrColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("clrColor.Properties.Buttons"))))});
			this.clrColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.clrColor.ColorChanged += new System.EventHandler(this.clrColor_ColorChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Name = "chartPanelControl6";
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.chAntialiasing, "chAntialiasing");
			this.chAntialiasing.Name = "chAntialiasing";
			this.chAntialiasing.Properties.AllowGrayed = true;
			this.chAntialiasing.Properties.Caption = resources.GetString("chAntialiasing.Properties.Caption");
			this.chAntialiasing.CheckStateChanged += new System.EventHandler(this.chAntialiasing_CheckStateChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.fontEditControlContainer);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.chartPanelControl6);
			this.Controls.Add(this.chartPanelControl7);
			this.Controls.Add(this.chAntialiasing);
			this.Name = "TextAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).EndInit();
			this.fontEditControlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialiasing.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.PopupContainerEdit popFont;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.ColorEdit clrColor;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl6;
		private ChartPanelControl chartPanelControl7;
		private DevExpress.XtraEditors.CheckEdit chAntialiasing;
		private DevExpress.XtraEditors.PopupContainerControl fontEditControlContainer;
		private DevExpress.XtraCharts.Design.FontEditControl fontEditControl;
	}
}
