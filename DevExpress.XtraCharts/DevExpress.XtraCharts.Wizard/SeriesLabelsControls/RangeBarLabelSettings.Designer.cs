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
	partial class RangeBarLabelSettings {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RangeBarLabelSettings));
			this.pnlLineSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtIndent = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlLineSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).BeginInit();
			this.pnlKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).BeginInit();
			this.pnlPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlIndent)).BeginInit();
			this.pnlIndent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtIndent.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlLineSettings, "pnlLineSettings");
			this.pnlLineSettings.BackColor = System.Drawing.Color.Transparent;
			this.pnlLineSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlLineSettings.Name = "pnlLineSettings";
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.pnlKind, "pnlKind");
			this.pnlKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlKind.Controls.Add(this.cbKind);
			this.pnlKind.Controls.Add(this.chartLabelControl2);
			this.pnlKind.Name = "pnlKind";
			resources.ApplyResources(this.cbKind, "cbKind");
			this.cbKind.Name = "cbKind";
			this.cbKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbKind.Properties.Buttons"))))});
			this.cbKind.Properties.Items.AddRange(new object[] {
			resources.GetString("cbKind.Properties.Items"),
			resources.GetString("cbKind.Properties.Items1"),
			resources.GetString("cbKind.Properties.Items2"),
			resources.GetString("cbKind.Properties.Items3")});
			this.cbKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbKind.SelectedIndexChanged += new System.EventHandler(this.cbKind_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			resources.ApplyResources(this.pnlPosition, "pnlPosition");
			this.pnlPosition.BackColor = System.Drawing.Color.Transparent;
			this.pnlPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPosition.Controls.Add(this.cbPosition);
			this.pnlPosition.Controls.Add(this.chartLabelControl3);
			this.pnlPosition.Name = "pnlPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPosition.Properties.Items"),
			resources.GetString("cbPosition.Properties.Items1"),
			resources.GetString("cbPosition.Properties.Items2")});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.pnlIndent, "pnlIndent");
			this.pnlIndent.BackColor = System.Drawing.Color.Transparent;
			this.pnlIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlIndent.Controls.Add(this.txtIndent);
			this.pnlIndent.Controls.Add(this.chartLabelControl1);
			this.pnlIndent.Name = "pnlIndent";
			resources.ApplyResources(this.txtIndent, "txtIndent");
			this.txtIndent.Name = "txtIndent";
			this.txtIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtIndent.Properties.IsFloatValue = false;
			this.txtIndent.Properties.Mask.EditMask = resources.GetString("txtIndent.Properties.Mask.EditMask");
			this.txtIndent.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtIndent.EditValueChanged += new System.EventHandler(this.txtIndent_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlIndent);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.pnlPosition);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.pnlKind);
			this.Controls.Add(this.pnlLineSettings);
			this.Name = "RangeBarLabelSettings";
			((System.ComponentModel.ISupportInitialize)(this.pnlLineSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).EndInit();
			this.pnlKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).EndInit();
			this.pnlPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlIndent)).EndInit();
			this.pnlIndent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtIndent.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlLineSettings;
		private ChartPanelControl panelControl1;
		private ChartPanelControl pnlKind;
		private ChartLabelControl chartLabelControl2;
		private DevExpress.XtraEditors.ComboBoxEdit cbKind;
		private ChartPanelControl pnlPosition;
		private DevExpress.XtraEditors.ComboBoxEdit cbPosition;
		private ChartLabelControl chartLabelControl3;
		private ChartPanelControl chartPanelControl4;
		private ChartPanelControl pnlIndent;
		private DevExpress.XtraEditors.SpinEdit txtIndent;
		private ChartLabelControl chartLabelControl1;
	}
}
