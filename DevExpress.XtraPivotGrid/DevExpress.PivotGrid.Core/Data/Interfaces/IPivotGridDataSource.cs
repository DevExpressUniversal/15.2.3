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
using System.IO;
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.XtraPivotGrid.Data {
	[Flags]
	public enum PivotDataSourceCaps {
		None = 0,
		UnboundColumns = 1,
		Prefilter = 2,
		RunningTotals = 4,
		DenyExpandValuesAllowed = 8
	};
	public class PivotCellValue {
		public static PivotCellValue Create(object value) {
			if(value == null)
				return null;
			return new PivotCellValue(value);
		}
		public static PivotCellValue Create(object value, string text) {
			if(value == null && string.IsNullOrEmpty(text))
				return null;
			if(string.IsNullOrEmpty(text)) {
				return new PivotCellValue(value);
			} else
				if(text == value as string)
					return new FormattedPivotCellValue(text, text);
				else
					return new FormattedPivotCellValue(value, text);
		}
		public static object GetValue(PivotCellValue value) {
			return value != null ? value.Value : null;
		}
		public static string GetDisplayText(PivotCellValue value) {
			return value != null ? value.DisplayText : null;
		}
		public static decimal GetDecimalValue(PivotCellValue value) {
			return Convert.ToDecimal(GetValue(value));
		}
		public static PivotCellValue Sum(PivotCellValue value1, PivotCellValue value2) {
			object val1 = GetValue(value1);
			object val2 = GetValue(value2);
			if(val1 == null && val2 == null)
				return new PivotCellValue(null);
			return new PivotCellValue(GetDecimalValue(value1) + GetDecimalValue(value2));
		}
		public static readonly PivotCellValue ErrorValue = new PivotCellValue(PivotSummaryValue.ErrorValue);
		public static readonly PivotCellValue Zero = new PivotCellValue(0m);
		public static readonly PivotCellValue Null = new PivotCellValue(null);
		public static readonly PivotCellValue One = new PivotCellValue(1m);
		readonly object value;
		public object Value { get { return value; } }
		public virtual string DisplayText { get { return null; } }
		public PivotCellValue(object value) {
			this.value = value;
		}
	}
	class FormattedPivotCellValue : PivotCellValue {
		readonly string displayText;
		public override string DisplayText { get { return displayText; } }
		public FormattedPivotCellValue(object value, string displayText) : base(value) {
			this.displayText = displayText;
		}
	}
	public class PivotDataSourceEventArgs : EventArgs {
		readonly IPivotGridDataSource pivotDataSource;
		public PivotDataSourceEventArgs(IPivotGridDataSource pivotDataSource) {
			this.pivotDataSource = pivotDataSource;
		}
		public IPivotGridDataSource PivotDataSource { get { return pivotDataSource; } }
	}
	public class PivotDataSourceExpandValueDeniedEventArgs : EventArgs {
		readonly bool isColumn;
		readonly int visibleIndex;
		readonly int level;
		public PivotDataSourceExpandValueDeniedEventArgs(bool isColumn, int visibleIndex, int level) {
			this.isColumn = isColumn;
			this.visibleIndex = visibleIndex;
			this.level = level;
		}
		public bool IsColumn { get { return isColumn; } }
		public int VisibleIndex {get { return visibleIndex; } }
		public int Level { get { return level; } }
	}
	public interface IPivotGridDataSourceOwner {
		IPivotGridDataSource DataSource { get; }
		bool IsDesignMode { get; }
		CancellationToken CancellationToken { get; }
		void CreateField(PivotArea area, string fieldName, string caption, string displayFolder, bool visible);
		PivotGridFieldReadOnlyCollection GetSortedFields();
		PivotGridOptionsOLAP OptionsOLAP { get; }
		CriteriaOperator PrefilterCriteria { get; }
		int? GetCustomFieldSort(IQueryMemberProvider value0, IQueryMemberProvider member1, PivotGridFieldBase field, ICustomSortHelper helper);
		string GetCustomFieldText(PivotGridFieldBase field, object value);
		IList<AggregationLevel> GetAggregations();
	}
	public interface IPivotGridDataSourceAsyncExpand {
		event EventHandler<PivotDataSourceExpandValueDeniedEventArgs> ExpandValueDenied;
	}
	public interface IPivotGridDataSource : IDisposable {
		event EventHandler<PivotDataSourceEventArgs> DataChanged;
		event EventHandler<PivotDataSourceEventArgs> LayoutChanged;
		IPivotGridDataSourceOwner Owner { get; set; }
		PivotDataSourceCaps Capabilities { get; }
		bool AutoExpandGroups { get; set; }
		void SetAutoExpandGroups(bool value, bool reloadData);
		ICustomObjectConverter CustomObjectConverter { get; set; }
		bool ShouldCalculateRunningSummary { get; }
		void RetrieveFields(PivotArea area, bool visible);
		void ReloadData();
		void DoRefresh();
		Type GetFieldType(PivotGridFieldBase field, bool raw);
		bool IsFieldTypeCheckRequired(PivotGridFieldBase field);
		bool ChangeFieldSortOrder(PivotGridFieldBase field);
		bool ChangeFieldSummaryType(PivotGridFieldBase field, PivotSummaryType oldSummaryType);
		PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount);
		PivotCellValue GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType);
		int GetVisibleIndexByValues(bool isColumn, object[] values);
		int GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext);
		bool IsObjectCollapsed(bool isColumn, int visibleIndex);
		bool IsObjectCollapsed(bool isColumn, object[] values);
		bool ChangeExpanded(bool isColumn, int visibleIndex, bool expanded);
		bool ChangeExpandedAll(bool isColumn, bool expanded);
		bool ChangeFieldExpanded(PivotGridFieldBase field, bool expanded);
		bool ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value);
		object GetFieldValue(bool isColumn, int visibleIndex, int areaIndex);
		object[] GetUniqueFieldValues(PivotGridFieldBase field);
		object[] GetSortedUniqueValues(PivotGridFieldBase field);
		object[] GetAvailableFieldValues(PivotGridFieldBase field, bool deferUpdates, ICustomFilterColumnsProvider customFilters);
		List<object> GetVisibleFieldValues(PivotGridFieldBase field);
		List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues);
		bool? IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value);
		bool GetIsEmptyGroupFilter(PivotGridGroup group);
		PivotSummaryInterval GetSummaryInterval(PivotGridFieldBase dataField, bool visibleValuesOnly,
			bool customLevel, PivotGridFieldBase rowField, PivotGridFieldBase columnField);
		bool HasNullValues(PivotGridFieldBase field);
		bool HasNullValues(string dataMember);
		bool GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex);
		int GetCellCount(bool isColumn);
		int GetObjectLevel(bool isColumn, int visibleIndex);
		void SaveCollapsedStateToStream(Stream stream);
		void WebSaveCollapsedStateToStream(Stream stream);
		void SaveDataToStream(Stream stream, bool compressed);
		void LoadCollapsedStateFromStream(Stream stream);
		void WebLoadCollapsedStateFromStream(Stream stream);
		bool IsAreaAllowed(PivotGridFieldBase field, PivotArea area);
		string[] GetFieldList();
		string GetFieldCaption(string fieldName);
		string GetLocalizedFieldCaption(string fieldName);
		bool IsUnboundExpressionValid(PivotGridFieldBase field);
		bool IsFieldReadOnly(PivotGridFieldBase field);
		void OnInitialized();
		void HideDataField(PivotGridFieldBase field, int dataIndex);
		void MoveDataField(PivotGridFieldBase field, int oldIndex, int newIndex);
	}
	public delegate void PivotListDataSourceEventHandler(IPivotListDataSource dataSource);
	public interface IPivotListDataSource : IPivotGridDataSource {
		bool CaseSensitive { get; set; }
		event PivotListDataSourceEventHandler ListSourceChanged;
		IList ListSource { get; }
		void SetListSource(IList value);
		object GetListSourceRowValue(int listSourceRow, string fieldName);
		string GetFieldName(PivotGridFieldBase field);
		PivotSummaryValue GetCellSummaryValue(int columnIndex, int rowIndex, int dataIndex);
		PivotDrillDownDataSource GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows);
		int CompareValues(object val1, object val2);
	}
	public delegate void PivotOLAPDataSourceEventHandler(IPivotOLAPDataSource dataSource);
	public delegate bool PivotQueryDataSourceExceptionEventHandler(IQueryDataSource dataSource, Exception ex);  
	public interface IPivotOLAPDataSource : IQueryDataSource {
		event PivotOLAPDataSourceEventHandler OLAPGroupsChanged;
		event PivotOLAPDataSourceEventHandler OLAPQueryTimeout;
		string FullConnectionString { get; set; }
		string CubeName { get; }
		string CubeCaption { get; }
		bool PopulateColumns();
		void Connect();
		void Disconnect();
		void EnsureConnected();
		List<string> GetKPIList();
		string GetKPIName(string fieldName, PivotKPIType kpiType);
		PivotKPIType GetKPIType(string fieldName);
		PivotKPIGraphic GetKPIGraphic(string fieldName);
		PivotOLAPKPIMeasures GetKPIMeasures(string kpiName);
		PivotOLAPKPIValue GetKPIValue(string kpiName);
		PivotKPIGraphic GetKPIServerDefinedGraphic(string kpiName, PivotKPIType kpiType);
		string GetMeasureServerDefinedFormatString(string fieldName);
		IOLAPMember GetMember(bool isColumn, int visibleIndex);
		IOLAPMember GetMemberByValue(string fieldName, object value);
		IOLAPMember GetMemberByUniqueName(string fieldName, object value);
		IOLAPMember[] GetUniqueMembers(string fieldName);
		Dictionary<string, PivotGrid.OLAP.OLAPDataType> GetProperties(string fieldName);
		string GetDefaultSortProperty(string fieldName);
		string GetDrillDownColumnName(string fieldName);
		string GetHierarchyName(string hierarchyName);
		DefaultBoolean GetColumnIsAggregatable(string dimensionName);
		int GetFieldHierarchyLevel(string fieldName);
		bool GetOlapIsUserHierarchy(string hierarchyName);
		bool Connected { get; }
		int[] GetFieldNotExpandedIndexes(PivotGridFieldBase field);
		int GetVisibleIndexByUniqueValues(bool isColumn, object[] values);
	}
	public interface IQueryDataSource : IPivotGridDataSource {		
		event PivotQueryDataSourceExceptionEventHandler QueryException;
		int GetLevelCount(bool isColumn);
		PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns);
		string GetColumnDisplayFolder(string columnName);
		string GetColumnCaption(string columnName);
		string SaveColumns();
		string SaveFieldValuesAndCellsToString();
		void RestoreColumns(string savedColumns);
		void RestoreFieldValuesAndCellsFromString(string stateString);
		void LoadCollapsedState(bool isColumn, CollapsedState state);
	}
}
