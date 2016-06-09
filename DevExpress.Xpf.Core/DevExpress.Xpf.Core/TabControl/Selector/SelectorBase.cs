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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public static class ItemsControlSourceHelper {
		public static object AddNewItem(object itemsSource) {
			CheckItemsSource(itemsSource);
			if(itemsSource is IBindingList) {
				IBindingList bl = (IBindingList)itemsSource;
				return bl.AddNew();
			}
			Type itemType = GetItemsSourceItemType(itemsSource);
			object res = Activator.CreateInstance(itemType);
			AddItem(itemsSource, res);
			return res;
		}
		public static void AddItem(object itemsSource, object item) {
			CheckItemsSource(itemsSource);
			IList iList = (IList)itemsSource;
			iList.Add(item);
		}
		public static void InsertItem(object itemsSource, object item, int index) {
			CheckItemsSource(itemsSource);
			IList iList = (IList)itemsSource;
			iList.Insert(index, item);
		}
		public static void RemoveItem(object itemsSource, object item) {
			CheckItemsSource(itemsSource);
			IList iList = (IList)itemsSource;
			iList.Remove(item);
		}
		public static void MoveItem(object itemsSource, object item, int newIndex) {
			CheckItemsSource(itemsSource);
			GetItemsSourceItemType(itemsSource);
			IList iList = (IList)itemsSource;
			var moveMethod = itemsSource.GetType().GetMethod("Move", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(int), typeof(int) }, null);
			if(moveMethod != null) {
			   moveMethod.Invoke(itemsSource, new object[] { iList.IndexOf(item), newIndex });
				return;
			}
			RemoveItem(itemsSource, item);
			InsertItem(itemsSource, item, newIndex);
		}
		public static Type GetItemsSourceItemType(object itemsSource) {
			Type itemsSourceType = itemsSource.GetType();
			if(itemsSourceType.IsAssignableFrom(typeof(IEnumerable<>)))
				return itemsSourceType.GetGenericParameterConstraints()[0];
			var list = ((IEnumerable)itemsSource).Cast<object>();
			if(list.Count() > 0) return list.First().GetType();
			throw new InvalidOperationException("Cannot determine items type in the specified ItemsSource.");
		}
		static void CheckItemsSource(object itemsSource) {
			if(itemsSource is IList) return;
			throw new InvalidOperationException("Cannot add new item. The specified ItemsSource does not implement the IList/IBindingList interface.");
		}
	}
	public interface ISelectorBase {
		int SelectedIndex { get; set; }
		object SelectedItem { get; set; }
		ContentControl GetContainer(object item);
		ContentControl GetContainer(int index);
		event EventHandler SelectionChanged;
		event NotifyCollectionChangedEventHandler ItemsChanged;
	}
	public abstract class SelectorBase<TContainer, TItem> : ItemsControl, ISelectorBase
		where TContainer : SelectorBase<TContainer, TItem>
		where TItem : SelectorItemBase<TContainer, TItem> {
		#region Properties
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedIndexProperty =
			Selector.SelectedIndexProperty.AddOwner(typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
				-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
				(d, e) => ((SelectorBase<TContainer, TItem>)d).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue),
				(d, e) => ((SelectorBase<TContainer, TItem>)d).CoerceSelectedIndex((int)e)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedItemProperty =
			Selector.SelectedItemProperty.AddOwner(typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
			   null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
			   (d, e) => ((SelectorBase<TContainer, TItem>)d).OnSelectedItemChanged(e.OldValue, e.NewValue),
			   (d, e) => ((SelectorBase<TContainer, TItem>)d).CoerceSelectedItem(e)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedContainerProperty =
			DependencyProperty.Register("SelectedContainer", typeof(TItem), typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
				null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((SelectorBase<TContainer, TItem>)d).OnSelectedContainerChanged((TItem)e.OldValue, (TItem)e.NewValue),
				(d, e) => ((SelectorBase<TContainer, TItem>)d).CoerceSelectedContainer((FrameworkElement)e)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
			Selector.IsSynchronizedWithCurrentItemProperty.AddOwner(typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
				(bool?)null,
				(d, e) => ((SelectorBase<TContainer, TItem>)d).OnIsSynchronizedWithCurrentItemChanged((bool?)e.OldValue, (bool?)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedValueProperty =
			Selector.SelectedValueProperty.AddOwner(typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
				null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((SelectorBase<TContainer, TItem>)d).OnSelectedValueChanged(e.OldValue, e.NewValue),
				(d, e) => ((SelectorBase<TContainer, TItem>)d).CoerceSelectedValue(e)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedValuePathProperty =
			Selector.SelectedValuePathProperty.AddOwner(typeof(SelectorBase<TContainer, TItem>), new FrameworkPropertyMetadata(
				string.Empty,
				(d, e) => ((SelectorBase<TContainer, TItem>)d).OnSelectedValuePathChanged((string)e.OldValue, (string)e.NewValue)));
		static readonly DependencyPropertyKey SelectedItemContentPropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemContent", typeof(object), typeof(SelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemContentTemplatePropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemContentTemplate", typeof(DataTemplate), typeof(SelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemContentTemplateSelectorPropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemContentTemplateSelector", typeof(DataTemplateSelector), typeof(SelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemContentStringFormatPropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemContentStringFormat", typeof(String), typeof(SelectorBase<TContainer, TItem>), null);
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedItemContentProperty = SelectedItemContentPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedItemContentTemplateProperty = SelectedItemContentTemplatePropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedItemContentTemplateSelectorProperty = SelectedItemContentTemplateSelectorPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedItemContentStringFormatProperty = SelectedItemContentStringFormatPropertyKey.DependencyProperty;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItem { get { return GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TItem SelectedContainer { get { return (TItem)GetValue(SelectedContainerProperty); } set { SetValue(SelectedContainerProperty, value); } }
		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? IsSynchronizedWithCurrentItem { get { return (bool?)GetValue(IsSynchronizedWithCurrentItemProperty); } set { SetValue(IsSynchronizedWithCurrentItemProperty, value); } }
		public object SelectedValue { get { return GetValue(SelectedValueProperty); } set { SetValue(SelectedValueProperty, value); } }
		public string SelectedValuePath { get { return (string)GetValue(SelectedValuePathProperty); } set { SetValue(SelectedValuePathProperty, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItemContent { get { return GetValue(SelectedItemContentProperty); } protected set { SetValue(SelectedItemContentPropertyKey, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplate SelectedItemContentTemplate { get { return (DataTemplate)GetValue(SelectedItemContentTemplateProperty); } protected set { SetValue(SelectedItemContentTemplatePropertyKey, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplateSelector SelectedItemContentTemplateSelector { get { return (DataTemplateSelector)GetValue(SelectedItemContentTemplateSelectorProperty); } protected set { SetValue(SelectedItemContentTemplateSelectorPropertyKey, value); } }
		public String SelectedItemContentStringFormat { get { return (String)GetValue(SelectedItemContentStringFormatProperty); } protected set { SetValue(SelectedItemContentStringFormatPropertyKey, value); } }
		#endregion Properties
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); } }
		List<DependencyObject> logicalChildren = new List<DependencyObject>();
		protected void OnLogicalElementTemplateChanged(DependencyProperty property, DependencyPropertyKey propertyKey, DataTemplate template) {
			GetValue(property).Do(x => {
				RemoveLogicalChild(x);
				logicalChildren.Remove(x as DependencyObject);
			});
			SetValue(propertyKey, template.Return(x => x.LoadContent(), null));
			GetValue(property).Do(x => {
				logicalChildren.Add(x as DependencyObject);
				AddLogicalChild(x);
			});
		}
		bool lockSelectedItemChanged = false;
		bool lockSelectedContainerChanged = false;
		bool lockSelectedValueChanged = false;
		protected virtual object CoerceSelectedIndex(int value) { return value; }
		protected virtual object CoerceSelectedItem(object value) { return value; }
		protected virtual object CoerceSelectedContainer(FrameworkElement value) { return value; }
		protected virtual object CoerceSelectedValue(object value) { return value; }
		protected virtual void OnSelectedIndexChanged(int oldValue, int newValue) {
			var oldSelectedItem = SelectedItem;
			lockSelectedItemChanged = true;
			lockSelectedContainerChanged = true;
			lockSelectedValueChanged = true;
			if(IsIndexInRange(newValue)) {
				SetCurrentValue(SelectedItemProperty, Items[newValue]);
				SetCurrentValue(SelectedContainerProperty, GetContainer(newValue));
				SetCurrentValue(SelectedValueProperty, DataAccessDescriptor.GetValue(SelectedItem));
			} else {
				SetCurrentValue(SelectedItemProperty, null);
				SetCurrentValue(SelectedContainerProperty, null);
				SetCurrentValue(SelectedValueProperty, null);
			}
			lockSelectedValueChanged = false;
			lockSelectedContainerChanged = false;
			lockSelectedItemChanged = false;
			UpdateSelectionProperties();
			if(IsSynchronizedWithCurrentItem.HasValue && IsSynchronizedWithCurrentItem.Value) {
				if(IsIndexInRange(SelectedIndex))
					Items.MoveCurrentToPosition(SelectedIndex);
				else Items.MoveCurrentTo(SelectedItem);
			}
			if(oldSelectedItem != SelectedItem) RaiseSelectionChanged(oldValue, newValue, oldSelectedItem, SelectedItem);
		}
		protected virtual void OnSelectedItemChanged(object oldValue, object newValue) {
			if(oldValue == newValue || lockSelectedItemChanged) return;
			var oldSelectedIndex = SelectedIndex;
			SetCurrentValue(SelectedIndexProperty, IndexOf(newValue));
			RaiseSelectionChanged(oldSelectedIndex, SelectedIndex, oldValue, newValue);
		}
		protected virtual void OnSelectedContainerChanged(TItem oldValue, TItem newValue) {
			oldValue.Do(x => x.SetIsSelectedInternal(false, true));
			newValue.Do(x => x.SetIsSelectedInternal(true, true));
			if(lockSelectedContainerChanged) return;
			SetCurrentValue(SelectedIndexProperty, IndexOf(newValue));
		}
		protected virtual void OnSelectedValueChanged(object oldValue, object newValue) {
			if(lockSelectedValueChanged) return;
			int index = GetIndex(x => object.Equals(DataAccessDescriptor.GetValue(x), newValue));
			SetCurrentValue(SelectedIndexProperty, index);
		}
		protected virtual void OnSelectedValuePathChanged(string oldValue, string newValue) {
			DataAccessDescriptor = new SelectorPropertyDescriptor(newValue);
			lockSelectedValueChanged = true;
			SetCurrentValue(SelectedValueProperty, DataAccessDescriptor.GetValue(SelectedItem));
			lockSelectedValueChanged = false;
		}
		internal void SetSelectedContainer(TItem value) {
			SetCurrentValue(SelectedContainerProperty, value);
		}
		protected internal virtual void UpdateSelectionProperties() {
			if(!IsIndexInRange(SelectedIndex)) {
				SelectedItemContent = null;
				SelectedItemContentTemplate = null;
				SelectedItemContentTemplateSelector = null;
				SelectedItemContentStringFormat = null;
				return;
			}
			TItem container = SelectedContainer; 
			if(container == null) return;
			container.Do(x => x.SetIsSelectedInternal(true, true));
			FrameworkElement containerPanel = VisualTreeHelper.GetParent(container) as FrameworkElement;
			SelectedItemContent = container.Content;
			if(container.ContentTemplate != null || container.ContentTemplateSelector != null || container.ContentStringFormat != null) {
				SelectedItemContentTemplate = container.ContentTemplate;
				SelectedItemContentTemplateSelector = container.ContentTemplateSelector;
				SelectedItemContentStringFormat = container.ContentStringFormat;
			} else {
				SelectedItemContentTemplate = ItemTemplate;
				SelectedItemContentTemplateSelector = ItemTemplateSelector;
				SelectedItemContentStringFormat = ItemStringFormat;
			}
		}
		EventHandler selectionChanged;
		event EventHandler ISelectorBase.SelectionChanged { add { selectionChanged += value; } remove { selectionChanged -= value; } }
		protected virtual void RaiseSelectionChanging(int oldIndex, int newIndex, object oldItem, object newItem, CancelEventArgs e) { }
		protected virtual void RaiseSelectionChanged(int oldIndex, int newIndex, object oldItem, object newItem) {
			selectionChanged.Do(x => x(this, EventArgs.Empty));
		}
		protected virtual void RaiseItemAdding(out object item, CancelEventArgs e) { item = null; }
		protected virtual void RaiseItemAdded(int index, object item) { }
		protected virtual void RaiseItemInserting(int index, object item, CancelEventArgs e) { }
		protected virtual void RaiseItemInserted(int index, object item) { }
		protected virtual void RaiseItemRemoving(int index, object item, CancelEventArgs e) { }
		protected virtual void RaiseItemRemoved(int index, object item) { }
		protected virtual void RaiseItemMoving(int oldIndex, int newIndex, object item, CancelEventArgs e) { }
		protected virtual void RaiseItemMoved(int oldIndex, int newIndex, object item) { }
		protected virtual void RaiseItemHiding(int index, object item, CancelEventArgs e) { }
		protected virtual void RaiseItemHidden(int index, object item) { }
		protected virtual void RaiseItemShowing(int index, object item, CancelEventArgs e) { }
		protected virtual void RaiseItemShown(int index, object item) { }
		SelectorPropertyDescriptor DataAccessDescriptor;
		public SelectorBase() {
			DataAccessDescriptor = new SelectorPropertyDescriptor(SelectedValuePath);
			IsVisibleChanged += OnIsVisibleChanged;
			IsEnabledChanged += OnIsEnabledChanged;
			Items.CurrentChanged += OnCurrentChanged;
			ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
		}
		public int IndexOf(object item) {
			if(item == null) return -1;
			if(Items.Contains(item))
				return Items.IndexOf(item);
			if(IsItemItsOwnContainerOverride(item))
				return ItemContainerGenerator.IndexFromContainer((TItem)item);
			return -1;
		}
		public bool IsIndexInRange(int index) {
			return index >= 0 && index < Items.Count;
		}
		protected int GetIndex(Func<object, bool> comparer) {
			for(int i = 0; i < Items.Count; i++) {
				if(comparer(Items[i])) return i;
			}
			return -1;
		}
		protected virtual bool GetIsNavigationInversed(FlowDirection flowDirection) {
			return flowDirection == FlowDirection.RightToLeft;
		}
		protected internal void SelectNextItem(bool cycle = false) {
			int newSelIndex = CalcSelectNextIndex(GetIsNavigationInversed(FlowDirection));
			if(newSelIndex == -1 && cycle) {
				SelectFirstItem();
				return;
			}
			NavigateCore(newSelIndex);
		}
		protected internal void SelectPrevItem(bool cycle = false) {
			int newSelIndex = CalcSelectNextIndex(!GetIsNavigationInversed(FlowDirection));
			if(newSelIndex == -1 && cycle) {
				SelectLastItem();
				return;
			}
			NavigateCore(newSelIndex);
		}
		protected internal void SelectFirstItem() {
			int newSelIndex = CalcSelectFirstIndex(GetIsNavigationInversed(FlowDirection));
			NavigateCore(newSelIndex);
		}
		protected internal void SelectLastItem() {
			int newSelIndex = CalcSelectFirstIndex(!GetIsNavigationInversed(FlowDirection));
			NavigateCore(newSelIndex);
		}
		protected internal bool CanSelectNextItem(bool cycle = false) {
			int newSelIndex = CalcSelectNextIndex(GetIsNavigationInversed(FlowDirection));
			if(newSelIndex == -1 && cycle)
				newSelIndex = CalcSelectFirstIndex();
			return newSelIndex != -1;
		}
		protected internal bool CanSelectPrevItem(bool cycle = false) {
			int newSelIndex = CalcSelectNextIndex(!GetIsNavigationInversed(FlowDirection));
			if(newSelIndex == -1 && cycle)
				newSelIndex = CalcSelectFirstIndex(true);
			return newSelIndex != -1;
		}
		void NavigateCore(int index) {
			if(index != -1 && index != SelectedIndex)
				SelectItem(index);
		}
		int CalcSelectNextIndex(bool IfTrueMovePrev = false) {
			if(SelectedIndex == -1)
				IfTrueMovePrev = false;
			int newSelectedIndex = IfTrueMovePrev ? GetNotHiddenItemIndexBefore(SelectedIndex) : GetNotHiddenItemIndexAfter(SelectedIndex);
			if(IfTrueMovePrev)
				return newSelectedIndex < SelectedIndex ? newSelectedIndex : -1;
			else return newSelectedIndex > SelectedIndex ? newSelectedIndex : -1;
		}
		int CalcSelectFirstIndex(bool IfTrueMoveLast = false) {
			if(!IfTrueMoveLast) {
				for(int i = 0; i < Items.Count; i++)
					if(IsVisibleAndEnabledItem(GetContainer(i))) return i;
			} else {
				for(int i = Items.Count - 1; i >= 0; i--)
					if(IsVisibleAndEnabledItem(GetContainer(i))) return i;
			}
			return -1;
		}
		internal int GetNotHiddenItemIndexBefore(int index) {
			int cur = index - 1;
			while(IsIndexInRange(cur)) {
				var tab = GetContainer(cur);
				if(tab != null && IsVisibleAndEnabledItem(tab))
					return cur;
				cur--;
			}
			return -1;
		}
		internal int GetNotHiddenItemIndexAfter(int index) {
			int cur = index + 1;
			while(IsIndexInRange(cur)) {
				var tab = GetContainer(cur);
				if(tab != null && IsVisibleAndEnabledItem(tab))
					return cur;
				cur++;
			}
			return -1;
		}
		protected internal object AddNewItem() {
			object newItem;
			CancelEventArgs args = new CancelEventArgs();
			RaiseItemAdding(out newItem, args);
			if(args.Cancel) return null;
			if(newItem != null) {
				InsertItemCore(newItem, Items.Count);
			} else {
				if(ItemsSource != null)
					newItem = ItemsControlSourceHelper.AddNewItem(ItemsSource);
				else Items.Add(newItem = CreateContainerForNewItem());
			}
			RaiseItemAdded(IndexOf(newItem), newItem);
			SelectItem(newItem);
			return newItem;
		}
		protected internal void InsertItem(object item, int index) {
			CancelEventArgs args = new CancelEventArgs();
			RaiseItemInserting(index, item, args);
			if(args.Cancel || Items.Contains(item)) return;
			InsertItemCore(item, index);
			RaiseItemInserted(index, item);
		}
		void InsertItemCore(object item, int index) {
			if(Items.Contains(item)) return;
			if(index == -1) index = 0;
			if(ItemsSource != null) {
				ItemsControlSourceHelper.InsertItem(ItemsSource, item, index);
				return;
			}
			Items.Insert(index, item);
		}
		protected internal void RemoveItem(object item) {
			RemoveItem(IndexOf(item));
		}
		protected internal void RemoveItem(int index) {
			if(!IsIndexInRange(index)) return;
			object item = Items[index];
			CancelEventArgs args = new CancelEventArgs();
			RaiseItemRemoving(index, item, args);
			if(args.Cancel || !Items.Contains(item)) return;
			if(ItemsSource != null)
				ItemsControlSourceHelper.RemoveItem(ItemsSource, item);
			else Items.RemoveAt(index);
			RaiseItemRemoved(index, item);
		}
		protected internal void MoveItem(object item, int index) {
			MoveItem(IndexOf(item), index);
		}
		protected internal void MoveItem(int oldIndex, int newIndex) {
			if(!IsIndexInRange(oldIndex) || oldIndex == newIndex) return;
			object item = Items[oldIndex];
			CancelEventArgs args = new CancelEventArgs();
			RaiseItemMoving(oldIndex, newIndex, item, args);
			if(args.Cancel || Items.IndexOf(item) == newIndex) return;
			if(ItemsSource != null)
				ItemsControlSourceHelper.MoveItem(ItemsSource, item, newIndex);
			else {
				lockItemsChanged = true;
				Items.RemoveAt(oldIndex);
				Items.Insert(newIndex, item);
				lockItemsChanged = false;
				OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
			}
			RaiseItemMoved(oldIndex, newIndex, item);
		}
		protected internal void ShowItem(object item, bool raiseEvents = true) {
			ShowItem(IndexOf(item), raiseEvents);
		}
		protected internal void ShowItem(int index, bool raiseEvents = true) {
			SetItemVisibility(index, Visibility.Visible, RaiseItemShowing, RaiseItemShown, raiseEvents);
			SelectItem(index);
		}
		protected internal void HideItem(object item, bool raiseEvents = true) {
			HideItem(IndexOf(item), raiseEvents);
		}
		protected internal void HideItem(int index, bool raiseEvents = true) {
			SetItemVisibility(index, Visibility.Collapsed, RaiseItemHiding, RaiseItemHidden, raiseEvents);
		}
		void SetItemVisibility(int index, Visibility visibility, Action<int, object, CancelEventArgs> raiseCancelableEvent, Action<int, object> raiseCompletedEvent, bool raiseEvents) {
			if(!IsIndexInRange(index)) return;
			object item = Items[index];
			TItem container = GetContainer(index);
			if(container == null || container.Visibility == visibility) return;
			CancelEventArgs args = new CancelEventArgs();
			if(raiseEvents) raiseCancelableEvent(index, item, args);
			container = GetContainer(item);
			if(args.Cancel || container == null) return;
			container.SetCurrentValue(SelectorItemBase<TContainer, TItem>.VisibilityProperty, visibility);
			if(raiseEvents) raiseCompletedEvent(index, item);
		}
		ContentControl ISelectorBase.GetContainer(object item) { return GetContainer(item); }
		ContentControl ISelectorBase.GetContainer(int index) { return GetContainer(index); }
		protected internal TItem GetContainer(object item) {
			return GetContainer(IndexOf(item));
		}
		protected internal TItem GetContainer(int index) {
			if(!IsIndexInRange(index)) return null;
			var item = Items[index];
			if(item is TItem) return (TItem)item;
			return (TItem)ItemContainerGenerator.ContainerFromItem(item);
		}
		protected internal IEnumerable<TItem> GetContainers() {
			return Items.OfType<object>().Select(GetContainer).Where(x => x != null);
		}
		protected internal void SelectItem(object item) {
			SelectItem(IndexOf(item));
		}
		protected internal void SelectItem(int index, bool forceRaiseSelectionChanging = false) {
			index = IsIndexInRange(index) ? index : -1;
			var newSelectedItem = IsIndexInRange(index) ? Items[index] : null;
			if(SelectedIndex != index) {
				if(IsInitialized && (forceRaiseSelectionChanging || SelectedItem != newSelectedItem)) {
					CancelEventArgs args = new CancelEventArgs();
					RaiseSelectionChanging(SelectedIndex, index, SelectedItem, newSelectedItem, args);
					if(args.Cancel) {
						OnSelectedIndexChanged(SelectedIndex, SelectedIndex);
						return;
					}
				}
				SetCurrentValue(SelectedIndexProperty, index);
			} else
				OnSelectedIndexChanged(SelectedIndex, SelectedIndex);
		}
		protected internal virtual bool IsVisibleAndEnabledItem(TItem item) {
			if(item == null) return false;
			return item.Visibility == Visibility.Visible && item.DefinedIsEnabled;
		}
		protected virtual int GetCoercedSelectedIndex(int index, NotifyCollectionChangedAction? originativeAction) {
			index = Math.Max(0, index);
			index = Math.Min(Items.Count - 1, index);
			return GetCoercedSelectedIndexCore(GetContainers(), index);
		}
		internal int GetCoercedSelectedIndexCore(IEnumerable<TItem> containers, int index) {
			var _containers = containers.ToList();
			if(_containers.Count == 0) return index;
			for(int i = index; i < _containers.Count; i++)
				if(IsVisibleAndEnabledItem(_containers[i])) return i;
			for(int i = index; i >= 0; i--)
				if(IsVisibleAndEnabledItem(_containers[i])) return i;
			return -1;
		}
		public event NotifyCollectionChangedEventHandler ItemsChanged;
		protected virtual void OnCurrentChanged(object sender, EventArgs e) {
			if(IsSynchronizedWithCurrentItem.GetValueOrDefault())
				SetCurrentValue(SelectedIndexProperty, Items.CurrentPosition);
		}
		protected virtual void OnIsSynchronizedWithCurrentItemChanged(bool? oldValue, bool? newValue) {
			if(newValue.HasValue && newValue.Value)
				SetCurrentValue(SelectedIndexProperty, Items.CurrentPosition);
		}
		bool lockItemsChanged = false;
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(lockItemsChanged) return;
			if(e.OldItems != null) foreach(var item in e.OldItems) (item as TItem).Do(ClearContainer);
			if(e.NewItems != null) foreach(var item in e.NewItems) (item as TItem).Do(InitContainer);
			base.OnItemsChanged(e);
			ItemsChanged.Do(x => x(this, e));
			if(e.Action == NotifyCollectionChangedAction.Add)
				OnAddItem(e.NewItems[0], e.NewStartingIndex);
			else if(e.Action == NotifyCollectionChangedAction.Remove)
				OnRemoveItem(e.OldItems[0], e.OldStartingIndex);
			else if(e.Action == NotifyCollectionChangedAction.Replace)
				OnReplaceItem(e.OldItems[0], e.NewItems[0], e.NewStartingIndex);
			if(e.Action == NotifyCollectionChangedAction.Move)
				OnMoveItem(e.NewItems[0], e.OldStartingIndex, e.NewStartingIndex);
			else if(e.Action == NotifyCollectionChangedAction.Reset)
				OnResetItem();
		}
		protected virtual void OnAddItem(object newItem, int index) {
			TItem container = GetContainer(index);
			if(container != null && container.IsSelected) {
				SelectItem(index);
				return;
			}
			if(SelectedIndex == index) {
				SelectItem(container ?? newItem);
				return;
			}
			if(SelectedIndex == -1) {
				InitSelection(index, IsInitialized);
				return;
			}
		}
		protected virtual void OnRemoveItem(object oldItem, int index) {
			SelectItem(GetCoercedSelectedIndex(SelectedIndex, NotifyCollectionChangedAction.Remove), SelectedItem == null);
		}
		protected virtual void OnReplaceItem(object oldItem, object newItem, int index) {
			if(SelectedIndex == index)
				SelectItem(index);
		}
		protected virtual void OnMoveItem(object item, int oldIndex, int newIndex) {
			if(SelectedItem == item)
				SelectItem(newIndex);
		}
		protected virtual void OnResetItem() {
			if(Items.Count == 0) {
				SelectItem(SelectedIndex, SelectedItem == null);
				return;
			}
			if(SelectedIndex == -1 && SelectedItem == null) {
				InitSelection();
				return;
			}
			if(SelectedIndex != -1) {
				SelectItem(SelectedIndex, SelectedItem == null);
				return;
			}
			if(SelectedItem != null) {
				SelectItem(SelectedItem);
				return;
			}
		}
		protected internal virtual void OnContainerVisibilityChanged(TItem container, Visibility oldValue, Visibility newValue) {
			if(container == SelectedContainer && newValue == Visibility.Collapsed)
				SelectItem(GetCoercedSelectedIndex(SelectedIndex, NotifyCollectionChangedAction.Remove));
		}
		protected internal virtual void OnContainerIsEnabledChanged(TItem container, bool oldValue, bool newValue) { }
		bool IsSelectionInitialized = false;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IsSelectionInitialized = true;
			InitSelection();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			InitSelection(0, true);
		}
		protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) { }
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) { }
		void InitSelection(int index = 0, bool force = false) {
			if(SelectedIndex == -1 && Items.Count > 0 && (IsSelectionInitialized || force))
				SelectItem(GetCoercedSelectedIndex(index, null));
			UpdateSelectionProperties();
		}
		protected virtual void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e) {
			if(ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return;
			if(SelectedItem != null && Items.Contains(SelectedItem))
				this.SetCurrentValue(SelectedContainerProperty, GetContainer(SelectedItem));
			UpdateSelectionProperties();
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return CreateContainer();
		}
		protected abstract TItem CreateContainer();
		protected virtual TItem CreateContainerForNewItem() {
			var res = CreateContainer();
			res.Content = "New Item";
			return res;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is TItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			InitContainer((TItem)element);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			ClearContainer((TItem)element);
		}
		void InitContainer(TItem container) {
			container.Assign((TContainer)this);
		}
		void ClearContainer(TItem container) {
			container.Assign(null);
			container.ClearValue(Selector.IsSelectedProperty);
		}
	}
	public abstract class SelectorItemBase<TContainer, TItem> : ContentControl
		where TContainer : SelectorBase<TContainer, TItem>
		where TItem : SelectorItemBase<TContainer, TItem> {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsSelectedProperty =
			Selector.IsSelectedProperty.AddOwner(typeof(SelectorItemBase<TContainer, TItem>), new FrameworkPropertyMetadata(false,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.Journal,
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue),
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).CoerceIsSelected((bool)e)));
		static readonly DependencyPropertyKey OwnerPropertyKey = DependencyProperty.RegisterReadOnly("Owner", typeof(TContainer), typeof(SelectorItemBase<TContainer, TItem>),
			new FrameworkPropertyMetadata((d, e) => ((SelectorItemBase<TContainer, TItem>)d).OnOwnerChanged((TContainer)e.OldValue, (TContainer)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public TContainer Owner {
			get { return (TContainer)GetValue(OwnerProperty); }
			private set { SetValue(OwnerPropertyKey, value); }
		}
		bool? definedIsEnabled = null;
		internal bool DefinedIsEnabled { get { return definedIsEnabled ?? IsEnabled; } private set { definedIsEnabled = value; } }
		static SelectorItemBase() {
			VisibilityProperty.OverrideMetadata(typeof(SelectorItemBase<TContainer, TItem>), new PropertyMetadata(Visibility.Visible,
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).OnVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue),
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).CoerceVisibility((Visibility)e)));
			IsEnabledProperty.OverrideMetadata(typeof(SelectorItemBase<TContainer, TItem>), new UIPropertyMetadata(
				true,
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).OnIsEnabledChanged((bool)e.OldValue, (bool)e.NewValue),
				(d, e) => ((SelectorItemBase<TContainer, TItem>)d).CoerceIsEnabled((bool)e)));
			EventManager.RegisterClassHandler(typeof(SelectorItemBase<TContainer, TItem>), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
		}
		static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			if(e.Handled || e.Scope != null) return;
			TItem item = sender as TItem;
			if(e.Target == null)
				e.Target = item;
			else if(!item.IsSelected) {
				e.Scope = item;
				e.Handled = true;
			}
		}
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
			Owner.Do(x => x.SelectItem(this));
		}
		public SelectorItemBase() {
			Loaded += OnLoaded;
		}
		internal void Assign(TContainer owner) {
			Owner = owner;
		}
		bool lockIsSelectedChanged = false;
		internal void SetIsSelectedInternal(bool value, bool lockIsSelectedChanged) {
			this.lockIsSelectedChanged = lockIsSelectedChanged;
			SetCurrentValue(IsSelectedProperty, value);
			this.lockIsSelectedChanged = false;
		}
		protected virtual object CoerceIsSelected(bool value) { return value; }
		protected virtual object CoerceVisibility(Visibility value) { return value; }
		protected virtual object CoerceIsEnabled(bool value) {
			DefinedIsEnabled = value;
			if(!value || Owner == null) return value;
			return Owner.IsEnabled;
		}
		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue) {
			if(lockIsSelectedChanged) return;
			if(newValue)
				Owner.Do(x => x.SetSelectedContainer((TItem)this));
			else Owner.Do(x => x.SetSelectedContainer(null));
		}
		protected virtual void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
			Owner.Do(x => x.OnContainerVisibilityChanged((TItem)this, oldValue, newValue));
		}
		protected virtual void OnIsEnabledChanged(bool oldValue, bool newValue) {
			Owner.Do(x => x.OnContainerIsEnabledChanged((TItem)this, (bool)oldValue, (bool)newValue));
		}
		protected virtual void OnOwnerChanged(TContainer oldValue, TContainer newValue) {
			if(IsSelected) Owner.Do(x => x.SetSelectedContainer((TItem)this));
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			if(oldContent != newContent) UpdateOwnerSelectionProperties();
		}
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			if(oldContentTemplate != newContentTemplate) UpdateOwnerSelectionProperties();
		}
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			if(oldContentTemplateSelector != newContentTemplateSelector) UpdateOwnerSelectionProperties();
		}
		protected override void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat) {
			base.OnContentStringFormatChanged(oldContentStringFormat, newContentStringFormat);
			if(oldContentStringFormat != newContentStringFormat) UpdateOwnerSelectionProperties();
		}
		protected void UpdateOwnerSelectionProperties() {
			Owner.Do(x => x.UpdateSelectionProperties());
		}
	}
}
namespace DevExpress.Xpf.Core {
	public class SelectorSelectionChangedEventArgs : RoutedEventArgs {
		public IEnumerable<object> RemovedItems { get; private set; }
		public IEnumerable<int> RemovedIndexes { get; private set; }
		public IEnumerable<object> AddedItems { get; private set; }
		public IEnumerable<int> AddedIndexes { get; private set; }
		public object RemovedItem { get { return RemovedItems.With(x => x.FirstOrDefault()); } }
		public object AddedItem { get { return AddedItems.With(x => x.FirstOrDefault()); } }
		public SelectorSelectionChangedEventArgs(RoutedEvent routedEvent, object source, 
			IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes) 
			: base(routedEvent, source) {
				Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		public SelectorSelectionChangedEventArgs(RoutedEvent routedEvent,
			IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes)
			: base(routedEvent) {
			Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		public SelectorSelectionChangedEventArgs(IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes) {
			Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		void Init(IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes) {
			RemovedItems = removedItems;
			RemovedIndexes = removedIndexes;
			AddedItems = addedItems;
			AddedIndexes = addedIndexes;
		}
	}
	public class SelectorSelectionChangingEventArgs : CancelRoutedEventArgs {
		public IEnumerable<object> RemovedItems { get; private set; }
		public IEnumerable<int> RemovedIndexes { get; private set; }
		public IEnumerable<object> AddedItems { get; private set; }
		public IEnumerable<int> AddedIndexes { get; private set; }
		public object RemovedItem { get { return RemovedItems.With(x => x.FirstOrDefault()); } }
		public object AddedItem { get { return AddedItems.With(x => x.FirstOrDefault()); } }
		public SelectorSelectionChangingEventArgs(RoutedEvent routedEvent, object source,
			IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes)
			: base(routedEvent, source) {
			Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		public SelectorSelectionChangingEventArgs(RoutedEvent routedEvent,
			IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes)
			: base(routedEvent) {
			Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		public SelectorSelectionChangingEventArgs(IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes) {
			Init(removedItems, removedIndexes, addedItems, addedIndexes);
		}
		void Init(IEnumerable<object> removedItems, IEnumerable<int> removedIndexes, IEnumerable<object> addedItems, IEnumerable<int> addedIndexes) {
			RemovedItems = removedItems;
			RemovedIndexes = removedIndexes;
			AddedItems = addedItems;
			AddedIndexes = addedIndexes;
		}
	}
	public delegate void SelectorSelectionChangedEventHandler(object sender, SelectorSelectionChangedEventArgs e);
	public delegate void SelectorSelectionChangingEventHandler(object sender, SelectorSelectionChangingEventArgs e);
}
