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
	public class DocumentSelectorFooterInfo : ObjectInfoArgs {
		public DocumentSelectorFooterInfo(string text, DocumentSelectorAdornerElementInfoArgs owner) {
			Text = text;
			PaintAppearance = new FrozenAppearance();
			TextInfo = new StringInfo(); 
			Owner = owner;
		}
		public string Text { get; private set; }
		public DocumentSelectorAdornerElementInfoArgs Owner { get; private set; }
		public StringInfo TextInfo { get; set; }
		public AppearanceObject PaintAppearance { get; private set; }
		public Rectangle TextBounds { get; private set; }
		public void Calc(GraphicsCache cache, DocumentSelectorFooterPainter painter, Point offset, int width) {
			PaintAppearance.Assign(painter.DefaultAppearance);
			PaintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			PaintAppearance.Options.UseTextOptions = true;
			bool hasText = !string.IsNullOrEmpty(Text);
			Bounds = new Rectangle(offset.X, offset.Y, width, 100);
			Rectangle contentRect = painter.GetObjectClientRectangle(this);
			Size textSize = GetTextSize(cache, hasText, contentRect);
			int contentHeight = Math.Max(painter.MinContentHeight, textSize.Height);
			Bounds = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, contentHeight - (contentRect.Height - 100));
			TextBounds = CalcTextBounds(contentRect.Location, textSize, contentHeight);
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
		protected virtual Rectangle CalcTextBounds(Point offset, Size textSize, int contentHeight) {
			return new Rectangle(offset.X, offset.Y + (contentHeight - textSize.Height) / 2, textSize.Width, textSize.Height);
		}
	}
	public class DocumentSelectorFooterPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorFooterInfo info = e as DocumentSelectorFooterInfo;
			DrawBackground(info.Cache, info);
			DrawContent(info.Cache, info);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -5, -5);
		}
		protected virtual void DrawBackground(GraphicsCache cache, DocumentSelectorFooterInfo info) {
			DevExpress.Utils.Helpers.PaintHelper.DrawDivider(cache, new Rectangle(info.Bounds.X, info.Bounds.Y - 1, info.Bounds.Width, 1));
		}
		protected void DrawContent(GraphicsCache cache, DocumentSelectorFooterInfo info) {
			DrawText(cache, info);
		}
		protected void DrawText(GraphicsCache cache, DocumentSelectorFooterInfo info) {
			if(info.TextBounds.IsEmpty) return;
			if(info.Owner.View != null && info.Owner.View.DocumentSelectorProperties.CanHtmlDraw) {
				StringPainter.Default.DrawString(cache, info.PaintAppearance, info.Text, info.TextBounds, info.PaintAppearance.TextOptions, info.Owner);
			}
			else
				cache.DrawString(info.Text, info.PaintAppearance.GetFont(),
					info.PaintAppearance.GetForeBrush(cache), info.TextBounds, info.PaintAppearance.GetStringFormat());
		}
		public virtual int MinContentHeight { get { return 24; } }
	}
	public class DocumentSelectorFooterSkinPainter : DocumentSelectorFooterPainter {
		ISkinProvider providerCore;
		public DocumentSelectorFooterSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected virtual SkinElement GetFooter() {
			return DockingSkins.GetSkin(providerCore)[DockingSkins.SkinDocumentSelectorFooter];
		}
		protected virtual SkinElement GetDivider() {
			return CommonSkins.GetSkin(providerCore)[CommonSkins.SkinLabelLine];
		}
		protected virtual SkinElement GetDefaultFooter() {
			return RibbonSkins.GetSkin(providerCore)[RibbonSkins.SkinPopupGallerySizerPanel];
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorFooterInfo info) {
			SkinElement divider = GetDivider();
			Rectangle bounds = new Rectangle(info.Bounds.X, info.Bounds.Top,
				info.Bounds.Width, divider.Size.MinSize.Height);
			DevExpress.Utils.Helpers.PaintHelper.DrawSkinnedDivider(cache, new SkinElementInfo(divider, bounds));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement footer = GetFooter();
			if(footer != null) {
				SkinElementInfo elementInfo = new SkinElementInfo(footer, client);
				return SkinElementPainter.Default.CalcBoundsByClientRectangle(elementInfo, client);
			}
			return new Rectangle(client.Left - 12, client.Top - 8, client.Width + 12 + 12, client.Height + 8 + 4);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinElement footer = GetFooter();
			if(footer != null) {
				SkinElementInfo elementInfo = new SkinElementInfo(footer, e.Bounds);
				return SkinElementPainter.Default.GetObjectClientRectangle(elementInfo, e.Bounds);
			}
			return new Rectangle(e.Bounds.Left + 12, e.Bounds.Top + 8, e.Bounds.Width - 12 - 12, e.Bounds.Height - 8 - 4);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, Color.Empty);
				(GetFooter() ?? GetDefaultFooter()).ApplyForeColorAndFont(appearance);
				return appearance;
			}
		}
		public override int MinContentHeight {
			get {
				SkinElement footer = GetFooter();
				if(footer != null)
					return footer.Size.MinSize.Height;
				return base.MinContentHeight;
			}
		}
	}
}
