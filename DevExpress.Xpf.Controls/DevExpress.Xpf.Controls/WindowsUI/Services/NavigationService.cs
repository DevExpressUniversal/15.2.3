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

using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Serialization;
using System.Collections;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public abstract class NavigationServiceBase : DocumentUIServiceBase, INavigationService, IDocumentManagerService, IDocumentOwner {
		EventHandler canGoBackChanged;
		event EventHandler INavigationService.CanGoBackChanged {
			add { canGoBackChanged += value; }
			remove { canGoBackChanged -= value; }
		}
		EventHandler canGoForwardChanged;
		event EventHandler INavigationService.CanGoForwardChanged {
			add { canGoForwardChanged += value; }
			remove { canGoForwardChanged -= value; }
		}
		object INavigationService.Current { get { return ViewHelper.GetViewModelFromView(Current); } }
		EventHandler currentChanged;
		event EventHandler INavigationService.CurrentChanged {
			add { currentChanged += value; }
			remove { currentChanged -= value; }
		}
		JournalEntry savedCurrent;
		void OnNavigationJournalCurrentChanged(object sender, EventArgs e) {
			if(savedCurrent != null)
				savedCurrent.ContentChanged -= OnCurrentContentChanged;
			savedCurrent = NavigationJournal.Current;
			if(savedCurrent != null)
				savedCurrent.ContentChanged += OnCurrentContentChanged;
			OnCurrentContentChanged(null, null);
		}
		void OnCurrentContentChanged(object sender, EventArgs e) {
			if(currentChanged != null)
				currentChanged(this, EventArgs.Empty);
		}
		class NavigationServiceJournal : Journal {
			protected override void OnCanGoBackChanged() {
				base.OnCanGoBackChanged();
				if(Owner.canGoBackChanged != null)
					Owner.canGoBackChanged(Owner, EventArgs.Empty);
			}
			protected override void OnCanGoForwardChanged() {
				base.OnCanGoForwardChanged();
				if(Owner.canGoForwardChanged != null)
					Owner.canGoForwardChanged(Owner, EventArgs.Empty);
			}
			public new bool NavigationInProgress { get { return base.NavigationInProgress; } }
			public NavigationServiceBase Owner { get; set; }
			public bool ForceClose { get; set; }
			protected override JournalEntry CreateJournalEntry(object content, object navigationState) {
				return new NavigationServiceJournalEntry(content);
			}
			public IDocument CreateDocument(object source, object viewModel, object parameter, object parentViewModel) {
				DocumentJournalEntry entry = new DocumentJournalEntry(source, Owner) { Parameter = parameter, ParentViewModel = parentViewModel, ViewModel = viewModel };
				entry.document.documentType = source as string;
				EnsureEntryContent(entry);
				return entry.document;
			}
			public void EnsurePageContent(IDocument document) {
				if(document == null) return;
				DocumentJournalEntry entry = ((DocumentJournalEntry.JournalDocument)document).Entry;
				PageAdornerControl page = entry.Page;
				if(page == null || page.Content != null) return;
				var view = CreateView(entry);
				page.Content = view;
				Owner.InitializeDocumentContainer(page, PageAdornerControl.ContentProperty, Owner.GetDocumentContainerStyle(page, view));
				if(page.Header == null)
					SetTitleBinding(view, PageAdornerControl.HeaderProperty, page);
			}
			void EnsureEntryContent(NavigationServiceJournalEntry entry) {
				if(entry == null || entry.Content != null) return;
				bool isInDocumentManagerMode = entry is DocumentJournalEntry;
				object source = entry.Source ?? entry.ViewModel;
				object page;
				if(!isInDocumentManagerMode && source != null && _Cache.TryGetPageAndClearOnDropHook(source, out page)) {
					ReInitializePage(page, entry);
					entry.SetContent(page);
					return;
				}
				page = source as FrameworkElement ?? CreatePage(entry);
				if(source != null && !isInDocumentManagerMode)
					_Cache.InsertPage(source, page, Navigator, false);
				entry.SetContent(page);
			}
			void ReInitializePage(object page, NavigationServiceJournalEntry entry) {
				ViewHelper.InitializeView(page, entry.ViewModel, entry.Parameter, entry.ParentViewModel, Owner);
			}
			FrameworkElement CreatePage(NavigationServiceJournalEntry entry) {
				DocumentJournalEntry documentJournalEntry = entry as DocumentJournalEntry;
				if(documentJournalEntry == null)
					return CreateView(entry);
				PageAdornerControl page = new PageAdornerControl();
				SetDocument(page, documentJournalEntry.document);
				return page;
			}
			FrameworkElement CreateView(NavigationServiceJournalEntry entry) {
				return (FrameworkElement)Owner.CreateAndInitializeView(entry.Source.With(x => x.ToString()), entry.ViewModel, entry.Parameter, entry.ParentViewModel, Owner);
			}
			protected override bool NavigateCoreOverride(JournalEntry entry, NavigationMode mode, object navigationState = null) {
				AssertionException.IsNotNull(entry);
				Owner.OnNavigating();
				try {
					object currentContent = Owner.GetCurrentContent();
					if(currentContent != null) {
						var viewModel = ViewHelper.GetViewModelFromView(currentContent);
						if(!ForceClose) {
							CancelEventArgs e = new CancelEventArgs();
							DocumentViewModelHelper.OnClose(viewModel, e);
							if(e.Cancel) return false;
						}
						ISupportNavigation pageViewModel = viewModel as ISupportNavigation;
						if(pageViewModel != null)
							pageViewModel.OnNavigatedFrom();
					}
					if(!Navigator.Navigating(entry.Source, mode, navigationState)) {
						Navigator.NavigationStopped(entry.Source, mode, navigationState);
						return false;
					}
					UpdateJournal(entry, mode);
					if(Current != null && !Current.KeepAlive) {
						object currentViewModel = ViewHelper.GetViewModelFromView(currentContent);
						object key = Current.Source ?? (Current as NavigationServiceJournalEntry).Return(x => x.ViewModel, () => null);
						if(key != null)
							_Cache.SetOnDropHook(key, () => DocumentViewModelHelper.OnDestroy(currentViewModel));
						Current.ClearContent();
					}
					Current = _BackStack.Count > 0 ? _BackStack.Peek() : entry as NavigationServiceJournalEntry;
					Current.Do(x => x.SetContent(entry.Content));
					Owner.SetSelectedViewModel(ViewHelper.GetViewModelFromView(Owner.GetCurrentContent()));
					UpdateProperties();
					Navigator.NavigationComplete(Current.With(x => x.Source), Current.With(x => x.Content), navigationState);
				} catch(NavigationException ex) {
					Navigator.NavigationFailed(entry.Source, ex);
					return false;
				} catch(Exception ex) {
					Navigator.NavigationFailed(entry.Source, new NavigationException(entry.Source, ex));
					return false;
				}
				Owner.showNewActiveDocument = false;
				try {
					var newActiveDocument = GetCurrentDocument();
					Owner.SetActiveDocument(newActiveDocument);
					Owner.SetActiveView(((DocumentJournalEntry.JournalDocument)newActiveDocument).With(x => x.Entry.GetView()));
				} finally {
					Owner.showNewActiveDocument = true;
				}
				return true;
			}
			protected override bool SupportNoneJournalEntry { get { return true; } }
			void UpdateJournal(JournalEntry entry, NavigationMode mode) {
				var navigationServiceJournalEntry = entry as NavigationServiceJournalEntry;
				var asJournalEntry = navigationServiceJournalEntry as DocumentJournalEntry;
				Owner.StartNavigation(asJournalEntry == null ? null : asJournalEntry.document);
				EnsureEntryContent(navigationServiceJournalEntry);
				EnsurePageContent(asJournalEntry == null ? null : asJournalEntry.document);
				switch(mode) {
					case NavigationMode.New:
						if(navigationServiceJournalEntry == null)
							throw new InvalidOperationException();
						if(navigationServiceJournalEntry.SaveToHistory)
							_BackStack.Push(navigationServiceJournalEntry);
						_ForwardStack.Clear();
						break;
					case NavigationMode.Back:
						AssertionException.IsNotNull(Current);
						_ForwardStack.Push(Current);
						if(entry is NoneJournalEntry)
							_BackStack.Clear();
						break;
					case NavigationMode.Forward:
						AssertionException.IsNotNull(Current);
						_BackStack.Push(_ForwardStack.Pop());
						break;
				}
			}
			public IDocument GetCurrentDocument() {
				var asJournalEntry = Current as DocumentJournalEntry;
				return asJournalEntry == null ? null : asJournalEntry.document;
			}
			public IEnumerable<IDocument> GetDocuments() {
				return BackStack.Concat(ForwardStack).Select(j => j as DocumentJournalEntry).Where(d => d != null && d.Page != null).Select(j => j.document).ToArray();
			}
			public void Navigate(string viewType, object viewModel, object parameter, object parentViewModel, bool saveToJournal) {
				NavigationServiceJournalEntry entry = new NavigationServiceJournalEntry(viewType) { ViewModel = viewModel, Parameter = parameter, ParentViewModel = parentViewModel, SaveToHistory = saveToJournal };
				NavigateCore(entry, NavigationMode.New, entry.Parameter);
			}
			public void Navigate(string viewType, object parameter, object parentViewModel, bool saveToJournal) {
				Navigate(viewType, null, parameter, parentViewModel, saveToJournal);
			}
			public void Navigate(IDocument document) {
				var currentDocument = GetCurrentDocument() as DocumentJournalEntry;
				if(document == GetCurrentDocument()) return;
				DocumentJournalEntry entry = ((DocumentJournalEntry.JournalDocument)document).Entry;
				NavigateCore(entry, NavigationMode.New, entry.Parameter);
			}
			public void AddBackStackEntry(JournalEntry entry) {
				if(entry == null) return;
				NavigationServiceJournalEntry navEntry = entry as NavigationServiceJournalEntry;
				if(navEntry == null) {
					navEntry = new NavigationServiceJournalEntry(entry.Source) { Parameter = entry.NavigationParameter };
					navEntry.SetContent(entry.Content);
				}
				_BackStack.Push(navEntry);
				if(Current == null) Current = navEntry;
			}
		}
		#region JournalEntry
		internal class NavigationServiceJournalEntry : JournalEntry {
			public NavigationServiceJournalEntry(object source)
				: base(source) {
				SaveToHistory = true;
			}
			public object ViewModel { get; set; }
			public object Parameter { get; set; }
			public object ParentViewModel { get; set; }
			public bool SaveToHistory { get; set; }
		}
		internal class DocumentJournalEntry : NavigationServiceJournalEntry {
			internal class JournalDocument : IDocument, IDocumentInfo {
				internal string documentType;
				public JournalDocument(DocumentJournalEntry entry) {
					Entry = entry;
					State = DocumentState.Hidden;
				}
				public DocumentJournalEntry Entry { get; private set; }
				public object Content {
					get {
						return ViewHelper.GetViewModelFromView(Entry.GetView());
					}
				}
				public object Id { get; set; }
				public object Title {
					get {
						CheckDocumentAccess(Entry.Page != null);
						return Entry.Page.Header;
					}
					set {
						CheckDocumentAccess(Entry.Page != null);
						Entry.Page.Header = value;
					}
				}
				void IDocument.Show() {
					State = DocumentState.Visible;
					Entry.navigationService.Navigate(this);
				}
				void IDocument.Hide() {
					State = DocumentState.Hidden;
					Entry.navigationService.GoBack();
				}
				void IDocument.Close(bool force) {
					if(!Entry.navigationService.IsSelectedDocument(this)) return;
					Entry.navigationService.CloseCurrentPage(force, true);
					State = Entry.DestroyOnClose ? DocumentState.Destroyed : DocumentState.Hidden;
				}
				public DocumentState State { get; internal set; }
				public string DocumentType {
					get { return documentType; }
				}
				public bool DestroyOnClose {
					get { return Entry.DestroyOnClose; }
					set { Entry.DestroyOnClose = value; }
				}
			}
			NavigationServiceBase navigationService;
			bool destroyOnClose = true;
			internal JournalDocument document;
			public DocumentJournalEntry(object source, NavigationServiceBase navigationService)
				: base(source) {
				if(navigationService == null) throw new ArgumentNullException("navigationService");
				this.navigationService = navigationService;
				SaveToHistory = true;
				document = new JournalDocument(this);
			}
			public bool DestroyOnClose {
				get { return destroyOnClose; }
				set {
					destroyOnClose = value;
					KeepAlive = !DestroyOnClose;
				}
			}
			public PageAdornerControl Page { get { return (PageAdornerControl)Content; } }
			public object GetView() {
				CheckDocumentAccess(Page != null);
				navigationService.NavigationJournal.EnsurePageContent(this.document);
				if(Page.Content as DependencyObject != null) {
					DXSerializer.SetEnabled((DependencyObject)Page.Content, false);
				}
				return Page.Content;
			}
		}
		#endregion
		protected IJournal Journal {
			get { return NavigationJournal; }
		}
		readonly NavigationServiceJournal NavigationJournal;
		protected virtual BackNavigationMode BackNavigationMode { get { return WindowsUI.BackNavigationMode.PreviousScreen; } }
		protected bool NavigationInProgress { get { return NavigationJournal.NavigationInProgress; } }
		public NavigationServiceBase() {
			NavigationJournal = new NavigationServiceJournal() { Owner = this };
			NavigationJournal.CurrentChanged += OnNavigationJournalCurrentChanged;
		}
		protected virtual void CheckNavigator() { }
		protected virtual void AddBackStackEntry(JournalEntry entry) {
			NavigationJournal.AddBackStackEntry(entry);
		}
		protected object GetCurrentContent() {
			var currentDocument = Journal.Current as DocumentJournalEntry;
			if(currentDocument != null)
				return currentDocument.Page.With(x => x.Content);
			return Journal.Current.With(x => x.Content);
		}
		protected virtual bool Initialized { get { return true; } }
		#region INavigationService Members
		public bool CanGoBack {
			get { return NavigationJournal.CanGoBack; }
		}
		public bool CanGoForward {
			get { return NavigationJournal.CanGoForward; }
		}
		public bool CanNavigate {
			get { return GetCanNavigate(); }
		}
		protected virtual bool GetCanNavigate() {
			return true;
		}
		public object Current {
			get { return NavigationJournal.With(x => x.Current.With(y => y.Content)); }
		}
		public void GoBack() {
			GoBack(false);
		}
		public void GoBack(object param) {
			if(BackNavigationMode == WindowsUI.BackNavigationMode.PreviousScreen)
				NavigationJournal.GoBack(false, param);
			else
				NavigationJournal.GoHome(param);
		}
		void GoBack(bool allowCloseLastPage) {
			if(BackNavigationMode == WindowsUI.BackNavigationMode.PreviousScreen) NavigationJournal.GoBack(allowCloseLastPage, null);
			else NavigationJournal.GoHome();
		}
		public void GoForward() {
			NavigationJournal.GoForward();
		}
		public void GoForward(object param) {
			NavigationJournal.GoForward(param);
		}
		protected virtual void InvokeNavigation(Action navigationAction) {
#if !SILVERLIGHT
			if(CheckAccess()) {
				navigationAction();
			} else Dispatcher.Invoke(navigationAction);
#else
			navigationAction();
#endif
		}
		public void ClearNavigationHistory() {
			Journal.ClearNavigationHistory();
		}
		public virtual void Navigate(string viewType, object param, object parentViewModel, bool saveToJournal) {
			Navigate(viewType, null, param, parentViewModel, saveToJournal);
		}
		public void Navigate(string viewType, object param = null, object parentViewModel = null) {
			Navigate(viewType, null, param, parentViewModel, true);
		}
		public void Navigate(string viewType, object viewModel, object param, object parentViewModel, bool saveToJournal) {
			Action action = new Action(() => NavigationJournal.Navigate(viewType, viewModel, param, parentViewModel, saveToJournal));
			InvokeNavigation(action);
		}
		protected virtual void Navigate(IDocument document) {
			SetSelectedDocument(document as DocumentJournalEntry.JournalDocument);
			Action action = new Action(() => NavigationJournal.Navigate(document));
			InvokeNavigation(action);
		}
		#endregion
		#region Selected
		DocumentJournalEntry.JournalDocument selectedDocument;
		object selectedEntryViewModel;
		void SetSelectedDocument(DocumentJournalEntry.JournalDocument selectedDocument) {
			this.selectedDocument = selectedDocument;
			this.selectedEntryViewModel = null;
		}
		void SetSelectedViewModel(object selectedEntryViewModel) {
			this.selectedDocument = null;
			this.selectedEntryViewModel = selectedEntryViewModel;
		}
		bool IsSelectedDocument(DocumentJournalEntry.JournalDocument document) {
			return selectedDocument != null
				? selectedDocument == document
				: (document.Entry.Page != null && Equals(selectedEntryViewModel, document.Content));
		}
		bool IsSelectedViewModel(object viewModel) {
			return selectedDocument != null
				? selectedDocument.Entry.Page != null && Equals(selectedDocument.Content, viewModel)
				: Equals(selectedEntryViewModel, viewModel);
		}
		#endregion
		#region IDocumentManagerService
		bool showNewActiveDocument = true;
		IDocument activeDocument;
		public IDocument ActiveDocument {
			get { return activeDocument; }
			set { SetActiveDocument(value); }
		}
		protected virtual void SetActiveDocument(IDocument newActiveDocument) {
			IDocument oldActiveDocument = activeDocument;
			activeDocument = newActiveDocument;
			if(oldActiveDocument != newActiveDocument)
				OnActiveDocumentChanged(oldActiveDocument, newActiveDocument);
		}
		protected virtual void OnActiveDocumentChanged(IDocument oldValue, IDocument newValue) {
			if(showNewActiveDocument && newValue != null)
				newValue.Show();
			if(activeDocumentChanged != null)
				activeDocumentChanged(this, new ActiveDocumentChangedEventArgs(oldValue, newValue));
		}
		protected virtual void SetActiveView(object newActiveView) { }
		ActiveDocumentChangedEventHandler activeDocumentChanged;
		event ActiveDocumentChangedEventHandler IDocumentManagerService.ActiveDocumentChanged {
			add { activeDocumentChanged += value; }
			remove { activeDocumentChanged -= value; }
		}
		IDocument IDocumentManagerService.CreateDocument(string documentType, object viewModel, object parameter, object parentViewModel) {
			if(!Initialized) return null;
			return NavigationJournal.CreateDocument(documentType, viewModel, parameter, parentViewModel);
		}
		public IEnumerable<IDocument> Documents {
			get {
				return NavigationJournal.GetDocuments();
			}
		}
		protected virtual Style GetDocumentContainerStyle(DependencyObject documentContainer, object view) {
			return null;
		}
		#endregion
		protected bool IsContentEquals(IDocument document) {
			var journalEntry = document as JournalEntry;
			object current = NavigationJournal.With(x => x.Current.With(y => y.Content));
			bool contentEquals = journalEntry != null && current != null && journalEntry.Content == current;
			return contentEquals;
		}
		void IDocumentOwner.Close(IDocumentContent documentContent, bool force) {
			if(!IsSelectedViewModel(documentContent)) return;
			CloseCurrentPage(force, true);
		}
		void CloseCurrentPage(bool force, bool allowCloseLastPage) {
			NavigationJournal.ForceClose = force;
			try {
				GoBack(allowCloseLastPage);
			} finally {
				NavigationJournal.ForceClose = false;
			}
		}
		protected internal virtual void StartNavigation(IDocument document) { }
		protected internal virtual void OnNavigating() {
			CheckNavigator();
		}
		public void ClearCache() {
			Journal.ClearNavigationCache();
		}
	}
	[DevExpress.Mvvm.UI.Interactivity.TargetTypeAttribute(typeof(NavigationFrame))]
	public class FrameNavigationService : NavigationServiceBase {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FrameNavigationServiceProperty = DependencyProperty.RegisterAttached("FrameNavigationService", typeof(FrameNavigationService), typeof(FrameNavigationService), null);
		public static readonly DependencyProperty SplashScreenServiceProperty =
			DependencyProperty.Register("SplashScreenService", typeof(ISplashScreenService), typeof(FrameNavigationService), new PropertyMetadata(null));
		public static readonly DependencyProperty ShowSplashScreenProperty =
			DependencyProperty.Register("ShowSplashScreen", typeof(bool), typeof(FrameNavigationService), new PropertyMetadata(true));
		public static readonly DependencyProperty FrameProperty = DependencyProperty.Register("Frame",
			typeof(NavigationFrame), typeof(FrameNavigationService), new PropertyMetadata(null, new PropertyChangedCallback(OnFrameChanged)));
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(string), typeof(FrameNavigationService), new PropertyMetadata(null,
				(dObj, e) => ((FrameNavigationService)dObj).OnSourceChanged(e.OldValue as string, e.NewValue as string)));
		public static readonly DependencyProperty PrefetchedSourcesProperty = 
			DependencyProperty.Register("PrefetchedSources", typeof(IEnumerable), typeof(FrameNavigationService), new FrameworkPropertyMetadata(null, OnPrefetchedSourcesChanged));
		private static void OnFrameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			FrameNavigationService service = o as FrameNavigationService;
			if(service != null)
				service.OnFrameChanged((NavigationFrame)e.OldValue, (NavigationFrame)e.NewValue);
		}
		static FrameNavigationService GetFrameNavigationService(DependencyObject target) {
			return (FrameNavigationService)target.GetValue(FrameNavigationServiceProperty);
		}
		static void SetFrameNavigationService(DependencyObject target, FrameNavigationService value) {
			target.SetValue(FrameNavigationServiceProperty, value);
		}
		protected internal override void StartNavigation(IDocument document) {
			base.StartNavigation(document);
			if(!IsContentEquals(document))
				TryShowSplashScreen();
		}
		static void OnPrefetchedSourcesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			FrameNavigationService service = o as FrameNavigationService;
			service.OnPrefetchedSourcesChanged((IEnumerable)e.NewValue);
		}
		#endregion
		void DetachFromNavigationFrame(NavigationFrame frame) {
			if(frame == null) return;
			frame.AttachedJournal = null;
			frame.Navigated -= OnFrameNavigated;
			frame.ContentRendered -= OnFrameContentRendered;
			frame.NavigationFailed -= OnFrameNavigationFailed;
			frame.ClearValue(FrameNavigationService.FrameNavigationServiceProperty);
		}
		private void OnActualFrameChanged(NavigationFrame oldValue, NavigationFrame newValue) {
			if(oldValue != null) {
				Journal.Navigator = null;
				DetachFromNavigationFrame(oldValue);
			}
			if(newValue != null) {
				FrameNavigationService attached = FrameNavigationService.GetFrameNavigationService(newValue);
				if(attached != null) {
					attached.DetachFromNavigationFrame(attached.Frame);
					attached.Frame = null;
				}
				AddBackStackEntry(newValue.Journal.Current);
				Journal.Navigator = newValue;
				newValue.AttachedJournal = Journal;
				newValue.Navigated += OnFrameNavigated;
				newValue.NavigationFailed += OnFrameNavigationFailed;
				newValue.ContentRendered += OnFrameContentRendered;
				FrameNavigationService.SetFrameNavigationService(newValue, this);
				if(!string.IsNullOrEmpty(Source))
					newValue.Navigate(Source);
			}
		}
		protected virtual void OnFrameChanged(NavigationFrame oldValue, NavigationFrame newValue) {
			ActualFrame = newValue;
		}
		protected virtual void OnSourceChanged(string oldValue, string newValue) {
			if(!string.IsNullOrEmpty(Source) && Frame != null)
				Frame.Navigate(Source);
		}
		void OnFrameContentRendered(object sender, EventArgs e) {
			object currentContent = GetCurrentContent();
			if(currentContent != null) {
				var viewModel = ViewHelper.GetViewModelFromView(currentContent) as ISupportNavigation;
				if(viewModel != null && notifySupportNavigation)
					viewModel.OnNavigatedTo();
			}
			notifySupportNavigation = false;
			TryHideSplashScreen();
		}
		void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e) {
			TryHideSplashScreen();
		}
		void TryShowSplashScreen() {
			if(ShowSplashScreen)
				GetSplashScreenService().Do(s => s.ShowSplashScreen());
		}
		void TryHideSplashScreen() {
			if(ShowSplashScreen)
				GetSplashScreenService().Do(s => BackgroundHelper.DoWithDispatcher(Dispatcher, new Action(() => s.HideSplashScreen())));
		}
		internal virtual void OnFrameNavigated(object sender, NavigationEventArgs e) { }
		ISplashScreenService GetSplashScreenService() {
			if(SplashScreenService != null)
				return SplashScreenService;
			var client = GetServicesClient();
			if(client == null)
				return null;
			return client.ServiceContainer.GetService<ISplashScreenService>();
		}
		protected override void InvokeNavigation(Action navigationAction) {
			if(CheckAccess()) {
				if(Frame == null) {
#if SILVERLIGHT
					Dispatcher.BeginInvoke(navigationAction);
#else
					Dispatcher.BeginInvoke(navigationAction, System.Windows.Threading.DispatcherPriority.Loaded);
#endif
					return;
				}
			}
			base.InvokeNavigation(navigationAction);
		}
		protected override bool AllowAttachInDesignMode { get { return true; } }
		protected override void OnAttached() {
			base.OnAttached();
			NavigationFrame frame = AssociatedObject as NavigationFrame;
			if(frame != null)
				SetActualFrame(frame);
			else
				base.AssociatedObject.Loaded += OnAssociatedObjectLoaded;
		}
		protected override void OnDetaching() {
			base.AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			base.OnDetaching();
		}
		void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
			base.AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			SetActualFrame(LayoutHelper.FindParentObject<NavigationFrame>(AssociatedObject));
		}
		void SetActualFrame(NavigationFrame frame) {
			if(ActualFrame == null)
				ActualFrame = frame;
		}
		protected override void CheckNavigator() {
			if(Journal.Navigator == null) throw new InvalidOperationException(ExceptionFrameNotSet);
		}
		protected internal override void OnNavigating() {
			base.OnNavigating();
			notifySupportNavigation = true;
		}
		bool notifySupportNavigation;
		internal const string ExceptionFrameNotSet = "Navigation failed because the Frame property has not been set. Try to attach FrameNavigationService directly to NavigationFrame.";
		protected override BackNavigationMode BackNavigationMode {
			get { return ActualFrame != null ? ActualFrame.BackNavigationMode : base.BackNavigationMode; }
		}
		private NavigationFrame _ActualFrame;
		NavigationFrame ActualFrame {
			get { return _ActualFrame; }
			set {
				if(_ActualFrame == value) return;
				var oldValue = _ActualFrame;
				_ActualFrame = value;
				OnActualFrameChanged(oldValue, value);
			}
		}
		public ISplashScreenService SplashScreenService {
			get { return (ISplashScreenService)GetValue(SplashScreenServiceProperty); }
			set { SetValue(SplashScreenServiceProperty, value); }
		}
		public bool ShowSplashScreen {
			get { return (bool)GetValue(ShowSplashScreenProperty); }
			set { SetValue(ShowSplashScreenProperty, value); }
		}
		public NavigationFrame Frame {
			get { return (NavigationFrame)GetValue(FrameProperty); }
			set { SetValue(FrameProperty, value); }
		}
		public string Source {
			get { return (string)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		protected override bool Initialized { get { return ActualFrame != null; } }
		protected override bool GetCanNavigate() {
			return Frame != null;
		}
		public IEnumerable PrefetchedSources {
			get { return (IEnumerable)GetValue(PrefetchedSourcesProperty); }
			set { SetValue(PrefetchedSourcesProperty, value); }
		}
		private void OnPrefetchedSourcesChanged(IEnumerable newValue) {
			Frame.NavigateEnumerable(newValue);
		}
	}
}
