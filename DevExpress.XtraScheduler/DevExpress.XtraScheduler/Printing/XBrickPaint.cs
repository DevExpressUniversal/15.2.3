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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class XBrickPaint : XPaint {
		PrintingHelper helper;
		AppointmentViewInfo appointmentViewInfo;
		public XBrickPaint(SchedulerPrinterParameters printerParameters, IBrickGraphics brickGraphics) {
			this.helper = new PrintingHelper(printerParameters, brickGraphics);
			this.appointmentViewInfo = null;
		}
		public PrintingHelper Helper { get { return helper; } }
		public AppointmentViewInfo AppointmentViewInfo { get { return this.appointmentViewInfo; } set { this.appointmentViewInfo = value; } }
		#region Not implemented functions
		public override void MultiColorDrawString(GraphicsCache cache, MultiColorDrawStringParams e) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.MultiColorDrawString not implemented");
#endif
		}
		public override void FillGradientRectangle(Graphics g, Brush brush, Rectangle r) {
			SolidBrush solidBrush = brush as SolidBrush;
			if (solidBrush != null) {
				AppearanceObject appearance = new AppearanceObject();
				appearance.BackColor = solidBrush.Color;
				Helper.DrawBackgroundBrick(appearance, r, this.appointmentViewInfo);
				appearance.Dispose();
			} else {
				Helper.DrawBackgroundBrick(brush, r);
			}
		}
		public override void FillRegion(Graphics g, Brush brush, Region reg) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.FillRegion not implemented");
#endif
		}
		public override void DrawCheckBox(Graphics g, Rectangle r, System.Windows.Forms.ButtonState state) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.DrawCheckBox not implemented");
#endif
		}
		public override void DrawRadioButton(Graphics g, Rectangle r, System.Windows.Forms.ButtonState state) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.DrawRadioButton not implemented");
#endif
		}
		protected override void DrawImageCore(System.Drawing.Graphics g, Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes attr) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.DrawImageCore not implemented");
#endif
		}
		public override void DrawFocusRectangle(Graphics g, Rectangle r, Color foreColor, Color backColor) {
#if DEBUG
			System.Diagnostics.Debug.Assert(false, "Method not implemented", "Method XBrickPaint.DrawFocusRectangle not implemented");
#endif
		}
		#endregion
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth) {
			SizeF size = base.CalcTextSize(g, s, font, strFormat, maxWidth);
			if (Math.Round(size.Width) != size.Width)
				size.Width++;
			return size;
		}
		public override void DrawImage(GraphicsCache info, object images, int imageIndex, Rectangle rect, bool enabled) {
			rect = info.CalcRectangle(rect);
			DrawImage(info.Graphics, ImageCollection.GetImageListImage(images, imageIndex), rect, new Rectangle(Point.Empty, rect.Size), enabled);
		}
		public override void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect, ImageAttributes attributes) {
			Helper.DrawImageBrick(image, destRect);
		}
		public override void DrawLine(Graphics g, Pen pen, Point pt1, Point pt2) {
			Helper.DrawLine(pen, pt1, pt2);
		}
		public override void DrawImage(Graphics g, Image image, Rectangle destRect, int srcX, int srcY,
			int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes attr) {
			Helper.DrawImageBrick(image, destRect);
		}
		public override void DrawPolygon(Graphics g, Pen pen, Point[] points) {
			Helper.DrawPolygon(pen, points);
		}
		public override void FillPolygon(Graphics g, Brush brush, Point[] points) {
			Helper.FillPolygon(brush, points);
		}
		public override void DrawRectangle(Graphics g, Brush brush, Rectangle r) {
			FillRectangle(g, brush, new Rectangle(r.X, r.Y, 1, r.Height));
			FillRectangle(g, brush, new Rectangle(r.X, r.Y, r.Width, 1));
			FillRectangle(g, brush, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			FillRectangle(g, brush, new Rectangle(r.X, r.Bottom - 1, r.Width, 1));
		}
		public override void DrawRectangle(Graphics g, Pen pen, Rectangle r) {
			Helper.DrawLine(pen, new PointF(r.X, r.Y), new PointF(r.X, r.Y + r.Height));
			Helper.DrawLine(pen, new PointF(r.X, r.Y), new PointF(r.X + r.Width, r.Y));
			Helper.DrawLine(pen, new PointF(r.Right - pen.Width, r.Y), new PointF(r.Right - pen.Width, r.Y + r.Height));
			Helper.DrawLine(pen, new PointF(r.X, r.Bottom - pen.Width), new PointF(r.X + r.Width, r.Bottom - pen.Width));
		}
		public override void FillRectangle(Graphics g, Brush brush, int l, int t, int w, int h) {
			FillRectangle(g, brush, new Rectangle(l, t, w, h));
		}
		public override void FillRectangle(Graphics g, Brush brush, Rectangle r) {
			SolidBrush solidBrush = brush as SolidBrush;
			if (solidBrush != null) {
				AppearanceObject appearance = new AppearanceObject();
				appearance.BackColor = solidBrush.Color;
				Helper.DrawBackgroundBrick(appearance, r, this.appointmentViewInfo);
				appearance.Dispose();
			} else {
				Helper.DrawBackgroundBrick(brush, r);
			}
		}
		protected override void InternalDrawString(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat) {
			DrawStringCore(cache, s, font, r, foreBrush, strFormat, 0);
		}
		public override void DrawVString(GraphicsCache cache, string text, Font font, Brush foreBrush, Rectangle rect, StringFormat strFormat, int angle) {
			DrawStringCore(cache, text, font, rect, foreBrush, strFormat, -angle);
		}
		protected internal virtual void DrawStringCore(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat, float angle) {
			SolidBrush solidBrush = foreBrush as SolidBrush;
			if (solidBrush != null) {
				AppearanceObject appearance = new AppearanceObject();
				appearance.ForeColor = solidBrush.Color;
				appearance.Font = font;
				Helper.DrawTextBrick(s, appearance, r, strFormat, angle, this.appointmentViewInfo);
				appearance.Dispose();
			}
		}
		public Point SetOffset(Point offset) {
			return Helper.SetOffset(offset);
		}
	}
}
