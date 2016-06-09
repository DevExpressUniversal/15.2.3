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

namespace DevExpress.Design.Filtering.UI {
	using System;
	using System.Collections.Generic;
	using DevExpress.Design.DataAccess;
	using DevExpress.Design.UI;
	using Utils.Filtering.Internal;
	class FilteringModelConfiguratorContext : IFilteringModelConfiguratorContext {
		#region static
		static IEndUserFilteringSettingsFactory settingsFactoryCore;
		static IEndUserFilteringMetricAttributesFactory metricAttributesFactoryCore;
		static FilteringModelConfiguratorContext() {
			var providerFactory = new BaseEndUserFilteringViewModelProviderFactory();
			settingsFactoryCore = providerFactory.GetService<IEndUserFilteringSettingsFactory>();
			metricAttributesFactoryCore = providerFactory.GetService<IEndUserFilteringMetricAttributesFactory>();
		}
		#endregion static
		public FilteringModelConfiguratorContext(IFilteringModelMetadata metadata) {
			this.Metadata = metadata;
		}
		public IFilteringModelMetadata Metadata { get; private set; }
		public Type ModelType { get; set; }
		public IEnumerable<IEndUserFilteringMetricAttributes> CustomAttributes { get; set; }
		public IEndUserFilteringMetric SelectedMetric { get; set; }
		public object Component { get; set; }
		public IEndUserFilteringSettingsFactory SettingsFactory {
			get { return settingsFactoryCore; }
		}
		public IEndUserFilteringMetricAttributesFactory MetricAttributesFactory {
			get { return metricAttributesFactoryCore; }
		}
	}
	class FilteringModelConfiguratorWindowViewModel : DXDesignWindowViewModel<IFilteringModelConfiguratorViewModel> {
		IFilteringModelConfiguratorViewModel contentViewModelCore;
		FilteringModelConfiguratorLocalizerStringId titleId;
		public FilteringModelConfiguratorWindowViewModel(System.IServiceProvider externalProvider, IDXDesignWindow window, IFilteringModelConfiguratorContext context)
			: base(window) {
			InitServices(externalProvider, context.Metadata.Platform);
			this.contentViewModelCore = new FilteringModelConfiguratorViewModel(this, context);
			this.titleId = FilteringModelConfiguratorLocalizerStringId.ConfiguratorTitle;
			if(context.Metadata.Platform == PlatformCodeName.Wpf || context.Metadata.Platform == PlatformCodeName.Silverlight)
				this.titleId = FilteringModelConfiguratorLocalizerStringId.ConfiguratorTitleXAML;
		}
		public override string Title {
			get { return Filtering.FilteringModelConfiguratorLocalizer.GetString(titleId); }
		}
		public override IFilteringModelConfiguratorViewModel ContentViewModel {
			get { return contentViewModelCore; }
		}
		void InitServices(System.IServiceProvider externalProvider, PlatformCodeName platformId) {
			ServiceContainer.Register<System.IServiceProvider>(
				() => externalProvider);
			ServiceContainer.Register<IDataAccessTechnologyTypesProvider>(
				() => Platform.ServiceContainer.Resolve<IDataAccessTechnologyTypesProviderFactory>(platformId).Create(externalProvider));
		}
	}
	class FilteringModelConfiguratorViewModel : StepByStepConfiguratorViewModel<IFilteringModelConfiguratorPageViewModel, IFilteringModelConfiguratorContext>,
		IFilteringModelConfiguratorViewModel {
		public FilteringModelConfiguratorViewModel(IViewModelBase parentViewModel, IFilteringModelConfiguratorContext context)
			: base(parentViewModel, context) {
		}
		protected override IEnumerable<IFilteringModelConfiguratorPageViewModel> GetPages() {
			return new IFilteringModelConfiguratorPageViewModel[] { 
				new MetricAttributesSettingsPageViewModel(this, Context),
				new ModelTypeSettingsPageViewModel(this, Context),
			};
		}
		protected override bool CanSelectPrevPage(IFilteringModelConfiguratorPageViewModel selectedPage) {
			if(selectedPage is ModelTypeSettingsPageViewModel)
				return base.CanSelectPrevPage(selectedPage) && selectedPage.IsCompleted;
			return base.CanSelectPrevPage(selectedPage);
		}
		protected override int GetStartPageIndex() { 
			return 1; 
		}
	}
}
