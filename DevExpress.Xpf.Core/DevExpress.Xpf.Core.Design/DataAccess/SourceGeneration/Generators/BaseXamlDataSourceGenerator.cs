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

using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using System;
namespace DevExpress.Design.DataAccess {
	abstract class BaseXamlDataSourceGenerator : DataSourceGeneratorBase {
		protected sealed override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			SourceType = context.SettingsModel.SourceType;
			IModelItem dataSourceItem = context.CreateDataSource(GetDataSourceType());
			if(context.SettingsModel.IsDesignDataAllowed && context.SettingsModel.ShowDesignData) {
				var designDataModelItem = GetDesignDataItem(dataSourceItem, SourceType);
				IModelService modelService = context.ModelItem.EditingContext.ServiceProvider.GetService<IModelService>();
				if(modelService != null) 
					InitializeDesignData(modelService.CreateModelItem(designDataModelItem), context);
			}
			return dataSourceItem;
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.SetDataSource(dataSourceItem);
		}
		static Microsoft.Windows.Design.Model.ModelItem GetDesignDataItem(IModelItem dataSourceItem, Type sourceType) {
			var designDataPropertyID = new Microsoft.Windows.Design.Metadata.PropertyIdentifier(
				typeof(DevExpress.Xpf.Core.DataSources.DesignDataManager), DesignDataProperty);
			var dataSourceModelItem = dataSourceItem.Value as Microsoft.Windows.Design.Model.ModelItem;
			return dataSourceModelItem.Properties[designDataPropertyID].Value;
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
		}
		protected virtual void InitializeDesignData(IModelItem designDataItem, IDataSourceGeneratorContext context) {
			SetProperty(designDataItem, RowCountProperty, context.SettingsModel.DesignDataRowCount);
		}
		protected Type SourceType { get; private set; }
		protected abstract System.Type GetDataSourceType();
		protected const string ContextTypeProperty = "ContextType";
		protected const string PathProperty = "Path";
		protected const string DesignDataProperty = "DesignData";
		protected const string DataObjectTypeProperty = "DataObjectType";
		protected const string EnumTypeProperty = "EnumType";
		protected const string AdapterTypeProperty = "AdapterType";
		protected const string RowCountProperty = "RowCount";
		protected void SetContextType(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(dataSourceItem, ContextTypeProperty, GetSourceType(context));
		}
		protected void SetPath(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(dataSourceItem, PathProperty, GetElementName(context));
		}
	}
}
