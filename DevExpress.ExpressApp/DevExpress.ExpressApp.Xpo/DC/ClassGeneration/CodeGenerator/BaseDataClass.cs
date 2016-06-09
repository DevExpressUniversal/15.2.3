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
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal abstract class BaseDataClass : ClassCode {
		private CodeModelLogicHelper logicHelper;
		private IList<PropertyMetadata> aliasedPropertyMetadata = new List<PropertyMetadata>();
		public BaseDataClass(string className) : base(className) { }
		public void SetLogicHelper(CodeModelLogicHelper logicHelper) {
			this.logicHelper = logicHelper;
		}
		protected CodeModelLogicHelper LogicHelper {
			get { return logicHelper; }
		}
		protected override void FillClassCore(ClassMetadata classMetadata, DataMetadata dataMetadata) {
			base.FillClassCore(classMetadata, dataMetadata);
			if(dataMetadata != null) {
				foreach(InterfaceMetadata interfaceMetadata in dataMetadata.StoredInterfaces) {
					FillClassPersistentInterfacesCodeModel(interfaceMetadata, null);
				}
				foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetDataClassStoredPropertyInfos(dataMetadata)) {
					FillClassPropertyCodeModel(propertyInfo.PropertyMetadata, propertyInfo.InterfaceMetadata, propertyInfo.DataMetadata, null);
				}
			}
			AddObjectChangingMethod(classMetadata != null ? classMetadata : dataMetadata);
			AddObjectChangedMethod(classMetadata != null ? classMetadata : dataMetadata);
		}
		protected virtual PropertyCode GenerateNonAggregatedNonListPropertyCode(PropertyMetadata propertyMetadata) {
			InterfaceMetadata propertyResultInterfaceMetadata = MetadataSource.FindInterfaceMetadataByType(propertyMetadata.PropertyType);
			if(propertyResultInterfaceMetadata != null && propertyResultInterfaceMetadata.DataClass != null) {
				return new AggregatedPersistentInterfacePropertyCode(propertyMetadata, this, "this");
			}
			return new PersistentPropertyWithFieldCodeBase(propertyMetadata, this);
		}
		protected abstract PropertyCode GenerateAggregatedNonListPropertyCodeCore(PropertyMetadata propertyMetadata, DataMetadata dataMetadata);
		protected abstract PropertyCode GenerateAggregatedListPropertyCode(PropertyMetadata propertyMetadata, DataMetadata dataMetadata);
		protected virtual PropertyCode GenerateNonAggregatedListPropertyCode(InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) {
			Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(propertyMetadata);
			InterfaceMetadata associatedPropertyInterfaceMetadata = MetadataSource.FindInterfaceMetadataByType(associatedType);
			if(interfaceMetadata.DataClass == null && associatedPropertyInterfaceMetadata != null && associatedPropertyInterfaceMetadata.DataClass != null) {
				string persistentInterfaceDataPropertyName = CodeModelGeneratorHelper.GetAliasPropertyName(propertyMetadata);
				return new AggregatedCollectionPropertyCode(propertyMetadata, this, persistentInterfaceDataPropertyName);
			}
			else {
				return new CollectionPropertyCode(propertyMetadata, this);
			}
		}
		protected virtual void FillClassPersistentInterfacesCodeModel(InterfaceMetadata interfaceMetadata, DataMetadata dataMetadata) { }
		protected virtual void FillAssociationNonAggregatedOneToManyProperty(PropertyCode propertyCode, PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			propertyCode.AddAttribute(new AssociationAttribute(propertyMetadata.AssociationInfo.Name));
		}
		protected virtual void FillAssociationAggregatedManyToManyCodeModel(PropertyCode propertyCode, InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) { }
		protected void FillAliasPropertyCode(PropertyCode mainPropertyCode, PropertyMetadata mainPropertyMetadata, InterfaceMetadata interfaceMetadata, bool isAssociation, bool isLogic) {
			aliasedPropertyMetadata.Add(mainPropertyMetadata);
			string aliasPropertyName = CodeModelGeneratorHelper.GetAliasPropertyName(mainPropertyMetadata);
			mainPropertyCode.AddAttribute(new PersistentAliasAttribute(string.Format("[{0}]", aliasPropertyName)));
			PropertyCode aliasPropertyCode = null;
			if(CodeModelGeneratorHelper.IsListType(mainPropertyMetadata)) {
				aliasPropertyCode = new AliaseCollectionPropertyCode(mainPropertyMetadata, this);
			}
			else {
				if(isLogic) {
					aliasPropertyCode = new LogicPersistentInterfaceDataPropertyCode(mainPropertyMetadata, this);
				}
				else {
					aliasPropertyCode = new AliasePersistentPropertyCode(mainPropertyMetadata, this);
				}
			}
			if(isAssociation) {
				if(interfaceMetadata.DataClass != null) {
					aliasPropertyCode.AddAttribute(new AssociationAttribute(mainPropertyMetadata.AssociationInfo.Name));
				}
				else {
					FillPropertyManyToManyAttribute(aliasPropertyCode, mainPropertyMetadata, interfaceMetadata);
				}
			}
			AddPropertyCode(aliasPropertyCode);
		}
		protected void FillAssociationManyToManyLinksPropertyCode(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			string linkPropertyAssociationName = CodeModelGeneratorHelper.GetLinkPropertyAssociationName(propertyMetadata, interfaceMetadata);
			PropertyCode linksPropertyCode = new LinkCollectionPropertyCode(propertyMetadata, this, linkPropertyAssociationName);
			AddPropertyCode(linksPropertyCode);
		}
		protected void FillPropertyManyToManyAttribute(PropertyCode propertyCode, PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			string linksPropertyName = CodeModelGeneratorHelper.GetLinksPropertyName(propertyMetadata);
			string associatedLinkPropertyInLinkClassName = CodeModelGeneratorHelper.GetLinkClassLinkPropertyName(propertyMetadata, interfaceMetadata);
			propertyCode.AddAttribute(new DevExpress.Xpo.ManyToManyAliasAttribute(linksPropertyName, associatedLinkPropertyInLinkClassName));
		}
		protected void FillExtendedPropertiesNonAggregatedPropertyCode(PropertyCode propertyCode, PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			if(propertyMetadata.AssociationInfo != null) {
				if(propertyMetadata.AssociationInfo.AssociationType == AssociationType.OneToMany) {
					FillAssociationNonAggregatedOneToManyProperty(propertyCode, propertyMetadata, interfaceMetadata);
				}
				else {
					Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(propertyMetadata);
					InterfaceMetadata associatedInterfaceMetadata = GetInterfaceMetadataByType(associatedType);
					if(associatedInterfaceMetadata.DataClass != null || interfaceMetadata.DataClass != null) {
						FillAssociationAggregatedManyToManyCodeModel(propertyCode, interfaceMetadata, propertyMetadata);
					}
					else {
						FillPropertyManyToManyAttribute(propertyCode, propertyMetadata, interfaceMetadata);
						FillAssociationManyToManyLinksPropertyCode(propertyMetadata, interfaceMetadata);
					}
				}
			}
			else {
				InterfaceMetadata propertyResultInterfaceMetadata = MetadataSource.FindInterfaceMetadataByType(propertyMetadata.PropertyType);
				if(propertyResultInterfaceMetadata != null) {
					if((!propertyMetadata.IsLogicRequired && propertyResultInterfaceMetadata.DataClass != null) ||
						(propertyMetadata.IsLogicRequired && propertyMetadata.FindAttribute<DevExpress.Xpo.PersistentAttribute>() != null)) {
						FillAliasPropertyCode(propertyCode, propertyMetadata, interfaceMetadata, false, propertyMetadata.IsLogicRequired);
					}
				}
			}
		}
		private void AddObjectChangedMethod(ClassMetadata entityMetadata) {
			bool isEntity = false;
			bool isBaseEntity = false;
			if(entityMetadata is EntityMetadata) {
				isEntity = true;
				isBaseEntity = ((EntityMetadata)entityMetadata).BaseEntity == null;
			}
			Dictionary<PropertyMetadata, MethodLogic> afterChangePropertyLogics = LogicHelper.GetObjectChangedMethodsInfo(entityMetadata);
			ObjectChangedMethodCode objectChangedMethodCode = new ObjectChangedMethodCode(afterChangePropertyLogics, aliasedPropertyMetadata, isEntity, isBaseEntity);
			AddMethodCode(objectChangedMethodCode);
			if(isEntity) {
				InvokeAfterChangeLogicMethodCode invokeAfterChangeLogicMethodCode = new InvokeAfterChangeLogicMethodCode(afterChangePropertyLogics, isBaseEntity);
				AddMethodCode(invokeAfterChangeLogicMethodCode);
			}
		}
		Dictionary<PropertyMetadata, MethodLogic> beforeChangePropertyLogics = null;
		Dictionary<PropertyMetadata, MethodLogic> GetBeforeChangePropertyLogics(ClassMetadata classMetadata) {
			if(beforeChangePropertyLogics == null) {
				beforeChangePropertyLogics = LogicHelper.GetObjectChangingMethodsInfo(classMetadata);
			}
			return beforeChangePropertyLogics;
		}
		private void AddObjectChangingMethod(ClassMetadata entityMetadata) {
			bool isEntity = false;
			bool isBaseEntity = false;
			if(entityMetadata is EntityMetadata) {
				isEntity = true;
				isBaseEntity = ((EntityMetadata)entityMetadata).BaseEntity == null;
			}
			Dictionary<PropertyMetadata, MethodLogic> beforeChangePropertyLogics = GetBeforeChangePropertyLogics(entityMetadata);
			ObjectChangingMethodCode objectChangingMethodCode = new ObjectChangingMethodCode(beforeChangePropertyLogics, aliasedPropertyMetadata, isEntity, isBaseEntity);
			AddMethodCode(objectChangingMethodCode);
			if(isEntity) {
				InvokeBeforeChangeLogicMethodCode invokeBeforeChangeLogicMethodCode = new InvokeBeforeChangeLogicMethodCode(beforeChangePropertyLogics, isBaseEntity);
				AddMethodCode(invokeBeforeChangeLogicMethodCode);
			}
		}
		private PropertyCode GeneratePropertyByAttributesIfNeed(PropertyMetadata propertyMetadata) {
			if(CodeModelGeneratorHelper.HasPersistentAliasAttribute(propertyMetadata)) {
				return new PersistentAliasPropertyCode(propertyMetadata, this);
			}
			return null;
		}
		protected void FillClassPropertyCodeModel(PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata, DataMetadata dataMetadata, EntityMetadata entityMetadata) {
			if(entityMetadata == null || (entityMetadata != null && !LogicHelper.IsLogicRequired(propertyMetadata, interfaceMetadata, entityMetadata))) {
				PropertyCode propertyCode = null;
				if(dataMetadata != null) {
					propertyCode = GenerateAggregatedPropertyCode(propertyMetadata, dataMetadata);
				}
				else {
					propertyCode = GenerateNonAggregatedPropertyCode(interfaceMetadata, propertyMetadata);
				}
				AddPropertyCode(propertyCode);
				if(dataMetadata == null) {
					FillExtendedPropertiesNonAggregatedPropertyCode(propertyCode, propertyMetadata, interfaceMetadata);
				}
			}
		}
		private PropertyCode GenerateNonAggregatedPropertyCode(InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) {
			PropertyCode propertyCode = GeneratePropertyByAttributesIfNeed(propertyMetadata);
			if(propertyCode == null) {
				if(CodeModelGeneratorHelper.IsListType(propertyMetadata)) {
					propertyCode = GenerateNonAggregatedListPropertyCode(interfaceMetadata, propertyMetadata);
				}
				else {
					propertyCode = GenerateNonAggregatedNonListPropertyCode(propertyMetadata);
				}
			}
			return propertyCode;
		}
		private PropertyCode GenerateAggregatedPropertyCode(PropertyMetadata propertyMetadata, DataMetadata dataMetadata) {
			PropertyCode propertyCode = null;
			if(CodeModelGeneratorHelper.IsListType(propertyMetadata)) {
				propertyCode = GenerateAggregatedListPropertyCode(propertyMetadata, dataMetadata);
			}
			else {
				propertyCode = GenerateAggregatedNonListPropertyCodeCore(propertyMetadata, dataMetadata);
			}
			string dataClassFieldName = CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata);
			propertyCode.AddAttribute(new PersistentAliasAttribute(string.Format("[{0}.{1}]", dataClassFieldName, propertyMetadata.Name)));
			return propertyCode;
		}
		protected Dictionary<InterfaceMetadata, IList<DataMetadata>> GetInterfaceDataInfo(EntityMetadata entityMetadata) {
			Dictionary<InterfaceMetadata, IList<DataMetadata>> interfaceInfo = new Dictionary<InterfaceMetadata, IList<DataMetadata>>();
			foreach(DataMetadata dataMetadata in entityMetadata.AggregatedData) {
				foreach(InterfaceMetadata interfaceMetadata in dataMetadata.StoredInterfaces) {
					IList<DataMetadata> dataMetadataList = null;
					if(!interfaceInfo.TryGetValue(interfaceMetadata, out dataMetadataList)) {
						dataMetadataList = new List<DataMetadata>();
						interfaceInfo.Add(interfaceMetadata, dataMetadataList);
					}
					dataMetadataList.Add(dataMetadata);
				}
			}
			return interfaceInfo;
		}
	}
}
