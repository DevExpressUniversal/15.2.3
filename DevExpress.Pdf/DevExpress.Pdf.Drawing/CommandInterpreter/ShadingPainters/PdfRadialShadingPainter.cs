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
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfRadialShadingPainter : PdfShadingPainter { 
		const int degree90 = 90;
		const int degree360 = 360;
		static double GetDistance(PointF p1, PointF p2) {
			double dx = p2.X - p1.X;
			double dy = p2.Y - p1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		readonly PdfRadialShading shading;
		readonly float startingCircleCenterX;
		readonly float startingCircleCenterY;
		readonly int startingCircleRadius;
		readonly float endingCircleCenterX;
		readonly float endingCircleCenterY;
		readonly int endingCircleRadius;
		readonly int maxShadingSide;
		float DistanceBetweenCircleCenters { 
			get {
				float dx = endingCircleCenterX - startingCircleCenterX;
				float dy = endingCircleCenterY - startingCircleCenterY;
				return (float)Math.Sqrt(dx * dx + dy * dy);
			}
		}
		float TangentAngle { 
			get {
				float distanceBetweenCircleCenters = DistanceBetweenCircleCenters;
				return distanceBetweenCircleCenters == 0 ? degree360 : (float)(Math.Acos(Math.Abs(endingCircleRadius - startingCircleRadius) / distanceBetweenCircleCenters) * RadianToDegreeFactor);
			}
		}
		float StartingOuterSpaceDistance {
			get {
				int dr = endingCircleRadius - startingCircleRadius;
				if (dr == 0)
					return maxShadingSide * 2;
				return dr > 0 ? (startingCircleRadius * DistanceBetweenCircleCenters / dr) : ((maxShadingSide * 2 - startingCircleRadius) * DistanceBetweenCircleCenters / -dr);
			}
		}
		float EndingOuterSpaceDistance { 
			get {
				int dr = endingCircleRadius - startingCircleRadius;
				if (dr == 0)
					return maxShadingSide * 2;
				return dr > 0 ? ((maxShadingSide * 2 - endingCircleRadius) * DistanceBetweenCircleCenters / dr) : (endingCircleCenterX * DistanceBetweenCircleCenters / -dr);
			}
		}
		public PdfRadialShadingPainter(PdfRadialShading shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
				: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) { 
			this.shading = shading;
			maxShadingSide = Math.Max(bitmapWidth, bitmapHeight);
			PdfPoint shadingStartingCircleCenter = shading.StartingCircleCenter;
			PdfPoint shadingEndingCircleCenter = shading.EndingCircleCenter;
			IShadingCoordsConverter coordsConverter = CoordsConverter;
			PointF startingCircleCenter = coordsConverter.Convert(shadingStartingCircleCenter);
			PointF startingCircleLeftBound = coordsConverter.Convert(new PdfPoint(shadingStartingCircleCenter.X - shading.StartingCircleRadius, shadingStartingCircleCenter.Y));
			PointF endingCircleCenter = coordsConverter.Convert(shadingEndingCircleCenter);
			PointF endingCircleLeftBound = coordsConverter.Convert(new PdfPoint(shadingEndingCircleCenter.X - shading.EndingCircleRadius, shadingEndingCircleCenter.Y));
			startingCircleCenterX = startingCircleCenter.X;
			startingCircleCenterY = startingCircleCenter.Y;
			endingCircleCenterX = endingCircleCenter.X;
			endingCircleCenterY = endingCircleCenter.Y;
			startingCircleRadius = Convert.ToInt32(GetDistance(startingCircleLeftBound, startingCircleCenter));
			endingCircleRadius = Convert.ToInt32(GetDistance(endingCircleLeftBound, endingCircleCenter));
		}
		GraphicsPath CreateStartingOuterSpaceGraphicsPath() { 
			GraphicsPath graphicsPath = new GraphicsPath();
			float startingCircleDiameter = startingCircleRadius * 2;
			int dr = endingCircleRadius - startingCircleRadius;
			if (dr == 0) { 
				graphicsPath.AddArc(new RectangleF(-startingCircleRadius, -startingCircleRadius, startingCircleDiameter, startingCircleDiameter), degree90, Degree180);
				graphicsPath.AddArc(new RectangleF(-StartingOuterSpaceDistance - startingCircleRadius, -startingCircleRadius, startingCircleDiameter, startingCircleDiameter), -degree90, -Degree180); 
			}
			else if (dr > 0) {
				float distance = StartingOuterSpaceDistance;
				float tangentAngle = TangentAngle;
				graphicsPath.AddArc(new RectangleF(-startingCircleRadius, -startingCircleRadius, startingCircleDiameter, startingCircleDiameter), Degree180 - tangentAngle, tangentAngle * 2);
				graphicsPath.AddLine(-distance, 0, -distance, 0);
			}
			else  { 
				float tangentAngle = TangentAngle;
				float sweepAngle = degree360 - tangentAngle * 2;
				graphicsPath.AddArc(new RectangleF(-startingCircleRadius, -startingCircleRadius, startingCircleDiameter, startingCircleDiameter), tangentAngle, sweepAngle);
				float outerCircleMaxRadius = maxShadingSide * 2;
				float outerCircleMaxDiameter = outerCircleMaxRadius * 2;
				graphicsPath.AddArc(new RectangleF(-StartingOuterSpaceDistance - outerCircleMaxRadius, -outerCircleMaxRadius, outerCircleMaxDiameter, outerCircleMaxDiameter), -tangentAngle, -sweepAngle);   
			}
			graphicsPath.CloseFigure(); 
			return graphicsPath;
		}
		GraphicsPath CreateEndingOuterSpaceGraphicsPath() { 
			GraphicsPath graphicsPath = new GraphicsPath();
			float endingCircleDiameter = endingCircleRadius * 2;
			int dr = endingCircleRadius - startingCircleRadius;
			if (dr == 0) { 
				graphicsPath.AddArc(new RectangleF(-endingCircleRadius, -endingCircleRadius, endingCircleDiameter, endingCircleDiameter), degree90, Degree180);
				graphicsPath.AddArc(new RectangleF(EndingOuterSpaceDistance - endingCircleRadius, -endingCircleRadius, endingCircleDiameter, endingCircleDiameter), -degree90, -Degree180); 
			}
			else if (dr < 0) {
				float distance = EndingOuterSpaceDistance;
				float tangentAngle = TangentAngle;
				graphicsPath.AddArc(new RectangleF(-endingCircleRadius, -endingCircleRadius, endingCircleDiameter, endingCircleDiameter), tangentAngle, degree360 - tangentAngle * 2);
				graphicsPath.AddLine(distance, 0, distance, 0);
			}
			else  { 
				float tangentAngle = TangentAngle;
				float startAngle = Degree180 - tangentAngle;
				float sweepAngle = tangentAngle * 2;
				graphicsPath.AddArc(new RectangleF(-endingCircleRadius, -endingCircleRadius, endingCircleDiameter, endingCircleDiameter), startAngle, sweepAngle);
				float outerCircleMaxRadius = maxShadingSide * 2;
				float outerCircleMaxDiameter = outerCircleMaxRadius * 2;
				graphicsPath.AddArc(new RectangleF(EndingOuterSpaceDistance - outerCircleMaxRadius, -outerCircleMaxRadius, outerCircleMaxDiameter, outerCircleMaxDiameter), -startAngle, -sweepAngle); 
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}
		void PaintGraphicsPath(Graphics graphics, GraphicsPath graphicsPath, SolidBrush brush, float x, float y) { 
			GraphicsState graphicsState = graphics.Save();
			try {  
				graphics.TranslateTransform(x, y);
				graphics.RotateTransform((float) (Math.Atan2(endingCircleCenterY - startingCircleCenterY, endingCircleCenterX - startingCircleCenterX) * RadianToDegreeFactor));
				graphics.FillPath(brush, graphicsPath);
			}
			finally { 
				graphics.Restore(graphicsState);
			}
		}
		protected override void Paint(Graphics graphics) {
			base.Paint(graphics);
			IShadingColorConverter colorConverter = ColorConverter;
			if (startingCircleRadius != 0 || endingCircleRadius != 0) { 
				float dx = endingCircleCenterX - startingCircleCenterX;
				float dy = endingCircleCenterY - startingCircleCenterY;
				float dr = endingCircleRadius - startingCircleRadius;
				double maxDistanceX = Math.Max(Math.Abs((endingCircleCenterX - endingCircleRadius) - (startingCircleCenterX - startingCircleRadius)),
					Math.Abs((endingCircleCenterX + endingCircleRadius) - (startingCircleCenterX + startingCircleRadius)));
				double maxDistanceY = Math.Max(Math.Abs((endingCircleCenterY - endingCircleRadius) - (startingCircleCenterY - startingCircleRadius)), 
					Math.Abs((endingCircleCenterY + endingCircleRadius) - (startingCircleCenterY + startingCircleRadius)));
				int gradientAxisLength = Convert.ToInt32(Math.Sqrt((maxDistanceX * maxDistanceX + maxDistanceY * maxDistanceY)) * 1.5);
				PdfRange domain = shading.Domain;
				double domainMin = domain.Min;
				double domainMax = domain.Max;
				double domainSize = domainMax - domainMin;
				float maxGradientAxisPosition = gradientAxisLength - 1;
				for (int i = 0; i < gradientAxisLength; i++) 
					using (Pen pen = new Pen(colorConverter.Convert(domainMin + i * domainSize / maxGradientAxisPosition))) { 
						float factor =  i / maxGradientAxisPosition;
						float radius = startingCircleRadius + factor * dr;
						float diameter = radius * 2;
						graphics.DrawEllipse(pen, startingCircleCenterX + factor * dx - radius, startingCircleCenterY + factor * dy - radius, diameter, diameter);
					}
				if (shading.ExtendX && startingCircleRadius != 0)  
					using (SolidBrush startingOuterSpaceBrush = new SolidBrush(colorConverter.Convert(domainMin))) 
						using (GraphicsPath graphicsPath = CreateStartingOuterSpaceGraphicsPath())	
							PaintGraphicsPath(graphics, graphicsPath, startingOuterSpaceBrush, startingCircleCenterX, startingCircleCenterY);
				if (shading.ExtendY && endingCircleRadius != 0)  
					using (SolidBrush endingOuterSpaceBrush = new SolidBrush(colorConverter.Convert(domainMax)))
						using (GraphicsPath graphicsPath = CreateEndingOuterSpaceGraphicsPath())
							PaintGraphicsPath(graphics, graphicsPath, endingOuterSpaceBrush, endingCircleCenterX, endingCircleCenterY);
			}
		}
	}
}
