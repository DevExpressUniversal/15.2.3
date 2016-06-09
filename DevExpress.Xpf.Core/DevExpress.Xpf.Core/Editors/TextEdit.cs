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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Interop;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Themes;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
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
using TextCompositionEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	public delegate void CustomDisplayTextEventHandler(object sender, CustomDisplayTextEventArgs e);
	public class CustomDisplayTextEventArgs : RoutedEventArgs {
		object editValue;
		string displayText;
		public CustomDisplayTextEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }
		public CustomDisplayTextEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
		public CustomDisplayTextEventArgs() : base() { }
		public object EditValue {
			get { return editValue; }
			set { editValue = value; }
		}
		public string DisplayText {
			get { return displayText; }
			set { displayText = value; }
		}
	}
	public delegate void SpinEventHandler(object sender, SpinEventArgs e);
	public class SpinEventArgs : RoutedEventArgs {
		public SpinEventArgs(bool isSpinUp) {
			RoutedEvent = TextEdit.SpinEvent;
			this.IsSpinUp = isSpinUp;
		}
		public bool IsSpinUp { get; set; }
	}
	public interface IMaskProperties {
		bool MaskAllowNullInput { get; }
		bool MaskSaveLiteral { get; }
		bool MaskShowPlaceHolders { get; }
		char MaskPlaceHolder { get; }
		string Mask { get; }
		MaskType MaskType { get; }
		bool MaskIgnoreBlank { get; }
		bool MaskUseAsDisplayFormat { get; }
		bool MaskBeepOnError { get; }
		AutoCompleteType MaskAutoComplete { get; }
		CultureInfo MaskCulture { get; }
		MaskType[] SupportedMaskTypes { get; }
	}
	[ContentProperty("EditValue")]
	public abstract partial class TextEditBase : BaseEdit, ITextExportSettings {
		#region static
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty EditNonEditableTemplateProperty;
		public static readonly DependencyProperty AcceptsReturnProperty;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty PrintTextWrappingProperty;
		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty;
		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;
		public static readonly DependencyProperty MaxLengthProperty;
		public static readonly DependencyProperty ShowTooltipForTrimmedTextProperty;
		public static readonly DependencyProperty SelectAllOnGotFocusProperty;
		public static readonly DependencyProperty NullTextForegroundProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty TextInputSettingsProperty;
#if !SL
		internal static readonly bool ShowTooltipForTrimmedTextDefaultValue = true;
		public static readonly DependencyProperty AcceptsTabProperty;
		public static readonly DependencyProperty TextTrimmingProperty;
#else
		internal static readonly bool ShowTooltipForTrimmedTextDefaultValue = false;
#endif
		static TextEditBase() {
			Type ownerType = typeof(TextEditBase);
			EditNonEditableTemplateProperty = DependencyPropertyManager.Register("EditNonEditableTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditNonEditableTemplateChanged)));
			AcceptsReturnProperty = TextBox.AcceptsReturnProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(OnAcceptsReturnChanged));
			TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(OnTextWrappingChanged));
			PrintTextWrappingProperty = DependencyPropertyManager.Register("PrintTextWrapping", typeof(TextWrapping?), typeof(TextEditBase), new PropertyMetadata(null, OnTextWrappingChanged));
			VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			MaxLengthProperty = DependencyPropertyManager.Register("MaxLength", typeof(int), ownerType, new FrameworkPropertyMetadata(0));
			ShowTooltipForTrimmedTextProperty = DependencyPropertyManager.Register("ShowTooltipForTrimmedText", typeof(bool), ownerType, new FrameworkPropertyMetadata(ShowTooltipForTrimmedTextDefaultValue, (d, e) => ((TextEditBase)d).OnShowTooltipForTrimmedTextChanged()));
			SelectAllOnGotFocusProperty = DependencyPropertyManager.Register("SelectAllOnGotFocus", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			TextInputSettingsProperty = DependencyPropertyManager.Register("TextInputSettings", typeof(TextInputSettingsBase), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).TextInputSettingsChanged((TextInputSettingsBase)e.OldValue, (TextInputSettingsBase)e.NewValue)));
#if !SL
			NullValueProperty.OverrideMetadata(typeof(PasswordBoxEdit), new FrameworkPropertyMetadata(string.Empty));
			AcceptsTabProperty = TextBox.AcceptsTabProperty.AddOwner(ownerType);
			TextProperty = DependencyPropertyManager.Register("Text", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTextChanged), OnCoerceTextProperty, true, UpdateSourceTrigger.LostFocus));
			TextTrimmingProperty = TextBlock.TextTrimmingProperty.AddOwner(ownerType, new PropertyMetadata(TextTrimming.CharacterEllipsis, (d, e) => ((TextEditBase)d).OnTextTrimmingChanged()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.SelectAll, (d, e) => ((TextEditBase)d).SelectAll(), (d, e) => ((TextEditBase)d).CanSelectAll(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Cut, (d, e) => ((TextEditBase)d).Cut(), (d, e) => ((TextEditBase)d).CanCut(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Delete, (d, e) => ((TextEditBase)d).Delete(), (d, e) => ((TextEditBase)d).CanDelete(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, (d, e) => ((TextEditBase)d).Copy(), (d, e) => ((TextEditBase)d).CanCopy(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, (d, e) => ((TextEditBase)d).Paste(), (d, e) => ((TextEditBase)d).CanPaste(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Undo, (d, e) => ((TextEditBase)d).Undo(), (d, e) => ((TextEditBase)d).CanUndo(d, e)));
			BackgroundProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).OnBackgroundChanged()));
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).OnFocusableChanged()));
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).OnHorizontalContentAlignmentChanged()));
			IsTabStopProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).OnIsTabStopChanged()));
			PaddingProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TextEditBase)d).OnPaddingChanged()));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(OnVerticalContentAlignmentChanged));
			NullTextForegroundProperty = DependencyPropertyManager.Register("NullTextForeground", typeof(Brush), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
#else
			TextProperty = DependencyPropertyManager.Register("Text", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTextChanged), OnCoerceTextProperty));
#endif
		}
		static object OnCoerceTextProperty(DependencyObject d, object text) {
			return ((TextEditBase)d).OnCoerceText((string)text);
		}
		protected static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextEditBase)obj).OnTextChanged((string)e.OldValue, (string)e.NewValue);
		}
		protected static void OnEditNonEditableTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextEditBase)obj).OnEditNonEditableTemplateChanged((ControlTemplate)e.NewValue);
		}
		protected virtual void OnEditNonEditableTemplateChanged(ControlTemplate template) {
			UpdateActualEditorControlTemplate();
		}
		protected static void OnAcceptsReturnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextEditBase)obj).OnAcceptsReturnChanged();
		}
		protected static void OnTextWrappingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextEditBase)obj).OnTextWrappingChanged();
		}
		protected static void OnVerticalContentAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextEditBase)obj).OnVerticalContentAlignmentChanged();
		}
		#endregion
		public TextEditBase() {
			EditBox = CreateEditBoxWrapper();
			selectAllBinding = new CommandBinding(ApplicationCommands.SelectAll, SelectAll);
			cutBinding = new CommandBinding(ApplicationCommands.Cut, Cut, CanCut);
			deleteBinding = new CommandBinding(ApplicationCommands.Delete, Delete, CanDelete);
			pasteBinding = new CommandBinding(ApplicationCommands.Paste, Paste, CanPaste);
			undoBinding = new CommandBinding(ApplicationCommands.Undo, Undo, CanUndo);
		}
		readonly CommandBinding selectAllBinding;
		readonly CommandBinding cutBinding;
		readonly CommandBinding pasteBinding;
		readonly CommandBinding undoBinding;
		readonly CommandBinding deleteBinding;
		void SelectAll(object sender, ExecutedRoutedEventArgs e) {
			SelectAll();
		}
		protected new TextEditStrategy EditStrategy {
			get { return base.EditStrategy as TextEditStrategy; }
			set { base.EditStrategy = value; }
		}
		protected internal EditBoxWrapper EditBox { get; private set; }
		protected virtual object OnCoerceText(string text) {
			return EditStrategy.CoerceText(text);
		}
		protected override void UnsubscribeEditEventsCore() {
			base.UnsubscribeEditEventsCore();
			TextBox editBox = EditCore as TextBox;
			if (editBox != null) {
				editBox.MouseLeftButtonUp -= OnEditBoxMouseLeftButtonUp;
				editBox.TextChanged -= OnEditBoxTextChanged;
				editBox.PreviewMouseLeftButtonDown -= EditBoxPreviewMouseLeftButtonDown;
				editBox.PreviewMouseLeftButtonUp -= EditBoxPreviewMouseLeftButtonUp;
			}
			if (EditBox != null) {
				EditBox.RemovePreviewExecutedHandler(OnCommandExecuted);
				EditBox.RemoveCommandBinding(selectAllBinding);
				EditBox.RemoveCommandBinding(cutBinding);
				EditBox.RemoveCommandBinding(deleteBinding);
				EditBox.RemoveCommandBinding(pasteBinding);
				EditBox.RemoveCommandBinding(undoBinding);
			}
			TextBlockService.RemoveIsTextTrimmedChangedHandler(EditCore, OnIsTextTrimmedChanged);
		}
		protected override void EndInitInternal(bool callBase) {
			base.EndInitInternal(callBase);
			assignSettingsLocker.DoLockedAction(UpdateTextInputSettings);
		}
		protected abstract EditBoxWrapper CreateEditBoxWrapper();
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
			TextBox editBox = EditCore as TextBox;
			if (editBox != null) {
				editBox.MouseLeftButtonUp += OnEditBoxMouseLeftButtonUp;
				editBox.TextChanged += OnEditBoxTextChanged;
				editBox.PreviewMouseLeftButtonDown += EditBoxPreviewMouseLeftButtonDown;
				editBox.PreviewMouseLeftButtonUp += EditBoxPreviewMouseLeftButtonUp;
				editBox.SelectionChanged += EditBoxOnSelectionChanged;
			}
			if (EditBox != null) {
				EditBox.AddPreviewExecutedHandler(OnCommandExecuted);
				EditBox.AddCommandBinding(selectAllBinding);
				EditBox.AddCommandBinding(cutBinding);
				EditBox.AddCommandBinding(deleteBinding);
				EditBox.AddCommandBinding(pasteBinding);
				EditBox.AddCommandBinding(undoBinding);
			}
			TextBlockService.AddIsTextTrimmedChangedHandler(EditCore, OnIsTextTrimmedChanged);
			ReInitializeStrategy();
		}
		void EditBoxOnSelectionChanged(object sender, RoutedEventArgs routedEventArgs) {
		}
		void ReInitializeStrategy() {
			try {
				EditStrategy.Release();
			}
			finally {
				EditStrategy.Initialize();
			}
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new TextEditStrategy(this);
		}
		void EditBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			EditStrategy.PreviewMouseDown(e);
		}
		void EditBoxPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			EditStrategy.PreviewMouseUp(e);
		}
		protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
			base.OnPreviewTextInput(e);
			if (!string.IsNullOrEmpty(e.Text))
				EditStrategy.OnPreviewTextInput(e);
		}
		protected virtual void OnEditBoxTextChanged(object sender, TextChangedEventArgs e) {
			EditStrategy.SyncWithEditor();
		}
		public Brush NullTextForeground {
			get { return (Brush)GetValue(NullTextForegroundProperty); }
			set { SetValue(NullTextForegroundProperty, value); }
		}
		[ Browsable(false)]
		public ControlTemplate EditNonEditableTemplate {
			get { return (ControlTemplate)GetValue(EditNonEditableTemplateProperty); }
			set { SetValue(EditNonEditableTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseText"),
#endif
 Category("CommonProperties")]
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseTextWrapping"),
#endif
 Category("Text ")]
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBasePrintTextWrapping")]
#endif
		public TextWrapping? PrintTextWrapping {
			get { return (TextWrapping?)GetValue(PrintTextWrappingProperty); }
			set { SetValue(PrintTextWrappingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBaseHorizontalScrollBarVisibility")]
#endif
		public ScrollBarVisibility HorizontalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
			set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBaseVerticalScrollBarVisibility")]
#endif
		public ScrollBarVisibility VerticalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
			set { SetValue(VerticalScrollBarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBaseMaxLength")]
#endif
		public int MaxLength {
			get { return (int)GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseSelectAllOnGotFocus"),
#endif
 Category("Behavior")]
		public bool SelectAllOnGotFocus {
			get { return (bool)GetValue(SelectAllOnGotFocusProperty); }
			set { SetValue(SelectAllOnGotFocusProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseAcceptsTab"),
#endif
 Category("Text ")]
		public bool AcceptsTab {
			get { return (bool)GetValue(AcceptsTabProperty); }
			set { SetValue(AcceptsTabProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseTextTrimming"),
#endif
 Category("Text ")]
		public TextTrimming TextTrimming {
			get { return (TextTrimming)GetValue(TextTrimmingProperty); }
			set { SetValue(TextTrimmingProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBaseShowTooltipForTrimmedText")]
#endif
		public bool ShowTooltipForTrimmedText {
			get { return (bool)GetValue(ShowTooltipForTrimmedTextProperty); }
			set { SetValue(ShowTooltipForTrimmedTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("TextEditBaseAcceptsReturn"),
#endif
 Category("Text ")]
		public bool AcceptsReturn {
			get { return (bool)GetValue(AcceptsReturnProperty); }
			set { SetValue(AcceptsReturnProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TextEditBaseLineCount")]
#endif
		public int LineCount { get { return EditBox.LineCount; } }
		public TextInputSettingsBase TextInputSettings {
			get { return (TextInputSettingsBase)GetValue(TextInputSettingsProperty); }
			protected set { SetValue(TextInputSettingsProperty, value); }
		}
		new TextEditPropertyProvider PropertyProvider { get { return (TextEditPropertyProvider)base.PropertyProvider; } }
		readonly Locker assignSettingsLocker = new Locker();
		public void Clear() {
			EditStrategy.Clear();
		}
		protected virtual void OnTextChanged(string oldText, string text) {
			EditStrategy.OnTextChanged(oldText, text);
		}
		protected virtual void OnAcceptsReturnChanged() { }
		protected internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (EditBox == null)
				return base.NeedsKey(key, modifiers);
			if (EditStrategy.NeedsKey(key, modifiers))
				return base.NeedsKey(key, modifiers);
			return false;
		}
		protected override bool NeedsEnter(ModifierKeys modifiers) {
			return EditStrategy.NeedsEnter;
		}
		internal override void FlushPendingEditActions(UpdateEditorSource updateEditor) {
			EditStrategy.FlushPendingEditActions(updateEditor);
		}
		public override void SelectAll() {
			base.SelectAll();
			EditStrategy.SelectAll();
		}
		#region commands handlers
		public virtual void Delete() {
			EditStrategy.Delete();
		}
		protected override void OnEditCoreAssigned() {
			if (IsPrintingMode && EditCore is TextBlock)
				((TextBlock)EditCore).Margin = new Thickness(0);
			SyncTextBlockPadding();
			SyncTextBlockTextWrapping();
			SyncTextBlockTextAlignment();
			SyncTextBlockProperty(VerticalContentAlignmentProperty, TextBlock.VerticalAlignmentProperty);
			UpdateAllowIsTextTrimmed();
#if !SL
			SyncTextBlockProperty(BackgroundProperty, TextBlock.BackgroundProperty);
			SyncTextBlockProperty(TextTrimmingProperty);
			SyncTextBlockProperty(IsTabStopProperty, KeyboardNavigation.IsTabStopProperty);
			SyncTextBlockProperty(FocusableProperty);
			SyncTextBlockProperty((object)null, FrameworkElement.FocusVisualStyleProperty);
			SyncTextBlockForegroundPropertyForNullText();
#endif
			base.OnEditCoreAssigned();
		}
		void SyncTextBlockPadding() {
			SyncTextBlockProperty(PaddingProperty, TextBlock.PaddingProperty);
		}
		void SyncTextBlockProperty(DependencyProperty editorProperty, DependencyProperty textBlockProperty) {
			SyncTextBlockProperty(GetValue(editorProperty), textBlockProperty);
		}
		protected void SyncTextBlockProperty(object newValue, DependencyProperty textBlockProperty) {
			if (IsInactiveModeWithTextBlock()) {
				ValueSource valueSource = System.Windows.DependencyPropertyHelper.GetValueSource(EditCore, textBlockProperty);
				if (valueSource.BaseValueSource <= BaseValueSource.DefaultStyleTrigger || valueSource.BaseValueSource == BaseValueSource.Local)
					EditCore.SetValue(textBlockProperty, newValue);
			}
		}
		void SyncTextBlockProperty(DependencyProperty property) {
			SyncTextBlockProperty(property, property);
		}
		internal bool IsInactiveModeWithTextBlock() {
			return IsInplaceMode && (EditCore is TextBlock);
		}
		protected virtual bool IsTextBlockModeCore() {
			return EditMode == EditMode.InplaceInactive;
		}
		protected virtual void OnTextWrappingChanged() {
			EditStrategy.UpdateDisplayText();
			SyncTextBlockTextWrapping();
		}
		void SyncTextBlockTextWrapping() {
			SyncTextBlockProperty(GetActualTextWrapping(), TextBlock.TextWrappingProperty);
		}
		internal TextWrapping GetActualTextWrapping() {
			if (IsPrintingMode && PrintTextWrapping.HasValue)
				return PrintTextWrapping.Value;
			return TextWrapping;
		}
		void SyncTextBlockTextAlignment() {
			TextAlignment alignment;
			switch (HorizontalContentAlignment) {
				case HorizontalAlignment.Center:
					alignment = TextAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					alignment = TextAlignment.Right;
					break;
#if !SL
				case HorizontalAlignment.Stretch:
					alignment = TextAlignment.Justify;
					break;
#endif
				default:
					alignment = TextAlignment.Left;
					break;
			}
			SyncTextBlockProperty(alignment, TextBlock.TextAlignmentProperty);
		}
		protected virtual void OnHorizontalContentAlignmentChanged() {
			SyncTextBlockTextAlignment();
		}
#if !SL
		protected virtual void OnEditBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if (!e.Handled && EditCore is TextBox && Mouse.Captured == EditCore)
				EditCore.ReleaseMouseCapture();
		}
		internal bool IsBrowserPasteCommandExecuted { get; set; }
		void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (e.Command == ApplicationCommands.Paste)
				IsBrowserPasteCommandExecuted = BrowserInteropHelper.IsBrowserHosted;
		}
		void ProcessBrowserPasteCommand(TextChangedEventArgs e) {
			List<TextChange> list = new List<TextChange>(e.Changes);
			if (list.Count > 0) {
				TextChange change = list[0];
				string pastedText = EditBox.Text.Substring(change.Offset, change.AddedLength);
				EditStrategy.Paste(pastedText);
			}
			IsBrowserPasteCommandExecuted = false;
		}
		public virtual void Copy() {
			EditStrategy.Copy();
		}
		public virtual void Cut() {
			EditStrategy.Cut();
		}
		public virtual void Paste() {
			EditStrategy.Paste();
		}
		public virtual void Undo() {
			EditStrategy.Undo();
		}
		protected internal virtual void CanSelectAll(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanSelectAll());
		}
		protected internal virtual void CanCut(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanCut());
		}
		protected internal virtual void CanCopy(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanCopy());
		}
		protected internal virtual void CanDelete(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanDelete());
		}
		protected internal virtual void CanPaste(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanPaste());
		}
		protected internal virtual void CanUndo(object sender, CanExecuteRoutedEventArgs e) {
			SetCanExecuteParameters(e, EditStrategy.CanUndo());
		}
		protected virtual void SetCanExecuteParameters(CanExecuteRoutedEventArgs e, bool canExecute) {
			e.CanExecute = EditMode != EditMode.InplaceInactive && canExecute;
			if (EditMode != EditMode.InplaceInactive) {
				e.ContinueRouting = !canExecute;
				e.Handled = !canExecute;
			}
		}
		protected internal virtual void Copy(object sender, ExecutedRoutedEventArgs e) {
			Copy();
		}
		protected internal virtual void Cut(object sender, ExecutedRoutedEventArgs e) {
			Cut();
		}
		protected internal virtual void Delete(object sender, ExecutedRoutedEventArgs e) {
			Delete();
		}
		protected internal virtual void Paste(object sender, ExecutedRoutedEventArgs e) {
			if (BrowserInteropHelper.IsBrowserHosted)
				return;
			Paste();
		}
		protected internal virtual void Undo(object sender, ExecutedRoutedEventArgs e) {
			Undo();
		}
#endif
		#endregion
		void OnIsTextTrimmedChanged(object sender, System.Windows.RoutedEventArgs e) {
			EditStrategy.CoerceToolTip();
		}
		void OnShowTooltipForTrimmedTextChanged() {
			UpdateAllowIsTextTrimmed();
		}
		protected override void OnEditModeChanged(EditMode oldValue, EditMode newValue) {
			base.OnEditModeChanged(oldValue, newValue);
			UpdateAllowIsTextTrimmed();
		}
		void UpdateAllowIsTextTrimmed() {
			if (IsInactiveModeWithTextBlock())
				TextBlockService.SetAllowIsTextTrimmed(EditCore, EditMode == EditMode.InplaceInactive && ShowTooltipForTrimmedText);
			else
				EditStrategy.CoerceToolTip();
		}
		protected virtual void UpdateTextInputSettings() {
			var settings = CreateTextInputSettings();
			if (TextInputSettings == null || TextInputSettings.GetType() != settings.GetType())
				TextInputSettings = settings;
			TextInputSettings.AssignProperties();
		}
		protected internal virtual TextInputSettingsBase CreateTextInputSettings() {
			return new TextInputSettings(this);
		}
#if !SL
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new TextEditAutomationPeer(this);
		}
		void OnFocusableChanged() {
			SyncTextBlockProperty(FocusableProperty);
		}
		void OnIsTabStopChanged() {
			SyncTextBlockProperty(IsTabStopProperty, KeyboardNavigation.IsTabStopProperty);
		}
		void OnBackgroundChanged() {
			SyncTextBlockProperty(BackgroundProperty, TextBlock.BackgroundProperty);
		}
		void OnTextTrimmingChanged() {
			SyncTextBlockProperty(TextTrimmingProperty);
		}
		void OnPaddingChanged() {
			SyncTextBlockPadding();
		}
		protected virtual void OnVerticalContentAlignmentChanged() {
			SyncTextBlockProperty(VerticalContentAlignmentProperty, TextBlock.VerticalAlignmentProperty);
		}
		protected override void OnIsNullTextVisibleChanged(bool isVisible) {
			base.OnIsNullTextVisibleChanged(isVisible);
			SyncTextBlockForegroundPropertyForNullText();
		}
		void SyncTextBlockForegroundPropertyForNullText() {
			if (!IsInactiveModeWithTextBlock())
				return;
			if (PropertyProvider.IsNullTextVisible) {
				object resourceKey = new TextEditThemeKeyExtension() { ResourceKey = TextEditThemeKeys.NullTextForeground, ThemeName = ThemeHelper.GetEditorThemeName(this) };
				Brush nullTextForeground = TryFindResource(resourceKey) as Brush;
				if (nullTextForeground != null) {
					SyncTextBlockProperty(nullTextForeground, TextBlock.ForegroundProperty);
					return;
				}
			}
			EditCore.ClearValue(TextBlock.ForegroundProperty);
		}
#endif
		protected virtual void TextInputSettingsChanged(TextInputSettingsBase oldValue, TextInputSettingsBase newValue) {
			PropertyProvider.SetTextInputSettings(newValue);
			if (IsInSupportInitializing)
				return;
			EditStrategy.SyncWithValue();
			assignSettingsLocker.DoIfNotLocked(new Action(() => DoValidate()));
		}
		internal void PerformKeyboardSelectAll() {
			if (!IsMousePressed) {
				SelectAll();
			}
		}
#if !SL
		protected override void ProcessActivatingKeyCore(Key key, ModifierKeys modifiers) {
			if (Settings.IsPasteGesture(key, modifiers)) {
				EditBox.ExecuteCommand(ApplicationCommands.Paste, null);
				return;
			}
			base.ProcessActivatingKeyCore(key, modifiers);
		}
		void ScrollToHome() {
			if (EditBox != null)
				EditBox.ScrollToHome();
		}
		bool IsMousePressed { get { return Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed; } }
		protected internal virtual CharacterCasing GetActualCharactedCasing() {
			return CharacterCasing.Normal;
		}
#endif
		internal bool CanSelectAllOnGotFocus {
			get {
				bool result = SelectAllOnGotFocus && EditMode == EditMode.Standalone;
#if SL
				result = result && !((EditCore is TextBox) && (EditCore as TextBox).IsClickedByMouseLeftButton);
#endif
				return result;
			}
		}
		protected virtual void UpdateEditBoxWrapper(){
			EditBox = CreateEditBoxWrapper();
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			if (EditMode != EditMode.Standalone)
				return new PointHitTestResult(this, hitTestParameters.HitPoint);
			return base.HitTestCore(hitTestParameters);
		}
		#region ITextExportSettings Members
		HorizontalAlignment ITextExportSettings.HorizontalAlignment {
			get {
				if(HorizontalContentAlignment == System.Windows.HorizontalAlignment.Stretch)
					return ExportSettingDefaultValue.HorizontalAlignment;
				return HorizontalContentAlignment;
			}
		}
		VerticalAlignment ITextExportSettings.VerticalAlignment {
			get {
				if(VerticalContentAlignment == System.Windows.VerticalAlignment.Stretch)
					return ExportSettingDefaultValue.VerticalAlignment;
				return VerticalContentAlignment;
			}
		}
		string ITextExportSettings.Text {
			get { return GetExportText(); }
		}
		protected virtual string GetExportText() {
			return PropertyProvider.DisplayText;
		}
		object ITextExportSettings.TextValue {
			get { return GetTextValueInternal(); }
		}
		protected virtual object GetTextValueInternal() {
			return EditStrategy.IsNullValue(EditValue) ? null : EditValue;
		}
		string ITextExportSettings.TextValueFormatString {
			get { return DisplayFormatString; }
		}
		FontFamily ITextExportSettings.FontFamily {
			get { return FontFamily; }
		}
		FontStyle ITextExportSettings.FontStyle {
			get { return FontStyle; }
		}
		FontWeight ITextExportSettings.FontWeight {
			get { return FontWeight; }
		}
		double ITextExportSettings.FontSize {
			get { return FontSize; }
		}
		Thickness ITextExportSettings.Padding {
			get {
				return Padding; 
			}
		}
		TextWrapping ITextExportSettings.TextWrapping {
			get { return GetActualTextWrapping(); }
		}
		bool ITextExportSettings.NoTextExport {
			get { return ExportSettingDefaultValue.NoTextExport; }
		}
		bool? ITextExportSettings.XlsExportNativeFormat { get { return GetXlsExportNativeFormatInternal(); } }
		protected virtual bool? GetXlsExportNativeFormatInternal() {
			return ExportSettingDefaultValue.XlsExportNativeFormat;
		}
		string ITextExportSettings.XlsxFormatString {
			get {
				return ExportSettingDefaultValue.XlsxFormatString;
			}
		}
		TextDecorationCollection ITextExportSettings.TextDecorations {
			get {
				return GetPrintTextDecorations();
			}
		}
		protected virtual TextDecorationCollection GetPrintTextDecorations() {
			return ExportSettingDefaultValue.TextDecorations;
		}
		TextTrimming ITextExportSettings.TextTrimming {
			get {
				return TextTrimming;
			}
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.Helpers {
	public static class TextBoxHelper {
		public static bool NeedKey(TextBox textBox, Key key) {
			if (textBox == null)
				return true;
			switch (key) {
				case Key.Left:
				case Key.Home:
					return (textBox.SelectionStart != 0 || textBox.SelectionLength != 0) && !textBox.IsReadOnly;
				case Key.Right:
				case Key.End:
					return (textBox.SelectionStart != textBox.Text.Length) && !textBox.IsReadOnly;
				case Key.Up:
				case Key.PageUp:
					return (textBox.GetLineIndexFromCharacterIndex(textBox.SelectionStart) != 0) && !textBox.IsReadOnly;
				case Key.Down:
				case Key.PageDown:
					return (textBox.GetLineIndexFromCharacterIndex(textBox.SelectionStart) != textBox.LineCount - 1) && !textBox.IsReadOnly;
			}
			return true;
		}
	}
}
namespace DevExpress.Xpf.Editors.Internal {
	public class TextBoxPaddingConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Thickness v = (Thickness)value;
#if SL
			v.Right--;
#endif
			return v;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
