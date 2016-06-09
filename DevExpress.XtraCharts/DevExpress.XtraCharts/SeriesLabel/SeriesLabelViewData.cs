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
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class SeriesLabelViewData {
		public const double InertAngle = Math.PI / 180.0 * 10;
		ISeriesPoint point;
		string text;
		Size textSize; 
		float additionalLenght = 0;
		public string Text { get { return text; } set { text = value; } }
		public Size TextSize { get { return textSize; } }
		public ISeriesPoint Point { get { return point; } }
		public float AdditionalLenght { get { return additionalLenght; } set { additionalLenght = value; } }
		public SeriesLabelViewData(ISeriesPoint point, string text) {
			this.point = point;
			this.text = text;
		}
		PointF CalcLocationForCenterDrawing(DiagramPoint center) {
			return new PointF(MathUtils.StrongRound(center.X - textSize.Width / 2.0), MathUtils.StrongRound(center.Y - textSize.Height / 2.0));
		}
		PointF CalcLocationForFlankDrawing(DiagramPoint anchorPoint, double angle, int borderThickness) {
			if (Double.IsNaN(angle))
				return CalcLocationForCenterDrawing(anchorPoint);
			angle = GeometricUtils.NormalizeRadian(angle);
			if (angle < 0)
				angle += Math.PI * 2;
			double width = textSize.Width + borderThickness * 2;
			double height = textSize.Height + borderThickness * 2;
			DiagramPoint location;
			if (ComparingUtils.CompareDoubles(angle, 0, InertAngle) == 0)
				location = new DiagramPoint(anchorPoint.X, MathUtils.StrongRound(anchorPoint.Y) - height / 2.0);
			else if (ComparingUtils.CompareDoubles(angle, Math.PI / 2.0, InertAngle) == 0)
				location = new DiagramPoint(MathUtils.StrongRound(anchorPoint.X) - width / 2.0, anchorPoint.Y - height + 1);
			else if (ComparingUtils.CompareDoubles(angle, Math.PI, InertAngle) == 0)
				location = new DiagramPoint(anchorPoint.X - width + 1, MathUtils.StrongRound(anchorPoint.Y) - height / 2.0);
			else if (ComparingUtils.CompareDoubles(angle, 3 * Math.PI / 2.0, InertAngle) == 0)
				location = new DiagramPoint(MathUtils.StrongRound(anchorPoint.X) - width / 2.0, anchorPoint.Y);
			else if (angle > 0 && angle < Math.PI / 2.0)
				location = new DiagramPoint(anchorPoint.X, anchorPoint.Y - height + 1);
			else if (angle > Math.PI / 2.0 && angle < Math.PI)
				location = new DiagramPoint(anchorPoint.X - width + 1, anchorPoint.Y - height + 1);
			else if (angle > Math.PI && angle < 3 * Math.PI / 2.0)
				location = new DiagramPoint(anchorPoint.X - width + 1, anchorPoint.Y);
			else
				location = new DiagramPoint(anchorPoint.X, anchorPoint.Y);
			return new PointF(MathUtils.StrongRound(location.X) + borderThickness, MathUtils.StrongRound(location.Y) + borderThickness);
		}
		TextPainter CreateTextPainter(SeriesLabelBase label, TextMeasurer textMeasurer, PointF location) {
			if (label.TextOrientation == TextOrientation.Horizontal)
				return new TextPainter(text, textSize, label, textMeasurer, location, label.MaxWidth, label.MaxLineCount, false);
			bool topToBottom = label.TextOrientation == TextOrientation.TopToBottom ? true : false;
			return new VerticalRotatedTextPainter(text, textSize, label, textMeasurer, location, topToBottom, label.MaxWidth, label.MaxLineCount, false);
		}
		public void CalculateTextSize(TextMeasurer textMeasurer, SeriesLabelBase label, Font font) {
			SizeF size = textMeasurer.MeasureString(text, font, label.TextAlignment, StringAlignment.Center);
			TextPainter painter = new TextPainter(text, size, label, textMeasurer, new PointF(0, 0), label.MaxWidth, label.MaxLineCount, false);
			if (label.TextOrientation == TextOrientation.Horizontal)
				textSize = new Size(painter.RoundedBounds.Width, painter.RoundedBounds.Height);
			else
				textSize = new Size(painter.RoundedBounds.Height, painter.RoundedBounds.Width);
		}
		public TextPainter CreateTextPainterAccordingAllowedBoundsForLabels(SeriesLabelBase label, TextMeasurer textMeasurer, 
																			DiagramPoint anchorPoint, double angle, 
																			Rectangle allowedBoundsForLabelPlacing) {
			PointF location = CalcLocationForFlankDrawing(anchorPoint, angle, label.Border.ActualThickness);
			RectangleF allowedBoundsForLabelPlacingF = allowedBoundsForLabelPlacing;
			RectangleF labelBounds = new RectangleF(location, this.textSize);
			labelBounds.Inflate(label.Border.Thickness, label.Border.Thickness); 
			RectangleF labelBoundsIntoAllowedBoundsF = RectangleF.Intersect(labelBounds, allowedBoundsForLabelPlacingF);
			if (labelBoundsIntoAllowedBoundsF == labelBounds)
				return CreateTextPainter(label, textMeasurer, location);
			if (labelBoundsIntoAllowedBoundsF == RectangleF.Empty)
				return null;
			RectangleF allowedBoundsForCourrentLabelF = CalculateAllowedBoundsForCurrentLabel(angle, labelBoundsIntoAllowedBoundsF, allowedBoundsForLabelPlacingF);
			PointF horizontalCorrectedPainterLocationWithoutBorder = labelBoundsIntoAllowedBoundsF.Location.Offset(label.Border.Thickness, label.Border.Thickness);
			TextPainter labelTextPainter = new TextPainter(text, textSize, label, textMeasurer, allowedBoundsForCourrentLabelF, horizontalCorrectedPainterLocationWithoutBorder);
			CorrectTextPainterLocation(ref labelTextPainter, allowedBoundsForCourrentLabelF, angle);
			if (labelTextPainter.Height < label.Border.Thickness || labelTextPainter.Width < label.Border.Thickness)
				return null;
			else
				return labelTextPainter;
		}
		void CorrectTextPainterLocation(ref TextPainter labelTextPainter, RectangleF allowedBoundsForCourrentLabelF, double directionToLabelInRadians) {
			if (Double.IsNaN(directionToLabelInRadians)) 
				return;
			directionToLabelInRadians = GeometricUtils.NormalizeRadian(directionToLabelInRadians);
			if (directionToLabelInRadians < 0)
				directionToLabelInRadians += Math.PI * 2;
			if (0 <= directionToLabelInRadians && directionToLabelInRadians <= Math.PI) {
				MovePainterUp(ref labelTextPainter, allowedBoundsForCourrentLabelF);
			}
			else 
				return;
		}
		void MovePainterUp(ref TextPainter labelTextPainter, RectangleF allowedBoundsForCourrentLabelF) {
			float offset = (labelTextPainter.BoundsWithBorder.Bottom - allowedBoundsForCourrentLabelF.Top) - allowedBoundsForCourrentLabelF.Height;
			labelTextPainter.Offset(0, -offset);
		}
		RectangleF CalculateAllowedBoundsForCurrentLabel(double angle, RectangleF labelBoundsIntoAllowedBoundsF, RectangleF allowedBoundsForLabelPlacingF) {
			if (Double.IsNaN(angle))
				return labelBoundsIntoAllowedBoundsF;
			angle = GeometricUtils.NormalizeRadian(angle);
			if (angle < 0)
				angle += Math.PI * 2;
			float newHeight;
			float verticalOffset;
			if (0 <= angle && angle <= Math.PI) {
				newHeight = labelBoundsIntoAllowedBoundsF.Bottom - allowedBoundsForLabelPlacingF.Top;
				verticalOffset = allowedBoundsForLabelPlacingF.Top - labelBoundsIntoAllowedBoundsF.Top;
			}
			else if (Math.PI <= angle && angle <= 2 * Math.PI) {
				verticalOffset = 0;
				newHeight = allowedBoundsForLabelPlacingF.Bottom - labelBoundsIntoAllowedBoundsF.Top;
			}
			else {
				ChartDebug.Fail("Unforeseen angle value: " + angle + " rad. It is expected in interval from 0 to 2*PI.");
				verticalOffset = 0;
				newHeight = labelBoundsIntoAllowedBoundsF.Height;
			}
			RectangleF allowedBoundsForCurrentLabelF = labelBoundsIntoAllowedBoundsF;
			allowedBoundsForCurrentLabelF.Offset(0, verticalOffset);
			allowedBoundsForCurrentLabelF.Height = newHeight;
			return allowedBoundsForCurrentLabelF;
		}
		public TextPainter CreateTextPainterForCenterDrawing(SeriesLabelBase label, TextMeasurer textMeasurer, DiagramPoint center) {
															 PointF location = CalcLocationForCenterDrawing(center);
			return CreateTextPainter(label, textMeasurer, location);
		}
		public TextPainter CreateTextPainterForFlankDrawing(SeriesLabelBase label, TextMeasurer textMeasurer, 
															DiagramPoint anchorPoint, double angle) {
			PointF location = CalcLocationForFlankDrawing(anchorPoint, angle, label.Border.ActualThickness);
			return CreateTextPainter(label, textMeasurer, location);
		}
	}
}
