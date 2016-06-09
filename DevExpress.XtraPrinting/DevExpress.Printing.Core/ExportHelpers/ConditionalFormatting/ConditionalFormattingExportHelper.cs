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

using DevExpress.Data.Filtering;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Printing.ExportHelpers {
	class ConditionalFormattingExporter<TCol, TRow> : ExportHelperBase<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		CriteriaOperatorToXlExpressionConverter converter;
		public ConditionalFormattingExporter(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo) {
		}
		CriteriaOperatorToXlExpressionConverter Converter {
			get {
				if(converter == null) {
					var columnTable = ColumnTableCreator<TCol>.Create(ExportInfo.GridColumns, gridColumn => gridColumn.LogicalPosition);
					converter = new CriteriaOperatorToXlExpressionConverter(new ColumnPositionConverter<TCol>(columnTable));
				}
				return converter;
			}
		}
		#region HelperMethods
		static bool CanDefaultValue(IFormatConditionRuleMinMaxBase rule) {
			return CanDefaultMinValue(rule) && CanDefaultMaxValue(rule);
		}
		static bool CanDefaultMinValue(IFormatConditionRuleMinMaxBase rule) {
			return rule.MinType == XlCondFmtValueObjectType.Percent && (decimal)rule.MinValue == 0m;
		}
		static bool CanDefaultMaxValue(IFormatConditionRuleMinMaxBase rule) {
			return rule.MaxType == XlCondFmtValueObjectType.Percent && (decimal)rule.MaxValue == 0m;
		}
		static bool CanDefaultMidValue(IFormatConditionRule3ColorScale gridRule) {
			return gridRule.MidpointType == XlCondFmtValueObjectType.Percent && (decimal)gridRule.MidpointValue == 0m;
		}
		static XlCondFmtAxisPosition GetDataBarAxisPosition(IFormatConditionRuleDataBar condFmtRDataBar) {
			XlCondFmtAxisPosition axisPosition = XlCondFmtAxisPosition.Automatic;
			if(!condFmtRDataBar.DrawAxis)
				axisPosition = XlCondFmtAxisPosition.None;
			else if(condFmtRDataBar.DrawAxisAtMiddle)
				axisPosition = XlCondFmtAxisPosition.Midpoint;
			return axisPosition;
		}
		static XlDataBarDirection GetDataBarDirection(IFormatConditionRuleDataBar condFmtRDataBar) {
			XlDataBarDirection direction = XlDataBarDirection.Context;
			switch(condFmtRDataBar.Direction) {
				case 0: direction = XlDataBarDirection.RightToLeft; break;
				case 1: direction = XlDataBarDirection.LeftToRight; break;
				case 2: direction = XlDataBarDirection.Context; break;
			}
			return direction;
		}
		static void SetMinMaxValue(IFormatConditionRuleMinMaxBase rule, XlCondFmtValueObject min, XlCondFmtValueObject max) {
			if(rule.MinValue == null && rule.MaxValue == null) return; 
			if(CanDefaultValue(rule)) return;
			if(!CanDefaultMinValue(rule)) {
				min.Value = XlValueObject.FromObject(rule.MinValue);
				min.ObjectType = rule.MinType;
			}
			if(!CanDefaultMaxValue(rule)) {
				max.Value = XlValueObject.FromObject(rule.MaxValue);
				max.ObjectType = rule.MaxType;
			}
		}
		static void SetMidValue(XlCondFmtRuleColorScale colorScale, IFormatConditionRule3ColorScale gridRule) {
			colorScale.MidpointValue.ObjectType = XlCondFmtValueObjectType.Percent;
			if(gridRule.MidpointValue == null) return;
			if(CanDefaultMidValue(gridRule)) return;
			colorScale.MidpointValue.Value = XlValueObject.FromObject(gridRule.MidpointValue);
			colorScale.MidpointValue.ObjectType = gridRule.MidpointType;
		}
		private void CalcRulePosition(XlConditionalFormatting cf, IFormatRuleBase rule) {
			IColumn positionCol = rule.ColumnApplyTo ?? rule.Column;
			foreach(var groupItem in ExportInfo.GroupsList) {
				int endPosition;
				if(object.Equals(ExportInfo.GroupsList.Last(), groupItem)) endPosition = groupItem.End - 1;
				else endPosition = groupItem.End;
				if(CheckPositionColumn(positionCol)) {
					int columnPositionLeft = rule.ApplyToRow ? 0 : positionCol.LogicalPosition;
					int columnPositionRight = rule.ApplyToRow ? ExportInfo.Exporter.CurrentColumnIndex - 1 : positionCol.LogicalPosition;
					cf.Ranges.Add(new XlCellRange(
					new XlCellPosition(columnPositionLeft, groupItem.Start),
					new XlCellPosition(columnPositionRight, endPosition)));
				}
			}
		}
		static bool CheckPositionColumn(IColumn positionCol){
			return positionCol != null && positionCol.GroupIndex==-1 && positionCol.IsVisible;
		}
		private XlCondFmtOperator TransformFormatConditionToXCondFmtOperator(FormatConditions fc) {
			switch(fc) {
				case FormatConditions.Between: return XlCondFmtOperator.Between;
				case FormatConditions.Equal: return XlCondFmtOperator.Equal;
				case FormatConditions.NotEqual: return XlCondFmtOperator.NotEqual;
				case FormatConditions.Greater: return XlCondFmtOperator.GreaterThan;
				case FormatConditions.GreaterOrEqual: return XlCondFmtOperator.GreaterThanOrEqual;
				case FormatConditions.Less: return XlCondFmtOperator.LessThan;
				case FormatConditions.LessOrEqual: return XlCondFmtOperator.LessThanOrEqual;
				case FormatConditions.NotBetween:
				default: return XlCondFmtOperator.NotBetween;
			}
		}
		string GetExpressionFromCondition(IFormatConditionRuleValue obj, string colName) {
			string res = string.Format("[{0}]", colName);
			switch(obj.Condition) {
				case FormatConditions.Equal: res += " = "; break;
				case FormatConditions.NotEqual: res += " != "; break;
				case FormatConditions.Greater: res += " > "; break;
				case FormatConditions.GreaterOrEqual: res += " >= "; break;
				case FormatConditions.Less: res += " < "; break;
				case FormatConditions.LessOrEqual: res += " <= "; break;
				case FormatConditions.Between: res += " between "; break;
				case FormatConditions.NotBetween: res += " notbetween "; break;
				default: return "";
			}
			return res + PrepareValues(obj);
		}
		string PrepareValues(IFormatConditionRuleValue obj) {
			string value1 = Convert.ToString(obj.Value1);
			string value2 = Convert.ToString(obj.Value2);
			CheckValue(obj.Value1, ref value1);
			CheckValue(obj.Value2, ref value2);
			if(obj.Condition == FormatConditions.Between ||
			   obj.Condition == FormatConditions.NotBetween)
				return string.Format("({0},{1})", value1, value2);
			else if(value1 == "") return value2;
			else if(value2 == "''") return value1;
			else return value1 + value2;
		}
		void CheckValue(object obj, ref string str) {
			if(obj != null) {
				if(obj.GetType() == typeof(string)) { str = String.Format("'{0}'", obj); }
			}
		}
		#endregion
		#region SimpleConditions
		#endregion SimpleConditions
		public void ExportFormatRules() {
			foreach(var fmtritem in ExportInfo.View.FormatRulesCollection) {
				if(fmtritem.Column == null) continue; 
				if(fmtritem.Rule is IFormatConditionRuleDataBar) { ExportDataBar(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleIconSet) { ExportIconSet(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleValue) { ExportFmtCondRuleValue(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleAboveBelowAverage) { ExportAboveBelowAverage(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleUniqueDuplicate) { ExportUniqueDuplicate(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRule3ColorScale) { ExportColorScale(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRule2ColorScale) { ExportColorScale(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleExpression) { ExportExpressionRule(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleTopBottom) { ExportTopBottomRule(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleContains) { ExportContainsRule(fmtritem); continue; }
				if(fmtritem.Rule is IFormatConditionRuleDateOccuring) { ExportDateOccuringRule(fmtritem); }
			}
		}
		void ExportContainsRule(IFormatRuleBase rule) {
			IFormatConditionRuleContains condFmtRC = rule.Rule as IFormatConditionRuleContains;
			XlConditionalFormatting cf = new XlConditionalFormatting();
			foreach(var value in condFmtRC.Values) {
				XlCondFmtRuleSpecificText exrule = new XlCondFmtRuleSpecificText(XlCondFmtSpecificTextType.Contains, value.ToString()) {
					Formatting = condFmtRC.Appearance
				};
				cf.Rules.Add(exrule);
			}
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportTopBottomRule(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleTopBottom condFmtRTB = rule.Rule as IFormatConditionRuleTopBottom;
			XlCondFmtRuleTop10 exrule = new XlCondFmtRuleTop10() {
				StopIfTrue = rule.StopIfTrue,
				Percent = condFmtRTB.Percent,
				Rank = condFmtRTB.Rank,
				Formatting = condFmtRTB.Appearance,
				Bottom = condFmtRTB.Bottom
			};
			cf.Rules.Add(exrule);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportIconSet(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleIconSet exrule = rule.Rule as IFormatConditionRuleIconSet;
			XlCondFmtRuleIconSet cfmtris = new XlCondFmtRuleIconSet() {
				IconSetType = exrule.IconSetType,
				Reverse = exrule.Reverse,
				ShowValues = exrule.ShowValues,
				Percent = exrule.Percent
			};
			if(exrule.Values.Count != 0) {
				cfmtris.Values.Clear();
				foreach(var icon in exrule.Values) cfmtris.Values.Add(icon);
			}
			cf.Rules.Add(cfmtris);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportFmtCondRuleValue(IFormatRuleBase rule) {
			IFormatConditionRuleValue condFmtRV = rule.Rule as IFormatConditionRuleValue;
			if(!string.IsNullOrEmpty(condFmtRV.Expression)) {
				ExportExpression(condFmtRV.Expression, condFmtRV.Appearance, rule);
			} else if(rule.ApplyToRow || rule.ColumnApplyTo!=null)
				ExportExpression(GetExpressionFromCondition(condFmtRV, rule.Column.FieldName), condFmtRV.Appearance, rule);
			else {
				XlConditionalFormatting cf = new XlConditionalFormatting();
				XlCondFmtRuleCellIs exrule = new XlCondFmtRuleCellIs {
					Operator = TransformFormatConditionToXCondFmtOperator(condFmtRV.Condition),
					Formatting = XlDifferentialFormatting.CopyObject(condFmtRV.Appearance)
				};
				exrule.Value = XlValueObject.FromObject(condFmtRV.Value1);
				exrule.SecondValue = XlValueObject.FromObject(condFmtRV.Value2);
				CalcRulePosition(cf, rule);
				cf.Rules.Add(exrule);
				ExportInfo.Sheet.ConditionalFormattings.Add(cf);
			}
		}
		void ExportExpressionRule(IFormatRuleBase rule) {
			IFormatConditionRuleExpression condFmtREx = rule.Rule as IFormatConditionRuleExpression;
			if(!string.IsNullOrEmpty(condFmtREx.Expression))
				ExportExpression(condFmtREx.Expression, condFmtREx.Appearance, rule);
		}
		void ExportExpression(string ruleExpression, XlDifferentialFormatting formatting, IFormatRuleBase rule) {
			CriteriaOperator co = CriteriaOperator.TryParse(ruleExpression);
			object cobj = co as object;
			if(cobj != null) {
				try {
					XlExpression expression = Converter.Execute(co);
					XlConditionalFormatting cf = new XlConditionalFormatting();
					XlCondFmtRuleExpression exrule = new XlCondFmtRuleExpression(expression) {
						Formatting = XlDifferentialFormatting.CopyObject(formatting)
					};
					CalcRulePosition(cf, rule);
					cf.Rules.Add(exrule);
					ExportInfo.Sheet.ConditionalFormattings.Add(cf);
				} catch(ExpressionConversionException) { }
			}
		}
		void ExportUniqueDuplicate(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleUniqueDuplicate exrule = rule.Rule as IFormatConditionRuleUniqueDuplicate;
			if(exrule.Duplicate) {
				XlCondFmtRuleDuplicates duplicates = new XlCondFmtRuleDuplicates() {
					Formatting = exrule.Formatting,
					StopIfTrue = rule.StopIfTrue
				};
				cf.Rules.Add(duplicates);
			} else {
				XlCondFmtRuleUnique unique = new XlCondFmtRuleUnique() {
					Formatting = exrule.Formatting,
					StopIfTrue = rule.StopIfTrue
				};
				cf.Rules.Add(unique);
			}
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportAboveBelowAverage(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleAboveBelowAverage condFmtRAB = rule.Rule as IFormatConditionRuleAboveBelowAverage;
			XlCondFmtRuleAboveAverage exrule = new XlCondFmtRuleAboveAverage() {
				StopIfTrue = rule.StopIfTrue,
				Condition = condFmtRAB.Condition,
				Formatting = condFmtRAB.Formatting
			};
			cf.Rules.Add(exrule);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportDataBar(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleDataBar condFmtRDataBar = rule.Rule as IFormatConditionRuleDataBar;
			XlCondFmtRuleDataBar exrule = new XlCondFmtRuleDataBar() {
				AxisColor = condFmtRDataBar.AxisColor,
				FillColor = condFmtRDataBar.FillColor,
				BorderColor = condFmtRDataBar.BorderColor,
				GradientFill = condFmtRDataBar.GradientFill,
				NegativeFillColor = condFmtRDataBar.NegativeFillColor,
				NegativeBorderColor = condFmtRDataBar.NegativeBorderColor,
				Direction = GetDataBarDirection(condFmtRDataBar),
				AxisPosition = GetDataBarAxisPosition(condFmtRDataBar)
			};
			SetMinMaxValue(condFmtRDataBar, exrule.MinValue, exrule.MaxValue);
			cf.Rules.Add(exrule);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportColorScale(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleColorScaleBase gridRule = rule.Rule as IFormatConditionRuleColorScaleBase;
			XlCondFmtColorScaleType type = rule.Rule is IFormatConditionRule3ColorScale ? XlCondFmtColorScaleType.ColorScale3 : XlCondFmtColorScaleType.ColorScale2;
			XlCondFmtRuleColorScale colorScale = new XlCondFmtRuleColorScale() {
				ColorScaleType = type,
				MaxColor = gridRule.MaxColor,
				MinColor = gridRule.MinColor
			};
			SetMinMaxValue(gridRule, colorScale.MinValue, colorScale.MaxValue);
			if(gridRule is IFormatConditionRule3ColorScale) {
				colorScale.MidpointColor = ((IFormatConditionRule3ColorScale)gridRule).MidpointColor;
				SetMidValue(colorScale, (IFormatConditionRule3ColorScale)gridRule);
			}
			cf.Rules.Add(colorScale);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
		void ExportDateOccuringRule(IFormatRuleBase rule) {
			XlConditionalFormatting cf = new XlConditionalFormatting();
			CalcRulePosition(cf, rule);
			IFormatConditionRuleDateOccuring exrule = rule.Rule as IFormatConditionRuleDateOccuring;
			XlCondFmtRuleTimePeriod cfmtris = new XlCondFmtRuleTimePeriod() {
				Formatting = exrule.Formatting,
				TimePeriod = exrule.DateType
			};
			cf.Rules.Add(cfmtris);
			ExportInfo.Sheet.ConditionalFormattings.Add(cf);
		}
	}
}
