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
using System.Linq;
using System.Text;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Keyboard;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using System.Windows.Input;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
#if SL
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = DevExpress.Data.KeyPressEventArgs;
using PlatformIndependentKeys = DevExpress.Data.Keys;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
#else
using PlatformIndependentKeys = System.Windows.Forms.Keys;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public enum InputState {
		Undefined,
		StartInput,
		UpdateInput,
		TextChanged,
		Input
	}
	public class ImeController {
		InputState inputState = InputState.Undefined;
		bool isActive = false;
		bool isClosed = false;
		bool isCanceled = false;
		bool enabled = true;
		XpfImeCompositionInfo compositionInfo;
		ICommandExecutionListenerService oldCommandExecutionListenerService;
		ImeCommandExecutionListenerService commandExecutionListenerService;
		string lastText = String.Empty;
		IMEKeyboardHandler keyboardHandler;
		IMEMouseHandler mouseHandler;
		RichEditControl control;
		public ImeController(RichEditControl control) {
			this.control = control;
			this.compositionInfo = new XpfImeCompositionInfo();
			this.keyboardHandler = new IMEKeyboardHandler(control);
			this.mouseHandler = new IMEMouseHandler(control);
		}
		internal bool Enabled { get { return enabled; } set { enabled = value; } }
		public InputState InputState { get { return inputState; } set { inputState = value; } }
		IMEKeyboardHandler KeyboardHandler { get { return keyboardHandler; } set { keyboardHandler = value; } }
		IMEMouseHandler MouseHandler { get { return mouseHandler; } set { mouseHandler = value; } }
		internal RichEditControl Control { get { return control; } }
		internal XpfImeCompositionInfo CompositionInfo { get { return compositionInfo; } }
		ImeCommandExecutionListenerService CommandExecutionListenerService { get { return commandExecutionListenerService; } set { commandExecutionListenerService = value; } }
		public bool IsActive {
			get { return isActive && Enabled; }
			private set {
				isActive = value;
			}
		}
		public bool IsCanceled { get { return isCanceled; } }
		bool IsClosed { get { return isClosed; } }
		public void Reset() {
			this.isActive = false;
			this.isClosed = false;
			this.isCanceled = false;
			this.compositionInfo.Reset(this.control.DocumentModel);
		}
		public void ResetCompositionInfo() {
			this.compositionInfo.Reset(this.control.DocumentModel);
		}
		public void Cancel() {
			if (!Enabled)
				return;
			this.isCanceled = true;
			if (!IsActive)
				return;
			if (CompositionInfo.Length != 0)
				Undo();
			EndImeInput();
		}
		public void Close() {
			if (!IsActive)
				return;
			bool shouldForceFlushPendingTextInput = CompositionInfo.Length == 0 && CompositionInfo.Offset != 0;
			if(CompositionInfo.Length != 0 || CompositionInfo.Offset != 0)
				Undo();
			Reset();
			EndImeInput();
			if(shouldForceFlushPendingTextInput)
				((DevExpress.XtraRichEdit.IRichEditControl)this.Control).ForceFlushPendingTextInput();
		}
		public void UpdateCaretPosition(int caretIndex, int selectionLength) {
			if (!IsActive)
				return;
			Selection selection = Control.DocumentModel.Selection;
			selection.BeginUpdate();
			DocumentLogPosition caretLogPosition = CompositionInfo.StartPos + caretIndex;
			selection.Start = caretLogPosition;
			selection.End = caretLogPosition + selectionLength;
			selection.EndUpdate();
			EnsureCaretVisibleHorizontallyCommand ensureCaretVisibleHorizontallyCommand = new EnsureCaretVisibleHorizontallyCommand(Control);
			EnsureCaretVisibleVerticallyCommand ensureCaretVisibleVerticallyCommand = new EnsureCaretVisibleVerticallyCommand(Control);
			ensureCaretVisibleHorizontallyCommand.Execute();
			ensureCaretVisibleVerticallyCommand.Execute();
		}
		public void OnKeyDown(KeyEventArgs e, ModifierKeys modifier) {
			if (!IsActive)
				return;
#if !SL
			bool isKeyCloseImeMode = e.ImeProcessedKey == Key.Enter || e.Handled;
#else
			bool isKeyCloseImeMode = e.Key == Key.Enter || e.Handled;
#endif
			bool isCancelImeMode = e.Key == Key.Z && modifier == ModifierKeys.Control;
			if (isCancelImeMode)
				Cancel();
			else if (isKeyCloseImeMode)
				Close();
		}
		public void OnTextInputStart(string composition) {
			if (!Enabled)
				return;
			if (String.IsNullOrEmpty(composition) && IsActive) {
				Close();
			}
			else if (!String.IsNullOrEmpty(composition) && !IsActive) {
				Reset();
				BeginImeInput();
				ResetCompositionInfo();
				CompositionInfo.CompositionText = composition;
			}
			InputState = Internal.InputState.StartInput;
		}
		public void OnTextInputUpdate(string composition) {
			if (!Enabled)
				return;
			switch (InputState) {
				case Internal.InputState.StartInput:
					Reset();
					if (!String.IsNullOrEmpty(composition))
						BeginImeInput();
					ResetCompositionInfo();
					break;
				case Internal.InputState.Input:
					break;
				case Internal.InputState.TextChanged:
					break;
				default:
					InputState = Internal.InputState.Undefined;
					return;
			}
			CompositionInfo.CompositionText = composition;
			InputState = Internal.InputState.UpdateInput;
		}
		public void OnTextInput(string composition) {
			if (!Enabled)
				return;
			InputState = Internal.InputState.Input;
		}
		public void OnTextChanged(string text) {
			if (!Enabled)
				return;
			switch (InputState) {
				case Internal.InputState.UpdateInput:
					if (!ValidateImeMode())
						return;
					UpplyComposition(text);
					break;
				case Internal.InputState.Input:
					if (!ValidateImeMode())
						return;
					if (CompositionInfo.Length != 0)
						Undo();
					EndImeInput();
					Reset();
					break;
				case Internal.InputState.StartInput:
					if (String.IsNullOrEmpty(text)) {
						if (IsActive) {
							if (CompositionInfo.Length != 0)
								Undo();
							EndImeInput();
							Reset();
						}
						return;
					}
					UpplyComposition(text);
					break;
			}
			InputState = Internal.InputState.TextChanged;
		}
		void CaptureUserInput() {
			MouseHandler.Initialize();
			KeyboardHandler.Initialize();
			Control.InnerControl.SetNewKeyboardHandler(KeyboardHandler);
			Control.InnerControl.SetNewMouseHandler(MouseHandler);
		}
		void ReleaseUserInput() {
			if (Object.Equals(Control.InnerControl.KeyboardHandler, KeyboardHandler))
				Control.InnerControl.RestoreKeyboardHandler();
			if (Object.Equals(Control.InnerControl.MouseHandler, MouseHandler))
				Control.InnerControl.RestoreMouseHandler();
		}
		void BeginImeInput() {
			this.isActive = true;
			CaptureUserInput();
			Control.DocumentModel.ResetMerging();
			DeleteNonEmptySelectionCommand deleteCommand = new DeleteNonEmptySelectionCommand(Control);
			if (deleteCommand.CanExecute())
				deleteCommand.Execute();
			RegisterImeCommandExecutionListenerService();
		}
		void EndImeInput() {
			ReleaseUserInput();
			this.isActive = false;
			this.isClosed = true;
			UnregisterImeCommandExecutionListenerService();
			Control.DocumentModel.ResetMerging();
		}
		void RegisterImeCommandExecutionListenerService() {
			this.oldCommandExecutionListenerService = Control.GetService(typeof(ICommandExecutionListenerService)) as ICommandExecutionListenerService;
			this.commandExecutionListenerService = new ImeCommandExecutionListenerService(this, this.oldCommandExecutionListenerService);
			Control.RemoveService(typeof(ICommandExecutionListenerService));
			Control.AddService(typeof(ICommandExecutionListenerService), CommandExecutionListenerService);
		}
		void UnregisterImeCommandExecutionListenerService() {
			Control.RemoveService(typeof(ICommandExecutionListenerService));
			Control.AddService(typeof(ICommandExecutionListenerService), this.oldCommandExecutionListenerService);
		}
		void UpplyComposition(string text) {
			if (!IsActive)
				return;
			if (CompositionInfo.Length != 0 || CompositionInfo.Offset != 0)
				Undo();
			InsertCompositionText(text);
		}
		void InsertCompositionText(string text) {
			this.lastText = text;
			DocumentModel model = Control.DocumentModel;
			model.BeginUpdate();
			try {
				int compositionLength = CompositionInfo.CompositionText.Length;
				int offset = Math.Max(0, text.Length - compositionLength);
				if (offset > 0)
					InsertText(text.Substring(0, offset));
				if (offset < text.Length) {
					model.ResetMerging();
					InsertText(text.Substring(offset));
				}
				CompositionInfo.Offset = offset;
				CompositionInfo.Length = compositionLength;
			}
			finally {
				model.EndUpdate();
			}
		}
		void InsertText(string imeString) {
			if (!String.IsNullOrEmpty(imeString)) {
				InsertTextCommand command = new InsertTextCommand(Control);
				command.CommandSourceType = CommandSourceType.Keyboard;
				command.Text = imeString;
				command.Execute();
			}
		}
		void Undo() {
			InputPosition pos = Control.ActiveView.CaretPosition.GetInputPosition();
			ImeUndoCommand undoCommand = new ImeUndoCommand(Control);
			undoCommand.Execute();
			if (pos != null)
				Control.ActiveView.CaretPosition.GetInputPosition().CopyFormattingFrom(pos);
		}
		bool ValidateImeMode() {
			if (!this.isActive)
				InputState = Internal.InputState.Undefined;
			return this.isActive;
		}
		public RunInfo GetImeStringRange() {
			return CompositionInfo.GetRunInfo(Control.DocumentModel);
		}
	}
	public class XpfImeCompositionInfo : ImeCompositionInfo {
		public int Offset { get; set; }
		internal string CompositionText { get; set; }
		public override void Reset(DocumentModel model) {
			Offset = 0;
			CompositionText = String.Empty;
			base.Reset(model);
		}
		public override RunInfo GetRunInfo(DocumentModel model) {
			if (Length == 0)
				return null;
			return model.ActivePieceTable.FindRunInfo(StartPos + Offset, Length);
		}
	}
	public class ImeCommandExecutionListenerService : ICommandExecutionListenerService {
		ICommandExecutionListenerService baseService;
		ImeController imeController;
		bool allowCloseIme = false;
		public ImeCommandExecutionListenerService(ImeController imeController, ICommandExecutionListenerService baseService) {
			Guard.ArgumentNotNull(imeController, "imeController");
			this.imeController = imeController;
			this.baseService = baseService;
		}
		ICommandExecutionListenerService BaseService { get { return baseService; } }
		ImeController ImeController { get { return imeController; } }
		public bool AllowCloseIme { get { return allowCloseIme; } set { allowCloseIme = value; } }
		#region ICommandExecutionListenerService Members
		public void BeginCommandExecution(Command command, ICommandUIState state) {
			if (ImeController.IsActive && AllowCloseIme)
				imeController.Close();
			if (BaseService != null)
				BaseService.BeginCommandExecution(command, state);
		}
		public void EndCommandExecution(Command command, ICommandUIState state) {
			if (BaseService != null)
				BaseService.EndCommandExecution(command, state);
		}
		#endregion
	}
	public class ImeService : IImeService {
		ImeController controller;
		public ImeService(RichEditControl control) {
			this.controller = control.ImeController;
		}
		public ImeController Controller { get { return controller; } }
		#region IImeService Members
		public bool IsActive { get { return controller.IsActive; } }
#if !SL
		public bool TrackMessage(ref System.Windows.Forms.Message m) {
			return false;
		}
#endif
		public void Close() {
			Controller.Close();
		}
		public void Cancel() {
			Controller.Cancel();
		}
		public RunInfo GetImeStringRange() {
			return Controller.GetImeStringRange();
		}
		#endregion
	}
	public class IMEKeyboardHandler : NormalKeyboardHandler {
		RichEditControl control;
		public IMEKeyboardHandler(RichEditControl control) {
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		public override bool HandleKeyPress(char character, PlatformIndependentKeys modifier) {
#if !SL
			return true;
#else
			return base.HandleKeyPress(character, modifier);
#endif
		}
		public void Initialize() {
			this.KeyHandlerIdTable.Clear();
			control.InnerControl.InitializeKeyboardHandlerDefaults(this);
		}
	}
	public class IMEMouseHandler : RichEditMouseHandler {
		public IMEMouseHandler(RichEditControl control)
			: base(control) {
		}
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			IImeService imeService = Control.GetService(typeof(IImeService)) as ImeService;
			if (imeService != null) {
				imeService.Close();
				return;
			}
			base.OnMouseDown(e);
		}
	}
	public class ImeHelper {
		public static void DisableIme(RichEditControl control) {
			control.ImeController.Enabled = false;
		}
		public static void EnableIme(RichEditControl control) {
			control.ImeController.Enabled = true;
		}
	}
}
