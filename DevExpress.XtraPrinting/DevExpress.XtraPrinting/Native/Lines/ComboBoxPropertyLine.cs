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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Native.Lines {
	public class ComboBoxPropertyLine : EditorPropertyLineBase {
		IEnumerable values;
		object[] editValues;
		object[] EditValues {
			get { return editValues; }
			set {
				if(!ArrayHelper.ArraysEqual<object>(value, editValues)) {
					editValues = value;
					FillComboBox();
				}
			}
		}
		protected ComboBoxEdit ComboBox {
			get { return (ComboBoxEdit)baseEdit; }
		}
		public ComboBoxPropertyLine(IStringConverter converter, IEnumerable values, PropertyDescriptor property, object obj)
			: base(converter, property, obj) {
			this.values = values;
		}
		protected override BaseEdit CreateEditor() {
			return new ComboBoxEdit();
		}
		static object[] GetEditValues(IEnumerable values) {
			List<object> editValues = new List<object>();
			if(values != null) {
				foreach(object value in values)
					editValues.Add(value);
			}
			return editValues.ToArray();
		}
		protected override void  IntializeEditor() {
 			base.IntializeEditor();
			EditValues = GetEditValues(values);
			UpdateEditor();
		}
		void UpdateEditor() {
			if(EditValues.Length > 0) {
				ComboBox.Properties.Buttons[0].Visible = true;
				ComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			} else {
				ComboBox.Properties.Buttons[0].Visible = false;
				ComboBox.Properties.ValidateOnEnterKey = true;
				ComboBox.Validating += OnBaseEditValidating;
			}
		}
		public override void RefreshProperty() {
			EditValues = GetEditValues(values);
			UpdateEditor();
			base.RefreshProperty();
		}
		void FillComboBox() {
			ComboBox.SelectedIndexChanged -= OnComboBoxIndexChanged;
			ComboBox.Properties.Items.Clear();
			foreach(object val in editValues) {
				string stringValue = ValueToString(val);
				ComboBox.Properties.Items.Add(stringValue);
				if(val != null && val.Equals(Value))
					ComboBox.SelectedItem = stringValue;
			}
			ComboBox.SelectedIndexChanged += OnComboBoxIndexChanged;
		}
		protected override void SetEditText(object val) {
			ComboBox.ToolTip = ComboBox.Text = ValueToString(val);
		}
		void OnComboBoxIndexChanged(object sender, EventArgs e) {
			Value = editValues[ComboBox.SelectedIndex] as object;
		}
	}
}
