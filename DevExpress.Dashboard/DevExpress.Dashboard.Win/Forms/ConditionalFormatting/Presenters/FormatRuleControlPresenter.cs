#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	abstract class FormatRuleControlPresenter : FormatRulePresenterBase {
		static DashboardItemFormatRule CreateRule(DataDashboardItem dashboardItem, DataItem dataItem) {
			DashboardItemFormatRule formatRule = dashboardItem.FormatRulesInternal.CreateRule();
			((CellsItemFormatRule)formatRule).DataItem = dataItem;
			return formatRule;
		}
		protected static double ConvertToPercent(object value) {
			return Helper.ConvertToDouble(value);
		}
		readonly bool isNewRule;
		FormatRuleHistoryItemBase historyItem;
		DataItemWrapperList applyToDataItems;
		LevelDimensionItemWrapperList levelColumns;
		LevelDimensionItemWrapperList levelRows;
		LevelModeWrapperList levelModes;
		protected DashboardItemFormatRule Rule { get { return historyItem.NewFormatRule; } }
		protected abstract string DescriptionFormat { get; }
		protected virtual bool IsApplyToReadOnly { get { return (PivotItemRule != null) && !IsIntersectionLevel; } }
		protected virtual bool IsApplyToColumnSupported { get { return IsIntersectionLevel; } }
		protected sealed override IFormatRuleBaseViewInitializationContext ViewInitializationContext {
			get {
				FormatRuleControlViewInitializationContext context = CreateViewInitializationContext();
				context.Description = string.Format(DescriptionFormat, "<i>" + DataItem.DisplayName + "</i>");
				context.IsApplyToReadOnly = IsApplyToReadOnly;
				context.IsIntersectionLevel = IsIntersectionLevel;
				context.IsApplyToColumnSupported = IsApplyToColumnSupported;
				return context;
			}
		}
		IFormatRuleControlView SpecificView {
			get { return (IFormatRuleControlView)base.View; }
		}
		CellsItemFormatRule CellsItemRule { get { return (CellsItemFormatRule)Rule; } }
		PivotItemFormatRule PivotItemRule { get { return Rule as PivotItemFormatRule; } }
		bool IsIntersectionLevel { get { return (PivotItemRule != null) && (PivotItemRule.Condition is IEvaluatorRequired || DataItem is Measure); } }
		DataItemWrapperList ApplyToDataItems {
			get {
				if(applyToDataItems == null) {
					IList<DataItem> applyToAllowedDataItems = DashboardItem.GetConditionalFormattingApplyToAllowedDataItems(DataItem, Rule.Condition);
					applyToDataItems = new DataItemWrapperList(applyToAllowedDataItems);
				}
				return applyToDataItems;
			}
		}
		LevelDimensionItemWrapperList IntersectionLevelColumns {
			get {
				if(levelColumns == null && IsIntersectionLevel)
					levelColumns = new LevelDimensionItemWrapperList(PivotDashboardItem.Columns);
				return levelColumns;
			}
		}
		LevelDimensionItemWrapperList IntersectionLevelRows {
			get {
				if(levelRows == null && IsIntersectionLevel)
					levelRows = new LevelDimensionItemWrapperList(PivotDashboardItem.Rows);
				return levelRows;
			}
		}
		LevelModeWrapperList IntersectionLevelModes {
			get {
				if(levelModes == null) {
					levelModes = new LevelModeWrapperList();
					EnumManager.Iterate<FormatConditionIntersectionLevelMode>((type) => {
						if(!CellsItemRule.IsAggregationsRequired || (CellsItemRule.IsAggregationsRequired && type != FormatConditionIntersectionLevelMode.AllLevels))
							levelModes.Add(type);
					});
				}
				return levelModes;
			}
		}
		protected FormatRuleControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule) 
			: base(serviceProvider, dashboardItem, dataItem) {
			Guard.ArgumentNotNull(dataItem, "dataItem");
			this.isNewRule = (initialFormatRule == null);
			if(isNewRule) {
				this.historyItem = new CreateFormatRuleHistoryItem(DashboardItem, CreateRule(dashboardItem, dataItem));
			} else {
				this.historyItem = new EditFormatRuleHistoryItem(DashboardItem, initialFormatRule);
			}
		}
		protected virtual FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			return new FormatRuleControlViewInitializationContext();
		}
		protected override bool? InitializeView() {
			if(SpecificView != null) {
				if(IsIntersectionLevel) {
					IFormatRuleIntersectionLevel level = SpecificView.IntersectionLevel;
					level.SetModes(IntersectionLevelModes);
					level.SetAxis1Items(IntersectionLevelColumns);
					level.SetAxis2Items(IntersectionLevelRows);
					level.SelectedModeIndex = IntersectionLevelModes.IndexOf(PivotItemRule.IntersectionLevelMode);
					level.SelectedAxis1ItemIndex = IntersectionLevelColumns.IndexOf(PivotItemRule.Level.Column);
					level.SelectedAxis2ItemIndex = IntersectionLevelRows.IndexOf(PivotItemRule.Level.Row);
					level.Enable(PivotItemRule.IntersectionLevelMode == FormatConditionIntersectionLevelMode.SpecificLevel);
				}
				int applyToItemIndex = ApplyToDataItems.IndexOf(CellsItemRule.DataItemApplyTo ?? CellsItemRule.DataItem);
				if(applyToItemIndex < 0)
					applyToItemIndex = 0;
				SpecificView.SetItemsApplyTo(ApplyToDataItems);
				SpecificView.SelectedItemApplyToIndex = applyToItemIndex;
				RefreshApplyToRowColumn(applyToItemIndex, CellsItemRule.ApplyToRow, PivotItemRule != null ? PivotItemRule.ApplyToColumn : false);
			}
			return isNewRule;
		}
		protected override void ApplyView() {
			CellsItemFormatRule formatRule = CellsItemRule;
			DataItem dataItem = ApplyToDataItems[SpecificView.SelectedItemApplyToIndex];
			formatRule.DataItem = DataItem;
			if((formatRule.DataItemApplyTo == null && dataItem != DataItem) || formatRule.DataItemApplyTo != null)
				formatRule.DataItemApplyTo = dataItem;
			formatRule.ApplyToRow = (SpecificView.ApplyToRow == true);
			if(IsIntersectionLevel) {
				IFormatRuleIntersectionLevel level = SpecificView.IntersectionLevel;
				PivotItemFormatRule pivotRule = PivotItemRule;
				pivotRule.ApplyToColumn = (SpecificView.ApplyToColumn == true);
				pivotRule.IntersectionLevelMode = IntersectionLevelModes[level.SelectedModeIndex];
				pivotRule.Level.Column = IntersectionLevelColumns[level.SelectedAxis1ItemIndex];
				pivotRule.Level.Row = IntersectionLevelRows[level.SelectedAxis2ItemIndex];
			}
		}
		protected sealed override void ApplyHistory() {
			this.historyItem.SetRuleCaption(Caption);
			HistoryService.RedoAndAdd(this.historyItem);
			this.historyItem = new EditFormatRuleHistoryItem(DashboardItem, this.historyItem.InitialFormatRule);
		}
		protected override void SubscribeViewEvents() {
			base.SubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.ItemApplyToChanged += OnItemApplyToChanged;
				if(IsIntersectionLevel)
					SpecificView.IntersectionLevel.ModeChanged += OnIntersectionLevelModeChanged;
			}
		}
		protected override void UnsubscribeViewEvents() {
			base.UnsubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.ItemApplyToChanged -= OnItemApplyToChanged;
				if(IsIntersectionLevel)
					SpecificView.IntersectionLevel.ModeChanged -= OnIntersectionLevelModeChanged;
			}
		}
		protected void UpdateIntersectionLevelModes() {
			if(IsIntersectionLevel) {
				IFormatRuleIntersectionLevel level = SpecificView.IntersectionLevel;
				levelModes = null;
				level.SetModes(IntersectionLevelModes);
			}
		}
		void OnItemApplyToChanged(object sender, EventArgs e) {
			RefreshApplyToRowColumn(SpecificView.SelectedItemApplyToIndex, false, false);
		}
		void OnIntersectionLevelModeChanged(object sender, EventArgs e) {
			FormatConditionIntersectionLevelMode mode = IntersectionLevelModes[SpecificView.IntersectionLevel.SelectedModeIndex];
			SpecificView.IntersectionLevel.Enable(mode == FormatConditionIntersectionLevelMode.SpecificLevel);
		}
		void RefreshApplyToRowColumn(int selectedIndex, bool defaultApplyToRow, bool defaultApplyToColumn) {
			DataItem dataItem = ApplyToDataItems[selectedIndex];
			if(IsApplyToRestricted(dataItem)) {
				SpecificView.ApplyToRow = SpecificView.ApplyToColumn = null;
			} else {
				SpecificView.ApplyToRow = defaultApplyToRow;
				SpecificView.ApplyToColumn = defaultApplyToColumn;
			}
		}
		bool IsApplyToRestricted(DataItem dataItem) {
			return (PivotItemRule != null) && (dataItem is Dimension);
		}
	}
}
