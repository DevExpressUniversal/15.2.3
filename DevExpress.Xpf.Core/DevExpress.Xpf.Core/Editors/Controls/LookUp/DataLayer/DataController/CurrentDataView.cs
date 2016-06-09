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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class CurrentDataView : PlainDataView {
		readonly Dictionary<DisplayTextCacheItem, int> editTextToDisplayTextCache = new Dictionary<DisplayTextCacheItem, int>();
		object visibleListWrapper;
		readonly object handle;
		public object Handle {
			get { return handle; }
		}
		public object VisibleList {
			get { return visibleListWrapper ?? (visibleListWrapper = CreateVisibleListWrapper()); }
		}
		protected object GetVisibleListInternal() {
			return visibleListWrapper;
		}
		protected CurrentDataView(object listSource, object handle, string valueMember, string displayMember)
			: base(listSource, valueMember, displayMember) {
			this.handle = handle;
		}
		protected abstract object CreateVisibleListWrapper();
		public override IEnumerator<DataProxy> GetEnumerator() {
			return View.GetEnumerator();
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			IDisposable visibleListDisposable = visibleListWrapper as IDisposable;
			if (visibleListDisposable != null)
				visibleListDisposable.Dispose();
			visibleListWrapper = null;
		}
		protected internal override bool ProcessChangeSource(ListChangedEventArgs e) {
			bool result = base.ProcessChangeSource(e);
			if (result)
				RaiseListChanged(ConvertListChangedEventArgs(e));
			return result;
		}
		protected virtual ListChangedEventArgs ConvertListChangedEventArgs(ListChangedEventArgs e) {
			return e;
		}
		public int FindItemIndexByText(string text, bool isCaseSensitive, bool autoComplete, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			if (text == null)
				return -1;
			int index;
			string findText = isCaseSensitive ? text : text.ToLower();
			var cacheItem = CreateDisplayTextCacheItem(findText, autoComplete, startItemIndex, searchNext, ignoreStartIndex);
			if (!editTextToDisplayTextCache.TryGetValue(cacheItem, out index)) {
				index = FindItemIndexByTextInternal(findText, isCaseSensitive, autoComplete, startItemIndex, searchNext, ignoreStartIndex);
				UpdateDisplayTextCache(findText, autoComplete, startItemIndex, searchNext, index, ignoreStartIndex);
			}
			return index == Data.DataController.OperationInProgress ? -1 : index;
		}
		DisplayTextCacheItem CreateDisplayTextCacheItem(string text, bool autoComplete, int startIndex, bool searchNext, bool ignoreStartIndex) {
			return new DisplayTextCacheItem() {DisplayText = text, AutoComplete = autoComplete, StartIndex = startIndex, SearchNext = searchNext, IgnoreStartIndex = ignoreStartIndex};
		}
		protected virtual void UpdateDisplayTextCache(string findText, bool autoComplete, int startIndex, bool searchNext, int listSourceIndex, bool ignoreStartIndex) {
			editTextToDisplayTextCache[CreateDisplayTextCacheItem(findText, autoComplete, startIndex, searchNext, ignoreStartIndex)] = listSourceIndex;
		}
		protected CriteriaOperator GetCompareCriteriaOperator(bool autoComplete, OperandProperty property, OperandValue value) {
			if (autoComplete)
				return new FunctionOperator(FunctionOperatorType.StartsWith, property, value);
			return new BinaryOperator(property, value, BinaryOperatorType.Equal);
		}
		protected virtual int FindItemIndexByTextInternal(string text, bool isCaseSensitive, bool autoComplete, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			var compareOperand = new OperandProperty(DataAccessor.DisplayPropertyName);
			var compareOperator = GetCompareCriteriaOperator(autoComplete && !string.IsNullOrEmpty(text), compareOperand, new OperandValue(text));
			return View.FindIndexByText(compareOperand, compareOperator, text, isCaseSensitive, startItemIndex, searchNext, ignoreStartIndex);
		}
		public virtual void ResetDisplayTextCache() {
			editTextToDisplayTextCache.Clear();
		}
		public override bool ProcessAddItem(int index) {
			ResetDisplayTextCache();
			return base.ProcessAddItem(index);
		}
		public override bool ProcessChangeItem(int index) {
			ResetDisplayTextCache();
			return base.ProcessChangeItem(index);
		}
		public override bool ProcessDeleteItem(int index) {
			ResetDisplayTextCache();
			return base.ProcessDeleteItem(index);
		}
		public override bool ProcessMoveItem(int oldIndex, int newIndex) {
			ResetDisplayTextCache();
			return base.ProcessMoveItem(oldIndex, newIndex);
		}
		public override bool ProcessReset() {
			ResetDisplayTextCache();
			return base.ProcessReset();
		}
		public override bool ProcessChangeSortFilter(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			return base.ProcessChangeSortFilter(groups, sorts, filterCriteria, displayFilterCriteria);
		}
		public virtual bool ProcessChangeFilter(CriteriaOperator filterCriteria) {
			return false;
		}
		public virtual void ProcessRowLoaded(object value) {
		}
		public virtual void ProcessRefreshed() {
			ResetDisplayTextCache();
		}
		public virtual void ProcessFindIncrementalCompleted(string text, int startIndex, bool searchNext, bool ignoreStartIndex, object value) {
		}
		public virtual void ResetVisibleList() {
			(visibleListWrapper as IDisposable).Do(x => x.Dispose());
			visibleListWrapper = null;
		}
		public virtual void CancelAsyncOperations() {
			ResetDisplayTextCache();
		}
		public virtual bool ProcessInconsistencyDetected() {
			ResetDisplayTextCache();
			return true;
		}
	}
}
