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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Localization;
#if !SL
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using System.Runtime.InteropServices;
using DevExpress.Utils.Design;
using System.ComponentModel;
using System.Resources;
#else
using DevExpress.Xpf.Core.Native;
#endif
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.tabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.tabPageMargins")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.tabPagePaper")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.tabPageLayout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMarginLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMarginTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMargins")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblOrientation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtMarginRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMarginRIght")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtMarginBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMarginBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtMarginLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtMarginTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.chkLandscape")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.chkPortrait")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtPaperHeight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.edtPaperWidth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblPaperHeight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblPaperWidth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblPaperSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.cbPaperSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblHeadersAndFooters")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblSectionStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.cbSectionStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.chkDifferentFirstPage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.chkDifferentOddAndEvenPage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblMarginsApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblPaperApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.cbLayoutApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.lblLayoutApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.cbMarginsApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.PageSetupForm.cbPaperApplyTo")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class PageSetupForm : XtraForm {
		readonly PageSetupFormController controller;
		readonly IRichEditControl control;
		readonly PaperKindConverter paperKindConverter = new PaperKindConverter(typeof(PaperKind));
		readonly Dictionary<PaperKind, PaperKindComboBoxItem> paperKindItems = new Dictionary<PaperKind,PaperKindComboBoxItem>();
		public PageSetupForm() {			
			InitializeComponent();
		}
		public PageSetupForm(PageSetupFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			InitializeForm();
			this.tabControl.SelectedTabPageIndex = (int)controllerParameters.InitialTabPage;
			SubscribeControlsEvents();
			UpdateForm();
		}
		protected internal PageSetupFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		protected internal virtual PageSetupFormController CreateController(PageSetupFormControllerParameters controllerParameters) {
			return new PageSetupFormController(controllerParameters);
		}
		void InitializeForm() {
			FillApplyToCombo(cbMarginsApplyTo);
			FillApplyToCombo(cbPaperApplyTo);
			FillApplyToCombo(cbLayoutApplyTo);
			SetApplyToComboInitialValue(cbMarginsApplyTo);
			SyncApplyToCombosCore(cbMarginsApplyTo.SelectedIndex);
			FillPaperKindCombo();
			FillSectionStartTypeCombo();
			SetSectionStartTypeComboInitialValue();
			edtMarginLeft.ValueUnitConverter = controller.ValueUnitConverter;
			edtMarginRight.ValueUnitConverter = controller.ValueUnitConverter;
			edtMarginTop.ValueUnitConverter = controller.ValueUnitConverter;
			edtMarginBottom.ValueUnitConverter = controller.ValueUnitConverter;
			edtPaperWidth.ValueUnitConverter = controller.ValueUnitConverter;
			edtPaperHeight.ValueUnitConverter = controller.ValueUnitConverter;
			DocumentUnit unit = Control.InnerControl.UIUnit;
			edtMarginLeft.Properties.DefaultUnitType = unit;
			edtMarginRight.Properties.DefaultUnitType = unit;
			edtMarginTop.Properties.DefaultUnitType = unit;
			edtMarginBottom.Properties.DefaultUnitType = unit;
			edtPaperWidth.Properties.DefaultUnitType = unit;
			edtPaperHeight.Properties.DefaultUnitType = unit;
			#region Set Min and Max value
			edtMarginLeft.Properties.MinValue = PageSetupFormDefaults.MinLeftAndRightMarginByDefault;
			edtMarginLeft.Properties.MaxValue = PageSetupFormDefaults.MaxLeftAndRightMarginByDefault;
			edtMarginRight.Properties.MinValue = PageSetupFormDefaults.MinLeftAndRightMarginByDefault;
			edtMarginRight.Properties.MaxValue = PageSetupFormDefaults.MaxLeftAndRightMarginByDefault;
			edtMarginTop.Properties.MinValue = PageSetupFormDefaults.MinTopAndBottomMarginByDefault;
			edtMarginTop.Properties.MaxValue = PageSetupFormDefaults.MaxTopAndBottomMarginByDefault;
			edtMarginBottom.Properties.MinValue = PageSetupFormDefaults.MinTopAndBottomMarginByDefault;
			edtMarginBottom.Properties.MaxValue = PageSetupFormDefaults.MaxTopAndBottomMarginByDefault;
			edtPaperWidth.Properties.MinValue = PageSetupFormDefaults.MinPaperWidthAndHeightByDefault;
			edtPaperWidth.Properties.MaxValue = PageSetupFormDefaults.MaxPaperWidthAndHeightByDefault;
			edtPaperHeight.Properties.MinValue = PageSetupFormDefaults.MinPaperWidthAndHeightByDefault;
			edtPaperHeight.Properties.MaxValue = PageSetupFormDefaults.MaxPaperWidthAndHeightByDefault;
			#endregion
			chkDifferentFirstPage.Checked = controller.DifferentFirstPage.HasValue && controller.DifferentFirstPage.Value;
			chkDifferentOddAndEvenPage.Checked = controller.DifferentOddAndEvenPages.HasValue && controller.DifferentOddAndEvenPages.Value;
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			chkPortrait.Checked = controller.Landscape.HasValue && !controller.Landscape.Value;
			chkLandscape.Checked = controller.Landscape.HasValue && controller.Landscape.Value;
			edtMarginLeft.Value = controller.LeftMargin;
			edtMarginRight.Value = controller.RightMargin;
			edtMarginTop.Value = controller.TopMargin;
			edtMarginBottom.Value = controller.BottomMargin;
			edtPaperWidth.Value = controller.PaperWidth;
			edtPaperHeight.Value = controller.PaperHeight;
			cbPaperSize.EditValue = GetEditValueByPaperSize(controller.PaperKind);
		}
		private object GetEditValueByPaperSize(PaperKind? paperKind) {
			if (!paperKind.HasValue)
				return null;
			PaperKindComboBoxItem result;
			if (paperKindItems.TryGetValue(paperKind.Value, out result))
				return result;
			result = new PaperKindComboBoxItem(paperKind.Value, String.Empty);
			return result;
		}
		protected internal virtual void SubscribeControlsEvents() {
			cbMarginsApplyTo.SelectedIndexChanged += OnCbMarginsApplyToSelectedIndexChanged;
			cbPaperApplyTo.SelectedIndexChanged += OnCbPaperApplyToSelectedIndexChanged;
			cbLayoutApplyTo.SelectedIndexChanged += OnCbLayoutApplyToSelectedIndexChanged;
			cbPaperSize.SelectedIndexChanged += OnCbPaperSizeSelectedIndexChanged;
			edtPaperWidth.ValueChanged += OnEdtPaperWidthValueChanged;
			edtPaperHeight.ValueChanged += OnEdtPaperHeightValueChanged;
			edtMarginLeft.ValueChanged += OnEdtMarginLeftValueChanged;
			edtMarginRight.ValueChanged += OnEdtMarginRightValueChanged;
			edtMarginTop.ValueChanged += OnEdtMarginTopValueChanged;
			edtMarginBottom.ValueChanged += OnEdtMarginBottomValueChanged;
			chkPortrait.CheckedChanged += OnOrientationChanged;
			chkLandscape.CheckedChanged += OnOrientationChanged;			
		}		
		protected internal virtual void UnsubscribeControlsEvents() {
			cbMarginsApplyTo.SelectedIndexChanged -= OnCbMarginsApplyToSelectedIndexChanged;
			cbPaperApplyTo.SelectedIndexChanged -= OnCbPaperApplyToSelectedIndexChanged;
			cbLayoutApplyTo.SelectedIndexChanged -= OnCbLayoutApplyToSelectedIndexChanged;
			cbPaperSize.SelectedIndexChanged -= OnCbPaperSizeSelectedIndexChanged;
			edtPaperWidth.ValueChanged -= OnEdtPaperWidthValueChanged;
			edtPaperHeight.ValueChanged -= OnEdtPaperHeightValueChanged;
			edtMarginLeft.ValueChanged -= OnEdtMarginLeftValueChanged;
			edtMarginRight.ValueChanged -= OnEdtMarginRightValueChanged;
			edtMarginTop.ValueChanged -= OnEdtMarginTopValueChanged;
			edtMarginBottom.ValueChanged -= OnEdtMarginBottomValueChanged;
			chkPortrait.CheckedChanged -= OnOrientationChanged;
			chkLandscape.CheckedChanged -= OnOrientationChanged;			
		}		
		void OnCbMarginsApplyToSelectedIndexChanged(object sender, EventArgs e) {
			SyncApplyToCombos(cbMarginsApplyTo.SelectedIndex);
		}
		void OnCbPaperApplyToSelectedIndexChanged(object sender, EventArgs e) {
			SyncApplyToCombos(cbPaperApplyTo.SelectedIndex);
		}
		void OnCbLayoutApplyToSelectedIndexChanged(object sender, EventArgs e) {
			SyncApplyToCombos(cbLayoutApplyTo.SelectedIndex);
		}
		protected internal virtual void FillApplyToCombo(ComboBoxEdit combo) {
			ComboBoxItemCollection items = combo.Properties.Items;
			items.BeginUpdate();
			try {
				AddApplyToComboItem(items, SectionPropertiesApplyType.WholeDocument);
				AddApplyToComboItem(items, SectionPropertiesApplyType.CurrentSection);
				AddApplyToComboItem(items, SectionPropertiesApplyType.SelectedSections);
				AddApplyToComboItem(items, SectionPropertiesApplyType.ThisPointForward);
			}
			finally {
				items.EndUpdate();
			}
		}
		protected internal virtual void SetApplyToComboInitialValue(ComboBoxEdit combo) {
			combo.EditValue = null;
			ComboBoxItemCollection items = combo.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SectionPropertiesApplyTypeListBoxItem item = (SectionPropertiesApplyTypeListBoxItem)items[i];
				if (item.ApplyType == controller.ApplyType) {
					combo.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void AddApplyToComboItem(ComboBoxItemCollection items, SectionPropertiesApplyType applyType) {
			if ((Controller.AvailableApplyType & applyType) == applyType)
				items.Add(new SectionPropertiesApplyTypeListBoxItem(applyType));
		}
		protected internal virtual void SyncApplyToCombos(int selectedIndex) {
			UnsubscribeControlsEvents();
			try {
				SyncApplyToCombosCore(selectedIndex);
			}
			finally {
				SubscribeControlsEvents();
			}
			CommitValuesToController();
		}
		protected internal virtual void SyncApplyToCombosCore(int selectedIndex) {
			cbMarginsApplyTo.SelectedIndex = selectedIndex;
			cbPaperApplyTo.SelectedIndex = selectedIndex;
			cbLayoutApplyTo.SelectedIndex = selectedIndex;
		}
		protected internal virtual void FillPaperKindCombo() {
			Type resourceFinder = typeof(DevExpress.Data.ResFinder);
			string resourceFileName = DXDisplayNameAttribute.DefaultResourceFile;
			ResourceManager resourceManager = new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), resourceFinder.Assembly);
			IList<PaperKind> paperKindList = controller.FullPaperKindList;
			ComboBoxItemCollection items = cbPaperSize.Properties.Items;
			items.BeginUpdate();
			try {
				int count = paperKindList.Count;
				for (int i = 0; i < count; i++) {
					PaperKind paperKind = paperKindList[i];
					string displayName = resourceManager.GetString(GetResourceName(paperKind));					
					PaperKindComboBoxItem item;
					item = new PaperKindComboBoxItem(paperKind, displayName);
					items.Add(new PaperKindComboBoxItem(paperKind, displayName));
					AddPaperKindItem(item);
				}
				AddPaperKindItem(new PaperKindComboBoxItem(PaperKind.Custom, resourceManager.GetString(GetResourceName(PaperKind.Custom))));
			}
			finally {
				items.EndUpdate();
			}
		}
		void AddPaperKindItem(PaperKindComboBoxItem item) {
			if (paperKindItems.ContainsKey(item.PaperKind))
				return;
			paperKindItems.Add(item.PaperKind, item);
		}
		internal static string GetResourceName(object enumValue) {
			return string.Concat(enumValue.GetType().FullName, ".", enumValue);
		}
		protected internal virtual void FillSectionStartTypeCombo() {
			ComboBoxItemCollection items = cbSectionStart.Properties.Items;
			items.BeginUpdate();
			try {
				items.Add(new PageSetupSectionStartListBoxItem(SectionStartType.Continuous));
				items.Add(new PageSetupSectionStartListBoxItem(SectionStartType.NextPage));
				items.Add(new PageSetupSectionStartListBoxItem(SectionStartType.EvenPage));
				items.Add(new PageSetupSectionStartListBoxItem(SectionStartType.OddPage));
			}
			finally {
				items.EndUpdate();
			}
		}
		protected internal virtual void SetSectionStartTypeComboInitialValue() {
			cbSectionStart.EditValue = null;
			ComboBoxItemCollection items = cbSectionStart.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				PageSetupSectionStartListBoxItem item = (PageSetupSectionStartListBoxItem)items[i];
				if (item.SectionStartType == controller.SectionStartType) {
					cbSectionStart.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void OnCbPaperSizeSelectedIndexChanged(object sender, EventArgs e) {
			PaperKindComboBoxItem item = cbPaperSize.EditValue as PaperKindComboBoxItem;
			PaperKind paperKind = item != null? item.PaperKind: PaperKind.Custom;
			Controller.PaperKind = paperKind;
			UpdateForm();
		}
		void OnEdtPaperWidthValueChanged(object sender, EventArgs e) {
			int? value = edtPaperWidth.Value as int?;
			if (value.HasValue) {
				Controller.CustomWidth = value.Value;
				Controller.PaperWidth = value.Value;
				Controller.UpdatePaperKind();
				UpdateForm();
			}
		}
		void OnEdtPaperHeightValueChanged(object sender, EventArgs e) {
			int? value = edtPaperHeight.Value as int?;
			if (value.HasValue) {
				Controller.CustomHeight = value.Value;
				Controller.PaperHeight = value.Value;
				Controller.UpdatePaperKind();
				UpdateForm();
			}
		}
		void OnEdtMarginLeftValueChanged(object sender, EventArgs e) {
			int? value = edtMarginLeft.Value as int?;
			if (value.HasValue) {
				Controller.LeftMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginRightValueChanged(object sender, EventArgs e) {
			int? value = edtMarginRight.Value as int?;
			if (value.HasValue) {
				Controller.RightMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginTopValueChanged(object sender, EventArgs e) {
			int? value = edtMarginTop.Value as int?;
			if (value.HasValue) {
				Controller.TopMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginBottomValueChanged(object sender, EventArgs e) {
			int? value = edtMarginBottom.Value as int?;
			if (value.HasValue) {
				Controller.BottomMargin = value.Value;
				UpdateForm();
			}
		}
		void OnOrientationChanged(object sender, EventArgs e) {
			Controller.Landscape = chkLandscape.Checked;
			UpdateForm();
		}
		protected internal virtual void CommitValuesToController() {
			SectionPropertiesApplyTypeListBoxItem applyTypeItem = cbMarginsApplyTo.SelectedItem as SectionPropertiesApplyTypeListBoxItem;
			if (applyTypeItem != null)
				controller.ApplyType = applyTypeItem.ApplyType;
			PageSetupSectionStartListBoxItem sectionStartItem = cbSectionStart.SelectedItem as PageSetupSectionStartListBoxItem;
			if (sectionStartItem != null)
				controller.SectionStartType = sectionStartItem.SectionStartType;
			if (chkDifferentFirstPage.Checked)
				controller.DifferentFirstPage = true;
			if (chkDifferentOddAndEvenPage.Checked)
				controller.DifferentOddAndEvenPages = true;
		}
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			CommitValuesToController();
			if (!Controller.IsTopBottomMarginsValid()) {
				Control.ShowWarningMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_TopBottomSectionMarginsTooLarge));
				return;
			}
			if (!Controller.IsLeftRightMarginsValid()) {
				Control.ShowWarningMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_LeftRightSectionMarginsTooLarge));
				return;
			}
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
	public class PaperKindComboBoxItem {
		public PaperKindComboBoxItem(PaperKind paperKind, string displayName) {
			PaperKind = paperKind;
			DisplayName = displayName;
		}
		public string DisplayName { get; private set; }
		public PaperKind PaperKind { get; private set; }
		public override bool Equals(object obj) {
			PaperKindComboBoxItem other = obj as PaperKindComboBoxItem;
			if (Object.ReferenceEquals(other, null))
				return false;
			return other.PaperKind == PaperKind;
		}
		public override int GetHashCode() {
			return PaperKind.GetHashCode();
		}
		public override string ToString() {
			return String.IsNullOrEmpty(DisplayName) ? PaperKind.ToString() : DisplayName;
		}
	}
}
