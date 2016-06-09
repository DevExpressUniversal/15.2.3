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
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class CodeModelLogicHelper {
		private LogicsSource logicsSource;
		private IMetadataSource metadataSource;
		public CodeModelLogicHelper(LogicsSource logicsSource, IMetadataSource metadataSource) {
			this.logicsSource = logicsSource;
			this.metadataSource = metadataSource;
		}
		public List<Type> GetConstructorLogicTypes(EntityMetadata entityMetadata) {
			List<Type> allLogicTypes = new List<Type>();
			foreach(CodeModelMethodInfo methodInfo in CodeModelGeneratorHelper.GetClassMethodInfos(entityMetadata)) {
				FillConstructorByMethodLogic(allLogicTypes, methodInfo.MethodMetadata, methodInfo.InterfaceMetadata, entityMetadata);
			}
			foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(entityMetadata, true)) {
				FillConstructorByPropertyLogic(allLogicTypes, propertyInfo.PropertyMetadata, propertyInfo.InterfaceMetadata, entityMetadata);
			}
			foreach(MethodLogic methodLogic in GetObjectChangedMethodsInfo(entityMetadata).Values) {
				FillConstructorLogicTypes(methodLogic, allLogicTypes);
			}
			foreach(MethodLogic methodLogic in GetObjectChangingMethodsInfo(entityMetadata).Values) {
				FillConstructorLogicTypes(methodLogic, allLogicTypes);
			}
			return allLogicTypes;
		}
		private void CheckPropertyLogic(string methodName, Type interfaceType, Type declaringType, Type propertyType, bool usePropertyType) {
			if(usePropertyType) {
				MethodLogic methodLogic = logicsSource.GetMethodLogic(interfaceType, declaringType, methodName, typeof(void), new Type[] { });
				if(methodLogic != null) {
					throw new InvalidOperationException(string.Format("Incorrect signature of the '{0}.{1}' Domain Logic method. This method's last parameter specifies new property value and should be of the {2} type.", methodLogic.Owner.FullName, methodName, propertyType.FullName));
				}
			}
		}
		private Dictionary<PropertyMetadata, MethodLogic> GetObjectMethodsInfo(ClassMetadata classMetadata, string methodPrefix, bool usePropertyType) {
			Dictionary<PropertyMetadata, MethodLogic> result = new Dictionary<PropertyMetadata, MethodLogic>();
			IList<InterfaceMetadata> implementedInterfaces = null;
			if(classMetadata is EntityMetadata) {
				EntityMetadata entityMetadata = classMetadata as EntityMetadata;
				implementedInterfaces = entityMetadata.OwnImplementedInterfaces;
				foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(entityMetadata.BaseEntity, true)) {
					string methodName = methodPrefix + propertyInfo.PropertyMetadata.Name;
					Type interfaceType = entityMetadata.PrimaryInterface.InterfaceType;
					Type declaringInterfaceType = entityMetadata.PrimaryInterface.InterfaceType;
					Type propertyType = propertyInfo.PropertyMetadata.PropertyType;
					CheckPropertyLogic(methodName, interfaceType, declaringInterfaceType, propertyType, usePropertyType);
					Type[] parameters = usePropertyType ? new Type[] { propertyType } : new Type[] { };
					MethodLogic methodLogic = logicsSource.GetMethodLogic(interfaceType, declaringInterfaceType, methodName, typeof(void), parameters);
					if(methodLogic != null) {
						result[propertyInfo.PropertyMetadata] = methodLogic;
					}
				}
			}
			else {
				DataMetadata dataMetadata = (DataMetadata)classMetadata;
				implementedInterfaces = dataMetadata.StoredInterfaces;
			}
			foreach(InterfaceMetadata interfaceMetadata in implementedInterfaces) {
				foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
					string methodName = methodPrefix + propertyMetadata.Name;
					Type interfaceType = classMetadata.PrimaryInterface.InterfaceType;
					Type declaringInterfaceType = interfaceMetadata.InterfaceType;
					Type propertyType = propertyMetadata.PropertyType;
					CheckPropertyLogic(methodName, interfaceType, declaringInterfaceType, propertyType, usePropertyType);
					Type[] parameters = usePropertyType ? new Type[] { propertyType } : new Type[] { };
					MethodLogic methodLogic = logicsSource.GetMethodLogic(interfaceType, declaringInterfaceType, methodName, typeof(void), parameters);
					if(methodLogic != null) {
						result[propertyMetadata] = methodLogic;
					}
				}
			}
			return result;
		}
		public Dictionary<PropertyMetadata, MethodLogic> GetObjectChangedMethodsInfo(ClassMetadata classMetadata) {
			return GetObjectMethodsInfo(classMetadata, DcSpecificWords.LogicPrefixAfterChange, false);
		}
		public Dictionary<PropertyMetadata, MethodLogic> GetObjectChangingMethodsInfo(ClassMetadata classMetadata) {
			return GetObjectMethodsInfo(classMetadata, DcSpecificWords.LogicPrefixBeforeChange, true);
		}
		public bool ExistsLogicPropertyInBaseEntities(PropertyMetadata propertyMetadata, EntityMetadata entityMetadata) {
			string propertyName = propertyMetadata.Name;
			foreach(CodeModelPropertyInfo basePropertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(entityMetadata, true)) {
				if(!basePropertyInfo.IsOwnMember && basePropertyInfo.PropertyMetadata.Name == propertyName && basePropertyInfo.PropertyMetadata.IsLogicRequired) {
					return true;
				}
			}
			return false;
		}
		public bool ExistsLogicMethodInBaseEntities(MethodMetadata methodMetadata, EntityMetadata entityMetadata) {
			string methodName = methodMetadata.Name;
			foreach(CodeModelMethodInfo methodInfo in CodeModelGeneratorHelper.GetClassMethodInfos(entityMetadata)) {
				if(!methodInfo.IsOwnMember && methodInfo.MethodMetadata.Name == methodName) {
					return true;
				}
			}
			return false;
		}
		public MethodLogic FindMethodLogic(MethodMetadata methodMetadata, InterfaceMetadata primaryInterface, InterfaceMetadata interfaceMetadata) {
			Type[] paramTypes = GetMethodParameterTypes(methodMetadata);
			MethodLogic methodLogic = logicsSource.GetMethodLogic(primaryInterface.InterfaceType, interfaceMetadata.InterfaceType, methodMetadata.Name, methodMetadata.ReturnType, paramTypes);
			return methodLogic;
		}
		public MethodLogic FindOwnMethodLogic(MethodMetadata methodMetadata, EntityMetadata entityMetadata, InterfaceMetadata interfaceMetadata) {
			Type[] paramTypes = GetMethodParameterTypes(methodMetadata);
			MethodLogic methodLogic = logicsSource.GetMethodLogic(entityMetadata.PrimaryInterface.InterfaceType, interfaceMetadata.InterfaceType, methodMetadata.Name, methodMetadata.ReturnType, paramTypes);
			if(methodLogic != null && entityMetadata.BaseEntity != null && !IsOwnImplementedInterface(interfaceMetadata, entityMetadata)) {
				MethodLogic baseEntityMethodLogic = logicsSource.GetMethodLogic(entityMetadata.BaseEntity.PrimaryInterface.InterfaceType, interfaceMetadata.InterfaceType, methodMetadata.Name, methodMetadata.ReturnType, paramTypes);
				if(baseEntityMethodLogic != null && baseEntityMethodLogic.Owner == methodLogic.Owner) {
					methodLogic = null;
				}
			}
			return methodLogic;
		}
		public IList<MethodLogic> GetClassLogics(string logicMethodName, EntityMetadata entityMetadata) {
			List<MethodLogic> classMethodLogics = new List<MethodLogic>();
			IList<InterfaceMetadata> interfaces = SortInterfacesBaseFirst(entityMetadata.OwnImplementedInterfaces);
			foreach(InterfaceMetadata interfaceMetadata in interfaces) {
				MethodLogic[] methodLogics = logicsSource.GetMethodLogicsForInterface(interfaceMetadata.InterfaceType, logicMethodName, typeof(void), new Type[] { });
				classMethodLogics.AddRange(methodLogics);
			}
			return classMethodLogics.AsReadOnly();
		}
		private IList<InterfaceMetadata> SortInterfacesBaseFirst(IList<InterfaceMetadata> interfaces) {
			List<InterfaceMetadata> result = new List<InterfaceMetadata>(interfaces);
			result.Sort((x, y) => {
				if(x == y) return 0;
				return x.InterfaceType.IsAssignableFrom(y.InterfaceType) ? -1 : 1;
			});
			return result;
		}
		public bool IsLogicRequired(PropertyMetadata propertyMetadata) {
			return IsLogicRequiredCore(propertyMetadata);
		}
		public bool IsLogicRequired(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata) {
			CheckLogicRequiring(propertyMetadata, interfaceMetadata, entityMetadata);
			return IsLogicRequiredCore(propertyMetadata);
		}
		public IList<EntityMetadata> GetCreateInstanceEntities(Type registerInterface) {
			List<EntityMetadata> candidates = new List<EntityMetadata>();
			foreach(EntityMetadata entityMetadata in metadataSource.GetEntitiesMetadata()) {
				Type primaryInterfaceType = entityMetadata.PrimaryInterface.InterfaceType;
				if(registerInterface.IsAssignableFrom(primaryInterfaceType)) {
					candidates.Add(entityMetadata);
				}
			}
			return candidates;
		}
		public MethodLogic FindOwnPropertyLogic(PropertyMetadata propertyMetadata, EntityMetadata entityMetadata, InterfaceMetadata interfaceMetadata, bool isSetter) {
			MethodLogic methodLogic = FindPropertyLogic(propertyMetadata, entityMetadata.PrimaryInterface, interfaceMetadata, isSetter);
			if(methodLogic != null && entityMetadata.BaseEntity != null && !IsOwnImplementedInterface(interfaceMetadata, entityMetadata)) {
				MethodLogic methodLogicInBaseClasses = FindPropertyLogic(propertyMetadata, entityMetadata.BaseEntity.PrimaryInterface, interfaceMetadata, isSetter);
				if(methodLogicInBaseClasses != null) {
					methodLogic = null;
				}
			}
			return methodLogic;
		}
		private MethodLogic FindPropertyLogic(PropertyMetadata propertyMetadata, InterfaceMetadata primaryInterface, InterfaceMetadata interfaceMetadata, bool isSetter) {
			string methodName = (isSetter ? DcSpecificWords.LogicPrefixSetStatic : DcSpecificWords.LogicPrefixGetStatic) + propertyMetadata.Name;
			Type returnType = isSetter ? typeof(void) : propertyMetadata.PropertyType;
			Type[] parameterTypes = isSetter ? new Type[] { propertyMetadata.PropertyType } : new Type[] { };
			MethodLogic methodLogic = logicsSource.GetMethodLogic(primaryInterface.InterfaceType, interfaceMetadata.InterfaceType, methodName, returnType, parameterTypes);
			if(methodLogic == null) {
				methodName = (isSetter ? DcSpecificWords.LogicPrefixSetNonStatic : DcSpecificWords.LogicPrefixGetNonStatic) + propertyMetadata.Name;
				methodLogic = logicsSource.GetMethodLogic(primaryInterface.InterfaceType, interfaceMetadata.InterfaceType, methodName, returnType, parameterTypes);
			}
			return methodLogic;
		}
		public PropertyInfo FindBaseClassProperty(PropertyMetadata propertyMetadata, EntityMetadata entityMetadata) {
			PropertyInfo propertyInfo = null;
			Type baseClass = CodeModelGeneratorHelper.GetBaseClass(entityMetadata);
			if(baseClass != null) {
				Type propertyOwnerType = propertyMetadata.Owner.InterfaceType;
				if(propertyOwnerType.IsAssignableFrom(baseClass)) {  
					propertyInfo = propertyOwnerType.GetProperty(propertyMetadata.Name, propertyMetadata.PropertyType);
				}
				if(propertyInfo == null) {
					propertyInfo = baseClass.GetProperty(propertyMetadata.Name, propertyMetadata.PropertyType);
				}
			}
			return propertyInfo;
		}
		private Type[] GetMethodParameterTypes(MethodMetadata methodMetadata) {
			List<Type> types = new List<Type>();
			foreach(ParameterMetadata parameterMetadata in methodMetadata.Parameters) {
				types.Add(parameterMetadata.Type);
			}
			return types.ToArray();
		}
		public MethodInfo FindBaseClassMethod(EntityMetadata entityMetadata, MethodMetadata methodMetadata) {
			MethodInfo methodInfo = null;
			Type baseClass = CodeModelGeneratorHelper.GetBaseClass(entityMetadata);
			if(baseClass != null) {
				Type[] parameterTypes = GetMethodParameterTypes(methodMetadata);
				if(methodMetadata.Owner.InterfaceType.IsAssignableFrom(baseClass)) {	
					methodInfo = methodMetadata.Owner.InterfaceType.GetMethod(methodMetadata.Name, parameterTypes);
				}
				if(methodInfo == null) {
					methodInfo = baseClass.GetMethod(methodMetadata.Name, parameterTypes);
				}
			}
			return methodInfo;
		}
		private void FillConstructorByMethodLogic(List<Type> allLogicTypes, MethodMetadata methodMetadata, InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata) {
			MethodLogic methodLogic = FindOwnMethodLogic(methodMetadata, entityMetadata, interfaceMetadata);
			FillConstructorLogicTypes(methodLogic, allLogicTypes);
		}
		private void FillConstructorByPropertyLogic(List<Type> allLogicTypes, PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata) {
			if(propertyMetadata.IsLogicRequired) {
				Virtuality virtuality = ExistsLogicPropertyInBaseEntities(propertyMetadata, entityMetadata) ? Virtuality.Override : Virtuality.Virtual;
				InterfaceMetadata declaringInterface = virtuality == Virtuality.Virtual ? interfaceMetadata : entityMetadata.PrimaryInterface;
				MethodLogic getterLogic = FindOwnPropertyLogic(propertyMetadata, entityMetadata, declaringInterface, false);
				MethodLogic setterLogic = FindOwnPropertyLogic(propertyMetadata, entityMetadata, declaringInterface, true);
				FillConstructorLogicTypes(getterLogic, allLogicTypes);
				FillConstructorLogicTypes(setterLogic, allLogicTypes);
			}
		}
		private void FillConstructorLogicTypes(MethodLogic methodLogic, List<Type> logicTypes) {
			if(methodLogic != null) {
				if(!methodLogic.IsStatic && !logicTypes.Contains(methodLogic.Owner)) {
					logicTypes.Add(methodLogic.Owner);
				}
			}
		}
		private bool IsLogicRequiredCore(PropertyMetadata propertyMetadata) {
			if(propertyMetadata.IsLogicRequired) {
				if(propertyMetadata.FindAttribute<ValueConverterAttribute>() != null) {
					return false;
				}
				return !CodeModelGeneratorHelper.HasPersistentAliasAttribute(propertyMetadata);
			}
			return false;
		}
		private PropertyMetadata FindBaseEntityProperty(PropertyMetadata propertyMetadata, EntityMetadata entityMetadata) {
			EntityMetadata baseEntity = entityMetadata.BaseEntity;
			if(baseEntity != null) {
				foreach(InterfaceMetadata interfaceMetadata in baseEntity.OwnImplementedInterfaces) {
					foreach(PropertyMetadata metadata in interfaceMetadata.Properties) {
						if(!metadata.IsLogicRequired && metadata != propertyMetadata && metadata.Name == propertyMetadata.Name) {
							return metadata;
						}
					}
				}
			}
			return null;
		}
		private void CheckLogicRequiring(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata) {
			MethodLogic initMethodLogic = logicsSource.GetMethodLogic(entityMetadata.PrimaryInterface.InterfaceType, interfaceMetadata.InterfaceType, DcSpecificWords.LogicPrefixInit + propertyMetadata.Name, typeof(void), new Type[] { });
			if(initMethodLogic != null) {
				string logicInitExceptionFormat = "The 'Init_' domain logic prefix is obsolete and is no longer supported.\r\nPlease move the {0}.{1} method body into the 'AfterConstruction' domain logic method.";
				throw new InvalidOperationException(string.Format(logicInitExceptionFormat, initMethodLogic.Owner.Name, initMethodLogic.Name));
			}
			MethodLogic getterLogic = FindPropertyLogic(propertyMetadata, entityMetadata.PrimaryInterface, interfaceMetadata, false);
			MethodLogic setterLogic = FindPropertyLogic(propertyMetadata, entityMetadata.PrimaryInterface, interfaceMetadata, true);
			PropertyInfo baseClassPropertyInfo = FindBaseClassProperty(propertyMetadata, entityMetadata);
			PropertyMetadata baseEntityProperty = FindBaseEntityProperty(propertyMetadata, entityMetadata);
			int logicsCount = 0;
			if(getterLogic != null || setterLogic != null) logicsCount++;
			if(baseClassPropertyInfo != null) logicsCount++;
			if(baseEntityProperty != null) logicsCount++;
			if(logicsCount > 1) {
				string methodsString = GetMethodsString(getterLogic, setterLogic, baseClassPropertyInfo, baseEntityProperty);
				string message = string.Format("There are several logics found for {0}.{1} property : {2}", interfaceMetadata.FullName, propertyMetadata.Name, methodsString);
				throw new InvalidOperationException(message);
			}
			if(logicsCount == 1 && !IsLogicRequiredCore(propertyMetadata)) {
				string methodsString = GetMethodsString(getterLogic, setterLogic, baseClassPropertyInfo, baseEntityProperty);
				string message = string.Format("The {0} logic method(s) will not be used for the '{1}.{2}' property because this property is persistent. Either remove the logic method(s) declaration or make the property non-persistent.", methodsString, interfaceMetadata.FullName, propertyMetadata.Name);
				throw new InvalidOperationException(message);
			}
			if(propertyMetadata.IsReadOnly && setterLogic != null) {
				throw new InvalidOperationException(string.Format("DomainLogic '{0}' has method Set_{1}, but property '{2}.{1}' is readonly", setterLogic.Owner.FullName, propertyMetadata.Name, interfaceMetadata.InterfaceType.FullName));
			}
			if(IsLogicRequiredCore(propertyMetadata) && getterLogic == null && baseClassPropertyInfo == null && baseEntityProperty == null) {
				if(interfaceMetadata == entityMetadata.PrimaryInterface) {
					throw new InvalidOperationException(string.Format("Getter logic was not found for property '{0}.{1}'", interfaceMetadata.InterfaceType.FullName, propertyMetadata.Name));
				}
				else {
					throw new InvalidOperationException(
						string.Format("The '{0}' Domain Component registered as the '{1}' entity does not provide getter logic for the '{2}' property of the '{3}' interface.",
						entityMetadata.PrimaryInterface.FullName, entityMetadata.Name, propertyMetadata.Name, interfaceMetadata.InterfaceType.FullName));
				}
			}
			bool isSetterInBaseClass = baseClassPropertyInfo != null && baseClassPropertyInfo.CanWrite;
			bool isSetterInBaseEntity = baseEntityProperty != null && !baseEntityProperty.IsReadOnly;
			if(IsLogicRequiredCore(propertyMetadata) && !propertyMetadata.IsReadOnly && setterLogic == null && !isSetterInBaseClass && !isSetterInBaseEntity) {
				throw new InvalidOperationException(string.Format("Setter logic was not found for property '{0}.{1}'", interfaceMetadata.InterfaceType.FullName, propertyMetadata.Name));
			}
		}
		private string GetMethodsString(MethodLogic getterLogic, MethodLogic setterLogic, PropertyInfo baseClassPropertyInfo, PropertyMetadata baseEntityProperty) {
			List<string> logicMethods = new List<string>();
			if(getterLogic != null) {
				logicMethods.Add(string.Format("'{0}.{1}'", getterLogic.Owner.FullName, getterLogic.Name));
			}
			if(setterLogic != null) {
				logicMethods.Add(string.Format("'{0}.{1}'", setterLogic.Owner.FullName, setterLogic.Name));
			}
			if(baseClassPropertyInfo != null) {
				logicMethods.Add(string.Format("'{0}.{1}'", baseClassPropertyInfo.DeclaringType.FullName, baseClassPropertyInfo.Name));
			}
			if(baseEntityProperty != null) {
				logicMethods.Add(string.Format("'{0}.{1}'", baseEntityProperty.Owner.FullName, baseEntityProperty.Name));
			}
			return string.Join(" and ", logicMethods.ToArray());
		}
		private bool IsOwnImplementedInterface(InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata) {
			foreach(InterfaceMetadata ownInterfaceMetadata in entityMetadata.OwnImplementedInterfaces) {
				if(interfaceMetadata == ownInterfaceMetadata) {
					return true;
				}
			}
			return false;
		}
	}
	internal static class CodeModelGeneratorHelper {
		public static void SynchronizePropertiesAttributes(InterfaceMetadata[] interfaces) {
			foreach(InterfaceMetadata i in interfaces) {
				foreach(PropertyMetadata propertyMetadata in i.Properties) {
					SynchronizeAttributes(propertyMetadata);
				}
			}
		}
		private static void SynchronizeAttributes(PropertyMetadata propertyMetadata) {
			DcAttributesSynchronizerBase attributesSynchronizer = new InterfaceAttributesSynchronizer();
			IList<Attribute> attributes = propertyMetadata.Attributes;
			Attribute xpAttribute;
			for(int iAttribute = 0; iAttribute < attributes.Count; iAttribute++) {
				if(attributesSynchronizer.TryCreateXPAttribute(attributes[iAttribute], out xpAttribute)) {
					attributes[iAttribute] = xpAttribute;
				}
			}
		}
		public static FieldCode FindFieldCode(ClassCode classCode, string name) {
			foreach(FieldCode fieldCode in classCode.Fields) {
				if(fieldCode.Name == name) {
					return fieldCode;
				}
			}
			return null;
		}
		public static IEnumerable<Attribute> GetClassAttributes(ClassMetadata classMetadata) {
			Dictionary<Type, Attribute> attributes = new Dictionary<Type, Attribute>();
			InterfaceMetadata interfaceMetadata = classMetadata.PrimaryInterface;
			foreach(Attribute attribute in interfaceMetadata.Attributes) {
				if(!attributes.ContainsKey(attribute.GetType())) {
					attributes[attribute.GetType()] = attribute;
				}
			}
			return attributes.Values;
		}
		public static Type GetAssociatedType(PropertyMetadata propertyMetadata) {
			if(IsListType(propertyMetadata)) {
				return propertyMetadata.PropertyType.GetGenericArguments()[0];
			}
			return propertyMetadata.PropertyType;
		}
		public static bool IsListType(PropertyMetadata propertyMetadata) {
			return propertyMetadata.PropertyType.IsGenericType && propertyMetadata.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
		}
		public static string GetFieldName(string propertyName) {
			return DcSpecificWords.FieldPropertyPrefix + propertyName;
		}
		public static string GetEntityDataClassName(DataMetadata dataMetadata, string entityName) {
			return string.Format("{0}_{1}", dataMetadata.Name, entityName);
		}
		public static string GetLinksPropertyName(PropertyMetadata propertyMetadata) {
			if(propertyMetadata.AssociationInfo != null) {
				return string.Format("{0}_{1}{2}", propertyMetadata.AssociationInfo.Name, propertyMetadata.Name, DcSpecificWords.LinksPostfix);
			}
			return null;
		}
		public static string GetLinkClassLinkPropertyName(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			if(propertyMetadata.AssociationInfo != null) {
				return string.Format("{0}_{1}{2}", interfaceMetadata.InterfaceType.Name, propertyMetadata.Name, DcSpecificWords.LinkedPostfix);
			}
			return null;
		}
		public static string GetLinkPropertyAssociationName(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			if(propertyMetadata.AssociationInfo != null) {
				return string.Format("{0}_{1}", interfaceMetadata.InterfaceType.FullName, propertyMetadata.Name);
			}
			return null;
		}
		public static string GetAliasPropertyName(PropertyMetadata propertyMetadata) {
			return GetAliasPropertyName(propertyMetadata.Name);
		}
		public static string GetAliasPropertyName(string propertyName) {
			return DcSpecificWords.PropertyPersistentAliasPrefix + propertyName;
		}
		public static string GetDataClassFieldName(DataMetadata dataMetadata) {
			return dataMetadata.Name;
		}
		public static T FindAttribute<T>(IEnumerable<Attribute> attributes) where T : Attribute {
			foreach(Attribute attribute in attributes) {
				if(attribute is T) {
					return (T)attribute;
				}
			}
			return null;
		}
		public static bool HasPersistentAliasAttribute(PropertyMetadata propertyMetadata) {
			bool hasAttribute = propertyMetadata.FindAttribute<PersistentAliasAttribute>() != null;
			if(hasAttribute && !propertyMetadata.IsReadOnly) {
				throw new InvalidOperationException(string.Format("The {0}.{1} aliased property exposes a setter", propertyMetadata.Owner.InterfaceType.Name, propertyMetadata.Name));
			}
			return hasAttribute;
		}
		public static List<CodeModelMethodInfo> GetClassMethodInfos(EntityMetadata entityMetadata) {
			return GetClassMethodInfos(entityMetadata, true);
		}
		private static List<CodeModelMethodInfo> GetClassMethodInfos(EntityMetadata entityMetadata, bool isOwnMember) {
			List<CodeModelMethodInfo> result = new List<CodeModelMethodInfo>();
			if(entityMetadata != null) {
				foreach(InterfaceMetadata interfaceMetadata in entityMetadata.OwnImplementedInterfaces) {
					foreach(MethodMetadata methodMetadata in interfaceMetadata.Methods) {
						result.Add(new CodeModelMethodInfo(methodMetadata, interfaceMetadata, entityMetadata.PrimaryInterface, isOwnMember));
					}
				}
				result.AddRange(GetClassMethodInfos(entityMetadata.BaseEntity, false));
			}
			return result;
		}
		public static List<CodeModelPropertyInfo> GetClassPropertyInfos(EntityMetadata entityMetadata, bool collectBaseProperties) {
			List<CodeModelPropertyInfo> allPropertyInfos = GetClassPropertyInfos(entityMetadata, true, collectBaseProperties);
			return GetFilteredPropertyInfos(allPropertyInfos, true);
		}
		public static List<CodeModelPropertyInfo> GetDataClassStoredPropertyInfos(DataMetadata dataMetadata) {
			List<CodeModelPropertyInfo> storedPropertyInfos = new List<CodeModelPropertyInfo>();
			foreach(InterfaceMetadata interfaceMetadata in dataMetadata.StoredInterfaces) {
				foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
					storedPropertyInfos.Add(new CodeModelPropertyInfo(propertyMetadata, interfaceMetadata, dataMetadata.PrimaryInterface, null, true));
				}
			}
			foreach(DataMetadata aggregatedDataMetadata in dataMetadata.AggregatedData) {
				foreach(InterfaceMetadata interfaceMetadata in aggregatedDataMetadata.StoredInterfaces) {
					foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
						storedPropertyInfos.Add(new CodeModelPropertyInfo(propertyMetadata, interfaceMetadata, aggregatedDataMetadata.PrimaryInterface, aggregatedDataMetadata, true));
					}
				}
			}
			return GetFilteredPropertyInfos(storedPropertyInfos, false);
		}
		private static IEnumerable<CodeModelPropertyInfo> CollectExplicitPropertyInfos(List<CodeModelPropertyInfo> propertiesWithSameName, Type persistentPropertyType) {
			foreach(CodeModelPropertyInfo propertyInfo in propertiesWithSameName) {
				bool isDifferentPropertyTypes = persistentPropertyType != propertyInfo.PropertyMetadata.PropertyType;
				bool isNonPersistent = propertyInfo.PropertyMetadata.IsLogicRequired;
				if(isNonPersistent && isDifferentPropertyTypes) {
					propertyInfo.IsExplicit = true;
					yield return propertyInfo;
				}
			}
		}
		private static List<CodeModelPropertyInfo> GetFilteredPropertyInfos(List<CodeModelPropertyInfo> allProperties, bool isEntityClass) {
			Dictionary<string, List<CodeModelPropertyInfo>> propertyInfosByName = new Dictionary<string, List<CodeModelPropertyInfo>>();
			foreach(CodeModelPropertyInfo propertyinfo in allProperties) {
				List<CodeModelPropertyInfo> items;
				if(!propertyInfosByName.TryGetValue(propertyinfo.PropertyMetadata.Name, out items)) {
					items = new List<CodeModelPropertyInfo>();
					propertyInfosByName[propertyinfo.PropertyMetadata.Name] = items;
				}
				items.Add(propertyinfo);
			}
			List<CodeModelPropertyInfo> result = new List<CodeModelPropertyInfo>();
			foreach(KeyValuePair<string, List<CodeModelPropertyInfo>> propertyInfos in propertyInfosByName) {
				if(propertyInfos.Value.Count > 1) {
					CodeModelPropertyInfo persistentPropertyInfo = null;
					foreach(CodeModelPropertyInfo propertyInfo in propertyInfos.Value) {
						if(propertyInfo.InterfaceMetadata.IsPersistent) {
							if(persistentPropertyInfo == null) {
								persistentPropertyInfo = propertyInfo;
							}
							else if(persistentPropertyInfo.PropertyMetadata.IsLogicRequired) {
								persistentPropertyInfo = propertyInfo;
							}
							else if(!propertyInfo.PropertyMetadata.IsLogicRequired) {
								throw new InvalidOperationException(string.Format("There are persistent properties with same name: {0}.{1} and {2}.{1}", persistentPropertyInfo.InterfaceMetadata.FullName, propertyInfos.Key, propertyInfo.InterfaceMetadata.FullName));
							}
						}
					}
					if(persistentPropertyInfo == null) {
						foreach(CodeModelPropertyInfo propertyInfo in propertyInfos.Value) {
							if(propertyInfo.InterfaceMetadata.IsDomainComponent) {
								if(persistentPropertyInfo == null) {
									persistentPropertyInfo = propertyInfo;
								}
								else if(persistentPropertyInfo.PropertyMetadata.IsLogicRequired) {
									persistentPropertyInfo = propertyInfo;
								}
							}
						}
					}
					if(persistentPropertyInfo == null) {
						persistentPropertyInfo = propertyInfos.Value[0];
					}
					result.Add(persistentPropertyInfo);
					if(isEntityClass) {
						result.AddRange(CollectExplicitPropertyInfos(propertyInfos.Value, persistentPropertyInfo.PropertyMetadata.PropertyType));
					}
				}
				else {
					result.Add(propertyInfos.Value[0]);
				}
			}
			return result;
		}
		private static List<CodeModelPropertyInfo> GetClassPropertyInfos(EntityMetadata entityMetadata, bool isOwnMember, bool collectBaseProperties) {
			List<CodeModelPropertyInfo> result = new List<CodeModelPropertyInfo>();
			if(entityMetadata != null) {
				foreach(InterfaceMetadata interfaceMetadata in entityMetadata.OwnImplementedInterfaces) {
					foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
						result.Add(new CodeModelPropertyInfo(propertyMetadata, interfaceMetadata, entityMetadata.PrimaryInterface, null, isOwnMember));
					}
				}
				if(collectBaseProperties) {
					result.AddRange(GetClassPropertyInfos(entityMetadata.BaseEntity, false, true));
				}
			}
			return result;
		}
		public static Type GetBaseClass(EntityMetadata entityMetadata) {
			EntityMetadata currentEntityMetadata = entityMetadata;
			while(currentEntityMetadata.BaseEntity != null) {
				currentEntityMetadata = currentEntityMetadata.BaseEntity;
			}
			return currentEntityMetadata.BaseClass;
		}
	}
	internal class CodeModelMemberInfoBase {
		InterfaceMetadata interfaceMetadata;
		InterfaceMetadata primaryInterface;
		bool isOwnMember = true;
		public CodeModelMemberInfoBase(InterfaceMetadata interfaceMetadata, InterfaceMetadata primaryInterface, bool isOwnMember) {
			this.interfaceMetadata = interfaceMetadata;
			this.primaryInterface = primaryInterface;
			this.isOwnMember = isOwnMember;
		}
		public InterfaceMetadata InterfaceMetadata {
			get {
				return interfaceMetadata;
			}
		}
		public InterfaceMetadata PrimaryInterface {
			get {
				return primaryInterface;
			}
		}
		public bool IsOwnMember {
			get {
				return isOwnMember;
			}
		}
	}
	internal class CodeModelPropertyInfo : CodeModelMemberInfoBase {
		private PropertyMetadata propertyMetadata;
		private bool isExplicit;
		private DataMetadata dataMetadata;
		public CodeModelPropertyInfo(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata, InterfaceMetadata primaryInterface, DataMetadata dataMetadata, bool isOwnMember)
			: base(interfaceMetadata, primaryInterface, isOwnMember) {
			this.propertyMetadata = propertyMetadata;
			this.dataMetadata = dataMetadata;
			this.isExplicit = false;
		}
		public PropertyMetadata PropertyMetadata {
			get {
				return propertyMetadata;
			}
		}
		public bool IsExplicit {
			get {
				return isExplicit;
			}
			set {
				isExplicit = value;
			}
		}
		public DataMetadata DataMetadata {
			get {
				return dataMetadata;
			}
		}
	}
	internal class CodeModelMethodInfo : CodeModelMemberInfoBase {
		MethodMetadata methodMetadata;
		public CodeModelMethodInfo(MethodMetadata methodMetadata, InterfaceMetadata interfaceMetadata, InterfaceMetadata primaryInterface, bool isOwnMember)
			: base(interfaceMetadata, primaryInterface, isOwnMember) {
			this.methodMetadata = methodMetadata;
		}
		public MethodMetadata MethodMetadata {
			get {
				return methodMetadata;
			}
		}
	}
	internal abstract class DcAttributesSynchronizerBase {
		private Dictionary<Type, Type> dcToXpAttributeMapping;
		private void SynchronizeWithDcAttributesCore(IBaseInfo info, XPTypeInfo xpInfo) {
			foreach(Type dcAttributeType in dcToXpAttributeMapping.Keys) {
				BaseInfo baseInfo = info as BaseInfo;
				if(baseInfo != null) {
					Attribute dcAttribute = baseInfo.FindAttribute(dcAttributeType);
					if(dcAttribute != null && !xpInfo.HasAttribute(dcToXpAttributeMapping[dcAttributeType])) {
						xpInfo.AddAttribute(CreateXPAttributeCore(dcAttribute));
					}
				}
			}
		}
		protected void AddAttributeMapping(Type dcAttibuteType, Type xpAttibuteType) {
			dcToXpAttributeMapping.Add(dcAttibuteType, xpAttibuteType);
		}
		protected abstract void Initialization();
		protected virtual Attribute CreateXPAttributeCore(Attribute dcAttribute) {
			return (Attribute)TypeHelper.CreateInstance(dcToXpAttributeMapping[dcAttribute.GetType()]);
		}
		public DcAttributesSynchronizerBase() {
			dcToXpAttributeMapping = new Dictionary<Type, Type>();
			Initialization();
		}
		public bool TryCreateXPAttribute(Attribute dcAttribute, out Attribute xpAttribute) {
			xpAttribute = null;
			if(dcToXpAttributeMapping.ContainsKey(dcAttribute.GetType())) {
				xpAttribute = CreateXPAttributeCore(dcAttribute);
				return true;
			}
			return false;
		}
		public void UpdateXPInfo(ITypeInfo info, XPClassInfo xpInfo) {
			SynchronizeWithDcAttributesCore(info, xpInfo);
			foreach(IMemberInfo memberInfo in info.Members) {
				XPMemberInfo memberXPInfo = xpInfo.FindMember(memberInfo.Name);
				if(memberXPInfo != null) {
					SynchronizeWithDcAttributesCore(memberInfo, memberXPInfo);
				}
			}
		}
	}
	internal class InterfaceAttributesSynchronizer : DcAttributesSynchronizerBase {
		protected override void Initialization() {
			AddAttributeMapping(typeof(PersistentDcAttribute), typeof(DevExpress.Xpo.PersistentAttribute));
			AddAttributeMapping(typeof(NonPersistentDcAttribute), typeof(DevExpress.Xpo.NonPersistentAttribute));
			AddAttributeMapping(typeof(AggregatedAttribute), typeof(DevExpress.Xpo.AggregatedAttribute));
			AddAttributeMapping(typeof(FieldSizeAttribute), typeof(DevExpress.Xpo.SizeAttribute));
			AddAttributeMapping(typeof(CalculatedAttribute), typeof(DevExpress.Xpo.PersistentAliasAttribute));
		}
		protected override Attribute CreateXPAttributeCore(Attribute dcAttribute) {
			if(dcAttribute is FieldSizeAttribute) {
				return new DevExpress.Xpo.SizeAttribute((dcAttribute as FieldSizeAttribute).Size);
			}
			if(dcAttribute is CalculatedAttribute) {
				return new DevExpress.Xpo.PersistentAliasAttribute((dcAttribute as CalculatedAttribute).Expression);
			}
			return base.CreateXPAttributeCore(dcAttribute);
		}
	}
}
