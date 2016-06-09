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
	partial class SingleLevelIndicatorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleLevelIndicatorControl));
			this.cbValueLevel = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelValueLevel = new DevExpress.XtraEditors.LabelControl();
			this.panelValueLevel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pageAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.lineStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			this.panelColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.labelColor = new DevExpress.XtraEditors.LabelControl();
			this.sepAppearance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).BeginInit();
			this.panelValueLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.pageAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).BeginInit();
			this.panelColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAppearance)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelName, "panelName");
			resources.ApplyResources(this.txtName, "txtName");
			resources.ApplyResources(this.chkVisible, "chkVisible");
			resources.ApplyResources(this.chkShowInLegend, "chkShowInLegend");
			resources.ApplyResources(this.chkCheckableInLegend, "chkCheckableInLegend");
			resources.ApplyResources(this.chkCheckedInLegend, "chkCheckedInLegend");
			resources.ApplyResources(this.cbValueLevel, "cbValueLevel");
			this.cbValueLevel.Name = "cbValueLevel";
			this.cbValueLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbValueLevel.Properties.Buttons"))))});
			this.cbValueLevel.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbValueLevel.SelectedIndexChanged += new System.EventHandler(this.cbValueLevel_SelectedIndexChanged);
			resources.ApplyResources(this.labelValueLevel, "labelValueLevel");
			this.labelValueLevel.Name = "labelValueLevel";
			resources.ApplyResources(this.panelValueLevel, "panelValueLevel");
			this.panelValueLevel.BackColor = System.Drawing.Color.Transparent;
			this.panelValueLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelValueLevel.Controls.Add(this.cbValueLevel);
			this.panelValueLevel.Controls.Add(this.labelValueLevel);
			this.panelValueLevel.Name = "panelValueLevel";
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.pageAppearance;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageAppearance});
			this.pageAppearance.Controls.Add(this.lineStyleControl);
			this.pageAppearance.Controls.Add(this.panelColor);
			this.pageAppearance.Name = "pageAppearance";
			resources.ApplyResources(this.pageAppearance, "pageAppearance");
			resources.ApplyResources(this.lineStyleControl, "lineStyleControl");
			this.lineStyleControl.Name = "lineStyleControl";
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
			this.sepAppearance.BackColor = System.Drawing.Color.Transparent;
			this.sepAppearance.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAppearance, "sepAppearance");
			this.sepAppearance.Name = "sepAppearance";
			this.Controls.Add(this.xtraTabControl);
			this.Controls.Add(this.sepAppearance);
			this.Controls.Add(this.panelValueLevel);
			this.Name = "SingleLevelIndicatorControl";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.panelValueLevel, 0);
			this.Controls.SetChildIndex(this.panelName, 0);
			this.Controls.SetChildIndex(this.chkVisible, 0);
			this.Controls.SetChildIndex(this.chkShowInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckableInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckedInLegend, 0);
			this.Controls.SetChildIndex(this.sepAppearance, 0);
			this.Controls.SetChildIndex(this.xtraTabControl, 0);
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).EndInit();
			this.panelValueLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.pageAppearance.ResumeLayout(false);
			this.pageAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).EndInit();
			this.panelColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAppearance)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected ChartPanelControl panelValueLevel;
		protected DevExpress.XtraEditors.ComboBoxEdit cbValueLevel;
		protected DevExpress.XtraEditors.LabelControl labelValueLevel;
		private XtraTab.XtraTabPage pageAppearance;
		private LineStyleControl lineStyleControl;
		private ChartPanelControl panelColor;
		private XtraEditors.ColorEdit ceColor;
		private XtraEditors.LabelControl labelColor;
		protected XtraTab.XtraTabControl xtraTabControl;
		protected ChartPanelControl sepAppearance;
	}
}
