#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.DC {
	public class ReflectionTypeInfoSource : BaseTypeInfoSource, ITypeInfoSource {
		private Boolean CalcHasPublicConstructor(TypeInfo info) {
			foreach(ConstructorInfo constructorInfo in info.Type.GetConstructors()) {
				if(constructorInfo.IsPublic) {
					return true;
				}
			}
			return false;
		}
		protected void CopyAttributes(IBaseInfo sourceInfo, BaseInfo targetInfo) {
			if(sourceInfo is BaseInfo) {
				List<Attribute> attributes = new List<Attribute>(((BaseInfo)sourceInfo).Attributes);
				targetInfo.SetAttributes(attributes.ToArray(), attributes.ToArray());
			}
		}
		public virtual Boolean TypeIsKnown(Type type) {
			return (type != null);
		}
		public virtual void InitTypeInfo(TypeInfo info) {
			Type type = info.Type;
			info.IsAbstract = type.IsAbstract;
			info.IsInterface = type.IsInterface;
			info.UnderlyingType = TypeHelper.GetUnderlyingType(info.Type);
			info.IsNullable = TypeHelper.IsNullable(info.Type);
			info.HasPublicConstructor = CalcHasPublicConstructor(info);
			info.IsListType = IsListType(info.Type);
		}
		public void EnumMembers(TypeInfo typeInfo, EnumMembersHandler handler) {
			Dictionary<String, Object> processedMemberNames = new Dictionary<String, Object>();
			Type type = typeInfo.Type;
			foreach(PropertyInfo propertyInfo in GetOwnProperties(type)) {
				processedMemberNames[propertyInfo.Name] = null;
				handler(propertyInfo, propertyInfo.Name);
			}
			foreach(FieldInfo fieldInfo in GetOwnFields(type)) {
				processedMemberNames[fieldInfo.Name] = null;
				handler(fieldInfo, fieldInfo.Name);
			}
			ITypeInfo baseTypeInfo = typeInfo.Base;
			foreach(PropertyDescriptor propertyDescriptor in GetPropertyDescriptors(type)) {
				String memberName = propertyDescriptor.Name;
				if(!processedMemberNames.ContainsKey(memberName)) {
					processedMemberNames.Add(memberName, null);
					if(baseTypeInfo != null && baseTypeInfo.FindMember(memberName) == null) {
						handler(propertyDescriptor, memberName);
					}
				}
			}
		}
		public virtual void InitMemberInfo(Object member, XafMemberInfo memberInfo) {
			memberInfo.Source = this;
			PropertyInfo propertyInfo = member as PropertyInfo;
			if(propertyInfo != null) {
				InitMemberFromPropertyInfo(memberInfo, propertyInfo);
			}
			else {
				FieldInfo fieldInfo = member as FieldInfo;
				if(fieldInfo != null) {
					InitMemberFromFieldInfo(memberInfo, fieldInfo);
				}
				else {
					PropertyDescriptor propertyDescriptor = member as PropertyDescriptor;
					if(propertyDescriptor != null) {
						InitMemberFromPropertyDescriptor(memberInfo, propertyDescriptor);
					}
				}
			}
		}
		public void RefreshMemberInfo(TypeInfo typeInfo, XafMemberInfo memberInfo) {
			if(memberInfo.Source == this) {
				Type type = typeInfo.Type;
				MemberInfo[] reflectionMembers = type.GetMember(memberInfo.Name, membersBindingFlags);
				if(reflectionMembers.Length > 0) {
					InitMemberInfo(reflectionMembers[0], memberInfo);
				}
				else {
					PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(type);
					PropertyDescriptor propertyDescriptor = collection.Find(memberInfo.Name, false);
					if(propertyDescriptor != null) {
						InitMemberInfo(propertyDescriptor, memberInfo);
					}
				}
			}
		}
		public virtual Boolean RegisterNewMember(ITypeInfo owner, XafMemberInfo memberInfo) {
			return false;
		}
		public virtual Boolean AddAttribute(IBaseInfo info, Attribute attribute) {
			return true;
		}
		public Boolean RemoveAttribute(IBaseInfo info, Type attributeType) {
			return true;
		}
	}
}
