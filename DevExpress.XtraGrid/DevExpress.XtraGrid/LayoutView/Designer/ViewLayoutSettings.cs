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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraGrid.Views.Layout.Designer {
	[ToolboxItem(false)]
	public partial class ViewSettingsManager : UserControl {
		LayoutView targetView = null;
		LayoutViewCustomizer customizerCore = null;
		bool showSaveLoadLayoutGroup,showViewGroup, showLayoutGroup, showCardsGroup, showFieldsGroup;
		public ViewSettingsManager(LayoutViewCustomizer customizer, LayoutView targetView) {
			this.targetView = targetView;
			this.customizerCore = customizer;
			InitializeComponent();
			layoutControl1.BeginUpdate();
			LayoutControlItem btnLoadItem = saveLoadButtonsGroup.AddItem("btnLoadItem", Customizer.btnLoad);
			LayoutControlItem btnSaveItem = saveLoadButtonsGroup.AddItem("btnSaveItem", Customizer.btnSave);
			btnLoadItem.TextVisible = false;
			btnSaveItem.TextVisible = false;
			btnLoadItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 20, 10, 2);
			btnSaveItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 20, 3, 10);
			SetComponentsText();
			SetGroupsVisibility();
			SetDefaultValues();
			SubscribeEvents();
			this.BorderStyle = BorderStyle.None;
			layoutControl1.EndUpdate();
		}
		protected virtual void DoDispose() {
			UnSubscribeEvents();
			targetView=null;
			customizerCore=null;
		}
		protected LayoutViewCustomizer Customizer { get { return customizerCore; } }
		public bool AllGroupsHidden {
			get { return !showViewGroup && !showLayoutGroup && !showCardsGroup && !showFieldsGroup && !showSaveLoadLayoutGroup; }
		}
		protected void SetComponentsText() {
			viewGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupView);
			showLines.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowLines);
			showHeaderPanel.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowHeaderPanel);
			filterPanelItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowFilterPanel);
			scrollVisibilityItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelScrollVisibility);
			layoutGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupLayout);
			viewModeItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelViewMode);
			arrangeRuleItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelCardArrangeRule);
			cardEdgeAlignmentItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelCardEdgeAlignment);
			intervalsGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.IntervalsGroup);
			cardsGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupCards);
			showCardCaption.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowCardCaption);
			showExpandButton.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowCardExpandButton);
			showCardBorder.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowCardBorder);
			fieldsGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupFields);
			allowHotTrack.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelAllowFieldHotTracking);
			showFieldBorder.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowFieldBorder);
			showFieldHint.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelShowFieldHint);
		}
		protected bool CalcCustomizationVisibilityGroupSaveLoadLayout() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowSaveLoadLayoutButtons : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityGroupView() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupView : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityGroupLayout() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupLayout : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityGroupCards() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupCards : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityGroupFields() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupFields : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected void SetGroupsVisibility() {
			if(Customizer!=null && Customizer.EditingLayoutView!=null) {
				viewGroup.ExpandOnDoubleClick = true;
				layoutGroup.ExpandOnDoubleClick = true;
				cardsGroup.ExpandOnDoubleClick = true;
				fieldsGroup.ExpandOnDoubleClick = true;
				showSaveLoadLayoutGroup = CalcCustomizationVisibilityGroupSaveLoadLayout();
				showViewGroup = CalcCustomizationVisibilityGroupView();
				showLayoutGroup = CalcCustomizationVisibilityGroupLayout();
				showCardsGroup = CalcCustomizationVisibilityGroupCards();
				showFieldsGroup = CalcCustomizationVisibilityGroupFields();
				ConfigureGroupVisibility(saveLoadButtonsGroup, showSaveLoadLayoutGroup);
				if(showSaveLoadLayoutGroup) saveLoadButtonsGroup.GroupBordersVisible = false;
				ConfigureGroupVisibility(viewGroup, showViewGroup);
				ConfigureGroupVisibility(layoutGroup, showLayoutGroup);
				ConfigureGroupVisibility(cardsGroup, showCardsGroup);
				ConfigureGroupVisibility(fieldsGroup, showFieldsGroup);
				if(Customizer.previewSplitContainer!=null) {
					Customizer.previewSplitContainer.PanelVisibility = AllGroupsHidden ? SplitPanelVisibility.Panel2 : SplitPanelVisibility.Both;
				}
			}
		}
		protected void ConfigureGroupVisibility(LayoutControlGroup group, bool visible) {
			if(visible) {
				group.Visibility = LayoutVisibility.Always;
				group.Expanded = group.GroupBordersVisible = true;
			} else {
				group.Expanded = group.GroupBordersVisible = false;
				group.Visibility = LayoutVisibility.Never;
			}
		}
		protected void SetDefaultValues() {
			showCardBorder.Enabled = false;
			showCardBorder.EditValue = true;
			showCardCaption.EditValue = true;
			showExpandButton.EditValue = true;
			showFieldBorder.EditValue = false;
			showFieldHint.EditValue = true;
			showFilterPanel.EditValue = ShowFilterPanelMode.Default;
			scrollVisibility.EditValue = ScrollVisibility.Always;
			showHeaderPanel.EditValue = true;
			showLines.EditValue = true;
			allowHotTrack.EditValue = true;
			horzInterval.EditValue = 4;
			vertInterval.EditValue = 4;
			cardsAlignment.EditValue = CardsAlignment.Center;
			viewMode.EditValue = LayoutViewMode.SingleRecord;
			arrangeRule.EditValue = LayoutCardArrangeRule.ShowWholeCards;
			if(Customizer.EditingLayoutView!=null && Customizer.EditingLayoutView.OptionsView!=null) {
				showCardBorder.EditValue =  Customizer.EditingLayoutView.OptionsView.ShowCardBorderIfCaptionHidden;
				showCardCaption.EditValue =  Customizer.EditingLayoutView.OptionsView.ShowCardCaption;
				showExpandButton.EditValue =  Customizer.EditingLayoutView.OptionsView.ShowCardExpandButton;
				showFieldBorder.EditValue =  Customizer.EditingLayoutView.OptionsView.ShowCardFieldBorders;
				showFieldHint.EditValue = Customizer.EditingLayoutView.OptionsView.ShowFieldHints;
				scrollVisibility.EditValue = Customizer.EditingLayoutView.OptionsBehavior.ScrollVisibility;
				showFilterPanel.EditValue = Customizer.EditingLayoutView.OptionsView.ShowFilterPanelMode;
				showHeaderPanel.EditValue = Customizer.EditingLayoutView.OptionsView.ShowHeaderPanel;
				showLines.EditValue = Customizer.EditingLayoutView.OptionsView.ShowCardLines;
				allowHotTrack.EditValue = Customizer.EditingLayoutView.OptionsView.AllowHotTrackFields;
				cardsAlignment.EditValue = Customizer.EditingLayoutView.OptionsView.CardsAlignment;
				viewMode.EditValue = Customizer.EditingLayoutView.OptionsView.ViewMode;
				arrangeRule.EditValue = Customizer.EditingLayoutView.OptionsView.CardArrangeRule;
				horzInterval.EditValue = Customizer.EditingLayoutView.CardHorzInterval;
				vertInterval.EditValue = Customizer.EditingLayoutView.CardVertInterval;
			}
			cardsAlignment.Properties.Items.AddEnum<CardsAlignment>(Customizer.Localizer.Converter);
			scrollVisibility.Properties.Items.AddEnum<ScrollVisibility>(Customizer.Localizer.Converter);
			showFilterPanel.Properties.Items.AddEnum<ShowFilterPanelMode>(Customizer.Localizer.Converter);
			viewMode.Properties.Items.AddEnum<LayoutViewMode>(Customizer.Localizer.Converter);
			arrangeRule.Properties.Items.AddEnum<LayoutCardArrangeRule>(Customizer.Localizer.Converter);
			SetTextWithValueSuffix();
		}
		protected void SubscribeEvents() {
			foreach(Control tempControl in layoutControl1.Controls) {
				BaseEdit edit = tempControl as BaseEdit;
				if(edit != null) edit.EditValueChanged += new EventHandler(edit_EditValueChanged);
			}
		}
		protected void UnSubscribeEvents() {
			foreach(Control tempControl in layoutControl1.Controls) {
				BaseEdit edit = tempControl as BaseEdit;
				if(edit != null) edit.EditValueChanged -= new EventHandler(edit_EditValueChanged);
			}
		}
		protected void edit_EditValueChanged(object sender, EventArgs e) {
			showCardBorder.Enabled = !(bool)showCardCaption.EditValue;
			SetTextWithValueSuffix();
			ApplyChanges();
		}
		protected void SetTextWithValueSuffix() {
			string horzInt = Customizer.Localizer.StringByID(CustomizationStringID.LabelHorizontal);
			string vertInt = Customizer.Localizer.StringByID(CustomizationStringID.LabelVertical);
			horizontalIntervalItem.Text = horzInt +  GetValueSuffix((int)horzInterval.EditValue);
			verticalIntervalItem.Text = vertInt + GetValueSuffix((int)vertInterval.EditValue);
		}
		protected string GetValueSuffix(int value) {
			return " ("+value.ToString()+")";
		}
		protected void ApplyChanges() {
			targetView.BeginUpdate();
			if(targetView != null) {
				targetView.OptionsView.ViewMode = (LayoutViewMode)viewMode.EditValue;
				targetView.OptionsView.CardsAlignment = (CardsAlignment)cardsAlignment.EditValue;
				targetView.OptionsView.ShowCardBorderIfCaptionHidden = (bool)showCardBorder.EditValue;
				targetView.OptionsView.ShowCardCaption = (bool)showCardCaption.EditValue;
				targetView.OptionsView.ShowCardExpandButton = (bool)showExpandButton.EditValue;
				targetView.OptionsView.ShowCardFieldBorders = (bool)showFieldBorder.EditValue;
				targetView.OptionsView.ShowCardLines = (bool)showLines.EditValue;
				targetView.OptionsView.ShowCardCaption = (bool)showCardCaption.EditValue;
				targetView.OptionsView.ShowFieldHints =  (bool)showFieldHint.EditValue;
				targetView.OptionsView.ShowFilterPanelMode =  (ShowFilterPanelMode)showFilterPanel.EditValue;
				targetView.OptionsBehavior.ScrollVisibility = (ScrollVisibility)scrollVisibility.EditValue;
				targetView.OptionsView.ShowHeaderPanel =  (bool)showHeaderPanel.EditValue;
				targetView.OptionsView.AllowHotTrackFields = (bool)allowHotTrack.EditValue;
				targetView.OptionsView.CardArrangeRule = (LayoutCardArrangeRule)arrangeRule.EditValue;
				targetView.CardHorzInterval = (int)horzInterval.EditValue;
				targetView.CardVertInterval = (int)vertInterval.EditValue;
			}
			targetView.EndUpdate();
			Customizer.ModificationExist = true;
		}
	}   
}
