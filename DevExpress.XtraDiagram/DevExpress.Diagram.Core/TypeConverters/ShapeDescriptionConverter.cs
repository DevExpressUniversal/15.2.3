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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Diagram.Core.Shapes.Native;
namespace DevExpress.Diagram.Core.TypeConverters {
	public class ShapeDescriptionTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return (destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is ShapeDescription) {
				ShapeDescription shape = (ShapeDescription)value;
				if((destinationType == typeof(string))) {
					return string.Join(".", DiagramToolboxRegistrator.GetStencilByShape(shape).Id, shape.Id);
				}
				if((destinationType == typeof(InstanceDescriptor))) {
					DiagramStencil category = DiagramToolboxRegistrator.GetStencilByShape(shape);
					if(category == null) {
						throw new ArgumentException("value");
					}
					Type ownerType = ShapeRegistratorHelper.GetShapeOwner(category.Id);
					PropertyInfo property = ownerType.GetProperty(shape.Id, BindingFlags.Static | BindingFlags.Public);
					if(property == null) {
						throw new ArgumentException("value");
					}
					return new InstanceDescriptor(property, null);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			var stringValue = value as string;
			if(stringValue != null) {
				var subStrings = stringValue.Split('.');
				if(subStrings.Length == 2) {
					string categoryId = subStrings[0];
					DiagramStencil category = DiagramToolboxRegistrator.GetStencil(categoryId);
					if(category == null)
						throw new ArgumentException("Invalid shape category", categoryId);
					string shapeId = subStrings[1];
					ShapeDescription description = category.GetShape(shapeId);
					if(description == null)
						throw new ArgumentException("Invalid shape name", shapeId);
					return description;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			var fieldNames = DiagramToolboxRegistrator.Stencils.SelectMany(category => category.Shapes.Select(shape => string.Format("{0}.{1}", category.Id, shape.Id))).ToList();
			return new StandardValuesCollection(fieldNames);
		}
	}
}
