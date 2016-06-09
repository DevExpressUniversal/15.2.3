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
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Editors {
	public class ShortcutEditor : XtraForm {
		private SimpleButton btnOK;
		private SimpleButton btnCancel;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtEditor;
		private System.Windows.Forms.Label lblCaption;
		private BarShortcut scValue;
		public static BarShortcut ShowShortcutEditor(IWindowsFormsEditorService edSvc, BarShortcut shortcutValue) {
			ShortcutEditor editor = new ShortcutEditor(shortcutValue);
			editor.ClientSize = new System.Drawing.Size(246, 94);
			DialogResult res = (edSvc != null ? edSvc.ShowDialog(editor) : editor.ShowDialog());
			if(res == DialogResult.OK) {
				return editor.ShortcutValue;
			} 
			return shortcutValue;
		}
		public BarShortcut ShortcutValue {
			get { return scValue; }
		}
		public ShortcutEditor(BarShortcut shortcutValue) {
			InitializeComponent();
			scValue = shortcutValue;
			if(scValue == null) scValue = new BarShortcut(); 
			ShowShortcuts();
		}
		private void ShowShortcuts() {
			if(scValue.IsExist)
				txtEditor.Text = scValue.ToString();
			else txtEditor.Text = "";
			txtEditor.SelectionStart = txtEditor.Text.Length;	
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) 
				if(components != null)
					components.Dispose();
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnOK = new SimpleButton();
			this.btnCancel = new SimpleButton();
			this.lblCaption = new System.Windows.Forms.Label();
			this.txtEditor = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(84, 65);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 24);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(164, 65);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 24);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.lblCaption.Location = new System.Drawing.Point(4, 4);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(232, 22);
			this.lblCaption.TabIndex = 4;
			this.lblCaption.Text = "Press shortcut key(s):";
			this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.txtEditor.Location = new System.Drawing.Point(4, 30);
			this.txtEditor.Name = "txtEditor";
			this.txtEditor.Size = new System.Drawing.Size(236, 20);
			this.txtEditor.TabIndex = 0;
			this.txtEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEditor_KeyPress);
			this.txtEditor.TextChanged += new System.EventHandler(this.txtEditor_TextChanged);
			this.txtEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEditor_KeyDown);
			this.ClientSize = new System.Drawing.Size(246, 90);
			this.ControlBox = false;
			this.Controls.Add(this.txtEditor);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblCaption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "ShortcutEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Shortcut Editor";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private bool EditingKey() {
			if(scValue.SecondKey != Keys.None || scValue.Key == Keys.None) 
				return true;
			else return false;
		} 
		private bool IsFitKey(KeyEventArgs e) {
			if(e.KeyCode == Keys.ControlKey ||
				e.KeyCode == Keys.ShiftKey ||
				e.KeyCode == Keys.Menu ||
				e.KeyCode == Keys.Capital)
				return false;
			if(e.KeyData == Keys.Back) {
				if(scValue.SecondKey != Keys.None) 
					scValue = new BarShortcut(scValue.Key);
				else scValue = new BarShortcut();
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
				scValue = new BarShortcut(e.KeyData);
			else 
				scValue = new BarShortcut(scValue.Key, e.KeyData);
			ShowShortcuts();
		}
		private void txtEditor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			e.Handled = true;
		}
		private void txtEditor_TextChanged(object sender, System.EventArgs e) {
			ShowShortcuts();
		}
	}
}
