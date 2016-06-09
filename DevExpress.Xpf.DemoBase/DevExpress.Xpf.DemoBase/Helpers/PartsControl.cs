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
using System.Threading;
using System.Windows;
using DevExpress.DemoData.Utils;
using System.Linq;
using DevExpress.Mvvm;
using System.Windows.Controls;
namespace DevExpress.DemoData.Helpers {
	public class DataLoadEventArgs<TData> : EventArgs {
		public TData Data { get; set; }
	}
	public abstract class PartsControlPart : ViewModelBase {
#if DEBUG
		static int nextID = 0;
#endif
		Action onDataLoaded;
		ManualResetEvent initialized = new ManualResetEvent(false);
		public PartsControlPart(PartsControl partsControl) {
#if DEBUG
			ID = ++nextID;
#endif
			PartsControl = partsControl;
			View = PartsControl.CreateView(this);
		}
#if DEBUG
		public int ID { get; private set; }
#endif
		public PartsControl PartsControl {
			get { return GetProperty(() => PartsControl); }
			set { SetProperty(() => PartsControl, value); }
		}
		public PartsControlPartView View {
			get { return GetProperty(() => View); }
			set { SetProperty(() => View, value); }
		}
		public void BeginInBackgroundThread(Action action) {
			BeginInBackgroundThread(action, 200);
		}
		public void BeginInBackgroundThread(Action action, int delayInMillisecons) {
			Thread thread = new Thread(() => {
				this.initialized.WaitOne();
				Thread.Sleep(delayInMillisecons);
				if(action != null)
					action();
			});
			thread.IsBackground = true;
			thread.Start();
		}
		public void BeginInMainThread(Action action) {
			PartsControl.Dispatcher.BeginInvoke(action);
		}
		public void DoInBackgroundThread(Action action, Action completed) {
			DoInBackgroundThread(action, completed, 200);
		}
		public void DoInBackgroundThread(Action action, Action completed, int delayInMillisecons) {
			BeginInBackgroundThread(() => {
				if(action != null)
					action();
				if(completed != null)
					BeginInMainThread(completed);
			}, delayInMillisecons);
		}
		public void DoInMainThread(Action action) {
			if(PartsControl.Dispatcher.CheckAccess()) {
				if(action != null)
					action();
			} else {
				AutoResetEvent done = new AutoResetEvent(false);
				BeginInMainThread(() => {
					if(action != null)
						action();
					done.Set();
				});
				done.WaitOne();
			}
		}
		public virtual void LoadData(Action onLoaded) {
			this.onDataLoaded = onLoaded;
			BeginInBackgroundThread(BeginLoadData, 0);
		}
		protected virtual void BeginLoadData() {
			BeginInMainThread(EndLoadData);
		}
		protected virtual void EndLoadData() {
			if(this.onDataLoaded != null)
				this.onDataLoaded();
			this.onDataLoaded = null;
		}
		protected void Initialized() {
			this.initialized.Set();
		}
	}
	public class PartsControlPartView : UserControl, IView {
#if DEBUG
		static int nextID = 0;
#endif
		bool isVisible;
		PartsControlPart part;
		bool partLoaded = false;
		bool viewLoaded = false;
		bool partLoading;
		public PartsControlPartView() {
#if DEBUG
			ID = ++nextID;
#endif
			DataContextChanged += OnPartsControlPartViewDataContextChanged;
			Loaded += OnPartsControlPartViewActualLoaded;
		}
#if DEBUG
		public int ID { get; private set; }
#endif
		public object GetObjectFromResources(object key) {
			return Resources[key];
		}
		public object GetObjectFromVisualTree(string name) {
			return FindName(name);
		}
		public bool IsReady { get; private set; }
		public event EventHandler Ready;
		public event EventHandler Hide;
		public event EventHandler Clear;
		public new event EventHandler IsVisibleChanged;
		public new bool IsVisible {
			get { return isVisible; }
			set {
				if(isVisible == value) return;
				isVisible = value;
				if(IsVisibleChanged != null)
					IsVisibleChanged(this, EventArgs.Empty);
			}
		}
		public void RaiseReady() {
			if(IsReady) return;
			IsReady = true;
			if(Ready != null)
				Ready(this, EventArgs.Empty);
		}
		public void OnHide() {
			if(Hide != null)
				Hide(this, EventArgs.Empty);
		}
		public void OnClear() {
			if(Clear != null)
				Clear(this, EventArgs.Empty);
		}
		void OnPartsControlPartViewActualLoaded(object sender, EventArgs e) {
			Loaded -= OnPartsControlPartViewActualLoaded;
			this.viewLoaded = true;
			RaiseReadyIfNeeded();
		}
		void OnPartsControlPartViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(partLoaded || partLoading) return;
			partLoading = true;
			this.part = DataContext as PartsControlPart;
			if(this.part == null)
				OnPartDataLoaded();
			else
				this.part.LoadData(OnPartDataLoaded);
		}
		void OnPartDataLoaded() {
			DataContextChanged -= OnPartsControlPartViewDataContextChanged;
			this.partLoaded = true;
			RaiseReadyIfNeeded();
		}
		void RaiseReadyIfNeeded() {
			if(this.viewLoaded && this.partLoaded)
				RaiseReady();
		}
	}
	public abstract class PartsControl : Control {
		#region Dependency Properties
		public static readonly DependencyProperty LoadingInProgressProperty;
		static PartsControl() {
			Type ownerType = typeof(PartsControl);
			LoadingInProgressProperty = DependencyProperty.Register("LoadingInProgress", typeof(bool), ownerType, new PropertyMetadata(false, RaiseLoadingInProgressChanged));
		}
		bool loadingInProgressValue = false;
		static void RaiseLoadingInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PartsControl)d).loadingInProgressValue = (bool)e.NewValue;
			((PartsControl)d).RaiseLoadingInProgressChanged(e);
		}
		#endregion
		Dictionary<Type, Type> views = new Dictionary<Type, Type>();
		Dictionary<object, bool> loadings = new Dictionary<object, bool>();
		public PartsControl() {
			RegisterViews();
			Template = XamlReaderHelper.CreateControlTemplate(typeof(PartsControl), typeof(ViewPresenter));
		}
		public bool LoadingInProgress { get { return loadingInProgressValue; } private set { SetValue(LoadingInProgressProperty, value); } }
		public event DepPropertyChangedEventHandler LoadingInProgressChanged;
		public PartsControlPartView CreateView(PartsControlPart part) {
			Type viewType = GetPartViewType(part);
			PartsControlPartView view = (PartsControlPartView)Activator.CreateInstance(viewType);
			view.DataContext = part;
			return view;
		}
		public void BeginLoading(object key) {
			Dispatcher.BeginInvoke((Action<object>)BeginLoadingCore, key);
		}
		public void EndLoading(object key) {
			Dispatcher.BeginInvoke((Action<object>)EndLoadingCore, key);
		}
		public void BeginInBackgroundThread(Action action) {
			Thread thread = new Thread(() => {
				action();
			});
			thread.IsBackground = true;
			thread.Start();
		}
		public void BeginInMainThread(Action action) {
			Dispatcher.BeginInvoke(action);
		}
		public void DoInBackgroundThread(Action action, Action completed) {
			BeginInBackgroundThread(() => {
				action();
				BeginInMainThread(completed);
			});
		}
		public void DoInMainThread(Action action) {
			if(Dispatcher.CheckAccess()) {
				action();
			} else {
				AutoResetEvent done = new AutoResetEvent(false);
				BeginInMainThread(() => {
					action();
					done.Set();
				});
				done.WaitOne();
			}
		}
		protected ViewPresenter Root { get; private set; }
		void CreateVisualTree() {
			if(Root == null) return;
			PartsControlPart mainPart = CreateMainPart();
			Root.Content = mainPart.View;
		}
		Type GetPartViewType(PartsControlPart part) {
			return views.First(p => part.GetType().IsSubclassOf(p.Key) || part.GetType() == p.Key).Value;
		}
		protected abstract PartsControlPart CreateMainPart();
		protected abstract void RegisterViews();
		protected void RegisterView(Type partType, Type viewType) {
			if(!partType.IsSubclassOf(typeof(PartsControlPart)))
				throw new ArgumentException("partType");
			if(!viewType.IsSubclassOf(typeof(PartsControlPartView)))
				throw new ArgumentException("viewType");
			views.Add(partType, viewType);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Root = GetTemplateChild(XamlReaderHelper.RootName) as ViewPresenter;
			CreateVisualTree();
		}
		void BeginLoadingCore(object key) {
			bool state;
			if(loadings.TryGetValue(key, out state)) {
				if(!state)
					throw new InvalidOperationException();
				loadings.Remove(key);
			} else {
				loadings.Add(key, false);
				LoadingInProgress = true;
			}
		}
		void EndLoadingCore(object key) {
			bool state;
			if(loadings.TryGetValue(key, out state)) {
				if(state)
					throw new InvalidOperationException();
				loadings.Remove(key);
				if(loadings.Count == 0)
					LoadingInProgress = false;
			} else {
				loadings.Add(key, true);
			}
		}
		void RaiseLoadingInProgressChanged(DependencyPropertyChangedEventArgs e) {
			if(LoadingInProgressChanged != null)
				LoadingInProgressChanged(this, new DepPropertyChangedEventArgs(e));
		}
	}
}
