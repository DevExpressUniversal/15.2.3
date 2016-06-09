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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Ruler;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design {
	public static class FlatPaintHelper {
		public static Color BorderColor = Color.FromArgb(0xac, 0xa8, 0x99);
		static readonly Brush sliderBrush = SystemBrushes.Control;
		public static Brush SliderBrush {
			get { return sliderBrush; }
		}
		public static void DrawBorders(GraphicsCache cache, Rectangle rect, Color color) {
			Brush brush = cache.GetSolidBrush(color);
			cache.FillRectangle(brush, new Rectangle(0, 0, rect.Width, 1));
			cache.FillRectangle(brush, new Rectangle(0, rect.Height - 1, rect.Width, 1));
			cache.FillRectangle(brush, new Rectangle(0, 0, 1, rect.Height));
			cache.FillRectangle(brush, new Rectangle(rect.Width - 1, 0, 1, rect.Height));
		}
	}
	public static class SkinPaintHelper {
		static string[] buttonStrings = new string[] { ReportsSkins.SkinBandButtonLevel0, ReportsSkins.SkinBandButtonLevel1, ReportsSkins.SkinBandButtonLevel2 };
		static string[] captionStrings = new string[] { ReportsSkins.SkinBandHeaderLevel0, ReportsSkins.SkinBandHeaderLevel1, ReportsSkins.SkinBandHeaderLevel2 };
		public static bool RequireHorzOffset(Skin skin) {
			return skin.Properties.GetBoolean("RequireHorzOffset");
		}
		public static bool RequireVertOffset(Skin skin) {
			return skin.Properties.GetBoolean("RequireVertOffset");
		}
		public static SkinElement GetBandCaptionSkinElement(Skin skin, int level, bool selected) {
			string skinElStr = string.Empty;
			skinElStr = selected ? ReportsSkins.SkinBandHeaderSelected : captionStrings[level % 3];
			return skin[skinElStr];
		}
		public static SkinElement GetBandButtonSkinElement(Skin skin, int level, bool selected) {
			string skinElStr = string.Empty;
			skinElStr = selected ? ReportsSkins.SkinBandButtonSelected : buttonStrings[level % 3];
			return skin[skinElStr];
		}
		public static Rectangle OffsetRectangle(Rectangle rect, int dx, int dy) {
			return new Rectangle(new Point(rect.X + dx, rect.Y + dy), rect.Size);
		}
		public static SkinElement GetSkinElement(DevExpress.Skins.ISkinProvider provider, string elementName) {
			Skin skin = ReportsSkins.GetSkin(provider);
			return skin[elementName];
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
	}
	public static class Office2003PaintHelper {
		enum ColorIndexes { Color1 = 0, Color2 = 1, HotTrackedColor1 = 2, HotTrackedColor2 = 3 };
		public static class Colors {
			public static Color ReportBackground { get { return Office2003Colors.Current.ReportBackground; } }
			public static Color RulerBackground { get { return Office2003Colors.Current.RulerBackground; } }
			public static Color CornerPanelOuterBorder { get { return Office2003Colors.Current.CornerPanelOuterBorder; } }
			public static Color CornerPanelInnerBorder { get { return Office2003Colors.Current.CornerPanelInnerBorder; } }
			public static Color RulerRightMargin { get { return Office2003Colors.Current.RulerRightMargin; } }
			public static Color PopupFormBorder { get { return Office2003Colors.Current.PopupFormBorder; } }
			public static Color PopupFormBackground1 { get { return Office2003Colors.Current.PopupFormBackground1; } }
			public static Color PopupFormBackground2 { get { return Office2003Colors.Current.PopupFormBackground2; } }
			public static Color PopupFormCaption1 { get { return Office2003Colors.Current.PopupFormCaption1; } }
			public static Color PopupFormCaption2 { get { return Office2003Colors.Current.PopupFormCaption2; } }
			public static Color PopupFormText { get { return Office2003Colors.Current.PopupFormText; } }
			public static Color ComponentTrayBackground { get { return Office2003Colors.Current.ComponentTrayBackground; } }
			public static Color SliderContentInner { get { return Office2003Colors.Current.SliderContentInner; } }
			public static Color SliderContentOuter { get { return Office2003Colors.Current.SliderContentOuter; } }
			public static Color BandButtonSign { get { return Office2003Colors.Current.BandButtonSign; } }
			public static Color SmartTagBackColor { get { return Office2003Colors.Current.SmartTagBackColor; } }
			static Color[] BandColorsLevel0 { get { return Office2003Colors.Current.BandColorsLevel0; } }
			static Color[] BandColorsLevel1 { get { return Office2003Colors.Current.BandColorsLevel1; } }
			static Color[] BandColorsLevel2 { get { return Office2003Colors.Current.BandColorsLevel2; } }
			public static object[] BandColors { get { return new object[] { BandColorsLevel0, BandColorsLevel1, BandColorsLevel2 }; } }
			public static Color[] SelectedBandColors { get { return Office2003Colors.Current.SelectedBandColors; } }
			static Color[] BandBorderColorsLevel0 { get { return Office2003Colors.Current.BandBorderColorsLevel0; } }
			static Color[] BandBorderColorsLevel1 { get { return Office2003Colors.Current.BandBorderColorsLevel1; } }
			static Color[] BandBorderColorsLevel2 { get { return Office2003Colors.Current.BandBorderColorsLevel2; } }
			public static object[] BandBorderColors { get { return new object[] { BandBorderColorsLevel0, BandBorderColorsLevel1, BandBorderColorsLevel2 }; } }
			public static Color[] SelectedBandBorderColors { get { return Office2003Colors.Current.SelectedBandBorderColors; } }
			static Color[] BandButtonBorderColorsLevel0 { get { return Office2003Colors.Current.BandButtonBorderColorsLevel0; } }
			static Color[] BandButtonBorderColorsLevel1 { get { return Office2003Colors.Current.BandButtonBorderColorsLevel1; } }
			static Color[] BandButtonBorderColorsLevel2 { get { return Office2003Colors.Current.BandButtonBorderColorsLevel2; } }
			public static object[] BandButtonBorderColors { get { return new object[] { BandButtonBorderColorsLevel0, BandButtonBorderColorsLevel1, BandButtonBorderColorsLevel2 }; } }
			public static Color[] SelectedBandButtonBorderColors { get { return Office2003Colors.Current.SelectedBandButtonBorderColors; } }
		}
		public static void DrawBandBorder(GraphicsCache cache, Rectangle rect, int level, bool selected) {
			Color[] colors = GetColorsByState(Colors.BandBorderColors, Colors.SelectedBandBorderColors, level, selected);
			cache.FillRectangle(cache.GetSolidBrush(colors[0]), new Rectangle(rect.X, rect.Y, rect.Width, 1));
			cache.FillRectangle(cache.GetSolidBrush(colors[1]), new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
		}
		public static Brush GetBandButtonBackgroundBrush(BandCaptionViewInfo viewInfo, Rectangle rect) {
			Color[] colors = GetColorsByState(Colors.BandColors, Colors.SelectedBandColors, viewInfo.Level, viewInfo.Selected);
			Color color1 = colors[(int)ColorIndexes.HotTrackedColor1];
			Color color2 = colors[(int)ColorIndexes.Color2];
			return viewInfo.Cache.GetGradientBrush(rect, color1, color2, LinearGradientMode.ForwardDiagonal);
		}
		public static Brush GetSliderBrush(GraphicsCache cache, VRulerSection section) {
			Color[] colors = (Color[])Colors.BandColors[section.Level % 3];
			return GetBrush(cache, colors, false, section.SliderPlaceBounds);
		}
		public static Color[] GetBandButtonBorderColors(BandCaptionViewInfo viewInfo) {
			return GetColorsByState(Colors.BandButtonBorderColors, Colors.SelectedBandButtonBorderColors, viewInfo.Level, viewInfo.Selected);
		}
		static Brush GetBrush(GraphicsCache cache, Color[] colors, bool hotTracked, Rectangle rect) {
			Color color1 = colors[hotTracked ? (int)ColorIndexes.HotTrackedColor1 : (int)ColorIndexes.Color1];
			Color color2 = colors[hotTracked ? (int)ColorIndexes.HotTrackedColor2 : (int)ColorIndexes.Color2];
			return cache.GetGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
		}
		static Color[] GetColorsByState(object[] levelColors, Color[] selectedColors, int level, bool selected) {
			return selected ? selectedColors : ((Color[])levelColors[level % 3]);
		}
	}
	public enum RulerOrientation { Horizontal = 0, Vertical = 1 };
	public class RulerSectionPaintHelper {
		static T FindParentControl<T>(Control control)
			where T : Control {
			while(control != null && !(control is T))
				control = control.Parent;
			return control as T;
		}
		protected virtual int HorzMargins {
			get { return 0; }
		}
		protected virtual int VertMargins {
			get { return 0; }
		}
		public virtual Brush TextBrush {
			get { return null; }
		}
		public virtual Margins ClientMargins {
			get { return new Margins(4, 4, 4, 4); }
		}
		public virtual Margins RulerSectionMargins {
			get { return new Margins(1, 1, 1, 1); }
		}
		public virtual void DrawBackground(RulerViewInfo viewInfo) {
		}
		public virtual void DrawSlider(GraphicsCache cache, VRulerSection section, RulerOrientation orientation) {
		}
		public virtual void FillSectionBackground(RulerViewInfo e, RulerSection section) {
			RulerBase ruler = section.Ruler;
			Rectangle sectionBounds = section.Bounds;
			ClipRulerSectionRect(e, section, ref sectionBounds);
			DrawSection(e.Cache, sectionBounds, e.RulerOrientation);
			DrawSectionBorder(e.Cache, sectionBounds, e.RulerOrientation);
			Rectangle r;
			if(e.RulerOrientation == RulerOrientation.Vertical) {
				VRulerSection vSection = (VRulerSection)section;
				if(vSection.IsExpanded) {
					ReportFrame reportFrame = FindParentControl<ReportFrame>(ruler);
					if(reportFrame != null && vSection.Index < reportFrame.BandViewInfos.Length && vSection.Index >= 0) {
						Band band = reportFrame.BandViewInfos[vSection.Index].Band;
						if(band is TopMarginBand || band is BottomMarginBand)
							DrawHMargin(e.Cache, vSection.Bounds, e.RulerOrientation);
					}
				}
			} else if(e.RulerOrientation == RulerOrientation.Horizontal) {
				r = ((HRuler)e.Ruler).LeftMarginBounds;
				r.Intersect(sectionBounds);
				if(r.Width > 0)
					DrawHMargin(e.Cache, r, e.RulerOrientation);
				r = ((HRuler)e.Ruler).RightMarginBounds;
				r.Intersect(sectionBounds);
				if(r.Width > 0)
					DrawHMargin(e.Cache, r, e.RulerOrientation);
			}
			for(int i = 0; i < ruler.ShadowRects.Length; i++) {
				r = ruler.ShadowRects[i];
				r.Intersect(sectionBounds);
				DrawShadowRect(e.Cache, r, ruler.ShadowLevel, e.RulerOrientation);
			}
			if(ruler.SelectionRect != Rectangle.Empty) {
				r = ruler.SelectionRect;
				r.Intersect(sectionBounds);
				DrawSelectionRect(e.Cache, r, e.RulerOrientation);
			}
		}
		protected virtual void DrawSectionBorder(GraphicsCache graphicsCache, Rectangle sectionBounds, RulerOrientation orientarion) {
		}
		protected virtual void DrawHMargin(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
		}
		protected virtual void DrawShadowRect(GraphicsCache cache, Rectangle rect, int shadowLevel, RulerOrientation orientation) {
		}
		protected virtual void DrawSelectionRect(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
		}
		protected virtual void DrawSection(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
		}
		void ClipRulerSectionRect(RulerViewInfo e, RulerSection section, ref Rectangle rect) {
			ClipHorzRulerSectionRect(e, ref rect);
			ClipVertRulerSectionRect(e, section, ref rect);
		}
		void ClipVertRulerSectionRect(RulerViewInfo e, RulerSection section, ref Rectangle rect) {
			Rectangle rulerBounds = e.Bounds;
			if(e.RulerOrientation == RulerOrientation.Vertical) {
				VRulerSection vRulerSection = (VRulerSection)section;
				rect.Size = new Size(rect.Width, rect.Height - vRulerSection.SliderHeight);
				rect.Offset(0, vRulerSection.SliderHeight);
				rulerBounds.Offset(0, -e.ViewOffset.Y);
				rulerBounds.Inflate(0, -VertMargins);
				rect.Intersect(rulerBounds);
			}
		}
		void ClipHorzRulerSectionRect(RulerViewInfo e, ref Rectangle rect) {
			if(e.RulerOrientation == RulerOrientation.Horizontal) {
				Rectangle rulerBounds = GetRealClientHRulerBounds(e.Bounds, e.ViewOffset);
				rect.Intersect(rulerBounds);
			}
		}
		Rectangle GetRealClientHRulerBounds(Rectangle rulerBounds, Point offset) {
			rulerBounds.Offset(-offset.X, 0);
			rulerBounds.Inflate(-HorzMargins, 0);
			return rulerBounds;
		}
	}
	public class RulerSectionPaintHelperFlat : RulerSectionPaintHelper {
		const int BorderWidth = 1;
		static readonly Brush innerBrush = SystemBrushes.Control;
		static readonly Brush outerBrush = SystemBrushes.ControlDark;
		static readonly Brush contentBrush = SystemBrushes.ControlDark;
		static readonly Brush selectionBrush = new SolidBrush(Color.FromArgb(89, ControlPaintHelper.NormalColor));
		static readonly Brush shadowBrush = selectionBrush;
		static Rectangle CorrectBoundsForRuler(Rectangle rect, RulerOrientation orientation) {
			switch(orientation) {
				case RulerOrientation.Horizontal:
					rect = RectHelper.DeflateRect(rect, 0, BorderWidth, 0, BorderWidth);
					break;
				case RulerOrientation.Vertical:
					rect = RectHelper.DeflateRect(rect, BorderWidth, 0, BorderWidth, 0);
					break;
			}
			return rect;
		}
		public override Brush TextBrush {
			get { return SystemBrushes.WindowText; }
		}
		public override void DrawSlider(GraphicsCache cache, VRulerSection section, RulerOrientation orientation) {
			Rectangle bounds = section.SliderPlaceBounds;
			switch(orientation) {
				case RulerOrientation.Horizontal:
					bounds = RectHelper.DeflateRect(bounds, BorderWidth, 0, BorderWidth, 0);
					break;
				case RulerOrientation.Vertical:
					bounds = RectHelper.DeflateRect(bounds, 0, BorderWidth, 0, BorderWidth);
					break;
			}
			cache.FillRectangle(GetSliderBackgroundBrush(cache, section), bounds);
			DrawSliderBorder(cache, section);
			if(section.CanResize)
				DrawSliderContent(cache, section, ((VRuler)section.Ruler).CanResizeSection(section));
		}
		protected override void DrawHMargin(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			cache.FillRectangle(GetRightMarginBrush(cache), CorrectBoundsForRuler(rect, orientation));
		}
		protected virtual Brush GetRightMarginBrush(GraphicsCache cache) {
			return SystemBrushes.ControlDark;
		}
		protected override void DrawShadowRect(GraphicsCache cache, Rectangle rect, int shadowLevel, RulerOrientation orientation) {
			cache.FillRectangle(shadowBrush, CorrectBoundsForRuler(rect, orientation));
		}
		protected override void DrawSelectionRect(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			cache.FillRectangle(selectionBrush, CorrectBoundsForRuler(rect, orientation));
		}
		protected override void DrawSection(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			cache.FillRectangle(SystemBrushes.Window, rect);
		}
		protected override void DrawSectionBorder(GraphicsCache graphicsCache, Rectangle sectionBounds, RulerOrientation orientarion) {
			if(sectionBounds.Height <= 0)
				return;
			Rectangle rect = sectionBounds;
			switch(orientarion) {
				case RulerOrientation.Horizontal:
					rect = RectHelper.InflateRect(rect, BorderWidth, 0, BorderWidth, 0);
					break;
				case RulerOrientation.Vertical:
					rect = RectHelper.InflateRect(rect, 0, BorderWidth, 0, BorderWidth);
					break;
			}
			ControlPaintHelper.DrawRectangle(graphicsCache.Graphics, SystemBrushes.ControlDark, 1, rect);
		}
		protected virtual void DrawSliderBorder(GraphicsCache cache, VRulerSection section) {
		}
		protected virtual Brush GetSliderBackgroundBrush(GraphicsCache cache, VRulerSection section) {
			return FlatPaintHelper.SliderBrush;
		}
		protected virtual Brush GetSliderContentInnerBrush(GraphicsCache cache) {
			return innerBrush;
		}
		protected virtual Brush GetSliderContentOuterBrush(GraphicsCache cache) {
			return outerBrush;
		}
		protected virtual Brush GetSliderContentBrush(GraphicsCache cache) {
			return contentBrush;
		}
		private void DrawSliderContent(GraphicsCache cache, VRulerSection section, bool canResize) {
			const int LinesCount = 4;
			Size grabSize = new Size(6, LinesCount * 2 - 1);
			Point location = RectHelper.CenterOf(section.SliderPlaceBounds);
			location.Offset(-grabSize.Width / 2, -grabSize.Height / 2);
			Brush br = canResize ? GetSliderContentBrush(cache) : GetSliderContentOuterBrush(cache);
			for(int i = 0; i < LinesCount; i++) {
				cache.FillRectangle(br, new Rectangle(location, new Size(grabSize.Width, 1)));
				location.Y += 2;
			}
		}
	}
	public class RulerSectionPaintHelperOffice2003 : RulerSectionPaintHelperFlat {
		public override void DrawBackground(RulerViewInfo viewInfo) {
			viewInfo.Cache.FillRectangle(viewInfo.Cache.GetSolidBrush(Office2003PaintHelper.Colors.RulerBackground), viewInfo.Bounds);
		}
		protected override Brush GetRightMarginBrush(GraphicsCache cache) {
			return cache.GetSolidBrush(Office2003PaintHelper.Colors.RulerRightMargin);
		}
		protected override Brush GetSliderBackgroundBrush(GraphicsCache cache, VRulerSection section) {
			return Office2003PaintHelper.GetSliderBrush(cache, section);
		}
		protected override void DrawSliderBorder(GraphicsCache cache, VRulerSection section) {
			Office2003PaintHelper.DrawBandBorder(cache, section.SliderPlaceBounds, section.Level, false);
		}
		protected override Brush GetSliderContentInnerBrush(GraphicsCache cache) {
			return cache.GetSolidBrush(Office2003PaintHelper.Colors.SliderContentInner);
		}
		protected override Brush GetSliderContentOuterBrush(GraphicsCache cache) {
			return cache.GetSolidBrush(Office2003PaintHelper.Colors.SliderContentOuter);
		}
		protected override Brush GetSliderContentBrush(GraphicsCache cache) {
			return cache.GetSolidBrush(Office2003PaintHelper.Colors.SliderContentOuter);
		}
	}
	public class RulerSectionPaintHelperSkin : RulerSectionPaintHelper {
		#region inner classes
		class SkinElementInfoEx : SkinElementInfo {
			bool drawGlyph = true;
			public bool DrawGlyph {
				get { return drawGlyph; }
			}
			public SkinElementInfoEx(SkinElement element, Rectangle bounds, bool drawGlyph)
				: base(element, bounds) {
				this.drawGlyph = drawGlyph;
			}
		}
		class SkinElementPainterEx : SkinElementPainter {
			static SkinElementPainterEx defaultPainter = new SkinElementPainterEx();
			public new static SkinElementPainterEx Default {
				get { return defaultPainter; }
			}
			protected override void DrawSkinForeground(SkinElementInfo ee) {
				if(ee is SkinElementInfoEx) {
					if(((SkinElementInfoEx)ee).DrawGlyph)
						base.DrawSkinForeground(ee);
				} else
					base.DrawSkinForeground(ee);
			}
		}
		#endregion inner classes
		#region static
		static int ValidateValue(int value, int validatingValue) {
			return value != 0 ? value : validatingValue;
		}
		#endregion
		UserLookAndFeel lookAndFeel;
		static Brush textBrush;
		public override Brush TextBrush {
			get { return textBrush; }
		}
		protected override int HorzMargins {
			get { return ReportsSkins.GetSkin(lookAndFeel).Properties.GetInteger("RulerHorzMargins"); }
		}
		protected override int VertMargins {
			get { return ReportsSkins.GetSkin(lookAndFeel).Properties.GetInteger("RulerVertMargins"); }
		}
		public override Margins RulerSectionMargins {
			get {
				Skin skin = ReportsSkins.GetSkin(lookAndFeel);
				SkinPaddingEdges padding = skin[ReportsSkins.SkinRulerSection].ContentMargins;
				return new Margins(padding.Left, padding.Right, padding.Top, padding.Bottom);
			}
		}
		public override Margins ClientMargins {
			get {
				SkinElement rulerBackground = SkinPaintHelper.GetSkinElement(lookAndFeel, ReportsSkins.SkinRulerBackground);
				SkinPaddingEdges margins = rulerBackground.ContentMargins;
				int top = ValidateValue(margins.Top, base.ClientMargins.Top);
				int bottom = ValidateValue(margins.Bottom, base.ClientMargins.Bottom);
				int left = ValidateValue(margins.Left, base.ClientMargins.Left);
				int right = ValidateValue(margins.Right, base.ClientMargins.Right);
				return new Margins(left, right, top, bottom);
			}
		}
		public RulerSectionPaintHelperSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			if(textBrush != null)
				textBrush.Dispose();
			textBrush = new SolidBrush(ReportsSkins.GetSkin(lookAndFeel).Properties.GetColor("RulerForeColor"));
		}
		public override void DrawSlider(GraphicsCache cache, VRulerSection section, RulerOrientation orientation) {
			Skin skin = ReportsSkins.GetSkin(lookAndFeel);
			Rectangle sliderPlaceBounds = section.SliderPlaceBounds;
			if(section.IsTopNeighbor && SkinPaintHelper.RequireVertOffset(skin))
				sliderPlaceBounds.Height++;
			SkinElement skinEl;
			SkinElementInfoEx skinElInfo;
			if(section.Selected) {
				skinEl = skin[ReportsSkins.SkinSliderSelected];
				skinElInfo = new SkinElementInfoEx(skinEl, sliderPlaceBounds, section.CanResize);
			} else {
				skinEl = skin[ReportsSkins.SkinSlider];
				skinElInfo = new SkinElementInfoEx(skinEl, sliderPlaceBounds, section.CanResize);
				skinElInfo.ImageIndex = section.Level % 3;
			}
			ObjectPainter.DrawObject(cache, SkinElementPainterEx.Default, skinElInfo);
		}
		protected override void DrawHMargin(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, ReportsSkins.SkinRulerRightMargin, rect);
		}
		public override void DrawBackground(RulerViewInfo viewInfo) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, ReportsSkins.SkinRulerBackground, viewInfo.Bounds, (int)viewInfo.RulerOrientation);
		}
		protected override void DrawShadowRect(GraphicsCache cache, Rectangle rect, int shadowLevel, RulerOrientation orientation) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, ReportsSkins.SkinRulerShadow, rect, shadowLevel % 3 + (int)orientation * 3);
		}
		protected override void DrawSelectionRect(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, ReportsSkins.SkinRulerSelection, rect, (int)orientation);
		}
		protected override void DrawSection(GraphicsCache cache, Rectangle rect, RulerOrientation orientation) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, ReportsSkins.SkinRulerSection, rect, (int)orientation);
		}
	}
	public class CornerPanelPainter : ObjectPainter, IDisposable {
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		public virtual void Dispose(bool disposing) {
		}
	}
	public class CornerPanelPainterFlat : CornerPanelPainter {
		Image image;
		public override void DrawObject(ObjectInfoArgs e) {
			CornerPanelViewInfo viewInfo = (CornerPanelViewInfo)e;
			DrawBackground(viewInfo);
			if(image == null)
				image = ResLoader.LoadBitmap("Images.CornerGlyph.bmp", typeof(LocalResFinder), Color.Magenta);
			viewInfo.Graphics.DrawImage(image, viewInfo.TagBounds);
			DrawTagBorder(viewInfo);
		}
		public override void Dispose(bool disposing) {
			if(disposing) {
				if(image != null) {
					image.Dispose();
					image = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void DrawBackground(CornerPanelViewInfo viewInfo) {
		}
		protected virtual void DrawTagBorder(CornerPanelViewInfo viewInfo) {
			Border3DStyle borderStyle = viewInfo.HotTracked ? Border3DStyle.RaisedOuter : Border3DStyle.RaisedInner;
			ControlPaint.DrawBorder3D(viewInfo.Graphics, viewInfo.TagBounds, borderStyle);
		}
	}
	public class CornerPanelPainterOffice2003 : CornerPanelPainterFlat {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CornerPanelViewInfo viewInfo = (CornerPanelViewInfo)e;
			Rectangle r = viewInfo.TagBounds;
			r.Offset(-1, -1);
			viewInfo.TagBounds = r;
			return base.CalcObjectMinBounds(e);
		}
		protected override void DrawBackground(CornerPanelViewInfo viewInfo) {
			viewInfo.Cache.FillRectangle(viewInfo.Cache.GetSolidBrush(Office2003PaintHelper.Colors.RulerBackground), viewInfo.Bounds);
			Rectangle r;
			r = viewInfo.Bounds;
			r.Inflate(-2, -2);
			r.Height--;
			r.Width--;
			r.X++;
			r.Y++;
			viewInfo.Cache.DrawRectangle(viewInfo.Cache.GetPen(Office2003PaintHelper.Colors.CornerPanelInnerBorder), r);
			r = viewInfo.Bounds;
			r.Height--;
			r.Inflate(-2, -2);
			viewInfo.Cache.DrawRectangle(viewInfo.Cache.GetPen(Office2003PaintHelper.Colors.CornerPanelOuterBorder), r);
		}
		protected override void DrawTagBorder(CornerPanelViewInfo viewInfo) {
		}
	}
	public class CornerPanelPainterSkin : CornerPanelPainter {
		UserLookAndFeel lookAndFeel;
		public CornerPanelPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			CornerPanelViewInfo viewInfo = (CornerPanelViewInfo)e;
			SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, ReportsSkins.SkinCornerPanel, viewInfo.Bounds, viewInfo.HotTracked ? 1 : 0);
		}
	}
	public class PopupFormPainter : ObjectPainter {
		protected static Font defaultFont = new Font("Tahoma", 8f, FontStyle.Bold);
		static int padding = 2;
		static StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
		public override void DrawObject(ObjectInfoArgs e) {
			PopupFormViewInfo viewInfo = (PopupFormViewInfo)e;
			DrawBackground(viewInfo);
			DrawCaptionBackground(viewInfo);
		}
		public virtual void DrawBackground(GraphicsCache cache, Rectangle bounds) {
		}
		public virtual void DrawBackground(GraphicsInfoArgs viewInfo) {
			DrawBackground(viewInfo.Cache, viewInfo.Bounds);
		}
		protected virtual void DrawCaptionBackground(PopupFormViewInfo viewInfo) {
		}
		protected virtual void DrawText(PopupFormViewInfo viewInfo, Font font) {
			Brush brush = viewInfo.Cache.GetSolidBrush(GetCaptionColor());
			viewInfo.Cache.DrawString(viewInfo.Text, font, brush, viewInfo.TextBounds, stringFormat);
		}
		protected virtual Color GetCaptionColor() {
			return Color.Empty;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			PopupFormViewInfo viewInfo = (PopupFormViewInfo)e;
			GraphicsInfo.Default.AddGraphics(null);
			try {
				GraphicsCache cache = GraphicsInfo.Default.Cache;
				SizeF size = cache.CalcTextSize(viewInfo.Text + "w", defaultFont, stringFormat, int.MaxValue);
				size.Height *= 1.5f;
				viewInfo.CaptionHeight = (int)size.Height;
				viewInfo.TextBounds = new Rectangle(viewInfo.Bounds.Location, Size.Ceiling(cache.CalcTextSize(viewInfo.Text, defaultFont, stringFormat, int.MaxValue)));
				viewInfo.TextBounds = SkinPaintHelper.OffsetRectangle(viewInfo.TextBounds, padding, padding);
				return viewInfo.Bounds;
			} finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
	}
	public class PopupFormPainterFlat : PopupFormPainter {
		public override void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(cache.GetGradientBrush(bounds, Color.FromArgb(0xfa, 0xf8, 0xf6), Color.FromArgb(0xec, 0xe9, 0xd8), LinearGradientMode.Horizontal), bounds);
			FlatPaintHelper.DrawBorders(cache, bounds, FlatPaintHelper.BorderColor);
		}
		protected override void DrawCaptionBackground(PopupFormViewInfo viewInfo) {
			viewInfo.Cache.FillRectangle(viewInfo.Cache.GetSolidBrush(Color.FromArgb(0xd4, 0xd0, 0xc8)), viewInfo.GetCaptionRect());
			FlatPaintHelper.DrawBorders(viewInfo.Cache, viewInfo.GetCaptionRect(), FlatPaintHelper.BorderColor);
			DrawText(viewInfo, defaultFont);
		}
		protected override Color GetCaptionColor() {
			return Color.Black;
		}
	}
	public class PopupFormPainterOffice2003 : PopupFormPainterFlat {
		public override void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			FlatPaintHelper.DrawBorders(cache, bounds, Office2003PaintHelper.Colors.PopupFormBorder);
		}
		public override void DrawBackground(GraphicsInfoArgs viewInfo) {
			Rectangle backgroundRect = viewInfo.Bounds;
			viewInfo.Cache.FillRectangle(viewInfo.Cache.GetGradientBrush(backgroundRect, Office2003PaintHelper.Colors.PopupFormBackground1, Office2003PaintHelper.Colors.PopupFormBackground2, LinearGradientMode.BackwardDiagonal), backgroundRect);
			viewInfo.Cache.DrawRectangle(viewInfo.Cache.GetPen(Office2003PaintHelper.Colors.PopupFormText), backgroundRect);
		}
		protected override void DrawCaptionBackground(PopupFormViewInfo viewInfo) {
			Rectangle captionRect = viewInfo.GetCaptionRect();
			viewInfo.Cache.FillRectangle(viewInfo.Cache.GetGradientBrush(captionRect, Office2003PaintHelper.Colors.PopupFormCaption1, Office2003PaintHelper.Colors.PopupFormCaption2, LinearGradientMode.Horizontal), captionRect);
			viewInfo.Cache.DrawRectangle(viewInfo.Cache.GetPen(Office2003PaintHelper.Colors.PopupFormText), captionRect);
			DrawText(viewInfo, defaultFont);
		}
		protected override Color GetCaptionColor() {
			return Office2003PaintHelper.Colors.PopupFormText;
		}
	}
	public class PopupFormPainterSkin : PopupFormPainter {
		UserLookAndFeel lookAndFeel;
		public PopupFormPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, ReportsSkins.SkinPopupFormBackground, bounds);
		}
		protected override void DrawCaptionBackground(PopupFormViewInfo viewInfo) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, ReportsSkins.SkinPopupFormCaption, viewInfo.GetCaptionRect());
			SkinElement skinEl = ReportsSkins.GetSkin(lookAndFeel)[ReportsSkins.SkinPopupFormCaption];
			Font captionFont = viewInfo.Cache.GetFont(defaultFont, skinEl.Color.FontBold ? FontStyle.Bold : FontStyle.Regular);
			DrawText(viewInfo, captionFont);
		}
		protected override Color GetCaptionColor() {
			return SkinPaintHelper.GetSkinElement(lookAndFeel, ReportsSkins.SkinPopupFormCaption).Color.ForeColor;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			base.CalcObjectMinBounds(e);
			PopupFormViewInfo viewInfo = (PopupFormViewInfo)e;
			SkinElement skinEl = SkinPaintHelper.GetSkinElement(lookAndFeel, ReportsSkins.SkinPopupFormCaption);
			int dx = skinEl.ContentMargins.Left;
			int dy = skinEl.ContentMargins.Top;
			viewInfo.TextBounds = SkinPaintHelper.OffsetRectangle(viewInfo.TextBounds, dx, dy);
			viewInfo.CaptionHeight += dy + skinEl.ContentMargins.Bottom;
			return viewInfo.Bounds;
		}
	}
	public class SmartTagPainter : ObjectPainter {
		#region static
		static void FillRectangle(GraphicsCache cache, RectangleF rect, float delta, Brush foreBrush) {
			RectangleF r = rect;
			r.Height = delta;
			cache.Graphics.FillRectangle(foreBrush, r);
			r.Y = rect.Bottom - delta;
			cache.Graphics.FillRectangle(foreBrush, r);
		}
		static void DrawTriangleToRight(GraphicsCache cache, RectangleF rect, Brush foreBrush) {
			float delta = XRConvert.Convert(1f, GraphicsUnit.Pixel, cache.Graphics.PageUnit);
			rect.Inflate(-3 * delta, -3 * delta);
			rect.Width = 2 * delta;
			rect.X += delta;
			while(rect.Height > delta / 2) {
				FillRectangle(cache, rect, delta, foreBrush);
				rect.Offset(delta, delta);
				rect.Height -= 2 * delta;
			}
		}
		static void DrawTriangleToLeft(GraphicsCache cache, RectangleF rect, Brush foreBrush) {
			float delta = XRConvert.Convert(1f, GraphicsUnit.Pixel, cache.Graphics.PageUnit);
			rect.Inflate(-3 * delta, -3 * delta);
			rect.X = rect.Right - 3 * delta;
			rect.Width = 2 * delta;
			while(rect.Height > delta / 2) {
				FillRectangle(cache, rect, delta, foreBrush);
				rect.Offset(-delta, delta);
				rect.Height -= 2 * delta;
			}
		}
		#endregion
		protected SmartTagViewInfo viewInfo;
		protected virtual Color BorderColor {
			get { return Color.Empty; }
		}
		protected virtual Color BorderColorHotTracked {
			get { return Color.Empty; }
		}
		protected virtual Color BackColor {
			get { return Color.Empty; }
		}
		protected virtual Color BackColorHotTracked {
			get { return Color.Empty; }
		}
		protected virtual Color ForeColor {
			get { return Color.Empty; }
		}
		protected virtual Color ForeColorHotTracked {
			get { return Color.Empty; }
		}
		Brush backgrBrush, foreBrush, borderBrush;
		public override void DrawObject(ObjectInfoArgs e) {
			viewInfo = (SmartTagViewInfo)e;
			RectangleF rect = viewInfo.SmartTagRect;
			backgrBrush = null;
			foreBrush = null;
			borderBrush = null;
			try {
				float dpi = GraphicsDpi.GetGraphicsDpi(e.Graphics);
				if((viewInfo.SmartTagState & SmartTagState.Popup) > 0) {
					CreateHotTrackedBrushes();
					viewInfo.Graphics.FillRectangle(backgrBrush, viewInfo.GetBackgroundRect(dpi));
					DrawTriangleToLeft(viewInfo.Cache, rect, foreBrush);
				} else if((viewInfo.SmartTagState & SmartTagState.ContainsMouse) > 0) {
					CreateHotTrackedBrushes();
					viewInfo.Graphics.FillRectangle(backgrBrush, viewInfo.GetBackgroundRect(dpi));
					DrawTriangleToRight(viewInfo.Cache, rect, foreBrush);
				} else {
					CreateNormalBrushes();
					viewInfo.Graphics.FillRectangle(backgrBrush, viewInfo.GetBackgroundRect(dpi));
					DrawTriangleToRight(viewInfo.Cache, rect, foreBrush);
				}
				if(CanDrawBorder(viewInfo))
					ControlPaintHelper.DrawRectangle(viewInfo.Graphics, 
						borderBrush,
						XRConvert.Convert(1f, GraphicsDpi.Pixel, dpi),
						rect);
			} finally {
				if(backgrBrush != null)
					backgrBrush.Dispose();
				if(foreBrush != null)
					foreBrush.Dispose();
				if(borderBrush != null)
					foreBrush.Dispose();
			}
		}
		protected virtual bool CanDrawBorder(SmartTagViewInfo viewInfo) {
			return viewInfo.DrawBorder;
		}
		protected virtual void CreateHotTrackedBrushes() {
			backgrBrush = new SolidBrush(BackColorHotTracked);
			foreBrush = new SolidBrush(ForeColorHotTracked);
			borderBrush = new SolidBrush(BorderColorHotTracked);
		}
		protected virtual void CreateNormalBrushes() {
			backgrBrush = new SolidBrush(BackColor);
			foreBrush = new SolidBrush(ForeColor);
			borderBrush = new SolidBrush(BorderColor);
		}
	}
	public class SmartTagPainterFlat : SmartTagPainter {
		protected override Color BorderColor {
			get { return Color.Black; }
		}
		protected override Color BorderColorHotTracked {
			get { return Color.Black; }
		}
		protected override Color BackColor {
			get { return Color.White; }
		}
		protected override Color BackColorHotTracked {
			get { return Color.FromArgb(0xd7, 0xe2, 0xf4); }
		}
		protected override Color ForeColor {
			get { return Color.Black; }
		}
		protected override Color ForeColorHotTracked {
			get { return Color.Black; }
		}
	}
	public class SmartTagPainterOffice2003 : SmartTagPainterFlat {
		protected override Color BackColor {
			get { return viewInfo.IsWinControlItem ? Office2003PaintHelper.Colors.SmartTagBackColor : base.BackColor; }
		}
		protected override bool CanDrawBorder(SmartTagViewInfo viewInfo) {
			if(viewInfo.IsWinControlItem)
				return false;
			return true;
		}
	}
	public class SmartTagPainterSkin : SmartTagPainter {
		UserLookAndFeel lookAndFeel;
		protected override bool CanDrawBorder(SmartTagViewInfo viewInfo) {
			return true;
		}
		protected override Color BorderColor {
			get { return GetColorByName("BorderColor"); }
		}
		protected override Color BorderColorHotTracked {
			get { return GetColorByName("BorderColorHotTracked"); }
		}
		protected override Color BackColor {
			get { return GetColorByName("BackColor"); }
		}
		protected override Color BackColorHotTracked {
			get { return GetColorByName("BackColorHotTracked"); }
		}
		protected override Color ForeColor {
			get { return GetColorByName("ForeColor"); }
		}
		protected override Color ForeColorHotTracked {
			get { return GetColorByName("ForeColorHotTracked"); }
		}
		public SmartTagPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		Color GetColorByName(string colorName) {
			Skin skin = ReportsSkins.GetSkin(lookAndFeel);
			SkinElement skinEl = skin[ReportsSkins.SkinSmartTag];
			return skinEl.Properties.GetColor(colorName);
		}
	}
	public class ComponentTrayPainter : ObjectPainter {
		public virtual Color GetForeColor() {
			return Control.DefaultForeColor;
		}
	}
	public class ComponentTrayPainterFlat : ComponentTrayPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(GetBackgroundColor()), e.Bounds);
		}
		public virtual Color GetBackgroundColor() {
			return SystemColors.Control;
		}
	}
	public class ComponentTrayPainterOffice2003 : ComponentTrayPainterFlat {
		public override Color GetBackgroundColor() {
			return Office2003PaintHelper.Colors.ComponentTrayBackground;
		}
	}
	public class ComponentTrayPainterSkin : ComponentTrayPainter {
		UserLookAndFeel lookAndFeel;
		public ComponentTrayPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, e.Cache, ReportsSkins.SkinComponentTrayBackground, e.Bounds);
		}
		public override Color GetForeColor() {
			SkinElement element = SkinPaintHelper.GetSkinElement(lookAndFeel, ReportsSkins.SkinComponentTrayBackground);
			return ReportsSkins.GetSkin(lookAndFeel).GetSystemColor(element.Color.ForeColor);
		}
	}
}
