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

using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
namespace DevExpress.XtraReports.Design {
	public class ParameterBindingConverter : TypeConverter {
		static PropertyDescriptor BindingProperty {
			get { return TypeDescriptor.GetProperties(typeof(DataBinding))[XRComponentPropertyNames.Binding]; }
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				ParameterBinding binding = (ParameterBinding)value;
				if(binding.Parameter != null) {
					System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(Parameter) });
					return new InstanceDescriptor(ci, new object[] { binding.ParameterName, binding.Parameter });
				} else {
					ISite site = binding.Owner.Parent != null ? binding.Owner.Site : null;
					object dataSource = XRBindingConverter.ValidateDataSource(binding.SerializeDataSource, site);
					System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(object), typeof(string) });
					return new InstanceDescriptor(ci, new object[] { binding.ParameterName, dataSource, binding.DataMember });
				}
			} else if(destinationType == typeof(string)) {
				ParameterBinding binding = (ParameterBinding)value;
				return binding.GetType().Name + (binding.Owner != null ? 
					(binding.Owner.ParameterBindings.IndexOf(binding) + 1).ToString() :
					string.Empty)
					+ (string.IsNullOrEmpty(binding.ParameterName) ? string.Empty : (": " + binding.ParameterName));
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			if(context != null && context.Instance != null) {
				ParameterBinding binding = (ParameterBinding)context.Instance;
				return new PropertyDescriptorCollection(
					new PropertyDescriptor[] {
						TypeDescriptor.GetProperties(binding)[XRComponentPropertyNames.ParameterName],
						new DesignBindingPropertyDescriptor(BindingProperty)
					});
			}
			return base.GetProperties(context, value, attributes);
		}
	}
	class DesignBindingPropertyDescriptor : DevExpress.XtraReports.Native.PropertyDescriptorWrapper {
		public DesignBindingPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor)
			: base(oldPropertyDescriptor) {
		}
		public override bool CanResetValue(object component) {
			return !((DesignBinding)GetValue(component)).IsNull;
		}
		public override Type ComponentType {
			get { return typeof(ParameterBinding); }
		}
		public override Type PropertyType {
			get { return typeof(DesignBinding); }
		}
		public override object GetValue(object component) {
			return GetDesignBinding((ParameterBinding)component);
		}
		public override void SetValue(object component, object value) {
			UpdateBinding((ParameterBinding)component, (DesignBinding)value);
		}
		public override void ResetValue(object component) {
			SetValue(component, new DesignBinding());
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override bool ShouldSerializeValue(object component) {
			return !((DesignBinding)GetValue(component)).IsNull;
		}
		DesignBinding GetDesignBinding(ParameterBinding binding) {
			if(binding.IsEmpty)
				return DesignBinding.Null;
			if(binding.Parameter == null)
				return new DesignBinding(binding.DataSource, binding.DataMember);
			return new DesignBinding(binding.ParametersDataSource, binding.Parameter.Name);
		}
		void UpdateBinding(ParameterBinding binding, DesignBinding designBinding) {
			binding.Parameter = GetParameter(designBinding.DataSource as ParametersDataSource, designBinding.DataMember);
			if(binding.Parameter == null) {
				binding.DataSource = designBinding.DataSource;
				binding.DataMember = designBinding.DataMember;
			}
		}
		static Parameter GetParameter(ParametersDataSource dataSource, string dataMember) {
			return dataSource != null ? (Parameter)dataSource.Parameters.GetByName(dataMember) : null;
		}
	}
}
