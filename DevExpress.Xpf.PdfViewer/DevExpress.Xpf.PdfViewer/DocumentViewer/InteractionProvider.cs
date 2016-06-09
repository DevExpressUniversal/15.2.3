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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.DocumentViewer.Extensions;
using System.Windows;
using System.IO;
namespace DevExpress.Xpf.PdfViewer {
	public class InteractionProvider : BindableBase, IPdfViewerController {
		DocumentViewerControl documentViewer;
		IPdfViewerValueEditingController valueEditingController;
		IPdfViewerNavigationController navigationController;
		IPdfViewerActionController actionController;
		readonly DispatcherSynchronizationContext context = new DispatcherSynchronizationContext();
		PdfViewerControl Viewer { get { return DocumentViewer as PdfViewerControl; } }
		protected internal PdfPresenterControl DocumentPresenter { get { return Viewer.DocumentPresenter; } }
		public SynchronizationContext SynchronizationContext { get { return context; } }
		public PdfViewerTool ViewerTool { get { return (PdfViewerTool)Viewer.CursorMode; } }
		public bool ReadOnly { get { return Viewer.IsReadOnly; } }
		public IPdfViewerNavigationController NavigationController { get { return navigationController ?? (navigationController = CreateNavigationController()); } }
		public IPdfViewerActionController ActionController { get { return actionController ?? (actionController = CreateActionController()); } }
		public IPdfViewerValueEditingController ValueEditingController { get { return valueEditingController ?? (valueEditingController = CreateValueEditingController()); } }
		protected internal PdfBehaviorProvider BehaviorProvider { get { return (PdfBehaviorProvider)Viewer.ActualBehaviorProvider; } }
		protected internal PdfCommandProvider CommandProvider { get { return (PdfCommandProvider)Viewer.ActualCommandProvider; } }
		protected virtual IPdfViewerActionController CreateActionController() {
			return new PdfViewerActionController(this);
		}
		protected virtual IPdfViewerNavigationController CreateNavigationController() {
			return new PdfViewerNavigationController(this);
		}
		protected virtual IPdfViewerValueEditingController CreateValueEditingController() {
			return new PdfViewerValueEditingController(this);
		}
		protected internal DocumentViewerControl DocumentViewer {
			get {
				return documentViewer;
			}
			set {
				if (Equals(documentViewer, value))
					return;
				UnsubscribeFromEvents();
				documentViewer = value;
				InitializeElementsInternal();
				SubscribeToEvents();
			}
		}
		protected virtual void SubscribeToEvents() {
		}
		protected virtual void InitializeElementsInternal() {
		}
		protected virtual void UnsubscribeFromEvents() {
		}
		public virtual void UpdateSelection() {
			var pdfDocument = (IPdfDocument)DocumentViewer.Document;
			pdfDocument.UpdateDocumentSelection();
			Viewer.UpdateSelection();
		}
		public virtual bool AllowAccessToPublicHyperlink(Uri uri) {
			return Viewer.AllowAccessToPublicHyperlink(uri);
		}
		public bool AllowOpenNewDocument(string documentPath, bool openInNewWindow, PdfTarget target) {
			return Viewer.RaiseRequestOpeningReferencedDocumentSource(documentPath, openInNewWindow, target);
		}
		public void OpenNewDocument(string documentPath, bool openInNewWindow, PdfTarget target) {
			if (openInNewWindow)
				RunExternalProgram(documentPath);
			else
				OpenNewDocument(documentPath, target);
		}
		void OpenNewDocument(string documentPath, PdfTarget target) {
			if (!File.Exists(documentPath)) {
				string documentFilePath = (Viewer.Document as PdfDocumentViewModel).Return(x => x.FilePath, () => string.Empty);
				string directory = Path.GetDirectoryName(documentFilePath);
				string tmpFilePath = Path.Combine(directory, documentPath);
				if (File.Exists(tmpFilePath))
					documentPath = tmpFilePath;
			}
			Viewer.DocumentSource = documentPath;
		}
		public bool OpenAttachment(PdfFileAttachment fileAttachment) {
			return Viewer.OpenAttachmentInternal(fileAttachment);
		}
		public string SaveAttachment(PdfFileAttachment fileAttachment) {
			return Viewer.SaveAttachmentInternal(fileAttachment);
		}
		void RunExternalProgram(string documentPath) {
			try {
				Process.Start(documentPath);
			}
			catch {
			}
		}
		public void ShowPrintDialog() {
			Viewer.Print();
		}
		public void SelectionStarted(PdfDocumentPosition position) {
			Viewer.RaiseEvent(new SelectionEventArgs(position) { RoutedEvent = PdfViewerControl.SelectionStartedEvent });
		}
		public void SelectionEnded(PdfDocumentPosition position) {
			Viewer.RaiseEvent(new SelectionEventArgs(position) { RoutedEvent = PdfViewerControl.SelectionEndedEvent });
		}
		public void SelectionContinued(PdfDocumentPosition position) {
			Viewer.RaiseEvent(new SelectionEventArgs(position) { RoutedEvent = PdfViewerControl.SelectionContinuedEvent });
		}
	}
	public class PdfViewerValueEditingController : IPdfViewerValueEditingController {
		readonly InteractionProvider provider;
		public PdfViewerValueEditingController(InteractionProvider provider) {
			this.provider = provider;
		}
		public void ShowEditor(PdfEditorSettings editorSettings, IPdfViewerValueEditingCallBack editorCallback) {
			provider.DocumentPresenter.Do(x => x.StartEditing(editorSettings, editorCallback));
		}
		public void CloseEditor() {
			provider.DocumentPresenter.Do(x => x.EndEditing());
		}
		public void HideTooltip() {
			provider.DocumentPresenter.Do(x => x.HideTooltip());
		}
		public void ShowTooltip(PdfEditorSettings tooltipSettings) {
			PdfStickyNoteEditSettings stickyNoteSettings = tooltipSettings as PdfStickyNoteEditSettings;
			if (stickyNoteSettings == null)
				return;
			provider.DocumentPresenter.Do(x => x.ShowTooltip(stickyNoteSettings));
		}
		public void CommitEditor() {
		}
	}
	public class PdfTargetScroll : PdfTarget {
		readonly PdfRectangleAlignMode alignMode;
		public PdfRectangleAlignMode AlignMode { get { return alignMode; } }
		public PdfTargetScroll(PdfRectangleAlignMode alignMode, int pageIndex, PdfRectangle bounds)
			: base(PdfTargetMode.XYZ, pageIndex, bounds) {
			this.alignMode = alignMode;
		}
	}
	public class PdfViewerNavigationController : IPdfViewerNavigationController {
		readonly InteractionProvider provider;
		PdfBehaviorProvider BehaviorProvider { get { return provider.BehaviorProvider; } }
		PdfCommandProvider CommandProvider { get { return provider.CommandProvider; } }
		int IPdfViewerNavigationController.CurrentPageNumber { get { return provider.DocumentViewer.CurrentPageNumber; } }
		public PdfViewerNavigationController(InteractionProvider provider) {
			this.provider = provider;
		}
		public PdfPoint GetClientPoint(PdfDocumentPosition endPosition) {
			int pageIndex = endPosition.PageIndex;
			var pageViewModel = provider.DocumentViewer.Document.Pages.ElementAt(pageIndex) as PdfPageViewModel;
			var point = pageViewModel.GetPoint(endPosition.Point, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			Point pagePosition = CalcPagePosition(pageIndex);
			return new PdfPoint(point.X + pagePosition.X, point.Y + pagePosition.Y);
		}
		Point CalcPagePosition(int pageIndex) {
			var positionCalculator = provider.DocumentPresenter.NavigationStrategy.PositionCalculator;
			var pageWrapperIndex = positionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = provider.DocumentPresenter.Pages.ElementAt(pageWrapperIndex);
			double verticalOffset = positionCalculator.GetPageWrapperVerticalOffset(pageWrapperIndex);
			double horizontalOffset = positionCalculator.GetPageWrapperOffset(pageIndex);
			return new Point(horizontalOffset, verticalOffset);
		}
		public void ShowDocumentPosition(PdfTarget target) {
			provider.DocumentPresenter.Do(x => x.ScrollIntoView(target));
		}
		public void ShowRectangleOnPage(PdfRectangleAlignMode alignMode, int pageIndex, PdfRectangle bounds) {
			provider.DocumentPresenter.Do(x => x.ScrollIntoView(new PdfTargetScroll(alignMode, pageIndex, bounds)));
		}
		public void Invalidate(PdfDocumentStateChangedFlags flags, int pageIndex) {
			if (flags.HasFlag(PdfDocumentStateChangedFlags.Selection))
				provider.UpdateSelection();
			if (flags.HasFlag(PdfDocumentStateChangedFlags.Rotate)) {
			}
			if (flags.HasFlag(PdfDocumentStateChangedFlags.Annotation)) {
			}
			if (flags.HasFlag(PdfDocumentStateChangedFlags.None)) {
			}
			provider.DocumentPresenter.Do(x => x.Update());
		}
		public void BringCurrentSelectionPointIntoView() {
			provider.DocumentPresenter.Do(x => x.BringCurrentSelectionPointIntoView());
		}
		public void GoToPreviousPage() {
			CommandProvider.PreviousPageCommand.TryExecute(null);
		}
		public void GoToNextPage() {
			CommandProvider.NextPageCommand.TryExecute(null);
		}
		public void GoToFirstPage() {
			CommandProvider.PaginationCommand.TryExecute(1);
		}
		public void GoToLastPage() {
			CommandProvider.PaginationCommand.TryExecute(provider.DocumentViewer.PageCount);
		}
	}
	public class PdfViewerActionController : IPdfViewerActionController {
		readonly InteractionProvider provider;
		public PdfViewerActionController(InteractionProvider provider) {
			this.provider = provider;
		}
		string IPdfViewerActionController.SaveFileAttachment(PdfFileAttachment attachment) {
			return provider.SaveAttachment(attachment);
		}
		bool IPdfViewerActionController.OpenFileAttachment(PdfFileAttachment attachment) {
			return provider.OpenAttachment(attachment);
		}
		public PdfActionRequestResult UriAccessRequest(Uri uri) {
			return provider.AllowAccessToPublicHyperlink(uri) ? PdfActionRequestResult.Ok : PdfActionRequestResult.Cancel;
		}
		public void RunPrintDialog() {
			provider.ShowPrintDialog();
		}
		public PdfActionRequestResult OpenDocument(string documentPath, bool openInNewWindow, PdfTarget target) {
			if (provider.AllowOpenNewDocument(documentPath, openInNewWindow, target))
				provider.OpenNewDocument(documentPath, openInNewWindow, target);
			return PdfActionRequestResult.Ok;
		}
		public void SelectionContinued(PdfDocumentPosition position) {
			provider.SelectionContinued(position);
		}
		public void SelectionEnded(PdfDocumentPosition position) {
			provider.SelectionEnded(position);
		}
		public void SelectionStarted(PdfDocumentPosition position) {
			provider.SelectionStarted(position);
		}
	}
}
