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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfAxialShadingPainter : PdfShadingPainter { 
		struct Range {
			readonly float min;
			readonly float max;
			public float Min{ get{ return min; } }
			public float Max{ get{ return max; } }
			public Range(float min, float max) {
				this.min = min;
				this.max = max;
			}
		}
		const int brushColorComponents = 3;
		const double halfPI = Math.PI / 2.0;
		static Range CalculateRange(float value1, float value2, float value3, float value4) {
			float minValue;
			float maxValue;
			if (value1 > value2) { 
				maxValue = value1;
				minValue = value2;
			}
			else {
				maxValue = value2;
				minValue = value1;
			} 
			if (value3 > maxValue) 
				maxValue = value3;
			else if (value3 < minValue)
				minValue = value3;
			if (value4 > maxValue) 
				maxValue = value4;
			else if (value4 < minValue)
				minValue = value4;
			return new Range(minValue, maxValue);
		}
		readonly PdfAxialShading shading;
		readonly float axisStartX;
		readonly float axisStartY;
		readonly float axisEndX;
		readonly float axisEndY;
		readonly double axisAngle;
		readonly float locationX;
		readonly float locationY;
		readonly int brushBitmapWidth;
		readonly float gradientBrushPenWidth;
		readonly int rotatedBitmapWidth;
		readonly int rotatedBitmapHeight;
		public PdfAxialShadingPainter(PdfAxialShading shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
				: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) { 
			this.shading = shading;
			IShadingCoordsConverter coordsConverter = CoordsConverter;
			PointF axisStart = coordsConverter.Convert(shading.AxisStart);
			PointF axisEnd = coordsConverter.Convert(shading.AxisEnd);
			axisStartX = axisStart.X;
			axisStartY = axisStart.Y;
			axisEndX = axisEnd.X;
			axisEndY = axisEnd.Y;
			float dx = axisEndX - axisStartX;
			float dy = axisEndY - axisStartY;
			axisAngle = Math.Atan2(dy, dx);
			locationX = axisStartX + dx / 2;
			locationY = axisStartY + dy / 2;
			Range rangeX = CalculateRange(0, axisStartX, axisEndX, bitmapWidth); 
			Range rangeY = CalculateRange(0, axisStartY, axisEndY, bitmapHeight); 
			float shadingWidth = rangeX.Max - rangeX.Min;
			float shadingHeight = rangeY.Max - rangeY.Min; 
			float a;
			float b;
			float c; 
			if (Math.Abs(dx) >= Math.Abs(dy)) {
				a = dx == 0 ? 0 : dy / dx;
				b = - 1;  
				c = axisStartY - a * axisStartX;
			}
			else {
				a = - 1;
				b = dy == 0 ? 0 : dx / dy;  
				c = axisStartX - b * axisStartY;
			}
			Range distanceRange = CalculateRange(Math.Abs(c), Math.Abs(a * bitmapWidth + c), Math.Abs(b * bitmapHeight + c), Math.Abs(a * bitmapWidth + b * bitmapHeight + c));
			gradientBrushPenWidth = (float)(distanceRange.Max / Math.Sqrt(a * a + b * b)) * 2;
			double adjacentTop;
			double oppositeTop;
			double adjacentBottom;
			double oppositeBottom;
			if ((axisAngle >= 0.0 && axisAngle < halfPI) || (axisAngle >= Math.PI && axisAngle < (Math.PI + halfPI))) {
				adjacentTop = Math.Cos(axisAngle) * shadingWidth;
				oppositeTop = Math.Sin(axisAngle) * shadingWidth;
				adjacentBottom = Math.Cos(axisAngle) * shadingHeight;
				oppositeBottom = Math.Sin(axisAngle) * shadingHeight;
			}
			else {
				adjacentTop = Math.Sin(axisAngle) * shadingHeight;
				oppositeTop = Math.Cos(axisAngle) * shadingHeight;
				adjacentBottom = Math.Sin(axisAngle) * shadingWidth;
				oppositeBottom = Math.Cos(axisAngle) * shadingWidth;
			}			
			rotatedBitmapWidth = Convert.ToInt32(Math.Ceiling(Math.Abs(adjacentTop) + Math.Abs(oppositeBottom)));
			rotatedBitmapHeight = Convert.ToInt32(Math.Ceiling(Math.Abs(adjacentBottom) + Math.Abs(oppositeTop)));
			brushBitmapWidth = Convert.ToInt32(Math.Sqrt(dx * dx + dy * dy)) + 1;
			if (brushBitmapWidth == 1)
				brushBitmapWidth++;
			if (dy == 0 && dx != 0) 
				if (axisEndX > axisStartX)
					axisEndX++;
				else
					axisStartX++;
				if (dx == 0 && dy != 0)
				if (axisEndY > axisStartY)
					axisEndY++;
				else
					axisStartY++;
		}
		[SecuritySafeCritical]
		protected override void Paint(Graphics graphics) { 
			base.Paint(graphics);
			IShadingColorConverter colorConverter = ColorConverter;
			PdfRange domain = shading.Domain;
			double domainMin = domain.Min;
			double domainMax = domain.Max;
			if (shading.ExtendX || shading.ExtendY) { 
				GraphicsState graphicsState = graphics.Save();
				try {  
					graphics.TranslateTransform(locationX, locationY);
					graphics.RotateTransform((float)(axisAngle * RadianToDegreeFactor));  
					int filledRectangleMaxHeight = rotatedBitmapHeight * 2;
					if (shading.ExtendX)  
						using (SolidBrush leftOuterSpaceBrush = new SolidBrush(colorConverter.Convert(domainMin)))
							graphics.FillRectangle(leftOuterSpaceBrush, new Rectangle(-rotatedBitmapWidth, -rotatedBitmapHeight, rotatedBitmapWidth, filledRectangleMaxHeight));
					if (shading.ExtendY) 
						using (SolidBrush rightOuterSpaceBrush = new SolidBrush(colorConverter.Convert(domainMax)))
							graphics.FillRectangle(rightOuterSpaceBrush, new Rectangle(0, -rotatedBitmapHeight, rotatedBitmapWidth, filledRectangleMaxHeight));	
				}
				finally { 
					graphics.Restore(graphicsState);
				}
			}  
			byte[] brushBitmapData = new byte[brushColorComponents * brushBitmapWidth];
			double domainSize = domainMax - domainMin;
			int maxBrushPixelPosition = brushBitmapWidth - 1;
			for (int i = 0, j = 0; i < brushBitmapWidth; i++) { 
				Color color = colorConverter.Convert(domainMin + i * domainSize / maxBrushPixelPosition);
				brushBitmapData[j++] = color.B;
				brushBitmapData[j++] = color.G;
				brushBitmapData[j++] = color.R;
			}
			GCHandle dataHandle = GCHandle.Alloc(brushBitmapData, GCHandleType.Pinned);
			try { 
				int stride = brushColorComponents * brushBitmapWidth;
				if (stride % 4 > 0) 
					stride += 4 - (stride % 4);
				using (Bitmap brushBitmap = new Bitmap(brushBitmapWidth, 1, stride, PixelFormat.Format24bppRgb, dataHandle.AddrOfPinnedObject())) 
					using (TextureBrush gradientBrush = new TextureBrush(brushBitmap)) { 
						gradientBrush.TranslateTransform(axisStartX, axisStartY);
						gradientBrush.RotateTransform((float)(axisAngle * RadianToDegreeFactor));
						if (axisAngle > halfPI || axisAngle < 0.0)
							gradientBrush.TranslateTransform(1, 0);	
						using (Pen gradientBrushPen = new Pen(gradientBrush, gradientBrushPenWidth))
							graphics.DrawLine(gradientBrushPen, axisStartX, axisStartY, axisEndX, axisEndY);	  
					}
			}
			finally {
				if (dataHandle.IsAllocated)
					dataHandle.Free();
			};		
		}	
	}
}
