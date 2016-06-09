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
	partial class FrmColorPicker {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmColorPicker));
			DevExpress.XtraEditors.ColorPick.Picker.RGBSuperTiptManager rgbSuperTiptManager2 = new DevExpress.XtraEditors.ColorPick.Picker.RGBSuperTiptManager();
			DevExpress.XtraEditors.ColorPick.Picker.HSBSuperTipManager hsbSuperTipManager2 = new DevExpress.XtraEditors.ColorPick.Picker.HSBSuperTipManager();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.tbctColors = new DevExpress.XtraTab.XtraTabControl();
			this.tabRGB = new DevExpress.XtraTab.XtraTabPage();
			this.rgbModelUserControl1 = new DevExpress.XtraEditors.ColorPick.Picker.RGBModelUserControl();
			this.tabHSB = new DevExpress.XtraTab.XtraTabPage();
			this.hsvModelUserControl1 = new DevExpress.XtraEditors.ColorPick.Picker.HSVModelUserControl();
			this.lblSample = new DevExpress.XtraEditors.LabelControl();
			this.pnlSample = new DevExpress.XtraEditors.ColorPick.Picker.SelectablePanel();
			this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.tbctColors)).BeginInit();
			this.tbctColors.SuspendLayout();
			this.tabRGB.SuspendLayout();
			this.tabHSB.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			resources.ApplyResources(this.tbctColors, "tbctColors");
			this.tbctColors.Name = "tbctColors";
			this.tbctColors.SelectedTabPage = this.tabRGB;
			this.tbctColors.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabRGB,
			this.tabHSB});
			this.tbctColors.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.OnTabControlSelectedPageChanged);
			this.tabRGB.Controls.Add(this.rgbModelUserControl1);
			this.tabRGB.Name = "tabRGB";
			resources.ApplyResources(this.tabRGB, "tabRGB");
			this.rgbModelUserControl1.CursorOn = false;
			resources.ApplyResources(this.rgbModelUserControl1, "rgbModelUserControl1");
			this.rgbModelUserControl1.GradientLabel = "Luminance";
			this.rgbModelUserControl1.GradientValue = 0D;
			this.rgbModelUserControl1.GridXCore = 0;
			this.rgbModelUserControl1.GridXLabel = "Hue";
			this.rgbModelUserControl1.GridYCore = 0;
			this.rgbModelUserControl1.GridYLabel = "Saturation";
			this.rgbModelUserControl1.LastOpacityHover = 0;
			this.rgbModelUserControl1.Name = "rgbModelUserControl1";
			this.rgbModelUserControl1.OpacityCore = 255;
			this.rgbModelUserControl1.SelectedColorEditOption = DevExpress.XtraEditors.ColorPick.Picker.ColorEditOption.Hue;
			this.rgbModelUserControl1.SuperTipManager = rgbSuperTiptManager2;
			this.tabHSB.Controls.Add(this.hsvModelUserControl1);
			this.tabHSB.Name = "tabHSB";
			resources.ApplyResources(this.tabHSB, "tabHSB");
			this.hsvModelUserControl1.CursorOn = false;
			resources.ApplyResources(this.hsvModelUserControl1, "hsvModelUserControl1");
			this.hsvModelUserControl1.GradientLabel = "Hue";
			this.hsvModelUserControl1.GradientValue = 0D;
			this.hsvModelUserControl1.GridXCore = 0;
			this.hsvModelUserControl1.GridXLabel = "Saturation";
			this.hsvModelUserControl1.GridYCore = 256;
			this.hsvModelUserControl1.GridYLabel = "Brightness";
			this.hsvModelUserControl1.LastOpacityHover = 0;
			this.hsvModelUserControl1.Name = "hsvModelUserControl1";
			this.hsvModelUserControl1.OpacityCore = 255;
			this.hsvModelUserControl1.SelectedColorEditOption = DevExpress.XtraEditors.ColorPick.Picker.ColorEditOption.Hue;
			this.hsvModelUserControl1.SuperTipManager = hsbSuperTipManager2;
			resources.ApplyResources(this.lblSample, "lblSample");
			this.lblSample.Name = "lblSample";
			resources.ApplyResources(this.pnlSample, "pnlSample");
			this.pnlSample.ForeColor = System.Drawing.Color.DarkRed;
			this.pnlSample.Name = "pnlSample";
			this.toolTipController.AutoPopDelay = 45000;
			this.toolTipController.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(this.OnToolTipControllerBeforeShow);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.pnlSample);
			this.Controls.Add(this.lblSample);
			this.Controls.Add(this.tbctColors);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmColorPicker";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tbctColors)).EndInit();
			this.tbctColors.ResumeLayout(false);
			this.tabRGB.ResumeLayout(false);
			this.tabHSB.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private SimpleButton btnCancel;
		private SimpleButton btnOK;
		private LabelControl lblSample;
		private SelectablePanel pnlSample;
		private DevExpress.XtraTab.XtraTabControl tbctColors;
		private DevExpress.XtraTab.XtraTabPage tabHSB;
		private DevExpress.XtraTab.XtraTabPage tabRGB;
		private RGBModelUserControl rgbModelUserControl1;
		private HSVModelUserControl hsvModelUserControl1;
		private DevExpress.Utils.ToolTipController toolTipController;
	}
}
