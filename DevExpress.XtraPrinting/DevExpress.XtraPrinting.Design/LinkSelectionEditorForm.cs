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
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrintingLinks.Design
{
	public class LinkSelectionEditorForm : System.Windows.Forms.Form
	{
		#region TODO delete
		#endregion
		private System.Windows.Forms.CheckedListBox checkedListBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button upButton;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.Button selectButton;
		private System.Windows.Forms.Button unselButton;
		private System.ComponentModel.Container components = null;
		private PrintingSystem ps;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private CompositeLink link;
		private LinkCollection links;
		public LinkSelectionEditorForm(LinkCollection links, CompositeLink link) {
			InitializeComponent();
			this.links = links;
			this.link = link;
			this.ps = link.PrintingSystem;
			InitializeListBox();
			UpdateButtons();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.checkedListBox = new System.Windows.Forms.CheckedListBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.upButton = new System.Windows.Forms.Button();
			this.downButton = new System.Windows.Forms.Button();
			this.selectButton = new System.Windows.Forms.Button();
			this.unselButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			this.checkedListBox.Location = new System.Drawing.Point(8, 24);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(184, 184);
			this.checkedListBox.TabIndex = 0;
			this.checkedListBox.SelectedIndexChanged += new System.EventHandler(this.checkedListBox_SelectedIndexChanged);
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(200, 216);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(88, 25);
			this.okButton.TabIndex = 10;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(296, 216);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(88, 25);
			this.cancelButton.TabIndex = 11;
			this.cancelButton.Text = "Cancel";
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 12;
			this.label1.Text = "Selected Links :";
			this.upButton.Location = new System.Drawing.Point(200, 80);
			this.upButton.Name = "upButton";
			this.upButton.Size = new System.Drawing.Size(88, 25);
			this.upButton.TabIndex = 13;
			this.upButton.Text = "Move Up";
			this.upButton.Click += new System.EventHandler(this.upButton_Click);
			this.downButton.Location = new System.Drawing.Point(200, 112);
			this.downButton.Name = "downButton";
			this.downButton.Size = new System.Drawing.Size(88, 25);
			this.downButton.TabIndex = 14;
			this.downButton.Text = "Move Down";
			this.downButton.Click += new System.EventHandler(this.downButton_Click);
			this.selectButton.Location = new System.Drawing.Point(296, 80);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new System.Drawing.Size(88, 25);
			this.selectButton.TabIndex = 15;
			this.selectButton.Text = "Select All";
			this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
			this.unselButton.Location = new System.Drawing.Point(296, 112);
			this.unselButton.Name = "unselButton";
			this.unselButton.Size = new System.Drawing.Size(88, 25);
			this.unselButton.TabIndex = 16;
			this.unselButton.Text = "Unselect All";
			this.unselButton.Click += new System.EventHandler(this.unselButton_Click);
#if DXWhidbey
			this.AutoScaleMode = AutoScaleMode.None;
#else
			this.AutoScale = false;
#endif
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 245);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.unselButton,
																		  this.selectButton,
																		  this.downButton,
																		  this.upButton,
																		  this.label1,
																		  this.okButton,
																		  this.cancelButton,
																		  this.checkedListBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "LinkSelectionEditorForm";
			this.ShowInTaskbar = false;
			this.Text = "Link Selection Editor";
			this.ResumeLayout(false);
		}
		#endregion
		private void InitializeListBox() {
			if(ps == null) return;
			foreach(LinkBase link in links) {
				if(link.Equals(this.link) == false)
					checkedListBox.Items.Add(link.Site.Name, CheckState.Checked);
			}
			foreach(LinkBase link in ps.Links) {
				if(link.Equals(this.link) == false && links.Contains(link) == false)
					checkedListBox.Items.Add(link.Site.Name, CheckState.Unchecked);
			}
			if(checkedListBox.Items.Count > 0)
				checkedListBox.SelectedIndex = 0;
		}
		private void UpdateButtons() {
			int index = checkedListBox.SelectedIndex;
			int count = checkedListBox.Items.Count;
			upButton.Enabled = (index > 0) ? true : false;
			downButton.Enabled = (index < count - 1) ? true : false;
		}
		private LinkBase FindLinkByName(string name) {
			foreach(LinkBase link in ps.Links)
				if( link.Site.Name.Equals(name) ) return link;
			return null;
		}
		private void okButton_Click(object sender, System.EventArgs e) {
			links.Clear();
			for(int i = 0; i < checkedListBox.Items.Count; i++) {
				if(checkedListBox.GetItemCheckState(i) != CheckState.Checked)
					continue;
				string name = (string)checkedListBox.Items[i];
				LinkBase link = FindLinkByName(name);
				if(link != null) links.Add(link);
			}
		}
		private void MoveListItem(int index, int offset) {
			object item = checkedListBox.Items[index];
			CheckState check = checkedListBox.GetItemCheckState(index); 
			checkedListBox.Items.RemoveAt(index);
			index += offset;
			checkedListBox.Items.Insert(index, item);
			checkedListBox.SetItemCheckState(index, check);
		}
		private void upButton_Click(object sender, System.EventArgs e) {
			int index = checkedListBox.SelectedIndex;
			if(index <= 0) return;
			MoveListItem(index, -1);
			checkedListBox.SelectedIndex = index - 1;
		}
		private void downButton_Click(object sender, System.EventArgs e) {
			int index = checkedListBox.SelectedIndex;
			if(index < 0 || index >= checkedListBox.Items.Count - 1) return;
			MoveListItem(index, 1);
			checkedListBox.SelectedIndex = index + 1;
		}
		private void selectButton_Click(object sender, System.EventArgs e) {
			for(int i = 0; i < checkedListBox.Items.Count; i++)
				checkedListBox.SetItemCheckState(i, CheckState.Checked);
		}
		private void unselButton_Click(object sender, System.EventArgs e) {
			for(int i = 0; i < checkedListBox.Items.Count; i++)
				checkedListBox.SetItemCheckState(i, CheckState.Unchecked);
		}
		private void checkedListBox_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateButtons();
		}
	}
}
