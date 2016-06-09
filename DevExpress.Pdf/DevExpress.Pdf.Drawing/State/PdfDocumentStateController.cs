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
using System.Threading;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfDocumentStateController : PdfDisposableObject {
		const int scrollTimeInterval = 10;
		const int maxPixelsInsideSelectionToClear = 3;
		static void BringCurrentSelectionPointIntoView(object controller) {
			IPdfViewerController viewerController = controller as IPdfViewerController;
			if (viewerController != null)
				viewerController.NavigationController.BringCurrentSelectionPointIntoView();
		}
		static void OnTimerClick(object controller) {
			IPdfViewerController viewerController = controller as IPdfViewerController;
			if (viewerController != null)
				viewerController.SynchronizationContext.Post(BringCurrentSelectionPointIntoView, controller);
		}
		readonly Dictionary<int, IList<PdfAnnotationStateController>> pageStateControllerStorage = new Dictionary<int, IList<PdfAnnotationStateController>>();
		readonly IPdfViewerController viewerController;
		readonly PdfInteractiveOperationController actionController;
		readonly PdfDocumentState documentState;
		readonly PdfDataSelector dataSelector;
		readonly PdfTextSearch documentTextSearch;
		readonly PdfTabNavigationController tabNavigationController;
		Timer scrollTimer;
		PdfAnnotationStateController focusedAnnotation;
		public PdfTextSearch TextSearch { get { return documentTextSearch; } }
		public IPdfViewerController ViewerController { get { return viewerController; } }
		public PdfDocumentState DocumentState { get { return documentState; } }
		public PdfDataSelector DataSelector { get { return dataSelector; } }
		public PdfTabNavigationController TabNavigationController { get { return tabNavigationController; } }
		public bool HasFocus { get { return focusedAnnotation != null; } }
		public bool IsDocumentModified {
			get { return ContainsAcroForm && documentState.Document.AcroForm.Modified; }
			set {
				if (ContainsAcroForm)
					documentState.Document.AcroForm.Modified = value;
			}
		}
		public PdfAnnotationStateController FocusedAnnotation {
			get { return focusedAnnotation; }
			set {
				if (focusedAnnotation != null)
					focusedAnnotation.SetFocus(false);
				focusedAnnotation = value;
				if (focusedAnnotation != null)
					focusedAnnotation.SetFocus(true);
			}
		}
		bool ContainsAcroForm { get { return documentState != null && documentState.Document != null && documentState.Document.AcroForm != null; } }
		public PdfDocumentStateController(IPdfViewerController viewerController, PdfDocumentState documentState) {
			this.viewerController = viewerController;
			this.documentState = documentState;
			this.actionController = new PdfInteractiveOperationController(this);
			dataSelector = new PdfDataSelector(viewerController.NavigationController, documentState);
			documentTextSearch = new PdfTextSearch(documentState.Document.Pages);
			tabNavigationController = new PdfTabNavigationController(this);
			documentState.DocumentStateChanged += OnDocumentStateChanged;
		}
		public PdfTextSearchResults FindText(string text, PdfTextSearchParameters parameters, int currentPageNumber, Func<int, bool> terminate) {
			PdfTextSearchResults results = documentTextSearch.Find(text, parameters ?? new PdfTextSearchParameters(), currentPageNumber, terminate);
			if (results.Status == PdfTextSearchStatus.Found) {
				int pageIndex = results.PageNumber - 1;
				viewerController.NavigationController.ShowRectangleOnPage(PdfRectangleAlignMode.Center, pageIndex, results.BoundingRectangle);
				PdfWord lastWord = results.Words[results.Words.Count - 1];
				PdfTextSelector textSelector = new PdfTextSelector(viewerController.NavigationController, documentTextSearch.Cache, documentState);
				textSelector.SelectText(new PdfPageTextRange(pageIndex, results.Words[0].WordNumber, 0, lastWord.WordNumber, lastWord.Characters.Count));
				dataSelector.HideCaret();
			}
			else
				dataSelector.ClearSelection();
			return results;
		}
		public IList<PdfAnnotationStateController> GetAnnotationStateControllers(int pageIndex) {
			IList<PdfAnnotationStateController> controllers;
			if (pageStateControllerStorage.TryGetValue(pageIndex, out controllers))
				return controllers;
			PdfPageState pageState = documentState.GetPageState(pageIndex);
			controllers = new List<PdfAnnotationStateController>();
			if (pageState == null)
				return controllers;
			foreach (PdfAnnotationState state in pageState.AnnotationStates) {
				PdfAnnotationStateController annotationStateController = PdfAnnotationStateController.Create(this, state, documentState.FormDataControllers);
				if (annotationStateController != null)
					controllers.Add(annotationStateController);
			}
			pageStateControllerStorage.Add(pageIndex, controllers);
			return controllers;
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
#endif
		public void MouseMove(PdfMouseAction mouseAction) {
			PdfDocumentPosition documentPosition = mouseAction.DocumentPosition;
			if (viewerController.ViewerTool != PdfViewerTool.MarqueeZoom)
				foreach (PdfAnnotationStateController annotationController in GetAnnotationStateControllers(documentPosition.PageIndex))
					annotationController.MouseMove(mouseAction);
			if (ShouldPerformSelection(mouseAction)) {
				dataSelector.PerformSelection(documentPosition);
				viewerController.ActionController.SelectionContinued(documentPosition);
			}
			if (mouseAction.Button == PdfMouseButton.Left && dataSelector.SelectionInProgress && mouseAction.IsOutsideOfView) {
				if (scrollTimer == null)
					scrollTimer = new Timer(OnTimerClick, viewerController, scrollTimeInterval, scrollTimeInterval);
			}
			else
				StopTimer();
		}
		public void MouseDown(PdfMouseAction mouseAction) {
			StopTimer();
			PdfDocumentPosition documentPosition = mouseAction.DocumentPosition;
			if (mouseAction.Button == PdfMouseButton.Left) {
				PdfViewerTool viewerTool = viewerController.ViewerTool;
				PdfAnnotationStateController focusedAnnotation = null;
				if (viewerTool != PdfViewerTool.MarqueeZoom) {
					foreach (PdfAnnotationStateController controller in GetAnnotationStateControllers(documentPosition.PageIndex))
						if (controller.ContainsActionPoint(mouseAction)) {
							dataSelector.ClearSelection();
							dataSelector.HideCaret();
							PdfAnnotationState annotationState = controller.State;
							if (!annotationState.Annotation.Flags.HasFlag(PdfAnnotationFlags.ReadOnly)) {
								focusedAnnotation = controller;
								controller.MouseDown(mouseAction);
							}
						}
				}
				FocusedAnnotation = focusedAnnotation;
				if (focusedAnnotation == null && viewerTool == PdfViewerTool.Selection) {
					dataSelector.StartSelection(mouseAction);
					viewerController.ActionController.SelectionStarted(documentPosition);
				}
			}
			else if (!dataSelector.GetContentInfoWhileSelecting(documentPosition).IsSelected)
				dataSelector.ClearSelection();
		}
		public void MouseUp(PdfMouseAction mouseAction) {
			StopTimer();
			PdfDocumentPosition documentPosition = mouseAction.DocumentPosition;
			tabNavigationController.LastPageIndex = documentPosition.PageIndex;
			if (mouseAction.Button == PdfMouseButton.Left) {
				PdfAnnotationStateController focusedAnnotation = FocusedAnnotation;
				if (focusedAnnotation != null && focusedAnnotation.ContainsActionPoint(mouseAction))
					focusedAnnotation.MouseUp(mouseAction);
			}
			if (ShouldPerformSelection(mouseAction)) {
				dataSelector.EndSelection(mouseAction);
				viewerController.ActionController.SelectionEnded(documentPosition);
			}
		}
		public void TabForward() {
			tabNavigationController.TabForward();
		}
		public void TabBackward() {
			tabNavigationController.TabBackward();
		}
		public void SubmitFocus() {
			if (focusedAnnotation != null)
				focusedAnnotation.ExecuteEnterAction();
		}
		public void CommitCurrentEditor() {
			viewerController.ValueEditingController.CommitEditor();
		}
		public void ExecuteInteractiveOperation(PdfInteractiveOperation interactiveOperation) {
			actionController.ExecuteInteractiveOperation(interactiveOperation);
		}
		public void OpenFileAttachment(PdfFileAttachment attachment) {
			if (viewerController.ActionController.OpenFileAttachment(attachment)) {
				string fileName = attachment.FileName;
				string path = Path.Combine(PdfTempFolder.Create() , String.IsNullOrEmpty(fileName) ? Path.GetRandomFileName() : fileName);
				SaveFileAttachement(path, attachment);
				PdfProcessorLauncher.Launch(path);
			}
		}
		public void SaveFileAttachment(PdfFileAttachment attachment) {
			string path = viewerController.ActionController.SaveFileAttachment(attachment);
			if (!String.IsNullOrEmpty(path))
				SaveFileAttachement(path, attachment);
		}
		void SaveFileAttachement(string path, PdfFileAttachment attachment) {
			using (Stream file = File.Create(path)) {
				byte[] data = attachment.Data;
				if (data != null)
					file.Write(data, 0, data.Length);
			}
		}
		bool ShouldPerformSelection(PdfMouseAction mouseAction) {
			return FocusedAnnotation == null && mouseAction.Button == PdfMouseButton.Left &&
				(mouseAction.Clicks <= 1 || dataSelector.GetContentInfoWhileSelecting(mouseAction.DocumentPosition).ContentType == PdfDocumentContentType.Image);
		}
		void StopTimer() {
			if (scrollTimer != null)
				scrollTimer.Dispose();
			scrollTimer = null;
		}
		void OnDocumentStateChanged(object sender, PdfDocumentStateChangedEventArgs e) {
			viewerController.NavigationController.Invalidate(e.Flags, e.PageIndex);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				StopTimer();
				documentState.DocumentStateChanged -= OnDocumentStateChanged;
			}
		}
	}
}
