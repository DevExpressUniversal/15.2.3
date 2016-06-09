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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors;
using DevExpress.XtraPrinting.Native.Lines;
using System.ComponentModel;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	class ComboBoxPropertyLine : EditorPropertyLine {
		ComboBoxItemWrapper[] items;
		internal ComboBoxEdit Editor { get { return (ComboBoxEdit)editor; } }
		public bool IsDropDownMode {
			get {
				return items != null;
			}
		}
		public ComboBoxPropertyLine(IStringConverter converter, object[] values, PropertyDescriptor property, object obj)
			: base(new ComboBoxEdit(), converter, property, obj) {
			if(values != null) {
				items = new ComboBoxItemWrapper[values.Length];
				for(int i = 0; i < values.Length; i++) {
					items[i] = new ComboBoxItemWrapper(values[i], converter);
				}
				Editor.DisplayMember = "Text";
				Editor.ShowEditorButtons = true;
				Editor.IsTextEditable = false;
				Editor.InvalidValueBehavior = Editors.Validation.InvalidValueBehavior.AllowLeaveEditor;
				Editor.SelectedIndexChanged += Editor_SelectedIndexChanged;
				FillEditor(items);
			} else {
				Editor.ShowEditorButtons = false;
				Editor.Validate += ValidateEditor;
				Editor.LostFocus += (o, e) => UpdateValue(Editor.Text);
			}
		}
		void Editor_SelectedIndexChanged(object sender, System.Windows.RoutedEventArgs e) {
			Value = items[Editor.SelectedIndex].Value;
		}
		void FillEditor(ComboBoxItemWrapper[] values) {
			foreach(ComboBoxItemWrapper item in values) {
				Editor.Items.Add(item);
				if(item != null && item.Value.Equals(Value)) {
					Editor.SelectedItem = item;
				}
			}
		}
		protected override void SetEditText(object value) {
			if(items == null) {
				Editor.Text = ValueToString(value);
			} else {
				Editor.EditValue = new ComboBoxItemWrapper(value, converter);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Editor.SelectedIndexChanged -= Editor_SelectedIndexChanged;
				Editor.Validate -= ValidateEditor;
				editor.LostFocus -= (o, e) => UpdateValue(Editor.Text);
			}
		}
	}
}
