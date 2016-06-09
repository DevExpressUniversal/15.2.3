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
using System.Data.Services.Providers;
using System.Diagnostics;
using DevExpress.Xpo.Metadata;
using System.Linq;
using DevExpress.Xpo.Helpers;
using System.Data.Services;
using System.Reflection;
using System.ServiceModel.Web;
using System.IO;
using DevExpress.Xpo.Metadata.Helpers;
using System.Linq.Expressions;
namespace DevExpress.Xpo {
	public class XpoMetadata : IDataServiceMetadataProvider {
		readonly Dictionary<string, ResourceSet> resourceSets;
		readonly Dictionary<string, ResourceType> resourceTypes;
		readonly Dictionary<Type, ResourceType> resourceTypesDict;
		readonly Dictionary<ResourceType, List<ResourceType>> derivedType;
		readonly Dictionary<ResourceType, List<ResourceType>> allDerivedType;
		readonly Dictionary<Type, StreamInfo> mediaResources;
		readonly Dictionary<Type, StructWrapperInfo> structWrapperDict;
		readonly Dictionary<Type, Dictionary<ResourceProperty, NamedStreamInfo>> namedStreamPropertiesDict;
		readonly Dictionary<Type, Dictionary<string, ResourceProperty>> resourcePropertyDict;
		readonly string containerName;
		readonly string namespaceName;
		readonly XpoContext context;
		readonly Dictionary<string, XpoAnnotation> annotations;
		public Dictionary<string, XpoAnnotation> Annotations {
			get {
				return annotations;
			}
		}
		public XpoMetadata(XpoContext context, string containerName, string namespaceName) {
			resourceSets = new Dictionary<string, ResourceSet>();
			resourceTypes = new Dictionary<string, ResourceType>();
			resourceTypesDict = new Dictionary<Type, ResourceType>();
			derivedType = new Dictionary<ResourceType, List<ResourceType>>();
			allDerivedType = new Dictionary<ResourceType, List<ResourceType>>();
			structWrapperDict = new Dictionary<Type, StructWrapperInfo>();
			mediaResources = new Dictionary<Type, StreamInfo>();
			List<XPMemberInfo> manyToMany = new List<XPMemberInfo>();
			namedStreamPropertiesDict = new Dictionary<Type, Dictionary<ResourceProperty, NamedStreamInfo>>();
			resourcePropertyDict = new Dictionary<Type, Dictionary<string, ResourceProperty>>();
			annotations = new Dictionary<string, XpoAnnotation>();
			this.containerName = containerName;
			this.namespaceName = namespaceName;
			this.context = context;
			XPDictionary dict = context.ObjectLayer.Dictionary;
			XPClassInfo[] order = GetCreateOrder(dict);
			for(int i = 0; i < order.Length; i++) {
				string typeName = TypeSystem.ProcessTypeName(namespaceName, order[i].ClassType);
				ResourceType entityType;
				XPClassInfo bci = GetBaseClass(order[i]);
				if(bci != null && resourceTypesDict.ContainsKey(bci.ClassType))
					entityType = AddEntityType(typeName, order[i], resourceTypesDict[bci.ClassType]);
				else
					entityType = AddEntityType(typeName, order[i]);
				resourceTypesDict.Add(order[i].ClassType, entityType);
				AddResourceSet(typeName, entityType);
			}
			foreach(XPClassInfo cInfo in order) {
				ResourceType entityType = null;
				if(!resourceTypesDict.TryGetValue(cInfo.ClassType, out entityType)) continue;
				XPMemberInfo[] properties = GetMembers(cInfo, entityType);
				foreach(XPMemberInfo prop in properties.Where(i => !(i.IsCollection || i.IsAssociationList))) {
					if(prop.ReferenceType != null) {
						string refTypeName = TypeSystem.ProcessTypeName(namespaceName, prop.ReferenceType.ClassType);
						string declareTypeName = TypeSystem.ProcessTypeName(namespaceName, prop.Owner.ClassType);
						ResourceSet targetResourceSet = null;
						ResourceSet sourceResourceSet = null;
						ResourceType targetResourceType;
						if(resourceSets.TryGetValue(refTypeName, out targetResourceSet) && resourceSets.TryGetValue(declareTypeName, out sourceResourceSet)
								&& resourceTypesDict.TryGetValue(prop.ReferenceType.ClassType, out targetResourceType)) {
							AddReferenceProperty(prop, entityType, sourceResourceSet, targetResourceType, targetResourceSet, false);
						}
					} else {
						if((prop.IsStruct || !prop.IsPersistent) && !prop.IsAliased) {
							string complexTypeName = TypeSystem.ProcessTypeName(namespaceName, prop.MemberType);
							ResourceType complexType = null;
							if(!resourceTypes.TryGetValue(complexTypeName, out complexType)) {
								complexType = AddComplexType(complexTypeName, prop);
								foreach(var item in prop.SubMembers) {
									ReflectionPropertyInfo fieldInfo = item as ReflectionPropertyInfo;
									if(fieldInfo == null) continue;
									AddPrimitiveProperty(complexType, fieldInfo.Name.Substring(fieldInfo.Name.LastIndexOf('.') + 1), fieldInfo.MemberType, false, null);
								}
							}
							AddComplexProperty(entityType, prop.Name, complexType);
							continue;
						}
						if(prop.IsKey) AddKeyProperty(entityType, prop.Name, prop);
						else AddPrimitiveProperty(entityType, prop.Name, prop);
					}
				}
				foreach(XPMemberInfo colProp in properties.Where(i => (i.IsCollection || i.IsAssociationList) && i.IsManyToMany)) {
					if(context.HidePropertyInternal(cInfo, colProp)) continue;
					string refTypeName = TypeSystem.ProcessTypeName(namespaceName, colProp.CollectionElementType.ClassType);
					string declareTypeName = TypeSystem.ProcessTypeName(namespaceName, colProp.Owner.ClassType);
					ResourceSet targetResourceSet = null;
					ResourceType targetResourceType = null;
					ResourceSet sourceResourceSet = null;
					if(resourceSets.TryGetValue(refTypeName, out targetResourceSet) && resourceSets.TryGetValue(declareTypeName, out sourceResourceSet)
								&& resourceTypesDict.TryGetValue(colProp.CollectionElementType.ClassType, out targetResourceType)) {
						if(manyToMany.Contains(colProp.GetAssociatedMember())) continue;
						AddReferenceProperty(colProp, entityType, sourceResourceSet, targetResourceType, targetResourceSet, true);
						manyToMany.Add(colProp);
					} else {
						Debug.WriteLine(DevExpress.Xpo.Extensions.Properties.Resources.CannotIdentifyCollectionProperty, colProp.Name);
					}
				}
			}
		}
		XPClassInfo GetBaseClass(XPClassInfo classInfo) {
			XPClassInfo result = classInfo.BaseClass;
			while(result != null && !result.IsPersistent) {
				result = result.BaseClass;
			}
			return result;
		}
		XPClassInfo[] GetCreateOrder(XPDictionary dict) {
			Dictionary<XPClassInfo, List<XPClassInfo>> derivedList = new Dictionary<XPClassInfo, List<XPClassInfo>>();
			XPClassInfo otObj = dict.QueryClassInfo(typeof(XPObjectType));
			foreach(XPClassInfo cInfo in dict.Classes) {
				if(!cInfo.IsPersistent || !cInfo.CanGetByClassType || cInfo == otObj || context.HideResourceSetInternal(cInfo)) continue;
				derivedList.Add(cInfo, new List<XPClassInfo>());
				XPClassInfo bci = GetBaseClass(cInfo);
				if(bci != null)
					derivedList[bci].Add(cInfo);
			}
			bool found;
			do {
				found = false;
				foreach(KeyValuePair<XPClassInfo, List<XPClassInfo>> item in derivedList) {
					foreach(XPClassInfo classInfo in item.Value.ToArray()) {
						foreach(XPClassInfo nestedClassInfo in derivedList[classInfo]) {
							if(!derivedList[item.Key].Contains(nestedClassInfo)) {
								derivedList[item.Key].Add(nestedClassInfo);
								found = true;
							}
						}
					}
				}
			} while(found);
			return derivedList.AsParallel().OrderByDescending(i => i.Value.Count).Select(i => i.Key).ToArray();
		}
		XPMemberInfo[] GetMembers(XPClassInfo classInfo, ResourceType entityType) {
			List<XPMemberInfo> properties = new List<XPMemberInfo>();
			Dictionary<XPMemberInfo, bool> subMembers = new Dictionary<XPMemberInfo, bool>();
			foreach(XPMemberInfo prop in classInfo.Members) {
				if((prop.IsPersistent && !subMembers.ContainsKey(prop)) || prop.IsAliased || prop.IsAssociationList) {
					properties.Add(prop);
					if(prop.SubMembers.Count > 0) {
						foreach(XPMemberInfo subMember in prop.SubMembers) {
							subMembers[subMember] = true;
						}
					}
				}
			}
			List<XPMemberInfo> result = new List<XPMemberInfo>();
			foreach(XPMemberInfo prop in properties) {
				ResourceType propertyOwnerResourseType = null;
				if(prop.Owner == classInfo) {
					propertyOwnerResourseType = entityType;
				} else {
					XPClassInfo propertyOwnerClassInfo = GetBaseClass(classInfo);
					while(propertyOwnerClassInfo != null && resourceTypesDict.ContainsKey(propertyOwnerClassInfo.ClassType)) {
						ResourceType baseClassResourseType = resourceTypesDict[propertyOwnerClassInfo.ClassType];
						propertyOwnerClassInfo = propertyOwnerClassInfo.BaseClass;
						if(baseClassResourseType.Properties.Where(i => i.Name == prop.Name).Count() > 0) {
							propertyOwnerResourseType = resourceTypesDict[baseClassResourseType.InstanceType];
							break;
						}
					}
				}
				if(propertyOwnerResourseType == null)
					propertyOwnerResourseType = entityType;
				if(propertyOwnerResourseType != entityType) {
					if(resourcePropertyDict.ContainsKey(propertyOwnerResourseType.InstanceType) && resourcePropertyDict[propertyOwnerResourseType.InstanceType].ContainsKey(prop.Name)) {
						ResourceProperty resProp = resourcePropertyDict[propertyOwnerResourseType.InstanceType][prop.Name];
						if(resProp != null)
							AddResourceProperty(classInfo.ClassType, resProp);
					}
				}
				if(prop.Name == GCRecordField.StaticName || prop.Name == OptimisticLockingAttribute.DefaultFieldName || propertyOwnerResourseType != entityType) continue;
				if(context.HidePropertyInternal(classInfo, prop) && !prop.IsKey) continue;
				result.Add(prop);
			}
			return result.ToArray();
		}
		void AddResourceProperty(Type type, ResourceProperty resourceProperty) {
			Dictionary<string, ResourceProperty> dict;
			if(!resourcePropertyDict.TryGetValue(type, out dict)) {
				dict = new Dictionary<string, ResourceProperty>();
				resourcePropertyDict.Add(type, dict);
			}
			if(!dict.ContainsKey(resourceProperty.Name))
				dict.Add(resourceProperty.Name, resourceProperty);
		}
		void AddReferenceProperty(XPMemberInfo column, ResourceType sourceResourceType, ResourceSet sourceResourceSet, ResourceType targetResourceType, ResourceSet targetResourceSet, bool isManyToMany) {
			ResourceProperty property = new ResourceProperty(column.Name, isManyToMany ? ResourcePropertyKind.ResourceSetReference : ResourcePropertyKind.ResourceReference, targetResourceType);
			property.CanReflectOnInstanceTypeProperty = false;
			sourceResourceType.AddProperty(property);
			AddResourceProperty(sourceResourceType.InstanceType, property);
			XPMemberInfo targetColumn = column.IsAssociation ? column.GetAssociatedMember() : null; ;
			ResourceProperty targetProperty = null;
			if(targetColumn != null) {
				targetProperty = new ResourceProperty(targetColumn.Name, ResourcePropertyKind.ResourceSetReference, sourceResourceType);
				targetProperty.CanReflectOnInstanceTypeProperty = false;
				targetResourceType.AddProperty(targetProperty);
				AddResourceProperty(targetResourceType.InstanceType, targetProperty);
			}
			ResourcePropertyAnnotation annot = new ResourcePropertyAnnotation() {
				ResourceAssociationSet = new ResourceAssociationSet(
					String.Format("{0}_{1}_{2}", sourceResourceType.Name, column.Name, targetResourceSet.Name),
					new ResourceAssociationSetEnd(sourceResourceSet, sourceResourceType, property),
					new ResourceAssociationSetEnd(targetResourceSet, targetResourceType, targetProperty))
			};
			property.CustomState = annot;
			if(targetColumn != null) {
				targetProperty.CustomState = new ResourcePropertyAnnotation() {
					ResourceAssociationSet = new ResourceAssociationSet(annot.ResourceAssociationSet.Name, annot.ResourceAssociationSet.End2, annot.ResourceAssociationSet.End1)
				};
			}
		}
		public ResourceType AddEntityType(string name, XPClassInfo cInfo) {
			return AddEntityType(name, cInfo, null);
		}
		public ResourceType AddEntityType(string name, XPClassInfo cInfo, ResourceType baseType) {
			ResourceType resourceType = new ResourceType(cInfo.ClassType, ResourceTypeKind.EntityType, baseType, namespaceName, name, false);
			resourceType.CanReflectOnInstanceType = false;
			resourceType.CustomState = new Annotation(cInfo);
			resourceTypes.Add(name, resourceType);
			return resourceType;
		}
		public ResourceType AddComplexType(string name, XPMemberInfo mInfo) {
			StructWrapperInfo structInfo;
			if(!TryResolveStructWrapper(mInfo.MemberType, out structInfo)) {
				structInfo = new StructWrapperInfo(mInfo.MemberType);
				structWrapperDict.Add(mInfo.MemberType, structInfo);
			}
			Type structWrapperType = typeof(StructWrapper<>).MakeGenericType(mInfo.MemberType);
			ResourceType resourceType = new ResourceType(structWrapperType, ResourceTypeKind.ComplexType, null, namespaceName, name, false);
			resourceType.CanReflectOnInstanceType = false;
			resourceTypes.Add(name, resourceType);
			return resourceType;
		}
		Type GetNullablePrimitiveType(Type type) {
			if(type.IsValueType && !type.IsGenericType) {
				TypeCode typeCode = Type.GetTypeCode(type);
				switch(typeCode) {
					case TypeCode.Boolean:
						return typeof(Nullable<bool>);
					case TypeCode.Byte:
						return typeof(Nullable<byte>);
					case TypeCode.Char:
						return typeof(string);
					case TypeCode.DateTime:
						return typeof(Nullable<DateTime>);
					case TypeCode.Decimal:
						return typeof(Nullable<decimal>);
					case TypeCode.Double:
						return typeof(Nullable<double>);
					case TypeCode.Int16:
						return typeof(Nullable<Int16>);
					case TypeCode.Int32:
						return typeof(Nullable<int>);
					case TypeCode.Int64:
						return typeof(Nullable<long>);
					case TypeCode.SByte:
						return typeof(Nullable<sbyte>);
					case TypeCode.Single:
						return typeof(Nullable<float>);
					case TypeCode.UInt16:
						return typeof(Nullable<Int32>);
					case TypeCode.UInt32:
						return typeof(Nullable<Int64>);
					case TypeCode.UInt64:
						return typeof(Nullable<Int64>);
				}
				return typeof(Nullable<>).MakeGenericType(type);
			}
			return type;
		}
		public void AddKeyProperty(ResourceType resourceType, string name, XPMemberInfo memberInfo) {
			AddPrimitiveProperty(resourceType, name, CheckMemberType(memberInfo), true, memberInfo);
		}
		public void AddPrimitiveProperty(ResourceType resourceType, string name, XPMemberInfo memberInfo) {
			string streamContentType = GetStreamEntryContentType(memberInfo);
			if(!string.IsNullOrEmpty(streamContentType)) {
				StreamInfo streamContent;
				if(!TryResolveMediaResource(resourceType.InstanceType, out streamContent)) {
					mediaResources.Add(resourceType.InstanceType, new StreamInfo(name, streamContentType));
				}
				resourceType.IsMediaLinkEntry = true;
				return;
			}
			NamedStreamInfo namedStreamData = GetNamedStreamInfo(memberInfo);
			if(namedStreamData != null) {
				ResourceProperty namedStreamProperty = new ResourceProperty(namedStreamData.Name, ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(Stream)));
				resourceType.AddProperty(namedStreamProperty);
				AddNamedStreamPropertyToDict(resourceType.InstanceType, namedStreamProperty, new NamedStreamInfo(memberInfo.Name, namedStreamData.ContentType));
				return;
			}
			AddPrimitiveProperty(resourceType, name, GetNullablePrimitiveType(CheckMemberType(memberInfo)), false, memberInfo);
		}
		void AddNamedStreamPropertyToDict(Type type, ResourceProperty namedStreamResourceProperty, NamedStreamInfo streamProperty) {
			lock(this) {
				if(!namedStreamPropertiesDict.ContainsKey(type)) namedStreamPropertiesDict.Add(type, new Dictionary<ResourceProperty, NamedStreamInfo>());
				namedStreamPropertiesDict[type].Add(namedStreamResourceProperty, streamProperty);
			}
		}
		static Type CheckMemberType(XPMemberInfo memberInfo) {
			Type memberType;
			if(memberInfo.Converter == null)
				memberType = memberInfo.MemberType;
			else
				memberType = memberInfo.StorageType;
			return memberType;
		}
		string GetStreamEntryContentType(XPMemberInfo memberInfo) {
			string result = string.Empty;
			StreamAttribute attr = memberInfo.FindAttributeInfo(typeof(StreamAttribute)) as StreamAttribute;
			if(attr != null && attr.IsNamed == false) {
				if(memberInfo.StorageType != GetNullablePrimitiveType(typeof(byte[])) && memberInfo.StorageType != GetNullablePrimitiveType(typeof(string)))
					throw new DataServiceException(500, string.Format("Internal Server Error: The '{0}' property of the '{1}' class cannot be represented as MediaResource(MR).", memberInfo.Name, memberInfo.Owner.ClassType.Name));
				result = attr.ContentType;
			}
			string overridenAttribute = context.SetStreamContentType(memberInfo.Owner.ClassType, memberInfo.Name);
			if(!string.IsNullOrEmpty(overridenAttribute)) {
				if(memberInfo.MemberType != GetNullablePrimitiveType(typeof(byte[])) && memberInfo.StorageType != GetNullablePrimitiveType(typeof(string)))
					throw new DataServiceException(500, string.Format("Internal Server Error: The '{0}' property of the '{1}' class cannot be represented as MediaResource(MR).", memberInfo.Name, memberInfo.Owner.ClassType.Name));
				result = overridenAttribute;
			}
			return result;
		}
		NamedStreamInfo GetNamedStreamInfo(XPMemberInfo memberInfo) {
			StreamAttribute attr = memberInfo.FindAttributeInfo(typeof(StreamAttribute)) as StreamAttribute;
			if(attr != null && attr.IsNamed == true) {
				if(memberInfo.StorageType != GetNullablePrimitiveType(typeof(byte[])) && memberInfo.StorageType != GetNullablePrimitiveType(typeof(string)))
					throw new DataServiceException(500, string.Format("Internal Server Error: The '{0}' property of the '{1}' class cannot be represented as Named Stream).", memberInfo.Name, memberInfo.Owner.ClassType.Name));
				return new NamedStreamInfo(attr.Name, attr.ContentType);
			}			
			NamedStreamInfo overridenAttribute = context.SetNamedStreamData(memberInfo.Owner.ClassType, memberInfo.Name);
			if(overridenAttribute != null) {
				if(memberInfo.StorageType != GetNullablePrimitiveType(typeof(byte[])) && memberInfo.StorageType != GetNullablePrimitiveType(typeof(string)))
					throw new DataServiceException(500, string.Format("Internal Server Error: The '{0}' property of the '{1}' class cannot be represented as Named Stream.", memberInfo.Name, memberInfo.Owner.ClassType.Name));
				if(overridenAttribute.ContentType != null)
					return overridenAttribute;
				else
					return null;
			}
			if(memberInfo.StorageType == GetNullablePrimitiveType(typeof(byte[]))) {
				if(context.ShowLargePropertyAsNamedStream(memberInfo.Owner.ClassType, memberInfo.Name))
					return new NamedStreamInfo(memberInfo.Name, "application/streamingmedia");
			}
			return null;
		}
		private void AddPrimitiveProperty(ResourceType resourceType, string name, Type propertyType, bool isKey, XPMemberInfo memberInfo) {
			XpoAnnotation annotation = new XpoAnnotation();
			if(memberInfo != null) {
				if(memberInfo.IsAliased) {
					annotation.ReadOnly = true;
				}
				if(memberInfo.StorageType == typeof(string) || memberInfo.StorageType == typeof(byte[])) {
					annotation.Size = memberInfo.MappingFieldSize;
				}
			}
			ResourceType type = ResourceType.GetPrimitiveResourceType(propertyType);
			if(type == null) return;
			ResourcePropertyKind kind = ResourcePropertyKind.Primitive;
			if(isKey) {
				kind |= ResourcePropertyKind.Key;
			}
			ResourceProperty property = new ResourceProperty(name, kind, type);
			property.CanReflectOnInstanceTypeProperty = false;
			resourceType.AddProperty(property);
			AddResourceProperty(resourceType.InstanceType, property);
			if(!annotation.IsDefault) {
				annotations.Add(string.Format("{0}/{1}", resourceType.FullName, name), annotation);
			}
		}
		public void AddComplexProperty(ResourceType resourceType, string name, ResourceType complexType) {
			if(complexType.ResourceTypeKind != ResourceTypeKind.ComplexType) {
				throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.AddComplexPropertyException, complexType.FullName, name));
			}
			ResourceProperty property = new ResourceProperty(name, ResourcePropertyKind.ComplexType, complexType);
			property.CanReflectOnInstanceTypeProperty = false;
			resourceType.AddProperty(property);
			AddResourceProperty(resourceType.InstanceType, property);
		}
		public ResourceSet AddResourceSet(string name, ResourceType entityType) {
			if(entityType.ResourceTypeKind != ResourceTypeKind.EntityType) {
				throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.AddResourceSetException, entityType.FullName, name));
			}
			ResourceSet resourceSet;
			if(entityType.BaseType != null && entityType.BaseType.GetAnnotation().ResourceSet != null) {
				resourceSet = entityType.BaseType.GetAnnotation().ResourceSet;
			} else {
				resourceSet = new ResourceSet(name, entityType);
			}
			resourceSets.Add(name, resourceSet);
			entityType.GetAnnotation().ResourceSet = resourceSet;
			return resourceSet;
		}
		internal void SetReadOnly() {
			lock(this) {
				foreach(var type in resourceTypes.Values) {
					type.SetReadOnly();
				}
				foreach(var set in resourceSets.Values) {
					set.SetReadOnly();
				}
				foreach(var serviceOperation in this.context.ServiceOperations) {
					serviceOperation.SetReadOnly();
				}
			}
		}
		#region IDataServiceMetadataProvider Members
		public string ContainerName {
			get { return containerName; }
		}
		public string ContainerNamespace {
			get { return namespaceName; }
		}
		public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType) {
			lock(allDerivedType) {
				if(!CheckDerivedTypes(resourceType)) return null;
				List<ResourceType> result;
				if(!allDerivedType.TryGetValue(resourceType, out result)) {
					result = BuildAllDerivedTypes(resourceType);
					allDerivedType.Add(resourceType, result);
				}
				return result;
			}
		}
		public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty) {
			lock(this) {
				ResourceAssociationSet resourceAssociationSet = resourceProperty.GetAnnotation().ResourceAssociationSet;
				if(resourceAssociationSet.End1.ResourceSet != resourceSet)
					return null;
				if(resourceAssociationSet.End1.ResourceType != resourceType)
					return null;
				if(resourceAssociationSet.End1.ResourceProperty != resourceProperty)
					return null;
				return resourceAssociationSet;
			}
		}
		bool CheckDerivedTypes(ResourceType resourceType) {
			if(resourceType == null) return false;
			List<ResourceType> resList;
			if(!derivedType.TryGetValue(resourceType, out resList)) {
				resList = new List<ResourceType>();
				derivedType.Add(resourceType, resList);
				foreach(ResourceType item in resourceTypesDict.Values) {
					XPClassInfo baseClassInfo = item.GetAnnotation().ClassInfo.BaseClass;
					ResourceType cachedResType;
					if(resourceTypesDict.TryGetValue(baseClassInfo.ClassType, out cachedResType) && cachedResType == resourceType)
						derivedType[cachedResType].Add(item);
				}
			}
			return resList.Count == 0 ? false : true;
		}
		List<ResourceType> BuildAllDerivedTypes(ResourceType resourceType) {
			if(!CheckDerivedTypes(resourceType)) return null;
			List<ResourceType> listAllTypes = derivedType[resourceType];
			for(int i = 0; i < listAllTypes.Count; i++) {
				ResourceType item = listAllTypes[i];
				List<ResourceType> listTypes = BuildAllDerivedTypes(item);
				if(listTypes != null) listAllTypes.AddRange(listTypes);
			}
			listAllTypes.Remove(resourceType);
			return listAllTypes;
		}
		public bool HasDerivedTypes(ResourceType resourceType) {
			lock(allDerivedType) {
				return CheckDerivedTypes(resourceType);
			}
		}
		public System.Collections.Generic.IEnumerable<ResourceSet> ResourceSets {
			get {
				lock(this) {
					return resourceSets.Values.AsParallel().Distinct();
				}
			}
		}
		public System.Collections.Generic.IEnumerable<ServiceOperation> ServiceOperations {
			get { return this.context.ServiceOperations.ToArray(); }
		}
		public bool TryResolveResourceSet(string name, out ResourceSet resourceSet) {
			return resourceSets.TryGetValue(name, out resourceSet);
		}
		public bool TryResolveResourceType(string name, out ResourceType resourceType) {
			string typeName = TypeSystem.ProcessTypeName(namespaceName, name);
			return resourceTypes.TryGetValue(typeName, out resourceType);
		}
		public bool TryResolveResourceTypeByType(Type type, out ResourceType resourceType) {
			return resourceTypesDict.TryGetValue(type, out resourceType);
		}
		public bool ContainsStructWrapper(Type type) {
			return structWrapperDict.ContainsKey(type);
		}
		public bool TryResolveStructWrapper(Type type, out StructWrapperInfo structWrapperInfo) {
			return structWrapperDict.TryGetValue(type, out structWrapperInfo);
		}
		public bool TryResolveMediaResource(Type objectType, out StreamInfo mediaResource) {
			lock(this) {
				ResourceType resType;
				while(!mediaResources.TryGetValue(objectType, out mediaResource)) {
					if(TryResolveResourceTypeByType(objectType, out resType)) {
						if(resType.BaseType != null) objectType = resType.BaseType.InstanceType;
						else break;
					} else return false;
				}
				return mediaResources.TryGetValue(objectType, out mediaResource);
			}
		}
		public bool TryResolveNamedStream(Type objectType, ResourceProperty namedStreamResourceProperty, out NamedStreamInfo propertyData) {
			lock(this) {
				propertyData = null;
				Dictionary<ResourceProperty, NamedStreamInfo> propertiesDict;
				ResourceType resType;
				while(!namedStreamPropertiesDict.Any(i => i.Key == objectType && i.Value.Keys.Contains(namedStreamResourceProperty))) {
					if(TryResolveResourceTypeByType(objectType, out resType)) {
						if(resType.BaseType != null) objectType = resType.BaseType.InstanceType;
						else break;
					} else return false;
				}
				namedStreamPropertiesDict.TryGetValue(objectType, out propertiesDict);
				return propertiesDict.TryGetValue(namedStreamResourceProperty, out propertyData);
			}
		}
		public bool TryResolveResourceProperty(Type objectType, string propertyName, out ResourceProperty property) {
			lock(this) {
				Dictionary<string, ResourceProperty> properties;
				if(!resourcePropertyDict.TryGetValue(objectType, out properties)) {
					property = null;
					return false;
				}
				return properties.TryGetValue(propertyName, out property);
			}
		}
		public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation) {
			return this.context.TryResolveServiceOperation(name, out serviceOperation);
		}
		public System.Collections.Generic.IEnumerable<ResourceType> Types {
			get {
				lock(this) {
					return resourceTypes.Values;
				}
			}
		}
		#endregion
	}
	public class StructWrapper<T> {
		public StructWrapper() {
		}
	}
	public class StructWrapperInfo {
		struct MemberCacheItem {
			public GetterHandler Getter;
			public SetterHandler Setter;
		}
		Dictionary<string, MemberCacheItem> memberCache = new Dictionary<string, MemberCacheItem>();
		delegate object GetterHandler(object instance);
		delegate void SetterHandler(object instance, object value);
		readonly Type instanceType;
		readonly static ParameterExpression ModelInstanceParameter = Expression.Parameter(typeof(object), "i");
		readonly static ParameterExpression ValueParameter = Expression.Parameter(typeof(object), "v");
		public StructWrapperInfo(Type type) {
			this.instanceType = type;
		}
		public Type InstanceType { get { return instanceType; } }
		public object GetValue(object instance, string propertyName) {
			MemberCacheItem item = GetMemberCacheItem(propertyName);
			return item.Getter(instance);
		}
		public void SetValue(object instance, string propertyName, object value) {
			MemberCacheItem item = GetMemberCacheItem(propertyName);
			item.Setter(instance, value);
		}
		MemberCacheItem GetMemberCacheItem(string propetyName) {
			MemberCacheItem item;
			lock(memberCache) {
				if(!memberCache.TryGetValue(propetyName, out item)) {
					MemberInfo[] members = instanceType.GetMember(propetyName);
					if(members.Length != 1) {
						throw new NotSupportedException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, instanceType));
					}
					item = new MemberCacheItem();
					MemberInfo member = members[0];
					switch(member.MemberType) {
						case MemberTypes.Field: {
								FieldInfo fi = (FieldInfo)member;
								item.Getter = Expression.Lambda<GetterHandler>(Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(ModelInstanceParameter, fi.DeclaringType), member), typeof(object)), ModelInstanceParameter).Compile();
								Expression modelExpressionForAssign = Expression.Field(Expression.Convert(ModelInstanceParameter, fi.DeclaringType), fi);
								item.Setter = Expression.Lambda<SetterHandler>(Expression.Assign(modelExpressionForAssign, Expression.Convert(ValueParameter, fi.FieldType)), ModelInstanceParameter, ValueParameter).Compile();
							} break;
						case MemberTypes.Property: {
								PropertyInfo pi = (PropertyInfo)member;
								if(pi.CanRead) {
									item.Getter = Expression.Lambda<GetterHandler>(Expression.Convert(Expression.Call(Expression.Convert(ModelInstanceParameter, pi.DeclaringType), pi.GetGetMethod()), typeof(object)), ModelInstanceParameter).Compile();
								}
								if(pi.CanWrite) {
									item.Setter = Expression.Lambda<SetterHandler>(Expression.Call(Expression.Convert(ModelInstanceParameter, pi.DeclaringType), pi.GetSetMethod(), Expression.Convert(ValueParameter, pi.PropertyType)), ModelInstanceParameter, ValueParameter).Compile();
								}
							} break;
						default:
							throw new NotSupportedException(member.MemberType.ToString());
					}
					memberCache.Add(propetyName, item);
				}
			}
			return item;
		}
	}
}
