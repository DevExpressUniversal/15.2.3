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
	partial class TopNOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopNOptionsControl));
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlCount = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtCount = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlThresholdValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtThresholdValue = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlThresholdPercent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtThresholdPercent = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlOthers = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtOthersArgument = new DevExpress.XtraEditors.TextEdit();
			this.chartLabelControl6 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl21 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chShowOthers = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCount)).BeginInit();
			this.pnlCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThresholdValue)).BeginInit();
			this.pnlThresholdValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtThresholdValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThresholdPercent)).BeginInit();
			this.pnlThresholdPercent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtThresholdPercent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlOthers)).BeginInit();
			this.pnlOthers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtOthersArgument.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowOthers.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.cbMode);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.cbMode, "cbMode");
			this.cbMode.Name = "cbMode";
			this.cbMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMode.Properties.Buttons"))))});
			this.cbMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbMode.Properties.Items"),
			resources.GetString("cbMode.Properties.Items1"),
			resources.GetString("cbMode.Properties.Items2")});
			this.cbMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.pnlCount, "pnlCount");
			this.pnlCount.BackColor = System.Drawing.Color.Transparent;
			this.pnlCount.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCount.Controls.Add(this.txtCount);
			this.pnlCount.Controls.Add(this.chartLabelControl1);
			this.pnlCount.Name = "pnlCount";
			resources.ApplyResources(this.txtCount, "txtCount");
			this.txtCount.Name = "txtCount";
			this.txtCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtCount.Properties.IsFloatValue = false;
			this.txtCount.Properties.Mask.EditMask = resources.GetString("txtCount.Properties.Mask.EditMask");
			this.txtCount.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.txtCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtCount.EditValueChanged += new System.EventHandler(this.txtCount_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this.pnlThresholdValue, "pnlThresholdValue");
			this.pnlThresholdValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlThresholdValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlThresholdValue.Controls.Add(this.txtThresholdValue);
			this.pnlThresholdValue.Controls.Add(this.chartLabelControl2);
			this.pnlThresholdValue.Name = "pnlThresholdValue";
			resources.ApplyResources(this.txtThresholdValue, "txtThresholdValue");
			this.txtThresholdValue.Name = "txtThresholdValue";
			this.txtThresholdValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtThresholdValue.EditValueChanged += new System.EventHandler(this.txtThresholdValue_EditValueChanged);
			this.txtThresholdValue.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtThresholdValue_EditValueChanging);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			resources.ApplyResources(this.pnlThresholdPercent, "pnlThresholdPercent");
			this.pnlThresholdPercent.BackColor = System.Drawing.Color.Transparent;
			this.pnlThresholdPercent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlThresholdPercent.Controls.Add(this.txtThresholdPercent);
			this.pnlThresholdPercent.Controls.Add(this.chartLabelControl4);
			this.pnlThresholdPercent.Name = "pnlThresholdPercent";
			resources.ApplyResources(this.txtThresholdPercent, "txtThresholdPercent");
			this.txtThresholdPercent.Name = "txtThresholdPercent";
			this.txtThresholdPercent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtThresholdPercent.EditValueChanged += new System.EventHandler(this.txtThresholdPercent_EditValueChanged);
			this.txtThresholdPercent.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtThresholdPercent_EditValueChanging);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.pnlOthers, "pnlOthers");
			this.pnlOthers.BackColor = System.Drawing.Color.Transparent;
			this.pnlOthers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlOthers.Controls.Add(this.chartPanelControl5);
			this.pnlOthers.Controls.Add(this.chShowOthers);
			this.pnlOthers.Name = "pnlOthers";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.txtOthersArgument);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl6);
			this.chartPanelControl5.Controls.Add(this.chartPanelControl21);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.txtOthersArgument, "txtOthersArgument");
			this.txtOthersArgument.Name = "txtOthersArgument";
			this.txtOthersArgument.EditValueChanged += new System.EventHandler(this.txtOthersArgument_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl6, "chartLabelControl6");
			this.chartLabelControl6.Name = "chartLabelControl6";
			this.chartPanelControl21.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl21.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl21, "chartPanelControl21");
			this.chartPanelControl21.Name = "chartPanelControl21";
			resources.ApplyResources(this.chShowOthers, "chShowOthers");
			this.chShowOthers.Name = "chShowOthers";
			this.chShowOthers.Properties.Caption = resources.GetString("chShowOthers.Properties.Caption");
			this.chShowOthers.CheckedChanged += new System.EventHandler(this.chShowOthers_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlOthers);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.pnlThresholdPercent);
			this.Controls.Add(this.pnlThresholdValue);
			this.Controls.Add(this.pnlCount);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl4);
			this.Name = "TopNOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCount)).EndInit();
			this.pnlCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThresholdValue)).EndInit();
			this.pnlThresholdValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtThresholdValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThresholdPercent)).EndInit();
			this.pnlThresholdPercent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtThresholdPercent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlOthers)).EndInit();
			this.pnlOthers.ResumeLayout(false);
			this.pnlOthers.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtOthersArgument.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowOthers.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.ComboBoxEdit cbMode;
		protected ChartLabelControl chartLabelControl3;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl pnlCount;
		private DevExpress.XtraEditors.SpinEdit txtCount;
		protected ChartLabelControl chartLabelControl1;
		private ChartPanelControl pnlThresholdValue;
		private DevExpress.XtraEditors.SpinEdit txtThresholdValue;
		protected ChartLabelControl chartLabelControl2;
		private ChartPanelControl pnlThresholdPercent;
		private DevExpress.XtraEditors.SpinEdit txtThresholdPercent;
		protected ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl pnlOthers;
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraEditors.TextEdit txtOthersArgument;
		protected ChartLabelControl chartLabelControl6;
		protected DevExpress.XtraEditors.CheckEdit chShowOthers;
		private ChartPanelControl chartPanelControl21;
	}
}
