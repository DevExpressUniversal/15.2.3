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
using System.Linq;
using System.Text;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Repository;
using DevExpress.Data.Summary;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.FormatRule.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using System.Reflection;
namespace DevExpress.XtraEditors {
	public class FormatRuleColumnInfo {
		public string Name { get; set; }
		public Type ColumnType { get; set; }
		public RepositoryItem RepositoryItem { get; set; }
		public ISupportLookAndFeel OwnerControl { get; set; }
		[Obsolete("Use MenuManager"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public IDXMenuManager MenuManeger { get { return MenuManager; } set { MenuManager = value; } }
		public IDXMenuManager MenuManager { get; set; }
		public bool IsServerMode { get; set; }
	}
	public class FormatRuleMenuOptions {
		public static FilterEditorViewMode FilterEditorViewMode = XtraEditors.FilterEditorViewMode.Visual; 
	}
	public class FormatRuleMenuHelper {
		public static object ConstructorInfoInvoker(string constructorAssembly, string constructorClass) {
			try {
				Type constructorType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType(constructorAssembly, constructorClass);
				if(constructorType != null) {
					ConstructorInfo constructorInfoObj = constructorType.GetConstructor(Type.EmptyTypes);
					if(constructorInfoObj != null) {
						return constructorInfoObj.Invoke(null);
					}
				}
			} catch { }
			return null;
		}
		public static IFilterControl CreateFilterControl() { return CreateFilterControl(FormatRuleMenuOptions.FilterEditorViewMode); }
		public static IFilterControl CreateFilterControl(FilterEditorViewMode mode) {
			IFilterControl ret = null;
			if(mode != XtraEditors.FilterEditorViewMode.Visual) {
				ret = ConstructorInfoInvoker(
					AssemblyInfo.SRAssemblyRichEdit + ", Version=" + AssemblyInfo.Version,
					"DevExpress.XtraFilterEditor.FilterEditorControl") as IFilterControl;
			}
			if(ret != null)
				ret.SetViewMode(mode);
			else
				ret = new FilterControl();
			return ret;
		}
	}
	public class FormatRuleMenuItems {
		class RuleInfo {
			public string AppearanceName { get; set; }
			public bool ApplyToRow { get; set; }
			public List<object> EditValues = new List<object>();
		}
		const string ellipsis = "...";
		FormatRuleColumnInfo column = null;
		DXMenuItemCollection collection = null;
		public FormatRuleMenuItems(FormatRuleColumnInfo column, DXMenuItemCollection collection) {
			this.column = column;
			this.collection = collection;
		}
		public Form OwnerForm {
			get {
				Control ctrl = OwnerControl as Control;
				if(ctrl == null) return null;
				return ctrl.FindForm();
			}
		}
		public ISupportLookAndFeel OwnerControl {
			get {
				if(column == null) return null;
				return column.OwnerControl;
			}
		}
		bool IsDisplayEditor {
			get {
				if(column == null) return false;
				return (column.RepositoryItem is RepositoryItemImageComboBox ||
					column.RepositoryItem is RepositoryItemLookUpEditBase ||
					column.RepositoryItem is RepositoryItemRadioGroup ||
					column.RepositoryItem is RepositoryItemCheckEdit ||
					column.RepositoryItem is RepositoryItemCheckedComboBoxEdit);
			}
		}
		bool IsServerMode { 
			get { 
				if(column == null) return false;
				return column.IsServerMode;
			} 
		}
		public int Count {
			get {
				if(collection == null) return -1;
				return collection.Count;
			}
		}
		bool AllowAverage { get { return SummaryItemTypeHelper.CanApplySummary(SummaryItemType.Average, column.ColumnType) && !IsDisplayEditor; } }
		bool IsDateTime { get { return SummaryItemTypeHelper.IsDateTime(column.ColumnType); } }
		bool IsNumeric { get { return SummaryItemTypeHelper.IsNumericalType(column.ColumnType); } }
		public void UpdateItems() {
			collection.Clear();
			CreateHighlightCellRules();
			if(!IsServerMode) {
				CreateTopBottomRules();
				CreateUniqueDuplicateRules();
			}
			if(AllowAverage) {
				CreateDataBars();
				CreateColorScales();
				CreateIconSets();
			}
			CreateClearItems();
			CreateManageRules();
		}
		DXMenuItem CreateFormatRuleEditMenuItem(StringId stringId, EventHandler eventHandler, string imageName) {
			Image image = string.IsNullOrEmpty(imageName) ? null : GetMenuItemImage(imageName);
			DXMenuItem item = new DXMenuItem(Localizer.Active.GetLocalizedString(stringId), eventHandler, image);
			item.Caption += ellipsis;
			item.Tag = stringId;
			return item;
		}
		protected virtual void CreateUniqueDuplicateRules() {
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemUniqueDuplicateRules, "UniqueDuplicate_16x16.png");
			item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemUnique, UniqueDuplicateRulesItemClick, "Unique_16x16.png"));
			item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemDuplicate, UniqueDuplicateRulesItemClick, "Duplicate_16x16.png"));
		}
		bool IsUniqueItem(object sender) {
			return StringId.FormatRuleMenuItemUnique.Equals(((DXMenuItem)sender).Tag);
		}
		void UniqueDuplicateRulesItemClick(object sender, EventArgs e) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(
				IsUniqueItem(sender) ? StringId.FormatRuleUniqueText : StringId.FormatRuleDuplicateText));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			FormatConditionRuleUniqueDuplicate rule = new FormatConditionRuleUniqueDuplicate();
			rule.FormatType = IsUniqueItem(sender) ? FormatConditionUniqueDuplicateType.Unique : FormatConditionUniqueDuplicateType.Duplicate;
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		protected virtual void UpdateFormatConditionRules(FormatConditionRuleAppearanceBase rule, bool applyToRow) {
		}
		RuleInfo ShowFormatRuleEditForm(object sender, string caption) {
			RuleInfo ret = new RuleInfo() { AppearanceName = string.Empty, ApplyToRow = false };
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return ret;
			using(FormatRuleEditFormBase form = new FormatRuleEditFormBase()) {
				form.Tag = column.MenuManager;
				if(column.OwnerControl != null)
					form.LookAndFeel.Assign(column.OwnerControl.LookAndFeel);
				form.Init(item.Caption.Replace(ellipsis, string.Empty), string.Format("{0}:", caption));
				InitRuleFormProperties(form);
				InitEditors(sender, form);
				form.UpdateSize();
				Control control = OwnerControl as Control;
				if(form.ShowDialog(control) == DialogResult.OK) {
					ret.AppearanceName = form.AppearanceName;
					ret.ApplyToRow = form.ApplyToRow;
					form.GetEditValues(ret.EditValues);
				}
			}
			return ret;
		}
		protected virtual void InitRuleFormProperties(FormatRuleEditFormBase form) {
			form.FormBorderEffect = FormBorderEffect.Shadow;
		}
		bool IsColumnExists { get { return !string.IsNullOrEmpty(column.Name); } }
		protected virtual void CreateManageRules() {
			DXMenuItem manageColumnItem = CreateDXMenuItem(StringId.FormatRuleMenuItemManageRules, ManageColumnRulesItemClick);
			string imageName = "ManageRules_16x16.png";
			if(!string.IsNullOrEmpty(imageName))
				manageColumnItem.Image = GetMenuItemImage(imageName);
			manageColumnItem.CloseMenuOnClick = true;
			collection.Add(manageColumnItem);
		}		
		protected virtual void CreateClearItems() {
			if(!IsRuleExists) return;
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemClearRules, "ClearRules_16x16.png");
			item.BeginGroup = true;
			if(IsColumnExists && IsRuleColumnExists) {
				DXMenuItem clearColumnItem = CreateDXMenuItem(StringId.FormatRuleMenuItemClearColumnRules, ClearColumnRulesItemClick);
				clearColumnItem.CloseMenuOnClick = false;
				item.Items.Add(clearColumnItem);
			}
			item.Items.Add(CreateDXMenuItem(StringId.FormatRuleMenuItemClearAllRules, ClearAllRulesItemClick));
		}
		DXMenuItem CreateDXMenuItem(StringId stringId, EventHandler click) {
			DXMenuItem item = new DXMenuItem(Localizer.Active.GetLocalizedString(stringId), click);
			item.Tag = stringId;
			return item;
		}
		protected virtual void ManageColumnRulesItemClick(object sender, EventArgs e) { }
		protected virtual void ClearColumnRulesItemClick(object sender, EventArgs e) { }
		protected virtual void ClearAllRulesItemClick(object sender, EventArgs e) { }
		protected virtual void CreateIconSets() {
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemIconSets, "IconSet_16x16.png");
			item.BeforePopup += SubMenuIconSets_BeforePopup;
		}
		protected virtual void SubMenuIconSets_BeforePopup(object sender, EventArgs e) {
			DXSubMenuItem item = sender as DXSubMenuItem;
			if(item.Items.Count > 0) return;
			string categoryName = string.Empty;
			foreach(FormatConditionIconSet iconSetSource in FormatPredefinedIconSets.Default) {
				var iconSet = iconSetSource.Clone();
				if(OwnerControl != null)
					iconSet.LookAndFeel = OwnerControl.LookAndFeel;
				else iconSet.LookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default;
				if(iconSetSource.CategoryName != categoryName) {
					categoryName = iconSetSource.CategoryName;
					DXMenuHeaderItem header = new DXMenuHeaderItem() { Caption = categoryName, MultiColumn = DefaultBoolean.True };
					item.Items.Add(header);
					header.OptionsMultiColumn.ColumnCount = 3;
					header.OptionsMultiColumn.ImageHorizontalAlignment = Utils.Drawing.ItemHorizontalAlignment.Left;
					header.OptionsMultiColumn.UseMaxItemWidth = DefaultBoolean.True;
				}
				Image iconSetMenuImage = GetIconsImage(iconSet);
				DXMenuItem iconSetMenuItem = new DXMenuItem(iconSetSource.Name, IconSetItemClick, iconSetMenuImage);
				iconSetMenuItem.Image = iconSetMenuImage;
				iconSetMenuItem.Tag = iconSetSource;
				iconSetMenuItem.CloseMenuOnClick = false;
				iconSetMenuItem.SuperTip = new SuperToolTip();
				iconSetMenuItem.SuperTip.AllowHtmlText = DefaultBoolean.True;
				iconSetMenuItem.SuperTip.Items.Add(string.Format("<b>{0}", iconSetSource.Title));
				string superTipDescription = Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemIconSetDescription);
				if(superTipDescription.IndexOf(":", superTipDescription.TrimEnd().Length - 1) > -1)
					iconSetMenuItem.SuperTip.Items.Add(string.Format("{0}\r\n{1}", superTipDescription, iconSetSource.RangeDescription));
				else iconSetMenuItem.SuperTip.Items.Add(superTipDescription);
				item.Items.Add(iconSetMenuItem);
				iconSet.LookAndFeel = null;
			}
		}
		System.Drawing.Image GetIconsImage(FormatConditionIconSet iconSet) {
			if(iconSet == null || !iconSet.HasIcons) return null;
			var icons = iconSet.SortIcons();
			Size iconSize = icons[0].GetIcon().Size;
			Bitmap iconsImage = new System.Drawing.Bitmap(iconSize.Width * icons.Count, iconSize.Height);
			using(Graphics g = Graphics.FromImage(iconsImage)) {
				for(int i = 0; i < icons.Count; i++)
					g.DrawImage(icons[i].GetIcon(), i * iconSize.Width, 0);
			}
			return iconsImage;
		}
		protected virtual void IconSetItemClick(object sender, EventArgs e) {
		}
		protected virtual void CreateColorScales() {
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemColorScales, "ColorScale_16x16.png");
			item.BeforePopup += SubMenuColorScales_BeforePopup;
		}
		protected virtual void SubMenuColorScales_BeforePopup(object sender, EventArgs e) {
			DXSubMenuItem item = sender as DXSubMenuItem;
			if(item.Items.Count > 0) return;
			DXMenuHeaderItem header = new DXMenuHeaderItem() { Caption = Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemColorScales), MultiColumn = DefaultBoolean.True };
			header.OptionsMultiColumn.LargeImages = DefaultBoolean.True;
			header.OptionsMultiColumn.ImageHorizontalAlignment = Utils.Drawing.ItemHorizontalAlignment.Left;
			header.OptionsMultiColumn.UseMaxItemWidth = DefaultBoolean.True;
			item.Items.Add(header);
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleGreenYellowRed, "Green, Yellow, Red", "GreenYellowRedColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScalePurpleWhiteAzure, "Purple, White, Azure", "PurpleWhiteAzureColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleYellowOrangeCoral, "Yellow, Orange, Coral", "YellowOrangeCoralColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleGreenWhiteRed, "Green, White, Red", "GreenWhiteRedColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleEmeraldAzureBlue, "Emerald, Azure, Blue", "EmeraldAzureBlueColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleBlueWhiteRed, "Blue, White, Red", "BlueWhiteRedColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleWhiteRed, "White, Red", "WhiteRedColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleWhiteGreen, "White, Green", "WhiteGreenColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleWhiteAzure, "White, Azure", "WhiteAzureColorScale_32x32.png"));
			item.Items.Add(CreateColorScaleItem(StringId.ColorScaleYellowGreen, "Yellow, Green", "YellowGreenColorScale_32x32.png"));
		}
		DXMenuItem CreateColorScaleItem(StringId id, string key, string imageName) {
			return CreatePredefinedItem(id, key, imageName, ColorScaleItemClick, Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemColorScaleDescription));
		}
		protected virtual void ColorScaleItemClick(object sender, EventArgs e) {
		}
		protected virtual void CreateDataBars() {
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemDataBars, "DataBar_16x16.png");
			item.BeforePopup += SubMenuDataBarss_BeforePopup;
			item.BeginGroup = true;
		}
		protected virtual void SubMenuDataBarss_BeforePopup(object sender, EventArgs e) {
			DXSubMenuItem item = sender as DXSubMenuItem;
			if(item.Items.Count > 0) return;
			DXMenuHeaderItem header1 = new DXMenuHeaderItem() { Caption = Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemGradientFill), MultiColumn = DefaultBoolean.True };
			header1.OptionsMultiColumn.ImageHorizontalAlignment = Utils.Drawing.ItemHorizontalAlignment.Left;
			header1.OptionsMultiColumn.UseMaxItemWidth = DefaultBoolean.True;
			item.Items.Add(header1);
			header1.OptionsMultiColumn.LargeImages = DefaultBoolean.True;
			item.Items.Add(CreateDataBarItem(StringId.DataBarBlueGradient, "Blue Gradient", "GradientBlueDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarLightBlueGradient, "Light Blue Gradient", "GradientLightBlueDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarGreenGradient, "Green Gradient", "GradientGreenDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarYellowGradient, "Yellow Gradient", "GradientYellowDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarOrangeGradient, "Orange Gradient", "GradientOrangeDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarMintGradient, "Mint Gradient", "GradientMintDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarVioletGradient, "Violet Gradient", "GradientVioletDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarRaspberryGradient, "Raspberry Gradient", "GradientRaspberryDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarCoralGradient, "Coral Gradient", "GradientCoralDataBar_32x32.png"));
			DXMenuHeaderItem header2 = new DXMenuHeaderItem() { Caption = Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemSolidFill), MultiColumn = DefaultBoolean.True };
			header2.OptionsMultiColumn.LargeImages = DefaultBoolean.True;
			header2.OptionsMultiColumn.ImageHorizontalAlignment = Utils.Drawing.ItemHorizontalAlignment.Left;
			header2.OptionsMultiColumn.UseMaxItemWidth = DefaultBoolean.True;
			item.Items.Add(header2);
			item.Items.Add(CreateDataBarItem(StringId.DataBarBlue, "Blue", "SolidBlueDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarLightBlue, "Light Blue", "SolidLightBlueDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarGreen, "Green", "SolidGreenDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarYellow, "Yellow", "SolidYellowDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarOrange, "Orange", "SolidOrangeDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarMint, "Mint", "SolidMintDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarViolet, "Violet", "SolidVioletDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarRaspberry, "Raspberry", "SolidRaspberryDataBar_32x32.png"));
			item.Items.Add(CreateDataBarItem(StringId.DataBarCoral, "Coral", "SolidCoralDataBar_32x32.png"));
		}
		DXMenuItem CreateDataBarItem(StringId id, string key, string imageName) {
			return CreatePredefinedItem(id, key, imageName, DataBarItemClick, Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemDataBarDescription));
		}
		DXMenuItem CreatePredefinedItem(StringId id, string key, string imageName, EventHandler eventHandler, string superTipCaption) {
			DXMenuItem item = new DXMenuItem(Localizer.Active.GetLocalizedString(id)) { LargeImage = GetMenuItemImage(imageName) };
			item.Tag = key;
			item.SuperTip = new SuperToolTip();
			item.SuperTip.AllowHtmlText = DefaultBoolean.True;
			item.SuperTip.Items.Add(string.Format("<b>{0}</b>", item.Caption));
			item.SuperTip.Items.Add(superTipCaption);
			item.Click += eventHandler;
			item.CloseMenuOnClick = false;
			return item;
		}
		protected virtual void DataBarItemClick(object sender, EventArgs e) {
		}
		protected virtual void CreateHighlightCellRules() {
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemHighlightCellRules, "HighlightCellsRules_16x16.png");
			if(!IsDisplayEditor) {
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemGreaterThan, GreaterRuleItemClick, "GreaterThan_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemLessThan, LessRuleItemClick, "LessThan_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemBetween, BetweenRuleItemClick, "Between_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemEqualTo, EqualRuleItemClick, "EqualTo_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemTextThatContains, TextThatContainsRuleItemClick, "TextThatContains_16x16.png"));
			}
			if(IsDateTime) 
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemDateOccurring, DateOccurringRuleItemClick, "DateOccurring_16x16.png"));
			item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemCustomCondition, CustomConditionRuleItemClick, "CustomCondition_16x16.png"));
		}
		void EqualRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleValue(sender, StringId.FormatRuleEqualToText, FormatCondition.Equal);
		}
		void GreaterRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleValue(sender, StringId.FormatRuleGreaterThanText, FormatCondition.Greater);
		}
		void LessRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleValue(sender, StringId.FormatRuleLessThanText, FormatCondition.Less);
		}
		void BetweenRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleValue(sender, StringId.FormatRuleBetweenText, FormatCondition.Between);
		}
		void FormatConditionRuleValue(object sender, StringId text, FormatCondition condition) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(text));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			for(int i = 0; i < rInfo.EditValues.Count; i++)
				if(string.IsNullOrEmpty(string.Format("{0}", rInfo.EditValues[i]))) return;
			FormatConditionRuleValue rule = new FormatConditionRuleValue();
			rule.Condition = condition;
			rule.Value1 = GetValue(rInfo.EditValues[0], column.ColumnType);
			if(rInfo.EditValues.Count > 1)
				rule.Value2 = GetValue(rInfo.EditValues[1], column.ColumnType);
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		void TextThatContainsRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleExpression(sender, StringId.FormatRuleTextThatContainsText, "Contains([{0}], '{1}')");
		}
		void CustomConditionRuleItemClick(object sender, EventArgs e) {
			FormatConditionRuleExpression(sender, StringId.FormatRuleCustomConditionText, "{1}");
		}
		void FormatConditionRuleExpression(object sender, StringId text, string expression) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(text));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			if(string.IsNullOrEmpty(string.Format("{0}", rInfo.EditValues[0]))) return;
			FormatConditionRuleExpression rule = new FormatConditionRuleExpression();
			rule.Expression = string.Format(expression, column.Name, rInfo.EditValues[0]);
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		void DateOccurringRuleItemClick(object sender, EventArgs e) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(StringId.FormatRuleDateOccurring));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			if(string.IsNullOrEmpty(string.Format("{0}", rInfo.EditValues[0]))) return;
			FormatConditionRuleDateOccuring rule = new FormatConditionRuleDateOccuring();
			rule.DateType = (FilterDateType)rInfo.EditValues[0];
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		static object GetValue(object value, Type type) {
			try {
				value = Convert.ChangeType(value, type);
			}
			catch {
			}
			return value;
		}
		protected virtual void CreateTopBottomRules() {
			if(IsDisplayEditor) return;
			DXSubMenuItem item = CreateSubMenuItem(StringId.FormatRuleMenuItemTopBottomRules, "TopBottomRules_16x16.png");
			item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemTop10Items, TopRulesItemClick, "Top10Items_16x16.png"));
			if(AllowAverage)
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemTop10Percent, TopRulesItemClick, "Top10Percent_16x16.png"));
			item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemBottom10Items, BottomRulesItemClick, "Bottom10Items_16x16.png"));
			if(AllowAverage) {
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemBottom10Percent, BottomRulesItemClick, "Bottom10Percent_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemAboveAverage, AverageRulesItemClick, "AboveAverage_16x16.png"));
				item.Items.Add(CreateFormatRuleEditMenuItem(StringId.FormatRuleMenuItemBelowAverage, AverageRulesItemClick, "BelowAverage_16x16.png"));
			}
		}
		bool IsTopPercentItem(object sender) {
			return StringId.FormatRuleMenuItemTop10Percent.Equals(((DXMenuItem)sender).Tag);
		}
		bool IsBottomPercentItem(object sender) {
			return StringId.FormatRuleMenuItemBottom10Percent.Equals(((DXMenuItem)sender).Tag);
		}
		void TopRulesItemClick(object sender, EventArgs e) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(StringId.FormatRuleTopText));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			FormatConditionRuleTopBottom rule = new FormatConditionRuleTopBottom();
			rule.RankType = IsTopPercentItem(sender) ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			rule.TopBottom = FormatConditionTopBottomType.Top;
			rule.Rank = Convert.ToDecimal(rInfo.EditValues[0]);
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		void BottomRulesItemClick(object sender, EventArgs e) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(StringId.FormatRuleBottomText));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			FormatConditionRuleTopBottom rule = new FormatConditionRuleTopBottom();
			rule.TopBottom = FormatConditionTopBottomType.Bottom;
			rule.RankType = IsBottomPercentItem(sender) ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			rule.Rank = Convert.ToDecimal(rInfo.EditValues[0]);
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		bool IsAboveItem(object sender) {
			return StringId.FormatRuleMenuItemAboveAverage.Equals(((DXMenuItem)sender).Tag);
		}
		void AverageRulesItemClick(object sender, EventArgs e) {
			RuleInfo rInfo = ShowFormatRuleEditForm(sender, Localizer.Active.GetLocalizedString(
				IsAboveItem(sender) ? StringId.FormatRuleAboveAverageText : StringId.FormatRuleBelowAverageText));
			if(string.IsNullOrEmpty(rInfo.AppearanceName)) return;
			FormatConditionRuleAboveBelowAverage rule = new FormatConditionRuleAboveBelowAverage();
			rule.AverageType = IsAboveItem(sender) ? FormatConditionAboveBelowType.Above : FormatConditionAboveBelowType.Below;
			rule.PredefinedName = rInfo.AppearanceName;
			UpdateFormatConditionRules(rule, rInfo.ApplyToRow);
		}
		Image GetMenuItemImage(string imageName) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraEditors.FormatRule.MenuImages.{0}", imageName), typeof(FormatRuleMenuItems).Assembly);
		}
		DXSubMenuItem CreateSubMenuItem(StringId stringId, string imageName) {
			DXSubMenuItem item = new DXSubMenuItem(Localizer.Active.GetLocalizedString(stringId));
			if(!string.IsNullOrEmpty(imageName))
				item.Image = GetMenuItemImage(imageName);
			item.Tag = stringId;
			collection.Add(item);
			return item;
		}
		protected virtual bool IsRuleColumnExists { get { return false; } }
		protected virtual bool IsRuleExists { get { return false; } }
		protected virtual void InitEditors(object sender, FormatRuleEditFormBase form) {
			DXMenuItem item = sender as DXMenuItem;
			if(StringId.FormatRuleMenuItemTop10Items.Equals(item.Tag)
				|| StringId.FormatRuleMenuItemBottom10Items.Equals(item.Tag)) InitTopBottomItemEditors(form);
			if(IsTopPercentItem(sender) || IsBottomPercentItem(sender)) InitTopBottomPercentItemEditors(form);
			if(StringId.FormatRuleMenuItemEqualTo.Equals(item.Tag) || StringId.FormatRuleMenuItemGreaterThan.Equals(item.Tag)
				|| StringId.FormatRuleMenuItemLessThan.Equals(item.Tag)) InitRepositoryItemEditors(form);
			if(StringId.FormatRuleMenuItemBetween.Equals(item.Tag)) InitBetweenItemEditors(form);
			if(StringId.FormatRuleMenuItemTextThatContains.Equals(item.Tag)) InitTextThatContainsItemEditors(form);
			if(StringId.FormatRuleMenuItemCustomCondition.Equals(item.Tag)) {
				if(UseEditFormForCustomCondition)
					InitCustomConditionItemEditors(form);
				else 
					InitCustomConditionItemFilterBuilder(form);
			}
			if(StringId.FormatRuleMenuItemDateOccurring.Equals(item.Tag)) InitDateOccurringItemEditors(form);
			SetMenuManager(form.Controls);
		}
		protected virtual bool UseEditFormForCustomCondition { get { return false; } }
		void SetMenuManager(Control.ControlCollection controlCollection) {
			foreach(Control c in controlCollection) {
				BaseEdit edit = c as BaseEdit;
				if(edit != null) edit.MenuManager = column.MenuManager;
				SetMenuManager(c.Controls);
			}
		}
		void InitTopBottomItemEditors(FormatRuleEditFormBase form) {
			SpinEdit edit = new SpinEdit();
			edit.Properties.IsFloatValue = false;
			edit.Properties.MinValue = 0;
			edit.Properties.MaxValue = 1000000;
			edit.EditValue = 10;
			form.AddEditors(new Control[] { form.CreateSeparator(), edit });
		}
		void InitTopBottomPercentItemEditors(FormatRuleEditFormBase form) {
			SpinEdit edit = new SpinEdit();
			edit.Properties.IsFloatValue = false;
			edit.Properties.MinValue = 0;
			edit.Properties.MaxValue = 100;
			edit.EditValue = 10;
			LabelControl lb = new LabelControl();
			lb.Text = "%";
			form.AddEditors(new Control[] { form.CreateSeparator(), lb, form.CreateSeparator(), edit });
		}
		BaseEdit GetRepositoryEditor(int width) {
			BaseEdit edit = column.RepositoryItem.CreateEditor();
			edit.Properties.Assign(column.RepositoryItem);
			FilterControl.InitRepositoryPropertiesForFilterEdit(edit.Properties);
			SetNullValuePrompt(edit, StringId.FilterEmptyEnter);
			if(width > 0) edit.Width = width;
			return edit;
		}
		void InitRepositoryItemEditors(FormatRuleEditFormBase form) {
			if(column == null) return;
			form.AddEditors(new Control[] { form.CreateSeparator(), GetRepositoryEditor(form.GetEditorWidth()) });
		}
		void InitBetweenItemEditors(FormatRuleEditFormBase form) {
			if(column == null) return;
			LabelControl lb = new LabelControl();
			lb.Text = Localizer.Active.GetLocalizedString(StringId.FilterGroupAnd);
			form.AddEditors(new Control[] { form.CreateSeparator(), GetRepositoryEditor(0), form.CreateSeparator(), lb, form.CreateSeparator(), GetRepositoryEditor(0) });
		}
		void InitTextThatContainsItemEditors(FormatRuleEditFormBase form) {
			TextEdit edit = new TextEdit();
			SetNullValuePrompt(edit, StringId.FilterEmptyEnter);
			edit.Width = form.GetEditorWidth();
			form.AddEditors(new Control[] { form.CreateSeparator(), edit });
		}
		void InitCustomConditionItemEditors(FormatRuleEditFormBase form) {
			ButtonEdit edit = new ButtonEdit();
			edit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			edit.ButtonClick += ExpressionBuilderClick;
			edit.CustomDisplayText += ExpressionBuilderCustomDisplayText;
			SetNullValuePrompt(edit, StringId.FormatRuleExpressionEmptyEnter);
			edit.Width = form.GetEditorWidth();
			form.AddEditors(new Control[] { form.CreateSeparator(), edit });
		}
		void InitCustomConditionItemFilterBuilder(FormatRuleEditFormBase form) {
			IFilterControl fc = FormatRuleMenuHelper.CreateFilterControl();
			FilterColumnCollection fcCollection = GetFilterColumns();
			form.AddFilterControl(fc, new Size(450, 270));
			fc.SetFilterColumnsCollection(fcCollection, column.MenuManager);
			fc.SetDefaultColumn(GetDefaultFilterColumn(fcCollection));
			fc.ShowOperandTypeIcon = true;
			fc.UseMenuForOperandsAndOperators = true;
			fc.FilterCriteria = null;
		}
		void InitDateOccurringItemEditors(FormatRuleEditFormBase form) {
			CheckedComboBoxEdit edit = new CheckedComboBoxEdit();
			edit.Tag = FilterDateType.User;
			edit.Properties.SetFlags(typeof(FilterDateType));
			edit.Properties.Items.Clear();
			foreach(FilterDateType key in FormatRuleDateOccurringHelper.FormatRuleDateOccurringValues.Keys)
				edit.Properties.Items.Add(key, FormatRuleDateOccurringHelper.FormatRuleDateOccurringValues[key]);
			edit.Properties.CustomDisplayText += (s, e) => {
				if(FilterDateType.None.Equals(e.Value)) e.DisplayText = Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionNone);
				else { 
					string ret = string.Empty;
					foreach(FilterDateType key in FormatRuleDateOccurringHelper.FormatRuleDateOccurringValues.Keys)
						if(((FilterDateType)e.Value & key) != 0) {
							ret += string.Format("{0}, ", FormatRuleDateOccurringHelper.FormatRuleDateOccurringValues[key]);
						}
					if(ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
					e.DisplayText = ret;
				}
			};
			edit.Properties.DropDownRows = 14;
			edit.Width = form.GetEditorWidth();
			form.AddEditors(new Control[] { form.CreateSeparator(), edit });
		}
		protected virtual FilterColumn GetDefaultFilterColumn(FilterColumnCollection fcCollection) { return null; }
		protected virtual FilterColumnCollection GetFilterColumns() { return null; }
		protected virtual void SetNullValuePrompt(BaseEdit edit, StringId text) {
			TextEdit textEdit = edit as TextEdit;
			if(textEdit == null) return;
			textEdit.Properties.NullValuePromptShowForEmptyValue = true;
			textEdit.Properties.ShowNullValuePromptWhenFocused = true;
			textEdit.Properties.NullValuePrompt = Localizer.Active.GetLocalizedString(text);
		}
		protected virtual void ExpressionBuilderClick(object sender, EventArgs e) { }
		protected virtual void ExpressionBuilderCustomDisplayText(object sender, CustomDisplayTextEventArgs e) { }
	}
	internal class FormatRuleDateOccurringHelper {
		static Dictionary<FilterDateType, string> formatRuleDateOccurringValues = null;
		public static Dictionary<FilterDateType, string> FormatRuleDateOccurringValues {
			get {
				if(formatRuleDateOccurringValues == null) {
					formatRuleDateOccurringValues = new Dictionary<FilterDateType, string>();
					formatRuleDateOccurringValues.Add(FilterDateType.BeyondThisYear, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear));
					formatRuleDateOccurringValues.Add(FilterDateType.LaterThisYear, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear));
					formatRuleDateOccurringValues.Add(FilterDateType.LaterThisMonth, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth));
					formatRuleDateOccurringValues.Add(FilterDateType.LaterThisWeek, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek));
					formatRuleDateOccurringValues.Add(FilterDateType.NextWeek, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek));
					formatRuleDateOccurringValues.Add(FilterDateType.Tomorrow, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow));
					formatRuleDateOccurringValues.Add(FilterDateType.Today, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalToday));
					formatRuleDateOccurringValues.Add(FilterDateType.Yesterday, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalYesterday));
					formatRuleDateOccurringValues.Add(FilterDateType.EarlierThisWeek, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek));
					formatRuleDateOccurringValues.Add(FilterDateType.LastWeek, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek));
					formatRuleDateOccurringValues.Add(FilterDateType.EarlierThisMonth, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth));
					formatRuleDateOccurringValues.Add(FilterDateType.EarlierThisYear, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear));
					formatRuleDateOccurringValues.Add(FilterDateType.PriorThisYear, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear));
				}
				return formatRuleDateOccurringValues;
			}
		}
	}
}
