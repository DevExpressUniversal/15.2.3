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
	partial class DateTimeOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateTimeOptionsControl));
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbFormat = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlFormatString = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtFormatString = new DevExpress.XtraEditors.TextEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatString)).BeginInit();
			this.pnlFormatString.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtFormatString.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.cbFormat);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.cbFormat, "cbFormat");
			this.cbFormat.Name = "cbFormat";
			this.cbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFormat.Properties.Buttons"))))});
			this.cbFormat.Properties.Items.AddRange(new object[] {
			resources.GetString("cbFormat.Properties.Items"),
			resources.GetString("cbFormat.Properties.Items1"),
			resources.GetString("cbFormat.Properties.Items2"),
			resources.GetString("cbFormat.Properties.Items3"),
			resources.GetString("cbFormat.Properties.Items4"),
			resources.GetString("cbFormat.Properties.Items5"),
			resources.GetString("cbFormat.Properties.Items6"),
			resources.GetString("cbFormat.Properties.Items7"),
			resources.GetString("cbFormat.Properties.Items8")});
			this.cbFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFormat.SelectedIndexChanged += new System.EventHandler(this.cbFormat_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.pnlFormatString, "pnlFormatString");
			this.pnlFormatString.BackColor = System.Drawing.Color.Transparent;
			this.pnlFormatString.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFormatString.Controls.Add(this.txtFormatString);
			this.pnlFormatString.Controls.Add(this.chartLabelControl1);
			this.pnlFormatString.Name = "pnlFormatString";
			resources.ApplyResources(this.txtFormatString, "txtFormatString");
			this.txtFormatString.Name = "txtFormatString";
			this.txtFormatString.EditValueChanged += new System.EventHandler(this.txtFormatString_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlFormatString);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl4);
			this.Name = "DateTimeOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatString)).EndInit();
			this.pnlFormatString.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtFormatString.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl4;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl chartPanelControl3;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlFormatString;
		private DevExpress.XtraEditors.ComboBoxEdit cbFormat;
		private DevExpress.XtraEditors.TextEdit txtFormatString;
		protected DevExpress.XtraCharts.Wizard.ChartLabelControl chartLabelControl3;
		protected DevExpress.XtraCharts.Wizard.ChartLabelControl chartLabelControl1;
	}
}
