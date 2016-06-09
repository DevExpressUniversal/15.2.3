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
using System.Globalization;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model.External {
	#region DdeExternalWorkbook
	public class DdeExternalWorkbook : ExternalWorkbook {
		#region Fields
		const char ddeSeparator = '|';
		string ddeServiceName;
		string ddeServerTopic;
		#endregion
		public DdeExternalWorkbook(DocumentModel hostWorkbook)
			: base(hostWorkbook) {
			this.ddeServiceName = string.Empty;
			this.ddeServerTopic = string.Empty;
		}
		#region Properties
		public string DdeServiceName { 
			get { return ddeServiceName; } 
			set {
				if(string.IsNullOrEmpty(value))
					ddeServiceName = string.Empty;
				else
					ddeServiceName = value; 
			} 
		}
		public string DdeServerTopic { 
			get { return ddeServerTopic; } 
			set {
				if(string.IsNullOrEmpty(value))
					ddeServerTopic = string.Empty;
				else
					ddeServerTopic = value;
			} 
		}
		public override string FilePath {
			get {
				return ddeServiceName + ddeSeparator.ToString() + ddeServerTopic;
			}
			set {
				if(string.IsNullOrEmpty(value)) {
					ddeServiceName = string.Empty;
					ddeServerTopic = string.Empty;
				}
				else {
					string[] parts = value.Split(new char[] { ddeSeparator });
					if(parts.Length != 2)
						throw new ArgumentException("Invalid DDE server|topic.");
					ddeServiceName = parts[0];
					ddeServerTopic = parts[1];
				}
			}
		}
		#endregion
		protected internal bool IsOleLink {
			get {
				if (Sheets.Count == 0)
					return false;
				foreach (ExternalWorksheet sheet in Sheets) {
					DdeExternalWorksheet item = sheet as DdeExternalWorksheet;
					if (!item.IsOleLink)
						return false;
				}
				return true;
			}
		}
	}
	#endregion
	#region DdeExternalWorksheet
	public class DdeExternalWorksheet : ExternalWorksheet {
		#region Fields
		bool isUsesOLE;
		bool advise;
		bool isDataImage;
		int columnCount;
		int rowCount;
		#endregion
		public DdeExternalWorksheet(IModelWorkbook workbook, string name) 
			: base(workbook, name) {
		}
		#region Properties
		public bool IsUsesOLE { get { return isUsesOLE; } set { isUsesOLE = value; } }
		public bool Advise { get { return advise; } set { advise = value; } }
		public bool IsDataImage { get { return isDataImage; } set { isDataImage = value; } }
		public bool IsOleLink { get; set; }
		public int ColumnCount { 
			get { return columnCount; } 
			set {
				ValueChecker.CheckValue(value, 0, IndicesChecker.MaxColumnCount, "ColumnCount");
				columnCount = value; 
			} 
		}
		public int RowCount { 
			get { return rowCount; } 
			set {
				ValueChecker.CheckValue(value, 0, IndicesChecker.MaxRowCount, "RowCount");
				rowCount = value; 
			} 
		}
		protected internal int ValuesCount {
			get {
				int result = 0;
				if(columnCount == 0 || rowCount == 0)
					return result;
				CellRange range = new CellRange(this, new CellPosition(0, 0), new CellPosition(columnCount - 1, rowCount - 1));
				foreach(ICellBase info in range.GetExistingCellsEnumerable()) {
					ExternalCell cell = info as ExternalCell;
					if(cell != null)
						result++;
				}
				return result;
			}
		}
		#endregion
	}
	#endregion
	#region ExternalDdeItemDefinition
	public class ExternalDdeItemDefinition {
		#region Fields
		ExternalDdeLinkValueCollection ddeLinkValues;
		string ddeName;
		bool isUsesOLE;
		bool advise;
		bool isDataImage;
		#endregion
		public ExternalDdeItemDefinition() {
			this.ddeLinkValues = new ExternalDdeLinkValueCollection();
		}
		#region Properties
		public ExternalDdeLinkValueCollection DdeLinkValues { get { return ddeLinkValues; } }
		public string DdeName { get { return ddeName; } set { ddeName = value; } }
		public bool IsUsesOLE { get { return isUsesOLE; } set { isUsesOLE = value; } }
		public bool Advise { get { return advise; } set { advise = value; } }
		public bool IsDataImage { get { return isDataImage; } set { isDataImage = value; } }
		#endregion
	}
	#endregion
	#region DdeValueType
	public enum DdeValueType {
		Nil,
		Boolean,
		RealNumber,
		Error,
		String
	}
	#endregion
	#region ExternalDdeLinkValue
	public class ExternalDdeLinkValue {
		#region Fields
		string value;
		DdeValueType type;
		#endregion
		#region Properties
		public string Value { get { return value; } set { this.value = value; } }
		public DdeValueType Type { get { return type; } set { type = value; } }
		#endregion
		public VariantValue GetVariantValue(WorkbookDataContext context) {
			VariantValue result = VariantValue.Empty;
			if(type == DdeValueType.String) {
				result = new VariantValue();
				result.InlineTextValue = value;
			}
			else if(type == DdeValueType.Error) {
				ICellError error;
				if (CellErrorFactory.TryCreateErrorByInvariantName(value, out error))
					result = error.Value;
			}
			else if(type == DdeValueType.Boolean) {
				result = new VariantValue();
				result.BooleanValue = value == "1" || StringExtensions.CompareInvariantCultureIgnoreCase(value, "true") == 0;
			}
			else if(type == DdeValueType.RealNumber) {
				result = new VariantValue();
				double numericValue = 0;
				double.TryParse(value, NumberStyles.Float, context.Culture, out numericValue);
				result.NumericValue = numericValue;
			}
			return result;
		}
		public void SetVariantValue(VariantValue value) {
			if(value.IsBoolean) {
				Type = DdeValueType.Boolean;
				Value = value.BooleanValue ? "1" : "0";
			}
			else if(value.IsError) {
				Type = DdeValueType.Error;
				Value = value.ErrorValue.Name;
			}
			else if(value.IsInlineText) {
				Type = DdeValueType.String;
				Value = value.InlineTextValue;
			}
			else if(value.IsNumeric) {
				Type = DdeValueType.RealNumber;
				Value = value.NumericValue.ToString("G17", CultureInfo.InvariantCulture);
			}
			else
				Type = DdeValueType.Nil;
		}
	}
	#endregion
	#region ExternalDdeLinkValueCollection
	public class ExternalDdeLinkValueCollection : SimpleCollection<ExternalDdeLinkValue> {
		#region Fields
		int columns;
		int rows;
		#endregion
		#region Properties
		public int Columns { get { return columns; } set { columns = value; } }
		public int Rows { get { return rows; } set { rows = value; } }
		#endregion
	}
	#endregion
}
