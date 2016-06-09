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
	using DevExpress.Design.Filtering.Services;
	using DevExpress.Design.UI;
	class FilteringModelConfigurationService : IFilteringModelConfigurationService {
		void IFilteringModelConfigurationService.Configure(object dataSourceComponent, IServiceContainer serviceContainer, IFilteringModelConfiguratorContext configuratorContext) {
			if(dataSourceComponent == null || configuratorContext == null) return;
			var configurationService = serviceContainer.Resolve<IFilteringModelConfigurationService>();
			var serviceProvider = serviceContainer.Resolve<System.IServiceProvider>();
			if(serviceProvider != null)
				configurationService.Configure(serviceProvider, configuratorContext);
		}
		void IFilteringModelConfigurationService.Configure(System.IServiceProvider serviceProvider, IFilteringModelConfiguratorContext configuratorContext) {
			PlatformCodeName platform = configuratorContext.Metadata.Platform;
			var modelSerializer = Platform.ServiceContainer.Resolve<IFilteringModelPropertiesSerializer>(platform);
			if(modelSerializer != null)
				modelSerializer.Serialize(serviceProvider, configuratorContext);
		}
	}
}
