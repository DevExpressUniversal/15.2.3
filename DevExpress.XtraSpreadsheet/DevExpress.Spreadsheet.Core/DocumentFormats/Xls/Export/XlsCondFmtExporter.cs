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
using System.IO;
using System.Text;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsCondFmtExporter
	public class XlsCondFmtExporter : XlsWorksheetExporterBase {
		XlsCondFmtGroupExporterBase groupExporter;
		XlsCondFmtGroupExporterBase group12Exporter;
		public XlsCondFmtExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, Worksheet sheet)
			: base(streamWriter, documentModel, exportStyleSheet, sheet) {
			groupExporter = new XlsCondFmtGroupExporter(this);
			group12Exporter = new XlsCondFmt12GroupExporter(this);
		}
		public override void WriteContent() {
			ConditionalFormattingGroupCollection groupCollection = new ConditionalFormattingGroupCollection();
			foreach(ConditionalFormatting item in Sheet.ConditionalFormattings)
				groupCollection.Register(item);
			RemoveGroupsOutOfXlsRange(groupCollection);
			RemoveIncompatibleGroups(groupCollection);
			XlsCondFmtGroupExporterBase groupExporter;
			int count = groupCollection.Count;
			for(int i = 0; i < count; i++) {
				groupExporter = GetGroupExporter(groupCollection[i]);
				groupExporter.WriteFormattingGroup(i + 1, groupCollection[i]);
			}
			for(int i = 0; i < count; i++) {
				groupExporter = GetGroupExporter(groupCollection[i]);
				groupExporter.WriteExtensions(i + 1, groupCollection[i]);
			}
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			this.groupExporter = null;
			this.group12Exporter = null;
		}
		#endregion
		#region Internals
		bool IsCondFmt12(List<ConditionalFormatting> group) {
			int count = group.Count;
			for(int i = 0; i < count; i++) {
				if(XlsCFExportHelper.IsNotCF12(group[i]))
					return false;
			}
			return true;
		}
		XlsCondFmtGroupExporterBase GetGroupExporter(List<ConditionalFormatting> group) {
			if(IsCondFmt12(group))
				return this.group12Exporter;
			return this.groupExporter;
		}
		void RemoveGroupsOutOfXlsRange(ConditionalFormattingGroupCollection groupCollection) {
			int count = groupCollection.Count;
			for(int i = count - 1; i >= 0; i--) {
				List<ConditionalFormatting> group = groupCollection[i];
				CellRangeBase range = group[0].CellRange;
				if(range.TopLeft.OutOfLimits())
					groupCollection.RemoveAt(i);
			}
		}
		void RemoveIncompatibleGroups(ConditionalFormattingGroupCollection groupCollection) {
			int count = groupCollection.Count;
			for (int i = count - 1; i >= 0; i--) {
				List<ConditionalFormatting> group = groupCollection[i];
				RemoveIncompatibleRules(group);
				if (group.Count == 0)
					groupCollection.RemoveAt(i);
			}
		}
		void RemoveIncompatibleRules(List<ConditionalFormatting> group) {
			int count = group.Count;
			for (int i = count - 1; i >= 0; i--) {
				List<ParsedExpression> expressions = group[i].GetExpressions(ExportStyleSheet.RPNContext);
				if (IsNotCompatibleExpressions(expressions))
					group.RemoveAt(i);
			}
		}
		bool IsNotCompatibleExpressions(List<ParsedExpression> expressions) {
			foreach (ParsedExpression expression in expressions) {
				if (!expression.IsXlsCFFormulaCompliant())
					return true;
			}
			return false;
		}
		#endregion
	}
	#endregion
	#region XlsCondFmtGroupExporterBase
	abstract class XlsCondFmtGroupExporterBase {
		XlsCondFmtExporter exporter;
		protected XlsCondFmtGroupExporterBase(XlsCondFmtExporter exporter) {
			this.exporter = exporter;
		}
		public XlsCondFmtExporter Exporter { get { return exporter; } }
		public void WriteFormattingGroup(int id, List<ConditionalFormatting> group) {
			if (GetRecordCount(group) > 0) {
				WriteFormattingRoot(id, group);
				WriteFormattingGroupCore(group);
			}
		}
		public void WriteExtensions(int id, List<ConditionalFormatting> group) {
			if(group.Count > 0)
				WriteExtensionsCore(id, group);
		}
		protected virtual void WriteFormattingGroupCore(List<ConditionalFormatting> group) {
			int count = group.Count;
			for(int i = 0; i < count; i++)
				WriteFormattingRule(group[i]);
		}
		protected virtual void WriteExtensionsCore(int id, List<ConditionalFormatting> group) {
		}
		void WriteFormattingRoot(int id, List<ConditionalFormatting> group) {
			XlsCommandConditionalFormat command = CreateRootCommand();
			command.Id = id;
			command.RecordCount = GetRecordCount(group);
			command.ToughRecalc = true;
			CellRangeBase cellRange = group[0].CellRange;
			command.BoundRange = GetBoundRangeRef8(cellRange);
			if(cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				int count = 0;
				foreach(CellRangeBase range in union.InnerCellRanges) {
					if(range.TopLeft.OutOfLimits()) continue;
					if(count >= XlsDefs.MaxCFRefCount) {
						break;
					}
					command.Ranges.Add(XlsRangeHelper.GetRef8(range));
					count++;
				}
			}
			else {
				command.Ranges.Add(XlsRangeHelper.GetRef8(cellRange));
			}
			command.Write(Exporter.StreamWriter);
		}
		protected abstract XlsCommandConditionalFormat CreateRootCommand();
		protected abstract void WriteFormattingRule(ConditionalFormatting cf);
		protected abstract int GetRecordCount(List<ConditionalFormatting> group);
		protected void WriteCF(ConditionalFormatting cf) {
			XlsCommandCF command = new XlsCommandCF();
			command.Format = DxfN12Info.FromDifferentialFormat(cf.DifferentialFormatInfo);
			IXlsCFRuleContentAdapter adapter = new XlsCFContentAdapter(command);
			PrepareContent(adapter, cf as FormulaConditionalFormatting);
			command.Write(Exporter.StreamWriter);
		}
		protected void WriteCF12(ConditionalFormatting cf) {
			XlsCommandCF12 command = new XlsCommandCF12();
			command.Priority = cf.Priority;
			command.StopIfTrue = cf.StopIfTrue;
			bool prepared = PrepareCF12Content(command, cf as FormulaConditionalFormatting);
			if(!prepared) prepared = PrepareCF12Content(command, cf as ColorScaleConditionalFormatting);
			if(!prepared) prepared = PrepareCF12Content(command, cf as DataBarConditionalFormatting);
			if(!prepared) PrepareCF12Content(command, cf as IconSetConditionalFormatting);
			command.Write(Exporter.StreamWriter);
		}
		protected void WriteCFEx(int id, ConditionalFormatting cf, bool isCF12, int cfIndex) {
			XlsCommandCFEx command = new XlsCommandCFEx();
			command.Id = id;
			command.IsCF12 = isCF12;
			command.Range = GetBoundRangeInfo(cf.CellRange);
			if(!isCF12) {
				command.Content.CFIndex = cfIndex;
				DxfN12Info format = DxfN12Info.FromDifferentialFormat(cf.DifferentialFormatInfo);
				if(format.ExtProperties.Count > 0) {
					command.Content.HasFormat = true;
					command.Content.Format = format;
				}
				else
					command.Content.HasFormat = false;
				command.Content.IsActive = true;
				command.Content.Priority = cf.Priority;
				command.Content.StopIfTrue = cf.StopIfTrue;
				IXlsCFRuleContentAdapter adapter = new XlsCFExNon12ContentAdapter(command.Content);
				PrepareContent(adapter, cf as FormulaConditionalFormatting);
			}
			command.Write(Exporter.StreamWriter);
		}
		#region Content by formatting
		bool PrepareCF12Content(XlsCommandCF12 command, FormulaConditionalFormatting cf) {
			if(cf == null) return false;
			command.Format = DxfN12Info.FromDifferentialFormat(cf.DifferentialFormatInfo);
			IXlsCFRuleContentAdapter adapter = new XlsCF12ContentAdapter(command, XlsCFExportHelper.HasFormulaInCF12(cf));
			PrepareContent(adapter, cf);
			return true;
		}
		bool PrepareCF12Content(XlsCommandCF12 command, ColorScaleConditionalFormatting cf) {
			if(cf == null) return false;
			command.RuleType = ConditionalFormattingRuleType.ColorScale;
			command.ComparisonFunction = ConditionalFormattingOperator.Unknown;
			command.RuleTemplate = XlsCFRuleTemplate.ColorScale;
			command.Format.IsEmpty = true;
			command.ColorScaleParams.Clamp = true;
			XlsRPNContext context = Exporter.ExportStyleSheet.RPNContext;
			ColorModelInfoCache colorCache = cf.DocumentModel.Cache.ColorModelInfoCache;
			XlsCFColorScalePoint point = new XlsCFColorScalePoint();
			point.CellValue = PrepareValueObject(cf.LowPointValue, context);
			point.ColorInfo = colorCache[cf.LowPointColorIndex];
			command.ColorScaleParams.Points.Add(point);
			if(cf.ScaleType == ColorScaleType.Color3) {
				point = new XlsCFColorScalePoint();
				point.CellValue = PrepareValueObject(cf.MiddlePointValue, context);
				point.ColorInfo = colorCache[cf.MiddlePointColorIndex];
				command.ColorScaleParams.Points.Add(point);
			}
			point = new XlsCFColorScalePoint();
			point.CellValue = PrepareValueObject(cf.HighPointValue, context);
			point.ColorInfo = colorCache[cf.HighPointColorIndex];
			command.ColorScaleParams.Points.Add(point);
			return true;
		}
		bool PrepareCF12Content(XlsCommandCF12 command, DataBarConditionalFormatting cf) {
			if(cf == null) return false;
			command.RuleType = ConditionalFormattingRuleType.DataBar;
			command.ComparisonFunction = ConditionalFormattingOperator.Unknown;
			command.RuleTemplate = XlsCFRuleTemplate.DataBar;
			command.Format.IsEmpty = true;
			XlsRPNContext context = Exporter.ExportStyleSheet.RPNContext;
			ColorModelInfoCache colorCache = cf.DocumentModel.Cache.ColorModelInfoCache;
			command.DataBarParams.ColorInfo = colorCache[cf.ColorIndex];
			command.DataBarParams.MaxValue = PrepareValueObject(cf.LowBound, context);
			command.DataBarParams.MinValue = PrepareValueObject(cf.HighBound, context);
			command.DataBarParams.PercentMin = cf.MinLength;
			command.DataBarParams.PercentMax = cf.MaxLength;
			command.DataBarParams.ShowBarOnly = !cf.ShowValue;
			return true;
		}
		bool PrepareCF12Content(XlsCommandCF12 command, IconSetConditionalFormatting cf) {
			if(cf == null) return false;
			command.RuleType = ConditionalFormattingRuleType.IconSet;
			command.ComparisonFunction = ConditionalFormattingOperator.Unknown;
			command.RuleTemplate = XlsCFRuleTemplate.IconSet;
			command.Format.IsEmpty = true;
			command.IconSetParams.IconSet = cf.IconSet;
			command.IconSetParams.IconsOnly = !cf.ShowValue;
			command.IconSetParams.Reverse = cf.Reversed;
			XlsRPNContext context = Exporter.ExportStyleSheet.RPNContext;
			int count = cf.ExpectedPointsNumber;
			for(int i = 0; i < count; i++) {
				ConditionalFormattingValueObject pointValue = cf.GetPointValue(i);
				XlsCFIconThreshold threshold = new XlsCFIconThreshold();
				threshold.Value = PrepareValueObject(pointValue, context);
				threshold.EqualPass = pointValue.IsGreaterOrEqual;
				command.IconSetParams.Thresholds.Add(threshold);
			}
			return true;
		}
		#endregion
		#region Content by comparer
		void PrepareContent(IXlsCFRuleContentAdapter content, FormulaConditionalFormatting cf) {
			bool prepared = PrepareContent(content, cf as ExpressionFormulaConditionalFormatting);
			if(!prepared) prepared = PrepareContent(content, cf as RangeFormulaConditionalFormatting);
			if(!prepared) prepared = PrepareContent(content, cf as TextFormulaConditionalFormatting);
			if(!prepared) prepared = PrepareContent(content, cf as SpecialFormulaConditionalFormatting);
			if(!prepared) prepared = PrepareContent(content, cf as TimePeriodFormulaConditionalFormatting);
			if(!prepared) prepared = PrepareContent(content, cf as AverageFormulaConditionalFormatting);
			if(!prepared) PrepareContent(content, cf as RankFormulaConditionalFormatting);
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, ExpressionFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			XlsRPNContext context = Exporter.ExportStyleSheet.RPNContext;
			context.WorkbookContext.PushSharedFormulaProcessing(true);
			context.WorkbookContext.PushCurrentCell(comparer.CellRange.TopLeft);
			try {
				switch (comparer.Condition) {
					case ConditionalFormattingExpressionCondition.ExpressionIsTrue:
						content.SetRuleType(ConditionalFormattingRuleType.ExpressionIsTrue);
						content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
						content.SetRuleTemplate(XlsCFRuleTemplate.Formula);
						content.SetFirstFormula(GetParsedExpression(comparer.Value, context), context);
						break;
					case ConditionalFormattingExpressionCondition.EqualTo:
					case ConditionalFormattingExpressionCondition.InequalTo:
					case ConditionalFormattingExpressionCondition.GreaterThan:
					case ConditionalFormattingExpressionCondition.GreaterThanOrEqual:
					case ConditionalFormattingExpressionCondition.LessThan:
					case ConditionalFormattingExpressionCondition.LessThanOrEqual:
						content.SetRuleType(ConditionalFormattingRuleType.CompareWithFormulaResult);
						content.SetComparisonFunction(comparer.Operator);
						content.SetRuleTemplate(XlsCFRuleTemplate.CellValue);
						content.SetFirstFormula(GetParsedExpression(comparer.Value, context), context);
						break;
				}
			}
			finally {
				context.WorkbookContext.PopCurrentCell();
				context.WorkbookContext.PopSharedFormulaProcessing();
			}
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, RangeFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.CompareWithFormulaResult);
			if(comparer.Condition == ConditionalFormattingRangeCondition.Inside)
				content.SetComparisonFunction(ConditionalFormattingOperator.Between);
			else
				content.SetComparisonFunction(ConditionalFormattingOperator.NotBetween);
			content.SetRuleTemplate(XlsCFRuleTemplate.CellValue);
			XlsRPNContext context = Exporter.ExportStyleSheet.RPNContext;
			content.SetFirstFormula(GetParsedExpression(comparer.Value, context), context);
			content.SetSecondFormula(GetParsedExpression(comparer.Value2, context), context);
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, TextFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.ExpressionIsTrue);
			content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
			content.SetRuleTemplate(XlsCFRuleTemplate.ContainsText);
			content.SetFirstFormula(comparer.GetRuleFormula(), Exporter.ExportStyleSheet.RPNContext);
			content.SetTextRule(comparer.Condition);
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, SpecialFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.ExpressionIsTrue);
			content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
			content.SetRuleTemplate(comparer.GetRuleTemplate());
			content.SetFirstFormula(comparer.GetRuleFormula(), Exporter.ExportStyleSheet.RPNContext);
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, TimePeriodFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.ExpressionIsTrue);
			content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
			content.SetRuleTemplate(comparer.GetRuleTemplate());
			content.SetFirstFormula(comparer.GetRuleFormula(), Exporter.ExportStyleSheet.RPNContext);
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, AverageFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.ExpressionIsTrue);
			content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
			content.SetRuleTemplate(comparer.GetRuleTemplate());
			content.SetFirstFormula(comparer.GetRuleFormula(), Exporter.ExportStyleSheet.RPNContext);
			content.SetStdDev(comparer.StdDev);
			return true;
		}
		bool PrepareContent(IXlsCFRuleContentAdapter content, RankFormulaConditionalFormatting comparer) {
			if(comparer == null) return false;
			content.SetRuleType(ConditionalFormattingRuleType.TopOrBottomValue);
			content.SetComparisonFunction(ConditionalFormattingOperator.Unknown);
			content.SetRuleTemplate(XlsCFRuleTemplate.Filter);
			content.SetFilterTop(comparer.Condition == ConditionalFormattingRankCondition.TopByPercent || comparer.Condition == ConditionalFormattingRankCondition.TopByRank);
			content.SetFilterPercent(comparer.Condition == ConditionalFormattingRankCondition.BottomByPercent || comparer.Condition == ConditionalFormattingRankCondition.TopByPercent);
			content.SetFilterValue(comparer.Rank);
			content.SetFirstFormula(comparer.GetRuleFormula(), Exporter.ExportStyleSheet.RPNContext);
			return true;
		}
		#endregion
		#region Utils
		XlsCFValueObject PrepareValueObject(ConditionalFormattingValueObject value, XlsRPNContext context) {
			XlsCFValueObject result = new XlsCFValueObject();
			switch(value.ValueType) {
				case ConditionalFormattingValueObjectType.Formula:
					result.ObjectType = ConditionalFormattingValueObjectType.Formula;
					SetFormula(result, value, context);
					break;
				case ConditionalFormattingValueObjectType.AutoMax:
				case ConditionalFormattingValueObjectType.Max:
					result.ObjectType = ConditionalFormattingValueObjectType.Max;
					break;
				case ConditionalFormattingValueObjectType.AutoMin:
				case ConditionalFormattingValueObjectType.Min:
					result.ObjectType = ConditionalFormattingValueObjectType.Min;
					break;
				case ConditionalFormattingValueObjectType.Num:
					result.ObjectType = ConditionalFormattingValueObjectType.Num;
					SetValueOrFormula(result, value, context);
					break;
				case ConditionalFormattingValueObjectType.Percent:
					result.ObjectType = ConditionalFormattingValueObjectType.Percent;
					SetValueOrFormula(result, value, context);
					break;
				case ConditionalFormattingValueObjectType.Percentile:
					result.ObjectType = ConditionalFormattingValueObjectType.Percentile;
					SetValueOrFormula(result, value, context);
					break;
			}
			return result;
		}
		void SetValueOrFormula(XlsCFValueObject result, ConditionalFormattingValueObject value, XlsRPNContext context) {
			ParsedExpression valueExpression = value.ValueExpression;
			if (valueExpression.Count == 1) {
				ParsedThingNumeric ptgNum = valueExpression[0] as ParsedThingNumeric;
				if (ptgNum != null) {
					result.Value = ptgNum.Value;
					return;
				}
			}
			SetFormula(result, value, context);
		}
		void SetFormula(XlsCFValueObject result, ConditionalFormattingValueObject value, XlsRPNContext context) {
			result.SetFormula(value.ValueExpression, context);
		}
		ParsedExpression GetParsedExpression(string formula, XlsRPNContext context) {
			if (!formula.StartsWith("=", StringComparison.Ordinal))
				formula = "=" + formula;
			return context.WorkbookContext.ParseExpression(formula, OperandDataType.Value, false);
		}
		CellRangeInfo GetBoundRangeInfo(CellRangeBase cellRange) {
			int minColumn = XlsDefs.MaxColumnCount;
			int minRow = XlsDefs.MaxRowCount;
			int maxColumn = 0;
			int maxRow = 0;
			if(cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				int count = 0;
				foreach(CellRangeBase range in union.InnerCellRanges) {
					if(range.TopLeft.OutOfLimits()) continue;
					if(count >= XlsDefs.MaxCFRefCount) break;
					CellRangeInfo rangeInfo = XlsRangeHelper.GetCellRangeInfo(range);
					minColumn = Math.Min(minColumn, rangeInfo.First.Column);
					minRow = Math.Min(minRow, rangeInfo.First.Row);
					maxColumn = Math.Max(maxColumn, rangeInfo.Last.Column);
					maxRow = Math.Max(maxRow, rangeInfo.Last.Row);
					count++;
				}
			}
			else {
				CellRangeInfo rangeInfo = XlsRangeHelper.GetCellRangeInfo(cellRange);
				minColumn = Math.Min(minColumn, rangeInfo.First.Column);
				minRow = Math.Min(minRow, rangeInfo.First.Row);
				maxColumn = Math.Max(maxColumn, rangeInfo.Last.Column);
				maxRow = Math.Max(maxRow, rangeInfo.Last.Row);
			}
			return new CellRangeInfo(new CellPosition(minColumn, minRow), new CellPosition(maxColumn, maxRow));
		}
		XlsRef8 GetBoundRangeRef8(CellRangeBase cellRange) {
			int minColumn = XlsDefs.MaxColumnCount;
			int minRow = XlsDefs.MaxRowCount;
			int maxColumn = 0;
			int maxRow = 0;
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				int count = 0;
				foreach (CellRangeBase range in union.InnerCellRanges) {
					if (range.TopLeft.OutOfLimits()) continue;
					if (count >= XlsDefs.MaxCFRefCount) break;
					CellRangeInfo rangeInfo = XlsRangeHelper.GetCellRangeInfo(range);
					minColumn = Math.Min(minColumn, rangeInfo.First.Column);
					minRow = Math.Min(minRow, rangeInfo.First.Row);
					maxColumn = Math.Max(maxColumn, rangeInfo.Last.Column);
					maxRow = Math.Max(maxRow, rangeInfo.Last.Row);
					count++;
				}
			}
			else {
				CellRangeInfo rangeInfo = XlsRangeHelper.GetCellRangeInfo(cellRange);
				minColumn = Math.Min(minColumn, rangeInfo.First.Column);
				minRow = Math.Min(minRow, rangeInfo.First.Row);
				maxColumn = Math.Max(maxColumn, rangeInfo.Last.Column);
				maxRow = Math.Max(maxRow, rangeInfo.Last.Row);
			}
			return new XlsRef8() { FirstColumnIndex = minColumn, FirstRowIndex = minRow, LastColumnIndex = maxColumn, LastRowIndex = maxRow };
		}
		#endregion
	}
	#endregion
	#region Group exporters (CF/CF12)
	class XlsCondFmtGroupExporter : XlsCondFmtGroupExporterBase {
		const int maxCFRecordCount = 3;
		int cfCount;
		public XlsCondFmtGroupExporter(XlsCondFmtExporter exporter)
			: base(exporter) {
		}
		protected override void WriteFormattingGroupCore(List<ConditionalFormatting> group) {
			cfCount = 0;
			base.WriteFormattingGroupCore(group);
		}
		protected override XlsCommandConditionalFormat CreateRootCommand() {
			return new XlsCommandConditionalFormat();
		}
		protected override void WriteFormattingRule(ConditionalFormatting cf) {
			if(cfCount >= maxCFRecordCount) return;
			if(XlsCFExportHelper.IsNotCF12(cf)) {
				WriteCF(cf);
				cfCount++;
			}
		}
		protected override void WriteExtensionsCore(int id, List<ConditionalFormatting> group) {
			cfCount = 0;
			int count = group.Count;
			for(int i = 0; i < count; i++) {
				ConditionalFormatting cf = group[i];
				if(XlsCFExportHelper.IsNotCF12(cf) && (cfCount < maxCFRecordCount)) {
					WriteCFEx(id, cf, false, cfCount);
					cfCount++;
				}
				else {
					WriteCFEx(id, cf, true, 0);
					WriteCF12(cf);
				}
			}
		}
		protected override int GetRecordCount(List<ConditionalFormatting> group) {
			int result = 0;
			for(int i = 0; i < group.Count; i++) {
				if(XlsCFExportHelper.IsNotCF12(group[i]))
					result++;
			}
			return Math.Min(maxCFRecordCount, result);
		}
	}
	class XlsCondFmt12GroupExporter : XlsCondFmtGroupExporterBase {
		public XlsCondFmt12GroupExporter(XlsCondFmtExporter exporter)
			: base(exporter) {
		}
		protected override XlsCommandConditionalFormat CreateRootCommand() {
			return new XlsCommandConditionalFormat12();
		}
		protected override void WriteFormattingRule(ConditionalFormatting cf) {
			IconSetConditionalFormatting iconCF = cf as IconSetConditionalFormatting;
			if (iconCF != null && !XlsCFHelper.IsSupportedIconSet(iconCF.IconSet))
				return;
			WriteCF12(cf);
		}
		protected override int GetRecordCount(List<ConditionalFormatting> group) {
			int result = 0;
			foreach (ConditionalFormatting cf in group) {
				IconSetConditionalFormatting iconCF = cf as IconSetConditionalFormatting;
				if (iconCF == null)
					result++;
				else if (XlsCFHelper.IsSupportedIconSet(iconCF.IconSet))
					result++;
			}
			return result;
		}
	}
	#endregion
	#region Rule content adapters
	public interface IXlsCFRuleContentAdapter {
		void SetRuleType(ConditionalFormattingRuleType ruleType);
		void SetComparisonFunction(ConditionalFormattingOperator comparisonFunction);
		void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate);
		void SetFirstFormula(ParsedExpression expression, XlsRPNContext context);
		void SetSecondFormula(ParsedExpression expression, XlsRPNContext context);
		void SetFilterTop(bool value);
		void SetFilterPercent(bool value);
		void SetFilterValue(int value);
		void SetTextRule(ConditionalFormattingTextCondition textRule);
		void SetStdDev(int value);
	}
	public class XlsCFContentAdapter : IXlsCFRuleContentAdapter {
		XlsCommandCF content;
		public XlsCFContentAdapter(XlsCommandCF command) {
			this.content = command;
		}
		#region IXlsCFRuleContentProxy Members
		public void SetRuleType(ConditionalFormattingRuleType ruleType) {
			if(ruleType == ConditionalFormattingRuleType.TopOrBottomValue)
				content.RuleType = ConditionalFormattingRuleType.ExpressionIsTrue;
			else
				content.RuleType = ruleType;
		}
		public void SetComparisonFunction(ConditionalFormattingOperator comparisonFunction) {
			content.ComparisonFunction = comparisonFunction;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
		}
		public void SetFirstFormula(ParsedExpression expression, XlsRPNContext context) {
			content.SetFirstFormula(expression, context);
		}
		public void SetSecondFormula(ParsedExpression expression, XlsRPNContext context) {
			content.SetSecondFormula(expression, context);
		}
		public void SetFilterTop(bool value) {
		}
		public void SetFilterPercent(bool value) {
		}
		public void SetFilterValue(int value) {
		}
		public void SetTextRule(ConditionalFormattingTextCondition textRule) {
		}
		public void SetStdDev(int value) {
		}
		#endregion
	}
	public class XlsCF12ContentAdapter : IXlsCFRuleContentAdapter {
		XlsCommandCF12 content;
		bool withFormula;
		public XlsCF12ContentAdapter(XlsCommandCF12 command, bool withFormula) {
			this.content = command;
			this.withFormula = withFormula;
		}
		#region IXlsCFRuleContentProxy Members
		public void SetRuleType(ConditionalFormattingRuleType ruleType) {
			content.RuleType = ruleType;
		}
		public void SetComparisonFunction(ConditionalFormattingOperator comparisonFunction) {
			content.ComparisonFunction = comparisonFunction;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
			content.RuleTemplate = ruleTemplate;
		}
		public void SetFirstFormula(ParsedExpression expression, XlsRPNContext context) {
			if(withFormula)
				content.SetFirstFormula(expression, context);
		}
		public void SetSecondFormula(ParsedExpression expression, XlsRPNContext context) {
			if(withFormula)
				content.SetSecondFormula(expression, context);
		}
		public void SetFilterTop(bool value) {
			content.FilterTop = false;
			content.FilterParams.Top = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetFilterPercent(bool value) {
			content.FilterPercent = false;
			content.FilterParams.Percent = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetFilterValue(int value) {
			content.FilterValue = 0;
			content.FilterParams.Value = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetTextRule(ConditionalFormattingTextCondition textRule) {
			content.TextRule = textRule;
		}
		public void SetStdDev(int value) {
			content.StdDev = value;
		}
		#endregion
	}
	public class XlsCFExNon12ContentAdapter : IXlsCFRuleContentAdapter {
		XlsCondFmtExtNonCF12 content;
		public XlsCFExNon12ContentAdapter(XlsCondFmtExtNonCF12 content) {
			this.content = content;
		}
		#region IXlsCFRuleContentProxy Members
		public void SetRuleType(ConditionalFormattingRuleType ruleType) {
		}
		public void SetComparisonFunction(ConditionalFormattingOperator comparisonFunction) {
			content.ComparisonFunction = comparisonFunction;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
			content.RuleTemplate = ruleTemplate;
		}
		public void SetFirstFormula(ParsedExpression expression, XlsRPNContext context) {
		}
		public void SetSecondFormula(ParsedExpression expression, XlsRPNContext context) {
		}
		public void SetFilterTop(bool value) {
			content.FilterTop = value;
		}
		public void SetFilterPercent(bool value) {
			content.FilterPercent = value;
		}
		public void SetFilterValue(int value) {
			content.FilterValue = value;
		}
		public void SetTextRule(ConditionalFormattingTextCondition textRule) {
			content.TextRule = textRule;
		}
		public void SetStdDev(int value) {
			content.StdDev = value;
		}
		#endregion
	}
	#endregion
}
