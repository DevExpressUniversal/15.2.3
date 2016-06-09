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
using System.Globalization;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.Utils.Design;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	public class EFParameterCollectionTypeConverter : CollectionTypeConverter {
		class EFParameterPropertyDescriptor : SimplePropertyDescriptor {
			class EFParameterExpandableObjectConverter : ExpandableObjectConverter {
				class EFParameterNamePropertyDescriptor : SimplePropertyDescriptor {
					public EFParameterNamePropertyDescriptor()
						: base(typeof(EFParameter), "Name", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
					}
					public override object GetValue(object component) {
						return ((EFParameter)component).Name;
					}
					public override void SetValue(object component, object value) {
						throw new NotSupportedException();
					}
				}
				class EFParameterTypePropertyDescriptor : SimplePropertyDescriptor {
					public EFParameterTypePropertyDescriptor()
						: base(typeof(EFParameter), "Type", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
					}
					public override object GetValue(object component) {
						EFParameter efParameter = component as EFParameter;   
						if(efParameter != null)
							return efParameter.Type.ToString();
						return string.Empty;
					}
					public override void SetValue(object component, object value) {
						throw new NotSupportedException();
					}
				}
				class EFParameterValuePropertyDescriptor : SimplePropertyDescriptor {
					public EFParameterValuePropertyDescriptor()
						: base(typeof(EFParameter), "Value", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
					}
					public override object GetValue(object component) {
						return Convert.ToString(((EFParameter)component).Value);
					}
					public override void SetValue(object component, object value) {
						throw new NotSupportedException();
					}
				}
				public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
					List<PropertyDescriptor> result = new List<PropertyDescriptor>();
					result.Add(new EFParameterNamePropertyDescriptor());
					result.Add(new EFParameterTypePropertyDescriptor());
					result.Add(new EFParameterValuePropertyDescriptor());
					return new PropertyDescriptorCollection(result.ToArray());
				}
				public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
					return true;
				}
				public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
					if(destinationType == typeof(string))
						return true;
					return base.CanConvertTo(context, destinationType);
				}
				public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
					if(destinationType == typeof(string))
						return ((EFParameter)value).Name;
					return base.ConvertTo(context, culture, value, destinationType);
				}
			}
			int parameterIndex;
			public EFParameterPropertyDescriptor(int index)
				: base(typeof(EFParameterCollection), string.Format("[{0}]", index), typeof(EFParameter),
					new Attribute[] { 
						new TypeConverterAttribute(typeof(EFParameterExpandableObjectConverter))
					}
				) {
				this.parameterIndex = index;
			}
			public override object GetValue(object component) {
				EFParameterCollection collection = (EFParameterCollection)component;
				return collection[this.parameterIndex];
			}
			public override void SetValue(object component, object value) {
				EFParameterCollection collection = (EFParameterCollection)component;
				collection[this.parameterIndex] = (EFParameter)value;
			}
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			EFParameterCollection collection = (EFParameterCollection)value;
			PropertyDescriptor[] result = new EFParameterPropertyDescriptor[collection.Count];
			for(int i = 0; i < result.Length; i++)
				result[i] = new EFParameterPropertyDescriptor(i);
			return new PropertyDescriptorCollection(result);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
