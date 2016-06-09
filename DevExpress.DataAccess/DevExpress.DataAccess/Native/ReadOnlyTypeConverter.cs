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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Native {
	public class ReadOnlyTypeConverter : ExpandableObjectConverter {
		class ReadOnlyPropertyDescriptor : PropertyDescriptor {
			readonly PropertyDescriptor original;
			public ReadOnlyPropertyDescriptor(PropertyDescriptor original) : base(original) { this.original = original; }
			#region Overrides of PropertyDescriptor
			public override bool CanResetValue(object component) { return original.CanResetValue(component); }
			public override object GetValue(object component) { return original.GetValue(component); }
			public override void ResetValue(object component) { original.ResetValue(component); }
			public override void SetValue(object component, object value) { original.SetValue(component, value); }
			public override bool ShouldSerializeValue(object component) { return original.ShouldSerializeValue(component); }
			public override Type ComponentType {
				get { return original.ComponentType; }
			}
			public override bool IsReadOnly {
				get { return true; }
			}
			public override Type PropertyType {
				get { return original.PropertyType; }
			}
			#endregion
		}
		#region Overrides of ExpandableObjectConverter
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return
				new PropertyDescriptorCollection(
					base.GetProperties(context, value, attributes)
						.Cast<PropertyDescriptor>()
						.Select(pd => (PropertyDescriptor)new ReadOnlyPropertyDescriptor(pd))
						.ToArray(), true);
		}
		#endregion
		#region Overrides of TypeConverter
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string))
				return value == null ? null : value.GetType().Name;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		#endregion
	}
}
