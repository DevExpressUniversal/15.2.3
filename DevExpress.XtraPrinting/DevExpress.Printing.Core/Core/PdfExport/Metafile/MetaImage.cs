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
using System.Linq;
using System.Text;
using System.Security;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using DevExpress.XtraPrinting.Export.Pdf;
using System.Drawing.Drawing2D;
using System.Reflection;
using DevExpress.XtraPrinting;
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class MetaImage {
		System.Drawing.Imaging.Metafile image;
		PdfDrawContext template;
		PdfVectorImage owner;
		internal PdfGraphicsImpl graphics;
		Matrix baseTransform;
		PointF offset;
		GraphicsUnit defaultPageUnit;
		bool hasUnsupportedRecords;
#if DEBUGTEST
		internal Func<bool> Test_EnumerateMetafile;
#endif
		internal MetaState State { get; set; }
		public MetaImage(PdfVectorImage owner, System.Drawing.Imaging.Metafile image, PdfDrawContext template, PdfGraphicsImpl graphics, PointF offset) 
			: this(owner, image, new MetaState(image), template, graphics, offset){
		}
		internal MetaImage(PdfVectorImage owner, System.Drawing.Imaging.Metafile image, MetaState state, PdfDrawContext template, PdfGraphicsImpl graphics, PointF offset) {
			this.graphics = graphics;
			this.owner = owner;
			this.image = image;
			this.template = template;
			this.State = state;
			this.offset = offset;
			this.defaultPageUnit = graphics.PageUnit;
			this.baseTransform = CreateBaseTransform(graphics.PageUnit);
			graphics.SetTransform(baseTransform);
			graphics.ClipBounds = GetBoundingBox(graphics.PageUnit);
		}
		public void Write() {
			Bitmap unsupportedRecordsImage = new Bitmap(State.ImageSize.Width, State.ImageSize.Height);
			unsupportedRecordsImage.SetResolution(State.HorizontalResolution, State.VerticalResolution);
			hasUnsupportedRecords = false;
			if(image != null) {
				using(Graphics g = Graphics.FromImage(unsupportedRecordsImage)) {
					g.EnumerateMetafile(image, new Point(0, 0), new Graphics.EnumerateMetafileProc(MetafileCallback));
				}
			}
#if DEBUGTEST
			else if(Test_EnumerateMetafile != null)
				hasUnsupportedRecords = Test_EnumerateMetafile();
#endif
			if(hasUnsupportedRecords) {
				graphics.SetTransform(baseTransform);
				graphics.DrawImage(unsupportedRecordsImage, new RectangleF(Point.Empty, unsupportedRecordsImage.Size), Color.Transparent);
			}
			graphics.ResetTransform();
		}
		Matrix CreateBaseTransform(GraphicsUnit pageUnit) {
			Matrix transform = new Matrix();
			transform.Scale(State.LogicalDpi.X / State.HorizontalResolution, State.LogicalDpi.Y / State.VerticalResolution);
			transform.Translate(-offset.X * GraphicsDpi.UnitToDpiI(pageUnit) / State.LogicalDpi.X, -offset.Y * GraphicsDpi.UnitToDpiI(pageUnit) / State.LogicalDpi.Y);
			return transform;
		}
		RectangleF GetBoundingBox(GraphicsUnit pageUnit) {
			double sx = GraphicsDpi.UnitToDpiI(pageUnit) / State.LogicalDpi.X;
			double sy = GraphicsDpi.UnitToDpiI(pageUnit) / State.LogicalDpi.Y;
			return new RectangleF((float)(sx * offset.X), (float)(sy * offset.Y), (float)(sx * State.ImageSize.Width), (float)(sy * State.ImageSize.Height));
		}
		[SecuritySafeCritical]
		internal bool MetafileCallback(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData) {
			byte[] dataArray = null;
			if(data != IntPtr.Zero) {
				dataArray = new byte[dataSize];
				Marshal.Copy(data, dataArray, 0, dataSize);
			}
			MemoryStream ms = dataArray != null ? new MemoryStream(dataArray) : new MemoryStream();
			ProcessRecord(recordType, new MetaReader(ms), flags, dataSize);
			return true;
		}
		[SecuritySafeCritical]
		public void ProcessRecord(EmfPlusRecordType recordType, MetaReader reader, int flagsRaw = 0, int dataSize = 0) {
			byte[] flags = BitConverter.GetBytes((ushort)flagsRaw);
			switch(recordType) {
				#region WMF
				case EmfPlusRecordType.WmfRecordBase:
					break;
				case EmfPlusRecordType.WmfSaveDC:
					break;
				case EmfPlusRecordType.WmfRealizePalette:
					break;
				case EmfPlusRecordType.WmfSetPalEntries:
					break;
				case EmfPlusRecordType.WmfCreatePalette:
					WmfPaletteObject paletteObject = new WmfPaletteObject();
					paletteObject.Read(reader);
					State.AddObject(paletteObject);
					break;
				case EmfPlusRecordType.WmfSetBkMode: {
						State.BackgroundMode = (MixMode)reader.ReadInt16();
						break;
					}
				case EmfPlusRecordType.WmfSetMapMode:
					State.SetMapMode((MapMode)reader.ReadUInt16());
					break;
				case EmfPlusRecordType.WmfSetROP2:
					break;
				case EmfPlusRecordType.WmfSetRelAbs:
					break;
				case EmfPlusRecordType.WmfSetPolyFillMode:
					State.PolygonFillMode = (PolyFillMode)reader.ReadInt16();
					break;
				case EmfPlusRecordType.WmfSetStretchBltMode:
					break;
				case EmfPlusRecordType.WmfSetTextCharExtra:
					break;
				case EmfPlusRecordType.WmfRestoreDC:
					break;
				case EmfPlusRecordType.WmfInvertRegion:
					break;
				case EmfPlusRecordType.WmfPaintRegion:
					break;
				case EmfPlusRecordType.WmfSelectClipRegion:
					break;
				case EmfPlusRecordType.WmfSelectObject: {
						int objectIndex = reader.ReadUInt16();
						State.SelectObject(objectIndex, template);
						break;
					}
				case EmfPlusRecordType.WmfSetTextAlign: {
						State.TextAlign = (TextAlignmentMode)reader.ReadInt16();
						break;
					}
				case EmfPlusRecordType.WmfResizePalette:
					break;
				case EmfPlusRecordType.WmfDibCreatePatternBrush:
					break;
				case EmfPlusRecordType.WmfSetLayout:
					break;
				case EmfPlusRecordType.WmfDeleteObject:
					State.RemoveObject(reader.ReadUInt16());
					break;
				case EmfPlusRecordType.WmfCreatePatternBrush:
					break;
				case EmfPlusRecordType.WmfSetBkColor: {
						State.BackgroundColor = reader.ReadColorRGB();
						break;
					}
				case EmfPlusRecordType.WmfSetTextColor: {
						State.TextColor = reader.ReadColorRGB();
						break;
					}
				case EmfPlusRecordType.WmfSetTextJustification:
					break;
				case EmfPlusRecordType.WmfSetWindowOrg: {
						State.WindowOrigin = reader.ReadPointYX();
						break;
					}
				case EmfPlusRecordType.WmfSetWindowExt:
					State.WindowExtent = reader.ReadPointYX();
					break;
				case EmfPlusRecordType.WmfScaleWindowExt: {
						int yDenom = reader.ReadInt16();
						int yNum = reader.ReadInt16();
						int xDenom = reader.ReadInt16();
						int xNum = reader.ReadInt16();
						State.WindowExtent = new PointF(State.WindowExtent.X * xNum / xDenom, State.WindowExtent.Y * yNum / yDenom);
						break;
					}
				case EmfPlusRecordType.WmfScaleViewportExt:
					break;
				case EmfPlusRecordType.WmfSetViewportOrg:
					break;
				case EmfPlusRecordType.WmfSetViewportExt:
					break;
				case EmfPlusRecordType.WmfOffsetWindowOrg: {
						Point p = reader.ReadPointYX();
						State.WindowOrigin = new Point(State.WindowOrigin.X + p.X, State.WindowOrigin.Y + p.Y);
						break;
					}
				case EmfPlusRecordType.WmfOffsetViewportOrg:
					break;
				case EmfPlusRecordType.WmfLineTo: {
						System.Drawing.PointF p = State.Transform(reader.ReadPointYX());
						template.LineTo(p.X, p.Y);
						Stroke();
						break;
					}
				case EmfPlusRecordType.WmfMoveTo: {
						PointF p = State.Transform(reader.ReadPointYX());
						template.MoveTo(p.X, p.Y);
						break;
					}
				case EmfPlusRecordType.WmfOffsetCilpRgn:
					break;
				case EmfPlusRecordType.WmfFillRegion:
					break;
				case EmfPlusRecordType.WmfSetMapperFlags:
					break;
				case EmfPlusRecordType.WmfSelectPalette: {
						int objectIndex = reader.Read();
						State.SelectObject(objectIndex, template);
						break;
					}
				case EmfPlusRecordType.WmfCreatePenIndirect: {
						WmfPenObject pen = new WmfPenObject();
						pen.Read(reader);
						State.AddObject(pen.Pen);
						break;
					}
				case EmfPlusRecordType.WmfCreateFontIndirect: {
						WmfFontObject font = new WmfFontObject();
						font.Read(reader);
						State.AddObject(font);
						break;
					}
				case EmfPlusRecordType.WmfCreateBrushIndirect:
					WmfLogBrush logBrush = new WmfLogBrush();
					logBrush.Read(reader);
					State.AddObject(logBrush.Brush);
					break;
				case EmfPlusRecordType.WmfPolygon: {
						if(IsNullStrokeFill(false))
							break;
						ushort numberOfPoints = reader.ReadUInt16();
						PointF[] points = ReadPoints(reader, numberOfPoints);
						points = State.Transform(points);
						DrawPath(points);
						FillAndStroke();
						break;
					}
				case EmfPlusRecordType.WmfPolyline: {
						ushort numberOfPoints = reader.ReadUInt16();
						PointF[] points = ReadPoints(reader, numberOfPoints);
						points = State.Transform(points);
						DrawPath(points);
						Stroke();
						break;
					}
				case EmfPlusRecordType.WmfPolyPolygon: {
						if(IsNullStrokeFill(false))
							break;
						ushort numberOfPolygons = reader.ReadUInt16();
						ushort[] aPointsPerPolygon = new ushort[numberOfPolygons];
						for(int i = 0; i < numberOfPolygons; i++) {
							aPointsPerPolygon[i] = reader.ReadUInt16();
						}
						for(int i = 0; i < numberOfPolygons; i++) {
							PointF[] points = ReadPoints(reader, aPointsPerPolygon[i]);
							points = State.Transform(points);
							DrawPath(points);
						}
						FillAndStroke();
					}
					break;
				case EmfPlusRecordType.WmfExcludeClipRect:
					break;
				case EmfPlusRecordType.WmfIntersectClipRect:
					break;
				case EmfPlusRecordType.WmfEllipse:
					int bottom = reader.ReadInt16();
					int right = reader.ReadInt16();
					int top = reader.ReadInt16();
					int left = reader.ReadInt16();
					template.Arc(State.Transform(new Point(left, top)), State.Transform(new Point(right, bottom)), 0, 360);
					FillAndStroke();
					break;
				case EmfPlusRecordType.WmfFloodFill:
					break;
				case EmfPlusRecordType.WmfRectangle: {
						if(IsNullStrokeFill(true))
							break;
						PointF p2 = State.Transform(reader.ReadPointYX());
						PointF p1 = State.Transform(reader.ReadPointYX());
						template.Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
						FillAndStroke();
						break;
					}
				case EmfPlusRecordType.WmfSetPixel:
					break;
				case EmfPlusRecordType.WmfFrameRegion:
					break;
				case EmfPlusRecordType.WmfAnimatePalette:
					break;
				case EmfPlusRecordType.WmfTextOut:
					int stringLength = reader.ReadInt16();
					byte[] array = new byte[stringLength];
					int j;
					for(j = 0; j < stringLength; j++) {
						byte b = reader.ReadByte();
						if(b == 0)
							break;
						array[j] = b;
					}
					if((stringLength % 2) == 1)
						reader.ReadByte();
					String text;
					try {
						text = System.Text.Encoding.GetEncoding(1252).GetString(array, 0, j);
					} catch {
						text = System.Text.ASCIIEncoding.ASCII.GetString(array, 0, j);
					}
					int yStart = reader.ReadInt16();
					int xStart = reader.ReadInt16();
					break;
				case EmfPlusRecordType.WmfExtFloodFill:
					break;
				case EmfPlusRecordType.WmfRoundRect:
					break;
				case EmfPlusRecordType.WmfPatBlt:
					break;
				case EmfPlusRecordType.WmfEscape:
					break;
				case EmfPlusRecordType.WmfCreateRegion:
					break;
				case EmfPlusRecordType.WmfArc:
					break;
				case EmfPlusRecordType.WmfPie:
					break;
				case EmfPlusRecordType.WmfChord:
					break;
				case EmfPlusRecordType.WmfBitBlt:
					break;
				case EmfPlusRecordType.WmfDibBitBlt:
					break;
				case EmfPlusRecordType.WmfExtTextOut: {
						reader.ReadPointYX();
						break;
					}
				case EmfPlusRecordType.WmfStretchBlt:
					break;
				case EmfPlusRecordType.WmfDibStretchBlt:
					break;
				case EmfPlusRecordType.WmfSetDibToDev:
					break;
				case EmfPlusRecordType.WmfStretchDib: {
						TernaryRasterOperation rasterOperation = (TernaryRasterOperation)reader.ReadUInt32();
						ColorUsage colorUsage = (ColorUsage)reader.ReadUInt16();
						int srcHeight = reader.ReadInt16();
						int srcWidth = reader.ReadInt16();
						int ySrc = reader.ReadInt16();
						int xSrc = reader.ReadInt16();
						RectangleF source = new RectangleF(xSrc, ySrc, srcWidth, srcHeight);
						int destHeight = reader.ReadInt16();
						int destWidth = reader.ReadInt16();
						int yDst = reader.ReadInt16();
						int xDst = reader.ReadInt16();
						RectangleF destination = State.Transform(new RectangleF(xDst, yDst, destWidth, destHeight));
						byte[] dib = reader.ReadToEnd();
						Bitmap bitmap = DIBHelper.CreateBitmapFromDIB(dib);
						DrawImage(bitmap, source, destination);
						break;
					}
				#endregion
				#region EMF
				#endregion
				#region EMFPlus
				case EmfPlusRecordType.Object:
					ProcessEmfPlus_Object(reader, flags);
					break;
				case EmfPlusRecordType.Clear:
					ProcessEmfPlus_Clear(reader, flags);
					break;
				case EmfPlusRecordType.DrawImagePoints:
					ProcessEmfPlus_DrawImagePoints(reader, flags);
					break;
				case EmfPlusRecordType.FillRects:
					ProcessEmfPlus_FillRects(reader, flags);
					break;
				case EmfPlusRecordType.DrawRects:
					ProcessEmfPlus_DrawRects(reader, flags);
					break;
				case EmfPlusRecordType.Header:
					ProcessEmfPlus_Header(reader);
					break;
				case EmfPlusRecordType.EndOfFile:
					break;
				case EmfPlusRecordType.SetPageTransform:
					ProcessEmfPlus_SetPageTransform(reader, flags);
					break;
				case EmfPlusRecordType.DrawLines:
					ProcessEmfPlus_DrawLines(reader, flags);
					break;
				case EmfPlusRecordType.FillPolygon:
					ProcessEmfPlus_FillPolygon(reader, flags);
					break;
				case EmfPlusRecordType.DrawEllipse:
					ProcessEmfPlus_DrawEllipse(reader, flags);
					break;
				case EmfPlusRecordType.FillEllipse:
					ProcessEmfPlus_FillEllipse(reader, flags);
					break;
				case EmfPlusRecordType.DrawPath:
					ProcessEmfPlus_DrawPath(reader, flags);
					break;
				case EmfPlusRecordType.FillPath:
					ProcessEmfPlus_FillPath(reader, flags);
					break;
				case EmfPlusRecordType.DrawArc:
					ProcessEmfPlus_DrawArc(reader, flags);
					break;
				case EmfPlusRecordType.DrawPie:
					ProcessEmfPlus_DrawPie(reader, flags);
					break;
				case EmfPlusRecordType.FillPie:
					ProcessEmfPlus_FillPie(reader, flags);
					break;
				case EmfPlusRecordType.DrawBeziers:
					ProcessEmfPlus_DrawBeziers(reader, flags);
					break;
				case EmfPlusRecordType.DrawString:
					ProcessEmfPlus_DrawString(reader, flags);
					break;
				case EmfPlusRecordType.SetWorldTransform:
					ProcessEmfPlus_SetWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.MultiplyWorldTransform:
					ProcessEmfPlus_MultiplyWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.ScaleWorldTransform:
					ProcessEmfPlus_ScaleWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.TranslateWorldTransform:
					ProcessEmfPlus_TranslateWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.RotateWorldTransform:
					ProcessEmfPlus_RotateWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.ResetWorldTransform:
					ProcessEmfPlus_ResetWorldTransform(reader, flags);
					break;
				case EmfPlusRecordType.Save:
					ProcessEmfPlus_Save(reader);
					break;
				case EmfPlusRecordType.Restore:
					ProcessEmfPlus_Restore(reader);
					break;
				case EmfPlusRecordType.SetClipRect:
					ProcessEmfPlus_SetClipRect(reader, flags);
					break;
				case EmfPlusRecordType.SetClipRegion:
					ProcessEmfPlus_SetClipRegion(flags);
					break;
				case EmfPlusRecordType.SetClipPath:
					ProcessEmfPlus_SetClipPath(flags);
					break;
				case EmfPlusRecordType.SetAntiAliasMode:
					break;
				case EmfPlusRecordType.SetPixelOffsetMode:
					break;
				case EmfPlusRecordType.SetTextRenderingHint:
					break;
				#endregion
				#region No Content
				case EmfPlusRecordType.EmfHeader:
				case EmfPlusRecordType.EmfEof:
				case EmfPlusRecordType.BeginContainer:
				case EmfPlusRecordType.BeginContainerNoParams:
				case EmfPlusRecordType.EndContainer: {
					DrawUnsupportedRecord(recordType, reader, flagsRaw, dataSize);
					break;
				}
				#endregion
				default: {
					hasUnsupportedRecords = true;
					DrawUnsupportedRecord(recordType, reader, flagsRaw, dataSize);
					break;
				}
			}
		}
		void DrawUnsupportedRecord(EmfPlusRecordType recordType, MetaReader reader, int flagsRaw, int dataSize) {
			image.PlayRecord(recordType, flagsRaw, dataSize, reader.ReadToEnd());
		}
		void DrawUnsupportedRecord(EmfPlusRecordType recordType, byte[] data, int flagsRaw) {
			image.PlayRecord(recordType, flagsRaw, data.Length, data);
		}
		#region Emf Plus record handlers
		void ProcessEmfPlus_Object(MetaReader reader, byte[] flags) {
			bool c = (flags[1] & 0x80) == 0x80;
			ObjectType objectType = (ObjectType)(flags[1] & 0x7F);
			byte objectId = flags[0];
			object stateObject = null;
			switch(objectType) {
				case ObjectType.ObjectTypeImage:
					stateObject = new EmfPlusImage(reader).Image;
					break;
				case ObjectType.ObjectTypeRegion: {
						stateObject = new EmfPlusRegion(reader).Region;
						break;
					}
				case ObjectType.ObjectTypePen: {
						stateObject = new EmfPlusPen(reader).Pen;
						break;
					}
				case ObjectType.ObjectTypeBrush: {
						stateObject = new EmfPlusBrush(reader).Brush;
						break;
					}
				case ObjectType.ObjectTypePath: {
						stateObject = new EmfPlusPath(reader).Path;
						break;
					}
				case ObjectType.ObjectTypeFont: {
						stateObject = new EmfPlusFont(reader, owner);
						break;
					}
				case ObjectType.ObjectTypeStringFormat: {
						stateObject = new EmfPlusStringFormat(reader);
						break;
					}
			}
			State.AddObject(objectId, stateObject);
		}
		void ProcessEmfPlus_Clear(MetaReader reader, byte[] flags) {
			Color color = reader.ReadEmfPlusARGB();
			using(Brush brush = new SolidBrush(color))
				graphics.FillRectangle(brush, GetBoundingBox(graphics.PageUnit));
		}
		void ProcessEmfPlus_DrawImagePoints(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			bool effect = (flags[1] & 0x20) == 0x20;
			bool pointDataType = (flags[1] & 0x8) == 0x8;
			byte objectId = flags[0];
			uint imageAttributesID = reader.ReadUInt32();
			GraphicsUnit srcUnit = (GraphicsUnit)reader.ReadInt32();
			RectangleF srcRect = reader.ReadRectF();
			uint count = reader.ReadUInt32();
			PointF[] points = ReadPoints(reader, count, compressed, pointDataType);
			RectangleF destination = new RectangleF(points[0], new SizeF(points[1].X - points[0].X, points[2].Y - points[0].Y)); 
			Image image = (Image)State.GetObject(objectId);
			if(image is System.Drawing.Imaging.Metafile) {
				Bitmap unsupportedRecordsImage = BitmapCreator.CreateBitmapWithResolutionLimit(image, Color.Transparent);
				BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
				using(Graphics gr = Graphics.FromImage(unsupportedRecordsImage)) {
					BufferedGraphics myBuffer = currentContext.Allocate(gr, new Rectangle((int)destination.X, (int)destination.Y, (int)destination.Width, (int)destination.Height));
					myBuffer.Graphics.Clear(Color.White);
					myBuffer.Graphics.DrawImage(image, srcRect);
					myBuffer.Render();
				}
				image = unsupportedRecordsImage;
			}
			graphics.DrawImage(image, destination, srcRect, Color.Empty);
		}
		void ProcessEmfPlus_FillRects(MetaReader reader, byte[] flags) {
			Brush brush = ReadBrushEmfPlus(reader, flags);
			bool compressed = (flags[1] & 0x40) == 0x40;
			uint count = reader.ReadUInt32();
			RectangleF[] rectangles = ReadRectangles(reader, (int)count, compressed);
			foreach(RectangleF rectangle in rectangles) {
				graphics.FillRectangle(brush, rectangle);
			}
		}
		void ProcessEmfPlus_DrawRects(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			Pen pen = GetPenEmfPlus(flags);
			uint count = reader.ReadUInt32();
			RectangleF[] rectangles = ReadRectangles(reader, count, compressed);
			foreach(RectangleF rectangle in rectangles) {
				graphics.DrawRectangle(pen, rectangle);
			}
		}
		void ProcessEmfPlus_Header(MetaReader reader) {
			new EmfPlusGraphicsVersion(reader);
			uint emfPlusFlags = reader.ReadUInt32();
			uint logicalDpiX = reader.ReadUInt32();
			uint logicalDpiY = reader.ReadUInt32();
			State.LogicalDpi = new Point((int)logicalDpiX, (int)logicalDpiX);
		}
		void ProcessEmfPlus_SetPageTransform(MetaReader reader, byte[] flags) {
			GraphicsUnit newPageUnit = (GraphicsUnit)flags[0];
			Matrix newTransform = baseTransform;
			newTransform.Invert();
			baseTransform = CreateBaseTransform(newPageUnit);
			newTransform.Multiply(baseTransform, MatrixOrder.Append);
			graphics.PageUnit = newPageUnit;
			graphics.MultiplyTransform(newTransform, MatrixOrder.Append);
			newTransform.Dispose();
			float pageScale = reader.ReadSingle();
		}
		void ProcessEmfPlus_DrawLines(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			bool l = (flags[1] & 0x20) == 0x20;
			bool pointDataType = (flags[1] & 0x8) == 0x8;
			Pen pen = GetPenEmfPlus(flags);
			uint count = reader.ReadUInt32();
			PointF[] points = new PointF[count];
			points = ReadPoints(reader, count, compressed, pointDataType);
			graphics.DrawLines(pen, points);
		}
		void ProcessEmfPlus_FillPolygon(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			bool pointDataType = (flags[1] & 0x8) == 0x8;
			Brush brush = ReadBrushEmfPlus(reader, flags);
			uint count = reader.ReadUInt32();
			PointF[] points = new PointF[count];
			points = ReadPoints(reader, count, compressed, pointDataType);
			byte[] pointTypes = new byte[points.Length];
			pointTypes[0] = (byte)PathPointType.Start;
			for(int i = 1; i < pointTypes.Length; i++) {
				pointTypes[i] = (byte)PathPointType.Line;
			}
			GraphicsPath path = new GraphicsPath(points, pointTypes, FillMode.Alternate);
			graphics.FillPath(brush, path);
		}
		void ProcessEmfPlus_DrawEllipse(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			Pen pen = GetPenEmfPlus(flags);
			RectangleF rectData = ReadRectangles(reader, 1, compressed)[0];
			graphics.DrawEllipse(pen, rectData);
		}
		void ProcessEmfPlus_FillEllipse(MetaReader reader, byte[] flags) {
			Brush brush = ReadBrushEmfPlus(reader, flags);
			bool compressed = (flags[1] & 0x40) == 0x40;
			RectangleF rectData = ReadRectangles(reader, 1, compressed)[0];
			graphics.FillEllipse(brush, rectData);
		}
		void ProcessEmfPlus_DrawPath(MetaReader reader, byte[] flags) {
			byte objectId = flags[0];
			GraphicsPath path = (GraphicsPath)State.GetObject(objectId);
			Pen pen = ReadPenEmfPlus(reader);
			graphics.DrawPath(pen, path);
		}
		void ProcessEmfPlus_FillPath(MetaReader reader, byte[] flags) {
			byte objectId = flags[0];
			GraphicsPath path = (GraphicsPath)State.GetObject(objectId);
			Brush brush = ReadBrushEmfPlus(reader, flags);
			graphics.FillPath(brush, path);
		}
		void ProcessEmfPlus_DrawArc(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			Pen pen = GetPenEmfPlus(flags);
			float startAngle = reader.ReadSingle();
			float sweepAngle = reader.ReadSingle();
			RectangleF rectData = reader.ReadRectangles(compressed, 1)[0];
			using(GraphicsPath path = new GraphicsPath()) {
				path.AddArc(rectData, startAngle, sweepAngle);
				graphics.DrawPath(pen, path);
			}
		}
		void ProcessEmfPlus_DrawPie(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			Pen pen = GetPenEmfPlus(flags);
			float startAngle = reader.ReadSingle();
			float sweepAngle = reader.ReadSingle();
			RectangleF rectData = reader.ReadRectangles(compressed, 1)[0];
			using(GraphicsPath path = new GraphicsPath()) {
				path.AddPie(rectData.X, rectData.Y, rectData.Width, rectData.Height, startAngle, sweepAngle);
				graphics.DrawPath(pen, path);
			}
		}
		void ProcessEmfPlus_FillPie(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			Brush brush = ReadBrushEmfPlus(reader, flags);
			float startAngle = reader.ReadSingle();
			float sweepAngle = reader.ReadSingle();
			RectangleF rectData = reader.ReadRectangles(compressed, 1)[0];
			using(GraphicsPath path = new GraphicsPath()) {
				path.AddPie(rectData.X, rectData.Y, rectData.Width, rectData.Height, startAngle, sweepAngle);
				graphics.FillPath(brush, path);
			}
		}
		void ProcessEmfPlus_DrawBeziers(MetaReader reader, byte[] flags) {
			bool compressed = (flags[1] & 0x40) == 0x40;
			bool pointDataType = (flags[1] & 0x8) == 0x8;
			Pen pen = GetPenEmfPlus(flags);
			uint count = reader.ReadUInt32();
			PointF[] points = new PointF[count];
			points = ReadPoints(reader, count, compressed, pointDataType);
			graphics.DrawBeziers(pen, points);
		}
		void ProcessEmfPlus_DrawString(MetaReader reader, byte[] flags) {
			byte objectId = flags[0];
			EmfPlusFont emfPlusFont = (EmfPlusFont)State.GetObject(objectId);
			Brush brush = ReadBrushEmfPlus(reader, flags);
			uint formatID = reader.ReadUInt32();
			uint length = reader.ReadUInt32();
			RectangleF layoutRect = reader.ReadRectF();
			string str = GetUnicodeStringData(reader, (int)length);
			float fontSize = GetFontSize(emfPlusFont);
			using(Font font = new System.Drawing.Font(emfPlusFont.FamilyName, fontSize, emfPlusFont.FontStyleFlags))
			using(StringFormat stringFormat = GetStringFormat((EmfPlusStringFormat)State.GetObject((int)formatID), layoutRect.IsEmpty)) {
				if(layoutRect.IsEmpty)
					graphics.DrawString(str, font, brush, new PointF(layoutRect.X, layoutRect.Y), stringFormat);
				else
					graphics.DrawString(str, font, brush, layoutRect, stringFormat);
			}
		}
		float GetFontSize(EmfPlusFont emfPlusFont) {
			GraphicsUnit fontUnit = (GraphicsUnit)emfPlusFont.SizeUnit;
			if(fontUnit < GraphicsUnit.Point && graphics.PageUnit >= GraphicsUnit.Point) {
				return GraphicsUnitConverter.Convert(emfPlusFont.EmSize, GraphicsDpi.UnitToDpiI(graphics.PageUnit), GraphicsDpi.Point);
			}
			if(fontUnit >= GraphicsUnit.Point && graphics.PageUnit <= GraphicsUnit.Point) {
				float fontSize = GraphicsUnitConverter.Convert(emfPlusFont.EmSize, GraphicsDpi.UnitToDpiI(fontUnit), (float)State.LogicalDpi.X);
				return GraphicsUnitConverter.Convert(fontSize, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Point);
			}
			return GraphicsUnitConverter.Convert(emfPlusFont.EmSize, GraphicsDpi.UnitToDpiI(fontUnit), GraphicsDpi.Point);
		}
		void ProcessEmfPlus_SetWorldTransform(MetaReader reader, byte[] flags) {
			EmfPlusTransformMatrix m = new EmfPlusTransformMatrix(reader);
			Matrix matrix = new Matrix(m.M11, m.M12, m.M21, m.M22, m.Dx, m.Dy);
			matrix.Multiply(baseTransform, MatrixOrder.Append);
			graphics.SetTransform(matrix);
		}
		bool IsAppendOrder(byte[] flags) {
			return (flags[1] & 0x20) == 0x20;
		}
		void AddTransform(Matrix matrix, bool append) {
			if(append) {
				using(Matrix transform = graphics.Transform.Clone())
				using(Matrix baseTransformInverted = baseTransform.Clone()) {
					baseTransformInverted.Invert();
					transform.Multiply(baseTransformInverted, MatrixOrder.Append);
					transform.Multiply(matrix, MatrixOrder.Append);
					transform.Multiply(baseTransform, MatrixOrder.Append);
					graphics.SetTransform(transform);
				}
			} else {
				graphics.MultiplyTransform(matrix, MatrixOrder.Prepend);
			}
		}
		void ProcessEmfPlus_MultiplyWorldTransform(MetaReader reader, byte[] flags) {
			bool a = (flags[1] & 0x20) == 0x20;
			EmfPlusTransformMatrix m = new EmfPlusTransformMatrix(reader);
			using(Matrix matrix = new Matrix(m.M11, m.M12, m.M21, m.M22, m.Dx, m.Dy)) {
				AddTransform(matrix, a);
			}
		}
		void ProcessEmfPlus_ScaleWorldTransform(MetaReader reader, byte[] flags) {
			bool a = (flags[1] & 0x20) == 0x20;
			float sx = reader.ReadSingle();
			float sy = reader.ReadSingle();
			using(Matrix matrix = new Matrix()) {
				matrix.Scale(sx, sy);
				AddTransform(matrix, a);
			}
		}
		void ProcessEmfPlus_TranslateWorldTransform(MetaReader reader, byte[] flags) {
			bool a = (flags[1] & 0x20) == 0x20;
			float dx = reader.ReadSingle();
			float dy = reader.ReadSingle();
			using(Matrix matrix = new Matrix()) {
				matrix.Translate(dx, dy);
				AddTransform(matrix, a);
			}
		}
		void ProcessEmfPlus_RotateWorldTransform(MetaReader reader, byte[] flags) {
			bool a = (flags[1] & 0x20) == 0x20;
			float angle = reader.ReadSingle();
			using(Matrix matrix = new Matrix()) {
				matrix.Rotate(angle);
				AddTransform(matrix, a);
			}
		}
		void ProcessEmfPlus_ResetWorldTransform(MetaReader reader, byte[] flags) {
			graphics.SetTransform(baseTransform);
		}
		void ProcessEmfPlus_Save(MetaReader reader) {
			uint stackIndex = reader.ReadUInt32();
			graphics.SaveTransformState();
		}
		void ProcessEmfPlus_Restore(MetaReader reader) {
			uint stackIndex = reader.ReadUInt32();
			graphics.ResetTransform();
			graphics.ApplyTransformState(MatrixOrder.Append, true);
		}
		void ProcessEmfPlus_SetClipRect(MetaReader reader, byte[] flags) {
			CombineMode cm = (CombineMode)(flags[1] & 0x0F);
			RectangleF clipRect = reader.ReadRectF();
			graphics.SetClip(clipRect, cm);
		}
		void ProcessEmfPlus_SetClipRegion(byte[] flags) {
			CombineMode cm = (CombineMode)(flags[1] & 0x0F);
			byte objectId = flags[0];
			Region region = (Region)State.GetObject(objectId);
			graphics.SetClip(region, cm);
		}
		void ProcessEmfPlus_SetClipPath(byte[] flags) {
			CombineMode cm = (CombineMode)(flags[1] & 0x0F);
			byte objectId = flags[0];
			GraphicsPath path = (GraphicsPath)State.GetObject(objectId);
			graphics.SetClip(path, cm);
		}
		#endregion
		static StringFormat GetStringFormat(EmfPlusStringFormat emfPlusStringFormat, bool forceNoWrap) {
			StringFormat stringFormat;
			if(emfPlusStringFormat != null) {
				if(emfPlusStringFormat.IsGenericTypographic) {
					stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
					stringFormat.FormatFlags = emfPlusStringFormat.FormatFlags;
				} else {
					stringFormat = new StringFormat(emfPlusStringFormat.FormatFlags, emfPlusStringFormat.Language);
				}
				stringFormat.Alignment = emfPlusStringFormat.Alignment;
				stringFormat.LineAlignment = emfPlusStringFormat.LineAlignment;
				stringFormat.Trimming = emfPlusStringFormat.Trimming;
				stringFormat.SetDigitSubstitution(emfPlusStringFormat.DigitLanguage, (StringDigitSubstitute)emfPlusStringFormat.DigitSubstitution);
				stringFormat.SetTabStops(emfPlusStringFormat.FirstTabOffset, emfPlusStringFormat.TabStops);
				if(emfPlusStringFormat.RangeCount > 0)
					stringFormat.SetMeasurableCharacterRanges(emfPlusStringFormat.CharRange);
			} else 
				stringFormat = (StringFormat)StringFormat.GenericDefault.Clone();
			if(forceNoWrap)
				stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.NoWrap;
			return stringFormat;
		}
		public static string GetStringData(MetaReader reader, int length) {
			byte[] stringData = new byte[length];
			int i = 0;
			while(i < length) {
				byte b = reader.ReadByte();
				if(b == 0)
					continue;
				stringData[i] = b;
				i++;
			}
			string text;
			try {
				text = System.Text.Encoding.GetEncoding(1252).GetString(stringData, 0, i);
			} catch {
				text = System.Text.ASCIIEncoding.ASCII.GetString(stringData, 0, i);
			}
			return text;
		}
		public static string GetUnicodeStringData(MetaReader reader, int length) {
			byte[] stringData = new byte[2 * length];
			reader.Read(stringData, 0, stringData.Length);
			return System.Text.Encoding.Unicode.GetString(stringData);
		}
		private void ShowText(PointF startPoint, string str) {
			WmfFontObject fontObject = State.CurrentFont;
			float fontSize = Math.Abs(State.TransformX(fontObject.Height));
			float angle = fontObject.Angle;
			float sin = (float)Math.Sin(angle);
			float cos = (float)Math.Cos(angle);
			PdfFont pdfFont = fontObject.GetFont(fontSize, owner);
			float textWidth = template.GetTextWidth(str);
			float tx = 0;
			float ty = 0;
			template.GSave();
			template.Concat(cos, sin, sin, cos, startPoint.X, startPoint.Y);
			if((State.TextAlign & TextAlignmentMode.TA_CENTER) == TextAlignmentMode.TA_CENTER)
				tx = -textWidth / 2;
			else if((State.TextAlign & TextAlignmentMode.TA_RIGHT) == TextAlignmentMode.TA_RIGHT)
				tx = -textWidth;
			if((State.TextAlign & TextAlignmentMode.TA_BASELINE) == TextAlignmentMode.TA_BASELINE)
				ty = 0;
			if(State.BackgroundMode == MixMode.OPAQUE) {
				template.SetRGBFillColor(Color.Red);
				template.Rectangle(tx, ty, textWidth, fontSize);
				template.Fill();
			}
			template.SetRGBFillColor(State.TextColor);
			template.BeginText();
			pdfFont.NeedToEmbedFont = false;
			template.SetFontAndSize(pdfFont, pdfFont.Font);
			template.SetTextMatrix(tx, ty - fontSize);
			template.ShowText(str);
			template.EndText();
			if(fontObject.Underline) {
				template.Rectangle(tx, ty - fontObject.Height / 2, textWidth, fontSize / 15);
				template.Fill();
			}
			if(fontObject.StrikeOut) {
				template.Rectangle(tx, ty - (3 * fontSize) / 4, textWidth, fontSize / 15);
				template.Fill();
			}
			template.GRestore();
		}
		private Pen GetPenEmfPlus(byte[] flags) {
			byte objectId = flags[0];
			return (Pen)State.GetObject(objectId);
		}
		private Pen ReadPenEmfPlus(MetaReader reader) {
			uint penId = reader.ReadUInt32();
			Pen pen = (Pen)State.GetObject((int)penId);
			return pen;
		}
		private Brush ReadBrushEmfPlus(MetaReader reader, byte[] flags) {
			bool s = (flags[1] & 0x80) == 0x80;
			uint brushId = reader.ReadUInt32();
			Brush brush;
			if(s) {
				brush = new SolidBrush(Color.FromArgb((int)brushId));
			} else {
				brush = (Brush)State.GetObject((int)brushId);
			}
			return brush;
		}
		private void DrawImage(Image image, RectangleF source, RectangleF destination) {
			template.GSave();
			template.Rectangle(destination.X, destination.Y, destination.Width, destination.Height);
			template.Clip();
			template.NewPath();
			template.Concat(new Matrix(destination.Width * image.Width / source.Width, 0, 0,
				destination.Height * image.Height / source.Height,
				destination.X - source.X / source.Width,
				destination.Y - source.Y / source.Height));
			template.ExecuteXObject(owner.CreateImage(image));
			template.GRestore();
		}
		internal void Stroke() {
			if(State.CurrentPen == null || State.CurrentPen.Color != Color.Transparent)
				template.Stroke();
		}
		private bool IsNullStrokeFill(bool isRectangle) {
			bool penStyleNull = (State.CurrentPen == null || State.CurrentPen.Color == Color.Transparent);
			bool isBrush;
			if(State.CurrentBrush == null) {
				isBrush = false;
			} else {
				isBrush = (State.CurrentBrush is SolidBrush || (State.CurrentBrush is HatchBrush && State.BackgroundMode == MixMode.OPAQUE));
			}
			return penStyleNull && !isBrush;
		}
		internal void FillAndStroke() {
			if(State.CurrentPen == null || State.CurrentPen.Color == Color.Transparent) {
				template.ClosePath();
				if(State.PolygonFillMode == PolyFillMode.ALTERNATE) {
					template.EoFill();
				} else {
					template.Fill();
				}
			} else if(State.CurrentBrush == null || State.CurrentBrush is NullBrush || (State.CurrentBrush is HatchBrush && State.BackgroundMode == MixMode.OPAQUE)) {
				template.ClosePathAndStroke();
			} else {
				if(State.PolygonFillMode == PolyFillMode.ALTERNATE) {
					template.ClosePathEoFillAndStroke();
				} else {
					template.ClosePathFillAndStroke();
				}
			}
		}
		private void DrawPath(PointF[] points) {
			template.MoveTo(points[0].X, points[0].Y);
			for(int i = 1; i < points.Length; i++) {
				template.LineTo(points[i].X, points[i].Y);
			}
		}
		private PointF[] ReadPoints(MetaReader reader, long numberOfPoints, bool compressed = true, bool relative = false) {
			PointF[] points = reader.ReadPoints(numberOfPoints, compressed);
			for(int i = 0; i < points.Length; i++) {
				if(relative && i > 0) {
					points[i] = new PointF(points[i].X + points[i - 1].X, points[i].Y + points[i - 1].Y);
				}
			}
			return points;
		}
		private RectangleF[] ReadRectangles(MetaReader reader, long count, bool compressed = true) {
			RectangleF[] rectangles = reader.ReadRectangles(compressed, count);
			return rectangles;
		}
	}
	public enum GraphicsMode {
		GM_COMPATIBLE = 0x00000001,
		GM_ADVANCED = 0x00000002
	}
	[CLSCompliant(false)]
	public class EmrTextObject {
		public PointF Reference { get; set; }
		public uint CharsCount { get; set; }
		public uint OffString { get; set; }
		public uint Options { get; set; }
		public RectangleF Rectangle { get; set; }
		public uint OffDx { get; set; }
		public int UndefinedSpace1 { get; set; }
		public byte[] OutputString { get; set; }
		public int UndefinedSpace2 { get; set; }
		public uint[] OutputDx { get; set; }
		public string Text { get; set; }
		public EmrTextObject(MetaReader reader) {
			Reference = new PointF(reader.ReadSingle(), reader.ReadSingle());
			CharsCount = reader.ReadUInt32();
			OffString = reader.ReadUInt32();
			Options = reader.ReadUInt32();
			Rectangle = reader.ReadRectF();
			OffDx = reader.ReadUInt32();
			Text = MetaImage.GetStringData(reader, (int)CharsCount);
		}
	}
}
