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
using DevExpress.Data.Filtering;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridFormatRuleMenuItems : FormatRuleMenuItems {
		protected static FormatRuleColumnInfo CreateFormatRuleColumnInfo(PivotGridViewInfoData data, PivotGridCellItem item) {
			Type dataType;
			if(item.DataField.UnboundType != DevExpress.Data.UnboundColumnType.Bound) {
				PivotGridFieldBase field = data.GetField(item.DataField);
				dataType = DevExpress.Data.PivotGrid.PivotSummaryValue.GetValueType(field.SummaryType, field.ActualDataType);
			} else
				dataType = item.DataField.DataType;	   
			if(dataType == null)
				dataType = typeof(object);
			if(dataType == typeof(object) && data.IsOLAP)
				dataType = typeof(double);
			return new FormatRuleColumnInfo() {
				Name = item.DataField.Caption,
				ColumnType = dataType,					  
				RepositoryItem = data.GetField(item.DataField).FieldEdit ?? data.PivotGrid.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string)),
				OwnerControl = data.PivotGrid
			};
		}												  
		readonly PivotGridCellItem cellItem;
		readonly PivotGridField field;
		readonly PivotGridViewInfoData data;
		public PivotGridFormatRuleMenuItems(PivotGridViewInfoData data, PivotGridCellItem cellItem, DXMenuItemCollection collection)
			: base(CreateFormatRuleColumnInfo(data, cellItem), collection) {
			this.cellItem = cellItem;
			this.field = data.GetField(cellItem.DataField);
			this.data = data;
			UpdateItems();
		}
		protected override void ManageColumnRulesItemClick(object sender, EventArgs e) {
			if(field == null) return;
			field.ShowFormatRulesManager();
		}
		List<ColumnNameInfo> GetColumnNames() {
			List<ColumnNameInfo> nameColumns = new List<ColumnNameInfo>();
			foreach(PivotGridField field in data.Fields) {
				ColumnNameInfo cni = new ColumnNameInfo() {
					Key = field.Name,
					Value = field.HeaderDisplayText,
					Visible = field.Visible && field.Options.ShowInCustomizationForm
				};
				if(!(nameColumns.Contains(cni) || string.IsNullOrEmpty(cni.Key))) nameColumns.Add(cni);
			}
			return nameColumns;					
		}
		protected override void DataBarItemClick(object sender, EventArgs e) {
			if(field == null || data == null)
				return;
			DXMenuItem item = sender as DXMenuItem;
			FormatConditionRuleDataBar rule = new FormatConditionRuleDataBar();
			data.FormatRules.BeginUpdate();
			RemoveRule(field, typeof(FormatConditionRuleDataBar), rule);
			rule.Appearance.Reset();
			rule.AppearanceNegative.Reset();
			rule.PredefinedName = string.Format("{0}", item.Tag);
			data.FormatRules.Add(field, rule, cellItem, true);
			data.FormatRules.EndUpdate();
		}
		protected override void IconSetItemClick(object sender, EventArgs e) {
			if(field == null || data == null)
				return;
			DXMenuItem item = sender as DXMenuItem;
			data.FormatRules.BeginUpdate();
			RemoveRule(field, typeof(FormatConditionRuleIconSet), null);
			FormatConditionRuleIconSet rule = new FormatConditionRuleIconSet();
			rule.IconSet = item.Tag as FormatConditionIconSet;
			data.FormatRules.Add(field, rule, cellItem, true);
			data.FormatRules.EndUpdate();
		}
		protected override void ColorScaleItemClick(object sender, EventArgs e) {
			if(field == null || data == null)
				return;
			DXMenuItem item = sender as DXMenuItem;
			data.FormatRules.BeginUpdate();
			RemoveRule(field, typeof(FormatConditionRule2ColorScale), null);
			RemoveRule(field, typeof(FormatConditionRule3ColorScale), null);
			data.FormatRules.Add(field, CreateColorScaleRule(string.Format("{0}", item.Tag)), cellItem, true);
			data.FormatRules.EndUpdate();
		}
		FormatConditionRule2ColorScale CreateColorScaleRule(string key) {
			FormatConditionRule2ColorScale rule;
			if(key.Split(',').Length > 2)
				rule = new FormatConditionRule3ColorScale();
			else
				rule = new FormatConditionRule2ColorScale();
			rule.PredefinedName = key;
			return rule;
		}
		void RemoveRule(PivotGridFieldBase field, Type type, FormatConditionRuleBase ruleBase) {
			RemoveRule(field, type, ruleBase, true);
		}
		void RemoveRule(PivotGridFieldBase field, Type type, FormatConditionRuleBase ruleBase, bool compareIntersection) {
			data.BeginUpdate();
			try {
				for(int i = data.FormatRules.Count - 1; i >= 0; i--) {
					PivotGridFormatRule rule = data.FormatRules[i];
					if(AllowRemove(field, rule, type, compareIntersection)) {
						if(ruleBase != null)
							ruleBase.Assign(rule.Rule);
						data.FormatRules.RemoveAt(i);
					}
				}
			} finally {
				data.EndUpdate();
			}
		}
		bool AllowRemove(PivotGridFieldBase field, PivotGridFormatRule rule, Type type, bool compareIntersection) {
			if(rule == null)
				return false;
			if(type == null && field == null)
				return true;
			if(type == null)
				return IsThisFieldRule(rule, compareIntersection);
			if(field == null)
				return rule.Rule.GetType() == type;
			return rule.Rule.GetType() == type && IsThisFieldRule(rule, compareIntersection);
		}
		bool IsThisFieldRule(PivotGridFormatRule rule, bool compareIntersection) {
			if(field != rule.Measure)
				return false;
			if(!compareIntersection)
				return true;
			FormatRuleFieldIntersectionSettings sett = rule.Settings as FormatRuleFieldIntersectionSettings;
			if(sett == null)
				return false;
			return sett.Row == data.GetField(cellItem.RowField) && sett.Column == data.GetField(cellItem.ColumnField);
		}
		protected override void ClearColumnRulesItemClick(object sender, EventArgs e) {
			if(field == null || data == null)
				return;
			RemoveRule(field, null, null);
		}
		protected override void ClearAllRulesItemClick(object sender, EventArgs e) {
			if(data == null)
				return;
			RemoveRule(null, null, null);
		}
		protected override bool IsRuleColumnExists {
			get {
				if(field == null || data == null)
					return false;
				foreach(PivotGridFormatRule rule in data.FormatRules)
					if(IsThisFieldRule(rule, false))
						return true;
				return false;
			}
		}
		protected override bool IsRuleExists {
			get {
				if(data == null)
					return false;
				return data.FormatRules.Count > 0;
			}
		}
		protected override void UpdateFormatConditionRules(FormatConditionRuleAppearanceBase rule, bool applyToLevel) {
			data.FormatRules.Add(field, rule, cellItem, applyToLevel);
		}
		protected override void CreateUniqueDuplicateRules() {
		}
		protected override void ExpressionBuilderCustomDisplayText(object sender, XtraEditors.Controls.CustomDisplayTextEventArgs e) {
			BaseEdit edit = sender as BaseEdit;
			CriteriaOperator criteria = e.Value as CriteriaOperator;
			if(!ReferenceEquals(criteria, null)) {
				e.DisplayText = PivotCriteriaProcessor.ToString(criteria, data);
			}
		}
		protected override FilterColumn GetDefaultFilterColumn(FilterColumnCollection fcCollection) {
			foreach(FormatRuleFieldFilterColumn fc in fcCollection)
				if(fc.FieldName == field.PrefilterColumnName)
					return fc;
			return null;
		}
		protected override FilterColumnCollection GetFilterColumns() {
			return data.FormatRules.GetFilterColumnCollection();
		}
		protected override void InitEditors(object sender, XtraEditors.FormatRule.Forms.FormatRuleEditFormBase form) {
			base.InitEditors(sender, form);
			DXMenuItem item = sender as DXMenuItem;
			bool condition = StringId.FormatRuleMenuItemEqualTo.Equals(item.Tag) || StringId.FormatRuleMenuItemGreaterThan.Equals(item.Tag) || 
							StringId.FormatRuleMenuItemLessThan.Equals(item.Tag) || StringId.FormatRuleMenuItemBetween.Equals(item.Tag) || 
					StringId.FormatRuleMenuItemTextThatContains.Equals(item.Tag) || StringId.FormatRuleMenuItemCustomCondition.Equals(item.Tag); 
			form.SetApplyToRowText(PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesIntersectionOnly), condition);
		}
		public void ConfigureClearFormatRulesItem(DXSubMenuItem item) {
			if(item == null)
				return;
			item.Items.Clear();
			if(IsRuleColumnExists) {
				DXMenuItem clearColumnItem = new DXMenuItem(PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearIntersectionRules), ClearColumnRulesItemClick);
				clearColumnItem.CloseMenuOnClick = false;
				item.Items.Add(clearColumnItem);
				clearColumnItem = new DXMenuItem(PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearMeasureRules), ClearWithoutIntersectionRulesItemClick);
				clearColumnItem.CloseMenuOnClick = false;
				item.Items.Add(clearColumnItem);
			}
			item.Items.Add(new DXMenuItem(PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearAllRules), ClearAllRulesItemClick));
		}
		protected void ClearWithoutIntersectionRulesItemClick(object sender, EventArgs e) {
			if(field == null || data == null)
				return;
			RemoveRule(field, null, null, false);
		}
	}
}
namespace DevExpress.XtraPivotGrid {
	public static class FormatRulesManagerExtension {
		public static void ShowFormatRulesManager(this PivotGridField field) {
			if(field == null) return;
			var data = field.PivotGrid.Data;
			if(data == null) return;
			var filterColumns = data.FormatRules.GetFilterColumnCollection();
			var filterColumnDefault = GetDefaultFilterColumn(filterColumns, field);
			using(ManagerRuleForm<PivotGridFormatRule, PivotGridField> manager =
				new DevExpress.XtraPivotGrid.ViewInfo.PivotManagerRuleForm(
					data.FormatRules,
					data.Fields,
					filterColumns,
					filterColumnDefault,
					field.Name,
					data.MenuManager,
					GetColumnNames(data))) {  
				manager.ShowDialog();
			}
		}
		static FilterColumn GetDefaultFilterColumn(FilterColumnCollection filterColumns, PivotGridField field) {
			foreach(FormatRuleFieldFilterColumn fc in filterColumns)
				if(fc.FieldName == field.PrefilterColumnName)
					return fc;
			return null;
		}
		static List<ColumnNameInfo> GetColumnNames(PivotGridViewInfoData data) {
			var nameColumns = new List<ColumnNameInfo>();
			foreach(PivotGridField field in data.Fields)
				if(!string.IsNullOrEmpty(field.Name))
					nameColumns.Add(new ColumnNameInfo() {
										Key = field.Name,
										Value = field.HeaderDisplayText,
										Name = field.Name,
										Visible = field.Visible && field.Options.ShowInCustomizationForm
									});
			return nameColumns;
		}
	}
}
