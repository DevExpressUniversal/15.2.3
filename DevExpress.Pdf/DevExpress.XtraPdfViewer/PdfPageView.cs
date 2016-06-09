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
using System.Drawing.Printing;
using DevExpress.DocumentView;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfPageView : PageBase {
		static void OnPageDrawing(IPage page, Graphics graphics, PointF position) {
			PdfPageView pageView = page as PdfPageView;
			if (pageView != null)
				pageView.DrawPage(graphics, position);
		}
		readonly PdfViewer viewer;
		readonly PdfPageState pageState;
		readonly PdfDocumentState documentState;
		readonly RectangleF visibleRect;
		readonly int pageIndex;
		protected override RectangleF UsefulPageRectF { get { return visibleRect; } }
		public PdfPageView(PdfViewer viewer, PdfDocumentState documentState, int pageIndex, Size pageSize, RectangleF visibleRect)
				: base(PaperKind.Custom, pageSize, new Margins(0, 0, 0, 0), false, (p, graphics, position) => OnPageDrawing(p, graphics, position)) {
			this.viewer = viewer;
			this.documentState = documentState;
			this.pageIndex = pageIndex;
			this.pageState = documentState.GetPageState(pageIndex);
			this.visibleRect = new RectangleF(visibleRect.X * 3, visibleRect.Top * 3, visibleRect.Width * 3, visibleRect.Height * 3);
		}
		void CheckFunctionalLimits() {
			if (pageState.Page.HasFunctionalLimits)
				viewer.Viewer.RaiseFunctionalLimitsOccurred();
		}
		void DrawContent(Graphics graphics, float scale, float offsetX, float offsetY, int pageIndex) {
			PdfPageCache pageCache = viewer.PageCache;
			Bitmap bitmap = pageCache.GetBitmap(pageState.Page);
			if (bitmap == null) {
				try {
					bitmap = PdfViewerCommandInterpreter.GetBitmap(documentState, pageIndex, scale, PdfRenderMode.View);
				}
				catch {
					try {
						using (PdfViewerCommandInterpreter commandInterpreter = new PdfViewerCommandInterpreter(documentState, pageIndex, graphics, scale, new PointF(offsetX / scale, offsetY / scale), PdfRenderMode.View)) {
							PdfPoint pageSize = documentState.GetPageSize(pageIndex);
							graphics.FillRectangle(Brushes.White, new Rectangle(Convert.ToInt32(offsetX), Convert.ToInt32(offsetY), Convert.ToInt32(pageSize.X * scale), Convert.ToInt32(pageSize.Y * scale)));
							commandInterpreter.DrawContent();
						}
						CheckFunctionalLimits();
					}
					catch { }
					return;
				}
				CheckFunctionalLimits();
				pageCache.AddBitmap(pageState.Page, bitmap);
			}
			graphics.DrawImage(bitmap, offsetX, offsetY);
		}
		void DrawPage(Graphics graphics, PointF position) {
			InterpolationMode interpolationMode = graphics.InterpolationMode;
			Matrix transform = graphics.Transform;
			try {
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.Transform = new Matrix();
				float scale = transform.Elements[0];
				float offsetX = position.X * scale;
				float offsetY = position.Y * scale;
				scale *= PdfRenderingCommandInterpreter.DpiFactor;
				PointF pdfLocation = new PointF(offsetX / scale, offsetY / scale);
				PdfCaret caret = viewer.Caret;
				if (caret != null && pageIndex == caret.Position.PageIndex)
					viewer.Viewer.CaretView.DrawCaret(pageState.Page, scale, pdfLocation);
				DrawContent(graphics, scale, offsetX, offsetY, pageIndex);
				using (PdfViewerCommandInterpreter interpreter = new PdfViewerCommandInterpreter(documentState, pageIndex, graphics, scale, pdfLocation, PdfRenderMode.View)) {
					interpreter.DrawAnnotations();
					interpreter.DrawSelection(viewer.SelectionColor);
				}
			}
			finally {
				Matrix m = graphics.Transform;
				graphics.Transform = transform;
				graphics.InterpolationMode = interpolationMode;
				m.Dispose();
			}
		}
	}
}
