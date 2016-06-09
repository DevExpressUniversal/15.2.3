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
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class EntityClassCode : BaseDataClass {
		public EntityClassCode(string className) : base(className) { }
		protected override void FillClassCore(ClassMetadata classMetadata, DataMetadata dataMetadata) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(LogicHelper, "logicHelper");
			base.FillClassCore(classMetadata, dataMetadata);
			EntityMetadata entityMetadata = (EntityMetadata)classMetadata;
			Dictionary<InterfaceMetadata, IList<DataMetadata>> interfaceInfo = GetInterfaceDataInfo(entityMetadata);
			if(entityMetadata.BaseEntity == null) {
				entityMetadata.OwnImplementedInterfaces.Add(GetObjectChangingInterface());
			}
			foreach(InterfaceMetadata interfaceMetadata in entityMetadata.OwnImplementedInterfaces) {
				DataMetadata _dataMetadata = null;
				IList<DataMetadata> _dataMetadatas;
				if(interfaceInfo.TryGetValue(interfaceMetadata, out _dataMetadatas)) {
					_dataMetadata = _dataMetadatas[0];
				}
				FillClassPersistentInterfacesCodeModel(interfaceMetadata, _dataMetadata);
			}
			foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(entityMetadata, false)) {
				IList<DataMetadata> _dataMetadatas = null;
				DataMetadata _dataMetadata = null;
				if(interfaceInfo.TryGetValue(propertyInfo.InterfaceMetadata, out _dataMetadatas)) {
					if(propertyInfo.PropertyMetadata.IsPersistent && _dataMetadatas.Count > 1) {
						throw new InvalidOperationException(string.Format("The '{0}' entity aggregates the '{1}' and '{2}' shared parts that both declare the '{3}.{4}' member.", entityMetadata.Name, _dataMetadatas[0].PrimaryInterface.FullName, _dataMetadatas[1].PrimaryInterface.FullName, propertyInfo.InterfaceMetadata.FullName, propertyInfo.PropertyMetadata.Name));
					}
					_dataMetadata = _dataMetadatas[0];
				}
				FillClassPropertyCodeModel(propertyInfo.PropertyMetadata, propertyInfo.InterfaceMetadata, _dataMetadata, entityMetadata);
			}
			FillClassMethodsCodeModel(entityMetadata);
			FillAfterConstructionClassLogic(entityMetadata);
			FillClassLogics(entityMetadata);
		}
		private InterfaceMetadata GetObjectChangingInterface() {
			InterfaceMetadata result = new InterfaceMetadata();
			result.FullName = typeof(IPropertyChangeNotificationReceiver).FullName;
			result.InterfaceType = typeof(IPropertyChangeNotificationReceiver);
			return result;
		}
		protected override void FillClassFieldsCore() {
			base.FillClassFieldsCore();
			foreach(MethodCode item in Methods) {
				if(item is ILogicsProvider) {
					FillClassLogicVariables(((ILogicsProvider)item).Logics);
				}
			}
			foreach(PropertyCodeBase item in Properties) {
				if(item is ILogicsProvider) {
					FillClassLogicVariables(((ILogicsProvider)item).Logics);
				}
			}
		}
		protected override void FillAssociationNonAggregatedOneToManyProperty(PropertyCode propertyCode, PropertyMetadata propertyMetadata, InterfaceMetadata interfaceMetadata) {
			Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(propertyMetadata);
			InterfaceMetadata associatedInterfaceMetadata = GetInterfaceMetadataByType(associatedType);
			if(associatedInterfaceMetadata.DataClass != null) {
				FillAliasPropertyCode(propertyCode, propertyMetadata, associatedInterfaceMetadata, true, false);
			}
			else {
				base.FillAssociationNonAggregatedOneToManyProperty(propertyCode, propertyMetadata, interfaceMetadata);
			}
		}
		protected override PropertyCode GenerateAggregatedNonListPropertyCodeCore(PropertyMetadata propertyMetadata, DataMetadata dataMetadata) {
			string dataClassFieldName = CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata);
			if(propertyMetadata.AssociationInfo != null) {
				return new AggregatedPersistentInterfacePropertyCode(propertyMetadata, this, dataClassFieldName);
			}
			else {
				return new AggregatedPropertyCode(propertyMetadata, this, dataClassFieldName);
			}
		}
		protected override PropertyCode GenerateAggregatedListPropertyCode(PropertyMetadata propertyMetadata, DataMetadata dataMetadata) {
			string dataClassFieldName = CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata);
			string persistentInterfaceDataPropertyName = string.Format("{0}.{1}", dataClassFieldName, propertyMetadata.Name);
			return new AggregatedCollectionPropertyCode(propertyMetadata, this, persistentInterfaceDataPropertyName);
		}
		protected override void FillAssociationAggregatedManyToManyCodeModel(PropertyCode propertyCode, InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) {
			base.FillAssociationAggregatedManyToManyCodeModel(propertyCode, interfaceMetadata, propertyMetadata);
			if(interfaceMetadata.DataClass == null) {
				FillAliasPropertyCode(propertyCode, propertyMetadata, interfaceMetadata, true, false);
				FillAssociationManyToManyLinksPropertyCode(propertyMetadata, interfaceMetadata);
			}
		}
		protected override void FillClassPersistentInterfacesCodeModel(InterfaceMetadata interfaceMetadata, DataMetadata dataMetadata) {
			base.FillClassPersistentInterfacesCodeModel(interfaceMetadata, dataMetadata);
			ImplementedInterfaceFullNames.Add(CodeBuilder.TypeToString(interfaceMetadata.InterfaceType));
			if(interfaceMetadata.IsPersistent) {
				Type persistentInterfaceType = typeof(DevExpress.Xpo.Helpers.IPersistentInterface<>).MakeGenericType(interfaceMetadata.InterfaceType);
				ImplementedInterfaceFullNames.Add(CodeBuilder.TypeToString(persistentInterfaceType));
				Type persistentInterfaceDataType = typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(interfaceMetadata.InterfaceType);
				string propertyTypeFullName = CodeBuilder.TypeToString(persistentInterfaceDataType);
				string propertyFieldTypeFullName = dataMetadata != null ?
					CodeModelGeneratorHelper.GetEntityDataClassName(dataMetadata, Name) : propertyTypeFullName;
				string fieldName = dataMetadata == null ? "this" : dataMetadata.Name;
				PersistentPropertyWithFieldCodeBase persistentInterfaceProperty = new AggregatedPersistentInterfaceDataPropertyCode(fieldName, propertyTypeFullName, propertyFieldTypeFullName, this);
				persistentInterfaceProperty.InterfaceFullName = CodeBuilder.TypeToString(persistentInterfaceType);
				persistentInterfaceProperty.IsReadOnly = true;
				AddPropertyCode(persistentInterfaceProperty);
				if(dataMetadata == null) {
					ImplementedInterfaceFullNames.Add(CodeBuilder.TypeToString(persistentInterfaceDataType));
					InstancePropertyCode instancePropertyCode = new InstancePropertyCode(CodeBuilder.TypeToString(interfaceMetadata.InterfaceType), "this", this);
					instancePropertyCode.InterfaceFullName = CodeBuilder.TypeToString(persistentInterfaceDataType);
					AddPropertyCode(instancePropertyCode);
				}
			}
		}
		protected override ConstructorCode GenerateDefaultConstructor(ClassMetadata classMetadata) {
			ConstructorCode result = base.GenerateDefaultConstructor(classMetadata);
			result.AddLogicInfo(LogicHelper.GetConstructorLogicTypes((EntityMetadata)classMetadata), (EntityMetadata)classMetadata);
			return result;
		}
		private void FillClassLogics(EntityMetadata entityMetadata) {
			FillClassLogicCodeModel(DcSpecificWords.LogicOnLoaded, DcSpecificWords.LogicOnLoaded, entityMetadata);
			FillClassLogicCodeModel(DcSpecificWords.LogicOnSaving, DcSpecificWords.LogicOnSaving, entityMetadata);
			FillClassLogicCodeModel(DcSpecificWords.LogicOnSaved, DcSpecificWords.LogicOnSaved, entityMetadata);
			FillClassLogicCodeModel(DcSpecificWords.LogicOnDeleting, DcSpecificWords.LogicOnDeleting, entityMetadata);
			FillClassLogicCodeModel(DcSpecificWords.LogicOnDeleted, DcSpecificWords.LogicOnDeleted, entityMetadata);
			GenerateLogicProperties(entityMetadata);
		}
		private void GenerateLogicProperties(EntityMetadata entityMetadata) {
			Dictionary<InterfaceMetadata, IList<DataMetadata>> interfaceInfo = GetInterfaceDataInfo(entityMetadata);
			foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(entityMetadata, true)) {
				if(LogicHelper.IsLogicRequired(propertyInfo.PropertyMetadata, propertyInfo.InterfaceMetadata, entityMetadata)) {
					PropertyCode code = GenerateLogicPropertyCodeIfNeed(propertyInfo.InterfaceMetadata, propertyInfo.PropertyMetadata, entityMetadata, propertyInfo.IsExplicit);
					if(code != null) {
						AddPropertyCode(code);
						if(!interfaceInfo.ContainsKey(propertyInfo.InterfaceMetadata)) {
							FillExtendedPropertiesNonAggregatedPropertyCode(code, propertyInfo.PropertyMetadata, propertyInfo.InterfaceMetadata);
						}
					}
				}
			}
		}
		private void FillClassLogicCodeModel(string methodName, string logicMathodName, EntityMetadata entityMetadata) {
			IList<MethodLogic> classMethodLogics = LogicHelper.GetClassLogics(logicMathodName, entityMetadata);
			if(classMethodLogics.Count > 0) {
				ClassLogicMethodCode methodCode = new ClassLogicMethodCode(methodName, logicMathodName, classMethodLogics, entityMetadata);
				AddMethodCode(methodCode);
			}
		}
		private void FillClassLogicVariables(IEnumerable<MethodLogic> methodLogics) {
			foreach(MethodLogic methodLogic in methodLogics) {
				if(!methodLogic.IsStatic) {
					string logicFieldName = DcSpecificWords.FieldLogicInstancePrefix + methodLogic.Owner.Name;
					if(CodeModelGeneratorHelper.FindFieldCode(this, logicFieldName) == null) {
						FieldCode newFieldCode = new FieldCode(logicFieldName, CodeBuilder.TypeToString(methodLogic.Owner));
						AddFieldCode(newFieldCode);
					}
				}
			}
		}
		private void FillAfterConstructionClassLogic(EntityMetadata entityMetadata) {
			IList<Type> constructorLogicTypes = LogicHelper.GetConstructorLogicTypes(entityMetadata);
			IList<MethodLogic> classMethodLogics = LogicHelper.GetClassLogics(DcSpecificWords.LogicAfterConstruction, entityMetadata);
			AfterConstructionMethodCode afterConstructionMethodCode = new AfterConstructionMethodCode(entityMetadata, classMethodLogics, constructorLogicTypes);
			AddMethodCode(afterConstructionMethodCode);
		}
		private bool CreateInstanceLogicRequired(MethodMetadata methodMetadata) {
			return methodMetadata.FindAttribute<CreateInstanceAttribute>() != null;
		}
		private PropertyCode GenerateLogicPropertyCodeIfNeed(InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata, EntityMetadata entityMetadata, bool isExplicit) {
			LogicPropertyCode result = null;
			if(LogicHelper.IsLogicRequired(propertyMetadata, interfaceMetadata, entityMetadata)) {
				bool existsInBaseClasses = LogicHelper.ExistsLogicPropertyInBaseEntities(propertyMetadata, entityMetadata);
				existsInBaseClasses = existsInBaseClasses || LogicHelper.FindBaseClassProperty(propertyMetadata, entityMetadata) != null;
				InterfaceMetadata declaringInterface = existsInBaseClasses ? entityMetadata.PrimaryInterface : interfaceMetadata;
				MethodLogic getterLogic = LogicHelper.FindOwnPropertyLogic(propertyMetadata, entityMetadata, declaringInterface, false);
				MethodLogic setterLogic = LogicHelper.FindOwnPropertyLogic(propertyMetadata, entityMetadata, declaringInterface, true);
				if(getterLogic != null) {
					PropertyLogicMethodCode getValue = new PropertyLogicMethodCode(LogicPropertyCode.GetGetterMethodName(propertyMetadata), propertyMetadata.Name, propertyMetadata.PropertyType, new ParameterCode[] { }, getterLogic);
					getValue.Virtuality = existsInBaseClasses ? Virtuality.Override : Virtuality.Virtual;
					AddMethodCode(getValue);
				}
				if(setterLogic != null) {
					PropertyLogicMethodCode setValue = new PropertyLogicMethodCode(LogicPropertyCode.GetSetterMethodName(propertyMetadata), propertyMetadata.Name, typeof(void), new ParameterCode[] { new ParameterCode("value", CodeBuilder.TypeToString(propertyMetadata.PropertyType)) }, setterLogic);
					setValue.Virtuality = existsInBaseClasses ? Virtuality.Override : Virtuality.Virtual;
					AddMethodCode(setValue);
				}
				if(!existsInBaseClasses) {
					result = new LogicPropertyCode(propertyMetadata, this, getterLogic, setterLogic);
					if(!propertyMetadata.IsPersistent) {
						bool hasNonPersistentInterface = false;
						foreach(Attribute attribute in result.Attributes) {
							if(attribute is DevExpress.Xpo.NonPersistentAttribute) {
								hasNonPersistentInterface = true;
								break;
							}
						}
						if(!hasNonPersistentInterface) {
							result.AddAttribute(new DevExpress.Xpo.NonPersistentAttribute());
						}
					}
					if(isExplicit) {
						result.InterfaceFullName = interfaceMetadata.FullName;
					}
					if(propertyMetadata.FindAttribute<PersistentAttribute>() != null) {
						DataMetadata dataMetadata = interfaceMetadata.DataClass;
						if(dataMetadata == null && entityMetadata != null) {
							foreach(InterfaceMetadata ownInterface in entityMetadata.OwnImplementedInterfaces) {
								if(ownInterface.DataClass != null) {
									foreach(InterfaceMetadata storedInterface in ownInterface.DataClass.StoredInterfaces) {
										if(interfaceMetadata == storedInterface) {
											dataMetadata = ownInterface.DataClass;
										}
									}
								}
							}
						}
						if(dataMetadata != null) {
							result.AddAttribute(new PersistentAliasAttribute(string.Format("[{0}.{1}]", CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata), propertyMetadata.Name)));
						}
					}
				}
			}
			return result;
		}
		private void FillClassMethodsCodeModel(EntityMetadata entityMetadata) {
			foreach(CodeModelMethodInfo methodInfo in CodeModelGeneratorHelper.GetClassMethodInfos(entityMetadata)) {
				FillClassMethodCodeModel(methodInfo.InterfaceMetadata, entityMetadata, methodInfo.MethodMetadata);
			}
		}
		private void FillClassMethodCodeModel(InterfaceMetadata interfaceMetadata, EntityMetadata entityMetadata, MethodMetadata methodMetadata) {
			MethodLogic methodLogic = LogicHelper.FindMethodLogic(methodMetadata, entityMetadata.PrimaryInterface, interfaceMetadata);
			MethodInfo baseClassMethodInfo = LogicHelper.FindBaseClassMethod(entityMetadata, methodMetadata);
			bool isMethodLogicExists = methodLogic != null || baseClassMethodInfo != null;
			if(methodLogic != null && baseClassMethodInfo != null) {
				throw new InvalidOperationException(string.Format("There are 2 methods found for {0}.{1} : {2}.{1} and {3}.{1}", interfaceMetadata.FullName, methodMetadata.Name, methodLogic.Owner.FullName, baseClassMethodInfo.DeclaringType.FullName));
			}
			if(!isMethodLogicExists && CreateInstanceLogicRequired(methodMetadata)) {
				Type methodReturnType = methodMetadata.ReturnType;
				IList<EntityMetadata> createInstanceEntities = LogicHelper.GetCreateInstanceEntities(methodReturnType);
				if(createInstanceEntities.Count == 0) {
					throw new InvalidOperationException(string.Format("Cannot generate the {0}.{1} method. There are no registered entities implementing the {2} interface.", interfaceMetadata.FullName, methodMetadata.Name, methodReturnType.FullName));
				}
				else if(createInstanceEntities.Count > 1) {
					throw new InvalidOperationException(string.Format("Cannot generate the {0}.{1} method. There are several registered entities implementing the {2} interface.", interfaceMetadata.FullName, methodMetadata.Name, methodReturnType.FullName));
				}
				CreateInstanceMethodCode createInstanceMethodCode = new CreateInstanceMethodCode(methodMetadata, createInstanceEntities[0].Name);
				AddMethodCode(createInstanceMethodCode);
			}
			else {
				if(!isMethodLogicExists) {
					throw new InvalidOperationException(string.Format("Logic for method {0}.{1} was not found", entityMetadata.PrimaryInterface.InterfaceType.FullName, methodMetadata.Name));
				}
				MethodLogic ownMethodLogic = LogicHelper.FindOwnMethodLogic(methodMetadata, entityMetadata, interfaceMetadata);
				if(ownMethodLogic != null) {
					MethodCode methodCode = new LogicMethodCode(methodMetadata, ownMethodLogic);
					IncludeItemAttribute includeItemAttribute = methodMetadata.FindAttribute<IncludeItemAttribute>();
					if(includeItemAttribute != null) {
						methodCode.Virtuality = includeItemAttribute.IsOverrideMethod ? Virtuality.Override : Virtuality.New;
					}
					else {
						methodCode.Virtuality = LogicHelper.ExistsLogicMethodInBaseEntities(methodMetadata, entityMetadata) ? Virtuality.Override : Virtuality.Virtual;
					}
					AddMethodCode(methodCode);
				}
			}
		}
	}
}
