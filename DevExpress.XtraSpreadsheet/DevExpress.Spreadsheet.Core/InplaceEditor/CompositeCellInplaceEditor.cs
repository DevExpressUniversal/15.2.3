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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	#region CompositeCellInplaceEditor
	public class CompositeCellInplaceEditor : ICellInplaceEditor {
		readonly List<ICellInplaceEditor> editors;
		public CompositeCellInplaceEditor() {
			this.editors = new List<ICellInplaceEditor>();
		}
		protected List<ICellInplaceEditor> Editors { get { return editors; } }
		ICellInplaceEditor Editor { get { return editors[editors.Count - 1]; } }
		ICellInplaceEditor CurrentEditor {
			get {
				for (int i = editors.Count - 1; i >= 0; i--)
					if (editors[i].CurrentEditable)
						return editors[i];
				return Editor;
			}
		}
		public void Add(ICellInplaceEditor editor) {
			Editors.Add(editor);
			editor.EditorTextChanged += OnTextChanged;
			editor.EditorSelectionChanged += OnEditorSelectionChanged;
			editor.GotFocus += OnGotFocus;
			editor.Registered = true;
		}
		void OnGotFocus(object sender, EventArgs e) {
			ICellInplaceEditor senderEditor = sender as ICellInplaceEditor;
			if(senderEditor == null)
				return;
			foreach(ICellInplaceEditor editor in editors)
				editor.CurrentEditable = false;
			senderEditor.CurrentEditable = true;
		}
		void OnTextChanged(object sender, TextChangedEventArgs e) {
			string text = e.Text;
			SetText(text);
		}
		void OnEditorSelectionChanged(object sender, EventArgs e) {
			RaiseEditorSelectionChanged();
		}
		void SetText(string text) {
			UnsubscribeEditorEvents();
			try {
				foreach (ICellInplaceEditor editor in editors) {
						editor.Text = text;
				}
			}
			finally {
				SubscribeEditorEvents();
			}
			RaiseEditorTextChanged(text);
		}
		void UnsubscribeEditorEvents() {
			foreach (ICellInplaceEditor editor in editors) {
				editor.EditorTextChanged -= OnTextChanged;
				editor.EditorSelectionChanged -= OnEditorSelectionChanged;
			}
		}
		void SubscribeEditorEvents() {
			foreach (ICellInplaceEditor editor in editors) {
				editor.EditorTextChanged += OnTextChanged;
				editor.EditorSelectionChanged += OnEditorSelectionChanged;
			}
		}
		bool ICellInplaceEditor.IsVisible {
			get { return Editor.IsVisible; }
			set {
				foreach (ICellInplaceEditor editor in editors)
					editor.IsVisible = value;
			}
		}
		bool ICellInplaceEditor.CurrentEditable {
			get { return Editor.CurrentEditable; }
			set { }
		}
		bool ICellInplaceEditor.Registered {
			get { return Editor.Registered; }
			set { }
		}
		bool ICellInplaceEditor.Focused {
			get {
				foreach(ICellInplaceEditor editor in editors)
					if(editor.Focused)
						return editor.Focused;
				return Editor.Focused; 
			}
		}
		bool ICellInplaceEditor.WrapText {
			get { return Editor.WrapText; }
			set {
				foreach (ICellInplaceEditor editor in editors)
					editor.WrapText = value;
			}
		}
		string ICellInplaceEditor.Text {
			get { return Editor.Text; }
			set {
				SetText(value);
			}
		}
		Color ICellInplaceEditor.ForeColor {
			get { return Editor.ForeColor; }
			set {
				foreach (ICellInplaceEditor editor in editors)
					editor.ForeColor = value;
			}
		}
		Color ICellInplaceEditor.BackColor {
			get { return Editor.BackColor; }
			set {
				foreach (ICellInplaceEditor editor in editors)
					editor.BackColor = value;
			}
		}
		int ICellInplaceEditor.SelectionStart { get { return CurrentEditor.SelectionStart; } }
		int ICellInplaceEditor.SelectionLength { get { return CurrentEditor.SelectionLength; } }
		void ICellInplaceEditor.Close() {
			foreach(ICellInplaceEditor editor in editors) {
				editor.Close();
				editor.CurrentEditable = false;
			}
		}
		void ICellInplaceEditor.SetFocus() {
			foreach(ICellInplaceEditor editor in editors)
				if(editor.CurrentEditable) {
					editor.SetFocus();
					return;
				}
		}
		void ICellInplaceEditor.SetSelection(int start, int length) {
			foreach (ICellInplaceEditor editor in editors)
				editor.SetSelection(start, length);
		}
		void ICellInplaceEditor.SetFont(FontInfo fontInfo, float zoomFactor) {
			foreach (ICellInplaceEditor editor in editors)
				editor.SetFont(fontInfo, zoomFactor);
		}
		void ICellInplaceEditor.SetHorizontalAlignment(XlHorizontalAlignment alignment) {
			foreach (ICellInplaceEditor editor in editors)
				editor.SetHorizontalAlignment(alignment);
		}
		void ICellInplaceEditor.SetVerticalAlignment(XlVerticalAlignment alignment) {
			foreach (ICellInplaceEditor editor in editors)
				editor.SetVerticalAlignment(alignment);
		}
		void ICellInplaceEditor.SetBounds(InplaceEditorBoundsInfo boundsInfo) {
			foreach (ICellInplaceEditor editor in editors)
				editor.SetBounds(boundsInfo);
		}
		event EventHandler ICellInplaceEditor.GotFocus {
			add {
				foreach (ICellInplaceEditor editor in editors)
					editor.GotFocus += value;
			}
			remove {
				foreach (ICellInplaceEditor editor in editors)
					editor.GotFocus -= value;
			}
		}
		#region EditorTextChanged
		TextChangedEventHandler onEditorTextChanged;
		public event TextChangedEventHandler EditorTextChanged {
			add {
				onEditorTextChanged -= value; 
				onEditorTextChanged += value;
			}
			remove { onEditorTextChanged -= value; }
		}
		void RaiseEditorTextChanged(string text) {
			if (onEditorTextChanged != null) {
				TextChangedEventArgs args = new TextChangedEventArgs(text);
				onEditorTextChanged(this, args);
			}
		}
		#endregion
		#region EditorSelectionChanged
		EventHandler onEditorSelectionChanged;
		public event EventHandler EditorSelectionChanged { add { onEditorSelectionChanged += value; } remove { onEditorSelectionChanged -= value; } }
		void RaiseEditorSelectionChanged() {
			if (onEditorSelectionChanged != null) {
				EventArgs args = new EventArgs();
				onEditorSelectionChanged(this, args);
			}
		}
		#endregion
		void IDisposable.Dispose() {
			UnsubscribeEditorEvents();
			foreach(ICellInplaceEditor editor in editors) {
				editor.Dispose();
				editor.CurrentEditable = false;
				editor.Registered = false;
			}
		}
		void ICellInplaceEditor.Activate() {
			foreach (ICellInplaceEditor editor in editors) {
				editor.Activate();
			}
		}
		void ICellInplaceEditor.Deactivate() {
			UnsubscribeEditorEvents();
			foreach(ICellInplaceEditor editor in editors) {
				editor.Deactivate();
				editor.CurrentEditable = false;
				editor.Registered = false;
			}
		}
		void ICellInplaceEditor.Rollback() {
			UnsubscribeEditorEvents();
			try {
				foreach(ICellInplaceEditor editor in editors)
					editor.Rollback();
			}
			finally {
				SubscribeEditorEvents();
			}
		}
		void ICellInplaceEditor.Copy() {
			CurrentEditor.Copy();
		}
		void ICellInplaceEditor.Cut() {
			CurrentEditor.Cut();
		}
		void ICellInplaceEditor.Paste() {
			CurrentEditor.Paste();
		}
	}
	#endregion
}
