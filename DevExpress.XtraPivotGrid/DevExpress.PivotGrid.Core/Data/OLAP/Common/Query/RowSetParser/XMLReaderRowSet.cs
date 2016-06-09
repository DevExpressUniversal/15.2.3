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
using System.Xml;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.OLAP {
	class XMLReaderRowSet : XmlReaderParserBase, IOLAPRowSet {
		List<XmlReaderRowSetColumn> columns = new List<XmlReaderRowSetColumn>();
		object[] row;
		Dictionary<string, int> indexes = new Dictionary<string, int>();
		public XMLReaderRowSet(XmlReader reader)
			: base(reader) {
			CheckException();
			reader.ReadStartElement();
			CheckException();
			reader.ReadStartElement(XmlaEntityName.schema, XmlaName.HttpSchema);
			CheckException();
			bool schemaFounded = false;
			while(reader.IsStartElement() && !schemaFounded) {
				if(!schemaFounded && reader.GetAttribute(XmlaAttributeName.name) == XmlaEntityName.Row) {
					schemaFounded = true;
					reader.ReadStartElement();
					reader.ReadStartElement();
					while(reader.IsStartElement()) {
						XmlReaderRowSetColumn column = new XmlReaderRowSetColumn(
						   reader.GetAttribute(XmlaAttributeName.name),
							reader.GetAttribute(XmlaAttributeName.field, XmlaName.XmlnsSqlNamespace),
							XsdTypeConverter.TypeOf(reader.GetAttribute(XmlaAttributeName.type))
							);
						columns.Add(column);
						indexes.Add(column.Name, columns.Count - 1);
						reader.Skip();
					}
					reader.ReadEndElement();
					reader.ReadEndElement();
				} else
					reader.Skip();
			}
			while(reader.IsStartElement())
				reader.Skip();
			reader.ReadEndElement();
			row = new object[columns.Count];
		}
		int IOLAPRowSet.ColumnCount {
			get { return columns.Count; }
		}
		Type IOLAPRowSet.GetColumnType(int index) {
			return columns[index].Type;
		}
		string IOLAPRowSet.GetColumnName(int index) {
			return columns[index].SqlName;
		}
		object IOLAPRowSet.GetCellValue(int columnIndex) {
			return row[columnIndex];
		}
		string IOLAPRowSet.GetCellStringValue(int columnIndex) {
			return (string)row[columnIndex];
		}
		bool IOLAPRowSet.NextRow() {
			if(reader != null && reader.IsStartElement(XmlaEntityName.Row)) {
				for(int i = 0; i < row.Length; i++)
					row[i] = null;
				reader.ReadStartElement();
				while(reader.IsStartElement()) {
					string eName = reader.Name;
					int index;
					if(eName != null && indexes.TryGetValue(eName, out index) && !reader.IsEmptyElement) {
						object val = null;
						try {
							val = Convert.ChangeType(reader.ReadElementContentAsString(), columns[index].Type);
						} catch { }
						row[index] = val;
					} else
						reader.Skip();
				}
				reader.ReadEndElement();
				return true;
			}
			reader.Close();
			return false;
		}
	}
}
