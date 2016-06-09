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
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using DevExpress.Data.Helpers;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.CodeConverter;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.Native;
using System.ComponentModel.Design;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Internal.EntityFrameworkWrappers;
using System.Diagnostics;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	class ManyToManyInfo {
		public EntityTypeRuntimeWrapper ThisEntity { get; set; }
		public EntityTypeRuntimeWrapper OtherEntity { get; set; }
		public IEnumerable<EdmPropertyRuntimeWrapper> ThisEntityJunctionForeignKey { get; set; }
		public IEnumerable<EdmPropertyRuntimeWrapper> OtherEntityJunctionForeignKey { get; set; }
		public EntityTypeRuntimeWrapper ExplicitJunctionEntity { get; set; }
	}
	public class TemplatesCodeGen : ITemplatesCodeGen {
		const string STR_GeneratingFiles = "Generating files";
		const string STR_AddingReferences = "Adding references";
		public const string STR__WinUI = "_WinUI";
		public const string STR__OutlookUI = "_OutlookUI";
		const string STR_DevExpressXpfMvvm = AssemblyInfo.SRAssemblyXpfMvvm;
		const string STR_PresentationFramework = "PresentationFramework";
		const string STR_PresentationCore = "PresentationCore";
		const string STR_SystemDataEntity = "System.Data.Entity";
		const string STR_DataAnnotations = "System.ComponentModel.DataAnnotations";
		const string STR_XamlFileExt = "xaml";
		const string STR_ResxFileExt = "resx";
		const string STR_DesignerFileExt = "Designer";
		public static bool AlwaysGenerateFullFeaturedCommon = false;
		string[] viewReferences = new string[] {
			"WindowsBase", "System.Xaml", "System.Drawing", "System.Windows.Forms", "UIAutomationProvider", "UIAutomationType",
			AssemblyInfo.SRAssemblyPrintingCore,
			AssemblyInfo.SRAssemblyXpfCore,
			AssemblyInfo.SRAssemblyXpfDocking,
			AssemblyInfo.SRAssemblyXpfGrid,
			AssemblyInfo.SRAssemblyXpfGridCore,
			AssemblyInfo.SRAssemblyXpfLayoutCore,
			AssemblyInfo.SRAssemblyXpfLayoutControl,
			AssemblyInfo.SRAssemblyXpfNavBar,
			AssemblyInfo.SRAssemblyXpfPrinting,
			AssemblyInfo.SRAssemblyXpfRibbon,
			AssemblyInfo.SRAssemblyXpfControls,
			AssemblyInfo.SRAssemblyXpfReportDesigner
		};
		string[] viewWinReferences = new string[] {"System.Drawing", "System.Windows.Forms",
			AssemblyInfo.SRAssemblyBonusSkins, AssemblyInfo.SRAssemblyData,
			AssemblyInfo.SRAssemblyMapCore,AssemblyInfo.SRAssemblyMvvm,
			AssemblyInfo.SRAssemblyBars, AssemblyInfo.SRAssemblyEditors,
			AssemblyInfo.SRAssemblyGrid, AssemblyInfo.SRAssemblyLayoutControl,
			AssemblyInfo.SRAssemblyNavBar, AssemblyInfo.SRAssemblyPrinting,
			AssemblyInfo.SRAssemblyUtils, AssemblyInfo.SRAssemblyPrintingCore };
		string[] viewModelReferences = new string[] {
			AssemblyInfo.SRAssemblyData, 
			AssemblyInfo.SRAssemblyPrintingCore,
			STR_DevExpressXpfMvvm,
			STR_PresentationFramework, 
			STR_PresentationCore, 
			STR_SystemDataEntity, 
			STR_DataAnnotations
		};
		readonly IServiceContainer serviceContainer;
		TemplatesPaths templatesPaths;
		public TemplatesCodeGen(IServiceContainer serviceContainer) {
			this.serviceContainer = serviceContainer;
		}
		protected IServiceContainer ServiceContainer { get { return serviceContainer; } }
		protected IDataAccessLayerService DataAccesLayerService { get { return (IDataAccessLayerService)serviceContainer.GetService(typeof(IDataAccessLayerService)); } }
		protected IViewModelLayerService ViewModelLayerService { get { return (IViewModelLayerService)this.serviceContainer.GetService(typeof(IViewModelLayerService)); } }
		protected ISolutionTypesProvider SolutionTypesProvider { get { return (ISolutionTypesProvider)this.serviceContainer.GetService(typeof(ISolutionTypesProvider)); } }
		protected IUndoManager UndoManager { get { return (IUndoManager)this.serviceContainer.GetService(typeof(IUndoManager)); } }
		DevExpress.Design.Mvvm.Wizards.UI.IWizardTaskManager TaskManager { get { return (DevExpress.Design.Mvvm.Wizards.UI.IWizardTaskManager)this.serviceContainer.GetService(typeof(DevExpress.Design.Mvvm.Wizards.UI.IWizardTaskManager)); } }
		protected ITemplatesPlatform TemplatesPlatform {
			get {
				return (ITemplatesPlatform)serviceContainer.GetService(typeof(ITemplatesPlatform));
			}
		}
		protected virtual ITemplatesPlatform GetTemplatesPlatform() {
			return new DTETemplatesPlatform(this.serviceContainer);
		}
		TemplatesPaths TemplatesPaths {
			get {
				if(templatesPaths == null)
					templatesPaths = GetTemplatesPaths();
				return templatesPaths;
			}
		}
		protected virtual TemplatesPaths GetTemplatesPaths() {
			return new TemplatesPaths();
		}
		void CreateDirectoryIfNeeded(string name) {
			if(!string.IsNullOrEmpty(name) && !Directory.Exists(name))
				Directory.CreateDirectory(name);
		}
		void GenerateEntityDataModel(TemplateGenerationContext context, EntityModelData entityModelData, IDbContainerInfo container, Dictionary<Type, string> namespaces, bool async) {
			if(container == null || namespaces == null)
				return;
			T4TemplateInfo templateInfo = new T4TemplateInfo();
			templateInfo.Properties[TemplatesConstants.STR_TemplateGenerationContext] = context;
			templateInfo.Properties[TemplatesConstants.STR_EntityModelData] = entityModelData;
			templateInfo.Properties[TemplatesCodeGen.STR_UnitOfWorkHooks] = GetUnitOfWorkHooks();
			AddAssemblyReferences(async, false, AssemblyInfo.SRAssemblyData);
			AddAssemblyReferences(async, false, AssemblyInfo.SRAssemblyMvvm);
			if(container != null && container.Assembly != null)
				AddAssemblyReferences(async, true, this.SolutionTypesProvider.GetAssemblyReferencePath(container.Assembly.AssemblyFullName, Constants.EntityFrameworkAssemblyName));
			string activeFileExt = TemplatesPlatform.GetFileExtensionForActiveProject();
			try {
				IEnumerable<Type> dataModelResources = TemplatesResources.GetDataModelResources(context, container.ContainerType, context.WithoutDesignTime);
				foreach(Type templateType in dataModelResources) {
					if(!TemplatesPaths.IsCommon(templateType)) {
						templateInfo.UsingList = Merge(Enumerable.Concat(templateInfo.UsingList, new string[] { container.NamespaceName }),
							entityModelData.Entities.Select<EntitySetData, string>(es => es.TypeNamespace));
					}
					if(templateType == typeof(Resources.Common.DataModel.IRepositoryTemplate))
						templateInfo.UsingList = new List<string>() { TemplatesPaths.GetCommonUtilsNamespace(TemplatesPlatform) };
					ExpandDataModelTemplate(async, templateInfo, templateType, namespaces, fileName => fileName + "." + activeFileExt);
				}
				GenerateCustomToolItemWithCodeBehind<Resources.Common.CommonResourcesTemplate, Resources.Common.CommonResources_DesignerTemplate>(
					"CommonResources", STR_ResxFileExt, "ResXFileCodeGenerator", async);
				GenerateCustomToolItemWithCodeBehind<Resources.Common.LayoutSettingsTemplate, Resources.Common.LayoutSettings_DesignerTemplate>(
					"LayoutSettings", "settings", "SettingsSingleFileGenerator", async);
				ExpandDataModelTemplate(async, templateInfo, typeof(Resources.DataModel.UnitOfWorkTemplate), namespaces, fileName => String.Format("I{0}UnitOfWork.{1}", entityModelData.Name, activeFileExt));
				ExpandDataModelTemplate(async, templateInfo, typeof(Resources.DataModel.UnitOfWorkRuntimeTemplate), namespaces, fileName => String.Format("{0}UnitOfWork.{1}", entityModelData.Name, activeFileExt));
				if(!context.WithoutDesignTime)
					ExpandDataModelTemplate(async, templateInfo, typeof(Resources.DataModel.UnitOfWorkDesignTimeTemplate), namespaces, fileName => String.Format("DesignTime{0}UnitOfWork.{1}", entityModelData.Name, activeFileExt));
			} catch(Exception ex) {
				Log.SendException(ex);
				return;
			}
		}
		void GenerateCustomToolItemWithCodeBehind<TPrimaryTemplate, TCodeBehindTemplate>(string name, string fileExtension, string customToolName, bool async) {
			try {
				T4TemplateInfo templateInfo = new T4TemplateInfo();
				string targetDirectory = TemplatesPaths.GetTemplateTargetDirectory(TemplatesPlatform, typeof(TPrimaryTemplate), templateInfo, null, UIType.None);
				string fileName;
				GetUniqueItemName(targetDirectory, name, fileExtension, out fileName);
				var properties = new Dictionary<string, string>();
				properties["CustomTool"] = customToolName;
				GenerateDependentFiles(fileName, targetDirectory, false, typeof(TPrimaryTemplate), typeof(TCodeBehindTemplate), async, templateInfo, STR_DesignerFileExt, properties);
			} catch(Exception ex) {
				Log.SendException(ex);
			}
		}
		void ExpandEntityTemplate(T4TemplateInfo info, Type templateType, bool async, Func<string> formatFileName, string folderName, UIType uiType) {
			string targetFolder = TemplatesPaths.GetTemplateTargetDirectory(TemplatesPlatform, templateType, info, folderName, uiType);
			info.UsingList = Merge(info.UsingList, null, info.GetProperty(TemplatesConstants.STR_Namespace) as string);
			ExpandTemplate(info, templateType, formatFileName(), targetFolder, TemplatesPaths.IsCommon(templateType), async);
		}
		void ExpandDataModelTemplate(bool async, T4TemplateInfo templateInfo, Type templateType, Dictionary<Type, string> namespaces, Func<string, string> formatFileName) {
			T4TemplateInfo templateInfo_Cloned = templateInfo.Clone();
			string targetFolder = TemplatesPaths.GetTemplateTargetDirectory(TemplatesPlatform, templateType, templateInfo_Cloned, null, UIType.None);
			namespaces[templateType] = templateInfo_Cloned.GetProperty(TemplatesConstants.STR_Namespace) as string;
			UpdateUsingList(templateInfo_Cloned, templateInfo);
			ExpandTemplate(templateInfo_Cloned, templateType, formatFileName(templateType.Name.Replace("Template", string.Empty)), targetFolder, TemplatesPaths.IsCommon(templateType), async);
			UpdateUsingList(templateInfo, templateInfo_Cloned);
		}
		void UpdateUsingList(T4TemplateInfo destination, T4TemplateInfo source) {
			if(source == null || destination == null)
				return;
			string destinationNamespace = destination.GetProperty(TemplatesConstants.STR_Namespace) as string;
			string sourceNamespace = source.GetProperty(TemplatesConstants.STR_Namespace) as string;
			IEnumerable<string> set = Enumerable.Concat(source.UsingList, new string[] { sourceNamespace });
			destination.UsingList = Merge(destination.UsingList, set, destinationNamespace);
		}
		List<string> Merge(IEnumerable<string> list1, IEnumerable<string> list2, string exclude = null) {
			list2 = list2 ?? Enumerable.Empty<string>();
			return new HashSet<string>(list1.Concat(list2).Except(new[] { exclude, null })).ToList();
		}
		protected virtual string GetUniqueItemName(string folder, string name, string ext, out string fileName) {
			string uniqueName = name;
			fileName = Path.ChangeExtension(uniqueName, ext);
			if(!Directory.Exists(folder) || TemplatesPlatform.RewriteExistingFiles)
				return uniqueName;
			int i = 1;
			while(IsFileExistCore(folder, fileName) || TemplatesPlatform.IsItemExist(uniqueName)) {
				uniqueName = name + i;
				fileName = Path.ChangeExtension(uniqueName, ext);
				i++;
			}
			return uniqueName;
		}
		static bool IsFileExistCore(string folder, string fileName) {
			try {
				string[] files = Directory.GetFiles(folder, fileName, SearchOption.AllDirectories);
				return files != null && files.Length != 0;
			} catch(Exception ex) {
				Log.SendException(ex);
				return true;
			}
		}
		protected virtual void WriteToFile(string fileName, string result) {
			File.WriteAllText(fileName, result, Encoding.UTF8); 
		}
		void ExpandTemplate(T4TemplateInfo templateInfo, Type templateType, string baseFileName, string folder, bool isCommon, bool async, bool rebuildNeeded = false) {
			try {
				string fileName = Path.Combine(folder, Path.GetFileName(baseFileName));
				DoAsyncIfNeeded(async, STR_GeneratingFiles, () => {
					if(File.Exists(fileName) && isCommon && TemplatesPlatform.IsItemExist(fileName))
						return;
					string result = TextTemplatingHelper.ExpandTemplate(templateType, templateInfo);
					result = ConvertToVBIfNeeded(result, true, fileName);
					CreateDirectoryIfNeeded(Path.GetDirectoryName(fileName));
					WriteToFile(fileName, result);
					TemplatesPlatform.AddFromFile(fileName, rebuildNeeded);
				});
				return;
			} catch(Exception ex) {
#if DEBUGTEST
				throw ex;
#else
				Log.SendException(ex);
#endif
			}
		}
		struct Replacement {
			public string FileName;
			public string Old;
			public string New;
		}
		string ConvertToVBIfNeeded(string code, bool checkForGlobal, string fileName) {
			if(!TemplatesPlatform.IsVisualBasic)
				return code;
			IEnumerable<string> modes = null;
			if(checkForGlobal && !string.IsNullOrEmpty(TemplatesPlatform.GetProjectNamespace()))
				modes = new string[] { "", "MakeGlobalNamespace" };
			var res = LanguageContainerService.ConvertCsToVb(code, new ConvertArguments(modes));
			var replacements = new[] {
				new Replacement {
					FileName = "DbRepository.vb",
					Old = "Private Function Create(ByVal add As Boolean) As TEntity Implements IRepository(Of TEntity, TPrimaryKey).Create",
					New = "Private Function Create(Optional ByVal add As Boolean = True) As TEntity Implements IRepository(Of TEntity, TPrimaryKey).Create"
				},
				new Replacement {
					FileName = "DesignTimeRepository.vb",
					Old = "Private Function Create(ByVal add As Boolean) As TEntity Implements IRepository(Of TEntity, TPrimaryKey).Create",
					New = "Private Function Create(Optional ByVal add As Boolean = True) As TEntity Implements IRepository(Of TEntity, TPrimaryKey).Create"
				},
				new Replacement {
					FileName = "UnitOfWorkFactory.vb",
					Old = "Of TEntity, TProjection, TPrimaryKey",
					New = "Of TEntity As { Class, New }, TProjection As Class, TPrimaryKey"
				},
				new Replacement {
					FileName = "DbUnitOfWorkFactory.vb",
					Old = "source.GetSource += Sub(ByVal s, ByVal e)",
					New = "AddHandler source.GetSource, Sub(s, e)"
				}
			};
			foreach(var replacement in replacements) {
				if(fileName.EndsWith(replacement.FileName))
					res = res.Replace(replacement.Old, replacement.New);
			}
			if(fileName.EndsWith("ViewModel.vb")) {
				var what = "inptLookUp(.*?)\\.PropertyChanged \\+= ReloadLookUp(.*?)EntityHandler";
				var with = "AddHandler inptLookUp$2.PropertyChanged, AddressOf ReloadLookUp$2EntityHandler";
				res = new Regex(what).Replace(res, with);
			}
			return res;
		}
		void GenerateViewModel(TemplateGenerationContext context, bool async, string viewModelName, string folderName, Type templateType, T4TemplateInfo templateInfo, bool skipGeneration = false) {
			if(templateInfo == null)
				return;
			templateInfo.Properties[TemplatesCodeGen.STR_CollectionViewModelHooks] = GetCollectionViewModelHooks();
			templateInfo.Properties[TemplatesConstants.STR_TemplateGenerationContext] = context;
			bool isCommon = TemplatesPaths.IsCommon(templateType);
			string targetFolder = TemplatesPaths.GetTemplateTargetDirectory(TemplatesPlatform, templateType, templateInfo, folderName, UIType.None);
			string fileName;
			templateInfo.Properties[STR_DebugLogHooks] = GetDebugLogHooks();
			if(isCommon)
				fileName = Path.Combine(targetFolder, templateType.Name.Replace("Template", ".") + TemplatesPlatform.GetFileExtensionForActiveProject());
			else
				templateInfo.Properties[TemplatesConstants.STR_ViewModelName] = GetUniqueItemName(targetFolder, viewModelName, TemplatesPlatform.GetFileExtensionForActiveProject(), out fileName);
			if(!skipGeneration)
				ExpandTemplate(templateInfo, templateType, fileName, targetFolder, isCommon, async, true);
			UpdateUsingList(templateInfo, templateInfo);
		}
		IViewModelInfo GetGeneratedViewModelTypeInfo(T4TemplateInfo templateInfo, ViewModelType viewModelType, IEntitySetInfo entity, IEnumerable<IEntitySetInfo> selectedEntities) {
			string nameSpace = templateInfo.Properties[TemplatesConstants.STR_Namespace].ToString();
			string viewModelName = templateInfo.Properties[TemplatesConstants.STR_ViewModelName].ToString();
			string assemblyName = SolutionTypesProvider.ActiveProjectTypes.ProjectAssembly.AssemblyFullName;
			EntitySetData entitySetData = templateInfo.Properties[TemplatesConstants.STR_EntitySetData] as EntitySetData;
			if(viewModelType == ViewModelType.Entity) {
				List<LookUpCollectionViewModelData> lookUpTables = new List<LookUpCollectionViewModelData>();
				foreach(EntitySetData lookUp in entitySetData.LookUpTables) {
					string lookUpViewModelName = entitySetData.Type + lookUp.Name + "ViewModel";
					IEntitySetInfo lookUpInfo = selectedEntities.FirstOrDefault(x => x.ElementType.Type.FullName == lookUp.TypeFullName);
					if(lookUpInfo == null)
						continue;
					LookUpCollectionViewModelData item = ViewModelDataSource.GetGeneratedLookUpCollectionViewModelData(assemblyName, lookUpViewModelName, nameSpace, lookUp.ElementType, lookUpInfo.ShouldGenerateReadOnlyView(), lookUpInfo.ElementType.AllProperties, null, lookUp.PropertyName);
					if(item != null)
						lookUpTables.Add(item);
				}
				EntityViewModelData entityViewModelData = ViewModelDataSource.GetGeneratedEntityViewModelData(assemblyName, viewModelName, nameSpace, entity.ElementType.Type, entity.ElementType, GetCollectionProperties(entity, selectedEntities), lookUpTables.ToArray(), GetForeignKeyInfoCallback(entity));
				return entityViewModelData;
			}
			if(viewModelType == ViewModelType.EntityRepository) {
				return ViewModelDataSource.GetGeneratedCollectionViewModelData(assemblyName, viewModelName, nameSpace, entity.ElementType.Type, entity.ShouldGenerateReadOnlyView(), entity.ElementType.AllProperties,null, GetForeignKeyInfoCallback(entity), GetCollectionProperties(entity, selectedEntities));
			}
			return null;
		}
		static IEnumerable<TypeNamePropertyPair> GetCollectionProperties(IEntitySetInfo entity, IEnumerable<IEntitySetInfo> selectedEntities) {
			return EntitySetData.GetFkSets(entity, selectedEntities).Select(x => new TypeNamePropertyPair(x.ElementType.Type.FullName, "LookUp" + x.Name + ".Entities"));
		}
		public static Func<IEdmPropertyInfo, ForeignKeyInfo> GetForeignKeyInfoCallback(IEntitySetInfo entity) {
			return property => {
				IEdmPropertyInfo foreignKeyProperty = entity.ElementType.GetForeignKey(property);
				if(foreignKeyProperty != null) {
					IEntitySetInfo associationSet = entity.EntityContainerInfo.EntitySets.FirstOrDefault(x => x.ElementType.Type == property.PropertyType);
					if(associationSet == null)
						return null;
					IEdmPropertyInfo primaryKeyProperty = associationSet.ElementType.KeyMembers.FirstOrDefault();
					if(primaryKeyProperty != null)
						return new ForeignKeyInfo(foreignKeyProperty.Name, primaryKeyProperty.Name);
				}
				return null;
			};
		}
		public IDataModel GenerateEntityDataModel(TemplateGenerationContext context, IDbContainerInfo container, bool async) {
			try {
				Dictionary<Type, string> namespaceDictionary = new Dictionary<Type, string>();
				GenerateEntityDataModel(context, EntityModelData.Create(container, context.SelectedTables, context.WithoutDesignTime), container, namespaceDictionary, async);
				if(container == null || context.SelectedTables == null)
					return null;
				string utilsNamespace = namespaceDictionary[typeof(Resources.Common.Utils.ExpressionHelperTemplate)];
				string dataModelNamespace = namespaceDictionary[typeof(Resources.DataModel.UnitOfWorkSourceTemplate)];
				string commonDataModelNamespace = namespaceDictionary[typeof(Resources.Common.DataModel.IUnitOfWorkTemplate)];
				IDataModel generatedDataModel = DataModel.DataModel.Create(container, context.SelectedTables, dataModelNamespace, commonDataModelNamespace, utilsNamespace);
				if(generatedDataModel != null) {
					DataAccesLayerService.Register(generatedDataModel);
					SolutionTypesProvider.Add(generatedDataModel.UnitOfWorkSource);
					UndoManager.Add(new SimpleActionUndoUnit(() => {
						DataAccesLayerService.Remove(generatedDataModel.UnitOfWorkSource);
						SolutionTypesProvider.Remove(generatedDataModel.UnitOfWorkSource);
					}));
				}
				return generatedDataModel;
			} catch(Exception ex) {
				Log.SendException(ex);
				return null;
			}
		}
		bool IsParamsValid(string viewModelName, IDataModel dataModel, IEntitySetInfo entity) {
			if(string.IsNullOrEmpty(viewModelName)) {
				Log.SendWarning("View Model Name is not defined.");
				return false;
			}
			if(dataModel == null || !dataModel.IsValid) {
				Log.SendWarning("DataModel is not defined.");
				return false;
			}
			if(entity == null) {
				Log.SendWarning("EntitySet is not defined.");
				return false;
			}
			return true;
		}
		string GetEntityFrameworkReference(IDataModel dataModel) {
			if(dataModel == null || dataModel.DbContainer == null || dataModel.DbContainer.Assembly == null)
				return null;
			return this.SolutionTypesProvider.GetAssemblyReferencePath(dataModel.DbContainer.Assembly.AssemblyFullName, Constants.EntityFrameworkAssemblyName);
		}
		T4TemplateInfo GetTemplateInfoForViewModel(TemplateGenerationContext context, IDataModel dataModel, IEntitySetInfo entity, bool isCommon) {
			T4TemplateInfo templateInfo = new T4TemplateInfo();
			EntityRepositoryInfo entityRepositoryInfo = dataModel.EntityRepositories.FirstOrDefault(er => er.EntitySet == entity);
			templateInfo.Properties["IUnitOfWorkFactoryType"] = dataModel.UnitOfWorkFactory.TypeInfo.Name;
			templateInfo.Properties["IUnitOfWorkType"] = dataModel.EntitiesUnitOfWork.TypeInfo.Name;
			templateInfo.Properties["IEntityRepositoryType"] = entityRepositoryInfo.TypeInfo.Name;
			templateInfo.Properties["CreateEntityRepositoryMethod"] = entityRepositoryInfo.Name;
			templateInfo.Properties["DataModel"] = dataModel;
			templateInfo.Properties["IEntitySetInfo"] = entity;
			EntitySetData entitySetData = EntitySetData.Create(entity, dataModel.Entities);
			templateInfo.Properties[TemplatesConstants.STR_EntitySetData] = entitySetData;
			if(isCommon) {
				templateInfo.UsingList = Merge(new string[] {
					dataModel.ExpressionHelper.With(t => t.NamespaceName),
					dataModel.EntitiesUnitOfWork.BaseType.NamespaceName
				},
					null
				);
			} else {
				var m2ms = new List<string>();
				if (context.MetadataWorkspace != null) {
					var entityType = FindEntityType(context.MetadataWorkspace, ((EntitySetInfoBase)entity).EntityType.Name);
					foreach(var navigation in entityType.NavigationProperties) {
						var association = FindManyToManyAssociation(context.MetadataWorkspace, entityType.Name, navigation.ToEndMember.GetEntityType().Name);
						if(association != null) {
							var lookupTable = context.SelectedTables.FirstOrDefault(t => t.ElementType.Type.Name == association.OtherEntity.Name);
							if (lookupTable != null) {
								m2ms.Add(lookupTable.ElementType.Type.Namespace);
							}
						}
					}
				}
				templateInfo.UsingList = Merge(
					new string[] {
						dataModel.ExpressionHelper.With(t => t.NamespaceName),
						dataModel.UnitOfWorkSource.NamespaceName,
						dataModel.EntitiesUnitOfWork.BaseType.NamespaceName,
						entityRepositoryInfo.TypeInfo.NamespaceName,
						entity.ElementType.Type.Namespace,
						dataModel.DbContainer != null ? dataModel.DbContainer.NamespaceName : string.Empty
					},
					entitySetData.FKCollections.Select<EntitySetData, string>(x => x.TypeNamespace).Concat(m2ms),
					null
				);
			}
			return templateInfo;
		}
		public IViewModelInfo GenerateViewModel(TemplateGenerationContext context, ViewModelType viewModelType, string viewModelName, string folderName, IDataModel dataModel, IEntitySetInfo entity, bool async) {
			try {
				if(!IsParamsValid(viewModelName, dataModel, entity))
					return null;
				AddAssemblyReferences(async, false, viewModelReferences);
				AddAssemblyReferences(async, true, GetEntityFrameworkReference(dataModel));
				var commonUsingList = GetTemplateInfoForViewModel(context, dataModel, entity, true).UsingList;
				var commonNamespacePrefix = Regex.Match(commonUsingList.First(), @".*?Common\.").ToString();
				T4TemplateInfo templateInfo = GetTemplateInfoForViewModel(context, dataModel, entity, false);
				templateInfo.UsingList = Merge(templateInfo.UsingList, new[] {
					commonNamespacePrefix + "ViewModel"
				});
				if(viewModelType == ViewModelType.EntityRepository)
					templateInfo.Properties[STR_CollectionViewModelHooks] = GetCollectionViewModelHooks();
				else
					templateInfo.Properties[STR_EntityViewModelHooks] = GetEntityViewModelHooks();
				GenerateViewModel(context, async, viewModelName, folderName, TemplatesResources.GetViewModelResourceType(viewModelType), templateInfo);
				return GetGeneratedViewModelTypeInfo(templateInfo, viewModelType, entity, dataModel.Entities);
			} catch(Exception ex) {
				Log.SendException(ex);
				return null;
			}
		}
		void AddAssemblyReferences(bool async, bool copyLocal, params string[] referenceNames) {
			if(referenceNames == null || referenceNames.Length == 0)
				return;
			if(!async) {
				foreach(string name in referenceNames)
					TemplatesPlatform.AddAssemblyReference(name, copyLocal);
				return;
			}
			CompoundMultiTask compoundMultiTask = GetCompoundMultiTask();
			foreach(string reference in referenceNames) {
				if(string.IsNullOrEmpty(reference))
					continue;
				ITask result = compoundMultiTask.Tasks.FirstOrDefault(task => {
					AddAssemblyReferenceTask arTask = task as AddAssemblyReferenceTask;
					return arTask != null && arTask.ReferenceName == reference;
				});
				if(result != null)
					continue;
				AddAssemblyReferenceTask referenceTask = new AddAssemblyReferenceTask(this.TemplatesPlatform, reference, copyLocal);
				compoundMultiTask.Add(referenceTask, STR_AddingReferences);
				UndoManager.Add(new SimpleActionUndoUnit(() => {
					compoundMultiTask.Remove(referenceTask);
				}));
			}
		}
		T4TemplateInfo GenerateDocumentManagerViewModel(TemplateGenerationContext context, IDataModel dataModel, DocumentManagerViewModelInfo viewModelInfo, string dbContainerName, UIType uiType, bool async, bool skipGeneration) {
			T4TemplateInfo templateInfo = new T4TemplateInfo();
			templateInfo.Properties[STR_DocumentManagerViewModelHooks] = GetDocumentManagerViewModelHooks();
			templateInfo.Properties["DbContainerName"] = dbContainerName;
			templateInfo.Properties["IViewModelInfo"] = viewModelInfo;
			templateInfo.Properties["UIType"] = uiType;
			templateInfo.UsingList = Merge(
				new string[] {
					TemplatesPaths.GetCommonDataModelNamespace(TemplatesPlatform),
					TemplatesPaths.GetCommonViewModelNamespace(TemplatesPlatform),
				},
				new string[] {
					dataModel.UnitOfWorkSource.NamespaceName,
					dataModel.EntitiesUnitOfWork.BaseType.NamespaceName,
					dataModel.DbContainer != null ? dataModel.DbContainer.NamespaceName : string.Empty,
				}
			);
			foreach(var table in viewModelInfo.Tables.Concat(viewModelInfo.Views)) {
				templateInfo.UsingList = Merge(templateInfo.UsingList, new string[] {
					table.RepositoryInfo.TypeInfo.NamespaceName,
					table.EntityInfo.ElementType.Type.Namespace,
				});
			}
			try {
				if(string.IsNullOrEmpty(viewModelInfo.Name)) {
					Log.SendWarning("View Model Name is not defined.");
					return templateInfo;
				}
				AddAssemblyReferences(async, false, STR_DevExpressXpfMvvm);
				GenerateViewModel(context, async, viewModelInfo.Name, string.Empty, typeof(Resources.ViewModels.DocumentManagerViewModelTemplate), templateInfo, skipGeneration);
			} catch(Exception ex) {
				Log.SendException(ex);
			}
			return templateInfo;
		}
		CompoundMultiTask GetCompoundMultiTask() {
			CompoundMultiTask compoundMultiTask = TaskManager.Tasks.FirstOrDefault(tk => tk is CompoundMultiTask) as CompoundMultiTask;
			if(compoundMultiTask == null) {
				compoundMultiTask = new CompoundMultiTask(serviceContainer);
				TaskManager.Add(compoundMultiTask);
			}
			return compoundMultiTask;
		}
		void DoAsyncIfNeeded(bool async, string description, Action action) {
			if(action == null)
				return;
			if(!async) {
				action.Invoke();
				return;
			}
			CompoundMultiTask compoundMultiTask = GetCompoundMultiTask();
			SimpleWizardTask task = new SimpleWizardTask(action);
			compoundMultiTask.Add(task, description);
			UndoManager.Add(new SimpleActionUndoUnit(() => compoundMultiTask.Remove(task)));
		}
		public void GenerateDocumentManagerView(TemplateGenerationContext context, DocumentManagerViewModelInfo viewModel, string viewName, string peekCollectionViewName, bool async) {
			if(context.PlatformType == PlatformType.WinForms && context.TaskType != TaskType.TabbedMDI) return;
			GenerateDocumentManagerViewCore(context, viewModel, viewName, peekCollectionViewName, TemplatesResources.GetDocumentManagerViewResourceType(context.PlatformType, context.UiType), async);
			if(context.UiType == UIType.OutlookInspired && context.PlatformType== PlatformType.WPF)
				GenerateDocumentManagerViewCore(context, viewModel, peekCollectionViewName, viewName, typeof(PeekCollectionViewTemplate), async);
		}
		void GenerateDocumentManagerViewCore(TemplateGenerationContext context, DocumentManagerViewModelInfo viewModel, string viewName, string relatedViewName, Type templateType, bool async) {
			GenerateViewCore(context, viewModel, viewName, relatedViewName, string.Empty, templateType, async, ti => {
				PrepareXamlNamespaces(null, viewModel, ti);
				ti.Properties[STR_DocumentManagerViewHooks] = GetDocumentManagerViewHooks();
			}, ViewType.None);
		}
		static void CreateCommandsForLookUpTables(UIType uiType, ViewModelDataBase viewModelData, T4TemplateInfo ti) {
			IEnumerable<IEdmPropertyInfo> scaffoldingProperties = viewModelData.GetScaffoldingProperties();
			Func<IEdmPropertyInfo, ForeignKeyInfo> getForeignKeyProperty = GetForeignKeyProperty(viewModelData);
			var pairs = viewModelData.GetCollectionProperties().With(x => x.ToArray());
			var editors = FluffyEditorsSource.GenerateEditors(scaffoldingProperties, GenerateEditorOptions.ForLayoutScaffolding(pairs), getForeignKeyProperty);
			var lookups = editors.Groups.Flatten(g => g.Groups)
				.Concat(new[] { editors })
				.SelectMany(g => g.Items)
				.Where(e => e.IsLookup && e.Lookup.ForeignKeyInfo != null)
				.ToList();
			ti.Properties["GeneratedLookups"] = lookups;
			if(viewModelData is EntityViewModelData) {
				foreach(CollectionViewModelData item in (viewModelData as EntityViewModelData).LookUpTables) {
					item.CreateCommands(GetImageType(uiType));
				}
			}
		}
		private static Func<IEdmPropertyInfo, ForeignKeyInfo> GetForeignKeyProperty(ViewModelDataBase viewModelData) {
			EntityViewModelData entityViewModelData = viewModelData as EntityViewModelData;
			CollectionViewModelData collectionViewModelData = viewModelData as CollectionViewModelData;
			if(entityViewModelData != null) return entityViewModelData.With(x => x.GetForeignKeyProperty);
			if(collectionViewModelData != null) return collectionViewModelData.With(x => x.GetForeignKeyProperty);
			return null;
		}
		void SetXamlInfoToT4Template(ViewType viewType, TemplateGenerationContext context, T4TemplateInfo ti, ViewModelDataBase viewModelData) {
			XamlEditingContext xamlContext = new XamlEditingContext();
			PrepareXamlNamespaces(xamlContext.Namespaces, viewModelData, ti);
			if(viewType == ViewType.Entity) {
				SetXamlInfoForEntityView(context, ti, (EntityViewModelData)viewModelData);
			} else {
				SetXamlInfoForCollectionView(ti, (CollectionViewModelData)viewModelData, xamlContext);
			}
		}
		void SetXamlInfoForCollectionView(T4TemplateInfo ti, CollectionViewModelData viewModelData, XamlEditingContext context) {
			IModelItem gridControl = context.CreateItem(XamlTypes.GridControlType.Value);
			ModelGridColumnGenerator.GenerateColumns(viewModelData.GetScaffoldingProperties(), gridControl, viewModelData.GetCollectionProperties().With(x => x.ToArray()));
			XamlInfo xamlInfo = context.GetXamlInfo(gridControl.Properties["Columns"].Collection);
			ti.Properties["GeneratedFieldsBase64"] = XamlToString(xamlInfo);
			xamlInfo = context.GetXamlInfo(gridControl.Properties["Bands"].Collection);
			ti.Properties["GeneratedBandsBase64"] = XamlToString(xamlInfo);
			ti.Properties[STR_CollectionViewHooks] = GetCollectionViewHooks();
		}
		void SetXamlInfoForEntityView(TemplateGenerationContext context, T4TemplateInfo ti, EntityViewModelData viewModelData) {
			var getForeignKeyProperty = viewModelData.GetForeignKeyProperty;
			var pairs = viewModelData.GetCollectionProperties().With(x => x.ToArray());
			var editorInfos = FluffyEditorsSource.GenerateEditors(viewModelData.GetScaffoldingProperties(), GenerateEditorOptions.ForLayoutScaffolding(pairs), getForeignKeyProperty);
			ti.Properties["EditorInfos"] = editorInfos;
			AddLookUpGeneratedData(context, viewModelData as EntityViewModelData, ti);
			ti.Properties[STR_EntityViewHooks] = GetEntityViewHooks();
		}
		static string XamlToString(XamlInfo info) {
			return info.Xaml ?? string.Empty;
		}
		internal static string FindDbSetName(MetadataWorkspaceRuntimeWrapper metadataWorkspace, string entityName) {
			var sets = metadataWorkspace.GetItems(DataSpaceRuntimeWrapper.CSpace)
				.Where(x => EntityContainerRuntimeWrapper.IsCompatible(x.Object.GetType()))
				.Select(EntityContainerRuntimeWrapper.Wrap)
				.SelectMany(x => x.BaseEntitySets);
			return sets.First(x => x.ElementType.Name == entityName).Name;
		}
		internal static EntityTypeRuntimeWrapper FindEntityType(MetadataWorkspaceRuntimeWrapper metadataWorkspace, string entityName) {
			return metadataWorkspace.GetItems(DataSpaceRuntimeWrapper.CSpace)
				.Where(x => x.BuiltInTypeKind == BuiltInTypeKindRuntimeWrapper.EntityType)
				.Select(EntityTypeRuntimeWrapper.Wrap)
				.Where(x => x.Name == entityName)
				.Single();
		}
		static bool IsJunctionTable(EntityTypeRuntimeWrapper entity) {
			return entity.KeyMembers.Count() == 2 && entity.Properties.Count() == 2;
		}
		internal static string PrintPrimaryKeyType(EntityTypeRuntimeWrapper entity) {
			var keys = entity.KeyMembers.Select(k => k.TypeUsage.EdmType.Name);
			if(keys.Count() == 1)
				return keys.Single();
			return string.Format("Tuple<{0}>", keys.Aggregate((l, r) => l + ", " + r));
		}
		internal static string PrintTrivialPrimaryKey(EntityTypeRuntimeWrapper entity) {
			if(entity.KeyMembers.Count() != 1)
				throw new InvalidOperationException();
			return string.Format("x => x.{0}", entity.KeyMembers.Single().Name);
		}
		class GenericEqualityComparer<T> : IEqualityComparer<T> {
			Func<T, T, bool> equals;
			Func<T, int> hash;
			public GenericEqualityComparer(Func<T, T, bool> equals, Func<T, int> hash = null) {
				if(equals == null)
					throw new ArgumentNullException();
				this.equals = equals;
				this.hash = hash;
			}
			public bool Equals(T x, T y) {
				return equals(x, y);
			}
			public int GetHashCode(T obj) {
				if(hash == null)
					throw new InvalidOperationException();
				return hash(obj);
			}
		}
		internal static ManyToManyInfo FindManyToManyAssociation(MetadataWorkspaceRuntimeWrapper metadataWorkspace, string ownerEntityName, string lookupTypeName) {
			var lookupEntityType = FindEntityType(metadataWorkspace, lookupTypeName);
			var thisEntity = FindEntityType(metadataWorkspace, ownerEntityName);
			var lookupNavigationProperty = lookupEntityType.NavigationProperties.FirstOrDefault(p => {
				return p.FromEndMember.GetEntityType().Name == ownerEntityName
					|| p.ToEndMember.GetEntityType().Name == ownerEntityName;
			});
			if(lookupNavigationProperty == null)
				return null;
			var associations = FindAssociations(metadataWorkspace, lookupNavigationProperty);
			if(associations.Count() == 1) {
				var association = associations.Single();
				if(association.AssociationEndMembers.All(m => m.RelationshipMultiplicity == RelationshipMultiplicityRuntimeWrapper.Many)) {
					return new ManyToManyInfo { ThisEntity = thisEntity, OtherEntity = lookupEntityType };
				}
				if (association.AssociationEndMembers.Count(m => m.RelationshipMultiplicity == RelationshipMultiplicityRuntimeWrapper.Many) == 1 &&
					association.AssociationEndMembers.Count(m => m.RelationshipMultiplicity == RelationshipMultiplicityRuntimeWrapper.One) == 1)
				{
					var leftEntity = association.AssociationEndMembers.First().GetEntityType();
					var rightEntity = association.AssociationEndMembers.Last().GetEntityType();
					var commonEntity = leftEntity.Name == ownerEntityName ? rightEntity : leftEntity;
					if(IsJunctionTable(commonEntity)) {
						var typeComparer = new GenericEqualityComparer<EntityTypeRuntimeWrapper>(
							(t1, t2) => t1.FullName == t2.FullName,
							t => t.FullName.GetHashCode()
						);
						var junctionAssociations = FindAssociations(metadataWorkspace, commonEntity);
						var allEntities = junctionAssociations.SelectMany(a => a.AssociationEndMembers).Select(m => m.GetEntityType());
						var otherEntity = allEntities.Distinct(typeComparer).Except(new[] { thisEntity, commonEntity }, typeComparer).FirstOrDefault();
						if(otherEntity != null) {
							var thisToJunctionAssociation = association;
							var otherToJunctionAssociation = junctionAssociations
								.First(a => a.AssociationEndMembers.Any(m => m.GetEntityType().FullName == otherEntity.FullName));
							var thisForeignKey = GetForeignProperties(thisEntity, thisToJunctionAssociation);
							var otherForeignKey = GetForeignProperties(otherEntity, otherToJunctionAssociation);
							return new ManyToManyInfo { 
								ThisEntity = thisEntity, 
								OtherEntity = otherEntity, 
								ExplicitJunctionEntity = commonEntity,
								ThisEntityJunctionForeignKey = thisForeignKey,
								OtherEntityJunctionForeignKey = otherForeignKey
							};
						}
					}
				}
			}
			return null;
		}
		static IEnumerable<EdmPropertyRuntimeWrapper> GetForeignProperties(EntityTypeRuntimeWrapper entity, AssociationTypeRuntimeWrapper association) {
			var thisToJunctionConstraint = association.ReferentialConstraints.Single();
			bool isEntityFrom = thisToJunctionConstraint.FromRole.GetEntityType().FullName == entity.FullName;
			return isEntityFrom ? thisToJunctionConstraint.ToProperties : thisToJunctionConstraint.FromProperties;
		}
		static internal bool IsAlreadyHandled(LookUpCollectionViewModelData lookupData, IEnumerable<NavigationPropertyRuntimeWrapper> navigations) {
			return navigations.Any(n => n.Name == lookupData.LookUpCollectionPropertyAssociationName);
		}
		static IEnumerable<EdmPropertyRuntimeWrapper> CollectEndFromProperties(ReferentialConstraintRuntimeWrapper constraint) {
			return constraint.FromProperties.Concat(constraint.ToProperties);
		}
		static AssociationTypeRuntimeWrapper[] FindAssociations(MetadataWorkspaceRuntimeWrapper metadataWorkspace, EntityTypeRuntimeWrapper entity) {
			var associations = metadataWorkspace.GetItems(DataSpaceRuntimeWrapper.CSpace)
				.Where(x => x.BuiltInTypeKind == BuiltInTypeKindRuntimeWrapper.AssociationType)
				.Select(AssociationTypeRuntimeWrapper.Wrap);
			return associations.Where(a => {
				var first = a.AssociationEndMembers.First().GetEntityType().FullName;
				var second = a.AssociationEndMembers.Last().GetEntityType().FullName;
				return first == entity.FullName || second == entity.FullName;
			}).ToArray();
		}
		static AssociationTypeRuntimeWrapper[] FindAssociations(
			MetadataWorkspaceRuntimeWrapper metadataWorkspace, 
			EntityTypeRuntimeWrapper toEntity, 
			EntityTypeRuntimeWrapper fromEntity)
		{
			var associations = metadataWorkspace.GetItems(DataSpaceRuntimeWrapper.CSpace)
				.Where(x => x.BuiltInTypeKind == BuiltInTypeKindRuntimeWrapper.AssociationType)
				.Select(AssociationTypeRuntimeWrapper.Wrap);
			return associations.Where(a => {
				var first = a.AssociationEndMembers.First().GetEntityType().FullName;
				var second = a.AssociationEndMembers.Last().GetEntityType().FullName;
				return (first == toEntity.FullName && second == fromEntity.FullName) ||
					   (first == fromEntity.FullName && second == toEntity.FullName);
			}).ToArray();
		}
		static AssociationTypeRuntimeWrapper[] FindAssociations(MetadataWorkspaceRuntimeWrapper metadataWorkspace, NavigationPropertyRuntimeWrapper navigationProperty) {
			var toEntity = AssociationEndMemberRuntimeWrapper.Wrap(navigationProperty.ToEndMember).GetEntityType();
			var fromEntity = AssociationEndMemberRuntimeWrapper.Wrap(navigationProperty.FromEndMember).GetEntityType();
			return FindAssociations(metadataWorkspace, toEntity, fromEntity);
		}
		static AssociationTypeRuntimeWrapper FindAssociation(MetadataWorkspaceRuntimeWrapper metadataWorkspace, NavigationPropertyRuntimeWrapper navigationProperty) {
			return FindAssociations(metadataWorkspace, navigationProperty).First();
		}
		void AddLookUpGeneratedData(TemplateGenerationContext context, EntityViewModelData viewModelData, T4TemplateInfo templateInfo) {
			XamlEditingContext xamlContext = new XamlEditingContext();
			foreach(var lookupTable in viewModelData.LookUpTables) {
				var properties = lookupTable.GetScaffoldingProperties();
				if(context.MetadataWorkspace != null) { 
					var entityTypeWrapper = FindEntityType(context.MetadataWorkspace, viewModelData.EntityTypeName);
					var parentNavigationProperty = entityTypeWrapper.NavigationProperties.First(p => p.Name == lookupTable.LookUpCollectionPropertyAssociationName);
					var parentConstraint = FindAssociation(context.MetadataWorkspace, parentNavigationProperty).ReferentialConstraints.FirstOrDefault();
					var foreignKeyProperties = parentConstraint == null ? new EdmPropertyRuntimeWrapper[0] : CollectEndFromProperties(parentConstraint)
						.Where(p => p.DeclaringType.Name != entityTypeWrapper.Name);
					var lookupEntityType = FindEntityType(context.MetadataWorkspace, lookupTable.EntityTypeName);
					var lookupNavigationProperty = lookupEntityType.NavigationProperties.First(
						p => p.FromEndMember.GetEntityType().Name == entityTypeWrapper.Name
						  || p.ToEndMember.GetEntityType().Name == entityTypeWrapper.Name);
					properties = properties.Except(properties.Where(p => p.Name == lookupNavigationProperty.Name || foreignKeyProperties.Any(fkp => fkp.Name == p.Name)));
				}
				lookupTable.CreateCommands(GetImageType(context.UiType));
				IModelItem gridControl = xamlContext.CreateItem(XamlTypes.GridControlType.Value);
				ModelGridColumnGenerator.GenerateColumns(properties, gridControl, lookupTable.GetCollectionProperties().With(x => x.ToArray()));
				XamlInfo xamlInfo = xamlContext.GetXamlInfo(gridControl.Properties["Columns"].Collection);
				templateInfo.Properties["GeneratedFieldsBase64." + lookupTable.Name] = XamlToString(xamlInfo);
				xamlInfo = xamlContext.GetXamlInfo(gridControl.Properties["Bands"].Collection);
				templateInfo.Properties["GeneratedBandsBase64." + lookupTable.Name] = XamlToString(xamlInfo);
			}
		}
		static ImageType GetImageType(UIType uiType) {
			switch(uiType) {
				case UIType.WindowsUI: return ImageType.GrayScaled;
				case UIType.OutlookInspired: return ImageType.Office2013;
				default: return ImageType.Colored;
			}
		}
		void PrepareXamlNamespaces(XamlNamespaces namespaces, IViewModelInfoBase viewModel, T4TemplateInfo ti) {
			if(namespaces == null)
				namespaces = new XamlNamespaces();
			ViewModelDataBase viewModelData = viewModel as ViewModelDataBase;
			ti.Properties[TemplatesConstants.STR_XamlNamespaces] = namespaces;
			if(viewModelData != null && viewModelData.XamlNamespace != null) {
				namespaces.Register(viewModelData.XamlNamespace);
				ti.Properties["viewModelPrefix"] = viewModelData.XamlNamespace.Prefix;
			} else {
				XamlClrNamespaceDeclaration declaration = new XamlClrNamespaceDeclaration(viewModel.Namespace, viewModel.AssemblyName);
				declaration.IsLocal = viewModel.IsLocalType;
				declaration = namespaces.Register(declaration) as XamlClrNamespaceDeclaration;
				ti.Properties["viewModelPrefix"] = namespaces.SetUniquePrefix(declaration, "viewmodel");
			}
		}
		void GenerateViewCore(TemplateGenerationContext context, IViewModelInfoBase viewModel, string viewName, string relatedViewName, string folderName, Type templateType, bool async, Action<T4TemplateInfo> prepareSpecificInfo, ViewType viewType) {
			try {
				if(viewModel == null)
					return;
				if(string.IsNullOrEmpty(viewName))
					viewName = ViewNameHelper.GetViewName(null, viewModel.Name, viewType);
				T4TemplateInfo templateInfo = new T4TemplateInfo();
				templateInfo.Properties["IViewModelInfo"] = viewModel;
				templateInfo.Properties["UIType"] = context.UiType;
				templateInfo.Properties["TemplateGenerationContext"] = context;
				var defaultNamespace = TemplatesPlatform.GetProjectNamespace();
				templateInfo.Properties["DefaultNamespacePrefix"] = TemplatesPlatform.IsVisualBasic && !string.IsNullOrEmpty(defaultNamespace) ? defaultNamespace + "." : "";
				prepareSpecificInfo(templateInfo);
				if(context.PlatformType == PlatformType.WinForms)
					GenerateViewByPlatformType(context, viewName, relatedViewName, folderName, templateType, async, viewType, templateInfo, viewWinReferences, TemplatesPlatform.GetFileExtensionForActiveProject(), STR_DesignerFileExt);
				else
					GenerateViewByPlatformType(context, viewName, relatedViewName, folderName, templateType, async, viewType, templateInfo, viewReferences, STR_XamlFileExt, STR_XamlFileExt);
#if !DEBUGTEST
							AddLicXFile(async);
#endif
			} catch (Exception ex) {
				Log.SendException(ex);
			}
		}
		private void GenerateViewByPlatformType(TemplateGenerationContext context, string viewName, string relatedViewName, string folderName, Type templateType, bool async, ViewType viewType, T4TemplateInfo templateInfo, string[] viewReferences, string fileExt, string designerFileExt) {
			AddAssemblyReferences(async, false, viewReferences);
			AddAssemblyReferences(async, true, AssemblyInfo.SRAssemblyImages);
			string targetDirectory = TemplatesPaths.GetTemplateTargetDirectory(TemplatesPlatform, templateType, templateInfo, folderName, context.UiType);
			string fileName;
			templateInfo.Properties["ViewName"] = GetUniqueItemName(targetDirectory, viewName, fileExt, out fileName);
			if(relatedViewName != null) {
				string relatedFileName;
				templateInfo.Properties["RelatedViewName"] = GetUniqueItemName(targetDirectory, relatedViewName, designerFileExt, out relatedFileName);
			}
			templateInfo.Properties["IsVisualBasic"] = TemplatesPlatform.IsVisualBasic;
			GenerateDependentFiles(fileName,
				targetDirectory,
				true,
				templateType,
				GetTypeFormUIAndViewType(context.PlatformType, context.UiType, viewType),
				async,
				templateInfo,
				designerFileExt,
				null,
				context.PlatformType);
		}
		private static Type GetTypeFormUIAndViewType(PlatformType platformType, UIType uiType, ViewType viewType) {
			if(platformType == PlatformType.WinForms) {
				switch(uiType) {	 
					case UIType.WindowsUI:
						switch(viewType) {
							case ViewType.None:
								return typeof(Resources.Views.WinForms.WinUI.DocumentManagerView_WinUIDesigner);
							case ViewType.Entity:
								return typeof(Resources.Views.WinForms.WinUI.ElementView_WinUIDesigner);
							case ViewType.Repository:
								return typeof(Resources.Views.WinForms.WinUI.CollectionView_WinUIDesigner);
						}
						break;
					case UIType.OutlookInspired:
						switch(viewType) {
							case ViewType.None:
								return typeof(Resources.Views.WinForms.Outlook.DocumentManagerView_OutlookDesigner);
							case ViewType.Entity:
								return typeof(Resources.Views.WinForms.Outlook.ElementView_OutlookDesigner);
							case ViewType.Repository:
								return typeof(Resources.Views.WinForms.Outlook.CollectionView_OutlookDesigner);
						}
						break;
					default:
						switch(viewType) {
							case ViewType.None:
								return typeof(Resources.Views.WinForms.Standart.DocumentManagerView_TabbedMDIDesigner);
							case ViewType.Entity:
								return typeof(Resources.Views.WinForms.Standart.ElementView_TabbedMDIDesigner);
							case ViewType.Repository:
								return typeof(Resources.Views.WinForms.Standart.CollectionView_TabbedMDIDesigner);
						}
						break;
				}
			}
			return typeof(ViewCodeBehindTemplate);
		}
		void GenerateDependentFiles(string fileName, string targetDirectory, bool rebuildNeeded, Type templateType, Type codeBehindTemplateType, bool async, T4TemplateInfo templateInfo, string fileExtension, IDictionary<string, string> properties = null, PlatformType platformType = PlatformType.WPF) {
			try {
				fileName = Path.Combine(targetDirectory, fileName);
				string codeBehind = Path.ChangeExtension(fileName, fileExtension + "." + TemplatesPlatform.GetFileExtensionForActiveProject());
				DoAsyncIfNeeded(async, STR_GeneratingFiles, () => {
					string result = TextTemplatingHelper.ExpandTemplate(templateType, templateInfo);
					if (!fileName.EndsWith("settings") && platformType != PlatformType.WinForms)
						result = TemplatesPlatform.FormatXaml(result);
					string codeBehindResult = TextTemplatingHelper.ExpandTemplate(codeBehindTemplateType, templateInfo);
					if(platformType == PlatformType.WPF) codeBehindResult = ConvertToVBIfNeeded(codeBehindResult, false, fileName);
					CreateDirectoryIfNeeded(targetDirectory);
					WriteToFile(fileName, result);
					WriteToFile(codeBehind, codeBehindResult);
					TemplatesPlatform.AddFileAndDependencies(fileName, rebuildNeeded, properties, codeBehind);
				});
			} catch (Exception ex) {
				Log.SendException(ex);
			}
		}
		void AddLicXFile(bool async) {
			if(!async) {
				TemplatesPlatform.AddLicXFile();
				return;
			}
			CompoundMultiTask compoundMultiTask = GetCompoundMultiTask();
			AddLicXFileTask task = compoundMultiTask.Tasks.FirstOrDefault(x => x is AddLicXFileTask) as AddLicXFileTask;
			if(task != null)
				return;
			task = new AddLicXFileTask(this.TemplatesPlatform);
			compoundMultiTask.Add(task, STR_GeneratingFiles);
			UndoManager.Add(new SimpleActionUndoUnit(() => {
				compoundMultiTask.Remove(task);
			}));
		}
		DocumentsGenerationOptions GetDocumentsGenerationOptions(TaskType taskType, ViewModelType viewModelType) {
			DocumentsGenerationOptions generationOptions = DocumentsGenerationOptions.All;
			switch(taskType) {
				case TaskType.ViewModelLayer:
				case TaskType.DataLayer:
					switch(viewModelType) {
						case ViewModelType.Entity:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.None, DocumentGenerationOption.ViewModel, DocumentGenerationOption.None);
							break;
						case ViewModelType.EntityRepository:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.ViewModel, DocumentGenerationOption.None, DocumentGenerationOption.None);
							break;
						default:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.ViewModel, DocumentGenerationOption.ViewModel, DocumentGenerationOption.ViewModel);
							break;
					}
					break;
				case TaskType.ViewLayer:
					switch(viewModelType) {
						case ViewModelType.Entity:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.None, DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.None);
							break;
						case ViewModelType.EntityRepository:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.None, DocumentGenerationOption.None);
							break;
						default:
							generationOptions = new DocumentsGenerationOptions(DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.View);
							break;
					}
					break;
				case TaskType.TabbedMDI:
					generationOptions = DocumentsGenerationOptions.All;
					break;
			}
			return generationOptions;
		}
		public void GenerateDocumentsViewCore(TemplateGenerationContext context, UIType[] uiTypes, IDataModel dataModel, bool async, DocumentsGenerationOptions options) {
			if(context.SelectedTables.Any()) {
				var entity = context.SelectedTables.First();
				var folderName = entity.ElementType.Type.Name;
				if (context.PlatformType == PlatformType.WPF) {
					AddAssemblyReferences(async, false, AssemblyInfo.SRAssemblyXpfCore);
				}
				foreach(var templateType in TemplatesResources.GetViewModelCommonResourceNames(context)) {
					var info = GetTemplateInfoForViewModel(context, dataModel, entity, true);
					GenerateViewModel(context, async, null, folderName, templateType, info);
				}
			}
			foreach(var entity in context.SelectedTables) {
				bool readOnly = entity.ShouldGenerateReadOnlyView();
				IViewModelInfo repositoryViewModel = null;
				if(options.GenerateCollectionViewModels)
					repositoryViewModel = GenerateViewModel(
						context,
						ViewModelType.EntityRepository,
						ViewModelLayerService.GetViewModelName(entity.ElementType.Type.Name, ViewModelType.EntityRepository),
						entity.ElementType.Type.Name,
						dataModel,
						entity,
						async);
				IViewModelInfo entityViewModel = null;
				if(!readOnly && options.GenerateEntityViewModels)
					entityViewModel = GenerateViewModel(
						context,
						ViewModelType.Entity,
						ViewModelLayerService.GetViewModelName(entity.ElementType.Type.Name, ViewModelType.Entity),
						entity.ElementType.Type.Name,
						dataModel,
						entity,
						async);
				foreach(var uiType in uiTypes) {
					context = new TemplateGenerationContext(
						context.PlatformType,
						context.TaskType,
						uiType,
						context.SelectedTables,
						context.DbContainer,
						context.WithoutDesignTime
					);
					if(options.GenerateCollectionViews)
						GenerateView(context, repositoryViewModel, ViewModelLayerService.GetViewName(repositoryViewModel.EntityTypeName, repositoryViewModel.Name, ViewType.Repository), repositoryViewModel.EntityTypeName, ViewType.Repository, async);
					if(!readOnly && options.GenerateEntityViews)
						GenerateView(context, entityViewModel, ViewModelLayerService.GetViewName(entityViewModel.EntityTypeName, entityViewModel.Name, ViewType.Entity), entityViewModel.EntityTypeName, ViewType.Entity, async);
				}
			}
			IDbContainerInfo dbContainer = dataModel.DbContainer;
			foreach(UIType uiType in uiTypes) {
				DocumentManagerViewModelInfo documentManagerViewModelInfo =
					new DocumentManagerViewModelInfo(null, true,
					dbContainer.Name + "ViewModel", TemplatesPlatform.GetDefaultNamespace(), true,
					dbContainer.Name + "ModuleDescription",
					GetTables(dataModel, dataModel.Entities, false), GetTables(dataModel, dataModel.Entities, true));
				T4TemplateInfo viewModelTemplateInfo = GenerateDocumentManagerViewModel(context, dataModel, documentManagerViewModelInfo, dbContainer.Name, uiType, async, !options.GenerateDocumentManagerViewModel);
				string uniqueViewModelName = viewModelTemplateInfo != null ? viewModelTemplateInfo.Properties[TemplatesConstants.STR_ViewModelName] as string : null;
				if(!string.IsNullOrEmpty(uniqueViewModelName) && String.Compare(documentManagerViewModelInfo.Name, uniqueViewModelName, false) != 0)
					documentManagerViewModelInfo.Name = uniqueViewModelName;
				string nameSpaceName = viewModelTemplateInfo.GetProperty(TemplatesConstants.STR_Namespace) as string;
				if(!string.IsNullOrEmpty(nameSpaceName))
					documentManagerViewModelInfo.Namespace = nameSpaceName;
				if(options.GenerateDocumentManagerView) {
					context = new TemplateGenerationContext(
						context.PlatformType,
						context.TaskType,
						uiType,
						context.SelectedTables,
						context.DbContainer,
						context.WithoutDesignTime);
					GenerateDocumentManagerView(context, documentManagerViewModelInfo, dbContainer.Name + "View", "PeekCollectionView", async);
				}
			}
		}
		static DocumentInfo[] GetTables(IDataModel dataModel, IEnumerable<IEntitySetInfo> selectedTables, bool selectViews) {
			return selectedTables.Where(x => x.IsView == selectViews).Select(x => new DocumentInfo(x.ElementType.Type.Name + "CollectionView", GetCaption(x.Name), x, dataModel.EntityRepositories.FirstOrDefault(er => er.EntitySet == x))).ToArray();
		}
		public static string GetCaption(string entitySetName) {
			return SplitStringHelper.SplitPascalCaseString(entitySetName).Replace('_', ' ');
		}
		IViewModelInfo ActivateViewModelInfoIfNeeded(IViewModelInfo info) {
			if(info == null)
				return null;
			if(info.Activated)
				return info;
			ViewModelDataBase viewModel = info as ViewModelDataBase;
			IDXTypeInfo entityType = this.SolutionTypesProvider.FindType(viewModel.EntityTypeFullName);
			if(entityType == null)
				return info;
			IDataModel model;
			IEntitySetInfo entity = this.DataAccesLayerService.FindEntitySetInAvailableDataModels(entityType, out model);
			if(entity == null || model == null)
				return info;
			string nameSpace = info.Namespace;
			string viewModelName = info.Name;
			string assemblyName = info.AssemblyName;
			IEnumerable<IEntitySetInfo> selectedEntities = model.Entities;
			if(info.ViewModelType == ViewModelType.Entity) {
				EntityViewModelData viewModelData = info as EntityViewModelData;
				List<LookUpCollectionViewModelData> lookUpTables = new List<LookUpCollectionViewModelData>();
				EntitySetData entitySetData = EntitySetData.Create(entity, selectedEntities);
				foreach(EntitySetData lookUp in entitySetData.LookUpTables) {
					LookUpCollectionViewModelData lookUpViewModel = viewModelData.LookUpTables.FirstOrDefault(x => x.Name == lookUp.PropertyName);
					if(lookUpViewModel == null)
						continue;
					IEntitySetInfo lookUpInfo = selectedEntities.FirstOrDefault(x => x.ElementType.Type.FullName == lookUp.TypeFullName);
					if(lookUpInfo == null)
						continue;
					LookUpCollectionViewModelData item = ViewModelDataSource.GetGeneratedLookUpCollectionViewModelData(assemblyName,
						lookUpViewModel.Name, lookUpViewModel.Namespace, lookUp.ElementType,
						lookUpInfo.ShouldGenerateReadOnlyView(), lookUpInfo.ElementType.AllProperties, null, lookUp.PropertyName);
					if(item != null)
						lookUpTables.Add(item);
				}
				return ViewModelDataSource.GetGeneratedEntityViewModelData(assemblyName, viewModelName, nameSpace, entity.ElementType.Type, entity.ElementType, GetCollectionProperties(entity, selectedEntities), lookUpTables.ToArray(), GetForeignKeyInfoCallback(entity));
			}
			if(info.ViewModelType == ViewModelType.EntityRepository)
				return ViewModelDataSource.GetGeneratedCollectionViewModelData(assemblyName, viewModelName, nameSpace, entity.ElementType.Type, entity.ShouldGenerateReadOnlyView(), entity.ElementType.AllProperties,null, GetForeignKeyInfoCallback(entity), GetCollectionProperties(entity, selectedEntities));
			return info;
		}
		public const string STR_DebugLogHooks = "DebugLogHooks";
		public const string STR_DebugLogHook_PrivateToProtectedVirtual = "PrivateToProtectedVirtual";
		public const string STR_DebugLogHook_PrivateToProtected = "PrivateToProtected";
		public const string STR_EntityViewHooks = "EntityViewHooks";
		public const string STR_EntityViewHook_GenerateCustomLayoutItems = "GenerateCustomLayoutItems";
		public const string STR_EntityViewHook_GenerateAdditionalLayoutItems = "GenerateAdditionalLayoutItems";
		public const string STR_EntityViewHook_GenerateAdditionalXmlNamespaces = "GenerateAdditionalXmlNamespaces";
		public const string STR_EntityViewModelHooks = "EntityViewModelHooks";
		public const string STR_EntityViewModelHook_GenerateLookUpProjection = "GenerateLookUpProjection";
		public const string STR_CollectionViewHooks = "CollectionViewHooks";
		public const string STR_CollectionViewHook_GenerateAdditionalRibbonPageGroups = "GenerateAdditionalRibbonPageGroups";
		public const string STR_CollectionViewHook_GenerateAdditionalViewProperties = "GenerateAdditionalViewProperties";
		public const string STR_CollectionViewHook_GenerateAdditionalGridProperties = "GenerateAdditionalGridProperties";
		public const string STR_CollectionViewHook_GenerateCustomGridColumns = "GenerateCustomGridColumns";
		public const string STR_CollectionViewHook_GenerateCustomContent = "GenerateCustomContent";
		public const string STR_CollectionViewHook_GenerateCustomXmlNamespaces = "GenerateCustomXmlNamespaces";
		public const string STR_CollectionViewHook_GenerateEmptyString = "GenerateEmptyString";
		public const string STR_CollectionViewHook_WinUI_GenerateAdditionalAppBarItems = "WinUI_GenerateAdditionalAppBarItems";
		public const string STR_CollectionViewModelHooks = "CollectionViewModelHooks";
		public const string STR_CollectionViewModelHook_GenerateIncludes = "GenerateIncludes";
		public const string STR_CollectionViewModelHook_GeneratePropertiesUsing = "GeneratePropertiesUsing";
		public const string STR_CollectionViewModelHook_GenerateProjectionType = "GenerateProjectionType";
		public const string STR_CollectionViewModelHook_CollectionViewModelBaseClassPrefix = "CollectionViewModelBaseClassPrefix";
		public const string STR_DocumentManagerViewHooks = "DocumentManagerViewHooks";
		public const string STR_DocumentManagerViewHook_GenerateRibbonPageHeaderItems = "GenerateRibbonPageHeaderItems";
		public const string STR_DocumentManagerViewHook_GenerateAdditionalBehaviors = "GenerateAdditionalBehaviors";
		public const string STR_DocumentManagerViewHook_OutlookUI_GenerateCustomNavBarGroupContent = "OutlookUI_GenerateCustomNavBarGroupContent";
		public const string STR_DocumentManagerViewHook_WinUI_GenerateAdditionalTileBarItemSettings = "WinUI_GenerateCustomTileBarItemSettings";
		public const string STR_DocumentManagerViewModelHooks = "DocumentManagerViewModelHooks";
		public const string STR_DocumentManagerViewModelHooks_GenerateAdditionalCommands = "GenerateAdditionalCommands";
		public const string STR_DocumentManagerViewModelHooks_GenerateAdditionalServices = "GenerateAdditionalServices";
		public const string STR_DocumentManagerViewModelHook_GenerateAdditionalModules = "GenerateAdditionalModules";
		public const string STR_UnitOfWorkHooks = "UnitOfWorkHooks";
		public const string STR_UnitOfWorkHooks_GetRepositoryAdditionalArguments = "GetRepositoryAdditionalArguments";
		protected virtual IEnumerable<T4TemplateHook> GetDebugLogHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetEntityViewHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetEntityViewModelHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetCollectionViewHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetCollectionViewModelHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetDocumentManagerViewHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetDocumentManagerViewModelHooks() {
			return null;
		}
		protected virtual IEnumerable<T4TemplateHook> GetUnitOfWorkHooks() {
			return null;
		}
		public void GenerateView(TemplateGenerationContext context, IViewModelInfo viewModel, string viewName, string folderName, ViewType viewType, bool async) {
			if(context.PlatformType == PlatformType.WinForms && (context.TaskType == TaskType.DataLayer || context.TaskType == TaskType.ViewModelLayer)) return;
			viewModel = ActivateViewModelInfoIfNeeded(viewModel);
			var viewResourceType = TemplatesResources.GetViewResourceType(context.PlatformType, viewType, context.UiType);
			GenerateViewCore(context, viewModel, viewName, null, folderName, viewResourceType, async, ti => {
				ViewModelDataBase viewModelData = (ViewModelDataBase)viewModel;
				viewModelData.CreateCommands(GetImageType(context.UiType));
				if(context.PlatformType == PlatformType.WPF) {
					SetXamlInfoToT4Template(viewType, context, ti, viewModelData);
				} else {
					CreateCommandsForLookUpTables(context.UiType, viewModelData, ti);
				}
			}, viewType);
		}
		public void GenerateDocumentsView(TemplateGenerationContext context, IDbContainerInfo dbContainerInfo, ViewModelType viewModelType, bool async) {
			IDataModel dataModel = GenerateEntityDataModel(context, dbContainerInfo, async);
			DocumentsGenerationOptions generationOptions = DocumentsGenerationOptions.All;
			if(context.PlatformType == PlatformType.WinForms) generationOptions = GetDocumentsGenerationOptions(context.TaskType, viewModelType);
			GenerateDocumentsViewCore(context, new[] { context.UiType }, dataModel, async, generationOptions);
		}
		public void GenerateDocumentsView(TemplateGenerationContext context, IDataModel dataModel, ViewModelType viewModelType, bool async) {
			DocumentsGenerationOptions generationOptions = DocumentsGenerationOptions.All;
			if(context.PlatformType == PlatformType.WinForms) generationOptions = GetDocumentsGenerationOptions(context.TaskType, viewModelType);
			GenerateDocumentsViewCore(context, new[] { context.UiType }, dataModel, async, generationOptions);
		}
	}
	public class T4TemplateHook {
		public T4TemplateHook(string text, string id, Func<T4TemplateInfo, bool> filter = null, bool addNewLine = true)
			: this(x => { if(addNewLine) x.WriteLine(text); else x.Write(text); }, id, filter) {
		}
		public T4TemplateHook(Action<IT4Template> action, string id, Func<T4TemplateInfo, bool> filter = null) {
			this.Action = action;
			this.Id = id;
			this.Filter = filter;
		}
		public Action<IT4Template> Action { get; private set; }
		public string Id { get; private set; }
		public Func<T4TemplateInfo, bool> Filter { get; private set; }
	}
	public class DocumentsGenerationOptions {
		public static DocumentsGenerationOptions None { get { return new DocumentsGenerationOptions(DocumentGenerationOption.None, DocumentGenerationOption.None, DocumentGenerationOption.None); } }
		public static DocumentsGenerationOptions All { get { return new DocumentsGenerationOptions(DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.ViewModelAndView, DocumentGenerationOption.ViewModelAndView); } }
		public static DocumentsGenerationOptions AllViewModels { get { return new DocumentsGenerationOptions(DocumentGenerationOption.ViewModel, DocumentGenerationOption.ViewModel, DocumentGenerationOption.None); } }
		public static DocumentsGenerationOptions CollectionViewModels { get { return new DocumentsGenerationOptions(DocumentGenerationOption.ViewModel, DocumentGenerationOption.None, DocumentGenerationOption.None); } }
		DocumentGenerationOption collectionGenerationOption;
		DocumentGenerationOption entityGenerationOption;
		DocumentGenerationOption documentManagerGenerationOption;
		public DocumentsGenerationOptions(DocumentGenerationOption collectionGenerationOption, DocumentGenerationOption entityGenerationOption, DocumentGenerationOption documentManagerGenerationOption) {
			this.collectionGenerationOption = collectionGenerationOption;
			this.entityGenerationOption = entityGenerationOption;
			this.documentManagerGenerationOption = documentManagerGenerationOption;
		}
		public bool GenerateCollectionViewModels { get { return collectionGenerationOption.HasFlag(DocumentGenerationOption.ViewModel); } }
		public bool GenerateEntityViewModels { get { return entityGenerationOption.HasFlag(DocumentGenerationOption.ViewModel); } }
		public bool GenerateCollectionViews { get { return collectionGenerationOption.HasFlag(DocumentGenerationOption.View); } }
		public bool GenerateEntityViews { get { return entityGenerationOption.HasFlag(DocumentGenerationOption.View); } }
		public bool GenerateDocumentManagerViewModel { get { return documentManagerGenerationOption.HasFlag(DocumentGenerationOption.ViewModel); } }
		public bool GenerateDocumentManagerView { get { return documentManagerGenerationOption.HasFlag(DocumentGenerationOption.View); } }
	}
	[Flags]
	public enum DocumentGenerationOption {
		None = 0,
		ViewModel = 1,
		View = 2,
		ViewModelAndView = ViewModel | View,
	}
}
