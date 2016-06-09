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
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Export.Xl;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport.Xlsx {
	#region XlsxDataAwareExporter
	partial class XlsxDataAwareExporter {
		#region Statics
		static Dictionary<XlCondFmtType, string> conditionalFormattingTypeTable = CreateConditionalFormattingTypeTable();
		static Dictionary<XlCondFmtType, string> CreateConditionalFormattingTypeTable() {
			Dictionary<XlCondFmtType, string> result = new Dictionary<XlCondFmtType, string>();
			result.Add(XlCondFmtType.AboveOrBelowAverage, "aboveAverage");
			result.Add(XlCondFmtType.BeginsWith, "beginsWith");
			result.Add(XlCondFmtType.CellIs, "cellIs");
			result.Add(XlCondFmtType.ColorScale, "colorScale");
			result.Add(XlCondFmtType.ContainsBlanks, "containsBlanks");
			result.Add(XlCondFmtType.ContainsErrors, "containsErrors");
			result.Add(XlCondFmtType.ContainsText, "containsText");
			result.Add(XlCondFmtType.DataBar, "dataBar");
			result.Add(XlCondFmtType.DuplicateValues, "duplicateValues");
			result.Add(XlCondFmtType.EndsWith, "endsWith");
			result.Add(XlCondFmtType.Expression, "expression");
			result.Add(XlCondFmtType.IconSet, "iconSet");
			result.Add(XlCondFmtType.NotContainsBlanks, "notContainsBlanks");
			result.Add(XlCondFmtType.NotContainsErrors, "notContainsErrors");
			result.Add(XlCondFmtType.NotContainsText, "notContainsText");
			result.Add(XlCondFmtType.TimePeriod, "timePeriod");
			result.Add(XlCondFmtType.Top10, "top10");
			result.Add(XlCondFmtType.UniqueValues, "uniqueValues");
			return result;
		}
		static Dictionary<XlCondFmtOperator, string> conditionalFormattingOperatorTable = CreateConditionalFormattingOperatorTable();
		static Dictionary<XlCondFmtOperator, string> CreateConditionalFormattingOperatorTable() {
			Dictionary<XlCondFmtOperator, string> result = new Dictionary<XlCondFmtOperator, string>();
			result.Add(XlCondFmtOperator.BeginsWith, "beginsWith");
			result.Add(XlCondFmtOperator.Between, "between");
			result.Add(XlCondFmtOperator.ContainsText, "containsText");
			result.Add(XlCondFmtOperator.EndsWith, "endsWith");
			result.Add(XlCondFmtOperator.Equal, "equal");
			result.Add(XlCondFmtOperator.GreaterThan, "greaterThan");
			result.Add(XlCondFmtOperator.GreaterThanOrEqual, "greaterThanOrEqual");
			result.Add(XlCondFmtOperator.LessThan, "lessThan");
			result.Add(XlCondFmtOperator.LessThanOrEqual, "lessThanOrEqual");
			result.Add(XlCondFmtOperator.NotBetween, "notBetween");
			result.Add(XlCondFmtOperator.NotContains, "notContains");
			result.Add(XlCondFmtOperator.NotEqual, "notEqual");
			return result;
		}
		static Dictionary<XlCondFmtValueObjectType, string> conditionalFormattingValueTypeTable = CreateConditionalFormattingValueTypeTable();
		static Dictionary<XlCondFmtValueObjectType, string> CreateConditionalFormattingValueTypeTable() {
			Dictionary<XlCondFmtValueObjectType, string> result = new Dictionary<XlCondFmtValueObjectType,string>();
			result.Add(XlCondFmtValueObjectType.Number, "num");
			result.Add(XlCondFmtValueObjectType.Percent, "percent");
			result.Add(XlCondFmtValueObjectType.Max, "max");
			result.Add(XlCondFmtValueObjectType.Min, "min");
			result.Add(XlCondFmtValueObjectType.Formula, "formula");
			result.Add(XlCondFmtValueObjectType.Percentile, "percentile");
			result.Add(XlCondFmtValueObjectType.AutoMax, "autoMax");
			result.Add(XlCondFmtValueObjectType.AutoMin, "autoMin");
			return result;
		}
		static Dictionary<XlCondFmtIconSetType, string> iconSetTypeTable = CreateIconSetTypeTable();
		static Dictionary<XlCondFmtIconSetType, string> CreateIconSetTypeTable() {
			Dictionary<XlCondFmtIconSetType, string> result = new Dictionary<XlCondFmtIconSetType, string>();
			result.Add(XlCondFmtIconSetType.Arrows3, "3Arrows");
			result.Add(XlCondFmtIconSetType.ArrowsGray3, "3ArrowsGray");
			result.Add(XlCondFmtIconSetType.Flags3, "3Flags");
			result.Add(XlCondFmtIconSetType.TrafficLights3, "3TrafficLights1");
			result.Add(XlCondFmtIconSetType.TrafficLights3Black, "3TrafficLights2");
			result.Add(XlCondFmtIconSetType.Signs3, "3Signs");
			result.Add(XlCondFmtIconSetType.Symbols3, "3Symbols");
			result.Add(XlCondFmtIconSetType.Symbols3Circled, "3Symbols2");
			result.Add(XlCondFmtIconSetType.Stars3, "3Stars");
			result.Add(XlCondFmtIconSetType.Triangles3, "3Triangles");
			result.Add(XlCondFmtIconSetType.Arrows4, "4Arrows");
			result.Add(XlCondFmtIconSetType.ArrowsGray4, "4ArrowsGray");
			result.Add(XlCondFmtIconSetType.RedToBlack4, "4RedToBlack");
			result.Add(XlCondFmtIconSetType.Rating4, "4Rating");
			result.Add(XlCondFmtIconSetType.TrafficLights4, "4TrafficLights");
			result.Add(XlCondFmtIconSetType.Arrows5, "5Arrows");
			result.Add(XlCondFmtIconSetType.ArrowsGray5, "5ArrowsGray");
			result.Add(XlCondFmtIconSetType.Rating5, "5Rating");
			result.Add(XlCondFmtIconSetType.Quarters5, "5Quarters");
			result.Add(XlCondFmtIconSetType.Boxes5, "5Boxes");
			return result;
		}
		static Dictionary<XlCondFmtTimePeriod, string> timePeriodTable = CreateTimePeriodTable();
		static Dictionary<XlCondFmtTimePeriod, string> CreateTimePeriodTable() {
			Dictionary<XlCondFmtTimePeriod, string> result = new Dictionary<XlCondFmtTimePeriod, string>();
			result.Add(XlCondFmtTimePeriod.Last7Days, "last7Days");
			result.Add(XlCondFmtTimePeriod.LastMonth, "lastMonth");
			result.Add(XlCondFmtTimePeriod.LastWeek, "lastWeek");
			result.Add(XlCondFmtTimePeriod.NextMonth, "nextMonth");
			result.Add(XlCondFmtTimePeriod.NextWeek, "nextWeek");
			result.Add(XlCondFmtTimePeriod.ThisMonth, "thisMonth");
			result.Add(XlCondFmtTimePeriod.ThisWeek, "thisWeek");
			result.Add(XlCondFmtTimePeriod.Today, "today");
			result.Add(XlCondFmtTimePeriod.Tomorrow, "tomorrow");
			result.Add(XlCondFmtTimePeriod.Yesterday, "yesterday");
			return result;
		}
		#endregion
		const string condFmtExtRefUri = "{B025F937-C7B1-47D3-B67F-A62EFF666E3E}";
		const string condFmtExtUri = "{78C0D931-6437-407d-A8EE-F0AAD7539E65}";
		const string x14NamespaceReference = @"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
		const string xmNamespaceReference = @"http://schemas.microsoft.com/office/excel/2006/main";
		void GenerateConditionalFormattings(IList<XlConditionalFormatting> conditionalFormattings) {
			foreach(XlConditionalFormatting item in conditionalFormattings)
				GenerateConditionalFormatting(item);
		}
		bool ShouldExportConditionalFormatting(XlConditionalFormatting conditionalFormatting) {
			if(conditionalFormatting.Ranges.Count == 0)
				return false;
			foreach(XlCondFmtRule rule in conditionalFormatting.Rules) {
				if(rule.RuleType != XlCondFmtType.IconSet)
					return true;
			}
			return false;
		}
		void GenerateConditionalFormatting(XlConditionalFormatting conditionalFormatting) {
			if(!ShouldExportConditionalFormatting(conditionalFormatting))
				return;
			WriteShStartElement("conditionalFormatting");
			try {
				WriteStringValue("sqref", GetSqRef(conditionalFormatting.Ranges));
				foreach(XlCondFmtRule rule in conditionalFormatting.Rules)
					GenerateConditionalFormattingRule(conditionalFormatting, rule);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateConditionalFormattingRule(XlConditionalFormatting conditionalFormatting, XlCondFmtRule rule) {
			if(rule.RuleType == XlCondFmtType.IconSet)
				return;
			WriteShStartElement("cfRule");
			try {
				WriteIntValue("priority", rule.Priority);
				if(rule.StopIfTrue)
					WriteBoolValue("stopIfTrue", rule.StopIfTrue);
				WriteDxfId(rule as XlCondFmtRuleWithFormatting);
				WriteStringValue("type", conditionalFormattingTypeTable[rule.RuleType]);
				switch(rule.RuleType) {
					case XlCondFmtType.AboveOrBelowAverage:
						GenerateCondFmtRuleAboveAverage(rule as XlCondFmtRuleAboveAverage);
						break;
					case XlCondFmtType.CellIs:
						GenerateCondFmtRuleCellIs(conditionalFormatting, rule as XlCondFmtRuleCellIs);
						break;
					case XlCondFmtType.Expression:
						GenerateCondFmtRuleExpression(conditionalFormatting, rule as XlCondFmtRuleExpression);
						break;
					case XlCondFmtType.ContainsBlanks:
					case XlCondFmtType.NotContainsBlanks:
						GenerateCondFmtRuleBlanks(conditionalFormatting, rule as XlCondFmtRuleBlanks);
						break;
					case XlCondFmtType.ContainsText:
					case XlCondFmtType.NotContainsText:
					case XlCondFmtType.BeginsWith:
					case XlCondFmtType.EndsWith:
						GenerateCondFmtRuleSpecificText(conditionalFormatting, rule as XlCondFmtRuleSpecificText);
						break;
					case XlCondFmtType.DataBar:
						GenerateCondFmtRuleDataBar(conditionalFormatting, rule as XlCondFmtRuleDataBar);
						break;
					case XlCondFmtType.ColorScale:
						GenerateCondFmtRuleColorScale(conditionalFormatting, rule as XlCondFmtRuleColorScale);
						break;
					case XlCondFmtType.Top10:
						GenerateCondFmtRuleTop10(rule as XlCondFmtRuleTop10);
						break;
					case XlCondFmtType.TimePeriod:
						GenerateCondFmtRuleTimePeriod(conditionalFormatting, rule as XlCondFmtRuleTimePeriod);
						break;
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteDxfId(XlCondFmtRuleWithFormatting rule) {
			if(rule == null)
				return;
			int dxfId = RegisterCondFmtDifferentialFormatting(rule.Formatting);
			if (dxfId >= 0)
				WriteIntValue("dxfId", dxfId);
		}
		void GenerateCondFmtRuleAboveAverage(XlCondFmtRuleAboveAverage rule) {
			if(!rule.AboveAverage)
				WriteBoolValue("aboveAverage", rule.AboveAverage);
			if(rule.EqualAverage)
				WriteBoolValue("equalAverage", rule.EqualAverage);
			if(rule.StdDev > 0)
				WriteIntValue("stdDev", rule.StdDev);
		}
		void GenerateCondFmtRuleTop10(XlCondFmtRuleTop10 rule) {
			if(rule.Bottom)
				WriteBoolValue("bottom", rule.Bottom);
			if(rule.Percent)
				WriteBoolValue("percent", rule.Percent);
			WriteIntValue("rank", rule.Rank);
		}
		void GenerateCondFmtRuleCellIs(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleCellIs rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			WriteStringValue("operator", conditionalFormattingOperatorTable[rule.Operator]);
			WriteCondFmtValue("formula", rule.Value, topLeft);
			if (rule.Operator == XlCondFmtOperator.Between || rule.Operator == XlCondFmtOperator.NotBetween)
				WriteCondFmtValue("formula", rule.SecondValue, topLeft);
		}
		void GenerateCondFmtRuleBlanks(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleBlanks rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			string formula;
			if(rule.RuleType == XlCondFmtType.ContainsBlanks)
				formula = string.Format("LEN(TRIM({0}))=0", topLeft.ToString());
			else
				formula = string.Format("LEN(TRIM({0}))>0", topLeft.ToString());
			WriteShString("formula", formula, true);
		}
		void GenerateCondFmtRuleExpression(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleExpression rule) {
			expressionContext.CurrentCell = conditionalFormatting.GetTopLeftCell();
			expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
			expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
			if(rule.Expression != null) {
				string formula = rule.Expression.ToString(expressionContext);
				WriteShString("formula", formula, true);
			}
			else if(!string.IsNullOrEmpty(rule.Formula)) {
				if(formulaParser != null) {
					XlExpression expression = formulaParser.Parse(rule.Formula, expressionContext);
					if(expression == null)
						throw new InvalidOperationException(string.Format("Can't parse rule formula '{0}'.", rule.Formula));
				}
				WriteShString("formula", rule.Formula, true);
			}
		}
		void GenerateCondFmtRuleSpecificText(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleSpecificText rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			string formula;
			if(rule.RuleType == XlCondFmtType.ContainsText) {
				WriteStringValue("operator", conditionalFormattingOperatorTable[XlCondFmtOperator.ContainsText]);
				formula = string.Format("NOT(ISERROR(SEARCH(\"{0}\",{1})))", rule.Text, topLeft.ToString());
			}
			else if(rule.RuleType == XlCondFmtType.NotContainsText) {
				WriteStringValue("operator", conditionalFormattingOperatorTable[XlCondFmtOperator.NotContains]);
				formula = string.Format("ISERROR(SEARCH(\"{0}\",{1}))", rule.Text, topLeft.ToString());
			}
			else if(rule.RuleType == XlCondFmtType.BeginsWith) {
				WriteStringValue("operator", conditionalFormattingOperatorTable[XlCondFmtOperator.BeginsWith]);
				formula = string.Format("LEFT({1},LEN(\"{0}\"))=\"{0}\"", rule.Text, topLeft.ToString());
			}
			else { 
				WriteStringValue("operator", conditionalFormattingOperatorTable[XlCondFmtOperator.EndsWith]);
				formula = string.Format("RIGHT({1},LEN(\"{0}\"))=\"{0}\"", rule.Text, topLeft.ToString());
			}
			WriteStringValue("text", rule.Text);
			WriteShString("formula", EncodeXmlChars(formula), true);
		}
		void GenerateCondFmtRuleTimePeriod(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleTimePeriod rule) {
			WriteStringValue("timePeriod", timePeriodTable[rule.TimePeriod]);
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			string formula = GetTimePeriodFormula(rule.TimePeriod, topLeft);
			WriteShString("formula", EncodeXmlChars(formula), true);
		}
		string GetTimePeriodFormula(XlCondFmtTimePeriod timePeriod, XlCellPosition topLeft) {
			switch(timePeriod) {
				case XlCondFmtTimePeriod.Last7Days:
					return string.Format("AND(TODAY()-FLOOR({0},1)<=6,FLOOR({0},1)<=TODAY())", topLeft.ToString());
				case XlCondFmtTimePeriod.LastMonth:
					return string.Format("AND(MONTH({0})=MONTH(EDATE(TODAY(),0-1)),YEAR({0})=YEAR(EDATE(TODAY(),0-1)))", topLeft.ToString());
				case XlCondFmtTimePeriod.LastWeek:
					return string.Format("AND(TODAY()-ROUNDDOWN({0},0)>=(WEEKDAY(TODAY())),TODAY()-ROUNDDOWN({0},0)<(WEEKDAY(TODAY())+7))", topLeft.ToString());
				case XlCondFmtTimePeriod.NextMonth:
					return string.Format("AND(MONTH({0})=MONTH(EDATE(TODAY(),0+1)),YEAR({0})=YEAR(EDATE(TODAY(),0+1)))", topLeft.ToString());
				case XlCondFmtTimePeriod.NextWeek:
					return string.Format("AND(ROUNDDOWN({0},0)-TODAY()>(7-WEEKDAY(TODAY())),ROUNDDOWN({0},0)-TODAY()<(15-WEEKDAY(TODAY())))", topLeft.ToString());
				case XlCondFmtTimePeriod.ThisMonth:
					return string.Format("AND(MONTH({0})=MONTH(TODAY()),YEAR({0})=YEAR(TODAY()))", topLeft.ToString());
				case XlCondFmtTimePeriod.ThisWeek:
					return string.Format("AND(TODAY()-ROUNDDOWN({0},0)<=WEEKDAY(TODAY())-1,ROUNDDOWN({0},0)-TODAY()<=7-WEEKDAY(TODAY()))", topLeft.ToString());
				case XlCondFmtTimePeriod.Today:
					return string.Format("FLOOR({0},1)=TODAY()", topLeft.ToString());
				case XlCondFmtTimePeriod.Tomorrow:
					return string.Format("FLOOR({0},1)=TODAY()+1", topLeft.ToString());
				case XlCondFmtTimePeriod.Yesterday:
				default:
					return string.Format("FLOOR(anchorCell,1)=TODAY()-1", topLeft.ToString());
			}
		}
		void GenerateCondFmtRuleDataBar(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleDataBar rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			WriteShStartElement("dataBar");
			try {
				if(!rule.ShowValues)
					WriteBoolValue("showValue", rule.ShowValues);
				WriteCfvo(rule.MinValue, topLeft);
				WriteCfvo(rule.MaxValue, topLeft);
				WriteColor(rule.FillColor, "color");
			}
			finally {
				WriteShEndElement();
			}
			WriteShStartElement("extLst");
			try {
				WriteExtListReference(rule.GetRuleId());
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteExtListReference(string id) {
			WriteShStartElement("ext");
			try {
				WriteStringValue("uri", condFmtExtRefUri);
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteString("id", x14NamespaceReference, id);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateExtList(IXlSheet sheet) {
			bool shouldExportCondFmtExt = ShouldExportCondFmtExtList(sheet.ConditionalFormattings);
			bool shouldExportDataValidationsExt = ShouldExportDataValidationsExt(sheet.DataValidations);
			bool shouldExportSparklineGroupsExt = ShouldExportSparklineGroupsExt(sheet.SparklineGroups);
			if(!shouldExportCondFmtExt && !shouldExportDataValidationsExt && !shouldExportSparklineGroupsExt)
				return;
			WriteShStartElement("extLst");
			try {
				if(shouldExportCondFmtExt)
					GenerateConditionalFormattingsExt(sheet.ConditionalFormattings);
				if(shouldExportDataValidationsExt)
					GenerateDataValidationsExt(sheet.DataValidations);
				if(shouldExportSparklineGroupsExt)
					GenerateSparklineGroupsExt(sheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateConditionalFormattingsExt(IList<XlConditionalFormatting> conditionalFormattings) {
			WriteShStartElement("ext");
			try {
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteStringValue("uri", condFmtExtUri);
				WriteStartElement("x14", "conditionalFormattings", null);
				try {
					foreach(XlConditionalFormatting item in conditionalFormattings)
						GenerateConditionalFormattingExt(item);
				}
				finally {
					WriteEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldExportCondFmtExtList(IList<XlConditionalFormatting> conditionalFormattings) {
			foreach(XlConditionalFormatting item in conditionalFormattings) {
				if(ShouldExportCondFmtExt(item))
					return true;
			}
			return false;
		}
		bool ShouldExportCondFmtExt(XlConditionalFormatting conditionalFormatting) {
			if(conditionalFormatting.Ranges.Count > 0) {
				foreach(XlCondFmtRule rule in conditionalFormatting.Rules) {
					if(rule.RuleType == XlCondFmtType.DataBar || rule.RuleType == XlCondFmtType.IconSet)
						return true;
				}
			}
			return false;
		}
		void GenerateConditionalFormattingExt(XlConditionalFormatting conditionalFormatting) {
			if(!ShouldExportCondFmtExt(conditionalFormatting))
				return;
			WriteStartElement("conditionalFormatting", x14NamespaceReference);
			WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
			try {
				foreach(XlCondFmtRule rule in conditionalFormatting.Rules) {
					if (rule.RuleType == XlCondFmtType.DataBar)
						WriteCondFmtRuleDataBarExt(conditionalFormatting, rule as XlCondFmtRuleDataBar);
					if(rule.RuleType == XlCondFmtType.IconSet)
						WriteCondFmtRuleIconSetExt(conditionalFormatting, rule as XlCondFmtRuleIconSet);
				}
				WriteString("sqref", xmNamespaceReference, GetSqRef(conditionalFormatting.Ranges));
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteCondFmtRuleDataBarExt(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleDataBar rule) {
			WriteStartElement("cfRule", x14NamespaceReference);
			try {
				WriteStringValue("id", rule.GetRuleId());
				WriteCondFmtDataBarExt(conditionalFormatting, rule);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteCondFmtDataBarExt(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleDataBar rule) {
			WriteStringValue("type", "dataBar");
			WriteStartElement("dataBar", x14NamespaceReference);
			try {
				if(rule.MinLength != 10)
					WriteIntValue("minLength", rule.MinLength);
				if(rule.MaxLength != 90)
					WriteIntValue("maxLength", rule.MaxLength);
				if(!rule.GradientFill)
					WriteBoolValue("gradient", false);
				bool hasBorder = !rule.BorderColor.IsEmpty;
				bool negativeBarColorSameAsPositive = rule.FillColor.Equals(rule.NegativeFillColor);
				bool negativeBarBorderColorSameAsPositive = rule.BorderColor.Equals(rule.NegativeBorderColor);
				if(negativeBarColorSameAsPositive)
					WriteBoolValue("negativeBarColorSameAsPositive", true);
				if(hasBorder) {
					WriteBoolValue("border", true);
					if(!negativeBarBorderColorSameAsPositive)
						WriteBoolValue("negativeBarBorderColorSameAsPositive", false);
				}
				if(rule.AxisPosition  == XlCondFmtAxisPosition.None)
					WriteStringValue("axisPosition", "none");
				else if (rule.AxisPosition == XlCondFmtAxisPosition.Midpoint)
					WriteStringValue("axisPosition", "middle");
				if (rule.Direction == XlDataBarDirection.LeftToRight)
					WriteStringValue("direction", "leftToRight");
				else if (rule.Direction == XlDataBarDirection.RightToLeft)
					WriteStringValue("direction", "rightToLeft");
				XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
				WriteCfvoExt(rule.MinValue, topLeft);
				WriteCfvoExt(rule.MaxValue, topLeft);
				if(hasBorder)
					WriteColor(rule.BorderColor, "borderColor", x14NamespaceReference);
				if(!negativeBarColorSameAsPositive)
					WriteColor(GetNegativeFillColor(rule), "negativeFillColor", x14NamespaceReference);
				if(hasBorder && !negativeBarBorderColorSameAsPositive) {
					XlColor negativeBorderColor = rule.NegativeBorderColor;
					if(negativeBorderColor.IsEmpty)
						negativeBorderColor = negativeBarColorSameAsPositive ? rule.BorderColor : GetNegativeFillColor(rule);
					WriteColor(negativeBorderColor, "negativeBorderColor", x14NamespaceReference);
				}
				if(rule.AxisPosition != XlCondFmtAxisPosition.None) {
					XlColor axisColor = rule.AxisColor;
					if(axisColor.IsEmpty)
						axisColor = DXColor.Black;
					WriteColor(axisColor, "axisColor", x14NamespaceReference);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		XlColor GetNegativeFillColor(XlCondFmtRuleDataBar rule) {
			XlColor negativeFillColor = rule.NegativeFillColor;
			if(negativeFillColor.IsAutoOrEmpty)
				negativeFillColor = DXColor.Red;
			return negativeFillColor;
		}
		void WriteCondFmtValue(string tag, XlValueObject value, XlCellPosition topLeft) {
			if(value.IsEmpty)
				return;
			if(value.IsRange)
				WriteShString(tag, value.RangeValue.ToString(), true);
			else if(value.IsExpression) {
				expressionContext.CurrentCell = topLeft;
				expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
				expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
				string formula = value.Expression.ToString(expressionContext);
				WriteShString(tag, EncodeXmlChars(formula), true);
			}
			else if(value.IsFormula) {
				string formula = value.Formula.Remove(0, 1);
				if(formulaParser != null) {
					expressionContext.CurrentCell = topLeft;
					expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
					expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
					XlExpression expression = formulaParser.Parse(formula, expressionContext);
					if(expression == null || expression.Count == 0)
						formula = "#VALUE!";
				}
				WriteShString(tag, EncodeXmlChars(formula), true);
			}
			else {
				XlVariantValue variantValue = value.VariantValue;
				string text;
				if(variantValue.IsText)
					text = string.Format("\"{0}\"", variantValue.TextValue);
				else
					text = variantValue.ToText().TextValue;
				WriteShString(tag, EncodeXmlChars(text), true);
			}
		}
		void WriteCfvo(XlCondFmtValueObject value, XlCellPosition topLeft) {
			WriteShStartElement("cfvo");
			try {
				XlCondFmtValueObjectType objectType = value.ObjectType;
				if(objectType == XlCondFmtValueObjectType.AutoMin)
					objectType = XlCondFmtValueObjectType.Min;
				if(objectType == XlCondFmtValueObjectType.AutoMax)
					objectType = XlCondFmtValueObjectType.Max;
				WriteStringValue("type", conditionalFormattingValueTypeTable[objectType]);
				if(objectType != XlCondFmtValueObjectType.Min && objectType != XlCondFmtValueObjectType.Max) {
					XlValueObject valueObject = value.Value;
					if(!valueObject.IsEmpty)
						WriteStringValue("val", EncodeXmlChars(GetValueObjectString(valueObject, topLeft)));
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		string GetValueObjectString(XlValueObject valueObject, XlCellPosition topLeft) {
			if(valueObject.IsExpression) {
				expressionContext.CurrentCell = topLeft;
				expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
				expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
				return valueObject.Expression.ToString(expressionContext);
			}
			if(valueObject.IsFormula) {
				string formula = valueObject.Formula.Remove(0, 1);
				if(formulaParser != null) {
					expressionContext.CurrentCell = topLeft;
					expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
					expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
					XlExpression expression = formulaParser.Parse(formula, expressionContext);
					if(expression == null || expression.Count == 0)
						return "#VALUE!";
				}
				return formula;
			}
			return valueObject.ToString();
		}
		void WriteCfvoExt(XlCondFmtValueObject value, XlCellPosition topLeft) {
			WriteStartElement("x14", "cfvo", null);
			try {
				XlCondFmtValueObjectType objectType = value.ObjectType;
				WriteStringValue("type", conditionalFormattingValueTypeTable[objectType]);
				if(objectType != XlCondFmtValueObjectType.Max && objectType != XlCondFmtValueObjectType.Min &&
					objectType != XlCondFmtValueObjectType.AutoMax && objectType != XlCondFmtValueObjectType.AutoMin) {
					XlValueObject valueObject = value.Value;
					if(!valueObject.IsEmpty)
						WriteCfvoFormula(EncodeXmlChars(GetValueObjectString(valueObject, topLeft)));
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteCfvoFormula(string formula) {
			WriteStartElement("xm", "f", null);
			try {
				WriteShString(EncodeXmlChars(formula));
			}
			finally {
				WriteEndElement();
			}
		}
		string GetSqRef(IList<XlCellRange> ranges) {
			StringBuilder sb = new StringBuilder();
			foreach(XlCellRange range in ranges) {
				if(sb.Length > 0)
					sb.Append(" ");
				sb.Append(range.ToString());
			}
			return sb.ToString();
		}
		XlCellPosition GetTopLeftCell(IList<XlCellRange> ranges) {
			if(ranges.Count == 0)
				return XlCellPosition.InvalidValue;
			XlCellRange range = ranges[0];
			int column = range.TopLeft.Column;
			int row = range.TopLeft.Row;
			for(int i = 1; i < ranges.Count; i++) {
				range = ranges[i];
				column = Math.Min(column, range.TopLeft.Column);
				row = Math.Min(row, range.TopLeft.Row);
			}
			return new XlCellPosition(column, row);
		}
		void WriteCondFmtRuleIconSetExt(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleIconSet rule) {
			WriteStartElement("cfRule", x14NamespaceReference);
			try {
				WriteStringValue("id", rule.GetRuleId());
				WriteIntValue("priority", rule.Priority);
				WriteStringValue("type", "iconSet");
				WriteCondFmtIconSetExt(conditionalFormatting, rule);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteCondFmtIconSetExt(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleIconSet rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			WriteStartElement("iconSet", x14NamespaceReference);
			try {
				if(rule.IconSetType != XlCondFmtIconSetType.TrafficLights3)
					WriteStringValue("iconSet", iconSetTypeTable[rule.IconSetType]);
				if(!rule.ShowValues)
					WriteBoolValue("showValue", rule.ShowValues);
				if(!rule.Percent)
					WriteBoolValue("percent", rule.Percent);
				if(rule.Reverse)
					WriteBoolValue("reverse", rule.Reverse);
				foreach(XlCondFmtValueObject cfvo in rule.Values)
					WriteCfvoExt(cfvo, topLeft);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateCondFmtRuleColorScale(XlConditionalFormatting conditionalFormatting, XlCondFmtRuleColorScale rule) {
			XlCellPosition topLeft = GetTopLeftCell(conditionalFormatting.Ranges);
			WriteShStartElement("colorScale");
			try {
				if(rule.ColorScaleType == XlCondFmtColorScaleType.ColorScale2) {
					WriteCfvo(rule.MinValue, topLeft);
					WriteCfvo(rule.MaxValue, topLeft);
					WriteColor(rule.MinColor, "color");
					WriteColor(rule.MaxColor, "color");
				}
				else {
					WriteCfvo(rule.MinValue, topLeft);
					WriteCfvo(rule.MidpointValue, topLeft);
					WriteCfvo(rule.MaxValue, topLeft);
					WriteColor(rule.MinColor, "color");
					WriteColor(rule.MidpointColor, "color");
					WriteColor(rule.MaxColor, "color");
				}
			}
			finally {
				WriteShEndElement();
			}
		}
	}
	#endregion
}
