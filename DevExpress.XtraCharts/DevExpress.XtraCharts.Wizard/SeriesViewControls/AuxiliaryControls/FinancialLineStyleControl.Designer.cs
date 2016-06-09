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
	partial class FinancialLineStyleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinancialLineStyleControl));
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlThickness = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtThickness = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtLength = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblOpenClose = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbOpenClose = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistanceFixed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblOpenClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbOpenClose.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.ceColor);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.ceColor, "ceColor");
			this.ceColor.Name = "ceColor";
			this.ceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceColor.Properties.Buttons"))))});
			this.ceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceColor.EditValueChanged += new System.EventHandler(this.ceColor_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			this.pnlThickness.BackColor = System.Drawing.Color.Transparent;
			this.pnlThickness.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlThickness, "pnlThickness");
			this.pnlThickness.Name = "pnlThickness";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.txtThickness);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.txtThickness, "txtThickness");
			this.txtThickness.Name = "txtThickness";
			this.txtThickness.Properties.Appearance.Options.UseTextOptions = true;
			this.txtThickness.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtThickness.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtThickness.Properties.IsFloatValue = false;
			this.txtThickness.Properties.Mask.EditMask = resources.GetString("txtThickness.Properties.Mask.EditMask");
			this.txtThickness.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtThickness.Properties.Mask.IgnoreMaskBlank")));
			this.txtThickness.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtThickness.Properties.Mask.ShowPlaceHolders")));
			this.txtThickness.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtThickness.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtThickness.EditValueChanged += new System.EventHandler(this.txtDistance_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.pnlDistanceFixed.BackColor = System.Drawing.Color.Transparent;
			this.pnlDistanceFixed.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlDistanceFixed, "pnlDistanceFixed");
			this.pnlDistanceFixed.Name = "pnlDistanceFixed";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.txtLength);
			this.chartPanelControl2.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.txtLength, "txtLength");
			this.txtLength.Name = "txtLength";
			this.txtLength.Properties.Appearance.Options.UseTextOptions = true;
			this.txtLength.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtLength.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.txtLength.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtLength.Properties.Mask.IgnoreMaskBlank")));
			this.txtLength.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtLength.Properties.Mask.ShowPlaceHolders")));
			this.txtLength.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtLength.EditValueChanged += new System.EventHandler(this.txtLength_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.lblOpenClose.BackColor = System.Drawing.Color.Transparent;
			this.lblOpenClose.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.lblOpenClose, "lblOpenClose");
			this.lblOpenClose.Name = "lblOpenClose";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.cbOpenClose);
			this.chartPanelControl3.Controls.Add(this.chartLabelControl3);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.cbOpenClose, "cbOpenClose");
			this.cbOpenClose.Name = "cbOpenClose";
			this.cbOpenClose.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOpenClose.Properties.Buttons"))))});
			this.cbOpenClose.Properties.Items.AddRange(new object[] {
			resources.GetString("cbOpenClose.Properties.Items"),
			resources.GetString("cbOpenClose.Properties.Items1"),
			resources.GetString("cbOpenClose.Properties.Items2")});
			this.cbOpenClose.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbOpenClose.EditValueChanged += new System.EventHandler(this.cbOpenClose_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.lblOpenClose);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.pnlDistanceFixed);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.pnlThickness);
			this.Controls.Add(this.chartPanelControl5);
			this.Name = "FinancialLineStyleControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistanceFixed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblOpenClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbOpenClose.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraEditors.ColorEdit ceColor;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl pnlThickness;
		private ChartPanelControl chartPanelControl1;
		protected DevExpress.XtraEditors.SpinEdit txtThickness;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl pnlDistanceFixed;
		private ChartPanelControl chartPanelControl2;
		protected DevExpress.XtraEditors.SpinEdit txtLength;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl lblOpenClose;
		private ChartPanelControl chartPanelControl3;
		private DevExpress.XtraEditors.ComboBoxEdit cbOpenClose;
		private ChartLabelControl chartLabelControl3;
	}
}
