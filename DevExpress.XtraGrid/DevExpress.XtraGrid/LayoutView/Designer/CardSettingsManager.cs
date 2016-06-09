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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout.Frames;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraGrid.Views.Layout.Designer {
	[ToolboxItem(false)]
	public partial class DesignerControlSettingsManager : UserControl {
		LayoutControl targetLayoutControl = null;
		LayoutViewCustomizer customizerCore = null;
		bool showIndentsGroup;
		bool showCaptionGroup;
		bool showResetShrinkButtons;
		string textShowCustForm, textHideCustForm;
		protected internal bool fCustomizationVisible = false;
		public DesignerControlSettingsManager(LayoutViewCustomizer customizer, LayoutControl targetLayoutControl) {
			this.targetLayoutControl = targetLayoutControl;
			this.customizerCore = customizer;
			((ILayoutControl)targetLayoutControl).DefaultValues = ((ILayoutControl)Customizer.EditingView).DefaultValues;
			InitializeComponent();
			layoutControl1.BeginUpdate();
			SetComponentsText();
			SetGroupsVisibility();
			SetDefaultValues();
			SubscribeEvents();
			this.BorderStyle = BorderStyle.None;
			this.layoutControl1.Root.GroupBordersVisible = false;
			layoutControl1.EndUpdate();
		}
		protected void DoDispose() {
			UnSubscribeEvents();
			targetLayoutControl = null;
			customizerCore = null;
		}
		public LayoutViewCustomizer Customizer { get { return customizerCore; } }
		protected void SetComponentsText() {
			textShowCustForm = Customizer.Localizer.StringByID(CustomizationStringID.ButtonCustomizeShow);
			textHideCustForm = Customizer.Localizer.StringByID(CustomizationStringID.ButtonCustomizeHide);
			resetCardBtn.Text = Customizer.Localizer.StringByID(CustomizationStringID.ButtonReset);
			shrinkBtn.Text = Customizer.Localizer.StringByID(CustomizationStringID.ButtonShrinkToMinimum);
			indentGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupIndents);
			captionGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupCaptions);
			paddingItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelPadding);
			spacingItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelSpacing);
			textIndentItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelTextIndent);
			captionLocationsItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelCaptionLocation);
			GroupCaptionsLocationItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelGroupCaptionLocation);
			fieldCaptionAlignmentItem.Text = Customizer.Localizer.StringByID(CustomizationStringID.LabelTextAlignment);
		}
		protected bool CalcCustomizationVisibilityResetShrinkButtonsGroup() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization
				? Customizer.EditingLayoutView.OptionsCustomization.ShowResetShrinkButtons : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityIndentsGroup() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupCardIndents : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityCaptionGroup() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupCardCaptions : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected void SetGroupsVisibility() {
			if(Customizer != null && Customizer.EditingLayoutView != null) {
				indentGroup.ExpandOnDoubleClick = true;
				captionGroup.ExpandOnDoubleClick = true;
				showResetShrinkButtons = CalcCustomizationVisibilityResetShrinkButtonsGroup();
				showIndentsGroup = CalcCustomizationVisibilityIndentsGroup();
				showCaptionGroup = CalcCustomizationVisibilityCaptionGroup();
				ConfigureGroupVisibility(resetShrinkButtonsGroup, showResetShrinkButtons);
				if(showResetShrinkButtons) resetShrinkButtonsGroup.GroupBordersVisible = false;
				ConfigureGroupVisibility(indentGroup, showIndentsGroup);
				ConfigureGroupVisibility(captionGroup, showCaptionGroup);
			}
			if(Customizer.designerSplitContainer1 != null) {
				Customizer.designerSplitContainer1.PanelVisibility = AllGroupsHidden ? SplitPanelVisibility.Panel2 : SplitPanelVisibility.Both;
			}
		}
		protected void SetTextWithValueSuffix() {
			string paddingText = Customizer.Localizer.StringByID(CustomizationStringID.LabelPadding);
			string spacingText = Customizer.Localizer.StringByID(CustomizationStringID.LabelSpacing);
			string textIndentText = Customizer.Localizer.StringByID(CustomizationStringID.LabelTextIndent);
			paddingItem.Text = paddingText + GetValueSuffix((int)PaddingControl.EditValue);
			spacingItem.Text = spacingText + GetValueSuffix((int)SpacingControl.EditValue);
			textIndentItem.Text = textIndentText + GetValueSuffix((int)TextToControlDistanceControl.EditValue);
		}
		protected string GetValueSuffix(int value) {
			return " (" + value.ToString() + ")";
		}
		public bool AllGroupsHidden {
			get { return !showIndentsGroup && !showCaptionGroup && !showResetShrinkButtons; }
		}
		protected void ConfigureGroupVisibility(LayoutControlGroup group, bool visible) {
			if(visible) {
				group.Visibility = LayoutVisibility.Always;
				group.Expanded = group.GroupBordersVisible = true;
			}
			else {
				group.Expanded = group.GroupBordersVisible = false;
				group.Visibility = LayoutVisibility.Never;
			}
		}
		protected internal virtual void SetDefaultValues() {
			LockOptionsChanging();
			SpacingControl.EditValue = 2;
			PaddingControl.EditValue = 2;
			TextToControlDistanceControl.EditValue = 5;
			SetLocationsDefaultValues();
			SetLayoutGenerateOptions();
			SetAlignmentDefaultValues();
			if(targetLayoutControl != null) {
				if(targetLayoutControl.Root != null && targetLayoutControl.Root.Items.Count > 0) {
					SpacingControl.EditValue = targetLayoutControl.Root.Items[0].Spacing.All;
					PaddingControl.EditValue = targetLayoutControl.Root.Items[0].Padding.All;
					ItemsCaptionLocationControl.EditValue = targetLayoutControl.Root.Items[0].TextLocation;
					GroupsCaptionLocationControl.EditValue = targetLayoutControl.Root.TextLocation;
				}
				CardTextAlignment.EditValue = Customizer.EditingLayoutView.OptionsItemText.AlignMode;
				TextToControlDistanceControl.EditValue = Customizer.EditingLayoutView.OptionsItemText.TextToControlDistance;
			}
			SetTextWithValueSuffix();
			UnlockOptionsChanging();
		}
		protected void SetLayoutGenerateOptions() {
			defaultColumnCount.EditValue = Customizer.EditingLayoutView.OptionsView.DefaultColumnCount; 
		}
		protected void SetLocationsDefaultValues() {
			ItemsCaptionLocationControl.Properties.Items.AddEnum<Locations>(Customizer.Localizer.Converter);
			GroupsCaptionLocationControl.Properties.Items.AddEnum<Locations>(Customizer.Localizer.Converter);
			GroupsCaptionLocationControl.EditValue = Locations.Default;
			ItemsCaptionLocationControl.EditValue = Locations.Default;
		}
		protected void SetAlignmentDefaultValues() {
			CardTextAlignment.Properties.Items.AddEnum<FieldTextAlignMode>(Customizer.Localizer.Converter);
			CardTextAlignment.EditValue = FieldTextAlignMode.AlignInGroups;
		}
		protected void SubscribeEvents() {
			foreach(Control tempControl in layoutControl1.Controls) {
				BaseEdit edit = tempControl as BaseEdit;
				if(edit != null) edit.EditValueChanged += OnOptionsEditValueChanged;
			}
			targetLayoutControl.ShowCustomization += OnTargetLayoutControlShowCustomization;
			targetLayoutControl.HideCustomization += OnTargetLayoutControlHideCustomization;
		}
		protected void UnSubscribeEvents() {
			foreach(Control tempControl in layoutControl1.Controls) {
				BaseEdit edit = tempControl as BaseEdit;
				if(edit != null) edit.EditValueChanged -= OnOptionsEditValueChanged;
			}
			targetLayoutControl.ShowCustomization -= OnTargetLayoutControlShowCustomization;
			targetLayoutControl.HideCustomization -= OnTargetLayoutControlHideCustomization;
		}
		int lockOptionsCounter;
		public void LockOptionsChanging() {
			lockOptionsCounter++;
		}
		public void UnlockOptionsChanging() {
			--lockOptionsCounter;
		}
		public bool IsOptionsChangingLocked {
			get { return lockOptionsCounter > 0; }
		}
		protected void OnOptionsEditValueChanged(object sender, EventArgs e) {
			if(IsOptionsChangingLocked) return;
			LockOptionsChanging();
			if(sender == PaddingControl) ApplyChanges(true, false, false, false);
			else if(sender == SpacingControl) ApplyChanges(false, true, false, false);
			else if(sender == ItemsCaptionLocationControl) ApplyChanges(false, false, true, false);
			else if(sender == GroupsCaptionLocationControl) ApplyChanges(false, false, false, true);
			else ApplyChanges(false, false, false, false);
			SetTextWithValueSuffix();
			UnlockOptionsChanging();
		}
		public void ApplyChanges(bool changePadding, bool changeSpacing, bool changeItemTextLocation, bool changeGroupTexpLocation) {
			targetLayoutControl.BeginUpdate();
			if(targetLayoutControl != null) {
				targetLayoutControl.OptionsItemText.TextAlignMode = Customizer.EditingLayoutView.OptionsItemText.ConvertTo((FieldTextAlignMode)CardTextAlignment.EditValue);
				targetLayoutControl.OptionsItemText.TextToControlDistance = (int)TextToControlDistanceControl.EditValue;
				int padding = (int)PaddingControl.EditValue;
				int spacing = (int)SpacingControl.EditValue;
				Locations itemsTextLocation = (Locations)ItemsCaptionLocationControl.EditValue;
				Locations groupsTextLocation = (Locations)GroupsCaptionLocationControl.EditValue;
				foreach(BaseLayoutItem item in targetLayoutControl.Items) {
					if(changePadding) item.Padding = new DevExpress.XtraLayout.Utils.Padding(padding);
					if(changeSpacing && item != targetLayoutControl.Root) item.Spacing = new DevExpress.XtraLayout.Utils.Padding(spacing);
					LayoutControlItem lci = item as LayoutControlItem;
					if(lci != null) {
						if(changeItemTextLocation) lci.TextLocation = itemsTextLocation;
					}
					LayoutGroup group = item as LayoutGroup;
					if(group != null) {
						if(changeGroupTexpLocation) group.TextLocation = groupsTextLocation;
					}
				}
			}
			Customizer.EditingLayoutView.OptionsView.DefaultColumnCount = Convert.ToInt32(defaultColumnCount.EditValue);
			targetLayoutControl.EndUpdate();
			Customizer.ModificationExist = true;
			targetLayoutControl.Invalidate();
		}
		protected internal void custimizeButton_Click(object sender, EventArgs e) {
			ActivateCustomization();
		}
		protected internal void ActivateCustomization() {
			Customizer.DesignerHelper.LockModification();
			if(!fCustomizationVisible) targetLayoutControl.ShowCustomizationForm();
			Customizer.DesignerHelper.UnLockModification();
		}
		protected void OnTargetLayoutControlHideCustomization(object sender, EventArgs e) {
			fCustomizationVisible = false;
		}
		protected void OnTargetLayoutControlShowCustomization(object sender, EventArgs e) {
			fCustomizationVisible = true;
		}
		protected internal void resetCardBtn_Click(object sender, EventArgs e) {
			using(new WaitCursor()) {
				Customizer.DesignerView.OptionsView.DefaultColumnCount = Customizer.EditingLayoutView.OptionsView.DefaultColumnCount;
				Customizer.DesignerHelper.ResetView(Customizer.DesignerView);
				Customizer.DesignerView.CardMinSize = Customizer.DesignerView.ScaleSize(new Size(200, 20));
				Customizer.DesignerView.Refresh();
				Customizer.DesignerHelper.SynchronizeDesignerFromView(Customizer.DesignerView);
				Customizer.DesignerHelper.PlaceInParentControl(Customizer.designerScroller.ClientSize);
				Customizer.DesignerHelper.sizeGrip.CustomizedControlSize = Size.Empty;
				Customizer.ModificationExist = true;
			}
		}
		protected internal void shrinkBtn_Click(object sender, EventArgs e) {
			using(new WaitCursor()) {
				Customizer.DesignerHelper.DesignerControl.ResetResizer();
				var minSize = Customizer.DesignerHelper.DesignerControl.Root.MinSize;
				Customizer.DesignerHelper.DesignerControl.Size = minSize;
				Customizer.DesignerHelper.sizeGrip.MinControlSize = minSize;
				Customizer.DesignerHelper.sizeGrip.CustomizedControlSize = minSize;
				Customizer.DesignerHelper.PlaceInParentControl(Customizer.designerScroller.ClientSize);
				targetLayoutControl.Invalidate();
				Customizer.ModificationExist = true;
			}
		}
	}
}
