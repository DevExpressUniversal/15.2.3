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

namespace DevExpress.Xpf.Core.Design.DataAccess.UI {
	using DevExpress.Design;
	using DevExpress.Design.DataAccess;
	using DevExpress.Design.DataAccess.UI;
	using DevExpress.Design.UI;
	using DataAccessPlatform = DevExpress.Design.DataAccess.Wpf;
	using ModelGenerationPlatform = DevExpress.Design.Wpf.ModelGeneration;
	public static class ItemsSourceWizard {
		static ItemsSourceWizard() {
			Platform.ServiceContainer.Register<IDataAccessTechnologyTypesProviderFactory, DataAccessPlatform.DataAccessTechnologyTypesProviderFactory>(PlatformCodeName.Wpf);
			Platform.ServiceContainer.Register<IModelGenerationServiceContainer, ModelGenerationPlatform.ModelGenerationServiceContainer>(PlatformCodeName.Wpf);
			Platform.ServiceContainer.Register<IDataSourceSettingsBuilderContextFactory, DataAccessPlatform.DataSourceSettingsBuilderContextFactory>(PlatformCodeName.Wpf);
			Platform.ServiceContainer.Register<IDataSourceGeneratorContextFactory, DataAccessPlatform.DataSourceGeneratorContextFactory>(PlatformCodeName.Wpf);
			Platform.ServiceContainer.Register<IDataSourceGeneratorFactory, DataAccessPlatform.DataSourceGeneratorFactory>(PlatformCodeName.Wpf);
		}
		public static bool? Run(Microsoft.Windows.Design.Model.ModelItem modelItem) {
			DataAccessPlatform.EditingContextTypesProvider.QueueTypesDiscovering(modelItem.Context);
			try {
				using(new SmartTagVisibilityContext(modelItem)) {
					return DataAccessConfiguratorWindow.Show(modelItem);
				}
			}
			finally { DataAccessPlatform.EditingContextTypesProvider.Release(); }
		}
		class SmartTagVisibilityContext : System.IDisposable {
			SmartTagDesignService service;
			public SmartTagVisibilityContext(Microsoft.Windows.Design.Model.ModelItem modelItem) {
				service = modelItem.Context.Services.GetService<SmartTagDesignService>();
				if(service != null) {
					service.IsSmartTagButtonPressed = false;
					service.IsSmartTagButtonVisible = false;
				}
			}
			void System.IDisposable.Dispose() {
				if(service != null)
					service.IsSmartTagButtonVisible = true;
				service = null;
			}
		}
	}
}
namespace DevExpress.Design.SmartTags {
	using DevExpress.Design.DataAccess;
	using DevExpress.Xpf.Core.Design;
	using DesignModel = Microsoft.Windows.Design.Model;
	public class CreateItemsSourceActionProvider : CommandActionLineProvider {
		protected DesignModel.ModelItem ModelItem { get; private set; }
		public CreateItemsSourceActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
			ModelItem = XpfModelItem.ToModelItem(Context.ModelItem);
		}
		protected override string GetCommandText() {
			return DataAccessLocalizer.GetString(DataAccessLocalizerStringId.DataSourceSmartTagActionNameXAML);
		}
		protected override void OnCommandExecute(object param) {
			Xpf.Core.Design.DataAccess.UI.ItemsSourceWizard.Run(ModelItem);
		}
	}
}
