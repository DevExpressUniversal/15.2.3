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
namespace DevExpress.Xpf.Layout.Core.Selection {
	public delegate bool SelectionChangingHandler<T>(
		T item, bool selected
	);
	public delegate void SelectionChangedHandler<T>(
		T item, bool selected
	);
	public delegate T[] RequestSelectionRangeHandler<T>(
		T first, T last
	);
	public class SelectionInfo<T> : IDisposable {
		bool isDisposing;
		public SelectionInfo(
			SelectionChangingHandler<T> selectionChanging,
			SelectionChangedHandler<T> selectionChanged,
			RequestSelectionRangeHandler<T> requestSelectionRange) {
			SelectionChanged = selectionChanged;
			SelectionChanging = selectionChanging;
			RequestSelectionRange = requestSelectionRange;
			Mode = SelectionMode.SingleItem;
			Selection = new Dictionary<object, T>();
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				SelectionChanged = null;
				RequestSelectionRange = null;
				ClearSelection();
				Selection = null;
			}
			GC.SuppressFinalize(this);
		}
		public SelectionMode Mode { get; set; }
		IDictionary<object, T> Selection { get; set; }
		public T LastSelectedElement { get; private set; }
		SelectionChangingHandler<T> SelectionChanging;
		SelectionChangedHandler<T> SelectionChanged;
		RequestSelectionRangeHandler<T> RequestSelectionRange;
		public void Select(T element) {
			if(element == null) {
				ClearSelection();
				return;
			}
			bool selected = GetSelectedState(element);
			switch(Mode) {
				case SelectionMode.SingleItem:
					ClearSelection(element);
					if(!selected) {
						if(CanSelectionChanging(element, true)) {
							SelectElementCore(element);
						}
					}
					break;
				case SelectionMode.MultipleItems:
					if(CanSelectionChanging(element, !selected)) {
						if(!selected)
							SelectElementCore(element);
						else
							UnSelectElementCore(element);
					}
					break;
				case SelectionMode.ItemRange:
					T[] range = RequestSelectionRange(LastSelectedElement, element);
					ClearSelection(range);
					for(int i = 0; i < range.Length; i++) {
						T toSelect = range[i];
						if(!GetSelectedState(toSelect)) {
							if(CanSelectionChanging(toSelect, true)) {
								SelectElementCore(toSelect);
							}
						}
					}
					break;
			}
			LastSelectedElement = element;
		}
		public bool GetSelectedState(T element) {
			object item = SelectionHelper.GetItem(element);
			return item != null ? Selection.ContainsKey(item) : true;
		}
		void SelectElementCore(T element) {
			Selection.Add(SelectionHelper.GetItem(element), element);
			RaiseSelectionChanged(element, true);
		}
		void UnSelectElementCore(T element) {
			Selection.Remove(SelectionHelper.GetItem(element));
			RaiseSelectionChanged(element, false);
		}
		protected void ClearSelection() {
			T[] selectedElements = new T[Selection.Count];
			Selection.Values.CopyTo(selectedElements, 0);
			for(int i = 0; i < selectedElements.Length; i++) {
				T toUnselect = selectedElements[i];
				if(CanSelectionChanging(toUnselect, false)) {
					UnSelectElementCore(toUnselect);
				}
			}
		}
		protected void ClearSelection(T element) {
			T[] selectedElements = new T[Selection.Count];
			Selection.Values.CopyTo(selectedElements, 0);
			for(int i = 0; i < selectedElements.Length; i++) {
				T toUnselect = selectedElements[i];
				if(!object.Equals(SelectionHelper.GetItem(toUnselect), SelectionHelper.GetItem(element))) {
					if(CanSelectionChanging(toUnselect, false)) {
						UnSelectElementCore(toUnselect);
					}
				}
				else Selection[SelectionHelper.GetItem(element)] = element;
			}
		}
		protected void ClearSelection(T[] range) {
			T[] selectedElements = new T[Selection.Count];
			Selection.Values.CopyTo(selectedElements, 0);
			object[] keys = SelectionHelper.GetItems(range);
			for(int i = 0; i < selectedElements.Length; i++) {
				T toUnselect = selectedElements[i];
				int index = Array.IndexOf(keys, SelectionHelper.GetItem(toUnselect));
				if(index == -1) {
					if(CanSelectionChanging(toUnselect, false)) {
						UnSelectElementCore(toUnselect);
					}
				}
				else {
					T element = range[index];
					Selection[SelectionHelper.GetItem(element)] = element;
				}
			}
		}
		protected bool CanSelectionChanging(T element, bool selected) {
			if(SelectionChanging != null)
				return SelectionChanging(element, selected);
			return true;
		}
		protected void RaiseSelectionChanged(T element, bool selected) {
			if(SelectionChanged != null)
				SelectionChanged(element, selected);
		}
		protected T[] RaiseRequestSelection(T first, T last) {
			T[] result = new T[0];
			if(RequestSelectionRange != null)
				result = RequestSelectionRange(first, last);
			return result;
		}
	}
	public static class SelectionHelper {
		public static T[] GetSelectionRange<T>(T[] baseRange, T first, T last)
			where T : class {
			object[] elements = GetItems(baseRange);
			int index1 = Array.IndexOf(elements, GetItem(first));
			int index2 = Array.IndexOf(elements, GetItem(last));
			if(index1 == -1 && index2 == -1) return new T[0];
			if(index1 == -1 || index2 == -1) return new T[] { last };
			int start = Math.Min(index1, index2);
			int end = Math.Max(index1, index2);
			T[] result = new T[end - start + 1];
			for(int i = start; i < end + 1; i++)
				result[i - start] = baseRange[i];
			return result;
		}
		public static object GetItem<T>(T element) {
			return (element is ISelectionKey) ? (element as ISelectionKey).Item : element;
		}
		public static object GetViewKey<T>(T element) {
			return (element is ISelectionKey) ? (element as ISelectionKey).ViewKey : element;
		}
		public static object GetElementKey<T>(T element) {
			return (element is ISelectionKey) ? (element as ISelectionKey).ElementKey : element;
		}
		public static object[] GetItems<T>(T[] elements) {
			return Array.ConvertAll<T, object>(elements, GetItem);
		}
	}
}
