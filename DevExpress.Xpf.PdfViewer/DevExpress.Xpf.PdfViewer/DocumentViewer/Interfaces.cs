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
using System.Collections.ObjectModel;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using Size = System.Windows.Size;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public interface IPdfDocument {
		IPdfDocumentSelectionResults SelectionResults { get; }
		bool HasInteractiveForm { get; }
		bool HasSelection { get; }
		bool HasOutlines { get; }
		bool HasAttachments { get; }
		PdfCaret Caret { get; }
		IEnumerable<IPdfPage> Pages { get; }
		long ImageCacheSize { get; set; }
		bool IsLoaded { get; }
		bool IsLoadingFailed { get; }
		bool IsDocumentModified { get; }
		PdfTextSearchResults PerformSearch(TextSearchParameter searchParameter);
		void PerformSelection(PdfDocumentSelectionParameter selectionParameter);
		void Print(PdfPrinterSettings print, int currentPageNumber, bool showPrintStatus, int maxPrintingDpi);
		void SetCurrentPage(int index, bool allowCurrentPageHighlighting);
		string GetText(PdfDocumentPosition start, PdfDocumentPosition end);
		string GetText(PdfDocumentArea area);
		PdfDocumentContent HitTest(PdfDocumentPosition position);
		IPdfDocumentProperties GetDocumentProperties();
		BitmapSource CreateBitmap(int pageIndex, int largestEdgeLength);
		void UpdateDocumentRotateAngle(double rotateAngle);
		void UpdateDocumentSelection();
		IEnumerable<PdfOutlineViewerNode> CreateOutlines();
		IEnumerable<int> CalcPrintPages(IEnumerable<PdfOutlineViewerNode> selectedNodes, bool useAsRange);
		bool CanPrintPages(IEnumerable<PdfOutlineViewerNode> selectedNodes, bool useAsRange);
		void NavigateToOutline(PdfOutlineViewerNode op);
		IPdfDocumentOutlinesViewerProperties GetOutlinesViewerProperties();
		IEnumerable<PdfAttachmentViewModel> CreateAttachments();
	}
	public interface IPdfDocumentOutlinesViewerProperties {
	}
	public interface IPdfDocumentProperties {
		string FileName { get; }
		string Title { get; }
		string Author { get; }
		string Subject { get; }
		string Keywords { get; }
		DateTime? Created { get; }
		DateTime? Modified { get; }
		string Application { get; }
		string Producer { get; }
		string Version { get; }
		string Location { get; }
		long FileSize { get; }
		int NumberOfPages { get; }
	}
	public interface IPdfPage {
		PdfDocumentArea PageArea { get; }
		int PageNumber { get; }
		double UserUnit { get; }
		Size PageSize { get; }
		bool IsSelected { get; }
	}
}
