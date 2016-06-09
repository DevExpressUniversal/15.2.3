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
namespace DevExpress.ExpressApp.DC {
	sealed class InterfaceAssociationInfo {
		private readonly ITypesInfo typesInfo;
		private readonly Dictionary<Type, object> implicitAssociationCollected;
		private readonly Dictionary<Type, object> expicitAssociationCollected;
		private readonly Dictionary<PropertyInfo, PropertyInfo> oneToManyAssociations;
		private readonly Dictionary<PropertyInfo, PropertyInfo> manyToManyAssociations;
		private bool IsDomainComponent(ITypeInfo typeInfo) {
			return typeInfo.FindAttribute<DomainComponentAttribute>() != null;
		}
		private bool IsInAssociation(PropertyInfo property) {
			return oneToManyAssociations.ContainsKey(property) || manyToManyAssociations.ContainsKey(property);
		}
		private PropertyInfo GetAssociatedProperty(PropertyInfo property) {
			PropertyInfo associatedProperty;
			if(oneToManyAssociations.TryGetValue(property, out associatedProperty)) {
				return associatedProperty;
			}
			else if(manyToManyAssociations.TryGetValue(property, out associatedProperty)) {
				return associatedProperty;
			}
			else {
				throw new ArgumentException();
			}
		}
		private void AddOneToManyAssociation(PropertyInfo leftProperty, PropertyInfo rightProperty) {
			oneToManyAssociations.Add(leftProperty, rightProperty);
			oneToManyAssociations.Add(rightProperty, leftProperty);
		}
		private void AddManyToManyAssociation(PropertyInfo leftProperty, PropertyInfo rightProperty) {
			manyToManyAssociations.Add(leftProperty, rightProperty);
			manyToManyAssociations.Add(rightProperty, leftProperty);
		}
		private bool IsAssociatedPropertyCandidate(PropertyInfo property, out bool isCollection, out Type associatedPropertyOwner) {
			isCollection = false;
			associatedPropertyOwner = null;
			if(IsInAssociation(property)) return false;
			if(property.IsDefined(typeof(NonPersistentDcAttribute), false)) return false;   
			if(property.IsDefined(typeof(IgnoreForAssociationAttribute), false)) return false;
			Type propertyType = property.PropertyType;
			if(propertyType.IsGenericType) {
				if(property.CanWrite) return false;
				if(propertyType.GetGenericTypeDefinition() != typeof(IList<>)) return false;
				Type genericArgumentType = propertyType.GetGenericArguments()[0];
				if(!genericArgumentType.IsInterface) return false;
				ITypeInfo genericArgumentTypeInfo = typesInfo.FindTypeInfo(genericArgumentType);
				if(IsDomainComponent(genericArgumentTypeInfo)) {
					isCollection = true;
					associatedPropertyOwner = genericArgumentType;
					return true;
				}
			}
			else {
				if(!propertyType.IsInterface) return false;
				ITypeInfo propertyTypeInfo = typesInfo.FindTypeInfo(propertyType);
				if(IsDomainComponent(propertyTypeInfo)) {
					isCollection = false;
					associatedPropertyOwner = propertyType;
					return true;
				}
			}
			return false;
		}
		private void ThrowMalformedAssociationException(PropertyInfo property, Type interfaceType) {
			throw new InvalidOperationException(string.Format("Malformed association. Cannot find the associated member for '{0}.{1}' in interface '{2}'.", property.DeclaringType.FullName, property.Name, interfaceType.FullName));
		}
		private void ThrowAmbiguousAssociationException(Type interfaceType, PropertyInfo[] properties) {
			string[] propertyNames = new string[properties.Length];
			for(int i = 0; i < properties.Length; i++) {
				propertyNames[i] = properties[i].Name;
			}
			throw new InvalidOperationException(string.Format("Association is ambiguous between the members {1} of interface '{0}'.", interfaceType.FullName, string.Join(", ", propertyNames)));
		}
		private void CollectExplicitAssociations(Type interfaceType) {
			if(interfaceType == null || !interfaceType.IsInterface || expicitAssociationCollected.ContainsKey(interfaceType)) return;
			expicitAssociationCollected.Add(interfaceType, null);
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceType);
			if(!IsDomainComponent(interfaceTypeInfo)) return;
			foreach(PropertyInfo leftProperty in interfaceType.GetProperties()) {
				if(IsInAssociation(leftProperty)) continue;
				bool isLeftPropertyCollection;
				Type rightPropertyOwner;
				if(IsAssociatedPropertyCandidate(leftProperty, out isLeftPropertyCollection, out rightPropertyOwner)) {
					object[] leftPropertyBackRefAttributes = leftProperty.GetCustomAttributes(typeof(BackReferencePropertyAttribute), false);
					if(leftPropertyBackRefAttributes.Length > 0) {
						string rightPropertyName = ((BackReferencePropertyAttribute)leftPropertyBackRefAttributes[0]).PropertyName;
						PropertyInfo rightProperty = rightPropertyOwner.GetProperty(rightPropertyName);
						if(rightProperty == null) {
							ThrowMalformedAssociationException(leftProperty, rightPropertyOwner);
						}
						if(IsInAssociation(rightProperty)) {
							ThrowAmbiguousAssociationException(interfaceType, new PropertyInfo[] { leftProperty, GetAssociatedProperty(rightProperty) });
						}
						object[] rightPropertyBackRefAttributes = rightProperty.GetCustomAttributes(typeof(BackReferencePropertyAttribute), false);
						if(rightPropertyBackRefAttributes.Length > 0) {
							string leftPropertyName = ((BackReferencePropertyAttribute)rightPropertyBackRefAttributes[0]).PropertyName;
							if(leftPropertyName != leftProperty.Name) {
								throw new InvalidOperationException(string.Format(
									"Malformed association. The BackReferenceProperty attribute applied to the {2}.{3} property does not point to the {0}.{1} property. While the BackReferenceProperty attribute applied to the {0}.{1} property points to the {2}.{3} property.",
									interfaceType.FullName, leftProperty.Name, rightPropertyOwner.FullName, rightProperty.Name
								));
							}
						}
						bool isRightPropertyCollection;
						Type leftPropertyOwner;
						if(IsAssociatedPropertyCandidate(rightProperty, out isRightPropertyCollection, out leftPropertyOwner)) {
							if(leftPropertyOwner != interfaceType) {
								ThrowMalformedAssociationException(leftProperty, rightPropertyOwner);
							}
							if(isLeftPropertyCollection && isRightPropertyCollection) {
								AddManyToManyAssociation(leftProperty, rightProperty);
							}
							else {
								AddOneToManyAssociation(rightProperty, leftProperty);
							}
						}
						else {
							ThrowMalformedAssociationException(leftProperty, rightPropertyOwner);
						}
					}
				}
			}
		}
		private void CollectImpicitAssociations(Type interfaceType) {
			if(interfaceType == null || !interfaceType.IsInterface || implicitAssociationCollected.ContainsKey(interfaceType)) return;
			implicitAssociationCollected.Add(interfaceType, null);
			ITypeInfo interfaceTypeInfo = typesInfo.FindTypeInfo(interfaceType);
			if(!IsDomainComponent(interfaceTypeInfo)) return;
			PropertyInfo[] properties = interfaceType.GetProperties();
			foreach(PropertyInfo leftProperty in properties) {
				if(IsInAssociation(leftProperty)) continue;
				bool isLeftPropertyCollection;
				Type rightPropertyOwner;
				if(IsAssociatedPropertyCandidate(leftProperty, out isLeftPropertyCollection, out rightPropertyOwner)) {
					CollectExplicitAssociations(rightPropertyOwner);
					if(IsInAssociation(leftProperty)) continue;
					bool isRightPropertyCollection = true;
					Type leftPropertyOwner = null; ;
					List<PropertyInfo> associatedPropertyCandidates = new List<PropertyInfo>();
					PropertyInfo[] rightPropertyOwnerProperties = rightPropertyOwner.GetProperties();
					foreach(PropertyInfo property in rightPropertyOwnerProperties) {
						if(property == leftProperty || IsInAssociation(property)) continue;
						bool isPropertyCollection;
						Type associatedPropertyOwner;
						if(IsAssociatedPropertyCandidate(property, out isPropertyCollection, out associatedPropertyOwner)) {
							if(!isLeftPropertyCollection && !isPropertyCollection) continue;
							if(associatedPropertyOwner == interfaceType) {
								isRightPropertyCollection = isPropertyCollection;
								leftPropertyOwner = associatedPropertyOwner;
								associatedPropertyCandidates.Add(property);
							}
						}
					}
					if(associatedPropertyCandidates.Count > 1) {
						ThrowAmbiguousAssociationException(rightPropertyOwner, associatedPropertyCandidates.ToArray());
					}
					if(associatedPropertyCandidates.Count == 1) {
						PropertyInfo rightProperty = associatedPropertyCandidates[0];
						foreach(PropertyInfo property in properties) {
							if(property == leftProperty || property == rightProperty || IsInAssociation(property)) continue;
							if(property.PropertyType == leftProperty.PropertyType) {
								ThrowAmbiguousAssociationException(interfaceType, new PropertyInfo[] { leftProperty, property });
							}
						}
						if(isLeftPropertyCollection && isRightPropertyCollection) {
							AddManyToManyAssociation(leftProperty, rightProperty);
						}
						else {
							AddOneToManyAssociation(rightProperty, leftProperty);
						}
					}
				}
			}
		}
		private void CollectAssociations(Type interfaceType) {
			CollectExplicitAssociations(interfaceType);
			CollectImpicitAssociations(interfaceType);
		}
		public InterfaceAssociationInfo(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
			implicitAssociationCollected = new Dictionary<Type, object>();
			expicitAssociationCollected = new Dictionary<Type, object>();
			oneToManyAssociations = new Dictionary<PropertyInfo, PropertyInfo>();
			manyToManyAssociations = new Dictionary<PropertyInfo, PropertyInfo>();
		}
		public PropertyInfo FindAssociatedProperty(PropertyInfo property, out Boolean isManyToMany) {
			CollectAssociations(property.DeclaringType);
			PropertyInfo result;
			if(oneToManyAssociations.TryGetValue(property, out result)) {
				isManyToMany = false;
				return result;
			}
			if(manyToManyAssociations.TryGetValue(property, out result)) {
				isManyToMany = true;
				return result;
			}
			isManyToMany = false;
			return null;
		}
	}
}
