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
using System.Globalization;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
namespace DevExpress.Utils.Editors {
	public class ObjectEditor : System.Windows.Forms.Form {
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.Label lblType;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.TextBox txtEditor;
		private System.Windows.Forms.ComboBox cmbEditor;
		private System.Windows.Forms.DateTimePicker dtEditor;
		private System.ComponentModel.Container components = null;
		private Control Editor {
			get {
				switch (cmbType.SelectedIndex) {
					case 9: return cmbEditor;
					case 10: return dtEditor;
					case 11: return null;
				}
				return txtEditor;
			}
		}
		string objEditValue;
		public ObjectEditor(object editValue) {
			InitializeComponent();
			this.cmbType.Items.Clear();
			foreach (string str in ObjectEditor.ObjectTypeNames) {
				this.cmbType.Items.Add(str);
			}
			this.objEditValue = ConvertObjectToString(editValue);
			this.cmbType.SelectedIndex = (int)GetObjectType(editValue);
			btnOK.Enabled = false;
		}
		public enum ObjectType {
			String, Int16, Int32, Int64, Single, Double, Byte,
			Decimal, Char, Boolean, DateTime, Null
		};
		public static string[] ObjectTypeNames = new string[] { 
			"String", "Numeric (Int16)", "Numeric (Int32)", "Numeric (Int64)", "Numeric (Single)", "Numeric (Double)", "Numeric (Byte)", 
			"Numeric (Decimal)", "Char", "Boolean", "DateTime", "<Null>" };
		public static ObjectType GetObjectType(object val) {
			ObjectType objType = ObjectType.Null;
			if (val != null) {
				string type = "";
				try {
					type = val.GetType().ToString().Split('.')[1].ToLower(CultureInfo.InvariantCulture);
				}
				catch { }
				if (type == "boolean") return ObjectType.Boolean;
				else if (type == "datetime") return ObjectType.DateTime;
				else if (type == "int16") return ObjectType.Int16;
				else if (type == "int32") return ObjectType.Int32;
				else if (type == "int64") return ObjectType.Int64;
				else if (type == "single") return ObjectType.Single;
				else if (type == "double") return ObjectType.Double;
				else if (type == "byte") return ObjectType.Byte;
				else if (type == "decimal") objType = ObjectType.Decimal;
				else if (type == "char") objType = ObjectType.Char;
				else objType = ObjectType.String;
			}
			return objType;
		}
		public static object ConvertObject(ObjectType type, object val) {
			try {
				switch (type) {
					case ObjectType.Null: return null;
					case ObjectType.DateTime: return val == null ? DateTime.MinValue : DateTime.Parse(val.ToString());
					case ObjectType.Int16: return Convert.ToInt16(val);
					case ObjectType.Int32: return Convert.ToInt32(val);
					case ObjectType.Int64: return Convert.ToInt64(val);
					case ObjectType.Single: return Convert.ToSingle(val);
					case ObjectType.Double: return Convert.ToDouble(val);
					case ObjectType.Byte: return Convert.ToByte(val);
					case ObjectType.Decimal: return Convert.ToDecimal(val);
					case ObjectType.Boolean: return Convert.ToBoolean(val);
					case ObjectType.Char: return Convert.ToChar(val);
					default:
						return val == null ? "" : val.ToString();
				}
			}
			catch {
			}
			return null;
		}
		public static string ConvertObjectToString(object val) {
			ObjectType type = GetObjectType(val);
			switch (type) {
				case ObjectType.Null: return ObjectEditorTypeConverter.NullString;
				default:
					if (val == null) return "";
					return val.ToString();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public object EditValue {
			get {
				ObjectType type = (ObjectType)cmbType.SelectedIndex;
				switch (type) {
					case ObjectType.Int16: return Convert.ToInt16(txtEditor.Text);
					case ObjectType.Int32: return Convert.ToInt32(txtEditor.Text);
					case ObjectType.Int64: return Convert.ToInt64(txtEditor.Text);
					case ObjectType.Single: return Convert.ToSingle(txtEditor.Text);
					case ObjectType.Double: return Convert.ToDouble(txtEditor.Text);
					case ObjectType.Byte: return Convert.ToByte(txtEditor.Text);
					case ObjectType.Decimal: return Convert.ToDecimal(txtEditor.Text);
					case ObjectType.Char:
						string ret = txtEditor.Text.Trim();
						if (ret.Length > 0) return Convert.ToChar(ret[0]);
						return null;
					case ObjectType.Boolean: return cmbEditor.SelectedIndex == 0;
					case ObjectType.DateTime: return dtEditor.Value;
					case ObjectType.Null: return null;
					default: return txtEditor.Text;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				if (components != null)
					components.Dispose();
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.cmbType = new System.Windows.Forms.ComboBox();
			this.lblType = new System.Windows.Forms.Label();
			this.lblValue = new System.Windows.Forms.Label();
			this.txtEditor = new System.Windows.Forms.TextBox();
			this.cmbEditor = new System.Windows.Forms.ComboBox();
			this.dtEditor = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Location = new System.Drawing.Point(88, 80);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Location = new System.Drawing.Point(168, 80);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbType.Items.AddRange(new object[] {
			"String",
			"Numeric",
			"Numeric(decimal)",
			"Boolean",
			"DateTime",
			"<Null>"});
			this.cmbType.Location = new System.Drawing.Point(64, 12);
			this.cmbType.Name = "cmbType";
			this.cmbType.Size = new System.Drawing.Size(180, 21);
			this.cmbType.TabIndex = 3;
			this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
			this.lblType.Location = new System.Drawing.Point(8, 12);
			this.lblType.Name = "lblType";
			this.lblType.Size = new System.Drawing.Size(56, 20);
			this.lblType.TabIndex = 4;
			this.lblType.Text = "Type:";
			this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblValue.Location = new System.Drawing.Point(8, 44);
			this.lblValue.Name = "lblValue";
			this.lblValue.Size = new System.Drawing.Size(56, 20);
			this.lblValue.TabIndex = 5;
			this.lblValue.Text = "Value:";
			this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.txtEditor.Location = new System.Drawing.Point(64, 44);
			this.txtEditor.Name = "txtEditor";
			this.txtEditor.Size = new System.Drawing.Size(180, 20);
			this.txtEditor.TabIndex = 0;
			this.txtEditor.Visible = false;
			this.txtEditor.TextChanged += new System.EventHandler(this.editor_TextChanged);
			this.cmbEditor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbEditor.Items.AddRange(new object[] {
			"True",
			"False"});
			this.cmbEditor.Location = new System.Drawing.Point(64, 44);
			this.cmbEditor.Name = "cmbEditor";
			this.cmbEditor.Size = new System.Drawing.Size(180, 21);
			this.cmbEditor.TabIndex = 0;
			this.cmbEditor.Visible = false;
			this.cmbEditor.SelectedIndexChanged += new System.EventHandler(this.editor_TextChanged);
			this.dtEditor.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtEditor.Location = new System.Drawing.Point(64, 44);
			this.dtEditor.Name = "dtEditor";
			this.dtEditor.Size = new System.Drawing.Size(180, 20);
			this.dtEditor.TabIndex = 0;
			this.dtEditor.Visible = false;
			this.dtEditor.ValueChanged += new System.EventHandler(this.editor_TextChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AcceptButton = this.btnOK;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(250, 110);
			this.Controls.Add(this.txtEditor);
			this.Controls.Add(this.lblValue);
			this.Controls.Add(this.cmbType);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblType);
			this.Controls.Add(this.cmbEditor);
			this.Controls.Add(this.dtEditor);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "ObjectEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Object Editor";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		Control oldEditor = null;
		bool lockUpdate = false;
		private void cmbType_SelectedIndexChanged(object sender, System.EventArgs e) {
			lockUpdate = true;
			Control editor = Editor;
			if (editor != null) {
				if (oldEditor != null && oldEditor.Equals(txtEditor))
					objEditValue = oldEditor.Text;
				txtEditor.Visible = editor.Equals(txtEditor);
				cmbEditor.Visible = editor.Equals(cmbEditor);
				dtEditor.Visible = editor.Equals(dtEditor);
				if (editor.Equals(cmbEditor))
					try {
						cmbEditor.SelectedIndex = (Convert.ToBoolean(objEditValue) ? 0 : 1);
					}
					catch { cmbEditor.SelectedIndex = 0; }
				else if (editor.Equals(dtEditor))
					try {
						dtEditor.Value = Convert.ToDateTime(objEditValue);
					}
					catch { dtEditor.Value = DateTime.Now; }
				else txtEditor.Text = objEditValue;
				oldEditor = editor;
			}
			else
				txtEditor.Visible = cmbEditor.Visible = dtEditor.Visible = false;
			btnOK.Enabled = true;
			lockUpdate = false;
		}
		private void editor_TextChanged(object sender, System.EventArgs e) {
			if (!lockUpdate)
				btnOK.Enabled = true;
		}
	}
	public class UIObjectEditor : UITypeEditor {
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (context != null && context.Instance != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					try {
						ObjectEditor editor = new ObjectEditor(objValue);
						if (edSvc.ShowDialog(editor) == DialogResult.OK) {
							objValue = editor.EditValue;
						}
					}
					catch { }
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
