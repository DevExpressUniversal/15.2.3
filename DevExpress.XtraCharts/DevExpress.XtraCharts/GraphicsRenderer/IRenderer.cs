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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IRenderer {
		Matrix Transform { get; }
		bool IsRightToLeft { get; set; }
		void Clear(Color color);
		void Present();
		void Reset(object context, Rectangle bounds);
		void SaveState();
		void RestoreState();
		void SetPixelOffsetMode(PixelOffsetMode mode);
		void RestorePixelOffsetMode();
		void EnableAntialiasing(bool enable);
		void RestoreAntialiasing();
		void EnablePolygonAntialiasing(bool enable);
		void RestorePolygonAntialiasing();
		void EnablePolygonOptimization(bool enable);
		void RestorePolygonOptimization();
		void TranslateModel(PointF offset);
		void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, IHitRegion region);
		void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, IHitRegion region, bool force);
		void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, bool useSpecificCursor, IHitRegion region, bool force);
		void TranslateModel(float x, float y);
		void RotateModel(float angle);
		void SetClipping(RectangleF rect);
		void SetClipping(Region region);
		void SetClipping(GraphicsPath path);
		void SetClipping(RectangleF rect, CombineMode mode);
		void SetClipping(Region region, CombineMode mode);
		void SetClipping(GraphicsPath path, CombineMode mode);
		void RestoreClipping();
		void DrawImage(Image image, RectangleF bounds);
		void DrawImage(Image image, RectangleF bounds, RectangleF source);
		void DrawImage(Image image, RectangleF bounds, ChartImageSizeMode sizeMode);
		void DrawImage(Image image, PointF position);
		void DrawRectangle(RectangleF bounds, Pen pen);
		void DrawRectangle(RectangleF bounds, Color color, float thickness);
		void FillRectangle(RectangleF bounds, Color color);
		void FillRectangle(RectangleF bounds, Color color, RectangleFillStyle fillSyle);
		void FillRectangle(RectangleF bounds, HatchStyle hatch, Color color);
		void FillRectangle(RectangleF bounds, HatchStyle hatch, Color color, Color backColor);
		void FillRectangle(RectangleF bounds, RectangleF gradient, Color color, Color color2, LinearGradientMode mode);
		void DrawPath(FillOptionsBase fillOptions, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		void DrawPath(GraphicsPath path, Color color, int thickness);
		void DrawPath(GraphicsPath path, Pen pen);
		void FillPath(GraphicsPath path, Color color);
		void FillPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2, LinearGradientMode mode);
		void FillPath(GraphicsPath path, HatchStyle hatch, Color color, Color color2);
		void DrawText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, PointF position);
		void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, float textHeight);
		void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, float textHeight, bool useTypographicStringFormat);
		void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment);
		void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, bool useTypographicStringFormat);
		void DrawLine(PointF p1, PointF p2, Color color, LineStyle lineStyle);
		void DrawLine(PointF p1, PointF p2, Color color, int thickness);
		void DrawLine(PointF p1, PointF p2, Color color, int thickness, LineStyle lineStyle, LineCap lineCap);
		void DrawLines(Point[] points, Color color, int thickness, LineStyle lineStyle);
		void DrawLines(PointF[] points, Color color, int thickness, LineStyle lineStyle);
		void DrawLines(LineStrip strip, Color color, int thickness, LineStyle lineStyle, LineCap lineCap);
		void DrawCircle(PointF center, float radius, Color color);
		void DrawCircle(PointF center, float radius, Color color, int thickness);
		void DrawCircle(PointF center, float radius, Color color, int thickness, LineStyle lineStyle);
		void FillCircle(PointF center, float radius, Color color);
		void FillCircle(PointF center, float radius, Color color, Color color2, LinearGradientMode mode);
		void FillCircle(PointF center, float radius, Color color, Color color2);
		void FillCircle(PointF center, float radius, HatchStyle hatch, Color color, Color color2);
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color);
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2, LinearGradientMode mode);
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, RectangleF gradientRect, Color color, Color color2, LinearGradientMode mode);
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2);
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, HatchStyle hatch, Color color, Color color2);
		void DrawPie(Pie pie, PointF basePoint, Color color, int thickness);
		void DrawPie(Pie pie, PointF basePoint, float startAngle, float sweepAngle, Color color, int thickness);
		void FillPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, Color color, Color color2);
		void FillPie(Pie pie, PointF basePoint, HatchStyle hatchStyle, Color color, Color backColor);
		void DrawPolygon(LineStrip strip, Color color, int thickness);
		void FillPolygon(LineStrip strip, Color color);
		void FillPolygon(LineStrip strip, HatchStyle hatch, Color color);
		void FillPolygon(LineStrip strip, HatchStyle hatch, Color color, Color backColor);
		void FillPolygonGradient(LineStrip strip, RectangleF gradient, Color color, Color color2, LinearGradientMode mode);
		void FillPolygonGradient(LineStrip strip, RectangleF bounds, Color color, Color color2);
		void DrawBezier(BezierStrip strip, Color color, float thickness);
		void DrawBezier(BezierStrip strip, Color color, float thickness, LineStyle lineStyle);
		void FillBezier(BezierRangeStrip strip, Color color);
		void FillBezier(BezierRangeStrip strip, RectangleF gradient, Color color, Color color2, LinearGradientMode mode);
		void FillBezier(BezierRangeStrip strip, HatchStyle hatch, Color color);
		void FillBezier(BezierRangeStrip strip, HatchStyle hatch, Color color, Color color2);
		void DrawArc(PointF center, float radius, float startAngleDegree, float sweepAngleDegree, Color color, int thickness, DashStyle dashStyle);
	}
}
