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
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.Web.Design {
	public class MaskExpressionUITypeEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(MaskExpressionEditorForm form = new MaskExpressionEditorForm(value as String)) {
				if(form.ShowDialog() == DialogResult.OK)
					return form.EditValue;
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class MaskExpressionEditorForm : Form {
		string editValue;
		bool indexChangeLock;
		string savedCustomMask;
		public MaskExpressionEditorForm(string value) {
			InitializeComponent();
			PopulateStandardMasks();
			this.editValue = value;
			textBox_UserMask.Text = value;
			SyncListBox();
		}
		public string EditValue { get { return editValue; } }
		void PopulateStandardMasks() {
			listBox_StandardMasks.Items.AddRange(new ListItem[] {				
				new ListItem("(Custom)", ""),
				new ListItem("Numeric (5 digits)", "00000"),
				new ListItem("Numeric range", "<0..100>"),
				new ListItem("Priority", "<Low|*Normal|High>"),
				new ListItem("Phone number", "(999) 000-0000"),
				new ListItem("Phone number without area code", "000-0000"),
				new ListItem("Currency", "$<0..99999g>.<00..99>"),
				new ListItem("Social security number", "000-00-0000"),
				new ListItem("Time (12-hour)", "hh:mm tt"),
				new ListItem("Time (24-hour)", "HH:mm"),
				new ListItem("Short date", "MM/dd/yyyy"),
				new ListItem("Long date", "MMMM dd',' yyyy")
			});
		}
		void SyncListBox() {
			string text = textBox_UserMask.Text;
			for(int i = 1; i < listBox_StandardMasks.Items.Count; i++) {
				if((listBox_StandardMasks.Items[i] as ListItem).Mask == text) {
					listBox_StandardMasks.SelectedIndex = i;
					return;
				}
			}
			indexChangeLock = true;
			try {
				this.savedCustomMask = text;
				listBox_StandardMasks.SelectedIndex = 0;
			} finally {
				indexChangeLock = false;
			}
		}
		void listBox_StandardMasks_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(indexChangeLock) return;
			if(listBox_StandardMasks.SelectedIndex == 0)
				textBox_UserMask.Text = this.savedCustomMask;
			else
				textBox_UserMask.Text = (listBox_StandardMasks.SelectedItem as ListItem).Mask;
		}
		void textBox_UserMask_LostFocus(object sender, EventArgs e) {			
			SyncListBox();
		}
		void button_Ok_Click(object sender, System.EventArgs e) {
			this.editValue = textBox_UserMask.Text;
			DialogResult = DialogResult.OK;
			Close();
		}
		#region Designer generated code
		private Label label_StandardMasks;
		private Label label_UserMask;
		private Button button_Cancel;
		private Button button_Ok;
		private TextBox textBox_UserMask;
		private ListBox listBox_StandardMasks;
		private void InitializeComponent() {
			this.listBox_StandardMasks = new System.Windows.Forms.ListBox();
			this.label_StandardMasks = new System.Windows.Forms.Label();
			this.label_UserMask = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_Ok = new System.Windows.Forms.Button();
			this.textBox_UserMask = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			this.listBox_StandardMasks.FormattingEnabled = true;
			this.listBox_StandardMasks.Location = new System.Drawing.Point(12, 29);
			this.listBox_StandardMasks.Name = "listBox_StandardMasks";
			this.listBox_StandardMasks.Size = new System.Drawing.Size(352, 82);
			this.listBox_StandardMasks.TabIndex = 0;			
			this.listBox_StandardMasks.SelectedIndexChanged += new System.EventHandler(this.listBox_StandardMasks_SelectedIndexChanged);
			this.label_StandardMasks.AutoSize = true;
			this.label_StandardMasks.Location = new System.Drawing.Point(9, 9);
			this.label_StandardMasks.Name = "label_StandardMasks";
			this.label_StandardMasks.Size = new System.Drawing.Size(86, 13);
			this.label_StandardMasks.TabIndex = 1;
			this.label_StandardMasks.Text = "Standard masks:";
			this.label_UserMask.AutoSize = true;
			this.label_UserMask.Location = new System.Drawing.Point(9, 123);
			this.label_UserMask.Name = "label_UserMask";
			this.label_UserMask.Size = new System.Drawing.Size(36, 13);
			this.label_UserMask.TabIndex = 2;
			this.label_UserMask.Text = "Mask:";
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(289, 177);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 3;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;			
			this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Ok.Location = new System.Drawing.Point(208, 177);
			this.button_Ok.Name = "button_Ok";
			this.button_Ok.Size = new System.Drawing.Size(75, 23);
			this.button_Ok.TabIndex = 2;
			this.button_Ok.Text = "OK";
			this.button_Ok.UseVisualStyleBackColor = true;
			this.button_Ok.Click += new System.EventHandler(this.button_Ok_Click);
			this.textBox_UserMask.Location = new System.Drawing.Point(12, 139);
			this.textBox_UserMask.Name = "textBox_UserMask";
			this.textBox_UserMask.Size = new System.Drawing.Size(352, 20);
			this.textBox_UserMask.TabIndex = 1;			
			this.textBox_UserMask.LostFocus += new EventHandler(textBox_UserMask_LostFocus);
			this.AcceptButton = this.button_Ok;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(376, 212);
			this.Controls.Add(this.textBox_UserMask);
			this.Controls.Add(this.button_Ok);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.label_UserMask);
			this.Controls.Add(this.label_StandardMasks);
			this.Controls.Add(this.listBox_StandardMasks);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MaskExpressionEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mask Editor";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		class ListItem {
			string text, mask;
			public ListItem(string text, string mask) {
				this.text = text;
				this.mask = mask;
			}
			public string Text { get { return text; } }
			public string Mask { get { return mask; } }
			public override string ToString() {
				return Text;
			}
		}
	}
}
