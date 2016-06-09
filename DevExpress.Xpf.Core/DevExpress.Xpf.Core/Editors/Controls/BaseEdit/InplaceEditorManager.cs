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

using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors.Internal {
	public interface IInplaceEditorManager {
		void PreviewTextInput(TextCompositionEventArgs args);
		void PreviewKeyDown(KeyEventArgs args);
		void PreviewMouseDown(KeyEventArgs args);
	}
	public interface IInplaceEditorOwnerInfo {
		bool IsReadOnly { get; }
		IInplaceEditorColumn EditorColumn { get; }
		bool IsInTree { get; }
		bool IsInactiveEditorButtonVisible { get; }
		object GetEditableValue();
	}
	public abstract class InplaceEditorManagerBase : IInplaceEditorManager {
		IBaseEdit editor;
		InplaceEditorOwnerBase editorOwner;
		IInplaceEditorOwnerInfo ownerInfo;
		protected internal IBaseEdit Editor { get { return editor; } }
		protected internal InplaceEditorOwnerBase EditorOwner { get { return editorOwner; } }
		protected internal IInplaceEditorOwnerInfo OwnerInfo { get { return ownerInfo; } }
		protected virtual bool IsEditorVisible { get { return Editor.EditMode == EditMode.InplaceActive; } }
		protected virtual bool IsReadOnly { get { return OwnerInfo.IsReadOnly; } }
		protected virtual bool HasAccessToCell { get { return OwnerInfo.IsInTree && OwnerInfo.EditorColumn != null; } }
		protected InplaceEditorManagerBase() {
		}
		public virtual void Assign(IBaseEdit editor, InplaceEditorOwnerBase editorOwner, IInplaceEditorOwnerInfo editorInfo) {
			this.editor = editor;
			this.editorOwner = editorOwner;
			this.ownerInfo = editorInfo;
		}
		public virtual void PreviewTextInput(TextCompositionEventArgs args) {
		}
		public virtual void PreviewKeyDown(KeyEventArgs args) {
		}
		public virtual void PreviewMouseDown(KeyEventArgs args) {
		}
		public virtual bool PostEditor() {
			return false;
		}
		public virtual bool ShowEditor(bool selectAll = false) {
			return false;
		}
	}
	public class InplaceEditorManager : InplaceEditorManagerBase {
		const string tab = "\t";
		const int EscCode = 27;
		public override void PreviewTextInput(TextCompositionEventArgs e) {
			if (!ShouldProcessTextInput(e))
				return;
			if (!IsEditorVisible)
				ShowEditor(true);
			if (IsEditorVisible && !Editor.IsEditorActive) {
				EditorOwner.EnqueueImmediateAction(new DelegateAction(() => {
					if(!OwnerInfo.IsInTree)
						return;
					ReraiseEventHelper.ReraiseEvent(
						e, (UIElement)FocusHelper.GetFocusedElement(), 
						UIElement.PreviewTextInputEvent, UIElement.TextInputEvent,
						args => new TextCompositionEventArgs(args.Device, args.TextComposition));
				}));
				e.Handled = true;
			}
		}
		bool ShouldProcessTextInput(TextCompositionEventArgs e) {
			if (e.Text == tab)
				return false;
			if (string.IsNullOrEmpty(e.Text) || !string.IsNullOrEmpty(e.ControlText) || !string.IsNullOrEmpty(e.SystemText) || e.Text[0] == EscCode)
				return false;
			return true;
		}
		public override void PreviewKeyDown(KeyEventArgs e) {
			if (!IsEditorVisible && Editor.IsActivatingKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e))) {
				ShowEditor(true);
				if (IsEditorVisible) {
					EditorOwner.EnqueueImmediateAction(new DelegateAction(() => Editor.ProcessActivatingKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e))));
					e.Handled = true;
				}
				else
					RaiseKeyDownEvent(e);
				return;
			}
			if (!Editor.NeedsKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e))) {
				if (e.Key == Key.Enter) {
					if (IsEditorVisible) {
						CommitEditor(true);
						CheckFocus();
					}
					else {
						ShowEditor(true);
					}
					e.Handled = true;
				}
				if (e.Key == Key.F2) {
					if (!IsEditorVisible) {
						ShowEditor(true);
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
						CancelRowEdit();
				}
				if (!e.Handled) {
					RaiseKeyDownEvent(e);
				}
			}
		}
		public override bool ShowEditor(bool selectAll = false) {
			if (!CanShowEditor())
				return false;
			if (!IsEditorVisible) {
				UpdateEditTemplate();
				Editor.IsReadOnly = IsReadOnly;
				SetActiveEditMode();
				EditorOwner.ActiveEditor = Editor;
				Editor.Focus();
				UpdateEditContext();
				Editor.EditValueChanged += OnEditValueChanged;
			}
			OnShowEditor();
			EditorOwner.EditorWasClosed = false;
			UpdateEditorButtonVisibility();
			BaseEditHelper.SetIsValueChanged(Editor, false);
			if (selectAll)
				EditorOwner.EnqueueImmediateAction(new DelegateAction(() => Editor.SelectAll()));
			return true;
		}
		void UpdateEditorButtonVisibility() {
			if (!OwnerInfo.IsInTree)
				return;
			Editor.ShowEditorButtons = IsEditorVisible || OwnerInfo.IsInactiveEditorButtonVisible;
		}
		void OnShowEditor() {
		}
		void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
		}
		void UpdateEditContext() {
			if (HasAccessToCell)
				Editor.EditValue = OwnerInfo.GetEditableValue();
		}
		void SetActiveEditMode() {
			Editor.EditMode = EditMode.InplaceActive;
			Editor.EditValue = OwnerInfo.GetEditableValue();
		}
		void UpdateEditTemplate() {
		}
		bool CanShowEditor() {
			return Editor.IsKeyboardFocusWithin;
		}
		void CancelRowEdit() {
		}
		void CancelEditInVisibleEditor() {
			if (!IsEditorVisible)
				return;
			Editor.ClearError();
			HideEditor(true);
		}
		void CheckFocus() {
		}
		void CommitEditor(bool closeEditor = false) {
			if (!IsEditorVisible)
				return;
			if (PostEditor())
				HideEditor(closeEditor);
		}
		public void HideEditor(bool closeEditor) {
			if (IsEditorVisible) {
				Editor.EditMode = EditMode.InplaceInactive;
				Editor.EditValue = OwnerInfo.GetEditableValue();
				Editor.EditValueChanged -= OnEditValueChanged;
				EditorOwner.ActiveEditor = null;
				OnHiddenEditor();
			}
			if (closeEditor)
				EditorOwner.EditorWasClosed = true;
		}
		void OnHiddenEditor() {
			UpdateEditorButtonVisibility();
		}
		void RaiseKeyDownEvent(KeyEventArgs e) {
			e.Handled = true;
			EditorOwner.ProcessKeyDown(e);
		}
	}
	public class DummyEditorManagerBase : InplaceEditorManagerBase {
		public DummyEditorManagerBase()
			: base() {
		}
	}
}
