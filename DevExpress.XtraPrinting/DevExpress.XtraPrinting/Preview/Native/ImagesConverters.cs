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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class ImageItemTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				ImageItem imageItem = (ImageItem)value;
				ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(Image) });
				return new InstanceDescriptor(ci, new object[] { imageItem.Name, imageItem.Image });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ImageCollectionTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType != typeof(string) && base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return new CollectionConverter().ConvertTo(value, typeof(string));
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			List<PropertyDescriptor> propertyList = new List<PropertyDescriptor>();
			foreach(string imageName in RibbonImageCollection.ImageNames)
				propertyList.Add(new ImageItemPropertyDescriptor(imageName));
			return new PropertyDescriptorCollection(propertyList.ToArray());
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	class NonExpandableImageConverter : ImageConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return null;
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	class ImageItemPropertyDescriptor : PropertyDescriptor {
		TypeConverter converter;
		public override Type ComponentType {
			get { return typeof(RibbonImageCollection); }
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override Type PropertyType {
			get { return typeof(Image); }
		}
		public override TypeConverter Converter {
			get {
				if(this.converter == null) {
					try {
						TypeConverter imageConverter = TypeDescriptor.GetConverter(typeof(Image));
						Type type = imageConverter.GetType();
						if(type.IsGenericType && type.GetGenericArguments().Length == 1) {
							type = type.GetGenericTypeDefinition().MakeGenericType(new Type[] { typeof(NonExpandableImageConverter) });
							this.converter = (TypeConverter)Activator.CreateInstance(type);
						}
					} finally {
						if(this.converter == null)
							this.converter = new NonExpandableImageConverter();
					}
				}
				return this.converter;
			}
		}
		public ImageItemPropertyDescriptor(string propertyName)
			: base(propertyName, null) {
		}
		public override bool CanResetValue(object component) {
			return true;
		}
		public override void ResetValue(object component) {
			SetValue(component, null);
		}
		public override object GetValue(object component) {
			return ((RibbonImageCollection)component).GetImage(this.Name);
		}
		public override void SetValue(object component, object value) {
			((RibbonImageCollection)component).SetImage(this.Name, (Image)value);
		}
		public override bool ShouldSerializeValue(object component) {
			return ((RibbonImageCollection)component).GetImage(this.Name) != null;
		}
	}
}
