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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Collections;
namespace DevExpress.Xpf.Charts {
	public enum ElementSelectionMode { None, Single, Multiple, Extended }
	public enum SeriesSelectionMode { Series, Point, Argument }
}
namespace DevExpress.Xpf.Charts.Native {
	public enum SelectionAction { None, Clear, Add, Remove, ReplaceAll }
	public class SelectionController {
		readonly ChartControl chart;
		IList currentList = null;
		Locker changeLocker = new Locker();
		public event EventHandler SelectionUpdated;
		public object SelectedItem { get { return chart.SelectedItem; } set { chart.SelectedItem = value; } }
		public IList SelectedItems { get { return chart.SelectedItems; } set { chart.SelectedItems = value; } }
		public SelectionController(ChartControl chart) {
			this.chart = chart;
		}
		void SetCurrentSelectedItems(IList newList) {
			SetCurrentList(newList);
			SelectedItems = newList;
		}
		void UpdateSelectedItemByList(IList list) {
			SelectedItem = list != null && list.Count > 0 ? list[0] : default(object);
		}
		void RaiseSelectionUpdated() {
			if (SelectionUpdated != null)
				SelectionUpdated(this, EventArgs.Empty);
		}
		IList CreateNewSelectedItems() {
			return new ObservableCollection<object>();
		}
		void EnsureSelectedItems() {
			if (SelectedItems == null)
				SetCurrentSelectedItems(CreateNewSelectedItems());
		}
		void SetCurrentList(IList list) {
			UnsubscribeListEvents(currentList);
			currentList = list;
			SubscribeListEvents(currentList);
		}
		void SubscribeListEvents(object list) {
			INotifyCollectionChanged coll = list as INotifyCollectionChanged;
			if (coll != null)
				coll.CollectionChanged += OnCollectionChanged;
		}
		void UnsubscribeListEvents(object list) {
			INotifyCollectionChanged coll = list as INotifyCollectionChanged;
			if (coll != null)
				coll.CollectionChanged -= OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			LockedUpdate(() => UpdateSelectedItemByList(SelectedItems));
		}
		void UpdateElementSelection(IInteractiveElement element) {
			element.IsSelected = (SelectedItems != null) && SelectedItems.Contains(element.Content);
		}
		bool CaluclateItemsSelection(IList<IInteractiveElement> elements) {
			foreach (IInteractiveElement element in elements)
				if (!element.IsSelected)
					return false;
			return true;
		}
		SelectionAction CalculateSelectionAction(ElementSelectionMode selectionMode, ModifierKeys keyModifiers, bool isItemSelected, bool touchSelectionStarted) {
			switch (selectionMode) {
				case ElementSelectionMode.Single:
					return isItemSelected ? SelectionAction.None : SelectionAction.ReplaceAll;
				case ElementSelectionMode.Multiple:
					return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
				case ElementSelectionMode.Extended:
					if (keyModifiers == ModifierKeys.Shift)
						return SelectionAction.Add;
					if (keyModifiers == ModifierKeys.Control || touchSelectionStarted)
						return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
					return SelectionAction.ReplaceAll;
			}
			return SelectionAction.None;
		}
		void AddElements(IList<IInteractiveElement> elements) {
			EnsureSelectedItems();
			foreach (IInteractiveElement element in elements) {
				element.IsSelected = true;
				SelectedItems.Add(element.Content);
			}
		}
		void RemoveElements(IList<IInteractiveElement> elements) {
			if (SelectedItems != null) {
				foreach (IInteractiveElement element in elements) {
					element.IsSelected = false;
					SelectedItems.Remove(element.Content);
				}
			}
		}
		void LockedUpdate(Action updateAction) {
			if (!changeLocker.IsLocked) {
				changeLocker.Lock();
				updateAction();
				UpdateElementsSelection();
				RaiseSelectionUpdated();
				changeLocker.Unlock();
			}
		}
		void ClearSelectedItems() {
			if (SelectedItems != null)
				SetCurrentSelectedItems(CreateNewSelectedItems());
		}
		internal void UpdateElementsSelection() {
			if (chart.Diagram == null || chart.Diagram.Series == null)
				return;
			foreach (Series series in chart.Diagram.Series) {
				UpdateElementSelection(series);
				if (!((IInteractiveElement)series).IsSelected) {
					foreach (SeriesPoint point in series.Points)
						UpdateElementSelection(point);
				}
			}
		}
		internal void HideSelection() {
			if (chart.Diagram == null || chart.Diagram.Series == null)
				return;
			foreach (Series series in chart.Diagram.Series) {
				((IInteractiveElement)series).IsSelected = false;
				foreach (SeriesPoint point in series.Points) {
					((IInteractiveElement)point).IsSelected = false;
				}
			}
		}
		public void UpdateSelection(IInteractiveElement element, ModifierKeys keyModifiers, bool touchSelectionStatrted) {
			if (element != null)
				UpdateSelection(new IInteractiveElement[] { element }, keyModifiers, touchSelectionStatrted);
		}
		public void UpdateSelection(IList<IInteractiveElement> elements, ModifierKeys keyModifiers, bool touchSelectionStarted) {
			if (elements.Count > 0) {
				SelectionAction action = CalculateSelectionAction(chart.SelectionMode, keyModifiers, CaluclateItemsSelection(elements), touchSelectionStarted);
				switch (action) {
					case SelectionAction.ReplaceAll:
						ClearSelectedItems();
						AddElements(elements);
						break;
					case SelectionAction.Add:
						AddElements(elements);
						break;
					case SelectionAction.Remove:
						RemoveElements(elements);
						break;
					case SelectionAction.None:
						break;
				}
			}
		}
		public void OnUpdateSelectedItem(object item) {
			LockedUpdate(() => {
				if (item != null) {
					IList newList = CreateNewSelectedItems();
					newList.Add(item);
					SetCurrentSelectedItems(newList);
				}
				else
					ClearSelectedItems();
			});
		}
		public void OnUpdateSelectedItems(IList list) {
			LockedUpdate(() => {
				SetCurrentList(list);
				UpdateSelectedItemByList(list);
			});
		}
	}
}
