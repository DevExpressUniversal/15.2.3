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
using System.Reflection;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Design {
	public class EFDesignTimeTypeInfoSource : ReflectionTypeInfoSource, IEntityStore, ITypeInfoSource {
		private List<Type> registeredEntities;
		private Dictionary<String, Type> entityTypes;
		private ITypesInfo typesInfo;
		public EFDesignTimeTypeInfoSource(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
			registeredEntities = new List<Type>();
			entityTypes = new Dictionary<String, Type>();
		}
		public void LoadTypesFromContext(Type dbContextType) {
			foreach(PropertyInfo property in dbContextType.GetProperties()) {
				Type propertyType = property.PropertyType;
				if(propertyType != null && propertyType.IsGenericType && EFDesignTimeTypeInfoHelper.IsEntitySetType(propertyType.GetGenericTypeDefinition())) {
					AddEntityType(propertyType.GetGenericArguments()[0]);
				}
			}
		}
		public bool CanRegister(Type type) {
			return TypeIsKnown(type);
		}
		public override Boolean TypeIsKnown(Type type) {
			return type.FullName == "System.Data.Entity.Core.Objects.DataClasses.EntityObject" || (FindEntityType(type) != null);
		}
		public void RegisterEntity(Type type) {
			if(TypeIsKnown(type) && !registeredEntities.Contains(type)) {
				registeredEntities.Add(type);
				TypeInfo typeInfo = (TypeInfo)typesInfo.FindTypeInfo(type);
				typeInfo.Source = this;
				typeInfo.Refresh();
				typeInfo.RefreshMembers();
				SpecialInitForBaseTypeInfo(typeInfo);
			}
		}
		public IEnumerable<Type> RegisteredEntities {
			get { return registeredEntities; }
		}
		public void Reset() {
			registeredEntities.Clear();
			entityTypes.Clear();
		}
		public override void InitTypeInfo(TypeInfo typeInfo) {
			base.InitTypeInfo(typeInfo);
			if(TypeIsKnown(typeInfo.Type)) {
				RealInitTypeInfo(typeInfo);
			}
		}
		public override void InitMemberInfo(Object member, XafMemberInfo xafMemberInfo) {
			base.InitMemberInfo(member, xafMemberInfo);
			if(!MemberIsKey(xafMemberInfo)) {
				xafMemberInfo.IsVisible = CalcMemberVisibility(xafMemberInfo);
			}
		}
		private void AddEntityType(Type entityType) {
			if(entityType != null && !entityTypes.ContainsKey(entityType.FullName)) {
				entityTypes[entityType.FullName] = entityType;
				TypeInfo typeInfo = (TypeInfo)typesInfo.FindTypeInfo(entityType);
				CollectMemberTypes(typeInfo);
			}
		}
		private Type FindEntityType(Type type) {
			Type entityType;
			entityTypes.TryGetValue(type.FullName, out entityType);
			return entityType;
		}
		private void RealInitTypeInfo(TypeInfo typeInfo) {
			if(IsStandartEntityType(typeInfo.Type)) {
				typeInfo.IsPersistent = false;
				typeInfo.IsVisible = false;
			}
			else {
				typeInfo.IsPersistent = true;
				typeInfo.IsDomainComponent = true;
			}
			typeInfo.IsVisible = CalcTypeVisibility(typeInfo);
		}
		private void SpecialInitForBaseTypeInfo(TypeInfo typeInfo) {
			foreach(IMemberInfo memberInfo in typeInfo.Members) {
				XafMemberInfo xafMemberInfo = memberInfo as XafMemberInfo;
				if(!MemberIsKey(xafMemberInfo) && IsMapped(xafMemberInfo)) {
					((XafMemberInfo)memberInfo).IsPersistent = true;
				}
			}
		}
		private bool IsMapped(XafMemberInfo memberInfo) {
			Attribute notMappedAttribute = FindMemberAttribute("NotMapped", memberInfo);
			Attribute editorBrowsableAttribute = FindMemberAttribute("EditorBrowsable", memberInfo);
			return notMappedAttribute == null && editorBrowsableAttribute == null;
		}
		private Attribute FindMemberAttribute(string attributeTypeName, XafMemberInfo memberInfo) {
			Attribute result = null;
			foreach(Attribute attribute in memberInfo.Attributes) {
				if(attribute.GetType().Name.Contains(attributeTypeName)) {
					result = attribute;
					break;
				}
			}
			return result;
		}
		private bool IsStandartEntityType(Type type) {
			return type != null && type.FullName != null && type.FullName.StartsWith("System.Data.Entity.Core");
		}
		private void CollectMemberTypes(ITypeInfo typeInfo) {
			foreach(IMemberInfo memberInfo in typeInfo.Members) {
				if(memberInfo.MemberType.IsClass && !memberInfo.IsList && IsPersistentMember(memberInfo.MemberType)) {
					AddEntityType(memberInfo.MemberType);
				}
				if(memberInfo.IsList && IsPersistentMember(memberInfo.ListElementType)) {
					AddEntityType(memberInfo.ListElementType);
				}
			}
		}
		private bool IsPersistentMember(Type type) {
			return type != null && !IsStandartEntityType(type) && (type.Module.ScopeName != "CommonLanguageRuntimeLibrary");
		}
		private bool MemberIsKey(IMemberInfo memberInfo) {
			return memberInfo.Name.ToLower() == "id";
		}
	}
}
