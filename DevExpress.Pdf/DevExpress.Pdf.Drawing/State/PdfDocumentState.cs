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
using System.Drawing;
using DevExpress.Pdf.Native;
using System.Security;
using DevExpress.Pdf.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
namespace DevExpress.Pdf.Drawing {
	public class PdfDocumentState : PdfDisposableObject {
		[SecuritySafeCritical]
		public static byte[] GetFileIcon(string name) {
			ShellFileInfo fileInfo = new ShellFileInfo();
			ShellInterop.SHGetFileInfo(name, 0x80, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), 0x111);
			Icon icon = (Icon)Icon.FromHandle(fileInfo.hIcon).Clone();
			ShellInterop.DestroyIcon(fileInfo.hIcon);
			using (MemoryStream stream = new MemoryStream()) {
				icon.ToBitmap().Save(stream, ImageFormat.Png);
				return stream.ToArray();
			}
		}
		readonly PdfDocument document;
		readonly PdfFontStorage fontStorage;
		readonly PdfImageDataStorage imageDataStorage;
		readonly PdfSelectionState selectionState;
		readonly Dictionary<int, PdfPageState> pageStates = new Dictionary<int, PdfPageState>();
		readonly IDictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder> formDataControllers = new Dictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder>();
		readonly PdfFormData formData;
		readonly List<byte[]> attachmentsViewerNodeIcons = new List<byte[]>();
		readonly Dictionary<string, byte[]> iconsDictionary = new Dictionary<string, byte[]>();
		IList<PdfAttachmentsViewerNode> attachmentsViewerNodes;
		ObservableCollection<PdfOutlineViewerNode> outlineViewerNodes;
		PdfFontSearch fontSearch;
		int rotationAngle;
		bool highlightFormFields;
		bool widgetAppearancesAreBuilt;
		Color highlightedFormFieldColor = Color.FromArgb(205, 215, 255);
		public PdfDocument Document { get { return document; } }
		public PdfFontStorage FontStorage { get { return fontStorage; } }
		public PdfFontSearch FontSearch { get { return fontSearch ?? (fontSearch = new PdfFontSearch()); } }
		public PdfImageDataStorage ImageDataStorage { get { return imageDataStorage; } }
		public PdfSelectionState SelectionState { get { return selectionState; } }
		public IList<byte[]> AttachmentsViewerNodesIcons {
			get {
				return attachmentsViewerNodeIcons;
			}
		}
		public IList<PdfAttachmentsViewerNode> AttachmentsViewerNodes {
			get {
				if (attachmentsViewerNodes == null)
					attachmentsViewerNodes = CreateAttachmentsViewerNodes();
				return attachmentsViewerNodes;
			}
		}
		public IList<PdfOutlineViewerNode> OutlineViewerNodes {
			get {
				if (outlineViewerNodes == null)
					outlineViewerNodes = PdfOutlineViewerNode.CreateTree(document);
				return outlineViewerNodes;
			}
		}
		public IDictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder> FormDataControllers { get { return formDataControllers; } }
		public PdfFormData FormData { get { return formData; } }
		public int RotationAngle {
			get { return rotationAngle; }
			set {
				rotationAngle = value;
				RaiseDocumentStateChanged(new PdfDocumentStateChangedEventArgs(PdfDocumentStateChangedFlags.Rotate));
			}
		}
		public bool HighlightFormFields {
			get { return highlightFormFields; }
			set {
				highlightFormFields = value;
				RaiseDocumentStateChanged(new PdfDocumentStateChangedEventArgs(PdfDocumentStateChangedFlags.Annotation));
			}
		}
		public Color HighlightedFormFieldColor {
			get { return highlightedFormFieldColor; }
			set {
				highlightedFormFieldColor = value;
				RaiseDocumentStateChanged(new PdfDocumentStateChangedEventArgs(PdfDocumentStateChangedFlags.Annotation));
			}
		}
		public event PdfDocumentStateChangedEventHandler DocumentStateChanged;
		public PdfDocumentState(PdfDocument document, PdfFontStorage fontStorage, long imageDataStorageLimit) {
			this.document = document;
			this.fontStorage = fontStorage;
			imageDataStorage = new PdfImageDataStorage(imageDataStorageLimit);
			selectionState = new PdfSelectionState();
			selectionState.SelectionChanged += OnSelectionChanged;
			formData = PdfWidgetAppearanceBuilder.CreateFormDataWithControllers(this, formDataControllers);
		}
		public PdfPageState GetPageState(int pageIndex) {
			if (pageIndex < 0)
				return null;
			if (!pageStates.ContainsKey(pageIndex)) {
				IList<PdfPage> pdfPageList = document.Pages;
				PdfPageState pageState = new PdfPageState(this, pdfPageList[pageIndex], pageIndex);
				pageStates.Add(pageIndex, pageState);
			}
			return pageIndex >= document.Pages.Count ? null : pageStates[pageIndex];
		}
		public PdfPoint GetPageSize(int pageIndex) {
			PdfPageState pageState = GetPageState(pageIndex);
			return pageState == null ? new PdfPoint() : pageState.Page.GetSize(rotationAngle);
		}
		public bool CanPrintOutlineNodesPages(IEnumerable<PdfOutlineViewerNode> nodes, bool printSection) {
			foreach (PdfOutlineViewerNode node in nodes)
				if (CanPrintNodePages(node.Outline, printSection))
					return true;
			return false;
		}
		public int[] GetOutlineNodesPrintPageNumbers(IEnumerable<PdfOutlineViewerNode> nodes, bool printSection) {
			List<int> pageNumbers = new List<int>();
			foreach (PdfOutlineViewerNode node in nodes) {
				List<PdfOutline> nodeWithChildrens = GetNodeWithChildrens(node.Outline);
				if (!TryGetPrintPageNumbers(pageNumbers, nodeWithChildrens, printSection))
					break;
			}
			pageNumbers.Sort();
			return pageNumbers.ToArray();
		}
		public void RaiseDocumentStateChanged(PdfDocumentStateChangedEventArgs args) {
			if (DocumentStateChanged != null)
				DocumentStateChanged(this, args);
		}
		public void EnsureWidgetAppearances() {
			if (!widgetAppearancesAreBuilt) {
				foreach (PdfWidgetAppearanceBuilder builder in formDataControllers.Values)
					builder.EnsureAppearance();
				widgetAppearancesAreBuilt = true;
			}
		}
		bool CanPrintNodePages(PdfOutline outline, bool printSection) {
			if (GetDestination(outline) != null && (!printSection || GetNextOutlinePageNumber(outline) != 0))
				return true;
			outline = outline.First;
			while (outline != null) {
				if (CanPrintNodePages(outline, printSection))
					return true;
				outline = outline.Next;
			}
			return false;
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			RaiseDocumentStateChanged(new PdfDocumentStateChangedEventArgs(PdfDocumentStateChangedFlags.Selection));
		}
		List<PdfOutline> GetNodeWithChildrens(PdfOutline outline) {
			List<PdfOutline> nodeWithChildrens = new List<PdfOutline>();
			nodeWithChildrens.Add(outline);
			PdfOutline childNode = outline.First;
			while (childNode != null) {
				nodeWithChildrens.AddRange(GetNodeWithChildrens(childNode));
				childNode = childNode.Next;
			}
			return nodeWithChildrens;
		}
		int GetNextOutlinePageNumber(PdfOutline outline) {
			do {
				PdfOutline nextOutline = outline.Next; 
				if (nextOutline != null)
					return GetOutlineViewerNodePageNumber(nextOutline);
				outline = outline.Parent as PdfOutline;
			} while (outline != null);
			return document.Pages.Count + 1;
		}
		bool TryGetPrintPageNumbers(List<int> pageNumbers, IEnumerable<PdfOutline> outlines, bool printSection) {
			foreach (PdfOutline node in outlines) {
				int number = GetOutlineViewerNodePageNumber(node);
				int nextOutlinePageNumber = number;
				if (printSection)
					nextOutlinePageNumber = GetNextOutlinePageNumber(node);
				if (number == 0 || nextOutlinePageNumber == 0) {
					pageNumbers.Clear();
					return false;
				}
				AddPrintPageRange(pageNumbers, GetPrintPageNumbersInInterval(number, nextOutlinePageNumber));
			}
			return true;
		}
		int GetOutlineViewerNodePageNumber(PdfOutline outline) {
			PdfDestination destination = GetDestination(outline);
			return destination != null ? destination.CreateTarget(document.Pages).PageIndex + 1 : 0;
		}
		PdfDestination GetDestination(PdfOutline outline) {
			if (outline != null) {
				if (outline.Destination != null)
					return outline.Destination;
				else {
					PdfGoToAction action = outline.Action as PdfGoToAction;
					if (action != null)
						return action.Destination;
				}
			}
			return null;
		}
		IList<int> GetPrintPageNumbersInInterval(int firstPageNumber, int lastPageNumber) {
			int minPageNumber = Math.Min(firstPageNumber, lastPageNumber);
			int maxPageNumber = Math.Max(firstPageNumber, lastPageNumber);
			List<int> result = new List<int>();
			for (int pageNumber = minPageNumber; pageNumber < maxPageNumber || pageNumber == minPageNumber; pageNumber++)
				result.Add(pageNumber);
			return result;
		}
		void AddPrintPageRange(List<int> pageNumbers, IList<int> pageRange) {
			foreach (int pageNumber in pageRange)
				if (!pageNumbers.Contains(pageNumber))
					pageNumbers.Add(pageNumber);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (fontSearch != null)
					fontSearch.Dispose();
				if (selectionState != null)
					selectionState.SelectionChanged -= OnSelectionChanged;
				foreach (PdfWidgetAppearanceBuilder builder in formDataControllers.Values)
					builder.Dispose();
				formDataControllers.Clear();
			}
		}
		IList<PdfAttachmentsViewerNode> CreateAttachmentsViewerNodes() {
			List<PdfAttachmentsViewerNode> result = new List<PdfAttachmentsViewerNode>();
			foreach (PdfFileAttachment attachment in document.FileAttachments) {
				string ext = Path.GetExtension(attachment.FileName);
				byte[] image;
				int imageIndex;
				if (!iconsDictionary.TryGetValue(ext, out image)) {
					image = GetFileIcon(ext);
					iconsDictionary.Add(ext, image);
					imageIndex = attachmentsViewerNodeIcons.Count;
					attachmentsViewerNodeIcons.Add(image);
				}
				imageIndex = attachmentsViewerNodeIcons.IndexOf(image);
				result.Add(new PdfAttachmentsViewerNode(image, imageIndex, attachment));
			}
			result.Sort();
			return result;
		}
	}
}
