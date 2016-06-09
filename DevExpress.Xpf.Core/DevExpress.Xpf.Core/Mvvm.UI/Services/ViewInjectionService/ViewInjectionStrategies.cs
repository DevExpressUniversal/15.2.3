﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

#if !FREE
using DevExpress.Xpf.Core;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.Mvvm.UI.ViewInjection {
	public interface IStrategyManager {
		void RegisterStrategy<TTarget, TStrategy>()
			where TTarget : DependencyObject
			where TStrategy : IStrategy, new();
		IStrategy CreateStrategy(DependencyObject target);
	}
	public class StrategyManager : IStrategyManager {
		public static IStrategyManager Default { get { return _default ?? _instance; } set { _default = value; } }
		static IStrategyManager _default;
		static StrategyManager _instance;
		static StrategyManager() {
			_instance = new StrategyManager();
			_instance.RegisterDefaultStrategies();
		}
		const string Exception = "Cannot find an appropriate strategy for the {0} container type.";
		readonly Dictionary<Type, Type> Strategies = new Dictionary<Type, Type>();
		public void RegisterStrategy<TTarget, TStrategy>()
			where TTarget : DependencyObject
			where TStrategy : IStrategy, new() {
			Type tTarget = typeof(TTarget);
			Type tStrategy = typeof(TStrategy);
			if(Strategies.ContainsKey(tTarget))
				Strategies[tTarget] = tStrategy;
			else Strategies.Add(tTarget, tStrategy);
		}
		public IStrategy CreateStrategy(DependencyObject target) {
			if(target == null) return null;
			Type tTarget = target.GetType();
			Type tStrategy = null;
			do {
				foreach(Type currentTTarget in Strategies.Keys) {
					if(currentTTarget == tTarget) {
						tStrategy = Strategies[currentTTarget];
						break;
					}
				}
				tTarget = tTarget.BaseType;
			} while(tTarget != null && tStrategy == null);
			if(tStrategy == null) throw new InvalidOperationException(string.Format(Exception, target.GetType().Name));
			return (IStrategy)Activator.CreateInstance(tStrategy, null);
		}
		public void RegisterDefaultStrategies() {
			RegisterStrategy<Panel, PanelStrategy<Panel, PanelWrapper>>();
			RegisterStrategy<ContentPresenter, ContentPresenterStrategy<ContentPresenter, ContentPresenterWrapper>>();
			RegisterStrategy<ContentControl, ContentPresenterStrategy<ContentControl, ContentControlWrapper>>();
			RegisterStrategy<ItemsControl, ItemsControlStrategy<ItemsControl, ItemsControlWrapper>>();
			RegisterStrategy<Selector, SelectorStrategy<Selector, SelectorWrapper>>();
			RegisterStrategy<TabControl, SelectorStrategy<TabControl, TabControlWrapper>>();
#if !FREE
			RegisterStrategy<DXTabControl, DXTabControlStrategy>();
#endif
		}
	}
	public interface IStrategy {
		IEnumerable<object> ViewModels { get; }
		object SelectedViewModel { get; set; }
		event PropertyValueChangedEventHandler<object> SelectedViewModelChanged;
		event ViewModelClosingEventHandler ViewModelClosing;
		bool IsInitialized { get; }
		void Initialize(ViewInjectionService owner);
		void Uninitialize();
		void Inject(object viewModel, string viewName, Type viewType);
		bool Remove(object viewModel);
	}
	public abstract class StrategyBase<T> : IStrategy where T : DependencyObject {
		const string Exception = "Cannot set the SelectedViewModel property to a value that does not exist in the ViewModels collection. Inject the view model before selecting it.";
		public bool IsInitialized { get; private set; }
		public event PropertyValueChangedEventHandler<object> SelectedViewModelChanged;
		public event ViewModelClosingEventHandler ViewModelClosing;
		public object SelectedViewModel {
			get { return selectedViewModel; }
			set {
				if(selectedViewModel == value) return;
				if(value != null && !ViewModels.Contains(value))
					throw new InvalidOperationException(Exception);
				object oldValue = selectedViewModel;
				selectedViewModel = value;
				OnSelectedViewModelChanged(oldValue, selectedViewModel);
				RaiseSelectedViewModelChanged(oldValue, selectedViewModel);
			}
		}
		object selectedViewModel;
		protected ViewInjectionService Owner { get; private set; }
		protected T Target { get { return (T)Owner.AssociatedObject; } }
		protected IViewLocator ViewLocator { get { return Owner.ViewLocator ?? (DevExpress.Mvvm.UI.ViewLocator.Default ?? DevExpress.Mvvm.UI.ViewLocator.Instance); } }
		protected DataTemplateSelector ViewSelector { get; private set; }
		protected ObservableCollection<object> ViewModels { get; private set; }
		ViewModelInfoManager ViewModelInfos { get; set; }
		IEnumerable<object> IStrategy.ViewModels { get { return ViewModels; } }
		public StrategyBase() {
			ViewModelInfos = new ViewModelInfoManager(this);
			ViewModels = new ObservableCollection<object>();
			ViewSelector = new ViewDataTemplateSelector(this);
		}
		public void Initialize(ViewInjectionService owner) {
			Owner = owner;
			if(IsInitialized) return;
			IsInitialized = true;
			InitializeCore();
		}
		public void Uninitialize() {
			if(!IsInitialized) return;
			UninitializeCore();
			IsInitialized = false;
		}
		public void Inject(object viewModel, string viewName, Type viewType) {
			if(viewModel == null || ViewModels.Contains(viewModel)) return;
			CheckInjectionProcedure(viewModel, viewName, viewType);
			ViewModelInfos.Add(viewModel, viewName, viewType);
			ViewModels.Add(viewModel);
			OnInjected(viewModel);
		}
		public bool Remove(object viewModel) {
			if(viewModel == null || !ViewModels.Contains(viewModel)) return false;
			if(!RaiseViewModelClosing(viewModel)) return false;
			ViewModelInfos.Remove(viewModel);
			ViewModels.Remove(viewModel);
			OnRemoved(viewModel);
			return true;
		}
		protected virtual void InitializeCore() { }
		protected virtual void UninitializeCore() { }
		protected virtual void OnInjected(object viewModel) { }
		protected virtual void OnRemoved(object viewModel) { }
		protected virtual void OnSelectedViewModelChanged(object oldValue, object newValue) { }
		protected virtual void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) { }
		void RaiseSelectedViewModelChanged(object oldValue, object newValue) {
			if(SelectedViewModelChanged != null)
				SelectedViewModelChanged(this, new PropertyValueChangedEventArgs<object>(oldValue, newValue));
		}
		bool RaiseViewModelClosing(object viewModel) {
			if(ViewModelClosing != null) {
				ViewModelClosingEventArgs e = new ViewModelClosingEventArgs(viewModel);
				ViewModelClosing(this, e);
				return !e.Cancel;
			}
			return true;
		}
		class ViewModelInfoManager {
			StrategyBase<T> Owner;
			List<ViewModelInfo> Infos = new List<ViewModelInfo>();
			public ViewModelInfoManager(StrategyBase<T> owner) {
				Owner = owner;
			}
			public void UpdateInfos() {
				List<ViewModelInfo> toRemove = new List<ViewModelInfo>();
				foreach(var info in Infos)
					if(!info.ViewModel.IsAlive) toRemove.Add(info);
				foreach(var info in toRemove)
					Infos.Remove(info);
			}
			public void Add(object viewModel, string viewName, Type viewType) {
				UpdateInfos();
				Infos.Add(new ViewModelInfo(viewModel, viewName, viewType));
			}
			public void Remove(object viewModel) {
				UpdateInfos();
			}
			public DataTemplate GetViewTemplate(object viewModel) {
				if(viewModel == null) return null;
				UpdateInfos();
				var info = Infos.FirstOrDefault(x => x.ViewModel.Target == viewModel);
				if(info == null) return null;
				info.UpdateViewTemplate(Owner.ViewLocator);
				return info.ViewTemplate;
			}
		}
		class ViewModelInfo {
			public WeakReference ViewModel { get; private set; }
			public string ViewName { get; private set; }
			public Type ViewType { get; private set; }
			public DataTemplate ViewTemplate { get; private set; }
			public ViewModelInfo(object viewModel, string viewName, Type viewType) {
				ViewModel = new WeakReference(viewModel);
				ViewName = viewName;
				ViewType = viewType;
			}
			public void UpdateViewTemplate(IViewLocator viewLocator) {
				if(ViewTemplate != null) return;
				if(ViewType != null) {
					ViewTemplate = viewLocator.CreateViewTemplate(ViewType);
					return;
				}
				if(!string.IsNullOrEmpty(ViewName)) {
					ViewTemplate = viewLocator.CreateViewTemplate(ViewName);
					return;
				}
				ViewTemplate = null;
				return;
			}
		}
		class ViewDataTemplateSelector : DataTemplateSelector {
			StrategyBase<T> Owner;
			public ViewDataTemplateSelector(StrategyBase<T> owner) {
				Owner = owner;
			}
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				return Owner.ViewModelInfos.GetViewTemplate(item) ?? base.SelectTemplate(item, container);
			}
		}
	}
	public abstract class CommonStrategyBase<T, TWrapper> : StrategyBase<T>
		where T : DependencyObject
		where TWrapper : class, ITargetWrapper<T>, new() {
		protected TWrapper Wrapper { get; private set; }
		protected override void InitializeCore() {
			base.InitializeCore();
			Wrapper = new TWrapper() { Target = this.Target };
		}
		protected override void UninitializeCore() {
			Wrapper.Target = null;
			Wrapper = null;
			base.UninitializeCore();
		}
	}
	public interface ITargetWrapper<T> where T : DependencyObject {
		T Target { set; }
	}
	public interface IPanelWrapper<T> : ITargetWrapper<T> where T : DependencyObject {
		IEnumerable Children { get; }
		void AddChild(UIElement child);
		void RemoveChild(UIElement child);
	}
	public interface IContentPresenterWrapper<T> : ITargetWrapper<T> where T : DependencyObject {
		object Content { get; set; }
		DataTemplate ContentTemplate { get; set; }
		DataTemplateSelector ContentTemplateSelector { get; set; }
	}
	public interface IItemsControlWrapper<T> : ITargetWrapper<T> where T : DependencyObject {
		object ItemsSource { get; set; }
		DataTemplate ItemTemplate { get; set; }
		DataTemplateSelector ItemTemplateSelector { get; set; }
	}
	public interface ISelectorWrapper<T> : IItemsControlWrapper<T> where T : DependencyObject {
		object SelectedItem { get; set; }
		event EventHandler SelectionChanged;
	}
	public class PanelStrategy<T, TWrapper> : CommonStrategyBase<T, TWrapper>
		where T : DependencyObject
		where TWrapper : class, IPanelWrapper<T>, new() {
		protected override void OnInjected(object viewModel) {
			base.OnInjected(viewModel);
			Wrapper.AddChild(new ContentPresenter() { Content = viewModel, ContentTemplateSelector = ViewSelector });
		}
		protected override void OnRemoved(object viewModel) {
			base.OnRemoved(viewModel);
			var child = FindChild(viewModel);
			Wrapper.RemoveChild(child);
			if(viewModel == SelectedViewModel)
				SelectedViewModel = null;
		}
		ContentPresenter FindChild(object viewModel) {
			return Wrapper.Children.Cast<ContentPresenter>().FirstOrDefault(x => x.Content == viewModel);
		}
	}
	public class ContentPresenterStrategy<T, TWrapper> : CommonStrategyBase<T, TWrapper>
		where T : DependencyObject
		where TWrapper : class, IContentPresenterWrapper<T>, new() {
		const string Exception1 = "It is impossible to use ViewInjectionService for the control that has the Content property set.";
		const string Exception2 = "ViewInjectionService cannot create view by name or type, because the target control has the ContentTemplate/ContentTemplateSelector property set.";
		protected override void InitializeCore() {
			base.InitializeCore();
			if(Wrapper.Content != null && !ViewModels.Contains(Wrapper.Content))
				throw new InvalidOperationException(Exception1);
			if(Wrapper.ContentTemplate == null && Wrapper.ContentTemplateSelector == null)
				Wrapper.ContentTemplateSelector = ViewSelector;
		}
		protected override void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) {
			base.CheckInjectionProcedure(viewModel, viewName, viewType);
			if(Wrapper.ContentTemplateSelector != ViewSelector && (!string.IsNullOrEmpty(viewName) || viewType != null))
				throw new InvalidOperationException(Exception2);
		}
		protected override void OnInjected(object viewModel) {
			base.OnInjected(viewModel);
			SelectedViewModel = viewModel;
		}
		protected override void OnRemoved(object viewModel) {
			base.OnRemoved(viewModel);
			if(viewModel == SelectedViewModel)
				SelectedViewModel = null;
		}
		protected override void OnSelectedViewModelChanged(object oldValue, object newValue) {
			base.OnSelectedViewModelChanged(oldValue, newValue);
			Wrapper.Content = newValue;
		}
	}
	public class ItemsControlStrategy<T, TWrapper> : CommonStrategyBase<T, TWrapper>
		where T : DependencyObject
		where TWrapper : class, IItemsControlWrapper<T>, new() {
		const string Exception1 = "It is impossible to use ViewInjectionService for the control that has the ItemsSource property set.";
		const string Exception2 = "ViewInjectionService cannot create view by name or type, because the target control has the ItemTemplate/ItemTemplateSelector property set.";
		protected override void InitializeCore() {
			base.InitializeCore();
			InitItemsSource();
			InitItemTemplate();
		}
		protected virtual void InitItemsSource() {
			if(Wrapper.ItemsSource != null && Wrapper.ItemsSource != ViewModels)
				throw new InvalidOperationException(Exception1);
			Wrapper.ItemsSource = ViewModels;
		}
		protected virtual void InitItemTemplate() {
			if(Wrapper.ItemTemplate == null && Wrapper.ItemTemplateSelector == null)
				Wrapper.ItemTemplateSelector = ViewSelector;
		}
		protected override void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) {
			base.CheckInjectionProcedure(viewModel, viewName, viewType);
			if(Wrapper.ItemTemplateSelector != ViewSelector && (!string.IsNullOrEmpty(viewName) || viewType != null))
				throw new InvalidOperationException(Exception2);
		}
		protected override void OnRemoved(object viewModel) {
			base.OnRemoved(viewModel);
			if(viewModel == SelectedViewModel)
				SelectedViewModel = null;
		}
	}
	public class SelectorStrategy<T, TWrapper> : ItemsControlStrategy<T, TWrapper>
		where T : DependencyObject
		where TWrapper : class, ISelectorWrapper<T>, new() {
		protected override void InitializeCore() {
			base.InitializeCore();
			Wrapper.SelectionChanged += OnTargetControlSelectionChanged;
		}
		protected override void UninitializeCore() {
			Wrapper.SelectionChanged -= OnTargetControlSelectionChanged;
			base.UninitializeCore();
		}
		protected override void OnSelectedViewModelChanged(object oldValue, object newValue) {
			Wrapper.SelectedItem = newValue;
		}
		void OnTargetControlSelectionChanged(object sender, EventArgs e) {
			SelectedViewModel = Wrapper.SelectedItem;
		}
	}
	public class PanelWrapper : IPanelWrapper<Panel> {
		public Panel Target { get; set; }
		public IEnumerable Children {
			get { return Target.Children; }
		}
		public void AddChild(UIElement child) {
			Target.Children.Add(child);
		}
		public void RemoveChild(UIElement child) {
			Target.Children.Remove(child);
		}
	}
	public class ContentPresenterWrapper : IContentPresenterWrapper<ContentPresenter> {
		public ContentPresenter Target { get; set; }
		public object Content {
			get { return Target.Content; }
			set { Target.Content = value; }
		}
		public DataTemplate ContentTemplate {
			get { return Target.ContentTemplate; }
			set { Target.ContentTemplate = value; }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return Target.ContentTemplateSelector; }
			set { Target.ContentTemplateSelector = value; }
		}
	}
	public class ContentControlWrapper : IContentPresenterWrapper<ContentControl> {
		public ContentControl Target { get; set; }
		public object Content {
			get { return Target.Content; }
			set { Target.Content = value; }
		}
		public DataTemplate ContentTemplate {
			get { return Target.ContentTemplate; }
			set { Target.ContentTemplate = value; }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return Target.ContentTemplateSelector; }
			set { Target.ContentTemplateSelector = value; }
		}
	}
	public class ItemsControlWrapper : IItemsControlWrapper<ItemsControl> {
		public ItemsControl Target { get; set; }
		public object ItemsSource {
			get { return Target.ItemsSource; }
			set { Target.ItemsSource = (IEnumerable)value; }
		}
		public virtual DataTemplate ItemTemplate {
			get { return Target.ItemTemplate; }
			set { Target.ItemTemplate = value; }
		}
		public virtual DataTemplateSelector ItemTemplateSelector {
			get { return Target.ItemTemplateSelector; }
			set { Target.ItemTemplateSelector = value; }
		}
	}
	public class SelectorWrapper : ItemsControlWrapper, ISelectorWrapper<Selector> {
		public new Selector Target {
			get { return (Selector)base.Target; }
			set { base.Target = value; }
		}
		public object SelectedItem {
			get { return Target.SelectedItem; }
			set { Target.SelectedItem = value; }
		}
		public event EventHandler SelectionChanged {
			add { Target.SelectionChanged += new SelectionChangedEventHandler(value); }
			remove { Target.SelectionChanged -= new SelectionChangedEventHandler(value); }
		}
	}
	public class TabControlWrapper : SelectorWrapper, ISelectorWrapper<TabControl> {
		public new TabControl Target {
			get { return (TabControl)base.Target; }
			set { base.Target = value; }
		}
		public override DataTemplate ItemTemplate {
			get { return Target.ContentTemplate; }
			set { Target.ContentTemplate = value; }
		}
		public override DataTemplateSelector ItemTemplateSelector {
			get { return Target.ContentTemplateSelector; }
			set { Target.ContentTemplateSelector = value; }
		}
	}
#if !FREE
	public class DXTabControlWrapper : ItemsControlWrapper, ISelectorWrapper<DXTabControl> {
		public new DXTabControl Target {
			get { return (DXTabControl)base.Target; }
			set { base.Target = value; }
		}
		public object SelectedItem {
			get { return Target.SelectedItem; }
			set { Target.SelectItem(value); }
		}
		public event EventHandler SelectionChanged {
			add { Target.SelectionChanged += new TabControlSelectionChangedEventHandler(value); }
			remove { Target.SelectionChanged -= new TabControlSelectionChangedEventHandler(value); }
		}
	}
	public class DXTabControlStrategy : SelectorStrategy<DXTabControl, DXTabControlWrapper> {
		protected override void InitializeCore() {
			base.InitializeCore();
			Target.SelectionChanged += OnTargetSelectionChanged;
			Target.TabRemoving += OnTargetTabRemoving;
		}
		protected override void UninitializeCore() {
			Target.SelectionChanged -= OnTargetSelectionChanged;
			Target.TabRemoving -= OnTargetTabRemoving;
			base.UninitializeCore();
		}
		void OnTargetTabRemoving(object sender, TabControlTabRemovingEventArgs e) {
			var tabItem = Target.GetTabItem(e.TabIndex);
			e.Cancel = !Remove(tabItem.DataContext);
		}
		void OnTargetSelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			var tabItem = Target.GetTabItem(e.NewSelectedIndex);
			if(tabItem == null || tabItem.IsVisible) return;
			Target.ShowTabItem(tabItem);
		}
	}
#endif
}
