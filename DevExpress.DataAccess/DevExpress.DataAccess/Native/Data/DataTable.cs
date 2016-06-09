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
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {	
	public class DataTable : Dictionary<string, DataColumn> {
		internal const string XmlTable = "Table";
		internal const string XmlName = "Name";
		internal const string XmlUniqueName = "UniqueName";
		internal const string XmlAlias = "Alias";
		internal const string XmlColumns = "Columns";
		public static Type GetColumnType(DBColumn column) {
			return DBColumn.GetType(column.ColumnType);
		}
		readonly DataSelection selection;
		readonly string uniqueName;
		string alias;
		DBTable table;
		string tableName;
		DataReferences references;
		public DataSelection Selection {
			get {
				return this.selection;
			}
		}
		public bool HasSelectedColumns {
			get {
				foreach(DataColumn column in Columns)
					if(column.Selected)
						return true;
				return false;
			}
		}
		public IEnumerable<DataColumn> SelectedColumns {
			get {
				foreach(DataColumn column in Columns)
					if(column.Selected)
						yield return column;
			}
		}
		public IEnumerable<DataColumn> Columns {
			get {
				return this.Select(p => p.Value);
			}
		}
		public DBTable Table {
			get {
				if(this.table == null)
					this.table = this.selection.DataProvider.FindTable(this.tableName);
				return this.table;
			}
			private set {
				this.table = value;
				this.tableName = value.Name;
			}
		}
		public string TableName {
			get {
				return this.table != null ? this.table.Name : this.tableName;
			}
		}
		public DataReferences References {
			get {
				return this.references;
			}
			set {
				this.references = value;
				this.selection.OnTableChanged();
			}
		}
		public string UniqueName {
			get {
				return this.uniqueName;
			}
		}
		public string Alias {
			get {
				return this.alias;
			}
			set {
				SetAlias(value, true);
			}
		}
		public DataTable(DataSelection selection, DBTable table, string uniqueName, DataReferences references)
			: this(selection, table, uniqueName, (IEnumerable<DataReference>)references) {
			if(references != null) {
				this.references.ActionType = references.ActionType;
			}
		}
		public DataTable(DataSelection selection, DBTable table, string uniqueName, IEnumerable<DataReference> references) {
			Guard.ArgumentNotNull(selection, "selection");
			Guard.ArgumentNotNull(table, "table");
			this.selection = selection;
			this.Table = table;
			this.uniqueName = uniqueName;
			this.alias = uniqueName;
			FillColumns(selection, table);
			this.references = new DataReferences(selection, table, uniqueName);
			if(references != null) {
				this.references.AddRange(references);
			}
		}
		public DataTable(DataSelection selection, DBTable table, string uniqueName)
			: this(selection, table, uniqueName, null) {
		}
		public DataTable(DataSelection selection, XElement element) {
			this.selection = selection;
			this.tableName = XmlHelperBase.GetAttributeValue(element, DataTable.XmlName);
			this.uniqueName = XmlHelperBase.GetAttributeValue(element, DataTable.XmlUniqueName) ?? this.tableName;
			this.alias = XmlHelperBase.GetAttributeValue(element, DataTable.XmlAlias) ?? this.uniqueName;
			XElement referencesElement = element.Element(DataReferences.XmlReferences);
			if(referencesElement != null)
				this.references = new DataReferences(selection, this.tableName, this.alias, referencesElement);
			else
				this.references = new DataReferences(selection, this.tableName, this.alias);
			XElement columnsElement = element.Element(DataTable.XmlColumns);
			if(columnsElement != null)
				foreach(XElement columnElement in columnsElement.Elements(DataColumn.XmlColumn)) {
					string columnName = XmlHelperBase.GetAttributeValue(columnElement, DataColumn.XmlName);
					Add(columnName, new DataColumn(selection, this, columnElement));
				}
		}
		void FillColumns(DataSelection selection, DBTable table) {
			foreach(DBColumn column in table.Columns) {
				string foreignKeyTableName = "";
				foreach(DBForeignKey foreignKey in table.ForeignKeys)
					if(foreignKey.Columns.Contains(column.Name)) {
						foreignKeyTableName = foreignKey.PrimaryKeyTable;
						break;
					}
				DBTable foreignKeyTable = null;
				if(!string.IsNullOrEmpty(foreignKeyTableName))
					foreignKeyTable = selection.DataProvider.FindTable(foreignKeyTableName);
				Add(column.Name, new DataColumn(selection, this, column, String.Empty, foreignKeyTable));
			}
		}
		public DataObjectCompareResult CompareWith(DataTable dataTable) {
			if(!Table.Equals(dataTable.Table))
				return DataObjectCompareResult.NotEqual;
			DataObjectCompareResult totalResult = Alias != dataTable.Alias ? DataObjectCompareResult.EqualExceptForAliases : DataObjectCompareResult.Equal;
			IEnumerator<DataColumn> columns1 = SelectedColumns.GetEnumerator();
			IEnumerator<DataColumn> columns2 = dataTable.SelectedColumns.GetEnumerator();
			bool nextStep1;
			bool nextStep2;
			do {
				nextStep1 = columns1.MoveNext();
				nextStep2 = columns2.MoveNext();
				if(nextStep1 != nextStep2)
					return DataObjectCompareResult.NotEqual;
				if(nextStep1) {
					DataObjectCompareResult result = columns1.Current.CompareWith(columns2.Current);
					if(result == DataObjectCompareResult.NotEqual)
						return DataObjectCompareResult.NotEqual;
					if(result == DataObjectCompareResult.EqualExceptForAliases)
						totalResult = DataObjectCompareResult.EqualExceptForAliases;
				}
			} while(nextStep1);
			if(this.references.ActionType != dataTable.references.ActionType)
				return DataObjectCompareResult.NotEqual;
			IEnumerator<DataReference> references1 = References.GetEnumerator();
			IEnumerator<DataReference> references2 = dataTable.References.GetEnumerator();
			do {
				nextStep1 = references1.MoveNext();
				nextStep2 = references2.MoveNext();
				if(nextStep1 != nextStep2)
					return DataObjectCompareResult.NotEqual;
				if(nextStep1) {
					if(!references1.Current.CompareWith(references2.Current))
						return DataObjectCompareResult.NotEqual;
				}
			} while(nextStep1);
			return totalResult;
		}
		public void FillChangeList(DataTable dataTable, IList<AliasChangeInfo> changeList) {
			if(!Table.Equals(dataTable.Table))
				return;
			IEnumerator<DataColumn> columns1 = SelectedColumns.GetEnumerator();
			IEnumerator<DataColumn> columns2 = dataTable.SelectedColumns.GetEnumerator();
			bool nextStep1;
			bool nextStep2;
			do {
				nextStep1 = columns1.MoveNext();
				nextStep2 = columns2.MoveNext();
				if(nextStep1 && nextStep2 && nextStep1 == nextStep2) {
					DataObjectCompareResult result = columns1.Current.CompareWith(columns2.Current);
					if(result == DataObjectCompareResult.EqualExceptForAliases)
						changeList.Add(new AliasChangeInfo(columns1.Current.ActualName, columns2.Current.ActualName));
				}
			} while(nextStep1);
		}
		public void PrepareSelectStatement(SelectStatement selectStatement) {
			this.references.PrepareSelect(selectStatement);
		}
		public void OnChanged() {
			this.selection.OnTableChanged();
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlTable);
			element.Add(new XAttribute(XmlName, this.tableName));
			if(this.uniqueName != this.tableName)
				element.Add(new XAttribute(XmlUniqueName, this.uniqueName));
			if(this.alias != this.uniqueName)
				element.Add(new XAttribute(XmlAlias, this.alias));
			if(this.references.Count > 0)
				element.Add(this.references.SaveToXml());
			List<DataColumn> selectedColumns = new List<DataColumn>(SelectedColumns);
			if(selectedColumns.Count > 0) {
				XElement columnsElement = new XElement(XmlColumns);
				foreach(DataColumn column in selectedColumns)
					columnsElement.Add(column.SaveToXml());
				element.Add(columnsElement);
			}
			return element;
		}
		public SchemaLoadingExceptionsInfo LoadDataObjects() {
			SchemaLoadingExceptionsInfo exceptions = new SchemaLoadingExceptionsInfo();
			this.table = this.selection.DataProvider.FindTable(this.tableName);
			foreach(DataColumn column in this.Columns) {
				SchemaLoadingExceptionInfo columnInfo = column.LoadDataObjects();
				if(columnInfo != null)
					exceptions.Add(columnInfo);
			}
			this.references.LoadDataObjects();
			return exceptions;
		}
		public void SetAllColumnsSelection(bool selected) {
			bool changed = false;
			foreach(DataColumn dataColumn in this.Columns)
				if(dataColumn.Selected != selected) {
					dataColumn.SetSelected(selected, false);
					changed = true;
				}
			if(changed)
				OnChanged();
		}
		public override string ToString() {
			return this.alias;
		}
		public void SetAlias(string newAlias, bool raiseChanged) {
			if(this.alias != newAlias) {
				if(this.selection.IsTableAliasExists(newAlias))
					throw new ArgumentException();
				FilterInfo filter = this.selection.Filters[this.alias];
				if(filter != null) {
					filter.TableName = newAlias;
					filter.FilterString = CriteriaOperator.ToString(AliasesCriteriaPatcher.Process(CriteriaOperator.Parse(filter.FilterString), Alias, newAlias));
				}
				this.alias = newAlias;
				this.references.Alias = this.alias;
				if(raiseChanged)
					this.selection.OnTableAliasChanged(this);
			}
		}
	}
}
