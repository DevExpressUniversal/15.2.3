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
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Browsing;
using System.Collections;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraReports.Native.CalculatedFields;
using System.Data;
using System.IO;
using System.Globalization;
using System.Linq;
namespace DevExpress.XtraReports.Native.Data {
	internal class EmptyListBrowser : ListBrowser {
		public override int Position { get { return 0; } set { } }
		public EmptyListBrowser()
			: base(new ListControllerBase(), true) {
			SetDataSource(new object[0]);
		}
	}
	public class SortedListController : ListControllerBase, IFilteredListController {
		#region static
		static GroupField[] ComposeGroupFields(ListSourceDataController dataController, GroupField[] groupFields) {
			if(groupFields == null || groupFields.Length == 0)
				return new GroupField[0];
			List<GroupField> result = new List<GroupField>();
			int index = 0;
			while(index < groupFields.Length) {
				GroupField current = groupFields[index];
				GroupHeaderBand groupHeaderBand = current.Band as GroupHeaderBand;
				if(groupHeaderBand != null) {
					List<GroupField> oneLevelGroupFields = new List<GroupField>();
					for(int i = index; i < groupFields.Length; i++) {
						if(groupHeaderBand != groupFields[i].Band)
							break;
						oneLevelGroupFields.Add(groupFields[i]);
					}
					if(oneLevelGroupFields.Count > 1) {
						CompositeGroupField compositeGroupField = new CompositeGroupField(dataController, oneLevelGroupFields.ToArray());
						result.Add(compositeGroupField);
						index += oneLevelGroupFields.Count;
						continue;
					}
				}
				result.Add(current);
				index++;
			}
			return result.ToArray();
		}
		#endregion
		#region inner classes
		class SortedValue {
			readonly object value;
			readonly XRColumnSortOrder sortOrder;
			public SortedValue(object value, XRColumnSortOrder sortOrder) {
				this.value = value;
				this.sortOrder = sortOrder;
			}
			public object Value { get { return value; } }
			public XRColumnSortOrder SortOrder { get { return sortOrder; } }
		}
		class CompositeValue : IComparable {
			static bool IsNull(object value) {
				return value == null || value is DBNull;
			}
			readonly SortedValue[] values;
			readonly XRColumnSortOrder baseSortOrder;
			public CompositeValue(XRColumnSortOrder baseSortOrder, params SortedValue[] values) {
				System.Diagnostics.Debug.Assert(values.Length > 0);
				this.values = values;
				this.baseSortOrder = baseSortOrder;
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
			public override bool Equals(object obj) {
				int result = CompareTo(obj);
				return result == 0;
			}
			#region IComparable Members
			public int CompareTo(object obj) {
				CompositeValue compositeValue = obj as CompositeValue;
				if(compositeValue == null)
					return 1;
				if(values == null && compositeValue.values == null)
					return 0;
				for(int i = 0; i < values.Length; i++) {
					SortedValue value1 = values[i];
					SortedValue value2 = compositeValue.values[i];
					if(IsNull(value1.Value) && IsNull(value2.Value))
						continue;
					if(!IsNull(value1.Value) && IsNull(value2.Value))
						return 1;
					if(IsNull(value1.Value) && !IsNull(value2.Value))
						return -1;
					int result = Comparer.Default.Compare(value1.Value, value2.Value);
					if(result != 0) {
						result = value1.SortOrder == baseSortOrder ? result : -result;
						return result;
					}
				}
				return 0;
			}
			#endregion
		}
		class CompositeGroupField : GroupField {
			readonly GroupField[] groupFields;
			readonly CalculatedField calculatedField = new CalculatedField();
			readonly Band band;
			readonly ListSourceDataController dataController;
			public CompositeGroupField(ListSourceDataController dataController, params GroupField[] groupFields) {
				System.Diagnostics.Debug.Assert(dataController != null);
				System.Diagnostics.Debug.Assert(groupFields.Length > 1);
				this.dataController = dataController;
				this.groupFields = groupFields;
				calculatedField.GetValue += new GetValueEventHandler(calculatedField_GetValue);
				calculatedField.Name = Path.GetRandomFileName().Replace(".", "");
				FieldName = calculatedField.Name;
				SortOrder = XRColumnSortOrder.Ascending;
				band = groupFields[0].Band;
			}
			public CalculatedField CalculatedField { get { return calculatedField; } }
			public GroupField[] GroupFields { get { return groupFields; } }
			internal override Band Band {
				get { return band; }
			}
			void calculatedField_GetValue(object sender, GetValueEventArgs e) {
				if(e.Row == null)
					return;
				List<SortedValue> values = new List<SortedValue>();
				foreach(GroupField groupField in groupFields) {
					DataColumnInfo columnInfo = dataController.Columns[groupField.FieldName];
					if(columnInfo == null || columnInfo.PropertyDescriptor == null)
						continue;
					object value = columnInfo.PropertyDescriptor.GetValue(e.Row);
					values.Add(new SortedValue(value, groupField.SortOrder));
				}
				if(values.Count > 0)
					e.Value = new CompositeValue(SortOrder, values.ToArray());
			}
		}
#if DEBUGTEST
		public
#endif
		class CustomListSourceDataController : ListSourceDataController {
			protected override BaseDataControllerHelper CreateHelper() {
				BaseDataControllerHelper helper = base.CreateHelper();
				if(helper.GetType() == typeof(ListDataControllerHelper))
					helper = new CustomListDataControllerHelper(this);
				return helper;
			}
		}
		class CustomListDataControllerHelper : ListDataControllerHelper {
			public CustomListDataControllerHelper(DataControllerBase controller)
				: base(controller) {
			}
			protected override PropertyDescriptorCollection GetTypeProperties(Type rowType) {
				if(rowType.IsInterface) {
					List<PropertyDescriptor> properties = GetInterfaceProperties(rowType);
					return new PropertyDescriptorCollection(properties.ToArray());
				}
				return TypeDescriptor.GetProperties(rowType);
			}
			static List<PropertyDescriptor> GetInterfaceProperties(Type rowType) {
				List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
				properties.AddRange(TypeDescriptor.GetProperties(rowType).Cast<PropertyDescriptor>());
				foreach(Type type in rowType.GetInterfaces())
					properties.AddRange(TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>());
				return properties;
			}
		}
		#endregion
		ListSourceDataController dataController;
		GroupField[] originalGroupFields;
		XRGroupSortingSummary[] sortingSummary;
		CalculatedFieldCollection calculatedFields;
		Dictionary<int, XRGroupSortingSummary> levelToSummaryDictionary;
		List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
		CriteriaOperator filterCriteria;
		RowIndicesMapper indicesMapper;
		DataBrowser dataBrowser;
		RowIndicesMapper IndicesMapper {
			get {
				if(indicesMapper == null)
					indicesMapper = new RowIndicesMapper(dataController);
				return indicesMapper;
			}
		}
		void InvalidateIndicesMapper() {
			indicesMapper = null;
		}
		public CriteriaOperator FilterCriteria {
			get {
				return filterCriteria;
			}
			set {
				if(dataController != null) {
					this.filterCriteria = value;
					dataController.FilterCriteria = value;
					InvalidateIndicesMapper();
				}
			}
		}
		public DataController DataController {
			get { return dataController; }
		}
		public override int Count {
			get { return dataController != null ? dataController.VisibleListSourceRowCount : 0; }
		}
		public void SetBrowser(DataBrowser dataBrowser) {
			this.dataBrowser = dataBrowser;
		}
		public override object GetItem(int index) {
			if(dataController == null)
				return null;
			int controllerRowIndex = IndicesMapper[index];
			return dataController.GetRow(controllerRowIndex);
		}
		public override object GetColumnValue(int position, string columnName) {
			if(dataController == null)
				return null;
			int controllerRowIndex = IndicesMapper[position];
			return dataController.GetRowValue(controllerRowIndex, columnName);
		}
		public void Initialize(CalculatedFieldCollection calculatedFields, GroupField[] groupFields, XRGroupSortingSummary[] sortingSummary, CriteriaOperator filterCriteria) {
			this.calculatedFields = calculatedFields;
			this.originalGroupFields = groupFields;
			this.sortingSummary = sortingSummary;
			this.filterCriteria = filterCriteria;
			if(dataController != null) {
				UpdateCore();
			}
		}
		public void UpdateDataController(CalculatedFieldCollection calculatedFields, GroupField[] groupFields, XRGroupSortingSummary[] sortingSummary, CriteriaOperator filterCriteria) {
			this.calculatedFields = calculatedFields;
			this.originalGroupFields = groupFields;
			this.sortingSummary = sortingSummary;
			this.filterCriteria = filterCriteria;
			ResetDataController();
			dataBrowser.RaiseCurrentChanged();
		}
		void UpdateCore() {
			if(calculatedFields == null)
				return;
			GroupField[] composedGroupFields = ComposeGroupFields(dataController, originalGroupFields);
			InitializeProperties(calculatedFields, composedGroupFields);
			ApplyProperties();
			GroupData(composedGroupFields);
			AssignFilterCriteria();
			InvalidateIndicesMapper();
		}
		protected internal override void SetList(IList list) {
			if(this.list == list)
				return;
			this.list = list;
			ResetDataController();
		}
		void ResetDataController() {
			DisposeDataController();
			if(list == null) return;
			dataController = new CustomListSourceDataController();
			dataController.CustomSummary += new CustomSummaryEventHandler(dataController_CustomSummary);
			dataController.BeginUpdate();
			try {
				dataController.ListSource = list;
				UpdateCore();
			} finally {
				dataController.EndUpdate();
			}
		}
		void AssignFilterCriteria() {
			if(!ReferenceEquals(this.filterCriteria, null))
				dataController.FilterCriteria = this.filterCriteria;
		}
		void InitializeProperties(GroupField groupField) {
			PropertyDescriptor property = null;
			if(dataController.Columns[groupField.FieldName] == null) {
				CompositeGroupField compositeGroupField = groupField as CompositeGroupField;
				if(compositeGroupField != null) {
					foreach(GroupField child in compositeGroupField.GroupFields)
						InitializeProperties(child);
					property = new CalculatedPropertyDescriptor(compositeGroupField.CalculatedField, null);
				} else
					property = RelatedPropertyDescriptor.CreateInstance(dataBrowser, groupField.FieldName);
			}
			if(property != null)
				properties.Add(property);
		}
		void InitializeProperties(CalculatedFieldCollection calculatedFields, GroupField[] groupFields) {
			System.Diagnostics.Debug.Assert(dataController != null);
			properties.Clear();
			PropertyDescriptorCollection itemProperties = null;
			foreach(CalculatedField item in calculatedFields) {
				if(dataController.Columns[item.Name] == null) {
					if(itemProperties == null)
						itemProperties = dataBrowser.GetItemProperties();
					PropertyDescriptor property = itemProperties.Find(item.Name, true);
					if(property is CalculatedPropertyDescriptor)
						properties.Add(property);
				}
			}
			foreach(GroupField item in groupFields)
				InitializeProperties(item);
			foreach(XRGroupSortingSummary item in sortingSummary) {
				if(dataController.Columns[item.FieldName] == null) {
					PropertyDescriptor property = RelatedPropertyDescriptor.CreateInstance(dataBrowser, item.FieldName);
					if(property != null)
						properties.Add(property);
				}
			}
		}
		void ApplyProperties() {
			if(properties != null) {
				foreach(PropertyDescriptor property in properties) {
					dataController.Columns.Add(property);
					dataController.Helper.DescriptorCollection.Add(property);
				}
			}
		}
		void GroupData(GroupField[] groupFields) {
			levelToSummaryDictionary = new Dictionary<int, XRGroupSortingSummary>();
			if(dataController == null) return;
			List<DataColumnSortInfo> dataColumnSortInfos = new List<DataColumnSortInfo>(groupFields.Length);
			List<SummarySortInfo> summarySortInfos = new List<SummarySortInfo>(groupFields.Length);
			FillSortInfos(groupFields, dataColumnSortInfos, summarySortInfos);
			dataController.GroupSummary.Clear();
			foreach(SummarySortInfo item in summarySortInfos)
				dataController.GroupSummary.Add(item.SummaryItem);
			dataController.UpdateSortGroup(dataColumnSortInfos.ToArray(), dataColumnSortInfos.Count, summarySortInfos.ToArray());
		}
		void FillSortInfos(GroupField[] groupFields, IList<DataColumnSortInfo> dataColumnSortInfos, IList<SummarySortInfo> summarySortInfos) {
			if(groupFields == null)
				return;
			System.Diagnostics.Debug.Assert(dataController != null);
			foreach(GroupField item in groupFields) {
				DataColumnInfo columnInfo = dataController.Columns[item.FieldName];
				ColumnSortOrder sortOrder = (ColumnSortOrder)item.SortOrder;
				if(columnInfo == null)
					continue;
				if(columnInfo.Unbound || !typeof(IComparable).IsAssignableFrom(DevExpress.Data.Helpers.NullableHelpers.GetBoxedType(columnInfo.Type)))
					columnInfo.CustomComparer = ColumnComparer.Default;
				if(!dataColumnSortInfos.Any<DataColumnSortInfo>(columnItem => columnItem.ColumnInfo.Index == columnInfo.Index && columnItem.SortOrder == sortOrder)) {
					dataColumnSortInfos.Add(new DataColumnSortInfo(columnInfo, sortOrder));
					SummarySortInfo info = CreateSummarySortInfo(item, dataColumnSortInfos.Count - 1);
					if(info != null)
						summarySortInfos.Add(info);
				}
			}
		}
		SummarySortInfo CreateSummarySortInfo(GroupField groupField, int level) {
			System.Diagnostics.Debug.Assert(groupField.Band != null);
			if(sortingSummary == null)
				return null;
			foreach(XRGroupSortingSummary summary in sortingSummary) {
				if(summary.Band != groupField.Band)
					continue;
				SummaryItem summaryItem = CreateSummaryItem(summary);
				if(summaryItem == null)
					break;
				levelToSummaryDictionary.Add(level, summary);
				return new SummarySortInfo(summaryItem, level, (ColumnSortOrder)summary.SortOrder);
			}
			return null;
		}
		SummaryItem CreateSummaryItem(XRGroupSortingSummary sortingSummary) {
			SummaryItemType summaryItemType = SummaryItemType.Custom;
			DataColumnInfo dataColumnInfo = dataController.Columns[sortingSummary.FieldName];
			if(dataColumnInfo == null) {
				if(sortingSummary.Function == SortingSummaryFunction.Count)
					summaryItemType = SummaryItemType.Count;
				else
					return null;
			}
			return new SummaryItem(dataColumnInfo, summaryItemType);
		}
		void dataController_CustomSummary(object sender, CustomSummaryEventArgs e) {
			if(sortingSummary == null || levelToSummaryDictionary == null)
				return;
			XRGroupSortingSummary summary;
			levelToSummaryDictionary.TryGetValue(e.GroupLevel, out summary);
			if(summary == null)
				return;
			switch(e.SummaryProcess) {
				case CustomSummaryProcess.Start:
					break;
				case CustomSummaryProcess.Calculate:
					summary.OnDataRowChanged(dataController.GetRow(e.RowHandle), e.FieldValue, e.RowHandle);
					break;
				case CustomSummaryProcess.Finalize:
					e.TotalValue = summary.OnGroupFinished();
					break;
			}
		}
		void DisposeDataController() {
			if(dataController != null) {
				dataController.CustomSummary -= new CustomSummaryEventHandler(dataController_CustomSummary);
				dataController.Dispose();
				dataController = null;
			}
		}
	}
	class ColumnComparer : IComparer {
		public static readonly IComparer Default = new ColumnComparer(CultureInfo.CurrentCulture);
		private CompareInfo m_compareInfo;
		public ColumnComparer(CultureInfo culture) {
			if(culture == null) {
				throw new ArgumentNullException("culture");
			}
			this.m_compareInfo = culture.CompareInfo;
		}
		public int Compare(object a, object b) {
			if(a == b) {
				return 0;
			}
			if(a == null) {
				return -1;
			}
			if(b == null) {
				return 1;
			}
			if(this.m_compareInfo != null) {
				string str = a as string;
				string str2 = b as string;
				if((str != null) && (str2 != null)) {
					return this.m_compareInfo.Compare(str, str2);
				}
			}
			IComparable comparable = a as IComparable;
			if(comparable == null) {
				return 0;
			}
			return comparable.CompareTo(b);
		}
	}
	public class RowIndicesMapper {
		DataController dataController;
		int[] rowIndices;
		public RowIndicesMapper()
			: this(null) {
		}
		public RowIndicesMapper(DataController dataController) {
			this.dataController = dataController;
			BuildRowIndices();
		}
		public int this[int index] {
			get {
				if(dataController == null)
					return DataController.InvalidRow;
				if(dataController.GroupInfo.Count == 0)
					return index;
				if(rowIndices == null || rowIndices.Length == 0 || index >= rowIndices.Length || index < 0)
					return DataController.InvalidRow;
				return rowIndices[index];
			}
		}
		void BuildRowIndices() {
			if(dataController == null || dataController.GroupInfo.Count == 0) {
				rowIndices = null;
				return;
			}
			rowIndices = new int[dataController.VisibleListSourceRowCount];
			int index = 0;
			foreach(GroupRowInfo rowInfo in dataController.GroupInfo) {
				if(rowInfo.Level != dataController.GroupInfo.LevelCount - 1)
					continue;
				if(rowInfo.ChildControllerRowCount == 0)
					continue;
				for(int k = 0; k < rowInfo.ChildControllerRowCount; k++)
					rowIndices[index++] = k + rowInfo.ChildControllerRow;
			}
			System.Diagnostics.Debug.Assert(rowIndices.Length.Equals(index));
		}
	}
	public static class GroupConverter {
		public static GroupField[] GetGroupFields(XRGroupCollection xrGroups) {
			ArrayList groupFields = new ArrayList();
			for(int i = xrGroups.Count - 1; i >= 0; i--) {
				GroupHeaderBand groupHeader = xrGroups[i].Header;
				if(groupHeader != null)
					groupFields.AddRange(GetGroupFields(groupHeader.GroupFields));
			}
			return (GroupField[])groupFields.ToArray(typeof(GroupField));
		}
		public static XRGroupSortingSummary[] GetSortingSummary(XRGroupCollection xrGroups) {
			List<XRGroupSortingSummary> sortingSummaryList = new List<XRGroupSortingSummary>();
			for(int i = xrGroups.Count - 1; i >= 0; i--) {
				GroupHeaderBand groupHeader = xrGroups[i].Header;
				if(groupHeader != null && groupHeader.SortingSummary != null && groupHeader.SortingSummary.Enabled)
					sortingSummaryList.Add(groupHeader.SortingSummary);
			}
			return sortingSummaryList.ToArray();
		}
		static GroupField[] GetGroupFields(GroupFieldCollection groupFieldCollection) {
			ArrayList groupFields = new ArrayList();
			foreach(GroupField groupField in groupFieldCollection) {
				groupFields.Add(groupField);
			}
			return (GroupField[])groupFields.ToArray(typeof(GroupField));
		}
	}
	static class DataControllerExtensions {
		public static IList<GroupRowInfo> GetDataGroups(this DataController dataController, int rowIndex) {
			if(dataController == null)
				return new GroupRowInfo[0];
			List<GroupRowInfo> groupInfos = new List<GroupRowInfo>();
			foreach(GroupRowInfo item in dataController.GroupInfo) {
				if(rowIndex >= item.ChildControllerRow && rowIndex < item.ChildControllerRow + item.ChildControllerRowCount)
					groupInfos.Add(item);
				else if(rowIndex < item.ChildControllerRow)
					break;
			}
			return groupInfos.ToArray();
		}
		public static IList<int> GetDataGroupLevels(this IList<XRGroup> groups) {
			int level = -1;
			int[] levels = new int[groups.Count];
			for(int i = groups.Count - 1; i >= 0; i--) 
				levels[i] = groups[i].GroupFields == null || groups[i].GroupFields.Count == 0 ? level : ++level;
			return levels;
		}
	}
}
