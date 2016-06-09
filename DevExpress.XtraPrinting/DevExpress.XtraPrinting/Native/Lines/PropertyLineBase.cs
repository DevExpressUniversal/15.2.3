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
	public abstract class PropertyLineBase : BaseLine {
		PropertyDescriptor property;
		object obj;
		public PropertyDescriptor Property {
			get { return property; }
		}
		public object Value {
			get {
				return Property.GetValue(obj);
			}
			set {
				System.Diagnostics.Debug.Assert(LinesContainer != null);
				if(!Object.Equals(value, Value))
					Property.SetValue(obj, value);
				OnValueSet();
			}
		}
		protected PropertyLineBase(PropertyDescriptor property, object obj)
			: base() {
			this.property = property;
			this.obj = obj;
		}
		protected virtual void OnValueSet() {
			RefreshLines();
		}
		public override void RefreshProperty() {
			if(!(Property is DevExpress.XtraReports.Native.Parameters.ParameterPropertyDescriptor) && Property.Converter != null) {
				ITypeDescriptorContext context = new RuntimeTypeDescriptorContext(Property, obj);
				Enabled = GetEditStyle(context) != UITypeEditorEditStyle.None ||
					   Property.Converter.CanConvertFrom(context, typeof(string)) || Property.Converter.GetStandardValuesSupported(context);
			}
		}
		UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			UITypeEditor editor = Property.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			return editor != null ? editor.GetEditStyle(context) : UITypeEditorEditStyle.None;
		}
		protected void RefreshLines() {
			RefreshPropertiesAttribute attr = property.Attributes[typeof(RefreshPropertiesAttribute)] as RefreshPropertiesAttribute;
			if(LinesContainer != null && attr != null && attr.RefreshProperties == System.ComponentModel.RefreshProperties.All) {
				LinesContainer.RefreshLines(this);
			}
		}
	}
}
