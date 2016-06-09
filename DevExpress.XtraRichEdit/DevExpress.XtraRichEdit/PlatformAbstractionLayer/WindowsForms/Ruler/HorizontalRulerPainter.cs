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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Utils;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Layout.Export;
namespace DevExpress.XtraRichEdit.Ruler {
	#region HorizontalRulerPainter (abstract class)
	public abstract partial class HorizontalRulerPainter : RulerPainterBase {
		#region Fields
		readonly HorizontalRulerControl ruler;
		Bitmap quarterTickImage;
		Bitmap halfTickImage;
		Bitmap defaultTabImage;
		RulerElementPainter elementPainter;
		#endregion
		protected HorizontalRulerPainter(HorizontalRulerControl ruler)
			: base(ruler) {
			this.ruler = ruler;
		}
		#region Properties
		public RulerElementPainter ElementPainter { get { return elementPainter; } }
		public HorizontalRulerControl Ruler { get { return ruler; } }
		public Bitmap QuarterTickImage { get { return quarterTickImage; } }
		public Bitmap HalfTickImage { get { return halfTickImage; } }
		protected abstract Color DefaultTabColor { get; }
		#endregion
		public virtual void Initialize() {
			this.elementPainter = CreateElementPainter();
			this.elementPainter.Initialize();
			this.halfTickImage = GenerateHalfTickImage();
			this.quarterTickImage = GenerateQuarterTickImage();
			this.defaultTabImage = GenerateDefaultTabImage();
		}
		public override int CalculateTotalRulerSize(int textSize) {
			return textSize + PaddingTop + PaddingBottom + VerticalTextPaddingBottom + VerticalTextPaddingTop; 
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (quarterTickImage != null) {
						quarterTickImage.Dispose();
						quarterTickImage = null;
					}
					if (halfTickImage != null) {
						halfTickImage.Dispose();
						halfTickImage = null;
					}
					if (defaultTabImage != null) {
						defaultTabImage.Dispose();
						defaultTabImage = null;
					}
					if (elementPainter != null) {
						elementPainter.Dispose();
						elementPainter = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Default images generation
		protected internal virtual Bitmap GenerateQuarterTickImage() {
			return GenerateTickImageCore(Rectangle.Round(ruler.ViewInfo.DisplayQuarterTickBounds));
		}
		protected internal virtual Bitmap GenerateHalfTickImage() {
			return GenerateTickImageCore(Rectangle.Round(ruler.ViewInfo.DisplayHalfTickBounds));
		}
		protected internal virtual Bitmap GenerateDefaultTabImage() {
			return elementPainter.GenerateImage(ruler.ViewInfo.DisplayDefaultTabBounds, DefaultTabColor, DrawTickmarkImage);
		}
		Bitmap GenerateTickImageCore(Rectangle bounds) {
			return elementPainter.GenerateImage(bounds, ForeColor, DrawTickmarkImage);
		}
		void DrawTickmarkImage(Graphics gr, Rectangle bounds, Brush brush) {
			gr.FillRectangle(brush, bounds);
		}
		#endregion
		public virtual void DrawBackground(GraphicsCache cache) {
			DrawControlBackground(cache, ruler.ClientRectangle);
		}
		public virtual void DrawTabTypeToggle(GraphicsCache cache) {
			DrawTabTypeToggleBackground(cache, ruler.ViewInfo.TabTypeToggleBackground.Bounds);
			DrawTabTypeToggleActiveArea(cache, ruler.ViewInfo.TabTypeToggleHotZone.DisplayBounds);
			ruler.Painter.ElementPainter.DrawHotZone(ruler.ViewInfo.TabTypeToggleHotZone.HotZone, cache, ruler.ViewInfo.TabTypeToggleHotZone.HotZoneBounds.Location);
		}
		public override void Draw(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			DrawInPixels(cache);
		}
		protected internal void DrawInPixels(GraphicsCache cache) {
			RichEditControl control = ruler.RichEditControl;
			CaretPosition caretPosition = control.ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.Column);
			if (caretPosition.PageViewInfo == null)
				return;
			Graphics gr = cache.Graphics;
			try {
				int viewBoundsLeft = ruler.LayoutUnitsToPixelsH(control.ViewBounds.Left);
				gr.TranslateTransform(viewBoundsLeft, 0);
				BeginDraw(cache);
				DrawRuler();
				EndDraw();
			}
			finally {
				gr.ResetTransform();
			}
		}
		protected internal override void BeginDraw(GraphicsCache cache) {
			CacheInitialize(cache);
			int physicalLeftInvisibleWidth = View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			cache.Graphics.TranslateTransform(-ruler.LayoutUnitsToPixelsH(physicalLeftInvisibleWidth), 0);
		}
		protected internal override void EndDraw() {
			CacheDispose();
		}
		protected internal virtual void DrawRuler() {
			DrawRulerAreaBackground(ruler.ViewInfo.DisplayClientBounds);
			DrawActiveAreas();
			DrawTickMarks();
			DrawSpaceAreas();
			DrawDefaultTabMarks();
			DrawHotZones();
		}
		void DrawActiveAreas() {
			ruler.ViewInfo.DisplayActiveAreaCollection.ForEach(DrawActiveArea);
		}
		void DrawSpaceAreas() {
			ruler.ViewInfo.DisplaySpaceAreaCollection.ForEach(DrawSpaceArea);
		}
		void DrawTickMarks() {
			ruler.ViewInfo.RulerTickmarks.ForEach(DrawTickMark);
		}
		void DrawDefaultTabMarks() {
			ruler.ViewInfo.DisplayDefaultTabsCollection.ForEach(DrawDefaultTabMark);
		}
		void DrawHotZones() {
			ruler.ViewInfo.HotZones.ForEach(DrawHotZone);
		}
		protected internal virtual void DrawHotZone(RulerHotZone hotZone) {
			if (hotZone.Visible)
				elementPainter.DrawHotZone(hotZone, Cache, hotZone.DisplayBounds.Location);
		}
		protected internal virtual void DrawTickmarkNumber(RulerTickmarkNumber tickmark) {
			PointF location = tickmark.DisplayBounds.Location;
			Graphics.DrawString(tickmark.Number, ruler.TickMarkFont, Cache.GetSolidBrush(ForeColor), location.X, location.Y, StringFormat.GenericTypographic);
		}
		protected internal virtual void DrawTickmarkHalf(RulerTickmarkHalf tickmark) {
			RectangleF bounds = tickmark.DisplayBounds;
			DrawBitmap(Graphics, halfTickImage, (int)bounds.X, (int)bounds.Y);
		}
		protected internal virtual void DrawTickmarkQuarter(RulerTickmarkQuarter tickmark) {
			RectangleF bounds = tickmark.DisplayBounds;
			DrawBitmap(Graphics, quarterTickImage, (int)bounds.X, (int)bounds.Y);
		}
		protected internal virtual void DrawTickMark(RulerTickmark tickMark) {
			tickMark.Draw(this);
		}
		protected internal virtual void DrawDefaultTabMark(Rectangle bounds) {
			DrawBitmap(Graphics, defaultTabImage, (int)bounds.X, (int)bounds.Y);
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, DevExpress.XtraRichEdit.Internal.PrintLayout.PageViewInfo page) {
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, Layout.CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
		}
		protected internal abstract Size GetTabTypeToggleActiveAreaSize();
		protected internal abstract void DrawTabTypeToggleBackground(GraphicsCache cache, Rectangle bounds);
		protected internal abstract void DrawTabTypeToggleActiveArea(GraphicsCache cache, Rectangle bounds);
		protected internal abstract void DrawControlBackground(GraphicsCache cache, Rectangle bounds);
		protected internal abstract void DrawRulerAreaBackground(Rectangle bounds);
		protected internal abstract void DrawActiveArea(RectangleF bounds);
		protected internal abstract void DrawSpaceArea(RectangleF bounds);
		protected internal abstract RulerElementPainter CreateElementPainter();
	}
	#endregion
	#region HorizontalRulerSkinPainter
	public class HorizontalRulerSkinPainter : HorizontalRulerPainter {
		public HorizontalRulerSkinPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		public override UserLookAndFeel LookAndFeel { get { return Ruler.LookAndFeel; } }
		protected override Color ForeColor {
			get {
				Skin skin = ReportsSkins.GetSkin(LookAndFeel);
				return skin.Properties.GetColor(RichEditPrintingSkins.SkinRulerForeColor);
			}
		}
		protected override Color DefaultTabColor {
			get {
				Skin skin = RichEditSkins.GetSkin(LookAndFeel);
				return skin.Colors.GetColor(RichEditSkins.SkinRulerDefaultTabColor);
			}
		}
		protected internal override int PaddingTop {
			get {
				SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[RichEditSkins.SkinHorizontalRulerBackground];
				return Ruler.PixelsToLayoutUnitsV(element.ContentMargins.Top);
			}
		}
		protected internal override int PaddingBottom {
			get {
				SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[RichEditSkins.SkinHorizontalRulerBackground];
				return Ruler.PixelsToLayoutUnitsV(element.ContentMargins.Bottom);
			}
		}
		protected internal override int VerticalTextPaddingBottom {
			get {
				SkinElement element = ReportsSkins.GetSkin(LookAndFeel)[ReportsSkins.SkinRulerSection];
				return Ruler.PixelsToLayoutUnitsV(element.ContentMargins.Bottom + 1);
			}
		}
		protected internal override int VerticalTextPaddingTop {
			get {
				SkinElement element = ReportsSkins.GetSkin(LookAndFeel)[ReportsSkins.SkinRulerSection];
				return Ruler.PixelsToLayoutUnitsV(element.ContentMargins.Top + 1);
			}
		}
		public override int CalculateTotalRulerSize(int textSize) {
			Size areaSize = CalculateReportsSkinElementMinSize(ReportsSkins.SkinRulerRightMargin);
			Size activeAreaSize = CalculateReportsSkinElementMinSize(ReportsSkins.SkinRulerSection);
			int textAreaHeight = Math.Max(textSize, Ruler.PixelsToLayoutUnitsV(Math.Max(areaSize.Height, activeAreaSize.Height)));
			return textAreaHeight + PaddingTop + PaddingBottom + VerticalTextPaddingBottom + VerticalTextPaddingTop;
		}
		protected internal virtual Size CalculateReportsSkinElementMinSize(string elementName) {
			return ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, GetReportsSkinElement(elementName, Rectangle.Empty)).Size;
		}
		protected internal override void DrawControlBackground(GraphicsCache cache, Rectangle bounds) {
			DrawRichEditSkinElement(cache, RichEditSkins.SkinHorizontalRulerBackground, bounds);
		}
		protected internal override void DrawTabTypeToggleBackground(GraphicsCache cache, Rectangle bounds) {
			DrawRichEditSkinElement(cache, RichEditSkins.SkinHorizontalCornerPanel, bounds);
		}
		protected internal override void DrawTabTypeToggleActiveArea(GraphicsCache cache, Rectangle bounds) {
			DrawRichEditSkinElement(cache, RichEditSkins.SkinRulerTabTypeBackground, bounds);
		}
		protected internal override void DrawRulerAreaBackground(Rectangle bounds) {
			DrawReportsSkinElement(Cache, ReportsSkins.SkinRulerRightMargin, bounds);
		}
		protected internal override void DrawActiveArea(RectangleF bounds) {
			DrawReportsSkinElement(Cache, ReportsSkins.SkinRulerSection, Rectangle.Round(bounds));
		}
		protected internal override void DrawSpaceArea(RectangleF bounds) {
			DrawReportsSkinElement(Cache, ReportsSkins.SkinRulerRightMargin, Rectangle.Round(bounds));
		}
		protected internal override RulerElementPainter CreateElementPainter() {
			return new RulerElementSkinPainter(Ruler, LookAndFeel);
		}
		protected internal override Size GetTabTypeToggleActiveAreaSize() {
			return new Size(15, 15);
		}
	}
	#endregion
	#region ColorBasedRulerPainter (abstract class)
	public abstract class ColorBasedRulerPainter : HorizontalRulerPainter {
		protected ColorBasedRulerPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected abstract Color ControlBackgroundColor { get; }
		protected abstract Color RulerAreaBackgroundColor { get; }
		protected abstract Color ActiveAreaColor { get; }
		protected abstract Color SpaceAreaColor { get; }
		protected abstract Color TabTypeToggleBorderColor { get; }
		protected internal override void DrawControlBackground(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(ControlBackgroundColor, bounds);
		}
		protected internal override void DrawTabTypeToggleBackground(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(SpaceAreaColor, bounds);
		}
		protected internal override void DrawTabTypeToggleActiveArea(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(ControlBackgroundColor, bounds);
			bounds.Inflate(1, 1);
			cache.DrawRectangle(cache.GetPen(TabTypeToggleBorderColor), bounds);
		}
		protected internal override void DrawRulerAreaBackground(Rectangle bounds) {
			Cache.FillRectangle(RulerAreaBackgroundColor, bounds);
		}
		protected internal override void DrawActiveArea(RectangleF bounds) {
			Graphics.FillRectangle(Cache.GetSolidBrush(ActiveAreaColor), bounds);
		}
		protected internal override void DrawSpaceArea(RectangleF bounds) {
			Graphics.FillRectangle(Cache.GetSolidBrush(SpaceAreaColor), bounds);
		}
		protected internal override RulerElementPainter CreateElementPainter() {
			return new ColorBasedRulerElementPainter(Ruler, ForeColor);
		}
		protected internal override Size GetTabTypeToggleActiveAreaSize() {
			return new Size(15, 15);
		}
	}
	#endregion
	#region HorizontalRulerFlatPainter
	public class HorizontalRulerFlatPainter : ColorBasedRulerPainter {
		public HorizontalRulerFlatPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
		protected override Color DefaultTabColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color TabTypeToggleBorderColor { get { return SystemColors.ControlDarkDark; } }
	}
	#endregion
	#region HorizontalRulerUltraFlatPainter
	public class HorizontalRulerUltraFlatPainter : ColorBasedRulerPainter {
		public HorizontalRulerUltraFlatPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
		protected override Color DefaultTabColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color TabTypeToggleBorderColor { get { return SystemColors.ControlDarkDark; } }
	}
	#endregion
	#region HorizontalRulerStyle3DPainter
	public class HorizontalRulerStyle3DPainter : ColorBasedRulerPainter {
		public HorizontalRulerStyle3DPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
		protected override Color DefaultTabColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color TabTypeToggleBorderColor { get { return SystemColors.ControlDarkDark; } }
	}
	#endregion
	#region HorizontalRulerWindowsXPPainter
	public class HorizontalRulerWindowsXPPainter : ColorBasedRulerPainter {
		public HorizontalRulerWindowsXPPainter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
		protected override Color DefaultTabColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color TabTypeToggleBorderColor { get { return SystemColors.ControlDarkDark; } }
	}
	#endregion
	#region HorizontalRulerOffice2003Painter
	public class HorizontalRulerOffice2003Painter : ColorBasedRulerPainter {
		public HorizontalRulerOffice2003Painter(HorizontalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return Office2003Colors.Default[Office2003Color.Button1]; } }
		protected override Color RulerAreaBackgroundColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
		protected override Color DefaultTabColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
		protected override Color TabTypeToggleBorderColor { get { return Office2003Colors.Default[Office2003Color.Button2]; } }
	}
	#endregion
}
