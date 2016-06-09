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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
namespace DevExpress.XtraEditors.Design {
	internal class MaskEditorForm : Panel { 
		public class MaskItem {
			internal MaskType maskType;
			internal string prefix;
			internal string text;
			public MaskItem(MaskType maskType, string prefix, string text) {
				this.maskType = maskType;
				this.prefix = prefix;
				this.text = text;
			}
		}
		private System.Windows.Forms.ListBox listBox;
		private System.ComponentModel.Container components = null;
		private MaskEditor editor;
		private string editValue;
		public string EditValue {
			get { return editValue; }
			set {
				editValue = value;
				SetSelectedItem(editValue);
			}
		}
		public MaskEditorForm(MaskEditor editor, MaskType type) {
			InitializeComponent();
			InitListBox(type);
			BorderStyle = BorderStyle.None;
			this.editor = editor;
			int index = Math.Min(7, listBox.Items.Count - 1);
			Rectangle r = listBox.GetItemRectangle(index);
			this.Size = new Size(0, r.Bottom);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
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
			this.listBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox.IntegralHeight = false;
			this.listBox.Items.AddRange(new object[] {
														 "",
														 "item1",
														 "item2",
														 "item3"});
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(158, 166);
			this.listBox.TabIndex = 0;
			this.listBox.Click += new System.EventHandler(this.listBox_Click);
			this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.ClientSize = new System.Drawing.Size(158, 166);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.listBox});
			this.Name = "MaskEditorForm";
			this.ResumeLayout(false);
		}
#endregion
		private void listBox_Click(object sender, System.EventArgs e) {
			editValue = GetText(listBox.SelectedItem);
			editor.edSvc.CloseDropDown();
		}
		private void listBox_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
			if(listBox.Items.Count == 0)
				return;
			e.DrawBackground();
			MaskItem item = ((MaskItem)listBox.Items[e.Index]);
			RectangleF r = e.Bounds;
			SizeF size = e.Graphics.MeasureString("WWWWg", e.Font);
			r.Width = size.Width;
			e.Graphics.DrawString(item.prefix, e.Font, new SolidBrush(e.ForeColor), r);
			r = e.Bounds;
			r.Offset(1.2f * size.Width, 0);
			e.Graphics.DrawString(item.text, e.Font, new SolidBrush(e.ForeColor), r);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				editValue = GetText(listBox.SelectedItem);
				editor.edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				editor.edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		private void SetSelectedItem(string s) {
			if(listBox.Items.Count == 0)
				return;
			foreach(MaskItem item in listBox.Items) {
				if( item.text.EndsWith(s) ) {
					listBox.SelectedItem = item;
					return;
				}
			}
			listBox.SelectedIndex = 0;
		}
		private void InitListBox(MaskType type) {
			listBox.Items.Clear();
			MaskItem item = new MaskItem(type, "", "");
			listBox.Items.Add(item);
			string path = System.Environment.SystemDirectory + "\\DevExpress.MaskCollection.xml";
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(path);
				XmlNodeList nodes = doc.GetElementsByTagName("Mask");
				foreach(XmlNode node in nodes) {
					item = ToMaskItem(node);
					if(item != null && item.maskType == type)
						listBox.Items.Add(item);
				}
			}
			catch {}
		}
		private MaskItem ToMaskItem(XmlNode node) {
			string s = (string)GetAttribute(node, "type");
			MaskType type = ToMaskType(s);
			if(type == MaskType.None) return null;
			string prefix = (string)GetAttribute(node, "prefix");
			string text = (string)GetAttribute(node, "text");
			return new MaskItem(type, prefix, text);
		}
		private MaskType ToMaskType(string s) {
			string value = s.ToLower();
			return value.Equals("simple") ? MaskType.Simple :
				value.Equals("regular") ? MaskType.Regular : MaskType.None;
		}
		private string GetText(object item) {
			return (item is MaskItem) ? ((MaskItem)item).text : "";
		}
		private object GetAttribute(XmlNode node, string name) {
			object obj = node.Attributes[name].InnerText;
			if(obj == null) {
				obj = node.Attributes[name.ToLower()].InnerText;
				if(obj == null) obj = node.Attributes[name.ToUpper()].InnerText;
			}
			return obj;
		}
	}
}
