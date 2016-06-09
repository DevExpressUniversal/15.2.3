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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfImageSelector {
		const double selectionPrecision = 0.001;
		static PdfRectangle GetImageSelectionRectangle(PdfRectangle imageRectangle, PdfRectangle rect) {
			return PdfRectangle.InflateToNonEmpty(PdfRectangle.Intersect(imageRectangle, rect));
		}
		readonly IPdfViewerNavigationController navigationController;
		readonly PdfPageDataCache pageDataCache;
		readonly PdfDocumentState documentState;
		readonly PdfSelectionState selectionState;
		bool selectionInProgress;
		PdfDocumentPosition selectionStartPosition = new PdfDocumentPosition(0, default(PdfPoint));
		int selectionStartImageIndex = -1;
		public bool SelectionInProgress { 
			get { return selectionInProgress; } 
			set { selectionInProgress = value; }
		}
		public PdfImageSelector(IPdfViewerNavigationController navigationController, PdfPageDataCache pageDataCache, PdfDocumentState documentState) {
			this.navigationController = navigationController;
			this.pageDataCache = pageDataCache;
			this.documentState = documentState;
			selectionState = documentState.SelectionState;
		}
		public bool HasContent(PdfDocumentPosition position) {
			return selectionInProgress || FindImageByPosition(position) != -1;
		}
		public bool StartSelection(PdfDocumentPosition position) {
			selectionStartPosition = position;
			selectionStartImageIndex = FindImageByPosition(position);
			selectionInProgress = selectionStartImageIndex != -1;
			return selectionInProgress;
		}
		public bool PerformSelection(PdfDocumentPosition position) {
			if (selectionStartImageIndex < 0)
				return false;
			int startPageIndex = selectionStartPosition.PageIndex;
			PdfPoint startPoint = selectionStartPosition.Point;
			double startX = startPoint.X;
			double startY = startPoint.Y;
			PdfPoint endPagePoint = position.Point;
			if (Math.Abs(startX - endPagePoint.X) < selectionPrecision || Math.Abs(startY - endPagePoint.Y) < selectionPrecision) {
				if (!selectionInProgress) {
					selectionState.Selection = new PdfImageSelection(startPageIndex, pageDataCache.GetImageData(startPageIndex)[selectionStartImageIndex], null);
					selectionStartImageIndex = -1;
				}
			}
			else if (selectionInProgress) {
				IList<PdfPageImageData> imageData = pageDataCache.GetImageData(startPageIndex);
				if (imageData.Count > selectionStartImageIndex) {
					PdfPageImageData pageImageData = imageData[selectionStartImageIndex];
					if (position.PageIndex == startPageIndex)
						selectionState.Selection = new PdfImageSelection(startPageIndex, pageImageData, 
							GetImageSelectionRectangle(pageImageData.BoundingRectangle, new PdfRectangle(startPoint, endPagePoint)));
					else {
						PdfPoint clientStartPoint = navigationController.GetClientPoint(selectionStartPosition);
						double clientStartX = clientStartPoint.X;
						double clientStartY = clientStartPoint.Y;
						PdfPoint clientEndPoint = navigationController.GetClientPoint(position);
						int startPageNumber = startPageIndex + 1;
						PdfRectangle imageRectangle = pageImageData.BoundingRectangle;
						PdfPoint imageRectTopLeft = navigationController.GetClientPoint(new PdfDocumentPosition(startPageNumber, imageRectangle.TopLeft));
						PdfPoint imageRectBottomRight = navigationController.GetClientPoint(new PdfDocumentPosition(startPageNumber, imageRectangle.BottomRight));
						double realLeft = Math.Min(imageRectTopLeft.X, imageRectBottomRight.X);
						double realRight = Math.Max(imageRectTopLeft.X, imageRectBottomRight.X);
						double realXPart = clientStartX - clientEndPoint.X;
						double xOffset = (realXPart > 0 ? Math.Min(realXPart, clientStartX - realLeft) : Math.Max(realXPart, clientStartX - realRight)) / (realLeft - realRight);
						double realTop = Math.Min(imageRectTopLeft.Y, imageRectBottomRight.Y);
						double realBottom = Math.Max(imageRectTopLeft.Y, imageRectBottomRight.Y);
						double realYPart = clientStartY - clientEndPoint.Y;
						double yOffset = (realYPart > 0 ? Math.Min(realYPart, clientStartY - realTop) : Math.Max(realYPart, clientStartY - realBottom)) / (realBottom - realTop);
						switch (PdfPageTreeNode.NormalizeRotate(documentState.RotationAngle + pageDataCache.DocumentPages[startPageIndex].Rotate)) {
							case 90: {
								double temp = yOffset;
								yOffset = xOffset * imageRectangle.Height;
								xOffset = -temp * imageRectangle.Width;
								break;
							}
							case 180:
								xOffset *= -imageRectangle.Width;
								yOffset *= -imageRectangle.Height;
								break;
							case 270: {
								double temp = yOffset;
								yOffset = -xOffset * imageRectangle.Height;
								xOffset = temp * imageRectangle.Width;
								break;
							}
							default:
								xOffset *= imageRectangle.Width;
								yOffset *= imageRectangle.Height;
								break;
						}   
						selectionState.Selection = new PdfImageSelection(startPageIndex, pageImageData, 
							GetImageSelectionRectangle(imageRectangle, new PdfRectangle(startPoint, new PdfPoint(startX + xOffset, startY + yOffset))));
					}
				}
			}
			return true;
		}
		public void EndSelection() {
			selectionInProgress = false;
			selectionStartImageIndex = -1;
			selectionStartPosition = new PdfDocumentPosition(0, default(PdfPoint));
		}
		public IList<PdfImageSelection> SelectImages(PdfDocumentArea documentArea) {
			int pageIndex = documentArea.PageIndex;
			PdfRectangle area = documentArea.Area;
			IList<PdfImageSelection> selection = new List<PdfImageSelection>();
			foreach (PdfPageImageData imageData in pageDataCache.GetImageData(pageIndex)) {
				PdfRectangle imageRectangle = imageData.BoundingRectangle;
				if (area.Intersects(imageRectangle))
					selection.Add(new PdfImageSelection(pageIndex, imageData, imageRectangle));
			}
			return selection.Count == 0 ? null : selection;
		}
		public IList<PdfImageSelection> SelectImages(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			int startPageIndex = startPosition.PageIndex;
			int endPageIndex = endPosition.PageIndex;
			if (startPageIndex == endPageIndex)
				return SelectImagesOnPageBetweenPoints(startPosition, endPosition);
			if (startPageIndex > endPageIndex) {
				int temp = startPageIndex;
				double startPointY = startPosition.Point.Y;
				double endPointY = endPosition.Point.Y;
				startPageIndex = endPageIndex;
				endPageIndex = temp;
				double dTemp = startPointY;
				startPointY = endPointY;
				endPointY = dTemp;
			}
			List<PdfImageSelection> selectedImages = new List<PdfImageSelection>();
			IList<PdfImageSelection> selection = SelectImagesOnPageBetweenPoints(startPosition, null);
			if (selection != null)
				selectedImages.AddRange(selection);
			PdfPoint pageEnd = new PdfPoint(0, 0);
			for (int i = startPageIndex + 2; i <= endPageIndex; i++) {
				selection = SelectImagesOnPageBetweenPoints(null, new PdfDocumentPosition(i, pageEnd));
				if (selection != null)
					selectedImages.AddRange(selection);
			}
			selection = SelectImagesOnPageBetweenPoints(null, endPosition);
			if (selection != null)
				selectedImages.AddRange(selection);
			return selectedImages.Count == 0 ? null : selectedImages;
		}
		int FindImageByPosition(PdfDocumentPosition position) {
			int pageIndex = position.PageIndex;
			if (pageIndex > -1) {
				PdfPoint point = position.Point;
				IList<PdfPageImageData> pageImageData = pageDataCache.GetImageData(pageIndex);
				int count = pageImageData.Count;
				for (int i = 0; i < count; i++)
					if (pageImageData[i].BoundingRectangle.Contains(point))
						return i;
			}
			return -1;
		}
		IList<PdfImageSelection> SelectImagesOnPageBetweenPoints(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			int pageIndex;
			double minY = Double.MinValue;
			double maxY = Double.MaxValue;
			if (startPosition == null) {
				if (endPosition == null)
					return null;
				pageIndex = endPosition.PageIndex;
			}
			else {
				pageIndex = startPosition.PageIndex;
				if (endPosition != null) {
					minY = startPosition.Point.Y;
					maxY = endPosition.Point.Y;
					if (minY > maxY) {
						double temp = minY;
						minY = maxY;
						maxY = temp;
					}
				}
			}
			List<PdfImageSelection> selectedImages = new List<PdfImageSelection>();
			foreach (PdfPageImageData imageData in pageDataCache.GetImageData(pageIndex)) {
				PdfRectangle rect = imageData.BoundingRectangle;
				if (rect.Bottom < maxY && rect.Top > minY)
					selectedImages.Add(new PdfImageSelection(pageIndex, imageData, rect));
			}
			return selectedImages.Count == 0 ? null : selectedImages;
		}
	}
}
