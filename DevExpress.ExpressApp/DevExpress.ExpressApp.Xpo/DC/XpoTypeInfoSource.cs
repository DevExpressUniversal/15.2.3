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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.DC.ClassGeneration;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.ExpressApp.DC.Xpo {
	public class XpoTypeInfoSource : BaseTypeInfoSource, ITypeInfoSource, IDCEntityStore, IEntityStore {
		private readonly static ReadOnlyCollection<Type> xpoBaseClasses;
		private readonly TypesInfo typesInfo;
		private XPDictionary entityDictionary;
		private XPDictionary infoDictionary;
		private readonly HashSet<Type> entityTypes;
		private readonly HashSet<Type> registeredEntityTypes = new HashSet<Type>();
		private readonly Dictionary<Type, Boolean> typeIsKnownCache = new Dictionary<Type, Boolean>();
		private XafReflectionDictionary dcEntityDictionary;
		private CustomLogics customLogics;
		private readonly EntitiesToGenerateInfo entitiesToGenerateInfo = new EntitiesToGenerateInfo();
		private readonly ExistingImplementorsInfo existingImplementorsInfo;
		private Assembly generatedAssembly;
		private readonly Dictionary<Type, Type> entityByInterface = new Dictionary<Type, Type>();
		private readonly Dictionary<Type, Type> interfaceByEntity = new Dictionary<Type, Type>();
		private readonly Dictionary<Type, Type> dataClassByInterface;
		private readonly InterfaceAssociationInfo interfaceAssociationInfo;
		private void CheckDictionary() {
			if(entityTypes != null) {
				throw new InvalidOperationException("The method can be called only for an instance that was created with the XpoTypeInfoSource(TypesInfo typesInfo) constructor.");
			}
		}
		private bool IsAlreadyBuild {
			get { return generatedAssembly != null; }
		}
		private void ProcessGeneratedAssembly(Assembly generatedAssembly) {
			foreach(Type interfaceType in existingImplementorsInfo.GetInterfaces()) {
				dcEntityDictionary.ExtendPersistentInterfaces(interfaceType, existingImplementorsInfo.GetImplementor(interfaceType));
			}
			foreach(Type type in existingImplementorsInfo.GetImplementors()) {
				ProcessGeneratedType(type);
			}
			foreach(Type entityInterface in entitiesToGenerateInfo.GetEntityInterfaces()) {
				dcEntityDictionary.ExtendPersistentInterfaces(entityInterface);
			}
			Type[] types = generatedAssembly.GetExportedTypes();
			foreach(Type type in types) {
				if(entitiesToGenerateInfo.ContainsEntityName(type.Name)) {
					Type interfaceType = entitiesToGenerateInfo.GetEntityInterface(type.Name);
					entityByInterface.Add(interfaceType, type);
					interfaceByEntity.Add(type, interfaceType);
				}
			}
			foreach(Type type in types) {
				if(type.IsInterface)
					continue;
				ProcessGeneratedType(type);
			}
			dcEntityDictionary.CollectClassInfos(generatedAssembly);
			InterfaceAttributesSynchronizer attributesSynchronizer = new InterfaceAttributesSynchronizer();
			foreach(Type type in entitiesToGenerateInfo.GetSharedParts()) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				XPClassInfo classInfo = dcEntityDictionary.GetClassInfo(type);
				if(typeInfo.Type == classInfo.ClassType) {
					attributesSynchronizer.UpdateXPInfo(typeInfo, classInfo);
				}
			}
		}
		private void ProcessGeneratedType(Type type) {
			Dictionary<Type, Type> foundPIDDictionary = new Dictionary<Type, Type>();
			Dictionary<Type, object> foundPIDictionary = new Dictionary<Type, object>();
			foreach(Type implementedInterface in type.GetInterfaces()) {
				if(implementedInterface.IsGenericType) {
					if(implementedInterface.GetGenericTypeDefinition() == typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>)) {
						foundPIDDictionary.Add(implementedInterface.GetGenericArguments()[0], implementedInterface);
					}
					else if((implementedInterface.GetGenericTypeDefinition() == typeof(DevExpress.Xpo.Helpers.IPersistentInterface<>))) {
						foundPIDictionary.Add(implementedInterface.GetGenericArguments()[0], null);
					}
				}
			}
			foreach(KeyValuePair<Type, Type> pidPair in foundPIDDictionary) {
				bool foundPIDInBase = false;
				foreach(Type implementedInterface in type.BaseType.GetInterfaces()) {
					if(implementedInterface == pidPair.Value) {
						foundPIDInBase = true;
						break;
					}
				}
				if(!foundPIDInBase) {
					dataClassByInterface.Add(pidPair.Key, type);
					dcEntityDictionary.ExtendPersistentInterfaces(pidPair.Value, type);
					if(foundPIDictionary.ContainsKey(pidPair.Key)) {
						dcEntityDictionary.ExtendPersistentInterfaces(pidPair.Key, type);
					}
				}
			}
		}
		private Type GetEntityTypeByInterface(Type interfaceType) {
			Type result;
			if(!entityByInterface.TryGetValue(interfaceType, out result)) {
				result = null;
			}
			return result;
		}
		private Type GetInterfaceByEntityType(Type entityType) {
			Type result;
			if(!interfaceByEntity.TryGetValue(entityType, out result)) {
				result = null;
			}
			return result;
		}
		private Type GetDataTypeByInterface(Type interfaceType) {
			Type result;
			if(!dataClassByInterface.TryGetValue(interfaceType, out result)) {
				result = null;
			}
			return result;
		}
		private Boolean IsEntityInterfaceDataType(Type type) {
			List<Type> interfacesToCheck = new List<Type>();
			if(type.IsInterface) {
				interfacesToCheck.Add(type);
			}
			else {
				interfacesToCheck.AddRange(type.GetInterfaces());
			}
			Type persistentInterfaceDataDefinition = typeof(IPersistentInterfaceData<>);
			foreach(Type interfaceToCheck in interfacesToCheck) {
				if(interfaceToCheck.IsGenericType && !interfaceToCheck.IsGenericTypeDefinition) {
					if(interfaceToCheck.GetGenericTypeDefinition() == persistentInterfaceDataDefinition) {
						Type interfaceType = interfaceToCheck.GetGenericArguments()[0];
						if(interfaceType.IsInterface && registeredEntityTypes.Contains(interfaceType)) {
							return dcEntityDictionary.CanGetClassInfoByType(type);
						}
					}
				}
			}
			return false;
		}
		private Boolean IsTypeFromEntityDictionary(Type type) {
			return registeredEntityTypes.Contains(type) || IsEntityInterfaceDataType(type);
		}
		private XPMemberInfo FindKey(XPClassInfo classInfo) {
			XPMemberInfo key = null;
			foreach(XPMemberInfo mi in classInfo.Members) {
				if(mi.IsKey) {
					key = mi;
					break;
				}
			}
			if((key != null) && !key.IsPublic) {
				foreach(XPMemberInfo mi in classInfo.Members) {
					if(mi.IsPublic && mi.IsAliased) {
						PersistentAliasAttribute alias = (PersistentAliasAttribute)mi.FindAttributeInfo(typeof(PersistentAliasAttribute));
						if((alias != null) && (alias.AliasExpression == key.Name)) {
							key = mi;
							break;
						}
					}
				}
			}
			return key;
		}
		private Boolean CalcTypeVisibility(XPClassInfo classInfo) {
			if(classInfo.ClassType == null) {
				return false;
			}
			if(XpoTypeInfoSource.XpoBaseClasses.Contains(classInfo.ClassType)) {
				return true;
			}
			if(!classInfo.IsVisibleInDesignTime) {
				return false;
			}
			if(!entityDictionary.CanGetClassInfoByType(classInfo.ClassType)) {
				return false;
			}
			BrowsableAttribute browsableAttribute = (BrowsableAttribute)classInfo.FindAttributeInfo(typeof(BrowsableAttribute));
			if((browsableAttribute != null) && !browsableAttribute.Browsable) {
				return false;
			}
			return true;
		}
		private Boolean CalcMemberVisibility(XafMemberInfo member, XPMemberInfo xpMember) {
			if((member.Owner != null) && !member.Owner.IsVisible) {
				return false;
			}
			if((xpMember != null) && (!xpMember.IsPublic
				|| !xpMember.IsVisibleInDesignTime
				|| xpMember.MemberType.ContainsGenericParameters
				|| (xpMember is ServiceField))
				) {
				return false;
			}
			if(member.FindAttribute<ObsoleteAttribute>() != null) {
				return false;
			}
			BrowsableAttribute browsableAttribute = member.FindAttribute<BrowsableAttribute>();
			if((browsableAttribute != null) && !browsableAttribute.Browsable) {
				return false;
			}
			if((xpMember != null) && (xpMember.Owner.ClassType.Assembly == typeof(XPBaseObject).Assembly)) {
				if((xpMember.Owner.ClassType == typeof(XPObject)) && (xpMember.Name == "Oid")) {
					return true;
				}
				return false;
			}
			return true;
		}
		private String CalcBindingName(ITypeInfo owner, IMemberInfo memberInfo) {
			String result = memberInfo.Name;
			if(owner != null && IsXpoType(owner)
				&& (
					IsXpoType(memberInfo.MemberTypeInfo)
					|| (memberInfo.MemberTypeInfo.FindAttribute<NonPersistentAttribute>() != null) 
				)
			) {
				result = result.TrimEnd('!') + "!";
			}
			return result;
		}
		private bool CalcHasValueConverter(XafMemberInfo memberInfo) {
			return memberInfo.FindAttribute<ValueConverterAttribute>() != null;
		}
		private Boolean IsXpoType(ITypeInfo typeInfo) {
			return typeof(PersistentBase).IsAssignableFrom(typeInfo.Type) || IsDCInterface(typeInfo);
		}
		private Boolean IsDCInterface(ITypeInfo typeInfo) {
			Boolean result = false;
			if(typeInfo.IsInterface) {
				if(typeInfo.IsPersistent) {
					result = true;
				}
				else if(typeInfo.IsDomainComponent) {
					foreach(ITypeInfo implementor in typeInfo.Implementors) {
						if(implementor.IsInterface && implementor.IsPersistent) {
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}
		private void InitXPClassTypeInfo(TypeInfo info) {
			Type type = info.Type;
			XPClassInfo classInfo = GetRealClassInfo(type);
			if(classInfo != null) {
				info.IsDomainComponent = !xpoBaseClasses.Contains(type) && (registeredEntityTypes.Contains(type) || CanRegister(type));
				info.IsAbstract = classInfo.IsAbstract;
				info.IsVisible = CalcTypeVisibility(classInfo);
				info.IsPersistent = classInfo.IsPersistent && IsTypeFromEntityDictionary(type);
			}
		}
		private void EnumXPClassMembers(TypeInfo info, EnumMembersHandler handler) {
			XPClassInfo classInfo = GetRealClassInfo(info.Type);
			if(classInfo != null) {
				HashSet<String> processedMemberNames = new HashSet<String>();
				List<XPMemberInfo> ownMembers = new List<XPMemberInfo>(classInfo.OwnMembers);
				foreach(XPMemberInfo xpMember in ownMembers) {
					XafMemberInfo memberInfo = info.FindOwnMember(xpMember.Name) as XafMemberInfo;
					if(memberInfo != null) {
						memberInfo.Source = this;
					}
					processedMemberNames.Add(xpMember.Name);
					handler(xpMember, xpMember.Name);
				}
				XPMemberInfo xpKey = FindKey(classInfo);
				if(xpKey != null) {
					info.KeyMember = info.FindMember(xpKey.Name);
				}
				foreach(PropertyInfo propertyInfo in GetOwnProperties(info.Type)) {
					String memberName = propertyInfo.Name;
					if(!processedMemberNames.Contains(memberName)) {
						processedMemberNames.Add(memberName);
						handler(propertyInfo, memberName);
					}
				}
				ITypeInfo baseTypeInfo = info.Base;
				if(baseTypeInfo != null) {
					foreach(PropertyDescriptor propertyDescriptor in GetPropertyDescriptors(info.Type)) {
						String memberName = propertyDescriptor.Name;
						if(!processedMemberNames.Contains(memberName)) {
							processedMemberNames.Add(memberName);
							if(baseTypeInfo.FindMember(memberName) == null) {
								handler(propertyDescriptor, memberName);
							}
						}
					}
				}
			}
		}
		private void InitMemberInfo(ITypeInfo owner, XafMemberInfo member, Object memberSource) {
			if(owner != null && owner.IsInterface) {
				InitDCInterfaceMemberInfo(memberSource, member);
			}
			else {
				InitXPClassMemberInfo(memberSource, member);
			}
			PropertyDescriptor propertyDescriptor = memberSource as PropertyDescriptor;
			if(propertyDescriptor != null) {
				InitMemberFromPropertyDescriptor(member, propertyDescriptor);
			}
			member.BindingName = CalcBindingName(owner, member);
			member.HasValueConverter = CalcHasValueConverter(member);
			if(!String.IsNullOrWhiteSpace(member.Expression)) {
				member.IsReadOnly = true;
			}
		}
		private void InitXPClassMemberInfo(Object memberSource, XafMemberInfo member) {
			XPMemberInfo xpMember = memberSource as XPMemberInfo;
			if(xpMember != null) {
				Attribute[] attributes = xpMember.Attributes;
				member.SetAttributes(attributes, attributes); 
				member.MemberType = xpMember.MemberType;
				member.IsReadOnly = member.IsReadOnly || xpMember.IsReadOnly; 
				member.IsPublic = xpMember.IsPublic;
				member.IsProperty = xpMember is ReflectionPropertyInfo;
				member.IsVisible = CalcMemberVisibility(member, xpMember);
				member.IsKey = xpMember.IsKey;
				member.IsAutoGenerate = xpMember.IsAutoGenerate;
				member.IsDelayed = xpMember.IsDelayed;
				member.IsAliased = xpMember.IsAliased;
				member.IsService = (member.Name == xpMember.Owner.OptimisticLockFieldName);
				if(xpMember.IsCollection) {
					member.IsList = true;
					member.ListElementType = xpMember.CollectionElementType.ClassType;
				}
				else {
					SetListInfo(member);
				}
				member.DisplayName = xpMember.DisplayName;
				int mappingFieldSize = xpMember.MappingFieldSize;
				if(xpMember.StorageType == typeof(Byte[]) && xpMember.FindAttributeInfo(typeof(SizeAttribute)) == null) {
					member.ValueMaxLength = 0;
				}
				else {
					member.ValueMaxLength = (mappingFieldSize > 0) ? mappingFieldSize : 0;
				}
				member.Size = mappingFieldSize;
				member.IsPersistent = xpMember.IsPersistent;
				member.IsAggregated = xpMember.IsAggregated;
				member.IsManyToMany = xpMember.IsManyToMany || xpMember.IsManyToManyAlias;
				member.IsAssociation = xpMember.IsAssociation;
				if(xpMember.IsAssociation && !xpMember.IsAliased) {
					XPMemberInfo ownerMemberInfo = xpMember.GetAssociatedMember();
					if(ownerMemberInfo != null) {
						member.AssociatedMemberName = ownerMemberInfo.Name;
						member.AssociatedMemberOwner = ownerMemberInfo.Owner.ClassType;
						member.IsReferenceToOwner = ownerMemberInfo.IsAggregated;
					}
				}
				if(member.IsReferenceToOwner) {
					member.Owner.ReferenceToOwner = member;
				}
				nativeMemberInfoDictionary[member] = xpMember;
			}
			PropertyInfo propertyInfo = memberSource as PropertyInfo;
			if(propertyInfo != null) {
				InitMemberFromPropertyInfo(member, propertyInfo);
			}
		}
		private void InitDCInterfaceTypeInfo(TypeInfo info) {
			Type type = info.Type;
			info.IsDomainComponent = !xpoBaseClasses.Contains(type) && (registeredEntityTypes.Contains(type) || CanRegister(type));
			if(registeredEntityTypes.Contains(type)) {
				if(entitiesToGenerateInfo.ContainsEntityInterface(type) || existingImplementorsInfo.ContainsInterface(type)) {
					info.IsPersistent = true;
				}
				else if(info.FindAttribute<NonPersistentDcAttribute>(false) == null) {
					foreach(Type entityInterface in entitiesToGenerateInfo.GetEntityInterfaces()) {
						if(Enumerator.Exists(entityInterface.GetInterfaces(), type)) {
							info.IsPersistent = true;
							break;
						}
					}
				}
			}
			if(IsAlreadyBuild && entitiesToGenerateInfo.ContainsEntityInterface(type)) {
				XPClassInfo generatedClassInfo = GetGeneratedEntityClassInfo(type);
				info.IsAbstract = generatedClassInfo.IsAbstract;
				info.IsVisible = CalcTypeVisibility(generatedClassInfo);
			}
		}
		private void EnumDCInterfaceMembers(TypeInfo info, EnumMembersHandler handler) {
			Type type = info.Type;
			foreach(PropertyInfo property in type.GetProperties()) {
				XafMemberInfo memberInfo = info.FindOwnMember(property.Name) as XafMemberInfo;
				if(memberInfo != null) {
					memberInfo.Source = this;
					handler(property, property.Name);
				}
			}
			if(IsAlreadyBuild && dcEntityDictionary.IsPersistentInterface(type)) {
				XPClassInfo implementationClassInfo = null;
				if(entitiesToGenerateInfo.ContainsEntityInterface(type)) {
					implementationClassInfo = GetGeneratedEntityClassInfo(type);
				}
				else {
					XPClassInfo dataClassInfo = dcEntityDictionary.QueryClassInfo(type);
					if(!dataClassInfo.ClassType.IsInterface) {
						implementationClassInfo = dataClassInfo;
					}
				}
				XPMemberInfo xpKey = null;
				if(implementationClassInfo == null) {
					Type dataType = typeof(IPersistentInterfaceData<>).MakeGenericType(new Type[] { type });
					XPClassInfo dataClassInfo = dcEntityDictionary.QueryClassInfo(dataType);
					if(dataClassInfo != null) {
						xpKey = FindKey(dataClassInfo);
					}
				}
				else {
					xpKey = FindKey(implementationClassInfo);
				}
				if(xpKey != null) {
					handler(xpKey, xpKey.Name);
					IMemberInfo keyMemberInfo = info.FindMember(xpKey.Name);
					if(keyMemberInfo != null) {
						info.KeyMember = keyMemberInfo;
						if(keyMemberInfo.FindAttribute<VisibleInListViewAttribute>() == null) {
							keyMemberInfo.AddAttribute(new VisibleInListViewAttribute(false));
						}
						if(keyMemberInfo.FindAttribute<VisibleInDetailViewAttribute>() == null) {
							keyMemberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
						}
						if(keyMemberInfo.FindAttribute<VisibleInLookupListViewAttribute>() == null) {
							keyMemberInfo.AddAttribute(new VisibleInLookupListViewAttribute(false));
						}
					}
				}
			}
		}
		private Boolean CalcIsAggregatedForDCInterfaceMemberInfo(IMemberInfo member) {
			return (member.FindAttribute<DevExpress.ExpressApp.DC.AggregatedAttribute>(false) != null)
				|| (member.FindAttribute<DevExpress.Xpo.AggregatedAttribute>(false) != null);
		}
		private Boolean IsRequiredAttributre(Attribute attribute) {
			if(attribute.GetType().Assembly.FullName.Contains("DevExpress.ExpressApp")) {
				return false;
			}
			return true;
		}
		private Type GetStorageType(XafMemberInfo memberInfo) {
			ValueConverterAttribute valueConverterAttribute = memberInfo.FindAttribute<ValueConverterAttribute>();
			if(valueConverterAttribute != null) {
				return valueConverterAttribute.Converter.StorageType;
			}
			else {
				return memberInfo.MemberType;
			}
		}
		private void InitDCInterfaceMemberInfo(Object memberSource, XafMemberInfo member) {
			if(memberSource is XPMemberInfo) {
				InitXPClassMemberInfo((XPMemberInfo)memberSource, member);
			}
			else {
				PropertyInfo propertyInfo = memberSource as PropertyInfo;
				if(propertyInfo != null) {
					member.DisplayName = CaptionHelper.ConvertCompoundName(propertyInfo.Name);
					int mappingFieldSize = 0;
					SizeAttribute sizeAttribute = member.FindAttribute<SizeAttribute>(false);
					if(sizeAttribute != null) {
						mappingFieldSize = sizeAttribute.Size;
					}
					else {
						FieldSizeAttribute fieldSizeAttribute = member.FindAttribute<FieldSizeAttribute>(false);
						if(fieldSizeAttribute != null) {
							mappingFieldSize = fieldSizeAttribute.Size;
						}
						else if(GetStorageType(member) == typeof(string)) {
							mappingFieldSize = SizeAttribute.DefaultStringMappingFieldSize;
						}
					}
					member.Size = mappingFieldSize;
					member.ValueMaxLength = (mappingFieldSize > 0) ? mappingFieldSize : 0;
					member.IsAggregated = CalcIsAggregatedForDCInterfaceMemberInfo(member);
					if((member.FindAttribute<PersistentDcAttribute>(false) != null)
						|| (member.FindAttribute<PersistentAttribute>(false) != null)
					) {
						member.IsPersistent = true;
					}
					else if((member.FindAttribute<NonPersistentDcAttribute>(false) != null)
						|| (member.FindAttribute<NonPersistentAttribute>(false) != null)
					) {
						member.IsPersistent = false;
					}
					else {
						member.IsPersistent = !member.IsReadOnly;
					}
					if(member.IsPersistent) {
						member.IsAliased = true; 
					}
					else {
						member.IsAliased =
							(member.FindAttribute<CalculatedAttribute>(false) != null)
							|| (member.FindAttribute<PersistentAliasAttribute>(false) != null);
					}
					member.IsVisible = CalcMemberVisibility(member, null);
					Boolean isManyToMany;
					PropertyInfo associationProperty = interfaceAssociationInfo.FindAssociatedProperty(propertyInfo, out isManyToMany);
					if(associationProperty != null) {
						ITypeInfo associationPropertyOwnerTypeInfo = typesInfo.FindTypeInfo(associationProperty.DeclaringType);
						if(associationPropertyOwnerTypeInfo != null) {
							IMemberInfo associatedMemberInfo = associationPropertyOwnerTypeInfo.FindMember(associationProperty.Name);
							if(associatedMemberInfo != null) {
								member.IsAssociation = true;
								member.IsManyToMany = isManyToMany;
								member.AssociatedMemberName = associatedMemberInfo.Name;
								member.AssociatedMemberOwner = associationPropertyOwnerTypeInfo.Type;
								member.IsReferenceToOwner = CalcIsAggregatedForDCInterfaceMemberInfo(associatedMemberInfo);
							}
						}
					}
					if(member.IsReferenceToOwner) {
						member.Owner.ReferenceToOwner = member;
					}
					if(IsAlreadyBuild
						&& member.IsList
						&& !member.IsAssociation
						&& !member.IsAggregated
						&& member.FindAttribute<NonPersistentDcAttribute>(false) == null
						&& member.FindAttribute<NonPersistentAttribute>(false) == null
					) {
						ITypeInfo listElementTypeInfo = member.ListElementTypeInfo;
						if(listElementTypeInfo != null) {
							bool isPersistent = listElementTypeInfo.IsPersistent || registeredEntityTypes.Contains(listElementTypeInfo.Type);
							member.IsManyToMany = listElementTypeInfo.IsInterface && isPersistent;
						}
					}
					nativeMemberInfoDictionary[member] = propertyInfo;
				}
			}
		}
		private XPClassInfo GetRegisteredEntityClassInfo(Type type) {
			if(registeredEntityTypes.Contains(type)) {
				return entityDictionary.QueryClassInfo(type);
			}
			return null;
		}
		private XPClassInfo GetGeneratedEntityClassInfo(Type type) {
			Type entityType = GetEntityTypeByInterface(type);
			if(entityType != null) {
				return dcEntityDictionary.GetClassInfo(entityType);
			}
			return null;
		}
		private XPClassInfo GetRealClassInfo(Type type) {
			if(IsTypeFromEntityDictionary(type)) {
				return entityDictionary.QueryClassInfo(type);
			}
			return infoDictionary.QueryClassInfo(type);
		}
		protected internal IList<Type> EntityTypes {
			get {
				if(entityTypes != null) {
					return entityTypes.ToList().AsReadOnly();
				}
				else {
					return null;
				}
			}
		}
		static XpoTypeInfoSource() {
			xpoBaseClasses = new ReadOnlyCollection<Type>(new Type[] { typeof(XPBaseObject), typeof(XPCustomObject), typeof(XPObject) });
		}
		public XpoTypeInfoSource(TypesInfo typesInfo) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			this.typesInfo = typesInfo;
			interfaceAssociationInfo = new InterfaceAssociationInfo(typesInfo);
			existingImplementorsInfo = new ExistingImplementorsInfo();
			dataClassByInterface = new Dictionary<Type, Type>();
			Reset();
		}
		public XpoTypeInfoSource(TypesInfo typesInfo, params Type[] types) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			this.typesInfo = typesInfo;
			entityDictionary = new ReflectionDictionary();
			entityDictionary.CollectClassInfos(types);
			infoDictionary = new ReflectionDictionary();
			entityTypes = new HashSet<Type>();
			PopulateEntityTypes();
		}
		public XpoTypeInfoSource(TypesInfo typesInfo, XPDictionary dictionary) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(dictionary, "dictionary");
			this.typesInfo = typesInfo;
			entityDictionary = dictionary;
			infoDictionary = new ReflectionDictionary();
			entityTypes = new HashSet<Type>();
			PopulateEntityTypes();
		}
		private void PopulateEntityTypes() {
			foreach(XPClassInfo classInfo in entityDictionary.Classes) {
				Type entityType = classInfo.ClassType;
				if(entityType != null) {
					entityTypes.Add(entityType);
				}
			}
		}
		public XPClassInfo GetEntityClassInfo(Type type) {
			XPClassInfo result = GetGeneratedEntityClassInfo(type);
			if(result == null) {
				result = GetRegisteredEntityClassInfo(type);
			}
			return result;
		}
		public XPDictionary XPDictionary {
			get { return entityDictionary; }
		}
		public CustomLogics CustomLogics {
			get { return customLogics; }
		}
		public static ReadOnlyCollection<Type> XpoBaseClasses {
			get { return xpoBaseClasses; }
		}
		public Boolean TypeIsKnown(Type type) {
			Guard.ArgumentNotNull(type, "type");
			Boolean result;
			if(entityTypes != null) {
				return entityTypes.Contains(type);
			}
			else if(!typeIsKnownCache.TryGetValue(type, out result)) {
				result = entityDictionary.CanGetClassInfoByType(type) || (type.IsInterface && type.IsDefined(typeof(DomainComponentAttribute), false));
				typeIsKnownCache.Add(type, result);
			}
			return result;
		}
		public void InitTypeInfo(TypeInfo info) {
			Type type = info.Type;
			if(TypeIsKnown(type)) {
				if(type.IsInterface) {
					InitDCInterfaceTypeInfo(info);
				}
				else {
					InitXPClassTypeInfo(info);
				}
			}
		}
		public void EnumMembers(TypeInfo info, EnumMembersHandler handler) {
			Type type = info.Type;
			if(TypeIsKnown(type)) {
				if(type.IsInterface) {
					EnumDCInterfaceMembers(info, handler);
				}
				else {
					EnumXPClassMembers(info, handler);
				}
			}
		}
		public void InitMemberInfo(Object memberSource, XafMemberInfo member) {
			InitMemberInfo(member.Owner, member, memberSource);
		}
		public virtual void RefreshMemberInfo(TypeInfo typeInfo, XafMemberInfo memberInfo) {
			Type type = typeInfo.Type;
			if(!type.IsInterface) {
				XPClassInfo classInfo = GetRealClassInfo(type);
				if(classInfo != null) {
					XPMemberInfo xpMemberInfo = classInfo.FindMember(memberInfo.Name);
					if(xpMemberInfo != null) {
						InitXPClassMemberInfo(xpMemberInfo, memberInfo);
					}
				}
			}
		}
		public Boolean RegisterNewMember(ITypeInfo typeInfo, XafMemberInfo memberInfo) {
			Boolean result = false;
			try {
				Type type = typeInfo.Type;
				if(TypeIsKnown(type)) {
					Type generatedEntityType = GetEntityTypeByInterface(type);
					if(generatedEntityType != null) {
						type = generatedEntityType;
					}
					XPClassInfo classInfo = GetRealClassInfo(type);
					if(classInfo != null) {
						XPCustomMemberInfo xpMemberInfo = null;
						if(String.IsNullOrWhiteSpace(memberInfo.Expression)) {
							Boolean nonPersistent = typeof(XPBaseCollection).IsAssignableFrom(memberInfo.MemberType);
							xpMemberInfo = classInfo.CreateMember(memberInfo.Name, memberInfo.MemberType, nonPersistent);
						}
						else {
							xpMemberInfo = new XpoExpressionMemberInfo(classInfo, memberInfo.Name, memberInfo.MemberType, memberInfo.Expression);
						}
						InitMemberInfo(typeInfo, memberInfo, xpMemberInfo);
						result = true;
					}
				}
			}
			catch {
				return result;
			}
			return result;
		}
		public override void UpdateMember(XafMemberInfo memberInfo) {
			if(!String.IsNullOrWhiteSpace(memberInfo.Expression)) {
				Type type = memberInfo.Owner.Type;
				Type generatedEntityType = GetEntityTypeByInterface(type);
				if(generatedEntityType != null) {
					type = generatedEntityType;
				}
				XPClassInfo classInfo = GetRealClassInfo(type);
				XpoExpressionMemberInfo xpMemberInfo = classInfo.FindMember(memberInfo.Name) as XpoExpressionMemberInfo;
				if(xpMemberInfo != null) {
					xpMemberInfo.PropertyType = memberInfo.MemberType;
					xpMemberInfo.Expression = memberInfo.Expression;
				}
			}
		}
		public Boolean AddAttribute(IBaseInfo info, Attribute attribute) {
			if(info is ITypeInfo) {
				Type type = ((ITypeInfo)info).Type;
				if(!type.IsInterface && IsRequiredAttributre(attribute)) {
					XPClassInfo xpClassInfo = GetRegisteredEntityClassInfo(type);
					if(xpClassInfo != null) {
						xpClassInfo.AddAttribute(attribute);
					}
				}
				info.AddAttribute(attribute);
				return true;
			}
			if(info is IMemberInfo) {
				IMemberInfo memberInfo = (IMemberInfo)info;
				Type type = memberInfo.Owner.Type;
				if(attribute is FieldSizeAttribute) {
					attribute = new SizeAttribute(((FieldSizeAttribute)attribute).Size);
				}
				if(!type.IsInterface && IsRequiredAttributre(attribute)) {
					XPClassInfo xpClassInfo = GetRegisteredEntityClassInfo(type);
					if(xpClassInfo != null) {
						XPMemberInfo xpMemberInfo = xpClassInfo.FindMember(memberInfo.Name);
						if(xpMemberInfo != null) {
							xpMemberInfo.AddAttribute(attribute);
						}
					}
				}
				info.AddAttribute(attribute);
				return true;
			}
			return false;
		}
		public Boolean RemoveAttribute(IBaseInfo info, Type attributeType) {
			Boolean result = false;
			XPTypeInfo xpTypeInfo = null;
			if(info is ITypeInfo) {
				result = true;
				xpTypeInfo = GetRegisteredEntityClassInfo(((ITypeInfo)info).Type);
			}
			else if(info is IMemberInfo) {
				result = true;
				IMemberInfo memberInfo = (IMemberInfo)info;
				XPClassInfo xpClassInfo = GetRegisteredEntityClassInfo(memberInfo.Owner.Type);
				if(xpClassInfo != null) {
					xpTypeInfo = xpClassInfo.FindMember(memberInfo.Name);
				}
			}
			if(xpTypeInfo != null) {
				xpTypeInfo.RemoveAttribute(attributeType);
			}
			return result;
		}
		public override Object GetValue(IMemberInfo memberInfo, Object obj) {
			Object nativeMemberInfo = null;
			if(nativeMemberInfoDictionary.TryGetValue(memberInfo, out nativeMemberInfo)) {
				if(nativeMemberInfo is XPMemberInfo) {
					return ((XPMemberInfo)nativeMemberInfo).GetValue(obj);
				}
				else if(nativeMemberInfo is PropertyInfo) {
					return ((PropertyInfo)nativeMemberInfo).GetValue(obj, null);
				}
			}
			return null;
		}
		public override void SetValue(IMemberInfo memberInfo, Object obj, Object value) {
			Object nativeMemberInfo = null;
			if(nativeMemberInfoDictionary.TryGetValue(memberInfo, out nativeMemberInfo)) {
				if(nativeMemberInfo is XPMemberInfo) {
					((XPMemberInfo)nativeMemberInfo).SetValue(obj, value);
				}
				else if(nativeMemberInfo is PropertyInfo && ((PropertyInfo)nativeMemberInfo).CanWrite) {
					((PropertyInfo)nativeMemberInfo).SetValue(obj, value, null);
				}
			}
		}
		public Boolean CanRegister(Type type) {
			Guard.ArgumentNotNull(type, "type");
			return TypeIsKnown(type) && !registeredEntityTypes.Contains(type);
		}
		public void RegisterEntity(Type type) {
			if(CanRegister(type)) {
				registeredEntityTypes.Add(type);
				TypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				RegisterUsedEnumerations(typeInfo);
				if(!type.IsInterface) {
					GetRealClassInfo(type);
				}
				typeInfo.Source = this;
				typeInfo.Refresh();
				typeInfo.RefreshMembers();
			}
		}
		public override Type GetOriginalType(Type type) {
			Type result = GetInterfaceByEntityType(type);
			if(result == null) {
				result = base.GetOriginalType(type);
			}
			return result;
		}
		public IEnumerable<Type> RegisteredEntities {
			get { return registeredEntityTypes; }
		}
		public void Reset() {
			CheckDictionary();
			infoDictionary = new ReflectionDictionary();
			entityDictionary = dcEntityDictionary = new XafReflectionDictionary();
			generatedAssembly = null;
			registeredEntityTypes.Clear();
			entitiesToGenerateInfo.Clear();
			customLogics = new CustomLogics();
			existingImplementorsInfo.Clear();
			entityByInterface.Clear();
			interfaceByEntity.Clear();
			dataClassByInterface.Clear();
		}
		public void RegisterEntity(string entityName, Type interfaceType) {
			CheckDictionary();
			entitiesToGenerateInfo.AddEntity(entityName, interfaceType);
			typesInfo.RegisterEntities(typeof(DCBaseObject));
		}
		public void RegisterEntity(string entityName, Type interfaceType, Type baseClass) {
			RegisterEntity(entityName, interfaceType);
			entitiesToGenerateInfo.AddBaseClass(entityName, baseClass);
		}
		public void RegisterSharedPart(Type interfaceType) {
			CheckDictionary();
			entitiesToGenerateInfo.AddSharedPart(interfaceType);
		}
		public void RegisterDomainLogic(Type interfaceType, Type logicType) {
			CheckDictionary();
			customLogics.RegisterLogic(interfaceType, logicType);
		}
		public void UnregisterDomainLogic(Type interfaceType, Type logicType) {
			CheckDictionary();
			customLogics.UnregisterLogic(interfaceType, logicType);
		}
		public void GenerateEntities() {
			GenerateEntities(null);
		}
		public void GenerateEntities(String generatedAssemblyFile) {
			Type[] entityInterfaces = entitiesToGenerateInfo.GetEntityInterfaces();
			if(!IsAlreadyBuild && entityInterfaces.Length > 0) {
				if(File.Exists(generatedAssemblyFile)) {
					generatedAssembly = Assembly.LoadFrom(generatedAssemblyFile);
				}
				else {
					DCBuilder builder = new DCBuilder(typesInfo);
					builder.Setup(entitiesToGenerateInfo, customLogics, existingImplementorsInfo);
					generatedAssembly = builder.GetAssembly(generatedAssemblyFile);
				}
				ProcessGeneratedAssembly(generatedAssembly);
				Dictionary<Type, object> typesToRefresh = new Dictionary<Type, object>();
				foreach(Type entityType in entityInterfaces) {
					if(!typesToRefresh.ContainsKey(entityType)) {
						typesToRefresh.Add(entityType, null);
						foreach(Type implementedInterface in entityType.GetInterfaces()) {
							if(registeredEntityTypes.Contains(implementedInterface) && !typesToRefresh.ContainsKey(implementedInterface)) {
								typesToRefresh.Add(implementedInterface, null);
							}
						}
					}
				}
				foreach(Type typeToRefresh in typesToRefresh.Keys) {
					typesInfo.RefreshInfo(typeToRefresh);
					Type entityType = GetEntityTypeByInterface(typeToRefresh);
					if(entityType != null) {
						typesInfo.RefreshInfo(entityType);
					}
					Type dataType = GetDataTypeByInterface(typeToRefresh);
					if(dataType != null) {
						typesInfo.RefreshInfo(dataType);
					}
				}
				foreach(Type generatedType in generatedAssembly.GetExportedTypes()) {
					if(TypeIsKnown(generatedType)) {
						TypeInfo typeInfo = typesInfo.FindTypeInfo(generatedType) as TypeInfo;
						if(typeInfo != null && typeInfo.Source != this) {
							typeInfo.Source = this;
							typesInfo.RefreshInfo(generatedType);
						}
					}
				}
			}
		}
		public Type GetInterfaceType(Type generatedEntityType) {
			return GetInterfaceByEntityType(generatedEntityType);
		}
		public Type GetGeneratedEntityType(Type interfaceType) {
			return GetEntityTypeByInterface(interfaceType);
		}
		public IEnumerable<Type> GeneratedEntities {
			get {
				if(IsAlreadyBuild) {
					return entitiesToGenerateInfo.GetEntityInterfaces();
				}
				return Type.EmptyTypes;
			}
		}
		#region Obsolete 13.2
		[Obsolete("This method is for internal use only."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public void AddEntityImplementation<IEntityInterface, ImplType>() {
			CheckDictionary();
			Type implementorType = typeof(ImplType);
			Type interfaceType = typeof(IEntityInterface);
			existingImplementorsInfo.AddImplementor(implementorType, interfaceType);
			Type dataType = typeof(IPersistentInterfaceData<IEntityInterface>);
			dcEntityDictionary.ExtendPersistentInterfaces(dataType, implementorType);
			dcEntityDictionary.ExtendPersistentInterfaces(interfaceType, implementorType);
		}
		#endregion
	}
}
