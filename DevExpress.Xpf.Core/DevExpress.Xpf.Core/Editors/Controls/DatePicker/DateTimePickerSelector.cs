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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Editors {
	public class DateTimePickerSelector : DXSelector {
		public static readonly DependencyProperty IsAnimatedProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty VisibleItemsCountProperty;
		public static readonly DependencyProperty UseTransitionsProperty;
		static DateTimePickerSelector() {
			Type ownerType = typeof(DateTimePickerSelector);
			IsAnimatedProperty = DependencyPropertyManager.Register("IsAnimated", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DateTimePickerSelector)d).OnAnimatedChanged((bool)e.NewValue)));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DateTimePickerSelector)d).OnExpandedChanged((bool)e.NewValue)));
			VisibleItemsCountProperty = DependencyPropertyManager.Register("VisibleItemsCount", typeof(int), ownerType,
				new PropertyMetadata(0));
			UseTransitionsProperty = DependencyPropertyManager.Register("UseTransitions", typeof(bool), ownerType,
				new PropertyMetadata(false));
		}
		protected virtual void OnExpandedChanged(bool newValue) {
			if (newValue)
				Focus();
			InvalidatePanel();
		}
		protected virtual void OnAnimatedChanged(bool newValue) {
			InvalidatePanel();
		}
		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
			BringToView();
		}
		public bool IsAnimated {
			get { return (bool)GetValue(IsAnimatedProperty); }
			set { SetValue(IsAnimatedProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public int VisibleItemsCount {
			get { return (int)GetValue(VisibleItemsCountProperty); }
			set { SetValue(VisibleItemsCountProperty, value); }
		}
		public bool UseTransitions {
			get { return (bool)GetValue(UseTransitionsProperty); }
			set { SetValue(UseTransitionsProperty, value); }
		}
		public DataTemplate SelectedItemTemplate {
			get { return GetItemTemplate(SelectedItem); }
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is DateTimePickerItem;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new DateTimePickerItem();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			if (item == null)
				return;
			base.PrepareContainerForItemOverride(element, item);
			var pickerItem = (DateTimePickerItem)element;
			pickerItem.UseTransitions = UseTransitions;
			pickerItem.IsExpanded = IsExpanded || IsAnimated;
			pickerItem.Opacity = !IsAnimated && SelectedItem.Return(x => x.Equals(item), () => false) ? 0d : 1d;
			pickerItem.IsFake = (item as DateTimePickerData).Return(x => x.DateTimePart, () => DateTimePart.None) == DateTimePart.None;
		}
		public DateTimePickerSelector() {
			DefaultStyleKey = typeof(DateTimePickerSelector);
			ProcessOutOfRangeItem += OnProcessOutOfRangeItem;
		}
		public void Spin(int count = 1) {
			if (ScrollViewer == null || SelectedIndex == InvalidHandle)
				return;
			var indexCalculator = Panel.IndexCalculator;
			int newSelectedIndex = SelectedIndex + count;
			double offset = indexCalculator.IndexToLogicalOffset(IsLooped ? newSelectedIndex : Math.Max(Math.Min(GetItemsCount() - 1, newSelectedIndex), 0));
			if (Panel.VerticalOffset.AreClose(offset))
				return;
			ScrollViewer.AnimateScrollToVerticalOffset(offset, () => {
				SelectedItem = null;
				IsAnimated = true;
			}, null, () => {
				ScrollViewer.IsIntermediate = false;
			}, IsLooped ? (Func<double, double>)ScrollViewer.EnsureVerticalOffset : null);
		}
		public void SpinToIndex(int index) {
			if(ScrollViewer == null || SelectedIndex == InvalidHandle)
				return;
			var indexCalculator = Panel.IndexCalculator;
			double offset = indexCalculator.IndexToLogicalOffset(index);
			if(Panel.VerticalOffset.AreClose(offset))
				return;
			IsAnimated = true;
			ScrollViewer.ScrollToVerticalOffset(offset);
		}
		void OnProcessOutOfRangeItem(object sender, DXItemsControlOutOfRangeItemEventArgs e) {
			e.Item = new DateTimePickerData { DateTimePart = DateTimePart.None, Text = e.Index.ToString() };
			e.Handled = true;
		}
		protected override void OnViewChanged(object sender, ViewChangedEventArgs e) {
			base.OnViewChanged(sender, e);
			if (IsAnimated != e.IsIntermediate)
				IsAnimated = e.IsIntermediate;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if (!IsExpanded) {
				UseTransitions = true;
				IsExpanded = true;
				e.Handled = true;
				Focus();
			}
			else if (SelectedItem != null) {
				IsExpanded = false;
				e.Handled = true;
				Focus();
			}
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
			IsExpanded = true;
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			IsExpanded = true;
		}
		protected override void SelectedIndexChanged(int newValue) {
			BringToView();
		}
		protected override void BringToView() {
			base.BringToView();
			if (ScrollViewer == null || SelectedIndex == InvalidHandle)
				return;
			var panel = GetPanelFromItemsControl(this);
			var indexCalculator = panel.IndexCalculator;
			double offset = indexCalculator.IndexToLogicalOffset(SelectedIndex);
			double currentOffset = panel.Orientation == Orientation.Vertical ? ScrollViewer.VerticalOffset : ScrollViewer.HorizontalOffset;
			if (currentOffset.AreClose(offset)) {
				InvalidatePanel();
				return;
			}
			if (panel.Orientation == Orientation.Vertical)
				ScrollViewer.ScrollToVerticalOffset(offset);
			else
				ScrollViewer.ScrollToHorizontalOffset(offset);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			var panel = GetPanelFromItemsControl(this);
			var indexCalculator = panel.IndexCalculator;
			double viewport = panel.Orientation == Orientation.Vertical ? panel.ViewportHeight : panel.ViewportWidth;
			double logicalViewport = indexCalculator.LogicalToNormalizedOffset(viewport);
			VisibleItemsCount = Convert.ToInt32(logicalViewport);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			BringToView();
		}
	}
}
