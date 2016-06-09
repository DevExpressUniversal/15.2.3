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
using DevExpress.XtraPrinting.Native.Lines;
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.XtraPrinting;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	public abstract class PropertyLine : LineBase {
		PropertyDescriptor property;
		object obj;
		public event EventHandler ValueChanged;
		public PropertyDescriptor Property { get { return property; } }
		public object Value {
			get {
				return property.GetValue(obj);
			}
			set {
				if(!object.Equals(value, Value)) {
					property.SetValue(obj, value);
					OnValueSet();
				}
			}
		}
		protected PropertyLine(PropertyDescriptor property, object obj) {
			this.property = property;
			this.obj = obj;
		}
		public override void RefreshContent() {
			if(obj is ExportOptionsBase && property.Converter != null) {
				bool lineIsEnabled = property.Converter.CanConvertFrom(new RuntimeTypeDescriptorContext(property, obj), typeof(string));
				Content.IsEnabled = lineIsEnabled;
				if(Header != null)
					Header.IsEnabled = lineIsEnabled;
			}
		}
		protected virtual void OnValueSet() {
			if(ValueChanged != null) {
				ValueChanged(this, EventArgs.Empty);
			}
		}
	}
}
