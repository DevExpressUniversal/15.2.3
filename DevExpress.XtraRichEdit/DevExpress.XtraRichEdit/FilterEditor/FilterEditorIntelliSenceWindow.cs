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

using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraFilterEditor.IntelliSense {
	public interface IFilterEditorIntelliSenseWindow {
		void Show(List<string> items, Point screenPos);
		void Show(RepositoryItem properties, object value, List<string> items, Point screenPos);
		void Hide();
		void MoveUp(Keys key);
		void MoveDown(Keys key);
		void ScrollItemsList(int delta);
		bool IsShowing { get; }
		bool IsFocused { get; }
		string SelectedItem { get; }
		object SelectedValue { get; }
		int SelectedIndex { get; set; }
		List<string> Items { get; }
		event EventHandler GotFocus;
		event EventHandler IntelliSenseListDoubleClick;
		event EventHandler EditorClosed;
	}
	[DXToolboxItem(false)]
	public class IntelliSenseListBoxControl : ListBoxControl {
		public void HandleKey(Keys key) {
			OnKeyDown(new KeyEventArgs(key));
		}
		public void HandleMouseScroll(int delta) {
			OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
		}
		public int GetBestWidth() {
			if (!ScrollInfo.VScrollVisible) return ViewInfo.CalcBestColumnWidth();
			return ViewInfo.CalcBestColumnWidth() + ScrollInfo.VScrollWidth;
		}
		public int GetBestHeight() {
			return ViewInfo.ItemHeight * Items.Count;
		}
	}
	[DXToolboxItem(false)]
	public class FilterEditorIntelliSenseWindow : CustomTopForm, IFilterEditorIntelliSenseWindow {
		internal const int MaxWindowHeight = 150;
		internal const int MinWindowWidth = 150;
		internal const int MaxWindowWidth = 400;
		internal const int HorizontalPadding = 10;
		internal const int VerticalPadding = 2;
		private static readonly object editorClosed = new object();
		IntelliSenseListBoxControl listbox;
		BaseEdit editor;
		List<string> items;
		object selectedValue;
		public FilterEditorIntelliSenseWindow() {
			this.listbox = new IntelliSenseListBoxControl();
			Listbox.Parent = this;
			Listbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.items = new List<string>();
		}
		public event EventHandler EditorClosed {
			add { this.Events.AddHandler(editorClosed, value); }
			remove { this.Events.RemoveHandler(editorClosed, value); }
		}
		protected internal IntelliSenseListBoxControl Listbox { get { return listbox; } }
		protected List<string> Items { get { return items; } set { items = value; } }
		protected internal BaseEdit Editor { 
			get { return editor; }
			set {
				if(editor != value) {
					if(editor != null) {
						editor.KeyDown -= new KeyEventHandler(EditorKeyDown);
						editor.EditValueChanged -= new EventHandler(EditorEditValueChanged);
						if(editor.Properties.IsFilterLookUp && editor is PopupBaseEdit) {
							((PopupBaseEdit)editor).Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(FilterEditorIntelliSenseWindow_Closed);
						}
					}
					editor = value;
					if(editor != null) {
						editor.Parent = this;
						editor.Dock = DockStyle.Top;
						if(editor is ComboBoxEdit) {
							((ComboBoxEdit)editor).Properties.UseCtrlScroll = false;
						}
						editor.KeyDown += new KeyEventHandler(EditorKeyDown);
						editor.EditValueChanged += new EventHandler(EditorEditValueChanged);
						this.selectedValue = editor.EditValue;
						if(editor.Properties.IsFilterLookUp && editor is PopupBaseEdit) {
							((PopupBaseEdit)editor).Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(FilterEditorIntelliSenseWindow_Closed);
						}
					}
				}
			}
		}
		protected virtual void SetWindowSize() {
			int bestWidth = Listbox.GetBestWidth() + HorizontalPadding;
			bestWidth = bestWidth > MinWindowWidth ? bestWidth : MinWindowWidth;
			Width = bestWidth <= MaxWindowWidth ? bestWidth : MaxWindowWidth;
			int editorHeight = Editor != null ? Editor.Height : 0;
			if(Listbox.Items.Count > 0) {
				Height = Listbox.GetBestHeight() <= MaxWindowHeight ? Listbox.GetBestHeight() + VerticalPadding + editorHeight : MaxWindowHeight;
			} else {
				Height = editorHeight + VerticalPadding;
			}
		}
		[System.Security.SecuritySafeCritical]
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ACTIVATE && Editor != null) {
				this.isEditorGetFocus = true;
			}
			base.WndProc(ref m);
			this.isEditorGetFocus = false;
		}
		bool isEditorGetFocus;
		bool IsEditorFocused { get { return Editor != null && (Editor.Focused || isEditorGetFocus); } }
		void HideWindow() {
			if(Editor != null) {
				BaseEdit edit = this.Editor;
				this.Editor = null;
				Controls.Remove(edit); 
				edit.Dispose();
			}
			Hide();
		}
		bool IsEditorPopupOpen { get { return Editor != null && Editor is PopupBaseEdit && ((PopupBaseEdit)Editor).IsPopupOpen; } }
		void EditorKeyDown(object sender, KeyEventArgs e) {
			if(Editor == null) return;
			if(e.KeyCode == Keys.Escape) {
				HideWindow();
			}
			if((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) && !IsEditorPopupOpen) {
				Editor.DoValidate();
				CloseWindowOnEditorClose();
			}
		}
		void CloseWindowOnEditorClose() {
			this.selectedValue = Editor.EditValue;
			EventHandler handler = (EventHandler)Events[editorClosed];
			if(handler != null) handler(this, EventArgs.Empty);
			HideWindow();
		}
		void EditorEditValueChanged(object sender, EventArgs e) {
			this.selectedValue = Editor.EditValue;
		}
		#region IFilterEditorIntelliSenseWindow Members
		List<string> IFilterEditorIntelliSenseWindow.Items { get { return Items; } }
		string IFilterEditorIntelliSenseWindow.SelectedItem { get { return (string)Listbox.SelectedItem; } }
		object IFilterEditorIntelliSenseWindow.SelectedValue { get { return selectedValue; } }
		int IFilterEditorIntelliSenseWindow.SelectedIndex { get { return Listbox.SelectedIndex; } set { Listbox.SelectedIndex = value; } }
		event EventHandler IFilterEditorIntelliSenseWindow.GotFocus {
			add { Listbox.GotFocus += value; }
			remove { Listbox.GotFocus -= value; }
		}
		event EventHandler IFilterEditorIntelliSenseWindow.IntelliSenseListDoubleClick {
			add { Listbox.DoubleClick += value; }
			remove { Listbox.DoubleClick -= value; }
		}
		void IFilterEditorIntelliSenseWindow.Show(List<string> items, Point screenPos) {
			((IFilterEditorIntelliSenseWindow)this).Show(null, null, items, screenPos);
		}
		void IFilterEditorIntelliSenseWindow.Show(RepositoryItem properties, object value, List<string> items, Point screenPos) {
			if(properties != null) {
				BaseEdit edit = properties.CreateEditor();
				edit.Properties.Assign(properties);
				edit.Properties.ReadOnly = false;
				if(value != null) {
					try {
						edit.EditValue = value;
					}
					catch { }
				}
				this.Editor = edit;
			}
			Items = items;
			Listbox.Items.BeginUpdate();
			try {
				Listbox.Items.Clear();
				foreach(string item in items) {
					Listbox.Items.Add(item);
				}
				Listbox.SelectedIndex = -1;
			} finally {
				Listbox.Items.EndUpdate();
			}
			Location = screenPos;
			SetWindowSize();
			Listbox.Visible = Listbox.Items.Count > 0;
			Show();
			if(Editor != null && Editor.Properties.IsFilterLookUp && Editor is PopupBaseEdit) {
				Editor.Focus();
				((PopupBaseEdit)Editor).ShowPopup();
			}
		}
		void FilterEditorIntelliSenseWindow_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e) {
			if(e.CloseMode != PopupCloseMode.Cancel) {
				CloseWindowOnEditorClose();
			} else {
				HideWindow();
			}
		}
		void IFilterEditorIntelliSenseWindow.Hide() {
			HideWindow();
		}
		bool IFilterEditorIntelliSenseWindow.IsShowing { get { return Visible; } }
		void IFilterEditorIntelliSenseWindow.MoveUp(Keys key) {
			if (!Visible) return;
			Listbox.HandleKey(key);
		}
		void IFilterEditorIntelliSenseWindow.MoveDown(Keys key) {
			if (!Visible) return;
			if(Editor != null) {
				this.isEditorGetFocus = true;
				Editor.Focus();
				this.isEditorGetFocus = false;
			} else {
				Listbox.HandleKey(key);
			}
		}
		void IFilterEditorIntelliSenseWindow.ScrollItemsList(int delta) {
			if (!Visible) return;
			Listbox.HandleMouseScroll(delta);
		}
		bool IFilterEditorIntelliSenseWindow.IsFocused { get { return Listbox.Focused || IsEditorFocused; } }
		#endregion
	}
}
