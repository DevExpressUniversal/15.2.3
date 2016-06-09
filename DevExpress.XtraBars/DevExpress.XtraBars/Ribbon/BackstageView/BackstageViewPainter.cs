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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Skins.XtraForm;
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class BackstageViewItemBasePainter {
		public void Draw(GraphicsCache cache, BackstageViewItemBaseViewInfo itemBase) {
			DrawItemBackground(cache, itemBase);
			BackstageViewItemViewInfo item = itemBase as BackstageViewItemViewInfo;
			if(item != null) {
				DrawItemGlyph(cache, item);
				DrawItemCaption(cache, item);
			}
			BackstageViewItemCustomDrawEventArgs ee = new BackstageViewItemCustomDrawEventArgs(itemBase, cache);
			itemBase.Control.RaiseItemCustomDraw(ee);
			DrawItemSelection(cache, itemBase);
			DrawDropMark(cache, itemBase);
		}
		protected virtual void DrawDropMark(GraphicsCache cache, BackstageViewItemBaseViewInfo item) {
			if(item.ViewInfo.DropItem == item && item.Item.Control.DesignModeCore) {
				DrawDropMark(cache, item.Bounds, item.ViewInfo.DropIndicatorLocation);
			}
		}
		protected virtual void DrawDropMark(GraphicsCache cache, Rectangle bounds, ItemLocation markLocation) {
			Rectangle m = bounds;
			m.Height = 6;
			if(markLocation == ItemLocation.Bottom)
				m.Y = bounds.Bottom - m.Height / 2;
			else
				m.Y = bounds.Y - m.Height / 2;
			cache.FillRectangle(Brushes.Black, new Rectangle(m.X, m.Y, 2, m.Height));
			cache.FillRectangle(Brushes.Black, new Rectangle(m.Right - 2, m.Y, 2, m.Height));
			cache.FillRectangle(Brushes.Black, new Rectangle(m.X, m.Y + 2, m.Width, 2));
		}
		protected virtual void DrawItemSelection(GraphicsCache cache, BackstageViewItemBaseViewInfo item) {
			if(item.ViewInfo.DesignTimeManager.IsComponentSelected(item.Item))
				item.ViewInfo.DesignTimeManager.DrawSelection(cache, item.Bounds, Color.Magenta);
		}
		protected virtual void DrawItemCaption(GraphicsCache cache, BackstageViewItemViewInfo item) {
			if(item.Item.AllowHtmlString)
				DrawHtmlCaption(cache, item);
			else
				DrawSimpleCaption(cache, item);
		}
		protected virtual void DrawSimpleCaption(GraphicsCache cache, BackstageViewItemViewInfo item) {
			item.PaintAppearance.DrawString(cache, item.Item.Caption, item.CaptionBounds);
		}
		protected virtual void DrawHtmlCaption(GraphicsCache cache, BackstageViewItemViewInfo item) {
			if(item.PaintAppearanceCore == null)
				item.CaptionInfo.UpdateAppearanceColors(item.PaintAppearance);
			StringPainter.Default.DrawString(cache, item.CaptionInfo);
		}
		protected virtual void DrawItemGlyph(GraphicsCache cache, BackstageViewItemViewInfo item) {
			Image img = item.GetGlyph();
			if(img == null) return;
			if(item.Item.GetAllowGlyphSkinning()) {
				DevExpress.Utils.Paint.XPaint.Graphics.DrawImage(cache.Graphics, img, item.GlyphBounds, new Rectangle(Point.Empty, item.GlyphBounds.Size), ImageColorizer.GetColoredAttributes(item.PaintAppearance.ForeColor));
			}
			else {
				cache.Graphics.DrawImage(img, item.GlyphBounds);
			}
		}
		protected virtual void DrawItemBackground(GraphicsCache cache, BackstageViewItemBaseViewInfo item) {
			if(item.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) 
				cache.Graphics.FillRectangle(item.PaintAppearance.GetBackBrush(cache, item.Bounds), item.Bounds);
			else ObjectPainter.DrawObject(cache, SkinElementPainter.Default, item.GetItemInfo());
		}
	}
	public class BackstageViewTabItemPainter : BackstageViewItemBasePainter {
		protected override void DrawItemBackground(GraphicsCache cache, BackstageViewItemBaseViewInfo item) {
			base.DrawItemBackground(cache, item);
			if(item.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) return;
			DrawArrow(cache, (BackstageViewTabItemViewInfo)item);
		}
		protected virtual void DrawArrow(GraphicsCache cache, BackstageViewTabItemViewInfo tab) {
			if(!tab.Item.Selected)
				return;
			SkinElementInfo info = tab.GetArrowInfo();
			if(info.Element == null)
				return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
	}
	public class BackstageViewPainter  {
		internal static void DrawColoredSkinElement(GraphicsCache cache, SkinElementInfo info, Image img) {
			if(info.Element == null)
				return;
			Image oldImage = info.Element.Image.Image;
			info.Element.Image.SetImage(img, Color.Empty);
			info.Element.Image.UseOwnImage = true;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			info.Element.Image.SetImage(oldImage, Color.Empty);
			info.Element.Image.UseOwnImage = false;
		}
		public virtual void Draw(GraphicsCache cache, BackstageViewInfo viewInfo) {
			DrawBackground(cache, viewInfo);
			DrawImage(cache, viewInfo, viewInfo.Control);
			DrawLeftPane(cache, viewInfo);
			DrawItems(cache, viewInfo);
		}
		protected virtual void DrawItems(GraphicsCache cache, BackstageViewInfo viewInfo) {
			if(!cache.Graphics.ClipBounds.IntersectsWith(viewInfo.LeftPaneContentBounds)) return;
			GraphicsClipState state = cache.ClipInfo.SaveAndSetClip(viewInfo.LeftPaneContentBounds);
			try {
				foreach(BackstageViewItemBaseViewInfo item in viewInfo.Items) {
					DrawItem(cache, item);
				}
			}
			finally {
				cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
		}
		protected virtual void DrawItem(GraphicsCache cache, BackstageViewItemBaseViewInfo itemBase) {
			itemBase.Painter.Draw(cache, itemBase);
		}
		protected virtual void DrawLeftPane(GraphicsCache cache, BackstageViewInfo viewInfo) {
			if(viewInfo.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) 
				cache.Graphics.FillRectangle(viewInfo.PaintAppearance.GetBackBrush(cache, viewInfo.LeftPaneBounds), viewInfo.LeftPaneBounds);
			else {
				SkinElementInfo info = viewInfo.GetLeftPaneInfo();
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
		}
		protected virtual void DrawBackground(GraphicsCache cache, BackstageViewInfo viewInfo) {
			if(viewInfo.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat)
				return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, viewInfo.GetBackgroundInfo());
		}
		protected virtual void DrawImage(GraphicsCache cache, BackstageViewInfo viewInfo, Control control) {
			if(!viewInfo.Control.ShowImage)
				return;
			SkinElementInfo info = viewInfo.GetBackgroundImage(control);
			if(info == null) {
				if(viewInfo.Control.Image != null) {
					cache.Graphics.DrawImage(viewInfo.Control.Image, viewInfo.GetBackgroundImageBounds(control));	
				}
				return;
			}
			if(viewInfo.Control.Image != null)
				cache.Graphics.DrawImage(viewInfo.Control.Image, info.Bounds);	
			else 
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		public static void DrawBackstageViewImage(GraphicsCache cache, Control control, BackstageViewControl backstageView) {
			backstageView.Painter.DrawImage(cache, backstageView.ViewInfo, control);
		}
		public static void DrawBackstageViewImage(PaintEventArgs e, Control control, BackstageViewControl backstageView) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				DrawBackstageViewImage(cache, control, backstageView);	
			}
		}
		public static Image GetBackgroundImage(BackstageViewControl backstageView, Control control) {
			SkinElementInfo info = backstageView.ViewInfo.GetBackgroundImage(control);
			if(info == null)
				return null;
			return info.Element.Image.Image;
		}
	}
	public class Office2013BackstageViewPainter : BackstageViewPainter {
		public Office2013BackstageViewPainter() {
		}
		public override void Draw(GraphicsCache cache, BackstageViewInfo viewInfo) {
			base.Draw(cache, viewInfo);
			Office2013BackstageViewInfo vi = viewInfo as Office2013BackstageViewInfo;
			DrawBackButton(cache, vi);
			DrawTitle(cache, vi);
			DrawBorders(cache, vi);
			if(vi.ShouldDrawFormButtons) {
				DrawFormButtons(cache, vi);
			}
		}
		protected virtual void DrawBackButton(GraphicsCache cache, Office2013BackstageViewInfo viewInfo) {
			ObjectPainter.DrawObject(cache, viewInfo.BackButtonPainter, viewInfo.GetBackButtonInfo());
		}
		protected virtual void DrawTitle(GraphicsCache cache, Office2013BackstageViewInfo viewInfo) {
			if(!viewInfo.CanDrawTitle(viewInfo))
				return;
			var ribbonViewInfo = viewInfo.Control.Ribbon.ViewInfo;
			var painter = CreateCaptionPainter(Color.Empty);
			Color color = painter.GetTextPart2ForeColor(ribbonViewInfo);
			var font = ribbonViewInfo.PaintAppearance.FormCaption.Font;
			var brush = cache.GetSolidBrush(color);
			var text = ribbonViewInfo.Form.Text;
			using(StringFormat format = new StringFormat()) {
				format.Alignment = StringAlignment.Center;
				format.Trimming = StringTrimming.EllipsisCharacter;
				format.FormatFlags = StringFormatFlags.NoWrap;
				format.LineAlignment = StringAlignment.Center;
				cache.DrawString(text, font, brush, viewInfo.HeaderBounds, format);
			}
		}
		protected virtual Color GetTitleTextColor(Office2013BackstageViewInfo viewInfo) {
			Color color = Color.Empty;
			Skin skin = RibbonSkins.GetSkin(viewInfo.Provider);
			if(skin.Colors.Contains(RibbonSkins.SkinForeColorInBackstageViewTitle))
				color = skin.Colors[RibbonSkins.SkinForeColorInBackstageViewTitle];
			return color;
		}
		protected virtual BackstageViewCaptionPainter CreateCaptionPainter(Color textColor) {
			return new BackstageViewCaptionPainter(textColor);
		}
		protected override void DrawBackground(GraphicsCache cache, BackstageViewInfo vi) {
			Office2013BackstageViewInfo viewInfo = vi as Office2013BackstageViewInfo;
			base.DrawBackground(cache, viewInfo);
			if(viewInfo.ShouldDrawHeaderStrip) cache.FillRectangle(cache.GetSolidBrush(viewInfo.HeaderBackColor), viewInfo.HeaderStripBounds);
		}
		public static readonly int BorderThinkness = 1;
		protected virtual void DrawBorders(GraphicsCache cache, Office2013BackstageViewInfo vi) {
			if(!vi.ShouldDrawBorder)
				return;
			Rectangle excludedClipRect = Rectangle.Inflate(vi.Bounds, -BorderThinkness, -BorderThinkness);
			GraphicsClipState clipState = cache.ClipInfo.SaveClip();
			cache.ClipInfo.ExcludeClip(excludedClipRect);
			try {
				SkinElementInfo elementInfo = new SkinElementInfo(RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinBackstageViewControlLeftPane], vi.Bounds);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
			}
			finally {
				cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		protected virtual void DrawFormButtons(GraphicsCache cache, Office2013BackstageViewInfo vi) {
			if(vi.BackstageViewShowRibbonItems == BackstageViewShowRibbonItems.None || vi.Control.Ribbon == null) return;
			BackstageViewCaptionButtonSkinPainter painter = new BackstageViewCaptionButtonSkinPainter(vi.Provider);
			RibbonForm ribbonForm = vi.Control.Ribbon.ViewInfo.Form;
			if(ribbonForm == null) return;
			var buttons = ribbonForm.FormPainter.Buttons;
			var borders = vi.CalcFormBorderSizes();
			for(var i = 0; i < buttons.Count; i++) {
				if(buttons[i].Kind == FormCaptionButtonKind.FullScreen) continue;
				FormCaptionButton buttonInfo = new FormCaptionButton(buttons[i].Kind);
				int deltaX = vi.IsRightToLeft ? buttons[i].Bounds.X : ribbonForm.Width - buttons[i].Bounds.Right;
				int deltaY = buttons[i].Bounds.Top;
				int locationX = vi.IsRightToLeft ? deltaX + borders : vi.ContentBounds.Right - deltaX - buttons[i].Bounds.Width + borders;
				int locationY = vi.ContentBounds.Top + deltaY;
				buttonInfo.Cache = cache;
				buttonInfo.State = buttons[i].State;				
				buttonInfo.Bounds = new Rectangle(new Point(locationX, locationY), buttons[i].Bounds.Size);
				painter.DrawObject(buttonInfo);
			}
		}
	}
	public class BackstageViewCaptionButtonSkinPainter : RibbonFormCaptionButtonSkinPainter {
		public BackstageViewCaptionButtonSkinPainter(ISkinProvider owner)
			: base(owner) {
		}
	}
	public class BackstageViewCaptionPainter : RibbonCaptionPainter {
		Color textColor;
		public BackstageViewCaptionPainter(Color textColor) {
			this.textColor = textColor;
		}
		protected internal override Color GetTextPart1ForeColor(RibbonViewInfo vi) {
			if(!TextColor.IsEmpty)
				return TextColor;
			return base.GetTextPart1ForeColor(vi);
		}
		protected internal override Color GetTextPart2ForeColor(RibbonViewInfo vi) {
			if(!TextColor.IsEmpty)
				return TextColor;
			return base.GetTextPart2ForeColor(vi);
		}
		public Color TextColor { get { return textColor; } }
	}
	public class BackstageViewBackButtonPainter : ObjectPainter {
		public BackstageViewBackButtonPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			BackstageViewBackButtonInfo ee = e as BackstageViewBackButtonInfo;
			ObjectPainter.DrawObject(ee.Cache, SkinElementPainter.Default, ee.GetInfo());
		}
	}
}
