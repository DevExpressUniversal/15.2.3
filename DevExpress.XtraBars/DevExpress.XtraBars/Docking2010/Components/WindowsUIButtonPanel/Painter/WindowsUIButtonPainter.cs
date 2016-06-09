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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using BaseHeaderButtonPreferredConstructor = DevExpress.XtraEditors.Controls.EditorButtonPreferredConstructorAttribute;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.Skins;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUIButtonSkinPainter : ActionsBarButtonSkinPainter {
		WindowsUIButtonInfo infoCore;
		public WindowsUIButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		new WindowsUIButtonInfo Info {
			get { return infoCore; }
		}
		public override void DrawObject(ObjectInfoArgs e) {
			infoCore = e as WindowsUIButtonInfo;
			UpdatePaintAppearance();
			base.DrawObject(e);
		}
		protected override SkinElement GetActionsBarButtonSkinElement() {
			return GetSkin()[MetroUISkins.SkinActionsBarButton];
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		protected override Color GetPressedColor(Color defaultColor) {
			if(!defaultColor.IsEmpty) return defaultColor;
			return GetSkin().Colors.GetColor(MetroUISkins.ActionsBarButtonPressedColor, defaultColor);
		}
		protected override Color GetHotColor(Color defaultColor) {
			if(!defaultColor.IsEmpty) return defaultColor;
			return GetSkin().Colors.GetColor(MetroUISkins.ActionsBarButtonHotColor, defaultColor);
		}
		protected override Color GetInvertedColor() {
			Color color = Color.Empty;
			if(PaintAppearance != null && !PaintAppearance.BackColor.IsEmpty)
				return PaintAppearance.BackColor;
			SkinElement element = GetActionsBarButtonSkinElement();
			if(element != null)
				color = element.Color.GetBackColor();
			return color;
		}		
		AppearanceDefault defaultAppearanceCore;
		public override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearanceCore == null)
					defaultAppearanceCore = new AppearanceDefault(GetColor(), GetInvertedColor(), WindowsUIViewPainter.DefaultFont);
				return defaultAppearanceCore;
			}
		}
		protected override void UpdatePaintAppearance() {
			if(Info == null) return;
			paintAppearanceCore = new FrozenAppearance();
			if(Info.ParentControl != null) {
				PaintAppearance.ForeColor = Info.ParentControl.ForeColor;
				PaintAppearance.BackColor = GetParentBackColor();
			}
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] {Info.StateAppearances, Info.Button.Properties.Appearance}, DefaultAppearance);
		}
		protected Color GetParentBackColor() {
			for(Control parentControl = Info.ParentControl; parentControl != null; parentControl = parentControl.Parent) {
				if(parentControl.BackColor != Color.Transparent)
					return parentControl.BackColor;
			}
			return Color.Transparent;
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage && info.Button.Properties.UseImage) {
				Image image = GetActualImage(info);
				if(image != null) {
					if(CanDrawTransparentImage(info))
						using(Image transparentImage = (Image)GetActualImage(info).Clone()) {
							DrawImageCore(cache, info, ImageHelper.MakeTransparent(transparentImage));
						}
					else DrawImageCore(cache, info, image);
				}
			}
		}
		protected bool CanDrawTransparentImage(BaseButtonInfo info) {
			return info.Button is WindowsUIButton && ((WindowsUIButton)info.Button).EnableImageTransparency ||
				(Info != null && Info.ButtonPanelOwner != null && Info.ButtonPanelOwner.EnableImageTransparency);
		}
		protected override Color GetColor() {
			Color color = Color.Empty;
			if(PaintAppearance != null && !PaintAppearance.ForeColor.IsEmpty)
				return PaintAppearance.ForeColor;
			SkinElement element = GetActionsBarButtonSkinElement();
			if(element != null)
				color = element.Color.GetForeColor();
			return color;
		}
		protected void DrawImageCore(GraphicsCache cache, BaseButtonInfo info, Image image) {
			Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
			Rectangle destRect = PlacementHelper.Arrange(image.Size, info.ImageBounds, ContentAlignment.MiddleCenter);
			if(Info.ButtonPanelOwner != null && !Info.ButtonPanelOwner.AllowGlyphSkinning) {
				cache.Paint.DrawImage(info.Graphics, image,  destRect, srcRect, !info.Disabled);
				return;
			}
			int stateIndex = GetStateIndex(info);
			Color bgColor = GetForegroundColor(stateIndex);
			using(Image coloredImage = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, bgColor)) {
				if(info.Disabled)
					cache.Paint.DrawImage(info.Graphics, coloredImage, destRect, srcRect, DevExpress.Utils.Helpers.ColoredImageHelper.DisabledImageAttr);
				else
					cache.Graphics.DrawImage(coloredImage, destRect, srcRect, GraphicsUnit.Pixel);
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
	}
}
