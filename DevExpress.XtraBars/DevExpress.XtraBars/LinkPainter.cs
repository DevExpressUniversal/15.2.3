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
using DevExpress.XtraBars.Controls;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Painters {
	public enum DropMarkLocation { None, Left, Right, Top, Bottom }
	public class BarLinkPaintArgs : GraphicsInfoArgs {
		BarLinkViewInfo linkInfo;
		object sourceInfo;
		public BarLinkPaintArgs(GraphicsCache graphicsCache, BarLinkViewInfo linkInfo,  object sourceInfo) : base(graphicsCache, Rectangle.Empty) {
			this.linkInfo = linkInfo;
			this.sourceInfo = sourceInfo;
		}
		public BarLinkViewInfo LinkInfo { get { return linkInfo; } }
		public object SourceInfo { get { return sourceInfo; } }
	}
	public class BarEmptyLinkPainter : BarLinkPainter {
		public BarEmptyLinkPainter(BarManagerPaintStyle paintStyle)
			: base(paintStyle) {
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
		}
	}
	public class BarLinkPainter : CustomPainter {
		public BarLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected internal virtual void DrawLinkArrow(BarLinkPaintArgs e, BarLinkState drawState) { }
		protected internal virtual void DrawLinkArrowInMenu(BarLinkPaintArgs e, BarLinkState drawState) { }
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			BarLinkViewInfo linkInfo = info as BarLinkViewInfo;
			if(!linkInfo.Link.Item.IsPrivateItem) {
				BarItemCustomDrawEventArgs ce = new BarItemCustomDrawEventArgs(e.Cache, linkInfo);
				linkInfo.Manager.RaiseCustomDrawItem(ce);
				if(ce.Handled)
					return;
			}
			DrawLink(new BarLinkPaintArgs(e.Cache, info as BarLinkViewInfo, sourceInfo));
		}
		int lockOnlyTextDraw = 0;
		public virtual void DrawLinkTextOnly(BarLinkPaintArgs e) {
			this.lockOnlyTextDraw++;
			try {
				DrawLink(e);
			}
			finally {
				this.lockOnlyTextDraw--;
			}
		}
		protected bool IsOnlyDrawTextAllowed { get { return lockOnlyTextDraw != 0; } }
		public virtual void DrawLink(BarLinkPaintArgs e) {
			if(e.LinkInfo == null || e.LinkInfo.Link == null || e.LinkInfo.Link.Item == null) return;
			switch(e.LinkInfo.DrawMode) {
				case BarLinkDrawMode.Horizontal : DrawLinkHorizontal(e); break;
				case BarLinkDrawMode.Vertical : DrawLinkVertical(e); break;
				case BarLinkDrawMode.InMenu : DrawLinkInMenu(e); break;
				case BarLinkDrawMode.InMenuGallery: DrawLinkInGalleryMenu(e); break;
				case BarLinkDrawMode.InMenuLarge: DrawLinkInMenu(e); break;
				case BarLinkDrawMode.InMenuLargeWithText: DrawLinkInMenu(e); break;
				default:
					throw new Exception("Unknown drawmode");
			}
			if(e.LinkInfo.Link.Ribbon != null && e.LinkInfo.Manager.SelectionInfo.DropSelectedLink == e.LinkInfo.Link)
				PPainter.DrawRibbonLinkDropMark(e.Graphics, e.LinkInfo.Link, e.LinkInfo.Manager.SelectionInfo.DropSelectStyle);
		}
		protected Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state) {
			return CalcLinkHighlightedBrush(e, state, BarLinkParts.Bounds);
		}
		protected virtual Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			Brush brush = null;
			if(state == BarLinkState.Pressed) 
				brush = e.LinkInfo.DrawParameters.Colors.Brushes(BarColor.LinkPressedBackColor);
			if(state == BarLinkState.Highlighted) {
				brush = e.LinkInfo.DrawParameters.Colors.Brushes(BarColor.LinkHighlightBackColor);
			}
			if(state == BarLinkState.Checked) 
				brush = e.LinkInfo.DrawParameters.Colors.Brushes(BarColor.LinkCheckedBackColor);
			if(brush == null) brush = e.LinkInfo.GetBackBrush(e.Cache, e.LinkInfo.Bounds);
			return brush;
		}
		protected virtual void DrawLinkHorizontal(BarLinkPaintArgs e) {
			BarLinkState st = e.LinkInfo.CalcRealPaintState();
			switch(st) {
				case BarLinkState.Highlighted : DrawLinkHighlighted(e); break;
				case BarLinkState.Pressed: DrawLinkPressed(e); break;
				default:
					DrawLinkNormal(e);
					break;
			}
		}
		protected virtual void DrawLinkVertical(BarLinkPaintArgs e) {
			DrawLinkHorizontal(e);
		}
		protected virtual void DrawLinkInMenu(BarLinkPaintArgs e) {
			BarLinkState st = e.LinkInfo.CalcRealPaintState();
			switch(st) {
				case BarLinkState.Highlighted : DrawLinkHighlightedInMenu(e); break;
				case BarLinkState.Pressed: DrawLinkPressedInMenu(e); break;
				default:
					DrawLinkNormalInMenu(e);
					break;
			}
		}
		protected virtual void DrawLinkInGalleryMenu(BarLinkPaintArgs e) {
			DrawLinkInMenu(e);
		}
		protected virtual void DrawLinkHighlightedInMenu(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Highlighted);
				DrawLinkShortcut(e, BarLinkState.Highlighted);
				return;
			}
			DrawBorder(e);
			DrawLinkRectangles(e);
			Rectangle r = e.LinkInfo.SelectRect;
			DrawLinkHighlightedBackgroundInMenu(e, BarLinkState.Highlighted);
			DrawLinkGlyphInMenu(e, BarLinkState.Highlighted);
			DrawLinkCaption(e, BarLinkState.Highlighted);
			DrawLinkShortcut(e, BarLinkState.Highlighted);
			DrawLinkAdornmentsInMenu(e, BarLinkState.Highlighted);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		public void DrawLinkGlyph(BarLinkPaintArgs e, BarLinkState state) {
			DrawLinkGlyphInMenu(e, state);
		}
		protected virtual void DrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			switch(state) {
				case BarLinkState.Highlighted : DrawLinkHighlightedGlyph(e); break;
				default:
					DrawLinkNormalGlyph(e, e.LinkInfo.AllowDrawLighter); break;
			}
		}
		protected virtual void DrawLinkPressedInMenu(BarLinkPaintArgs e) {
			DrawLinkHighlightedInMenu(e);
		}
		protected virtual void DrawLinkNormalInMenu(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Normal);
				DrawLinkShortcut(e, BarLinkState.Normal);
				return;
			}
			if(e.LinkInfo.AllowDrawBackground)
				PPainter.DrawLinkBackgroundInMenu(e);
			DrawBorder(e);
			DrawLinkCheckedInGalleryMenu(e, e.LinkInfo.LinkState);
			DrawLinkRectangles(e);
			DrawLinkCaption(e, BarLinkState.Normal);
			DrawLinkShortcut(e, BarLinkState.Normal);
			DrawLinkAdornmentsInMenu(e, BarLinkState.Normal);
			DrawLinkGlyphInMenu(e, e.LinkInfo.LinkState);
			Rectangle r = e.LinkInfo.SelectRect;
			DrawLinkSelection(e, ref r);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		protected virtual void DrawLinkCheckedInGalleryMenu(BarLinkPaintArgs e, BarLinkState barLinkState) { }
		protected internal virtual void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.AllowDrawBackground) return;
			PPainter.DrawLinkHighlightedBackgroundInMenu(e, CalcEmptyBorder(e), CalcLinkHighlightedBrush(e, e.LinkInfo.CalcRealPaintState()), drawState);
		}
		protected internal virtual void DrawLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.AllowDrawBackground) return;
			PPainter.DrawLinkHighLightedBackground(e, e.LinkInfo.SelectRect, CalcEmptyBorder(e), e.LinkInfo.CalcRealPaintState(), CalcLinkHighlightedBrush(e, drawState));
		}
		protected virtual void DrawLinkHighlighted(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Highlighted);
				return;
			}
			DrawBorder(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(e.LinkInfo.AllowDrawBackground)
				DrawLinkHighlightedBackground(e, BarLinkState.Highlighted);
			DrawLinkHighlightedGlyph(e);
			DrawLinkCaption(e, BarLinkState.Highlighted);
			DrawLinkAdornments(e, BarLinkState.Highlighted);
			DrawLinkRectangles(e);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		protected virtual BarLinkEmptyBorder CalcEmptyBorder(BarLinkPaintArgs e) {
			return BarLinkEmptyBorder.None;
		}
		protected virtual void DrawLinkPressed(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Pressed);
				return;
			}
			DrawBorder(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(e.LinkInfo.AllowDrawBackground)
				DrawLinkHighlightedBackground(e, BarLinkState.Pressed);
			DrawLinkPressedGlyph(e);
			DrawLinkCaption(e, BarLinkState.Pressed);
			DrawLinkAdornments(e, BarLinkState.Pressed);
			DrawLinkRectangles(e);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		protected virtual void DrawLinkNormal(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Normal);
				return;
			}
			DrawBorder(e);
			Rectangle r = e.LinkInfo.SelectRect;
			DrawLinkSelection(e, ref r);
			if(e.LinkInfo.AllowDrawBackground) {
				Brush brush = e.LinkInfo.GetBackBrush(e.Cache, r);
				PPainter.DrawLinkNormalBackground(e, brush, r);
			}
			DrawLinkNormalGlyph(e, e.LinkInfo.AllowDrawLighter);
			DrawLinkCaption(e, BarLinkState.Normal);
			DrawLinkAdornments(e, BarLinkState.Normal);
			DrawLinkRectangles(e);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		public virtual void DrawLinkHighlightedGlyph(BarLinkPaintArgs e) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph)) return;
			if(e.LinkInfo.DrawDisabled) {
				DrawLinkDisabledGlyph(e);
				return;
			}
			Rectangle r = e.LinkInfo.GlyphRect;
			if(DrawParameters.Constants.AllowLinkGlyphShadow) {
				r.Offset(DrawParameters.Constants.ImageShadowHIndent, DrawParameters.Constants.ImageShadowVIndent);
				DrawLinkImageCore(e, r, BarLinkState.Highlighted, PaintHelper.GrayAttributes);
				r = e.LinkInfo.GlyphRect;
			} else {
				r.Inflate(-DrawParameters.Constants.ImageOffset, -DrawParameters.Constants.ImageOffset);
			}
			DrawLinkImageCore(e, r, BarLinkState.Highlighted, GetColoredAttributes(e));
		}
		protected virtual ImageAttributes GetColoredAttributes(BarLinkPaintArgs e) {
			if(e.LinkInfo.Link.Item.GetAllowGlyphSkinning())
				return ImageColorizer.GetColoredAttributes(e.LinkInfo.OwnerAppearance.ForeColor);
			return null;
		}
		protected virtual void DrawLinkDisabledGlyph(BarLinkPaintArgs e) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph)) return;
			Rectangle r = e.LinkInfo.GlyphRect;
			r.Inflate(-DrawParameters.Constants.ImageOffset, -DrawParameters.Constants.ImageOffset);
			DrawLinkImageCore(e, r, BarLinkState.Disabled, e.LinkInfo.HasImage(BarLinkState.Disabled)? null : PaintHelper.DisabledAttributes);
		}
		protected virtual void DrawLinkPressedGlyph(BarLinkPaintArgs e) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph)) return;
			if(e.LinkInfo.DrawDisabled) {
				DrawLinkDisabledGlyph(e);
				return;
			}
			Rectangle r = e.LinkInfo.GlyphRect;
			r.Inflate(-DrawParameters.Constants.ImageOffset, -DrawParameters.Constants.ImageOffset);
			DrawLinkImageCore(e, r, BarLinkState.Pressed, GetColoredAttributes(e));
		}
		public void DrawLinkNormalGlyph(BarLinkPaintArgs e, bool lighter) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph)) return;
			if(e.LinkInfo.DrawDisabled) {
				DrawLinkDisabledGlyph(e);
				return;
			}
			Rectangle r = e.LinkInfo.GlyphRect;
			r.Inflate(-DrawParameters.Constants.ImageOffset, -DrawParameters.Constants.ImageOffset);
			DrawLinkImageCore(e, r, BarLinkState.Normal, lighter ? PaintHelper.LighterAttributes : GetColoredAttributes(e));
		}
		protected virtual void DrawLinkImageCore(BarLinkPaintArgs e, Rectangle r, BarLinkState state, ImageAttributes attr) {
			PPainter.DrawLinkImage(e, r, attr,  e.LinkInfo.GetLinkImage(state), state);
		}
		protected void DrawLinkCaption(BarLinkPaintArgs e, BarLinkState state) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Caption)) return;
			SolidBrush brush = (SolidBrush)PPainter.CalcLinkForeBrush(e, state);
			if(e.LinkInfo.Link.IsAllowHtmlText) {
				e.LinkInfo.HtmlStringCaptionInfo.UpdateAppearanceColors(brush.Color, e.LinkInfo.OwnerAppearance.BackColor, e.LinkInfo.OwnerAppearance.BackColor2);
				StringPainter.Default.DrawString(e.Cache, e.LinkInfo.HtmlStringCaptionInfo);
			}
			else
				PPainter.DrawLinkCaption(e, brush, state);
			DrawLinkDescription(e, state);
		}
		protected void DrawLinkDescription(BarLinkPaintArgs e, BarLinkState state) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Description)) return;
			SolidBrush brush = (SolidBrush)PPainter.CalcLinkForeBrush(e, state);
			if(e.LinkInfo.Link.IsAllowHtmlText) {
				e.LinkInfo.HtmlStringDescriptionInfo.UpdateAppearanceColors(brush.Color, e.LinkInfo.OwnerAppearance.BackColor, e.LinkInfo.OwnerAppearance.BackColor2);
				StringPainter.Default.DrawString(e.Cache, e.LinkInfo.HtmlStringDescriptionInfo);
			}
			else
				PPainter.DrawLinkDescription(e, state);
		}
		protected virtual void DrawLinkShortcut(BarLinkPaintArgs e, BarLinkState state) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Shortcut)) return;
			if(e.LinkInfo.IsRightToLeft) {
				if(e.LinkInfo.Rects[BarLinkParts.Shortcut].Right > e.LinkInfo.Rects[BarLinkParts.Caption].Left) return; 
			} else
			if(e.LinkInfo.Rects[BarLinkParts.Caption].Right > e.LinkInfo.Rects[BarLinkParts.Shortcut].Left) return; 
			Brush itemBrush = PPainter.CalcLinkForeBrush(e, state);
			PaintHelper.DrawString(e.Cache, e.LinkInfo.ShortCutDisplayText, e.LinkInfo.Font, 
				itemBrush, e.LinkInfo.Rects[BarLinkParts.Shortcut], DrawParameters.ShortCutStringFormat);
		}
		protected virtual void DrawLinkAdornments(BarLinkPaintArgs e, BarLinkState drawState) {
		}
		protected virtual void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
		}
		protected virtual void DrawLinkRectangles(BarLinkPaintArgs e) {
			foreach(RectInfo info in e.LinkInfo.Rects.ExtRectangles) {
				switch(info.Type) {
					case RectInfoType.SubMenuSeparator:
						if(e.LinkInfo.AllowDrawLinkSeparator)
							PPainter.DrawSubmenuSeparator(e, info.Rect, e.LinkInfo, e.SourceInfo);
						break;
					case RectInfoType.BackGround:
						if(e.LinkInfo.BarControlInfo != null) {
							PaintHelper.FillRectangle(e.Graphics, e.LinkInfo.BarControlInfo.Appearance.Normal.GetBackBrush(e.Cache, info.Rect), info.Rect);
						}
						break;
					case RectInfoType.VertSeparator:
					case RectInfoType.HorzSeparator:
						PPainter.DrawSeparator(e, info, e.LinkInfo, e.SourceInfo);
						break;
					default:
						if(info.Brush != null)
							PaintHelper.FillRectangle(e.Graphics, info.Brush, info.Rect);
						break;
				}
			}
		}
		protected virtual void DrawLinkSelection(BarLinkPaintArgs e, ref Rectangle r) {
			if(GetManager(e.LinkInfo).SelectionInfo.CustomizeSelectedLink == e.LinkInfo.Link && !e.LinkInfo.Link.Manager.IsLinkSizing) {
				PaintHelper.DrawSelectedFrame(e.Graphics, ref r);
			}
		}
		protected virtual void DrawBorder(BarLinkPaintArgs e) {
			if((e.LinkInfo.DrawParts & BarLinkParts.Border) == 0) return;
			Rectangle r = e.LinkInfo.Rects[BarLinkParts.Border];
			if(!r.IsEmpty)  {
				ObjectPainter.DrawObject(e.Cache, e.LinkInfo.GetBorderPainter(), new BorderObjectInfoArgs(null, r, e.LinkInfo.Link.Item.Appearance.Options.UseBorderColor? e.LinkInfo.Link.Item.Appearance: null));
			}
		}
	}
	public class BarButtonLinkPainter : BarBaseButtonLinkPainter {
		public BarButtonLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkAdornments(BarLinkPaintArgs e, BarLinkState drawState) {
			base.DrawLinkAdornments(e, drawState);
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow)) return;
			DrawLinkButtonAdorments(e, drawState);
		}
		protected internal virtual void DrawLinkButtonAdorments(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return;
			r = ar;
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(e.LinkInfo.AllowDrawBackground && !link.Opened && !link.Item.ActAsDropDown && drawState != BarLinkState.Normal)
				PPainter.DrawLinkHighLightedBackground(e, ar, CalcEmptyBorder(e), e.LinkInfo.CalcRealPaintState(), CalcLinkHighlightedBrush(e, BarLinkState.Highlighted));
			DrawLinkArrow(e, drawState);
		}
		protected internal override void DrawLinkArrow(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return;
			r = ar;
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			PPainter.DrawArrow(e, PPainter.CalcLinkForeBrush(e, drawState), r.Location, r.Width, MarkArrowDirection.Down);
		}
		protected internal override void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			BarButtonLinkViewInfo info = e.LinkInfo as BarButtonLinkViewInfo;
			if(!info.IsNeedOpenArrow || info.Link.Item.ActAsDropDown) {
				base.DrawLinkHighlightedBackgroundInMenu(e, drawState);
				return;
			}
			PPainter.DrawSplitLinkHighlightedBackgroundInMenu(e, CalcEmptyBorder(e), CalcLinkHighlightedBrush(e, e.LinkInfo.CalcRealPaintState()), drawState);
		}
		protected override BarLinkEmptyBorder CalcEmptyBorder(BarLinkPaintArgs e) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(e.LinkInfo.IsLinkInMenu) return BarLinkEmptyBorder.None;
			if(link.Item.DropDownControl != null && link.LinkViewInfo != null) {
				Rectangle r = link.Item.DropDownControl.Bounds, linkRect = link.LinkViewInfo.Bounds;
				linkRect.Location = link.LinkPointToScreen(linkRect.Location);
				if(linkRect.Right == r.Left && linkRect.Top == r.Top) return BarLinkEmptyBorder.Right;
				if(linkRect.Left == r.Right && linkRect.Top == r.Top) return BarLinkEmptyBorder.Left;
				if(linkRect.Bottom == r.Top) return BarLinkEmptyBorder.Down;
				if(linkRect.Top == r.Bottom) return BarLinkEmptyBorder.Up;
			}
			return BarLinkEmptyBorder.None;
		}
		protected internal override void DrawLinkArrowInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			if(e.LinkInfo.Rects[BarLinkParts.OpenArrow].IsEmpty) return;
			PPainter.DrawButtonLinkArrowAdormentsInMenu(e, drawState, CalcLinkHighlightedBrush(e, BarLinkState.Pressed));
		}
		protected override void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			base.DrawLinkAdornmentsInMenu(e, drawState);
			if(e.LinkInfo.IsInnerLink || !e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow)) return;
			if(e.LinkInfo.Rects[BarLinkParts.OpenArrow].IsEmpty) return;
			PPainter.DrawButtonLinkArrowAdormentsInMenu(e, drawState, CalcLinkHighlightedBrush(e, BarLinkState.Pressed));
		}
		protected virtual void DrawButtonLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			r = e.LinkInfo.SelectRect;
			if(!link.Opened && !link.Item.ActAsDropDown) r.Width = ar.Left - r.Left + 1;
			PPainter.DrawLinkHighLightedBackground(e, r, CalcEmptyBorder(e), e.LinkInfo.CalcRealPaintState(), CalcLinkHighlightedBrush(e, drawState));
		}
		protected internal override void DrawLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.AllowDrawBackground) return;
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow) || e.LinkInfo.Rects[BarLinkParts.OpenArrow].IsEmpty) {
				base.DrawLinkHighlightedBackground(e, drawState);
				return;
			}
			DrawButtonLinkHighlightedBackground(e, drawState);
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(link.Opened && !e.LinkInfo.IsLinkInMenu) return e.LinkInfo.OwnerAppearance.GetBackBrush(e.Cache);
			if((part == BarLinkParts.Glyph || !e.LinkInfo.IsLinkInMenu) && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				if((e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0)
					return DrawParameters.Colors.Brushes(BarColor.LinkCheckedPressedBackColor);
			}
			if(e.LinkInfo.IsLinkInMenu || !link.Opened) {
				return base.CalcLinkHighlightedBrush(e, state, part);
			}
			return e.LinkInfo.OwnerAppearance.GetBackBrush(e.Cache);
		}
	}
	public class BarBaseButtonLinkPainter : BarLinkPainter {
		public BarBaseButtonLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkHorizontal(BarLinkPaintArgs e) {
			BarLinkState st = e.LinkInfo.CalcRealPaintState();
			switch(st) {
				case BarLinkState.Checked : DrawLinkChecked(e); break;
				default:
					base.DrawLinkHorizontal(e);
					break;
			}
		}
		protected virtual bool AllowFillCheckBackground { get { return true; } }
		protected override void DrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			if(!IsCheckedState(e) || e.LinkInfo.DrawNormalGlyphInCheckedState || e.LinkInfo.DrawMode == BarLinkDrawMode.InMenuGallery) {
				base.DrawLinkGlyphInMenu(e, state);
				return;
			}
			if(!((BarBaseButtonLinkViewInfo)e.LinkInfo).AllowDrawCheckInMenu)
				return;
			PPainter.DrawLinkCheckInMenu(e, state, AllowFillCheckBackground ? CalcLinkHighlightedBrush(e, state, BarLinkParts.Glyph) : null, this);
		}
		protected bool IsCheckedState(BarLinkPaintArgs e) {
			if(e.LinkInfo.Link != null) {
				BarButtonItem button = e.LinkInfo.Link.Item as BarButtonItem;
				if(button != null && button.ButtonStyle == BarButtonStyle.CheckDropDown && button.Down)
					return true;
			}
			return (e.LinkInfo.LinkState & BarLinkState.Checked) != 0;
		}
		protected override void DrawLinkCheckedInGalleryMenu(BarLinkPaintArgs e, BarLinkState barLinkState) {
			if(e.LinkInfo.DrawMode != BarLinkDrawMode.InMenuGallery || e.LinkInfo.LinkState != BarLinkState.Checked)
				return;
			DrawLinkCheckedBackgroundInGalleryMenu(e);
		}
		private void DrawLinkCheckedBackgroundInGalleryMenu(BarLinkPaintArgs e) {
			PPainter.DrawLinkCheckedInGalleryMenu(e, e.LinkInfo.SelectRect, CalcEmptyBorder(e), e.LinkInfo.CalcRealPaintState(), CalcLinkHighlightedBrush(e, e.LinkInfo.LinkState));
		}
		protected virtual void DrawLinkChecked(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Checked);
				return;
			}
			DrawBorder(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(e.LinkInfo.AllowDrawBackground)
				DrawLinkHighlightedBackground(e, BarLinkState.Checked);
			DrawLinkNormalGlyph(e, false);
			DrawLinkCaption(e, BarLinkState.Checked);
			DrawLinkAdornments(e, BarLinkState.Checked);
			DrawLinkRectangles(e);
			DrawLinkSelection(e, ref r);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarBaseButtonItemLink link = e.LinkInfo.Link as BarBaseButtonItemLink;
			if((part == BarLinkParts.Glyph || !e.LinkInfo.IsLinkInMenu) && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				if((e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0)
					return DrawParameters.Colors.Brushes(BarColor.LinkCheckedPressedBackColor);
			}
			return base.CalcLinkHighlightedBrush(e, state, part);
		}
	}
	public class BarToggleSwitchLinkPainter : BarLinkPainter {
		public BarToggleSwitchLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected internal override void DrawLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) { }
		public override void DrawLink(BarLinkPaintArgs e) {
			base.DrawLink(e);
			BarToggleSwitchLinkViewInfo linkInfo = (BarToggleSwitchLinkViewInfo)e.LinkInfo;
			if(linkInfo == null) return;
			DrawToggleSwitch(e.Cache, linkInfo);
		}
		protected virtual void DrawToggleSwitch(GraphicsCache cache, BarToggleSwitchLinkViewInfo vi) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetToggleBackgroundElementInfo(vi));
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetToggleElementInfo(vi));
		}
		protected virtual SkinElementInfo GetToggleBackgroundElementInfo(BarToggleSwitchLinkViewInfo vi) {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(vi.Manager.GetController().LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitch], vi.ToggleBounds);
			info.ImageIndex = (vi.Item.Checked && vi.LinkState != BarLinkState.Disabled) ? 1 : 0;
			return info;
		}
		protected virtual SkinElementInfo GetToggleElementInfo(BarToggleSwitchLinkViewInfo vi) {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(vi.Manager.GetController().LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitchThumb], vi.ToggleContentBounds);
			info.ImageIndex = (int)vi.LinkState;
			if(vi.LinkState == BarLinkState.Disabled) info.ImageIndex = 3;
			return info;
		}
	}
	public class BarCheckButtonLinkPainter : BarBaseButtonLinkPainter {
		public BarCheckButtonLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected internal override void DrawLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			if(IsCheckVisible(e.LinkInfo)) return;
			base.DrawLinkHighlightedBackground(e, drawState);
		}
		protected override void DrawLinkNormal(BarLinkPaintArgs e) {
			base.DrawLinkNormal(e);
			if(IsOnlyDrawTextAllowed || !IsCheckVisible(e.LinkInfo)) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & BarLinkState.Disabled) == BarLinkState.Disabled)
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Disabled);
			else
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Normal);
		}
		protected override void DrawLinkHighlighted(BarLinkPaintArgs e) {
			base.DrawLinkHighlighted(e);
			if(IsOnlyDrawTextAllowed || !IsCheckVisible(e.LinkInfo)) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & BarLinkState.Disabled) == BarLinkState.Disabled)
				DrawDisabledCheckBox(e);
			else if((st & (BarLinkState.Checked | BarLinkState.Highlighted)) == (BarLinkState.Checked | BarLinkState.Highlighted))
				DrawCheckBox(e, CheckState.Checked, ObjectState.Hot);
			else if((st & (BarLinkState.Checked | BarLinkState.Highlighted)) == BarLinkState.Highlighted)
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Hot);
		}
		protected override void DrawLinkPressed(BarLinkPaintArgs e) {
			base.DrawLinkPressed(e);
			if(IsOnlyDrawTextAllowed || !IsCheckVisible(e.LinkInfo)) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & BarLinkState.Disabled) == BarLinkState.Disabled)
				DrawDisabledCheckBox(e);
			else if((st & (BarLinkState.Checked | BarLinkState.Pressed)) == (BarLinkState.Checked | BarLinkState.Pressed))
				DrawCheckBox(e, CheckState.Checked, ObjectState.Pressed);
			else if((st & (BarLinkState.Checked | BarLinkState.Pressed)) == BarLinkState.Pressed)
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Pressed);
		}
		protected override void DrawLinkChecked(BarLinkPaintArgs e) {
			if(!IsCheckVisible(e.LinkInfo)) {
				base.DrawLinkChecked(e);
				return;
			}
			base.DrawLinkNormal(e);
			if(IsOnlyDrawTextAllowed) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & BarLinkState.Disabled) == BarLinkState.Disabled)
				DrawCheckBox(e, CheckState.Checked, ObjectState.Disabled);
			else
				DrawCheckBox(e, CheckState.Checked, ObjectState.Normal);
		}
		protected override void DrawLinkSelection(BarLinkPaintArgs e, ref Rectangle r) {
			base.DrawLinkSelection(e, ref r);
			if(IsOnlyDrawTextAllowed || !IsCheckVisible(e.LinkInfo)) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & BarLinkState.Disabled) == BarLinkState.Disabled)
				DrawDisabledCheckBox(e);
			else if((st & (BarLinkState.Checked | BarLinkState.Selected)) == (BarLinkState.Checked | BarLinkState.Selected))
				DrawCheckBox(e, CheckState.Checked, ObjectState.Selected);
			else if((st & (BarLinkState.Checked | BarLinkState.Selected)) == BarLinkState.Selected)
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Selected);
		}
		private void DrawDisabledCheckBox(BarLinkPaintArgs e) {
			if(!IsCheckVisible(e.LinkInfo)) return;
			BarLinkState st = e.LinkInfo.LinkState;
			if((st & (BarLinkState.Checked | BarLinkState.Disabled)) == (BarLinkState.Checked | BarLinkState.Disabled))
				DrawCheckBox(e, CheckState.Checked, ObjectState.Disabled);
			else if((st & (BarLinkState.Checked | BarLinkState.Disabled)) == BarLinkState.Disabled)
				DrawCheckBox(e, CheckState.Unchecked, ObjectState.Disabled);
		}
		protected internal void DrawCheckBox(BarLinkPaintArgs e, CheckState checkState, ObjectState objState) {
			var linkInfo = e.LinkInfo as BarCheckButtonLinkViewInfo;
			if(linkInfo == null || !IsCheckVisible(e.LinkInfo)) return;
			var checkInfo = new CheckObjectInfoArgs(e.Cache, AppearanceObject.ControlAppearance);
			checkInfo.Bounds = linkInfo.Rects[BarLinkParts.Checkbox];
			checkInfo.CheckState = checkState;
			checkInfo.State = objState;
			checkInfo.CheckStyle = ((BarCheckItem)e.LinkInfo.Link.Item).GetCheckStyle();
			ObjectPainter.CalcObjectBounds(e.Cache.Graphics, linkInfo.CheckPainter, checkInfo);
			ObjectPainter.DrawObject(e.Cache, linkInfo.CheckPainter, checkInfo);
		}
		private bool IsCheckVisible(BarLinkViewInfo info) {
			var linkInfo = info as BarCheckButtonLinkViewInfo;
			if(linkInfo == null || linkInfo.IsLinkInMenu || linkInfo.GetCheckBoxVisibility() == CheckBoxVisibility.None) return false;
			return true;
		}
	}
	public class BarLargeButtonLinkPainter : BarButtonLinkPainter {
		public BarLargeButtonLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
	}
	public class BarRecentExpanderLinkPainter : BarLinkPainter {
		public BarRecentExpanderLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle ar = e.LinkInfo.Bounds, r;
			r = ar;
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			PPainter.DrawMark(e, SystemPens.ControlText, r.Location, r.Width, MarkArrowDirection.Down);
		}
	}
	public class BarScrollLinkPainter : BarLinkPainter {
		public BarScrollLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawLink(BarLinkPaintArgs e) {
			Rectangle r = e.LinkInfo.Bounds;
			BarScrollItemLink link = e.LinkInfo.Link as BarScrollItemLink;
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			r.Y += (r.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			r.X += (e.LinkInfo.Bounds.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			PPainter.DrawArrow(e, DrawParameters.Colors.MenuAppearance.Menu.GetForeBrush(e.Cache), r.Location, r.Width, link.ScrollDown ? MarkArrowDirection.Down : MarkArrowDirection.Up);
		}
	}
	public class BarHeaderLinkPainter : BarStaticLinkPainter {
		public BarHeaderLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override BarLinkEmptyBorder CalcEmptyBorder(BarLinkPaintArgs e) {
			return BarLinkEmptyBorder.All;
		}
		protected override void DrawBorder(BarLinkPaintArgs e) {
			e.LinkInfo.GetMenuHeaderPainter().DrawObject(e);
		}
	}
	public class BarStaticLinkPainter : BarLinkPainter {
		public BarStaticLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
	}
	public class BarEditLinkPainter : BarLinkPainter {
		static DevExpress.Utils.Paint.XPaint standardPainter = DevExpress.Utils.Paint.XPaint.CreateDefaultPainter();
		public BarEditLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawLink(BarLinkPaintArgs e) {
			base.DrawLink(e);
			DrawEdit(e);
		}
		protected virtual void DrawEdit(BarLinkPaintArgs e) {
			Rectangle r;
			BarEditLinkViewInfo vInfo = e.LinkInfo as BarEditLinkViewInfo;
			if(vInfo.EditViewInfo == null || vInfo.Link == null || vInfo.Link.Item == null) return;
			vInfo.UpdateEditState();
			GetManager(e.LinkInfo).EditorHelper.DrawCellEdit(e, vInfo.EditViewInfo.Item, vInfo.EditViewInfo);
			r = e.LinkInfo.SelectRect;
			DrawLinkSelection(e, ref r);
		}
	}
	public class BarQMenuAddRemoveButtonsLinkPainter : BarCustomContainerLinkPainter {
		public BarQMenuAddRemoveButtonsLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override BarLinkEmptyBorder CalcEmptyBorder(BarLinkPaintArgs e) {
			BarCustomContainerItemLink link = e.LinkInfo.Link as BarCustomContainerItemLink;
			if(link.SubControl != null) {
				Rectangle linkRect = link.SubControl.PopupOwnerRectangle, r = link.SubControl.Form.Bounds;
				if(linkRect.Right == r.Left && linkRect.Top == r.Top) return BarLinkEmptyBorder.Right;
				if(linkRect.Left == r.Right && linkRect.Top == r.Top) return BarLinkEmptyBorder.Left;
				if(link.SubControl.PopupOwnerRectangle.Bottom == link.SubControl.Form.Top) 
					return BarLinkEmptyBorder.Down;
				if(link.SubControl.PopupOwnerRectangle.Top == link.SubControl.Form.Bottom) 
					return BarLinkEmptyBorder.Up;
				return BarLinkEmptyBorder.All;
			}
			return BarLinkEmptyBorder.None;
		}
		public override void DrawLink(BarLinkPaintArgs e) {
			e.LinkInfo.ReverseLinkRects();
			base.DrawLink(e);
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarCustomContainerItemLink link = e.LinkInfo.Link as BarCustomContainerItemLink;
			if(link.Opened) return e.LinkInfo.AppearanceMenu.SideStrip.GetBackBrush(e.Cache);
			return base.CalcLinkHighlightedBrush(e, state, part);
		}
		protected override void DrawLinkHighlightedInMenu(BarLinkPaintArgs e) {
			if(IsOnlyDrawTextAllowed) {
				DrawLinkCaption(e, BarLinkState.Highlighted);
				DrawLinkShortcut(e, BarLinkState.Highlighted);
				return;
			}
			BarCustomContainerItemLink link = e.LinkInfo.Link as BarCustomContainerItemLink;
			DrawBorder(e);
			DrawLinkRectangles(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(e.LinkInfo.AllowDrawBackground) {
				if(link.Opened) 
					DrawLinkHighlightedBackground(e, BarLinkState.Highlighted);
				else
					PPainter.DrawLinkHighLightedBackground(e, r, CalcEmptyBorder(e), e.LinkInfo.CalcRealPaintState(), CalcLinkHighlightedBrush(e, BarLinkState.Highlighted));
			}
			DrawLinkGlyphInMenu(e, BarLinkState.Highlighted);
			DrawLinkCaption(e, BarLinkState.Highlighted);
			DrawLinkShortcut(e, BarLinkState.Highlighted);
			DrawLinkAdornmentsInMenu(e, BarLinkState.Highlighted);
			PPainter.DrawBarLinkDropMark(e, e.LinkInfo);
		}
	}
	public class BarCustomContainerLinkPainter : BarLinkPainter {
		public BarCustomContainerLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override BarLinkEmptyBorder CalcEmptyBorder(BarLinkPaintArgs e) {
			BarCustomContainerItemLink link = e.LinkInfo.Link as BarCustomContainerItemLink;
			if(e.LinkInfo.IsLinkInMenu) return BarLinkEmptyBorder.None;
			if(link.SubControl != null && link.SubControl.Form != null) {
				Rectangle linkRect = link.SubControl.PopupOwnerRectangle, r = link.SubControl.Form.Bounds;
				if(linkRect.Right == r.Left && linkRect.Top == r.Top) return BarLinkEmptyBorder.Right;
				if(linkRect.Left == r.Right && linkRect.Top == r.Top) return BarLinkEmptyBorder.Left;
				if(link.SubControl.PopupOwnerRectangle.Bottom == link.SubControl.Form.Top) 
					return BarLinkEmptyBorder.Down;
				if(link.SubControl.PopupOwnerRectangle.Top == link.SubControl.Form.Bottom) 
					return BarLinkEmptyBorder.Up;
			}
			return BarLinkEmptyBorder.None;
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarCustomContainerItemLink link = e.LinkInfo.Link as BarCustomContainerItemLink;
			if(e.LinkInfo.IsLinkInMenu || !link.Opened || !e.LinkInfo.CanDrawAs(BarLinkState.Highlighted)) {
				if(state == BarLinkState.Pressed) state = BarLinkState.Highlighted;
				return base.CalcLinkHighlightedBrush(e, state, part);
			}
			if(e.LinkInfo.Link.Bar != null)
				return e.LinkInfo.Link.Bar.ViewInfo.Appearance.Normal.GetBackBrush(e.Cache);
			return e.LinkInfo.OwnerAppearance.GetBackBrush(e.Cache);
		}
		protected internal override void DrawLinkArrow(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return;
			r = ar;
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			PPainter.DrawArrow(e, PPainter.CalcLinkForeBrush(e, drawState), r.Location, r.Width, e.LinkInfo.IsDrawVerticalRotated ? MarkArrowDirection.Left : MarkArrowDirection.Down);
		}
		protected override void DrawLinkAdornments(BarLinkPaintArgs e, BarLinkState drawState) {
			base.DrawLinkAdornments(e, drawState);
			DrawLinkArrow(e, drawState);
		}
		protected internal override void DrawLinkArrowInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			if(e.LinkInfo.IsInnerLink || !e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return;
			r = ar;
			r.Width = DrawParameters.Constants.SubMenuRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			PPainter.DrawArrow(e, PPainter.CalcLinkForeBrush(e, drawState), r.Location, r.Width, e.LinkInfo.IsRightToLeft ? MarkArrowDirection.Left : MarkArrowDirection.Right);
		}
		protected override void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			base.DrawLinkAdornmentsInMenu(e, drawState);
			DrawLinkArrowInMenu(e, drawState);
		}
	}
	public class BarQMenuCustomizationLinkPainter : BarButtonLinkPainter {
		public BarQMenuCustomizationLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawLink(BarLinkPaintArgs e) {
			BarQMenuCustomizationLinkViewInfo li = e.LinkInfo as BarQMenuCustomizationLinkViewInfo;
			base.DrawLink(e);
			if(li.InnerViewInfo != null) {
				li.InnerViewInfo.SetBackBrush(li.GetBackBrush(e.Cache, li.InnerViewInfo.Bounds));
				li.InnerViewInfo.DrawNormalGlyphInCheckedState = true;
				li.InnerViewInfo.Painter.DrawLink(new BarLinkPaintArgs(e.Cache, li.InnerViewInfo, e.SourceInfo));
				li.InnerViewInfo.DrawNormalGlyphInCheckedState = false;
			}
			DrawQLinkFrame(e);
		}
		protected virtual void DrawQLinkFrame(BarLinkPaintArgs e) {
			if(e.LinkInfo.CalcRealPaintState() == BarLinkState.Highlighted) {
				Rectangle r = e.LinkInfo.SelectRect;
				PPainter.DrawLinkHighlightedFrame(e, ref r, BarLinkEmptyBorder.None);
			}
		}
		protected override void DrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			BarQMenuCustomizationLinkViewInfo li = e.LinkInfo as BarQMenuCustomizationLinkViewInfo;
			Rectangle savedBounds = li.Rects[BarLinkParts.Glyph];
			try {
				if(li.InnerViewInfo != null) {
					if(li.IsRightToLeft) {
						li.Rects.Offset(BarLinkParts.Glyph,  li.GlyphWidth, 0);
					}
					li.Rects.SetWidth(BarLinkParts.Glyph, li.GlyphWidth - DrawParameters.Constants.SubMenuQGlyphGlyphIndent);
				}
				base.DrawLinkGlyphInMenu(e, state);
			}
			finally {
			   li.Rects[BarLinkParts.Glyph] = savedBounds;
			}
		}
	}
	public class  BarQBarCustomizationLinkPainter : BarCustomContainerLinkPainter { 
		public BarQBarCustomizationLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkAdornments(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r, m;
			if(ar.IsEmpty) return;
			r = ar;
			bool isVertical = (e.LinkInfo.DrawMode & BarLinkDrawMode.Vertical) != 0;
			bool drawExpand = (e.LinkInfo as BarQBarCustomizationLinkViewInfo).DrawExpandMark;
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			if(isVertical) {
				r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2 - 1;
				r.X += 3;
				m = r;
				m.X = ar.Right - 8;
			} else {
				r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) - 4;
				r.X += (ar.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
				m = r;
				m.X --;
				m.Y = ar.Top + 4;
			}
			PPainter.DrawArrow(e, PPainter.CalcLinkForeBrush(e, drawState), r.Location, r.Width, isVertical ? MarkArrowDirection.Left: MarkArrowDirection.Down);
			if(drawExpand) {
				PPainter.DrawMark(e, SystemPens.ControlText, m.Location, r.Width, !isVertical ? MarkArrowDirection.Right: MarkArrowDirection.Down);
			}
		}
	}
	public class BarMdiButtonLinkPainter : BarLargeButtonLinkPainter {
		public BarMdiButtonLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
	}
	public class PaintHelper {
		protected static XPaint Paint { get { return XPaint.Graphics; } }
		public static bool IsSystemControl(Color color) {
			return color.ToKnownColor() == SystemColors.Control.ToKnownColor();
		}
		public static Color Light(Color color) {
			if(IsSystemControl(color)) return SystemColors.ControlLight;
			return ControlPaint.Light(color, 0.5f);
		}
		public static Color RealLight(Color color) {
			return ControlPaint.Light(color, 0.5f);
		}
		public static Color LightLight(Color color) {
			if(IsSystemControl(color)) return SystemColors.ControlLightLight;
			return ControlPaint.Light(color, 1.0f);
		}
		public static Color Dark(Color color) {
			if(IsSystemControl(color)) return SystemColors.ControlDark;
			return ControlPaint.Dark(color, 0.5f);
		}
		public static Color RealDark(Color color) {
			return ControlPaint.Dark(color, 0.5f);
		}
		public static Color DarkDark(Color color) {
			if(IsSystemControl(color)) return SystemColors.ControlDarkDark;
			return ControlPaint.Dark(color, 1.0f);
		}
		public static Color Light(Brush brush) { return Light(GetBrushColor(brush)); }
		public static Color LightLight(Brush brush) { return LightLight(GetBrushColor(brush)); }
		public static Color Dark(Brush brush) { return Dark(GetBrushColor(brush)); }
		public static Color DarkDark(Brush brush) { return DarkDark(GetBrushColor(brush)); }
		public static Color GetBrushColor(Brush brush) {
			SolidBrush sb = brush as SolidBrush;
			if(sb != null) return sb.Color;
			LinearGradientBrush lb = brush as LinearGradientBrush ;
			if(lb != null) return lb.LinearColors[0];
			return SystemColors.Control;
		}
		public static SizeF CalcTextSize(Graphics g, string s, Font font, int maxWidth, StringFormat strFormat, BarItemLink link) {
			if(link != null && link.IsAllowHtmlText) {
				Rectangle rect = StringPainter.Default.Calculate(g, link.Item.Appearance, TextOptions.DefaultOptions, s.Replace("&", ""), link.Bounds).Bounds;
				return CorrectLinkSize(rect.Size);
			}
			if((strFormat.FormatFlags & StringFormatFlags.DirectionVertical) != 0) {
				return g.MeasureString(s, font, maxWidth, strFormat);
			}
			return Paint.CalcTextSize(g, s, font, strFormat, maxWidth);
		}
		static Size CorrectLinkSize(Size size) {
			return new Size(size.Width + 7, size.Height + 2);
		}
		public static SizeF CalcTextSize(Graphics g, string s, Font font, int maxWidth, StringFormat strFormat) {
			return CalcTextSize(g, s, font, maxWidth, strFormat, null);
		}
		public static void DrawString(GraphicsCache cache, string s, Font font, Brush foreBrush, Rectangle r, StringFormat strFormat) {
			if((strFormat.FormatFlags & StringFormatFlags.DirectionVertical) != 0) 
				cache.Graphics.DrawString(s, font, foreBrush, r, strFormat); 
			else
				Paint.DrawString(cache, s, font, foreBrush, r, strFormat);
		}
		public static void FillRectangle(Graphics g, Brush brush, Rectangle r) {
			g.FillRectangle(brush, r);
		}
		public static void DrawLine(Graphics g, Pen pen, Point p1, Point p2) {
			g.DrawLine(pen, p1, p2);
		}
		public static void DrawRectangle(Graphics g, Pen pen, Rectangle r) {
			r.Width--; r.Height--; 
			g.DrawRectangle(pen, r);
		}
		public static void DrawSelectedFrame(Graphics g, ref Rectangle back) {
			for(int n = 0; n < 2; n++) {
				DrawRectangle(g, SystemPens.ControlText, back);
				back.Inflate(-1, -1);
			}
		}
		[ThreadStatic]
		static ImageAttributes imgAttr = new ImageAttributes();
		public static void DrawImageCore2(System.Drawing.Graphics g, Image image, int x, int y, Rectangle srcRect, ImageAttributes attr) {
			if(attr == null) attr = imgAttr;
			g.DrawImage(image, new Point[] { new Point(x, y), new Point(x + srcRect.Width, y), new Point(x, y + srcRect.Height)}, srcRect, GraphicsUnit.Pixel, attr);
		}
		public static void DrawImageCore(System.Drawing.Graphics g, Image image, int x, int y, int width, int height, Rectangle srcRect, ImageAttributes attr) {
			if(attr == null) attr = imgAttr;
			g.DrawImage(image, new Point[] { new Point(x, y), new Point(x + width, y), new Point(x, y + height)}, srcRect, GraphicsUnit.Pixel, attr);
		}
		[ThreadStatic]
		static ImageAttributes grayAttributes = null, disabledAttributes = null, lighterAttributes = null, whiteAttributes, ribbonDisabledAttributes;
		public static ImageAttributes GrayAttributes {
			get {
				if(grayAttributes == null) InitGrayAttributes();
				return grayAttributes;
			}
		}
		public static ImageAttributes WhiteAttributes {
			get {
				if(whiteAttributes == null) InitGrayAttributes();
				return whiteAttributes;
			}
		}
		public static ImageAttributes LighterAttributes {
			get {
				if(lighterAttributes == null) InitGrayAttributes();
				return lighterAttributes;
			}
		}
		public static ImageAttributes DisabledAttributes {
			get {
				if(disabledAttributes == null) InitGrayAttributes();
				return disabledAttributes;
			}
		}
		public static ImageAttributes RibbonDisabledAttributes {
			get {
				if(ribbonDisabledAttributes == null) InitGrayAttributes();
				return ribbonDisabledAttributes;
			}
		}
		public static void InitGrayAttributes() {
			if(grayAttributes != null) return;
			float[][] array = new float[5][];
			array[0] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[1] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[2] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[3] = new float[5] {0.00f, 0.00f, 0.00f, 1.00f, 0};
			array[4] = new float[5] {0.60f, 0.60f, 0.60f, 0.00f, 1};
			ColorMatrix grayMatrix = new ColorMatrix(array);
			grayAttributes = new ImageAttributes();
			grayAttributes.ClearColorKey();
			grayAttributes.SetColorMatrix(grayMatrix);
			array[0] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[1] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[2] = new float[5] {0.00f, 0.00f, 0.00f, 0.00f, 0};
			array[3] = new float[5] {0.00f, 0.00f, 0.00f, 1.00f, 0};
			array[4] = new float[5] {1.00f, 1.0f, 1.0f, 0.00f, 1};
			grayMatrix = new ColorMatrix(array);
			whiteAttributes = new ImageAttributes();
			whiteAttributes.ClearColorKey();
			whiteAttributes.SetColorMatrix(grayMatrix);
				array[0] = new float[5] { 0.10801f, 0.10801f, 0.10801f, 0, 0 };
				array[1] = new float[5] { 0.21329f, 0.21329f, 0.21329f, 0, 0 };
				array[2] = new float[5] { 0.0287f, 0.0287f, 0.0287f, 0, 0 };
				array[3] = new float[5] {0, 0, 0, 0.9f, 0};
				array[4] = new float[5] {0.5f, 0.5f, 0.5f, 0, 1f};
			ColorMatrix	 cm = new ColorMatrix(array);
			disabledAttributes = new ImageAttributes();
			disabledAttributes.SetColorMatrix( cm );
			cm = new ColorMatrix();
			cm.Matrix00 = 1.0f;
			cm.Matrix11 = 1.0f;
			cm.Matrix22 = 1.0f;
			cm.Matrix33 = 0.65f;	 
			cm.Matrix44 = 1.0f;
			lighterAttributes = new ImageAttributes();
			lighterAttributes.SetColorMatrix( cm );
			ribbonDisabledAttributes = new ImageAttributes();
			ribbonDisabledAttributes.SetColorMatrix(GetSaturationColorMatrix(0.4f));
		}
		static ColorMatrix GetSaturationColorMatrix(float s) {
			float lumR = 0.3086f;
			float lumG = 0.6094f;
			float lumB = 0.0820f;
			float sr = (1 - s) * lumR;
			float sg = (1 - s) * lumG;
			float sb = (1 - s) * lumB;
			ColorMatrix m = new ColorMatrix(
				new float[][] { 
					new float[] { s + sr, sr, sr, 0.0f, 0.0f },
					new float[] { sg, s + sg, sg, 0.0f, 0.0f },
					new float[] { sb, sb, s + sb, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
					new float[] { 0.0f, 0,0f, 0.0f, 0.0f, 1.0f }
				}
			);
			return m;
		}
	}
	public class MenuHeaderPainter : ObjectPainter {
		public virtual void DrawObject(BarLinkPaintArgs e) { }
		public virtual int CalcElementHeight(Graphics g, BarHeaderLinkViewInfo linkInfo, Size captionSize, int res) { return 0; }
	}
	public class SkinMenuHeaderPainter : MenuHeaderPainter {
		public override void DrawObject(BarLinkPaintArgs e) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(BarAndDockingController.Default.LookAndFeel.ActiveLookAndFeel)[RibbonSkins.SkinPopupGalleryGroupCaption], e.LinkInfo.Bounds);
			DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override int CalcElementHeight(Graphics g, BarHeaderLinkViewInfo linkInfo, Size captionSize, int res) {
			Skin skin = RibbonSkins.GetSkin(BarAndDockingController.Default.LookAndFeel.ActiveLookAndFeel);
			SkinElement element = skin[RibbonSkins.SkinPopupGalleryGroupCaption];
			SkinElementInfo info = new SkinElementInfo(element, linkInfo.Bounds);
			return ObjectPainter.CalcBoundsByClientRectangle(g, SkinElementPainter.Default, info, new Rectangle(0, 0, captionSize.Width, res)).Height;
		}
	}
	public class PrimitivesPainter { 
		BarManagerPaintStyle paintStyle;
		ObjectPainter borderPainter;
		MenuHeaderPainter headerPainter;
		public PrimitivesPainter(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
			this.borderPainter = CreateDefaultLinkBorderPainter();
			this.headerPainter = CreateDefaultMenuHeaderPainter();
		}
		protected virtual ObjectPainter CreateDefaultLinkBorderPainter() {
			return new LinkOffice2000BorderPainter();
		}
		protected virtual MenuHeaderPainter CreateDefaultMenuHeaderPainter() {
			return new MenuHeaderPainter();
		}
		public virtual ObjectPainter DefaultLinkBorderPainter { get { return borderPainter; } }
		public virtual MenuHeaderPainter DefaultMenuHeaderPainter { get { return headerPainter; } }
		protected BarManager GetManager(CustomViewInfo viewInfo) { return viewInfo.Manager; } 
		public virtual BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public virtual BarDrawParameters DrawParameters { get { return PaintStyle.DrawParameters; } }
		protected bool IsBlackSkin(RibbonControl ribbon) { return ribbon.ViewInfo.Provider.SkinName == "Black" || ribbon.ViewInfo.Provider.SkinName == "Office 2007 Black"; }
		protected Color GetLineColor(RibbonPageCategory category) {
			return IsBlackSkin(category.Ribbon) ? Color.White : Color.Black;
		}
		protected Color GetLineColor(RibbonPage page) {
			return IsBlackSkin(page.Ribbon)? Color.White: Color.Black;
		}
		protected Color GetLineColor(BarItemLink link) {
			if(link.Ribbon == null || !link.Ribbon.Toolbar.ItemLinks.Contains(link)) return Color.Black;
			return IsBlackSkin(link.Ribbon) ? Color.White : Color.Black;
		}
		public virtual void DrawRibbonLinkDropMark(Graphics g, BarItemLink link, LinkDropTargetEnum dropTarget) {
			Color color = GetLineColor(link);
			if(link.IsLinkInMenu && link.LinkViewInfo.DrawMode != BarLinkDrawMode.InMenuGallery) {
				DrawVerticalDropMark(g, link.Bounds, dropTarget, color);
			}
			else {
				DrawHorizontalDropMark(g, link.Bounds, dropTarget, color);		
			}
		}
		public virtual void DrawVerticalDropMark(Graphics g, Rectangle bounds, LinkDropTargetEnum dropTarget, Color color) {
			if(dropTarget == LinkDropTargetEnum.After)
				DrawDropMark(g, bounds, DropMarkLocation.Bottom, color);
			else
				DrawDropMark(g, bounds, DropMarkLocation.Top, color);
		}
		public virtual void DrawHorizontalDropMark(Graphics g, Rectangle bounds, LinkDropTargetEnum dropTarget, Color color) {
			if(dropTarget == LinkDropTargetEnum.After)
				DrawDropMark(g, bounds, DropMarkLocation.Right, color);
			else
				DrawDropMark(g, bounds, DropMarkLocation.Left, color);
		}
		public virtual void DrawRibbonCategoryDropMark(Graphics g, RibbonPageCategoryViewInfo categoryInfo, LinkDropTargetEnum dropTarget) {
			DrawHorizontalDropMark(g, categoryInfo.Bounds, dropTarget, GetLineColor(categoryInfo.Category));
		}
		public virtual void DrawRibbonPageDropMark(Graphics g, RibbonPageViewInfo pageInfo, LinkDropTargetEnum dropTarget) {
			DrawHorizontalDropMark(g, pageInfo.Bounds, dropTarget, GetLineColor(pageInfo.Page));
		}
		public virtual void DrawRibbonPageGroupDropMark(Graphics g, RibbonPageGroupViewInfo pageInfo, LinkDropTargetEnum dropTarget) {
			DrawHorizontalDropMark(g, pageInfo.Bounds, dropTarget, Color.Black);
		}
		public virtual void DrawGalleryItemDropMark(Graphics g, GalleryItemViewInfo item, LinkDropTargetEnum dropTarget) {
			DrawHorizontalDropMark(g, item.Bounds, dropTarget, Color.Black);
		}
		public virtual void DrawGalleryItemGroupDropMark(Graphics g, GalleryItemGroupViewInfo group, LinkDropTargetEnum dropTarget) {
			DrawVerticalDropMark(g, group.Bounds, dropTarget, Color.Black);
		}
		public virtual void DrawDropMark(Graphics g, Rectangle r, DropMarkLocation location, Color color) {
			if(location == DropMarkLocation.None) return;
			int x = r.X + 3, y = r.Y + 3;
			using(Pen pen = new Pen(color)) {
				if(location == DropMarkLocation.Left || location == DropMarkLocation.Right) {
					if(location == DropMarkLocation.Right) x = r.Right - 3;
					g.DrawLine(pen, new Point(x, r.Y), new Point(x, r.Bottom - 1));
					g.DrawLine(pen, new Point(x - 2, r.Y), new Point(x + 2, r.Y));
					g.DrawLine(pen, new Point(x - 2, r.Bottom - 1), new Point(x + 2, r.Bottom - 1));
				}
				else {
					if(location == DropMarkLocation.Bottom) y = r.Bottom - 3;
					g.DrawLine(pen, new Point(r.X, y), new Point(r.Right - 1, y));
					g.DrawLine(pen, new Point(r.X, y - 2), new Point(r.X, y + 2));
					g.DrawLine(pen, new Point(r.Right - 1, y - 2), new Point(r.Right - 1, y + 2));
				}
			}
		}
		public virtual void DrawBarLinkDropMark(GraphicsInfoArgs e, BarLinkViewInfo li) {
			if(li.Link.Ribbon != null || li.Link != GetManager(li).SelectionInfo.DropSelectedLink) return;
			LinkDropTargetEnum markStyle = GetManager(li).SelectionInfo.DropSelectStyle;
			if(markStyle == LinkDropTargetEnum.None) return;
			Rectangle r = li.Bounds;
			Point[] p = new Point[4];
			if((li.IsLinkInMenu && li.DrawMode != BarLinkDrawMode.InMenuGallery) || li.IsDrawVerticalRotated || li.BarControlInfo.IsVertical) {
				if(markStyle == LinkDropTargetEnum.After)
					r.Y = r.Bottom - 7;
				p[0] = new Point(r.X, r.Y);
				p[1] = new Point(r.X + 3, r.Y + 3);
				p[2] = new Point(r.X + 3, r.Y + 4);
				p[3] = new Point(r.X, r.Y + 7);
				e.Graphics.FillPolygon(SystemBrushes.ControlText, p);
				p[3].X = p[0].X = r.Right;
				p[2].X  = p[1].X = r.Right - 3;
				e.Graphics.FillPolygon(SystemBrushes.ControlText, p);
				r.Height = 2;
				r.Width -= 6;
				r.Y += 3;
				r.X += 3;
			} else {
				if(markStyle == LinkDropTargetEnum.After)
					r.X = r.Right - 4;
				Rectangle r2 = r;
				r2.Height = 2;
				r2.Width = 4;
				PaintHelper.FillRectangle(e.Graphics, SystemBrushes.ControlText, r2);
				r2.Y = r.Bottom - 2;
				PaintHelper.FillRectangle(e.Graphics, SystemBrushes.ControlText, r2);
				r.Width = 2;
				r.Height -= 4;
				r.X += 1;
				r.Y += 2;
			}
			PaintHelper.FillRectangle(e.Graphics, SystemBrushes.ControlText, r);
		}
		public virtual void DrawButtonLinkArrowAdormentsInMenu(BarLinkPaintArgs e, BarLinkState drawState, Brush brush) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			r = ar;
			r.Width = DrawParameters.Constants.SubMenuRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			if(e.LinkInfo.AllowDrawBackground && drawState != BarLinkState.Normal)
				DrawLinkHighLightedBackground(e, ar, BarLinkEmptyBorder.None, e.LinkInfo.CalcRealPaintState(), brush);
			DrawArrow(e, CalcLinkForeBrush(e, drawState), r.Location, r.Width, MarkArrowDirection.Right);
			if(drawState == BarLinkState.Normal) {
				ar.Width = 1;
				PaintHelper.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarColor.SubMenuSeparatorColor), ar);
			}
		}
		public virtual void DrawSeparator(GraphicsInfoArgs e, RectInfo info, BarLinkViewInfo li, object sourceInfo) {
			Rectangle r = info.Rect;
			int th = li.DrawParameters.Constants.BarSeparatorLineThickness;
			bool vert = info.Type == RectInfoType.VertSeparator;
			int width = (vert ? info.Rect.Width : info.Rect.Height);
			int indt = (width - th) / 2;
			if(vert) {
				r.Width = th;
				r.X += indt;
			} else {
				r.Height = th;
				r.Y += indt;
			}
			Brush darkBrush = SystemBrushes.ControlDark;
			PaintHelper.FillRectangle(e.Graphics, darkBrush, r);
		}
		public virtual void DrawSubmenuSeparator(GraphicsInfoArgs e, Rectangle rect, BarLinkViewInfo li, object sourceInfo) {
			CustomSubMenuBarControlViewInfo menuInfo = sourceInfo as CustomSubMenuBarControlViewInfo;
			if(menuInfo == null) return;
			Rectangle r = rect;
			r.Width = menuInfo.GlyphWidth;
			Brush backBrush;
			if(IsAllowDrawSideStrip(li)) {
				if(li.IsRecentLink)
					backBrush = menuInfo.AppearanceMenu.SideStrip.GetBackBrush(e.Cache, r);
				else
					backBrush = menuInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, r);
				PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			}
			backBrush = li.GetBackBrush(e.Cache);
			r.Y += 1;
			r.X = r.Right;
			r.Width = rect.Right - r.Left;
			r.Height = 1;
			PaintHelper.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarColor.SubMenuSeparatorColor), r);
		}
		public virtual bool IsExistsLinkImage(BarLinkViewInfo linkInfo) {
			if(linkInfo == null) return false;
			if(linkInfo.Link.Glyph != null) return true;
			if(linkInfo.DrawMode == BarLinkDrawMode.InMenuLarge || linkInfo.DrawMode == BarLinkDrawMode.InMenuLargeWithText) {
				if(ImageCollection.IsImageListImageExists(linkInfo.Link.Item.Manager.LargeImages, linkInfo.Link.Item.LargeImageIndex)) return true;
				return linkInfo.Link.Item.LargeGlyph != null;
			}
			return ImageCollection.IsImageListImageExists(linkInfo.Link.Item.Manager.Images, linkInfo.Link.ImageIndex);
		}
		public virtual void DrawFlatFrame(GraphicsInfoArgs e, Rectangle rect, Color backColor, bool downed) { }
		public virtual Rectangle CalcInMenuGlyphRect(BarLinkPaintArgs e) { return e.LinkInfo.GlyphRect; }
		public virtual void DrawLinkCheckInMenu(BarLinkPaintArgs e, BarLinkState state, Brush backBrush, BarLinkPainter painter) {
			Rectangle r = e.LinkInfo.GlyphRect;
			if(backBrush != null) {
				DrawLinkHighlightedFrame(e, ref r, BarLinkEmptyBorder.None);
				PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			}
			else {
				Color clr = PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache));
				DrawFlatFrame(e, CalcInMenuGlyphRect(e), clr, (e.LinkInfo.LinkState & BarLinkState.Checked) != 0);  
			}
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph) || !IsExistsLinkImage(e.LinkInfo)) 
				DrawLinkCheckMark(e, e.LinkInfo.GlyphRect, state);
			else
				painter.DrawLinkNormalGlyph(e, false);
		}
		public virtual void DrawLinkImage(BarLinkPaintArgs e, Rectangle r, ImageAttributes attr, Image image, BarLinkState state) {
			if(image == null) image = e.LinkInfo.GetLinkImage(state);
			if(image == null) return;
			Size linkImageSize = CalcLinkImageSize(e.LinkInfo, image);
			ImageLayoutMode mode = e.LinkInfo.DrawParameters.ScaleImages ? ImageLayoutMode.ZoomInside : ImageLayoutMode.Squeeze;
			Rectangle rect = ImageLayoutHelper.GetImageBounds(r, linkImageSize, mode);
			PaintHelper.DrawImageCore(e.Graphics, image, rect.X, rect.Y, rect.Width, rect.Height, new Rectangle(Point.Empty, image.Size), attr);
		}
		protected Size CalcLinkImageSize(BarLinkViewInfo linkInfo, Image image) {
			if(image == null) return Size.Empty;
			Size calcSize = linkInfo.UpdateGlyphSize(linkInfo.GlyphSize, false), size = image.Size;
			if(calcSize != size) {
				size = calcSize;
			}
			return linkInfo.ScaleSize(size);
		}
		public virtual void DrawLinkNormalBackground(BarLinkPaintArgs e, Brush brush, Rectangle r) {
			if(e.LinkInfo.Link.Item.Appearance.Options.UseBackColor)
				e.Cache.FillRectangle(brush, r);
		}
		public virtual void DrawLinkCheckedInGalleryMenu(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			DrawLinkHighLightedBackground(e, r, border, realState, backBrush);
		}
		public virtual void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			DrawLinkHighlightedFrame(e, ref r, border);
			PaintHelper.FillRectangle(e.Graphics, backBrush, r);
		}
		public virtual void DrawLinkHighlightedFrame(BarLinkPaintArgs e, ref Rectangle r, BarLinkEmptyBorder border) {
			Pen pen = DrawParameters.Colors.Pens(BarColor.LinkHighlightBorderColor);
			if(border == BarLinkEmptyBorder.None || border == BarLinkEmptyBorder.All) { 
				if(border == BarLinkEmptyBorder.All) pen = DrawParameters.Colors.Pens(BarColor.LinkBorderColor);
				PaintHelper.DrawRectangle(e.Graphics, pen, r);
				r.Inflate(-1, -1);
				return;
			}
			pen = DrawParameters.Colors.MenuAppearance.Menu.GetBorderPen(e.Cache);
			if(border != BarLinkEmptyBorder.Left)
				PaintHelper.DrawLine(e.Graphics, pen, r.Location, new Point(r.X, r.Bottom - 1));
			if(border != BarLinkEmptyBorder.Right)
				PaintHelper.DrawLine(e.Graphics, pen, new Point(r.Right - 1, r.Y), new Point(r.Right - 1, r.Bottom - 1));
			if(border != BarLinkEmptyBorder.Up)
				PaintHelper.DrawLine(e.Graphics, pen, r.Location, new Point(r.Right - 1, r.Y));
			if(border != BarLinkEmptyBorder.Down)
				PaintHelper.DrawLine(e.Graphics, pen, new Point(r.X, r.Bottom - 1), new Point(r.Right - 1, r.Bottom - 1));
			if(border != BarLinkEmptyBorder.Up) {
				r.Y ++;
				r.Height --;
			} 
			if(border != BarLinkEmptyBorder.Down) r.Height --;
			if(border != BarLinkEmptyBorder.Left) { 
				r.X ++;
				r.Width --;
			}
			if(border != BarLinkEmptyBorder.Right) r.Width --;
		}
		public void DrawLinkBackgroundInMenu(BarLinkPaintArgs e) {
			DrawLinkBackgroundInMenuCore(e);
			if(IsAllowDrawSideStrip(e.LinkInfo))
				DrawLinkSideStrip(e);
		}
		protected virtual bool IsAllowDrawSideStrip(BarLinkViewInfo linkInfo) {
			if(linkInfo.Link.Item.ItemInMenuAppearance.GetAppearance(linkInfo.LinkState).BackColor != Color.Empty)
				return false;
			return (linkInfo.DrawMode != BarLinkDrawMode.InMenuLarge && linkInfo.DrawMode != BarLinkDrawMode.InMenuLargeWithText && linkInfo.DrawMode != BarLinkDrawMode.InMenuGallery);
		}
		protected virtual void DrawLinkBackgroundInMenuCore(BarLinkPaintArgs e) { }
		protected virtual void DrawLinkSideStrip(BarLinkPaintArgs e) {
			Brush backBrush;
			Rectangle r = e.LinkInfo.SelectRect;
			r.Width = e.LinkInfo.GlyphRect.Width;
			if(r.Width != 0) {
				r.Width = e.LinkInfo.GlyphRect.Width + DrawParameters.Constants.SubMenuGlyphHorzIndent + DrawParameters.Constants.SubMenuGlyphCaptionIndent;
				if(e.LinkInfo.IsRecentLink)
					backBrush = e.LinkInfo.AppearanceMenu.SideStrip.GetBackBrush(e.Cache, r);
				else
					backBrush = e.LinkInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, r);
				PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			}
			r.X = r.Right;
			r.Width = e.LinkInfo.SelectRect.Right - r.X;
		}
		protected virtual Pen GetLinkCheckPen(BarLinkPaintArgs e, BarLinkState state) {
			return SystemPens.ControlText;
		}
		public virtual void DrawLinkCheckMark(BarLinkPaintArgs e, Rectangle r, BarLinkState state) {
			int x = r.X + r.Width / 2 - 5;
			int y = r.Y + r.Height / 2 - 3;
			for(int n = 0; n < 2; n++)
				DrawCheckMark(e, GetLinkCheckPen(e, state), x, y, 1, n);
		}
		void DrawCheckMark(GraphicsInfoArgs e, Pen pen, int x, int y, int doubleSize, int offsetY) {
			Point[] p = new Point[3] {
										 new Point(x + doubleSize * 1, y + offsetY + doubleSize * 2),
										 new Point(x + doubleSize * 3, y + offsetY + doubleSize * 4),
										 new Point(x + doubleSize * 8, y + offsetY - doubleSize * 1)};
			e.Graphics.DrawLines(pen, p);
		}
		public virtual void DrawMark(GraphicsInfoArgs e, Pen pen, Point cp, int size, MarkArrowDirection arrow) {
			Point[] p = new Point[3];	
			int d = 2, xx = 2;
			switch(arrow) {
				case MarkArrowDirection.Down:
					p[0] = new Point(cp.X, cp.Y + d - xx);
					p[1] = new Point(cp.X + size / 2, cp.Y + size - d + 1 - xx);
					p[2] = new Point(cp.X + size - 1, cp.Y + d - xx);
					for(int n = 0; n < 4; n++) {
						e.Graphics.DrawLines(pen ,p); 
						for(int k = 0; k < 3; k++) 
							p[k].Y += (n != 1 ? 1 : 2);
					}
					break;
				case MarkArrowDirection.Right:
					p[0] = new Point(cp.X + d - xx, cp.Y);
					p[1] = new Point(cp.X + size - d + 1 - xx, cp.Y + size / 2);
					p[2] = new Point(cp.X + d - xx, cp.Y + size - 1);
					for(int n = 0; n < 4; n++) {
						e.Graphics.DrawLines(pen ,p); 
						for(int k = 0; k < 3; k++) 
							p[k].X += (n != 1 ? 1 : 3);
					}
					break;
			}
		}
		public virtual void DrawArrow(GraphicsInfoArgs e, Brush brush, Point cp, int size, MarkArrowDirection arrow) {
			Point[] p = new Point[3];	
			int d = 2;
			int m = 0;
			if(size < 6) m = 1;
			switch(arrow) {
				case MarkArrowDirection.Down:
					p[0] = new Point(cp.X, cp.Y + d);
					p[1] = new Point(cp.X + size, cp.Y + d);
					p[2] = new Point(cp.X + size / 2, cp.Y + size - d + 2);
					break;
				case MarkArrowDirection.Up:
					size++;
					p[0] = new Point(cp.X, cp.Y + size - d + 2);
					p[1] = new Point(cp.X + size, cp.Y + size - d + 2);
					p[2] = new Point(cp.X + size / 2, cp.Y + d);
					break;
				case MarkArrowDirection.Left:
					p[0] = new Point(cp.X + size - d + 1, cp.Y - m);
					p[1] = new Point(cp.X + d - m, cp.Y + size / 2 + 1);
					p[2] = new Point(cp.X + size - d + 1, cp.Y + size + 1 + m);
					break;
				case MarkArrowDirection.Right:
					p[0] = new Point(cp.X + d, cp.Y - m);
					p[1] = new Point(cp.X + size - d + 1, cp.Y + size / 2 + 1);
					p[2] = new Point(cp.X + d, cp.Y + size + 1 + m);
					break;
			}
			e.Graphics.FillPolygon(brush,p); 
		}
		public virtual Brush CalcLinkForeBrush(BarLinkPaintArgs e, BarLinkState state) {
			Brush itemBrush = e.LinkInfo.GetForeBrush(e.Cache, state);
			if(e.LinkInfo.ShouldDrawCheckedCaption(e.LinkInfo, state))
				itemBrush = e.Cache.GetSolidBrush(e.LinkInfo.DrawParameters.GetInStatusBarLinkForeColorChecked(e.LinkInfo, BarLinkState.Checked));
			return itemBrush;
		}
		public virtual void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			Rectangle r = e.LinkInfo.SelectRect;
			DrawLinkHighlightedFrame(e, ref r, border);
			if(e.LinkInfo.DrawDisabled) backBrush = e.LinkInfo.GetBackBrush(e.Cache, r);
			PaintHelper.FillRectangle(e.Graphics, backBrush, r);
		}
		public virtual void DrawLinkCaption(BarLinkPaintArgs e, Brush foreBrush, BarLinkState state) {
			PaintHelper.DrawString(e.Cache, e.LinkInfo.DisplayCaption, e.LinkInfo.GetFontByState(state), 
				foreBrush, e.LinkInfo.CaptionRect, e.LinkInfo.LinkCaptionStringFormat);
		}
		protected virtual void DrawLinkDescriptionCore(BarLinkPaintArgs e, BarLinkState state, Rectangle bounds, string text, Brush foreBrush) {
			e.LinkInfo.AppearanceMenuItemDescription.DrawString(e.Cache, text, bounds, foreBrush);
		}
		public virtual void DrawLinkDescription(BarLinkPaintArgs e, BarLinkState state) {
			Rectangle bounds = e.LinkInfo.Rects[BarLinkParts.Description];
			string text = e.LinkInfo.Link.Item.Description;
			if(bounds.IsEmpty || text == string.Empty) return;
			Brush foreBrush = e.LinkInfo.AppearanceMenuItemDescription.GetForeBrush(e.Cache);
			DrawLinkDescriptionCore(e, state, bounds, text, foreBrush);
		}
		public virtual void DrawSplitLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			DrawLinkHighlightedBackgroundInMenu(e, border, backBrush, drawState);
		}
	}
	public class LinkOffice2000BorderPainter : SimpleBorderPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			e.Cache.Paint.DrawRectangle(e.Graphics, SystemPens.ControlDark, r);
		}
	}
	public class LinkOffice2003BorderPainter : SimpleBorderPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			e.Cache.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(Office2003Colors.Default[Office2003Color.LinkBorder]), r);
		}
	}
}
