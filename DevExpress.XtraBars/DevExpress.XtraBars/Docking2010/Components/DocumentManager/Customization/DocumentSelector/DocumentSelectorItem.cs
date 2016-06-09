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
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.Utils.Text;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class DocumentSelectorItemInfo : ObjectInfoArgs {
		public DocumentSelectorItemInfo(string text, Image image) {
			PaintAppearance = new FrozenAppearance();
			Text = text;
			Image = image;
			Index = 0;
			ItemFormat = string.Empty;
			FooterFormat = string.Empty;
			HeaderFormat = string.Empty;
		}
		public string Text { get; private set; }
		public string Header { get; set; }
		public string Footer { get; set; }
		public Image Image { get; private set; }
		public AppearanceObject PaintAppearance { get; private set; }
		public virtual Rectangle ImageBounds { get; set; }
		public Rectangle TextBounds { get; private set; }
		public StringInfo TextInfo { get; private set; }
		public DocumentSelectorAdornerElementInfoArgs Owner { get; internal set; }
		public int Index { get; set; }
		public string ItemFormat { get; set; }
		public string FooterFormat { get; set; }
		public string HeaderFormat { get; set; }
		public bool AllowGlyphSkinning { get; set; }
		public virtual void Calc(GraphicsCache cache, DocumentSelectorItemPainter painter, Point offset) {
			if(painter is DocumentSelectorItemSkinPainter)
				PaintAppearance.Assign(painter.DefaultAppearance);
			PaintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			bool hasText = !string.IsNullOrEmpty(Text);
			bool hasImage = (Image != null);
			Bounds = new Rectangle(offset.X, offset.Y, painter.MaxItemWidth, 100);
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
			string text = string.Format(ItemFormat, Text);
			if(Owner.View != null && Owner.View.DocumentSelectorProperties.CanHtmlDraw) {
				TextInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, PaintAppearance.TextOptions, text, maxTextBounds, cache.Paint, Owner);
				return TextInfo.Bounds.Size;
			}
			return Size.Round(PaintAppearance.CalcTextSize(cache, text, maxTextBounds.Width));
		}
		protected virtual Rectangle CalcImageBounds(Point offset, Size imageSize, int contentHeight) {
			return new Rectangle(offset.X, offset.Y + (contentHeight - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
		}
		protected virtual Rectangle CalcTextBounds(Point offset, Size textSize, int contentHeight, int textOffset) {
			return new Rectangle(offset.X + textOffset, offset.Y + (contentHeight - textSize.Height) / 2, textSize.Width, textSize.Height);
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			return AllowGlyphSkinning && Owner.View.DocumentSelectorProperties.CanUseGlyphSkinning;
		}
	}
	public class DocumentSelectorItemPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorItemInfo info = e as DocumentSelectorItemInfo;
			DrawBackground(info.Cache, info);
			DrawContent(info.Cache, info);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -5, -5);
		}
		protected virtual void DrawBackground(GraphicsCache cache, DocumentSelectorItemInfo info) {
			info.PaintAppearance.DrawBackground(cache, info.Bounds);
		}
		protected virtual void DrawContent(GraphicsCache cache, DocumentSelectorItemInfo info) {
			DrawImage(cache, info);
			DrawText(cache, info);
		}
		protected void DrawImage(GraphicsCache cache, DocumentSelectorItemInfo info) {
			if(info.ImageBounds.Width == 0 && info.ImageBounds.Height == 0) return;
			if(info.GetAllowGlyphSkinning()) {
				var attributes = ImageColorizer.GetColoredAttributes(info.PaintAppearance.GetForeColor());
				cache.Graphics.DrawImage(info.Image, info.ImageBounds, 0, 0, info.Image.Width, info.Image.Height, GraphicsUnit.Pixel, attributes);
			}
			else cache.Graphics.DrawImage(info.Image, info.ImageBounds);
		}
		protected void DrawText(GraphicsCache cache, DocumentSelectorItemInfo info) {
			if(info.TextBounds.IsEmpty) return;
			string text = string.Format(info.ItemFormat, info.Text);
			if(info.Owner.View != null && info.Owner.View.DocumentSelectorProperties.CanHtmlDraw) {
				StringPainter.Default.DrawString(cache, info.PaintAppearance, text, info.TextBounds, info.PaintAppearance.TextOptions, info.Owner);
			}
			else
				cache.DrawString(text, info.PaintAppearance.GetFont(),
					info.PaintAppearance.GetForeBrush(cache), info.TextBounds, info.PaintAppearance.GetStringFormat());
		}
		public int ImageToTextInterval { get { return 5; } }
		public int MaxItemWidth { get { return 150; } }
	}
	public class DocumentSelectorItemSkinPainter : DocumentSelectorItemPainter {
		ISkinProvider providerCore;
		public DocumentSelectorItemSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected Skin GetSkin() {
			return RibbonSkins.GetSkin(providerCore);
		}
		protected SkinElement GetBackground() {
			return GetSkin()[RibbonSkins.SkinGalleryButton];
		}
		protected SkinElement GetGalleryBackground() {
			SkinElement backgroud = DockingSkins.GetSkin(providerCore)[DockingSkins.SkinDocumentSelector];
			return backgroud ?? BarSkins.GetSkin(providerCore)[BarSkins.SkinAlertWindow];
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorItemInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.Bounds);
			elementInfo.State = info.State;
			if((info.State & ObjectState.Hot) != 0)
				elementInfo.ImageIndex = 1;
			if((info.State & ObjectState.Selected) != 0)
				elementInfo.ImageIndex = 4;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), e.Bounds);
			return SkinElementPainter.Default.GetObjectClientRectangle(elementInfo, e.Bounds);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault();
				SkinElement backElement = GetBackground();
				if(backElement.Color.GetForeColor().IsEmpty)
					backElement = GetGalleryBackground();
				backElement.ApplyForeColorAndFont(appearance);
				appearance.ForeColor = DockingSkins.GetSkin(providerCore).Colors.GetColor(DockingSkins.DocumentSelectorForeColor, backElement.Color.GetForeColor()); ;
				return appearance;
			}
		}
	}
	public class DocumentSelectorItem {
		Image imageCore;
		string captionCore;
		string headerCore;
		string footerCore;
		protected DocumentSelectorItem(Image image, string caption,  string header, string footer) {
			imageCore = image;
			captionCore = caption;
			headerCore = header;
			footerCore = footer;
			CaptionFormat = string.Empty;
			FooterFormat = string.Empty;
			HeaderFormat = string.Empty;
		}
		public DocumentSelectorItem(DockPanel panel) :
			this(panel.Image, panel.Text,
			string.IsNullOrEmpty(panel.Header) ? panel.Text : panel.Header, panel.Footer) {
			AllowGlyphSkinning = panel.GetAllowGlyphSkinning();
		}
		public DocumentSelectorItem(BaseDocument document) :
			this(document.Image ?? ImageCollection.GetImageListImage(document.Manager.Images, document.ImageIndex), document.Caption,
			string.IsNullOrEmpty(document.Header) ? document.Caption : document.Header,
			string.IsNullOrEmpty(document.Footer) ? document.Caption : document.Footer) {
			AllowGlyphSkinning = document.Properties.CanUseGlyphSkinning;
		}
		public Image Image {
			get { return imageCore; }
		}
		public string Caption {
			get { return captionCore; }
		}
		public string Header {
			get { return headerCore; }
			set { headerCore = value; }
		}
		public string Footer {
			get { return footerCore; }
			set { footerCore = value; }
		}
		public string CaptionFormat { get; set; }
		public string FooterFormat { get; set; }
		public string HeaderFormat { get; set; }
		public bool AllowGlyphSkinning { get; set; }
	}
}
