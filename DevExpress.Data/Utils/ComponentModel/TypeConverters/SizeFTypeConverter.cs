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
using System.Globalization;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Utils.Design {
	public class SizeFTypeConverter : SizeFConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(destinationType == typeof(string) && value is SizeF) {
				SizeF ef = (SizeF)value;
				if(culture == null) {
					culture = CultureInfo.CurrentCulture;
				}
				string[] strArray = new string[2];
				strArray[0] = SingleTypeConverter.ToString(context, culture, ef.Width);
				strArray[1] = SingleTypeConverter.ToString(context, culture, ef.Height);
				string separator = culture.TextInfo.ListSeparator + " ";
				return string.Join(separator, strArray);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return PathProperties(typeof(SizeF), attributes);
		}
		static PropertyDescriptorCollection PathProperties(Type type, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type, attributes);
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				string name = string.Format("{0}.{1}", propertyDescriptor.ComponentType, propertyDescriptor.Name);
				props.Add(TypeDescriptorHelper.CreateProperty(type,
					propertyDescriptor,
					new Attribute[] {
						new TypeConverterAttribute(typeof(SingleTypeConverter)),
						new DXDisplayNameAttribute(typeof(ResFinder), name) }));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}
	}
}
