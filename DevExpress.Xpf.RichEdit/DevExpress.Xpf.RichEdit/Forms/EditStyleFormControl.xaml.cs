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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.RichEdit.UI;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class EditStyleFormControl : UserControl, IDialogContent, ILogicalOwner {
		readonly RichEditControl richEditControl;
		EditStyleFormController controller;
		Button dropDownButton;
		PopupMenu menuPopup;
		BarButtonItem fontButtonItem;
		BarButtonItem paragraphButtonItem;
		BarButtonItem tabsButtonItem;
		public EditStyleFormControl(EditStyleFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.richEditControl = (RichEditControl)controllerParameters.Control;
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded -= OnUnloaded;
			this.controller = CreateController(controllerParameters);
			InitializeBarItems();
			InitializeForm();
			UpdateForm();
		}
		#region Properties
		protected internal EditStyleFormController Controller { get { return controller; } }
		protected internal ParagraphStyle ParagraphStyle { get { return Controller.IntermediateParagraphStyle; } }
		protected internal CharacterStyle CharacterStyle { get { return Controller.IntermediateCharacterStyle; } }
		protected internal bool IsParagraphStyle { get { return Controller.IsParagraphStyle; } }
		protected internal ParagraphProperties ParagraphProperties {
			get { return IsParagraphStyle ? ParagraphStyle.ParagraphProperties : null; }
		}
		protected internal CharacterProperties CharacterProperties {
			get { return IsParagraphStyle ? ParagraphStyle.CharacterProperties : CharacterStyle.CharacterProperties; }
		}
		protected internal CharacterStyle CharacterStyleParent {
			get { return (cbParent.SelectedIndex != 0) ? ((CharacterStyle)cbParent.SelectedItem) : null; }
		}
		public List<FontFamily> FontFamilyItems {
			get {
				List<FontFamily> list = new List<FontFamily>();
#if SL
				foreach (string fontFamily in DevExpress.Utils.Internal.FontManager.GetFontFamilyNames())
					list.Add(new FontFamily(fontFamily));
#else
				foreach (string fontFamily in DevExpress.Office.Internal.FontManager.GetFontNames())
					list.Add(new FontFamily(fontFamily));
#endif
				list.Sort(new FontFamilyComparer());
				return list;
			}
		}
		public int?[] FontSizeItems {
			get {
				return new int?[] { 
					null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 30,
					32, 34, 36, 38, 40, 44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 88, 96, 104, 112, 120, 128, 136, 144
				};
			}
		}
		#endregion
		public class FontFamilyComparer : IComparer<FontFamily> {
			#region IComparer<FontFamily> Members
			int IComparer<FontFamily>.Compare(FontFamily x, FontFamily y) {
				return x.ToString().CompareTo(y.ToString());
			}
			#endregion
		}
		protected internal virtual EditStyleFormController CreateController(EditStyleFormControllerParameters controllerParameters) {
			return new EditStyleFormController(previewRichEditControl, controllerParameters);
		}
		protected virtual void ApplyChanges() {
			if (Controller != null)
				Controller.ApplyChanges();
			UpdateRibbonStyleGalleryItems();
		}
		void UpdateRibbonStyleGalleryItems() {
			if (this.richEditControl.BarManager == null)
				return;
			IEnumerable<GalleryStyleItem> items = BarNameScope.GetService<IElementRegistratorService>(this.richEditControl.BarManager).GetElements<IFrameworkInputElement>().OfType<GalleryStyleItem>();
			if (items.Count<GalleryStyleItem>() > 0)
				((RichEditStyleGallery)items.First().Gallery).UpdateStyleItems();
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
		private void InitializeBarItems() {
			fontButtonItem = new BarButtonItem();
			paragraphButtonItem = new BarButtonItem();
			tabsButtonItem = new BarButtonItem();
			menuPopup = new PopupMenu();
		}
		void ButtonsEnabled(bool value) {
			cbNextStyle.IsEnabled = value;
			paragraphButtonItem.IsEnabled = value;
			tabsButtonItem.IsEnabled = value;
		}
		private void InitializeForm() {
			previewRichEditControl.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			Controller.FillTempRichEdit(previewRichEditControl);
			Controller.ChangePreviewControlCurrentStyle(previewRichEditControl);
			edtName.Text = Controller.StyleName;
			FillStyleCombo(cbCurrentStyle);
			if (IsParagraphStyle) {
				ButtonsEnabled(true);
				ParagraphStyleCollection styleCollection = Controller.ParagraphSourceStyle.DocumentModel.ParagraphStyles;
				FillStyleCombo<ParagraphStyle>(cbParent, styleCollection, IsParagraphParentValid);
				FillStyleCombo<ParagraphStyle>(cbNextStyle, styleCollection, IsNextValid);
			}
			else {
				if (!IsParagraphStyle)
					cbParent.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_EmptyParentStyle));
				FillStyleCombo<CharacterStyle>(cbParent, Controller.CharacterSourceStyle.DocumentModel.CharacterStyles, IsCharacterParentValid);
				ButtonsEnabled(false);
			}
			UpdateRichEditBars();
			ApplyPicturesToBarItems();
		}
		private bool IsParagraphParentValid(ParagraphStyle style) {
			return Controller.ParagraphSourceStyle.IsParentValid(style);
		}
		private bool IsCharacterParentValid(CharacterStyle style) {
			return Controller.CharacterSourceStyle.IsParentValid(style);
		}
		private bool IsNextValid(ParagraphStyle style) {
			return true;
		}
		void FillCurrentStyleCombo<T>(ComboBoxEdit comboBoxEdit, StyleCollectionBase<T> styles) where T : StyleBase<T> {
			for (int i = 0; i < styles.Count; i++) {
				if (IsStyleValid(styles[i]))
					comboBoxEdit.Items.Add(styles[i]);
			}
		}
		protected internal virtual bool IsStyleValid(IStyle style) {
			bool isStyleValid = style.Deleted || style.Hidden || style.Semihidden;
			CharacterStyle currentStyle = style as CharacterStyle;
			if (currentStyle != null)
				return !isStyleValid && !currentStyle.HasLinkedStyle;
			else
				return !isStyleValid;
		}
		protected internal virtual void FillStyleCombo(ComboBoxEdit comboBoxEdit) {
			ListItemCollection collection = comboBoxEdit.Items;
			collection.BeginUpdate();
			try {
				DocumentModel model = Controller.Control.InnerControl.DocumentModel;
				FillCurrentStyleCombo<ParagraphStyle>(comboBoxEdit, model.ParagraphStyles);
				FillCurrentStyleCombo<CharacterStyle>(comboBoxEdit, model.CharacterStyles);
			}
			finally {
				collection.EndUpdate();
			}
		}
		protected internal virtual void FillStyleCombo<T>(ComboBoxEdit comboBoxEdit, StyleCollectionBase<T> styleCollection, Predicate<T> match) where T : StyleBase<T> {
			ListItemCollection collection = comboBoxEdit.Items;
			collection.BeginUpdate();
			try {
				int count = styleCollection.Count;
				for (int i = 0; i < count; i++)
					if (match(styleCollection[i]))
						collection.Add(styleCollection[i]);
			}
			finally {
				collection.EndUpdate();
			}
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
		protected internal void UnsubscribeToggleButtonsEvents() {
			toggleFontBoldItem.CheckedChanged -= toggleFontBoldItem_CheckedChanged;
			toggleFontItalicItem.CheckedChanged -= toggleFontItalicItem_CheckedChanged;
			toggleFontUnderlineItem.CheckedChanged -= toggleFontUnderlineItem_CheckedChanged;
		}
		protected internal void SubscribeToggleButtonsEvents() {
			toggleFontBoldItem.CheckedChanged += toggleFontBoldItem_CheckedChanged;
			toggleFontItalicItem.CheckedChanged += toggleFontItalicItem_CheckedChanged;
			toggleFontUnderlineItem.CheckedChanged += toggleFontUnderlineItem_CheckedChanged;
		}
		protected internal void UnsubscribeFontButtonsEvents() {
			fontEditItem.EditValueChanged -= fontEditItem_EditValueChanged;
			colorEditItem.EditValueChanged -= colorEditItem_EditValueChanged;
			fontSizeEditItem.EditValueChanged -= fontSizeEditItem_EditValueChanged;
		}
		protected internal void SubscribeFontButtonsEvents() {
			fontEditItem.EditValueChanged += fontEditItem_EditValueChanged;
			colorEditItem.EditValueChanged += colorEditItem_EditValueChanged;
			fontSizeEditItem.EditValueChanged += fontSizeEditItem_EditValueChanged;
		}
		protected internal void UpdateCharacterBars(CharacterFormattingInfo mergedCharacterProperties) {
			UnsubscribeToggleButtonsEvents();
			toggleFontBoldItem.IsChecked = mergedCharacterProperties.FontBold;
			toggleFontItalicItem.IsChecked = mergedCharacterProperties.FontItalic;
			toggleFontUnderlineItem.IsChecked = (mergedCharacterProperties.FontUnderlineType == UnderlineType.Single);
			SubscribeToggleButtonsEvents();
			UnsubscribeFontButtonsEvents();
			fontEditItem.EditValue = CharacterProperties.FontName;
			colorEditItem.EditValue = DevExpress.Office.Internal.XpfTypeConverter.ToPlatformColor(CharacterProperties.ForeColor);
			fontSizeEditItem.EditValue = CharacterProperties.DoubleFontSize / 2f;
			SubscribeFontButtonsEvents();
		}
		void BarEnabled(bool value) {
			toggleParagraphAlignmentLeftItem.IsEnabled = value;
			toggleParagraphAlignmentCenterItem.IsEnabled = value;
			toggleParagraphAlignmentRightItem.IsEnabled = value;
			toggleParagraphAlignmentJustifyItem.IsEnabled = value;
			barLineSpacingSubItem.IsEnabled = value;
			spacingDecreaseItem.IsEnabled = value;
			spacingIncreaseItem.IsEnabled = value;
			indentDecreaseItem.IsEnabled = value;
			indentIncreaseItem.IsEnabled = value;
		}
		protected internal void UpdateRichEditBars() {
			CharacterFormattingInfo mergedCharacterProperties;
			if (Controller.IsParagraphStyle) {
				BarEnabled(true);
				mergedCharacterProperties = ParagraphStyle.GetMergedCharacterProperties().Info;
				UpdateCharacterBars(mergedCharacterProperties);
				ParagraphAlignment alignment = ParagraphStyle.GetMergedParagraphProperties().Info.Alignment;
				UnsubscribeParagraphAlignmentEvents();
				toggleParagraphAlignmentLeftItem.IsChecked = (alignment == ParagraphAlignment.Left);
				toggleParagraphAlignmentCenterItem.IsChecked = (alignment == ParagraphAlignment.Center);
				toggleParagraphAlignmentRightItem.IsChecked = (alignment == ParagraphAlignment.Right);
				toggleParagraphAlignmentJustifyItem.IsChecked = (alignment == ParagraphAlignment.Justify);
				SubscribeParagraphAlignmentEvents();
				UpdateParagraphLineSpacing();
			}
			else {
				mergedCharacterProperties = CharacterStyle.GetMergedCharacterProperties().Info;
				UpdateCharacterBars(mergedCharacterProperties);
				BarEnabled(false);
			}
		}
		protected internal virtual void UpdateParagraphLineSpacing() {
			if (!this.IsParagraphStyle)
				return;
			if (changeParagraphLineSingleSpacingItem != null && changeParagraphLineSingleHalfSpacingItem != null && changeParagraphLineDoubleSpacingItem != null) {
				UnsubscribeParagraphLineSpacingEvents();
				ParagraphLineSpacing spacing = ParagraphProperties.LineSpacingType;
				changeParagraphLineSingleSpacingItem.IsChecked = (spacing == ParagraphLineSpacing.Single);
				changeParagraphLineSingleHalfSpacingItem.IsChecked = (spacing == ParagraphLineSpacing.Sesquialteral);
				changeParagraphLineDoubleSpacingItem.IsChecked = (spacing == ParagraphLineSpacing.Double);
				SubscribeParagraphLineSpacingEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
			edtName.EditValueChanged += OnStyleNameEditValueChanged;
			cbCurrentStyle.SelectedIndexChanged += OnCurrentStyleSelectedIndexChanged;
			cbParent.SelectedIndexChanged += OnParentStyleSelectedIndexChanged;
			if (IsParagraphStyle)
				cbNextStyle.SelectedIndexChanged += OnNextStyleSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			edtName.EditValueChanged -= OnStyleNameEditValueChanged;
			cbCurrentStyle.SelectedIndexChanged -= OnCurrentStyleSelectedIndexChanged;
			cbParent.SelectedIndexChanged -= OnParentStyleSelectedIndexChanged;
			if (IsParagraphStyle)
				cbNextStyle.SelectedIndexChanged -= OnNextStyleSelectedIndexChanged;
		}
		protected internal virtual void UpdateFormCore() {
			edtName.Text = Controller.StyleName;
			if (IsParagraphStyle) {
				cbCurrentStyle.SelectedItem = Controller.ParagraphSourceStyle;
				cbParent.SelectedItem = Controller.ParagraphSourceStyle.Parent;
				cbNextStyle.SelectedItem = Controller.ParagraphSourceStyle.NextParagraphStyle;
			}
			else {
				cbCurrentStyle.SelectedItem = Controller.CharacterSourceStyle;
				if (Controller.CharacterSourceStyle.Parent == null)
					cbParent.SelectedIndex = 0;
				else
					cbParent.SelectedItem = Controller.CharacterSourceStyle.Parent;
			}
		}
		protected internal virtual void OnStyleNameEditValueChanged(object sender, EventArgs e) {
			Controller.StyleName = edtName.Text;
		}
		protected internal virtual void OnCurrentStyleSelectedIndexChanged(object sender, EventArgs e) {
			cbCurrentStyle.SelectedIndexChanged -= OnCurrentStyleSelectedIndexChanged;
			cbCurrentStyle.Items.Clear();
			cbParent.Items.Clear();
			cbNextStyle.Items.Clear();
			cbNextStyle.SelectedIndex = -1;
			previewRichEditControl.Text = String.Empty;
			string styleName = ((IStyle)cbCurrentStyle.SelectedItem).StyleName;
			if (cbCurrentStyle.SelectedItem is ParagraphStyle)
				Controller.Parameters.ParagraphSourceStyle = controller.Model.ParagraphStyles.GetStyleByName(styleName);
			else
				Controller.Parameters.CharacterSourceStyle = controller.Model.CharacterStyles.GetStyleByName(styleName);
			this.controller = CreateController(Controller.Parameters);
			InitializeForm();
			cbCurrentStyle.SelectedIndexChanged += OnCurrentStyleSelectedIndexChanged;
			UpdateForm();
		}
		protected internal virtual void OnParentStyleSelectedIndexChanged(object sender, EventArgs e) {
			if (IsParagraphStyle)
				Controller.IntermediateParagraphStyle.Parent = (ParagraphStyle)cbParent.SelectedItem;
			else
				Controller.IntermediateCharacterStyle.Parent = CharacterStyleParent;
			UpdateRichEditBars();
		}
		protected internal virtual void OnNextStyleSelectedIndexChanged(object sender, EventArgs e) {
			Controller.IntermediateParagraphStyle.NextParagraphStyle = (ParagraphStyle)cbNextStyle.SelectedItem;
		}
		void LoadBarItem(BarButtonItem item, ItemClickEventHandler hendler, RichEditControlStringId caption) {
			if (item != null) {
				item.Content = XpfRichEditLocalizer.GetString(caption);
				item.ItemClick += hendler;
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			((DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings)fontEditItem.EditSettings).ItemsSource = FontFamilyItems;
			((DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings)fontSizeEditItem.EditSettings).ItemsSource = FontSizeItems;
			LoadBarItem(fontButtonItem, FontClick, RichEditControlStringId.Caption_EditStyleFormFontButton);
			LoadBarItem(paragraphButtonItem, ParagraphClick, RichEditControlStringId.Caption_EditStyleFormParagraphButton);
			LoadBarItem(tabsButtonItem, TabsClick, RichEditControlStringId.Caption_EditStyleFormTabsButton);
			if (dropDownButton == null) {
				dropDownButton = CreateDropDownButton();
				if (dropDownButton != null)
					dropDownButton.Click += DropDownButtonClick;
			}
			if (changeParagraphLineSingleSpacingItem == null)
				changeParagraphLineSingleSpacingItem = FindSpacingItem(barLineSpacingSubItem.ItemLinksSource, "changeParagraphLineSingleSpacingItem");
			if (changeParagraphLineSingleHalfSpacingItem == null)
				changeParagraphLineSingleHalfSpacingItem = FindSpacingItem(barLineSpacingSubItem.ItemLinksSource, "changeParagraphLineSingleHalfSpacingItem");
			if (changeParagraphLineDoubleSpacingItem == null)
				changeParagraphLineDoubleSpacingItem = FindSpacingItem(barLineSpacingSubItem.ItemLinksSource, "changeParagraphLineDoubleSpacingItem");
			UpdateForm();
			UpdateParagraphLineSpacing();
		}
		BarCheckItem FindSpacingItem(System.Collections.IEnumerable itemLinksSource, string name) {
			foreach (BarCheckItem item in itemLinksSource)
				if (item.Name == name)
					return item;
			return null;
		}
		void UnloadBarItem(BarButtonItem item, ItemClickEventHandler hendler) {
			if (item != null) {
				item.ItemClick -= hendler;
				item = null;
			}
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnloadBarItem(fontButtonItem, FontClick);
			UnloadBarItem(paragraphButtonItem, ParagraphClick);
			UnloadBarItem(tabsButtonItem, TabsClick);
		}
		void CreateButtonLink(BarButtonItem buttonItem) {
			menuPopup.Items.Add(buttonItem);
		}
		Button CreateDropDownButton() {
			CreateButtonLink(fontButtonItem);
			CreateButtonLink(paragraphButtonItem);
			CreateButtonLink(tabsButtonItem);
#if SL
			DXDialog dialog = FloatingContainer.GetDialogOwner(this) as DXDialog;
			if (dialog != null) {
				Panel footer = dialog.OkButton.Parent as Panel;
				if (footer != null) {
					Button dropDownBtn = new Button();
					dropDownBtn.Content = XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_EditStyleFormFormatButton);
					dropDownBtn.HorizontalAlignment = HorizontalAlignment.Left;
					dropDownBtn.SetSize(new Size(dialog.OkButton.ActualWidth, dialog.OkButton.ActualHeight));
					StyleManager.SetApplyApplicationTheme(dropDownBtn, true);
					footer.Children.Add(dropDownBtn);
					menuPopup.PlacementTarget = dropDownBtn;
					return dropDownBtn;
				}
			}
#else
			FloatingContainer container = FloatingContainer.GetDialogOwner(this) as FloatingContainer;
			if (container != null) {
				DialogControl dialog = container.Content as DialogControl;
				if (dialog != null) {
					dialog.ShowApplyButton = true;
					dialog.ApplyButton.Content = XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_EditStyleFormFormatButton);
					menuPopup.PlacementTarget = dialog.ApplyButton;
					return dialog.ApplyButton;
				}
			}
#endif
			return null;
		}
		void ShowForm(DialogClosedDelegate onFormClosed, UserControl form, RichEditControlStringId caption) {
			FloatingContainerParameters floatingParameters = new FloatingContainerParameters();
			floatingParameters.ClosedDelegate = onFormClosed;
			floatingParameters.Title = XpfRichEditLocalizer.GetString(caption);
			floatingParameters.CloseOnEscape = true;
			floatingParameters.AllowSizing = true;
#if SL
			FloatingContainer.ShowDialogContent(form, previewRichEditControl, System.Windows.Size.Empty, floatingParameters, true);
#else
			FloatingContainer container = FloatingContainer.ShowDialogContent(form, this, System.Windows.Size.Empty, floatingParameters, true);
			container.SizeToContent = SizeToContent.WidthAndHeight;
#endif
		}
		#region ILogicalOwner Members
#if SL
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
		}
		bool ILogicalOwner.IsLoaded { get { return true; } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add { } remove { } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
#else
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
#endif
		#endregion
		void FontClick(object sender, RoutedEventArgs e) {
			MergedCharacterProperties mergedProperties = Controller.IntermediateParagraphStyle.GetMergedWithDefaultCharacterProperties();
			FontFormControllerParameters parameters = new FontFormControllerParameters(richEditControl, mergedProperties);
			RichEditFontControl fontForm = new RichEditFontControl(parameters);
			DocumentModel model = previewRichEditControl.InnerControl.DocumentModel;
			model.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						Controller.CopyCharacterPropertiesFromMerged(mergedProperties);
					UpdateRichEditBars();
				};
				ShowForm(onFormClosed, fontForm, RichEditControlStringId.Caption_FontForm);
			}
			finally {
				model.EndUpdate();
			}
		}
		void ParagraphClick(object sender, RoutedEventArgs e) {
			MergedParagraphProperties mergedProperties = Controller.IntermediateParagraphStyle.GetMergedWithDefaultParagraphProperties();
			DocumentModel model = previewRichEditControl.InnerControl.DocumentModel;
			ParagraphFormControllerParameters parameters = new ParagraphFormControllerParameters(richEditControl, mergedProperties, model.UnitConverter);
			ParagraphFormControl paragraphForm = new ParagraphFormControl(parameters);
			model.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						Controller.CopyParagraphPropertiesFromMerged(mergedProperties);
					UpdateRichEditBars();
				};
				ShowForm(onFormClosed, paragraphForm, RichEditControlStringId.Caption_ParagraphForm);
			}
			finally {
				model.EndUpdate();
			}
		}
		void TabsClick(object sender, RoutedEventArgs e) {
			TabFormattingInfo info = Controller.IntermediateParagraphStyle.GetTabs();
			DocumentModel model = previewRichEditControl.InnerControl.DocumentModel;
			int defaultTabWidth = model.DocumentProperties.DefaultTabWidth;
			TabsFormControllerParameters parameters = new TabsFormControllerParameters(richEditControl, info, defaultTabWidth, model.UnitConverter);
			TabsFormControl tabsForm = new TabsFormControl(parameters);
			model.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						Controller.IntermediateParagraphStyle.Tabs.SetTabs(info);
					UpdateRichEditBars();
				};
				ShowForm(onFormClosed, tabsForm, RichEditControlStringId.Caption_TabsForm);
			}
			finally {
				model.EndUpdate();
			}
		}
		private void toggleFontBoldItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeCurrentValue<bool>(new FontBoldAccessor(), (bool)toggleFontBoldItem.IsChecked);
		}
		private void toggleFontItalicItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			Controller.ChangeCurrentValue<bool>(new FontItalicAccessor(), (bool)toggleFontItalicItem.IsChecked);
		}
		private void toggleFontUnderlineItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			if ((bool)toggleFontUnderlineItem.IsChecked)
				Controller.ChangeCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.Single);
			else
				Controller.ChangeCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), UnderlineType.None);
		}
		void ChangeActiveItem(BarCheckItem item, ParagraphAlignment alignment) {
			UnsubscribeParagraphAlignmentEvents();
			try {
				UncheckedAllParagraphAligmentButtons();
				item.IsChecked = true;
				ParagraphProperties.Alignment = alignment;
			}
			finally {
				SubscribeParagraphAlignmentEvents();
			}
		}
		void ChangeActiveItem(BarCheckItem item, ParagraphLineSpacing spacing) {
			UnsubscribeParagraphLineSpacingEvents();
			try {
				UncheckedAllParagraphLineSpacingButtons();
				item.IsChecked = true;
				ParagraphProperties.LineSpacingType = spacing;
			}
			finally {
				SubscribeParagraphLineSpacingEvents();
			}
		}
		protected internal virtual void UnsubscribeParagraphAlignmentEvents() {
			toggleParagraphAlignmentLeftItem.CheckedChanged -= toggleParagraphAlignmentLeftItem_CheckedChanged;
			toggleParagraphAlignmentCenterItem.CheckedChanged -= toggleParagraphAlignmentCenterItem_CheckedChanged;
			toggleParagraphAlignmentRightItem.CheckedChanged -= toggleParagraphAlignmentRightItem_CheckedChanged;
			toggleParagraphAlignmentJustifyItem.CheckedChanged -= toggleParagraphAlignmentJustifyItem_CheckedChanged;
		}
		private void UncheckedAllParagraphAligmentButtons() {
			toggleParagraphAlignmentLeftItem.IsChecked = false;
			toggleParagraphAlignmentCenterItem.IsChecked = false;
			toggleParagraphAlignmentRightItem.IsChecked = false;
			toggleParagraphAlignmentJustifyItem.IsChecked = false;
		}
		protected internal virtual void SubscribeParagraphAlignmentEvents() {
			toggleParagraphAlignmentLeftItem.CheckedChanged += toggleParagraphAlignmentLeftItem_CheckedChanged;
			toggleParagraphAlignmentCenterItem.CheckedChanged += toggleParagraphAlignmentCenterItem_CheckedChanged;
			toggleParagraphAlignmentRightItem.CheckedChanged += toggleParagraphAlignmentRightItem_CheckedChanged;
			toggleParagraphAlignmentJustifyItem.CheckedChanged += toggleParagraphAlignmentJustifyItem_CheckedChanged;
		}
		private void toggleParagraphAlignmentLeftItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphAlignment.Left);
		}
		private void toggleParagraphAlignmentCenterItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphAlignment.Center);
		}
		private void toggleParagraphAlignmentRightItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphAlignment.Right);
		}
		private void toggleParagraphAlignmentJustifyItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphAlignment.Justify);
		}
		protected internal virtual void UnsubscribeParagraphLineSpacingEvents() {
			changeParagraphLineSingleSpacingItem.CheckedChanged -= changeParagraphLineSingleSpacingItem_CheckedChanged;
			changeParagraphLineSingleHalfSpacingItem.CheckedChanged -= changeParagraphLineSingleHalfSpacingItem_CheckedChanged;
			changeParagraphLineDoubleSpacingItem.CheckedChanged -= changeParagraphLineDoubleSpacingItem_CheckedChanged;
		}
		private void UncheckedAllParagraphLineSpacingButtons() {
			changeParagraphLineSingleSpacingItem.IsChecked = false;
			changeParagraphLineSingleHalfSpacingItem.IsChecked = false;
			changeParagraphLineDoubleSpacingItem.IsChecked = false;
		}
		protected internal virtual void SubscribeParagraphLineSpacingEvents() {
			changeParagraphLineSingleSpacingItem.CheckedChanged += changeParagraphLineSingleSpacingItem_CheckedChanged;
			changeParagraphLineSingleHalfSpacingItem.CheckedChanged += changeParagraphLineSingleHalfSpacingItem_CheckedChanged;
			changeParagraphLineDoubleSpacingItem.CheckedChanged += changeParagraphLineDoubleSpacingItem_CheckedChanged;
		}
		private void changeParagraphLineSingleSpacingItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphLineSpacing.Single);
		}
		private void changeParagraphLineSingleHalfSpacingItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphLineSpacing.Sesquialteral);
		}
		private void changeParagraphLineDoubleSpacingItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			ChangeActiveItem((BarCheckItem)sender, ParagraphLineSpacing.Double);
		}
		private void spacingIncreaseItem_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.IncreaseSpacing();
		}
		private void spacingDecreaseItem_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.DecreaseSpacing();
		}
		private void indentDecreaseItem_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.DecreaseIndent();
		}
		private void indentIncreaseItem_ItemClick(object sender, ItemClickEventArgs e) {
			Controller.IncreaseIndent();
		}
		ImageSource LoadSmallImageToGlyph(string name) {
#if SL
			Image barImage;
#else
			System.Drawing.Image barImage;
#endif
			barImage = DevExpress.Utils.CommandResourceImageLoader.LoadSmallImage("DevExpress.XtraRichEdit.Images", name, typeof(RichEditCommand).Assembly);
			return DevExpress.Xpf.Core.Native.ImageHelper.CreatePlatformImage(barImage).Source;
		}
		private void ApplyPicturesToBarItems() {
			toggleFontBoldItem.Glyph = LoadSmallImageToGlyph("Bold");
			toggleFontItalicItem.Glyph = LoadSmallImageToGlyph("Italic");
			toggleFontUnderlineItem.Glyph = LoadSmallImageToGlyph("Underline");
			toggleParagraphAlignmentLeftItem.Glyph = LoadSmallImageToGlyph("AlignLeft");
			toggleParagraphAlignmentCenterItem.Glyph = LoadSmallImageToGlyph("AlignCenter");
			toggleParagraphAlignmentRightItem.Glyph = LoadSmallImageToGlyph("AlignRight");
			toggleParagraphAlignmentJustifyItem.Glyph = LoadSmallImageToGlyph("AlignJustify");
			spacingIncreaseItem.Glyph = LoadSmallImageToGlyph("LineSpacing");
			spacingDecreaseItem.Glyph = LoadSmallImageToGlyph("SpacingDecrease");
			indentDecreaseItem.Glyph = LoadSmallImageToGlyph("IndentDecrease");
			indentIncreaseItem.Glyph = LoadSmallImageToGlyph("IndentIncrease");
			colorEditItem.Glyph = LoadSmallImageToGlyph("FontColor");
		}
		private void fontEditItem_EditValueChanged(object sender, RoutedEventArgs e) {
			string fontName = ((FontFamily)(fontEditItem.EditValue)).Source;
			Controller.ChangeCurrentValue<string>(new FontNameAccessor(), fontName);
		}
		private void fontSizeEditItem_EditValueChanged(object sender, RoutedEventArgs e) {
			string text;
			int value;
			if (EditStyleHelper.IsFontSizeValid(fontSizeEditItem.EditValue, out text, out value))
				Controller.ChangeCurrentValue<int>(new DoubleFontSizeAccessor(), value);
			else {
				fontSizeEditItem.EditValue = CharacterProperties.DoubleFontSize / 2f;
			}
		}
		private void colorEditItem_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			Color currentColor = (Color)colorEditItem.EditValue;
			CharacterProperties.ForeColor =
#if SL
 currentColor;
#else
 DevExpress.Office.Internal.XpfTypeConverter.ToPlatformIndependentColor(currentColor);
#endif
		}
		private void DropDownButtonClick(object sender, RoutedEventArgs e) {
			menuPopup.IsOpen = true;
#if !SL
			menuPopup.StaysOpen = false;
#else
			CaptureMouse((ContentControl)Parent);
#endif
		}
#if SL
		private void CaptureMouse(ContentControl parent) {
			parent.CaptureMouse();
			parent.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(menuPopup_MouseLeftButtonDown);
		}
		void menuPopup_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			menuPopup.IsOpen = false;
			((ContentControl)Parent).ReleaseMouseCapture();
		}
#endif
	}
}
