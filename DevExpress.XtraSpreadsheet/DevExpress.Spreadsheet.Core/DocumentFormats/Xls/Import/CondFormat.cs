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

using DevExpress.Utils;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office.Model;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCondFormat {
		CellRangeBase cellRange;
		readonly List<XlsCondFormatRule> rules = new List<XlsCondFormatRule>();
		public int Id { get; private set; }
		public bool ToughRecalc { get; private set; }
		public CellRangeBase CellRange { get { return cellRange; } }
		public IList<XlsCondFormatRule> Rules { get { return rules; } }
		public XlsCondFormat(int id, bool toughRecalc, CellRangeBase cellRange) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			Id = id;
			ToughRecalc = toughRecalc;
			this.cellRange = cellRange;
		}
		protected internal void Register(XlsContentBuilder contentBuilder) {
			foreach (XlsCondFormatRule rule in Rules)
				rule.Register(contentBuilder, this);
		}
	}
	public class XlsCondFormatRule {
		#region Fields
		XlsDifferentialFormatInfo formatInfo = new XlsDifferentialFormatInfo();
		ParsedExpression firstFormula = new ParsedExpression();
		ParsedExpression secondFormula = new ParsedExpression();
		ParsedExpression activeFormula = new ParsedExpression();
		XlsCFColorScaleParams colorScaleParams = new XlsCFColorScaleParams();
		XlsCFDatabarParams dataBarParams = new XlsCFDatabarParams();
		XlsCFFilterParams filterParams = new XlsCFFilterParams();
		XlsCFIconSetParams iconSetParams = new XlsCFIconSetParams();
		#endregion
		#region Properties
		public ConditionalFormattingRuleType RuleType { get; set; }
		public ConditionalFormattingOperator ComparisonFunction { get; set; }
		public ConditionalFormattingAverageCondition AverageCondition { get; set; }
		public ConditionalFormattingTimePeriod TimePeriod { get; set; }
		public XlsDifferentialFormatInfo FormatInfo { get { return formatInfo; } }
		public ParsedExpression FirstFormula { get { return firstFormula; } set { firstFormula = value; } }
		public ParsedExpression SecondFormula { get { return secondFormula; } set { secondFormula = value; } }
		public ParsedExpression ActiveFormula { get { return activeFormula; } set { activeFormula = value; } }
		public bool IsActive { get; set; }
		public bool StopIfTrue { get; set; }
		public int Priority { get; set; }
		public bool FilterTop { get; set; }
		public bool FilterPercent { get; set; }
		public int FilterValue { get; set; }
		public int StdDev { get; set; }
		public XlsCFColorScaleParams ColorScaleParams { get { return colorScaleParams; } set { colorScaleParams = value; } }
		public XlsCFDatabarParams DataBarParams { get { return dataBarParams; } set { dataBarParams = value; } }
		public XlsCFFilterParams FilterParams { get { return filterParams; } set { filterParams = value; } }
		public XlsCFIconSetParams IconSetParams { get { return iconSetParams; } set { iconSetParams = value; } }
		#endregion
		public XlsCondFormatRule() {
			RuleType = ConditionalFormattingRuleType.CompareWithFormulaResult;
			ComparisonFunction = ConditionalFormattingOperator.Between;
			AverageCondition = ConditionalFormattingAverageCondition.Above;
			TimePeriod = ConditionalFormattingTimePeriod.Unknown;
			IsActive = true;
		}
		protected internal void Register(XlsContentBuilder contentBuilder, XlsCondFormat hostFormat) {
			ConditionalFormatting cf = null;
			switch (RuleType) {
				case ConditionalFormattingRuleType.ContainsText:
				case ConditionalFormattingRuleType.NotContainsText:
				case ConditionalFormattingRuleType.BeginsWithText:
				case ConditionalFormattingRuleType.EndsWithText:
				case ConditionalFormattingRuleType.InsideDatePeriod:
				case ConditionalFormattingRuleType.CellIsBlank:
				case ConditionalFormattingRuleType.ContainsErrors:
				case ConditionalFormattingRuleType.CellIsNotBlank:
				case ConditionalFormattingRuleType.NotContainsErrors:
				case ConditionalFormattingRuleType.CompareWithFormulaResult:
				case ConditionalFormattingRuleType.ExpressionIsTrue:
				case ConditionalFormattingRuleType.TopOrBottomValue:
				case ConditionalFormattingRuleType.DuplicateValues:
				case ConditionalFormattingRuleType.UniqueValue:
				case ConditionalFormattingRuleType.AboveOrBelowAverage:
					cf = CreateFormulaConditionalFormatting(contentBuilder, hostFormat.CellRange);
					break;
				case ConditionalFormattingRuleType.ColorScale:
					cf = CreateColorScaleConditionalFormatting(contentBuilder, hostFormat.CellRange);
					break;
				case ConditionalFormattingRuleType.DataBar:
					cf = CreateDataBarConditionalFormatting(contentBuilder, hostFormat.CellRange);
					break;
				case ConditionalFormattingRuleType.IconSet:
					cf = CreateIconSetConditionalFormatting(contentBuilder, hostFormat.CellRange);
					break;
				case ConditionalFormattingRuleType.Unknown:
					contentBuilder.ThrowInvalidFile("Unknown conditional formatting rule type.");
					break;
			}
			if (cf != null) {
				ConditionalFormattingInfo info = cf.ConditionalFormattingInfo.Clone();
				info.StopIfTrue = StopIfTrue;
				int index = cf.Sheet.Workbook.Cache.ConditionalFormattingInfoCache.GetItemIndex(info);
				cf.ConditionalFormattingIndex = index;
				cf.Priority = Priority;
				if (cf.SupportsDifferentialFormat)
					cf.DifferentialFormatIndex = RegisterDifferentialFormat(contentBuilder);
				contentBuilder.CurrentSheet.ConditionalFormattings.AddWithPrioritiesCorrection(cf);
			}
		}
		ConditionalFormatting CreateFormulaConditionalFormatting(XlsContentBuilder contentBuilder, CellRangeBase cellRange) {
			Worksheet sheet = contentBuilder.CurrentSheet;
			sheet.DataContext.PushCurrentCell(cellRange.TopLeft);
			try {
				return CreateComparer(sheet, cellRange);
			}
			finally {
				sheet.DataContext.PopCurrentCell();
			}
		}
		ConditionalFormatting CreateColorScaleConditionalFormatting(XlsContentBuilder contentBuilder, CellRangeBase cellRange) {
			int count = ColorScaleParams.Points.Count;
			if (count < 2 || count > 3)
				return null;
			ConditionalFormattingValueObject[] valueObjects = new ConditionalFormattingValueObject[count];
			ColorModelInfo[] colors = new ColorModelInfo[count];
			for (int i = 0; i < count; i++) {
				valueObjects[i] = CreateValueObject(contentBuilder, ColorScaleParams.Points[i].CellValue);
				colors[i] = ColorScaleParams.Points[i].ColorInfo;
			}
			return new ColorScaleConditionalFormatting(contentBuilder.CurrentSheet, cellRange, valueObjects, colors);
		}
		ConditionalFormatting CreateDataBarConditionalFormatting(XlsContentBuilder contentBuilder, CellRangeBase cellRange) {
			ConditionalFormattingValueObject lowBound = CreateValueObject(contentBuilder, DataBarParams.MaxValue);
			ConditionalFormattingValueObject highBound = CreateValueObject(contentBuilder, DataBarParams.MinValue);
			DataBarConditionalFormatting cf = new DataBarConditionalFormatting(contentBuilder.CurrentSheet, cellRange, lowBound, highBound, DataBarParams.ColorInfo);
			cf.MinLength = DataBarParams.PercentMin;
			cf.MaxLength = DataBarParams.PercentMax;
			cf.AxisPosition = ConditionalFormattingDataBarAxisPosition.None; 
			cf.NegativeValueColor = cf.Color;
			cf.ShowValue = !DataBarParams.ShowBarOnly;
			return cf;
		}
		ConditionalFormatting CreateIconSetConditionalFormatting(XlsContentBuilder contentBuilder, CellRangeBase cellRange) {
			int count = IconSetParams.Thresholds.Count;
			if (count == 0)
				return null;
			ConditionalFormattingValueObject[] valueObjects = new ConditionalFormattingValueObject[count];
			for (int i = 0; i < count; i++) {
				XlsCFIconThreshold threshold = IconSetParams.Thresholds[i];
				valueObjects[i] = CreateValueObject(contentBuilder, threshold.Value, threshold.EqualPass);
			}
			IconSetConditionalFormatting cf = new IconSetConditionalFormatting(contentBuilder.CurrentSheet, cellRange, IconSetParams.IconSet, valueObjects);
			cf.Reversed = IconSetParams.Reverse;
			cf.ShowValue = !IconSetParams.IconsOnly;
			return cf;
		}
		#region Applies
		protected internal void ApplyFormat(DxfN12Info format, DocumentModel documentModel) {
			if (format.IsEmpty)
				return;
			format.DifferentialFormatInfo.AssignProperties(this.formatInfo, documentModel);
			if (format.ExtProperties.Count > 0)
				format.ExtProperties.ApplyContent(new XlsDifferentialFormatInfoAdapter(this.formatInfo));
		}
		protected internal void ApplyTemplate(XlsCFRuleTemplate template, ConditionalFormattingTextCondition textRule) {
			switch (template) {
				case XlsCFRuleTemplate.CellValue:
					RuleType = ConditionalFormattingRuleType.CompareWithFormulaResult;
					break;
				case XlsCFRuleTemplate.Formula:
					RuleType = ConditionalFormattingRuleType.ExpressionIsTrue;
					break;
				case XlsCFRuleTemplate.ColorScale:
					RuleType = ConditionalFormattingRuleType.ColorScale;
					break;
				case XlsCFRuleTemplate.DataBar:
					RuleType = ConditionalFormattingRuleType.DataBar;
					break;
				case XlsCFRuleTemplate.IconSet:
					RuleType = ConditionalFormattingRuleType.IconSet;
					break;
				case XlsCFRuleTemplate.Filter:
					RuleType = ConditionalFormattingRuleType.TopOrBottomValue;
					break;
				case XlsCFRuleTemplate.UniqueValues:
					RuleType = ConditionalFormattingRuleType.UniqueValue;
					break;
				case XlsCFRuleTemplate.DuplicateValues:
					RuleType = ConditionalFormattingRuleType.DuplicateValues;
					break;
				case XlsCFRuleTemplate.ContainsText:
					switch (textRule) {
						case ConditionalFormattingTextCondition.Contains:
							RuleType = ConditionalFormattingRuleType.ContainsText;
							break;
						case ConditionalFormattingTextCondition.NotContains:
							RuleType = ConditionalFormattingRuleType.NotContainsText;
							break;
						case ConditionalFormattingTextCondition.BeginsWith:
							RuleType = ConditionalFormattingRuleType.BeginsWithText;
							break;
						case ConditionalFormattingTextCondition.EndsWith:
							RuleType = ConditionalFormattingRuleType.EndsWithText;
							break;
					}
					break;
				case XlsCFRuleTemplate.ContainsBlanks:
					RuleType = ConditionalFormattingRuleType.CellIsBlank;
					break;
				case XlsCFRuleTemplate.ContainsNoBlanks:
					RuleType = ConditionalFormattingRuleType.CellIsNotBlank;
					break;
				case XlsCFRuleTemplate.ContainsErrors:
					RuleType = ConditionalFormattingRuleType.ContainsErrors;
					break;
				case XlsCFRuleTemplate.ContainsNoError:
					RuleType = ConditionalFormattingRuleType.NotContainsErrors;
					break;
				case XlsCFRuleTemplate.Today:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.Today;
					break;
				case XlsCFRuleTemplate.Tomorrow:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.Tomorrow;
					break;
				case XlsCFRuleTemplate.Yesterday:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.Yesterday;
					break;
				case XlsCFRuleTemplate.Last7Days:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.Last7Days;
					break;
				case XlsCFRuleTemplate.LastMonth:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.LastMonth;
					break;
				case XlsCFRuleTemplate.NextMonth:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.NextMonth;
					break;
				case XlsCFRuleTemplate.ThisWeek:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.ThisWeek;
					break;
				case XlsCFRuleTemplate.NextWeek:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.NextWeek;
					break;
				case XlsCFRuleTemplate.LastWeek:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.LastWeek;
					break;
				case XlsCFRuleTemplate.ThisMonth:
					RuleType = ConditionalFormattingRuleType.InsideDatePeriod;
					TimePeriod = ConditionalFormattingTimePeriod.ThisMonth;
					break;
				case XlsCFRuleTemplate.AboveAverage:
					RuleType = ConditionalFormattingRuleType.AboveOrBelowAverage;
					AverageCondition = ConditionalFormattingAverageCondition.Above;
					break;
				case XlsCFRuleTemplate.BelowAverage:
					RuleType = ConditionalFormattingRuleType.AboveOrBelowAverage;
					AverageCondition = ConditionalFormattingAverageCondition.Below;
					break;
				case XlsCFRuleTemplate.AboveOrEqualToAverage:
					RuleType = ConditionalFormattingRuleType.AboveOrBelowAverage;
					AverageCondition = ConditionalFormattingAverageCondition.AboveOrEqual;
					break;
				case XlsCFRuleTemplate.BelowOrEqualToAverage:
					RuleType = ConditionalFormattingRuleType.AboveOrBelowAverage;
					AverageCondition = ConditionalFormattingAverageCondition.BelowOrEqual;
					break;
			}
		}
		#endregion
		#region Arguments
		string GetFirstArgument(WorkbookDataContext context) {
			return "=" + FirstFormula.BuildExpressionString(context);
		}
		string GetSecondArgument(WorkbookDataContext context) {
			return "=" + SecondFormula.BuildExpressionString(context);
		}
		string GetConditionText() {
			foreach (IParsedThing ptg in FirstFormula) {
				ParsedThingStringValue ptgStr = ptg as ParsedThingStringValue;
				if (ptgStr != null)
					return ptgStr.Value;
			}
			return string.Empty;
		}
		#endregion
		#region Comparer creation
		FormulaConditionalFormatting CreateComparerFromOperator(Worksheet sheet, CellRangeBase cellRange) {
			string firstArgument = GetFirstArgument(sheet.DataContext);
			switch (ComparisonFunction) {
				case ConditionalFormattingOperator.BeginsWith:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.BeginsWith, firstArgument);
				case ConditionalFormattingOperator.Between:
					return new RangeFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingRangeCondition.Inside, firstArgument, GetSecondArgument(sheet.DataContext));
				case ConditionalFormattingOperator.ContainsText:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.Contains, firstArgument);
				case ConditionalFormattingOperator.EndsWith:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.EndsWith, firstArgument);
				case ConditionalFormattingOperator.Equal:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.EqualTo, firstArgument);
				case ConditionalFormattingOperator.GreaterThan:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.GreaterThan, firstArgument);
				case ConditionalFormattingOperator.GreaterThanOrEqual:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.GreaterThanOrEqual, firstArgument);
				case ConditionalFormattingOperator.LessThan:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.LessThan, firstArgument);
				case ConditionalFormattingOperator.LessThanOrEqual:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.LessThanOrEqual, firstArgument);
				case ConditionalFormattingOperator.NotBetween:
					return new RangeFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingRangeCondition.Outside, firstArgument, GetSecondArgument(sheet.DataContext));
				case ConditionalFormattingOperator.NotContains:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.NotContains, firstArgument);
				case ConditionalFormattingOperator.NotEqual:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.InequalTo, firstArgument);
			}
			return null;
		}
		FormulaConditionalFormatting CreateComparer(Worksheet sheet, CellRangeBase cellRange) {
			switch (RuleType) {
				case ConditionalFormattingRuleType.CompareWithFormulaResult:
					return CreateComparerFromOperator(sheet, cellRange);
				case ConditionalFormattingRuleType.ExpressionIsTrue:
					return new ExpressionFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingExpressionCondition.ExpressionIsTrue, GetFirstArgument(sheet.DataContext));
				case ConditionalFormattingRuleType.AboveOrBelowAverage:
					return new AverageFormulaConditionalFormatting(sheet, cellRange, AverageCondition, StdDev);
				case ConditionalFormattingRuleType.BeginsWithText:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.BeginsWith, GetConditionText());
				case ConditionalFormattingRuleType.ContainsText:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.Contains, GetConditionText());
				case ConditionalFormattingRuleType.EndsWithText:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.EndsWith, GetConditionText());
				case ConditionalFormattingRuleType.NotContainsText:
					return new TextFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingTextCondition.NotContains, GetConditionText());
				case ConditionalFormattingRuleType.CellIsBlank:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.ContainBlanks);
				case ConditionalFormattingRuleType.CellIsNotBlank:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.ContainNonBlanks);
				case ConditionalFormattingRuleType.ContainsErrors:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.ContainError);
				case ConditionalFormattingRuleType.DuplicateValues:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.ContainDuplicateValue);
				case ConditionalFormattingRuleType.InsideDatePeriod:
					return new TimePeriodFormulaConditionalFormatting(sheet, cellRange, TimePeriod);
				case ConditionalFormattingRuleType.NotContainsErrors:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.NotContainError);
				case ConditionalFormattingRuleType.TopOrBottomValue:
					return new RankFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingFormulaCreatorData.ConvertToRankCondition(FilterTop, FilterPercent), FilterValue);
				case ConditionalFormattingRuleType.UniqueValue:
					return new SpecialFormulaConditionalFormatting(sheet, cellRange, ConditionalFormattingSpecialCondition.ContainUniqueValue);
			}
			return null;
		}
		#endregion
		#region Utils
		int RegisterDifferentialFormat(XlsContentBuilder contentBuilder) {
			DifferentialFormat format = FormatInfo.GetDifferentialFormat(contentBuilder.StyleSheet);
			int formatIndex = contentBuilder.DocumentModel.Cache.CellFormatCache.AddItem(format);
			return formatIndex;
		}
		ConditionalFormattingValueObject CreateValueObject(XlsContentBuilder contentBuilder, XlsCFValueObject valueObject) {
			return CreateValueObject(contentBuilder, valueObject, true);
		}
		ConditionalFormattingValueObject CreateValueObject(XlsContentBuilder contentBuilder, XlsCFValueObject valueObject, bool gte) {
			ParsedExpression expression = valueObject.GetFormula(contentBuilder.RPNContext);
			if (expression.Count > 0) {
				string formula = expression.BuildExpressionString(contentBuilder.RPNContext.WorkbookContext);
				const NumberStyles allowedStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
				double d;
				if (double.TryParse(formula, allowedStyle, contentBuilder.DocumentModel.Culture, out d))
					return new ConditionalFormattingValueObject(contentBuilder.CurrentSheet, valueObject.ObjectType, d, gte);
				return new ConditionalFormattingValueObject(contentBuilder.CurrentSheet, valueObject.ObjectType, formula, gte);
			}
			return new ConditionalFormattingValueObject(contentBuilder.CurrentSheet, valueObject.ObjectType, valueObject.Value, gte);
		}
		#endregion
	}
}
