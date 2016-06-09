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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region ConditionalFormattingCellRangeChangedHistoryItem
	public class ConditionalFormattingCellRangeHistoryItem : SpreadsheetSimpleTypeHistoryItem<CellRangeBase> {
		#region Fields
		readonly ConditionalFormatting conditionalFormatting;
		#endregion
		public ConditionalFormattingCellRangeHistoryItem(ConditionalFormatting conditionalFormatting, CellRangeBase oldRange, CellRangeBase newRange)
			: base(conditionalFormatting.DocumentModel.ActiveSheet, oldRange, newRange) {
			this.conditionalFormatting = conditionalFormatting;
		}
		#region Properties
		public ConditionalFormatting ConditionalFormatting { get { return conditionalFormatting; } }
		#endregion
		protected override void RedoCore() {
			ConditionalFormatting.SetCellRangeNoHistoryAndRebindComparer(NewValue);
			Workbook.IncrementContentVersion();
		}
		protected override void UndoCore() {
			ConditionalFormatting.SetCellRangeNoHistoryAndRebindComparer(OldValue);
			Workbook.IncrementContentVersion();
		}
	}
	#endregion
	#region ConditionalFormattingColorChangeHistoryItem
	public class ConditionalFormattingColorChangeHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly int itemIndex;
		readonly ConditionalFormatting conditionalFormatting;
		#endregion
		public ConditionalFormattingColorChangeHistoryItem(DocumentModel documentModel, ConditionalFormatting conditionalFormatting, int itemIndex, int oldValue, int newValue)
			: base(documentModel.ActiveSheet, oldValue, newValue) {
			this.itemIndex = itemIndex;
			this.conditionalFormatting = conditionalFormatting;
		}
		#region Properties
		public int ItemIndex { get { return itemIndex; } }
		public ConditionalFormatting ConditionalFormatting { get { return conditionalFormatting; } }
		#endregion
		#region value helper
		void ApplyDataBarColor(DataBarConditionalFormatting dataBar, int newValue) {
			if (dataBar != null) {
				switch (ItemIndex) {
					case DataBarConditionalFormatting.fillColorItemIndex:
						dataBar.ColorIndex = newValue;
						break;
					case DataBarConditionalFormatting.negativeFillColorItemIndex:
						dataBar.NegativeValueColorIndex = newValue;
						break;
					case DataBarConditionalFormatting.borderColorItemIndex:
						dataBar.BorderColorIndex = newValue;
						break;
					case DataBarConditionalFormatting.negativeBorderColorItemIndex:
						dataBar.NegativeValueBorderColorIndex = newValue;
						break;
					case DataBarConditionalFormatting.axisColorItemIndex:
						dataBar.AxisColorIndex = newValue;
						break;
				}
			}
		}
		void ApplyValue(int newValue) {
			switch (ConditionalFormatting.Type) {
				case ConditionalFormattingType.ColorScale:
					ColorScaleConditionalFormatting colorScale = ConditionalFormatting as ColorScaleConditionalFormatting;
					if (colorScale != null)
						colorScale.SetPointColorIndex((ColorScaleItemIndex)ItemIndex, newValue);
					break;
				case ConditionalFormattingType.DataBar:
					ApplyDataBarColor(ConditionalFormatting as DataBarConditionalFormatting, newValue);
					break;
			}
		}
		#endregion
		protected override void RedoCore() {
			ApplyValue(NewValue);
			Workbook.IncrementContentVersion();
		}
		protected override void UndoCore() {
			ApplyValue(OldValue);
			Workbook.IncrementContentVersion();
		}
	}
	#endregion
	#region ConditionalFormattingValueChangeHistoryItem
	public class ConditionalFormattingValueChangeHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly int itemIndex;
		readonly ConditionalFormatting conditionalFormatting;
		#endregion
		public ConditionalFormattingValueChangeHistoryItem(DocumentModel documentModel, ConditionalFormatting conditionalFormatting, int itemIndex, int oldValue, int newValue)
			: base(documentModel.ActiveSheet, oldValue, newValue) {
			this.itemIndex = itemIndex;
			this.conditionalFormatting = conditionalFormatting;
		}
		#region Properties
		public int ItemIndex { get { return itemIndex; } }
		public ConditionalFormatting ConditionalFormatting { get { return conditionalFormatting; } }
		#endregion
		#region value helper
		void ApplyValue(int newValue) {
			switch (ConditionalFormatting.Type) {
				case ConditionalFormattingType.ColorScale:
					ColorScaleConditionalFormatting colorScale = ConditionalFormatting as ColorScaleConditionalFormatting;
					if (colorScale != null)
						colorScale.SetPointValueIndex((ColorScaleItemIndex)ItemIndex, newValue);
					break;
				case ConditionalFormattingType.DataBar:
					DataBarConditionalFormatting dataBar = ConditionalFormatting as DataBarConditionalFormatting;
					if (dataBar != null)
						dataBar.SetBoundIndex((DataBarItemIndex)ItemIndex, newValue);
					break;
				case ConditionalFormattingType.IconSet:
					IconSetConditionalFormatting iconSet = ConditionalFormatting as IconSetConditionalFormatting;
					if (iconSet != null)
						iconSet.SetPointValueIndex(ItemIndex, newValue);
					break;
			}
		}
		#endregion
		protected override void RedoCore() {
			ApplyValue(NewValue);
			Workbook.IncrementContentVersion();
		}
		protected override void UndoCore() {
			ApplyValue(OldValue);
			Workbook.IncrementContentVersion();
		}
	}
	#endregion
	#region ConditionalFormattingFormatIndexChangeHistoryItem
	public class ConditionalFormattingFormatIndexChangeHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		#region Fields
		readonly ConditionalFormatting obj;
		#endregion
		public ConditionalFormattingFormatIndexChangeHistoryItem(ConditionalFormatting obj)
			: base(obj.DocumentModel) {
			this.obj = obj;
		}
		#region Properties
		protected ConditionalFormatting Object { get { return obj; } }
		#endregion
		protected override void UndoCore() {
			Object.SetIndexCore(ConditionalFormatting.DifferentialFormatIndexAccessor, OldIndex, ChangeActions);
			obj.DocumentModel.IncrementContentVersion();
		}
		protected override void RedoCore() {
			Object.SetIndexCore(ConditionalFormatting.DifferentialFormatIndexAccessor, NewIndex, ChangeActions);
			obj.DocumentModel.IncrementContentVersion();
		}
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null; 
		}
	}
	#endregion
	#region ConditionalFormattingIndexChangeHistoryItem
	public class ConditionalFormattingIndexChangeHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		#region Fields
		readonly ConditionalFormatting obj;
		#endregion
		public ConditionalFormattingIndexChangeHistoryItem(ConditionalFormatting obj)
			: base(obj.DocumentModel) {
			this.obj = obj;
		}
		#region Properties
		protected ConditionalFormatting Object { get { return obj; } }
		#endregion
		protected override void UndoCore() {
			Object.SetIndexCore(ConditionalFormatting.ConditionalFormattingInfoIndexAccessor, OldIndex, ChangeActions);
			obj.DocumentModel.IncrementContentVersion();
		}
		protected override void RedoCore() {
			Object.SetIndexCore(ConditionalFormatting.ConditionalFormattingInfoIndexAccessor, NewIndex, ChangeActions);
			obj.DocumentModel.IncrementContentVersion();
		}
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null; 
		}
	}
	#endregion
	#region ConditionalFormattingPriorityChangeHistoryItem
	public class ConditionalFormattingPriorityChangeHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly ConditionalFormatting conditionalFormatting;
		#endregion
		public ConditionalFormattingPriorityChangeHistoryItem(ConditionalFormatting conditionalFormatting, int oldValue, int newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void RedoCore() {
			conditionalFormatting.SetPriorityCore(NewValue);
		}
		protected override void UndoCore() {
			conditionalFormatting.SetPriorityCore(OldValue);
		}
	}
	#endregion
	#region MoveConditionalFormattingHistoryItem
	public class RemoveInsertNoApiInvalidationConditionalFormattingHistoryItem : SpreadsheetIntHistoryItem {
		public RemoveInsertNoApiInvalidationConditionalFormattingHistoryItem(Worksheet sheet, int oldValue, int newValue)
			: base(sheet, oldValue, newValue) {
		}
		protected override void RedoCore() {
			Worksheet.ConditionalFormattings.RemoveInsertNoApiInvalidationCore(OldValue, NewValue);
		}
		protected override void UndoCore() {
			Worksheet.ConditionalFormattings.RemoveInsertNoApiInvalidationCore(NewValue, OldValue);
		}
	}
	#endregion
	#region FormulaConditionalFormatting
	#region Condition
	public class ConditionalFormattingAverageConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingAverageCondition> {
		readonly AverageFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingAverageConditionHistoryItem(AverageFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingAverageCondition oldValue, ConditionalFormattingAverageCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingExpressionConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingExpressionCondition> {
		readonly ExpressionFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingExpressionConditionHistoryItem(ExpressionFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingExpressionCondition oldValue, ConditionalFormattingExpressionCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingRangeConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingRangeCondition> {
		readonly RangeFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingRangeConditionHistoryItem(RangeFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingRangeCondition oldValue, ConditionalFormattingRangeCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingRankConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingRankCondition> {
		readonly RankFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingRankConditionHistoryItem(RankFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingRankCondition oldValue, ConditionalFormattingRankCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingSpecialConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingSpecialCondition> {
		readonly SpecialFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingSpecialConditionHistoryItem(SpecialFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingSpecialCondition oldValue, ConditionalFormattingSpecialCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingTextConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingTextCondition> {
		readonly TextFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingTextConditionHistoryItem(TextFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingTextCondition oldValue, ConditionalFormattingTextCondition newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	public class ConditionalFormattingTimePeriodConditionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ConditionalFormattingTimePeriod> {
		readonly TimePeriodFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingTimePeriodConditionHistoryItem(TimePeriodFormulaConditionalFormatting conditionalFormatting, ConditionalFormattingTimePeriod oldValue, ConditionalFormattingTimePeriod newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void UndoCore() {
			conditionalFormatting.SetConditionCore(OldValue);
		}
		protected override void RedoCore() {
			conditionalFormatting.SetConditionCore(NewValue);
		}
	}
	#endregion
	public class ConditionalFormattingAverageStdDevHistoryItem : SpreadsheetIntHistoryItem {
		readonly AverageFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingAverageStdDevHistoryItem(AverageFormulaConditionalFormatting conditionalFormatting, int oldValue, int newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void RedoCore() {
			conditionalFormatting.SetStdDevCore(NewValue);
		}
		protected override void UndoCore() {
			conditionalFormatting.SetStdDevCore(OldValue);
		}
	}
	public class ConditionalFormattingRankRankHistoryItem : SpreadsheetIntHistoryItem {
		readonly RankFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingRankRankHistoryItem(RankFormulaConditionalFormatting conditionalFormatting, int oldValue, int newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void RedoCore() {
			conditionalFormatting.SetRankCore(NewValue);
		}
		protected override void UndoCore() {
			conditionalFormatting.SetRankCore(OldValue);
		}
	}
	public class ConditionalFormattingValueBasedValueHistoryItem : SpreadsheetSimpleTypeHistoryItem<ParsedExpression> {
		readonly ValueBasedFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingValueBasedValueHistoryItem(ValueBasedFormulaConditionalFormatting conditionalFormatting, ParsedExpression oldValue, ParsedExpression newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void RedoCore() {
			conditionalFormatting.SetValueCore(NewValue);
		}
		protected override void UndoCore() {
			conditionalFormatting.SetValueCore(OldValue);
		}
	}
	public class ConditionalFormattingValueBasedValue2HistoryItem : SpreadsheetSimpleTypeHistoryItem<ParsedExpression> {
		readonly RangeValueBasedFormulaConditionalFormatting conditionalFormatting;
		public ConditionalFormattingValueBasedValue2HistoryItem(RangeValueBasedFormulaConditionalFormatting conditionalFormatting, ParsedExpression oldValue, ParsedExpression newValue)
			: base(conditionalFormatting.Sheet, oldValue, newValue) {
			this.conditionalFormatting = conditionalFormatting;
		}
		protected override void RedoCore() {
			conditionalFormatting.SetValue2Core(NewValue);
		}
		protected override void UndoCore() {
			conditionalFormatting.SetValue2Core(OldValue);
		}
	}
	#endregion
}
