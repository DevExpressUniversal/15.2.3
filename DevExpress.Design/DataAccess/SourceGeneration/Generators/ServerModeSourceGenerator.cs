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
	abstract class BaseServerModeSourceGenerator : DataSourceGeneratorBase {
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IServerModeSettingsModel serverModeSettingsModel = context.SettingsModel as IServerModeSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.KeyExpression, serverModeSettingsModel.KeyExpression);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.DefaultSorting, serverModeSettingsModel.DefaultSorting);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ElementType, GetTableRowType(context));
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IModelItemExpression querableSourceExpression = GenerateQueryableSourceBindingExpression(context);
			context.GenerateParameterAssignment(dataSourceItem, GetQuerableSourceParameterName(), querableSourceExpression);
			return true;
		}
		protected abstract IModelItemExpression GenerateQueryableSourceBindingExpression(IDataSourceGeneratorContext context);
		protected abstract string GetQuerableSourceParameterName();
	}
	class EntityFrameworkServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.Linq.EntityServerModeSource));
		}
		protected override IModelItemExpression GenerateQueryableSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(new object[] { GetElementName(context) }, "{0}.{1}");
		}
		protected override string GetQuerableSourceParameterName() {
			return "QueryableSource";
		}
	}
	class LinqToSqlServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.Linq.LinqServerModeSource));
		}
		protected override IModelItemExpression GenerateQueryableSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(new object[] { GetElementName(context) }, "{0}.{1}");
		}
		protected override string GetQuerableSourceParameterName() {
			return "QueryableSource";
		}
	}
	class WcfServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.WcfLinq.WcfServerModeSource));
		}
		protected override IModelItemExpression GenerateQueryableSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetServiceUri(context), GetElementName(context) }, "{0}(new System.Uri(\"{1}\")).{2}");
		}
		protected override string GetQuerableSourceParameterName() {
			return "Query";
		}
	}
	class XPOServerModeSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override string GetDataSourceObjectTypeName() {
			return "XPServerCollectionSource";
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(xpoSourceItem, context);
			var settingsModel = context.SettingsModel as IXPServerCollectionSourceSettingsModel;
			SetProperty(xpoSourceItem, DataSourcePropertyCodeName.DefaultSorting, settingsModel.DefaultSorting);
		}
	}
}
