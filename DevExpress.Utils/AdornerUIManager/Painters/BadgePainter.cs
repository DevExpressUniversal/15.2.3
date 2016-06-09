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

using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.Skins;
using System.Drawing.Text;
namespace DevExpress.Utils.VisualEffects {
	public class BadgePainter : AdornerElementPainter<BadgeViewInfo> {
		protected override void DrawBackground(BadgeViewInfo e) { }
		protected override void DrawContent(BadgeViewInfo e) {
			if(e.AllowGlyphSkinning)
				DrawImageColorized(e);
			else
				DrawImage(e, e.Image);
			DrawText(e);
		}
		protected virtual void DrawImage(BadgeViewInfo e, Image image) {
			if(image == null) return;
			if(!e.CanStretchImage) {
				e.Graphics.DrawImage(image, e.ImageBounds);
				return;
			}
			using(Bitmap bmp = new Bitmap(e.ImageBounds.Width, e.ImageBounds.Height)) {
				using(Graphics g = Graphics.FromImage(bmp))
					DevExpress.Utils.Helpers.PaintHelper.DrawImage(g, image, new Rectangle(Point.Empty, e.ImageBounds.Size), e.ImageStretchMargins);
				e.Graphics.DrawImageUnscaled(bmp, e.ImageBounds.Location);
			}
		}
		protected Bitmap GetImageColorized(Image image, Color color) {
			return DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, color) as Bitmap;
		}
		protected virtual void DrawImageColorized(BadgeViewInfo e) {
			if(e.Image == null) return;
			using(Bitmap bmp = GetImageColorized(e.Image, e.PaintAppearance.GetBackColor()))
				DrawImage(e, bmp);
		}
		protected virtual void DrawText(BadgeViewInfo e) {
			if(e.TextBounds.IsEmpty) return;
			Graphics g = e.Graphics;
			TextRenderingHint oldHint = g.TextRenderingHint;
			if(e.Image == null)
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
			Font font = e.PaintAppearance.GetFont();
			Brush brush = e.PaintAppearance.GetForeBrush(e.Cache);
			StringFormat format = e.PaintAppearance.GetStringFormat();
			g.DrawString(e.Text, font, brush, e.TextBounds, format);
			g.TextRenderingHint = oldHint;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault defaultApp = new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight);
			defaultApp.HAlignment = HorzAlignment.Center;
			return defaultApp;
		}
	}
	public class BadgeSkinPainter : BadgePainter {
		ISkinProvider provider;
		public BadgeSkinPainter(ISkinProvider provider) { this.provider = provider; }
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = base.CreateDefaultAppearance();
			appearance.ForeColor = GetForeColor();
			appearance.BackColor = CommonSkins.GetSkin(provider).Colors.GetColor("Critical", SystemColors.Highlight);
			return appearance;
		}
		Color GetForeColor() {
			var elem = NavPaneSkins.GetSkin(provider)["TileNavPane"];
			if(elem != null && elem.Properties.ContainsProperty("ForeColor"))
				return elem.Properties.GetColor("ForeColor");
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		SkinElement GetDefaultSkinElement(string elementName) {
			SkinElement elem = MetroUISkins.GetSkin(provider)[elementName];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)[elementName];
			return elem;
		}
	}
}
