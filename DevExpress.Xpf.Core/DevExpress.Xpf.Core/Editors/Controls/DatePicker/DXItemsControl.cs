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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors {
	public class DXItemsControlOutOfRangeItemEventArgs : EventArgs {
		public int Index { get; private set; }
		public object Item { get; set; }
		public bool Handled { get; set; }
		public DXItemsControlOutOfRangeItemEventArgs(int index) {
			Index = index;
		}
	}
	public interface IDXItemContainerGenerator {
		int GetItemsCount();
		UIElement Generate(int index, out bool isNew);
		UIElement GetContainer(int index);
		void PrepareItemContainer(int index, UIElement container);
		void ClearItemContainer(int index, UIElement container);
		void RemoveItems();
		void StartManipulation();
		void StopManipulation();
		void StartAt(int index);
		void Stop();
	}
	public class DXItemsControl : ItemsControl, IDXItemContainerGenerator {
		public const int InvalidHandle = Int32.MinValue;
		public static readonly DependencyProperty IsLoopedProperty;
		static DXItemsControl() {
			Type ownerType = typeof(DXItemsControl);
			IsLoopedProperty = DependencyPropertyManager.Register("IsLooped", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((DXItemsControl)d).IsLoopedChanged((bool)e.NewValue)));
		}
		public DXItemsControl() {
			DefaultStyleKey = typeof(DXItemsControl);
			Loaded += OnLoaded;
			Initialize();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
		}
		public bool IsLooped {
			get { return (bool)GetValue(IsLoopedProperty); }
			set { SetValue(IsLoopedProperty, value); }
		}
		const int MaxRecycledCount = 20;
		bool IsInItemGeneration { get; set; }
		Dictionary<int, object> OutOfRangeData { get; set; }
		public event EventHandler<DXItemsControlOutOfRangeItemEventArgs> ProcessOutOfRangeItem;
		Dictionary<int, FrameworkElement> InternalIndex2Item { get; set; }
		Dictionary<int, FrameworkElement> PreviousInternalIndex2Item { get; set; }
		RecycledCollection<FrameworkElement> Recycled { get; set; }
		protected LoopedPanel Panel { get; private set; }
		protected DXScrollViewer ScrollViewer { get; private set; }
		public override void OnApplyTemplate() {
			ScrollViewer.Do(x => {
				x.ScrollChanged -= OnScrollChanged;
				x.ViewChanged -= OnViewChanged;
			});
			base.OnApplyTemplate();
			ScrollViewer = (DXScrollViewer)LayoutHelper.FindElement(this, element => element is DXScrollViewer);
			ScrollViewer.Do(x => {
				x.ScrollChanged += OnScrollChanged;
				x.ViewChanged += OnViewChanged;
				x.IsLooped = IsLooped;
			});
			if (ScrollViewer != null) {
				Panel = ScrollViewer.Content as LoopedPanel;
				if (Panel != null) {
					Panel.ScrollOwner = ScrollViewer;
					Panel.IsLooped = IsLooped;
				}
			}
			else {
				Panel = (LoopedPanel)LayoutHelper.FindElement(this, element => element is LoopedPanel);
			}
			if (Panel == null)
				throw new NotSupportedException();
			Panel.ItemsContainerGenerator = this;
		}
		protected virtual void OnViewChanged(object sender, ViewChangedEventArgs e) {
		}
		protected virtual void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
		}
		protected virtual void ItemTemplateSelectorChanged(DataTemplateSelector newValue) {
			InvalidatePanel();
		}
		protected virtual void IsLoopedChanged(bool newValue) {
			if (Panel != null)
				Panel.IsLooped = newValue;
		}
		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
			OutOfRangeData.Clear();
			InvalidatePanel();
		}
		protected virtual void ItemContainerStyleChanged(Style newValue) {
			InvalidatePanel();
		}
		void Initialize() {
			InternalIndex2Item = new Dictionary<int, FrameworkElement>();
			PreviousInternalIndex2Item = new Dictionary<int, FrameworkElement>();
			Recycled = new RecycledCollection<FrameworkElement>();
			OutOfRangeData = new Dictionary<int, object>();
		}
		protected virtual void ItemTemplateChanged(DataTemplate template) {
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ContentPresenter();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			(element as FrameworkElement).If(x => ItemContainerStyle != null && x.GetType() == ItemContainerStyle.TargetType).Do(x => x.Style = ItemContainerStyle);
			(element as ContentPresenter).Do(x => {
				x.Content = item;
				x.ContentTemplate = GetItemTemplate(item);
			});
			(element as ContentControl).Do(x => {
				x.Content = item;
				x.ContentTemplate = GetItemTemplate(item);
			});
		}
		protected DataTemplate GetItemTemplate(object item) {
			return ItemTemplateSelector != null ? ItemTemplateSelector.SelectTemplate(item, this) : ItemTemplate;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			(element as FrameworkElement).Do(x => x.Style = null);
			(element as ContentPresenter).Do(x => {
				x.Content = null;
				x.ContentTemplate = null;
			});
			(element as ContentControl).Do(x => {
				x.Content = null;
				x.ContentTemplate = null;
			});
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is ContentPresenter;
		}
		public static LoopedPanel GetPanelFromItemsControl(DXItemsControl itemsControl) {
			return itemsControl.Panel;
		}
		protected void InvalidatePanel() {
			Panel.Do(x => x.InvalidatePanel());
		}
		UIElement IDXItemContainerGenerator.Generate(int index, out bool isNew) {
			isNew = false;
			if (Panel == null)
				return null;
			index = Panel.IndexCalculator.GetIndex(index, GetItemsCount(), IsLooped);
			if (!CanGenerateItem(index))
				return null;
			FrameworkElement container;
			if (PreviousInternalIndex2Item.TryGetValue(index, out container))
				PreviousInternalIndex2Item.Remove(index);
			else if (Recycled.Count > 0 && Recycled.Contains(index))
				container = Recycled.Pop(index);
			else {
				isNew = true;
				object item = GetItem(index);
				container = IsItemItsOwnContainerOverride(item) ? (FrameworkElement)item : (FrameworkElement)GetContainerForItemOverride();
			}
			InternalIndex2Item.Add(index, container);
			return container;
		}
		protected int GetIndex(object item, Func<object, bool> comparer) {
			for (int i = 0; i < GetItemsCount(); i++) {
				if (comparer(Items[i]))
					return i;
			}
			foreach (var pair in OutOfRangeData.Where(pair => comparer(pair.Value)))
				return pair.Key;
			return InvalidHandle;
		}
		protected int GetIndex(object item) {
			return GetIndex(item, x => object.Equals(x, item));
		}
		protected object GetItem(int index) {
			if (index == InvalidHandle)
				return null;
			bool itemInRange = index >= 0 && index < GetItemsCount();
			if (IsLooped || itemInRange)
				return Items[index];
			object item;
			if (OutOfRangeData.TryGetValue(index, out item))
				return item;
			return null;
		}
		protected bool CanGenerateItem(int index) {
			if (index == InvalidHandle)
				return false;
			bool itemInRange = index >= 0 && index < GetItemsCount();
			if (itemInRange)
				return true;
			if (IsLooped)
				return false;
			object item;
			OutOfRangeData.TryGetValue(index, out item);
			bool hasItem = RaiseProcessOutOfRangeItem(index, ref item);
			if (hasItem)
				OutOfRangeData[index] = item;
			return hasItem;
		}
		UIElement IDXItemContainerGenerator.GetContainer(int index) {
			if (GetItemsCount() == 0)
				return null;
			index = Panel.IndexCalculator.GetIndex(index, GetItemsCount(), IsLooped);
			FrameworkElement container;
			if (InternalIndex2Item.TryGetValue(index, out container))
				return container;
			return null;
		}
		void IDXItemContainerGenerator.PrepareItemContainer(int index, UIElement container) {
			index = Panel.IndexCalculator.GetIndex(index, GetItemsCount(), IsLooped);
			object dataContext = GetItem(index);
			PrepareContainerForItemOverride(container, dataContext);
		}
		void IDXItemContainerGenerator.StartAt(int index) {
			if (IsInItemGeneration)
				throw new ArgumentException("isinitemgeneration");
			IsInItemGeneration = true;
			PreviousInternalIndex2Item.AddRange(InternalIndex2Item);
			InternalIndex2Item.Clear();
		}
		void IDXItemContainerGenerator.Stop() {
			if (!IsInItemGeneration)
				throw new ArgumentException("!isinitemgeneration");
			IDXItemContainerGenerator generator = this;
			IsInItemGeneration = false;
			foreach (var value in PreviousInternalIndex2Item) {
				generator.ClearItemContainer(value.Key, value.Value);
				Recycled.Push(value.Key, value.Value);
			}
			PreviousInternalIndex2Item.Clear();
		}
		void IDXItemContainerGenerator.ClearItemContainer(int index, UIElement container) {
			if (Recycled.IsManipulating)
				return;
			index = Panel.IndexCalculator.GetIndex(index, GetItemsCount(), IsLooped);
			object dataContext = GetItem(index);
			ClearContainerForItemOverride(container, dataContext);
		}
		public int GetItemsCount() {
			return Items.Count;
		}
		void IDXItemContainerGenerator.RemoveItems() {
			if (Recycled.Count > MaxRecycledCount) {
				IDXItemContainerGenerator generator = this;
				foreach (var item in Recycled) {
					generator.ClearItemContainer(0, item);
					Panel.Children.Remove(item);
				}
				Recycled.Reset();
			}
		}
		void IDXItemContainerGenerator.StartManipulation() {
			Recycled.IsManipulating = true;
		}
		void IDXItemContainerGenerator.StopManipulation() {
			Recycled.IsManipulating = false;
		}
		bool RaiseProcessOutOfRangeItem(int index, ref object item) {
			item = null;
			if (ProcessOutOfRangeItem == null)
				return false;
			var args = new DXItemsControlOutOfRangeItemEventArgs(index);
			ProcessOutOfRangeItem(this, args);
			if (!args.Handled)
				return false;
			item = args.Item;
			return true;
		}
	}
}
