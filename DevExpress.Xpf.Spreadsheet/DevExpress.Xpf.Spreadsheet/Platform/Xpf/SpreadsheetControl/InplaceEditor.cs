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
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Office.Drawing;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using PlatformIndependentColor = System.Drawing.Color;
using PlatformIndependentRectangle = System.Drawing.Rectangle;
using PlatformIndependentKeys = System.Windows.Forms.Keys;
using PlatformIndependentFont = System.Drawing.Font;
using PlatformIndependentSize = System.Drawing.Size;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Text;
using System.Globalization;
using System.Windows;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class XpfCellInplaceEditorBase : TextBox, ICellInplaceEditor {
		public XpfCellInplaceEditorBase() {
			this.Loaded += XpfCellInplaceEditorLoaded;
		}
		protected InnerCellInplaceEditor InnerEditor { get; set; }
		protected ISpreadsheetControl Control { get; set; }
		protected WorksheetControl WorksheetControl { get; set; }
		protected Brush DefaultForeground { get; set; }
		protected Brush DefaultBackground { get; set; }
		void XpfCellInplaceEditorLoaded(object sender, System.Windows.RoutedEventArgs e) {
			OnLoaded();
		}
		protected virtual void OnLoaded() {
			DefaultBackground = this.Background;
			DefaultForeground = this.Foreground;
		}
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.SystemKey == Key.Enter && Keyboard.Modifiers == ModifierKeys.Alt) {
				this.Text += Environment.NewLine;
				this.CaretIndex = this.Text.Length;
				e.Handled = true;
				return;
			}
			InnerEditor.OnKeyDown(e.ToPlatformIndependent());
		}
		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e) {
			base.OnKeyUp(e);
			InnerEditor.OnKeyUp(e.ToPlatformIndependent());
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			OnTextInputCore(e);
		}
		protected override void OnSelectionChanged(RoutedEventArgs e) {
			base.OnSelectionChanged(e);
			if (lockTextChanged)
				return;
			RaiseSelectionChanged();
		}
		protected virtual void OnTextInputCore(TextCompositionEventArgs e) {
			char[] text = e.Text.ToCharArray();
			if (InnerEditor != null && text.Length > 0)
				InnerEditor.OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(text[0]));
		}
		bool lockTextChanged = false;
		protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e) {
			base.OnTextChanged(e);
			if (lockTextChanged || InnerEditor == null) return;
			lockTextChanged = true;
			try {
				InnerEditor.InputPercentage();
				RaiseTextChanged();
			}
			finally {
				lockTextChanged = false;
			}
		}
		protected internal virtual void RaiseTextChanged() {
			if (onEditorTextChanged != null) {
				DevExpress.XtraSpreadsheet.Model.TextChangedEventArgs args =
					new DevExpress.XtraSpreadsheet.Model.TextChangedEventArgs(Text);
				onEditorTextChanged(this, args);
			}
		}
		protected internal virtual void RaiseSelectionChanged() {
			if (onEditorSelectionChanged != null) {
				onEditorSelectionChanged(this, new EventArgs());
			}
		}
		public new bool IsVisible {
			get { return Visibility == System.Windows.Visibility.Visible; }
			set {
				Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
				OnVisibilityChanged(value);
			}
		}
		protected virtual void OnVisibilityChanged(bool isVisible) {
			SetBackground(isVisible);
		}
		protected virtual void SetBackground(bool isVisible) {
		}
		public PlatformIndependentColor ForeColor { get; set; }
		public PlatformIndependentColor BackColor { get; set; }
		public bool CurrentEditable { get; set; }
		public bool Registered { get; set; }
		public bool Focused { get { return false; } }
		DevExpress.XtraSpreadsheet.Model.TextChangedEventHandler onEditorTextChanged;
		event DevExpress.XtraSpreadsheet.Model.TextChangedEventHandler ICellInplaceEditor.EditorTextChanged { add { onEditorTextChanged += value; } remove { onEditorTextChanged -= value; } }
		EventHandler onEditorSelectionChanged;
		event EventHandler ICellInplaceEditor.EditorSelectionChanged { add { onEditorSelectionChanged += value; } remove { onEditorSelectionChanged -= value; } }
		EventHandler onGotFocus;
		event EventHandler ICellInplaceEditor.GotFocus { add { onGotFocus += value; } remove { onGotFocus -= value; } }
		public void Close() {
			TextInfo = null;
			((SpreadsheetControl)Control).Focus();
		}
		public void SetFocus() {
			this.Focus();
		}
		public void SetSelection(int start, int length) {
			this.Select(start, length);
		}
		protected TextSettings TextInfo { get; set; }
		public void SetFont(FontInfo fontInfo, float zoomFactor) {
			SetFontCore(fontInfo);
		}
		protected virtual void SetFontCore(FontInfo fontInfo) { }
		protected virtual void SetBoundsCore(InplaceEditorBoundsInfo boundsInfo) { }
		protected virtual void SetTextAlignment(XlHorizontalAlignment alignment) { }
		public void SetHorizontalAlignment(XlHorizontalAlignment alignment) {
			SetTextAlignment(alignment);
		}
		public void Dispose() { }
		public void SetBounds(InplaceEditorBoundsInfo boundsInfo) {
			SetBoundsCore(boundsInfo);
		}
		protected virtual void OnRollBack() { }
		#region ICellInplaceEditor
		void ICellInplaceEditor.Rollback() {
			OnRollBack();
		}
		bool ICellInplaceEditor.WrapText {
			get { return TextWrapping != System.Windows.TextWrapping.NoWrap; }
			set { TextWrapping = value ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap; }
		}
		void ICellInplaceEditor.SetVerticalAlignment(XlVerticalAlignment alignment) {
			SetVerticalAlignmentCore(alignment);
		}
		protected virtual void SetVerticalAlignmentCore(XlVerticalAlignment alignment) { }
		void ICellInplaceEditor.Activate() {
			Activate();
		}
		protected internal virtual void Activate() {
		}
		void ICellInplaceEditor.Deactivate() {
			Deactivate();
		}
		protected internal virtual void Deactivate() {
			ChangeVisibility(false);
			((SpreadsheetControl)Control).MeasureAndRedraw();
		}
		protected virtual void ChangeVisibility(bool value) { }
		#endregion
	}
	public class XpfCellInplaceEditor : XpfCellInplaceEditorBase {
		public XpfCellInplaceEditor() {
			DefaultStyleKey = typeof(XpfCellInplaceEditor);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			WorksheetControl = LayoutHelper.FindParentObject<WorksheetControl>(this);
			Control = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
			if (Control != null)
				InnerEditor = Control.InnerControl.InplaceEditor;
		}
		protected override void SetFontCore(FontInfo fontInfo) {
			TextInfo = TextInfoCalculator.SetText(Text, fontInfo, ForeColor, DefaultForeground);
			this.FontFamily = TextInfo.FontFamily;
			this.FontSize = TextInfo.FontSize;
			this.FontStyle = TextInfo.FontStyle;
			this.FontWeight = TextInfo.FontWeight;
			this.Foreground = new SolidColorBrush(TextInfo.Foreground);
			this.TextDecorations = TextInfo.TextDecorations;
		}
		protected override void SetBoundsCore(InplaceEditorBoundsInfo boundsInfo) {
			TextAlignment alignment = TextAlignment.Left;
			Size size = boundsInfo.EditorBounds.ToRect().Size;
			Size calcSize = size;
			if (TextWrapping != System.Windows.TextWrapping.NoWrap) {
				this.Measure(new Size(size.Width + 1, double.PositiveInfinity));
				calcSize = new Size(size.Width, Math.Max(size.Height, this.DesiredSize.Height));
			}
			else {
				this.Measure(new Size(double.PositiveInfinity, size.Height));
				calcSize = new Size(Math.Max(size.Width, this.DesiredSize.Width), size.Height);
			}
			if (calcSize.Width > size.Width || calcSize.Height > size.Height) alignment = TextAlignment;
			if (WorksheetControl != null) WorksheetControl.ShowInplaceEditor(boundsInfo, calcSize, alignment);
		}
		protected override void ChangeVisibility(bool value) {
			base.ChangeVisibility(value);
			IsVisible = value;
		}
		protected override void SetTextAlignment(XlHorizontalAlignment alignment) {
			switch (alignment) {
				case XlHorizontalAlignment.Center:
					TextAlignment = System.Windows.TextAlignment.Center;
					break;
				case XlHorizontalAlignment.Left:
					TextAlignment = System.Windows.TextAlignment.Left;
					break;
				case XlHorizontalAlignment.Right:
					TextAlignment = System.Windows.TextAlignment.Right;
					break;
				case XlHorizontalAlignment.Justify:
					TextAlignment = System.Windows.TextAlignment.Justify;
					break;
				default:
					TextAlignment = System.Windows.TextAlignment.Left;
					break;
			}
		}
		protected override void SetVerticalAlignmentCore(XlVerticalAlignment alignment) {
			switch (alignment) {
				case XlVerticalAlignment.Bottom:
					VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
					break;
				case XlVerticalAlignment.Center:
					VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
					break;
				case XlVerticalAlignment.Top:
					VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
					break;
				case XlVerticalAlignment.Justify:
					VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
					break;
				default:
					VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
					break;
			}
		}
		protected override void SetBackground(bool isVisible) {
			if (isVisible)
				this.Background = (DXColor.IsEmpty(BackColor)) ? DefaultBackground : new SolidColorBrush(BackColor.ToWpfColor());
			else this.Background = null;
		}
	}
}
