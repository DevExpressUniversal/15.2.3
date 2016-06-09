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
	public abstract class EditorPropertyLineBase : PropertyLineBase {
		string labelText;
		protected BaseEdit baseEdit;
		protected IStringConverter converter;
		protected LabelControl label;
		public override bool HasError { get { return baseEdit != null && !String.IsNullOrEmpty(baseEdit.ErrorText); } }
		protected virtual bool ShouldUpdateValue { get { return baseEdit is PopupBaseEdit && ((PopupBaseEdit)baseEdit).Properties.TextEditStyle == TextEditStyles.Standard; } }
		protected EditorPropertyLineBase(IStringConverter converter, PropertyDescriptor property, object obj)
			: base(property, obj) {
			this.converter = converter;
		}
		public override void SetText(string labelText) {
			this.labelText = labelText;
		}
		public override Size GetLineSize() {
			Size size = label.Size;
			size.Width += baseEdit.Width;
			size.Height = Math.Max(size.Height, baseEdit.Height);
			return size;
		}
		protected override System.Windows.Forms.Control[] GetControls() {
			return new System.Windows.Forms.Control[] { label, baseEdit };
		}
		public override void RefreshProperty() {
			base.RefreshProperty();
			SetEditText(Value);
		}
		public override void CommitChanges() {
			try {
				if(ShouldUpdateValue)
					UpdateValue(baseEdit);
			} catch {
			}
		}
		protected override void Initialize() {
			label = new LabelControl();
			SetParentLookAndFeel(label);
			label.AutoSize = true;
			IntPtr ignore = label.Handle;
			label.Text = labelText;
			label.Dock = DockStyle.Left;
			baseEdit = CreateEditor();
			SetEditText(Value);
			label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
			IntializeEditor();
			label.Padding = new Padding(0, (baseEdit.Height - label.Height) / 2, 3, 0);
			RefreshProperty();
		}
		protected abstract BaseEdit CreateEditor();
		protected string ValueToString(object val) {
			return converter != null ? converter.ConvertToString(val) : string.Empty;
		}
		protected virtual void OnBaseEditValidating(object sender, CancelEventArgs e) {
			try {
				UpdateValue((BaseEdit)sender);
			} catch(Exception ex) {
				((BaseEdit)sender).ErrorText = ex.Message;
				e.Cancel = true;
			}
		}
		protected override void OnValueSet() {
			SetEditText(Value);
			base.OnValueSet();
		}
		protected virtual void SetEditText(object val) {
		}
		void UpdateValue(BaseEdit edit) {
			if(!DisableUpdatingValue)
				UpdateValueCore(edit);
		}
		protected virtual void UpdateValueCore(BaseEdit edit) {
			string text = edit.Text;
			if(converter.CanConvertFromString()) {
				object val = converter.ConvertFromString(text);
				if(!Object.Equals(Value, val))
					Value = val;
			}
		}
		protected virtual void IntializeEditor() {
			SetParentLookAndFeel(baseEdit);
			baseEdit.TabStop = true;
			baseEdit.Width = 170;
			baseEdit.Dock = DockStyle.Right;
			baseEdit.Properties.Appearance.TextOptions.Trimming = Trimming.Character;
			baseEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
		}
	}
}
