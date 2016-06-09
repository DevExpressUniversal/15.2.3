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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using IDocument = DevExpress.Xpf.DocumentViewer.IDocument;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.Utils;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Threading;
using ReflectionHelper = DevExpress.Xpf.Core.Internal.ReflectionHelper;
using System.Reflection;
using DevExpress.Xpf.DocumentViewer.Internal;
namespace DevExpress.Xpf.PdfViewer {
	public class RequestPasswordEventArgs : EventArgs {
		public bool Handled { get; set; }
		public SecureString Password { get; set; }
		public int TryNumber { get; private set; }
		public string Path { get; private set; }
		public RequestPasswordEventArgs(string path, int tryNumber) {
			Path = path;
			TryNumber = tryNumber;
		}
	}
	public class DocumentProgressChangedEventArgs : EventArgs {
		public bool IsCompleted { get; private set; }
		public long TotalProgress { get; private set; }
		public long Progress { get; private set; }
		public DocumentProgressChangedEventArgs(bool isCompleted, long totalProgress, long progress) {
			IsCompleted = isCompleted;
			TotalProgress = totalProgress;
			Progress = progress;
		}
	}
	public class UpdateRequiredEventArgs : EventArgs {
		public bool ShouldScrollIntoView { get; private set; }
		public ScrollIntoViewMode ScrollMode { get; private set; }
		public bool InvalidateRender { get; private set; }
		public int InvalidatePage { get; private set; }
		public PdfTarget ScrollIntoViewTarget { get; private set; }
		public UpdateRequiredEventArgs(int invalidatePage = -1) {
			InvalidatePage = invalidatePage;
			InvalidateRender = true;
		}
		public UpdateRequiredEventArgs(PdfTarget target) {
			ShouldScrollIntoView = true;
			ScrollIntoViewTarget = target;
		}
		public UpdateRequiredEventArgs(bool shouldScrollIntoView, ScrollIntoViewMode scrollMode) {
			ShouldScrollIntoView = shouldScrollIntoView;
			ScrollMode = scrollMode;
		}
		public UpdateRequiredEventArgs(bool shouldScrollIntoView) : this(shouldScrollIntoView, ScrollIntoViewMode.TopLeft) { }
	}
	public class PdfDocumentViewModel : BindableBase, IDocument, IPdfDocument, IDisposable {
#if DEBUGTEST
		internal static bool IsDebugTest = false;
#endif
		PdfDocument document;
		PdfDocumentState documentState;
		PdfDocumentStateController documentStateController;
		readonly PdfFontStorage fontStorage = new PdfFontStorage();
		long imageCacheSize = PdfImageDataStorage.DefaultLimit;
		IList<PdfPageViewModel> pages;
		IPdfDocumentSelectionResults documentSelection;
		readonly IPdfViewerController viewerController;
		IPagesRenderer pagesRenderer;
		string filePath;
		long fileSize;
		bool isLoaded;
		bool isLoadingFailed;
		bool isDocumentModified;
		bool isCancelled;
		PdfDataSelector DataSelector { get { return documentStateController.DataSelector; } }
		public PdfDocumentStateController DocumentStateController { get { return documentStateController; } }
		public IPdfDocumentSelectionResults SelectionResults {
			get { return documentSelection; }
			private set { SetProperty(ref documentSelection, value, () => SelectionResults); }
		}
		public IList<PdfPageViewModel> Pages {
			get { return pages; }
			private set { SetProperty(ref pages, value, () => Pages); }
		}
		public bool IsLoadingFailed { 
			get { return isLoadingFailed; } 
			private set { SetProperty(ref isLoadingFailed, value, () => IsLoadingFailed); } 
		}
		public bool IsLoaded { 
			get { return isLoaded; } 
			private set { SetProperty(ref isLoaded, value, () => IsLoaded); } 
		}
		public bool IsCancelled {
			get { return isCancelled; }
			private set { SetProperty(ref isCancelled, value, () => IsCancelled); }
		}
		public bool IsDocumentModified {
			get { return isDocumentModified; }
			internal set { SetProperty(ref isDocumentModified, value, () => IsDocumentModified); }
		}
		public void Print(PdfPrinterSettings print, int currentPageNumber, bool showPrintStatus, int maxPrintingDpi) {
			PdfDocumentPrinter.Print(new PdfDocumentState(document, fontStorage, imageCacheSize), filePath, currentPageNumber, print, showPrintStatus);
		}
		public PdfCaret Caret {
			get { return documentState.SelectionState.Caret; }
		}
		public long ImageCacheSize {
			get { return imageCacheSize; }
			set {
				imageCacheSize = value;
				if (documentState != null)
					documentState.ImageDataStorage.UpdateLimit(imageCacheSize);
			}
		}
		public PdfDocumentViewModel(IPdfViewerController viewerController) {
			this.viewerController = viewerController;
		}
		bool IPdfDocument.HasSelection { get { return SelectionResults != null; } }
		public bool HasOutlines { get { return IsLoaded && DocumentState.If(x => x.OutlineViewerNodes != null && x.OutlineViewerNodes.Count > 0).ReturnSuccess(); } }
		public bool HasAttachments { get { return IsLoaded && DocumentState.If(x => x.AttachmentsViewerNodes != null && x.AttachmentsViewerNodes.Count > 0).ReturnSuccess(); } }
		IEnumerable<IPdfPage> IPdfDocument.Pages { get { return Pages; } }
		internal PdfDocument PdfDocument { get { return document; } }
		public string FilePath { get { return filePath; } }
		public long FileSize { get { return fileSize; } }
		Stream FileStream { get; set; }
		CancellationTokenSource cancellationTokenSource { get; set; }
		public PdfDocumentState DocumentState { get { return documentState; } }
		public bool HasInteractiveForm { get { return document != null && document.AcroForm != null; } }
		public virtual IPdfDocumentProperties GetDocumentProperties() {
			return new PdfDocumentProperties(this);
		}
		public virtual IPdfDocumentOutlinesViewerProperties GetOutlinesViewerProperties() {
			return new PdfDocumentOutlinesViewerProperties();
		}
		public virtual BitmapSource CreateBitmap(int pageIndex, int largestEdgeLength) {
			if (largestEdgeLength < 1)
				throw new ArgumentOutOfRangeException("largestEdgeLength");
			PdfDocumentState documentState = new PdfDocumentState(document, fontStorage, imageCacheSize);
			using (System.Drawing.Bitmap bmp = PdfViewerCommandInterpreter.GetBitmap(documentState, pageIndex, largestEdgeLength, PdfRenderMode.Print)) {
				return Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
		}
		void IPdfDocument.PerformSelection(PdfDocumentSelectionParameter selectionParameter) {
			if (selectionParameter == null) {
				DataSelector.ClearSelection();
				return;
			}
			switch (selectionParameter.SelectionAction) {
				case PdfSelectionAction.StartSelection:
					DataSelector.StartSelection(selectionParameter.Position);
					break;
				case PdfSelectionAction.EndSelection:
					DataSelector.EndSelection(selectionParameter.Position);
					break;
				case PdfSelectionAction.ContinueSelection:
					DataSelector.PerformSelection(selectionParameter.Position);
					break;
				case PdfSelectionAction.SelectViaArea:
					DataSelector.Select(selectionParameter.Area);
					break;
				case PdfSelectionAction.ClearSelection:
					DataSelector.ClearSelection();
					break;
				case PdfSelectionAction.SelectAllText:
					DataSelector.SelectAllText();
					break;
				case PdfSelectionAction.SelectWord:
					DataSelector.SelectWord(selectionParameter.Position);
					break;
				case PdfSelectionAction.SelectLine:
					DataSelector.SelectLine(selectionParameter.Position);
					break;
				case PdfSelectionAction.SelectPage:
					DataSelector.SelectPage(selectionParameter.Position);
					break;
				case PdfSelectionAction.SelectImage:
					DataSelector.StartSelection(selectionParameter.Position);
					DataSelector.EndSelection(selectionParameter.Position);
					break;
				case PdfSelectionAction.SelectLeft:
					DataSelector.SelectWithCaret(PdfMovementDirection.Left);
					break;
				case PdfSelectionAction.SelectUp:
					DataSelector.SelectWithCaret(PdfMovementDirection.Up);
					break;
				case PdfSelectionAction.SelectRight:
					DataSelector.SelectWithCaret(PdfMovementDirection.Right);
					break;
				case PdfSelectionAction.SelectDown:
					DataSelector.SelectWithCaret(PdfMovementDirection.Down);
					break;
				case PdfSelectionAction.SelectLineStart:
					DataSelector.SelectWithCaret(PdfMovementDirection.LineStart);
					break;
				case PdfSelectionAction.SelectLineEnd:
					DataSelector.SelectWithCaret(PdfMovementDirection.LineEnd);
					break;
				case PdfSelectionAction.SelectDocumentStart:
					DataSelector.SelectWithCaret(PdfMovementDirection.DocumentStart);
					break;
				case PdfSelectionAction.SelectDocumentEnd:
					DataSelector.SelectWithCaret(PdfMovementDirection.DocumentEnd);
					break;
				case PdfSelectionAction.SelectNextWord:
					DataSelector.SelectWithCaret(PdfMovementDirection.NextWord);
					break;
				case PdfSelectionAction.SelectPreviousWord:
					DataSelector.SelectWithCaret(PdfMovementDirection.PreviousWord);
					break;
				case PdfSelectionAction.MoveLeft:
					DataSelector.MoveCaret(PdfMovementDirection.Left);
					break;
				case PdfSelectionAction.MoveUp:
					DataSelector.MoveCaret(PdfMovementDirection.Up);
					break;
				case PdfSelectionAction.MoveRight:
					DataSelector.MoveCaret(PdfMovementDirection.Right);
					break;
				case PdfSelectionAction.MoveDown:
					DataSelector.MoveCaret(PdfMovementDirection.Down);
					break;
				case PdfSelectionAction.MoveLineStart:
					DataSelector.MoveCaret(PdfMovementDirection.LineStart);
					break;
				case PdfSelectionAction.MoveLineEnd:
					DataSelector.MoveCaret(PdfMovementDirection.LineEnd);
					break;
				case PdfSelectionAction.MoveDocumentStart:
					DataSelector.MoveCaret(PdfMovementDirection.DocumentStart);
					break;
				case PdfSelectionAction.MoveDocumentEnd:
					DataSelector.MoveCaret(PdfMovementDirection.DocumentEnd);
					break;
				case PdfSelectionAction.MoveNextWord:
					DataSelector.MoveCaret(PdfMovementDirection.NextWord);
					break;
				case PdfSelectionAction.MovePreviousWord:
					DataSelector.MoveCaret(PdfMovementDirection.PreviousWord);
					break;
			}
		}
		PdfDocumentContent IPdfDocument.HitTest(PdfDocumentPosition position) {
			return DataSelector.GetContentInfo(position);
		}
		void IPdfDocument.UpdateDocumentSelection() {
			SelectionResults = DocumentState.SelectionState.HasSelection ? new PdfDocumentSelectionResults(this) : null;
		}
		PdfTextSearchResults IPdfDocument.PerformSearch(TextSearchParameter searchParameter) {
			SelectionResults = null;
			PdfTextSearchParameters parameters = new PdfTextSearchParameters {
				CaseSensitive = searchParameter.IsCaseSensitive,
				Direction = (PdfTextSearchDirection)searchParameter.SearchDirection,
				WholeWords = searchParameter.WholeWord
			};
			return documentStateController.FindText(searchParameter.Text, parameters, searchParameter.CurrentPage, null);
		}
		void IPdfDocument.SetCurrentPage(int index, bool allowCurrentPageHighlighting) {
			SetCurrentPageInternal(index, allowCurrentPageHighlighting);
		}
		string IPdfDocument.GetText(PdfDocumentPosition start, PdfDocumentPosition end) {
			return GetTextInternal(start, end);
		}
		string IPdfDocument.GetText(PdfDocumentArea area) {
			return GetTextInternal(area);
		}
		void IPdfDocument.UpdateDocumentRotateAngle(double angle) {
			documentState.RotationAngle = (int)angle;
		}
		protected virtual string GetTextInternal(PdfDocumentArea area) {
			var result = DataSelector.GetTextSelection(area);
			return result.Return(x => x.Text, () => string.Empty);
		}
		protected virtual string GetTextInternal(PdfDocumentPosition start, PdfDocumentPosition end) {
			var result = DataSelector.GetTextSelection(start, end);
			return result.Return(x => x.Text, () => string.Empty);
		}
		protected virtual void SetCurrentPageInternal(int index, bool allowCurrentPageHighlighting) {
			if (!Pages.Return(x => x.Any(), () => false))
				return;
			foreach (var page in Pages)
				page.IsSelected = allowCurrentPageHighlighting && page.PageIndex == index;
		}
		public event EventHandler<RequestPasswordEventArgs> RequestPassword;
		public event EventHandler<DocumentProgressChangedEventArgs> DocumentProgressChanged;
		public void LoadDocument(object source, bool detachStreamOnLoadComplete) {
#if DEBUGTEST
			if (IsDebugTest) {
				LoadDocumentSync(source, detachStreamOnLoadComplete);
				DocumentProgressChanged.Do(x => x(this, new DocumentProgressChangedEventArgs(true, 0, 0)));
				return;
			}
#endif
			if (!Net45Detector.IsNet45()) {
				LoadDocumentSync(source, detachStreamOnLoadComplete);
				DocumentProgressChanged.Do(x => x(this, new DocumentProgressChangedEventArgs(true, 0, 0)));
				return;
			}
			LoadDocumentAsync(source, detachStreamOnLoadComplete);
		}
		void LoadDocumentAsync(object source, bool detachStreamOnLoadComplete) {
			cancellationTokenSource = new CancellationTokenSource();
			LoadDocumentState loadState = new LoadDocumentState() { Current = new DispatcherSynchronizationContext() };
			Task.Factory.StartNew(() => GetDocumentStream(source))
				.ContinueWith((task, state) => {
					GetPasswordState getPasswordState = new GetPasswordState(GetPassword);
					var context = ((LoadDocumentState)state).Current;
					Func<string, int, SecureString> getPasswordHandler = (path, number) => {
						context.Send(currentState => ((GetPasswordState)currentState).GetPassword(path, number), getPasswordState);
						return getPasswordState.Password;
					};
					LoadDocument(task.Result, source as string, detachStreamOnLoadComplete, !IsUserStream(source), getPasswordHandler);
				}, loadState, cancellationTokenSource.Token, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Default)
					.ContinueWith(task => {
					IsLoadingFailed = task.IsFaulted || task.IsCanceled;
					SynchronizationContext.Current.Post(state => DocumentProgressChanged.Do(x => x(this, new DocumentProgressChangedEventArgs(true, 0, 0))), null);
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		public void LoadDocumentSync(object source, bool detachStreamOnLoadComplete) {
			LoadDocument(GetDocumentStream(source), source as string, detachStreamOnLoadComplete, !IsUserStream(source), GetPassword);
			DocumentProgressChanged.Do(x => x(this, new DocumentProgressChangedEventArgs(true, 0, 0)));
		}
		public void CancelLoadDocument() {
			IsCancelled = true;
			cancellationTokenSource.Do(x => x.Cancel());
			DocumentProgressChanged.Do(x => x(this, new DocumentProgressChangedEventArgs(true, 0, 0)));
		}
		bool IsUserStream(object source) {
			return source is Stream;
		}
		protected virtual Stream GetDocumentStream(object source) {
			Stream documentStream = source as Stream;
			if (source is Uri)
				documentStream = GetDocumentStream(source as Uri);
			else if (source is string)
				documentStream = GetDocumentStream(source as string);
			return documentStream;
		}
		protected virtual Stream GetDocumentStream(string path) {
			return new FileStream(path, FileMode.Open, FileAccess.Read);
		}
		protected virtual Stream GetDocumentStream(Uri source) {
			ResourceLocator resourceLocator = new ResourceLocator(source);
			return resourceLocator.LoadStreamSync();
		}
		protected virtual void LoadDocument(Stream stream, string path, bool detachStreamOnLoadComplete, bool internalDispose, Func<string, int, SecureString> getPasswordAction) {
			Stream correctedStream = ValidateStream(stream);
			this.document = PdfDocumentReader.Read(correctedStream, detachStreamOnLoadComplete, number => getPasswordAction(path, number));
			if (cancellationTokenSource.Return(x => x.IsCancellationRequested, () => false))
				return;
			this.filePath = path;
			this.fileSize = correctedStream.Length;
			if (!detachStreamOnLoadComplete && internalDispose)
				FileStream = stream;
			else if (internalDispose)
				stream.Dispose();
			Initialize();
		}
		SecureString GetPassword(string path, int tryNumber) {
			var result = RaiseRequestPasswordEvent(path, tryNumber);
			if (result.Handled)
				return result.Password;
			return null;
		}
		Stream ValidateStream(Stream stream) {
			if (stream.CanSeek)
				return stream;
			return new MemoryStream(stream.CopyAllBytes());
		}
		void Initialize() {
			if (document != null) {
				ObservableCollection<PdfPageViewModel> result = new ObservableCollection<PdfPageViewModel>();
				int index = 0;
				foreach (PdfPage page in document.Pages)
					result.Add(new PdfPageViewModel(page, fontStorage, index++));
				Pages = result;
				documentState = new PdfDocumentState(document, fontStorage, imageCacheSize);
				documentStateController = new PdfDocumentStateController(viewerController, documentState);
				pagesRenderer = new PdfViewerPagesRenderer();
				IsLoaded = true;
			}
		}
		RequestPasswordEventArgs RaiseRequestPasswordEvent(string path, int tryNumber) {
			var args = new RequestPasswordEventArgs(path, tryNumber);
			if (RequestPassword != null)
				RequestPassword(this, args);
			return args;
		}
		void IDisposable.Dispose() {
			FileStream.Do(x => x.Dispose());
			fontStorage.Dispose();
		}
		IEnumerable<IPage> IDocument.Pages {
			get { return Pages; }
		}
		public virtual void NavigateToOutline(PdfOutlineViewerNode node) {
			if (!IsLoaded)
				return;
			documentStateController.ExecuteInteractiveOperation(node.InteractiveOperation);
		}
		public void ApplyFormData(PdfFormData formData) {
			DocumentState.With(x => x.FormData).Do(x => x.Apply(formData));
		}
		public void SaveFormData(string filePath, PdfFormDataFormat formDataFormat) {
			DocumentState.With(x => x.FormData).Do(x => x.Save(filePath, formDataFormat));
		}
		public PdfFormData GetFormData() {
			return DocumentState.With(x => x.FormData);
		}
		public virtual bool CanPrintPages(IEnumerable<PdfOutlineViewerNode> nodes, bool useAsRange) {
			return IsLoaded && documentState.Return(x => x.CanPrintOutlineNodesPages(nodes, useAsRange), () => false);
		}
		public virtual IEnumerable<int> CalcPrintPages(IEnumerable<PdfOutlineViewerNode> nodes, bool useAsRange) {
			if (documentState == null || !IsLoaded)
				return Enumerable.Empty<int>();
			return documentState.GetOutlineNodesPrintPageNumbers(nodes, useAsRange);
		}
		public virtual IEnumerable<PdfOutlineViewerNode> CreateOutlines() {
			return new ObservableCollection<PdfOutlineViewerNode>(documentState.OutlineViewerNodes);
		}
		public virtual IEnumerable<PdfAttachmentViewModel> CreateAttachments() {
			return new ObservableCollection<PdfAttachmentViewModel>(documentState.Return(x => x.AttachmentsViewerNodes.Select(y => new PdfAttachmentViewModel(y)), () => new List<PdfAttachmentViewModel>()));
		}
	}
	internal static class TaskContinueWithWithStateForNet45 {
		static readonly ReflectionHelper Helper = new ReflectionHelper();
		public static Task ContinueWith<T>(this Task<T> task, Action<Task<T>, object> action, object state, CancellationToken token, TaskContinuationOptions continuation, TaskScheduler scheduler) {
			var handler = Helper.GetInstanceMethodHandler<Func<Task<T>, Action<Task<T>, object>, object, CancellationToken, TaskContinuationOptions, TaskScheduler, Task>>(task, "ContinueWith",
				BindingFlags.Public | BindingFlags.Instance, task.GetType(), parametersCount: 5);
			return handler(task, action, state, token, continuation, scheduler);
		}
	}
	class LoadDocumentState {
		public SynchronizationContext Current { get; set; }
		public object InnerState { get; set; }
	}
	class GetPasswordState {
		readonly Func<string, int, SecureString> getPasswordHandler;
		public GetPasswordState(Func<string, int, SecureString> getPasswordHandler) {
			this.getPasswordHandler = getPasswordHandler;
		}
		public SecureString Password { get; private set; }
		public SecureString GetPassword(string path, int number) {
			Password = getPasswordHandler(path, number);
			return Password;
		}
	}
	public class UriToBitmapImageConverterExtension : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Uri uri = value as Uri;
			return uri == null ? null : new BitmapImage(uri);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
}
