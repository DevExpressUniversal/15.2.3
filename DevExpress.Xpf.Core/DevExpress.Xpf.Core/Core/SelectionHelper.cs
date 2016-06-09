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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors;
using System.Reflection;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core {
	public class ListEditSelectionControlWrapper : SelectionControlWrapper {
		Action<IList, IList> Action { get; set; }
		ListBoxEdit ListBox { get; set; }
		public ListEditSelectionControlWrapper(ListBoxEdit view) {
			ListBox = view;
		}
		public override void SubscribeSelectionChanged(Action<IList, IList> a) {
			Action = a;
			ListBox.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
		}
		public override void UnsubscribeSelectionChanged() {
			ListBox.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
		}
		void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Action(e.NewItems, e.OldItems);
		}
		public override IList GetSelectedItems() {
			return ListBox.SelectedItems;
		}
		public override void ClearSelection() {
			ListBox.SelectedItems.Clear();
		}
		public override void SelectItem(object item) {
			ListBox.SelectedItems.Add(item);
		}
		public override void UnselectItem(object item) {
			ListBox.SelectedItems.Remove(item);
		}
	}
	public class ListBoxSelectionControlWrapper : SelectionControlWrapper {
		ListBox ListBox { get; set; }
		Action<IList, IList> Action { get; set; }
		public ListBoxSelectionControlWrapper(ListBox view) {
			ListBox = view;
		}
		public override void SubscribeSelectionChanged(Action<IList, IList> a) {
			Action = a;
			ListBox.SelectionChanged += ListBox_SelectionChanged;
		}
		public override void UnsubscribeSelectionChanged() {
			ListBox.SelectionChanged -= ListBox_SelectionChanged;
		}
		void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			Action(e.AddedItems, e.RemovedItems);
		}
		public override IList GetSelectedItems() {
			return ListBox.SelectedItems;
		}
		public override void ClearSelection() {
			ListBox.SelectedItems.Clear();
		}
		public override void SelectItem(object item) {
			ListBox.SelectedItems.Add(item);
		}
		public override void UnselectItem(object item) {
			ListBox.SelectedItems.Remove(item);
		}
	}
	public abstract class SelectionControlWrapper {
		static Dictionary<Type, Type> wrappers = new Dictionary<Type, Type>() {
			{typeof(ListBox), typeof(ListBoxSelectionControlWrapper)},
			{typeof(ListBoxEdit), typeof(ListEditSelectionControlWrapper)}
		};
		public static Dictionary<Type, Type> Wrappers { get { return wrappers; } }
		public static SelectionControlWrapper Create(object source) {
			foreach(KeyValuePair<Type, Type> pair in Wrappers)
				if(pair.Key.IsInstanceOfType(source))
					return (SelectionControlWrapper)Activator.CreateInstance(pair.Value, source);
			return null;
		}
		public abstract void SubscribeSelectionChanged(Action<IList, IList> a);
		public abstract void UnsubscribeSelectionChanged();
		public abstract IList GetSelectedItems();
		public abstract void ClearSelection();
		public abstract void SelectItem(object item);
		public abstract void UnselectItem(object item);
	}
	public class SelectedItemsSourceBrowsableAttribute : Attribute { }
	public class SelectionAttachedBehavior : DependencyObject, IWeakEventListener {
#if !SILVERLIGHT
		[AttachedPropertyBrowsableForType(typeof(ListBox))]
		[AttachedPropertyBrowsableForType(typeof(ListBoxEdit))]
		[AttachedPropertyBrowsableWhenAttributePresentAttribute(typeof(SelectedItemsSourceBrowsableAttribute))]
#endif
		public static IList GetSelectedItemsSource(DependencyObject obj) {
			return (IList)obj.GetValue(SelectedItemsSourceProperty);
		}
		public static void SetSelectedItemsSource(DependencyObject obj, IList value) {
			obj.SetValue(SelectedItemsSourceProperty, value);
		}
		public static readonly DependencyProperty SelectedItemsSourceProperty =
			DependencyProperty.RegisterAttached("SelectedItemsSource", typeof(IList), typeof(SelectionAttachedBehavior), new PropertyMetadata(OnSelectedItemsSourcePropertyChanged));
		static void SetSelectionAttachedBehavior(DependencyObject obj, SelectionAttachedBehavior value) {
			obj.SetValue(SelectionAttachedBehaviorProperty, value);
		}
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty SelectionAttachedBehaviorProperty =
			DependencyProperty.RegisterAttached("SelectionAttachedBehavior", typeof(SelectionAttachedBehavior), typeof(SelectionAttachedBehavior), new PropertyMetadata(OnSelectionControlAttachedBehaviorChanged));
		static void OnSelectedItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IList selectedItemsSource = e.NewValue as IList;
			SetSelectionAttachedBehavior(d, new SelectionAttachedBehavior(selectedItemsSource));
		}
		static void OnSelectionControlAttachedBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SelectionAttachedBehavior oldBehaviour = (SelectionAttachedBehavior)e.OldValue;
			if(oldBehaviour != null)
				oldBehaviour.DisconnectFromSelectionControl();
			SelectionAttachedBehavior behaviour = (SelectionAttachedBehavior)e.NewValue;
			if(behaviour != null)
				behaviour.ConnectToSelectionControl(d);
		}
		bool lockSelection;
		IList selectedItemsSource;
		SelectionControlWrapper SelectionControl { get; set; }
		IList SelectedItemsSource {
			get { return selectedItemsSource; }
			set {
				UnsubscribeSelectedItemsSource();
				selectedItemsSource = value;
				SubscribeSelectedItemsSource();
			}
		}
		void SubscribeSelectedItemsSource() {
			INotifyCollectionChanged collection = SelectedItemsSource as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.AddListener(collection, this);
			}
		}
		void UnsubscribeSelectedItemsSource() {
			INotifyCollectionChanged collection = SelectedItemsSource as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.RemoveListener(collection, this);
			}
		}
		public SelectionAttachedBehavior(IList selectedItemsSource) {
			SelectedItemsSource = selectedItemsSource;
		}
		protected virtual void DisconnectFromSelectionControl() {
			if(SelectionControl != null)
				SelectionControl.UnsubscribeSelectionChanged();
			SelectionControl = null;
		}
		protected virtual void ConnectToSelectionControl(object control) {
			if(SelectionControl != null)
				SelectionControl.UnsubscribeSelectionChanged();
			SelectionControl = SelectionControlWrapper.Create(control);
			SelectionControl.SubscribeSelectionChanged(SelectionChanged);
			SelectedItemsSourceChanged();
		}
		void SelectionChanged(IList addedItems, IList removedItems) {
			if(lockSelection || SelectedItemsSource == null)
				return;
			lockSelection = true;
			ILockable collection = SelectedItemsSource as ILockable;
			if(collection != null) collection.BeginUpdate();
			if(addedItems == null && removedItems == null) {
				SelectedItemsSource.Clear();
				foreach(object o in SelectionControl.GetSelectedItems()) {
					SelectedItemsSource.Add(o);
				}
			} else {
				if(addedItems != null) {
					foreach(object o in addedItems) {
						SelectedItemsSource.Add(o);
					}
				}
				if(removedItems != null) {
					foreach(object o in removedItems) {
						SelectedItemsSource.Remove(o);
					}
				}
			}
			if(collection != null) collection.EndUpdate();
			lockSelection = false;
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				OnSelectedItemsSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
				return true;
			}
			return false;
		}
		void SelectedItemsSourceChanged() {
			OnSelectedItemsSourceCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		private void OnSelectedItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(SelectionControl == null || lockSelection)
				return;
			lockSelection = true;
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					DoSelectAction(e.NewItems, (item) => SelectionControl.SelectItem(item));
					break;
				case NotifyCollectionChangedAction.Remove:
					DoSelectAction(e.OldItems, (item) => SelectionControl.UnselectItem(item));
					break;
				case NotifyCollectionChangedAction.Reset:
					SelectionControl.ClearSelection();
					if(SelectedItemsSource != null)
						DoSelectAction(SelectedItemsSource, (item) => SelectionControl.SelectItem(item));
					break;
			}
			lockSelection = false;
		}
		void DoSelectAction(IList list, Action<object> action) {
			foreach(object item in list) {
				action(item);
			}
		}
	}
}
