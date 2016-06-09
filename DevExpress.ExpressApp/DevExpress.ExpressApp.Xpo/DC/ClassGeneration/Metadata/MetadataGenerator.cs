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
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal interface IMetadataSource {
		EntityMetadata[] GetEntitiesMetadata();
		InterfaceMetadata[] GetInterfacesMetadata();
		InterfaceMetadata FindInterfaceMetadataByType(Type interfaceType);
	}
	sealed class MetadataGenerator : IMetadataSource {
		internal static string GetDataMetadataName(Type interfaceType) {
			return string.Format("{0}_Data", interfaceType.Name);
		}
		private readonly ITypesInfo typesInfo;
		private readonly InterfaceInheritanceMap interfaceInheritanceMap;
		private readonly Dictionary<Type, EntityMetadata> entityClasses;
		private readonly Dictionary<Type, DataMetadata> dataClasses;
		private readonly Dictionary<Type, InterfaceMetadata> interfaces;
		private readonly Dictionary<Type, object> interfacesWithExistingImplementors;
		private EntitiesToGenerateInfo entitiesToGenerateInfo;
		internal MetadataGenerator(ITypesInfo typesInfo) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			this.typesInfo = typesInfo;
			interfaceInheritanceMap = new InterfaceInheritanceMap();
			entityClasses = new Dictionary<Type, EntityMetadata>();
			dataClasses = new Dictionary<Type, DataMetadata>();
			interfaces = new Dictionary<Type, InterfaceMetadata>();
			interfacesWithExistingImplementors = new Dictionary<Type, object>();
		}
		private void CollectInterfacesWithExistingImplementors(ExistingImplementorsInfo existingImplementorsInfo) {
			foreach(Type interfaceType in existingImplementorsInfo.GetInterfaces()) {
				if(!interfacesWithExistingImplementors.ContainsKey(interfaceType)) {
					interfacesWithExistingImplementors.Add(interfaceType, null);
					foreach(Type implementedInterfaceType in interfaceType.GetInterfaces()) {
						if(!interfacesWithExistingImplementors.ContainsKey(implementedInterfaceType)) {
							interfacesWithExistingImplementors.Add(implementedInterfaceType, null);
						}
					}
				}
			}
		}
		private bool IsDomainComponent(Type interfaceType) {
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceType);
			return interfaceTypeInfo.IsDomainComponent;
		}
		private bool IsPersistent(Type interfaceType) {
			if(!IsDomainComponent(interfaceType)) {
				return false;
			}
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceType);
			return (interfaceTypeInfo.FindAttribute<NonPersistentDcAttribute>(false) == null);
		}
		private void CollectEntityClasses() {
			foreach(string entityName in entitiesToGenerateInfo.GetEntityNames()) {
				CreateEntityMetadata(entityName);
			}
		}
		private void CollectInterfaces() {
			foreach(Type type in interfaceInheritanceMap.GetProcessedTypes()) {
				CheckInterfaceType(type);
				CreateInterfaceMetadata(type);
			}
			CheckInterfaceImplementations();
			SetupInterfacesMetadata();
		}
		private void CheckInterfaceType(Type interfaceType) {
			if(interfaceType.IsGenericType) {
				throw new InvalidOperationException("Generic interfaces are not supported.");
			}
		}
		private InterfaceMetadata CreateInterfaceMetadata(Type interfaceType) {
			InterfaceMetadata metadata = new InterfaceMetadata();
			metadata.InterfaceType = interfaceType;
			metadata.FullName = interfaceType.FullName;
			metadata.IsDomainComponent = IsDomainComponent(interfaceType);
			metadata.IsPersistent = IsPersistent(interfaceType);
			interfaces.Add(interfaceType, metadata);
			return metadata;
		}
		private void CheckInterfaceImplementations() {
			foreach(InterfaceMetadata interfaceMetadata in interfaces.Values) {
				if(!interfaceMetadata.IsPersistent) {
					foreach(Type parent in interfaceInheritanceMap.GetParentTypes(interfaceMetadata.InterfaceType)) {
						if(interfaces[parent].IsPersistent) {
							throw new InvalidOperationException(string.Format("You cannot use '{0}' as the base interface for '{1}' because '{0}' is the Domain Component.", parent.FullName, interfaceMetadata.FullName));
						}
					}
				}
			}
		}
		private void SetupInterfacesMetadata() {
			foreach(InterfaceMetadata interfaceMetadata in interfaces.Values) {
				CollectInterfaceAttributes(interfaceMetadata);
				CollectInterfaceProperties(interfaceMetadata);
				CollectInterfaceMethods(interfaceMetadata);
			}
		}
		private void CollectInterfaceAttributes(InterfaceMetadata interfaceMetadata) {
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceMetadata.InterfaceType);
			foreach(Attribute attribute in interfaceTypeInfo.FindAttributes<Attribute>(false)) {
				interfaceMetadata.Attributes.Add(attribute);
			}
		}
		private void CollectInterfaceProperties(InterfaceMetadata interfaceMetadata) {
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceMetadata.InterfaceType);
			foreach(IMemberInfo interfaceMemberInfo in interfaceTypeInfo.OwnMembers) {
				PropertyMetadata propertyMetadata = CreatePropertyMetadata(interfaceMetadata, interfaceMemberInfo);
				interfaceMetadata.Properties.Add(propertyMetadata);
			}
		}
		private PropertyMetadata CreatePropertyMetadata(InterfaceMetadata interfaceMetadata, IMemberInfo interfaceMemberInfo) {
			PropertyInfo propertyInfo = interfaceMemberInfo.GetExtender<MemberInfo>() as PropertyInfo;
			if(propertyInfo != null && !propertyInfo.CanRead) {
				throw new InvalidOperationException(string.Format("The '{0}.{1}' property must expose a getter. Write-only properties are not supported.", interfaceMetadata.FullName, interfaceMemberInfo.Name));
			}
			PropertyMetadata propertyMetadata = new PropertyMetadata();
			propertyMetadata.Owner = interfaceMetadata;
			propertyMetadata.Name = interfaceMemberInfo.Name;
			propertyMetadata.IsReadOnly = interfaceMemberInfo.IsReadOnly;
			propertyMetadata.PropertyType = interfaceMemberInfo.MemberType;
			bool containsExclusiveAttribute = false;
			foreach(Attribute attribute in interfaceMemberInfo.FindAttributes<Attribute>(false)) {
				if(attribute is CalculatedAttribute || attribute is PersistentDcAttribute || attribute is NonPersistentDcAttribute) {
					if(containsExclusiveAttribute) {
						throw new InvalidOperationException(string.Format("CalculatedAttribute, PersistentDcAttribute, NonPersistentDcAttribute are exclusive. Only one of them can be applied to property '{0}.{1}'.", interfaceMetadata.FullName, interfaceMemberInfo.Name));
					}
					containsExclusiveAttribute = true;
				}
				propertyMetadata.Attributes.Add(attribute);
			}
			if(interfaceMemberInfo.IsAssociation) {
				CreateAssociationInfo(interfaceMetadata, interfaceMemberInfo, propertyMetadata);
				propertyMetadata.IsPersistent = true;
			}
			else if(IsImlicitAssociation(propertyMetadata)) {
				SetupImpicitAssociation(propertyMetadata, interfaceMemberInfo.IsAggregated);
				propertyMetadata.IsPersistent = true;
			}
			else {
				propertyMetadata.IsPersistent = GetIsPersistent(interfaceMetadata, propertyMetadata);
				propertyMetadata.IsLogicRequired = GetIsLogicRequired(interfaceMetadata, propertyMetadata);
			}
			return propertyMetadata;
		}
		private bool IsImlicitAssociation(PropertyMetadata property) {
			if(!property.IsReadOnly) return false;
			if(property.FindAttribute<NonPersistentDcAttribute>() != null) return false;
			Type propertyType = property.PropertyType;
			if(propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IList<>)) {
				Type listElementType = propertyType.GetGenericArguments()[0];
				InterfaceMetadata associatedInterface = FindInterfaceMetadataByType(listElementType);
				if(associatedInterface != null) {
					return property.Owner.IsPersistent && associatedInterface.IsPersistent;
				}
			}
			return false;
		}
		private void SetupImpicitAssociation(PropertyMetadata property, bool isAggregated) {
			Type associatedInterfaceType = property.PropertyType.GetGenericArguments()[0];
			InterfaceMetadata associatedInterface = FindInterfaceMetadataByType(associatedInterfaceType);
			PropertyMetadata associatedProperty = isAggregated ? CreateImplicitAssociatedReference(property) : CreateImplicitAssociatedCollection(property);
			associatedProperty.Owner = associatedInterface;
			associatedProperty.IsPersistent = true;
			associatedInterface.Properties.Add(associatedProperty);
			CreateAssociation(associatedProperty, property, isAggregated ? AssociationType.OneToMany : AssociationType.ManyToMany);
		}
		private PropertyMetadata CreateImplicitAssociatedCollection(PropertyMetadata property) {
			PropertyMetadata associatedPropertyMetadata = new PropertyMetadata();
			associatedPropertyMetadata.Name = string.Format("Implicit_{0}_{1}_List", property.Owner.InterfaceType.Name, property.Name);
			associatedPropertyMetadata.IsReadOnly = true;
			associatedPropertyMetadata.PropertyType = typeof(IList<>).MakeGenericType(property.Owner.InterfaceType);
			return associatedPropertyMetadata;
		}
		private PropertyMetadata CreateImplicitAssociatedReference(PropertyMetadata property) {
			PropertyMetadata associatedPropertyMetadata = new PropertyMetadata();
			associatedPropertyMetadata.Name = string.Format("Implicit_{0}_{1}", property.Owner.InterfaceType.Name, property.Name);
			associatedPropertyMetadata.IsReadOnly = false;
			associatedPropertyMetadata.PropertyType = property.Owner.InterfaceType;
			return associatedPropertyMetadata;
		}
		private bool GetIsPersistent(InterfaceMetadata interfaceMetadata, PropertyMetadata property) {
			if(!IsDomainComponent(interfaceMetadata.InterfaceType)) {
				return false;
			}
			if(property.AssociationInfo != null) {
				return true;
			}
			if(property.FindAttribute<NonPersistentDcAttribute>() != null) {
				return false;
			}
			if(property.FindAttribute<PersistentDcAttribute>() != null) {
				return true;
			}
			if(property.IsReadOnly) {
				return false;
			}
			Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
			Type checkingType = underlyingType ?? property.PropertyType;
			if(DBColumn.IsStorableType(checkingType)) {
				return true;
			}
			if(checkingType.IsInterface) {
				if(!interfaceInheritanceMap.IsProcessedType(checkingType) && !interfacesWithExistingImplementors.ContainsKey(checkingType)) {
					return false;
				}
				return IsPersistent(checkingType);
			}
			ITypeInfo propertyTypeInfo = typesInfo.FindTypeInfo(checkingType);
			return propertyTypeInfo.IsPersistent;
		}
		private bool GetIsLogicRequired(InterfaceMetadata interfaceMetadata, PropertyMetadata property) {
			if(property.IsReadOnly) {
				return (property.FindAttribute<CalculatedAttribute>() == null);
			}
			if(!GetIsPersistent(interfaceMetadata, property)) {
				return true;
			}
			return false;
		}
		private void CreateAssociationInfo(InterfaceMetadata interfaceMetadata, IMemberInfo interfaceMemberInfo, PropertyMetadata propertyMetadata) {
			PropertyMetadata associatedProperty = null;
			IMemberInfo associatedMemberInfo = interfaceMemberInfo.AssociatedMemberInfo;
			Type associatedMemberOwnerType = associatedMemberInfo.Owner.Type;
			InterfaceMetadata associatedInterface;
			if(!interfaces.TryGetValue(associatedMemberOwnerType, out associatedInterface)) {
				if(interfaceMetadata.InterfaceType == associatedMemberOwnerType) {
					associatedInterface = interfaceMetadata;
				}
			}
			if(associatedInterface != null) {
				foreach(PropertyMetadata property in associatedInterface.Properties) {
					if(property.Name == associatedMemberInfo.Name) {
						associatedProperty = property;
						break;
					}
				}
			}
			if(associatedProperty != null) {
				AssociationType associationType = AssociationType.OneToMany;
				if(interfaceMemberInfo.IsManyToMany) {
					associationType = AssociationType.ManyToMany;
				}
				CreateAssociation(propertyMetadata, associatedProperty, associationType);
			}
		}
		private void CreateAssociation(PropertyMetadata property, PropertyMetadata associatedProperty, AssociationType associationType) {
			string associationName = string.Format("{0}_{1}", associatedProperty.Owner.InterfaceType.Name, associatedProperty.Name);
			AssociationMetadata associationInfo = new AssociationMetadata(associationName, associationType);
			associationInfo.AssociatedProperty = associatedProperty;
			property.AssociationInfo = associationInfo;
			associationInfo = new AssociationMetadata(associationName, associationType);
			associationInfo.AssociatedProperty = property;
			associatedProperty.AssociationInfo = associationInfo;
		}
		private void CollectInterfaceMethods(InterfaceMetadata interfaceMetadata) {
			foreach(MethodInfo interfaceMethodInfo in interfaceMetadata.InterfaceType.GetMethods()) {
				if(!interfaceMethodInfo.IsSpecialName) {
					MethodMetadata methodMetadata = new MethodMetadata();
					methodMetadata.Name = interfaceMethodInfo.Name;
					methodMetadata.Owner = interfaceMetadata;
					methodMetadata.ReturnType = interfaceMethodInfo.ReturnType;
					foreach(ParameterInfo parameterInfo in interfaceMethodInfo.GetParameters()) {
						methodMetadata.Parameters.Add(new ParameterMetadata(parameterInfo.Name, parameterInfo.ParameterType, parameterInfo.IsOut));
					}
					foreach(Attribute attribute in interfaceMethodInfo.GetCustomAttributes(false)) {
						methodMetadata.Attributes.Add(attribute);
					}
					interfaceMetadata.Methods.Add(methodMetadata);
				}
			}
		}
		private DataMetadata CreateDataMetadata(Type interfaceType) {
			DataMetadata dataClass = new DataMetadata();
			dataClass.Name = GetDataMetadataName(interfaceType);
			dataClass.StoredInterfaces.Add(interfaces[interfaceType]);
			foreach(Type implementedInterface in interfaceType.GetInterfaces()) {
				dataClass.StoredInterfaces.Add(interfaces[implementedInterface]);
			}
			dataClasses.Add(interfaceType, dataClass);
			return dataClass;
		}
		private EntityMetadata CreateEntityMetadata(string entityName) {
			EntityMetadata metadata = new EntityMetadata();
			metadata.Name = entityName;
			if(entitiesToGenerateInfo.HasBaseClass(entityName)) {
				metadata.BaseClass = entitiesToGenerateInfo.GetBaseClass(entityName);
			}
			Type entityType = entitiesToGenerateInfo.GetEntityInterface(entityName);
			InterfaceMetadata interfaceMetadata = interfaces[entityType];
			metadata.OwnImplementedInterfaces.Add(interfaceMetadata);
			foreach(Type implementedInterface in entityType.GetInterfaces()) {
				interfaceMetadata = interfaces[implementedInterface];
				metadata.OwnImplementedInterfaces.Add(interfaceMetadata);
			}
			entityClasses.Add(entityType, metadata);
			return metadata;
		}
		private void SetBaseEntityToEntities() {
			List<EntityMetadata> list = new List<EntityMetadata>(entityClasses.Values);
			list.Reverse();
			Queue<EntityMetadata> queue = new Queue<EntityMetadata>(list);
			while(queue.Count > 0) {
				EntityMetadata entity = queue.Dequeue();
				Type entityType = entitiesToGenerateInfo.GetEntityInterface(entity.Name);
				bool skip = false;
				List<EntityMetadata> candidates = new List<EntityMetadata>();
				foreach(Type implementedInterface in entityType.GetInterfaces()) {
					EntityMetadata candidate;
					if(entityClasses.TryGetValue(implementedInterface, out candidate)) {
						if(queue.Contains(candidate)) {
							skip = true;
							break;
						}
						else {
							candidates.Add(candidate);
						}
					}
				}
				if(!skip) {
					foreach(EntityMetadata candidate in candidates.ToArray()) {
						EntityMetadata item = candidate;
						while(item.BaseEntity != null) {
							item = item.BaseEntity;
							candidates.Remove(item);
						}
					}
					if(candidates.Count == 1) {
						entity.BaseEntity = candidates[0];
					}
				}
				else {
					queue.Enqueue(entity);
				}
			}
			foreach(EntityMetadata entity in entityClasses.Values) {
				if(entity.BaseClass != null && entity.BaseEntity != null) {
					entity.BaseEntity = null;
				}
			}
		}
		private void FilterOwnImplementedInterfaces() {
			foreach(EntityMetadata entity in entityClasses.Values) {
				EntityMetadata baseEntity = entity.BaseEntity;
				while(baseEntity != null) {
					foreach(InterfaceMetadata implementedInterface in baseEntity.OwnImplementedInterfaces) {
						entity.OwnImplementedInterfaces.Remove(implementedInterface);
					}
					baseEntity = baseEntity.BaseEntity;
				}
			}
		}
		private void CollectDataClasses() {
			Dictionary<Type, ClassMetadata> storages = new Dictionary<Type, ClassMetadata>();
			foreach(Type type in interfaceInheritanceMap.GetProcessedTypesOrderedByAssignabilityAscending()) {
				if(IsPersistent(type)) {
					ClassMetadata storage;
					if(entitiesToGenerateInfo.ContainsSharedPart(type)) {
						storage = CreateDataMetadata(type);
					}
					else {
						Type[] childTypes = interfaceInheritanceMap.GetChildrenTypes(type);
						if(childTypes.Length == 0) {
							storage = entityClasses[type];
						}
						else {
							if(entityClasses.ContainsKey(type)) {
								EntityMetadata current = entityClasses[type];
								int count = 0;
								foreach(Type childType in childTypes) {
									EntityMetadata child = storages[childType] as EntityMetadata;
									if(child != null && child.BaseEntity == current) {
										++count;
									}
								}
								if(count < childTypes.Length) {
									storage = CreateDataMetadata(type);
								}
								else {
									storage = current;
								}
							}
							else {
								storage = storages[childTypes[0]];
								foreach(Type childType in childTypes) {
									if(storage != storages[childType]) {
										storage = CreateDataMetadata(type);
										break;
									}
								}
							}
						}
					}
					if(storage is DataMetadata) {
						interfaces[type].DataClass = (DataMetadata)storage;
					}
					storages.Add(type, storage);
				}
			}
		}
		private void SetAggregatedDataToEntities() {
			foreach(EntityMetadata entity in entityClasses.Values) {
				foreach(InterfaceMetadata implementedInterface in entity.OwnImplementedInterfaces) {
					DataMetadata dataClass;
					if(dataClasses.TryGetValue(implementedInterface.InterfaceType, out dataClass)) {
						entity.AggregatedData.Add(dataClass);
					}
				}
			}
		}
		private void SetAggregatedDataToDataClasses() {
			foreach(DataMetadata dataClass in dataClasses.Values) {
				foreach(InterfaceMetadata storedInterface in dataClass.StoredInterfaces) {
					DataMetadata aggregatedDataClass;
					if(dataClasses.TryGetValue(storedInterface.InterfaceType, out aggregatedDataClass) && aggregatedDataClass != dataClass) {
						dataClass.AggregatedData.Add(aggregatedDataClass);
					}
				}
			}
		}
		private void FilterStoredInterfaces() {
			foreach(Type type in interfaceInheritanceMap.GetProcessedTypesOrderedByAssignabilityDescending()) {
				DataMetadata dataClass;
				if(dataClasses.TryGetValue(type, out dataClass)) {
					foreach(DataMetadata aggregated in dataClass.AggregatedData) {
						foreach(InterfaceMetadata stored in aggregated.StoredInterfaces) {
							dataClass.StoredInterfaces.Remove(stored);
						}
					}
				}
			}
		}
		private void SetNeedInitializeKeyPropertyToEntities() {
			foreach(EntityMetadata entity in entityClasses.Values) {
				if(entity.BaseEntity != null) {
					entity.NeedInitializeKeyProperty = false;
				}
				else if(entity.BaseClass == null) {
					entity.NeedInitializeKeyProperty = true;
				}
				else {
					ITypeInfo baseClassInfo = typesInfo.FindTypeInfo(entity.BaseClass);
					IMemberInfo keyMember = baseClassInfo.KeyMember;
					entity.NeedInitializeKeyProperty = (keyMember != null && keyMember.Name == "Oid" && keyMember.MemberType == typeof(Guid) && !keyMember.IsReadOnly);
				}
			}
		}
		private void CheckDataClasses() {
			List<Type> unregisteredInterfacesWithDataClass = new List<Type>();
			foreach(Type interfaceWithDataClass in dataClasses.Keys) {
				if(!entitiesToGenerateInfo.ContainsSharedPart(interfaceWithDataClass)) {
					unregisteredInterfacesWithDataClass.Add(interfaceWithDataClass);
				}
			}
			if(unregisteredInterfacesWithDataClass.Count > 0) {
				List<string> unregisteredInterfaces = new List<string>();
				List<string> commonEntities = new List<string>();
				foreach(Type unregisteredInterfaceWithDataClass in unregisteredInterfacesWithDataClass) {
					unregisteredInterfaces.Add(unregisteredInterfaceWithDataClass.FullName);
					foreach(KeyValuePair<Type, EntityMetadata> entry in entityClasses) {
						if(unregisteredInterfaceWithDataClass.IsAssignableFrom(entry.Key)) {
							string entityName = entry.Value.Name;
							if(!commonEntities.Contains(entityName)) {
								commonEntities.Add(entityName);
							}
						}
					}
				}
				throw new InvalidOperationException(string.Format(
@"Cannot register the following entities because they implement common interfaces decorated with the DomainComponent attribute:
{0}.

As a possible solution, register the following interfaces as shared parts via the RegisterSharedPart method:
{1}."
				, string.Join(",\r\n", commonEntities.ToArray()), string.Join(",\r\n", unregisteredInterfaces.ToArray())));
			}
		}
		private void CheckBaseClasses() {
			foreach(EntityMetadata entity in entityClasses.Values) {
				string entityName = entity.Name;
				if(entitiesToGenerateInfo.HasBaseClass(entityName)) {
					Type baseClass = entitiesToGenerateInfo.GetBaseClass(entityName);
					Type entityInterface = entity.PrimaryInterface.InterfaceType;
					bool containsDataClass = false;
					foreach(Type dataInterface in dataClasses.Keys) {
						if(dataInterface.IsAssignableFrom(entityInterface)) {
							containsDataClass = true;
							break;
						}
					}
					if(containsDataClass) {
						CheckEntityWithSharedPartBaseClass(baseClass, entityName);
					}
					else {
						CheckEntityBaseClass(baseClass);
					}
				}
			}
		}
		private void CheckEntityBaseClass(Type baseClass) {
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(baseClass);
			if(typeInfo == null) throw new InvalidOperationException(string.Format("The types info subsystem does not have information on the '{0}' class.", baseClass.FullName));
			IMemberInfo keyMember = typeInfo.KeyMember;
			if(keyMember == null) throw new InvalidOperationException(string.Format("The '{0}' class has no key member.", baseClass.FullName));
		}
		private void CheckEntityWithSharedPartBaseClass(Type baseClass, string enitityName) {
			CheckEntityBaseClass(baseClass);
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(baseClass);
			IMemberInfo keyMember = typeInfo.KeyMember;
			string commonWarningFormat = "You cannot use the '{0}' class as base for the '{1}' entity that aggregates a shared part.";
			if(keyMember.Name != "Oid") throw new InvalidOperationException(string.Format(commonWarningFormat + " In this case, the base class' key member must be named 'Oid'. However, the '{0}' class' key property name is '{2}'.", baseClass.FullName, enitityName, keyMember.Name));
			if(keyMember.MemberType != typeof(Guid)) throw new InvalidOperationException(string.Format(commonWarningFormat + " In this case, the base class' key member must be of the 'System.Guid' type. However, the '{0}' class' key property type is '{2}'.", baseClass.FullName, enitityName, keyMember.MemberType.FullName));
			if(keyMember.IsReadOnly) throw new InvalidOperationException(string.Format(commonWarningFormat + " In this case, the base class' key property must expose a set accessor. However, the '{0}' class' key property '{2}' does not expose a setter.", baseClass.FullName, enitityName, keyMember.Name));
		}
		internal void GenerateMetadata(EntitiesToGenerateInfo entitiesToGenerateInfo, ExistingImplementorsInfo existingImplementorsInfo) {
			entityClasses.Clear();
			dataClasses.Clear();
			interfaces.Clear();
			interfacesWithExistingImplementors.Clear();
			this.entitiesToGenerateInfo = entitiesToGenerateInfo;
			interfaceInheritanceMap.Build(entitiesToGenerateInfo.GetEntityInterfaces());
			CollectInterfacesWithExistingImplementors(existingImplementorsInfo);
			CollectInterfaces();
			CollectEntityClasses();
			SetBaseEntityToEntities();
			FilterOwnImplementedInterfaces();
			CollectDataClasses();
			SetAggregatedDataToEntities();
			SetAggregatedDataToDataClasses();
			FilterStoredInterfaces();
			SetNeedInitializeKeyPropertyToEntities();
			CheckDataClasses();
			CheckBaseClasses();
		}
		internal EntityMetadata[] GetEntitiesMetadata() {
			return new List<EntityMetadata>(entityClasses.Values).ToArray();
		}
		internal InterfaceMetadata[] GetInterfacesMetadata() {
			return new List<InterfaceMetadata>(interfaces.Values).ToArray();
		}
		internal InterfaceMetadata FindInterfaceMetadataByType(Type interfaceType) {
			InterfaceMetadata interfaceMetadata;
			if(interfaces.TryGetValue(interfaceType, out interfaceMetadata)) {
				return interfaceMetadata;
			}
			return null;
		}
		#region IMetadataSource Members
		EntityMetadata[] IMetadataSource.GetEntitiesMetadata() {
			return GetEntitiesMetadata();
		}
		InterfaceMetadata[] IMetadataSource.GetInterfacesMetadata() {
			return GetInterfacesMetadata();
		}
		InterfaceMetadata IMetadataSource.FindInterfaceMetadataByType(Type interfaceType) {
			return FindInterfaceMetadataByType(interfaceType);
		}
		#endregion
	}
}
