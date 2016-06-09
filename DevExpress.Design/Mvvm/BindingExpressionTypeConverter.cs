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

namespace DevExpress.Utils.MVVM.Design {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design.Serialization;
	using System.Globalization;
	using System.Linq;
	public class CommandBindingExpressionConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				CommandBindingExpression be = value as CommandBindingExpression;
				return new InstanceDescriptor(GetCreateMethod(), be.GetSerializerParameters());
			}
			throw new NotSupportedException();
		}
		protected virtual System.Reflection.MethodInfo GetCreateMethod() {
			return typeof(BindingExpression).GetMethod("CreateCommandBinding");
		}
	}
	public class ParameterizedCommandBindingExpressionConverter : CommandBindingExpressionConverter {
		protected override System.Reflection.MethodInfo GetCreateMethod() {
			return typeof(BindingExpression).GetMethod("CreateParameterizedCommandBinding");
		}
	}
	public class CancelCommandBindingExpressionConverter : CommandBindingExpressionConverter {
		protected override System.Reflection.MethodInfo GetCreateMethod() {
			return typeof(BindingExpression).GetMethod("CreateCancelCommandBinding");
		}
	}
	public class PropertyBindingExpressionConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				PropertyBindingExpression be = value as PropertyBindingExpression;
				return new InstanceDescriptor(GetCreateMethod(), be.GetSerializerParameters());
			}
			throw new NotSupportedException();
		}
		protected virtual System.Reflection.MethodInfo GetCreateMethod() {
			return typeof(BindingExpression).GetMethod("CreatePropertyBinding");
		}
	}
	public abstract class BindingExpressionMemberConverterBase : TypeConverter {
		public const string None = "(none)";
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string propertyName = value as string;
			if(propertyName == None)
				return null;
			if(!GetMembers(context).Contains(propertyName))
				return null;
			return propertyName;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(value == null)
				return None;
			if(!GetMembers(context).Contains(value as string))
				return None;
			return value;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> list = new List<string>(GetMembers(context));
			list.Insert(0, null);
			return new StandardValuesCollection(list);
		}
		protected T GetInstance<T>(ITypeDescriptorContext context) where T : class {
			if(context == null)
				return null;
			var wrapper = context.Instance as DevExpress.Utils.Design.IDXObjectWrapper;
			return context.Instance as T ?? (wrapper != null ? wrapper.SourceObject as T : null);
		}
		protected abstract string[] GetMembers(ITypeDescriptorContext context);
		protected bool TypeFilter(Type memberType, Type expressionMemberType) {
			return (expressionMemberType == memberType) || expressionMemberType.IsAssignableFrom(memberType);
		}
		protected bool PropertyInfoFilter(System.Reflection.PropertyInfo pInfo, Type expressionMemberType) {
			return TypeFilter(pInfo.PropertyType, expressionMemberType);
		}
		protected bool EventInfoFilter(System.Reflection.EventInfo eInfo, Type expressionArgsType) {
			var mInfo = eInfo.EventHandlerType.GetMethod("Invoke");
			var parameters = mInfo.GetParameters();
			return (parameters.Length == 2) && TypeFilter(parameters[1].ParameterType, expressionArgsType);
		}
	}
	public class CommandBindingParameterConverter : BindingExpressionMemberConverterBase {
		protected override string[] GetMembers(ITypeDescriptorContext context) {
			var be = GetInstance<ParameterizedCommandBindingExpression>(context);
			if(be == null)
				return new string[0];
			var parameters = be.GetSerializerParameters();
			Type sourceType = parameters[0] as Type;
			return sourceType.GetProperties()
				.Where(p => PropertyInfoFilter(p, be.ParameterType))
				.OrderBy(p => p.Name)
				.Select(p => p.Name)
				.ToArray();
		}
	}
	public class BindingMemberParameterConverter : BindingExpressionMemberConverterBase {
		protected override string[] GetMembers(ITypeDescriptorContext context) {
			var be = GetInstance<PropertyBindingExpression>(context);
			if(be == null || be.Target == null)
				return new string[0];
			Type targetType = be.Target.GetType();
			return targetType.GetProperties()
				.Where(p => PropertyInfoFilter(p, be.BindingMemberType))
				.OrderBy(p => p.Name)
				.Select(p => p.Name)
				.ToArray();
		}
	}
	public class EventNameConverter : BindingExpressionMemberConverterBase {
		protected override string[] GetMembers(ITypeDescriptorContext context) {
			var re = GetInstance<BehaviorRegistrationExpression>(context);
			if(re == null || re.Target == null)
				return new string[0];
			Type targetType = re.Target.GetType();
			return targetType.GetEvents()
				.Where(e => EventInfoFilter(e, re.EventArgsType))
				.OrderBy(e => e.Name)
				.Select(e => e.Name)
				.ToArray();
		}
	}
	public class CommandNameConverter : BindingExpressionMemberConverterBase {
		protected override string[] GetMembers(ITypeDescriptorContext context) {
			var methodsProvider = GetInstance<ICommandMethodsProvider>(context);
			if(methodsProvider == null)
				return new string[0];
			return methodsProvider.GetCommandMethods()
				.OrderBy(m => m.Name)
				.Select(m => m.Name)
				.ToArray();
		}
	}
	public class CommandParameterNameConverter : BindingExpressionMemberConverterBase {
		protected override string[] GetMembers(ITypeDescriptorContext context) {
			var re = GetInstance<EventToCommandBehaviorParameterizedRegistrationExpression>(context);
			if(re == null)
				return new string[0];
			Type commandParameterType = re.GetCommandParameterType();
			if(commandParameterType == null)
				return new string[0];
			var parameters = re.GetSerializerParameters();
			Type sourceType = parameters[0] as Type;
			return sourceType.GetProperties()
			   .Where(p => PropertyInfoFilter(p, commandParameterType))
			   .OrderBy(p => p.Name)
			   .Select(p => p.Name)
			   .ToArray();
		}
	}
}
