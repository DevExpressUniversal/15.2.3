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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.Native;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Drawing;
using DevExpress.Pdf.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Pdf;
namespace DevExpress.Xpf.PdfViewer {
	public interface IPdfDocumentSelectionResults {
		string Text { get; }
		BitmapSource GetImage(int rotationAngle);
		PdfDocumentContentType ContentType { get; }
	}
	public class PdfDocumentSelectionResults : IPdfDocumentSelectionResults {
		readonly PdfDocumentViewModel document;
		Dictionary<int, Geometry> selectionGeometry;
		public PdfSelection Selection { get; private set; }
		public string Text { get { return (Selection as PdfTextSelection).Return(x => x.Text, () => string.Empty); } }
		public bool HasSelection { get; private set; }
		public int PageIndex { get { return SelectionGeometry.Min(x => x.Key); } }
		Dictionary<int, Geometry> SelectionGeometry {
			get {
				if (selectionGeometry == null)
					selectionGeometry = GenerateGeometry();
				return selectionGeometry;
			}
		}
		public PdfDocumentContentType ContentType { get; private set; }
		public PdfDocumentSelectionResults(PdfDocumentViewModel document) {
			this.document = document;
			System.Diagnostics.Debug.Assert(document.DocumentState.SelectionState.HasSelection);
			var selection = document.DocumentState.SelectionState.Selection;
			Selection = selection;
			ContentType = selection.ContentType;
		}
		public Rect GetBoundBox(double zoomFactor, double angle) {
			return GetBoundBox(PageIndex, zoomFactor, angle);
		}
		public Rect GetBoundBox(int pageIndex, double zoomFactor, double angle) {
			PdfPageViewModel page = document.Pages[pageIndex];
			var topLeftPoint = SelectionGeometry[pageIndex].Bounds.TopLeft;
			var bottomRightPoint = SelectionGeometry[pageIndex].Bounds.BottomRight;
			var topLeft = page.GetPoint(new PdfPoint(topLeftPoint.X, topLeftPoint.Y), zoomFactor, angle);
			var bottomRight = page.GetPoint(new PdfPoint(bottomRightPoint.X, bottomRightPoint.Y), zoomFactor, angle);
			return new Rect(topLeft, bottomRight);
		}
		Dictionary<int, Geometry> GenerateGeometry() {
			Dictionary<int, Geometry> geometryRects = new Dictionary<int, Geometry>();
			foreach (var highlight in Selection.Highlights) {
				CombinedGeometry combinedGeometry = new CombinedGeometry() { GeometryCombineMode = GeometryCombineMode.Union };
				var rectangles = highlight.Rectangles;
				foreach (var rect in rectangles) {
					Geometry geometry = GetRectangleGeometry(rect);
					combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, combinedGeometry, geometry);
				}
				if (geometryRects.ContainsKey(highlight.PageIndex)) {
					var geometry = geometryRects[highlight.PageIndex];
					geometryRects[highlight.PageIndex] = new CombinedGeometry(GeometryCombineMode.Union, combinedGeometry, geometry);
				}
				else {
					geometryRects.Add(highlight.PageIndex, combinedGeometry);
				}
			}
			return geometryRects;
		}
		Geometry GetRectangleGeometry(PdfOrientedRectangle rectangle) {
			var points = rectangle.Vertices;
			var start = points[0];
			List<LineSegment> segments = new List<LineSegment>();
			for (int i = 1; i < points.Count; i++) {
				var point = points[i];
				segments.Add(new LineSegment(new System.Windows.Point(point.X, point.Y), true));
			}
			PathFigure figure = new PathFigure(new System.Windows.Point(start.X, start.Y), segments, true);
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry;
		}
		public BitmapSource GetImage(int rotationAngle) {
			PdfImageSelection imageSelection = Selection as PdfImageSelection;
			if (imageSelection == null)
				return null;
			using (Bitmap bmp = PdfImageSelectionCommandInterpreter.GetSelectionBitmap(document.Pages[imageSelection.Highlights[0].PageIndex].Page, imageSelection, document.DocumentState.ImageDataStorage, rotationAngle, PdfRenderingCommandInterpreter.DefaultDpi))
				return bmp != null ? Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) : null;
		}
	}
}
