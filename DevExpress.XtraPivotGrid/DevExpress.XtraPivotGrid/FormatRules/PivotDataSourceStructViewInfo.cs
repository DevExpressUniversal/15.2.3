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

using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	[ToolboxItem(false)]
	public class PivotDataSourceStructViewInfo : DataSourceStructViewInfo {
		const string grandTotalName = "XtraPivotGridGrandTotalField";
		const string anyFieldName = "XtraPivotGridAnyField";
		public PivotDataSourceStructViewInfo() : base() { }
		public string GrandTotalName { get { return grandTotalName; } }
		public string AnyFieldName { get { return anyFieldName; } }
		string GetRowFieldName(PivotGridFormatRule rule) {
			FormatRuleFieldIntersectionSettings sett = rule.Settings as FormatRuleFieldIntersectionSettings;
			if(sett == null)
				return anyFieldName;
			if(sett.Row == null)
				return grandTotalName;
			return sett.Row.Name;
		}
		string GetColumnFieldName(PivotGridFormatRule rule) {
			FormatRuleFieldIntersectionSettings sett = rule.Settings as FormatRuleFieldIntersectionSettings;
			if(sett == null)
				return anyFieldName;
			if(sett.Column == null)
				return grandTotalName;
			return sett.Column.Name;
		}
		internal void SetNames(PivotGridFormatRule rule, string rowName, string columnName) {
			if(rowName == anyFieldName || columnName == anyFieldName) {
				rule.Settings = new FormatRuleTotalTypeSettings();
				return;
			}
			FormatRuleFieldIntersectionSettings sett = new FormatRuleFieldIntersectionSettings();
			rule.Settings = sett;
			if(rowName == grandTotalName)
				sett.Row = null;
			else
				sett.RowName = rowName;
			if(columnName == grandTotalName)								
				sett.Column = null;
			else
				sett.ColumnName = columnName;
		}
		protected override RowViewInfo CreateHeaderViewInfo() {
			RowViewInfo viewInfo = new RowViewInfo(-1, ScaleFactor, 5);
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionRule)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionFormat)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesMeasure)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesColumn)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesColumn)));
			return viewInfo;
		}
		protected override RowViewInfo CreateRowViewInfo(IFormatRule rule, int index) {
			PivotFormatRule pivotRule = new PivotFormatRule();
			pivotRule.Assign((FormatRule)rule);
			pivotRule.ColumnField = ((PivotFormatRule)rule).ColumnField;
			pivotRule.RowField = ((PivotFormatRule)rule).RowField;
			RowViewInfo viewInfo = new RowViewInfo(index, ScaleFactor, 5) { Rule = pivotRule };
			viewInfo.Cells.Add(new RuleLabelViewInfo(pivotRule.RuleBase) { NameRule = pivotRule.RuleName });
			viewInfo.Cells.Add(new RulePreviewViewInfo(pivotRule.RuleBase));
			viewInfo.Cells.Add(CreateComboBoxViewInfo(pivotRule, pivotRule.ColumnFieldName, TypeComboBoxPivot.Measure));
			viewInfo.Cells.Add(CreateComboBoxViewInfo(pivotRule, pivotRule.RowField, TypeComboBoxPivot.Row));
			viewInfo.Cells.Add(CreateComboBoxViewInfo(pivotRule, pivotRule.ColumnField, TypeComboBoxPivot.Column));
			return viewInfo;
		}
		RuleComboBoxViewInfo CreateComboBoxViewInfo(IFormatRule rule, string field, TypeComboBoxPivot typeComboBox) {
			PivotFormatRule pivotRule = new PivotFormatRule();
			pivotRule.Assign(rule);
			var ruleComboBoxViewInfo = new PivotRuleComboBoxViewInfo(pivotRule.RuleBase, typeComboBox) { FieldName = field };
			string caption;
			ruleComboBoxViewInfo.Caption = ColumnsSource.TryGetValue(field, out caption) ? caption : string.Empty;
			return ruleComboBoxViewInfo;
		}
		public override void SetColumns(List<ColumnNameInfo> columnNames, string defaultColumnName) {
			ColumnsSource.Add(anyFieldName, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesAnyField));
			ColumnsSource.Add(grandTotalName, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesGrandTotal));
			base.SetColumns(columnNames, defaultColumnName);
		}
		public override void SetRules(IFormatRuleCollection ruleCollection) {
			var pivotRuleCollection = ruleCollection as PivotGridFormatRuleCollection;
			foreach(var item in pivotRuleCollection) {					 
				string ruleName = string.Empty;
				var fr = new PivotFormatRule() {
					ColumnFieldName = item.MeasureName,
					ColumnField = GetColumnFieldName(item),
					RowField = GetRowFieldName(item),
					RuleBase = item.Rule,
					RuleType = GetRuleType(item.Rule, out ruleName),
					RuleName = ruleName,
					Enabled = item.Enabled
				};
				if(!ColumnsSource.ContainsKey(fr.ColumnFieldName)) HiddenFormatRule.Add(fr);
				else formatRulesCore.Add(fr);
			}
		}
		protected override DXPopupMenu CreateMenu(IFormatRule rule, RuleComboBoxViewInfo vi) {
			DXPopupMenu columnsMenu = new DXPopupMenu();
			if(ColumnsSource == null) return columnsMenu;
			var typeComboBox = ((PivotRuleComboBoxViewInfo)vi).TypeComboBox;
			foreach(var item in ColumnsSource) {
				if(IsMeasureSpecific(typeComboBox, item.Key)) continue;
				DXMenuItem menuItem = new DXMenuItem(item.Value);
				menuItem.Click += (s, e) => { SetField(typeComboBox, rule, item.Key); UpdateRows(); };
				columnsMenu.Items.Add(menuItem);
			}
			return columnsMenu;
		}
		void SetField(TypeComboBoxPivot typeComboBox, IFormatRule rule, string key) {
			foreach(var item in formatRulesCore) {
				if(item.Equals(rule)) {
					var pivotRule = item as PivotFormatRule;
					switch(typeComboBox) {
						case TypeComboBoxPivot.Measure: pivotRule.ColumnFieldName = key; break;
						case TypeComboBoxPivot.Row: pivotRule.RowField = key; break;
						case TypeComboBoxPivot.Column: pivotRule.ColumnField = key; break;
					}
					return;
				}
			}
		}
		bool IsMeasureSpecific(TypeComboBoxPivot typeComboBox, string key) {
			return typeComboBox == TypeComboBoxPivot.Measure && (key == AnyFieldName || key == GrandTotalName);
		}
	}
	public enum TypeComboBoxPivot { Measure, Row, Column }
	public class PivotRuleComboBoxViewInfo : RuleComboBoxViewInfo {
		TypeComboBoxPivot typeComboBox;
		public TypeComboBoxPivot TypeComboBox { get { return typeComboBox; } }
		public PivotRuleComboBoxViewInfo(FormatConditionRuleBase owner, TypeComboBoxPivot typeComboBox) : base(owner) {
			this.typeComboBox = typeComboBox;
		}
	}
}
