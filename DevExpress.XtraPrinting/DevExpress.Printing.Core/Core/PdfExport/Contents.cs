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
using DevExpress.Utils;
using DevExpress.Data.Helpers;
using System.Drawing.Drawing2D;
using System.IO;
using DevExpress.Pdf.Common;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfHashtable {
		Dictionary<Color, string> strokeColors = new Dictionary<Color, string>();
		Dictionary<Color, string> fillColors = new Dictionary<Color, string>();
		Dictionary<RectangleF, string> rectangles = new Dictionary<RectangleF, string>();
		Dictionary<SizeF, string> sizes = new Dictionary<SizeF, string>();
		public void Clear() {
			strokeColors.Clear();
			fillColors.Clear();
			rectangles.Clear();
			sizes.Clear();
		}
		public string GetRGBStrokeColor(Color color) {
			string res;
			if(!strokeColors.TryGetValue(color, out res)) {
				res = ColorToString(color) + " RG";
				strokeColors.Add(color, res);
			}
			return res;
		}
		public string GetRGBFillColor(Color color) {
			string res;
			if(!fillColors.TryGetValue(color, out res)) {
				res = ColorToString(color) + " rg";
				fillColors.Add(color, res);
			}
			return res;
		}
		string ColorToString(Color color) {
			Color blended = DXColor.Blend(color, DXColor.White);
			return
				Utils.ToString(Math.Round((float)blended.R / 255, 3)) + " " +
				Utils.ToString(Math.Round((float)blended.G / 255, 3)) + " " +
				Utils.ToString(Math.Round((float)blended.B / 255, 3));
		}
		public string GetRectangle(float x, float y, float width, float height) {
			RectangleF rect = new RectangleF(x, y, width, height);
			string res;
			if(!rectangles.TryGetValue(rect, out res)) {
				res = Utils.ToString(x) + " " +
				Utils.ToString(y) + " " +
				Utils.ToString(width) + " " +
				Utils.ToString(height) + " re";
				rectangles.Add(rect, res);
			}
			return res;
		}
		public string GetSize(float tx, float ty) {
			SizeF size = new SizeF(tx, ty);
			string res;
			if(!sizes.TryGetValue(size, out res)) {
				res = Utils.ToString(tx) + " " + Utils.ToString(ty) + " Td";
				sizes.Add(size, res);
			}
			return res;
		}
	}
	public class PdfContents : PdfDocumentStreamObject {
		PdfDrawContext context;
		public PdfDrawContext DrawContext { get { return context; } }
		public PdfContents(IPdfContentsOwner owner, bool compressed, PdfHashtable pdfHashtable)
			: base(compressed) {
			context = PdfDrawContext.Create(this.Stream, owner, pdfHashtable);
		}
	}
	public class PdfDrawContext : PdfMeasuringContext {
		IPdfContentsOwner owner;
		PdfHashtable pdfHashtable;
		PdfStream stream;
		PdfStream Stream { get { return stream; } }
		public PdfDrawContext(PdfStream stream, IPdfContentsOwner owner, PdfHashtable pdfHashtable) {
			this.stream = stream;
			this.owner = owner;
			this.pdfHashtable = pdfHashtable;
		}
		public static PdfDrawContext Create(PdfStream stream, IPdfContentsOwner owner, PdfHashtable pdfHashtable) {
			return SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd && !PdfGraphics.EnableAzureCompatibility
			  ? new PdfDrawContext(stream, owner, pdfHashtable)
			 : new PartialTrustPdfDrawContext(stream, owner, pdfHashtable);
		}
		#region Special Graphics States
		public void GSave() {
			Stream.SetStringLine("q");
		}
		public void GRestore() {
			Stream.SetStringLine("Q");
		}
		public void Concat(Matrix matrix) {
			float[] elements = matrix.Elements;
			Concat(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
		}
		public void Concat(float a, float b, float c, float d, float e, float f) {
			Stream.SetStringLine(
				Utils.ToString(a) + " " +
				Utils.ToString(b) + " " +
				Utils.ToString(c) + " " +
				Utils.ToString(d) + " " +
				Utils.ToString(e) + " " +
				Utils.ToString(f) + " cm");
		}
		#endregion
		#region General Graphics States
		public void SetFlat(int flatness) {
			if(flatness < 0)
				flatness = 0;
			if(flatness > 100)
				flatness = 100;
			Stream.SetStringLine(Convert.ToString(flatness) + " i");
		}
		public void SetLineCap(PdfLineCapStyle lineCap) {
			Stream.SetStringLine(Convert.ToString(Convert.ToInt16(lineCap)) + " J");
		}
		public void SetDash(float[] array, int phase) {
			Stream.SetString("[");
			for(int i = 0; i < array.Length; i++) {
				Stream.SetString(Utils.ToString(array[i]));
				if(i < array.Length - 1)
					Stream.SetString(" ");
			}
			Stream.SetStringLine("] " + Convert.ToString(phase) + " d");
		}
		public void SetLineJoin(PdfLineJoinStyle lineJoin) {
			Stream.SetStringLine(Convert.ToString(Convert.ToInt16(lineJoin)) + " j");
		}
		public void SetLineWidth(float lineWidth) {
			Stream.SetStringLine(Utils.ToString(lineWidth) + " w");
		}
		public void SetMiterLimit(int miterLimit) {
			Stream.SetStringLine(Convert.ToString(miterLimit) + " M");
		}
		#endregion
		#region Device-dependent Color States
		public void SetRGBFillColor(Color color) {
			Stream.SetStringLine(pdfHashtable.GetRGBFillColor(color));
		}
		public void SetRGBStrokeColor(Color color) {
			Stream.SetStringLine(pdfHashtable.GetRGBStrokeColor(color));
		}
		#endregion
		#region Path States
		public void MoveTo(float x, float y) {
			Stream.SetStringLine(Utils.ToString(x) + " " + Utils.ToString(y) + " m");
		}
		public void LineTo(float x, float y) {
			Stream.SetStringLine(Utils.ToString(x) + " " + Utils.ToString(y) + " l");
		}
		public void CurveTo(float x1, float y1, float x2, float y2, float x3, float y3) {
			Stream.SetStringLine(
				Utils.ToString(x1) + " " +
				Utils.ToString(y1) + " " +
				Utils.ToString(x2) + " " +
				Utils.ToString(y2) + " " +
				Utils.ToString(x3) + " " +
				Utils.ToString(y3) + " c");
		}
		public void CurveToV(float x2, float y2, float x3, float y3) {
			Stream.SetStringLine(
				Utils.ToString(x2) + " " +
				Utils.ToString(y2) + " " +
				Utils.ToString(x3) + " " +
				Utils.ToString(y3) + " v");
		}
		public void CurveToY(float x1, float y1, float x3, float y3) {
			Stream.SetStringLine(
				Utils.ToString(x1) + " " +
				Utils.ToString(y1) + " " +
				Utils.ToString(x3) + " " +
				Utils.ToString(y3) + " y");
		}
		public void Arc(PointF point1, PointF point2, int startAngle, int endAngle) {
			List<float[]> array = CreateBezierArc(point1.X, point1.Y, point2.X, point2.Y, startAngle, endAngle);
			if(array.Count == 0)
				return;
			float[] pt = array[0];
			MoveTo(pt[0], pt[1]);
			for(int k = 0; k < array.Count; ++k) {
				pt = array[k];
				CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
			}
		}
		List<float[]> CreateBezierArc(float x1, float y1, float x2, float y2, float startAngle, float endAngle) {
			float tmp;
			if(x1 > x2) {
				tmp = x1;
				x1 = x2;
				x2 = tmp;
			}
			if(y1 > y2) {
				tmp = y1;
				y1 = y2;
				y2 = tmp;
			}
			float sectorAngle;
			int sectorCount;
			if(Math.Abs(endAngle) <= 90f) {
				sectorAngle = endAngle;
				sectorCount = 1;
			} else {
				sectorCount = (int)(Math.Ceiling(Math.Abs(endAngle) / 90f));
				sectorAngle = endAngle / sectorCount;
			}
			float centerX = (x1 + x2) / 2f;
			float centerY = (y1 + y2) / 2f;
			float halfWidth = (x2 - x1) / 2f;
			float halfHeight = (y2 - y1) / 2f;
			float halfAngle = (float)(sectorAngle * Math.PI / 360.0);
			float kappa = (float)(Math.Abs(4.0 / 3.0 * (1.0 - Math.Cos(halfAngle)) / Math.Sin(halfAngle)));
			List<float[]> pointList = new List<float[]>();
			for(int i = 0; i < sectorCount; ++i) {
				float theta0 = (float)((startAngle + i * sectorAngle) * Math.PI / 180.0);
				float theta1 = (float)((startAngle + (i + 1) * sectorAngle) * Math.PI / 180.0);
				float cos0 = (float)Math.Cos(theta0);
				float cos1 = (float)Math.Cos(theta1);
				float sin0 = (float)Math.Sin(theta0);
				float sin1 = (float)Math.Sin(theta1);
				if(sectorAngle > 0f) {
					pointList.Add(new float[]{   centerX + halfWidth * cos0,
												 centerY - halfHeight * sin0,
												 centerX + halfWidth * (cos0 - kappa * sin0),
												 centerY - halfHeight * (sin0 + kappa * cos0),
												 centerX + halfWidth * (cos1 + kappa * sin1),
												 centerY - halfHeight * (sin1 - kappa * cos1),
												 centerX + halfWidth * cos1,
												 centerY - halfHeight * sin1});
				} else {
					pointList.Add(new float[]{  centerX + halfWidth * cos0,
												centerY - halfHeight * sin0,
												centerX + halfWidth * (cos0 + kappa * sin0),
												centerY - halfHeight * (sin0 - kappa * cos0),
												centerX + halfWidth * (cos1 - kappa * sin1),
												centerY - halfHeight * (sin1 + kappa * cos1),
												centerX + halfWidth * cos1,
												centerY - halfHeight * sin1});
				}
			}
			return pointList;
		}
		public void Rectangle(float x, float y, float width, float height) {
			Stream.SetStringLine(pdfHashtable.GetRectangle(x, y, width, height));
		}
		public void ClosePath() {
			Stream.SetStringLine("h");
		}
		public void NewPath() {
			Stream.SetStringLine("n");
		}
		public void Stroke() {
			Stream.SetStringLine("S");
		}
		public void ClosePathAndStroke() {
			Stream.SetStringLine("s");
		}
		public void Shading(PdfShading shading) {
			Stream.SetStringLine("/" + shading.Name + " sh");
		}
		public void Pattern(PdfPattern pattern) {
			Stream.SetStringLine("/Pattern cs /" + pattern.Name + " scn");
		}
		public void Fill() {
			Stream.SetStringLine("f");
		}
		public void EoFill() {
			Stream.SetStringLine("f*");
		}
		public void FillAndStroke() {
			Stream.SetStringLine("B");
		}
		public void ClosePathFillAndStroke() {
			Stream.SetStringLine("b");
		}
		public void EoFillAndStroke() {
			Stream.SetStringLine("B*");
		}
		public void ClosePathEoFillAndStroke() {
			Stream.SetStringLine("b*");
		}
		public void Clip() {
			Stream.SetStringLine("W");
		}
		public void EoClip() {
			Stream.SetStringLine("W*");
		}
		#endregion
		#region Text States
		public void BeginText() {
			Stream.SetStringLine("BT");
		}
		public void EndText() {
			Stream.SetStringLine("ET");
		}
		internal void SetFontAndSize(PdfFont pdfFont, Font actualFont) {
			if(pdfFont == null)
				return;
			if(pdfFont.Name == null)
				return;
			if(actualFont.Size < 0 || actualFont.Size > PdfFont.MaxFontSize)
				return;
			owner.Fonts.AddUnique(pdfFont);
			Stream.SetStringLine("/" + pdfFont.Name + " " + Utils.ToString(actualFont.Size) + "  Tf");
			pdfFont.CreateInnerFont();
			SetFont(pdfFont, actualFont);
		}
		public void MoveTextPoint(float tx, float ty) {
			Stream.SetStringLine(pdfHashtable.GetSize(tx, ty));
		}
		public void MoveTextToNextLine() {
			Stream.SetStringLine("T*");
		}
		public void ShowText(string text) {
			ShowText(new TextRun() { Text = text });
		}
		public void ShowText(TextRun run) {
			ShowText(run, " Tj");
		}
		public void ShowTextOnNextLine(string text) {
			ShowText(new TextRun() { Text = text }, " '");
		}
		void ShowText(TextRun textRun, string controlString) {
			this.CurrentFont.CharCache.RegisterTextRun(textRun);
			Stream.SetString(this.CurrentFont.ProcessText(textRun));
			Stream.SetStringLine(controlString);
		}
		public override void SetCharSpacing(float charSpace) {
			base.SetCharSpacing(charSpace);
			Stream.SetStringLine(Utils.ToString(charSpace) + " Tc");
		}
		public override void SetWordSpacing(float wordSpace) {
			base.SetWordSpacing(wordSpace);
			Stream.SetStringLine(Utils.ToString(wordSpace) + " Tw");
		}
		public override void SetHorizontalScaling(int scale) {
			base.SetHorizontalScaling(scale);
			Stream.SetStringLine(Convert.ToString(scale) + " Tz");
		}
		public void SetLeading(float leading) {
			Stream.SetStringLine(Utils.ToString(leading) + " TL");
		}
		public void SetRenderingMode(PdfTextRenderingMode render) {
			Stream.SetStringLine(Convert.ToString(Convert.ToInt16(render)) + " Tr");
		}
		public void SetTextRise(int rise) {
			Stream.SetStringLine(Convert.ToString(rise) + " Ts");
		}
		public void SetTextMatrix(float x, float y) {
			SetTextMatrix(1, 0, 0, 1, x, y);
		}
		public void SetTextMatrix(float a, float b, float c, float d, float x, float y) {
			Stream.SetStringLine(
				Utils.ToString(a) + " " +
				Utils.ToString(b) + " " +
				Utils.ToString(c) + " " +
				Utils.ToString(d) + " " +
				Utils.ToString(x) + " " +
				Utils.ToString(y) + " Tm");
		}
		#endregion
		#region External Objects States
		public void ExecuteXObject(PdfXObject xObject) {
			if(xObject != null)
				Stream.SetStringLine("/" + xObject.Name + " Do");
		}
		#endregion
		public void ExecuteGraphicsState(PdfTransparencyGS gs) {
			if(gs != null)
				Stream.SetStringLine("/" + gs.Name + " gs");
		}
	}
	public enum PdfTextRenderingMode {
		Fill,
		Stroke,
		FillThenStroke,
		Invisible,
		FillClipping,
		StrokeClipping,
		FillStrokeClipping,
		Clipping
	}
	public enum PdfLineCapStyle {
		Butt = 0,
		Round = 1,
		ProtectingSquare = 2
	}
	public enum PdfLineJoinStyle {
		Miter = 0,
		Round = 1,
		Bevel = 2
	}
}
