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
using System.Windows.Forms;
namespace DevExpress.XtraLayout.Customization {
	public class RenamerTextBox : DevExpress.XtraEditors.Controls.ModalTextBox, IMessageFilter {
		#region IMessageFilter Members
		private bool mFiltering;
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_RBUTTONDOWN = 0x0204;
		public static int SignedLOWORD(int n) {
			return (short)(n & 0xffff);
		}
		public static int SignedHIWORD(int n) {
			return (short)((n >> 0x10) & 0xffff);
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			Close();
		}
		protected void Cancel() {
			DialogResult = DialogResult.Cancel; Close();
		}
		protected bool HasInChildren(Control control, Control child) {
			if(control == child) return true;
			foreach(Control temp in control.Controls) if(HasInChildren(temp, child)) return true;
			return false;
		}
		public bool PreFilterMessage(ref Message m) {
			if((m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN) && !mFiltering) {
				mFiltering = true;
				Control control = Control.FromHandle(m.HWnd);
				if(!HasInChildren(this, control)) {
					BeginInvoke(new MethodInvoker(Cancel));
					mFiltering = false;
					return true;
				}
				mFiltering = false;
				return false;
			}
			return false;
		}
		#endregion
	}
	public class RenameItemManager {
		protected virtual bool CanRename(BaseLayoutItem item) {
			if(item.Parent != null) {
				if(!item.Parent.AllowCustomizeChildren) {
					return false;
				}
				else {
					return CanRename(item.Parent);
				}
			}
			return true;
		}
		RenamerTextBox textBox;
		BaseLayoutItem itemCore;
		public virtual void Rename(BaseLayoutItem item, Rectangle screenEditorRect) {
			if(item == null || screenEditorRect.Size.IsEmpty) return;
			if(!CanRename(item)) return;
			itemCore = item;
			if(screenEditorRect.Height < 20) screenEditorRect.Height = 20;
			textBox = new RenamerTextBox();
			Application.AddMessageFilter(textBox);
			textBox.Controls[0].BackColor = Color.LightBlue;
			textBox.Bounds = screenEditorRect;
			EditorBeforeOpen(itemCore, textBox);
			textBox.Show();
			textBox.FormClosed += new FormClosedEventHandler(textBox_FormClosed);
		}
		void textBox_FormClosed(object sender, FormClosedEventArgs e) {
			EditorAfterClose(itemCore, textBox, textBox.DialogResult);
			Application.RemoveMessageFilter(textBox);
			textBox.Dispose();
			itemCore = null;
			textBox = null;
		}
		protected virtual void EditorAfterClose(BaseLayoutItem item, DevExpress.XtraEditors.Controls.ModalTextBox textBox, DialogResult result) {
			if(result != DialogResult.Cancel) {
				item.Text = textBox.EditText;
			}
		}
		protected virtual void EditorBeforeOpen(BaseLayoutItem item, DevExpress.XtraEditors.Controls.ModalTextBox textBox) {
			textBox.EditText = item.Text;
		}
	}
}
