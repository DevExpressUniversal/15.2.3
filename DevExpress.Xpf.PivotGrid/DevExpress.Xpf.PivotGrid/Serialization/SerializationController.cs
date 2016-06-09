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
using System.Collections.Generic;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.PivotGrid.Serialization {
	public class PivotSerializationController {
		public const string DefaultID = "PivotGridControl";
		#region statics
		static readonly Dictionary<string, object> obsoleteProps;
		static PivotSerializationController() {
			obsoleteProps = new Dictionary<string, object>();
			obsoleteProps.Add("ChartDataVertical", new object());
			obsoleteProps.Add("ChartExportFieldValueMode", new object());
			obsoleteProps.Add("ChartShowColumnTotals", new object());
			obsoleteProps.Add("ChartShowColumnGrandTotals", new object());
			obsoleteProps.Add("ChartShowColumnCustomTotals", new object());
			obsoleteProps.Add("ChartShowRowTotals", new object());
			obsoleteProps.Add("ChartShowRowGrandTotals", new object());
			obsoleteProps.Add("ChartShowRowCustomTotals", new object());
			obsoleteProps.Add("ChartExportColumnFieldValuesAsType", new object());
			obsoleteProps.Add("ChartExportRowFieldValuesAsType", new object());
			obsoleteProps.Add("ChartExportCellValuesAsType", new object());
		}
		public static bool IsPropertyObsolete(string propName) {
			return obsoleteProps.ContainsKey(propName);
		}
		#endregion
		PivotGridControl pivot;
		AsyncCompletedHandler asyncCompleted;
		public PivotSerializationController(PivotGridControl pivot) {
			this.pivot = pivot;
			this.asyncCompleted = DefaultAsyncCompletedProc;
		}
		AsyncCompletedHandler DefaultAsyncCompletedProc {
			get { return DevExpress.Xpf.PivotGrid.Internal.AsyncModeHelper.DoEmptyComplete; }
		}
		protected PivotGridControl PivotGrid { get { return pivot; } }
		protected virtual string GetAppName() {
			return "PivotGridControl";
		}
		public void SaveLayout(object path) {
			DXSerializer.SerializeSingleObject(PivotGrid, path, GetAppName());
		}
		public void RestoreLayout(object path) {
			DXSerializer.DeserializeSingleObject(PivotGrid, path, GetAppName());
		}
		public void RestoreLayoutAsync(object path, AsyncCompletedHandler completed) {
			this.asyncCompleted = completed;
			try {
				RestoreLayout(path);
			} finally {
				this.asyncCompleted = DefaultAsyncCompletedProc;
			}
		}
		public virtual void OnClearCollection(XtraItemRoutedEventArgs e) {
			switch(e.Item.Name) {
				case SerializationPropertiesNames.Fields:
					OnClearFields(e);
					break;
				case SerializationPropertiesNames.Groups:
					OnClearGroups(e);
					break;
				case SerializationPropertiesNames.GroupFields:
					OnClearGroupFields(e);
					break;
				case SerializationPropertiesNames.SortByConditions:
					OnClearSortByConditions(e);
					break;
				case SerializationPropertiesNames.MRUFilters:
					OnClearMRUFilters(e);
					break;
			}
		}
		public virtual object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.Fields:
					return OnCreateField(e.Item);
				case SerializationPropertiesNames.Groups:
					return OnCreateGroup(e.Item);
				case SerializationPropertiesNames.SortByConditions:
					return OnCreateSortByCondition((SortByConditionCollection)e.Collection, e.Item);
				case SerializationPropertiesNames.MRUFilters:
					return OnCreateMRUFilters(e.Item);
				case SerializationPropertiesNames.FormatConditions:
					return OnCreateFormatCondition(e.Item);
			}
			return null;
		}
		public virtual object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.Fields:
					return OnFindField(e.Item);
				case SerializationPropertiesNames.Groups:
					return OnFindGroup(e.Item);
				case SerializationPropertiesNames.GroupFields:
					return OnFindGroupField(e.Item);
			}
			return null;
		}
		public virtual void OnStartDeserializing(DependencyObject obj, DevExpress.Utils.LayoutAllowEventArgs e) {
			if(obj == PivotGrid) {
				OnStartGridDeserializing(e);
				OnDeserializeFormatConditionsStart();
				return;
			}
			PivotGridField field = obj as PivotGridField;
			if(field != null)
				field.OnStartDeserializing();
		}
		void OnDeserializeFormatConditionsStart() {
			pivot.FormatConditions.BeginUpdate();
		}
		void OnDeserializeFormatConditionsEnd() {
			foreach(FormatConditionBase condition in pivot.FormatConditions)
				condition.OnDeserializeEnd();
			pivot.FormatConditions.EndUpdate();
		}
		public virtual void OnEndDeserializing(DependencyObject obj, string restoredVersion) {
			if(obj == PivotGrid) {
				OnDeserializeFormatConditionsEnd();
				OnEndGridDeserializing(restoredVersion);
			}
		}
		public virtual bool OnAllowProperty(AllowPropertyEventArgs e) {
			if(IgnoreProperty(e))
				return false;
			StoreLayoutMode storeMode = PivotSerializationOptions.GetStoreLayoutMode(PivotGrid);
			if(storeMode == StoreLayoutMode.AllOptions)
				return true;
			switch(e.PropertyId) {
				case PivotSerializationOptions.AppearanceID:
					return (storeMode & StoreLayoutMode.Appearance) == StoreLayoutMode.Appearance;
				case PivotSerializationOptions.DataSettingsID:
					return (storeMode & StoreLayoutMode.DataSettings) == StoreLayoutMode.DataSettings;
				case PivotSerializationOptions.VisualOptionsID:
					return (storeMode & StoreLayoutMode.VisualOptions) == StoreLayoutMode.VisualOptions;
				case PivotSerializationOptions.LayoutID:
					return (storeMode & StoreLayoutMode.Layout) == StoreLayoutMode.Layout;
				case PivotSerializationOptions.StoreAlwaysID:
					return true;
			}
			return true;
		}
		bool IgnoreProperty(AllowPropertyEventArgs e) {
			if(!e.IsSerializing)
				return false;
			DevExpress.XtraPivotGrid.PivotGridFieldFilterValues filters = e.Source as DevExpress.XtraPivotGrid.PivotGridFieldFilterValues;
			if(e.IsSerializing && filters != null && e.Property.Name == "Values" && filters.Count > 0 && !(filters.Values[0] is Enum))
				return true;
			if(e.IsSerializing && filters != null && e.Property.Name == "ValuesCore" && filters.Count > 0 && filters.Values[0] is Enum)
				return true;
			return IsPropertyObsolete(e.Property.Name);
		}
		protected string GetPropertyName(XtraPropertyInfo propertyInfo) {
			return GetStringProperty(propertyInfo, "Name");
		}
		protected string GetPropertyHierarchy(XtraPropertyInfo propertyInfo) {
			return GetStringProperty(propertyInfo, "Hierarchy");
		}
		protected string GetStringProperty(XtraPropertyInfo propertyInfo, string name) {
			if(propertyInfo.ChildProperties == null)
				return null;
			XtraPropertyInfo xp = propertyInfo.ChildProperties[name];
			if(xp != null && xp.Value != null)
				return xp.Value.ToString();
			return null;
		}
		protected virtual void OnClearFields(XtraItemRoutedEventArgs e) {
			bool addNewColumns = PivotSerializationOptions.GetAddNewFields(PivotGrid);
			List<PivotGridField> commonFields = GetCommonFields(e.Item.ChildProperties);
			List<PivotGridField> newFields = GetNewFields(commonFields);
			if(!addNewColumns)
				RemoveNewFields(newFields);
			else
				SetNewFields(newFields);
		}
		protected virtual PivotGridField OnFindField(XtraPropertyInfo propertyInfo) {
			string name = GetPropertyName(propertyInfo);
			if(!string.IsNullOrEmpty(name))
				return PivotGrid.Fields.GetFieldByName(name);
			return null;
		}
		protected virtual PivotGridGroup OnFindGroup(XtraPropertyInfo propertyInfo) {
			string name = GetPropertyHierarchy(propertyInfo);
			if(!string.IsNullOrEmpty(name)) {
				foreach(PivotGridGroup group in PivotGrid.Groups)
					if(name == group.Hierarchy)
						return group;
			}
			return null;
		}
		protected virtual PivotGridField OnCreateField(XtraPropertyInfo propertyInfo) {
			if(PivotSerializationOptions.GetRemoveOldFields(PivotGrid))
				return null;
			PivotGridField field = PivotGrid.Fields.Add();
			field.FieldName = GetStringProperty(propertyInfo, "FieldName");
			return field;
		}
		List<PivotGridField> GetNewFields(List<PivotGridField> savingFields) {
			List<PivotGridField> newFields = new List<PivotGridField>();
			for(int i = PivotGrid.Fields.Count - 1; i >= 0; i--) {
				PivotGridField field = PivotGrid.Fields[i];
				if(!savingFields.Contains(field))
					newFields.Add(field);
			}
			return newFields;
		}
		void SetNewFields(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				field.InternalField.IsNew = true;
		}
		List<PivotGridField> GetCommonFields(IXtraPropertyCollection layoutItems) {
			List<PivotGridField> commonFields = new List<PivotGridField>();
			if(layoutItems == null)
				return commonFields;
			foreach(XtraPropertyInfo pInfo in layoutItems) {
				PivotGridField commonField = OnFindField(pInfo);
				if(commonField != null)
					commonFields.Add(commonField);
			}
			return commonFields;
		}
		void RemoveNewFields(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				PivotGrid.Fields.Remove(field);
		}
		protected virtual void OnClearGroups(XtraItemRoutedEventArgs e) {
			bool clearGroups = !PivotSerializationOptions.GetAddNewGroups(PivotGrid);
			if(clearGroups)
				PivotGrid.Groups.Clear();
		}
		protected virtual PivotGridGroup OnCreateGroup(XtraPropertyInfo propertyInfo) {
			return PivotGrid.Groups.Add();
		}
		protected virtual void OnClearGroupFields(XtraItemRoutedEventArgs e) {
			((IList)e.Collection).Clear();
		}
		protected virtual PivotGridField OnFindGroupField(XtraPropertyInfo propertyInfo) {
			string name = Convert.ToString(propertyInfo.Value);
			if(string.IsNullOrEmpty(name))
				return null;
			return PivotGrid.Fields.GetFieldByName(name);
		}
		protected virtual void OnClearSortByConditions(XtraItemRoutedEventArgs e) {
			((IList)e.Collection).Clear();
		}
		protected virtual SortByCondition OnCreateSortByCondition(SortByConditionCollection collection, XtraPropertyInfo propertyInfo) {
			SortByCondition res = new SortByCondition();
			collection.Add(res);
			return res;
		}
		protected virtual void OnClearMRUFilters(XtraItemRoutedEventArgs e) {
			PivotGrid.MRUFiltersInternal.Clear();
		}
		protected virtual object OnCreateMRUFilters(XtraPropertyInfo Item) {
			XtraPropertyInfo filterTextProperty = Item.ChildProperties["FilterText"];
			CriteriaOperator op = GetCriteriaOperator(Item);
			CriteriaOperatorInfo info = new CriteriaOperatorInfo(op, filterTextProperty.Value.ToString());
			PivotGrid.MRUFiltersInternal.Add(info);
			return info;
		}
		protected virtual object OnCreateFormatCondition(XtraPropertyInfo item) {
			XtraPropertyInfo typeNamePropertyInfo = item.ChildProperties["TypeName"];
			if(typeNamePropertyInfo == null)
				return null;
			FormatConditionBase condition = CreateFormatCondition(typeNamePropertyInfo.Value.ToString());
			if(condition == null)
				return null;
			condition.OnDeserializeStart();
			if(item.ChildProperties["Format"] != null) {
				DependencyProperty formatProperty = condition.FormatPropertyForBinding;
				condition.SetValue(formatProperty, Activator.CreateInstance(formatProperty.PropertyType));
			}
			PivotGrid.FormatConditions.Add(condition);
			return condition;
		}
		protected virtual FormatConditionBase CreateFormatCondition(string conditionTypeName) {
			var assembly = typeof(FormatConditionBase).Assembly;
			if(assembly == null)
				return null;
			Type type = assembly.GetType(String.Format("{0}.{1}", XmlNamespaceConstants.PivotGridNamespace, conditionTypeName));
			if(type == null)
				return null;
			return Activator.CreateInstance(type) as FormatConditionBase;
		}
		protected virtual void OnStartGridDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			PivotLayoutAllowEventArgs routedEventArgs = new PivotLayoutAllowEventArgs(PivotGridControl.BeforeLoadLayoutEvent, e);
			PivotGrid.RaiseBeforeLoadLayout(routedEventArgs);
			e.Allow = routedEventArgs.Allow;
			if(!e.Allow)
				return;
			pivot.Data.SetIsDeserializing(true);
			PivotGrid.BeginUpdate();
		}
		protected virtual void OnEndGridDeserializing(string restoredVersion) {
			if(DXSerializer.GetLayoutVersion(PivotGrid) != restoredVersion)
				PivotGrid.RaiseLayoutUpgrade(new DevExpress.Utils.LayoutUpgradeEventArgs(restoredVersion));
			PivotGrid.OnEndDeserializing();
			PivotGrid.Fields.OnEndDeserializing();
			pivot.Data.SetIsDeserializing(false);
			if(PivotGrid.UseAsyncMode)
				PivotGrid.EndUpdateAsync(this.asyncCompleted);
			else {
				PivotGrid.EndUpdate();
				this.asyncCompleted.Invoke(null);
			}
		}
		CriteriaOperator GetCriteriaOperator(XtraPropertyInfo xtraPropertyInfo) {
			XtraPropertyInfo filterOperatorProperty = xtraPropertyInfo.ChildProperties["FilterOperator"];
			if(filterOperatorProperty == null) {
				XtraPropertyInfo filterStringProperty = xtraPropertyInfo.ChildProperties["FilterString"];
				String filterString = (String)filterStringProperty.ValueToObject(typeof(String));
				return CriteriaOperator.Parse(filterString);
			}
			return filterOperatorProperty.ValueToObject(typeof(CriteriaOperator)) as CriteriaOperator;
		}
	}
}
