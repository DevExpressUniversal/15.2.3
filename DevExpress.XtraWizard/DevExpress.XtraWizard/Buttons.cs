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
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using DevExpress.XtraEditors;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.Utils.WXPaint;
namespace DevExpress.XtraWizard {
	[ToolboxItem(false)]
	public class WizardButton : SimpleButton {
		const int DefaultWidth = 75;
		public WizardButton(WizardControl parent) {
			this.Parent = parent;
		}
		WizardControl WizardParent { get { return Parent as WizardControl; } }
		public void SetText(string text) {
			this.Text = text;
			CalcSize();
		}
		void CalcSize() {
			Size size = this.CalcBestSize();
			int defaultMinWidth = WizardParent != null ? WizardParent.GetScaleWidth(DefaultWidth) : DefaultWidth;
			this.Size = new Size(Math.Max(defaultMinWidth, size.Width), size.Height);
		}
	}
	[ToolboxItem(false)]
	public class BackButton : BaseButton {
		WizardControl wizard;
		public BackButton(WizardControl wizard)
			: base() {
			this.wizard = wizard;
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new BackButtonViewInfo(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size MinimumSize { get { return base.MinimumSize; } set { base.MinimumSize = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size MaximumSize { get { return base.MaximumSize; } set { base.MaximumSize = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			if(wizard.ViewInfo.IsAeroEnabled() && !wizard.IsDesignMode)
				e.Graphics.Clear(Color.Transparent);
			base.OnPaint(e);
		}
	}
	public class BackButtonViewInfo : BaseButtonViewInfo {
		public BackButtonViewInfo(BackButton owner)
			: base(owner) {
		}
		protected override void UpdateButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.UpdateButton(buttonInfo);
			buttonInfo.Button.Caption = string.Empty;
			buttonInfo.Button.Image = null;
			buttonInfo.DrawFocusRectangle = false;
		}
		protected override EditorButtonPainter GetButtonPainter() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinBackButtonPainter(LookAndFeel);
			if(NativeVista.IsVista && VistaBackButtonPainter.IsThemeExists()) 
				return new VistaEditorBackButtonPainter();
			return new XPEditorBackButtonPainter();
		}
	}
	public class VistaEditorBackButtonPainter : WindowsXPEditorButtonPainter {
		readonly Size DefaultMinSize = new Size(30, 30);
		protected override ObjectPainter CreateButtonPainter() {
			return new VistaBackButtonPainter();
		}
		protected override Rectangle CalcMinBestSize(ObjectInfoArgs e) {
			Rectangle rect, saveBounds = e.Bounds;
			e.Bounds = new Rectangle(Point.Empty, DefaultMinSize);
			rect = CalcBoundsByClientRectangle(e);
			e.Bounds = saveBounds;
			return rect;
		}
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			base.UpdateButtonInfo(e);
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(!ee.BuiltIn) {
				ee.Transparent = true;
				ee.FillBackground = false;
			}
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			UpdateButtonInfo(e);
			return base.CalcObjectBounds(e);
		}
	}
	public class XPEditorBackButtonPainter : WindowsXPEditorButtonPainter {
		protected override ObjectPainter CreateButtonPainter() {
			return new XPBackButtonPainter();
		}
	}
	public class VistaBackButtonPainter : XPAdvButtonPainter {
		const string ThemeName = "navigation";
		const int PartId = 1; 
		public static bool IsThemeExists() {
			Size size = WXPPainter.Default.GetThemeSize(new WXPPainterArgs(ThemeName, PartId, 0), true);
			return !size.IsEmpty;
		}
		public VistaBackButtonPainter()
			: base() {
		}
		protected override void UpdateThemeNamePart(ObjectInfoArgs e) {
			DrawArgs.ThemeName = ThemeName;
			DrawArgs.PartId = PartId; 
		}
	}
	public class XPBackButtonPainter : XPAdvButtonPainter {
		[ThreadStatic]
		static ImageCollection buttonImage;
		static ImageCollection ButtonImage {
			get {
				if(buttonImage == null) 
					buttonImage = CreateImageCollection();
				return buttonImage;
			}
		}
		static ImageCollection CreateImageCollection() {
			Bitmap image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraWizard.Images.XPBackButton.png", typeof(WizardControl).Assembly);
			ImageCollection images = new ImageCollection();
			images.ImageSize = new Size(30,30);
			images.AddImageStripVertical(image);
			return images;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			int index = CalcImageIndex(e.State);
			e.Paint.DrawImage(e.Graphics, GetImage(index), e.Bounds);
		}
		protected int CalcImageIndex(ObjectState state) {
			ObjectState tempState = state & (~ObjectState.Selected);
			if(tempState == ObjectState.Disabled) return 3;
			if((tempState & ObjectState.Pressed) != 0) return 2; 
			if((tempState & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		Image GetImage(int index) {
			if(!ImageCollection.IsImageListImageExists(ButtonImage, index)) return null;
			return ButtonImage.Images[index];
		}
	}
	public class SkinBackButtonPainter : SkinEditorButtonPainter {
		public SkinBackButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
			return CommonSkins.GetSkin(Provider)[CommonSkins.SkinBackButton];
		}
	}
}
