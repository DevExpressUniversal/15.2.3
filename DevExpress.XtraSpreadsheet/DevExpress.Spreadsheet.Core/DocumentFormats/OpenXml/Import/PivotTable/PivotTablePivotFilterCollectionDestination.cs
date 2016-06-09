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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTablePivotFilterCollectionDestination
	public class PivotTablePivotFilterCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotFilterCollection filters;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("filter", OnFilter);
			return result;
		}
		#endregion
		public PivotTablePivotFilterCollectionDestination(SpreadsheetMLBaseImporter importer, PivotFilterCollection filters, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.filters = filters;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotFilterCollection Filters { get { return filters; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFilterCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFilterCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFilterCollectionDestination self = GetThis(importer);
			return new PivotFilterDestination(importer, self.Filters, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotFilterDestination
	public class PivotFilterDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		PivotFilter filter;
		readonly PivotFilterCollection filters;
		readonly Worksheet worksheet;
		public static Dictionary<PivotFilterType, string> pivotTablePivotFilterTypeTable = CreatePivotTablePivotFilterTypeTable();
		public static Dictionary<string, PivotFilterType> reversePivotTablePivotFilterTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTablePivotFilterTypeTable);
		static Dictionary<PivotFilterType, string> CreatePivotTablePivotFilterTypeTable() {
			Dictionary<PivotFilterType, string> result = new Dictionary<PivotFilterType, string>();
			result.Add(PivotFilterType.CaptionBeginsWith, "captionBeginsWith");
			result.Add(PivotFilterType.CaptionBetween, "captionBetween");
			result.Add(PivotFilterType.CaptionContains, "captionContains");
			result.Add(PivotFilterType.CaptionEndsWith, "captionEndsWith");
			result.Add(PivotFilterType.CaptionEqual, "captionEqual");
			result.Add(PivotFilterType.CaptionGreaterThan, "captionGreaterThan");
			result.Add(PivotFilterType.CaptionGreaterThanOrEqual, "captionGreaterThanOrEqual");
			result.Add(PivotFilterType.CaptionLessThan, "captionLessThan");
			result.Add(PivotFilterType.CaptionLessThanOrEqual, "captionLessThanOrEqual");
			result.Add(PivotFilterType.CaptionNotBeginsWith, "captionNotBeginsWith");
			result.Add(PivotFilterType.CaptionNotBetween, "captionNotBetween");
			result.Add(PivotFilterType.CaptionNotContains, "captionNotContains");
			result.Add(PivotFilterType.CaptionNotEndsWith, "captionNotEndsWith");
			result.Add(PivotFilterType.CaptionNotEqual, "captionNotEqual");
			result.Add(PivotFilterType.Count, "count");
			result.Add(PivotFilterType.DateBetween, "dateBetween");
			result.Add(PivotFilterType.DateEqual, "dateEqual");
			result.Add(PivotFilterType.DateNewerThan, "dateNewerThan");
			result.Add(PivotFilterType.DateNewerThanOrEqual, "dateNewerThanOrEqual");
			result.Add(PivotFilterType.DateNotBetween, "dateNotBetween");
			result.Add(PivotFilterType.DateNotEqual, "dateNotEqual");
			result.Add(PivotFilterType.DateOlderThan, "dateOlderThan");
			result.Add(PivotFilterType.DateOlderThanOrEqual, "dateOlderThanOrEqual");
			result.Add(PivotFilterType.LastMonth, "lastMonth");
			result.Add(PivotFilterType.LastQuarter, "lastQuarter");
			result.Add(PivotFilterType.LastWeek, "lastWeek");
			result.Add(PivotFilterType.LastYear, "lastYear");
			result.Add(PivotFilterType.January, "M1");
			result.Add(PivotFilterType.February, "M2");
			result.Add(PivotFilterType.March, "M3");
			result.Add(PivotFilterType.April, "M4");
			result.Add(PivotFilterType.May, "M5");
			result.Add(PivotFilterType.June, "M6");
			result.Add(PivotFilterType.July, "M7");
			result.Add(PivotFilterType.August, "M8");
			result.Add(PivotFilterType.September, "M9");
			result.Add(PivotFilterType.October, "M10");
			result.Add(PivotFilterType.November, "M11");
			result.Add(PivotFilterType.December, "M12");
			result.Add(PivotFilterType.NextMonth, "nextMonth");
			result.Add(PivotFilterType.NextQuarter, "nextQuarter");
			result.Add(PivotFilterType.NextWeek, "nextWeek");
			result.Add(PivotFilterType.NextYear, "nextYear");
			result.Add(PivotFilterType.Percent, "percent");
			result.Add(PivotFilterType.FirstQuarter, "Q1");
			result.Add(PivotFilterType.SecondQuarter, "Q2");
			result.Add(PivotFilterType.ThirdQuarter, "Q3");
			result.Add(PivotFilterType.FourthQuarter, "Q4");
			result.Add(PivotFilterType.Sum, "sum");
			result.Add(PivotFilterType.ThisMonth, "thisMonth");
			result.Add(PivotFilterType.ThisQuarter, "thisQuarter");
			result.Add(PivotFilterType.ThisWeek, "thisWeek");
			result.Add(PivotFilterType.ThisYear, "thisYear");
			result.Add(PivotFilterType.Today, "today");
			result.Add(PivotFilterType.Tomorrow, "tomorrow");
			result.Add(PivotFilterType.Unknown, "unknown");
			result.Add(PivotFilterType.ValueBetween, "valueBetween");
			result.Add(PivotFilterType.ValueEqual, "valueEqual");
			result.Add(PivotFilterType.ValueGreaterThan, "valueGreaterThan");
			result.Add(PivotFilterType.ValueGreaterThanOrEqual, "valueGreaterThanOrEqual");
			result.Add(PivotFilterType.ValueLessThan, "valueLessThan");
			result.Add(PivotFilterType.ValueLessThanOrEqual, "valueLessThanOrEqual");
			result.Add(PivotFilterType.ValueNotBetween, "valueNotBetween");
			result.Add(PivotFilterType.ValueNotEqual, "valueNotEqual");
			result.Add(PivotFilterType.YearToDate, "yearToDate");
			result.Add(PivotFilterType.Yesterday, "yesterday");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("autoFilter", OnAutoFilter);
			return result;
		}
		#endregion
		public PivotFilterDestination(SpreadsheetMLBaseImporter importer, PivotFilterCollection filters, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.filters = filters;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotFilter PivotFilter { get { return filter; } }
		public PivotFilterCollection Filters { get { return filters; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				filter = new PivotFilter(Worksheet.Workbook);
				Filters.AddCore(filter);
				PivotFilter.SetFieldIndexCore(Importer.GetWpSTIntegerValue(reader, "fld"));
				int value = Importer.GetWpSTIntegerValue(reader, "mpFld", -1);
				if (value >= 0)
					PivotFilter.SetMemberPropertyFieldIdCore(value);
				PivotFilter.SetFilterTypeCore((int)Importer.GetWpEnumValue<PivotFilterType>(reader, "type", reversePivotTablePivotFilterTypeTable, PivotFilterType.CaptionEqual));
				PivotFilter.SetEvalOrderCore(Importer.GetWpSTIntegerValue(reader, "evalOrder", 0));
				PivotFilter.SetPivotFilterIdCore(Importer.GetWpSTIntegerValue(reader, "id", 0));
				value = Importer.GetWpSTIntegerValue(reader, "iMeasureHier", -1);
				if (value >= 0)
					PivotFilter.SetMeasureIndexCore(value);
				value = Importer.GetWpSTIntegerValue(reader, "iMeasureFld", -1);
				if (value >= 0)
					PivotFilter.SetMeasureFieldIndexCore(value);
				PivotFilter.SetNameCore(Importer.GetWpSTXString(reader, "name"));
				PivotFilter.SetDescriptionCore(Importer.GetWpSTXString(reader, "description"));
				PivotFilter.SetLabelPivotCore(Importer.GetWpSTXString(reader, "stringValue1"));
				PivotFilter.SetLabelPivotFilterCore(Importer.GetWpSTXString(reader, "stringValue2"));
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (filter.FilterType == PivotFilterType.Sum) {
				foreach (AutoFilterColumn column in filter.AutoFilter.FilterColumns) {
					column.Top10FilterType = Top10FilterType.Sum;
				}
			}
		}
		static PivotFilterDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotFilterDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnAutoFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotFilterDestination self = GetThis(importer);
			return new AutoFilterDestination(importer, self.PivotFilter.AutoFilter, self.Worksheet);
		}
		#endregion
	}
	#endregion
}
