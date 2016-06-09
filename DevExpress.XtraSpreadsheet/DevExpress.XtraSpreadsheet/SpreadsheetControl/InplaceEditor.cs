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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.Office.PInvoke;
using System.Diagnostics;
using System.Text;
#if SL || WPF
using DevExpress.Xpf.Spreadsheet.Internal;
#else
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.Office.Layout;
#endif
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl {
		[Browsable(false)]
		public bool IsCellEditorActive { get { return InnerControl != null ? InnerControl.IsInplaceEditorActive : false; } }
		public bool OpenCellEditor(CellEditorMode mode) {
			if (InnerControl != null)
				InnerControl.OpenInplaceEditor(mode);
			return IsCellEditorActive;
		}
		public bool CloseCellEditor(CellEditorEnterValueMode mode) {
			if (InnerControl != null)
				return InnerControl.CloseInplaceEditor(mode);
			else
				return false;
		}
		ICellInplaceEditor IInnerSpreadsheetControlOwner.CreateCellInplaceEditor(bool formulaBarShouldFocused) {
			return this.CreateCellInplaceEditor(formulaBarShouldFocused);
		}
		protected internal virtual ICellInplaceEditor CreateCellInplaceEditor(bool formulaBarShouldFocused) {
			CompositeCellInplaceEditor compositeEditor = new CompositeCellInplaceEditor();
#if SL || WPF
			XpfCellInplaceEditor editor = GetInplaceEditor();
			IFormulaBarControl formulaBar = GetService<IFormulaBarControl>();
			if (formulaBar != null)
				compositeEditor.Add(formulaBar.InplaceEditor);
			if (formulaBarShouldFocused)
				formulaBar.InplaceEditor.CurrentEditable = true;
			else
				((ICellInplaceEditor)editor).CurrentEditable = true;
			compositeEditor.Add(editor);
#else
			IFormulaBarControl formulaBar = GetService<IFormulaBarControl>();
			if (formulaBar != null)
				compositeEditor.Add(formulaBar.InplaceEditor);
			WinFormsCellInplaceEditor editor = new WinFormsCellInplaceEditor(this);
			compositeEditor.Add(editor);
			if (formulaBarShouldFocused)
				formulaBar.InplaceEditor.CurrentEditable = true;
			else
				((ICellInplaceEditor)editor).CurrentEditable = true;
#endif
			return compositeEditor;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Internal {
#if SL || WPF
#else
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	Designer("DevExpress.XtraSpreadsheet.Design.XtraSpreadsheetDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign)
	]
	public class WinFormsCellInplaceEditor : Control, ICellInplaceEditor {
		TextBoxWithTransparency textBox;
		readonly SpreadsheetControl control;
		readonly InnerCellInplaceEditor innerEditor;
		readonly Dictionary<int, Font> cachedFonts = new Dictionary<int, Font>();
		XlVerticalAlignment verticalAlignment;
		int previousSelectionStart = 0;
		int previousSelectionLength = 0;
		public WinFormsCellInplaceEditor(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.innerEditor = control.InnerControl.InplaceEditor;
			this.textBox = new TextBoxWithTransparency();
			this.ImeMode = control.ImeMode;
			textBox.Visible = false;
			textBox.BorderStyle = BorderStyle.None;
			textBox.ScrollBars = ScrollBars.None;
			textBox.Multiline = true;
			textBox.AutoSize = false;
			textBox.WordWrap = true;
			textBox.AcceptsReturn = false;
			textBox.AcceptsTab = true;
			SubscribeTextBoxEvents();
			this.Controls.Add(textBox);
			textBox.Parent = this;
			control.Controls.Add(this);
			this.Parent = control;
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		#region ICellInplaceEditor implementation
		bool ICellInplaceEditor.IsVisible { get { return this.Visible; } set { this.Visible = value; textBox.Visible = value; } }
		string ICellInplaceEditor.Text { get { return textBox.Text; } set { if (textBox != null) textBox.Text = value; } }
		Color ICellInplaceEditor.ForeColor { get { return textBox.ForeColor; } set { textBox.ForeColor = value; } }
		Color ICellInplaceEditor.BackColor { get { return textBox.BackColor; } set { this.BackColor = value; textBox.BackColor = value; } }
		public int SelectionStart { get { return textBox.SelectionStart; } }
		public int SelectionLength { get { return textBox.SelectionLength; } }
		public bool WrapText { get; set; }
		bool ICellInplaceEditor.Focused { get { return textBox.Focused; } }
		bool ICellInplaceEditor.CurrentEditable { get; set; }
		bool ICellInplaceEditor.Registered { get; set; }
		void ICellInplaceEditor.Close() {
			control.Focus(); 
			control.Controls.Remove(this);
			this.Parent = null;
		}
		void ICellInplaceEditor.SetFocus() {
			textBox.Focus();
		}
		void ICellInplaceEditor.SetSelection(int start, int length) {
			textBox.Select(start, length);
		}
		TextChangedEventHandler onEditorTextChanged;
		public event TextChangedEventHandler EditorTextChanged { add { onEditorTextChanged += value; } remove { onEditorTextChanged -= value; } }
		protected internal virtual void RaiseEditorTextChanged() {
			if (onEditorTextChanged != null) {
				TextChangedEventArgs args = new TextChangedEventArgs(textBox.Text);
				onEditorTextChanged(this, args);
			}
		}
		EventHandler onEditorSelectionChanged;
		public event EventHandler EditorSelectionChanged { add { onEditorSelectionChanged += value; } remove { onEditorSelectionChanged -= value; } }
		protected internal virtual void RaiseEditorSelectionChanged() {
			if (onEditorSelectionChanged != null)
				onEditorSelectionChanged(this, new EventArgs());
		}
		#region SetFont
		void ICellInplaceEditor.SetFont(FontInfo fontInfo, float zoomFactor) {
			int hash = zoomFactor.GetHashCode() ^ fontInfo.Font.GetHashCode() ^ fontInfo.Name.GetHashCode();
			Font font;
			if (!cachedFonts.TryGetValue(hash, out font)) {
				Font originalFont = fontInfo.Font;
				font = new Font(originalFont.Name, zoomFactor * fontInfo.SizeInPoints, originalFont.Style, GraphicsUnit.Point, originalFont.GdiCharSet, originalFont.GdiVerticalFont);
				cachedFonts[hash] = font;
			}
			textBox.Font = font;
		}
		void ClearCachedFonts() {
			foreach (int key in cachedFonts.Keys)
				cachedFonts[key].Dispose();
			cachedFonts.Clear();
		}
		#endregion
		#region SetHorizontalAlignment
		void ICellInplaceEditor.SetHorizontalAlignment(XlHorizontalAlignment alignment) {
			textBox.TextAlign = CalculateHorizontalAlignment(alignment);
		}
		HorizontalAlignment CalculateHorizontalAlignment(XlHorizontalAlignment alignment) {
			switch (alignment) {
				default:
				case XlHorizontalAlignment.General:
				case XlHorizontalAlignment.Left:
				case XlHorizontalAlignment.Fill:
				case XlHorizontalAlignment.Justify:
				case XlHorizontalAlignment.Distributed:
					return HorizontalAlignment.Left;
				case XlHorizontalAlignment.Center:
				case XlHorizontalAlignment.CenterContinuous:
					return HorizontalAlignment.Center;
				case XlHorizontalAlignment.Right:
					return HorizontalAlignment.Right;
			}
		}
		#endregion
		#region SetVerticalAlignment
		void ICellInplaceEditor.SetVerticalAlignment(XlVerticalAlignment alignment) {
			this.verticalAlignment = alignment;
		}
		#endregion
		#region SetBounds
		void ICellInplaceEditor.SetBounds(InplaceEditorBoundsInfo boundsInfo) {
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
			boundsInfo.HostCellBounds = unitConverter.LayoutUnitsToPixels(boundsInfo.HostCellBounds, DocumentModel.DpiX, DocumentModel.DpiY);
			boundsInfo.EditorBounds = control.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(boundsInfo.EditorBounds, DocumentModel.DpiX, DocumentModel.DpiY);
			boundsInfo.HostCellBounds = AdjustBounds(boundsInfo.HostCellBounds);
			boundsInfo.EditorBounds = AdjustBounds(boundsInfo.EditorBounds);
			InplaceEditorActualBoundsCalculator boundsCalculator = new InplaceEditorActualBoundsCalculator();
			boundsCalculator.LeftTextMargin = leftTextBoxMargin;
			boundsCalculator.RightTextMargin = rightTextBoxMargin;
			boundsCalculator.WrapText = WrapText;
			boundsCalculator.HorizontalAlignment = textBox.TextAlign;
			boundsCalculator.VerticalAlignment = verticalAlignment;
			boundsCalculator.ControlClientBounds = control.ClientBounds;
			boundsCalculator.Text = textBox.Text + "."; 
			boundsCalculator.Font = textBox.Font;
			boundsCalculator.Calculate(boundsInfo);
			this.Bounds = boundsCalculator.Bounds;
			textBox.Bounds = boundsCalculator.TextBoxBounds;
		}
		Rectangle AdjustBounds(Rectangle bounds) {
			bounds.X -= leftTextBoxMargin;
			bounds.Width += leftTextBoxMargin + rightTextBoxMargin;
			bounds.Width += 1;
			return bounds;
		}
		#endregion
		void ICellInplaceEditor.Activate() {
		}
		void ICellInplaceEditor.Deactivate() {
			this.Dispose();
		}
		void ICellInplaceEditor.Rollback() {
		}
		void ICellInplaceEditor.Copy() {
			if (textBox != null)
				textBox.Copy();
		}
		void ICellInplaceEditor.Cut() {
			if (textBox != null)
				textBox.Cut();
		}
		void ICellInplaceEditor.Paste() {
			if (textBox != null)
				textBox.Paste();
		}
		#endregion
		void SubscribeTextBoxEvents() {
			textBox.KeyDown += OnKeyDown;
			textBox.KeyPress += OnKeyPress;
			textBox.KeyUp += OnKeyUp;
			textBox.HandleCreated += OnHandleCreated;
			textBox.MouseWheel += OnMouseWheel;
			textBox.TextChanged += OnTextChanged;
			textBox.GotFocus += OnTextBoxGotFocus;
			textBox.MouseClick += OnMouseClick;
			Application.Idle += OnApplicationIdle;
		}
		void UnsubscribeTextBoxEvents() {
			textBox.KeyDown -= OnKeyDown;
			textBox.KeyPress -= OnKeyPress;
			textBox.KeyUp -= OnKeyUp;
			textBox.HandleCreated -= OnHandleCreated;
			textBox.MouseWheel -= OnMouseWheel;
			textBox.TextChanged -= OnTextChanged;
			textBox.GotFocus -= OnTextBoxGotFocus;
			textBox.MouseClick -= OnMouseClick;
			Application.Idle -= OnApplicationIdle;
		}
		void OnApplicationIdle(object sender, EventArgs e) {
			if (IsSelectionChanged()) {
				previousSelectionLength = SelectionLength;
				previousSelectionStart = SelectionStart;
				RaiseEditorSelectionChanged();
			}
		}
		bool IsSelectionChanged() {
			if (previousSelectionLength == 0 && SelectionLength == 0)
				return false;
			return previousSelectionLength != SelectionLength || previousSelectionStart != SelectionStart;
		}
		void OnTextBoxGotFocus(object sender, EventArgs e) {
			OnGotFocus(e);
		}
		bool lockTextChanged = false;
		void OnTextChanged(object sender, EventArgs e) {
			if (lockTextChanged)
				return;
			lockTextChanged = true;
			try {
				innerEditor.InputPercentage();
				RaiseEditorTextChanged();
			}
			finally {
				lockTextChanged = false;
			}
		}
		#region Custom word break procedure implementation for TextBox
		delegate int EditWordBreakProc(IntPtr text, int position, int charCount, int action);
		const int leftTextBoxMargin = 1;
		const int rightTextBoxMargin = 1;
		void OnHandleCreated(object sender, EventArgs e) {
			const int EM_SETMARGINS = 0x00D3;
			const int EC_LEFTMARGIN = 0x0001;
			const int EC_RIGHTMARGIN = 0x0002;
			IntPtr marginValue = new IntPtr(leftTextBoxMargin | (rightTextBoxMargin << 16));
			DevExpress.Office.PInvoke.Win32.SendMessage(textBox.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, marginValue);
		}
		#endregion
		void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Return) {
				textBox.SelectedText = Environment.NewLine;
				e.SuppressKeyPress = true;
				e.Handled = true;
				return;
			}
			innerEditor.OnKeyDown(e);
			e.SuppressKeyPress = (e.KeyCode == Keys.Return);
		}
		void OnKeyUp(object sender, KeyEventArgs e) {
			innerEditor.OnKeyUp(e);
		}
		void OnKeyPress(object sender, KeyPressEventArgs e) {
			innerEditor.OnKeyPress(e);
		}
		void OnMouseWheel(object sender, MouseEventArgs e) {
			innerEditor.OnMouseWheel(e);
		}
		void OnMouseClick(object sender, MouseEventArgs e) {
			innerEditor.OnMouseClick(e);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (textBox != null) {
					UnsubscribeTextBoxEvents();
					textBox.Dispose();
					this.textBox = null;
				}
				ClearCachedFonts();
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
	[DXToolboxItem(false)]
	public class TextBoxWithTransparency : TextBox {
		ImeController imeController;
		public TextBoxWithTransparency()
			: base() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			imeController = new ImeController(this);
		}
		[System.Security.SecuritySafeCritical]
		protected override void WndProc(ref Message m) {
			if (ImeHelper.IsImeMessage(ref m)) {
				if (imeController.TrackMessage(ref m))
					return;
			}
			base.WndProc(ref m);
		}
	}
	#region ImeHelper
	public static class ImeHelper {
		[System.Security.SecuritySafeCritical]
		public static bool IsImeMessage(ref Message msg) {
			return msg.Msg == Win32.WM_IME_STARTCOMPOSITION || msg.Msg == Win32.WM_IME_COMPOSITION || msg.Msg == Win32.WM_IME_ENDCOMPOSITION;
		}
	}
	#endregion
	#region ImeController
	public class ImeController {
		#region Fields
		readonly TextBox box;
		string originalText;
		int originalCaretPosition;
		#endregion
		public ImeController(TextBox box) {
			this.box = box;
		}
		#region Properties
		IntPtr WindowHandle { get { return box.Handle; } }
		#endregion
		[System.Security.SecuritySafeCritical]
		public bool TrackMessage(ref Message m) {
			switch (m.Msg) {
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
			KeepOriginalParameters();
		}
		void KeepOriginalParameters() {
			this.originalText = box.Text;
			this.originalCaretPosition = box.SelectionStart;
		}
		void OnEndComposition() {
		}
		void OnComposition(int compositionState) {
			if (compositionState == 0) { 
				InsertCompositionText();
			}
			if ((compositionState & Win32.GcsFlags.GCS_RESULTSTR) != 0) {
				InsertCompositionResultText();
			}
			if ((compositionState & Win32.GcsFlags.GCS_COMPSTR) != 0) {
				InsertCompositionText();
			}
			if ((compositionState & Win32.GcsFlags.GCS_CURSORPOS) != 0) {
				MoveImeCaret();
			}
		}
		void MoveImeCaret() {
			MoveCaretCore(GetImeCompositionCursorPosition());
		}
		void MoveCaretCore(int offset) {
			box.SelectionStart = originalCaretPosition + offset;
		}
		void InsertCompositionText() {
			string imeString = GetImeString(Win32.GcsFlags.GCS_COMPSTR);
			InsertText(imeString);
		}
		void InsertCompositionResultText() {
			string imeString = GetImeString(Win32.GcsFlags.GCS_RESULTSTR);
			InsertText(imeString);
			KeepOriginalParameters();
		}
		void InsertText(string imeString) {
			if (String.IsNullOrEmpty(originalText))
				box.Text = imeString;
			else
				box.Text = originalText.Insert(originalCaretPosition, imeString);
			MoveCaretCore(imeString.Length);
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
	}
	#endregion
	#region InplaceEditorActualBoundsCalculator
	public class InplaceEditorActualBoundsCalculator {
		static IWordBreakProvider WordBreakProvider { get { return DocumentModel.WordBreakProvider; } }
		public Rectangle ControlClientBounds { get; set; }
		public string Text { get; set; }
		public Font Font { get; set; }
		public bool WrapText { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public XlVerticalAlignment VerticalAlignment { get; set; }
		public Rectangle Bounds { get; private set; }
		public Rectangle TextBoxBounds { get; private set; }
		public Size BestSize { get; private set; }
		string NonEmptyText { get { return String.IsNullOrEmpty(Text) ? " " : Text; } }
		public int LeftTextMargin { get; set; }
		public int RightTextMargin { get; set; }
		public void Calculate(InplaceEditorBoundsInfo boundsInfo) {
			if (WrapText) {
				this.BestSize = this.CalcBestSize(boundsInfo.EditorBounds.Width);
				this.Bounds = AdjustBoundsWrapText(boundsInfo.EditorBounds);
			}
			else {
				this.BestSize = this.CalcBestSize();
				if (this.HorizontalAlignment == HorizontalAlignment.Left)
					this.Bounds = AdjustBoundsAlignLeft(boundsInfo.EditorBounds);
				else if (this.HorizontalAlignment == HorizontalAlignment.Right)
					this.Bounds = AdjustBoundsAlignRight(boundsInfo.EditorBounds);
				else 
					this.Bounds = AdjustBoundsAlignCenter(boundsInfo.EditorBounds, boundsInfo.HostCellBounds);
			}
			TextBoxBounds = new Rectangle(0, CalculateEditorTop(this.Bounds, BestSize), this.Bounds.Width, this.Bounds.Height);
		}
		Size CalcBestSize() {
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
				return TextUtils.GetStringSize(graphics, NonEmptyText, Font, StringFormat.GenericTypographic, Int32.MaxValue, WordBreakProvider);
			}
		}
		internal Size CalcBestSize(int maxTextWidth) {
			maxTextWidth = Math.Max(0, maxTextWidth - LeftTextMargin - RightTextMargin);
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
				return TextUtils.GetStringSize(graphics, NonEmptyText, Font, StringFormat.GenericTypographic, maxTextWidth, WordBreakProvider);
			}
		}
		Rectangle FitBoundsToControlClientBounds(Rectangle bounds) {
			Rectangle clientBounds = ControlClientBounds;
			int maxRight = clientBounds.Right;
			if (bounds.Right > maxRight)
				bounds.Width -= bounds.Right - maxRight;
			int minLeft = clientBounds.Left;
			if (bounds.Left < minLeft) {
				bounds.Width -= minLeft - bounds.Left;
				bounds.X = minLeft;
			}
			return bounds;
		}
		Rectangle AdjustBoundsAlignLeft(Rectangle bounds) {
			int bestWidth = BestSize.Width + BestSize.Height;
			if (bestWidth > bounds.Width)
				bounds.Width = bestWidth;
			Rectangle actualBounds = FitBoundsToControlClientBounds(bounds);
			if (actualBounds.Width != bounds.Width)
				this.BestSize = this.CalcBestSize(actualBounds.Width);
			actualBounds.Height = Math.Max(BestSize.Height, actualBounds.Height);
			return actualBounds;
		}
		Rectangle AdjustBoundsWrapText(Rectangle bounds) {
			bounds.Height = BestSize.Height;
			return bounds;
		}
		Rectangle AdjustBoundsAlignRight(Rectangle bounds) {
			int bestWidth = BestSize.Width + BestSize.Height;
			if (bestWidth > bounds.Width) {
				bounds.X -= bestWidth - bounds.Width;
				bounds.Width = bestWidth;
			}
			Rectangle actualBounds = FitBoundsToControlClientBounds(bounds);
			if (actualBounds.Width != bounds.Width)
				this.BestSize = this.CalcBestSize(actualBounds.Width);
			actualBounds.Height = Math.Max(BestSize.Height, actualBounds.Height);
			return actualBounds;
		}
		Rectangle AdjustBoundsAlignCenter(Rectangle editorBounds, Rectangle hostCellBounds) {
			int bestWidth = BestSize.Width + BestSize.Height;
			if (bestWidth > editorBounds.Width) {
				editorBounds.X -= (bestWidth - editorBounds.Width) / 2;
				editorBounds.Width = bestWidth;
			}
			Rectangle actualBounds = FitBoundsToControlClientBounds(editorBounds);
			if (actualBounds.Width != editorBounds.Width) {
				actualBounds = hostCellBounds;
				BestSize = this.CalcBestSize(hostCellBounds.Width);
			}
			actualBounds.Height = Math.Max(BestSize.Height, actualBounds.Height);
			return actualBounds;
		}
		int CalculateEditorTop(Rectangle bounds, Size bestSize) {
			int result = 0;
			if (VerticalAlignment == XlVerticalAlignment.Bottom)
				result = bounds.Height - bestSize.Height;
			else if (VerticalAlignment == XlVerticalAlignment.Center)
				result = (bounds.Height - bestSize.Height) / 2;
			return Math.Max(0, result);
		}
	}
	#endregion
#endif
}
#region version with RichTextControl
#endregion
