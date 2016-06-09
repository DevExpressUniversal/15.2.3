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
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Utils {
	#region Units
	public static class Units {
		static int MulDiv(int value, int mul, int div) {
			return mul * value / div;
		}
		static int MulDiv(int value, int mul, float div) {
			return (int)(float)((mul * value) / div);
		}
		static long MulDivL(long value, long mul, long div) {
			return mul * value / div;
		}
		static long MulDivL(long value, long mul, float div) {
			return (long)(float)((mul * value) / div);
		}
		static float MulDivF(float value, float mul, float div) {
			return (float)((float)(mul * value) / div);
		}
		#region DocumentsTo(*)
		public static int DocumentsToTwips(int val) {
			return MulDiv(val, 24, 5);
		}
		public static int DocumentsToEmu(int val) {
			return MulDiv(val, 9144, 3);
		}
		public static long DocumentsToEmuL(long val) {
			return MulDivL(val, 9144, 3);
		}
		public static int DocumentsToEmuF(float val) {
			return (int)MulDivF(val, 9144, 3);
		}
		public static long DocumentsToTwipsL(long val) {
			return MulDivL(val, 24, 5);
		}
		public static float DocumentsToTwipsF(float val) {
			return MulDivF(val, 24.0f, 5.0f);
		}
		public static Size DocumentsToTwips(Size val) {
			return new Size(DocumentsToTwips(val.Width), DocumentsToTwips(val.Height));
		}
		public static Rectangle DocumentsToTwips(Rectangle val) {
			return Rectangle.FromLTRB(DocumentsToTwips(val.Left), DocumentsToTwips(val.Top), DocumentsToTwips(val.Right), DocumentsToTwips(val.Bottom));
		}
		public static RectangleF DocumentsToTwips(RectangleF val) {
			return RectangleF.FromLTRB(DocumentsToTwipsF(val.Left), DocumentsToTwipsF(val.Top), DocumentsToTwipsF(val.Right), DocumentsToTwipsF(val.Bottom));
		}
		public static int DocumentsToPixels(int val, float dpi) {
			if (val >= 0)
				return (int)(dpi * val / 300 + .99);
			return (int)(dpi * val / 300 - .99);
		}
		public static Point DocumentsToPixels(Point val, float dpiX, float dpiY) {
			return new Point(DocumentsToPixels(val.X, dpiX), DocumentsToPixels(val.Y, dpiY));
		}
		public static Size DocumentsToPixels(Size val, float dpiX, float dpiY) {
			return new Size(DocumentsToPixels(val.Width, dpiX), DocumentsToPixels(val.Height, dpiY));
		}
		public static Rectangle DocumentsToPixels(Rectangle val, float dpiX, float dpiY) {
			return new Rectangle(DocumentsToPixels(val.X, dpiX), DocumentsToPixels(val.Y, dpiY), DocumentsToPixels(val.Width, dpiX), DocumentsToPixels(val.Height, dpiY));
		}
		public static RectangleF DocumentsToPixels(RectangleF val, float dpiX, float dpiY) {
			return new RectangleF(DocumentsToPixelsF(val.X, dpiX), DocumentsToPixelsF(val.Y, dpiY), DocumentsToPixelsF(val.Width, dpiX), DocumentsToPixelsF(val.Height, dpiY));
		}
		public static float DocumentsToPixelsF(float val, float dpi) {
			return MulDivF(val, dpi, 300.0f);
		}
		public static int DocumentsToPoints(int val) {
			return MulDiv(val, 6, 25);
		}
		public static float DocumentsToPointsF(float val) {
			return MulDivF(val, 6.0f, 25.0f);
		}
		public static float DocumentsToPointsFRound(float val) {
			return (float)Math.Round(DocumentsToPointsF(val));
		}
		public static int DocumentsToHundredthsOfMillimeter(int val) {
			return MulDiv(val, 127, 15);
		}
		public static Size DocumentsToHundredthsOfMillimeter(Size val) {
			return new Size(DocumentsToHundredthsOfMillimeter(val.Width), DocumentsToHundredthsOfMillimeter(val.Height));
		}
		public static int DocumentsToHundredthsOfInch(int val) {
			return val / 3;
		}
		public static Size DocumentsToHundredthsOfInch(Size val) {
			return new Size(DocumentsToHundredthsOfInch(val.Width), DocumentsToHundredthsOfInch(val.Height));
		}
		public static float DocumentsToCentimetersF(float value) {
			return MulDivF(value, 127.0f, 15000.0f);
		}
		public static float DocumentsToInchesF(float value) {
			return (float)(value / 300.0f);
		}
		public static float DocumentsToMillimetersF(float value) {
			return MulDivF(value, 127.0f, 1500.0f);
		}
		public static float DocumentsToPicasF(float value) {
			return (float)(value / 50.0f);
		}
		#endregion
		#region TwipsTo(*)
		public static int TwipsToDocuments(int val) {
			return MulDiv(val, 5, 24);
		}
		public static Size TwipsToDocuments(Size val) {
			return new Size(TwipsToDocuments(val.Width), TwipsToDocuments(val.Height));
		}
		public static int TwipsToEmu(int val) {
			return val * 635;
		}
		public static long TwipsToEmuL(long val) {
			return val * 635L;
		}
		public static int TwipsToEmuF(float val) {
			return (int)(val * 635);
		}
		public static long TwipsToDocumentsL(long val) {
			return MulDivL(val, 5, 24);
		}
		public static Rectangle TwipsToDocuments(Rectangle val) {
			return Rectangle.FromLTRB(TwipsToDocuments(val.Left), TwipsToDocuments(val.Top), TwipsToDocuments(val.Right), TwipsToDocuments(val.Bottom));
		}
		public static RectangleF TwipsToDocuments(RectangleF val) {
			return RectangleF.FromLTRB(TwipsToDocumentsF(val.Left), TwipsToDocumentsF(val.Top), TwipsToDocumentsF(val.Right), TwipsToDocumentsF(val.Bottom));
		}
		public static int TwipsToPixels(int val, float dpi) {
			return (int)(dpi * val / 1440 + .99);
		}
		public static long TwipsToPixelsL(long val, float dpi) {
			return (long)(dpi * val / 1440 + .99);
		}
		public static Point TwipsToPixels(Point val, float dpiX, float dpiY) {
			return new Point(TwipsToPixels(val.X, dpiX), TwipsToPixels(val.Y, dpiY));
		}
		public static Size TwipsToPixels(Size val, float dpiX, float dpiY) {
			return new Size(TwipsToPixels(val.Width, dpiX), TwipsToPixels(val.Height, dpiY));
		}
		public static Rectangle TwipsToPixels(Rectangle val, float dpiX, float dpiY) {
			return new Rectangle(TwipsToPixels(val.X, dpiX), TwipsToPixels(val.Y, dpiY), TwipsToPixels(val.Width, dpiX), TwipsToPixels(val.Height, dpiY));
		}
		public static float TwipsToPixelsF(float val, float dpi) {
			return MulDivF(val, dpi, 1440.0f);
		}
		public static Size TwipsToHundredthsOfMillimeter(Size val) {
			return new Size(MulDiv(val.Width, 127, 72), MulDiv(val.Height, 127, 72));
		}
		public static float TwipsToPointsF(float val) {
			return (float)(val / 20.0f);
		}
		public static float TwipsToPointsFRound(float val) {
			return (float)Math.Round(TwipsToPointsF(val));
		}
		public static float TwipsToCentimetersF(float value) {
			return MulDivF(value, 2.54f, 1440.0f);
		}
		public static float TwipsToInchesF(float value) {
			return (float)(value / 1440.0f);
		}
		public static float TwipsToMillimetersF(float value) {
			return MulDivF(value, 25.4f, 1440.0f);
		}
		public static float TwipsToDocumentsF(float value) {
			return MulDivF(value, 5.0f, 24.0f);
		}
		public static int TwipsToHundredthsOfInch(int val) {
			return MulDiv(val, 5, 72);
		}
		public static Size TwipsToHundredthsOfInch(Size val) {
			return new Size(TwipsToHundredthsOfInch(val.Width), TwipsToHundredthsOfInch(val.Height));
		}
		#endregion
		#region PixelsTo(*)
		public static int PixelsToPoints(int val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToPointsCore(val, dpi);
		}
		public static float PixelsToPointsF(float val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToPointsCore(val, dpi);
		}
		internal static int PixelsToPointsCore(int val, float dpi) {
			return MulDiv(val, 72, dpi);
		}
		internal static float PixelsToPointsCore(float val, float dpi) {
			return MulDivF(val, 72, dpi);
		}
		public static int PixelsToDocuments(int val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToDocumentsCore(val, dpi);
		}
		public static float PixelsToDocumentsF(float val, float dpi) {
			if (dpi == 0)
				return 0;
			return MulDivF(val, 300, dpi);
		}
		public static int PixelsToTwips(int val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToTwipsCore(val, dpi);
		}
		public static long PixelsToTwipsL(long val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToTwipsCore(val, dpi);
		}
		public static float PixelsToTwipsF(float val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToTwipsCore(val, dpi);
		}
		internal static int PixelsToDocumentsCore(int val, float dpi) {
			return MulDiv(val, 300, dpi);
		}
		internal static int PixelsToDocumentsCoreRound(int val, float dpi) {
			return (int)Math.Round(MulDivF(val, 300.0f, dpi));
		}
		internal static int PixelsToTwipsCore(int val, float dpi) {
			return MulDiv(val, 1440, dpi);
		}
		internal static int PixelsToTwipsCoreRound(int val, float dpi) {
			return (int)Math.Round(MulDivF(val, 1440, dpi));
		}
		internal static long PixelsToTwipsCore(long val, float dpi) {
			return MulDivL(val, 1440, dpi);
		}
		internal static float PixelsToTwipsCore(float val, float dpi) {
			return MulDivF(val, 1440.0f, dpi);
		}
		internal static int PixelsToDocumentsCore(double val, float dpi) {
			return (int)Math.Round((double)((double)(300 * val) / dpi));
		}
		static float PixelsToDocumentsCoreF(float val, float dpi) {
			return MulDivF(val, 300.0f, dpi);
		}
		public static int PixelsToDocuments(double val, float dpi) {
			if (dpi == 0)
				return 0;
			return PixelsToDocumentsCore(val, dpi);
		}
		public static Rectangle PixelsToDocuments(Rectangle rect, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 300;
			if (dpiY == 0)
				dpiY = 300;
			return new Rectangle(PixelsToDocumentsCore(rect.X, dpiX), PixelsToDocumentsCore(rect.Y, dpiY), PixelsToDocumentsCore(rect.Width, dpiX), PixelsToDocumentsCore(rect.Height, dpiY));
		}
		public static RectangleF PixelsToDocuments(RectangleF rect, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 300;
			if (dpiY == 0)
				dpiY = 300;
			return new RectangleF(PixelsToDocumentsCoreF(rect.X, dpiX), PixelsToDocumentsCoreF(rect.Y, dpiY), PixelsToDocumentsCoreF(rect.Width, dpiX), PixelsToDocumentsCoreF(rect.Height, dpiY));
		}
		public static Size PixelsToDocuments(Size size, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 300;
			if (dpiY == 0)
				dpiY = 300;
			return new Size(PixelsToDocumentsCore(size.Width, dpiX), PixelsToDocumentsCore(size.Height, dpiY));
		}
		public static Size PixelsToDocumentsRound(Size size, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 300;
			if (dpiY == 0)
				dpiY = 300;
			return new Size(PixelsToDocumentsCoreRound(size.Width, dpiX), PixelsToDocumentsCoreRound(size.Height, dpiY));
		}
		public static Size PixelsToTwips(Size size, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 1440;
			if (dpiY == 0)
				dpiY = 1440;
			return new Size(PixelsToTwipsCore(size.Width, dpiX), PixelsToTwipsCore(size.Height, dpiY));
		}
		public static Size PixelsToTwipsRound(Size size, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 1440;
			if (dpiY == 0)
				dpiY = 1440;
			return new Size(PixelsToTwipsCoreRound(size.Width, dpiX), PixelsToTwipsCoreRound(size.Height, dpiY));
		}
		public static Rectangle PixelsToTwips(Rectangle rect, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 1440;
			if (dpiY == 0)
				dpiY = 1440;
			return new Rectangle(PixelsToTwipsCore(rect.X, dpiX), PixelsToTwipsCore(rect.Y, dpiY), PixelsToTwipsCore(rect.Width, dpiX), PixelsToTwipsCore(rect.Height, dpiY));
		}
		public static Point PixelsToDocuments(Point point, float dpiX, float dpiY) {
			if (dpiX == 0)
				dpiX = 300;
			if (dpiY == 0)
				dpiY = 300;
			return new Point(PixelsToDocumentsCore(point.X, dpiX), PixelsToDocumentsCore(point.Y, dpiY));
		}
		public static int PixelsToHundredthsOfMillimeter(int val, float dpi) {
			return (int)Math.Round(2540 * val / dpi);
		}
		public static Size PixelsToHundredthsOfMillimeter(Size val, float dpiX, float dpiY) {
			return new Size(PixelsToHundredthsOfMillimeter(val.Width, dpiX), PixelsToHundredthsOfMillimeter(val.Height, dpiY));
		}
		public static int PixelsToHundredthsOfInch(int val, float dpi) {
			return MulDiv(val, 100, dpi);
		}
		public static Size PixelsToHundredthsOfInch(Size val, float dpi) {
			return new Size(PixelsToHundredthsOfInch(val.Width, dpi), PixelsToHundredthsOfInch(val.Height, dpi));
		}
		#endregion
		#region HundredthsOfMillimeterTo(*)
		public static int HundredthsOfMillimeterToDocuments(int val) {
			return MulDiv(val, 15, 127);
		}
		public static Size HundredthsOfMillimeterToDocuments(Size val) {
			return new Size(HundredthsOfMillimeterToDocuments(val.Width), HundredthsOfMillimeterToDocuments(val.Height));
		}
		static int HundredthsOfMillimeterToDocumentsRound(int val) {
			return (int)Math.Round(MulDivF(val, 15.0f, 127.0f));
		}
		public static Size HundredthsOfMillimeterToDocumentsRound(Size val) {
			return new Size(HundredthsOfMillimeterToDocumentsRound(val.Width), HundredthsOfMillimeterToDocumentsRound(val.Height));
		}
		public static int HundredthsOfMillimeterToTwips(int val) {
			return MulDiv(val, 72, 127);
		}
		public static Size HundredthsOfMillimeterToTwips(Size val) {
			return new Size(HundredthsOfMillimeterToTwips(val.Width), HundredthsOfMillimeterToTwips(val.Height));
		}
		static int HundredthsOfMillimeterToTwipsRound(int val) {
			return (int)Math.Round(MulDivF(val, 72.0f, 127.0f));
		}
		public static Size HundredthsOfMillimeterToTwipsRound(Size val) {
			return new Size(HundredthsOfMillimeterToTwipsRound(val.Width), HundredthsOfMillimeterToTwipsRound(val.Height));
		}
		public static int HundredthsOfMillimeterToPixels(int val, float dpi) {
			return (int)Math.Round(MulDivF(val, dpi, 2540.0f));
		}
		public static Size HundredthsOfMillimeterToPixels(Size val, float dpiX, float dpiY) {
			return new Size(HundredthsOfMillimeterToPixels(val.Width, dpiX), HundredthsOfMillimeterToPixels(val.Height, dpiY));
		}
		#endregion
		#region PointsTo(*)
		public static int PointsToDocuments(int val) {
			return MulDiv(val, 25, 6);
		}
		public static float PointsToDocumentsF(float value) {
			return MulDivF(value, 25.0f, 6.0f);
		}
		public static float PointsToTwipsF(float value) {
			return (float)(value * 20);
		}
		public static int PointsToTwips(int value) {
			return value * 20;
		}
		public static int PointsToPixels(int val, float dpi) {
			if (val >= 0)
				return (int)(dpi * val / 72 + .99);
			return (int)(dpi * val / 72 - .99);
		}
		public static float PointsToPixelsF(float val, float dpi) {
			return MulDivF(val, dpi, 72.0f);
		}
		#endregion
		#region EmuTo(*)
		public static int EmuToDocuments(int val) {
			return MulDiv(val, 3, 9144);
		}
		public static long EmuToDocumentsL(long val) {
			return MulDivL(val, 3, 9144);
		}
		public static float EmuToDocumentsF(int val) {
			return MulDivF(val, 3, 9144);
		}
		public static int EmuToTwips(int val) {
			return val / 635;
		}
		public static long EmuToTwipsL(long val) {
			return val / 635L;
		}
		public static float EmuToTwipsF(int val) {
			return val / 635f;
		}
		#endregion
		public static int HundredthsOfInchToDocuments(int val) {
			return val * 3;
		}
		public static Size HundredthsOfInchToDocuments(Size val) {
			return new Size(HundredthsOfInchToDocuments(val.Width), HundredthsOfInchToDocuments(val.Height));
		}
		public static int HundredthsOfInchToTwips(int val) {
			return MulDiv(val, 72, 5);
		}
		public static Size HundredthsOfInchToTwips(Size val) {
			return new Size(HundredthsOfInchToTwips(val.Width), HundredthsOfInchToTwips(val.Height));
		}
		public static float CentimetersToDocumentsF(float value) {
			return MulDivF(value, 15000.0f, 127.0f);
		}
		public static float CentimetersToTwipsF(float value) {
			return MulDivF(value, 1440.0f, 2.54f);
		}
		public static float InchesToDocumentsF(float value) {
			return (float)(300.0f * value);
		}
		public static float InchesToTwipsF(float value) {
			return (float)(1440.0f * value);
		}
		public static float InchesToPointsF(float value) {
			return (float)(72.0f * value);
		}
		public static float MillimetersToDocumentsF(float value) {
			return MulDivF(value, 1500.0f, 127.0f);
		}
		public static float MillimetersToTwipsF(float value) {
			return MulDivF(value, 1440.0f, 25.4f);
		}
		public static float PicasToDocumentsF(float value) {
			return (float)(50.0f * value);
		}
		public static float PicasToTwipsF(float value) {
			return (float)(240.0f * value);
		}
		public static int MillimetersToPoints(int value) {
			return MulDiv(value, 360, 127);
		}
		public static float MillimetersToPointsF(float value) {
			return MulDivF(value, 72.0f, 25.4f);
		}
	}
	#endregion
}
