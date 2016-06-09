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
using System.Linq;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Native.ObjectBinding;
namespace DevExpress.DataAccess.ObjectBinding {
	public sealed class ObjectMember {
		public class EqualityComparer : IEqualityComparer<ObjectMember> {
			public static EqualityComparer Instance = new EqualityComparer();
			public static bool Equals(ObjectMember x, ObjectMember y) {
				if(ReferenceEquals(x, y))
					return true;
				if(x == null || y == null)
					return false;
				return Equals(x.Pd, y.Pd) && Equals(x.MemberInfo, y.MemberInfo);
			}
			public static bool Equals(PropertyDescriptor x, PropertyDescriptor y) {
				if(ReferenceEquals(x, y))
					return true;
				if(x == null || y == null)
					return false;
				return string.Equals(x.Name, y.Name, StringComparison.Ordinal);
			}
			public static bool Equals(MemberInfo x, MemberInfo y) {
				if(ReferenceEquals(x, y))
					return true;
				if(x == null || y == null)
					return false;
				PropertyInfo xProperty = x as PropertyInfo;
				if(xProperty != null) {
					PropertyInfo yProperty = y as PropertyInfo; 
					if(yProperty == null)
						return false;
					return string.Equals(xProperty.Name, yProperty.Name, StringComparison.Ordinal);
				}
				MethodInfo yMethod = y as MethodInfo;
				if(yMethod == null)
					return false;
				MethodInfo xMethod = (MethodInfo)x;
				if(!string.Equals(xMethod.Name, yMethod.Name))
					return false;
				ParameterInfo[] xParameters = xMethod.GetParameters();
				ParameterInfo[] yParameters = yMethod.GetParameters();
				if(yParameters.Length != xParameters.Length)
					return false;
				IEnumerator<ParameterInfo> xEnumerator = ((IEnumerable<ParameterInfo>)xParameters).GetEnumerator();
				IEnumerator<ParameterInfo> yEnumerator = ((IEnumerable<ParameterInfo>)yParameters).GetEnumerator();
				while(xEnumerator.MoveNext() && yEnumerator.MoveNext()) {
					ParameterInfo xParam = xEnumerator.Current;
					ParameterInfo yParam = yEnumerator.Current;
					if(xParam.ParameterType != yParam.ParameterType || !string.Equals(xParam.Name, yParam.Name, StringComparison.Ordinal))
						return false;
				}
				return true;
			}
			#region Implementation of IEqualityComparer<in ObjectMember>
			bool IEqualityComparer<ObjectMember>.Equals(ObjectMember x, ObjectMember y) { return Equals(x, y); }
			int IEqualityComparer<ObjectMember>.GetHashCode(ObjectMember obj) {
				return (obj.Pd == null ? 0 : obj.Pd.GetHashCode()) ^
					   (obj.MemberInfo == null ? 0 : obj.MemberInfo.GetHashCode());
			}
			#endregion
		}
		readonly MemberInfo memberInfo;
		readonly PropertyDescriptor pd;
		readonly bool hasParams;
		readonly bool isStatic;
#if !DXPORTABLE
		readonly MemberTypes memberType;
#endif
		readonly bool highlighted;
		readonly Type returnType;
		readonly string name;
		readonly string parameters;
		readonly Attribute[] attributes;
		public ObjectMember(MemberInfo memberInfo) {
			this.memberInfo = memberInfo;
			MethodInfo methodInfo = memberInfo as MethodInfo;
			if(methodInfo == null) {
				hasParams = false;
#if !DXPORTABLE
				memberType = MemberTypes.Property;
#endif
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				attributes = propertyInfo.GetCustomAttributes(false).Cast<Attribute>().ToArray();
				MethodInfo getterInfo = propertyInfo.GetGetMethod();
				isStatic = getterInfo.IsStatic;
				returnType = getterInfo.ReturnType;
				parameters = null;
			}
			else {
				ParameterInfo[] parameterInfos = methodInfo.GetParameters();
				hasParams = parameterInfos.Length > 0;
#if !DXPORTABLE
				memberType = MemberTypes.Method;
#endif
				isStatic = methodInfo.IsStatic;
				returnType = methodInfo.ReturnType;
				parameters = string.Format("({0})",
					string.Join(", ",
						parameterInfos.Select(
							info => string.Format("{0} {1}", TypeNamesHelper.ShortName(info.ParameterType), info.Name))));
				attributes = methodInfo.GetCustomAttributes(false).Cast<Attribute>().ToArray();
			}
			highlighted = memberInfo.GetCustomAttributes(typeof(HighlightedMemberAttribute), false).Any();
			name = memberInfo.Name;
		}
		public ObjectMember(PropertyDescriptor pd) {
			this.pd = pd;
			hasParams = false;
			isStatic = false;
#if !DXPORTABLE
			memberType = MemberTypes.Property;
#endif
			highlighted = pd.Attributes.Cast<Attribute>().Any(attr => attr is HighlightedMemberAttribute);
			returnType = pd.PropertyType;
			name = pd.Name;
			parameters = null;
			attributes = pd.Attributes.Cast<Attribute>().ToArray();
		}
		public string Name { get { return name; } }
		public MemberInfo MemberInfo { get { return memberInfo; } }
		public PropertyDescriptor Pd { get { return pd; } }
		public IEnumerable<Parameter> GetParameters(IEnumerable<Parameter> defaultValues) {
			if(!hasParams)
				return new Parameter[0];
			MethodInfo method = (MethodInfo)MemberInfo;
			return method.GetParameters().Select(p => GetParameter(p, defaultValues));
		}
		internal static Parameter GetParameter(ParameterInfo p, IEnumerable<Parameter> defaultValues) {
			object value = p.DefaultValue;
			Type type = p.ParameterType;
			if(defaultValues != null)
				foreach(Parameter parameter in defaultValues)
					if(string.Equals(parameter.Name, p.Name, StringComparison.Ordinal) && (parameter.Type == p.ParameterType || parameter.Type == typeof(Expression))) {
						value = parameter.Value;
						type = parameter.Type;
						break;
					}
			return new Parameter(p.Name, type, value);
		}
		public bool HasParams { get { return hasParams; } }
		public bool IsStatic { get { return isStatic; } }
#if !DXPORTABLE
		public MemberTypes MemberType { get { return memberType; } }
#endif
		public bool Highlighted { get { return highlighted; } }
		public Type ReturnType { get { return returnType; } }
		public string Parameters { get { return parameters; } }
		public object Invoke(object instance, object[] parameters) {
			if(Pd != null)
				return Pd.GetValue(instance);
			MethodInfo methodInfo = MemberInfo as MethodInfo ?? ((PropertyInfo)MemberInfo).GetGetMethod();
			return methodInfo.Invoke(instance, parameters);
		}
		public bool IsProperty { get { return Pd != null || MemberInfo is PropertyInfo; } }
		public bool IsMethod { get { return Pd == null && MemberInfo is MethodInfo; } }
		public IEnumerable<Attribute> Attributes { get { return attributes; } }
		#region Overrides of Object
		public override string ToString() {
			return string.Format(IsProperty ? "{0} : {1}" : "{0}{2} : {1}", Name,
				TypeNamesHelper.ShortName(ReturnType), Parameters);
		}
		#endregion
	}
}
