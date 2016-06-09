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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public static class DrawingLocker {
		[System.Runtime.InteropServices.DllImport("user32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private extern static IntPtr SendMessage(IntPtr hWnd,
				int msg, int wParam, IntPtr lParam);
		private const int WM_SETREDRAW = 11;
		[System.Security.SecuritySafeCritical]
		public static void LockDrawing(IntPtr Handle) {
			SendMessage(Handle, WM_SETREDRAW, 0, IntPtr.Zero);
		}
		[System.Security.SecuritySafeCritical]
		public static void UnlockDrawing(IntPtr Handle) {
			SendMessage(Handle, WM_SETREDRAW, 1, IntPtr.Zero);
		}
	}
	public static class GraphicsParamsInfo {
		static InterpolationMode interpolationMode;
		static SmoothingMode smoothingMode;
		static PixelOffsetMode pixelOffsetMode;
		static CompositingQuality compositingQuality;
		static TextRenderingHint textRenderingHint;
		public static void OptimizeGraphics(Graphics g) {
			interpolationMode = g.InterpolationMode;
			smoothingMode = g.SmoothingMode;
			pixelOffsetMode = g.PixelOffsetMode;
			compositingQuality = g.CompositingQuality;
			textRenderingHint = g.TextRenderingHint;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.PixelOffsetMode = PixelOffsetMode.Half;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
		}
		public static void RestoreGraphicsParams(Graphics g) {
			g.InterpolationMode = interpolationMode;
			g.SmoothingMode = smoothingMode;
			g.PixelOffsetMode = pixelOffsetMode;
			g.CompositingQuality = compositingQuality;
			g.TextRenderingHint = textRenderingHint;
		}
	}
	public class AccordionControlPainter : BaseControlPainter {
		enum DrawingOptions {
			PRF_CHECKVISIBLE = 1,
			PRF_NONCLIENT = 2,
			PRF_CLIENT = 4,
			PRF_ERASEBKGND = 8,
			PRF_CHILDREN = 16,
			PRF_OWNED = 32
		}
		public static Image GetControlImage(Control ctrl) {
			if(ctrl.Width == 0 || ctrl.Height == 0)
				return null;
			ctrl.Visible = true;
			Image image = (Image)new Bitmap(ctrl.Width, ctrl.Height);
			using(Graphics g = Graphics.FromImage(image)) {
				ScreenCaptureHelper.PrintRecursiveControl(ctrl, g);
				AccordionContentContainer cont = ctrl as AccordionContentContainer;
				if(cont != null) cont.DrawScrollBars(g);
			}
			return image;
		}
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) { }
		protected override void DrawAdornments(ControlGraphicsInfoArgs info) { }
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			AccordionControlViewInfo vi = info.ViewInfo as AccordionControlViewInfo;
			DrawBackground(info, vi);
			if(vi.IsInMinimizeAnimation) return;
			if(vi.ShouldOptimizeAnimation) GraphicsParamsInfo.OptimizeGraphics(info.Graphics);
			DrawGroups(info);
			DrawElementDropMark(info, vi);
			if(vi.ShouldOptimizeAnimation) GraphicsParamsInfo.RestoreGraphicsParams(info.Graphics);
		}
		protected virtual void DrawBackground(ControlGraphicsInfoArgs info, AccordionControlViewInfo vi) {
			Rectangle bounds = info.Bounds;
			bounds.Y += vi.AnimationAddedTop;
			bounds.Height += vi.AddedAnimationHeight + Math.Abs(vi.AnimationAddedTop);
			info.Cache.FillRectangle(new SolidBrush(vi.GetBackColor()), bounds);
		}
		private void DrawGroups(ControlGraphicsInfoArgs info) {
			AccordionControlViewInfo vi = (AccordionControlViewInfo)info.ViewInfo;
			foreach(AccordionGroupViewInfo groupInfo in vi.ElementsInfo) {
				if(ShouldDrawGroup(info, groupInfo)) continue;
				DrawElement(info, groupInfo);
				if(vi.IsMinimized && groupInfo.Element.GetVisible() && groupInfo.Element.HeaderVisible && groupInfo.HasSeparator)
					DrawSeparator(info, groupInfo);
			}
		}
		protected virtual void DrawSeparator(ControlGraphicsInfoArgs info, AccordionGroupViewInfo groupInfo) {
			Rectangle rect = groupInfo.ControlInfo.GetSeparatorRect(groupInfo);
			SkinElementInfo elem = new SkinElementInfo(groupInfo.ControlInfo.GetSeparatorSkinElement(), rect);
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, elem);
		}
		protected bool ShouldDrawGroup(ControlGraphicsInfoArgs info, AccordionGroupViewInfo groupInfo) {
			return groupInfo.HeaderBounds.Y - groupInfo.ControlInfo.AddedAnimationHeight > info.Bounds.Height;
		}
		protected bool ShouldDrawItem(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo elementInfo) {
			AccordionItemViewInfo itemInfo = elementInfo as AccordionItemViewInfo;
			if(itemInfo != null && itemInfo.ParentElementInfo.IsInAnimation)
				return itemInfo.HeaderBounds.Y - itemInfo.ControlInfo.AddedAnimationHeight > itemInfo.ParentElementInfo.RealHeaderBounds.Bottom + itemInfo.ParentElementInfo.InnerContentHeight;
			return elementInfo.HeaderBounds.Y - elementInfo.ControlInfo.AddedAnimationHeight > info.Bounds.Height;
		}
		protected internal virtual void DrawElementSelection(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo elementInfo) {
			AccordionControlViewInfo vi = (AccordionControlViewInfo)info.ViewInfo;
			if(vi.AccordionControl.KeyNavHelper.SelectedElement == elementInfo.Element)
				info.Paint.DrawFocusRectangle(info.Graphics, elementInfo.HeaderBounds, elementInfo.PaintAppearance.ForeColor, Color.Empty);
			if(!vi.AccordionControl.DesignManager.IsComponentSelected(elementInfo.Element)) return;
			vi.AccordionControl.DesignManager.DrawSelection(info.Cache, elementInfo.RealHeaderBounds, elementInfo.PaintAppearance.BackColor);
		}
		protected internal virtual void DrawContextButtons(GraphicsCache cache, AccordionElementBaseViewInfo itemInfo) {
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(itemInfo.ContextButtonsViewInfo, cache, itemInfo.HeaderContentBounds));
		}
		protected internal virtual void DrawImage(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.Image == null) return;
			Image img = elementInfo.GetImage();
			if(elementInfo.Element.GetEnabled()) {
				if(elementInfo.Element.GetAllowGlyphSkinning()) {
					ImageAttributes attr = ImageColorizer.GetColoredAttributes(elementInfo.PaintAppearance.ForeColor);
					cache.Graphics.DrawImage(img, elementInfo.ImageBounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
				}
				else cache.Graphics.DrawImage(img, elementInfo.ImageBounds);
			}
			else cache.Graphics.DrawImage(img, elementInfo.ImageBounds, 0, 0, elementInfo.ImageBounds.Width, elementInfo.ImageBounds.Height, GraphicsUnit.Pixel, XPaint.DisabledImageAttr);
		}
		protected void DrawItems(ControlGraphicsInfoArgs info, AccordionGroupViewInfo groupInfo) {
			if(groupInfo.IsInAnimation) {
				Rectangle clipRectangle = groupInfo.HeaderBounds;
				clipRectangle.Height += groupInfo.InnerContentHeight;
				GraphicsClipState savedClip = info.Cache.ClipInfo.SaveAndSetClip(clipRectangle, true, true);
				DrawElements(info, groupInfo);
				info.Cache.ClipInfo.RestoreClip(savedClip);
			}
			else {
				DrawElements(info, groupInfo);
			}
		}
		protected virtual void DrawElements(ControlGraphicsInfoArgs info, AccordionGroupViewInfo groupInfo) {
			if(groupInfo.ElementsInfo == null) return;
			for(int i = 0; i < groupInfo.ElementsInfo.Count; i++) {
				if(!groupInfo.ElementsInfo[i].Element.GetVisible()) continue;
				if(ShouldDrawItem(info, groupInfo.ElementsInfo[i])) break;
				DrawElement(info, groupInfo.ElementsInfo[i]);
			}
		}
		protected void DrawElement(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo is AccordionGroupViewInfo) DrawGroup(info, elementInfo as AccordionGroupViewInfo);
			else DrawItem(info, elementInfo as AccordionItemViewInfo);
			AccordionControlViewInfo vi = (AccordionControlViewInfo)info.ViewInfo;
		}
		protected virtual void DrawItem(ControlGraphicsInfoArgs info, AccordionItemViewInfo itemInfo) {
			if(!itemInfo.Expanded && itemInfo.IsInAnimation && itemInfo.ControlInfo.ShouldDrawClearControl) return;
			if(itemInfo.Element.HeaderVisible) {
				DrawHeader(info, itemInfo);
				if(EnableDrawControlImage(itemInfo.ControlInfo)) {
					if(itemInfo.ParentElementInfo.Expanded || itemInfo.IsInAnimation) DrawHeaderControl(info, itemInfo);
				}
			}
			if(EnableDrawControlImage(itemInfo.ControlInfo)) {
				if(itemInfo.Expanded || (itemInfo.IsInAnimation && !itemInfo.ParentElementInfo.IsInAnimation)) DrawContentContainer(info, itemInfo);
			}
		}
		protected virtual void DrawGroup(ControlGraphicsInfoArgs info, AccordionGroupViewInfo groupInfo) {
			if(!groupInfo.Element.GetVisible()) return;
			if(groupInfo.ParentGroup != null && !groupInfo.ParentGroup.Expanded && groupInfo.ParentGroup.IsInAnimation && groupInfo.ControlInfo.ShouldDrawClearControl)
				return;
			if(groupInfo.Element.HeaderVisible) {
				DrawHeader(info, groupInfo);
				if(!groupInfo.ControlInfo.ShouldDrawClearControl && !groupInfo.ControlInfo.IsMinimized) {
					DrawHeaderControl(info, groupInfo);
				}
			}
			if(groupInfo.ControlInfo.IsMinimized) return;
			if(EnableDrawControlImage(groupInfo.ControlInfo)) {
				if(groupInfo.Expanded || groupInfo.IsInAnimation)
					DrawContentContainer(info, groupInfo);
			}
			if(ShouldDrawItems(groupInfo)) DrawItems(info, groupInfo);
		}
		bool EnableDrawControlImage(AccordionControlViewInfo vi) {
			return !vi.ShouldDrawClearControl && vi.AccordionControl.AnimationType != AnimationType.None;
		}
		protected bool ShouldDrawItems(AccordionGroupViewInfo groupInfo) {
			return (groupInfo.Expanded || groupInfo.IsInAnimation) && groupInfo.Element.Style == ElementStyle.Group && !groupInfo.ControlInfo.IsMinimized;
		}
		protected virtual void DrawHeader(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.HeaderBounds == Rectangle.Empty) return;
			DrawHeaderCore(info.Cache, elementInfo);
			DrawElementSelection(info, elementInfo);
		}
		protected internal virtual void DrawHeaderCore(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo) {
			CustomDrawElementEventArgs custom = new CustomDrawElementEventArgs(cache, elementInfo, this);
			elementInfo.ControlInfo.AccordionControl.RaiseCustomDrawElement(custom);
			if(custom.Handled)
				return;
			DrawHeaderBackground(cache, elementInfo);
			if(elementInfo.ShouldShowExpandButtons)
				DrawExpandCollapseButton(cache, elementInfo);
			DrawImage(cache, elementInfo);
			DrawContextButtons(cache, elementInfo);
			DrawText(cache, elementInfo);
		}
		protected internal virtual void DrawExpandCollapseButton(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.Element.OwnerElement != null && elementInfo.Element.Style == ElementStyle.Item && !elementInfo.Element.HasContentContainer) return;
			if(elementInfo.ControlInfo.UseOffice2003Style) {
				if(ButtonPainter == null) this.buttonPainter =  new AdvExplorerBarOpenCloseButtonObjectPainter();
				Office2003PaintHelper.DrawExpandCollapseButton(cache, elementInfo, ButtonPainter);
				return;
			}
			if(elementInfo.ControlInfo.UseFlatStyle) {
				if(ButtonPainter == null) this.buttonPainter = new ExplorerBarOpenCloseButtonObjectPainter();
				FlatPaintHelper.DrawExpandCollapseButton(cache, elementInfo, ButtonPainter);
				return;
			}
			SkinElementInfo elem = elementInfo.GetExpandCollapseButtonInfo();
			if(elem == null) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elem);
		}
		private void DrawHeaderControl(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo itemInfo) {
			if(itemInfo.Element.HeaderControl == null || itemInfo.Element.HeaderControlImage == null || itemInfo.Element.HeaderControl.Visible) return;
			Rectangle headerBounds = itemInfo.HeaderContentBounds;
			Rectangle destRect = new Rectangle(itemInfo.GetHeaderControlLeft, headerBounds.Y + ((headerBounds.Height - itemInfo.Element.HeaderControl.Height) / 2), itemInfo.Element.HeaderControlImage.Width, itemInfo.Element.HeaderControlImage.Height);
			Rectangle srcRect = new Rectangle(0, 0, itemInfo.Element.HeaderControlImage.Width, itemInfo.Element.HeaderControlImage.Height);
			info.Graphics.DrawImage(itemInfo.Element.HeaderControlImage, destRect, srcRect, GraphicsUnit.Pixel);
		}
		private void DrawContentContainer(ControlGraphicsInfoArgs info, AccordionElementBaseViewInfo itemInfo) {
			if(itemInfo.Element.ContentContainer == null || itemInfo.Element.ContentContainerImage == null) return;
			Rectangle headerBounds = itemInfo.HeaderBounds;
			Rectangle destRect = new Rectangle(headerBounds.X, headerBounds.Bottom, headerBounds.Width, itemInfo.InnerContentHeight);
			Rectangle srcRect = new Rectangle(0, 0, headerBounds.Width, itemInfo.InnerContentHeight);
			info.Graphics.DrawImage(itemInfo.Element.ContentContainerImage, destRect, srcRect, GraphicsUnit.Pixel);
		}
		protected internal virtual void DrawVerticalText(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle rect, int angle) {
			Rectangle fixedRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + 25); 
			appearance.DrawVString(cache, text, appearance.GetFont(), appearance.GetForeBrush(cache), fixedRect, appearance.GetStringFormat(), angle);
		}
		protected internal virtual void DrawText(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.TextBounds.Width == 0 || elementInfo.TextBounds.Height == 0) return;
			if(elementInfo.ControlInfo.IsMinimized) {
				DrawVerticalText(cache, elementInfo.PaintAppearance, elementInfo.DisplayText, elementInfo.TextBounds, 90);
				return;
			}
			if(elementInfo.ControlInfo.AccordionControl.AllowHtmlText) {
				StringPainter.Default.DrawString(cache, elementInfo.PaintAppearance, elementInfo.DisplayText, elementInfo.TextBounds);
				return;
			}
			elementInfo.PaintAppearance.DrawString(cache, elementInfo.DisplayText, elementInfo.TextBounds);
		}
		protected internal virtual void DrawHeaderBackground(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.ControlInfo.UseOffice2003Style) {
				Office2003PaintHelper.DrawHeader(cache.Graphics, elementInfo);
				return;
			}
			if(elementInfo.ControlInfo.UseFlatStyle) {
				FlatPaintHelper.DrawHeader(cache.Graphics, elementInfo);
				return;
			}
			if(elementInfo.PaintAppearance.Options.UseBackColor) {
				cache.FillRectangle(new SolidBrush(elementInfo.PaintAppearance.BackColor), elementInfo.HeaderBounds);
				return;
			}
			SkinElementInfo elem = elementInfo.GetHeaderInfo();
			if(elementInfo.ControlInfo.AccordionControl.IsRightToLeft) elem.RightToLeft = true;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elem);
		}
		protected virtual void DrawElementDropMark(ControlGraphicsInfoArgs info, AccordionControlViewInfo vi) {
			AccordionControlHandler handler = (AccordionControlHandler)vi.AccordionControl.Handler;
			AccordionDropTargetArgs target = handler.DragController.DropTarget;
			if(target == null || !target.CanInsertElement() || !target.CanDrop)
				return;
			if(target.PrevElementInfo != null)
				DrawElementDropMark(info.Cache, target.PrevElementInfo.Element, false);
			else if(target.TargetOwner is AccordionControlElement)
				DrawElementDropMark(info.Cache, (AccordionControlElement)target.TargetOwner, true);
			else if(target.TargetOwner is AccordionControl && vi.AccordionControl.ControlInfo.ElementsInfo.Count > 0)
				DrawElementDropMark(info.Cache, vi.AccordionControl.ControlInfo.ElementsInfo[0].Element, true);
		}
		static readonly Pen DropMarkBorderPen = new Pen(Color.FromArgb(191, 0, 182, 249));
		static readonly Color DropMarkBackColor = Color.FromArgb(38, 0, 182, 249);
		protected virtual void DrawElementDropMark(GraphicsCache cache, AccordionControlElement elem, bool upperPart) {
			if(elem.Bounds.Height == 0) return;
			int top = upperPart ? elem.Bounds.Y : elem.Bounds.Y + elem.Bounds.Height - 4;
			int height = upperPart ? elem.Bounds.Height : 4;
			Rectangle rect = new Rectangle(elem.Bounds.X, top, elem.Bounds.Width, height);
			cache.FillRectangle(DropMarkBackColor, rect);
			cache.DrawRectangle(DropMarkBorderPen, new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
		}
		ButtonObjectPainter buttonPainter;
		protected internal ButtonObjectPainter ButtonPainter {
			get {
				return buttonPainter;
			}
		}
		public static Bitmap CreateExpandButtonBitmap(AccordionControlViewInfo vi) {
			if(vi.ExpandButtonState == ObjectState.Disabled) return null;
			Bitmap bmp = new Bitmap(vi.ExpandButtonRect.Width, vi.ExpandButtonRect.Height);
			using(Graphics g = Graphics.FromImage(bmp)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					DrawAccordionExpandButton(cache, vi);
				}
			}
			return bmp;
		}
		static void DrawAccordionExpandButton(GraphicsCache cache, AccordionControlViewInfo vi) {
			SkinElementInfo button = new SkinElementInfo(vi.GetExpandButtonSkinElement(), new Rectangle(Point.Empty, vi.ExpandButtonRect.Size));
			button.ImageIndex = -1;
			button.State = vi.ExpandButtonState;
			button.GlyphIndex = vi.ShouldRotateExpandButton() ? 0 : 1;
			ObjectPainter.DrawObject(cache, AccordionSkinElementPainter.Default, button);
		}
	}
	public class AccordionControlObjectPainter : ObjectPainter {
		public AccordionControlObjectPainter(AccordionControlViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		public AccordionControlViewInfo ViewInfo { get; private set; }
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			ViewInfo.Painter.Draw(new ControlGraphicsInfoArgs(ViewInfo, e.Cache, e.Bounds));
		}
	}
	public static class Office2003PaintHelper {
		public static Color BackColor {
			get { return Office2003Colors.Default[Office2003Color.NavBarBackColor1]; }
		}
		public static Color HeaderBackColor {
			get { return Office2003Colors.Default[Office2003Color.NavBarGroupCaptionBackColor1]; }
		}
		public static Color HeaderBackColor2 {
			get { return Office2003Colors.Default[Office2003Color.NavBarGroupCaptionBackColor2]; }
		}
		public static Color HeaderForeColor {
			get { return Color.Empty; }
		}
		const int TopIndent = 4;
		const int LeftIndent = 10;
		const int LeftContentIndent = 18;
		public static void DrawHeader(Graphics g, AccordionElementBaseViewInfo vi) {
			Rectangle rect = vi.HeaderBounds;
			rect.Height = Math.Max(0, rect.Height - TopIndent);
			rect.Width = Math.Max(0, rect.Width - 2 * LeftIndent);
			rect.X += LeftIndent;
			rect.Y += TopIndent;
			if(rect.Width == 0 || rect.Height == 0)
				return;
			Region reg = new Region(new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height - 2));
			reg.Union(new Rectangle(rect.X + 2, rect.Y, rect.Width - 4, 1));
			reg.Union(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, 1));
			Brush brush = GetBackBrush(rect, vi.PaintAppearance.BackColor, vi.PaintAppearance.BackColor2);
			g.FillRegion(brush, reg);
			reg.Dispose();
		}
		public static Rectangle CalcContentBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			rect.Height = Math.Max(0, rect.Height - TopIndent);
			rect.Width = Math.Max(0, rect.Width - 2 * LeftContentIndent);
			rect.X += LeftContentIndent;
			rect.Y += TopIndent;
			return rect;
		}
		public static Brush GetBackBrush(Rectangle rect, Color backColor1, Color backColor2) {
			Color c1 = backColor1, c2 = backColor2;
			return new LinearGradientBrush(rect, c1, c2, LinearGradientMode.Horizontal);
		}
		public static void DrawExpandCollapseButton(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo, ButtonObjectPainter painter) {
			ExplorerBarOpenCloseButtonInfoArgs oa = new ExplorerBarOpenCloseButtonInfoArgs(cache, elementInfo.ExpandCollapseButtonBounds, null, elementInfo.State, elementInfo.Expanded);
			oa.BackAppearance.BackColor = elementInfo.IsRootElement ? elementInfo.PaintAppearance.BackColor2 : elementInfo.PaintAppearance.BackColor;
			oa.BackAppearance.ForeColor = elementInfo.PaintAppearance.ForeColor;
			painter.DrawObject(oa);
		}
		public static Size ExpandCollapseButtonSize = new Size(18, 18);
	}
	public static class FlatPaintHelper {
		public static Color BackColor {
			get { return SystemColors.Window; }
		}
		public static Color HeaderBackColor {
			get { return SystemColors.Control; }
		}
		public static Color HeaderForeColor {
			get { return SystemColors.ControlText; }
		}
		const int TopIndent = 4;
		const int LeftIndent = 10;
		const int LeftContentIndent = 18;
		public static void DrawHeader(Graphics g, AccordionElementBaseViewInfo vi) {
			Rectangle rect = vi.HeaderBounds;
			rect.Height = Math.Max(0, rect.Height - TopIndent);
			rect.Width = Math.Max(0, rect.Width - 2 * LeftIndent);
			rect.X += LeftIndent;
			rect.Y += TopIndent;
			Brush brush = new SolidBrush(vi.PaintAppearance.BackColor);
			g.FillRectangle(brush, rect);
		}
		public static Rectangle CalcContentBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			rect.Height = Math.Max(0, rect.Height - TopIndent);
			rect.Width = Math.Max(0, rect.Width - 2 * LeftContentIndent);
			rect.X += LeftContentIndent;
			rect.Y += TopIndent;
			return rect;
		}
		public static void DrawExpandCollapseButton(GraphicsCache cache, AccordionElementBaseViewInfo elementInfo, ButtonObjectPainter painter) {
			ExplorerBarOpenCloseButtonInfoArgs oa = new ExplorerBarOpenCloseButtonInfoArgs(cache, elementInfo.ExpandCollapseButtonBounds, null, elementInfo.State, elementInfo.Expanded);
			oa.BackAppearance.BackColor = elementInfo.PaintAppearance.BackColor;
			oa.BackAppearance.ForeColor = elementInfo.PaintAppearance.ForeColor;
			painter.DrawObject(oa);
		}
		public static Size ExpandCollapseButtonSize = new Size(16, 16);
	}
	public class AccordionSkinElementPainter : SkinElementPainter {
		static SkinElementPainter defaultPainter;
		static AccordionSkinElementPainter() {
			defaultPainter = new AccordionSkinElementPainter();
		}
		public static new SkinElementPainter Default { get { return defaultPainter; } }
		protected override void DrawSkinForeground(SkinElementInfo ee) {
			Color glyphColor = GetGlyphColor(ee);
			ImageAttributes prevAttributes = null;
			if(glyphColor != Color.Empty) {
				if(ee.Attributes != null)
					prevAttributes = ee.Attributes.Clone() as ImageAttributes;
				ee.Attributes = ImageColorizer.GetColoredAttributes(glyphColor);
			}
			try {
				Rectangle glyphRect = ee.Bounds;
				glyphRect.X += ee.Element.ContentMargins.Left;
				glyphRect.Width -= ee.Element.ContentMargins.Width;
				ee.Bounds = glyphRect;
				base.DrawSkinForeground(ee);
			}
			finally {
				if(prevAttributes != null) ee.Attributes = prevAttributes;
			}
		}
		protected Color GetGlyphColor(SkinElementInfo info) {
			Color glyphColor = Color.Empty;
			switch(info.State) {
				case ObjectState.Hot:
					glyphColor = info.Element.Properties.GetColor("GlyphForeColorHot");
					break;
				case ObjectState.Disabled:
					glyphColor = info.Element.Properties.GetColor("GlyphForeColorDisabled");
					break;
				case ObjectState.Selected:
					glyphColor = info.Element.Properties.GetColor("GlyphForeColorSelected");
					break;
				case ObjectState.Pressed:
					glyphColor = info.Element.Properties.GetColor("GlyphForeColorPressed");
					break;
			}
			if(glyphColor != Color.Empty)
				return glyphColor;
			return info.Element.Properties.GetColor("GlyphForeColor");
		}
	}
}
