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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Input;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using IInputElement = System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class InplaceEditorOwnerBase
#if SL
		: IInplaceEditOwner
#endif
	{
		#region inner classes
		abstract class EditorShowModeStrategyBase {
			protected EditorShowModeStrategyBase() { }
			public virtual void OnAfterProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) { }
			public virtual void OnBeforeProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) { }
			public virtual void OnProcessLeftButtonUp(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) { }
#if SL
			public virtual bool BeginInplaceEditing(InplaceEditorOwnerBase inplaceEditorOwner) {
				return false;
			}
#endif
		}
		class MouseDownEditorShowModeStrategy : EditorShowModeStrategyBase {
			public static readonly MouseDownEditorShowModeStrategy Instance = new MouseDownEditorShowModeStrategy();
			private MouseDownEditorShowModeStrategy() { }
			public override void OnAfterProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				inplaceEditorOwner.ActivateOnLeftMouse(e, true);
			}
#if SL
			public override bool BeginInplaceEditing(InplaceEditorOwnerBase inplaceEditorOwner) {
				return inplaceEditorOwner.ActivateOnEditorFeedBack();
			}
#endif
		}
		class EditorNeverShowModeStrategy : EditorShowModeStrategyBase {
			public static readonly EditorNeverShowModeStrategy Instance = new EditorNeverShowModeStrategy();
			private EditorNeverShowModeStrategy() { }
			public override void OnAfterProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				inplaceEditorOwner.EditorWasClosed = true;
			}
		}
		class MouseDownFocusedEditorShowModeStrategy : EditorShowModeStrategyBase {
			public static readonly MouseDownFocusedEditorShowModeStrategy Instance = new MouseDownFocusedEditorShowModeStrategy();
			private MouseDownFocusedEditorShowModeStrategy() { }
			public override void OnBeforeProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				inplaceEditorOwner.ActivateOnLeftMouse(e, true);
			}
			public override void OnAfterProcessLeftButtonDown(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				if(inplaceEditorOwner.ActiveEditor == null)
					inplaceEditorOwner.EditorWasClosed = true;
			}
		}
		class MouseUpEditorShowModeStrategy : EditorShowModeStrategyBase {
			public static readonly MouseUpEditorShowModeStrategy Instance = new MouseUpEditorShowModeStrategy();
			private MouseUpEditorShowModeStrategy() { }
			public override void OnProcessLeftButtonUp(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				inplaceEditorOwner.ActivateEditorOnMouseUp(e);
			}
		}
		class MouseUpFocusedEditorShowModeStrategy : EditorShowModeStrategyBase {
			public static readonly MouseUpFocusedEditorShowModeStrategy Instance = new MouseUpFocusedEditorShowModeStrategy();
			private MouseUpFocusedEditorShowModeStrategy() { }
			public override void OnProcessLeftButtonUp(InplaceEditorOwnerBase inplaceEditorOwner, MouseButtonEventArgs e) {
				if(inplaceEditorOwner.CurrentCellEditor == inplaceEditorOwner.currentBeforeMouseDownEditor) {
					inplaceEditorOwner.ActivateEditorOnMouseUp(e);
				} else {
					inplaceEditorOwner.EditorWasClosed = true;
				}
			}
		}
		#endregion
		protected readonly FrameworkElement owner;
		InplaceEditorBase lastMouseDownEditor;
		InplaceEditorBase currentBeforeMouseDownEditor;
		IBaseEdit activeEditor;
		public virtual bool EditorWasClosed { get; set; }
		public InplaceEditorBase CurrentCellEditor { get; set; }
		public virtual FrameworkElement TopOwner { get { return owner; } }
		internal FrameworkElement OwnerElement { get { return owner; } }
		public Locker KeyboardLocker { get; private set; }
		public Locker CommitEditorLocker { get; private set; }
		EditorShowModeStrategyBase EditorShowModeStrategy {
			get {
				switch(EditorShowMode) {
					case EditorShowMode.Default:
						if(EditorSetInactiveAfterClick)
							return EditorNeverShowModeStrategy.Instance;
						if(UseMouseUpFocusedEditorShowModeStrategy)
							return MouseUpFocusedEditorShowModeStrategy.Instance;
						return MouseDownEditorShowModeStrategy.Instance;
					case EditorShowMode.MouseDown:
						return MouseDownEditorShowModeStrategy.Instance;
					case EditorShowMode.MouseDownFocused:
						return MouseDownFocusedEditorShowModeStrategy.Instance;
					case EditorShowMode.MouseUp:
						return MouseUpEditorShowModeStrategy.Instance;
					case EditorShowMode.MouseUpFocused:
						return MouseUpFocusedEditorShowModeStrategy.Instance;
					default:
						throw new Exception();
				}
			}
		}
		protected abstract FrameworkElement FocusOwner { get; }
		public IBaseEdit ActiveEditor {
			get { return activeEditor; }
			internal set {
				if(activeEditor == value)
					return;
				activeEditor = value;
				OnActiveEditorChanged();
			} 
		}
		protected abstract EditorShowMode EditorShowMode { get; }
		protected abstract bool EditorSetInactiveAfterClick { get; }
		protected abstract Type OwnerBaseType { get; }
		protected virtual bool CanCommitEditing { get { return CurrentCellEditor != null && CurrentCellEditor.IsEditorVisible && CurrentCellEditor.Edit.IsValueChanged; } }
		protected virtual bool UseMouseUpFocusedEditorShowModeStrategy { get { return false; } }
		protected internal virtual bool CanFocusEditor { get { return TopOwner.GetIsKeyboardFocusWithin(); } }
		protected InplaceEditorOwnerBase(FrameworkElement owner, bool subscribeKeyEvents = true) {
			this.owner = owner;
			EditorWasClosed = true;
			KeyboardLocker = new Locker();
			CommitEditorLocker = new Locker();
			if(subscribeKeyEvents) {
				owner.KeyUp += new KeyEventHandler(OnOwnerKeyUp);
				owner.KeyDown += new KeyEventHandler(OnOwnerKeyDown);
			}
		}
		protected internal abstract void EnqueueImmediateAction(IAction action);
		public abstract void ProcessKeyDown(KeyEventArgs e);
		protected internal abstract string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value);
		protected internal abstract bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText);
		protected abstract bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e);
		protected abstract bool CommitEditing();
		protected virtual void OnActiveEditorChanged() { }
		public void ProcessKeyUp(KeyEventArgs e) {
			if(!KeyboardLocker.IsLocked && IsNavigationKey(e.Key)) {
				DependencyObject originalSource = e.OriginalSource as DependencyObject;
				if(!object.ReferenceEquals(TopOwner, LayoutHelper.FindLayoutOrVisualParentObject(originalSource, OwnerBaseType)))
					return;
				ShowFocusedCellEditorIfNeeded();
			}
		}
		void OnOwnerKeyUp(object sender, KeyEventArgs e) {
			ProcessKeyUp(e);
		}
		protected virtual bool IsNavigationKey(Key key) {
			if(key == Key.Up || key == Key.Down || key == Key.Right || key == Key.Left || key == Key.Tab
			   || key == Key.Home || key == Key.End || key == Key.PageDown || key == Key.PageUp || key == Key.Escape
#if!SL
			   || key == Key.Next || key == Key.Prior
#endif
			   )
				return true;
			return false;
		}
		void OnOwnerKeyDown(object sender, KeyEventArgs e) {
			if(!KeyboardLocker.IsLocked)
				ProcessKeyDown(e);
		}
		void ShowFocusedCellEditorIfNeeded() {
			if(!EditorWasClosed && CurrentCellEditor != null && !CurrentCellEditor.IsEditorVisible) {
				CurrentCellEditor.ShowEditorAndSelectAll();
			}
		}
		protected internal virtual bool ShowEditor(bool selectAll) {
			return CurrentCellEditor != null && CurrentCellEditor.ShowEditorInternal(selectAll);
		}
		protected virtual bool IsChildOfCurrentEditor(MouseButtonEventArgs e) {
			return IsChildOfCurrentEditor(e.OriginalSource as DependencyObject);
		}
		protected virtual bool IsChildOfCurrentEditor(DependencyObject originalSource) {
			return LayoutHelper.IsChildElement(CurrentCellEditor, originalSource);
		}
#if SL
		bool ActivateOnEditorFeedBack() {
			if(CurrentCellEditor != null)
				return CurrentCellEditor.ShowEditor(false);
			return false;
		}
		public virtual bool NeedsKey(Key key, ModifierKeys modifiers) { return false; }
#endif
		void ActivateOnLeftMouse(MouseButtonEventArgs e, bool reRaiseEvent) {
			if(CurrentCellEditor != null && IsChildOfCurrentEditor(e))
				CurrentCellEditor.ActivateOnLeftMouseButton(e, reRaiseEvent);
		}
		void ActivateEditorOnMouseUp(MouseButtonEventArgs e) {
			if(CurrentCellEditor == lastMouseDownEditor && ModifierKeysHelper.NoModifiers(Keyboard.Modifiers)) {
				ActivateOnLeftMouse(e, false);
				lastMouseDownEditor = null;
			}
		}
		public void ProcessStylusUpCore(DependencyObject originalSource) {
			if(CurrentCellEditor != null && IsChildOfCurrentEditor(originalSource))
				CurrentCellEditor.ActivateOnStylusUp();
			if(!object.ReferenceEquals(owner, LayoutHelper.FindLayoutOrVisualParentObject(originalSource, OwnerBaseType)))
				return;
			if(!owner.GetIsKeyboardFocusWithin())
				KeyboardHelper.Focus(owner);
		}
		public void ProcessMouseLeftButtonDown(MouseButtonEventArgs e) {
			ProcessMouseButtonDown(e, true);
		}
		public void ProcessMouseRightButtonDown(MouseButtonEventArgs e) {
			ProcessMouseButtonDown(e, false);
		}
		void ProcessMouseButtonDown(MouseButtonEventArgs e, bool canShowEditor) {
			currentBeforeMouseDownEditor = CurrentCellEditor;
			if(canShowEditor) EditorShowModeStrategy.OnBeforeProcessLeftButtonDown(this, e);
			DependencyObject originalSource = e.OriginalSource as DependencyObject;
			if(!object.ReferenceEquals(TopOwner, LayoutHelper.FindLayoutOrVisualParentObject(originalSource, OwnerBaseType)))
				return;
			if(!TopOwner.GetIsKeyboardFocusWithin())
				KeyboardHelper.Focus(TopOwner);
			canShowEditor &= PerformNavigationOnLeftButtonDown(e);
			if(canShowEditor) EditorShowModeStrategy.OnAfterProcessLeftButtonDown(this, e);
			if(IsChildOfCurrentEditor(e))
				lastMouseDownEditor = CurrentCellEditor;
		}
#if SL
		#region IInplaceEditOwner Members
		InplaceEditHorizontalContentAlignment IInplaceEditOwner.HorizontalContentAlignment {
			get { return InplaceEditHorizontalContentAlignment.Default; }
		}
		VerticalAlignment IInplaceEditOwner.VerticalContentAlignment {
			get { return VerticalAlignment.Center; }
		}
		bool IInplaceEditOwner.BeginEdit(BaseEdit edit, object initialEditValue) {
			return BeginEdit(edit, initialEditValue);
		}
		protected virtual bool BeginEdit(BaseEdit edit, object initialEditValue) {
			return EditorShowModeStrategy.BeginInplaceEditing(this);
		}
		#endregion
#endif
		public void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) {
			EditorShowModeStrategy.OnProcessLeftButtonUp(this, e);
		}
		public void ProcessPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			if(KeyboardLocker.IsLocked && LayoutHelper.IsChildElementEx(FocusOwner, e.NewFocus as DependencyObject))
				e.Handled = true;
			if (!CanCommitEditing || LayoutHelper.IsChildElementEx(FocusOwner, e.NewFocus as DependencyObject) ||
				(CurrentCellEditor != null && CurrentCellEditor.IsChildElementOrMessageBox((IInputElement)e.NewFocus)))
				return;
			if(e.NewFocus != null && !InplaceEditorBase.CloseEditorOnLostKeyboardFocus && TopOwner != null  && FocusManager.GetFocusScope((DependencyObject)e.NewFocus) != FocusManager.GetFocusScope(TopOwner)) {		 
					return;
			}
			if(CurrentCellEditor != null)
				CurrentCellEditor.LockEditorFocus();
			bool wasVisible = CurrentCellEditor != null && CurrentCellEditor.IsEditorVisible;
			if(!CommitEditing()) {
				if(wasVisible && CurrentCellEditor!=null && !CurrentCellEditor.IsEditorVisible) CurrentCellEditor.Edit.SetKeyboardFocus();
				e.Handled = true;
			}
			if(CurrentCellEditor != null) {
				CurrentCellEditor.UnlockEditorFocus();
				if(!CurrentCellEditor.IsEditorVisible) EditorWasClosed = true;
			}
		}
		public void ProcessIsKeyboardFocusWithinChanged() {
			if(!LayoutHelper.IsChildElement(LayoutHelper.GetTopLevelVisual(FocusOwner), FocusHelper.GetFocusedElement() as DependencyObject))
				return;
			if(FocusOwner.GetIsKeyboardFocusWithin() && !owner.GetIsKeyboardFocusWithin()) {
				KeyboardHelper.Focus(owner);
			}
			bool isActiveEditorFocused = ActiveEditor != null && ((FrameworkElement)ActiveEditor).GetIsKeyboardFocusWithin();
			if(owner.GetIsKeyboardFocusWithin() && !isActiveEditorFocused)
				CurrentCellEditor.Do(editor => editor.SetKeyboardFocus());
		}
		public virtual bool IsActiveEditorHaveValidationError() { 
			if(ActiveEditor == null)
				return false;
			BaseEdit activeEdit = ActiveEditor as BaseEdit;
			if(activeEdit == null)
				return false;
			return activeEdit.HasValidationError;
		}
		public virtual bool ProcessKeyForLookUp(KeyEventArgs e) {
			return CurrentCellEditor.Return(x => x.ProcessKeyForLookUp(e), () => false);
		}
		public void MoveFocus(KeyEventArgs e) {
			MoveFocusHelper.MoveFocus(FocusOwner, ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
			e.Handled = true;
		}
		internal protected virtual bool CommitEditorForce(){
			return CommitEditing();
		}
	}
	public static class MoveFocusHelper {
		public static void MoveFocus(FrameworkElement focusOwner, bool isReverse) {
			if(isReverse) {
				focusOwner.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
			} else {
				FrameworkElement lastElement = focusOwner;
				LayoutHelper.ForEachElement(focusOwner, element => {
					lastElement = element;
				});
				lastElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
			}
		}
	}
}
