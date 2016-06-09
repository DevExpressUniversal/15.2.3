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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Editors.Internal {
	public class ContentToTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			return content as RenderTemplate;
		}
	}
	public sealed class RenderSpinButtonLeftTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderSpinLeftButtonTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonUpTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderSpinUpButtonTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonRightTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderSpinRightButtonTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonDownTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderSpinDownButtonTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonLeftGlyphTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetButtonInfoSpinLeftGlyphKindTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonUpGlyphTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetButtonInfoSpinUpGlyphKindTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonRightGlyphTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetButtonInfoSpinDownGlyphKindTemplate(chrome);
		}
	}
	public sealed class RenderSpinButtonDownGlyphTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetButtonInfoSpinDownGlyphKindTemplate(chrome);
		}
	}
	public class InplaceBaseEditTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			if (content == null)
				return base.SelectTemplate(chrome, null);
			InplaceResourceProvider provider = ThemeHelper.GetResourceProvider(chrome);
			EditorContent editor = (EditorContent)content;
			return editor.EditMode == EditMode.InplaceInactive ?
				GetTextEditInplaceInactiveTemplate(chrome, editor) : provider.GetTextEditInplaceActiveTemplate(chrome);
		}
		static RenderTemplate GetTextEditInplaceInactiveTemplate(FrameworkElement chrome, EditorContent editor) {
			var settings = editor.Settings;
			var ceSettings = settings as CheckEditSettings;
			InplaceResourceProvider provider = ThemeHelper.GetResourceProvider(chrome);
			if (editor.DisplayTemplate != null || (editor.Settings as LookUpEditSettingsBase).If(x => x.IsTextEditable != null && (x.ApplyItemTemplateToSelectedItem && !x.IsTextEditable.Value)).ReturnSuccess())
				return provider.GetCommonBaseEditInplaceInactiveTemplateWithDisplayTemplate(chrome);
			if (ceSettings != null)
				return provider.GetCheckEditInplaceInactiveTemplate(chrome);
			var beSettings = settings as ButtonEditSettings;
			bool showEditorButtons = editor.ShowEditorButtons;
			if (beSettings != null && showEditorButtons || editor.ShowBorder || editor.Error != null)
				return provider.GetCommonBaseEditInplaceInactiveTemplate(chrome);
			return provider.GetTextEditInplaceInactiveTemplate(chrome);
		}
	}
	public class InplaceButtonInfoTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			if (content == null || !(content is ButtonInfo))
				return null;
			var buttonInfo = (ButtonInfo)content;
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			var glyphKind = buttonInfo.GlyphKind;
			if (buttonInfo.ContentRenderTemplate != null)
				return buttonInfo.ContentRenderTemplate;
			switch (glyphKind) {
				case GlyphKind.DropDown:
					return resourceProvider.GetButtonInfoDropDownGlyphKindTemplate(chrome);
				case GlyphKind.Regular:
					return resourceProvider.GetButtonInfoRegularGlyphKindTemplate(chrome);
				case GlyphKind.Right:
					return resourceProvider.GetButtonInfoRightGlyphKindTemplate(chrome);
				case GlyphKind.Left:
					return resourceProvider.GetButtonInfoLeftGlyphKindTemplate(chrome);
				case GlyphKind.Up:
					return resourceProvider.GetButtonInfoUpGlyphKindTemplate(chrome);
				case GlyphKind.Down:
					return resourceProvider.GetButtonInfoDownGlyphKindTemplate(chrome);
				case GlyphKind.Cancel:
					return resourceProvider.GetButtonInfoCancelGlyphKindTemplate(chrome);
				case GlyphKind.Apply:
					return resourceProvider.GetButtonInfoApplyGlyphKindTemplate(chrome);
				case GlyphKind.Plus:
					return resourceProvider.GetButtonInfoPlusGlyphKindTemplate(chrome);
				case GlyphKind.Minus:
					return resourceProvider.GetButtonInfoMinusGlyphKindTemplate(chrome);
				case GlyphKind.Redo:
					return resourceProvider.GetButtonInfoRedoGlyphKindTemplate(chrome);
				case GlyphKind.Undo:
					return resourceProvider.GetButtonInfoUndoGlyphKindTemplate(chrome);
				case GlyphKind.Refresh:
					return resourceProvider.GetButtonInfoRefreshGlyphKindTemplate(chrome);
				case GlyphKind.Search:
					return resourceProvider.GetButtonInfoSearchGlyphKindTemplate(chrome);
				case GlyphKind.NextPage:
					return resourceProvider.GetButtonInfoNextPageGlyphKindTemplate(chrome);
				case GlyphKind.PrevPage:
					return resourceProvider.GetButtonInfoPrevPageGlyphKindTemplate(chrome);
				case GlyphKind.Last:
					return resourceProvider.GetButtonInfoLastGlyphKindTemplate(chrome);
				case GlyphKind.First:
					return resourceProvider.GetButtonInfoFirstGlyphKindTemplate(chrome);
				case GlyphKind.Edit:
					return resourceProvider.GetButtonInfoEditGlyphKindTemplate(chrome);
			}
			return resourceProvider.GetButtonInfoNoneGlyphKindTemplate(chrome);
		}
	}
	public class DefaultButtonInfoTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var context = content as RenderContentControlContext;
			var dataContext = context.With(x => x.Content);
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			var spinInfo = dataContext as SpinButtonInfo;
			if (spinInfo != null)
				return SelectSpinButtonTemplate(chrome, spinInfo);
			return (dataContext as ButtonInfo).With(x => x.RenderTemplate) ?? resourceProvider.GetRenderButtonTemplate(chrome);
		}
		RenderTemplate SelectSpinButtonTemplate(FrameworkElement chrome, SpinButtonInfo spinInfo) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderSpinButtonTemplate(chrome);
		}
	}
	public class ValidationErrorTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			if (content == null)
				return base.SelectTemplate(chrome, content);
		   var error = (BaseValidationError)((RenderControlContext)content).DataContext;
		   if (error != null) {
			   ErrorType errorType = error.ErrorType;
			   var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			   var edit = chrome as IBaseEdit;
			   if (edit != null && edit.ValidationErrorTemplate != null)
				   return resourceProvider.GetValidationErrorTemplate(chrome);
			   if (errorType == ErrorType.Critical)
				   return resourceProvider.GetCriticalErrorTemplate(chrome);
			   if (errorType == ErrorType.Information)
				   return resourceProvider.GetInformationErrorTemplate(chrome);
			   if (errorType == ErrorType.Warning)
				   return resourceProvider.GetWarningErrorTemplate(chrome);
			   return resourceProvider.GetCriticalErrorTemplate(chrome);
		   }
		   return null;
		}
	}
#if DEBUGTEST
	[DevExpress.Xpf.Editors.Tests.IgnoreFREChecker]
#endif
	public class RenderBaseEdit : RenderControlBase {
		protected override FrameworkElement CreateFrameworkElement(FrameworkRenderElementContext context) {
			RenderBaseEditContext bec = (RenderBaseEditContext)context;
			var chrome = (InplaceBaseEdit)context.ElementHost.Parent;
			var baseEdit = (IInplaceBaseEdit)chrome;
			var settings = chrome.Settings;
			var ibe = settings.CreateEditor();
			settings.AssignToEdit(ibe);
			ibe.EditMode = baseEdit.EditMode;
			ibe.VerticalContentAlignment = bec.VerticalContentAlignment;
			ibe.HorizontalContentAlignment = bec.HorizontalContentAlignment;
			(ibe as TextEditBase).Do(x => x.TextTrimming = baseEdit.TextTrimming);
			(ibe as TextEditBase).Do(x => x.TextWrapping = baseEdit.TextWrapping);
			ibe.ValidationError = baseEdit.ValidationError;
			return (FrameworkElement)ibe;
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderBaseEditContext(this);
		}
	}
	public class RenderBaseEditContext : RenderControlBaseContext {
		public event EventHandler<EditValueChangedEventArgs> EditValueChanged;
		BaseEdit Editor { get { return (BaseEdit)Control; } }
		public RenderBaseEditContext(RenderBaseEdit factory)
			: base(factory) {
		}
		public bool IsEditorActive { get { return Editor.IsEditorActive; } }
		public object EditValue {
			get { return Editor.EditValue; }
			set { Editor.EditValue = value; }
		}
		public bool IsValueChanged {
			get { return Editor.IsValueChanged; }
			set { Editor.IsValueChanged = value; }
		}
		public BaseValidationError ValidationError {
			get { return Editor.ValidationError; }
			set { Editor.ValidationError = value; }
		}
		public bool IsReadOnly {
			get { return Editor.IsReadOnly; }
			set { Editor.IsReadOnly = value; }
		}
		public ControlTemplate EditTemplate {
			get { return Editor.EditTemplate; }
			set { Editor.EditTemplate = value; }
		}
		public ControlTemplate DisplayTemplate {
			get { return Editor.DisplayTemplate; }
			set { Editor.DisplayTemplate = value; }
		}
		public object RealDataContext {
			get { return Editor.DataContext; }
			set { Editor.DataContext = value; }
		}
		public Style Style {
			get { return Editor.Style; }
			set { Editor.Style = value; }
		}
		public EditMode EditMode {
			get { return Editor.EditMode; }
			set { Editor.EditMode = value; }
		}
		public InvalidValueBehavior InvalidValueBehavior {
			get { return Editor.InvalidValueBehavior; }
			set { Editor.InvalidValueBehavior = value; }
		}
		protected internal override void AttachToVisualTree(FrameworkElement root) {
			base.AttachToVisualTree(root);
			Editor.EditValueChanged += EditorValueChanged;
			if (root.IsKeyboardFocused && ((IBaseEdit)root).CanAcceptFocus)
				Editor.Focus();
		}
		protected internal override void DetachFromVisualTree(FrameworkElement root) {
			if (Editor.IsKeyboardFocusWithin && ((IBaseEdit)root).CanAcceptFocus)
				root.Focus();
			base.DetachFromVisualTree(root);
			Editor.EditValueChanged -= EditorValueChanged;
		}
		void EditorValueChanged(object sender, EditValueChangedEventArgs args) {
			RaiseEditValueChanged(args);
		}
		void RaiseEditValueChanged(EditValueChangedEventArgs args) {
			if (EditValueChanged != null)
				EditValueChanged(this, args);
		}
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			return Editor.NeedsKey(key, modifiers);
		}
		public void ProcessActivatingKey(Key key, ModifierKeys modifiers) {
			Editor.ProcessActivatingKey(key, modifiers);
		}
		public void FlushPendingEditActions() {
			BaseEditHelper.FlushPendingEditActions(Editor);
		}
		public void SelectAll() {
			Editor.SelectAll();
		}
		public bool DoValidate() {
			return Editor.Return(x => x.DoValidate(), () => true);
		}
		public void SetInplaceEditingProvider(IInplaceEditingProvider provider) {
			((IBaseEdit)Editor).SetInplaceEditingProvider(provider);
		}
		public void SetDisplayTextProvider(IDisplayTextProvider provider) {
			Editor.SetDisplayTextProvider(provider);
		}
	}
	public class RenderEditorControl : RenderRealControl {
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderEditorControlContext(this);
		}
		protected override FrameworkElement CreateFrameworkElement(FrameworkRenderElementContext context) {
			return new EditorControlStub() { Focusable = false, FocusVisualStyle = null };
		}
	}
	public class RenderEditorControlContext : RenderRealControlContext {
		EditorControlStub EditorControl { get { return base.Control as EditorControlStub; } }
		public bool IsReadOnly {
			get { return EditorControl.IsReadOnly; }
			set { EditorControl.IsReadOnly = value; }
		}
		public object EditValue {
			get { return EditorControl.EditValue; }
			set { EditorControl.EditValue = value; }
		}
		public object IsChecked {
			get { return EditorControl.IsChecked; }
			set { EditorControl.IsChecked = value; }
		}
		public object SelectedItem {
			get { return EditorControl.SelectedItem; }
			set { EditorControl.SelectedItem = value; }
		}
		public object SelectedIndex {
			get { return EditorControl.SelectedIndex; }
			set { EditorControl.SelectedIndex = value; }
		}
		public string HighlightedText {
			get { return EditorControl.HighlightedText; }
			set { EditorControl.HighlightedText = value; }
		}
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return EditorControl.HighlightedTextCriteria; }
			set { EditorControl.HighlightedTextCriteria = value; }
		}
		public object DisplayText {
			get { return EditorControl.DisplayText; }
			set { EditorControl.DisplayText = value; }
		}
		public object RealDataContext {
			get { return EditorControl.DataContext; }
			set { EditorControl.DataContext = value; }
		}
		public DataTemplate ItemTemplate {
			get { return EditorControl.ItemTemplate; }
			set { EditorControl.ItemTemplate = value; }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return EditorControl.ItemTemplateSelector; }
			set { EditorControl.ItemTemplateSelector = value; }
		}
		public bool IsTextEditable {
			get { return EditorControl.IsTextEditable; }
			set { EditorControl.IsTextEditable = value; }
		}
		public RenderEditorControlContext(RenderRealControl factory)
			: base(factory) {
		}
	}
	public class EditorControlStub : ControlEx {
		protected const string ContentPresenterName = "PART_ContentPresenter";
		protected const string EditCoreName = "PART_Editor";
		protected const string ImagePresenterName = "PART_Image";
		public static readonly DependencyProperty IsTextEditableProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedIndexProperty;
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty HighlightedTextProperty;
		public static readonly DependencyProperty HighlightedTextCriteriaProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		static EditorControlStub() {
			Type ownerType = typeof(EditorControlStub);
			HighlightedTextProperty = DependencyPropertyManager.Register("HighlightedText", typeof(string), ownerType,
				new PropertyMetadata(null, (o, args) => ((EditorControlStub)o).HighlightedTextChanged(args.NewValue)));
			HighlightedTextCriteriaProperty = DependencyPropertyManager.Register("HighlightedTextCriteria", typeof(HighlightedTextCriteria), ownerType, new PropertyMetadata(HighlightedTextCriteria.StartsWith));
			IsTextEditableProperty = DependencyPropertyManager.Register("IsTextEditable", typeof(bool), ownerType, new PropertyMetadata(false));
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool), ownerType, new PropertyMetadata(false));
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new PropertyMetadata(null));
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(object), ownerType, new PropertyMetadata(null));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), ownerType, new PropertyMetadata(null, SelectedItemChanged));
			SelectedIndexProperty = DependencyPropertyManager.Register("SelectedIndex", typeof(object), ownerType, new PropertyMetadata(null));
			DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(object), ownerType,
				new PropertyMetadata(null, (o, args) => ((EditorControlStub)o).DisplayTextChanged(args.NewValue)));
			ItemTemplateSelectorProperty = DependencyPropertyManager.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, ItemTemplateSelectorChanged));
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, ItemTemplateChanged));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(VerticalContentAlignmentChanged));
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(HorizontalContentAlignmentChanged));
		}
		static void HorizontalContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControlStub)d).AssignHorizontalContentAlignment();
		}
		static void VerticalContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControlStub)d).AssignVerticalContentAlignment();
		}
		static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControlStub)d).AssignContent();
		}
		static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControlStub)d).AssignContentTemplate();
		}
		static void ItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControlStub)d).AssignContentTemplateSelector();
		}
		public string HighlightedText {
			get { return (string)GetValue(HighlightedTextProperty); }
			set { SetValue(HighlightedTextProperty, value); }
		}
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return (HighlightedTextCriteria)GetValue(HighlightedTextCriteriaProperty); }
			set { SetValue(HighlightedTextCriteriaProperty, value); }
		}
		public bool IsTextEditable {
			get { return (bool)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public object IsChecked {
			get { return GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public object SelectedIndex {
			get { return GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		public object DisplayText {
			get { return GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		protected ContentPresenter ContentPresenter { get; private set; }
		protected FrameworkElement EditCore { get; private set; }
		protected ImagePresenter ImagePresenter { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentPresenter = GetTemplateChild(ContentPresenterName) as ContentPresenter;
			EditCore = GetTemplateChild(EditCoreName) as FrameworkElement;
			ImagePresenter = GetTemplateChild(ImagePresenterName) as ImagePresenter;
			AssignProperties();
		}
		protected virtual void DisplayTextChanged(object newValue) {
			AssignText();
		}
		protected virtual void HighlightedTextChanged(object newValue) {
			AssignText();
		}
		protected virtual void ImageVisibilityChanged(Visibility oldValue, Visibility newValue) {
			AssignImage();
		}
		void AssignProperties() {
			AssignReadOnly();
			AssignText();
			AssignContent();
			AssignContentTemplate();
			AssignContentTemplateSelector();
			AssignHorizontalContentAlignment();
			AssignVerticalContentAlignment();
			AssignImage();
		}
		void AssignReadOnly() {
			if (EditCore == null)
				return;
			var ab = EditCore as TokenEditor;
			if (ab != null) {
				ab.IsReadOnly = IsReadOnly;
				ab.IsTextEditable = IsTextEditable;
				return;
			}
			var tb = EditCore as TextBox;
			if (tb != null) {
				tb.IsReadOnly = IsReadOnly;
				return;
			}
		}
		void AssignText() {
			if (EditCore == null)
				return;
			var ab = EditCore as TokenEditor;
			if (ab != null) {
				ab.SetEditValue(EditValue);
				return;
			}
			var tb = EditCore as TextBlock;
			if (tb != null)
				TextBlockService.UpdateTextBlock(tb, DisplayText as string, HighlightedText, HighlightedTextCriteria);
			else
				EditCore.SetValue(TextBlock.TextProperty, DisplayText);
		}
		void AssignImage() {
			if (ImagePresenter == null)
				return;
			ImagePresenter.DataContext = SelectedItem;
		}
		void AssignContent() {
			if (ContentPresenter == null)
				return;
			ContentPresenter.Content = SelectedItem;
		}
		void AssignContentTemplate() {
			if (ContentPresenter == null)
				return;
			ContentPresenter.ContentTemplate = ItemTemplate;
		}
		void AssignContentTemplateSelector() {
			if (ContentPresenter == null)
				return;
			ContentPresenter.ContentTemplateSelector = ItemTemplateSelector;
		}
		void AssignHorizontalContentAlignment() {
			if (ContentPresenter != null)
				ContentPresenter.HorizontalAlignment = HorizontalContentAlignment;
			var tb = EditCore as TextBlock;
			if (tb != null)
				tb.TextAlignment = HorizontalContentAlignment.ToTextAlignment();
		}
		void AssignVerticalContentAlignment() {
			if (ContentPresenter != null)
				ContentPresenter.VerticalAlignment = VerticalContentAlignment;
			var tb = EditCore as TextBlock;
			if (tb != null)
				tb.VerticalAlignment = VerticalContentAlignment;
		}
	}
}
