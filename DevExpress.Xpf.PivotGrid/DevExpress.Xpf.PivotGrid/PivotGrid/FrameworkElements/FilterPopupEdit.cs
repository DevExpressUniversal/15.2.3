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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
#if !SL
using DevExpress.Xpf.Editors.Themes;
using DependencyPropertyManager = System.Windows.DependencyProperty;
using DevExpress.Xpf.Editors.Internal;
#else
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Internal;
#endif
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.Bars;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.PivotGrid.Internal {
	[ContentProperty("Items")]
	[DXToolboxBrowsable(false)]
	public class FilterPopupEdit : ComboBoxEdit {
		public PivotGridField Field {
			get { return (PivotGridField)GetValue(FieldProperty); }
			set { SetValue(FieldProperty, value); }
		}
		public static readonly DependencyProperty FieldProperty =
			DependencyPropertyManager.Register("Field", typeof(PivotGridField), typeof(FilterPopupEdit), new PropertyMetadata(null, (d,e) => ((FilterPopupEdit)d).OnFieldChanged()));
		public FilterPopupEdit()
			: base() {
			PopupOpened += FilterPopupOpened;
			PopupOpening += FilterPopupOpening;
			PopupClosed += FilterPopupClosed;
		}
		public PivotGridControl PivotGrid { get; private set; }
		public CoreXtraPivotGrid.PivotFilterItemsBase FilterItems { get; set; }
		public bool IsDeferredFilling { get { return PivotGrid.UseAsyncMode; } }
		internal FilterCheckedTreeView PopupTreeView { get; set; }
		protected virtual void OnFieldChanged() {
			if(Field != null) {
				PivotGrid = Field.PivotGrid;
				FilterItems = Field.FilterInfo.FilterItems;
				SetContentContainerTemplate(CheckedListFilterInfo.IsSimpleFilterPopup(Field));
			} else {
				PivotGrid = null;
			}
		}
#if !SL
		public virtual void SetContentContainerTemplate(bool isSimple) {
			if(isSimple)
				SetResourceReference(PopupContentTemplateProperty, GetComboBoxEditTemplate());
			else
				SetResourceReference(PopupContentTemplateProperty, GetCheckedTreeViewTemplate());
		}
	   object GetComboBoxEditTemplate() {
			return new ComboBoxEditThemeKeyExtension() {
				ResourceKey = ComboBoxThemeKeys.PopupContentTemplate,
				ThemeName = OwnerThemeName
			};
		}
#else
		 public virtual void SetContentContainerTemplate(bool isSimple) {
			 PopupContentTemplate = (ControlTemplate)ResourceHelper.FindResource(PivotGrid, isSimple ? "ComboBoxEditThemeKey_PopupContentTemplate" : GetCheckedTreeViewTemplate());
		 }
#endif 
		object GetCheckedTreeViewTemplate() {
			return new PivotGridThemeKeyExtension() {
				ResourceKey = PivotGridThemeKeys.FilterCheckedTreeViewPopupTemplate,
				ThemeName = OwnerThemeName
			};
		}
		public ControlTemplate GetWaitIndicatorTemplate() {
			return (ControlTemplate)ResourceHelper.FindResource(PivotGrid, new PivotGridThemeKeyExtension() { 
				ResourceKey = PivotGridThemeKeys.FilterWaitIndicatorTemplate,
				ThemeName = OwnerThemeName
			});
		}
		string OwnerThemeName {
			get {
				return ThemeHelper.GetEditorThemeName(this);
			}
		}
		public List<object> GetFirstLevelItems() {
			List<object> result = new List<object>();
			IEnumerable itemsSource = ((ISelectorEdit)this).GetPopupContentCustomItemsSource();
			if(itemsSource != null)
				result.AddRange(itemsSource.Cast<object>());
			itemsSource = ((ISelectorEdit)this).GetPopupContentItemsSource() as IEnumerable;
			if(itemsSource == null)
				return result;
			result.AddRange(itemsSource.Cast<object>());
			return result;
		}
		protected override void ItemsSourceChanged(object itemsSource) {
			base.ItemsSourceChanged(itemsSource);
			if(PopupTreeView != null) {
				PopupTreeView.IsItemsValid = false;
				PopupTreeView.InvalidateMeasure();
			}
		}
		protected override bool IsEditorKeyboardFocused {
			get {
				return base.IsEditorKeyboardFocused 
					|| PivotGrid != null && PivotGrid.Data.IsInProcessing;
			}
		}
		protected override bool CanShowPopupCore() {
			return base.CanShowPopupCore()
				|| PivotGrid != null && PivotGrid.Data.IsInProcessing;
		}
		FieldHeaderBase Header {
			get {
				return LayoutHelper.FindParentObject<FieldHeaderBase>(this);
			}
		}
		bool IsFieldListHeader {
			get {
				FieldHeaderBase header = Header;
				if(header == null)
					return false;
				return FieldHeadersBase.GetFieldListArea(header) == FieldListArea.All;
			}
		}
		bool DeferUpdates {
			get {
				return PivotGrid.Data.FieldListFields.DeferUpdates && IsFieldListHeader;
			}
		}
#if SL
		protected override bool CanShowPopup {
			get {
				return CanShowPopupCore();
			}
		}
#endif
#if DEBUGTEST
		internal Button GetCancelButton() { return PopupBaseEditHelper.GetCancelButton(this); }
#endif
		public void DisableOkButton() {
			PropertyProvider.PopupViewModel.OkButtonIsEnabled = false;
		}
		public void EnableOkButton() {
			PropertyProvider.PopupViewModel.OkButtonIsEnabled = true;
		}
		protected override void OnPopupOpened() {
			base.OnPopupOpened();
		}
		void FilterPopupOpened(object sender, RoutedEventArgs e) {
			if(PivotGrid != null && sender is FilterPopupEdit && ((FilterPopupEdit)sender).PopupTreeView != null)
				((FilterPopupEdit)sender).PopupTreeView.SetValue(BarManager.DXContextMenuProperty, PivotGrid.GridMenu);
		}
		void FilterPopupOpening(object sender, RoutedEventArgs e) {
			FieldHeader header = Header as FieldHeader;
			if(Field == null || 
					!Field.Visible && !IsFieldListHeader && !PivotGrid.DeferredUpdates ||
					   IsFieldListHeader && header != null && header.GetArea() == FieldListArea.All || 
							IsFieldListHeader && !PivotGrid.AllowFilterInFieldList) {
				this.CancelPopup();
#if !SL
				e.Handled = true;
#endif
				return;
			}
			Field.FilterInfo.OnPopupOpening(this, DeferUpdates);
		}
		void FilterPopupClosed(object sender, ClosePopupEventArgs e) {
			if(Field == null || e == null)
				return;
			Field.FilterInfo.OnPopupClosed(this, e.CloseMode != PopupCloseMode.Cancel, DeferUpdates);
			FieldHeader header = Header as FieldHeader;
			if(header != null)
				header.UpdateIsFilterButtonVisible();
		}
	}
	public class CheckedTreeViewItem : TreeViewItem {
		#region static stuff
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		static CheckedTreeViewItem() {
			Type ownerType = typeof(CheckedTreeViewItem);
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((CheckedTreeViewItem)d).OnIsCheckedChanged((bool?)e.NewValue)));
			IsThreeStateProperty = DependencyPropertyManager.Register("IsThreeState", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		#endregion
		static int itemCount = 0;
		public CheckedTreeViewItem() {
#if SL
			this.SetDefaultStyleKey(typeof(CheckedTreeViewItem));
#endif
			itemCount++;
		}
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { SetValue(IsThreeStateProperty, value); }
		}
		protected virtual void OnIsCheckedChanged(bool? newValue) {
			EnsureThreeState(newValue);
		}
		protected void EnsureThreeState(bool? newValue) {
			IsThreeState = newValue == null;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e) {
			base.OnKeyUp(e);
			bool handled = e.Handled;
			ProcessKeyUp(e.Key, ref handled);
			e.Handled = handled;
		}
		internal void ProcessKeyUp(System.Windows.Input.Key key, ref bool handled) {
			if(!handled && key == System.Windows.Input.Key.Space) {
				if(IsChecked == false)
					IsChecked = true;
				else
					IsChecked = false;
				handled = true;
			}
		}
	}
	public class FilterCheckedTreeViewItem : CheckedTreeViewItem {
		static Binding menuBinding = (Binding)XamlHelper.LoadObjectCore(@"<Binding 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""    
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:dxb=""http://schemas.devexpress.com/winfx/2008/xaml/bars""
    xmlns:dxpgi=""http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid/internal""
Path=""(dxb:BarManager.DXContextMenu)"" RelativeSource=""{RelativeSource FindAncestor, AncestorType=dxpgi:FilterCheckedTreeView}""
/>
");
		bool lockUpdateTreeIsChecked;
		CoreXtraPivotGrid.PivotGroupFilterItem filterItem;
		public FilterCheckedTreeViewItem(CoreXtraPivotGrid.PivotGroupFilterItem filterItem)
			: base() {
			this.filterItem = filterItem;
			this.Header = filterItem;
			SetIsCheckedFromItem();
			lockUpdateTreeIsChecked = false;
			IsItemsLoaded = false;
			PivotGridPopupMenu.SetGridMenuType(this, PivotGridMenuType.FilterPopup);
			this.SetBinding(BarManager.DXContextMenuProperty, menuBinding);
		}
		public CoreXtraPivotGrid.PivotGroupFilterItem FilterItem { get { return filterItem; } }
		public virtual bool IsLastLevel { get { return FilterItem.Level == ParentTreeView.FilterItems.LevelCount - 1; } }
		protected internal FilterCheckedTreeView ParentTreeView { get { return GetParentTreeView(); } }
		protected FilterCheckedTreeViewItem ParentItem { get { return ItemsControl.ItemsControlFromItemContainer(this) as FilterCheckedTreeViewItem; } }
		protected internal bool IsItemsLoaded { get; set; }
		protected override DependencyObject GetContainerForItemOverride() {
			return new ContentPresenter();
		}
		#region Update Check State
		protected override void OnIsCheckedChanged(bool? newValue) {
			base.OnIsCheckedChanged(newValue);
			FilterItem.IsChecked = newValue;
			if(!lockUpdateTreeIsChecked)
				UpdateTreeNodesIsChecked(newValue);
		}
		protected virtual void SetIsCheckedFromItem() {
			SetIsChecked(filterItem.IsChecked);
		}
		protected void UpdateTreeNodesIsChecked(bool? value) {
			SetChildsChecked(value);
			if(ParentItem != null)
				ParentItem.UpdateCheckState(value);
			ParentTreeView.UpdateSelectAllItemIsChecked(value);
		}
		public void UpdateCheckState(bool? value) {
			bool? state = GetCheckState(value);
			if(state == IsChecked) return;
			SetIsChecked(state);
			if(ParentItem != null)
				ParentItem.UpdateCheckState(value);
		}
		protected internal void SetChildsChecked(bool? value) {
			SetIsChecked(value);
			if(IsItemsLoaded)
				for(int i = 0; i < Items.Count; i++)
					((FilterCheckedTreeViewItem)Items[i]).SetChildsChecked(value);
		}
		protected virtual bool? GetCheckState(bool? value) {
			if(value == null || !IsItemsLoaded || Items.Count == 0) return null;
			bool? state = ((FilterCheckedTreeViewItem)Items[0]).IsChecked;
			if(Items.Count > 1) {
				if(IsChecked != null && IsChecked != value) return null;
				foreach(FilterCheckedTreeViewItem item in Items)
					if(item.IsChecked != state) return null;
			}
			return state;
		}
		public void SetIsChecked(bool? value) {
			if(IsChecked == value) return;
			lockUpdateTreeIsChecked = true;
			IsChecked = value;
			lockUpdateTreeIsChecked = false;
		}
		#endregion
		#region Lazy items initialization
		protected override void OnCollapsed(RoutedEventArgs e) {
			base.OnCollapsed(e);
		}
		protected override void OnExpanded(RoutedEventArgs e) {
			if(IsExpanded && !IsItemsLoaded && !IsLastLevel) {
				FilterCheckedTreeView treeView = ParentTreeView;
				if(treeView.IsDeferredFilling) {
					treeView.StartAsyncAction(() => {
						LoadItems(treeView, () => {
							base.OnExpanded(e);
							treeView.OnEndLoadingItems();
						});
					});
				} else {
					LoadItems();
					base.OnExpanded(e);
				}
			}
		}
		public override void OnApplyTemplate() {
			LoadFilterItem();
			base.OnApplyTemplate();
		}
		FilterCheckedTreeView GetParentTreeView() {
			for(ItemsControl control = this; control != null; control = ItemsControl.ItemsControlFromItemContainer(control)) {
				FilterCheckedTreeView view = control as FilterCheckedTreeView;
				if(view != null) {
					return view;
				}
			}
			return null;
		}
		protected internal void LoadItems() {
			LoadItems(null, null);
		}
		protected internal virtual void LoadItems(FilterCheckedTreeView treeView, Action callback) {
			if(IsItemsLoaded) {
				callback();
				return;
			}
			FilterCheckedTreeView tree = treeView ?? ParentTreeView;
			if(tree.IsDeferredFilling) {
				tree.LoadSubItems(this, () => {
					IsItemsLoaded = true;
					callback();
				});
			} else {
				tree.LoadSubItems(this);
				IsItemsLoaded = true;
			}
		}
		void LoadFilterItem() {
			SetIsChecked(FilterItem.IsChecked);
			if(!IsItemsLoaded && !IsLastLevel)
				Items.Add("");
		}
		#endregion
	}
	public class PivotSelectAllTreeViewItem : FilterCheckedTreeViewItem {
		public PivotSelectAllTreeViewItem(CoreXtraPivotGrid.PivotGroupFilterItem filterItem)
			: base(filterItem) { }
		public override bool IsLastLevel { get { return true; } }
		protected override void OnIsCheckedChanged(bool? newValue) {
			EnsureThreeState(newValue);
			ParentTreeView.UpdatePopupOkButtonState(newValue);
			if(newValue == null) return;
			ItemCollection items = ParentTreeView.Items;
			for(int i = 1; i < items.Count; i++)
				((FilterCheckedTreeViewItem)items[i]).SetChildsChecked(newValue);
		}
		protected override void SetIsCheckedFromItem() { }
		protected override bool? GetCheckState(bool? value) { return ParentTreeView.FilterItems.VisibleCheckState; }
		protected internal override void LoadItems(FilterCheckedTreeView treeView = null, Action callback = null) { }
	}
	[DXToolboxBrowsable(false)]
	public class FilterCheckedTreeView : TreeView {
		public static readonly DependencyProperty IsItemsValidProperty;
		public static readonly DependencyProperty IsItemsLoadedProperty;
		public static readonly DependencyProperty ItemsHeaderTemplateProperty;
		static FilterCheckedTreeView() {
			Type ownerType = typeof(FilterCheckedTreeView);
			IsItemsValidProperty = DependencyPropertyManager.Register("IsItemsValid", typeof(bool), ownerType, new PropertyMetadata());
			IsItemsLoadedProperty = DependencyPropertyManager.Register("IsItemsLoaded", typeof(bool), ownerType, new PropertyMetadata());
			ItemsHeaderTemplateProperty = DependencyPropertyManager.Register("ItemsHeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata());
		}
		public FilterCheckedTreeView() { }
		protected internal PivotSelectAllTreeViewItem SelectAllItem { get; set; }
		protected internal virtual CoreXtraPivotGrid.PivotGroupFilterItems FilterItems { get { return OwnerEdit != null ? OwnerEdit.FilterItems as CoreXtraPivotGrid.PivotGroupFilterItems : null; } }
		public bool IsDeferredFilling { get { return OwnerEdit.IsDeferredFilling; } }
		public bool IsItemsValid {
			get { return (bool)GetValue(IsItemsValidProperty); }
			set { SetValue(IsItemsValidProperty, value); }
		}
		public bool IsItemsLoaded {
			get { return (bool)GetValue(IsItemsLoadedProperty); }
			set { SetValue(IsItemsLoadedProperty, value); }
		}
		public DataTemplate ItemsHeaderTemplate {
			get { return (DataTemplate)GetValue(ItemsHeaderTemplateProperty); }
			set { SetValue(ItemsHeaderTemplateProperty, value); }
		}
		internal protected virtual FilterPopupEdit OwnerEdit {
			get {
				return BaseEdit.GetOwnerEdit(this) as FilterPopupEdit;
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ContentPresenter();
		}
		protected CoreXtraPivotGrid.PivotGroupFilterItem CreateSelectAllFilterItem(string displayText) {
			return new CoreXtraPivotGrid.PivotGroupFilterItem(null, 0, displayText, displayText, FilterItems.VisibleCheckState);
		}
		public void UpdateSelectAllItemIsChecked(bool? value) {
			SelectAllItem.UpdateCheckState(value);
		}
		public void UpdatePopupOkButtonState(bool? newValue) {
			PopupBaseEditHelper.GetOkButton(OwnerEdit).IsEnabled = newValue != false;
		}
		public override void OnApplyTemplate() {
			EnsureItems();
			OwnerEdit.PopupTreeView = this;
		}
		protected override Size MeasureOverride(Size constraint) {
			EnsureItems();
			return base.MeasureOverride(constraint);
		}
		void EnsureItems() {
			if(IsItemsValid) return;
			if(OwnerEdit != null && FilterItems != null) {
				IsItemsValid = true;
				IsItemsLoaded = true;
				List<object> items = OwnerEdit.GetFirstLevelItems();
				SelectAllItem selectAllItem = items[0] as SelectAllItem;
				if(selectAllItem != null) {
					CoreXtraPivotGrid.PivotGroupFilterItem selectAllFilterItem = CreateSelectAllFilterItem(selectAllItem.DisplayText);
					selectAllFilterItem.IsChecked = FilterItems.VisibleCheckState;
					SelectAllItem = new PivotSelectAllTreeViewItem(selectAllFilterItem) { HeaderTemplate = ItemsHeaderTemplate };
					items[0] = SelectAllItem;
				}
				AssignItems(items, this);
			}
		}
		public void LoadSubItems(FilterCheckedTreeViewItem item, Action callback = null){
			List<object> items = FilterItems.GetChildValues(item.FilterItem.GetBranch());
			if(items != null) {
				AssignItems(items, item);
				if(callback != null)
					callback();
				return;
			}
			if(OwnerEdit.IsDeferredFilling) {
				FilterItems.LoadValuesAsync(item.FilterItem.GetBranch(), result => {
					AssignItems((List<object>)result.Value, item);
					if(callback != null)
						callback();
				});
			} else {
				items = FilterItems.LoadValues(item.FilterItem.GetBranch());
				AssignItems(items, item);
			}
		}
		void AssignItems(List<object> items, ItemsControl itemsControl) {
#if !SL
			itemsControl.BeginInit();
#endif
			itemsControl.Items.Clear();
			for(int i = 0; i < items.Count; i++)
				if(items[i] as TreeViewItem == null)
					itemsControl.Items.Add(new FilterCheckedTreeViewItem((CoreXtraPivotGrid.PivotGroupFilterItem)items[i]) { HeaderTemplate = ItemsHeaderTemplate });
				else
					itemsControl.Items.Add(items[i]);
#if !SL
			itemsControl.EndInit();
#endif
		}
		#region FilterPopupMenu
		public void InvertFilter() {
			if(SelectAllItem.IsChecked != null)
				SelectAllItem.IsChecked = !SelectAllItem.IsChecked;
			else
				InvertItemsCheckState(this);
		}
		public void CollapseAll(int level) {
			ChangeLevelIsExpanded(level, this, false);
		}
		public void ExpandAll(int level) {
			IsEnabled = false;
			ChangeLevelIsExpanded(level, this, true);
			if(lockExecute <= 0)
				IsEnabled = true;
		}
		void InvertItemsCheckState(ItemsControl itemsControl) {
			for(int i = 0; i < itemsControl.Items.Count; i++) {
				FilterCheckedTreeViewItem item = itemsControl.Items[i] as FilterCheckedTreeViewItem;
				if(item == null || item == SelectAllItem) continue;
				if(item.FilterItem.IsChecked != null)
					item.SetChildsChecked(!item.FilterItem.IsChecked);
				else {
					if(!item.IsItemsLoaded)
						StartAsyncAction(() => {
							item.LoadItems(this, () => {
								UnlockActions();
							});
						});
					StartAsyncAction(() => {
						InvertItemsCheckState(item);
						UnlockActions();
					});
				}
			}
		}
		void ChangeLevelIsExpanded(int level, ItemsControl itemsControl, bool isExpanded) {
			if(itemsControl is PivotSelectAllTreeViewItem)
				return;
			if(itemsControl is FilterCheckedTreeViewItem && ((FilterCheckedTreeViewItem)itemsControl).IsExpanded && !((FilterCheckedTreeViewItem)itemsControl).IsItemsLoaded)
				throw new NotImplementedException();
			for(int i = 0; i < itemsControl.Items.Count; i++) {
				FilterCheckedTreeViewItem item = itemsControl.Items[i] as FilterCheckedTreeViewItem;
				if(item == null) continue;
				if(item.FilterItem.Level == level) {
					item.IsExpanded = isExpanded;
					continue;
				} else {
					if(isExpanded && !item.IsExpanded)
						item.IsExpanded = true;
					if(isExpanded && item.IsItemsLoaded)
						ChangeLevelIsExpanded(level, item, isExpanded);
					else {
						StartAsyncAction(() => {
							ChangeLevelIsExpanded(level, item, isExpanded);
							UnlockActions();
						});
					}
				}
			}
		}
		#endregion
#if DEBUGTEST
		internal FilterPopupEdit GetOwnerEditAccess() {
			return OwnerEdit;
		}
#endif
		#region AsyncExpandCollapse
		int lockExecute;
		List<Action> actions;
		List<Action> Actions {
			get {
				if(actions == null)
					actions = new List<Action>();
				return actions;
			}
		}
		void LockActions() {
			lockExecute++;
			IsItemsLoaded = false;
			OwnerEdit.DisableOkButton();
		}
		void UnlockActions() {
			lockExecute--;
			if(lockExecute == 0 && Actions.Count == 0) {
				IsItemsLoaded = true;
				OwnerEdit.EnableOkButton();
				IsEnabled = true;
			}
			StartExecuteActions();
		}
		public void OnEndLoadingItems() {
			UnlockActions();
		}
		void ExecuteAction(Action action) {
			if(action != null) {
				LockActions();
				Dispatcher.BeginInvoke(action);
			}
		}
		void ExecuteNextAction() {
			Action action = Actions[0];
			Actions.RemoveAt(0);
			ExecuteAction(action);
		}
		internal void StartAsyncAction(Action action) {
			if(!IsDeferredFilling) {
				action.Invoke();
				return;
			}
			if(lockExecute > 0) {
				Actions.Add(action);
			} else {
				StartExecuteActions(action);
			}
		}
		void StartExecuteActions(Action action = null) {
			ExecuteAction(action);
			while(lockExecute == 0 && Actions.Count > 0) {
				ExecuteNextAction();
			}
		}
		public void ResetAsyncLoad() {
			Actions.Clear();
		}
		#endregion
	}
}
