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
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Data.Filtering;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Utils;
#endif
#if SL
using IInputElement = System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Editors {
	public interface IBaseEditWrapper {
		void LockEditorFocus();
		void UnlockEditorFocus();
		bool NeedsKey(KeyEventArgs e);
		bool IsActivatingKey(KeyEventArgs e);
		void ProcessActivatingKey(KeyEventArgs e);
		bool IsReadOnly { get; set; }
		bool ShowEditorButtons { get; set; }
		bool IsEditorActive { get; }
		bool IsValueChanged { get; set; }
		EditMode EditMode { get; set; }
		object EditValue { get; set; }
		void SetKeyboardFocus();
		void SetValidationError(BaseValidationError validationError);
		bool IsChildElement(IInputElement element, DependencyObject root = null);
		void SetDisplayTemplate(ControlTemplate template);
		void SetEditTemplate(ControlTemplate template);
		void SelectAll();
		event EditValueChangedEventHandler EditValueChanged;
		void FlushPendingEditActions();
		bool DoValidate();
		void ClearEditorError();
		HorizontalAlignment HorizontalContentAlignment { get; }
		void ResetEditorCache();
		void SetValidationErrorTemplate(DataTemplate template);
	}
	public class BaseEditWrapper : IBaseEditWrapper {
		readonly IBaseEdit edit;
		public BaseEditWrapper(IBaseEdit edit) {
			this.edit = edit;
		}
		bool IBaseEditWrapper.IsReadOnly { get { return edit.IsReadOnly; } set { edit.IsReadOnly = value; } }
		bool IBaseEditWrapper.IsEditorActive { get { return edit.IsEditorActive; } }
		EditMode IBaseEditWrapper.EditMode { get { return edit.EditMode; } set { edit.EditMode = value; } }
		object IBaseEditWrapper.EditValue { get { return edit.EditValue; } set { edit.EditValue = value; } }
		bool IBaseEditWrapper.ShowEditorButtons {
			get { return BaseEditHelper.GetShowEditorButtons(edit); }
			set { BaseEditHelper.SetShowEditorButtons(edit, value); }
		}
		bool IBaseEditWrapper.IsValueChanged { 
			get { return BaseEditHelper.GetIsValueChanged(edit); } 
			set { BaseEditHelper.SetIsValueChanged(edit, value); } 
		}
		void IBaseEditWrapper.LockEditorFocus() {
			edit.CanAcceptFocus = false;
		}
		void IBaseEditWrapper.UnlockEditorFocus() {
			edit.CanAcceptFocus = true;
		}
		void IBaseEditWrapper.SetKeyboardFocus() {
#if !SL
			KeyboardHelper.Focus((UIElement)edit);
#else
			if (!edit.Focus())
				((FrameworkElement)edit).LayoutUpdated += OnEditLayoutUpdated;
#endif
		}
#if SL
		void OnEditLayoutUpdated(object sender, System.EventArgs e) {
			((FrameworkElement)edit).LayoutUpdated -= OnEditLayoutUpdated;
			edit.Focus();
		}
#endif
		void IBaseEditWrapper.SetDisplayTemplate(ControlTemplate template) {
#if !SL
			if (edit is IInplaceBaseEdit || template != null) {
#else
			if (template != null) {
#endif
				edit.DisplayTemplate = template;
				return;
			}
			edit.ClearValue(BaseEdit.DisplayTemplateProperty);
		}
		void IBaseEditWrapper.SetEditTemplate(ControlTemplate template) {
#if !SL
			if (edit is IInplaceBaseEdit || template != null) {
#else
			if (template != null) {
#endif
				edit.EditTemplate = template;
				return;
			}
			edit.ClearValue(BaseEdit.EditTemplateProperty);
		}
		void IBaseEditWrapper.SetValidationError(BaseValidationError validationError) {
			BaseEditHelper.SetValidationError((DependencyObject)edit, validationError);
		}
		void IBaseEditWrapper.SetValidationErrorTemplate(DataTemplate template) {
			BaseEditHelper.SetValidationErrorTemplate((DependencyObject)edit, template);
		}
		bool IBaseEditWrapper.DoValidate() {
			return edit.DoValidate();
		}
		void IBaseEditWrapper.SelectAll() {
			edit.SelectAll();
		}
		event EditValueChangedEventHandler IBaseEditWrapper.EditValueChanged {
			add { edit.EditValueChanged += value; }
			remove { edit.EditValueChanged -= value; }
		}
		bool IBaseEditWrapper.IsChildElement(IInputElement element, DependencyObject root) {
			return edit.IsChildElement(element, root);
		}
		bool IBaseEditWrapper.NeedsKey(KeyEventArgs e) {
			return BaseEditHelper.GetNeedsKey(edit, e);
		}
		bool IBaseEditWrapper.IsActivatingKey(KeyEventArgs e) {
			return BaseEditHelper.GetIsActivatingKey(edit, e);
		}
		void IBaseEditWrapper.ProcessActivatingKey(KeyEventArgs e) {
			BaseEditHelper.ProcessActivatingKey(edit, e);
		}
		void IBaseEditWrapper.FlushPendingEditActions() {
			BaseEditHelper.FlushPendingEditActions(edit);
		}
		void IBaseEditWrapper.ClearEditorError() {
			edit.ClearError();
		}
		HorizontalAlignment IBaseEditWrapper.HorizontalContentAlignment { get { return edit.HorizontalContentAlignment; } }
		void IBaseEditWrapper.ResetEditorCache() {
			BaseEditHelper.ResetEditorCache(edit);
		}
	}
	public class FakeBaseEdit : IBaseEditWrapper {
		public static readonly IBaseEditWrapper Instance = new FakeBaseEdit();
		private FakeBaseEdit() {
		}
		void IBaseEditWrapper.LockEditorFocus() { }
		void IBaseEditWrapper.UnlockEditorFocus() { }
		bool IBaseEditWrapper.NeedsKey(KeyEventArgs e) {
			return true;
		}
		bool IBaseEditWrapper.IsActivatingKey(KeyEventArgs e) {
			return false;
		}
		void IBaseEditWrapper.ProcessActivatingKey(KeyEventArgs e) { }
		bool IBaseEditWrapper.IsReadOnly { get { return true; } set { } }
		bool IBaseEditWrapper.ShowEditorButtons { get { return false; } set { } }
		bool IBaseEditWrapper.IsEditorActive { get { return false; } }
		bool IBaseEditWrapper.IsValueChanged { get { return false; } set { } }
		EditMode IBaseEditWrapper.EditMode { get { return EditMode.InplaceInactive; } set { } }
		object IBaseEditWrapper.EditValue { get { return null; } set { } }
		void IBaseEditWrapper.SetKeyboardFocus() { }
		void IBaseEditWrapper.SetValidationError(BaseValidationError validationError) { }
		void IBaseEditWrapper.SetValidationErrorTemplate(DataTemplate template) { }
		bool IBaseEditWrapper.IsChildElement(IInputElement element, DependencyObject root) { return false; }
		void IBaseEditWrapper.SetDisplayTemplate(ControlTemplate template) { }
		void IBaseEditWrapper.SetEditTemplate(ControlTemplate template) { }
		void IBaseEditWrapper.SelectAll() { }
		event EditValueChangedEventHandler IBaseEditWrapper.EditValueChanged { add { } remove { } }
		void IBaseEditWrapper.FlushPendingEditActions() { }
		bool IBaseEditWrapper.DoValidate() { return true; }
		void IBaseEditWrapper.ClearEditorError() { }
		HorizontalAlignment IBaseEditWrapper.HorizontalContentAlignment { get { return default(HorizontalAlignment); } }
		void IBaseEditWrapper.ResetEditorCache() { }
	}
}
