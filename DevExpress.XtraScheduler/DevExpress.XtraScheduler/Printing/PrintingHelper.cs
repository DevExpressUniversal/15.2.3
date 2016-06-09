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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class PrintingHelper {
		Point offset = Point.Empty;
		SchedulerPrinterParameters printerParameters;
		IBrickGraphics brickGraphics;
		public PrintingHelper(SchedulerPrinterParameters printerParameters, IBrickGraphics brickGraphics) {
			this.printerParameters = printerParameters;
			this.brickGraphics = brickGraphics;
		}
		internal IBrickGraphics BrickGraphics {
			get { return brickGraphics; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("value");
				brickGraphics = value;
			}
		}
		IPrintingSystem PS { get { return printerParameters.PS; } }
		RectangleF ClipRectangle { get { return printerParameters.GInfo.Graphics.ClipBounds; } }
		public void InsertPageBreak(float pos) {
			PS.InsertPageBreak(pos);
		}
		protected void SetDefaultStyle(AppearanceObject appearance) {
			SetDefaultStyle(appearance, BorderSide.None, Color.Empty, 1);
		}
		protected void SetDefaultStyle(AppearanceObject appearance, BorderSide border, Color borderColor, int lineWidth) {
			if (appearance != null) {
				if (appearance.Font != null)
					appearance.Font = (Font)appearance.Font.Clone();
				BrickGraphics.DefaultBrickStyle = AppearanceHelper.CreateBrick(appearance, border, borderColor, lineWidth);
			}
		}
		IBrick DrawBrickCore(IBrick brick, RectangleF rect) {
			rect.Offset(offset);
			rect.Intersect(ClipRectangle);
			return BrickGraphics.DrawBrick(brick, rect);
		}
		IBrick DrawPSBrickCore(string typeName, RectangleF rect) {
			if (rect.Width == 0 || rect.Height == 0)
				return null;
			IBrick brick = PS.CreateBrick(typeName);
			brick = DrawBrickCore(brick, rect);
			return brick;
		}
		protected internal void DrawTextBrick(string text, AppearanceObject appearance, RectangleF bounds, StringFormat strFormat, float angle, AppointmentViewInfo viewInfo) {
			SetDefaultStyle(appearance, BorderSide.None, Color.Empty, 0);
			bounds.Width += 1;
			bounds.Height += 1;
			if (bounds.Width == 0 || bounds.Height == 0)
				return;
			SchedulerLabelBrick brick = new SchedulerLabelBrick(viewInfo);
			brick = (SchedulerLabelBrick)DrawBrickCore(brick, bounds);
			ILabelBrick labelBrick = (ILabelBrick)brick;
			labelBrick.Text = text;
			labelBrick.BackColor = Color.Empty;
			BrickStringFormat brickStringFormat = GetBrickStringFormat(strFormat, angle);
			labelBrick.StringFormat = brickStringFormat;
			labelBrick.Padding = new PaddingInfo(0, 0, 0, 0);
			labelBrick.Angle = angle;
		}
		BrickStringFormat GetBrickStringFormat(StringFormat strFormat, float angle) {
			StringFormat actualStrFormat = CalculateActualStringFormat(strFormat, angle);
			actualStrFormat.FormatFlags |= StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;
			actualStrFormat.Trimming = StringTrimming.Character;
			BrickStringFormat brickStringFormat = new BrickStringFormat(actualStrFormat);
			brickStringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			return brickStringFormat;
		}
		protected internal virtual StringFormat CalculateActualStringFormat(StringFormat strFormat, float angle) {
			if (angle < 0)
				angle += 360;
			if (angle == 90)
				return CalculateCounterclockwiseStringFormat(strFormat);
			if (angle == 270)
				return CalculateClockwiseStringFormat(strFormat);
			return strFormat;
		}
		protected internal virtual StringFormat CalculateClockwiseStringFormat(StringFormat strFormat) {
			StringFormat result = (StringFormat)strFormat.Clone();
			result.LineAlignment = strFormat.Alignment;
			result.Alignment = TransformStringAlignment(strFormat.LineAlignment);
			return result;
		}
		protected internal virtual StringFormat CalculateCounterclockwiseStringFormat(StringFormat strFormat) {
			StringFormat result = (StringFormat)strFormat.Clone();
			result.LineAlignment = TransformStringAlignment(strFormat.Alignment);
			result.Alignment = strFormat.LineAlignment;
			return result;
		}
		protected internal virtual StringAlignment TransformStringAlignment(StringAlignment alignment) {
			switch (alignment) {
				case StringAlignment.Center:
					return StringAlignment.Center;
				case StringAlignment.Far:
					return StringAlignment.Near;
				case StringAlignment.Near:
					return StringAlignment.Far;
				default:
					return StringAlignment.Near;
			}
		}
		public void DrawImageBrick(Image image, Rectangle bounds) {
			ImageBrick brick = (ImageBrick)DrawPSBrickCore("ImageBrick", bounds);
			if (brick == null)
				return;
			brick.Image = (Image)image.Clone();
			brick.BackColor = Color.Empty;
		}
		public void DrawBackgroundBrick(AppearanceObject appearance, Rectangle bounds, AppointmentViewInfo viewInfo) {
			SetDefaultStyle(appearance);
			SchedulerSeparableBrick brick = new SchedulerSeparableBrick(viewInfo);
			brick = (SchedulerSeparableBrick)DrawBrickCore(brick, bounds);
			if (brick == null)
				return;
			brick.BackColor = appearance.BackColor;
			brick.BorderColor = appearance.BorderColor;
		}
		public void DrawBackgroundBrick(Brush brush, RectangleF bounds) {
			BrushBrick brick = new BrushBrick(brush);
			DrawBrickCore(brick, bounds);
		}
		public void DrawPolygon(Pen pen, Point[] points) {
			Rectangle bounds = GetPointBounds(points);
			Point[] relativePoints = GetRelativePoints(points, bounds);
			PolygonBrick brick = new PolygonBrick(pen, relativePoints);
			DrawBrickCore(brick, bounds);
		}
		public void FillPolygon(Brush brush, Point[] points) {
			Rectangle bounds = GetPointBounds(points);
			Point[] relativePoints = GetRelativePoints(points, bounds);
			PolygonBrick brick = new PolygonBrick(brush, relativePoints);
			DrawBrickCore(brick, bounds);
		}
		protected internal virtual Point[] GetRelativePoints(Point[] points, RectangleF parentBounds) {
			Matrix matrix = new Matrix();
			matrix.Translate(-parentBounds.X, -parentBounds.Y);
			Point[] result = new Point[points.Length];
			points.CopyTo(result, 0);
			matrix.TransformPoints(result);
			return result;
		}
		public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
			RectangleF bounds = RectF.FromPoints(pt1, pt2);
			if (bounds.Width == 0)
				bounds.Width = pen.Width;
			if (bounds.Height == 0)
				bounds.Height = pen.Width;
			IBrick brick = DrawPSBrickCore("LineBrick", bounds);
			if (brick == null)
				return;
			ILineBrick lineBrick = (ILineBrick)brick;
			lineBrick.LineWidth = pen.Width;
			lineBrick.LineStyle = pen.DashStyle;
			lineBrick.ForeColor = pen.Color;
			lineBrick.CalculateDirection(pt1, pt2);
		}
		public Point SetOffset(Point pt) {
			Point oldOffset = offset;
			offset = pt;
			return oldOffset;
		}
		public void IncreaseOffset(Point delta) {
			offset.X += delta.X;
			offset.Y += delta.Y;
		}
		public static Rectangle GetPointBounds(Point[] points) {
			int length = points.Length;
			if (length == 0)
				return Rectangle.Empty;
			int top = Int32.MaxValue, left = Int32.MaxValue;
			int bottom = Int32.MinValue, right = Int32.MinValue;
			for (int i = 0; i < length; i++) {
				Point pt = points[i];
				top = Math.Min(top, pt.Y);
				bottom = Math.Max(bottom, pt.Y);
				left = Math.Min(left, pt.X);
				right = Math.Max(right, pt.X);
			}
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
}
