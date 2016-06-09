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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
namespace DevExpress.SpreadsheetSource {
	#region ITable
	public interface ITable {
		string Name { get; }
		XlCellRange Range { get; }
		XlCellRange DataRange { get; }
		string RefersTo { get; }
		bool HasHeaderRow { get; }
		bool HasTotalRow { get; }
	}
	#endregion
	#region ITablesCollection
	public interface ITablesCollection : IEnumerable<ITable>, ICollection {
		ITable this[int index] { get; }
		ITable this[string name] { get; }
		IList<ITable> GetTables(string sheetName);
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Implementation {
	using DevExpress.Utils;
	#region TableColumnInfo
	public class TableColumnInfo {
		public string Name { get; set; }
		public XlNumberFormat NumberFormat { get; set; }
		public XlNumberFormat TotalRowNumberFormat { get; set; }
	}
	#endregion
	#region Table
	public class Table : ITable {
		readonly List<TableColumnInfo> columns = new List<TableColumnInfo>();
		public Table(string name, XlCellRange range, bool hasHeaderRow, bool hasTotalRow) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentIsNotNullOrEmpty(range.SheetName, "range.SheetName");
			Name = name;
			Range = range.AsAbsolute();
			HasHeaderRow = hasHeaderRow;
			HasTotalRow = hasTotalRow;
			int topRowIndex = range.FirstRow;
			int bottomRowIndex = range.LastRow;
			if(hasHeaderRow)
				topRowIndex++;
			if(hasTotalRow)
				bottomRowIndex--;
			XlCellRange dataRange = XlCellRange.FromLTRB(range.FirstColumn, topRowIndex, range.LastColumn, bottomRowIndex);
			dataRange.SheetName = Range.SheetName;
			DataRange = dataRange.AsAbsolute();
			RefersTo = Range.ToString(true);
		}
		#region ITable Members
		public string Name { get; private set; }
		public XlCellRange Range { get; private set; }
		public XlCellRange DataRange { get; private set; }
		public string RefersTo { get; private set; }
		public bool HasHeaderRow { get; private set; }
		public bool HasTotalRow { get; private set; }
		public List<TableColumnInfo> Columns { get { return columns; } }
		#endregion
		public bool HasDxfNumberFormats() {
			foreach(TableColumnInfo item in columns) {
				if(item.NumberFormat != null || item.TotalRowNumberFormat != null)
					return true;
			}
			return false;
		}
	}
	#endregion
	#region TablesCollection
	public class TablesCollection : ITablesCollection {
		readonly List<ITable> innerList = new List<ITable>();
		public TablesCollection() {
		}
		public void Add(ITable item) {
			innerList.Add(item);
		}
		public void Clear() {
			innerList.Clear();
		}
		#region ITablesCollection Members
		public ITable this[string name] {
			get {
				foreach(ITable item in innerList) {
					if(item.Name == name)
						return item;
				}
				return null;
			}
		}
		public ITable this[int index] {
			get { return innerList[index]; }
		}
		public IList<ITable> GetTables(string sheetName) {
			List<ITable> result = new List<ITable>();
			foreach(ITable item in innerList) {
				if(item.Range.SheetName == sheetName)
					result.Add(item);
			}
			return result;
		}
		#endregion
		#region IEnumerable<ITable> Members
		public IEnumerator<ITable> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)innerList).GetEnumerator();
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		public int Count {
			get { return innerList.Count; }
		}
		public bool IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		public object SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		#endregion
	}
	#endregion
}
