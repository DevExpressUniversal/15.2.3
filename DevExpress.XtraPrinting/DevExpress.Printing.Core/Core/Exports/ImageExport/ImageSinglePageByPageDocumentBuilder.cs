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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Export.Imaging {
	public class ImageSinglePageByPageDocumentBuilder : ImageDocumentBuilderBase {
		int[] pageIndices;
		internal override protected ImageGraphicsFactory ImageGraphicsFactory { get { return ImageGraphicsFactory.MultiplePageImageGraphicsFactory; } }
		protected override float DocumentWidth { get { return DocInfo.PageWidth; } }
		protected override float DocumentHeight { get { return DocInfo.PagesHeight; } }
		public ImageSinglePageByPageDocumentBuilder(PrintingSystemBase ps, ImageExportOptions options)
			: base(ps, options) {
			pageIndices = ExportOptionsHelper.GetPageIndices(options, ps.Document.Pages.Count);
		}
		public override void CreateDocument(Stream stream) {			
			DocInfo.Update(Ps.Document.Pages, Options);
			CreatePicture(stream);
		}
		internal protected override void DrawDocument(IGraphics gr) {
			float pageVerticalPosition = 0;
			foreach(int pageIndex in pageIndices) {
				SizeF pageSize = Ps.Document.Pages[pageIndex].PageData.PageSize;
				gr.FillRectangle(gr.GetBrush(Options.PageBorderColor),
						new RectangleF(new PointF(0, pageVerticalPosition), new SizeF(pageSize.Width + 2 * DocInfo.BorderWidth, pageSize.Height + 2 * DocInfo.BorderWidth)));
				pageVerticalPosition += DocInfo.BorderWidth;
				Page page = Ps.Document.Pages[pageIndex];
				PageExporter exporter = Ps.ExportersFactory.GetExporter(page) as PageExporter;
				exporter.DrawPage(gr, new PointF(DocInfo.BorderWidth, pageVerticalPosition));
				pageVerticalPosition += (int)pageSize.Height;
				Ps.ProgressReflector.RangeValue++;
			}
		}
		internal protected override void FlushDocument() {			
		}
		internal protected override void CreateImage(Stream stream, ImageFormat format) {
			if(IsMultiPageFormat(format)) {
				DrawMultiPageImage(stream, Ps.Document);
				return;
			}
			base.CreateImage(stream, format);
		}
		bool IsMultiPageFormat(ImageFormat format) {
			return (format == ImageFormat.Tiff);
		}
		void DrawMultiPageImage(Stream stream, DevExpress.XtraPrinting.Document document) {
			ImageCodecInfo tiffCodecInfo = GetEncoderInfo("image/tiff");
			if(tiffCodecInfo == null)
				return;
			Encoder encoder = Encoder.SaveFlag;
			EncoderParameters parameters = new EncoderParameters(1);
			try {
				Image multiImg = null;
				foreach(int i in pageIndices) {
					Page page = document.Pages[i];
					Bitmap img = CreateBitmap(page.PageSize, GraphicsDpi.Document);
					IGraphics gr = CreateGraphics(img);
					gr.ClipBounds = new RectangleF(new PointF(0, 0), page.PageSize);
					try {
						PageExporter exporter = gr.PrintingSystem.ExportersFactory.GetExporter(page) as PageExporter;
						exporter.DrawPage(gr, PointF.Empty);
						if(i == pageIndices[0]) {
							parameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
							img.Save(stream, tiffCodecInfo, parameters);
							multiImg = img;
						} else {
							parameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);
							multiImg.SaveAdd(img, parameters);
						}
					} finally {
						if(img != multiImg) img.Dispose();
						gr.Dispose();
					}
					Ps.ProgressReflector.RangeValue++;
				}
				parameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
				multiImg.SaveAdd(parameters);
				multiImg.Dispose();
			} finally {
				Ps.ProgressReflector.MaximizeRange();
			}
		}
		ImageCodecInfo GetEncoderInfo(String mimeType) {
			ImageCodecInfo[] infos = ImageCodecInfo.GetImageEncoders();
			foreach(ImageCodecInfo info in infos)
				if(info.MimeType == mimeType)
					return info;
			return null;
		}
	}
}
