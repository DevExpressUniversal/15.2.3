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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	static class MemberInfoHelper {
		internal static MethodInfo GetMethod(LambdaExpression expression) {
			return ((MethodCallExpression)expression.Body).Method;
		}
		internal static MemberInfo GetMember(LambdaExpression expression) {
			return ((MemberExpression)expression.Body).Member;
		}
		internal static bool HasChangedEvent(Type sourceType, string propertyName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) {
			return GetEventInfo(sourceType, propertyName + "Changed", flags) != null;
		}
		internal static EventInfo GetEventInfo(Type sourceType, string eventName, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
			return GetMemberInfo(sourceType, (type) => type.GetEvent(eventName, flags));
		}
		internal static MethodInfo GetMethodInfo(Type sourceType, string methodName, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
			return GetMemberInfo(sourceType, (type) => type.GetMethod(methodName, flags));
		}
		internal static MethodInfo GetMethodInfo(Type sourceType, string methodName, Type[] types, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
			return GetMemberInfo(sourceType, (type) => type.GetMethod(methodName, flags, null, types, null));
		}
		static TMemberInfo GetMemberInfo<TMemberInfo>(Type sourceType, Func<Type, TMemberInfo> getMember) where TMemberInfo : MemberInfo {
			Type[] types = GetTypes(sourceType, sourceType.GetInterfaces());
			for(int i = 0; i < types.Length; i++) {
				var memberInfo = getMember(types[i]);
				if(memberInfo != null)
					return memberInfo;
			}
			return null;
		}
		static Type[] GetTypes(Type sourceType, Type[] interfaces) {
			Type[] types = new Type[interfaces.Length + 1];
			Array.Copy(interfaces, types, interfaces.Length);
			types[interfaces.Length] = sourceType;
			return types;
		}
		readonly internal static Type[] SingleObjectParameterTypes = new Type[] { typeof(object) };
		internal static PropertyInfo GetCommandProperty(object source, IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			return source.GetType().GetProperty(GetCommandPropertyName(typesResolver, mInfo));
		}
		internal static string GetCommandDisplayName(IMVVMTypesResolver typesResolver, MemberInfo mInfo, Data.Utils.AnnotationAttributes attributes) {
			if(attributes != null) {
				string caption = Data.Utils.AnnotationAttributes.GetColumnCaption(attributes);
				if(!string.IsNullOrEmpty(caption))
					return caption;
			}
			return GetCommandName(typesResolver, mInfo);
		}
		internal static string GetCommandImageNameOrUri(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			Type imageAttributeType = typesResolver.GetAttributeType("DataAnnotations.ImageAttribute");
			Attribute imageAttribute = GetAttribute(typesResolver, mInfo, imageAttributeType);
			if(imageAttribute != null) {
				string imageUri = GetAttributeString(imageAttributeType, imageAttribute, "ImageUri");
				if(!string.IsNullOrEmpty(imageUri))
					return imageUri;
			}
			Type dxImageAttributeType = typesResolver.GetAttributeType("DataAnnotations.DXImageAttribute");
			Attribute dxImageAttribute = GetAttribute(typesResolver, mInfo, dxImageAttributeType);
			if(dxImageAttribute != null) {
				string imageName = GetAttributeString(dxImageAttributeType, dxImageAttribute, "ImageName");
				if(!string.IsNullOrEmpty(imageName))
					return imageName;
				string smallImageUri = GetAttributeString(dxImageAttributeType, dxImageAttribute, "SmallImageUri");
				if(!string.IsNullOrEmpty(smallImageUri))
					return smallImageUri;
				string largeImageUri = GetAttributeString(dxImageAttributeType, dxImageAttribute, "LargeImageUri");
				if(!string.IsNullOrEmpty(largeImageUri))
					return largeImageUri;
			}
			return GetCommandName(typesResolver, mInfo);
		}
		internal static string GetCommandName(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			Type commandAttributeType = typesResolver.GetCommandAttributeType();
			Attribute commandAttribute = GetAttribute(typesResolver, mInfo, commandAttributeType);
			string commandPropertyName = GetCommandPropertyName(mInfo, commandAttributeType, commandAttribute);
			return GetCommandName(commandPropertyName);
		}
		internal static string GetCommandParameter(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			Type commandParameterAttributeType = typesResolver.GetCommandParameterAttributeType();
			Attribute commandParameterAttribute = GetAttribute(typesResolver, mInfo, commandParameterAttributeType);
			return (commandParameterAttribute != null) ? InterfacesProxy.GetAttributeCommandParameter(commandParameterAttributeType, commandParameterAttribute) : null;
		}
		internal static MethodInfo[] GetCommandMethods(IMVVMTypesResolver typesResolver, Type type, int? parametersCount = null) {
			Type commandAttributeType = typesResolver.GetCommandAttributeType();
			var commandMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => IsCommandMethod(typesResolver, m, commandAttributeType, parametersCount)).ToArray();
			return commandMethods;
		}
		static bool IsCommandMethod(IMVVMTypesResolver typesResolver, MethodInfo mInfo, Type commandAttributeType, int? parametersCount = null) {
			if(mInfo.IsSpecialName || mInfo.DeclaringType == typeof(object))
				return false;
			ParameterInfo[] parameters = mInfo.GetParameters();
			if(parameters.Length > 1)
				return false;
			if(parameters.Length == 1) {
				if(parameters[0].IsOut || parameters[0].ParameterType.IsByRef)
					return false;
			}
			Attribute attribute = GetAttribute(typesResolver, mInfo, commandAttributeType);
			if(attribute == null) {
				if(!mInfo.IsPublic)
					return false;
				if((mInfo.ReturnType != typeof(void) && mInfo.ReturnType != typeof(System.Threading.Tasks.Task)))
					return false;
			}
			else {
				if(!InterfacesProxy.GetAttributeIsCommand(commandAttributeType, attribute))
					return false;
				if(!CanAccessFromDescendant(mInfo))
					return false;
			}
			return !parametersCount.HasValue || (parametersCount.Value == parameters.Length);
		}
		static string GetCommandPropertyName(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			Type commandAttributeType = typesResolver.GetCommandAttributeType();
			Attribute attribute = GetAttribute(typesResolver, mInfo, commandAttributeType);
			return GetCommandPropertyName(mInfo, commandAttributeType, attribute);
		}
		static string GetCommandPropertyName(MemberInfo mInfo, Type commandAttributeType, Attribute attribute) {
			if(attribute == null)
				return GetCommandPropertyName(mInfo.Name);
			return InterfacesProxy.GetAttributeName(commandAttributeType, attribute) ?? GetCommandPropertyName(mInfo.Name);
		}
		internal static PropertyInfo[] GetBindableProperties(IMVVMTypesResolver typesResolver, Type type) {
			Type bindablePropertyAttributeType = typesResolver.GetBindablePropertyAttributeType();
			return type.GetProperties()
				.Where(p => IsBindableProperty(typesResolver, p, bindablePropertyAttributeType)).ToArray();
		}
		internal static PropertyInfo[] GetBindableProperties(IMVVMTypesResolver typesResolver, Type type, Func<PropertyInfo, bool> canOverrideProperty) {
			Type bindablePropertyAttributeType = typesResolver.GetBindablePropertyAttributeType();
			Func<PropertyInfo, bool> predicate = (p) => IsBindableProperty(typesResolver, p, bindablePropertyAttributeType);
			if(canOverrideProperty != null)
				predicate = (p) => IsBindableProperty(typesResolver, p, bindablePropertyAttributeType) || canOverrideProperty(p);
			return type.GetProperties()
				.Where(predicate).ToArray();
		}
		static bool IsBindableProperty(IMVVMTypesResolver typesResolver, PropertyInfo pInfo, Type bindableAttributeType) {
			Attribute attribute = GetAttribute(typesResolver, pInfo, bindableAttributeType);
			if(attribute != null && !InterfacesProxy.GetAttributeIsBindable(bindableAttributeType, attribute))
				return false;
			var getMethod = pInfo.GetGetMethod();
			if(getMethod == null || !getMethod.IsVirtual || getMethod.IsFinal)
				return false;
			var setMethod = pInfo.GetSetMethod(true);
			if(setMethod == null || setMethod.IsAssembly)
				return false;
			if(!(IsAutoImplemented(pInfo)))
				return attribute != null && InterfacesProxy.GetAttributeIsBindable(bindableAttributeType, attribute);
			return true;
		}
		internal static bool IsAutoImplemented(PropertyInfo pInfo) {
			var compilerGeneratedAttribute = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
			if(pInfo.GetGetMethod().GetCustomAttributes(compilerGeneratedAttribute, false).Any())
				return true;
			if(pInfo.GetSetMethod(true).GetParameters().Single().Name != "AutoPropertyValue")
				return false;
			FieldInfo field = pInfo.DeclaringType.GetField("_" + pInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic);
			return (field != null) && (field.FieldType == pInfo.PropertyType) &&
				field.GetCustomAttributes(compilerGeneratedAttribute, false).Any();
		}
		internal static bool CanAccessFromDescendant(MethodBase method) {
			return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
		}
		static string GetAttributeString(Type attributeType, Attribute attribute, string propertyName) {
			return InterfacesProxy.GetAttributeProperty(attributeType, attribute, propertyName) as string;
		}
		internal static Attribute[] GetAttributes(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			var attributes = mInfo.GetCustomAttributes(false).OfType<Attribute>() ?? new Attribute[0];
			return attributes.Concat(GetExternalAndFluentAPIAttributes(typesResolver, mInfo)).ToArray();
		}
		readonly static Attribute[] EmptyAttributes = new Attribute[0];
		static Attribute GetAttribute(IMVVMTypesResolver typesResolver, MemberInfo mInfo, Type attributeType) {
			var attributes = (attributeType != null) ?
				(mInfo.GetCustomAttributes(attributeType, false).OfType<Attribute>() ?? EmptyAttributes) : EmptyAttributes;
			return attributes.Concat(GetExternalAndFluentAPIAttributes(typesResolver, mInfo)).FirstOrDefault();
		}
		static IEnumerable<Attribute> GetExternalAndFluentAPIAttributes(IMVVMTypesResolver typesResolver, MemberInfo mInfo) {
			Type metadataHelperType = typesResolver.GetMetadataHelperType();
			if(metadataHelperType == null) return EmptyAttributes;
			return InterfacesProxy.GetExternalAndFluentAPIAttributes(metadataHelperType, mInfo.ReflectedType, mInfo.Name);
		}
		const string CommandNameSuffix = "Command";
		static string GetCommandPropertyName(string methodName) {
			return methodName.EndsWith(CommandNameSuffix) ? methodName : methodName + CommandNameSuffix;
		}
		static string GetCommandName(string commandPropertyName) {
			if(commandPropertyName.EndsWith(CommandNameSuffix))
				return commandPropertyName.Substring(0, commandPropertyName.Length - CommandNameSuffix.Length);
			return commandPropertyName;
		}
	}
}
