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
using System.Drawing.Imaging;
using System.Drawing.Text;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Ribbon {
	public class RadialMenuPainter {
		RadialMenuViewInfo viewInfo;
		public RadialMenuPainter(RadialMenuViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public RadialMenuViewInfo ViewInfo { get { return viewInfo; } }
		public void Draw(RadialMenuGraphicsInfoArgs e) {
			e.Graphics.Clear(Color.Transparent);
			if(IsContentChangeAnimation(e)) {
				DrawContentChangeAnimation(e);
			}
			else if(e.ViewInfo.MenuState == RadialMenuState.Collapsed) {
				DrawCollapsed(e);
			}
			else {
				DrawExpanded(e);
			}
		}
		protected virtual void DrawContentChangeAnimation(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo info = GetContentChangeAnimation(e);
			if(info.State == RadialMenuContentChangeState.HidePrevContent ||
				info.State == RadialMenuContentChangeState.ShowNextContent) {
				DrawBackground(e);
				DrawLinksAdorments(e);
				if(info.State == RadialMenuContentChangeState.HidePrevContent)
					DrawImage(e, e.ViewInfo.RenderImage, info.PrevContentBounds, info.PrevContentOpacity);
				else
					DrawImage(e, e.ViewInfo.NextRenderImage, info.NextContentBounds, info.NextContentOpacity);
				DrawGlyph(e);
			} else if(info.State == RadialMenuContentChangeState.HidePrevBounds ||
				info.State == RadialMenuContentChangeState.ShowNextBounds) {
				DrawBackground(e);
				DrawLinksAdorments(e);
				DrawGlyph(e);
			}
		}
		static ColorMatrix OpacityColorMatrix;
		static float[][] Matrix;
		void DrawImage(RadialMenuGraphicsInfoArgs e, Image image, Rectangle bounds, float opacity) {
			using(ImageAttributes attr = new ImageAttributes()) {
				attr.SetColorMatrix(GetOpacityMatrix(opacity));
				e.Graphics.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
			}
		}
		private ColorMatrix GetOpacityMatrix(float opacity) {
			if(OpacityColorMatrix == null) {
				Matrix = new float[][] { 
					new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
				};
			}
			Matrix[3][3] = opacity;
			OpacityColorMatrix = new ColorMatrix(Matrix);
			return OpacityColorMatrix;
		}
		protected RadialMenuContentChangeAnimationInfo GetContentChangeAnimation(RadialMenuGraphicsInfoArgs e) {
			return XtraAnimator.Current.Get(e.ViewInfo.Window, RadialMenuViewInfo.ContentChangeAnimationId) as RadialMenuContentChangeAnimationInfo;
		}
		protected bool IsContentChangeAnimation(RadialMenuGraphicsInfoArgs e) {
			return GetContentChangeAnimation(e) != null;
		}
		protected Rectangle GetMenuInnerBounds(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo info = GetContentChangeAnimation(e);
			Rectangle innerRect = e.ViewInfo.MenuInnerBounds;
			if(info != null && (info.State == RadialMenuContentChangeState.HidePrevBounds || info.State == RadialMenuContentChangeState.HidePrevContent || info.State == RadialMenuContentChangeState.ShowNextBounds))
				innerRect = info.LinkBoundsInnerRect;
			return innerRect;
		}
		protected Rectangle GetClientBackgroundBounds(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo info = GetContentChangeAnimation(e);
			Rectangle innerRect = e.ViewInfo.MenuInnerBounds;
			if(info != null)
				innerRect = info.ClientBackgroundBounds;
			return innerRect;
		}
		protected virtual void DrawBackground(RadialMenuGraphicsInfoArgs e) {
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			try {
				if(e.DrawOptions.AllowDrawLinkBound) {
					e.Graphics.FillEllipse(e.Cache.GetSolidBrush(e.ViewInfo.BorderColor), e.ViewInfo.Bounds);
				}
				e.Graphics.FillEllipse(e.Cache.GetSolidBrush(e.ViewInfo.BackColor), GetClientBackgroundBounds(e));
			} finally {
				e.Graphics.SmoothingMode = mode;
			}
		}
		protected virtual void DrawGlyphSelection(RadialMenuGraphicsInfoArgs e) {
			FloatAnimationInfo info = XtraAnimator.Current.Get(e.ViewInfo.Window, RadialMenuViewInfo.GlyphAnimationId) as FloatAnimationInfo;
			if((e.ViewInfo.HoverInfo.HitTest != RadialMenuHitTest.Glyph) && info == null)
				return;
			Color selectionColor = info != null ? Color.FromArgb((int)(info.Value * 255), e.ViewInfo.MenuColor) : e.ViewInfo.MenuColor;
			e.Graphics.DrawEllipse(e.Cache.GetPen(selectionColor, 3), e.ViewInfo.GlyphSelectionBounds);
		}
		protected virtual void DrawGlyph(RadialMenuGraphicsInfoArgs e) {
			if(e.ViewInfo.GlyphBounds.IsEmpty || !e.DrawOptions.AllowDrawGlyph)
				return;
			Image glyph = GetActualGlyph(e);
			if(glyph == RadialMenuViewInfo.BackArrowIcon)
				DrawColoredGlyph(e, glyph);
			else
				e.Graphics.DrawImage(glyph, e.ViewInfo.CalcGlyphClientBounds(e.ViewInfo.GlyphBounds, glyph));
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			try {
				e.Graphics.DrawEllipse(e.Cache.GetPen(e.ViewInfo.MenuColor, 3), e.ViewInfo.GlyphBounds);
				DrawGlyphSelection(e);
			} finally {
				e.Graphics.SmoothingMode = mode;
			}
		}
		protected virtual void DrawColoredGlyph(RadialMenuGraphicsInfoArgs e, Image glyph) {
			using(ImageAttributes attr = new ImageAttributes()) {
				attr.SetColorMatrix(GetColorMatrix(e.ViewInfo.MenuColor));
				e.Graphics.DrawImage(glyph, e.ViewInfo.CalcGlyphClientBounds(e.ViewInfo.GlyphBounds, glyph), 0, 0, glyph.Width, glyph.Height, GraphicsUnit.Pixel, attr);
			}
		}
		static float[][] ColorMatrix;
		protected virtual ColorMatrix GetColorMatrix(Color color) {
			if(ColorMatrix == null) {
				ColorMatrix = new float[][] { 
					new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
				};
			}
			ColorMatrix[0][0] = color.R / 255.0f;
			ColorMatrix[1][1] = color.G / 255.0f;
			ColorMatrix[2][2] = color.B / 255.0f;
			return new ColorMatrix(ColorMatrix);
		}
		protected virtual Image GetActualGlyph(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo info = GetContentChangeAnimation(e);
			if(info == null) {
				return e.ViewInfo.Menu.ActualLinksHolder == e.ViewInfo.Menu ? e.ViewInfo.Glyph : RadialMenuViewInfo.BackArrowIcon;
			} else {
				if(e.ViewInfo.Menu.LinksHolderList.Count > 1 || e.ViewInfo.Menu.ActualLinksHolder != e.ViewInfo.Menu)
					return RadialMenuViewInfo.BackArrowIcon;
				if(info.State == RadialMenuContentChangeState.HidePrevContent || info.State == RadialMenuContentChangeState.HidePrevBounds)
					return e.ViewInfo.Menu.ActualLinksHolder == e.ViewInfo.Menu ? RadialMenuViewInfo.BackArrowIcon : e.ViewInfo.Glyph;
				else
					return e.ViewInfo.Menu.ActualLinksHolder == e.ViewInfo.Menu ? e.ViewInfo.Glyph : RadialMenuViewInfo.BackArrowIcon;
			}
		}
		protected virtual void DrawCollapsed(RadialMenuGraphicsInfoArgs e) {
			DrawBackground(e);
			if(IsInExpandCollapseAnimation(e.ViewInfo)) {
				DrawAnimatedContent(e);
			}
			DrawGlyph(e);
		}
		bool IsInExpandCollapseAnimation(RadialMenuViewInfo vi) {
			return XtraAnimator.Current.Get(vi.Window, RadialMenuWindow.BoundAnimationId) != null;
		}
		protected virtual void DrawExpanded(RadialMenuGraphicsInfoArgs e) {
			DrawBackground(e);
			if(IsInExpandCollapseAnimation(e.ViewInfo))
				DrawAnimatedContent(e);
			else {
				DrawLinksAdorments(e);
				DrawLinksContent(e);
			}
			DrawGlyph(e);
		}
		protected virtual void DrawAnimatedContent(RadialMenuGraphicsInfoArgs e) {
			if(e.ViewInfo.RenderImage == null)
				return;
			Point upperLeft = new Point(e.ViewInfo.Bounds.X, e.ViewInfo.Bounds.Y);
			Point upperRight = new Point(e.ViewInfo.Bounds.Right, e.ViewInfo.Bounds.Y);
			Point lowerLeft = new Point(e.ViewInfo.Bounds.X, e.ViewInfo.Bounds.Bottom);
			float angle = -(float)(e.ViewInfo.GetOffsetAngle() * Math.PI / 180);
			PointF[] points = new PointF[] { e.ViewInfo.RotatePoint(upperLeft, angle), e.ViewInfo.RotatePoint(upperRight, angle), e.ViewInfo.RotatePoint(lowerLeft, angle) };
			e.Graphics.DrawImage(e.ViewInfo.RenderImage, points);
		}
		protected virtual void DrawLinksContent(RadialMenuGraphicsInfoArgs e) {
			foreach(BarLinkViewInfo linkInfo in e.ViewInfo.LinksInfo) {
				DrawLink(e, linkInfo);
			}
		}
		protected virtual void DrawLinkBackground(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			if(!ViewInfo.ShouldDrawLinkBackground) return;
			BarItem item = linkInfo.Link.Item;
			SmoothingMode prev = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			try {
				if(e.ViewInfo.ShouldDrawLinkBackgroundCustomColor(linkInfo)) {
					e.Graphics.FillPie(e.Cache.GetSolidBrush(GetLinkBackgroundColor(e, linkInfo)), ViewInfo.ContentBounds, e.ViewInfo.GetStartAngle(linkInfo), e.ViewInfo.GetSweepAngle(linkInfo));
				}
				e.Graphics.FillEllipse(e.Cache.GetSolidBrush(e.ViewInfo.BackColor), ViewInfo.FreeSpaceBounds);
			}
			finally {
				e.Graphics.SmoothingMode = prev;
			}
		}
		protected virtual Color GetLinkBackgroundColor(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			ColorAnimationInfo cinfo = XtraAnimator.Current.Get(ViewInfo.Window, new LinkAnimationId(linkInfo.Link, RadialMenuViewInfo.LinkBackgroundSelectionAnimationId)) as ColorAnimationInfo;
			if(cinfo != null)
				return cinfo.CurrentColor;
			return ViewInfo.HoverInfo.HitTest == RadialMenuHitTest.Link && ViewInfo.HoverInfo.LinkInfo == linkInfo ? ViewInfo.GetLinkBackgroundHoverColor(linkInfo) : ViewInfo.GetLinkBackgroundColor(linkInfo);
		}
		protected virtual void DrawLink(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			DrawLinkGlyph(e, linkInfo);
			DrawLinkCaption(e, linkInfo);
			if(e.ViewInfo.Menu.Manager.Helper.CustomizationManager.GetObjectSelected(linkInfo.Link.Item)) {
				DrawDesignTimeSelection(e, linkInfo);
			}
		}
		protected virtual void DrawDesignTimeSelection(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			Rectangle bounds = linkInfo.Bounds;
			bounds.Inflate(1, 1);
			e.ViewInfo.Menu.Manager.Helper.CustomizationManager.DrawDesignTimeSelection(e, bounds, 128);
		}
		protected virtual void DrawLinkCaption(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			RadialMenuViewInfo vi = e.ViewInfo;
			if(!e.DrawOptions.AllowDrawLinkText || string.IsNullOrEmpty(linkInfo.Link.Caption))
				return;
			if(linkInfo.RadialMenuLinkMetrics.WrappedText != null) {
				DrawWrappedCaption(e, linkInfo);
			}
			else {
				string str = linkInfo.Link.Caption;
				DrawLinkCaptionCore(e, linkInfo, linkInfo.CaptionRect, str);
			}
		}
		protected virtual void DrawWrappedCaption(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			RadialMenuLinkMetrics metrics = linkInfo.RadialMenuLinkMetrics;
			if(metrics.WrappedText.Length == 0) return;
			Rectangle rect = new Rectangle(linkInfo.CaptionRect.X, linkInfo.CaptionRect.Y, linkInfo.CaptionRect.Width, metrics.WrappedTextSize[0].Height);
			DrawLinkCaptionCore(e, linkInfo, rect, metrics.WrappedText[0]);
			if(metrics.WrappedText.Length > 1) {
				rect.Y += BarItemLinkTextHelper.LineIndent + metrics.WrappedTextSize[0].Height;
				rect.Height = metrics.WrappedTextSize[1].Height;
				DrawLinkCaptionCore(e, linkInfo, rect, metrics.WrappedText[1]);
			}
		}
		protected virtual void DrawLinkCaptionCore(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo, Rectangle rect, string str) {
			if(linkInfo.Link.Item.IsAllowHtmlText)
				str = StringPainter.Default.RemoveFormat(str);
			TextRenderingHint oldTextRenderingHint = e.Graphics.TextRenderingHint;
			try {
				e.Graphics.TextRenderingHint = ViewInfo.Menu.TextRenderingHint;
				using(StringFormat format = new StringFormat(linkInfo.LinkCaptionStringFormat)) {
					format.Alignment = StringAlignment.Center;
					e.Graphics.DrawString(str, linkInfo.OwnerAppearance.Font, GetLinkCaptionBrush(e, linkInfo), Rectangle.Inflate(rect, 1, 1), format);
				}
			}
			finally {
				e.Graphics.TextRenderingHint = oldTextRenderingHint;
			}
		}
		protected virtual Brush GetLinkCaptionBrush(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			if(e.ViewInfo.ShouldDrawLinkCaptionCustomColor(linkInfo)) {
				ColorAnimationInfo cinfo = XtraAnimator.Current.Get(ViewInfo.Window, new LinkAnimationId(linkInfo.Link, RadialMenuViewInfo.LinkCaptionSelectionAnimationId)) as ColorAnimationInfo;
				if(cinfo != null)
					return new SolidBrush(cinfo.CurrentColor);
				return new SolidBrush(ViewInfo.HoverInfo.HitTest == RadialMenuHitTest.Link && ViewInfo.HoverInfo.LinkInfo == linkInfo ? ViewInfo.GetLinkCaptionHoverColor(linkInfo) : ViewInfo.GetLinkCaptionColor(linkInfo));
			}
			else
				return e.Cache.GetSolidBrush(e.ViewInfo.GetTextColor(!linkInfo.Link.Enabled ? BarLinkState.Disabled : linkInfo.LinkState));
		}
		PrimitivesPainter primitivesPainterCore = null;
		protected PrimitivesPainter PrimitivesPainter {
			get {
				if(primitivesPainterCore == null) primitivesPainterCore = CreatePrimitivesPainter();
				return primitivesPainterCore;
			}
		}
		protected virtual PrimitivesPainter CreatePrimitivesPainter() {
			return new PrimitivesPainter(ViewInfo.Menu.Manager.PaintStyle);
		}
		protected virtual void DrawLinkGlyph(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo linkInfo) {
			BarLinkState linkState = e.ViewInfo.IsCustomizationMode ? BarLinkState.Normal : linkInfo.LinkState;
			if(!linkInfo.Link.Enabled) {
				linkState = BarLinkState.Disabled;
			}
			Image image = linkInfo.GetLinkImage(linkState);
			if(image == null)
				return;
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				Rectangle r = linkInfo.GlyphRect;
				r.Offset(0, e.ViewInfo.GetLinkGlyphVerticalOffset(e));
				BarLinkPaintArgs arg = new BarLinkPaintArgs(e.Cache, linkInfo, null);
				PrimitivesPainter.DrawLinkImage(arg, r, GetImageAttributes(linkInfo, linkState, e), image, linkState);
			}
			finally {
				e.Graphics.SmoothingMode = mode;
			}
		}
		protected virtual ImageAttributes GetImageAttributes(BarLinkViewInfo linkInfo, BarLinkState state, RadialMenuGraphicsInfoArgs e) {
			if(ViewInfo.GetAllowGlyphSkinning(linkInfo)) {
				Color cr = ((SolidBrush)GetLinkCaptionBrush(e, linkInfo)).Color;
				return ImageColorizer.GetColoredAttributes(cr);
			}
			if(state == BarLinkState.Disabled && !linkInfo.HasImage(BarLinkState.Disabled))
				return PaintHelper.DisabledAttributes;
			return null;
		}
		protected List<BarLinkViewInfo> GetActualDrawLinks(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo info = GetContentChangeAnimation(e);
			if(info == null || info.State == RadialMenuContentChangeState.ShowNextBounds || info.State == RadialMenuContentChangeState.ShowNextContent)
				return e.ViewInfo.LinksInfo;
			return e.ViewInfo.PrevLinksInfo;
		}
		protected virtual void DrawLinksAdorments(RadialMenuGraphicsInfoArgs e) {
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			List<BarLinkViewInfo> links = GetActualDrawLinks(e);
			try {
				if(e.DrawOptions.AllowDrawLinkBound) {
					foreach(BarLinkViewInfo link in links) {
						DrawLinkAdorments(e, link);
					}
				}
				if(IsContentChangeAnimation(e))
					e.Graphics.FillEllipse(e.Cache.GetSolidBrush(e.ViewInfo.BackColor), GetMenuInnerBounds(e));
				e.Graphics.FillEllipse(e.Cache.GetSolidBrush(e.ViewInfo.BackColor), GetClientBackgroundBounds(e));
				foreach(BarLinkViewInfo link in links) {
					if(!IsContentChangeAnimation(e)) DrawLinkBackground(e, link);
					DrawLinkCheckMark(e, link);
					DrawLinkSelection(e, link);
				}
			} finally {
				e.Graphics.SmoothingMode = mode;
			}
		}
		public virtual void DrawLinkAdorments(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo info) {
			if(!e.ViewInfo.HasArrow(info)) return;
			DrawLinkBound(e, info);
			DrawLinkArrow(e, info);
		}
		protected virtual Color GetLinkArrowSelectionColor(RadialMenuViewInfo vi, BarLinkViewInfo info) {
			ColorAnimationInfo cinfo = XtraAnimator.Current.Get(vi.Window, new LinkAnimationId(info.Link, RadialMenuViewInfo.LinkArrowSelectionAnimationId)) as ColorAnimationInfo;
			if(cinfo != null)
				return cinfo.CurrentColor;
			return vi.HoverInfo.HitTest == RadialMenuHitTest.LinkArrow && vi.HoverInfo.LinkInfo == info ? vi.GetLinkHoverColor(info) : vi.GetLinkColor(info);
		}
		protected virtual Rectangle GetLinkBoundsOuterRect(RadialMenuGraphicsInfoArgs e) {
			RadialMenuContentChangeAnimationInfo ainfo = GetContentChangeAnimation(e);
			Rectangle outerRect = e.ViewInfo.Bounds;
			if(ainfo != null && (ainfo.State == RadialMenuContentChangeState.HidePrevBounds || ainfo.State == RadialMenuContentChangeState.ShowNextBounds))
				outerRect = ainfo.LinkBoundsOuterRect;
			return outerRect;
		}
		protected virtual void DrawLinkBound(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo info) {
			if(!e.ViewInfo.ShouldDrawLinkBound(info))
				return;
			SmoothingMode prev = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			try {
				float startAngle = e.ViewInfo.GetStartAngle(info), sweepAngle = e.ViewInfo.GetSweepAngle(info), angleWidth = (float)info.AngleWidth;
				if(!info.Link.Item.ItemInMenuAppearance.Normal.BorderColor.IsEmpty) {
					e.Graphics.FillPie(e.Cache.GetSolidBrush(e.ViewInfo.BorderColor), GetLinkBoundsOuterRect(e), startAngle - angleWidth, sweepAngle + angleWidth * 2);
				}
				e.Graphics.FillPie(e.Cache.GetSolidBrush(GetLinkArrowSelectionColor(e.ViewInfo, info)), GetLinkBoundsOuterRect(e), startAngle, sweepAngle);
			}
			finally {
				e.Graphics.SmoothingMode = prev;
			}
		}
		protected virtual void DrawLinkArrow(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo info) {
			if(!e.ViewInfo.HasArrow(info) || IsContentChangeAnimation(e))
				return;
			SmoothingMode prev = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			try {
				e.Graphics.FillPolygon(e.Cache.GetSolidBrush(e.ViewInfo.BackColor), e.ViewInfo.GetArrowPoints(info));
			} finally {
				e.Graphics.SmoothingMode = prev;
			}
		}
		bool ShouldDrawLinkSelection(RadialMenuViewInfo vi, BarLinkViewInfo info) {
			FloatAnimationInfo ainfo = XtraAnimator.Current.Get(vi.Window, new LinkAnimationId(info.Link, RadialMenuViewInfo.LinkSelectionAnimationId)) as FloatAnimationInfo;
			if((vi.HoverInfo.HitTest != RadialMenuHitTest.Link || vi.HoverInfo.LinkInfo != info) && ainfo == null)
				return false;
			return true;
		}
		Color GetLinkSelectionColor(RadialMenuViewInfo vi, BarLinkViewInfo info) {
			return GetLinkSelectionColor(vi, info, vi.GetLinkColor(info));
		}
		Color GetLinkSelectionColorInMenu(RadialMenuViewInfo vi, BarLinkViewInfo info) {
			return GetLinkSelectionColor(vi, info, vi.MenuColor);
		}
		Color GetLinkSelectionColor(RadialMenuViewInfo vi, BarLinkViewInfo info, Color color) {
			FloatAnimationInfo ainfo = XtraAnimator.Current.Get(vi.Window, new LinkAnimationId(info.Link, RadialMenuViewInfo.LinkSelectionAnimationId)) as FloatAnimationInfo;
			Color selectionColor = ainfo != null ? Color.FromArgb((int)(ainfo.Value * 255), color) : color;
			return selectionColor;
		}
		bool ShouldDrawLinkCheck(BarLinkViewInfo info) {
			BarButtonItemLink link = info.Link as BarButtonItemLink;
			if(link != null && link.Item.IsCheckButtonStyle)
				return link.Item.Down;
			BarCheckItemLink clink = info.Link as BarCheckItemLink;
			if(clink != null)
				return ((BarCheckItem)clink.Item).Checked;
			return false;
		}
		protected virtual void DrawLinkCheckMark(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo info) {
			if(!ShouldDrawLinkCheck(info))
				return;
			e.Graphics.DrawArc(e.Cache.GetPen(e.ViewInfo.GetLinkColor(info), 1), e.ViewInfo.MenuItemSelectionBounds, e.ViewInfo.GetStartAngle(info), e.ViewInfo.GetSweepAngle(info));
		}
		protected virtual void DrawLinkSelection(RadialMenuGraphicsInfoArgs e, BarLinkViewInfo info) {
			if(!ShouldDrawLinkSelection(e.ViewInfo, info))
				return;
			e.Graphics.DrawArc(e.Cache.GetPen(GetLinkSelectionColorInMenu(e.ViewInfo, info), 3), e.ViewInfo.MenuItemSelectionBounds, e.ViewInfo.GetStartAngle(info), e.ViewInfo.GetSweepAngle(info));
		}
		float Rad2Degree(float value) {
			return (float)(value * 180 / Math.PI);
		}
	}
}
