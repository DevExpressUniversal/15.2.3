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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
namespace DevExpress.Xpf.Editors.Internal {
	public class CellEditor : InplaceEditorBase {
		public static readonly DependencyProperty OwnerTokenEditorProperty;
		public static readonly DependencyProperty IsEditorFocusedProperty;
		public static readonly DependencyProperty IsTokenFocusedProperty;
		public static readonly DependencyProperty ItemDataProperty;
		static CellEditor() {
			Type ownerType = typeof(CellEditor);
			OwnerTokenEditorProperty = DependencyProperty.Register("OwnerTokenEditor", typeof(TokenEditor), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((CellEditor)d).OnTokenEditorChanged(e.OldValue as TokenEditor)));
			IsEditorFocusedProperty = DependencyProperty.Register("IsEditorFocused", typeof(bool), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((CellEditor)d).OnIsEditorFocusedChanged()));
			IsTokenFocusedProperty = DependencyProperty.Register("IsTokenFocused", typeof(bool), ownerType,
			  new FrameworkPropertyMetadata((d, e) => ((CellEditor)d).OnIsTokenFocusedChanged()));
			ItemDataProperty = DependencyProperty.Register("ItemData", typeof(TokenItemData), ownerType,
			   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((CellEditor)d).OnItemDataChanged(e.OldValue as TokenItemData)));
		}
		public CellEditor() : base() { }
		public TokenEditor OwnerTokenEditor {
			get { return (TokenEditor)GetValue(OwnerTokenEditorProperty); }
			set { SetValue(OwnerTokenEditorProperty, value); }
		}
		public bool IsEditorFocused {
			get { return (bool)GetValue(IsEditorFocusedProperty); }
			set { SetValue(IsEditorFocusedProperty, value); }
		}
		public bool IsTokenFocused {
			get { return (bool)GetValue(IsTokenFocusedProperty); }
			set { SetValue(IsTokenFocusedProperty, value); }
		}
		public TokenItemData ItemData {
			get { return (TokenItemData)GetValue(ItemDataProperty); }
			set { SetValue(ItemDataProperty, value); }
		}
		TokenEditorPresenter PresenterOwner { get; set; }
		public override void ValidateEditorCore() {
			base.ValidateEditorCore();
			Edit.DoValidate();
		}
		public override bool CanShowEditor() {
			return true;
		}
		public bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (editCore != null)
				return !IsEditorFocused && editCore.NeedsKey(key, modifiers) && ModifierKeysHelper.NoModifiers(modifiers);
			return false;
		}
		protected override void InitializeBaseEdit(IBaseEdit newEdit, InplaceEditorBase.BaseEditSourceType newBaseEditSourceType) {
			base.InitializeBaseEdit(newEdit, newBaseEditSourceType);
			ShowEditorButtons(PresenterOwner.ShowButtons);
		}
		protected override void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			base.OnEditValueChanged(sender, e);
			OwnerTokenEditor.ProcessActiveEditorEditValueChanged(e.OldValue, e.NewValue);
		}
		protected override InplaceEditorOwnerBase Owner {
			get { return OwnerTokenEditor != null ? OwnerTokenEditor.CellEditorOwner : null; }
		}
		protected override IInplaceEditorColumn EditorColumn {
			get { return ItemData != null ? ItemData.Column : null; }
		}
		protected override bool IsCellFocused {
			get { return IsEditorFocused; }
		}
		protected override bool IsReadOnly {
			get { return OwnerTokenEditor != null ? OwnerTokenEditor.IsReadOnly : false; }
		}
		protected override bool OverrideCellTemplate {
			get { return false; }
		}
		protected override bool IsInactiveEditorButtonVisible() {
			return true;
		}
		protected override object GetEditableValue() {
			if (ItemData != null) return ItemData.DisplayText;
			return null;
		}
		protected override void OnEditorActivated(object sender, RoutedEventArgs e) {
			base.OnEditorActivated(sender, e);
			UpdateActiveEditor();
			ShowEditorButtons(false);
		}
		public override void CancelEditInVisibleEditor() {
			OwnerTokenEditor.BeforeCancelEdit();
			base.CancelEditInVisibleEditor();
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			OwnerTokenEditor.OnTokenHided();
			ShowEditorButtons(PresenterOwner.ShowButtons);
		}
		protected override bool PostEditorCore() {
			return true;
		}
		protected override void OnEditorPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if (!OwnerTokenEditor.IsEditorKeyboardFocused)
				base.OnEditorPreviewLostKeyboardFocus(sender, e);
		}
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			if (editor != null && ItemData != null) {
				editor.EditValue = ItemData.DisplayText;
				(editCore as UIElement).Do(x => x.InvalidateMeasure());
			}
		}
		protected override EditableDataObject GetEditorDataContext() {
			return ItemData;
		}
		protected override IBaseEdit CreateEditor(Settings.BaseEditSettings settings) {
			var editor = settings.CreateEditor(EditorColumn, Helpers.EditorOptimizationMode.Disabled);
			SetupEditor(editor);
			return editor;
		}
		private void OnIsTokenFocusedChanged() { }
		private void OnTokenEditorChanged(TokenEditor oldValue) {
			OnOwnerChanged(ItemData != null ? ItemData.Column : null);
		}
		private void OnIsEditorFocusedChanged() {
			OnIsFocusedCellChanged();
			ShowEditorButtons(!IsEditorFocused);
		}
		internal void Refresh() {
			OnIsFocusedCellChanged();
		}
		private void ShowEditorButtons(bool value) {
			GetEditor().Do((x) => x.ShowEditorButtons = true);
		}
		protected override void ProcessMouseEventInInplaceInactiveMode(MouseButtonEventArgs e) {
			base.ProcessMouseEventInInplaceInactiveMode(e);
		}
		private void OnItemDataChanged(TokenItemData oldData) {
			if (IsInEditingMode())
				UpdateEditValueInEditingMode();
			else {
				if (ItemData == null) SetEdit(null);
				OnOwnerChanged(oldData.Return(x => x.Column, () => null));
				UpdateEditCoreEditValue();
			}
		}
		private bool IsInEditingMode() {
			return OwnerTokenEditor != null && OwnerTokenEditor.HasActiveEditor;
		}
		private void UpdateActiveEditor() {
			OwnerTokenEditor.OnStartEditing(GetEditor() as TextEdit);
		}
		internal void UpdateEditCoreEditValue() {
			UpdateEditValue(editCore);
		}
		private void UpdateEditValueInEditingMode() {
			UpdateEditCoreEditValue();
		}
		internal void SetPresenterOwner(TokenEditorPresenter owner) {
			PresenterOwner = owner;
		}
		private void SetupEditor(IBaseEdit editor) {
			var inplaceEdit = editor as ButtonEdit;
			if (inplaceEdit == null) return;
			inplaceEdit.IsTabStop = false;
			inplaceEdit.ShowEditorButtons = PresenterOwner.ShowButtons;
			inplaceEdit.Style = PresenterOwner.ActiveEditorStyle;
			inplaceEdit.IsReadOnly = IsReadOnly;
			inplaceEdit.SelectAllOnGotFocus = true;
			inplaceEdit.CharacterCasing = OwnerTokenEditor.CharacterCasing;
			inplaceEdit.ShowTooltipForTrimmedText = false;
		}
		internal IBaseEdit GetEditor() {
			return BaseEditHelper.GetBaseEdit(editCore);
		}
		internal IBaseEdit GetInactiveEditor() {
			return editCore;
		}
		internal void FocusEditCore() {
			if (editCore != null) editCore.Focus();
		}
		public bool ShouldUpdateOwner { get; set; }
	}
	public class CellEditorOwner : InplaceEditorOwnerBase {
		public CellEditorOwner(TokenEditor owner) : base(owner) { }
		public TokenEditor OwnerBox { get { return owner as TokenEditor; } }
		protected override System.Windows.FrameworkElement FocusOwner {
			get { return OwnerBox; }
		}
		protected override Core.EditorShowMode EditorShowMode {
			get { return Core.EditorShowMode.MouseDown; }
		}
		protected override bool EditorSetInactiveAfterClick {
			get { throw new NotImplementedException(); }
		}
		protected override Type OwnerBaseType {
			get { return typeof(TokenEditor); }
		}
		protected internal override void EnqueueImmediateAction(IAction action) {
			OwnerBox.ImmediateActionsManager.EnqueueAction(action);
		}
		public override void ProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
			OwnerBox.ProcessKeyDownFromCellEditor(e);
		}
		protected internal override string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value) {
			return originalDisplayText;
		}
		protected internal override bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText) {
			displayText = originalDisplayText;
			return true;
		}
		protected override bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e) {
			return true;
		}
		protected override bool CommitEditing() {
			return true;
		}
	}
	public class TokenEditorPresenter : Control {
		public static readonly DependencyProperty IsTokenFocusedProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty ActiveEditorStyleProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty NullTextProperty;
		public static readonly DependencyProperty ShowButtonsProperty;
		public static readonly DependencyProperty ItemProperty;
		public static readonly DependencyProperty OwnerPresenterProperty;
		static readonly DependencyPropertyKey OwnerPresenterPropertyKey;
		public static readonly DependencyProperty IsEditorActivatedProperty;
		public static readonly DependencyProperty DeleteItemButtonTemplateProperty;
		public static readonly DependencyProperty TokenButtonsProperty;
		public static readonly DependencyProperty HasNullTextProperty;
		public static readonly DependencyProperty IsTextEditableProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty NewTokenTextProperty;
		public static readonly DependencyProperty IsTokenEditorReadOnlyProperty;
		public static readonly DependencyProperty IsNewTokenEditorPresenterProperty;
		static TokenEditorPresenter() {
			Type ownerType = typeof(TokenEditorPresenter);
			IsTokenFocusedProperty = DependencyProperty.Register("IsTokenFocused", typeof(bool), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((TokenEditorPresenter)d).OnIsTokenFocusedChanged()));
			BorderTemplateProperty = DependencyProperty.Register("BorderTemplate", typeof(ControlTemplate), ownerType);
			ActiveEditorStyleProperty = DependencyProperty.Register("ActiveEditorStyle", typeof(Style), ownerType);
			ShowBorderProperty = DependencyProperty.Register("ShowBorder", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowButtonsProperty = DependencyProperty.Register("ShowButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			NullTextProperty = DependencyProperty.Register("NullText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, (d, e) => ((TokenEditorPresenter)d).OnNullTextChanged()));
			ItemProperty = DependencyProperty.Register("Item", typeof(CustomItem), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((TokenEditorPresenter)d).OnItemChanged()));
			OwnerPresenterPropertyKey = DependencyProperty.RegisterAttachedReadOnly("OwnerPresenter", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			OwnerPresenterProperty = OwnerPresenterPropertyKey.DependencyProperty;
			IsEditorActivatedProperty = DependencyProperty.Register("IsEditorActivated", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((TokenEditorPresenter)d).OnIsEditorActivatedChanged()));
			DeleteItemButtonTemplateProperty = DependencyProperty.Register("DeleteItemButtonTemplate", typeof(DataTemplate), ownerType);
			TokenButtonsProperty = DependencyProperty.Register("TokenButtons", typeof(ButtonInfoCollection), ownerType);
			HasNullTextProperty = DependencyProperty.Register("HasNullText", typeof(bool), ownerType);
			IsTextEditableProperty = DependencyProperty.Register("IsTextEditable", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming), ownerType);
			NewTokenTextProperty = DependencyProperty.Register("NewTokenText", typeof(string), ownerType, new FrameworkPropertyMetadata(EditorLocalizer.GetString(EditorStringId.TokenEditorNewTokenText)));
			IsNewTokenEditorPresenterProperty = DependencyProperty.Register("IsNewTokenEditorPresenter", typeof(bool), ownerType);
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType);
			IsTokenEditorReadOnlyProperty = DependencyProperty.Register("IsTokenEditorReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		public TokenEditorPresenter() {
			SetOwnerPresenter(this, this);
			DefaultStyleKey = typeof(TokenEditorPresenter);
		}
		public bool IsTokenEditorReadOnly {
			get { return (bool)GetValue(IsTokenEditorReadOnlyProperty); }
			set { SetValue(IsTokenEditorReadOnlyProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public bool IsNewTokenEditorPresenter {
			get { return (bool)GetValue(IsNewTokenEditorPresenterProperty); }
			set { SetValue(IsNewTokenEditorPresenterProperty, value); }
		}
		public string NewTokenText {
			get { return (string)GetValue(NewTokenTextProperty); }
			set { SetValue(NewTokenTextProperty, value); }
		}
		public TextTrimming TokenTextTrimming {
			get { return (TextTrimming)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public ButtonInfoCollection TokenButtons {
			get { return (ButtonInfoCollection)GetValue(TokenButtonsProperty); }
			set { SetValue(TokenButtonsProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public bool IsTextEditable {
			get { return (bool)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		public bool IsTokenFocused {
			get { return (bool)GetValue(IsTokenFocusedProperty); }
			set { SetValue(IsTokenFocusedProperty, value); }
		}
		public bool ShowButtons {
			get { return (bool)GetValue(ShowButtonsProperty); }
			set { SetValue(ShowButtonsProperty, value); }
		}
		TokenItemData itemData;
		public TokenItemData ItemData {
			get { return itemData; }
			private set {
				if (itemData != value) {
					itemData = value;
					OnItemDataChanged();
				}
			}
		}
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public DataTemplate DeleteItemButtonTemplate {
			get { return (DataTemplate)GetValue(DeleteItemButtonTemplateProperty); }
			set { SetValue(DeleteItemButtonTemplateProperty, value); }
		}
		public Style ActiveEditorStyle {
			get { return (Style)GetValue(ActiveEditorStyleProperty); }
			set { SetValue(ActiveEditorStyleProperty, value); }
		}
		public string NullText {
			get { return (string)GetValue(NullTextProperty); }
			set { SetValue(NullTextProperty, value); }
		}
		public CustomItem Item {
			get { return (CustomItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public bool IsEditorActivated {
			get { return (bool)GetValue(IsEditorActivatedProperty); }
			set { SetValue(IsEditorActivatedProperty, value); }
		}
		public bool HasNullText {
			get { return (bool)GetValue(HasNullTextProperty); }
			set { SetValue(HasNullTextProperty, value); }
		}
		public Brush NullTextForeground { get { return Owner.Return(x => x.NullTextForeground, () => Brushes.Black); } } 
		public CellEditor Editor { get; private set; }
		TokenEditor Owner { get; set; }
		bool HasItemData { get { return ItemData != null; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateEditor();
			CreateOwner();
			UpdateItemData(Item);
			InitializeEditor();
		}
		private void CreateOwner() {
			Owner = LayoutHelper.FindLayoutOrVisualParentObject<TokenEditor>(this, true);
			var bind = new Binding("IsReadOnly") { Source = Owner, Mode = BindingMode.OneWay };
			SetBinding(IsTokenEditorReadOnlyProperty, bind);
		}
		public static TokenEditorPresenter GetOwnerPresenter(DependencyObject element) {
			if (element == null) return null;
			return (TokenEditorPresenter)element.GetValue(OwnerPresenterProperty);
		}
		internal static void SetOwnerPresenter(DependencyObject element, TokenEditorPresenter value) {
			if (element == null) return;
			element.SetValue(OwnerPresenterPropertyKey, value);
		}
		protected virtual void OnNullTextChanged() {
			HasNullText = !string.IsNullOrEmpty(NullText);
			if (HasItemData)
				AssignItemData(Item);
		}
		protected virtual void OnItemChanged() {
			DataContext = Item;
			UpdateItemData(Item);
			if (Editor != null)
				Editor.InvalidateMeasure();
		}
		private void OnIsEditorActivatedChanged() {
			UpdateEditor();
			if (Owner.CanActivateToken() && IsEditorActivated) {
				Editor.IsEditorFocused = true;
			}
			else
				Editor.IsEditorFocused = false;
		}
		private void UpdateItemData(CustomItem item) {
			if (Owner == null || item == null) return;
			if (!HasItemData)
				ItemData = CreateItemData(item);
			else
				AssignItemData(item);
			UpdateEditorEditValue();
		}
		private void AssignItemData(CustomItem item) {
			ItemData.Value = item.EditValue;
			ItemData.DisplayText = item.DisplayText;
			ItemData.Settings.IsTextEditable = IsNewTokenEditorPresenter ? Owner.ShowDefaultToken && Owner.IsTextEditable : Owner.IsTextEditable;
		}
		private ButtonInfo CreateDeleteButtonInfo() {
			var buttonInfo = new ButtonInfo();
			buttonInfo.Template = DeleteItemButtonTemplate;
			buttonInfo.Command = CreateDeleteCommand();
			buttonInfo.CommandParameter = buttonInfo;
			buttonInfo.RaiseClickEventInInplaceInactiveMode = true;
			return buttonInfo;
		}
		private ICommand CreateDeleteCommand() {
			return new DevExpress.Mvvm.DelegateCommand(() => Owner.RemoveFocusedToken(this));
		}
		private TokenItemData CreateItemData(CustomItem value) {
			var item = new TokenItemData(Owner) { Value = value.EditValue, DisplayText = value.DisplayText };
			var settings = item.Settings;
			if (ShowButtons) {
				if (!Owner.IsReadOnly)
					settings.Buttons.Add(CreateDeleteButtonInfo());
				if (TokenButtons != null)
					TokenButtons.ForEach(x => settings.Buttons.Add(CloneButton(x)));
			}
			settings.AllowDefaultButton = false;
			settings.IsTextEditable = IsTextEditable;
			settings.TextTrimming = TokenTextTrimming;
			return item;
		}
		private ButtonInfo CloneButton(ButtonInfoBase info) {
			return ((ICloneable)info).Clone() as ButtonInfo;
		}
		private void CreateEditor() {
			Editor = GetCellEditor();
			if (Editor != null)
				Editor.SetPresenterOwner(this);
		}
		private CellEditor GetCellEditor() {
			return LayoutHelper.FindElementByType(this, typeof(CellEditor)) as CellEditor;
		}
		private void OnItemDataChanged() {
			UpdateEditor();
		}
		public void UpdateEditor() {
			if (Editor != null) {
				Editor.ItemData = ItemData;
				Editor.OwnerTokenEditor = Owner;
				Editor.IsTokenFocused = IsTokenFocused;
			}
		}
		private void PrepareActivateEditor() {
			Editor.IsEditorFocused = IsEditorActivated;
		}
		private void InitializeEditor() {
			UpdateEditor();
		}
		private void OnIsTokenFocusedChanged() {
			if (!IsTokenFocused)
				IsEditorActivated = false;
			else
				Owner.MakeVisibleToken(this);
		}
		public void UpdateEditorEditValue() {
			if (Editor != null)
				Editor.UpdateEditCoreEditValue();
		}
		public void Clear() {
			IsTokenFocused = IsEditorActivated = false;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (Editor != null)
				Editor.Measure(constraint);
			return base.MeasureOverride(constraint);
		}
		public void HideEditor() {
			SetCurrentValue(IsEditorActivatedProperty, false);
			Editor.HideEditor(true);
		}
		public bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (Editor != null)
				return Editor.IsActivatingKey(key, modifiers);
			return false;
		}
		public void ProcessActivatingKey(KeyEventArgs e) {
			if (Editor != null) {
				SetCurrentValue(IsEditorActivatedProperty, true);
			}
		}
		internal void FocusEditCore() {
			if (Editor != null) Editor.FocusEditCore();
		}
		internal void CommitEditor() {
			if (Editor != null)
				Editor.CommitEditor(true);
		}
	}
}
