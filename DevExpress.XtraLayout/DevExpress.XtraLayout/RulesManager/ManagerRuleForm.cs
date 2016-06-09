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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.XtraEditors.Frames {
	public partial class ManagerRuleForm<T, TColumnType> : XtraForm where T : FormatRuleBase, new() {
		protected FormatRuleCollection<T, TColumnType> ruleCollectionCore;
		protected CollectionBase columnsInfo;
		List<ColumnNameInfo> nameColumns;
		DataSourceStructViewInfo lbxRules;												  
		IDXMenuManager menuManager;										   
		protected ManagerRuleForm() {
			InitializeComponent();
			CreateDataSourceStructViewInfo();
			InitLocalizationText();				  
			UpdateStateButton();				 
			FocusRules();				
			SetSizeTouchMode(); 
		}
		public ManagerRuleForm(FormatRuleCollection<T, TColumnType> ruleCollection,
							   CollectionBase columnsInfo,
							   FilterColumnCollection filterColumns,
							   FilterColumn filterColumnDefault,
							   string defaultColumnName,
							   IDXMenuManager menuManager,
							   List<ColumnNameInfo> columnsName = null)
			: this() {
			ManagerRuleFormConvertedLayout.OptionsView.UseParentAutoScaleFactor = true;
			lbxRules.ScaleFactor = ((DevExpress.XtraLayout.ILayoutControl)ManagerRuleFormConvertedLayout).AutoScaleFactor;
			SetColumns(filterColumns, filterColumnDefault, columnsInfo, defaultColumnName, columnsName);
			SetRules(ruleCollection);
			SetMenuManager(menuManager);			  
		}
		void CreateDataSourceStructViewInfo() { 
			lbxRules = CreateDataSourceStructViewInfoCore();
			panelControl1ConvertedLayout.Controls.Add(lbxRules);
			layoutControlItem8.Control = lbxRules;
			lbxRules.AllowTouchScroll = true;
			lbxRules.Location = new System.Drawing.Point(2, 28);
			lbxRules.Name = "lbxRules";
			lbxRules.ScaleFactor = new System.Drawing.SizeF(1F, 1F);
			lbxRules.Size = new System.Drawing.Size(533, 176);
			lbxRules.TabIndex = 0;
			lbxRules.Click += new System.EventHandler(this.lbxRules_Click);
			lbxRules.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.btnEditRule_Click);
		}
		protected virtual DataSourceStructViewInfo CreateDataSourceStructViewInfoCore() {
			return new DevExpress.XtraEditors.Frames.DataSourceStructViewInfo();
		}
		protected DataSourceStructViewInfo RulesGrid { get { return lbxRules; } }
		void SetSizeTouchMode() {
			if(WindowsFormsSettings.TouchUIMode != DevExpress.LookAndFeel.TouchUIMode.True) return;
			int maxWidthButton = Math.Max(Math.Max(lciNewRule.Width, lciEditRule.Width), lciDeleteRule.Width);
			lciNewRule.Width = lciEditRule.Width = lciDeleteRule.Width = maxWidthButton;
			maxWidthButton = Math.Max(lciUp.Width, lciDown.Width);
			lciUp.Width = lciDown.Width = maxWidthButton;
			maxWidthButton = Math.Max(Math.Max(lciOK.Width, lciCancel.Width), lciApply.Width);
			lciNewRule.Width = lciEditRule.Width = lciDeleteRule.Width = maxWidthButton;
			Size = new System.Drawing.Size(650, 430);
		}
		void FocusRules() {
			lbxRules.Focus();
			lbxRules.Select();
		}
		void InitLocalizationText() {
			Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCaption);
			lciShowFormattingRules.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleShowFormattingRules);
			btnOK.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			btnCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
			btnApply.Text = Localizer.Active.GetLocalizedString(StringId.Apply);
			btnNewRule.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleNewRule);
			btnEditRule.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleEditRule);
			btnDeleteRule.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDeleteRule);
			btnUp.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleUp);
			btnDown.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDown);
		}
		void UpdateStateButton() {
			if(lbxRules.SelectedRowIndex == -1) {
				btnEditRule.Enabled = false;
				btnDeleteRule.Enabled = false;
				btnUp.Enabled = false;
				btnDown.Enabled = false;
			} else {
				btnEditRule.Enabled = true;
				btnDeleteRule.Enabled = true;
				btnUp.Enabled = true;
				btnDown.Enabled = true;
			}
			if(lbxRules.SelectedRowIndex == 0) btnUp.Enabled = false;
			if(lbxRules.SelectedRowIndex == lbxRules.FormatRules.Count - 1) btnDown.Enabled = false;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				if(newRuleFormCore != null) { newRuleFormCore.Dispose(); newRuleFormCore = null; }
			}
			base.Dispose(disposing);
		}
		void SetMenuManager(IDXMenuManager menuManager) {
			this.menuManager = menuManager;
			lbxRules.MenuManager = menuManager;
		}
		void SetRules(FormatRuleCollection<T, TColumnType> ruleCollection) {
			ruleCollectionCore = ruleCollection;
			lbxRules.SetRules(ruleCollectionCore);
		}
		FilterColumnCollection filterColumns;
		FilterColumn filterColumnDefault;
		void SetColumns(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault, CollectionBase columnsInfo, string defaultColumnName, List<ColumnNameInfo> columnsName) {
			this.filterColumns = filterColumns;
			this.filterColumnDefault = filterColumnDefault;
			this.nameColumns = columnsName;
			this.columnsInfo = columnsInfo;
			lbxRules.SetColumns(nameColumns, defaultColumnName);
			cmbInfoColumn.Properties.Items.Add(new ColumnNameInfo() {
				Key = Localizer.Active.GetLocalizedString(StringId.ManageRuleAllColumns),
				Value = Localizer.Active.GetLocalizedString(StringId.ManageRuleAllColumns)
			});
			foreach(var column in nameColumns) 
				if(column.Visible) cmbInfoColumn.Properties.Items.Add(column);
			cmbInfoColumn.SelectedIndex = 0;
			}
		protected virtual NewRuleForm CreateNewRuleForm() {
			return new NewRuleForm(filterColumns, filterColumnDefault, menuManager) { Owner = this };
		}
		NewRuleForm newRuleFormCore = null;
		public NewRuleForm NewRuleForm {
			get {
				if(newRuleFormCore == null) newRuleFormCore = CreateNewRuleForm();
				return newRuleFormCore;
			}
		}		
		void btnUp_Click(object sender, EventArgs e) {
			lbxRules.RowUp();
			UpdateStateButton();
		}
		void btnDown_Click(object sender, EventArgs e) {
			lbxRules.RowDown();
			UpdateStateButton();
		}
		void btnDeleteRule_Click(object sender, EventArgs e) {
			lbxRules.RowDelete();
			UpdateStateButton();
		}
		void btnEditRule_Click(object sender, EventArgs e) {
			var selectedRowIndex = lbxRules.SelectedRowIndex;
			if(selectedRowIndex < 0) return;
			ShowFormNewRule(lbxRules.FormatRules[selectedRowIndex], false);
		}
		void btnNewRule_Click(object sender, EventArgs e) {
			ShowFormNewRule(new FormatRule(), true);
		}
		void ShowFormNewRule(IFormatRule formatRule, bool isAdd) {
			NewRuleForm.LoadControl(formatRule as FormatRule);
			NewRuleForm.Text = string.Format("{0}", isAdd ? Localizer.Active.GetLocalizedString(StringId.NewFormattingRule)
														  : Localizer.Active.GetLocalizedString(StringId.EditFormattingRule));
			if(NewRuleForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				IFormatRule ruleCore = SetFieldsAssociatedWithSourceControl(formatRule, NewRuleForm.GetFormatRule(), isAdd);
				if(ruleCore != null) {
					if(!isAdd) {
						int index = lbxRules.FormatRules.IndexOf(formatRule);
						lbxRules.FormatRules.Remove(formatRule);
						lbxRules.FormatRules.Insert(index, ruleCore);
					} else
						lbxRules.FormatRules.Add(ruleCore);
				}
			}
			UpdateStateButton();
		}
		protected virtual IFormatRule SetFieldsAssociatedWithSourceControl(IFormatRule formatRule, IFormatRule rule, bool isAdd) {
			if(!isAdd) {
				rule.ColumnFieldName = formatRule.ColumnFieldName;
				((FormatRule)rule).ApplyToRow = ((FormatRule)formatRule).ApplyToRow; 
				rule.Enabled = formatRule.Enabled;
			} else {
				rule.ColumnFieldName = RulesGrid.DefaultColumnName;
				((FormatRule)rule).ApplyToRow = false;
				rule.Enabled = true;
			}
			return rule;									
		}
		protected TColumnType GetColumnByFieldName(string name) {
			int index = 0;
			foreach(ColumnNameInfo cni in nameColumns) {
				if(cni.Key == name) return (TColumnType)((IList)columnsInfo)[index];
				index++;
			}
			return default(TColumnType);
		}
		void btnOK_Click(object sender, EventArgs e) {
			SyncronizeChanges();
		}
		void btnApply_Click(object sender, EventArgs e) {
			SyncronizeChanges();
		}
		protected virtual void SyncronizeChanges() {
			ClearCollection();
			int i = AddingRulesInSourceControl(0, lbxRules.FormatRules);
			if(lbxRules.HiddenFormatRule.Count > 0)
				AddingRulesInSourceControl(i, lbxRules.HiddenFormatRule);
		}
		int AddingRulesInSourceControl(int i, IList<IFormatRule> formatRules) {
			foreach(FormatRule fr in formatRules) {
				ruleCollectionCore.Add(GetColumnByFieldName(fr.ColumnFieldName), fr.RuleBase);
				i = SetPropertyApplyToRow(i, fr);
			}
			return i;	  
		}
		int SetPropertyApplyToRow(int i, FormatRule fr) {
			var tcolumn = ((IList)ruleCollectionCore)[i++];
			var piProp = tcolumn.GetType().GetProperty("ApplyToRow", BindingFlags.Instance | BindingFlags.Public);
			piProp.SetValue(tcolumn, fr.ApplyToRow, null);
			piProp = tcolumn.GetType().GetProperty("Enabled", BindingFlags.Instance | BindingFlags.Public);
			piProp.SetValue(tcolumn, fr.Enabled, null);
			return i;
		}
		protected void ClearCollection() {
			foreach(var rule in ruleCollectionCore) rule.Rule = null;
			ruleCollectionCore.Clear();
		}
		void lbxRules_Click(object sender, EventArgs e) {
			UpdateStateButton();
		}
		void cmbInfoColumn_SelectedIndexChanged(object sender, EventArgs e) {
			lbxRules.FilterVisibleColumn(((ColumnNameInfo)cmbInfoColumn.SelectedItem).Key);
		}
	}
	public interface IFormatRule {
		FormatConditionRuleBase RuleBase { get; set; }
		RuleType RuleType { get; set; }
		string ColumnFieldName { get; set; }
		string RuleName { get; set; }
		bool Enabled { get; set; }
	}
	public class FormatRule : IFormatRule {
		bool? applyToRow;
		public FormatConditionRuleBase RuleBase { get; set; }
		public RuleType RuleType { get; set; }
		public string ColumnFieldName { get; set; }
		public string RuleName { get; set; }
		public bool Enabled { get; set; }
		public bool? ApplyToRow {
			get { return IsComplexRule(RuleBase) ? null : GetDefaultBoolValue(); }
			set { applyToRow = value; }
		}
		bool? GetDefaultBoolValue() {
			if(!applyToRow.HasValue) applyToRow = false;
			return applyToRow;
		}
		bool IsComplexRule(FormatConditionRuleBase rule) {
			return rule is FormatConditionRule2ColorScale ||
				   rule is FormatConditionRule3ColorScale ||
				   rule is FormatConditionRuleDataBar ||
				   rule is FormatConditionRuleIconSet;
		}
		public virtual void Assign(IFormatRule formatRule) {
			ColumnFieldName = formatRule.ColumnFieldName;
			if(formatRule.RuleBase != null)
				RuleBase = formatRule.RuleBase.CreateInstance();
			RuleBase.Assign(formatRule.RuleBase);
			RuleName = formatRule.RuleName;
			RuleType = formatRule.RuleType;
			Enabled = formatRule.Enabled;
		}
		public override bool Equals(object obj) {
			FormatRule fobj = obj as FormatRule;
			return fobj != null &&
				   RuleBase == fobj.RuleBase &&
				   RuleType == fobj.RuleType &&
				   ColumnFieldName == fobj.ColumnFieldName &&
				   RuleName == fobj.RuleName &&
				   Enabled == fobj.Enabled;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class ColumnNameInfo {
		public string Key { get; set; }
		public string Value { get; set; }
		public string Name { get; set; }
		public bool Visible { get; set; }
		public override string ToString() {
			return Value;
		}
	}
}
