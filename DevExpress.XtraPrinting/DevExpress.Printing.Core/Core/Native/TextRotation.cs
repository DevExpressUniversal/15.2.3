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
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Native.TextRotation {
	public enum TextRotation {
		LeftTop,
		CenterTop,
		RightTop,
		LeftCenter,
		CenterCenter,
		RightCenter,
		LeftBottom,
		CenterBottom,
		RightBottom
	}
	public class StringFormatContainer : IDisposable {
		StringFormat stringFormat;
		bool dispose;
		public StringFormatContainer(StringFormat stringFormat, bool dispose) {
			this.stringFormat = stringFormat;
			this.dispose = dispose;
		}
		public StringFormat Value {
			get { return stringFormat; }
		}
		public void Dispose() {
			if(stringFormat != null && dispose) {
				stringFormat.Dispose();
			}
			stringFormat = null;
		}
	}
	public class RotatedTextHelper : IDisposable {
		static StringFormatContainer ValidateStringFormat(StringFormat sf, string text) {
			if(text.IndexOf('\n') < 0) {
				StringFormat format = StringFormat.GenericTypographic.Clone() as StringFormat;
				format.Alignment = sf.Alignment;
				format.LineAlignment = sf.LineAlignment;
				if((sf.FormatFlags & StringFormatFlags.NoWrap) != 0)
					format.FormatFlags |= StringFormatFlags.NoWrap;
				format.FormatFlags &= ~StringFormatFlags.NoClip;
				format.FormatFlags &= ~StringFormatFlags.LineLimit;
				return new StringFormatContainer(format, true);
			} else
				return new StringFormatContainer(sf, false);
		}
		Font font;
		StringFormat prototype;
		StringFormatContainer sfContainer;
		string text;
		public StringFormat StringFormat {
			get {
				if(sfContainer == null)
					sfContainer = ValidateStringFormat(prototype, text);				
				return sfContainer.Value;
			}
		}
		public RotatedTextHelper(Font font, StringFormat prototype, string text) {
			this.font = font;
			this.prototype = prototype;
			this.text = text;
		}
		void IDisposable.Dispose() {
			if(sfContainer != null) {
				sfContainer.Dispose();
				sfContainer = null;
			}
		}
		public Rectangle GetBounds(Point pt, float angle, TextRotation textRotation, float width, GraphicsUnit pageUnit, Measurer measurer) {
			Size size = MeasureString(text, width, pageUnit, measurer);
			return GetBounds(pt, size, angle, textRotation);
		}
		Size MeasureString(string text, float width, GraphicsUnit pageUnit, Measurer measurer) {
			if(width <= 0)
				return Size.Ceiling(measurer.MeasureString(text, font, pageUnit));
			return Size.Ceiling(measurer.MeasureString(text, font, width, StringFormat, pageUnit));
		}
		static Rectangle GetBounds(Point location, Size size, float angle, TextRotation textRotation) {
			Rectangle rect = new Rectangle(location, size);
			double radAngle = angle * Math.PI / 180;
			PointF offset = GetOffset(rect, radAngle, textRotation);
			RectangleF _rect = new RectangleF(rect.Left + offset.X, rect.Top + offset.Y, rect.Width, rect.Height);
			float halfHeightXProection = (float)((_rect.Height / 2) * Math.Sin(radAngle));
			float halfWidthXProection = (float)((_rect.Width / 2) * (1 - Math.Cos(radAngle)));
			float halfHeightYProection = (float)((_rect.Height / 2) * (1 - Math.Cos(radAngle)));
			float halfWidthYProection = (float)((_rect.Width / 2) * Math.Sin(radAngle));
			PointF pt1 = new PointF(
				(float)(_rect.Left + halfHeightXProection + halfWidthXProection),
				(float)(_rect.Top + halfHeightYProection - halfWidthYProection)
				);
			PointF pt2 = new PointF(
				(float)(_rect.Right + halfHeightXProection - halfWidthXProection),
				(float)(_rect.Top + halfHeightYProection + halfWidthYProection)
				);
			PointF pt3 = new PointF(
				(float)(_rect.Right - halfHeightXProection - halfWidthXProection),
				(float)(_rect.Bottom - halfHeightYProection + halfWidthYProection)
				);
			PointF pt4 = new PointF(
				(float)(_rect.Left - halfHeightXProection + halfWidthXProection),
				(float)(_rect.Bottom - halfHeightYProection - halfWidthYProection)
				);
			float x1 = Math.Min(Math.Min(pt1.X, pt2.X), Math.Min(pt3.X, pt4.X));
			float y1 = Math.Min(Math.Min(pt1.Y, pt2.Y), Math.Min(pt3.Y, pt4.Y));
			float x2 = Math.Max(Math.Max(pt1.X, pt2.X), Math.Max(pt3.X, pt4.X));
			float y2 = Math.Max(Math.Max(pt1.Y, pt2.Y), Math.Max(pt3.Y, pt4.Y));
			return new Rectangle((int)Math.Ceiling(x1), (int)Math.Ceiling(y1), (int)Math.Ceiling(x2 - x1), (int)Math.Ceiling(y2 - y1));
		}
		static PointF GetOffset(Rectangle rect, double radAngle, TextRotation textRotation) {
			float xOffset = 0;
			float yOffset = 0;
			switch(textRotation) {
				case TextRotation.LeftTop: {
						xOffset = (float)(-(rect.Width / 2) * (1 - Math.Cos(radAngle))
							- (rect.Height / 2) * Math.Sin(radAngle));
						yOffset = (float)((rect.Width / 2) * Math.Sin(radAngle)
							- (rect.Height / 2) * (1 - Math.Cos(radAngle))
							);
					}; break;
				case TextRotation.CenterTop: {
						xOffset = (float)(-(rect.Height / 2) * Math.Sin(radAngle));
						yOffset = (float)(-(rect.Height / 2) * (1 - Math.Cos(radAngle)));
					}; break;
				case TextRotation.RightTop: {
						xOffset = (float)((rect.Width / 2) * (1 - Math.Cos(radAngle))
							- (rect.Height / 2) * Math.Sin(radAngle)
							);
						yOffset = (float)(-(rect.Width / 2) * Math.Sin(radAngle)
							- (rect.Height / 2) * (1 - Math.Cos(radAngle)));
					}; break;
				case TextRotation.LeftCenter: {
						xOffset = (float)(-(rect.Width / 2) * (1 - Math.Cos(radAngle))
							);
						yOffset = (float)((rect.Width / 2) * Math.Sin(radAngle));
					}; break;
				case TextRotation.RightCenter: {
						xOffset = (float)((rect.Width / 2) * (1 - Math.Cos(radAngle))
							);
						yOffset = (float)(-(rect.Width / 2) * Math.Sin(radAngle));
					}; break;
				case TextRotation.LeftBottom: {
						xOffset = (float)(-(rect.Width / 2) * (1 - Math.Cos(radAngle))
							+ (rect.Height / 2) * Math.Sin(radAngle)
							);
						yOffset = (float)((rect.Width / 2) * Math.Sin(radAngle) +
							(rect.Height / 2) * (1 - Math.Cos(radAngle)));
					}; break;
				case TextRotation.CenterBottom: {
						xOffset = (float)((rect.Height / 2) * Math.Sin(radAngle));
						yOffset = (float)((rect.Height / 2) * (1 - Math.Cos(radAngle)));
					}; break;
				case TextRotation.RightBottom: {
						xOffset = (float)((rect.Width / 2) * (1 - Math.Cos(radAngle))
							+ (rect.Height / 2) * Math.Sin(radAngle)
							);
						yOffset = (float)(
							-(rect.Width / 2) * Math.Sin(radAngle) +
							(rect.Height / 2) * (1 - Math.Cos(radAngle)));
					}; break;
				default: {
						xOffset = 0;
						yOffset = 0;
					}; break;
			}
			return new PointF(xOffset, yOffset);
		}
	}
	public class RotatedTextPainter {
		public static void DrawRotatedString(IGraphics g, string text, Font font, Brush br, Rectangle bounds, StringFormat sf, float angle, bool drawRect, int width, TextAlignment textAlignment) {
			angle = -angle;
			using(RotatedTextHelper helper = new RotatedTextHelper(font, sf, text)) {
				Rectangle frameBounds = helper.GetBounds(bounds.Location, angle, TextRotation.CenterCenter, width, g.PageUnit, g.Measurer);
				Point p = GetRotatedTextPoint(bounds, frameBounds, textAlignment);
				int dx = frameBounds.X - bounds.X;
				int dy = bounds.Y - frameBounds.Y;
				p.X -= dx;
				p.Y += dy;
				DrawRotatedString(g, text, font, br, p, helper.StringFormat, angle, TextRotation.CenterCenter, drawRect, width, textAlignment);
			}
		}
		static Region EnlargeRegionByPixel(Region sourceRegion) {
			Region tempRegion, result;
			tempRegion = sourceRegion.Clone();
			result = sourceRegion.Clone();
			try {
				tempRegion.Translate(0, 1);
				result.Union(tempRegion);
				tempRegion.Translate(1, 0);
				result.Union(tempRegion);
				tempRegion.Translate(0, -1);
				result.Union(tempRegion);
				tempRegion.Translate(-1, 0);
				result.Union(tempRegion);
				tempRegion.Translate(1, 1);
				result.Union(tempRegion);
				tempRegion.Translate(1, -1);
				result.Union(tempRegion);
				tempRegion.Translate(-1, 1);
				result.Union(tempRegion);
				tempRegion.Translate(-1, -1);
				result.Union(tempRegion);
			} finally {
				tempRegion.Dispose();
			}
			return result;
		}
		static void DrawRotatedString(IGraphics g, string text, Font font, Brush br, Point pt, StringFormat sf, float angle, TextRotation textRotation, bool drawRect, int width, TextAlignment textAlignment) {
			g.SaveTransformState();
			try {
				SizeF sizeF = width > 0 ? g.Measurer.MeasureString(text, font, width, sf, g.PageUnit) :
					g.Measurer.MeasureString(text, font, g.PageUnit);
				Size size = Size.Ceiling(sizeF);
				Rectangle rect = new Rectangle(pt, size);
				Point offset = GetOffset(rect, textRotation);
				rect.Offset(-offset.X, -offset.Y);
				g.ResetTransform();
				g.RotateTransform(angle);
				g.TranslateTransform(offset.X, offset.Y, MatrixOrder.Append);
				g.ApplyTransformState(MatrixOrder.Append, false);
				if(drawRect)
					g.DrawRectangle(new Pen(br), rect);
				if(textAlignment >= TextAlignment.TopJustify)
					JustifiedStringPainter.DrawString(text, g, font, br, rect, sf, false);
				else
					g.DrawString(text, font, br, rect, sf);
			} finally {
				g.ResetTransform();
				g.ApplyTransformState(MatrixOrder.Append, true);
			}
		}
		static Point GetOffset(Rectangle rect, TextRotation textRotation) {
			Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			int xOffset = 0;
			int yOffset = 0;
			switch(textRotation) {
				case TextRotation.LeftTop: {
						xOffset = rect.Left;
						yOffset = rect.Top;
					}; break;
				case TextRotation.CenterTop: {
						xOffset = center.X;
						yOffset = rect.Top;
					}; break;
				case TextRotation.RightTop: {
						xOffset = rect.Right;
						yOffset = rect.Top;
					}; break;
				case TextRotation.LeftCenter: {
						xOffset = rect.Left;
						yOffset = center.Y;
					}; break;
				case TextRotation.CenterCenter: {
						xOffset = center.X;
						yOffset = center.Y;
					}; break;
				case TextRotation.RightCenter: {
						xOffset = rect.Right;
						yOffset = center.Y;
					}; break;
				case TextRotation.LeftBottom: {
						xOffset = rect.Left;
						yOffset = rect.Bottom;
					}; break;
				case TextRotation.CenterBottom: {
						xOffset = center.X;
						yOffset = rect.Bottom;
					}; break;
				case TextRotation.RightBottom: {
						xOffset = rect.Right;
						yOffset = rect.Bottom;
					}; break;
			}
			return new Point(xOffset, yOffset);
		}
		static Point GetRotatedTextPoint(Rectangle clientRectangle, Rectangle frameBounds, TextAlignment alignment) {
			Point result = Point.Empty;
			switch(alignment) {
				case TextAlignment.TopLeft:
				case TextAlignment.TopCenter:
				case TextAlignment.TopRight:
				case TextAlignment.TopJustify:
					result.Y = clientRectangle.Top;
					break;
				case TextAlignment.MiddleLeft:
				case TextAlignment.MiddleCenter:
				case TextAlignment.MiddleRight:
				case TextAlignment.MiddleJustify:
					result.Y = clientRectangle.Top + clientRectangle.Height / 2 - frameBounds.Height / 2;
					break;
				case TextAlignment.BottomLeft:
				case TextAlignment.BottomCenter:
				case TextAlignment.BottomRight:
				case TextAlignment.BottomJustify:
					result.Y = clientRectangle.Bottom - frameBounds.Height;
					break;
			}
			switch(alignment) {
				case TextAlignment.TopLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.BottomLeft:
					result.X = clientRectangle.Left;
					break;
				case TextAlignment.TopCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.BottomCenter:
				case TextAlignment.TopJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.BottomJustify:
					result.X = clientRectangle.Left + clientRectangle.Width / 2 - frameBounds.Width / 2;
					break;
				case TextAlignment.TopRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.BottomRight:
					result.X = clientRectangle.Right - frameBounds.Width;
					break;
			}
			return result;
		}
	}
}
