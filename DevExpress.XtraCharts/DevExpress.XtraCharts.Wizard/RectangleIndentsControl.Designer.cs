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
	partial class RectangleIndentsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RectangleIndentsControl));
			this.txtBottom = new DevExpress.XtraEditors.SpinEdit();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtLeft = new DevExpress.XtraEditors.SpinEdit();
			this.txtRight = new DevExpress.XtraEditors.SpinEdit();
			this.txtTop = new DevExpress.XtraEditors.SpinEdit();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblBottom = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlThickness = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblLeft = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblRight = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblTop = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtAll = new DevExpress.XtraEditors.SpinEdit();
			this.lblAll = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.txtBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			this.chartPanelControl7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			this.chartPanelControl6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtAll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.txtBottom, "txtBottom");
			this.txtBottom.Name = "txtBottom";
			this.txtBottom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtBottom.Properties.IsFloatValue = false;
			this.txtBottom.Properties.Mask.EditMask = resources.GetString("txtBottom.Properties.Mask.EditMask");
			this.txtBottom.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtBottom.Properties.ValidateOnEnterKey = true;
			this.txtBottom.EditValueChanged += new System.EventHandler(this.txtBottom_EditValueChanged);
			this.txtBottom.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.CustomDisplayText);
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this.txtLeft, "txtLeft");
			this.txtLeft.Name = "txtLeft";
			this.txtLeft.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtLeft.Properties.IsFloatValue = false;
			this.txtLeft.Properties.Mask.EditMask = resources.GetString("txtLeft.Properties.Mask.EditMask");
			this.txtLeft.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtLeft.Properties.ValidateOnEnterKey = true;
			this.txtLeft.EditValueChanged += new System.EventHandler(this.txtLeft_EditValueChanged);
			this.txtLeft.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.CustomDisplayText);
			resources.ApplyResources(this.txtRight, "txtRight");
			this.txtRight.Name = "txtRight";
			this.txtRight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtRight.Properties.IsFloatValue = false;
			this.txtRight.Properties.Mask.EditMask = resources.GetString("txtRight.Properties.Mask.EditMask");
			this.txtRight.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtRight.Properties.ValidateOnEnterKey = true;
			this.txtRight.EditValueChanged += new System.EventHandler(this.txtRight_EditValueChanged);
			this.txtRight.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.CustomDisplayText);
			resources.ApplyResources(this.txtTop, "txtTop");
			this.txtTop.Name = "txtTop";
			this.txtTop.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtTop.Properties.IsFloatValue = false;
			this.txtTop.Properties.Mask.EditMask = resources.GetString("txtTop.Properties.Mask.EditMask");
			this.txtTop.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtTop.Properties.ValidateOnEnterKey = true;
			this.txtTop.EditValueChanged += new System.EventHandler(this.txtTop_EditValueChanged);
			this.txtTop.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.CustomDisplayText);
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.txtBottom);
			this.chartPanelControl4.Controls.Add(this.lblBottom);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.Name = "lblBottom";
			this.pnlThickness.BackColor = System.Drawing.Color.Transparent;
			this.pnlThickness.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlThickness, "pnlThickness");
			this.pnlThickness.Name = "pnlThickness";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.txtLeft);
			this.chartPanelControl2.Controls.Add(this.lblLeft);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.Name = "lblLeft";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.txtRight);
			this.chartPanelControl5.Controls.Add(this.lblRight);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl7.Controls.Add(this.txtTop);
			this.chartPanelControl7.Controls.Add(this.lblTop);
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.Name = "lblTop";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Controls.Add(this.txtAll);
			this.chartPanelControl6.Controls.Add(this.lblAll);
			this.chartPanelControl6.Name = "chartPanelControl6";
			resources.ApplyResources(this.txtAll, "txtAll");
			this.txtAll.Name = "txtAll";
			this.txtAll.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtAll.Properties.IsFloatValue = false;
			this.txtAll.Properties.Mask.EditMask = resources.GetString("txtAll.Properties.Mask.EditMask");
			this.txtAll.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtAll.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.txtAll.Properties.ValidateOnEnterKey = true;
			this.txtAll.EditValueChanged += new System.EventHandler(this.txtAll_EditValueChanged);
			this.txtAll.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.txtAll_CustomDisplayText);
			resources.ApplyResources(this.lblAll, "lblAll");
			this.lblAll.Name = "lblAll";
			this.chartPanelControl8.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl8, "chartPanelControl8");
			this.chartPanelControl8.Name = "chartPanelControl8";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl7);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl5);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.pnlThickness);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.panelControl4);
			this.Controls.Add(this.chartPanelControl8);
			this.Controls.Add(this.chartPanelControl6);
			this.Name = "RectangleIndentsControl";
			((System.ComponentModel.ISupportInitialize)(this.txtBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			this.chartPanelControl7.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			this.chartPanelControl6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtAll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SpinEdit txtBottom;
		private ChartPanelControl panelControl4;
		private DevExpress.XtraEditors.SpinEdit txtLeft;
		private DevExpress.XtraEditors.SpinEdit txtRight;
		private DevExpress.XtraEditors.SpinEdit txtTop;
		private ChartPanelControl chartPanelControl4;
		private ChartLabelControl lblBottom;
		private ChartPanelControl pnlThickness;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl2;
		private ChartLabelControl lblLeft;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl5;
		private ChartLabelControl lblRight;
		private ChartPanelControl chartPanelControl7;
		private ChartLabelControl lblTop;
		private ChartPanelControl chartPanelControl6;
		private DevExpress.XtraEditors.SpinEdit txtAll;
		private ChartLabelControl lblAll;
		private ChartPanelControl chartPanelControl8;
	}
}
