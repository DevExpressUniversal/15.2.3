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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.XtraGrid;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Xpo;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Access;
namespace DevExpress.Snap.Core.Native.Data {
	public class SnapDataContext : XRDataContextBase {
		readonly IEnumerable<ICalculatedField> fCalculatedFields;
		readonly ParameterCollection parameterCollection;
		readonly IDataSourceDisplayNameProvider dataSourceNameProvider;
		GroupFieldInfo[] groupFields;
		CriteriaOperator filterExpression;		
		bool initialized;
		string[] nestedFields;
		public SnapDataContext() { 
		}
		public SnapDataContext(IEnumerable<ICalculatedField> calculatedFields, ParameterCollection parameterCollection, IDataSourceDisplayNameProvider dataSourceNameProvider, string[] nestedFields) : base(null) {
			this.nestedFields = nestedFields;
			this.fCalculatedFields = calculatedFields;
			this.parameterCollection = parameterCollection;
			this.dataSourceNameProvider = dataSourceNameProvider;
			ApplyCalculatedFields(calculatedFields);
		}
		public SnapDataContext(IEnumerable<ICalculatedField> calculatedFields, ParameterCollection parameterCollection, IDataSourceDisplayNameProvider dataSourceNameProvider)
			: this(calculatedFields, parameterCollection, dataSourceNameProvider, null) {
		}
		public SnapDataContext(IDataSourceDisplayNameProvider dataSourceNameProvider)
			: this(null, null, dataSourceNameProvider, null) { }
		protected override ListControllerBase CreateListCotroller() {
			return new SnapListController(nestedFields);
		}
		public void Initialize(DataBrowser dataBrowser, GroupFieldInfo[] groupFields, CriteriaOperator filterExpression) {			
			ListBrowser listBrowser = dataBrowser as ListBrowser;
			if (listBrowser == null)
				return;
			if (!ShouldInitialize(groupFields, filterExpression))
				return;
			((SnapListController)listBrowser.ListController).Initialize(fCalculatedFields, parameterCollection, groupFields, filterExpression, dataBrowser);
			this.groupFields = groupFields;
			this.filterExpression = filterExpression;
			initialized = true;
		}
		protected virtual bool ShouldInitialize(GroupFieldInfo[] newGroupFields, CriteriaOperator newFilterExpression) {
			if (!initialized)
				return true;
			if (!Object.Equals(newFilterExpression, filterExpression))
				return true;
			if ((groupFields == null && newGroupFields != null) || (groupFields != null && newGroupFields == null))
				return true;
			if (!Object.ReferenceEquals(groupFields, null)) {
				if (newGroupFields.Length != groupFields.Length)
					return true;
				for (int i = 0; i < groupFields.Length; i++) {
					if (!groupFields[i].Equals(newGroupFields[i]))
						return true;
				}
			}
			return false;				
		}
		protected override CalculatedPropertyDescriptorBase CreateCalculatedPropertyDescriptor(ICalculatedField calculatedField) {
			return new CalculatedPropertyDescriptor(calculatedField, parameterCollection, this);
		}
		public override string GetDataSourceDisplayName(object dataSource, string dataMember) {
			string dataSourceName = dataSourceNameProvider != null ? dataSourceNameProvider.GetDataSourceName(dataSource) : null;
			if (!string.IsNullOrEmpty(dataSourceName))
				return dataSourceName;
			return base.GetDataSourceDisplayName(dataSource, dataMember);
		}
	}
	public class CalculatedPropertyDescriptor : CalculatedPropertyDescriptorBase {
		public CalculatedPropertyDescriptor(ICalculatedField calculatedField, IEnumerable<IParameter> parameter, DataContext dataContext)
			: base(calculatedField, parameter, dataContext) {
		}
	}
	public class SnapPropertyAggregator : PropertyAggregator {
		protected override PropertyDescriptor GetAggregatedProperty(PropertyDescriptor property, List<PropertyDescriptor> list, object dataSource, string dataMember) {
			return new AggregatedPropertyDescriptor(property, property.PropertyType, GetName(property));
		}
	}
	public class SnapListSourceDataController : ListSourceDataController {
		class SnapDataControllerHelper : ListDataControllerHelper {
			public SnapDataControllerHelper(DataControllerBase controller)
				: base(controller) {
			}
			protected override PropertyDescriptorCollection GetPropertyDescriptorCollection() {
				XRDataContextBase dataContext = new XRDataContextBase();
				PropertyDescriptorCollection properties = dataContext[Controller.ListSource].GetItemProperties();
				IDataControllerData2 dc2 = Controller.DataClient as IDataControllerData2;
				if (dc2 != null)
					properties = dc2.PatchPropertyDescriptorCollection(properties);
				if (dc2 == null || dc2.CanUseFastProperties)
					properties = DataListDescriptor.GetFastProperties(properties);
				PropertyDescriptor[] props = new SnapPropertyAggregator().Aggregate(properties, Controller.ListSource, string.Empty);
				return new PropertyDescriptorCollection(props);
			}
		}
		protected override BaseDataControllerHelper CreateHelper() {
			return new SnapDataControllerHelper(this);
		}
	}
	public class SnapListController : ListControllerBase, IUniqueFilteredValuesAccessor, IDisposable {
		#region inner classes
		class SnapDataClient : IDataControllerData2 {
			readonly SnapListSourceDataController dataController;
			readonly object dataSource;
			readonly string[] nestedFields;
			public SnapDataClient(SnapListSourceDataController dataController, object dataSource, string[] nestedFields) {
				this.dataController = dataController;
				this.dataSource = dataSource;
				this.nestedFields = nestedFields;
			}
			public bool CanUseFastProperties {
				get { return true; }
			}
			public ComplexColumnInfoCollection GetComplexColumns() {
				List<string> realNestedFields = new List<string>();
				foreach (string fieldName in this.nestedFields) {
					if (dataController.Columns[fieldName] == null)
						realNestedFields.Add(fieldName);
				}
				ComplexColumnInfoCollection result = new ComplexColumnInfoCollection();
				realNestedFields.ForEach(s => result.Add(s));
				return result;
			}
			void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
			public bool HasUserFilter {
				get { return true; }
			}
			public bool? IsRowFit(int listSourceRow, bool fit) {
				return null;
			}
			public PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
				return collection;
			}
			public UnboundColumnInfoCollection GetUnboundColumns() {
				return null;
			}
			public object GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
				return null;
			}
			public void SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			}
		}
		#endregion
		#region static
		static IList GetDataSourceSource(object value) {
			IList list = value as IList;
			if (list != null)
				return list;
			IListSource listSource = value as IListSource;
			if (listSource != null)
				return listSource.GetList();
			if (value is IEnumerable)
				return new ListIEnumerable(value as IEnumerable);
			return null;
		}
		#endregion
		SnapListSourceDataController sourceDataController;
		GroupFieldInfo[] groupFields;
		readonly List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
		CriteriaOperator filterExpression;
		ParameterCollection parameters;
		string[] nestedFields;
		public SnapListController() { 
		}
		public SnapListController(string[] nestedFields) {
			this.nestedFields = nestedFields;
		}
		public override int Count {
			get {
				return sourceDataController != null ? sourceDataController.VisibleCount : 0;
			}
		}
		public bool AllowSort(string fieldName) {
			return sourceDataController != null && sourceDataController.Columns[fieldName] != null && sourceDataController.Columns[fieldName].AllowSort;
		}
		public Type GetColumnType(string fieldName) {
			return sourceDataController != null && sourceDataController.Columns[fieldName] != null ?
				sourceDataController.Columns[fieldName].Type : null;
		}
		public DataColumnInfoCollection Columns { get { return this.sourceDataController.Columns; } }
		public SummaryItemCollection TotalSummary {
			get { return this.sourceDataController.TotalSummary; }
		}
		public SummaryItemCollection GroupSummary {
			get { return this.sourceDataController.GroupSummary; }
		}
		public void UpdateTotalSummary() {
			this.sourceDataController.UpdateTotalSummary();
		}
		public void UpdateGroupSummary() {
			this.sourceDataController.UpdateGroupSummary();
		}
		public bool SummariesIgnoreNullValues {
			get { return this.sourceDataController.SummariesIgnoreNullValues; }
			set { this.sourceDataController.SummariesIgnoreNullValues = value; }
		}
		public object GetGroupSummaryValue(int controllerRow, SummaryItem summaryItem, int groupLevel) {
			GroupRowInfo groupRowInfo = sourceDataController.GetParentGroupRow(controllerRow);
			while (groupRowInfo != null && groupRowInfo.Level != groupLevel) {
				groupRowInfo = groupRowInfo.ParentGroup;
			}
			if (groupRowInfo != null) {
				Hashtable groupSummary = sourceDataController.GetGroupSummary(groupRowInfo.Handle);
				return groupSummary[summaryItem];
			}
			return null;
		}
		public override object GetColumnValue(int controllerRow, string columnName) {
			return sourceDataController.GetRowValue(controllerRow, columnName);
		}
		public override object GetItem(int controllerRow) {
			return sourceDataController.GetRow(controllerRow);
		}
		public void Initialize(IEnumerable<ICalculatedField> calculatedFields, ParameterCollection parameters, GroupFieldInfo[] groupFields, CriteriaOperator filterExpression, DataBrowser dataBrowser) {
			if (this.sourceDataController != null) {
				this.filterExpression = filterExpression;
				this.groupFields = groupFields;
				this.parameters = parameters;
				InitializeProperties(calculatedFields, dataBrowser);
				ApplyProperties();
				GroupData();
				sourceDataController.FilterCriteria = filterExpression;
			}
		}
		protected
#if DXCommon
		internal
#endif
		override void SetList(IList list) {
			if (this.list == list)
				return;
			SetListCore(list);
		}
		void SetListCore(IList list) {
			DisposeDataController();
			if (list != null) {
				sourceDataController = new SnapListSourceDataController() { AllowIEnumerableDetails = true };
				if (nestedFields != null)
					sourceDataController.DataClient = new SnapDataClient(sourceDataController, list, nestedFields);
				sourceDataController.ListSource = list;
			}
			this.list = list;
		}
		void ApplyProperties() {
			if (properties != null) {
				foreach (PropertyDescriptor property in properties) {
					sourceDataController.Columns.Add(property);
					sourceDataController.Helper.DescriptorCollection.Add(property);
				}
			}
		}
		void DisposeDataController() {
			if (sourceDataController != null) {
				sourceDataController.Dispose();
				sourceDataController = null;
			}
		}
		public void UpdateCore(DataBrowser dataBrowser) {			
			SetListCore(list);
			if (sourceDataController != null) {
				GroupData();
				sourceDataController.FilterCriteria = filterExpression;
			}
			dataBrowser.RaiseCurrentChanged();
		}
		void GroupData() {
			DataColumnSortInfo[] dataColumnSortInfo = GetDataColumnSortInfo(groupFields);
			SummarySortInfo[] summarySortInfo = new SummarySortInfo[0];
			System.Diagnostics.Debug.Assert(dataColumnSortInfo != null);
			if (sourceDataController != null) {
				sourceDataController.GroupSummary.Clear();
				sourceDataController.UpdateSortGroup(dataColumnSortInfo, dataColumnSortInfo.Length, summarySortInfo);
			}
		}
		public DataColumnSortInfo[] GetDataColumnSortInfo(GroupFieldInfo[] groupFields) {			
			List<DataColumnSortInfo> result = new List<DataColumnSortInfo>();
			if (groupFields == null)
				return result.ToArray();
			foreach(GroupFieldInfo groupField in groupFields) {
				if (string.IsNullOrEmpty(groupField.FieldName))
					continue;
				DataColumnInfo columnInfo = sourceDataController.Columns[groupField.FieldName];
				if (columnInfo == null)
					continue;				
				DataColumnSortInfo sortInfo = new DataColumnSortInfo(columnInfo, groupField.SortOrder, ToColumnGroupInterval(groupField.GroupInterval));
				result.Add(sortInfo);				
			}			
			return result.ToArray();
		}
		static ColumnGroupInterval ToColumnGroupInterval(GroupInterval groupInterval) {
			if (groupInterval == GroupInterval.Day)
				return ColumnGroupInterval.Date;
			if (groupInterval == GroupInterval.Month)
				return ColumnGroupInterval.DateMonth;
			if (groupInterval == GroupInterval.Year)
				return ColumnGroupInterval.DateYear;
			if (groupInterval == GroupInterval.Smart)
				return ColumnGroupInterval.DateRange;
			return ColumnGroupInterval.Default;
		}
		public void InitializeProperties(IEnumerable<ICalculatedField> calculatedFields, DataBrowser dataBrowser) {
			System.Diagnostics.Debug.Assert(sourceDataController != null);
			properties.Clear();
			if (calculatedFields == null)
				return;
			foreach (CalculatedField item in calculatedFields) {
				if (sourceDataController.Columns[item.Name] == null) {
					PropertyDescriptor property = dataBrowser.FindItemProperty(item.Name, true);
					if (property is CalculatedPropertyDescriptorBase)
						properties.Add(property);
				}
			}		   
		}
		void InitializeCalculatedFields(IEnumerable<ICalculatedField> calculatedFields) {
			properties.Clear();
			if (calculatedFields == null || calculatedFields.Count() == 0)
				return;
			using (SnapDataContext dataContext = new SnapDataContext(calculatedFields, parameters, null)) {
				foreach (CalculatedField item in calculatedFields) {
					DataBrowser dataBrowser = dataContext.GetDataBrowser(item.DataSource, item.DataMember, true);
					if (dataBrowser == null)
						continue;
					if (sourceDataController.Columns[item.Name] == null) {
						PropertyDescriptor property = dataBrowser.FindItemProperty(item.Name, true);
						if (property is CalculatedPropertyDescriptorBase)
							properties.Add(property);
					}
				}
			}
		}
		public IEnumerable<object> GetUniqueFilteredValues(string fieldName) {
			return sourceDataController.GetUniqueColumnValues(fieldName, -1, true, false, null);
		}
		#region IUniqueFilteredValuesAccessor Members
		public IEnumerable<object> GetUniqueFilteredValues(string fieldName, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed) {
			return sourceDataController.GetUniqueColumnValues(fieldName, maxCount, includeFilteredOut, roundDataTime, completed);
		}
		#endregion
		protected internal virtual SnapListController CreateDetailController(string fullPath, ListParameters listParameters, int controllerRow, IEnumerable<ICalculatedField> calculatedFields, string[] nestedFields) {
			int relationIndex = GetRelationIndex(fullPath);
			if (relationIndex < 0)
				return null;
			IList detailList = sourceDataController.GetDetailList(controllerRow, relationIndex);
			if (detailList == null)
				return null;
			SnapListController result = new SnapListController();
			result.Update(detailList, listParameters, calculatedFields, parameters, nestedFields);
			return result;
		}
		int GetRelationIndex(string fullPath) {
			DataColumnInfo  detailsColumn = sourceDataController.DetailColumns[fullPath];
			if (detailsColumn != null)
				return detailsColumn.Index;			
			DataColumnInfo columnInfo = sourceDataController.Columns[fullPath];
			if (columnInfo == null) {
				columnInfo = RegisterComplexColumn(fullPath);
				if (columnInfo == null)
					return -1;
			}
			sourceDataController.DetailColumns.Add(columnInfo);
			return columnInfo.Index;
		}
		DataColumnInfo RegisterComplexColumn(string fullPath) {
			if (fullPath.IndexOf('.') < 0)
				return null;
			return sourceDataController.Columns.Add(new SnapComplexPropertyDescriptor(sourceDataController, fullPath));
		}
		public class SnapComplexPropertyDescriptor : ComplexPropertyDescriptorReflection {
			public SnapComplexPropertyDescriptor(DataControllerBase controller, string path)
				: base(controller, path) {
			}
			protected override PropertyDescriptor GetDescriptor(string name, object obj, Type type) {
				if(ExpandoPropertyDescriptor.IsDynamicType(type)) {
					return ExpandoPropertyDescriptor.GetProperty(name, obj, type);
				}
				if(obj == null || obj == DBNull.Value) {
					PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(type);
					if(coll == null)
						return null;
					return coll[name];
				}
				else {
					PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
					if(coll == null)
						return null;
					return coll[name];
				}
			}
		}
		internal bool Update(object source, ListParameters listParameters, IEnumerable<ICalculatedField> calculatedFields, ParameterCollection parameters, string[] nestedFields) {
			this.parameters = parameters;
			if (listParameters != null) {
				this.groupFields = listParameters.GetGroupFieldInfos();
				filterExpression = CriteriaOperator.Parse(GetFullFilterString(listParameters.Filters));
				if(parameters != null)
					ParametersValueSetter.Process(filterExpression, parameters);				
			}
			else
				this.groupFields = null;
			this.nestedFields = GetAllNestedFields(nestedFields);
			SetList(GetDataSourceSource(source));
			if (sourceDataController == null)
				return false;
			InitializeCalculatedFields(calculatedFields);
			ApplyProperties();
			GroupData();
			sourceDataController.FilterCriteria = filterExpression;
			sourceDataController.ExpandAll();
			return true;
		}
		string[] GetAllNestedFields(string[] nestedFields) {
			if (groupFields != null && nestedFields != null) {
				List<string> resultedNestedFields = new List<string>(nestedFields);
				foreach (GroupFieldInfo groupFieldInfo in groupFields) {
					if (!resultedNestedFields.Contains(groupFieldInfo.FieldName, StringComparer.OrdinalIgnoreCase))
						resultedNestedFields.Add(groupFieldInfo.FieldName);
				}
				return resultedNestedFields.ToArray();
			}
			return nestedFields;
		}
		string GetFullFilterString(FilterProperties filterProperties) {
			if (filterProperties == null || filterProperties.IsEmpty)
				return String.Empty;			
			if (filterProperties.Filters.Count == 1)
				return filterProperties.Filters[0];
			StringBuilder sb = new StringBuilder();
			foreach (string filter in filterProperties.Filters) {
				if (sb.Length > 0)
					sb.Append("&&");
				sb.Append("(");
				sb.Append(filter);
				sb.Append(")");
			}
			return sb.ToString();
		}
		internal int GetVisibleIndex(int rowHandle) {
			return sourceDataController.GetVisibleIndex(rowHandle);
		}
		internal int GetControllerRowHandle(int visibleIndex) {
			return sourceDataController.GetControllerRowHandle(visibleIndex);
		}
		internal GroupRowInfo GetParentGroupRow(int controllerRow) {
			return sourceDataController.GetParentGroupRow(controllerRow);
		}
		public object GetGroupSummaryValue(int controllerRow, string columnName, Fields.SummaryRunning summaryRunning, SummaryItemType summaryFunc, bool ignoreNullValues, int groupLevel) {
			DataColumnInfo dataColumnInfo = sourceDataController.Columns[columnName];
			if (dataColumnInfo == null)
				return null;
			SummaryItemCollection summaryItemCollection = summaryRunning == Fields.SummaryRunning.Report ? TotalSummary : GroupSummary;
			SummaryItem summaryItem = new SummaryItem(dataColumnInfo, summaryFunc);			
			if (!summaryItemCollection.Contains(summaryItem)) {
				summaryItemCollection.Add(summaryItem);
			}
			if (summaryRunning == Fields.SummaryRunning.Group) {
				UpdateGroupSummary();
				return GetGroupSummaryValue(controllerRow, summaryItem, groupLevel);
			}
			else
				return summaryItem.SummaryValue;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SnapListController() {
			Dispose(false);
		}
		void Dispose(bool disposing) {
			if (disposing) {
				if (this.sourceDataController != null) {
					this.sourceDataController.Dispose();
					this.sourceDataController = null;
				}
			}
		}
		#endregion
	}
}
