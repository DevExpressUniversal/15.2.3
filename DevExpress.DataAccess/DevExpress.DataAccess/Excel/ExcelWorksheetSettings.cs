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

using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public sealed class ExcelWorksheetSettings : ExcelSettingsBase {
		const string xml_WorksheetName = "WorksheetName";
		const string xml_CellRange = "CellRange";
		public string WorksheetName { get; set; }
		public string CellRange { get; set; }
		public ExcelWorksheetSettings() {}
		public ExcelWorksheetSettings(string worksheetName) : this(worksheetName, null) { }
		public ExcelWorksheetSettings(string worksheetName, string cellRange) {
			WorksheetName = worksheetName;
			CellRange = cellRange;
		}
		#region Equality members
		bool Equals(ExcelWorksheetSettings other) {
			return string.Equals(WorksheetName, other.WorksheetName) 
				&& string.Equals(CellRange, other.CellRange);
		}
		public override bool Equals(object obj) {
			var other = obj as ExcelWorksheetSettings;
			return other != null && Equals(other);
		}
		public override int GetHashCode() {
			return 0;
		}
		#endregion
		protected internal override ISpreadsheetDataReader CreateReader(ISpreadsheetSource source) {
			if(!string.IsNullOrEmpty(WorksheetName) && !string.IsNullOrEmpty(CellRange))
				return source.GetDataReader(source.Worksheets[WorksheetName], XlCellRange.Parse(CellRange));
			if(!string.IsNullOrEmpty(CellRange))
				return source.GetDataReader(XlCellRange.Parse(CellRange));
			return source.GetDataReader(source.Worksheets[WorksheetName]);
		}
		protected internal override void SaveToXml(XElement worksheetSettings) {
			Guard.ArgumentNotNull(worksheetSettings, "worksheetSettings");
			if(!string.IsNullOrEmpty(WorksheetName))
				worksheetSettings.Add(new XAttribute(xml_WorksheetName, WorksheetName));
			if(!string.IsNullOrEmpty(CellRange))
				worksheetSettings.Add(new XAttribute(xml_CellRange, CellRange));
		}
		protected internal override void LoadFromXml(XElement worksheetSettings) {
			Guard.ArgumentNotNull(worksheetSettings, "cellRangeSettings");
			WorksheetName = worksheetSettings.GetAttributeValue(xml_WorksheetName);
			CellRange = worksheetSettings.GetAttributeValue(xml_CellRange);
		}
		protected internal override ExcelSettingsBase Clone() {
			return new ExcelWorksheetSettings(WorksheetName, CellRange);
		}
	}
}
