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
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Office.Internal;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Design.Internal;
#if !SL
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public partial class PageSetupFormControl : UserControl, IDialogContent {
		readonly PageSetupFormController controller;
		readonly PageSetupFormControllerParameters controllerParameters;
		readonly IRichEditControl control;
		readonly Dictionary<PaperKind, PaperKindComboBoxItem> paperKindItems = new Dictionary<PaperKind, PaperKindComboBoxItem>();
		public PageSetupFormControl() {
			InitializeComponent();
		}
		public PageSetupFormControl(PageSetupFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeControlsEvents();
		}
		#region Properties
		public PageSetupFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		#endregion
		protected internal virtual PageSetupFormController CreateController(PageSetupFormControllerParameters controllerParameters) {
			return new PageSetupFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			this.tabControl.SelectedIndex = (int)controllerParameters.InitialTabPage;
			UpdateForm();
		}
		void InitializeForm() {
			if (Controller == null)
				return;
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
			edtMarginLeft.DefaultUnitType = unit;
			edtMarginRight.DefaultUnitType = unit;
			edtMarginTop.DefaultUnitType = unit;
			edtMarginBottom.DefaultUnitType = unit;
			edtPaperWidth.DefaultUnitType = unit;
			edtPaperHeight.DefaultUnitType = unit;
			#region Set Min and Max value
			edtMarginLeft.MinValue = PageSetupFormDefaults.MinLeftAndRightMarginByDefault;
			edtMarginLeft.MaxValue = PageSetupFormDefaults.MaxLeftAndRightMarginByDefault;
			edtMarginRight.MinValue = PageSetupFormDefaults.MinLeftAndRightMarginByDefault;
			edtMarginRight.MaxValue = PageSetupFormDefaults.MaxLeftAndRightMarginByDefault;
			edtMarginTop.MinValue = PageSetupFormDefaults.MinTopAndBottomMarginByDefault;
			edtMarginTop.MaxValue = PageSetupFormDefaults.MaxTopAndBottomMarginByDefault;
			edtMarginBottom.MinValue = PageSetupFormDefaults.MinTopAndBottomMarginByDefault;
			edtMarginBottom.MaxValue = PageSetupFormDefaults.MaxTopAndBottomMarginByDefault;
			edtPaperWidth.MinValue = PageSetupFormDefaults.MinPaperWidthAndHeightByDefault;
			edtPaperWidth.MaxValue = PageSetupFormDefaults.MaxPaperWidthAndHeightByDefault;
			edtPaperHeight.MinValue = PageSetupFormDefaults.MinPaperWidthAndHeightByDefault;
			edtPaperHeight.MaxValue = PageSetupFormDefaults.MaxPaperWidthAndHeightByDefault;
			#endregion
			chkDifferentFirstPage.IsChecked = controller.DifferentFirstPage.HasValue && controller.DifferentFirstPage.Value;
			chkDifferentOddAndEvenPage.IsChecked = controller.DifferentOddAndEvenPages.HasValue && controller.DifferentOddAndEvenPages.Value;
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
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			chkPortrait.IsChecked = Controller.Landscape.HasValue && !controller.Landscape.Value;
			chkLandscape.IsChecked = Controller.Landscape.HasValue && controller.Landscape.Value;
			edtMarginLeft.Value = Controller.LeftMargin;
			edtMarginRight.Value = Controller.RightMargin;
			edtMarginTop.Value = Controller.TopMargin;
			edtMarginBottom.Value = Controller.BottomMargin;
			edtPaperWidth.Value = Controller.PaperWidth;
			edtPaperHeight.Value = Controller.PaperHeight;
			cbPaperSize.EditValue = GetEditValueByPaperSize(Controller.PaperKind);
		}
		object GetEditValueByPaperSize(PaperKind? paperKind) {
			if (!paperKind.HasValue)
				return null;
			PaperKindComboBoxItem result;
			if (paperKindItems.TryGetValue(paperKind.Value, out result))
				return result;
			result = new PaperKindComboBoxItem(paperKind.Value, String.Empty);
			return result;
		}
		public void SubscribeControlsEvents() {
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
			chkPortrait.Checked += OnOrientationChanged;
			chkPortrait.Unchecked += OnOrientationChanged;
			chkLandscape.Checked += OnOrientationChanged;
			chkLandscape.Unchecked += OnOrientationChanged;
		}
		public void UnsubscribeControlsEvents() {
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
			chkPortrait.Checked -= OnOrientationChanged;
			chkPortrait.Unchecked -= OnOrientationChanged;
			chkLandscape.Checked -= OnOrientationChanged;
			chkLandscape.Unchecked -= OnOrientationChanged;
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
			ListItemCollection items = combo.Items;
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
			if (Controller == null)
				return;
			combo.EditValue = null;
			ListItemCollection items = combo.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SectionPropertiesApplyTypeListBoxItem item = (SectionPropertiesApplyTypeListBoxItem)items[i];
				if (item.ApplyType == controller.ApplyType) {
					combo.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void AddApplyToComboItem(ListItemCollection items, SectionPropertiesApplyType applyType) {
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
			ResourceManager resourceManager = OfficeLocalizationHelper.CreateResourceManager(typeof(DevExpress.Data.ResFinder));
			IList<PaperKind> paperKindList = controller.FullPaperKindList;
			ListItemCollection items = cbPaperSize.Items;
			items.BeginUpdate();
			try {
				int count = paperKindList.Count;
				for (int i = 0; i < count; i++) {
					PaperKind paperKind = paperKindList[i];
					string displayName = OfficeLocalizationHelper.GetPaperKindString(resourceManager, paperKind);
					PaperKindComboBoxItem item = new PaperKindComboBoxItem(paperKind, displayName);
					items.Add(item);
					AddPaperKindItem(item);
				}
				AddPaperKindItem(new PaperKindComboBoxItem(PaperKind.Custom, OfficeLocalizationHelper.GetPaperKindString(resourceManager, PaperKind.Custom)));
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
		protected internal virtual void FillSectionStartTypeCombo() {
			ListItemCollection items = cbSectionStart.Items;
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
			if (Controller == null)
				return;
			cbSectionStart.EditValue = null;
			ListItemCollection items = cbSectionStart.Items;
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
			if (Controller == null)
				return;
			PaperKindComboBoxItem paperKindItem = cbPaperSize.EditValue as PaperKindComboBoxItem;
			Controller.PaperKind = paperKindItem != null ? paperKindItem.PaperKind : PaperKind.Custom;
			UpdateForm();
		}
		void OnEdtPaperWidthValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtPaperWidth.Value as int?;
			if (value.HasValue) {
				Controller.CustomWidth = value.Value;
				Controller.PaperWidth = value.Value;
				Controller.UpdatePaperKind();
				UpdateForm();
			}
		}
		void OnEdtPaperHeightValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtPaperHeight.Value as int?;
			if (value.HasValue) {
				Controller.CustomHeight = value.Value;
				Controller.PaperHeight = value.Value;
				Controller.UpdatePaperKind();
				UpdateForm();
			}
		}
		void OnEdtMarginLeftValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtMarginLeft.Value as int?;
			if (value.HasValue) {
				Controller.LeftMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginRightValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtMarginRight.Value as int?;
			if (value.HasValue) {
				Controller.RightMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginTopValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtMarginTop.Value as int?;
			if (value.HasValue) {
				Controller.TopMargin = value.Value;
				UpdateForm();
			}
		}
		void OnEdtMarginBottomValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int? value = edtMarginBottom.Value as int?;
			if (value.HasValue) {
				Controller.BottomMargin = value.Value;
				UpdateForm();
			}
		}
		void OnOrientationChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			Controller.Landscape = chkLandscape.IsChecked;
			UpdateForm();
		}
		protected internal virtual void CommitValuesToController() {
			if (Controller == null)
				return;
			SectionPropertiesApplyTypeListBoxItem applyTypeItem = cbMarginsApplyTo.SelectedItem as SectionPropertiesApplyTypeListBoxItem;
			if (applyTypeItem != null)
				controller.ApplyType = applyTypeItem.ApplyType;
			PageSetupSectionStartListBoxItem sectionStartItem = cbSectionStart.SelectedItem as PageSetupSectionStartListBoxItem;
			if (sectionStartItem != null)
				controller.SectionStartType = sectionStartItem.SectionStartType;
			if (chkDifferentFirstPage.IsChecked.HasValue && chkDifferentFirstPage.IsChecked.Value)
				controller.DifferentFirstPage = true;
			if (chkDifferentOddAndEvenPage.IsChecked.HasValue && chkDifferentOddAndEvenPage.IsChecked.Value)
				controller.DifferentOddAndEvenPages = true;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				CommitValuesToController();
				Controller.ApplyChanges();
			}
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			if (!Controller.IsTopBottomMarginsValid()) {
				Control.ShowWarningMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_TopBottomSectionMarginsTooLarge));
				return false;
			}
			if (!Controller.IsLeftRightMarginsValid()) {
				Control.ShowWarningMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_LeftRightSectionMarginsTooLarge));
				return false;
			}
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
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
