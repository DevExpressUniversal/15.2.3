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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public interface IUnchangeableModelProvider {
		ModelApplicationBase GetUnchangeableModel();
	}
	public interface IApplicationModelManagerProvider {
		ApplicationModelManager GetModelManager();
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ApplicationModelManager : IUnchangeableModelProvider {
		private Type[] boModelTypes;
		private ModuleBase[] modules;
		private Controller[] controllers;
		private IXafResourceLocalizer[] resourceLocalizers;
		private EditorDescriptor[] editorDescriptors;
		private ModelApplicationCreator creator;
		private ModelApplicationBase unchangeableLayer;
		private static ModelApplicationCreatorProperties CreateModelApplicationCreatorProperties(IEnumerable<ModuleBase> modules, IEnumerable<Controller> controllers, string assemblyFileAbsolutePath) {
			ModelApplicationCreatorProperties properties = ModelApplicationCreatorProperties.CreateDefault();
			ModelInterfaceExtenders interfacesExtenders = properties.InterfacesExtenders;
			CustomLogics customLogics = properties.CustomLogics;
			foreach(ModuleBase module in modules) {
				module.ExtendModelInterfaces(interfacesExtenders);
				module.CustomizeLogics(customLogics);
			}
			foreach(Controller controller in controllers) {
				IModelExtender extendModel = controller as IModelExtender;
				if(extendModel != null) {
					extendModel.ExtendModelInterfaces(interfacesExtenders);
				}
			}
			properties.AssemblyFileAbsolutePath = assemblyFileAbsolutePath;
			return properties;
		}
		private static void SetupModelApplicationCreator(ModelApplicationCreator creator, IEnumerable<ModuleBase> modules, IEnumerable<string> aspects) {
			ModelNodesGeneratorUpdaters updaters = new ModelNodesGeneratorUpdaters();
			foreach(ModuleBase module in modules) {
				module.AddGeneratorUpdaters(updaters);
				module.AddModelNodeValidators(creator);
				module.AddModelNodeUpdaters(creator);
				if(module is IModelXmlConverter) {
					creator.AddOnAddNodeFromXmlDelegate(((IModelXmlConverter)module).ConvertXml);
				}
			}
			creator.AddNodesGeneratorUpdaters(updaters);
			creator.AddAspects(aspects);
		}
		private static ModelStoreBase[] CollectModelStores(IEnumerable<ModuleBase> modules) {
			List<ModelStoreBase> result = new List<ModelStoreBase>();
			foreach(ModuleBase module in modules) {
				result.Add(module.DiffsStore);
			}
			return result.ToArray();
		}
		private static string[] CollectAspects(IEnumerable<string> aspects, ModelStoreBase[] modelDifferenceStores, ModelStoreBase applicationModelDifferenceStore) {
			List<string> result = new List<string>(aspects);
			foreach(ModelStoreBase modelDifferenceStore in modelDifferenceStores) {
				foreach(string aspect in modelDifferenceStore.GetAspects()) {
					if(!result.Contains(aspect)) {
						result.Add(aspect);
					}
				}
			}
			if(applicationModelDifferenceStore != null) {
				foreach(string aspect in applicationModelDifferenceStore.GetAspects()) {
					if(!result.Contains(aspect)) {
						result.Add(aspect);
					}
				}
			}
			return result.ToArray();
		}
		private static Type[] CollectBOModelTypes(ITypesInfo typesInfo, IEnumerable<Type> boModelTypes) {
			IEnumerable<Type> list1 = GetVisibleAndRequiredDomainComponents(typesInfo, boModelTypes);
			IEnumerable<Type> list2 = GetDomainComponentsWithInterfaces(list1);
			return new List<Type>(list2).ToArray();
		}
		private static IEnumerable<Type> GetVisibleAndRequiredDomainComponents(ITypesInfo typesInfo, IEnumerable<Type> allDomainComponents) {	
			HashSet<Type> visibleAndRequiredDomainComponents = new HashSet<Type>();
			foreach(Type type in allDomainComponents) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				if(typeInfo.IsVisible) {
					if(!visibleAndRequiredDomainComponents.Contains(type)) {
						visibleAndRequiredDomainComponents.Add(type);
						foreach(ITypeInfo requiredTypeInfo in typeInfo.GetRequiredTypes(info => info.IsDomainComponent)) {
							visibleAndRequiredDomainComponents.Add(requiredTypeInfo.Type);
						}
					}
				}
			}
			return visibleAndRequiredDomainComponents;
		}
		private static IEnumerable<Type> GetDomainComponentsWithInterfaces(IEnumerable<Type> list) {
			HashSet<Type> set = new HashSet<Type>();
			foreach(Type type in list) {
				if(!set.Contains(type)) {
					set.Add(type);
					if(type.IsInterface) {
						set.UnionWith(type.GetInterfaces());
					}
				}
			}
			return set;
		}
		private static EditorDescriptor[] CollectEditorDescriptors(IEnumerable<ModuleBase> modules) {
			List<EditorDescriptor> list = new List<EditorDescriptor>();
			foreach(ModuleBase module in modules) {
				module.RegisterEditorDescriptors(list);
			}
			return list.ToArray();
		}
		private static IXafResourceLocalizer[] CollectResourceLocalizers(IEnumerable<Type> applicationLocalizerTypes, IEnumerable<ModuleBase> modules) {
			HashSet<Type> types = new HashSet<Type>(ExceptionLocalizer.ExceptionLocalizers);
			types.UnionWith(applicationLocalizerTypes);
			foreach(ModuleBase module in modules) {
				foreach(Type item in module.ResourcesExportedToModel) {
					types.Add(item);
				}
			}
			List<IXafResourceLocalizer> result = new List<IXafResourceLocalizer>();
			foreach(Type type in types) {
				if(typeof(IXafResourceLocalizer).IsAssignableFrom(type)) {
					IXafResourceLocalizer localizer = (IXafResourceLocalizer)TypeHelper.CreateInstance(type);
					result.Add(localizer);
				}
			}
			return result.ToArray();
		}
		private static void InitImageLoader(IModelApplication model) {  
			Guard.ArgumentNotNull(model, "model");
			lock(ImageLoader.Instance) {
				if(!ImageLoader.IsInitialized) {
					ImageSource[] imageSources = new ImageSource[model.ImageSources.NodeCount];
					int index = 0;
					for(int i = 0; i < model.ImageSources.Count; i++) {
						IModelImageSource node = model.ImageSources[i];
						if(node is IModelFileImageSource) {
							imageSources[index] = new FileImageSource(node.Folder);
							index++;
						}
						if(node is IModelAssemblyResourceImageSource) {
							imageSources[index] = new AssemblyResourceImageSource(
								((IModelAssemblyResourceImageSource)node).AssemblyName,
								node.Folder);
							index++;
						}
					}
					ImageLoader.Init(imageSources);
				}
			}
		}
		private static void UpdateModulesVersion(ModelApplicationBase model, IEnumerable<ModuleBase> modules) {
			IModelApplication modelApplication = model as IModelApplication;
			if(modelApplication != null) {
				IModelSchemaModules schemaModules = modelApplication.SchemaModules;
				if(schemaModules == null) {
					schemaModules = model.AddNode<IModelSchemaModules>("SchemaModules");
				}
				foreach(ModuleBase module in modules) {
					if(!String.IsNullOrEmpty(module.Name)) {
						IModelSchemaModule schemaModule = schemaModules[module.Name];
						if(schemaModule == null && module is IModelXmlConverter) {
							schemaModule = schemaModules.AddNode<IModelSchemaModule>(module.Name);
							schemaModule.SetValue<bool>(ModelValueNames.IsNewNode, true);
						}
						if(schemaModule != null) {
							schemaModule.Version = module.Version.ToString();
						}
					}
				}
			}
		}
		private ModelApplicationBase CreateUnchangeableLayer(ModelStoreBase[] modelDifferenceStores) {
			ModelApplicationBase result = CreateLayer(ModelApplicationLayerIds.UnchangedMasterPart);
			ModelApplicationBase generatorLayer = CreateLayer(ModelApplicationLayerIds.Generator);
			result.AddLayerInternal(generatorLayer);
			foreach(ModelStoreBase store in modelDifferenceStores) {
				ModelApplicationBase moduleLayer = CreateLayerByStore(null, store);
				result.AddLayerInternal(moduleLayer);
			}
			return result;
		}
		private void InitializeModelSources(IModelSources sources) {
			sources.BOModelTypes = boModelTypes;
			sources.Modules = modules;
			sources.Controllers = controllers;
			sources.Localizers = resourceLocalizers;
			sources.EditorDescriptors = new EditorDescriptors(editorDescriptors);
		}
		protected internal static IModelApplication RecreateModel(IModelApplication model, ModelStoreBase userDifferencesStore) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(userDifferencesStore, "userDifferencesStore");
			ModelApplicationBase node = (ModelApplicationBase)model;
			ModelApplicationCreator creator = node.CreatorInstance;
			ModelApplicationBase lastLayer = node.LastLayer;
			ModelApplicationBase userLayer = creator.CreateModelApplication();
			userLayer.Id = lastLayer.Id;
			((IModelSources)userLayer).BOModelTypes = ((IModelSources)lastLayer).BOModelTypes;
			((IModelSources)userLayer).Modules = ((IModelSources)lastLayer).Modules;
			((IModelSources)userLayer).Controllers = ((IModelSources)lastLayer).Controllers;
			((IModelSources)userLayer).Localizers = ((IModelSources)lastLayer).Localizers;
			((IModelSources)userLayer).EditorDescriptors = ((IModelSources)lastLayer).EditorDescriptors;
			userDifferencesStore.Load(userLayer);
			UpdateModulesVersion(userLayer, ((IModelSources)userLayer).Modules);
			node.RemoveLayerInternal(lastLayer);  
			node.AddLayerInternal(userLayer);
			return (IModelApplication)node;
		}
		protected internal void DropModel(ModelApplicationBase model) {
			Guard.Assert(creator != null, "ApplicationModelManager must be set up");
			Guard.Assert(model != null, "'model' must not be null");
			Guard.Assert(model != unchangeableLayer, "'model' must not be unchangeable model");
			while(model.LayersCount > 0) {
				model.RemoveLayerInternal(model.LastLayer);
			}
		}
		protected virtual void AddCustomMembersToTypeInfo(IModelApplication model) {
			AddCustomMembersFromModelToTypeInfo(model);
		}
		public ApplicationModelManager() {
		}
		public void Setup(ITypesInfo typesInfo, IEnumerable<Type> boModelTypes, IEnumerable<ModuleBase> modules, IEnumerable<Controller> controllers, IEnumerable<Type> applicationLocalizerTypes, IEnumerable<string> applicationAspects, ModelStoreBase applicationModelDifferenceStore, string modelAssemblyFile) {
			Guard.Assert(creator == null, "ApplicationModelManager is already set up");
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(boModelTypes, "boModelTypes");
			Guard.ArgumentNotNull(modules, "modules");
			Guard.ArgumentNotNull(controllers, "controllers");
			Guard.ArgumentNotNull(applicationLocalizerTypes, "applicationLocalizerTypes");
			Guard.ArgumentNotNull(applicationAspects, "applicationAspects");
			ModelApplicationCreatorProperties properties = CreateModelApplicationCreatorProperties(modules, controllers, modelAssemblyFile);
			creator = ModelApplicationCreator.GetModelApplicationCreator(properties);
			ModelStoreBase[] modelDifferenceStores = CollectModelStores(modules);
			String[] allAspects = CollectAspects(applicationAspects, modelDifferenceStores, applicationModelDifferenceStore);
			SetupModelApplicationCreator(creator, modules, allAspects);
			this.boModelTypes = CollectBOModelTypes(typesInfo, boModelTypes);
			this.modules = new List<ModuleBase>(modules).ToArray();
			this.controllers = new List<Controller>(controllers).ToArray();
			this.editorDescriptors = CollectEditorDescriptors(modules);
			this.resourceLocalizers = CollectResourceLocalizers(applicationLocalizerTypes, modules);
			unchangeableLayer = CreateUnchangeableLayer(modelDifferenceStores);
			IModelApplication model = (IModelApplication)unchangeableLayer;
			AddCustomMembersToTypeInfo(model);
			if(applicationModelDifferenceStore != null) {
				ModelApplicationBase applicationLayer = CreateLayerByStore(null, applicationModelDifferenceStore);
				unchangeableLayer.AddLayerInternal(applicationLayer);
				AddCustomMembersToTypeInfo(model);
			}
			InitImageLoader(model);
		}
		public ModelApplicationBase CreateLayer(string id) {
			Guard.Assert(creator != null, "ApplicationModelManager must be set up");
			ModelApplicationBase result = creator.CreateModelApplication();
			result.Id = id;
			InitializeModelSources(result);
			return result;
		}
		public ModelApplicationBase CreateLayerByStore(string id, ModelStoreBase store) {   
			Guard.ArgumentNotNull(store, "store");
			ModelApplicationBase model = CreateLayer(id ?? store.Name);
			store.Load(model);
			UpdateModulesVersion(model, modules);
			return model;
		}
		public ModelApplicationBase CreateModelApplication(IEnumerable<ModelApplicationBase> layers) {
			Guard.Assert(creator != null, "ApplicationModelManager must be set up");
			Guard.Assert(layers != null, "'layers' must not be null");
			ModelApplicationBase model = CreateLayer(ModelApplicationLayerIds.Application);
			model.AddLayerInternal(unchangeableLayer);
			foreach(ModelApplicationBase layer in layers) {
				Guard.Assert(layer != null, "'layer' must not be null");
				model.AddLayerInternal(layer);
			}
			AddCustomMembersToTypeInfo((IModelApplication)model);
			return model;
		}
		public static void AddCustomMembersFromModelToTypeInfo(IModelApplication model) {
			Guard.ArgumentNotNull(model, "model");
			foreach(IModelClass modelClass in model.BOModel) {
				foreach(IModelMember modelMember in modelClass.OwnMembers) {
					if(modelMember.IsCustom) {
						try {
							TypeInfo typeInfo = (TypeInfo)modelClass.TypeInfo;
							lock(typeInfo) {
								IMemberInfo memberInfo = typeInfo.FindMember(modelMember.Name);
								String expression = modelMember.IsCalculated ? modelMember.Expression : "";
								if(memberInfo == null) {
									memberInfo = typeInfo.CreateMember(modelMember.Name, modelMember.Type, expression);
									if(memberInfo != null) {
										if(modelMember.Size != 0) {
											memberInfo.AddAttribute(new FieldSizeAttribute(modelMember.Size));
										}
										modelMember.SetValue<IMemberInfo>("MemberInfo", memberInfo);
									}
								}
								else {
									if(memberInfo.IsCustom) {
										typeInfo.UpdateMember(memberInfo, modelMember.Type, expression);
									}
									else {
										throw new Exception(string.Format("The '{0}' member is already declared.", memberInfo.Name));
									}
								}
							}
						}
						catch(Exception e) {
							throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
								modelMember.Type,
								modelClass.Name,
								modelMember.Name,
								e.Message));
						}
					}
				}
			}
		}
		ModelApplicationBase IUnchangeableModelProvider.GetUnchangeableModel() {
			Guard.Assert(creator != null, "ApplicationModelManager must be set up");
			return unchangeableLayer;
		}
#if DebugTest
		public ModelApplicationCreator DebugTest_GetCreator() {
			Guard.Assert(creator != null, "ApplicationModelManager must be set up");
			return creator;
		}
#endif
	}
}
