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
	partial class ScrollingOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingOptionsControl));
			this.chbUseMouse = new DevExpress.XtraEditors.CheckEdit();
			this.separator1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chbUseKeyboard = new DevExpress.XtraEditors.CheckEdit();
			this.chbUseScrollbars = new DevExpress.XtraEditors.CheckEdit();
			this.separator2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chbUseTouchDevice = new DevExpress.XtraEditors.CheckEdit();
			this.separator3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.chbUseMouse.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboard.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseScrollbars.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseTouchDevice.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator3)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chbUseMouse, "chbUseMouse");
			this.chbUseMouse.Name = "chbUseMouse";
			this.chbUseMouse.Properties.Caption = resources.GetString("chbUseMouse.Properties.Caption");
			this.chbUseMouse.CheckedChanged += new System.EventHandler(this.chbUseMouse_CheckedChanged);
			this.separator1.BackColor = System.Drawing.Color.Transparent;
			this.separator1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator1, "separator1");
			this.separator1.Name = "separator1";
			resources.ApplyResources(this.chbUseKeyboard, "chbUseKeyboard");
			this.chbUseKeyboard.Name = "chbUseKeyboard";
			this.chbUseKeyboard.Properties.Caption = resources.GetString("chbUseKeyboard.Properties.Caption");
			this.chbUseKeyboard.CheckedChanged += new System.EventHandler(this.chbUseKeyboard_CheckedChanged);
			resources.ApplyResources(this.chbUseScrollbars, "chbUseScrollbars");
			this.chbUseScrollbars.Name = "chbUseScrollbars";
			this.chbUseScrollbars.Properties.Caption = resources.GetString("chbUseScrollbars.Properties.Caption");
			this.chbUseScrollbars.CheckedChanged += new System.EventHandler(this.chbUseScrollbars_CheckedChanged);
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
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chbUseScrollbars);
			this.Controls.Add(this.separator3);
			this.Controls.Add(this.chbUseTouchDevice);
			this.Controls.Add(this.separator2);
			this.Controls.Add(this.chbUseMouse);
			this.Controls.Add(this.separator1);
			this.Controls.Add(this.chbUseKeyboard);
			this.Name = "ScrollingOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chbUseMouse.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseKeyboard.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseScrollbars.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbUseTouchDevice.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator3)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chbUseMouse;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl separator1;
		private DevExpress.XtraEditors.CheckEdit chbUseKeyboard;
		private DevExpress.XtraEditors.CheckEdit chbUseScrollbars;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl separator2;
		private DevExpress.XtraEditors.CheckEdit chbUseTouchDevice;
		private ChartPanelControl separator3;
	}
}
