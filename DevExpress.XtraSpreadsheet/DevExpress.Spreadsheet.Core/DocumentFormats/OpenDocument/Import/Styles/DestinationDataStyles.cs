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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Export.OpenDocument;
using DevExpress.XtraSpreadsheet.Internal;
using System;
using System.Text;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region Base
	public abstract class ImportDataStyleElementDestinationBase : ElementDestination {
		#region Static
		protected static void AddCommonHandlers(ElementHandlerTable table) {
			table.Add("map", OnMap);
		}
		protected static ImportDataStyleElementDestinationBase GetThis(OpenDocumentWorkbookImporter importer) {
			return (ImportDataStyleElementDestinationBase)importer.PeekDestination();
		}
		#endregion
		string name;
		protected ImportDataStyleElementDestinationBase(OpenDocumentWorkbookImporter importer)
			: base(importer) {
			FormatStringBuilder = new StringBuilder();
			ConditionStringBuilder = new StringBuilder();
		}
		public StringBuilder ConditionStringBuilder { get; private set; }
		public StringBuilder FormatStringBuilder { get; protected set; }
		public override void ProcessElementOpen(XmlReader reader) {
			name = reader.GetAttribute("style:name");
		}
		public override void ProcessElementClose(XmlReader reader) {
			string formatString = ConditionStringBuilder.ToString() + FormatStringBuilder.ToString();
			if (FormatStringBuilder.Length < 1 && ConditionStringBuilder.Length > 0)
					formatString = formatString.Remove(formatString.Length - 1);
			Importer.RegisterFormatString(name, formatString);
		}
		#region Handlers
		static Destination OnMap(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder conditionStringBuilder = GetThis(importer).ConditionStringBuilder;
			return new DataStyleMapDestination(importer, conditionStringBuilder);
		}
		#endregion
	}
	public abstract class ImportDataStyleLeafElementDestinationBase : LeafElementDestination {
		protected ImportDataStyleLeafElementDestinationBase(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer) {
			this.FormatStringBuilder = formatStringBuilder;
		}
		protected StringBuilder FormatStringBuilder { get; set; }
	}
	#endregion
	#region BooleanDataStyle
	public class BooleanDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public BooleanDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		#endregion
	}
	#endregion
	#region CurrencyDataStyleDestination
	public class CurrencyDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("currency-symbol", OnCurrencySymbol);
			result.Add("number", OnNumber);
			result.Add("text", OnText);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public CurrencyDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnCurrencySymbol(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new CurrencySymbolDestination(importer, formatStringBuilder);
		}
		static Destination OnNumber(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new NumberDestination(importer, formatStringBuilder);
		}
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, false);
		}
		#endregion
	}
	public class CurrencySymbolDestination : ImportDataStyleLeafElementDestinationBase {
		string currencySymbol = string.Empty;
		public CurrencySymbolDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			currencySymbol = "$";
		}
		public override bool ProcessText(XmlReader reader) {
			currencySymbol = reader.Value;
			return true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			FormatStringBuilder.Append(currencySymbol);
		}
	}
	#endregion
	#region DateDataStyleDestination
	public class DateDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("am-pm", OnAmPm);
			result.Add("day", OnDay);
			result.Add("day-of-week", OnDayOfWeek);
			result.Add("era", OnEra);
			result.Add("hours", OnHours);
			result.Add("minutes", OnMinutes);
			result.Add("month", OnMonth);
			result.Add("quarter", OnQuarter);
			result.Add("seconds", OnSeconds);
			result.Add("week-of-year", OnWeekOfYear);
			result.Add("year", OnYear);
			result.Add("text", OnText);
			return result;
		}
		#endregion Static members
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public DateDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnAmPm(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new AmPmDestination(importer, formatStringBuilder);
		}
		static Destination OnDay(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new DayDestination(importer, formatStringBuilder);
		}
		static Destination OnDayOfWeek(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new DayOfWeekDestination(importer, formatStringBuilder);
		}
		static Destination OnEra(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new EraDestination(importer);
		}
		static Destination OnHours(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new HoursDestination(importer, formatStringBuilder);
		}
		static Destination OnMinutes(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new MinutesDestination(importer, formatStringBuilder);
		}
		static Destination OnMonth(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new MonthDestination(importer, formatStringBuilder);
		}
		static Destination OnQuarter(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new QuarterDestination(importer);
		}
		static Destination OnSeconds(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new SecondsDestination(importer, formatStringBuilder);
		}
		static Destination OnWeekOfYear(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new WeekOfYearDestination(importer);
		}
		static Destination OnYear(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new YearDestination(importer, formatStringBuilder);
		}
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, false);
		}
		#endregion
	}
	public class MonthDestination : ImportDataStyleLeafElementDestinationBase {
		public MonthDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string calendar = Importer.GetAttribute(reader, "number:calendar", "gregorian");
			bool isGregorian = calendar.Equals("gregorian", StringComparison.OrdinalIgnoreCase);
			if (!isGregorian)
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsCalendarIgnored);
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("mm");
			else
				FormatStringBuilder.Append("m");
			bool isTextual = Importer.GetWpSTOnOffValue(reader, "number:textual", false);
			if (isTextual)
				FormatStringBuilder.Append("mm");
		}
	}
	public class YearDestination : ImportDataStyleLeafElementDestinationBase {
		public YearDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string calendar = Importer.GetAttribute(reader, "number:calendar", "gregorian");
			bool isGregorian = calendar.Equals("gregorian", StringComparison.OrdinalIgnoreCase);
			if (!isGregorian)
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsCalendarIgnored);
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("yyyy");
			else
				FormatStringBuilder.Append("yy");
		}
	}
	public class DayDestination : ImportDataStyleLeafElementDestinationBase {
		public DayDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string calendar = Importer.GetAttribute(reader, "number:calendar", "gregorian");
			bool isGregorian = calendar.Equals("gregorian", StringComparison.OrdinalIgnoreCase);
			if (!isGregorian)
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsCalendarIgnored);
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("dd");
			else
				FormatStringBuilder.Append("d");
		}
	}
	public class DayOfWeekDestination : ImportDataStyleLeafElementDestinationBase {
		public DayOfWeekDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string calendar = Importer.GetAttribute(reader, "number:calendar", "gregorian");
			bool isGregorian = calendar.Equals("gregorian", StringComparison.OrdinalIgnoreCase);
			if (!isGregorian)
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsCalendarIgnored);
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("dddd");
			else
				FormatStringBuilder.Append("ddd");
		}
	}
	public class WeekOfYearDestination : LeafElementDestination {
		public WeekOfYearDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsUnknownDateFormat);
		}
	}
	public class QuarterDestination : LeafElementDestination {
		public QuarterDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsUnknownDateFormat);
		}
	}
	public class EraDestination : LeafElementDestination {
		public EraDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
			Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsUnknownDateFormat);
		}
	}
	public class HoursDestination : ImportDataStyleLeafElementDestinationBase {
		public HoursDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("hh");
			else
				FormatStringBuilder.Append("h");
		}
	}
	public class MinutesDestination : ImportDataStyleLeafElementDestinationBase {
		public MinutesDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("mm");
			else
				FormatStringBuilder.Append("m");
		}
	}
	public class SecondsDestination : ImportDataStyleLeafElementDestinationBase {
		public SecondsDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string style = Importer.GetAttribute(reader, "number:style", "short");
			bool isLong = style.Equals("long", StringComparison.OrdinalIgnoreCase);
			if (isLong)
				FormatStringBuilder.Append("ss");
			else
				FormatStringBuilder.Append("s");
			int decimalPlaces = Importer.GetWpSTIntegerValue(reader, "number:decimal-places", 0);
			if (decimalPlaces > 0) {
				FormatStringBuilder.Append(".");
				FormatStringBuilder.Append('0', decimalPlaces);
			}
		}
	}
	public class AmPmDestination : ImportDataStyleLeafElementDestinationBase {
		public AmPmDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FormatStringBuilder.Append("AM/PM");
		}
	}
	#endregion
	#region NumberDataStyleDestination
	public class NumberDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("fraction", OnFraction);
			result.Add("scientific-number", OnScientific);
			result.Add("number", OnNumber);
			result.Add("text", OnText);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public NumberDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnFraction(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new FractionDestination(importer, formatStringBuilder);
		}
		static Destination OnScientific(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new ScientificDestination(importer, formatStringBuilder);
		}
		static Destination OnNumber(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new NumberDestination(importer, formatStringBuilder);
		}
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, false);
		}
		#endregion
	}
	public class FractionDestination : ImportDataStyleLeafElementDestinationBase {
		public FractionDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool isGrouing = Importer.GetWpSTOnOffValue(reader, "number:grouping", false);
			if (isGrouing)
				FormatStringBuilder.Append("#,");
			int minIntegerDigits = Importer.GetWpSTIntegerValue(reader, "number:min-integer-digits", -1);
			if (minIntegerDigits >= 0) {
				if (minIntegerDigits == 0)
					FormatStringBuilder.Append("#");
				else
					FormatStringBuilder.Append('0', minIntegerDigits);
				FormatStringBuilder.Append(' ');
			}
			int minNumeratorDigits = Importer.GetWpSTIntegerValue(reader, "number:min-numerator-digits");
			int minDenominatorDigits = Importer.GetWpSTIntegerValue(reader, "number:min-denominator-digits");
			FormatStringBuilder.Append('?', minNumeratorDigits);
			FormatStringBuilder.Append('/');
			FormatStringBuilder.Append('?', minDenominatorDigits);
		}
	}
	public class ScientificDestination : ImportDataStyleLeafElementDestinationBase {
		public ScientificDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool isGrouping = Importer.GetWpSTOnOffValue(reader, "number:grouping", false);
			if (isGrouping)
				FormatStringBuilder.Append("#,");
			int minIntegerDigits = Importer.GetWpSTIntegerValue(reader, "number:min-integer-digits", 0);
			if (minIntegerDigits == 0)
				FormatStringBuilder.Append("#");
			else
				if (minIntegerDigits > 0)
					FormatStringBuilder.Append('0', minIntegerDigits);
			int decimalPlaces = Importer.GetWpSTIntegerValue(reader, "number:decimal-places", 0);
			if (decimalPlaces > 0) {
				FormatStringBuilder.Append(".");
				FormatStringBuilder.Append('0', decimalPlaces);
			}
			int minExponentDigits = Importer.GetWpSTIntegerValue(reader, "number:min-exponent-digits", 1);
			FormatStringBuilder.Append("E+");
			FormatStringBuilder.Append('0', minExponentDigits);
		}
	}
	#endregion
	#region PercentageDataStyleDestination
	public class PercentageDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("number", OnNumber);
			result.Add("text", OnText);
			return result;
		}
		#endregion Static members
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public PercentageDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnNumber(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new NumberDestination(importer, formatStringBuilder);
		}
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, true);
		}
		#endregion
	}
	#endregion
	#region TextDataStyleDestination
	public class TextDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("text", OnText);
			result.Add("text-content", OnTextContent);
			return result;
		}
		#endregion Static members
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TextDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, false);
		}
		static Destination OnTextContent(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextContentDestination(importer, formatStringBuilder);
		}
		#endregion
	}
	public class TextContentDestination : ImportDataStyleLeafElementDestinationBase {
		public TextContentDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FormatStringBuilder.Append("@");
		}
	}
	#endregion
	#region TimeDataStyleDestination
	public class TimeDataStyleDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("am-pm", OnAmPm);
			result.Add("hours", OnHours);
			result.Add("minutes", OnMinutes);
			result.Add("seconds", OnSeconds);
			result.Add("text", OnText);
			return result;
		}
		#endregion Static members
		bool truncateOverflow;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TimeDataStyleDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			truncateOverflow = Importer.GetWpSTOnOffValue(reader, "number:truncate-on-overflow", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			string formatString = FormatStringBuilder.ToString();
			if (!truncateOverflow) {
				if (formatString.Contains("h"))
					formatString = formatString.Insert(formatString.LastIndexOf('h') + 1, "]").Insert(formatString.IndexOf('h'), "[");
				else
					if (formatString.Contains("m"))
						formatString = formatString.Insert(formatString.LastIndexOf('m') + 1, "]").Insert(formatString.IndexOf('m'), "[");
					else
						if (formatString.Contains("s"))
							formatString = formatString.Insert(formatString.LastIndexOf('s') + 1, "]").Insert(formatString.IndexOf('s'), "[");
			}
			FormatStringBuilder = new StringBuilder(formatString);
			base.ProcessElementClose(reader);
		}
		#region Handlers
		static Destination OnAmPm(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new AmPmDestination(importer, formatStringBuilder);
		}
		static Destination OnHours(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new HoursDestination(importer, formatStringBuilder);
		}
		static Destination OnMinutes(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new MinutesDestination(importer, formatStringBuilder);
		}
		static Destination OnSeconds(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new SecondsDestination(importer, formatStringBuilder);
		}
		static Destination OnText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new TextDestination(importer, formatStringBuilder, false);
		}
		#endregion
	}
	#endregion
	#region Common
	public class NumberDestination : ImportDataStyleElementDestinationBase {
		#region Static members
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("embedded-text", OnEmbeddedText);
			return result;
		}
		#endregion Static members
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public NumberDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer) {
			this.FormatStringBuilder = formatStringBuilder;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool isGrouping = Importer.GetWpSTOnOffValue(reader, "number:grouping", false);
			if (isGrouping)
				FormatStringBuilder.Append("#,");
			int minIntegerDigits = Importer.GetWpSTIntegerValue(reader, "number:min-integer-digits", 0);
			if (minIntegerDigits == 0)
				FormatStringBuilder.Append("#");
			else
				if (minIntegerDigits > 0)
					FormatStringBuilder.Append('0', minIntegerDigits);
			int decimalPlaces = Importer.GetWpSTIntegerValue(reader, "number:decimal-places", 0);
			if (decimalPlaces > 0) {
				string decimalReplacement = Importer.GetAttribute(reader, "number:decimal-replacement", "0");
				if (string.IsNullOrEmpty(decimalReplacement))
					decimalReplacement = "#";
				FormatStringBuilder.Append('.');
				for (int i = 0; i < decimalPlaces; i++) {
					FormatStringBuilder.Append(decimalReplacement);
				}
			}
			int displayFactor = Importer.GetWpSTIntegerValue(reader, "number:display-factor", 1);
			while (displayFactor > 1) {
				displayFactor /= 1000;
				FormatStringBuilder.Append(',');
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		#region Handlers
		static Destination OnEmbeddedText(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			StringBuilder formatStringBuilder = GetThis(importer).FormatStringBuilder;
			return new EmbeddedTextDestination(importer, formatStringBuilder);
		}
		#endregion
	}
	public class EmbeddedTextDestination : ImportDataStyleLeafElementDestinationBase {
		#region Fields
		int position;
		string text;
		#endregion
		public EmbeddedTextDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder)
			: base(importer, formatStringBuilder) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			position = Importer.GetWpSTIntegerValue(reader, "number:position", 0);
		}
		public override bool ShouldProcessWhitespaces(XmlReader reader) {
			return true;
		}
		public override bool ProcessText(XmlReader reader) {
			text = reader.Value;
			text = Importer.NormalizeNumberFormatText(text, false);
			return true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			string formatString = FormatStringBuilder.ToString();
			int pastePos = OpenDocumentExporter.GetNonTextSymbolPosition(formatString, '.', true);
			if (pastePos < 0)
				pastePos = formatString.Length - 1;
			else
				--pastePos;
			int digitCount = 0;
			while (digitCount < position && pastePos >= 0) {
				if (OpenDocumentExporter.IsNumericSymbol(formatString[pastePos]))
					++digitCount;
				--pastePos;
			}
			if (pastePos < 0 && digitCount != position)
				pastePos = formatString.Length - 1;
			FormatStringBuilder.Insert(pastePos + 1, text);
		}
	}
	public class TextDestination : ImportDataStyleLeafElementDestinationBase {
		bool isPercentage;
		public TextDestination(OpenDocumentWorkbookImporter importer, StringBuilder formatStringBuilder, bool isPercentage)
			: base(importer, formatStringBuilder) {
			this.isPercentage = isPercentage;
		}
		public override bool ShouldProcessWhitespaces(XmlReader reader) {
			return true;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			text = Importer.NormalizeNumberFormatText(text, isPercentage);
			FormatStringBuilder.Append(text);
			return true;
		}
	}
	#endregion
}
#endif
