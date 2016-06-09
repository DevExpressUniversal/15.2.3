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

namespace DevExpress.Design.DataAccess.UI {
	using System.Collections.Generic;
	using DevExpress.Design.UI;
	class DataAccessConfiguratorContext : IDataAccessConfiguratorContext {
		public DataAccessConfiguratorContext(IDataAccessMetadata metadata) {
			this.Metadata = metadata;
		}
		public IDataAccessMetadata Metadata { get; private set; }
		public IDataAccessTechnologyName TechnologyName { get; set; }
		public IDataAccessTechnologyInfoItem TechnologyItem { get; set; }
		public IDataProcessingMode ProcessingMode { get; set; }
		public IDataSourceSettingsModel SettingsModel { get; set; }
		public object Component { get; set; }
	}
	class DataAccessConfiguratorWindowViewModel : DXDesignWindowViewModel<IDataAccessConfiguratorViewModel> {
		IDataAccessConfiguratorViewModel contentViewModelCore;
		DataAccessLocalizerStringId titleId;
		public DataAccessConfiguratorWindowViewModel(System.IServiceProvider externalProvider, IDXDesignWindow window, IDataAccessConfiguratorContext context)
			: base(window) {
			InitServices(externalProvider, context.Metadata.Platform);
			this.contentViewModelCore = new DataAccessConfiguratorViewModel(this, context);
			this.titleId = DataAccessLocalizerStringId.DataSourceConfiguratorTitle;
			if(context.Metadata.Platform == PlatformCodeName.Wpf || context.Metadata.Platform == PlatformCodeName.Silverlight)
				this.titleId = DataAccessLocalizerStringId.DataSourceConfiguratorTitleXAML;
		}
		public override string Title {
			get { return DataAccess.DataAccessLocalizer.GetString(titleId); }
		}
		public override IDataAccessConfiguratorViewModel ContentViewModel {
			get { return contentViewModelCore; }
		}
		void InitServices(System.IServiceProvider externalProvider, PlatformCodeName platformId) {
			ServiceContainer.Register<System.IServiceProvider>(
				() => externalProvider);
			ServiceContainer.Register<IDataAccessTechnologyTypesProvider>(
				() => Platform.ServiceContainer.Resolve<IDataAccessTechnologyTypesProviderFactory>(platformId).Create(externalProvider));
			ServiceContainer.Register<IDataAccessTechnologyComponentsProvider>(
				() => Platform.ServiceContainer.Resolve<IDataAccessTechnologyComponentsProviderFactory>(platformId).Create(externalProvider));
		}
	}
	class DataAccessConfiguratorViewModel : StepByStepConfiguratorViewModel<IDataAccessConfiguratorPageViewModel, IDataAccessConfiguratorContext>,
		IDataAccessConfiguratorViewModel {
		public DataAccessConfiguratorViewModel(IViewModelBase parentViewModel, IDataAccessConfiguratorContext context)
			: base(parentViewModel, context) {
		}
		protected override IEnumerable<IDataAccessConfiguratorPageViewModel> GetPages() {
			return new IDataAccessConfiguratorPageViewModel[] { 
				new DataAccessTechnologyConfiguratorViewModel(this, Context),
				new DataProcessingModeConfiguratorViewModel(this, Context),
				new DataSourceSettingsConfiguratorViewModel(this, Context),
			};
		}
		protected override void InitServices() {
			ServiceContainer.Register<IDataAccessTechnologyInfoFactory>(
				() => new DefaultDataAccessTechnologyInfoFactory(ServiceContainer));
			ServiceContainer.Register<IDataAccessTechnologyNewItemService>(
				() => new DefaultDataAccessTechnologyNewItemFactory(ServiceContainer, Context));
			ServiceContainer.Register<IDataProcessingModesInfoFactory, DefaultDataProcessingModesInfoFactory>();
			ServiceContainer.Register<IDataSourceInfoFactory, DefaultDataSourceInfoFactory>();
			ServiceContainer.Register<IDataSourceSettingsFactory, DefaultDataSourceSettingsFactory>();
		}
	}
}
