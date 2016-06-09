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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data;
namespace DevExpress.Utils.Design {
	public class FontTypeConverter : FontConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Font), attributes);
			return new AttributeHelper(typeof(ResFinder)).LocalizeProperties(properties);
		}
	}
	public class AttributeHelper {
		Type resourceFinder;
		public AttributeHelper(Type resourceFinder) {
			this.resourceFinder = resourceFinder;
		}
		public virtual PropertyDescriptorCollection LocalizeProperties(PropertyDescriptorCollection properties) {
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				props.Add(TypeDescriptorHelper.CreateProperty(propertyDescriptor.ComponentType, propertyDescriptor, GetPropertyAttributes(resourceFinder, propertyDescriptor)));
			}
			PropertyDescriptorCollection result = new PropertyDescriptorCollection(props.ToArray());
			return result;
		}
		public static Attribute[] GetPropertyAttributes(Type resourceFinder, PropertyDescriptor propertyDescriptor) {
			List<Attribute> result = new List<Attribute>();
			string name = string.Format("{0}.{1}", propertyDescriptor.ComponentType, propertyDescriptor.Name);
			result.Add(new DXDisplayNameAttribute(resourceFinder, name));
			if(propertyDescriptor.PropertyType.Equals(typeof(Boolean)))
				result.Add(new TypeConverterAttribute(typeof(DevExpress.Utils.Design.BooleanTypeConverter)));
			else if(propertyDescriptor.PropertyType.Equals(typeof(Size)))
				result.Add(new TypeConverterAttribute(typeof(DevExpress.Utils.Design.SizeTypeConverter)));
			else if(propertyDescriptor.PropertyType.Equals(typeof(SizeF)))
				result.Add(new TypeConverterAttribute(typeof(DevExpress.Utils.Design.SizeFTypeConverter)));
			return result.ToArray();
		}
	}
}
