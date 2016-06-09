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
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public class DataBindingConverter : TypeConverter {
		#region inner classes
		class FormatStringPropertyDescriptorWrapper : PropertyDescriptorWrapper {
			public FormatStringPropertyDescriptorWrapper(PropertyDescriptor oldPropertyDescriptor)
				: base(oldPropertyDescriptor, new Attribute[] { new TypeConverterAttribute(typeof(FormatStringTypeConverter))}) {
			}
		}
		class FormatStringTypeConverter : StringConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return context != null ? DataBinding.IsThereBinding(context.Instance) : base.CanConvertFrom(context, sourceType);
			}
		}
		#endregion
		public DataBindingConverter() { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(typeof(DataBinding), attributes);
			if(!DataBinding.IsThereBinding(value))
				collection = new PropertyDescriptorCollection(new PropertyDescriptor[] { collection["Binding"], new FormatStringPropertyDescriptorWrapper(collection["FormatString"]) });
			return collection.Sort(new string[] { "Binding", "FormatString" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class MultiDataBindingConverter : DataBindingConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { collection["FormatString"] });
		}
	}
}
