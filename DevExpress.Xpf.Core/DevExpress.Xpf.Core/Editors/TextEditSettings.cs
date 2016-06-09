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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Interop;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public class TextEditSettings : BaseEditSettings {
		#region static
		public static readonly DependencyProperty EditNonEditableTemplateProperty;
		public static readonly DependencyProperty MaskSaveLiteralProperty;
		public static readonly DependencyProperty MaskShowPlaceHoldersProperty;
		public static readonly DependencyProperty MaskPlaceHolderProperty;
		public static readonly DependencyProperty MaskProperty;
		public static readonly DependencyProperty MaskTypeProperty;
		public static readonly DependencyProperty MaskIgnoreBlankProperty;
		public static readonly DependencyProperty MaskUseAsDisplayFormatProperty;
		public static readonly DependencyProperty MaskBeepOnErrorProperty;
		public static readonly DependencyProperty MaskAutoCompleteProperty;
		public static readonly DependencyProperty MaskCultureProperty;
		public static readonly DependencyProperty AcceptsReturnProperty;
		public static readonly DependencyProperty MaxLengthProperty;
		public static readonly DependencyProperty TextDecorationsProperty;
#if !SL
		public static readonly DependencyProperty CharacterCasingProperty;
		public static readonly DependencyProperty TextTrimmingProperty;
		public static readonly DependencyProperty ShowTooltipForTrimmedTextProperty;
		public static readonly DependencyProperty TrimmedTextToolTipContentTemplateProperty;
#endif
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty PrintTextWrappingProperty;
		public static readonly DependencyProperty ValidateOnEnterKeyPressedProperty;
		public static readonly DependencyProperty ValidateOnTextInputProperty;
		public static readonly DependencyProperty AllowSpinOnMouseWheelProperty;
		static readonly DependencyPropertyKey HighlightedTextPropertyKey;
		public static readonly DependencyProperty HighlightedTextProperty;
		static readonly DependencyPropertyKey HighlightedTextCriteriaPropertyKey;
		public static readonly DependencyProperty HighlightedTextCriteriaProperty;
		static TextEditSettings() {
			Type ownerType = typeof(TextEditSettings);
			HighlightedTextCriteriaPropertyKey = DependencyPropertyManager.RegisterReadOnly("HighlightedTextCriteria", typeof(HighlightedTextCriteria), ownerType, new FrameworkPropertyMetadata(HighlightedTextCriteria.StartsWith, OnPropertyChanged));
			HighlightedTextCriteriaProperty = HighlightedTextCriteriaPropertyKey.DependencyProperty;
			HighlightedTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("HighlightedText", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, OnPropertyChanged));
			HighlightedTextProperty = HighlightedTextPropertyKey.DependencyProperty;
			AllowSpinOnMouseWheelProperty = DependencyPropertyManager.Register("AllowSpinOnMouseWheel", typeof(bool), ownerType, new PropertyMetadata(true));
			EditNonEditableTemplateProperty = DependencyPropertyManager.Register("EditNonEditableTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
			MaskSaveLiteralProperty = DependencyPropertyManager.Register("MaskSaveLiteral", typeof(bool), ownerType, new PropertyMetadata(true, OnPropertyChanged));
			MaskShowPlaceHoldersProperty = DependencyPropertyManager.Register("MaskShowPlaceHolders", typeof(bool), ownerType, new PropertyMetadata(true, OnPropertyChanged));
			MaskPlaceHolderProperty = DependencyPropertyManager.Register("MaskPlaceHolder", typeof(char), ownerType, new PropertyMetadata('_', OnPropertyChanged));
			MaskProperty = DependencyPropertyManager.Register("Mask", typeof(string), ownerType, new PropertyMetadata(String.Empty, OnPropertyChanged));
			MaskTypeProperty = DependencyPropertyManager.Register("MaskType", typeof(MaskType), ownerType, new PropertyMetadata(MaskType.None, OnMaskTypeChanged));
			MaskIgnoreBlankProperty = DependencyPropertyManager.Register("MaskIgnoreBlank", typeof(bool), ownerType, new PropertyMetadata(true, OnPropertyChanged));
			MaskUseAsDisplayFormatProperty = DependencyPropertyManager.Register("MaskUseAsDisplayFormat", typeof(bool), ownerType, new PropertyMetadata(false, OnPropertyChanged));
			MaskBeepOnErrorProperty = DependencyPropertyManager.Register("MaskBeepOnError", typeof(bool), ownerType, new PropertyMetadata(false, OnPropertyChanged));
			MaskAutoCompleteProperty = DependencyPropertyManager.Register("MaskAutoComplete", typeof(AutoCompleteType), ownerType, new PropertyMetadata(AutoCompleteType.Default, OnPropertyChanged));
			MaskCultureProperty = DependencyPropertyManager.Register("MaskCulture", typeof(CultureInfo), ownerType, new PropertyMetadata(null, OnPropertyChanged));
			AcceptsReturnProperty = TextEditBase.AcceptsReturnProperty.AddOwner(typeof(TextEditSettings), new FrameworkPropertyMetadata(false, OnSettingsPropertyChanged));
			TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(TextEditSettings), new PropertyMetadata(OnSettingsPropertyChanged));
			PrintTextWrappingProperty = DependencyPropertyManager.Register("PrintTextWrapping", typeof(TextWrapping?), typeof(TextEditSettings), new PropertyMetadata(null));
			MaxLengthProperty = DependencyPropertyManager.Register("MaxLength", typeof(int), ownerType, new FrameworkPropertyMetadata(0));
			TextDecorationsProperty = DependencyPropertyManager.Register("TextDecorations", typeof(TextDecorationCollection), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
#if !SL
			CharacterCasingProperty = DependencyPropertyManager.Register("CharacterCasing", typeof(CharacterCasing), ownerType, new FrameworkPropertyMetadata(CharacterCasing.Normal));
			TextTrimmingProperty = TextEditBase.TextTrimmingProperty.AddOwner(typeof(TextEditSettings), new PropertyMetadata(TextTrimming.CharacterEllipsis, OnSettingsPropertyChanged));
			ShowTooltipForTrimmedTextProperty = TextEditBase.ShowTooltipForTrimmedTextProperty.AddOwner(typeof(TextEditSettings), new PropertyMetadata(true));
			TrimmedTextToolTipContentTemplateProperty = DependencyPropertyManager.Register("TrimmedTextToolTipContentTemplate", typeof(DataTemplate),
				ownerType, new FrameworkPropertyMetadata(null));
#endif
			ValidateOnEnterKeyPressedProperty = DependencyPropertyManager.Register("ValidateOnEnterKeyPressed", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ValidateOnTextInputProperty = DependencyPropertyManager.Register("ValidateOnTextInput", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		static void OnMaskTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TextEditSettings)d).MaskTypeCore = (MaskType)e.NewValue;
			OnPropertyChanged(d, e);
		}
		static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		#endregion
		[Category(EditSettingsCategories.Behavior)]
		public bool AllowSpinOnMouseWheel {
			get { return (bool)GetValue(AllowSpinOnMouseWheelProperty); }
			set { SetValue(AllowSpinOnMouseWheelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsEditNonEditableTemplate"),
#endif
		Category(EditSettingsCategories.Behavior)]
		public ControlTemplate EditNonEditableTemplate {
			get { return (ControlTemplate)GetValue(EditNonEditableTemplateProperty); }
			set { SetValue(EditNonEditableTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskSaveLiteral"),
#endif
 Category(EditSettingsCategories.Format)]
		public bool MaskSaveLiteral {
			get { return (bool)GetValue(MaskSaveLiteralProperty); }
			set { SetValue(MaskSaveLiteralProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskAutoComplete"),
#endif
 Category(EditSettingsCategories.Format)]
		public AutoCompleteType MaskAutoComplete {
			get { return (AutoCompleteType)GetValue(MaskAutoCompleteProperty); }
			set { SetValue(MaskAutoCompleteProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskShowPlaceHolders"),
#endif
 Category(EditSettingsCategories.Format)]
		public bool MaskShowPlaceHolders {
			get { return (bool)GetValue(MaskShowPlaceHoldersProperty); }
			set { SetValue(MaskShowPlaceHoldersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskPlaceHolder"),
#endif
 Category(EditSettingsCategories.Format)]
		public char MaskPlaceHolder {
			get { return (char)GetValue(MaskPlaceHolderProperty); }
			set { SetValue(MaskPlaceHolderProperty, value); }
		}
		MaskType MaskTypeCore { get; set; }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskType"),
#endif
 Category(EditSettingsCategories.Format)]
		public MaskType MaskType {
			get { return (MaskType)GetValue(MaskTypeProperty); }
			set { SetValue(MaskTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMask"),
#endif
 Category(EditSettingsCategories.Format)]
		public string Mask {
			get { return (string)GetValue(MaskProperty); }
			set { SetValue(MaskProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskIgnoreBlank"),
#endif
 Category(EditSettingsCategories.Format)]
		public bool MaskIgnoreBlank {
			get { return (bool)GetValue(MaskIgnoreBlankProperty); }
			set { SetValue(MaskIgnoreBlankProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskUseAsDisplayFormat"),
#endif
 Category(EditSettingsCategories.Format)]
		public bool MaskUseAsDisplayFormat {
			get { return (bool)GetValue(MaskUseAsDisplayFormatProperty); }
			set { SetValue(MaskUseAsDisplayFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskBeepOnError"),
#endif
 Category(EditSettingsCategories.Format)]
		public bool MaskBeepOnError {
			get { return (bool)GetValue(MaskBeepOnErrorProperty); }
			set { SetValue(MaskBeepOnErrorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaskCulture"),
#endif
 Category(EditSettingsCategories.Format)]
		public CultureInfo MaskCulture {
			get { return (CultureInfo)GetValue(MaskCultureProperty); }
			set { SetValue(MaskCultureProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsTextWrapping"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
#if SL
		[TypeConverter(typeof(NullableConverter<TextWrapping>))]
#endif
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditSettingsPrintTextWrapping")]
#endif
		public TextWrapping? PrintTextWrapping {
			get { return (TextWrapping?)GetValue(PrintTextWrappingProperty); }
			set { SetValue(PrintTextWrappingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditSettingsValidateOnEnterKeyPressed")]
#endif
		public bool ValidateOnEnterKeyPressed {
			get { return (bool)GetValue(ValidateOnEnterKeyPressedProperty); }
			set { SetValue(ValidateOnEnterKeyPressedProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditSettingsValidateOnTextInput")]
#endif
		public bool ValidateOnTextInput {
			get { return (bool)GetValue(ValidateOnTextInputProperty); }
			set { SetValue(ValidateOnTextInputProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsAcceptsReturn"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool AcceptsReturn {
			get { return (bool)GetValue(AcceptsReturnProperty); }
			set { SetValue(AcceptsReturnProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsMaxLength"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public int MaxLength {
			get { return (int)base.GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsTextDecorations"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public TextDecorationCollection TextDecorations {
			get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public string HighlightedText {
			get { return (string)GetValue(HighlightedTextProperty); }
			internal set { this.SetValue(HighlightedTextPropertyKey, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return (HighlightedTextCriteria)GetValue(HighlightedTextCriteriaProperty); }
			internal set { this.SetValue(HighlightedTextCriteriaPropertyKey, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsCharacterCasing"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public CharacterCasing CharacterCasing {
			get { return (CharacterCasing)base.GetValue(CharacterCasingProperty); }
			set { SetValue(CharacterCasingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsTextTrimming"),
#endif
 Category(EditSettingsCategories.Behavior), SkipPropertyAssertion]
		public TextTrimming TextTrimming {
			get { return (TextTrimming)GetValue(TextBlock.TextTrimmingProperty); }
			set { SetValue(TextBlock.TextTrimmingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsShowTooltipForTrimmedText"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool ShowTooltipForTrimmedText {
			get { return (bool)GetValue(ShowTooltipForTrimmedTextProperty); }
			set { SetValue(ShowTooltipForTrimmedTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSettingsTrimmedTextToolTipContentTemplate"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public DataTemplate TrimmedTextToolTipContentTemplate {
			get { return (DataTemplate)GetValue(TrimmedTextToolTipContentTemplateProperty); }
			set { SetValue(TrimmedTextToolTipContentTemplateProperty, value); }
		}
#endif
		public TextEditSettings() {
			MaskTypeCore = MaskType;
		}
		protected internal override void AssignViewInfoProperties(IBaseEdit edit, IDefaultEditorViewInfo defaultViewInfo) {
			base.AssignViewInfoProperties(edit, defaultViewInfo);
			var inplaceBaseEdit = edit as IInplaceBaseEdit;
			if (inplaceBaseEdit != null) {
				inplaceBaseEdit.HasTextDecorations = defaultViewInfo.HasTextDecorations;
			}
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
#if !SL
			IInplaceBaseEdit inplaceEdit = edit as IInplaceBaseEdit;
			if (inplaceEdit != null) {
				inplaceEdit.TextTrimming = TextTrimming;
				inplaceEdit.TextWrapping = TextWrapping;
				inplaceEdit.HighlightedText = HighlightedText;
				inplaceEdit.HighlightedTextCriteria = HighlightedTextCriteria;
				if (!inplaceEdit.HasTextDecorations)
					inplaceEdit.TextDecorations = TextDecorations;
				inplaceEdit.ShowToolTipForTrimmedText = ShowTooltipForTrimmedText;
				return;
			}
#endif
			TextEdit te = edit as TextEdit;
			if (te == null)
				return;
			SetValueFromSettings(AllowSpinOnMouseWheelProperty, () => te.AllowSpinOnMouseWheel = AllowSpinOnMouseWheel);
			SetValueFromSettings(MaxLengthProperty, () => te.MaxLength = MaxLength);
			SetValueFromSettings(AcceptsReturnProperty, () => te.AcceptsReturn = AcceptsReturn);
			SetValueFromSettings(TextWrappingProperty, () => te.TextWrapping = TextWrapping);
			SetValueFromSettings(PrintTextWrappingProperty, () => te.PrintTextWrapping = PrintTextWrapping);
			SetValueFromSettings(TextDecorationsProperty, () => te.TextDecorations = TextDecorations);
			SetValueFromSettings(MaskAutoCompleteProperty, () => te.MaskAutoComplete = MaskAutoComplete);
			SetValueFromSettings(MaskCultureProperty, () => te.MaskCulture = MaskCulture);
			SetValueFromSettings(MaskProperty, () => te.Mask = Mask);
			SetValueFromSettings(MaskIgnoreBlankProperty, () => te.MaskIgnoreBlank = MaskIgnoreBlank);
			SetValueFromSettings(MaskTypeProperty, () => te.MaskType = this.MaskType);
			SetValueFromSettings(MaskPlaceHolderProperty, () => te.MaskPlaceHolder = MaskPlaceHolder);
			SetValueFromSettings(MaskSaveLiteralProperty, () => te.MaskSaveLiteral = MaskSaveLiteral);
			SetValueFromSettings(MaskShowPlaceHoldersProperty, () => te.MaskShowPlaceHolders = MaskShowPlaceHolders);
			SetValueFromSettings(MaskUseAsDisplayFormatProperty, () => te.MaskUseAsDisplayFormat = MaskUseAsDisplayFormat);
			SetValueFromSettings(ValidateOnTextInputProperty, () => te.ValidateOnTextInput = ValidateOnTextInput);
			SetValueFromSettings(ValidateOnEnterKeyPressedProperty, () => te.ValidateOnEnterKeyPressed = ValidateOnEnterKeyPressed);
#if !SL
			SetValueFromSettings(MaskBeepOnErrorProperty, () => te.MaskBeepOnError = MaskBeepOnError);
			SetValueFromSettings(CharacterCasingProperty, () => te.CharacterCasing = CharacterCasing);
			SetValueFromSettings(TextTrimmingProperty, () => te.TextTrimming = TextTrimming);
			SetValueFromSettings(ShowTooltipForTrimmedTextProperty, () => te.ShowTooltipForTrimmedText = ShowTooltipForTrimmedText);
			SetValueFromSettings(TrimmedTextToolTipContentTemplateProperty, 
				() => te.TrimmedTextToolTipContentTemplate = TrimmedTextToolTipContentTemplate,
				() => ClearEditorPropertyIfNeeded(te, BaseEdit.TrimmedTextToolTipContentTemplateProperty, TrimmedTextToolTipContentTemplateProperty));
#endif
			SetValueFromSettings(HighlightedTextCriteriaProperty, () => te.HighlightedTextCriteria = HighlightedTextCriteria);
			SetValueFromSettings(HighlightedTextProperty, () => te.HighlightedText = HighlightedText);
			SetValueFromSettings(EditNonEditableTemplateProperty,
				() => te.EditNonEditableTemplate = EditNonEditableTemplate,
				() => ClearEditorPropertyIfNeeded(te, TextEditBase.EditNonEditableTemplateProperty, EditNonEditableTemplateProperty));
		}
		public override string GetDisplayTextFromEditor(object editValue) {
			if (MaskTypeCore != MaskType.None) {
				Editor.EditValue = editValue;
				return (string)Editor.GetValue(BaseEdit.DisplayTextProperty);
			}
			return base.GetDisplayTextFromEditor(editValue);
		}
		protected internal override bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (IsPasteGesture(key, modifiers))
				return true;
			return base.IsActivatingKey(key, modifiers);
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class TextSettingsExtension : BaseSettingsExtension {
		public bool AllowSpinOnMouseWheel { get; set; }
		public bool AcceptsReturn { get; set; }
		public int MaxLength { get; set; }
		public CharacterCasing CharacterCasing { get; set; }
		public string DisplayFormat { get; set; }
		public string EditFormat { get; set; }
		public TextWrapping TextWrapping { get; set; }
		public TextTrimming TextTrimming { get; set; }
		public bool ShowTooltipForTrimmedText { get; set; }
		public IValueConverter DisplayTextConverter { get; set; }
		public bool MaskSaveLiteral { get; set; }
		public bool MaskShowPlaceHolders { get; set; }
		public char MaskPlaceHolder { get; set; }
		public string Mask { get; set; }
		public MaskType MaskType { get; set; }
		public bool MaskIgnoreBlank { get; set; }
		public bool MaskUseAsDisplayFormat { get; set; }
		public bool MaskBeepOnError { get; set; }
		public AutoCompleteType MaskAutoComplete { get; set; }
		public CultureInfo MaskCulture { get; set; }
		public TextDecorationCollection TextDecorations { get; set; }
		public bool ValidateOnTextInput { get; set; }
		public TextSettingsExtension() {
			AllowSpinOnMouseWheel = (bool)TextEditSettings.AllowSpinOnMouseWheelProperty.DefaultMetadata.DefaultValue;
			AcceptsReturn = (bool)TextEditSettings.AcceptsReturnProperty.DefaultMetadata.DefaultValue;
			CharacterCasing = (CharacterCasing)TextEditSettings.CharacterCasingProperty.DefaultMetadata.DefaultValue;
			MaxLength = (int)TextEditSettings.MaxLengthProperty.DefaultMetadata.DefaultValue;
			DisplayFormat = (string)TextEditSettings.DisplayFormatProperty.DefaultMetadata.DefaultValue;
			TextWrapping = (TextWrapping)TextEditSettings.TextWrappingProperty.DefaultMetadata.DefaultValue;
			TextTrimming = (TextTrimming)TextEditSettings.TextTrimmingProperty.GetMetadata(typeof(TextEditSettings)).DefaultValue;
			ShowTooltipForTrimmedText = (bool)TextEditSettings.ShowTooltipForTrimmedTextProperty.GetMetadata(typeof(TextEditSettings)).DefaultValue;
			MaskSaveLiteral = (bool)TextEditSettings.MaskSaveLiteralProperty.DefaultMetadata.DefaultValue;
			MaskShowPlaceHolders = (bool)TextEditSettings.MaskShowPlaceHoldersProperty.DefaultMetadata.DefaultValue;
			MaskPlaceHolder = (char)TextEditSettings.MaskPlaceHolderProperty.DefaultMetadata.DefaultValue;
			Mask = (string)TextEditSettings.MaskProperty.DefaultMetadata.DefaultValue;
			MaskType = (MaskType)TextEditSettings.MaskTypeProperty.DefaultMetadata.DefaultValue;
			MaskIgnoreBlank = (bool)TextEditSettings.MaskIgnoreBlankProperty.DefaultMetadata.DefaultValue;
			MaskUseAsDisplayFormat = (bool)TextEditSettings.MaskUseAsDisplayFormatProperty.DefaultMetadata.DefaultValue;
			MaskBeepOnError = (bool)TextEditSettings.MaskBeepOnErrorProperty.DefaultMetadata.DefaultValue;
			MaskAutoComplete = (AutoCompleteType)TextEditSettings.MaskAutoCompleteProperty.DefaultMetadata.DefaultValue;
			MaskCulture = (CultureInfo)TextEditSettings.MaskCultureProperty.DefaultMetadata.DefaultValue;
			ValidateOnTextInput = (bool)TextEditSettings.ValidateOnTextInputProperty.DefaultMetadata.DefaultValue;
		}
		protected sealed override BaseEditSettings CreateEditSettings() {
			TextEditSettings settings = CreateTextEditSettings();
			settings.AllowSpinOnMouseWheel = AllowSpinOnMouseWheel;
			settings.DisplayFormat = this.DisplayFormat;
			settings.TextWrapping = TextWrapping;
			settings.TextTrimming = TextTrimming;
			settings.ShowTooltipForTrimmedText = ShowTooltipForTrimmedText;
			settings.DisplayTextConverter = DisplayTextConverter;
			settings.AcceptsReturn = AcceptsReturn;
			settings.MaxLength = MaxLength;
			settings.CharacterCasing = CharacterCasing;
			settings.Mask = Mask;
			settings.MaskAutoComplete = MaskAutoComplete;
			settings.MaskBeepOnError = MaskBeepOnError;
			settings.MaskCulture = MaskCulture;
			settings.MaskIgnoreBlank = MaskIgnoreBlank;
			settings.MaskPlaceHolder = MaskPlaceHolder;
			settings.MaskSaveLiteral = MaskSaveLiteral;
			settings.MaskShowPlaceHolders = MaskShowPlaceHolders;
			settings.MaskType = MaskType;
			settings.MaskUseAsDisplayFormat = MaskUseAsDisplayFormat;
			settings.TextDecorations = TextDecorations;
			settings.ValidateOnTextInput = ValidateOnTextInput;
			return settings;
		}
		protected virtual TextEditSettings CreateTextEditSettings() {
			return new TextEditSettings();
		}
	}
}
#endif
