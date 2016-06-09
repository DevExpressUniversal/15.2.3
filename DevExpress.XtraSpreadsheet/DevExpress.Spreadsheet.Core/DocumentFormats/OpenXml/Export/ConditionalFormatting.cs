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
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	public class OpenXmlConditionalFormattingGroup {
		Dictionary<string, ConditionalFormatting> mainItemsList = new Dictionary<string, ConditionalFormatting>();
		Dictionary<string, ConditionalFormatting> extItemsList = new Dictionary<string, ConditionalFormatting>();
		public bool IsPivot { get; set; }
		public CellRangeBase SqRef { get; set; }
		public Dictionary<string, ConditionalFormatting> MainItemsList { get { return mainItemsList; } }
		public Dictionary<string, ConditionalFormatting> ExtItemsList { get { return extItemsList; } }
		public bool HasMainPart { get { return MainItemsList.Count > 0; } }
		public bool HasExtPart { get { return ExtItemsList.Count > 0; } }
		void Add(ConditionalFormatting item, IGuidProvider guidGen, bool mainFlag, bool extFlag) {
			if (mainFlag || extFlag) {
				string guid = guidGen.GetUppercaseString("B"); 
				if (mainFlag)
					MainItemsList.Add(guid, item);
				if (extFlag)
					ExtItemsList.Add(guid, item);
			}
		}
		public void Add(ConditionalFormatting item, IGuidProvider guidGen) {
			bool needExtPart = OpenXmlExporter.ConditionalFormattingNeedExtPart(item);
			bool dontNeedMainPart = (item.Type == ConditionalFormattingType.IconSet) && needExtPart;
			Add(item, guidGen, !dontNeedMainPart, needExtPart);
		}
	}
	partial class OpenXmlExporter {
		#region Conditional formatting
		const string x14NamespaceReference = @"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
		const string xmNamespaceReference = @"http://schemas.microsoft.com/office/excel/2006/main";
		static readonly Dictionary<ConditionalFormattingTimePeriod, string> TimePeriodTable = CreateTimePeriodTable();
		static readonly List<IconSetType> IconSetExtendedTypeTable = CreateIconSetExtendedTypeTable();
		IGuidProvider guidgen = new GuidProvider();
		protected internal IGuidProvider GuidGen { get { return guidgen; } set { guidgen = value; } }
		List<OpenXmlConditionalFormattingGroup> conditionalFormattingGroups;
		protected internal List<OpenXmlConditionalFormattingGroup> ConditionalFormattingGroups { get { return conditionalFormattingGroups; } }
		#region Create table of 'timePeriod' attribute possible values
		static Dictionary<ConditionalFormattingTimePeriod, string> CreateTimePeriodTable() {
			Dictionary<ConditionalFormattingTimePeriod, string> result = new Dictionary<ConditionalFormattingTimePeriod, string>();
			result.Add(ConditionalFormattingTimePeriod.Last7Days, "last7Days");
			result.Add(ConditionalFormattingTimePeriod.LastMonth, "lastMonth");
			result.Add(ConditionalFormattingTimePeriod.LastWeek, "lastWeek");
			result.Add(ConditionalFormattingTimePeriod.NextMonth, "nextMonth");
			result.Add(ConditionalFormattingTimePeriod.NextWeek, "nextWeek");
			result.Add(ConditionalFormattingTimePeriod.ThisMonth, "thisMonth");
			result.Add(ConditionalFormattingTimePeriod.ThisWeek, "thisWeek");
			result.Add(ConditionalFormattingTimePeriod.Today, "today");
			result.Add(ConditionalFormattingTimePeriod.Tomorrow, "tomorrow");
			result.Add(ConditionalFormattingTimePeriod.Yesterday, "yesterday");
			return result;
		}
		#endregion
		static List<IconSetType> CreateIconSetExtendedTypeTable() {
			List<IconSetType> result = new List<IconSetType>();
			result.Add(IconSetType.Stars3);
			result.Add(IconSetType.Triangles3);
			result.Add(IconSetType.Boxes5);
			return result;
		}
		internal bool ConditionalFormattingShouldGenerateExtLst() {
			if (ConditionalFormattingGroups != null)
				foreach (OpenXmlConditionalFormattingGroup group in ConditionalFormattingGroups)
					if (group.HasExtPart)
						return true;
			return false;
		}
		void WriteConditionalFormattingExtItem(string id, ConditionalFormatting item) {
			WriteStartElement("conditionalFormatting", x14NamespaceReference);
			WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
			try {
				WriteConditionalFormattingExtRule(id, item);
				WriteString("sqref", xmNamespaceReference, GetStSqrefValue(item.CellRange));
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteConditionalFormattingExtLst() {
			foreach (OpenXmlConditionalFormattingGroup group in ConditionalFormattingGroups) {
				if (!group.HasExtPart)
					continue;
				WriteShStartElement("ext");
				try {
					WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
					WriteStringValue("uri", ConditionalFormatting.OpenXmlUri);
					WriteStartElement("conditionalFormattings", x14NamespaceReference);
					try {
						foreach(KeyValuePair<string, ConditionalFormatting> item in group.ExtItemsList)
							WriteConditionalFormattingExtItem(item.Key, item.Value);
					}
					finally {
						WriteEndElement();
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		void WriteConditionalFormattingExtRule(string id, ConditionalFormatting item) {
			WriteStartElement("cfRule", x14NamespaceReference);
			try {
				WriteStringValue("id", id);
				switch(item.Type) {
					case ConditionalFormattingType.ColorScale:
						break;
					case ConditionalFormattingType.DataBar:
						WriteConditionalFormattingDataBarExt(item as DataBarConditionalFormatting);
						break;
					case ConditionalFormattingType.IconSet:
						WriteConditionalFormattingIconSetExt(item as IconSetConditionalFormatting);
						break;
					case ConditionalFormattingType.Formula:
						break;
					default:
						break;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		#region Export sheet conditional formatting
		protected internal virtual void GenerateConditionalFormattings() {
			ExportConditionalFormatting(ActiveSheet.ConditionalFormattings);
		}
		protected internal void ExportConditionalFormatting(ConditionalFormattingCollection collection) {
			ConditionalFormattingGroupCollection groupCollection = new ConditionalFormattingGroupCollection();
			foreach(ConditionalFormatting item in collection)
				groupCollection.Register(item);
			conditionalFormattingGroups = new List<OpenXmlConditionalFormattingGroup>();
			foreach (List<ConditionalFormatting> item in groupCollection) {
				OpenXmlConditionalFormattingGroup group = new OpenXmlConditionalFormattingGroup();
				group.IsPivot = item[0].IsPivot;
				group.SqRef = item[0].CellRange;
				foreach (ConditionalFormatting cf in item) {
					group.Add(cf, GuidGen);
				}
				ConditionalFormattingGroups.Add(group);
			}
			ExportConditionalFormattingGroups(ConditionalFormattingGroups);
		}
		void ExportConditionalFormattingGroups(List<OpenXmlConditionalFormattingGroup> groupsCollection) {
			foreach (OpenXmlConditionalFormattingGroup group in groupsCollection)
				if (group.HasMainPart) {
					WriteConditionalFormattingRoot(group);
				}
		}
		#region Export single formatting
		 void WriteConditionalFormattingRoot(OpenXmlConditionalFormattingGroup cfGroup) {
			WriteShStartElement("conditionalFormatting");
			bool isPivot = cfGroup.IsPivot;
			CellRangeBase cellRange = cfGroup.SqRef;
			try {
				if (isPivot)
					WriteBoolValue("pivot", isPivot);
				WriteStSqref(cellRange, "sqref");
				foreach (KeyValuePair<string, ConditionalFormatting> item in cfGroup.MainItemsList)
					if (item.Value != null)
						WriteConditionalFormattingRule(item.Value, item.Key);
			}
			finally {
				WriteShEndElement();
			}
		}
		static bool IsConditionalFormattingHaveExtCfvo(params ConditionalFormattingValueObject[] values) {
			bool result = false;
			foreach(ConditionalFormattingValueObject item in values)
				if(item.ValueType == ConditionalFormattingValueObjectType.AutoMax || item.ValueType == ConditionalFormattingValueObjectType.AutoMin) {
					result = true;
					break;
				}
			return result;
		}
		#region Export single rule
		internal static bool ConditionalFormattingNeedExtPart(ConditionalFormatting item) {
			bool result = false;
			switch(item.Type) {
				case ConditionalFormattingType.DataBar:
					DataBarConditionalFormatting dataBar = item as DataBarConditionalFormatting;
					if(dataBar != null) {
						result |= IsConditionalFormattingHaveExtCfvo(dataBar.LowBound, dataBar.HighBound);
						result |= (dataBar.AxisPosition != OpenXmlDefaultDataBarConditionalFormattingValues.AxisPosition);
						result |= (dataBar.Direction != OpenXmlDefaultDataBarConditionalFormattingValues.Direction);
						result |= (dataBar.GradientFill != OpenXmlDefaultDataBarConditionalFormattingValues.GradientFill);
						result |= dataBar.IsBorderColorAssigned;
						result |= (dataBar.IsNegativeColorSameAsPositive != OpenXmlDefaultDataBarConditionalFormattingValues.NegativeUseSameColor);
					}
					break;
				case ConditionalFormattingType.IconSet:
					IconSetConditionalFormatting iconSet = item as IconSetConditionalFormatting;
					if(iconSet != null) {
						result |= IsConditionalFormattingHaveExtCfvo(iconSet.GetPointValues());
						result |= iconSet.IsCustom;
						result |= IconSetExtendedTypeTable.Contains(iconSet.IconSet);
					}
					break;
			}
			return result;
		}
		void WriteExtReferralItem(string uriValue, string idValue) {
			WriteShStartElement("ext");
			try {
				WriteStringValue("uri", uriValue);
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteString("id", x14NamespaceReference, idValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteExtLstReferralItem(string idValue) {
			WriteShStartElement("extLst");
			try {
				WriteExtReferralItem(ConditionalFormattingRuleDestination.ExtUri, idValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void WriteConditionalFormattingRule(ConditionalFormatting item, string extLstRefGuid) {
			bool needExt = ConditionalFormattingNeedExtPart(item);
			WriteShStartElement("cfRule");
			try {
				WriteIntValue("priority", item.Priority); 
				if(item.StopIfTrue)
					WriteBoolValue("stopIfTrue", item.StopIfTrue); 
				if(item.SupportsDifferentialFormat && (item.DifferentialFormatIndex >= 0)) {
					int dxfId;
					if(exportStyleSheet.DifferentialFormatTable.TryGetValue(item.DifferentialFormatIndex, out dxfId) && (dxfId >= 0)) {
						WriteIntValue("dxfId", dxfId); 
					}
				}
				switch(item.Type) {
					case ConditionalFormattingType.ColorScale:
						WriteConditionalFormattingColorScale(item as ColorScaleConditionalFormatting);
						break;
					case ConditionalFormattingType.DataBar:
						WriteConditionalFormattingDataBar(item as DataBarConditionalFormatting);
						break;
					case ConditionalFormattingType.IconSet:
						WriteConditionalFormattingIconSet(item as IconSetConditionalFormatting);
						break;
					case ConditionalFormattingType.Formula:
						WriteConditionalFormattingFormula(item as FormulaConditionalFormatting);
						break;
					default:
						Exceptions.ThrowInvalidOperationException("invalid conditional formatting rule");
						break;
				}
				if (needExt && !string.IsNullOrEmpty(extLstRefGuid)) {
					WriteExtLstReferralItem(extLstRefGuid);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#endregion
		#endregion
		#region Conditional formatting export helper functions
		protected internal void WriteConditionalFormattingColorScale(ColorScaleConditionalFormatting item) {
			const string typeName = "colorScale";
			if(item != null) {
				WriteStringValue("type", typeName);
				WriteShStartElement(typeName);
				ColorModelInfoCache colorCache = Workbook.Cache.ColorModelInfoCache;
				try {
					switch(item.ScaleType) {
						case ColorScaleType.Color2:
							WriteConditionalFormattingValueObject(item.LowPointValue, String.Empty, true);
							WriteConditionalFormattingValueObject(item.HighPointValue, String.Empty, true);
							WriteColor(colorCache[item.LowPointColorIndex], "color");
							WriteColor(colorCache[item.HighPointColorIndex], "color");
							break;
						case ColorScaleType.Color3:
							WriteConditionalFormattingValueObject(item.LowPointValue, String.Empty, true);
							WriteConditionalFormattingValueObject(item.MiddlePointValue, String.Empty, true);
							WriteConditionalFormattingValueObject(item.HighPointValue, String.Empty, false);
							WriteColor(colorCache[item.LowPointColorIndex], "color");
							WriteColor(colorCache[item.MiddlePointColorIndex], "color");
							WriteColor(colorCache[item.HighPointColorIndex], "color");
							break;
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected void WriteConditionalFormattingCustomIcon(ConditionalFormattingCustomIcon value) {
			WriteStartElement("cfIcon", x14NamespaceReference);
			try {
				string s;
				if(IconSetTypeTable.TryGetValue(value.IconSet, out s)) {
					WriteStringValue("iconSet", s);
					WriteIntValue("iconId", value.IconIndex);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteConditionalFormattingCfvoFormula(string formula) {
			WriteStartElement("xm", "f", null);
			try {
				WriteShString(EncodeXmlChars(formula));
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteConditionalFormattingCfvoValue(string value, bool useValAttribute) {
			if(useValAttribute)
				WriteStringValue("val", value);
			else
				WriteConditionalFormattingCfvoFormula(value);
		}
		protected void WriteConditionalFormattingValueObject(ConditionalFormattingValueObject value, string prefix, bool distinguishAutoValues) {
			bool prefixMissed = String.IsNullOrEmpty(prefix);
			if(prefixMissed)
				WriteStartElement("cfvo", null);
			else
				WriteStartElement(prefix, "cfvo", null);
			try {
				if(!value.IsGreaterOrEqual)
					WriteBoolValue("gte", value.IsGreaterOrEqual);
				switch(value.ValueType) {
					case ConditionalFormattingValueObjectType.Formula:
						WriteStringValue("type", "formula");
						WriteConditionalFormattingCfvoValue(CondFmtValueAsExpression(value.ToString()), prefixMissed);
						break;
					case ConditionalFormattingValueObjectType.AutoMax: 
						WriteStringValue("type", distinguishAutoValues ? "autoMax" : "max");
						break;
					case ConditionalFormattingValueObjectType.Max:
						WriteStringValue("type", "max");
						break;
					case ConditionalFormattingValueObjectType.AutoMin: 
						WriteStringValue("type", distinguishAutoValues ? "autoMin" : "min");
						break;
					case ConditionalFormattingValueObjectType.Min:
						WriteStringValue("type", "min");
						break;
					case ConditionalFormattingValueObjectType.Num:
						WriteStringValue("type", "num");
						WriteConditionalFormattingCfvoValue(CondFmtValueAsExpression(value.ToString()), prefixMissed);
						break;
					case ConditionalFormattingValueObjectType.Percent:
						WriteStringValue("type", "percent");
						WriteConditionalFormattingCfvoValue(CondFmtValueAsExpression(value.ToString()), prefixMissed);
						break;
					case ConditionalFormattingValueObjectType.Percentile:
						WriteStringValue("type", "percentile");
						WriteConditionalFormattingCfvoValue(CondFmtValueAsExpression(value.ToString()), prefixMissed);
						break;
					default:
						Exceptions.ThrowInvalidOperationException("Invalid conditional formatting value object");
						break;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void WriteConditionalFormattingDataBarAxisPosition(DataBarConditionalFormatting item) {
			switch(item.AxisPosition) {
				case ConditionalFormattingDataBarAxisPosition.None:
					WriteStringValue("axisPosition", "none");
					break;
				case ConditionalFormattingDataBarAxisPosition.Middle:
					WriteStringValue("axisPosition", "middle");
					break;
			}
		}
		protected internal void WriteConditionalFormattingDataBarAxisDirection(DataBarConditionalFormatting item) {
			switch(item.Direction) {
				case ConditionalFormattingDataBarDirection.LeftToRight:
					WriteStringValue("direction", "leftToRight");
					break;
				case ConditionalFormattingDataBarDirection.RightToLeft:
					WriteStringValue("direction", "rightToLeft");
					break;
			}
		}
		protected internal void WriteConditionalFormattingDataBar(DataBarConditionalFormatting item) {
			const string typeName = "dataBar";
			if(item != null) {
				WriteStringValue("type", typeName);
				WriteShStartElement(typeName);
				try {
					if(item.MinLength != DataBarConditionalFormattingInfo.DefaultMinWidth) 
						WriteIntValue("minLength", item.MinLength);
					if(item.MaxLength != DataBarConditionalFormattingInfo.DefaultMaxWidth) 
						WriteIntValue("maxLength", item.MaxLength);
					if(item.ShowValue != DataBarConditionalFormattingInfo.DefaultShowValue)
						WriteBoolValue("showValue", item.ShowValue);
					WriteConditionalFormattingValueObject(item.LowBound, String.Empty, false);
					WriteConditionalFormattingValueObject(item.HighBound, String.Empty, false);
					WriteColor(Workbook.Cache.ColorModelInfoCache[item.ColorIndex], "color");
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal void WriteConditionalFormattingDataBarExt(DataBarConditionalFormatting item) {
			const string typeName = "dataBar";
			if(item != null) {
				WriteStringValue("type", typeName);
				WriteStartElement(typeName, x14NamespaceReference);
				try {
					if (item.GradientFill != OpenXmlDefaultDataBarConditionalFormattingValues.GradientFill)
						WriteBoolValue("gradient", item.GradientFill);
					if(item.IsNegativeColorSameAsPositive != OpenXmlDefaultDataBarConditionalFormattingValues.NegativeUseSameColor)
						WriteBoolValue("negativeBarColorSameAsPositive", item.IsNegativeColorSameAsPositive);
					if(item.IsBorderColorAssigned) { 
						WriteBoolValue("border", item.IsBorderColorAssigned);
						if(item.IsNegativeBorderColorSameAsPositive != OpenXmlDefaultDataBarConditionalFormattingValues.NegativeUseSameBorderColor)
							WriteBoolValue("negativeBarBorderColorSameAsPositive", item.IsNegativeBorderColorSameAsPositive);
					}
					WriteConditionalFormattingDataBarAxisPosition(item);
					WriteConditionalFormattingDataBarAxisDirection(item);
					WriteConditionalFormattingValueObject(item.LowBound, "x14", true);
					WriteConditionalFormattingValueObject(item.HighBound, "x14", true);
					if(item.IsBorderColorAssigned)
						WriteColor(Workbook.Cache.ColorModelInfoCache[item.BorderColorIndex], "borderColor", x14NamespaceReference);
					if(!item.IsNegativeColorSameAsPositive)
						WriteColor(Workbook.Cache.ColorModelInfoCache[item.NegativeValueColorIndex], "negativeFillColor", x14NamespaceReference);
					if(item.IsBorderColorAssigned && !item.IsNegativeBorderColorSameAsPositive)
						WriteColor(Workbook.Cache.ColorModelInfoCache[item.NegativeValueBorderColorIndex], "negativeBorderColor", x14NamespaceReference);
					if(item.AxisPosition != ConditionalFormattingDataBarAxisPosition.None)
						WriteColor(Workbook.Cache.ColorModelInfoCache[item.AxisColorIndex], "axisColor", x14NamespaceReference);
				}
				finally {
					WriteEndElement();
				}
			}
		}
		protected internal void WriteConditionalFormattingIconSet(IconSetConditionalFormatting item) {
			const string typeName = "iconSet";
			if((item != null)) {
				WriteStringValue("type", typeName);
				WriteShStartElement(typeName);
				try {
					int count = item.ExpectedPointsNumber;
					if((item.IconSet != OpenXmlDefaultIconSetConditionalFormattingValues.IconSet) && !IconSetExtendedTypeTable.Contains(item.IconSet))
						WriteIconSetAttribute(item.IconSet);
					if(item.Percent != OpenXmlDefaultIconSetConditionalFormattingValues.Percent)
						WriteBoolValue("percent", item.Percent);
					if(item.Reversed != OpenXmlDefaultIconSetConditionalFormattingValues.Reverse)
						WriteBoolValue("reverse", item.Reversed);
					if(item.ShowValue != OpenXmlDefaultIconSetConditionalFormattingValues.ShowValue)
						WriteBoolValue("showValue", item.ShowValue);
					for(int i = 0; i < count; ++i)
						WriteConditionalFormattingValueObject(item.GetPointValue(i), String.Empty, false);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		void WriteIconSetAttribute(IconSetType iconSet) {
			string s;
			if(IconSetTypeTable.TryGetValue(iconSet, out s))
				WriteStringValue("iconSet", s);
		}
		protected internal void WriteConditionalFormattingIconSetExt(IconSetConditionalFormatting item) {
			const string typeName = "iconSet";
			if((item != null)) {
				WriteIntValue("priority", item.Priority);
				WriteStringValue("type", typeName);
				WriteStartElement(typeName, x14NamespaceReference);
				try {
					int count = item.ExpectedPointsNumber;
					if(IconSetExtendedTypeTable.Contains(item.IconSet))
						WriteIconSetAttribute(item.IconSet);
					if (item.Percent != OpenXmlDefaultIconSetConditionalFormattingValues.Percent)
						WriteBoolValue("percent", item.Percent);
					if (item.Reversed != OpenXmlDefaultIconSetConditionalFormattingValues.Reverse)
						WriteBoolValue("reverse", item.Reversed);
					if (item.ShowValue != OpenXmlDefaultIconSetConditionalFormattingValues.ShowValue)
						WriteBoolValue("showValue", item.ShowValue);
					if (item.IsCustom)
						WriteBoolValue("custom", item.IsCustom);
					for(int i = 0; i < count; ++i)
						WriteConditionalFormattingValueObject(item.GetPointValue(i), "x14", true);
					if(item.IsCustom) {
						int iconCount = item.ExpectedPointsNumber;
						for(int i = 0; i < iconCount; ++i) {
							ConditionalFormattingCustomIcon icon = item.GetIcon(i);
							WriteConditionalFormattingCustomIcon(icon);
						}
					}
				}
				finally {
					WriteEndElement();
				}
			}
		}
		protected void WriteConditionalFormattingTextCriteria(bool isOperator, string name, TextFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue((isOperator ? "operator" : "type"), name);
				WriteStringValue("text", item.Value);
				WriteString("formula", EncodeXmlChars(item.GetFormulaString()), null, false);
			}
		}
		protected void WriteConditionalFormattingTop10Criteria(RankFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue("type", "top10");
				WriteBoolValue("bottom", !(item.Condition == ConditionalFormattingRankCondition.TopByPercent || item.Condition == ConditionalFormattingRankCondition.TopByRank));
				WriteBoolValue("percent", item.Condition == ConditionalFormattingRankCondition.BottomByPercent || item.Condition == ConditionalFormattingRankCondition.TopByPercent);
				WriteIntValue("rank", item.Rank);
			}
		}
		protected void WriteConditionalFormattingPeriodCriteria(TimePeriodFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue("type", "timePeriod");
				string s;
				WriteStringValue("timePeriod", TimePeriodTable.TryGetValue(item.TimePeriod, out s) ? s : "today");
				WriteString("formula", EncodeXmlChars(item.GetFormulaString()), null, false);
			}
		}
		string CondFmtValueAsExpression(string value) {
			if (!string.IsNullOrEmpty(value) && (value[0] == '='))
				value = value.Remove(0, 1);
			return value;
		}
		protected void WriteConditionalFormattingExpressionCriteria(ExpressionFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue("type", "expression");
				WriteString("formula", EncodeXmlChars(CondFmtValueAsExpression(item.Value)), null, false);
			}
		}
		protected void WriteConditionalFormattingBetweenCriteria(RangeFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue("operator", item.Condition == ConditionalFormattingRangeCondition.Inside ? "between" : "notBetween");
				WriteString("formula", EncodeXmlChars(CondFmtValueAsExpression(item.Value)), null, false);
				WriteString("formula", EncodeXmlChars(CondFmtValueAsExpression(item.Value2)), null, false);
			}
		}
		protected void WriteConditionalFormattingConditionCriteria(string operatorName, ExpressionFormulaConditionalFormatting item) {
			if(item != null) {
				WriteStringValue("operator", operatorName);
				WriteString("formula", EncodeXmlChars(CondFmtValueAsExpression(item.Value)), null, false);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800")]
		protected void WriteConditionalFormattingCellIsCriteria(FormulaConditionalFormatting item) {
			WriteStringValue("type", "cellIs");
			ConditionalFormattingOperator xmlOperator = item.Operator;
			switch(xmlOperator) {
				case ConditionalFormattingOperator.BeginsWith:
					WriteConditionalFormattingTextCriteria(true, "beginsWith", item as TextFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.Between:
				case ConditionalFormattingOperator.NotBetween:
					WriteConditionalFormattingBetweenCriteria(item as RangeFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.ContainsText:
					WriteConditionalFormattingTextCriteria(true, "containsText", item as TextFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.EndsWith:
					WriteConditionalFormattingTextCriteria(true, "endsWith", item as TextFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.Equal:
					WriteConditionalFormattingConditionCriteria("equal", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.GreaterThan:
					WriteConditionalFormattingConditionCriteria("greaterThan", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.GreaterThanOrEqual:
					WriteConditionalFormattingConditionCriteria("greaterThanOrEqual", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.LessThan:
					WriteConditionalFormattingConditionCriteria("lessThan", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.LessThanOrEqual:
					WriteConditionalFormattingConditionCriteria("lessThanOrEqual", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.NotEqual:
					WriteConditionalFormattingConditionCriteria("notEqual", item as ExpressionFormulaConditionalFormatting);
					break;
				case ConditionalFormattingOperator.NotContains:
					WriteConditionalFormattingTextCriteria(true, "notContains", item as TextFormulaConditionalFormatting);
					break;
				default:
					Exceptions.ThrowInvalidOperationException("Invalid conditional fortmatting operator");
					break;
			}
		}
		protected internal void WriteConditionalFormattingSpecialCondition(string name, SpecialFormulaConditionalFormatting item) {
			WriteStringValue("type", name);
			string formula = item.GetFormulaString();
			if(!string.IsNullOrEmpty(formula))
				WriteString("formula", EncodeXmlChars(formula), null, false);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800")] 
		protected internal void WriteConditionalFormattingFormula(FormulaConditionalFormatting item) {
			if(item != null) {
				switch(item.RuleType) {
					case ConditionalFormattingRuleType.AboveOrBelowAverage:
						WriteStringValue("type", "aboveAverage");
						AverageFormulaConditionalFormatting averageComparer = item as AverageFormulaConditionalFormatting;
						if (averageComparer != null) {
							ConditionalFormattingAverageCondition condition = averageComparer.Condition;
							WriteBoolValue("aboveAverage", condition == ConditionalFormattingAverageCondition.Above || condition == ConditionalFormattingAverageCondition.AboveOrEqual);
							WriteBoolValue("equalAverage", condition == ConditionalFormattingAverageCondition.AboveOrEqual || condition == ConditionalFormattingAverageCondition.BelowOrEqual);
							if(averageComparer.StdDev > 0) {
								WriteIntValue("stdDev", averageComparer.StdDev);
							}
						}
						break;
					case ConditionalFormattingRuleType.BeginsWithText:
						WriteConditionalFormattingTextCriteria(false, "beginsWith", item as TextFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.CellIsBlank:
						WriteConditionalFormattingSpecialCondition("containsBlanks", item as SpecialFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.CellIsNotBlank:
						WriteConditionalFormattingSpecialCondition("notContainsBlanks", item as SpecialFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.CompareWithFormulaResult:
						WriteConditionalFormattingCellIsCriteria(item);
						break;
					case ConditionalFormattingRuleType.ContainsErrors:
						WriteConditionalFormattingSpecialCondition("containsErrors", item as SpecialFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.ContainsText:
						WriteConditionalFormattingTextCriteria(false, "containsText", item as TextFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.DuplicateValues:
						WriteStringValue("type", "duplicateValues");
						break;
					case ConditionalFormattingRuleType.EndsWithText:
						WriteConditionalFormattingTextCriteria(false, "endsWith", item as TextFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.ExpressionIsTrue:
						WriteConditionalFormattingExpressionCriteria(item as ExpressionFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.InsideDatePeriod:
						WriteConditionalFormattingPeriodCriteria(item as TimePeriodFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.NotContainsErrors:
						WriteConditionalFormattingSpecialCondition("notContainsErrors", item as SpecialFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.NotContainsText:
						WriteConditionalFormattingTextCriteria(false, "notContainsText", item as TextFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.TopOrBottomValue:
						WriteConditionalFormattingTop10Criteria(item as RankFormulaConditionalFormatting);
						break;
					case ConditionalFormattingRuleType.UniqueValue:
						WriteStringValue("type", "uniqueValues");
						break;
					default:
						Exceptions.ThrowInvalidOperationException("Invalid conditional formatting formula type");
						break;
				}
			}
		}
		#endregion
		#endregion
	}
}
