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

using DevExpress.Utils.Controls;
using DevExpress.XtraPivotGrid.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace DevExpress.XtraPivotGrid.Data {
	public abstract class PivotFilterItemsBase : Collection<PivotGridFilterItem>, IFilterItems {
		public const char CheckedChar = 'T';
		public const char UncheckedChar = 'F';
		public const char IndeterminateChar = 'I';
		bool deferUpdates;
		protected static char GetCheckedChar(bool? isChecked) {
			if(!isChecked.HasValue) return IndeterminateChar;
			return isChecked.Value ? CheckedChar : UncheckedChar;
		}
		protected static bool? GetIsChecked(char checkedChar) {
			if(checkedChar == IndeterminateChar) return null;
			return checkedChar == CheckedChar;
		}
		List<bool?> savedChecks;
		PivotFilterType lastFilterType;
		bool showNewValues;
		public bool ShowNewValues {
			get { return showNewValues; }
			set { if(showNewValues == value) return;
				showNewValues = value;
			}
		}
		public PivotFilterItemsBase(PivotGridData data, PivotGridFieldBase field)
			: this(data, field, false, false, false) {
		}
		public PivotFilterItemsBase(PivotGridData data, PivotGridFieldBase field, bool radioMode)
			: this(data, field, radioMode, false, false) {
		}
		public PivotFilterItemsBase(PivotGridData data, PivotGridFieldBase field, bool radioMode, bool showOnlyAvailableItems, bool deferUpdates) {
			Data = data;
			Field = field;
			RadioMode = radioMode;
			ShowOnlyAvailableItems = showOnlyAvailableItems;
			this.lastFilterType = FilterValues.FilterType;
			showNewValues = ActualShowNewValues;
			ResetChecks();
			this.deferUpdates = deferUpdates;
		}
		public bool DeferUpdates { get { return deferUpdates; } }
		IFilterItem IFilterItems.this[int index] { get { return this[index]; } }
		public PivotGridData Data { get; private set; }
		protected PivotGridDataAsync DataAsync { get { return (PivotGridDataAsync)Data; } }
		protected abstract bool ActualShowNewValues { get; }
		public PivotGridFieldBase Field { get; private set; }
		public abstract IFilterValues FilterValues { get; }
		public virtual PivotGridGroup Group { get { return Field.Group; } }
		public bool RadioMode { get; set; }
		public bool ShowOnlyAvailableItems { get; set; }
		public virtual IEnumerable<PivotGridFilterItem> VisibleItems { get { return AllVisibleItems; } }
		public IEnumerable<PivotGridFilterItem> AllVisibleItems {
			get {
				foreach(PivotGridFilterItem item in this)
					if(item.IsVisible)
						yield return item;
			}
		}
		protected internal Type ColumnType { get { return Field.ActualOLAPFilterByUniqueName ? typeof(string) : Data.GetFieldType(Field); } }
		protected abstract void UpdateBlankItemCaptions();
		public void CreateItems() {
			Reset();
			AddValues();
			AddChecksRange();
		}
		public void CreateItemsAsync(AsyncCompletedHandler completed) {
			DataAsync.SetLoadingPanelType(PivotLoadingPanelType.FilterPopupLoadingPanel);
			Reset();
			AddValuesAsync(delegate(AsyncOperationResult result) {
				AddChecksRange();
				DataAsync.SetLoadingPanelType(PivotLoadingPanelType.MainLoadingPanel);
				if(completed != null)
					completed.Invoke(result);
			});
		}
		void Reset() {
			this.lastFilterType = FilterValues.FilterType;
			this.showNewValues = ActualShowNewValues;
			Clear();
		}
		public virtual int CheckCount { get { return GetCheckCountCore(false); } }
		public virtual int VisibleCheckCount { get { return GetCheckCountCore(true); } }
		public virtual void EnsureAvailableItems() { }
		public virtual void EnsureAvailableItemsAsync(AsyncCompletedHandler completed) {
			if(completed != null)
				completed(null);
		}
		int GetCheckCountCore(bool visibleItemsOnly) {
			int checkCount = 0;
			foreach(PivotGridFilterItem item in visibleItemsOnly ? AllVisibleItems : this) {
				if(item.IsChecked == true)
					checkCount++;
			}
			return checkCount;
		}
		public int VisibleCount {
			get {
				int count = 0;
				foreach(PivotGridFilterItem item in AllVisibleItems)
					count++;
				return count;
			}
		}
		public bool CanAccept {
			get {
				if(RadioMode)
					return VisibleCheckCount == 1 || VisibleCheckCount == VisibleCount;
				return VisibleCheckState != false;
			}
		}
		public void CheckAllItems(bool isChecked) {
			CheckItemsCore(isChecked, false);
		}
		public void CheckVisibleItems(bool isChecked) {
			CheckItemsCore(isChecked, true);
		}
		void CheckItemsCore(bool isChecked, bool visibleItemsOnly) {
			foreach(PivotGridFilterItem item in visibleItemsOnly ? AllVisibleItems : this)
				item.IsChecked = isChecked;
		}
		public bool? CheckState { get { return GetCheckStateCore(false); } }
		public bool? VisibleCheckState { get { return GetCheckStateCore(true); } }
		bool? GetCheckStateCore(bool visibleItemsOnly) {
			int checkCount = 0;
			foreach(PivotGridFilterItem item in visibleItemsOnly ? AllVisibleItems : this) {
				if(!item.IsChecked.HasValue) return null;
				if(item.IsChecked == true) checkCount++;
			}
			if(checkCount == (visibleItemsOnly ? VisibleCount : Count)) return true;
			return checkCount == 0 ? (bool?)false : null;
		}
		public bool HasChanges { get { return !AreChecksEqual(this.savedChecks, GetChecks()) || (ShowNewValues != ActualShowNewValues) || DeferUpdates; } }
		public abstract List<object> LoadValues(object[] branchItems);
		public abstract void LoadValuesAsync(object[] parentValues, AsyncCompletedHandler asyncCompleted);
		public bool ApplyFilter(bool allowOnChanged, bool deferUpdates, bool force) {
			if((!force && !HasChanges) || !CanAccept) 
				return false;
			if(deferUpdates) {
				AddToCustomizationForm();
				return true;
			}
			if(!ApplyFilterCore(allowOnChanged))
				return false;
			this.lastFilterType = FilterValues.FilterType;
			return true;
		}
		void AddToCustomizationForm() {
			if(this is PivotGridFilterItems)
				Data.GetCustomizationFormFields().AddFieldFilter(Field, this);
			else
				Data.GetCustomizationFormFields().AddGroupFilter(Field, this);
		}
		public void ApplyFilter() {
			ApplyFilter(true, deferUpdates, false);
		}
		protected abstract bool ApplyFilterCore(bool allowOnChanged);
		public void InvertCheckState() {
			InvertCheckStateCore(false);
		}
		public void InvertVisibleCheckState() {
			InvertCheckStateCore(true);
		}
		protected virtual void InvertCheckStateCore(bool visibleItemsOnly) {
			foreach(PivotGridFilterItem item in visibleItemsOnly ? AllVisibleItems : AllItems) {
				if(item.IsChecked != null)
					item.IsChecked = !item.IsChecked.Value;
			}
		}
		public int LevelCount { get { return Group != null ? Group.Count : 0; } }
		public string ShowAllItemCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll); } }
		public void Initialize(string persistent, string states) {
			PersistentString = persistent;
			StatesString = states;
		}
		public void InitializeVisible(string persistent, string visibleStates) {
			PersistentString = persistent;
			VisibleStatesString = visibleStates;
		}
		public abstract string PersistentString { get; set; }
		public string StatesString {
			get { return GetStatesStringCore(false); }
			set { SetStatesStringCore(value, false); }
		}
		public string VisibleStatesString {
			get { return GetStatesStringCore(true); }
			set { SetStatesStringCore(value, true); }
		}
		string GetStatesStringCore(bool visibleItemsOnly) {
			StringBuilder sb = new StringBuilder(visibleItemsOnly ? VisibleCount : Count);
			foreach(PivotGridFilterItem item in visibleItemsOnly ? AllVisibleItems : AllItems)
				sb.Append(GetCheckedChar(item.IsChecked));
			return sb.ToString();
		}
		void SetStatesStringCore(string value, bool visibleItemsOnly) {
			if(string.IsNullOrEmpty(value)) return;
			int count = Math.Min(visibleItemsOnly ? VisibleCount : Count, value.Length);
			foreach(var pair in (visibleItemsOnly ? AllVisibleItems : AllItems).WithIndex()) {
				if(pair.Index >= count) return;
				pair.Value.IsChecked = GetIsChecked(value[pair.Index]);
			}
		}
		protected abstract void Add(object value, string text, bool? isChecked, bool isVisible);
		protected abstract void AddValues();
		protected abstract void AddValuesAsync(AsyncCompletedHandler completed);
		protected virtual Collection<PivotGridFilterItem> AllItems { get { return this; } }
		protected override void ClearItems() {
			base.ClearItems();
			ResetChecks();
		}
		protected abstract PivotGridFilterItem CreatePivotFilterItem(IFilterItem parent, int level, object value, string text, bool? isChecked, bool isVisible);
		public string GetDisplayText(object value) {
			return GetDisplayTextCore(value, 0, false);
		}
		protected string GetDisplayTextCore(object value, int level, bool changeBlank) {
			string valueText = Data.GetCustomFilterFieldValueText(GetLevelField(level), value);
			if(changeBlank && value == null && string.IsNullOrEmpty(valueText))
				return GetShowBlanksItemCaptionCore();
			return valueText;
		}
		protected virtual string GetShowBlanksItemCaptionCore() {
			return PivotGridLocalizer.GetString(PivotGridStringId.FilterShowBlanks);
		}
		protected PivotGridFieldBase GetLevelField(int level) {
			if(Group == null) return Field;
			if(level >= 0 && level < Group.Count)
				return Group[level];
			throw new ArgumentException("PivotFilterItemsBase.GetLevelField");
		}
		protected void AddChecksRange() {
			this.savedChecks = GetChecks();
		}
		protected void InsertChecksRange(int startIndex, int count) {
			this.savedChecks.InsertRange(startIndex >= 0 ? startIndex : 0, GetChecksCore(AllItems, startIndex, count));
		}
		protected void ResetChecks() {
			this.savedChecks = null;
		}
		protected virtual bool HasNullValues { get { return Data.HasNullValues(Field); } }
		List<bool?> GetChecks() {
			return GetChecksCore(AllItems, -1, -1);
		}
		List<bool?> GetChecksCore(IList<PivotGridFilterItem> items, int startIndex, int count) {
			if(startIndex < 0 || count < 0) {
				startIndex = 0;
				count = items.Count;
			}
			int endIndex = startIndex + count;
			List<bool?> checks = new List<bool?>(count);
			for(int i = startIndex; i < endIndex; i++)
				checks.Add(items[i].IsChecked);
			return checks;
		}
		bool AreChecksEqual(List<bool?> checks1, List<bool?> checks2) {
			if(checks1 == null || checks2 == null) return false;
			if(checks1.Count != checks2.Count) return false;
			for(int i = 0; i < checks1.Count; i++)
				if(checks1[i] != checks2[i]) return false;
			return true;
		}
	}
	static class IEnumerableExtension {
		public static IEnumerable<IndexValuePair<int, T>> WithIndex<T>(this IEnumerable<T> enumerable) {
			int index = 0;
			foreach(T item in enumerable)
				yield return new IndexValuePair<int, T>(index++, item);
		}
	}
	class IndexValuePair<I, V> {
		public IndexValuePair() { }
		public IndexValuePair(I index, V value) {
			Index = index;
			Value = value;
		}
		public I Index { get; set; }
		public V Value { get; set; }
	}
}
