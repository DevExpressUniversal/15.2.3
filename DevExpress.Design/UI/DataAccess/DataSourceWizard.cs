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
	using System.ComponentModel;
	using DevExpress.Design.UI;
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DataAccessConfiguratorWindow {
		static DataAccessConfiguratorWindow() {
			Platform.ServiceContainer.Register<IDataAccessConfigurationService, DataAccessConfigurationService>();
		}
		public static bool? Show(Microsoft.Windows.Design.Model.ModelItem modelItem) {
			AssertionException.IsNotNull(modelItem);
			Platform.QueryDTEInfo();
			try {
				return ShowCore(modelItem.Context.Services, modelItem, GetDataAccessContext(modelItem.GetCurrentValue(), modelItem));
			}
			finally { Platform.ReleaseDTEInfo(); }
		}
		public static bool? Show(System.IServiceProvider serviceProvider, object component) {
			AssertionException.IsNotNull(component);
			AssertionException.IsNotNull(serviceProvider);
			Platform.QueryDTEInfo();
			try {
				DevExpress.Design.Metadata.AvailableTypes.assemblyCache = null;
				return ShowCore(serviceProvider, component, GetDataAccessContext(component, component));
			}
			finally { Platform.ReleaseDTEInfo(); }
		}
		static bool? ShowCore(System.IServiceProvider serviceProvider, object component, IDataAccessConfiguratorContext configuratorContext) {
			var wizardWindow = new Design.UI.DXDesignWindow();
			var windowViewModel = new DataAccessConfiguratorWindowViewModel(serviceProvider, wizardWindow, configuratorContext);
			wizardWindow.DataContext = windowViewModel;
			wizardWindow.Content = new DataAccessConfigurator() { DataContext = windowViewModel.ContentViewModel };
			bool? result = Design.UI.DXDesignWindow.ShowModal(wizardWindow, Platform.IsVS2013OrAbove);
			if(result.GetValueOrDefault()) {
				var configurationService = Platform.ServiceContainer.Resolve<IDataAccessConfigurationService>();
				configurationService.Configure(serviceProvider, configuratorContext);
			}
			return result;
		}
		static IDataAccessConfiguratorContext GetDataAccessContext(object metadataProvider, object component) {
			IDataAccessMetadata metadata = DataAccessMetadataLoader.Load(metadataProvider.GetType());
			return new DataAccessConfiguratorContext(metadata) { Component = component };
		}
	}
}
