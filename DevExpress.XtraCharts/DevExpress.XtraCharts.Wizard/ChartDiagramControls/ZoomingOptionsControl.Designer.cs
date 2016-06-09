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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class ZoomingOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZoomingOptionsControl));
			this.chbUseKeyboardWithMouse = new DevExpress.XtraEditors.CheckEdit();
			this.separator1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chbUseKeyboard = new DevExpress.XtraEditors.CheckEdit();
			this.chbUseMouseWheel = new DevExpress.XtraEditors.CheckEdit();
			this.separator2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chbUseTouchDevice = new DevExpress.XtraEditors.CheckEdit();
			this.separator3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxZoomPercent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxZoomPercentY = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.seMaxZoomPercentY = new DevExpress.XtraEditors.SpinEdit();
			this.lbMaxZoomPercentY = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.separator5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxZoomPercentX = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.seMaxZoomPercentX = new DevExpress.XtraEditors.SpinEdit();
			this.lbMaxZoomPercentX = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.separator4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboardWithMouse.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboard.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseMouseWheel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseTouchDevice.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercent)).BeginInit();
			this.pnlMaxZoomPercent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercentY)).BeginInit();
			this.pnlMaxZoomPercentY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seMaxZoomPercentY.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercentX)).BeginInit();
			this.pnlMaxZoomPercentX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seMaxZoomPercentX.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator4)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chbUseKeyboardWithMouse, "chbUseKeyboardWithMouse");
			this.chbUseKeyboardWithMouse.Name = "chbUseKeyboardWithMouse";
			this.chbUseKeyboardWithMouse.Properties.Caption = resources.GetString("chbUseKeyboardWithMouse.Properties.Caption");
			this.chbUseKeyboardWithMouse.CheckedChanged += new System.EventHandler(this.chbUseKeyboardWithMouse_CheckedChanged);
			this.separator1.BackColor = System.Drawing.Color.Transparent;
			this.separator1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator1, "separator1");
			this.separator1.Name = "separator1";
			resources.ApplyResources(this.chbUseKeyboard, "chbUseKeyboard");
			this.chbUseKeyboard.Name = "chbUseKeyboard";
			this.chbUseKeyboard.Properties.Caption = resources.GetString("chbUseKeyboard.Properties.Caption");
			this.chbUseKeyboard.CheckedChanged += new System.EventHandler(this.chbUseKeyboard_CheckedChanged);
			resources.ApplyResources(this.chbUseMouseWheel, "chbUseMouseWheel");
			this.chbUseMouseWheel.Name = "chbUseMouseWheel";
			this.chbUseMouseWheel.Properties.Caption = resources.GetString("chbUseMouseWheel.Properties.Caption");
			this.chbUseMouseWheel.CheckedChanged += new System.EventHandler(this.chbUseMouseWheel_CheckedChanged);
			this.separator2.BackColor = System.Drawing.Color.Transparent;
			this.separator2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator2, "separator2");
			this.separator2.Name = "separator2";
			resources.ApplyResources(this.chbUseTouchDevice, "chbUseTouchDevice");
			this.chbUseTouchDevice.Name = "chbUseTouchDevice";
			this.chbUseTouchDevice.Properties.Caption = resources.GetString("chbUseTouchDevice.Properties.Caption");
			this.chbUseTouchDevice.CheckedChanged += new System.EventHandler(this.chbUseTouchDevice_CheckedChanged);
			this.separator3.BackColor = System.Drawing.Color.Transparent;
			this.separator3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator3, "separator3");
			this.separator3.Name = "separator3";
			resources.ApplyResources(this.pnlMaxZoomPercent, "pnlMaxZoomPercent");
			this.pnlMaxZoomPercent.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxZoomPercent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxZoomPercent.Controls.Add(this.pnlMaxZoomPercentY);
			this.pnlMaxZoomPercent.Controls.Add(this.separator5);
			this.pnlMaxZoomPercent.Controls.Add(this.pnlMaxZoomPercentX);
			this.pnlMaxZoomPercent.Controls.Add(this.separator4);
			this.pnlMaxZoomPercent.Name = "pnlMaxZoomPercent";
			resources.ApplyResources(this.pnlMaxZoomPercentY, "pnlMaxZoomPercentY");
			this.pnlMaxZoomPercentY.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxZoomPercentY.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxZoomPercentY.Controls.Add(this.seMaxZoomPercentY);
			this.pnlMaxZoomPercentY.Controls.Add(this.lbMaxZoomPercentY);
			this.pnlMaxZoomPercentY.Name = "pnlMaxZoomPercentY";
			resources.ApplyResources(this.seMaxZoomPercentY, "seMaxZoomPercentY");
			this.seMaxZoomPercentY.Name = "seMaxZoomPercentY";
			this.seMaxZoomPercentY.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seMaxZoomPercentY.Properties.Increment = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seMaxZoomPercentY.Properties.MaxValue = new decimal(new int[] {
			1410065408,
			2,
			0,
			0});
			this.seMaxZoomPercentY.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seMaxZoomPercentY.EditValueChanged += new System.EventHandler(this.seMaxZoomPercentY_EditValueChanged);
			resources.ApplyResources(this.lbMaxZoomPercentY, "lbMaxZoomPercentY");
			this.lbMaxZoomPercentY.Name = "lbMaxZoomPercentY";
			this.separator5.BackColor = System.Drawing.Color.Transparent;
			this.separator5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator5, "separator5");
			this.separator5.Name = "separator5";
			resources.ApplyResources(this.pnlMaxZoomPercentX, "pnlMaxZoomPercentX");
			this.pnlMaxZoomPercentX.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxZoomPercentX.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxZoomPercentX.Controls.Add(this.seMaxZoomPercentX);
			this.pnlMaxZoomPercentX.Controls.Add(this.lbMaxZoomPercentX);
			this.pnlMaxZoomPercentX.Name = "pnlMaxZoomPercentX";
			resources.ApplyResources(this.seMaxZoomPercentX, "seMaxZoomPercentX");
			this.seMaxZoomPercentX.Name = "seMaxZoomPercentX";
			this.seMaxZoomPercentX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seMaxZoomPercentX.Properties.Increment = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seMaxZoomPercentX.Properties.MaxValue = new decimal(new int[] {
			1410065408,
			2,
			0,
			0});
			this.seMaxZoomPercentX.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seMaxZoomPercentX.Properties.ValidateOnEnterKey = true;
			this.seMaxZoomPercentX.EditValueChanged += new System.EventHandler(this.seMaxZoomPercentX_EditValueChanged);
			resources.ApplyResources(this.lbMaxZoomPercentX, "lbMaxZoomPercentX");
			this.lbMaxZoomPercentX.Name = "lbMaxZoomPercentX";
			this.separator4.BackColor = System.Drawing.Color.Transparent;
			this.separator4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator4, "separator4");
			this.separator4.Name = "separator4";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlMaxZoomPercent);
			this.Controls.Add(this.chbUseTouchDevice);
			this.Controls.Add(this.separator3);
			this.Controls.Add(this.chbUseMouseWheel);
			this.Controls.Add(this.separator2);
			this.Controls.Add(this.chbUseKeyboardWithMouse);
			this.Controls.Add(this.separator1);
			this.Controls.Add(this.chbUseKeyboard);
			this.Name = "ZoomingOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboardWithMouse.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboard.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseMouseWheel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseTouchDevice.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercent)).EndInit();
			this.pnlMaxZoomPercent.ResumeLayout(false);
			this.pnlMaxZoomPercent.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercentY)).EndInit();
			this.pnlMaxZoomPercentY.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.seMaxZoomPercentY.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxZoomPercentX)).EndInit();
			this.pnlMaxZoomPercentX.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.seMaxZoomPercentX.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator4)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chbUseKeyboardWithMouse;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl separator1;
		private DevExpress.XtraEditors.CheckEdit chbUseKeyboard;
		private DevExpress.XtraEditors.CheckEdit chbUseMouseWheel;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl separator2;
		private DevExpress.XtraEditors.CheckEdit chbUseTouchDevice;
		private ChartPanelControl separator3;
		private ChartPanelControl pnlMaxZoomPercent;
		private ChartPanelControl separator4;
		private ChartPanelControl separator5;
		private ChartPanelControl pnlMaxZoomPercentX;
		private XtraEditors.SpinEdit seMaxZoomPercentX;
		private ChartLabelControl lbMaxZoomPercentX;
		private ChartPanelControl pnlMaxZoomPercentY;
		private XtraEditors.SpinEdit seMaxZoomPercentY;
		private ChartLabelControl lbMaxZoomPercentY;
	}
}
