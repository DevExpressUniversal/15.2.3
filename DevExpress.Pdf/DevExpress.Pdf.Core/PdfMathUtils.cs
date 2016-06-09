#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public static class PdfMathUtils {
		const double doublePi = Math.PI * 2;
		static double CalcDistanceToSegment(PdfPoint point, double startX, double startY, double endX, double endY) {
			double dx = endX - startX;
			double dy = endY - startY;
			double c1 = dx * (point.X - startX) + dy * (point.Y - startY);
			if (c1 <= 0)
				return CalcDistance(point, new PdfPoint(startX, startY));
			double c2 = dx * dx + dy * dy;
			if (c2 <= c1)
				return CalcDistance(point, new PdfPoint(endX, endY));
			double b = c1 / c2;
			return CalcDistance(point, new PdfPoint(startX + dx * b, startY + dy * b));
		}
		public static double CalcDistance(PdfPoint start, PdfPoint end) {
			double dx = end.X - start.X;
			double dy = end.Y - start.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public static double CalcDistanceToRectangle(PdfPoint point, PdfRectangle rect) {
			double left = rect.Left;
			double bottom = rect.Bottom;
			double right = rect.Right;
			double top = rect.Top;
			return Math.Min(Math.Min(CalcDistanceToSegment(point, left, top, right, top), CalcDistanceToSegment(point, right, top, right, bottom)),
							Math.Min(CalcDistanceToSegment(point, right, bottom, left, bottom), CalcDistanceToSegment(point, left, bottom, left, top)));
		}
		public static byte ToByte(double value) {
			return (byte)(value + 0.5);
		}
		public static int ToInt32(double value) {
			return (int)(value + 0.5);
		}
		public static double Min(double value1, double value2) {
			return value1 < value2 ? value1 : value2;
		}
		public static double Max(double value1, double value2) {
			return value1 > value2 ? value1 : value2;
		}
		public static double NormalizeAngle(double angle) {
			while (angle < 0)
				angle += doublePi;
			while (angle >= doublePi)
				angle -= doublePi;
			return angle;
		}
	}
}
