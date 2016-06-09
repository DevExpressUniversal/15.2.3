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
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using System.Windows.Data;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	abstract class EditorPropertyLine : EditorPropertyLineBase {
		protected readonly IStringConverter converter;
		protected EditorPropertyLine(BaseEdit editor, IStringConverter converter, PropertyDescriptor property, object obj)
			: base(editor, property, obj) {
			this.converter = converter;
			editor.ValidateOnTextInput = true;
			editor.ValidateOnEnterKeyPressed = true;
		}
		public override void RefreshContent() {
			base.RefreshContent();
			SetEditText(Value);
		}
		protected override void OnValueSet() {
			SetEditText(Value);
			base.OnValueSet();
		}
		protected string ValueToString(object value) {
			return converter.ConvertToString(value);
		}
		protected virtual void ValidateEditor(object sender, ValidationEventArgs e) {
			try {
				if(converter.CanConvertFromString()) {
					converter.ConvertFromString(e.Value != null ? e.Value.ToString() : null);
				}
			}
			catch(NotSupportedException exception) {
				OnUpdateValueError(e, exception);
			}
			catch(InvalidOperationException exception){
				OnUpdateValueError(e, exception);		   
			}
		}
		void OnUpdateValueError(ValidationEventArgs e, Exception exception) {
			e.ErrorContent = exception.Message;
			e.IsValid = false;
			e.Handled = true;
		}
		protected void UpdateValue(string text) {
			if(converter.CanConvertFromString()) {
				Value = converter.ConvertFromString(text);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				editor.Validate -= ValidateEditor;
			}
		}
		protected abstract void SetEditText(object value);
	}
}
