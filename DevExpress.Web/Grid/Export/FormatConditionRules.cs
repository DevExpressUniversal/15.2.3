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
using System.Drawing;
using System.Linq;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Web.Internal {
	public class GridExportCondition : IFormatRuleBase {
		public GridExportCondition(IGridView<GridXlsExportColumn, GridXlsExportRowBase> grid, GridFormatConditionBase condition) {
			Grid = grid;
			Condition = condition;
		}
		protected IGridView<GridXlsExportColumn, GridXlsExportRowBase> Grid { get; private set; }
		protected GridFormatConditionBase Condition { get; private set; }
		#region IFormatRuleBase Members
		string IFormatRuleBase.Name { get { return GetHashCode().ToString(); } }
		bool IFormatRuleBase.ApplyToRow { get { return Condition.ApplyToItem; } }
		string IFormatRuleBase.ColumnName { get { return Condition.FieldName; } }
		IColumn IFormatRuleBase.Column { get { return Grid.GetAllColumns().FirstOrDefault(c => c.DataColumn.FieldName == Condition.FieldName); } }
		IColumn IFormatRuleBase.ColumnApplyTo { get { return null; } }
		bool IFormatRuleBase.Enabled { get { return Condition.Enabled; } }
		bool IFormatRuleBase.StopIfTrue { get { return false; } }
		object IFormatRuleBase.Tag { get; set; }
		IFormatConditionRuleBase IFormatRuleBase.Rule { get { return Condition.GetExportRule(); } }
		#endregion
	}
	public class GridExportConditionRule3ColorScale : GridExportConditionRuleColorScaleBase, IFormatConditionRule3ColorScale {
		public GridExportConditionRule3ColorScale(GridFormatConditionColorScale condition)
			: base(condition) {
		}
		#region IFormatConditionRule3ColorScale Members
		Color IFormatConditionRule3ColorScale.MidpointColor { get { return Condition.GetMiddleColor(); } }
		XlCondFmtValueObjectType IFormatConditionRule3ColorScale.MidpointType { get { return XlCondFmtValueObjectType.Number; } }
		object IFormatConditionRule3ColorScale.MidpointValue {
			get {
				if(MinValue.HasValue && MaxValue.HasValue)
					return (MinValue.Value + MaxValue.Value) / 2;
				return null;
			}
		}
		#endregion
	}
	public class GridExportConditionRule2ColorScale : GridExportConditionRuleColorScaleBase, IFormatConditionRule2ColorScale {
		public GridExportConditionRule2ColorScale(GridFormatConditionColorScale condition)
			: base(condition) {
		}
	}
	public abstract class GridExportConditionRuleColorScaleBase : GridExportConditionRuleBase, IFormatConditionRuleColorScaleBase, IFormatConditionRuleMinMaxBase {
		public GridExportConditionRuleColorScaleBase(GridFormatConditionColorScale condition)
			: base(condition) {
		}
		protected new GridFormatConditionColorScale Condition { get { return (GridFormatConditionColorScale)base.Condition; } }
		protected decimal? MinValue { get { return DataUtils.ConvertToDecimalValue(Condition.MinimumValue); } }
		protected decimal? MaxValue { get { return DataUtils.ConvertToDecimalValue(Condition.MaximumColor); } }
		#region IFormatConditionRuleColorScaleBase Members
		Color IFormatConditionRuleColorScaleBase.MinColor { get { return Condition.GetMinimumColor(); } }
		Color IFormatConditionRuleColorScaleBase.MaxColor { get { return Condition.GetMaximumColor(); } }
		#endregion
		#region IFormatConditionRuleMinMaxBase Members
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MaxType { get { return XlCondFmtValueObjectType.Number; } }
		object IFormatConditionRuleMinMaxBase.MaxValue { get { return MaxValue; } }
		XlCondFmtValueObjectType IFormatConditionRuleMinMaxBase.MinType { get { return XlCondFmtValueObjectType.Number; } }
		object IFormatConditionRuleMinMaxBase.MinValue { get { return MinValue; } }
		#endregion
	}
	public class GridExportConditionRuleIconSet : GridExportConditionRuleBase, IFormatConditionRuleIconSet {
		public GridExportConditionRuleIconSet(GridFormatConditionIconSet condition)
			: base(condition) {
		}
		protected new GridFormatConditionIconSet Condition { get { return (GridFormatConditionIconSet)base.Condition; } }
		protected GridFormatConditionIconCollection Icons { get { return Condition.Icons; } }
		#region IFormatConditionRuleIconSet Members
		XlCondFmtIconSetType IFormatConditionRuleIconSet.IconSetType { get { return Icons.IconSet; } }
		bool IFormatConditionRuleIconSet.Percent { get { return Icons.ThresholdType == GridFormatConditionThresholdType.Percent; } }
		bool IFormatConditionRuleIconSet.Reverse { get { return false; } }
		bool IFormatConditionRuleIconSet.ShowValues { get { return true; } }
		IList<XlCondFmtValueObject> IFormatConditionRuleIconSet.Values {
			get {
				var values = new List<XlCondFmtValueObject>();
				foreach(GridFormatConditionIcon icon in Icons) {
					XlCondFmtValueObject valueObject = new XlCondFmtValueObject();
					valueObject.ObjectType = Icons.ThresholdType == GridFormatConditionThresholdType.Percent ? XlCondFmtValueObjectType.Percent : XlCondFmtValueObjectType.Number;
					valueObject.Value = icon.Threshold == decimal.MinValue ? 0 : (double)icon.Threshold;
					values.Insert(0, valueObject);
				}
				if(values.Count > 0)
					values[0].ObjectType = XlCondFmtValueObjectType.Percent;
				return values;
			}
		}
		#endregion
	}
	public class GridExportConditionValueRule : GridExportConditionExpressionRuleBase, IFormatConditionRuleValue {
		public GridExportConditionValueRule(GridFormatConditionHighlight condition)
			: base(condition) {
		}
		protected new GridFormatConditionHighlight Condition { get { return (GridFormatConditionHighlight)base.Condition; } }
		#region IFormatConditionRuleValue Members
		XlDifferentialFormatting IFormatConditionRuleValue.Appearance { get { return GetExportAppearance(); } }
		FormatConditions IFormatConditionRuleValue.Condition { get { return (FormatConditions)Condition.Rule; } }
		string IFormatConditionRuleValue.Expression { get { return Condition.Expression; } }
		object IFormatConditionRuleValue.Value1 { get { return Condition.Value1; } }
		object IFormatConditionRuleValue.Value2 { get { return Condition.Value2; } }
		#endregion
	}
	public class GridExportConditionTopBottomRule : GridExportConditionExpressionRuleBase, IFormatConditionRuleTopBottom {
		const int
			MaxPercentValue = 100,
			MaxItemCount = 1000;
		public GridExportConditionTopBottomRule(GridFormatConditionTopBottom condition)
			: base(condition) {
		}
		protected new GridFormatConditionTopBottom Condition { get { return (GridFormatConditionTopBottom)base.Condition; } }
		int GetExportedRank() {
			decimal rank = Math.Max(1, Condition.Threshold);
			rank = Math.Min(rank, Condition.IsPercentRule ? MaxPercentValue : MaxItemCount);
			return Convert.ToInt32(rank);
		}
		#region IFormatConditionRuleTopBottom Members
		XlDifferentialFormatting IFormatConditionRuleTopBottom.Appearance { get { return GetExportAppearance(); } }
		bool IFormatConditionRuleTopBottom.Bottom { get { return !Condition.IsTopRule; } }
		bool IFormatConditionRuleTopBottom.Percent { get { return Condition.IsPercentRule; } }
		int IFormatConditionRuleTopBottom.Rank { get { return GetExportedRank(); } }
		#endregion
	}
	public class GridExportConditionAboveBelowAverageRule : GridExportConditionExpressionRuleBase, IFormatConditionRuleAboveBelowAverage {
		public GridExportConditionAboveBelowAverageRule(GridFormatConditionTopBottom condition)
			: base(condition) {
		}
		protected new GridFormatConditionTopBottom Condition { get { return (GridFormatConditionTopBottom)base.Condition; } }
		#region IFormatConditionRuleAboveBelowAverage Members
		XlCondFmtAverageCondition IFormatConditionRuleAboveBelowAverage.Condition {
			get {
				if(Condition.Rule == GridTopBottomRule.AboveAverage)
					return XlCondFmtAverageCondition.Above;
				return XlCondFmtAverageCondition.Below;
			}
		}
		XlDifferentialFormatting IFormatConditionRuleAboveBelowAverage.Formatting { get { return GetExportAppearance(); } }
		#endregion
	}
	public class GridExportConditionExpressionRuleBase : GridExportConditionRuleBase {
		public GridExportConditionExpressionRuleBase(GridFormatConditionExpressionBase condition)
			: base(condition) {
		}
		protected new GridFormatConditionExpressionBase Condition { get { return (GridFormatConditionExpressionBase)base.Condition; } }
		protected AppearanceStyle ItemCellStyle {
			get {
				if(Condition.ApplyToItem)
					return Condition.GetItemStyle(null, -1);
				return Condition.GetItemCellStyle(null, -1);
			}
		}
		protected XlDifferentialFormatting GetExportAppearance() {
			XlDifferentialFormatting appearance = new XlDifferentialFormatting();
			appearance.Fill = new XlFill {
				BackColor = ItemCellStyle.BackColor,
				ForeColor = ItemCellStyle.ForeColor,
				PatternType = XlPatternType.Solid
			};
			appearance.Font = CreateExportFont();
			return appearance;
		}
		XlFont CreateExportFont() {
			XlFont font = new XlFont();
			font.Color = ItemCellStyle.ForeColor;
			if(ItemCellStyle.Font.Underline)
				font.Underline = XlUnderlineType.Single;
			font.Bold = ItemCellStyle.Font.Bold;
			font.StrikeThrough = ItemCellStyle.Font.Strikeout;
			font.Italic = ItemCellStyle.Font.Italic;
			return font;
		}
	}
	public abstract class GridExportConditionRuleBase : IFormatConditionRuleBase {
		public GridExportConditionRuleBase(GridFormatConditionBase condition) {
			Condition = condition;
		}
		protected GridFormatConditionBase Condition { get; private set; }
	}
}
