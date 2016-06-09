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

namespace DevExpress.XtraEditors.ColorWheel {
	partial class ColorWheelForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorWheelForm));
			this.okImages = new DevExpress.Utils.ImageCollection(this.components);
			this.cancelImages = new DevExpress.Utils.ImageCollection(this.components);
			this.resetImages = new DevExpress.Utils.ImageCollection(this.components);
			this.colorSlider2 = new DevExpress.XtraEditors.ColorWheel.ColorSlider();
			this.colorSlider1 = new DevExpress.XtraEditors.ColorWheel.ColorSlider();
			this.colorIndicator2 = new DevExpress.XtraEditors.ColorWheel.ColorIndicator();
			this.colorIndicator1 = new DevExpress.XtraEditors.ColorWheel.ColorIndicator();
			this.colorWheel1 = new DevExpress.XtraEditors.ColorWheel.ColorWheelControl();
			this.lbReset = new DevExpress.XtraEditors.LabelControl();
			this.lbCancel = new DevExpress.XtraEditors.LabelControl();
			this.lbOk = new DevExpress.XtraEditors.LabelControl();
			this.colorWheel2 = new DevExpress.XtraEditors.ColorWheel.ColorWheelControl();
			((System.ComponentModel.ISupportInitialize)(this.okImages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cancelImages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.resetImages)).BeginInit();
			this.SuspendLayout();
			this.okImages.ImageSize = new System.Drawing.Size(98, 50);
			this.okImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("okImages.ImageStream")));
			this.okImages.Images.SetKeyName(0, "Ok-Normal.png");
			this.okImages.Images.SetKeyName(1, "Ok-Hover.png");
			this.okImages.Images.SetKeyName(2, "Ok-Pressed.png");
			this.cancelImages.ImageSize = new System.Drawing.Size(98, 50);
			this.cancelImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("cancelImages.ImageStream")));
			this.cancelImages.Images.SetKeyName(0, "Cancel-Normal.png");
			this.cancelImages.Images.SetKeyName(1, "Cancel-Hover.png");
			this.cancelImages.Images.SetKeyName(2, "Cancel-Pressed.png");
			this.resetImages.ImageSize = new System.Drawing.Size(98, 50);
			this.resetImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("resetImages.ImageStream")));
			this.resetImages.Images.SetKeyName(0, "Reset-Normal.png");
			this.resetImages.Images.SetKeyName(1, "Reset-Hover.png");
			this.resetImages.Images.SetKeyName(2, "Reset-Pressed.png");
			this.colorSlider2.BackColor = System.Drawing.Color.Transparent;
			this.colorSlider2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.colorSlider2.Location = new System.Drawing.Point(470, 303);
			this.colorSlider2.Name = "colorSlider2";
			this.colorSlider2.Size = new System.Drawing.Size(259, 53);
			this.colorSlider2.TabIndex = 8;
			this.colorSlider2.Text = "colorSlider2";
			this.colorSlider2.Value = 1D;
			this.colorSlider2.ValueChanged += new System.EventHandler(this.colorSlider2_ValueChanged);
			this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
			this.colorSlider1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.colorSlider1.Location = new System.Drawing.Point(26, 303);
			this.colorSlider1.Name = "colorSlider1";
			this.colorSlider1.Size = new System.Drawing.Size(259, 53);
			this.colorSlider1.TabIndex = 7;
			this.colorSlider1.Text = "colorSlider1";
			this.colorSlider1.Value = 1D;
			this.colorSlider1.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
			this.colorIndicator2.BackColor = System.Drawing.Color.Transparent;
			this.colorIndicator2.Color = System.Drawing.Color.Empty;
			this.colorIndicator2.Location = new System.Drawing.Point(697, 50);
			this.colorIndicator2.Name = "colorIndicator2";
			this.colorIndicator2.Size = new System.Drawing.Size(26, 26);
			this.colorIndicator2.TabIndex = 6;
			this.colorIndicator2.Text = "colorIndicator2";
			this.colorIndicator1.BackColor = System.Drawing.Color.Transparent;
			this.colorIndicator1.Color = System.Drawing.Color.Empty;
			this.colorIndicator1.Location = new System.Drawing.Point(31, 50);
			this.colorIndicator1.Name = "colorIndicator1";
			this.colorIndicator1.Size = new System.Drawing.Size(26, 26);
			this.colorIndicator1.TabIndex = 5;
			this.colorIndicator1.Text = "colorIndicator1";
			this.colorWheel1.BackColor = System.Drawing.Color.Transparent;
			this.colorWheel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.colorWheel1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.colorWheel1.Location = new System.Drawing.Point(57, 64);
			this.colorWheel1.Name = "colorWheel1";
			this.colorWheel1.Size = new System.Drawing.Size(195, 195);
			this.colorWheel1.TabIndex = 0;
			this.colorWheel1.Text = "button1";
			this.colorWheel1.ColorChanged += new System.EventHandler(this.button1_ColorChanged);
			this.lbReset.AppearanceHovered.ImageIndex = 1;
			this.lbReset.Appearance.ImageIndex = 0;
			this.lbReset.Appearance.ImageList = this.resetImages;
			this.lbReset.AppearancePressed.ImageIndex = 2;
			this.lbReset.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbReset.Location = new System.Drawing.Point(328, 290);
			this.lbReset.Name = "lbReset";
			this.lbReset.Size = new System.Drawing.Size(98, 50);
			this.lbReset.TabIndex = 3;
			this.lbReset.Click += new System.EventHandler(this.lbReset_Click);
			this.lbCancel.AppearanceHovered.ImageIndex = 1;
			this.lbCancel.Appearance.ImageIndex = 0;
			this.lbCancel.Appearance.ImageList = this.cancelImages;
			this.lbCancel.AppearancePressed.ImageIndex = 2;
			this.lbCancel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbCancel.Location = new System.Drawing.Point(328, 240);
			this.lbCancel.Name = "lbCancel";
			this.lbCancel.Size = new System.Drawing.Size(98, 50);
			this.lbCancel.TabIndex = 2;
			this.lbCancel.Click += new System.EventHandler(this.lbCancel_Click);
			this.lbOk.AppearanceHovered.ImageIndex = 1;
			this.lbOk.Appearance.ImageIndex = 0;
			this.lbOk.Appearance.ImageList = this.okImages;
			this.lbOk.AppearancePressed.ImageIndex = 2;
			this.lbOk.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbOk.Location = new System.Drawing.Point(328, 190);
			this.lbOk.Name = "lbOk";
			this.lbOk.Size = new System.Drawing.Size(98, 50);
			this.lbOk.TabIndex = 1;
			this.lbOk.Click += new System.EventHandler(this.lbOk_Click);
			this.colorWheel2.BackColor = System.Drawing.Color.Transparent;
			this.colorWheel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.colorWheel2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.colorWheel2.Location = new System.Drawing.Point(501, 64);
			this.colorWheel2.Name = "colorWheel2";
			this.colorWheel2.Size = new System.Drawing.Size(195, 195);
			this.colorWheel2.TabIndex = 4;
			this.colorWheel2.Text = "colorWheel1";
			this.colorWheel2.ColorChanged += new System.EventHandler(this.colorWheel2_ColorChanged);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(757, 414);
			this.Controls.Add(this.colorSlider2);
			this.Controls.Add(this.colorSlider1);
			this.Controls.Add(this.colorIndicator2);
			this.Controls.Add(this.colorIndicator1);
			this.Controls.Add(this.colorWheel1);
			this.Controls.Add(this.lbReset);
			this.Controls.Add(this.lbCancel);
			this.Controls.Add(this.lbOk);
			this.Controls.Add(this.colorWheel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ColorWheelForm";
			this.Text = "Form2";
			((System.ComponentModel.ISupportInitialize)(this.okImages)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cancelImages)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.resetImages)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ColorWheelControl colorWheel1;
		private DevExpress.XtraEditors.LabelControl lbOk;
		private DevExpress.XtraEditors.LabelControl lbCancel;
		private DevExpress.XtraEditors.LabelControl lbReset;
		private ColorWheelControl colorWheel2;
		private ColorIndicator colorIndicator1;
		private ColorIndicator colorIndicator2;
		private DevExpress.Utils.ImageCollection okImages;
		private DevExpress.Utils.ImageCollection cancelImages;
		private DevExpress.Utils.ImageCollection resetImages;
		private ColorSlider colorSlider1;
		private ColorSlider colorSlider2;
	}
}
