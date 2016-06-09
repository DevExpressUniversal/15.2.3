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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using System.Linq;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
using DevExpress.Xpf.NavBar.Internal;
using DevExpress.Xpf.Editors.Flyout;
namespace DevExpress.Xpf.NavBar {
	public static class Categories {
		public const string Selection = "SelectionOptions";
		public const string Data = "Data";
		public const string Appearance = "Appearance";
		public const string Templates = "Templates";
		public const string OptionsView = "OptionsView";
	}
	[DXToolboxBrowsable]
	[ContentProperty("Groups")]
	public partial class NavBarControl : Control, IEventArgsConverterSource, IWeakEventListener, ISupportInitialize, INavigatorClient, IExpandableChild {
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty ActiveGroupProperty;
		public static readonly DependencyProperty AllowSelectItemProperty;
		public static readonly DependencyProperty AllowSelectDisabledItemProperty;
		public static readonly DependencyProperty EachGroupHasSelectedItemProperty;
		public static readonly DependencyProperty SelectedItemsProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty GroupDescriptionProperty;
		public static readonly DependencyProperty SelectedGroupProperty;										
		internal event RequestContainersEventHandler RequestContainers;		
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualItemsSourceProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		protected internal void GetContainers(out List<NavBarGroupControl> groups, out List<NavBarItemControl> items) {
			if (RequestContainers == null) {
				groups = new List<NavBarGroupControl>();
				items = new List<NavBarItemControl>();
				return;
			}
			RequestContainersEventArgs args = new RequestContainersEventArgs();
			RequestContainers(this, args);
			groups = args.Groups;
			items = args.Items;
		}
		protected virtual void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			(e.OldValue as INotifyCollectionChanged).Do(x => CollectionChangedEventManager.RemoveListener(x, this));
			(e.NewValue as INotifyCollectionChanged).Do(x => CollectionChangedEventManager.AddListener(x, this));
			(e.OldValue as IBindingList).Do(x => ListChangedEventManager.RemoveListener(x, this));
			(e.NewValue as IBindingList).Do(x => ListChangedEventManager.AddListener(x, this));
			UpdateActualItemsSource();
		}
		protected virtual void OnActualItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			SelectionStrategy.Lock();
			ItemsAttachedBehaviorCore<NavBarControl, NavBarGroup>.OnItemsSourcePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty,
				ItemTemplateProperty,
				ItemTemplateSelectorProperty,
				ItemStyleProperty,
				navBarControl => navBarControl.Groups,
				navBarControl => new NavBarGroup(),
				null, group => { }, null, LinkGroup);
			SelectionStrategy.Unlock();
		}
		protected virtual void LinkGroup(NavBarGroup group, object source) {
			group.SourceObject = source;
			group.NavBar = this;		 
			if(source is CollectionViewGroup)
				source = ((CollectionViewGroup)source).Name;
			if (View != null)
				View.RaiseGroupAdding(group, source);
		}
		protected virtual void UpdateActualItemsSourceAsync() {
			if (GetBindingExpression(GroupDescriptionProperty) == null || GetBindingExpression(GroupDescriptionProperty).Status == BindingStatus.Active)
				UpdateActualItemsSource();
			else
				Dispatcher.BeginInvoke(new Action(() => UpdateActualItemsSource()));
		}
		 void UpdateActualItemsSource() {
			if (ItemsSource == null) {
				ActualItemsSource = null;
				return;
			}
			ICollectionView collectionView = null;
			ListCollectionView listCollectionView = null;
			BindingListCollectionView bindingListCollectionView = null;
			if (ItemsSource is ICollectionView) {
				collectionView = ItemsSource as ICollectionView;
			} else if (!String.IsNullOrEmpty(GroupDescription)) {
				if (ItemsSource is IBindingList) {
					bindingListCollectionView = new BindingListCollectionView(ItemsSource as IBindingList);
					bindingListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(GroupDescription));
					ActualItemsSource = bindingListCollectionView.Groups;
					return;
				} else if (ItemsSource is IList) {
					listCollectionView = new ListCollectionView(ItemsSource as IList);
					listCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(GroupDescription));
					ActualItemsSource = (IList)listCollectionView.Groups;
					return;
				} else 
				if (ItemsSource is IEnumerable) {
					CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = ItemsSource };
					collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription(GroupDescription));
					collectionView = collectionViewSource.View;
					ActualItemsSource = collectionView.Groups;
					return;
				}
			}
			if (collectionView != null) {
				ActualItemsSource = collectionView.GroupDescriptions.Count == 1 ? collectionView.Groups : null;
				return;
			}
			if (ItemsSource is IEnumerable) {
				IEnumerable<object> source = ItemsSource as IEnumerable<object>;
				ActualItemsSource = source.ToList();
				return;
			}
		}
		protected virtual void OnItemTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			SelectionStrategy.Lock();
			ItemsAttachedBehaviorCore<NavBarControl, NavBarGroup>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
			SelectionStrategy.Unlock();
		}
		protected virtual void OnItemStyleChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			SelectionStrategy.Lock();
			ItemsAttachedBehaviorCore<NavBarControl, NavBarGroup>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
			SelectionStrategy.Unlock();
		}
		protected virtual void OnItemTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			SelectionStrategy.Lock();
			ItemsAttachedBehaviorCore<NavBarControl, NavBarGroup>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
			SelectionStrategy.Unlock();
		}
		static NavBarControl() {
			DevExpress.Mvvm.UI.ViewInjection.NavBarControlStrategy.RegisterStrategy();
			FocusableProperty.OverrideMetadata(typeof(NavBarControl), new FrameworkPropertyMetadata(false));
			ViewProperty = DependencyPropertyManager.Register("View", typeof(NavBarViewBase), typeof(NavBarControl), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarControl)d).OnViewChanged(d, e)));
			ActiveGroupProperty = DependencyPropertyManager.Register("ActiveGroup", typeof(NavBarGroup), typeof(NavBarControl), new FrameworkPropertyMetadata(null, OnActiveGroupChanged, CoerceActiveGroup));			
			IsEnabledProperty.OverrideMetadata(typeof(NavBarControl), new FrameworkPropertyMetadata(true, OnIsEnabledChanged));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(NavBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback((d, e) => ((NavBarControl)d).OnItemsSourceChanged(e))));
			GroupDescriptionProperty = DependencyPropertyManager.Register("GroupDescription", typeof(string), typeof(NavBarControl), new FrameworkPropertyMetadata(string.Empty, (d, e) => ((NavBarControl)d).UpdateActualItemsSourceAsync()));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback((d, e) => ((NavBarControl)d).OnItemTemplateSelectorChanged(e))));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(NavBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback((d, e) => ((NavBarControl)d).OnItemStyleChanged(e))));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(NavBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback((d, e) => ((NavBarControl)d).OnItemTemplateChanged(e))));
			ActualItemsSourceProperty = DependencyProperty.Register("ActualItemsSource", typeof(IEnumerable), typeof(NavBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback((d, e) => ((NavBarControl)d).OnActualItemsSourceChanged(e))));
			ItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<NavBarControl, NavBarGroup>), typeof(NavBarControl), new PropertyMetadata(null));
			SelectedGroupProperty = DependencyPropertyManager.Register("SelectedGroup", typeof(object), typeof(NavBarControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((NavBarControl)d).OnSelectedGroupChanged((object)e.OldValue)));
			SelectedItemsProperty = DependencyPropertyManager.Register("SelectedItems", typeof(IEnumerable), typeof(NavBarControl), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarControl)d).OnSelectedItemsChanged((IEnumerable)e.OldValue)));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), typeof(NavBarControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((NavBarControl)d).OnSelectedItemChanged((object)e.OldValue), (d, v) => ((NavBarControl)d).OnSelectedItemChanging(v)));
			AllowSelectItemProperty = DependencyPropertyManager.Register("AllowSelectItem", typeof(bool), typeof(NavBarControl), new PropertyMetadata(true, OnAllowSelectItemPropertyChanged));
			AllowSelectDisabledItemProperty = DependencyPropertyManager.Register("AllowSelectDisabledItem", typeof(bool), typeof(NavBarControl), new PropertyMetadata(false, OnAllowSelectDisabledItemPropertyChanged));
			EachGroupHasSelectedItemProperty = DependencyPropertyManager.Register("EachGroupHasSelectedItem", typeof(bool), typeof(NavBarControl), new PropertyMetadata(false, OnEachGroupHasSelectedItemPropertyChanged));			
		}				
		public static NavBarViewBase CreateDefaultView() {
			return new ExplorerBarView();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetCompact(compact);
		}
		void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null) {
				if(e.OldValue is NavigationPaneView) ((NavigationPaneView)e.OldValue).IsExpanded = true;
				RemoveChild(e.OldValue);
				((NavBarViewBase)e.OldValue).SetNavBar(null); 
			}
			if(e.NewValue != null) {
				NavBarViewBase view = (NavBarViewBase)e.NewValue;
				view.SetNavBar((NavBarControl)d);
			} else
				View = CreateDefaultView();
			if(View != null) AddChild(View);
			if (ActiveGroup != null)
				ActiveGroup.UpdateScrollMode();
			UpdateGroups(group => group.UpdateUseCustomIsVisible());
			if(View != null) View.UpdateViewForce();
		}
		static void OnActiveGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarControl)d).OnActiveGroupChanged((NavBarGroup)e.OldValue);
		}
		static object CoerceActiveGroup(DependencyObject d, object value) {
			return ((NavBarControl)d).CoerceActiveGroup((NavBarGroup)value);
		}
		static void OnAllowSelectItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarControl)d).SelectionStrategy.UpdateSelectionSettings();
		}
		static void OnAllowSelectDisabledItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarControl)d).SelectionStrategy.UpdateSelectionSettings();
		}
		static void OnEachGroupHasSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarControl)d).SelectionStrategy.UpdateSelectionSettings();
		}
		static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarControl)d).OnIsEnabledChanged();
		}
		NavBarGroupCollection groups;
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlGroups"),
#endif
 Category(Categories.Data)]
		public NavBarGroupCollection Groups { get { return groups; } }
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarControlView")]
#endif
		public NavBarViewBase View {
			get { return (NavBarViewBase)base.GetValue(ViewProperty); }
			set {
				if (value != View)
					CheckAcceptCompactNavigation();
				SetValue(ViewProperty, value); }
		}
		private void CheckAcceptCompactNavigation() {
			if (View is NavigationPaneView)
				RaisePropertyChanged("AcceptCompactNavigation");
		}
		#region Selection
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavBarGroup ActiveGroup {
			get { return (NavBarGroup)GetValue(ActiveGroupProperty); }
			set { SetValue(ActiveGroupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlAllowSelectItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Selection)]
		public bool AllowSelectItem {
			get { return (bool)GetValue(AllowSelectItemProperty); }
			set { SetValue(AllowSelectItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlAllowSelectDisabledItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Selection)]
		public bool AllowSelectDisabledItem {
			get { return (bool)GetValue(AllowSelectDisabledItemProperty); }
			set { SetValue(AllowSelectDisabledItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlEachGroupHasSelectedItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Selection)]
		public bool EachGroupHasSelectedItem {
			get { return (bool)GetValue(EachGroupHasSelectedItemProperty); }
			set { SetValue(EachGroupHasSelectedItemProperty, value); }
		}
		public IEnumerable SelectedItems {
			get { return (IEnumerable)GetValue(SelectedItemsProperty); }
			set { this.SetValue(SelectedItemsProperty, value); }
		}
		public Object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public object SelectedGroup {
			get { return (object)GetValue(SelectedGroupProperty); }
			set { SetValue(SelectedGroupProperty, value); }
		}
		protected internal SelectionStrategy SelectionStrategy { get; private set; }
		#endregion
		#region MVVM
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlItemsSource"),
#endif
 Category(Categories.Data)]
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarControlGroupDescription"),
#endif
 Category(Categories.Data)]
		public string GroupDescription {
			get { return (string)GetValue(GroupDescriptionProperty); }
			set { SetValue(GroupDescriptionProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		protected internal IList ActualItemsSource {
			get { return (IList)GetValue(ActualItemsSourceProperty); }
			set { SetValue(ActualItemsSourceProperty, value); }
		}
		#endregion
		public NavBarControl() {
			SelectionStrategy = new SelectionStrategy(this);
			SelectionStrategy.UpdateSelectionSettings();
			this.groups = new NavBarGroupCollection(this);
			Loaded += new RoutedEventHandler(OnLoaded);
			SelectedItems = new ReadOnlyObservableCollection<NavBarItem>(new ObservableCollection<NavBarItem>());			
			this.SetDefaultStyleKey(typeof(NavBarControl));
			eventArgsConverter = new EventArgsConverter(this);
			SetLocalBinding();
			compact = true;
		}		
		public void ReloadGroups() {
			UpdateActualItemsSource();
		}
		void ISupportInitialize.BeginInit() {
			SelectionStrategy.Lock();
			base.BeginInit();
		}
		void ISupportInitialize.EndInit() {
			base.EndInit();
			SelectionStrategy.Unlock();			
		}		
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if(View == null)
				View = CreateDefaultView();
			UpdateGroups(group => group.isNavBarInitialized = true);
		}				
		void SetLocalBinding() {
			Binding binding = new Binding("IsEnabled") { Source = this, Converter = new EnabledToOpacityConverter() };
			BindingOperations.SetBinding(this, NavBarControl.OpacityProperty, binding);
		}
		void OnActiveGroupChanged(NavBarGroup prevGroup) {
			SelectionStrategy.SelectGroup(ActiveGroup);
		}
		protected virtual void OnSelectedGroupChanged(object oldValue) {
			SelectionStrategy.SelectGroup(SelectedGroup);
			RaisePropertyChanged("SelectedItem");
		}
		private object OnSelectedItemChanging(object value) {
			return SelectionStrategy.CoerceSelection(value);
		}
		protected virtual void OnSelectedItemChanged(object oldValue) {
			SelectionStrategy.SelectItem(SelectedItem);
		}
		protected virtual void OnSelectedItemsChanged(IEnumerable oldValue) {
			SelectionStrategy.SelectItems(SelectedItems);
		}
		void OnIsEnabledChanged() {
			foreach (NavBarGroup group in Groups)
				group.CoerceValue(NavBarGroup.IsEnabledProperty);
		}		
		object CoerceActiveGroup(NavBarGroup newGroup) {
			return CoerceActiveGroupCore(newGroup, ActiveGroup);
		}
		object CoerceActiveGroupCore(NavBarGroup newGroup, NavBarGroup oldGroup) {
			if (Groups.Count == 0)
				return null;
			if (newGroup == null || !newGroup.ActualIsVisible || Groups.IndexOf(newGroup) == ConstantHelper.InvalidIndex || AnimationInProgress())
				return oldGroup;
			if (View != null && oldGroup != newGroup && SelectionStrategy.AllowGroupChangingEvent) {
				NavBarActiveGroupChangingEventArgs e = new NavBarActiveGroupChangingEventArgs(oldGroup, newGroup);
				e.RoutedEvent = NavBarViewBase.ActiveGroupChangingEvent;
				View.RaiseEvent(e);
				if (e.Cancel)
					return oldGroup;
			}
			return newGroup;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if (View == null)
				return;
			View.UpdateView();
			UpdateGroups(group => group.isNavBarInitialized = true);
		}
		internal void UpdateGroups(Action<NavBarGroup> action) {
			foreach(NavBarGroup group in Groups)
				action(group);
		}
		internal bool AnimationInProgress() {
			foreach(NavBarGroup group in Groups)
				if(group.IsCollapsing || group.IsExpanding)					
					return true;
			return false;
		}
		class EventArgsConverter : IDataRowEventArgsConverter {
			readonly NavBarControl navBar;
			public EventArgsConverter(NavBarControl navBar) {
				this.navBar = navBar;
			}
			object IDataRowEventArgsConverter.GetDataRow(RoutedEventArgs e) {
				if (navBar == null || navBar.View == null)
					return null;
				NavBarItem item = navBar.View.GetNavBarItem(e);
				if (item == null)
					return null;
				return item.SourceObject ?? item.DataContext;
			}
		}
		readonly EventArgsConverter eventArgsConverter;
		object IEventArgsConverterSource.EventArgsConverter {
			get { return eventArgsConverter; }
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (ActualItemsSource == null || (ItemsSource is ICollectionView && ((ICollectionView)ItemsSource).GroupDescriptions.Count != 1) || String.IsNullOrEmpty(GroupDescription)) {
				UpdateActualItemsSource();
			}
			return true;
		}
		public bool AcceptsCompactNavigation {
			get {
				if (View is NavigationPaneView)
					return true;
				else
					return false;
			}
		}
		public bool Compact {
			get {
				return compact;
			}
			set {
				if (value == compact)
					return;
				SetCompact(value);
				compact = value;
				RaisePropertyChanged("Compact");
			}
		}
		private void SetCompact(bool hide) {
			if (View == null || !(View is NavigationPaneView) )
				return;
			((NavigationPaneView)View).IsOverflowPanelVisibleInternal = hide;
			((NavigationPaneView)View).IsSplitterVisibleInternal = hide;
			((NavigationPaneView)View).IsCompact = !hide;
		}
		public IEnumerable<INavigationItem> Items {
			get { return Groups; }
		}
		INavigationItem INavigatorClient.SelectedItem {
			get {
				return SelectionStrategy.SelectedGroup;
			}
			set {
				SelectionStrategy.SelectGroup(value);
			}
		}
		public IList<Bars.IBarManagerControllerAction> MenuActions {
			get {
				if (View == null || !(View is NavigationPaneView))
					return null;
				return ((NavigationPaneView)View).MenuCustomizations;
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected internal void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		bool compact;
		int INavigatorClient.PeekFormHideDelay {
			get {
				if (View as NavigationPaneView != null) 
					return ((NavigationPaneView)View).PeekFormHideDelay;
				return 0;
			}
		}
		int INavigatorClient.PeekFormShowDelay {
			get {
				if (View as NavigationPaneView != null)
					return ((NavigationPaneView)View).PeekFormShowDelay;
				return 0;
			}
		}
		bool isAttachedClient;
		bool INavigatorClient.IsAttached {
			get { return isAttachedClient; }
			set { isAttachedClient = value; }
		}
		protected override IEnumerator LogicalChildren {
			get {
				List<object> res = new List<object>();
				res.Add(View);
				res.AddRange(Groups);
				return res.GetEnumerator();
			}
		}
		bool IExpandableChild.IsExpanded {
			get { return (View as NavigationPaneView).Return(x => x.IsExpanded, () => true); }
			set {
				if (!(View is NavigationPaneView))
					return;
				((NavigationPaneView)View).IsExpanded = value;
			}
		}
		double IExpandableChild.CollapseWidth { get { return (View as NavigationPaneView).Return(x => 36d, () => double.NaN); } }
		double IExpandableChild.ExpandedWidth {
			get { return (View as NavigationPaneView).Return(x => x.ExpandedWidth, () => double.NaN); }
			set {
				if(!(View is NavigationPaneView)) return;
				((NavigationPaneView)View).ExpandedWidth = value;
			}
		}
		ValueChangedEventHandler<bool> isExpandedChanged;
		event ValueChangedEventHandler<bool> IExpandableChild.IsExpandedChanged {
			add { isExpandedChanged += value; }
			remove { isExpandedChanged -= value; }
		}
		protected internal virtual void RaiseIsExpandedChanged(bool oldValue, bool newValue) {
			if (isExpandedChanged == null)
				return;
			isExpandedChanged(this, new ValueChangedEventArgs<bool>(oldValue, newValue));
		}
		internal void AddChild(object child) {
			AddLogicalChild(child);
		}
		internal void RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
	}
	public class ConstantHelper {
		public const int InvalidIndex = -1;
	}
	delegate void RequestContainersEventHandler(object sender, RequestContainersEventArgs args);
	class RequestContainersEventArgs : EventArgs {
		public List<NavBarItemControl> Items { get; private set; }
		public List<NavBarGroupControl> Groups { get; private set; }
		public RequestContainersEventArgs() {
			Items = new List<NavBarItemControl>();
			Groups = new List<NavBarGroupControl>();
		}
		public void AddContainer(INavBarContainer container) {
			if (container is NavBarItemControl)
				Items.Add(container as NavBarItemControl);
			if (container is NavBarGroupControl)
				Groups.Add(container as NavBarGroupControl);
		}
	}
	interface INavBarContainer {
		RequestContainersWeakEventHandler RequestContainerHandler { get; }
	}
	static class INavBarContainerHelper {
		public static void OnNavBarChanged(this INavBarContainer container, NavBarControl oldValue, NavBarControl newValue) {
			if (oldValue != null)
				oldValue.RequestContainers -= container.RequestContainerHandler.Handler;
			if (newValue != null)
				newValue.RequestContainers += container.RequestContainerHandler.Handler;
		}
	}
	class RequestContainersWeakEventHandler : WeakEventHandler<INavBarContainer, RequestContainersEventArgs, RequestContainersEventHandler> {
		static Action<WeakEventHandler<INavBarContainer, RequestContainersEventArgs, RequestContainersEventHandler>, object> onDetachAction = (h, o) => ((NavBarControl)o).RequestContainers -= h.Handler;
		static Func<WeakEventHandler<INavBarContainer, RequestContainersEventArgs, RequestContainersEventHandler>, RequestContainersEventHandler> createHandlerFunction = h => h.OnEvent;
		public RequestContainersWeakEventHandler(INavBarContainer owner, Action<INavBarContainer, object, RequestContainersEventArgs> onEventAction)
			:
			base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}
