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
using System.Reflection;
using DevExpress.ExpressApp.Utils.CodeGeneration;
using DevExpress.Utils;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class DCBuilder {
		private readonly MetadataGenerator metadataGenerator;
		private bool isInitialized;
		private EntitiesToGenerateInfo entitiesToGenerateInfo;
		private CustomLogics customLogics;
		private ExistingImplementorsInfo existingImplementorsInfo;
		internal DCBuilder(ITypesInfo typesInfo) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			metadataGenerator = new MetadataGenerator(typesInfo);
		}
		private void CheckXpoNonPersistentAttribute(MetadataGenerator metadataGenerator) {
			NonPersistentAttribute nonPersistentAttribute;
			foreach(InterfaceMetadata interfaceMetadata in metadataGenerator.GetInterfacesMetadata()) {
				nonPersistentAttribute = interfaceMetadata.FindAttribute<NonPersistentAttribute>();
				if(nonPersistentAttribute != null) {
					throw new InvalidOperationException(string.Format("The '{0}' Domain Component is decorated with 'DevExpress.Xpo.NonPersistentAttribute'. This attribute has no effect when applied to Domain Components. Use 'DevExpress.ExpressApp.DC.NonPersistentDcAttribute' instead.", interfaceMetadata.FullName));
				}
				foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
					nonPersistentAttribute = propertyMetadata.FindAttribute<NonPersistentAttribute>();
					if(nonPersistentAttribute != null) {
						throw new InvalidOperationException(string.Format("The '{0}.{1}' Domain Component property is decorated with 'DevExpress.Xpo.NonPersistentAttribute'. This attribute has no effect when applied to Domain Component properties. Use 'DevExpress.ExpressApp.DC.NonPersistentDcAttribute' instead.", interfaceMetadata.FullName, propertyMetadata.Name));
					}
				}
			}
		}
		private void CheckXpoPersistentAttribute(MetadataGenerator metadataGenerator) {
			PersistentAttribute nonPersistentAttribute;
			foreach(InterfaceMetadata interfaceMetadata in metadataGenerator.GetInterfacesMetadata()) {
				nonPersistentAttribute = interfaceMetadata.FindAttribute<PersistentAttribute>();
				if(nonPersistentAttribute != null) {
					throw new InvalidOperationException(string.Format("The '{0}' Domain Component is decorated with 'DevExpress.Xpo.PersistentAttribute'. This attribute has no effect when applied to Domain Components. Use 'DevExpress.ExpressApp.DC.PersistentDcAttribute' instead.", interfaceMetadata.FullName));
				}
				foreach(PropertyMetadata propertyMetadata in interfaceMetadata.Properties) {
					nonPersistentAttribute = propertyMetadata.FindAttribute<PersistentAttribute>();
					if(nonPersistentAttribute != null) {
						throw new InvalidOperationException(string.Format("The '{0}.{1}' Domain Component property is decorated with 'DevExpress.Xpo.PersistentAttribute'. This attribute has no effect when applied to Domain Component properties. Use 'DevExpress.ExpressApp.DC.PersistentDcAttribute' instead.", interfaceMetadata.FullName, propertyMetadata.Name));
					}
				}
			}
		}
		private void CheckDeferredDeletionAttributes(MetadataGenerator metadataGenerator) {
			foreach(EntityMetadata entity in metadataGenerator.GetEntitiesMetadata()) {
				DeferredDeletionAttribute mainAttribute = FindDeferredDeletionAttribute(entity.PrimaryInterface);
				EntityMetadata checkEntity = entity;
				while(checkEntity != null) {
					if(checkEntity.BaseClass != null) {
						object[] attributes = checkEntity.BaseClass.GetCustomAttributes(typeof(DeferredDeletionAttribute), true);
						if(attributes.Length > 0) {
							DeferredDeletionAttribute deferredDeletionAttribute = (DeferredDeletionAttribute)attributes[0];
							if(!deferredDeletionAttribute.Enabled && !AreDeferredDeletionAttributesEquals(mainAttribute, deferredDeletionAttribute)) {
								string message = string.Format(
@"Cannot generate the {0} entity because the {1} base persistent class is decorated with the 'DeferredDeletion(false)' attribute.
To fix this error, ensure that all the Domain Components forming the entity are decorated with the 'DeferredDeletion(false)' attribute.
Alternatively, remove the attribute declaration from the base class.",
									entity.Name, checkEntity.BaseClass.FullName
								);
								throw new InvalidOperationException(message);
							}
						}
					}
					foreach(InterfaceMetadata interfaceMetadata in checkEntity.OwnImplementedInterfaces) {
						if(interfaceMetadata.IsPersistent) {
							DeferredDeletionAttribute deferredDeletionAttribute = FindDeferredDeletionAttribute(interfaceMetadata);
							if(!AreDeferredDeletionAttributesEquals(mainAttribute, deferredDeletionAttribute)) {
								string message = string.Format(
@"Cannot generate the {0} entity because some of the Domain Components forming the entity use deferred deletion and others do not.
To fix this error, ensure that all the Domain Components forming the entity are decorated with the DeferredDeletionAttribute and the attributes' Enabled parameter is set to the same value.
Alternatively, remove DeferredDeletionAttribute declarations from the interfaces altogether.",
									entity.Name
								);
								throw new InvalidOperationException(message);
							}
						}
					}
					checkEntity = checkEntity.BaseEntity;
				}
			}
		}
		private DeferredDeletionAttribute FindDeferredDeletionAttribute(InterfaceMetadata interfaceMetadata) {
			return interfaceMetadata.FindAttribute<DeferredDeletionAttribute>();
		}
		private bool AreDeferredDeletionAttributesEquals(DeferredDeletionAttribute first, DeferredDeletionAttribute second) {
			if((first == null) != (second == null)) {
				return false;
			}
			if(first != null && (first.Enabled != second.Enabled)) {
				return false;
			}
			return true;
		}
		private void CheckIsInitialized() {
			if(!isInitialized) {
				throw new InvalidOperationException("The DCBuilder is not initialized.");
			}
		}
		internal void Setup(EntitiesToGenerateInfo entitiesToGenerateInfo, CustomLogics customLogics, ExistingImplementorsInfo existingImplementorsInfo) {
			this.entitiesToGenerateInfo = entitiesToGenerateInfo;
			this.customLogics = customLogics;
			this.existingImplementorsInfo = existingImplementorsInfo;
			isInitialized = true;
		}
		internal Assembly GetAssembly(string assemblyFile) {
			CheckIsInitialized();
			metadataGenerator.GenerateMetadata(entitiesToGenerateInfo, existingImplementorsInfo);
			CheckXpoPersistentAttribute(metadataGenerator);
			CheckXpoNonPersistentAttribute(metadataGenerator);
			CheckDeferredDeletionAttributes(metadataGenerator);
			LogicsSource logicsSource = new LogicsSource();
			logicsSource.CollectLogics(entitiesToGenerateInfo.GetEntityInterfaces(), customLogics);
			CodeModelGenerator codeModelGenerator = new CodeModelGenerator(metadataGenerator, logicsSource);
			CodeProvider codeProvider = codeModelGenerator.GenerateCodeModel();
			CodeBuilder codeBuilder = new CodeBuilder();
			codeProvider.GetCode(codeBuilder);
			DCReferencesCollector referencesCollector = new DCReferencesCollector();
			referencesCollector.Add(typeof(DCBaseObject));
			referencesCollector.Add(typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>));
			referencesCollector.AddRange(entitiesToGenerateInfo.GetEntityInterfaces());
			foreach(string entityName in entitiesToGenerateInfo.GetEntityNames()) {
				if(entitiesToGenerateInfo.HasBaseClass(entityName)) {
					referencesCollector.Add(entitiesToGenerateInfo.GetBaseClass(entityName));
				}
			}
			CSCodeCompiler compiler = new CSCodeCompiler();
			Assembly assembly = compiler.Compile(codeBuilder.ToString(), referencesCollector.Collect(), assemblyFile);
			return assembly;
		}
	}
}
