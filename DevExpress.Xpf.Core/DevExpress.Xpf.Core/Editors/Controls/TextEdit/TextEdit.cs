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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class TextEdit : TextEditBase {
		public static readonly DependencyProperty MaskSaveLiteralProperty;
		public static readonly DependencyProperty MaskShowPlaceHoldersProperty;
		public static readonly DependencyProperty MaskPlaceHolderProperty;
		public static readonly DependencyProperty MaskProperty;
		public static readonly DependencyProperty MaskTypeProperty;
		public static readonly DependencyProperty MaskIgnoreBlankProperty;
		public static readonly DependencyProperty MaskUseAsDisplayFormatProperty;
		public static readonly DependencyProperty MaskAutoCompleteProperty;
		public static readonly DependencyProperty MaskCultureProperty;
		public static readonly DependencyProperty TextDecorationsProperty;
		public static readonly DependencyProperty AllowSpinOnMouseWheelProperty;
		static readonly DependencyPropertyKey HighlightedTextPropertyKey;
		public static readonly DependencyProperty HighlightedTextProperty;
		static readonly DependencyPropertyKey HighlightedTextCriteriaPropertyKey;
		public static readonly DependencyProperty HighlightedTextCriteriaProperty;
		public static readonly RoutedEvent SpinEvent;
#if !SL
		public static readonly DependencyProperty CharacterCasingProperty;
		public static readonly DependencyProperty MaskBeepOnErrorProperty;
#endif
		static TextEdit() {
			Type ownerType = typeof(TextEdit);
			HighlightedTextCriteriaPropertyKey = DependencyPropertyManager.RegisterReadOnly("HighlightedTextCriteria", typeof(HighlightedTextCriteria), ownerType, new FrameworkPropertyMetadata(HighlightedTextCriteria.StartsWith, (d, e) => ((TextEdit)d).HighlightedTextCriteriaChanged((HighlightedTextCriteria)e.NewValue)));
			HighlightedTextCriteriaProperty = HighlightedTextCriteriaPropertyKey.DependencyProperty;
			HighlightedTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("HighlightedText", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, (d, e) => ((TextEdit)d).HighlightedTextChanged((string)e.NewValue)));
			HighlightedTextProperty = HighlightedTextPropertyKey.DependencyProperty;
			AllowSpinOnMouseWheelProperty = DependencyPropertyManager.Register("AllowSpinOnMouseWheel", typeof(bool), ownerType, new PropertyMetadata(true));
			MaskSaveLiteralProperty = DependencyPropertyManager.Register("MaskSaveLiteral", typeof(bool), ownerType, new PropertyMetadata(true, OnMaskPropertyChanged));
			MaskShowPlaceHoldersProperty = DependencyPropertyManager.Register("MaskShowPlaceHolders", typeof(bool), ownerType, new PropertyMetadata(true, OnMaskPropertyChanged));
			MaskPlaceHolderProperty = DependencyPropertyManager.Register("MaskPlaceHolder", typeof(char), ownerType, new PropertyMetadata('_', OnMaskPropertyChanged));
			MaskProperty = DependencyPropertyManager.Register("Mask", typeof(string), ownerType, new PropertyMetadata(String.Empty, OnMaskPropertyChanged));
			MaskTypeProperty = DependencyPropertyManager.Register("MaskType", typeof(MaskType), ownerType, new PropertyMetadata(MaskType.None, OnMaskTypePropertyChanged, new CoerceValueCallback(OnCoerceMaskType)));
			MaskIgnoreBlankProperty = DependencyPropertyManager.Register("MaskIgnoreBlank", typeof(bool), ownerType, new PropertyMetadata(true, OnMaskPropertyChanged));
			MaskUseAsDisplayFormatProperty = DependencyPropertyManager.Register("MaskUseAsDisplayFormat", typeof(bool), ownerType, new PropertyMetadata(false, OnMaskPropertyChanged));
			MaskAutoCompleteProperty = DependencyPropertyManager.Register("MaskAutoComplete", typeof(AutoCompleteType), ownerType, new PropertyMetadata(AutoCompleteType.Default, OnMaskPropertyChanged));
			MaskCultureProperty = DependencyPropertyManager.Register("MaskCulture", typeof(CultureInfo), ownerType, new PropertyMetadata(null, OnMaskPropertyChanged));
			TextDecorationsProperty = DependencyPropertyManager.Register("TextDecorations", typeof(TextDecorationCollection), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextDecorationsChanged));
			SpinEvent = EventManager.RegisterRoutedEvent("Spin", RoutingStrategy.Direct, typeof(SpinEventHandler), ownerType);
#if !SL
			MaskBeepOnErrorProperty = DependencyPropertyManager.Register("MaskBeepOnError", typeof(bool), ownerType, new PropertyMetadata(false, OnMaskPropertyChanged));
			CharacterCasingProperty = DependencyPropertyManager.Register("CharacterCasing", typeof(CharacterCasing), ownerType, new FrameworkPropertyMetadata(CharacterCasing.Normal, (d, e) => ((TextEdit)d).OnCharacterCasingChanged((CharacterCasing)e.NewValue)));
#endif
		}
		static object OnCoerceMaskType(DependencyObject d, object maskType) {
			return ((TextEdit)d).OnCoerceMaskType((MaskType)maskType);
		}
		static void OnMaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TextEdit)d).MaskPropertiesChanged();
		}
		static void OnMaskTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TextEdit)d).MaskTypeChanged((MaskType)e.NewValue);
		}
		static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TextEdit)d).OnTextDecorationsChanged();
		}
		protected internal override Type StyleSettingsType { get { return typeof(TextEditStyleSettings); } }
		new TextEditPropertyProvider PropertyProvider { get { return base.PropertyProvider as TextEditPropertyProvider; } }
		[Category("Behavior")]
		public bool AllowSpinOnMouseWheel {
			get { return (bool)GetValue(AllowSpinOnMouseWheelProperty); }
			set { SetValue(AllowSpinOnMouseWheelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskSaveLiteral"),
#endif
 Category("Mask")]
		public bool MaskSaveLiteral {
			get { return (bool)GetValue(MaskSaveLiteralProperty); }
			set { SetValue(MaskSaveLiteralProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskAutoComplete"),
#endif
 Category("Mask")]
		public AutoCompleteType MaskAutoComplete {
			get { return (AutoCompleteType)GetValue(MaskAutoCompleteProperty); }
			set { SetValue(MaskAutoCompleteProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskShowPlaceHolders"),
#endif
 Category("Mask")]
		public bool MaskShowPlaceHolders {
			get { return (bool)GetValue(MaskShowPlaceHoldersProperty); }
			set { SetValue(MaskShowPlaceHoldersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskPlaceHolder"),
#endif
 Category("Mask")]
		public char MaskPlaceHolder {
			get { return (char)GetValue(MaskPlaceHolderProperty); }
			set { SetValue(MaskPlaceHolderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskType"),
#endif
 Category("Mask")]
		public MaskType MaskType {
			get { return (MaskType)GetValue(MaskTypeProperty); }
			set { SetValue(MaskTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMask"),
#endif
 Category("Mask")]
		public string Mask {
			get { return (string)GetValue(MaskProperty); }
			set { SetValue(MaskProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskIgnoreBlank"),
#endif
 Category("Mask")]
		public bool MaskIgnoreBlank {
			get { return (bool)GetValue(MaskIgnoreBlankProperty); }
			set { SetValue(MaskIgnoreBlankProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskUseAsDisplayFormat"),
#endif
 Category("Mask")]
		public bool MaskUseAsDisplayFormat {
			get { return (bool)GetValue(MaskUseAsDisplayFormatProperty); }
			set { SetValue(MaskUseAsDisplayFormatProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskBeepOnError"),
#endif
 Category("Mask")]
		public bool MaskBeepOnError {
			get { return (bool)GetValue(MaskBeepOnErrorProperty); }
			set { SetValue(MaskBeepOnErrorProperty, value); }
		}
#endif
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditMaskCulture"),
#endif
 Category("Mask")]
		public CultureInfo MaskCulture {
			get { return (CultureInfo)GetValue(MaskCultureProperty); }
			set { SetValue(MaskCultureProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditTextDecorations"),
#endif
 Category("Text")]
		public TextDecorationCollection TextDecorations {
			get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSelectedText"),
#endif
 Browsable(false)]
		public string SelectedText {
			get { return EditBox.SelectedText; }
			set { EditStrategy.SetSelectedText(value); }
		}
		[Browsable(false)]
		public string HighlightedText {
			get { return (string)GetValue(HighlightedTextProperty); }
			internal set { this.SetValue(HighlightedTextPropertyKey, value); }
		}
		[Browsable(false)]
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return (HighlightedTextCriteria)GetValue(HighlightedTextCriteriaProperty); }
			internal set { this.SetValue(HighlightedTextCriteriaPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSelectionLength"),
#endif
 Browsable(false)]
		public int SelectionLength {
			get { return EditBox.SelectionLength; }
			set { EditStrategy.SetSelectionLength(value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditSelectionStart"),
#endif
 Browsable(false)]
		public int SelectionStart {
			get {
				if (EditBox != null)
					return EditBox.SelectionStart;
				return -1;
			}
			set { EditStrategy.SetSelectionStart(value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditCaretIndex"),
#endif
 Browsable(false)]
		public int CaretIndex {
			get {
				if (EditBox != null)
					return EditBox.CaretIndex;
				return -1;
			}
			set { EditStrategy.CaretIndexChanged(value); }
		}
		protected internal virtual MaskType DefaultMaskType { get { return MaskType.None; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditSpinUpCommand")]
#endif
		public ICommand SpinUpCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditSpinDownCommand")]
#endif
		public ICommand SpinDownCommand { get; private set; }
		public event SpinEventHandler Spin {
			add { this.AddHandler(SpinEvent, value); }
			remove { this.RemoveHandler(SpinEvent, value); }
		}
#if !SL
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditCharacterCasing")]
#endif
		public CharacterCasing CharacterCasing {
			get { return (CharacterCasing)base.GetValue(CharacterCasingProperty); }
			set { SetValue(CharacterCasingProperty, value); }
		}
#endif
		public TextEdit() {
			this.SetDefaultStyleKey(typeof(TextEdit));
			SpinUpCommand = DelegateCommandFactory.Create<object>(parameter => SpinUp(), CanSpinUp, false);
			SpinDownCommand = DelegateCommandFactory.Create<object>(parameter => SpinDown(), CanSpinDown, false);
		}
		public int GetCharacterIndexFromLineIndex(int lineIndex) {
			return EditBox.GetCharacterIndexFromLineIndex(lineIndex);
		}
		public int GetCharacterIndexFromPoint(Point point, bool snapToText) {
			return EditBox.GetCharacterIndexFromPoint(point, snapToText);
		}
		public int GetFirstVisibleLineIndex() {
			return EditBox.GetFirstVisibleLineIndex();
		}
		public int GetLastVisibleLineIndex() {
			return EditBox.GetLastVisibleLineIndex();
		}
		public int GetLineIndexFromCharacterIndex(int charIndex) {
			return EditBox.GetLineIndexFromCharacterIndex(charIndex);
		}
		public int GetLineLength(int lineIndex) {
			return EditBox.GetLineLength(lineIndex);
		}
		public string GetLineText(int lineIndex) {
			return EditBox.GetLineText(lineIndex);
		}
		public void Select(int start, int length) {
			EditStrategy.Select(start, length);
		}
		protected override void OnAllowNullInputChanged() {
			base.OnAllowNullInputChanged();
			MaskPropertiesChanged();
		}
		protected override EditStrategyBase CreateEditStrategy() {
			TextInputSettingsBase settings = TextInputSettings;
			return settings.Return(x => x.CreateEditStrategy(), () => CreateEditStrategy(null));
		}
		protected internal EditStrategyBase CreateEditStrategy(object parameter) {
			return new TextEditStrategy(this);
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new TextEditAutomationPeer(this);
		}
		protected virtual object OnCoerceMaskType(MaskType maskType) {
			return EditStrategy.CoerceMaskType(maskType);
		}
		public void SpinUp() {
			EditStrategy.PerformSpinUp();
			EditStrategy.FlushPendingEditActions(UpdateEditorSource.TextInput);
		}
		bool CanSpinUp(object parameter) {
			return EditStrategy.CanSpinUp();
		}
		public void SpinDown() {
			EditStrategy.PerformSpinDown();
			EditStrategy.FlushPendingEditActions(UpdateEditorSource.TextInput);
		}
		bool CanSpinDown(object parameter) {
			return EditStrategy.CanSpinDown();
		}
		protected internal virtual void MaskPropertiesChanged() {
			if (IsInSupportInitializing)
				return;
			UpdateTextInputSettings();
			EditStrategy.SyncWithValue();
			DoValidate();
		}
		protected override TextDecorationCollection GetPrintTextDecorations() {
			if (TextDecorations != null)
				return TextDecorations;
			return base.GetPrintTextDecorations();
		}
		protected internal virtual void MaskTypeChanged(MaskType maskType) {
			PropertyProvider.SetMaskType(maskType);
			if (IsInSupportInitializing)
				return;
			UpdateTextInputSettings();
			EditStrategy.SyncWithValue();
		}
		protected virtual void OnTextDecorationsChanged() {
			SyncTextBlockTextDecorations();
		}
		void SyncTextBlockTextDecorations() {
			SyncTextBlockProperty(TextDecorations, TextBlock.TextDecorationsProperty);
		}
		protected override void OnEditCoreAssigned() {
			SyncTextBlockTextDecorations();
			base.OnEditCoreAssigned();
		}
		protected virtual void HighlightedTextChanged(string text) {
			EditStrategy.HighlightedTextChanged(text);
		}
		protected virtual void HighlightedTextCriteriaChanged(HighlightedTextCriteria criteria) {
			EditStrategy.HighlightedTextCriteriaChanged(criteria);
		}
		protected virtual void OnCharacterCasingChanged(CharacterCasing characterCasing) {
			PropertyProvider.SetCharacterCasing(characterCasing);
			EditStrategy.UpdateDisplayText();
		}
		protected internal override CharacterCasing GetActualCharactedCasing() {
			return PropertyProvider.CharacterCasing;
		}
		protected override EditBoxWrapper CreateEditBoxWrapper() {
			return new TextBoxWrapper(this);
		}
		protected internal virtual MaskType[] GetSupportedMaskTypes() {
			return new[] { MaskType.None, MaskType.DateTime, MaskType.DateTimeAdvancingCaret, MaskType.Numeric, MaskType.RegEx, MaskType.Regular, MaskType.Simple };
		}
		protected internal override TextInputSettingsBase CreateTextInputSettings() {
			return MaskType == MaskType.None 
				? (TextInputSettingsBase)new TextInputSettings(this) 
				: new TextInputMaskSettings(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new TextEditPropertyProvider(this);
		}
	}
}
