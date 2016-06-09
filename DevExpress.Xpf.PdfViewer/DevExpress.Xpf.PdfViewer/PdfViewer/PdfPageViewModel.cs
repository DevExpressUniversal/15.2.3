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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Mvvm;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfPageViewModel : BindableBase, IPdfPage, IPage {
		const double DpiMultiplierInternal = 152d / 96d;
		static double ScaleX = ScreenHelper.ScaleX;
		readonly PdfPage page;
		readonly PdfFontStorage fontStorage;
		readonly int pageIndex;
		readonly int pageNumber;
		readonly Size pageSize;
		Size visibleSize;
		readonly double userUnit;
		readonly Size inchPageSize;
		bool isSelected;
		IEnumerable<PdfElement> renderContent;
		bool needsInvalidate;
		bool forceInvalidate;
		internal bool HasFunctionalLimits { get { return Page.HasFunctionalLimits; } }
		public PdfPage Page { get { return page; } }
		public PdfFontStorage FontStorage { get { return fontStorage; } }
		public PdfDocumentArea PageArea { get { return new PdfDocumentArea(PageNumber, Page.CropBox); } }
		public int PageIndex { get { return pageIndex; } }
		public int PageNumber { get { return pageNumber; } }
		public Size PageSize { get { return pageSize; } }
		public double UserUnit { get { return userUnit; } }
		public Size InchPageSize { get { return inchPageSize; } }
		public Thickness Margin { get { return new Thickness(5); } }
		public bool NeedsInvalidate {
			get { return needsInvalidate; }
			internal set { SetProperty(ref needsInvalidate, value, () => NeedsInvalidate); }
		}
		public bool ForceInvalidate {
			get { return forceInvalidate; }
			internal set { SetProperty(ref forceInvalidate, value, () => ForceInvalidate); }
		}
		public IEnumerable<PdfElement> RenderContent {
			get { return renderContent; }
			internal set { SetProperty(ref renderContent, value, () => RenderContent); }
		}
		public Size VisibleSize {
			get { return visibleSize; }
			private set { SetProperty(ref visibleSize, value, () => VisibleSize); }
		}
		public bool IsSelected {
			get { return isSelected; }
			internal set { SetProperty(ref isSelected, value, () => IsSelected); }
		}
		public PdfPageViewModel(PdfPage page, PdfFontStorage fontStorage, int pageIndex) {
			this.page = page;
			this.fontStorage = fontStorage;
			this.pageIndex = pageIndex;
			this.pageNumber = pageIndex + 1;
			userUnit = page.UserUnit;
			pageSize = GetPageSize(x => x.CropBox);
			visibleSize = GetPageSize(x => x.BleedBox);
			inchPageSize = GetInchPageSize(x => x.MediaBox);
		}
		public Point GetPoint(PdfPoint pdfPoint, double zoomFactor, double angle) {
			PdfPoint point = Page.ToUserSpace(pdfPoint, zoomFactor * DpiMultiplier, zoomFactor * DpiMultiplier, (int)angle);
			return new Point(point.X, point.Y);
		}
		public PdfPoint GetPdfPoint(Point point, double zoomFactor, double angle) {
			return Page.FromUserSpace(new PdfPoint(point.X, point.Y), zoomFactor * DpiMultiplier, zoomFactor * DpiMultiplier, (int)angle);
		}
		public Size GetPageSize(Func<PdfPage, PdfRectangle> getRectHandle) {
			PdfRectangle mediaBox = getRectHandle(Page);
			double width;
			double height;
			int rotateAngle = page.Rotate;
			if (rotateAngle == 90 || rotateAngle == 270) {
				width = mediaBox.Height;
				height = mediaBox.Width;
			}
			else {
				width = mediaBox.Width;
				height = mediaBox.Height;
			}
			return new Size(width * DpiMultiplier, height * DpiMultiplier);
		}
		public Size GetInchPageSize(Func<PdfPage, PdfRectangle> getRectHandle) {
			PdfRectangle mediaBox = getRectHandle(page);
			int rotateAngle = page.Rotate;
			double inchFactor = page.UserUnit / 72;
			return (rotateAngle == 90 || rotateAngle == 270) ?
				new Size(mediaBox.Height * inchFactor, mediaBox.Width * inchFactor) :
				new Size(mediaBox.Width * inchFactor, mediaBox.Height * inchFactor);
		}
		public double DpiMultiplier {
			get { return page.UserUnit * DpiMultiplierInternal / ScaleX; }
		}
		bool IPage.IsLoading {
			get { return false; }
		}
		public PdfPoint CalcTopLeftPoint(double angle) {
			return page.GetTopLeftCorner((int)angle);
		}
	}
}
