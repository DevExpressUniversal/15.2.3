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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Keyboard;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Export.Xl;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet {
	public enum CellEditorCommitMode {
		Auto,
		Always,
		ChangedOnly
	}
	#region CellEditorMode
	public enum CellEditorMode {
		Edit,
		Enter,
		EditInFormulaBar
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InnerCellInplaceEditor
	public partial class InnerCellInplaceEditor : ICellInplaceEditorCommitControllerOwner  {
		#region Fields
		public const int CornerFormulaRangeResizWidthInPixels = 6;
		readonly InnerSpreadsheetControl control;
		readonly CellInplaceEditorKeyboardHandler editModeKeyboardHandler;
		readonly CellInplaceEditorKeyboardHandler enterModeKeyboardHandler;
		readonly InplaceEditorMouseHandler mouseHandler;
		HotZoneCollection hotZones;
		ICellInplaceEditor editor;
		CellRange cellRange;
		CellEditorMode mode;
		bool isActive;
		bool isModalMessageShown;
		bool shouldCalculateRanges;
		FormulaReferencedRanges referencedRanges;
		bool isEditReferenceMode;
		#endregion
		public InnerCellInplaceEditor(InnerSpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.editModeKeyboardHandler = CreateKeyboardHandler();
			this.enterModeKeyboardHandler = CreateKeyboardHandler();
			this.mouseHandler = new InplaceEditorMouseHandler(control.Owner, this);
			this.hotZones = new HotZoneCollection();
			mouseHandler.Initialize();
			SpreadsheetKeyHashProvider provider = new SpreadsheetKeyHashProvider();
			InitializeEditModeKeyboardHandler(this.editModeKeyboardHandler, provider);
			InitializeEnterModeKeyboardHandler(this.enterModeKeyboardHandler, provider);
			isEditReferenceMode = false;
		}
		#region Properties
		public InnerSpreadsheetControl Control { get { return control; } }
		public bool IsActive { get { return isActive; } }
		public CellEditorMode Mode { get { return mode; } }
		CellInplaceEditorKeyboardHandler KeyboardHandler { get { return Mode == CellEditorMode.Enter ? enterModeKeyboardHandler : editModeKeyboardHandler; } }
		public CellPosition CellPosition { get { return cellRange.TopLeft; } }
		public CellRange CellRange { get { return cellRange; } }
		public FormulaReferencedRanges ReferencedRanges {
			get {
				if (!shouldCalculateRanges)
					return new FormulaReferencedRanges();
				if (this.referencedRanges == null)
					this.referencedRanges = CalculateReferencedRanges();
				return referencedRanges;
			}
		}
		public bool IsModalMessageShown { get { return isModalMessageShown; } }
		public HotZoneCollection HotZones {
			get {
				if (hotZones == null)
					hotZones = CalculateHotZones();
				return hotZones;
			}
		}
		public InplaceEditorMouseHandler MouseHandler { get { return mouseHandler; } }
		public bool IsEditReferenceMode { get { return isEditReferenceMode; } }
		public int SelectionStart { get { return editor.SelectionStart; } }
		public int SelectionLength { get { return editor.SelectionLength; } }
		public string Text { get { return editor.Text; } }
		#endregion
		CellInplaceEditorKeyboardHandler CreateKeyboardHandler() {
			return new CellInplaceEditorKeyboardHandler();
		}
		protected internal virtual void InitializeEditModeKeyboardHandler(CellInplaceEditorKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.ForceHandleKeyAgainShortcuts.Clear();
			keyboardHandler.ForceHandleKeyAgainShortcuts.Add(Keys.Enter | Keys.None);
			keyboardHandler.ForceHandleKeyAgainShortcuts.Add(Keys.Enter | Keys.Shift);
			keyboardHandler.ForceHandleKeyAgainShortcuts.Add(Keys.Tab | Keys.None);
			keyboardHandler.ForceHandleKeyAgainShortcuts.Add(Keys.Tab | Keys.Shift);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.None, SpreadsheetCommandId.InplaceEndEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Shift, SpreadsheetCommandId.InplaceEndEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control, SpreadsheetCommandId.InplaceEndEditEnterToMultipleCells);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control | Keys.Shift, SpreadsheetCommandId.InplaceEndEditEnterArrayFormula);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, SpreadsheetCommandId.InplaceCancelEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F2, Keys.None, SpreadsheetCommandId.InplaceToggleEditMode);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Shift, SpreadsheetCommandId.InsertFunction);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.None, SpreadsheetCommandId.InplaceEndEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.Shift, SpreadsheetCommandId.InplaceEndEdit);
		}
		protected internal virtual void InitializeEnterModeKeyboardHandler(CellInplaceEditorKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			control.AppendSelectionKeyboardShortcutsSharedWithInplaceEditor(keyboardHandler, provider);
			keyboardHandler.UpdateForceHandleKeyAgainShortcuts();
			keyboardHandler.ReplaceAllCommandIdsWith(SpreadsheetCommandId.InplaceEndEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control, SpreadsheetCommandId.InplaceEndEditEnterToMultipleCells);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control | Keys.Shift, SpreadsheetCommandId.InplaceEndEditEnterArrayFormula);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F2, Keys.None, SpreadsheetCommandId.InplaceToggleEditMode);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, SpreadsheetCommandId.InplaceCancelEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Shift, SpreadsheetCommandId.InsertFunction);
		}
		public void ToggleEditMode() {
			control.RestoreKeyboardHandler();
			if (mode == CellEditorMode.Enter)
				mode = CellEditorMode.Edit;
			else
				mode = CellEditorMode.Enter;
			control.SetNewKeyboardHandler(KeyboardHandler);
		}
		public void ActivateAt(CellPosition cellPosition, string text, CellEditorMode mode) {
			ActivateAt(cellPosition, text, -1, mode);
		}
		public void ActivateAt(CellPosition cellPosition, string text, int caretPosition, CellEditorMode mode) {
			if (IsActive) {
				UnsubscribeEditorEvents();
				try {
					InsertEditorText(text, caretPosition);
				}
				finally {
					SubscribeEditorEvents();
				}
				return;
			}
			control.DocumentModel.ClearCopiedRange();
			this.cellRange = control.DocumentModel.ActiveSheet.Selection.GetActualCellRange(cellPosition);
			this.editor = CreateEditor(mode == CellEditorMode.EditInFormulaBar);
			SetupEditor(ObtainCell(), text, caretPosition);
			this.isActive = true;
			ActivateEditor();
			SetFocusToEditor();
			SubscribeEditorEvents();
			this.mode = GetActualMode(mode);
			control.SetNewKeyboardHandler(KeyboardHandler);
			control.MouseHandlers.Push(mouseHandler);
			this.isModalMessageShown = false;
			this.isEditReferenceMode = false;
			this.shouldCalculateRanges = true;
			ResetReferencedRanges();
			control.RaiseUpdateUI();
			control.Owner.Redraw();
		}
		CellEditorMode GetActualMode(CellEditorMode mode) {
			if (mode == CellEditorMode.EditInFormulaBar)
				return CellEditorMode.Edit;
			return mode;
		}
		void ActivateEditor() {
			this.editor.Activate();
		}
		internal void SetFocusToEditor() {
			if (editor != null)
				editor.SetFocus();
		}
		void SetupEditor(ICell cell, string text, int caretPosition) {
			IActualRunFontInfo fontInfo = cell.ActualFont;
			this.editor.ForeColor = Cell.GetTextColor(control.DocumentModel, fontInfo);
			Color backColor = cell.ActualBackgroundColor;
			if (DXColor.IsTransparentOrEmpty(backColor))
				backColor = this.Control.DocumentModel.SkinBackColor;
			if (DXColor.IsTransparentOrEmpty(backColor))
				backColor = DXColor.White;
			this.editor.BackColor = backColor;
			if (cell.HasFormula && cell.ActualAlignment.Horizontal == XlHorizontalAlignment.General)
				this.editor.SetHorizontalAlignment(XlHorizontalAlignment.Left);
			else
				this.editor.SetHorizontalAlignment(cell.ActualHorizontalAlignment);
			this.editor.SetVerticalAlignment(cell.ActualAlignment.Vertical);
			this.editor.WrapText = cell.ActualAlignment.WrapText;
			UpdateFont(cell);
			text = CellInplaceEditorHelper.CalculateEditorText(text, cell);
			SetupEditorText(text, caretPosition);
			this.editor.IsVisible = true;
		}
		void SetupEditorText(string text, int caretPosition) {
			this.editor.Text = text;
			UpdateBounds();
			if (IsPercentage(text) && caretPosition < 0)
				caretPosition = 1;
			this.editor.SetSelection((caretPosition < 0 ? editor.Text.Length : caretPosition), 0);
		}
		bool IsPercentage(string text) {
			if (string.IsNullOrEmpty(text) || text.Length != 2)
				return false;
			return Char.IsDigit(text[0]) && text[1] == '%';
		}
		void InsertEditorText(string text, int caretPosition) {
			if (caretPosition < 0 || caretPosition == int.MaxValue) {
				if (String.IsNullOrEmpty(text))
					caretPosition = 0;
				else
					caretPosition = text.Length;
			}
			int selectionStart = editor.SelectionStart;
			string editorText = editor.Text;
			if (!String.IsNullOrEmpty(editorText)) {
				if (!String.IsNullOrEmpty(text)) {
					if (text[0] == '=') {
						text = text.Substring(1);
						caretPosition = Math.Max(0, caretPosition - 1);
					}
				}
			}
			string beforeSelection = editorText.Substring(0, selectionStart);
			int selectionEnd = Math.Min(editorText.Length, selectionStart + editor.SelectionLength);
			string afterSelection = editorText.Substring(selectionEnd);
			this.editor.Text = beforeSelection + text + afterSelection;
			UpdateBounds();
			this.editor.SetSelection(selectionStart + caretPosition, 0);
		}
		public void UpdateBoundsAndFont() {
			if (!IsActive)
				return;
			UpdateFont(ObtainCell());
			UpdateBounds();
		}
		void UpdateBounds() {
			InplaceEditorBoundsInfo bounds = CalculateEditorBounds();
			if (bounds == null) {
				this.editor.IsVisible = false;
			}
			else {
				SpreadsheetView view = control.ActiveView;
				bounds.HostCellBounds = view.GetPhysicalPixelRectangle(bounds.HostCellBounds);
				bounds.EditorBounds = view.GetPhysicalPixelRectangle(bounds.EditorBounds);
				this.editor.SetBounds(bounds);
				this.editor.IsVisible = true;
			}
		}
		void UpdateFont(ICell cell) {
			this.editor.SetFont(cell.ActualFont.GetFontInfo(), control.ActiveView.ZoomFactor);
		}
		ICell ObtainCell() {
			Worksheet sheet = control.DocumentModel.ActiveSheet;
			return sheet.GetCellForFormatting(CellPosition.Column, CellPosition.Row);
		}
		public DialogResult ShowDataValidationDialog(string text, string message, string title, DataValidationErrorStyle errorStyle) {
			DialogResult dialogResult = DialogResult.Cancel;
			editor.Text = text;
			if (Mode == CellEditorMode.Enter)
				ToggleEditMode();
			isModalMessageShown = true;
			UnsubscribeEditorEvents();
			try {
				dialogResult = this.Control.Owner.ShowDataValidationDialog(message, title, errorStyle);
				isModalMessageShown = false;
				SetFocusToEditor();
			}
			finally {
				SubscribeEditorEvents();
			}
			return dialogResult;
		}
		public bool Commit(bool fillSelection, bool enterArrayFormula) {
			if (editor == null)
				return false;
			if (isModalMessageShown)
				return false;
			string text = editor.Text;
			if (text == null)
				text = String.Empty;
			CellInplaceEditorCommitController controller = new CellInplaceEditorCommitController(this, ObtainCell());
			controller.FillSelection = fillSelection;
			controller.EnterArrayFormula = enterArrayFormula;
			UnsubscribeEditorEvents(); 
			try {
				CellInplaceEditorCommitResult result = controller.Commit(text);
				string message = result.Message;
				if (!String.IsNullOrEmpty(message)) {
					if (result.AttemptToChangeLockedCells) {
						if (!Control.RaiseProtectionWarning())
							InvokeShowMessage(text, message);
					}
					else
						InvokeShowMessage(text, message);
				}
				return result.Success;
			}
			catch (InvalidOperationException exception) {
				ProcessInvalidOperation(text, exception.Message);
				return false;
			}
			finally {
				SubscribeEditorEvents();
			}
		}
		public bool CanClose() {
			if (editor == null)
				return true;
			string text = editor.Text;
			if (text == null)
				text = String.Empty;
			UnsubscribeEditorEvents();
			try {
				CellEndEditResult cellEndEditResult = control.RaiseCellEndEdit(control.DocumentModel.ActiveSheet, CellPosition.Column, CellPosition.Row, text);
				text = cellEndEditResult.Text;
				if (text == null)
					text = String.Empty;
				return !cellEndEditResult.Cancelled;
			}
			finally {
				editor.Text = text;
				SubscribeEditorEvents();
			}
		}
		public bool CanAddFormulaRange() {
			if (editor == null)
				return false;
			if (isEditReferenceMode)
				return true;
			string text = editor.Text;
			if (String.IsNullOrEmpty(text))
				return false;
			char firstSymbol = text[0];
			if (firstSymbol != '=')
				return false;
			int cursorPosition = editor.SelectionStart;
			if (cursorPosition <= 0)
				return false;
			char previousSymbol = text[cursorPosition - 1];
			return IsSymbolAllowToAddFormulaRange(previousSymbol);
		}
		bool IsSymbolAllowToAddFormulaRange(char symbol) {
			const string symbols = "=(.,:;+-*/^!<>& ";
			return symbols.IndexOf(symbol) >= 0;
		}
		public int AddReferencedRange(Worksheet sheet, CellPosition cellPosition) {
			if (!cellPosition.IsValid)
				return -1;
			CellRangeBase cellRange = new CellRange(sheet, cellPosition, cellPosition) as CellRangeBase;
			FormulaReferencedRange formulaReferencedRange = new FormulaReferencedRange(cellRange, editor.SelectionStart, editor.SelectionLength, false);
			ReferencedRanges.Add(formulaReferencedRange);
			int referencedRangeIndex = ReferencedRanges.Count - 1;
			ReplaceRangeText(referencedRangeIndex, false);
			return referencedRangeIndex;
		}
		public void RemoveSelectedRanges() {
			if (editor.SelectionLength <= 0)
				return;
			List<int> rangeInSelectionIndexes = new List<int>();
			for (int count = 0; count < ReferencedRanges.Count; count++) {
				FormulaReferencedRange formulaRange = ReferencedRanges[count];
				int selectionStart = editor.SelectionStart;
				int selectionEnd = selectionStart + editor.SelectionLength;
				if (formulaRange.Position >= editor.SelectionStart && formulaRange.Position <= selectionEnd - formulaRange.Length) {
					rangeInSelectionIndexes.Add(count);
				}
			}
			foreach (int index in rangeInSelectionIndexes)
				ReferencedRanges.RemoveAt(index);
		}
		public void Rollback() {
			if (editor == null)
				return;
			editor.Rollback();
		}
		public void Deactivate(bool commit) {
			if (editor == null)
				return;
			control.MouseHandlers.Pop();
			mouseHandler.SwitchToDefaultState();
			control.RestoreKeyboardHandler();
			UnsubscribeEditorEvents();
			this.editor.Close();
			this.editor.Deactivate();
			this.editor = null;
			this.isActive = false;
			if (!commit)
				control.RaiseCellCancelEdit(control.DocumentModel.ActiveSheet, CellPosition.Column, CellPosition.Row);
			control.RaiseUpdateUI();
			control.Owner.Redraw();
		}
		public void DeactivateHandlers() {
			if (control.MouseHandlers.Count > 1 && control.MouseHandlers.Peek() == mouseHandler) {
				control.MouseHandlers.Pop();
				control.KeyboardHandlers.Pop();
			}
			ResetReferencedRanges();
			shouldCalculateRanges = false;
		}
		public void ActivateHandlers() {
			control.SetNewKeyboardHandler(KeyboardHandler);
			control.MouseHandlers.Push(mouseHandler);
			shouldCalculateRanges = true;
		}
		void ProcessInvalidOperation(string text, string message) {
			InvokeShowMessage(text, message);
		}
		void InvokeShowMessage(string text, string message) {
			editor.Text = text;
			if (Mode == CellEditorMode.Enter)
				ToggleEditMode();
			IThreadSyncService service = Control.GetService<IThreadSyncService>();
			if (service != null) {
				isModalMessageShown = true;
				service.EnqueueInvokeInUIThread(new Action(delegate() { ShowMessage(message); }));
			}
		}
		void ShowMessage(string message) {
			if (editor == null) {
				this.Control.Owner.ShowWarningMessage(message);
				return;
			}
			UnsubscribeEditorEvents();
			try {
				this.Control.Owner.ShowWarningMessage(message);
				isModalMessageShown = false;
				SetFocusToEditor();
			}
			finally {
				SubscribeEditorEvents();
			}
		}
		void OnTextChanged(object sender, EventArgs e) {
			UpdateBounds();
			RemoveEditReferenceMode();
			FormulaReferencedRanges previousReferencedRanges = referencedRanges;
			ResetReferencedRanges();
			if (ShouldRedrawReferencedRanges() || (previousReferencedRanges != null && previousReferencedRanges.Count > 0)) {
				if (!CompareReferencedRanges(previousReferencedRanges, ReferencedRanges))
					this.Control.Owner.Redraw();
			}
		}
		bool CompareReferencedRanges(FormulaReferencedRanges list1, FormulaReferencedRanges list2) {
			if (list1 == null)
				return list2 == null || list2.Count == 0;
			else {
				if (list2 == null)
					return list1.Count == 0;
			}
			int list1Count = list1.Count;
			int list2Count = list2.Count;
			if (list1Count != list2Count)
				return false;
			for (int i = 0; i < list1Count; i++) {
				FormulaReferencedRange range1 = list1[i];
				FormulaReferencedRange range2 = list2[i];
				if (!range1.Equals(range2))
					return false;
			}
			return true;
		}
		bool ShouldRedrawReferencedRanges() {
			return FormulaBase.IsFormula(this.Text);
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if (control != null && !control.IsDisposed)
				control.OnUpdateUI();
		}
		public void InputPercentage() {
			if (this.editor == null)
				return;
			string editorText = this.editor.Text;
			if (!string.IsNullOrEmpty(editorText) && editorText.Length == 2 && (editorText[0] == '+' || editorText[0] == '-') && Char.IsDigit(editorText[1])) {
				ICell cell = ObtainCell();
				string formatCode = cell.ActualFormat.FormatCode.Trim();
				if (formatCode.EndsWith("%") && !formatCode.EndsWith("\\%")) {
					this.editor.Text = editorText + "%";
					this.editor.SetSelection(2, 0);
				}
			}
		}
		FormulaReferencedRanges CalculateReferencedRanges() {
			ReferencedRangesCalculator calculator = new ReferencedRangesCalculator(this.Control.DocumentModel);
			return calculator.Calculate(editor.Text, CellPosition);
		}
		void ResetReferencedRanges() {
			this.referencedRanges = null;
			this.hotZones = null;
		}
		HotZoneCollection CalculateHotZones() {
			FormulaReferencedRanges referencedRanges = this.ReferencedRanges;
			HotZoneCollection hotZones = new HotZoneCollection();
			int count = referencedRanges.Count;
			for (int i = 0; i < count; i++) {
				List<HotZone> zones = CreateHotZones(referencedRanges[i], i);
				if (zones != null)
					hotZones.AddRange(zones);
			}
			return hotZones;
		}
		List<HotZone> CreateHotZones(FormulaReferencedRange formulaReferencedRange, int referencedRangeIndex) {
			if (formulaReferencedRange.IsReadOnly)
				return null;
			CellRange range = formulaReferencedRange.CellRange as CellRange;
			if (range == null)
				return null;
			if (range.Worksheet != Control.DocumentModel.ActiveSheet)
				return null;
			List<HotZone> result = new List<HotZone>();
			IList<Page> pages = Control.DesignDocumentLayout.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				Rectangle bounds = pages[i].CalculateRangeBounds(range);
				if (bounds != Rectangle.Empty) {
					AddResizeHotZones(bounds, i, result, referencedRangeIndex);
					AddDragHotZones(bounds, i, result, referencedRangeIndex);
				}
			}
			return result;
		}
		void AddResizeHotZones(Rectangle bounds, int pageIndex, List<HotZone> result, int referencedRangeIndex) {
			int hotZoneOffset = CornerFormulaRangeResizWidthInPixels / 2;
			int left = bounds.Left - 1;
			int right = bounds.Right - 2;
			int top = bounds.Top - 1;
			int bottom = bounds.Bottom - 2;
			result.Add(CreateFormulaRangeResizeHotZone(Rectangle.FromLTRB(left - hotZoneOffset, top - hotZoneOffset, left + hotZoneOffset, top + hotZoneOffset), pageIndex, referencedRangeIndex, FormulaRangeAnchor.TopLeft));
			result.Add(CreateFormulaRangeResizeHotZone(Rectangle.FromLTRB(right - hotZoneOffset, top - hotZoneOffset, right + hotZoneOffset, top + hotZoneOffset), pageIndex, referencedRangeIndex, FormulaRangeAnchor.TopRight));
			result.Add(CreateFormulaRangeResizeHotZone(Rectangle.FromLTRB(left - hotZoneOffset, bottom - hotZoneOffset, left + hotZoneOffset, bottom + hotZoneOffset), pageIndex, referencedRangeIndex, FormulaRangeAnchor.BottomLeft));
			result.Add(CreateFormulaRangeResizeHotZone(Rectangle.FromLTRB(right - hotZoneOffset, bottom - hotZoneOffset, right + hotZoneOffset, bottom + hotZoneOffset), pageIndex, referencedRangeIndex, FormulaRangeAnchor.BottomRight));
		}
		FormulaRangeResizeHotZone CreateFormulaRangeResizeHotZone(Rectangle bounds, int pageIndex, int referencedRangeIndex, FormulaRangeAnchor anchor) {
			FormulaRangeResizeHotZone result = new FormulaRangeResizeHotZone(this.Control, pageIndex, referencedRangeIndex, anchor);
			result.Bounds = bounds;
			return result;
		}
		void AddDragHotZones(Rectangle bounds, int pageIndex, List<HotZone> result, int referencedRangeIndex) {
			int hotZoneOffset = RangeBorderSelectionLayoutItem.HotZoneOffset;
			int left = bounds.Left - 1;
			int right = bounds.Right - 2;
			int top = bounds.Top - 1;
			int bottom = bounds.Bottom - 2;
			result.Add(CreateFormulaRangeDragHotZone(Rectangle.FromLTRB(left + hotZoneOffset, top - hotZoneOffset, right - hotZoneOffset, top + hotZoneOffset), pageIndex, referencedRangeIndex)); 
			result.Add(CreateFormulaRangeDragHotZone(Rectangle.FromLTRB(left + hotZoneOffset, bottom - hotZoneOffset, right - hotZoneOffset, bottom + hotZoneOffset), pageIndex, referencedRangeIndex)); 
			result.Add(CreateFormulaRangeDragHotZone(Rectangle.FromLTRB(left - hotZoneOffset, top - hotZoneOffset, left + hotZoneOffset, bottom + hotZoneOffset), pageIndex, referencedRangeIndex)); 
			result.Add(CreateFormulaRangeDragHotZone(Rectangle.FromLTRB(right - hotZoneOffset, top - hotZoneOffset, right + hotZoneOffset, bottom - hotZoneOffset), pageIndex, referencedRangeIndex)); 
		}
		FormulaRangeDragHotZone CreateFormulaRangeDragHotZone(Rectangle bounds, int pageIndex, int referencedRangeIndex) {
			FormulaRangeDragHotZone result = new FormulaRangeDragHotZone(this.Control, pageIndex, referencedRangeIndex);
			result.Bounds = bounds;
			return result;
		}
		protected internal virtual void SubscribeEditorEvents() {
			editor.EditorTextChanged += OnTextChanged;
			editor.EditorSelectionChanged += OnSelectionChanged;
		}
		protected internal virtual void UnsubscribeEditorEvents() {
			editor.EditorTextChanged -= OnTextChanged;
			editor.EditorSelectionChanged -= OnSelectionChanged;
		}
		ICellInplaceEditor CreateEditor(bool formulaBarFocused) {
			return control.CreateCellInplaceEditor(formulaBarFocused);
		}
		InplaceEditorBoundsInfo CalculateEditorBounds() {
			DocumentLayout layout = control.DesignDocumentLayout;
			IList<Page> pages = layout.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				InplaceEditorBoundsInfo bounds = CalculateEditorBounds(pages[i]);
				if (bounds != null)
					return bounds;
			}
			return null;
		}
		Rectangle CalculateHostCellBounds(Page page) {
			CellPosition topLeft = cellRange.TopLeft;
			int topLeftColumnGridIndex = page.GridColumns.CalculateExactOrFarItem(topLeft.Column);
			int topLeftRowGridIndex = page.GridRows.CalculateExactOrFarItem(topLeft.Row);
			CellPosition bottomRight = cellRange.BottomRight;
			int bottomRightColumnGridIndex = page.GridColumns.CalculateExactOrNearItem(bottomRight.Column);
			int bottomRightRowGridIndex = page.GridRows.CalculateExactOrNearItem(bottomRight.Row);
			if (topLeft.EqualsPosition(bottomRight))
				if (topLeftColumnGridIndex < page.GridColumns.ActualFirstIndex
					|| topLeftColumnGridIndex > page.GridColumns.ActualLastIndex)
					return Rectangle.Empty;
			if (bottomRightRowGridIndex < 0 || bottomRightColumnGridIndex < 0)
				return Rectangle.Empty;
			if (topLeftColumnGridIndex < 0 && bottomRightColumnGridIndex < 0)
				return Rectangle.Empty;
			if (topLeftRowGridIndex < 0 && bottomRightRowGridIndex < 0)
				return Rectangle.Empty;
			if (topLeftColumnGridIndex < 0)
				topLeftColumnGridIndex = page.GridColumns.ActualFirstIndex;
			if (bottomRightColumnGridIndex < 0)
				bottomRightColumnGridIndex = page.GridColumns.ActualLastIndex;
			if (topLeftRowGridIndex < 0)
				topLeftRowGridIndex = page.GridRows.ActualFirstIndex;
			if (bottomRightRowGridIndex < 0)
				bottomRightRowGridIndex = page.GridRows.ActualLastIndex;
			if (bottomRightColumnGridIndex < topLeftColumnGridIndex)
				return Rectangle.Empty;
			if (bottomRightRowGridIndex < topLeftRowGridIndex)
				return Rectangle.Empty;
			PageGridItem topLeftColumnGridItem = page.GridColumns[topLeftColumnGridIndex];
			PageGridItem topLeftRowGridItem = page.GridRows[topLeftRowGridIndex];
			PageGridItem bottomRightColumnGridItem = page.GridColumns[bottomRightColumnGridIndex];
			PageGridItem bottomRightRowGridItem = page.GridRows[bottomRightRowGridIndex];
			return Rectangle.FromLTRB(topLeftColumnGridItem.Near, topLeftRowGridItem.Near, bottomRightColumnGridItem.Far, bottomRightRowGridItem.Far);
		}
		InplaceEditorBoundsInfo CalculateEditorBounds(Page page) {
			Rectangle hostCellBounds = CalculateHostCellBounds(page);
			if (hostCellBounds == Rectangle.Empty)
				return null;
			InplaceEditorBoundsInfo result = new InplaceEditorBoundsInfo();
			result.HostCellBounds = hostCellBounds;
			Rectangle bounds = hostCellBounds;
			ICell cell = ObtainCell();
			IActualCellAlignmentInfo actualAlign = cell.ActualAlignment;
			if (!actualAlign.WrapText && !actualAlign.ShrinkToFit) {
				DocumentLayoutCalculator calculator = new DocumentLayoutCalculator(Control.InnerDocumentLayout, Control.DocumentModel.ActiveSheet, Control.Owner.LayoutViewBounds, Control.ActiveView.ZoomFactor);
				Rectangle availableTextBounds = bounds;
				availableTextBounds.Width = Int32.MaxValue;
				int textWidth = calculator.CalculateCellSingleLineTextWidth(cell, page, availableTextBounds);
				if (textWidth > bounds.Width) {
					XlHorizontalAlignment horizontalAlignment = actualAlign.Horizontal;
					if (horizontalAlignment == XlHorizontalAlignment.Right) {
						bounds = AdjustBoundsLeft(bounds, page.GridColumns, bounds.Right - textWidth);
					}
					else if (horizontalAlignment == XlHorizontalAlignment.Center || horizontalAlignment == XlHorizontalAlignment.CenterContinuous) {
						bounds = AdjustBoundsLeft(bounds, page.GridColumns, bounds.Left - (textWidth - bounds.Width) / 2);
						bounds = AdjustBoundsRight(bounds, page.GridColumns, bounds.Right + (textWidth - bounds.Width) / 2);
					}
					else {
						bounds = AdjustBoundsRight(bounds, page.GridColumns, bounds.X + textWidth);
					}
				}
			}
			bounds = CellTextBoxBase.GetTextBounds(cell, bounds, page, Control.InnerDocumentLayout, false);
			result.EditorBounds = bounds;
			return result;
		}
		Rectangle AdjustBoundsLeft(Rectangle bounds, PageGrid columnsGrid, int left) {
			int columnGridIndex = columnsGrid.LookupItemIndexByPosition(left);
			if (columnGridIndex < 0)
				columnGridIndex = 0;
			return Rectangle.FromLTRB(columnsGrid[columnGridIndex].Near, bounds.Top, bounds.Right, bounds.Bottom);
		}
		Rectangle AdjustBoundsRight(Rectangle bounds, PageGrid columnsGrid, int right) {
			int columnGridIndex = columnsGrid.LookupItemIndexByPosition(right);
			if (columnGridIndex < 0)
				columnGridIndex = columnsGrid.Count - 1;
			return Rectangle.FromLTRB(bounds.Left, bounds.Top, columnsGrid[columnGridIndex].Far, bounds.Bottom);
		}
		public void OnMouseWheel(MouseEventArgs e) {
			if (isModalMessageShown)
				return;
			mouseHandler.OnMouseWheel(e);
		}
		public void OnMouseClick(MouseEventArgs e) {
			isEditReferenceMode = false;
		}
		void ICellInplaceEditorCommitControllerOwner.RaiseCellValueChanged(SpreadsheetCellEventArgs args) {
			control.RaiseCellValueChanged(args);
		}
		void ModifyReferenceType() {
			WorkbookDataContext context = this.Control.DocumentModel.DataContext;
			ReferenceTypeModifier modifier = new ReferenceTypeModifier();
			ReferenceTypeInfo info = new ReferenceTypeInfo(context);
			info.Text = this.editor.Text;
			if (this.editor.SelectionLength < 0) {
				info.SelectionStart = this.editor.SelectionStart + this.editor.SelectionLength;
				info.SelectionLength = -this.editor.SelectionLength;
			}
			else {
				info.SelectionStart = this.editor.SelectionStart;
				info.SelectionLength = this.editor.SelectionLength;
			}
			info.Ranges = this.CalculateReferencedRanges();
			ReferenceTypeInfo result;
			context.PushCurrentCell(CellPosition);
			try {
				result = modifier.ModifyReferenceType(info);
			}
			finally {
				context.PopCurrentCell();
			}
			if (result == null || !result.IsChanged)
				return;
			this.editor.Text = result.Text;
			this.editor.SetSelection(result.SelectionStart, result.SelectionLength);
		}
		public virtual HotZone CalculateHotZone(Point point) {
			return HotZoneCalculator.CalculateHotZone(HotZones, point, Control.ActiveView.ZoomFactor, Control.DocumentModel.LayoutUnitConverter);
		}
		public void ReplaceRangeText(int rangeIndex, bool detectIsPositionByRange) {
			ReferenceTypeModifier modifier = new ReferenceTypeModifier();
			string newText = modifier.ReplaceRangeText(this.editor.Text, Control.DocumentModel.DataContext, ReferencedRanges, rangeIndex, detectIsPositionByRange);
			this.editor.Text = newText;
			this.editor.SetSelection(newText.Length, 0);
			SetFocusToEditor();
			Control.Owner.Redraw();
		}
		public void ReplaceRangeText(FormulaReferencedRange range, bool detectIsPositionByRange) {
			WorkbookDataContext context = Control.DocumentModel.DataContext;
			ReferenceTypeModifier modifier = new ReferenceTypeModifier();
			string newText = modifier.ReplaceRangeText(this.editor.Text, context, range, detectIsPositionByRange);
			this.editor.Text = newText;
			int newPosition = range.Position + range.CellRange.ToString(context).Length;
			this.editor.SetSelection(newPosition, 0);
			SetFocusToEditor();
		}
		public void SetEditReferenceMode() {
			isEditReferenceMode = true;
		}
		public void RemoveEditReferenceMode() {
			isEditReferenceMode = false;
		}
		public void BeginMultiSelection() {
			int selectionStart = editor.SelectionStart;
			string text = editor.Text;
			if (selectionStart >= 0 && selectionStart <= text.Length) {
				if (ShouldInsertSeparator(text, selectionStart - 1)) {
					text = text.Insert(selectionStart, new string(this.Control.DocumentModel.DataContext.GetListSeparator(), 1));
					editor.Text = text;
					editor.SetSelection(selectionStart + 1, 0);
				}
			}
			isEditReferenceMode = false;
		}
		bool ShouldInsertSeparator(string text, int position) {
			if (position < 0)
				return false;
			for (; position >= 0; position--) {
				if (text[position] != ' ')
					break;
			}
			return !IsSymbolAllowToAddFormulaRange(text[position]);
		}
		public void Cut() {
			if (editor != null)
				editor.Cut();
		}
		public void Copy() {
			if (editor != null)
				editor.Copy();
		}
		public void Paste() {
			if (editor != null)
				editor.Paste();
		}
		public bool CanCopy() {
			if (editor == null)
				return false;
			return editor.SelectionLength != 0;
		}
		public void InsertFunction(string function, int selectionStart, int selectionLength) {
			editor.SetSelection(selectionStart, selectionLength);
			if (String.IsNullOrEmpty(function))
				return;
			string result = String.Empty;
			if (String.IsNullOrEmpty(editor.Text) || editor.Text[0] != '=') {
				if (!String.IsNullOrEmpty(editor.Text))
					result = "=";
				result += "=" + function + "()";
				editor.SetSelection(0, editor.Text.Length);
			}
			else {
				const string chars = "=!^&*(+:<>,./ ";
				if (SelectionStart == 0)
					result = "==";
				else if (chars.IndexOf(editor.Text[SelectionStart - 1]) == -1)
					result = "+";
				result += function + "()";
			}
			InsertEditorText(result, result.Length - 1);
		}
		public void InsertFunction(string function) {
			int selectionStart = SelectionStart;
			editor.Text = function;
			editor.SetSelection(selectionStart, 0);
		}
	}
	#endregion
	#region ReferencedRangesCalculator
	public class ReferencedRangesCalculator {
		readonly DocumentModel documentModel;
		static readonly List<Color> referencedRangesColors = CreateReferencedRangesColorTable();
		static List<Color> CreateReferencedRangesColorTable() {
			List<Color> result = new List<Color>();
			result.Add(DXColor.FromArgb(95, 140, 237));
			result.Add(DXColor.FromArgb(235, 94, 96));
			result.Add(DXColor.FromArgb(141, 97, 194));
			result.Add(DXColor.FromArgb(45, 150, 57));
			result.Add(DXColor.FromArgb(191, 76, 145));
			result.Add(DXColor.FromArgb(227, 130, 34));
			result.Add(DXColor.FromArgb(55, 127, 158));
			return result;
		}
		public static Color CalculateReferencedRangeColor(int index) {
			return referencedRangesColors[index % referencedRangesColors.Count];
		}
		public ReferencedRangesCalculator(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public FormulaReferencedRanges Calculate(string text, CellPosition position) {
			WorkbookDataContext context = documentModel.DataContext;
			IncompleteExpressionParserContext parserContext;
			FormulaReferencedRanges result = null;
			context.PushCurrentCell(position);
			try {
				parserContext = context.ExpressionParser.ParseIncomplete(text, OperandDataType.Value);
				if (parserContext == null)
					return new FormulaReferencedRanges();
				result = parserContext.GetReferencedRanges();
			}
			finally {
				context.PopCurrentCell();
			}
			AssignColors(result);
			return result;
		}
		void AssignColors(FormulaReferencedRanges referencedRanges) {
			List<CellRangeBase> processedRanges = new List<CellRangeBase>();
			int count = referencedRanges.Count;
			int colorIndex = 0;
			for (int i = 0; i < count; i++) {
				FormulaReferencedRange referencedRange = referencedRanges[i];
				int index = processedRanges.IndexOf(referencedRange.CellRange);
				if (index >= 0)
					referencedRange.Color = CalculateReferencedRangeColor(index);
				else {
					referencedRange.Color = CalculateReferencedRangeColor(colorIndex);
					processedRanges.Add(referencedRange.CellRange);
					colorIndex++;
				}
			}
		}
	}
	#endregion
	#region CellInplaceEditorCommitResult
	public class CellInplaceEditorCommitResult {
		public CellInplaceEditorCommitResult(bool success, string message) {
			this.Success = success;
			this.Message = message;
		}
		public bool Success { get; set; }
		public string Message { get; set; }
		public bool AttemptToChangeLockedCells { get; set; }
	}
	#endregion
	public interface ICellInplaceEditorCommitControllerOwner {
		void RaiseCellValueChanged(SpreadsheetCellEventArgs args);
		DialogResult ShowDataValidationDialog(string text, string message, string title, DataValidationErrorStyle errorStyle);
	}
	#region CellInplaceEditorCommitController
	public class CellInplaceEditorCommitController {
		#region Fields
		readonly ICellInplaceEditorCommitControllerOwner owner;
		readonly DocumentModel documentModel;
		readonly ICell cell;
		#endregion
		public CellInplaceEditorCommitController(ICellInplaceEditorCommitControllerOwner owner, ICell cell) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(cell, "cell");
			this.owner = owner;
			this.cell = cell;
			this.documentModel = cell.Worksheet.Workbook;
		}
		#region Properties
		public ICellInplaceEditorCommitControllerOwner Owner { get { return owner; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public ICell Cell { get { return cell; } }
		public bool FillSelection { get; set; }
		public bool EnterArrayFormula { get; set; }
		public CellPosition CellPosition { get { return Cell.Position; } }
		#endregion
		public CellInplaceEditorCommitResult Commit(string text) {
			bool isFormula = FormulaBase.IsFormula(text);
			if (isFormula) {
				text = PrepareFormula(text);
				if (String.IsNullOrEmpty(text))
					return new CellInplaceEditorCommitResult(false, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFormula));
			}
			FormattedInplaceEditorValue value = new FormattedInplaceEditorValue();
			value.Text = text;
			value.IsFormula = isFormula;
			if (isFormula)
				value.Formula = text;
			InplaceEditorCommitStrategy commitStrategy = CreateCommitStrategy(value, FillSelection, EnterArrayFormula);
			bool commitResult = false;
			bool documentModelAlreadyContainsCircular = DocumentModel.HasCircularReferences;
			DocumentModel.BeginUpdate();
			try {
				commitResult = commitStrategy.Commit(value);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			if (commitResult)
				return CommitCore(commitStrategy, documentModelAlreadyContainsCircular);
			return new CellInplaceEditorCommitResult(false, String.Empty);
		}
		public void CommitSingleCell(string text) {
			FormattedInplaceEditorValue value = new FormattedInplaceEditorValue();
			value.Text = text;
			InplaceEditorCommitStrategy commitStrategy = new SingleCellValueInplaceEditorCommitStrategy(owner, DocumentModel.ActiveSheet, CellPosition);
			if (commitStrategy.Commit(value))
				CommitCore(commitStrategy);
		}
		CellInplaceEditorCommitResult CommitCore(InplaceEditorCommitStrategy commitStrategy, bool documentModelAlreadyContainsCircular) {
			CommitCore(commitStrategy);
			if (!documentModelAlreadyContainsCircular && (commitStrategy.CircularReferenceFound || DocumentModel.HasCircularReferences))
				if (!DocumentModel.Properties.CalculationOptions.IterationsEnabled)
					return new CellInplaceEditorCommitResult(true, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CircularReference));
			if (commitStrategy.AttemptToChangeLockedCells)
				return new CellInplaceEditorCommitResult(true, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CellOrChartIsReadonly)) { AttemptToChangeLockedCells = true };
			return new CellInplaceEditorCommitResult(true, String.Empty);
		}
		void CommitCore(InplaceEditorCommitStrategy commitStrategy) {
			IList<CellContentSnapshot> affectedCells = commitStrategy.AffectedCells;
			foreach (CellContentSnapshot snapshot in affectedCells) {
				SpreadsheetCellEventArgs args = new SpreadsheetCellEventArgs(snapshot);
				Owner.RaiseCellValueChanged(args);
			}
		}
		string PrepareFormula(string text) {
			WorkbookDataContext context = DocumentModel.DataContext;
			context.PushCurrentCell(Cell);
			try {
				IncompleteExpressionParserContext parserContext = context.ExpressionParser.ParseIncomplete(text, OperandDataType.Value);
				if (!parserContext.ParsingSuccessful) {
					string repairedText = context.ExpressionParser.TryRepairFormula(text, OperandDataType.Value, parserContext);
					if (StringExtensions.CompareInvariantCultureIgnoreCase(repairedText, text) == 0)
						return null;
					else
						return repairedText;
				}
			}
			finally {
				context.PopCurrentCell();
			}
			return text;
		}
		InplaceEditorCommitStrategy CreateCommitStrategy(FormattedInplaceEditorValue value, bool fillSelection, bool enterArrayFormula) {
			Worksheet sheet = DocumentModel.ActiveSheet;
			CellRangeBase selection = sheet.Selection.AsRange();
			if ((fillSelection && selection.CellCount > 1) || enterArrayFormula) {
				if (value.IsFormula) {
					if (enterArrayFormula)
						return new MultipleCellsArrayFormulaInplaceEditorCommitStrategy(owner, sheet, CellPosition);
					else
						return new MultipleCellsFormulaInplaceEditorCommitStrategy(owner, sheet, CellPosition);
				}
				else
					return new MultipleCellsValueInplaceEditorCommitStrategy(owner, sheet, CellPosition);
			}
			else {
				if (value.IsFormula)
					return new SingleCellFormulaInplaceEditorCommitStrategy(owner, sheet, CellPosition);
				else
					return new SingleCellValueInplaceEditorCommitStrategy(owner, sheet, CellPosition);
			}
		}
	}
	#endregion
	#region CellInplaceEditorHelper
	public static class CellInplaceEditorHelper {
		public static string CalculateEditorText(string text, ICell cell) {
			string formatCode = cell.ActualFormat.FormatCode.Trim();
			bool isPercentage = formatCode.EndsWith("%") && !formatCode.EndsWith("\\%");
			if (text != null) {
				if (text.Length == 1 && Char.IsDigit(text[0]) && isPercentage)
					text += '%';
				return text;
			}
			if (cell.Worksheet.Properties.Protection.SheetLocked && cell.ActualProtection.Hidden)
				return String.Empty;
			if (cell.HasFormula)
				return cell.FormulaBody;
			if (cell.HasError)
				return CellErrorFactory.GetErrorName(cell.Error, cell.Context);
			else {
				VariantValue value = cell.Value;
				if (!value.IsNumeric)
					return CalculateDefaultText(value, cell);
				if (cell.ActualFormat.IsDateTime)
					return CalculateDateTimeText(value, cell);
				else if (isPercentage) {
					string displayText = cell.Text;
					if (displayText.Trim().EndsWith("%"))
						return (value.NumericValue * 100).ToString(cell.Context.Culture) + '%';
					else
						return CalculateDefaultText(value, cell);
				}
				else
					return CalculateDefaultText(value, cell);
			}
		}
		static string CalculateDefaultText(VariantValue value, ICell cell) {
			string text = value.ToText(cell.Context).InlineTextValue;
			if (cell.FormatInfo.QuotePrefix)
				text = text.Insert(0, "'");
			return text;
		}
		static string CalculateDateTimeText(VariantValue value, ICell cell) {
			try {
				if (WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue, cell.Context.DateSystem))
					return CalculateSimpleTextValue(value, cell.Context);
				bool isTimeFormat = cell.ActualFormat.IsTime;
				DateTime dateTime = value.ToDateTime(cell.Context);
				DateTimeFormatInfo dateTimeFormat = cell.Context.Culture.DateTimeFormat;
				if (Is1900BaseDate(value, cell) || IsLeapYearBug(value, cell)) {
					if (isTimeFormat)
						return dateTime.ToString(dateTimeFormat.LongTimePattern);
					string result = ShortDateNumberFormat.Instance.Format((int)value.NumericValue, cell.Context, NumberFormatParameters.Empty).Text;
					if (dateTime != dateTime.Date)
						result += " " + dateTime.ToString(dateTimeFormat.LongTimePattern);
					return result;
				}
				if (dateTime == dateTime.Date)
					return dateTime.ToString(dateTimeFormat.ShortDatePattern);
				if (value.NumericValue < 1 && isTimeFormat) 
					return dateTime.ToString(dateTimeFormat.LongTimePattern);
				return dateTime.ToString(dateTimeFormat.ShortDatePattern + "  " + dateTimeFormat.LongTimePattern);
			}
			catch {
				return CalculateSimpleTextValue(value, cell.Context);
			}
		}
		static bool IsLeapYearBug(VariantValue value, ICell cell) {
			if (cell.Context.DateSystem != DateSystem.Date1900)
				return false;
			return (int)value.NumericValue == 60;
		}
		static bool Is1900BaseDate(VariantValue value, ICell cell) {
			if (cell.Context.DateSystem != DateSystem.Date1900)
				return false;
			return (int)value.NumericValue == 0;
		}
		static string CalculateSimpleTextValue(VariantValue value, WorkbookDataContext context) {
			return value.ToText(context).GetTextValue(context.StringTable);
		}
	}
	#endregion
	#region InplaceEditorBoundsInfo
	public class InplaceEditorBoundsInfo {
		public Page Page { get; set; }
		public Rectangle HostCellBounds { get; set; }
		public Rectangle EditorBounds { get; set; }
	}
	#endregion
	#region ReferenceTypeInfo
	public class ReferenceTypeInfo {
		readonly WorkbookDataContext dataContext;
		public ReferenceTypeInfo(WorkbookDataContext dataContext) {
			this.dataContext = dataContext;
		}
		public WorkbookDataContext DataContext { get { return dataContext; } }
		public string Text { get; set; }
		public int SelectionStart { get; set; }
		public int SelectionEnd { get; set; }
		public int SelectionLength {
			get { return SelectionEnd - SelectionStart; }
			set { SelectionEnd = SelectionStart + value; }
		}
		public FormulaReferencedRanges Ranges { get; set; }
		public bool IsChanged { get; set; }
	}
	#endregion
	#region ReferenceTypeModifier
	public class ReferenceTypeModifier {
		public ReferenceTypeInfo ModifyReferenceType(ReferenceTypeInfo info) {
			ReferenceTypeInfo result = new ReferenceTypeInfo(info.DataContext);
			result.Ranges = info.Ranges;
			result.Text = info.Text;
			result.SelectionStart = info.SelectionStart;
			result.SelectionEnd = info.SelectionEnd;
			if (info.Ranges == null || info.Ranges.Count <= 0)
				return result;
			List<int> rangeIndices = CalculateAffectedRangeIndices(info);
			if (rangeIndices == null || rangeIndices.Count <= 0)
				return result;
			bool selectionAfterText = result.SelectionLength == 0 && result.SelectionStart >= result.Text.Length;
			CellPosition initialPosition = CalculateInitialPosition(info, rangeIndices[0]);
			bool isIntervalRange = info.Ranges[rangeIndices[0]].CellRange is CellIntervalRange;
			PositionType columnType = initialPosition.ColumnType;
			PositionType rowType = initialPosition.RowType;
			PositionType newColumnType = CalculateNextColumnType(columnType, rowType, isIntervalRange);
			PositionType newRowType = CalculateNextRowType(columnType, rowType, isIntervalRange);
			ModifyRanges(result, rangeIndices, newColumnType, newRowType);
			if (result.IsChanged) {
				ReplaceText(result, rangeIndices);
				if (selectionAfterText) {
					result.SelectionStart = result.Text.Length;
					result.SelectionEnd = result.SelectionStart;
				}
			}
			return result;
		}
		PositionType CalculateNextColumnType(PositionType columnType, PositionType rowType, bool isIntervalRange) {
			if (columnType == PositionType.Relative)
				return PositionType.Absolute;
			else
				return PositionType.Relative;
		}
		PositionType CalculateNextRowType(PositionType columnType, PositionType rowType, bool isIntervalRange) {
			if (isIntervalRange) {
				if (rowType == PositionType.Relative)
					return PositionType.Absolute;
				else
					return PositionType.Relative;
			}
			else {
				if (columnType == rowType)
					return PositionType.Absolute;
				else
					return PositionType.Relative;
			}
		}
		List<int> CalculateAffectedRangeIndices(ReferenceTypeInfo info) {
			List<int> result = new List<int>();
			FormulaReferencedRanges ranges = info.Ranges;
			int count = ranges.Count;
			for (int i = 0; i < count; i++) {
				if (ShouldAddRange(info, ranges[i]))
					result.Add(i);
			}
			return result;
		}
		bool IsRangeIntersectsWithSelection(ReferenceTypeInfo info, int start, int length) {
			if (info.SelectionLength == 0) {
				if (start <= info.SelectionEnd && start + length >= info.SelectionStart)
					return true;
			}
			else {
				if (start < info.SelectionEnd && start + length > info.SelectionStart)
					return true;
			}
			return false;
		}
		bool ShouldAddRange(ReferenceTypeInfo info, FormulaReferencedRange range) {
			if (range.IsReadOnly || range.CellRange is CellUnion)
				return false;
			return IsRangeIntersectsWithSelection(info, range.Position, range.Length);
		}
		CellPosition CalculateInitialPosition(ReferenceTypeInfo info, int rangeIndex) {
			FormulaReferencedRange range = info.Ranges[rangeIndex];
			string text = info.Text.Substring(range.Position, range.Length);
			int colonIndex = text.LastIndexOf(':');
			if (colonIndex < 0)
				return range.CellRange.TopLeft;
			if (IsRangeIntersectsWithSelection(info, 0, colonIndex + 1))
				return range.CellRange.TopLeft;
			else
				return range.CellRange.BottomRight;
		}
		void ModifyRanges(ReferenceTypeInfo info, List<int> rangeIndices, PositionType columnType, PositionType rowType) {
			for (int i = rangeIndices.Count - 1; i >= 0; i--) {
				if (ModifyRange(info, info.Ranges[rangeIndices[i]], columnType, rowType))
					info.IsChanged = true;
			}
		}
		bool ModifyRange(ReferenceTypeInfo info, FormulaReferencedRange range, PositionType columnType, PositionType rowType) {
			CellRangeBase cellRange = range.CellRange;
			CellPosition topLeft = cellRange.TopLeft;
			CellPosition bottomRight = cellRange.BottomRight;
			bool result = !(topLeft.ColumnType == columnType && topLeft.RowType == rowType && bottomRight.ColumnType == columnType && bottomRight.RowType == rowType);
			if (!result)
				return result;
			string text = info.Text.Substring(range.Position, range.Length);
			int sheetNameSeparatorIndex = text.LastIndexOf('!');
			if (sheetNameSeparatorIndex >= 0 && !IsRangeIntersectsWithSelection(info, range.Position + sheetNameSeparatorIndex + 1, text.Length - sheetNameSeparatorIndex - 1))
				return false; 
			int topLeftIndex = sheetNameSeparatorIndex >= 0 ? sheetNameSeparatorIndex + 1 : 0;
			int colonPosition = text.LastIndexOf(':');
			if (colonPosition > sheetNameSeparatorIndex) { 
				bool topLeftAffected = IsRangeIntersectsWithSelection(info, range.Position + topLeftIndex, colonPosition - topLeftIndex);
				bool bottomRightAffected = IsRangeIntersectsWithSelection(info, range.Position + colonPosition + 1, text.Length - colonPosition - 1);
				if (!topLeftAffected && !bottomRightAffected) {
					topLeftAffected = true;
					bottomRightAffected = true;
				}
				if (cellRange is CellIntervalRange && (topLeftAffected || bottomRightAffected)) {
					topLeftAffected = true;
					bottomRightAffected = true;
				}
				if (topLeftAffected) {
					info.SelectionStart = Math.Min(info.SelectionStart, range.Position + topLeftIndex);
					info.SelectionEnd = Math.Max(info.SelectionEnd, range.Position + colonPosition);
					cellRange.TopLeft = new CellPosition(topLeft.Column, topLeft.Row, columnType, rowType);
				}
				if (bottomRightAffected) {
					info.SelectionStart = Math.Min(info.SelectionStart, range.Position + colonPosition + 1);
					info.SelectionEnd = Math.Max(info.SelectionEnd, range.Position + range.Length);
					cellRange.BottomRight = new CellPosition(bottomRight.Column, bottomRight.Row, columnType, rowType);
				}
			}
			else {
				info.SelectionStart = Math.Min(info.SelectionStart, range.Position + topLeftIndex);
				info.SelectionEnd = Math.Max(info.SelectionEnd, range.Position + range.Length);
				cellRange.TopLeft = new CellPosition(topLeft.Column, topLeft.Row, columnType, rowType);
				cellRange.BottomRight = new CellPosition(bottomRight.Column, bottomRight.Row, columnType, rowType);
			}
			return result;
		}
		void ReplaceText(ReferenceTypeInfo info, List<int> rangeIndices) {
			int count = rangeIndices.Count;
			string text = info.Text;
			for (int i = count - 1; i >= 0; i--) {
				text = ReplaceRangeText(text, info.DataContext, info.Ranges, rangeIndices[i], false);
			}
			info.SelectionLength += text.Length - info.Text.Length;
			info.Text = text;
		}
		public string ReplaceRangeText(string formulaText, WorkbookDataContext context, FormulaReferencedRanges ranges, int rangeIndex, bool detectIsPositionByRange) {
			FormulaReferencedRange range = ranges[rangeIndex];
			return ReplaceRangeText(formulaText, context, range, detectIsPositionByRange);
		}
		public string ReplaceRangeText(string formulaText, WorkbookDataContext context, FormulaReferencedRange range, bool detectIsPositionByRange) {
			string newText = CalculateNewText(formulaText, context, range, detectIsPositionByRange);
			formulaText = formulaText.Remove(range.Position, range.Length);
			formulaText = formulaText.Insert(range.Position, newText);
			return formulaText;
		}
		string CalculateNewText(string formulaText, WorkbookDataContext context, FormulaReferencedRange range, bool detectIsPositionByRange) {
			string text = formulaText.Substring(range.Position, range.Length);
			int sheetNameSeparatorIndex = text.LastIndexOf('!');
			string prefix;
			if (sheetNameSeparatorIndex >= 0) {
				prefix = text.Substring(0, sheetNameSeparatorIndex + 1);
				text = text.Substring(sheetNameSeparatorIndex + 1);
			}
			else
				prefix = String.Empty;
			bool isPosition;
			if (detectIsPositionByRange)
				isPosition = range.CellRange.Width == 1 && range.CellRange.Height == 1;
			else
				isPosition = !(range.CellRange is CellIntervalRange) && (text.IndexOf(':') < 0);
			if (isPosition)
				text = range.CellRange.TopLeft.ToString(context);
			else
				text = range.CellRange.ToString(context);
			if (!String.IsNullOrEmpty(prefix))
				text = prefix + text;
			return text;
		}
	}
	#endregion
}
