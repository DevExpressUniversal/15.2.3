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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ConditionalFormattingRuleContent
	[Flags]
	public enum ConditionalFormattingRuleContent {
		None = 0,
		ExpectFormula = 1,		 
		ExpectColorScale = 2,	  
		ExpectDataBar = 4,		 
		ExpectIconSet = 8,		 
		IgnoreFormula = 0x100,	 
		IgnoreColorScale = 0x200,
		IgnoreDataBar = 0x400,
		IgnoreIconSet = 0x800
	}
	#endregion
	#region ConditionalFormattingRuleDestination
	public class ConditionalFormattingRuleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		const ConditionalFormattingRuleContent expectActivePresentCF = ConditionalFormattingRuleContent.ExpectColorScale | ConditionalFormattingRuleContent.ExpectDataBar | ConditionalFormattingRuleContent.ExpectIconSet;
		internal const string ExtUri = "{B025F937-C7B1-47D3-B67F-A62EFF666E3E}";
		#region Static members
		internal static readonly Dictionary<string, ConditionalFormattingRuleType> RuleTypeTable = CreateRuleTypeTable();
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static readonly Dictionary<string, ConditionalFormattingOperator> OperatorTable = CreateOperatorTable();
		static readonly Dictionary<string, ConditionalFormattingTimePeriod> TimePeriodTable = CreateTimePeriodTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("colorScale", OnColorScale);
			result.Add("dataBar", OnDataBar);
			result.Add("extLst", OnExtensionList);
			result.Add("f", OnActivePresentFormula);
			result.Add("formula", OnFormula);
			result.Add("iconSet", OnIconSet);
			return result;
		}
		#region Create table of 'type' attribute possible values
		static Dictionary<string, ConditionalFormattingRuleType> CreateRuleTypeTable() {
			Dictionary<string, ConditionalFormattingRuleType> result = new Dictionary<string, ConditionalFormattingRuleType>();
			result.Add("aboveAverage", ConditionalFormattingRuleType.AboveOrBelowAverage);
			result.Add("beginsWith", ConditionalFormattingRuleType.BeginsWithText);
			result.Add("cellIs", ConditionalFormattingRuleType.CompareWithFormulaResult);
			result.Add("colorScale", ConditionalFormattingRuleType.ColorScale);
			result.Add("containsBlanks", ConditionalFormattingRuleType.CellIsBlank);
			result.Add("containsErrors", ConditionalFormattingRuleType.ContainsErrors);
			result.Add("containsText", ConditionalFormattingRuleType.ContainsText);
			result.Add("dataBar", ConditionalFormattingRuleType.DataBar);
			result.Add("duplicateValues", ConditionalFormattingRuleType.DuplicateValues);
			result.Add("endsWith", ConditionalFormattingRuleType.EndsWithText);
			result.Add("expression", ConditionalFormattingRuleType.ExpressionIsTrue);
			result.Add("iconSet", ConditionalFormattingRuleType.IconSet);
			result.Add("notContainsBlanks", ConditionalFormattingRuleType.CellIsNotBlank);
			result.Add("notContainsErrors", ConditionalFormattingRuleType.NotContainsErrors);
			result.Add("notContainsText", ConditionalFormattingRuleType.NotContainsText);
			result.Add("timePeriod", ConditionalFormattingRuleType.InsideDatePeriod);
			result.Add("top10", ConditionalFormattingRuleType.TopOrBottomValue);
			result.Add("uniqueValues", ConditionalFormattingRuleType.UniqueValue);
			return result;
		}
		#endregion
		#region Create table of 'operator' attribute possible values
		static Dictionary<string, ConditionalFormattingOperator> CreateOperatorTable() {
			Dictionary<string, ConditionalFormattingOperator> result = new Dictionary<string, ConditionalFormattingOperator>();
			result.Add("beginsWith", ConditionalFormattingOperator.BeginsWith);
			result.Add("between", ConditionalFormattingOperator.Between);
			result.Add("containsText", ConditionalFormattingOperator.ContainsText);
			result.Add("endsWith", ConditionalFormattingOperator.EndsWith);
			result.Add("equal", ConditionalFormattingOperator.Equal);
			result.Add("greaterThan", ConditionalFormattingOperator.GreaterThan);
			result.Add("greaterThanOrEqual", ConditionalFormattingOperator.GreaterThanOrEqual);
			result.Add("lessThan", ConditionalFormattingOperator.LessThan);
			result.Add("lessThanOrEqual", ConditionalFormattingOperator.LessThanOrEqual);
			result.Add("notBetween", ConditionalFormattingOperator.NotBetween);
			result.Add("notContains", ConditionalFormattingOperator.NotContains);
			result.Add("notEqual", ConditionalFormattingOperator.NotEqual);
			return result;
		}
		#endregion
		#region Create table of 'timePeriod' attribute possible values
		static Dictionary<string, ConditionalFormattingTimePeriod> CreateTimePeriodTable() {
			Dictionary<string, ConditionalFormattingTimePeriod> result = new Dictionary<string, ConditionalFormattingTimePeriod>();
			result.Add("last7Days", ConditionalFormattingTimePeriod.Last7Days);
			result.Add("lastMonth", ConditionalFormattingTimePeriod.LastMonth);
			result.Add("lastWeek", ConditionalFormattingTimePeriod.LastWeek);
			result.Add("nextMonth", ConditionalFormattingTimePeriod.NextMonth);
			result.Add("nextWeek", ConditionalFormattingTimePeriod.NextWeek);
			result.Add("thisMonth", ConditionalFormattingTimePeriod.ThisMonth);
			result.Add("thisWeek", ConditionalFormattingTimePeriod.ThisWeek);
			result.Add("today", ConditionalFormattingTimePeriod.Today);
			result.Add("tomorrow", ConditionalFormattingTimePeriod.Tomorrow);
			result.Add("yesterday", ConditionalFormattingTimePeriod.Yesterday);
			return result;
		}
		#endregion
		static ConditionalFormattingRuleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingRuleDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		ConditionalFormattingRuleContent contentRestriction;
		ConditionalFormattingCreatorInitData initData;
		#endregion
		internal ConditionalFormattingRuleDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorInitData initData)
			: base(importer) {
			this.initData = initData;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ConditionalFormattingRuleContent ContentRestriction { get { return contentRestriction; } protected set { contentRestriction = value; } }
		internal ConditionalFormattingCreatorInitData InitData { get { return initData; } }
		#endregion
		ConditionalFormattingRuleContent GetContentRectrictions() {
			ConditionalFormattingRuleContent result = ConditionalFormattingRuleContent.None;
			switch(InitData.RuleType) {
				case ConditionalFormattingRuleType.ContainsText:
				case ConditionalFormattingRuleType.NotContainsText:
				case ConditionalFormattingRuleType.BeginsWithText:
				case ConditionalFormattingRuleType.EndsWithText:
				case ConditionalFormattingRuleType.InsideDatePeriod:
				case ConditionalFormattingRuleType.CellIsBlank:
				case ConditionalFormattingRuleType.ContainsErrors:
				case ConditionalFormattingRuleType.CellIsNotBlank:
				case ConditionalFormattingRuleType.NotContainsErrors:
				case ConditionalFormattingRuleType.TopOrBottomValue:
				case ConditionalFormattingRuleType.AboveOrBelowAverage:
					result = ConditionalFormattingRuleContent.IgnoreFormula | ConditionalFormattingRuleContent.ExpectFormula;
					break;
				case ConditionalFormattingRuleType.CompareWithFormulaResult:
					result = ConditionalFormattingRuleContent.ExpectFormula;
					break;
				case ConditionalFormattingRuleType.ColorScale:
					result = ConditionalFormattingRuleContent.ExpectColorScale;
					break;
				case ConditionalFormattingRuleType.ExpressionIsTrue:
					result = ConditionalFormattingRuleContent.ExpectFormula;
					break;
				case ConditionalFormattingRuleType.DataBar:
					result = ConditionalFormattingRuleContent.ExpectDataBar;
					break;
				case ConditionalFormattingRuleType.IconSet:
					result = ConditionalFormattingRuleContent.ExpectIconSet;
					break;
				case ConditionalFormattingRuleType.Unknown:
					Exceptions.ThrowInternalException();
					break;
			}
			return result;
		}
		void ProcessColorScale() {
			ConditionalFormattingColorScaleCreatorData details = new ConditionalFormattingColorScaleCreatorData(InitData.Sheet, InitData.CellRange, InitData.IsPivot);
			details.IsActivePresent = InitData.IsActivePresent;
			details.Priority = InitData.Priority;
			details.RuleType = InitData.RuleType;
			details.ShowValue = InitData.ShowValue;
			details.StopIfTrue = InitData.StopIfTrue;
			InitData.Details = details;
		}
		void ProcessDataBar() {
			ConditionalFormattingDataBarCreatorData details = new ConditionalFormattingDataBarCreatorData(InitData.Sheet, InitData.CellRange, InitData.IsPivot);
			details.IsActivePresent = InitData.IsActivePresent;
			details.Priority = InitData.Priority;
			details.RuleType = InitData.RuleType;
			details.ShowValue = InitData.ShowValue;
			details.StopIfTrue = InitData.StopIfTrue;
			InitData.Details = details;
		}
		void ProcessFormula(XmlReader reader) {
			ConditionalFormattingFormulaCreatorData details = new ConditionalFormattingFormulaCreatorData(InitData.Sheet, InitData.CellRange, InitData.IsPivot);
			details.IsActivePresent = InitData.IsActivePresent;
			details.Priority = InitData.Priority;
			details.RuleType = InitData.RuleType;
			details.ShowValue = InitData.ShowValue;
			details.StopIfTrue = InitData.StopIfTrue;
			details.AboveAverage = Importer.GetOnOffValue(reader, "aboveAverage", true);
			details.Bottom = Importer.GetOnOffValue(reader, "bottom", false);
			details.CfOperator = Importer.GetWpEnumValue<ConditionalFormattingOperator>(reader, "operator", OperatorTable, ConditionalFormattingOperator.Unknown);
			details.EqualAverage = Importer.GetOnOffValue(reader, "equalAverage", false);
			details.Percent = Importer.GetOnOffValue(reader, "percent", false);
			details.Rank = Importer.GetIntegerValue(reader, "rank", 10); 
			details.StdDev = Importer.GetIntegerValue(reader, "stdDev", 0);
			details.Text = Importer.ReadAttribute(reader, "text");
			details.TimePeriod = Importer.GetWpEnumValue<ConditionalFormattingTimePeriod>(reader, "timePeriod", TimePeriodTable, ConditionalFormattingTimePeriod.Unknown);
			details.DxfId = Importer.StyleSheet.GetDifferentialFormatIndex(InitData.DxfId);
			InitData.Details = details;
		}
		void ProcessIconSet() {
			ConditionalFormattingIconSetCreatorData details = new ConditionalFormattingIconSetCreatorData(InitData.Sheet, InitData.CellRange, InitData.IsPivot);
			details.IsActivePresent = InitData.IsActivePresent;
			details.Priority = InitData.Priority;
			details.RuleType = InitData.RuleType;
			details.ShowValue = InitData.ShowValue;
			details.StopIfTrue = InitData.StopIfTrue;
			InitData.Details = details;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			InitData.RuleType = Importer.GetWpEnumValue<ConditionalFormattingRuleType>(reader, "type", RuleTypeTable, ConditionalFormattingRuleType.Unknown);
			InitData.IsActivePresent = Importer.GetOnOffValue(reader, "activePresent", false);
			InitData.StopIfTrue = Importer.GetOnOffValue(reader, "stopIfTrue", false);
			InitData.Priority = Importer.GetIntegerValue(reader, "priority", 1);
			InitData.DxfId = Importer.GetIntegerValue(reader, "dxfId", -1);
			switch(InitData.RuleType) {
				case ConditionalFormattingRuleType.ColorScale:
					ProcessColorScale();
					break;
				case ConditionalFormattingRuleType.DataBar:
					ProcessDataBar();
					break;
				case ConditionalFormattingRuleType.IconSet:
					ProcessIconSet();
					break;
				case ConditionalFormattingRuleType.Unknown:
					Exceptions.ThrowInternalException();
					break;
				default:
					ProcessFormula(reader);
					break;
			}
			ContentRestriction = GetContentRectrictions();
		}
		public override void ProcessElementClose(XmlReader reader) {
			if((ContentRestriction == ConditionalFormattingRuleContent.None) || (ContentRestriction & ConditionalFormattingRuleContent.ExpectFormula) == ConditionalFormattingRuleContent.ExpectFormula) {
				ConditionalFormattingCreatorData actualData = InitData.Details;
				Importer.ConditionalFormattingCreatorCollection.Add(actualData);
			}
		}
		static void HandleContentError(string s) {
			Exceptions.ThrowInvalidOperationException(s + " not allowed here");
		}
		#region colorScale handler
		static Destination OnColorScale(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.IgnoreColorScale) == ConditionalFormattingRuleContent.IgnoreColorScale)
				return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.ExpectColorScale) != ConditionalFormattingRuleContent.ExpectColorScale)
				HandleContentError("colorScale");
			return new ConditionalFormattingColorScaleDestination(importer, self.InitData);
		}
		#endregion
		#region dataBar handler
		static Destination OnDataBar(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.IgnoreDataBar) == ConditionalFormattingRuleContent.IgnoreDataBar)
				return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.ExpectDataBar) != ConditionalFormattingRuleContent.ExpectDataBar)
				HandleContentError("dataBar");
			return new ConditionalFormattingDataBarDestination(importer, self.InitData);
		}
		#endregion
		void FormulaAction(string value) {
			ConditionalFormattingFormulaCreatorData data = InitData.Details as ConditionalFormattingFormulaCreatorData;
			if(data != null)
				data.Formulas.Add(value);
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.IgnoreFormula) == ConditionalFormattingRuleContent.IgnoreFormula)
				return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.ExpectFormula) != ConditionalFormattingRuleContent.ExpectFormula)
				HandleContentError("formula");
			return new ConditionalFormattingFormulaDestination(importer, self.FormulaAction);
		}
		#region iconSet handler
		static Destination OnIconSet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.IgnoreIconSet) == ConditionalFormattingRuleContent.IgnoreIconSet)
				return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
			if((self.ContentRestriction & ConditionalFormattingRuleContent.ExpectIconSet) != ConditionalFormattingRuleContent.ExpectIconSet)
				HandleContentError("iconSet");
			return new ConditionalFormattingIconSetDestination(importer, self.InitData);
		}
		#endregion
		void ActivePresentFormulaAction(string value) {
			InitData.Details.ActivePresent = value;
		}
		static Destination OnActivePresentFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			if(((self.ContentRestriction & expectActivePresentCF) != 0) && !self.InitData.IsActivePresent)
				HandleContentError("f");
			return new ConditionalFormattingFormulaDestination(importer, self.ActivePresentFormulaAction);
		}
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleDestination self = GetThis(importer);
			return new ConditionalFormattingRuleExtLstDestination(importer, self.InitData.Details);
		}
	}
	#endregion
	#region ConditionalFormattingRuleExtLstDestination
	public class ConditionalFormattingExtRuleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		string referenceId;
		ConditionalFormattingRuleType ruleType;
		int priority;
		ConditionalFormattingExtRuleSqRefRegistrar parentNodeRegistrar;
		ConditionalFormattingCreatorInitData initData;
		string text;
		int dxfId = -1;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("colorScale", OnColorScale);
			result.Add("dataBar", OnDataBar);
			result.Add("iconSet", OnIconSet);
			result.Add("f", OnFormula);
			result.Add("dxf", OnDifferentialFormat);
			return result;
		}
		#endregion
		internal ConditionalFormattingExtRuleDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingExtRuleSqRefRegistrar registrar)
			: base(importer) {
				parentNodeRegistrar = registrar;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public string ReferenceId { get { return referenceId; } protected set { referenceId = value; } }
		public ConditionalFormattingRuleType RuleType { get { return ruleType; } protected set { ruleType = value; } }
		public int Priority { get { return priority; } protected set { priority = value; } }
		#endregion
		static ConditionalFormattingExtRuleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingExtRuleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ReferenceId = Importer.ReadAttribute(reader, "id");
			RuleType = Importer.GetWpEnumValue<ConditionalFormattingRuleType>(reader, "type", ConditionalFormattingRuleDestination.RuleTypeTable, ConditionalFormattingRuleType.Unknown);
			Priority = Importer.GetIntegerValue(reader, "priority", -1);
			text = Importer.ReadAttribute(reader, "text");
		}
		ConditionalFormattingCreatorData GetConditionalFormatting() {
			List<ConditionalFormattingCreatorData> creatorCache = Importer.ConditionalFormattingCreatorCollection;
			foreach(ConditionalFormattingCreatorData item in creatorCache) {
				if(StringExtensions.CompareInvariantCultureIgnoreCase(item.ExtId, ReferenceId) == 0)
					return item;
			}
			return null;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (initData != null) {
				ConditionalFormattingFormulaCreatorData data = initData.Details as ConditionalFormattingFormulaCreatorData;
				if (data != null) {
					data.Text = text;
					data.ApplyFormulas();
					if (dxfId >= 0) {
						data.DxfId = Importer.StyleSheet.DifferentialFormatTable[dxfId];
						Importer.ConditionalFormattingCreatorCollection.Add(data);
					}
				}
			}
			base.ProcessElementClose(reader);
		}
		void FormulaAction(string value) {
			if (initData == null)
				return;
			ConditionalFormattingFormulaCreatorData data = initData.Details as ConditionalFormattingFormulaCreatorData;
			if (data != null)
				data.Formulas.Add(value);
		}
		void InitalizeCreatorData(ConditionalFormattingCreatorData data) {
			data.ExtId = this.ReferenceId;
			if (this.Priority >= 0)
				data.Priority = this.Priority;
		}
		static Destination OnColorScale(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtRuleDestination self = GetThis(importer);
			ConditionalFormattingCreatorData data = self.GetConditionalFormatting();
			if((data != null) && (self.RuleType == ConditionalFormattingRuleType.ColorScale) && (data.Type == ConditionalFormattingType.ColorScale)) {
				self.InitalizeCreatorData(data);
				return new ConditionalFormattingColorScaleDestination(importer, data as ConditionalFormattingColorScaleCreatorData);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnDataBar(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtRuleDestination self = GetThis(importer);
			ConditionalFormattingCreatorData data = self.GetConditionalFormatting();
			if((data != null) && (self.RuleType == ConditionalFormattingRuleType.DataBar) && (data.Type == ConditionalFormattingType.DataBar)) {
				self.InitalizeCreatorData(data);
				self.parentNodeRegistrar(data);
				return new ConditionalFormattingDataBarDestination(importer, data as ConditionalFormattingDataBarCreatorData);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnIconSet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtRuleDestination self = GetThis(importer);
			ConditionalFormattingCreatorData data = self.GetConditionalFormatting();
			if(data == null) {
				ConditionalFormattingCreatorInitData initData = new ConditionalFormattingCreatorInitData();
				initData.RuleType = ConditionalFormattingRuleType.IconSet;
				data = new ConditionalFormattingIconSetCreatorData(importer.CurrentWorksheet);
				self.InitalizeCreatorData(data);
				initData.Details = data;
				self.parentNodeRegistrar(data);
				return new ConditionalFormattingIconSetDestination(importer, initData);
			}
			else {
				if((self.RuleType != ConditionalFormattingRuleType.IconSet) || (data.Type != ConditionalFormattingType.IconSet))
					return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
				self.InitalizeCreatorData(data);
			}
			self.parentNodeRegistrar(data);
			return new ConditionalFormattingIconSetDestination(importer, data as ConditionalFormattingIconSetCreatorData);
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtRuleDestination self = GetThis(importer);
			if (self.initData == null) {
				ConditionalFormattingCreatorInitData initData = new ConditionalFormattingCreatorInitData();
				initData.RuleType = ConditionalFormattingRuleType.IconSet;
				ConditionalFormattingCreatorData data = new ConditionalFormattingFormulaCreatorData(importer.CurrentWorksheet);
				self.InitalizeCreatorData(data);
				data.RuleType = self.RuleType;
				initData.Details = data;
				self.parentNodeRegistrar(data);
				self.initData = initData;
				return new ConditionalFormattingFormulaDestination(importer, self.FormulaAction);
			}
			else {
				ConditionalFormattingCreatorData data = self.initData.Details;
				if ( (data.Type != ConditionalFormattingType.Formula))
					return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
				self.InitalizeCreatorData(data);
			}
			return new ConditionalFormattingFormulaDestination(importer, self.FormulaAction);
		}
		static Destination OnDifferentialFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtRuleDestination self = GetThis(importer);
			self.dxfId = importer.StyleSheet.GetNextDifferentialFormatIndex();
			return new DifferentialFormatDestination(importer);
		}
	}
	#endregion
}
