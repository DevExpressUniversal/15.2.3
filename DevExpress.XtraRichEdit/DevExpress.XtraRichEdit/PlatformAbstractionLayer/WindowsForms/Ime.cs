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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Keyboard;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.PInvoke;
using DevExpress.XtraRichEdit.Localization;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Utils {
	public static class ImeHelper {
		[System.Security.SecuritySafeCritical]
		public static bool IsImeMessage(ref Message msg) {
			return (msg.Msg & (Win32.WM_IME_STARTCOMPOSITION | Win32.WM_IME_COMPOSITION | Win32.WM_IME_ENDCOMPOSITION)) != 0;
		}
	}
	public class ImeController {
		ImeCompositionInfo compositionInfo;
		RichEditControl control;
		IMEKeyboardHandler keyboardHandler;
		IMEMouseHandler mouseHandler;
		ImeCommandExecutionListenerService commandExecutionListenerService;
		bool isActive;
		ICommandExecutionListenerService oldCommandExecutionListenerService;
		public ImeController(RichEditControl control) {
			this.compositionInfo = new ImeCompositionInfo();
			this.control = control;
			this.keyboardHandler = new IMEKeyboardHandler(control);
			this.mouseHandler = new IMEMouseHandler(control);
		}
		ImeCompositionInfo CompositionInfo { get { return compositionInfo; } }
		DocumentModel DocumentModel { get { return control.DocumentModel; } }
		public RichEditControl Control { get { return control; } }
		IntPtr WindowHandle { get { return control.Handle; } }
		IMEKeyboardHandler KeyboardHandler { get { return keyboardHandler; } }
		IMEMouseHandler MouseHandler { get { return mouseHandler; } }
		public bool IsActive { get { return isActive; } }
		ImeCommandExecutionListenerService CommandExecutionListenerService { get { return commandExecutionListenerService; } }
		[System.Security.SecuritySafeCritical]
		public bool TrackMessage(ref Message m) {
			switch(m.Msg) {
				case Win32.WM_IME_STARTCOMPOSITION:
					OnStartComposition();
					return true; 
				case Win32.WM_IME_COMPOSITION:
					int lParam = (int)m.LParam;
					OnComposition(lParam);
					return true; 
				case Win32.WM_IME_ENDCOMPOSITION:
					OnEndComposition();
					break;
			}
			return false;
		}
		void OnStartComposition() {
			Debug.WriteLine("->OnStartComposition");
			BeginImeInput();
			CompositionInfo.Reset(DocumentModel);
			ResetMerging();
			DeleteNonEmptySelectionCommand deleteCommand = (DeleteNonEmptySelectionCommand)Control.CreateCommand(RichEditCommandId.DeleteNonEmptySelection);
			if(deleteCommand.CanExecute())
				deleteCommand.Execute();
		}
		void OnEndComposition() {
			Debug.WriteLine("->OnEndComposition");
			EndImeInput();
		}
		void BeginImeInput() {
			if(IsActive)
				return;
			Debug.WriteLine("->BeginImeInput");
			this.isActive = true;
			RegisterImeCommandExecutionListenerService();
			CaptureUserInput();
		}
		void EndImeInput() {
			if(!IsActive)
				return;
			Debug.WriteLine("->EndImeInput");
			this.isActive = false;
			UnregisterImeCommandExecutionListenerService();
			ReleaseUserInput();
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
		void OnComposition(int compositionState) {
			Debug.WriteLine("->OnComposition");
			BeginImeInput();
			CommandExecutionListenerService.AllowCloseIme = false;
			Control.BeginUpdate();
			if(CompositionInfo.Length > 0) {
				Undo();
			}
			if ((compositionState & Win32.GcsFlags.GCS_RESULTSTR) != 0) {
				Debug.WriteLine("   :GCS_RESULTSTR");
				InsertCompositionResultText();
				EndImeInput();
			}
			if ((compositionState & Win32.GcsFlags.GCS_COMPSTR) != 0) {
				BeginImeInput(); 
				Debug.WriteLine("   :GCS_COMPSTR");
				InsertCompositionText();
			}
			if ((compositionState & Win32.GcsFlags.GCS_CURSORPOS) != 0) {
				BeginImeInput();
				Debug.WriteLine("   :GCS_CURSORPOS");
				MoveCaret();
			}
			Control.EndUpdate();
			SetCandidateWindowPosition();
			CommandExecutionListenerService.AllowCloseIme = true;
		}
		void MoveCaret() {
			DocumentModel.BeginUpdate();
			int relativePosition = Math.Min(CompositionInfo.Length, GetImeCompositionCursorPosition());
			DocumentLogPosition caretLogPosition = CompositionInfo.StartPos + relativePosition;
			Control.DocumentModel.Selection.Start = caretLogPosition;
			Control.DocumentModel.Selection.End = caretLogPosition;
			DocumentModel.EndUpdate();
			EnsureCaretVisibleHorizontallyCommand ensureCaretVisibleHorizontallyCommand = new EnsureCaretVisibleHorizontallyCommand(Control);
			EnsureCaretVisibleVerticallyCommand ensureCaretVisibleVerticallyCommand = new EnsureCaretVisibleVerticallyCommand(Control);
			ensureCaretVisibleHorizontallyCommand.Execute();
			ensureCaretVisibleVerticallyCommand.Execute();
		}
		void Undo() {
			InputPosition pos = Control.ActiveView.CaretPosition.GetInputPosition();			
			ImeUndoCommand undoCommand = (ImeUndoCommand)Control.CreateCommand(RichEditCommandId.ImeUndo);
			undoCommand.Execute();
			if (pos != null)
				Control.ActiveView.CaretPosition.GetInputPosition().CopyFormattingFrom(pos);
		}
		void InsertCompositionText() {
			string imeString = GetImeString(Win32.GcsFlags.GCS_COMPSTR);
			InsertText(imeString);
			CompositionInfo.Length = imeString.Length;
		}
		void InsertCompositionResultText() {
			string imeString = GetImeString(Win32.GcsFlags.GCS_RESULTSTR);
			InsertText(imeString);
			CompositionInfo.Reset(DocumentModel);
			ResetMerging();
		}
		void InsertText(string imeString) {
			if(!String.IsNullOrEmpty(imeString)) {
				IInsertTextCommand command = (IInsertTextCommand)Control.CreateCommand(RichEditCommandId.InsertText);
				command.Text = imeString;
				command.Execute();
			}
		}
		void ResetMerging() {
			ResetMergingCommand command = (ResetMergingCommand)Control.CreateCommand(RichEditCommandId.ResetMerging);
			command.Execute();
		}
		string GetImeString(int index) {
			IntPtr hImc = Win32.ImmGetContext(WindowHandle);
			String result = String.Empty;
			try {
				int compositionStringCount = Win32.ImmGetCompositionStringW(hImc, index, null, 0);
				byte[] compositionStringByteArray = new byte[compositionStringCount * 2];
				Win32.ImmGetCompositionStringW(hImc, index, compositionStringByteArray, compositionStringCount);
				result = Encoding.Unicode.GetString(compositionStringByteArray, 0, compositionStringCount);
			}
			finally {
				Win32.ImmReleaseContext(this.WindowHandle, hImc);
			}
			return result;
		}
		int GetImeCompositionCursorPosition() {
			IntPtr hImc = Win32.ImmGetContext(WindowHandle);
			try {
				return Win32.ImmGetCompositionStringW(hImc, Win32.GcsFlags.GCS_CURSORPOS, null, 0);
			}
			finally {
				Win32.ImmReleaseContext(WindowHandle, hImc);
			}
		}
		void CaptureUserInput() {
			MouseHandler.Initialize();
			KeyboardHandler.Initialize();
			Control.InnerControl.SetNewKeyboardHandler(KeyboardHandler);
			Control.InnerControl.SetNewMouseHandler(MouseHandler);
		}
		void ReleaseUserInput() {
			Debug.Assert(Object.Equals(Control.InnerControl.KeyboardHandler, KeyboardHandler));
			Debug.Assert(Object.Equals(Control.InnerControl.MouseHandler, MouseHandler));
			if(Object.Equals(Control.InnerControl.KeyboardHandler, KeyboardHandler))
				Control.InnerControl.RestoreKeyboardHandler();
			if (Object.Equals(Control.InnerControl.MouseHandler, MouseHandler))
				Control.InnerControl.RestoreMouseHandler();
		}
		void SetCandidateWindowPosition() {
			Win32.CANDIDATEFORM candidateForm = new Win32.CANDIDATEFORM();
			candidateForm.dwStyle = Win32.CfsFlags.CFS_POINT;
			Rectangle caretBounds = Control.ActiveView.GetCursorBounds();
			candidateForm.ptCurrentPos = caretBounds.Location + new Size(0, caretBounds.Height);
			Win32.CANDIDATEFORM candidateFormEx = new Win32.CANDIDATEFORM();
			candidateFormEx.dwStyle = Win32.CfsFlags.CFS_EXCLUDE;
			candidateFormEx.rcArea = caretBounds;
			candidateFormEx.ptCurrentPos = caretBounds.Location + new Size(0, caretBounds.Height);
			IntPtr hIMC = Win32.ImmGetContext(WindowHandle);
			if(hIMC == IntPtr.Zero)
				return;
			try {
				Win32.ImmSetCandidateWindow(hIMC, ref candidateForm);
				Win32.ImmSetCandidateWindow(hIMC, ref candidateFormEx);
			}
			finally {
				Win32.ImmReleaseContext(WindowHandle, hIMC);
			}
		}
		public RunInfo GetImeStringRange() {
			return CompositionInfo.GetRunInfo(DocumentModel);
		}
		public void Close() {
			IntPtr hImc = Win32.ImmGetContext(WindowHandle);
			try {
				Win32.ImmNotifyIME(hImc, Win32.NI_COMPOSITIONSTR, Win32.CpsFlags.CPS_COMPLETE, 0);
			}
			finally {
				Win32.ImmReleaseContext(WindowHandle, hImc);
			}
		}
		public void Cancel() {
			IntPtr hImc = Win32.ImmGetContext(WindowHandle);
			try {
				Win32.ImmNotifyIME(hImc, Win32.NI_COMPOSITIONSTR, Win32.CpsFlags.CPS_CANCEL, 0);
			}
			finally {
				Win32.ImmReleaseContext(WindowHandle, hImc);
			}
		}
	}
	#region ImeBoxExporter
	public class ImeBoxExporter : UnderlineBoxExporter<UnderlineBox> {
		readonly IImeService imeService;
		readonly RunInfo imeStringRunInfo;
		readonly UnderlineDotted underlineDotted;
		public ImeBoxExporter(NotPrintableGraphicsBoxExporter exporter, IImeService imeService)
			: base(exporter) {
			Guard.ArgumentNotNull(imeService, "imeService");
			this.imeService = imeService;
			if (ImeService.IsActive)
				this.imeStringRunInfo = ImeService.GetImeStringRange();
			this.underlineDotted = new UnderlineDotted();
		}
		protected internal override Color UnderlineColor { get { return DXColor.Black; } }
		RunInfo ImeStringRunInfo { get { return imeStringRunInfo; } }
		IImeService ImeService { get { return imeService; } }
		public virtual void Export(Row row) {
			if (!ImeService.IsActive || ImeStringRunInfo == null)
				return;
			ImeUnderlineCalculator calculator = new ImeUnderlineCalculator(row.Paragraph.PieceTable, ImeStringRunInfo.NormalizedStart.RunIndex, ImeStringRunInfo.NormalizedEnd.RunIndex);
			calculator.Calculate(row);
			ExportTo(calculator.UnderlineBoxesByType);
		}
		protected override RichEditPatternLinePainter CreateCharacterLinePainter() {
			return new DevExpress.XtraRichEdit.Design.Internal.ScreenCharacterLinePainter(Exporter.Painter, Exporter.UnitConverter);
		}
		protected override void DrawUnderlineByLines(RectangleF bounds, Pen pen) {
			LinePainter.DrawUnderline(underlineDotted, bounds, pen.Color);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Mouse {
#if !SL
	#region IMEMouseHandler
	public class IMEMouseHandler : RichEditMouseHandler {
		public IMEMouseHandler(RichEditControl control)
			: base(control) {
		}
		public override void OnMouseDown(MouseEventArgs e) {
			IImeService imeService = Control.GetService(typeof(IImeService)) as ImeService;
			if (imeService != null) {
				imeService.Close();
			}
			base.OnMouseDown(e);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}
	}
	#endregion
#endif
}
namespace DevExpress.XtraRichEdit.Keyboard {
	public class IMEKeyboardHandler : NormalKeyboardHandler {
		RichEditControl control;
		public IMEKeyboardHandler(RichEditControl control) {
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		public override bool HandleKeyPress(char character, Keys modifier) {
			return true;
		}
		public void Initialize() {
			this.KeyHandlerIdTable.Clear();
			control.InnerControl.InitializeKeyboardHandlerDefaults(this);
		}
	}
	public class ImeUnderlineCalculator : UnderlineCalculator {		
		readonly RunIndex startRunIndex;
		readonly RunIndex endRunIndex;
		public ImeUnderlineCalculator(PieceTable pieceTable, RunIndex startRunIndex, RunIndex endRunIndex)
			: base(pieceTable) {
			this.startRunIndex = startRunIndex;
			this.endRunIndex = endRunIndex;
		}
		protected internal override void BeforeCalculate(Row row) {
			base.BeforeCalculate(row);			
		}
		protected internal override void AfterCalculate() {
			base.AfterCalculate();
			UnderlineBoxesByType.ForEach(SetUnderlineBoxBounds);
		}
		protected internal override UnderlineType GetRunCharacterLineType(TextRun run, RunIndex runIndex) {
			if (runIndex >= startRunIndex && runIndex <= endRunIndex)
				return UnderlineType.Dotted;
			else
				return UnderlineType.None;
		}
		protected internal override bool GetRunCharacterLineUseForWordsOnly(TextRun run, RunIndex runIndex) {
			return false;
		}
	}
}
namespace DevExpress.Services.Implementation {
	public class ImeCommandExecutionListenerService : ICommandExecutionListenerService {
		ICommandExecutionListenerService baseService;
		ImeController imeController;
		bool allowCloseIme;
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
			if(BaseService != null)
				BaseService.EndCommandExecution(command, state);
		}
		#endregion
	}   
}
namespace DevExpress.XtraRichEdit {
	public enum ImeCloseStatus { ImeCompositionCancel, ImeCompositionComplete };
}
