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
using System.ComponentModel;
using System.Windows;
using System.Linq;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public enum NavigationMode {
		New = 0,
		Back = 1,
		Forward = 2,
	}
	public class Journal : IJournal {
		private INavigationContentProvider _NavigationContentProvider;
		private INavigationContentProvider _DefaultContentProvider;
		internal JournalEntryStack _BackStack;
		internal JournalEntryStack _ForwardStack;
		internal CacheQueue _Cache;
		public Journal()
			: this(null) {
		}
		public Journal(INavigationContentProvider contentProvider) {
			NavigationContentProvider = contentProvider;
			_BackStack = new JournalEntryStack();
			_BackStack.CountChanged += OnBackStackCountChanged;
			_ForwardStack = new JournalEntryStack();
			_ForwardStack.CountChanged += OnForwardStackCountChanged;
			_Cache = new CacheQueue();
		}
		void ClearCache() {
			_Cache.Clear();
		}
		public void ClearNavigationHistory() {
			clearHistoryLocker.Reset();
			if(Navigator.Status == NavigationStatus.Executing) {
				clearHistoryLocker.Lock();
			}
			else {
				ClearNavigationHistoryOverride();
			}
		}
		Locker clearHistoryLocker = new Locker();
		protected virtual void ClearNavigationHistoryOverride() {
			_ForwardStack.Clear();
			_BackStack.Clear();
			if(Current != null) _BackStack.Push(Current);
			UpdateProperties();
		}
		NavigationOperation _CurrentNavigation;
		protected bool NavigationInProgress { get; private set; }
		protected virtual bool NavigateCore(JournalEntry entry, NavigationMode mode, object navigationState = null) {
			NavigationInProgress = true;
			try {
				return NavigateCoreOverride(entry, mode, navigationState);
			}
			finally {
				NavigationInProgress = false;
			}
		}
		protected virtual bool NavigateCoreOverride(JournalEntry entry, NavigationMode mode, object navigationState) {
			AssertionException.IsNotNull(NavigationContentProvider);
			AssertionException.IsNotNull(entry);
			AssertionException.IsNotNull(Navigator);
			object actualNavigationState = navigationState ?? entry.NavigationParameter;
			_CurrentNavigation = new NavigationOperation() { TargetEntry = entry, NavigationMode = mode, NavigationState = actualNavigationState };
			try {
				if(Navigator.Navigating(entry.Source, mode, _CurrentNavigation.NavigationState)) {
					if(_Cache.Contains(entry.Source)) {
						ApplyContent(_Cache.GetPage(entry.Source));
					}
					else {
						if(entry.Source != null) {
							_CurrentNavigation.AsyncResult = NavigationContentProvider.BeginLoad(entry.Source, OnContentLoaded, _CurrentNavigation);
						}
						else {
							ApplyContent(null);
						}
					}
					return true;
				}
				else Navigator.NavigationStopped(entry.Source, mode, _CurrentNavigation.NavigationState);
			}
			catch(NavigationException ex) {
				Navigator.NavigationFailed(entry.Source, ex);
			}
			catch(Exception ex) {
				Navigator.NavigationFailed(entry.Source, new NavigationException(entry.Source, ex));
			}
			return false;
		}
		void OnContentLoaded(IAsyncResult result) {
			try {
				LoadResult loadResult = this.NavigationContentProvider.EndLoad(result);
				if(loadResult == null) {
					throw new NavigationException(_CurrentNavigation.TargetEntry.Source);
				}
				object content = loadResult.LoadedContent;
				_Cache.InsertPage(_CurrentNavigation.TargetEntry.Source, content, Navigator, false);
				ApplyContent(content);
				if(clearHistoryLocker) {
					ClearNavigationHistoryOverride();
				}
			}
			catch(NavigationException ex) {
				Navigator.NavigationFailed(Current != null ? Current.Source : null, ex);
			}
			catch(Exception ex) {
				object source = Current != null ? Current.Source : _CurrentNavigation.TargetEntry.Source;
				Navigator.NavigationFailed(source, new NavigationException(source, ex));
			}
			finally {
				clearHistoryLocker.Unlock();
			}
		}
		void ApplyContent(object content) {
			JournalEntry entry = _CurrentNavigation.TargetEntry;
			switch(_CurrentNavigation.NavigationMode) {
				case NavigationMode.New:
					if(_BackStack.Count > 0) {
						var lastEntry = _BackStack.Peek();
						if(lastEntry != null && lastEntry.Content == null) _BackStack.Pop();
					}
					_BackStack.Push(entry);
					_ForwardStack.Clear();
					break;
				case NavigationMode.Back:
					AssertionException.IsNotNull(Current);
					if(Current.Content != null)
						_ForwardStack.Push(Current);
					break;
				case NavigationMode.Forward:
					AssertionException.IsNotNull(Current);
					if(Current.Content != null)
						_BackStack.Push(_ForwardStack.Pop());
					break;
			}
			if(Current != null && !Current.KeepAlive) Current.ClearContent();
			Current = _BackStack.Peek();
			Current.SetContent(content);
			UpdateProperties();
			Navigator.NavigationComplete(Current.Source, Current.Content, _CurrentNavigation.NavigationState);
		}
		protected virtual void UpdateProperties() {
			OnPropertyChanged("CanGoBack");
			OnPropertyChanged("CanGoForward");
		}
		#region IJournal Members
		JournalEntry current;
		public JournalEntry Current {
			get { return current; }
			protected set {
				current = value;
				if(CurrentChanged != null)
					CurrentChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler CurrentChanged;
		public IEnumerable<JournalEntry> BackStack { get { return _BackStack; } }
		public IEnumerable<JournalEntry> ForwardStack { get { return _ForwardStack; } }
		public bool CanGoBack { get { return _BackStack.Count > 1; } }
		void OnBackStackCountChanged(object sender, JournalEntryStackCountChangedEventArgs e) {
			if(e.OldValue == 1 && _BackStack.Count == 2 || e.OldValue == 2 && _BackStack.Count == 1)
				OnCanGoBackChanged();
		}
		protected virtual void OnCanGoBackChanged() {
			if(CanGoBackChanged != null)
				CanGoBackChanged(this, EventArgs.Empty);
		}
		public event EventHandler CanGoBackChanged;
		public bool CanGoForward { get { return _ForwardStack.Count > 0; } }
		public event EventHandler CanGoForwardChanged;
		void OnForwardStackCountChanged(object sender, JournalEntryStackCountChangedEventArgs e) {
			if(e.OldValue == 0 && _BackStack.Count == 1 || e.OldValue == 1 && _BackStack.Count == 0)
				OnCanGoForwardChanged();
		}
		protected virtual void OnCanGoForwardChanged() {
			if(CanGoForwardChanged != null)
				CanGoForwardChanged(this, EventArgs.Empty);
		}
		public void GoHome() {
			GoHome(null);
		}
		public void GoHome(object param) {
			if(_BackStack.Count > 1) {
				JournalEntryStack tmpBackStack = new JournalEntryStack();
				JournalEntry previous = null;
				while(CanGoBack) {
					tmpBackStack.Push(_BackStack.Pop());
				}
				previous = _BackStack.Peek();
				if(previous != null) {
					if(!NavigateCore(previous, NavigationMode.Back, param)) {
						while(tmpBackStack.Count > 0) {
							_BackStack.Push(tmpBackStack.Pop());
						}
					}
				}
			}
		}
		public void GoBack() {
			GoBack(false, null);
		}
		public void GoBack(object param) {
			GoBack(false, param);
		}
		internal void GoBack(bool closeLastPage, object param) {
			if(_BackStack.Count == 0 || (!SupportNoneJournalEntry || !closeLastPage) && _BackStack.Count == 1)
				return;
			JournalEntry current = _BackStack.Pop();
			JournalEntry previous = _BackStack.Count != 0 ? _BackStack.Peek() : new NoneJournalEntry();
			AssertionException.IsNotNull(previous);
			if(!NavigateCore(previous, NavigationMode.Back, param)) {
				_BackStack.Push(current);
			}
		}
		public void GoForward() {
			GoForward(null);
		}
		internal void GoForward(object param) {
			if(_ForwardStack.Count > 0) {
				JournalEntry next = _ForwardStack.Peek();
				AssertionException.IsNotNull(next);
				NavigateCore(next, NavigationMode.Forward, param);
			}
		}
		protected virtual bool SupportNoneJournalEntry { get { return false; } }
		protected virtual JournalEntry CreateJournalEntry(object content, object navigationState) {
			return new JournalEntry(content, navigationState) { KeepAlive = Navigator.NavigationCacheMode != NavigationCacheMode.Disabled };
		}
		public void Navigate(object content, object navigationState = null) {
			JournalEntry entry = CreateJournalEntry(content, navigationState);
			NavigateCore(entry, NavigationMode.New, navigationState);
		}
		protected virtual INavigationContentProvider ResolveContentProvider() {
			return new NavigationContentProvider();
		}
		public INavigationContentProvider DefaultContentProvider {
			get {
				if(_DefaultContentProvider == null) _DefaultContentProvider = ResolveContentProvider();
				return _DefaultContentProvider;
			}
		}
		public INavigationContentProvider NavigationContentProvider {
			get { return _NavigationContentProvider ?? DefaultContentProvider; }
			set { _NavigationContentProvider = value; }
		}
		public INavigationFrame Navigator { get; set; }
		public void ClearNavigationCache() {
			ClearCache();
		}
		public void PushInForwardStack(object obj) {
			if(obj != null)
				_ForwardStack.Push(CreateJournalEntry(obj, null));
		}
		#endregion
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
		#region internal
		internal class NavigationOperation {
			public IAsyncResult AsyncResult { get; set; }
			public JournalEntry TargetEntry { get; set; }
			public NavigationMode NavigationMode { get; set; }
			public object NavigationState { get; set; }
		}
		protected class NoneJournalEntry : JournalEntry {
			protected internal override void SetContent(object content) {
				throw new NotSupportedException();
			}
		}
		internal class CacheQueue : DependencyObject {
			#region Dependency Properties
			public static readonly DependencyProperty MaxSizeProperty =
				DependencyProperty.Register("MaxSize", typeof(int), typeof(CacheQueue), new PropertyMetadata(3));
			public int MaxSize {
				get { return (int)GetValue(MaxSizeProperty); }
				set { SetValue(MaxSizeProperty, value); }
			}
			#endregion
#if DEBUGTEST
			internal int Count {
				get { return pages.Keys.Count; }
			}
#endif
			Dictionary<object, CacheEntry> pages = new Dictionary<object, CacheEntry>();
			List<object> orderedKeys = new List<object>();
			object locker = new object();
			public bool Contains(object key) {
				return key != null ? pages.ContainsKey(key) : false;
			}
			public object GetPage(object key) {
				lock(locker) {
					if(orderedKeys.Count > 1) {
						int index = orderedKeys.IndexOf(key);
						orderedKeys.RemoveAt(index);
						orderedKeys.Add(key);
					}
					return pages[key].Page;
				}
			}
			public void InsertPage(object key, object page, INavigationFrame navigator, bool force = false) {
				lock(locker) {
					bool alreadyIn = pages.ContainsKey(key);
					if(alreadyIn) {
						orderedKeys.Remove(key);
						orderedKeys.Add(key);
					}
					else {
						NavigationCacheMode effectiveMode = force ? NavigationCacheMode.Required : GetVirtualCacheMode(page, navigator);
						if(effectiveMode == NavigationCacheMode.Disabled) return;
						foreach(var e in orderedKeys.Select(o => new { key = o, entry = pages[o] }).Where(p => p.entry.CacheMode == NavigationCacheMode.Enabled).Take(orderedKeys.Count - MaxSize + 1).ToArray())
							DropPage(e.key, e.entry);
						if(effectiveMode == NavigationCacheMode.Required || orderedKeys.Count < MaxSize) {
							pages[key] = new CacheEntry(page, effectiveMode);
							orderedKeys.Add(key);
						}
					}
				}
			}
			public void RemovePage(object key) {
				lock(locker) {
					bool alreadyIn = pages.ContainsKey(key);
					if(!alreadyIn) return;
					orderedKeys.Remove(key);
					pages.Remove(key);
				}
			}
			public void SetOnDropHook(object key, Action onDrop) {
				lock(locker) {
					CacheEntry entry;
					bool alreadyIn = pages.TryGetValue(key, out entry);
					if(!alreadyIn) {
						onDrop();
						return;
					}
					if(entry.CacheMode != NavigationCacheMode.Required)
						entry.OnDrop = onDrop;
				}
			}
			public bool TryGetPageAndClearOnDropHook(object key, out object page) {
				lock(locker) {
					CacheEntry entry;
					if(key == null || !pages.TryGetValue(key, out entry)) {
						page = null;
						return false;
					}
					entry.OnDrop = null;
					page = entry.Page;
					return true;
				}
			}
			void DropPage(object key, CacheEntry entry) {
				orderedKeys.Remove(key);
				pages.Remove(key);
				if(entry.OnDrop != null)
					entry.OnDrop();
			}
			NavigationCacheMode GetVirtualCacheMode(object page, INavigationFrame navigator) {
				DependencyObject dep = page as DependencyObject;
				if(dep != null) {
					object value = dep.ReadLocalValue(NavigationPage.NavigationCacheModeProperty);
					return value == DependencyProperty.UnsetValue ? navigator.NavigationCacheMode : (NavigationCacheMode)value;
				}
				return navigator.NavigationCacheMode;
			}
			public void Clear() {
				pages.Clear();
				orderedKeys.Clear();
			}
			class CacheEntry {
				public CacheEntry(object page, NavigationCacheMode cacheMode) {
					Page = page;
					CacheMode = cacheMode;
				}
				public object Page { get; private set; }
				public NavigationCacheMode CacheMode { get; private set; }
				public Action OnDrop { get; set; }
			}
		}
		#endregion
	}
}
