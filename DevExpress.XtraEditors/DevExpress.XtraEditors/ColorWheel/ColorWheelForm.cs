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

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraEditors.ColorWheel {
	public partial class ColorWheelForm : XtraForm {
		public ColorWheelForm() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			LookAndFeel.UseDefaultLookAndFeel = false;
			LookAndFeel.SkinName = "DevExpress Style";
			FormBorderEffect = XtraEditors.FormBorderEffect.Shadow;
			this.skinMaskColor = GetSkinLookAndFeel().SkinMaskColor;
			this.skinMaskColor2 = GetSkinLookAndFeel().SkinMaskColor2;
			this.colorSlider1.SliderImage = this.colorSlider2.SliderImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Resources.Thumb.png", typeof(ColorWheelForm).Assembly);
			this.colorWheel1.CursorImage = this.colorWheel2.CursorImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Resources.Pointer.png", typeof(ColorWheelForm).Assembly);
			this.BackgroundImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Resources.Form.png", typeof(ColorWheelForm).Assembly);
			Size = BackgroundImage.Size;
			UpdateControls();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		protected override ControlHelper CreateHelper() {
			return new ColorWheelFormHelper(this, true);
		}
		protected override void OnShown(EventArgs e) {
			((ColorWheelLookAndFeel)((ISupportLookAndFeel)this).LookAndFeel).SuppressStyleChanged = true;
			base.OnShown(e);
		}
		UserLookAndFeel skinLookAndFeel = null;
		public UserLookAndFeel SkinLookAndFeel {
			get { return skinLookAndFeel; }
			set {
				if(SkinLookAndFeel == value)
					return;
				skinLookAndFeel = value;
				this.skinMaskColor = GetSkinLookAndFeel().SkinMaskColor;
				this.skinMaskColor2 = GetSkinLookAndFeel().SkinMaskColor2;
				UpdateControls();
			}
		}
		protected UserLookAndFeel GetSkinLookAndFeel() {
			return SkinLookAndFeel == null? UserLookAndFeel.Default : SkinLookAndFeel;
		}
		Color skinMaskColor = Color.Empty;
		public Color SkinMaskColor {
			get { return skinMaskColor; }
			set {
				if(SkinMaskColor == value)
					return;
				skinMaskColor = value;
				UpdateControls();
			}
		}
		protected virtual void UpdateControls() {
			this.colorWheel1.Color = SkinMaskColor;
			this.colorWheel2.Color = SkinMaskColor2;
			this.colorSlider1.Value = this.colorWheel1.Brightness;
			this.colorSlider2.Value = this.colorWheel2.Brightness;
			if(CommonSkins.GetSkin(GetSkinLookAndFeel()).ColorizationMode == SkinColorizationMode.Hue ||
				CommonSkins.GetSkin(GetSkinLookAndFeel()).ColorizationMode == SkinColorizationMode.Color) {
				this.colorWheel2.Enabled = false;
				this.colorSlider2.Enabled = false;
			}
			else {
				this.colorWheel2.Enabled = true;
				this.colorWheel2.Enabled = true;
			}
		}
		Color skinMaskColor2 = Color.Empty;
		public Color SkinMaskColor2 {
			get { return skinMaskColor2; }
			set {
				if(SkinMaskColor2 == value)
					return;
				UpdateControls();
			}
		}
		private void lbOk_Click(object sender, EventArgs e) {
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
		private void lbCancel_Click(object sender, EventArgs e) {
			GetSkinLookAndFeel().SetSkinMaskColors(SkinMaskColor, SkinMaskColor2);
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
		private void lbReset_Click(object sender, EventArgs e) {
			this.colorWheel1.Color = Color.Empty;
			this.colorWheel2.Color = Color.Empty;
			this.colorSlider1.Value = 1.0;
			this.colorSlider2.Value = 1.0;
			GetSkinLookAndFeel().ResetSkinMaskColors();
		}
		private void button1_ColorChanged(object sender, EventArgs e) {
			this.colorIndicator1.Color = this.colorWheel1.Color;
			GetSkinLookAndFeel().SkinMaskColor = this.colorWheel1.Color;
		}
		private void colorWheel2_ColorChanged(object sender, EventArgs e) {
			this.colorIndicator2.Color = this.colorWheel2.Color;
			GetSkinLookAndFeel().SkinMaskColor2 = this.colorWheel2.Color;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateFormRegion();
		}
		protected virtual void UpdateFormRegion() {
			Region = NativeMethods.CreateRoundRegion(new Rectangle(0, 0, Bounds.Width, Bounds.Height), 32);
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_NCLBUTTONDBLCLK)
				return;
			base.WndProc(ref m);
			if(m.Msg == MSG.WM_NCHITTEST) {
				m.Result = new IntPtr(DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTCAPTION);
			}
		}
		private void colorSlider1_ValueChanged(object sender, EventArgs e) {
			this.colorWheel1.Brightness = 0.8 + 0.2 * this.colorSlider1.Value;
		}
		private void colorSlider2_ValueChanged(object sender, EventArgs e) {
			this.colorWheel2.Brightness = 0.8 + 0.2 * this.colorSlider2.Value;
		}
	}
	internal class ColorWheelFormHelper : ControlHelper {
		public ColorWheelFormHelper(IDXControl owner, bool isForm) : base(owner, isForm) { }
		protected override UserLookAndFeel CreateLookAndFeel(bool isForm, bool isUserControl) {
			return new ColorWheelLookAndFeel((Form)Owner);
		}
	}
	internal class ColorWheelLookAndFeel : FormUserLookAndFeel {
		public ColorWheelLookAndFeel(Form form) : base(form) { }
		public bool SuppressStyleChanged { get; set; }
		protected override void OnStyleChanged() {
			if(SuppressStyleChanged)
				return;
			base.OnStyleChanged();
		}
	}
}
