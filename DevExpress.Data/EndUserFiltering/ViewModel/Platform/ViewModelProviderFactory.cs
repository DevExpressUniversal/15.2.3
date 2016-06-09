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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using DevExpress.Utils.IoC;
	public class BaseEndUserFilteringViewModelProviderFactory : IServiceProvider {
		#region Services
		IntegrityContainer serviceContainer = new IntegrityContainer();
		public BaseEndUserFilteringViewModelProviderFactory() {
			serviceContainer.RegisterInstance(GetViewModelFactory());
			serviceContainer.RegisterInstance(GetValueBoxTypeResolver());
			serviceContainer.RegisterInstance(GetEndUserFilteringSettingsFactory());
			serviceContainer.RegisterInstance(GetEndUserFilteringMetricAttributesFactory());
			serviceContainer.RegisterInstance(GetMetricAttributesQueryFactory());
			serviceContainer.RegisterInstance(GetEndUserFilteringMetricViewModelFactory());
			serviceContainer.RegisterInstance(GetEndUserFilteringViewModelTypeBuilder());
			serviceContainer.RegisterInstance(GetViewModelBuilderResolver());
		}
		protected virtual IViewModelFactory GetViewModelFactory() {
			return DefaultViewModelFactory.Instance;
		}
		protected virtual IValueTypeResolver GetValueBoxTypeResolver() {
			return DefaultValueTypeResolver.Instance;
		}
		protected virtual IEndUserFilteringSettingsFactory GetEndUserFilteringSettingsFactory() {
			return DefaultEndUserFilteringSettingsFactory.Instance;
		}
		protected virtual IEndUserFilteringMetricAttributesFactory GetEndUserFilteringMetricAttributesFactory() {
			return DefaultEndUserFilteringMetricAttributesFactory.Instance;
		}
		protected virtual IMetricAttributesQueryFactory GetMetricAttributesQueryFactory() {
			return DefaultMetricAttributesQueryFactory.Instance;
		}
		protected virtual IEndUserFilteringMetricViewModelFactory GetEndUserFilteringMetricViewModelFactory() {
			return DefaultEndUserFilteringMetricViewModelFactory.Instance;
		}
		protected virtual IEndUserFilteringViewModelTypeBuilder GetEndUserFilteringViewModelTypeBuilder() {
			return DefaultEndUserFilteringViewModelTypeBuilder.Instance;
		}
		protected virtual IViewModelBuilderResolver GetViewModelBuilderResolver() {
			return DefaultViewModelBuilderResolver.Instance;
		}
		#endregion Services
		#region Public API
		public IEndUserFilteringViewModelProvider CreateViewModelProvider() {
			return CreateViewModelProvider(serviceContainer);
		}
		public TService GetService<TService>() {
			return serviceContainer.Resolve<TService>();
		}
		protected virtual IEndUserFilteringViewModelProvider CreateViewModelProvider(IServiceProvider serviceProvider) {
			return new EndUserFilteringViewModelProvider(serviceProvider);
		}
		#endregion
		#region IServiceProvider
		object IServiceProvider.GetService(Type serviceType) {
			return ((IServiceProvider)serviceContainer).GetService(serviceType);
		}
		#endregion
	}
}
