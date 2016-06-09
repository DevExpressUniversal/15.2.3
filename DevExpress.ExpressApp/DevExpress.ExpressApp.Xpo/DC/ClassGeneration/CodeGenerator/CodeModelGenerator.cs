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
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class CodeModelGenerator {
		enum ClassGenerationType { EntityClass, DataClass, EntityDataClass };
		private IMetadataSource metadataSource;
		private CodeModelLogicHelper logicHelper;
		private Dictionary<string, ClassCode> createdClasses = new Dictionary<string, ClassCode>();
		private Dictionary<DataMetadata, List<EntityMetadata>> dataEntitiesInfo = new Dictionary<DataMetadata, List<EntityMetadata>>();
		public const string DefaultClassesNamespace = "DevExpress.ExpressApp.DC.GeneratedClasses";
		public CodeModelGenerator(IMetadataSource metadataSource, LogicsSource logicsSource) {
			if(metadataSource == null) {
				throw new ArgumentNullException("metadataSource");
			}
			this.metadataSource = metadataSource;
			this.logicHelper = new CodeModelLogicHelper(logicsSource, metadataSource);
		}
		public CodeProvider GenerateCodeModel() {
			createdClasses.Clear();
			dataEntitiesInfo.Clear();
			CodeModelGeneratorHelper.SynchronizePropertiesAttributes(metadataSource.GetInterfacesMetadata());
			NamespaceCode namespaceCode = new NamespaceCode(DefaultClassesNamespace);
			foreach(EntityMetadata entityMetadata in metadataSource.GetEntitiesMetadata()) {
				foreach(DataMetadata dataMetadata in entityMetadata.AggregatedData) {
					FillDataEntitiesInfo(dataMetadata, entityMetadata);
				}
				ClassCode classCode = GenerateEntityClassCodeModel(entityMetadata);
				foreach(PropertyCodeBase pr in classCode.Properties) {
					if(pr is LinkCollectionPropertyCode) {
						GenerateLinkClassCodeModel((LinkCollectionPropertyCode)pr, classCode.Name);
					}
				}
				createdClasses.Add(classCode.Name, classCode);
			}
			foreach(DataMetadata dataMetadata in dataEntitiesInfo.Keys) {
				ClassCode classCode = GenerateDataClassCodeModel(dataMetadata);
				foreach(PropertyCodeBase pr in classCode.Properties) {
					if(pr is LinkCollectionPropertyCode) {
						GenerateLinkClassCodeModel((LinkCollectionPropertyCode)pr, classCode.Name);
					}
				}
				createdClasses.Add(classCode.Name, classCode);
			}
			foreach(DataMetadata dataMetadata in dataEntitiesInfo.Keys) {
				foreach(EntityMetadata entityMetadata in dataEntitiesInfo[dataMetadata]) {
					ClassCode classCode = GenerateEntityDataClassCodeModel(dataMetadata, entityMetadata);
					createdClasses.Add(classCode.Name, classCode);
				}
			}
			foreach(ClassCode item in createdClasses.Values) {
				namespaceCode.Content.Add(item);
			}
			return namespaceCode;
		}
		private void FillDataEntitiesInfo(DataMetadata dataMetadata, EntityMetadata entityMetadata) {
			List<EntityMetadata> entityMetadatas = null;
			if(!dataEntitiesInfo.TryGetValue(dataMetadata, out entityMetadatas)) {
				entityMetadatas = new List<EntityMetadata>();
				dataEntitiesInfo[dataMetadata] = entityMetadatas;
			}
			if(!entityMetadatas.Contains(entityMetadata)) {
				entityMetadatas.Add(entityMetadata);
			}
		}
		private T GenerateEmptyClassCodeModel<T>(EntityMetadata entityMetadata) where T : ClassCode {
			string baseClassFullName = entityMetadata.BaseEntity != null ? entityMetadata.BaseEntity.Name : null;
			if(baseClassFullName == null && entityMetadata.BaseClass != null) {
				baseClassFullName = CodeBuilder.TypeToString(entityMetadata.BaseClass);
			}
			T result = GenerateEmptyClassCodeModel<T>(entityMetadata.Name, baseClassFullName);
			result.AddAttribute(CodeModelGeneratorHelper.GetClassAttributes(entityMetadata));
			return result;
		}
		private T GenerateEmptyClassCodeModel<T>(string className, string baseClassFullName) where T : ClassCode {
			T classCode = (T)TypeHelper.CreateInstance(typeof(T), className);
			classCode.BaseClassFullName = baseClassFullName ?? CodeBuilder.TypeToString(typeof(DCBaseObject));
			return classCode;
		}
		private ClassCode GenerateEntityClassCodeModel(EntityMetadata entityMetadata) {
			EntityClassCode classCode = GenerateEmptyClassCodeModel<EntityClassCode>(entityMetadata);
			classCode.SetLogicHelper(logicHelper);
			classCode.FillClass(entityMetadata, null, metadataSource);
			return classCode;
		}
		private ClassCode GenerateDataClassCodeModel(DataMetadata dataMetadata) {
			DataClassCode classCode = GenerateEmptyClassCodeModel<DataClassCode>(dataMetadata.Name, null);
			classCode.AddAttribute(CodeModelGeneratorHelper.GetClassAttributes(dataMetadata));
			classCode.SetLogicHelper(logicHelper);
			classCode.FillClass(null, dataMetadata, metadataSource);
			return classCode;
		}
		private ClassCode GenerateEntityDataClassCodeModel(DataMetadata dataMetadata, EntityMetadata entityMetadata) {
			ClassCode classCode = GenerateEmptyClassCodeModel<EntityDataClassCode>(CodeModelGeneratorHelper.GetEntityDataClassName(dataMetadata, entityMetadata.Name), dataMetadata.Name);
			classCode.FillClass(entityMetadata, dataMetadata, metadataSource);
			return classCode;
		}
		private void GenerateLinkClassCodeModel(LinkCollectionPropertyCode linkCollectionProperty, string clasName) {
			PropertyMetadata propertyMetadata = linkCollectionProperty.PropertyMetadata;
			ClassCode linkClass = GetLinkClass(propertyMetadata);
			PersistentPropertyWithFieldCodeBase linkPropertyInLinkClass = new LinkPropertyCode(propertyMetadata, linkClass, linkCollectionProperty.LinkPropertyAssociationName, clasName);
			linkClass.AddPropertyCode(linkPropertyInLinkClass);
			linkClass.FillClass(null, null, null);
		}
		private ClassCode GetLinkClass(PropertyMetadata propertyMetadata) {
			string linkClassName = propertyMetadata.AssociationInfo.Name;
			ClassCode linkClass = null;
			if(!createdClasses.TryGetValue(linkClassName, out linkClass)) {
				linkClass = GenerateEmptyClassCodeModel<LinkClassCode>(linkClassName, null);
				createdClasses[linkClassName] = linkClass;
			}
			return linkClass;
		}
	}
	public class DcSpecificWords {
		public const string FieldLogicInstancePrefix = "logic_";
		public const string FieldPropertyPrefix = "_";
		public const string PropertyPersistentAliasPrefix = "Alias_";
		public const string LinkedPostfix = "_Link";
		public const string LinksPostfix = "__Links";
		public const string ObjectChangedPostfix = "_Changed";
		public const string LogicPrefixGetStatic = "Get_";
		public const string LogicPrefixGetNonStatic = "get_";
		public const string LogicPrefixSetStatic = "Set_";
		public const string LogicPrefixSetNonStatic = "set_";
		public const string LogicPrefixInit = "Init_";
		public const string LogicPrefixAfterChange = "AfterChange_";
		public const string LogicPrefixBeforeChange = "BeforeChange_";
		public const string LogicAfterConstruction = "AfterConstruction";
		public const string LogicOnLoaded = "OnLoaded";
		public const string LogicOnSaving = "OnSaving";
		public const string LogicOnSaved = "OnSaved";
		public const string LogicOnDeleting = "OnDeleting";
		public const string LogicOnDeleted = "OnDeleted";
	}
}
