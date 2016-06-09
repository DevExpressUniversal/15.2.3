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
using System.Xml;
namespace DevExpress.XtraPrinting.Design
{
	internal class SelectAreaEditorForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;
		private SelectAreaEditor editor;
		private System.Windows.Forms.CheckedListBox listBox;
		private BrickModifier editValue;
		private BrickModifier oldValue;
		public BrickModifier EditValue { 
			get { return editValue; }
			set {
				editValue = value;
				UpdateItems(editValue);
			}
		}
		public SelectAreaEditorForm(SelectAreaEditor editor, BrickModifier value) {
			InitializeComponent();
			InitListBox();
			listBox.SelectedIndex = 0;
			Rectangle r = listBox.GetItemRectangle(listBox.Items.Count - 1);
			this.ClientSize = new Size(0, r.Bottom + r.Height);
			TopLevel = false;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.editor = editor;
			EditValue = value;
			oldValue = editValue;
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
		private void InitializeComponent()
		{
			this.listBox = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBox.CheckOnClick = true;
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.IntegralHeight = false;
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(158, 166);
			this.listBox.TabIndex = 0;
			this.listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBox_ItemCheck);
#if DXWhidbey
			this.AutoScaleMode = AutoScaleMode.None;
#else
			this.AutoScale = false;
#endif
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(158, 166);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.listBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SelectAreaEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
			this.TopMost = true;
			this.ResumeLayout(false);
		}
		#endregion
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				editValue = GetModifierValue();
				editor.edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				editValue = oldValue;
				editor.edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		private void UpdateItems(BrickModifier modifier) {
			for(int i = 0; i < listBox.Items.Count; i++) {
				BrickModifier itemModifier = (BrickModifier)listBox.Items[i];
				bool check = ((itemModifier & modifier) > 0) ? true : false;
				listBox.SetItemChecked(i, check);
			}
		}
		private BrickModifier GetModifierValue() {
			BrickModifier modifier = BrickModifier.None;
			foreach(BrickModifier item in listBox.Items)
				if( listBox.CheckedItems.Contains(item) )
					modifier |= item;
			return modifier;
		}
		private void InitListBox() {
			BrickModifier[] modifiers = {	BrickModifier.MarginalHeader, BrickModifier.MarginalFooter,
											BrickModifier.InnerPageHeader, BrickModifier.InnerPageFooter, 
											BrickModifier.ReportHeader, BrickModifier.ReportFooter, 
											BrickModifier.DetailHeader, BrickModifier.DetailFooter};
			foreach(object item in modifiers)
				listBox.Items.Add(item);
		}
		private void listBox_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e) {
			editValue = GetModifierValue();
			BrickModifier modifier = (BrickModifier)listBox.Items[e.Index];
			if(e.NewValue == CheckState.Checked)
				editValue |= modifier;
			else
				editValue &= ~modifier;
		}
	}
}
