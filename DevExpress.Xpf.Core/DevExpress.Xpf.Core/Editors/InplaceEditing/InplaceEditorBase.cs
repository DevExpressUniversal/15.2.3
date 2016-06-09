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

using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using System;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Utils;
using System.Windows.Interop;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Native;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
#if SL
using IInputElement = System.Windows.UIElement;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using TextCompositionEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs;
using System.Diagnostics;
#endif
namespace DevExpress.Xpf.Editors {
#if SL
	public class InplaceEditorContentPresenter : ContentPresenter {
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InplaceEditorBase owner = (InplaceEditorBase)System.Windows.Media.VisualTreeHelper.GetParent(this);
			if(ContentTemplate != null) {
				BaseEdit editFromTemplate = this.FindChild(typeof(BaseEdit), "PART_Editor", false) as BaseEdit;
				owner.InitializeEditorFromTemplate(editFromTemplate);
			}
		}
	}
#endif
	public class EditableDataObject : DataObjectBase {
		protected static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditableDataObject)d).OnContentChanged();
		}
		object value;
		public object Value {
			get { return value; }
			set {
				if(!object.Equals(this.value, value)) {
					object oldValue = this.value;
					this.value = CoerceValue(value);
					RaisePropertyChanged("Value");
					OnValueChanged(oldValue);
					OnContentChanged();
				} 
			}
		}
		protected virtual void OnContentChanged() {
			RaiseContentChanged();
		}
		protected virtual object CoerceValue(object newValue) {
			return newValue;
		}
		protected virtual void OnValueChanged(object oldValue) {
		}
	}
	public abstract partial class InplaceEditorBase : DataContentPresenter, IDisplayTextProvider, IBaseEditOwner {
		const int EscCode = 27;
		protected enum BaseEditSourceType { EditSettings, CellTemplate }
		static InplaceEditorBase() {
#if !SL
			FocusableProperty.OverrideMetadata(typeof(InplaceEditorBase), new FrameworkPropertyMetadata(true));
#endif
		}
		public static bool CloseEditorOnLostKeyboardFocus;
		public virtual bool IsEditorVisible { get { return Edit.EditMode == EditMode.InplaceActive; } }
		protected BaseEditSourceType EditorSourceType { get; set; }
		public IBaseEditWrapper Edit { get { return editWrapper ?? FakeBaseEdit.Instance; } }
		protected IBaseEdit editCore;
		protected IBaseEditWrapper editWrapper;
		protected internal object EditableValue { get { return Edit.EditValue; } set { Edit.EditValue = value; } }
		protected abstract InplaceEditorOwnerBase Owner { get; }
		protected abstract IInplaceEditorColumn EditorColumn { get; }
		protected internal virtual bool IsInTree { get { return Owner != null; } }
		protected bool HasAccessToCellValue { get { return IsInTree && EditorColumn != null; } }
		internal Locker CommitEditorLocker { get { return Owner.CommitEditorLocker; } }
		protected abstract bool IsCellFocused { get; }
		protected abstract bool IsReadOnly { get; }
		ColumnContentChangedEventHandler<InplaceEditorBase> ColumnContentChangedEventHandler { get; set; }
		InnerContentChangedEventHandler<InplaceEditorBase> InnerContentChangedEventHandler { get; set; } 
		EditableDataObject editorDataContext;
		protected virtual bool OptimizeEditorPerformance { get { return false; } }
		protected EditableDataObject EditorDataContext {
			get { return editorDataContext; }
			set {
				if(EditorDataContext == value)
					return;
				if(EditorDataContext != null) {
					EditorDataContext.ContentChanged -= InnerContentChangedEventHandler.Handler;
					NullEditorInEditorDataContext();
				}
				editorDataContext = value;
				if(EditorDataContext != null) {
					EditorDataContext.ContentChanged += InnerContentChangedEventHandler.Handler;
					SetEditorInEditorDataContext();
				}
			}
		}
		protected virtual void NullEditorInEditorDataContext() { 
		}
		protected virtual void SetEditorInEditorDataContext() {
		}
		protected abstract bool OverrideCellTemplate { get; }
		internal void LockEditorFocus() { Edit.LockEditorFocus(); }
		internal void UnlockEditorFocus() { Edit.UnlockEditorFocus(); }
		protected InplaceEditorBase() {
			ColumnContentChangedEventHandler = new ColumnContentChangedEventHandler<InplaceEditorBase>(this, (owner, o, e) => owner.OnColumnContentChanged(o, e), (h, e) => ((IInplaceEditorColumn)e).ContentChanged -= h.Handler);
			InnerContentChangedEventHandler = new InnerContentChangedEventHandler<InplaceEditorBase>(this, (owner, o, e) => owner.OnInnerContentChanged(o, e));
		}
		protected virtual void OnEditValueChanged(object sender, EditValueChangedEventArgs e) { }
		protected virtual bool RaiseShowingEditor() { return true; }
		protected virtual void OnShowEditor() { }
		protected virtual bool HasEditorError() { return false; }
		protected virtual void OnEditorActivated(object sender, RoutedEventArgs e) { }
		protected virtual DataTemplate SelectTemplate() { return null; }
		protected virtual void RestoreValidationError() { }
		protected virtual void CancelRowEdit(KeyEventArgs e) { }
		protected virtual void UpdateValidationError() { }
		protected abstract bool IsInactiveEditorButtonVisible();
		protected abstract object GetEditableValue();
		public virtual void SetEditableValueFromExternalEditor(object value) {
			EditableValue = value;
			BaseEditHelper.SetIsValueChanged(this.editCore, true);
		}
		public virtual object GetEditableValueForExternalEditor() {
			return GetEditableValue();
		}
		public bool PostEditor() {
			if(Edit.IsValueChanged) {
				ValidateEditor();
				return PostEditorCore();
			}
			return true;
		}
		protected abstract bool PostEditorCore();
		protected void UpdateEditValue(IBaseEdit editor) {
			if(editor == null)
				return;
			UpdateEditValueCore(editor);
		}
		protected abstract void UpdateEditValueCore(IBaseEdit editor);
		protected abstract EditableDataObject GetEditorDataContext();
		public virtual bool CanShowEditor() {
			return IsCellFocused;
		}
		public void ShowEditorIfNotVisible(bool selectAll) {
			if(!IsEditorVisible)
				ShowEditor(selectAll);
		}
		public void ClearError() {
			Edit.ClearEditorError();
		}
		internal void ShowEditorAndSelectAll() {
			ShowEditor(true);
		}
		public void ShowEditor() {
			ShowEditor(false);
		}
		internal bool ShowEditor(bool selectAll) {
			if(!CanShowEditor())
				return false;
			return ShowEditorCore(selectAll);
		}
		protected virtual bool ShowEditorCore(bool selectAll) {
			return Owner.ShowEditor(selectAll);
		}
		protected internal virtual bool ShowEditorInternal(bool selectAll) {
			if (!IsEditorVisible) {
				UpdateEditTemplate();
				if(!IsReadOnlyHasBindingExpression())
					Edit.IsReadOnly = IsReadOnly;
				SetActiveEditMode();
				Owner.ActiveEditor = editCore;
				Edit.SetKeyboardFocus();
				UpdateEditContext();
				Edit.EditValueChanged += new EditValueChangedEventHandler(OnEditValueChanged);
			}
			OnShowEditor();
			Owner.EditorWasClosed = false;
			UpdateEditorButtonVisibility();
			Edit.IsValueChanged = false;
			if (selectAll)
				Owner.EnqueueImmediateAction(new SelectAllAction(this));
			return true;
		}
		protected bool IsReadOnlyHasBindingExpression() {			
			BaseEdit baseEdit = editCore as BaseEdit;
			return baseEdit != null && System.Windows.Data.BindingOperations.GetBindingExpression(baseEdit, BaseEdit.IsReadOnlyProperty) != null;		   
		}
		protected virtual void SetActiveEditMode() {
			Edit.EditMode = EditMode.InplaceActive;
			UpdateEditValue(editCore);
		}
		protected virtual void UpdateEditTemplate() {
			Edit.SetEditTemplate(EditorColumn.EditTemplate);
		}
		protected virtual void UpdateDisplayTemplate(bool updateForce = false) {
			if(editCore != null) {
				if(EditorSourceType == BaseEditSourceType.EditSettings)
					editCore.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = OptimizeEditorPerformance && EditorColumn.DisplayTemplate == null;
			}
			if(!updateForce && EditorColumn.DisplayTemplate == null)
				return;
			Edit.SetDisplayTemplate(EditorColumn.DisplayTemplate);
		}
		protected virtual void UpdateEditContext() {
			if(HasAccessToCellValue)
				EditableValue = GetEditableValue();
		}
		public virtual void UpdateEditorButtonVisibility() {
			if(!IsInTree)
				return;
			Edit.ShowEditorButtons = IsEditorVisible || IsInactiveEditorButtonVisible();
		}
		protected internal TextHighlightingProperties LastHighlightingProperties { get; set; }
		public void UpdateHighlightingText(TextHighlightingProperties highlightingProperties, bool skipEqualsCheck) {
			if(LastHighlightingProperties == null && highlightingProperties == null)
				return;
			if(LastHighlightingProperties == null || highlightingProperties == null) {
				UpdateHighlightingTextInternal(highlightingProperties);
				return;
			}
			if(!skipEqualsCheck && (LastHighlightingProperties.FilterCondition == highlightingProperties.FilterCondition) && (LastHighlightingProperties.Text == highlightingProperties.Text))
				return;
			UpdateHighlightingTextInternal(highlightingProperties);
		}
#if DEBUGTEST
		public static int Debug_HighlightingTextUpdatesCount = 0;
#endif
		void UpdateHighlightingTextInternal(TextHighlightingProperties highlightingProperties) {
			LastHighlightingProperties = highlightingProperties;
			if(editCore == null)
				return;
#if DEBUGTEST
			Debug_HighlightingTextUpdatesCount++;
#endif
			if(EditorSourceType == BaseEditSourceType.CellTemplate)
				BaseEditHelper.UpdateHighlightingText(editCore, highlightingProperties);
			else
				SearchControlHelper.UpdateTextHighlighting(editCore.Settings, highlightingProperties);
		}
		public void UpdateDisplayText() {
			BaseEdit edit = this.editCore as BaseEdit;
			if(edit == null) return;
			edit.EditStrategy.UpdateDisplayText();
		}
		internal void SelectAll() {
			Edit.SelectAll();
		}
		internal void ProcessActivatingKey(KeyEventArgs e) {
			Edit.ProcessActivatingKey(e);
		}
		public void HideEditor(bool closeEditor) {
			if(IsEditorVisible) {
				Edit.EditMode = EditMode.InplaceInactive;
				Edit.EditValueChanged -= OnEditValueChanged;
				UpdateEditValue(editCore);
				if(Owner.ActiveEditor == editCore)
					Owner.ActiveEditor = null;
				OnHiddenEditor(closeEditor);
			}
			if(closeEditor) Owner.EditorWasClosed = true;
		}
		protected virtual void OnHiddenEditor(bool closeEditor) {
			UpdateEditorButtonVisibility();
		}
		const string tab = "\t";
		protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
			base.OnPreviewTextInput(e);
			if(e.Text == tab)
				return;
#if !SL
			if(string.IsNullOrEmpty(e.Text) || !string.IsNullOrEmpty(e.ControlText) || !string.IsNullOrEmpty(e.SystemText) || (int)e.Text[0] == EscCode)
				return;
			if(!IsEditorVisible)
				ShowEditorAndSelectAll();
			if(IsEditorVisible && !Edit.IsEditorActive) {
				Owner.EnqueueImmediateAction(new RaisePostponedTextInputEventAction(this, e));
				e.Handled = true;
			}
#else
#endif
		}
		protected virtual bool ShouldProcessMouseEventsInInplaceInactiveMode(MouseButtonEventArgs e) {
			return false;
		}
		internal void ActivateOnLeftMouseButton(MouseButtonEventArgs e, bool isMouseDown) {
			if(IsCellFocused) {
				if(!Edit.IsEditorActive) {
					if (ShouldProcessMouseEventsInInplaceInactiveMode(e)) 
						return;
					ShowEditorIfNotVisible(!isMouseDown);
					if(ReraiseMouseEventEditor != null && ReraiseMouseEventEditor.IsEditorVisible && isMouseDown) {
						RaisePostponedMouseEvent(e);
						e.Handled = true;
					}
				}
#if SL //B212391
				if(e.OriginalSource is Border && ((Border)e.OriginalSource).Child != null)
					e.Handled = true;				
#endif
			}
		}
		protected virtual InplaceEditorBase ReraiseMouseEventEditor { get { return this; } }
		protected virtual void ProcessMouseEventInInplaceInactiveMode(MouseButtonEventArgs e) {
			RaisePostponedMouseEvent(e);
		}
		private void RaisePostponedMouseEvent(MouseButtonEventArgs e) {
#if !SL
			Owner.EnqueueImmediateAction(new RaisePostponedMouseEventAction(ReraiseMouseEventEditor, e));
#endif
		}
		internal void ActivateOnStylusUp() {
			if(IsCellFocused && !Edit.IsEditorActive) {
				ShowEditorIfNotVisible(true);
			}
		}
		public virtual void CancelEditInVisibleEditor() {
			if(!IsEditorVisible)
				return;
			Edit.ClearEditorError();
			RestoreValidationError();
			HideEditor(true);
		}
		public void CommitEditor(bool closeEditor = false) {
			CommitEditorLocker.DoLockedActionIfNotLocked(() => CommitEditorCore(closeEditor));
		}
		void CommitEditorCore(bool closeEditor) {
			if(!IsEditorVisible)
				return;
			if(PostEditor())
				HideEditor(closeEditor);
		}
		protected internal virtual bool ProcessKeyForLookUp(KeyEventArgs e) {
			return IsEditorVisible && (Edit.IsActivatingKey(e) || !Edit.NeedsKey(e));
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if(!IsEditorVisible && Edit.IsActivatingKey(e)) {
				ShowEditorAndSelectAll();
				if(IsEditorVisible) {
					Owner.EnqueueImmediateAction(new ProcessActivatingKeyAction(this, e));
					e.Handled = true;
				} else {
					RaiseKeyDownEvent(e);
				}
				return;
			}
			if(!Edit.NeedsKey(e)) {
				if(e.Key == Key.Enter) {
					if(IsEditorVisible) {
						CommitEditor(true);
						CheckFocus();
					} else {
						ShowEditorAndSelectAll();
					}
					e.Handled = true;
				}
				if(e.Key == Key.F2) {
					if(!IsEditorVisible) {
						ShowEditorAndSelectAll();
						e.Handled = true;
					}
				}
				if(e.Key == Key.Escape) {
					if(IsEditorVisible) {
						CancelEditInVisibleEditor();
						CheckFocus();
						e.Handled = true;
					} else
						CancelRowEdit(e);
				}
				if(!e.Handled) {
					RaiseKeyDownEvent(e);
				}
			}
		}
		protected virtual void CheckFocus() { }
		void RaiseKeyDownEvent(KeyEventArgs e) {
			if(e.Key != Key.Escape)
				e.Handled = true;
			Owner.ProcessKeyDown(e);
		}
		protected virtual void OnEditorPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if(!IsInTree || IsChildElementOrMessageBox((IInputElement)e.NewFocus)) {
				return;
			}
			PopupBaseEdit popupEdit = editCore as PopupBaseEdit;
			if(popupEdit != null && popupEdit.IsPopupCloseInProgress)
				return;
			if(!CloseEditorOnLostKeyboardFocus) {
				DependencyObject thisScope = FocusManager.GetFocusScope(this);
				if(thisScope is UIElement && ((UIElement)thisScope).IsVisible && FocusManager.GetFocusScope((DependencyObject)e.NewFocus) != thisScope) {
					UIElement scopeFocus = (UIElement)thisScope;
					scopeFocus.PreviewGotKeyboardFocus += scopeFocus_PreviewGotKeyboardFocus;
					this.IsVisibleChanged += InplaceEditorBase_IsVisibleChanged;
					return;
				}
			}
			LockEditorFocus();
			if(IsEditorVisible && Edit.IsEditorActive) {
				CommitEditor();
			}
			UnlockEditorFocus();
			if(HasEditorError())
				e.Handled = true;
		}
		void InplaceEditorBase_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UnsubscribingFocusScope();
			if(Owner != null)
				Owner.CommitEditorForce();
		}
		void UnsubscribingFocusScope() {
			UIElement scopeFocus = FocusManager.GetFocusScope(this) as UIElement;
			if(scopeFocus != null) {
				scopeFocus.PreviewGotKeyboardFocus -= scopeFocus_PreviewGotKeyboardFocus;
				this.IsVisibleChanged -= InplaceEditorBase_IsVisibleChanged;
			}
		}
		void scopeFocus_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if(FocusManager.GetFocusScope((DependencyObject)e.NewFocus) != FocusManager.GetFocusScope(this))
				return;
			UnsubscribingFocusScope();
			DependencyObject focusObject = e.Source as DependencyObject;
			if(!this.IsKeyboardFocusWithin) {
				CommitEditor();
				if(Owner.ActiveEditor != null && Owner.ActiveEditor.ValidationError != null) {
					e.Handled = true;
					Keyboard.Focus(FocusManager.GetFocusedElement(FocusManager.GetFocusScope(this)));
				}
			}
		}
		protected virtual void SetEdit(IBaseEdit value) {
			if(editCore == value)
				return;
			if(editCore != null) {
				editCore.EditorActivated -= new RoutedEventHandler(OnEditorActivated);
				editCore.PreviewLostKeyboardFocus -= OnEditorPreviewLostKeyboardFocus;
			}
			editCore = value;
			editWrapper = editCore != null ? new BaseEditWrapper(editCore) : null;
			if(editCore != null) {
				editCore.EditorActivated += new RoutedEventHandler(OnEditorActivated);
				editCore.PreviewLostKeyboardFocus += OnEditorPreviewLostKeyboardFocus;
			}
		}
		protected virtual void UpdateContent(bool updateDisplayTemplate = true) {
			if(!HasAccessToCellValue)
				return;
			DataTemplate template = SelectTemplate();
			if(template == null || OverrideCellTemplate) {
				if(editCore == null || EditorSourceType == BaseEditSourceType.CellTemplate || !IsProperEditorSettings()) {
					if(editCore != null)
						editCore.DataContext = null;
					IBaseEdit newEdit = CreateEditor(EditorColumn.EditSettings);
					UpdateEditorDataContext();
					newEdit.DataContext = EditorDataContext;
					InitializeBaseEdit(newEdit, BaseEditSourceType.EditSettings);
					ContentTemplate = null;
					Content = newEdit;
				} else {
					if(!object.ReferenceEquals(BaseEditHelper.GetEditSettings(editCore), EditorColumn.EditSettings)) {
						editCore.BeginInit();
						EditorColumn.EditSettings.ApplyToEdit(editCore, true, EditorColumn, true);
						UpdateEditorDataContext();
						if(editCore != null) {
							editCore.DataContext = EditorDataContext;
							editCore.EndInit();
						}
					} else {
						EditorColumn.EditSettings.AssignViewInfoProperties(editCore, EditorColumn);
					}
				}
			} else {
				Content = EditorDataContext;
				if(EditorSourceType == BaseEditSourceType.EditSettings || ContentTemplate != template)
					SetEdit(null);
				ContentTemplate = template;
			}
			if(updateDisplayTemplate)
				UpdateDisplayTemplate(true);
		}
		protected virtual void UpdateEditorDataContext() { }
		bool IsSameDataTemplateSelectors(ActualTemplateSelectorWrapper selector1, ActualTemplateSelectorWrapper selector2) {
			if(selector1 == null || selector2 == null)
				return false;
			return selector1.Template == selector2.Template && selector1.Selector == selector2.Selector;
		}
		protected virtual bool IsProperEditorSettings() {
			return object.ReferenceEquals(BaseEditHelper.GetEditSettings(editCore), EditorColumn.EditSettings);
		}
		protected virtual IBaseEdit CreateEditor(BaseEditSettings settings) {
			return settings.CreateEditor(EditorColumn, GetEditorOptimizationMode());
		}
		protected virtual EditorOptimizationMode GetEditorOptimizationMode() {
			return EditorOptimizationMode.Disabled;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
#if !SL
			IBaseEdit editFromTemplate = GetTemplateChild("PART_Editor") as IBaseEdit;
			InitializeEditorFromTemplate(editFromTemplate);
#endif
			if(IsCellFocused) {
				RefreshFocus();
			}
		}
		internal void InitializeEditorFromTemplate(IBaseEdit editFromTemplate) {
			if(editFromTemplate != null) {
				editFromTemplate.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = false;
				InitializeBaseEdit(editFromTemplate, BaseEditSourceType.CellTemplate);
				BaseEditHelper.ApplySettings(editFromTemplate, EditorColumn.EditSettings, EditorColumn);
				UpdateHighlightingText(LastHighlightingProperties, true);
			} else if(EditorSourceType == BaseEditSourceType.CellTemplate)
				SetEdit(null);
		}
		protected virtual void InitializeBaseEdit(IBaseEdit newEdit, BaseEditSourceType newBaseEditSourceType) {
			this.EditorSourceType = newBaseEditSourceType;
			newEdit.EditMode = EditMode.InplaceInactive;
			SetEdit(newEdit);
			UpdateEditValue(newEdit);
			newEdit.InvalidValueBehavior = InvalidValueBehavior.AllowLeaveEditor;
			UpdateEditorButtonVisibility();
			UpdateDisplayTemplate();
			UpdateValidationError();
			UpdateValidationErrorTemplate(newEdit.Settings.Return(x => x.ValidationErrorTemplate, () => null));
			SetDisplayTextProvider(newEdit);
#if SL
			((BaseEdit)editCore).InplaceOwner2 = Owner;
#endif
		}
		void UpdateValidationErrorTemplate(DataTemplate dataTemplate) {
			Edit.SetValidationErrorTemplate(dataTemplate);
		}
		protected virtual void SetDisplayTextProvider(IBaseEdit newEdit) {
			BaseEditHelper.SetDisplayTextProvider(newEdit, this);
		}
		protected void OnIsFocusedCellChanged() {
			if(IsCellFocused) {
				RefreshFocus();
			} else {
				if(IsEditorVisible)
					CommitEditor();
				if(IsInTree) {
					if(this.GetIsKeyboardFocusWithin())
						KeyboardHelper.Focus(Owner.OwnerElement);
					ClearViewCurrentCellEditor(Owner);
				}
			}
			UpdateEditorButtonVisibility();
		}
		void RefreshFocus() {
			if(!IsInTree)
				return;
			if(Owner.CanFocusEditor) {
				SetKeyboardFocus();
			}
			UpdateEditorToShow();
		}
#if DEBUGTEST
		public static bool UpdateLayoutAfterSetKeyboardFocus = false;
#endif
		internal void SetKeyboardFocus() {
			Edit.SetKeyboardFocus();
			if(!this.GetIsKeyboardFocusWithin())
				KeyboardHelper.Focus(this);
#if DEBUGTEST
			if(UpdateLayoutAfterSetKeyboardFocus)
				UpdateLayout();
#endif
		}
		void UpdateEditorToShow() {
			if(IsCellFocused) {
				Owner.CurrentCellEditor = this;
			}
		}
		protected void ClearViewCurrentCellEditor(InplaceEditorOwnerBase owner) {
			if(owner.CurrentCellEditor == this)
				owner.CurrentCellEditor = null;
		}
		protected void OnOwnerChanged(IInplaceEditorColumn oldValue) {
			UpdateData();
			if(oldValue != null) 
				oldValue.ContentChanged -= ColumnContentChangedEventHandler.Handler;
			if(EditorColumn != null) {
				EditorColumn.ContentChanged += ColumnContentChangedEventHandler.Handler;
				UpdateContent();
			}
		}
		protected void UpdateData() {
			if(HasAccessToCellValue) {
				EditorDataContext = GetEditorDataContext();
			} else {
				EditorDataContext = null;
				Content = null;
			}
		}
#if DEBUGTEST
		public int InnerContentChangedFireCount { get; set; }
#endif
		protected override void OnInnerContentChangedCore() {
			if(!IsEditorVisible) {
				base.OnInnerContentChangedCore();
#if DEBUGTEST
				InnerContentChangedFireCount++;
#endif
				if(!IsInTree)
					return;
				UpdateContent(false);
				UpdateEditorButtonVisibility();
			} 
			UpdateValidationError();
		}
		protected virtual void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			if(e.Property == null)
				UpdateContent();
		}
		protected internal bool IsChildElementOrMessageBox(IInputElement element) {
			return Edit.IsChildElement(element, EditorRoot) || IsMessageBox(element);
		}
		protected virtual DependencyObject EditorRoot { get { return null; } }
		bool IsMessageBox(IInputElement focus) {
#if !SL     
			return BrowserInteropHelper.IsBrowserHosted ? IsMessageBoxCoreXBAP(focus) : IsMessageBoxCore(focus);
#else
			return IsMessageBoxCore(focus);
#endif
		}
#if !SL 
		bool IsMessageBoxCore(IInputElement focus) {
			WindowContentHolder window = focus as WindowContentHolder;
			if(window == null) return false;
			FloatingContainer container = window.Container as FloatingContainer;
			if(container == null) return false;
			return container.ShowModal;
		}
		bool IsMessageBoxCoreXBAP(IInputElement focus) {
			return focus is DXMessageBox;
		}
#else
		bool IsMessageBoxCore(IInputElement focus) {
			DependencyObject newFocus = focus as DependencyObject;
			if(newFocus == null) return false;
			DependencyObject root = LayoutHelper.FindVisualRoot(newFocus);
			return root is DXDialog && root != LayoutHelper.FindVisualRoot(this);
		}
#endif
		public virtual void ValidateEditor() {
			if(IsEditorVisible) {
				Edit.FlushPendingEditActions();
				ValidateEditorCore();
			}
		}
		public virtual void ValidateEditorCore() {
		}
		#region IDisplayTextProvider Members
		bool? IDisplayTextProvider.GetDisplayText(string originalDisplayText, object value, out string displayText) {
			if(!IsInTree) {
				displayText = originalDisplayText;
				return true;
			}
			return Owner.GetDisplayText(this, originalDisplayText, value, out displayText);
		}
		#endregion
		#region IBaseEditOwner Members
		HorizontalAlignment IBaseEditOwner.DefaultHorizontalAlignment {
			get { return EditorColumn != null ? EditorColumn.DefaultHorizontalAlignment : HorizontalAlignment.Left; }
		}
		bool IBaseEditOwner.HasTextDecorations {
			get { return EditorColumn != null && EditorColumn.HasTextDecorations; }
		}
		bool? IBaseEditOwner.AllowDefaultButton {
			get { return GetAllowDefaultButton(); }
		}
		IDisplayTextProvider IBaseEditOwner.DisplayTextProvider {
			get { return this; }
		}
		protected virtual bool? GetAllowDefaultButton() {
			return null;
		}
		#endregion
	}
	public interface IInplaceEditorColumn : IDefaultEditorViewInfo {
		BaseEditSettings EditSettings { get; }
		DataTemplateSelector EditorTemplateSelector { get; }
		ControlTemplate EditTemplate { get; }
		ControlTemplate DisplayTemplate { get; }
		event ColumnContentChangedEventHandler ContentChanged;
	}
	public interface IBaseEditOwner {
		HorizontalAlignment DefaultHorizontalAlignment { get; }
		bool HasTextDecorations { get; }
		bool? AllowDefaultButton { get; }
		IDisplayTextProvider DisplayTextProvider { get; }
	}
	public interface IAction {
		void Execute();
	}
	public interface IAggregateAction : IAction {
		bool CanAggregate(IAction action);
	}
	public sealed class ImmediateActionsManager {
		readonly FrameworkElement owner;
		public ImmediateActionsManager(FrameworkElement owner = null) {
			this.owner = owner;
		}
		public int Count { get { return actionsQueue.Count; } }
		readonly Queue<IAction> actionsQueue = new Queue<IAction>();
		readonly Queue<IAction> tempQueue = new Queue<IAction>();
		readonly Locker executeLocker = new Locker();
		public void ExecuteActions() {
			if(actionsQueue.Count == 0 || executeLocker.IsLocked)
				return;
			LogBase.Add(owner,  actionsQueue.Count);
			executeLocker.DoLockedAction(() => {
				IAction lastQueue = actionsQueue.Last();
				while (true) {
					IAction action = actionsQueue.Dequeue();
					action.Execute();
					if (action == lastQueue)
						return;
				}
			});
			if (tempQueue.Count > 0) {
				tempQueue.ForEach(EnqueueActionInternal);
				tempQueue.Clear();
				if (owner != null)
					owner.InvalidateMeasure();
			}
		}
		public void EnqueueAction(IAction action) {
			LogBase.Add(owner, null);
			if (executeLocker.IsLocked) {
				tempQueue.Enqueue(action);
				return;
			}
			EnqueueActionInternal(action);
			if(owner != null)
				owner.InvalidateMeasure();
		}
		void EnqueueActionInternal(IAction action) {
			var aggregateAction = action as IAggregateAction;
			if (aggregateAction != null) {
				var queue = new Queue<IAction>();
				while (actionsQueue.Count > 0) {
					IAction current = actionsQueue.Dequeue();
					if (!aggregateAction.CanAggregate(current))
						queue.Enqueue(current);
				}
				queue.ForEach(x => actionsQueue.Enqueue(x));
			}
			actionsQueue.Enqueue(action);
		}
		public void EnqueueAction(Action action) {
			EnqueueAction(new DelegateAction(action));
		}
		public IAction FindActionOfType(Type actionType) {
			return FindAction(actionType.IsInstanceOfType);			
		}
		public IAction FindAction(Predicate<IAction> predicate) {
			return FindAction(actionsQueue, predicate) ?? FindAction(tempQueue, predicate);
		}
		IAction FindAction(Queue<IAction> queue, Predicate<IAction> predicate) {
			foreach(IAction action in queue) {
				if(predicate(action))
					return action;
			}
			return null;
		}
	}
	public class DelegateAction : IAction {
		readonly Action action;
		public DelegateAction(Action action) {
			this.action = action;
		}
		void IAction.Execute() {
			action();
		}
	}
}
