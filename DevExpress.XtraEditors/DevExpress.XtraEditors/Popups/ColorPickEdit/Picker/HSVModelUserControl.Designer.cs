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

namespace DevExpress.XtraEditors.ColorPick.Picker {
	partial class HSVModelUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			if(disposing) {
				this.UnsubscribeCore();
			}
			FrmColorPicker = null;
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HSVModelUserControl));
			this.pnlColorWheel = new System.Windows.Forms.Panel();
			this.hsbGroup = new DevExpress.XtraEditors.RadioGroup();
			this.label7 = new DevExpress.XtraEditors.LabelControl();
			this.txtHexadecimal = new DevExpress.XtraEditors.TextEdit();
			this.lblOpacityStatic = new DevExpress.XtraEditors.LabelControl();
			this.txtOpacity = new DevExpress.XtraEditors.TextEdit();
			this.pnlOpacity = new DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel();
			this.btnMakeWebSafe = new DevExpress.XtraEditors.SimpleButton();
			this.txtBright = new DevExpress.XtraEditors.TextEdit();
			this.txtSaturation = new DevExpress.XtraEditors.TextEdit();
			this.txtHue = new DevExpress.XtraEditors.TextEdit();
			this.pnlGradient = new DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel();
			this.pnlColorGrid = new DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel();
			this.lblOpacity = new DevExpress.XtraEditors.LabelControl();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.opacityToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
			this.pnlColorWheel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.hsbGroup.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHexadecimal.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOpacity.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtBright.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSaturation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
			this.SuspendLayout();
			this.pnlColorWheel.Controls.Add(this.hsbGroup);
			this.pnlColorWheel.Controls.Add(this.label7);
			this.pnlColorWheel.Controls.Add(this.txtHexadecimal);
			this.pnlColorWheel.Controls.Add(this.lblOpacityStatic);
			this.pnlColorWheel.Controls.Add(this.txtOpacity);
			this.pnlColorWheel.Controls.Add(this.pnlOpacity);
			this.pnlColorWheel.Controls.Add(this.btnMakeWebSafe);
			this.pnlColorWheel.Controls.Add(this.txtBright);
			this.pnlColorWheel.Controls.Add(this.txtSaturation);
			this.pnlColorWheel.Controls.Add(this.txtHue);
			this.pnlColorWheel.Controls.Add(this.pnlGradient);
			this.pnlColorWheel.Controls.Add(this.pnlColorGrid);
			this.pnlColorWheel.Controls.Add(this.lblOpacity);
			resources.ApplyResources(this.pnlColorWheel, "pnlColorWheel");
			this.pnlColorWheel.Name = "pnlColorWheel";
			resources.ApplyResources(this.hsbGroup, "hsbGroup");
			this.hsbGroup.Name = "hsbGroup";
			this.hsbGroup.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("hsbGroup.Properties.Appearance.BackColor")));
			this.hsbGroup.Properties.Appearance.Options.UseBackColor = true;
			this.hsbGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.hsbGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("hsbGroup.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("hsbGroup.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("hsbGroup.Properties.Items2"))});
			this.hsbGroup.SelectedIndexChanged += new System.EventHandler(this.OnHSBSelectedIndexChanged);
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			resources.ApplyResources(this.txtHexadecimal, "txtHexadecimal");
			this.txtHexadecimal.Name = "txtHexadecimal";
			this.txtHexadecimal.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtHexadecimal.Validating += new System.ComponentModel.CancelEventHandler(this.OnHexadecimalValidating);
			this.txtHexadecimal.TextChanged += new System.EventHandler(this.OnHexadecimalTextChanged);
			resources.ApplyResources(this.lblOpacityStatic, "lblOpacityStatic");
			this.lblOpacityStatic.Name = "lblOpacityStatic";
			resources.ApplyResources(this.txtOpacity, "txtOpacity");
			this.txtOpacity.Name = "txtOpacity";
			this.txtOpacity.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtOpacity.Validating += new System.ComponentModel.CancelEventHandler(this.OnOpacityValidating);
			this.txtOpacity.TextChanged += new System.EventHandler(this.OpacityPanelTextChanged);
			this.pnlOpacity.Cursor = System.Windows.Forms.Cursors.VSplit;
			this.pnlOpacity.ForeColor = System.Drawing.Color.DarkRed;
			resources.ApplyResources(this.pnlOpacity, "pnlOpacity");
			this.pnlOpacity.Name = "pnlOpacity";
			this.pnlOpacity.TabStop = true;
			this.pnlOpacity.Paint += new System.Windows.Forms.PaintEventHandler(this.OnOpacityPanelPaint);
			this.pnlOpacity.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnOpacityPanelMouseMove);
			this.pnlOpacity.Leave += new System.EventHandler(this.OnOpacityPanelLeave);
			this.pnlOpacity.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnOpacityPanelMouseDown);
			this.pnlOpacity.Enter += new System.EventHandler(this.OnOpacityPanelEnter);
			this.pnlOpacity.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnOpacityPanelMouseUp);
			resources.ApplyResources(this.btnMakeWebSafe, "btnMakeWebSafe");
			this.btnMakeWebSafe.Name = "btnMakeWebSafe";
			this.btnMakeWebSafe.Click += new System.EventHandler(this.OnMakeWebSafeBtnClick);
			resources.ApplyResources(this.txtBright, "txtBright");
			this.txtBright.Name = "txtBright";
			this.txtBright.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtBright.Validating += new System.ComponentModel.CancelEventHandler(this.OnBrightValidating);
			this.txtBright.TextChanged += new System.EventHandler(this.OnBrightTextChanged);
			resources.ApplyResources(this.txtSaturation, "txtSaturation");
			this.txtSaturation.Name = "txtSaturation";
			this.txtSaturation.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtSaturation.Validating += new System.ComponentModel.CancelEventHandler(this.OnSaturationValidating);
			this.txtSaturation.TextChanged += new System.EventHandler(this.OnSaturationTextChanged);
			resources.ApplyResources(this.txtHue, "txtHue");
			this.txtHue.Name = "txtHue";
			this.txtHue.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtHue.Validating += new System.ComponentModel.CancelEventHandler(this.OnHueValidating);
			this.txtHue.TextChanged += new System.EventHandler(this.OnHueTextChanged);
			this.pnlGradient.Cursor = System.Windows.Forms.Cursors.HSplit;
			resources.ApplyResources(this.pnlGradient, "pnlGradient");
			this.pnlGradient.Name = "pnlGradient";
			this.pnlGradient.TabStop = true;
			this.pnlGradient.Paint += new System.Windows.Forms.PaintEventHandler(this.OnGradientPanelPaint);
			this.pnlGradient.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnGradientPanelMouseMove);
			this.pnlGradient.Leave += new System.EventHandler(this.OnGradientPanelLeave);
			this.pnlGradient.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnGradientPanelMouseDown);
			this.pnlGradient.Enter += new System.EventHandler(this.OnGradientPanelEnter);
			this.pnlGradient.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnGradientPanelMouseUp);
			this.pnlColorGrid.Cursor = System.Windows.Forms.Cursors.Cross;
			resources.ApplyResources(this.pnlColorGrid, "pnlColorGrid");
			this.pnlColorGrid.Name = "pnlColorGrid";
			this.pnlColorGrid.TabStop = true;
			this.pnlColorGrid.DoubleClick += new System.EventHandler(this.OnColorGridDoubleClick);
			this.pnlColorGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.OnColorGridPaint);
			this.pnlColorGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnColorGridMouseMove);
			this.pnlColorGrid.Leave += new System.EventHandler(this.OnColorGridLeave);
			this.pnlColorGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnColorGridMouseDown);
			this.pnlColorGrid.Enter += new System.EventHandler(this.OnColorGridEnter);
			this.pnlColorGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnColorGridMouseUp);
			resources.ApplyResources(this.lblOpacity, "lblOpacity");
			this.lblOpacity.Name = "lblOpacity";
			this.timer.Tick += new System.EventHandler(this.OnTimerTick);
			this.opacityToolTip.AutoPopDelay = 45000;
			this.opacityToolTip.InitialDelay = 500;
			this.opacityToolTip.ReshowDelay = 100;
			this.dxErrorProvider.ContainerControl = this;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlColorWheel);
			this.Name = "HSVModelUserControl";
			this.pnlColorWheel.ResumeLayout(false);
			this.pnlColorWheel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.hsbGroup.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHexadecimal.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOpacity.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtBright.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSaturation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.Panel pnlColorWheel;
		private DevExpress.XtraEditors.RadioGroup hsbGroup;
		private DevExpress.XtraEditors.LabelControl label7;
		private DevExpress.XtraEditors.TextEdit txtHexadecimal;
		private DevExpress.XtraEditors.LabelControl lblOpacityStatic;
		private DevExpress.XtraEditors.TextEdit txtOpacity;
		private DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel pnlOpacity;
		private DevExpress.XtraEditors.SimpleButton btnMakeWebSafe;
		private DevExpress.XtraEditors.TextEdit txtBright;
		private DevExpress.XtraEditors.TextEdit txtSaturation;
		private DevExpress.XtraEditors.TextEdit txtHue;
		private DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel pnlGradient;
		private DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel pnlColorGrid;
		private DevExpress.XtraEditors.LabelControl lblOpacity;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolTip opacityToolTip;
		private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
	}
}
