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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using System.Windows.Forms;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab;
using DevExpress.Utils;
using System.Reflection;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.DocumentView;
namespace DevExpress.XtraRichEdit.Painters {
	#region PrintLayoutViewPainter (abstract class)
	public abstract class PrintLayoutViewPainter : RichEditViewPainter, IHeaderFooterDecoratorPainterOwner {
		protected PrintLayoutViewPainter(PrintLayoutView view)
			: base(view) {
			AddDecoratorPainter(new HeaderFooterDecoratorPainter(view, this));
		}
		#region Properties
		public virtual int LeftHeaderFooterLineOffset { get { return 2; } }
		public virtual int RightHeaderFooterLineOffset { get { return 2; } }
		public virtual Color HeaderFooterLineColor { get { return SystemColors.ControlDark; } }
		public virtual Color HeaderFooterMarkBackColor { get { return SystemColors.Control; } }
		public virtual Color HeaderFooterMarkTextColor { get { return SystemColors.ControlText; } }
		#endregion
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
			cache.FillRectangle(GetActualPageBackColor(), page.ClientBounds);
			cache.DrawRectangle(Pens.Black, page.ClientBounds);
			base.DrawEmptyPage(cache, page);
#if (DEBUG)
			int pageIndex = View.FormattingController.PageController.Pages.IndexOf(page.Page);
			RichEditControl control = (RichEditControl)View.Control;
			cache.Graphics.DrawString(String.Format("DBG: PageIndex={0}", pageIndex), control.Font, Brushes.Red, page.Bounds, StringFormat.GenericTypographic);
#endif
		}
		protected internal virtual Color GetActualPageBackColor() {
			DocumentProperties documentProperties = DocumentModel.DocumentProperties;
			Color pageBackColor = documentProperties.PageBackColor;
			if (documentProperties.DisplayBackgroundShape && pageBackColor != DXColor.Empty)
				return pageBackColor;
			return View.ActualBackColor;
		}
	}
	#endregion
	#region PrintLayoutViewFlatPainter
	public class PrintLayoutViewFlatPainter : PrintLayoutViewPainter {
		public PrintLayoutViewFlatPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewUltraFlatPainter
	public class PrintLayoutViewUltraFlatPainter : PrintLayoutViewPainter {
		public PrintLayoutViewUltraFlatPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewStyle3DPainter
	public class PrintLayoutViewStyle3DPainter : PrintLayoutViewPainter {
		public PrintLayoutViewStyle3DPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewOffice2003Painter
	public class PrintLayoutViewOffice2003Painter : PrintLayoutViewPainter {
		public PrintLayoutViewOffice2003Painter(PrintLayoutView view)
			: base(view) {
		}
		public override Color HeaderFooterLineColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
		public override Color HeaderFooterMarkBackColor { get { return Office2003Colors.Default[Office2003Color.Button1]; } }
		public override Color HeaderFooterMarkTextColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText); } }
	}
	#endregion
	#region PrintLayoutViewWindowsXPPainter
	public class PrintLayoutViewWindowsXPPainter : PrintLayoutViewPainter {
		public PrintLayoutViewWindowsXPPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region CommentImage
	public class CommentImage {
		Point offset;
		Image image;
		public CommentImage(Image image, Point offset) {
			this.image = image;
			this.offset = offset;
		}
		public Point Offset { get { return offset; } }
		public Image Image { get { return image; } }
	}
	#endregion
	#region CommentImageCasheKey
	public struct CommentImageCacheKey {
		Size commentBoundsSize;
		Color fillColor;
		public CommentImageCacheKey(Size commentBoundsSize, Color fillColor) {
			this.commentBoundsSize = commentBoundsSize;
			this.fillColor = fillColor;
		}
		public Size CommentBoundsSize { get { return commentBoundsSize; } }
		public Color FillColor { get { return fillColor; } }
		public override bool Equals(object obj) {
			CommentImageCacheKey info = (CommentImageCacheKey)obj;
			return commentBoundsSize == info.CommentBoundsSize && fillColor == info.FillColor;
		}
		public override int GetHashCode() {
			return (commentBoundsSize.Width * commentBoundsSize.Height) + fillColor.A + fillColor.R + fillColor.G + fillColor.B;
		}
		int makeColorAsInt(int alpha, int red, int green, int blue) {
			return alpha << 24 | red << 16 | green << 8 | blue;
		} 
	}
	#endregion
	#region CommentImageCache
	public class CommentImageCache : Dictionary<CommentImageCacheKey, CommentImage> {
		#region Fields
		Dictionary<Color, SkinElement> coloredCommentSkinElementCollection;
		#endregion
		public CommentImageCache() {
			this.coloredCommentSkinElementCollection = new Dictionary<Color, SkinElement>();
		}
		#region Properties
		public Dictionary<Color, SkinElement> ColoredCommentSkinElementCollection { get { return coloredCommentSkinElementCollection; } }
		#endregion
		public void Reset() {
			Clear();
			coloredCommentSkinElementCollection.Clear();
		}
	}
	#endregion
	#region PageImage
	public class PageImage {
		Point offset;
		Image image;
		public PageImage(Image image, Point offset) {
			this.image = image;
			this.offset = offset;
		}
		public Point Offset { get { return offset; } }
		public Image Image { get { return image; } }
	}
	#endregion
	#region PageImageCacheKey
	public struct PageImageCacheKey {
		Size pixelBoundsSize;
		int commentsAreaWidth;
		public PageImageCacheKey(Size pixelBoundsSize, int commentsAreaWidth) {
			this.pixelBoundsSize = pixelBoundsSize;
			this.commentsAreaWidth = commentsAreaWidth;
		}
		public Size PixelBoundsSize { get { return pixelBoundsSize; } }
		public int CommentsAreaWidth { get { return commentsAreaWidth; } }
		public override bool Equals(object obj) {
			PageImageCacheKey info = (PageImageCacheKey)obj;
			return pixelBoundsSize == info.PixelBoundsSize && commentsAreaWidth == info.CommentsAreaWidth;
		}
		public override int GetHashCode() {
			return (PixelBoundsSize.Width + PixelBoundsSize.Height) * CommentsAreaWidth;
		}
	}
	#endregion
	#region PageImageCache
	public class PageImageCache : Dictionary<PageImageCacheKey, PageImage> {
		#region Fields
		SkinElement coloredPageSkinElement;
		SkinElement coloredPageWithCommentSkinElement;
		#endregion
		#region Properties
		public SkinElement ColoredPageSkinElement { get { return coloredPageSkinElement; } set { coloredPageSkinElement = value; } }
		public SkinElement ColoredPageWithCommentSkinElement { get { return coloredPageWithCommentSkinElement; } set { coloredPageWithCommentSkinElement = value; } }
		#endregion
		public void Reset() {
			Clear();
			coloredPageSkinElement = null;
			coloredPageWithCommentSkinElement = null;
		}
	}
	#endregion
	#region PrintLayoutViewSkinPainter
	public class PrintLayoutViewSkinPainter : PrintLayoutViewPainter {
		readonly RichEditViewSkinPainterHelper helper;
		SkinElement moreButtonElement;
		public PrintLayoutViewSkinPainter(PrintLayoutView view)
			: base(view) {
			this.helper = new RichEditViewSkinPainterHelper(view);
		}
		public override Color HeaderFooterLineColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control); } }
		public override Color HeaderFooterMarkBackColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control); } }
		public override Color HeaderFooterMarkTextColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText); } }
		public override int LeftHeaderFooterLineOffset { get { return Math.Max(2, SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Left); } }
		public override int RightHeaderFooterLineOffset { get { return Math.Max(2, SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Right); } }
		protected internal override void DrawEmptyPages(GraphicsCache cache) {
			Graphics gr = cache.Graphics;
			GraphicsState state = gr.Save();
			gr.PageUnit = GraphicsUnit.Pixel;
			gr.PageScale = 1.0f;
			gr.ResetTransform();
			try {
				base.DrawEmptyPages(cache);
			}
			finally {
				gr.Restore(state);
			}
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
#if (DEBUG)
			int pageIndex = View.FormattingController.PageController.Pages.IndexOf(page.Page);
			RichEditControl control = (RichEditControl)View.Control;
			cache.Graphics.DrawString(String.Format("DBG: PageIndex={0}", pageIndex), control.Font, Brushes.Red, page.Bounds, StringFormat.GenericTypographic);
#endif
			int leftOffset = SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Left - 2;
			int rightOffset = SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Right - 2;
			int topOffset = SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Top - 2;
			int bottomOffset = SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Bottom - 2;
			Rectangle rect = helper.CalculateViewInfoPixelBounds(page.ClientBounds);
			Rectangle commentsRect = helper.CalculateViewInfoPixelBounds(page.CommentsBounds);
			const long maxBitmapArea = 1920 * 1200;
			if (View.Control.UseSkinMargins) {
				rect.X -= leftOffset;
				rect.Width += leftOffset + rightOffset;
				rect.Y -= topOffset;
				rect.Height += topOffset + bottomOffset;
			}
			long area = (long)rect.Width * (long)rect.Height; 
			if (area > 0 && area <= maxBitmapArea) { 
				PageImage pageImage = helper.GetPageImage(page.Page, rect, commentsRect.Width);
				Point location = rect.Location;
				location.Offset(pageImage.Offset);
				cache.Graphics.DrawImageUnscaled(pageImage.Image, location);
			}
			else {
				helper.DrawPageImage(cache, rect, commentsRect.Width);
			}
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, CommentViewInfo comment) {
			Rectangle rect = helper.CalculateCommentViewInfoPixelBounds(comment.Bounds);
			const long maxBitmapArea = 1920 * 1200;
			long area = (long)rect.Width * (long)rect.Height;
			if (area > 0 && area <= maxBitmapArea) {
				helper.DrawEmptyComment(cache, comment);
			}
			else {
				DrawCommentImage(cache, comment, rect);
			}
		}
		protected internal virtual void DrawCommentImage(GraphicsCache cache, CommentViewInfo comment, Rectangle commentBounds) {
			Color fillColor = DocumentModel.CommentColorer.GetColor(comment.Comment);
			SkinElementInfo element = helper.CreateCommentSkinElementInfo(commentBounds, RichEditSkins.SkinActiveCommentBorder, fillColor);
			if (element.Element == null)
				element = helper.CreateCommentSkinElementInfo(commentBounds, RichEditSkins.SkinCommentBorder, fillColor);
			if (element.Element != null) {
				helper.DrawCommentImageCore(cache, commentBounds, element); 
			}
			else {
				base.DrawEmptyComment(cache, comment);
			} 
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
			if (moreButtonElement == null) {
				moreButtonElement = helper.GetSkinElement(LookAndFeel, RichEditSkins.SkinCommentMoreButton);
				if (moreButtonElement == null || moreButtonElement.Image == null) {
					base.DrawEmptyExtensionComment(cache, comment);
					return;
				}
			}
			helper.DrawEmptyExtensionComment(cache, comment, moreButtonElement);
		}
		protected internal override void ResetCache() {
			helper.ResetCache();
		}
	}
	#endregion
	#region SkinPaintHelper
	public static class SkinPaintHelper {
		public static readonly string MinVerticalCommentsDistanceProperty = "MinVerticalCommentsDistance";
		public static readonly string CommentLineOffsetX = "CommentLineOffsetX";
		public static readonly string CommentLineOffsetY = "CommentLineOffsetY";
		public static readonly string HideCommentLine = "HideCommentLine";
		public static SkinElement GetSkinElement(ISkinProvider provider, string elementName) {
			Skin skin = PrintingSkins.GetSkin(provider);
			return skin != null ? skin[elementName] : null;
		}
		public static SkinElement GetRichEditSkinElement(ISkinProvider provider, string elementName) {
			Skin skin = RichEditSkins.GetSkin(provider);
			return skin != null ? skin[elementName] : null;
		}
		public static int GetRichEditSkinIntProperty(ISkinProvider provider, string propertyName) {
			Skin skin = RichEditSkins.GetSkin(provider);
			return skin != null ? skin.Properties.GetInteger(propertyName) : 0;
		}
		public static bool GetRichEditSkinBoolProperty(ISkinProvider provider, string propertyName, bool defaultValue) {
			Skin skin = RichEditSkins.GetSkin(provider);
			return skin != null ? skin.Properties.GetBoolean(propertyName, defaultValue) : false;
		}
		public static SkinElementInfo GetSkinElementInfo(ISkinProvider skinProvider, string elementName, Rectangle bounds) {
			return new SkinElementInfo(GetSkinElement(skinProvider, elementName), bounds);
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSkinElementInfo(lookAndFeel, elementName, bounds));
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex) {
			SkinElementInfo skinElInfo = GetSkinElementInfo(lookAndFeel, elementName, bounds);
			skinElInfo.ImageIndex = imageIndex;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinElInfo);
		}
		public static SkinPaddingEdges GetSkinEdges(UserLookAndFeel lookAndFeel, string skinElementName) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			return GetSkinEdges(lookAndFeel, skinEl);
		}
		public static SkinPaddingEdges GetSkinEdges(UserLookAndFeel lookAndFeel, SkinElement skinEl) {
			if (skinEl == null)
				return new SkinPaddingEdges();
			else
				if (skinEl.Image == null)
					return new SkinPaddingEdges();
				else
					return skinEl.Image.SizingMargins;
		}
	}
	#endregion
	#region PrintLayoutViewBackgroundPainter (abstract class)
	public abstract class PrintLayoutViewBackgroundPainter : RichEditViewBackgroundPainter {
		protected PrintLayoutViewBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewFlatBackgroundPainter
	public class PrintLayoutViewFlatBackgroundPainter : PrintLayoutViewBackgroundPainter {
		public PrintLayoutViewFlatBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewUltraFlatBackgroundPainter
	public class PrintLayoutViewUltraFlatBackgroundPainter : PrintLayoutViewBackgroundPainter {
		public PrintLayoutViewUltraFlatBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewStyle3DBackgroundPainter
	public class PrintLayoutViewStyle3DBackgroundPainter : PrintLayoutViewBackgroundPainter {
		public PrintLayoutViewStyle3DBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewOffice2003BackgroundPainter
	public class PrintLayoutViewOffice2003BackgroundPainter : PrintLayoutViewBackgroundPainter {
		public PrintLayoutViewOffice2003BackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewWindowsXPBackgroundPainter
	public class PrintLayoutViewWindowsXPBackgroundPainter : PrintLayoutViewBackgroundPainter {
		public PrintLayoutViewWindowsXPBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region PrintLayoutViewSkinBackgroundPainter
	public class PrintLayoutViewSkinBackgroundPainter : PrintLayoutViewBackgroundPainter {
		Image background;
		public PrintLayoutViewSkinBackgroundPainter(PrintLayoutView view)
			: base(view) {
		}
		public override void Draw(GraphicsCache cache, Rectangle bounds) {
			RichEditControl control = (RichEditControl)View.Control;
			SkinElementInfo skinElInfo = SkinPaintHelper.GetSkinElementInfo(control.LookAndFeel, RichEditPrintingSkins.SkinControlBackground, bounds);
			if (!DXColor.IsEmpty(control.BackColor))
				cache.FillRectangle(LookAndFeelHelper.GetSystemColor(control.LookAndFeel, control.BackColor), bounds);
			else if (skinElInfo.Element.Image != null)
				cache.Graphics.DrawImageUnscaled(GetBackground(bounds, skinElInfo), bounds.Location);
			else
				SkinPaintHelper.DrawSkinElement(control.LookAndFeel, cache, RichEditPrintingSkins.SkinControlBackground, bounds);
		}
		protected Image GetBackground(Rectangle bounds, SkinElementInfo skinElInfo) {
			if (background == null || background.Size != bounds.Size) {
				bounds.X = 0;
				bounds.Y = 0;
				DisposeBackground();
				background = new Bitmap(Math.Max(8, bounds.Width), Math.Max(8, bounds.Height));
				using (Graphics gr = Graphics.FromImage(background)) {
					using (GraphicsCache newCache = new GraphicsCache(gr)) {
						skinElInfo.ImageIndex = 0;
						ObjectPainter.DrawObject(newCache, SkinElementPainter.Default, skinElInfo);
					}
				}
			}
			return background;
		}
		void DisposeBackground() {
			if (background == null)
				return;
			background.Dispose();
			background = null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DisposeBackground();
			}
			base.Dispose(disposing);
		}
	}
	#endregion
}
