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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Editors;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public interface IPdfNativeRendererImpl : IDisposable {
		bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle);
		void Reset();
	}
	public abstract class NativeRendererImpl : IPdfNativeRendererImpl {
		public abstract bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle);
		public void Dispose() {
			DisposeInternal();
		}
		protected virtual void DisposeInternal() {
		}
		public virtual void Reset() {
		}
		public virtual void InvalidateCaches() {
		}
	}
	public class DirectNativeRendererImpl : NativeRendererImpl {
		public override bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle) {
			if (zoomFactor.AreClose(0d))
				return true;
			var drawingContent = renderedContent.RenderedPages;
			foreach (var pair in drawingContent) {
				PdfPageViewModel page = pair.Page;
				Rect rect = pair.Rect;
				float actualZoomFactor = (float)(zoomFactor * page.DpiMultiplier * ScreenHelper.ScaleX);
				var location = new PointF((float)rect.Location.X / actualZoomFactor, (float)rect.Location.Y / actualZoomFactor);
				using (
					PdfViewerCommandInterpreter commandInterpreter = new PdfViewerCommandInterpreter(renderedContent.DocumentState, page.PageIndex, graphics, actualZoomFactor, location,
						PdfRenderMode.View)) {
					commandInterpreter.Execute();
					commandInterpreter.DrawAnnotations();
					commandInterpreter.DrawSelection(renderedContent.SelectionColor.ToWinFormsColor());
				}
			}
			return true;
		}
	}
	public class TextureCacheNativeRendererImpl : NativeRendererImpl {
		const int MaxCacheSizeDefault = 100000000;
		readonly int maxCacheSize;
		readonly RenderedPagesCache texturesCache = new RenderedPagesCache();
		static Bitmap GetPdfPageAsBitmap(PdfDocumentState documentState, RenderItem renderItem, double zoomFactor) {
			try {
				var page = renderItem.Page;
				var bmp = PdfViewerCommandInterpreter.GetBitmap(documentState, page.PageIndex, (float)zoomFactor, PdfRenderMode.View);
				return bmp;
			}
			catch {
			}
			return null;
		}
		RenderedPage CreateRenderedPage(PdfDocumentState state, RenderItem item, double zoomFactor, double angle) {
			return new RenderedPage(item.Page.PageIndex) {
				ZoomFactor = zoomFactor,
				Angle = angle,
				RenderedContent = GetPdfPageAsBitmap(state, item, zoomFactor)
			};
		}
		public TextureCacheNativeRendererImpl(int cacheSize) {
			maxCacheSize = Math.Max(cacheSize, MaxCacheSizeDefault);
		}
		RenderedPage GetRenderedPage(PdfDocumentState documentState, RenderItem item, double zoomFactor, double angle) {
			var page = texturesCache.GetPage(item.Page.PageIndex);
			if (item.ForceInvalidate) {
				page = CreateRenderedPage(documentState, item, zoomFactor, angle);
				texturesCache.AddPage(page);
				item.ForceInvalidate = false;
			}
			return page ?? new NotRenderedStub();
		}
		void ValidateCache(IEnumerable<RenderItem> drawingContent) {
			ReleaseOutdatedPages(drawingContent, false);
		}
		void ReleaseOutdatedPages(IEnumerable<RenderItem> drawingContent, bool force) {
			var renderItems = drawingContent as IList<RenderItem> ?? drawingContent.ToList();
			RenderItem first = renderItems.FirstOrDefault();
			if (first == null)
				return;
			RenderItem last = renderItems.LastOrDefault();
			int firstPageIndex = first.Page.PageIndex - 1;
			int lastPageIndex = last.Page.PageIndex + 1;
			int cacheSize = 0;
			List<int> outdatedPages = new List<int>();
			foreach (var page in texturesCache) {
				cacheSize += page.GetAllocatedSize();
				if (page.PageIndex < firstPageIndex || page.PageIndex > lastPageIndex)
					outdatedPages.Add(page.PageIndex);
			}
			if (cacheSize > maxCacheSize || force)
				ReleasePages(outdatedPages);
		}
		void ReleasePages(IEnumerable<int> outdatedPages) {
			foreach (var pageIndex in outdatedPages) {
				RenderedPageBase page = texturesCache.GetPage(pageIndex);
				page.Dispose();
				texturesCache.RemovePage(pageIndex);
			}
		}
		void ReleaseAllPages() {
			foreach (var page in texturesCache)
				page.Dispose();
			texturesCache.Clear();
		}
		public override bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle) {
			var renderItems = renderedContent.RenderedPages as IList<RenderItem> ?? renderedContent.RenderedPages.ToList();
			var documentState = renderedContent.DocumentState;
			bool needsInvalidate = false;
			ValidateCache(renderItems);
			foreach (RenderItem item in renderItems) {
				var offset = item.Rect.Location.ToWinFormsPoint();
				var realSize = item.Rect.Size.ToWinFormsSize();
				float actualZoomFactor = (float)(zoomFactor * item.Page.DpiMultiplier);
				var location = new PointF(offset.X / actualZoomFactor, offset.Y / actualZoomFactor);
				using (var interpreter = new PdfViewerCommandInterpreter(documentState, item.Page.PageIndex, graphics, actualZoomFactor, new PointF(location.X, location.Y), PdfRenderMode.View)) {
					RenderedPage renderedPage = GetRenderedPage(renderedContent.DocumentState, item, actualZoomFactor, angle);
					item.NeedsInvalidate = !renderedPage.Match(actualZoomFactor, angle);
					if (renderedPage.RenderedContent != null) {
						var correctedOffset = new PointF(offset.X, offset.Y);
						if (item.NeedsInvalidate) {
							needsInvalidate = true;
							graphics.DrawImage(renderedPage.RenderedContent, correctedOffset.X, correctedOffset.Y, realSize.Width, realSize.Height);
						}
						else {
							graphics.DrawImageUnscaled(renderedPage.RenderedContent, (int)correctedOffset.X, (int)correctedOffset.Y);
						}
						interpreter.DrawAnnotations();
						interpreter.DrawSelection(renderedContent.SelectionColor.ToWinFormsColor());
					}
					else
						needsInvalidate = true;
				}
			}
			return needsInvalidate;
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			ReleaseAllPages();
		}
		public override void Reset() {
			base.Reset();
			ReleaseAllPages();
		}
	}
	public class PdfViewerDocumentRenderer : DocumentViewerRenderer {
		IPdfNativeRendererImpl pdfRenderer;
		new PdfPresenterControl Presenter { get { return base.Presenter as PdfPresenterControl; } }
		public PdfViewerDocumentRenderer(DocumentPresenterControl presenter) : base(presenter) {
			UpdateInnerRenderer();
		}
		public void UpdateInnerRenderer() {
			pdfRenderer = Presenter.AllowCachePages ? (IPdfNativeRendererImpl)new TextureCacheNativeRendererImpl(Presenter.CacheSize) : new DirectNativeRendererImpl();
		}
		public override void Reset() {
			base.Reset();
			pdfRenderer.Reset();
		}
		public override bool RenderToGraphics(Graphics graphics, INativeImageRenderer renderer, Rect invalidateRect, System.Windows.Size totalSize) {
			PdfDocumentViewModel model = (PdfDocumentViewModel)Presenter.Document;
			if (model == null || Presenter.PdfBehaviorProvider == null) {
				SetRenderMask(new DrawingBrush(new GeometryDrawing()));
				return false;
			}
			if (!model.Return(x => x.IsLoaded, () => false)) {
				SetRenderMask(new DrawingBrush(new GeometryDrawing()));
				return true;
			}
			var renderedPages = Presenter.GetDrawingContent().ToList();
			var renderedContent = new RenderedContent() {
				RenderedPages = renderedPages,
				DocumentState = model.DocumentState,
				SelectionColor = Presenter.HighlightSelectionColor,
			};
			SetRenderMask(Presenter.GenerateRenderMask(renderedContent.RenderedPages));
			bool needsRefresh = pdfRenderer.RenderToGraphics(graphics, renderedContent, Presenter.PdfBehaviorProvider.ZoomFactor, Presenter.PdfBehaviorProvider.RotateAngle);
			renderedContent.RenderedPages.ForEach(x => x.Page.NeedsInvalidate = x.NeedsInvalidate);
			renderedContent.RenderedPages.ForEach(x => x.Page.ForceInvalidate = x.ForceInvalidate);
			if (needsRefresh)
				Presenter.ImmediateActionsManager.EnqueueAction(new InvalidateRenderingAction(Presenter));
			return true;
		}
	}
}
