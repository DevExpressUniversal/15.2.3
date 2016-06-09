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
using System.Collections;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.XtraDataLayout {
	public class ControlBindingHelper {
		public static ArrayList GetBindablePropertiesList(Control control, bool getOnlyCommonBindings) {
			ControlBindingsCollection bindings;
			ArrayList commonProperties = new ArrayList();
			ArrayList allProperties = new ArrayList();
			bindings = control.DataBindings;
			try {
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(bindings.BindableComponent);
				for(int i = 0; i < properties.Count; i++) {
					if(!properties[i].IsReadOnly) {
						BindableAttribute bindableAttr = (BindableAttribute)properties[i].Attributes[typeof(BindableAttribute)];
						BrowsableAttribute browsableAttr = (BrowsableAttribute)properties[i].Attributes[typeof(BrowsableAttribute)];
						if(((browsableAttr == null) || browsableAttr.Browsable) || ((bindableAttr != null) && bindableAttr.Bindable)) {
							BindingInfo info = new BindingInfo(properties[i].Name);
							info.Binding = FindBinding(properties[i].Name, bindings);
							if(info.Binding != null) {
								info.FormatType = "Binding != null";
							}
							else {
								info.FormatType = "";
							}
							if((bindableAttr != null) && bindableAttr.Bindable) {
								commonProperties.Add(info);
							}
							else {
								allProperties.Add(info);
							}
						}
					}
				}
			}
			finally {
			}
			if(getOnlyCommonBindings) return commonProperties;
			else return allProperties;
		}
		private static Binding FindBinding(string propertyName, ControlBindingsCollection bindings) {
			for(int i = 0; i < bindings.Count; i++) {
				if(string.Equals(propertyName, bindings[i].PropertyName, StringComparison.OrdinalIgnoreCase)) {
					return bindings[i];
				}
			}
			return null;
		}
	}
	public class BindingInfo {
		Binding binding;
		string formatType;
		string name;
		public BindingInfo(string name) {
			this.name = name;
		}
		public Binding Binding {
			get {
				return this.binding;
			}
			set {
				this.binding = value;
			}
		}
		public string FormatType {
			get {
				return this.formatType;
			}
			set {
				this.formatType = value;
			}
		}
	}
}
