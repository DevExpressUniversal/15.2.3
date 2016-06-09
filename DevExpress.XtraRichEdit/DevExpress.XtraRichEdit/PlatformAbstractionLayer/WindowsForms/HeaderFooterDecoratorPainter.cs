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
using System.Windows.Forms;
using DevExpress.Office.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Painters {
	public interface IHeaderFooterDecoratorPainterOwner {
		int LeftHeaderFooterLineOffset { get; }
		int RightHeaderFooterLineOffset { get; }
		Color HeaderFooterLineColor { get; }
		Color HeaderFooterMarkBackColor { get; }
		Color HeaderFooterMarkTextColor { get; }
	}
	public class HeaderFooterDecoratorPainter : IDecoratorPainter, IDisposable {
		#region Fields
		const int HeaderFooterLineThinkness = 2;
		PageViewInfo currentPageInfo;
		Pen headerFooterLinePen;
		Font headerFooterMarkFont;
		const int roundedRectangleRadius = 6;
		PrintLayoutView view;
		readonly RichEditControl control;
		IHeaderFooterDecoratorPainterOwner owner;
		#endregion
		public HeaderFooterDecoratorPainter(PrintLayoutView view, IHeaderFooterDecoratorPainterOwner owner) {
			this.view = view;
			this.owner = owner;
			this.control = (RichEditControl)view.Control;
			this.headerFooterLinePen = CreateHeaderFooterLinePen();
			this.headerFooterMarkFont = new Font("Arial", 8);			
		}
		#region Properties
		protected virtual PrintLayoutView View { get { return view; } }
		protected virtual IHeaderFooterDecoratorPainterOwner Owner { get { return owner; } }
		protected RichEditControl Control { get { return control; } }
		protected DocumentModel DocumentModel { get { return control.DocumentModel; } }
		protected PageViewInfo CurrentPageInfo { get { return currentPageInfo; } }
		#endregion
		public void DrawDecorators(Painter painter, PageViewInfoCollection viewInfos) {		   
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				this.currentPageInfo = viewInfos[i];
				DrawPageDecorators(viewInfos[i].Page, painter);
				this.currentPageInfo = null;
			}
		}
		protected internal virtual void DrawPageDecorators(Page page, Painter painter) {
			if (DocumentModel.ActivePieceTable.IsHeaderFooter) {
				if (page.Header != null)
					DrawHeaderFrame(page, page.Header, painter);
				if (page.Footer != null)
					DrawFooterFrame(page, page.Footer, painter);
			}
		}
		protected internal virtual void DrawHeaderFrame(Page page, PageArea area, Painter painter) {
			Rectangle bounds = page.Bounds;
			bounds.Y = area.Bounds.Bottom;
			bounds = DrawHeaderFooterLine(painter, bounds);
			GdiPlusPainter gdiPlusPainter = (GdiPlusPainter)painter;
			SectionHeaderFooterBase headerFooter = (SectionHeaderFooterBase)area.PieceTable.ContentType;
			string caption = GetCaption(headerFooter, page);
			Rectangle baseBounds = CalculateHeaderCaptionBaseBounds(caption, bounds, gdiPlusPainter);
			DrawHeaderCaptionBackground(baseBounds, gdiPlusPainter);
			DrawCaptionText(caption, baseBounds, gdiPlusPainter, roundedRectangleRadius / 2);
			if (area.Section.Headers.IsLinkedToPrevious(headerFooter.Type)) {
				caption = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_SameAsPrevious);
				baseBounds = CalculateHeaderLinkCaptionBaseBounds(caption, bounds, gdiPlusPainter);
				DrawHeaderCaptionBackground(baseBounds, gdiPlusPainter);
				DrawCaptionText(caption, baseBounds, gdiPlusPainter, roundedRectangleRadius / 2);
			}
		}
		protected internal virtual void DrawFooterFrame(Page page, PageArea area, Painter painter) {
			Rectangle bounds = page.Bounds;
			bounds.Y = area.Bounds.Top;
			bounds = DrawHeaderFooterLine(painter, bounds);
			GdiPlusPainter gdiPlusPainter = (GdiPlusPainter)painter;
			SectionHeaderFooterBase headerFooter = (SectionHeaderFooterBase)area.PieceTable.ContentType;
			string caption = GetCaption(headerFooter, page);
			Rectangle baseBounds = CalculateFooterCaptionBaseBounds(caption, bounds, gdiPlusPainter);
			DrawFooterCaptionBackground(baseBounds, gdiPlusPainter);
			DrawCaptionText(caption, baseBounds, gdiPlusPainter, -roundedRectangleRadius / 2);
			if (area.Section.Footers.IsLinkedToPrevious(headerFooter.Type)) {
				caption = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_SameAsPrevious);
				baseBounds = CalculateFooterLinkCaptionBaseBounds(caption, bounds, gdiPlusPainter);
				DrawFooterCaptionBackground(baseBounds, gdiPlusPainter);
				DrawCaptionText(caption, baseBounds, gdiPlusPainter, -roundedRectangleRadius / 2);
			}
		}
		protected internal virtual Rectangle DrawHeaderFooterLine(Painter painter, Rectangle initialLineBounds) {
			Rectangle bounds = Control.GetPixelPhysicalBounds(CurrentPageInfo, initialLineBounds);
			bounds.Height = 1;
			bounds.X += Owner.LeftHeaderFooterLineOffset;
			bounds.Width -= Owner.LeftHeaderFooterLineOffset + Owner.RightHeaderFooterLineOffset;
			painter.DrawLine(headerFooterLinePen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
			return bounds;
		}
		protected internal string GetCaption(SectionHeaderFooterBase headerFooter, Page page) {
			string caption = headerFooter.GetCaption();
			if (DocumentModel.Sections.Count > 1) {
				SectionIndex sectionIndex = DocumentModel.Sections.IndexOf(page.Areas.First.Section);
				IConvertToInt<SectionIndex> convertToInt = sectionIndex;
				caption = String.Format("{0} -Section {1}-", caption, convertToInt.ToInt() + 1);
			}
			return caption;
		}
		protected internal virtual Size MeasureString(string caption, GdiPlusPainter painter) {
			SizeF size = painter.Cache.CalcTextSize(caption, headerFooterMarkFont, StringFormat.GenericTypographic, Int32.MaxValue);
			return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
		}
		protected internal virtual Rectangle CalculateHeaderCaptionBaseBounds(string caption, Rectangle headerLineBounds, GdiPlusPainter gdiPlusPainter) {
			Rectangle bounds = headerLineBounds;
			bounds.Size = MeasureString(caption, gdiPlusPainter);
			bounds.X += roundedRectangleRadius;
			return bounds;
		}
		protected internal virtual Rectangle CalculateHeaderLinkCaptionBaseBounds(string caption, Rectangle headerLineBounds, GdiPlusPainter gdiPlusPainter) {
			Rectangle bounds = headerLineBounds;
			Size size = MeasureString(caption, gdiPlusPainter);
			bounds.X = bounds.Right - size.Width - roundedRectangleRadius;
			bounds.Size = size;
			return bounds;
		}
		protected internal virtual Rectangle CalculateFooterCaptionBaseBounds(string caption, Rectangle headerLineBounds, GdiPlusPainter gdiPlusPainter) {
			Rectangle bounds = headerLineBounds;
			bounds.Size = MeasureString(caption, gdiPlusPainter);
			bounds.X += roundedRectangleRadius;
			bounds.Y -= bounds.Size.Height;
			return bounds;
		}
		protected internal virtual Rectangle CalculateFooterLinkCaptionBaseBounds(string caption, Rectangle footerLineBounds, GdiPlusPainter gdiPlusPainter) {
			Rectangle bounds = footerLineBounds;
			Size size = MeasureString(caption, gdiPlusPainter);
			bounds.X = bounds.Right - size.Width - roundedRectangleRadius;
			bounds.Size = size;
			bounds.Y -= bounds.Size.Height;
			return bounds;
		}
		protected internal virtual void DrawHeaderCaptionBackground(Rectangle bounds, GdiPlusPainter gdiPlusPainter) {
			using (GraphicsPath path = CreateHeaderCaptionBackgroundPath(bounds)) {
				DrawHeaderFooterCaptionBackground(path, gdiPlusPainter);
			}
		}
		protected internal virtual void DrawFooterCaptionBackground(Rectangle bounds, GdiPlusPainter gdiPlusPainter) {
			using (GraphicsPath path = CreateFooterCaptionBackgroundPath(bounds)) {
				DrawHeaderFooterCaptionBackground(path, gdiPlusPainter);
			}
		}
		protected internal virtual GraphicsPath CreateHeaderCaptionBackgroundPath(Rectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			path.AddLine(bounds.X - roundedRectangleRadius, bounds.Y, bounds.X - roundedRectangleRadius, bounds.Y + bounds.Height);
			path.AddArc(bounds.X - roundedRectangleRadius, bounds.Y + bounds.Height - roundedRectangleRadius, 2 * roundedRectangleRadius, 2 * roundedRectangleRadius, 180, -90);
			path.AddLine(bounds.X + roundedRectangleRadius, bounds.Y + bounds.Height + roundedRectangleRadius, bounds.Right - roundedRectangleRadius, bounds.Y + bounds.Height + roundedRectangleRadius);
			path.AddArc(bounds.Right - roundedRectangleRadius, bounds.Y + bounds.Height - roundedRectangleRadius, 2 * roundedRectangleRadius, 2 * roundedRectangleRadius, 90, -90);
			path.AddLine(bounds.Right + roundedRectangleRadius, bounds.Y + bounds.Height - roundedRectangleRadius, bounds.Right + roundedRectangleRadius, bounds.Y);
			path.AddLine(bounds.Right + roundedRectangleRadius, bounds.Y, bounds.X - roundedRectangleRadius, bounds.Y);
			return path;
		}
		protected internal virtual GraphicsPath CreateFooterCaptionBackgroundPath(Rectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			path.AddLine(bounds.X - roundedRectangleRadius, bounds.Y, bounds.X - roundedRectangleRadius, bounds.Bottom);
			path.AddLine(bounds.X - roundedRectangleRadius, bounds.Bottom, bounds.Right + roundedRectangleRadius, bounds.Bottom);
			path.AddLine(bounds.Right + roundedRectangleRadius, bounds.Bottom, bounds.Right + roundedRectangleRadius, bounds.Y);
			path.AddArc(bounds.Right - roundedRectangleRadius, bounds.Y - roundedRectangleRadius, 2 * roundedRectangleRadius, 2 * roundedRectangleRadius, 0, -90);
			path.AddLine(bounds.Right - roundedRectangleRadius, bounds.Y - roundedRectangleRadius, bounds.X + roundedRectangleRadius, bounds.Y - roundedRectangleRadius);
			path.AddArc(bounds.X - roundedRectangleRadius, bounds.Y - roundedRectangleRadius, 2 * roundedRectangleRadius, 2 * roundedRectangleRadius, -90, -90);
			return path;
		}
		protected internal virtual void DrawHeaderFooterCaptionBackground(GraphicsPath path, GdiPlusPainter gdiPlusPainter) {
			IGraphicsCache cache = gdiPlusPainter.Cache;
			Graphics graphics = cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			try {
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.FillPath(cache.GetSolidBrush(Owner.HeaderFooterMarkBackColor), path);
				graphics.DrawPath(cache.GetPen(Owner.HeaderFooterMarkTextColor), path);
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
		protected internal virtual void DrawCaptionText(string caption, Rectangle bounds, GdiPlusPainter gdiPlusPainter, int offset) {
			bounds.Y += offset;
			IGraphicsCache cache = gdiPlusPainter.Cache;
			cache.DrawString(caption, headerFooterMarkFont, cache.GetSolidBrush(Owner.HeaderFooterMarkTextColor), bounds, StringFormat.GenericTypographic);
		}
		protected internal virtual Pen CreateHeaderFooterLinePen() {
			Pen result = new Pen(Owner.HeaderFooterLineColor, HeaderFooterLineThinkness);
			result.DashStyle = DashStyle.Dash;
			return result;
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (headerFooterLinePen != null) {
					headerFooterLinePen.Dispose();
					headerFooterLinePen = null;
				}
				if (headerFooterMarkFont != null) {
					headerFooterMarkFont.Dispose();
					headerFooterMarkFont = null;
				}
			}
		}
		#endregion
	}
}
