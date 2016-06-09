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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class TrendLineControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrendLineControl));
			this.panelProperties = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbcData = new DevExpress.XtraTab.XtraTabPage();
			this.financialIndicatorControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FinancialIndicatorControl();
			this.tbcAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.lineStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			this.panelColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraEditors.LabelControl();
			this.chkExtrapolate = new DevExpress.XtraEditors.CheckEdit();
			this.panelPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelProperties)).BeginInit();
			this.panelProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.tbcData.SuspendLayout();
			this.tbcAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).BeginInit();
			this.panelColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkExtrapolate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPadding)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelName, "panelName");
			resources.ApplyResources(this.txtName, "txtName");
			resources.ApplyResources(this.chkVisible, "chkVisible");
			resources.ApplyResources(this.chkShowInLegend, "chkShowInLegend");
			resources.ApplyResources(this.chkCheckableInLegend, "chkCheckableInLegend");
			resources.ApplyResources(this.chkCheckedInLegend, "chkCheckedInLegend");
			this.panelProperties.BackColor = System.Drawing.Color.Transparent;
			this.panelProperties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelProperties.Controls.Add(this.xtraTabControl);
			resources.ApplyResources(this.panelProperties, "panelProperties");
			this.panelProperties.Name = "panelProperties";
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.tbcData;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbcData,
			this.tbcAppearance});
			this.tbcData.Controls.Add(this.financialIndicatorControl);
			this.tbcData.Name = "tbcData";
			resources.ApplyResources(this.tbcData, "tbcData");
			resources.ApplyResources(this.financialIndicatorControl, "financialIndicatorControl");
			this.financialIndicatorControl.Name = "financialIndicatorControl";
			this.tbcAppearance.Controls.Add(this.lineStyleControl);
			this.tbcAppearance.Controls.Add(this.panelColor);
			this.tbcAppearance.Name = "tbcAppearance";
			resources.ApplyResources(this.tbcAppearance, "tbcAppearance");
			resources.ApplyResources(this.lineStyleControl, "lineStyleControl");
			this.lineStyleControl.Name = "lineStyleControl";
			resources.ApplyResources(this.panelColor, "panelColor");
			this.panelColor.BackColor = System.Drawing.Color.Transparent;
			this.panelColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelColor.Controls.Add(this.ceColor);
			this.panelColor.Controls.Add(this.lblColor);
			this.panelColor.Name = "panelColor";
			resources.ApplyResources(this.ceColor, "ceColor");
			this.ceColor.Name = "ceColor";
			this.ceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceColor.Properties.Buttons"))))});
			this.ceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceColor.EditValueChanged += new System.EventHandler(this.ceColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			resources.ApplyResources(this.chkExtrapolate, "chkExtrapolate");
			this.chkExtrapolate.Name = "chkExtrapolate";
			this.chkExtrapolate.Properties.Caption = resources.GetString("chkExtrapolate.Properties.Caption");
			this.chkExtrapolate.CheckedChanged += new System.EventHandler(this.chkExtrapolate_CheckedChanged);
			this.panelPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelPadding, "panelPadding");
			this.panelPadding.Name = "panelPadding";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelProperties);
			this.Controls.Add(this.panelPadding);
			this.Controls.Add(this.chkExtrapolate);
			this.Name = "TrendLineControl";
			this.Controls.SetChildIndex(this.panelName, 0);
			this.Controls.SetChildIndex(this.chkVisible, 0);
			this.Controls.SetChildIndex(this.chkShowInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckableInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckedInLegend, 0);
			this.Controls.SetChildIndex(this.chkExtrapolate, 0);
			this.Controls.SetChildIndex(this.panelPadding, 0);
			this.Controls.SetChildIndex(this.panelProperties, 0);
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelProperties)).EndInit();
			this.panelProperties.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.tbcData.ResumeLayout(false);
			this.tbcData.PerformLayout();
			this.tbcAppearance.ResumeLayout(false);
			this.tbcAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).EndInit();
			this.panelColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkExtrapolate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPadding)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelProperties;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl;
		private DevExpress.XtraTab.XtraTabPage tbcData;
		private FinancialIndicatorControl financialIndicatorControl;
		private DevExpress.XtraTab.XtraTabPage tbcAppearance;
		private LineStyleControl lineStyleControl;
		private ChartPanelControl panelColor;
		private DevExpress.XtraEditors.ColorEdit ceColor;
		private DevExpress.XtraEditors.LabelControl lblColor;
		private DevExpress.XtraEditors.CheckEdit chkExtrapolate;
		private ChartPanelControl panelPadding;
	}
}
