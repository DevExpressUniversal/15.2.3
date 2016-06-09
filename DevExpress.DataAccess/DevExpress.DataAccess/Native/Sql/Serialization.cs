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
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public sealed class FieldDescriptor {
		internal const string XML_Field = "Field";
		internal const string XML_Name = "Name";
		internal const string XML_FieldType = "Type";
		internal const string XML_DisplayName = "DisplayName";
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public Type FieldType { get; set; }
		public XElement SaveToXml() {
			XElement element = new XElement(XML_Field);
			element.Add(new XAttribute(XML_Name, Name ?? string.Empty));
			element.Add(new XAttribute(XML_FieldType, DBColumn.GetColumnType(FieldType, true).ToString()));
			if(!string.IsNullOrEmpty(DisplayName) && !string.Equals(Name, DisplayName, StringComparison.Ordinal))
				element.Add(new XAttribute(XML_DisplayName, DisplayName));
			return element;
		}
		public static FieldDescriptor LoadFromXml(XElement element) {
			return new FieldDescriptor() {
				Name = XmlHelperBase.GetAttributeValue(element, XML_Name),
				FieldType = DBColumn.GetType((DBColumnType)Enum.Parse(typeof(DBColumnType), XmlHelperBase.GetAttributeValue(element, XML_FieldType))),
				DisplayName = XmlHelperBase.GetAttributeValue(element, XML_DisplayName)
			};
		}
	}
	public sealed class ViewDescriptor : List<FieldDescriptor> {
		internal const string XML_View = "View";
		internal const string XML_Name = "Name";
		internal const string XML_DisplayName = "DisplayName";
		public ViewDescriptor() { }
		public ViewDescriptor(int capacity) : base(capacity) { }
		public ViewDescriptor(IEnumerable<FieldDescriptor> collection) : base(collection) { }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public XElement SaveToXml() {
			XElement element = new XElement(XML_View);
			if(!string.IsNullOrEmpty(Name))
				element.Add(new XAttribute(XML_Name, Name));
			if(!string.IsNullOrEmpty(DisplayName) && !string.Equals(Name, DisplayName, StringComparison.Ordinal))
				element.Add(new XAttribute(XML_DisplayName, DisplayName));
			foreach(FieldDescriptor fd in this)
				element.Add(fd.SaveToXml());
			return element;
		}
		public static ViewDescriptor LoadFromXml(XElement element) {
			ViewDescriptor result = new ViewDescriptor() { 
				Name = XmlHelperBase.GetAttributeValue(element, XML_Name), 
				DisplayName = XmlHelperBase.GetAttributeValue(element, XML_DisplayName) 
			};
			foreach(XElement fieldElement in element.Elements(FieldDescriptor.XML_Field))
				result.Add(FieldDescriptor.LoadFromXml(fieldElement));
			return result;
		}
	}
	internal sealed class KeyColumnDescriptor {
		internal const string XML_KeyColumn = "KeyColumn";
		internal const string XML_MasterTableColumn = "Master";
		internal const string XML_DetailTableColumn = "Detail";
		public string MasterTableColumn { get; set; }
		public string DetailTableColumn { get; set; }
		public XElement SaveToXml() {
			XElement element = new XElement(XML_KeyColumn);
			element.Add(new XAttribute(XML_MasterTableColumn, MasterTableColumn));
			element.Add(new XAttribute(XML_DetailTableColumn, DetailTableColumn));
			return element;
		}
		public static KeyColumnDescriptor LoadFromXml(XElement element) {
			KeyColumnDescriptor result = new KeyColumnDescriptor() { 
				MasterTableColumn = XmlHelperBase.GetAttributeValue(element, XML_MasterTableColumn), 
				DetailTableColumn = XmlHelperBase.GetAttributeValue(element, XML_DetailTableColumn) 
			};
			return result;
		}
	}
	internal sealed class RelationDescriptor : List<KeyColumnDescriptor> {
		internal const string XML_Relation = "Relation";
		internal const string XML_Name = "Name";
		internal const string XML_Master = "Master";
		internal const string XML_Detail = "Detail";
		public string MasterTable { get; set; }
		public string DetailTable { get; set; }
		public string CustomName { get; set; }
		public XElement SaveToXml() {
			XElement element = new XElement(XML_Relation);
			if(CustomName != null)
				element.Add(new XAttribute(XML_Name, CustomName));
			element.Add(new XAttribute(XML_Master, MasterTable));
			element.Add(new XAttribute(XML_Detail, DetailTable));
			foreach(KeyColumnDescriptor column in this)
				element.Add(column.SaveToXml());
			return element;
		}
		public static RelationDescriptor LoadFromXml(XElement element) {
			RelationDescriptor result = new RelationDescriptor() { 
				MasterTable = XmlHelperBase.GetAttributeValue(element, XML_Master), 
				DetailTable = XmlHelperBase.GetAttributeValue(element, XML_Detail), 
				CustomName = XmlHelperBase.GetAttributeValue(element, XML_Name)
			};
			foreach(XElement columnElement in element.Elements(KeyColumnDescriptor.XML_KeyColumn))
				result.Add(KeyColumnDescriptor.LoadFromXml(columnElement));
			return result;
		}
	}
	internal sealed class FieldListDescriptor : List<ViewDescriptor> {
		internal const string XML_DataSet = "DataSet";
		internal const string XML_Name = "Name";
		internal const string XML_DisplayName = "DisplayName";
		readonly List<RelationDescriptor> relations = new List<RelationDescriptor>();
		public FieldListDescriptor() { }
		public FieldListDescriptor(int capacity) : base(capacity) { }
		public FieldListDescriptor(IEnumerable<ViewDescriptor> collection) : base(collection) {  }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public List<RelationDescriptor> Relations { get { return this.relations; } }
		public XElement SaveToXml() {
			if(this.Count == 0)
				return null;
			XElement element = new XElement(XML_DataSet);
			if(!string.IsNullOrEmpty(Name))
				element.Add(new XAttribute(XML_Name, Name));
			if(!string.IsNullOrEmpty(DisplayName) && !string.Equals(Name, DisplayName, StringComparison.Ordinal))
				element.Add(new XAttribute(XML_DisplayName, DisplayName));
			foreach(ViewDescriptor vd in this)
				element.Add(vd.SaveToXml());
			foreach(RelationDescriptor rd in Relations)
				element.Add(rd.SaveToXml());
			return element;
		}
		public static FieldListDescriptor LoadFromXml(XElement element) {
			FieldListDescriptor result = new FieldListDescriptor();
			result.Name = XmlHelperBase.GetAttributeValue(element, XML_Name);
			result.DisplayName = XmlHelperBase.GetAttributeValue(element, XML_DisplayName);
			foreach(XElement viewElement in element.Elements(ViewDescriptor.XML_View))
				result.Add(ViewDescriptor.LoadFromXml(viewElement));
			foreach(XElement relElement in element.Elements(RelationDescriptor.XML_Relation))
				result.Relations.Add(RelationDescriptor.LoadFromXml(relElement));
			return result;
		}
		public void FillResultSet(ResultSet resultSet) {
			List<ResultTable> tables = new List<ResultTable>();
			foreach(ViewDescriptor viewDescriptor in this) {
				ResultTable table = new ResultTable(viewDescriptor.Name) {DisplayName = viewDescriptor.DisplayName};
				foreach(FieldDescriptor fieldDescriptor in viewDescriptor) {
					ResultColumn column = table.AddColumn(fieldDescriptor.Name, fieldDescriptor.FieldType);
					if(!string.IsNullOrEmpty(fieldDescriptor.DisplayName))
						column.SetDisplayName(fieldDescriptor.DisplayName);
				}
				tables.Add(table);
			}
			resultSet.SetTables(tables);
			foreach(RelationDescriptor relationDescriptor in this.relations) {
				if(!resultSet.Contains(relationDescriptor.MasterTable) || !resultSet.Contains(relationDescriptor.DetailTable))
					continue;
				ResultTable master = resultSet[relationDescriptor.MasterTable];
				ResultTable detail = resultSet[relationDescriptor.DetailTable];
				IEnumerable<RelationColumnInfo> columns =
					relationDescriptor.Select(
						descriptor => new RelationColumnInfo(descriptor.MasterTableColumn, descriptor.DetailTableColumn));
				if(relationDescriptor.CustomName != null)
					master.AddDetail(detail, columns, relationDescriptor.CustomName);
				else
					master.AddDetail(detail, columns);
			}
		}
		public static FieldListDescriptor ConvertFromResultSet(ResultSet resultSet) {
			if(resultSet == null)
				return null;
			FieldListDescriptor dd = new FieldListDescriptor { Name = resultSet.Name };
			foreach(ResultTable table in resultSet.Tables) {
				ViewDescriptor vd = new ViewDescriptor { Name = table.TableName, DisplayName = table.DisplayName };
				foreach(ResultColumn column in table.Columns) {
					FieldDescriptor fd = new FieldDescriptor {
						Name = column.Name,
						FieldType = column.PropertyType,
						DisplayName = column.DisplayName
					};
					vd.Add(fd);
				}
				dd.Add(vd);
				foreach(ResultRelation rel in table.Details) {
					RelationDescriptor rd = new RelationDescriptor {
						MasterTable = rel.Master.TableName,
						DetailTable = rel.Detail.TableName
					};
					if(rel.Name != rd.MasterTable + rd.DetailTable)
						rd.CustomName = rel.Name;
					foreach(var keyCol in rel.KeyColumns) {
						KeyColumnDescriptor kd = new KeyColumnDescriptor {
							MasterTableColumn = keyCol.ParentKeyColumn,
							DetailTableColumn = keyCol.NestedKeyColumn
						};
						rd.Add(kd);
					}
					dd.Relations.Add(rd);
				}
			}
			return dd;
		}
	}
}
