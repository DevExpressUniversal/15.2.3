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
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Data.Utils;
using DevExpress.Utils.Extensions.Helpers;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid.Customization {
	public class CustomizationFormFieldsEventArgs : EventArgs {
		readonly CustomizationFormFields customizationFields;
		public CustomizationFormFieldsEventArgs(CustomizationFormFields customizationFields) {
			this.customizationFields = customizationFields;
		}
		public CustomizationFormFields CustomizationFields { get { return customizationFields; } }
	}
	public interface ICustomFilterColumnsProvider {
		List<PivotGridFieldBase> Fields { get; }
	}
	public class CustomizationFormFields : ICustomFilterColumnsProvider, IDisposable {
		static readonly int PivotAreasCount = Helpers.GetEnumValues(typeof(PivotArea)).Length;
		public delegate void UpdateCompletedCallback();
		WeakEventHandler<EventArgs, EventHandler> deferUpdatesChangedHandler;
		public event EventHandler DeferUpdatesChanged {
			add { deferUpdatesChangedHandler += value; }
			remove { deferUpdatesChangedHandler -= value; }
		}
		public event EventHandler<CustomizationFormFieldsEventArgs> ListsChanged;
		public event EventHandler<CustomizationFormFieldsEventArgs> BeforeSettingFields;
		public event EventHandler<CustomizationFormFieldsEventArgs> AfterSettingFields;
		UpdateCompletedCallback updateCompletedCallback;
		readonly PivotGridData data;
		readonly List<PivotFieldItemBase> hiddenFields;
		readonly ReadOnlyCollection<PivotFieldItemBase> publicHiddenFields;
		readonly List<PivotFieldItemBase>[] fieldLists;
		readonly ReadOnlyCollection<PivotFieldItemBase>[] publicLists;
		bool deferUpdates;
		public CustomizationFormFields(PivotGridData data) {
			this.data = data;
			this.hiddenFields = new List<PivotFieldItemBase>();
			this.publicHiddenFields = new ReadOnlyCollection<PivotFieldItemBase>(hiddenFields);
			this.fieldLists = new List<PivotFieldItemBase>[PivotAreasCount];
			this.publicLists = new ReadOnlyCollection<PivotFieldItemBase>[PivotAreasCount];
			SubscribeEvents();
			GetFieldsFromData();
		}
		protected PivotGridData Data { get { return data; } }
		protected PivotGridDataAsync DataAsync { get { return Data as PivotGridDataAsync; } }
		public PivotFieldItemCollection FieldItems { get { return Data.FieldItems; } }
		public ReadOnlyCollection<PivotFieldItemBase> HiddenFields { get { return publicHiddenFields; } }
		public ReadOnlyCollection<PivotFieldItemBase> this[HeaderPresenterType header] { get { return header == HeaderPresenterType.FieldList ? HiddenFields : this[PivotEnumExtensionsBase.ToPivotArea(header)]; } }
		public ReadOnlyCollection<PivotFieldItemBase> this[PivotArea area] { get { return publicLists[(int)area] ?? (publicLists[(int)area] = new ReadOnlyCollection<PivotFieldItemBase>(GetFieldList(area))); } }
		public bool DeferUpdates {
			get { return deferUpdates; }
			set {
				if(deferUpdates == value) return;
				deferUpdates = value;
				RaiseDeferUpdatesChanged();
			}
		}
		void RaiseDeferUpdatesChanged() {
			deferUpdatesChangedHandler.SafeRaise(this, EventArgs.Empty);
		}
		public void AddFieldFilter(PivotGridFieldBase field, PivotFilterItemsBase items) {
			if(!DeferUpdates)
				return;
			field.FilterValues.SetDefereFilter((IFieldFilter)items);
			RaiseDeferUpdatesChanged();
		}
		public PivotFilterItemsBase GetFieldFilter(PivotGridFieldBase field) {
			if(!DeferUpdates)
				return null;
			return field.FilterValues.GetDefereFilter() as PivotFilterItemsBase;
		}
		public bool IsFieldFiltered(PivotGridFieldBase field) {
			return DeferUpdates && (field.FilterValues.GetDefereFilter() != null || field.Group != null && field.Group.FilterValues.GetDefereFilter() != null);
		}
		public void AddGroupFilter(PivotGridFieldBase field, PivotFilterItemsBase items) {
			if(!DeferUpdates || field.Group == null)
				return;
			field.Group.FilterValues.SetDefereFilter((IGroupFilter)items);
			RaiseDeferUpdatesChanged();
		}
		public PivotFilterItemsBase GetGroupFilter(PivotGridFieldBase field) {
			if(!DeferUpdates || field.Group == null)
				return null;
			return field.Group.FilterValues.GetDefereFilter() as PivotFilterItemsBase;
		}
		protected List<PivotFieldItemBase> GetFieldList(PivotArea area) {
			return fieldLists[(int)area] ?? (fieldLists[(int)area] = new List<PivotFieldItemBase>());
		}
		public void SetUpdateCompletedCallback(UpdateCompletedCallback callback) {
			this.updateCompletedCallback = callback;
		}
		void RaiseUpdateCompleted() {
			if(updateCompletedCallback != null)
				updateCompletedCallback();
			ResetUpdateCompleted();
		}
		void ResetUpdateCompleted() {
			this.updateCompletedCallback = null;
		}
		void RaiseListsChanged() {
			ListsChanged.SafeRaise(this, new CustomizationFormFieldsEventArgs(this));
			OnListsChanged();
		}
		protected virtual void OnListsChanged() { }
		#region IDisposable Members
		public void Dispose() {
			UnsubscribeEvents();
		}
		#endregion
		protected void SubscribeEvents() {
			Data.LayoutChangedEvent += OnDataLayoutChanged;
		}
		protected void UnsubscribeEvents() {
			Data.LayoutChangedEvent -= OnDataLayoutChanged;
		}
		protected void OnDataLayoutChanged(object sender, EventArgs e) {
			GetFieldsFromData();
		}
		protected internal virtual void GetFieldsFromData() {
			GetVisibleFieldsFromData();
			GetHiddenFieldsFromData();
			RaiseListsChanged();
		}
		void GetVisibleFieldsFromData() {
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea))) {
				List<PivotFieldItemBase> list = GetFieldList(area);
				list.Clear();
				foreach(PivotFieldItemBase field in Data.FieldItems.GetFieldItemsByArea(area, true)) {
					if(field.Group == null || field.IsFirstFieldInGroup)
						list.Add(field);
				}
			}
		}
		void GetHiddenFieldsFromData() {
			hiddenFields.Clear();
			for(int i = 0; i < Data.FieldItems.Count; i++) {
				PivotFieldItemBase field = Data.FieldItems[i];
				if(!field.Visible && field.CanShowInCustomizationForm)
					hiddenFields.Add(field);
			}
			hiddenFields.Sort(new StringComparer());
		}
		void RemoveFieldFromLists(PivotFieldItemBase field) {
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea)))
				GetFieldList(area).Remove(field);
			hiddenFields.Remove(field);
		}
		public bool MoveField(PivotFieldItemBase item, PivotArea areaTo, int index) {
			PivotGridFieldBase field = Data.GetField(item);
			if(!field.IsAreaAllowed(areaTo))
				return false;
			if(field.IsDataField && !field.CanChangeDataFieldLocationTo(areaTo, index, field.Area == areaTo ? GetFieldList(areaTo).Count - 1 : GetFieldList(areaTo).Count))
				return false;
			if(Data.FieldItems.DataFieldItem.Visible && areaTo == (this[PivotArea.ColumnArea].Contains(Data.FieldItems.DataFieldItem) ? PivotArea.ColumnArea : PivotArea.RowArea)) {
				List<PivotFieldItemBase> fields = new List<PivotFieldItemBase>();
				fields.AddRange(GetFieldList(areaTo));
				if(fields.Contains(item))
					fields.Remove(item);
				InsertFieldInList(item, index, fields);
				bool canDrag = field.CanChangeDataFieldLocationTo(areaTo, fields.IndexOf(Data.FieldItems.DataFieldItem), fields.Count - 1);
				if(!canDrag)
					return false;
			}
			RemoveFieldFromLists(item);
			InsertFieldInList(item, index, GetFieldList(areaTo));
			ApplyFieldChanges();
			return true;
		}
		void ApplyFieldChanges() {
			if(!DeferUpdates) {
				SetFieldsToData();
			} else {
				RaiseListsChanged();
				RaiseUpdateCompleted();
			}
		}
		void InsertFieldInList(PivotFieldItemBase field, int index, List<PivotFieldItemBase> fieldsList) {
			if(index < 0 || index >= fieldsList.Count)
				fieldsList.Add(field);
			else
				fieldsList.Insert(index, field);
		}
		public void HideField(PivotFieldItemBase field) {
			RemoveFieldFromLists(field);
			hiddenFields.Add(field);
			hiddenFields.Sort(new StringComparer());
			ApplyFieldChanges();
		}
		public virtual void SetFieldsToData() {
			List<FieldChange> fieldChanges = CreateFieldChangeList();
			if(fieldChanges.Count == 0) {
				RaiseUpdateCompleted();
				return;
			}
			BeforeSettingFields.SafeRaise(this, new CustomizationFormFieldsEventArgs(this));
			SetFieldsToDataCore(fieldChanges, DataAsync != null);
		}
		void SetFieldsToDataCore(List<FieldChange> fieldChanges, bool isAsync) {
			Data.BeginUpdate();
			Dictionary<PivotGridFieldBase, PivotFilterItemsBase> fieldFilters = GetFieldFilters(Data);
			Dictionary<PivotGridFieldBase, PivotFilterItemsBase> groupFilters = GetGroupFilters(Data);
			bool changed = false;
			for(int i = 0; i < fieldChanges.Count; i++)
				if(fieldChanges[i].IsFilterChanged) {
					changed = true;
					break;
				}
			Dictionary<PivotGridFieldBase, int> dataIndexes = new Dictionary<PivotGridFieldBase, int>();
			foreach(PivotGridFieldBase field in Data.GetFieldsByArea(PivotArea.DataArea, false))
				dataIndexes.Add(field, field.AreaIndex);
			List<PivotGridFieldBase> hiddenData = new List<PivotGridFieldBase>();
			try {
				foreach(PivotFieldItemBase fd in hiddenFields) {
					PivotGridFieldBase field = Data.GetField(fd);
					if(field.Visible) {
						Data.GetField(fd).Visible = false;
						if(field.Area != PivotArea.DataArea && !(!Data.IsOLAP && field.Area == PivotArea.FilterArea && !field.NeedApplyFilterOnShowInFilterArea))
							changed = true;
						else
							if(field.Area == PivotArea.DataArea)
								hiddenData.Add(field);
					}
				}
				foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea))) {
					List<PivotFieldItemBase> list = GetFieldList(area);
					int counter = 0;
					for(int i = 0; i < list.Count; i++) {
						PivotGridFieldBase field =  Data.GetField(list[i]);
						bool filterFieldShow = !field.Visible && !Data.IsOLAP && 
												   area == PivotArea.FilterArea && !field.NeedApplyFilterOnShowInFilterArea;
						bool filterFieldMoving = field.Area == PivotArea.FilterArea && area == PivotArea.FilterArea && field.Visible;
						bool dataFieldMoving = field.Area == PivotArea.DataArea && field.Visible && area == PivotArea.DataArea;
						if(!filterFieldShow && !filterFieldMoving && !dataFieldMoving && (field.AreaIndex != counter || field.Area != area || !field.Visible))
							changed = true;
						field.SetAreaPosition(area, counter);
						field.Visible = true;
						if(field.Group != null)
							counter += field.Group.Fields.Count;
						else
							counter++;
					}
				}
				foreach(KeyValuePair<PivotGridFieldBase, PivotFilterItemsBase> item in fieldFilters) {
					if(item.Key.Visible)
						item.Value.ApplyFilter(false, false, false);
					item.Key.FilterValues.SetDefereFilter(null);
				}
				foreach(KeyValuePair<PivotGridFieldBase, PivotFilterItemsBase> item in groupFilters) {
					if(item.Key.Visible)
						item.Value.ApplyFilter(false, false, false);
					item.Key.Group.FilterValues.SetDefereFilter(null);
				}
			} finally {
				AsyncProcess process = () => EndUpdate(fieldChanges, changed, dataIndexes, hiddenData);
				AsyncCompletedHandler asyncCompleted = (d) => OnSetFieldsToDataCompleted(fieldChanges);
				if (isAsync)
				{
					DataAsync.InvokeAsync(process, asyncCompleted);
				} else {
					process();
					asyncCompleted(null);
				}
			}
		}
		object EndUpdate(List<FieldChange> fieldChanges, bool changed, Dictionary<PivotGridFieldBase, int> dataIndexes, List<PivotGridFieldBase> hiddenData) {
			if(!changed) {
				hiddenData.Sort((PivotGridFieldBase x, PivotGridFieldBase y) => {
					return Comparer<int>.Default.Compare(dataIndexes[x], dataIndexes[y]);
				});
				for(int i = hiddenData.Count - 1; i >= 0; i--)
					Data.HideDataField(hiddenData[i], dataIndexes[hiddenData[i]]);
				Data.GetSortedFields();
			}
			if(IsHideFieldChanges(fieldChanges)) {
				if(ShouldReloadDataOnly(fieldChanges)) {
					Data.CancelUpdate();
					Data.ReloadData();
				} else {
					if(changed) {
						Data.EndUpdate();
					} else {
						Data.CancelUpdate();
						Data.LayoutChanged();
					}
				}
			} else {
				if(IsShowUnboundFieldChanges(fieldChanges)) {
					Data.CancelUpdate();
					Data.ReloadData();
				} else {
					if(changed) {
						Data.EndUpdate();
					} else {
						Data.CancelUpdate();
						Data.LayoutChanged();
					}
				}
			}
			return null;
		}
		static Dictionary<PivotGridFieldBase, PivotFilterItemsBase> GetFieldFilters(PivotGridData data) {
			Dictionary<PivotGridFieldBase, PivotFilterItemsBase> fieldFilters = new Dictionary<PivotGridFieldBase, PivotFilterItemsBase>();
			foreach(PivotGridFieldBase field in data.Fields) {
				PivotFilterItemsBase item = field.FilterValues.GetDefereFilter() as PivotFilterItemsBase;
				if(item != null)
					fieldFilters.Add(field, item);
			}
			return fieldFilters;
		}
		static Dictionary<PivotGridFieldBase, PivotFilterItemsBase> GetGroupFilters(PivotGridData data) {
			Dictionary<PivotGridFieldBase, PivotFilterItemsBase> fieldFilters = new Dictionary<PivotGridFieldBase, PivotFilterItemsBase>();
			foreach(PivotGridFieldBase field in data.Fields) {
				PivotFilterItemsBase item = field.Group != null && field.Group[0] == field ? field.Group.FilterValues.GetDefereFilter() as PivotFilterItemsBase : null;
				if(item != null)
					fieldFilters.Add(field, item);
			}
			return fieldFilters;
		}
		static internal void ClearDefereFilters(PivotGridData data) {
			Dictionary<PivotGridFieldBase, PivotFilterItemsBase> filters = GetFieldFilters(data);
			foreach(KeyValuePair<PivotGridFieldBase, PivotFilterItemsBase> pair in filters)
				pair.Key.FilterValues.SetDefereFilter(null);
			filters = GetGroupFilters(data);
			foreach(KeyValuePair<PivotGridFieldBase, PivotFilterItemsBase> pair in filters)
				pair.Key.Group.FilterValues.SetDefereFilter(null);
		}
		void OnSetFieldsToDataCompleted(List<FieldChange> fieldChanges) {
			RaiseChangedFieldListEvents(fieldChanges, false);
			AfterSettingFields.SafeRaise(this, new CustomizationFormFieldsEventArgs(this));
			RaiseListsChanged();
			RaiseUpdateCompleted();
		}
		bool IsHideFieldChanges(List<FieldChange> fieldChanges) {
			return fieldChanges.Count == 1 && fieldChanges[0].IsVisibleChangeOnly;
		}
		bool ShouldReloadDataOnly(List<FieldChange> fieldChanges) {
			if(!IsHideFieldChanges(fieldChanges))
				return false;
			PivotGridFieldBase field = fieldChanges[0].Field;
			return (field.Area != PivotArea.DataArea || field.Visible || field.AreaIndex < 0) && field.GroupInterval != PivotGroupInterval.Default;
		}
		bool IsShowUnboundFieldChanges(List<FieldChange> fieldChanges) {
			for(int i = 0; i < fieldChanges.Count; i++)
				if(fieldChanges[i].IsVisibilityChanged && fieldChanges[i].Field.GroupInterval != PivotGroupInterval.Default && fieldChanges[i].Field.Visible)
					return true;
			return false;
		}
		public string GetHierarchyCaption(string hierarchy) {
			return Data.GetHierarchyCaption(hierarchy);
		}
		class FieldChange {
			readonly PivotGridFieldBase field;
			readonly bool isAreaChanged;
			readonly bool isAreaIndexChanged;
			readonly bool isVisibilityChanged;
			readonly bool isFilterChanged;
			public FieldChange(PivotGridFieldBase field, bool isAreaChanged, bool isAreaIndexChanged, bool isVisibilityChanged, bool isFilterChanged) {
				this.field = field;
				this.isAreaChanged = isAreaChanged;
				this.isAreaIndexChanged = isAreaIndexChanged;
				this.isVisibilityChanged = isVisibilityChanged;
				this.isFilterChanged = isFilterChanged;
			}
			public PivotGridFieldBase Field { get { return field; } }
			public bool IsAreaChanged { get { return isAreaChanged; } }
			public bool IsAreaIndexChanged { get { return isAreaIndexChanged; } }
			public bool IsVisibilityChanged { get { return isVisibilityChanged; } }
			public bool IsVisibleChangeOnly { get { return IsVisibilityChanged && !IsAreaIndexChanged && !IsAreaChanged && !isFilterChanged; } }
			public bool IsFilterChanged { get { return isFilterChanged; } }
		}
		List<FieldChange> CreateFieldChangeList() {
			List<FieldChange> changes = new List<FieldChange>();
			foreach(PivotFieldItemBase item in hiddenFields) {
				if(item.Visible != false)
					changes.Add(new FieldChange(Data.GetField(item), false, false, true, IsFieldFiltered(data.GetField(item))));
			}
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea))) {
				List<PivotFieldItemBase> list = GetFieldList(area);
				for(int i = 0; i < list.Count; i++) {
					if(IsFieldFiltered(data.GetField(list[i])) || list[i].Area != area || list[i].AreaIndex != i || list[i].Visible != true) {
						changes.Add(new FieldChange(Data.GetField(list[i]), list[i].Area != area,
							list[i].AreaIndex != i, list[i].Visible != true, IsFieldFiltered(data.GetField(list[i]))));
					}
				}
			}
			return changes;
		}
		void RaiseChangedFieldListEvents(List<FieldChange> changes, bool doRefresh) {
			foreach(FieldChange change in changes) {
				if(change.IsAreaChanged) Data.OnFieldAreaChanged(change.Field);
				if(change.IsAreaIndexChanged) Data.OnFieldAreaIndexChanged(change.Field, false, false);
				if(change.IsVisibilityChanged) Data.OnFieldVisibleChanged(change.Field, -1, doRefresh);
				if(change.IsFilterChanged)
					if(change.Field.Group == null)
						Data.OnFieldFilteringChanged(change.Field);
					else
						Data.OnGroupFilteringChanged(change.Field.Group);
			}
		}
		public PivotGridStringId GetStringId(HeaderPresenterType headerPresenterType) {
			switch(headerPresenterType) {
				case HeaderPresenterType.FieldList: return PivotGridStringId.CustomizationFormHiddenFields;
				default: return GetStringId(PivotEnumExtensionsBase.ToPivotArea(headerPresenterType));
			}
		}
		public PivotGridStringId GetStringId(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea: return PivotGridStringId.ColumnArea;
				case PivotArea.RowArea: return PivotGridStringId.RowArea;
				case PivotArea.FilterArea: return PivotGridStringId.FilterArea;
				case PivotArea.DataArea: return PivotGridStringId.DataArea;
				default: throw new ArgumentException("area");
			}
		}
		class StringComparer : IComparer<PivotFieldItemBase> {
			int IComparer<PivotFieldItemBase>.Compare(PivotFieldItemBase x, PivotFieldItemBase y) {
				return Comparer.Default.Compare(x.ToString(), y.ToString());
			}
		}
		List<PivotGridFieldBase> ICustomFilterColumnsProvider.Fields {
			get {
				List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
				AddDefereFilteredFields(fields, false, this[PivotArea.ColumnArea]);
				AddDefereFilteredFields(fields, false, this[PivotArea.RowArea]);
				AddDefereFilteredFields(fields, false, this[PivotArea.FilterArea]);
				if(!Data.OptionsData.FilterByVisibleFieldsOnly)
					AddDefereFilteredFields(fields, true, HiddenFields);
				return fields;
			}
		}
		void AddDefereFilteredFields(List<PivotGridFieldBase> fields, bool checkFilterArea, IList<PivotFieldItemBase> items) {
			for(int i = 0; i < items.Count; i++)
				if((!checkFilterArea || checkFilterArea && items[i].Area == PivotArea.FilterArea) &&
					(Data.GetField(items[i]).Group != null && Data.GetField(items[i]).Group.FilterValues.GetActual(true).HasFilter
					||  Data.GetField(items[i]).FilterValues.GetActual(true).HasFilter))
					fields.Add(Data.GetField(items[i]));
		}
	}
}
