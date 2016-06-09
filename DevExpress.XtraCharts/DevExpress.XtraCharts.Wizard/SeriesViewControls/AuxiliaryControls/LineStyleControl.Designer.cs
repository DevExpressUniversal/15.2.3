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
	partial class LineStyleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineStyleControl));
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbDashStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtSize = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDashStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.cbDashStyle);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.cbDashStyle, "cbDashStyle");
			this.cbDashStyle.Name = "cbDashStyle";
			this.cbDashStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDashStyle.Properties.Buttons"))))});
			this.cbDashStyle.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDashStyle.Properties.Items"),
			resources.GetString("cbDashStyle.Properties.Items1"),
			resources.GetString("cbDashStyle.Properties.Items2"),
			resources.GetString("cbDashStyle.Properties.Items3"),
			resources.GetString("cbDashStyle.Properties.Items4")});
			this.cbDashStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDashStyle.SelectedIndexChanged += new System.EventHandler(this.cbDashStyle_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtSize);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtSize, "txtSize");
			this.txtSize.Name = "txtSize";
			this.txtSize.Properties.Appearance.Options.UseTextOptions = true;
			this.txtSize.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtSize.Properties.IsFloatValue = false;
			this.txtSize.Properties.Mask.EditMask = resources.GetString("txtSize.Properties.Mask.EditMask");
			this.txtSize.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtSize.Properties.Mask.IgnoreMaskBlank")));
			this.txtSize.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtSize.Properties.Mask.ShowPlaceHolders")));
			this.txtSize.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtSize.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtSize.EditValueChanged += new System.EventHandler(this.txtSize_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.panelControl3);
			this.Controls.Add(this.chartPanelControl5);
			this.Name = "LineStyleControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDashStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl chartPanelControl5;
		protected DevExpress.XtraEditors.ComboBoxEdit cbDashStyle;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.SpinEdit txtSize;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl panelControl3;
	}
}
