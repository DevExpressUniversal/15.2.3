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
	using DevExpress.Design.UI;
	using DataAccessPlatform = DevExpress.Design.DataAccess.Win;
	using ModelGenerationPlatform = DevExpress.Design.Win.ModelGeneration;
	public static class DataSourceWizard {
		static DataSourceWizard() {
			Platform.ServiceContainer.Register<IDataAccessTechnologyTypesProviderFactory, DataAccessPlatform.DataAccessTechnologyTypesProviderFactory>(PlatformCodeName.Win);
			Platform.ServiceContainer.Register<IDataAccessTechnologyComponentsProviderFactory, DataAccessPlatform.DataAccessTechnologyComponentsProviderFactory>(PlatformCodeName.Win);
			Platform.ServiceContainer.Register<IModelGenerationServiceContainer, ModelGenerationPlatform.ModelGenerationServiceContainer>(PlatformCodeName.Win);
			Platform.ServiceContainer.Register<IDataSourceSettingsBuilderContextFactory, DataAccessPlatform.DataSourceSettingsBuilderContextFactory>(PlatformCodeName.Win);
			Platform.ServiceContainer.Register<IDataSourceGeneratorContextFactory, DataAccessPlatform.DataSourceGeneratorContextFactory>(PlatformCodeName.Win);
			Platform.ServiceContainer.Register<IDataSourceGeneratorFactory, DataAccessPlatform.DataSourceGeneratorFactory>(PlatformCodeName.Win);
		}
		public static bool? Run(System.IServiceProvider serviceProvider, object component) {
			return DataAccessConfiguratorWindow.Show(serviceProvider, component);
		}
	}
}
