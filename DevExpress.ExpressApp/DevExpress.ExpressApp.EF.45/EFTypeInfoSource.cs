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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF.Utils;
namespace DevExpress.ExpressApp.EF {
	public class EFTypeInfoSource : ReflectionTypeInfoSource, ITypeInfoSource, IEntityStore, IDisposable {
		private ITypesInfo typesInfo;
		private Type contextType;
		private MetadataWorkspace metadataWorkspace;
		private List<Type> registeredEntities;
		private Dictionary<String, EntitySet> entitySets;
		private Dictionary<String, EntityType> entityTypes;
		private Dictionary<ITypeInfo, ITypeInfo> metadataTypeInfoMap;
		private XafTypeDescriptionProvider xafTypeDescriptionProvider;
		private Dictionary<Type, Object> typesWithCalculatedMembers;
		private void RemoveCustomTypeDescriptionProvider() {
			foreach(Type type in typesWithCalculatedMembers.Keys) {
				TypeDescriptor.RemoveProvider(xafTypeDescriptionProvider, type);
			}
			typesWithCalculatedMembers.Clear();
		}
		private EntityType FindEntityType(Type type) {
			EntityType entityType = null;
			if(entityTypes.TryGetValue(type.FullName, out entityType)) {
				return entityType;
			}
			else {
				return null;
			}
		}
		private EntitySet FindEntitySet(Type type) {
			EntitySet entitySet = null;
			if(entitySets.TryGetValue(type.Name, out entitySet)) {
				return entitySet;
			}
			else {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				if((typeInfo != null) && (typeInfo.Base != null)) {
					return FindEntitySet(typeInfo.Base.Type);
				}
				else {
					return null;
				}
			}
		}
		private EdmMember FindAssociatedEdmMember(NavigationProperty sourceEdmMember, EntityType associatedEntityType) {
			foreach(NavigationProperty edmMember in associatedEntityType.NavigationProperties) {
				if(edmMember.RelationshipType == sourceEdmMember.RelationshipType) {
					return edmMember;
				}
			}
			return null;
		}
		private void InitNavigationMemberInfo(NavigationProperty edmMember, XafMemberInfo xafMemberInfo) {
			TypeInfo associatedTypeInfo = null;
			if(xafMemberInfo.IsList) {
				associatedTypeInfo = xafMemberInfo.ListElementTypeInfo;
			}
			else {
				associatedTypeInfo = xafMemberInfo.MemberTypeInfo;
			}
			NavigationProperty navigationEdmMember = (NavigationProperty)edmMember;
			EntityType associatedEntityType = FindEntityType(associatedTypeInfo.Type);
			EdmMember associatedEdmMember = FindAssociatedEdmMember(navigationEdmMember, associatedEntityType);
			if(associatedEdmMember != null) {
				xafMemberInfo.IsAssociation = true;
				xafMemberInfo.AssociatedMemberInfo = associatedTypeInfo.FindMember(associatedEdmMember.Name);
				xafMemberInfo.AssociatedMemberName = xafMemberInfo.AssociatedMemberInfo.Name;
				xafMemberInfo.AssociatedMemberOwner = associatedTypeInfo.Type;
				xafMemberInfo.IsManyToMany =
					(navigationEdmMember.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
					&&
					(navigationEdmMember.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many);
				if(xafMemberInfo.IsAggregated && xafMemberInfo.IsList && !xafMemberInfo.IsManyToMany && (xafMemberInfo.AssociatedMemberInfo != null)) {
					xafMemberInfo.AssociatedMemberInfo.IsReferenceToOwner = true;
				}
			}
			EdmRelationshipNavigationPropertyAttribute attribute = xafMemberInfo.FindAttribute<EdmRelationshipNavigationPropertyAttribute>();
			xafMemberInfo.AddExtender<EdmRelationshipNavigationPropertyAttribute>(attribute);
		}
		private void InitComplexTypeMemberInfo(EdmProperty edmProperty, XafMemberInfo xafMemberInfo) {
			xafMemberInfo.IsPersistent = true;
			xafMemberInfo.MemberTypeInfo.IsPersistent = false;
			xafMemberInfo.MemberTypeInfo.IsDomainComponent = true;
			xafMemberInfo.MemberTypeInfo.AddExtender<ComplexType>(edmProperty.ComplexType);
			foreach(XafMemberInfo xafMemberInfo2 in xafMemberInfo.MemberTypeInfo.OwnMembers) {
				xafMemberInfo2.IsPersistent = true;
			}
		}
		private void CopyAttributesFromMetadataType(XafMemberInfo xafMemberInfo) {
			ITypeInfo metadataTypeInfo = null;
			if(metadataTypeInfoMap.TryGetValue(xafMemberInfo.Owner, out metadataTypeInfo)) {
				IMemberInfo metadataXafMemberInfo = metadataTypeInfo.FindMember(xafMemberInfo.Name);
				try {
					CopyAttributes(metadataXafMemberInfo, xafMemberInfo);
				}
				catch {
				}
			}
		}
		private void RealInitTypeInfo(TypeInfo typeInfo) {
			if(typeInfo.Type == typeof(EntityObject)) {
				typeInfo.IsPersistent = false;
				typeInfo.IsVisible = false;
			}
			else {
				typeInfo.IsPersistent = true;
				typeInfo.IsDomainComponent = true;
			}
			typeInfo.IsVisible = CalcTypeVisibility(typeInfo);
			if(!metadataTypeInfoMap.ContainsKey(typeInfo)) {
				MetadataTypeAttribute metadataTypeAttribute = typeInfo.FindAttribute<MetadataTypeAttribute>(false);
				if(metadataTypeAttribute != null) {
					ITypeInfo metadataTypeInfo = typesInfo.FindTypeInfo(metadataTypeAttribute.MetadataClassType);
					if(metadataTypeInfo != null) {
						metadataTypeInfoMap[typeInfo] = metadataTypeInfo;
					}
				}
			}
		}
		private void RealInitMemberInfo(EdmMember edmMember, XafMemberInfo xafMemberInfo) {
			CopyAttributesFromMetadataType(xafMemberInfo);
			xafMemberInfo.IsVisible = CalcMemberVisibility(xafMemberInfo);
			xafMemberInfo.IsDelayed = (xafMemberInfo.FindAttribute<DelayedAttribute>(false) != null);
			HandleIsAggregatedAttribute(xafMemberInfo);
			HandleCalculatedAttribute(xafMemberInfo);
			xafMemberInfo.IsPersistent = (edmMember != null);
			if(edmMember != null) {
				EntityType entityType = (EntityType)edmMember.DeclaringType;
				if((entityType.KeyMembers.Count > 0) && (entityType.KeyMembers.IndexOf(edmMember) >= 0)) {
					xafMemberInfo.IsKey = true;
					if(!xafMemberInfo.Owner.KeyMembers.Contains(xafMemberInfo)) {
						xafMemberInfo.Owner.AddKeyMember(xafMemberInfo);
					}
				}
				if(edmMember is EdmProperty) {
					EdmProperty edmProperty = (EdmProperty)edmMember;
					MetadataProperty metadataProperty = edmProperty.MetadataProperties.FirstOrDefault((mp) => mp.Name.Contains("StoreGeneratedPattern"));
					if((metadataProperty != null)
						&&
						(metadataProperty.Value != null)
						&&
						((metadataProperty.Value.ToString() == "Identity") || (metadataProperty.Value.ToString() == "Computed"))
					) {
						xafMemberInfo.IsAutoGenerate = true;
					}
					if(edmProperty.MaxLength.HasValue) {
						xafMemberInfo.Size = edmProperty.MaxLength.Value;
						xafMemberInfo.ValueMaxLength = edmProperty.MaxLength.Value;
					}
					if(edmProperty.IsComplexType && (edmProperty.ComplexType != null)) {
						InitComplexTypeMemberInfo(edmProperty, xafMemberInfo);
					}
				}
				if(edmMember is NavigationProperty) {
					InitNavigationMemberInfo((NavigationProperty)edmMember, xafMemberInfo);
				}
			}
			HandleFieldSizeAttribute(xafMemberInfo);
		}
		private void SpecialInitForBaseTypeInfo(TypeInfo typeInfo) {
			EntityType entityType = FindEntityType(typeInfo.Type);
			if(entityType != null) {
				foreach(EdmMember edmMember in entityType.Members) {
					XafMemberInfo xafMemberInfo = typeInfo.FindOwnMember(edmMember.Name);
					if(xafMemberInfo == null) {
						xafMemberInfo = typeInfo.FindMember(edmMember.Name);
						if(xafMemberInfo != null) {
							RealInitTypeInfo(xafMemberInfo.Owner);
							RealInitMemberInfo(edmMember, xafMemberInfo);
						}
					}
				}
			}
		}
		public EFTypeInfoSource(ITypesInfo typesInfo, Type contextType, MetadataWorkspace metadataWorkspace) {
			this.typesInfo = typesInfo;
			this.contextType = contextType;
			this.metadataWorkspace = metadataWorkspace;
			registeredEntities = new List<Type>();
			entitySets = new Dictionary<String, EntitySet>();
			entityTypes = new Dictionary<String, EntityType>();
			metadataTypeInfoMap = new Dictionary<ITypeInfo, ITypeInfo>();
			xafTypeDescriptionProvider = new XafTypeDescriptionProvider(typesInfo);
			typesWithCalculatedMembers = new Dictionary<Type, Object>();
			if(typeof(ObjectContext).IsAssignableFrom(contextType)) {
				foreach(EntityType entityType in metadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)) {
					entityTypes.Add(contextType.Namespace + "." + entityType.Name, entityType);
				}
			}
			else {
				foreach(EntityType entityType in metadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)) {
					entityTypes.Add(metadataWorkspace.GetObjectSpaceType(entityType).FullName, entityType);
				}
			}
			foreach(EntityContainer entityContainer in metadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace)) {
				foreach(EntitySetBase entitySetBase in entityContainer.BaseEntitySets) {
					if(entitySetBase.BuiltInTypeKind == BuiltInTypeKind.EntitySet) {
						EntitySet entitySet = (EntitySet)entitySetBase;
						entitySets.Add(entitySet.ElementType.Name, entitySet);
					}
				}
			}
		}
		public String GetEntitySetName(Type type) {
			EntitySet entitySet = FindEntitySet(type);
			if(entitySet == null) {
				throw new Exception(String.Format("The entity set for the '{0}' type is not found.", type.FullName));
			}
			return entitySet.Name;
		}
		public Type ContextType {
			get { return contextType; }
		}
		public override Boolean TypeIsKnown(Type type) {
			return (type == typeof(EntityObject)) || (FindEntityType(type) != null);
		}
		public override void InitTypeInfo(TypeInfo typeInfo) {
			base.InitTypeInfo(typeInfo);
			if(TypeIsKnown(typeInfo.Type)) {
				RealInitTypeInfo(typeInfo);
			}
		}
		public override void InitMemberInfo(Object member, XafMemberInfo xafMemberInfo) {
			base.InitMemberInfo(member, xafMemberInfo);
			if((xafMemberInfo.MemberType == typeof(EntityKey))
					|| (xafMemberInfo.MemberType == typeof(EntityState))
					|| typeof(EntityReference).IsAssignableFrom(xafMemberInfo.MemberType)) {
				xafMemberInfo.IsVisible = false;
				xafMemberInfo.IsService = true;
			}
			else {
				xafMemberInfo.Source = this;
				EntityType entityType = FindEntityType(xafMemberInfo.Owner.Type);
				EdmMember edmMember = null;
				entityType.Members.TryGetValue(xafMemberInfo.Name, false, out edmMember);
				RealInitMemberInfo(edmMember, xafMemberInfo);
			}
		}
		public override Boolean RegisterNewMember(ITypeInfo owner, XafMemberInfo memberInfo) {
			Boolean result = false;
			if(!String.IsNullOrWhiteSpace(memberInfo.Expression)) {
				memberInfo.IsPersistent = false;
				memberInfo.IsPublic = true;
				memberInfo.IsReadOnly = true;
				memberInfo.IsProperty = true;
				memberInfo.IsVisible = true;
				result = true;
				if(!typesWithCalculatedMembers.ContainsKey(owner.Type)) {
					TypeDescriptor.AddProvider(xafTypeDescriptionProvider, owner.Type);
					typesWithCalculatedMembers.Add(owner.Type, null);
				}
			}
			return result;
		}
		public override void UpdateMember(XafMemberInfo memberInfo) {
			if(!String.IsNullOrWhiteSpace(memberInfo.Expression)) {
				memberInfo.IsPersistent = false;
				memberInfo.IsPublic = true;
				memberInfo.IsReadOnly = true;
				memberInfo.IsProperty = true;
				memberInfo.IsVisible = true;
				if(!typesWithCalculatedMembers.ContainsKey(memberInfo.Owner.Type)) {
					TypeDescriptor.AddProvider(xafTypeDescriptionProvider, memberInfo.Owner.Type);
					typesWithCalculatedMembers.Add(memberInfo.Owner.Type, null);
				}
				memberInfo.AddExtender<ExpressionEvaluator>(null);
			}
		}
		public override Object GetValue(IMemberInfo memberInfo, Object obj) {
			Object result = null;
			if(memberInfo.IsCustom && !String.IsNullOrWhiteSpace(memberInfo.Expression)) {
				ExpressionEvaluator expressionEvaluator = memberInfo.GetExtender<ExpressionEvaluator>();
				if(expressionEvaluator == null) {
					EvaluatorContextDescriptorDefault evaluatorContextDescriptor = new EvaluatorContextDescriptorDefault(memberInfo.LastMember.Owner.Type);
					expressionEvaluator = new ExpressionEvaluator(evaluatorContextDescriptor, CriteriaOperator.Parse(memberInfo.Expression));
					memberInfo.AddExtender<ExpressionEvaluator>(expressionEvaluator);
				}
				Object objectForEvaluation = memberInfo.GetOwnerInstance(obj);
				result = expressionEvaluator.Evaluate(objectForEvaluation);
			}
			else {
				result = base.GetValue(memberInfo, obj);
			}
			return result;
		}
		public Boolean CanRegister(Type type) {
			return TypeIsKnown(type) && !registeredEntities.Contains(type);
		}
		public void RegisterEntity(Type type) {
			if(TypeIsKnown(type) && !registeredEntities.Contains(type)) {
				registeredEntities.Add(type);
				TypeInfo typeInfo = (TypeInfo)typesInfo.FindTypeInfo(type);
				RegisterUsedEnumerations(typeInfo);
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
			RemoveCustomTypeDescriptionProvider();
			registeredEntities.Clear();
		}
		public override Type GetOriginalType(Type type) {
			Type result = type;
#if NET45
			if(type is System.Reflection.IReflectableType)
#endif
			{
				result = ObjectContext.GetObjectType(type);
			}
			return result;
		}
		public void Dispose() {
			RemoveCustomTypeDescriptionProvider();
		}
	}
}
