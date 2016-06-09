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
using System.IO;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Localization;
using System;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.Imaging {
	public abstract class ImageDocumentBuilderBase {
		PrintingSystemBase ps;
		DocumentInfo docInfo;
		ImageExportOptions options;
		internal DocumentInfo DocInfo { get { return docInfo; } }
		internal protected ImageExportOptions Options { get { return options; } }
		internal protected PrintingSystemBase Ps { get { return ps; } }
		internal protected abstract ImageGraphicsFactory ImageGraphicsFactory { get; }
		protected abstract float DocumentWidth { get; }
		protected abstract float DocumentHeight { get; }
		protected SizeF DocumentSize {
			get { return new SizeF(Math.Max(1, DocumentWidth), Math.Max(1, DocumentHeight)); }
		}
		protected ImageDocumentBuilderBase(PrintingSystemBase ps, ImageExportOptions options) {
			this.ps = ps;
			this.options = options;
			docInfo = new DocumentInfo();
		}
		public abstract void CreateDocument(Stream stream);
		internal protected void DrawImageContent(IGraphics gr, SizeF size) {
			Color backColor = GetValidColor(Ps.Graph.PageBackColor);
			using(Brush brush = new SolidBrush(backColor)) {
				gr.FillRectangle(brush, 0, 0, size.Width, size.Height);
			}
			DrawDocument(gr);
		}
		internal protected abstract void DrawDocument(IGraphics gr);
		internal protected abstract void FlushDocument();
		internal protected virtual void CreateImage(Stream stream, ImageFormat format) {
			try {
				Image img = CreateBitmap(DocumentSize, GraphicsDpi.Document);
				IGraphics gr = CreateGraphics(img);
				gr.ClipBounds = new RectangleF(new PointF(0, 0), DocumentSize);
				try {
					DrawImageContent(gr, DocumentSize);
					DevExpress.XtraPrinting.Native.PSConvert.SaveImage(img, stream, format);
				} catch {
					throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_BigBitmapToCreate));
				} finally {
					img.Dispose();
					gr.Dispose();
				}
			} finally {
				FlushDocument();
			}
		}
		internal protected void CreatePicture(Stream stream) {
			if(IsMetafile(options.Format))
				CreateMetafile(stream);
			else
				CreateImage(stream, GetValidFormat(options.Format));
		}
		internal protected Color GetValidColor(Color c) {
			return DXColor.IsTransparentOrEmpty(c) ? Color.White : c;
		}
		internal protected Bitmap CreateBitmap(SizeF size, float dpi) {
			Size pxSize = Size.Ceiling(GraphicsUnitConverter.Convert(size, dpi, options.Resolution));
			Bitmap bitMap = null;
			try { bitMap = new Bitmap(pxSize.Width, pxSize.Height); }
			catch {
				throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_BigBitmapToCreate));
			}
			bitMap.SetResolution(options.Resolution, options.Resolution);
			return bitMap;
		}
		internal protected ImageFormat GetValidFormat(ImageFormat format) {
			ImageFormat[] formats = new ImageFormat[] { ImageFormat.Exif, ImageFormat.Icon, ImageFormat.MemoryBmp };
			foreach(ImageFormat f in formats)
				if(format == f) return ImageFormat.Png;
			return format;
		}
		internal protected bool IsMetafile(ImageFormat format) {
			return format == ImageFormat.Emf || format == ImageFormat.Wmf;
		}
		internal protected IGraphics CreateGraphics(Image img) {
			return ImageGraphicsFactory.CreateGraphics(img, ps);
		}
		void CreateMetafile(Stream stream) {
			try {
				int width = (int)Math.Round(DocumentWidth, MidpointRounding.AwayFromZero);
				int height = (int)Math.Round(DocumentHeight, MidpointRounding.AwayFromZero);
				Image img = Native.MetafileCreator.CreateInstance(stream, width, height, MetafileFrameUnit.Document, EmfType.EmfPlusOnly);
				IGraphics gr = CreateGraphics(img);
				try {
					if(gr is GdiGraphics) {
						GdiGraphics gdiGraphics = (GdiGraphics)gr;
						MetafileHeader header = ((Metafile)img).GetMetafileHeader();
						gr.ScaleTransform(header.DpiX / gdiGraphics.Dpi, header.DpiY / gdiGraphics.Dpi);
					}
					DrawImageContent(gr, DocumentSize);
				} finally {
					gr.Dispose();
					img.Dispose();
				}
			} finally {
				FlushDocument();
			}
		}
	}
}
