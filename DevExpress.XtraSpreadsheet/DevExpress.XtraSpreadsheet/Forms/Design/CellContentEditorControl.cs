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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	]
	public class CellContentEditorControl : PanelControl, ICellInplaceEditor {
		#region Fields
		TextBox textBox;
		SpreadsheetControl control;
		InnerCellInplaceEditor innerEditor;
		#endregion
		public CellContentEditorControl() {
			this.textBox = new TextBox();
			textBox.Visible = false;
			textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			textBox.ScrollBars = ScrollBars.None;
			textBox.Multiline = true;
			textBox.AutoSize = false;
			textBox.WordWrap = true;
			textBox.AcceptsReturn = false;
			textBox.AcceptsTab = true;
			textBox.Dock = DockStyle.Fill;
			SubscribeTextBoxEvents();
			this.Controls.Add(textBox);
			textBox.Parent = this;
		}
		#region Properties
		public SpreadsheetControl Control { get { return control; } set { SetControl(value); } }
		#endregion
		#region ICellInplaceEditor implementation
		Color ICellInplaceEditor.ForeColor { get { return textBox.ForeColor; } set {} }
		Color ICellInplaceEditor.BackColor { get { return textBox.BackColor; } set { } }
		bool ICellInplaceEditor.IsVisible { get { return this.Visible; } set { this.Visible = value; textBox.Visible = value; } }
		string ICellInplaceEditor.Text { get { return textBox.Text; } set { textBox.Text = value; } }
		bool ICellInplaceEditor.WrapText { get { return true; } set { } }
		int ICellInplaceEditor.SelectionStart { get { return textBox.SelectionStart; } }
		int ICellInplaceEditor.SelectionLength { get { return textBox.SelectionLength; } }
		bool ICellInplaceEditor.Focused { get { return textBox.Focused; } }
		bool ICellInplaceEditor.CurrentEditable { get; set; }
		bool ICellInplaceEditor.Registered { get; set; }
		TextChangedEventHandler onEditorTextChanged;
		public event TextChangedEventHandler EditorTextChanged { add { onEditorTextChanged += value; } remove { onEditorTextChanged -= value; } }
		protected internal virtual void RaiseEditorTextChanged() {
			if(onEditorTextChanged != null) {
				TextChangedEventArgs args = new TextChangedEventArgs(textBox.Text);
				onEditorTextChanged(this, args);
			}
		}
		#region EditorSelectionChanged
		EventHandler onEditorSelectionChanged;
		public event EventHandler EditorSelectionChanged { add { onEditorSelectionChanged += value; } remove { onEditorSelectionChanged -= value; } }
		#endregion
		void ICellInplaceEditor.SetBounds(InplaceEditorBoundsInfo boundsInfo) {
		}
		void ICellInplaceEditor.SetFont(Office.Drawing.FontInfo fontInfo, float zoomFactor) {
		}
		void ICellInplaceEditor.SetHorizontalAlignment(XlHorizontalAlignment alignment) {
		}
		void ICellInplaceEditor.SetVerticalAlignment(XlVerticalAlignment alignment) {
		}
		void ICellInplaceEditor.Rollback() {
		}
		void ICellInplaceEditor.SetFocus() {
			textBox.Focus();
		}
		void ICellInplaceEditor.SetSelection(int start, int length) {
			textBox.Select(start, length);
		}
		void ICellInplaceEditor.Close() {
			if (control != null && !control.IsDisposed)
				control.Focus();
		}
		void ICellInplaceEditor.Activate() {
		}
		void ICellInplaceEditor.Deactivate() {
			this.Dispose();
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
		void SetControl(SpreadsheetControl control) {
			this.control = control;
			if (control != null)
				this.innerEditor = control.InnerControl.InplaceEditor;
			else
				this.innerEditor = null;
		}
		void SubscribeTextBoxEvents() {
			textBox.KeyDown += OnKeyDown;
			textBox.KeyPress += OnKeyPress;
			textBox.KeyUp += OnKeyUp;
			textBox.MouseWheel += OnMouseWheel;
			textBox.TextChanged += OnTextChanged;
			textBox.GotFocus += OnTextBoxGotFocus;
		}
		void UnsubscribeTextBoxEvents() {
			textBox.KeyDown -= OnKeyDown;
			textBox.KeyPress -= OnKeyPress;
			textBox.KeyUp -= OnKeyUp;
			textBox.MouseWheel -= OnMouseWheel;
			textBox.TextChanged -= OnTextChanged;
			textBox.GotFocus -= OnTextBoxGotFocus;
		}
		void OnTextBoxGotFocus(object sender, EventArgs e) {
			OnGotFocus(e);
		}
		void OnTextChanged(object sender, EventArgs e) {
			RaiseEditorTextChanged();
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			if (innerEditor != null) {
				innerEditor.OnKeyDown(e);
				e.SuppressKeyPress = (e.KeyCode == Keys.Return);
			}
		}
		void OnKeyUp(object sender, KeyEventArgs e) {
			if (innerEditor != null)
				innerEditor.OnKeyUp(e);
		}
		void OnKeyPress(object sender, KeyPressEventArgs e) {
			if (innerEditor != null)
				innerEditor.OnKeyPress(e);
		}
		void OnMouseWheel(object sender, MouseEventArgs e) {
			if (innerEditor != null)
				innerEditor.OnMouseWheel(e);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (textBox != null) {
					UnsubscribeTextBoxEvents();
					textBox.Dispose();
					this.textBox = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
}
