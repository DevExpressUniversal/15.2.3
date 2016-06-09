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
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfMetafileGraphics : PdfDisposableObject {
		bool processEmf = true;
		readonly PdfGraphicsFormContentCommandConstructor graphics;
		readonly Dictionary<int, IEmfStateObject> objectTable = new Dictionary<int, IEmfStateObject>();
		readonly Dictionary<int, object> emfPlusObjectTable = new Dictionary<int, object>();
		readonly EmfPlusGraphicsStateStack emfPlusState = new EmfPlusGraphicsStateStack();
		Font currentFont;
		Color fontColor;
		Color bkColor;
		public Font CurrentFont { get { return currentFont; } }
		public Color FontColor { get { return fontColor; } }
		public Color BkColor { get { return bkColor; } }
		public bool ProcessEmf {
			get { return processEmf; }
			set { processEmf = value; }
		}
		internal EmfMetafileGraphics(PdfGraphicsFormContentCommandConstructor graphics) {
			this.graphics = graphics;
		}
		public void DrawPolygon16(EmfPointS[] points) {
			graphics.DrawPolygon(ToPointF(points));
		}
		public void DrawPolyLine16(EmfPointS[] points) {
			graphics.DrawLines(ToPointF(points));
		}
		public void DrawBeziers(EmfPointS[] points) {
			graphics.DrawBeziers(ToPointF(points));
		}
		public void Save(int id) {
			emfPlusState.Push(id);
			graphics.SaveGraphicsState();
		}
		public void Restore(int id) {
			int stateId;
			do {
				graphics.RestoreGraphicsState();
				emfPlusState.Pop();
				int? currentId = emfPlusState.CurrentId;
				stateId = emfPlusState.CurrentId.Value;
			}
			while (stateId != id);
		}
		public void SaveGraphicsState() {
			graphics.SaveGraphicsState();
		}
		public void RestoreGraphicsState(int count) {
			if (count < 0)
				for (int i = count; i < 0; i++) 
					graphics.RestoreGraphicsState();
		}
		public void TranslateTransform(double x, double y) {
			graphics.TranslateTransform(x, y);
			emfPlusState.Transform = PdfTransformationMatrix.Translate(emfPlusState.Transform, x, y);
		}
		public void DrawPath(PointF[] pathPoints, byte[] pathTypes) {
			graphics.DrawPath(pathPoints, pathTypes);
		}
		public void DrawRectangles(RectangleF[] rects) {
			foreach (RectangleF rect in rects)
				graphics.DrawRectangle(rect);
		}
		public void DrawBeziers(PointF[] points) {
			graphics.DrawBeziers(points);
		}
		public void DrawEllipse(RectangleF bounds) {
			graphics.DrawEllipse(bounds);
		}
		public void DrawLines(PointF[] points) {
			graphics.DrawLines(points);
		}
		public void DrawPolygon(PointF[] points) {
			graphics.DrawPolygon(points);
		}
		public void DrawPath(int id) {
			EmfPlusPath path = GetEmfPlusObject<EmfPlusPath>(id);
			if (path != null)
				graphics.DrawPath(path.Points, path.Types);
		}
		public void RotateTransform(float angle) {
			emfPlusState.Transform = PdfTransformationMatrix.Rotate(emfPlusState.Transform, angle);
			graphics.RotateTransform(angle);
		}
		public void FillEllipse(RectangleF bounds) {
			graphics.FillEllipse(bounds);
		}
		public void FillPath(PointF[] pathPoints, byte[] pathTypes) {
			graphics.FillPath(pathPoints, pathTypes, true);
		}
		public void FillPath(int id) {
			EmfPlusPath path = GetEmfPlusObject<EmfPlusPath>(id);
			if (path != null)
				FillPath(path.Points, path.Types);
		}
		public void FillRects(RectangleF[] rects) {
			foreach (RectangleF rect in rects)
				graphics.FillRectangle(rect);
		}
		public void SetBrush(int id) {
			EmfPlusBrush brush = GetEmfPlusObject<EmfPlusBrush>(id);
			if (brush != null)
				graphics.SetBrush(brush.BrushContainer);
		}
		public void MultiplyWorldTransform(PdfTransformationMatrix matrix, bool direction) {
			if (direction)
				SetMatrix(PdfTransformationMatrix.Multiply(emfPlusState.Transform, matrix));
			else 
				UpdateMatrix(matrix);
		}
		public void SetWorldTransform(PdfTransformationMatrix matrix) {
			SetMatrix(matrix);
		}
		public void SetBrush(PdfSolidBrush brush) {
			graphics.SetBrush(brush);
		}
		public void SetPen(int id) {
			EmfPlusPen pen = GetEmfPlusObject<EmfPlusPen>(id);
			if (pen != null)
				graphics.SetPen(pen.GetPen());
		}
		public void SetWorldTransform(EmfTransformationMatrix matrix, EmfModifyWorldTransformMode mode) {
			PdfTransformationMatrix transform = matrix.GetMatrix();
			switch (mode) {
				case EmfModifyWorldTransformMode.MWT_IDENTITY:
					SetMatrix(new PdfTransformationMatrix());
					return;
				case EmfModifyWorldTransformMode.MWT_LEFTMULTIPLY:
					UpdateMatrix(transform);
					return;
				case EmfModifyWorldTransformMode.MWT_RIGHTMULTIPLY:
					transform = PdfTransformationMatrix.Multiply(emfPlusState.Transform, transform);
					break;
				default:
					break;
			}
			SetMatrix(transform);
		}
		public void SetPen(PdfPen pen) {
			graphics.SetPen(pen);
		}
		public void SelectObject(int index) {
			IEmfStateObject value;
			if (objectTable.TryGetValue(index, out value))
				value.SetObject(this);
		}
		public void DeleteObject(int index) {
			IEmfStateObject value;
			if (objectTable.TryGetValue(index, out value)) {
				value.DeleteObject();
				objectTable.Remove(index);
			}
		}
		public void FillPolygon(PointF[] points) {
			graphics.FillPolygon(points, false);
		}
		public void DrawImage(int id, RectangleF bounds, RectangleF srcRect) {
			EmfPlusImage img = emfPlusObjectTable[id] as EmfPlusImage;
			if (img == null)
				return;
			Image image = img.GetImage();
			Metafile metafile = image as Metafile;
			if (metafile != null) {
				graphics.DrawMetafile(graphics.ImageCache.GetPdfFormObjectNumber(metafile), bounds);
				return;
			}
			Bitmap bmp = image as Bitmap;
			if (bmp != null) {
				using (Bitmap actualBitmap = bmp.Clone(srcRect, bmp.PixelFormat))
					graphics.DrawImage(graphics.ImageCache.GetPdfImageObjectNumber(actualBitmap, false, null, 0), bounds);
				return;
			}
		}
		public void AddObject(int index, IEmfStateObject stateObject) {
			if (!objectTable.ContainsKey(index))
				objectTable.Add(index, stateObject);
		}
		public void SetMiterLimit(int value) {
			graphics.SetMiterLimit(value);
		}
		public void ResetClip() {
			using (Region region = new Region()) {
				region.MakeInfinite();
				SetClip(new EmfPlusRegion(region), EmfPlusCombineMode.CombineModeReplace);
			}
		}
		public void SetPageUnit(GraphicsUnit unit) {
			switch (unit) {
				case GraphicsUnit.Document:
					SetDpi(300f);
					break;
				case GraphicsUnit.Inch:
					SetDpi(1f);
					break;
				case GraphicsUnit.Millimeter:
					SetDpi(25.4f);
					break;
				case GraphicsUnit.Pixel:
					SetDpi(96f);
					break;
				case GraphicsUnit.Point:
					SetDpi(72f);
					break;
				default:
					break;
			}
		}
		public void SetClipPath(int id, EmfPlusCombineMode mode) {
			EmfPlusPath path = emfPlusObjectTable[id] as EmfPlusPath;
			if (path == null)
				return;
			using (GraphicsPath gPath = new GraphicsPath(path.Points, path.Types)) {
				using (EmfPlusRegion region = new EmfPlusRegion(new Region(gPath)))
					SetClip(region, mode);
			}
		}
		public void SetClipRegion(int id, EmfPlusCombineMode mode) {
			EmfPlusRegion region = emfPlusObjectTable[id] as EmfPlusRegion;
			if (region == null)
				return;
			SetClip(region, mode);
		}
		public void SetClipRectangle(RectangleF rect, EmfPlusCombineMode mode) {
			using (EmfPlusRegion region = new EmfPlusRegion(new Region(rect)))
				SetClip(region, mode);
		}
		public void ApplyClip() {
			if (emfPlusState.Clip == null)
				return;
			graphics.IntersectClipWithoutWorldTransform(emfPlusState.Clip.GetClip());
		}
		public void SetFont(Font font) {
			currentFont = font;
		}
		public void SetBkColor(Color color) {
			bkColor = color;
		}
		public void SetFontColor(Color color) {
			fontColor = color;
		}
		public void DrawString(string text, RectangleF layoutRect, int fontId, Color? color, int brushId, int formatId) {
			fontColor = SetFontColor(color, brushId);
			using (Font font = GetFont(fontId)) {
				PdfEditableFontData fontData = graphics.FontCache.GetEditableFontData(font.Style, font.FontFamily.Name);
				EmfPlusStringFormat stringFormat = GetEmfPlusObject<EmfPlusStringFormat>(formatId);
				if (stringFormat != null)
					graphics.DrawString(text, layoutRect, stringFormat.Format, fontData, font.SizeInPoints, fontColor, false);
				else
					graphics.DrawString(text, layoutRect.Location, PdfGraphicsTextOrigin.TopLeftCorner, fontData, font.SizeInPoints, fontColor, false);
			}
		}
		public void DrawUnicodeString(char[] glyphs, PointF[] positions, int fontId, int brushId, Color? color, PdfTransformationMatrix transform) {
			SetFontColor(color, brushId);
			using (Font font = GetFont(fontId)) {
				PdfEditableFontData fontData = graphics.FontCache.GetEditableFontData(font.Style, font.FontFamily.Name);
				for (int i = 0; i < glyphs.Length; i++) {
					graphics.DrawString(glyphs[i].ToString(), positions[i], PdfGraphicsTextOrigin.Baseline, fontData, font.SizeInPoints, fontColor, false);
				}
			}
		}
		public void Clear(Color color) {
			SetBrush(new PdfSolidBrush(color));
			graphics.Clear();
		}
		public void AddEmfPlusContinuedObject(int index, EmfPlusContinuedObject obj) {
			if (emfPlusObjectTable.ContainsKey(index)) {
				EmfPlusContinuedObject continuedObject = emfPlusObjectTable[index] as EmfPlusContinuedObject;
				if (continuedObject == null)
					emfPlusObjectTable.Add(index, obj);
				else
					emfPlusObjectTable[index] = continuedObject.Append(obj);
			}
			else
				emfPlusObjectTable.Add(index, obj);
		}
		public void AddEmfPlusObject(int index, object obj) {
			if (emfPlusObjectTable.ContainsKey(index))
				emfPlusObjectTable[index] = obj;
			else
				emfPlusObjectTable.Add(index, obj);
		}
		T GetEmfPlusObject<T>(int id) where T : class {
			object obj;
			if (emfPlusObjectTable.TryGetValue(id, out obj))
				return obj as T;
			return null;
		}
		PointF[] ToPointF(EmfPointS[] points) {
			int count = points.Length;
			PointF[] fPoints = new PointF[count];
			for (int i = 0; i < count; i++) {
				EmfPointS point = points[i];
				fPoints[i] = new PointF((float)point.X, (float)point.Y);
			}
			return fPoints;
		}
		void SetMatrix(PdfTransformationMatrix matrix) {
			emfPlusState.Transform = PdfTransformationMatrix.Invert(emfPlusState.Transform);
			graphics.UpdateTransformationMatrix(emfPlusState.Transform);
			UpdateMatrix(matrix);
		}
		void UpdateMatrix(PdfTransformationMatrix matrix) {
			graphics.UpdateTransformationMatrix(matrix);
			emfPlusState.Transform = matrix;
		}
		void SetClip(EmfPlusRegion region, EmfPlusCombineMode mode) {
			PdfTransformationMatrix transform = emfPlusState.Transform;
			using (Matrix matrix = new Matrix((float)transform.A, (float)transform.B, (float)transform.C, (float)transform.D, (float)transform.E, (float)transform.F))
				region.Transform(matrix);
			if (emfPlusState.Clip != null)
				emfPlusState.Clip.Combine(mode, region);
			else
				emfPlusState.Clip = region.Clone();
		}
		void SetDpi(float dpi) {
			graphics.DpiX = dpi;
			graphics.DpiY = dpi;
		}
		Color SetFontColor(Color? color, int brushId) {
			Color fontColor = Color.Empty;
			if (color.HasValue) {
				fontColor = color.Value;
				graphics.SetBrush(new PdfSolidBrush(fontColor));
			}
			else {
				EmfPlusBrush emfBrush = GetEmfPlusObject<EmfPlusBrush>(brushId);
				if (emfBrush != null)
					graphics.SetBrush(emfBrush.BrushContainer);
			}
			return fontColor;
		}
		Font GetFont(int fontId) {
			EmfPlusFont emfFont = GetEmfPlusObject<EmfPlusFont>(fontId);
			if (emfFont == null)
				return new Font("Arial", 10);
			return emfFont.GetFont();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				emfPlusState.Dispose();
				foreach (object obj in emfPlusObjectTable.Values) {
					IDisposable disposableObject = obj as IDisposable;
					if (disposableObject != null)
						disposableObject.Dispose();
				}
				if (currentFont != null)
					currentFont.Dispose();
			}
		}
	}
}
