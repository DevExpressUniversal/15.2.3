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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class DocumentSelectorHeaderInfo : ObjectInfoArgs {
		public DocumentSelectorHeaderInfo(string text, Image image, DocumentSelectorAdornerElementInfoArgs owner) {
			PaintAppearance = new FrozenAppearance();
			Text = text;
			Image = image;
			TextInfo = new StringInfo();
			Owner = owner;
		}
		public string Text { get; private set; }
		public Image Image { get; private set; }
		public AppearanceObject PaintAppearance { get; private set; }
		public DocumentSelectorAdornerElementInfoArgs Owner { get; private set; }
		public Rectangle ImageBounds { get; private set; }
		public Rectangle TextBounds { get; private set; }
		public StringInfo TextInfo { get; set; }
		public bool AllowGlyphSkinning { get; set; }
		public int CalcMinHeight(GraphicsCache cache, DocumentSelectorHeaderPainter painter) {
			PaintAppearance.Assign(painter.DefaultAppearance);
			bool hasText = !string.IsNullOrEmpty(Text);
			bool hasImage = (Image != null);
			int interval = (hasImage && hasText) ? painter.ImageToTextInterval : 0;
			Size imageSize = GetImageSize(hasImage);
			Size textSize = GetTextSize(cache, hasText, Rectangle.Empty);
			Rectangle content = new Rectangle(0, 0, 0, Math.Max(imageSize.Height, textSize.Height));
			return painter.CalcBoundsByClientRectangle(this, content).Height;
		}
		public void Calc(GraphicsCache cache, DocumentSelectorHeaderPainter painter, Point offset, int width) {
			PaintAppearance.Assign(painter.DefaultAppearance);
			PaintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			PaintAppearance.Options.UseTextOptions = true;
			bool hasText = !string.IsNullOrEmpty(Text);
			bool hasImage = (Image != null);
			Bounds = new Rectangle(offset.X, offset.Y, width, 100);
			Rectangle contentRect = painter.GetObjectClientRectangle(this);
			int interval = (hasImage && hasText) ? painter.ImageToTextInterval : 0;
			Size imageSize = GetImageSize(hasImage);
			Size textSize = GetTextSize(cache, hasText, new Rectangle(offset.X + imageSize.Width - interval, offset.Y, contentRect.Width - imageSize.Width - interval, contentRect.Height));
			int contentHeight = Math.Max(imageSize.Height, textSize.Height);
			Bounds = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, contentHeight - (contentRect.Height - 100));
			ImageBounds = CalcImageBounds(contentRect.Location, imageSize, contentHeight);
			TextBounds = CalcTextBounds(contentRect.Location, textSize, contentHeight, imageSize.Width + interval);
		}
		protected virtual Size GetImageSize(bool hasImage) {
			return hasImage ? Image.Size : Size.Empty;
		}
		protected virtual Size GetTextSize(GraphicsCache cache, bool hasText, Rectangle maxTextBounds) {
			if(!hasText) return Size.Empty;
			if(Owner.View != null && Owner.View.DocumentSelectorProperties.CanHtmlDraw) {
				PaintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
				TextInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, PaintAppearance.TextOptions, Text, maxTextBounds, cache.Paint, Owner);
				return TextInfo.Bounds.Size;
			}
			return Size.Round(PaintAppearance.CalcTextSize(cache, Text, maxTextBounds.Width));
		}
		protected virtual Rectangle CalcImageBounds(Point offset, Size imageSize, int contentHeight) {
			return new Rectangle(offset.X, offset.Y + (contentHeight - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
		}
		protected virtual Rectangle CalcTextBounds(Point offset, Size textSize, int contentHeight, int textOffset) {
			return new Rectangle(offset.X + textOffset, offset.Y + (contentHeight - textSize.Height) / 2, textSize.Width, textSize.Height);
		}
	}
	public class DocumentSelectorHeaderPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorHeaderInfo info = e as DocumentSelectorHeaderInfo;
			DrawBackground(info.Cache, info);
			DrawContent(info.Cache, info);
		}
		protected virtual void DrawBackground(GraphicsCache cache, DocumentSelectorHeaderInfo info) {
			DevExpress.Utils.Helpers.PaintHelper.DrawDivider(cache, new Rectangle(info.Bounds.X, info.Bounds.Y + info.Bounds.Height - 1, info.Bounds.Width, 1));
		}
		protected void DrawContent(GraphicsCache cache, DocumentSelectorHeaderInfo info) {
			DrawImage(cache, info);
			DrawText(cache, info);
		}
		protected void DrawImage(GraphicsCache cache, DocumentSelectorHeaderInfo info) {
			if(info.ImageBounds.Width == 0 && info.ImageBounds.Height == 0) return;
			if(info.AllowGlyphSkinning) {
				var attributes = ImageColorizer.GetColoredAttributes(info.PaintAppearance.GetForeColor());
				cache.Graphics.DrawImage(info.Image, info.ImageBounds, 0, 0, info.Image.Width, info.Image.Height, GraphicsUnit.Pixel, attributes);
			}
			else cache.Graphics.DrawImage(info.Image, info.ImageBounds);
		}
		protected void DrawText(GraphicsCache cache, DocumentSelectorHeaderInfo info) {
			if(info.TextBounds.IsEmpty) return;
			if(info.Owner.View != null && info.Owner.View.DocumentSelectorProperties.CanHtmlDraw)
				StringPainter.Default.DrawString(cache, info.PaintAppearance, info.Text, info.TextBounds, info.PaintAppearance.TextOptions, info.Owner);
			else
				cache.DrawString(info.Text, info.PaintAppearance.GetFont(),
					info.PaintAppearance.GetForeBrush(cache), info.TextBounds, info.PaintAppearance.GetStringFormat());
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -5, -5);
		}
		public virtual int ImageToTextInterval { get { return 5; } }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 5, 5);
		}
	}
	public class DocumentSelectorHeaderSkinPainter : DocumentSelectorHeaderPainter {
		ISkinProvider providerCore;
		public DocumentSelectorHeaderSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected virtual SkinElement GetHeader() {
			return DockingSkins.GetSkin(providerCore)[DockingSkins.SkinDocumentSelectorHeader];
		}
		protected virtual SkinElement GetDivider() {
			return CommonSkins.GetSkin(providerCore)[CommonSkins.SkinLabelLine];
		}
		protected virtual SkinElement GetDefaultHeader() {
			return RibbonSkins.GetSkin(providerCore)[RibbonSkins.SkinPopupGallerySizerPanel];
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorHeaderInfo info) {
			SkinElement divider = GetDivider();
			Rectangle bounds = new Rectangle(info.Bounds.X, info.Bounds.Bottom - divider.Size.MinSize.Height,
				info.Bounds.Width, divider.Size.MinSize.Height);
			DevExpress.Utils.Helpers.PaintHelper.DrawSkinnedDivider(cache, new SkinElementInfo(divider, bounds));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement header = GetHeader();
			if(header != null) {
				SkinElementInfo elementInfo = new SkinElementInfo(header, client);
				return SkinElementPainter.Default.CalcBoundsByClientRectangle(elementInfo, client);
			}
			return new Rectangle(client.Left - 12, client.Top - 8, client.Width + 12 + 12, client.Height + 8 + 12);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinElement header = GetHeader();
			if(header != null) {
				SkinElementInfo elementInfo = new SkinElementInfo(header, e.Bounds);
				return SkinElementPainter.Default.GetObjectClientRectangle(elementInfo, e.Bounds);
			}
			return new Rectangle(e.Bounds.Left + 12, e.Bounds.Top + 8, e.Bounds.Width - 12 - 12, e.Bounds.Height - 8 - 12);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, Color.Empty);
				(GetHeader() ?? GetDefaultHeader()).ApplyForeColorAndFont(appearance);
				return appearance;
			}
		}
	}
}
