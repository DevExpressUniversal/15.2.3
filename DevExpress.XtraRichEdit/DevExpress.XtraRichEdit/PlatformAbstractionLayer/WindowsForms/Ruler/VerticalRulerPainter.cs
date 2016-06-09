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
using System.Text;
using DevExpress.XtraRichEdit.Painters;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing.Drawing2D;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Export;
namespace DevExpress.XtraRichEdit.Ruler {
	#region VerticalRulerPainter (abstract class)
	public abstract class VerticalRulerPainter : RulerPainterBase {
		#region Fields
		readonly VerticalRulerControl ruler;
		Bitmap quarterTickImage;
		Bitmap halfTickImage;
		#endregion
		protected VerticalRulerPainter(VerticalRulerControl ruler)
			: base(ruler) {
			this.ruler = ruler;
		}
		#region Properties
		public VerticalRulerControl Ruler { get { return ruler; } }
		public Bitmap QuarterTickImage { get { return quarterTickImage; } }
		public Bitmap HalfTickImage { get { return halfTickImage; } }
		#endregion
		public virtual void Initialize() {
			this.halfTickImage = GenerateHalfTickImage();
			this.quarterTickImage = GenerateQuarterTickImage();
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
		Bitmap GenerateTickImageCore(Rectangle bounds) {
			return GenerateImage(bounds, ForeColor, DrawTickmarkImage);
		}
		void DrawTickmarkImage(Graphics gr, Rectangle bounds, Brush brush) {
			gr.FillRectangle(brush, bounds);
		}
		#endregion
		protected internal Bitmap GenerateImage(Rectangle bounds, Color color, GenerateImageDelegate generateImage) {
			Size size = bounds.Size;
			size.Width = Math.Max(1, size.Width);
			size.Height = Math.Max(1, size.Height);
			bounds = new Rectangle(Point.Empty, size);
			Bitmap bmp = new Bitmap(size.Width, size.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (SolidBrush brush = new SolidBrush(color)) {
					generateImage(gr, bounds, brush);
				}
			}
			return bmp;
		}
		public virtual void DrawBackground(GraphicsCache cache) {
			DrawControlBackground(cache, ruler.ClientRectangle);
		}
		public override void Draw(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			DrawInPixels(cache);
		}
		void DrawInPixels(GraphicsCache cache) {
			RichEditControl control = ruler.RichEditControl;
			CaretPosition caretPosition = control.ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.Column);
			if (caretPosition.PageViewInfo == null)
				return;
			Graphics gr = cache.Graphics;
			try {
				int viewBoundsTop = ruler.LayoutUnitsToPixelsV(control.ViewBounds.Top + caretPosition.PageViewInfo.ClientBounds.Y);
				gr.TranslateTransform(0, viewBoundsTop);
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
		}
		protected internal override void EndDraw() {
			CacheDispose();
		}
		protected internal virtual void DrawRuler() {
			DrawRulerAreaBackground(ruler.ViewInfo.DisplayClientBounds);
			DrawActiveAreas();
			DrawTickMarks();
			DrawSpaceAreas();
			DrawHotZones();
		}
		void DrawActiveAreas() {
			ruler.ViewInfo.DisplayActiveAreaCollection.ForEach(DrawActiveArea);
		}
		void DrawSpaceAreas() {
			ruler.ViewInfo.DisplaySpaceAreaCollection.ForEach(DrawSpaceArea);
		}		
		void DrawHotZones() {
			ruler.ViewInfo.HotZones.ForEach(DrawHotZone);
		}
		void DrawTickMarks() {
			ruler.ViewInfo.RulerTickmarks.ForEach(DrawTickMark);
		}
		protected internal virtual void DrawTickmarkNumber(RulerTickmarkNumber tickmark) {
			Cache.DrawVString(tickmark.Number, ruler.TickMarkFont, Cache.GetSolidBrush(ForeColor), Rectangle.Round(tickmark.DisplayBounds), StringFormat.GenericTypographic, 270);
		}
		protected internal virtual void DrawTickmarkHalf(RulerTickmarkHalf tickmark) {
			RectangleF bounds = tickmark.DisplayBounds;
			HorizontalRulerPainter.DrawBitmap(Graphics, halfTickImage, (int)bounds.X, (int)bounds.Y);
		}
		protected internal virtual void DrawTickmarkQuarter(RulerTickmarkQuarter tickmark) {
			RectangleF bounds = tickmark.DisplayBounds;
			HorizontalRulerPainter.DrawBitmap(Graphics, quarterTickImage, (int)bounds.X, (int)bounds.Y);
		}
		protected internal virtual void DrawTickMark(RulerTickmark tickMark) {
			tickMark.Draw(this);
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, DevExpress.XtraRichEdit.Internal.PrintLayout.PageViewInfo page) {
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, Layout.CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
		}
		protected internal abstract void DrawControlBackground(GraphicsCache cache, Rectangle bounds);
		protected internal abstract void DrawRulerAreaBackground(Rectangle bounds);
		protected internal abstract void DrawActiveArea(RectangleF bounds);
		protected internal abstract void DrawSpaceArea(RectangleF bounds);
		protected internal abstract void DrawHotZone(RulerHotZone hotZone);
	}
	#endregion
	#region VerticalRulerSkinPainter
	public class VerticalRulerSkinPainter : VerticalRulerPainter {
		public VerticalRulerSkinPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		public override UserLookAndFeel LookAndFeel { get { return Ruler.LookAndFeel; } }
		protected override Color ForeColor {
			get {
				Skin skin = ReportsSkins.GetSkin(LookAndFeel);
				return skin.Properties.GetColor(RichEditPrintingSkins.SkinRulerForeColor);
			}
		}
		protected internal override int PaddingTop {
			get {
				SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[RichEditSkins.SkinVerticalRulerBackground];
				return Ruler.PixelsToLayoutUnitsV(element.ContentMargins.Top);
			}
		}
		protected internal override int PaddingBottom {
			get {
				SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[RichEditSkins.SkinVerticalRulerBackground];
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
			DrawRichEditSkinElement(cache, RichEditSkins.SkinVerticalRulerBackground, bounds);
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
		protected internal override void DrawHotZone(RulerHotZone hotZone) {
			if (hotZone.GetType() == typeof(VerticalTableHotZone))
				DrawReportsSkinElement(Cache, ReportsSkins.SkinRulerRightMargin, hotZone.DisplayBounds);
		}
	}
	#endregion
	#region ColorBasedRulerPainter (abstract class)
	public abstract class VerticalColorBasedRulerPainter : VerticalRulerPainter {
		protected VerticalColorBasedRulerPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected abstract Color ControlBackgroundColor { get; }
		protected abstract Color RulerAreaBackgroundColor { get; }
		protected abstract Color ActiveAreaColor { get; }
		protected abstract Color SpaceAreaColor { get; }
		protected virtual Color TableRowAreaColor { get { return SpaceAreaColor; } }
		protected internal override void DrawControlBackground(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(ControlBackgroundColor, bounds);
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
		protected internal override void DrawHotZone(RulerHotZone hotZone) {
			if (hotZone.GetType() == typeof(VerticalTableHotZone))
				Graphics.FillRectangle(Cache.GetSolidBrush(TableRowAreaColor), hotZone.DisplayBounds);
		}
	}
	#endregion
	#region VerticalRulerFlatPainter
	public class VerticalRulerFlatPainter : VerticalColorBasedRulerPainter {
		public VerticalRulerFlatPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
	}
	#endregion
	#region VerticalRulerUltraFlatPainter
	public class VerticalRulerUltraFlatPainter : VerticalColorBasedRulerPainter {
		public VerticalRulerUltraFlatPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
	}
	#endregion
	#region VerticalRulerStyle3DPainter
	public class VerticalRulerStyle3DPainter : VerticalColorBasedRulerPainter {
		public VerticalRulerStyle3DPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
	}
	#endregion
	#region VerticalRulerWindowsXPPainter
	public class VerticalRulerWindowsXPPainter : VerticalColorBasedRulerPainter {
		public VerticalRulerWindowsXPPainter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return SystemColors.Control; } }
		protected override Color RulerAreaBackgroundColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return SystemColors.Control; } }
	}
	#endregion
	#region VerticalRulerOffice2003Painter
	public class VerticalRulerOffice2003Painter : VerticalColorBasedRulerPainter {
		public VerticalRulerOffice2003Painter(VerticalRulerControl ruler)
			: base(ruler) {
		}
		protected override Color ControlBackgroundColor { get { return Office2003Colors.Default[Office2003Color.Button1]; } }
		protected override Color RulerAreaBackgroundColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
		protected override Color ActiveAreaColor { get { return SystemColors.Window; } }
		protected override Color SpaceAreaColor { get { return ControlPaint.Light(Office2003Colors.Default[Office2003Color.Button2]); } }
	}
	#endregion
}
