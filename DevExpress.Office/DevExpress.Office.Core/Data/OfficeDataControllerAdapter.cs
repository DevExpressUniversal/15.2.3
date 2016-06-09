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
using DevExpress.Utils;
using DevExpress.Data;
using System.Collections;
using System.ComponentModel;
#if !SL && !DXPORTABLE
using System.Data;
#endif
namespace DevExpress.Office {
	public enum MailMergeFieldType {
		Null,
		DbColumn
	}
	public enum MailMergeDestination {
		EMail,
		Fax,
		NewDocument,
		Printer
	}
	public enum MailMergeDataType {
		Database,
		Native,
		Odbc,
		Query,
		Spreadsheet,
		TextFile
	}
	public enum MailMergeSourceType {
		AddressBook,
		Database,
		Document1,
		Document2,
		EMailProgram,
		Legacy,
		Master,
		Native,
		Text
	}
	public abstract class OfficeDataControllerAdapterBase : IDisposable {
		public abstract bool IsReady { get; }
		public abstract int ListSourceRowCount { get; }
		public abstract int CurrentControllerRow { get; set; }
		public abstract int GetColumnIndex(string name);
		public abstract object DataSource { get; set; }
		public abstract string DataMember { get; set; }
		public abstract object GetCurrentRowValue(int columnIndex);
		public abstract object GetCurrentRow();
		#region CurrentRowChanged
		EventHandler onCurrentRowChaned;
		public event EventHandler CurrentRowChanged {
			add { onCurrentRowChaned += value; }
			remove { onCurrentRowChaned -= value; }
		}
		protected virtual void RaiseCurrentRowChangedEvent() {
			if(onCurrentRowChaned != null)
				onCurrentRowChaned(this, EventArgs.Empty);
		}
		#endregion
		#region DataSourceChanged
		EventHandler onDataSourceChanged;
		public event EventHandler DataSourceChanged {
			add { onDataSourceChanged += value; }
			remove { onDataSourceChanged -= value; }
		}
		protected virtual void RaiseDataSourceChanged() {
			if(onDataSourceChanged != null)
				onDataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		#endregion
	}
	public class MailMergeProperties : IDisposable {
		#region Fields
		int activeRecord;
		DataSourceObjectProperties dataSourceObjectProperties;
		string eMailAddressColumnName;
		string connectionString;
		string dataSource;
		MailMergeDataType dataType;
		MailMergeDestination destination;
		bool leaveBlankLines;
		string query;
		bool viewMergedData;
		#endregion
		public MailMergeProperties() {
			this.dataSourceObjectProperties = new DataSourceObjectProperties();
		}
		#region Properties
		public DataSourceObjectProperties DataSourceObjectProperties { get { return dataSourceObjectProperties; } }
		#region ActiveRecord
		public int ActiveRecord {
			get { return activeRecord; }
			set {
				if(activeRecord == value)
					return;
				activeRecord = value;
				RaiseActiveRecordChanged();
			}
		}
		EventHandler activeRecordChanged;
		public event EventHandler ActiveRecordChanged { add { activeRecordChanged += value; } remove { activeRecordChanged -= value; } }
		protected virtual void RaiseActiveRecordChanged() {
			if(activeRecordChanged != null)
				activeRecordChanged(this, EventArgs.Empty);
		}
		#endregion
		#region EMailAddressColumnName
		public string EMailAddressColumnName {
			get { return eMailAddressColumnName; }
			set {
				if(eMailAddressColumnName == value)
					return;
				eMailAddressColumnName = value;
				RaiseEMailAddressColumnNameChanged();
			}
		}
		EventHandler eMailAddressColumnNameChanged;
		public event EventHandler EMailAddressColumnNameChanged { add { eMailAddressColumnNameChanged += value; } remove { eMailAddressColumnNameChanged -= value; } }
		protected virtual void RaiseEMailAddressColumnNameChanged() {
			if(eMailAddressColumnNameChanged != null)
				eMailAddressColumnNameChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ConnectionString
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString == value)
					return;
				connectionString = value;
				RaiseConnectionStringChanged();
			}
		}
		EventHandler connectionStringChanged;
		public event EventHandler ConnectionStringChanged { add { connectionStringChanged += value; } remove { connectionStringChanged -= value; } }
		protected virtual void RaiseConnectionStringChanged() {
			if(connectionStringChanged != null)
				connectionStringChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DataSource
		public string DataSource {
			get { return dataSource; }
			set {
				if(dataSource == value)
					return;
				dataSource = value;
				RaiseDataSourceChanged();
			}
		}
		EventHandler dataSourceChanged;
		public event EventHandler DataSourceChanged { add { dataSourceChanged += value; } remove { dataSourceChanged -= value; } }
		protected virtual void RaiseDataSourceChanged() {
			if(dataSourceChanged != null)
				dataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DataType
		public MailMergeDataType DataType {
			get { return dataType; }
			set {
				if(dataType == value)
					return;
				dataType = value;
				RaiseDataTypeChanged();
			}
		}
		EventHandler dataTypeChanged;
		public event EventHandler DataTypeChanged { add { dataTypeChanged += value; } remove { dataTypeChanged -= value; } }
		protected virtual void RaiseDataTypeChanged() {
			if(dataTypeChanged != null)
				dataTypeChanged(this, EventArgs.Empty);
		}
		#endregion
		#region Destination
		public MailMergeDestination Destination {
			get { return destination; }
			set {
				if(destination == value)
					return;
				destination = value;
				RaiseDestinationChanged();
			}
		}
		EventHandler destinationChanged;
		public event EventHandler DestinationChanged { add { destinationChanged += value; } remove { destinationChanged -= value; } }
		protected virtual void RaiseDestinationChanged() {
			if(destinationChanged != null)
				destinationChanged(this, EventArgs.Empty);
		}
		#endregion
		#region LeaveBlankLines
		public bool LeaveBlankLines {
			get { return leaveBlankLines; }
			set {
				if(leaveBlankLines == value)
					return;
				leaveBlankLines = value;
				RaiseLeaveBlankLinesChanged();
			}
		}
		EventHandler leaveBlankLinesChanged;
		public event EventHandler LeaveBlankLinesChanged { add { leaveBlankLinesChanged += value; } remove { leaveBlankLinesChanged -= value; } }
		protected virtual void RaiseLeaveBlankLinesChanged() {
			if(leaveBlankLinesChanged != null)
				leaveBlankLinesChanged(this, EventArgs.Empty);
		}
		#endregion
		#region Query
		public string Query {
			get { return query; }
			set {
				if(query == value)
					return;
				query = value;
				RaiseQueryChanged();
			}
		}
		EventHandler queryChanged;
		public event EventHandler QueryChanged { add { queryChanged += value; } remove { queryChanged -= value; } }
		protected virtual void RaiseQueryChanged() {
			if(queryChanged != null)
				queryChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ViewMergedData
		public bool ViewMergedData {
			get { return viewMergedData; }
			set {
				if(viewMergedData == value)
					return;
				viewMergedData = value;
				RaiseViewMergedDataChanged();
			}
		}
		EventHandler viewMergedDataChanged;
		public event EventHandler ViewMergedDataChanged { add { viewMergedDataChanged += value; } remove { viewMergedDataChanged -= value; } }
		protected virtual void RaiseViewMergedDataChanged() {
			if(viewMergedDataChanged != null)
				viewMergedDataChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(DataSourceObjectProperties != null) {
					dataSourceObjectProperties.Dispose();
					dataSourceObjectProperties = null;
				}
			}
		}
		#endregion
	}
	public class DataSourceObjectProperties : IDisposable {
		Dictionary<string, FieldMapData> mapColumnName;
		Dictionary<string, FieldMapData> mapFieldMappedName;
		char columnDelimiter;
		bool firstRowHeader;
		NotificationCollection<FieldMapData> fieldsMapData;
		string tableName;
		MailMergeSourceType dataSourceType;
		string udlConnectionString;
		NotificationCollectionChangedListener<FieldMapData> fieldsMapDataListener;
		public DataSourceObjectProperties() {
			this.fieldsMapData = new NotificationCollection<FieldMapData>();
			this.fieldsMapDataListener = new NotificationCollectionChangedListener<FieldMapData>(fieldsMapData);
			this.mapColumnName = new Dictionary<string, FieldMapData>();
			this.mapFieldMappedName = new Dictionary<string, FieldMapData>();
			SubscribeFieldsMapDataEvents();
		}
		#region Properties
		#region ColumnDelimiter
		public char ColumnDelimiter {
			get { return columnDelimiter; }
			set {
				if(columnDelimiter == value)
					return;
				columnDelimiter = value;
				RaiseColumnDelimiterChanged();
			}
		}
		EventHandler columnDelimiterChanged;
		public event EventHandler ColumnDelimiterChanged { add { columnDelimiterChanged += value; } remove { columnDelimiterChanged -= value; } }
		protected virtual void RaiseColumnDelimiterChanged() {
			if(columnDelimiterChanged != null)
				columnDelimiterChanged(this, EventArgs.Empty);
		}
		#endregion
		#region FirstRowHeader
		public bool FirstRowHeader {
			get { return firstRowHeader; }
			set {
				if(firstRowHeader == value)
					return;
				firstRowHeader = value;
				RaiseFirstRowHeaderChanged();
			}
		}
		EventHandler firstRowHeaderChanged;
		public event EventHandler FirstRowHeaderChanged { add { firstRowHeaderChanged += value; } remove { firstRowHeaderChanged -= value; } }
		protected virtual void RaiseFirstRowHeaderChanged() {
			if(firstRowHeaderChanged != null)
				firstRowHeaderChanged(this, EventArgs.Empty);
		}
		#endregion
		#region TableName
		public string TableName {
			get { return tableName; }
			set {
				if(tableName == value)
					return;
				tableName = value;
				RaiseTableNameChanged();
			}
		}
		EventHandler tableNameChanged;
		public event EventHandler TableNameChanged { add { tableNameChanged += value; } remove { tableNameChanged -= value; } }
		protected virtual void RaiseTableNameChanged() {
			if(tableNameChanged != null)
				tableNameChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DataSourceType
		public MailMergeSourceType DataSourceType {
			get { return dataSourceType; }
			set {
				if(dataSourceType == value)
					return;
				dataSourceType = value;
				RaiseDataSourceTypeChanged();
			}
		}
		EventHandler dataSourceTypeChanged;
		public event EventHandler DataSourceTypeChanged { add { dataSourceTypeChanged += value; } remove { dataSourceTypeChanged -= value; } }
		protected virtual void RaiseDataSourceTypeChanged() {
			if(dataSourceTypeChanged != null)
				dataSourceTypeChanged(this, EventArgs.Empty);
		}
		#endregion
		#region UdlConnectionString
		public string UdlConnectionString {
			get { return udlConnectionString; }
			set {
				if(udlConnectionString == value)
					return;
				udlConnectionString = value;
				RaiseUdlConnectionStringChanged();
			}
		}
		EventHandler udlConnectionStringChanged;
		public event EventHandler UdlConnectionStringChanged { add { udlConnectionStringChanged += value; } remove { udlConnectionStringChanged -= value; } }
		protected virtual void RaiseUdlConnectionStringChanged() {
			if(udlConnectionStringChanged != null)
				udlConnectionStringChanged(this, EventArgs.Empty);
		}
		#endregion
		#region FieldsMapData
		public NotificationCollection<FieldMapData> FieldsMapData { get { return fieldsMapData; } }
		#endregion
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(fieldsMapDataListener != null) {
					UnsubscribeFieldsMapDataEvents();
					this.fieldsMapDataListener.Dispose();
				}
				this.fieldsMapDataListener = null;
				this.fieldsMapData = null;
			}
		}
		#endregion
		protected virtual void SubscribeFieldsMapDataEvents() {
			this.fieldsMapDataListener.Changed += new EventHandler(OnFieldsMapDataChanged);
		}
		protected virtual void UnsubscribeFieldsMapDataEvents() {
			this.fieldsMapDataListener.Changed -= new EventHandler(OnFieldsMapDataChanged);
		}
		protected virtual void OnFieldsMapDataChanged(object sender, EventArgs e) {
			int count = FieldsMapData.Count;
			for(int i = 0; i < count; i++) {
				FieldMapData fieldMapData = FieldsMapData[i];
				if(!String.IsNullOrEmpty(fieldMapData.MappedName))
					mapFieldMappedName.Add(fieldMapData.MappedName, fieldMapData);
				if(!String.IsNullOrEmpty(fieldMapData.ColumnName))
					mapColumnName.Add(fieldMapData.ColumnName, fieldMapData);
			}
			RaiseFieldsMapDataChanged();
		}
		EventHandler fieldsMapDataChanged;
		public event EventHandler FieldsMapDataChanged { add { fieldsMapDataChanged += value; } remove { fieldsMapDataChanged -= value; } }
		protected virtual void RaiseFieldsMapDataChanged() {
			if(fieldsMapDataChanged != null)
				fieldsMapDataChanged(this, EventArgs.Empty);
		}
		public virtual FieldMapData FindMapDataByColumnName(string columnName) {
			FieldMapData result;
			if(mapColumnName.TryGetValue(columnName, out result))
				return result;
			else
				return null;
		}
		public virtual FieldMapData FindMapDataByMapName(string mappedFieldName) {
			FieldMapData result;
			if(mapFieldMappedName.TryGetValue(mappedFieldName, out result))
				return result;
			else
				return null;
		}
	}
	public class FieldMapData : ISupportObjectChanged {
		int columnIndex;
		bool dynamicAddress;
		string mappedName;
		string columnName;
		MailMergeFieldType fieldType;
		int mergeFieldNameLanguageId;
		#region ColumnIndex
		public int ColumnIndex {
			get { return columnIndex; }
			set {
				columnIndex = value;
				RaiseChanged();
			}
		}
		#endregion
		#region DynamicAddress
		public bool DynamicAddress {
			get { return dynamicAddress; }
			set {
				dynamicAddress = value;
				RaiseChanged();
			}
		}
		#endregion
		#region MappedName
		public string MappedName {
			get { return mappedName; }
			set {
				mappedName = value;
				RaiseChanged();
			}
		}
		#endregion
		#region ColumnName
		public string ColumnName {
			get { return columnName; }
			set {
				columnName = value;
				RaiseChanged();
			}
		}
		#endregion
		#region FieldType
		public MailMergeFieldType FieldType {
			get { return fieldType; }
			set {
				fieldType = value;
				RaiseChanged();
			}
		}
		#endregion
		#region MergeFieldNameLanguageId
		public int MergeFieldNameLanguageId {
			get { return mergeFieldNameLanguageId; }
			set {
				mergeFieldNameLanguageId = value;
				RaiseChanged();
			}
		}
		#endregion
		EventHandler changed;
		public event EventHandler Changed { add { changed += value; } remove { changed -= value; } }
		protected virtual void RaiseChanged() {
			if(changed != null)
				changed(this, EventArgs.Empty);
		}
	}
	public class OfficeDataController : GridDataController {
		protected override IList GetListSource() {
			if(DataSource == null)
				return null;
#if !SL && !DXPORTABLE
			DataSet dataSet = DataSource as DataSet;
			if(dataSet != null && !String.IsNullOrEmpty(DataMember) && dataSet.Tables != null) {
				int index = dataSet.Tables.IndexOf(DataMember);
				if(index >= 0) {
					IListSource table = dataSet.Tables[index] as IListSource;
					if(table != null)
						return table.GetList();
				}
			}
#endif
			return base.GetListSource();
		}
	}
}
