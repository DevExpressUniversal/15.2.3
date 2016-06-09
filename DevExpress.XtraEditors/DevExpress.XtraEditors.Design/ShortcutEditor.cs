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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Design {
	public class KeyShortcutEditor : System.Windows.Forms.Form {
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtEditor;
		private System.Windows.Forms.Label lblCaption;
		private KeyShortcut scValue;
		public static KeyShortcut ShowShortcutEditor(IWindowsFormsEditorService edSvc, KeyShortcut shortcutValue) {
			KeyShortcutEditor editor = new KeyShortcutEditor(shortcutValue);
			DialogResult res = (edSvc != null ? edSvc.ShowDialog(editor) : editor.ShowDialog());
			if(res == DialogResult.OK) {
				return editor.ShortcutValue;
			}
			return shortcutValue;
		}
		public KeyShortcut ShortcutValue {
			get { return scValue; }
		}
		public KeyShortcutEditor(KeyShortcut shortcutValue) {
			InitializeComponent();
			scValue = shortcutValue;
			if(scValue == null) scValue = new KeyShortcut();
			ShowShortcuts();
		}
		private void ShowShortcuts() {
			if(scValue.IsExist)
				txtEditor.Text = scValue.ToString();
			else txtEditor.Text = "";
			txtEditor.SelectionStart = txtEditor.Text.Length;
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				if(components != null)
					components.Dispose();
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyShortcutEditor));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblCaption = new System.Windows.Forms.Label();
			this.txtEditor = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblCaption, "lblCaption");
			this.lblCaption.Name = "lblCaption";
			resources.ApplyResources(this.txtEditor, "txtEditor");
			this.txtEditor.Name = "txtEditor";
			this.txtEditor.TextChanged += new System.EventHandler(this.txtEditor_TextChanged);
			this.txtEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEditor_KeyDown);
			this.txtEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEditor_KeyPress);
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.txtEditor);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblCaption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "KeyShortcutEditor";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private bool EditingKey() {
			return true;
		}
		private bool IsFitKey(KeyEventArgs e) {
			if(e.KeyCode == Keys.ControlKey ||
				e.KeyCode == Keys.ShiftKey ||
				e.KeyCode == Keys.Menu ||
				e.KeyCode == Keys.Capital)
				return false;
			if(e.KeyData == Keys.Back) {
				scValue = new KeyShortcut(scValue.Key);
				ShowShortcuts();
				return false;
			}
			if(e.KeyData == Keys.Enter) {
				this.DialogResult = DialogResult.OK;
				return false;
			}
			if(e.KeyData == Keys.Escape) {
				this.DialogResult = DialogResult.Cancel;
				return false;
			}
			if(EditingKey()) {
				if(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z && !e.Alt && !e.Control)
					return false;
			}
			return true;
		}
		private void txtEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			e.Handled = true;
			if(!IsFitKey(e)) return;
			if(EditingKey())
				scValue = new KeyShortcut(e.KeyData);
			ShowShortcuts();
		}
		private void txtEditor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			e.Handled = true;
		}
		private void txtEditor_TextChanged(object sender, System.EventArgs e) {
			ShowShortcuts();
		}
	}
	public class EditorButtonShortcutEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return value;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(edSvc == null) return value;
			object savedValue = value;
			EditorButtonShortcutEditorForm form = new EditorButtonShortcutEditorForm(this, edSvc, value);
			edSvc.DropDownControl(form);
			value = form.EditValue;
			if(value is string) {
				value = DevExpress.XtraEditors.Design.KeyShortcutEditor.ShowShortcutEditor(edSvc, savedValue as KeyShortcut);
			}
			form.Dispose();
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
	}
	[ToolboxItem(false)]
	public class EditorButtonShortcutEditorForm : Panel {
		EditorButtonShortcutEditor editor;
		System.Windows.Forms.ListBox listBox;
		object editValue, originalValue;
		IWindowsFormsEditorService edSvc;
		internal class InputListBox : ListBox {
		}
		public EditorButtonShortcutEditorForm(EditorButtonShortcutEditor editor, IWindowsFormsEditorService edSvc, object editValue) {
			this.editValue = this.originalValue = editValue;
			this.editor = editor;
			this.BorderStyle = BorderStyle.None;
			this.edSvc = edSvc;
			listBox = new InputListBox();
			listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			listBox.Dock = DockStyle.Fill;
			foreach(object obj in DevExpress.Utils.Design.Serialization.KeyShortcutTypeConverter.KeysList) {
				listBox.Items.Add(obj);
			}
			if(listBox.Items.Contains(EditValue))
				listBox.SelectedItem = EditValue;
			else
				listBox.SelectedIndex = 1;
			this.listBox.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);
			this.listBox.Click += new EventHandler(OnClick);
			Controls.Add(listBox);
			this.listBox.CreateControl();
			this.Size = new Size(0, listBox.ItemHeight * Math.Min(listBox.Items.Count, 10));
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected virtual void OnClick(object sender, EventArgs e) {
			edSvc.CloseDropDown();
		}
		protected virtual void OnSelectedValueChanged(object sender, EventArgs e) {
			EditValue = listBox.SelectedItem;
		}
		public EditorButtonShortcutEditor Editor { get { return editor; } }
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				editValue = originalValue;
				edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		public object EditValue {
			get { return editValue; }
			set {
				if(editValue == value) return;
				editValue = value;
			}
		}
	}
}
