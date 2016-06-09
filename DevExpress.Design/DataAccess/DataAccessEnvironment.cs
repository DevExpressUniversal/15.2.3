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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	public static class DataAccessEnvironment {
		public static DevExpress.Design.UI.IServiceContainer ServiceContainer {
			get { return DataAccessInfoServiceContainer.Instance; }
		}
		public static IDataAccessTechnologyInfo GetInfo(DataAccessTechnologyCodeName technologyCodeName, IDataAccessTechnologyTypesProvider typesProvider = null) {
			var infoFactory = ServiceContainer.Resolve<IDataAccessTechnologyInfoFactory>();
			var actualTypesProvider = typesProvider ?? ServiceContainer.Resolve<IDataAccessTechnologyTypesProvider>();
			return infoFactory.GetInfo(technologyCodeName, actualTypesProvider);
		}
		public static IEnumerable<IDataAccessTechnologyInfo> GetInfos() {
			return GetInfosCore(AllTechnologyCodeNames, null);
		}
		public static IEnumerable<IDataAccessTechnologyInfo> GetInfos(object component) {
			AssertionException.IsNotNull(component);
			return GetInfos(component.GetType(), null);
		}
		public static IEnumerable<IDataAccessTechnologyInfo> GetInfos(System.Type componentType, IDataAccessTechnologyTypesProvider typesProvider = null) {
			IDataAccessMetadata metadata = DataAccessMetadataLoader.Load(componentType);
			return GetInfosCore(metadata.SupportedTechnologies, typesProvider);
		}
		public static IEnumerable<IDataAccessTechnologyInfo> GetInfos(IDataAccessMetadata metadata, IDataAccessTechnologyTypesProvider typesProvider = null) {
			return GetInfosCore(metadata.SupportedTechnologies, typesProvider);
		}
		static IEnumerable<IDataAccessTechnologyInfo> GetInfosCore(IEnumerable<DataAccessTechnologyCodeName> technologyCodeNames, IDataAccessTechnologyTypesProvider typesProvider = null) {
			var infoFactory = ServiceContainer.Resolve<IDataAccessTechnologyInfoFactory>();
			var actualTypesProvider = typesProvider ?? ServiceContainer.Resolve<IDataAccessTechnologyTypesProvider>();
			foreach(DataAccessTechnologyCodeName codeName in technologyCodeNames)
				yield return infoFactory.GetInfo(codeName, typesProvider);
		}
		static readonly DataAccessTechnologyCodeName[] AllTechnologyCodeNames;
		static DataAccessEnvironment() {
			AllTechnologyCodeNames = (DataAccessTechnologyCodeName[])System.Enum.GetValues(typeof(DataAccessTechnologyCodeName));
		}
		#region ServiceContainer
		class DataAccessInfoServiceContainer : DevExpress.Design.UI.ServiceContainer {
			DataAccessInfoServiceContainer()
				: base(DevExpress.Design.UI.Platform.ServiceContainer) {
				Register<IDataAccessTechnologyInfoFactory>(() => new DefaultDataAccessTechnologyInfoFactory(this));
				Register<IDataAccessTechnologyTypesProvider, DefaultDataAccessTechnologyLocalTypesProvider>();
			}
			public static DevExpress.Design.UI.IServiceContainer Instance = new DataAccessInfoServiceContainer();
		}
		#endregion ServiceContainer
	}
}
namespace DevExpress.Design.DataAccess.UI {
	using System.Collections.Generic;
	using DevExpress.Design.UI;
	class DataAccessConfigurationService : IDataAccessConfigurationService {
		IEnumerable<IDataAccessTechnologyInfo> IDataAccessConfigurationService.InitTechnologyInfos(IServiceContainer serviceContainer, IDataAccessConfiguratorContext context) {
			List<IDataAccessTechnologyInfo> list = new List<IDataAccessTechnologyInfo>();
			var infoFactory = serviceContainer.Resolve<IDataAccessTechnologyInfoFactory>();
			var typesProvider = serviceContainer.Resolve<IDataAccessTechnologyTypesProvider>();
			foreach(var codeName in context.Metadata.SupportedTechnologies)
				list.Add(infoFactory.GetInfo(codeName, typesProvider));
			return list;
		}
		IEnumerable<IDataProcessingMode> IDataAccessConfigurationService.InitProcessingModes(IServiceContainer serviceContainer, IDataAccessConfiguratorContext configuratorContext) {
			if(configuratorContext.TechnologyName == null)
				return DataProcessingModesInfo.EmptyModes;
			var infoFactory = serviceContainer.Resolve<IDataProcessingModesInfoFactory>();
			var technologyCodeName = configuratorContext.TechnologyName.GetCodeName();
			IEnumerable<DataProcessingModeCodeName> supportedModes = configuratorContext.Metadata.SupportedProcessingModes;
			DataProcessingModeCodeName excludeMode;
			if(configuratorContext.Metadata.ExcludeProcessingModes.TryGetValue(technologyCodeName, out excludeMode)) {
				supportedModes = System.Linq.Enumerable.Except(
					configuratorContext.Metadata.SupportedProcessingModes,
					new DataProcessingModeCodeName[] { excludeMode });
			}
			return infoFactory.GetInfo(technologyCodeName, supportedModes).Modes;
		}
		IDataSourceSettings IDataAccessConfigurationService.InitSettings(IServiceContainer serviceContainer, IDataAccessConfiguratorContext configuratorContext) {
			PlatformCodeName platform = configuratorContext.Metadata.Platform;
			IDataAccessTechnologyName technologyName = configuratorContext.TechnologyName;
			if(technologyName != null) {
				IDataSourceSettingsBuilderContextFactory contextFactory =
					Platform.ServiceContainer.Resolve<IDataSourceSettingsBuilderContextFactory>(configuratorContext.Metadata.Platform);
				IDataSourceInfoFactory infoFactory = serviceContainer.Resolve<IDataSourceInfoFactory>();
				IDataSourceSettingsFactory settingsFactory = serviceContainer.Resolve<IDataSourceSettingsFactory>();
				var info = infoFactory.GetInfo(technologyName, configuratorContext.TechnologyItem);
				var settingsBuilderContext = contextFactory.GetContext(technologyName, configuratorContext.ProcessingMode, info);
				settingsBuilderContext.Metadata = configuratorContext.Metadata;
				configuratorContext.SettingsModel = settingsBuilderContext.Model;
				return settingsFactory.GetSettings(technologyName, settingsBuilderContext);
			}
			else return DataSourceSettings.Empty;
		}
		void IDataAccessConfigurationService.Configure(object dataSourceComponent, IServiceContainer serviceContainer, IDataAccessConfiguratorContext configuratorContext) {
			if(dataSourceComponent == null || configuratorContext == null) return;
			configuratorContext.TechnologyItem = new ComponentDataAccessTechnologyInfoItem(dataSourceComponent, dataSourceComponent.GetType());
			var configurationService = serviceContainer.Resolve<IDataAccessConfigurationService>();
			var modes = configurationService.InitProcessingModes(serviceContainer, configuratorContext);
			configuratorContext.ProcessingMode = System.Linq.Enumerable.FirstOrDefault(modes);
			configurationService.InitSettings(serviceContainer, configuratorContext);
			if(configuratorContext.SettingsModel != null && string.IsNullOrEmpty(configuratorContext.SettingsModel.Error)) {
				configuratorContext.SettingsModel.ShowCodeBehind = false;
				var serviceProvider = serviceContainer.Resolve<System.IServiceProvider>();
				if(serviceProvider != null)
					configurationService.Configure(serviceProvider, configuratorContext);
			}
		}
		void IDataAccessConfigurationService.Configure(System.IServiceProvider serviceProvider, IDataAccessConfiguratorContext configuratorContext) {
			PlatformCodeName platform = configuratorContext.Metadata.Platform;
			IDataSourceGeneratorFactory generatorFactory = Platform.ServiceContainer.Resolve<IDataSourceGeneratorFactory>(platform);
			using(IModelGenerationServiceContainer container = Platform.ServiceContainer.Resolve<IModelGenerationServiceContainer>(platform)) {
				container.Initialize(serviceProvider);
				IModelService modelService = container.GetService<IModelService>();
				IModelItem modelItem = modelService.CreateModelItem(configuratorContext.Component);
				var generatorContextFactory = Platform.ServiceContainer.Resolve<IDataSourceGeneratorContextFactory>(platform);
				var generatorContext = generatorContextFactory.GetContext(modelItem, configuratorContext.SettingsModel, configuratorContext.Metadata);
				var generator = generatorFactory.GetGenerator(configuratorContext.TechnologyName, configuratorContext.SettingsModel);
				generator.Generate(generatorContext);
			}
		}
	}
}
