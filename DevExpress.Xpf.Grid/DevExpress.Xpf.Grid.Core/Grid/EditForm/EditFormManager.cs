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

using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Native;
using System.ComponentModel;
namespace DevExpress.Xpf.Grid.EditForm {
	public interface IEditFormManager {
		bool IsEditFormVisible { get; }
		bool AllowEditForm { get; }
		void OnDoubleClick(MouseButtonEventArgs e);
		void OnPreviewKeyDown(KeyEventArgs e);
		void OnInlineFormClosed();
		bool RequestUIUpdate();
		void OnBeforeScroll(int targetRowHandle);
		void OnAfterScroll();
	}
	public class EditFormManager : IEditFormManager {
		#region inner classes
		class UIUpdateResultWrapper {
			readonly Action executeAction;
			readonly bool isLocked;
			public UIUpdateResultWrapper(bool isLocked) : this(isLocked, null) { }
			public UIUpdateResultWrapper(bool isLocked, Action executeAction) {
				this.executeAction = executeAction;
				this.isLocked = isLocked;
			}
			public void Execute() {
				if(executeAction != null)
					executeAction();
			}
			public bool IsLocked { get { return isLocked; } }
		}
		#endregion
		readonly ITableView view;
		bool isPopupEditFormVisible;
		protected ITableView TableView { get { return view; } }
		protected DataViewBase View { get { return TableView.ViewBase; } }
		public EditFormManager(ITableView view) {
			if(view == null)
				throw new ArgumentNullException("view");
			this.view = view;
		}
		internal protected virtual EditFormRowData CreateEditFormData() {
			var vm = EditFormRowData.Factory();
			vm.EditFormOwner = CreateEditFormOwner();
			return vm;
		}
		internal IEditFormOwner CreateEditFormOwner() {
			return View.CreateEditFormOwner();
		}
		IMessageBoxService messageBoxServiceCore = new DXMessageBoxService();
		public IMessageBoxService MessageBoxService {
			get { return messageBoxServiceCore; }
			set {
				if(messageBoxServiceCore != value)
					messageBoxServiceCore = value;
			}
		}
		public void ShowDialogEditForm() {
			if(!CanShowEditForm())
				return;
			Mvvm.UI.Native.AssignableServiceHelper2<FrameworkElement, IDialogService>.DoServiceAction(View, TableView.EditFormDialogServiceTemplate, service => {
				isPopupEditFormVisible = true;
				EditFormRowData data = CreateEditFormData();
				data.CanShowUpdateCancelButtons = false;
				List<UICommand> commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new EditFormPopupButtonLocalizer());
				commands[0].Command = new DelegateCommand<CancelEventArgs>(x => x.Cancel = !data.TryCommit());
				UICommand result = service.ShowDialog(commands, CreateDialogTitle(), data);
				isPopupEditFormVisible = false;
				if(result == commands[1])
					data.Cancel();
			});
		}
		string CreateDialogTitle() {
			return GetBindingValue(TableView.EditFormCaptionBinding, View.GetRowData(View.FocusedRowHandle));
		}
		bool CanShowEditForm() {
			if(IsEditFormVisible)
				return false;
			int rowHandle = View.FocusedRowHandle;
			if(rowHandle == DataControlBase.InvalidRowHandle)
				return false;
			DataControlBase dataControl = View.DataControl;
			if(dataControl != null && dataControl.IsGroupRowHandleCore(rowHandle))
				return false;
			return true;
		}
		string GetBindingValue(BindingBase binding, object content) {
			if(binding == null || content == null)
				return null;
			BindingBase bnd = BindingCloneHelper.Clone(binding, content);
			if(bnd == null)
				return string.Empty;
			BindingValueEvaluator evl = new BindingValueEvaluator(bnd);
			if(evl.Value == null)
				return string.Empty;
			return evl.Value.ToString();
		}
		RowData activeInplaceRowDataCore;
		RowData ActiveInplaceRowData {
			get { return activeInplaceRowDataCore; }
			set {
				if(activeInplaceRowDataCore != value) {
					activeInplaceRowDataCore = value;
					View.UpdateColumnsViewInfo();
				}
			}
		}
		public void ShowInlineEditForm() {
			if(!CanShowEditForm())
				return;
			int rowHandle = View.FocusedRowHandle;
			ActiveInplaceRowData = View.GetRowData(rowHandle);
			if(ActiveInplaceRowData != null) {
				EditFormRowData data = CreateEditFormData();
				data.CanShowUpdateCancelButtons = true;
				ActiveInplaceRowData.EditFormData = data;
				View.ScrollIntoView(rowHandle);
			}
		}
		public void HideEditForm() {
			CloseActiveInplaceFormCore(x => x.Cancel());
		}
		public void CloseEditForm() {
			CloseActiveInplaceFormCore(x => x.Commit());
		}
		void CloseActiveInplaceForm() {
			CloseActiveInplaceFormCore(x => x.Close());
		}
		void CloseActiveInplaceFormCore(Action<EditFormRowData> closeAction) {
			EditFormRowData data = ActiveInplaceRowData.With(x => x.EditFormData);
			if(data != null)
				closeAction(data);
		}
		public void OnInlineFormClosed() {
			if(ActiveInplaceRowData != null) {
				ActiveInplaceRowData.EditFormData = null;
				ActiveInplaceRowData = null;
			}
		}
		public void ShowEditForm() {
			EditFormShowMode mode = TableView.EditFormShowMode;
			switch(mode) {
				case EditFormShowMode.Dialog:
					ShowDialogEditForm();
					break;
				case EditFormShowMode.InlineHideRow:
				case EditFormShowMode.Inline:
					ShowInlineEditForm();
					break;
			}
		}
		bool IsInlineEditFormVisible { get { return ActiveInplaceRowData != null; } }
		public bool IsEditFormVisible { get { return isPopupEditFormVisible || IsInlineEditFormVisible; } }
		public bool AllowEditForm { get { return TableView.EditFormShowMode != EditFormShowMode.None; } }
		public bool RequestUIUpdate() {
			UIUpdateResultWrapper result = CalcUIUpdateResult() ?? new UIUpdateResultWrapper(false);
			result.Execute();
			return !result.IsLocked;
		}
		UIUpdateResultWrapper CalcUIUpdateResult() {
			EditFormRowData data = ActiveInplaceRowData.With(x => x.EditFormData);
			if(data == null || !ActiveInplaceRowData.IsInlineEditFormVisible || scrollLocker.IsLocked)
				return null;
			return data.IsModified ? GetModifiedRowResult() : new UIUpdateResultWrapper(false, CloseActiveInplaceForm);
		}
		UIUpdateResultWrapper GetModifiedRowResult() {
			PostConfirmationMode mode = TableView.EditFormPostConfirmation;
			switch(mode) {
				case PostConfirmationMode.YesNoCancel:
					return GetSaveDialogResult(ShowRowChangeMessage(Localize(GridControlStringId.EditForm_Modified), MessageButton.YesNoCancel));
				case PostConfirmationMode.YesNo:
					return GetCancelDialogResult(ShowRowChangeMessage(Localize(GridControlStringId.EditForm_Cancel), MessageButton.YesNo));
				case PostConfirmationMode.None:
					return new UIUpdateResultWrapper(true);
				default:
					return null;
			}
		}
		UIUpdateResultWrapper GetSaveDialogResult(MessageResult messageResult) {
			switch(messageResult) {
				case MessageResult.Yes:
					return new UIUpdateResultWrapper(false, CloseEditForm);
				case MessageResult.No:
					return new UIUpdateResultWrapper(false, HideEditForm);
				case MessageResult.Cancel:
					return new UIUpdateResultWrapper(true);
				default:
					return null;
			}
		}
		UIUpdateResultWrapper GetCancelDialogResult(MessageResult messageResult) {
			switch(messageResult) {
				case MessageResult.Yes:
					return new UIUpdateResultWrapper(false, HideEditForm);
				case MessageResult.No:
					return new UIUpdateResultWrapper(true);
				default:
					return null;
			}
		}
		MessageResult ShowRowChangeMessage(string message, MessageButton button) {
			return MessageBoxService.ShowMessage(message, Localize(GridControlStringId.EditForm_Warning), button, MessageIcon.None, MessageResult.Yes);
		}
		static string Localize(GridControlStringId id) {
			return GridControlLocalizer.GetString(id);
		}
		public void OnDoubleClick(MouseButtonEventArgs e) {
			if(TableView.ShowEditFormOnDoubleClick)
				ShowEditForm();
		}
		public void OnPreviewKeyDown(KeyEventArgs e) {
			Key key = e.Key;
			if(key == Key.Enter && ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers))
				CloseEditForm();
			else if(key == Key.F2 && TableView.ShowEditFormOnF2Key || key == Key.Enter && TableView.ShowEditFormOnEnterKey)
				ShowEditForm();
			else if(key == Key.Escape)
				HideEditForm();
		}
		Locker scrollLocker = new Locker();
		public void OnBeforeScroll(int targetRowHandle) {
			var rowHandle = ActiveInplaceRowData.With(x => x.RowHandle);
			if(rowHandle != null && rowHandle.Value == targetRowHandle)
				scrollLocker.Lock();
		}
		public void OnAfterScroll() {
			scrollLocker.Unlock();
		}
	}
	internal class EditFormPopupButtonLocalizer : IMessageBoxButtonLocalizer {
		public string Localize(MessageBoxResult button) {
			switch(button) {
				case MessageBoxResult.OK:
					return Localize(GridControlStringId.EditForm_UpdateButton);
				case MessageBoxResult.Cancel:
					return Localize(GridControlStringId.EditForm_CancelButton);
				default:
					return string.Empty;
			}
		}
		static string Localize(GridControlStringId id) {
			return GridControlLocalizer.GetString(id);
		}
	}
	public class EmptyEditFormManager : IEditFormManager {
		static EmptyEditFormManager instance;
		static EmptyEditFormManager() { }
		public static EmptyEditFormManager Instance {
			get {
				if(instance == null) {
					instance = new EmptyEditFormManager();
				}
				return instance;
			}
		}
		public bool IsEditFormVisible { get { return false; } }
		public bool AllowEditForm { get { return false; } }
		public void OnDoubleClick(MouseButtonEventArgs e) { }
		public void OnPreviewKeyDown(KeyEventArgs e) { }
		public bool RequestUIUpdate() {
			return true;
		}
		public void OnInlineFormClosed() { }
		public void OnBeforeScroll(int targetIndex) { }
		public void OnAfterScroll() { }
	}
}
