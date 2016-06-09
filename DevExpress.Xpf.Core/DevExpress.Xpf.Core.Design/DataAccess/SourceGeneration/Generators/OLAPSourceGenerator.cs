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
	using DataAccessPlatform = DevExpress.Design.DataAccess.Wpf;
	using DesignModel = Microsoft.Windows.Design.Model;
	using DesignTimeService = DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
	class OLAPSourceGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.PivotOlapDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IOLAPDataSourceSettingsModel olapSettingsModel = context.SettingsModel as IOLAPDataSourceSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Provider, olapSettingsModel.SelectedProvider);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Server, olapSettingsModel.Server);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Catalog, olapSettingsModel.SelectedCatalog);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Cube, olapSettingsModel.SelectedCube);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.QueryTimeout, olapSettingsModel.QueryTimeout);
			SetProperty(dataSourceItem, "LocaleIdentifier", GetLocaleIdentifier(olapSettingsModel.SelectedCulture));
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ConnectionTimeout, olapSettingsModel.ConnectionTimeout);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.UserId, olapSettingsModel.UserId);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Password, olapSettingsModel.Password);
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataSource();
		}
		protected override void SetCustomBindingProperties(IDataSourceGeneratorContext context) {
			base.SetCustomBindingProperties(context);
			IOLAPDataSourceSettingsModel olapSettingsModel = context.SettingsModel as IOLAPDataSourceSettingsModel;
			if(olapSettingsModel.DataProvider != null)
				context.SetCustomBindingProperty(context.OLAPDataProviderProperty, olapSettingsModel.DataProvider.ToString());
			var xamlGeneratorContext = context as DataAccessPlatform.IXAMLDataSourceGeneratorContext;
			var bindingItem = DesignTimeService.DesignTimeObjectModelCreateService.CreateBindingItem(
				((DesignModel.ModelItem)context.ModelItem.Value), "ConnectionString", xamlGeneratorContext.DataSourceResourceName);
			context.SetCustomBindingProperty(context.OLAPConnectionStringProperty, bindingItem);
		}
		static int GetLocaleIdentifier(System.Globalization.CultureInfo culture) {
			return (culture != null) ? culture.LCID : Xpf.Core.DataSources.PivotOlapDataSource.DefaultLCID;
		}
	}
}
