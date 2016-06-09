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
using System.Xml;
using System.Xml.Linq;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {
	public class DataColumn {
		internal const string XmlColumn = "Column";
		internal const string XmlName = "Name";
		internal const string XmlAlias = "Alias";
		readonly DataSelection selection;
		readonly DataTable dataTable;
		readonly DBTable foreignKeyTable;
		string alias;
		DBColumn column;
		string columnName;
		bool selected;
		bool HasAlias { get { return !String.IsNullOrEmpty(this.alias); } }
		public bool IsForeignKey { get { return this.foreignKeyTable != null; } }
		public DBTable ForeignKeyTable { get { return this.foreignKeyTable; } }
		public DBColumn Column
		{
			get
			{
				if(this.column == null)
					this.column = this.selection.DataProvider.FindColumn(this.dataTable.TableName, this.columnName);
				return this.column;
			}
			private set
			{
				this.column = value;
				this.columnName = value.Name;
			}
		}
		public string ColumnName { get { return this.column != null ? this.column.Name : this.columnName; } }
		public DataTable DataTable { get { return this.dataTable; } }
		public DataSelection DataSelection { get { return this.selection; } }
		public bool Selected { get { return this.selected; } set { SetSelected(value, true); } }
		public string Alias { get { return this.alias; } set { SetAlias(value, true); } }
		public string ActualName { get { return HasAlias ? this.alias : this.columnName; } }
		public DataColumn(DataSelection selection, DataTable dataTable, DBColumn column, string alias, DBTable foreignKeyTable) {
			Guard.ArgumentNotNull(selection, "selection");
			this.selection = selection;
			this.Column = column;
			this.alias = alias;
			this.dataTable = dataTable;
			this.foreignKeyTable = foreignKeyTable;
		}
		public DataColumn(DataSelection selection, DataTable dataTable, string columnName, string alias, DBTable foreignKeyTable) {
			Guard.ArgumentNotNull(selection, "selection");
			this.selection = selection;
			this.dataTable = dataTable;
			this.columnName = columnName;
			this.alias = alias;
			this.foreignKeyTable = foreignKeyTable;
		}
		public DataColumn(DataSelection selection, DataTable dataTable, XElement columnElement) {
			Guard.ArgumentNotNull(selection, "selection");
			this.selection = selection;
			this.dataTable = dataTable;
			string columnName = XmlHelperBase.GetAttributeValue(columnElement, XmlName);
			string columnAlias = XmlHelperBase.GetAttributeValue(columnElement, XmlAlias) ?? String.Empty;
			if(String.IsNullOrEmpty(columnName))
				throw new XmlException();
			this.columnName = columnName;
			SetAlias(columnAlias, false);
			SetSelected(true, false);
		}
		public SchemaLoadingExceptionInfo LoadDataObjects() {
			if(Column == null)
				return new SchemaLoadingExceptionInfo(this.dataTable.TableName, this.columnName);
			return null;
		}
		public DataObjectCompareResult CompareWith(DataColumn dataColumn) {
			if(Column != dataColumn.Column || this.selected != dataColumn.Selected)
				return DataObjectCompareResult.NotEqual;
			if(this.alias != dataColumn.Alias)
				return DataObjectCompareResult.EqualExceptForAliases;
			return DataObjectCompareResult.Equal;
		}
		public void SetAlias(string alias, bool raiseChanged) {
			if(this.alias != alias) {
				this.alias = alias;
				if(raiseChanged)
					this.selection.OnColumnAliasChanged(this);
			}
		}
		public void SetSelected(bool selected, bool raiseChanged) {
			if(this.selected != selected) {
				this.selected = selected;
				if(selected)
					this.selection.OnColumnSelected(this);
				else
					this.selection.OnColumnDeselected(this);
				if(raiseChanged)
					this.selection.OnTableChanged();
			}
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlColumn);
			element.Add(new XAttribute(XmlName, this.columnName));
			if(HasAlias)
				element.Add(new XAttribute(XmlAlias, this.alias));
			return element;
		}
	}
}
