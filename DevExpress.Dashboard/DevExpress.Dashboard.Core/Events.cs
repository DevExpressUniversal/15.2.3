#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon {
	public class MasterFilterSetEventArgs : EventArgs {
		readonly string dashboardItemName;
		readonly DashboardDataSet selectedValues;
		readonly RangeFilterSelection selectedRange;
		public string DashboardItemName { get { return dashboardItemName; } }
		public DashboardDataSet SelectedValues { get { return selectedValues; } }
		public RangeFilterSelection SelectedRange { get { return selectedRange; } }
		public bool IsNullValue(object value) {
			return DashboardSpecialValues.IsNullValue(value);
		}
		public bool IsOthersValue(object value) {
			return DashboardSpecialValues.IsOthersValue(value);
		}
		public MasterFilterSetEventArgs(string dashboardItemName, DashboardDataSet selectedValues, RangeFilterSelection masterFilterRange) {
			this.dashboardItemName = dashboardItemName;
			this.selectedValues = selectedValues;
			this.selectedRange = masterFilterRange;
		}
	}
	public delegate void MasterFilterSetEventHandler(object sender, MasterFilterSetEventArgs e);
	public class MasterFilterClearedEventArgs : EventArgs {
		readonly string dashboardItemName;
		public string DashboardItemName { get { return dashboardItemName; } }
		public MasterFilterClearedEventArgs(string dashboardItemName) {
			this.dashboardItemName = dashboardItemName;
		}
	}
	public delegate void MasterFilterClearedEventHandler(object sender, MasterFilterClearedEventArgs e);
	public class SingleFilterDefaultValueEventArgs : EventArgs {
		readonly string dashboardItemName;
		readonly DashboardDataSet availableSelection;
		DashboardDataSet filterValue;
		List<string> columnNames;
				Dashboard dashboard;
		public string DashboardItemName { get { return dashboardItemName; } }
		public DashboardDataSet FilterValue { get { return filterValue; } }
		public SingleFilterDefaultValueEventArgs(string dashboardItemName, DashboardDataSet availableSelection, DashboardDataRow filterValue) {
			this.dashboardItemName = dashboardItemName;
			this.availableSelection = availableSelection;
			this.columnNames = availableSelection.GetColumnNames();
			SetFilterValue(filterValue);
		}
		public void SetFilterValue(DashboardDataRow value) {
			if(value != null)
				SetFilterValue(value.ToList());
			else
				filterValue = null;
		}
		public void SetFilterValue(IList value) {
			if(value.Count == columnNames.Count) {
				DashboardDataSetInternal internalDataSet = new DashboardDataSetInternal(columnNames);
				internalDataSet.AddRow(value);
				filterValue = new DashboardDataSet(internalDataSet);
			} else {
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectFilterValueLength));
			}
		}
		public DashboardDataSet GetAvailableSelections() {
			return availableSelection;
		}
		internal void SetDashboard(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			if (this.dashboard == null)
				this.dashboard = dashboard;
			else
				throw new InvalidOperationException("The dashboard has already been set");
		}
	}
	public delegate void SingleFilterDefaultValueEventHandler(object sender, SingleFilterDefaultValueEventArgs e);
	public class FilterElementDefaultValuesEventArgs : EventArgs {
		readonly FilterElementDashboardItem filterElement;
		readonly DashboardDataSet availableFilterValues;
		IEnumerable<ISelectionRow> initialValues;
		IList<DashboardDataRow> filterValues;
		[Obsolete("This property is now obsolete. Use the ItemComponentName property instead.")]
		public string FilterElementName { get { return ItemComponentName; } }
		public string ItemName { get { return filterElement.Name; } }
		public string ItemComponentName { get { return filterElement.ComponentName; } }
		public DashboardDataSet AvailableFilterValues { get { return availableFilterValues; } }
		public IList<DashboardDataRow> FilterValues
		{
			get
			{
				if(filterValues == null) {
					filterValues = new List<DashboardDataRow>();
					HashSet<ISelectionRow> set = new HashSet<ISelectionRow>(initialValues);
					foreach(DashboardDataRow dataRow in AvailableFilterValues)
						if(set.Contains(dataRow.ToList().Cast<object>().AsSelectionRow()))
							filterValues.Add(dataRow);
				}
				return filterValues;
			}
		}
		internal bool Changed { get { return filterValues != null; } }
		internal FilterElementDefaultValuesEventArgs(FilterElementDashboardItem filterElement, DashboardDataSet availableFilterValues, IEnumerable<ISelectionRow> initialValues) {
			this.filterElement = filterElement;
			this.availableFilterValues = availableFilterValues;
			this.initialValues = initialValues;
		}
	}
	public delegate void FilterElementDefaultValuesEventHandler(object sender, FilterElementDefaultValuesEventArgs e);
	public class RangeFilterDefaultValueEventArgs : EventArgs {
		public string DashboardItemName { get; private set; }
		public RangeFilterSelection Range { get; set; }
		public RangeFilterDefaultValueEventArgs(string dashboardItemName) {
			DashboardItemName = dashboardItemName;
		}
	}
	public delegate void RangeFilterDefaultValueEventHandler(object sender, RangeFilterDefaultValueEventArgs e);
	public class DrillActionEventArgs : EventArgs {
		readonly string dashboardItemName;
		readonly int drillDownLevel;
		readonly DashboardDataSet values;
		public string DashboardItemName { get { return dashboardItemName; } }
		public int DrillDownLevel { get { return drillDownLevel; } }
		public DashboardDataSet Values { get { return values; } }
		public bool IsNullValue(object value) {
			return DashboardSpecialValues.IsNullValue(value);
		}
		public bool IsOthersValue(object value) {
			return DashboardSpecialValues.IsOthersValue(value);
		}
		public DrillActionEventArgs(string dashboardItemName, int drillDownLevel, DashboardDataSet values) {
			this.dashboardItemName = dashboardItemName;
			this.drillDownLevel = drillDownLevel;
			this.values = values;
		}
	}
	public delegate void DrillActionEventHandler(object sender, DrillActionEventArgs e);
	public class DataSourceEventArgs : EventArgs {
		readonly IDashboardDataSource dataSource;
		public IDashboardDataSource DataSource { get { return dataSource; } }
		public DataSourceEventArgs(IDashboardDataSource dataSource) {
			this.dataSource = dataSource;
		}
	}
	public class DashboardDataLoadingEventArgs : DataSourceEventArgs {
		public object Data { get; set; }
		public DashboardDataLoadingEventArgs(IDashboardDataSource dataSource)
			: base(dataSource) {
		}
	}
	public delegate void DashboardDataLoadingEventHandler(object sender, DashboardDataLoadingEventArgs e);
	public class DashboardCustomFilterExpressionEventArgs : DataSourceEventArgs {
		readonly string tableName;
		public string TableName { get { return tableName; } }
		public CriteriaOperator FilterExpression { get; set; }
		public DashboardCustomFilterExpressionEventArgs(IDashboardDataSource dataSource, string tableName)
			: base(dataSource) {
			this.tableName = tableName;
		}
	}
	public delegate void DashboardCustomFilterExpressionEventHandler(object sender, DashboardCustomFilterExpressionEventArgs e);
	public class CustomParametersEventArgs : EventArgs {
		public List<IParameter> Parameters { get; set; }
		public CustomParametersEventArgs(IEnumerable<IParameter> parameters) {
			Parameters = parameters.ToList<IParameter>();
		}
	}
	public delegate void CustomParametersEventHandler(object sender, CustomParametersEventArgs e);
	public class DataLoadingEventArgs : EventArgs {
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public object Data { get; set; }
		public DataLoadingEventArgs(string dataSourceComponentName, string dataSourceName) {
			DataSourceComponentName = dataSourceComponentName;
			DataSourceName = dataSourceName;
		}
	}
	public delegate void DataLoadingEventHandler(object sender, DataLoadingEventArgs e);
	public class DashboardConfigureDataConnectionEventArgs : ConfigureDataConnectionEventArgs {
		public string DataSourceName{ get; private set; }
		public DashboardConfigureDataConnectionEventArgs(string connectionName, string dataSourceName, DataConnectionParametersBase connectionParameters) :
			base(connectionName, connectionParameters) {
			this.DataSourceName = dataSourceName;
		}
	}
	public delegate void DashboardConfigureDataConnectionEventHandler(object sender, DashboardConfigureDataConnectionEventArgs e);
	public class DashboardConnectionErrorEventArgs : ConnectionErrorEventArgs {
		public string DataSourceName { get; private set; }
		public DashboardConnectionErrorEventArgs(string connectionName, string dataSourceName, DataConnectionParametersBase connectionParameters, Exception exception) :
			base(connectionName, connectionParameters, exception) {
			this.DataSourceName = dataSourceName;
		}
	}
	public delegate void DashboardConnectionErrorEventHandler(object sender, DashboardConnectionErrorEventArgs e);
	public abstract class CustomDrawCellEventArgsBase : EventArgs {
		readonly bool isDarkSkin;
		readonly bool ignoreColorAndBackColor;
		readonly Color defaultBackColor;
		public StyleSettingsInfo StyleSettings { get; protected set; }
		public bool IsDarkSkin { get { return isDarkSkin; } }
		public bool IgnoreColorAndBackColor { get { return ignoreColorAndBackColor; } }
		public Color DefaultBackColor { get { return defaultBackColor; } }
		protected CustomDrawCellEventArgsBase(bool isDarkSkin, bool ignoreColorAndBackColor, Color defaultBackColor) {
			this.isDarkSkin = isDarkSkin;
			this.ignoreColorAndBackColor = ignoreColorAndBackColor;
			this.defaultBackColor = defaultBackColor;
		}
	}
	public delegate void DashboardOptionsChangedEventHandler(object sender, DashboardOptionsChangedEventArgs e);
	public class DashboardOptionsChangedEventArgs : EventArgs {
	}
	public class ValidateDashboardCustomSqlQueryEventArgs : EventArgs {
		readonly ValidateCustomSqlQueryEventArgs innerArgs;
		internal string DashboardId { get; set; }
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public string ConnectionName { get; private set; }
		public DataConnectionParametersBase ConnectionParameters { get; private set; }
		public CustomSqlQuery CustomSqlQuery { get { return innerArgs.CustomSqlQuery; } }
		public string ExceptionMessage { get { return innerArgs.ExceptionMessage; } set { innerArgs.ExceptionMessage = value; } }
		public bool Valid { get { return innerArgs.Valid; } set { innerArgs.Valid = value; } }
		public ValidateDashboardCustomSqlQueryEventArgs(DataConnectionParametersBase connectionParameters, ValidateCustomSqlQueryEventArgs innerArgs)
			: this(innerArgs) {
			Guard.ArgumentNotNull(connectionParameters, "connectionParameters");
			DataSourceComponentName = string.Empty;
			DataSourceName = string.Empty;
			ConnectionName = string.Empty;
			ConnectionParameters = connectionParameters;
		}
		public ValidateDashboardCustomSqlQueryEventArgs(DashboardSqlDataSource dataSource, ValidateCustomSqlQueryEventArgs innerArgs)
			: this(innerArgs) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Guard.ArgumentNotNull(dataSource.Connection, "connection");
			DataSourceComponentName = dataSource.ComponentName;
			DataSourceName = dataSource.Name;
			ConnectionName = dataSource.Connection.Name;
			ConnectionParameters = dataSource.Connection.ConnectionParameters;
		}
		ValidateDashboardCustomSqlQueryEventArgs(ValidateCustomSqlQueryEventArgs innerArgs) {
			Guard.ArgumentNotNull(innerArgs, "innerArgs");
			this.innerArgs = innerArgs;
		}
	}
	public delegate void ValidateDashboardCustomSqlQueryEventHandler(object sender, ValidateDashboardCustomSqlQueryEventArgs e);
}
