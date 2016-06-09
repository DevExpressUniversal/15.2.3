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

using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class IconSetControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionIconSet iconSetCore;
		FormatRule formatRuleCore;
		List<Image> iconSetImage;  
		List<LayoutControlGroup> group;
		bool init;
		public IconSetControl() {
			InitializeComponent();
			InitLocalizationText();
			CreateRule();
			CreateExternalGroup();
			SetIconsCollection();
			ResizeControls();
		}
		void ResizeControls() {
			lcIconSet.OptionsView.UseParentAutoScaleFactor = true;
			SizeF scale = ((DevExpress.XtraLayout.ILayoutControl)lcIconSet).AutoScaleFactor;
			ResizeDPI(scale);
			ResizeTouchMode(scale);
		}
		void ResizeDPI(SizeF scale) {
			Size defaultSize = new Size((int)(lciHighValue.Size.Width * scale.Width), (int)(lciHighValue.Size.Height * scale.Height));
			lciHighValue.MaxSize = lciHighValue.MinSize = lciHighSign.MaxSize = lciHighSign.MinSize = defaultSize;
			lciMiddleOneValue.MaxSize = lciMiddleOneValue.MinSize = lciMiddleOneSign.MaxSize = lciMiddleOneSign.MinSize = defaultSize;
			lciMiddleTwoValue.MaxSize = lciMiddleTwoValue.MinSize = lciMiddleTwoSign.MaxSize = lciMiddleTwoSign.MinSize = defaultSize;
			lciMiddleThreeValue.MaxSize = lciMiddleThreeValue.MinSize = lciMiddleThreeSign.MaxSize = lciMiddleThreeSign.MinSize = defaultSize;
			lciLowValue.MaxSize = lciLowValue.MinSize = lciLowSign.MaxSize = lciLowSign.MinSize = defaultSize;
		}
		void ResizeTouchMode(SizeF scale) {
			if(WindowsFormsSettings.TouchUIMode != DevExpress.LookAndFeel.TouchUIMode.True) return;
			Size defaultSize = new Size((int)(100 * scale.Width), (int)(40 * scale.Height));
			lciHighValue.MaxSize = lciHighValue.MinSize = lciHighSign.MaxSize = lciHighSign.MinSize = defaultSize;
			lciMiddleOneValue.MaxSize = lciMiddleOneValue.MinSize = lciMiddleOneSign.MaxSize = lciMiddleOneSign.MinSize = defaultSize;
			lciMiddleTwoValue.MaxSize = lciMiddleTwoValue.MinSize = lciMiddleTwoSign.MaxSize = lciMiddleTwoSign.MinSize = defaultSize;
			lciMiddleThreeValue.MaxSize = lciMiddleThreeValue.MinSize = lciMiddleThreeSign.MaxSize = lciMiddleThreeSign.MinSize = defaultSize;
			lciLowValue.MaxSize = lciLowValue.MinSize = lciLowSign.MaxSize = lciLowSign.MinSize = defaultSize;
		}
		void InitLocalizationText() {
			sliInfo.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsDisplayEachIconAccordingToTheseRules);
			btnReverse.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsReverseIconOrder);
			cmbValueType.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
		}
		void CreateRule() {
			iconSetCore = new FormatConditionIconSet();
		}
		void CreateExternalGroup() {
			group = new List<LayoutControlGroup>();
			group.AddRange(new List<LayoutControlGroup> {
				lcgHigh, lcgMiddleOne, lcgMiddleTwo, lcgMiddleThree, lcgLow
			});
		}
		void SetIconsCollection() {
			iconSetImage = new List<Image>();
			int i = 0;
			foreach(FormatConditionIconSet iconSetSource in FormatPredefinedIconSets.Default.OrderBy(x => x.Icons.Count)) {
				FormatConditionIconSet iconSet = Assign(iconSetSource);
				iconSet.LookAndFeel = LookAndFeel;
				Image img = GetIconsImage(iconSet);  
				iconSetImage.Add(img);
				imgcmbPreviewIconSet.Properties.Items.Add(new IconSetItem() {
					Image = img,
					ImageIndex = i++,
					Value = iconSet
				});
			}
			imgcmbPreviewIconSet.Properties.SmallImages = iconSetImage;
			imgcmbPreviewIconSet.SelectedIndex = 0;
			imgcmbPreviewIconSet.RaiseIXtraResizableControlSizeChanged();
			Update();
		}
		new void Update() {
			List<IconSetInfoUI> values = new List<IconSetInfoUI>();
			for(int i = 0; i < iconSetCore.Icons.Count; i++) {
				var val = new IconSetInfoUI();
				foreach(LayoutControlItem titem in group[i].Items) {
					if(titem.Control is ComboBoxEdit) { val.Sign = GetSignStringUpdate(((ComboBoxEdit)titem.Control).SelectedIndex); continue; }
					if(titem.Control is TextEdit) { val.Value = ((TextEdit)titem.Control).Text; continue; }
				}
				values.Add(val);
			}
			for(int i = 1; i < iconSetCore.Icons.Count; i++) {
				foreach(LayoutControlItem titem in group[i].Items) {
					if(titem.Control is PictureEdit) titem.Text = GetIconSetText(i, values);
				}
			}
		}
		string GetSignStringUpdate(int i) {
			return i == 0 ? "<" : "<=";
		}
		string GetIconSetText(int i, List<IconSetInfoUI> values, int textLength = 0) {
			string itemText = string.Empty;
			var icons = values[i - 1];
			if(i == 0) itemText = string.Format("{0} {1}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen),
														   Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsValueIs));
			else if(i == values.Count - 1) itemText = string.Format("{0} {1} {2}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen), icons.Sign, icons.Value);
			else itemText = string.Format("{0} {1} {2} {3}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen), icons.Sign, icons.Value,
															 Localizer.Active.GetLocalizedString(StringId.FilterClauseBetweenAnd));
			return itemText;
		}
		#region overrite events
		void imgcmbPreviewIconSet_SelectedIndexChanged(object sender, EventArgs e) {
			IconSetItem selected = (IconSetItem)imgcmbPreviewIconSet.SelectedItem;
			if(!init) iconSetCore = (FormatConditionIconSet)selected.Value;
			imgcmbPreviewIconSet.Properties.ContextImage = selected.Image;
			SetFormFormat();
			if(ParentForm != null) ((NewRuleForm)ParentForm).CheckFormSize();
		}
		void cmbHighSign_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbMiddleOneSign_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbMiddleTwoSign_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbMiddleThreeSign_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbLowSign_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void tedHighValue_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void tedMiddleOneValue_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void tedMiddleTwoValue_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void tedMiddleThreeValue_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void tedLowValue_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		#endregion
		#region Helpers method
		System.Drawing.Image GetIconsImage(FormatConditionIconSet iconSet) {
			if(iconSet == null || !iconSet.HasIcons) return null;
			List<FormatConditionIconSetIcon> icons = SortIcons(iconSet);
			Size iconSize = icons[0].GetIcon().Size;
			Bitmap iconsImage = new System.Drawing.Bitmap((iconSize.Width + 4) * icons.Count, iconSize.Height + 4);
				using(Graphics g = Graphics.FromImage(iconsImage)) {
				for(int i = 0; i < icons.Count; i++)
					g.DrawImage(icons[i].GetIcon(), i * (iconSize.Width + 2) + 4, 2);
			}
			return iconsImage;
		}
		FormatConditionIconSet Assign(FormatConditionIconSet iconSetSource) {
			FormatConditionIconSet iconSet = new FormatConditionIconSet() {
				ValueType = iconSetSource.ValueType,
				Title = iconSetSource.Title,
				Name = iconSetSource.Name,
				CategoryName = iconSetSource.CategoryName
			};
			iconSet.Icons.Clear();
			foreach(var icon in iconSetSource.Icons)
				iconSet.Icons.Add(Assign(icon));
			return iconSet;
		}
		FormatConditionIconSetIcon Assign(FormatConditionIconSetIcon source) {
			FormatConditionIconSetIcon icon = new FormatConditionIconSetIcon() {
				Icon = source.Icon,
				Value = source.Value,
				ValueComparison = source.ValueComparison,
				PredefinedName = source.PredefinedName
			};
			return icon;
		}
		List<FormatConditionIconSetIcon> SortIcons(FormatConditionIconSet iconSet) {
			List<FormatConditionIconSetIcon> icons = new List<FormatConditionIconSetIcon>(iconSet.Icons);
			icons.Sort((a, b) => {
				int c = decimal.Compare(a.Value, b.Value);
				if(c != 0) return c * -1;
				if(a.ValueComparison == b.ValueComparison) return 0;
				if(a.ValueComparison == FormatConditionComparisonType.Greater) return -1;
				return 1;
			});
			return icons;
		}
		FormatConditionRuleIconSet GetFormatConditionRule(FormatConditionIconSet iconSet) {
			FormatConditionRuleIconSet iconSetCondition = new FormatConditionRuleIconSet() {
				IconSet = iconSet
			};
			return iconSetCondition;
		}
		#endregion
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			GetFormFormat();
			FormatConditionIconSet format = new FormatConditionIconSet();
			format = Assign(iconSetCore);
			formatRuleCore.RuleBase = new FormatConditionRuleIconSet() { IconSet = format };
			formatRuleCore.RuleType = RuleType.IconSet;
			formatRuleCore.RuleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSet);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleIconSet;
			if(rule != null) iconSetCore = ((FormatConditionRuleIconSet)rule).IconSet;
			else iconSetCore = (FormatConditionIconSet)imgcmbPreviewIconSet.Properties.Items[0].Value;
			SetFormFormat();
		}
		#endregion
		#region GetForm
		public void GetFormFormat() {
			for(int i = 0; i < iconSetCore.Icons.Count; i++) {
				FormatConditionIconSetIcon icon = iconSetCore.Icons[i];
				foreach(LayoutControlItem titem in group[i].Items) {
					if(titem.Control is ComboBoxEdit) { icon.ValueComparison = GetSign(((ComboBoxEdit)titem.Control).SelectedIndex); continue; }
					if(titem.Control is TextEdit) { icon.Value = GetValue(((TextEdit)titem.Control).Text, icon.Value); continue; }
				}
			}
			iconSetCore.ValueType = GetValueType(cmbValueType.SelectedIndex);
		}
		decimal GetValue(string valueStr, decimal valueDefault) {
			decimal value;
			if(decimal.TryParse(valueStr, out value)) return value;
			else return valueDefault;
		}
		FormatConditionComparisonType GetSign(int selected) {
			return selected == 0 ? FormatConditionComparisonType.GreaterOrEqual : FormatConditionComparisonType.Greater;
		}
		FormatConditionValueType GetValueType(int selected) {
			return selected == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			init = true;
			for(int i = 0; i < imgcmbPreviewIconSet.Properties.Items.Count; i++) {
				IconSetItem icon = imgcmbPreviewIconSet.Properties.Items[i] as IconSetItem;
				string iconName = ((FormatConditionIconSet)icon.Value).Name;
				if(iconSetCore.Name == iconName) {
					imgcmbPreviewIconSet.SelectedIndex = icon.ImageIndex;
					break;
				}
			}
			for(int i = 0; i < iconSetCore.Icons.Count; i++) {
				FormatConditionIconSetIcon icon = iconSetCore.Icons[i];
				group[i].Visibility = LayoutVisibility.Always;
				foreach(LayoutControlItem titem in group[i].Items) {
					if(titem.Control is PictureEdit) { SetPictureItem(titem, icon, i); continue; }
					if(titem.Control is ComboBoxEdit) { SetComboBoxSelected((ComboBoxEdit)titem.Control, icon); continue; }
					if(titem.Control is TextEdit) { SetTextValue((TextEdit)titem.Control, icon); continue; }
				}
			}
			for(int i = iconSetCore.Icons.Count; i < group.Count; i++) group[i].Visibility = LayoutVisibility.Never;
			cmbValueType.SelectedIndex = GetValueType(iconSetCore.ValueType);
			init = false;
		}
		void SetPictureItem(LayoutControlItem titem, FormatConditionIconSetIcon icon, int i) {
			((PictureEdit)titem.Control).Image = icon.GetIcon();
			titem.Text = SetIconSetText(i);
		}
		string SetIconSetText(int i) {
			string itemText = string.Empty;
			var icons = i != 0 ? iconSetCore.Icons[i - 1] : null;
			if(i == 0)
				itemText = string.Format("{0} {1}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen), Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsValueIs));
			else if(i == iconSetCore.Icons.Count - 1)
				itemText = string.Format("{0} {1} {2}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen), GetSignString(icons), GetValue(icons));
			else
				itemText = string.Format("{0} {1} {2} {3}", Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSetsWhen), GetSignString(icons), GetValue(icons), Localizer.Active.GetLocalizedString(StringId.FilterClauseBetweenAnd));
			return itemText;
		}
		void SetComboBoxSelected(ComboBoxEdit comboBox, FormatConditionIconSetIcon icon) {
			comboBox.SelectedIndex = GetSign(icon);
		}
		int GetSign(FormatConditionIconSetIcon icon) {
			return icon.ValueComparison == FormatConditionComparisonType.GreaterOrEqual ? 0 : 1;
		}
		void SetTextValue(TextEdit tedit, FormatConditionIconSetIcon icon) {
			if(icon.Value == Decimal.MinValue) { tedit.Text = "-\x221E"; return; }
			tedit.Text = GetValue(icon);
		}
		string GetValue(FormatConditionIconSetIcon icon) {
			if(icon.Value == Decimal.MinValue) { return "-\x221E"; }
			return icon.Value.ToString(); ;
		}
		int GetValueType(FormatConditionValueType valueType) {
			return valueType == FormatConditionValueType.Number ? 1 : 0;
		}
		string GetSignString(FormatConditionIconSetIcon icon) {
			return icon.ValueComparison == FormatConditionComparisonType.GreaterOrEqual ? "<" : "<=";
		}
		#endregion
	}
	public class IconSetItem : ImageComboBoxItem {
		public Image Image { get; set; }
	}
	public class IconSetInfoUI {
		public string Sign { get; set; }
		public string Value { get; set; }
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class IconSetComboBoxEdit : ImageComboBoxEdit {
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupIconSetListBoxForm(this);
		}
		public void RaiseIXtraResizableControlSizeChanged() {
			base.RaiseSizeableChanged();
		}
	}
	public class PopupIconSetListBoxForm : PopupImageComboBoxEditListBoxForm {
		public PopupIconSetListBoxForm(ComboBoxEdit ownerEdit) : base(ownerEdit) { }
		protected override PopupListBox CreateListBox() {
			return new PopupIconSetListBox(this);
		}
	}
	public class PopupIconSetListBox : PopupImageComboBoxEditListBox {
		public PopupIconSetListBox(PopupListBoxForm ownerForm) : base(ownerForm) { }
		protected override Drawing.BaseControlPainter CreatePainter() {
			return new PopupIconSetListBoxPainter();
		}
	}
	public class PopupIconSetListBoxPainter : PopupImageListBoxPainter {
		protected override void DrawItemCore(Drawing.ControlGraphicsInfoArgs info, ViewInfo.BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			base.DrawItemCore(info, itemInfo, e);
			var img = ((DevExpress.XtraEditors.Frames.IconSetItem)e.Item).Image;
			e.Graphics.DrawImage(img, e.Bounds.Location);
		}
	}
}
