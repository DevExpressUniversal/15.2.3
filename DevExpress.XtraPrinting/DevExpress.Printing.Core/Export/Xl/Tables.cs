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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.Export.Xl {
}
namespace DevExpress.XtraExport.Implementation {
	using DevExpress.Export.Xl;
	#region XlTotalRowFunction
	public enum XlTotalRowFunction {
		None,
		Average,
		Count,
		CountNums,
		Max,
		Min,
		StdDev,
		Sum,
		Var
	}
	#endregion
	#region XlBuiltInTableStyleId
	public static class XlBuiltInTableStyleId {
		public const string None = "";
		public const string Light1 = "TableStyleLight1";
		public const string Light2 = "TableStyleLight2";
		public const string Light3 = "TableStyleLight3";
		public const string Light4 = "TableStyleLight4";
		public const string Light5 = "TableStyleLight5";
		public const string Light6 = "TableStyleLight6";
		public const string Light7 = "TableStyleLight7";
		public const string Light8 = "TableStyleLight8";
		public const string Light9 = "TableStyleLight9";
		public const string Light10 = "TableStyleLight10";
		public const string Light11 = "TableStyleLight11";
		public const string Light12 = "TableStyleLight12";
		public const string Light13 = "TableStyleLight13";
		public const string Light14 = "TableStyleLight14";
		public const string Light15 = "TableStyleLight15";
		public const string Light16 = "TableStyleLight16";
		public const string Light17 = "TableStyleLight17";
		public const string Light18 = "TableStyleLight18";
		public const string Light19 = "TableStyleLight19";
		public const string Light20 = "TableStyleLight20";
		public const string Light21 = "TableStyleLight21";
		public const string Medium1 = "TableStyleMedium1";
		public const string Medium2 = "TableStyleMedium2";
		public const string Medium3 = "TableStyleMedium3";
		public const string Medium4 = "TableStyleMedium4";
		public const string Medium5 = "TableStyleMedium5";
		public const string Medium6 = "TableStyleMedium6";
		public const string Medium7 = "TableStyleMedium7";
		public const string Medium8 = "TableStyleMedium8";
		public const string Medium9 = "TableStyleMedium9";
		public const string Medium10 = "TableStyleMedium10";
		public const string Medium11 = "TableStyleMedium11";
		public const string Medium12 = "TableStyleMedium12";
		public const string Medium13 = "TableStyleMedium13";
		public const string Medium14 = "TableStyleMedium14";
		public const string Medium15 = "TableStyleMedium15";
		public const string Medium16 = "TableStyleMedium16";
		public const string Medium17 = "TableStyleMedium17";
		public const string Medium18 = "TableStyleMedium18";
		public const string Medium19 = "TableStyleMedium19";
		public const string Medium20 = "TableStyleMedium20";
		public const string Medium21 = "TableStyleMedium21";
		public const string Medium22 = "TableStyleMedium22";
		public const string Medium23 = "TableStyleMedium23";
		public const string Medium24 = "TableStyleMedium24";
		public const string Medium25 = "TableStyleMedium25";
		public const string Medium26 = "TableStyleMedium26";
		public const string Medium27 = "TableStyleMedium27";
		public const string Medium28 = "TableStyleMedium28";
		public const string Dark1 = "TableStyleDark1";
		public const string Dark2 = "TableStyleDark2";
		public const string Dark3 = "TableStyleDark3";
		public const string Dark4 = "TableStyleDark4";
		public const string Dark5 = "TableStyleDark5";
		public const string Dark6 = "TableStyleDark6";
		public const string Dark7 = "TableStyleDark7";
		public const string Dark8 = "TableStyleDark8";
		public const string Dark9 = "TableStyleDark9";
		public const string Dark10 = "TableStyleDark10";
		public const string Dark11 = "TableStyleDark11";
	}
	#endregion
	#region IXlTableColumn
	public interface IXlTableColumn {
		string Name { get; }
		XlTotalRowFunction TotalRowFunction { get; set; }
		string TotalRowLabel { get; set; }
		XlDifferentialFormatting DataFormatting { get; set; }
		XlDifferentialFormatting HeaderRowFormatting { get; set; }
		XlDifferentialFormatting TotalRowFormatting { get; set; }
	}
	#endregion
	#region IXlTableColumnCollection
	public interface IXlTableColumnCollection : IEnumerable<IXlTableColumn>, IEnumerable {
		IXlTableColumn this[int index] { get; }
		int Count { get; }
	}
	#endregion
	#region IXlTableStyleInfo
	public interface IXlTableStyleInfo {
		string Name { get; set; }
		bool ShowColumnStripes { get; set; }
		bool ShowRowStripes { get; set; }
		bool ShowFirstColumn { get; set; }
		bool ShowLastColumn { get; set; }
	}
	#endregion
	#region IXlTable
	public interface IXlTable {
		string Comment { get; set; }
		string Name { get; set; }
		bool HasHeaderRow { get; }
		bool HasTotalRow { get; }
		bool HasAutoFilter { get; set; }
		bool InsertRowShowing { get; set; }
		bool InsertRowShift { get; set; }
		bool Published { get; set; }
		IXlCellRange Range { get; }
		IXlTableColumnCollection Columns { get; }
		IXlTableStyleInfo Style { get; }
		XlDifferentialFormatting DataFormatting { get; set; }
		XlDifferentialFormatting HeaderRowFormatting { get; set; }
		XlDifferentialFormatting TotalRowFormatting { get; set; }
		XlDifferentialFormatting HeaderRowBorderFormatting { get; set; }
		XlDifferentialFormatting TableBorderFormatting { get; set; }
		XlDifferentialFormatting TotalRowBorderFormatting { get; set; }
	}
	#endregion
	#region XlTableFormattingBase (abstract)
	public abstract class XlTableFormattingBase {
		public XlDifferentialFormatting DataFormatting { get; set; }
		public XlDifferentialFormatting HeaderRowFormatting { get; set; }
		public XlDifferentialFormatting TotalRowFormatting { get; set; }
	}
	#endregion
	#region XlTableColumn
	public class XlTableColumn : XlTableFormattingBase, IXlTableColumn {
		string name;
		public string Name {
			get { return name; }
			set {
				Guard.ArgumentIsNotNullOrEmpty(value, "Name");
				name = value;
			}
		}
		public XlTotalRowFunction TotalRowFunction { get; set; }
		public string TotalRowLabel { get; set; }
	}
	#endregion
	#region XlTableColumnCollection
	public class XlTableColumnCollection : IXlTableColumnCollection {
		#region Enumerator
		class XlTableColumnEnumerator : IEnumerator<IXlTableColumn> {
			IEnumerator<XlTableColumn> innerEnumerator;
			public XlTableColumnEnumerator(IEnumerator<XlTableColumn> enumerator) {
				this.innerEnumerator = enumerator;
			}
			#region IEnumerator<IXlTableColumn> Members
			public IXlTableColumn Current {
				get { return innerEnumerator.Current; }
			}
			#endregion
			#region IDisposable Members
			public void Dispose() {
				innerEnumerator.Dispose();
			}
			#endregion
			#region IEnumerator Members
			object IEnumerator.Current {
				get { return Current; }
			}
			public bool MoveNext() {
				return innerEnumerator.MoveNext();
			}
			public void Reset() {
				innerEnumerator.Reset();
			}
			#endregion
		}
		#endregion
		readonly List<XlTableColumn> innerList;
		public XlTableColumnCollection(List<XlTableColumn> columns) {
			innerList = columns;
		}
		#region IXlTableColumnCollection Members
		public IXlTableColumn this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		#endregion
		#region IEnumerable<IXlTableColumn> Members
		public IEnumerator<IXlTableColumn> GetEnumerator() {
			return new XlTableColumnEnumerator(innerList.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region XlTableStyleInfo
	public class XlTableStyleInfo : IXlTableStyleInfo {
		public XlTableStyleInfo() {
			Name = "TableStyleMedium2";
			ShowRowStripes = true;
		}
		public string Name { get; set; }
		public bool ShowColumnStripes { get; set; }
		public bool ShowRowStripes { get; set; }
		public bool ShowFirstColumn { get; set; }
		public bool ShowLastColumn { get; set; }
	}
	#endregion
	#region XlTable
	public class XlTable : XlTableFormattingBase, IXlTable {
		string name;
		readonly XlCellRange range;
		readonly XlTableStyleInfo styleInfo;
		readonly List<XlTableColumn> innerColumns = new List<XlTableColumn>();
		readonly XlTableColumnCollection columns;
		public XlTable(XlCellRange range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range.AsRelative();
			this.range.SheetName = string.Empty;
			this.styleInfo = new XlTableStyleInfo();
			this.HasHeaderRow = true;
			this.HasAutoFilter = true;
			int count = range.ColumnCount;
			this.innerColumns = new List<XlTableColumn>(count);
			for(int i = 0; i < count; i++)
				innerColumns.Add(new XlTableColumn() { Name = string.Format("Column{0}", i + 1) });
			this.columns = new XlTableColumnCollection(innerColumns);
		}
		internal int Id { get; set; }
		public string Comment { get; set; }
		public string Name {
			get { return name; }
			set {
				if(!IsValidTableName(value))
					throw new ArgumentException(string.Format("'{0}' is not valid table name.", value));
				name = value;
			}
		}
		public bool HasHeaderRow { get; set; }
		public bool HasTotalRow { get; set; }
		public bool HasAutoFilter { get; set; }
		public bool InsertRowShowing { get; set; }
		public bool InsertRowShift { get; set; }
		public bool Published { get; set; }
		public IXlCellRange Range { get { return range; } }
		public IXlTableColumnCollection Columns { get { return columns; } }
		public IXlTableStyleInfo Style { get { return styleInfo; } }
		public XlDifferentialFormatting HeaderRowBorderFormatting { get; set; }
		public XlDifferentialFormatting TableBorderFormatting { get; set; }
		public XlDifferentialFormatting TotalRowBorderFormatting { get; set; }
		internal void Validate() {
			int minRowCount = 1;
			if(HasHeaderRow)
				minRowCount++;
			if(HasTotalRow)
				minRowCount++;
			if(range.RowCount < minRowCount)
				throw new Exception(string.Format("Table range must contains at least {0} rows.", minRowCount));
			HashSet<string> uniqueColumnNames = new HashSet<string>();
			foreach(XlTableColumn column in innerColumns) {
				if(uniqueColumnNames.Contains(column.Name))
					throw new Exception("Table columns must have unique names.");
				uniqueColumnNames.Add(column.Name);
			}
		}
		bool IsValidTableName(string value) {
			if(string.IsNullOrEmpty(value))
				return true;
			return XlNameChecker.IsValidIdentifier(value);
		}
	}
	#endregion
	#region XlNameChecker
	static class XlNameChecker {
		public static bool IsValidIdentifier(string value) {
			Guard.ArgumentIsNotNullOrEmpty(value, "value");
			for(int i = 0; i < value.Length; i++) {
				if(!IsValidIdentifierChar(value[i], i, value))
					return false;
			}
			return true;
		}
		static bool IsValidIdentifierChar(char curChar, int index, string value) {
			if(index == 0) {
				if(curChar == '\\') {
					if(value.Length == 1)
						return true;
					if(value[1] != '\\' && value[1] != '.' && value[1] != '?' && value[1] != '_')
						return false;
				}
				else
					if(!char.IsLetter(curChar) && curChar != '_')
						return false;
			}
			return !(!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '.' && curChar != '\\' && curChar != '?');
		}
	}
	#endregion
}
