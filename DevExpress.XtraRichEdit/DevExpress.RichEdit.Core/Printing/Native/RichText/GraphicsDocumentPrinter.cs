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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraPrinting.Native.RichText {
	#region GraphicsDocumentPrinter
	public class GraphicsDocumentPrinter : SimpleDocumentPrinter {
		public GraphicsDocumentPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		public virtual void Format(SizeF formatSize) {
			ColumnSize = Size.Truncate(formatSize);
			Format();
		}
		public void Draw(IGraphics gr, RectangleF bounds) {
			Draw(gr, bounds, 1.0f);
		}
		public void Draw(IGraphics gr, RectangleF bounds, float scaleFactor) {			
			RectangleF oldClip = gr.ClipBounds;
			try {
				RectangleF clipBounds = gr.ClipBounds;
				float top = Math.Max(clipBounds.Top, bounds.Top);
				float bottom = Math.Min(clipBounds.Bottom, bounds.Bottom);
				if (bottom >= top) {
					clipBounds.Y = top;
					clipBounds.Height = bottom - top;
					gr.ClipBounds = clipBounds;
				}
				gr.SaveTransformState();
				try {
					gr.ResetTransform();
					gr.TranslateTransform(bounds.X, bounds.Y);
					gr.ScaleTransform(scaleFactor, scaleFactor);
					gr.ApplyTransformState(MatrixOrder.Append, false);
					RectangleF adjustedBounds = new RectangleF(0, 0, bounds.Width / scaleFactor, bounds.Height / scaleFactor);
					using (IGraphicsPainter painter = new SimpleIGraphicsPainter(gr, bounds.Location, scaleFactor)) {
						WinFormsGraphicsDocumentLayoutExporterAdapter adapter = new WinFormsGraphicsDocumentLayoutExporterAdapter();
						using (ScaledGraphicsDocumentLayoutExporter exporter = new ScaledGraphicsDocumentLayoutExporter(DocumentModel, painter, adapter, Rectangle.Round(adjustedBounds), TextColors.Defaults)) {
							RectangleF nonAdjustedClipBounds = gr.ClipBounds;
							try {
								if (gr.ClipBounds.X <= 0)
									gr.ClipBounds = SnapRectHorizontal(gr.ClipBounds, bounds.Location, gr.Dpi);
#if!SL
								AdjustClipBounds(gr);
#endif
								Export(exporter);
							}
							finally {
								gr.ClipBounds = nonAdjustedClipBounds;
							}
						}
					}
				}
				finally {
					gr.ResetTransform();
					gr.ApplyTransformState(MatrixOrder.Append, true);
				}
			}
			finally {
				gr.ClipBounds = oldClip;
			}
		}
#if!SL
		private void AdjustClipBounds(IGraphics gr) {
				GdiGraphics gdiGraphics = gr as GdiGraphics;
				if (gdiGraphics == null)
					return;
			try {
				Region clipRegion = gdiGraphics.Clip;			
				if(clipRegion.IsInfinite(gdiGraphics.Graphics)) {					
					RectangleF clipBounds = gdiGraphics.ClipBounds;
					RectangleF newClipBounds = new RectangleF(clipBounds.X + 1, clipBounds.Y + 1, clipBounds.Width - 2, clipBounds.Height - 2);
					gdiGraphics.Clip = new Region(newClipBounds);
				}
			}
			catch(Exception) {
			}
		}
#endif        
		protected override void Export(IDocumentLayoutExporter exporter) {
			IPageFloatingObjectExporter floatingObjectExporter = exporter as IPageFloatingObjectExporter;
			if (floatingObjectExporter != null && Controller.PageController.Pages.Count > 0) {
				floatingObjectExporter.ExportPageAreaCore(Controller.PageController.Pages[0], Controller.DocumentModel.MainPieceTable, page => ExportCore(exporter));
			}
			else
				base.Export(exporter);
		}
		void ExportCore(IDocumentLayoutExporter exporter) {
			base.Export(exporter);
		}
		static RectangleF SnapRectHorizontal(RectangleF rect, PointF pos, float dpi) {
			RectangleF result = rect;
			result.Offset(pos);
			result = RectHelper.SnapRectangleHorizontal(result, GraphicsDpi.Document, dpi);
			result.Offset(-pos.X, -pos.Y);
			return result;
		}
		public string GetPlainText() {
			return DocumentModel.InternalAPI.GetDocumentPlainTextContent();
		}
		public string GetFormattedPlainText() {
			using (PlainTextDocumentLayoutExporter exporter = new PlainTextDocumentLayoutExporter(DocumentModel, true)) {
				Export(exporter);
				return exporter.GetResultingText();
			}
		}
		protected internal override DocumentPrinterController CreateDocumentPrinterController() {
			return new PlatformDocumentPrinterController();
		}
		protected internal override BoxMeasurer CreateMeasurer(Graphics gr) {
			if (PrecalculatedMetricsFontCacheManager.ShouldUse())
				return new PrecalculatedMetricsBoxMeasurer(DocumentModel);
			return new GdiPlusBoxMeasurer(DocumentModel, gr);
		}
		protected override int GetRowBottom(XtraRichEdit.Layout.Row row) {
			return Math.Max(row.Bounds.Bottom, ScaledGraphicsDocumentLayoutExporter.CalcRowContentBoundsByBoxes(row).Bottom);
		}
	}
	#endregion
}
