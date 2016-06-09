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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Entity.Model.Metadata {
	public interface IPluralizationService {
		string GetPluralizedName(string name);
	}
	public interface IMapper : IPluralizationService {
		bool HasView(EntitySetBaseInfo entitySetBase);
		EntityTypeBaseInfo GetMappedOSpaceType(EntityTypeBaseInfo cSpaceType);
		Type ResolveClrType(EntityTypeBaseInfo cSpaceType);		
	}
	public interface IAssociationTypeSource {
		AssociationTypeInfo GetAssociationTypeFromCSpace(string fullName);
	}
	public class EntityTypeInfoFactory {
		public virtual IEntityTypeInfo Create(EntityTypeBaseInfo entityType, IAssociationTypeSource associationTypeSource, IMapper mapper, IDataColumnAttributesProvider attributesProvider = null) {
			EntityTypeBaseInfo mappedEntityType = mapper.GetMappedOSpaceType(entityType);
			return new EntityTypeInfo(mappedEntityType, mapper.ResolveClrType(mappedEntityType), associationTypeSource, mapper, attributesProvider ?? new EmptyDataColumnAttributesProvider(), this);
		}
	}
	public class EntityTypeInfo : IEntityTypeInfo {
		ICollection<IEdmPropertyInfo> keyMembers;
		ICollection<IEdmPropertyInfo> properties;
		ICollection<IEdmAssociationPropertyInfo> lookupTables;
		Dictionary<string, string> foreignKeysNames;
		Dictionary<IEdmPropertyInfo, IEdmPropertyInfo> foreignKeyDependentProperties;
		IDataColumnAttributesProvider  attributesProvider;
		readonly IMapper mapper;
		readonly EntityTypeBaseInfo entityType;
		readonly Type type;
		readonly EntityTypeInfoFactory entityTypeInfoFactory;
		public EntityTypeInfo(EntityTypeBaseInfo entityType, Type clrType, IAssociationTypeSource associationTypeSource, IMapper mapper, IDataColumnAttributesProvider attributesProvider, EntityTypeInfoFactory entityTypeInfoFactory) {
			this.mapper = mapper;
			this.type = clrType;
			this.entityType = entityType;
			this.AssociationTypeSource = associationTypeSource;
			this.attributesProvider = attributesProvider;
			this.entityTypeInfoFactory = entityTypeInfoFactory;
		}
		public Dictionary<string, string> ForeignKeysNames {
			get {
				if(foreignKeysNames == null)
					Init();
				return foreignKeysNames;
			}
		}
		public IAssociationTypeSource AssociationTypeSource { get; private set; }
		public EntityTypeBaseInfo EntityType {
			get {
				return entityType;
			}
		}
		void Init() {
			foreignKeysNames = new Dictionary<string, string>();			
			properties = new List<IEdmPropertyInfo>();
			lookupTables = new List<IEdmAssociationPropertyInfo>();
			if(EntityType == null)
				return;
			IEnumerable<EdmMemberInfo> sortedProperties = GetSortedProperties();
			foreach(EdmMemberInfo member in sortedProperties) {
				if(!member.IsProperty || !member.IsNavigationProperty)
					continue;
				IEnumerable<string> names = GetDependentPropertyNames(this, member);
				foreach(string name in names)
					if(!foreignKeysNames.ContainsKey(name))
						foreignKeysNames.Add(name, member.Name);
			}
			foreach(EdmMemberInfo member in sortedProperties) {
				if(!member.IsProperty)
					continue;
				bool isLookupTable = member.IsNavigationProperty && member.IsCollectionProperty;
				if(isLookupTable) {
					IEdmAssociationPropertyInfo property = CreateAssociationPropertyInfo(GetClrType(EntityType, mapper), member, false, true);
					if(IsValidLookUpTableProperty(property))
						lookupTables.Add(property);
				}
				else if(member.IsNavigationProperty)
					properties.Add(CreateEdmPropertyInfo(GetClrType(EntityType, mapper), member, false, true));
				else
					properties.Add(CreateEdmPropertyInfo(type, member, foreignKeysNames.ContainsKey(member.Name), false));
			}			
		}
		protected virtual bool IsValidLookUpTableProperty(IEdmAssociationPropertyInfo property) {
			return property != null;
		}
		public IEdmPropertyInfo GetDependentProperty(IEdmPropertyInfo foreignKey) {
			if(foreignKey == null || !foreignKey.IsForeignKey)
				return null;
			if(foreignKeyDependentProperties == null)
				InitForeignKeyDependencies();
			IEdmPropertyInfo dependentProperty;
			if(foreignKeyDependentProperties.TryGetValue(foreignKey, out dependentProperty))
				return dependentProperty;
			return null;
		}
		public IEdmPropertyInfo GetForeignKey(IEdmPropertyInfo dependentProperty) {
			if(dependentProperty == null) 
				return null;
			if(foreignKeyDependentProperties == null)
				InitForeignKeyDependencies();
			return foreignKeyDependentProperties.Where(x => x.Value == dependentProperty).Select<KeyValuePair<IEdmPropertyInfo, IEdmPropertyInfo>, IEdmPropertyInfo>(x=>x.Key).FirstOrDefault();			
		}
		void InitForeignKeyDependencies() {
			foreignKeyDependentProperties = new Dictionary<IEdmPropertyInfo, IEdmPropertyInfo>();
			foreach(string foreignKey in ForeignKeysNames.Keys) {
				IEdmPropertyInfo foreignKeyMember = properties.FirstOrDefault(x => x.IsForeignKey && x.Name == foreignKey);
				string dependentPropertyName = ForeignKeysNames[foreignKey];
				IEdmPropertyInfo dependentProperty = properties.FirstOrDefault(x => x.Name == dependentPropertyName);
				if(foreignKeyMember != null && dependentProperty != null)
					foreignKeyDependentProperties[foreignKeyMember] = dependentProperty;
			}
		}
		IEnumerable<EdmMemberInfo> GetDependentProperties(IEntityTypeInfo declaringType, EdmMemberInfo navigationProperty) {
			AssociationTypeInfo associationType = navigationProperty.GetAssociationType();			
			if (associationType == null || !associationType.IsForeignKey)
				return null;
			IEnumerable<EdmMemberInfo> foreignProperties = associationType.GetDependentProperties(navigationProperty);
			if (foreignProperties == null) {
				associationType = associationType.GetCSpaceAssociationType(declaringType);
				foreignProperties = associationType.GetDependentProperties(navigationProperty);
			}
			return foreignProperties;
		}
		IEnumerable<EdmMemberInfo> GetToEndProperties(IEntityTypeInfo declaringType, EdmMemberInfo navigationProperty, EntityTypeBaseInfo toEndEntityTypeInfo) {
			AssociationTypeInfo associationType = navigationProperty.GetAssociationType();
			if(associationType == null || !associationType.IsForeignKey)
				return null;
			var toEndProperties = associationType.GetToEndPropertyNames(navigationProperty, toEndEntityTypeInfo);
			if(toEndProperties == null) {
				associationType = associationType.GetCSpaceAssociationType(declaringType);
				toEndProperties = associationType.GetToEndPropertyNames(navigationProperty, toEndEntityTypeInfo);
			}
			return toEndProperties;
		}
		IEnumerable<string> GetDependentPropertyNames(IEntityTypeInfo declaringType, EdmMemberInfo navigationProperty) {
			IEnumerable<EdmMemberInfo> ps = GetDependentProperties(declaringType, navigationProperty);
			if (ps == null)
				return new string[0];
			return ps.Select<EdmMemberInfo, string>(x => x.Name);
		}
		Type GetClrType(EntityTypeBaseInfo entityType, IMapper mapper) {
			EntityTypeBaseInfo mappedEntityType = mapper.GetMappedOSpaceType(entityType);
			return mapper.ResolveClrType(mappedEntityType);
		}
		IEnumerable<EdmMemberInfo> GetProperties() {
			EntityTypeBaseInfo entityType = this.entityType;
			foreach (EdmMemberInfo item in entityType.Properties)
				yield return item;
			foreach (EdmMemberInfo item in entityType.NavigationProperties)
				yield return item;
		}
		IEnumerable<EdmMemberInfo> GetSortedProperties() {
			List<EdmMemberInfo> properties = new List<EdmMemberInfo>();
			foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(type)) {
				EdmMemberInfo property =  GetProperties().FirstOrDefault(x => x.Name == propertyDescriptor.Name);
				if (property != null)
					properties.Add(property);				
			}
			return properties;
		}
		protected virtual IEdmAssociationPropertyInfo CreateAssociationPropertyInfo(Type type, EdmMemberInfo edmProperty, bool isForeignKey, bool isNavigationProperty) {
			PropertyDescriptor property = TypeDescriptor.GetProperties(type)[edmProperty.Name];
			DataColumnAttributes attributes = attributesProvider.GetAtrributes(property, type);
			object toEndElementType = PropertyAccessor.GetValue(PropertyAccessor.GetValue(PropertyAccessor.GetValue(edmProperty.ToEndMember, "TypeUsage"), "EdmType"), "ElementType");
			if(toEndElementType == null)
				return null;
			EntityTypeBaseInfo toEndEntityTypeInfo = new EntityTypeBaseInfo(toEndElementType);
			IEntityTypeInfo toEndEntityType = this.entityTypeInfoFactory.Create(toEndEntityTypeInfo, this.AssociationTypeSource, this.mapper, this.attributesProvider);
			var foreignKey = GetToEndProperties(this, edmProperty, toEndEntityTypeInfo);
			if(foreignKey == null)
				return null;
			return new EdmAssociationPropertyInfo(property, attributes, toEndEntityType, isNavigationProperty, foreignKey, isForeignKey);
		}
		protected virtual EdmPropertyInfo CreateEdmPropertyInfo(Type type, EdmMemberInfo edmProperty, bool isForeignKey, bool isNavigationProperty) {
			PropertyDescriptor property = TypeDescriptor.GetProperties(type)[edmProperty.Name];
			DataColumnAttributes attributes = attributesProvider.GetAtrributes(property, type);
			return new EdmPropertyInfo(property, attributes, isNavigationProperty, isForeignKey);
		}
		Type IEntityTypeInfo.Type {
			get { return type; }
		}
		IEnumerable<IEdmPropertyInfo> IEntityProperties.AllProperties {
			get { return PropertiesCore; }
		}
		IEnumerable<IEdmPropertyInfo> PropertiesCore {
			get {
				if(properties == null)
					Init();				
				return properties;
			}
		}
		IEnumerable<IEdmPropertyInfo> IEntityTypeInfo.KeyMembers {
			get {
				if(keyMembers != null)
					return keyMembers;
				if(EntityType == null)
					return null;				
				keyMembers = new List<IEdmPropertyInfo>();
				foreach (EdmMemberInfo edmMember in this.entityType.KeyMembers)
					if (edmMember != null && edmMember.BuiltInTypeKind == BuiltInTypeKind.EdmProperty)
						keyMembers.Add(CreateEdmPropertyInfo(type, edmMember, false, false));
				return keyMembers;				
			}
		}
		public IEnumerable<IEdmAssociationPropertyInfo> LookupTables {
			get {
				if(lookupTables == null)
					Init();
				return lookupTables;
			}
		}
	}
}
