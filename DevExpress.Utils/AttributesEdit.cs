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

namespace DevExpress.Utils.Editors
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Windows.Forms;
	using System.Windows.Forms.Design;
	using System.Drawing.Design;
	using System.ComponentModel;
	internal class AttributesListBox : CheckedListBox {
		int lockUpdate = 0;
		public AttributesListBox() {
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.IntegralHeight = false;
		}
		public void ListBoxBeginUpdate() {
			lockUpdate++;
			BeginUpdate();
		}
		public void ListBoxEndUpdate() {
			--lockUpdate;
			EndUpdate();
			if(lockUpdate == 0) Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(lockUpdate != 0) return;
			base.OnPaint(e);
		}
	}
	internal class AttributesEditorForm : Panel {
		AttributesEditor mainEditor;
		AttributesListBox listBox;
		private object editValue, originalValue;
		int lockCheckUpdate;
		public AttributesEditorForm(AttributesEditor editor) {
			lockCheckUpdate = 0;
			mainEditor = editor;
			this.BorderStyle = BorderStyle.None;
			listBox = new AttributesListBox();
			listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			listBox.Dock = DockStyle.Fill;
			listBox.ItemCheck += new ItemCheckEventHandler(listBox_ItemCheckEventHandler);
			Controls.Add(listBox);
			this.ClientSize = new Size(0, (listBox.ItemHeight + 1) * 7);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				mainEditor.edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				editValue = originalValue;
				mainEditor.edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected void listBox_ItemCheckEventHandler(object sender, ItemCheckEventArgs e) {
			if(lockCheckUpdate != 0) return;
			BeginUpdate();
			listBox.BeginUpdate();
			try {
				string optionName = listBox.Items[e.Index].ToString();
				if(e.NewValue == CheckState.Checked) 
					EnableOption(optionName);
				else
					DisableOption(optionName);
				UpdateListBox();
			}
			finally {
				listBox.EndUpdate();
				EndUpdate();
			}
		}
		void BeginUpdate() {
			lockCheckUpdate++;
		}
		void EndUpdate() {
			lockCheckUpdate --;
		}
		void SelectAll() {
			foreach(string optionName in listBox.Items) {
				if(optionName != noneOption) EnableOption(optionName);
			}
		}
		void ClearAll() {
			foreach(string optionName in listBox.Items) {
				if(optionName != noneOption) DisableOption(optionName);
			}
		}
		void UpdateListBox() {
			listBox.ListBoxBeginUpdate();
			try {
				for(int n = 0; n < listBox.Items.Count; n++) {
					string optionName = listBox.Items[n].ToString();
					listBox.SetItemChecked(n, IsOptionEnabled(optionName));
				}
			}
			finally {
				listBox.ListBoxEndUpdate();
			}
		}
		protected int GetOptionValue(string optionName) {
			Type t = EditValue.GetType();
			return (int)t.GetField(optionName).GetValue(EditValue);
		}
		protected void ConvertFromInt(int value) {
			editValue = Enum.ToObject(EditValue.GetType(), value);
		}
		protected void DisableOption(string optionName) {
			if(optionName == noneOption) {
				SelectAll();
				return;
			}
			ConvertFromInt((int)editValue & (~GetOptionValue(optionName)));
		}
		protected void EnableOption(string optionName) {
			if(optionName == noneOption) {
				ClearAll();
				return;
			}
			ConvertFromInt((int)editValue | GetOptionValue(optionName));
		}
		protected bool IsOptionEnabled(string optionName) {
			int opt = (int)editValue;
			if(optionName == noneOption) return opt == GetOptionValue(optionName);
			return (opt & GetOptionValue(optionName)) == GetOptionValue(optionName);
		}
		public object EditValue {
			get { return editValue; }
			set {
				if(editValue == value) return;
				originalValue = editValue = value;
				listBox.Items.Clear();
				ArrayList list = GetFields(editValue.GetType());
				BeginUpdate();
				try {
					foreach(System.Reflection.FieldInfo fi in list) {
						listBox.Items.Add(fi.Name, IsOptionEnabled(fi.Name) ? CheckState.Checked : CheckState.Unchecked);
					}
				}
				finally {
					EndUpdate();
				}
				int maxItems = Math.Min(listBox.Items.Count, 15);
				this.ClientSize = new Size(this.Size.Width, (listBox.ItemHeight + 1) * maxItems);
			}
		}
		protected ArrayList GetFields(Type t) {
			ArrayList list = new ArrayList();
			System.Reflection.FieldInfo[] fields = t.GetFields();
			foreach(System.Reflection.FieldInfo field in fields) {
				if(field.IsSpecialName) continue;
				list.Add(field);
				if(GetOptionValue(field.Name) == 0) noneOption = field.Name;
			}
			list.Sort(new FieldsComparer(noneOption));
			return list;
		}
		string noneOption = "None";
		class FieldsComparer : IComparer {
			string noneOption;
			public FieldsComparer(string noneOption) {
				this.noneOption = noneOption;
			}
			int IComparer.Compare(object x, object y) {
				System.Reflection.FieldInfo fi1 = x as System.Reflection.FieldInfo;
				System.Reflection.FieldInfo fi2 = y as System.Reflection.FieldInfo;
				if(fi1 == null || fi2 == null) return Comparer.Default.Compare(x, y);
				if(fi1 == fi2) return 0;
				if(fi1.Name == noneOption) return -1;
				if(fi2.Name == noneOption) return 1;
				return Comparer.Default.Compare(fi1.Name, fi2.Name);
			}
		}
	}
	public class AttributesEditor : UITypeEditor {
		internal IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return value;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null) return value;
			AttributesEditorForm form = new AttributesEditorForm(this);
			form.EditValue = value;
			edSvc.DropDownControl(form);
			value = form.EditValue;
			form.Dispose();
			edSvc = null;
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
	}
}
