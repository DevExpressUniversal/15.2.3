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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {	
	public class DataReference {
		internal const BinaryOperatorType DefaultOperatorType = BinaryOperatorType.Equal;
		internal const string XmlReference = "Reference";
		internal const string XmlKey = "Key";
		internal const string XmlParentKey = "ParentKey";
		internal const string XmlParentTable = "ParentTable";
		internal const string XmlOperator = "Operator";
		public static BinaryOperatorType GetOperatorTypeFormSurrogate(string operatorTypeAttr) {
			return operatorTypeAttr != null ? XmlHelperBase.EnumFromString<BinaryOperatorType>(operatorTypeAttr) : DefaultOperatorType;
		}
		readonly DataSelection dataSelection;
		DBTable keyDBTable;
		string keyDBTableName;
		string keyColumn;
		DataTable parentDataTable;
		readonly string parentDataTableAlias;
		string parentKeyColumn;
		BinaryOperatorType operatorType;
		public DataSelection DataSelection { get { return this.dataSelection; } }
		public DBTable KeyDBTable
		{
			get
			{
				if(this.keyDBTable == null)
					this.keyDBTable = this.dataSelection.DataProvider.FindTable(this.keyDBTableName);
				return this.keyDBTable;
			}
			private set
			{
				this.keyDBTable = value;
				this.keyDBTableName = value.Name;
			}
		}
		public string KeyDBTableName
		{
			get
			{
				if(KeyDBTable == null)
					return this.keyDBTableName;
				return KeyDBTable.Name;
			}
		}
		public string KeyColumn { get { return this.keyColumn; } set { this.keyColumn = value; } }
		public DataTable ParentDataTable
		{
			get
			{
				if(this.parentDataTable == null)
					this.parentDataTable = this.dataSelection.Find(dataTable => dataTable.Alias == this.parentDataTableAlias);
				return this.parentDataTable;
			}
			private set { this.parentDataTable = value; }
		}
		public string ParentKeyColumn { get { return this.parentKeyColumn; } private set { this.parentKeyColumn = value; } }
		public BinaryOperatorType OperatorType { get { return this.operatorType; } set { SetOperatorType(value, true); } }
		public DataReference(DataSelection dataSelection, string keyColumn, DBTable keyDBTable, BinaryOperatorType operatorType, string parentKey, string parentDataTableAlias) {
			Guard.ArgumentIsNotNullOrEmpty(keyColumn, "keyColumn");
			Guard.ArgumentIsNotNullOrEmpty(parentKey, "parentKey");
			this.dataSelection = dataSelection;
			this.keyColumn = keyColumn;
			KeyDBTable = keyDBTable;
			SetOperatorType(operatorType, false);
			ParentKeyColumn = parentKey;
			this.parentDataTableAlias = parentDataTableAlias;
		}
		public DataReference(DataSelection dataSelection, string keyColumn, DBTable keyDBTable, BinaryOperatorType operatorType, string parentKey, DataTable parentDataTable) {
			Guard.ArgumentIsNotNullOrEmpty(keyColumn, "keyColumn");
			Guard.ArgumentIsNotNullOrEmpty(parentKey, "parentKey");
			this.dataSelection = dataSelection;
			this.keyColumn = keyColumn;
			this.keyDBTable = keyDBTable;
			SetOperatorType(operatorType, false);
			ParentKeyColumn = parentKey;
			ParentDataTable = parentDataTable;
		}
		public DataReference(DataSelection dataSelection, string keyColumn, DBTable dbTable, string parentKey, DataTable parentDataTable)
			: this(dataSelection, keyColumn, dbTable, BinaryOperatorType.Equal, parentKey, parentDataTable) {
		}
		public DataReference(DataSelection dataSelection, string keyDBTableName, XElement element) {
			this.dataSelection = dataSelection;
			this.keyColumn = XmlHelperBase.GetAttributeValue(element, XmlKey);
			this.keyDBTableName = keyDBTableName;
			string parentKey = XmlHelperBase.GetAttributeValue(element, XmlParentKey);
			string parentTableAlias = XmlHelperBase.GetAttributeValue(element, XmlParentTable);
			if(String.IsNullOrEmpty(KeyColumn) || String.IsNullOrEmpty(parentKey) || String.IsNullOrEmpty(parentTableAlias))
				throw new XmlException();
			string operatorTypeString = XmlHelperBase.GetAttributeValue(element, XmlOperator);
			SetOperatorType(GetOperatorTypeFormSurrogate(operatorTypeString), false);
			ParentDataTable = dataSelection.Find(t => t.Alias == parentTableAlias);
			ParentKeyColumn = parentKey;
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlReference);
			element.Add(new XAttribute(XmlKey, KeyColumn));
			element.Add(new XAttribute(XmlParentKey, ParentKeyColumn));
			element.Add(new XAttribute(XmlParentTable, ParentDataTable.Alias));
			if(this.operatorType != DefaultOperatorType)
				element.Add(new XAttribute(XmlOperator, this.operatorType));
			return element;
		}
		public void LoadDataObjects() {
			if(this.keyDBTable == null)
				this.keyDBTable = this.dataSelection.DataProvider.FindTable(this.keyDBTableName);
		}
		public bool CompareWith(DataReference dataReference) {
			if(KeyColumn != dataReference.KeyColumn || KeyDBTable != dataReference.KeyDBTable || ParentKeyColumn != dataReference.ParentKeyColumn)
				return false;
			if(ParentDataTable.UniqueName != dataReference.ParentDataTable.UniqueName)
				return false;
			return true;
		}
		public DataReference Clone() {
			return new DataReference(this.dataSelection, KeyColumn, KeyDBTable, this.operatorType, ParentKeyColumn, ParentDataTable);
		}
		public override string ToString() {
			if(this.parentDataTable != null) {
				DBColumn column = this.parentDataTable.Table.Columns.FirstOrDefault(p => p.Name == this.ParentKeyColumn);
#pragma warning disable 612, 618
				DBColumnWithAlias customColumn = column as DBColumnWithAlias;
				if(customColumn != null)
					return string.Format("[{0}].[{1}]", this.ParentDataTable.Alias, customColumn.Alias);
#pragma warning restore 612, 618
				if(column != null)
					return string.Format("[{0}].[{1}]", this.ParentDataTable.Alias, column.Name);
			}
			return string.Format("[{0}].[{1}]", this.ParentDataTable.Alias, this.ParentKeyColumn);
		}
		void SetOperatorType(BinaryOperatorType newOperatorType, bool raiseChanged) {
			if(this.operatorType != newOperatorType) {
				this.operatorType = newOperatorType;
				if(raiseChanged)
					this.dataSelection.OnTableChanged();
			}
		}
	}
}
