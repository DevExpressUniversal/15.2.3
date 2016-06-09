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
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Frames;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotManagerRuleForm : ManagerRuleForm<PivotGridFormatRule, PivotGridField> {
		PivotManagerRuleForm() { }
		public PivotManagerRuleForm(PivotGridFormatRuleCollection ruleCollection, CollectionBase columnsInfo, FilterColumnCollection filterColumns,
									FilterColumn filterColumnDefault, string defaultColumnName, IDXMenuManager menuManager, List<ColumnNameInfo> columnsName)
			: base(ruleCollection, columnsInfo, filterColumns, filterColumnDefault, defaultColumnName, menuManager, columnsName) { }
		protected override DataSourceStructViewInfo CreateDataSourceStructViewInfoCore() {				   
			return new PivotDataSourceStructViewInfo();											
		}														  
		protected override IFormatRule SetFieldsAssociatedWithSourceControl(IFormatRule formatRule, IFormatRule rule, bool isAdd) {
			PivotFormatRule ruleCore = new PivotFormatRule();
			ruleCore.Assign(rule);
			if(!isAdd) {
				var formatRuleSource = formatRule as PivotFormatRule;
				ruleCore.ColumnField = formatRuleSource.ColumnField;
				ruleCore.RowField = formatRuleSource.RowField;
				ruleCore.Enabled = formatRuleSource.Enabled;
			} else {
				ruleCore.ColumnFieldName = RulesGrid.DefaultColumnName;
				ruleCore.ColumnField = ((PivotDataSourceStructViewInfo)RulesGrid).AnyFieldName;
				ruleCore.RowField = ((PivotDataSourceStructViewInfo)RulesGrid).AnyFieldName;
				ruleCore.Enabled = true;
			}
			return ruleCore; 
		}
		protected override void SyncronizeChanges() {
			ruleCollectionCore.BeginUpdate();
			ClearCollection();
			AddingRulesInSourceControl(RulesGrid.FormatRules);
			if(RulesGrid.HiddenFormatRule.Count > 0)
				AddingRulesInSourceControl(RulesGrid.HiddenFormatRule);
			ruleCollectionCore.EndUpdate();
		}
		private void AddingRulesInSourceControl(IList<IFormatRule> rules) {
			foreach(PivotFormatRule rule in rules) {
				PivotGridFormatRule gridRule = new PivotGridFormatRule();
				((PivotGridFormatRuleCollection)ruleCollectionCore).Add(gridRule);
				((PivotDataSourceStructViewInfo)RulesGrid).SetNames(gridRule, rule.RowField, rule.ColumnField);
				gridRule.Rule = rule.RuleBase;
				var fieldCollection = (PivotGridFieldCollection)columnsInfo;
				var field = fieldCollection.GetFieldByName(rule.ColumnFieldName);
				gridRule.Enabled = rule.Enabled;
				gridRule.Measure = field;
			}
		}
	}
	public class PivotFormatRule : FormatRule {
		public string RowField { get; set; }
		public string ColumnField { get; set; }
		public override void Assign(IFormatRule formatRule) {
			base.Assign(formatRule);
			if(formatRule is PivotFormatRule) {
				var pFormatRule = formatRule as PivotFormatRule;
				RowField = pFormatRule.RowField;
			}
		}
		public override bool Equals(object obj) {
			FormatRule fobj = obj as FormatRule;
			PivotFormatRule pobj = obj as PivotFormatRule;
			return fobj != null && pobj != null &&
				   base.Equals(fobj) &&
				   ColumnField == pobj.ColumnField &&
				   RowField == pobj.RowField;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
