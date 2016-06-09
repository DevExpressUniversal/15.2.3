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

namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	partial class LegendTextControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendTextControl));
			this.txtFont = new DevExpress.XtraEditors.PopupContainerEdit();
			this.fontEditControlContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.fontEditControl = new DevExpress.XtraCharts.Design.FontEditControl();
			this.ceTextColor = new DevExpress.XtraEditors.ColorEdit();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chAntialize = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblFont = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl14 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.txtFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).BeginInit();
			this.fontEditControlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.txtFont, "txtFont");
			this.txtFont.Name = "txtFont";
			this.txtFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("txtFont.Properties.Buttons"))))});
			this.txtFont.Properties.PopupControl = this.fontEditControlContainer;
			this.txtFont.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.txtFont_QueryResultValue);
			this.txtFont.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.txtFont_QueryPopUp);
			this.fontEditControlContainer.Controls.Add(this.fontEditControl);
			resources.ApplyResources(this.fontEditControlContainer, "fontEditControlContainer");
			this.fontEditControlContainer.Name = "fontEditControlContainer";
			resources.ApplyResources(this.fontEditControl, "fontEditControl");
			this.fontEditControl.EditedFont = null;
			this.fontEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.fontEditControl.Name = "fontEditControl";
			this.fontEditControl.OnNeedClose += new System.EventHandler(this.fontEditControl_OnNeedClose);
			resources.ApplyResources(this.ceTextColor, "ceTextColor");
			this.ceTextColor.Name = "ceTextColor";
			this.ceTextColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceTextColor.Properties.Buttons"))))});
			this.ceTextColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceTextColor.EditValueChanged += new System.EventHandler(this.ceTextColor_EditValueChanged);
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.chAntialize, "chAntialize");
			this.chAntialize.Name = "chAntialize";
			this.chAntialize.Properties.AllowGrayed = true;
			this.chAntialize.Properties.Caption = resources.GetString("chAntialize.Properties.Caption");
			this.chAntialize.CheckStateChanged += new System.EventHandler(this.chAntialize_CheckedChanged);
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.ceTextColor);
			this.chartPanelControl2.Controls.Add(this.lblColor);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.txtFont);
			this.chartPanelControl3.Controls.Add(this.lblFont);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.lblFont, "lblFont");
			this.lblFont.Name = "lblFont";
			this.panelControl14.BackColor = System.Drawing.Color.Transparent;
			this.panelControl14.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl14, "panelControl14");
			this.panelControl14.Name = "panelControl14";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.fontEditControlContainer);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.chAntialize);
			this.Controls.Add(this.panelControl14);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.chVisible);
			this.Name = "LegendTextControl";
			((System.ComponentModel.ISupportInitialize)(this.txtFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fontEditControlContainer)).EndInit();
			this.fontEditControlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceTextColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chAntialize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.PopupContainerEdit txtFont;
		private DevExpress.XtraEditors.ColorEdit ceTextColor;
		private ChartPanelControl panelControl1;
		private DevExpress.XtraEditors.CheckEdit chAntialize;
		private DevExpress.XtraEditors.PopupContainerControl fontEditControlContainer;
		private DevExpress.XtraCharts.Design.FontEditControl fontEditControl;
		private ChartPanelControl chartPanelControl2;
		private ChartLabelControl lblColor;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl3;
		private ChartLabelControl lblFont;
		private ChartPanelControl panelControl14;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.CheckEdit chVisible;
	}
}
