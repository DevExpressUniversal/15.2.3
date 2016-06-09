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
	partial class AxisFormatControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisFormatControl));
			this.pnlNumericOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ctrlNumericOptions = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisNumericOptionsControl();
			this.ctrlDateTimeOptions = new DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisDateTimeOptionsControl();
			this.pnlDateTimeOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelBeginText = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtBeginText = new DevExpress.XtraEditors.TextEdit();
			this.labelBeginText = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelBeginTextPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelEndText = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtEndText = new DevExpress.XtraEditors.TextEdit();
			this.labelEndText = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelEndTextPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlNumericOptions)).BeginInit();
			this.pnlNumericOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDateTimeOptions)).BeginInit();
			this.pnlDateTimeOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelBeginText)).BeginInit();
			this.panelBeginText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtBeginText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBeginTextPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEndText)).BeginInit();
			this.panelEndText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtEndText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEndTextPadding)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlNumericOptions, "pnlNumericOptions");
			this.pnlNumericOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnlNumericOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlNumericOptions.Controls.Add(this.ctrlNumericOptions);
			this.pnlNumericOptions.Name = "pnlNumericOptions";
			resources.ApplyResources(this.ctrlNumericOptions, "ctrlNumericOptions");
			this.ctrlNumericOptions.CausesValidation = false;
			this.ctrlNumericOptions.Name = "ctrlNumericOptions";
			resources.ApplyResources(this.ctrlDateTimeOptions, "ctrlDateTimeOptions");
			this.ctrlDateTimeOptions.Name = "ctrlDateTimeOptions";
			resources.ApplyResources(this.pnlDateTimeOptions, "pnlDateTimeOptions");
			this.pnlDateTimeOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnlDateTimeOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDateTimeOptions.Controls.Add(this.ctrlDateTimeOptions);
			this.pnlDateTimeOptions.Name = "pnlDateTimeOptions";
			resources.ApplyResources(this.panelBeginText, "panelBeginText");
			this.panelBeginText.BackColor = System.Drawing.Color.Transparent;
			this.panelBeginText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelBeginText.Controls.Add(this.txtBeginText);
			this.panelBeginText.Controls.Add(this.labelBeginText);
			this.panelBeginText.Name = "panelBeginText";
			resources.ApplyResources(this.txtBeginText, "txtBeginText");
			this.txtBeginText.Name = "txtBeginText";
			this.txtBeginText.EditValueChanged += new System.EventHandler(this.txtBeginText_EditValueChanged);
			resources.ApplyResources(this.labelBeginText, "labelBeginText");
			this.labelBeginText.Name = "labelBeginText";
			this.panelBeginTextPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelBeginTextPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelBeginTextPadding, "panelBeginTextPadding");
			this.panelBeginTextPadding.Name = "panelBeginTextPadding";
			resources.ApplyResources(this.panelEndText, "panelEndText");
			this.panelEndText.BackColor = System.Drawing.Color.Transparent;
			this.panelEndText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelEndText.Controls.Add(this.txtEndText);
			this.panelEndText.Controls.Add(this.labelEndText);
			this.panelEndText.Name = "panelEndText";
			resources.ApplyResources(this.txtEndText, "txtEndText");
			this.txtEndText.Name = "txtEndText";
			this.txtEndText.EditValueChanged += new System.EventHandler(this.txtEndText_EditValueChanged);
			resources.ApplyResources(this.labelEndText, "labelEndText");
			this.labelEndText.Name = "labelEndText";
			this.panelEndTextPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelEndTextPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelEndTextPadding, "panelEndTextPadding");
			this.panelEndTextPadding.Name = "panelEndTextPadding";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelEndText);
			this.Controls.Add(this.panelEndTextPadding);
			this.Controls.Add(this.panelBeginText);
			this.Controls.Add(this.panelBeginTextPadding);
			this.Controls.Add(this.pnlDateTimeOptions);
			this.Controls.Add(this.pnlNumericOptions);
			this.Name = "AxisFormatControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlNumericOptions)).EndInit();
			this.pnlNumericOptions.ResumeLayout(false);
			this.pnlNumericOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDateTimeOptions)).EndInit();
			this.pnlDateTimeOptions.ResumeLayout(false);
			this.pnlDateTimeOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelBeginText)).EndInit();
			this.panelBeginText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtBeginText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBeginTextPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEndText)).EndInit();
			this.panelEndText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtEndText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEndTextPadding)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlNumericOptions;
		private DevExpress.XtraCharts.Wizard.ChartAxesControls.AxisNumericOptionsControl ctrlNumericOptions;
		private AxisDateTimeOptionsControl ctrlDateTimeOptions;
		private ChartPanelControl pnlDateTimeOptions;
		private ChartPanelControl panelBeginText;
		private DevExpress.XtraEditors.TextEdit txtBeginText;
		private ChartLabelControl labelBeginText;
		private ChartPanelControl panelBeginTextPadding;
		private ChartPanelControl panelEndText;
		private DevExpress.XtraEditors.TextEdit txtEndText;
		private ChartLabelControl labelEndText;
		private ChartPanelControl panelEndTextPadding;
	}
}
