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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public abstract class InplaceEditorBase2 : DataContentPresenter, IDisplayTextProvider, IBaseEditOwner {
		const int EscCode = 27;
		const string tab = "\t";
		public static bool CloseEditorOnLostKeyboardFocus;
		public virtual bool IsEditorVisible {
			get { return Edit.EditMode == EditMode.InplaceActive; }
		}
		protected BaseEditSourceType EditorSourceType { get; set; }
		public IBaseEditWrapper Edit {
			get { return editWrapper ?? FakeBaseEdit.Instance; }
		}
		protected internal object EditableValue {
			get { return Edit.EditValue; }
			set { Edit.EditValue = value; }
		}
		protected abstract InplaceEditorOwnerBase2 Owner { get; }
		protected abstract IInplaceEditorColumn EditorColumn { get; }
		protected internal virtual bool IsInTree {
			get { return Owner != null; }
		}
		protected bool HasAccessToCellValue {
			get { return IsInTree && EditorColumn != null; }
		}
		internal Locker CommitEditorLocker {
			get { return Owner.CommitEditorLocker; }
		}
		protected abstract bool IsCellFocused { get; }
		protected abstract bool IsReadOnly { get; }
		ColumnContentChangedEventHandler<InplaceEditorBase2> ColumnContentChangedEventHandler { get; set; }
		InnerContentChangedEventHandler<InplaceEditorBase2> InnerContentChangedEventHandler { get; set; }
		protected virtual bool OptimizeEditorPerformance {
			get { return false; }
		}
		protected EditableDataObject EditorDataContext {
			get { return editorDataContext; }
			set {
				if (EditorDataContext == value)
					return;
				if (EditorDataContext != null) {
					EditorDataContext.ContentChanged -= InnerContentChangedEventHandler.Handler;
					NullEditorInEditorDataContext();
				}
				editorDataContext = value;
				if (EditorDataContext != null) {
					EditorDataContext.ContentChanged += InnerContentChangedEventHandler.Handler;
					SetEditorInEditorDataContext();
				}
			}
		}
		protected abstract bool OverrideCellTemplate { get; }
		protected internal TextHighlightingProperties LastHighlightingProperties { get; set; }
		protected virtual InplaceEditorBase2 ReraiseMouseEventEditor {
			get { return this; }
		}
#if DEBUGTEST
		public int InnerContentChangedFireCount { get; set; }
#endif
		protected virtual DependencyObject EditorRoot {
			get { return null; }
		}
		protected IBaseEdit editCore;
		EditableDataObject editorDataContext;
		protected IBaseEditWrapper editWrapper;
		static InplaceEditorBase2() {
			FocusableProperty.OverrideMetadata(typeof(InplaceEditorBase2), new FrameworkPropertyMetadata(true));
		}
		protected InplaceEditorBase2() {
			ColumnContentChangedEventHandler = new ColumnContentChangedEventHandler<InplaceEditorBase2>(this, (owner, o, e) => owner.OnColumnContentChanged(o, e),
				(h, e) => ((IInplaceEditorColumn)e).ContentChanged -= h.Handler);
			InnerContentChangedEventHandler = new InnerContentChangedEventHandler<InplaceEditorBase2>(this, (owner, o, e) => owner.OnInnerContentChanged(o, e));
		}
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
		bool? IDisplayTextProvider.GetDisplayText(string originalDisplayText, object value, out string displayText) {
			if (!IsInTree) {
				displayText = originalDisplayText;
				return true;
			}
			return Owner.GetDisplayText(this, originalDisplayText, value, out displayText);
		}
		protected virtual void NullEditorInEditorDataContext() {
		}
		protected virtual void SetEditorInEditorDataContext() {
		}
		internal void LockEditorFocus() {
			Edit.LockEditorFocus();
		}
		internal void UnlockEditorFocus() {
			Edit.UnlockEditorFocus();
		}
		protected virtual void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
		}
		protected virtual bool RaiseShowingEditor() {
			return true;
		}
		protected virtual void OnShowEditor() {
		}
		protected virtual bool HasEditorError() {
			return false;
		}
		protected virtual void OnEditorActivated(object sender, RoutedEventArgs e) {
		}
		protected virtual DataTemplate SelectTemplate() {
			return null;
		}
		protected virtual void RestoreValidationError() {
		}
		protected virtual void CancelRowEdit(KeyEventArgs e) {
		}
		protected virtual void UpdateValidationError() {
		}
		protected abstract bool IsInactiveEditorButtonVisible();
		protected abstract object GetEditableValue();
		public bool PostEditor() {
			if (Edit.IsValueChanged) {
				ValidateEditor();
				return PostEditorCore();
			}
			return true;
		}
		protected abstract bool PostEditorCore();
		protected void UpdateEditValue(IBaseEdit editor) {
			if (editor == null)
				return;
			UpdateEditValueCore(editor);
		}
		protected abstract void UpdateEditValueCore(IBaseEdit editor);
		protected abstract EditableDataObject GetEditorDataContext();
		public virtual bool CanShowEditor() {
			return IsCellFocused;
		}
		public void ShowEditorIfNotVisible(bool selectAll) {
			if (!IsEditorVisible)
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
			if (!CanShowEditor())
				return false;
			return ShowEditorCore(selectAll);
		}
		protected virtual bool ShowEditorCore(bool selectAll) {
			return Owner.ShowEditor(selectAll);
		}
		protected internal virtual bool ShowEditorInternal(bool selectAll) {
			if (!IsEditorVisible) {
				UpdateEditTemplate();
				Edit.IsReadOnly = IsReadOnly;
				SetActiveEditMode();
				Owner.ActiveEditor = editCore;
				Edit.SetKeyboardFocus();
				UpdateEditContext();
				Edit.EditValueChanged += OnEditValueChanged;
			}
			OnShowEditor();
			Owner.EditorWasClosed = false;
			UpdateEditorButtonVisibility();
			Edit.IsValueChanged = false;
			if (selectAll)
				Owner.EnqueueImmediateAction(new SelectAllAction2(this));
			return true;
		}
		protected virtual void SetActiveEditMode() {
			Edit.EditMode = EditMode.InplaceActive;
			UpdateEditValue(editCore);
		}
		protected virtual void UpdateEditTemplate() {
			Edit.SetEditTemplate(EditorColumn.EditTemplate);
		}
		protected virtual void UpdateDisplayTemplate(bool updateForce = false) {
			if (editCore != null) {
				if (EditorSourceType == BaseEditSourceType.EditSettings)
					editCore.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = OptimizeEditorPerformance && EditorColumn.DisplayTemplate == null;
			}
			if (!updateForce && EditorColumn.DisplayTemplate == null)
				return;
			Edit.SetDisplayTemplate(EditorColumn.DisplayTemplate);
		}
		protected virtual void UpdateEditContext() {
			if (HasAccessToCellValue)
				EditableValue = GetEditableValue();
		}
		public virtual void UpdateEditorButtonVisibility() {
			if (!IsInTree)
				return;
			Edit.ShowEditorButtons = IsEditorVisible || IsInactiveEditorButtonVisible();
		}
		public void UpdateHighlightingText(TextHighlightingProperties highlightingProperties, bool skipEqualsCheck) {
			if (LastHighlightingProperties == null && highlightingProperties == null)
				return;
			if (LastHighlightingProperties == null || highlightingProperties == null) {
				UpdateHighlightingTextInternal(highlightingProperties);
				return;
			}
			if (!skipEqualsCheck && (LastHighlightingProperties.FilterCondition == highlightingProperties.FilterCondition) && (LastHighlightingProperties.Text == highlightingProperties.Text))
				return;
			UpdateHighlightingTextInternal(highlightingProperties);
		}
		void UpdateHighlightingTextInternal(TextHighlightingProperties highlightingProperties) {
			LastHighlightingProperties = highlightingProperties;
			if (editCore == null)
				return;
			if (EditorSourceType == BaseEditSourceType.CellTemplate)
				BaseEditHelper.UpdateHighlightingText(editCore, highlightingProperties);
			else
				SearchControlHelper.UpdateTextHighlighting(editCore.Settings, highlightingProperties);
		}
		public void UpdateDisplayText() {
			BaseEdit edit = this.editCore as BaseEdit;
			if (edit == null)
				return;
			edit.EditStrategy.UpdateDisplayText();
		}
		internal void SelectAll() {
			Edit.SelectAll();
		}
		internal void ProcessActivatingKey(KeyEventArgs e) {
			Edit.ProcessActivatingKey(e);
		}
		public void HideEditor(bool closeEditor) {
			if (IsEditorVisible) {
				Edit.EditMode = EditMode.InplaceInactive;
				Edit.EditValueChanged -= OnEditValueChanged;
				UpdateEditValue(editCore);
				if (Owner.ActiveEditor == editCore)
					Owner.ActiveEditor = null;
				OnHiddenEditor(closeEditor);
			}
			if (closeEditor)
				Owner.EditorWasClosed = true;
		}
		protected virtual void OnHiddenEditor(bool closeEditor) {
			UpdateEditorButtonVisibility();
		}
		protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
			base.OnPreviewTextInput(e);
			if (e.Text == tab)
				return;
			if (string.IsNullOrEmpty(e.Text) || !string.IsNullOrEmpty(e.ControlText) || !string.IsNullOrEmpty(e.SystemText) || e.Text[0] == EscCode)
				return;
			if (!IsEditorVisible)
				ShowEditorAndSelectAll();
			if (IsEditorVisible && !Edit.IsEditorActive) {
				Owner.EnqueueImmediateAction(new RaisePostponedTextInputEventAction2(this, e));
				e.Handled = true;
			}
		}
		protected virtual bool ShouldProcessMouseEventsInInplaceInactiveMode(MouseButtonEventArgs e) {
			return false;
		}
		internal void ActivateOnLeftMouseButton(MouseButtonEventArgs e, bool isMouseDown) {
			if (IsCellFocused) {
				if (!Edit.IsEditorActive) {
					if (ShouldProcessMouseEventsInInplaceInactiveMode(e))
						return;
					ShowEditorIfNotVisible(!isMouseDown);
					if (ReraiseMouseEventEditor != null && ReraiseMouseEventEditor.IsEditorVisible && isMouseDown) {
						RaisePostponedMouseEvent(e);
						e.Handled = true;
					}
				}
			}
		}
		protected virtual void ProcessMouseEventInInplaceInactiveMode(MouseButtonEventArgs e) {
			RaisePostponedMouseEvent(e);
		}
		void RaisePostponedMouseEvent(MouseButtonEventArgs e) {
			Owner.EnqueueImmediateAction(new RaisePostponedMouseEventAction2(ReraiseMouseEventEditor, e));
		}
		internal void ActivateOnStylusUp() {
			if (IsCellFocused && !Edit.IsEditorActive) {
				ShowEditorIfNotVisible(true);
			}
		}
		public virtual void CancelEditInVisibleEditor() {
			if (!IsEditorVisible)
				return;
			Edit.ClearEditorError();
			RestoreValidationError();
			HideEditor(true);
		}
		public void CommitEditor(bool closeEditor = false) {
			CommitEditorLocker.DoLockedActionIfNotLocked(() => CommitEditorCore(closeEditor));
		}
		void CommitEditorCore(bool closeEditor) {
			if (!IsEditorVisible)
				return;
			if (PostEditor())
				HideEditor(closeEditor);
		}
		protected internal virtual bool ProcessKeyForLookUp(KeyEventArgs e) {
			return IsEditorVisible && (Edit.IsActivatingKey(e) || !Edit.NeedsKey(e));
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (!IsEditorVisible && Edit.IsActivatingKey(e)) {
				ShowEditorAndSelectAll();
				if (IsEditorVisible) {
					Owner.EnqueueImmediateAction(new ProcessActivatingKeyAction2(this, e));
					e.Handled = true;
				}
				else {
					RaiseKeyDownEvent(e);
				}
				return;
			}
			if (!Edit.NeedsKey(e)) {
				if (e.Key == Key.Enter) {
					if (IsEditorVisible) {
						CommitEditor(true);
						CheckFocus();
					}
					else {
						ShowEditorAndSelectAll();
					}
					e.Handled = true;
				}
				if (e.Key == Key.F2) {
					if (!IsEditorVisible) {
						ShowEditorAndSelectAll();
						e.Handled = true;
					}
				}
				if (e.Key == Key.Escape) {
					if (IsEditorVisible) {
						CancelEditInVisibleEditor();
						CheckFocus();
						e.Handled = true;
					}
					else
						CancelRowEdit(e);
				}
				if (!e.Handled) {
					RaiseKeyDownEvent(e);
				}
			}
		}
		protected virtual void CheckFocus() {
		}
		void RaiseKeyDownEvent(KeyEventArgs e) {
			if (e.Key != Key.Escape)
				e.Handled = true;
			Owner.ProcessKeyDown(e);
		}
		protected virtual void OnEditorPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if (!IsInTree || IsChildElementOrMessageBox(e.NewFocus)) {
				return;
			}
			PopupBaseEdit popupEdit = editCore as PopupBaseEdit;
			if (popupEdit != null && popupEdit.IsPopupCloseInProgress)
				return;
			if (!CloseEditorOnLostKeyboardFocus) {
				DependencyObject thisScope = FocusManager.GetFocusScope(this);
				if (FocusManager.GetFocusScope((DependencyObject)e.NewFocus) != thisScope) {
					UIElement scopeFocus = (UIElement)thisScope;
					scopeFocus.PreviewGotKeyboardFocus += scopeFocus_PreviewGotKeyboardFocus;
					return;
				}
			}
			LockEditorFocus();
			if (IsEditorVisible && Edit.IsEditorActive) {
				CommitEditor();
			}
			UnlockEditorFocus();
			if (HasEditorError())
				e.Handled = true;
		}
		void scopeFocus_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if (FocusManager.GetFocusScope((DependencyObject)e.NewFocus) != FocusManager.GetFocusScope(this))
				return;
			UIElement scopeFocus = FocusManager.GetFocusScope(this) as UIElement;
			if (scopeFocus != null)
				scopeFocus.PreviewGotKeyboardFocus -= scopeFocus_PreviewGotKeyboardFocus;
			DependencyObject focusObject = e.Source as DependencyObject;
			if (!this.IsKeyboardFocusWithin) {
				CommitEditor();
				if (Owner.ActiveEditor != null && Owner.ActiveEditor.ValidationError != null) {
					e.Handled = true;
					Keyboard.Focus(FocusManager.GetFocusedElement(FocusManager.GetFocusScope(this)));
				}
			}
		}
		protected virtual void SetEdit(IBaseEdit value) {
			if (editCore == value)
				return;
			if (editCore != null) {
				editCore.EditorActivated -= OnEditorActivated;
				editCore.PreviewLostKeyboardFocus -= OnEditorPreviewLostKeyboardFocus;
			}
			editCore = value;
			editWrapper = editCore != null ? new BaseEditWrapper(editCore) : null;
			if (editCore != null) {
				editCore.EditorActivated += OnEditorActivated;
				editCore.PreviewLostKeyboardFocus += OnEditorPreviewLostKeyboardFocus;
			}
		}
		protected virtual void UpdateContent(bool updateDisplayTemplate = true) {
			if (!HasAccessToCellValue)
				return;
			DataTemplate template = SelectTemplate();
			if (template == null || OverrideCellTemplate) {
				if (editCore == null || EditorSourceType == BaseEditSourceType.CellTemplate || !IsProperEditorSettings()) {
					if (editCore != null)
						editCore.DataContext = null;
					IBaseEdit newEdit = CreateEditor(EditorColumn.EditSettings);
					newEdit.DataContext = EditorDataContext;
					InitializeBaseEdit(newEdit, BaseEditSourceType.EditSettings);
					ContentTemplate = null;
					Content = newEdit;
				}
				else {
					if (!ReferenceEquals(BaseEditHelper.GetEditSettings(editCore), EditorColumn.EditSettings)) {
						editCore.BeginInit();
						EditorColumn.EditSettings.ApplyToEdit(editCore, true, EditorColumn, true);
						UpdateEditorDataContext();
						if (editCore != null) {
							editCore.DataContext = EditorDataContext;
							editCore.EndInit();
						}
					}
					else {
						EditorColumn.EditSettings.AssignViewInfoProperties(editCore, EditorColumn);
					}
				}
			}
			else {
				Content = EditorDataContext;
				if (EditorSourceType == BaseEditSourceType.EditSettings || ContentTemplate != template)
					SetEdit(null);
				ContentTemplate = template;
			}
			if (updateDisplayTemplate)
				UpdateDisplayTemplate(true);
		}
		protected virtual void UpdateEditorDataContext() {
		}
		protected virtual bool IsProperEditorSettings() {
			return EditorSettingsProvider.Default.IsCompatible(editCore, EditorColumn.EditSettings);
		}
		protected virtual IBaseEdit CreateEditor(BaseEditSettings settings) {
			return settings.CreateEditor(EditorColumn, GetEditorOptimizationMode());
		}
		protected virtual EditorOptimizationMode GetEditorOptimizationMode() {
			return EditorOptimizationMode.Disabled;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IBaseEdit editFromTemplate = GetTemplateChild("PART_Editor") as IBaseEdit;
			InitializeEditorFromTemplate(editFromTemplate);
			if (IsCellFocused) {
				RefreshFocus();
			}
		}
		internal void InitializeEditorFromTemplate(IBaseEdit editFromTemplate) {
			if (editFromTemplate != null) {
				editFromTemplate.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = false;
				InitializeBaseEdit(editFromTemplate, BaseEditSourceType.CellTemplate);
				BaseEditHelper.ApplySettings(editFromTemplate, EditorColumn.EditSettings, EditorColumn);
				UpdateHighlightingText(LastHighlightingProperties, true);
			}
			else if (EditorSourceType == BaseEditSourceType.CellTemplate)
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
			SetDisplayTextProvider(newEdit);
		}
		protected virtual void SetDisplayTextProvider(IBaseEdit newEdit) {
			BaseEditHelper.SetDisplayTextProvider(newEdit, this);
		}
		protected void OnIsFocusedCellChanged() {
			if (IsCellFocused) {
				RefreshFocus();
			}
			else {
				if (IsEditorVisible)
					CommitEditor();
				if (IsInTree) {
					if (this.GetIsKeyboardFocusWithin())
						KeyboardHelper.Focus(Owner.OwnerElement);
					ClearViewCurrentCellEditor(Owner);
				}
			}
			UpdateEditorButtonVisibility();
		}
		void RefreshFocus() {
			if (!IsInTree)
				return;
			if (Owner.CanFocusEditor) {
				SetKeyboardFocus();
			}
			UpdateEditorToShow();
		}
		internal void SetKeyboardFocus() {
			Edit.SetKeyboardFocus();
			if (!this.GetIsKeyboardFocusWithin())
				KeyboardHelper.Focus(this);
		}
		void UpdateEditorToShow() {
			if (IsCellFocused) {
				Owner.CurrentCellEditor = this;
			}
		}
		protected void ClearViewCurrentCellEditor(InplaceEditorOwnerBase2 owner) {
			if (owner.CurrentCellEditor == this)
				owner.CurrentCellEditor = null;
		}
		protected void OnOwnerChanged(IInplaceEditorColumn oldValue) {
			UpdateData();
			if (oldValue != null)
				oldValue.ContentChanged -= ColumnContentChangedEventHandler.Handler;
			if (EditorColumn != null) {
				EditorColumn.ContentChanged += ColumnContentChangedEventHandler.Handler;
				UpdateContent();
			}
		}
		protected void UpdateData() {
			if (HasAccessToCellValue) {
				EditorDataContext = GetEditorDataContext();
			}
			else {
				EditorDataContext = null;
				Content = null;
			}
		}
		protected override void OnInnerContentChangedCore() {
			if (!IsEditorVisible) {
				base.OnInnerContentChangedCore();
				if (!IsInTree)
					return;
				UpdateContent(false);
				UpdateEditorButtonVisibility();
			}
			UpdateValidationError();
		}
		protected virtual void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			if (e.Property == null)
				UpdateContent();
		}
		protected internal bool IsChildElementOrMessageBox(IInputElement element) {
			return Edit.IsChildElement(element, EditorRoot) || IsMessageBox(element);
		}
		bool IsMessageBox(IInputElement focus) {
			return BrowserInteropHelper.IsBrowserHosted ? IsMessageBoxCoreXBAP(focus) : IsMessageBoxCore(focus);
		}
		bool IsMessageBoxCore(IInputElement focus) {
			WindowContentHolder window = focus as WindowContentHolder;
			if (window == null)
				return false;
			FloatingContainer container = window.Container as FloatingContainer;
			if (container == null)
				return false;
			return container.ShowModal;
		}
		bool IsMessageBoxCoreXBAP(IInputElement focus) {
			return focus is DXMessageBox;
		}
		public virtual void ValidateEditor() {
			if (IsEditorVisible) {
				Edit.FlushPendingEditActions();
				ValidateEditorCore();
			}
		}
		public virtual void ValidateEditorCore() {
		}
		protected virtual bool? GetAllowDefaultButton() {
			return null;
		}
		protected enum BaseEditSourceType {
			EditSettings,
			CellTemplate
		}
	}
	public sealed class ImmediateActionsManager2 {
		public int Count {
			get { return actionsQueue.Count; }
		}
#if DEBUGTEST
		WeakEventHandler<ImmediateActionsManager2, EventArgs, EventHandler> ForceUpdateLayoutHandler { get; set; }
#endif
		readonly Queue<IAction> actionsQueue = new Queue<IAction>();
		readonly Locker executeLocker = new Locker();
		readonly FrameworkElement owner;
		readonly Stopwatch stopwatch = new Stopwatch();
		readonly Queue<IAction> tempQueue = new Queue<IAction>();
		readonly DispatcherTimer timer;
		bool delayed;
		public ImmediateActionsManager2(FrameworkElement owner = null) {
			this.owner = owner;
			timer = new DispatcherTimer(DispatcherPriority.Normal);
			timer.Interval = TimeSpan.FromMilliseconds(153);
#if DEBUGTEST
			ForceUpdateLayoutHandler = new WeakEventHandler<ImmediateActionsManager2, EventArgs, EventHandler>(this, (x, sender, handler) => { x.StopTimer(); InvalidateMeasure(); },
				(h, sender) => DispatcherHelper.ForceUpdateLayout -= h.Handler, h => h.OnEvent);
			DispatcherHelper.ForceUpdateLayout += ForceUpdateLayoutHandler.Handler;
#endif
		}
		void TimerOnTick(object sender, EventArgs eventArgs) {
			ProcessTimerTick();
		}
		void ProcessTimerTick() {
			StopTimer();
			InvalidateMeasure();
		}
		public void ExecuteActions() {
			if (ShouldDelay())
				return;
			if (actionsQueue.Count == 0 || executeLocker.IsLocked)
				return;
			LogBase.Add(owner, actionsQueue.Count);
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
				InvalidateMeasure();
			}
		}
		bool ShouldDelay() {
			return delayed && stopwatch.ElapsedMilliseconds < 153;
		}
		void StartTimer() {
			if (!delayed) {
				stopwatch.Restart();
				timer.Tick += TimerOnTick;
				timer.Start();
			}
			delayed = true;
		}
		void StopTimer() {
			if (delayed)
				timer.Tick -= TimerOnTick;
			stopwatch.Restart();
			timer.Stop();
			delayed = false;
		}
		void InvalidateMeasure() {
			if (ShouldDelay())
				return;
			if (owner != null)
				owner.InvalidateMeasure();
		}
		public void EnqueueAction(IAction action) {
			LogBase.Add(owner, null);
			if (executeLocker.IsLocked) {
				tempQueue.Enqueue(action);
				return;
			}
			EnqueueActionInternal(action);
			StartTimer();
			InvalidateMeasure();
		}
		void EnqueueActionInternal(IAction action) {
			IAggregateAction aggregateAction = action as IAggregateAction;
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
			return queue.FirstOrDefault(action => predicate(action));
		}
	}
	public class SelectAllAction2 : CellEditorAction2 {
		public SelectAllAction2(InplaceEditorBase2 editor) : base(editor) {
		}
		public override void Execute() {
			editor.SelectAll();
		}
	}
	public abstract class CellEditorAction2 : IAction {
		protected InplaceEditorBase2 editor;
		protected CellEditorAction2(InplaceEditorBase2 editor) {
			this.editor = editor;
		}
		public abstract void Execute();
	}
	public class ProcessActivatingKeyAction2 : CellEditorAction2 {
		readonly KeyEventArgs e;
		public ProcessActivatingKeyAction2(InplaceEditorBase2 editor, KeyEventArgs e) : base(editor) {
			this.e = e;
		}
		public override void Execute() {
			editor.ProcessActivatingKey(e);
		}
	}
	public abstract class RaisePosponedEventAction2<T> : CellEditorAction2 where T : RoutedEventArgs {
		protected abstract RoutedEvent BubblingEvent { get; }
		protected abstract RoutedEvent TunnelingEvent { get; }
		readonly T posponedEventArgs;
		protected RaisePosponedEventAction2(InplaceEditorBase2 editor, T e) : base(editor) {
			this.posponedEventArgs = e;
		}
		public override sealed void Execute() {
#if !SL
			if (!editor.IsInTree)
				return;
			ReraiseEventHelper.ReraiseEvent(posponedEventArgs, GetElement(posponedEventArgs), TunnelingEvent, BubblingEvent, CloneEventArgs);
#else
			throw new NotImplementedException();
#endif
		}
		protected abstract T CloneEventArgs(T posponedEventArgs);
		protected abstract UIElement GetElement(T posponedEventArgs);
	}
	public class RaisePostponedTextInputEventAction2 : RaisePosponedEventAction2<TextCompositionEventArgs> {
		protected override RoutedEvent BubblingEvent {
			get { return UIElement.TextInputEvent; }
		}
		protected override RoutedEvent TunnelingEvent {
			get { return UIElement.PreviewTextInputEvent; }
		}
		public RaisePostponedTextInputEventAction2(InplaceEditorBase2 editor, TextCompositionEventArgs e) : base(editor, e) {
		}
		protected override TextCompositionEventArgs CloneEventArgs(TextCompositionEventArgs posponedEventArgs) {
			return new TextCompositionEventArgs(posponedEventArgs.Device, posponedEventArgs.TextComposition);
		}
		protected override UIElement GetElement(TextCompositionEventArgs posponedEventArgs) {
			return (UIElement)FocusHelper.GetFocusedElement();
		}
	}
	public class RaisePostponedMouseEventAction2 : RaisePosponedEventAction2<MouseButtonEventArgs> {
		protected override RoutedEvent BubblingEvent {
			get { return UIElement.MouseDownEvent; }
		}
		protected override RoutedEvent TunnelingEvent {
			get { return UIElement.PreviewMouseDownEvent; }
		}
		public RaisePostponedMouseEventAction2(InplaceEditorBase2 editor, MouseButtonEventArgs e) : base(editor, e) {
		}
		public static UIElement GetMouseEventReraiseElement(UIElement sourceElement, Point position) {
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(sourceElement, position);
#if DEBUGTEST
			if (forcedHitTestResult != null) {
				hitTestResult = new PointHitTestResult(forcedHitTestResult, new Point());
			}
#endif
			if (hitTestResult == null)
				return null;
			return LayoutHelper.FindParentObject<UIElement>(hitTestResult.VisualHit);
		}
		protected override MouseButtonEventArgs CloneEventArgs(MouseButtonEventArgs posponedEventArgs) {
			return ReraiseEventHelper.CloneMouseButtonEventArgs(posponedEventArgs);
		}
		protected override UIElement GetElement(MouseButtonEventArgs posponedEventArgs) {
			return GetMouseEventReraiseElement(editor, posponedEventArgs.GetPosition(editor));
		}
#if DEBUGTEST
		internal static UIElement forcedHitTestResult;
		public static void ForceHitTestResult(UIElement hitTestResult) {
			forcedHitTestResult = hitTestResult;
		}
#endif
	}
}
