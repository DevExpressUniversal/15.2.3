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
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Validation;
namespace DevExpress.Xpf.Editors {
	public partial class InplaceBaseEdit : Chrome, IInplaceBaseEdit {
		const string BorderName = "PART_Border";
		const string EditorName = "PART_Editor";
		const string ErrorProviderName = "PART_ErrorProvider";
		const string LeftButtonsName = "PART_LeftButtons";
		const string RightButtonsName = "PART_RightButtons";
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty EditCoreStyleProperty;
		static InplaceBaseEdit() {
			Type ownerType = typeof(InplaceBaseEdit);
			EditValueProperty = DependencyProperty.Register("EditValue", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
					(o, args) => ((InplaceBaseEdit)o).OnEditValueChanged(args.OldValue, args.NewValue), null, false, UpdateSourceTrigger.PropertyChanged)
				{ BindsTwoWayByDefault = true });
			EditCoreStyleProperty = DependencyProperty.Register("EditCoreStyle", typeof(Style), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((InplaceBaseEdit)o).EditCoreStyleChanged((Style)args.NewValue)));
			ToolTipService.InitialShowDelayProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(500));
			ClipToBoundsProperty.OverrideMetadata(ownerType, new PropertyMetadata(true));
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			NumberSubstitution.SubstitutionProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(NumberSubstitutionMethod.European));
			RecognizesAccessKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
		}
		bool initialized;
		bool showBorder;
		HorizontalAlignment hca;
		VerticalAlignment vca;
		bool allowDefaultButton = true;
		bool showEditorButtons;
		BaseValidationError validationError = null;
		DataTemplate validationErrorTemplate = null;
		BaseEditSettings editSettings;
		EditMode editMode = EditMode.InplaceInactive;
		EditSettingsChangedEventHandler<InplaceBaseEdit> EditSettingsChangedEventHandler { get; set; }
		ItemsProviderChangedEventHandler<InplaceBaseEdit> ItemsProviderChangedEventHandler { get; set; }
		readonly Locker supportInitializeLocker = new Locker();
		TextTrimming textTrimming = TextTrimming.CharacterEllipsis;
		TextWrapping textWrapping = TextWrapping.NoWrap;
		RenderTextBlockContext textBlock;
		RenderCheckBoxContext checkBox;
		RenderBaseEditContext activeEditor;
		RenderEditorControlContext editorControl;
		RenderItemsControlContext leftButtonsItemsControl;
		RenderItemsControlContext rightButtonsItemsControl;
		RenderControlContext errorProvider;
		RenderControlContext borderControl;
		IDisplayTextProvider displayTextProvider;
		string displayText;
		bool isNullTextVisible;
		bool isReadOnly;
		bool hasTextDecorations;
		ControlTemplate displayTemplate;
		ControlTemplate editTemplate;
		RenderTemplate borderTemplate;
		object dataContext;
		HighlightedTextCriteria highlightedTextCriteria;
		string highlightedText;
		TextDecorationCollection textDecorations;
		Style editCoreStyle;
		string nullText;
		object nullValue;
		InvalidValueBehavior invalidValueBehavior;
		IInplaceEditingProvider provider;
		public BaseEditSettings Settings { get { return editSettings ?? CreateDefaultEditSettings(); } }
		public object EditValueCache { get; private set; }
		public Style EditCoreStyle {
			get { return (Style)GetValue(EditCoreStyleProperty); }
			set { SetValue(EditCoreStyleProperty, value); }
		}
		public string DisplayText {
			get {
				return CalcDisplayText(EditValueCache);
			}
		}
		public bool HasTextDecorations {
			get { return hasTextDecorations; }
			set {
				if (hasTextDecorations == value)
					return;
				hasTextDecorations = value;
				AssignTextBlockValues();
			}
		}
		bool IsNullValue(object value) {
			return BaseEditSettings.IsNativeNullValue(value) || object.Equals(value, nullValue);
		}
		bool IsInSupportInitialize { get { return supportInitializeLocker; } }
		public EditMode EditMode {
			get { return editMode; }
			set {
				if (editMode == value)
					return;
				editMode = value;
				OnEditModeChanged();
			}
		}
		protected virtual void EditCoreStyleChanged(Style newValue) {
			editCoreStyle = newValue;
		}
		void OnEditModeChanged() {
			RenderContent = CreateEditorContent();
			EditValue = EditValueCache;
		}
		public void SetSettings(BaseEditSettings settings) {
			UnsubscribeFromSettings(this.editSettings);
			this.editSettings = settings;
			UpdateEditorContent();
			SubscribeToSettings(settings);
			AfterSetSettings();
		}
		protected virtual void AfterSetSettings() {
			ResetDisplayText();
			ProcessInnerContext();
		}
		void SubscribeToSettings(BaseEditSettings s) {
			if (s != null)
				s.EditSettingsChanged += EditSettingsChangedEventHandler.Handler;
			LookUpEditSettingsBase lookup = s as LookUpEditSettingsBase;
			if (lookup != null)
				lookup.ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
		void UnsubscribeFromSettings(BaseEditSettings s) {
			if (s != null)
				s.EditSettingsChanged -= EditSettingsChangedEventHandler.Handler;
			LookUpEditSettingsBase lookup = s as LookUpEditSettingsBase;
			if (lookup != null)
				lookup.ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
		}
		string CalcDisplayText(object value) {
			hasDisplayTextProviderText = false;
			if (isNullTextVisible && displayTextProvider == null)
				return nullText;
			if (displayText == null)
				displayText = Settings.GetDisplayTextFromEditor(value);
			if (displayTextProvider != null) {
				string displayTextProviderText;
				hasDisplayTextProviderText = displayTextProvider.GetDisplayText(displayText, value, out displayTextProviderText);
				if (hasDisplayTextProviderText.GetValueOrDefault(true))
					return displayTextProviderText;
			}
			return displayText;
		}
		#region IBaseEdit
		bool IBaseEdit.ShouldDisableExcessiveUpdatesInInplaceInactiveMode {
			get { return false; }
			set { }
		}
		bool? IBaseEdit.DisableExcessiveUpdatesInInplaceInactiveMode {
			get { return null; }
			set { }
		}
		bool IBaseEdit.IsReadOnly {
			get { return isReadOnly; }
			set {
				if (isReadOnly == value)
					return;
				isReadOnly = value;
				if (activeEditor != null)
					activeEditor.IsReadOnly = value;
			}
		}
		object IBaseEdit.DataContext {
			get { return dataContext; }
			set {
				if (dataContext == value)
					return;
				dataContext = value;
				if (editorControl != null) {
					editorControl.DataContext = value;
					editorControl.RealDataContext = value;
				}
				if (activeEditor != null)
					activeEditor.RealDataContext = value;
			}
		}
		bool IBaseEdit.IsEditorActive { get { return activeEditor.Return(x => x.IsEditorActive, null); } }
		IValueConverter IBaseEdit.DisplayTextConverter { get; set; }
		InvalidValueBehavior IBaseEdit.InvalidValueBehavior {
			get { return invalidValueBehavior; }
			set {
				if (invalidValueBehavior == value)
					return;
				invalidValueBehavior = value;
				if (activeEditor != nullValue)
					activeEditor.InvalidValueBehavior = value;
			}
		}
		string IBaseEdit.DisplayFormatString { get; set; }
		HorizontalAlignment IBaseEdit.HorizontalContentAlignment {
			get { return hca; }
			set {
				if (hca == value)
					return;
				hca = value;
				if (activeEditor != null)
					activeEditor.HorizontalContentAlignment = value;
				if (textBlock != null)
					textBlock.TextAlignment = value.ToTextAlignment();
			}
		}
		VerticalAlignment IBaseEdit.VerticalContentAlignment {
			get { return vca; }
			set {
				if (vca == value)
					return;
				vca = value;
				if (activeEditor != null)
					activeEditor.VerticalContentAlignment = value;
				if (textBlock != null)
					textBlock.VerticalAlignment = value;
			}
		}
		bool IBaseEdit.ShowEditorButtons {
			get { return showEditorButtons; }
			set {
				if (showEditorButtons == value)
					return;
				showEditorButtons = value;
				UpdateEditorContent();
			}
		}
		bool IInplaceBaseEdit.ShowBorder {
			get { return showBorder; }
			set {
				if (showBorder == value)
					return;
				showBorder = value;
				UpdateEditorContent();
			}
		}
		bool? hasDisplayTextProviderText;
		bool IInplaceBaseEdit.IsNullTextVisible { get { return isNullTextVisible && !hasDisplayTextProviderText.GetValueOrDefault(false); } }
		bool IInplaceBaseEdit.ShowToolTipForTrimmedText { get; set; }
		public event EditValueChangedEventHandler EditValueChanged;
		public event RoutedEventHandler EditorActivated;
		public ControlTemplate EditTemplate {
			get { return editTemplate; }
			set {
				if (editTemplate == value)
					return;
				editTemplate = value;
				if (activeEditor != null) {
					activeEditor.RealDataContext = dataContext ?? DataContext;
					activeEditor.EditTemplate = value;
				}
			}
		}
		ControlTemplate IBaseEdit.DisplayTemplate {
			get { return displayTemplate; }
			set {
				if (displayTemplate == value)
					return;
				displayTemplate = value;
				UpdateEditorContent();
				if (editorControl != null)
					editorControl.Template = value ?? CalcDisplayTemplate();
			}
		}
		bool IBaseEdit.CanAcceptFocus { get; set; }
		bool IBaseEdit.HasValidationError { get { return validationError != null; } }
		BaseValidationError IBaseEdit.ValidationError {
			get { return validationError; }
			set {
				if (validationError == value)
					return;
				validationError = value;
				OnValidationErrorChanged(value);
				AssignActiveEditorValues();
			}
		}
		DataTemplate IBaseEdit.ValidationErrorTemplate {
			get { return validationErrorTemplate; }
			set {
				if (validationErrorTemplate == value)
					return;
				validationErrorTemplate = value;
			}
		}
		void OnValidationErrorChanged(BaseValidationError error) {
			if (EditMode == EditMode.InplaceInactive)
				UpdateEditorContent();
			else
				activeEditor.Do(x => x.ValidationError = error);
		}
		public void ForceInitialize(bool callBase) {
		}
		bool IBaseEdit.IsPrintingMode { get; set; }
		bool isValueChanged = false;
		bool IBaseEdit.IsValueChanged {
			get { return activeEditor.Return(x => x.IsValueChanged, () => isValueChanged); }
			set {
				isValueChanged = value;
				SetEditIsValueChanged(value);
			}
		}
		void SetEditIsValueChanged(bool value) {
			activeEditor.Do(x => x.IsValueChanged = value);
		}
		public bool DoValidate() {
			return activeEditor.Return(x => x.DoValidate(), () => true);
		}
		public void SelectAll() {
			activeEditor.Do(x => x.SelectAll());
		}
		public bool GetShowEditorButtons() {
			return ((IBaseEdit)this).ShowEditorButtons;
		}
		public void SetShowEditorButtons(bool show) {
			((IBaseEdit)this).ShowEditorButtons = show;
		}
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			return activeEditor.Return(x => x.NeedsKey(key, modifiers), () => BaseEdit.NeedsBasicKey(key, () => false) ?? true);
		}
		public virtual bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			GetIsActivatingKeyEventArgs e = new GetIsActivatingKeyEventArgs(key, modifiers, this, IsActivatingKeyCore(key, modifiers));
			Settings.RaiseGetIsActivatingKey(this, e);
			return e.IsActivatingKey;
		}
		bool IsActivatingKeyCore(Key key, ModifierKeys modifiers) {
			return !((IBaseEdit)this).IsReadOnly && IsEnabled && Settings.IsActivatingKey(key, modifiers);
		}
		public void ProcessActivatingKey(Key key, ModifierKeys modifiers) {
			activeEditor.Do(x => x.ProcessActivatingKey(key, modifiers));
		}
		public void FlushPendingEditActions() {
			activeEditor.Do(x => x.FlushPendingEditActions());
		}
		public string GetDisplayText(object editValue, bool applyFormatting) {
			return DisplayText;
		}
		internal void SetDisplayTextProvider(IDisplayTextProvider displayTextProvider) {
			this.displayTextProvider = displayTextProvider;
			ResetDisplayText();
		}
		void ResetDisplayText() {
			displayText = null;
			isNullTextVisible = IsNullValue(EditValueCache);
		}
		public FrameworkElement EditCore { get { return null; } }
		string IBaseEdit.NullText {
			get { return nullText; }
			set {
				if (nullText == value)
					return;
				nullText = value;
				ResetDisplayText();
				AssignTextBlockValues();
			}
		}
		object IBaseEdit.NullValue {
			get { return nullValue; }
			set {
				if (nullValue == value)
					return;
				nullValue = value;
				ResetDisplayText();
				AssignTextBlockValues();
			}
		}
		public void ClearError() {
		}
		#endregion
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public InplaceBaseEdit()
			: this(false) {
			Content = null;
			ContentTemplate = null;
			ContentTemplateSelector = null;
		}
		protected internal InplaceBaseEdit(bool fake) {
			EditSettingsChangedEventHandler = new EditSettingsChangedEventHandler<InplaceBaseEdit>(this, (owner, o, e) => owner.OnEditSettingsChanged());
			ItemsProviderChangedEventHandler = new ItemsProviderChangedEventHandler<InplaceBaseEdit>(this, (owner, o, e) => owner.ItemsProviderChanged(e));
			FocusVisualStyle = null;
			((IBaseEdit)this).CanAcceptFocus = true;
			if (fake)
				this.SetBypassLayoutPolicies(true);
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			UpdateEditorContent();
			if (EditMode != EditMode.InplaceInactive)
				EditValue = EditValueCache;
		}
		void UpdateEditorContent() {
			RenderContent = CreateEditorContent();
		}
		EditorContent CreateEditorContent() {
			return new EditorContent()
			{
				EditMode = EditMode,
				Error = validationError,
				Settings = Settings,
				DisplayTemplate = displayTemplate ?? CalcDisplayTemplate(),
				ShowBorder = showBorder,
				ShowEditorButtons = showEditorButtons,
			};
		}
		void OnEditSettingsChanged() {
			Settings.AssignToEdit(this);
			ResetDisplayText();
			AssignValues();
		}
		void ItemsProviderChanged(ItemsProviderChangedEventArgs e) {
			if (EditMode == Editors.EditMode.InplaceActive) return;
			ResetDisplayText();
			AssignValues();
		}
		protected virtual void OnEditValueChanged(object oldValue, object newValue) {
			EditValueCache = newValue;
			ResetDisplayText();
			AssignValues();
			RaiseEditValueChanged(oldValue, newValue);
		}
		void AssignCheckBoxValues() {
			if (IsInSupportInitialize)
				return;
			IInplaceBaseEdit baseEdit = this;
			if (checkBox != null) {
				checkBox.BeginInit();
				checkBox.IsChecked = CheckEditSettings.GetBooleanFromEditValue(EditValueCache, ((CheckEditSettings)Settings).IsThreeState);
				checkBox.HorizontalContentAlignment = baseEdit.HorizontalContentAlignment;
				checkBox.EndInit();
			}
		}
		void AssignActiveEditorValues() {
			if (IsInSupportInitialize)
				return;
			IInplaceBaseEdit baseEdit = this;
			if (activeEditor != null) {
				activeEditor.EditMode = EditMode;
				activeEditor.EditValue = EditValueCache;
				activeEditor.HorizontalContentAlignment = baseEdit.HorizontalContentAlignment;
				activeEditor.VerticalContentAlignment = baseEdit.VerticalContentAlignment;
				activeEditor.ValidationError = validationError;
				activeEditor.InvalidValueBehavior = invalidValueBehavior;
				activeEditor.IsReadOnly = isReadOnly;
				activeEditor.RealDataContext = dataContext ?? DataContext;
				if (editTemplate != null) 
					activeEditor.EditTemplate = editTemplate;
				if (displayTemplate != null)
					activeEditor.DisplayTemplate = displayTemplate;
				if (editCoreStyle != null)
					activeEditor.Style = editCoreStyle;
			}
		}
		void AssignTextBlockValues() {
			if (IsInSupportInitialize)
				return;
			IInplaceBaseEdit baseEdit = this;
			if (textBlock != null) {
				textBlock.BeginInit();
				textBlock.Text = DisplayText;
				textBlock.HighlightedText = baseEdit.HighlightedText;
				textBlock.TextTrimming = textTrimming;
				textBlock.TextWrapping = textWrapping;
				textBlock.TextAlignment = baseEdit.HorizontalContentAlignment.ToTextAlignment();
				textBlock.VerticalAlignment = baseEdit.VerticalContentAlignment;
				textBlock.HighlightedTextCriteria = baseEdit.HighlightedTextCriteria;
				textBlock.ForceUseRealTextBlock = baseEdit.HasTextDecorations;
				textBlock.TextDecorations = baseEdit.TextDecorations;
				textBlock.Opacity = baseEdit.IsNullTextVisible ? 0.35 : 1d;
				textBlock.EndInit();
			}
		}
		protected virtual BaseEditSettings CreateDefaultEditSettings() {
			var settings = CreateDefaultEditSettingsInternal();
			settings.ApplyToEdit(this, true);
			return settings;
		}
		protected virtual BaseEditSettings CreateDefaultEditSettingsInternal() {
			return new TextEditSettings();
		}
		protected override FrameworkRenderElementContext InitializeContext() {
			try {
				return base.InitializeContext();
			} finally {
				ProcessInnerContext();
			}
		}
		protected override void RenderContentChanged(object oldContent, object newContent) {
		}
		void ProcessInnerContext() {
			ProcessTextBlock();
			ProcessEditorControl();
			ProcessBorder();
			ProcessActiveEditor();
			ProcessInplaceButtonEdit();
			ProcessInplaceCheckBox();
			ProcessValidationProvider();
		}
		void AssignValues() {
			AssignCheckBoxValues();
			AssignTextBlockValues();
			AssignBorderValues();
			AssignEditorControlValues();
			AssignActiveEditorValues();
			AssignErrorProviderValues();
		}
		void ProcessBorder() {
			borderControl = Namescope.GetElement(BorderName) as RenderControlContext;
			AssignBorderValues();
		}
		void AssignBorderValues() {
			if (IsInSupportInitialize)
				return;
			if (borderControl != null) {
				borderControl.BeginInit();
				borderControl.Visibility = showBorder && borderTemplate != null ? Visibility.Visible : Visibility.Collapsed;
				borderControl.RenderTemplate = borderTemplate;
				borderControl.EndInit();
			}
		}
		void ProcessEditorControl() {
			editorControl = Namescope.GetElement(EditorName) as RenderEditorControlContext;
			AssignEditorControlValues();
		}
		void AssignEditorControlValues() {
			if (IsInSupportInitialize)
				return;
			IInplaceBaseEdit baseEdit = this;
			if (editorControl != null) {
				editorControl.Template = displayTemplate ?? CalcDisplayTemplate();
				editorControl.DataContext = dataContext ?? this;
				editorControl.RealDataContext = dataContext ?? this;
				editorControl.EditValue = CalcEditorControlValue();
				editorControl.DisplayText = DisplayText;
				editorControl.HighlightedText = baseEdit.HighlightedText;
				editorControl.HighlightedTextCriteria = baseEdit.HighlightedTextCriteria;
				editorControl.IsChecked = CalcIsChecked();
				editorControl.SelectedItem = CalcSelectedItem();
				editorControl.SelectedIndex = CalcSelectedIndex();
				editorControl.ItemTemplate = CalcItemTemplate();
				editorControl.ItemTemplateSelector = CalcItemTemplateSelector();
				editorControl.HorizontalContentAlignment = baseEdit.HorizontalContentAlignment;
				editorControl.VerticalContentAlignment = baseEdit.VerticalContentAlignment;
				editorControl.IsReadOnly = baseEdit.IsReadOnly;
				editorControl.IsTextEditable = CalcIsTextEditable();
			}
		}
		bool CalcIsTextEditable() {
			var bes = editSettings as ButtonEditSettings;
			if (bes == null || bes.IsTextEditable == null)
				return false;
			return bes.IsTextEditable.Value;
		}
		object CalcIsChecked() {
			var checkEditSettings = Settings as CheckEditSettings;
			if (checkEditSettings == null)
				return null;
			return CheckEditSettings.GetBooleanFromEditValue(EditValueCache, checkEditSettings.IsThreeState);
		}
		bool CalcIsTokenMode() {
			if (Settings == null)
				return false;
			var styleSettings = Settings.StyleSettings as ITokenStyleSettings;
			if (styleSettings == null)
				return false;
			return styleSettings.IsTokenStyleSettings();
		}
		object CalcEditorControlValue() {
			bool isTokenMode = CalcIsTokenMode();
			return isTokenMode ? CalcTokenEditValue(EditValueCache) : EditValueCache;
		}
		object CalcTokenEditValue(object editValue) {
			if (editValue == null)
				return null;
			var editSettings = Settings as LookUpEditSettingsBase;
			if (editSettings == null)
				return null;
			IItemsProvider2 provider = editSettings.ItemsProvider;
			IList listEditValue = editValue as IList;
			TokenEditorCustomItem newEditBoxValue = null;
			if (listEditValue != null) {
				List<CustomItem> items = listEditValue.Cast<object>().Select(x => GetCustomItemFromValue(provider, x)).Where(item => item != null).ToList();
				if (items.Count > 0) {
					newEditBoxValue = new TokenEditorCustomItem() { Index = -1 };
					newEditBoxValue.EditValue = items;
				}
			}
			else {
				newEditBoxValue = new TokenEditorCustomItem() { Index = -1 };
				object displayValue = provider.GetDisplayValueByEditValue(editValue, provider.CurrentDataViewHandle);
				newEditBoxValue.EditValue = new List<CustomItem>() {
					new CustomItem() { EditValue = editValue, DisplayText = displayValue != null ? displayValue.ToString() : string.Empty }
				};
			}
			return newEditBoxValue;
		}
		private CustomItem GetCustomItemFromValue(IItemsProvider2 provider, object editValue) {
			var newItem = new CustomItem();
			GetDisplayText(editValue, true);
			object displayValue = CalcDisplayText(editValue);
			if (editValue != null) {
				newItem.EditValue = editValue;
				newItem.DisplayText = displayValue != null ? displayValue.ToString() : null;
			}
			return newItem;
		}
		object CalcSelectedItem() {
			var editSettings = Settings as LookUpEditSettingsBase;
			if (editSettings == null)
				return null;
			return editSettings.ItemsProvider.GetItem(EditValueCache, editSettings.ItemsProvider.CurrentDataViewHandle);
		}
		object CalcSelectedIndex() {
			var editSettings = Settings as LookUpEditSettingsBase;
			if (editSettings == null)
				return null;
			return editSettings.ItemsProvider.IndexOfValue(EditValueCache, editSettings.ItemsProvider.CurrentDataViewHandle);
		}
		ControlTemplate CalcDisplayTemplate() {
			var editSettings = Settings as LookUpEditSettingsBase;
			var resourceProvider = ThemeHelper.GetResourceProvider(this);
			if (editSettings == null)
				return null;
			if (CalcIsTokenMode())
				return resourceProvider.GetTokenEditorDisplayTemplate(this);
			if (!editSettings.ApplyItemTemplateToSelectedItem || editSettings.IsTextEditable.GetValueOrDefault(true))
				return null;
			var comboBoxEditSettings = Settings as ComboBoxEditSettings;
			if (comboBoxEditSettings != null && comboBoxEditSettings.GetApplyImageTemplateToSelectedItem()) {
				return resourceProvider.GetSelectedItemImageTemplate(this);
			}
			return resourceProvider.GetSelectedItemTemplate(this);
		}
		DataTemplate CalcItemTemplate() {
			var editSettings = Settings as LookUpEditSettingsBase;
			if (editSettings == null)
				return null;
			return editSettings.ItemTemplate;
		}
		DataTemplateSelector CalcItemTemplateSelector() {
			var editSettings = Settings as LookUpEditSettingsBase;
			if (editSettings == null)
				return null;
			return editSettings.ItemTemplateSelector;
		}
		void ProcessValidationProvider() {
			errorProvider = (RenderControlContext)Namescope.GetElement(ErrorProviderName);
			AssignErrorProviderValues();
		}
		void AssignErrorProviderValues() {
			if (IsInSupportInitialize)
				return;
			if (errorProvider != null) {
				errorProvider.BeginInit();
				errorProvider.Visibility = validationError != null ? Visibility.Visible : Visibility.Collapsed;
				errorProvider.DataContext = validationError;
				errorProvider.EndInit();
			}
		}
		void ProcessInplaceButtonEdit() {
			ButtonEditSettings settings = Settings as ButtonEditSettings;
			if (settings == null)
				return;
			leftButtonsItemsControl = Namescope.GetElement(LeftButtonsName) as RenderItemsControlContext;
			rightButtonsItemsControl = Namescope.GetElement(RightButtonsName) as RenderItemsControlContext;
			IInplaceBaseEdit ibe = this;
			var buttons = ibe.GetSortedButtons();
			var leftButtons = buttons.Where(btn => btn.IsLeft).ToList();
			if (leftButtonsItemsControl != null) {
				leftButtonsItemsControl.Visibility = leftButtons.Any() ? Visibility.Visible : Visibility.Collapsed;
				leftButtonsItemsControl.ItemsSource = leftButtons;
			}
			var rightButtons = buttons.Where(btn => !btn.IsLeft).ToList();
			if (rightButtonsItemsControl != null) {
				rightButtonsItemsControl.Visibility = rightButtons.Any() ? Visibility.Visible : Visibility.Collapsed;
				rightButtonsItemsControl.ItemsSource = rightButtons;
			}
		}
		void ProcessTextBlock() {
			textBlock = Namescope.GetElement(EditorName) as RenderTextBlockContext;
			AssignTextBlockValues();
		}
		void ProcessInplaceCheckBox() {
			checkBox = Namescope.GetElement(EditorName) as RenderCheckBoxContext;
			AssignCheckBoxValues();
		}
		void ProcessActiveEditor() {
			isValueChanged = activeEditor.Return(x => x.IsValueChanged, () => isValueChanged);
			activeEditor.Do(x => x.EditValueChanged -= ActiveEditorEditValueChanged);
			activeEditor = Namescope.GetElement(EditorName) as RenderBaseEditContext;
			activeEditor.Do(x => x.SetDisplayTextProvider(displayTextProvider));
			AssignActiveEditorValues();
			SetEditIsValueChanged(isValueChanged);
			activeEditor.Do(x => x.EditValueChanged += ActiveEditorEditValueChanged);
			RaiseEditorActivated();
		}
		void RaiseEditorActivated() {
			if (activeEditor == null)
				return;
			if (EditorActivated != null)
				EditorActivated(this, new RoutedEventArgs());
		}
		void ActiveEditorEditValueChanged(object sender, EditValueChangedEventArgs e) {
			EditValue = e.NewValue;
		}
		protected override void ReleaseContext(FrameworkRenderElementContext context) {
			base.ReleaseContext(context);
			ProcessInnerContext();
		}
		void RaiseEditValueChanged(object oldValue, object newValue) {
			if (EditValueChanged != null)
				EditValueChanged(this, new EditValueChangedEventArgs(oldValue, newValue));
		}
		public override void BeginInit() {
			base.BeginInit();
			initialized = false;
			((IBaseEdit)this).BeginInit(false);
		}
		public override void EndInit() {
			base.EndInit();
			((IBaseEdit)this).EndInit(false);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (!initialized) {
				InternalInitialize();
				initialized = true;
			}
		}
		void IBaseEdit.BeginInit(bool callBase) {
			supportInitializeLocker.Lock();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			IInplaceBaseEdit editor = this;
			if (!editor.ShowToolTipForTrimmedText)
				return;
			bool isTextTrimmed = textBlock.Return(x => TextBlockService.CalcIsTextTrimmed(textBlock), () => false);
			ToolTipService.SetToolTip(this, CalcToolTip(isTextTrimmed, validationError));
		}
		object CalcToolTip(bool isTextTrimmed, BaseValidationError error) {
			if (error != null)
				return new ContentControl()
				{
					Content = error,
					ContentTemplate = (DataTemplate)FindResource(new InplaceBaseEditThemeKeyExtension { ResourceKey = InplaceBaseEditThemeKeys.ValidationErrorToolTipTemplate, ThemeName = ThemeHelper.GetEditorThemeName(this) })
				};
			if (isTextTrimmed)
				return new ContentControl()
				{
					Content = DisplayText,
					ContentTemplate = (DataTemplate)FindResource(new InplaceBaseEditThemeKeyExtension { ResourceKey = InplaceBaseEditThemeKeys.TrimmedTextToolTipTemplate, ThemeName = ThemeHelper.GetEditorThemeName(this) })
				};
			return null;
		}
		void IBaseEdit.SetInplaceEditingProvider(IInplaceEditingProvider provider) {
			this.provider = provider;
			activeEditor.SetInplaceEditingProvider(provider);
		}
		bool IBaseEdit.IsChildElement(IInputElement element, DependencyObject root) {
			return LayoutHelper.IsChildElementEx(root ?? this, (DependencyObject)element, true) || activeEditor.With(x => x.Control as IBaseEdit).If(x => x.IsChildElement(element, root ?? this)).ReturnSuccess();
		}
		void IBaseEdit.EndInit(bool callBase, bool shouldSync) {
			supportInitializeLocker.Unlock();
			if (IsInSupportInitialize || initialized)
				return;
			InternalInitialize();
		}
		void InternalInitialize() {
			var resourceProvider = ThemeHelper.GetResourceProvider(this);
			RenderTemplateSelector = resourceProvider.GetTextEditTemplateSelector(this);
			AssignValues();
		}
		IBaseEdit IInplaceBaseEdit.BaseEdit { get { return (BaseEdit)activeEditor.With(x => x.Control); } }
		void IInplaceBaseEdit.RaiseEditValueChanged(object oldValue, object newValue) {
			EditValue = newValue;
		}
		TextTrimming IInplaceBaseEdit.TextTrimming {
			get { return textTrimming; }
			set {
				if (textTrimming == value)
					return;
				textTrimming = value;
				(Settings as TextEditSettings).Do(x => x.TextTrimming = value);
				AssignTextBlockValues();
			}
		}
		TextWrapping IInplaceBaseEdit.TextWrapping {
			get { return textWrapping; }
			set {
				if (textWrapping == value)
					return;
				textWrapping = value;
				var ts = Settings as TextEditSettings;
				if (ts != null) {
					var wrapping = ts.TextWrapping;
					if (wrapping != value)
						ts.SetCurrentValue(TextEditSettings.TextWrappingProperty, value);
				}
				AssignTextBlockValues();
			}
		}
		bool IInplaceBaseEdit.AllowDefaultButton {
			get { return allowDefaultButton; }
			set {
				if (allowDefaultButton == value)
					return;
				allowDefaultButton = value;
				ProcessInplaceButtonEdit();
			}
		}
		HighlightedTextCriteria IInplaceBaseEdit.HighlightedTextCriteria {
			get { return highlightedTextCriteria; }
			set {
				if (highlightedTextCriteria == value)
					return;
				highlightedTextCriteria = value;
				AssignTextBlockValues();
			}
		}
		TextDecorationCollection IInplaceBaseEdit.TextDecorations {
			get { return textDecorations; }
			set {
				if (textDecorations == value)
					return;
				textDecorations = value;
				AssignTextBlockValues();
			}
		}
		string IInplaceBaseEdit.HighlightedText {
			get { return highlightedText; }
			set {
				if (highlightedText == value)
					return;
				highlightedText = value;
				AssignTextBlockValues();
			}
		}
		RenderTemplate IInplaceBaseEdit.BorderTemplate {
			get { return borderTemplate; }
			set {
				if (borderTemplate == value)
					return;
				borderTemplate = value;
				UpdateEditorContent();
			}
		}
		IEnumerable<ButtonInfoBase> IInplaceBaseEdit.GetSortedButtons() {
			ButtonEditSettings settings = Settings as ButtonEditSettings;
			if (settings == null)
				return new List<ButtonInfoBase>();
			List<ButtonInfoBase> list = new List<ButtonInfoBase>();
			if (!GetShowEditorButtons())
				return list;
			var rightButtons = new List<ButtonInfoBase>();
			foreach (var bi in settings.GetActualButtons()) {
				if (bi.IsLeft)
					list.Add(bi);
				else
					rightButtons.Add(bi);
			}
			if (allowDefaultButton) {
				var defaultButton = settings.CreateDefaultButtonInfo();
				list.Add(defaultButton);
			}
			list.AddRange(rightButtons);
			list = list.OrderBy(x => x.Index).ToList();
			string themeName = ThemeHelper.GetEditorThemeName(this);
			foreach (var buttonInfoBase in list) {
				ThemeManager.SetThemeName(buttonInfoBase, themeName);
			}
			return list;
		}
		object IBaseEdit.EditValue {
			get { return EditValueCache; }
			set {
				if (EditMode == EditMode.InplaceInactive)
					OnEditValueChanged(EditValueCache, value);
				else {
					EditValue = value;
				}
			}
		}
		Style IInplaceBaseEdit.ActiveEditorStyle {
			get { return editCoreStyle; }
			set {
				if (editCoreStyle == value)
					return;
				editCoreStyle = value;
				if (activeEditor != nullValue)
					activeEditor.Style = editCoreStyle;
			}
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new UIElementAutomationPeer(this);
		}
	}
	public class EditorContent {
		public bool HasDisplayTemplate { get { return DisplayTemplate != null; } }
		public bool ShowEditorButtons { get; set; }
		public bool ShowBorder { get; set; }
		public ControlTemplate DisplayTemplate { get; set; }
		public BaseValidationError Error { get; set; }
		public bool HasValidationError { get { return Error != null; } }
		public EditMode EditMode { get; set; }
		public BaseEditSettings Settings { get; set; }
		protected bool Equals(EditorContent other) {
			return ShowEditorButtons.Equals(other.ShowEditorButtons)
				&& ShowBorder.Equals(other.ShowBorder)
				&& EditMode == other.EditMode
				&& Settings.IsCompatibleWith(other.Settings)
				&& HasValidationError == other.HasValidationError
				&& HasDisplayTemplate == other.HasDisplayTemplate;
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((EditorContent)obj);
		}
		public override int GetHashCode() {
			unchecked {
				var hashCode = ShowEditorButtons.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowBorder.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)EditMode;
				return hashCode;
			}
		}
	}
}
