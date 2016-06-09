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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors.Helpers {
	public interface IItemsProvider : IItemsProviderCollectionViewSupport {
		object CurrentDataViewHandle { get; }
		int IndexOfValue(object value, object handle);
		object GetItem(object value, object handle);
		object GetValueByIndex(int index, object handle);
		object GetDisplayValueByIndex(int index, object handle);
		event ItemsProviderChangedEventHandler ItemsProviderChanged;
		int Count { get; }
		int GetCount(object handle);
		IEnumerable<string> AvailableColumns { get; }
		CriteriaOperator ActualFilterCriteria { get; }
		bool IsAsyncServerMode { get; }
		bool IsInLookUpMode { get; }
		CriteriaOperator DisplayFilterCriteria { get; set; }
		IEnumerable VisibleListSource { get; }
		void UpdateDisplayMember();
		void UpdateValueMember();
		void ProcessSelectionChanged(NotifyItemsProviderSelectionChangedEventArgs e);
		void ProcessCollectionChanged(NotifyItemsProviderChangedEventArgs e);
		object GetValueFromItem(object item, object handle);
		void Reset();
		object this[int index] { get; }
		int GetIndexByItem(object item, object handle);
		ServerModeDataControllerBase GetServerModeDataController();
		void DoRefresh();
		int FindItemIndexByText(string text, bool isCaseSensitiveSearch, bool allowTextInputSuggestions, object handle, int startControllerIndex = -1);
		object GetDisplayValueByEditValue(object editValue, object handle);
		int GetControllerIndexByIndex(int selectedIndex, object handle);
		int GetIndexByControllerIndex(int newControllerIndex, object handle);
		object GetItemByControllerIndex(int controllerIndex, object handle);
		void ResetDisplayTextCache();
		CriteriaOperator CreateDisplayFilterCriteria(string searchText, FilterCondition condition);
#if DEBUGTEST
		DataControllerItemsCache ItemsCache { get; }
#endif
		void UpdateFilterCriteria();
		void UpdateItemsSource();
		void RegisterSnapshot(object handle);
		void ReleaseSnapshot(object handle);
		void SetDisplayFilterCriteria(CriteriaOperator criteria, object handle);
		IEnumerable GetVisibleListSource(object handle);
		string GetDisplayPropertyName(object handle);
	}
	public interface IItemsProvider2 : IItemsProviderCollectionViewSupport {
		object CurrentDataViewHandle { get; }
		int IndexOfValue(object value, object handle);
		object GetItem(object value, object handle);
		object GetValueByIndex(int index, object handle);
		object GetDisplayValueByIndex(int index, object handle);
		object GetValueByRowKey(object rowKey, object handle);
		event ItemsProviderChangedEventHandler ItemsProviderChanged;
		int Count { get; }
		int GetCount(object handle);
		IEnumerable<string> AvailableColumns { get; }
		CriteriaOperator ActualFilterCriteria { get; }
		bool IsAsyncServerMode { get; }
		bool IsSyncServerMode { get; }
		bool IsServerMode { get; }
		bool IsInLookUpMode { get; }
		bool HasValueMember { get; }
		IEnumerable VisibleListSource { get; }
		void UpdateDisplayMember();
		void UpdateValueMember();
		void ProcessSelectionChanged(NotifyItemsProviderSelectionChangedEventArgs e);
		void ProcessCollectionChanged(NotifyItemsProviderChangedEventArgs e);
		object GetValueFromItem(object item, object handle);
		void Reset();
		object this[int index] { get; }
		int GetIndexByItem(object item, object handle);
		void DoRefresh();
		bool NeedsRefresh { get; }
		int FindItemIndexByText(string text, bool isCaseSensitiveSearch, bool allowTextInputSuggestions, object handle, int startIndex = -1, bool searchNext = true, bool ignoreStartIndex = false);
		object GetDisplayValueByEditValue(object editValue, object handle);
		int GetControllerIndexByIndex(int selectedIndex, object handle);
		int GetIndexByControllerIndex(int newControllerIndex, object handle);
		object GetItemByControllerIndex(int controllerIndex, object handle);
		void ResetDisplayTextCache();
		CriteriaOperator CreateDisplayFilterCriteria(string searchText, FilterCondition condition);
		void ResetVisibleList(object handle);
		void CancelAsyncOperations(object handle);
#if DEBUGTEST
		DataControllerItemsCache ItemsCache { get; }
#endif
		void UpdateFilterCriteria();
		void UpdateItemsSource();
		void RegisterSnapshot(object handle);
		void ReleaseSnapshot(object handle);
		void SetDisplayFilterCriteria(CriteriaOperator criteria, object handle);
		void SetFilterCriteria(CriteriaOperator criteria, object handle);
		IEnumerable GetVisibleListSource(object handle);
		string GetDisplayPropertyName(object handle);
		string GetValuePropertyName(object handle);
	}
}
